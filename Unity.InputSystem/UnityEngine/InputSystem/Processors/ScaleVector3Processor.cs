using System;

namespace UnityEngine.InputSystem.Processors
{
	public class ScaleVector3Processor : InputProcessor<Vector3>
	{
		public override Vector3 Process(Vector3 value, InputControl control)
		{
			return new Vector3(value.x * this.x, value.y * this.y, value.z * this.z);
		}

		public override string ToString()
		{
			return string.Format("ScaleVector3(x={0},y={1},z={2})", this.x, this.y, this.z);
		}

		[Tooltip("Scale factor to multiply the incoming Vector3's X component by.")]
		public float x = 1f;

		[Tooltip("Scale factor to multiply the incoming Vector3's Y component by.")]
		public float y = 1f;

		[Tooltip("Scale factor to multiply the incoming Vector3's Z component by.")]
		public float z = 1f;
	}
}
