using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Modio.Unity.UI.Components.ModGallery
{
	public class ModioUIModGalleryPagination : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		public void OnPointerClick(PointerEventData eventData)
		{
			if (this.Gallery)
			{
				this.Gallery.GoTo(this.Index);
			}
		}

		public void SetState(bool active)
		{
			if (this._inactiveGameObject != null)
			{
				this._inactiveGameObject.SetActive(!active);
			}
			if (this._activeGameObject != null)
			{
				this._activeGameObject.SetActive(active);
			}
		}

		internal ModioUIModGallery Gallery;

		internal int Index;

		[SerializeField]
		private GameObject _inactiveGameObject;

		[SerializeField]
		private GameObject _activeGameObject;
	}
}
