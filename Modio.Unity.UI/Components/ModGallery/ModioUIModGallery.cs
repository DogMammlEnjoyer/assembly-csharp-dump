using System;
using System.Collections.Generic;
using System.Linq;
using Modio.Images;
using Modio.Mods;
using Modio.Unity.UI.Input;
using UnityEngine;
using UnityEngine.UI;

namespace Modio.Unity.UI.Components.ModGallery
{
	public class ModioUIModGallery : ModioUIModProperties
	{
		protected override void Awake()
		{
			base.Awake();
			if (this._paginationTemplate != null)
			{
				this._paginationTemplate.Gallery = this;
				this._pagination.Add(this._paginationTemplate);
			}
		}

		private void OnDisable()
		{
			ModioUIInput.RemoveHandler(ModioUIInput.ModioAction.TabLeft, new Action(this.Prev));
			ModioUIInput.RemoveHandler(ModioUIInput.ModioAction.TabRight, new Action(this.Next));
		}

		protected override void UpdateProperties()
		{
			if (this.Owner.Mod == null)
			{
				return;
			}
			if (this.Owner.Mod != this._mod)
			{
				this.SetMod(this.Owner.Mod);
			}
			this.UpdateTabListener();
			this.GoTo(this._index);
		}

		private void SetMod(Mod mod)
		{
			this._mod = mod;
			this._galleryCount = Mathf.Min(mod.Gallery.Length, this._max);
			this._index = 0;
			if (this._pagination.Any<ModioUIModGalleryPagination>())
			{
				for (int i = this._pagination.Count; i < this._galleryCount; i++)
				{
					ModioUIModGalleryPagination modioUIModGalleryPagination = Object.Instantiate<ModioUIModGalleryPagination>(this._pagination[0], this._pagination[0].transform.parent);
					modioUIModGalleryPagination.Gallery = this;
					modioUIModGalleryPagination.Index = i;
					this._pagination.Add(modioUIModGalleryPagination);
				}
				for (int j = 0; j < this._pagination.Count; j++)
				{
					this._pagination[j].gameObject.SetActive(this._galleryCount > 1 && j < this._galleryCount);
				}
			}
		}

		private void UpdateTabListener()
		{
			ModioUIInput.RemoveHandler(ModioUIInput.ModioAction.TabLeft, new Action(this.Prev));
			ModioUIInput.RemoveHandler(ModioUIInput.ModioAction.TabRight, new Action(this.Next));
			if (this._galleryCount > 1)
			{
				ModioUIInput.AddHandler(ModioUIInput.ModioAction.TabLeft, new Action(this.Prev));
				ModioUIInput.AddHandler(ModioUIInput.ModioAction.TabRight, new Action(this.Next));
			}
		}

		public void GoTo(int index)
		{
			index = ((this._galleryCount != 0) ? ((index + this._galleryCount) % this._galleryCount) : 0);
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
			if (this._galleryCount > 0)
			{
				this._lazyImage.SetImage<Mod.GalleryResolution>(this._mod.Gallery[index], this._resolution);
			}
			else
			{
				this._lazyImage.SetImage<Mod.LogoResolution>(this._mod.Logo, (Mod.LogoResolution)this._resolution);
			}
			if (this._pagination.Any<ModioUIModGalleryPagination>())
			{
				for (int i = 0; i < this._galleryCount; i++)
				{
					this._pagination[i].SetState(i == index);
				}
			}
			this._index = index;
		}

		public void Prev()
		{
			if (this._wrap || this._index > 0)
			{
				this.GoTo(this._index - 1);
			}
		}

		public void Next()
		{
			if (this._wrap || this._index < this._galleryCount - 1)
			{
				this.GoTo(this._index + 1);
			}
		}

		[SerializeField]
		private RawImage _image;

		[SerializeField]
		private Mod.GalleryResolution _resolution = Mod.GalleryResolution.X1280_Y720;

		[SerializeField]
		private bool _useHighestAvailableResolutionAsFallback = true;

		[SerializeField]
		private ModioUIModGalleryPagination _paginationTemplate;

		[SerializeField]
		private int _max = 10;

		[SerializeField]
		private bool _wrap = true;

		[Space]
		[Tooltip("(Optional) Active while loading, inactive once loaded.")]
		[SerializeField]
		private GameObject _loadingActive;

		[Tooltip("(Optional) Inactive while loading, active once loaded.")]
		[SerializeField]
		private GameObject _loadedActive;

		private Mod _mod;

		private int _galleryCount;

		private int _index;

		private readonly List<ModioUIModGalleryPagination> _pagination = new List<ModioUIModGalleryPagination>();

		private LazyImage<Texture2D> _lazyImage;
	}
}
