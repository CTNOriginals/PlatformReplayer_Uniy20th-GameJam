using CTNOriginals.PlatformReplayer.Extensions;
using UnityEngine;

namespace CTNOriginals.PlatformReplayer.Player {
	public class PlayerEyes : MonoBehaviour {
		public GameObject EyeLeft;
		public GameObject EyeRight;


		private bool _isPlayer;
		private Replayer _replayer;
		private PlayerController _controller;

		private void Awake() {
			if (this.TryGetComponent<Replayer>(out _replayer)) {
				this._isPlayer = false;
			} else if (this.TryGetComponent<PlayerController>(out _controller)) {
				this._isPlayer = true;
			} else {
				throw new System.Exception("Unknown player type");
			}
		}

		private void FixedUpdate() {
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
					this._replayer.State != Replayer.EState.Replaying
					|| this._replayer.Index == 0
					|| this._replayer.Index >= this._replayer.Positions.Count
				) {
					return Vector2.zero;
				}
				
				return this._replayer.Positions[this._replayer.Index] - this._replayer.Positions[this._replayer.Index - 1];
			}
		}
	}
}