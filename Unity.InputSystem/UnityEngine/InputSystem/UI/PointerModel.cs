using System;
using UnityEngine.EventSystems;

namespace UnityEngine.InputSystem.UI
{
	internal struct PointerModel
	{
		public UIPointerType pointerType
		{
			get
			{
				return this.eventData.pointerType;
			}
		}

		public Vector2 screenPosition
		{
			get
			{
				return this.m_ScreenPosition;
			}
			set
			{
				if (this.m_ScreenPosition != value)
				{
					this.m_ScreenPosition = value;
					this.changedThisFrame = true;
				}
			}
		}

		public Vector3 worldPosition
		{
			get
			{
				return this.m_WorldPosition;
			}
			set
			{
				if (this.m_WorldPosition != value)
				{
					this.m_WorldPosition = value;
					this.changedThisFrame = true;
				}
			}
		}

		public Quaternion worldOrientation
		{
			get
			{
				return this.m_WorldOrientation;
			}
			set
			{
				if (this.m_WorldOrientation != value)
				{
					this.m_WorldOrientation = value;
					this.changedThisFrame = true;
				}
			}
		}

		public Vector2 scrollDelta
		{
			get
			{
				return this.m_ScrollDelta;
			}
			set
			{
				if (this.m_ScrollDelta != value)
				{
					this.changedThisFrame = true;
					this.m_ScrollDelta = value;
				}
			}
		}

		public float pressure
		{
			get
			{
				return this.m_Pressure;
			}
			set
			{
				if (this.m_Pressure != value)
				{
					this.changedThisFrame = true;
					this.m_Pressure = value;
				}
			}
		}

		public float azimuthAngle
		{
			get
			{
				return this.m_AzimuthAngle;
			}
			set
			{
				if (this.m_AzimuthAngle != value)
				{
					this.changedThisFrame = true;
					this.m_AzimuthAngle = value;
				}
			}
		}

		public float altitudeAngle
		{
			get
			{
				return this.m_AltitudeAngle;
			}
			set
			{
				if (this.m_AltitudeAngle != value)
				{
					this.changedThisFrame = true;
					this.m_AltitudeAngle = value;
				}
			}
		}

		public float twist
		{
			get
			{
				return this.m_Twist;
			}
			set
			{
				if (this.m_Twist != value)
				{
					this.changedThisFrame = true;
					this.m_Twist = value;
				}
			}
		}

		public Vector2 radius
		{
			get
			{
				return this.m_Radius;
			}
			set
			{
				if (this.m_Radius != value)
				{
					this.changedThisFrame = true;
					this.m_Radius = value;
				}
			}
		}

		public PointerModel(ExtendedPointerEventData eventData)
		{
			this.eventData = eventData;
			this.changedThisFrame = false;
			this.leftButton = default(PointerModel.ButtonState);
			this.leftButton.OnEndFrame();
			this.rightButton = default(PointerModel.ButtonState);
			this.rightButton.OnEndFrame();
			this.middleButton = default(PointerModel.ButtonState);
			this.middleButton.OnEndFrame();
			this.m_ScreenPosition = default(Vector2);
			this.m_ScrollDelta = default(Vector2);
			this.m_WorldOrientation = default(Quaternion);
			this.m_WorldPosition = default(Vector3);
			this.m_Pressure = 0f;
			this.m_AzimuthAngle = 0f;
			this.m_AltitudeAngle = 0f;
			this.m_Twist = 0f;
			this.m_Radius = default(Vector2);
		}

		public void OnFrameFinished()
		{
			this.changedThisFrame = false;
			this.scrollDelta = default(Vector2);
			this.leftButton.OnEndFrame();
			this.rightButton.OnEndFrame();
			this.middleButton.OnEndFrame();
		}

		public void CopyTouchOrPenStateFrom(PointerEventData eventData)
		{
			this.pressure = eventData.pressure;
			this.azimuthAngle = eventData.azimuthAngle;
			this.altitudeAngle = eventData.altitudeAngle;
			this.twist = eventData.twist;
			this.radius = eventData.radius;
		}

		public bool changedThisFrame;

		public PointerModel.ButtonState leftButton;

		public PointerModel.ButtonState rightButton;

