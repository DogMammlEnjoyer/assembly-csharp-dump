using System;
using System.Collections.Generic;

namespace Oculus.Platform.Models
{
	public class AchievementDefinitionList : DeserializableList<AchievementDefinition>
	{
		public AchievementDefinitionList(IntPtr a)
		{
			int num = (int)((uint)CAPI.ovr_AchievementDefinitionArray_GetSize(a));
			this._Data = new List<AchievementDefinition>(num);
			for (int i = 0; i < num; i++)
			{
				this._Data.Add(new AchievementDefinition(CAPI.ovr_AchievementDefinitionArray_GetElement(a, (UIntPtr)((ulong)((long)i)))));
			}
			this._NextUrl = CAPI.ovr_AchievementDefinitionArray_GetNextUrl(a);
		}
	}
}
