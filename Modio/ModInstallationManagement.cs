using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Modio.API.SchemaDefinitions;
using Modio.Caching;
using Modio.Mods;
using Modio.Users;

namespace Modio
{
	public static class ModInstallationManagement
	{
		public static bool DownloadAndExtractAsSingleJob { get; set; } = true;

		public static Mod CurrentOperationOnMod
		{
			get
			{
				ModInstallationManagement.Job currentOperation = ModInstallationManagement._currentOperation;
				if (currentOperation == null)
				{
					return null;
				}
				return currentOperation.Mod;
			}
		}

		public static event ModInstallationManagement.InstallationManagementEventDelegate ManagementEvents;

		public static bool IsInitialized
		{
			get
			{
				return ModInstallationManagement._index != null;
			}
		}

		public static int PendingModOperationCount
		{
			get
			{
				Queue<ModInstallationManagement.Job> operationQueue = ModInstallationManagement._operationQueue;
				if (operationQueue == null)
				{
					return 0;
				}
				return operationQueue.Count;
			}
		}

		public static bool IsRunning
		{
			get
			{
				return ModInstallationManagement._isRunning;
			}
		}

		internal static Task<Error> Init()
		{
			ModInstallationManagement.<Init>d__28 <Init>d__;
			<Init>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<Init>d__.<>1__state = -1;
			<Init>d__.<>t__builder.Start<ModInstallationManagement.<Init>d__28>(ref <Init>d__);
			return <Init>d__.<>t__builder.Task;
		}

		internal static Task Shutdown()
		{
			ModInstallationManagement.<Shutdown>d__29 <Shutdown>d__;
			<Shutdown>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Shutdown>d__.<>1__state = -1;
			<Shutdown>d__.<>t__builder.Start<ModInstallationManagement.<Shutdown>d__29>(ref <Shutdown>d__);
			return <Shutdown>d__.<>t__builder.Task;
		}

		internal static void WakeUp()
		{
			if (ModInstallationManagement._index != null)
			{
				ModInstallationManagement.ExecuteJobs();
				return;
			}
			ModioLog warning = ModioLog.Warning;
			if (warning == null)
			{
				return;
			}
			warning.Log("[WakeUp] _index is null.");
		}

		public static void Activate()
		{
			ModInstallationManagement._isDeactivated = false;
			ModInstallationManagement.WakeUp();
		}

		public static void Deactivate(bool cancelCurrentJob)
		{
			ModInstallationManagement._isDeactivated = true;
			ModInstallationManagement._operationQueue.Clear();
			if (cancelCurrentJob)
			{
				ModInstallationManagement.CancelInstallOperation(ModInstallationManagement.CurrentOperationOnMod);
			}
		}

		public static bool IsModSubscribed(long modId, long userId)
		{
			ModIndex.IndexEntry indexEntry;
			return ModInstallationManagement.IsInitialized && ModInstallationManagement._index.Index.TryGetValue(modId, out indexEntry) && indexEntry.Subscribers.Contains(userId);
		}

		public static Task<ICollection<Mod>> GetAllInstalledMods(bool forceRefresh = false)
		{
			ModInstallationManagement.<GetAllInstalledMods>d__34 <GetAllInstalledMods>d__;
			<GetAllInstalledMods>d__.<>t__builder = AsyncTaskMethodBuilder<ICollection<Mod>>.Create();
			<GetAllInstalledMods>d__.forceRefresh = forceRefresh;
			<GetAllInstalledMods>d__.<>1__state = -1;
			<GetAllInstalledMods>d__.<>t__builder.Start<ModInstallationManagement.<GetAllInstalledMods>d__34>(ref <GetAllInstalledMods>d__);
			return <GetAllInstalledMods>d__.<>t__builder.Task;
		}

