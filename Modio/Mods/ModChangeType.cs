using System;

namespace Modio.Mods
{
	[Flags]
	public enum ModChangeType
	{
		Modfile = 1,
		IsEnabled = 2,
		IsSubscribed = 4,
		ModObject = 8,
		DownloadProgress = 16,
		FileState = 32,
		Rating = 64,
		IsPurchased = 128,
		Generic = 256,
		Dependencies = 512,
		Everything = -1
	}
}
