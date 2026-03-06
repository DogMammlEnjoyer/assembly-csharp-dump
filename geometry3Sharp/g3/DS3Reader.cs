using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace g3
{
	internal class DS3Reader : IMeshReader
	{
		public event ParsingMessagesHandler warningEvent;

		public IOReadResult Read(BinaryReader reader, ReadOptions options, IMeshBuilder builder)
		{
			this.MeshName = "";
			this.hasMesh = false;
			this.is3ds = false;
			for (;;)
			{
				ushort num;
				try
				{
					num = reader.ReadUInt16();
				}
				catch
				{
					break;
				}
				string text = num.ToString("X");
				uint num2 = reader.ReadUInt32();
				uint num3 = <PrivateImplementationDetails>.ComputeStringHash(text);
				if (num3 <= 2699927877U)
				{
					if (num3 <= 1544088527U)
					{
						if (num3 != 1443275718U)
						{
							if (num3 == 1544088527U)
							{
								if (text == "4110")
								{
									ushort num4 = reader.ReadUInt16();
									for (int i = 0; i < (int)num4; i++)
									{
										double x = (double)reader.ReadSingle();
										double y = (double)reader.ReadSingle();
										double z = (double)reader.ReadSingle();
										builder.AppendVertex(x, y, z);
									}
									continue;
								}
							}
						}
						else if (text == "4100")
						{
							builder.AppendNewMesh(false, false, false, false);
							if (builder.SupportsMetaData)
							{
								builder.AppendMetaData("name", this.MeshName);
								continue;
							}
							continue;
						}
					}
					else if (num3 != 1669477685U)
					{
						if (num3 == 2699927877U)
						{
							if (text == "4D4D")
							{
								this.is3ds = true;
								reader.ReadChars(10);
								continue;
							}
						}
					}
					else if (text == "3D3D")
					{
						reader.ReadChars(10);
						continue;
					}
				}
				else if (num3 <= 3724340354U)
				{
					if (num3 != 3567127025U)
					{
						if (num3 == 3724340354U)
						{
							if (text == "4140")
							{
								ushort num5 = reader.ReadUInt16();
								for (ushort num6 = 0; num6 < num5; num6 += 1)
								{
									Vector2f uvs = new Vector2f(reader.ReadSingle(), reader.ReadSingle());
									builder.SetVertexUV((int)num6, uvs);
								}
								continue;
							}
						}
					}
					else if (text == "4000")
					{
						List<char> list = new List<char>();
						for (;;)
						{
							char c = reader.ReadChar();
							if (c == '\0')
							{
								break;
							}
							list.Add(c);
						}
						this.MeshName = new string(list.ToArray<char>());
						continue;
					}
				}
				else if (num3 != 3925965972U)
				{
					if (num3 == 4026778781U)
					{
						if (text == "4130")
						{
							List<char> list2 = new List<char>();
							for (;;)
							{
								char c2 = reader.ReadChar();
								if (c2 == '\0')
								{
									break;
								}
								list2.Add(c2);
							}
							new string(list2.ToArray<char>());
							ushort num7 = reader.ReadUInt16();
							for (int j = 0; j < (int)num7; j++)
							{
								reader.ReadUInt16();
							}
							continue;
						}
					}
				}
				else if (text == "4120")
				{
					ushort num8 = reader.ReadUInt16();
					for (int k = 0; k < (int)num8; k++)
					{
						int i2 = (int)reader.ReadInt16();
						int j2 = (int)reader.ReadInt16();
						int k2 = (int)reader.ReadInt16();
						reader.ReadUInt16();
						builder.AppendTriangle(i2, j2, k2);
					}
					continue;
				}
				reader.ReadChars((int)(num2 - 6U));
			}
			if (!this.is3ds)
			{
				return new IOReadResult(IOCode.FileAccessError, "File is not in .3DS format");
			}
			if (!this.hasMesh)
			{
				return new IOReadResult(IOCode.FileParsingError, "no mesh found in file");
			}
			return new IOReadResult(IOCode.Ok, "");
		}

		public IOReadResult Read(TextReader reader, ReadOptions options, IMeshBuilder builder)
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

		private Dictionary<string, int> warningCount = new Dictionary<string, int>();

		private string MeshName;

		private bool hasMesh;

		private bool is3ds;
	}
}
