using System;
using Liv.Lck.Rendering;
using UnityEngine;

namespace Liv.Lck.GorillaTag
{
	[Serializable]
	public class LckOverlayFrameLayer : LckOrientedCompositionLayer
	{
		public override Texture CurrentTexture
		{
			get
			{
				if (this.IsActive)
				{
					return base.CurrentTexture;
				}
				return null;
			}
		}

		public LckOverlayFrameLayer()
		{
			this.Name = "Overlay Frame";
		}
	}
}
