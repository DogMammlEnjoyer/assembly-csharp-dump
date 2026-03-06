using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using Unity.XR.CoreUtils.Bindings.Variables;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors
{
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	[AddComponentMenu("XR/Interactors/XR Poke Interactor", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Interactors.XRPokeInteractor.html")]
	public class XRPokeInteractor : XRBaseInteractor, IUIHoverInteractor, IUIInteractor, IPokeStateDataProvider, IAttachPointVelocityProvider
	{
		public float pokeDepth
		{
			get
			{
				return this.m_PokeDepth;
			}
			set
			{
				this.m_PokeDepth = value;
			}
		}

		public float pokeWidth
		{
			get
			{
				return this.m_PokeWidth;
			}
			set
			{
				this.m_PokeWidth = value;
			}
		}

		public float pokeSelectWidth
		{
			get
			{
				return this.m_PokeSelectWidth;
			}
			set
			{
				this.m_PokeSelectWidth = value;
			}
		}

		public float pokeHoverRadius
		{
			get
			{
				return this.m_PokeHoverRadius;
			}
			set
			{
				this.m_PokeHoverRadius = value;
			}
		}

		public float pokeInteractionOffset
		{
			get
			{
				return this.m_PokeInteractionOffset;
			}
			set
			{
				this.m_PokeInteractionOffset = value;
			}
		}

		public LayerMask physicsLayerMask
		{
			get
			{
				return this.m_PhysicsLayerMask;
			}
			set
			{
				this.m_PhysicsLayerMask = value;
			}
		}

		public QueryTriggerInteraction physicsTriggerInteraction
		{
			get
			{
				return this.m_PhysicsTriggerInteraction;
			}
			set
			{
				this.m_PhysicsTriggerInteraction = value;
			}
		}

		public QueryUIDocumentInteraction uiDocumentTriggerInteraction
		{
			get
			{
				return this.m_UIDocumentTriggerInteraction;
			}
			set
			{
				this.m_UIDocumentTriggerInteraction = value;
			}
		}

		public bool requirePokeFilter
		{
			get
			{
				return this.m_RequirePokeFilter;
			}
			set
			{
				this.m_RequirePokeFilter = value;
			}
		}

		public bool enableUIInteraction
		{
			get
			{
				return this.m_EnableUIInteraction;
			}
			set
			{
				if (this.m_EnableUIInteraction != value)
				{
					this.m_EnableUIInteraction = value;
					RegisteredUIInteractorCache registeredUIInteractorCache = this.m_RegisteredUIInteractorCache;
					if (registeredUIInteractorCache == null)
					{
						return;
					}
					registeredUIInteractorCache.RegisterOrUnregisterXRUIInputModule(this.m_EnableUIInteraction);
				}
			}
		}

		public bool clickUIOnDown
		{
			get
			{
				return this.m_ClickUIOnDown;
			}
			set
			{
				this.m_ClickUIOnDown = value;
			}
		}

		public bool debugVisualizationsEnabled
		{
			get
			{
				return this.m_DebugVisualizationsEnabled;
			}
			set
			{
				this.m_DebugVisualizationsEnabled = value;
			}
		}

		public UIHoverEnterEvent uiHoverEntered
		{
			get
			{
				return this.m_UIHoverEntered;
			}
			set
			{
				this.m_UIHoverEntered = value;
			}
		}

		public UIHoverExitEvent uiHoverExited
		{
			get
			{
				return this.m_UIHoverExited;
			}
			set
			{
				this.m_UIHoverExited = value;
			}
		}

		public IReadOnlyBindableVariable<PokeStateData> pokeStateData
		{
			get
			{
				return this.m_PokeStateData;
			}
		}

		protected IAttachPointVelocityTracker attachPointVelocityTracker { get; set; } = new AttachPointVelocityTracker();

		internal virtual void UpdateUIRegistration()
		{
			RegisteredUIInteractorCache registeredUIInteractorCache = this.m_RegisteredUIInteractorCache;
			if (registeredUIInteractorCache != null)
			{
				registeredUIInteractorCache.UnregisterFromXRUIInputModule();
			}
			XRUIToolkitHandler.Unregister(this);
			if (this.m_EnableUIInteraction)
			{
				RegisteredUIInteractorCache registeredUIInteractorCache2 = this.m_RegisteredUIInteractorCache;
				if (registeredUIInteractorCache2 != null)
				{
					registeredUIInteractorCache2.RegisterWithXRUIInputModule();
				}
			}
			if (this.m_EnableUIInteraction)
			{
				XRUIToolkitHandler.Register(this);
				if (this.m_UIToolkitPokeHandler == null)
				{
					this.m_UIToolkitPokeHandler = new XRUIToolkitPokeHandler(this);
				}
				XRUIToolkitPokeHandler uitoolkitPokeHandler = this.m_UIToolkitPokeHandler;
				if (uitoolkitPokeHandler == null)
				{
					return;
				}
				uitoolkitPokeHandler.UpdateVisualizersState();
			}
		}

		private bool canProcessUIToolkit
		{
			get
			{
				return this.m_EnableUIInteraction && XRUIToolkitHandler.uiToolkitSupportEnabled;
			}
		}

		internal bool enableMultiPick
		{
			get
			{
				return this.m_EnableMultiPick;
			}
			set
			{
				this.m_EnableMultiPick = value;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			this.m_LocalPhysicsScene = base.gameObject.scene.GetPhysicsScene();
			this.m_RegisteredUIInteractorCache = new RegisteredUIInteractorCache(this);
			this.m_PositionProvider = new Func<Vector3>(this.GetPokePosition);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.SetDebugObjectVisibility(true);
			this.m_FirstFrame = true;
			if (this.m_EnableUIInteraction)
			{
				this.UpdateUIRegistration();
			}
			AttachPointVelocityTracker attachPointVelocityTracker = this.attachPointVelocityTracker as AttachPointVelocityTracker;
			if (attachPointVelocityTracker != null)
			{
				attachPointVelocityTracker.ResetVelocityTracking();
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			this.SetDebugObjectVisibility(false);
			RegisteredUIInteractorCache registeredUIInteractorCache = this.m_RegisteredUIInteractorCache;
			if (registeredUIInteractorCache != null)
			{
				registeredUIInteractorCache.UnregisterFromXRUIInputModule();
			}
			if (this.canProcessUIToolkit)
			{
				XRUIToolkitPokeHandler uitoolkitPokeHandler = this.m_UIToolkitPokeHandler;
				if (uitoolkitPokeHandler != null)
				{
					uitoolkitPokeHandler.ResetPointerState();
				}
			}
			XRUIToolkitHandler.Unregister(this);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			XRUIToolkitPokeHandler uitoolkitPokeHandler = this.m_UIToolkitPokeHandler;
			if (uitoolkitPokeHandler == null)
			{
				return;
			}
			uitoolkitPokeHandler.Dispose();
		}

		public override void PreprocessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
		{
			base.PreprocessInteractor(updatePhase);
			if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
			{
				Transform xrOriginTransform;
				if (base.TryGetXROrigin(out xrOriginTransform))
				{
					this.attachPointVelocityTracker.UpdateAttachPointVelocityData(this.GetAttachTransform(null), xrOriginTransform);
				}
				else
				{
					this.attachPointVelocityTracker.UpdateAttachPointVelocityData(this.GetAttachTransform(null));
				}
				this.RegisterValidTargets(out this.m_CurrentPokeTarget, out this.m_CurrentPokeFilter);
				this.ProcessPokeStateData();
			}
		}

		public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
		{
			base.ProcessInteractor(updatePhase);
			if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
			{
				this.UpdateDebugVisuals();
			}
		}

		private bool RegisterValidTargets(out IXRSelectInteractable currentTarget, out IXRPokeFilter pokeFilter)
		{
			bool flag = this.EvaluateSphereOverlap() > 0;
			this.m_ValidTargets.Clear();
			XRPokeInteractor.s_ValidTargetsScratchMap.Clear();
			if (flag)
			{
				int count = this.m_PokeTargets.Count;
				for (int i = 0; i < count; i++)
				{
					IXRInteractable interactable = this.m_PokeTargets[i].interactable;
					if (!this.m_ValidTargets.Contains(interactable))
					{
						IXRSelectInteractable ixrselectInteractable = interactable as IXRSelectInteractable;
						if (ixrselectInteractable != null)
						{
							IXRHoverInteractable ixrhoverInteractable = ixrselectInteractable as IXRHoverInteractable;
							if (ixrhoverInteractable != null && ixrhoverInteractable.IsHoverableBy(this))
							{
								this.m_ValidTargets.Add(this.m_PokeTargets[i].interactable);
								XRPokeInteractor.s_ValidTargetsScratchMap.Add(this.m_PokeTargets[i].interactable, this.m_PokeTargets[i].filter);
							}
						}
					}
				}
				if (this.m_ValidTargets.Count > 1)
				{
					SortingHelpers.SortByDistanceToInteractor(this, this.m_ValidTargets, SortingHelpers.squareDistanceAttachPointEvaluator);
				}
				IXRTargetFilter targetFilter = base.targetFilter;
				if (targetFilter != null && targetFilter.canProcess)
				{
					targetFilter.Process(this, this.m_ValidTargets, XRPokeInteractor.s_Results);
					this.m_ValidTargets.Clear();
					this.m_ValidTargets.AddRange(XRPokeInteractor.s_Results);
				}
				if (this.m_ValidTargets.Count == 0)
				{
					flag = false;
				}
			}
			currentTarget = (flag ? ((IXRSelectInteractable)this.m_ValidTargets[0]) : null);
			pokeFilter = (flag ? XRPokeInteractor.s_ValidTargetsScratchMap[currentTarget] : null);
			return flag;
		}

		private void ProcessPokeStateData()
		{
			PokeStateData value;
			if (TrackedDeviceGraphicRaycaster.TryGetPokeStateDataForInteractor(this, out value))
			{
				this.m_PokeStateData.Value = value;
				return;
			}
			IPokeStateDataProvider pokeStateDataProvider = this.m_CurrentPokeFilter as IPokeStateDataProvider;
			if (pokeStateDataProvider != null)
			{
				this.m_PokeStateData.Value = pokeStateDataProvider.pokeStateData.Value;
				return;
			}
			this.m_PokeStateData.Value = default(PokeStateData);
		}

		public override void GetValidTargets(List<IXRInteractable> targets)
		{
			targets.Clear();
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (this.m_ValidTargets.Count > 0)
			{
				targets.Add(this.m_ValidTargets[0]);
			}
		}

		private int EvaluateSphereOverlap()
		{
			this.m_PokeTargets.Clear();
			Vector3 position = this.GetAttachTransform(null).position;
			Vector3 lastPokeInteractionPoint = this.m_LastPokeInteractionPoint;
			Vector3 position2 = position;
			Vector3 direction;
			float num;
			float maxDistance;
			BurstPhysicsUtils.GetSphereOverlapParameters(lastPokeInteractionPoint, position2, out direction, out num, out maxDistance);
			if (this.m_FirstFrame || num < 0.001f)
			{
				int num2 = this.m_LocalPhysicsScene.OverlapSphere(position2, this.m_PokeHoverRadius, this.m_OverlapSphereHits, this.m_PhysicsLayerMask, this.m_PhysicsTriggerInteraction);
				for (int i = 0; i < num2; i++)
				{
					XRPokeInteractor.PokeCollision item;
					if (this.FindPokeTarget(this.m_OverlapSphereHits[i], out item))
					{
						this.m_PokeTargets.Add(item);
					}
				}
			}
			else
			{
				int num3 = this.m_LocalPhysicsScene.SphereCast(lastPokeInteractionPoint, this.m_PokeHoverRadius, direction, this.m_SphereCastHits, maxDistance, this.m_PhysicsLayerMask, this.m_PhysicsTriggerInteraction);
				for (int j = 0; j < num3; j++)
				{
					XRPokeInteractor.PokeCollision item2;
					if (this.FindPokeTarget(this.m_SphereCastHits[j].collider, out item2))
					{
						this.m_PokeTargets.Add(item2);
					}
				}
			}
			this.m_LastPokeInteractionPoint = position;
			this.m_FirstFrame = false;
			return this.m_PokeTargets.Count;
		}

		private bool FindPokeTarget(Collider hitCollider, out XRPokeInteractor.PokeCollision newPokeCollision)
		{
			newPokeCollision = default(XRPokeInteractor.PokeCollision);
			IXRInteractable interactable;
			if (base.interactionManager.TryGetInteractableForCollider(hitCollider, out interactable))
			{
				IXRPokeFilter ixrpokeFilter;
				if (this.TryGetPokeFilter(interactable, out ixrpokeFilter))
				{
					newPokeCollision = new XRPokeInteractor.PokeCollision(interactable, ixrpokeFilter);
					this.ProcessValidInteraction(hitCollider, interactable, ixrpokeFilter);
					return true;
				}
				if (!this.m_RequirePokeFilter)
				{
					newPokeCollision = new XRPokeInteractor.PokeCollision(interactable, null);
					this.ProcessValidInteraction(hitCollider, interactable, null);
					return true;
				}
			}
			return false;
		}

		private bool TryGetPokeFilter(IXRInteractable interactable, out IXRPokeFilter pokeFilter)
		{
			pokeFilter = null;
			XRBaseInteractable xrbaseInteractable = interactable as XRBaseInteractable;
			if (xrbaseInteractable != null)
			{
				xrbaseInteractable.selectFilters.GetAll(this.m_InteractableSelectFilters);
				foreach (IXRSelectFilter ixrselectFilter in this.m_InteractableSelectFilters)
				{
					IXRPokeFilter ixrpokeFilter = ixrselectFilter as IXRPokeFilter;
					if (ixrpokeFilter != null && ixrselectFilter.canProcess)
					{
						pokeFilter = ixrpokeFilter;
						return true;
					}
				}
				return false;
			}
			return false;
		}

		private void ProcessValidInteraction(Collider hitCollider, IXRInteractable interactable, IXRPokeFilter pokeFilter)
		{
			if (this.canProcessUIToolkit)
			{
				XRUIToolkitPokeHandler uitoolkitPokeHandler = this.m_UIToolkitPokeHandler;
				if (uitoolkitPokeHandler == null)
				{
					return;
				}
				uitoolkitPokeHandler.ProcessPokeInteraction(hitCollider, interactable.transform, interactable, this.m_EnableMultiPick, pokeFilter);
			}
		}

		private void SetDebugObjectVisibility(bool isVisible)
		{
			if (this.m_DebugVisualizationsEnabled && this.m_HoverDebugSphere == null)
			{
				this.m_HoverDebugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				this.m_HoverDebugSphere.name = "[Debug] Poke - HoverVisual: " + ((this != null) ? this.ToString() : null);
				this.m_HoverDebugSphere.transform.SetParent(this.GetAttachTransform(null), false);
				this.m_HoverDebugSphere.transform.localScale = new Vector3(this.m_PokeHoverRadius, this.m_PokeHoverRadius, this.m_PokeHoverRadius);
				Collider obj;
				if (this.m_HoverDebugSphere.TryGetComponent<Collider>(out obj))
				{
					Object.Destroy(obj);
				}
				this.m_HoverDebugRenderer = XRPokeInteractor.GetOrAddComponent<MeshRenderer>(this.m_HoverDebugSphere);
			}
			bool flag = this.m_DebugVisualizationsEnabled && isVisible;
			if (this.m_HoverDebugSphere != null && this.m_HoverDebugSphere.activeSelf != flag)
			{
				this.m_HoverDebugSphere.SetActive(flag);
			}
		}

		private void UpdateDebugVisuals()
		{
			this.SetDebugObjectVisibility(this.m_CurrentPokeTarget != null);
			if (!this.m_DebugVisualizationsEnabled)
			{
				return;
			}
			this.m_HoverDebugRenderer.material.color = ((this.m_PokeTargets.Count > 0) ? new Color(0f, 0.8f, 0f, 0.1f) : new Color(0.8f, 0f, 0f, 0.1f));
		}

		private static T GetOrAddComponent<T>(GameObject go) where T : Component
		{
			T result;
			if (!go.TryGetComponent<T>(out result))
			{
				return go.AddComponent<T>();
			}
			return result;
		}

		public virtual void UpdateUIModel(ref TrackedDeviceModel model)
		{
			if (!base.isActiveAndEnabled || this.IsBlockedByInteractionWithinGroup())
			{
				model.Reset(false);
				return;
			}
			Pose worldPose = this.GetAttachTransform(null).GetWorldPose();
			Vector3 position = worldPose.position;
			Vector3 a = worldPose.rotation * Vector3.forward;
			Vector3 item = position + a * this.m_PokeDepth;
			model.position = worldPose.position;
			model.orientation = worldPose.rotation;
			model.positionProvider = this.m_PositionProvider;
			model.raycastLayerMask = this.m_PhysicsLayerMask;
			model.pokeDepth = this.m_PokeDepth;
			model.interactionType = UIInteractionType.Poke;
			model.clickOnDown = this.m_ClickUIOnDown;
			model.UpdatePokeSelectState();
			List<Vector3> raycastPoints = model.raycastPoints;
			raycastPoints.Clear();
			raycastPoints.Add(position);
			raycastPoints.Add(item);
		}

		private Vector3 GetPokePosition()
		{
			return this.GetAttachTransform(null).position;
		}

		public bool TryGetUIModel(out TrackedDeviceModel model)
		{
			if (this.m_RegisteredUIInteractorCache == null)
			{
				model = TrackedDeviceModel.invalid;
				return false;
			}
			return this.m_RegisteredUIInteractorCache.TryGetUIModel(out model);
		}

		void IUIHoverInteractor.OnUIHoverEntered(UIHoverEventArgs args)
		{
			this.OnUIHoverEntered(args);
		}

		void IUIHoverInteractor.OnUIHoverExited(UIHoverEventArgs args)
		{
			this.OnUIHoverExited(args);
		}

		protected virtual void OnUIHoverEntered(UIHoverEventArgs args)
		{
			UIHoverEnterEvent uihoverEntered = this.m_UIHoverEntered;
			if (uihoverEntered == null)
			{
				return;
			}
			uihoverEntered.Invoke(args);
		}

		protected virtual void OnUIHoverExited(UIHoverEventArgs args)
		{
			UIHoverExitEvent uihoverExited = this.m_UIHoverExited;
			if (uihoverExited == null)
			{
				return;
			}
			uihoverExited.Invoke(args);
		}

		protected override void OnHoverExited(HoverExitEventArgs args)
		{
			base.OnHoverExited(args);
			if (args.interactableObject != null && this.canProcessUIToolkit && XRUIToolkitHandler.IsValidUIToolkitInteraction(args.interactableObject.colliders))
			{
				XRUIToolkitPokeHandler uitoolkitPokeHandler = this.m_UIToolkitPokeHandler;
				if (uitoolkitPokeHandler == null)
				{
					return;
				}
				uitoolkitPokeHandler.ResetPointerState();
			}
		}

		protected override void OnHoverEntering(HoverEnterEventArgs args)
		{
			base.OnHoverEntering(args);
		}

		public Vector3 GetAttachPointVelocity()
		{
			Transform xrOriginTransform;
			if (base.TryGetXROrigin(out xrOriginTransform))
			{
				return this.attachPointVelocityTracker.GetAttachPointVelocity(xrOriginTransform);
			}
			return this.attachPointVelocityTracker.GetAttachPointVelocity();
		}

		public Vector3 GetAttachPointAngularVelocity()
		{
			Transform xrOriginTransform;
			if (base.TryGetXROrigin(out xrOriginTransform))
			{
				return this.attachPointVelocityTracker.GetAttachPointAngularVelocity(xrOriginTransform);
			}
			return this.attachPointVelocityTracker.GetAttachPointAngularVelocity();
		}

		private static readonly List<IXRInteractable> s_Results = new List<IXRInteractable>();

		[SerializeField]
		private float m_PokeDepth = 0.1f;

		[SerializeField]
		private float m_PokeWidth = 0.0075f;

		[SerializeField]
		private float m_PokeSelectWidth = 0.015f;

		[SerializeField]
		private float m_PokeHoverRadius = 0.015f;

		[SerializeField]
		private float m_PokeInteractionOffset = 0.005f;

		[SerializeField]
		private LayerMask m_PhysicsLayerMask = -1;

		[SerializeField]
		private QueryTriggerInteraction m_PhysicsTriggerInteraction = QueryTriggerInteraction.Ignore;

		[SerializeField]
		private QueryUIDocumentInteraction m_UIDocumentTriggerInteraction = QueryUIDocumentInteraction.Collide;

		[SerializeField]
		private bool m_RequirePokeFilter = true;

		[SerializeField]
		private bool m_EnableUIInteraction = true;

		[SerializeField]
		private bool m_ClickUIOnDown;

		[SerializeField]
		private bool m_DebugVisualizationsEnabled;

		[SerializeField]
		private UIHoverEnterEvent m_UIHoverEntered = new UIHoverEnterEvent();

		[SerializeField]
		private UIHoverExitEvent m_UIHoverExited = new UIHoverExitEvent();

		private BindableVariable<PokeStateData> m_PokeStateData = new BindableVariable<PokeStateData>(default(PokeStateData), true, null, false);

		private GameObject m_HoverDebugSphere;

		private MeshRenderer m_HoverDebugRenderer;

		private Vector3 m_LastPokeInteractionPoint;

		private bool m_FirstFrame = true;

		private IXRSelectInteractable m_CurrentPokeTarget;

		private IXRPokeFilter m_CurrentPokeFilter;

		private readonly RaycastHit[] m_SphereCastHits = new RaycastHit[25];

		private readonly Collider[] m_OverlapSphereHits = new Collider[25];

		private readonly List<XRPokeInteractor.PokeCollision> m_PokeTargets = new List<XRPokeInteractor.PokeCollision>();

		private readonly List<IXRSelectFilter> m_InteractableSelectFilters = new List<IXRSelectFilter>();

		private readonly List<IXRInteractable> m_ValidTargets = new List<IXRInteractable>();

		private static readonly Dictionary<IXRInteractable, IXRPokeFilter> s_ValidTargetsScratchMap = new Dictionary<IXRInteractable, IXRPokeFilter>();

		private RegisteredUIInteractorCache m_RegisteredUIInteractorCache;

		private PhysicsScene m_LocalPhysicsScene;

		private Func<Vector3> m_PositionProvider;

		private XRUIToolkitPokeHandler m_UIToolkitPokeHandler;

		[HideInInspector]
		[SerializeField]
		[Tooltip("When enabled, multi-point sampling is used for more forgiving UI element detection. Off by default for performance.")]
		private bool m_EnableMultiPick;

		private readonly struct PokeCollision
		{
			public PokeCollision(IXRInteractable interactable, IXRPokeFilter filter)
			{
				this.interactable = interactable;
				this.filter = filter;
			}

			public readonly IXRInteractable interactable;

			public readonly IXRPokeFilter filter;
		}
	}
}
