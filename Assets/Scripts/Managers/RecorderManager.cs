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
		
		[ConfigGroup, InlineProperty, SerializeField]
		[Tooltip("The amoutn of time between each frame while rewinding.\nThe value is lerped between the min and max, where max will be the value at the very last frame")]
		private MinMax<float> rewindTime;
		[ConfigGroup, SerializeField]
		private AnimationCurve rewindTimeCurve;

		[RuntimeGroup]
		public List<CPlayerRecording> Recordings = new List<CPlayerRecording>();
		public CPlayerRecording Current => this.Recordings[this.Recordings.Count - 1];

		public List<Replayer> Replayers;

		[RuntimeGroup]
		public EState State;

		PlayerController player => ReferenceManager.Instance.PlayerController;
		ReferenceManager.EGameState gameState => ReferenceManager.Instance.GameState;

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
			if (Input.GetKeyDown(KeyCode.R)) {
				StartCoroutine(this.Rewind());
			}
		}

		private void NewRecording() {
			CPlayerRecording rec = new CPlayerRecording() {
				Color = this.replayColors[this.Recordings.Count % replayColors.Count]
			};

			this.Recordings.Add(rec);
		}

		private Replayer InitializeRewind() {
			ReferenceManager.Instance.GameState = ReferenceManager.EGameState.Rewinding;

			GameObject newReplayer = Instantiate(
				original: ReferenceManager.Instance.PlayerRecordingPrefab,
				position: this.Current.Positions[0],
				rotation: ReferenceManager.Instance.PlayerRecordingPrefab.transform.rotation,
				parent: ReferenceManager.Instance.ReplayerHolder
			);

			Replayer replayer = newReplayer.GetComponent<Replayer>();

			replayer.Recording = this.Current;
			replayer.Initialize();
			this.Replayers.Add(replayer);

			this.player.gameObject.SetActive(false);

			return replayer;
		}

		public IEnumerator Rewind() {
			this.InitializeRewind();

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

				yield return new WaitForSeconds(waitTime);
			}

			this.Replay();
			yield return null;
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
	}
	
}
