using System;
using System.Collections.Generic;

namespace UnityEngine.EventSystems
{
	[HelpURL("https://developer.oculus.com/documentation/unity/unity-isdk-input-processing/")]
	public class OVRInputModule : PointerInputModule
	{
		protected OVRInputModule()
		{
		}

		protected override void Awake()
		{
			base.Awake();
			OVRInputModule.instance = this;
			for (int i = 0; i < OVRInputModule._pendingInputSources.Count; i++)
			{
				OVRInputModule.InputSource inputSource = OVRInputModule._pendingInputSources[i];
				if (inputSource != null && inputSource.IsValid())
				{
					OVRInputModule.TrackInputSource(inputSource);
				}
			}
			OVRInputModule._pendingInputSources.Clear();
		}

		[Obsolete("Mode is no longer needed on input module as it handles both mouse and keyboard simultaneously.", false)]
		public OVRInputModule.InputMode inputMode
		{
			get
			{
				return OVRInputModule.InputMode.Mouse;
			}
		}

		public bool allowActivationOnMobileDevice
		{
			get
			{
				return this.m_AllowActivationOnMobileDevice;
			}
			set
			{
				this.m_AllowActivationOnMobileDevice = value;
			}
		}

		public float inputActionsPerSecond
		{
			get
			{
				return this.m_InputActionsPerSecond;
			}
			set
			{
				this.m_InputActionsPerSecond = value;
			}
		}

		public string horizontalAxis
		{
			get
			{
				return this.m_HorizontalAxis;
			}
			set
			{
				this.m_HorizontalAxis = value;
			}
		}

		public string verticalAxis
		{
			get
			{
				return this.m_VerticalAxis;
			}
			set
			{
				this.m_VerticalAxis = value;
			}
		}

		public string submitButton
		{
			get
			{
				return this.m_SubmitButton;
			}
			set
			{
				this.m_SubmitButton = value;
			}
		}

		public string cancelButton
		{
			get
			{
				return this.m_CancelButton;
			}
			set
			{
				this.m_CancelButton = value;
			}
		}

		public override void UpdateModule()
		{
		}

		public override bool IsModuleSupported()
		{
			return this.m_AllowActivationOnMobileDevice || Input.mousePresent;
		}

		public override bool ShouldActivateModule()
		{
			base.ShouldActivateModule();
			return false;
		}

		public override void ActivateModule()
		{
			base.ActivateModule();
			GameObject gameObject = base.eventSystem.currentSelectedGameObject;
			if (gameObject == null)
			{
				gameObject = base.eventSystem.firstSelectedGameObject;
			}
			base.eventSystem.SetSelectedGameObject(gameObject, this.GetBaseEventData());
		}

		public override void DeactivateModule()
		{
			base.DeactivateModule();
			this.ClearSelection();
		}

		private bool SendSubmitEventToSelectedObject()
		{
			return !(base.eventSystem.currentSelectedGameObject == null) && this.GetBaseEventData().used;
		}

		private bool AllowMoveEventProcessing(float time)
		{
			return false;
		}

		private Vector2 GetRawMoveVector()
		{
			Vector2 zero = Vector2.zero;
			zero.x = Input.GetAxisRaw(this.m_HorizontalAxis);
			zero.y = Input.GetAxisRaw(this.m_VerticalAxis);
			if (Input.GetButtonDown(this.m_HorizontalAxis))
			{
				if (zero.x < 0f)
				{
					zero.x = -1f;
				}
				if (zero.x > 0f)
				{
					zero.x = 1f;
				}
			}
			if (Input.GetButtonDown(this.m_VerticalAxis))
			{
				if (zero.y < 0f)
				{
					zero.y = -1f;
				}
				if (zero.y > 0f)
				{
					zero.y = 1f;
				}
			}
			return zero;
		}