		public static long GetTotalDiskUsage(bool includeQueued)
		{
			ModIndex index = ModInstallationManagement._index;
			if (((index != null) ? index.Index : null) == null)
			{
				return 0L;
			}
			long num = 0L;
			foreach (KeyValuePair<long, ModIndex.IndexEntry> keyValuePair in ModInstallationManagement._index.Index)
			{
				long modId;
				ModIndex.IndexEntry indexEntry;
				keyValuePair.Deconstruct(out modId, out indexEntry);
				Mod modRespectingIndexCache = ModInstallationManagement.GetModRespectingIndexCache(modId);
				if (((modRespectingIndexCache != null) ? modRespectingIndexCache.File : null) != null && modRespectingIndexCache.File.State != ModFileState.FileOperationFailed)
				{
					long num2 = modRespectingIndexCache.File.FileSize;
					if (!includeQueued)
					{
						if (modRespectingIndexCache.File.State == ModFileState.Downloading || modRespectingIndexCache.File.State == ModFileState.Installing)
						{
							num2 = (long)((float)num2 * modRespectingIndexCache.File.FileStateProgress);
						}
						else if (modRespectingIndexCache.File.State != ModFileState.Installed)
						{
							continue;
						}
					}
					num += num2;
				}
			}
			return num;
		}

		private static Mod GetModRespectingIndexCache(long modId)
		{
			Mod mod;
			if (ModCache.TryGetMod(modId, out mod))
			{
				ModInstallationManagement._index.ModObjectCache[modId] = mod.LastModObject;
				return mod;
			}
			ModObject modObject;
			if (ModInstallationManagement._index.ModObjectCache.TryGetValue(modId, out modObject))
			{
				if (modObject.Id != 0L)
				{
					return ModCache.GetMod(modObject);
				}
				ModioLog message = ModioLog.Message;
				if (message != null)
				{
					message.Log(string.Format("Removing invalid cached mod object for mod: {0}", modId));
				}
				ModInstallationManagement._index.ModObjectCache.Remove(modId);
			}
			return Mod.Get(modId);
		}

		private static Task<Error> SaveIndex()
		{
			ModInstallationManagement.<SaveIndex>d__37 <SaveIndex>d__;
			<SaveIndex>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<SaveIndex>d__.<>1__state = -1;
			<SaveIndex>d__.<>t__builder.Start<ModInstallationManagement.<SaveIndex>d__37>(ref <SaveIndex>d__);
			return <SaveIndex>d__.<>t__builder.Task;
		}

		private static void ExecuteJobs()
		{
			ModInstallationManagement.<ExecuteJobs>d__38 <ExecuteJobs>d__;
			<ExecuteJobs>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<ExecuteJobs>d__.<>1__state = -1;
			<ExecuteJobs>d__.<>t__builder.Start<ModInstallationManagement.<ExecuteJobs>d__38>(ref <ExecuteJobs>d__);
		}

		private static Task EnqueueJobs()
		{
			ModInstallationManagement.<EnqueueJobs>d__39 <EnqueueJobs>d__;
			<EnqueueJobs>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<EnqueueJobs>d__.<>1__state = -1;
			<EnqueueJobs>d__.<>t__builder.Start<ModInstallationManagement.<EnqueueJobs>d__39>(ref <EnqueueJobs>d__);
			return <EnqueueJobs>d__.<>t__builder.Task;
		}

		public static Task<Error> StartTempModSession(List<ModId> tempMods, bool appendCurrentSession = false)
		{
			ModInstallationManagement.<StartTempModSession>d__40 <StartTempModSession>d__;
			<StartTempModSession>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<StartTempModSession>d__.tempMods = tempMods;
			<StartTempModSession>d__.appendCurrentSession = appendCurrentSession;
			<StartTempModSession>d__.<>1__state = -1;
			<StartTempModSession>d__.<>t__builder.Start<ModInstallationManagement.<StartTempModSession>d__40>(ref <StartTempModSession>d__);
			return <StartTempModSession>d__.<>t__builder.Task;
		}

