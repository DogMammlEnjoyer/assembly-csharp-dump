using System;
using System.Collections.Generic;

namespace g3
{
	public class BiArcFit2
	{
		public BiArcFit2(Vector2d point1, Vector2d tangent1, Vector2d point2, Vector2d tangent2)
		{
			this.Point1 = point1;
			this.Tangent1 = tangent1;
			this.Point2 = point2;
			this.Tangent2 = tangent2;
			this.Fit();
			this.set_output();
		}

		public BiArcFit2(Vector2d point1, Vector2d tangent1, Vector2d point2, Vector2d tangent2, double d1)
		{
			this.Point1 = point1;
			this.Tangent1 = tangent1;
			this.Point2 = point2;
			this.Tangent2 = tangent2;
			this.Fit(d1);
			this.set_output();
		}

		private void set_output()
		{
			if (this.arc1.IsSegment)
			{
				this.Arc1IsSegment = true;
				this.Segment1 = new Segment2d(this.arc1.P0, this.arc1.P1);
			}
			else
			{
				this.Arc1IsSegment = false;
				this.Arc1 = this.get_arc(0);
			}
			if (this.arc2.IsSegment)
			{
				this.Arc2IsSegment = true;
				this.Segment2 = new Segment2d(this.arc2.P1, this.arc2.P0);
				return;
			}
			this.Arc2IsSegment = false;
			this.Arc2 = this.get_arc(1);
		}

		public double Distance(Vector2d point)
		{
			double val = this.Arc1IsSegment ? Math.Sqrt(this.Segment1.DistanceSquared(point)) : this.Arc1.Distance(point);
			double val2 = this.Arc2IsSegment ? Math.Sqrt(this.Segment2.DistanceSquared(point)) : this.Arc2.Distance(point);
			return Math.Min(val, val2);
		}

		public Vector2d NearestPoint(Vector2d point)
		{
			Vector2d result = this.Arc1IsSegment ? this.Segment1.NearestPoint(point) : this.Arc1.NearestPoint(point);
			Vector2d result2 = this.Arc2IsSegment ? this.Segment2.NearestPoint(point) : this.Arc2.NearestPoint(point);
			if (result.DistanceSquared(point) >= result2.DistanceSquared(point))
			{
				return result2;
			}
			return result;
		}

		public List<IParametricCurve2d> Curves
		{
			get
			{
				IParametricCurve2d parametricCurve2d2;
				if (!this.Arc1IsSegment)
				{
					IParametricCurve2d parametricCurve2d = this.Arc1;
					parametricCurve2d2 = parametricCurve2d;
				}
				else
				{
					IParametricCurve2d parametricCurve2d = this.Segment1;
					parametricCurve2d2 = parametricCurve2d;
				}
				IParametricCurve2d item = parametricCurve2d2;
				IParametricCurve2d parametricCurve2d3;
				if (!this.Arc2IsSegment)
				{
					IParametricCurve2d parametricCurve2d = this.Arc2;
					parametricCurve2d3 = parametricCurve2d;
				}
				else
				{
					IParametricCurve2d parametricCurve2d = this.Segment2;
					parametricCurve2d3 = parametricCurve2d;
				}
				IParametricCurve2d item2 = parametricCurve2d3;
				return new List<IParametricCurve2d>
				{
					item,
					item2
				};
			}
		}

		public IParametricCurve2d Curve1
		{
			get
			{
				if (!this.Arc1IsSegment)
				{
					return this.Arc1;
				}
				return this.Segment1;
			}
		}

		public IParametricCurve2d Curve2
		{
			get
			{
				if (!this.Arc2IsSegment)
				{
					return this.Arc2;
				}
				return this.Segment2;
			}
		}

		private void set_arc(int i, BiArcFit2.Arc a)
		{
			if (i == 0)
			{
				this.arc1 = a;
				return;
			}
			this.arc2 = a;
		}