		private bool SendMoveEventToSelectedObject()
		{
			float unscaledTime = Time.unscaledTime;
			if (!this.AllowMoveEventProcessing(unscaledTime))
			{
				return false;
			}
			Vector2 rawMoveVector = this.GetRawMoveVector();
			AxisEventData axisEventData = this.GetAxisEventData(rawMoveVector.x, rawMoveVector.y, 0.6f);
			if (!Mathf.Approximately(axisEventData.moveVector.x, 0f) || !Mathf.Approximately(axisEventData.moveVector.y, 0f))
			{
				ExecuteEvents.Execute<IMoveHandler>(base.eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
			}
			this.m_NextAction = unscaledTime + 1f / this.m_InputActionsPerSecond;
			return axisEventData.used;
		}

		private bool SendUpdateEventToSelectedObject()
		{
			if (base.eventSystem.currentSelectedGameObject == null)
			{
				return false;
			}
			BaseEventData baseEventData = this.GetBaseEventData();
			ExecuteEvents.Execute<IUpdateSelectedHandler>(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.updateSelectedHandler);
			return baseEventData.used;
		}

		private void ProcessMousePress(PointerInputModule.MouseButtonEventData data)
		{
			PointerEventData buttonData = data.buttonData;
			GameObject gameObject = buttonData.pointerCurrentRaycast.gameObject;
			if (data.PressedThisFrame())
			{
				buttonData.eligibleForClick = true;
				buttonData.delta = Vector2.zero;
				buttonData.dragging = false;
				buttonData.useDragThreshold = true;
				buttonData.pressPosition = buttonData.position;
				buttonData.IsVRPointer();
				buttonData.pointerPressRaycast = buttonData.pointerCurrentRaycast;
				base.DeselectIfSelectionChanged(gameObject, buttonData);
				GameObject gameObject2 = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>(gameObject, buttonData, ExecuteEvents.pointerDownHandler);
				if (gameObject2 == null)
				{
					gameObject2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
				}
				if (!this._objectsHitThisFrame.Contains(gameObject2))
				{
					this._objectsHitThisFrame.Add(gameObject2);
					float unscaledTime = Time.unscaledTime;
					if (gameObject2 == buttonData.lastPress)
					{
						if (unscaledTime - buttonData.clickTime < 0.3f)
						{
							PointerEventData pointerEventData = buttonData;
							int clickCount = pointerEventData.clickCount + 1;
							pointerEventData.clickCount = clickCount;
						}
						else
						{
							buttonData.clickCount = 1;
						}
						buttonData.clickTime = unscaledTime;
					}
					else
					{
						buttonData.clickCount = 1;
					}
					buttonData.pointerPress = gameObject2;
					buttonData.rawPointerPress = gameObject;
					buttonData.clickTime = unscaledTime;
					buttonData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject);
					if (buttonData.pointerDrag != null)
					{
						ExecuteEvents.Execute<IInitializePotentialDragHandler>(buttonData.pointerDrag, buttonData, ExecuteEvents.initializePotentialDrag);
					}
				}
			}
			if (data.ReleasedThisFrame())
			{
				ExecuteEvents.Execute<IPointerUpHandler>(buttonData.pointerPress, buttonData, ExecuteEvents.pointerUpHandler);
				GameObject eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
				if (buttonData.pointerPress == eventHandler && buttonData.eligibleForClick)
				{
					ExecuteEvents.Execute<IPointerClickHandler>(buttonData.pointerPress, buttonData, ExecuteEvents.pointerClickHandler);
				}
				else if (buttonData.pointerDrag != null)
				{
					ExecuteEvents.ExecuteHierarchy<IDropHandler>(gameObject, buttonData, ExecuteEvents.dropHandler);
				}
				buttonData.eligibleForClick = false;
				buttonData.pointerPress = null;
				buttonData.rawPointerPress = null;
				if (buttonData.pointerDrag != null && buttonData.dragging)
				{
					ExecuteEvents.Execute<IEndDragHandler>(buttonData.pointerDrag, buttonData, ExecuteEvents.endDragHandler);
				}
				buttonData.dragging = false;
				buttonData.pointerDrag = null;
				if (gameObject != buttonData.pointerEnter)
				{
					base.HandlePointerExitAndEnter(buttonData, null);
					base.HandlePointerExitAndEnter(buttonData, gameObject);
				}
			}
		}

