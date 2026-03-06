using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Unity.Cinemachine
{
	[NullableContext(1)]
	[Nullable(0)]
	internal static class Clipper
	{
		public static List<List<Point64>> Intersect(List<List<Point64>> subject, List<List<Point64>> clip, FillRule fillRule)
		{
			return Clipper.BooleanOp(ClipType.Intersection, fillRule, subject, clip);
		}

		public static List<List<PointD>> Intersect(List<List<PointD>> subject, List<List<PointD>> clip, FillRule fillRule)
		{
			return Clipper.BooleanOp(ClipType.Intersection, fillRule, subject, clip, 0);
		}

		public static List<List<Point64>> Union(List<List<Point64>> subject, FillRule fillRule)
		{
			return Clipper.BooleanOp(ClipType.Union, fillRule, subject, null);
		}

		public static List<List<Point64>> Union(List<List<Point64>> subject, List<List<Point64>> clip, FillRule fillRule)
		{
			return Clipper.BooleanOp(ClipType.Union, fillRule, subject, clip);
		}

		public static List<List<PointD>> Union(List<List<PointD>> subject, FillRule fillRule)
		{
			return Clipper.BooleanOp(ClipType.Union, fillRule, subject, null, 0);
		}

		public static List<List<PointD>> Union(List<List<PointD>> subject, List<List<PointD>> clip, FillRule fillRule)
		{
			return Clipper.BooleanOp(ClipType.Union, fillRule, subject, clip, 0);
		}

		public static List<List<Point64>> Difference(List<List<Point64>> subject, List<List<Point64>> clip, FillRule fillRule)
		{
			return Clipper.BooleanOp(ClipType.Difference, fillRule, subject, clip);
		}

		public static List<List<PointD>> Difference(List<List<PointD>> subject, List<List<PointD>> clip, FillRule fillRule)
		{
			return Clipper.BooleanOp(ClipType.Difference, fillRule, subject, clip, 0);
		}

		public static List<List<Point64>> Xor(List<List<Point64>> subject, List<List<Point64>> clip, FillRule fillRule)
		{
			return Clipper.BooleanOp(ClipType.Xor, fillRule, subject, clip);
		}

		public static List<List<PointD>> Xor(List<List<PointD>> subject, List<List<PointD>> clip, FillRule fillRule)
		{
			return Clipper.BooleanOp(ClipType.Xor, fillRule, subject, clip, 0);
		}

		public static List<List<Point64>> BooleanOp(ClipType clipType, FillRule fillRule, [Nullable(new byte[]
		{
			2,
			1
		})] List<List<Point64>> subject, [Nullable(new byte[]
		{
			2,
			1
		})] List<List<Point64>> clip)
		{
			List<List<Point64>> list = new List<List<Point64>>();
			if (subject == null)
			{
				return list;
			}
			Clipper64 clipper = new Clipper64();
			clipper.AddPaths(subject, PathType.Subject, false);
			if (clip != null)
			{
				clipper.AddPaths(clip, PathType.Clip, false);
			}
			clipper.Execute(clipType, fillRule, list);
			return list;
		}

		public static List<List<PointD>> BooleanOp(ClipType clipType, FillRule fillRule, List<List<PointD>> subject, [Nullable(new byte[]
		{
			2,
			1
		})] List<List<PointD>> clip, int roundingDecimalPrecision = 0)
		{
			List<List<PointD>> list = new List<List<PointD>>();
			ClipperD clipperD = new ClipperD(roundingDecimalPrecision);
			clipperD.AddSubject(subject);
			if (clip != null)
			{
				clipperD.AddClip(clip);
			}
			clipperD.Execute(clipType, fillRule, list);
			return list;
		}

		public static List<List<Point64>> InflatePaths(List<List<Point64>> paths, double delta, JoinType joinType, EndType endType, double miterLimit = 2.0)
		{
			ClipperOffset clipperOffset = new ClipperOffset(miterLimit, 0.0, false, false);
			clipperOffset.AddPaths(paths, joinType, endType);
			return clipperOffset.Execute(delta);
		}

		public static List<List<PointD>> InflatePaths(List<List<PointD>> paths, double delta, JoinType joinType, EndType endType, double miterLimit = 2.0, int precision = 2)
		{
			if (precision < -8 || precision > 8)
			{
				throw new Exception("Error: Precision is out of range.");
			}
			double num = Math.Pow(10.0, (double)precision);
			List<List<Point64>> paths2 = Clipper.ScalePaths64(paths, num);
			ClipperOffset clipperOffset = new ClipperOffset(miterLimit, 0.0, false, false);
			clipperOffset.AddPaths(paths2, joinType, endType);
			paths2 = clipperOffset.Execute(delta * num);
			return Clipper.ScalePathsD(paths2, 1.0 / num);
		}

		public static List<List<Point64>> MinkowskiSum(List<Point64> pattern, List<Point64> path, bool isClosed)
		{
			return Minkowski.Sum(pattern, path, isClosed);
		}

		public static List<List<Point64>> MinkowskiDiff(List<Point64> pattern, List<Point64> path, bool isClosed)
		{
			return Minkowski.Diff(pattern, path, isClosed);
		}

		public static double Area(List<Point64> path)
		{
			double num = 0.0;
			int count = path.Count;
			if (count < 3)
			{
				return 0.0;
			}
			Point64 point = path[count - 1];
			foreach (Point64 point2 in path)
			{
				num += (double)(point.Y + point2.Y) * (double)(point.X - point2.X);
				point = point2;
			}
			return num * 0.5;
		}

		public static double Area(List<List<Point64>> paths)
		{
			double num = 0.0;
			foreach (List<Point64> path in paths)
			{
				num += Clipper.Area(path);
			}
			return num;
		}

		public static double Area(List<PointD> path)
		{
			double num = 0.0;
			int count = path.Count;
			if (count < 3)
			{
				return 0.0;
			}
			PointD pointD = path[count - 1];
			foreach (PointD pointD2 in path)
			{
				num += (pointD.y + pointD2.y) * (pointD.x - pointD2.x);
				pointD = pointD2;
			}
			return num * 0.5;
		}

		public static double Area(List<List<PointD>> paths)
		{
			double num = 0.0;
			foreach (List<PointD> path in paths)
			{
				num += Clipper.Area(path);
			}
			return num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsPositive(List<Point64> poly)
		{
			return Clipper.Area(poly) >= 0.0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsPositive(List<PointD> poly)
		{
			return Clipper.Area(poly) >= 0.0;
		}

		public static string Path64ToString(List<Point64> path)
		{
			string str = "";
			foreach (Point64 point in path)
			{
				str += point.ToString();
			}
			return str + "\n";
		}

		public static string Paths64ToString(List<List<Point64>> paths)
		{
			string text = "";
			foreach (List<Point64> path in paths)
			{
				text += Clipper.Path64ToString(path);
			}
			return text;
		}

		public static string PathDToString(List<PointD> path)
		{
			string str = "";
			foreach (PointD pointD in path)
			{
				str += pointD.ToString();
			}
			return str + "\n";
		}

		public static string PathsDToString(List<List<PointD>> paths)
		{
			string text = "";
			foreach (List<PointD> path in paths)
			{
				text += Clipper.PathDToString(path);
			}
			return text;
		}

		public static List<Point64> OffsetPath(List<Point64> path, long dx, long dy)
		{
			List<Point64> list = new List<Point64>(path.Count);
			foreach (Point64 point in path)
			{
				list.Add(new Point64(point.X + dx, point.Y + dy));
			}
			return list;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Point64 ScalePoint64(Point64 pt, double scale)
		{
			return new Point64
			{
				X = (long)((double)pt.X * scale),
				Y = (long)((double)pt.Y * scale)
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static PointD ScalePointD(Point64 pt, double scale)
		{
			return new PointD
			{
				x = (double)pt.X * scale,
				y = (double)pt.Y * scale
			};
		}

		public static List<Point64> ScalePath(List<Point64> path, double scale)
		{
			if (scale == 1.0)
			{
				return path;
			}
			List<Point64> list = new List<Point64>(path.Count);
			foreach (Point64 point in path)
			{
				list.Add(new Point64((double)point.X * scale, (double)point.Y * scale));
			}
			return list;
		}

		public static List<List<Point64>> ScalePaths(List<List<Point64>> paths, double scale)
		{
			if (scale == 1.0)
			{
				return paths;
			}
			List<List<Point64>> list = new List<List<Point64>>(paths.Count);
			foreach (List<Point64> path in paths)
			{
				list.Add(Clipper.ScalePath(path, scale));
			}
			return list;
		}

		public static List<PointD> ScalePath(List<PointD> path, double scale)
		{
			if (scale == 1.0)
			{
				return path;
			}
			List<PointD> list = new List<PointD>(path.Count);
			foreach (PointD pt in path)
			{
				list.Add(new PointD(pt, scale));
			}
			return list;
		}

		public static List<List<PointD>> ScalePaths(List<List<PointD>> paths, double scale)
		{
			if (scale == 1.0)
			{
				return paths;
			}
			List<List<PointD>> list = new List<List<PointD>>(paths.Count);
			foreach (List<PointD> path in paths)
			{
				list.Add(Clipper.ScalePath(path, scale));
			}
			return list;
		}

		public static List<Point64> ScalePath64(List<PointD> path, double scale)
		{
			List<Point64> list = new List<Point64>(path.Count);
			foreach (PointD pt in path)
			{
				list.Add(new Point64(pt, scale));
			}
			return list;
		}

		public static List<List<Point64>> ScalePaths64(List<List<PointD>> paths, double scale)
		{
			List<List<Point64>> list = new List<List<Point64>>(paths.Count);
			foreach (List<PointD> path in paths)
			{
				list.Add(Clipper.ScalePath64(path, scale));
			}
			return list;
		}

		public static List<PointD> ScalePathD(List<Point64> path, double scale)
		{
			List<PointD> list = new List<PointD>(path.Count);
			foreach (Point64 pt in path)
			{
				list.Add(new PointD(pt, scale));
			}
			return list;
		}

		public static List<List<PointD>> ScalePathsD(List<List<Point64>> paths, double scale)
		{
			List<List<PointD>> list = new List<List<PointD>>(paths.Count);
			foreach (List<Point64> path in paths)
			{
				list.Add(Clipper.ScalePathD(path, scale));
			}
			return list;
		}

		public static List<Point64> Path64(List<PointD> path)
		{
			List<Point64> list = new List<Point64>(path.Count);
			foreach (PointD pt in path)
			{
				list.Add(new Point64(pt));
			}
			return list;
		}

		public static List<List<Point64>> Paths64(List<List<PointD>> paths)
		{
			List<List<Point64>> list = new List<List<Point64>>(paths.Count);
			foreach (List<PointD> path in paths)
			{
				list.Add(Clipper.Path64(path));
			}
			return list;
		}

		public static List<List<PointD>> PathsD(List<List<Point64>> paths)
		{
			List<List<PointD>> list = new List<List<PointD>>(paths.Count);
			foreach (List<Point64> path in paths)
			{
				list.Add(Clipper.PathD(path));
			}
			return list;
		}

		public static List<PointD> PathD(List<Point64> path)
		{
			List<PointD> list = new List<PointD>(path.Count);
			foreach (Point64 pt in path)
			{
				list.Add(new PointD(pt));
			}
			return list;
		}

		public static List<Point64> TranslatePath(List<Point64> path, long dx, long dy)
		{
			List<Point64> list = new List<Point64>(path.Count);
			foreach (Point64 point in path)
			{
				list.Add(new Point64(point.X + dx, point.Y + dy));
			}
			return list;
		}

		public static List<List<Point64>> TranslatePaths(List<List<Point64>> paths, long dx, long dy)
		{
			List<List<Point64>> list = new List<List<Point64>>(paths.Count);
			foreach (List<Point64> path in paths)
			{
				list.Add(Clipper.OffsetPath(path, dx, dy));
			}
			return list;
		}

		public static List<PointD> TranslatePath(List<PointD> path, double dx, double dy)
		{
			List<PointD> list = new List<PointD>(path.Count);
			foreach (PointD pointD in path)
			{
				list.Add(new PointD(pointD.x + dx, pointD.y + dy));
			}
			return list;
		}

		public static List<List<PointD>> TranslatePaths(List<List<PointD>> paths, double dx, double dy)
		{
			List<List<PointD>> list = new List<List<PointD>>(paths.Count);
			foreach (List<PointD> path in paths)
			{
				list.Add(Clipper.TranslatePath(path, dx, dy));
			}
			return list;
		}

		public static List<Point64> ReversePath(List<Point64> path)
		{
			List<Point64> list = new List<Point64>(path);
			list.Reverse();
			return list;
		}

		public static List<PointD> ReversePath(List<PointD> path)
		{
			List<PointD> list = new List<PointD>(path);
			list.Reverse();
			return list;
		}

		public static List<List<Point64>> ReversePaths(List<List<Point64>> paths)
		{
			List<List<Point64>> list = new List<List<Point64>>(paths.Count);
			foreach (List<Point64> path in paths)
			{
				list.Add(Clipper.ReversePath(path));
			}
			return list;
		}

		public static List<List<PointD>> ReversePaths(List<List<PointD>> paths)
		{
			List<List<PointD>> list = new List<List<PointD>>(paths.Count);
			foreach (List<PointD> path in paths)
			{
				list.Add(Clipper.ReversePath(path));
			}
			return list;
		}

		public static Rect64 GetBounds(List<List<Point64>> paths)
		{
			Rect64 maxInvalidRect = Clipper.MaxInvalidRect64;
			foreach (List<Point64> list in paths)
			{
				foreach (Point64 point in list)
				{
					if (point.X < maxInvalidRect.left)
					{
						maxInvalidRect.left = point.X;
					}
					if (point.X > maxInvalidRect.right)
					{
						maxInvalidRect.right = point.X;
					}
					if (point.Y < maxInvalidRect.top)
					{
						maxInvalidRect.top = point.Y;
					}
					if (point.Y > maxInvalidRect.bottom)
					{
						maxInvalidRect.bottom = point.Y;
					}
				}
			}
			if (!maxInvalidRect.IsEmpty())
			{
				return maxInvalidRect;
			}
			return default(Rect64);
		}

		public static RectD GetBounds(List<List<PointD>> paths)
		{
			RectD maxInvalidRectD = Clipper.MaxInvalidRectD;
			foreach (List<PointD> list in paths)
			{
				foreach (PointD pointD in list)
				{
					if (pointD.x < maxInvalidRectD.left)
					{
						maxInvalidRectD.left = pointD.x;
					}
					if (pointD.x > maxInvalidRectD.right)
					{
						maxInvalidRectD.right = pointD.x;
					}
					if (pointD.y < maxInvalidRectD.top)
					{
						maxInvalidRectD.top = pointD.y;
					}
					if (pointD.y > maxInvalidRectD.bottom)
					{
						maxInvalidRectD.bottom = pointD.y;
					}
				}
			}
			if (!maxInvalidRectD.IsEmpty())
			{
				return maxInvalidRectD;
			}
			return default(RectD);
		}

		public static List<Point64> MakePath(int[] arr)
		{
			int num = arr.Length / 2;
			List<Point64> list = new List<Point64>(num);
			for (int i = 0; i < num; i++)
			{
				list.Add(new Point64((long)arr[i * 2], (long)arr[i * 2 + 1]));
			}
			return list;
		}

		public static List<Point64> MakePath(long[] arr)
		{
			int num = arr.Length / 2;
			List<Point64> list = new List<Point64>(num);
			for (int i = 0; i < num; i++)
			{
				list.Add(new Point64(arr[i * 2], arr[i * 2 + 1]));
			}
			return list;
		}

		public static List<PointD> MakePath(double[] arr)
		{
			int num = arr.Length / 2;
			List<PointD> list = new List<PointD>(num);
			for (int i = 0; i < num; i++)
			{
				list.Add(new PointD(arr[i * 2], arr[i * 2 + 1]));
			}
			return list;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Sqr(double value)
		{
			return value * value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool PointsNearEqual(PointD pt1, PointD pt2, double distanceSqrd)
		{
			return Clipper.Sqr(pt1.x - pt2.x) + Clipper.Sqr(pt1.y - pt2.y) < distanceSqrd;
		}

		public static List<PointD> StripNearDuplicates(List<PointD> path, double minEdgeLenSqrd, bool isClosedPath)
		{
			int count = path.Count;
			List<PointD> list = new List<PointD>(count);
			if (count == 0)
			{
				return list;
			}
			PointD pointD = path[0];
			list.Add(pointD);
			for (int i = 1; i < count; i++)
			{
				if (!Clipper.PointsNearEqual(pointD, path[i], minEdgeLenSqrd))
				{
					pointD = path[i];
					list.Add(pointD);
				}
			}
			if (isClosedPath && Clipper.PointsNearEqual(pointD, list[0], minEdgeLenSqrd))
			{
				list.RemoveAt(list.Count - 1);
			}
			return list;
		}

		public static List<Point64> StripDuplicates(List<Point64> path, bool isClosedPath)
		{
			int count = path.Count;
			List<Point64> list = new List<Point64>(count);
			if (count == 0)
			{
				return list;
			}
			Point64 point = path[0];
			list.Add(point);
			for (int i = 1; i < count; i++)
			{
				if (point != path[i])
				{
					point = path[i];
					list.Add(point);
				}
			}
			if (isClosedPath && point == list[0])
			{
				list.RemoveAt(list.Count - 1);
			}
			return list;
		}

		private static void AddPolyNodeToPaths(PolyPath64 polyPath, List<List<Point64>> paths)
		{
			if (polyPath.Polygon.Count > 0)
			{
				paths.Add(polyPath.Polygon);
			}
			for (int i = 0; i < polyPath.Count; i++)
			{
				Clipper.AddPolyNodeToPaths((PolyPath64)polyPath._childs[i], paths);
			}
		}

		public static List<List<Point64>> PolyTreeToPaths64(PolyTree64 polyTree)
		{
			List<List<Point64>> list = new List<List<Point64>>();
			for (int i = 0; i < polyTree.Count; i++)
			{
				Clipper.AddPolyNodeToPaths((PolyPath64)polyTree._childs[i], list);
			}
			return list;
		}

		public static void AddPolyNodeToPathsD(PolyPathD polyPath, List<List<PointD>> paths)
		{
			if (polyPath.Polygon.Count > 0)
			{
				paths.Add(polyPath.Polygon);
			}
			for (int i = 0; i < polyPath.Count; i++)
			{
				Clipper.AddPolyNodeToPathsD((PolyPathD)polyPath._childs[i], paths);
			}
		}

		public static List<List<PointD>> PolyTreeToPathsD(PolyTreeD polyTree)
		{
			List<List<PointD>> list = new List<List<PointD>>();
			foreach (PolyPathBase polyPathBase in polyTree)
			{
				Clipper.AddPolyNodeToPathsD((PolyPathD)polyPathBase, list);
			}
			return list;
		}

		public static double PerpendicDistFromLineSqrd(PointD pt, PointD line1, PointD line2)
		{
			double num = pt.x - line1.x;
			double num2 = pt.y - line1.y;
			double num3 = line2.x - line1.x;
			double num4 = line2.y - line1.y;
			if (num3 == 0.0 && num4 == 0.0)
			{
				return 0.0;
			}
			return Clipper.Sqr(num * num4 - num3 * num2) / (num3 * num3 + num4 * num4);
		}

		public static double PerpendicDistFromLineSqrd(Point64 pt, Point64 line1, Point64 line2)
		{
			double num = (double)pt.X - (double)line1.X;
			double num2 = (double)pt.Y - (double)line1.Y;
			double num3 = (double)line2.X - (double)line1.X;
			double num4 = (double)line2.Y - (double)line1.Y;
			if (num3 == 0.0 && num4 == 0.0)
			{
				return 0.0;
			}
			return Clipper.Sqr(num * num4 - num3 * num2) / (num3 * num3 + num4 * num4);
		}

		internal static void RDP(List<Point64> path, int begin, int end, double epsSqrd, List<bool> flags)
		{
			int num = 0;
			double num2 = 0.0;
			while (end > begin && path[begin] == path[end])
			{
				flags[end--] = false;
			}
			for (int i = begin + 1; i < end; i++)
			{
				double num3 = Clipper.PerpendicDistFromLineSqrd(path[i], path[begin], path[end]);
				if (num3 > num2)
				{
					num2 = num3;
					num = i;
				}
			}
			if (num2 <= epsSqrd)
			{
				return;
			}
			flags[num] = true;
			if (num > begin + 1)
			{
				Clipper.RDP(path, begin, num, epsSqrd, flags);
			}
			if (num < end - 1)
			{
				Clipper.RDP(path, num, end, epsSqrd, flags);
			}
		}

		public static List<Point64> RamerDouglasPeucker(List<Point64> path, double epsilon)
		{
			int count = path.Count;
			if (count < 5)
			{
				return path;
			}
			List<bool> list = new List<bool>(new bool[count]);
			list[0] = true;
			list[count - 1] = true;
			Clipper.RDP(path, 0, count - 1, Clipper.Sqr(epsilon), list);
			List<Point64> list2 = new List<Point64>(count);
			for (int i = 0; i < count; i++)
			{
				if (list[i])
				{
					list2.Add(path[i]);
				}
			}
			return list2;
		}

		public static List<List<Point64>> RamerDouglasPeucker(List<List<Point64>> paths, double epsilon)
		{
			List<List<Point64>> list = new List<List<Point64>>(paths.Count);
			foreach (List<Point64> path in paths)
			{
				list.Add(Clipper.RamerDouglasPeucker(path, epsilon));
			}
			return list;
		}

		internal static void RDP(List<PointD> path, int begin, int end, double epsSqrd, List<bool> flags)
		{
			int num = 0;
			double num2 = 0.0;
			while (end > begin && path[begin] == path[end])
			{
				flags[end--] = false;
			}
			for (int i = begin + 1; i < end; i++)
			{
				double num3 = Clipper.PerpendicDistFromLineSqrd(path[i], path[begin], path[end]);
				if (num3 > num2)
				{
					num2 = num3;
					num = i;
				}
			}
			if (num2 <= epsSqrd)
			{
				return;
			}
			flags[num] = true;
			if (num > begin + 1)
			{
				Clipper.RDP(path, begin, num, epsSqrd, flags);
			}
			if (num < end - 1)
			{
				Clipper.RDP(path, num, end, epsSqrd, flags);
			}
		}

		public static List<PointD> RamerDouglasPeucker(List<PointD> path, double epsilon)
		{
			int count = path.Count;
			if (count < 5)
			{
				return path;
			}
			List<bool> list = new List<bool>(new bool[count]);
			list[0] = true;
			list[count - 1] = true;
			Clipper.RDP(path, 0, count - 1, Clipper.Sqr(epsilon), list);
			List<PointD> list2 = new List<PointD>(count);
			for (int i = 0; i < count; i++)
			{
				if (list[i])
				{
					list2.Add(path[i]);
				}
			}
			return list2;
		}

		public static List<List<PointD>> RamerDouglasPeucker(List<List<PointD>> paths, double epsilon)
		{
			List<List<PointD>> list = new List<List<PointD>>(paths.Count);
			foreach (List<PointD> path in paths)
			{
				list.Add(Clipper.RamerDouglasPeucker(path, epsilon));
			}
			return list;
		}

		public static List<Point64> TrimCollinear(List<Point64> path, bool isOpen = false)
		{
			int num = path.Count;
			int i = 0;
			if (!isOpen)
			{
				while (i < num - 1)
				{
					if (InternalClipper.CrossProduct(path[num - 1], path[i], path[i + 1]) != 0.0)
					{
						break;
					}
					i++;
				}
				while (i < num - 1 && InternalClipper.CrossProduct(path[num - 2], path[num - 1], path[i]) == 0.0)
				{
					num--;
				}
			}
			if (num - i >= 3)
			{
				List<Point64> list = new List<Point64>(num - i);
				Point64 point = path[i];
				list.Add(point);
				for (i++; i < num - 1; i++)
				{
					if (InternalClipper.CrossProduct(point, path[i], path[i + 1]) != 0.0)
					{
						point = path[i];
						list.Add(point);
					}
				}
				if (isOpen)
				{
					list.Add(path[num - 1]);
				}
				else if (InternalClipper.CrossProduct(point, path[num - 1], list[0]) != 0.0)
				{
					list.Add(path[num - 1]);
				}
				else
				{
					while (list.Count > 2 && InternalClipper.CrossProduct(list[list.Count - 1], list[list.Count - 2], list[0]) == 0.0)
					{
						list.RemoveAt(list.Count - 1);
					}
					if (list.Count < 3)
					{
						list.Clear();
					}
				}
				return list;
			}
			if (!isOpen || num < 2 || path[0] == path[1])
			{
				return new List<Point64>();
			}
			return path;
		}

		public static List<PointD> TrimCollinear(List<PointD> path, int precision, bool isOpen = false)
		{
			if (precision < -8 || precision > 8)
			{
				throw new Exception("Error: Precision is out of range.");
			}
			double num = Math.Pow(10.0, (double)precision);
			return Clipper.ScalePathD(Clipper.TrimCollinear(Clipper.ScalePath64(path, num), isOpen), 1.0 / num);
		}

		public static PointInPolygonResult PointInPolygon(Point64 pt, List<Point64> polygon)
		{
			return InternalClipper.PointInPolygon(pt, polygon);
		}

		public static Rect64 MaxInvalidRect64 = new Rect64(long.MaxValue, long.MaxValue, long.MinValue, long.MinValue);

		public static RectD MaxInvalidRectD = new RectD(double.MaxValue, double.MinValue, double.MinValue, double.MinValue);
	}
}
