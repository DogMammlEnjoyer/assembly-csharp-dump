using System;

namespace g3
{
	public class MeshIterativeSmooth
	{
		public MeshIterativeSmooth(DMesh3 mesh, int[] vertices, bool bOwnVertices = false)
		{
			this.Mesh = mesh;
			this.Vertices = (bOwnVertices ? vertices : ((int[])vertices.Clone()));
			this.SmoothedPostions = new Vector3d[this.Vertices.Length];
			this.ProjectF = null;
		}

		public virtual ValidationStatus Validate()
		{
			return ValidationStatus.Ok;
		}

		public virtual bool Smooth()
		{
			int num = this.Vertices.Length;
			double a = MathUtil.Clamp(this.Alpha, 0.0, 1.0);
			double num2 = (double)MathUtil.Clamp(this.Rounds, 0, 10000);
			Func<DMesh3, int, double, Vector3d> smoothFunc = new Func<DMesh3, int, double, Vector3d>(MeshUtil.UniformSmooth);
			if (this.SmoothType == MeshIterativeSmooth.SmoothTypes.MeanValue)
			{
				smoothFunc = new Func<DMesh3, int, double, Vector3d>(MeshUtil.MeanValueSmooth);
			}
			else if (this.SmoothType == MeshIterativeSmooth.SmoothTypes.Cotan)
			{
				smoothFunc = new Func<DMesh3, int, double, Vector3d>(MeshUtil.CotanSmooth);
			}
			Action<int> body = delegate(int i)
			{
				int arg = this.Vertices[i];
				this.SmoothedPostions[i] = smoothFunc(this.Mesh, arg, a);
			};
			Action<int> body2 = delegate(int i)
			{
				Vector3d arg = this.SmoothedPostions[i];
				this.SmoothedPostions[i] = this.ProjectF(arg, Vector3f.AxisY, this.Vertices[i]);
			};
			IndexRangeEnumerator source = new IndexRangeEnumerator(0, num);
			int num3 = 0;
			while ((double)num3 < num2)
			{
				gParallel.ForEach<int>(source, body);
				if (this.ProjectF != null)
				{
					gParallel.ForEach<int>(source, body2);
				}
				for (int j = 0; j < num; j++)
				{
					this.Mesh.SetVertex(this.Vertices[j], this.SmoothedPostions[j]);
				}
				num3++;
			}
			return true;
		}

		public DMesh3 Mesh;

		public int[] Vertices;

		public double Alpha = 0.25;

		public int Rounds = 10;

		public MeshIterativeSmooth.SmoothTypes SmoothType;

		public Func<Vector3d, Vector3f, int, Vector3d> ProjectF;

		private Vector3d[] SmoothedPostions;

		public enum SmoothTypes
		{
			Uniform,
			Cotan,
			MeanValue
		}
	}
}
