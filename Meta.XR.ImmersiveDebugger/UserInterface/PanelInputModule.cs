using System;
using System.Collections.Generic;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Meta.XR.ImmersiveDebugger.UserInterface
{
	internal class PanelInputModule : OVRInputModule
	{
		public static void RegisterRaycaster(PanelRaycaster raycaster)
		{
			if (PanelInputModule._raycasters.Contains(raycaster))
			{
				return;
			}
			PanelInputModule._raycasters.Add(raycaster);
		}

		public static void UnregisterRaycaster(PanelRaycaster raycaster)
		{
			if (!PanelInputModule._raycasters.Contains(raycaster))
			{
				return;
			}
			PanelInputModule._raycasters.Remove(raycaster);
		}

		internal void SetDebugInterface(Interface debugInterface)
		{
			this._debugInterface = debugInterface;
		}

		protected override void Awake()
		{
			GameObject gameObject = new GameObject("rayHelper");
			this.rayTransform = gameObject.transform;
			this.rayTransform.SetParent(base.transform);
		}

		public override bool ShouldActivateModule()
		{
			return false;
		}

		public override bool IsModuleSupported()
		{
			return false;
		}

		private void Update()
		{
			if (this._debugInterface && !this._debugInterface.Visibility)
			{
				return;
			}
			this.Process();
		}

		private bool Raycast(PointerEventData data, out RaycastResult raycast)
		{
			foreach (PanelRaycaster panelRaycaster in PanelInputModule._raycasters)
			{
				if (panelRaycaster.IsValid)
				{
					panelRaycaster.RaycastOnRaycastableGraphics(data, this.m_RaycastResultCache);
				}
			}
			this.m_RaycastResultCache.Sort(PanelInputModule._comparer);
			raycast = BaseInputModule.FindFirstRaycast(this.m_RaycastResultCache);
			data.pointerCurrentRaycast = raycast;
			this.m_RaycastResultCache.Clear();
			return raycast.isValid;
		}

		private PointerInputModule.MouseState GetMouseStateFromRaycast(OVRInput.Controller controller, Transform rayOrigin)
		{
			if (this.m_Cursor)
			{
				this.m_Cursor.SetCursorRay(rayOrigin);
			}
			OVRPointerEventData ovrpointerEventData;
			base.GetPointerData(-1, out ovrpointerEventData, true);
			ovrpointerEventData.Reset();
			ovrpointerEventData.worldSpaceRay = new Ray(rayOrigin.position, rayOrigin.forward);
			ovrpointerEventData.scrollDelta = base.GetExtraScrollDelta();
			ovrpointerEventData.button = PointerEventData.InputButton.Left;
			ovrpointerEventData.useDragThreshold = true;
			RaycastResult raycastResult;
			if (this.Raycast(ovrpointerEventData, out raycastResult))
			{
				PanelRaycaster panelRaycaster = raycastResult.module as PanelRaycaster;
				if (panelRaycaster)
				{
					ovrpointerEventData.position = panelRaycaster.GetScreenPosition(raycastResult);
					RectTransform rectTransform;
					if (this.m_Cursor && raycastResult.gameObject.TryGetComponent<RectTransform>(out rectTransform))
					{
						Vector3 worldPosition = raycastResult.worldPosition;
						Vector3 rectTransformNormal = OVRInputModule.GetRectTransformNormal(rectTransform);
						this.m_Cursor.SetCursorStartDest(rayOrigin.position, worldPosition, rectTransformNormal);
					}
				}
			}
			OVRPointerEventData ovrpointerEventData2;
			base.GetPointerData(-2, out ovrpointerEventData2, true);
			base.CopyFromTo(ovrpointerEventData, ovrpointerEventData2);
			ovrpointerEventData2.button = PointerEventData.InputButton.Right;
			OVRPointerEventData ovrpointerEventData3;
			base.GetPointerData(-3, out ovrpointerEventData3, true);
			base.CopyFromTo(ovrpointerEventData, ovrpointerEventData3);
			ovrpointerEventData3.button = PointerEventData.InputButton.Middle;
			PointerEventData.FramePressState framePressState = PanelInputModule.ComputeControllerState(controller);
			Meta.XR.ImmersiveDebugger.UserInterface.Generic.Cursor cursor = this.m_Cursor as Meta.XR.ImmersiveDebugger.UserInterface.Generic.Cursor;
			if (cursor != null)
			{
				cursor.SetClickState(framePressState);
			}
			this.m_MouseState.SetButtonState(PointerEventData.InputButton.Left, framePressState, ovrpointerEventData);
			this.m_MouseState.SetButtonState(PointerEventData.InputButton.Right, PointerEventData.FramePressState.NotChanged, ovrpointerEventData2);
			this.m_MouseState.SetButtonState(PointerEventData.InputButton.Middle, PointerEventData.FramePressState.NotChanged, ovrpointerEventData3);
			return this.m_MouseState;
		}

		public override void Process()
		{
			PanelInputModule.Processing = true;
			this._controller = PanelInputModule.ChooseBestController(this._controller);
			this.UpdateRayTransform(this.rayTransform, this._controller);
			base.ProcessMouseEvent(this.GetMouseStateFromRaycast(this._controller, this.rayTransform));
			this._objectsHitThisFrame.Clear();
			PanelInputModule.Processing = false;
		}

		private static PointerEventData.FramePressState ComputeControllerState(OVRInput.Controller controller)
		{
			OVRInput.Button clickButton = RuntimeSettings.Instance.ClickButton;
			bool down = OVRInput.GetDown(clickButton, controller);
			bool up = OVRInput.GetUp(clickButton, controller);
			if (down && up)
			{
				return PointerEventData.FramePressState.PressedAndReleased;
			}
			if (down)
			{
				return PointerEventData.FramePressState.Pressed;
			}
			if (up)
			{
				return PointerEventData.FramePressState.Released;
			}
			return PointerEventData.FramePressState.NotChanged;
		}

		private static OVRInput.Controller ChooseBestController(OVRInput.Controller previousController)
		{
			OVRInput.Controller controller = previousController;
			OVRInput.Controller activeControllerForHand = OVRInput.GetActiveControllerForHand(OVRInput.Handedness.LeftHanded);
			OVRInput.Controller activeControllerForHand2 = OVRInput.GetActiveControllerForHand(OVRInput.Handedness.RightHanded);
			if (controller == OVRInput.Controller.None || (controller != activeControllerForHand && controller != activeControllerForHand2))
			{
				if (activeControllerForHand2 == OVRInput.Controller.None)
				{
					controller = activeControllerForHand;
				}
				else if (activeControllerForHand == OVRInput.Controller.None)
				{
					controller = activeControllerForHand2;
				}
				else
				{
					controller = ((OVRInput.GetDominantHand() == OVRInput.Handedness.LeftHanded) ? activeControllerForHand : activeControllerForHand2);
				}
			}
			if (controller != activeControllerForHand && OVRInput.Get(OVRInput.Button.Any, activeControllerForHand))
			{
				controller = activeControllerForHand;
			}
			if (controller != activeControllerForHand2 && OVRInput.Get(OVRInput.Button.Any, activeControllerForHand2))
			{
				controller = activeControllerForHand2;
			}
			if (controller == OVRInput.Controller.None)
			{
				controller = OVRInput.Controller.RTouch;
			}
			return controller;
		}

		private void UpdateRayTransform(Transform rayTransform, OVRInput.Controller controller)
		{
			OVRPlugin.Hand hand;
			if (controller != OVRInput.Controller.LHand)
			{
				if (controller != OVRInput.Controller.RHand)
				{
					hand = OVRPlugin.Hand.None;
				}
				else
				{
					hand = OVRPlugin.Hand.HandRight;
				}
			}
			else
			{
				hand = OVRPlugin.Hand.HandLeft;
			}
			OVRPlugin.Hand hand2 = hand;
			if (hand2 != OVRPlugin.Hand.None)
			{
				OVRPlugin.GetHandState(OVRPlugin.Step.Render, hand2, ref PanelInputModule._handState);
			}
			Vector3 vector;
			if (controller != OVRInput.Controller.LHand)
			{
				if (controller == OVRInput.Controller.RHand)
				{
					vector = PanelInputModule._handState.PointerPose.Position.FromFlippedZVector3f();
				}
				else
				{
					vector = OVRInput.GetLocalControllerPosition(controller);
				}
			}
			else
			{
				vector = PanelInputModule._handState.PointerPose.Position.FromFlippedZVector3f();
			}
			Vector3 position = vector;
			Quaternion quaternion;
			if (controller != OVRInput.Controller.LHand)
			{
				if (controller == OVRInput.Controller.RHand)
				{
					quaternion = PanelInputModule._handState.PointerPose.Orientation.FromFlippedZQuatf();
				}
				else
				{
					quaternion = OVRInput.GetLocalControllerRotation(controller);
				}
			}
			else
			{
				quaternion = PanelInputModule._handState.PointerPose.Orientation.FromFlippedZQuatf();
			}
			Quaternion orientation = quaternion;
			OVRPose ovrpose = new OVRPose
			{
				position = position,
				orientation = orientation
			};
			ovrpose = ovrpose.ToWorldSpacePose(this._debugInterface.Camera);
			rayTransform.SetPositionAndRotation(ovrpose.position, ovrpose.orientation);
		}

		internal static bool Processing;

		private Interface _debugInterface;

		private OVRInput.Controller _controller;

		private static OVRPlugin.HandState _handState = default(OVRPlugin.HandState);

		private static readonly List<PanelRaycaster> _raycasters = new List<PanelRaycaster>();

		private static IComparer<RaycastResult> _comparer = new PanelInputModule.RaycastComparer();

		public class RaycastComparer : IComparer<RaycastResult>
		{
			public int Compare(RaycastResult lhs, RaycastResult rhs)
			{
				PanelRaycaster panelRaycaster = lhs.module as PanelRaycaster;
				PanelRaycaster panelRaycaster2 = rhs.module as PanelRaycaster;
				if (panelRaycaster != null && panelRaycaster2 != null && panelRaycaster.sortOrder != panelRaycaster2.sortOrder)
				{
					return panelRaycaster2.sortOrder.CompareTo(panelRaycaster.sortOrder);
				}
				if (lhs.depth != rhs.depth && lhs.module.rootRaycaster == rhs.module.rootRaycaster)
				{
					return rhs.depth.CompareTo(lhs.depth);
				}
				if (lhs.distance != rhs.distance)
				{
					return lhs.distance.CompareTo(rhs.distance);
				}
				return lhs.index.CompareTo(rhs.index);
			}
		}
	}
}
