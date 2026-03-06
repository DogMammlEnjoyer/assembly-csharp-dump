using System;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.UserInterface.Generic
{
	public class OverlayCanvasPanel : Panel
	{
		protected override void Setup(Controller owner)
		{
			base.Setup(owner);
			if (!RuntimeSettings.Instance.ShouldUseOverlay)
			{
				return;
			}
			this._canvas.sortingOrder = -100;
			this._canvas.additionalShaderChannels = (AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.Tangent);
			this._overlayCanvas = base.GameObject.AddComponent<OverlayCanvas>();
			this._overlayCanvas.Panel = this;
		}

		private OverlayCanvas _overlayCanvas;
	}
}
