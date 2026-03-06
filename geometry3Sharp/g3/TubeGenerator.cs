using System;
using System.Collections.Generic;

namespace g3
{
	public class TubeGenerator : MeshGenerator
	{
		public TubeGenerator()
		{
		}

		public TubeGenerator(Polygon2d tubePath, Frame3f pathPlane, Polygon2d tubeShape, int nPlaneNormal = 2)
		{
			this.Vertices = new List<Vector3d>();
			foreach (Vector2d v in tubePath.Vertices)
			{
				this.Vertices.Add(pathPlane.FromPlaneUV((Vector2f)v, nPlaneNormal));
			}
			this.Polygon = new Polygon2d(tubeShape);
			this.ClosedLoop = true;
			this.Capped = false;
		}

		public TubeGenerator(PolyLine2d tubePath, Frame3f pathPlane, Polygon2d tubeShape, int nPlaneNormal = 2)
		{
			this.Vertices = new List<Vector3d>();
			foreach (Vector2d v in tubePath.Vertices)
			{
				this.Vertices.Add(pathPlane.FromPlaneUV((Vector2f)v, nPlaneNormal));
			}
			this.Polygon = new Polygon2d(tubeShape);
			this.ClosedLoop = false;
			this.Capped = true;
		}

		public TubeGenerator(DCurve3 tubePath, Polygon2d tubeShape)
		{
			this.Vertices = new List<Vector3d>(tubePath.Vertices);
			this.Polygon = new Polygon2d(tubeShape);
			this.ClosedLoop = tubePath.Closed;
			this.Capped = !this.ClosedLoop;
		}

