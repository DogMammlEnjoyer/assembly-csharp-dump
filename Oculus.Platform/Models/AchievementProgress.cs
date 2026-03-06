using System;

namespace Oculus.Platform.Models
{
	public class AchievementProgress
	{
		public AchievementProgress(IntPtr o)
		{
			this.Bitfield = CAPI.ovr_AchievementProgress_GetBitfield(o);
			this.Count = CAPI.ovr_AchievementProgress_GetCount(o);
			this.IsUnlocked = CAPI.ovr_AchievementProgress_GetIsUnlocked(o);
			this.Name = CAPI.ovr_AchievementProgress_GetName(o);
			this.UnlockTime = CAPI.ovr_AchievementProgress_GetUnlockTime(o);
		}

		public readonly string Bitfield;

		public readonly ulong Count;

		public readonly bool IsUnlocked;

		public readonly string Name;

		public readonly DateTime UnlockTime;
	}
}
