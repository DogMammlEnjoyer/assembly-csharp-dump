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
			long currentStorageThreshold = this.GetCurrentStorageThreshold();
			if (this._freeSpace < currentStorageThreshold)
			{
				this._eventBus.Trigger<LckEvents.LowStorageSpaceDetectedEvent>(new LckEvents.LowStorageSpaceDetectedEvent(LckResult.NewSuccess()));
			}
		}

		private long GetCurrentStorageThreshold()
		{
			if (!this._isRecordingActive)
			{
				return 524288000L;
			}
			return this.CalculateEstimatedRecordingSize() + 52428800L;
		}

		private long CalculateEstimatedRecordingSize()
		{
			Func<float> getDurationSeconds = this._getDurationSeconds;
			float num = (getDurationSeconds != null) ? getDurationSeconds() : 0f;
			if (num <= 0f)
			{
				return 0L;
			}
			uint bitrate = this._recordingDescriptor.Bitrate;
			uint audioBitrate = this._recordingDescriptor.AudioBitrate;
			return (long)((bitrate + audioBitrate) * num / 8f);
		}

		public void SetRecordingContext(CameraTrackDescriptor descriptor, Func<float> getDurationSeconds)
		{
			this._recordingDescriptor = descriptor;
			this._getDurationSeconds = getDurationSeconds;
			this._isRecordingActive = true;
		}

		public void ClearRecordingContext()
		{
			this._isRecordingActive = false;
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
					LckLog.LogError("Failed to get Windows storage space: " + Marshal.GetLastWin32Error().ToString(), "GetWindowsAvailableStorageSpace", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Components\\LckStorageWatcher.cs", 146);
					result = -1L;
				}
			}
			catch (Exception ex)
			{
				LckLog.LogError("Failed to get Windows storage space: " + ex.Message, "GetWindowsAvailableStorageSpace", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Components\\LckStorageWatcher.cs", 152);
				result = -1L;
			}
			return result;
		}

		public bool HasEnoughFreeStorage()
		{
			return this._freeSpace > this.GetCurrentStorageThreshold();
		}

		public void Dispose()
		{
			LckMonoBehaviourMediator.StopCoroutineByName("LckStorageWatcher:Update");
		}

		private readonly ILckEventBus _eventBus;

		private const long DefaultStorageThreshold = 524288000L;

		private const long SafetyBufferBytes = 52428800L;

		private const float PollIntervalInSeconds = 5f;

		private long _freeSpace = long.MaxValue;

		private bool _isRecordingActive;

		private CameraTrackDescriptor _recordingDescriptor;

		private Func<float> _getDurationSeconds;
	}
}
