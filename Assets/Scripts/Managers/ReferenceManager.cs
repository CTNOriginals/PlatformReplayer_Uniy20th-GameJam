using CTNOriginals.PlatformReplayer.Player;
using CTNOriginals.PlatformReplayer.Utilities;
using UnityEngine;

namespace CTNOriginals.PlatformReplayer.Managers {
	public class ReferenceManager : Singleton<ReferenceManager> {
		public PlayerController PlayerController;

		public Transform ReplayerHolder;
		public GameObject PlayerRecordingPrefab;
		public Vector2 PlayerStartPosition;

		private void Start() {
			this.PlayerStartPosition = this.PlayerController.transform.position;
		}
	}
}
