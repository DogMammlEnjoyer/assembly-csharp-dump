using System;
using System.Collections.Generic;

namespace g3
{
	public class ContMinCircle2
	{
		public ContMinCircle2(IList<Vector2d> pointsIn, double epsilon = 1E-05)
		{
			this.mEpsilon = epsilon;
			this.mUpdate[0] = null;
			this.mUpdate[1] = new Func<int, int[], ContMinCircle2.Support, ContMinCircle2.Circle>(this.UpdateSupport1);
			this.mUpdate[2] = new Func<int, int[], ContMinCircle2.Support, ContMinCircle2.Circle>(this.UpdateSupport2);
			this.mUpdate[3] = new Func<int, int[], ContMinCircle2.Support, ContMinCircle2.Circle>(this.UpdateSupport3);
			ContMinCircle2.Support support = new ContMinCircle2.Support();
			double num = 0.0;
			this.Points = pointsIn;
			int count = pointsIn.Count;
			Random random = new Random();
			if (count >= 1)
			{
				int[] array = new int[count];
				for (int i = 0; i < count; i++)
				{
					array[i] = i;
				}
				for (int j = count - 1; j > 0; j--)
				{
					int num2 = random.Next() % (j + 1);
					if (num2 != j)
					{
						int num3 = array[j];
						array[j] = array[num2];
						array[num2] = num3;
					}
				}
				ContMinCircle2.Circle circle = new ContMinCircle2.Circle(this.Points[array[0]], 0.0);
				support.Quantity = 1;
				support.Index[0] = 0;
				int num4 = 1 % count;
				int num5 = 0;
				while (num4 != num5)
				{
					if (!support.Contains(num4, this.Points, array, this.mEpsilon) && !this.Contains(this.Points[array[num4]], ref circle, ref num))
					{
						ContMinCircle2.Circle circle2 = this.mUpdate[support.Quantity](num4, array, support);
						if (circle2.Radius > circle.Radius)
						{
							circle = circle2;
							num5 = num4;
						}
					}
					num4 = (num4 + 1) % count;
				}
				this.Result = new Circle2d(circle.Center, Math.Sqrt(circle.Radius));
				return;
			}
			throw new Exception("ContMinCircle2: Input must contain points\n");
		}

		private bool Contains(Vector2d point, ref ContMinCircle2.Circle circle, ref double distDiff)
		{
			double lengthSquared = (point - circle.Center).LengthSquared;
			distDiff = lengthSquared - circle.Radius;
			return distDiff <= 0.0;
		}

		private ContMinCircle2.Circle ExactCircle2(ref Vector2d P0, ref Vector2d P1)
		{
			return new ContMinCircle2.Circle(0.5 * (P0 + P1), 0.25 * P1.DistanceSquared(P0));
		}

		private ContMinCircle2.Circle ExactCircle2(Vector2d P0, ref Vector2d P1)
		{
			return new ContMinCircle2.Circle(0.5 * (P0 + P1), 0.25 * P1.DistanceSquared(P0));
		}

		private ContMinCircle2.Circle ExactCircle3(ref Vector2d P0, ref Vector2d P1, ref Vector2d P2)
		{
			Vector2d vector2d = P1 - P0;
			Vector2d vector2d2 = P2 - P0;
			Matrix2d matrix2d = new Matrix2d(vector2d.x, vector2d.y, vector2d2.x, vector2d2.y);
			Vector2d vector2d3 = new Vector2d(0.5 * vector2d.LengthSquared, 0.5 * vector2d2.LengthSquared);
			double num = matrix2d.m00 * matrix2d.m11 - matrix2d.m01 * matrix2d.m10;
			if (Math.Abs(num) > this.mEpsilon)
			{
				double num2 = 1.0 / num;
				Vector2d o;
				o.x = (matrix2d.m11 * vector2d3.x - matrix2d.m01 * vector2d3.y) * num2;
				o.y = (matrix2d.m00 * vector2d3.y - matrix2d.m10 * vector2d3.x) * num2;
				return new ContMinCircle2.Circle(P0 + o, o.LengthSquared);
			}
			return new ContMinCircle2.Circle(Vector2d.Zero, double.MaxValue);
		}

		private ContMinCircle2.Circle ExactCircle3(Vector2d P0, Vector2d P1, ref Vector2d P2)
		{
			return this.ExactCircle3(ref P0, ref P1, ref P2);
		}

		private ContMinCircle2.Circle ExactCircle3(Vector2d P0, ref Vector2d P1, ref Vector2d P2)
		{
			return this.ExactCircle3(ref P0, ref P1, ref P2);
		}

		private ContMinCircle2.Circle UpdateSupport1(int i, int[] permutation, ContMinCircle2.Support support)
		{
			Vector2d vector2d = this.Points[permutation[support.Index[0]]];
			Vector2d vector2d2 = this.Points[permutation[i]];
			ContMinCircle2.Circle result = this.ExactCircle2(ref vector2d, ref vector2d2);
			support.Quantity = 2;
			support.Index[1] = i;
			return result;
		}

