using System;
using Modio.Images;
using Modio.Mods;
using Modio.Users;
using UnityEngine;
using UnityEngine.UI;

namespace Modio.Unity.UI.Components.ModProperties
{
	[Serializable]
	public class ModPropertyCreatorAvatar : IModProperty
	{
		public void OnModUpdate(Mod mod)
		{
			if (this._lazyImage == null)
			{
				this._lazyImage = new LazyImage<Texture2D>(ImageCacheTexture2D.Instance, delegate(Texture2D texture2D)
				{
					if (this._image != null)
					{
						this._image.texture = texture2D;
					}
				}, null);
			}
			this._lazyImage.SetImage<UserProfile.AvatarResolution>(mod.Creator.Avatar, this._resolution);
		}

		[SerializeField]
		private RawImage _image;

		[SerializeField]
		private UserProfile.AvatarResolution _resolution;

		private LazyImage<Texture2D> _lazyImage;
	}
}
