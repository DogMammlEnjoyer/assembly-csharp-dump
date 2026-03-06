using System;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
	internal class RegisteredUIInteractorCache
	{
		public RegisteredUIInteractorCache(IUIInteractor uiInteractor)
		{
			this.m_UiInteractor = uiInteractor;
			this.m_BaseInteractor = (uiInteractor as XRBaseInteractor);
		}

		public void RegisterOrUnregisterXRUIInputModule(bool enabled)
		{
			if (!Application.isPlaying || (this.m_BaseInteractor != null && !this.m_BaseInteractor.isActiveAndEnabled))
			{
				return;
			}
			if (enabled)
			{
				this.RegisterWithXRUIInputModule();
				return;
			}
			this.UnregisterFromXRUIInputModule();
		}

		public void RegisterWithXRUIInputModule()
		{
			if (this.m_InputModule == null)
			{
				this.FindOrCreateXRUIInputModule();
			}
			if (this.m_RegisteredInputModule == this.m_InputModule)
			{
				return;
			}
			this.UnregisterFromXRUIInputModule();
			this.m_InputModule.RegisterInteractor(this.m_UiInteractor);
			this.m_RegisteredInputModule = this.m_InputModule;
		}

		public void UnregisterFromXRUIInputModule()
		{
			if (this.m_RegisteredInputModule != null)
			{
				this.m_RegisteredInputModule.UnregisterInteractor(this.m_UiInteractor);
			}
			this.m_RegisteredInputModule = null;
		}

		private void FindOrCreateXRUIInputModule()
		{
			EventSystem eventSystem = EventSystem.current;
			if (eventSystem == null)
			{
				if (ComponentLocatorUtility<EventSystem>.TryFindComponent(out eventSystem))
				{
					StandaloneInputModule obj;
					if (eventSystem.TryGetComponent<StandaloneInputModule>(out obj))
					{
						Object.Destroy(obj);
					}
				}
				else
				{
					eventSystem = new GameObject("EventSystem", new Type[]
					{
						typeof(EventSystem)
					}).GetComponent<EventSystem>();
				}
			}
			if (!eventSystem.TryGetComponent<XRUIInputModule>(out this.m_InputModule))
			{
				this.m_InputModule = eventSystem.gameObject.AddComponent<XRUIInputModule>();
			}
		}

		public bool TryGetUIModel(out TrackedDeviceModel model)
		{
			if (this.m_InputModule != null)
			{
				return this.m_InputModule.GetTrackedDeviceModel(this.m_UiInteractor, out model);
			}
			model = TrackedDeviceModel.invalid;
			return false;
		}

		public bool IsOverUIGameObject()
		{
			TrackedDeviceModel trackedDeviceModel;
			return this.m_InputModule != null && this.TryGetUIModel(out trackedDeviceModel) && this.m_InputModule.IsPointerOverGameObject(trackedDeviceModel.pointerId);
		}

		public bool TryGetCurrentUIGameObject(bool useAnyPointerId, out GameObject currentGameObject)
		{
			if (this.m_InputModule != null)
			{
				TrackedDeviceModel trackedDeviceModel;
				if (useAnyPointerId)
				{
					currentGameObject = this.m_InputModule.GetCurrentGameObject(-1);
				}
				else if (this.TryGetUIModel(out trackedDeviceModel))
				{
					currentGameObject = this.m_InputModule.GetCurrentGameObject(trackedDeviceModel.pointerId);
				}
				else
				{
					currentGameObject = null;
				}
				return currentGameObject != null;
			}
			currentGameObject = null;
			return false;
		}

		private XRUIInputModule m_InputModule;

		private XRUIInputModule m_RegisteredInputModule;

		private readonly IUIInteractor m_UiInteractor;

		private readonly XRBaseInteractor m_BaseInteractor;
	}
}