		private ContMinCircle2.Circle UpdateSupport2(int i, int[] permutation, ContMinCircle2.Support support)
		{
			Vector2dTuple2 vector2dTuple = new Vector2dTuple2(this.Points[permutation[support.Index[0]]], this.Points[permutation[support.Index[1]]]);
			Vector2d vector2d = this.Points[permutation[i]];
			int num = 2;
			ContMinCircle2.Circle[] array = this.circle_buf;
			int num2 = 0;
			double num3 = double.MaxValue;
			int num4 = -1;
			double num5 = 0.0;
			double num6 = double.MaxValue;
			int num7 = -1;
			int j = 0;
			while (j < num)
			{
				array[num2] = this.ExactCircle2(vector2dTuple[ContMinCircle2.type2_2[j, 0]], ref vector2d);
				if (array[num2].Radius < num3)
				{
					if (this.Contains(vector2dTuple[ContMinCircle2.type2_2[j, 1]], ref array[num2], ref num5))
					{
						num3 = array[num2].Radius;
						num4 = num2;
					}
					else if (num5 < num6)
					{
						num6 = num5;
						num7 = num2;
					}
				}
				j++;
				num2++;
			}
			array[num2] = this.ExactCircle3(vector2dTuple[0], vector2dTuple[1], ref vector2d);
			if (array[num2].Radius < num3)
			{
				num3 = array[num2].Radius;
				num4 = num2;
			}
			if (num4 == -1)
			{
				num4 = num7;
			}
			ContMinCircle2.Circle result = array[num4];
			switch (num4)
			{
			case 0:
				support.Index[1] = i;
				break;
			case 1:
				support.Index[0] = i;
				break;
			case 2:
				support.Quantity = 3;
				support.Index[2] = i;
				break;
			}
			return result;
		}

		private ContMinCircle2.Circle UpdateSupport3(int i, int[] permutation, ContMinCircle2.Support support)
		{
			Vector2dTuple3 vector2dTuple = new Vector2dTuple3(this.Points[permutation[support.Index[0]]], this.Points[permutation[support.Index[1]]], this.Points[permutation[support.Index[2]]]);
			Vector2d vector2d = this.Points[permutation[i]];
			int num = 3;
			int num2 = 3;
			ContMinCircle2.Circle[] array = this.circle_buf;
			int num3 = 0;
			double num4 = double.MaxValue;
			int num5 = -1;
			double num6 = 0.0;
			double num7 = double.MaxValue;
			int num8 = -1;
			int j = 0;
			while (j < num)
			{
				array[num3] = this.ExactCircle2(vector2dTuple[ContMinCircle2.type2_3[j, 0]], ref vector2d);
				if (array[num3].Radius < num4)
				{
					if (this.Contains(vector2dTuple[ContMinCircle2.type2_3[j, 1]], ref array[num3], ref num6) && this.Contains(vector2dTuple[ContMinCircle2.type2_3[j, 2]], ref array[num3], ref num6))
					{
						num4 = array[num3].Radius;
						num5 = num3;
					}
					else if (num6 < num7)
					{
						num7 = num6;
						num8 = num3;
					}
				}
				j++;
				num3++;
			}
			j = 0;
			while (j < num2)
			{
				array[num3] = this.ExactCircle3(vector2dTuple[ContMinCircle2.type3_3[j, 0]], vector2dTuple[ContMinCircle2.type3_3[j, 1]], ref vector2d);
				if (array[num3].Radius < num4)
				{
					if (this.Contains(vector2dTuple[ContMinCircle2.type3_3[j, 2]], ref array[num3], ref num6))
					{
						num4 = array[num3].Radius;
						num5 = num3;
					}
					else if (num6 < num7)
					{
						num7 = num6;
						num8 = num3;
					}
				}
				j++;
				num3++;
			}
			if (num5 == -1)
			{
				num5 = num8;
			}
			ContMinCircle2.Circle result = array[num5];
			switch (num5)
			{
			case 0:
				support.Quantity = 2;
				support.Index[1] = i;
				break;
			case 1:
				support.Quantity = 2;
				support.Index[0] = i;
				break;
			case 2:
				support.Quantity = 2;
				support.Index[0] = support.Index[2];
				support.Index[1] = i;
				break;
			case 3:
				support.Index[2] = i;
				break;
			case 4:
				support.Index[1] = i;
				break;
			case 5:
				support.Index[0] = i;
				break;
			}
			return result;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static ContMinCircle2()
		{
			int[,] array = new int[2, 2];
			array[0, 1] = 1;
			array[1, 0] = 1;
			ContMinCircle2.type2_2 = array;
			ContMinCircle2.type2_3 = new int[,]
			{
				{
					0,
					1,
					2
				},
				{
					1,
					0,
					2
				},
				{
					2,
					0,
					1
				}
			};
			ContMinCircle2.type3_3 = new int[,]
			{
				{
					0,
					1,
					2
				},
				{
					0,
					2,
					1
				},
				{
					1,
					2,
					0
				}
			};
		}

		private double mEpsilon;

		private Func<int, int[], ContMinCircle2.Support, ContMinCircle2.Circle>[] mUpdate = new Func<int, int[], ContMinCircle2.Support, ContMinCircle2.Circle>[4];

		private IList<Vector2d> Points;

		private ContMinCircle2.Circle[] circle_buf = new ContMinCircle2.Circle[6];

		public Circle2d Result;

		private static readonly int[,] type2_2;

		private static readonly int[,] type2_3;

		private static readonly int[,] type3_3;

		private struct Circle
		{
			public Circle(Vector2d c, double radius)
			{
				this.Center = c;
				this.Radius = radius;
			}

			public Vector2d Center;

			public double Radius;
		}

		protected class Support
		{
			public bool Contains(int index, IList<Vector2d> Points, int[] permutation, double epsilon)
			{
				for (int i = 0; i < this.Quantity; i++)
				{
					if ((Points[permutation[index]] - Points[permutation[this.Index[i]]]).LengthSquared < epsilon)
					{
						return true;
					}
				}
				return false;
			}

			public int Quantity;

			public Index3i Index;
		}
	}
}
