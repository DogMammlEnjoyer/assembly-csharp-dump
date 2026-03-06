using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.XR.CoreUtils.Bindings.Variables;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Casters;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors
{
	[DisallowMultipleComponent]
	[AddComponentMenu("XR/Interactors/Near-Far Interactor", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Interactors.NearFarInteractor.html")]
	public class NearFarInteractor : XRBaseInputInteractor, IXRRayProvider, IUIHoverInteractor, IUIInteractor, ICurveInteractionDataProvider
	{
		public IInteractionAttachController interactionAttachController
		{
			get
			{
				return this.m_InteractionAttachControllerObjectRef.Get(this.m_InteractionAttachController);
			}
			set
			{
				this.m_InteractionAttachControllerObjectRef.Set(ref this.m_InteractionAttachController, value);
			}
		}

		public bool enableNearCasting
		{
			get
			{
				return this.m_EnableNearCasting;
			}
			set
			{
				this.m_EnableNearCasting = value;
			}
		}

		public IInteractionCaster nearInteractionCaster
		{
			get
			{
				return this.m_NearCasterObjectRef.Get(this.m_NearInteractionCaster);
			}
			set
			{
				this.m_NearCasterObjectRef.Set(ref this.m_NearInteractionCaster, value);
			}
		}

		public NearFarInteractor.NearCasterSortingStrategy nearCasterSortingStrategy
		{
			get
			{
				return this.m_NearCasterSortingStrategy;
			}
			set
			{
				this.m_NearCasterSortingStrategy = value;
			}
		}

		public bool sortNearTargetsAfterTargetFilter
		{
			get
			{
				return this.m_SortNearTargetsAfterTargetFilter;
			}
			set
			{
				this.m_SortNearTargetsAfterTargetFilter = value;
			}
		}

		public bool enableFarCasting
		{
			get
			{
				return this.m_EnableFarCasting;
			}
			set
			{
				this.m_EnableFarCasting = value;
			}
		}

		public ICurveInteractionCaster farInteractionCaster
		{
			get
			{
				return this.m_FarCasterObjectRef.Get(this.m_FarInteractionCaster);
			}
			set
			{
				this.m_FarCasterObjectRef.Set(ref this.m_FarInteractionCaster, value);
			}
		}

		public InteractorFarAttachMode farAttachMode
		{
			get
			{
				return this.m_FarAttachMode;
			}
			set
			{
				this.m_FarAttachMode = value;
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

		public bool blockUIOnInteractableSelection
		{
			get
			{
				return this.m_BlockUIOnInteractableSelection;
			}
			set
			{
				this.m_BlockUIOnInteractableSelection = value;
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

		public XRInputButtonReader uiPressInput
		{
			get
			{
				return this.m_UIPressInput;
			}
			set
			{
				base.SetInputProperty(ref this.m_UIPressInput, value);
			}
		}

		public XRInputValueReader<Vector2> uiScrollInput
		{
			get
			{
				return this.m_UIScrollInput;
			}
			set
			{
				base.SetInputProperty<Vector2>(ref this.m_UIScrollInput, value);
			}
		}

		Transform IXRRayProvider.rayEndTransform
		{
			get
			{
				return this.m_RayEndTransform;
			}
		}

		Vector3 IXRRayProvider.rayEndPoint
		{
			get
			{
				Vector3 result;
				if (this.TryGetCurveEndPoint(out result, false, false) != EndPointType.None)
				{
					return result;
				}
				return this.farInteractionCaster.lastSamplePoint;
			}
		}

		public IReadOnlyBindableVariable<NearFarInteractor.Region> selectionRegion
		{
			get
			{
				return this.m_SelectionRegion;
			}
		}

		private IUIModelUpdater uiModelUpdater
		{
			get
			{
				return this.m_UIModelUpdaterReferenceCache.Get(this.m_FarInteractionCaster);
			}
		}

		private bool isUiSelectInputActive
		{
			get
			{
				return this.m_UIPressInput.ReadIsPerformed();
			}
		}

		private Vector2 uiScrollInputValue
		{
			get
			{
				return this.m_UIScrollInput.ReadValue();
			}
		}

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
			}
		}

		private bool canProcessUIToolkit
		{
			get
			{
				return this.m_EnableUIInteraction && XRUIToolkitHandler.uiToolkitSupportEnabled;
			}
		}

		protected override void Awake()
		{
			this.InitializeReferences();
			base.Awake();
			this.m_RegisteredUIInteractorCache = new RegisteredUIInteractorCache(this);
			base.buttonReaders.Add(this.m_UIPressInput);
			base.valueReaders.Add(this.m_UIScrollInput);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (this.m_EnableUIInteraction)
			{
				this.UpdateUIRegistration();
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			RegisteredUIInteractorCache registeredUIInteractorCache = this.m_RegisteredUIInteractorCache;
			if (registeredUIInteractorCache != null)
			{
				registeredUIInteractorCache.UnregisterFromXRUIInputModule();
			}
			XRUIToolkitHandler.Unregister(this);
			this.InitializeInteractor();
		}

		protected virtual void InitializeReferences()
		{
			if (this.farInteractionCaster == null)
			{
				ICurveInteractionCaster farInteractionCaster;
				if (base.TryGetComponent<ICurveInteractionCaster>(out farInteractionCaster))
				{
					this.farInteractionCaster = farInteractionCaster;
				}
				else
				{
					this.farInteractionCaster = base.gameObject.AddComponent<CurveInteractionCaster>();
				}
			}
			if (this.nearInteractionCaster == null)
			{
				IInteractionCaster[] components = base.GetComponents<IInteractionCaster>();
				IInteractionCaster interactionCaster = null;
				if (components.Length != 0)
				{
					foreach (IInteractionCaster interactionCaster2 in components)
					{
						if (!(interactionCaster2 is ICurveInteractionCaster))
						{
							interactionCaster = interactionCaster2;
							break;
						}
					}
				}
				this.nearInteractionCaster = (interactionCaster ?? base.gameObject.AddComponent<SphereInteractionCaster>());
			}
			if (this.interactionAttachController == null)
			{
				IInteractionAttachController interactionAttachController;
				if (base.TryGetComponent<IInteractionAttachController>(out interactionAttachController))
				{
					this.interactionAttachController = interactionAttachController;
				}
				else
				{
					this.interactionAttachController = base.gameObject.AddComponent<InteractionAttachController>();
				}
			}
			base.attachTransform = this.interactionAttachController.GetOrCreateAnchorTransform(false);
		}

		public override void PreprocessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
		{
			base.PreprocessInteractor(updatePhase);
			if (updatePhase != XRInteractionUpdateOrder.UpdatePhase.Dynamic)
			{
				return;
			}
			this.InitializeInteractor();
			this.UpdateAnchor();
			NearFarInteractor.Region newSelectionRegion = this.DetermineSelectionRegion();
			this.EvaluateNearInteraction();
			this.EvaluateFarInteraction(newSelectionRegion);
			this.UpdateSelectionRegion(newSelectionRegion);
			this.HandleUIToolkitEvents();
		}

		private void InitializeInteractor()
		{
			this.m_InternalValidTargets.Clear();
			this.m_IndexToSnapVolumeMap.Clear();
			this.m_TargetColliders.Clear();
			this.m_FarRayCastHits.Clear();
			this.m_HasValidRayHit = false;
			this.m_ValidHitIsSnapVolume = false;
			this.m_ValidTargetCastSource = NearFarInteractor.Region.None;
		}

		private void UpdateAnchor()
		{
			this.interactionAttachController.DoUpdate(Time.unscaledDeltaTime);
		}

		private NearFarInteractor.Region DetermineSelectionRegion()
		{
			if (!base.hasSelection)
			{
				return NearFarInteractor.Region.None;
			}
			if (!this.interactionAttachController.hasOffset)
			{
				return NearFarInteractor.Region.Near;
			}
			return NearFarInteractor.Region.Far;
		}

		private void UpdateSelectionRegion(NearFarInteractor.Region newSelectionRegion)
		{
			this.m_ReleasedNearInteractionThisFrame = false;
			this.m_SelectionRegion.Value = newSelectionRegion;
		}

		private void EvaluateNearInteraction()
		{
			if (!this.m_EnableNearCasting || base.hasSelection || !this.nearInteractionCaster.TryGetColliderTargets(base.interactionManager, this.m_TargetColliders))
			{
				return;
			}
			int num = this.RegisterNearValidTargets(this.m_TargetColliders, this.m_InternalValidTargets);
			if (num > 0)
			{
				this.m_ValidTargetCastSource = NearFarInteractor.Region.Near;
			}
			if (base.targetFilter != null || this.m_SortNearTargetsAfterTargetFilter)
			{
				return;
			}
			if (num > 1)
			{
				IInteractorDistanceEvaluator evaluatorForSortingStrategy = this.GetEvaluatorForSortingStrategy(this.m_NearCasterSortingStrategy);
				if (evaluatorForSortingStrategy != null)
				{
					SortingHelpers.SortByDistanceToInteractor(this, this.m_InternalValidTargets, evaluatorForSortingStrategy);
				}
			}
		}

		private void EvaluateFarInteraction(NearFarInteractor.Region newSelectionRegion)
		{
			if ((!this.m_EnableFarCasting && newSelectionRegion != NearFarInteractor.Region.Far) || this.m_ValidTargetCastSource == NearFarInteractor.Region.Near)
			{
				return;
			}
			ICurveInteractionCaster farInteractionCaster = this.farInteractionCaster;
			bool flag = farInteractionCaster.TryGetColliderTargets(base.interactionManager, this.m_TargetColliders, this.m_FarRayCastHits);
			bool flag2 = this.canProcessUIToolkit && XRUIToolkitHandler.IsValidUIToolkitInteraction(this.m_TargetColliders);
			if (flag && flag2)
			{
				this.ProcessUIToolkitHit(this.m_FarRayCastHits[0]);
				this.m_HasValidRayHit = true;
				this.m_ValidTargetCastSource = NearFarInteractor.Region.Far;
				return;
			}
			RaycastResult raycastResult;
			bool flag3 = this.TryGetCurrentUIRaycastResult(out raycastResult);
			this.m_HasValidRayHit = (flag3 || flag);
			this.m_ValidHitIsUI = false;
			if (!this.m_HasValidRayHit)
			{
				return;
			}
			this.m_ValidTargetCastSource = NearFarInteractor.Region.Far;
			Vector3 b = farInteractionCaster.samplePoints[0];
			bool flag4 = false;
			if (flag && flag3)
			{
				RaycastHit raycastHit = this.m_FarRayCastHits[0];
				flag4 = (raycastHit.collider != null && raycastResult.gameObject == raycastHit.collider.gameObject);
			}
			float num = flag3 ? (raycastResult.worldPosition - b).sqrMagnitude : float.MaxValue;
			float num2 = flag ? (this.m_FarRayCastHits[0].point - b).sqrMagnitude : float.MaxValue;
			bool flag5 = flag3 && (flag4 || !flag || num < num2);
			if (flag && (flag4 || !flag3 || num2 < num))
			{
				this.Process3dHit(b, flag3, num, ref flag5);
			}
			if (flag5)
			{
				this.Process2dHit(raycastResult);
			}
		}

		private void ProcessUIToolkitHit(RaycastHit raycastHit)
		{
			this.m_RayEndTransform = raycastHit.transform;
			this.m_RayEndPoint = raycastHit.point;
			this.m_RayEndNormal = raycastHit.normal;
			this.m_ValidHitIsUI = true;
		}

		private void HandleUIToolkitEvents()
		{
			if (!this.canProcessUIToolkit)
			{
				return;
			}
			int validHitIsUI = this.m_ValidHitIsUI ? 1 : 0;
			bool lastValidHitIsUI = this.m_LastValidHitIsUI;
			this.m_LastValidHitIsUI = this.m_ValidHitIsUI;
			bool shouldReset = (validHitIsUI == 0 && lastValidHitIsUI) || !this.isCurveActive;
			XRUIToolkitHandler.HandlePointerUpdate(this, this.farInteractionCaster.effectiveCastOrigin.position, this.farInteractionCaster.effectiveCastOrigin.rotation, this.isUiSelectInputActive, shouldReset);
		}

		private void Process3dHit(in Vector3 farCasterOrigin, bool has2dHit, float uiHitSqDistance, ref bool shouldProcess2dHit)
		{
			if (!base.hasSelection)
			{
				int num;
				if (this.RegisterFarValidTargets(this.m_TargetColliders, this.m_InternalValidTargets, out num) > 0)
				{
					IXRInteractable ixrinteractable;
					if (this.m_IndexToSnapVolumeMap.TryGetValue(num, out ixrinteractable))
					{
						this.m_RayEndTransform = ixrinteractable.GetAttachTransform(this);
						this.m_RayEndPoint = this.m_RayEndTransform.position;
						this.m_RayEndNormal = this.m_RayEndTransform.up;
						this.m_ValidHitIsSnapVolume = true;
						this.m_ValidHitSnapVolumeInteractable = ixrinteractable;
					}
					else if (num >= 0 && num < this.m_FarRayCastHits.Count)
					{
						RaycastHit raycastHit = this.m_FarRayCastHits[num];
						this.m_RayEndTransform = raycastHit.transform;
						this.m_RayEndPoint = raycastHit.point;
						this.m_RayEndNormal = raycastHit.normal;
					}
					if (has2dHit && num > 0)
					{
						shouldProcess2dHit = (uiHitSqDistance < (this.m_RayEndPoint - farCasterOrigin).sqrMagnitude);
						return;
					}
				}
				else
				{
					if (has2dHit)
					{
						shouldProcess2dHit = true;
						return;
					}
					this.m_HasValidRayHit = false;
					return;
				}
			}
			else
			{
				RaycastHit raycastHit2 = this.m_FarRayCastHits[0];
				this.m_RayEndTransform = raycastHit2.transform;
				this.m_RayEndPoint = raycastHit2.point;
				this.m_RayEndNormal = raycastHit2.normal;
			}
		}

		private void Process2dHit(in RaycastResult uiHit)
		{
			RaycastResult raycastResult = uiHit;
			this.m_RayEndTransform = raycastResult.gameObject.transform;
			this.m_RayEndPoint = uiHit.worldPosition;
			this.m_RayEndNormal = uiHit.worldNormal;
			this.m_ValidHitIsUI = true;
		}

		protected virtual IInteractorDistanceEvaluator GetEvaluatorForSortingStrategy(NearFarInteractor.NearCasterSortingStrategy strategy)
		{
			switch (strategy)
			{
			case NearFarInteractor.NearCasterSortingStrategy.SquareDistance:
				return SortingHelpers.squareDistanceAttachPointEvaluator;
			case NearFarInteractor.NearCasterSortingStrategy.InteractableBased:
				return SortingHelpers.interactableBasedEvaluator;
			case NearFarInteractor.NearCasterSortingStrategy.ClosestPointOnCollider:
				return SortingHelpers.closestPointOnColliderEvaluator;
			default:
				return null;
			}
		}

		private int RegisterNearValidTargets(List<Collider> targets, List<IXRInteractable> interactables)
		{
			foreach (Collider interactableCollider in targets)
			{
				IXRInteractable ixrinteractable;
				if (base.interactionManager.TryGetInteractableForCollider(interactableCollider, out ixrinteractable) && base.interactionManager.IsHoverPossible(this, ixrinteractable as IXRHoverInteractable))
				{
					interactables.Add(ixrinteractable);
				}
			}
			IXRTargetFilter targetFilter = base.targetFilter;
			if (targetFilter != null && targetFilter.canProcess)
			{
				this.m_PreFilteredTargets.Clear();
				this.m_PreFilteredTargets.AddRange(interactables);
				targetFilter.Process(this, this.m_PreFilteredTargets, interactables);
			}
			return interactables.Count;
		}

		private int RegisterFarValidTargets(List<Collider> targets, List<IXRInteractable> interactables, out int firstRegisteredIndex)
		{
			firstRegisteredIndex = -1;
			int count = targets.Count;
			bool flag = false;
			IXRTargetFilter targetFilter = base.targetFilter;
			bool flag2 = targetFilter != null && targetFilter.canProcess;
			if (flag2)
			{
				this.m_FarTargetToIndexMap.Clear();
			}
			this.m_IndexToSnapVolumeMap.Clear();
			for (int i = 0; i < count; i++)
			{
				IXRInteractable ixrinteractable;
				XRInteractableSnapVolume x;
				bool flag3 = base.interactionManager.TryGetInteractableForCollider(targets[i], out ixrinteractable, out x);
				bool flag4 = x != null;
				bool flag5 = flag3 && base.interactionManager.IsHoverPossible(this, ixrinteractable as IXRHoverInteractable);
				if (flag5)
				{
					if (!flag)
					{
						firstRegisteredIndex = i;
						flag = true;
					}
					interactables.Add(ixrinteractable);
					if (flag4)
					{
						this.m_IndexToSnapVolumeMap.Add(i, ixrinteractable);
					}
					if (flag2)
					{
						this.m_FarTargetToIndexMap.TryAdd(ixrinteractable, i);
					}
				}
				if (!flag2 && (flag5 || !flag4))
				{
					break;
				}
			}
			if (flag2)
			{
				this.m_PreFilteredTargets.Clear();
				this.m_PreFilteredTargets.AddRange(interactables);
				targetFilter.Process(this, this.m_PreFilteredTargets, interactables);
				int num;
				firstRegisteredIndex = ((interactables.Count > 0 && this.m_FarTargetToIndexMap.TryGetValue(interactables[0], out num)) ? num : -1);
			}
			return interactables.Count;
		}

		public override void GetValidTargets(List<IXRInteractable> targets)
		{
			targets.Clear();
			if (this.m_InternalValidTargets.Count == 0)
			{
				return;
			}
			if (this.m_AllowMultipleValidTargets)
			{
				targets.AddRange(this.m_InternalValidTargets);
				return;
			}
			targets.Add(this.m_InternalValidTargets[0]);
		}

		protected override void OnSelectEntering(SelectEnterEventArgs args)
		{
			base.OnSelectEntering(args);
			if (base.interactablesSelected.Count == 1)
			{
				this.m_SelectedTargetCastSource = this.m_ValidTargetCastSource;
				bool flag = false;
				Vector3 zero = Vector3.zero;
				if (this.m_SelectedTargetCastSource == NearFarInteractor.Region.Far && this.TryGetCurveEndPoint(out zero, false, false) != EndPointType.None)
				{
					flag = (this.m_FarAttachMode == InteractorFarAttachMode.Far);
					IFarAttachProvider farAttachProvider = args.interactableObject as IFarAttachProvider;
					if (farAttachProvider != null && farAttachProvider.farAttachMode != InteractableFarAttachMode.DeferToInteractor)
					{
						flag = (farAttachProvider.farAttachMode == InteractableFarAttachMode.Far);
					}
				}
				if (flag)
				{
					this.interactionAttachController.MoveTo(zero);
					return;
				}
				this.interactionAttachController.ResetOffset();
			}
		}

		protected override void OnSelectEntered(SelectEnterEventArgs args)
		{
			base.OnSelectEntered(args);
			if (this.m_SelectedTargetCastSource == NearFarInteractor.Region.Far)
			{
				this.m_NormalRelativeToInteractable = base.firstInteractableSelected.GetAttachTransform(this).InverseTransformDirection(this.m_RayEndNormal);
			}
		}

		protected override void OnSelectExiting(SelectExitEventArgs args)
		{
			base.OnSelectExiting(args);
			if (!base.hasSelection)
			{
				this.m_ValidTargetCastSource = NearFarInteractor.Region.None;
				this.m_SelectedTargetCastSource = NearFarInteractor.Region.None;
				this.m_ReleasedNearInteractionThisFrame = !this.interactionAttachController.hasOffset;
				this.interactionAttachController.ResetOffset();
				this.m_SelectionRegion.SetValueWithoutNotify(NearFarInteractor.Region.None);
			}
		}

		protected override void OnSelectExited(SelectExitEventArgs args)
		{
			base.OnSelectExited(args);
			if (!base.hasSelection)
			{
				this.m_SelectionRegion.BroadcastValue();
			}
		}

		Transform IXRRayProvider.GetOrCreateAttachTransform()
		{
			return this.interactionAttachController.transformToFollow;
		}

		void IXRRayProvider.SetAttachTransform(Transform newAttach)
		{
			this.interactionAttachController.transformToFollow = newAttach;
		}

		Transform IXRRayProvider.GetOrCreateRayOrigin()
		{
			return this.farInteractionCaster.castOrigin;
		}

		void IXRRayProvider.SetRayOrigin(Transform newOrigin)
		{
			this.farInteractionCaster.castOrigin = newOrigin;
		}

		public void UpdateUIModel(ref TrackedDeviceModel model)
		{
			if (!base.isActiveAndEnabled || !this.m_EnableFarCasting || !this.m_EnableUIInteraction || this.uiModelUpdater == null || this.m_ValidTargetCastSource == NearFarInteractor.Region.Near || (this.m_BlockUIOnInteractableSelection && base.hasSelection) || this.IsBlockedByInteractionWithinGroup())
			{
				model.Reset(false);
				return;
			}
			IUIModelUpdater uiModelUpdater = this.uiModelUpdater;
			bool isUiSelectInputActive = this.isUiSelectInputActive;
			Vector2 uiScrollInputValue = this.uiScrollInputValue;
			if (!uiModelUpdater.UpdateUIModel(ref model, isUiSelectInputActive, uiScrollInputValue))
			{
				model.Reset(false);
			}
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

		public bool TryGetCurrentUIRaycastResult(out RaycastResult raycastResult)
		{
			TrackedDeviceModel trackedDeviceModel;
			if (this.m_EnableUIInteraction && this.TryGetUIModel(out trackedDeviceModel) && trackedDeviceModel.currentRaycast.isValid)
			{
				raycastResult = trackedDeviceModel.currentRaycast;
				return trackedDeviceModel.currentRaycastEndpointIndex > 0;
			}
			raycastResult = default(RaycastResult);
			return false;
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

		private bool isCurveActive
		{
			get
			{
				bool flag = this.selectionRegion.Value == NearFarInteractor.Region.Far;
				bool flag2 = this.m_EnableFarCasting || flag;
				if (!base.isActiveAndEnabled || !flag2 || !this.farInteractionCaster.isInitialized || this.m_ReleasedNearInteractionThisFrame)
				{
					return false;
				}
				if (base.hasSelection)
				{
					return flag;
				}
				return this.m_ValidTargetCastSource != NearFarInteractor.Region.Near && !this.IsBlockedByInteractionWithinGroup();
			}
		}

		bool ICurveInteractionDataProvider.isActive
		{
			get
			{
				return this.isCurveActive;
			}
		}

		bool ICurveInteractionDataProvider.hasValidSelect
		{
			get
			{
				if (!this.m_ValidHitIsUI)
				{
					return base.hasSelection;
				}
				return this.isUiSelectInputActive;
			}
		}

		NativeArray<Vector3> ICurveInteractionDataProvider.samplePoints
		{
			get
			{
				return this.farInteractionCaster.samplePoints;
			}
		}

		Vector3 ICurveInteractionDataProvider.lastSamplePoint
		{
			get
			{
				return this.farInteractionCaster.lastSamplePoint;
			}
		}

		public Transform curveOrigin
		{
			get
			{
				return this.farInteractionCaster.effectiveCastOrigin;
			}
		}

		public EndPointType TryGetCurveEndPoint(out Vector3 endPoint, bool snapToSelectedAttachIfAvailable = false, bool snapToSnapVolumeIfAvailable = false)
		{
			bool flag = this.interactionAttachController.hasOffset && base.hasSelection;
			if (snapToSelectedAttachIfAvailable && flag)
			{
				Transform attachTransform = base.firstInteractableSelected.GetAttachTransform(this);
				endPoint = attachTransform.position;
				return EndPointType.AttachPoint;
			}
			if (snapToSnapVolumeIfAvailable && this.m_ValidHitIsSnapVolume)
			{
				endPoint = ((this.m_ValidHitSnapVolumeInteractable != null) ? this.m_ValidHitSnapVolumeInteractable.GetAttachTransform(this).position : this.m_RayEndPoint);
				return EndPointType.AttachPoint;
			}
			endPoint = this.m_RayEndPoint;
			if (!this.m_HasValidRayHit)
			{
				return EndPointType.None;
			}
			if (this.m_ValidHitIsUI)
			{
				return EndPointType.UI;
			}
			if (this.m_InternalValidTargets.Count <= 0 && !flag)
			{
				return EndPointType.EmptyCastHit;
			}
			return EndPointType.ValidCastHit;
		}

		public EndPointType TryGetCurveEndNormal(out Vector3 endNormal, bool snapToSelectedAttachIfAvailable = false)
		{
			bool flag = this.interactionAttachController.hasOffset && base.hasSelection;
			if (snapToSelectedAttachIfAvailable && flag)
			{
				Transform attachTransform = base.firstInteractableSelected.GetAttachTransform(this);
				endNormal = attachTransform.TransformDirection(this.m_NormalRelativeToInteractable);
				return EndPointType.AttachPoint;
			}
			endNormal = this.m_RayEndNormal;
			if (!this.m_HasValidRayHit)
			{
				return EndPointType.None;
			}
			if (this.m_ValidHitIsUI)
			{
				return EndPointType.UI;
			}
			if (this.m_InternalValidTargets.Count <= 0 && !flag)
			{
				return EndPointType.EmptyCastHit;
			}
			return EndPointType.ValidCastHit;
		}

		[SerializeField]
		[RequireInterface(typeof(IInteractionAttachController))]
		private Object m_InteractionAttachController;

		private readonly UnityObjectReferenceCache<IInteractionAttachController, Object> m_InteractionAttachControllerObjectRef = new UnityObjectReferenceCache<IInteractionAttachController, Object>();

		[SerializeField]
		private bool m_EnableNearCasting = true;

		[SerializeField]
		[RequireInterface(typeof(IInteractionCaster))]
		private Object m_NearInteractionCaster;

		private readonly UnityObjectReferenceCache<IInteractionCaster, Object> m_NearCasterObjectRef = new UnityObjectReferenceCache<IInteractionCaster, Object>();

		[SerializeField]
		private NearFarInteractor.NearCasterSortingStrategy m_NearCasterSortingStrategy = NearFarInteractor.NearCasterSortingStrategy.SquareDistance;

		[SerializeField]
		private bool m_SortNearTargetsAfterTargetFilter;

		[Space]
		[SerializeField]
		private bool m_EnableFarCasting = true;

		[SerializeField]
		[RequireInterface(typeof(ICurveInteractionCaster))]
		private Object m_FarInteractionCaster;

		private readonly UnityObjectReferenceCache<ICurveInteractionCaster, Object> m_FarCasterObjectRef = new UnityObjectReferenceCache<ICurveInteractionCaster, Object>();

		[SerializeField]
		private InteractorFarAttachMode m_FarAttachMode = InteractorFarAttachMode.Far;

		[SerializeField]
		private bool m_EnableUIInteraction = true;

		[SerializeField]
		private bool m_BlockUIOnInteractableSelection = true;

		[SerializeField]
		private UIHoverEnterEvent m_UIHoverEntered = new UIHoverEnterEvent();

		[SerializeField]
		private UIHoverExitEvent m_UIHoverExited = new UIHoverExitEvent();

		[SerializeField]
		private XRInputButtonReader m_UIPressInput = new XRInputButtonReader("UI Press", null, false, XRInputButtonReader.InputSourceMode.InputActionReference);

		[SerializeField]
		private XRInputValueReader<Vector2> m_UIScrollInput = new XRInputValueReader<Vector2>("UI Scroll", XRInputValueReader.InputSourceMode.InputActionReference);

		private readonly BindableEnum<NearFarInteractor.Region> m_SelectionRegion = new BindableEnum<NearFarInteractor.Region>(NearFarInteractor.Region.None, true, null, false);

		private NearFarInteractor.Region m_ValidTargetCastSource;

		private NearFarInteractor.Region m_SelectedTargetCastSource;

		private readonly List<Collider> m_TargetColliders = new List<Collider>();

		private readonly List<RaycastHit> m_FarRayCastHits = new List<RaycastHit>();

		private readonly List<IXRInteractable> m_InternalValidTargets = new List<IXRInteractable>();

		private readonly Dictionary<int, IXRInteractable> m_IndexToSnapVolumeMap = new Dictionary<int, IXRInteractable>();

		private readonly Dictionary<IXRInteractable, int> m_FarTargetToIndexMap = new Dictionary<IXRInteractable, int>();

		private readonly List<IXRInteractable> m_PreFilteredTargets = new List<IXRInteractable>();

		private bool m_ReleasedNearInteractionThisFrame;

		private RegisteredUIInteractorCache m_RegisteredUIInteractorCache;

		private readonly UnityObjectReferenceCache<IUIModelUpdater, Object> m_UIModelUpdaterReferenceCache = new UnityObjectReferenceCache<IUIModelUpdater, Object>();

		private bool m_HasValidRayHit;

		private bool m_LastValidHitIsUI;

		private Transform m_RayEndTransform;

		private Vector3 m_RayEndPoint;

		private Vector3 m_RayEndNormal;

		private Vector3 m_NormalRelativeToInteractable;

		private bool m_ValidHitIsUI;

		private bool m_ValidHitIsSnapVolume;

		private IXRInteractable m_ValidHitSnapVolumeInteractable;

		private readonly bool m_AllowMultipleValidTargets;

		public enum Region
		{
			None,
			Near,
			Far
		}

		public enum NearCasterSortingStrategy
		{
			None,
			SquareDistance,
			InteractableBased,
			ClosestPointOnCollider
		}
	}
}
