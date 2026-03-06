using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using Modio.Errors;
using Modio.Mods;
using Modio.Users;

namespace Modio.FileIO
{
	public class BaseDataStorage : IModioDataStorage
	{
		public virtual Task<Error> Init()
		{
			this.SetupRootPaths();
			this.OngoingTaskCount = 0;
			this.ShutdownTokenSource = new CancellationTokenSource();
			this.ShutdownToken = this.ShutdownTokenSource.Token;
			this.IsShuttingDown = false;
			this.Initialized = true;
			this.MigrateLegacyModInstalls();
			return Task.FromResult<Error>(Error.None);
		}

		protected virtual void SetupRootPaths()
		{
			this.GameId = ModioServices.Resolve<ModioSettings>().GameId;
			this.Root = string.Format("{0}{1}", Path.Combine(ModioServices.Resolve<IModioRootPathProvider>().Path, "mod.io", this.GameId.ToString()), Path.DirectorySeparatorChar);
			this.UserRoot = string.Format("{0}{1}", Path.Combine(ModioServices.Resolve<IModioRootPathProvider>().UserPath, "mod.io", this.GameId.ToString()), Path.DirectorySeparatorChar);
		}

		public virtual Task Shutdown()
		{
			BaseDataStorage.<Shutdown>d__10 <Shutdown>d__;
			<Shutdown>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Shutdown>d__.<>4__this = this;
			<Shutdown>d__.<>1__state = -1;
			<Shutdown>d__.<>t__builder.Start<BaseDataStorage.<Shutdown>d__10>(ref <Shutdown>d__);
			return <Shutdown>d__.<>t__builder.Task;
		}

		[ModioDebugMenu]
		public static void DebugDeleteAllGameData()
		{
			ModioClient.DataStorage.DeleteAllGameData();
			User.LogOut();
		}