		protected void ProcessMouseEvent(PointerInputModule.MouseState mouseData)
		{
			bool pressed = mouseData.AnyPressesThisFrame();
			bool released = mouseData.AnyReleasesThisFrame();
			PointerInputModule.MouseButtonEventData eventData = mouseData.GetButtonState(PointerEventData.InputButton.Left).eventData;
			if (!OVRInputModule.UseMouse(pressed, released, eventData.buttonData))
			{
				return;
			}
			this.ProcessMousePress(eventData);
			this.ProcessMove(eventData.buttonData);
			this.ProcessDrag(eventData.buttonData);
			this.ProcessMousePress(mouseData.GetButtonState(PointerEventData.InputButton.Right).eventData);
			this.ProcessDrag(mouseData.GetButtonState(PointerEventData.InputButton.Right).eventData.buttonData);
			this.ProcessMousePress(mouseData.GetButtonState(PointerEventData.InputButton.Middle).eventData);
			this.ProcessDrag(mouseData.GetButtonState(PointerEventData.InputButton.Middle).eventData.buttonData);
			if (!Mathf.Approximately(eventData.buttonData.scrollDelta.sqrMagnitude, 0f))
			{
				ExecuteEvents.ExecuteHierarchy<IScrollHandler>(ExecuteEvents.GetEventHandler<IScrollHandler>(eventData.buttonData.pointerCurrentRaycast.gameObject), eventData.buttonData, ExecuteEvents.scrollHandler);
			}
		}

		public override void Process()
		{
			bool flag = this.SendUpdateEventToSelectedObject();
			if (base.eventSystem.sendNavigationEvents)
			{
				if (!flag)
				{
					flag |= this.SendMoveEventToSelectedObject();
				}
				if (!flag)
				{
					this.SendSubmitEventToSelectedObject();
				}
			}
			bool flag2 = false;
			for (int i = this._trackedInputSources.Count - 1; i >= 0; i--)
			{
				if (this._trackedInputSources[i].IsActive())
				{
					flag2 = true;
					PointerInputModule.MouseState mouseStateFromInputSource = this.GetMouseStateFromInputSource(this._trackedInputSources[i], i);
					this.ProcessMouseEvent(mouseStateFromInputSource);
				}
			}
			if (!flag2 && this.rayTransform != null)
			{
				this.ProcessMouseEvent(this.GetMouseStateFromRaycast(this.rayTransform));
			}
			this._objectsHitThisFrame.Clear();
			this.ProcessMouseEvent(this.GetCanvasPointerData());
		}

		private static bool UseMouse(bool pressed, bool released, PointerEventData pointerData)
		{
			return pressed || released || OVRInputModule.IsPointerMoving(pointerData) || pointerData.IsScrolling();
		}

		protected void CopyFromTo(OVRPointerEventData from, OVRPointerEventData to)
		{
			to.position = from.position;
			to.delta = from.delta;
			to.scrollDelta = from.scrollDelta;
			to.pointerCurrentRaycast = from.pointerCurrentRaycast;
			to.pointerEnter = from.pointerEnter;
			to.worldSpaceRay = from.worldSpaceRay;
		}

		protected new void CopyFromTo(PointerEventData from, PointerEventData to)
		{
			to.position = from.position;
			to.delta = from.delta;
			to.scrollDelta = from.scrollDelta;
			to.pointerCurrentRaycast = from.pointerCurrentRaycast;
			to.pointerEnter = from.pointerEnter;
		}

		protected bool GetPointerData(int id, out OVRPointerEventData data, bool create)
		{
			if (!this.m_VRRayPointerData.TryGetValue(id, out data) && create)
			{
				data = new OVRPointerEventData(base.eventSystem)
				{
					pointerId = id
				};
				this.m_VRRayPointerData.Add(id, data);
				return true;
			}
			return false;
		}

		protected new void ClearSelection()
		{
			BaseEventData baseEventData = this.GetBaseEventData();
			foreach (PointerEventData currentPointerData in this.m_PointerData.Values)
			{
				base.HandlePointerExitAndEnter(currentPointerData, null);
			}
			foreach (OVRPointerEventData currentPointerData2 in this.m_VRRayPointerData.Values)
			{
				base.HandlePointerExitAndEnter(currentPointerData2, null);
			}
			this.m_PointerData.Clear();
			base.eventSystem.SetSelectedGameObject(null, baseEventData);
		}

		protected static Vector3 GetRectTransformNormal(RectTransform rectTransform)
		{
			Vector3[] array = new Vector3[4];
			rectTransform.GetWorldCorners(array);
			Vector3 lhs = array[3] - array[0];
			Vector3 rhs = array[1] - array[0];
			rectTransform.GetWorldCorners(array);
			return Vector3.Cross(lhs, rhs).normalized;
		}

