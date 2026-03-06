using System;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.UserInterface.Generic
{
	public class Flex : Controller
	{
		internal Vector2 SizeDelta
		{
			get
			{
				return this._sizeDelta;
			}
		}

		internal Vector2 SizeDeltaWithMargin
		{
			get
			{
				return this._sizeDelta + base.LayoutStyle.TopLeftMargin + base.LayoutStyle.BottomRightMargin;
			}
		}

		internal ScrollViewport ScrollViewport { get; set; }

		private void UpdateAnchoredPosition(Controller controller, ref Vector2 offset, Vector2 direction)
		{
			Vector2 margin = controller.LayoutStyle.margin;
			Vector2 a = new Vector2(margin.x, -margin.y);
			Vector2 sizeDelta = controller.RectTransform.sizeDelta;
			controller.RectTransform.anchoredPosition = a + offset;
			offset += direction * sizeDelta;
			offset += direction * this._layoutStyle.spacing;
		}

		private void UpdateChildrenWidth()
		{
			if (this._children == null)
			{
				return;
			}
			if (!this._layoutStyle.autoFitChildren || this._layoutStyle.size.x <= 0f)
			{
				return;
			}
			float num = base.RectTransform.sizeDelta.x;
			int num2 = 0;
			foreach (Controller controller in this._children)
			{
				if (controller.LayoutStyle.layout == LayoutStyle.Layout.Fixed)
				{
					num -= controller.LayoutStyle.size.x + controller.LayoutStyle.margin.x * 2f + this._layoutStyle.spacing;
					num2++;
				}
			}
			int num3 = this._children.Count - num2;
			if (num3 == 0)
			{
				return;
			}
			foreach (Controller controller2 in this._children)
			{
				float width = (float)Mathf.RoundToInt(num / (float)num3) - this._layoutStyle.spacing;
				if (controller2.LayoutStyle.layout != LayoutStyle.Layout.Fixed)
				{
					controller2.SetWidth(width);
				}
			}
		}

		private void RefreshVisibilities(bool force = false)
		{
			if (this.ScrollViewport == null)
			{
				return;
			}
			if (this._children == null)
			{
				return;
			}
			Vector2 anchoredPosition = base.RectTransform.anchoredPosition;
			if (!force && anchoredPosition == this._previousAnchoredPosition)
			{
				return;
			}
			this._previousAnchoredPosition = new Vector2?(anchoredPosition);
			Vector2 anchoredPosition2 = base.RectTransform.anchoredPosition;
			Rect viewportRect = new Rect(this.ScrollViewport.RectTransform.anchoredPosition, this.ScrollViewport.RectTransform.rect.size);
			bool flag = false;
			bool flag2 = false;
			foreach (Controller controller in this._children)
			{
				if (!flag2 && Flex.IsVerticallyInViewport(controller, viewportRect, anchoredPosition2))
				{
					controller.Show();
					flag = true;
				}
				else
				{
					controller.Hide();
					if (flag)
					{
						flag2 = true;
					}
				}
			}
		}

		private static bool IsVerticallyInViewport(Controller controller, Rect viewportRect, Vector2 scroll)
		{
			Vector2 vector = -controller.RectTransform.anchoredPosition - scroll;
			if (vector.y >= viewportRect.yMin)
			{
				if (vector.y < viewportRect.yMax)
				{
					return true;
				}
			}
			else if (vector.y + controller.RectTransform.sizeDelta.y >= viewportRect.yMin)
			{
				return true;
			}
			return false;
		}

		protected override void RefreshLayoutPreChildren()
		{
			base.RefreshLayoutPreChildren();
			this.UpdateChildrenWidth();
		}

		protected override void RefreshLayoutPostChildren()
		{
			if (!this._hasRectTransform)
			{
				return;
			}
			if (this._children != null)
			{
				Vector2 vector;
				switch (this._layoutStyle.flexDirection)
				{
				case LayoutStyle.Direction.Left:
					vector = Vector2.left;
					break;
				case LayoutStyle.Direction.Right:
					vector = Vector2.right;
					break;
				case LayoutStyle.Direction.Down:
					vector = Vector2.down;
					break;
				case LayoutStyle.Direction.Up:
					vector = Vector2.up;
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				Vector2 direction = vector;
				Vector2 zero = Vector2.zero;
				foreach (Controller controller in this._children)
				{
					this.UpdateAnchoredPosition(controller, ref zero, direction);
				}
				this._previousAnchoredPosition = null;
				this._sizeDelta = new Vector2(Mathf.Abs(zero.x), Mathf.Abs(zero.y));
			}
			if (base.LayoutStyle.adaptHeight)
			{
				base.RectTransform.sizeDelta = new Vector2(base.RectTransform.sizeDelta.x, Mathf.Abs(this._sizeDelta.y));
			}
		}

		private void LateUpdate()
		{
			this.RefreshVisibilities(false);
		}

		internal void Forget(Controller controller)
		{
			base.Remove(controller, false);
			controller.Hide();
		}

		internal void Remember(Controller controller)
		{
			base.Append(controller);
			controller.Show();
		}

		internal void ForgetAll()
		{
			if (this._children == null)
			{
				return;
			}
			foreach (Controller controller in this._children)
			{
				controller.Hide();
			}
			base.Clear(false);
		}

		private Vector2 _sizeDelta;

		private Vector2? _previousAnchoredPosition;
	}
}
