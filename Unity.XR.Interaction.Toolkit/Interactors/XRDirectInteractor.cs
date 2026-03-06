using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors
{
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	[DisallowMultipleComponent]
	[AddComponentMenu("XR/Interactors/XR Direct Interactor", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor.html")]
	public class XRDirectInteractor : XRBaseInputInteractor
	{
		public bool improveAccuracyWithSphereCollider
		{
			get
			{
				return this.m_ImproveAccuracyWithSphereCollider;
			}
			set
			{
				this.m_ImproveAccuracyWithSphereCollider = value;
			}
		}

		public bool usingSphereColliderAccuracyImprovement
		{
			get
			{
				return this.m_UsingSphereColliderAccuracyImprovement;
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

		protected List<IXRInteractable> unsortedValidTargets { get; } = new List<IXRInteractable>();

		protected override void Awake()
		{
			base.Awake();
			this.m_LocalPhysicsScene = base.gameObject.scene.GetPhysicsScene();
			this.m_TriggerContactMonitor.interactionManager = base.interactionManager;
			this.m_UpdateCollidersAfterTriggerStay = this.UpdateCollidersAfterOnTriggerStay();
			this.ValidateColliderConfiguration();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.m_TriggerContactMonitor.contactAdded += this.OnContactAdded;
			this.m_TriggerContactMonitor.contactRemoved += this.OnContactRemoved;
			this.ResetCollidersAndValidTargets();
			if (!this.m_UsingSphereColliderAccuracyImprovement)
			{
				base.StartCoroutine(this.m_UpdateCollidersAfterTriggerStay);
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			this.m_TriggerContactMonitor.contactAdded -= this.OnContactAdded;
			this.m_TriggerContactMonitor.contactRemoved -= this.OnContactRemoved;
			this.ResetCollidersAndValidTargets();
			if (!this.m_UsingSphereColliderAccuracyImprovement)
			{
				base.StopCoroutine(this.m_UpdateCollidersAfterTriggerStay);
			}
		}

		protected void OnTriggerEnter(Collider other)
		{
			if (this.m_UsingSphereColliderAccuracyImprovement)
			{
				return;
			}
			this.m_TriggerContactMonitor.AddCollider(other);
		}

		protected void OnTriggerStay(Collider other)
		{
			if (this.m_UsingSphereColliderAccuracyImprovement)
			{
				return;
			}
			this.m_StayedColliders.Add(other);
		}

		protected void OnTriggerExit(Collider other)
		{
			if (this.m_UsingSphereColliderAccuracyImprovement)
			{
				return;
			}
			this.m_TriggerContactMonitor.RemoveCollider(other);
		}

		private IEnumerator UpdateCollidersAfterOnTriggerStay()
		{
			for (;;)
			{
				yield return XRDirectInteractor.s_WaitForFixedUpdate;
				this.m_TriggerContactMonitor.UpdateStayedColliders(this.m_StayedColliders);
			}
			yield break;
		}

		public override void PreprocessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
		{
			base.PreprocessInteractor(updatePhase);
			if (this.m_UsingSphereColliderAccuracyImprovement && updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
			{
				this.EvaluateSphereOverlap();
			}
		}

		public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
		{
			base.ProcessInteractor(updatePhase);
			if (!this.m_UsingSphereColliderAccuracyImprovement && updatePhase == XRInteractionUpdateOrder.UpdatePhase.Fixed)
			{
				this.m_StayedColliders.Clear();
			}
		}

		private void EvaluateSphereOverlap()
		{
			this.m_ContactsSortedThisFrame = false;
			this.m_StayedColliders.Clear();
			Vector3 vector = this.GetAttachTransform(null).TransformPoint(this.m_SphereCollider.center);
			Vector3 lastSphereCastOrigin = this.m_LastSphereCastOrigin;
			Vector3 position = vector;
			float radius = this.m_SphereCollider.radius * this.m_SphereCollider.transform.lossyScale.x;
			Vector3 direction;
			float num;
			float maxDistance;
			BurstPhysicsUtils.GetSphereOverlapParameters(lastSphereCastOrigin, position, out direction, out num, out maxDistance);
			if (this.m_FirstFrame || num < 0.001f)
			{
				int num2 = this.m_LocalPhysicsScene.OverlapSphere(position, radius, this.m_OverlapSphereHits, this.m_PhysicsLayerMask, this.m_PhysicsTriggerInteraction);
				for (int i = 0; i < num2; i++)
				{
					this.m_StayedColliders.Add(this.m_OverlapSphereHits[i]);
				}
			}
			else
			{
				int num3 = this.m_LocalPhysicsScene.SphereCast(lastSphereCastOrigin, radius, direction, this.m_SphereCastHits, maxDistance, this.m_PhysicsLayerMask, this.m_PhysicsTriggerInteraction);
				for (int j = 0; j < num3; j++)
				{
					this.m_StayedColliders.Add(this.m_SphereCastHits[j].collider);
				}
			}
			this.m_TriggerContactMonitor.UpdateStayedColliders(this.m_StayedColliders);
			this.m_LastSphereCastOrigin = vector;
			this.m_FirstFrame = false;
		}

		private void ValidateColliderConfiguration()
		{
			Rigidbody rigidbody;
			if (!base.TryGetComponent<Rigidbody>(out rigidbody))
			{
				Collider[] components = base.GetComponents<Collider>();
				if (this.m_ImproveAccuracyWithSphereCollider && components.Length == 1)
				{
					SphereCollider sphereCollider = components[0] as SphereCollider;
					if (sphereCollider != null)
					{
						this.m_SphereCollider = sphereCollider;
						this.m_SphereCollider.enabled = false;
						this.m_UsingSphereColliderAccuracyImprovement = true;
						return;
					}
				}
				bool flag = false;
				Collider[] array = components;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].isTrigger)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					Debug.LogWarning("Direct Interactor does not have required Collider set as a trigger.", this);
				}
			}
		}

		public override void GetValidTargets(List<IXRInteractable> targets)
		{
			targets.Clear();
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			IXRTargetFilter targetFilter = base.targetFilter;
			if (targetFilter != null && targetFilter.canProcess)
			{
				targetFilter.Process(this, this.unsortedValidTargets, targets);
				return;
			}
			if (this.m_ContactsSortedThisFrame)
			{
				targets.AddRange(this.m_SortedValidTargets);
				return;
			}
			SortingHelpers.SortByDistanceToInteractor(this, this.unsortedValidTargets, this.m_SortedValidTargets);
			targets.AddRange(this.m_SortedValidTargets);
			this.m_ContactsSortedThisFrame = true;
		}

		public override bool CanHover(IXRHoverInteractable interactable)
		{
			return base.CanHover(interactable) && (!base.hasSelection || base.IsSelecting(interactable));
		}

		public override bool CanSelect(IXRSelectInteractable interactable)
		{
			return base.CanSelect(interactable) && (!base.hasSelection || base.IsSelecting(interactable));
		}

		protected override void OnRegistered(InteractorRegisteredEventArgs args)
		{
			base.OnRegistered(args);
			args.manager.interactableRegistered += this.OnInteractableRegistered;
			args.manager.interactableUnregistered += this.OnInteractableUnregistered;
			this.m_TriggerContactMonitor.interactionManager = args.manager;
			if (!this.m_UsingSphereColliderAccuracyImprovement)
			{
				this.m_TriggerContactMonitor.ResolveUnassociatedColliders();
				XRInteractionManager.RemoveAllUnregistered(args.manager, this.unsortedValidTargets);
			}
		}

		protected override void OnUnregistered(InteractorUnregisteredEventArgs args)
		{
			base.OnUnregistered(args);
			args.manager.interactableRegistered -= this.OnInteractableRegistered;
			args.manager.interactableUnregistered -= this.OnInteractableUnregistered;
		}

		private void OnInteractableRegistered(InteractableRegisteredEventArgs args)
		{
			IXRInteractable interactableObject = args.interactableObject;
			this.m_TriggerContactMonitor.ResolveUnassociatedColliders(interactableObject);
			if (this.m_TriggerContactMonitor.IsContacting(interactableObject))
			{
				this.OnContactAdded(interactableObject);
			}
		}

		private void OnInteractableUnregistered(InteractableUnregisteredEventArgs args)
		{
			this.OnContactRemoved(args.interactableObject);
		}

		private void OnContactAdded(IXRInteractable interactable)
		{
			if (this.unsortedValidTargets.Contains(interactable))
			{
				return;
			}
			this.unsortedValidTargets.Add(interactable);
			this.m_ContactsSortedThisFrame = false;
		}

		private void OnContactRemoved(IXRInteractable interactable)
		{
			if (this.unsortedValidTargets.Remove(interactable))
			{
				this.m_ContactsSortedThisFrame = false;
			}
		}

		private void ResetCollidersAndValidTargets()
		{
			this.unsortedValidTargets.Clear();
			this.m_SortedValidTargets.Clear();
			this.m_ContactsSortedThisFrame = false;
			this.m_FirstFrame = true;
			this.m_StayedColliders.Clear();
			this.m_TriggerContactMonitor.UpdateStayedColliders(this.m_StayedColliders);
		}

		[SerializeField]
		private bool m_ImproveAccuracyWithSphereCollider;

		[SerializeField]
		private LayerMask m_PhysicsLayerMask = 1;

		[SerializeField]
		private QueryTriggerInteraction m_PhysicsTriggerInteraction = QueryTriggerInteraction.Ignore;

		private readonly HashSet<Collider> m_StayedColliders = new HashSet<Collider>();

		private readonly TriggerContactMonitor m_TriggerContactMonitor = new TriggerContactMonitor();

		private static readonly WaitForFixedUpdate s_WaitForFixedUpdate = new WaitForFixedUpdate();

		private IEnumerator m_UpdateCollidersAfterTriggerStay;

		private bool m_UsingSphereColliderAccuracyImprovement;

		private SphereCollider m_SphereCollider;

		private PhysicsScene m_LocalPhysicsScene;

		private Vector3 m_LastSphereCastOrigin = Vector3.zero;

		private readonly Collider[] m_OverlapSphereHits = new Collider[25];

		private readonly RaycastHit[] m_SphereCastHits = new RaycastHit[25];

		private bool m_FirstFrame = true;

		private bool m_ContactsSortedThisFrame;

		private readonly List<IXRInteractable> m_SortedValidTargets = new List<IXRInteractable>();
	}
}