		private Arc2d get_arc(int i)
		{
			BiArcFit2.Arc arc = (i == 0) ? this.arc1 : this.arc2;
			double num = arc.AngleStartR * 57.29577951308232;
			double num2 = arc.AngleEndR * 57.29577951308232;
			if (arc.PositiveRotation)
			{
				double num3 = num;
				num = num2;
				num2 = num3;
			}
			Arc2d arc2d = new Arc2d(arc.Center, arc.Radius, num, num2);
			if (i == 0 && arc2d.SampleT(0.0).DistanceSquared(this.Point1) > 1E-08)
			{
				arc2d.Reverse();
			}
			if (i == 1 && arc2d.SampleT(1.0).DistanceSquared(this.Point2) > 1E-08)
			{
				arc2d.Reverse();
			}
			return arc2d;
		}

		private void Fit()
		{
			Vector2d point = this.Point1;
			Vector2d point2 = this.Point2;
			Vector2d tangent = this.Tangent1;
			Vector2d tangent2 = this.Tangent2;
			Vector2d vector2d = point2 - point;
			double lengthSquared = vector2d.LengthSquared;
			Vector2d v = tangent + tangent2;
			bool flag = MathUtil.EpsilonEqual(v.LengthSquared, 4.0, this.Epsilon);
			double num = vector2d.Dot(tangent);
			bool flag2 = MathUtil.EpsilonEqual(num, 0.0, this.Epsilon);
			if (flag && flag2)
			{
				this.FitD1 = (this.FitD2 = double.PositiveInfinity);
				double num2 = Math.Atan2(vector2d.y, vector2d.x);
				Vector2d c = point + 0.25 * vector2d;
				Vector2d c2 = point + 0.75 * vector2d;
				double r = Math.Sqrt(lengthSquared) * 0.25;
				double num3 = vector2d.x * tangent.y - vector2d.y * tangent.x;
				this.arc1 = new BiArcFit2.Arc(c, r, num2, num2 + 3.141592653589793, num3 < 0.0);
				this.arc1 = new BiArcFit2.Arc(c2, r, num2, num2 + 3.141592653589793, num3 > 0.0);
				return;
			}
			double num4 = vector2d.Dot(v);
			double num5;
			if (flag)
			{
				num5 = lengthSquared / (4.0 * num);
			}
			else
			{
				double num6 = 2.0 - 2.0 * tangent.Dot(tangent2);
				num5 = (Math.Sqrt(num4 * num4 + num6 * lengthSquared) - num4) / num6;
			}
			this.FitD1 = (this.FitD2 = num5);
			Vector2d vector2d2 = point + point2 + num5 * (tangent - tangent2);
			vector2d2 *= 0.5;
			this.SetArcFromEdge(0, point, tangent, vector2d2, true);
			this.SetArcFromEdge(1, point2, tangent2, vector2d2, false);
		}

		private void Fit(double d1)
		{
			Vector2d point = this.Point1;
			Vector2d point2 = this.Point2;
			Vector2d tangent = this.Tangent1;
			Vector2d tangent2 = this.Tangent2;
			Vector2d vector2d = point2 - point;
			double lengthSquared = vector2d.LengthSquared;
			double lengthSquared2 = (tangent + tangent2).LengthSquared;
			double num = vector2d.Dot(tangent);
			double num2 = vector2d.Dot(tangent2);
			double num3 = tangent.Dot(tangent2);
			double num4 = num2 - d1 * (num3 - 1.0);
			if (MathUtil.EpsilonEqual(num4, 0.0, 9.999999974752427E-07))
			{
				this.FitD1 = d1;
				this.FitD2 = double.PositiveInfinity;
				Vector2d vector2d2 = point + d1 * tangent;
				vector2d2 += (num2 - d1 * num3) * tangent2;
				this.SetArcFromEdge(0, point, tangent, vector2d2, true);
				this.SetArcFromEdge(1, point2, tangent2, vector2d2, false);
				return;
			}
			double num5 = (0.5 * lengthSquared - d1 * num) / num4;
			double f = 1.0 / (d1 + num5);
			Vector2d vector2d3 = d1 * num5 * (tangent - tangent2);
			vector2d3 += d1 * point2;
			vector2d3 += num5 * point;
			vector2d3 *= f;
			this.FitD1 = d1;
			this.FitD2 = num5;
			this.SetArcFromEdge(0, point, tangent, vector2d3, true);
			this.SetArcFromEdge(1, point2, tangent2, vector2d3, false);
		}