		public override MeshGenerator Generate()
		{
			if (this.Polygon == null)
			{
				this.Polygon = Polygon2d.MakeCircle(1.0, 8, 0.0);
			}
			int count = this.Vertices.Count;
			int vertexCount = this.Polygon.VertexCount;
			int num = (this.ClosedLoop && this.NoSharedVertices) ? (count + 1) : count;
			int num2 = this.NoSharedVertices ? (vertexCount + 1) : vertexCount;
			int num3 = this.NoSharedVertices ? (vertexCount + 1) : 1;
			if (!this.Capped || this.ClosedLoop)
			{
				num3 = 0;
			}
			this.vertices = new VectorArray3d(num * num2 + 2 * num3, false);
			this.uv = new VectorArray2f(this.vertices.Count);
			this.normals = new VectorArray3f(this.vertices.Count);
			int num4 = (this.ClosedLoop ? count : (count - 1)) * (2 * vertexCount);
			int num5 = (this.Capped && !this.ClosedLoop) ? (2 * vertexCount) : 0;
			this.triangles = new IndexArray3i(num4 + num5);
			Frame3f copy = new Frame3f(this.Frame);
			Vector3d tangent = CurveUtils.GetTangent(this.Vertices, 0, this.ClosedLoop);
			copy.Origin = (Vector3f)this.Vertices[0];
			copy.AlignAxis(2, (Vector3f)tangent);
			Frame3f frame3f = new Frame3f(copy);
			double arcLength = this.Polygon.ArcLength;
			double num6 = CurveUtils.ArcLength(this.Vertices, this.ClosedLoop);
			double num7 = 0.0;
			for (int i = 0; i < num; i++)
			{
				int num8 = i % count;
				Vector3d tangent2 = CurveUtils.GetTangent(this.Vertices, num8, this.ClosedLoop);
				copy.Origin = (Vector3f)this.Vertices[num8];
				copy.AlignAxis(2, (Vector3f)tangent2);
				int num9 = i * num2;
				double num10 = 0.0;
				for (int j = 0; j < num2; j++)
				{
					int i2 = num9 + j;
					Vector2d v = this.Polygon.Vertices[j % vertexCount];
					Vector2d v2 = this.Polygon.Vertices[(j + 1) % vertexCount];
					Vector3d vector3d = copy.FromPlaneUV((Vector2f)v, 2);
					this.vertices[i2] = vector3d;
					this.uv[i2] = new Vector2f(num7, num10);
					num10 += v.Distance(v2) / arcLength;
					Vector3f value = (Vector3f)(vector3d - copy.Origin).Normalized;
					this.normals[i2] = value;
				}
				int index = (i + 1) % count;
				double num11 = this.Vertices[num8].Distance(this.Vertices[index]);
				num7 += num11 / num6;
			}
			int num12 = 0;
			int num13 = (this.ClosedLoop && !this.NoSharedVertices) ? num : (num - 1);
			for (int k = 0; k < num13; k++)
			{
				int num14 = k * num2;
				int num15 = num14 + num2;
				if (this.ClosedLoop && k == num13 - 1 && !this.NoSharedVertices)
				{
					num15 = 0;
				}
				for (int l = 0; l < num2 - 1; l++)
				{
					this.triangles.Set(num12++, num14 + l, num14 + l + 1, num15 + l + 1, this.Clockwise);
					this.triangles.Set(num12++, num14 + l, num15 + l + 1, num15 + l, this.Clockwise);
				}
				if (!this.NoSharedVertices)
				{
					int num16 = num2 - 1;
					this.triangles.Set(num12++, num14 + num16, num14, num15, this.Clockwise);
					this.triangles.Set(num12++, num14 + num16, num15, num15 + num16, this.Clockwise);
				}
			}
			if (this.Capped && !this.ClosedLoop)
			{
				Vector2d vector2d = this.OverrideCapCenter ? this.CapCenter : this.Polygon.Bounds.Center;
				int num17 = num * num2;
				this.vertices[num17] = frame3f.FromPlaneUV((Vector2f)vector2d, 2);
				this.uv[num17] = new Vector2f(0.5f, 0.5f);
				this.normals[num17] = -frame3f.Z;
				this.startCapCenterIndex = num17;
				int num18 = num17 + 1;
				this.vertices[num18] = copy.FromPlaneUV((Vector2f)vector2d, 2);
				this.uv[num18] = new Vector2f(0.5f, 0.5f);
				this.normals[num18] = copy.Z;
				this.endCapCenterIndex = num18;
				if (this.NoSharedVertices)
				{
					int num19 = 0;
					int num20 = num18 + 1;
					for (int m = 0; m < vertexCount; m++)
					{
						this.vertices[num20 + m] = this.vertices[num19 + m];
						Vector2d v3 = ((this.Polygon[m] - vector2d).Normalized + Vector2d.One) * 0.5;
						this.uv[num20 + m] = (Vector2f)v3;
						this.normals[num20 + m] = this.normals[num17];
					}
					base.append_disc(vertexCount, num17, num20, true, this.Clockwise, ref num12, -1);
					int num21 = num2 * (num - 1);
					int num22 = num20 + vertexCount;
					for (int n = 0; n < vertexCount; n++)
					{
						this.vertices[num22 + n] = this.vertices[num21 + n];
						this.uv[num22 + n] = this.uv[num20 + n];
						this.normals[num22 + n] = this.normals[num18];
					}
					base.append_disc(vertexCount, num18, num22, true, !this.Clockwise, ref num12, -1);
				}
				else
				{
					base.append_disc(vertexCount, num17, 0, true, this.Clockwise, ref num12, -1);
					base.append_disc(vertexCount, num18, num2 * (num - 1), true, !this.Clockwise, ref num12, -1);
				}
			}
			return this;
		}

		public List<Vector3d> Vertices;

		public Polygon2d Polygon;

		public bool Capped = true;

		public bool OverrideCapCenter;

		public Vector2d CapCenter = Vector2d.Zero;

		public bool ClosedLoop;

		public Frame3f Frame = Frame3f.Identity;

		public bool NoSharedVertices = true;

		public int startCapCenterIndex = -1;

		public int endCapCenterIndex = -1;
	}
}
