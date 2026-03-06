using System;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.Localization.PropertyVariants.TrackedObjects
{
	[DisplayName("Rect Transform", null)]
	[CustomTrackedObject(typeof(RectTransform), false)]
	[Serializable]
	public class TrackedRectTransform : TrackedTransform
	{
		protected override void AddPropertyHandlers(Dictionary<string, Action<float>> handlers)
		{
			base.AddPropertyHandlers(handlers);
			handlers["m_AnchoredPosition.x"] = delegate(float val)
			{
				this.m_AnchorPosToApply.x = val;
			};
			handlers["m_AnchoredPosition.y"] = delegate(float val)
			{
				this.m_AnchorPosToApply.y = val;
			};
			handlers["m_AnchoredPosition.z"] = delegate(float val)
			{
				this.m_AnchorPosToApply.z = val;
			};
			handlers["m_AnchorMin.x"] = delegate(float val)
			{
				this.m_AnchorMinToApply.x = val;
			};
			handlers["m_AnchorMin.y"] = delegate(float val)
			{
				this.m_AnchorMinToApply.y = val;
			};
			handlers["m_AnchorMax.x"] = delegate(float val)
			{
				this.m_AnchorMaxToApply.x = val;
			};
			handlers["m_AnchorMax.y"] = delegate(float val)
			{
				this.m_AnchorMaxToApply.y = val;
			};
			handlers["m_SizeDelta.x"] = delegate(float val)
			{
				this.m_SizeDeltaToApply.x = val;
			};
			handlers["m_SizeDelta.y"] = delegate(float val)
			{
				this.m_SizeDeltaToApply.y = val;
			};
			handlers["m_Pivot.x"] = delegate(float val)
			{
				this.m_PivotToApply.x = val;
			};
			handlers["m_Pivot.y"] = delegate(float val)
			{
				this.m_PivotToApply.y = val;
			};
		}

		public override AsyncOperationHandle ApplyLocale(Locale variantLocale, Locale defaultLocale)
		{
			RectTransform rectTransform = (RectTransform)base.Target;
			this.m_AnchorPosToApply = rectTransform.anchoredPosition3D;
			this.m_AnchorMinToApply = rectTransform.anchorMin;
			this.m_AnchorMaxToApply = rectTransform.anchorMax;
			this.m_PivotToApply = rectTransform.pivot;
			this.m_SizeDeltaToApply = rectTransform.sizeDelta;
			base.ApplyLocale(variantLocale, defaultLocale);
			rectTransform.anchoredPosition3D = this.m_AnchorPosToApply;
			rectTransform.anchorMin = this.m_AnchorMinToApply;
			rectTransform.anchorMax = this.m_AnchorMaxToApply;
			rectTransform.pivot = this.m_PivotToApply;
			rectTransform.sizeDelta = this.m_SizeDeltaToApply;
			return default(AsyncOperationHandle);
		}

		private Vector3 m_AnchorPosToApply;

		private Vector2 m_AnchorMinToApply;

		private Vector2 m_AnchorMaxToApply;

		private Vector2 m_PivotToApply;

		private Vector2 m_SizeDeltaToApply;
	}
}
