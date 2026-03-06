using System;

namespace UnityEngine.ProBuilder
{
	internal sealed class Transform2D
	{
		public Transform2D(Vector2 position, float rotation, Vector2 scale)
		{
			this.position = position;
			this.rotation = rotation;
			this.scale = scale;
		}

		public Vector2 TransformPoint(Vector2 p)
		{
			p += this.position;
			p.RotateAroundPoint(p, this.rotation);
			p.ScaleAroundPoint(p, this.scale);
			return p;
		}

		public override string ToString()
		{
			string[] array = new string[6];
			array[0] = "T: ";
			int num = 1;
			Vector2 vector = this.position;
			array[num] = vector.ToString();
			array[2] = "\nR: ";
			array[3] = this.rotation.ToString();
			array[4] = "°\nS: ";
			int num2 = 5;
			vector = this.scale;
			array[num2] = vector.ToString();
			return string.Concat(array);
		}

		public Vector2 position;

		public float rotation;

		public Vector2 scale;
	}
}
