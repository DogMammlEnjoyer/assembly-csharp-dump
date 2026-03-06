using System;
using UnityEngine;
using UnityEngine.UI;

namespace Meta.XR.ImmersiveDebugger.UserInterface.Generic
{
	public class Background : Controller
	{
		public Sprite Sprite
		{
			set
			{
				this._image.sprite = value;
			}
		}

		public Color Color
		{
			set
			{
				this._image.color = value;
			}
		}

		public float PixelDensityMultiplier
		{
			set
			{
				this._image.pixelsPerUnitMultiplier = value;
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
			this._image = base.GameObject.AddComponent<Image>();
			this._image.type = Image.Type.Sliced;
			this.RaycastTarget = false;
		}

		private Image _image;
	}
}
