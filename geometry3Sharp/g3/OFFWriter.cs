using System;
using System.Collections.Generic;
using System.IO;

namespace g3
{
	public class OFFWriter : IMeshWriter
	{
		public IOWriteResult Write(BinaryWriter writer, List<WriteMesh> vMeshes, WriteOptions options)
		{
			return new IOWriteResult(IOCode.FormatNotSupportedError, "binary write not supported for OFF format");
		}

		public IOWriteResult Write(TextWriter writer, List<WriteMesh> vMeshes, WriteOptions options)
		{
			int count = vMeshes.Count;
			writer.WriteLine("OFF");
			string format = Util.MakeVec3FormatString(0, 1, 2, options.RealPrecisionDigits);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int[][] array = new int[count][];
			for (int i = 0; i < count; i++)
			{
				num += vMeshes[i].Mesh.VertexCount;
				num2 += vMeshes[i].Mesh.TriangleCount;
				num3 = num3;
				array[i] = new int[vMeshes[i].Mesh.MaxVertexID];
			}
			writer.WriteLine(string.Format("{0} {1} {2}", num, num2, num3));
			int num4 = 0;
			for (int j = 0; j < count; j++)
			{
				IMesh mesh = vMeshes[j].Mesh;
				if (options.ProgressFunc != null)
				{
					options.ProgressFunc(j, 2 * (count - 1));
				}
				foreach (int num5 in mesh.VertexIndices())
				{
					Vector3d vertex = mesh.GetVertex(num5);
					writer.WriteLine(format, vertex.x, vertex.y, vertex.z);
					array[j][num5] = num4;
					num4++;
				}
			}
			for (int k = 0; k < count; k++)
			{
				IMesh mesh2 = vMeshes[k].Mesh;
				if (options.ProgressFunc != null)
				{
					options.ProgressFunc(count + k, 2 * (count - 1));
				}
				foreach (int i2 in mesh2.TriangleIndices())
				{
					Index3i triangle = mesh2.GetTriangle(i2);
					triangle[0] = array[k][triangle[0]];
					triangle[1] = array[k][triangle[1]];
					triangle[2] = array[k][triangle[2]];
					writer.WriteLine(string.Format("3 {0} {1} {2}", triangle[0], triangle[1], triangle[2]));
				}
			}
			return new IOWriteResult(IOCode.Ok, "");
		}
	}
}
