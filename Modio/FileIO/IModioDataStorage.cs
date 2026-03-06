using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Modio.Mods;
using Modio.Users;

namespace Modio.FileIO
{
	public interface IModioDataStorage
	{
		Task<Error> Init();

		Task Shutdown();

		Task<Error> DeleteAllGameData();

		[return: TupleElementNames(new string[]
		{
			"error",
			"result"
		})]
		Task<ValueTuple<Error, GameData>> ReadGameData();

		Task<Error> WriteGameData(GameData gameData);

		Task<Error> DeleteGameData();

		[return: TupleElementNames(new string[]
		{
			"error",
			"index"
		})]
		Task<ValueTuple<Error, ModIndex>> ReadIndexData();

		Task<Error> WriteIndexData(ModIndex index);

		Task<Error> DeleteIndexData();

		[return: TupleElementNames(new string[]
		{
			"error",
			"results"
		})]
		Task<ValueTuple<Error, UserSaveObject[]>> ReadAllSavedUserData();

		[return: TupleElementNames(new string[]
		{
			"error",
			"result"
		})]
		Task<ValueTuple<Error, UserSaveObject>> ReadUserData(string localUserId);

		[return: TupleElementNames(new string[]
		{
			"error",
			"result"
		})]
		Task<ValueTuple<Error, LegacyUserSaveObject>> ReadLegacyUserData(string localUserId);

		Task<Error> WriteUserData(UserSaveObject userObject);

		Task<Error> DeleteUserData(string localUserId);

		Error DeleteLegacyUserData();

		Task<Error> DownloadModFileFromStream(long modId, long modfileId, Stream downloadStream, string md5Hash, CancellationToken token);

		Task<Error> DeleteModfile(long modId, long modfileId);

		[return: TupleElementNames(new string[]
		{
			"error",
			"results",
			"modId",
			"modfileId"
		})]
		Task<ValueTuple<Error, List<ValueTuple<long, long>>>> ScanForModfiles();

		Task<Error> InstallMod(Mod mod, long modfileId, CancellationToken token);

		Task<Error> InstallModFromStream(Mod mod, long modfileId, Stream stream, string md5Hash, CancellationToken token);

		Task<Error> DeleteInstalledMod(Mod mod, long modfileId);

		[return: TupleElementNames(new string[]
		{
			"error",
			"results",
			"modId",
			"modfileId"
		})]
		Task<ValueTuple<Error, List<ValueTuple<long, long>>>> ScanForInstalledMods();

		[return: TupleElementNames(new string[]
		{
			"error",
			"installed",
			"modfileId"
		})]
		Task<ValueTuple<Error, bool, long>> ScanForInstalledMod(Mod mod);

		[return: TupleElementNames(new string[]
		{
			"error",
			"result"
		})]
		Task<ValueTuple<Error, byte[]>> ReadCachedImage(Uri serverPath);

		Task<Error> WriteCachedImage(Uri serverPath, byte[] data);

		Task<Error> DeleteCachedImage(Uri serverPath);

		Task<bool> IsThereAvailableFreeSpaceFor(long tempBytes, long persistentBytes);

		Task<bool> IsThereAvailableFreeSpaceForModfile(long bytes);

		Task<long> GetAvailableFreeSpaceForModfile();

		Task<bool> IsThereAvailableFreeSpaceForModInstall(long bytes);

		Task<long> GetAvailableFreeSpaceForModInstall();

		string GetModfilePath(long modId, long modfileId);

		string GetInstallPath(long modId, long modfileId);

		bool DoesModfileExist(long modId, long modfileId);

		bool DoesInstallExist(long modId, long modfileId);

		Task<Error> CompressToZip(string filePath, Stream outputTo);
	}
}