		public static void EndCurrentTempModSession()
		{
			ModInstallationManagement._currentSessionMods.Clear();
			ModInstallationManagement.ExecuteJobs();
		}

		public static Task<Error> AddTemporaryMods(List<ModId> tempMods, int lifeTimeDaysOverride = -1)
		{
			ModInstallationManagement.<AddTemporaryMods>d__42 <AddTemporaryMods>d__;
			<AddTemporaryMods>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<AddTemporaryMods>d__.tempMods = tempMods;
			<AddTemporaryMods>d__.lifeTimeDaysOverride = lifeTimeDaysOverride;
			<AddTemporaryMods>d__.<>1__state = -1;
			<AddTemporaryMods>d__.<>t__builder.Start<ModInstallationManagement.<AddTemporaryMods>d__42>(ref <AddTemporaryMods>d__);
			return <AddTemporaryMods>d__.<>t__builder.Task;
		}

		private static void AddTemporaryMod(Mod modId, int lifetime)
		{
			ModIndex.IndexEntry entry = ModInstallationManagement._index.GetEntry(modId);
			switch (entry.FileState)
			{
			case ModFileState.None:
			case ModFileState.Uninstalling:
				entry.ExpiresAfter = ((lifetime == 0) ? DateTime.UnixEpoch : DateTime.Today.ToUniversalTime().AddDays((double)lifetime));
				ModInstallationManagement.WakeUp();
				return;
			case ModFileState.Queued:
			case ModFileState.Downloading:
			case ModFileState.Downloaded:
			case ModFileState.Installing:
			case ModFileState.Installed:
			case ModFileState.Updating:
				entry.ExpiresAfter = ((lifetime == 0) ? entry.ExpiresAfter : DateTime.Today.ToUniversalTime().AddDays((double)lifetime));
				break;
			case ModFileState.FileOperationFailed:
				break;
			default:
				return;
			}
		}

		public static void ClearExpiredTempMods()
		{
			ModInstallationManagement.ExecuteJobs();
		}

		private static void RetryInstallingTaintedMods()
		{
			foreach (KeyValuePair<long, ModIndex.IndexEntry> keyValuePair in ModInstallationManagement._index.Index)
			{
				if (keyValuePair.Value.FileState == ModFileState.FileOperationFailed)
				{
					keyValuePair.Value.FileState = ModFileState.None;
					Mod modRespectingIndexCache = ModInstallationManagement.GetModRespectingIndexCache(keyValuePair.Key);
					modRespectingIndexCache.File.State = ModFileState.None;
					modRespectingIndexCache.InvokeModUpdated(ModChangeType.FileState);
				}
			}
			ModInstallationManagement.WakeUp();
		}

		public static Task<Error> RetryInstallingMod(Mod mod)
		{
			ModInstallationManagement.<RetryInstallingMod>d__46 <RetryInstallingMod>d__;
			<RetryInstallingMod>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<RetryInstallingMod>d__.mod = mod;
			<RetryInstallingMod>d__.<>1__state = -1;
			<RetryInstallingMod>d__.<>t__builder.Start<ModInstallationManagement.<RetryInstallingMod>d__46>(ref <RetryInstallingMod>d__);
			return <RetryInstallingMod>d__.<>t__builder.Task;
		}

		public static void MarkModForUninstallation(Mod mod)
		{
			ModInstallationManagement._modsToUninstall.Add(mod);
			ModInstallationManagement.WakeUp();
		}

		private static void OnModSubscriptionChange(Mod mod, ModChangeType changeType)
		{
			if (ModChangeType.IsSubscribed == changeType && !mod.IsSubscribed)
			{
				ModInstallationManagement.CancelInstallOperation(mod);
			}
			if (!mod.IsSubscribed || !ModInstallationManagement._modsToUninstall.Contains(mod))
			{
				return;
			}
			ModInstallationManagement._modsToUninstall.Remove(mod);
			ModInstallationManagement.WakeUp();
		}

