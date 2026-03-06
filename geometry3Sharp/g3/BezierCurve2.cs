using System;

namespace g3
{
	public class BezierCurve2 : BaseCurve2, IParametricCurve2d
	{
		public int Degree
		{
			get
			{
				return this.mDegree;
			}
		}

		public Vector2d[] ControlPoints
		{
			get
			{
				return this.mCtrlPoint;
			}
		}

		public BezierCurve2(int degree, Vector2d[] ctrlPoint, bool bTakeOwnership = false) : base(0.0, 1.0)
		{
			if (degree < 2)
			{
				throw new Exception("BezierCurve2() The degree must be three or larger\n");
			}
			this.mDegree = degree;
			this.mNumCtrlPoints = this.mDegree + 1;
			if (bTakeOwnership)
			{
				this.mCtrlPoint = ctrlPoint;
			}
			else
			{
				this.mCtrlPoint = new Vector2d[ctrlPoint.Length];
				Array.Copy(ctrlPoint, this.mCtrlPoint, ctrlPoint.Length);
			}
			this.mDer1CtrlPoint = new Vector2d[this.mNumCtrlPoints - 1];
			for (int i = 0; i < this.mNumCtrlPoints - 1; i++)
			{
				this.mDer1CtrlPoint[i] = this.mCtrlPoint[i + 1] - this.mCtrlPoint[i];
			}
			this.mDer2CtrlPoint = new Vector2d[this.mNumCtrlPoints - 2];
			for (int i = 0; i < this.mNumCtrlPoints - 2; i++)
			{
				this.mDer2CtrlPoint[i] = this.mDer1CtrlPoint[i + 1] - this.mDer1CtrlPoint[i];
			}
			if (degree >= 3)
			{
				this.mDer3CtrlPoint = new Vector2d[this.mNumCtrlPoints - 3];
				for (int i = 0; i < this.mNumCtrlPoints - 3; i++)
				{
					this.mDer3CtrlPoint[i] = this.mDer2CtrlPoint[i + 1] - this.mDer2CtrlPoint[i];
				}
			}
			else
			{
				this.mDer3CtrlPoint = null;
			}
			this.mChoose = new DenseMatrix(this.mNumCtrlPoints, this.mNumCtrlPoints);
			this.mChoose[0, 0] = 1.0;
			this.mChoose[1, 0] = 1.0;
			this.mChoose[1, 1] = 1.0;
			for (int i = 2; i <= this.mDegree; i++)
			{
				this.mChoose[i, 0] = 1.0;
				this.mChoose[i, i] = 1.0;
				for (int j = 1; j < i; j++)
				{
					this.mChoose[i, j] = this.mChoose[i - 1, j - 1] + this.mChoose[i - 1, j];
				}
			}
		}

		protected BezierCurve2() : base(0.0, 1.0)
		{
		}

		public override Vector2d GetPosition(double t)
		{
			double f = 1.0 - t;
			double num = t;
			Vector2d a = f * this.mCtrlPoint[0];
			for (int i = 1; i < this.mDegree; i++)
			{
				double f2 = this.mChoose[this.mDegree, i] * num;
				a = (a + this.mCtrlPoint[i] * f2) * f;
				num *= t;
			}
			return a + this.mCtrlPoint[this.mDegree] * num;
		}

		public override Vector2d GetFirstDerivative(double t)
		{
			double f = 1.0 - t;
			double num = t;
			Vector2d a = f * this.mDer1CtrlPoint[0];
			int num2 = this.mDegree - 1;
			for (int i = 1; i < num2; i++)
			{
				double f2 = this.mChoose[num2, i] * num;
				a = (a + this.mDer1CtrlPoint[i] * f2) * f;
				num *= t;
			}
			a += this.mDer1CtrlPoint[num2] * num;
			return a * (double)this.mDegree;
		}

