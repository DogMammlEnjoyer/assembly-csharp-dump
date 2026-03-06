using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Modio.API.SchemaDefinitions;
using Modio.Images;
using Modio.Mods;
using Newtonsoft.Json;

namespace Modio
{
	[Serializable]
	public class ModIndex
	{
		[JsonIgnore]
		public bool IsDirty { get; internal set; }

		public ModIndex()
		{
			Mod.AddChangeListener(ModChangeType.ModObject, new Action<Mod, ModChangeType>(this.OnModObjectUpdate));
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"result"
		})]
		internal static Task<ValueTuple<Error, ModIndex>> CreateIndexFromScan()
		{
			ModIndex.<CreateIndexFromScan>d__7 <CreateIndexFromScan>d__;
			<CreateIndexFromScan>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, ModIndex>>.Create();
			<CreateIndexFromScan>d__.<>1__state = -1;
			<CreateIndexFromScan>d__.<>t__builder.Start<ModIndex.<CreateIndexFromScan>d__7>(ref <CreateIndexFromScan>d__);
			return <CreateIndexFromScan>d__.<>t__builder.Task;
		}

		internal Task<bool> UpdateIndexWithMissingEntriesFromScan()
		{
			ModIndex.<UpdateIndexWithMissingEntriesFromScan>d__8 <UpdateIndexWithMissingEntriesFromScan>d__;
			<UpdateIndexWithMissingEntriesFromScan>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<UpdateIndexWithMissingEntriesFromScan>d__.<>4__this = this;
			<UpdateIndexWithMissingEntriesFromScan>d__.<>1__state = -1;
			<UpdateIndexWithMissingEntriesFromScan>d__.<>t__builder.Start<ModIndex.<UpdateIndexWithMissingEntriesFromScan>d__8>(ref <UpdateIndexWithMissingEntriesFromScan>d__);
			return <UpdateIndexWithMissingEntriesFromScan>d__.<>t__builder.Task;
		}

		[return: TupleElementNames(new string[]
		{
			"modIsInstalled",
			"installedModFileId"
		})]
		internal Task<ValueTuple<bool, long>> UpdateIndexForMod(Mod mod)
		{
			ModIndex.<UpdateIndexForMod>d__9 <UpdateIndexForMod>d__;
			<UpdateIndexForMod>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, long>>.Create();
			<UpdateIndexForMod>d__.<>4__this = this;
			<UpdateIndexForMod>d__.mod = mod;
			<UpdateIndexForMod>d__.<>1__state = -1;
			<UpdateIndexForMod>d__.<>t__builder.Start<ModIndex.<UpdateIndexForMod>d__9>(ref <UpdateIndexForMod>d__);
			return <UpdateIndexForMod>d__.<>t__builder.Task;
		}

		internal ModIndex.IndexEntry GetEntry(Mod mod)
		{
			ModIndex.IndexEntry indexEntry;
			if (this.Index.TryGetValue(mod.Id, out indexEntry))
			{
				return indexEntry;
			}
			indexEntry = new ModIndex.IndexEntry();
			this.Index[mod.Id] = indexEntry;
			this.ModObjectCache[mod.Id] = mod.LastModObject;
			ModioImageSource<Mod.LogoResolution> logo = mod.Logo;
			if (logo != null)
			{
				logo.CacheLowestResolutionOnDisk(true);
			}
			return indexEntry;
		}

		internal bool TryGetEntry(ModId modId, out ModIndex.IndexEntry entry)
		{
			return this.Index.TryGetValue(modId, out entry);
		}

		internal void RemoveEntry(Mod mod)
		{
			this.Index.Remove(mod.Id);
			this.ModObjectCache.Remove(mod.Id);
			ModioImageSource<Mod.LogoResolution> logo = mod.Logo;
			if (logo == null)
			{
				return;
			}
			logo.CacheLowestResolutionOnDisk(false);
		}

		internal void Shutdown()
		{
			Mod.RemoveChangeListener(ModChangeType.ModObject, new Action<Mod, ModChangeType>(this.OnModObjectUpdate));
		}

		private void OnModObjectUpdate(Mod mod, ModChangeType changeType)
		{
			ModIndex.IndexEntry indexEntry;
			if (!this.TryGetEntry(mod.Id, out indexEntry))
			{
				return;
			}
			this.ModObjectCache[mod.Id] = mod.LastModObject;
			ModioImageSource<Mod.LogoResolution> logo = mod.Logo;
			if (logo == null)
			{
				return;
			}
			logo.CacheLowestResolutionOnDisk(true);
		}

		internal Task RefreshIndexModObjects(bool installedOnly = true)
		{
			ModIndex.<RefreshIndexModObjects>d__16 <RefreshIndexModObjects>d__;
			<RefreshIndexModObjects>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<RefreshIndexModObjects>d__.<>4__this = this;
			<RefreshIndexModObjects>d__.installedOnly = installedOnly;
			<RefreshIndexModObjects>d__.<>1__state = -1;
			<RefreshIndexModObjects>d__.<>t__builder.Start<ModIndex.<RefreshIndexModObjects>d__16>(ref <RefreshIndexModObjects>d__);
			return <RefreshIndexModObjects>d__.<>t__builder.Task;
		}

		[JsonProperty]
		internal Dictionary<long, ModIndex.IndexEntry> Index = new Dictionary<long, ModIndex.IndexEntry>();

		[JsonProperty]
		internal Dictionary<long, ModObject> ModObjectCache = new Dictionary<long, ModObject>();

		[Serializable]
		internal class IndexEntry
		{
			public const long ID_NONE = -1L;

			public long DownloadedModfileId = -1L;

			public long InstalledModfileId = -1L;

			public long InstallationSize;

			public List<long> Subscribers = new List<long>();

			public DateTime ExpiresAfter = DateTime.UnixEpoch;

			public ModFileState FileState;
		}
	}
}
