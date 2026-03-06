using System;
using Modio.Images;
using Modio.Users;
using UnityEngine;
using UnityEngine.UI;

namespace Modio.Unity.UI.Components.UserProperties
{
	[Serializable]
	public class UserPropertyCreatorAvatar : IUserProperty
	{
		public void OnUserUpdate(UserProfile user)
		{
			if (user == null)
			{
				this._image.texture = this._noUserImage;
				return;
			}
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
			this._lazyImage.SetImage<UserProfile.AvatarResolution>(user.Avatar, this._resolution);
		}

		[SerializeField]
		private RawImage _image;

		[SerializeField]
		private UserProfile.AvatarResolution _resolution;

		[SerializeField]
		private bool _useHighestAvailableResolutionAsFallback = true;

		[SerializeField]
		private Texture _noUserImage;

		private LazyImage<Texture2D> _lazyImage;
	}
}
