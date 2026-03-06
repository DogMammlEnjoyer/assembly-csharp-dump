using System;
using System.Collections.Generic;
using System.IO;

namespace g3
{
	internal class PLYReader : IMeshReader
	{
		public event ParsingMessagesHandler warningEvent;

		public IOReadResult Read(TextReader reader, ReadOptions options, IMeshBuilder builder)
		{
			List<PLYReader.element> list = new List<PLYReader.element>();
			if (!reader.ReadLine().Contains("ply"))
			{
				return new IOReadResult(IOCode.GarbageDataError, "Not a valid PLY file - Magic number failure");
			}
			string text = reader.ReadLine();
			if (text == null)
			{
				return new IOReadResult(IOCode.GarbageDataError, "Not a valid PLY file - header is corrupt");
			}
			string[] array;
			for (;;)
			{
				if (text.StartsWith("element"))
				{
					array = text.Split(' ', StringSplitOptions.None);
					if (array.Length != 3)
					{
						break;
					}
					PLYReader.element element = default(PLYReader.element);
					element.properties = new List<string>();
					element.types = new List<string>();
					element.list = new List<bool>();
					element.name = array[1];
					element.size = this.ParseInt(array[2]);
					for (;;)
					{
						text = reader.ReadLine();
						if (text == null)
						{
							goto Block_5;
						}
						if (!text.StartsWith("property"))
						{
							break;
						}
						array = text.Split(' ', StringSplitOptions.None);
						int num = array.Length;
						if (num != 3)
						{
							if (num != 5)
							{
								goto Block_8;
							}
							element.properties.Add(array[4].ToLower());
							element.types.Add(array[3].ToLower());
							element.list.Add(true);
						}
						else
						{
							element.properties.Add(array[2].ToLower());
							element.types.Add(array[1].ToLower());
							element.list.Add(false);
						}
					}
					list.Add(element);
				}
				else
				{
					if (text.StartsWith("end_header"))
					{
						goto IL_1E9;
					}
					if (!text.StartsWith("binary"))
					{
						if (text.StartsWith("comment crs "))
						{
							text.Remove(12);
							text = reader.ReadLine();
							if (text == null)
							{
								goto Block_12;
							}
						}
						else
						{
							text = reader.ReadLine();
							if (text == null)
							{
								goto Block_13;
							}
						}
					}
				}
			}
			return new IOReadResult(IOCode.GarbageDataError, "Not a valid PLY file - header is corrupt");
			Block_5:
			return new IOReadResult(IOCode.GarbageDataError, "Not a valid PLY file - header is corrupt");
			Block_8:
			return new IOReadResult(IOCode.GarbageDataError, "Not a valid PLY file - invalid element line" + text);
			Block_12:
			return new IOReadResult(IOCode.GarbageDataError, "Not a valid PLY file - header is corrupt");
			Block_13:
			return new IOReadResult(IOCode.GarbageDataError, "Not a valid PLY file - header is corrupt");
			IL_1E9:
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			for (int i = 0; i < list.Count; i++)
			{
				PLYReader.element element2 = list[i];
				if (element2.name == "vertex")
				{
					if (!element2.properties.Contains("x") || !element2.properties.Contains("y") || !element2.properties.Contains("z"))
					{
						return new IOReadResult(IOCode.GarbageDataError, "Verteces do not have XYZ");
					}
					if (element2.properties.Contains("red") && element2.properties.Contains("blue") && element2.properties.Contains("green"))
					{
						flag = true;
					}
					if (element2.properties.Contains("nx") && element2.properties.Contains("ny") && element2.properties.Contains("nz"))
					{
						flag2 = true;
					}
					if (element2.properties.Contains("u") && element2.properties.Contains("v"))
					{
						flag3 = true;
					}
				}
				if (element2.name == "faces" && !element2.properties.Contains("vertex_indices"))
				{
					return new IOReadResult(IOCode.GarbageDataError, "Faces do not have vertex indices");
				}
				if (element2.name == "edges" && (!element2.properties.Contains("vertex1") || !element2.properties.Contains("vertex2")))
				{
					return new IOReadResult(IOCode.GarbageDataError, "Edges do not have vertex indices");
				}
			}
			builder.AppendNewMesh(flag2, flag, flag3, false);
			text = reader.ReadLine();
			if (text == null)
			{
				return new IOReadResult(IOCode.GarbageDataError, "Not a valid PLY file - header is corrupt");
			}
			array = text.Split(' ', StringSplitOptions.None);
			for (int j = 0; j < list.Count; j++)
			{
				PLYReader.element element3 = list[j];
				for (int k = 0; k < element3.size; k++)
				{
					int num2 = element3.properties.Count;
					if (element3.list[0])
					{
						num2 += this.ParseInt(array[0]);
					}
					if (array.Length != num2)
					{
						return new IOReadResult(IOCode.GarbageDataError, "Not a valid PLY file - contains invalid line : " + text);
					}
					if (element3.name == "vertex")
					{
						Vector3d vtx = default(Vector3d);
						Vector3f norm = default(Vector3f);
						Vector3f cols = default(Vector3f);
						Vector2f uvs = default(Vector2f);
						vtx.x = this.ParseValue(array[element3.properties.FindIndex((string item) => item == "x")]);
						vtx.y = this.ParseValue(array[element3.properties.FindIndex((string item) => item == "y")]);
						vtx.z = this.ParseValue(array[element3.properties.FindIndex((string item) => item == "z")]);
						if (flag2)
						{
							norm.x = (float)this.ParseValue(array[element3.properties.FindIndex((string item) => item == "nx")]);
							norm.y = (float)this.ParseValue(array[element3.properties.FindIndex((string item) => item == "ny")]);
							norm.z = (float)this.ParseValue(array[element3.properties.FindIndex((string item) => item == "nz")]);
						}
						if (flag)
						{
							cols.x = (float)this.ParseValue(array[element3.properties.FindIndex((string item) => item == "red")]);
							cols.y = (float)this.ParseValue(array[element3.properties.FindIndex((string item) => item == "blue")]);
							cols.z = (float)this.ParseValue(array[element3.properties.FindIndex((string item) => item == "green")]);
						}
						if (flag3)
						{
							uvs.x = (float)this.ParseValue(array[element3.properties.FindIndex((string item) => item == "u")]);
							uvs.y = (float)this.ParseValue(array[element3.properties.FindIndex((string item) => item == "v")]);
						}
						this.append_vertex(builder, vtx, norm, cols, uvs, flag2, flag, flag3);
					}
					else if (element3.name == "face")
					{
						if (this.ParseInt(array[0]) != 3)
						{
							this.emit_warning("[PLYReader] cann only read triangles");
							return new IOReadResult(IOCode.FormatNotSupportedError, "Can only read tri faces");
						}
						this.append_triangle(builder, new Index3i
						{
							a = this.ParseInt(array[1]),
							b = this.ParseInt(array[2]),
							c = this.ParseInt(array[3])
						});
					}
					text = reader.ReadLine();
					if (text == null && j != list.Count - 1 && k != element3.size - 1)
					{
						return new IOReadResult(IOCode.GarbageDataError, " does not contain enough definitions of type " + element3.name);
					}
					array = ((text != null) ? text.Split(' ', StringSplitOptions.None) : null);
				}
			}
			return new IOReadResult(IOCode.Ok, "");
		}

