using System;
using Modio.Mods;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Modio.Unity.UI.Components
{
	public class ModioUIMod : MonoBehaviour, IModioUIPropertiesOwner, IPointerClickHandler, IEventSystemHandler, ISubmitHandler
	{
		public Mod Mod { get; private set; }

		private void OnDestroy()
		{
			if (this.Mod != null)
			{
				this.Mod.OnModUpdated -= this.OnModUpdated;
			}
		}

		public void AddUpdatePropertiesListener(UnityAction listener)
		{
			this.onModUpdate.AddListener(listener);
		}

		public void RemoveUpdatePropertiesListener(UnityAction listener)
		{
			this.onModUpdate.RemoveListener(listener);
		}

		public void SetMod(Mod mod)
		{
			if (this.Mod != null)
			{
				this.Mod.OnModUpdated -= this.OnModUpdated;
			}
			this.Mod = mod;
			if (mod == null)
			{
				return;
			}
			this.Mod.OnModUpdated += this.OnModUpdated;
			this.OnModUpdated();
		}

		private void OnModUpdated()
		{
			UnityEvent unityEvent = this.onModUpdate;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			this.OnSubmit(eventData);
		}

		public void OnSubmit(BaseEventData eventData)
		{
			UnityEvent<Mod> unityEvent = this.onClickOrSubmit;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this.Mod);
		}

		public void OnDisplayMoreInfoClicked()
		{
			UnityEvent<ModioUIMod> unityEvent = this.onDisplayMoreInfo;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke(this);
		}

		public UnityEvent onModUpdate;

		public UnityEvent<Mod> onClickOrSubmit;

		public UnityEvent<ModioUIMod> onDisplayMoreInfo;
	}
}