		protected virtual PointerInputModule.MouseState GetMouseStateFromInputSource(OVRInputModule.InputSource inputSource, int id)
		{
			Transform pointerRayTransform = inputSource.GetPointerRayTransform();
			OVRPointerEventData ovrpointerEventData;
			this.GetPointerData(id, out ovrpointerEventData, true);
			ovrpointerEventData.Reset();
			OVRInputRayData ovrinputRayData = default(OVRInputRayData);
			if (pointerRayTransform != null)
			{
				ovrpointerEventData.worldSpaceRay = new Ray(pointerRayTransform.position, pointerRayTransform.forward);
				ovrpointerEventData.scrollDelta = this.GetExtraScrollDelta();
				ovrpointerEventData.button = PointerEventData.InputButton.Left;
				ovrpointerEventData.useDragThreshold = true;
				base.eventSystem.RaycastAll(ovrpointerEventData, this.m_RaycastResultCache);
				RaycastResult raycastResult = BaseInputModule.FindFirstRaycast(this.m_RaycastResultCache);
				ovrpointerEventData.pointerCurrentRaycast = raycastResult;
				this.m_RaycastResultCache.Clear();
				ovrinputRayData.IsOverCanvas = raycastResult.isValid;
				if (ovrinputRayData.IsOverCanvas)
				{
					ovrinputRayData.DistanceToCanvas = raycastResult.distance;
					ovrinputRayData.WorldPosition = raycastResult.worldPosition;
					ovrinputRayData.WorldNormal = raycastResult.worldNormal;
				}
				if (this.m_Cursor)
				{
					this.m_Cursor.SetCursorRay(pointerRayTransform);
				}
				OVRRaycaster ovrraycaster = raycastResult.module as OVRRaycaster;
				if (ovrraycaster)
				{
					ovrpointerEventData.position = ovrraycaster.GetScreenPosition(raycastResult);
					RectTransform rectTransform;
					if (this.m_Cursor && raycastResult.gameObject.TryGetComponent<RectTransform>(out rectTransform))
					{
						Vector3 worldPosition = raycastResult.worldPosition;
						Vector3 rectTransformNormal = OVRInputModule.GetRectTransformNormal(rectTransform);
						this.m_Cursor.SetCursorStartDest(pointerRayTransform.position, worldPosition, rectTransformNormal);
					}
				}
				OVRPhysicsRaycaster ovrphysicsRaycaster = raycastResult.module as OVRPhysicsRaycaster;
				if (ovrphysicsRaycaster)
				{
					Vector3 worldPosition2 = raycastResult.worldPosition;
					if (this.performSphereCastForGazepointer)
					{
						List<RaycastResult> list = new List<RaycastResult>();
						ovrphysicsRaycaster.Spherecast(ovrpointerEventData, list, this.m_SpherecastRadius);
						if (list.Count > 0 && list[0].distance < raycastResult.distance)
						{
							worldPosition2 = list[0].worldPosition;
							ovrinputRayData.IsOverCanvas |= list[0].isValid;
							ovrinputRayData.DistanceToCanvas = list[0].distance;
							ovrinputRayData.WorldPosition = list[0].worldPosition;
							ovrinputRayData.WorldNormal = list[0].worldNormal;
						}
					}
					ovrpointerEventData.position = ovrphysicsRaycaster.GetScreenPos(worldPosition2);
				}
			}
			OVRPointerEventData ovrpointerEventData2;
			this.GetPointerData(-2, out ovrpointerEventData2, true);
			this.CopyFromTo(ovrpointerEventData, ovrpointerEventData2);
			ovrpointerEventData2.button = PointerEventData.InputButton.Right;
			OVRPointerEventData ovrpointerEventData3;
			this.GetPointerData(-3, out ovrpointerEventData3, true);
			this.CopyFromTo(ovrpointerEventData, ovrpointerEventData3);
			ovrpointerEventData3.button = PointerEventData.InputButton.Middle;
			PointerEventData.FramePressState stateForMouseButton = PointerEventData.FramePressState.NotChanged;
			bool flag = inputSource.IsPressed();
			bool flag2 = inputSource.IsReleased();
			if (flag)
			{
				if (flag2)
				{
					stateForMouseButton = PointerEventData.FramePressState.PressedAndReleased;
				}
				else
				{
					stateForMouseButton = PointerEventData.FramePressState.Pressed;
				}
			}
			else if (flag2)
			{
				stateForMouseButton = PointerEventData.FramePressState.Released;
			}
			ovrinputRayData.IsActive = flag;
			inputSource.UpdatePointerRay(ovrinputRayData);
			this.m_MouseState.SetButtonState(PointerEventData.InputButton.Left, stateForMouseButton, ovrpointerEventData);
			this.m_MouseState.SetButtonState(PointerEventData.InputButton.Right, PointerEventData.FramePressState.NotChanged, ovrpointerEventData2);
			this.m_MouseState.SetButtonState(PointerEventData.InputButton.Middle, PointerEventData.FramePressState.NotChanged, ovrpointerEventData3);
			return this.m_MouseState;
		}

