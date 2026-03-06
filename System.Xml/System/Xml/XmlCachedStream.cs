using System;
using System.IO;

namespace System.Xml
{
	internal class XmlCachedStream : MemoryStream
	{
		internal XmlCachedStream(Uri uri, Stream stream)
		{
			this.uri = uri;
			try
			{
				byte[] buffer = new byte[4096];
				int count;
				while ((count = stream.Read(buffer, 0, 4096)) > 0)
				{
					this.Write(buffer, 0, count);
				}
				base.Position = 0L;
			}
			finally
			{
				stream.Close();
			}
		}

		private const int MoveBufferSize = 4096;

		private Uri uri;
	}
}
