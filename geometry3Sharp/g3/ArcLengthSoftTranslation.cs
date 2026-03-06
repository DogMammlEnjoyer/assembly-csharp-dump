using System;

namespace g3
{
	public class ArcLengthSoftTranslation
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
					this.invalidate_roi();
				}
			}
		}

		public Vector3d Handle
		{
			get
			{
				return this._handle;
			}
			set
			{
				if (this._handle != value)
				{
					this._handle = value;
					this.invalidate_roi();
				}
			}
		}

		public double ArcRadius
		{
			get
			{
				return this._arcradius;
			}
			set
			{
				if (this._arcradius != value)
				{
					this._arcradius = value;
					this.invalidate_roi();
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
					this.invalidate_roi();
				}
			}
		}

		public ArcLengthSoftTranslation()
		{
			this.Handle = Vector3d.Zero;
			this.ArcRadius = 1.0;
			this.WeightFunc = ((double d, double r) => MathUtil.WyvillFalloff01(MathUtil.Clamp(d / r, 0.0, 1.0)));
			this.roi_valid = false;
		}

		public void BeginDeformation()
		{
			this.UpdateROI(-1);
			this.start_handle = this.Handle;
			if (this.start_positions == null || this.start_positions.Length != this.roi_index.Length)
			{
				this.start_positions = new Vector3d[this.roi_index.Length];
			}
			for (int i = 0; i < this.roi_index.Length; i++)
			{
				this.start_positions[i] = this.Curve.GetVertex(this.roi_index[i]);
			}
		}

		public void UpdateDeformation(Vector3d newHandlePos)
		{
			Vector3d v = newHandlePos - this.start_handle;
			for (int i = 0; i < this.roi_index.Length; i++)
			{
				Vector3d v2 = this.start_positions[i] + this.roi_weights[i] * v;
				this.Curve.SetVertex(this.roi_index[i], v2);
			}
		}

		public void EndDeformation()
		{
		}

		private void invalidate_roi()
		{
			this.roi_valid = false;
		}

		private bool check_roi_valid()
		{
			return this.roi_valid && this.Curve.Timestamp == this.curve_timestamp;
		}

		public void UpdateROI(int nNearVertexHint = -1)
		{
			if (this.check_roi_valid())
			{
				return;
			}
			int num = nNearVertexHint;
			if (nNearVertexHint < 0)
			{
				num = CurveUtils.FindNearestIndex(this.Curve, this.Handle);
			}
			int vertexCount = this.Curve.VertexCount;
			int num2 = 1;
			double num3 = 0.0;
			int num4 = -1;
			int num5 = num + 1;
			while (num5 < vertexCount && num3 < this.ArcRadius)
			{
				double length = (this.Curve.GetVertex(num5) - this.Curve.GetVertex(num5 - 1)).Length;
				num3 += length;
				if (num3 < this.ArcRadius)
				{
					num2++;
					num4 = num5;
				}
				num5++;
			}
			double num6 = 0.0;
			int num7 = -1;
			int num8 = num - 1;
			while (num8 >= 0 && num6 < this.ArcRadius)
			{
				double length2 = (this.Curve.GetVertex(num8) - this.Curve.GetVertex(num8 + 1)).Length;
				num6 += length2;
				if (num6 < this.ArcRadius)
				{
					num2++;
					num7 = num8;
				}
				num8--;
			}
			if (this.roi_index == null || this.roi_index.Length != num2)
			{
				this.roi_index = new int[num2];
				this.roi_weights = new double[num2];
			}
			int num9 = 0;
			this.roi_index[num9] = num;
			this.roi_weights[num9++] = this.WeightFunc(0.0, this.ArcRadius);
			if (num4 >= 0)
			{
				num3 = 0.0;
				for (int i = num + 1; i <= num4; i++)
				{
					num3 += (this.Curve.GetVertex(i) - this.Curve.GetVertex(i - 1)).Length;
					this.roi_index[num9] = i;
					this.roi_weights[num9++] = this.WeightFunc(num3, this.ArcRadius);
				}
			}
			if (num7 >= 0)
			{
				num6 = 0.0;
				for (int j = num - 1; j >= num7; j--)
				{
					num6 += (this.Curve.GetVertex(j) - this.Curve.GetVertex(j + 1)).Length;
					this.roi_index[num9] = j;
					this.roi_weights[num9++] = this.WeightFunc(num6, this.ArcRadius);
				}
			}
			this.roi_valid = true;
			this.curve_timestamp = this.Curve.Timestamp;
		}

		private DCurve3 _curve;

		private Vector3d _handle;

		private double _arcradius;

		private Func<double, double, double> _weightfunc;

		public int[] roi_index;

		public double[] roi_weights;

		public Vector3d[] start_positions;

		private bool roi_valid;

		private int curve_timestamp;

		private Vector3d start_handle;
	}
}
