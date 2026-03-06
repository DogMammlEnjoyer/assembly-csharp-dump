using System;

namespace UnityEngine.ProBuilder.Shapes
{
	[Shape("Plane")]
	public class Plane : Shape
	{
		internal override void SetParametersToBuiltInShape()
		{
			this.m_HeightSegments = 5;
			this.m_WidthSegments = 5;
		}

		public override void CopyShape(Shape shape)
		{
			if (shape is Plane)
			{
				this.m_HeightSegments = ((Plane)shape).m_HeightSegments;
				this.m_WidthSegments = ((Plane)shape).m_WidthSegments;
			}
		}

		public override Bounds RebuildMesh(ProBuilderMesh mesh, Vector3 size, Quaternion rotation)
		{
			int num = this.m_WidthSegments + 1;
			int num2 = this.m_HeightSegments + 1;
			Vector2[] array = new Vector2[num * num2 * 4];
			Vector3[] array2 = new Vector3[num * num2 * 4];
			float num3 = 1f;
			float num4 = 1f;
			int i = 0;
			for (int j = 0; j < num2; j++)
			{
				for (int k = 0; k < num; k++)
				{
					float x = (float)k * (num3 / (float)num) - num3 / 2f;
					float x2 = (float)(k + 1) * (num3 / (float)num) - num3 / 2f;
					float y = (float)j * (num4 / (float)num2) - num4 / 2f;
					float y2 = (float)(j + 1) * (num4 / (float)num2) - num4 / 2f;
					array[i] = new Vector2(x, y);
					array[i + 1] = new Vector2(x2, y);
					array[i + 2] = new Vector2(x, y2);
					array[i + 3] = new Vector2(x2, y2);
					i += 4;
				}
			}
			for (i = 0; i < array2.Length; i++)
			{
				array2[i] = new Vector3(array[i].y, 0f, array[i].x);
			}
			mesh.GeometryWithPoints(array2);
			return mesh.mesh.bounds;
		}

		[Min(0f)]
		[SerializeField]
		private int m_HeightSegments = 1;

		[Min(0f)]
		[SerializeField]
		private int m_WidthSegments = 1;
	}
}
