using System;
using System.Collections.Generic;
using System.IO;

namespace g3
{
	public class OBJWriter : IMeshWriter
	{
		public IOWriteResult Write(BinaryWriter writer, List<WriteMesh> vMeshes, WriteOptions options)
		{
			throw new NotImplementedException();
		}

		public IOWriteResult Write(TextWriter writer, List<WriteMesh> vMeshes, WriteOptions options)
		{
			if (options.groupNamePrefix != null)
			{
				this.GroupNamePrefix = options.groupNamePrefix;
			}
			if (options.GroupNameF != null)
			{
				this.GroupNameF = options.GroupNameF;
			}
			int num = 1;
			int num2 = 1;
			string text = "";
			int num3 = 0;
			if (options.bWriteMaterials && options.MaterialFilePath.Length > 0)
			{
				List<GenericMaterial> vMaterials = MeshIOUtil.FindUniqueMaterialList(vMeshes);
				if (this.write_materials(vMaterials, options).code == IOCode.Ok)
				{
					text = Path.GetFileName(options.MaterialFilePath);
					num3 = vMeshes.Count;
				}
			}
			if (options.AsciiHeaderFunc != null)
			{
				writer.WriteLine(options.AsciiHeaderFunc());
			}
			if (text != "")
			{
				writer.WriteLine("mtllib {0}", text);
			}
			for (int i = 0; i < vMeshes.Count; i++)
			{
				IMesh mesh = vMeshes[i].Mesh;
				if (options.ProgressFunc != null)
				{
					options.ProgressFunc(i, vMeshes.Count);
				}
				bool flag = options.bPerVertexColors && mesh.HasVertexColors;
				bool flag2 = options.bPerVertexNormals && mesh.HasVertexNormals;
				bool flag3 = options.bPerVertexUVs && mesh.HasVertexUVs;
				if (vMeshes[i].UVs != null)
				{
					flag3 = false;
				}
				int[] array = new int[mesh.MaxVertexID];
				foreach (int num4 in mesh.VertexIndices())
				{
					array[num4] = num++;
					Vector3d vertex = mesh.GetVertex(num4);
					if (flag)
					{
						Vector3d vector3d = mesh.GetVertexColor(num4);
						writer.WriteLine("v {0} {1} {2} {3:F8} {4:F8} {5:F8}", new object[]
						{
							vertex[0],
							vertex[1],
							vertex[2],
							vector3d[0],
							vector3d[1],
							vector3d[2]
						});
					}
					else
					{
						writer.WriteLine("v {0} {1} {2}", vertex[0], vertex[1], vertex[2]);
					}
					if (flag2)
					{
						Vector3d vector3d2 = mesh.GetVertexNormal(num4);
						writer.WriteLine("vn {0:F10} {1:F10} {2:F10}", vector3d2[0], vector3d2[1], vector3d2[2]);
					}
					if (flag3)
					{
						Vector2f vertexUV = mesh.GetVertexUV(num4);
						writer.WriteLine("vt {0:F10} {1:F10}", vertexUV.x, vertexUV.y);
					}
				}
				IIndexMap mapUV = flag3 ? new IdentityIndexMap() : null;
				DenseUVMesh denseUVMesh = null;
				if (vMeshes[i].UVs != null)
				{
					denseUVMesh = vMeshes[i].UVs;
					int length = denseUVMesh.UVs.Length;
					IndexMap indexMap = new IndexMap(false, length);
					for (int j = 0; j < length; j++)
					{
						writer.WriteLine("vt {0:F8} {1:F8}", denseUVMesh.UVs[j].x, denseUVMesh.UVs[j].y);
						indexMap[j] = num2++;
					}
					mapUV = indexMap;
				}
				bool bMaterials = num3 > 0 && vMeshes[i].TriToMaterialMap != null && vMeshes[i].Materials != null;
				if (options.bWriteGroups && mesh.HasTriangleGroups)
				{
					this.write_triangles_bygroup(writer, mesh, array, denseUVMesh, mapUV, flag2);
				}
				else
				{
					this.write_triangles_flat(writer, vMeshes[i], array, denseUVMesh, mapUV, flag2, bMaterials);
				}
				if (options.ProgressFunc != null)
				{
					options.ProgressFunc(i + 1, vMeshes.Count);
				}
			}
			return new IOWriteResult(IOCode.Ok, "");
		}