		private static void CancelInstallOperation(Mod mod)
		{
			if (ModInstallationManagement._currentOperation == null)
			{
				return;
			}
			if (ModInstallationManagement._currentOperation.Mod != mod)
			{
				return;
			}
			if (ModInstallationManagement._currentOperation.Type == ModInstallationManagement.OperationType.Download || ModInstallationManagement._currentOperation.Type == ModInstallationManagement.OperationType.Install)
			{
				ModInstallationManagement._currentOperation.Cancel();
			}
		}

		public static bool DoesModNeedUpdate(Mod mod)
		{
			return mod.File.State == ModFileState.Queued && ModInstallationManagement._index.GetEntry(mod).InstalledModfileId != -1L;
		}

		public static Task<bool> DownloadAndInstallMod(ModId modId)
		{
			ModInstallationManagement.<DownloadAndInstallMod>d__51 <DownloadAndInstallMod>d__;
			<DownloadAndInstallMod>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<DownloadAndInstallMod>d__.modId = modId;
			<DownloadAndInstallMod>d__.<>1__state = -1;
			<DownloadAndInstallMod>d__.<>t__builder.Start<ModInstallationManagement.<DownloadAndInstallMod>d__51>(ref <DownloadAndInstallMod>d__);
			return <DownloadAndInstallMod>d__.<>t__builder.Task;
		}

		public static Task<bool> IsThereAvailableSpaceFor(Mod mod)
		{
			ModInstallationManagement.<IsThereAvailableSpaceFor>d__62 <IsThereAvailableSpaceFor>d__;
			<IsThereAvailableSpaceFor>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<IsThereAvailableSpaceFor>d__.mod = mod;
			<IsThereAvailableSpaceFor>d__.<>1__state = -1;
			<IsThereAvailableSpaceFor>d__.<>t__builder.Start<ModInstallationManagement.<IsThereAvailableSpaceFor>d__62>(ref <IsThereAvailableSpaceFor>d__);
			return <IsThereAvailableSpaceFor>d__.<>t__builder.Task;
		}

		public static Task UninstallAllMods()
		{
			ModInstallationManagement.<UninstallAllMods>d__63 <UninstallAllMods>d__;
			<UninstallAllMods>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<UninstallAllMods>d__.<>1__state = -1;
			<UninstallAllMods>d__.<>t__builder.Start<ModInstallationManagement.<UninstallAllMods>d__63>(ref <UninstallAllMods>d__);
			return <UninstallAllMods>d__.<>t__builder.Task;
		}

		public static void RefreshMod(Mod mod)
		{
			if (!ModInstallationManagement.IsInitialized || ModInstallationManagement._isDeactivated)
			{
				return;
			}
			ModInstallationManagement._modsToRefresh.Add(mod);
			ModInstallationManagement.WakeUp();
		}

		public static void RefreshMods(List<Mod> mods)
		{
			if (!ModInstallationManagement.IsInitialized || ModInstallationManagement._isDeactivated)
			{
				return;
			}
			foreach (Mod item in mods)
			{
				ModInstallationManagement._modsToRefresh.Add(item);
			}
			ModInstallationManagement.WakeUp();
		}

