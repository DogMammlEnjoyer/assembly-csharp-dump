using System;
using System.Collections.Generic;

namespace g3
{
	public class LaplacianMeshDeformer
	{
		public LaplacianMeshDeformer(DMesh3 mesh)
		{
			this.Mesh = mesh;
		}

		public void SetConstraint(int vID, Vector3d targetPos, double weight, bool bForceToFixedPos = false)
		{
			this.SoftConstraints[vID] = new LaplacianMeshDeformer.SoftConstraintV
			{
				Position = targetPos,
				Weight = weight,
				PostFix = bForceToFixedPos
			};
			this.HavePostFixedConstraints = (this.HavePostFixedConstraints || bForceToFixedPos);
			this.need_solve_update = true;
		}

		public bool IsConstrained(int vID)
		{
			return this.SoftConstraints.ContainsKey(vID);
		}

		public void ClearConstraints()
		{
			this.SoftConstraints.Clear();
			this.HavePostFixedConstraints = false;
			this.need_solve_update = true;
		}

		public void Initialize()
		{
			this.ToMeshV = new int[this.Mesh.MaxVertexID];
			this.ToIndex = new int[this.Mesh.MaxVertexID];
			this.N = 0;
			foreach (int num in this.Mesh.VertexIndices())
			{
				this.ToMeshV[this.N] = num;
				this.ToIndex[num] = this.N;
				this.N++;
			}
			this.Px = new double[this.N];
			this.Py = new double[this.N];
			this.Pz = new double[this.N];
			this.nbr_counts = new int[this.N];
			SymmetricSparseMatrix symmetricSparseMatrix = new SymmetricSparseMatrix(0);
			for (int i = 0; i < this.N; i++)
			{
				int vID = this.ToMeshV[i];
				Vector3d vertex = this.Mesh.GetVertex(vID);
				this.Px[i] = vertex.x;
				this.Py[i] = vertex.y;
				this.Pz[i] = vertex.z;
				this.nbr_counts[i] = this.Mesh.GetVtxEdgeCount(vID);
			}
			for (int j = 0; j < this.N; j++)
			{
				int num2 = this.ToMeshV[j];
				int num3 = this.nbr_counts[j];
				double num4 = 0.0;
				foreach (int num5 in this.Mesh.VtxVerticesItr(num2))
				{
					int num6 = this.ToIndex[num5];
					int num7 = this.nbr_counts[num6];
					double num8 = -1.0 / Math.Sqrt((double)(num3 + num7));
					symmetricSparseMatrix.Set(j, num6, num8);
					num4 += num8;
				}
				num4 = -num4;
				symmetricSparseMatrix.Set(num2, num2, num4);
			}
			if (this.UseSoftConstraintNormalEquations)
			{
				this.PackedM = symmetricSparseMatrix.SquarePackedParallel();
			}
			else
			{
				this.PackedM = new PackedSparseMatrix(symmetricSparseMatrix, false);
			}
			this.MLx = new double[this.N];
			this.MLy = new double[this.N];
			this.MLz = new double[this.N];
			this.PackedM.Multiply(this.Px, this.MLx);
			this.PackedM.Multiply(this.Py, this.MLy);
			this.PackedM.Multiply(this.Pz, this.MLz);
			this.Preconditioner = new DiagonalMatrix(this.N);
			this.WeightsM = new DiagonalMatrix(this.N);
			this.Cx = new double[this.N];
			this.Cy = new double[this.N];
			this.Cz = new double[this.N];
			this.Bx = new double[this.N];
			this.By = new double[this.N];
			this.Bz = new double[this.N];
			this.Sx = new double[this.N];
			this.Sy = new double[this.N];
			this.Sz = new double[this.N];
			this.need_solve_update = true;
			this.UpdateForSolve();
		}

		private void UpdateForSolve()
		{
			if (!this.need_solve_update)
			{
				return;
			}
			this.WeightsM.Clear();
			Array.Clear(this.Cx, 0, this.N);
			Array.Clear(this.Cy, 0, this.N);
			Array.Clear(this.Cz, 0, this.N);
			foreach (KeyValuePair<int, LaplacianMeshDeformer.SoftConstraintV> keyValuePair in this.SoftConstraints)
			{
				int key = keyValuePair.Key;
				int num = this.ToIndex[key];
				double num2 = keyValuePair.Value.Weight;
				if (this.UseSoftConstraintNormalEquations)
				{
					num2 *= num2;
				}
				this.WeightsM.Set(num, num, num2);
				Vector3d position = keyValuePair.Value.Position;
				this.Cx[num] = num2 * position.x;
				this.Cy[num] = num2 * position.y;
				this.Cz[num] = num2 * position.z;
			}
			for (int i = 0; i < this.N; i++)
			{
				this.Bx[i] = this.MLx[i] + this.Cx[i];
				this.By[i] = this.MLy[i] + this.Cy[i];
				this.Bz[i] = this.MLz[i] + this.Cz[i];
			}
			for (int j = 0; j < this.N; j++)
			{
				double num3 = this.PackedM[j, j] + this.WeightsM[j, j];
				this.Preconditioner.Set(j, j, 1.0 / num3);
			}
			this.need_solve_update = false;
		}

