using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject(MemberSerialization.Fields)]
	internal readonly struct ModfileObject
	{
		[JsonConstructor]
		public ModfileObject(long id, long mod_id, long date_added, long date_updated, long date_scanned, long virus_status, long virus_positive, string virustotal_hash, long filesize, long filesize_uncompressed, FilehashObject filehash, string filename, string version, string changelog, string metadata_blob, DownloadObject download, ModfilePlatformObject[] platforms)
		{
			this.Id = id;
			this.ModId = mod_id;
			this.DateAdded = date_added;
			this.DateUpdated = date_updated;
			this.DateScanned = date_scanned;
			this.VirusStatus = virus_status;
			this.VirusPositive = virus_positive;
			this.VirustotalHash = virustotal_hash;
			this.Filesize = filesize;
			this.FilesizeUncompressed = filesize_uncompressed;
			this.Filehash = filehash;
			this.Filename = filename;
			this.Version = version;
			this.Changelog = changelog;
			this.MetadataBlob = metadata_blob;
			this.Download = download;
			this.Platforms = platforms;
		}

		internal readonly long Id;

		internal readonly long ModId;

		internal readonly long DateAdded;

		internal readonly long DateUpdated;

		internal readonly long DateScanned;

		internal readonly long VirusStatus;

		internal readonly long VirusPositive;

		internal readonly string VirustotalHash;

		internal readonly long Filesize;

		internal readonly long FilesizeUncompressed;

		internal readonly FilehashObject Filehash;

		internal readonly string Filename;

		internal readonly string Version;

		internal readonly string Changelog;

		internal readonly string MetadataBlob;

		internal readonly DownloadObject Download;

		internal readonly ModfilePlatformObject[] Platforms;
	}
}
