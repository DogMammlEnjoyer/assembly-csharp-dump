using System;

namespace Oculus.Platform.Models
{
	public class AchievementDefinition
	{
		public AchievementDefinition(IntPtr o)
		{
			this.Type = CAPI.ovr_AchievementDefinition_GetType(o);
			this.Name = CAPI.ovr_AchievementDefinition_GetName(o);
			this.BitfieldLength = CAPI.ovr_AchievementDefinition_GetBitfieldLength(o);
			this.Target = CAPI.ovr_AchievementDefinition_GetTarget(o);
		}

		public readonly AchievementType Type;

		public readonly string Name;

		public readonly uint BitfieldLength;

		public readonly ulong Target;
	}
}