		private int append_vertex(IMeshBuilder builder, Vector3d vtx, Vector3f norm, Vector3f cols, Vector2f uvs, bool bHaveNormals, bool bHaveColors, bool bHaveUVs)
		{
			if (!bHaveNormals && !bHaveColors && !bHaveUVs)
			{
				return builder.AppendVertex(vtx.x, vtx.y, vtx.z);
			}
			NewVertexInfo info = default(NewVertexInfo);
			info.bHaveC = (info.bHaveN = (info.bHaveUV = false));
			info.v = vtx;
			if (bHaveNormals)
			{
				info.bHaveN = true;
				info.n = norm;
			}
			if (bHaveColors)
			{
				info.bHaveC = true;
				info.c = cols;
			}
			if (bHaveUVs)
			{
				info.bHaveUV = true;
				info.uv = uvs;
			}
			return builder.AppendVertex(info);
		}

		private int append_triangle(IMeshBuilder builder, Index3i tri)
		{
			if (tri.a < 0 || tri.b < 0 || tri.c < 0)
			{
				this.emit_warning(string.Format("[PLYReader] invalid triangle:  {0} {1} {2}", tri.a, tri.b, tri.c));
				return -1;
			}
			return builder.AppendTriangle(tri.a, tri.b, tri.c);
		}

		public IOReadResult Read(BinaryReader reader, ReadOptions options, IMeshBuilder builder)
		{
			return new IOReadResult(IOCode.FormatNotSupportedError, "text read not supported for 3DS format");
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

		private double ParseValue(string value)
		{
			value = value.TrimStart(PLYReader.TRIM_CHARS).TrimEnd(PLYReader.TRIM_CHARS).Replace("\\", "");
			double result;
			if (double.TryParse(value, out result))
			{
				return result;
			}
			return 0.0;
		}

		private int ParseInt(string value)
		{
			value = value.TrimStart(PLYReader.TRIM_CHARS).TrimEnd(PLYReader.TRIM_CHARS).Replace("\\", "");
			int result;
			if (int.TryParse(value, out result))
			{
				return result;
			}
			return 0;
		}

		private static char[] TRIM_CHARS = new char[]
		{
			'"',
			'/',
			'\\',
			' '
		};

		private Dictionary<string, int> warningCount = new Dictionary<string, int>();

		private bool hasMesh;

		public struct element
		{
			public bool Equals(PLYReader.element other)
			{
				return this.name == other.name;
			}

			public string name;

			public List<string> properties;

			public List<string> types;

			public List<bool> list;

			public int size;
		}
	}
}
