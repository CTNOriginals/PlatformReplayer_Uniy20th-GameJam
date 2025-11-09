using System.Collections.Generic;
using CTNOriginals.PlatformReplayer.Managers;
using UnityEngine;

namespace CTNOriginals.PlatformReplayer.Player {
	[RequireComponent(typeof(SpriteRenderer), typeof(PlayerEyes))]
	public class Replayer : MonoBehaviour {
		public RecorderManager.CPlayerRecording Recording;
		public int Index;

		PlayerEyes playerEyes;
		ReferenceManager.EGameState gameState => ReferenceManager.Instance.GameState;
		
		private void Start() {
			this.playerEyes = this.GetComponent<PlayerEyes>();
		}

		public void Initialize() {
			this.GetComponent<SpriteRenderer>().color = this.Recording.Color;
		}

		public void StartReplayer() {
			this.Index = 0;
		}

		private void FixedUpdate() {
			if (
				this.gameState != (ReferenceManager.EGameState.Playing)
				|| this.Index >= this.Recording.Positions.Count
			) {
				return;
			}

			this.DoMove();
			this.Index++;
		}

		public void DoMove() {
			this.playerEyes.ValidateEyeDirection();
			this.transform.position = this.Recording.Positions[this.Index];
		}
	}
}
