using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem
{
	[RequireComponent(typeof(Interactable))]
	[RequireComponent(typeof(Rigidbody))]
	public class Throwable : MonoBehaviour
	{
		protected virtual void Awake()
		{
			this.velocityEstimator = base.GetComponent<VelocityEstimator>();
			this.interactable = base.GetComponent<Interactable>();
			this.rigidbody = base.GetComponent<Rigidbody>();
			this.rigidbody.maxAngularVelocity = 50f;
			this.attachmentOffset != null;
		}

		protected virtual void OnHandHoverBegin(Hand hand)
		{
			bool flag = false;
			if (!this.attached && this.catchingSpeedThreshold != -1f)
			{
				float num = this.catchingSpeedThreshold * SteamVR_Utils.GetLossyScale(Player.instance.trackingOriginTransform);
				GrabTypes bestGrabbingType = hand.GetBestGrabbingType();
				if (bestGrabbingType != GrabTypes.None && this.rigidbody.linearVelocity.magnitude >= num)
				{
					hand.AttachObject(base.gameObject, bestGrabbingType, this.attachmentFlags, null);
					flag = false;
				}
			}
			if (flag)
			{
				hand.ShowGrabHint();
			}
		}

		protected virtual void OnHandHoverEnd(Hand hand)
		{
			hand.HideGrabHint();
		}

		protected virtual void HandHoverUpdate(Hand hand)
		{
			GrabTypes grabStarting = hand.GetGrabStarting(GrabTypes.None);
			if (grabStarting != GrabTypes.None)
			{
				hand.AttachObject(base.gameObject, grabStarting, this.attachmentFlags, this.attachmentOffset);
				hand.HideGrabHint();
			}
		}

		protected virtual void OnAttachedToHand(Hand hand)
		{
			this.hadInterpolation = this.rigidbody.interpolation;
			this.attached = true;
			this.onPickUp.Invoke();
			hand.HoverLock(null);
			this.rigidbody.interpolation = RigidbodyInterpolation.None;
			if (this.velocityEstimator != null)
			{
				this.velocityEstimator.BeginEstimatingVelocity();
			}
			this.attachTime = Time.time;
			this.attachPosition = base.transform.position;
			this.attachRotation = base.transform.rotation;
		}

		protected virtual void OnDetachedFromHand(Hand hand)
		{
			this.attached = false;
			this.onDetachFromHand.Invoke();
			hand.HoverUnlock(null);
			this.rigidbody.interpolation = this.hadInterpolation;
			Vector3 linearVelocity;
			Vector3 angularVelocity;
			this.GetReleaseVelocities(hand, out linearVelocity, out angularVelocity);
			this.rigidbody.linearVelocity = linearVelocity;
			this.rigidbody.angularVelocity = angularVelocity;
		}

		public virtual void GetReleaseVelocities(Hand hand, out Vector3 velocity, out Vector3 angularVelocity)
		{
			if (hand.noSteamVRFallbackCamera && this.releaseVelocityStyle != ReleaseStyle.NoChange)
			{
				this.releaseVelocityStyle = ReleaseStyle.ShortEstimation;
			}
			switch (this.releaseVelocityStyle)
			{
			case ReleaseStyle.GetFromHand:
				velocity = hand.GetTrackedObjectVelocity(this.releaseVelocityTimeOffset);
				angularVelocity = hand.GetTrackedObjectAngularVelocity(this.releaseVelocityTimeOffset);
				goto IL_FE;
			case ReleaseStyle.ShortEstimation:
				if (this.velocityEstimator != null)
				{
					this.velocityEstimator.FinishEstimatingVelocity();
					velocity = this.velocityEstimator.GetVelocityEstimate();
					angularVelocity = this.velocityEstimator.GetAngularVelocityEstimate();
					goto IL_FE;
				}
				Debug.LogWarning("[SteamVR Interaction System] Throwable: No Velocity Estimator component on object but release style set to short estimation. Please add one or change the release style.");
				velocity = this.rigidbody.linearVelocity;
				angularVelocity = this.rigidbody.angularVelocity;
				goto IL_FE;
			case ReleaseStyle.AdvancedEstimation:
				hand.GetEstimatedPeakVelocities(out velocity, out angularVelocity);
				goto IL_FE;
			}
			velocity = this.rigidbody.linearVelocity;
			angularVelocity = this.rigidbody.angularVelocity;
			IL_FE:
			if (this.releaseVelocityStyle != ReleaseStyle.NoChange)
			{
				float num = 1f;
				if (this.scaleReleaseVelocityThreshold > 0f)
				{
					num = Mathf.Clamp01(this.scaleReleaseVelocityCurve.Evaluate(velocity.magnitude / this.scaleReleaseVelocityThreshold));
				}
				velocity *= num * this.scaleReleaseVelocity;
			}
		}

		protected virtual void HandAttachedUpdate(Hand hand)
		{
			if (hand.IsGrabEnding(base.gameObject))
			{
				hand.DetachObject(base.gameObject, this.restoreOriginalParent);
			}
			if (this.onHeldUpdate != null)
			{
				this.onHeldUpdate.Invoke(hand);
			}
		}

		protected virtual IEnumerator LateDetach(Hand hand)
		{
			yield return new WaitForEndOfFrame();
			hand.DetachObject(base.gameObject, this.restoreOriginalParent);
			yield break;
		}

		protected virtual void OnHandFocusAcquired(Hand hand)
		{
			base.gameObject.SetActive(true);
			if (this.velocityEstimator != null)
			{
				this.velocityEstimator.BeginEstimatingVelocity();
			}
		}

		protected virtual void OnHandFocusLost(Hand hand)
		{
			base.gameObject.SetActive(false);
			if (this.velocityEstimator != null)
			{
				this.velocityEstimator.FinishEstimatingVelocity();
			}
		}

		[EnumFlags]
		[Tooltip("The flags used to attach this object to the hand.")]
		public Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.DetachFromOtherHand | Hand.AttachmentFlags.ParentToHand | Hand.AttachmentFlags.TurnOnKinematic;

		[Tooltip("The local point which acts as a positional and rotational offset to use while held")]
		public Transform attachmentOffset;

		[Tooltip("How fast must this object be moving to attach due to a trigger hold instead of a trigger press? (-1 to disable)")]
		public float catchingSpeedThreshold = -1f;

		public ReleaseStyle releaseVelocityStyle = ReleaseStyle.GetFromHand;

		[Tooltip("The time offset used when releasing the object with the RawFromHand option")]
		public float releaseVelocityTimeOffset = -0.011f;

		public float scaleReleaseVelocity = 1.1f;

		[Tooltip("The release velocity magnitude representing the end of the scale release velocity curve. (-1 to disable)")]
		public float scaleReleaseVelocityThreshold = -1f;

		[Tooltip("Use this curve to ease into the scaled release velocity based on the magnitude of the measured release velocity. This allows greater differentiation between a drop, toss, and throw.")]
		public AnimationCurve scaleReleaseVelocityCurve = AnimationCurve.EaseInOut(0f, 0.1f, 1f, 1f);

		[Tooltip("When detaching the object, should it return to its original parent?")]
		public bool restoreOriginalParent;

		protected VelocityEstimator velocityEstimator;

		protected bool attached;

		protected float attachTime;

		protected Vector3 attachPosition;

		protected Quaternion attachRotation;

		protected Transform attachEaseInTransform;

		public UnityEvent onPickUp;

		public UnityEvent onDetachFromHand;

		public HandEvent onHeldUpdate;

		protected RigidbodyInterpolation hadInterpolation;

		protected Rigidbody rigidbody;

		[HideInInspector]
		public Interactable interactable;
	}
}
