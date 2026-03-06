using System;
using System.Collections.Generic;
using System.Linq;

namespace g3
{
	public class NURBSCurve2 : BaseCurve2, IParametricCurve2d
	{
		public NURBSCurve2(int numCtrlPoints, Vector2d[] ctrlPoint, double[] ctrlWeight, int degree, bool loop, bool open) : base(0.0, 1.0)
		{
			if (numCtrlPoints < 2)
			{
				throw new Exception("NURBSCurve2(): only received " + numCtrlPoints.ToString() + " control points!");
			}
			if (degree < 1 || degree > numCtrlPoints - 1)
			{
				throw new Exception("NURBSCurve2(): invalid degree " + degree.ToString());
			}
			this.mLoop = loop;
			this.mNumCtrlPoints = numCtrlPoints;
			this.mReplicate = (loop ? (open ? 1 : degree) : 0);
			this.CreateControl(ctrlPoint, ctrlWeight);
			this.mBasis = new BSplineBasis(this.mNumCtrlPoints + this.mReplicate, degree, open);
		}

		public NURBSCurve2(int numCtrlPoints, Vector2d[] ctrlPoint, double[] ctrlWeight, int degree, bool loop, double[] knot, bool bIsInteriorKnot = true) : base(0.0, 1.0)
		{
			if (numCtrlPoints < 2)
			{
				throw new Exception("NURBSCurve2(): only received " + numCtrlPoints.ToString() + " control points!");
			}
			if (degree < 1 || degree > numCtrlPoints - 1)
			{
				throw new Exception("NURBSCurve2(): invalid degree " + degree.ToString());
			}
			if (loop)
			{
				throw new Exception("NURBSCUrve2(): loop mode is broken?");
			}
			this.mLoop = loop;
			this.mNumCtrlPoints = numCtrlPoints;
			this.mReplicate = (loop ? 1 : 0);
			this.CreateControl(ctrlPoint, ctrlWeight);
			this.mBasis = new BSplineBasis(this.mNumCtrlPoints + this.mReplicate, degree, knot, bIsInteriorKnot);
		}

		protected NURBSCurve2() : base(0.0, 1.0)
		{
		}

		public int GetNumCtrlPoints()
		{
			return this.mNumCtrlPoints;
		}

		public int GetDegree()
		{
			return this.mBasis.GetDegree();
		}

		public bool IsUniform()
		{
			return this.mBasis.IsUniform();
		}

		public void SetControlPoint(int i, Vector2d ctrl)
		{
			if (0 <= i && i < this.mNumCtrlPoints)
			{
				this.mCtrlPoint[i] = ctrl;
				if (i < this.mReplicate)
				{
					this.mCtrlPoint[this.mNumCtrlPoints + i] = ctrl;
				}
			}
		}

		public Vector2d GetControlPoint(int i)
		{
			if (0 <= i && i < this.mNumCtrlPoints)
			{
				return this.mCtrlPoint[i];
			}
			return new Vector2d(double.MaxValue, double.MaxValue);
		}

		public void SetControlWeight(int i, double weight)
		{
			if (0 <= i && i < this.mNumCtrlPoints)
			{
				this.mCtrlWeight[i] = weight;
				if (i < this.mReplicate)
				{
					this.mCtrlWeight[this.mNumCtrlPoints + i] = weight;
				}
			}
		}

		public double GetControlWeight(int i)
		{
			if (0 <= i && i < this.mNumCtrlPoints)
			{
				return this.mCtrlWeight[i];
			}
			return double.MaxValue;
		}

		public void SetKnot(int i, double value)
		{
			this.mBasis.SetInteriorKnot(i, value);
		}

		public double GetKnot(int i)
		{
			return this.mBasis.GetInteriorKnot(i);
		}

		public override Vector2d GetPosition(double t)
		{
			int num = 0;
			int num2 = 0;
			this.mBasis.Compute(t, 0, ref num, ref num2);
			if (num2 >= this.mCtrlWeight.Length)
			{
				num2 = this.mCtrlWeight.Length - 1;
			}
			Vector2d a = Vector2d.Zero;
			double num3 = 0.0;
			for (int i = num; i <= num2; i++)
			{
				double num4 = this.mBasis.GetD0(i) * this.mCtrlWeight[i];
				a += num4 * this.mCtrlPoint[i];
				num3 += num4;
			}
			return 1.0 / num3 * a;
		}

