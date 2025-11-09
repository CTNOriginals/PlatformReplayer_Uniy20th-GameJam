using UnityEngine;
using Sirenix.OdinInspector;

namespace CTNOriginals.PlatformReplayer.Utilities {
	[System.Serializable]
	public class CTimeSlice {
		[ShowInInspector, HorizontalGroup(LabelWidth = 30)]
		public float StartTime { get; private set; }
		[ShowInInspector, HorizontalGroup(LabelWidth = 24, PaddingLeft = 5)]
		public float EndTime { get; private set; }

		[ShowInInspector, HorizontalGroup("getters", Width = 0.3f)]
		public bool IsActive {
			get { return (this.StartTime > this.EndTime); }
		}

		[ShowInInspector, HorizontalGroup("getters", Width = 0.7f), ShowIf("@IsActive"), HideLabel]
		public float Duration {
			get {
				if (this.IsActive) {
					return this.GetTime() - this.StartTime;
				} else {
					return this.EndTime - this.StartTime;
				}
			}
		}

		private bool _deltaTime;

		public CTimeSlice(bool isDeltaTime = true) {
			StartTime = 0;
			EndTime = 0;

			_deltaTime = isDeltaTime;
		}

		public void Start() {
			StartTime = this.GetTime();
		}
		public void End() {
			EndTime = this.GetTime();
		}

		private float GetTime() {
			return (_deltaTime) ? Time.time : Time.fixedTime;
		}
	}

	[System.Serializable]
	public struct SCurveTime {
		[HorizontalGroup(Width = 0.8f), HideLabel]
		public AnimationCurve Curve;

		/// <summary>The time it takes (in seconds) to travel on the curve's X axis</summary>
		[HorizontalGroup(Width = 0.2f), HideLabel, PropertyTooltip("TimeFactor")]
		public float TimeFactor;

		public float GetValue(float time) {
			return this.Curve.Evaluate(time / this.TimeFactor);
		}
	}
	
	[System.Serializable]
	public struct MinMax<T> {
		[HorizontalGroup(LabelWidth = 25)]
		public T Min;
		[HorizontalGroup(LabelWidth = 30)]
		public T Max;
	}
}