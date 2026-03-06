using System;

namespace UnityEngine.InputSystem.Processors
{
	public class InvertVector3Processor : InputProcessor<Vector3>
	{
		public override Vector3 Process(Vector3 value, InputControl control)
		{
			if (this.invertX)
			{
				value.x *= -1f;
			}
			if (this.invertY)
			{
				value.y *= -1f;
			}
			if (this.invertZ)
			{
				value.z *= -1f;
			}
			return value;
		}

		public override string ToString()
		{
			return string.Format("InvertVector3(invertX={0},invertY={1},invertZ={2})", this.invertX, this.invertY, this.invertZ);
		}

		public bool invertX = true;

		public bool invertY = true;

		public bool invertZ = true;
	}
}
