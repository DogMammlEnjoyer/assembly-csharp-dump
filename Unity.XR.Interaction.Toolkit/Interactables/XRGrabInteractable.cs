using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Profiling;
using Unity.XR.CoreUtils;
using UnityEngine.Pool;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
using UnityEngine.XR.Interaction.Toolkit.Gaze;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Transformers;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Pooling;

namespace UnityEngine.XR.Interaction.Toolkit.Interactables
{
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	[SelectionBase]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody))]
	[AddComponentMenu("XR/XR Grab Interactable", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable.html")]
	[BurstCompile]
	public class XRGrabInteractable : XRBaseInteractable, IFarAttachProvider
	{
		public Transform attachTransform
		{
			get
			{
				return this.m_AttachTransform;
			}
			set
			{
				this.m_AttachTransform = value;
			}
		}

		public Transform secondaryAttachTransform
		{
			get
			{
				return this.m_SecondaryAttachTransform;
			}
			set
			{
				this.m_SecondaryAttachTransform = value;
			}
		}

		public bool useDynamicAttach
		{
			get
			{
				return this.m_UseDynamicAttach;
			}
			set
			{
				this.m_UseDynamicAttach = value;
			}
		}

		public bool matchAttachPosition
		{
			get
			{
				return this.m_MatchAttachPosition;
			}
			set
			{
				this.m_MatchAttachPosition = value;
			}
		}

		public bool matchAttachRotation
		{
			get
			{
				return this.m_MatchAttachRotation;
			}
			set
			{
				this.m_MatchAttachRotation = value;
			}
		}

		public bool snapToColliderVolume
		{
			get
			{
				return this.m_SnapToColliderVolume;
			}
			set
			{
				this.m_SnapToColliderVolume = value;
			}
		}

		public bool reinitializeDynamicAttachEverySingleGrab
		{
			get
			{
				return this.m_ReinitializeDynamicAttachEverySingleGrab;
			}
			set
			{
				this.m_ReinitializeDynamicAttachEverySingleGrab = value;
			}
		}

		public float attachEaseInTime
		{
			get
			{
				return this.m_AttachEaseInTime;
			}
			set
			{
				this.m_AttachEaseInTime = value;
			}
		}

		public XRBaseInteractable.MovementType movementType
		{
			get
			{
				return this.m_MovementType;
			}
			set
			{
				this.m_MovementType = value;
				this.UpdateCurrentMovementType();
			}
		}

		public Transform predictedVisualsTransform
		{
			get
			{
				return this.m_PredictedVisualsTransform;
			}
			set
			{
				this.m_PredictedVisualsTransform = value;
			}
		}

		public float velocityDamping
		{
			get
			{
				return this.m_VelocityDamping;
			}
			set
			{
				this.m_VelocityDamping = value;
			}
		}

		public float velocityScale
		{
			get
			{
				return this.m_VelocityScale;
			}
			set
			{
				this.m_VelocityScale = value;
			}
		}

		public float angularVelocityDamping
		{
			get
			{
				return this.m_AngularVelocityDamping;
			}
			set
			{
				this.m_AngularVelocityDamping = value;
			}
		}

		public float angularVelocityScale
		{
			get
			{
				return this.m_AngularVelocityScale;
			}
			set
			{
				this.m_AngularVelocityScale = value;
			}
		}

		public bool trackPosition
		{
			get
			{
				return this.m_TrackPosition;
			}
			set
			{
				this.m_TrackPosition = value;
			}
		}

		public bool smoothPosition
		{
			get
			{
				return this.m_SmoothPosition;
			}
			set
			{
				this.m_SmoothPosition = value;
			}
		}

		public float smoothPositionAmount
		{
			get
			{
				return this.m_SmoothPositionAmount;
			}
			set
			{
				this.m_SmoothPositionAmount = value;
			}
		}

		public float tightenPosition
		{
			get
			{
				return this.m_TightenPosition;
			}
			set
			{
				this.m_TightenPosition = value;
			}
		}

		public bool trackRotation
		{
			get
			{
				return this.m_TrackRotation;
			}
			set
			{
				this.m_TrackRotation = value;
			}
		}

		public bool smoothRotation
		{
			get
			{
				return this.m_SmoothRotation;
			}
			set
			{
				this.m_SmoothRotation = value;
			}
		}

		public float smoothRotationAmount
		{
			get
			{
				return this.m_SmoothRotationAmount;
			}
			set
			{
				this.m_SmoothRotationAmount = value;
			}
		}

		public float tightenRotation
		{
			get
			{
				return this.m_TightenRotation;
			}
			set
			{
				this.m_TightenRotation = value;
			}
		}

		public bool trackScale
		{
			get
			{
				return this.m_TrackScale;
			}
			set
			{
				this.m_TrackScale = value;
			}
		}

		public bool smoothScale
		{
			get
			{
				return this.m_SmoothScale;
			}
			set
			{
				this.m_SmoothScale = value;
			}
		}

		public float smoothScaleAmount
		{
			get
			{
				return this.m_SmoothScaleAmount;
			}
			set
			{
				this.m_SmoothScaleAmount = value;
			}
		}

		public float tightenScale
		{
			get
			{
				return this.m_TightenScale;
			}
			set
			{
				this.m_TightenScale = value;
			}
		}

		public bool throwOnDetach
		{
			get
			{
				return this.m_ThrowOnDetach;
			}
			set
			{
				this.m_ThrowOnDetach = value;
			}
		}

		public float throwSmoothingDuration
		{
			get
			{
				return this.m_ThrowSmoothingDuration;
			}
			set
			{
				this.m_ThrowSmoothingDuration = value;
			}
		}

		public AnimationCurve throwSmoothingCurve
		{
			get
			{
				return this.m_ThrowSmoothingCurve;
			}
			set
			{
				this.m_ThrowSmoothingCurve = value;
			}
		}

		public float throwVelocityScale
		{
			get
			{
				return this.m_ThrowVelocityScale;
			}
			set
			{
				this.m_ThrowVelocityScale = value;
			}
		}

		public float throwAngularVelocityScale
		{
			get
			{
				return this.m_ThrowAngularVelocityScale;
			}
			set
			{
				this.m_ThrowAngularVelocityScale = value;
			}
		}

		public bool forceGravityOnDetach
		{
			get
			{
				return this.m_ForceGravityOnDetach;
			}
			set
			{
				this.m_ForceGravityOnDetach = value;
			}
		}

		public bool retainTransformParent
		{
			get
			{
				return this.m_RetainTransformParent;
			}
			set
			{
				this.m_RetainTransformParent = value;
			}
		}

		public List<XRBaseGrabTransformer> startingSingleGrabTransformers
		{
			get
			{
				return this.m_StartingSingleGrabTransformers;
			}
			set
			{
				this.m_StartingSingleGrabTransformers = value;
			}
		}

		public List<XRBaseGrabTransformer> startingMultipleGrabTransformers
		{
			get
			{
				return this.m_StartingMultipleGrabTransformers;
			}
			set
			{
				this.m_StartingMultipleGrabTransformers = value;
			}
		}

		public bool addDefaultGrabTransformers
		{
			get
			{
				return this.m_AddDefaultGrabTransformers;
			}
			set
			{
				this.m_AddDefaultGrabTransformers = value;
			}
		}

		public InteractableFarAttachMode farAttachMode
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

		public bool limitLinearVelocity
		{
			get
			{
				return this.m_LimitLinearVelocity;
			}
			set
			{
				this.m_LimitLinearVelocity = value;
			}
		}

		public bool limitAngularVelocity
		{
			get
			{
				return this.m_LimitAngularVelocity;
			}
			set
			{
				this.m_LimitAngularVelocity = value;
			}
		}

		public float maxLinearVelocityDelta
		{
			get
			{
				return this.m_MaxLinearVelocityDelta;
			}
			set
			{
				this.m_MaxLinearVelocityDelta = Mathf.Max(0f, value);
			}
		}

		public float maxAngularVelocityDelta
		{
			get
			{
				return this.m_MaxAngularVelocityDelta;
			}
			set
			{
				this.m_MaxAngularVelocityDelta = Mathf.Max(0f, value);
			}
		}

		private bool isRigidbodyMovement
		{
			get
			{
				return this.m_CurrentMovementType == XRBaseInteractable.MovementType.VelocityTracking || this.m_CurrentMovementType == XRBaseInteractable.MovementType.Kinematic;
			}
		}

		public int singleGrabTransformersCount
		{
			get
			{
				return this.m_SingleGrabTransformers.flushedCount;
			}
		}

		public int multipleGrabTransformersCount
		{
			get
			{
				return this.m_MultipleGrabTransformers.flushedCount;
			}
		}

		protected bool allowVisualAttachTransform { get; set; }

		private bool isTransformDirty
		{
			get
			{
				return this.m_IsTargetPoseDirty || this.m_IsTargetLocalScaleDirty;
			}
			set
			{
				this.m_IsTargetPoseDirty = value;
				this.m_IsTargetLocalScaleDirty = value;
			}
		}

		protected override void Reset()
		{
			Transform transform = null;
			int num = 0;
			for (int i = 0; i < base.transform.childCount; i++)
			{
				Transform child = base.transform.GetChild(i);
				MeshRenderer meshRenderer;
				SkinnedMeshRenderer skinnedMeshRenderer;
				if (child.TryGetComponent<MeshRenderer>(out meshRenderer) || child.TryGetComponent<SkinnedMeshRenderer>(out skinnedMeshRenderer))
				{
					if (num == 0)
					{
						transform = child;
					}
					num++;
				}
			}
			if (num == 1 && transform != null && transform.GetComponentInChildren<Collider>() == null)
			{
				this.m_PredictedVisualsTransform = transform;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			this.m_TeleportationMonitor = new TeleportationMonitor();
			this.m_TeleportationMonitor.teleported += this.OnTeleported;
			this.m_CurrentMovementType = this.m_MovementType;
			if (!base.TryGetComponent<Rigidbody>(out this.m_Rigidbody))
			{
				Debug.LogError("XR Grab Interactable does not have a required Rigidbody.", this);
			}
			this.m_Rigidbody.GetComponentsInChildren<Collider>(true, this.m_RigidbodyColliders);
			for (int i = this.m_RigidbodyColliders.Count - 1; i >= 0; i--)
			{
				if (this.m_RigidbodyColliders[i].attachedRigidbody != this.m_Rigidbody)
				{
					this.m_RigidbodyColliders.RemoveAt(i);
				}
			}
			this.m_Transform = base.transform;
			this.InitializeTargetPoseAndScale(this.m_Transform);
			this.FindStartingGrabTransformers();
			this.RegisterStartingGrabTransformers();
			this.FlushRegistration();
		}

		protected override void OnDestroy()
		{
			this.ClearSingleGrabTransformers();
			this.ClearMultipleGrabTransformers();
			base.OnDestroy();
		}

		protected virtual void OnCollisionStay()
		{
			this.m_RigidbodyColliding = true;
		}

		public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
		{
			base.ProcessInteractable(updatePhase);
			if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
			{
				this.AddDefaultGrabTransformers();
			}
			this.FlushRegistration();
			this.allowVisualAttachTransform = false;
			switch (updatePhase)
			{
			case XRInteractionUpdateOrder.UpdatePhase.Fixed:
				this.m_RigidbodyColliding = false;
				if ((base.isSelected || this.isTransformDirty) && this.isRigidbodyMovement)
				{
					if (this.m_IsTargetLocalScaleDirty && !this.m_IsTargetPoseDirty && !base.isSelected)
					{
						this.ApplyTargetScale();
					}
					else if (this.m_CurrentMovementType == XRBaseInteractable.MovementType.Kinematic)
					{
						this.PerformKinematicUpdate();
					}
					else if (this.m_CurrentMovementType == XRBaseInteractable.MovementType.VelocityTracking)
					{
						this.PerformVelocityTrackingUpdate(Time.deltaTime);
					}
					this.m_LastFixedFrame = Time.frameCount;
				}
				if (this.m_IgnoringCharacterCollision && !this.m_StopIgnoringCollisionInLateUpdate && this.m_SelectingCharacterInteractors.Count == 0 && this.m_SelectingCharacterController != null && this.IsOutsideCharacterCollider(this.m_SelectingCharacterController))
				{
					this.m_StopIgnoringCollisionInLateUpdate = true;
					return;
				}
				break;
			case XRInteractionUpdateOrder.UpdatePhase.Dynamic:
			case XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender:
				if (this.isTransformDirty)
				{
					if (this.m_IsTargetLocalScaleDirty && !this.m_IsTargetPoseDirty)
					{
						this.ApplyTargetScale();
					}
					else
					{
						this.PerformInstantaneousUpdate();
					}
				}
				if (base.isSelected || (this.m_GrabCountChanged && this.m_DropTransformersCount > 0))
				{
					this.UpdateTarget(updatePhase, Time.deltaTime);
					if (this.m_LastFixedFrame == Time.frameCount)
					{
						this.m_LastFixedDynamicTime = Time.time;
					}
					if (this.m_CurrentMovementType == XRBaseInteractable.MovementType.Instantaneous)
					{
						this.PerformInstantaneousUpdate();
					}
					else if (this.m_CurrentMovementType == XRBaseInteractable.MovementType.Kinematic)
					{
						this.PerformKinematicVisualsUpdate();
					}
					else if (this.m_CurrentMovementType == XRBaseInteractable.MovementType.VelocityTracking)
					{
						this.PerformVelocityVisualsUpdate();
					}
				}
				if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender && base.isSelected && this.isRigidbodyMovement && this.m_PredictedVisualsTransform != null)
				{
					this.PerformVisualAttachUpdate();
					return;
				}
				break;
			case XRInteractionUpdateOrder.UpdatePhase.Late:
				if (this.m_DetachInLateUpdate)
				{
					if (!base.isSelected)
					{
						this.Detach();
					}
					this.m_DetachInLateUpdate = false;
				}
				if (this.m_StopIgnoringCollisionInLateUpdate)
				{
					if (this.m_IgnoringCharacterCollision && this.m_SelectingCharacterController != null)
					{
						this.StopIgnoringCharacterCollision(this.m_SelectingCharacterController);
						this.m_SelectingCharacterController = null;
					}
					this.m_StopIgnoringCollisionInLateUpdate = false;
				}
				break;
			default:
				return;
			}
		}

		public override Transform GetAttachTransform(IXRInteractor interactor)
		{
			IXRSelectInteractor ixrselectInteractor = interactor as IXRSelectInteractor;
			bool flag = ixrselectInteractor != null;
			Transform result;
			if (this.allowVisualAttachTransform && base.isSelected && this.isRigidbodyMovement && this.m_PredictedVisualsTransform != null && flag && this.m_VisualAttachTransforms.TryGetValue(ixrselectInteractor, out result))
			{
				return result;
			}
			bool flag2 = base.interactorsSelecting.Count <= 1 || interactor == base.interactorsSelecting[0];
			bool flag3 = this.m_UseDynamicAttach || (!flag2 && this.m_SecondaryAttachTransform == null);
			Transform transform;
			if (flag3 && flag && this.m_DynamicAttachTransforms.TryGetValue(ixrselectInteractor, out transform))
			{
				if (transform != null)
				{
					return transform;
				}
				this.m_DynamicAttachTransforms.Remove(ixrselectInteractor);
				Debug.LogWarning(string.Format("Dynamic Attach Transform created by {0} for {1} was destroyed after being created.", this, interactor) + " Continuing as if Use Dynamic Attach was disabled for this pair.", this);
			}
			if (!flag2 && !flag3)
			{
				return this.m_SecondaryAttachTransform;
			}
			if (!(this.m_AttachTransform != null))
			{
				return base.GetAttachTransform(interactor);
			}
			return this.m_AttachTransform;
		}

		public void AddSingleGrabTransformer(IXRGrabTransformer transformer)
		{
			this.AddGrabTransformer(transformer, this.m_SingleGrabTransformers);
		}

		public void AddMultipleGrabTransformer(IXRGrabTransformer transformer)
		{
			this.AddGrabTransformer(transformer, this.m_MultipleGrabTransformers);
		}

		public bool RemoveSingleGrabTransformer(IXRGrabTransformer transformer)
		{
			return this.RemoveGrabTransformer(transformer, this.m_SingleGrabTransformers);
		}

		public bool RemoveMultipleGrabTransformer(IXRGrabTransformer transformer)
		{
			return this.RemoveGrabTransformer(transformer, this.m_MultipleGrabTransformers);
		}

		public void ClearSingleGrabTransformers()
		{
			this.ClearGrabTransformers(this.m_SingleGrabTransformers);
		}

		public void ClearMultipleGrabTransformers()
		{
			this.ClearGrabTransformers(this.m_MultipleGrabTransformers);
		}

		public void GetSingleGrabTransformers(List<IXRGrabTransformer> results)
		{
			XRGrabInteractable.GetGrabTransformers(this.m_SingleGrabTransformers, results);
		}

		public void GetMultipleGrabTransformers(List<IXRGrabTransformer> results)
		{
			XRGrabInteractable.GetGrabTransformers(this.m_MultipleGrabTransformers, results);
		}

		public IXRGrabTransformer GetSingleGrabTransformerAt(int index)
		{
			return this.m_SingleGrabTransformers.GetRegisteredItemAt(index);
		}

		public IXRGrabTransformer GetMultipleGrabTransformerAt(int index)
		{
			return this.m_MultipleGrabTransformers.GetRegisteredItemAt(index);
		}

		public void MoveSingleGrabTransformerTo(IXRGrabTransformer transformer, int newIndex)
		{
			this.MoveGrabTransformerTo(transformer, newIndex, this.m_SingleGrabTransformers);
		}

		public void MoveMultipleGrabTransformerTo(IXRGrabTransformer transformer, int newIndex)
		{
			this.MoveGrabTransformerTo(transformer, newIndex, this.m_MultipleGrabTransformers);
		}

		public Pose GetTargetPose()
		{
			return this.m_TargetPose;
		}

		public void SetTargetPose(Pose pose)
		{
			this.m_TargetPose = pose;
			this.m_IsTargetPoseDirty = (base.interactorsSelecting.Count == 0);
		}

		public Vector3 GetTargetLocalScale()
		{
			return this.m_TargetLocalScale;
		}

		public void SetTargetLocalScale(Vector3 localScale)
		{
			this.m_TargetLocalScale = localScale;
			this.m_IsTargetLocalScaleDirty = (base.interactorsSelecting.Count == 0);
		}

		private void InitializeTargetPoseAndScale(Transform thisTransform)
		{
			if (!this.m_IsTargetPoseDirty)
			{
				this.m_TargetPose = thisTransform.GetWorldPose();
			}
			if (!this.m_IsTargetLocalScaleDirty)
			{
				this.m_TargetLocalScale = thisTransform.localScale;
			}
		}

		private void AddGrabTransformer(IXRGrabTransformer transformer, BaseRegistrationList<IXRGrabTransformer> grabTransformers)
		{
			if (transformer == null)
			{
				throw new ArgumentNullException("transformer");
			}
			if (this.m_IsProcessingGrabTransformers)
			{
				Debug.LogWarning(string.Format("{0} added while {1} is processing grab transformers. It won't be processed until the next process.", transformer, base.name), this);
			}
			if (grabTransformers.Register(transformer))
			{
				this.OnAddedGrabTransformer(transformer);
			}
		}

		private bool RemoveGrabTransformer(IXRGrabTransformer transformer, BaseRegistrationList<IXRGrabTransformer> grabTransformers)
		{
			if (grabTransformers.Unregister(transformer))
			{
				this.OnRemovedGrabTransformer(transformer);
				return true;
			}
			return false;
		}

		private void ClearGrabTransformers(BaseRegistrationList<IXRGrabTransformer> grabTransformers)
		{
			for (int i = grabTransformers.flushedCount - 1; i >= 0; i--)
			{
				IXRGrabTransformer registeredItemAt = grabTransformers.GetRegisteredItemAt(i);
				this.RemoveGrabTransformer(registeredItemAt, grabTransformers);
			}
		}

		private static void GetGrabTransformers(BaseRegistrationList<IXRGrabTransformer> grabTransformers, List<IXRGrabTransformer> results)
		{
			if (results == null)
			{
				throw new ArgumentNullException("results");
			}
			grabTransformers.GetRegisteredItems(results);
		}

		private void MoveGrabTransformerTo(IXRGrabTransformer transformer, int newIndex, BaseRegistrationList<IXRGrabTransformer> grabTransformers)
		{
			if (transformer == null)
			{
				throw new ArgumentNullException("transformer");
			}
			if (this.m_IsProcessingGrabTransformers)
			{
				Debug.LogError(string.Format("Cannot move {0} while {1} is processing grab transformers.", transformer, base.name), this);
				return;
			}
			grabTransformers.Flush();
			if (grabTransformers.MoveItemImmediately(transformer, newIndex))
			{
				this.OnAddedGrabTransformer(transformer);
			}
		}

		private void FindStartingGrabTransformers()
		{
			if (this.m_StartingSingleGrabTransformers.Count != 0 || this.m_StartingMultipleGrabTransformers.Count != 0)
			{
				return;
			}
			List<IXRGrabTransformer> list;
			using (UnityEngine.Pool.CollectionPool<List<IXRGrabTransformer>, IXRGrabTransformer>.Get(out list))
			{
				base.GetComponents<IXRGrabTransformer>(list);
				if (list.Count != 0)
				{
					bool flag = false;
					foreach (IXRGrabTransformer ixrgrabTransformer in list)
					{
						XRBaseGrabTransformer xrbaseGrabTransformer = ixrgrabTransformer as XRBaseGrabTransformer;
						if (xrbaseGrabTransformer != null)
						{
							switch (xrbaseGrabTransformer.GetRegistrationMode())
							{
							case XRBaseGrabTransformer.RegistrationMode.Single:
								this.m_StartingSingleGrabTransformers.Add(xrbaseGrabTransformer);
								break;
							case XRBaseGrabTransformer.RegistrationMode.Multiple:
								this.m_StartingMultipleGrabTransformers.Add(xrbaseGrabTransformer);
								break;
							case XRBaseGrabTransformer.RegistrationMode.SingleAndMultiple:
								this.m_StartingSingleGrabTransformers.Add(xrbaseGrabTransformer);
								this.m_StartingMultipleGrabTransformers.Add(xrbaseGrabTransformer);
								break;
							}
						}
						else
						{
							flag = true;
						}
					}
					if (flag)
					{
						string text = "XR Grab Interactable \"" + base.name + "\" has a custom IXRGrabTransformer component on the same GameObject that cannot be added to either the Starting Multiple Grab Transformers or Starting Single Grab Transformers lists. Custom transformers must be registered during runtime using methods like AddSingleGrabTransformer and AddMultipleGrabTransformer.";
						if (this.m_StartingSingleGrabTransformers.Count > 0 || this.m_StartingMultipleGrabTransformers.Count > 0)
						{
							text += " The other XRBaseGrabTransformer derived grab transformers have been added to the starting lists.";
						}
						Debug.LogWarning(text, this);
					}
				}
			}
		}

		private void RegisterStartingGrabTransformers()
		{
			if (this.m_SingleGrabTransformers.flushedCount > 0)
			{
				int num = 0;
				using (List<XRBaseGrabTransformer>.Enumerator enumerator = this.m_StartingSingleGrabTransformers.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						XRBaseGrabTransformer xrbaseGrabTransformer = enumerator.Current;
						if (xrbaseGrabTransformer != null)
						{
							this.MoveSingleGrabTransformerTo(xrbaseGrabTransformer, num++);
						}
					}
					goto IL_93;
				}
			}
			foreach (XRBaseGrabTransformer xrbaseGrabTransformer2 in this.m_StartingSingleGrabTransformers)
			{
				if (xrbaseGrabTransformer2 != null)
				{
					this.AddSingleGrabTransformer(xrbaseGrabTransformer2);
				}
			}
			IL_93:
			if (this.m_MultipleGrabTransformers.flushedCount > 0)
			{
				int num2 = 0;
				using (List<XRBaseGrabTransformer>.Enumerator enumerator = this.m_StartingMultipleGrabTransformers.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						XRBaseGrabTransformer xrbaseGrabTransformer3 = enumerator.Current;
						if (xrbaseGrabTransformer3 != null)
						{
							this.MoveMultipleGrabTransformerTo(xrbaseGrabTransformer3, num2++);
						}
					}
					return;
				}
			}
			foreach (XRBaseGrabTransformer xrbaseGrabTransformer4 in this.m_StartingMultipleGrabTransformers)
			{
				if (xrbaseGrabTransformer4 != null)
				{
					this.AddMultipleGrabTransformer(xrbaseGrabTransformer4);
				}
			}
		}

		private void FlushRegistration()
		{
			this.m_SingleGrabTransformers.Flush();
			this.m_MultipleGrabTransformers.Flush();
		}

		private void InvokeGrabTransformersOnGrab()
		{
			this.m_IsProcessingGrabTransformers = true;
			if (this.m_SingleGrabTransformers.registeredSnapshot.Count > 0)
			{
				foreach (IXRGrabTransformer ixrgrabTransformer in this.m_SingleGrabTransformers.registeredSnapshot)
				{
					if (this.m_SingleGrabTransformers.IsStillRegistered(ixrgrabTransformer))
					{
						ixrgrabTransformer.OnGrab(this);
					}
				}
			}
			if (this.m_MultipleGrabTransformers.registeredSnapshot.Count > 0)
			{
				foreach (IXRGrabTransformer ixrgrabTransformer2 in this.m_MultipleGrabTransformers.registeredSnapshot)
				{
					if (this.m_MultipleGrabTransformers.IsStillRegistered(ixrgrabTransformer2))
					{
						ixrgrabTransformer2.OnGrab(this);
					}
				}
			}
			this.m_IsProcessingGrabTransformers = false;
		}

		private void InvokeGrabTransformersOnDrop(DropEventArgs args)
		{
			this.m_IsProcessingGrabTransformers = true;
			if (this.m_SingleGrabTransformers.registeredSnapshot.Count > 0)
			{
				foreach (IXRGrabTransformer ixrgrabTransformer in this.m_SingleGrabTransformers.registeredSnapshot)
				{
					IXRDropTransformer ixrdropTransformer = ixrgrabTransformer as IXRDropTransformer;
					if (ixrdropTransformer != null && this.m_SingleGrabTransformers.IsStillRegistered(ixrgrabTransformer))
					{
						ixrdropTransformer.OnDrop(this, args);
					}
				}
			}
			if (this.m_MultipleGrabTransformers.registeredSnapshot.Count > 0)
			{
				foreach (IXRGrabTransformer ixrgrabTransformer2 in this.m_MultipleGrabTransformers.registeredSnapshot)
				{
					IXRDropTransformer ixrdropTransformer2 = ixrgrabTransformer2 as IXRDropTransformer;
					if (ixrdropTransformer2 != null && this.m_MultipleGrabTransformers.IsStillRegistered(ixrgrabTransformer2))
					{
						ixrdropTransformer2.OnDrop(this, args);
					}
				}
			}
			this.m_IsProcessingGrabTransformers = false;
		}

		private void InvokeGrabTransformersProcess(XRInteractionUpdateOrder.UpdatePhase updatePhase, ref Pose targetPose, ref Vector3 localScale)
		{
			this.m_IsProcessingGrabTransformers = true;
			using (XRGrabInteractable.s_ProcessGrabTransformersMarker.Auto())
			{
				bool isSelected = base.isSelected;
				bool flag = this.m_SingleGrabTransformers.registeredSnapshot.Count > 0;
				bool flag2 = this.m_MultipleGrabTransformers.registeredSnapshot.Count > 0;
				if (this.m_GrabCountChanged)
				{
					if (isSelected)
					{
						if (flag)
						{
							foreach (IXRGrabTransformer ixrgrabTransformer in this.m_SingleGrabTransformers.registeredSnapshot)
							{
								if (this.m_SingleGrabTransformers.IsStillRegistered(ixrgrabTransformer))
								{
									ixrgrabTransformer.OnGrabCountChanged(this, targetPose, localScale);
								}
							}
						}
						if (flag2)
						{
							foreach (IXRGrabTransformer ixrgrabTransformer2 in this.m_MultipleGrabTransformers.registeredSnapshot)
							{
								if (this.m_MultipleGrabTransformers.IsStillRegistered(ixrgrabTransformer2))
								{
									ixrgrabTransformer2.OnGrabCountChanged(this, targetPose, localScale);
								}
							}
						}
					}
					this.m_GrabCountChanged = false;
					List<IXRGrabTransformer> grabTransformersAddedWhenGrabbed = this.m_GrabTransformersAddedWhenGrabbed;
					if (grabTransformersAddedWhenGrabbed != null)
					{
						grabTransformersAddedWhenGrabbed.Clear();
					}
				}
				else
				{
					List<IXRGrabTransformer> grabTransformersAddedWhenGrabbed2 = this.m_GrabTransformersAddedWhenGrabbed;
					if (grabTransformersAddedWhenGrabbed2 != null && grabTransformersAddedWhenGrabbed2.Count > 0)
					{
						if (isSelected)
						{
							foreach (IXRGrabTransformer ixrgrabTransformer3 in this.m_GrabTransformersAddedWhenGrabbed)
							{
								ixrgrabTransformer3.OnGrabCountChanged(this, targetPose, localScale);
							}
						}
						this.m_GrabTransformersAddedWhenGrabbed.Clear();
					}
				}
				if (isSelected)
				{
					bool flag3 = false;
					if (flag2 && (base.interactorsSelecting.Count > 1 || !this.CanProcessAnySingleGrabTransformer()))
					{
						foreach (IXRGrabTransformer ixrgrabTransformer4 in this.m_MultipleGrabTransformers.registeredSnapshot)
						{
							if (this.m_MultipleGrabTransformers.IsStillRegistered(ixrgrabTransformer4) && ixrgrabTransformer4.canProcess)
							{
								ixrgrabTransformer4.Process(this, updatePhase, ref targetPose, ref localScale);
								flag3 = true;
							}
						}
					}
					if (flag3 || !flag)
					{
						goto IL_35B;
					}
					using (List<IXRGrabTransformer>.Enumerator enumerator = this.m_SingleGrabTransformers.registeredSnapshot.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							IXRGrabTransformer ixrgrabTransformer5 = enumerator.Current;
							if (this.m_SingleGrabTransformers.IsStillRegistered(ixrgrabTransformer5) && ixrgrabTransformer5.canProcess)
							{
								ixrgrabTransformer5.Process(this, updatePhase, ref targetPose, ref localScale);
							}
						}
						goto IL_36B;
					}
				}
				if (flag2)
				{
					foreach (IXRGrabTransformer ixrgrabTransformer6 in this.m_MultipleGrabTransformers.registeredSnapshot)
					{
						IXRDropTransformer ixrdropTransformer = ixrgrabTransformer6 as IXRDropTransformer;
						if (ixrdropTransformer != null && this.m_MultipleGrabTransformers.IsStillRegistered(ixrgrabTransformer6) && ixrdropTransformer.canProcessOnDrop && ixrgrabTransformer6.canProcess)
						{
							ixrgrabTransformer6.Process(this, updatePhase, ref targetPose, ref localScale);
						}
					}
				}
				if (flag)
				{
					foreach (IXRGrabTransformer ixrgrabTransformer7 in this.m_SingleGrabTransformers.registeredSnapshot)
					{
						IXRDropTransformer ixrdropTransformer2 = ixrgrabTransformer7 as IXRDropTransformer;
						if (ixrdropTransformer2 != null && this.m_SingleGrabTransformers.IsStillRegistered(ixrgrabTransformer7) && ixrdropTransformer2.canProcessOnDrop && ixrgrabTransformer7.canProcess)
						{
							ixrgrabTransformer7.Process(this, updatePhase, ref targetPose, ref localScale);
						}
					}
				}
				IL_35B:;
			}
			IL_36B:
			this.m_IsProcessingGrabTransformers = false;
		}

		private bool CanProcessAnySingleGrabTransformer()
		{
			if (this.m_SingleGrabTransformers.registeredSnapshot.Count > 0)
			{
				foreach (IXRGrabTransformer ixrgrabTransformer in this.m_SingleGrabTransformers.registeredSnapshot)
				{
					if (this.m_SingleGrabTransformers.IsStillRegistered(ixrgrabTransformer) && ixrgrabTransformer.canProcess)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		private void OnAddedGrabTransformer(IXRGrabTransformer transformer)
		{
			if (transformer is IXRDropTransformer)
			{
				this.m_DropTransformersCount++;
			}
			transformer.OnLink(this);
			if (base.interactorsSelecting.Count == 0)
			{
				return;
			}
			transformer.OnGrab(this);
			if (this.m_GrabTransformersAddedWhenGrabbed == null)
			{
				this.m_GrabTransformersAddedWhenGrabbed = new List<IXRGrabTransformer>();
			}
			this.m_GrabTransformersAddedWhenGrabbed.Add(transformer);
		}

		private void OnRemovedGrabTransformer(IXRGrabTransformer transformer)
		{
			if (transformer is IXRDropTransformer)
			{
				this.m_DropTransformersCount--;
			}
			transformer.OnUnlink(this);
			List<IXRGrabTransformer> grabTransformersAddedWhenGrabbed = this.m_GrabTransformersAddedWhenGrabbed;
			if (grabTransformersAddedWhenGrabbed == null)
			{
				return;
			}
			grabTransformersAddedWhenGrabbed.Remove(transformer);
		}

		private bool AddDefaultGrabTransformers()
		{
			if (!this.m_AddDefaultGrabTransformers)
			{
				return false;
			}
			bool result = false;
			if (this.m_SingleGrabTransformers.flushedCount == 0)
			{
				this.AddDefaultSingleGrabTransformer();
				result = true;
			}
			if (base.selectMode == InteractableSelectMode.Multiple && base.interactorsSelecting.Count > 1 && this.m_MultipleGrabTransformers.flushedCount == 0)
			{
				this.AddDefaultMultipleGrabTransformer();
				result = true;
			}
			return result;
		}

		protected virtual void AddDefaultSingleGrabTransformer()
		{
			if (this.m_SingleGrabTransformers.flushedCount == 0)
			{
				IXRGrabTransformer orAddDefaultGrabTransformer = this.GetOrAddDefaultGrabTransformer();
				this.AddSingleGrabTransformer(orAddDefaultGrabTransformer);
			}
		}

		protected virtual void AddDefaultMultipleGrabTransformer()
		{
			if (this.m_MultipleGrabTransformers.flushedCount == 0)
			{
				IXRGrabTransformer orAddDefaultGrabTransformer = this.GetOrAddDefaultGrabTransformer();
				this.AddMultipleGrabTransformer(orAddDefaultGrabTransformer);
			}
		}

		private IXRGrabTransformer GetOrAddDefaultGrabTransformer()
		{
			return this.GetOrAddComponent<XRGeneralGrabTransformer>();
		}

		private T GetOrAddComponent<T>() where T : Component
		{
			T result;
			if (!base.TryGetComponent<T>(out result))
			{
				return base.gameObject.AddComponent<T>();
			}
			return result;
		}

		private void UpdateTarget(XRInteractionUpdateOrder.UpdatePhase updatePhase, float deltaTime)
		{
			Transform dynamicAttachTransform;
			if (this.m_ReinitializeDynamicAttachEverySingleGrab && this.m_GrabCountChanged && this.m_GrabCountBeforeAndAfterChange.Item2 < this.m_GrabCountBeforeAndAfterChange.Item1 && base.interactorsSelecting.Count == 1 && this.m_DynamicAttachTransforms.Count > 0 && this.m_DynamicAttachTransforms.TryGetValue(base.interactorsSelecting[0], out dynamicAttachTransform))
			{
				this.InitializeDynamicAttachPoseInternal(base.interactorsSelecting[0], dynamicAttachTransform);
			}
			Pose targetPose = this.m_TargetPose;
			Vector3 targetLocalScale = this.m_TargetLocalScale;
			this.InvokeGrabTransformersProcess(updatePhase, ref targetPose, ref targetLocalScale);
			if (!base.isSelected)
			{
				this.m_TargetPose = targetPose;
				this.m_TargetLocalScale = targetLocalScale;
				return;
			}
			if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
			{
				this.StepThrowSmoothing(targetPose, deltaTime);
			}
			this.StepSmoothing(targetPose, targetLocalScale, deltaTime);
		}

		private void StepSmoothing(in Pose rawTargetPose, in Vector3 rawTargetLocalScale, float deltaTime)
		{
			if (this.m_AttachEaseInTime > 0f && this.m_CurrentAttachEaseTime <= this.m_AttachEaseInTime)
			{
				XRGrabInteractable.EaseAttachBurst(ref this.m_TargetPose, ref this.m_TargetLocalScale, rawTargetPose, rawTargetLocalScale, deltaTime, this.m_AttachEaseInTime, ref this.m_CurrentAttachEaseTime);
				return;
			}
			XRGrabInteractable.StepSmoothingBurst(ref this.m_TargetPose, ref this.m_TargetLocalScale, rawTargetPose, rawTargetLocalScale, deltaTime, this.m_SmoothPosition, this.m_SmoothPositionAmount, this.m_TightenPosition, this.m_SmoothRotation, this.m_SmoothRotationAmount, this.m_TightenRotation, this.m_SmoothScale, this.m_SmoothScaleAmount, this.m_TightenScale);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(XRGrabInteractable.EaseAttachBurst_00000F9A$PostfixBurstDelegate))]
		private static void EaseAttachBurst(ref Pose targetPose, ref Vector3 targetLocalScale, in Pose rawTargetPose, in Vector3 rawTargetLocalScale, float deltaTime, float attachEaseInTime, ref float currentAttachEaseTime)
		{
			XRGrabInteractable.EaseAttachBurst_00000F9A$BurstDirectCall.Invoke(ref targetPose, ref targetLocalScale, rawTargetPose, rawTargetLocalScale, deltaTime, attachEaseInTime, ref currentAttachEaseTime);
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(XRGrabInteractable.StepSmoothingBurst_00000F9B$PostfixBurstDelegate))]
		private static void StepSmoothingBurst(ref Pose targetPose, ref Vector3 targetLocalScale, in Pose rawTargetPose, in Vector3 rawTargetLocalScale, float deltaTime, bool smoothPos, float smoothPosAmount, float tightenPos, bool smoothRot, float smoothRotAmount, float tightenRot, bool smoothScale, float smoothScaleAmount, float tightenScale)
		{
			XRGrabInteractable.StepSmoothingBurst_00000F9B$BurstDirectCall.Invoke(ref targetPose, ref targetLocalScale, rawTargetPose, rawTargetLocalScale, deltaTime, smoothPos, smoothPosAmount, tightenPos, smoothRot, smoothRotAmount, tightenRot, smoothScale, smoothScaleAmount, tightenScale);
		}

		private void PerformInstantaneousUpdate()
		{
			if (this.m_TrackPosition && this.m_TrackRotation)
			{
				this.m_Transform.SetWorldPose(this.m_TargetPose);
			}
			else if (this.m_TrackPosition)
			{
				this.m_Transform.position = this.m_TargetPose.position;
			}
			else if (this.m_TrackRotation)
			{
				this.m_Transform.rotation = this.m_TargetPose.rotation;
			}
			this.ApplyTargetScale();
			this.isTransformDirty = false;
		}

		private void PerformKinematicUpdate()
		{
			if (this.m_TrackPosition)
			{
				this.m_Rigidbody.MovePosition(this.m_TargetPose.position);
			}
			if (this.m_TrackRotation)
			{
				this.m_Rigidbody.MoveRotation(this.m_TargetPose.rotation);
			}
			this.ApplyTargetScale();
			this.isTransformDirty = false;
		}

		private void PerformVelocityTrackingUpdate(float fixedDeltaTime)
		{
			if (fixedDeltaTime < 0.001f)
			{
				return;
			}
			if (this.m_TrackPosition)
			{
				Vector3 vector = this.m_Rigidbody.linearVelocity;
				vector *= 1f - this.m_VelocityDamping;
				Vector3 a = this.m_TargetPose.position - this.m_Rigidbody.position;
				Vector3 vector2 = vector + a / fixedDeltaTime * this.m_VelocityScale;
				Vector3 linearVelocity = this.m_LimitLinearVelocity ? Vector3.MoveTowards(vector, vector2, this.m_MaxLinearVelocityDelta) : vector2;
				this.m_Rigidbody.linearVelocity = linearVelocity;
			}
			if (this.m_TrackRotation)
			{
				Vector3 vector3 = this.m_Rigidbody.angularVelocity;
				vector3 *= 1f - this.m_AngularVelocityDamping;
				float num;
				Vector3 normalized;
				(this.m_TargetPose.rotation * Quaternion.Inverse(this.m_Rigidbody.rotation)).ToAngleAxis(out num, out normalized);
				if (num > 180f)
				{
					num -= 360f;
				}
				if (Mathf.Abs(num) > Mathf.Epsilon)
				{
					normalized = normalized.normalized;
					Vector3 a2 = normalized * (num * 0.017453292f) / fixedDeltaTime;
					Vector3 vector4 = vector3 + a2 * this.m_AngularVelocityScale;
					this.m_Rigidbody.angularVelocity = (this.m_LimitAngularVelocity ? Vector3.MoveTowards(vector3, vector4, this.m_MaxAngularVelocityDelta) : vector4);
				}
				else
				{
					this.m_Rigidbody.angularVelocity = vector3;
				}
			}
			this.ApplyTargetScale();
			this.isTransformDirty = false;
		}

		private void PerformVelocityVisualsUpdate()
		{
			if (this.m_PredictedVisualsTransform == null)
			{
				return;
			}
			float fixedDeltaTime = Time.fixedDeltaTime;
			float deltaTime = Time.deltaTime;
			if (fixedDeltaTime < 0.001f)
			{
				return;
			}
			if (deltaTime < 0.001f)
			{
				return;
			}
			if (this.m_RigidbodyColliding || this.m_Rigidbody.IsSleeping())
			{
				this.m_PredictedVisualsTransform.SetLocalPose(this.m_InitialVisualsTransformLocalPose);
				this.m_PredictedVisualsTransform.localScale = this.m_InitialVisualsTransformLocalScale;
				this.m_Rigidbody.interpolation = this.m_InterpolationOnGrab;
				return;
			}
			this.m_Rigidbody.interpolation = RigidbodyInterpolation.None;
			Pose identity = Pose.identity;
			if (this.m_TrackPosition)
			{
				Vector3 vector = this.m_Rigidbody.linearVelocity;
				vector *= 1f - this.m_VelocityDamping;
				Vector3 a = this.m_TargetPose.position - this.m_Rigidbody.position;
				Vector3 vector2 = vector + a / fixedDeltaTime * this.m_VelocityScale;
				Vector3 a2 = vector2;
				if (this.m_LimitLinearVelocity)
				{
					float maxDistanceDelta = Mathf.Min(this.m_Rigidbody.maxLinearVelocity, this.m_MaxLinearVelocityDelta);
					a2 = Vector3.MoveTowards(vector, vector2, maxDistanceDelta);
				}
				Vector3 vector3 = (a2 - vector) * fixedDeltaTime;
				if (Mathf.Abs(vector3.x) <= 0.001f && Mathf.Abs(vector3.y) <= 0.001f && Mathf.Abs(vector3.z) <= 0.001f)
				{
					identity.position = this.m_Rigidbody.position;
				}
				else
				{
					identity.position = this.m_Rigidbody.position + vector3;
				}
			}
			if (this.m_TrackRotation)
			{
				Vector3 a3 = this.m_Rigidbody.angularVelocity;
				a3 *= 1f - this.m_AngularVelocityDamping;
				float num;
				Vector3 normalized;
				(this.m_TargetPose.rotation * Quaternion.Inverse(this.m_Rigidbody.rotation)).ToAngleAxis(out num, out normalized);
				if (num > 180f)
				{
					num -= 360f;
				}
				if (Mathf.Abs(num) > Mathf.Epsilon)
				{
					normalized = normalized.normalized;
					Vector3 a4 = normalized * (num * 0.017453292f) / fixedDeltaTime;
					Vector3 vector4 = a3 + a4 * this.m_AngularVelocityScale;
					float num2 = this.m_LimitAngularVelocity ? Mathf.Min(this.m_Rigidbody.maxAngularVelocity, this.m_MaxAngularVelocityDelta) : this.m_Rigidbody.maxAngularVelocity;
					if (vector4.sqrMagnitude <= num2 * num2)
					{
						identity.rotation = this.m_TargetPose.rotation;
					}
					else
					{
						float num3 = Time.time - this.m_LastFixedDynamicTime + deltaTime;
						if (num3 >= Time.fixedDeltaTime)
						{
							num3 = Time.fixedDeltaTime;
						}
						float maxDegreesDelta = num2 * 57.29578f * num3;
						Quaternion rotation = Quaternion.RotateTowards(this.m_Rigidbody.rotation, this.m_TargetPose.rotation, maxDegreesDelta);
						identity.rotation = rotation;
					}
				}
				else
				{
					identity.rotation = this.m_Rigidbody.rotation;
				}
			}
			this.ApplyVisuals(identity);
		}

		private void PerformKinematicVisualsUpdate()
		{
			if (this.m_PredictedVisualsTransform == null)
			{
				return;
			}
			this.ApplyVisuals(this.m_TargetPose);
		}

		private void ApplyVisuals(Pose visualsPose)
		{
			Pose pose;
			if (this.m_InitialVisualsTransformLocalPoseIsIdentity)
			{
				pose = visualsPose;
			}
			else
			{
				Pose initialVisualsTransformLocalPose = this.m_InitialVisualsTransformLocalPose;
				initialVisualsTransformLocalPose.position = Vector3.Scale(initialVisualsTransformLocalPose.position, this.m_TrackScale ? this.m_TargetLocalScale : this.m_Transform.localScale);
				pose = initialVisualsTransformLocalPose.GetTransformedBy(visualsPose);
			}
			if (this.m_TrackPosition && this.m_TrackRotation)
			{
				this.m_PredictedVisualsTransform.SetWorldPose(pose);
			}
			else if (this.m_TrackPosition)
			{
				this.m_PredictedVisualsTransform.position = pose.position;
			}
			else if (this.m_TrackRotation)
			{
				this.m_PredictedVisualsTransform.rotation = pose.rotation;
			}
			if (this.m_TrackScale)
			{
				Vector3 a = this.m_TargetLocalScale.SafeDivide(this.m_Transform.localScale);
				this.m_PredictedVisualsTransform.localScale = Vector3.Scale(a, this.m_InitialVisualsTransformLocalScale);
			}
		}

		private void ApplyTargetScale()
		{
			if (this.m_TrackScale)
			{
				this.m_Transform.localScale = this.m_TargetLocalScale;
			}
			this.m_IsTargetLocalScaleDirty = false;
		}

		private void PerformVisualAttachUpdate()
		{
			this.allowVisualAttachTransform = false;
			foreach (IXRSelectInteractor ixrselectInteractor in base.interactorsSelecting)
			{
				Transform transform;
				if (this.m_VisualAttachTransforms.TryGetValue(ixrselectInteractor, out transform))
				{
					Pose pose;
					if (this.m_InitialVisualsTransformLocalPoseIsIdentity)
					{
						pose = this.m_PredictedVisualsTransform.GetWorldPose();
					}
					else
					{
						Quaternion rotation = Quaternion.Inverse(this.m_InitialVisualsTransformLocalPose.rotation);
						Vector3 position = -(rotation * this.m_InitialVisualsTransformLocalPose.position);
						Pose pose2 = new Pose(position, rotation);
						pose = pose2.GetTransformedBy(this.m_PredictedVisualsTransform.GetWorldPose());
					}
					Transform attachTransform = this.GetAttachTransform(ixrselectInteractor);
					if (attachTransform == this.m_Transform)
					{
						transform.SetWorldPose(pose);
					}
					else
					{
						Pose pose3 = (attachTransform.parent == this.m_Transform) ? attachTransform.GetLocalPose() : this.m_Transform.InverseTransformPose(attachTransform.GetWorldPose());
						transform.SetWorldPose(pose3.GetTransformedBy(pose));
					}
					transform.localPosition = Vector3.Scale(transform.localPosition, this.m_TrackScale ? this.m_TargetLocalScale : this.m_Transform.localScale);
				}
			}
			this.allowVisualAttachTransform = true;
		}

		private void UpdateCurrentMovementType()
		{
			if (!base.isSelected)
			{
				this.m_CurrentMovementType = this.m_MovementType;
				return;
			}
			XRBaseInteractable.MovementType? movementType = null;
			for (int i = base.interactorsSelecting.Count - 1; i >= 0; i--)
			{
				XRBaseInteractor xrbaseInteractor = base.interactorsSelecting[i] as XRBaseInteractor;
				if (xrbaseInteractor != null && xrbaseInteractor.selectedInteractableMovementTypeOverride != null)
				{
					if (movementType == null)
					{
						movementType = new XRBaseInteractable.MovementType?(xrbaseInteractor.selectedInteractableMovementTypeOverride.Value);
					}
					else
					{
						XRBaseInteractable.MovementType? movementType2 = movementType;
						XRBaseInteractable.MovementType? selectedInteractableMovementTypeOverride = xrbaseInteractor.selectedInteractableMovementTypeOverride;
						if (!(movementType2.GetValueOrDefault() == selectedInteractableMovementTypeOverride.GetValueOrDefault() & movementType2 != null == (selectedInteractableMovementTypeOverride != null)))
						{
							Debug.LogWarning("Multiple interactors selecting \"" + base.name + "\" have different movement type override values set (selectedInteractableMovementTypeOverride)." + string.Format(" Conflict resolved using {0} from the most recent interactor to select this object with an override.", movementType.Value), this);
						}
					}
				}
			}
			XRBaseInteractable.MovementType movementType3 = movementType ?? this.m_MovementType;
			if (movementType3 == this.m_CurrentMovementType)
			{
				return;
			}
			this.SetupRigidbodyDrop(this.m_Rigidbody);
			this.m_CurrentMovementType = movementType3;
			this.SetupRigidbodyGrab(this.m_Rigidbody);
			if (this.m_CurrentMovementType == XRBaseInteractable.MovementType.Instantaneous && this.m_PredictedVisualsTransform != null)
			{
				this.m_PredictedVisualsTransform.SetLocalPose(this.m_InitialVisualsTransformLocalPose);
				this.m_PredictedVisualsTransform.localScale = this.m_InitialVisualsTransformLocalScale;
			}
		}

		protected override void OnHoverEntering(HoverEnterEventArgs args)
		{
			base.OnHoverEntering(args);
			this.AddDefaultGrabTransformers();
		}

		protected override void OnSelectEntering(SelectEnterEventArgs args)
		{
			Transform transform = this.CreateDynamicAttachTransform(args.interactorObject);
			this.InitializeDynamicAttachPoseInternal(args.interactorObject, transform);
			if (this.m_PredictedVisualsTransform != null)
			{
				Transform transform2 = this.CreateVisualAttachTransform(args.interactorObject);
				this.m_VisualAttachTransforms.Remove(args.interactorObject);
				this.m_VisualAttachTransforms[args.interactorObject] = transform2;
				transform2.SetWorldPose(transform.GetWorldPose());
			}
			int count = base.interactorsSelecting.Count;
			base.OnSelectEntering(args);
			int count2 = base.interactorsSelecting.Count;
			this.m_GrabCountChanged = true;
			this.m_GrabCountBeforeAndAfterChange = new ValueTuple<int, int>(count, count2);
			this.m_CurrentAttachEaseTime = 0f;
			this.ResetThrowSmoothing();
			if (!this.m_IgnoringCharacterCollision)
			{
				this.m_SelectingCharacterController = args.interactorObject.transform.GetComponentInParent<CharacterController>();
				if (this.m_SelectingCharacterController != null)
				{
					this.m_SelectingCharacterInteractors.Add(args.interactorObject);
					this.StartIgnoringCharacterCollision(this.m_SelectingCharacterController);
				}
			}
			else if (this.m_SelectingCharacterController != null && args.interactorObject.transform.IsChildOf(this.m_SelectingCharacterController.transform))
			{
				this.m_SelectingCharacterInteractors.Add(args.interactorObject);
			}
			if (base.interactorsSelecting.Count == 1)
			{
				this.Grab();
				if (!this.AddDefaultGrabTransformers())
				{
					this.InvokeGrabTransformersOnGrab();
				}
			}
			else
			{
				this.UpdateCurrentMovementType();
			}
			this.SubscribeTeleportationProvider(args.interactorObject);
		}

		protected override void OnSelectExiting(SelectExitEventArgs args)
		{
			int count = base.interactorsSelecting.Count;
			base.OnSelectExiting(args);
			int count2 = base.interactorsSelecting.Count;
			this.m_GrabCountChanged = true;
			this.m_GrabCountBeforeAndAfterChange = new ValueTuple<int, int>(count, count2);
			this.m_CurrentAttachEaseTime = 0f;
			if (base.interactorsSelecting.Count == 0)
			{
				if (this.m_ThrowOnDetach)
				{
					this.m_ThrowAssist = args.interactorObject.transform.GetComponentInParent<IXRAimAssist>();
				}
				this.Drop();
				if (this.m_DropTransformersCount <= 0)
				{
					goto IL_A9;
				}
				DropEventArgs dropEventArgs;
				using (XRGrabInteractable.s_DropEventArgs.Get(out dropEventArgs))
				{
					dropEventArgs.selectExitEventArgs = args;
					this.InvokeGrabTransformersOnDrop(dropEventArgs);
					goto IL_A9;
				}
			}
			this.UpdateCurrentMovementType();
			IL_A9:
			this.m_SelectingCharacterInteractors.Remove(args.interactorObject);
			this.UnsubscribeTeleportationProvider(args.interactorObject);
		}

		protected override void OnSelectExited(SelectExitEventArgs args)
		{
			base.OnSelectExited(args);
			this.ReleaseDynamicAttachTransform(args.interactorObject);
		}

		private Transform CreateDynamicAttachTransform(IXRSelectInteractor interactor)
		{
			Transform transform;
			do
			{
				transform = XRGrabInteractable.s_DynamicAttachTransformPool.Get();
			}
			while (transform == null);
			transform.SetParent(this.m_Transform, false);
			return transform;
		}

		private Transform CreateVisualAttachTransform(IXRSelectInteractor interactor)
		{
			Transform transform;
			do
			{
				transform = XRGrabInteractable.s_DynamicAttachTransformPool.Get();
			}
			while (transform == null);
			transform.SetParent(this.m_PredictedVisualsTransform, false);
			return transform;
		}

		private void InitializeDynamicAttachPoseInternal(IXRSelectInteractor interactor, Transform dynamicAttachTransform)
		{
			this.InitializeDynamicAttachPoseWithStatic(interactor, dynamicAttachTransform);
			this.InitializeDynamicAttachPose(interactor, dynamicAttachTransform);
		}

		private void InitializeDynamicAttachPoseWithStatic(IXRSelectInteractor interactor, Transform dynamicAttachTransform)
		{
			this.m_DynamicAttachTransforms.Remove(interactor);
			Transform attachTransform = this.GetAttachTransform(interactor);
			this.m_DynamicAttachTransforms[interactor] = dynamicAttachTransform;
			if (attachTransform == this.m_Transform)
			{
				dynamicAttachTransform.SetLocalPose(Pose.identity);
				return;
			}
			if (attachTransform.parent == this.m_Transform)
			{
				dynamicAttachTransform.SetLocalPose(attachTransform.GetLocalPose());
				return;
			}
			dynamicAttachTransform.SetWorldPose(attachTransform.GetWorldPose());
		}

		private void ReleaseDynamicAttachTransform(IXRSelectInteractor interactor)
		{
			XRGrabInteractable.<ReleaseDynamicAttachTransform>g__Release|303_0(this.m_DynamicAttachTransforms, interactor);
			XRGrabInteractable.<ReleaseDynamicAttachTransform>g__Release|303_0(this.m_VisualAttachTransforms, interactor);
		}

		protected virtual bool ShouldMatchAttachPosition(IXRSelectInteractor interactor)
		{
			if (!this.m_MatchAttachPosition)
			{
				return false;
			}
			if (!(interactor is XRSocketInteractor))
			{
				XRRayInteractor xrrayInteractor = interactor as XRRayInteractor;
				if (xrrayInteractor == null || !xrrayInteractor.useForceGrab)
				{
					return true;
				}
			}
			return false;
		}

		protected virtual bool ShouldMatchAttachRotation(IXRSelectInteractor interactor)
		{
			return this.m_MatchAttachRotation && !(interactor is XRSocketInteractor);
		}

		protected virtual bool ShouldSnapToColliderVolume(IXRSelectInteractor interactor)
		{
			return this.m_SnapToColliderVolume;
		}

		protected virtual void InitializeDynamicAttachPose(IXRSelectInteractor interactor, Transform dynamicAttachTransform)
		{
			bool flag = this.ShouldMatchAttachPosition(interactor);
			bool flag2 = this.ShouldMatchAttachRotation(interactor);
			if (!flag && !flag2)
			{
				return;
			}
			Pose worldPose = interactor.GetAttachTransform(this).GetWorldPose();
			DistanceInfo distanceInfo;
			if (flag && this.ShouldSnapToColliderVolume(interactor) && XRInteractableUtility.TryGetClosestPointOnCollider(this, worldPose.position, out distanceInfo))
			{
				worldPose.position = distanceInfo.point;
			}
			if (flag && flag2)
			{
				dynamicAttachTransform.SetWorldPose(worldPose);
				return;
			}
			if (flag)
			{
				dynamicAttachTransform.position = worldPose.position;
				return;
			}
			dynamicAttachTransform.rotation = worldPose.rotation;
		}

		protected virtual void Grab()
		{
			this.m_OriginalSceneParent = this.m_Transform.parent;
			this.m_Transform.SetParent(null);
			if (this.m_PredictedVisualsTransform != null)
			{
				this.m_InitialVisualsTransformLocalPose = this.m_PredictedVisualsTransform.GetLocalPose();
				this.m_InitialVisualsTransformLocalPoseIsIdentity = (this.m_InitialVisualsTransformLocalPose == Pose.identity);
				this.m_InitialVisualsTransformLocalScale = this.m_PredictedVisualsTransform.localScale;
			}
			else
			{
				this.m_InitialVisualsTransformLocalPose = Pose.identity;
				this.m_InitialVisualsTransformLocalPoseIsIdentity = true;
				this.m_InitialVisualsTransformLocalScale = Vector3.one;
			}
			this.UpdateCurrentMovementType();
			this.SetupRigidbodyGrab(this.m_Rigidbody);
			this.m_DetachLinearVelocity = Vector3.zero;
			this.m_DetachAngularVelocity = Vector3.zero;
			this.InitializeTargetPoseAndScale(this.m_Transform);
		}

		protected virtual void Drop()
		{
			if (!false && this.m_RetainTransformParent && this.m_OriginalSceneParent != null)
			{
				if (!this.m_OriginalSceneParent.gameObject.activeInHierarchy)
				{
					Debug.LogWarning("Retain Transform Parent is set to true, and has a non-null Original Scene Parent. However, the old parent is deactivated so we are choosing not to re-parent upon dropping.", this);
				}
				else if (base.gameObject.activeInHierarchy)
				{
					this.m_Transform.SetParent(this.m_OriginalSceneParent);
				}
			}
			this.SetupRigidbodyDrop(this.m_Rigidbody);
			this.m_CurrentMovementType = this.m_MovementType;
			this.m_DetachInLateUpdate = true;
			this.EndThrowSmoothing();
			if (this.m_PredictedVisualsTransform != null)
			{
				this.m_PredictedVisualsTransform.SetLocalPose(this.m_InitialVisualsTransformLocalPose);
				this.m_PredictedVisualsTransform.localScale = this.m_InitialVisualsTransformLocalScale;
			}
		}

		protected virtual void Detach()
		{
			if (this.m_ThrowOnDetach)
			{
				if (this.m_Rigidbody.isKinematic)
				{
					Debug.LogWarning("Cannot throw a kinematic Rigidbody since updating the velocity and angular velocity of a kinematic Rigidbody is not supported. Disable Throw On Detach or Is Kinematic to fix this issue.", this);
					return;
				}
				if (this.m_ThrowAssist != null)
				{
					IXRAimAssist throwAssist = this.m_ThrowAssist;
					Vector3 position = this.m_Rigidbody.position;
					this.m_DetachLinearVelocity = throwAssist.GetAssistedVelocity(position, this.m_DetachLinearVelocity, this.m_Rigidbody.useGravity ? (-Physics.gravity.y) : 0f);
					this.m_ThrowAssist = null;
				}
				else if (this.m_LimitLinearVelocity)
				{
					this.m_DetachLinearVelocity = Vector3.ClampMagnitude(this.m_DetachLinearVelocity, this.m_MaxLinearVelocityDelta);
				}
				if (this.m_LimitAngularVelocity)
				{
					this.m_DetachAngularVelocity = Vector3.ClampMagnitude(this.m_DetachAngularVelocity, this.m_MaxAngularVelocityDelta);
				}
				this.m_Rigidbody.linearVelocity = this.m_DetachLinearVelocity;
				this.m_Rigidbody.angularVelocity = this.m_DetachAngularVelocity;
			}
		}

		protected virtual void SetupRigidbodyGrab(Rigidbody rigidbody)
		{
			this.m_WasKinematic = rigidbody.isKinematic;
			this.m_UsedGravity = rigidbody.useGravity;
			this.m_InterpolationOnGrab = rigidbody.interpolation;
			this.m_LinearDampingOnGrab = rigidbody.linearDamping;
			this.m_AngularDampingOnGrab = rigidbody.angularDamping;
			rigidbody.isKinematic = (this.m_CurrentMovementType == XRBaseInteractable.MovementType.Kinematic || this.m_CurrentMovementType == XRBaseInteractable.MovementType.Instantaneous);
			rigidbody.useGravity = false;
			if (this.isRigidbodyMovement && this.m_PredictedVisualsTransform != null)
			{
				rigidbody.interpolation = RigidbodyInterpolation.None;
			}
			rigidbody.linearDamping = 0f;
			rigidbody.angularDamping = 0f;
		}

		protected virtual void SetupRigidbodyDrop(Rigidbody rigidbody)
		{
			rigidbody.isKinematic = this.m_WasKinematic;
			rigidbody.useGravity = this.m_UsedGravity;
			if (this.m_PredictedVisualsTransform != null)
			{
				rigidbody.interpolation = this.m_InterpolationOnGrab;
			}
			rigidbody.linearDamping = this.m_LinearDampingOnGrab;
			rigidbody.angularDamping = this.m_AngularDampingOnGrab;
			if (!base.isSelected)
			{
				this.m_Rigidbody.useGravity |= this.m_ForceGravityOnDetach;
			}
		}

		private void ResetThrowSmoothing()
		{
			Array.Clear(this.m_ThrowSmoothingFrameTimes, 0, this.m_ThrowSmoothingFrameTimes.Length);
			Array.Clear(this.m_ThrowSmoothingLinearVelocityFrames, 0, this.m_ThrowSmoothingLinearVelocityFrames.Length);
			Array.Clear(this.m_ThrowSmoothingAngularVelocityFrames, 0, this.m_ThrowSmoothingAngularVelocityFrames.Length);
			this.m_ThrowSmoothingCurrentFrame = 0;
			this.m_ThrowSmoothingFirstUpdate = true;
		}

		private void EndThrowSmoothing()
		{
			if (this.m_ThrowOnDetach)
			{
				Vector3 smoothedVelocityValue = this.GetSmoothedVelocityValue(this.m_ThrowSmoothingLinearVelocityFrames);
				Vector3 smoothedVelocityValue2 = this.GetSmoothedVelocityValue(this.m_ThrowSmoothingAngularVelocityFrames);
				this.m_DetachLinearVelocity = smoothedVelocityValue * this.m_ThrowVelocityScale;
				this.m_DetachAngularVelocity = smoothedVelocityValue2 * this.m_ThrowAngularVelocityScale;
			}
		}

		private void StepThrowSmoothing(Pose targetPose, float deltaTime)
		{
			if (deltaTime < 0.001f)
			{
				return;
			}
			if (this.m_ThrowSmoothingFirstUpdate)
			{
				this.m_ThrowSmoothingFirstUpdate = false;
			}
			else
			{
				this.m_ThrowSmoothingLinearVelocityFrames[this.m_ThrowSmoothingCurrentFrame] = (targetPose.position - this.m_LastThrowReferencePose.position) / deltaTime;
				Vector3 eulerAngles = (targetPose.rotation * Quaternion.Inverse(this.m_LastThrowReferencePose.rotation)).eulerAngles;
				Vector3 a = new Vector3(Mathf.DeltaAngle(0f, eulerAngles.x), Mathf.DeltaAngle(0f, eulerAngles.y), Mathf.DeltaAngle(0f, eulerAngles.z));
				this.m_ThrowSmoothingAngularVelocityFrames[this.m_ThrowSmoothingCurrentFrame] = a / deltaTime * 0.017453292f;
			}
			this.m_ThrowSmoothingFrameTimes[this.m_ThrowSmoothingCurrentFrame] = Time.time;
			this.m_ThrowSmoothingCurrentFrame = (this.m_ThrowSmoothingCurrentFrame + 1) % 20;
			this.m_LastThrowReferencePose = targetPose;
		}

		private Vector3 GetSmoothedVelocityValue(Vector3[] velocityFrames)
		{
			Vector3 a = Vector3.zero;
			float num = 0f;
			for (int i = 0; i < 20; i++)
			{
				int num2 = ((this.m_ThrowSmoothingCurrentFrame - i - 1) % 20 + 20) % 20;
				if (this.m_ThrowSmoothingFrameTimes[num2] == 0f)
				{
					break;
				}
				float num3 = (Time.time - this.m_ThrowSmoothingFrameTimes[num2]) / this.m_ThrowSmoothingDuration;
				float num4 = this.m_ThrowSmoothingCurve.Evaluate(Mathf.Clamp(1f - num3, 0f, 1f));
				a += velocityFrames[num2] * num4;
				num += num4;
				if (Time.time - this.m_ThrowSmoothingFrameTimes[num2] > this.m_ThrowSmoothingDuration)
				{
					break;
				}
			}
			if (num > 0f)
			{
				return a / num;
			}
			return Vector3.zero;
		}

		private void SubscribeTeleportationProvider(IXRInteractor interactor)
		{
			this.m_TeleportationMonitor.AddInteractor(interactor);
		}

		private void UnsubscribeTeleportationProvider(IXRInteractor interactor)
		{
			this.m_TeleportationMonitor.RemoveInteractor(interactor);
		}

		private void OnTeleported(Pose beforePose, Pose afterPose, Pose deltaPose)
		{
			Quaternion rotation = deltaPose.rotation;
			int num = 0;
			while (num < 20 && this.m_ThrowSmoothingFrameTimes[num] != 0f)
			{
				this.m_ThrowSmoothingLinearVelocityFrames[num] = rotation * this.m_ThrowSmoothingLinearVelocityFrames[num];
				num++;
			}
			Vector3 point = this.m_LastThrowReferencePose.position - beforePose.position;
			Vector3 b = rotation * point;
			this.m_LastThrowReferencePose.position = afterPose.position + b;
			this.m_LastThrowReferencePose.rotation = rotation * this.m_LastThrowReferencePose.rotation;
		}

		private void StartIgnoringCharacterCollision(Collider characterCollider)
		{
			this.m_IgnoringCharacterCollision = true;
			this.m_CollidersThatAllowedCharacterCollision.Clear();
			for (int i = 0; i < this.m_RigidbodyColliders.Count; i++)
			{
				Collider collider = this.m_RigidbodyColliders[i];
				if (!(collider == null) && !collider.isTrigger && !Physics.GetIgnoreCollision(collider, characterCollider))
				{
					this.m_CollidersThatAllowedCharacterCollision.Add(collider);
					Physics.IgnoreCollision(collider, characterCollider, true);
				}
			}
		}

		private bool IsOutsideCharacterCollider(Collider characterCollider)
		{
			Bounds bounds = characterCollider.bounds;
			foreach (Collider collider in this.m_CollidersThatAllowedCharacterCollision)
			{
				if (!(collider == null) && collider.bounds.Intersects(bounds))
				{
					return false;
				}
			}
			return true;
		}

		private void StopIgnoringCharacterCollision(Collider characterCollider)
		{
			this.m_IgnoringCharacterCollision = false;
			foreach (Collider collider in this.m_CollidersThatAllowedCharacterCollision)
			{
				if (collider != null)
				{
					Physics.IgnoreCollision(collider, characterCollider, false);
				}
			}
		}

		private static Transform OnCreatePooledItem()
		{
			Transform transform = new GameObject().transform;
			transform.SetLocalPose(Pose.identity);
			transform.localScale = Vector3.one;
			return transform;
		}

		private static void OnGetPooledItem(Transform item)
		{
			if (item == null)
			{
				return;
			}
			item.hideFlags &= ~HideFlags.HideInHierarchy;
		}

		private static void OnReleasePooledItem(Transform item)
		{
			if (item == null)
			{
				return;
			}
			item.hideFlags |= HideFlags.HideInHierarchy;
		}

		private static void OnDestroyPooledItem(Transform item)
		{
			if (item == null)
			{
				return;
			}
			Object.Destroy(item.gameObject);
		}

		[Obsolete("attachPointCompatibilityMode has been deprecated and will be removed in a future version of XRI.", true)]
		public XRGrabInteractable.AttachPointCompatibilityMode attachPointCompatibilityMode
		{
			get
			{
				Debug.LogError("attachPointCompatibilityMode has been deprecated and will be removed in a future version of XRI.", this);
				throw new NotSupportedException("attachPointCompatibilityMode has been deprecated and will be removed in a future version of XRI.");
			}
			set
			{
				Debug.LogError("attachPointCompatibilityMode has been deprecated and will be removed in a future version of XRI.", this);
				throw new NotSupportedException("attachPointCompatibilityMode has been deprecated and will be removed in a future version of XRI.");
			}
		}

		[Obsolete("gravityOnDetach has been deprecated. Use forceGravityOnDetach instead. (UnityUpgradable) -> forceGravityOnDetach", true)]
		public bool gravityOnDetach
		{
			get
			{
				Debug.LogError("gravityOnDetach has been deprecated. Use forceGravityOnDetach instead. (UnityUpgradable) -> forceGravityOnDetach", this);
				throw new NotSupportedException("gravityOnDetach has been deprecated. Use forceGravityOnDetach instead. (UnityUpgradable) -> forceGravityOnDetach");
			}
			set
			{
				Debug.LogError("gravityOnDetach has been deprecated. Use forceGravityOnDetach instead. (UnityUpgradable) -> forceGravityOnDetach", this);
				throw new NotSupportedException("gravityOnDetach has been deprecated. Use forceGravityOnDetach instead. (UnityUpgradable) -> forceGravityOnDetach");
			}
		}

		[CompilerGenerated]
		internal static void <ReleaseDynamicAttachTransform>g__Release|303_0(Dictionary<IXRSelectInteractor, Transform> transforms, IXRSelectInteractor interactor)
		{
			Transform transform;
			if (transforms.Count > 0 && transforms.TryGetValue(interactor, out transform))
			{
				if (transform != null)
				{
					XRGrabInteractable.s_DynamicAttachTransformPool.Release(transform);
				}
				transforms.Remove(interactor);
			}
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void EaseAttachBurst$BurstManaged(ref Pose targetPose, ref Vector3 targetLocalScale, in Pose rawTargetPose, in Vector3 rawTargetLocalScale, float deltaTime, float attachEaseInTime, ref float currentAttachEaseTime)
		{
			float t = currentAttachEaseTime / attachEaseInTime;
			targetPose.position = math.lerp(targetPose.position, rawTargetPose.position, t);
			targetPose.rotation = math.slerp(targetPose.rotation, rawTargetPose.rotation, t);
			targetLocalScale = math.lerp(targetLocalScale, rawTargetLocalScale, t);
			currentAttachEaseTime += deltaTime;
		}

		[BurstCompile]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void StepSmoothingBurst$BurstManaged(ref Pose targetPose, ref Vector3 targetLocalScale, in Pose rawTargetPose, in Vector3 rawTargetLocalScale, float deltaTime, bool smoothPos, float smoothPosAmount, float tightenPos, bool smoothRot, float smoothRotAmount, float tightenRot, bool smoothScale, float smoothScaleAmount, float tightenScale)
		{
			if (smoothPos)
			{
				targetPose.position = math.lerp(targetPose.position, rawTargetPose.position, smoothPosAmount * deltaTime);
				targetPose.position = math.lerp(targetPose.position, rawTargetPose.position, tightenPos);
			}
			else
			{
				targetPose.position = rawTargetPose.position;
			}
			if (smoothRot)
			{
				targetPose.rotation = math.slerp(targetPose.rotation, rawTargetPose.rotation, smoothRotAmount * deltaTime);
				targetPose.rotation = math.slerp(targetPose.rotation, rawTargetPose.rotation, tightenRot);
			}
			else
			{
				targetPose.rotation = rawTargetPose.rotation;
			}
			if (smoothScale)
			{
				targetLocalScale = math.lerp(targetLocalScale, rawTargetLocalScale, smoothScaleAmount * deltaTime);
				targetLocalScale = math.lerp(targetLocalScale, rawTargetLocalScale, tightenScale);
				return;
			}
			targetLocalScale = rawTargetLocalScale;
		}

		private const float k_DefaultTighteningAmount = 0.1f;

		private const float k_DefaultSmoothingAmount = 8f;

		private const float k_LinearVelocityDamping = 1f;

		private const float k_LinearVelocityScale = 1f;

		private const float k_AngularVelocityDamping = 1f;

		private const float k_AngularVelocityScale = 1f;

		private const int k_ThrowSmoothingFrameCount = 20;

		private const float k_DefaultAttachEaseInTime = 0.15f;

		private const float k_DefaultThrowSmoothingDuration = 0.25f;

		private const float k_DefaultThrowLinearVelocityScale = 1.5f;

		private const float k_DefaultThrowAngularVelocityScale = 1f;

		private const float k_DeltaTimeThreshold = 0.001f;

		private const float k_DefaultMaxLinearVelocityDelta = 10f;

		private const float k_DefaultMaxAngularVelocityDelta = 20f;

		[SerializeField]
		private Transform m_AttachTransform;

		[SerializeField]
		private Transform m_SecondaryAttachTransform;

		[SerializeField]
		private bool m_UseDynamicAttach;

		[SerializeField]
		private bool m_MatchAttachPosition = true;

		[SerializeField]
		private bool m_MatchAttachRotation = true;

		[SerializeField]
		private bool m_SnapToColliderVolume = true;

		[SerializeField]
		private bool m_ReinitializeDynamicAttachEverySingleGrab = true;

		[SerializeField]
		private float m_AttachEaseInTime = 0.15f;

		[SerializeField]
		private XRBaseInteractable.MovementType m_MovementType = XRBaseInteractable.MovementType.Instantaneous;

		[SerializeField]
		[FormerlySerializedAs("m_VisualsTransform")]
		private Transform m_PredictedVisualsTransform;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_VelocityDamping = 1f;

		[SerializeField]
		private float m_VelocityScale = 1f;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_AngularVelocityDamping = 1f;

		[SerializeField]
		private float m_AngularVelocityScale = 1f;

		[SerializeField]
		private bool m_TrackPosition = true;

		[SerializeField]
		private bool m_SmoothPosition;

		[SerializeField]
		[Range(0f, 20f)]
		private float m_SmoothPositionAmount = 8f;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_TightenPosition = 0.1f;

		[SerializeField]
		private bool m_TrackRotation = true;

		[SerializeField]
		private bool m_SmoothRotation;

		[SerializeField]
		[Range(0f, 20f)]
		private float m_SmoothRotationAmount = 8f;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_TightenRotation = 0.1f;

		[SerializeField]
		private bool m_TrackScale = true;

		[SerializeField]
		private bool m_SmoothScale;

		[SerializeField]
		[Range(0f, 20f)]
		private float m_SmoothScaleAmount = 8f;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_TightenScale = 0.1f;

		[SerializeField]
		private bool m_ThrowOnDetach = true;

		[SerializeField]
		private float m_ThrowSmoothingDuration = 0.25f;

		[SerializeField]
		private AnimationCurve m_ThrowSmoothingCurve = AnimationCurve.Linear(1f, 1f, 1f, 0f);

		[SerializeField]
		private float m_ThrowVelocityScale = 1.5f;

		[SerializeField]
		private float m_ThrowAngularVelocityScale = 1f;

		[SerializeField]
		[FormerlySerializedAs("m_GravityOnDetach")]
		private bool m_ForceGravityOnDetach;

		[SerializeField]
		private bool m_RetainTransformParent = true;

		[SerializeField]
		private List<XRBaseGrabTransformer> m_StartingSingleGrabTransformers = new List<XRBaseGrabTransformer>();

		[SerializeField]
		private List<XRBaseGrabTransformer> m_StartingMultipleGrabTransformers = new List<XRBaseGrabTransformer>();

		[SerializeField]
		private bool m_AddDefaultGrabTransformers = true;

		[SerializeField]
		private InteractableFarAttachMode m_FarAttachMode;

		[SerializeField]
		private bool m_LimitLinearVelocity;

		[SerializeField]
		private bool m_LimitAngularVelocity;

		[SerializeField]
		private float m_MaxLinearVelocityDelta = 10f;

		[SerializeField]
		private float m_MaxAngularVelocityDelta = 20f;

		private readonly SmallRegistrationList<IXRGrabTransformer> m_SingleGrabTransformers = new SmallRegistrationList<IXRGrabTransformer>();

		private readonly SmallRegistrationList<IXRGrabTransformer> m_MultipleGrabTransformers = new SmallRegistrationList<IXRGrabTransformer>();

		private List<IXRGrabTransformer> m_GrabTransformersAddedWhenGrabbed;

		private bool m_GrabCountChanged;

		private ValueTuple<int, int> m_GrabCountBeforeAndAfterChange;

		private bool m_IsProcessingGrabTransformers;

		private int m_DropTransformersCount;

		private static readonly UnityEngine.XR.Interaction.Toolkit.Utilities.Pooling.LinkedPool<DropEventArgs> s_DropEventArgs = new UnityEngine.XR.Interaction.Toolkit.Utilities.Pooling.LinkedPool<DropEventArgs>(() => new DropEventArgs(), null, null, null, false, 10000);

		private Pose m_TargetPose;

		private Vector3 m_TargetLocalScale;

		private bool m_IsTargetPoseDirty;

		private bool m_IsTargetLocalScaleDirty;

		private Transform m_Transform;

		private float m_CurrentAttachEaseTime;

		private XRBaseInteractable.MovementType m_CurrentMovementType;

		private bool m_DetachInLateUpdate;

		private Vector3 m_DetachLinearVelocity;

		private Vector3 m_DetachAngularVelocity;

		private int m_ThrowSmoothingCurrentFrame;

		private readonly float[] m_ThrowSmoothingFrameTimes = new float[20];

		private readonly Vector3[] m_ThrowSmoothingLinearVelocityFrames = new Vector3[20];

		private readonly Vector3[] m_ThrowSmoothingAngularVelocityFrames = new Vector3[20];

		private bool m_ThrowSmoothingFirstUpdate;

		private Pose m_LastThrowReferencePose;

		private IXRAimAssist m_ThrowAssist;

		private Rigidbody m_Rigidbody;

		private bool m_RigidbodyColliding;

		private bool m_WasKinematic;

		private bool m_UsedGravity;

		private RigidbodyInterpolation m_InterpolationOnGrab;

		private float m_LinearDampingOnGrab;

		private float m_AngularDampingOnGrab;

		private int m_LastFixedFrame;

		private float m_LastFixedDynamicTime;

		private Pose m_InitialVisualsTransformLocalPose;

		private bool m_InitialVisualsTransformLocalPoseIsIdentity = true;

		private Vector3 m_InitialVisualsTransformLocalScale;

		private bool m_IgnoringCharacterCollision;

		private bool m_StopIgnoringCollisionInLateUpdate;

		private CharacterController m_SelectingCharacterController;

		private readonly HashSet<IXRSelectInteractor> m_SelectingCharacterInteractors = new HashSet<IXRSelectInteractor>();

		private readonly List<Collider> m_RigidbodyColliders = new List<Collider>();

		private readonly HashSet<Collider> m_CollidersThatAllowedCharacterCollision = new HashSet<Collider>();

		private Transform m_OriginalSceneParent;

		private TeleportationMonitor m_TeleportationMonitor;

		private readonly Dictionary<IXRSelectInteractor, Transform> m_DynamicAttachTransforms = new Dictionary<IXRSelectInteractor, Transform>();

		private readonly Dictionary<IXRSelectInteractor, Transform> m_VisualAttachTransforms = new Dictionary<IXRSelectInteractor, Transform>();

		private static readonly UnityEngine.XR.Interaction.Toolkit.Utilities.Pooling.LinkedPool<Transform> s_DynamicAttachTransformPool = new UnityEngine.XR.Interaction.Toolkit.Utilities.Pooling.LinkedPool<Transform>(new Func<Transform>(XRGrabInteractable.OnCreatePooledItem), new Action<Transform>(XRGrabInteractable.OnGetPooledItem), new Action<Transform>(XRGrabInteractable.OnReleasePooledItem), new Action<Transform>(XRGrabInteractable.OnDestroyPooledItem), true, 10000);

		private static readonly ProfilerMarker s_ProcessGrabTransformersMarker = new ProfilerMarker("XRI.ProcessGrabTransformers");

		private const string k_AttachPointCompatibilityModeDeprecated = "attachPointCompatibilityMode has been deprecated and will be removed in a future version of XRI.";

		private const string k_GravityOnDetachDeprecated = "gravityOnDetach has been deprecated. Use forceGravityOnDetach instead. (UnityUpgradable) -> forceGravityOnDetach";

		[Obsolete("AttachPointCompatibilityMode has been deprecated and will be removed in a future version of XRI.", true)]
		public enum AttachPointCompatibilityMode
		{
			[Obsolete("Default has been deprecated and will be removed in a future version of XRI. It is the only mode now.", true)]
			Default,
			[Obsolete("Legacy has been deprecated and will be removed in a future version of XRI.", true)]
			Legacy
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void EaseAttachBurst_00000F9A$PostfixBurstDelegate(ref Pose targetPose, ref Vector3 targetLocalScale, in Pose rawTargetPose, in Vector3 rawTargetLocalScale, float deltaTime, float attachEaseInTime, ref float currentAttachEaseTime);

		internal static class EaseAttachBurst_00000F9A$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (XRGrabInteractable.EaseAttachBurst_00000F9A$BurstDirectCall.Pointer == 0)
				{
					XRGrabInteractable.EaseAttachBurst_00000F9A$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<XRGrabInteractable.EaseAttachBurst_00000F9A$PostfixBurstDelegate>(new XRGrabInteractable.EaseAttachBurst_00000F9A$PostfixBurstDelegate(XRGrabInteractable.EaseAttachBurst)).Value;
				}
				A_0 = XRGrabInteractable.EaseAttachBurst_00000F9A$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				XRGrabInteractable.EaseAttachBurst_00000F9A$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(ref Pose targetPose, ref Vector3 targetLocalScale, in Pose rawTargetPose, in Vector3 rawTargetLocalScale, float deltaTime, float attachEaseInTime, ref float currentAttachEaseTime)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = XRGrabInteractable.EaseAttachBurst_00000F9A$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(UnityEngine.Pose&,UnityEngine.Vector3&,UnityEngine.Pose&,UnityEngine.Vector3&,System.Single,System.Single,System.Single&), ref targetPose, ref targetLocalScale, ref rawTargetPose, ref rawTargetLocalScale, deltaTime, attachEaseInTime, ref currentAttachEaseTime, functionPointer);
						return;
					}
				}
				XRGrabInteractable.EaseAttachBurst$BurstManaged(ref targetPose, ref targetLocalScale, rawTargetPose, rawTargetLocalScale, deltaTime, attachEaseInTime, ref currentAttachEaseTime);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void StepSmoothingBurst_00000F9B$PostfixBurstDelegate(ref Pose targetPose, ref Vector3 targetLocalScale, in Pose rawTargetPose, in Vector3 rawTargetLocalScale, float deltaTime, bool smoothPos, float smoothPosAmount, float tightenPos, bool smoothRot, float smoothRotAmount, float tightenRot, bool smoothScale, float smoothScaleAmount, float tightenScale);

		internal static class StepSmoothingBurst_00000F9B$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (XRGrabInteractable.StepSmoothingBurst_00000F9B$BurstDirectCall.Pointer == 0)
				{
					XRGrabInteractable.StepSmoothingBurst_00000F9B$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<XRGrabInteractable.StepSmoothingBurst_00000F9B$PostfixBurstDelegate>(new XRGrabInteractable.StepSmoothingBurst_00000F9B$PostfixBurstDelegate(XRGrabInteractable.StepSmoothingBurst)).Value;
				}
				A_0 = XRGrabInteractable.StepSmoothingBurst_00000F9B$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				XRGrabInteractable.StepSmoothingBurst_00000F9B$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(ref Pose targetPose, ref Vector3 targetLocalScale, in Pose rawTargetPose, in Vector3 rawTargetLocalScale, float deltaTime, bool smoothPos, float smoothPosAmount, float tightenPos, bool smoothRot, float smoothRotAmount, float tightenRot, bool smoothScale, float smoothScaleAmount, float tightenScale)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = XRGrabInteractable.StepSmoothingBurst_00000F9B$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(UnityEngine.Pose&,UnityEngine.Vector3&,UnityEngine.Pose&,UnityEngine.Vector3&,System.Single,System.Boolean,System.Single,System.Single,System.Boolean,System.Single,System.Single,System.Boolean,System.Single,System.Single), ref targetPose, ref targetLocalScale, ref rawTargetPose, ref rawTargetLocalScale, deltaTime, smoothPos, smoothPosAmount, tightenPos, smoothRot, smoothRotAmount, tightenRot, smoothScale, smoothScaleAmount, tightenScale, functionPointer);
						return;
					}
				}
				XRGrabInteractable.StepSmoothingBurst$BurstManaged(ref targetPose, ref targetLocalScale, rawTargetPose, rawTargetLocalScale, deltaTime, smoothPos, smoothPosAmount, tightenPos, smoothRot, smoothRotAmount, tightenRot, smoothScale, smoothScaleAmount, tightenScale);
			}

			private static IntPtr Pointer;
		}
	}
}
