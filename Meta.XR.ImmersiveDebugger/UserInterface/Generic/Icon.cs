using System;
using UnityEngine;
using UnityEngine.UI;

namespace Meta.XR.ImmersiveDebugger.UserInterface.Generic
{
	public class Icon : Controller
	{
		internal RawImage RawImage
		{
			get
			{
				return this._image;
			}
		}

		public virtual Texture2D Texture
		{
			internal get
			{
				return (Texture2D)this._image.texture;
			}
			set
			{
				this._image.texture = value;
			}
		}

		public Color Color
		{
			set
			{
				this._image.color = value;
			}
		}

		public bool RaycastTarget
		{
			set
			{
				this._image.raycastTarget = value;
			}
		}

		protected override void Setup(Controller owner)
		{
			base.Setup(owner);
			this._image = base.GameObject.AddComponent<RawImage>();
			this.RaycastTarget = false;
		}

		private RawImage _image;
	}
}
