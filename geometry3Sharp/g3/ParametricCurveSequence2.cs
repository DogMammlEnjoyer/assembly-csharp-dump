using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace g3
{
	public class ParametricCurveSequence2 : IParametricCurve2d, IMultiCurve2d
	{
		public ParametricCurveSequence2()
		{
			this.curves = new List<IParametricCurve2d>();
		}

		public ParametricCurveSequence2(IEnumerable<IParametricCurve2d> curvesIn, bool isClosed = false)
		{
			this.curves = new List<IParametricCurve2d>(curvesIn);
			this.closed = true;
		}

		public int Count
		{
			get
			{
				return this.curves.Count;
			}
		}

		public ReadOnlyCollection<IParametricCurve2d> Curves
		{
			get
			{
				return this.curves.AsReadOnly();
			}
		}

		public bool IsClosed
		{
			get
			{
				return this.closed;
			}
			set
			{
				this.closed = value;
			}
		}

		public void Append(IParametricCurve2d c)
		{
			this.curves.Add(c);
		}

		public void Prepend(IParametricCurve2d c)
		{
			this.curves.Insert(0, c);
		}

		public double ParamLength
		{
			get
			{
				double num = 0.0;
				foreach (IParametricCurve2d parametricCurve2d in this.Curves)
				{
					num += parametricCurve2d.ParamLength;
				}
				return num;
			}
		}

		public Vector2d SampleT(double t)
		{
			double num = 0.0;
			for (int i = 0; i < this.Curves.Count; i++)
			{
				double paramLength = this.curves[i].ParamLength;
				if (t <= num + paramLength)
				{
					double t2 = t - num;
					return this.curves[i].SampleT(t2);
				}
				num += paramLength;
			}
			throw new ArgumentException("ParametricCurveSequence2.SampleT: argument out of range");
		}

		public Vector2d TangentT(double t)
		{
			double num = 0.0;
			for (int i = 0; i < this.Curves.Count; i++)
			{
				double paramLength = this.curves[i].ParamLength;
				if (t <= num + paramLength)
				{
					double t2 = t - num;
					return this.curves[i].TangentT(t2);
				}
				num += paramLength;
			}
			throw new ArgumentException("ParametricCurveSequence2.SampleT: argument out of range");
		}

		public bool HasArcLength
		{
			get
			{
				using (IEnumerator<IParametricCurve2d> enumerator = this.Curves.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator.Current.HasArcLength)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		public double ArcLength
		{
			get
			{
				double num = 0.0;
				foreach (IParametricCurve2d parametricCurve2d in this.Curves)
				{
					num += parametricCurve2d.ArcLength;
				}
				return num;
			}
		}

		public Vector2d SampleArcLength(double a)
		{
			double num = 0.0;
			for (int i = 0; i < this.Curves.Count; i++)
			{
				double arcLength = this.curves[i].ArcLength;
				if (a <= num + arcLength)
				{
					double a2 = a - num;
					return this.curves[i].SampleArcLength(a2);
				}
				num += arcLength;
			}
			throw new ArgumentException("ParametricCurveSequence2.SampleArcLength: argument out of range");
		}

		public void Reverse()
		{
			foreach (IParametricCurve2d parametricCurve2d in this.curves)
			{
				parametricCurve2d.Reverse();
			}
			this.curves.Reverse();
		}

		public IParametricCurve2d Clone()
		{
			ParametricCurveSequence2 parametricCurveSequence = new ParametricCurveSequence2();
			parametricCurveSequence.closed = this.closed;
			parametricCurveSequence.curves = new List<IParametricCurve2d>();
			foreach (IParametricCurve2d parametricCurve2d in this.curves)
			{
				parametricCurveSequence.curves.Add(parametricCurve2d.Clone());
			}
			return parametricCurveSequence;
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
			foreach (IParametricCurve2d parametricCurve2d in this.curves)
			{
				parametricCurve2d.Transform(xform);
			}
		}

		private List<IParametricCurve2d> curves;

		private bool closed;
	}
}
