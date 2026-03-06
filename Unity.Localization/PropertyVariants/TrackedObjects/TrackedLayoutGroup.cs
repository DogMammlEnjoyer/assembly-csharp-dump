using System;
using UnityEngine.UI;

namespace UnityEngine.Localization.PropertyVariants.TrackedObjects
{
	[DisplayName("Layout Group", null)]
	[CustomTrackedObject(typeof(LayoutGroup), true)]
	[Serializable]
	public class TrackedLayoutGroup : JsonSerializerTrackedObject
	{
		protected override void PostApplyTrackedProperties()
		{
			LayoutGroup layoutGroup = base.Target as LayoutGroup;
			if (layoutGroup != null)
			{
				RectTransform rectTransform = layoutGroup.transform as RectTransform;
				if (rectTransform != null)
				{
					LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
				}
			}
			base.PostApplyTrackedProperties();
		}
	}
}
