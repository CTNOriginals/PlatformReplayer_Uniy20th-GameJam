using CTNOriginals.PlatformReplayer.Attributes.Composite;
using CTNOriginals.PlatformReplayer.Utilities;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace CTNOriginals.PlatformReplayer.Managers {
	public class TimeManager : Singleton<TimeManager> {
		[ConfigGroup]
		[Tooltip("The maximum time in seconds the lavel allows the player to take until the level is rewinded")]
		public int MaxTime = 30;

		[RuntimeGroup]
		public float TimeStart;
		[RuntimeGroup]
		public int TimeLeft;

		[FoldoutGroup("Debug"), SerializeField]
		private int D_current;
		[FoldoutGroup("Debug"), SerializeField]
		private int D_elapsed;
		[FoldoutGroup("Debug"), Range(0,1), SerializeField] 
		private float D_percentage;
		[FoldoutGroup("Debug"), ReadOnly, SerializeField]
		private float D_time;

		TMP_Text timerText => ReferenceManager.Instance.TimerText;
		ReferenceManager.EGameState gameState => ReferenceManager.Instance.GameState;

		private void Start() {
			this.ResetTimer();
		}

		private void Update() {
			if (gameState != ReferenceManager.EGameState.Playing) {
				return;
			}

			int currentTime = this.MaxTime - (int)(Time.time - this.TimeStart);

			if (currentTime != TimeLeft) {
				this.TimeLeft = currentTime;

				if (this.TimeLeft < 0) {
					RecorderManager.Instance.DoRewind();
					return;
				}
				
				this.timerText.text = this.GetTimerText(this.TimeLeft);
			}
		}

		public void ResetTimer() {
			this.TimeStart = Time.time;
			this.TimeLeft = this.MaxTime;
			this.timerText.text = this.GetTimerText(this.TimeLeft);
		}
		
		public void SetTimerPercentage(float percentage) {
			D_current = this.TimeLeft;
			D_percentage = percentage;

			D_elapsed = this.MaxTime - D_current;
			D_time = D_current + (D_elapsed * D_percentage);
			
			this.timerText.text = this.GetTimerText((int)Mathf.Ceil(D_time));
		}

		private string GetTimerText(int seconds) {
			int min = seconds / 60;
			int sec = seconds % 60;

			string minStr = System.String.Format("{0}", (min < 10) ? "0" + min : min);
			string secStr = System.String.Format("{0}", (sec < 10) ? "0" + sec : sec);

			return System.String.Format("{0}:{1}", minStr, secStr);
		}

		private void OnValidate() {
			D_elapsed = this.MaxTime - D_current;
			D_time = D_current + (D_elapsed * D_percentage);
		}
	}
}