		private void write_triangles_bygroup(TextWriter writer, IMesh mesh, int[] mapV, DenseUVMesh uvSet, IIndexMap mapUV, bool bNormals)
		{
			bool flag = mapUV != null;
			HashSet<int> hashSet = new HashSet<int>();
			foreach (int i in mesh.TriangleIndices())
			{
				hashSet.Add(mesh.GetTriangleGroup(i));
			}
			List<int> list = new List<int>(hashSet);
			list.Sort();
			foreach (int num in list)
			{
				string str = this.GroupNamePrefix;
				if (this.GroupNameF != null)
				{
					str = this.GroupNameF(num);
				}
				else
				{
					str = string.Format("{0}{1}", this.GroupNamePrefix, num);
				}
				writer.WriteLine("g " + str);
				foreach (int i2 in mesh.TriangleIndices())
				{
					if (mesh.GetTriangleGroup(i2) == num)
					{
						Index3i triangle = mesh.GetTriangle(i2);
						triangle[0] = mapV[triangle[0]];
						triangle[1] = mapV[triangle[1]];
						triangle[2] = mapV[triangle[2]];
						if (flag)
						{
							Index3i index3i = (uvSet != null) ? uvSet.TriangleUVs[i2] : triangle;
							index3i[0] = mapUV[index3i[0]];
							index3i[1] = mapUV[index3i[1]];
							index3i[2] = mapUV[index3i[2]];
							this.write_tri(writer, ref triangle, bNormals, true, ref index3i);
						}
						else
						{
							this.write_tri(writer, ref triangle, bNormals, false, ref triangle);
						}
					}
				}
			}
		}

		private void write_triangles_flat(TextWriter writer, WriteMesh write_mesh, int[] mapV, DenseUVMesh uvSet, IIndexMap mapUV, bool bNormals, bool bMaterials)
		{
			bool flag = mapUV != null;
			int num = -1;
			IMesh mesh = write_mesh.Mesh;
			foreach (int num2 in mesh.TriangleIndices())
			{
				if (bMaterials)
				{
					this.set_current_material(writer, num2, write_mesh, ref num);
				}
				Index3i triangle = mesh.GetTriangle(num2);
				triangle[0] = mapV[triangle[0]];
				triangle[1] = mapV[triangle[1]];
				triangle[2] = mapV[triangle[2]];
				if (flag)
				{
					Index3i index3i = (uvSet != null) ? uvSet.TriangleUVs[num2] : triangle;
					index3i[0] = mapUV[index3i[0]];
					index3i[1] = mapUV[index3i[1]];
					index3i[2] = mapUV[index3i[2]];
					this.write_tri(writer, ref triangle, bNormals, true, ref index3i);
				}
				else
				{
					this.write_tri(writer, ref triangle, bNormals, false, ref triangle);
				}
			}
		}

		public void set_current_material(TextWriter writer, int ti, WriteMesh mesh, ref int cur_material)
		{
			int num = mesh.TriToMaterialMap[ti];
			if (num != cur_material && num >= 0 && num < mesh.Materials.Count)
			{
				writer.WriteLine("usemtl " + mesh.Materials[num].name);
				cur_material = num;
			}
		}

		private void write_tri(TextWriter writer, ref Index3i t, bool bNormals, bool bUVs, ref Index3i tuv)
		{
			if (!bNormals && !bUVs)
			{
				writer.WriteLine("f {0} {1} {2}", t[0], t[1], t[2]);
				return;
			}
			if (bNormals && !bUVs)
			{
				writer.WriteLine("f {0}//{0} {1}//{1} {2}//{2}", t[0], t[1], t[2]);
				return;
			}
			if (!bNormals && bUVs)
			{
				writer.WriteLine("f {0}/{3} {1}/{4} {2}/{5}", new object[]
				{
					t[0],
					t[1],
					t[2],
					tuv[0],
					tuv[1],
					tuv[2]
				});
				return;
			}
			writer.WriteLine("f {0}/{3}/{0} {1}/{4}/{1} {2}/{5}/{2}", new object[]
			{
				t[0],
				t[1],
				t[2],
				tuv[0],
				tuv[1],
				tuv[2]
			});
		}

