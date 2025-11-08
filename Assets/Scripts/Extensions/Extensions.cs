using UnityEngine;


namespace CTNOriginals.PlatformReplayer.Extensions {
	public static class Vector2Extensions {
		public static Vector2 ToVector2(this Vector3 vector) {
			return new Vector2(vector.x, vector.y);
		}
		public static Vector3 ToVector3(this Vector2 vector) {
			return new Vector3(vector.x, vector.y, 0);
		}

		public static Vector2 AddY(this Vector2 vector, float num) {
			return new Vector2(vector.x, vector.y + num);
		}
		public static Vector2 SubY(this Vector2 vector, float num) {
			return new Vector2(vector.x, vector.y - num);
		}

		public static float Direction(this float num) {
			return (num > 0) ? 1 : (num < 0) ? -1 : 0;
		}
		public static Vector2 Direction(this Vector2 vector) {
			return new Vector2(
				(vector.x > 0) ? 1 : (vector.x < 0) ? -1 : 0,
				(vector.y > 0) ? 1 : (vector.y < 0) ? -1 : 0
			);
		}
		public static Vector3 Direction(this Vector3 vector) {
			return new Vector3(
				(vector.x > 0) ? 1 : (vector.x < 0) ? -1 : 0,
				(vector.y > 0) ? 1 : (vector.y < 0) ? -1 : 0,
				(vector.z > 0) ? 1 : (vector.z < 0) ? -1 : 0
			);
		}

		
	}
}