		public override Vector2d GetFirstDerivative(double t)
		{
			int num = 0;
			int num2 = 0;
			this.mBasis.Compute(t, 0, ref num, ref num2);
			this.mBasis.Compute(t, 1, ref num, ref num2);
			if (num2 >= this.mCtrlWeight.Length)
			{
				num2 = this.mCtrlWeight.Length - 1;
			}
			Vector2d a = Vector2d.Zero;
			double num3 = 0.0;
			for (int i = num; i <= num2; i++)
			{
				double num4 = this.mBasis.GetD0(i) * this.mCtrlWeight[i];
				a += num4 * this.mCtrlPoint[i];
				num3 += num4;
			}
			double f = 1.0 / num3;
			Vector2d a2 = f * a;
			Vector2d a3 = Vector2d.Zero;
			double num5 = 0.0;
			for (int i = num; i <= num2; i++)
			{
				double num4 = this.mBasis.GetD1(i) * this.mCtrlWeight[i];
				a3 += num4 * this.mCtrlPoint[i];
				num5 += num4;
			}
			return f * (a3 - num5 * a2);
		}

		public override Vector2d GetSecondDerivative(double t)
		{
			NURBSCurve2.CurveDerivatives curveDerivatives = default(NURBSCurve2.CurveDerivatives);
			curveDerivatives.init(false, false, true, false);
			this.Get(t, ref curveDerivatives);
			return curveDerivatives.d2;
		}

		public override Vector2d GetThirdDerivative(double t)
		{
			NURBSCurve2.CurveDerivatives curveDerivatives = default(NURBSCurve2.CurveDerivatives);
			curveDerivatives.init(false, false, false, true);
			this.Get(t, ref curveDerivatives);
			return curveDerivatives.d3;
		}

		public void Get(double t, ref NURBSCurve2.CurveDerivatives result)
		{
			int num = 0;
			int num2 = 0;
			if (result.bDer3)
			{
				this.mBasis.Compute(t, 0, ref num, ref num2);
				this.mBasis.Compute(t, 1, ref num, ref num2);
				this.mBasis.Compute(t, 2, ref num, ref num2);
				this.mBasis.Compute(t, 3, ref num, ref num2);
			}
			else if (result.bDer2)
			{
				this.mBasis.Compute(t, 0, ref num, ref num2);
				this.mBasis.Compute(t, 1, ref num, ref num2);
				this.mBasis.Compute(t, 2, ref num, ref num2);
			}
			else if (result.bDer1)
			{
				this.mBasis.Compute(t, 0, ref num, ref num2);
				this.mBasis.Compute(t, 1, ref num, ref num2);
			}
			else
			{
				this.mBasis.Compute(t, 0, ref num, ref num2);
			}
			if (num2 >= this.mCtrlWeight.Length)
			{
				num2 = this.mCtrlWeight.Length - 1;
			}
			Vector2d a = Vector2d.Zero;
			double num3 = 0.0;
			for (int i = num; i <= num2; i++)
			{
				double num4 = this.mBasis.GetD0(i) * this.mCtrlWeight[i];
				a += num4 * this.mCtrlPoint[i];
				num3 += num4;
			}
			double f = 1.0 / num3;
			Vector2d vector2d = f * a;
			result.p = vector2d;
			result.bPosition = true;
			if (!result.bDer1 && !result.bDer2 && !result.bDer3)
			{
				return;
			}
			Vector2d a2 = Vector2d.Zero;
			double num5 = 0.0;
			for (int i = num; i <= num2; i++)
			{
				double num4 = this.mBasis.GetD1(i) * this.mCtrlWeight[i];
				a2 += num4 * this.mCtrlPoint[i];
				num5 += num4;
			}
			Vector2d vector2d2 = f * (a2 - num5 * vector2d);
			result.d1 = vector2d2;
			result.bDer1 = true;
			if (!result.bDer2 && !result.bDer3)
			{
				return;
			}
			Vector2d a3 = Vector2d.Zero;
			double num6 = 0.0;
			for (int i = num; i <= num2; i++)
			{
				double num4 = this.mBasis.GetD2(i) * this.mCtrlWeight[i];
				a3 += num4 * this.mCtrlPoint[i];
				num6 += num4;
			}
			Vector2d vector2d3 = f * (a3 - 2.0 * num5 * vector2d2 - num6 * vector2d);
			result.d2 = vector2d3;
			result.bDer2 = true;
			if (!result.bDer3)
			{
				return;
			}
			Vector2d a4 = Vector2d.Zero;
			double num7 = 0.0;
			for (int i = num; i <= num2; i++)
			{
				double num4 = this.mBasis.GetD3(i) * this.mCtrlWeight[i];
				a4 += num4 * this.mCtrlPoint[i];
				num7 += num4;
			}
			result.d3 = f * (a4 - 3.0 * num5 * vector2d3 - 3.0 * num6 * vector2d2 - num7 * vector2d);
		}

