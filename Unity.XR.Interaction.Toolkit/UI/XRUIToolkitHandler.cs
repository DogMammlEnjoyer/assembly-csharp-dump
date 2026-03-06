using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.InputForUI;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
	internal static class XRUIToolkitHandler
	{
		public static bool uiToolkitSupportEnabled { get; set; }

		public static int count
		{
			get
			{
				return XRUIToolkitHandler.s_RegisteredInteractors.Count;
			}
		}

		public static int Register(IXRInteractor interactor)
		{
			XRUIToolkitHandler.InteractorInfo interactorInfo;
			if (XRUIToolkitHandler.s_RegisteredInteractors.TryGetValue(interactor, out interactorInfo))
			{
				Debug.LogWarning(string.Format("interactor {0} is already registered with XR UI Toolkit Handler.", interactor));
				return interactorInfo.index;
			}
			int num = -1;
			for (int i = 0; i < 8; i++)
			{
				if (!XRUIToolkitHandler.s_UsedIndices[i])
				{
					num = i;
					XRUIToolkitHandler.s_UsedIndices[i] = true;
					break;
				}
			}
			if (num == -1)
			{
				Debug.LogError("No available indices for pointer registration.");
				return -1;
			}
			XRUIToolkitHandler.InteractorInfo value = new XRUIToolkitHandler.InteractorInfo
			{
				interactor = interactor,
				index = num
			};
			XRUIToolkitHandler.s_RegisteredInteractors.Add(interactor, value);
			return num;
		}

		public static void Unregister(IXRInteractor interactor)
		{
			XRUIToolkitHandler.InteractorInfo interactorInfo;
			if (!XRUIToolkitHandler.s_RegisteredInteractors.TryGetValue(interactor, out interactorInfo))
			{
				return;
			}
			XRUIToolkitHandler.s_LastWasDown.Remove(interactorInfo.index);
			XRUIToolkitHandler.s_WasReset.Remove(interactorInfo.index);
			XRUIToolkitHandler.s_UsedIndices[interactorInfo.index] = false;
			XRUIToolkitHandler.s_RegisteredInteractors.Remove(interactor);
			XRUIToolkitHandler.s_InteractorHitData.Remove(interactor);
			XRUIToolkitHandler.ClearZDepthForInteractor(interactor);
		}

		public static bool TryGetPointerIndex(IXRInteractor interactor, out int index)
		{
			XRUIToolkitHandler.InteractorInfo interactorInfo;
			if (XRUIToolkitHandler.s_RegisteredInteractors.TryGetValue(interactor, out interactorInfo))
			{
				index = interactorInfo.index;
				return true;
			}
			index = -1;
			return false;
		}

		public static void UpdateInteractorHitData(IXRInteractor interactor, InteractorHitData hitData)
		{
			XRUIToolkitHandler.s_InteractorHitData[interactor] = hitData;
		}

		public static bool TryGetInteractorHitData(IXRInteractor interactor, out InteractorHitData hitData)
		{
			return XRUIToolkitHandler.s_InteractorHitData.TryGetValue(interactor, out hitData);
		}

		public static void ClearInteractorHitData(IXRInteractor interactor)
		{
			XRUIToolkitHandler.ClearZDepthForInteractor(interactor);
			XRUIToolkitHandler.s_InteractorHitData.Remove(interactor);
		}

		public static void Clear()
		{
			XRUIToolkitHandler.s_RegisteredInteractors.Clear();
			XRUIToolkitHandler.s_LastWasDown.Clear();
			XRUIToolkitHandler.s_WasReset.Clear();
			XRUIToolkitHandler.s_InteractorHitData.Clear();
			XRUIToolkitHandler.s_InteractorElements.Clear();
			for (int i = 0; i < 8; i++)
			{
				XRUIToolkitHandler.s_UsedIndices[i] = false;
			}
		}

		public static bool IsRegistered(IXRInteractor interactor)
		{
			return XRUIToolkitHandler.s_RegisteredInteractors.ContainsKey(interactor);
		}

		public static void HandlePointerUpdate(IXRInteractor interactor, Vector3 pos, Quaternion rot, bool isUiSelectInputActive, bool shouldReset)
		{
			int num;
			if (!XRUIToolkitHandler.TryGetPointerIndex(interactor, out num))
			{
				return;
			}
			XRUIToolkitHandler.s_LastWasDown.TryAdd(num, false);
			XRUIToolkitHandler.s_WasReset.TryAdd(num, shouldReset);
			if (shouldReset && XRUIToolkitHandler.s_WasReset[num])
			{
				return;
			}
			if (XRUIToolkitHandler.ShouldCheckPanelInputConfigurationValidation())
			{
				XRUIToolkitHandler.ValidatePanelInputConfiguration();
			}
			Vector3 worldPosition = shouldReset ? XRUIToolkitHandler.k_ResetPos : pos;
			Quaternion worldOrientation = shouldReset ? Quaternion.identity : rot;
			Event @event = Event.From(new PointerEvent
			{
				pointerIndex = num,
				type = PointerEvent.Type.PointerMoved,
				worldPosition = worldPosition,
				worldOrientation = worldOrientation,
				eventSource = EventSource.TrackedDevice,
				maxDistance = 10f
			});
			EventProvider.Dispatch(@event);
			bool flag = !shouldReset && isUiSelectInputActive;
			if (flag != XRUIToolkitHandler.s_LastWasDown[num])
			{
				XRUIToolkitHandler.s_LastWasDown[num] = flag;
				PointerEvent.Type type = flag ? PointerEvent.Type.ButtonPressed : PointerEvent.Type.ButtonReleased;
				@event = Event.From(new PointerEvent
				{
					pointerIndex = num,
					type = type,
					button = PointerEvent.Button.Primary,
					clickCount = 1,
					worldPosition = worldPosition,
					worldOrientation = worldOrientation,
					eventSource = EventSource.TrackedDevice,
					maxDistance = 10f
				});
				EventProvider.Dispatch(@event);
			}
			XRUIToolkitHandler.s_WasReset[num] = shouldReset;
			if (shouldReset)
			{
				XRUIToolkitHandler.ClearInteractorHitData(interactor);
			}
		}

		public static bool TryGetPointerHitData(IXRInteractor interactor, out PointerHitData hitData)
		{
			hitData = default(PointerHitData);
			int num;
			if (!XRUIToolkitHandler.TryGetPointerIndex(interactor, out num))
			{
				return false;
			}
			PointerDeviceState.TrackedPointerState trackedState = PointerDeviceState.GetTrackedState(PointerId.trackedPointerIdBase + num, false);
			if (trackedState == null)
			{
				return false;
			}
			hitData = new PointerHitData
			{
				worldPosition = trackedState.worldPosition,
				worldOrientation = trackedState.worldOrientation,
				hitDistance = trackedState.hit.distance,
				hitCollider = trackedState.hit.collider,
				hitDocument = trackedState.hit.document,
				hitElement = trackedState.hit.element
			};
			return true;
		}

		public static float SetZDepthForInteractor(VisualElement ve, IXRInteractor interactor, float z)
		{
			XRUIToolkitHandler.s_InteractorElements[interactor] = ve;
			Translate value = ve.style.translate.value;
			if (!XRUIToolkitHandler.s_InitialZDepth.TryAdd(ve.controlid, value.z))
			{
				XRUIToolkitHandler.s_InitialZDepth[ve.controlid] = value.z;
			}
			ve.style.translate = new Translate(value.x.value, value.y.value, z);
			return z;
		}

		private static float ResetDepth(VisualElement ve)
		{
			Translate value = ve.style.translate.value;
			float num;
			if (XRUIToolkitHandler.s_InitialZDepth.TryGetValue(ve.controlid, out num))
			{
				ve.style.translate = new Translate(value.x.value, value.y.value, num);
			}
			else
			{
				ve.style.translate = new Translate(value.x.value, value.y.value, 0f);
			}
			return num;
		}

		public static void ClearZDepthForInteractor(IXRInteractor interactor)
		{
			VisualElement visualElement;
			if (XRUIToolkitHandler.s_InteractorElements.TryGetValue(interactor, out visualElement) && visualElement != null)
			{
				XRUIToolkitHandler.ResetDepth(visualElement);
				XRUIToolkitHandler.s_InteractorElements.Remove(interactor);
			}
		}

		public static void UpdateEventSystem()
		{
			if (XRUIToolkitHandler.count > 0)
			{
				UIElementsRuntimeUtility.UpdateEventSystem();
			}
		}

		public static bool IsValidUIToolkitInteraction(List<Collider> colliders)
		{
			return colliders.Count > 0 && XRUIToolkitHandler.HasUIDocument(colliders[0]);
		}

		public static bool HasUIDocument(Collider collider)
		{
			UIDocument uidocument;
			return collider.TryGetComponent<UIDocument>(out uidocument);
		}

		private static bool ShouldCheckPanelInputConfigurationValidation()
		{
			if (XRUIToolkitHandler.s_PanelInputConfigurationRef != PanelInputConfiguration.current)
			{
				XRUIToolkitHandler.s_EventSystemValidated = false;
				XRUIToolkitHandler.s_PanelInputConfigurationValidated = false;
				XRUIToolkitHandler.s_DidCheckPanelInputConfiguration = false;
				return true;
			}
			return !XRUIToolkitHandler.s_DidCheckPanelInputConfiguration && (!XRUIToolkitHandler.s_EventSystemValidated || !XRUIToolkitHandler.s_PanelInputConfigurationValidated);
		}

		private static void ValidatePanelInputConfiguration()
		{
			XRUIToolkitHandler.s_DidCheckPanelInputConfiguration = true;
			XRUIToolkitHandler.s_PanelInputConfigurationValidated = false;
			if (!XRUIToolkitHandler.s_EventSystemValidated && EventSystem.current == null)
			{
				return;
			}
			XRUIToolkitHandler.s_EventSystemValidated = true;
			PanelInputConfiguration current = PanelInputConfiguration.current;
			XRUIToolkitHandler.s_PanelInputConfigurationRef = current;
			if (current == null)
			{
				Debug.LogWarning("Detected an Event System component that could interfere with UI Toolkit input. Create a Panel Input Configuration component and configured it by setting Panel Input Redirection to No input redirection to prevent interactions with the Event System.");
				return;
			}
			if (current.panelInputRedirection != PanelInputConfiguration.PanelInputRedirection.Never)
			{
				Debug.LogWarning("Detected an Event System component that could interfere with UI Toolkit input. Configure your Panel Input Configuration component to set Panel Input Redirection to No input redirection to prevent interactions with the Event System.");
				return;
			}
			XRUIToolkitHandler.s_PanelInputConfigurationValidated = true;
		}

		private const int k_MaxInteractors = 8;

		private const int k_InvalidIndex = -1;

		private static readonly Vector3 k_ResetPos = new Vector3(0f, -1000f, 0f);

		private static readonly Dictionary<IXRInteractor, XRUIToolkitHandler.InteractorInfo> s_RegisteredInteractors = new Dictionary<IXRInteractor, XRUIToolkitHandler.InteractorInfo>();

		private static readonly Dictionary<IXRInteractor, InteractorHitData> s_InteractorHitData = new Dictionary<IXRInteractor, InteractorHitData>();

		private static readonly bool[] s_UsedIndices = new bool[8];

		private static readonly Dictionary<int, bool> s_LastWasDown = new Dictionary<int, bool>();

		private static readonly Dictionary<int, bool> s_WasReset = new Dictionary<int, bool>();

		private static PanelInputConfiguration s_PanelInputConfigurationRef;

		private static bool s_EventSystemValidated;

		private static bool s_PanelInputConfigurationValidated;

		private static bool s_DidCheckPanelInputConfiguration;

		private static readonly Dictionary<IXRInteractor, VisualElement> s_InteractorElements = new Dictionary<IXRInteractor, VisualElement>();

		private static readonly Dictionary<uint, float> s_InitialZDepth = new Dictionary<uint, float>();

		private class InteractorInfo
		{
			public IXRInteractor interactor;

			public int index;
		}
	}
}
