using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	[Obsolete("AxisState is deprecated.  Use InputAxis instead")]
	[Serializable]
	public struct AxisState
	{
		public AxisState(float minValue, float maxValue, bool wrap, bool rangeLocked, float maxSpeed, float accelTime, float decelTime, string name, bool invert)
		{
			this.m_MinValue = minValue;
			this.m_MaxValue = maxValue;
			this.m_Wrap = wrap;
			this.ValueRangeLocked = rangeLocked;
			this.HasRecentering = false;
			this.m_Recentering = new AxisState.Recentering(false, 1f, 2f);
			this.m_SpeedMode = AxisState.SpeedMode.MaxSpeed;
			this.m_MaxSpeed = maxSpeed;
			this.m_AccelTime = accelTime;
			this.m_DecelTime = decelTime;
			this.Value = (minValue + maxValue) / 2f;
			this.m_InputAxisName = name;
			this.m_InputAxisValue = 0f;
			this.m_InvertInput = invert;
			this.m_CurrentSpeed = 0f;
			this.m_InputAxisProvider = null;
			this.m_InputAxisIndex = 0;
			this.m_LastUpdateTime = 0f;
			this.m_LastUpdateFrame = 0;
		}

		public void Validate()
		{
			if (this.m_SpeedMode == AxisState.SpeedMode.MaxSpeed)
			{
				this.m_MaxSpeed = Mathf.Max(0f, this.m_MaxSpeed);
			}
			this.m_AccelTime = Mathf.Max(0f, this.m_AccelTime);
			this.m_DecelTime = Mathf.Max(0f, this.m_DecelTime);
			this.m_MaxValue = Mathf.Clamp(this.m_MaxValue, this.m_MinValue, this.m_MaxValue);
		}

		public void Reset()
		{
			this.m_InputAxisValue = 0f;
			this.m_CurrentSpeed = 0f;
			this.m_LastUpdateTime = 0f;
			this.m_LastUpdateFrame = 0;
		}

		public void SetInputAxisProvider(int axis, AxisState.IInputAxisProvider provider)
		{
			this.m_InputAxisIndex = axis;
			this.m_InputAxisProvider = provider;
		}

		public bool HasInputProvider
		{
			get
			{
				return this.m_InputAxisProvider != null;
			}
		}

		public bool Update(float deltaTime)
		{
			if (CinemachineCore.CurrentUpdateFrame == this.m_LastUpdateFrame)
			{
				return false;
			}
			this.m_LastUpdateFrame = CinemachineCore.CurrentUpdateFrame;
			if (deltaTime > 0f && this.m_LastUpdateTime != 0f)
			{
				deltaTime = Time.realtimeSinceStartup - this.m_LastUpdateTime;
			}
			this.m_LastUpdateTime = Time.realtimeSinceStartup;
			if (this.m_InputAxisProvider != null)
			{
				this.m_InputAxisValue = this.m_InputAxisProvider.GetAxisValue(this.m_InputAxisIndex);
			}
			else if (!string.IsNullOrEmpty(this.m_InputAxisName))
			{
				try
				{
					this.m_InputAxisValue = CinemachineCore.GetInputAxis(this.m_InputAxisName);
				}
				catch (ArgumentException ex)
				{
					Debug.LogError(ex.ToString());
				}
			}
			float num = this.m_InputAxisValue;
			if (this.m_InvertInput)
			{
				num *= -1f;
			}
			if (this.m_SpeedMode == AxisState.SpeedMode.MaxSpeed)
			{
				return this.MaxSpeedUpdate(num, deltaTime);
			}
			num *= this.m_MaxSpeed;
			if (deltaTime < 0.0001f)
			{
				this.m_CurrentSpeed = 0f;
			}
			else
			{
				float num2 = num / deltaTime;
				float dampTime = (Mathf.Abs(num2) < Mathf.Abs(this.m_CurrentSpeed)) ? this.m_DecelTime : this.m_AccelTime;
				num2 = this.m_CurrentSpeed + Damper.Damp(num2 - this.m_CurrentSpeed, dampTime, deltaTime);
				this.m_CurrentSpeed = num2;
				float num3 = this.m_MaxValue - this.m_MinValue;
				if (!this.m_Wrap && this.m_DecelTime > 0.0001f && num3 > 0.0001f)
				{
					float num4 = this.ClampValue(this.Value);
					float num5 = this.ClampValue(num4 + num2 * deltaTime);
					if (((num2 > 0f) ? (this.m_MaxValue - num5) : (num5 - this.m_MinValue)) < 0.1f * num3 && Mathf.Abs(num2) > 0.0001f)
					{
						num2 = Damper.Damp(num5 - num4, this.m_DecelTime, deltaTime) / deltaTime;
					}
				}
				num = num2 * deltaTime;
			}
			this.Value = this.ClampValue(this.Value + num);
			return Mathf.Abs(num) > 0.0001f;
		}

		private float ClampValue(float v)
		{
			float num = this.m_MaxValue - this.m_MinValue;
			if (this.m_Wrap && num > 0.0001f)
			{
				v = (v - this.m_MinValue) % num;
				v += this.m_MinValue + ((v < 0f) ? num : 0f);
			}
			return Mathf.Clamp(v, this.m_MinValue, this.m_MaxValue);
		}

		private bool MaxSpeedUpdate(float input, float deltaTime)
		{
			if (this.m_MaxSpeed > 0.0001f)
			{
				float num = input * this.m_MaxSpeed;
				if (Mathf.Abs(num) < 0.0001f || (Mathf.Sign(this.m_CurrentSpeed) == Mathf.Sign(num) && Mathf.Abs(num) < Mathf.Abs(this.m_CurrentSpeed)))
				{
					float num2 = Mathf.Min(Mathf.Abs(num - this.m_CurrentSpeed) / Mathf.Max(0.0001f, this.m_DecelTime) * deltaTime, Mathf.Abs(this.m_CurrentSpeed));
					this.m_CurrentSpeed -= Mathf.Sign(this.m_CurrentSpeed) * num2;
				}
				else
				{
					float num3 = Mathf.Abs(num - this.m_CurrentSpeed) / Mathf.Max(0.0001f, this.m_AccelTime);
					this.m_CurrentSpeed += Mathf.Sign(num) * num3 * deltaTime;
					if (Mathf.Sign(this.m_CurrentSpeed) == Mathf.Sign(num) && Mathf.Abs(this.m_CurrentSpeed) > Mathf.Abs(num))
					{
						this.m_CurrentSpeed = num;
					}
				}
			}
			float maxSpeed = this.GetMaxSpeed();
			this.m_CurrentSpeed = Mathf.Clamp(this.m_CurrentSpeed, -maxSpeed, maxSpeed);
			if (Mathf.Abs(this.m_CurrentSpeed) < 0.0001f)
			{
				this.m_CurrentSpeed = 0f;
			}
			this.Value += this.m_CurrentSpeed * deltaTime;
			if (this.Value > this.m_MaxValue || this.Value < this.m_MinValue)
			{
				if (this.m_Wrap)
				{
					if (this.Value > this.m_MaxValue)
					{
						this.Value = this.m_MinValue + (this.Value - this.m_MaxValue);
					}
					else
					{
						this.Value = this.m_MaxValue + (this.Value - this.m_MinValue);
					}
				}
				else
				{
					this.Value = Mathf.Clamp(this.Value, this.m_MinValue, this.m_MaxValue);
					this.m_CurrentSpeed = 0f;
				}
			}
			return Mathf.Abs(input) > 0.0001f;
		}

		private float GetMaxSpeed()
		{
			float num = this.m_MaxValue - this.m_MinValue;
			if (!this.m_Wrap && num > 0f)
			{
				float num2 = num / 10f;
				if (this.m_CurrentSpeed > 0f && this.m_MaxValue - this.Value < num2)
				{
					float t = (this.m_MaxValue - this.Value) / num2;
					return Mathf.Lerp(0f, this.m_MaxSpeed, t);
				}
				if (this.m_CurrentSpeed < 0f && this.Value - this.m_MinValue < num2)
				{
					float t2 = (this.Value - this.m_MinValue) / num2;
					return Mathf.Lerp(0f, this.m_MaxSpeed, t2);
				}
			}
			return this.m_MaxSpeed;
		}

		public bool ValueRangeLocked { readonly get; set; }

		public bool HasRecentering { readonly get; set; }

		[NoSaveDuringPlay]
		[Tooltip("The current value of the axis.")]
		public float Value;

		[Tooltip("How to interpret the Max Speed setting: in units/second, or as a direct input value multiplier")]
		public AxisState.SpeedMode m_SpeedMode;

		[Tooltip("The maximum speed of this axis in units/second, or the input value multiplier, depending on the Speed Mode")]
		public float m_MaxSpeed;

		[Tooltip("The amount of time in seconds it takes to accelerate to MaxSpeed with the supplied Axis at its maximum value")]
		public float m_AccelTime;

		[Tooltip("The amount of time in seconds it takes to decelerate the axis to zero if the supplied axis is in a neutral position")]
		public float m_DecelTime;

		[InputAxisNameProperty]
		[FormerlySerializedAs("m_AxisName")]
		[Tooltip("The name of this axis as specified in Unity Input manager. Setting to an empty string will disable the automatic updating of this axis")]
		public string m_InputAxisName;

		[NoSaveDuringPlay]
		[Tooltip("The value of the input axis.  A value of 0 means no input.  You can drive this directly from a custom input system, or you can set the Axis Name and have the value driven by the internal Input Manager")]
		public float m_InputAxisValue;

		[FormerlySerializedAs("m_InvertAxis")]
		[Tooltip("If checked, then the raw value of the input axis will be inverted before it is used")]
		public bool m_InvertInput;

		[Tooltip("The minimum value for the axis")]
		public float m_MinValue;

		[Tooltip("The maximum value for the axis")]
		public float m_MaxValue;

		[Tooltip("If checked, then the axis will wrap around at the min/max values, forming a loop")]
		public bool m_Wrap;

		[Tooltip("Automatic recentering to at-rest position")]
		public AxisState.Recentering m_Recentering;

		private float m_CurrentSpeed;

		private float m_LastUpdateTime;

		private int m_LastUpdateFrame;

		private const float Epsilon = 0.0001f;

		private AxisState.IInputAxisProvider m_InputAxisProvider;

		private int m_InputAxisIndex;

		public enum SpeedMode
		{
			MaxSpeed,
			InputValueGain
		}

		[Obsolete("IInputAxisProvider is deprecated.  Use InputAxis and InputAxisController instead")]
		public interface IInputAxisProvider
		{
			float GetAxisValue(int axis);
		}

		[Obsolete("IRequiresInput is deprecated.  Use InputAxis and InputAxisController instead")]
		public interface IRequiresInput
		{
			bool RequiresInput();
		}

		[Obsolete("AxisState.Recentering is deprecated.  Use InputAxis and InputAxisController instead")]
		[Serializable]
		public struct Recentering
		{
			public Recentering(bool enabled, float waitTime, float recenteringTime)
			{
				this.m_enabled = enabled;
				this.m_WaitTime = waitTime;
				this.m_RecenteringTime = recenteringTime;
				this.mLastAxisInputTime = 0f;
				this.mRecenteringVelocity = 0f;
				this.m_LegacyHeadingDefinition = (this.m_LegacyVelocityFilterStrength = -1);
				this.m_LastUpdateTime = 0f;
			}

			public void Validate()
			{
				this.m_WaitTime = Mathf.Max(0f, this.m_WaitTime);
				this.m_RecenteringTime = Mathf.Max(0f, this.m_RecenteringTime);
			}

			public void CopyStateFrom(ref AxisState.Recentering other)
			{
				if (this.mLastAxisInputTime != other.mLastAxisInputTime)
				{
					other.mRecenteringVelocity = 0f;
				}
				this.mLastAxisInputTime = other.mLastAxisInputTime;
			}

			public void CancelRecentering()
			{
				this.mLastAxisInputTime = Time.realtimeSinceStartup;
				this.mRecenteringVelocity = 0f;
			}

			public void RecenterNow()
			{
				this.mLastAxisInputTime = -1f;
			}

			public void DoRecentering(ref AxisState axis, float deltaTime, float recenterTarget)
			{
				if (deltaTime > 0f)
				{
					deltaTime = Time.realtimeSinceStartup - this.m_LastUpdateTime;
				}
				this.m_LastUpdateTime = Time.realtimeSinceStartup;
				if (!this.m_enabled && deltaTime >= 0f)
				{
					return;
				}
				recenterTarget = axis.ClampValue(recenterTarget);
				if (deltaTime < 0f)
				{
					this.CancelRecentering();
					if (this.m_enabled)
					{
						axis.Value = recenterTarget;
					}
					return;
				}
				float num = axis.ClampValue(axis.Value);
				float num2 = recenterTarget - num;
				if (num2 == 0f)
				{
					return;
				}
				if (this.mLastAxisInputTime >= 0f && Time.realtimeSinceStartup < this.mLastAxisInputTime + this.m_WaitTime)
				{
					return;
				}
				float num3 = axis.m_MaxValue - axis.m_MinValue;
				if (axis.m_Wrap && Mathf.Abs(num2) > num3 * 0.5f)
				{
					num += Mathf.Sign(recenterTarget - num) * num3;
				}
				if (this.m_RecenteringTime < 0.001f)
				{
					num = recenterTarget;
				}
				else
				{
					num = Mathf.SmoothDamp(num, recenterTarget, ref this.mRecenteringVelocity, this.m_RecenteringTime, 9999f, deltaTime);
				}
				axis.Value = axis.ClampValue(num);
			}

			internal bool LegacyUpgrade(ref int heading, ref int velocityFilter)
			{
				if (this.m_LegacyHeadingDefinition != -1 && this.m_LegacyVelocityFilterStrength != -1)
				{
					heading = this.m_LegacyHeadingDefinition;
					velocityFilter = this.m_LegacyVelocityFilterStrength;
					this.m_LegacyHeadingDefinition = (this.m_LegacyVelocityFilterStrength = -1);
					return true;
				}
				return false;
			}

			[Tooltip("If checked, will enable automatic recentering of the axis. If unchecked, recenting is disabled.")]
			public bool m_enabled;

			[Tooltip("If no user input has been detected on the axis, the axis will wait this long in seconds before recentering.")]
			public float m_WaitTime;

			[Tooltip("How long it takes to reach destination once recentering has started.")]
			public float m_RecenteringTime;

			private float m_LastUpdateTime;

			private float mLastAxisInputTime;

			private float mRecenteringVelocity;

			[SerializeField]
			[HideInInspector]
			[FormerlySerializedAs("m_HeadingDefinition")]
			private int m_LegacyHeadingDefinition;

			[SerializeField]
			[HideInInspector]
			[FormerlySerializedAs("m_VelocityFilterStrength")]
			private int m_LegacyVelocityFilterStrength;
		}
	}
}
