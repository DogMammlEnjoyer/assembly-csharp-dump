using System;
using System.Collections.Generic;
using System.IO;

namespace g3
{
	public class DS3FormatReader : MeshFormatReader
	{
		public List<string> SupportedExtensions
		{
			get
			{
				return new List<string>
				{
					"3ds"
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
					result = this.ReadFile(fileStream, builder, options, messages);
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
			DS3Reader ds3Reader = new DS3Reader();
			ds3Reader.warningEvent += messages;
			return ds3Reader.Read(new BinaryReader(stream), options, builder);
		}
	}
}
