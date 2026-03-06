using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Unity.Cinemachine
{
	internal class ClipperOffset
	{
		public double ArcTolerance { get; set; }

		public bool MergeGroups { get; set; }

		public double MiterLimit { get; set; }

		public bool PreserveCollinear { get; set; }

		public bool ReverseSolution { get; set; }

		public ClipperOffset(double miterLimit = 2.0, double arcTolerance = 0.0, bool preserveCollinear = false, bool reverseSolution = false)
		{
			this.MiterLimit = miterLimit;
			this.ArcTolerance = arcTolerance;
			this.MergeGroups = true;
			this.PreserveCollinear = preserveCollinear;
			this.ReverseSolution = reverseSolution;
		}

		public void Clear()
		{
			this._pathGroups.Clear();
		}

		public void AddPath(List<Point64> path, JoinType joinType, EndType endType)
		{
			if (path.Count == 0)
			{
				return;
			}
			List<List<Point64>> paths = new List<List<Point64>>(1)
			{
				path
			};
			this.AddPaths(paths, joinType, endType);
		}

		public void AddPaths(List<List<Point64>> paths, JoinType joinType, EndType endType)
		{
			if (paths.Count == 0)
			{
				return;
			}
			this._pathGroups.Add(new PathGroup(paths, joinType, endType));
		}

		public void AddPath(List<PointD> path, JoinType joinType, EndType endType)
		{
			if (path.Count == 0)
			{
				return;
			}
			List<List<PointD>> paths = new List<List<PointD>>(1)
			{
				path
			};
			this.AddPaths(paths, joinType, endType);
		}

		public void AddPaths(List<List<PointD>> paths, JoinType joinType, EndType endType)
		{
			if (paths.Count == 0)
			{
				return;
			}
			this._pathGroups.Add(new PathGroup(Clipper.Paths64(paths), joinType, endType));
		}

		public List<List<Point64>> Execute(double delta)
		{
			this.solution.Clear();
			if (Math.Abs(delta) < 0.5)
			{
				foreach (PathGroup pathGroup in this._pathGroups)
				{
					foreach (List<Point64> item in pathGroup._inPaths)
					{
						this.solution.Add(item);
					}
				}
				return this.solution;
			}
			this._tmpLimit = ((this.MiterLimit <= 1.0) ? 2.0 : (2.0 / Clipper.Sqr(this.MiterLimit)));
			foreach (PathGroup group in this._pathGroups)
			{
				this.DoGroupOffset(group, delta);
			}
			if (this.MergeGroups && this._pathGroups.Count > 0)
			{
				Clipper64 clipper = new Clipper64
				{
					PreserveCollinear = this.PreserveCollinear,
					ReverseSolution = (this.ReverseSolution != this._pathGroups[0]._pathsReversed)
				};
				clipper.AddSubject(this.solution);
				if (this._pathGroups[0]._pathsReversed)
				{
					clipper.Execute(ClipType.Union, FillRule.Negative, this.solution);
				}
				else
				{
					clipper.Execute(ClipType.Union, FillRule.Positive, this.solution);
				}
			}
			return this.solution;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static PointD GetUnitNormal(Point64 pt1, Point64 pt2)
		{
			double num = (double)(pt2.X - pt1.X);
			double num2 = (double)(pt2.Y - pt1.Y);
			if (num == 0.0 && num2 == 0.0)
			{
				return default(PointD);
			}
			double num3 = 1.0 / Math.Sqrt(num * num + num2 * num2);
			num *= num3;
			num2 *= num3;
			return new PointD(num2, -num);
		}

		private int GetLowestPolygonIdx(List<List<Point64>> paths)
		{
			Point64 point = new Point64(0L, long.MinValue);
			int result = -1;
			for (int i = 0; i < paths.Count; i++)
			{
				List<Point64> list = paths[i];
				for (int j = 0; j < list.Count; j++)
				{
					if (list[j].Y >= point.Y && (list[j].Y > point.Y || list[j].X < point.X))
					{
						result = i;
						point = list[j];
					}
				}
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private PointD TranslatePoint(PointD pt, double dx, double dy)
		{
			return new PointD(pt.x + dx, pt.y + dy);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private PointD ReflectPoint(PointD pt, PointD pivot)
		{
			return new PointD(pivot.x + (pivot.x - pt.x), pivot.y + (pivot.y - pt.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool AlmostZero(double value, double epsilon = 0.001)
		{
			return Math.Abs(value) < epsilon;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private double Hypotenuse(double x, double y)
		{
			return Math.Sqrt(Math.Pow(x, 2.0) + Math.Pow(y, 2.0));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private PointD NormalizeVector(PointD vec)
		{
			double num = this.Hypotenuse(vec.x, vec.y);
			if (this.AlmostZero(num, 0.001))
			{
				return new PointD(0L, 0L);
			}
			double num2 = 1.0 / num;
			return new PointD(vec.x * num2, vec.y * num2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private PointD GetAvgUnitVector(PointD vec1, PointD vec2)
		{
			return this.NormalizeVector(new PointD(vec1.x + vec2.x, vec1.y + vec2.y));
		}

		private PointD IntersectPoint(PointD pt1a, PointD pt1b, PointD pt2a, PointD pt2b)
		{
			if (pt1a.x == pt1b.x)
			{
				if (pt2a.x == pt2b.x)
				{
					return new PointD(0L, 0L);
				}
				double num = (pt2b.y - pt2a.y) / (pt2b.x - pt2a.x);
				double num2 = pt2a.y - num * pt2a.x;
				return new PointD(pt1a.x, num * pt1a.x + num2);
			}
			else
			{
				if (pt2a.x == pt2b.x)
				{
					double num3 = (pt1b.y - pt1a.y) / (pt1b.x - pt1a.x);
					double num4 = pt1a.y - num3 * pt1a.x;
					return new PointD(pt2a.x, num3 * pt2a.x + num4);
				}
				double num5 = (pt1b.y - pt1a.y) / (pt1b.x - pt1a.x);
				double num6 = pt1a.y - num5 * pt1a.x;
				double num7 = (pt2b.y - pt2a.y) / (pt2b.x - pt2a.x);
				double num8 = pt2a.y - num7 * pt2a.x;
				if (num5 == num7)
				{
					return new PointD(0L, 0L);
				}
				double num9 = (num8 - num6) / (num5 - num7);
				return new PointD(num9, num5 * num9 + num6);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void DoSquare(PathGroup group, List<Point64> path, int j, int k)
		{
			PointD avgUnitVector = this.GetAvgUnitVector(new PointD(-this._normals[k].y, this._normals[k].x), new PointD(this._normals[j].y, -this._normals[j].x));
			PointD pointD = new PointD(path[j]);
			pointD = this.TranslatePoint(pointD, this._delta * avgUnitVector.x, this._delta * avgUnitVector.y);
			PointD pt1a = this.TranslatePoint(pointD, this._delta * avgUnitVector.y, this._delta * -avgUnitVector.x);
			PointD pt1b = this.TranslatePoint(pointD, this._delta * -avgUnitVector.y, this._delta * avgUnitVector.x);
			PointD pt2a = new PointD((double)path[k].X + this._normals[k].x * this._delta, (double)path[k].Y + this._normals[k].y * this._delta);
			PointD pt2b = new PointD((double)path[j].X + this._normals[k].x * this._delta, (double)path[j].Y + this._normals[k].y * this._delta);
			PointD pt = this.IntersectPoint(pt1a, pt1b, pt2a, pt2b);
			group._outPath.Add(new Point64(pt));
			group._outPath.Add(new Point64(this.ReflectPoint(pt, pointD)));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void DoMiter(PathGroup group, List<Point64> path, int j, int k, double cosA)
		{
			double num = this._delta / (cosA + 1.0);
			group._outPath.Add(new Point64((double)path[j].X + (this._normals[k].x + this._normals[j].x) * num, (double)path[j].Y + (this._normals[k].y + this._normals[j].y) * num));
		}

		private void DoRound(PathGroup group, Point64 pt, PointD normal1, PointD normal2, double angle)
		{
			PointD pointD = new PointD(normal2.x * this._delta, normal2.y * this._delta);
			int num = (int)Math.Round(this._stepsPerRad * Math.Abs(angle) + 0.501);
			group._outPath.Add(new Point64((double)pt.X + pointD.x, (double)pt.Y + pointD.y));
			double num2 = Math.Sin(angle / (double)num);
			double num3 = Math.Cos(angle / (double)num);
			for (int i = 0; i < num; i++)
			{
				pointD = new PointD(pointD.x * num3 - num2 * pointD.y, pointD.x * num2 + pointD.y * num3);
				group._outPath.Add(new Point64((double)pt.X + pointD.x, (double)pt.Y + pointD.y));
			}
			group._outPath.Add(new Point64((double)pt.X + normal1.x * this._delta, (double)pt.Y + normal1.y * this._delta));
		}

		private void BuildNormals(List<Point64> path)
		{
			int count = path.Count;
			this._normals.Clear();
			this._normals.Capacity = count;
			for (int i = 0; i < count - 1; i++)
			{
				this._normals.Add(ClipperOffset.GetUnitNormal(path[i], path[i + 1]));
			}
			this._normals.Add(ClipperOffset.GetUnitNormal(path[count - 1], path[0]));
		}

		private void OffsetPoint(PathGroup group, List<Point64> path, int j, ref int k)
		{
			double num = this._normals[k].x * this._normals[j].y - this._normals[j].x * this._normals[k].y;
			if (num > 1.0)
			{
				num = 1.0;
			}
			else if (num < -1.0)
			{
				num = -1.0;
			}
			if (num * this._delta < 0.0)
			{
				Point64 point = new Point64((double)path[j].X + this._normals[k].x * this._delta, (double)path[j].Y + this._normals[k].y * this._delta);
				Point64 point2 = new Point64((double)path[j].X + this._normals[j].x * this._delta, (double)path[j].Y + this._normals[j].y * this._delta);
				group._outPath.Add(point);
				if (point != point2)
				{
					group._outPath.Add(path[j]);
					group._outPath.Add(point2);
				}
			}
			else
			{
				double num2 = InternalClipper.DotProduct(this._normals[j], this._normals[k]);
				JoinType joinType = this._joinType;
				if (joinType != JoinType.Square)
				{
					if (joinType == JoinType.Miter)
					{
						if (1.0 + num2 < this._tmpLimit)
						{
							this.DoSquare(group, path, j, k);
						}
						else
						{
							this.DoMiter(group, path, j, k, num2);
						}
					}
					else
					{
						this.DoRound(group, path[j], this._normals[j], this._normals[k], Math.Atan2(num, num2));
					}
				}
				else if (num2 >= 0.0)
				{
					this.DoMiter(group, path, j, k, num2);
				}
				else
				{
					this.DoSquare(group, path, j, k);
				}
			}
			k = j;
		}

		private void OffsetPolygon(PathGroup group, List<Point64> path)
		{
			group._outPath = new List<Point64>();
			int count = path.Count;
			int num = count - 1;
			for (int i = 0; i < count; i++)
			{
				this.OffsetPoint(group, path, i, ref num);
			}
			group._outPaths.Add(group._outPath);
		}

		private void OffsetOpenJoined(PathGroup group, List<Point64> path)
		{
			this.OffsetPolygon(group, path);
			path = Clipper.ReversePath(path);
			this.BuildNormals(path);
			this.OffsetPolygon(group, path);
		}

		private void OffsetOpenPath(PathGroup group, List<Point64> path, EndType endType)
		{
			group._outPath = new List<Point64>();
			int num = path.Count - 1;
			int num2 = 0;
			for (int i = 1; i < num; i++)
			{
				this.OffsetPoint(group, path, i, ref num2);
			}
			num++;
			this._normals[num - 1] = new PointD(-this._normals[num - 2].x, -this._normals[num - 2].y);
			if (endType != EndType.Butt)
			{
				if (endType != EndType.Round)
				{
					this.DoSquare(group, path, num - 1, num - 2);
				}
				else
				{
					this.DoRound(group, path[num - 1], this._normals[num - 1], this._normals[num - 2], 3.141592653589793);
				}
			}
			else
			{
				group._outPath.Add(new Point64((double)path[num - 1].X + this._normals[num - 2].x * this._delta, (double)path[num - 1].Y + this._normals[num - 2].y * this._delta));
				group._outPath.Add(new Point64((double)path[num - 1].X - this._normals[num - 2].x * this._delta, (double)path[num - 1].Y - this._normals[num - 2].y * this._delta));
			}
			for (int j = num - 2; j > 0; j--)
			{
				this._normals[j] = new PointD(-this._normals[j - 1].x, -this._normals[j - 1].y);
			}
			this._normals[0] = new PointD(-this._normals[1].x, -this._normals[1].y);
			num2 = num - 1;
			for (int k = num - 2; k > 0; k--)
			{
				this.OffsetPoint(group, path, k, ref num2);
			}
			if (endType != EndType.Butt)
			{
				if (endType != EndType.Round)
				{
					this.DoSquare(group, path, 0, 1);
				}
				else
				{
					this.DoRound(group, path[0], this._normals[0], this._normals[1], 3.141592653589793);
				}
			}
			else
			{
				group._outPath.Add(new Point64((double)path[0].X + this._normals[1].x * this._delta, (double)path[0].Y + this._normals[1].y * this._delta));
				group._outPath.Add(new Point64((double)path[0].X - this._normals[1].x * this._delta, (double)path[0].Y - this._normals[1].y * this._delta));
			}
			group._outPaths.Add(group._outPath);
		}

		private bool IsFullyOpenEndType(EndType et)
		{
			return et != EndType.Polygon && et != EndType.Joined;
		}

		private void DoGroupOffset(PathGroup group, double delta)
		{
			if (group._endType != EndType.Polygon)
			{
				delta = Math.Abs(delta) / 2.0;
			}
			bool flag = !this.IsFullyOpenEndType(group._endType);
			if (flag)
			{
				int lowestPolygonIdx = this.GetLowestPolygonIdx(group._inPaths);
				if (lowestPolygonIdx < 0)
				{
					return;
				}
				double num = Clipper.Area(group._inPaths[lowestPolygonIdx]);
				if (num == 0.0)
				{
					return;
				}
				group._pathsReversed = (num < 0.0);
				if (group._pathsReversed)
				{
					delta = -delta;
				}
			}
			else
			{
				group._pathsReversed = false;
			}
			this._delta = delta;
			double num2 = Math.Abs(this._delta);
			this._joinType = group._joinType;
			if (group._joinType == JoinType.Round || group._endType == EndType.Round)
			{
				double num3 = (this.ArcTolerance > 0.01) ? this.ArcTolerance : (Math.Log10(2.0 + num2) * 0.25);
				this._stepsPerRad = 3.141592653589793 / Math.Acos(1.0 - num3 / num2) / 6.283185307179586;
			}
			foreach (List<Point64> path in group._inPaths)
			{
				List<Point64> list = Clipper.StripDuplicates(path, flag);
				int count = list.Count;
				if (count != 0 && (count >= 3 || this.IsFullyOpenEndType(group._endType)))
				{
					if (count == 1)
					{
						group._outPath = new List<Point64>();
						if (group._endType == EndType.Round)
						{
							this.DoRound(group, list[0], new PointD(1.0, 0.0), new PointD(-1.0, 0.0), 6.283185307179586);
						}
						else
						{
							group._outPath.Capacity = 4;
							group._outPath.Add(new Point64((double)list[0].X - this._delta, (double)list[0].Y - this._delta));
							group._outPath.Add(new Point64((double)list[0].X + this._delta, (double)list[0].Y - this._delta));
							group._outPath.Add(new Point64((double)list[0].X + this._delta, (double)list[0].Y + this._delta));
							group._outPath.Add(new Point64((double)list[0].X - this._delta, (double)list[0].Y + this._delta));
						}
						group._outPaths.Add(group._outPath);
					}
					else
					{
						this.BuildNormals(list);
						if (group._endType == EndType.Polygon)
						{
							this.OffsetPolygon(group, list);
						}
						else if (group._endType == EndType.Joined)
						{
							this.OffsetOpenJoined(group, list);
						}
						else
						{
							this.OffsetOpenPath(group, list, group._endType);
						}
					}
				}
			}
			if (!this.MergeGroups)
			{
				Clipper64 clipper = new Clipper64
				{
					PreserveCollinear = this.PreserveCollinear,
					ReverseSolution = (this.ReverseSolution != group._pathsReversed)
				};
				clipper.AddSubject(group._outPaths);
				if (group._pathsReversed)
				{
					clipper.Execute(ClipType.Union, FillRule.Negative, group._outPaths);
				}
				else
				{
					clipper.Execute(ClipType.Union, FillRule.Positive, group._outPaths);
				}
			}
			this.solution.AddRange(group._outPaths);
			group._outPaths.Clear();
		}

		private readonly List<PathGroup> _pathGroups = new List<PathGroup>();

		private readonly List<PointD> _normals = new List<PointD>();

		private readonly List<List<Point64>> solution = new List<List<Point64>>();

		private double _delta;

		private double _tmpLimit;

		private double _stepsPerRad;

		private JoinType _joinType;

		private const double TwoPi = 6.283185307179586;
	}
}
