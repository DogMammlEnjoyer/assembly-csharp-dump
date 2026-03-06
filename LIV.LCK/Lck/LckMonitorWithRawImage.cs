using System;
using UnityEngine;
using UnityEngine.UI;

namespace Liv.Lck
{
	public class LckMonitorWithRawImage : LckMonitor
	{
		public override void SetRenderTexture(RenderTexture renderTexture)
		{
			base.SetRenderTexture(renderTexture);
			if (this._monitorImage == null)
			{
				LckLog.LogWarning("LckMonitorWithRawImage has no Raw Image assigned.");
				return;
			}
			this._monitorImage.texture = renderTexture;
			this._monitorImage.color = Color.white;
			if (this._correctImageSize && renderTexture != null)
			{
				this._monitorImage.rectTransform.sizeDelta = new Vector2((float)renderTexture.width, (float)renderTexture.height);
			}
		}

		private void OnDisable()
		{
			LckMediator.UnregisterMonitor(this);
			if (this._monitorImage != null)
			{
				this._monitorImage.color = Color.black;
				this._monitorImage.texture = null;
			}
		}

		[SerializeField]
		private RawImage _monitorImage;

		[SerializeField]
		private bool _correctImageSize;
	}
}
