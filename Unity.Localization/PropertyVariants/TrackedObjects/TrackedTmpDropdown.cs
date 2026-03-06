using System;
using TMPro;

namespace UnityEngine.Localization.PropertyVariants.TrackedObjects
{
	[DisplayName("TMP Dropdown", null)]
	[CustomTrackedObject(typeof(TMP_Dropdown), true)]
	[Serializable]
	public class TrackedTmpDropdown : JsonSerializerTrackedObject
	{
		protected override void PostApplyTrackedProperties()
		{
			((TMP_Dropdown)base.Target).RefreshShownValue();
			base.PostApplyTrackedProperties();
		}
	}
}