		public static void NotifyLoggingOut()
		{
			if (!ModInstallationManagement.IsInitialized || ModInstallationManagement._isDeactivated)
			{
				return;
			}
			foreach (KeyValuePair<long, ModIndex.IndexEntry> keyValuePair in ModInstallationManagement._index.Index)
			{
				ModId modId = new ModId(keyValuePair.Key);
				bool flag = User.Current.ModRepository.IsSubscribed(modId);
				List<long> subscribers = keyValuePair.Value.Subscribers;
				if (flag && subscribers.Contains(User.Current.UserId))
				{
					subscribers.Remove(User.Current.Profile.UserId);
					ModInstallationManagement._index.IsDirty = true;
				}
				if (subscribers.Count <= 0)
				{
					Mod modRespectingIndexCache = ModInstallationManagement.GetModRespectingIndexCache(modId);
					if (modRespectingIndexCache.File.State == ModFileState.Queued && modRespectingIndexCache.File.InstallLocation == null)
					{
						modRespectingIndexCache.File.State = ModFileState.None;
						modRespectingIndexCache.InvokeModUpdated(ModChangeType.FileState);
						ModInstallationManagement.InstallationManagementEventDelegate managementEvents = ModInstallationManagement.ManagementEvents;
						if (managementEvents != null)
						{
							managementEvents(modRespectingIndexCache, modRespectingIndexCache.File, ModInstallationManagement.OperationType.Validate, ModInstallationManagement.OperationPhase.Completed);
						}
					}
				}
			}
		}

		public static bool ValidateInstalledMod(Mod mod)
		{
			if (mod.File.State != ModFileState.Installed)
			{
				return false;
			}
			if (ModioClient.DataStorage.DoesInstallExist(mod.Id, mod.File.Id))
			{
				return true;
			}
			mod.File.State = ModFileState.Queued;
			ModioLog message = ModioLog.Message;
			if (message != null)
			{
				message.Log("[ValidateInstalledMod] Validate failed, refreshing mod " + mod.Name);
			}
			ModInstallationManagement.RefreshMod(mod);
			return false;
		}

		internal static ModObject GetHiddenModObjectFromIndex(ModId modId, ModIndex tempIndex = null)
		{
			ModIndex modIndex = tempIndex ?? ModInstallationManagement._index;
			if (modIndex == null)
			{
				return default(ModObject);
			}
			long modFileId = -1L;
			ModIndex.IndexEntry indexEntry;
			if (modIndex.TryGetEntry(modId, out indexEntry))
			{
				modFileId = indexEntry.InstalledModfileId;
			}
			modIndex.ModObjectCache.Remove(modId);
			ModObject hiddenModObject = ModObject.GetHiddenModObject(modId, modFileId);
			modIndex.ModObjectCache[modId] = hiddenModObject;
			return hiddenModObject;
		}

		[CompilerGenerated]
		internal static void <EnqueueJobs>g__EnqueueJobsIfNeeded|39_0(ModIndex.IndexEntry entry, Mod mod)
		{
			if (ModInstallationManagement._unverifiedMods.Contains(mod))
			{
				ModInstallationManagement._operationQueue.Enqueue(new ModInstallationManagement.ValidateJob(mod));
			}
			if (mod.File == null || mod.File.State == ModFileState.Installing || mod.File.State == ModFileState.Updating || mod.File.State == ModFileState.FileOperationFailed)
			{
				return;
			}
			if (entry.InstalledModfileId == mod.File.Id && mod.File.Id > 0L)
			{
				if (mod.File.State != ModFileState.Installed)
				{
					ModInstallationManagement._operationQueue.Enqueue(new ModInstallationManagement.ValidateJob(mod));
				}
				return;
			}
			if (ModInstallationManagement._requestedModDownloads.Contains(mod.Id))
			{
				bool isUpdateJob = entry.InstalledModfileId != -1L;
				if (ModInstallationManagement.DownloadAndExtractAsSingleJob)
				{
					if (mod.File.State != ModFileState.Downloading)
					{
						ModInstallationManagement._operationQueue.Enqueue(new ModInstallationManagement.DownloadAndExtractJob(mod, isUpdateJob));
					}
				}
				else
				{
					if (entry.DownloadedModfileId != mod.File.Id && mod.File.State != ModFileState.Downloading)
					{
						ModInstallationManagement._operationQueue.Enqueue(new ModInstallationManagement.DownloadJob(mod));
					}
					ModInstallationManagement._operationQueue.Enqueue(new ModInstallationManagement.InstallJob(mod, isUpdateJob));
				}
				ModInstallationManagement._requestedModDownloads.Remove(mod.Id);
			}
			if (mod.File.State == ModFileState.None || mod.File.State == ModFileState.Installed)
			{
				mod.File.State = ModFileState.Queued;
				mod.InvokeModUpdated(ModChangeType.FileState);
				ModInstallationManagement.InstallationManagementEventDelegate managementEvents = ModInstallationManagement.ManagementEvents;
				if (managementEvents == null)
				{
					return;
				}
				managementEvents(mod, mod.File, ModInstallationManagement.OperationType.Validate, ModInstallationManagement.OperationPhase.Completed);
			}
		}

