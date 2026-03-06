using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	[Serializable]
	public struct InputAxis
	{
		public float ClampValue(float v)
		{
			float num = this.Range.y - this.Range.x;
			if (!this.Wrap || num < 0.0001f)
			{
				return Mathf.Clamp(v, this.Range.x, this.Range.y);
			}
			float num2 = (v - this.Range.x) % num;
			return num2 + ((num2 < 0f) ? num : 0f) + this.Range.x;
		}

		public float GetNormalizedValue()
		{
			float num = this.ClampValue(this.Value);
			float num2 = this.Range.y - this.Range.x;
			return (num - this.Range.x) / ((num2 > 0.0001f) ? num2 : 1f);
		}

		public float GetClampedValue()
		{
			return this.ClampValue(this.Value);
		}

		public void Validate()
		{
			this.Range.y = Mathf.Max(this.Range.x, this.Range.y);
			this.Center = this.ClampValue(this.Center);
			this.Value = this.ClampValue(this.Value);
			this.Recentering.Validate();
		}

		public void Reset()
		{
			this.CancelRecentering();
			if (this.Recentering.Enabled && (this.Restrictions & InputAxis.RestrictionFlags.NoRecentering) == InputAxis.RestrictionFlags.None)
			{
				this.Value = this.ClampValue(this.Center);
			}
		}

		public static InputAxis DefaultMomentary
		{
			get
			{
				return new InputAxis
				{
					Range = new Vector2(-1f, 1f),
					Restrictions = (InputAxis.RestrictionFlags.NoRecentering | InputAxis.RestrictionFlags.Momentary)
				};
			}
		}

		public bool TrackValueChange()
		{
			float num = this.ClampValue(this.Value);
			if (num != this.m_RecenteringState.m_LastValue)
			{
				this.m_RecenteringState.m_LastValueChangeTime = InputAxis.RecenteringState.CurrentTime;
				this.m_RecenteringState.m_LastValue = num;
				return true;
			}
			return false;
		}

		internal void SetValueAndLastValue(float value)
		{
			this.m_RecenteringState.m_LastValue = value;
			this.Value = value;
		}

		public void UpdateRecentering(float deltaTime, bool forceCancel)
		{
			this.UpdateRecentering(deltaTime, forceCancel, this.Center);
		}

		public void UpdateRecentering(float deltaTime, bool forceCancel, float center)
		{
			if ((this.Restrictions & (InputAxis.RestrictionFlags.NoRecentering | InputAxis.RestrictionFlags.Momentary)) != InputAxis.RestrictionFlags.None)
			{
				return;
			}
			if (forceCancel)
			{
				this.CancelRecentering();
				return;
			}
			if ((this.m_RecenteringState.m_ForceRecenter || this.Recentering.Enabled) && deltaTime < 0f)
			{
				this.Value = this.ClampValue(center);
				this.CancelRecentering();
				return;
			}
			if (this.m_RecenteringState.m_ForceRecenter || (this.Recentering.Enabled && InputAxis.RecenteringState.CurrentTime - this.m_RecenteringState.m_LastValueChangeTime >= this.Recentering.Wait))
			{
				float num = this.ClampValue(this.Value);
				float num2 = Mathf.Abs(center - num);
				if (num2 < 0.0001f || this.Recentering.Time < 0.0001f)
				{
					num = center;
					this.m_RecenteringState.m_RecenteringVelocity = 0f;
				}
				else
				{
					float num3 = this.Range.y - this.Range.x;
					if (this.Wrap && num2 > num3 * 0.5f)
					{
						num += Mathf.Sign(center - num) * num3;
					}
					num = Mathf.SmoothDamp(num, center, ref this.m_RecenteringState.m_RecenteringVelocity, this.Recentering.Time * 0.5f, 9999f, deltaTime);
				}
				this.Value = (this.m_RecenteringState.m_LastValue = this.ClampValue(num));
				if (Mathf.Abs(this.Value - center) < 0.0001f)
				{
					this.m_RecenteringState.m_ForceRecenter = false;
				}
			}
		}

		public void TriggerRecentering()
		{
			this.m_RecenteringState.m_ForceRecenter = true;
		}

		public void CancelRecentering()
		{
			this.m_RecenteringState.m_LastValueChangeTime = InputAxis.RecenteringState.CurrentTime;
			this.m_RecenteringState.m_LastValue = this.ClampValue(this.Value);
			this.m_RecenteringState.m_RecenteringVelocity = 0f;
			this.m_RecenteringState.m_ForceRecenter = false;
		}

		[Tooltip("The current value of the axis.  You can drive this directly from a script.")]
		[NoSaveDuringPlay]
		public float Value;

		[Delayed]
		[Tooltip("The centered, or at-rest value of this axis.")]
		public float Center;

		[Tooltip("The valid range for the axis value.  Value will be clamped to this range.")]
		[Vector2AsRange]
		public Vector2 Range;

		[Tooltip("If set, then the axis will wrap around at the min/max values, forming a loop")]
		public bool Wrap;

		[FoldoutWithEnabledButton("Enabled")]
		public InputAxis.RecenteringSettings Recentering;

		[HideInInspector]
		public InputAxis.RestrictionFlags Restrictions;

		private InputAxis.RecenteringState m_RecenteringState;

		[Serializable]
		public struct RecenteringSettings
		{
			public static InputAxis.RecenteringSettings Default
			{
				get
				{
					return new InputAxis.RecenteringSettings
					{
						Wait = 1f,
						Time = 2f
					};
				}
			}

			public void Validate()
			{
				this.Wait = Mathf.Max(0f, this.Wait);
				this.Time = Mathf.Max(0f, this.Time);
			}

			[Tooltip("If set, will enable automatic re-centering of the axis")]
			public bool Enabled;

			[Tooltip("If no user input has been detected on the axis for this many seconds, re-centering will begin.")]
			public float Wait;

			[Tooltip("How long it takes to reach center once re-centering has started.")]
			public float Time;
		}

		[Flags]
		public enum RestrictionFlags
		{
			None = 0,
			RangeIsDriven = 1,
			NoRecentering = 2,
			Momentary = 4
		}

		private struct RecenteringState
		{
			public static float CurrentTime
			{
				get
				{
					return CinemachineCore.CurrentUnscaledTime;
				}
			}

			public const float k_Epsilon = 0.0001f;

			public float m_RecenteringVelocity;

			public bool m_ForceRecenter;

			public float m_LastValueChangeTime;

			public float m_LastValue;
		}
	}
}
