using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace Liv.Lck
{
	internal class LckStorageWatcher : ILckStorageWatcher, IDisposable
	{
		[Preserve]
		public LckStorageWatcher(ILckEventBus eventBus)
		{
			this._eventBus = eventBus;
			LckMonoBehaviourMediator.StartCoroutine("LckStorageWatcher:Update", this.Update());
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool GetDiskFreeSpaceEx(string lpDirectoryName, out ulong lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);

		private IEnumerator Update()
		{
			for (;;)
			{
				yield return new WaitForSeconds(5f);
				this.CheckStorageSpace();
			}
			yield break;
		}

		private void CheckStorageSpace()
		{
			this._freeSpace = this.GetAvailableStorageSpace();
			if (this._freeSpace < 524288000L)
			{
				this._eventBus.Trigger<LckEvents.LowStorageSpaceDetectedEvent>(new LckEvents.LowStorageSpaceDetectedEvent(LckResult.NewSuccess()));
			}
		}

		private long GetAvailableStorageSpace()
		{
			return this.GetWindowsAvailableStorageSpace();
		}

		public long GetWindowsAvailableStorageSpace()
		{
			long result;
			try
			{
				ulong num;
				ulong num2;
				ulong num3;
				if (LckStorageWatcher.GetDiskFreeSpaceEx(Path.GetPathRoot(Application.temporaryCachePath), out num, out num2, out num3))
				{
					result = (long)num;
				}
				else
				{
					LckLog.LogError("Failed to get Windows storage space: " + Marshal.GetLastWin32Error().ToString());
					result = -1L;
				}
			}
			catch (Exception ex)
			{
				LckLog.LogError("Failed to get Windows storage space: " + ex.Message);
				result = -1L;
			}
			return result;
		}

		public bool HasEnoughFreeStorage()
		{
			return this._freeSpace > 524288000L;
		}

		public void Dispose()
		{
			LckMonoBehaviourMediator.StopCoroutineByName("LckStorageWatcher:Update");
		}

		private readonly ILckEventBus _eventBus;

		private const long StorageThreshold = 524288000L;

		private const float PollIntervalInSeconds = 5f;

		private long _freeSpace = long.MaxValue;
	}
}
