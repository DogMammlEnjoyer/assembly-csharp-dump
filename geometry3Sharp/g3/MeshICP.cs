using System;

namespace g3
{
	public class MeshICP
	{
		public MeshICP(IPointSet source, DMeshAABBTree3 target)
		{
			this.Source = source;
			this.TargetSurface = target;
			this.Translation = Vector3d.Zero;
			this.Rotation = Quaterniond.Identity;
		}

		public void Solve(bool bUpdate = false)
		{
			if (!bUpdate)
			{
				this.is_initialized = false;
			}
			if (!this.is_initialized)
			{
				this.initialize();
				this.is_initialized = true;
			}
			this.update_from();
			this.update_to();
			this.LastError = this.measure_error();
			int num = 0;
			int num2 = 5;
			int num3 = 0;
			while (num3 < this.MaxIterations && num < num2)
			{
				if (this.VerboseF != null)
				{
					this.VerboseF(string.Format("[ICP] iter {0} : error {1}", num3, this.LastError));
				}
				this.update_transformation();
				this.update_from();
				this.update_to();
				double num4 = this.measure_error();
				if (Math.Abs(this.LastError - num4) < this.ConvergeTolerance)
				{
					num++;
				}
				else
				{
					this.LastError = num4;
					num = 0;
				}
				num3++;
			}
			this.Converged = (num >= num2);
		}

		public double Error
		{
			get
			{
				return this.LastError;
			}
		}

		public void UpdateVertices(IDeformableMesh target)
		{
			bool hasVertexNormals = target.HasVertexNormals;
			this.update_from();
			foreach (int num in target.VertexIndices())
			{
				int num2 = this.MapV[num];
				target.SetVertex(num, this.From[num2]);
				if (hasVertexNormals)
				{
					target.SetVertexNormal(num, (Vector3f)(this.Rotation * target.GetVertexNormal(num)));
				}
			}
		}

		private void initialize()
		{
			this.From = new Vector3d[this.Source.VertexCount];
			this.To = new Vector3d[this.Source.VertexCount];
			this.Weights = new double[this.Source.VertexCount];
			this.MapV = new int[this.Source.MaxVertexID];
			int num = 0;
			foreach (int num2 in this.Source.VertexIndices())
			{
				this.MapV[num2] = num;
				this.Weights[num] = 1.0;
				this.From[num++] = this.Source.GetVertex(num2);
			}
		}

		private void update_from()
		{
			int num = 0;
			foreach (int i in this.Source.VertexIndices())
			{
				this.Weights[num] = 1.0;
				Vector3d vertex = this.Source.GetVertex(i);
				this.From[num++] = this.Rotation * vertex + this.Translation;
			}
		}

		private void update_to()
		{
			double max_dist = double.MaxValue;
			bool bNormals = this.UseNormals && this.Source.HasVertexNormals;
			gParallel.ForEach<int>(Interval1i.Range(this.From.Length), delegate(int vi)
			{
				int num = this.TargetSurface.FindNearestTriangle(this.From[vi], max_dist);
				if (num == -1)
				{
					this.Weights[vi] = 0.0;
					return;
				}
				DistPoint3Triangle3 distPoint3Triangle = MeshQueries.TriangleDistance(this.TargetSurface.Mesh, num, this.From[vi]);
				if (distPoint3Triangle.DistanceSquared > this.MaxAllowableDistance * this.MaxAllowableDistance)
				{
					this.Weights[vi] = 0.0;
					return;
				}
				this.To[vi] = distPoint3Triangle.TriangleClosest;
				this.Weights[vi] = 1.0;
				if (bNormals)
				{
					Vector3d vector3d = this.Rotation * this.Source.GetVertexNormal(vi);
					Vector3d triNormal = this.TargetSurface.Mesh.GetTriNormal(num);
					double num2 = vector3d.Dot(triNormal);
					if (num2 < 0.0)
					{
						this.Weights[vi] = 0.0;
						return;
					}
					this.Weights[vi] += Math.Sqrt(num2);
				}
			});
		}

		private double measure_error()
		{
			double num = 0.0;
			double num2 = 0.0;
			for (int i = 0; i < this.From.Length; i++)
			{
				num += this.Weights[i] * this.From[i].Distance(this.To[i]);
				num2 += this.Weights[i];
			}
			return num / num2;
		}

		private void update_transformation()
		{
			int num = this.From.Length;
			double num2 = 0.0;
			for (int i = 0; i < num; i++)
			{
				num2 += this.Weights[i];
			}
			double num3 = 1.0 / num2;
			Vector3d vector3d = Vector3d.Zero;
			Vector3d vector3d2 = Vector3d.Zero;
			for (int j = 0; j < num; j++)
			{
				vector3d += this.Weights[j] * num3 * this.From[j];
				vector3d2 += this.Weights[j] * num3 * this.To[j];
			}
			for (int k = 0; k < num; k++)
			{
				this.From[k] -= vector3d;
				this.To[k] -= vector3d2;
			}
			double[] array = new double[9];
			for (int l = 0; l < 3; l++)
			{
				int num4 = 3 * l;
				for (int m = 0; m < num; m++)
				{
					double num5 = this.Weights[m] * num3 * this.From[m][l];
					array[num4] += num5 * this.To[m].x;
					array[num4 + 1] += num5 * this.To[m].y;
					array[num4 + 2] += num5 * this.To[m].z;
				}
			}
			SingularValueDecomposition singularValueDecomposition = new SingularValueDecomposition(3, 3, 100);
			singularValueDecomposition.Solve(array, -1);
			double[] array2 = new double[9];
			double[] array3 = new double[9];
			double[] array4 = new double[9];
			singularValueDecomposition.GetU(array2);
			singularValueDecomposition.GetV(array3);
			double[] array5 = new double[9];
			double num6 = MatrixUtil.Determinant3x3(array2);
			double num7 = MatrixUtil.Determinant3x3(array3);
			if (num6 * num7 < 0.0)
			{
				double[] b = MatrixUtil.MakeDiagonal3x3(1.0, 1.0, -1.0);
				MatrixUtil.Multiply3x3(array3, b, array4);
				MatrixUtil.Transpose3x3(array2);
				MatrixUtil.Multiply3x3(array4, array2, array5);
			}
			else
			{
				MatrixUtil.Transpose3x3(array2);
				MatrixUtil.Multiply3x3(array3, array2, array5);
			}
			Matrix3d mat = new Matrix3d(array5);
			Quaterniond quaterniond = new Quaterniond(mat);
			Vector3d v = vector3d2 - quaterniond * vector3d;
			this.Translation += v;
			this.Rotation = quaterniond * this.Rotation;
		}

		public IPointSet Source;

		public DMeshAABBTree3 TargetSurface;

		public Vector3d Translation;

		public Quaterniond Rotation;

		public Action<string> VerboseF;

		public int MaxIterations = 50;

		public bool UseNormals;

		public double MaxAllowableDistance = double.MaxValue;

		public double ConvergeTolerance = 1E-05;

		public bool Converged;

		private bool is_initialized;

		private int[] MapV;

		private Vector3d[] From;

		private Vector3d[] To;

		private double[] Weights;

		private double LastError;
	}
}
