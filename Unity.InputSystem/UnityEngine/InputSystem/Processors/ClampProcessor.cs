using System;

namespace UnityEngine.InputSystem.Processors
{
	public class ClampProcessor : InputProcessor<float>
	{
		public override float Process(float value, InputControl control)
		{
			return Mathf.Clamp(value, this.min, this.max);
		}

		public override string ToString()
		{
			return string.Format("Clamp(min={0},max={1})", this.min, this.max);
		}

		public float min;

		public float max;
	}
}
