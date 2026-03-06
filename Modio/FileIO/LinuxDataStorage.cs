using System;
using System.Runtime.InteropServices;

namespace Modio.FileIO
{
	public class LinuxDataStorage : BaseDataStorage
	{
		protected override long GetAvailableFreeSpace()
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
			LinuxDataStorage.UnixStatsFs unixStatsFs = default(LinuxDataStorage.UnixStatsFs);
			LinuxDataStorage.statvfs(this.Root, out unixStatsFs);
			ulong num = (unixStatsFs.f_frsize > 0UL) ? unixStatsFs.f_frsize : unixStatsFs.f_bsize;
			return (long)(unixStatsFs.f_bavail * num);
		}

		[DllImport("libc", CharSet = CharSet.Ansi, SetLastError = true)]
		private static extern short statvfs(string directory, out LinuxDataStorage.UnixStatsFs statsFs);

		private struct UnixStatsFs
		{
			public ulong f_bsize;

			public ulong f_frsize;

			public ulong f_blocks;

			public ulong f_bfree;

			public ulong f_bavail;

			public ulong f_files;

			public ulong f_ffre;

			public ulong f_favail;

			public ulong f_fsid;

			public ulong f_flag;

			public ulong f_namemax;
		}
	}
}
