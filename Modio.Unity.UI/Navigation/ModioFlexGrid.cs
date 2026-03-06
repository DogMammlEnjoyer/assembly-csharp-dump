using System;
using UnityEngine;
using UnityEngine.UI;

namespace Modio.Unity.UI.Navigation
{
	public class ModioFlexGrid : LayoutGroup
	{
		public override void CalculateLayoutInputHorizontal()
		{
			base.CalculateLayoutInputHorizontal();
			this.CalcAlongAxis(0, false);
			base.SetLayoutInputForAxis(0f, 0f, 1f, 0);
		}

		public override void CalculateLayoutInputVertical()
		{
			base.SetLayoutInputForAxis(0f, 100f, 0f, 1);
			this.CalcAlongAxis(1, false);
		}

		public override void SetLayoutHorizontal()
		{
			this.SetChildrenAlongAxis(0);
		}

		public override void SetLayoutVertical()
		{
			this.SetChildrenAlongAxis(1);
		}

		private void CalcAlongAxis(int axis, bool isVertical)
		{
			float num = (float)((axis == 0) ? base.padding.horizontal : base.padding.vertical);
			bool flag = (axis == 0) ? this.m_ChildScaleWidth : this.m_ChildScaleHeight;
			float num2 = num;
			float num3 = num;
			float num4 = 0f;
			bool flag2 = isVertical ^ axis == 1;
			int count = base.rectChildren.Count;
			float num5 = 0f;
			float num6 = 0f;
			float num7 = 0f;
			float num8 = 0f;
			float num9 = 0f;
			float num10 = 0f;
			float num11 = base.rectTransform.rect.width - (float)base.padding.horizontal;
			for (int i = 0; i < count; i++)
			{
				RectTransform rectTransform = base.rectChildren[i];
				float num12;
				float num13;
				float num14;
				this.GetChildSizes(rectTransform, 0, this.m_ChildControlWidth, this.m_ChildForceExpandWidth, out num12, out num13, out num14);
				float num15;
				float num16;
				float num17;
				this.GetChildSizes(rectTransform, 1, this.m_ChildControlHeight, this.m_ChildForceExpandHeight, out num15, out num16, out num17);
				float num18 = (axis == 0) ? num12 : num15;
				float num19 = (axis == 0) ? num13 : num16;
				float num20 = (axis == 0) ? num14 : num17;
				if (flag)
				{
					float num21 = rectTransform.localScale[axis];
					num18 *= num21;
					num19 *= num21;
					num20 *= num21;
				}
				num5 += num12;
				num6 += num13;
				num7 += num14;
				num8 = Mathf.Max(num15, num8);
				num9 = Mathf.Max(num16, num9);
				num10 = Mathf.Max(num17, num10);
				if (num6 > num11)
				{
					if (axis == 1)
					{
						num2 += num8 + this.m_Spacing;
						num3 += num9 + this.m_Spacing;
						num4 += num10;
					}
					num5 = num12;
					num6 = num13;
					num7 = num14;
				}
				if (axis == 0)
				{
					num2 = Mathf.Max(num5 + num, num2);
					num3 = Mathf.Max(num6 + num, num3);
					num4 = Mathf.Max(num7, num4);
				}
				if (flag2)
				{
					num2 = Mathf.Max(num18 + num, num2);
					num3 = Mathf.Max(num19 + num, num3);
					num4 = Mathf.Max(num20, num4);
				}
				else
				{
					num2 += num18 + this.m_Spacing;
					num3 += num19 + this.m_Spacing;
					num4 += num20;
				}
			}
			if (!flag2 && base.rectChildren.Count > 0)
			{
				num2 -= this.m_Spacing;
				num3 -= this.m_Spacing;
			}
			num3 = Mathf.Max(num2, num3);
			base.SetLayoutInputForAxis(num2, num3, num4, axis);
		}

		private void SetChildrenAlongAxis(int axis)
		{
			float num = base.rectTransform.rect.size[axis];
			int num2 = this.m_ReverseArrangement ? (base.rectChildren.Count - 1) : 0;
			int num3 = this.m_ReverseArrangement ? 0 : base.rectChildren.Count;
			int num4 = this.m_ReverseArrangement ? -1 : 1;
			float num5 = 0f;
			float num6 = base.rectTransform.rect.width - (float)base.padding.horizontal;
			float num7 = (float)((axis == 0) ? base.padding.left : base.padding.top);
			if (num - base.GetTotalPreferredSize(axis) > 0f && base.GetTotalFlexibleSize(axis) == 0f)
			{
				num7 = base.GetStartOffset(axis, base.GetTotalPreferredSize(axis) - (float)((axis == 0) ? base.padding.horizontal : base.padding.vertical));
			}
			int num8 = num2;
			while (this.m_ReverseArrangement ? (num8 >= num3) : (num8 < num3))
			{
				RectTransform rectTransform = base.rectChildren[num8];
				float num9 = this.m_ChildControlWidth ? LayoutUtility.GetPreferredSize(rectTransform, 0) : rectTransform.sizeDelta.x;
				float num10 = this.m_ChildControlHeight ? LayoutUtility.GetPreferredSize(rectTransform, 1) : rectTransform.sizeDelta.y;
				num5 += num9;
				if (num5 > num6)
				{
					if (axis == 1)
					{
						num7 += num10 + this.m_Spacing;
					}
					else
					{
						num7 = (float)base.padding.left;
					}
					num5 = num9;
				}
				if (axis == 1)
				{
					base.SetChildAlongAxisWithScale(rectTransform, axis, num7, num10, 1f);
				}
				else
				{
					base.SetChildAlongAxisWithScale(rectTransform, axis, num7, num9, 1f);
					num7 += num9 + this.m_Spacing;
				}
				num8 += num4;
			}
		}

		private void GetChildSizes(RectTransform child, int axis, bool controlSize, bool childForceExpand, out float min, out float preferred, out float flexible)
		{
			if (!controlSize)
			{
				min = child.sizeDelta[axis];
				preferred = min;
				flexible = 0f;
			}
			else
			{
				min = LayoutUtility.GetMinSize(child, axis);
				preferred = LayoutUtility.GetPreferredSize(child, axis);
				flexible = LayoutUtility.GetFlexibleSize(child, axis);
			}
			if (childForceExpand)
			{
				flexible = Mathf.Max(flexible, 1f);
			}
		}

		[SerializeField]
		protected float m_Spacing;

		[SerializeField]
		protected bool m_ChildForceExpandWidth;

		[SerializeField]
		protected bool m_ChildForceExpandHeight;

		[SerializeField]
		protected bool m_ChildControlWidth = true;

		[SerializeField]
		protected bool m_ChildControlHeight = true;

		[SerializeField]
		protected bool m_ChildScaleWidth;

		[SerializeField]
		protected bool m_ChildScaleHeight;

		[SerializeField]
		protected bool m_ReverseArrangement;
	}
}
