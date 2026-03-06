using System;
using System.IO;
using ICSharpCode.SharpZipLib.Core;

namespace ICSharpCode.SharpZipLib.Zip
{
	public class DiskArchiveStorage : BaseArchiveStorage
	{
		public DiskArchiveStorage(ZipFile file, FileUpdateMode updateMode) : base(updateMode)
		{
			if (file.Name == null)
			{
				throw new ZipException("Cant handle non file archives");
			}
			this.fileName_ = file.Name;
		}

		public DiskArchiveStorage(ZipFile file) : this(file, FileUpdateMode.Safe)
		{
		}

		public override Stream GetTemporaryOutput()
		{
			this.temporaryName_ = PathUtils.GetTempFileName(this.temporaryName_);
			this.temporaryStream_ = File.Open(this.temporaryName_, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
			return this.temporaryStream_;
		}

		public override Stream ConvertTemporaryToFinal()
		{
			if (this.temporaryStream_ == null)
			{
				throw new ZipException("No temporary stream has been created");
			}
			Stream result = null;
			string tempFileName = PathUtils.GetTempFileName(this.fileName_);
			bool flag = false;
			try
			{
				this.temporaryStream_.Dispose();
				File.Move(this.fileName_, tempFileName);
				File.Move(this.temporaryName_, this.fileName_);
				flag = true;
				File.Delete(tempFileName);
				result = File.Open(this.fileName_, FileMode.Open, FileAccess.Read, FileShare.Read);
			}
			catch (Exception)
			{
				result = null;
				if (!flag)
				{
					File.Move(tempFileName, this.fileName_);
					File.Delete(this.temporaryName_);
				}
				throw;
			}
			return result;
		}

		public override Stream MakeTemporaryCopy(Stream stream)
		{
			stream.Dispose();
			this.temporaryName_ = PathUtils.GetTempFileName(this.fileName_);
			File.Copy(this.fileName_, this.temporaryName_, true);
			this.temporaryStream_ = new FileStream(this.temporaryName_, FileMode.Open, FileAccess.ReadWrite);
			return this.temporaryStream_;
		}

		public override Stream OpenForDirectUpdate(Stream stream)
		{
			Stream result;
			if (stream == null || !stream.CanWrite)
			{
				if (stream != null)
				{
					stream.Dispose();
				}
				result = new FileStream(this.fileName_, FileMode.Open, FileAccess.ReadWrite);
			}
			else
			{
				result = stream;
			}
			return result;
		}

		public override void Dispose()
		{
			if (this.temporaryStream_ != null)
			{
				this.temporaryStream_.Dispose();
			}
		}

		private Stream temporaryStream_;

		private readonly string fileName_;

		private string temporaryName_;
	}
}
