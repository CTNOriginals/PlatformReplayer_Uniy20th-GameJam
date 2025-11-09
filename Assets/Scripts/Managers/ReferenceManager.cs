using CTNOriginals.PlatformReplayer.Player;
using CTNOriginals.PlatformReplayer.Utilities;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace CTNOriginals.PlatformReplayer.Managers {
	public class ReferenceManager : Singleton<ReferenceManager> {
		[FoldoutGroup("Player")]
		public PlayerController PlayerController;
		[FoldoutGroup("Player")]
		public Transform ReplayerHolder;
		[FoldoutGroup("Player")]
		public GameObject PlayerRecordingPrefab;
		[FoldoutGroup("Player")]
		public Vector2 PlayerStartPosition;

		public TMP_Text TimerText;

		private void Start() {
			this.PlayerStartPosition = this.PlayerController.transform.position;
		}
	}
}