		protected virtual PointerInputModule.MouseState GetMouseStateFromRaycast(Transform rayOrigin)
		{
			OVRPointerEventData ovrpointerEventData;
			this.GetPointerData(-1, out ovrpointerEventData, true);
			ovrpointerEventData.Reset();
			ovrpointerEventData.worldSpaceRay = new Ray(rayOrigin.position, rayOrigin.forward);
			ovrpointerEventData.scrollDelta = this.GetExtraScrollDelta();
			ovrpointerEventData.button = PointerEventData.InputButton.Left;
			ovrpointerEventData.useDragThreshold = true;
			base.eventSystem.RaycastAll(ovrpointerEventData, this.m_RaycastResultCache);
			RaycastResult raycastResult = BaseInputModule.FindFirstRaycast(this.m_RaycastResultCache);
			ovrpointerEventData.pointerCurrentRaycast = raycastResult;
			this.m_RaycastResultCache.Clear();
			if (this.m_Cursor)
			{
				this.m_Cursor.SetCursorRay(rayOrigin);
			}
			OVRRaycaster ovrraycaster = raycastResult.module as OVRRaycaster;
			if (ovrraycaster)
			{
				ovrpointerEventData.position = ovrraycaster.GetScreenPosition(raycastResult);
				RectTransform rectTransform;
				if (this.m_Cursor && raycastResult.gameObject.TryGetComponent<RectTransform>(out rectTransform))
				{
					Vector3 worldPosition = raycastResult.worldPosition;
					Vector3 rectTransformNormal = OVRInputModule.GetRectTransformNormal(rectTransform);
					this.m_Cursor.SetCursorStartDest(rayOrigin.position, worldPosition, rectTransformNormal);
				}
			}
			OVRPhysicsRaycaster ovrphysicsRaycaster = raycastResult.module as OVRPhysicsRaycaster;
			if (ovrphysicsRaycaster)
			{
				Vector3 worldPosition2 = raycastResult.worldPosition;
				if (this.performSphereCastForGazepointer)
				{
					List<RaycastResult> list = new List<RaycastResult>();
					ovrphysicsRaycaster.Spherecast(ovrpointerEventData, list, this.m_SpherecastRadius);
					if (list.Count > 0 && list[0].distance < raycastResult.distance)
					{
						worldPosition2 = list[0].worldPosition;
					}
				}
				ovrpointerEventData.position = ovrphysicsRaycaster.GetScreenPos(worldPosition2);
				if (this.m_Cursor)
				{
					this.m_Cursor.SetCursorStartDest(rayOrigin.position, worldPosition2, raycastResult.worldNormal);
				}
			}
			OVRPointerEventData ovrpointerEventData2;
			this.GetPointerData(-2, out ovrpointerEventData2, true);
			this.CopyFromTo(ovrpointerEventData, ovrpointerEventData2);
			ovrpointerEventData2.button = PointerEventData.InputButton.Right;
			OVRPointerEventData ovrpointerEventData3;
			this.GetPointerData(-3, out ovrpointerEventData3, true);
			this.CopyFromTo(ovrpointerEventData, ovrpointerEventData3);
			ovrpointerEventData3.button = PointerEventData.InputButton.Middle;
			this.m_MouseState.SetButtonState(PointerEventData.InputButton.Left, this.GetGazeButtonState(), ovrpointerEventData);
			this.m_MouseState.SetButtonState(PointerEventData.InputButton.Right, PointerEventData.FramePressState.NotChanged, ovrpointerEventData2);
			this.m_MouseState.SetButtonState(PointerEventData.InputButton.Middle, PointerEventData.FramePressState.NotChanged, ovrpointerEventData3);
			return this.m_MouseState;
		}

