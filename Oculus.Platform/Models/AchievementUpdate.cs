using System;

namespace Oculus.Platform.Models
{
	public class AchievementUpdate
	{
		public AchievementUpdate(IntPtr o)
		{
			this.JustUnlocked = CAPI.ovr_AchievementUpdate_GetJustUnlocked(o);
			this.Name = CAPI.ovr_AchievementUpdate_GetName(o);
		}

		public readonly bool JustUnlocked;

		public readonly string Name;
	}
}
