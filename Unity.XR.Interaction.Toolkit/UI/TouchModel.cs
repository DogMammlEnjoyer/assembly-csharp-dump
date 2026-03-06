using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
	internal struct TouchModel
	{
		public readonly int pointerId { get; }

		public TouchPhase selectPhase
		{
			get
			{
				return this.m_SelectPhase;
			}
			set
			{
				if (this.m_SelectPhase != value)
				{
					if (value == TouchPhase.Began)
					{
						this.selectDelta |= ButtonDeltaState.Pressed;
					}
					if (value == TouchPhase.Ended || value == TouchPhase.Canceled)
					{
						this.selectDelta |= ButtonDeltaState.Released;
					}
					this.m_SelectPhase = value;
					this.changedThisFrame = true;
				}
			}
		}

		public ButtonDeltaState selectDelta { readonly get; private set; }

		public bool changedThisFrame { readonly get; private set; }

		public Vector2 position
		{
			get
			{
				return this.m_Position;
			}
			set
			{
				if (this.m_Position != value)
				{
					this.deltaPosition = value - this.m_Position;
					this.m_Position = value;
					this.changedThisFrame = true;
				}
			}
		}

		public Vector2 deltaPosition { readonly get; private set; }

		public TouchModel(int pointerId)
		{
			this.pointerId = pointerId;
			this.m_Position = (this.deltaPosition = Vector2.zero);
			this.m_SelectPhase = TouchPhase.Canceled;
			this.changedThisFrame = false;
			this.selectDelta = ButtonDeltaState.NoChange;
			this.m_ImplementationData = default(TouchModel.ImplementationData);
			this.m_ImplementationData.Reset();
		}

		public void Reset()
		{
			this.m_Position = (this.deltaPosition = Vector2.zero);
			this.changedThisFrame = false;
			this.selectDelta = ButtonDeltaState.NoChange;
			this.m_ImplementationData.Reset();
		}

		public void OnFrameFinished()
		{
			this.deltaPosition = Vector2.zero;
			this.selectDelta = ButtonDeltaState.NoChange;
			this.changedThisFrame = false;
		}

		public void CopyTo(PointerEventData eventData)
		{
			eventData.pointerId = this.pointerId;
			eventData.position = this.position;
			eventData.delta = (((this.selectDelta & ButtonDeltaState.Pressed) != ButtonDeltaState.NoChange) ? Vector2.zero : this.deltaPosition);
			eventData.pointerEnter = this.m_ImplementationData.pointerTarget;
			eventData.dragging = this.m_ImplementationData.isDragging;
			eventData.clickTime = this.m_ImplementationData.pressedTime;
			eventData.pressPosition = this.m_ImplementationData.pressedPosition;
			eventData.pointerPressRaycast = this.m_ImplementationData.pressedRaycast;
			eventData.pointerPress = this.m_ImplementationData.pressedGameObject;
			eventData.rawPointerPress = this.m_ImplementationData.pressedGameObjectRaw;
			eventData.pointerDrag = this.m_ImplementationData.draggedGameObject;
			eventData.hovered.Clear();
			eventData.hovered.AddRange(this.m_ImplementationData.hoverTargets);
		}

		public void CopyFrom(PointerEventData eventData)
		{
			this.m_ImplementationData.pointerTarget = eventData.pointerEnter;
			this.m_ImplementationData.isDragging = eventData.dragging;
			this.m_ImplementationData.pressedTime = eventData.clickTime;
			this.m_ImplementationData.pressedPosition = eventData.pressPosition;
			this.m_ImplementationData.pressedRaycast = eventData.pointerPressRaycast;
			this.m_ImplementationData.pressedGameObject = eventData.pointerPress;
			this.m_ImplementationData.pressedGameObjectRaw = eventData.rawPointerPress;
			this.m_ImplementationData.draggedGameObject = eventData.pointerDrag;
			this.m_ImplementationData.hoverTargets.Clear();
			this.m_ImplementationData.hoverTargets.AddRange(eventData.hovered);
		}

		private TouchPhase m_SelectPhase;

		private Vector2 m_Position;

		private TouchModel.ImplementationData m_ImplementationData;

		internal struct ImplementationData
		{
			public List<GameObject> hoverTargets { readonly get; set; }

			public GameObject pointerTarget { readonly get; set; }

			public bool isDragging { readonly get; set; }

			public float pressedTime { readonly get; set; }

			public Vector2 pressedPosition { readonly get; set; }

			public RaycastResult pressedRaycast { readonly get; set; }

			public GameObject pressedGameObject { readonly get; set; }

			public GameObject pressedGameObjectRaw { readonly get; set; }

			public GameObject draggedGameObject { readonly get; set; }

			public void Reset()
			{
				this.isDragging = false;
				this.pressedTime = 0f;
				this.pressedPosition = Vector2.zero;
				this.pressedRaycast = default(RaycastResult);
				this.pressedGameObject = (this.pressedGameObjectRaw = (this.draggedGameObject = null));
				if (this.hoverTargets == null)
				{
					this.hoverTargets = new List<GameObject>();
					return;
				}
				this.hoverTargets.Clear();
			}
		}
	}
}
