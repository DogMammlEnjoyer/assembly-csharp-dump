using System;
using Modio.API.SchemaDefinitions;

namespace Modio.Mods
{
	public class Modfile
	{
		public long ModId { get; private set; }

		public long Id { get; private set; }

		public long FileSize { get; private set; }

		public long ArchiveFileSize { get; private set; }

		public string InstallLocation { get; internal set; }

		public string Version { get; private set; }

		public string MetadataBlob { get; private set; }

		public ModFileState State { get; internal set; }

		public Error FileStateErrorCause { get; internal set; } = Error.None;

		public float FileStateProgress { get; internal set; }

		public long DownloadingBytesPerSecond { get; internal set; }

		public ModfileDownloadReference Download { get; private set; }

		public string Md5Hash { get; private set; }

		internal Modfile(ModfileObject modfileObject)
		{
			this.ApplyDetailsFromModfileObject(modfileObject);
		}

		internal void ApplyDetailsFromModfileObject(ModfileObject modfileObject)
		{
			this.ModId = modfileObject.ModId;
			this.Id = modfileObject.Id;
			this.FileSize = modfileObject.FilesizeUncompressed;
			this.ArchiveFileSize = modfileObject.Filesize;
			this.Version = modfileObject.Version;
			this.Download = new ModfileDownloadReference(modfileObject.Download);
			this.Md5Hash = modfileObject.Filehash.Md5;
			this.MetadataBlob = modfileObject.MetadataBlob;
		}
	}
}
