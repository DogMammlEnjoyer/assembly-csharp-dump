using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UnityEngine.ProBuilder.Stl
{
	internal static class pb_Stl_Importer
	{
		public static Mesh[] Import(string path)
		{
			if (pb_Stl_Importer.IsBinary(path))
			{
				try
				{
					return pb_Stl_Importer.ImportBinary(path);
				}
				catch (Exception ex)
				{
					Debug.LogWarning(string.Format("Failed importing mesh at path {0}.\n{1}", path, ex.ToString()));
					return null;
				}
			}
			return pb_Stl_Importer.ImportAscii(path);
		}

		private static Mesh[] ImportBinary(string path)
		{
			pb_Stl_Importer.Facet[] array;
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream, new ASCIIEncoding()))
				{
					binaryReader.ReadBytes(80);
					uint num = binaryReader.ReadUInt32();
					array = new pb_Stl_Importer.Facet[num];
					for (uint num2 = 0U; num2 < num; num2 += 1U)
					{
						array[(int)num2] = binaryReader.GetFacet();
					}
				}
			}
			return pb_Stl_Importer.CreateMeshWithFacets(array);
		}

		private static pb_Stl_Importer.Facet GetFacet(this BinaryReader binaryReader)
		{
			pb_Stl_Importer.Facet facet = new pb_Stl_Importer.Facet();
			facet.normal = binaryReader.GetVector3();
			facet.a = binaryReader.GetVector3();
			facet.c = binaryReader.GetVector3();
			facet.b = binaryReader.GetVector3();
			binaryReader.ReadUInt16();
			return facet;
		}

		private static Vector3 GetVector3(this BinaryReader binaryReader)
		{
			Vector3 vector = default(Vector3);
			for (int i = 0; i < 3; i++)
			{
				vector[i] = binaryReader.ReadSingle();
			}
			return vector.UnityCoordTrafo();
		}

		private static Vector3 UnityCoordTrafo(this Vector3 vector3)
		{
			return new Vector3(-vector3.y, vector3.z, vector3.x);
		}

		private static int ReadState(string line)
		{
			if (line.StartsWith("solid"))
			{
				return 1;
			}
			if (line.StartsWith("facet"))
			{
				return 2;
			}
			if (line.StartsWith("outer"))
			{
				return 3;
			}
			if (line.StartsWith("vertex"))
			{
				return 4;
			}
			if (line.StartsWith("endloop"))
			{
				return 5;
			}
			if (line.StartsWith("endfacet"))
			{
				return 6;
			}
			if (line.StartsWith("endsolid"))
			{
				return 7;
			}
			return 0;
		}

		private static Mesh[] ImportAscii(string path)
		{
			List<pb_Stl_Importer.Facet> list = new List<pb_Stl_Importer.Facet>();
			using (StreamReader streamReader = new StreamReader(path))
			{
				int num = 0;
				pb_Stl_Importer.Facet facet = null;
				bool flag = false;
				while (streamReader.Peek() > 0 && !flag)
				{
					string text = streamReader.ReadLine().Trim();
					switch (pb_Stl_Importer.ReadState(text))
					{
					case 2:
						facet = new pb_Stl_Importer.Facet();
						facet.normal = pb_Stl_Importer.StringToVec3(text.Replace("facet normal ", ""));
						break;
					case 3:
						num = 0;
						break;
					case 4:
						if (num == 0)
						{
							facet.a = pb_Stl_Importer.StringToVec3(text.Replace("vertex ", ""));
						}
						else if (num == 2)
						{
							facet.c = pb_Stl_Importer.StringToVec3(text.Replace("vertex ", ""));
						}
						else if (num == 1)
						{
							facet.b = pb_Stl_Importer.StringToVec3(text.Replace("vertex ", ""));
						}
						num++;
						break;
					case 6:
						list.Add(facet);
						break;
					case 7:
						flag = true;
						break;
					}
				}
			}
			return pb_Stl_Importer.CreateMeshWithFacets(list);
		}

		private static Vector3 StringToVec3(string str)
		{
			string[] array = str.Trim().Split(null);
			Vector3 vector = default(Vector3);
			float.TryParse(array[0], out vector.x);
			float.TryParse(array[1], out vector.y);
			float.TryParse(array[2], out vector.z);
			return vector.UnityCoordTrafo();
		}

		private static bool IsBinary(string path)
		{
			FileInfo fileInfo = new FileInfo(path);
			if (fileInfo.Length < 130L)
			{
				return false;
			}
			bool flag = false;
			using (FileStream fileStream = fileInfo.OpenRead())
			{
				using (BufferedStream bufferedStream = new BufferedStream(fileStream))
				{
					for (long num = 0L; num < 80L; num += 1L)
					{
						if (bufferedStream.ReadByte() == 0)
						{
							flag = true;
							break;
						}
					}
				}
			}
			if (!flag)
			{
				using (FileStream fileStream2 = fileInfo.OpenRead())
				{
					using (BufferedStream bufferedStream2 = new BufferedStream(fileStream2))
					{
						byte[] array = new byte[6];
						for (int i = 0; i < 6; i++)
						{
							array[i] = (byte)bufferedStream2.ReadByte();
						}
						flag = (Encoding.UTF8.GetString(array) != "solid ");
					}
				}
			}
			return flag;
		}

		private static Mesh[] CreateMeshWithFacets(IList<pb_Stl_Importer.Facet> facets)
		{
			int count = facets.Count;
			int num = 0;
			int val = 65535;
			Mesh[] array = new Mesh[count / 21845 + 1];
			for (int i = 0; i < array.Length; i++)
			{
				int num2 = Math.Min(val, (count - num) * 3);
				Vector3[] array2 = new Vector3[num2];
				Vector3[] array3 = new Vector3[num2];
				int[] array4 = new int[num2];
				for (int j = 0; j < num2; j += 3)
				{
					array2[j] = facets[num].a;
					array2[j + 1] = facets[num].b;
					array2[j + 2] = facets[num].c;
					array3[j] = facets[num].normal;
					array3[j + 1] = facets[num].normal;
					array3[j + 2] = facets[num].normal;
					array4[j] = j;
					array4[j + 1] = j + 1;
					array4[j + 2] = j + 2;
					num++;
				}
				array[i] = new Mesh();
				array[i].vertices = array2;
				array[i].normals = array3;
				array[i].triangles = array4;
			}
			return array;
		}

		private const int MAX_FACETS_PER_MESH = 21845;

		private const int SOLID = 1;

		private const int FACET = 2;

		private const int OUTER = 3;

		private const int VERTEX = 4;

		private const int ENDLOOP = 5;

		private const int ENDFACET = 6;

		private const int ENDSOLID = 7;

		private const int EMPTY = 0;

		private class Facet
		{
			public override string ToString()
			{
				return string.Format("{0:F2}: {1:F2}, {2:F2}, {3:F2}", new object[]
				{
					this.normal,
					this.a,
					this.b,
					this.c
				});
			}

			public Vector3 normal;

			public Vector3 a;

			public Vector3 b;

			public Vector3 c;
		}
	}
}
