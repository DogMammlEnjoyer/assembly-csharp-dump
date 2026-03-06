using System;
using System.IO;

namespace g3
{
	public class BinaryG3Reader : IMeshReader
	{
		public IOReadResult Read(BinaryReader reader, ReadOptions options, IMeshBuilder builder)
		{
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				DMesh3 dmesh = new DMesh3(true, false, false, false);
				gSerialization.Restore(dmesh, reader);
				builder.AppendNewMesh(dmesh);
			}
			return new IOReadResult(IOCode.Ok, "");
		}

		public IOReadResult Read(TextReader reader, ReadOptions options, IMeshBuilder builder)
		{
			throw new NotSupportedException("BinaryG3Reader Writer does not support ascii mode");
		}
	}
}
