using System;
using UnityEngine;
using UnityEngine.UI;

namespace Modio.Unity.UI.Panels
{
	public class ModioPopupPositioning : MonoBehaviour, ILayoutSelfController, ILayoutController
	{
		public void PositionNextTo(RectTransform target)
		{
			this._target = target;
			LayoutRebuilder.MarkLayoutForRebuild(base.transform as RectTransform);
		}

		public void SetLayoutHorizontal()
		{
			this.SetLayout(RectTransform.Axis.Horizontal);
		}

		public void SetLayoutVertical()
		{
			this.SetLayout(RectTransform.Axis.Vertical);
		}

		private void SetLayout(RectTransform.Axis axis)
		{
			RectTransform rectTransform = (RectTransform)base.transform;
			float preferredSize = LayoutUtility.GetPreferredSize(rectTransform, (int)axis);
			rectTransform.SetSizeWithCurrentAnchors(axis, preferredSize);
			if (this._target == null)
			{
				return;
			}
			float num;
			float num2;
			this.GetMinMax(this._target, axis, out num, out num2);
			float num3;
			float num4;
			this.GetMinMax(this._containWithin, axis, out num3, out num4);
			int num5 = (axis == RectTransform.Axis.Horizontal) ? this._padding.horizontal : this._padding.vertical;
			Vector3 position = rectTransform.position;
			if (axis == RectTransform.Axis.Horizontal)
			{
				if (num4 > num2 + preferredSize + (float)num5)
				{
					position.x = num2 + (float)this._padding.left;
				}
				else
				{
					position.x = num - (float)this._padding.right - preferredSize;
				}
			}
			else
			{
				position.y = Mathf.Max(num - (float)this._padding.top, num3 + preferredSize + (float)this._padding.bottom);
			}
			rectTransform.position = position;
		}

		private void GetMinMax(RectTransform rectTransform, RectTransform.Axis axis, out float min, out float max)
		{
			rectTransform.GetWorldCorners(ModioPopupPositioning.FourCornersArray);
			min = float.MaxValue;
			max = float.MinValue;
			foreach (Vector3 vector in ModioPopupPositioning.FourCornersArray)
			{
				float b = (axis == RectTransform.Axis.Horizontal) ? vector.x : vector.y;
				min = Mathf.Min(min, b);
				max = Mathf.Max(max, b);
			}
		}

		[SerializeField]
		private RectTransform _containWithin;

		[SerializeField]
		private RectTransform _target;

		[SerializeField]
		private RectOffset _padding = new RectOffset();

		private static readonly Vector3[] FourCornersArray = new Vector3[4];
	}
}
