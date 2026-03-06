using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace UnityEngine.InputSystem.UI
{
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.inputsystem@1.14/manual/UISupport.html#setting-up-ui-input")]
	public class InputSystemUIInputModule : BaseInputModule
	{
		public bool deselectOnBackgroundClick
		{
			get
			{
				return this.m_DeselectOnBackgroundClick;
			}
			set
			{
				this.m_DeselectOnBackgroundClick = value;
			}
		}

		public UIPointerBehavior pointerBehavior
		{
			get
			{
				return this.m_PointerBehavior;
			}
			set
			{
				this.m_PointerBehavior = value;
			}
		}

		public InputSystemUIInputModule.CursorLockBehavior cursorLockBehavior
		{
			get
			{
				return this.m_CursorLockBehavior;
			}
			set
			{
				this.m_CursorLockBehavior = value;
			}
		}

		internal GameObject localMultiPlayerRoot
		{
			get
			{
				return this.m_LocalMultiPlayerRoot;
			}
			set
			{
				this.m_LocalMultiPlayerRoot = value;
			}
		}

		public float scrollDeltaPerTick
		{
			get
			{
				return this.m_ScrollDeltaPerTick;
			}
			set
			{
				this.m_ScrollDeltaPerTick = value;
			}
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

		public override bool IsPointerOverGameObject(int pointerOrTouchId)
		{
			if (InputSystem.isProcessingEvents)
			{
				Debug.LogWarning("Calling IsPointerOverGameObject() from within event processing (such as from InputAction callbacks) will not work as expected; it will query UI state from the last frame");
			}
			int num = -1;
			if (pointerOrTouchId < 0)
			{
				if (this.m_CurrentPointerId != -1)
				{
					num = this.m_CurrentPointerIndex;
				}
				else if (this.m_PointerStates.length > 0)
				{
					num = 0;
				}
			}
			else
			{
				num = this.GetPointerStateIndexFor(pointerOrTouchId);
			}
			return num != -1 && this.m_PointerStates[num].eventData.pointerEnter != null;
		}

		public RaycastResult GetLastRaycastResult(int pointerOrTouchId)
		{
			int pointerStateIndexFor = this.GetPointerStateIndexFor(pointerOrTouchId);
			if (pointerStateIndexFor == -1)
			{
				return default(RaycastResult);
			}
			return this.m_PointerStates[pointerStateIndexFor].eventData.pointerCurrentRaycast;
		}

		private RaycastResult PerformRaycast(ExtendedPointerEventData eventData)
		{
			if (eventData == null)
			{
				throw new ArgumentNullException("eventData");
			}
			if (eventData.pointerType == UIPointerType.Tracked && TrackedDeviceRaycaster.s_Instances.length > 0)
			{
				for (int i = 0; i < TrackedDeviceRaycaster.s_Instances.length; i++)
				{
					TrackedDeviceRaycaster trackedDeviceRaycaster = TrackedDeviceRaycaster.s_Instances[i];
					this.m_RaycastResultCache.Clear();
					trackedDeviceRaycaster.PerformRaycast(eventData, this.m_RaycastResultCache);
					if (this.m_RaycastResultCache.Count > 0)
					{
						RaycastResult result = this.m_RaycastResultCache[0];
						this.m_RaycastResultCache.Clear();
						return result;
					}
				}
				return default(RaycastResult);
			}
			base.eventSystem.RaycastAll(eventData, this.m_RaycastResultCache);
			RaycastResult result2 = BaseInputModule.FindFirstRaycast(this.m_RaycastResultCache);
			this.m_RaycastResultCache.Clear();
			return result2;
		}

		private void ProcessPointer(ref PointerModel state)
		{
			ExtendedPointerEventData eventData = state.eventData;
			UIPointerType pointerType = eventData.pointerType;
			if (pointerType == UIPointerType.MouseOrPen && Cursor.lockState == CursorLockMode.Locked)
			{
				eventData.position = ((this.m_CursorLockBehavior == InputSystemUIInputModule.CursorLockBehavior.OutsideScreen) ? new Vector2(-1f, -1f) : new Vector2((float)Screen.width / 2f, (float)Screen.height / 2f));
				eventData.delta = default(Vector2);
			}
			else if (pointerType == UIPointerType.Tracked)
			{
				Vector3 vector = state.worldPosition;
				Quaternion quaternion = state.worldOrientation;
				if (this.m_XRTrackingOrigin != null)
				{
					vector = this.m_XRTrackingOrigin.TransformPoint(vector);
					quaternion = this.m_XRTrackingOrigin.rotation * quaternion;
				}
				eventData.trackedDeviceOrientation = quaternion;
				eventData.trackedDevicePosition = vector;
			}
			else
			{
				eventData.delta = state.screenPosition - eventData.position;
				eventData.position = state.screenPosition;
			}
			eventData.Reset();
			eventData.pointerCurrentRaycast = this.PerformRaycast(eventData);
			if (pointerType == UIPointerType.Tracked && eventData.pointerCurrentRaycast.isValid)
			{
				Vector2 screenPosition = eventData.pointerCurrentRaycast.screenPosition;
				eventData.delta = screenPosition - eventData.position;
				eventData.position = eventData.pointerCurrentRaycast.screenPosition;
			}
			eventData.button = PointerEventData.InputButton.Left;
			state.leftButton.CopyPressStateTo(eventData);
			this.ProcessPointerMovement(ref state, eventData);
			if (!state.changedThisFrame && (this.xrTrackingOrigin == null || state.pointerType != UIPointerType.Tracked))
			{
				return;
			}
			this.ProcessPointerButton(ref state.leftButton, eventData);
			this.ProcessPointerButtonDrag(ref state.leftButton, eventData);
			InputSystemUIInputModule.ProcessPointerScroll(ref state, eventData);
			eventData.button = PointerEventData.InputButton.Right;
			state.rightButton.CopyPressStateTo(eventData);
			this.ProcessPointerButton(ref state.rightButton, eventData);
			this.ProcessPointerButtonDrag(ref state.rightButton, eventData);
			eventData.button = PointerEventData.InputButton.Middle;
			state.middleButton.CopyPressStateTo(eventData);
			this.ProcessPointerButton(ref state.middleButton, eventData);
			this.ProcessPointerButtonDrag(ref state.middleButton, eventData);
		}

		private bool PointerShouldIgnoreTransform(Transform t)
		{
			MultiplayerEventSystem multiplayerEventSystem = base.eventSystem as MultiplayerEventSystem;
			return multiplayerEventSystem != null && multiplayerEventSystem.playerRoot != null && !t.IsChildOf(multiplayerEventSystem.playerRoot.transform);
		}

		private void ProcessPointerMovement(ref PointerModel pointer, ExtendedPointerEventData eventData)
		{
			GameObject currentPointerTarget = (eventData.pointerType == UIPointerType.Touch && !pointer.leftButton.isPressed && !pointer.leftButton.wasReleasedThisFrame) ? null : eventData.pointerCurrentRaycast.gameObject;
			this.ProcessPointerMovement(eventData, currentPointerTarget);
		}

		private void ProcessPointerMovement(ExtendedPointerEventData eventData, GameObject currentPointerTarget)
		{
			bool flag = eventData.IsPointerMoving();
			if (flag)
			{
				for (int i = 0; i < eventData.hovered.Count; i++)
				{
					ExecuteEvents.Execute<IPointerMoveHandler>(eventData.hovered[i], eventData, ExecuteEvents.pointerMoveHandler);
				}
			}
			if (currentPointerTarget == null || eventData.pointerEnter == null)
			{
				for (int j = 0; j < eventData.hovered.Count; j++)
				{
					ExecuteEvents.Execute<IPointerExitHandler>(eventData.hovered[j], eventData, ExecuteEvents.pointerExitHandler);
				}
				eventData.hovered.Clear();
				if (currentPointerTarget == null)
				{
					eventData.pointerEnter = null;
					return;
				}
			}
			if (eventData.pointerEnter == currentPointerTarget && currentPointerTarget)
			{
				return;
			}
			GameObject gameObject = BaseInputModule.FindCommonRoot(eventData.pointerEnter, currentPointerTarget);
			Transform y = (gameObject != null) ? gameObject.transform : null;
			Component component = (Component)currentPointerTarget.GetComponentInParent<IPointerExitHandler>();
			Transform y2 = (component != null) ? component.transform : null;
			if (eventData.pointerEnter != null)
			{
				Transform transform = eventData.pointerEnter.transform;
				while (transform != null && (!this.sendPointerHoverToParent || !(transform == y)) && (this.sendPointerHoverToParent || !(transform == y2)))
				{
					eventData.fullyExited = (transform != y && eventData.pointerEnter != currentPointerTarget);
					ExecuteEvents.Execute<IPointerExitHandler>(transform.gameObject, eventData, ExecuteEvents.pointerExitHandler);
					eventData.hovered.Remove(transform.gameObject);
					if (this.sendPointerHoverToParent)
					{
						transform = transform.parent;
					}
					if (transform == y)
					{
						break;
					}
					if (!this.sendPointerHoverToParent)
					{
						transform = transform.parent;
					}
				}
			}
			Transform y3 = eventData.pointerEnter ? eventData.pointerEnter.transform : null;
			eventData.pointerEnter = currentPointerTarget;
			if (currentPointerTarget != null)
			{
				Transform transform2 = currentPointerTarget.transform;
				while (transform2 != null && !this.PointerShouldIgnoreTransform(transform2))
				{
					eventData.reentered = (transform2 == y && transform2 != y3);
					if (this.sendPointerHoverToParent && eventData.reentered)
					{
						break;
					}
					ExecuteEvents.Execute<IPointerEnterHandler>(transform2.gameObject, eventData, ExecuteEvents.pointerEnterHandler);
					if (flag)
					{
						ExecuteEvents.Execute<IPointerMoveHandler>(transform2.gameObject, eventData, ExecuteEvents.pointerMoveHandler);
					}
					eventData.hovered.Add(transform2.gameObject);
					if (!this.sendPointerHoverToParent && transform2.GetComponent<IPointerEnterHandler>() != null)
					{
						break;
					}
					if (this.sendPointerHoverToParent)
					{
						transform2 = transform2.parent;
					}
					if (transform2 == y)
					{
						break;
					}
					if (!this.sendPointerHoverToParent)
					{
						transform2 = transform2.parent;
					}
				}
			}
		}

		private void ProcessPointerButton(ref PointerModel.ButtonState button, PointerEventData eventData)
		{
			GameObject gameObject = eventData.pointerCurrentRaycast.gameObject;
			if (gameObject != null && this.PointerShouldIgnoreTransform(gameObject.transform))
			{
				return;
			}
			if (button.wasPressedThisFrame)
			{
				button.pressTime = InputRuntime.s_Instance.unscaledGameTime;
				eventData.delta = Vector2.zero;
				eventData.dragging = false;
				eventData.pressPosition = eventData.position;
				eventData.pointerPressRaycast = eventData.pointerCurrentRaycast;
				eventData.eligibleForClick = true;
				eventData.useDragThreshold = true;
				GameObject eventHandler = ExecuteEvents.GetEventHandler<ISelectHandler>(gameObject);
				if (eventHandler != base.eventSystem.currentSelectedGameObject && (eventHandler != null || this.m_DeselectOnBackgroundClick))
				{
					base.eventSystem.SetSelectedGameObject(null, eventData);
				}
				GameObject gameObject2 = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>(gameObject, eventData, ExecuteEvents.pointerDownHandler);
				GameObject eventHandler2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
				if (gameObject2 == null)
				{
					gameObject2 = eventHandler2;
				}
				button.clickedOnSameGameObject = (gameObject2 == eventData.lastPress && button.pressTime - eventData.clickTime <= 0.3f);
				if (eventData.clickCount > 0 && !button.clickedOnSameGameObject)
				{
					eventData.clickCount = 0;
					eventData.clickTime = 0f;
				}
				eventData.pointerPress = gameObject2;
				eventData.pointerClick = eventHandler2;
				eventData.rawPointerPress = gameObject;
				eventData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject);
				if (eventData.pointerDrag != null)
				{
					ExecuteEvents.Execute<IInitializePotentialDragHandler>(eventData.pointerDrag, eventData, ExecuteEvents.initializePotentialDrag);
				}
			}
			if (button.wasReleasedThisFrame)
			{
				GameObject eventHandler3 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
				bool flag = eventData.pointerClick != null && eventData.pointerClick == eventHandler3 && eventData.eligibleForClick;
				if (flag)
				{
					if (button.clickedOnSameGameObject)
					{
						int clickCount = eventData.clickCount + 1;
						eventData.clickCount = clickCount;
					}
					else
					{
						eventData.clickCount = 1;
					}
					eventData.clickTime = InputRuntime.s_Instance.unscaledGameTime;
				}
				ExecuteEvents.Execute<IPointerUpHandler>(eventData.pointerPress, eventData, ExecuteEvents.pointerUpHandler);
				if (flag)
				{
					ExecuteEvents.Execute<IPointerClickHandler>(eventData.pointerClick, eventData, ExecuteEvents.pointerClickHandler);
				}
				else if (eventData.dragging && eventData.pointerDrag != null)
				{
					ExecuteEvents.ExecuteHierarchy<IDropHandler>(gameObject, eventData, ExecuteEvents.dropHandler);
				}
				eventData.eligibleForClick = false;
				eventData.pointerPress = null;
				eventData.rawPointerPress = null;
				if (eventData.dragging && eventData.pointerDrag != null)
				{
					ExecuteEvents.Execute<IEndDragHandler>(eventData.pointerDrag, eventData, ExecuteEvents.endDragHandler);
				}
				eventData.dragging = false;
				eventData.pointerDrag = null;
				button.ignoreNextClick = false;
			}
			button.CopyPressStateFrom(eventData);
		}

		private void ProcessPointerButtonDrag(ref PointerModel.ButtonState button, ExtendedPointerEventData eventData)
		{
			if (!eventData.IsPointerMoving() || (eventData.pointerType == UIPointerType.MouseOrPen && Cursor.lockState == CursorLockMode.Locked) || eventData.pointerDrag == null)
			{
				return;
			}
			if (!eventData.dragging && (!eventData.useDragThreshold || (double)(eventData.pressPosition - eventData.position).sqrMagnitude >= (double)base.eventSystem.pixelDragThreshold * (double)base.eventSystem.pixelDragThreshold * (double)((eventData.pointerType == UIPointerType.Tracked) ? this.m_TrackedDeviceDragThresholdMultiplier : 1f)))
			{
				ExecuteEvents.Execute<IBeginDragHandler>(eventData.pointerDrag, eventData, ExecuteEvents.beginDragHandler);
				eventData.dragging = true;
			}
			if (eventData.dragging)
			{
				if (eventData.pointerPress != eventData.pointerDrag)
				{
					ExecuteEvents.Execute<IPointerUpHandler>(eventData.pointerPress, eventData, ExecuteEvents.pointerUpHandler);
					eventData.eligibleForClick = false;
					eventData.pointerPress = null;
					eventData.rawPointerPress = null;
				}
				ExecuteEvents.Execute<IDragHandler>(eventData.pointerDrag, eventData, ExecuteEvents.dragHandler);
				button.CopyPressStateFrom(eventData);
			}
		}

		private static void ProcessPointerScroll(ref PointerModel pointer, PointerEventData eventData)
		{
			Vector2 scrollDelta = pointer.scrollDelta;
			if (!Mathf.Approximately(scrollDelta.sqrMagnitude, 0f))
			{
				eventData.scrollDelta = scrollDelta;
				ExecuteEvents.ExecuteHierarchy<IScrollHandler>(ExecuteEvents.GetEventHandler<IScrollHandler>(eventData.pointerEnter), eventData, ExecuteEvents.scrollHandler);
			}
		}

		internal void ProcessNavigation(ref NavigationModel navigationState)
		{
			bool flag = false;
			if (base.eventSystem.currentSelectedGameObject != null)
			{
				BaseEventData baseEventData = this.GetBaseEventData();
				ExecuteEvents.Execute<IUpdateSelectedHandler>(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.updateSelectedHandler);
				flag = baseEventData.used;
			}
			if (!base.eventSystem.sendNavigationEvents)
			{
				return;
			}
			Vector2 move = navigationState.move;
			if (!flag && (!Mathf.Approximately(move.x, 0f) || !Mathf.Approximately(move.y, 0f)))
			{
				float unscaledGameTime = InputRuntime.s_Instance.unscaledGameTime;
				Vector2 move2 = navigationState.move;
				MoveDirection moveDirection = MoveDirection.None;
				if (move2.sqrMagnitude > 0f)
				{
					if (Mathf.Abs(move2.x) > Mathf.Abs(move2.y))
					{
						moveDirection = ((move2.x > 0f) ? MoveDirection.Right : MoveDirection.Left);
					}
					else
					{
						moveDirection = ((move2.y > 0f) ? MoveDirection.Up : MoveDirection.Down);
					}
				}
				if (moveDirection != this.m_NavigationState.lastMoveDirection)
				{
					this.m_NavigationState.consecutiveMoveCount = 0;
				}
				if (moveDirection != MoveDirection.None)
				{
					bool flag2 = true;
					if (this.m_NavigationState.consecutiveMoveCount != 0)
					{
						if (this.m_NavigationState.consecutiveMoveCount > 1)
						{
							flag2 = (unscaledGameTime > this.m_NavigationState.lastMoveTime + this.moveRepeatRate);
						}
						else
						{
							flag2 = (unscaledGameTime > this.m_NavigationState.lastMoveTime + this.moveRepeatDelay);
						}
					}
					if (flag2)
					{
						ExtendedAxisEventData extendedAxisEventData = this.m_NavigationState.eventData as ExtendedAxisEventData;
						if (extendedAxisEventData == null)
						{
							extendedAxisEventData = new ExtendedAxisEventData(base.eventSystem);
							this.m_NavigationState.eventData = extendedAxisEventData;
						}
						extendedAxisEventData.Reset();
						extendedAxisEventData.moveVector = move2;
						extendedAxisEventData.moveDir = moveDirection;
						extendedAxisEventData.device = navigationState.device;
						if (this.IsMoveAllowed(extendedAxisEventData))
						{
							ExecuteEvents.Execute<IMoveHandler>(base.eventSystem.currentSelectedGameObject, extendedAxisEventData, ExecuteEvents.moveHandler);
							flag = extendedAxisEventData.used;
							this.m_NavigationState.consecutiveMoveCount = this.m_NavigationState.consecutiveMoveCount + 1;
							this.m_NavigationState.lastMoveTime = unscaledGameTime;
							this.m_NavigationState.lastMoveDirection = moveDirection;
						}
					}
				}
				else
				{
					this.m_NavigationState.consecutiveMoveCount = 0;
				}
			}
			else
			{
				this.m_NavigationState.consecutiveMoveCount = 0;
			}
			if (!flag && base.eventSystem.currentSelectedGameObject != null)
			{
				InputActionReference submitAction = this.m_SubmitAction;
				InputAction inputAction = (submitAction != null) ? submitAction.action : null;
				InputActionReference cancelAction = this.m_CancelAction;
				InputAction inputAction2 = (cancelAction != null) ? cancelAction.action : null;
				ExtendedSubmitCancelEventData extendedSubmitCancelEventData = this.m_SubmitCancelState.eventData as ExtendedSubmitCancelEventData;
				if (extendedSubmitCancelEventData == null)
				{
					extendedSubmitCancelEventData = new ExtendedSubmitCancelEventData(base.eventSystem);
					this.m_SubmitCancelState.eventData = extendedSubmitCancelEventData;
				}
				extendedSubmitCancelEventData.Reset();
				extendedSubmitCancelEventData.device = this.m_SubmitCancelState.device;
				if (inputAction2 != null && inputAction2.WasPerformedThisDynamicUpdate())
				{
					ExecuteEvents.Execute<ICancelHandler>(base.eventSystem.currentSelectedGameObject, extendedSubmitCancelEventData, ExecuteEvents.cancelHandler);
				}
				if (!extendedSubmitCancelEventData.used && inputAction != null && inputAction.WasPerformedThisDynamicUpdate())
				{
					ExecuteEvents.Execute<ISubmitHandler>(base.eventSystem.currentSelectedGameObject, extendedSubmitCancelEventData, ExecuteEvents.submitHandler);
				}
			}
		}

		private bool IsMoveAllowed(AxisEventData eventData)
		{
			if (this.m_LocalMultiPlayerRoot == null)
			{
				return true;
			}
			if (base.eventSystem.currentSelectedGameObject == null)
			{
				return true;
			}
			Selectable component = base.eventSystem.currentSelectedGameObject.GetComponent<Selectable>();
			if (component == null)
			{
				return true;
			}
			Selectable selectable = null;
			switch (eventData.moveDir)
			{
			case MoveDirection.Left:
				selectable = component.FindSelectableOnLeft();
				break;
			case MoveDirection.Up:
				selectable = component.FindSelectableOnUp();
				break;
			case MoveDirection.Right:
				selectable = component.FindSelectableOnRight();
				break;
			case MoveDirection.Down:
				selectable = component.FindSelectableOnDown();
				break;
			}
			return selectable == null || selectable.transform.IsChildOf(this.m_LocalMultiPlayerRoot.transform);
		}

		public float moveRepeatDelay
		{
			get
			{
				return this.m_MoveRepeatDelay;
			}
			set
			{
				this.m_MoveRepeatDelay = value;
			}
		}

		public float moveRepeatRate
		{
			get
			{
				return this.m_MoveRepeatRate;
			}
			set
			{
				this.m_MoveRepeatRate = value;
			}
		}

		private bool explictlyIgnoreFocus
		{
			get
			{
				return InputSystem.settings.backgroundBehavior == InputSettings.BackgroundBehavior.IgnoreFocus;
			}
		}

		private bool shouldIgnoreFocus
		{
			get
			{
				return this.explictlyIgnoreFocus || InputRuntime.s_Instance.runInBackground;
			}
		}

		[Obsolete("'repeatRate' has been obsoleted; use 'moveRepeatRate' instead. (UnityUpgradable) -> moveRepeatRate", false)]
		public float repeatRate
		{
			get
			{
				return this.moveRepeatRate;
			}
			set
			{
				this.moveRepeatRate = value;
			}
		}

		[Obsolete("'repeatDelay' has been obsoleted; use 'moveRepeatDelay' instead. (UnityUpgradable) -> moveRepeatDelay", false)]
		public float repeatDelay
		{
			get
			{
				return this.moveRepeatDelay;
			}
			set
			{
				this.moveRepeatDelay = value;
			}
		}

		public Transform xrTrackingOrigin
		{
			get
			{
				return this.m_XRTrackingOrigin;
			}
			set
			{
				this.m_XRTrackingOrigin = value;
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

		private void SwapAction(ref InputActionReference property, InputActionReference newValue, bool actionsHooked, Action<InputAction.CallbackContext> actionCallback)
		{
			if (property == newValue || (property != null && newValue != null && property.action == newValue.action))
			{
				return;
			}
			if (property != null && actionCallback != null && actionsHooked)
			{
				property.action.performed -= actionCallback;
				property.action.canceled -= actionCallback;
			}
			InputActionReference inputActionReference = property;
			bool flag = ((inputActionReference != null) ? inputActionReference.action : null) == null;
			InputActionReference inputActionReference2 = property;
			bool flag2 = ((inputActionReference2 != null) ? inputActionReference2.action : null) != null && property.action.enabled;
			this.TryDisableInputAction(property, false);
			property = newValue;
			if (((newValue != null) ? newValue.action : null) != null && actionCallback != null && actionsHooked)
			{
				property.action.performed += actionCallback;
				property.action.canceled += actionCallback;
			}
			if (base.isActiveAndEnabled && ((newValue != null) ? newValue.action : null) != null && (flag2 || flag))
			{
				this.EnableInputAction(property);
			}
		}

		public InputActionReference point
		{
			get
			{
				return this.m_PointAction;
			}
			set
			{
				this.SwapAction(ref this.m_PointAction, value, this.m_ActionsHooked, this.m_OnPointDelegate);
			}
		}

		public InputActionReference scrollWheel
		{
			get
			{
				return this.m_ScrollWheelAction;
			}
			set
			{
				this.SwapAction(ref this.m_ScrollWheelAction, value, this.m_ActionsHooked, this.m_OnScrollWheelDelegate);
			}
		}

		public InputActionReference leftClick
		{
			get
			{
				return this.m_LeftClickAction;
			}
			set
			{
				this.SwapAction(ref this.m_LeftClickAction, value, this.m_ActionsHooked, this.m_OnLeftClickDelegate);
			}
		}

		public InputActionReference middleClick
		{
			get
			{
				return this.m_MiddleClickAction;
			}
			set
			{
				this.SwapAction(ref this.m_MiddleClickAction, value, this.m_ActionsHooked, this.m_OnMiddleClickDelegate);
			}
		}

		public InputActionReference rightClick
		{
			get
			{
				return this.m_RightClickAction;
			}
			set
			{
				this.SwapAction(ref this.m_RightClickAction, value, this.m_ActionsHooked, this.m_OnRightClickDelegate);
			}
		}

		public InputActionReference move
		{
			get
			{
				return this.m_MoveAction;
			}
			set
			{
				this.SwapAction(ref this.m_MoveAction, value, this.m_ActionsHooked, this.m_OnMoveDelegate);
			}
		}

		public InputActionReference submit
		{
			get
			{
				return this.m_SubmitAction;
			}
			set
			{
				this.SwapAction(ref this.m_SubmitAction, value, this.m_ActionsHooked, this.m_OnSubmitCancelDelegate);
			}
		}

		public InputActionReference cancel
		{
			get
			{
				return this.m_CancelAction;
			}
			set
			{
				this.SwapAction(ref this.m_CancelAction, value, this.m_ActionsHooked, this.m_OnSubmitCancelDelegate);
			}
		}

		public InputActionReference trackedDeviceOrientation
		{
			get
			{
				return this.m_TrackedDeviceOrientationAction;
			}
			set
			{
				this.SwapAction(ref this.m_TrackedDeviceOrientationAction, value, this.m_ActionsHooked, this.m_OnTrackedDeviceOrientationDelegate);
			}
		}

		public InputActionReference trackedDevicePosition
		{
			get
			{
				return this.m_TrackedDevicePositionAction;
			}
			set
			{
				this.SwapAction(ref this.m_TrackedDevicePositionAction, value, this.m_ActionsHooked, this.m_OnTrackedDevicePositionDelegate);
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetDefaultActions()
		{
			if (InputSystemUIInputModule.defaultActions != null)
			{
				InputSystemUIInputModule.defaultActions.Dispose();
				InputSystemUIInputModule.defaultActions = null;
			}
		}

		public void AssignDefaultActions()
		{
			if (InputSystemUIInputModule.defaultActions == null)
			{
				InputSystemUIInputModule.defaultActions = new DefaultInputActions();
			}
			this.actionsAsset = InputSystemUIInputModule.defaultActions.asset;
			this.cancel = InputActionReference.Create(InputSystemUIInputModule.defaultActions.UI.Cancel);
			this.submit = InputActionReference.Create(InputSystemUIInputModule.defaultActions.UI.Submit);
			this.move = InputActionReference.Create(InputSystemUIInputModule.defaultActions.UI.Navigate);
			this.leftClick = InputActionReference.Create(InputSystemUIInputModule.defaultActions.UI.Click);
			this.rightClick = InputActionReference.Create(InputSystemUIInputModule.defaultActions.UI.RightClick);
			this.middleClick = InputActionReference.Create(InputSystemUIInputModule.defaultActions.UI.MiddleClick);
			this.point = InputActionReference.Create(InputSystemUIInputModule.defaultActions.UI.Point);
			this.scrollWheel = InputActionReference.Create(InputSystemUIInputModule.defaultActions.UI.ScrollWheel);
			this.trackedDeviceOrientation = InputActionReference.Create(InputSystemUIInputModule.defaultActions.UI.TrackedDeviceOrientation);
			this.trackedDevicePosition = InputActionReference.Create(InputSystemUIInputModule.defaultActions.UI.TrackedDevicePosition);
		}

		public void UnassignActions()
		{
			DefaultInputActions defaultInputActions = InputSystemUIInputModule.defaultActions;
			if (defaultInputActions != null)
			{
				defaultInputActions.Dispose();
			}
			InputSystemUIInputModule.defaultActions = null;
			this.actionsAsset = null;
			this.cancel = null;
			this.submit = null;
			this.move = null;
			this.leftClick = null;
			this.rightClick = null;
			this.middleClick = null;
			this.point = null;
			this.scrollWheel = null;
			this.trackedDeviceOrientation = null;
			this.trackedDevicePosition = null;
		}

		[Obsolete("'trackedDeviceSelect' has been obsoleted; use 'leftClick' instead.", true)]
		public InputActionReference trackedDeviceSelect
		{
			get
			{
				throw new InvalidOperationException();
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		protected override void Awake()
		{
			base.Awake();
			this.m_NavigationState.Reset();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.UnhookActions();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (this.m_OnControlsChangedDelegate == null)
			{
				this.m_OnControlsChangedDelegate = new Action<object>(this.OnControlsChanged);
			}
			InputActionState.s_GlobalState.onActionControlsChanged.AddCallback(this.m_OnControlsChangedDelegate);
			if (this.HasNoActions())
			{
				this.AssignDefaultActions();
			}
			this.ResetPointers();
			this.HookActions();
			this.EnableAllActions();
		}

		protected override void OnDisable()
		{
			this.ResetPointers();
			InputActionState.s_GlobalState.onActionControlsChanged.RemoveCallback(this.m_OnControlsChangedDelegate);
			this.DisableAllActions();
			this.UnhookActions();
			if (InputSystemUIInputModule.defaultActions != null && InputSystemUIInputModule.defaultActions.asset == this.actionsAsset)
			{
				this.UnassignActions();
			}
			base.OnDisable();
		}

		private void ResetPointers()
		{
			for (int i = 0; i < this.m_PointerStates.length; i++)
			{
				if (this.SendPointerExitEventsAndRemovePointer(i))
				{
					i--;
				}
			}
			this.m_CurrentPointerId = -1;
			this.m_CurrentPointerIndex = -1;
			this.m_CurrentPointerType = UIPointerType.None;
		}

		private bool HasNoActions()
		{
			if (this.m_ActionsAsset != null)
			{
				return false;
			}
			InputActionReference pointAction = this.m_PointAction;
			if (((pointAction != null) ? pointAction.action : null) == null)
			{
				InputActionReference leftClickAction = this.m_LeftClickAction;
				if (((leftClickAction != null) ? leftClickAction.action : null) == null)
				{
					InputActionReference rightClickAction = this.m_RightClickAction;
					if (((rightClickAction != null) ? rightClickAction.action : null) == null)
					{
						InputActionReference middleClickAction = this.m_MiddleClickAction;
						if (((middleClickAction != null) ? middleClickAction.action : null) == null)
						{
							InputActionReference submitAction = this.m_SubmitAction;
							if (((submitAction != null) ? submitAction.action : null) == null)
							{
								InputActionReference cancelAction = this.m_CancelAction;
								if (((cancelAction != null) ? cancelAction.action : null) == null)
								{
									InputActionReference scrollWheelAction = this.m_ScrollWheelAction;
									if (((scrollWheelAction != null) ? scrollWheelAction.action : null) == null)
									{
										InputActionReference trackedDeviceOrientationAction = this.m_TrackedDeviceOrientationAction;
										if (((trackedDeviceOrientationAction != null) ? trackedDeviceOrientationAction.action : null) == null)
										{
											InputActionReference trackedDevicePositionAction = this.m_TrackedDevicePositionAction;
											return ((trackedDevicePositionAction != null) ? trackedDevicePositionAction.action : null) == null;
										}
									}
								}
							}
						}
					}
				}
			}
			return false;
		}

		private void EnableAllActions()
		{
			this.EnableInputAction(this.m_PointAction);
			this.EnableInputAction(this.m_LeftClickAction);
			this.EnableInputAction(this.m_RightClickAction);
			this.EnableInputAction(this.m_MiddleClickAction);
			this.EnableInputAction(this.m_MoveAction);
			this.EnableInputAction(this.m_SubmitAction);
			this.EnableInputAction(this.m_CancelAction);
			this.EnableInputAction(this.m_ScrollWheelAction);
			this.EnableInputAction(this.m_TrackedDeviceOrientationAction);
			this.EnableInputAction(this.m_TrackedDevicePositionAction);
		}

		private void DisableAllActions()
		{
			this.TryDisableInputAction(this.m_PointAction, true);
			this.TryDisableInputAction(this.m_LeftClickAction, true);
			this.TryDisableInputAction(this.m_RightClickAction, true);
			this.TryDisableInputAction(this.m_MiddleClickAction, true);
			this.TryDisableInputAction(this.m_MoveAction, true);
			this.TryDisableInputAction(this.m_SubmitAction, true);
			this.TryDisableInputAction(this.m_CancelAction, true);
			this.TryDisableInputAction(this.m_ScrollWheelAction, true);
			this.TryDisableInputAction(this.m_TrackedDeviceOrientationAction, true);
			this.TryDisableInputAction(this.m_TrackedDevicePositionAction, true);
		}

		private void EnableInputAction(InputActionReference inputActionReference)
		{
			InputAction inputAction = (inputActionReference != null) ? inputActionReference.action : null;
			if (inputAction == null)
			{
				return;
			}
			InputSystemUIInputModule.InputActionReferenceState value;
			if (InputSystemUIInputModule.s_InputActionReferenceCounts.TryGetValue(inputAction, out value))
			{
				value.refCount++;
				InputSystemUIInputModule.s_InputActionReferenceCounts[inputAction] = value;
			}
			else
			{
				value = new InputSystemUIInputModule.InputActionReferenceState
				{
					refCount = 1,
					enabledByInputModule = !inputAction.enabled
				};
				InputSystemUIInputModule.s_InputActionReferenceCounts.Add(inputAction, value);
			}
			inputAction.Enable();
		}

		private void TryDisableInputAction(InputActionReference inputActionReference, bool isComponentDisabling = false)
		{
			InputAction inputAction = (inputActionReference != null) ? inputActionReference.action : null;
			if (inputAction == null)
			{
				return;
			}
			if (!base.isActiveAndEnabled && !isComponentDisabling)
			{
				return;
			}
			InputSystemUIInputModule.InputActionReferenceState inputActionReferenceState;
			if (!InputSystemUIInputModule.s_InputActionReferenceCounts.TryGetValue(inputAction, out inputActionReferenceState))
			{
				return;
			}
			if (inputActionReferenceState.refCount - 1 == 0 && inputActionReferenceState.enabledByInputModule)
			{
				inputAction.Disable();
				InputSystemUIInputModule.s_InputActionReferenceCounts.Remove(inputAction);
				return;
			}
			inputActionReferenceState.refCount--;
			InputSystemUIInputModule.s_InputActionReferenceCounts[inputAction] = inputActionReferenceState;
		}

		private int GetPointerStateIndexFor(int pointerOrTouchId)
		{
			if (pointerOrTouchId == this.m_CurrentPointerId)
			{
				return this.m_CurrentPointerIndex;
			}
			for (int i = 0; i < this.m_PointerIds.length; i++)
			{
				if (this.m_PointerIds[i] == pointerOrTouchId)
				{
					return i;
				}
			}
			for (int j = 0; j < this.m_PointerStates.length; j++)
			{
				ExtendedPointerEventData eventData = this.m_PointerStates[j].eventData;
				if (eventData.touchId == pointerOrTouchId || (eventData.touchId != 0 && eventData.device.deviceId == pointerOrTouchId))
				{
					return j;
				}
			}
			return -1;
		}

		private ref PointerModel GetPointerStateForIndex(int index)
		{
			if (index == 0)
			{
				return ref this.m_PointerStates.firstValue;
			}
			return ref this.m_PointerStates.additionalValues[index - 1];
		}

		private int GetDisplayIndexFor(InputControl control)
		{
			int result = 0;
			Pointer pointer = control.device as Pointer;
			if (pointer != null)
			{
				result = pointer.displayIndex.ReadValue();
			}
			return result;
		}

		private int GetPointerStateIndexFor(ref InputAction.CallbackContext context)
		{
			if (this.CheckForRemovedDevice(ref context))
			{
				return -1;
			}
			InputActionPhase phase = context.phase;
			return this.GetPointerStateIndexFor(context.control, phase != InputActionPhase.Canceled);
		}

		private unsafe int GetPointerStateIndexFor(InputControl control, bool createIfNotExists = true)
		{
			InputDevice device = control.device;
			InputControl parent = control.parent;
			int num = device.deviceId;
			int num2 = 0;
			Vector2 screenPosition = Vector2.zero;
			TouchControl touchControl = parent as TouchControl;
			if (touchControl != null)
			{
				num2 = *touchControl.touchId.value;
				screenPosition = *touchControl.position.value;
			}
			else
			{
				Touchscreen touchscreen = parent as Touchscreen;
				if (touchscreen != null)
				{
					num2 = *touchscreen.primaryTouch.touchId.value;
					screenPosition = *touchscreen.primaryTouch.position.value;
				}
			}
			int displayIndexFor = this.GetDisplayIndexFor(control);
			if (num2 != 0)
			{
				num = ExtendedPointerEventData.MakePointerIdForTouch(num, num2);
			}
			if (this.m_CurrentPointerId == num)
			{
				return this.m_CurrentPointerIndex;
			}
			for (int i = 0; i < this.m_PointerIds.length; i++)
			{
				if (this.m_PointerIds[i] == num)
				{
					this.m_CurrentPointerId = num;
					this.m_CurrentPointerIndex = i;
					this.m_CurrentPointerType = this.m_PointerStates[i].pointerType;
					return i;
				}
			}
			if (!createIfNotExists)
			{
				return -1;
			}
			UIPointerType uipointerType = UIPointerType.None;
			if (num2 != 0)
			{
				uipointerType = UIPointerType.Touch;
			}
			else if (InputSystemUIInputModule.HaveControlForDevice(device, this.point))
			{
				uipointerType = UIPointerType.MouseOrPen;
			}
			else if (InputSystemUIInputModule.HaveControlForDevice(device, this.trackedDevicePosition))
			{
				uipointerType = UIPointerType.Tracked;
			}
			if ((this.m_PointerBehavior == UIPointerBehavior.SingleUnifiedPointer && uipointerType != UIPointerType.None) || (this.m_PointerBehavior == UIPointerBehavior.SingleMouseOrPenButMultiTouchAndTrack && uipointerType == UIPointerType.MouseOrPen))
			{
				if (this.m_CurrentPointerIndex == -1)
				{
					this.m_CurrentPointerIndex = this.AllocatePointer(num, displayIndexFor, num2, uipointerType, control, device, (num2 != 0) ? parent : null);
				}
				else
				{
					ref PointerModel pointerStateForIndex = ref this.GetPointerStateForIndex(this.m_CurrentPointerIndex);
					ExtendedPointerEventData eventData = pointerStateForIndex.eventData;
					eventData.control = control;
					eventData.device = device;
					eventData.pointerType = uipointerType;
					eventData.pointerId = num;
					eventData.touchId = num2;
					eventData.displayIndex = displayIndexFor;
					eventData.trackedDeviceOrientation = default(Quaternion);
					eventData.trackedDevicePosition = default(Vector3);
					if (this.m_PointerBehavior == UIPointerBehavior.SingleUnifiedPointer)
					{
						pointerStateForIndex.leftButton.OnEndFrame();
						pointerStateForIndex.rightButton.OnEndFrame();
						pointerStateForIndex.middleButton.OnEndFrame();
					}
				}
				if (uipointerType == UIPointerType.Touch)
				{
					this.GetPointerStateForIndex(this.m_CurrentPointerIndex).screenPosition = screenPosition;
				}
				this.m_CurrentPointerId = num;
				this.m_CurrentPointerType = uipointerType;
				return this.m_CurrentPointerIndex;
			}
			int num3;
			if (uipointerType != UIPointerType.None)
			{
				num3 = this.AllocatePointer(num, displayIndexFor, num2, uipointerType, control, device, (num2 != 0) ? parent : null);
			}
			else
			{
				if (this.m_CurrentPointerId != -1)
				{
					return this.m_CurrentPointerIndex;
				}
				InputActionReference point = this.point;
				ReadOnlyArray<InputControl>? readOnlyArray;
				if (point == null)
				{
					readOnlyArray = null;
				}
				else
				{
					InputAction action = point.action;
					readOnlyArray = ((action != null) ? new ReadOnlyArray<InputControl>?(action.controls) : null);
				}
				ReadOnlyArray<InputControl>? readOnlyArray2 = readOnlyArray;
				InputDevice inputDevice = (readOnlyArray2 != null && readOnlyArray2.Value.Count > 0) ? readOnlyArray2.Value[0].device : null;
				if (inputDevice != null && !(inputDevice is Touchscreen))
				{
					num3 = this.AllocatePointer(inputDevice.deviceId, displayIndexFor, 0, UIPointerType.MouseOrPen, readOnlyArray2.Value[0], inputDevice, null);
				}
				else
				{
					InputActionReference trackedDevicePosition = this.trackedDevicePosition;
					ReadOnlyArray<InputControl>? readOnlyArray3;
					if (trackedDevicePosition == null)
					{
						readOnlyArray3 = null;
					}
					else
					{
						InputAction action2 = trackedDevicePosition.action;
						readOnlyArray3 = ((action2 != null) ? new ReadOnlyArray<InputControl>?(action2.controls) : null);
					}
					ReadOnlyArray<InputControl>? readOnlyArray4 = readOnlyArray3;
					InputDevice inputDevice2 = (readOnlyArray4 != null && readOnlyArray4.Value.Count > 0) ? readOnlyArray4.Value[0].device : null;
					if (inputDevice2 != null)
					{
						num3 = this.AllocatePointer(inputDevice2.deviceId, displayIndexFor, 0, UIPointerType.Tracked, readOnlyArray4.Value[0], inputDevice2, null);
					}
					else
					{
						num3 = this.AllocatePointer(num, displayIndexFor, 0, UIPointerType.None, control, device, null);
					}
				}
			}
			if (uipointerType == UIPointerType.Touch)
			{
				this.GetPointerStateForIndex(num3).screenPosition = screenPosition;
			}
			this.m_CurrentPointerId = num;
			this.m_CurrentPointerIndex = num3;
			this.m_CurrentPointerType = uipointerType;
			return num3;
		}

		private int AllocatePointer(int pointerId, int displayIndex, int touchId, UIPointerType pointerType, InputControl control, InputDevice device, InputControl touchControl = null)
		{
			ExtendedPointerEventData extendedPointerEventData = null;
			if (this.m_PointerStates.Capacity > this.m_PointerStates.length)
			{
				if (this.m_PointerStates.length == 0)
				{
					extendedPointerEventData = this.m_PointerStates.firstValue.eventData;
				}
				else
				{
					extendedPointerEventData = this.m_PointerStates.additionalValues[this.m_PointerStates.length - 1].eventData;
				}
			}
			if (extendedPointerEventData == null)
			{
				extendedPointerEventData = new ExtendedPointerEventData(base.eventSystem);
			}
			extendedPointerEventData.pointerId = pointerId;
			extendedPointerEventData.displayIndex = displayIndex;
			extendedPointerEventData.touchId = touchId;
			extendedPointerEventData.pointerType = pointerType;
			extendedPointerEventData.control = control;
			extendedPointerEventData.device = device;
			this.m_PointerIds.AppendWithCapacity(pointerId, 10);
			return this.m_PointerStates.AppendWithCapacity(new PointerModel(extendedPointerEventData), 10);
		}

		private bool SendPointerExitEventsAndRemovePointer(int index)
		{
			ExtendedPointerEventData eventData = this.m_PointerStates[index].eventData;
			if (eventData.pointerEnter != null)
			{
				this.ProcessPointerMovement(eventData, null);
			}
			return this.RemovePointerAtIndex(index);
		}

		private bool RemovePointerAtIndex(int index)
		{
			if (this.m_PointerStates.length == 0)
			{
				return false;
			}
			ExtendedPointerEventData eventData = this.m_PointerStates[index].eventData;
			if (index == this.m_CurrentPointerIndex)
			{
				this.m_CurrentPointerId = -1;
				this.m_CurrentPointerIndex = -1;
				this.m_CurrentPointerType = UIPointerType.None;
			}
			else if (this.m_CurrentPointerIndex == this.m_PointerIds.length - 1)
			{
				this.m_CurrentPointerIndex = index;
			}
			this.m_PointerIds.RemoveAtByMovingTailWithCapacity(index);
			this.m_PointerStates.RemoveAtByMovingTailWithCapacity(index);
			eventData.hovered.Clear();
			eventData.device = null;
			eventData.pointerCurrentRaycast = default(RaycastResult);
			eventData.pointerPressRaycast = default(RaycastResult);
			eventData.pointerPress = null;
			eventData.pointerPress = null;
			eventData.pointerDrag = null;
			eventData.pointerEnter = null;
			eventData.rawPointerPress = null;
			if (this.m_PointerStates.length == 0)
			{
				this.m_PointerStates.firstValue.eventData = eventData;
			}
			else
			{
				this.m_PointerStates.additionalValues[this.m_PointerStates.length - 1].eventData = eventData;
			}
			return true;
		}

		private void PurgeStalePointers()
		{
			for (int i = 0; i < this.m_PointerStates.length; i++)
			{
				InputDevice device = this.GetPointerStateForIndex(i).eventData.device;
				if ((!device.added || (!InputSystemUIInputModule.HaveControlForDevice(device, this.point) && !InputSystemUIInputModule.HaveControlForDevice(device, this.trackedDevicePosition) && !InputSystemUIInputModule.HaveControlForDevice(device, this.trackedDeviceOrientation))) && this.SendPointerExitEventsAndRemovePointer(i))
				{
					i--;
				}
			}
			this.m_NeedToPurgeStalePointers = false;
		}

		private static bool HaveControlForDevice(InputDevice device, InputActionReference actionReference)
		{
			InputAction inputAction = (actionReference != null) ? actionReference.action : null;
			if (inputAction == null)
			{
				return false;
			}
			ReadOnlyArray<InputControl> controls = inputAction.controls;
			for (int i = 0; i < controls.Count; i++)
			{
				if (controls[i].device == device)
				{
					return true;
				}
			}
			return false;
		}

		private void OnPointCallback(InputAction.CallbackContext context)
		{
			if (this.CheckForRemovedDevice(ref context) || context.canceled)
			{
				return;
			}
			int pointerStateIndexFor = this.GetPointerStateIndexFor(context.control, true);
			if (pointerStateIndexFor == -1)
			{
				return;
			}
			ref PointerModel pointerStateForIndex = ref this.GetPointerStateForIndex(pointerStateIndexFor);
			pointerStateForIndex.screenPosition = context.ReadValue<Vector2>();
			pointerStateForIndex.eventData.displayIndex = this.GetDisplayIndexFor(context.control);
		}

		private bool IgnoreNextClick(ref InputAction.CallbackContext context, bool wasPressed)
		{
			return !this.explictlyIgnoreFocus && (context.canceled && !InputRuntime.s_Instance.isPlayerFocused && !context.control.device.canRunInBackground && wasPressed);
		}

		private void OnLeftClickCallback(InputAction.CallbackContext context)
		{
			int pointerStateIndexFor = this.GetPointerStateIndexFor(ref context);
			if (pointerStateIndexFor == -1)
			{
				return;
			}
			ref PointerModel pointerStateForIndex = ref this.GetPointerStateForIndex(pointerStateIndexFor);
			bool isPressed = pointerStateForIndex.leftButton.isPressed;
			pointerStateForIndex.leftButton.isPressed = context.ReadValueAsButton();
			pointerStateForIndex.changedThisFrame = true;
			if (this.IgnoreNextClick(ref context, isPressed))
			{
				pointerStateForIndex.leftButton.ignoreNextClick = true;
			}
			pointerStateForIndex.eventData.displayIndex = this.GetDisplayIndexFor(context.control);
		}

		private void OnRightClickCallback(InputAction.CallbackContext context)
		{
			int pointerStateIndexFor = this.GetPointerStateIndexFor(ref context);
			if (pointerStateIndexFor == -1)
			{
				return;
			}
			ref PointerModel pointerStateForIndex = ref this.GetPointerStateForIndex(pointerStateIndexFor);
			bool isPressed = pointerStateForIndex.rightButton.isPressed;
			pointerStateForIndex.rightButton.isPressed = context.ReadValueAsButton();
			pointerStateForIndex.changedThisFrame = true;
			if (this.IgnoreNextClick(ref context, isPressed))
			{
				pointerStateForIndex.rightButton.ignoreNextClick = true;
			}
			pointerStateForIndex.eventData.displayIndex = this.GetDisplayIndexFor(context.control);
		}

		private void OnMiddleClickCallback(InputAction.CallbackContext context)
		{
			int pointerStateIndexFor = this.GetPointerStateIndexFor(ref context);
			if (pointerStateIndexFor == -1)
			{
				return;
			}
			ref PointerModel pointerStateForIndex = ref this.GetPointerStateForIndex(pointerStateIndexFor);
			bool isPressed = pointerStateForIndex.middleButton.isPressed;
			pointerStateForIndex.middleButton.isPressed = context.ReadValueAsButton();
			pointerStateForIndex.changedThisFrame = true;
			if (this.IgnoreNextClick(ref context, isPressed))
			{
				pointerStateForIndex.middleButton.ignoreNextClick = true;
			}
			pointerStateForIndex.eventData.displayIndex = this.GetDisplayIndexFor(context.control);
		}

		private bool CheckForRemovedDevice(ref InputAction.CallbackContext context)
		{
			if (context.canceled && !context.control.device.added)
			{
				this.m_NeedToPurgeStalePointers = true;
				return true;
			}
			return false;
		}

		private void OnScrollCallback(InputAction.CallbackContext context)
		{
			int pointerStateIndexFor = this.GetPointerStateIndexFor(ref context);
			if (pointerStateIndexFor == -1)
			{
				return;
			}
			ref PointerModel pointerStateForIndex = ref this.GetPointerStateForIndex(pointerStateIndexFor);
			Vector2 a = context.ReadValue<Vector2>();
			pointerStateForIndex.scrollDelta = a / InputSystem.scrollWheelDeltaPerTick * this.scrollDeltaPerTick;
			pointerStateForIndex.eventData.displayIndex = this.GetDisplayIndexFor(context.control);
		}

		private void OnMoveCallback(InputAction.CallbackContext context)
		{
			this.m_NavigationState.move = context.ReadValue<Vector2>();
			this.m_NavigationState.device = context.control.device;
		}

		private void OnSubmitCancelCallback(InputAction.CallbackContext context)
		{
			this.m_SubmitCancelState.device = context.control.device;
		}

		private void OnTrackedDeviceOrientationCallback(InputAction.CallbackContext context)
		{
			int pointerStateIndexFor = this.GetPointerStateIndexFor(ref context);
			if (pointerStateIndexFor == -1)
			{
				return;
			}
			ref PointerModel pointerStateForIndex = ref this.GetPointerStateForIndex(pointerStateIndexFor);
			pointerStateForIndex.worldOrientation = context.ReadValue<Quaternion>();
			pointerStateForIndex.eventData.displayIndex = this.GetDisplayIndexFor(context.control);
		}

		private void OnTrackedDevicePositionCallback(InputAction.CallbackContext context)
		{
			int pointerStateIndexFor = this.GetPointerStateIndexFor(ref context);
			if (pointerStateIndexFor == -1)
			{
				return;
			}
			ref PointerModel pointerStateForIndex = ref this.GetPointerStateForIndex(pointerStateIndexFor);
			pointerStateForIndex.worldPosition = context.ReadValue<Vector3>();
			pointerStateForIndex.eventData.displayIndex = this.GetDisplayIndexFor(context.control);
		}

		private void OnControlsChanged(object obj)
		{
			this.m_NeedToPurgeStalePointers = true;
		}

		private void FilterPointerStatesByType()
		{
			UIPointerType uipointerType = UIPointerType.None;
			for (int i = 0; i < this.m_PointerStates.length; i++)
			{
				ref PointerModel pointerStateForIndex = ref this.GetPointerStateForIndex(i);
				pointerStateForIndex.eventData.ReadDeviceState();
				pointerStateForIndex.CopyTouchOrPenStateFrom(pointerStateForIndex.eventData);
				if (pointerStateForIndex.changedThisFrame && uipointerType == UIPointerType.None)
				{
					uipointerType = pointerStateForIndex.pointerType;
				}
			}
			if (this.m_PointerBehavior == UIPointerBehavior.SingleMouseOrPenButMultiTouchAndTrack && uipointerType != UIPointerType.None)
			{
				if (uipointerType == UIPointerType.MouseOrPen)
				{
					for (int j = 0; j < this.m_PointerStates.length; j++)
					{
						if (this.m_PointerStates[j].pointerType != UIPointerType.MouseOrPen && this.SendPointerExitEventsAndRemovePointer(j))
						{
							j--;
						}
					}
					return;
				}
				for (int k = 0; k < this.m_PointerStates.length; k++)
				{
					if (this.m_PointerStates[k].pointerType == UIPointerType.MouseOrPen && this.SendPointerExitEventsAndRemovePointer(k))
					{
						k--;
					}
				}
			}
		}

		public override void Process()
		{
			if (this.m_NeedToPurgeStalePointers)
			{
				this.PurgeStalePointers();
			}
			if (!base.eventSystem.isFocused && !this.shouldIgnoreFocus)
			{
				for (int i = 0; i < this.m_PointerStates.length; i++)
				{
					this.m_PointerStates[i].OnFrameFinished();
				}
				return;
			}
			this.ProcessNavigation(ref this.m_NavigationState);
			this.FilterPointerStatesByType();
			for (int j = 0; j < this.m_PointerStates.length; j++)
			{
				ref PointerModel pointerStateForIndex = ref this.GetPointerStateForIndex(j);
				this.ProcessPointer(ref pointerStateForIndex);
				if (pointerStateForIndex.pointerType == UIPointerType.Touch && !pointerStateForIndex.leftButton.isPressed && !pointerStateForIndex.leftButton.wasReleasedThisFrame)
				{
					if (this.RemovePointerAtIndex(j))
					{
						j--;
					}
				}
				else
				{
					pointerStateForIndex.OnFrameFinished();
				}
			}
		}

		public override int ConvertUIToolkitPointerId(PointerEventData sourcePointerData)
		{
			if (this.m_PointerBehavior == UIPointerBehavior.SingleUnifiedPointer)
			{
				return PointerId.mousePointerId;
			}
			ExtendedPointerEventData extendedPointerEventData = sourcePointerData as ExtendedPointerEventData;
			if (extendedPointerEventData == null)
			{
				return base.ConvertUIToolkitPointerId(sourcePointerData);
			}
			return extendedPointerEventData.uiToolkitPointerId;
		}

		public override Vector2 ConvertPointerEventScrollDeltaToTicks(Vector2 scrollDelta)
		{
			if (Mathf.Abs(this.scrollDeltaPerTick) < 1E-05f)
			{
				return Vector2.zero;
			}
			return scrollDelta / this.scrollDeltaPerTick;
		}

		public override NavigationDeviceType GetNavigationEventDeviceType(BaseEventData eventData)
		{
			INavigationEventData navigationEventData = eventData as INavigationEventData;
			if (navigationEventData == null)
			{
				return NavigationDeviceType.Unknown;
			}
			if (navigationEventData.device is Keyboard)
			{
				return NavigationDeviceType.Keyboard;
			}
			return NavigationDeviceType.NonKeyboard;
		}

		private void HookActions()
		{
			if (this.m_ActionsHooked)
			{
				return;
			}
			if (this.m_OnPointDelegate == null)
			{
				this.m_OnPointDelegate = new Action<InputAction.CallbackContext>(this.OnPointCallback);
			}
			if (this.m_OnLeftClickDelegate == null)
			{
				this.m_OnLeftClickDelegate = new Action<InputAction.CallbackContext>(this.OnLeftClickCallback);
			}
			if (this.m_OnRightClickDelegate == null)
			{
				this.m_OnRightClickDelegate = new Action<InputAction.CallbackContext>(this.OnRightClickCallback);
			}
			if (this.m_OnMiddleClickDelegate == null)
			{
				this.m_OnMiddleClickDelegate = new Action<InputAction.CallbackContext>(this.OnMiddleClickCallback);
			}
			if (this.m_OnScrollWheelDelegate == null)
			{
				this.m_OnScrollWheelDelegate = new Action<InputAction.CallbackContext>(this.OnScrollCallback);
			}
			if (this.m_OnMoveDelegate == null)
			{
				this.m_OnMoveDelegate = new Action<InputAction.CallbackContext>(this.OnMoveCallback);
			}
			if (this.m_OnSubmitCancelDelegate == null)
			{
				this.m_OnSubmitCancelDelegate = new Action<InputAction.CallbackContext>(this.OnSubmitCancelCallback);
			}
			if (this.m_OnTrackedDeviceOrientationDelegate == null)
			{
				this.m_OnTrackedDeviceOrientationDelegate = new Action<InputAction.CallbackContext>(this.OnTrackedDeviceOrientationCallback);
			}
			if (this.m_OnTrackedDevicePositionDelegate == null)
			{
				this.m_OnTrackedDevicePositionDelegate = new Action<InputAction.CallbackContext>(this.OnTrackedDevicePositionCallback);
			}
			this.SetActionCallbacks(true);
		}

		private void UnhookActions()
		{
			if (!this.m_ActionsHooked)
			{
				return;
			}
			this.SetActionCallbacks(false);
		}

		private void SetActionCallbacks(bool install)
		{
			this.m_ActionsHooked = install;
			InputSystemUIInputModule.SetActionCallback(this.m_PointAction, this.m_OnPointDelegate, install);
			InputSystemUIInputModule.SetActionCallback(this.m_MoveAction, this.m_OnMoveDelegate, install);
			InputSystemUIInputModule.SetActionCallback(this.m_SubmitAction, this.m_OnSubmitCancelDelegate, install);
			InputSystemUIInputModule.SetActionCallback(this.m_CancelAction, this.m_OnSubmitCancelDelegate, install);
			InputSystemUIInputModule.SetActionCallback(this.m_LeftClickAction, this.m_OnLeftClickDelegate, install);
			InputSystemUIInputModule.SetActionCallback(this.m_RightClickAction, this.m_OnRightClickDelegate, install);
			InputSystemUIInputModule.SetActionCallback(this.m_MiddleClickAction, this.m_OnMiddleClickDelegate, install);
			InputSystemUIInputModule.SetActionCallback(this.m_ScrollWheelAction, this.m_OnScrollWheelDelegate, install);
			InputSystemUIInputModule.SetActionCallback(this.m_TrackedDeviceOrientationAction, this.m_OnTrackedDeviceOrientationDelegate, install);
			InputSystemUIInputModule.SetActionCallback(this.m_TrackedDevicePositionAction, this.m_OnTrackedDevicePositionDelegate, install);
		}

		private static void SetActionCallback(InputActionReference actionReference, Action<InputAction.CallbackContext> callback, bool install)
		{
			if (!install && callback == null)
			{
				return;
			}
			if (actionReference == null)
			{
				return;
			}
			InputAction action = actionReference.action;
			if (action == null)
			{
				return;
			}
			if (install)
			{
				action.performed += callback;
				action.canceled += callback;
				return;
			}
			action.performed -= callback;
			action.canceled -= callback;
		}

		private InputActionReference UpdateReferenceForNewAsset(InputActionReference actionReference)
		{
			InputAction inputAction = (actionReference != null) ? actionReference.action : null;
			if (inputAction == null)
			{
				return null;
			}
			InputActionMap actionMap = inputAction.actionMap;
			InputActionAsset actionsAsset = this.m_ActionsAsset;
			InputActionMap inputActionMap = (actionsAsset != null) ? actionsAsset.FindActionMap(actionMap.name, false) : null;
			if (inputActionMap == null)
			{
				return null;
			}
			InputAction inputAction2 = inputActionMap.FindAction(inputAction.name, false);
			if (inputAction2 == null)
			{
				return null;
			}
			return InputActionReference.Create(inputAction2);
		}

		public InputActionAsset actionsAsset
		{
			get
			{
				return this.m_ActionsAsset;
			}
			set
			{
				if (value != this.m_ActionsAsset)
				{
					this.UnhookActions();
					this.m_ActionsAsset = value;
					this.point = this.UpdateReferenceForNewAsset(this.point);
					this.move = this.UpdateReferenceForNewAsset(this.move);
					this.leftClick = this.UpdateReferenceForNewAsset(this.leftClick);
					this.rightClick = this.UpdateReferenceForNewAsset(this.rightClick);
					this.middleClick = this.UpdateReferenceForNewAsset(this.middleClick);
					this.scrollWheel = this.UpdateReferenceForNewAsset(this.scrollWheel);
					this.submit = this.UpdateReferenceForNewAsset(this.submit);
					this.cancel = this.UpdateReferenceForNewAsset(this.cancel);
					this.trackedDeviceOrientation = this.UpdateReferenceForNewAsset(this.trackedDeviceOrientation);
					this.trackedDevicePosition = this.UpdateReferenceForNewAsset(this.trackedDevicePosition);
					this.HookActions();
				}
			}
		}

		internal new bool sendPointerHoverToParent
		{
			get
			{
				return base.sendPointerHoverToParent;
			}
			set
			{
				base.sendPointerHoverToParent = value;
			}
		}

		private const float kClickSpeed = 0.3f;

		[FormerlySerializedAs("m_RepeatDelay")]
		[Tooltip("The Initial delay (in seconds) between an initial move action and a repeated move action.")]
		[SerializeField]
		private float m_MoveRepeatDelay = 0.5f;

		[FormerlySerializedAs("m_RepeatRate")]
		[Tooltip("The speed (in seconds) that the move action repeats itself once repeating (max 1 per frame).")]
		[SerializeField]
		private float m_MoveRepeatRate = 0.1f;

		[Tooltip("Scales the Eventsystem.DragThreshold, for tracked devices, to make selection easier.")]
		private float m_TrackedDeviceDragThresholdMultiplier = 2f;

		[Tooltip("Transform representing the real world origin for tracking devices. When using the XR Interaction Toolkit, this should be pointing to the XR Rig's Transform.")]
		[SerializeField]
		private Transform m_XRTrackingOrigin;

		private static DefaultInputActions defaultActions;

		private const float kSmallestScrollDeltaPerTick = 1E-05f;

		[SerializeField]
		[HideInInspector]
		private InputActionAsset m_ActionsAsset;

		[SerializeField]
		[HideInInspector]
		private InputActionReference m_PointAction;

		[SerializeField]
		[HideInInspector]
		private InputActionReference m_MoveAction;

		[SerializeField]
		[HideInInspector]
		private InputActionReference m_SubmitAction;

		[SerializeField]
		[HideInInspector]
		private InputActionReference m_CancelAction;

		[SerializeField]
		[HideInInspector]
		private InputActionReference m_LeftClickAction;

		[SerializeField]
		[HideInInspector]
		private InputActionReference m_MiddleClickAction;

		[SerializeField]
		[HideInInspector]
		private InputActionReference m_RightClickAction;

		[SerializeField]
		[HideInInspector]
		private InputActionReference m_ScrollWheelAction;

		[SerializeField]
		[HideInInspector]
		private InputActionReference m_TrackedDevicePositionAction;

		[SerializeField]
		[HideInInspector]
		private InputActionReference m_TrackedDeviceOrientationAction;

		[SerializeField]
		private bool m_DeselectOnBackgroundClick = true;

		[SerializeField]
		private UIPointerBehavior m_PointerBehavior;

		[SerializeField]
		[HideInInspector]
		internal InputSystemUIInputModule.CursorLockBehavior m_CursorLockBehavior;

		[SerializeField]
		private float m_ScrollDeltaPerTick = 6f;

		private static Dictionary<InputAction, InputSystemUIInputModule.InputActionReferenceState> s_InputActionReferenceCounts = new Dictionary<InputAction, InputSystemUIInputModule.InputActionReferenceState>();

		[NonSerialized]
		private bool m_ActionsHooked;

		[NonSerialized]
		private bool m_NeedToPurgeStalePointers;

		private Action<InputAction.CallbackContext> m_OnPointDelegate;

		private Action<InputAction.CallbackContext> m_OnMoveDelegate;

		private Action<InputAction.CallbackContext> m_OnSubmitCancelDelegate;

		private Action<InputAction.CallbackContext> m_OnLeftClickDelegate;

		private Action<InputAction.CallbackContext> m_OnRightClickDelegate;

		private Action<InputAction.CallbackContext> m_OnMiddleClickDelegate;

		private Action<InputAction.CallbackContext> m_OnScrollWheelDelegate;

		private Action<InputAction.CallbackContext> m_OnTrackedDevicePositionDelegate;

		private Action<InputAction.CallbackContext> m_OnTrackedDeviceOrientationDelegate;

		private Action<object> m_OnControlsChangedDelegate;

		[NonSerialized]
		private int m_CurrentPointerId = -1;

		[NonSerialized]
		private int m_CurrentPointerIndex = -1;

		[NonSerialized]
		internal UIPointerType m_CurrentPointerType;

		internal InlinedArray<int> m_PointerIds;

		internal InlinedArray<PointerModel> m_PointerStates;

		private NavigationModel m_NavigationState;

		private SubmitCancelModel m_SubmitCancelState;

		[NonSerialized]
		private GameObject m_LocalMultiPlayerRoot;

		private struct InputActionReferenceState
		{
			public int refCount;

			public bool enabledByInputModule;
		}

		public enum CursorLockBehavior
		{
			OutsideScreen,
			ScreenCenter
		}
	}
}