		public BSplineBasis GetBasis()
		{
			return this.mBasis;
		}

		protected void CreateControl(Vector2d[] ctrlPoint, double[] ctrlWeight)
		{
			int num = this.mNumCtrlPoints + this.mReplicate;
			this.mCtrlPoint = new Vector2d[num];
			Array.Copy(ctrlPoint, this.mCtrlPoint, this.mNumCtrlPoints);
			this.mCtrlWeight = new double[num];
			Array.Copy(ctrlWeight, this.mCtrlWeight, this.mNumCtrlPoints);
			for (int i = 0; i < this.mReplicate; i++)
			{
				this.mCtrlPoint[this.mNumCtrlPoints + i] = ctrlPoint[i];
				this.mCtrlWeight[this.mNumCtrlPoints + i] = ctrlWeight[i];
			}
		}

		public bool IsClosed
		{
			get
			{
				return this.is_closed;
			}
			set
			{
				this.is_closed = value;
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
			return new NURBSCurve2
			{
				mNumCtrlPoints = this.mNumCtrlPoints,
				mCtrlPoint = (Vector2d[])this.mCtrlPoint.Clone(),
				mCtrlWeight = (double[])this.mCtrlWeight.Clone(),
				mLoop = this.mLoop,
				mBasis = this.mBasis.Clone(),
				mReplicate = this.mReplicate,
				is_closed = this.is_closed
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
		}

		public List<double> GetParamIntervals()
		{
			List<double> list = new List<double>();
			list.Add(0.0);
			for (int i = 0; i < this.mBasis.KnotCount; i++)
			{
				double knot = this.mBasis.GetKnot(i);
				if (knot != list.Last<double>())
				{
					list.Add(knot);
				}
			}
			if (list.Last<double>() != 1.0)
			{
				list.Add(1.0);
			}
			return list;
		}

		public List<double> GetContinuousParamIntervals()
		{
			List<double> list = new List<double>();
			double num = -1.0;
			int num2 = 0;
			for (int i = 0; i < this.mBasis.KnotCount; i++)
			{
				double knot = this.mBasis.GetKnot(i);
				if (knot == num)
				{
					num2++;
				}
				else
				{
					if (num2 > 1)
					{
						list.Add(num);
					}
					num = knot;
					num2 = 1;
				}
			}
			if (list.Last<double>() != 1.0)
			{
				list.Add(1.0);
			}
			return list;
		}

		protected int mNumCtrlPoints;

		protected Vector2d[] mCtrlPoint;

		protected double[] mCtrlWeight;

		protected bool mLoop;

		protected BSplineBasis mBasis;

		protected int mReplicate;

		protected bool is_closed;

		public struct CurveDerivatives
		{
			public void init()
			{
				this.bPosition = (this.bDer1 = (this.bDer2 = (this.bDer3 = false)));
			}

			public void init(bool pos, bool der1, bool der2, bool der3)
			{
				this.bPosition = pos;
				this.bDer1 = der1;
				this.bDer2 = der2;
				this.bDer3 = der3;
			}

			public Vector2d p;

			public Vector2d d1;

			public Vector2d d2;

			public Vector2d d3;

			public bool bPosition;

			public bool bDer1;

			public bool bDer2;

			public bool bDer3;
		}
	}
}