		private void SetArcFromEdge(int i, Vector2d p1, Vector2d t1, Vector2d p2, bool fromP1)
		{
			Vector2d vector2d = p2 - p1;
			Vector2d vector2d2 = new Vector2d(-t1.y, t1.x);
			double num = vector2d.Dot(vector2d2);
			if (MathUtil.EpsilonEqual(num, 0.0, this.Epsilon))
			{
				this.set_arc(i, new BiArcFit2.Arc(p1, p2));
				return;
			}
			double num2 = vector2d.LengthSquared / (2.0 * num);
			Vector2d vector2d3 = p1 + num2 * vector2d2;
			Vector2d vector2d4 = p1 - vector2d3;
			Vector2d vector2d5 = p2 - vector2d3;
			double startR = Math.Atan2(vector2d4.y, vector2d4.x);
			double endR = Math.Atan2(vector2d5.y, vector2d5.x);
			if (vector2d4.x * t1.y - vector2d4.y * t1.x > 0.0)
			{
				this.set_arc(i, new BiArcFit2.Arc(vector2d3, Math.Abs(num2), startR, endR, !fromP1));
				return;
			}
			this.set_arc(i, new BiArcFit2.Arc(vector2d3, Math.Abs(num2), startR, endR, fromP1));
		}

		public void DebugPrint()
		{
			Console.WriteLine("biarc fit Pt0 {0} Pt1 {1}  Tan0 {2} Tan1 {3}", new object[]
			{
				this.Point1,
				this.Point2,
				this.Tangent1,
				this.Tangent2
			});
			Console.WriteLine("  First: Start {0} End {1}  {2}", this.Arc1IsSegment ? this.Segment1.P0 : this.Arc1.SampleT(0.0), this.Arc1IsSegment ? this.Segment1.P1 : this.Arc1.SampleT(1.0), this.Arc1IsSegment ? "segment" : "arc");
			Console.WriteLine("  Second: Start {0} End {1}  {2}", this.Arc2IsSegment ? this.Segment2.P0 : this.Arc2.SampleT(0.0), this.Arc2IsSegment ? this.Segment2.P1 : this.Arc2.SampleT(1.0), this.Arc2IsSegment ? "segment" : "arc");
		}

		public Vector2d Point1;

		public Vector2d Point2;

		public Vector2d Tangent1;

		public Vector2d Tangent2;

		public double Epsilon = 1E-08;

		public Arc2d Arc1;

		public Arc2d Arc2;

		public bool Arc1IsSegment;

		public bool Arc2IsSegment;

		public Segment2d Segment1;

		public Segment2d Segment2;

		public double FitD1;

		public double FitD2;

		private BiArcFit2.Arc arc1;

		private BiArcFit2.Arc arc2;

		private struct Arc
		{
			public Arc(Vector2d c, double r, double startR, double endR, bool posRotation)
			{
				this.Center = c;
				this.Radius = r;
				this.AngleStartR = startR;
				this.AngleEndR = endR;
				this.PositiveRotation = posRotation;
				this.IsSegment = false;
				this.P0 = (this.P1 = Vector2d.Zero);
			}

			public Arc(Vector2d p0, Vector2d p1)
			{
				this.Center = Vector2d.Zero;
				this.Radius = (this.AngleStartR = (this.AngleEndR = 0.0));
				this.PositiveRotation = false;
				this.IsSegment = true;
				this.P0 = p0;
				this.P1 = p1;
			}

			public Vector2d Center;

			public double Radius;

			public double AngleStartR;

			public double AngleEndR;

			public bool PositiveRotation;

			public bool IsSegment;

			public Vector2d P0;

			public Vector2d P1;
		}
	}
}
