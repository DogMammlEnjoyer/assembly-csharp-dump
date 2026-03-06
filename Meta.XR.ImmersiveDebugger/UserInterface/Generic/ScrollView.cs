using System;
using UnityEngine.UI;

namespace Meta.XR.ImmersiveDebugger.UserInterface.Generic
{
	public class ScrollView : InteractableController
	{
		internal ScrollRect ScrollRect
		{
			get
			{
				return this._scrollRect;
			}
		}

		internal Flex Flex
		{
			get
			{
				return this._viewport.Flex;
			}
		}

		public float Progress
		{
			get
			{
				return this._scrollRect.verticalNormalizedPosition;
			}
			set
			{
				this._scrollRect.verticalNormalizedPosition = value;
			}
		}

		protected override void Setup(Controller owner)
		{
			base.Setup(owner);
			this._scrollRect = base.GameObject.AddComponent<PanelScrollRect>();
			this._scrollRect.horizontal = false;
			this._scrollRect.vertical = true;
			this._scrollRect.inertia = true;
			this._viewport = base.Append<ScrollViewport>("viewport");
			this._viewport.LayoutStyle = Style.Load<LayoutStyle>("Fill");
			this._scrollRect.content = this._viewport.Flex.RectTransform;
		}

		protected override void RefreshLayoutPreChildren()
		{
			this._previousProgress = this.Progress;
			base.RefreshLayoutPreChildren();
		}

		protected override void RefreshLayoutPostChildren()
		{
			this.Progress = this._previousProgress;
			base.RefreshLayoutPostChildren();
		}

		private ScrollRect _scrollRect;

		private ScrollViewport _viewport;

		private Mask _mask;

		private float _previousProgress;
	}
}