		public override Vector2d GetSecondDerivative(double t)
		{
			double f = 1.0 - t;
			double num = t;
			Vector2d a = f * this.mDer2CtrlPoint[0];
			int num2 = this.mDegree - 2;
			for (int i = 1; i < num2; i++)
			{
				double f2 = this.mChoose[num2, i] * num;
				a = (a + this.mDer2CtrlPoint[i] * f2) * f;
				num *= t;
			}
			a += this.mDer2CtrlPoint[num2] * num;
			return a * (double)(this.mDegree * (this.mDegree - 1));
		}

		public override Vector2d GetThirdDerivative(double t)
		{
			if (this.mDegree < 3)
			{
				return Vector2d.Zero;
			}
			double f = 1.0 - t;
			double num = t;
			Vector2d a = f * this.mDer3CtrlPoint[0];
			int num2 = this.mDegree - 3;
			for (int i = 1; i < num2; i++)
			{
				double f2 = this.mChoose[num2, i] * num;
				a = (a + this.mDer3CtrlPoint[i] * f2) * f;
				num *= t;
			}
			a += this.mDer3CtrlPoint[num2] * num;
			return a * (double)(this.mDegree * (this.mDegree - 1) * (this.mDegree - 2));
		}

		public bool IsClosed
		{
			get
			{
				return false;
			}
		}

		public double ParamLength
		{
			get
			{
				return this.mTMax - this.mTMin;
			}
		}

		public Vector2d SampleT(double t)
		{
			return this.GetPosition(t);
		}

		public Vector2d TangentT(double t)
		{
			return this.GetFirstDerivative(t).Normalized;
		}

		public bool HasArcLength
		{
			get
			{
				return true;
			}
		}

		public double ArcLength
		{
			get
			{
				return base.GetTotalLength();
			}
		}

		public Vector2d SampleArcLength(double a)
		{
			double time = this.GetTime(a, 32, 1E-06);
			return this.GetPosition(time);
		}

		public void Reverse()
		{
			throw new NotSupportedException("NURBSCurve2.Reverse: how to reverse?!?");
		}

		public IParametricCurve2d Clone()
		{
			return new BezierCurve2
			{
				mDegree = this.mDegree,
				mNumCtrlPoints = this.mNumCtrlPoints,
				mCtrlPoint = (Vector2d[])this.mCtrlPoint.Clone(),
				mDer1CtrlPoint = (Vector2d[])this.mDer1CtrlPoint.Clone(),
				mDer2CtrlPoint = (Vector2d[])this.mDer2CtrlPoint.Clone(),
				mDer3CtrlPoint = (Vector2d[])this.mDer3CtrlPoint.Clone(),
				mChoose = new DenseMatrix(this.mChoose)
			};
		}

		public bool IsTransformable
		{
			get
			{
				return true;
			}
		}

		public void Transform(ITransform2 xform)
		{
			for (int i = 0; i < this.mCtrlPoint.Length; i++)
			{
				this.mCtrlPoint[i] = xform.TransformP(this.mCtrlPoint[i]);
			}
			for (int j = 0; j < this.mNumCtrlPoints - 1; j++)
			{
				this.mDer1CtrlPoint[j] = this.mCtrlPoint[j + 1] - this.mCtrlPoint[j];
			}
			for (int k = 0; k < this.mNumCtrlPoints - 2; k++)
			{
				this.mDer2CtrlPoint[k] = this.mDer1CtrlPoint[k + 1] - this.mDer1CtrlPoint[k];
			}
			if (this.mDegree >= 3)
			{
				for (int l = 0; l < this.mNumCtrlPoints - 3; l++)
				{
					this.mDer3CtrlPoint[l] = this.mDer2CtrlPoint[l + 1] - this.mDer2CtrlPoint[l];
				}
			}
		}

		private int mDegree;

		private int mNumCtrlPoints;

		private Vector2d[] mCtrlPoint;

		private Vector2d[] mDer1CtrlPoint;

		private Vector2d[] mDer2CtrlPoint;

		private Vector2d[] mDer3CtrlPoint;

		private DenseMatrix mChoose;
	}
}