		public bool SolveMultipleCG(Vector3d[] Result)
		{
			if (this.WeightsM == null)
			{
				this.Initialize();
			}
			this.UpdateForSolve();
			Array.Copy(this.Px, this.Sx, this.N);
			Array.Copy(this.Py, this.Sy, this.N);
			Array.Copy(this.Pz, this.Sz, this.N);
			int j;
			Action<double[], double[]> multiplyF = delegate(double[] X, double[] B)
			{
				this.PackedM.Multiply_Parallel(X, B);
				for (int j = 0; j < this.N; j++)
				{
					B[j] += this.WeightsM[j, j] * X[j];
				}
			};
			SparseSymmetricCG sparseSymmetricCG = new SparseSymmetricCG
			{
				B = this.Bx,
				X = this.Sx,
				MultiplyF = multiplyF,
				PreconditionMultiplyF = new Action<double[], double[]>(this.Preconditioner.Multiply),
				UseXAsInitialGuess = true
			};
			SparseSymmetricCG sparseSymmetricCG2 = new SparseSymmetricCG
			{
				B = this.By,
				X = this.Sy,
				MultiplyF = multiplyF,
				PreconditionMultiplyF = new Action<double[], double[]>(this.Preconditioner.Multiply),
				UseXAsInitialGuess = true
			};
			SparseSymmetricCG sparseSymmetricCG3 = new SparseSymmetricCG
			{
				B = this.Bz,
				X = this.Sz,
				MultiplyF = multiplyF,
				PreconditionMultiplyF = new Action<double[], double[]>(this.Preconditioner.Multiply),
				UseXAsInitialGuess = true
			};
			SparseSymmetricCG[] solvers = new SparseSymmetricCG[]
			{
				sparseSymmetricCG,
				sparseSymmetricCG2,
				sparseSymmetricCG3
			};
			bool[] ok = new bool[3];
			IEnumerable<int> source = new int[]
			{
				0,
				1,
				2
			};
			Action<int> body = delegate(int i)
			{
				ok[i] = solvers[i].Solve();
			};
			gParallel.ForEach<int>(source, body);
			if (!ok[0] || !ok[1] || !ok[2])
			{
				return false;
			}
			for (j = 0; j < this.N; j++)
			{
				int num = this.ToMeshV[j];
				Result[num] = new Vector3d(this.Sx[j], this.Sy[j], this.Sz[j]);
			}
			if (this.HavePostFixedConstraints)
			{
				foreach (KeyValuePair<int, LaplacianMeshDeformer.SoftConstraintV> keyValuePair in this.SoftConstraints)
				{
					if (keyValuePair.Value.PostFix)
					{
						int key = keyValuePair.Key;
						Result[key] = keyValuePair.Value.Position;
					}
				}
			}
			return true;
		}

		public bool SolveMultipleRHS(Vector3d[] Result)
		{
			if (this.WeightsM == null)
			{
				this.Initialize();
			}
			this.UpdateForSolve();
			double[][] b = BufferUtil.InitNxM(3, this.N, new double[][]
			{
				this.Bx,
				this.By,
				this.Bz
			});
			double[][] array = BufferUtil.InitNxM(3, this.N, new double[][]
			{
				this.Px,
				this.Py,
				this.Pz
			});
			Action<double[][], double[][]> multiplyF = delegate(double[][] Xt, double[][] Bt)
			{
				this.PackedM.Multiply_Parallel_3(Xt, Bt);
				gParallel.ForEach<int>(Interval1i.Range(3), delegate(int j)
				{
					BufferUtil.MultiplyAdd(Bt[j], this.WeightsM.D, Xt[j]);
				});
			};
			if (!new SparseSymmetricCGMultipleRHS
			{
				B = b,
				X = array,
				MultiplyF = multiplyF,
				PreconditionMultiplyF = null,
				UseXAsInitialGuess = true
			}.Solve())
			{
				return false;
			}
			for (int i = 0; i < this.N; i++)
			{
				int num = this.ToMeshV[i];
				Result[num] = new Vector3d(array[0][i], array[1][i], array[2][i]);
			}
			if (this.HavePostFixedConstraints)
			{
				foreach (KeyValuePair<int, LaplacianMeshDeformer.SoftConstraintV> keyValuePair in this.SoftConstraints)
				{
					if (keyValuePair.Value.PostFix)
					{
						int key = keyValuePair.Key;
						Result[key] = keyValuePair.Value.Position;
					}
				}
			}
			return true;
		}

		public bool Solve(Vector3d[] Result)
		{
			if (this.Mesh.VertexCount < 10000)
			{
				return this.SolveMultipleCG(Result);
			}
			return this.SolveMultipleRHS(Result);
		}

		public bool SolveAndUpdateMesh()
		{
			int maxVertexID = this.Mesh.MaxVertexID;
			Vector3d[] array = new Vector3d[maxVertexID];
			if (!this.Solve(array))
			{
				return false;
			}
			for (int i = 0; i < maxVertexID; i++)
			{
				if (this.Mesh.IsVertex(i))
				{
					this.Mesh.SetVertex(i, array[i]);
				}
			}
			return true;
		}

		public DMesh3 Mesh;

		private PackedSparseMatrix PackedM;

		private int N;

		private int[] ToMeshV;

		private int[] ToIndex;

		private double[] Px;

		private double[] Py;

		private double[] Pz;

		private int[] nbr_counts;

		private double[] MLx;

		private double[] MLy;

		private double[] MLz;

		private Dictionary<int, LaplacianMeshDeformer.SoftConstraintV> SoftConstraints = new Dictionary<int, LaplacianMeshDeformer.SoftConstraintV>();

		private bool HavePostFixedConstraints;

		private bool need_solve_update;

		private DiagonalMatrix WeightsM;

		private double[] Cx;

		private double[] Cy;

		private double[] Cz;

		private double[] Bx;

		private double[] By;

		private double[] Bz;

		private DiagonalMatrix Preconditioner;

		public bool UseSoftConstraintNormalEquations = true;

		private double[] Sx;

		private double[] Sy;

		private double[] Sz;

		public struct SoftConstraintV
		{
			public Vector3d Position;

			public double Weight;

			public bool PostFix;
		}
	}
}
