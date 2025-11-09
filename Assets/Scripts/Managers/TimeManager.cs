using CTNOriginals.PlatformReplayer.Attributes.Composite;
using CTNOriginals.PlatformReplayer.Utilities;
using TMPro;
using UnityEngine;

namespace CTNOriginals.PlatformReplayer.Managers {
	public class TimeManager : Singleton<TimeManager> {
		[ConfigGroup]
		[Tooltip("The maximum time in seconds the lavel allows the player to take until the level is rewinded")]
		public int MaxTime = 30;

		[RuntimeGroup]
		public int TimeLeft;
		public float LevelStartTime;

		TMP_Text timerText => ReferenceManager.Instance.TimerText;

		private void Start() {
			this.ResetTimer();
		}

		private void Update() {
			int currentTime = this.MaxTime - (int)(Time.time - this.LevelStartTime);
			if (currentTime != TimeLeft) {
				this.TimeLeft = currentTime;
				this.timerText.text = this.GetTimerText();
			}
		}
		
		private void ResetTimer() {
			this.LevelStartTime = Time.time;
			this.TimeLeft = this.MaxTime;
			this.timerText.text = this.GetTimerText();
		}

		private string GetTimerText() {
			int min = this.TimeLeft / 60;
			int sec = this.TimeLeft % 60;
			
			string minStr = System.String.Format("{0}", (min < 10) ? "0" + min : min);
			string secStr = System.String.Format("{0}", (sec < 10) ? "0" + sec : sec);

			return System.String.Format("{0}:{1}", minStr, secStr);
		}
	}
}