		public PointerModel.ButtonState middleButton;

		public ExtendedPointerEventData eventData;

		private Vector2 m_ScreenPosition;

		private Vector2 m_ScrollDelta;

		private Vector3 m_WorldPosition;

		private Quaternion m_WorldOrientation;

		private float m_Pressure;

		private float m_AzimuthAngle;

		private float m_AltitudeAngle;

		private float m_Twist;

		private Vector2 m_Radius;

		public struct ButtonState
		{
			public bool isPressed
			{
				get
				{
					return this.m_IsPressed;
				}
				set
				{
					if (this.m_IsPressed != value)
					{
						this.m_IsPressed = value;
						if (this.m_FramePressState == PointerEventData.FramePressState.NotChanged && value)
						{
							this.m_FramePressState = PointerEventData.FramePressState.Pressed;
							return;
						}
						if (this.m_FramePressState == PointerEventData.FramePressState.NotChanged && !value)
						{
							this.m_FramePressState = PointerEventData.FramePressState.Released;
							return;
						}
						if (this.m_FramePressState == PointerEventData.FramePressState.Pressed && !value)
						{
							this.m_FramePressState = PointerEventData.FramePressState.PressedAndReleased;
						}
					}
				}
			}

			public bool ignoreNextClick
			{
				get
				{
					return this.m_IgnoreNextClick;
				}
				set
				{
					this.m_IgnoreNextClick = value;
				}
			}

			public float pressTime
			{
				get
				{
					return this.m_PressTime;
				}
				set
				{
					this.m_PressTime = value;
				}
			}

			public bool clickedOnSameGameObject
			{
				get
				{
					return this.m_ClickedOnSameGameObject;
				}
				set
				{
					this.m_ClickedOnSameGameObject = value;
				}
			}

			public bool wasPressedThisFrame
			{
				get
				{
					return this.m_FramePressState == PointerEventData.FramePressState.Pressed || this.m_FramePressState == PointerEventData.FramePressState.PressedAndReleased;
				}
			}

			public bool wasReleasedThisFrame
			{
				get
				{
					return this.m_FramePressState == PointerEventData.FramePressState.Released || this.m_FramePressState == PointerEventData.FramePressState.PressedAndReleased;
				}
			}

			public void CopyPressStateTo(PointerEventData eventData)
			{
				eventData.pointerPressRaycast = this.m_PressRaycast;
				eventData.pressPosition = this.m_PressPosition;
				eventData.clickCount = this.m_ClickCount;
				eventData.clickTime = this.m_ClickTime;
				eventData.pointerPress = this.m_LastPressObject;
				eventData.pointerPress = this.m_PressObject;
				eventData.rawPointerPress = this.m_RawPressObject;
				eventData.pointerDrag = this.m_DragObject;
				eventData.dragging = this.m_Dragging;
				if (this.ignoreNextClick)
				{
					eventData.eligibleForClick = false;
				}
			}

			public void CopyPressStateFrom(PointerEventData eventData)
			{
				this.m_PressRaycast = eventData.pointerPressRaycast;
				this.m_PressObject = eventData.pointerPress;
				this.m_RawPressObject = eventData.rawPointerPress;
				this.m_LastPressObject = eventData.lastPress;
				this.m_PressPosition = eventData.pressPosition;
				this.m_ClickTime = eventData.clickTime;
				this.m_ClickCount = eventData.clickCount;
				this.m_DragObject = eventData.pointerDrag;
				this.m_Dragging = eventData.dragging;
			}

			public void OnEndFrame()
			{
				this.m_FramePressState = PointerEventData.FramePressState.NotChanged;
			}

			private bool m_IsPressed;

			private PointerEventData.FramePressState m_FramePressState;

			private float m_PressTime;

			private RaycastResult m_PressRaycast;

			private GameObject m_PressObject;

			private GameObject m_RawPressObject;

			private GameObject m_LastPressObject;

			private GameObject m_DragObject;

			private Vector2 m_PressPosition;

			private float m_ClickTime;

			private int m_ClickCount;

			private bool m_Dragging;

			private bool m_ClickedOnSameGameObject;

			private bool m_IgnoreNextClick;
		}
	}
}
