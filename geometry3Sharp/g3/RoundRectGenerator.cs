using System;

namespace g3
{
	public class RoundRectGenerator : MeshGenerator
	{
		public override MeshGenerator Generate()
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < 4; i++)
			{
				if ((this.SharpCorners & (RoundRectGenerator.Corner)(1 << i)) != (RoundRectGenerator.Corner)0)
				{
					num++;
					num2 += 2;
				}
				else
				{
					num += this.CornerSteps;
					num2 += this.CornerSteps + 1;
				}
			}
			this.vertices = new VectorArray3d(12 + num, false);
			this.uv = new VectorArray2f(this.vertices.Count);
			this.normals = new VectorArray3f(this.vertices.Count);
			this.triangles = new IndexArray3i(10 + num2);
			float num3 = this.Width - 2f * this.Radius;
			float num4 = this.Height - 2f * this.Radius;
			this.vertices[0] = new Vector3d((double)(-(double)num3 / 2f), 0.0, (double)(-(double)num4 / 2f));
			this.vertices[1] = new Vector3d((double)(num3 / 2f), 0.0, (double)(-(double)num4 / 2f));
			this.vertices[2] = new Vector3d((double)(num3 / 2f), 0.0, (double)(num4 / 2f));
			this.vertices[3] = new Vector3d((double)(-(double)num3 / 2f), 0.0, (double)(num4 / 2f));
			this.vertices[4] = new Vector3d((double)(-(double)num3 / 2f), 0.0, (double)(-(double)this.Height / 2f));
			this.vertices[5] = new Vector3d((double)(num3 / 2f), 0.0, (double)(-(double)this.Height / 2f));
			this.vertices[6] = new Vector3d((double)(this.Width / 2f), 0.0, (double)(-(double)num4 / 2f));
			this.vertices[7] = new Vector3d((double)(this.Width / 2f), 0.0, (double)(num4 / 2f));
			this.vertices[8] = new Vector3d((double)(num3 / 2f), 0.0, (double)(this.Height / 2f));
			this.vertices[9] = new Vector3d((double)(-(double)num3 / 2f), 0.0, (double)(this.Height / 2f));
			this.vertices[10] = new Vector3d((double)(-(double)this.Width / 2f), 0.0, (double)(num4 / 2f));
			this.vertices[11] = new Vector3d((double)(-(double)this.Width / 2f), 0.0, (double)(-(double)num4 / 2f));
			bool bCycle = !this.Clockwise;
			int num5 = 0;
			base.append_rectangle(0, 1, 2, 3, bCycle, ref num5, -1);
			base.append_rectangle(4, 5, 1, 0, bCycle, ref num5, -1);
			base.append_rectangle(1, 6, 7, 2, bCycle, ref num5, -1);
			base.append_rectangle(3, 2, 8, 9, bCycle, ref num5, -1);
			base.append_rectangle(11, 0, 3, 10, bCycle, ref num5, -1);
			int num6 = 12;
			for (int j = 0; j < 4; j++)
			{
				if ((this.SharpCorners & (RoundRectGenerator.Corner)(1 << j)) > (RoundRectGenerator.Corner)0)
				{
					base.append_2d_disc_segment(RoundRectGenerator.corner_spans[3 * j], RoundRectGenerator.corner_spans[3 * j + 1], RoundRectGenerator.corner_spans[3 * j + 2], 1, bCycle, ref num6, ref num5, -1, 1.4142135623730951 * (double)this.Radius);
				}
				else
				{
					base.append_2d_disc_segment(RoundRectGenerator.corner_spans[3 * j], RoundRectGenerator.corner_spans[3 * j + 1], RoundRectGenerator.corner_spans[3 * j + 2], this.CornerSteps, bCycle, ref num6, ref num5, -1, 0.0);
				}
			}
			for (int k = 0; k < this.vertices.Count; k++)
			{
				this.normals[k] = Vector3f.AxisY;
			}
			float num7 = 0f;
			float num8 = 1f;
			float num9 = 0f;
			float num10 = 1f;
			if (this.UVMode != RoundRectGenerator.UVModes.FullUVSquare)
			{
				if (this.Width > this.Height)
				{
					float num11 = this.Height / this.Width;
					if (this.UVMode == RoundRectGenerator.UVModes.CenteredUVRectangle)
					{
						num9 = 0.5f - num11 / 2f;
						num10 = 0.5f + num11 / 2f;
					}
					else
					{
						num10 = num11;
					}
				}
				else if (this.Height > this.Width)
				{
					float num12 = this.Width / this.Height;
					if (this.UVMode == RoundRectGenerator.UVModes.CenteredUVRectangle)
					{
						num7 = 0.5f - num12 / 2f;
						num8 = 0.5f + num12 / 2f;
					}
					else
					{
						num8 = num12;
					}
				}
			}
			Vector3d vector3d = new Vector3d((double)(-(double)this.Width / 2f), 0.0, (double)(-(double)this.Height / 2f));
			for (int l = 0; l < this.vertices.Count; l++)
			{
				Vector3d vector3d2 = this.vertices[l];
				double num13 = (vector3d2.x - vector3d.x) / (double)this.Width;
				double num14 = (vector3d2.y - vector3d.y) / (double)this.Height;
				this.uv[l] = new Vector2f((1.0 - num13) * (double)num7 + num13 * (double)num8, (1.0 - num14) * (double)num9 + num14 * (double)num10);
			}
			return this;
		}

		public Vector3d[] GetBorderLoop()
		{
			int num = 0;
			for (int i = 0; i < 4; i++)
			{
				if ((this.SharpCorners & (RoundRectGenerator.Corner)(1 << i)) != (RoundRectGenerator.Corner)0)
				{
					num++;
				}
				else
				{
					num += this.CornerSteps;
				}
			}
			float num2 = this.Width - 2f * this.Radius;
			float num3 = this.Height - 2f * this.Radius;
			Vector3d[] array = new Vector3d[4 + num];
			int num4 = 0;
			for (int j = 0; j < 4; j++)
			{
				array[num4++] = new Vector3d((double)(RoundRectGenerator.signx[j] * this.Width / 2f), 0.0, (double)(RoundRectGenerator.signy[j] * this.Height / 2f));
				bool flag = (this.SharpCorners & (RoundRectGenerator.Corner)(1 << j)) > (RoundRectGenerator.Corner)0;
				Arc2d arc2d = new Arc2d(new Vector2d(RoundRectGenerator.signx[j] * num2, RoundRectGenerator.signy[j] * num3), flag ? (1.4142135623730951 * (double)this.Radius) : ((double)this.Radius), (double)RoundRectGenerator.startangle[j], (double)RoundRectGenerator.endangle[j]);
				int num5 = flag ? 1 : this.CornerSteps;
				for (int k = 0; k < num5; k++)
				{
					double t = (double)(j + 1) / (double)(num5 + 1);
					Vector2d vector2d = arc2d.SampleT(t);
					array[num4++] = new Vector3d(vector2d.x, 0.0, vector2d.y);
				}
			}
			return array;
		}

		public float Width = 1f;

		public float Height = 1f;

		public float Radius = 0.1f;

		public int CornerSteps = 4;

		public RoundRectGenerator.Corner SharpCorners;

		public RoundRectGenerator.UVModes UVMode;

		private static int[] corner_spans = new int[]
		{
			0,
			11,
			4,
			1,
			5,
			6,
			2,
			7,
			8,
			3,
			9,
			10
		};

		private static readonly float[] signx = new float[]
		{
			1f,
			1f,
			-1f,
			-1f
		};

		private static readonly float[] signy = new float[]
		{
			-1f,
			1f,
			1f,
			-1f
		};

		private static readonly float[] startangle = new float[]
		{
			270f,
			0f,
			90f,
			180f
		};

		private static readonly float[] endangle = new float[]
		{
			360f,
			90f,
			180f,
			270f
		};

		[Flags]
		public enum Corner
		{
			BottomLeft = 1,
			BottomRight = 2,
			TopRight = 4,
			TopLeft = 8
		}

		public enum UVModes
		{
			FullUVSquare,
			CenteredUVRectangle,
			BottomCornerUVRectangle
		}
	}
}