		private static bool _uninstallUnsubscribedMods = true;

		private static ModIndex _index;

		private static ModInstallationManagement.Job _currentOperation;

		private static bool _isRunning;

		private static Queue<ModInstallationManagement.Job> _operationQueue;

		private static HashSet<ModId> _requestedModDownloads = new HashSet<ModId>();

		private static HashSet<Mod> _modsToRefresh = new HashSet<Mod>();

		private static HashSet<ModId> _currentSessionMods = new HashSet<ModId>();

		private static HashSet<Mod> _modsToUninstall = new HashSet<Mod>();

		private static HashSet<Mod> _unverifiedMods = new HashSet<Mod>();

		private static bool _hasScannedMissingMods;

		private static bool _isDeactivated;

		public delegate void InstallationManagementEventDelegate(Mod mod, Modfile modfile, ModInstallationManagement.OperationType jobType, ModInstallationManagement.OperationPhase jobPhase);

		private abstract class Job
		{
			public Mod Mod
			{
				get
				{
					return this._mod;
				}
				private set
				{
					this._mod = value;
				}
			}

			protected Job(Mod mod, ModInstallationManagement.OperationType type)
			{
				this.Type = type;
				this.Mod = mod;
				this._cancellationTokenSource = new CancellationTokenSource();
				this.CancellationToken = this._cancellationTokenSource.Token;
			}

			public abstract Task<Error> Run();

			protected void PostEvent(ModInstallationManagement.OperationPhase jobPhase, ModFileState modState, Error errorCause = null)
			{
				if (this.Mod.File.State != modState)
				{
					this.Mod.File.State = modState;
					this.Mod.File.FileStateErrorCause = (errorCause ?? Error.None);
					this.Mod.InvokeModUpdated(ModChangeType.FileState);
				}
				this.Phase = jobPhase;
				ModInstallationManagement.InstallationManagementEventDelegate managementEvents = ModInstallationManagement.ManagementEvents;
				if (managementEvents == null)
				{
					return;
				}
				managementEvents(this.Mod, this.Mod.File, this.Type, jobPhase);
			}

			internal void Cancel()
			{
				this._cancellationTokenSource.Cancel();
			}

			internal abstract void GetPendingSpaceChange(ref long spaceRequired, ref long tempSpaceRequired);

			protected internal void ClearMod()
			{
				this.Mod = null;
			}

			private readonly CancellationTokenSource _cancellationTokenSource;

			public Func<Task<Error>> Operation;

			public readonly ModInstallationManagement.OperationType Type;

			private Mod _mod;

			protected readonly CancellationToken CancellationToken;

			protected ModInstallationManagement.OperationPhase Phase;
		}

		private class DownloadJob : ModInstallationManagement.Job
		{
			public DownloadJob(Mod mod) : base(mod, ModInstallationManagement.OperationType.Download)
			{
			}

			public override Task<Error> Run()
			{
				ModInstallationManagement.DownloadJob.<Run>d__1 <Run>d__;
				<Run>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
				<Run>d__.<>4__this = this;
				<Run>d__.<>1__state = -1;
				<Run>d__.<>t__builder.Start<ModInstallationManagement.DownloadJob.<Run>d__1>(ref <Run>d__);
				return <Run>d__.<>t__builder.Task;
			}

