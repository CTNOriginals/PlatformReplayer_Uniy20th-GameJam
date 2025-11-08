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
			public Vector2 StartPosition;
			public List<Vector2> MoveDirections = new List<Vector2>();

			public CPlayerRecording(Vector2 pos) {
				this.StartPosition = pos;
			}
		}

		public enum EState {
			Recording,
			Replaying,
		}

		[RuntimeGroup]
		public List<CPlayerRecording> Recordings = new List<CPlayerRecording>();
		public CPlayerRecording Current => this.Recordings[this.Recordings.Count - 1];

		public List<Replayer> Replayers;

		[RuntimeGroup]
		public EState State;

		PlayerController player => ReferenceManager.Instance.PlayerController;

		private void Start() {
			this.Recordings.Add(new CPlayerRecording(this.player.transform.position));
			this.State = EState.Recording;
		}

		private void FixedUpdate() {
			this.Current.MoveDirections.Add(this.player.MoveDirection);
		}

		private void Update() {
			if (Input.GetKeyDown(KeyCode.R)) {
				this.State = EState.Replaying;
				this.Replay();
			}
		}

		private void Replay() {
			GameObject newReplayer = Instantiate(
				original: ReferenceManager.Instance.PlayerRecordingPrefab,
				position: this.Current.MoveDirections[0],
				rotation: ReferenceManager.Instance.PlayerRecordingPrefab.transform.rotation,
				parent: ReferenceManager.Instance.ReplayerHolder
			);

			Replayer replayer = newReplayer.GetComponent<Replayer>();
			replayer.Recording = this.Current;

			this.Replayers.Add(replayer);

			foreach (Replayer rep in this.Replayers) {
				rep.StartReplayer();
			}

			Vector2 startPos = ReferenceManager.Instance.PlayerStartPosition;
			this.player.transform.position = new Vector2(
				startPos.x + (this.player.transform.localScale.x * 2) * (int)(this.Recordings.Count / 10),
				startPos.y + (this.player.transform.localScale.y * 2) * (this.Recordings.Count - ((int)(this.Recordings.Count / 10) * 10))
			);

			this.Recordings.Add(new CPlayerRecording(this.player.transform.position));
		}
	}
}
