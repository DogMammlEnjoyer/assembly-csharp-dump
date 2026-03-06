using System;
using System.Linq;

namespace g3
{
	public class VerticalGeneralizedCylinderGenerator : MeshGenerator
	{
		public override MeshGenerator Generate()
		{
			int num = this.NoSharedVertices ? (2 * (this.Sections.Length - 1)) : this.Sections.Length;
			int num2 = this.NoSharedVertices ? (this.Slices + 1) : this.Slices;
			int num3 = this.NoSharedVertices ? (this.Slices + 1) : 1;
			if (!this.Capped)
			{
				num3 = 0;
			}
			this.vertices = new VectorArray3d(num * num2 + 2 * num3, false);
			this.uv = new VectorArray2f(this.vertices.Count);
			this.normals = new VectorArray3f(this.vertices.Count);
			int num4 = (this.Sections.Length - 1) * (2 * this.Slices);
			int num5 = this.Capped ? (2 * this.Slices) : 0;
			this.triangles = new IndexArray3i(num4 + num5);
			float num6 = (float)(6.283185307179586 / (double)this.Slices);
			float num7 = this.Sections.Last<MeshGenerator.CircularSection>().SectionY - this.Sections[0].SectionY;
			if (num7 == 0f)
			{
				num7 = 1f;
			}
			int num8 = 0;
			for (int i = 0; i < this.Sections.Length; i++)
			{
				int num9 = num8 * num2;
				float sectionY = this.Sections[i].SectionY;
				float y = (sectionY - this.Sections[0].SectionY) / num7;
				for (int j = 0; j < num2; j++)
				{
					int i2 = num9 + j;
					float num10 = (float)j * num6;
					double num11 = Math.Cos((double)num10);
					double num12 = Math.Sin((double)num10);
					this.vertices[i2] = new Vector3d((double)this.Sections[i].Radius * num11, (double)sectionY, (double)this.Sections[i].Radius * num12);
					float x = (float)j / (float)(this.Slices - 1);
					this.uv[i2] = new Vector2f(x, y);
					Vector3f value = new Vector3f((float)num11, 0f, (float)num12);
					value.Normalize(1.1920929E-07f);
					this.normals[i2] = value;
				}
				num8++;
				if (this.NoSharedVertices && i != 0 && i != this.Sections.Length - 1)
				{
					base.duplicate_vertex_span(num9, num2);
					num8++;
				}
			}
			int num13 = 0;
			num8 = 0;
			for (int k = 0; k < this.Sections.Length - 1; k++)
			{
				int num14 = num8 * num2;
				int num15 = num14 + num2;
				num8 += (this.NoSharedVertices ? 2 : 1);
				for (int l = 0; l < num2 - 1; l++)
				{
					this.triangles.Set(num13++, num14 + l, num14 + l + 1, num15 + l + 1, this.Clockwise);
					this.triangles.Set(num13++, num14 + l, num15 + l + 1, num15 + l, this.Clockwise);
				}
				if (!this.NoSharedVertices)
				{
					this.triangles.Set(num13++, num15 - 1, num14, num15, this.Clockwise);
					this.triangles.Set(num13++, num15 - 1, num15, num15 + num2 - 1, this.Clockwise);
				}
			}
			if (this.Capped)
			{
				MeshGenerator.CircularSection circularSection = this.Sections[0];
				MeshGenerator.CircularSection circularSection2 = this.Sections.Last<MeshGenerator.CircularSection>();
				int num16 = num * num2;
				this.vertices[num16] = new Vector3d(0.0, (double)circularSection.SectionY, 0.0);
				this.uv[num16] = new Vector2f(0.5f, 0.5f);
				this.normals[num16] = new Vector3f(0f, -1f, 0f);
				this.startCapCenterIndex = num16;
				int num17 = num16 + 1;
				this.vertices[num17] = new Vector3d(0.0, (double)circularSection2.SectionY, 0.0);
				this.uv[num17] = new Vector2f(0.5f, 0.5f);
				this.normals[num17] = new Vector3f(0f, 1f, 0f);
				this.endCapCenterIndex = num17;
				if (this.NoSharedVertices)
				{
					int num18 = num17 + 1;
					for (int m = 0; m < this.Slices; m++)
					{
						float num19 = (float)m * num6;
						double num20 = Math.Cos((double)num19);
						double num21 = Math.Sin((double)num19);
						this.vertices[num18 + m] = new Vector3d((double)circularSection.Radius * num20, (double)circularSection.SectionY, (double)circularSection.Radius * num21);
						this.uv[num18 + m] = new Vector2f(0.5 * (1.0 + num20), 0.5 * (1.0 + num21));
						this.normals[num18 + m] = -Vector3f.AxisY;
					}
					base.append_disc(this.Slices, num16, num18, true, this.Clockwise, ref num13, -1);
					int num22 = num18 + this.Slices;
					for (int n = 0; n < this.Slices; n++)
					{
						float num23 = (float)n * num6;
						double num24 = Math.Cos((double)num23);
						double num25 = Math.Sin((double)num23);
						this.vertices[num22 + n] = new Vector3d((double)circularSection2.Radius * num24, (double)circularSection2.SectionY, (double)circularSection2.Radius * num25);
						this.uv[num22 + n] = new Vector2f(0.5 * (1.0 + num24), 0.5 * (1.0 + num25));
						this.normals[num22 + n] = Vector3f.AxisY;
					}
					base.append_disc(this.Slices, num17, num22, true, !this.Clockwise, ref num13, -1);
				}
				else
				{
					base.append_disc(this.Slices, num16, 0, true, this.Clockwise, ref num13, -1);
					base.append_disc(this.Slices, num17, num2 * (this.Sections.Length - 1), true, !this.Clockwise, ref num13, -1);
				}
			}
			return this;
		}

		public MeshGenerator.CircularSection[] Sections;

		public int Slices = 16;

		public bool Capped = true;

		public bool NoSharedVertices = true;

		public int startCapCenterIndex = -1;

		public int endCapCenterIndex = -1;
	}
}
