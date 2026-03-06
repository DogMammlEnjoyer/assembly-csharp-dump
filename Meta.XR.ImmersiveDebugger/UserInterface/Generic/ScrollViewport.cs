using System;
using UnityEngine.UI;

namespace Meta.XR.ImmersiveDebugger.UserInterface.Generic
{
	public class ScrollViewport : Controller
	{
		internal Flex Flex
		{
			get
			{
				return this._flex;
			}
		}

		protected override void Setup(Controller owner)
		{
			base.Setup(owner);
			ScrollView scrollView = owner as ScrollView;
			if (scrollView == null)
			{
				return;
			}
			this._image = base.GameObject.AddComponent<RawImage>();
			this._image.raycastTarget = true;
			this._mask = base.GameObject.AddComponent<Mask>();
			this._mask.showMaskGraphic = false;
			this._flex = base.Append<Flex>("content");
			scrollView.ScrollRect.content = this._flex.RectTransform;
			this._flex.ScrollViewport = this;
		}

		private RawImage _image;

		private Mask _mask;

		private Flex _flex;
	}
}
