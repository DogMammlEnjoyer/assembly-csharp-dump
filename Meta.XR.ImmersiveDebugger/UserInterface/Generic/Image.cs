using System;
using Meta.XR.ImmersiveDebugger.Manager;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.UserInterface.Generic
{
	public class Image : Icon
	{
		public override Texture2D Texture
		{
			internal get
			{
				return base.Texture;
			}
			set
			{
				base.Texture = value;
				if (value != null)
				{
					this.UpdateSize();
				}
				base.RefreshLayout();
			}
		}

		internal void Setup(WatchTexture watchTexture)
		{
			this._watchTexture = watchTexture;
			this._defaultHeight = base.LayoutStyle.size.y;
			this.Texture = watchTexture.Texture;
		}

		private void UpdateSize()
		{
			int width = this.Texture.width;
			int height = this.Texture.height;
			int num = width / height;
			float num2 = Mathf.Min((float)height, this._defaultHeight);
			float x = num2 * (float)num;
			base.LayoutStyle.size = new Vector2(x, num2);
			base.Owner.LayoutStyle.size.y = num2;
		}

		private void Update()
		{
			WatchTexture watchTexture = this._watchTexture;
			if (watchTexture == null || !watchTexture.Valid)
			{
				return;
			}
			if (this._watchTexture.Texture == null)
			{
				return;
			}
			this.Texture = this._watchTexture.Texture;
		}

		private WatchTexture _watchTexture;

		private float _defaultHeight;
	}
}
