using System;

namespace g3
{
	public class InPlaceIterativeCurveSmooth
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

		public int Start
		{
			get
			{
				return this._startRange;
			}
			set
			{
				this._startRange = value;
			}
		}

		public int End
		{
			get
			{
				return this._endRange;
			}
			set
			{
				this._endRange = value;
			}
		}

		public float Alpha
		{
			get
			{
				return this._alpha;
			}
			set
			{
				this._alpha = MathUtil.Clamp(value, 0f, 1f);
			}
		}

		public InPlaceIterativeCurveSmooth()
		{
			this.Start = (this.End = -1);
			this.Alpha = 0.25f;
		}

		public InPlaceIterativeCurveSmooth(DCurve3 curve, float alpha = 0.25f)
		{
			this.Curve = curve;
			this.Start = 0;
			this.End = this.Curve.VertexCount;
			this.Alpha = alpha;
		}

		public void UpdateDeformation(int nIterations = 1)
		{
			if (this.Curve.Closed)
			{
				this.UpdateDeformation_Closed(nIterations);
				return;
			}
			this.UpdateDeformation_Open(nIterations);
		}

		public void UpdateDeformation_Closed(int nIterations = 1)
		{
			if (this.Start < 0 || this.Start > this.Curve.VertexCount || this.End > this.Curve.VertexCount)
			{
				throw new ArgumentOutOfRangeException("InPlaceIterativeCurveSmooth.UpdateDeformation: range is invalid");
			}
			int vertexCount = this.Curve.VertexCount;
			for (int i = 0; i < nIterations; i++)
			{
				for (int j = this.Start; j < this.End; j++)
				{
					int key = j % vertexCount;
					int key2 = (j == 0) ? (vertexCount - 1) : (j - 1);
					int key3 = (j + 1) % vertexCount;
					Vector3d v = this.Curve[key2];
					Vector3d v2 = this.Curve[key3];
					Vector3d v3 = (v + v2) * 0.5;
					this.Curve[key] = (double)(1f - this.Alpha) * this.Curve[key] + (double)this.Alpha * v3;
				}
			}
		}

		public void UpdateDeformation_Open(int nIterations = 1)
		{
			if (this.Start < 0 || this.Start > this.Curve.VertexCount || this.End > this.Curve.VertexCount)
			{
				throw new ArgumentOutOfRangeException("InPlaceIterativeCurveSmooth.UpdateDeformation: range is invalid");
			}
			for (int i = 0; i < nIterations; i++)
			{
				for (int j = this.Start; j <= this.End; j++)
				{
					if (j != 0 && j < this.Curve.VertexCount - 1)
					{
						Vector3d v = this.Curve[j - 1];
						Vector3d v2 = this.Curve[j + 1];
						Vector3d v3 = (v + v2) * 0.5;
						this.Curve[j] = (double)(1f - this.Alpha) * this.Curve[j] + (double)this.Alpha * v3;
					}
				}
			}
		}

		private DCurve3 _curve;

		private int _startRange;

		private int _endRange;

		private float _alpha;
	}
}
