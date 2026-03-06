using System;
using Modio.Images;
using Modio.Mods;
using UnityEngine;
using UnityEngine.UI;

namespace Modio.Unity.UI.Components.ModProperties
{
	[Serializable]
	public class ModPropertyLogo : IModProperty
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
				}, delegate(bool isLoading)
				{
					if (this._loadingActive)
					{
						this._loadingActive.SetActive(isLoading);
					}
					if (this._loadedActive)
					{
						this._loadedActive.SetActive(!isLoading);
					}
				});
			}
			this._lazyImage.SetImage<Mod.LogoResolution>(mod.Logo, this._resolution);
		}

		[SerializeField]
		private RawImage _image;

		[SerializeField]
		private Mod.LogoResolution _resolution;

		[SerializeField]
		private bool _useHighestAvailableResolutionAsFallback = true;

		[Space]
		[Tooltip("(Optional) Active while loading, inactive once loaded.")]
		[SerializeField]
		private GameObject _loadingActive;

		[Tooltip("(Optional) Inactive while loading, active once loaded.")]
		[SerializeField]
		private GameObject _loadedActive;

		private LazyImage<Texture2D> _lazyImage;
	}
}