		protected PointerInputModule.MouseState GetCanvasPointerData()
		{
			PointerEventData pointerEventData;
			base.GetPointerData(-1, out pointerEventData, true);
			pointerEventData.Reset();
			pointerEventData.position = Vector2.zero;
			pointerEventData.button = PointerEventData.InputButton.Left;
			if (this.activeGraphicRaycaster)
			{
				this.activeGraphicRaycaster.RaycastPointer(pointerEventData, this.m_RaycastResultCache);
				RaycastResult raycastResult = BaseInputModule.FindFirstRaycast(this.m_RaycastResultCache);
				pointerEventData.pointerCurrentRaycast = raycastResult;
				this.m_RaycastResultCache.Clear();
				OVRRaycaster ovrraycaster = raycastResult.module as OVRRaycaster;
				if (ovrraycaster)
				{
					Vector2 screenPosition = ovrraycaster.GetScreenPosition(raycastResult);
					pointerEventData.delta = screenPosition - pointerEventData.position;
					pointerEventData.position = screenPosition;
				}
			}
			PointerEventData pointerEventData2;
			base.GetPointerData(-2, out pointerEventData2, true);
			this.CopyFromTo(pointerEventData, pointerEventData2);
			pointerEventData2.button = PointerEventData.InputButton.Right;
			PointerEventData pointerEventData3;
			base.GetPointerData(-3, out pointerEventData3, true);
			this.CopyFromTo(pointerEventData, pointerEventData3);
			pointerEventData3.button = PointerEventData.InputButton.Middle;
			return this.m_MouseState;
		}

		private bool ShouldStartDrag(PointerEventData pointerEvent)
		{
			if (!pointerEvent.useDragThreshold)
			{
				return true;
			}
			if (!pointerEvent.IsVRPointer())
			{
				return (pointerEvent.pressPosition - pointerEvent.position).sqrMagnitude >= (float)(base.eventSystem.pixelDragThreshold * base.eventSystem.pixelDragThreshold);
			}
			Vector3 position = pointerEvent.pressEventCamera.transform.position;
			Vector3 normalized = (pointerEvent.pointerPressRaycast.worldPosition - position).normalized;
			Vector3 normalized2 = (pointerEvent.pointerCurrentRaycast.worldPosition - position).normalized;
			return Vector3.Dot(normalized, normalized2) < Mathf.Cos(0.017453292f * this.angleDragThreshold);
		}

		private static bool IsPointerMoving(PointerEventData pointerEvent)
		{
			return pointerEvent.IsVRPointer() || pointerEvent.IsPointerMoving();
		}

		protected Vector2 SwipeAdjustedPosition(Vector2 originalPosition, PointerEventData pointerEvent)
		{
			return originalPosition;
		}

		protected override void ProcessDrag(PointerEventData pointerEvent)
		{
			Vector2 position = pointerEvent.position;
			bool flag = OVRInputModule.IsPointerMoving(pointerEvent);
			if (flag && pointerEvent.pointerDrag != null && !pointerEvent.dragging && this.ShouldStartDrag(pointerEvent))
			{
				if (pointerEvent.IsVRPointer())
				{
					pointerEvent.position = this.SwipeAdjustedPosition(position, pointerEvent);
				}
				ExecuteEvents.Execute<IBeginDragHandler>(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.beginDragHandler);
				pointerEvent.dragging = true;
			}
			if (pointerEvent.dragging && flag && pointerEvent.pointerDrag != null)
			{
				if (pointerEvent.IsVRPointer())
				{
					pointerEvent.position = this.SwipeAdjustedPosition(position, pointerEvent);
				}
				if (pointerEvent.pointerPress != pointerEvent.pointerDrag)
				{
					ExecuteEvents.Execute<IPointerUpHandler>(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);
					pointerEvent.eligibleForClick = false;
					pointerEvent.pointerPress = null;
					pointerEvent.rawPointerPress = null;
				}
				ExecuteEvents.Execute<IDragHandler>(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.dragHandler);
			}
		}

		protected virtual PointerEventData.FramePressState GetGazeButtonState()
		{
			bool down = OVRInput.GetDown(this.joyPadClickButton, OVRInput.Controller.Active);
			bool up = OVRInput.GetUp(this.joyPadClickButton, OVRInput.Controller.Active);
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

		protected Vector2 GetExtraScrollDelta()
		{
			Vector2 result = default(Vector2);
			if (this.useRightStickScroll)
			{
				Vector2 vector = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick, OVRInput.Controller.Active);
				if (Mathf.Abs(vector.x) < this.rightStickDeadZone)
				{
					vector.x = 0f;
				}
				if (Mathf.Abs(vector.y) < this.rightStickDeadZone)
				{
					vector.y = 0f;
				}
				result = vector;
			}
			return result;
		}

