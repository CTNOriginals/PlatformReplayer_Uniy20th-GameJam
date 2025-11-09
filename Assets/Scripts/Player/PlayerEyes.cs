using CTNOriginals.PlatformReplayer.Extensions;
using CTNOriginals.PlatformReplayer.Managers;
using UnityEngine;

namespace CTNOriginals.PlatformReplayer.Player {
	public class PlayerEyes : MonoBehaviour {
		public GameObject EyeLeft;
		public GameObject EyeRight;

		private bool _isPlayer;
		private Replayer _replayer;
		private PlayerController _controller;
		private ReferenceManager.EGameState gameState => ReferenceManager.Instance.GameState;

		private void Awake() {
			if (this.TryGetComponent<Replayer>(out _replayer)) {
				this._isPlayer = false;
			} else if (this.TryGetComponent<PlayerController>(out _controller)) {
				this._isPlayer = true;
			} else {
				throw new System.Exception("Unknown player type");
			}
		}

		public void ValidateEyeDirection() {
			float step = this.GetStep().x;

			if (Mathf.Abs(step) < this.transform.localScale.x * 0.025f) {
				return;
			}

			float dir = step.Direction();

			if (dir == 1 && !EyeRight.activeSelf) {
				EyeLeft.SetActive(false);
				EyeRight.SetActive(true);
			} else if (dir == -1 && !EyeLeft.activeSelf) {
				EyeLeft.SetActive(true);
				EyeRight.SetActive(false);
			}
		}

		private Vector2 GetStep() {
			if (this._isPlayer) {
				return this._controller.Step;
			} else {
				if (
					(gameState != ReferenceManager.EGameState.Playing && gameState != ReferenceManager.EGameState.Rewinding)
					|| this._replayer.Index == 0
					|| this._replayer.Index >= this._replayer.Recording.Positions.Count
				) {
					return Vector2.zero;
				}

				switch (ReferenceManager.Instance.GameState) {
					default:
					case ReferenceManager.EGameState.Playing:
						return this._replayer.Recording.Positions[this._replayer.Index] - this._replayer.Recording.Positions[this._replayer.Index - 1];
					case ReferenceManager.EGameState.Rewinding:
						return this._replayer.Recording.Positions[this._replayer.Index - 1] - this._replayer.Recording.Positions[this._replayer.Index];
				}
			}
		}
	}
}