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
			public List<Vector2> Positions = new List<Vector2>();
		}

		public enum EState {
			Recording,
			Replaying,
		}

		[RuntimeGroup]
		public List<CPlayerRecording> Recordings = new List<CPlayerRecording>();
		public CPlayerRecording Current => this.Recordings[this.Recordings.Count - 1];

		public List<Replayer> Replayers;

		public Vector2 D_pos;
		public int D_count;

		[RuntimeGroup]
		public EState State;

		PlayerController player => ReferenceManager.Instance.PlayerController;

		private void Start() {
			this.Recordings.Add(new CPlayerRecording());
			this.State = EState.Recording;
		}

		private void FixedUpdate() {
			this.Current.Positions.Add(this.player.transform.position);
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
				position: this.Current.Positions[0],
				rotation: ReferenceManager.Instance.PlayerRecordingPrefab.transform.rotation,
				parent: ReferenceManager.Instance.ReplayerHolder
			);

			Replayer replayer = newReplayer.GetComponent<Replayer>();
			replayer.Positions = this.Current.Positions;

			this.Replayers.Add(replayer);

			foreach (Replayer rep in this.Replayers) {
				rep.StartReplayer();
			}

			Vector2 startPos = ReferenceManager.Instance.PlayerStartPosition;
			this.player.transform.position = new Vector2(
				startPos.x + (int)(this.Recordings.Count / 5),
				startPos.y + ((this.player.transform.localScale.y * 2) * this.Recordings.Count) - ((int)(this.Recordings.Count / 5) * 5)
			);

			this.Recordings.Add(new CPlayerRecording());
		}
	}
}
