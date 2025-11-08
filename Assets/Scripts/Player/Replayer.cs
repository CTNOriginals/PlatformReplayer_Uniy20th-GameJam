using System.Collections.Generic;
using CTNOriginals.PlatformReplayer.Managers;
using UnityEngine;

namespace CTNOriginals.PlatformReplayer.Player {
	public class Replayer : MonoBehaviour {
		public enum EState {
			Idle,
			Replaying,
			Done,
		}

		public RecorderManager.CPlayerRecording Recording;
		public EState State = EState.Idle;

		public int Index;

		private void FixedUpdate() {
			if (this.State != EState.Replaying) {
				return;
			}

			if (Index >= this.Recording.Positions.Count) {
				this.State = EState.Done;
				return;
			}

			this.transform.position = this.Recording.Positions[this.Index];
			this.Index++;
		}

		public void StartReplayer() {
			this.Index = 0;
			this.State = EState.Replaying;
		}
	}
}
