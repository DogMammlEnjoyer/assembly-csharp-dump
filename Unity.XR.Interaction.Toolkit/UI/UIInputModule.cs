using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
	[DefaultExecutionOrder(-200)]
	public abstract class UIInputModule : BaseInputModule
	{
		public float clickSpeed
		{
			get
			{
				return this.m_ClickSpeed;
			}
			set
			{
				this.m_ClickSpeed = value;
			}
		}

		public float moveDeadzone
		{
			get
			{
				return this.m_MoveDeadzone;
			}
			set
			{
				this.m_MoveDeadzone = value;
			}
		}

		public float repeatDelay
		{
			get
			{
				return this.m_RepeatDelay;
			}
			set
			{
				this.m_RepeatDelay = value;
			}
		}

		public float repeatRate
		{
			get
			{
				return this.m_RepeatRate;
			}
			set
			{
				this.m_RepeatRate = value;
			}
		}

		public float trackedDeviceDragThresholdMultiplier
		{
			get
			{
				return this.m_TrackedDeviceDragThresholdMultiplier;
			}
			set
			{
				this.m_TrackedDeviceDragThresholdMultiplier = value;
			}
		}

		public float trackedScrollDeltaMultiplier
		{
			get
			{
				return this.m_TrackedScrollDeltaMultiplier;
			}
			set
			{
				this.m_TrackedScrollDeltaMultiplier = value;
			}
		}

		public bool bypassUIToolkitEvents
		{
			get
			{
				return this.m_BypassUIToolkitEvents;
			}
			set
			{
				this.m_BypassUIToolkitEvents = value;
			}
		}

		public Camera uiCamera
		{
			get
			{
				if (this.m_UICamera != null)
				{
					return this.m_UICamera;
				}
				if (this.m_MainCameraCache == null || !this.m_MainCameraCache.isActiveAndEnabled)
				{
					this.m_MainCameraCache = Camera.main;
				}
				return this.m_MainCameraCache;
			}
			set
			{
				this.m_UICamera = value;
			}
		}

		protected virtual void Update()
		{
			if (base.eventSystem.IsActive() && base.eventSystem.currentInputModule == this && base.eventSystem == EventSystem.current)
			{
				this.DoProcess();
			}
		}

		protected virtual void DoProcess()
		{
			this.SendUpdateEventToSelectedObject();
		}

		public override void Process()
		{
		}

		protected bool SendUpdateEventToSelectedObject()
		{
			GameObject currentSelectedGameObject = base.eventSystem.currentSelectedGameObject;
			if (currentSelectedGameObject == null)
			{
				return false;
			}
			BaseEventData baseEventData = this.GetBaseEventData();
			Action<GameObject, BaseEventData> action = this.updateSelected;
			if (action != null)
			{
				action(currentSelectedGameObject, baseEventData);
			}
			ExecuteEvents.Execute<IUpdateSelectedHandler>(currentSelectedGameObject, baseEventData, ExecuteEvents.updateSelectedHandler);
			return baseEventData.used;
		}

		public override void ActivateModule()
		{
			base.ActivateModule();
			if (this.bypassUIToolkitEvents)
			{
				EventSystem.SetUITookitEventSystemOverride(base.eventSystem, false, false);
			}
			GameObject gameObject = base.eventSystem.currentSelectedGameObject;
			if (gameObject == null)
			{
				gameObject = base.eventSystem.firstSelectedGameObject;
			}
			base.eventSystem.SetSelectedGameObject(gameObject, this.GetBaseEventData());
		}

		public GameObject GetCurrentGameObject(int pointerId)
		{
			if (pointerId < 0)
			{
				foreach (TrackedDeviceEventData trackedDeviceEventData in this.m_TrackedDeviceEventByPointerId.Values)
				{
					if (trackedDeviceEventData != null && trackedDeviceEventData.pointerEnter != null)
					{
						return trackedDeviceEventData.pointerEnter;
					}
				}
				using (Dictionary<int, PointerEventData>.ValueCollection.Enumerator enumerator2 = this.m_PointerEventByPointerId.Values.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						PointerEventData pointerEventData = enumerator2.Current;
						if (pointerEventData != null && pointerEventData.pointerEnter != null)
						{
							return pointerEventData.pointerEnter;
						}
					}
					goto IL_E6;
				}
			}
			TrackedDeviceEventData trackedDeviceEventData2;
			PointerEventData pointerEventData2;
			if (this.m_TrackedDeviceEventByPointerId.TryGetValue(pointerId, out trackedDeviceEventData2))
			{
				if (trackedDeviceEventData2 == null)
				{
					return null;
				}
				return trackedDeviceEventData2.pointerEnter;
			}
			else if (this.m_PointerEventByPointerId.TryGetValue(pointerId, out pointerEventData2))
			{
				if (pointerEventData2 == null)
				{
					return null;
				}
				return pointerEventData2.pointerEnter;
			}
			IL_E6:
			return null;
		}

		public override bool IsPointerOverGameObject(int pointerId)
		{
			return this.GetCurrentGameObject(pointerId) != null;
		}

		private RaycastResult PerformRaycast(PointerEventData eventData)
		{
			if (eventData == null)
			{
				throw new ArgumentNullException("eventData");
			}
			base.eventSystem.RaycastAll(eventData, this.m_RaycastResultCache);
			Action<PointerEventData, List<RaycastResult>> action = this.finalizeRaycastResults;
			if (action != null)
			{
				action(eventData, this.m_RaycastResultCache);
			}
			RaycastResult result = BaseInputModule.FindFirstRaycast(this.m_RaycastResultCache);
			this.m_RaycastResultCache.Clear();
			return result;
		}

		private protected void ProcessPointerState(ref PointerModel pointerState)
		{
			if (!pointerState.changedThisFrame)
			{
				return;
			}
			PointerEventData orCreateCachedPointerEvent = this.GetOrCreateCachedPointerEvent(pointerState.pointerId);
			orCreateCachedPointerEvent.Reset();
			pointerState.CopyTo(orCreateCachedPointerEvent);
			orCreateCachedPointerEvent.pointerCurrentRaycast = this.PerformRaycast(orCreateCachedPointerEvent);
			MouseButtonModel mouseButtonModel = pointerState.leftButton;
			orCreateCachedPointerEvent.button = PointerEventData.InputButton.Left;
			mouseButtonModel.CopyTo(orCreateCachedPointerEvent);
			this.ProcessPointerButton(mouseButtonModel.lastFrameDelta, orCreateCachedPointerEvent, false);
			this.ProcessPointerMovement(orCreateCachedPointerEvent);
			this.ProcessScrollWheel(orCreateCachedPointerEvent);
			pointerState.CopyFrom(orCreateCachedPointerEvent);
			this.ProcessPointerButtonDrag(orCreateCachedPointerEvent, UIPointerType.MouseOrPen, 1f);
			mouseButtonModel.CopyFrom(orCreateCachedPointerEvent);
			pointerState.leftButton = mouseButtonModel;
			mouseButtonModel = pointerState.rightButton;
			orCreateCachedPointerEvent.button = PointerEventData.InputButton.Right;
			mouseButtonModel.CopyTo(orCreateCachedPointerEvent);
			this.ProcessPointerButton(mouseButtonModel.lastFrameDelta, orCreateCachedPointerEvent, false);
			this.ProcessPointerButtonDrag(orCreateCachedPointerEvent, UIPointerType.MouseOrPen, 1f);
			mouseButtonModel.CopyFrom(orCreateCachedPointerEvent);
			pointerState.rightButton = mouseButtonModel;
			mouseButtonModel = pointerState.middleButton;
			orCreateCachedPointerEvent.button = PointerEventData.InputButton.Middle;
			mouseButtonModel.CopyTo(orCreateCachedPointerEvent);
			this.ProcessPointerButton(mouseButtonModel.lastFrameDelta, orCreateCachedPointerEvent, false);
			this.ProcessPointerButtonDrag(orCreateCachedPointerEvent, UIPointerType.MouseOrPen, 1f);
			mouseButtonModel.CopyFrom(orCreateCachedPointerEvent);
			pointerState.middleButton = mouseButtonModel;
			pointerState.OnFrameFinished();
		}

		private void ProcessPointerMovement(PointerEventData eventData)
		{
			GameObject gameObject = eventData.pointerCurrentRaycast.gameObject;
			bool flag = eventData.IsPointerMoving();
			if (flag)
			{
				for (int i = 0; i < eventData.hovered.Count; i++)
				{
					Action<GameObject, PointerEventData> action = this.pointerMove;
					if (action != null)
					{
						action(eventData.hovered[i], eventData);
					}
					ExecuteEvents.Execute<IPointerMoveHandler>(eventData.hovered[i], eventData, ExecuteEvents.pointerMoveHandler);
				}
			}
			if (gameObject == null || eventData.pointerEnter == null)
			{
				foreach (GameObject gameObject2 in eventData.hovered)
				{
					Action<GameObject, PointerEventData> action2 = this.pointerExit;
					if (action2 != null)
					{
						action2(gameObject2, eventData);
					}
					ExecuteEvents.Execute<IPointerExitHandler>(gameObject2, eventData, ExecuteEvents.pointerExitHandler);
				}
				eventData.hovered.Clear();
				if (gameObject == null)
				{
					eventData.pointerEnter = null;
					return;
				}
			}
			if (eventData.pointerEnter == gameObject)
			{
				return;
			}
			GameObject gameObject3 = BaseInputModule.FindCommonRoot(eventData.pointerEnter, gameObject);
			if (eventData.pointerEnter != null)
			{
				Transform transform = eventData.pointerEnter.transform;
				while (transform != null && (!(gameObject3 != null) || !(gameObject3.transform == transform)))
				{
					GameObject gameObject4 = transform.gameObject;
					Action<GameObject, PointerEventData> action3 = this.pointerExit;
					if (action3 != null)
					{
						action3(gameObject4, eventData);
					}
					ExecuteEvents.Execute<IPointerExitHandler>(gameObject4, eventData, ExecuteEvents.pointerExitHandler);
					eventData.hovered.Remove(gameObject4);
					transform = transform.parent;
				}
			}
			eventData.pointerEnter = gameObject;
			if (gameObject != null)
			{
				Transform transform2 = gameObject.transform;
				while (transform2 != null && transform2.gameObject != gameObject3)
				{
					GameObject gameObject5 = transform2.gameObject;
					Action<GameObject, PointerEventData> action4 = this.pointerEnter;
					if (action4 != null)
					{
						action4(gameObject5, eventData);
					}
					ExecuteEvents.Execute<IPointerEnterHandler>(gameObject5, eventData, ExecuteEvents.pointerEnterHandler);
					if (flag)
					{
						Action<GameObject, PointerEventData> action5 = this.pointerMove;
						if (action5 != null)
						{
							action5(gameObject5, eventData);
						}
						ExecuteEvents.Execute<IPointerMoveHandler>(gameObject5, eventData, ExecuteEvents.pointerMoveHandler);
					}
					eventData.hovered.Add(gameObject5);
					transform2 = transform2.parent;
				}
			}
		}

		private void ProcessPointerButton(ButtonDeltaState mouseButtonChanges, PointerEventData eventData, bool clickOnDown = false)
		{
			GameObject gameObject = eventData.pointerCurrentRaycast.gameObject;
			if ((mouseButtonChanges & ButtonDeltaState.Pressed) != ButtonDeltaState.NoChange)
			{
				eventData.eligibleForClick = true;
				eventData.delta = Vector2.zero;
				eventData.dragging = false;
				eventData.pressPosition = eventData.position;
				eventData.pointerPressRaycast = eventData.pointerCurrentRaycast;
				eventData.useDragThreshold = true;
				GameObject eventHandler = ExecuteEvents.GetEventHandler<ISelectHandler>(gameObject);
				if (base.eventSystem.currentSelectedGameObject != null && eventHandler != base.eventSystem.currentSelectedGameObject)
				{
					base.eventSystem.SetSelectedGameObject(null, eventData);
				}
				Action<GameObject, PointerEventData> action = this.pointerDown;
				if (action != null)
				{
					action(gameObject, eventData);
				}
				GameObject gameObject2 = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>(gameObject, eventData, ExecuteEvents.pointerDownHandler);
				if (gameObject2 == null)
				{
					gameObject2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
				}
				float unscaledTime = Time.unscaledTime;
				if (gameObject2 == eventData.lastPress && unscaledTime - eventData.clickTime < this.m_ClickSpeed)
				{
					int clickCount = eventData.clickCount + 1;
					eventData.clickCount = clickCount;
				}
				else
				{
					eventData.clickCount = 1;
				}
				eventData.clickTime = unscaledTime;
				eventData.pointerPress = gameObject2;
				eventData.rawPointerPress = gameObject;
				GameObject eventHandler2 = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject);
				eventData.pointerDrag = eventHandler2;
				if (eventHandler2 != null)
				{
					Action<GameObject, PointerEventData> action2 = this.initializePotentialDrag;
					if (action2 != null)
					{
						action2(eventHandler2, eventData);
					}
					ExecuteEvents.Execute<IInitializePotentialDragHandler>(eventHandler2, eventData, ExecuteEvents.initializePotentialDrag);
				}
				GameObject pointerPress = eventData.pointerPress;
				if (clickOnDown && UIInputModule.CanTargetClickOnDown(pointerPress))
				{
					mouseButtonChanges = ButtonDeltaState.Released;
				}
			}
			if ((mouseButtonChanges & ButtonDeltaState.Released) != ButtonDeltaState.NoChange)
			{
				GameObject pointerPress2 = eventData.pointerPress;
				Action<GameObject, PointerEventData> action3 = this.pointerUp;
				if (action3 != null)
				{
					action3(pointerPress2, eventData);
				}
				ExecuteEvents.Execute<IPointerUpHandler>(pointerPress2, eventData, ExecuteEvents.pointerUpHandler);
				GameObject eventHandler3 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
				GameObject pointerDrag = eventData.pointerDrag;
				if (pointerPress2 == eventHandler3 && eventData.eligibleForClick)
				{
					Action<GameObject, PointerEventData> action4 = this.pointerClick;
					if (action4 != null)
					{
						action4(pointerPress2, eventData);
					}
					ExecuteEvents.Execute<IPointerClickHandler>(pointerPress2, eventData, ExecuteEvents.pointerClickHandler);
				}
				else if (eventData.dragging && pointerDrag != null)
				{
					Action<GameObject, PointerEventData> action5 = this.drop;
					if (action5 != null)
					{
						action5(gameObject, eventData);
					}
					ExecuteEvents.ExecuteHierarchy<IDropHandler>(gameObject, eventData, ExecuteEvents.dropHandler);
				}
				eventData.eligibleForClick = false;
				eventData.pointerPress = null;
				eventData.rawPointerPress = null;
				if (eventData.dragging && pointerDrag != null)
				{
					Action<GameObject, PointerEventData> action6 = this.endDrag;
					if (action6 != null)
					{
						action6(pointerDrag, eventData);
					}
					ExecuteEvents.Execute<IEndDragHandler>(pointerDrag, eventData, ExecuteEvents.endDragHandler);
				}
				eventData.dragging = false;
				eventData.pointerDrag = null;
			}
		}

		private void ProcessPointerButtonDrag(PointerEventData eventData, UIPointerType pointerType, float pixelDragThresholdMultiplier = 1f)
		{
			if (!eventData.IsPointerMoving() || (pointerType == UIPointerType.MouseOrPen && Cursor.lockState == CursorLockMode.Locked) || eventData.pointerDrag == null)
			{
				return;
			}
			if (!eventData.dragging)
			{
				float num = (float)base.eventSystem.pixelDragThreshold * pixelDragThresholdMultiplier;
				if (!eventData.useDragThreshold || (eventData.pressPosition - eventData.position).sqrMagnitude >= num * num)
				{
					GameObject pointerDrag = eventData.pointerDrag;
					Action<GameObject, PointerEventData> action = this.beginDrag;
					if (action != null)
					{
						action(pointerDrag, eventData);
					}
					ExecuteEvents.Execute<IBeginDragHandler>(pointerDrag, eventData, ExecuteEvents.beginDragHandler);
					eventData.dragging = true;
				}
			}
			if (eventData.dragging)
			{
				GameObject pointerPress = eventData.pointerPress;
				if (pointerPress != eventData.pointerDrag)
				{
					Action<GameObject, PointerEventData> action2 = this.pointerUp;
					if (action2 != null)
					{
						action2(pointerPress, eventData);
					}
					ExecuteEvents.Execute<IPointerUpHandler>(pointerPress, eventData, ExecuteEvents.pointerUpHandler);
					eventData.eligibleForClick = false;
					eventData.pointerPress = null;
					eventData.rawPointerPress = null;
				}
				Action<GameObject, PointerEventData> action3 = this.drag;
				if (action3 != null)
				{
					action3(eventData.pointerDrag, eventData);
				}
				ExecuteEvents.Execute<IDragHandler>(eventData.pointerDrag, eventData, ExecuteEvents.dragHandler);
			}
		}

		private void ProcessScrollWheel(PointerEventData eventData)
		{
			if (!Mathf.Approximately(eventData.scrollDelta.sqrMagnitude, 0f))
			{
				GameObject eventHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(eventData.pointerEnter);
				Action<GameObject, PointerEventData> action = this.scroll;
				if (action != null)
				{
					action(eventHandler, eventData);
				}
				ExecuteEvents.ExecuteHierarchy<IScrollHandler>(eventHandler, eventData, ExecuteEvents.scrollHandler);
			}
		}

		private protected void ProcessTrackedDevice(ref TrackedDeviceModel deviceState, bool force = false)
		{
			if (!deviceState.changedThisFrame && !force)
			{
				return;
			}
			TrackedDeviceEventData orCreateCachedTrackedDeviceEvent = this.GetOrCreateCachedTrackedDeviceEvent(deviceState.pointerId);
			orCreateCachedTrackedDeviceEvent.Reset();
			deviceState.CopyTo(orCreateCachedTrackedDeviceEvent);
			orCreateCachedTrackedDeviceEvent.scrollDelta *= this.m_TrackedScrollDeltaMultiplier;
			orCreateCachedTrackedDeviceEvent.button = PointerEventData.InputButton.Left;
			Vector2 position = orCreateCachedTrackedDeviceEvent.position;
			Vector2 delta = orCreateCachedTrackedDeviceEvent.delta;
			orCreateCachedTrackedDeviceEvent.position = new Vector2(-1f, -1f);
			orCreateCachedTrackedDeviceEvent.delta = Vector2.zero;
			orCreateCachedTrackedDeviceEvent.pointerCurrentRaycast = this.PerformRaycast(orCreateCachedTrackedDeviceEvent);
			orCreateCachedTrackedDeviceEvent.position = position;
			orCreateCachedTrackedDeviceEvent.delta = delta;
			Camera camera;
			if (this.TryGetCamera(orCreateCachedTrackedDeviceEvent, out camera))
			{
				Vector2 vector;
				if (orCreateCachedTrackedDeviceEvent.pointerCurrentRaycast.isValid)
				{
					vector = camera.WorldToScreenPoint(orCreateCachedTrackedDeviceEvent.pointerCurrentRaycast.worldPosition);
					if ((deviceState.selectDelta & ButtonDeltaState.Pressed) != ButtonDeltaState.NoChange)
					{
						orCreateCachedTrackedDeviceEvent.pressWorldPosition = orCreateCachedTrackedDeviceEvent.pointerCurrentRaycast.worldPosition;
					}
				}
				else
				{
					Vector3 position2 = (orCreateCachedTrackedDeviceEvent.rayPoints.Count > 0) ? orCreateCachedTrackedDeviceEvent.rayPoints[orCreateCachedTrackedDeviceEvent.rayPoints.Count - 1] : Vector3.zero;
					vector = camera.WorldToScreenPoint(position2);
					orCreateCachedTrackedDeviceEvent.position = vector;
				}
				Vector2 delta2 = vector - orCreateCachedTrackedDeviceEvent.position;
				orCreateCachedTrackedDeviceEvent.position = vector;
				orCreateCachedTrackedDeviceEvent.delta = delta2;
				this.ProcessPointerButton(deviceState.selectDelta, orCreateCachedTrackedDeviceEvent, deviceState.clickOnDown);
				this.ProcessPointerMovement(orCreateCachedTrackedDeviceEvent);
				this.ProcessScrollWheel(orCreateCachedTrackedDeviceEvent);
				if (orCreateCachedTrackedDeviceEvent.pressPosition != Vector2.zero)
				{
					orCreateCachedTrackedDeviceEvent.pressPosition = camera.WorldToScreenPoint(orCreateCachedTrackedDeviceEvent.pressWorldPosition);
				}
				this.ProcessPointerButtonDrag(orCreateCachedTrackedDeviceEvent, UIPointerType.Tracked, this.m_TrackedDeviceDragThresholdMultiplier);
				Object pointerTarget = deviceState.implementationData.pointerTarget;
				deviceState.CopyFrom(orCreateCachedTrackedDeviceEvent);
				GameObject pointerTarget2 = deviceState.implementationData.pointerTarget;
				if (pointerTarget != pointerTarget2)
				{
					if (pointerTarget2 != null)
					{
						ISelectHandler componentInParent = pointerTarget2.GetComponentInParent<ISelectHandler>();
						IScrollHandler componentInParent2 = pointerTarget2.GetComponentInParent<IScrollHandler>();
						Component component = componentInParent as Component;
						deviceState.selectableObject = ((component != null) ? component.gameObject : null);
						deviceState.isScrollable = (componentInParent2 != null);
					}
					else
					{
						deviceState.selectableObject = null;
						deviceState.isScrollable = false;
					}
				}
			}
			deviceState.OnFrameFinished();
		}

		private bool TryGetCamera(PointerEventData eventData, out Camera screenPointCamera)
		{
			screenPointCamera = this.uiCamera;
			if (screenPointCamera != null)
			{
				return true;
			}
			BaseRaycaster module = eventData.pointerCurrentRaycast.module;
			if (module != null)
			{
				screenPointCamera = module.eventCamera;
				return screenPointCamera != null;
			}
			return false;
		}

		private protected void ProcessNavigationState(ref NavigationModel navigationState)
		{
			bool flag = this.SendUpdateEventToSelectedObject();
			if (!base.eventSystem.sendNavigationEvents)
			{
				return;
			}
			NavigationModel.ImplementationData implementationData = navigationState.implementationData;
			GameObject currentSelectedGameObject = base.eventSystem.currentSelectedGameObject;
			Vector2 vector = navigationState.move;
			if (!flag && (!Mathf.Approximately(vector.x, 0f) || !Mathf.Approximately(vector.y, 0f)))
			{
				float unscaledTime = Time.unscaledTime;
				MoveDirection moveDirection = MoveDirection.None;
				if (vector.sqrMagnitude > this.m_MoveDeadzone * this.m_MoveDeadzone)
				{
					if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
					{
						moveDirection = ((vector.x > 0f) ? MoveDirection.Right : MoveDirection.Left);
					}
					else
					{
						moveDirection = ((vector.y > 0f) ? MoveDirection.Up : MoveDirection.Down);
					}
				}
				if (moveDirection != implementationData.lastMoveDirection)
				{
					implementationData.consecutiveMoveCount = 0;
				}
				if (moveDirection != MoveDirection.None)
				{
					bool flag2 = true;
					if (implementationData.consecutiveMoveCount != 0)
					{
						if (implementationData.consecutiveMoveCount > 1)
						{
							flag2 = (unscaledTime > implementationData.lastMoveTime + this.m_RepeatRate);
						}
						else
						{
							flag2 = (unscaledTime > implementationData.lastMoveTime + this.m_RepeatDelay);
						}
					}
					if (flag2)
					{
						AxisEventData orCreateCachedAxisEvent = this.GetOrCreateCachedAxisEvent();
						orCreateCachedAxisEvent.Reset();
						orCreateCachedAxisEvent.moveVector = vector;
						orCreateCachedAxisEvent.moveDir = moveDirection;
						Action<GameObject, AxisEventData> action = this.move;
						if (action != null)
						{
							action(currentSelectedGameObject, orCreateCachedAxisEvent);
						}
						ExecuteEvents.Execute<IMoveHandler>(currentSelectedGameObject, orCreateCachedAxisEvent, ExecuteEvents.moveHandler);
						flag = orCreateCachedAxisEvent.used;
						int consecutiveMoveCount = implementationData.consecutiveMoveCount;
						implementationData.consecutiveMoveCount = consecutiveMoveCount + 1;
						implementationData.lastMoveTime = unscaledTime;
						implementationData.lastMoveDirection = moveDirection;
					}
				}
				else
				{
					implementationData.consecutiveMoveCount = 0;
				}
			}
			else
			{
				implementationData.consecutiveMoveCount = 0;
			}
			if (!flag && currentSelectedGameObject != null)
			{
				BaseEventData baseEventData = this.GetBaseEventData();
				if ((navigationState.submitButtonDelta & ButtonDeltaState.Pressed) != ButtonDeltaState.NoChange)
				{
					Action<GameObject, BaseEventData> action2 = this.submit;
					if (action2 != null)
					{
						action2(currentSelectedGameObject, baseEventData);
					}
					ExecuteEvents.Execute<ISubmitHandler>(currentSelectedGameObject, baseEventData, ExecuteEvents.submitHandler);
				}
				if (!baseEventData.used && (navigationState.cancelButtonDelta & ButtonDeltaState.Pressed) != ButtonDeltaState.NoChange)
				{
					Action<GameObject, BaseEventData> action3 = this.cancel;
					if (action3 != null)
					{
						action3(currentSelectedGameObject, baseEventData);
					}
					ExecuteEvents.Execute<ICancelHandler>(currentSelectedGameObject, baseEventData, ExecuteEvents.cancelHandler);
				}
			}
			navigationState.implementationData = implementationData;
			navigationState.OnFrameFinished();
		}

		private protected void RemovePointerEventData(int pointerId)
		{
			if (!this.m_TrackedDeviceEventByPointerId.Remove(pointerId))
			{
				this.m_PointerEventByPointerId.Remove(pointerId);
			}
		}

		private PointerEventData GetOrCreateCachedPointerEvent(int pointerId)
		{
			PointerEventData pointerEventData;
			if (!this.m_PointerEventByPointerId.TryGetValue(pointerId, out pointerEventData))
			{
				pointerEventData = new PointerEventData(base.eventSystem);
				this.m_PointerEventByPointerId.Add(pointerId, pointerEventData);
			}
			return pointerEventData;
		}

		private TrackedDeviceEventData GetOrCreateCachedTrackedDeviceEvent(int pointerId)
		{
			TrackedDeviceEventData trackedDeviceEventData;
			if (!this.m_TrackedDeviceEventByPointerId.TryGetValue(pointerId, out trackedDeviceEventData))
			{
				trackedDeviceEventData = new TrackedDeviceEventData(base.eventSystem);
				this.m_TrackedDeviceEventByPointerId.Add(pointerId, trackedDeviceEventData);
			}
			return trackedDeviceEventData;
		}

		private AxisEventData GetOrCreateCachedAxisEvent()
		{
			AxisEventData axisEventData = this.m_CachedAxisEvent;
			if (axisEventData == null)
			{
				axisEventData = new AxisEventData(base.eventSystem);
				this.m_CachedAxisEvent = axisEventData;
			}
			return axisEventData;
		}

		private static bool CanTargetClickOnDown(GameObject clickOnDownTarget)
		{
			Selectable selectable;
			if (clickOnDownTarget == null || !clickOnDownTarget.TryGetComponent<Selectable>(out selectable))
			{
				return false;
			}
			Transform parent = clickOnDownTarget.transform.parent;
			IScrollHandler scrollHandler = (parent != null) ? parent.GetComponentInParent<IScrollHandler>() : null;
			if (scrollHandler != null)
			{
				ScrollRect scrollRect = scrollHandler as ScrollRect;
				if (scrollRect == null)
				{
					return false;
				}
				if (scrollRect.IsActive())
				{
					if (scrollRect.content == null)
					{
						return false;
					}
					Rect rect = scrollRect.content.rect;
					Rect rect2 = (scrollRect.viewport != null) ? scrollRect.viewport.rect : ((RectTransform)scrollRect.transform).rect;
					if (scrollRect.vertical && rect.height > rect2.height)
					{
						return false;
					}
					if (scrollRect.horizontal && rect.width > rect2.width)
					{
						return false;
					}
				}
			}
			return selectable is Button || selectable is Toggle || selectable is InputField || selectable is Dropdown || (selectable is TMP_InputField || selectable is TMP_Dropdown);
		}

		public event Action<PointerEventData, List<RaycastResult>> finalizeRaycastResults;

		public event Action<GameObject, PointerEventData> pointerEnter;

		public event Action<GameObject, PointerEventData> pointerExit;

		public event Action<GameObject, PointerEventData> pointerDown;

		public event Action<GameObject, PointerEventData> pointerUp;

		public event Action<GameObject, PointerEventData> pointerClick;

		public event Action<GameObject, PointerEventData> pointerMove;

		public event Action<GameObject, PointerEventData> initializePotentialDrag;

		public event Action<GameObject, PointerEventData> beginDrag;

		public event Action<GameObject, PointerEventData> drag;

		public event Action<GameObject, PointerEventData> endDrag;

		public event Action<GameObject, PointerEventData> drop;

		public event Action<GameObject, PointerEventData> scroll;

		public event Action<GameObject, BaseEventData> updateSelected;

		public event Action<GameObject, AxisEventData> move;

		public event Action<GameObject, BaseEventData> submit;

		public event Action<GameObject, BaseEventData> cancel;

		[Header("Configuration")]
		[SerializeField]
		[FormerlySerializedAs("clickSpeed")]
		[Tooltip("The maximum time (in seconds) between two mouse presses for it to be consecutive click.")]
		private float m_ClickSpeed = 0.3f;

		[SerializeField]
		[FormerlySerializedAs("moveDeadzone")]
		[Tooltip("The absolute value required by a move action on either axis required to trigger a move event.")]
		private float m_MoveDeadzone = 0.6f;

		[SerializeField]
		[FormerlySerializedAs("repeatDelay")]
		[Tooltip("The Initial delay (in seconds) between an initial move action and a repeated move action.")]
		private float m_RepeatDelay = 0.5f;

		[FormerlySerializedAs("repeatRate")]
		[SerializeField]
		[Tooltip("The speed (in seconds) that the move action repeats itself once repeating.")]
		private float m_RepeatRate = 0.1f;

		[FormerlySerializedAs("trackedDeviceDragThresholdMultiplier")]
		[SerializeField]
		[Tooltip("Scales the EventSystem.pixelDragThreshold, for tracked devices, to make selection easier.")]
		private float m_TrackedDeviceDragThresholdMultiplier = 2f;

		[SerializeField]
		[Tooltip("Scales the scrollDelta in event data, for tracked devices, to scroll at an expected speed.")]
		private float m_TrackedScrollDeltaMultiplier = 5f;

		[SerializeField]
		[Tooltip("Disables sending events from Event System to UI Toolkit on behalf of this Input Module.")]
		private bool m_BypassUIToolkitEvents;

		private Camera m_UICamera;

		private Camera m_MainCameraCache;

		private AxisEventData m_CachedAxisEvent;

		private readonly Dictionary<int, PointerEventData> m_PointerEventByPointerId = new Dictionary<int, PointerEventData>();

		private readonly Dictionary<int, TrackedDeviceEventData> m_TrackedDeviceEventByPointerId = new Dictionary<int, TrackedDeviceEventData>();
	}
}
