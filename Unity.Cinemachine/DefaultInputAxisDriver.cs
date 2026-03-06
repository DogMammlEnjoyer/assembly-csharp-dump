using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	[Serializable]
	public struct DefaultInputAxisDriver
	{
		public void Validate()
		{
			this.AccelTime = Mathf.Max(0f, this.AccelTime);
			this.DecelTime = Mathf.Max(0f, this.DecelTime);
		}

		public static DefaultInputAxisDriver Default
		{
			get
			{
				return new DefaultInputAxisDriver
				{
					AccelTime = 0.2f,
					DecelTime = 0.2f
				};
			}
		}

		public void ProcessInput(ref InputAxis axis, float inputValue, float deltaTime)
		{
			float dampTime = (Mathf.Abs(inputValue) < Mathf.Abs(this.m_CurrentSpeed)) ? this.DecelTime : this.AccelTime;
			if ((axis.Restrictions & InputAxis.RestrictionFlags.Momentary) == InputAxis.RestrictionFlags.None)
			{
				if (deltaTime < 0f)
				{
					this.m_CurrentSpeed = 0f;
				}
				else
				{
					this.m_CurrentSpeed += Damper.Damp(inputValue - this.m_CurrentSpeed, dampTime, deltaTime);
					if (!axis.Wrap && this.DecelTime > 0.0001f && Mathf.Abs(this.m_CurrentSpeed) > 0.0001f)
					{
						float num = axis.ClampValue(axis.Value);
						float num2 = (this.m_CurrentSpeed > 0f) ? (axis.Range.y - num) : (num - axis.Range.x);
						float num3 = 0.1f + 4f * num2 / this.DecelTime;
						if (Mathf.Abs(this.m_CurrentSpeed) > Mathf.Abs(num3))
						{
							this.m_CurrentSpeed = num3 * Mathf.Sign(this.m_CurrentSpeed);
						}
					}
				}
				axis.Value = axis.ClampValue(axis.Value + this.m_CurrentSpeed * deltaTime);
				return;
			}
			if (deltaTime < 0f)
			{
				axis.Value = axis.Center;
				return;
			}
			float num4 = axis.ClampValue(inputValue + axis.Center);
			axis.Value += Damper.Damp(num4 - axis.Value, dampTime, deltaTime);
		}

		public void Reset(ref InputAxis axis)
		{
			this.m_CurrentSpeed = 0f;
			axis.Reset();
		}

		private float m_CurrentSpeed;

		[Tooltip("The amount of time in seconds it takes to accelerate to MaxSpeed with the supplied Axis at its maximum value")]
		public float AccelTime;

		[Tooltip("The amount of time in seconds it takes to decelerate the axis to zero if the supplied axis is in a neutral position")]
		public float DecelTime;
	}
}
