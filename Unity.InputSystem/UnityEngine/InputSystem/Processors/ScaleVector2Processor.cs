using System;

namespace UnityEngine.InputSystem.Processors
{
	public class ScaleVector2Processor : InputProcessor<Vector2>
	{
		public override Vector2 Process(Vector2 value, InputControl control)
		{
			return new Vector2(value.x * this.x, value.y * this.y);
		}

		public override string ToString()
		{
			return string.Format("ScaleVector2(x={0},y={1})", this.x, this.y);
		}

		[Tooltip("Scale factor to multiply the incoming Vector2's X component by.")]
		public float x = 1f;

		[Tooltip("Scale factor to multiply the incoming Vector2's Y component by.")]
		public float y = 1f;
	}
}
