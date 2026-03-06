using System;
using System.Collections.Generic;
using System.IO;

namespace g3
{
	public class STLFormatReader : MeshFormatReader
	{
		public List<string> SupportedExtensions
		{
			get
			{
				return new List<string>
				{
					"stl"
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
			bool flag = Util.IsBinaryStream(stream, 500);
			stream.Seek(0L, SeekOrigin.Begin);
			STLReader stlreader = new STLReader();
			stlreader.warningEvent += messages;
			if (!flag)
			{
				return stlreader.Read(new StreamReader(stream), options, builder);
			}
			return stlreader.Read(new BinaryReader(stream), options, builder);
		}
	}
}