		public static void TrackInputSource(OVRInputModule.InputSource hand)
		{
			if (OVRInputModule.instance)
			{
				if (!OVRInputModule.instance._trackedInputSources.Contains(hand))
				{
					OVRInputModule.instance._trackedInputSources.Add(hand);
					return;
				}
			}
			else
			{
				OVRInputModule._pendingInputSources.Add(hand);
			}
		}

		public static void UntrackInputSource(OVRInputModule.InputSource hand)
		{
			if (OVRInputModule.instance)
			{
				OVRInputModule.instance._trackedInputSources.Remove(hand);
				return;
			}
			if (OVRInputModule._pendingInputSources.Contains(hand))
			{
				OVRInputModule._pendingInputSources.Remove(hand);
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			this._trackedInputSources.Clear();
			OVRInputModule._pendingInputSources.Clear();
		}

		public static OVRInputModule instance { get; private set; }

		[Tooltip("Object which points with Z axis. E.g. CentreEyeAnchor from OVRCameraRig")]
		public Transform rayTransform;

		public OVRCursor m_Cursor;

		[Tooltip("Gamepad button to act as gaze click")]
		public OVRInput.Button joyPadClickButton = OVRInput.Button.One;

		[Tooltip("Keyboard button to act as gaze click")]
		public KeyCode gazeClickKey = KeyCode.Space;

		[Header("Physics")]
		[Tooltip("Perform an sphere cast to determine correct depth for gaze pointer")]
		public bool performSphereCastForGazepointer;

		[Header("Gamepad Stick Scroll")]
		[Tooltip("Enable scrolling with the right stick on a gamepad")]
		public bool useRightStickScroll = true;

		[Tooltip("Deadzone for right stick to prevent accidental scrolling")]
		public float rightStickDeadZone = 0.15f;

		[Header("Touchpad Swipe Scroll")]
		[Tooltip("Enable scrolling by swiping the touchpad")]
		public bool useSwipeScroll = true;

		[Tooltip("Minimum trackpad movement in pixels to start swiping")]
		public float swipeDragThreshold = 2f;

		[Tooltip("Distance scrolled when swipe scroll occurs")]
		public float swipeDragScale = 1f;

		[Tooltip("Invert X axis on touchpad")]
		public bool InvertSwipeXAxis;

		[NonSerialized]
		public OVRRaycaster activeGraphicRaycaster;

		[Header("Dragging")]
		[Tooltip("Minimum pointer movement in degrees to start dragging")]
		public float angleDragThreshold = 1f;

		[SerializeField]
		private float m_SpherecastRadius = 1f;

		private float m_NextAction;

		private Vector2 m_LastMousePosition;

		private Vector2 m_MousePosition;

		protected HashSet<GameObject> _objectsHitThisFrame = new HashSet<GameObject>();

		[Header("Standalone Input Module")]
		[SerializeField]
		private string m_HorizontalAxis = "Horizontal";

		[SerializeField]
		private string m_VerticalAxis = "Vertical";

		[SerializeField]
		private string m_SubmitButton = "Submit";

		[SerializeField]
		private string m_CancelButton = "Cancel";

		[SerializeField]
		private float m_InputActionsPerSecond = 10f;

		[SerializeField]
		private bool m_AllowActivationOnMobileDevice;

		protected Dictionary<int, OVRPointerEventData> m_VRRayPointerData = new Dictionary<int, OVRPointerEventData>();

		protected readonly PointerInputModule.MouseState m_MouseState = new PointerInputModule.MouseState();

		private List<OVRInputModule.InputSource> _trackedInputSources = new List<OVRInputModule.InputSource>();

		private static List<OVRInputModule.InputSource> _pendingInputSources = new List<OVRInputModule.InputSource>();

		[Obsolete("Mode is no longer needed on input module as it handles both mouse and keyboard simultaneously.", false)]
		public enum InputMode
		{
			Mouse,
			Buttons
		}

		public interface InputSource
		{
			bool IsPressed();

			bool IsReleased();

			Transform GetPointerRayTransform();

			bool IsValid();

			bool IsActive();

			void UpdatePointerRay(OVRInputRayData rayData);

			OVRPlugin.Hand GetHand();
		}
	}
}
