using System;

namespace UnityEngine.InputSystem.Processors
{
	public class StickDeadzoneProcessor : InputProcessor<Vector2>
	{
		private float minOrDefault
		{
			get
			{
				if (this.min != 0f)
				{
					return this.min;
				}
				return InputSystem.settings.defaultDeadzoneMin;
			}
		}

		private float maxOrDefault
		{
			get
			{
				if (this.max != 0f)
				{
					return this.max;
				}
				return InputSystem.settings.defaultDeadzoneMax;
			}
		}

		public override Vector2 Process(Vector2 value, InputControl control = null)
		{
			float magnitude = value.magnitude;
			float deadZoneAdjustedValue = this.GetDeadZoneAdjustedValue(magnitude);
			if (deadZoneAdjustedValue == 0f)
			{
				value = Vector2.zero;
			}
			else
			{
				value *= deadZoneAdjustedValue / magnitude;
			}
			return value;
		}

		private float GetDeadZoneAdjustedValue(float value)
		{
			float minOrDefault = this.minOrDefault;
			float maxOrDefault = this.maxOrDefault;
			float num = Mathf.Abs(value);
			if (num < minOrDefault)
			{
				return 0f;
			}
			if (num > maxOrDefault)
			{
				return Mathf.Sign(value);
			}
			return Mathf.Sign(value) * ((num - minOrDefault) / (maxOrDefault - minOrDefault));
		}

		public override string ToString()
		{
			return string.Format("StickDeadzone(min={0},max={1})", this.minOrDefault, this.maxOrDefault);
		}

		public float min;

		public float max;
	}
}
