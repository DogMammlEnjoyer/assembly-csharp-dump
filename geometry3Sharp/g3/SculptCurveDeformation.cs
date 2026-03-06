using System;

namespace g3
{
	public abstract class SculptCurveDeformation
	{
		public DCurve3 Curve
		{
			get
			{
				return this._curve;
			}
			set
			{
				if (this._curve != value)
				{
					this._curve = value;
				}
			}
		}

		public Func<double, double, double> WeightFunc
		{
			get
			{
				return this._weightfunc;
			}
			set
			{
				if (this._weightfunc != value)
				{
					this._weightfunc = value;
				}
			}
		}

		public double Radius
		{
			get
			{
				return this.radius;
			}
			set
			{
				this.radius = value;
			}
		}

		public SculptCurveDeformation()
		{
			this.WeightFunc = ((double d, double r) => MathUtil.WyvillFalloff01(MathUtil.Clamp(d / r, 0.0, 1.0)));
		}

		public virtual void BeginDeformation(Frame3f vStartPos)
		{
			this.vPreviousPos = vStartPos;
		}

		public virtual SculptCurveDeformation.DeformInfo UpdateDeformation(Frame3f vNextPos)
		{
			SculptCurveDeformation.DeformInfo result = this.Apply(vNextPos);
			this.vPreviousPos = vNextPos;
			return result;
		}

		public abstract SculptCurveDeformation.DeformInfo Apply(Frame3f vNextPos);

		protected DCurve3 _curve;

		protected Func<double, double, double> _weightfunc;

		protected double radius = 0.10000000149011612;

		protected Frame3f vPreviousPos;

		public struct DeformInfo
		{
			public bool bNoChange;

			public double maxEdgeLenSqr;

			public double minEdgeLenSqr;
		}
	}
}
