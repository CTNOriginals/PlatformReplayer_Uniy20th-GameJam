using CTNOriginals.PlatformReplayer.Attributes.Composite;
using CTNOriginals.PlatformReplayer.Extensions;
using CTNOriginals.PlatformReplayer.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CTNOriginals.PlatformReplayer.Player {
	[RequireComponent(typeof(PlayerInput), typeof(Rigidbody2D))]
	public class PlayerController : MonoBehaviour {
		[ReferenceGroup, SerializeField]
		private GameObject EyeLeft;
		[ReferenceGroup, SerializeField]
		private GameObject EyeRight;

		[ConfigGroup, SerializeField, Range(0, 100)]
		private float _baseSpeed = 10;
		[ConfigGroup, SerializeField, Range(0, 100)]
		private float _jumpForce = 10;

		[ConfigGroup, SerializeField, Range(0, -100)]
		private float _gravity = -9.81f;

		[ConfigGroup, SerializeField, InlineProperty]
		[Tooltip("Player Acceleration from stationary to moving")]
		private SCurveTime _initialAccel;

		[RuntimeGroup, SerializeField]
		private float _currentSpeed;
		[RuntimeGroup, SerializeField]
		private Vector2 _velocity;
		[RuntimeGroup, SerializeField]
		private Vector2 _moveDir;

		[RuntimeGroup, SerializeField]
		private bool _isGrounded;
		
		[RuntimeGroup, SerializeField]
		private Transform _ground;
		[RuntimeGroup, SerializeField]
		private Transform _wallLeft;
		[RuntimeGroup, SerializeField]
		private Transform _wallRight;

		public Vector2 D_step;

		PlayerInput playerInput;
		Rigidbody2D rb;

		private void Awake() {
			this.playerInput = this.GetComponent<PlayerInput>();
			this.rb = this.GetComponent<Rigidbody2D>();
		}

		private void FixedUpdate() {
			this._currentSpeed = this.GetMovementSpeed();
			this._velocity = this.GetPlayerVelocity(this._velocity, this._currentSpeed);

			Vector2 step = this._velocity * Time.fixedDeltaTime;

			if (step.x < 0 && _wallLeft != null || step.x > 0 && _wallRight != null) {
				this._velocity.x = 0;
				step.x = 0;
			}
			
			Vector2 dir = step.Direction();
			if (dir.x == 1 && !EyeRight.activeSelf) {
				EyeLeft.SetActive(false);
				EyeRight.SetActive(true);
			} else if (dir.x == -1 && !EyeLeft.activeSelf) {
				EyeLeft.SetActive(true);
				EyeRight.SetActive(false);
			}
			
			this.transform.position += (Vector3)step;
		}
		
		private float GetMovementSpeed() {
			if (!this.playerInput.MovementState.IsActive) {
				return 0;
			}

			return this._baseSpeed * this._initialAccel.GetValue(playerInput.MovementState.Duration);
		}

		/// <summary>
		/// Calculates the players velocity for this fixed frame based on current velocity and speed.
		/// </summary>
		/// <param name="velocity">The current velocity of the player</param>
		/// <param name="speed">The current speed value of the player</param>
		/// <remarks>Currently ignores the current x and z velocity axis that are passed in</remarks>
		/// <returns>The player's velocity during this fixed frame</returns>
		private Vector2 GetPlayerVelocity(Vector3 velocity, float speed) {
			//? Clamp the magnitude to prevent the player from gaining more speed 
			//? while more then 1 input is held down at the same time, like holding W and D.
			this._moveDir = Vector2.ClampMagnitude(playerInput.MoveDirection, 1);

			Vector2 newVelocity = new Vector2 {
				x = this._moveDir.x * speed,
				y = velocity.y,
			};

			//* Apply gravity
			newVelocity.y += this._gravity * Time.fixedDeltaTime;

			if (_isGrounded) {
				newVelocity.y = 0;

				if (this.playerInput.Jump.State.IsActive) {
					/* Calculate the jump force of the player.
						? The jump force is calculated with (-2 * gravity) to be able to counteract the force of gravity on the player,
						? the -2 reverses the force of gravity (which should be a negative number) and then doubles it upwards
						? to allow the player to jump with by applying this force in just one fixed frame instead of applying a force over multiple frames.
						? This effectively calculates the force needed to push the player up to a fixed height (_jumpForce) 
						? which will be the peak of the jump before falling down again. 
					*/
					newVelocity.y = Mathf.Sqrt(this._jumpForce * -2 * _gravity);
				}
			}


			return newVelocity;
		}

		private void OnCollisionEnter2D(Collision2D other) {
			Vector2 normal = other.GetContact(0).normal;
			
			switch (other.transform.tag) {
				case "Ground": 
					if (normal == Vector2.up) {
						this._isGrounded = true;
						this._ground = other.transform;
					}
					break;
				case "Wall":
					if (normal == Vector2.right) {
						this._wallLeft = other.transform;
					} else if (normal == Vector2.left) {
						this._wallRight = other.transform;
					}
				break;
			}
		}
		
		private void OnCollisionExit2D(Collision2D other) {
			switch (other.transform.tag) {
				case "Ground":
					if (other.transform == this._ground) {
						this._isGrounded = false;
						this._ground = null;
					}
					break;
				case "Wall":
					if (other.transform == _wallLeft) {
						_wallLeft = null;
					} else if (other.transform == _wallRight) {
						_wallRight = null;
					}
					break;
			}
		}
	}
}
