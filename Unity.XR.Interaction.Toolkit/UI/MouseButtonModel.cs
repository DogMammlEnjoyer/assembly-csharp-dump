using System;
using UnityEngine.EventSystems;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
	public struct MouseButtonModel
	{
		public bool isDown
		{
			get
			{
				return this.m_IsDown;
			}
			set
			{
				if (this.m_IsDown != value)
				{
					this.m_IsDown = value;
					this.lastFrameDelta |= (value ? ButtonDeltaState.Pressed : ButtonDeltaState.Released);
				}
			}
		}

		internal ButtonDeltaState lastFrameDelta { readonly get; private set; }

		public void Reset()
		{
			this.lastFrameDelta = ButtonDeltaState.NoChange;
			this.m_IsDown = false;
			this.m_ImplementationData.Reset();
		}

		public void OnFrameFinished()
		{
			this.lastFrameDelta = ButtonDeltaState.NoChange;
		}

		public void CopyTo(PointerEventData eventData)
		{
			eventData.dragging = this.m_ImplementationData.isDragging;
			eventData.clickTime = this.m_ImplementationData.pressedTime;
			eventData.pressPosition = this.m_ImplementationData.pressedPosition;
			eventData.pointerPressRaycast = this.m_ImplementationData.pressedRaycast;
			eventData.pointerPress = this.m_ImplementationData.pressedGameObject;
			eventData.rawPointerPress = this.m_ImplementationData.pressedGameObjectRaw;
			eventData.pointerDrag = this.m_ImplementationData.draggedGameObject;
		}

		public void CopyFrom(PointerEventData eventData)
		{
			this.m_ImplementationData.isDragging = eventData.dragging;
			this.m_ImplementationData.pressedTime = eventData.clickTime;
			this.m_ImplementationData.pressedPosition = eventData.pressPosition;
			this.m_ImplementationData.pressedRaycast = eventData.pointerPressRaycast;
			this.m_ImplementationData.pressedGameObject = eventData.pointerPress;
			this.m_ImplementationData.pressedGameObjectRaw = eventData.rawPointerPress;
			this.m_ImplementationData.draggedGameObject = eventData.pointerDrag;
		}

		private bool m_IsDown;

		private MouseButtonModel.ImplementationData m_ImplementationData;

		internal struct ImplementationData
		{
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
			}
		}
	}
}
