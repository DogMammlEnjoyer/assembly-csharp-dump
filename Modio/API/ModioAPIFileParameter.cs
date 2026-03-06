using System;
using System.IO;

namespace Modio.API
{
	public struct ModioAPIFileParameter
	{
		public static ModioAPIFileParameter None
		{
			get
			{
				return new ModioAPIFileParameter
				{
					Unused = true
				};
			}
		}

		public ModioAPIFileParameter(string name, string contentType, string path)
		{
			this.Name = name;
			this.ContentType = contentType;
			this.MediaType = "multipart/form-data";
			this.Path = path;
			this.Unused = false;
			this._stream = null;
		}

		public ModioAPIFileParameter(Stream stream)
		{
			this = default(ModioAPIFileParameter);
			this._stream = stream;
		}

		public ModioAPIFileParameter(Stream stream, string name, string contentType)
		{
			this = default(ModioAPIFileParameter);
			this._stream = stream;
			this.ContentType = contentType;
			this.Name = name;
		}

		public Stream GetContent()
		{
			Stream result;
			if ((result = this._stream) == null)
			{
				if (this.Path != null)
				{
					return new FileStream(this.Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				}
				result = null;
			}
			return result;
		}

		public bool Unused;

		public string Name;

		public readonly string ContentType;

		public readonly string MediaType;

		public string Path;

		private readonly Stream _stream;
	}
}
