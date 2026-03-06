using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem
{
	public class Hand : MonoBehaviour
	{
		public ReadOnlyCollection<Hand.AttachedObject> AttachedObjects
		{
			get
			{
				return this.attachedObjects.AsReadOnly();
			}
		}

		public bool hoverLocked { get; private set; }

		public bool isActive
		{
			get
			{
				if (this.trackedObject != null)
				{
					return this.trackedObject.isActive;
				}
				return base.gameObject.activeInHierarchy;
			}
		}

		public bool isPoseValid
		{
			get
			{
				return this.trackedObject.isValid;
			}
		}

		public Interactable hoveringInteractable
		{
			get
			{
				return this._hoveringInteractable;
			}
			set
			{
				if (this._hoveringInteractable != value)
				{
					if (this._hoveringInteractable != null)
					{
						if (this.spewDebugText)
						{
							string str = "HoverEnd ";
							GameObject gameObject = this._hoveringInteractable.gameObject;
							this.HandDebugLog(str + ((gameObject != null) ? gameObject.ToString() : null));
						}
						this._hoveringInteractable.SendMessage("OnHandHoverEnd", this, SendMessageOptions.DontRequireReceiver);
						if (this._hoveringInteractable != null)
						{
							base.BroadcastMessage("OnParentHandHoverEnd", this._hoveringInteractable, SendMessageOptions.DontRequireReceiver);
						}
					}
					this._hoveringInteractable = value;
					if (this._hoveringInteractable != null)
					{
						if (this.spewDebugText)
						{
							string str2 = "HoverBegin ";
							GameObject gameObject2 = this._hoveringInteractable.gameObject;
							this.HandDebugLog(str2 + ((gameObject2 != null) ? gameObject2.ToString() : null));
						}
						this._hoveringInteractable.SendMessage("OnHandHoverBegin", this, SendMessageOptions.DontRequireReceiver);
						if (this._hoveringInteractable != null)
						{
							base.BroadcastMessage("OnParentHandHoverBegin", this._hoveringInteractable, SendMessageOptions.DontRequireReceiver);
						}
					}
				}
			}
		}

		public GameObject currentAttachedObject
		{
			get
			{
				this.CleanUpAttachedObjectStack();
				if (this.attachedObjects.Count > 0)
				{
					return this.attachedObjects[this.attachedObjects.Count - 1].attachedObject;
				}
				return null;
			}
		}

		public Hand.AttachedObject? currentAttachedObjectInfo
		{
			get
			{
				this.CleanUpAttachedObjectStack();
				if (this.attachedObjects.Count > 0)
				{
					return new Hand.AttachedObject?(this.attachedObjects[this.attachedObjects.Count - 1]);
				}
				return null;
			}
		}

		public AllowTeleportWhileAttachedToHand currentAttachedTeleportManager
		{
			get
			{
				if (this.currentAttachedObjectInfo != null)
				{
					return this.currentAttachedObjectInfo.Value.allowTeleportWhileAttachedToHand;
				}
				return null;
			}
		}

		public SteamVR_Behaviour_Skeleton skeleton
		{
			get
			{
				if (this.mainRenderModel != null)
				{
					return this.mainRenderModel.GetSkeleton();
				}
				return null;
			}
		}

		public void ShowController(bool permanent = false)
		{
			if (this.mainRenderModel != null)
			{
				this.mainRenderModel.SetControllerVisibility(true, permanent);
			}
			if (this.hoverhighlightRenderModel != null)
			{
				this.hoverhighlightRenderModel.SetControllerVisibility(true, permanent);
			}
		}

		public void HideController(bool permanent = false)
		{
			if (this.mainRenderModel != null)
			{
				this.mainRenderModel.SetControllerVisibility(false, permanent);
			}
			if (this.hoverhighlightRenderModel != null)
			{
				this.hoverhighlightRenderModel.SetControllerVisibility(false, permanent);
			}
		}

		public void ShowSkeleton(bool permanent = false)
		{
			if (this.mainRenderModel != null)
			{
				this.mainRenderModel.SetHandVisibility(true, permanent);
			}
			if (this.hoverhighlightRenderModel != null)
			{
				this.hoverhighlightRenderModel.SetHandVisibility(true, permanent);
			}
		}

		public void HideSkeleton(bool permanent = false)
		{
			if (this.mainRenderModel != null)
			{
				this.mainRenderModel.SetHandVisibility(false, permanent);
			}
			if (this.hoverhighlightRenderModel != null)
			{
				this.hoverhighlightRenderModel.SetHandVisibility(false, permanent);
			}
		}

		public bool HasSkeleton()
		{
			return this.mainRenderModel != null && this.mainRenderModel.GetSkeleton() != null;
		}

		public void Show()
		{
			this.SetVisibility(true);
		}

		public void Hide()
		{
			this.SetVisibility(false);
		}

		public void SetVisibility(bool visible)
		{
			if (this.mainRenderModel != null)
			{
				this.mainRenderModel.SetVisibility(visible, false);
			}
		}

		public void SetSkeletonRangeOfMotion(EVRSkeletalMotionRange newRangeOfMotion, float blendOverSeconds = 0.1f)
		{
			for (int i = 0; i < this.renderModels.Count; i++)
			{
				this.renderModels[i].SetSkeletonRangeOfMotion(newRangeOfMotion, blendOverSeconds);
			}
		}

		public void SetTemporarySkeletonRangeOfMotion(SkeletalMotionRangeChange temporaryRangeOfMotionChange, float blendOverSeconds = 0.1f)
		{
			for (int i = 0; i < this.renderModels.Count; i++)
			{
				this.renderModels[i].SetTemporarySkeletonRangeOfMotion(temporaryRangeOfMotionChange, blendOverSeconds);
			}
		}

		public void ResetTemporarySkeletonRangeOfMotion(float blendOverSeconds = 0.1f)
		{
			for (int i = 0; i < this.renderModels.Count; i++)
			{
				this.renderModels[i].ResetTemporarySkeletonRangeOfMotion(blendOverSeconds);
			}
		}

		public void SetAnimationState(int stateValue)
		{
			for (int i = 0; i < this.renderModels.Count; i++)
			{
				this.renderModels[i].SetAnimationState(stateValue);
			}
		}

		public void StopAnimation()
		{
			for (int i = 0; i < this.renderModels.Count; i++)
			{
				this.renderModels[i].StopAnimation();
			}
		}

		public void AttachObject(GameObject objectToAttach, GrabTypes grabbedWithType, Hand.AttachmentFlags flags = Hand.AttachmentFlags.SnapOnAttach | Hand.AttachmentFlags.DetachOthers | Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.TurnOnKinematic, Transform attachmentOffset = null)
		{
			Hand.AttachedObject attachedObject = default(Hand.AttachedObject);
			attachedObject.attachmentFlags = flags;
			attachedObject.attachedOffsetTransform = attachmentOffset;
			attachedObject.attachTime = Time.time;
			if (flags == (Hand.AttachmentFlags)0)
			{
				flags = (Hand.AttachmentFlags.SnapOnAttach | Hand.AttachmentFlags.DetachOthers | Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.TurnOnKinematic);
			}
			this.CleanUpAttachedObjectStack();
			if (this.ObjectIsAttached(objectToAttach))
			{
				this.DetachObject(objectToAttach, true);
			}
			if (attachedObject.HasAttachFlag(Hand.AttachmentFlags.DetachFromOtherHand) && this.otherHand != null)
			{
				this.otherHand.DetachObject(objectToAttach, true);
			}
			if (attachedObject.HasAttachFlag(Hand.AttachmentFlags.DetachOthers))
			{
				while (this.attachedObjects.Count > 0)
				{
					this.DetachObject(this.attachedObjects[0].attachedObject, true);
				}
			}
			if (this.currentAttachedObject)
			{
				this.currentAttachedObject.SendMessage("OnHandFocusLost", this, SendMessageOptions.DontRequireReceiver);
			}
			attachedObject.attachedObject = objectToAttach;
			attachedObject.interactable = objectToAttach.GetComponent<Interactable>();
			attachedObject.allowTeleportWhileAttachedToHand = objectToAttach.GetComponent<AllowTeleportWhileAttachedToHand>();
			attachedObject.handAttachmentPointTransform = base.transform;
			if (attachedObject.interactable != null)
			{
				if (attachedObject.interactable.attachEaseIn)
				{
					attachedObject.easeSourcePosition = attachedObject.attachedObject.transform.position;
					attachedObject.easeSourceRotation = attachedObject.attachedObject.transform.rotation;
					attachedObject.interactable.snapAttachEaseInCompleted = false;
				}
				if (attachedObject.interactable.useHandObjectAttachmentPoint)
				{
					attachedObject.handAttachmentPointTransform = this.objectAttachmentPoint;
				}
				if (attachedObject.interactable.hideHandOnAttach)
				{
					this.Hide();
				}
				if (attachedObject.interactable.hideSkeletonOnAttach && this.mainRenderModel != null && this.mainRenderModel.displayHandByDefault)
				{
					this.HideSkeleton(false);
				}
				if (attachedObject.interactable.hideControllerOnAttach && this.mainRenderModel != null && this.mainRenderModel.displayControllerByDefault)
				{
					this.HideController(false);
				}
				if (attachedObject.interactable.handAnimationOnPickup != 0)
				{
					this.SetAnimationState(attachedObject.interactable.handAnimationOnPickup);
				}
				if (attachedObject.interactable.setRangeOfMotionOnPickup != SkeletalMotionRangeChange.None)
				{
					this.SetTemporarySkeletonRangeOfMotion(attachedObject.interactable.setRangeOfMotionOnPickup, 0.1f);
				}
			}
			attachedObject.originalParent = ((objectToAttach.transform.parent != null) ? objectToAttach.transform.parent.gameObject : null);
			attachedObject.attachedRigidbody = objectToAttach.GetComponent<Rigidbody>();
			if (attachedObject.attachedRigidbody != null)
			{
				if (attachedObject.interactable.attachedToHand != null)
				{
					for (int i = 0; i < attachedObject.interactable.attachedToHand.attachedObjects.Count; i++)
					{
						Hand.AttachedObject attachedObject2 = attachedObject.interactable.attachedToHand.attachedObjects[i];
						if (attachedObject2.interactable == attachedObject.interactable)
						{
							attachedObject.attachedRigidbodyWasKinematic = attachedObject2.attachedRigidbodyWasKinematic;
							attachedObject.attachedRigidbodyUsedGravity = attachedObject2.attachedRigidbodyUsedGravity;
							attachedObject.originalParent = attachedObject2.originalParent;
						}
					}
				}
				else
				{
					attachedObject.attachedRigidbodyWasKinematic = attachedObject.attachedRigidbody.isKinematic;
					attachedObject.attachedRigidbodyUsedGravity = attachedObject.attachedRigidbody.useGravity;
				}
			}
			attachedObject.grabbedWithType = grabbedWithType;
			if (attachedObject.HasAttachFlag(Hand.AttachmentFlags.ParentToHand))
			{
				objectToAttach.transform.parent = base.transform;
				attachedObject.isParentedToHand = true;
			}
			else
			{
				attachedObject.isParentedToHand = false;
			}
			if (attachedObject.HasAttachFlag(Hand.AttachmentFlags.SnapOnAttach))
			{
				if (attachedObject.interactable != null && attachedObject.interactable.skeletonPoser != null && this.HasSkeleton())
				{
					SteamVR_Skeleton_PoseSnapshot blendedPose = attachedObject.interactable.skeletonPoser.GetBlendedPose(this.skeleton);
					objectToAttach.transform.position = base.transform.TransformPoint(blendedPose.position);
					objectToAttach.transform.rotation = base.transform.rotation * blendedPose.rotation;
					attachedObject.initialPositionalOffset = attachedObject.handAttachmentPointTransform.InverseTransformPoint(objectToAttach.transform.position);
					attachedObject.initialRotationalOffset = Quaternion.Inverse(attachedObject.handAttachmentPointTransform.rotation) * objectToAttach.transform.rotation;
				}
				else
				{
					if (attachmentOffset != null)
					{
						Quaternion rhs = Quaternion.Inverse(attachmentOffset.transform.rotation) * objectToAttach.transform.rotation;
						objectToAttach.transform.rotation = attachedObject.handAttachmentPointTransform.rotation * rhs;
						Vector3 b = objectToAttach.transform.position - attachmentOffset.transform.position;
						objectToAttach.transform.position = attachedObject.handAttachmentPointTransform.position + b;
					}
					else
					{
						objectToAttach.transform.rotation = attachedObject.handAttachmentPointTransform.rotation;
						objectToAttach.transform.position = attachedObject.handAttachmentPointTransform.position;
					}
					Transform transform = objectToAttach.transform;
					attachedObject.initialPositionalOffset = attachedObject.handAttachmentPointTransform.InverseTransformPoint(transform.position);
					attachedObject.initialRotationalOffset = Quaternion.Inverse(attachedObject.handAttachmentPointTransform.rotation) * transform.rotation;
				}
			}
			else if (attachedObject.interactable != null && attachedObject.interactable.skeletonPoser != null && this.HasSkeleton())
			{
				attachedObject.initialPositionalOffset = attachedObject.handAttachmentPointTransform.InverseTransformPoint(objectToAttach.transform.position);
				attachedObject.initialRotationalOffset = Quaternion.Inverse(attachedObject.handAttachmentPointTransform.rotation) * objectToAttach.transform.rotation;
			}
			else if (attachmentOffset != null)
			{
				Quaternion rhs2 = Quaternion.Inverse(attachmentOffset.transform.rotation) * objectToAttach.transform.rotation;
				Quaternion rotation = attachedObject.handAttachmentPointTransform.rotation * rhs2 * Quaternion.Inverse(objectToAttach.transform.rotation);
				Vector3 b2 = rotation * objectToAttach.transform.position - rotation * attachmentOffset.transform.position;
				attachedObject.initialPositionalOffset = attachedObject.handAttachmentPointTransform.InverseTransformPoint(attachedObject.handAttachmentPointTransform.position + b2);
				attachedObject.initialRotationalOffset = Quaternion.Inverse(attachedObject.handAttachmentPointTransform.rotation) * (attachedObject.handAttachmentPointTransform.rotation * rhs2);
			}
			else
			{
				attachedObject.initialPositionalOffset = attachedObject.handAttachmentPointTransform.InverseTransformPoint(objectToAttach.transform.position);
				attachedObject.initialRotationalOffset = Quaternion.Inverse(attachedObject.handAttachmentPointTransform.rotation) * objectToAttach.transform.rotation;
			}
			if (attachedObject.HasAttachFlag(Hand.AttachmentFlags.TurnOnKinematic) && attachedObject.attachedRigidbody != null)
			{
				attachedObject.collisionDetectionMode = attachedObject.attachedRigidbody.collisionDetectionMode;
				if (attachedObject.collisionDetectionMode == CollisionDetectionMode.Continuous)
				{
					attachedObject.attachedRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
				}
				attachedObject.attachedRigidbody.isKinematic = true;
			}
			if (attachedObject.HasAttachFlag(Hand.AttachmentFlags.TurnOffGravity) && attachedObject.attachedRigidbody != null)
			{
				attachedObject.attachedRigidbody.useGravity = false;
			}
			if (attachedObject.interactable != null && attachedObject.interactable.attachEaseIn)
			{
				attachedObject.attachedObject.transform.position = attachedObject.easeSourcePosition;
				attachedObject.attachedObject.transform.rotation = attachedObject.easeSourceRotation;
			}
			this.attachedObjects.Add(attachedObject);
			this.UpdateHovering();
			if (this.spewDebugText)
			{
				this.HandDebugLog("AttachObject " + ((objectToAttach != null) ? objectToAttach.ToString() : null));
			}
			objectToAttach.SendMessage("OnAttachedToHand", this, SendMessageOptions.DontRequireReceiver);
		}

		public bool ObjectIsAttached(GameObject go)
		{
			for (int i = 0; i < this.attachedObjects.Count; i++)
			{
				if (this.attachedObjects[i].attachedObject == go)
				{
					return true;
				}
			}
			return false;
		}

		public void ForceHoverUnlock()
		{
			this.hoverLocked = false;
		}

		public void DetachObject(GameObject objectToDetach, bool restoreOriginalParent = true)
		{
			int num = this.attachedObjects.FindIndex((Hand.AttachedObject l) => l.attachedObject == objectToDetach);
			if (num != -1)
			{
				if (this.spewDebugText)
				{
					string str = "DetachObject ";
					GameObject objectToDetach2 = objectToDetach;
					this.HandDebugLog(str + ((objectToDetach2 != null) ? objectToDetach2.ToString() : null));
				}
				GameObject currentAttachedObject = this.currentAttachedObject;
				if (this.attachedObjects[num].interactable != null)
				{
					if (this.attachedObjects[num].interactable.hideHandOnAttach)
					{
						this.Show();
					}
					if (this.attachedObjects[num].interactable.hideSkeletonOnAttach && this.mainRenderModel != null && this.mainRenderModel.displayHandByDefault)
					{
						this.ShowSkeleton(false);
					}
					if (this.attachedObjects[num].interactable.hideControllerOnAttach && this.mainRenderModel != null && this.mainRenderModel.displayControllerByDefault)
					{
						this.ShowController(false);
					}
					if (this.attachedObjects[num].interactable.handAnimationOnPickup != 0)
					{
						this.StopAnimation();
					}
					if (this.attachedObjects[num].interactable.setRangeOfMotionOnPickup != SkeletalMotionRangeChange.None)
					{
						this.ResetTemporarySkeletonRangeOfMotion(0.1f);
					}
				}
				Transform parent = null;
				if (this.attachedObjects[num].isParentedToHand)
				{
					if (restoreOriginalParent && this.attachedObjects[num].originalParent != null)
					{
						parent = this.attachedObjects[num].originalParent.transform;
					}
					if (this.attachedObjects[num].attachedObject != null)
					{
						this.attachedObjects[num].attachedObject.transform.parent = parent;
					}
				}
				if (this.attachedObjects[num].HasAttachFlag(Hand.AttachmentFlags.TurnOnKinematic) && this.attachedObjects[num].attachedRigidbody != null)
				{
					this.attachedObjects[num].attachedRigidbody.isKinematic = this.attachedObjects[num].attachedRigidbodyWasKinematic;
					this.attachedObjects[num].attachedRigidbody.collisionDetectionMode = this.attachedObjects[num].collisionDetectionMode;
				}
				if (this.attachedObjects[num].HasAttachFlag(Hand.AttachmentFlags.TurnOffGravity) && this.attachedObjects[num].attachedObject != null && this.attachedObjects[num].attachedRigidbody != null)
				{
					this.attachedObjects[num].attachedRigidbody.useGravity = this.attachedObjects[num].attachedRigidbodyUsedGravity;
				}
				if (this.attachedObjects[num].interactable != null && this.attachedObjects[num].interactable.handFollowTransform && this.HasSkeleton())
				{
					this.skeleton.transform.localPosition = Vector3.zero;
					this.skeleton.transform.localRotation = Quaternion.identity;
				}
				if (this.attachedObjects[num].attachedObject != null)
				{
					if (this.attachedObjects[num].interactable == null || (this.attachedObjects[num].interactable != null && !this.attachedObjects[num].interactable.isDestroying))
					{
						this.attachedObjects[num].attachedObject.SetActive(true);
					}
					this.attachedObjects[num].attachedObject.SendMessage("OnDetachedFromHand", this, SendMessageOptions.DontRequireReceiver);
				}
				this.attachedObjects.RemoveAt(num);
				this.CleanUpAttachedObjectStack();
				GameObject currentAttachedObject2 = this.currentAttachedObject;
				this.hoverLocked = false;
				if (currentAttachedObject2 != null && currentAttachedObject2 != currentAttachedObject)
				{
					currentAttachedObject2.SetActive(true);
					currentAttachedObject2.SendMessage("OnHandFocusAcquired", this, SendMessageOptions.DontRequireReceiver);
				}
			}
			this.CleanUpAttachedObjectStack();
			if (this.mainRenderModel != null)
			{
				this.mainRenderModel.MatchHandToTransform(this.mainRenderModel.transform);
			}
			if (this.hoverhighlightRenderModel != null)
			{
				this.hoverhighlightRenderModel.MatchHandToTransform(this.hoverhighlightRenderModel.transform);
			}
		}

		public Vector3 GetTrackedObjectVelocity(float timeOffset = 0f)
		{
			if (this.trackedObject == null)
			{
				Vector3 result;
				Vector3 vector;
				this.GetUpdatedAttachedVelocities(this.currentAttachedObjectInfo.Value, out result, out vector);
				return result;
			}
			if (!this.isActive)
			{
				return Vector3.zero;
			}
			if (timeOffset == 0f)
			{
				return Player.instance.trackingOriginTransform.TransformVector(this.trackedObject.GetVelocity());
			}
			Vector3 vector2;
			Vector3 vector3;
			this.trackedObject.GetVelocitiesAtTimeOffset(timeOffset, out vector2, out vector3);
			return Player.instance.trackingOriginTransform.TransformVector(vector2);
		}

		public Vector3 GetTrackedObjectAngularVelocity(float timeOffset = 0f)
		{
			if (this.trackedObject == null)
			{
				Vector3 vector;
				Vector3 result;
				this.GetUpdatedAttachedVelocities(this.currentAttachedObjectInfo.Value, out vector, out result);
				return result;
			}
			if (!this.isActive)
			{
				return Vector3.zero;
			}
			if (timeOffset == 0f)
			{
				return Player.instance.trackingOriginTransform.TransformDirection(this.trackedObject.GetAngularVelocity());
			}
			Vector3 vector2;
			Vector3 direction;
			this.trackedObject.GetVelocitiesAtTimeOffset(timeOffset, out vector2, out direction);
			return Player.instance.trackingOriginTransform.TransformDirection(direction);
		}

		public void GetEstimatedPeakVelocities(out Vector3 velocity, out Vector3 angularVelocity)
		{
			this.trackedObject.GetEstimatedPeakVelocities(out velocity, out angularVelocity);
			velocity = Player.instance.trackingOriginTransform.TransformVector(velocity);
			angularVelocity = Player.instance.trackingOriginTransform.TransformDirection(angularVelocity);
		}

		private void CleanUpAttachedObjectStack()
		{
			this.attachedObjects.RemoveAll((Hand.AttachedObject l) => l.attachedObject == null);
		}

		protected virtual void Awake()
		{
			this.inputFocusAction = SteamVR_Events.InputFocusAction(new UnityAction<bool>(this.OnInputFocus));
			if (this.hoverSphereTransform == null)
			{
				this.hoverSphereTransform = base.transform;
			}
			if (this.objectAttachmentPoint == null)
			{
				this.objectAttachmentPoint = base.transform;
			}
			this.applicationLostFocusObject = new GameObject("_application_lost_focus");
			this.applicationLostFocusObject.transform.parent = base.transform;
			this.applicationLostFocusObject.SetActive(false);
			if (this.trackedObject == null)
			{
				this.trackedObject = base.gameObject.GetComponent<SteamVR_Behaviour_Pose>();
				if (this.trackedObject != null)
				{
					SteamVR_Behaviour_Pose steamVR_Behaviour_Pose = this.trackedObject;
					steamVR_Behaviour_Pose.onTransformUpdatedEvent = (SteamVR_Behaviour_Pose.UpdateHandler)Delegate.Combine(steamVR_Behaviour_Pose.onTransformUpdatedEvent, new SteamVR_Behaviour_Pose.UpdateHandler(this.OnTransformUpdated));
				}
			}
		}

		protected virtual void OnDestroy()
		{
			if (this.trackedObject != null)
			{
				SteamVR_Behaviour_Pose steamVR_Behaviour_Pose = this.trackedObject;
				steamVR_Behaviour_Pose.onTransformUpdatedEvent = (SteamVR_Behaviour_Pose.UpdateHandler)Delegate.Remove(steamVR_Behaviour_Pose.onTransformUpdatedEvent, new SteamVR_Behaviour_Pose.UpdateHandler(this.OnTransformUpdated));
			}
		}

		protected virtual void OnTransformUpdated(SteamVR_Behaviour_Pose updatedPose, SteamVR_Input_Sources updatedSource)
		{
			this.HandFollowUpdate();
		}

		protected virtual IEnumerator Start()
		{
			this.playerInstance = Player.instance;
			if (!this.playerInstance)
			{
				Debug.LogError("<b>[SteamVR Interaction]</b> No player instance found in Hand Start()", this);
			}
			if (base.gameObject.layer == 0)
			{
				Debug.LogWarning("<b>[SteamVR Interaction]</b> Hand is on default layer. This puts unnecessary strain on hover checks as it is always true for hand colliders (which are then ignored).", this);
			}
			else
			{
				this.hoverLayerMask &= ~(1 << base.gameObject.layer);
			}
			this.overlappingColliders = new Collider[32];
			if (this.noSteamVRFallbackCamera)
			{
				yield break;
			}
			while (!this.isPoseValid)
			{
				yield return null;
			}
			this.InitController();
			yield break;
		}

		protected virtual void UpdateHovering()
		{
			if (this.noSteamVRFallbackCamera == null && !this.isActive)
			{
				return;
			}
			if (this.hoverLocked)
			{
				return;
			}
			if (this.applicationLostFocusObject.activeSelf)
			{
				return;
			}
			float maxValue = float.MaxValue;
			Interactable hoveringInteractable = null;
			if (this.useHoverSphere)
			{
				float hoverRadius = this.hoverSphereRadius * Mathf.Abs(SteamVR_Utils.GetLossyScale(this.hoverSphereTransform));
				this.CheckHoveringForTransform(this.hoverSphereTransform.position, hoverRadius, ref maxValue, ref hoveringInteractable, Color.green);
			}
			if (this.useControllerHoverComponent && this.mainRenderModel != null && this.mainRenderModel.IsControllerVisibile())
			{
				float num = this.controllerHoverRadius * Mathf.Abs(SteamVR_Utils.GetLossyScale(base.transform));
				this.CheckHoveringForTransform(this.mainRenderModel.GetControllerPosition(this.controllerHoverComponent), num / 2f, ref maxValue, ref hoveringInteractable, Color.blue);
			}
			if (this.useFingerJointHover && this.mainRenderModel != null && this.mainRenderModel.IsHandVisibile())
			{
				float num2 = this.fingerJointHoverRadius * Mathf.Abs(SteamVR_Utils.GetLossyScale(base.transform));
				this.CheckHoveringForTransform(this.mainRenderModel.GetBonePosition((int)this.fingerJointHover, false), num2 / 2f, ref maxValue, ref hoveringInteractable, Color.yellow);
			}
			this.hoveringInteractable = hoveringInteractable;
		}

		protected virtual bool CheckHoveringForTransform(Vector3 hoverPosition, float hoverRadius, ref float closestDistance, ref Interactable closestInteractable, Color debugColor)
		{
			bool flag = false;
			for (int i = 0; i < this.overlappingColliders.Length; i++)
			{
				this.overlappingColliders[i] = null;
			}
			if (Physics.OverlapSphereNonAlloc(hoverPosition, hoverRadius, this.overlappingColliders, this.hoverLayerMask.value) >= 32)
			{
				Debug.LogWarning("<b>[SteamVR Interaction]</b> This hand is overlapping the max number of colliders: " + 32.ToString() + ". Some collisions may be missed. Increase ColliderArraySize on Hand.cs");
			}
			int num = 0;
			for (int j = 0; j < this.overlappingColliders.Length; j++)
			{
				Collider collider = this.overlappingColliders[j];
				if (!(collider == null))
				{
					Interactable componentInParent = collider.GetComponentInParent<Interactable>();
					if (!(componentInParent == null))
					{
						IgnoreHovering component = collider.GetComponent<IgnoreHovering>();
						if (!(component != null) || (!(component.onlyIgnoreHand == null) && !(component.onlyIgnoreHand == this)))
						{
							bool flag2 = false;
							for (int k = 0; k < this.attachedObjects.Count; k++)
							{
								if (this.attachedObjects[k].attachedObject == componentInParent.gameObject)
								{
									flag2 = true;
									break;
								}
							}
							if (!flag2)
							{
								float num2 = Vector3.Distance(componentInParent.transform.position, hoverPosition);
								bool flag3 = false;
								if (closestInteractable != null)
								{
									flag3 = (componentInParent.hoverPriority < closestInteractable.hoverPriority);
								}
								if (num2 < closestDistance && !flag3)
								{
									closestDistance = num2;
									closestInteractable = componentInParent;
									flag = true;
								}
								num++;
							}
						}
					}
				}
			}
			if (this.showDebugInteractables && flag)
			{
				Debug.DrawLine(hoverPosition, closestInteractable.transform.position, debugColor, 0.05f, false);
			}
			if (num > 0 && num != this.prevOverlappingColliders)
			{
				this.prevOverlappingColliders = num;
				if (this.spewDebugText)
				{
					this.HandDebugLog("Found " + num.ToString() + " overlapping colliders.");
				}
			}
			return flag;
		}

		protected virtual void UpdateNoSteamVRFallback()
		{
			if (this.noSteamVRFallbackCamera)
			{
				Ray ray = this.noSteamVRFallbackCamera.ScreenPointToRay(Input.mousePosition);
				if (this.attachedObjects.Count > 0)
				{
					base.transform.position = ray.origin + this.noSteamVRFallbackInteractorDistance * ray.direction;
					return;
				}
				Vector3 position = base.transform.position;
				base.transform.position = this.noSteamVRFallbackCamera.transform.forward * -1000f;
				RaycastHit raycastHit;
				if (Physics.Raycast(ray, out raycastHit, this.noSteamVRFallbackMaxDistanceNoItem))
				{
					base.transform.position = raycastHit.point;
					this.noSteamVRFallbackInteractorDistance = Mathf.Min(this.noSteamVRFallbackMaxDistanceNoItem, raycastHit.distance);
					return;
				}
				if (this.noSteamVRFallbackInteractorDistance > 0f)
				{
					base.transform.position = ray.origin + Mathf.Min(this.noSteamVRFallbackMaxDistanceNoItem, this.noSteamVRFallbackInteractorDistance) * ray.direction;
					return;
				}
				base.transform.position = position;
			}
		}

		private void UpdateDebugText()
		{
			if (this.showDebugText)
			{
				if (this.debugText == null)
				{
					this.debugText = new GameObject("_debug_text").AddComponent<TextMesh>();
					this.debugText.fontSize = 120;
					this.debugText.characterSize = 0.001f;
					this.debugText.transform.parent = base.transform;
					this.debugText.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
				}
				if (this.handType == SteamVR_Input_Sources.RightHand)
				{
					this.debugText.transform.localPosition = new Vector3(-0.05f, 0f, 0f);
					this.debugText.alignment = TextAlignment.Right;
					this.debugText.anchor = TextAnchor.UpperRight;
				}
				else
				{
					this.debugText.transform.localPosition = new Vector3(0.05f, 0f, 0f);
					this.debugText.alignment = TextAlignment.Left;
					this.debugText.anchor = TextAnchor.UpperLeft;
				}
				this.debugText.text = string.Format("Hovering: {0}\nHover Lock: {1}\nAttached: {2}\nTotal Attached: {3}\nType: {4}\n", new object[]
				{
					this.hoveringInteractable ? this.hoveringInteractable.gameObject.name : "null",
					this.hoverLocked,
					this.currentAttachedObject ? this.currentAttachedObject.name : "null",
					this.attachedObjects.Count,
					this.handType.ToString()
				});
				return;
			}
			if (this.debugText != null)
			{
				Object.Destroy(this.debugText.gameObject);
			}
		}

		protected virtual void OnEnable()
		{
			this.inputFocusAction.enabled = true;
			float time = (this.otherHand != null && this.otherHand.GetInstanceID() < base.GetInstanceID()) ? (0.5f * this.hoverUpdateInterval) : 0f;
			base.InvokeRepeating("UpdateHovering", time, this.hoverUpdateInterval);
			base.InvokeRepeating("UpdateDebugText", time, this.hoverUpdateInterval);
		}

		protected virtual void OnDisable()
		{
			this.inputFocusAction.enabled = false;
			base.CancelInvoke();
		}

		protected virtual void Update()
		{
			this.UpdateNoSteamVRFallback();
			GameObject currentAttachedObject = this.currentAttachedObject;
			if (currentAttachedObject != null)
			{
				currentAttachedObject.SendMessage("HandAttachedUpdate", this, SendMessageOptions.DontRequireReceiver);
			}
			if (this.hoveringInteractable)
			{
				this.hoveringInteractable.SendMessage("HandHoverUpdate", this, SendMessageOptions.DontRequireReceiver);
			}
		}

		public bool IsStillHovering(Interactable interactable)
		{
			return this.hoveringInteractable == interactable;
		}

		protected virtual void HandFollowUpdate()
		{
			if (this.currentAttachedObject != null && this.currentAttachedObjectInfo.Value.interactable != null)
			{
				SteamVR_Skeleton_PoseSnapshot steamVR_Skeleton_PoseSnapshot = null;
				if (this.currentAttachedObjectInfo.Value.interactable.skeletonPoser != null && this.HasSkeleton())
				{
					steamVR_Skeleton_PoseSnapshot = this.currentAttachedObjectInfo.Value.interactable.skeletonPoser.GetBlendedPose(this.skeleton);
				}
				if (this.currentAttachedObjectInfo.Value.interactable.handFollowTransform)
				{
					Quaternion handRotation;
					Vector3 handPosition;
					if (steamVR_Skeleton_PoseSnapshot == null)
					{
						Quaternion rotation = Quaternion.Inverse(base.transform.rotation) * this.currentAttachedObjectInfo.Value.handAttachmentPointTransform.rotation;
						handRotation = this.currentAttachedObjectInfo.Value.interactable.transform.rotation * Quaternion.Inverse(rotation);
						Vector3 point = base.transform.position - this.currentAttachedObjectInfo.Value.handAttachmentPointTransform.position;
						Vector3 b = this.mainRenderModel.GetHandRotation() * Quaternion.Inverse(base.transform.rotation) * point;
						handPosition = this.currentAttachedObjectInfo.Value.interactable.transform.position + b;
					}
					else
					{
						Transform transform = this.currentAttachedObjectInfo.Value.attachedObject.transform;
						Vector3 position = transform.position;
						Quaternion rotation2 = transform.transform.rotation;
						transform.position = this.TargetItemPosition(this.currentAttachedObjectInfo.Value);
						transform.rotation = this.TargetItemRotation(this.currentAttachedObjectInfo.Value);
						Vector3 position2 = transform.InverseTransformPoint(base.transform.position);
						Quaternion rhs = Quaternion.Inverse(transform.rotation) * base.transform.rotation;
						transform.position = position;
						transform.rotation = rotation2;
						handPosition = transform.TransformPoint(position2);
						handRotation = transform.rotation * rhs;
					}
					if (this.mainRenderModel != null)
					{
						this.mainRenderModel.SetHandRotation(handRotation);
					}
					if (this.hoverhighlightRenderModel != null)
					{
						this.hoverhighlightRenderModel.SetHandRotation(handRotation);
					}
					if (this.mainRenderModel != null)
					{
						this.mainRenderModel.SetHandPosition(handPosition);
					}
					if (this.hoverhighlightRenderModel != null)
					{
						this.hoverhighlightRenderModel.SetHandPosition(handPosition);
					}
				}
			}
		}

		protected virtual void FixedUpdate()
		{
			if (this.currentAttachedObject != null)
			{
				Hand.AttachedObject value = this.currentAttachedObjectInfo.Value;
				if (value.attachedObject != null)
				{
					if (value.HasAttachFlag(Hand.AttachmentFlags.VelocityMovement))
					{
						if (!value.interactable.attachEaseIn || value.interactable.snapAttachEaseInCompleted)
						{
							this.UpdateAttachedVelocity(value);
						}
					}
					else if (value.HasAttachFlag(Hand.AttachmentFlags.ParentToHand))
					{
						value.attachedObject.transform.position = this.TargetItemPosition(value);
						value.attachedObject.transform.rotation = this.TargetItemRotation(value);
					}
					if (value.interactable.attachEaseIn)
					{
						float num = Util.RemapNumberClamped(Time.time, value.attachTime, value.attachTime + value.interactable.snapAttachEaseInTime, 0f, 1f);
						if (num < 1f)
						{
							if (value.HasAttachFlag(Hand.AttachmentFlags.VelocityMovement))
							{
								value.attachedRigidbody.linearVelocity = Vector3.zero;
								value.attachedRigidbody.angularVelocity = Vector3.zero;
							}
							num = value.interactable.snapAttachEaseInCurve.Evaluate(num);
							value.attachedObject.transform.position = Vector3.Lerp(value.easeSourcePosition, this.TargetItemPosition(value), num);
							value.attachedObject.transform.rotation = Quaternion.Lerp(value.easeSourceRotation, this.TargetItemRotation(value), num);
							return;
						}
						if (!value.interactable.snapAttachEaseInCompleted)
						{
							value.interactable.gameObject.SendMessage("OnThrowableAttachEaseInCompleted", this, SendMessageOptions.DontRequireReceiver);
							value.interactable.snapAttachEaseInCompleted = true;
						}
					}
				}
			}
		}

		protected void UpdateAttachedVelocity(Hand.AttachedObject attachedObjectInfo)
		{
			Vector3 target;
			Vector3 target2;
			if (this.GetUpdatedAttachedVelocities(attachedObjectInfo, out target, out target2))
			{
				float lossyScale = SteamVR_Utils.GetLossyScale(this.currentAttachedObjectInfo.Value.handAttachmentPointTransform);
				float maxDistanceDelta = 20f * lossyScale;
				float maxDistanceDelta2 = 10f * lossyScale;
				attachedObjectInfo.attachedRigidbody.linearVelocity = Vector3.MoveTowards(attachedObjectInfo.attachedRigidbody.linearVelocity, target, maxDistanceDelta2);
				attachedObjectInfo.attachedRigidbody.angularVelocity = Vector3.MoveTowards(attachedObjectInfo.attachedRigidbody.angularVelocity, target2, maxDistanceDelta);
			}
		}

		public void ResetAttachedTransform(Hand.AttachedObject attachedObject)
		{
			attachedObject.attachedObject.transform.position = this.TargetItemPosition(attachedObject);
			attachedObject.attachedObject.transform.rotation = this.TargetItemRotation(attachedObject);
		}

		protected Vector3 TargetItemPosition(Hand.AttachedObject attachedObject)
		{
			if (attachedObject.interactable != null && attachedObject.interactable.skeletonPoser != null && this.HasSkeleton())
			{
				Vector3 position = attachedObject.handAttachmentPointTransform.InverseTransformPoint(base.transform.TransformPoint(attachedObject.interactable.skeletonPoser.GetBlendedPose(this.skeleton).position));
				return this.currentAttachedObjectInfo.Value.handAttachmentPointTransform.TransformPoint(position);
			}
			return this.currentAttachedObjectInfo.Value.handAttachmentPointTransform.TransformPoint(attachedObject.initialPositionalOffset);
		}

		protected Quaternion TargetItemRotation(Hand.AttachedObject attachedObject)
		{
			if (attachedObject.interactable != null && attachedObject.interactable.skeletonPoser != null && this.HasSkeleton())
			{
				Quaternion rhs = Quaternion.Inverse(attachedObject.handAttachmentPointTransform.rotation) * (base.transform.rotation * attachedObject.interactable.skeletonPoser.GetBlendedPose(this.skeleton).rotation);
				return this.currentAttachedObjectInfo.Value.handAttachmentPointTransform.rotation * rhs;
			}
			return this.currentAttachedObjectInfo.Value.handAttachmentPointTransform.rotation * attachedObject.initialRotationalOffset;
		}

		protected bool GetUpdatedAttachedVelocities(Hand.AttachedObject attachedObjectInfo, out Vector3 velocityTarget, out Vector3 angularTarget)
		{
			bool flag = false;
			float d = 6000f;
			float d2 = 50f;
			Vector3 a = this.TargetItemPosition(attachedObjectInfo) - attachedObjectInfo.attachedRigidbody.position;
			velocityTarget = a * d * Time.deltaTime;
			if (!float.IsNaN(velocityTarget.x) && !float.IsInfinity(velocityTarget.x))
			{
				if (this.noSteamVRFallbackCamera)
				{
					velocityTarget /= 10f;
				}
				flag = true;
			}
			else
			{
				velocityTarget = Vector3.zero;
			}
			float num;
			Vector3 vector;
			(this.TargetItemRotation(attachedObjectInfo) * Quaternion.Inverse(attachedObjectInfo.attachedObject.transform.rotation)).ToAngleAxis(out num, out vector);
			if (num > 180f)
			{
				num -= 360f;
			}
			if (num != 0f && !float.IsNaN(vector.x) && !float.IsInfinity(vector.x))
			{
				angularTarget = num * vector * d2 * Time.deltaTime;
				if (this.noSteamVRFallbackCamera)
				{
					angularTarget /= 10f;
				}
				flag = flag;
			}
			else
			{
				angularTarget = Vector3.zero;
			}
			return flag;
		}

		protected virtual void OnInputFocus(bool hasFocus)
		{
			if (hasFocus)
			{
				this.DetachObject(this.applicationLostFocusObject, true);
				this.applicationLostFocusObject.SetActive(false);
				this.UpdateHovering();
				base.BroadcastMessage("OnParentHandInputFocusAcquired", SendMessageOptions.DontRequireReceiver);
				return;
			}
			this.applicationLostFocusObject.SetActive(true);
			this.AttachObject(this.applicationLostFocusObject, GrabTypes.Scripted, Hand.AttachmentFlags.ParentToHand, null);
			base.BroadcastMessage("OnParentHandInputFocusLost", SendMessageOptions.DontRequireReceiver);
		}

		protected virtual void OnDrawGizmos()
		{
			if (this.useHoverSphere && this.hoverSphereTransform != null)
			{
				Gizmos.color = Color.green;
				float num = this.hoverSphereRadius * Mathf.Abs(SteamVR_Utils.GetLossyScale(this.hoverSphereTransform));
				Gizmos.DrawWireSphere(this.hoverSphereTransform.position, num / 2f);
			}
			if (this.useControllerHoverComponent && this.mainRenderModel != null && this.mainRenderModel.IsControllerVisibile())
			{
				Gizmos.color = Color.blue;
				float num2 = this.controllerHoverRadius * Mathf.Abs(SteamVR_Utils.GetLossyScale(base.transform));
				Gizmos.DrawWireSphere(this.mainRenderModel.GetControllerPosition(this.controllerHoverComponent), num2 / 2f);
			}
			if (this.useFingerJointHover && this.mainRenderModel != null && this.mainRenderModel.IsHandVisibile())
			{
				Gizmos.color = Color.yellow;
				float num3 = this.fingerJointHoverRadius * Mathf.Abs(SteamVR_Utils.GetLossyScale(base.transform));
				Gizmos.DrawWireSphere(this.mainRenderModel.GetBonePosition((int)this.fingerJointHover, false), num3 / 2f);
			}
		}

		private void HandDebugLog(string msg)
		{
			if (this.spewDebugText)
			{
				Debug.Log("<b>[SteamVR Interaction]</b> Hand (" + base.name + "): " + msg);
			}
		}

		public void HoverLock(Interactable interactable)
		{
			if (this.spewDebugText)
			{
				this.HandDebugLog("HoverLock " + ((interactable != null) ? interactable.ToString() : null));
			}
			this.hoverLocked = true;
			this.hoveringInteractable = interactable;
		}

		public void HoverUnlock(Interactable interactable)
		{
			if (this.spewDebugText)
			{
				this.HandDebugLog("HoverUnlock " + ((interactable != null) ? interactable.ToString() : null));
			}
			if (this.hoveringInteractable == interactable)
			{
				this.hoverLocked = false;
			}
		}

		public void TriggerHapticPulse(ushort microSecondsDuration)
		{
			float num = (float)microSecondsDuration / 1000000f;
			this.hapticAction.Execute(0f, num, 1f / num, 1f, this.handType);
		}

		public void TriggerHapticPulse(float duration, float frequency, float amplitude)
		{
			this.hapticAction.Execute(0f, duration, frequency, amplitude, this.handType);
		}

		public void ShowGrabHint()
		{
			ControllerButtonHints.ShowButtonHint(this, new ISteamVR_Action_In_Source[]
			{
				this.grabGripAction
			});
		}

		public void HideGrabHint()
		{
			ControllerButtonHints.HideButtonHint(this, new ISteamVR_Action_In_Source[]
			{
				this.grabGripAction
			});
		}

		public void ShowGrabHint(string text)
		{
			ControllerButtonHints.ShowTextHint(this, this.grabGripAction, text, true);
		}

		public GrabTypes GetGrabStarting(GrabTypes explicitType = GrabTypes.None)
		{
			if (explicitType != GrabTypes.None)
			{
				if (this.noSteamVRFallbackCamera)
				{
					if (Input.GetMouseButtonDown(0))
					{
						return explicitType;
					}
					return GrabTypes.None;
				}
				else
				{
					if (explicitType == GrabTypes.Pinch && this.grabPinchAction.GetStateDown(this.handType))
					{
						return GrabTypes.Pinch;
					}
					if (explicitType == GrabTypes.Grip && this.grabGripAction.GetStateDown(this.handType))
					{
						return GrabTypes.Grip;
					}
				}
			}
			else if (this.noSteamVRFallbackCamera)
			{
				if (Input.GetMouseButtonDown(0))
				{
					return GrabTypes.Grip;
				}
				return GrabTypes.None;
			}
			else
			{
				if (this.grabPinchAction != null && this.grabPinchAction.GetStateDown(this.handType))
				{
					return GrabTypes.Pinch;
				}
				if (this.grabGripAction != null && this.grabGripAction.GetStateDown(this.handType))
				{
					return GrabTypes.Grip;
				}
			}
			return GrabTypes.None;
		}

		public GrabTypes GetGrabEnding(GrabTypes explicitType = GrabTypes.None)
		{
			if (explicitType != GrabTypes.None)
			{
				if (this.noSteamVRFallbackCamera)
				{
					if (Input.GetMouseButtonUp(0))
					{
						return explicitType;
					}
					return GrabTypes.None;
				}
				else
				{
					if (explicitType == GrabTypes.Pinch && this.grabPinchAction.GetStateUp(this.handType))
					{
						return GrabTypes.Pinch;
					}
					if (explicitType == GrabTypes.Grip && this.grabGripAction.GetStateUp(this.handType))
					{
						return GrabTypes.Grip;
					}
				}
			}
			else if (this.noSteamVRFallbackCamera)
			{
				if (Input.GetMouseButtonUp(0))
				{
					return GrabTypes.Grip;
				}
				return GrabTypes.None;
			}
			else
			{
				if (this.grabPinchAction.GetStateUp(this.handType))
				{
					return GrabTypes.Pinch;
				}
				if (this.grabGripAction.GetStateUp(this.handType))
				{
					return GrabTypes.Grip;
				}
			}
			return GrabTypes.None;
		}

		public bool IsGrabEnding(GameObject attachedObject)
		{
			for (int i = 0; i < this.attachedObjects.Count; i++)
			{
				if (this.attachedObjects[i].attachedObject == attachedObject)
				{
					return !this.IsGrabbingWithType(this.attachedObjects[i].grabbedWithType);
				}
			}
			return false;
		}

		public bool IsGrabbingWithType(GrabTypes type)
		{
			if (this.noSteamVRFallbackCamera)
			{
				return Input.GetMouseButton(0);
			}
			if (type != GrabTypes.Pinch)
			{
				return type == GrabTypes.Grip && this.grabGripAction.GetState(this.handType);
			}
			return this.grabPinchAction.GetState(this.handType);
		}

		public bool IsGrabbingWithOppositeType(GrabTypes type)
		{
			if (this.noSteamVRFallbackCamera)
			{
				return Input.GetMouseButton(0);
			}
			if (type != GrabTypes.Pinch)
			{
				return type == GrabTypes.Grip && this.grabPinchAction.GetState(this.handType);
			}
			return this.grabGripAction.GetState(this.handType);
		}

		public GrabTypes GetBestGrabbingType()
		{
			return this.GetBestGrabbingType(GrabTypes.None, false);
		}

		public GrabTypes GetBestGrabbingType(GrabTypes preferred, bool forcePreference = false)
		{
			if (this.noSteamVRFallbackCamera)
			{
				if (Input.GetMouseButton(0))
				{
					return preferred;
				}
				return GrabTypes.None;
			}
			else
			{
				if (preferred == GrabTypes.Pinch)
				{
					if (this.grabPinchAction.GetState(this.handType))
					{
						return GrabTypes.Pinch;
					}
					if (forcePreference)
					{
						return GrabTypes.None;
					}
				}
				if (preferred == GrabTypes.Grip)
				{
					if (this.grabGripAction.GetState(this.handType))
					{
						return GrabTypes.Grip;
					}
					if (forcePreference)
					{
						return GrabTypes.None;
					}
				}
				if (this.grabPinchAction.GetState(this.handType))
				{
					return GrabTypes.Pinch;
				}
				if (this.grabGripAction.GetState(this.handType))
				{
					return GrabTypes.Grip;
				}
				return GrabTypes.None;
			}
		}

		private void InitController()
		{
			if (this.spewDebugText)
			{
				this.HandDebugLog("Hand " + base.name + " connected with type " + this.handType.ToString());
			}
			bool flag = this.mainRenderModel != null;
			EVRSkeletalMotionRange newRangeOfMotion = EVRSkeletalMotionRange.WithController;
			if (flag)
			{
				newRangeOfMotion = this.mainRenderModel.GetSkeletonRangeOfMotion;
			}
			foreach (RenderModel renderModel in this.renderModels)
			{
				if (renderModel != null)
				{
					Object.Destroy(renderModel.gameObject);
				}
			}
			this.renderModels.Clear();
			GameObject gameObject = Object.Instantiate<GameObject>(this.renderModelPrefab);
			gameObject.layer = base.gameObject.layer;
			gameObject.tag = base.gameObject.tag;
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = this.renderModelPrefab.transform.localScale;
			int deviceIndex = this.trackedObject.GetDeviceIndex();
			this.mainRenderModel = gameObject.GetComponent<RenderModel>();
			this.renderModels.Add(this.mainRenderModel);
			if (flag)
			{
				this.mainRenderModel.SetSkeletonRangeOfMotion(newRangeOfMotion, 0.1f);
			}
			base.BroadcastMessage("SetInputSource", this.handType, SendMessageOptions.DontRequireReceiver);
			base.BroadcastMessage("OnHandInitialized", deviceIndex, SendMessageOptions.DontRequireReceiver);
		}

		public void SetRenderModel(GameObject prefab)
		{
			this.renderModelPrefab = prefab;
			if (this.mainRenderModel != null && this.isPoseValid)
			{
				this.InitController();
			}
		}

		public void SetHoverRenderModel(RenderModel hoverRenderModel)
		{
			this.hoverhighlightRenderModel = hoverRenderModel;
			this.renderModels.Add(hoverRenderModel);
		}

		public int GetDeviceIndex()
		{
			return this.trackedObject.GetDeviceIndex();
		}

		public const Hand.AttachmentFlags defaultAttachmentFlags = Hand.AttachmentFlags.SnapOnAttach | Hand.AttachmentFlags.DetachOthers | Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.TurnOnKinematic;

		public Hand otherHand;

		public SteamVR_Input_Sources handType;

		public SteamVR_Behaviour_Pose trackedObject;

		public SteamVR_Action_Boolean grabPinchAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabPinch", false);

		public SteamVR_Action_Boolean grabGripAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip", false);

		public SteamVR_Action_Vibration hapticAction = SteamVR_Input.GetAction<SteamVR_Action_Vibration>("Haptic", false);

		public SteamVR_Action_Boolean uiInteractAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI", false);

		public bool useHoverSphere = true;

		public Transform hoverSphereTransform;

		public float hoverSphereRadius = 0.05f;

		public LayerMask hoverLayerMask = -1;

		public float hoverUpdateInterval = 0.1f;

		public bool useControllerHoverComponent = true;

		public string controllerHoverComponent = "tip";

		public float controllerHoverRadius = 0.075f;

		public bool useFingerJointHover = true;

		public SteamVR_Skeleton_JointIndexEnum fingerJointHover = SteamVR_Skeleton_JointIndexEnum.indexTip;

		public float fingerJointHoverRadius = 0.025f;

		[Tooltip("A transform on the hand to center attached objects on")]
		public Transform objectAttachmentPoint;

		public Camera noSteamVRFallbackCamera;

		public float noSteamVRFallbackMaxDistanceNoItem = 10f;

		public float noSteamVRFallbackMaxDistanceWithItem = 0.5f;

		private float noSteamVRFallbackInteractorDistance = -1f;

		public GameObject renderModelPrefab;

		[HideInInspector]
		public List<RenderModel> renderModels = new List<RenderModel>();

		[HideInInspector]
		public RenderModel mainRenderModel;

		[HideInInspector]
		public RenderModel hoverhighlightRenderModel;

		public bool showDebugText;

		public bool spewDebugText;

		public bool showDebugInteractables;

		private List<Hand.AttachedObject> attachedObjects = new List<Hand.AttachedObject>();

		private Interactable _hoveringInteractable;

		private TextMesh debugText;

		private int prevOverlappingColliders;

		private const int ColliderArraySize = 32;

		private Collider[] overlappingColliders;

		private Player playerInstance;

		private GameObject applicationLostFocusObject;

		private SteamVR_Events.Action inputFocusAction;

		protected const float MaxVelocityChange = 10f;

		protected const float VelocityMagic = 6000f;

		protected const float AngularVelocityMagic = 50f;

		protected const float MaxAngularVelocityChange = 20f;

		[Flags]
		public enum AttachmentFlags
		{
			SnapOnAttach = 1,
			DetachOthers = 2,
			DetachFromOtherHand = 4,
			ParentToHand = 8,
			VelocityMovement = 16,
			TurnOnKinematic = 32,
			TurnOffGravity = 64,
			AllowSidegrade = 128
		}

		public struct AttachedObject
		{
			public bool HasAttachFlag(Hand.AttachmentFlags flag)
			{
				return (this.attachmentFlags & flag) == flag;
			}

			public GameObject attachedObject;

			public Interactable interactable;

			public Rigidbody attachedRigidbody;

			public CollisionDetectionMode collisionDetectionMode;

			public bool attachedRigidbodyWasKinematic;

			public bool attachedRigidbodyUsedGravity;

			public GameObject originalParent;

			public bool isParentedToHand;

			public GrabTypes grabbedWithType;

			public Hand.AttachmentFlags attachmentFlags;

			public Vector3 initialPositionalOffset;

			public Quaternion initialRotationalOffset;

			public Transform attachedOffsetTransform;

			public Transform handAttachmentPointTransform;

			public Vector3 easeSourcePosition;

			public Quaternion easeSourceRotation;

			public float attachTime;

			public AllowTeleportWhileAttachedToHand allowTeleportWhileAttachedToHand;
		}
	}
}