			internal override void GetPendingSpaceChange(ref long spaceRequired, ref long tempSpaceRequired)
			{
				ModInstallationManagement.OperationPhase phase = this.Phase;
				if (phase == ModInstallationManagement.OperationPhase.Checking)
				{
					tempSpaceRequired += base.Mod.File.ArchiveFileSize;
					return;
				}
				if (phase != ModInstallationManagement.OperationPhase.Started)
				{
					return;
				}
				tempSpaceRequired += (long)((float)base.Mod.File.ArchiveFileSize * (1f - base.Mod.File.FileStateProgress));
			}
		}

		private class InstallJob : ModInstallationManagement.Job
		{
			public InstallJob(Mod mod, bool isUpdateJob) : base(mod, isUpdateJob ? ModInstallationManagement.OperationType.Update : ModInstallationManagement.OperationType.Install)
			{
				this._isUpdateJob = isUpdateJob;
			}

			public override Task<Error> Run()
			{
				ModInstallationManagement.InstallJob.<Run>d__2 <Run>d__;
				<Run>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
				<Run>d__.<>4__this = this;
				<Run>d__.<>1__state = -1;
				<Run>d__.<>t__builder.Start<ModInstallationManagement.InstallJob.<Run>d__2>(ref <Run>d__);
				return <Run>d__.<>t__builder.Task;
			}

			internal override void GetPendingSpaceChange(ref long spaceRequired, ref long tempSpaceRequired)
			{
				tempSpaceRequired -= base.Mod.File.ArchiveFileSize;
				ModInstallationManagement.OperationPhase phase = this.Phase;
				if (phase != ModInstallationManagement.OperationPhase.Checking)
				{
					if (phase != ModInstallationManagement.OperationPhase.Started)
					{
						return;
					}
					spaceRequired += (long)((double)base.Mod.File.FileSize * (1.0 - (double)base.Mod.File.FileStateProgress));
					if (this._isUpdateJob && base.Mod.File.FileStateProgress == 0f)
					{
						spaceRequired -= ModInstallationManagement._index.GetEntry(base.Mod).InstallationSize;
					}
				}
				else
				{
					spaceRequired += base.Mod.File.FileSize;
					if (this._isUpdateJob)
					{
						spaceRequired -= ModInstallationManagement._index.GetEntry(base.Mod).InstallationSize;
						return;
					}
				}
			}

			private readonly bool _isUpdateJob;
		}

		private class DownloadAndExtractJob : ModInstallationManagement.Job
		{
			private bool IsUpdateJob
			{
				get
				{
					return this.Type == ModInstallationManagement.OperationType.Update;
				}
			}

			public DownloadAndExtractJob(Mod mod, bool isUpdateJob) : base(mod, isUpdateJob ? ModInstallationManagement.OperationType.Update : ModInstallationManagement.OperationType.Download)
			{
			}

			public override Task<Error> Run()
			{
				ModInstallationManagement.DownloadAndExtractJob.<Run>d__3 <Run>d__;
				<Run>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
				<Run>d__.<>4__this = this;
				<Run>d__.<>1__state = -1;
				<Run>d__.<>t__builder.Start<ModInstallationManagement.DownloadAndExtractJob.<Run>d__3>(ref <Run>d__);
				return <Run>d__.<>t__builder.Task;
			}

