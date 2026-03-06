using System;
using System.Collections.Generic;
using System.IO;

namespace g3
{
	public class OBJFormatReader : MeshFormatReader
	{
		public List<string> SupportedExtensions
		{
			get
			{
				return new List<string>
				{
					"obj"
				};
			}
		}

		public IOReadResult ReadFile(string sFilename, IMeshBuilder builder, ReadOptions options, ParsingMessagesHandler messages)
		{
			IOReadResult result;
			try
			{
				using (FileStream fileStream = File.Open(sFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					OBJReader objreader = new OBJReader();
					if (options.ReadMaterials)
					{
						objreader.MTLFileSearchPaths.Add(Path.GetDirectoryName(sFilename));
					}
					objreader.warningEvent += messages;
					result = objreader.Read(new StreamReader(fileStream), options, builder);
				}
			}
			catch (Exception ex)
			{
				result = new IOReadResult(IOCode.FileAccessError, "Could not open file " + sFilename + " for reading : " + ex.Message);
			}
			return result;
		}

		public IOReadResult ReadFile(Stream stream, IMeshBuilder builder, ReadOptions options, ParsingMessagesHandler messages)
		{
			OBJReader objreader = new OBJReader();
			objreader.warningEvent += messages;
			return objreader.Read(new StreamReader(stream), options, builder);
		}
	}
}
