using UnityEngine;
using Sirenix.OdinInspector;
using CTNOriginals.PlatformReplayer.Handlers;
using CTNOriginals.PlatformReplayer.Attributes.Composite;
using System.Collections.Generic;
using CTNOriginals.PlatformReplayer.Utilities;

namespace CTNOriginals.PlatformReplayer.Player {
	public class PlayerInput : MonoBehaviour {
		[FoldoutGroup("Config/Movement")]

		[ConfigGroup, FoldoutGroup("Config/Movement/Left"), HideLabel, InlineProperty]
		public InputState MoveLeft;
		[ConfigGroup, FoldoutGroup("Config/Movement/Right"), HideLabel, InlineProperty]
		public InputState MoveRight;
		[ConfigGroup, FoldoutGroup("Config/Movement/Jump"), HideLabel, InlineProperty]
		public InputState Jump;

		[RuntimeGroup]
		public Vector2 MoveDirection;

		[RuntimeGroup, FoldoutGroup("Runtime/Movement State"), ReadOnly, InlineProperty, HideLabel]
		[Tooltip("The time that passed while the player is pressing any movement inputs")]
		public CTimeSlice MovementState = new CTimeSlice(false);

		private List<InputState> _inputStates = new List<InputState>();

		private void Awake() {
			_inputStates.AddRange(new List<InputState> {
				MoveLeft,
				MoveRight,
				Jump
			});
		}

		private void Update() {
			_inputStates.ForEach((input) => { input.Update(); });
		}

		private void FixedUpdate() {
			_inputStates.ForEach((input) => { input.FixedUpdate(); });

			MoveDirection = Vector2.zero;
			foreach (InputState input in _inputStates) {
				if (!input.State.IsActive) { continue; }

				MoveDirection += input.Direction;
			}

			//? Register the start/end of any movement inputs being pressed
			if (!MovementState.IsActive && MoveDirection != Vector2.zero) {
				MovementState.Start();
			} else if (MovementState.IsActive && MoveDirection == Vector2.zero) {
				MovementState.End();
			}
		}
	}
}
