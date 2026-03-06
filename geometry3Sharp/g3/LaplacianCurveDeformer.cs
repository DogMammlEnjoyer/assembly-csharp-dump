using System;
using System.Collections.Generic;

namespace g3
{
	public class LaplacianCurveDeformer
	{
		public LaplacianCurveDeformer(DCurve3 curve)
		{
			this.Curve = curve;
		}

		public void SetConstraint(int vID, Vector3d targetPos, double weight, bool bForceToFixedPos = false)
		{
			this.SoftConstraints[vID] = new LaplacianCurveDeformer.SoftConstraintV
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
			int vertexCount = this.Curve.VertexCount;
			this.ToCurveV = new int[vertexCount];
			this.ToIndex = new int[vertexCount];
			this.N = 0;
			for (int i = 0; i < vertexCount; i++)
			{
				int num = i;
				this.ToCurveV[this.N] = num;
				this.ToIndex[num] = this.N;
				this.N++;
			}
			this.Px = new double[this.N];
			this.Py = new double[this.N];
			this.Pz = new double[this.N];
			this.nbr_counts = new int[this.N];
			SymmetricSparseMatrix symmetricSparseMatrix = new SymmetricSparseMatrix(0);
			for (int j = 0; j < this.N; j++)
			{
				int i2 = this.ToCurveV[j];
				Vector3d vertex = this.Curve.GetVertex(i2);
				this.Px[j] = vertex.x;
				this.Py[j] = vertex.y;
				this.Pz[j] = vertex.z;
				this.nbr_counts[j] = ((j == 0 || j == this.N - 1) ? 1 : 2);
			}
			for (int k = 0; k < this.N; k++)
			{
				int num2 = this.ToCurveV[k];
				int num3 = this.nbr_counts[k];
				Index2i index2i = this.Curve.Neighbours(num2);
				double num4 = 0.0;
				for (int l = 0; l < 2; l++)
				{
					int num5 = index2i[l];
					if (num5 != -1)
					{
						int num6 = this.ToIndex[num5];
						int num7 = this.nbr_counts[num6];
						double num8 = -1.0;
						symmetricSparseMatrix.Set(k, num6, num8);
						num4 += num8;
					}
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
			foreach (KeyValuePair<int, LaplacianCurveDeformer.SoftConstraintV> keyValuePair in this.SoftConstraints)
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
			int k;
			Action<double[], double[]> multiplyF = delegate(double[] X, double[] B)
			{
				this.PackedM.Multiply_Parallel(X, B);
				for (int k = 0; k < this.N; k++)
				{
					B[k] += this.WeightsM[k, k] * X[k];
				}
			};
			List<SparseSymmetricCG> Solvers = new List<SparseSymmetricCG>();
			if (this.SolveX)
			{
				Solvers.Add(new SparseSymmetricCG
				{
					B = this.Bx,
					X = this.Sx,
					MultiplyF = multiplyF,
					PreconditionMultiplyF = new Action<double[], double[]>(this.Preconditioner.Multiply),
					UseXAsInitialGuess = true
				});
			}
			if (this.SolveY)
			{
				Solvers.Add(new SparseSymmetricCG
				{
					B = this.By,
					X = this.Sy,
					MultiplyF = multiplyF,
					PreconditionMultiplyF = new Action<double[], double[]>(this.Preconditioner.Multiply),
					UseXAsInitialGuess = true
				});
			}
			if (this.SolveZ)
			{
				Solvers.Add(new SparseSymmetricCG
				{
					B = this.Bz,
					X = this.Sz,
					MultiplyF = multiplyF,
					PreconditionMultiplyF = new Action<double[], double[]>(this.Preconditioner.Multiply),
					UseXAsInitialGuess = true
				});
			}
			bool[] ok = new bool[Solvers.Count];
			gParallel.ForEach<int>(Interval1i.Range(Solvers.Count), delegate(int i)
			{
				ok[i] = Solvers[i].Solve();
			});
			this.ConvergeFailed = false;
			bool[] ok2 = ok;
			for (k = 0; k < ok2.Length; k++)
			{
				if (!ok2[k])
				{
					this.ConvergeFailed = true;
				}
			}
			for (int j = 0; j < this.N; j++)
			{
				int num = this.ToCurveV[j];
				Result[num] = new Vector3d(this.Sx[j], this.Sy[j], this.Sz[j]);
			}
			if (this.HavePostFixedConstraints)
			{
				foreach (KeyValuePair<int, LaplacianCurveDeformer.SoftConstraintV> keyValuePair in this.SoftConstraints)
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
				int num = this.ToCurveV[i];
				Result[num] = new Vector3d(array[0][i], array[1][i], array[2][i]);
			}
			if (this.HavePostFixedConstraints)
			{
				foreach (KeyValuePair<int, LaplacianCurveDeformer.SoftConstraintV> keyValuePair in this.SoftConstraints)
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
			if (this.Curve.VertexCount < 10000)
			{
				return this.SolveMultipleCG(Result);
			}
			return this.SolveMultipleRHS(Result);
		}

		public bool SolveAndUpdateCurve()
		{
			int vertexCount = this.Curve.VertexCount;
			Vector3d[] array = new Vector3d[vertexCount];
			if (!this.Solve(array))
			{
				return false;
			}
			for (int i = 0; i < vertexCount; i++)
			{
				this.Curve[i] = array[i];
			}
			return true;
		}

		public DCurve3 Curve;

		public bool SolveX = true;

		public bool SolveY = true;

		public bool SolveZ = true;

		public bool ConvergeFailed;

		private PackedSparseMatrix PackedM;

		private int N;

		private int[] ToCurveV;

		private int[] ToIndex;

		private double[] Px;

		private double[] Py;

		private double[] Pz;

		private int[] nbr_counts;

		private double[] MLx;

		private double[] MLy;

		private double[] MLz;

		private Dictionary<int, LaplacianCurveDeformer.SoftConstraintV> SoftConstraints = new Dictionary<int, LaplacianCurveDeformer.SoftConstraintV>();

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
