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

		public EState State = EState.Idle;
		public RecorderManager.CPlayerRecording Recording;

		public int Index;

		public void StartReplayer() {
			this.Index = 0;
			this.State = EState.Replaying;
			this.transform.position = this.Recording.StartPosition;
		}

		public Vector2 GetMoveDirection() {
			if (this.State != EState.Replaying) {
				return Vector2.zero;
			}

			if (Index >= this.Recording.MoveDirections.Count) {
				this.State = EState.Done;
				return Vector2.zero;
			}

			Vector2 dir = this.Recording.MoveDirections[this.Index];
			this.Index++;

			return dir;
		}
	}
}
