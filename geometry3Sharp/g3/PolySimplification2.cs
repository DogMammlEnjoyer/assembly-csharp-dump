using System;
using System.Collections.Generic;

namespace g3
{
	public class PolySimplification2
	{
		public PolySimplification2(Polygon2d polygon)
		{
			this.Vertices = new List<Vector2d>(polygon.Vertices);
			this.IsLoop = true;
		}

		public PolySimplification2(PolyLine2d polycurve)
		{
			this.Vertices = new List<Vector2d>(polycurve.Vertices);
			this.IsLoop = false;
		}

		public static void Simplify(GeneralPolygon2d solid, double deviationThresh)
		{
			PolySimplification2 polySimplification = new PolySimplification2(solid.Outer);
			polySimplification.SimplifyDeviationThreshold = deviationThresh;
			polySimplification.Simplify();
			solid.Outer.SetVertices(polySimplification.Result, true);
			foreach (Polygon2d polygon2d in solid.Holes)
			{
				PolySimplification2 polySimplification2 = new PolySimplification2(polygon2d);
				polySimplification2.SimplifyDeviationThreshold = deviationThresh;
				polySimplification2.Simplify();
				polygon2d.SetVertices(polySimplification2.Result, true);
			}
		}

		public void Simplify()
		{
			bool[] array = new bool[this.Vertices.Count];
			Array.Clear(array, 0, array.Length);
			List<Vector2d> list = this.collapse_by_deviation_tol(this.Vertices, array, this.StraightLineDeviationThreshold);
			this.find_constrained_segments(list, array);
			this.Result = this.collapse_by_deviation_tol(list, array, this.SimplifyDeviationThreshold);
		}

		private void find_constrained_segments(List<Vector2d> vertices, bool[] markers)
		{
			int count = vertices.Count;
			int num = this.IsLoop ? vertices.Count : (vertices.Count - 1);
			for (int i = 0; i < num; i++)
			{
				int num2 = i;
				int index = (i + 1) % count;
				if (vertices[num2].DistanceSquared(vertices[index]) > this.PreserveStraightSegLen)
				{
					markers[num2] = true;
				}
			}
		}

		private List<Vector2d> collapse_by_deviation_tol(List<Vector2d> input, bool[] keep_segments, double offset_threshold)
		{
			int count = input.Count;
			int num = this.IsLoop ? input.Count : (input.Count - 1);
			List<Vector2d> list = new List<Vector2d>();
			list.Add(input[0]);
			double num2 = offset_threshold * offset_threshold;
			int num3 = 0;
			int i = 1;
			int num4 = 0;
			if (keep_segments[0])
			{
				list.Add(input[1]);
				num3 = 1;
				i = 2;
			}
			while (i < num)
			{
				int num5 = i;
				int num6 = (i + 1) % count;
				if (keep_segments[num5])
				{
					if (num3 != num5 && input[num5].Distance(list[list.Count - 1]) > 2.220446049250313E-16)
					{
						list.Add(input[num5]);
					}
					list.Add(input[num6]);
					num3 = num6;
					num4 = 0;
					if (num6 == 0)
					{
						i = num;
					}
					else
					{
						i = num6;
					}
				}
				else
				{
					Vector2d vector2d = input[num6] - input[num3];
					Line2d line2d = new Line2d(input[num3], vector2d.Normalized);
					double num7 = 0.0;
					for (int j = num3 + 1; j <= i; j++)
					{
						double num8 = line2d.DistanceSquared(input[j]);
						if (num8 > num7)
						{
							num7 = num8;
						}
					}
					if (num7 > num2)
					{
						list.Add(input[i]);
						num3 = i;
						i++;
						num4 = 0;
					}
					else
					{
						i++;
						num4++;
					}
				}
			}
			if (this.IsLoop)
			{
				if (list.Count < 3)
				{
					return this.handle_tiny_case(list, input, keep_segments, offset_threshold);
				}
				Line2d line2d2 = Line2d.FromPoints(input[num3], input[i % count]);
				bool flag = line2d2.DistanceSquared(list[0]) < num2;
				bool flag2 = line2d2.DistanceSquared(list[1]) < num2;
				if (flag && flag2 && list.Count > 3)
				{
					list[0] = input[num3];
					list.RemoveAt(list.Count - 1);
				}
				else if (!flag)
				{
					list.Add(input[input.Count - 1]);
				}
			}
			else
			{
				list.Add(input[input.Count - 1]);
			}
			return list;
		}

		private List<Vector2d> handle_tiny_case(List<Vector2d> result, List<Vector2d> input, bool[] keep_segments, double offset_threshold)
		{
			int count = input.Count;
			if (count == 3)
			{
				return input;
			}
			result.Clear();
			result.Add(input[0]);
			result.Add(input[count / 3]);
			result.Add(input[count - count / 3]);
			return result;
		}

		private List<Vector2d> Vertices;

		private bool IsLoop;

		public double StraightLineDeviationThreshold = 0.01;

		public double PreserveStraightSegLen = 2.0;

		public double SimplifyDeviationThreshold = 0.2;

		public List<Vector2d> Result;
	}
}
