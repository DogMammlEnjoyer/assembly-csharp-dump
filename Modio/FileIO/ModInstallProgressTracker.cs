using System;
using Modio.Mods;

namespace Modio.FileIO
{
	public class ModInstallProgressTracker
	{
		public ModInstallProgressTracker(Mod mod, long totalSize, Func<long> currentBytesGetter = null)
		{
			this._totalSize = totalSize;
			this._currentBytesGetter = currentBytesGetter;
			this._mod = mod;
		}

		public Func<long> CurrentBytesGetter
		{
			get
			{
				return this._currentBytesGetter;
			}
			set
			{
				this._currentBytesGetter = value;
			}
		}

		public void Update()
		{
			if (this._currentBytesGetter != null)
			{
				this.SetBytesRead(this._currentBytesGetter());
			}
		}

		public void SetBytesRead(long currentBytes)
		{
			DateTime now = DateTime.Now;
			float num = (float)(now - this._lastCalculatedAt).TotalMilliseconds / 1000f;
			if (num > 1f || this._lastCalculatedSpeedAtBytes == 0L)
			{
				this._bytesPerSecond = (long)((float)(currentBytes - this._lastCalculatedSpeedAtBytes) / num);
				if (num > 1f)
				{
					this._lastCalculatedAt = now;
					this._lastCalculatedSpeedAtBytes = currentBytes;
				}
			}
			float fileStateProgress = 0.99f * (float)currentBytes / (float)this._totalSize;
			this._mod.File.FileStateProgress = fileStateProgress;
			this._mod.File.DownloadingBytesPerSecond = this._bytesPerSecond;
			this._mod.InvokeModUpdated(ModChangeType.DownloadProgress);
		}

		private readonly Mod _mod;

		private readonly long _totalSize;

		private Func<long> _currentBytesGetter;

		private DateTime _lastCalculatedAt;

		private long _bytesPerSecond;

		private long _lastCalculatedSpeedAtBytes;
	}
}
