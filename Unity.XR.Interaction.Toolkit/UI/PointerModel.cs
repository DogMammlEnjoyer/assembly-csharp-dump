using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
	internal struct PointerModel
	{
		public readonly int pointerId { get; }

		public bool changedThisFrame { readonly get; private set; }

		public int displayIndex
		{
			get
			{
				return this.m_DisplayIndex;
			}
			set
			{
				if (this.m_DisplayIndex != value)
				{
					this.m_DisplayIndex = value;
					this.changedThisFrame = true;
				}
			}
		}

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
					this.m_ScrollDelta = value;
					this.changedThisFrame = true;
				}
			}
		}

		public MouseButtonModel leftButton
		{
			get
			{
				return this.m_LeftButton;
			}
			set
			{
				this.changedThisFrame |= (value.lastFrameDelta > ButtonDeltaState.NoChange);
				this.m_LeftButton = value;
			}
		}

		public bool leftButtonPressed
		{
			set
			{
				this.changedThisFrame |= (this.m_LeftButton.isDown != value);
				this.m_LeftButton.isDown = value;
			}
		}

		public MouseButtonModel rightButton
		{
			get
			{
				return this.m_RightButton;
			}
			set
			{
				this.changedThisFrame |= (value.lastFrameDelta > ButtonDeltaState.NoChange);
				this.m_RightButton = value;
			}
		}

		public bool rightButtonPressed
		{
			set
			{
				this.changedThisFrame |= (this.m_RightButton.isDown != value);
				this.m_RightButton.isDown = value;
			}
		}

		public MouseButtonModel middleButton
		{
			get
			{
				return this.m_MiddleButton;
			}
			set
			{
				this.changedThisFrame |= (value.lastFrameDelta > ButtonDeltaState.NoChange);
				this.m_MiddleButton = value;
			}
		}

		public bool middleButtonPressed
		{
			set
			{
				this.changedThisFrame |= (this.m_MiddleButton.isDown != value);
				this.m_MiddleButton.isDown = value;
			}
		}

		public PointerModel(int pointerId)
		{
			this.pointerId = pointerId;
			this.changedThisFrame = false;
			this.m_DisplayIndex = 0;
			this.m_Position = Vector2.zero;
			this.deltaPosition = Vector2.zero;
			this.m_ScrollDelta = Vector2.zero;
			this.m_LeftButton = default(MouseButtonModel);
			this.m_RightButton = default(MouseButtonModel);
			this.m_MiddleButton = default(MouseButtonModel);
			this.m_LeftButton.Reset();
			this.m_RightButton.Reset();
			this.m_MiddleButton.Reset();
			this.m_InternalData = default(PointerModel.InternalData);
			this.m_InternalData.Reset();
		}

		public void OnFrameFinished()
		{
			this.changedThisFrame = false;
			this.deltaPosition = Vector2.zero;
			this.m_ScrollDelta = Vector2.zero;
			this.m_LeftButton.OnFrameFinished();
			this.m_RightButton.OnFrameFinished();
			this.m_MiddleButton.OnFrameFinished();
		}

		public void CopyTo(PointerEventData eventData)
		{
			eventData.pointerId = this.pointerId;
			eventData.displayIndex = this.m_DisplayIndex;
			eventData.position = this.position;
			eventData.delta = this.deltaPosition;
			eventData.scrollDelta = this.scrollDelta;
			eventData.pointerEnter = this.m_InternalData.pointerTarget;
			eventData.hovered.Clear();
			eventData.hovered.AddRange(this.m_InternalData.hoverTargets);
		}

		public void CopyFrom(PointerEventData eventData)
		{
			List<GameObject> hoverTargets = this.m_InternalData.hoverTargets;
			this.m_InternalData.hoverTargets.Clear();
			this.m_InternalData.hoverTargets.AddRange(eventData.hovered);
			this.m_InternalData.hoverTargets = hoverTargets;
			this.m_InternalData.pointerTarget = eventData.pointerEnter;
			this.m_DisplayIndex = eventData.displayIndex;
		}

		private int m_DisplayIndex;

		private Vector2 m_Position;

		private Vector2 m_ScrollDelta;

		private MouseButtonModel m_LeftButton;

		private MouseButtonModel m_RightButton;

		private MouseButtonModel m_MiddleButton;

		private PointerModel.InternalData m_InternalData;

		internal struct InternalData
		{
			public List<GameObject> hoverTargets { readonly get; set; }

			public GameObject pointerTarget { readonly get; set; }

			public void Reset()
			{
				this.pointerTarget = null;
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