		private IOWriteResult write_materials(List<GenericMaterial> vMaterials, WriteOptions options)
		{
			Stream stream = this.OpenStreamF(options.MaterialFilePath);
			if (stream == null)
			{
				return new IOWriteResult(IOCode.FileAccessError, "Could not open file " + options.MaterialFilePath + " for writing");
			}
			try
			{
				StreamWriter streamWriter = new StreamWriter(stream);
				foreach (GenericMaterial genericMaterial in vMaterials)
				{
					if (genericMaterial is OBJMaterial)
					{
						OBJMaterial objmaterial = genericMaterial as OBJMaterial;
						streamWriter.WriteLine("newmtl {0}", objmaterial.name);
						if (objmaterial.Ka != GenericMaterial.Invalid)
						{
							streamWriter.WriteLine("Ka {0} {1} {2}", objmaterial.Ka.x, objmaterial.Ka.y, objmaterial.Ka.z);
						}
						if (objmaterial.Kd != GenericMaterial.Invalid)
						{
							streamWriter.WriteLine("Kd {0} {1} {2}", objmaterial.Kd.x, objmaterial.Kd.y, objmaterial.Kd.z);
						}
						if (objmaterial.Ks != GenericMaterial.Invalid)
						{
							streamWriter.WriteLine("Ks {0} {1} {2}", objmaterial.Ks.x, objmaterial.Ks.y, objmaterial.Ks.z);
						}
						if (objmaterial.Ke != GenericMaterial.Invalid)
						{
							streamWriter.WriteLine("Ke {0} {1} {2}", objmaterial.Ke.x, objmaterial.Ke.y, objmaterial.Ke.z);
						}
						if (objmaterial.Tf != GenericMaterial.Invalid)
						{
							streamWriter.WriteLine("Tf {0} {1} {2}", objmaterial.Tf.x, objmaterial.Tf.y, objmaterial.Tf.z);
						}
						if (objmaterial.d != -3.4028235E+38f)
						{
							streamWriter.WriteLine("d {0}", objmaterial.d);
						}
						if (objmaterial.Ns != -3.4028235E+38f)
						{
							streamWriter.WriteLine("Ns {0}", objmaterial.Ns);
						}
						if (objmaterial.Ni != -3.4028235E+38f)
						{
							streamWriter.WriteLine("Ni {0}", objmaterial.Ni);
						}
						if (objmaterial.sharpness != -3.4028235E+38f)
						{
							streamWriter.WriteLine("sharpness {0}", objmaterial.sharpness);
						}
						if (objmaterial.illum != -1)
						{
							streamWriter.WriteLine("illum {0}", objmaterial.illum);
						}
						if (objmaterial.map_Ka != null && objmaterial.map_Ka != "")
						{
							streamWriter.WriteLine("map_Ka {0}", objmaterial.map_Ka);
						}
						if (objmaterial.map_Kd != null && objmaterial.map_Kd != "")
						{
							streamWriter.WriteLine("map_Kd {0}", objmaterial.map_Kd);
						}
						if (objmaterial.map_Ks != null && objmaterial.map_Ks != "")
						{
							streamWriter.WriteLine("map_Ks {0}", objmaterial.map_Ks);
						}
						if (objmaterial.map_Ke != null && objmaterial.map_Ke != "")
						{
							streamWriter.WriteLine("map_Ke {0}", objmaterial.map_Ke);
						}
						if (objmaterial.map_d != null && objmaterial.map_d != "")
						{
							streamWriter.WriteLine("map_d {0}", objmaterial.map_d);
						}
						if (objmaterial.map_Ns != null && objmaterial.map_Ns != "")
						{
							streamWriter.WriteLine("map_Ns {0}", objmaterial.map_Ns);
						}
						if (objmaterial.bump != null && objmaterial.bump != "")
						{
							streamWriter.WriteLine("bump {0}", objmaterial.bump);
						}
						if (objmaterial.disp != null && objmaterial.disp != "")
						{
							streamWriter.WriteLine("disp {0}", objmaterial.disp);
						}
						if (objmaterial.decal != null && objmaterial.decal != "")
						{
							streamWriter.WriteLine("decal {0}", objmaterial.decal);
						}
						if (objmaterial.refl != null && objmaterial.refl != "")
						{
							streamWriter.WriteLine("refl {0}", objmaterial.refl);
						}
					}
				}
			}
			finally
			{
				this.CloseStreamF(stream);
			}
			return IOWriteResult.Ok;
		}

		public Func<string, Stream> OpenStreamF = (string sFilename) => File.Open(sFilename, FileMode.Create);

		public Action<Stream> CloseStreamF = delegate(Stream stream)
		{
			stream.Dispose();
		};

		public string GroupNamePrefix = "mmGroup";

		public Func<int, string> GroupNameF;
	}
}
