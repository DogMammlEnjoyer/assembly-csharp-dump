using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
	public struct TrackedDeviceModel
	{
		internal TrackedDeviceModel.ImplementationData implementationData
		{
			get
			{
				return this.m_ImplementationData;
			}
		}

		public readonly int pointerId { get; }

		public bool select
		{
			get
			{
				return this.m_SelectDown;
			}
			set
			{
				if (this.m_SelectDown != value)
				{
					this.m_SelectDown = value;
					this.selectDelta |= (value ? ButtonDeltaState.Pressed : ButtonDeltaState.Released);
					this.changedThisFrame = true;
				}
			}
		}

		public bool clickOnDown
		{
			get
			{
				return this.m_ClickOnDown;
			}
			set
			{
				this.m_ClickOnDown = value;
			}
		}

		public ButtonDeltaState selectDelta { readonly get; private set; }

		public bool changedThisFrame { readonly get; private set; }

		public Vector3 position
		{
			get
			{
				Func<Vector3> positionProvider = this.m_PositionProvider;
				if (positionProvider == null)
				{
					return this.m_Position;
				}
				return positionProvider();
			}
			set
			{
				if (this.m_Position != value)
				{
					this.m_Position = value;
					this.changedThisFrame = true;
				}
			}
		}

		public Func<Vector3> positionProvider
		{
			get
			{
				return this.m_PositionProvider;
			}
			set
			{
				if (this.m_PositionProvider != value)
				{
					this.m_PositionProvider = value;
					this.changedThisFrame = true;
				}
			}
		}

		public Quaternion orientation
		{
			get
			{
				return this.m_Orientation;
			}
			set
			{
				if (this.m_Orientation != value)
				{
					this.m_Orientation = value;
					this.changedThisFrame = true;
				}
			}
		}

		public List<Vector3> raycastPoints
		{
			get
			{
				return this.m_RaycastPoints;
			}
			set
			{
				this.changedThisFrame |= (this.m_RaycastPoints.Count != value.Count);
				this.m_RaycastPoints = value;
			}
		}

		public RaycastResult currentRaycast { readonly get; private set; }

		public int currentRaycastEndpointIndex { readonly get; private set; }

		public LayerMask raycastLayerMask
		{
			get
			{
				return this.m_RaycastLayerMask;
			}
			set
			{
				if (this.m_RaycastLayerMask != value)
				{
					this.changedThisFrame = true;
					this.m_RaycastLayerMask = value;
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
					this.m_ScrollDelta = value;
					this.changedThisFrame = true;
				}
			}
		}

		public float pokeDepth
		{
			get
			{
				return this.m_PokeDepth;
			}
			set
			{
				if (this.m_PokeDepth != value)
				{
					this.m_PokeDepth = value;
					this.changedThisFrame = true;
				}
			}
		}

		public UIInteractionType interactionType
		{
			get
			{
				return this.m_InteractionType;
			}
			set
			{
				if (this.m_InteractionType != value)
				{
					this.m_InteractionType = value;
					this.changedThisFrame = true;
				}
			}
		}

		internal IUIInteractor interactor { readonly get; set; }

		internal void UpdatePokeSelectState()
		{
			if (this.m_InteractionType == UIInteractionType.Poke)
			{
				this.select = TrackedDeviceGraphicRaycaster.IsPokeSelectingWithUI(this.interactor);
			}
		}

		public GameObject selectableObject { readonly get; set; }

		public bool isScrollable { readonly get; set; }

		public static TrackedDeviceModel invalid { get; } = new TrackedDeviceModel(-1);

		public TrackedDeviceModel(int pointerId)
		{
			this = default(TrackedDeviceModel);
			this.pointerId = pointerId;
			this.m_RaycastPoints = new List<Vector3>();
			this.m_ImplementationData = default(TrackedDeviceModel.ImplementationData);
			this.Reset(true);
		}

		public void Reset(bool resetImplementation = true)
		{
			this.m_Orientation = Quaternion.identity;
			this.m_Position = Vector3.zero;
			this.m_PositionProvider = null;
			this.changedThisFrame = false;
			this.m_SelectDown = false;
			this.selectDelta = ButtonDeltaState.NoChange;
			List<Vector3> raycastPoints = this.m_RaycastPoints;
			if (raycastPoints != null)
			{
				raycastPoints.Clear();
			}
			this.currentRaycastEndpointIndex = 0;
			this.m_RaycastLayerMask = -5;
			this.m_ScrollDelta = Vector2.zero;
			if (resetImplementation)
			{
				this.m_ImplementationData.Reset();
			}
		}

		public void OnFrameFinished()
		{
			this.selectDelta = ButtonDeltaState.NoChange;
			this.m_ScrollDelta = Vector2.zero;
			this.changedThisFrame = false;
		}

		public void CopyTo(TrackedDeviceEventData eventData)
		{
			eventData.rayPoints = this.m_RaycastPoints;
			eventData.layerMask = this.m_RaycastLayerMask;
			eventData.pointerId = this.pointerId;
			eventData.scrollDelta = this.m_ScrollDelta;
			eventData.pointerEnter = this.m_ImplementationData.pointerTarget;
			eventData.dragging = this.m_ImplementationData.isDragging;
			eventData.clickTime = this.m_ImplementationData.pressedTime;
			eventData.position = this.m_ImplementationData.position;
			eventData.pressPosition = this.m_ImplementationData.pressedPosition;
			eventData.pressWorldPosition = this.m_ImplementationData.pressedWorldPosition;
			eventData.pointerPressRaycast = this.m_ImplementationData.pressedRaycast;
			eventData.pointerPress = this.m_ImplementationData.pressedGameObject;
			eventData.rawPointerPress = this.m_ImplementationData.pressedGameObjectRaw;
			eventData.pointerDrag = this.m_ImplementationData.draggedGameObject;
			eventData.hovered.Clear();
			eventData.hovered.AddRange(this.m_ImplementationData.hoverTargets);
		}

		public void CopyFrom(TrackedDeviceEventData eventData)
		{
			this.m_ImplementationData.pointerTarget = eventData.pointerEnter;
			this.m_ImplementationData.isDragging = eventData.dragging;
			this.m_ImplementationData.pressedTime = eventData.clickTime;
			this.m_ImplementationData.position = eventData.position;
			this.m_ImplementationData.pressedPosition = eventData.pressPosition;
			this.m_ImplementationData.pressedWorldPosition = eventData.pressWorldPosition;
			this.m_ImplementationData.pressedRaycast = eventData.pointerPressRaycast;
			this.m_ImplementationData.pressedGameObject = eventData.pointerPress;
			this.m_ImplementationData.pressedGameObjectRaw = eventData.rawPointerPress;
			this.m_ImplementationData.draggedGameObject = eventData.pointerDrag;
			this.m_ImplementationData.hoverTargets.Clear();
			this.m_ImplementationData.hoverTargets.AddRange(eventData.hovered);
			this.currentRaycast = eventData.pointerCurrentRaycast;
			this.currentRaycastEndpointIndex = eventData.rayHitIndex;
		}

		[Obsolete("maxRaycastDistance has been deprecated. Its value was unused, calling this property is unnecessary and should be removed.", true)]
		public float maxRaycastDistance
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		private TrackedDeviceModel.ImplementationData m_ImplementationData;

		private bool m_SelectDown;

		private bool m_ClickOnDown;

		private Vector3 m_Position;

		private Func<Vector3> m_PositionProvider;

		private Quaternion m_Orientation;

		private List<Vector3> m_RaycastPoints;

		private LayerMask m_RaycastLayerMask;

		private Vector2 m_ScrollDelta;

		private float m_PokeDepth;

		private UIInteractionType m_InteractionType;

		internal struct ImplementationData
		{
			public List<GameObject> hoverTargets { readonly get; set; }

			public GameObject pointerTarget { readonly get; set; }

			public bool isDragging { readonly get; set; }

			public float pressedTime { readonly get; set; }

			public Vector2 position { readonly get; set; }

			public Vector2 pressedPosition { readonly get; set; }

			public Vector3 pressedWorldPosition { readonly get; set; }

			public RaycastResult pressedRaycast { readonly get; set; }

			public GameObject pressedGameObject { readonly get; set; }

			public GameObject pressedGameObjectRaw { readonly get; set; }

			public GameObject draggedGameObject { readonly get; set; }

			public void Reset()
			{
				this.isDragging = false;
				this.pressedTime = 0f;
				this.position = Vector2.zero;
				this.pressedPosition = Vector2.zero;
				this.pressedWorldPosition = Vector3.zero;
				this.pressedRaycast = default(RaycastResult);
				this.pressedGameObject = null;
				this.pressedGameObjectRaw = null;
				this.draggedGameObject = null;
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
