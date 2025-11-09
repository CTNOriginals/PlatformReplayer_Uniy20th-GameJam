using System.Collections;
using System.Collections.Generic;
using CTNOriginals.PlatformReplayer.Attributes.Composite;
using CTNOriginals.PlatformReplayer.Player;
using CTNOriginals.PlatformReplayer.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CTNOriginals.PlatformReplayer.Managers {
	public class RecorderManager : Singleton<RecorderManager> {
		[System.Serializable]
		public class CPlayerRecording {
			public Color Color;
			public List<Vector2> Positions = new List<Vector2>();
		}

		public enum EState {
			Recording,
			Replaying,
		}

		[ConfigGroup, SerializeField]
		private List<Color> replayColors;

		[Space]
		
		[ConfigGroup, InlineProperty, SerializeField]
		[Tooltip("The amoutn of time between each frame while rewinding.\nThe value is lerped between the min and max, where max will be the value at the very last frame")]
		private MinMax<float> rewindTime;
		[ConfigGroup, SerializeField]
		private AnimationCurve rewindTimeCurve;

		[Space]

		[ConfigGroup, InlineProperty, SerializeField]
		[Tooltip("The time factor will be halved and the curve will be evaluated twice, one for down, the next for reverse up into negative pitch.")]
		private SCurveTime audioReverseCurve;
		[ConfigGroup, Range(0, 0.5f), SerializeField]
		[Tooltip(@"This will dictate how detauled the audio reverse is.
			if this value is 0.1f and the time factor of the curve 1.0f, the pitch will be adjusted 10 times.
			In other words, this value is how many seconds each step takes.
		")]
		private float audioReverseTimeStep;
		[ConfigGroup, Range(0, -3), SerializeField]
		private float audioMaxReversePitch;

		[Range(0, 100)] public int D_index;
		[Range(-1, 1)] public int D_dir;
		[ReadOnly] public float D_timeSteps;
		[ReadOnly] public float D_steps;
		[ReadOnly] public float D_prog;
		[ReadOnly] public float D_pitch;

		[RuntimeGroup]
		public List<CPlayerRecording> Recordings = new List<CPlayerRecording>();
		public CPlayerRecording Current => this.Recordings[this.Recordings.Count - 1];
		[RuntimeGroup]
		public List<Replayer> Replayers;

		[RuntimeGroup]
		public EState State;

		PlayerController player => ReferenceManager.Instance.PlayerController;
		ReferenceManager.EGameState gameState => ReferenceManager.Instance.GameState;
		AudioSource audioSource => ReferenceManager.Instance.AudioSource;

		private void Start() {
			this.NewRecording();
			ReferenceManager.Instance.GameState = ReferenceManager.EGameState.Playing;
		}

		private void FixedUpdate() {
			if (this.gameState != ReferenceManager.EGameState.Playing) {
				return;
			}

			this.Current.Positions.Add(this.player.transform.position);
		}

		private void Update() {
			if (gameState != ReferenceManager.EGameState.Playing) {
				return;
			}

			if (Input.GetKeyDown(KeyCode.R)) {
				this.DoRewind();
			}
		}

		private void NewRecording() {
			CPlayerRecording rec = new CPlayerRecording() {
				Color = this.replayColors[this.Recordings.Count % replayColors.Count]
			};

			this.Recordings.Add(rec);
		}

		public void DoRewind() {
			ReferenceManager.Instance.GameState = ReferenceManager.EGameState.Rewinding;

			GameObject newReplayer = Instantiate(
				original: ReferenceManager.Instance.PlayerRecordingPrefab,
				position: this.Current.Positions[this.Current.Positions.Count - 1],
				rotation: ReferenceManager.Instance.PlayerRecordingPrefab.transform.rotation,
				parent: ReferenceManager.Instance.ReplayerHolder
			);

			Replayer replayer = newReplayer.GetComponent<Replayer>();

			replayer.Recording = this.Current;
			replayer.Initialize();
			this.Replayers.Add(replayer);

			this.player.gameObject.SetActive(false);

			StartCoroutine(this.Rewind());
		}

		private float GetPitch(int step, int dir) {
			// if (!Application.isPlaying) {
			// 	dir = D_dir;
			// }


			D_steps = this.audioReverseTimeStep * step;
			D_prog = (this.audioReverseCurve.TimeFactor / 100) * D_steps * 100;
			D_pitch = (this.audioReverseCurve.GetValue(D_prog) * dir);

			// D_pitch = (D_pitch > 0) ? D_pitch : D_pitch * Time.fixedDeltaTime / rewindTime.Min;

			return D_pitch;
		}

		private IEnumerator ReverseAudio() {
			int timeSteps = Mathf.RoundToInt(this.audioReverseCurve.TimeFactor / this.audioReverseTimeStep);
			D_timeSteps = timeSteps;

			WaitForSeconds timeStepWait = new WaitForSeconds(this.audioReverseTimeStep);
			int dir = (this.audioSource.pitch > 0) ? -1 : 1;

			for (int i = 0; i < timeSteps; i++) {
				this.audioSource.pitch = this.GetPitch(i, dir * -1);
				yield return timeStepWait;
			}

			// this.audioSource.pitch = ((D_pitch > 0) ? 1 : Time.fixedDeltaTime / rewindTime.Min) * dir;
			this.audioSource.pitch = dir;
		}

		private IEnumerator Rewind() {
			yield return StartCoroutine(this.ReverseAudio());

			for (int i = this.Current.Positions.Count; i >= 0; i--) {
				foreach (Replayer replayer in this.Replayers) {
					if (i >= replayer.Recording.Positions.Count) {
						continue;
					}

					replayer.Index = i;
					replayer.DoMove();
				}

				float progress = 1 - ((float)i / this.Current.Positions.Count);
				float progressCurve = rewindTimeCurve.Evaluate(progress);
				float waitTime = Mathf.Lerp(rewindTime.Min, rewindTime.Max, progressCurve);

				TimeManager.Instance.SetTimerPercentage(progress);

				this.audioSource.pitch = Mathf.Lerp(D_pitch, audioMaxReversePitch, progressCurve);

				yield return new WaitForSeconds(waitTime);
			}

			yield return StartCoroutine(this.ReverseAudio());

			this.audioSource.Stop();
			this.audioSource.Play();

			this.Replay();
		}

		private void Replay() {
			foreach (Replayer rep in this.Replayers) {
				rep.StartReplayer();
			}

			Vector2 startPos = ReferenceManager.Instance.PlayerStartPosition;
			this.player.transform.position = new Vector2(
				startPos.x
				+ (this.player.transform.localScale.x * 2)
				* (int)(this.Recordings.Count / this.replayColors.Count),
				startPos.y
				+ (this.player.transform.localScale.y * 2)
				* (this.Recordings.Count - ((int)(this.Recordings.Count / this.replayColors.Count) * this.replayColors.Count))
			);

			this.NewRecording();

			this.player.gameObject.SetActive(true);

			TimeManager.Instance.ResetTimer();
			ReferenceManager.Instance.GameState = ReferenceManager.EGameState.Playing;
		}
		
		private void OnValidate() {
			this.GetPitch(D_index, D_dir);
		}
	}
	
}