		public Task<Error> DeleteAllGameData()
		{
			if (!this.Initialized)
			{
				return Task.FromResult<Error>(new Error(ErrorCode.NOT_INITIALIZED));
			}
			Error error = this.DeleteDirectoryAndContents(this.Root);
			if (error)
			{
				return Task.FromResult<Error>(error);
			}
			error = this.DeleteDirectoryAndContents(this.UserRoot);
			if (error)
			{
				return Task.FromResult<Error>(error);
			}
			return Task.FromResult<Error>(Error.None);
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"result"
		})]
		protected virtual Task<ValueTuple<Error, T>> ReadData<T>(string filePath)
		{
			BaseDataStorage.<ReadData>d__13<T> <ReadData>d__;
			<ReadData>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, T>>.Create();
			<ReadData>d__.<>4__this = this;
			<ReadData>d__.filePath = filePath;
			<ReadData>d__.<>1__state = -1;
			<ReadData>d__.<>t__builder.Start<BaseDataStorage.<ReadData>d__13<T>>(ref <ReadData>d__);
			return <ReadData>d__.<>t__builder.Task;
		}

		protected virtual Task<Error> WriteData<T>(T data, string filePath)
		{
			BaseDataStorage.<WriteData>d__14<T> <WriteData>d__;
			<WriteData>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<WriteData>d__.<>4__this = this;
			<WriteData>d__.data = data;
			<WriteData>d__.filePath = filePath;
			<WriteData>d__.<>1__state = -1;
			<WriteData>d__.<>t__builder.Start<BaseDataStorage.<WriteData>d__14<T>>(ref <WriteData>d__);
			return <WriteData>d__.<>t__builder.Task;
		}

		private Task<Error> DeleteData(string filePath)
		{
			Error error = this.DeleteFile(filePath);
			if (error)
			{
				ModioLog error2 = ModioLog.Error;
				if (error2 != null)
				{
					error2.Log(string.Format("Error deleting [{0}] game data: {1}\nAt: {2}", this.GameId, error.GetMessage(), filePath));
				}
			}
			return Task.FromResult<Error>(error);
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"result"
		})]
		public virtual Task<ValueTuple<Error, GameData>> ReadGameData()
		{
			return this.ReadData<GameData>(this.GetGameDataFilePath());
		}

		public virtual Task<Error> WriteGameData(GameData gameData)
		{
			return this.WriteData<GameData>(gameData, this.GetGameDataFilePath());
		}

		public virtual Task<Error> DeleteGameData()
		{
			return this.DeleteData(this.GetGameDataFilePath());
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"index"
		})]
		public virtual Task<ValueTuple<Error, ModIndex>> ReadIndexData()
		{
			return this.ReadData<ModIndex>(this.GetIndexFilePath());
		}

		public virtual Task<Error> WriteIndexData(ModIndex index)
		{
			return this.WriteData<ModIndex>(index, this.GetIndexFilePath());
		}

		public virtual Task<Error> DeleteIndexData()
		{
			return this.DeleteData(this.GetIndexFilePath());
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"result"
		})]
		public virtual Task<ValueTuple<Error, UserSaveObject>> ReadUserData(string localUserId)
		{
			return this.ReadData<UserSaveObject>(this.GetUserDataFilePath(localUserId));
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"result"
		})]
		public virtual Task<ValueTuple<Error, LegacyUserSaveObject>> ReadLegacyUserData(string localUserId)
		{
			BaseDataStorage.<ReadLegacyUserData>d__23 <ReadLegacyUserData>d__;
			<ReadLegacyUserData>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, LegacyUserSaveObject>>.Create();
			<ReadLegacyUserData>d__.<>4__this = this;
			<ReadLegacyUserData>d__.<>1__state = -1;
			<ReadLegacyUserData>d__.<>t__builder.Start<BaseDataStorage.<ReadLegacyUserData>d__23>(ref <ReadLegacyUserData>d__);
			return <ReadLegacyUserData>d__.<>t__builder.Task;
		}

		public virtual Task<Error> WriteUserData(UserSaveObject userObject)
		{
			return this.WriteData<UserSaveObject>(userObject, this.GetUserDataFilePath(userObject.LocalUserId));
		}

		public virtual Task<Error> DeleteUserData(string localUserId)
		{
			return this.DeleteData(this.GetUserDataFilePath(localUserId));
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"results"
		})]
		public virtual Task<ValueTuple<Error, UserSaveObject[]>> ReadAllSavedUserData()
		{
			BaseDataStorage.<ReadAllSavedUserData>d__26 <ReadAllSavedUserData>d__;
			<ReadAllSavedUserData>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, UserSaveObject[]>>.Create();
			<ReadAllSavedUserData>d__.<>4__this = this;
			<ReadAllSavedUserData>d__.<>1__state = -1;
			<ReadAllSavedUserData>d__.<>t__builder.Start<BaseDataStorage.<ReadAllSavedUserData>d__26>(ref <ReadAllSavedUserData>d__);
			return <ReadAllSavedUserData>d__.<>t__builder.Task;
		}

		public virtual Error DeleteLegacyUserData()
		{
			Error result;
			try
			{
				ModioServices.Resolve<IModioRootPathProvider>();
				string text = "";
				WindowsRootPathProvider windowsRootPathProvider = new WindowsRootPathProvider();
				if (windowsRootPathProvider != null)
				{
					text = windowsRootPathProvider.LegacyUserPath;
					text = text.Replace('/', '\\');
				}
				string text2 = text;
				ModioLog verbose = ModioLog.Verbose;
				if (verbose != null)
				{
					verbose.Log("Attempting to delete legacy user data in: " + text2);
				}
				if (this.DoesDirectoryExist(text2))
				{
					Directory.Delete(text2, true);
					result = Error.None;
				}
				else
				{
					ModioLog warning = ModioLog.Warning;
					if (warning != null)
					{
						warning.Log("No legacy user data exists, nothing to delete...");
					}
					result = new Error(ErrorCode.DIRECTORY_NOT_FOUND);
				}
			}
			catch (Exception ex)
			{
				ModioLog error = ModioLog.Error;
				if (error != null)
				{
					error.Log(string.Format("Exception deleting legacy UserData: {0}", ex));
				}
				result = new Error(ErrorCode.UNKNOWN, ex.ToString());
			}
			return result;
		}

		public virtual Task<Error> DownloadModFileFromStream(long modId, long modfileId, Stream downloadStream, string md5Hash, CancellationToken token)
		{
			BaseDataStorage.<DownloadModFileFromStream>d__28 <DownloadModFileFromStream>d__;
			<DownloadModFileFromStream>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<DownloadModFileFromStream>d__.<>4__this = this;
			<DownloadModFileFromStream>d__.modId = modId;
			<DownloadModFileFromStream>d__.modfileId = modfileId;
			<DownloadModFileFromStream>d__.downloadStream = downloadStream;
			<DownloadModFileFromStream>d__.md5Hash = md5Hash;
			<DownloadModFileFromStream>d__.token = token;
			<DownloadModFileFromStream>d__.<>1__state = -1;
			<DownloadModFileFromStream>d__.<>t__builder.Start<BaseDataStorage.<DownloadModFileFromStream>d__28>(ref <DownloadModFileFromStream>d__);
			return <DownloadModFileFromStream>d__.<>t__builder.Task;
		}

		protected virtual Stream CreateFileStream(string filePath, FileMode mode)
		{
			return new FileStream(filePath, mode, (mode == FileMode.Append) ? FileAccess.Write : FileAccess.ReadWrite, FileShare.None);
		}

		public static Task<byte[]> CalculateMd5Hash(string filePath, byte[] buffer)
		{
			BaseDataStorage.<CalculateMd5Hash>d__30 <CalculateMd5Hash>d__;
			<CalculateMd5Hash>d__.<>t__builder = AsyncTaskMethodBuilder<byte[]>.Create();
			<CalculateMd5Hash>d__.filePath = filePath;
			<CalculateMd5Hash>d__.buffer = buffer;
			<CalculateMd5Hash>d__.<>1__state = -1;
			<CalculateMd5Hash>d__.<>t__builder.Start<BaseDataStorage.<CalculateMd5Hash>d__30>(ref <CalculateMd5Hash>d__);
			return <CalculateMd5Hash>d__.<>t__builder.Task;
		}

		public virtual Task<Error> DeleteModfile(long modId, long modfileId)
		{
			string modfilePath = this.GetModfilePath(modId, modfileId);
			Error error = this.DeleteFile(modfilePath);
			if (error)
			{
				ModioLog error2 = ModioLog.Error;
				if (error2 != null)
				{
					error2.Log(string.Format("Error deleting Modfile {0}: {1}\nAt: {2}", modId, error.GetMessage(), modfilePath));
				}
			}
			return Task.FromResult<Error>(error);
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"results",
			"modId",
			"modfileId"
		})]
		public virtual Task<ValueTuple<Error, List<ValueTuple<long, long>>>> ScanForModfiles()
		{
			if (!Directory.Exists(Path.Combine(this.Root, "Modfiles")))
			{
				return Task.FromResult<ValueTuple<Error, List<ValueTuple<long, long>>>>(new ValueTuple<Error, List<ValueTuple<long, long>>>(Error.None, new List<ValueTuple<long, long>>()));
			}
			Task<ValueTuple<Error, List<ValueTuple<long, long>>>> result;
			try
			{
				string[] files = Directory.GetFiles(Path.Combine(this.Root, "Modfiles"));
				List<ValueTuple<long, long>> list = new List<ValueTuple<long, long>>();
				foreach (string text in files)
				{
					if (text.Contains("_modfile"))
					{
						string fileName = Path.GetFileName(text);
						string[] array2 = fileName.Split('_', StringSplitOptions.None);
						long item;
						long item2;
						if (array2.Length == 2 && long.TryParse(array2[0], out item) && long.TryParse(array2[1], out item2))
						{
							list.Add(new ValueTuple<long, long>(item, item2));
						}
						else
						{
							ModioLog message = ModioLog.Message;
							if (message != null)
							{
								message.Log("Invalid Modfile name: [" + fileName + "], skipping");
							}
						}
					}
				}
				result = Task.FromResult<ValueTuple<Error, List<ValueTuple<long, long>>>>(new ValueTuple<Error, List<ValueTuple<long, long>>>(Error.None, list));
			}
			catch (Exception ex)
			{
				ModioLog error = ModioLog.Error;
				if (error != null)
				{
					error.Log(string.Format("Exception scanning Modfiles: {0}", ex));
				}
				result = Task.FromResult<ValueTuple<Error, List<ValueTuple<long, long>>>>(new ValueTuple<Error, List<ValueTuple<long, long>>>(new ErrorException(ex), new List<ValueTuple<long, long>>()));
			}
			return result;
		}

		public virtual Task<Error> InstallMod(Mod mod, long modfileId, CancellationToken token)
		{
			BaseDataStorage.<InstallMod>d__33 <InstallMod>d__;
			<InstallMod>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<InstallMod>d__.<>4__this = this;
			<InstallMod>d__.mod = mod;
			<InstallMod>d__.modfileId = modfileId;
			<InstallMod>d__.token = token;
			<InstallMod>d__.<>1__state = -1;
			<InstallMod>d__.<>t__builder.Start<BaseDataStorage.<InstallMod>d__33>(ref <InstallMod>d__);
			return <InstallMod>d__.<>t__builder.Task;
		}

		public virtual Task<Error> InstallModFromStream(Mod mod, long modfileId, Stream stream, string md5Hash, CancellationToken token)
		{
			BaseDataStorage.<InstallModFromStream>d__34 <InstallModFromStream>d__;
			<InstallModFromStream>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<InstallModFromStream>d__.<>4__this = this;
			<InstallModFromStream>d__.mod = mod;
			<InstallModFromStream>d__.modfileId = modfileId;
			<InstallModFromStream>d__.stream = stream;
			<InstallModFromStream>d__.md5Hash = md5Hash;
			<InstallModFromStream>d__.token = token;
			<InstallModFromStream>d__.<>1__state = -1;
			<InstallModFromStream>d__.<>t__builder.Start<BaseDataStorage.<InstallModFromStream>d__34>(ref <InstallModFromStream>d__);
			return <InstallModFromStream>d__.<>t__builder.Task;
		}

		protected virtual Error MoveTempInstallToCorrectLocation(Mod mod, string installDirectoryPath, string temporaryDirectoryPath)
		{
			if (this.DoesDirectoryExist(installDirectoryPath))
			{
				this.DeleteDirectoryAndContents(installDirectoryPath);
			}
			string directoryName = Path.GetDirectoryName(installDirectoryPath);
			if (!this.DoesDirectoryExist(directoryName))
			{
				Error error = this.CreateDirectory(directoryName);
				if (error)
				{
					if (!error.IsSilent)
					{
						ModioLog error2 = ModioLog.Error;
						if (error2 != null)
						{
							error2.Log(string.Format("Install operation for Modfile {0} aborted. Failed to create directory {1} with error {2}", mod.Id, directoryName, error));
						}
					}
					return error;
				}
			}
			Directory.Move(temporaryDirectoryPath, installDirectoryPath);
			return Error.None;
		}

		protected virtual Task<Error> ExtractFileFromZipStream(ZipInputStream zipStream, ZipEntry entry, string filePath, ModInstallProgressTracker progressTracker, CancellationToken token)
		{
			BaseDataStorage.<ExtractFileFromZipStream>d__36 <ExtractFileFromZipStream>d__;
			<ExtractFileFromZipStream>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<ExtractFileFromZipStream>d__.<>4__this = this;
			<ExtractFileFromZipStream>d__.zipStream = zipStream;
			<ExtractFileFromZipStream>d__.entry = entry;
			<ExtractFileFromZipStream>d__.filePath = filePath;
			<ExtractFileFromZipStream>d__.progressTracker = progressTracker;
			<ExtractFileFromZipStream>d__.token = token;
			<ExtractFileFromZipStream>d__.<>1__state = -1;
			<ExtractFileFromZipStream>d__.<>t__builder.Start<BaseDataStorage.<ExtractFileFromZipStream>d__36>(ref <ExtractFileFromZipStream>d__);
			return <ExtractFileFromZipStream>d__.<>t__builder.Task;
		}

		public virtual Task<Error> DeleteInstalledMod(Mod mod, long modfileId)
		{
			string installPath = this.GetInstallPath(mod.Id, modfileId);
			Error error = this.DeleteDirectoryAndContents(installPath);
			if (error)
			{
				ModioLog error2 = ModioLog.Error;
				if (error2 != null)
				{
					error2.Log(string.Format("Error deleting installed mod {0}: {1}\nAt: {2}", mod, error.GetMessage(), installPath));
				}
			}
			return Task.FromResult<Error>(error);
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"results",
			"modId",
			"modfileId"
		})]
		public virtual Task<ValueTuple<Error, List<ValueTuple<long, long>>>> ScanForInstalledMods()
		{
			Task<ValueTuple<Error, List<ValueTuple<long, long>>>> result;
			try
			{
				List<ValueTuple<long, long>> list = new List<ValueTuple<long, long>>();
				foreach (ValueTuple<Error, string> valueTuple in this.IterateDirectoriesInDirectory(Path.Combine(this.Root, "mods")))
				{
					Error item = valueTuple.Item1;
					string item2 = valueTuple.Item2;
					if (!item)
					{
						string fileName = Path.GetFileName(item2);
						string[] array = fileName.Split('_', StringSplitOptions.None);
						long item3;
						long item4;
						if (array.Length == 2 && long.TryParse(array[0], out item3) && long.TryParse(array[1], out item4))
						{
							list.Add(new ValueTuple<long, long>(item3, item4));
						}
						else
						{
							ModioLog message = ModioLog.Message;
							if (message != null)
							{
								message.Log("Invalid Install name: [" + fileName + "], skipping");
							}
						}
					}
				}
				result = Task.FromResult<ValueTuple<Error, List<ValueTuple<long, long>>>>(new ValueTuple<Error, List<ValueTuple<long, long>>>(Error.None, list));
			}
			catch (Exception ex)
			{
				ModioLog error = ModioLog.Error;
				if (error != null)
				{
					error.Log(string.Format("Exception scanning Mod Installations: {0}", ex));
				}
				result = Task.FromResult<ValueTuple<Error, List<ValueTuple<long, long>>>>(new ValueTuple<Error, List<ValueTuple<long, long>>>(new ErrorException(ex), new List<ValueTuple<long, long>>()));
			}
			return result;
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"installed",
			"modfileId"
		})]
		public virtual Task<ValueTuple<Error, bool, long>> ScanForInstalledMod(Mod mod)
		{
			Task<ValueTuple<Error, bool, long>> result;
			try
			{
				foreach (ValueTuple<Error, string> valueTuple in this.IterateDirectoriesInDirectory(Path.Combine(this.Root, "mods")))
				{
					Error item = valueTuple.Item1;
					string item2 = valueTuple.Item2;
					if (!item)
					{
						string fileName = Path.GetFileName(item2);
						string[] array = fileName.Split('_', StringSplitOptions.None);
						long id;
						long item3;
						if (array.Length == 2 && long.TryParse(array[0], out id) && long.TryParse(array[1], out item3) && id == mod.Id)
						{
							return Task.FromResult<ValueTuple<Error, bool, long>>(new ValueTuple<Error, bool, long>(Error.None, true, item3));
						}
						ModioLog message = ModioLog.Message;
						if (message != null)
						{
							message.Log("Invalid Install name: [" + fileName + "], skipping");
						}
					}
				}
				result = Task.FromResult<ValueTuple<Error, bool, long>>(new ValueTuple<Error, bool, long>(Error.None, false, -1L));
			}
			catch (Exception ex)
			{
				ModioLog error = ModioLog.Error;
				if (error != null)
				{
					error.Log(string.Format("Exception scanning Mod Installations: {0}", ex));
				}
				result = Task.FromResult<ValueTuple<Error, bool, long>>(new ValueTuple<Error, bool, long>(new ErrorException(ex), false, -1L));
			}
			return result;
		}

		protected virtual void MigrateLegacyModInstalls()
		{
			this.MigrateLegacyModInstalls(Path.Combine(this.Root, "Installed"));
			try
			{
				ModioLog verbose = ModioLog.Verbose;
				if (verbose != null)
				{
					verbose.Log("Checking for legacy mod installs... (windows only code path specific to GT)");
				}
				WindowsRootPathProvider windowsRootPathProvider = new WindowsRootPathProvider();
				if (windowsRootPathProvider != null)
				{
					string text = Path.Combine(windowsRootPathProvider.LegacyPath, "mod.io");
					string text2 = Path.Combine(text, this.GameId.ToString("00000"), string.Format("data{0}mods", Path.DirectorySeparatorChar)) + Path.DirectorySeparatorChar.ToString();
					text2 = text2.Replace('/', '\\');
					if (this.DoesDirectoryExist(text2))
					{
						ModioLog verbose2 = ModioLog.Verbose;
						if (verbose2 != null)
						{
							verbose2.Log("Migrating legacy mods from: \"" + text2 + "\"");
						}
						this.MigrateLegacyModInstalls(text2);
					}
					text += Path.DirectorySeparatorChar.ToString();
					text = text.Replace('/', '\\');
					if (this.DoesDirectoryExist(text))
					{
						ModioLog verbose3 = ModioLog.Verbose;
						if (verbose3 != null)
						{
							verbose3.Log("Deleting legacy mod.io folder at: \"" + text + "\"");
						}
						Directory.Delete(text, true);
					}
				}
			}
			catch (Exception arg)
			{
				Console.WriteLine(string.Format("[mod.io] Exception while migrating legacy data: {0}", arg));
			}
		}

		protected void MigrateLegacyModInstalls(string legacyDirectoryPath)
		{
			try
			{
				foreach (ValueTuple<Error, string> valueTuple in this.IterateDirectoriesInDirectory(legacyDirectoryPath))
				{
					Error item = valueTuple.Item1;
					string item2 = valueTuple.Item2;
					if (!item)
					{
						string fileName = Path.GetFileName(item2);
						string[] array = fileName.Split('_', StringSplitOptions.None);
						long modId;
						long modfileId;
						if (array.Length != 2 || !long.TryParse(array[0], out modId) || !long.TryParse(array[1], out modfileId))
						{
							ModioLog message = ModioLog.Message;
							if (message != null)
							{
								message.Log("Invalid Install name in legacy folder: [" + fileName + "], skipping");
							}
						}
						else
						{
							string installPath = this.GetInstallPath(modId, modfileId);
							if (Directory.Exists(installPath))
							{
								ModioLog message2 = ModioLog.Message;
								if (message2 != null)
								{
									message2.Log("Deleting redundant legacy folder: " + installPath);
								}
								Directory.Delete(item2, true);
							}
							else
							{
								ModioLog message3 = ModioLog.Message;
								if (message3 != null)
								{
									message3.Log("Moving legacy folder: " + item2 + " to " + installPath);
								}
								this.CreateDirectory(Path.GetFullPath(Path.Combine(installPath, "..") + Path.DirectorySeparatorChar.ToString()));
								Directory.Move(item2, installPath);
							}
						}
					}
				}
			}
			catch (Exception arg)
			{
				ModioLog error = ModioLog.Error;
				if (error != null)
				{
					error.Log(string.Format("Exception scanning legacy Mod Installations: {0}", arg));
				}
			}
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"result"
		})]
		public virtual Task<ValueTuple<Error, byte[]>> ReadCachedImage(Uri serverPath)
		{
			BaseDataStorage.<ReadCachedImage>d__42 <ReadCachedImage>d__;
			<ReadCachedImage>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, byte[]>>.Create();
			<ReadCachedImage>d__.<>4__this = this;
			<ReadCachedImage>d__.serverPath = serverPath;
			<ReadCachedImage>d__.<>1__state = -1;
			<ReadCachedImage>d__.<>t__builder.Start<BaseDataStorage.<ReadCachedImage>d__42>(ref <ReadCachedImage>d__);
			return <ReadCachedImage>d__.<>t__builder.Task;
		}

		public virtual Task<Error> WriteCachedImage(Uri serverPath, byte[] data)
		{
			BaseDataStorage.<WriteCachedImage>d__43 <WriteCachedImage>d__;
			<WriteCachedImage>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<WriteCachedImage>d__.<>4__this = this;
			<WriteCachedImage>d__.serverPath = serverPath;
			<WriteCachedImage>d__.data = data;
			<WriteCachedImage>d__.<>1__state = -1;
			<WriteCachedImage>d__.<>t__builder.Start<BaseDataStorage.<WriteCachedImage>d__43>(ref <WriteCachedImage>d__);
			return <WriteCachedImage>d__.<>t__builder.Task;
		}

		public virtual Task<Error> DeleteCachedImage(Uri serverPath)
		{
			if (serverPath == null)
			{
				return Task.FromResult<Error>(new Error(ErrorCode.BAD_PARAMETER));
			}
			string imageDataFilePath = this.GetImageDataFilePath(serverPath);
			Error error = this.DeleteFile(imageDataFilePath);
			if (error)
			{
				ModioLog warning = ModioLog.Warning;
				if (warning != null)
				{
					warning.Log("Error deleting image: " + error.GetMessage() + "\nAt: " + imageDataFilePath);
				}
			}
			return Task.FromResult<Error>(error);
		}

		public virtual Task<bool> IsThereAvailableFreeSpaceFor(long tempBytes, long persistentBytes)
		{
			return Task.FromResult<bool>(this.IsThereEnoughDiskSpaceFor(tempBytes + persistentBytes));
		}

		public virtual Task<bool> IsThereAvailableFreeSpaceForModfile(long bytes)
		{
			return Task.FromResult<bool>(this.IsThereEnoughDiskSpaceFor(bytes));
		}

		public virtual Task<long> GetAvailableFreeSpaceForModfile()
		{
			return Task.FromResult<long>(this.GetAvailableFreeSpace());
		}

		public virtual Task<bool> IsThereAvailableFreeSpaceForModInstall(long bytes)
		{
			return Task.FromResult<bool>(this.IsThereEnoughDiskSpaceFor(bytes));
		}

		public virtual Task<long> GetAvailableFreeSpaceForModInstall()
		{
			return Task.FromResult<long>(this.GetAvailableFreeSpace());
		}

		protected virtual Task<bool> IsThereEnoughSpaceForExtracting(string archiveFilePath)
		{
			BaseDataStorage.<IsThereEnoughSpaceForExtracting>d__50 <IsThereEnoughSpaceForExtracting>d__;
			<IsThereEnoughSpaceForExtracting>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<IsThereEnoughSpaceForExtracting>d__.<>4__this = this;
			<IsThereEnoughSpaceForExtracting>d__.archiveFilePath = archiveFilePath;
			<IsThereEnoughSpaceForExtracting>d__.<>1__state = -1;
			<IsThereEnoughSpaceForExtracting>d__.<>t__builder.Start<BaseDataStorage.<IsThereEnoughSpaceForExtracting>d__50>(ref <IsThereEnoughSpaceForExtracting>d__);
			return <IsThereEnoughSpaceForExtracting>d__.<>t__builder.Task;
		}

		protected virtual bool IsThereEnoughDiskSpaceFor(long bytes)
		{
			long availableFreeSpace = this.GetAvailableFreeSpace();
			return availableFreeSpace <= 0L || bytes < availableFreeSpace;
		}

		protected virtual long GetAvailableFreeSpace()
		{
			ModioDiskTestSettings modioDiskTestSettings;
			if (ModioClient.Settings.TryGetPlatformSettings<ModioDiskTestSettings>(out modioDiskTestSettings) && modioDiskTestSettings.OverrideDiskSpaceRemaining)
			{
				return (long)modioDiskTestSettings.BytesRemaining;
			}
			if (!this.Initialized)
			{
				return 0L;
			}
			return new DriveInfo(Path.GetPathRoot(this.Root)).AvailableFreeSpace;
		}

		protected virtual bool IsValidPath(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				return false;
			}
			bool result;
			try
			{
				string pathRoot = Path.GetPathRoot(Path.GetFullPath(filePath));
				result = (pathRoot == "/" || !string.IsNullOrEmpty(pathRoot.Trim(new char[]
				{
					'\\',
					'/'
				})));
			}
			catch
			{
				result = false;
			}
			return result;
		}

		protected virtual bool DoesDirectoryExist(string filePath)
		{
			return Directory.Exists(filePath);
		}

		protected virtual bool DoesFileExist(string filePath)
		{
			return File.Exists(filePath);
		}

		protected virtual Error CreateDirectory(string filePath)
		{
			if (!this.IsValidPath(filePath))
			{
				return new Error(ErrorCode.BAD_PARAMETER);
			}
			string directoryName = Path.GetDirectoryName(filePath);
			if (string.IsNullOrEmpty(directoryName))
			{
				return new Error(ErrorCode.BAD_PARAMETER);
			}
			try
			{
				Directory.CreateDirectory(directoryName);
			}
			catch (Exception exception)
			{
				return new ErrorException(exception);
			}
			return Error.None;
		}

		protected virtual Error DeleteDirectoryAndContents(string filePath)
		{
			if (!this.IsValidPath(filePath))
			{
				return new Error(ErrorCode.BAD_PARAMETER);
			}
			if (!this.DoesDirectoryExist(filePath))
			{
				return Error.None;
			}
			try
			{
				Directory.Delete(filePath, true);
			}
			catch (Exception exception)
			{
				return new ErrorException(exception);
			}
			return Error.None;
		}

		protected virtual Error DeleteFile(string filePath)
		{
			if (!this.IsValidPath(filePath))
			{
				return new Error(ErrorCode.BAD_PARAMETER);
			}
			if (!this.DoesFileExist(filePath))
			{
				return Error.None;
			}
			try
			{
				File.Delete(filePath);
			}
			catch (Exception exception)
			{
				return new ErrorException(exception);
			}
			return Error.None;
		}

		protected virtual Task<Error> WriteFile(string path, byte[] data, int bytesToWrite)
		{
			BaseDataStorage.<WriteFile>d__59 <WriteFile>d__;
			<WriteFile>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<WriteFile>d__.<>4__this = this;
			<WriteFile>d__.path = path;
			<WriteFile>d__.data = data;
			<WriteFile>d__.bytesToWrite = bytesToWrite;
			<WriteFile>d__.<>1__state = -1;
			<WriteFile>d__.<>t__builder.Start<BaseDataStorage.<WriteFile>d__59>(ref <WriteFile>d__);
			return <WriteFile>d__.<>t__builder.Task;
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"result"
		})]
		protected virtual Task<ValueTuple<Error, byte[]>> ReadFile(string path)
		{
			BaseDataStorage.<ReadFile>d__60 <ReadFile>d__;
			<ReadFile>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, byte[]>>.Create();
			<ReadFile>d__.<>4__this = this;
			<ReadFile>d__.path = path;
			<ReadFile>d__.<>1__state = -1;
			<ReadFile>d__.<>t__builder.Start<BaseDataStorage.<ReadFile>d__60>(ref <ReadFile>d__);
			return <ReadFile>d__.<>t__builder.Task;
		}

		protected virtual Task<Error> WriteTextFile(string path, string data)
		{
			BaseDataStorage.<WriteTextFile>d__61 <WriteTextFile>d__;
			<WriteTextFile>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<WriteTextFile>d__.<>4__this = this;
			<WriteTextFile>d__.path = path;
			<WriteTextFile>d__.data = data;
			<WriteTextFile>d__.<>1__state = -1;
			<WriteTextFile>d__.<>t__builder.Start<BaseDataStorage.<WriteTextFile>d__61>(ref <WriteTextFile>d__);
			return <WriteTextFile>d__.<>t__builder.Task;
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"result"
		})]
		protected virtual Task<ValueTuple<Error, string>> ReadTextFile(string path)
		{
			BaseDataStorage.<ReadTextFile>d__62 <ReadTextFile>d__;
			<ReadTextFile>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, string>>.Create();
			<ReadTextFile>d__.<>4__this = this;
			<ReadTextFile>d__.path = path;
			<ReadTextFile>d__.<>1__state = -1;
			<ReadTextFile>d__.<>t__builder.Start<BaseDataStorage.<ReadTextFile>d__62>(ref <ReadTextFile>d__);
			return <ReadTextFile>d__.<>t__builder.Task;
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"result"
		})]
		protected virtual ValueTuple<Error, string> TryParseUTF8Data(byte[] data)
		{
			if (data == null)
			{
				return new ValueTuple<Error, string>(new Error(ErrorCode.BAD_PARAMETER), string.Empty);
			}
			ValueTuple<Error, string> result;
			try
			{
				string @string = Encoding.UTF8.GetString(data);
				result = new ValueTuple<Error, string>(Error.None, @string);
			}
			catch (Exception ex)
			{
				ModioLog error = ModioLog.Error;
				if (error != null)
				{
					error.Log(string.Format("Exception parsing bytes to string: {0}", ex));
				}
				result = new ValueTuple<Error, string>(new ErrorException(ex), string.Empty);
			}
			return result;
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"result"
		})]
		protected virtual ValueTuple<Error, byte[]> ConvertUTF8Data(string data)
		{
			if (string.IsNullOrEmpty(data))
			{
				return new ValueTuple<Error, byte[]>(new Error(ErrorCode.BAD_PARAMETER), Array.Empty<byte>());
			}
			ValueTuple<Error, byte[]> result;
			try
			{
				byte[] bytes = Encoding.UTF8.GetBytes(data);
				result = new ValueTuple<Error, byte[]>(Error.None, bytes);
			}
			catch (Exception arg)
			{
				ModioLog error = ModioLog.Error;
				if (error != null)
				{
					error.Log(string.Format("Exception parsing string to bytes: {0}", arg));
				}
				result = new ValueTuple<Error, byte[]>(new Error(ErrorCode.BAD_PARAMETER), Array.Empty<byte>());
			}
			return result;
		}

		public virtual string GetModfilePath(long modId, long modfileId)
		{
			return Path.Combine(this.Root, "Modfiles", string.Format("{0}_{1}_modfile.zip", modId, modfileId));
		}

		public virtual string GetInstallPath(long modId, long modfileId)
		{
			return string.Format("{0}{1}", Path.Combine(this.Root, "mods", string.Format("{0}_{1}", modId, modfileId)), Path.DirectorySeparatorChar);
		}

		protected virtual string GetTemporaryInstallPath(long modId, long modfileId)
		{
			return string.Format("{0}{1}", Path.Combine(this.Root, "Temp", string.Format("{0}_{1}", modId, modfileId)), Path.DirectorySeparatorChar);
		}

		protected virtual string GetGameDataFilePath()
		{
			return Path.Combine(this.Root, string.Format("{0}_game_data.json", this.GameId));
		}

		protected virtual string GetIndexFilePath()
		{
			return Path.Combine(this.Root, string.Format("{0}_mod_index.json", this.GameId));
		}

		protected virtual string GetUserDataFilePath(string localUserId)
		{
			return Path.Combine(this.UserRoot, localUserId + "_user_data.json");
		}

		protected virtual string GetImageDataFilePath(Uri serverPath)
		{
			return Path.Combine(this.Root, "ImageCache" + serverPath.LocalPath);
		}

		public virtual bool DoesModfileExist(long modId, long modfileId)
		{
			string modfilePath = this.GetModfilePath(modId, modfileId);
			return this.DoesFileExist(modfilePath);
		}

		public virtual bool DoesInstallExist(long modId, long modfileId)
		{
			string installPath = this.GetInstallPath(modId, modfileId);
			return this.DoesDirectoryExist(installPath);
		}

		public virtual Task<Error> CompressToZip(string filePath, Stream outputTo)
		{
			BaseDataStorage.<CompressToZip>d__74 <CompressToZip>d__;
			<CompressToZip>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<CompressToZip>d__.<>4__this = this;
			<CompressToZip>d__.filePath = filePath;
			<CompressToZip>d__.outputTo = outputTo;
			<CompressToZip>d__.<>1__state = -1;
			<CompressToZip>d__.<>t__builder.Start<BaseDataStorage.<CompressToZip>d__74>(ref <CompressToZip>d__);
			return <CompressToZip>d__.<>t__builder.Task;
		}

		protected virtual Task CompressStream(string entryName, Stream stream, ZipOutputStream zipStream)
		{
			BaseDataStorage.<CompressStream>d__75 <CompressStream>d__;
			<CompressStream>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<CompressStream>d__.entryName = entryName;
			<CompressStream>d__.stream = stream;
			<CompressStream>d__.zipStream = zipStream;
			<CompressStream>d__.<>1__state = -1;
			<CompressStream>d__.<>t__builder.Start<BaseDataStorage.<CompressStream>d__75>(ref <CompressStream>d__);
			return <CompressStream>d__.<>t__builder.Task;
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"fileName"
		})]
		protected virtual IEnumerable<ValueTuple<Error, string>> IterateFilesInDirectory(string directoryPath)
		{
			if (!this.DoesDirectoryExist(directoryPath))
			{
				if (this.DoesFileExist(directoryPath))
				{
					yield return new ValueTuple<Error, string>(Error.None, directoryPath);
				}
				else
				{
					yield return new ValueTuple<Error, string>(new Error(ErrorCode.FILE_NOT_FOUND), null);
				}
				yield break;
			}
			foreach (string item in Directory.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories))
			{
				yield return new ValueTuple<Error, string>(Error.None, item);
			}
			IEnumerator<string> enumerator = null;
			yield break;
			yield break;
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"directoryPath"
		})]
		protected virtual IEnumerable<ValueTuple<Error, string>> IterateDirectoriesInDirectory(string directoryPath)
		{
			if (!this.DoesDirectoryExist(directoryPath))
			{
				yield return new ValueTuple<Error, string>(new Error(ErrorCode.FILE_NOT_FOUND), null);
				yield break;
			}
			foreach (string item in Directory.EnumerateDirectories(directoryPath))
			{
				yield return new ValueTuple<Error, string>(Error.None, item);
			}
			IEnumerator<string> enumerator = null;
			yield break;
			yield break;
		}

		[CompilerGenerated]
		private Error <InstallModFromStream>g__LogTaskCancelAndCleanup|34_0(ref BaseDataStorage.<>c__DisplayClass34_0 A_1)
		{
			A_1.error = new Error(this.IsShuttingDown ? ErrorCode.SHUTTING_DOWN : ErrorCode.OPERATION_CANCELLED);
			ModioLog verbose = ModioLog.Verbose;
			if (verbose != null)
			{
				verbose.Log(string.Concat(new string[]
				{
					"Cancelled installing mod: \nInstall Path: ",
					A_1.installDirectoryPath,
					"\nTemp Path: ",
					A_1.temporaryDirectoryPath,
					"\n"
				}));
			}
			Error error = this.DeleteDirectoryAndContents(A_1.temporaryDirectoryPath);
			if (error)
			{
				ModioLog message = ModioLog.Message;
				if (message != null)
				{
					message.Log("Error cleaning up temporary download location: " + error.GetMessage() + "\nAt: " + A_1.temporaryDirectoryPath);
				}
			}
			return A_1.error;
		}

		protected bool Initialized;

		protected bool IsShuttingDown;

		protected long GameId;

		protected string Root;

		protected string UserRoot;

		protected int OngoingTaskCount;

		protected CancellationTokenSource ShutdownTokenSource;

		protected CancellationToken ShutdownToken;
	}
}
