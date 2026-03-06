using System;
using System.Collections.Generic;

namespace g3
{
	public class ContMinBox2
	{
		public Box2d MinBox
		{
			get
			{
				return this.mMinBox;
			}
		}

		public ContMinBox2(IList<Vector2d> points, double epsilon, QueryNumberType queryType, bool isConvexPolygon)
		{
			IList<Vector2d> list;
			int num;
			if (isConvexPolygon)
			{
				list = points;
				num = list.Count;
			}
			else
			{
				ConvexHull2 convexHull = new ConvexHull2(points, epsilon, queryType);
				int dimension = convexHull.Dimension;
				int numSimplices = convexHull.NumSimplices;
				int[] hullIndices = convexHull.HullIndices;
				if (dimension == 0)
				{
					this.mMinBox.Center = points[0];
					this.mMinBox.AxisX = Vector2d.AxisX;
					this.mMinBox.AxisY = Vector2d.AxisY;
					this.mMinBox.Extent[0] = 0.0;
					this.mMinBox.Extent[1] = 0.0;
					return;
				}
				if (dimension == 1)
				{
					throw new NotImplementedException("ContMinBox2: Have not implemented 1d case");
				}
				num = numSimplices;
				Vector2d[] array = new Vector2d[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = points[hullIndices[i]];
				}
				list = array;
			}
			int num2 = num - 1;
			Vector2d[] array2 = new Vector2d[num];
			bool[] array3 = new bool[num];
			for (int j = 0; j < num2; j++)
			{
				array2[j] = list[j + 1] - list[j];
				array2[j].Normalize(2.220446049250313E-16);
				array3[j] = false;
			}
			array2[num2] = list[0] - list[num2];
			array2[num2].Normalize(2.220446049250313E-16);
			array3[num2] = false;
			double x = list[0].x;
			double num3 = x;
			double y = list[0].y;
			double num4 = y;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			for (int k = 1; k < num; k++)
			{
				if (list[k].x <= x)
				{
					x = list[k].x;
					num5 = k;
				}
				if (list[k].x >= num3)
				{
					num3 = list[k].x;
					num6 = k;
				}
				if (list[k].y <= y)
				{
					y = list[k].y;
					num7 = k;
				}
				if (list[k].y >= num4)
				{
					num4 = list[k].y;
					num8 = k;
				}
			}
			if (num5 == num2 && list[0].x <= x)
			{
				x = list[0].x;
				num5 = 0;
			}
			if (num6 == num2 && list[0].x >= num3)
			{
				num3 = list[0].x;
				num6 = 0;
			}
			if (num7 == num2 && list[0].y <= y)
			{
				y = list[0].y;
				num7 = 0;
			}
			if (num8 == num2 && list[0].y >= num4)
			{
				num4 = list[0].y;
				num8 = 0;
			}
			this.mMinBox.Center.x = 0.5 * (x + num3);
			this.mMinBox.Center.y = 0.5 * (y + num4);
			this.mMinBox.AxisX = Vector2d.AxisX;
			this.mMinBox.AxisY = Vector2d.AxisY;
			this.mMinBox.Extent[0] = 0.5 * (num3 - x);
			this.mMinBox.Extent[1] = 0.5 * (num4 - y);
			double num9 = this.mMinBox.Extent[0] * this.mMinBox.Extent[1];
			Vector2d vector2d = Vector2d.AxisX;
			Vector2d vector2d2 = Vector2d.AxisY;
			bool flag = false;
			while (!flag)
			{
				ContMinBox2.RCFlags rcflags = ContMinBox2.RCFlags.F_NONE;
				double num10 = 0.0;
				double num11 = vector2d.Dot(array2[num7]);
				if (num11 > num10)
				{
					num10 = num11;
					rcflags = ContMinBox2.RCFlags.F_BOTTOM;
				}
				num11 = vector2d2.Dot(array2[num6]);
				if (num11 > num10)
				{
					num10 = num11;
					rcflags = ContMinBox2.RCFlags.F_RIGHT;
				}
				num11 = -vector2d.Dot(array2[num8]);
				if (num11 > num10)
				{
					num10 = num11;
					rcflags = ContMinBox2.RCFlags.F_TOP;
				}
				num11 = -vector2d2.Dot(array2[num5]);
				if (num11 > num10)
				{
					rcflags = ContMinBox2.RCFlags.F_LEFT;
				}
				switch (rcflags)
				{
				case ContMinBox2.RCFlags.F_NONE:
					flag = true;
					break;
				case ContMinBox2.RCFlags.F_LEFT:
					if (array3[num5])
					{
						flag = true;
					}
					else
					{
						vector2d2 = -array2[num5];
						vector2d = vector2d2.Perp;
						this.UpdateBox(list[num5], list[num6], list[num7], list[num8], ref vector2d, ref vector2d2, ref num9);
						array3[num5] = true;
						if (++num5 == num)
						{
							num5 = 0;
						}
					}
					break;
				case ContMinBox2.RCFlags.F_RIGHT:
					if (array3[num6])
					{
						flag = true;
					}
					else
					{
						vector2d2 = array2[num6];
						vector2d = vector2d2.Perp;
						this.UpdateBox(list[num5], list[num6], list[num7], list[num8], ref vector2d, ref vector2d2, ref num9);
						array3[num6] = true;
						if (++num6 == num)
						{
							num6 = 0;
						}
					}
					break;
				case ContMinBox2.RCFlags.F_BOTTOM:
					if (array3[num7])
					{
						flag = true;
					}
					else
					{
						vector2d = array2[num7];
						vector2d2 = -vector2d.Perp;
						this.UpdateBox(list[num5], list[num6], list[num7], list[num8], ref vector2d, ref vector2d2, ref num9);
						array3[num7] = true;
						if (++num7 == num)
						{
							num7 = 0;
						}
					}
					break;
				case ContMinBox2.RCFlags.F_TOP:
					if (array3[num8])
					{
						flag = true;
					}
					else
					{
						vector2d = -array2[num8];
						vector2d2 = -vector2d.Perp;
						this.UpdateBox(list[num5], list[num6], list[num7], list[num8], ref vector2d, ref vector2d2, ref num9);
						array3[num8] = true;
						if (++num8 == num)
						{
							num8 = 0;
						}
					}
					break;
				}
			}
		}

		protected void UpdateBox(Vector2d LPoint, Vector2d RPoint, Vector2d BPoint, Vector2d TPoint, ref Vector2d U, ref Vector2d V, ref double minAreaDiv4)
		{
			Vector2d v = RPoint - LPoint;
			Vector2d v2 = TPoint - BPoint;
			double num = 0.5 * U.Dot(v);
			double num2 = 0.5 * V.Dot(v2);
			double num3 = num * num2;
			if (num3 < minAreaDiv4)
			{
				minAreaDiv4 = num3;
				this.mMinBox.AxisX = U;
				this.mMinBox.AxisY = V;
				this.mMinBox.Extent[0] = num;
				this.mMinBox.Extent[1] = num2;
				Vector2d v3 = LPoint - BPoint;
				this.mMinBox.Center = LPoint + U * num + V * (num2 - V.Dot(v3));
			}
		}

		private Box2d mMinBox;

		protected enum RCFlags
		{
			F_NONE,
			F_LEFT,
			F_RIGHT,
			F_BOTTOM,
			F_TOP
		}
	}
}
