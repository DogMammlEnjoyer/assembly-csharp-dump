using System;
using System.IO;

namespace Backtrace.Unity.Model.Breadcrumbs.Storage
{
	internal sealed class BreadcrumbFile : IBreadcrumbFile
	{
		public BreadcrumbFile(string path)
		{
			this._path = path;
		}

		public void Delete()
		{
			File.Delete(this._path);
		}

		public bool Exists()
		{
			return File.Exists(this._path);
		}

		public Stream GetCreateStream()
		{
			return new FileStream(this._path, FileMode.CreateNew, FileAccess.Write);
		}

		public Stream GetIOStream()
		{
			return new FileStream(this._path, FileMode.Open, FileAccess.ReadWrite);
		}

		public Stream GetWriteStream()
		{
			return new FileStream(this._path, FileMode.Open, FileAccess.Write);
		}

		private readonly string _path;
	}
}
