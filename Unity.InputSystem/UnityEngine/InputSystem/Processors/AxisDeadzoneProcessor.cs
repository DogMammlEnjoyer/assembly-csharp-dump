using System;

namespace UnityEngine.InputSystem.Processors
{
	public class AxisDeadzoneProcessor : InputProcessor<float>
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

		public override float Process(float value, InputControl control = null)
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
			return string.Format("AxisDeadzone(min={0},max={1})", this.minOrDefault, this.maxOrDefault);
		}

		public float min;

		public float max;
	}
}
