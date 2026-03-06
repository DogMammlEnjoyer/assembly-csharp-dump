using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UnityEngine.InputSystem.OnScreen
{
	[AddComponentMenu("Input/On-Screen Stick")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.inputsystem@1.14/manual/OnScreen.html#on-screen-sticks")]
	public class OnScreenStick : OnScreenControl, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IDragHandler
	{
		public void OnPointerDown(PointerEventData eventData)
		{
			if (this.m_UseIsolatedInputActions)
			{
				return;
			}
			if (eventData == null)
			{
				throw new ArgumentNullException("eventData");
			}
			this.BeginInteraction(eventData.position, eventData.pressEventCamera);
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (this.m_UseIsolatedInputActions)
			{
				return;
			}
			if (eventData == null)
			{
				throw new ArgumentNullException("eventData");
			}
			this.MoveStick(eventData.position, eventData.pressEventCamera);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (this.m_UseIsolatedInputActions)
			{
				return;
			}
			this.EndInteraction();
		}

		private void Start()
		{
			if (this.m_UseIsolatedInputActions)
			{
				this.m_RaycastResults = new List<RaycastResult>();
				this.m_PointerEventData = new PointerEventData(EventSystem.current);
				if (this.m_PointerDownAction == null || this.m_PointerDownAction.bindings.Count == 0)
				{
					if (this.m_PointerDownAction == null)
					{
						this.m_PointerDownAction = new InputAction(null, InputActionType.PassThrough, null, null, null, null);
					}
					else if (this.m_PointerDownAction.m_Type != InputActionType.PassThrough)
					{
						this.m_PointerDownAction.m_Type = InputActionType.PassThrough;
					}
					this.m_PointerDownAction.AddBinding("<Mouse>/leftButton", null, null, null);
					this.m_PointerDownAction.AddBinding("<Pen>/tip", null, null, null);
					this.m_PointerDownAction.AddBinding("<Touchscreen>/touch*/press", null, null, null);
					this.m_PointerDownAction.AddBinding("<XRController>/trigger", null, null, null);
				}
				if (this.m_PointerMoveAction == null || this.m_PointerMoveAction.bindings.Count == 0)
				{
					if (this.m_PointerMoveAction == null)
					{
						this.m_PointerMoveAction = new InputAction();
					}
					this.m_PointerMoveAction.AddBinding("<Mouse>/position", null, null, null);
					this.m_PointerMoveAction.AddBinding("<Pen>/position", null, null, null);
					this.m_PointerMoveAction.AddBinding("<Touchscreen>/touch*/position", null, null, null);
				}
				this.m_PointerDownAction.performed += this.OnPointerChanged;
				this.m_PointerDownAction.Enable();
				this.m_PointerMoveAction.Enable();
			}
			if (!(base.transform is RectTransform))
			{
				return;
			}
			this.m_StartPos = ((RectTransform)base.transform).anchoredPosition;
			if (this.m_Behaviour != OnScreenStick.Behaviour.ExactPositionWithDynamicOrigin)
			{
				return;
			}
			this.m_PointerDownPos = this.m_StartPos;
			GameObject gameObject = new GameObject("DynamicOriginClickable", new Type[]
			{
				typeof(Image)
			});
			gameObject.transform.SetParent(base.transform);
			Image component = gameObject.GetComponent<Image>();
			component.color = new Color(1f, 1f, 1f, 0f);
			RectTransform rectTransform = (RectTransform)gameObject.transform;
			rectTransform.sizeDelta = new Vector2(this.m_DynamicOriginRange * 2f, this.m_DynamicOriginRange * 2f);
			rectTransform.localScale = new Vector3(1f, 1f, 0f);
			rectTransform.anchoredPosition3D = Vector3.zero;
			component.sprite = SpriteUtilities.CreateCircleSprite(16, new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue));
			component.alphaHitTestMinimumThreshold = 0.5f;
		}

		private void OnDestroy()
		{
			if (this.m_UseIsolatedInputActions)
			{
				this.m_PointerDownAction.performed -= this.OnPointerChanged;
			}
		}

		private void BeginInteraction(Vector2 pointerPosition, Camera uiCamera)
		{
			RectTransform canvasRectTransform = UGUIOnScreenControlUtils.GetCanvasRectTransform(base.transform);
			if (canvasRectTransform == null)
			{
				Debug.LogError(base.GetWarningMessage());
				return;
			}
			switch (this.m_Behaviour)
			{
			case OnScreenStick.Behaviour.RelativePositionWithStaticOrigin:
				RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, pointerPosition, uiCamera, out this.m_PointerDownPos);
				return;
			case OnScreenStick.Behaviour.ExactPositionWithStaticOrigin:
				RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, pointerPosition, uiCamera, out this.m_PointerDownPos);
				this.MoveStick(pointerPosition, uiCamera);
				return;
			case OnScreenStick.Behaviour.ExactPositionWithDynamicOrigin:
			{
				Vector2 anchoredPosition;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, pointerPosition, uiCamera, out anchoredPosition);
				this.m_PointerDownPos = (((RectTransform)base.transform).anchoredPosition = anchoredPosition);
				return;
			}
			default:
				return;
			}
		}

		private void MoveStick(Vector2 pointerPosition, Camera uiCamera)
		{
			RectTransform canvasRectTransform = UGUIOnScreenControlUtils.GetCanvasRectTransform(base.transform);
			if (canvasRectTransform == null)
			{
				Debug.LogError(base.GetWarningMessage());
				return;
			}
			Vector2 a;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, pointerPosition, uiCamera, out a);
			Vector2 vector = a - this.m_PointerDownPos;
			switch (this.m_Behaviour)
			{
			case OnScreenStick.Behaviour.RelativePositionWithStaticOrigin:
				vector = Vector2.ClampMagnitude(vector, this.movementRange);
				((RectTransform)base.transform).anchoredPosition = this.m_StartPos + vector;
				break;
			case OnScreenStick.Behaviour.ExactPositionWithStaticOrigin:
				vector = a - this.m_StartPos;
				vector = Vector2.ClampMagnitude(vector, this.movementRange);
				((RectTransform)base.transform).anchoredPosition = this.m_StartPos + vector;
				break;
			case OnScreenStick.Behaviour.ExactPositionWithDynamicOrigin:
				vector = Vector2.ClampMagnitude(vector, this.movementRange);
				((RectTransform)base.transform).anchoredPosition = this.m_PointerDownPos + vector;
				break;
			}
			Vector2 value = new Vector2(vector.x / this.movementRange, vector.y / this.movementRange);
			base.SendValueToControl<Vector2>(value);
		}

		private void EndInteraction()
		{
			((RectTransform)base.transform).anchoredPosition = (this.m_PointerDownPos = this.m_StartPos);
			base.SendValueToControl<Vector2>(Vector2.zero);
		}

		private void OnPointerDown(InputAction.CallbackContext ctx)
		{
			if (this.m_IsIsolationActive)
			{
				return;
			}
			Vector2 vector = Vector2.zero;
			TouchControl touchControl = null;
			InputControl control = ctx.control;
			TouchControl touchControl2 = ((control != null) ? control.parent : null) as TouchControl;
			if (touchControl2 != null)
			{
				touchControl = touchControl2;
				vector = touchControl2.position.ReadValue();
			}
			else
			{
				InputControl control2 = ctx.control;
				Pointer pointer = ((control2 != null) ? control2.device : null) as Pointer;
				if (pointer != null)
				{
					vector = pointer.position.ReadValue();
				}
			}
			this.m_PointerEventData.position = vector;
			EventSystem.current.RaycastAll(this.m_PointerEventData, this.m_RaycastResults);
			if (this.m_RaycastResults.Count == 0)
			{
				return;
			}
			bool flag = false;
			foreach (RaycastResult raycastResult in this.m_RaycastResults)
			{
				if (!(raycastResult.gameObject != base.gameObject))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return;
			}
			this.BeginInteraction(vector, this.GetCameraFromCanvas());
			if (touchControl != null)
			{
				this.m_TouchControl = touchControl;
				this.m_PointerMoveAction.ApplyBindingOverride(touchControl.path + "/position", null, "<Touchscreen>/touch*/position");
			}
			this.m_PointerMoveAction.performed += this.OnPointerMove;
			this.m_IsIsolationActive = true;
		}

		private void OnPointerChanged(InputAction.CallbackContext ctx)
		{
			if (ctx.control.IsPressed(0f))
			{
				this.OnPointerDown(ctx);
				return;
			}
			this.OnPointerUp(ctx);
		}

		private void OnPointerMove(InputAction.CallbackContext ctx)
		{
			Vector2 pointerPosition;
			if (this.m_TouchControl != null)
			{
				if (!this.m_TouchControl.isInProgress)
				{
					return;
				}
				pointerPosition = this.m_TouchControl.position.ReadValue();
			}
			else
			{
				pointerPosition = ((Pointer)ctx.control.device).position.ReadValue();
			}
			this.MoveStick(pointerPosition, this.GetCameraFromCanvas());
		}

		private void OnPointerUp(InputAction.CallbackContext ctx)
		{
			if (!this.m_IsIsolationActive)
			{
				return;
			}
			if (this.m_TouchControl != null)
			{
				if (this.m_TouchControl.isInProgress)
				{
					return;
				}
				this.m_PointerMoveAction.ApplyBindingOverride(null, null, "<Touchscreen>/touch*/position");
				this.m_TouchControl = null;
			}
			this.EndInteraction();
			this.m_PointerMoveAction.performed -= this.OnPointerMove;
			this.m_IsIsolationActive = false;
		}

		private Camera GetCameraFromCanvas()
		{
			Canvas componentInParent = base.GetComponentInParent<Canvas>();
			RenderMode? renderMode = (componentInParent != null) ? new RenderMode?(componentInParent.renderMode) : null;
			RenderMode? renderMode2 = renderMode;
			RenderMode renderMode3 = RenderMode.ScreenSpaceOverlay;
			if (!(renderMode2.GetValueOrDefault() == renderMode3 & renderMode2 != null))
			{
				renderMode2 = renderMode;
				renderMode3 = RenderMode.ScreenSpaceCamera;
				if (!(renderMode2.GetValueOrDefault() == renderMode3 & renderMode2 != null) || !(((componentInParent != null) ? componentInParent.worldCamera : null) == null))
				{
					return ((componentInParent != null) ? componentInParent.worldCamera : null) ?? Camera.main;
				}
			}
			return null;
		}

		private void OnDrawGizmosSelected()
		{
			RectTransform rectTransform = base.transform.parent as RectTransform;
			if (rectTransform == null)
			{
				return;
			}
			Gizmos.matrix = rectTransform.localToWorldMatrix;
			Vector2 vector = rectTransform.anchoredPosition;
			if (Application.isPlaying)
			{
				vector = this.m_StartPos;
			}
			Gizmos.color = new Color32(84, 173, 219, byte.MaxValue);
			Vector2 center = vector;
			if (Application.isPlaying && this.m_Behaviour == OnScreenStick.Behaviour.ExactPositionWithDynamicOrigin)
			{
				center = this.m_PointerDownPos;
			}
			this.DrawGizmoCircle(center, this.m_MovementRange);
			if (this.m_Behaviour != OnScreenStick.Behaviour.ExactPositionWithDynamicOrigin)
			{
				return;
			}
			Gizmos.color = new Color32(158, 84, 219, byte.MaxValue);
			this.DrawGizmoCircle(vector, this.m_DynamicOriginRange);
		}

		private void DrawGizmoCircle(Vector2 center, float radius)
		{
			for (int i = 0; i < 32; i++)
			{
				float f = (float)i / 32f * 3.1415927f * 2f;
				float f2 = (float)(i + 1) / 32f * 3.1415927f * 2f;
				Gizmos.DrawLine(new Vector3(center.x + Mathf.Cos(f) * radius, center.y + Mathf.Sin(f) * radius, 0f), new Vector3(center.x + Mathf.Cos(f2) * radius, center.y + Mathf.Sin(f2) * radius, 0f));
			}
		}

		private void UpdateDynamicOriginClickableArea()
		{
			Transform transform = base.transform.Find("DynamicOriginClickable");
			if (transform)
			{
				((RectTransform)transform).sizeDelta = new Vector2(this.m_DynamicOriginRange * 2f, this.m_DynamicOriginRange * 2f);
			}
		}

		public float movementRange
		{
			get
			{
				return this.m_MovementRange;
			}
			set
			{
				this.m_MovementRange = value;
			}
		}

		public float dynamicOriginRange
		{
			get
			{
				return this.m_DynamicOriginRange;
			}
			set
			{
				if (this.m_DynamicOriginRange != value)
				{
					this.m_DynamicOriginRange = value;
					this.UpdateDynamicOriginClickableArea();
				}
			}
		}

		public bool useIsolatedInputActions
		{
			get
			{
				return this.m_UseIsolatedInputActions;
			}
			set
			{
				this.m_UseIsolatedInputActions = value;
			}
		}

		protected override string controlPathInternal
		{
			get
			{
				return this.m_ControlPath;
			}
			set
			{
				this.m_ControlPath = value;
			}
		}

		public OnScreenStick.Behaviour behaviour
		{
			get
			{
				return this.m_Behaviour;
			}
			set
			{
				this.m_Behaviour = value;
			}
		}

		private const string kDynamicOriginClickable = "DynamicOriginClickable";

		[FormerlySerializedAs("movementRange")]
		[SerializeField]
		[Min(0f)]
		private float m_MovementRange = 50f;

		[SerializeField]
		[Tooltip("Defines the circular region where the onscreen control may have it's origin placed.")]
		[Min(0f)]
		private float m_DynamicOriginRange = 100f;

		[InputControl(layout = "Vector2")]
		[SerializeField]
		private string m_ControlPath;

		[SerializeField]
		[Tooltip("Choose how the onscreen stick will move relative to it's origin and the press position.\n\nRelativePositionWithStaticOrigin: The control's center of origin is fixed. The control will begin un-actuated at it's centered position and then move relative to the pointer or finger motion.\n\nExactPositionWithStaticOrigin: The control's center of origin is fixed. The stick will immediately jump to the exact position of the click or touch and begin tracking motion from there.\n\nExactPositionWithDynamicOrigin: The control's center of origin is determined by the initial press position. The stick will begin un-actuated at this center position and then track the current pointer or finger position.")]
		private OnScreenStick.Behaviour m_Behaviour;

		[SerializeField]
		[Tooltip("Set this to true to prevent cancellation of pointer events due to device switching. Cancellation will appear as the stick jumping back and forth between the pointer position and the stick center.")]
		private bool m_UseIsolatedInputActions;

		[SerializeField]
		[Tooltip("The action that will be used to detect pointer down events on the stick control. Note that if no bindings are set, default ones will be provided.")]
		private InputAction m_PointerDownAction;

		[SerializeField]
		[Tooltip("The action that will be used to detect pointer movement on the stick control. Note that if no bindings are set, default ones will be provided.")]
		private InputAction m_PointerMoveAction;

		private Vector3 m_StartPos;

		private Vector2 m_PointerDownPos;

		[NonSerialized]
		private List<RaycastResult> m_RaycastResults;

		[NonSerialized]
		private PointerEventData m_PointerEventData;

		[NonSerialized]
		private TouchControl m_TouchControl;

		[NonSerialized]
		private bool m_IsIsolationActive;

		public enum Behaviour
		{
			RelativePositionWithStaticOrigin,
			ExactPositionWithStaticOrigin,
			ExactPositionWithDynamicOrigin
		}
	}
}
