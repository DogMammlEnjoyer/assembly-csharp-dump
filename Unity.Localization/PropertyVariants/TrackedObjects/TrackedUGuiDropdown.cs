using System;
using UnityEngine.UI;

namespace UnityEngine.Localization.PropertyVariants.TrackedObjects
{
	[DisplayName("UI Dropdown", null)]
	[CustomTrackedObject(typeof(Dropdown), true)]
	[Serializable]
	public class TrackedUGuiDropdown : JsonSerializerTrackedObject
	{
		protected override void PostApplyTrackedProperties()
		{
			((Dropdown)base.Target).RefreshShownValue();
			base.PostApplyTrackedProperties();
		}
	}
}
