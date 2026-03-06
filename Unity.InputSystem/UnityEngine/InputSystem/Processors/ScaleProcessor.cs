using System;

namespace UnityEngine.InputSystem.Processors
{
	public class ScaleProcessor : InputProcessor<float>
	{
		public override float Process(float value, InputControl control)
		{
			return value * this.factor;
		}

		public override string ToString()
		{
			return string.Format("Scale(factor={0})", this.factor);
		}

		[Tooltip("Scale factor to multiply incoming float values by.")]
		public float factor = 1f;
	}
}
