using System;

namespace g3
{
	public class LineGenerator : CurveGenerator
	{
		public override void Generate()
		{
			double length = (this.Start - this.End).Length;
			int num = 10;
			if (this.Subdivisions > 0)
			{
				num = this.Subdivisions;
			}
			else if (this.StepSize > 0.0)
			{
				num = MathUtil.Clamp((int)(length / this.StepSize), 2, 10000);
			}
			this.vertices = new VectorArray3d(num + 1, false);
			for (int i = 0; i < num; i++)
			{
				double num2 = (double)i / (double)num;
				Vector3d value = (1.0 - num2) * this.Start + num2 * this.End;
				this.vertices[i] = value;
			}
			this.vertices[num] = this.End;
			this.closed = false;
		}

		public Vector3d Start = Vector3d.Zero;

		public Vector3d End = Vector3d.AxisX;

		public int Subdivisions = -1;

		public double StepSize;
	}
}
