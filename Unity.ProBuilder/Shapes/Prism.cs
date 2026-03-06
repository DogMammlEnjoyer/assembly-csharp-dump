using System;

namespace UnityEngine.ProBuilder.Shapes
{
	[Shape("Prism")]
	public class Prism : Shape
	{
		internal override void SetParametersToBuiltInShape()
		{
		}

		public override void CopyShape(Shape shape)
		{
		}

		public override Bounds RebuildMesh(ProBuilderMesh mesh, Vector3 size, Quaternion rotation)
		{
			Vector3 vector = size.Abs();
			vector.y = ((vector.y == 0f) ? (2f * Mathf.Epsilon) : vector.y);
			Vector3 b = new Vector3(0f, vector.y / 2f, 0f);
			vector.y *= 2f;
			Vector3[] array = new Vector3[]
			{
				Vector3.Scale(new Vector3(-0.5f, 0f, -0.5f), vector) - b,
				Vector3.Scale(new Vector3(0.5f, 0f, -0.5f), vector) - b,
				Vector3.Scale(new Vector3(0f, 0.5f, -0.5f), vector) - b,
				Vector3.Scale(new Vector3(-0.5f, 0f, 0.5f), vector) - b,
				Vector3.Scale(new Vector3(0.5f, 0f, 0.5f), vector) - b,
				Vector3.Scale(new Vector3(0f, 0.5f, 0.5f), vector) - b
			};
			Vector3[] array2 = new Vector3[]
			{
				array[0],
				array[1],
				array[2],
				array[1],
				array[4],
				array[2],
				array[5],
				array[4],
				array[3],
				array[5],
				array[3],
				array[0],
				array[5],
				array[2],
				array[0],
				array[1],
				array[3],
				array[4]
			};
			Face[] array3 = new Face[5];
			int num = 0;
			int[] array4 = new int[3];
			array4[0] = 2;
			array4[1] = 1;
			array3[num] = new Face(array4);
			array3[1] = new Face(new int[]
			{
				5,
				4,
				3,
				5,
				6,
				4
			});
			array3[2] = new Face(new int[]
			{
				9,
				8,
				7
			});
			array3[3] = new Face(new int[]
			{
				12,
				11,
				10,
				12,
				13,
				11
			});
			array3[4] = new Face(new int[]
			{
				14,
				15,
				16,
				15,
				17,
				16
			});
			Face[] array5 = array3;
			Vector3 b2 = size.Sign();
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = Vector3.Scale(rotation * array2[i], b2);
			}
			if (Mathf.Sign(size.x) * Mathf.Sign(size.y) * Mathf.Sign(size.z) < 0f)
			{
				Face[] array6 = array5;
				for (int j = 0; j < array6.Length; j++)
				{
					array6[j].Reverse();
				}
			}
			mesh.RebuildWithPositionsAndFaces(array2, array5);
			return mesh.mesh.bounds;
		}
	}
}
