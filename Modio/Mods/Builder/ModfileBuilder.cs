using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Modio.API.SchemaDefinitions;

namespace Modio.Mods.Builder
{
	public class ModfileBuilder
	{
		public string FilePath { get; private set; }

		public string Version { get; private set; }

		public string ChangeLog { get; private set; }

		public string MetadataBlob { get; private set; }

		public ModfileBuilder.Platform[] Platforms { get; private set; }

		private ModId ParentId
		{
			get
			{
				return this._parentModBuilder.EditTarget.Id;
			}
		}

		internal ModfileBuilder(ModBuilder parent)
		{
			this._parentModBuilder = parent;
		}

		public ModfileBuilder SetSourceDirectoryPath(string filePath)
		{
			this.FilePath = filePath;
			return this;
		}

		public ModfileBuilder SetVersion(string version)
		{
			this.Version = version;
			return this;
		}

		public ModfileBuilder SetChangelog(string changelog)
		{
			this.ChangeLog = changelog;
			return this;
		}

		public ModfileBuilder SetMetadataBlob(string metadataBlob)
		{
			this.MetadataBlob = metadataBlob;
			return this;
		}

		public ModfileBuilder SetPlatform(ModfileBuilder.Platform platform)
		{
			return this.SetPlatforms(new ModfileBuilder.Platform[]
			{
				platform
			});
		}

		public ModfileBuilder SetPlatforms(ICollection<ModfileBuilder.Platform> platforms)
		{
			this.Platforms = platforms.ToArray<ModfileBuilder.Platform>();
			return this;
		}

		public ModfileBuilder AppendPlatform(ModfileBuilder.Platform platform)
		{
			return this.AppendPlatforms(new ModfileBuilder.Platform[]
			{
				platform
			});
		}

		public ModfileBuilder AppendPlatforms(ICollection<ModfileBuilder.Platform> platforms)
		{
			this.Platforms = this.Platforms.Concat(platforms).ToArray<ModfileBuilder.Platform>();
			return this;
		}

		public ModBuilder FinishModfile()
		{
			return this._parentModBuilder;
		}

		internal Task<Error> PublishModfile()
		{
			ModfileBuilder.<PublishModfile>d__33 <PublishModfile>d__;
			<PublishModfile>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<PublishModfile>d__.<>4__this = this;
			<PublishModfile>d__.<>1__state = -1;
			<PublishModfile>d__.<>t__builder.Start<ModfileBuilder.<PublishModfile>d__33>(ref <PublishModfile>d__);
			return <PublishModfile>d__.<>t__builder.Task;
		}

		private Task<Error> AddMultipartModfile(Stream readStream)
		{
			ModfileBuilder.<AddMultipartModfile>d__34 <AddMultipartModfile>d__;
			<AddMultipartModfile>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<AddMultipartModfile>d__.<>4__this = this;
			<AddMultipartModfile>d__.readStream = readStream;
			<AddMultipartModfile>d__.<>1__state = -1;
			<AddMultipartModfile>d__.<>t__builder.Start<ModfileBuilder.<AddMultipartModfile>d__34>(ref <AddMultipartModfile>d__);
			return <AddMultipartModfile>d__.<>t__builder.Task;
		}

		private Task<Error> AddAllMulipartUploadParts(string uploadId, int partCount, Stream readStream)
		{
			ModfileBuilder.<AddAllMulipartUploadParts>d__35 <AddAllMulipartUploadParts>d__;
			<AddAllMulipartUploadParts>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<AddAllMulipartUploadParts>d__.<>4__this = this;
			<AddAllMulipartUploadParts>d__.uploadId = uploadId;
			<AddAllMulipartUploadParts>d__.partCount = partCount;
			<AddAllMulipartUploadParts>d__.readStream = readStream;
			<AddAllMulipartUploadParts>d__.<>1__state = -1;
			<AddAllMulipartUploadParts>d__.<>t__builder.Start<ModfileBuilder.<AddAllMulipartUploadParts>d__35>(ref <AddAllMulipartUploadParts>d__);
			return <AddAllMulipartUploadParts>d__.<>t__builder.Task;
		}

		private Task<ValueTuple<Error, ModfileObject?>> RetryAddMultipartModfile(string uploadId, string version, string changelog, string metadataBlob, string[] platforms, Stream readStream)
		{
			ModfileBuilder.<RetryAddMultipartModfile>d__36 <RetryAddMultipartModfile>d__;
			<RetryAddMultipartModfile>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, ModfileObject?>>.Create();
			<RetryAddMultipartModfile>d__.<>4__this = this;
			<RetryAddMultipartModfile>d__.uploadId = uploadId;
			<RetryAddMultipartModfile>d__.version = version;
			<RetryAddMultipartModfile>d__.changelog = changelog;
			<RetryAddMultipartModfile>d__.metadataBlob = metadataBlob;
			<RetryAddMultipartModfile>d__.platforms = platforms;
			<RetryAddMultipartModfile>d__.readStream = readStream;
			<RetryAddMultipartModfile>d__.<>1__state = -1;
			<RetryAddMultipartModfile>d__.<>t__builder.Start<ModfileBuilder.<RetryAddMultipartModfile>d__36>(ref <RetryAddMultipartModfile>d__);
			return <RetryAddMultipartModfile>d__.<>t__builder.Task;
		}

		private static string GetPlatformHeader(ModfileBuilder.Platform platform)
		{
			string result;
			switch (platform)
			{
			case ModfileBuilder.Platform.Windows:
				result = "windows";
				break;
			case ModfileBuilder.Platform.Mac:
				result = "mac";
				break;
			case ModfileBuilder.Platform.Linux:
				result = "linux";
				break;
			case ModfileBuilder.Platform.Android:
				result = "android";
				break;
			case ModfileBuilder.Platform.IOS:
				result = "ios";
				break;
			case ModfileBuilder.Platform.XboxOne:
				result = "xboxone";
				break;
			case ModfileBuilder.Platform.XboxSeriesX:
				result = "xboxseriesx";
				break;
			case ModfileBuilder.Platform.PlayStation4:
				result = "ps4";
				break;
			case ModfileBuilder.Platform.PlayStation5:
				result = "ps5";
				break;
			case ModfileBuilder.Platform.Switch:
				result = "switch";
				break;
			case ModfileBuilder.Platform.Oculus:
				result = "oculus";
				break;
			default:
				result = string.Empty;
				break;
			}
			return result;
		}

		private readonly ModBuilder _parentModBuilder;

		public enum Platform
		{
			Windows,
			Mac,
			Linux,
			Android,
			IOS,
			XboxOne,
			XboxSeriesX,
			PlayStation4,
			PlayStation5,
			Switch,
			Oculus
		}
	}
}
