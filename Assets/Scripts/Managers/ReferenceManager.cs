using CTNOriginals.PlatformReplayer.Player;
using CTNOriginals.PlatformReplayer.Utilities;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace CTNOriginals.PlatformReplayer.Managers {
	public class ReferenceManager : Singleton<ReferenceManager> {
		public enum EGameState {
			Initializing,
			Playing,
			Rewinding,
			Finished,
		}

		public EGameState GameState = EGameState.Initializing;

		[FoldoutGroup("Player")]
		public PlayerController PlayerController;
		[FoldoutGroup("Player")]
		public Transform ReplayerHolder;
		[FoldoutGroup("Player")]
		public GameObject PlayerRecordingPrefab;
		[FoldoutGroup("Player")]
		public Vector2 PlayerStartPosition;

		public TMP_Text TimerText;

		public AudioSource AudioSource;

		private void Start() {
			this.PlayerStartPosition = this.PlayerController.transform.position;
		}
	}
}
