using System;
using System.Collections.Generic;

namespace Unity.Cinemachine
{
	internal static class InternalClipper
	{
		internal static double CrossProduct(Point64 pt1, Point64 pt2, Point64 pt3)
		{
			return (double)(pt2.X - pt1.X) * (double)(pt3.Y - pt2.Y) - (double)(pt2.Y - pt1.Y) * (double)(pt3.X - pt2.X);
		}

		internal static double DotProduct(Point64 pt1, Point64 pt2, Point64 pt3)
		{
			return (double)(pt2.X - pt1.X) * (double)(pt3.X - pt2.X) + (double)(pt2.Y - pt1.Y) * (double)(pt3.Y - pt2.Y);
		}

		internal static double DotProduct(PointD vec1, PointD vec2)
		{
			return vec1.x * vec2.x + vec1.y * vec2.y;
		}

		internal static bool GetIntersectPoint(Point64 ln1a, Point64 ln1b, Point64 ln2a, Point64 ln2b, out PointD ip)
		{
			ip = default(PointD);
			if (ln1b.X == ln1a.X)
			{
				if (ln2b.X == ln2a.X)
				{
					return false;
				}
				double num = (double)(ln2b.Y - ln2a.Y) / (double)(ln2b.X - ln2a.X);
				double num2 = (double)ln2a.Y - num * (double)ln2a.X;
				ip.x = (double)ln1a.X;
				ip.y = num * (double)ln1a.X + num2;
			}
			else if (ln2b.X == ln2a.X)
			{
				double num3 = (double)(ln1b.Y - ln1a.Y) / (double)(ln1b.X - ln1a.X);
				double num4 = (double)ln1a.Y - num3 * (double)ln1a.X;
				ip.x = (double)ln2a.X;
				ip.y = num3 * (double)ln2a.X + num4;
			}
			else
			{
				double num3 = (double)(ln1b.Y - ln1a.Y) / (double)(ln1b.X - ln1a.X);
				double num4 = (double)ln1a.Y - num3 * (double)ln1a.X;
				double num = (double)(ln2b.Y - ln2a.Y) / (double)(ln2b.X - ln2a.X);
				double num2 = (double)ln2a.Y - num * (double)ln2a.X;
				if (Math.Abs(num3 - num) > 1E-15)
				{
					ip.x = (num2 - num4) / (num3 - num);
					ip.y = num3 * ip.x + num4;
				}
				else
				{
					ip.x = (double)(ln1a.X + ln1b.X) * 0.5;
					ip.y = (double)(ln1a.Y + ln1b.Y) * 0.5;
				}
			}
			return true;
		}

		internal static bool SegmentsIntersect(Point64 seg1a, Point64 seg1b, Point64 seg2a, Point64 seg2b)
		{
			double num = (double)(seg1a.X - seg1b.X);
			double num2 = (double)(seg1a.Y - seg1b.Y);
			double num3 = (double)(seg2a.X - seg2b.X);
			double num4 = (double)(seg2a.Y - seg2b.Y);
			return (num2 * (double)(seg2a.X - seg1a.X) - num * (double)(seg2a.Y - seg1a.Y)) * (num2 * (double)(seg2b.X - seg1a.X) - num * (double)(seg2b.Y - seg1a.Y)) < 0.0 && (num4 * (double)(seg1a.X - seg2a.X) - num3 * (double)(seg1a.Y - seg2a.Y)) * (num4 * (double)(seg1b.X - seg2a.X) - num3 * (double)(seg1b.Y - seg2a.Y)) < 0.0;
		}

		public static PointInPolygonResult PointInPolygon(Point64 pt, List<Point64> polygon)
		{
			int count = polygon.Count;
			int i = count - 1;
			if (count < 3)
			{
				return PointInPolygonResult.IsOutside;
			}
			while (i >= 0 && polygon[i].Y == pt.Y)
			{
				i--;
			}
			if (i < 0)
			{
				return PointInPolygonResult.IsOutside;
			}
			int num = 0;
			bool flag = polygon[i].Y < pt.Y;
			i = 0;
			while (i < count)
			{
				if (flag)
				{
					while (i < count && polygon[i].Y < pt.Y)
					{
						i++;
					}
					if (i == count)
					{
						break;
					}
				}
				else
				{
					while (i < count && polygon[i].Y > pt.Y)
					{
						i++;
					}
					if (i == count)
					{
						break;
					}
				}
				Point64 point = polygon[i];
				Point64 point2;
				if (i > 0)
				{
					point2 = polygon[i - 1];
				}
				else
				{
					point2 = polygon[count - 1];
				}
				if (point.Y == pt.Y)
				{
					if (point.X == pt.X || (point.Y == point2.Y && pt.X < point2.X != pt.X < point.X))
					{
						return PointInPolygonResult.IsOn;
					}
					i++;
				}
				else
				{
					if (pt.X >= point.X || pt.X >= point2.X)
					{
						if (pt.X > point2.X && pt.X > point.X)
						{
							num = 1 - num;
						}
						else
						{
							double num2 = InternalClipper.CrossProduct(point2, point, pt);
							if (num2 == 0.0)
							{
								return PointInPolygonResult.IsOn;
							}
							if (num2 < 0.0 == flag)
							{
								num = 1 - num;
							}
						}
					}
					flag = !flag;
					i++;
				}
			}
			if (num == 0)
			{
				return PointInPolygonResult.IsOutside;
			}
			return PointInPolygonResult.IsInside;
		}

		internal const double floatingPointTolerance = 1E-15;

		internal const double defaultMinimumEdgeLength = 0.1;
	}
}
