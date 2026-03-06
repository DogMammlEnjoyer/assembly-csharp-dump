using System;

namespace g3
{
	public class Sphere3Generator_NormalizedCube : GridBox3Generator
	{
		public override MeshGenerator Generate()
		{
			base.Generate();
			for (int i = 0; i < this.vertices.Count; i++)
			{
				Vector3d v = this.vertices[i] - this.Box.Center;
				if (this.NormalizeType == Sphere3Generator_NormalizedCube.NormalizationTypes.CubeMapping)
				{
					double num = v.Dot(this.Box.AxisX) / this.Box.Extent.x;
					double num2 = v.Dot(this.Box.AxisY) / this.Box.Extent.y;
					double num3 = v.Dot(this.Box.AxisZ) / this.Box.Extent.z;
					double num4 = num * num;
					double num5 = num2 * num2;
					double num6 = num3 * num3;
					double f = num * Math.Sqrt(1.0 - num5 * 0.5 - num6 * 0.5 + num5 * num6 / 3.0);
					double f2 = num2 * Math.Sqrt(1.0 - num4 * 0.5 - num6 * 0.5 + num4 * num6 / 3.0);
					double f3 = num3 * Math.Sqrt(1.0 - num4 * 0.5 - num5 * 0.5 + num4 * num5 / 3.0);
					v = f * this.Box.AxisX + f2 * this.Box.AxisY + f3 * this.Box.AxisZ;
				}
				v.Normalize(2.220446049250313E-16);
				this.vertices[i] = this.Box.Center + this.Radius * v;
				this.normals[i] = (Vector3f)v;
			}
			return this;
		}

		public double Radius = 1.0;

		private Sphere3Generator_NormalizedCube.NormalizationTypes NormalizeType = Sphere3Generator_NormalizedCube.NormalizationTypes.CubeMapping;

		public enum NormalizationTypes
		{
			NormalizedVector,
			CubeMapping
		}
	}
}
