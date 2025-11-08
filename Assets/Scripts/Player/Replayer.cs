using System.Collections.Generic;
using UnityEngine;

namespace CTNOriginals.PlatformReplayer.Player {
	public class Replayer : MonoBehaviour {
		public enum EState {
			Idle,
			Replaying,
			Done,
		}

		public List<Vector2> Positions;
		public EState State = EState.Idle;

		public int Index;

		private void FixedUpdate() {
			if (this.State != EState.Replaying) {
				return;
			}

			if (Index >= Positions.Count) {
				this.State = EState.Done;
				return;
			}

			this.transform.position = this.Positions[this.Index];
			this.Index++;
		}

		public void StartReplayer() {
			this.Index = 0;
			this.State = EState.Replaying;
		}
	}
}
