using System;

namespace g3
{
	public class TrivialBox3Generator : MeshGenerator
	{
		public override MeshGenerator Generate()
		{
			this.vertices = new VectorArray3d(this.NoSharedVertices ? 24 : 8, false);
			this.uv = new VectorArray2f(this.vertices.Count);
			this.normals = new VectorArray3f(this.vertices.Count);
			this.triangles = new IndexArray3i(12);
			if (!this.NoSharedVertices)
			{
				for (int i = 0; i < 8; i++)
				{
					this.vertices[i] = this.Box.Corner(i);
					this.normals[i] = (Vector3f)(this.vertices[i] - this.Box.Center[i]).Normalized;
					this.uv[i] = Vector2f.Zero;
				}
				int num = 0;
				for (int j = 0; j < 6; j++)
				{
					this.triangles.Set(num++, gIndices.BoxFaces[j, 0], gIndices.BoxFaces[j, 1], gIndices.BoxFaces[j, 2], this.Clockwise);
					this.triangles.Set(num++, gIndices.BoxFaces[j, 0], gIndices.BoxFaces[j, 2], gIndices.BoxFaces[j, 3], this.Clockwise);
				}
			}
			else
			{
				int num2 = 0;
				int num3 = 0;
				Vector2f[] array = new Vector2f[]
				{
					Vector2f.Zero,
					new Vector2f(1f, 0f),
					new Vector2f(1f, 1f),
					new Vector2f(0f, 1f)
				};
				for (int k = 0; k < 6; k++)
				{
					int num4 = num3++;
					num3 += 3;
					int value = gIndices.BoxFaceNormals[k];
					Vector3f value2 = (Vector3f)((double)Math.Sign(value) * this.Box.Axis(Math.Abs(value) - 1));
					for (int l = 0; l < 4; l++)
					{
						this.vertices[num4 + l] = this.Box.Corner(gIndices.BoxFaces[k, l]);
						this.normals[num4 + l] = value2;
						this.uv[num4 + l] = array[l];
					}
					this.triangles.Set(num2++, num4, num4 + 1, num4 + 2, this.Clockwise);
					this.triangles.Set(num2++, num4, num4 + 2, num4 + 3, this.Clockwise);
				}
			}
			return this;
		}

		public Box3d Box = Box3d.UnitZeroCentered;

		public bool NoSharedVertices;
	}
}
