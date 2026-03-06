using System;
using UnityEngine;

namespace Liv.Lck.Rendering
{
	public class LckOrientedCompositionLayer : LckCompositionLayer, ILckOrientationAwareLayer
	{
		public void SetOrientation(bool isHorizontal)
		{
			Debug.Log("LCK LckOrientedCompositionLayer:SetOrientation");
			this._isHorizontal = isHorizontal;
		}

		public override Texture CurrentTexture
		{
			get
			{
				if (!this._isHorizontal)
				{
					return this.VerticalTexture;
				}
				return this.HorizontalTexture;
			}
		}

		[Header("Orientation Textures")]
		public Texture HorizontalTexture;

		public Texture VerticalTexture;

		private bool _isHorizontal = true;
	}
}
