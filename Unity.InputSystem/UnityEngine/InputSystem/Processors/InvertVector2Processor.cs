using System;

namespace UnityEngine.InputSystem.Processors
{
	public class InvertVector2Processor : InputProcessor<Vector2>
	{
		public override Vector2 Process(Vector2 value, InputControl control)
		{
			if (this.invertX)
			{
				value.x *= -1f;
			}
			if (this.invertY)
			{
				value.y *= -1f;
			}
			return value;
		}

		public override string ToString()
		{
			return string.Format("InvertVector2(invertX={0},invertY={1})", this.invertX, this.invertY);
		}

		public bool invertX = true;

		public bool invertY = true;
	}
}
