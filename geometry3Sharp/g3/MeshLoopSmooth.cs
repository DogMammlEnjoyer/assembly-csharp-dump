using System;
using System.Collections.Generic;

namespace g3
{
	public class MeshLoopSmooth
	{
		public MeshLoopSmooth(DMesh3 mesh, EdgeLoop loop)
		{
			this.Mesh = mesh;
			this.Loop = loop;
			this.SmoothedPostions = new Vector3d[this.Loop.Vertices.Length];
			this.ProjectF = null;
		}

		public virtual ValidationStatus Validate()
		{
			return MeshValidation.IsEdgeLoop(this.Mesh, this.Loop);
		}

		public virtual bool Smooth()
		{
			int NV = this.Loop.Vertices.Length;
			double a = MathUtil.Clamp(this.Alpha, 0.0, 1.0);
			double num = (double)MathUtil.Clamp(this.Rounds, 0, 10000);
			int num2 = 0;
			Action<int> <>9__0;
			Action<int> <>9__1;
			while ((double)num2 < num)
			{
				IEnumerable<int> source = Interval1i.Range(NV);
				Action<int> body;
				if ((body = <>9__0) == null)
				{
					body = (<>9__0 = delegate(int i)
					{
						int vID = this.Loop.Vertices[(i + 1) % NV];
						Vector3d vertex = this.Mesh.GetVertex(this.Loop.Vertices[i]);
						Vector3d vertex2 = this.Mesh.GetVertex(vID);
						Vector3d vertex3 = this.Mesh.GetVertex(this.Loop.Vertices[(i + 2) % NV]);
						Vector3d v = (vertex + vertex3) * 0.5;
						this.SmoothedPostions[i] = (1.0 - a) * vertex2 + a * v;
					});
				}
				gParallel.ForEach<int>(source, body);
				IEnumerable<int> source2 = Interval1i.Range(NV);
				Action<int> body2;
				if ((body2 = <>9__1) == null)
				{
					body2 = (<>9__1 = delegate(int i)
					{
						int num3 = this.Loop.Vertices[(i + 1) % NV];
						Vector3d vector3d = this.SmoothedPostions[i];
						if (this.ProjectF != null)
						{
							vector3d = this.ProjectF(vector3d, num3);
						}
						this.Mesh.SetVertex(num3, vector3d);
					});
				}
				gParallel.ForEach<int>(source2, body2);
				num2++;
			}
			return true;
		}

		public DMesh3 Mesh;

		public EdgeLoop Loop;

		public double Alpha = 0.25;

		public int Rounds = 10;

		public Func<Vector3d, int, Vector3d> ProjectF;

		private Vector3d[] SmoothedPostions;
	}
}
