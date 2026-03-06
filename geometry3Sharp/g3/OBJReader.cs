using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace g3
{
	public class OBJReader : IMeshReader
	{
		public OBJReader()
		{
			this.splitDoubleSlash = new string[]
			{
				"//"
			};
			this.splitSlash = new char[]
			{
				'/'
			};
			this.MTLFileSearchPaths = new List<string>();
		}

		public List<string> MTLFileSearchPaths { get; set; }

		public event ParsingMessagesHandler warningEvent;

		public bool HasPerVertexColors
		{
			get
			{
				return this.m_bOBJHasPerVertexColors;
			}
		}

		public int UVDimension
		{
			get
			{
				return this.m_nUVComponents;
			}
		}

		public bool HasTriangleGroups
		{
			get
			{
				return this.m_bOBJHasTriangleGroups;
			}
		}

		public bool HasComplexVertices { get; set; }

		public IOReadResult Read(BinaryReader reader, ReadOptions options, IMeshBuilder builder)
		{
			throw new NotImplementedException();
		}

		public IOReadResult Read(TextReader reader, ReadOptions options, IMeshBuilder builder)
		{
			this.Materials = new Dictionary<string, OBJMaterial>();
			this.UsedMaterials = new Dictionary<int, string>();
			this.HasComplexVertices = false;
			if (this.nWarningLevel >= 1)
			{
				this.emit_warning("[OBJReader] starting parse");
			}
			IOReadResult result = this.ParseInput(reader, options);
			if (result.code != IOCode.Ok)
			{
				return result;
			}
			if (this.nWarningLevel >= 1)
			{
				this.emit_warning("[OBJReader] completed parse. building.");
			}
			IOReadResult result2 = (this.UsedMaterials.Count > 1 || this.HasComplexVertices) ? this.BuildMeshes_ByMaterial(options, builder) : this.BuildMeshes_Simple(options, builder);
			if (this.nWarningLevel >= 1)
			{
				this.emit_warning("[OBJReader] build complete.");
			}
			if (result2.code != IOCode.Ok)
			{
				return result2;
			}
			return new IOReadResult(IOCode.Ok, "");
		}

		private int append_vertex(IMeshBuilder builder, Index3i vertIdx, bool bHaveNormals, bool bHaveColors, bool bHaveUVs)
		{
			int num = 3 * vertIdx.a;
			if (vertIdx.a < 0 || vertIdx.a >= this.vPositions.Length / 3)
			{
				this.emit_warning("[OBJReader] append_vertex() referencing invalid vertex " + vertIdx.a.ToString());
				return -1;
			}
			if (!bHaveNormals && !bHaveColors && !bHaveUVs)
			{
				return builder.AppendVertex(this.vPositions[num], this.vPositions[num + 1], this.vPositions[num + 2]);
			}
			NewVertexInfo info = default(NewVertexInfo);
			info.bHaveC = (info.bHaveN = (info.bHaveUV = false));
			info.v = new Vector3d(this.vPositions[num], this.vPositions[num + 1], this.vPositions[num + 2]);
			if (bHaveNormals)
			{
				info.bHaveN = true;
				int num2 = 3 * vertIdx.b;
				info.n = new Vector3f(this.vNormals[num2], this.vNormals[num2 + 1], this.vNormals[num2 + 2]);
			}
			if (bHaveColors)
			{
				info.bHaveC = true;
				info.c = new Vector3f(this.vColors[num], this.vColors[num + 1], this.vColors[num + 2]);
			}
			if (bHaveUVs)
			{
				info.bHaveUV = true;
				int num3 = 2 * vertIdx.c;
				info.uv = new Vector2f(this.vUVs[num3], this.vUVs[num3 + 1]);
			}
			return builder.AppendVertex(info);
		}

		private int append_triangle(IMeshBuilder builder, int nTri, int[] mapV)
		{
			Triangle triangle = this.vTriangles[nTri];
			int num = mapV[triangle.vIndices[0] - 1];
			int num2 = mapV[triangle.vIndices[1] - 1];
			int num3 = mapV[triangle.vIndices[2] - 1];
			if (num == -1 || num2 == -1 || num3 == -1)
			{
				this.emit_warning(string.Format("[OBJReader] invalid triangle:  {0} {1} {2}  mapped to {3} {4} {5}", new object[]
				{
					triangle.vIndices[0],
					triangle.vIndices[1],
					triangle.vIndices[2],
					num,
					num2,
					num3
				}));
				return -1;
			}
			int g = (this.vTriangles[nTri].nGroupID == -1) ? this.m_nSetInvalidGroupsTo : this.vTriangles[nTri].nGroupID;
			return builder.AppendTriangle(num, num2, num3, g);
		}

		private int append_triangle(IMeshBuilder builder, Triangle t)
		{
			if (t.vIndices[0] < 0 || t.vIndices[1] < 0 || t.vIndices[2] < 0)
			{
				this.emit_warning(string.Format("[OBJReader] invalid triangle:  {0} {1} {2}", t.vIndices[0], t.vIndices[1], t.vIndices[2]));
				return -1;
			}
			int g = (t.nGroupID == -1) ? this.m_nSetInvalidGroupsTo : t.nGroupID;
			return builder.AppendTriangle(t.vIndices[0], t.vIndices[1], t.vIndices[2], g);
		}

		private IOReadResult BuildMeshes_Simple(ReadOptions options, IMeshBuilder builder)
		{
			if (this.vPositions.Length == 0)
			{
				return new IOReadResult(IOCode.GarbageDataError, "No vertices in file");
			}
			if (this.vTriangles.Length == 0)
			{
				return new IOReadResult(IOCode.GarbageDataError, "No triangles in file");
			}
			bool flag = this.vNormals.Length == this.vPositions.Length;
			bool flag2 = this.vColors.Length == this.vPositions.Length;
			bool flag3 = this.vUVs.Length / 2 == this.vPositions.Length / 3;
			int num = this.vPositions.Length / 3;
			int[] array = new int[num];
			int meshID = builder.AppendNewMesh(flag, flag2, flag3, this.m_bOBJHasTriangleGroups);
			for (int i = 0; i < num; i++)
			{
				Index3i vertIdx = new Index3i(i, i, i);
				array[i] = this.append_vertex(builder, vertIdx, flag, flag2, flag3);
			}
			for (int j = 0; j < this.vTriangles.Length; j++)
			{
				this.append_triangle(builder, j, array);
			}
			if (this.UsedMaterials.Count == 1)
			{
				int key = this.UsedMaterials.Keys.First<int>();
				string key2 = this.UsedMaterials[key];
				OBJMaterial m = this.Materials[key2];
				int materialID = builder.BuildMaterial(m);
				builder.AssignMaterial(materialID, meshID);
			}
			return new IOReadResult(IOCode.Ok, "");
		}

		private IOReadResult BuildMeshes_ByMaterial(ReadOptions options, IMeshBuilder builder)
		{
			if (this.vPositions.Length == 0)
			{
				return new IOReadResult(IOCode.GarbageDataError, "No vertices in file");
			}
			if (this.vTriangles.Length == 0)
			{
				return new IOReadResult(IOCode.GarbageDataError, "No triangles in file");
			}
			bool flag = this.vNormals.Length > 0;
			bool flag2 = this.vColors.Length > 0;
			bool flag3 = this.vUVs.Length > 0;
			foreach (int num in new List<int>(this.UsedMaterials.Keys)
			{
				-1
			})
			{
				int num2 = -1;
				if (num != -1)
				{
					string key = this.UsedMaterials[num];
					OBJMaterial m = this.Materials[key];
					num2 = builder.BuildMaterial(m);
				}
				bool flag4 = num != -1 && flag3;
				int num3 = -1;
				Dictionary<Index3i, int> dictionary = new Dictionary<Index3i, int>();
				for (int i = 0; i < this.vTriangles.Length; i++)
				{
					Triangle triangle = this.vTriangles[i];
					if (triangle.nMaterialID == num)
					{
						if (num3 == -1)
						{
							num3 = builder.AppendNewMesh(flag, flag2, flag4, false);
						}
						Triangle t = default(Triangle);
						for (int j = 0; j < 3; j++)
						{
							Index3i index3i = new Index3i(triangle.vIndices[j] - 1, triangle.vNormals[j] - 1, triangle.vUVs[j] - 1);
							int value;
							if (!dictionary.ContainsKey(index3i))
							{
								value = this.append_vertex(builder, index3i, flag, flag2, flag4);
								dictionary[index3i] = value;
							}
							else
							{
								value = dictionary[index3i];
							}
							t.vIndices[j] = value;
						}
						this.append_triangle(builder, t);
					}
				}
				if (num2 != -1)
				{
					builder.AssignMaterial(num2, num3);
				}
			}
			return new IOReadResult(IOCode.Ok, "");
		}

		public IOReadResult ParseInput(TextReader reader, ReadOptions options)
		{
			this.vPositions = new DVector<double>();
			this.vNormals = new DVector<float>();
			this.vUVs = new DVector<float>();
			this.vColors = new DVector<float>();
			this.vTriangles = new DVector<Triangle>();
			bool bOBJHasPerVertexColors = false;
			int num = 0;
			OBJMaterial objmaterial = null;
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			int num2 = 0;
			int num3 = -1;
			int num4 = 0;
			while (reader.Peek() >= 0)
			{
				string text = reader.ReadLine();
				num4++;
				string[] array = text.Split(null, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length != 0)
				{
					try
					{
						if (array[0][0] == 'v')
						{
							if (array[0].Length == 1)
							{
								if (array.Length == 7)
								{
									this.vPositions.Add(double.Parse(array[1]));
									this.vPositions.Add(double.Parse(array[2]));
									this.vPositions.Add(double.Parse(array[3]));
									this.vColors.Add(float.Parse(array[4]));
									this.vColors.Add(float.Parse(array[5]));
									this.vColors.Add(float.Parse(array[6]));
									bOBJHasPerVertexColors = true;
								}
								else if (array.Length >= 4)
								{
									this.vPositions.Add(double.Parse(array[1]));
									this.vPositions.Add(double.Parse(array[2]));
									this.vPositions.Add(double.Parse(array[3]));
								}
								if (array.Length != 4 && array.Length != 7)
								{
									this.emit_warning("[OBJReader] vertex has unknown format: " + text);
								}
							}
							else if (array[0][1] == 'n')
							{
								if (array.Length >= 4)
								{
									this.vNormals.Add(float.Parse(array[1]));
									this.vNormals.Add(float.Parse(array[2]));
									this.vNormals.Add(float.Parse(array[3]));
								}
								if (array.Length != 4)
								{
									this.emit_warning("[OBJReader] normal has more than 3 coordinates: " + text);
								}
							}
							else if (array[0][1] == 't')
							{
								if (array.Length >= 3)
								{
									this.vUVs.Add(float.Parse(array[1]));
									this.vUVs.Add(float.Parse(array[2]));
									num = Math.Max(num, array.Length);
								}
								if (array.Length != 3)
								{
									this.emit_warning("[OBJReader] UV has unknown format: " + text);
								}
							}
						}
						else if (array[0][0] == 'f')
						{
							if (array.Length < 4)
							{
								this.emit_warning("[OBJReader] degenerate face specified : " + text);
							}
							else if (array.Length == 4)
							{
								Triangle value = default(Triangle);
								this.parse_triangle(array, ref value);
								value.nGroupID = num3;
								if (objmaterial != null)
								{
									value.nMaterialID = objmaterial.id;
									this.UsedMaterials[objmaterial.id] = objmaterial.name;
								}
								this.vTriangles.Add(value);
								if (value.is_complex())
								{
									this.HasComplexVertices = true;
								}
							}
							else
							{
								this.append_face(array, objmaterial, num3);
							}
						}
						else if (array[0][0] == 'g')
						{
							string key = (array.Length == 2) ? array[1] : text.Substring(text.IndexOf(array[1]));
							if (dictionary.ContainsKey(key))
							{
								num3 = dictionary[key];
							}
							else
							{
								num3 = num2;
								dictionary[key] = num2++;
							}
						}
						else if (array[0][0] != 'o')
						{
							if (array[0] == "mtllib" && options.ReadMaterials)
							{
								if (this.MTLFileSearchPaths.Count == 0)
								{
									this.emit_warning("Materials requested but Material Search Paths not initialized!");
								}
								string text2 = (array.Length == 2) ? array[1] : text.Substring(text.IndexOf(array[1]));
								string text3 = this.FindMTLFile(text2);
								if (text3 != null)
								{
									IOReadResult ioreadResult = this.ReadMaterials(text3);
									if (ioreadResult.code != IOCode.Ok)
									{
										this.emit_warning("error parsing " + text3 + " : " + ioreadResult.message);
									}
								}
								else
								{
									this.emit_warning("material file " + text2 + " could not be found in material search paths");
								}
							}
							else if (array[0] == "usemtl" && options.ReadMaterials)
							{
								objmaterial = this.find_material(array[1]);
							}
						}
					}
					catch (Exception ex)
					{
						this.emit_warning(string.Concat(new string[]
						{
							"error parsing line ",
							num4.ToString(),
							": ",
							text,
							", exception ",
							ex.Message
						}));
					}
				}
			}
			this.m_bOBJHasPerVertexColors = bOBJHasPerVertexColors;
			this.m_bOBJHasTriangleGroups = (num3 != -1);
			this.m_nSetInvalidGroupsTo = num2++;
			this.m_nUVComponents = num;
			return new IOReadResult(IOCode.Ok, "");
		}

		private int parse_v(string sToken)
		{
			int num = int.Parse(sToken);
			if (num < 0)
			{
				num = this.vPositions.Length / 3 + num + 1;
			}
			return num;
		}

		private int parse_n(string sToken)
		{
			int num = int.Parse(sToken);
			if (num < 0)
			{
				num = this.vNormals.Length / 3 + num + 1;
			}
			return num;
		}

		private int parse_u(string sToken)
		{
			int num = int.Parse(sToken);
			if (num < 0)
			{
				num = this.vUVs.Length / 2 + num + 1;
			}
			return num;
		}

		private void append_face(string[] tokens, OBJMaterial activeMaterial, int nActiveGroup)
		{
			int num = 0;
			if (tokens[1].IndexOf("//") != -1)
			{
				num = 1;
			}
			else if (tokens[1].IndexOf('/') != -1)
			{
				num = 2;
			}
			Triangle value = default(Triangle);
			value.clear();
			for (int i = 0; i < tokens.Length - 1; i++)
			{
				int num2 = (i < 3) ? i : 2;
				if (i >= 3)
				{
					value.move_vertex(2, 1);
				}
				if (num == 0)
				{
					value.set_vertex(num2, this.parse_v(tokens[i + 1]), -1, -1);
				}
				else if (num == 1)
				{
					string[] array = tokens[i + 1].Split(this.splitDoubleSlash, StringSplitOptions.RemoveEmptyEntries);
					value.set_vertex(num2, this.parse_v(array[0]), this.parse_n(array[1]), -1);
				}
				else if (num == 2)
				{
					string[] array2 = tokens[i + 1].Split(this.splitSlash, StringSplitOptions.RemoveEmptyEntries);
					if (array2.Length == 2)
					{
						value.set_vertex(num2, this.parse_v(array2[0]), -1, this.parse_u(array2[1]));
					}
					else if (array2.Length == 3)
					{
						value.set_vertex(num2, this.parse_v(array2[0]), this.parse_n(array2[2]), this.parse_u(array2[1]));
					}
					else
					{
						this.emit_warning("parse_triangle unexpected face component " + tokens[num2]);
					}
				}
				if (i >= 2)
				{
					if (activeMaterial != null)
					{
						value.nMaterialID = activeMaterial.id;
						this.UsedMaterials[activeMaterial.id] = activeMaterial.name;
					}
					value.nGroupID = nActiveGroup;
					this.vTriangles.Add(value);
					if (value.is_complex())
					{
						this.HasComplexVertices = true;
					}
				}
			}
		}

		private void parse_triangle(string[] tokens, ref Triangle t)
		{
			int num = 0;
			if (tokens[1].IndexOf("//") != -1)
			{
				num = 1;
			}
			else if (tokens[1].IndexOf('/') != -1)
			{
				num = 2;
			}
			t.clear();
			for (int i = 0; i < 3; i++)
			{
				if (num == 0)
				{
					t.set_vertex(i, this.parse_v(tokens[i + 1]), -1, -1);
				}
				else if (num == 1)
				{
					string[] array = tokens[i + 1].Split(this.splitDoubleSlash, StringSplitOptions.RemoveEmptyEntries);
					t.set_vertex(i, this.parse_v(array[0]), this.parse_n(array[1]), -1);
				}
				else if (num == 2)
				{
					string[] array2 = tokens[i + 1].Split(this.splitSlash, StringSplitOptions.RemoveEmptyEntries);
					if (array2.Length == 2)
					{
						t.set_vertex(i, this.parse_v(array2[0]), -1, this.parse_u(array2[1]));
					}
					else if (array2.Length == 3)
					{
						t.set_vertex(i, this.parse_v(array2[0]), this.parse_n(array2[2]), this.parse_u(array2[1]));
					}
					else
					{
						this.emit_warning("parse_triangle unexpected face component " + tokens[i]);
					}
				}
			}
		}

		private string FindMTLFile(string sMTLFilePath)
		{
			foreach (string path in this.MTLFileSearchPaths)
			{
				string text = Path.Combine(path, sMTLFilePath);
				if (File.Exists(text))
				{
					return text;
				}
			}
			return null;
		}

		public IOReadResult ReadMaterials(string sPath)
		{
			if (this.nWarningLevel >= 1)
			{
				this.emit_warning("[OBJReader] ReadMaterials " + sPath);
			}
			StreamReader streamReader;
			try
			{
				streamReader = new StreamReader(sPath);
				if (streamReader.EndOfStream)
				{
					return new IOReadResult(IOCode.FileAccessError, "");
				}
			}
			catch
			{
				return new IOReadResult(IOCode.FileAccessError, "");
			}
			OBJMaterial objmaterial = null;
			while (streamReader.Peek() >= 0)
			{
				string text = streamReader.ReadLine();
				string[] array = text.Split(null, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length != 0 && array[0][0] != '#')
				{
					if (array[0] == "newmtl")
					{
						objmaterial = new OBJMaterial();
						objmaterial.name = array[1];
						objmaterial.id = this.Materials.Count;
						if (this.Materials.ContainsKey(objmaterial.name))
						{
							this.emit_warning(string.Concat(new string[]
							{
								"Material file ",
								sPath,
								" / material ",
								objmaterial.name,
								" : already exists in Material set. Replacing."
							}));
						}
						if (this.nWarningLevel >= 1)
						{
							this.emit_warning("[OBJReader] parsing material " + objmaterial.name);
						}
						this.Materials[objmaterial.name] = objmaterial;
					}
					else if (array[0] == "Ka")
					{
						if (objmaterial != null)
						{
							objmaterial.Ka = this.parse_mtl_color(array);
						}
					}
					else if (array[0] == "Kd")
					{
						if (objmaterial != null)
						{
							objmaterial.Kd = this.parse_mtl_color(array);
						}
					}
					else if (array[0] == "Ks")
					{
						if (objmaterial != null)
						{
							objmaterial.Ks = this.parse_mtl_color(array);
						}
					}
					else if (array[0] == "Ke")
					{
						if (objmaterial != null)
						{
							objmaterial.Ke = this.parse_mtl_color(array);
						}
					}
					else if (array[0] == "Tf")
					{
						if (objmaterial != null)
						{
							objmaterial.Tf = this.parse_mtl_color(array);
						}
					}
					else if (array[0] == "illum")
					{
						if (objmaterial != null)
						{
							objmaterial.illum = int.Parse(array[1]);
						}
					}
					else if (array[0] == "d")
					{
						if (objmaterial != null)
						{
							objmaterial.d = float.Parse(array[1]);
						}
					}
					else if (array[0] == "Tr")
					{
						if (objmaterial != null)
						{
							objmaterial.d = 1f - float.Parse(array[1]);
						}
					}
					else if (array[0] == "Ns")
					{
						if (objmaterial != null)
						{
							objmaterial.Ns = float.Parse(array[1]);
						}
					}
					else if (array[0] == "sharpness")
					{
						if (objmaterial != null)
						{
							objmaterial.sharpness = float.Parse(array[1]);
						}
					}
					else if (array[0] == "Ni")
					{
						if (objmaterial != null)
						{
							objmaterial.Ni = float.Parse(array[1]);
						}
					}
					else if (array[0] == "map_Ka")
					{
						if (objmaterial != null)
						{
							objmaterial.map_Ka = this.parse_mtl_path(text, array);
						}
					}
					else if (array[0] == "map_Kd")
					{
						if (objmaterial != null)
						{
							objmaterial.map_Kd = this.parse_mtl_path(text, array);
						}
					}
					else if (array[0] == "map_Ks")
					{
						if (objmaterial != null)
						{
							objmaterial.map_Ks = this.parse_mtl_path(text, array);
						}
					}
					else if (array[0] == "map_Ke")
					{
						if (objmaterial != null)
						{
							objmaterial.map_Ke = this.parse_mtl_path(text, array);
						}
					}
					else if (array[0] == "map_d")
					{
						if (objmaterial != null)
						{
							objmaterial.map_d = this.parse_mtl_path(text, array);
						}
					}
					else if (array[0] == "map_Ns")
					{
						if (objmaterial != null)
						{
							objmaterial.map_Ns = this.parse_mtl_path(text, array);
						}
					}
					else if (array[0] == "bump" || array[0] == "map_bump")
					{
						if (objmaterial != null)
						{
							objmaterial.bump = this.parse_mtl_path(text, array);
						}
					}
					else if (array[0] == "disp")
					{
						if (objmaterial != null)
						{
							objmaterial.disp = this.parse_mtl_path(text, array);
						}
					}
					else if (array[0] == "decal")
					{
						if (objmaterial != null)
						{
							objmaterial.decal = this.parse_mtl_path(text, array);
						}
					}
					else if (array[0] == "refl")
					{
						if (objmaterial != null)
						{
							objmaterial.refl = this.parse_mtl_path(text, array);
						}
					}
					else
					{
						this.emit_warning("unknown material command " + array[0]);
					}
				}
			}
			if (this.nWarningLevel >= 1)
			{
				this.emit_warning("[OBJReader] ReadMaterials completed");
			}
			return new IOReadResult(IOCode.Ok, "ok");
		}

		private string parse_mtl_path(string line, string[] tokens)
		{
			if (tokens.Length == 2)
			{
				return tokens[1];
			}
			return line.Substring(line.IndexOf(tokens[1]));
		}

		private Vector3f parse_mtl_color(string[] tokens)
		{
			if (tokens[1] == "spectral")
			{
				this.emit_warning("OBJReader::parse_material_color : spectral color not supported!");
				return new Vector3f(1f, 0f, 0f);
			}
			if (tokens[1] == "xyz")
			{
				this.emit_warning("OBJReader::parse_material_color : xyz color not supported!");
				return new Vector3f(1f, 0f, 0f);
			}
			float x = float.Parse(tokens[1]);
			float y = float.Parse(tokens[2]);
			float z = float.Parse(tokens[3]);
			return new Vector3f(x, y, z);
		}

		private OBJMaterial find_material(string sName)
		{
			if (this.Materials.ContainsKey(sName))
			{
				return this.Materials[sName];
			}
			try
			{
				return this.Materials.First((KeyValuePair<string, OBJMaterial> x) => string.Equals(x.Key, sName, StringComparison.OrdinalIgnoreCase)).Value;
			}
			catch
			{
			}
			this.emit_warning("unknown material " + sName + " referenced");
			return null;
		}

		private void emit_warning(string sMessage)
		{
			string key = sMessage.Substring(0, 15);
			int num = this.warningCount.ContainsKey(key) ? this.warningCount[key] : 0;
			num++;
			this.warningCount[key] = num;
			if (num > 10)
			{
				return;
			}
			if (num == 10)
			{
				sMessage += " (additional message surpressed)";
			}
			ParsingMessagesHandler parsingMessagesHandler = this.warningEvent;
			if (parsingMessagesHandler != null)
			{
				parsingMessagesHandler(sMessage, null);
			}
		}

		private DVector<double> vPositions;

		private DVector<float> vNormals;

		private DVector<float> vUVs;

		private DVector<float> vColors;

		private DVector<Triangle> vTriangles;

		private Dictionary<string, OBJMaterial> Materials;

		private Dictionary<int, string> UsedMaterials;

		private bool m_bOBJHasPerVertexColors;

		private int m_nUVComponents;

		private bool m_bOBJHasTriangleGroups;

		private int m_nSetInvalidGroupsTo;

		private string[] splitDoubleSlash;

		private char[] splitSlash;

		private int nWarningLevel;

		private Dictionary<string, int> warningCount = new Dictionary<string, int>();
	}
}
