using System;
using System.Collections.Generic;
using CTNOriginals.PlatformReplayer.Attributes.Composite;
using CTNOriginals.PlatformReplayer.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CTNOriginals.PlatformReplayer.Handlers {
	[Serializable]
	public class InputState {
		public List<KeyCode> KeyList;
		public Vector2 Direction;

		[RuntimeGroup, FoldoutGroup("Runtime/State"), InlineProperty, HideLabel, ReadOnly]
		public CTimeSlice State = new CTimeSlice(true);
		public bool IsActive => this.State.IsActive;
		public float Duration => this.State.Duration;

		[RuntimeGroup, FoldoutGroup("Runtime/Fixed State"), InlineProperty, HideLabel, ReadOnly]
		public CTimeSlice FixedState = new CTimeSlice(false);
		public bool FixedActive => this.FixedState.IsActive;
		public float DixedDuration => this.FixedState.Duration;

		public Action OnKeyDown;
		public Action OnKeyUp;

		public void Update() {
			CheckState(true);
		}

		public void FixedUpdate() {
			CheckState(false);
		}

		private void CheckState(bool isDeltaTime) {
			CTimeSlice state = (isDeltaTime) ? this.State : this.FixedState;

			bool input = false;

			foreach (KeyCode key in KeyList) {
				if (!Input.GetKey(key)) {
					continue;
				}

				input = true;
				break;
			}

			if (!state.IsActive && input) {
				if (isDeltaTime) {
					OnKeyDown?.Invoke();
					this.State.Start();
				} else {
					this.FixedState.Start();
				}
			} else if (state.IsActive && !input) {
				if (isDeltaTime) {
					OnKeyUp?.Invoke();
					this.State.End();
				} else {
					this.FixedState.End();
				}
			}
		}
	}
}