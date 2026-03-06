using System;

namespace UnityEngine.InputSystem.Processors
{
	public class NormalizeProcessor : InputProcessor<float>
	{
		public override float Process(float value, InputControl control)
		{
			return NormalizeProcessor.Normalize(value, this.min, this.max, this.zero);
		}

		public static float Normalize(float value, float min, float max, float zero)
		{
			if (zero < min)
			{
				zero = min;
			}
			if (Mathf.Approximately(value, min))
			{
				if (min < zero)
				{
					return -1f;
				}
				return 0f;
			}
			else
			{
				float num = (value - min) / (max - min);
				if (min < zero)
				{
					return 2f * num - 1f;
				}
				return num;
			}
		}

		internal static float Denormalize(float value, float min, float max, float zero)
		{
			if (zero < min)
			{
				zero = min;
			}
			if (min >= zero)
			{
				return min + (max - min) * value;
			}
			if (value < 0f)
			{
				return min + (zero - min) * (value * -1f);
			}
			return zero + (max - zero) * value;
		}

		public override string ToString()
		{
			return string.Format("Normalize(min={0},max={1},zero={2})", this.min, this.max, this.zero);
		}

		public float min;

		public float max;

		public float zero;
	}
}
