using System;
using UnityEngine.UI;

namespace UnityEngine.Localization.PropertyVariants.TrackedObjects
{
	[DisplayName("UI Graphic", null)]
	[CustomTrackedObject(typeof(Graphic), true)]
	[Serializable]
	public class TrackedUGuiGraphic : JsonSerializerTrackedObject
	{
		protected override void PostApplyTrackedProperties()
		{
			((Graphic)base.Target).SetAllDirty();
			base.PostApplyTrackedProperties();
		}
	}
}