			internal override void GetPendingSpaceChange(ref long spaceRequired, ref long tempSpaceRequired)
			{
				ModInstallationManagement.OperationPhase phase = this.Phase;
				if (phase != ModInstallationManagement.OperationPhase.Checking)
				{
					if (phase != ModInstallationManagement.OperationPhase.Started)
					{
						return;
					}
					spaceRequired += (long)((double)base.Mod.File.FileSize * (1.0 - (double)base.Mod.File.FileStateProgress));
					if (this.IsUpdateJob && base.Mod.File.FileStateProgress == 0f)
					{
						spaceRequired -= ModInstallationManagement._index.GetEntry(base.Mod).InstallationSize;
					}
				}
				else
				{
					spaceRequired += base.Mod.File.FileSize;
					if (this.IsUpdateJob)
					{
						spaceRequired -= ModInstallationManagement._index.GetEntry(base.Mod).InstallationSize;
						return;
					}
				}
			}
		}

		private class UninstallJob : ModInstallationManagement.Job
		{
			public UninstallJob(Mod mod) : base(mod, ModInstallationManagement.OperationType.Uninstall)
			{
			}

			public override Task<Error> Run()
			{
				ModInstallationManagement.UninstallJob.<Run>d__1 <Run>d__;
				<Run>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
				<Run>d__.<>4__this = this;
				<Run>d__.<>1__state = -1;
				<Run>d__.<>t__builder.Start<ModInstallationManagement.UninstallJob.<Run>d__1>(ref <Run>d__);
				return <Run>d__.<>t__builder.Task;
			}

			internal override void GetPendingSpaceChange(ref long spaceRequired, ref long tempSpaceRequired)
			{
				spaceRequired -= base.Mod.File.FileSize;
			}
		}

		private class ValidateJob : ModInstallationManagement.Job
		{
			public ValidateJob(Mod mod) : base(mod, ModInstallationManagement.OperationType.Validate)
			{
			}

			public override Task<Error> Run()
			{
				ModInstallationManagement.ValidateJob.<Run>d__1 <Run>d__;
				<Run>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
				<Run>d__.<>4__this = this;
				<Run>d__.<>1__state = -1;
				<Run>d__.<>t__builder.Start<ModInstallationManagement.ValidateJob.<Run>d__1>(ref <Run>d__);
				return <Run>d__.<>t__builder.Task;
			}

			internal override void GetPendingSpaceChange(ref long spaceRequired, ref long tempSpaceRequired)
			{
			}
		}

		private class ScanMissingInstallsJob : ModInstallationManagement.Job
		{
			public ScanMissingInstallsJob() : base(null, ModInstallationManagement.OperationType.Scan)
			{
			}

			public override Task<Error> Run()
			{
				ModInstallationManagement.ScanMissingInstallsJob.<Run>d__1 <Run>d__;
				<Run>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
				<Run>d__.<>4__this = this;
				<Run>d__.<>1__state = -1;
				<Run>d__.<>t__builder.Start<ModInstallationManagement.ScanMissingInstallsJob.<Run>d__1>(ref <Run>d__);
				return <Run>d__.<>t__builder.Task;
			}

			internal override void GetPendingSpaceChange(ref long spaceRequired, ref long tempSpaceRequired)
			{
			}
		}

		private class ScanForInstalledModJob : ModInstallationManagement.Job
		{
			public ScanForInstalledModJob(Mod mod) : base(mod, ModInstallationManagement.OperationType.Scan)
			{
			}

			public override Task<Error> Run()
			{
				ModInstallationManagement.ScanForInstalledModJob.<Run>d__1 <Run>d__;
				<Run>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
				<Run>d__.<>4__this = this;
				<Run>d__.<>1__state = -1;
				<Run>d__.<>t__builder.Start<ModInstallationManagement.ScanForInstalledModJob.<Run>d__1>(ref <Run>d__);
				return <Run>d__.<>t__builder.Task;
			}

			internal override void GetPendingSpaceChange(ref long spaceRequired, ref long tempSpaceRequired)
			{
			}
		}

		public enum OperationType
		{
			Download,
			Install,
			Update,
			Uninstall,
			Validate,
			Scan
		}

		public enum OperationPhase
		{
			Checking,
			Started,
			Completed,
			Cancelled,
			Failed
		}
	}
}
