using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaLocomotion.Climbing
{
	public class GorillaHandClimber : MonoBehaviour
	{
		public bool isClimbingOrGrabbing
		{
			get
			{
				return this.isClimbing || this.grabber.isGrabbing;
			}
		}

		private void Awake()
		{
			this.col = base.GetComponent<Collider>();
			this.grabber = base.GetComponent<GorillaGrabber>();
		}

		public void CheckHandClimber()
		{
			for (int i = this.potentialClimbables.Count - 1; i >= 0; i--)
			{
				GorillaClimbable gorillaClimbable = this.potentialClimbables[i];
				if (gorillaClimbable == null || !gorillaClimbable.isActiveAndEnabled)
				{
					this.potentialClimbables.RemoveAt(i);
				}
				else if (gorillaClimbable.climbOnlyWhileSmall && !ZoneManagement.IsInZone(GTZone.monkeBlocksShared) && this.player.scale > 0.99f)
				{
					this.potentialClimbables.RemoveAt(i);
				}
			}
			bool grab = ControllerInputPoller.GetGrab(this.xrNode);
			bool grabRelease = ControllerInputPoller.GetGrabRelease(this.xrNode);
			if (!this.isClimbing)
			{
				if (this.queuedToBecomeValidToGrabAgain && Vector3.Distance(this.lastAutoReleasePos, this.handRoot.localPosition) >= 0.35f)
				{
					this.queuedToBecomeValidToGrabAgain = false;
				}
				if (grabRelease)
				{
					this.queuedToBecomeValidToGrabAgain = false;
					this.dontReclimbLast = null;
				}
				GorillaClimbable closestClimbable = this.GetClosestClimbable();
				if (!this.queuedToBecomeValidToGrabAgain && closestClimbable && grab && this.CanInitiateClimb() && closestClimbable != this.dontReclimbLast)
				{
					GorillaClimbableRef gorillaClimbableRef = closestClimbable as GorillaClimbableRef;
					if (gorillaClimbableRef != null)
					{
						this.player.BeginClimbing(gorillaClimbableRef.climb, this, gorillaClimbableRef);
					}
					else
					{
						this.player.BeginClimbing(closestClimbable, this, null);
					}
				}
			}
			else if (grabRelease && this.canRelease)
			{
				this.player.EndClimbing(this, false, false);
			}
			this.grabber.CheckGrabber(this.CanInitiateClimb() && grab);
		}

		private bool CanInitiateClimb()
		{
			return !this.isClimbing && !this.equipmentInteractor.GetIsHolding(this.xrNode) && !this.equipmentInteractor.builderPieceInteractor.GetIsHolding(this.xrNode) && !this.equipmentInteractor.IsGrabDisabled(this.xrNode) && !GamePlayerLocal.IsHandHolding(this.xrNode) && !this.player.inOverlay;
		}

		public void SetCanRelease(bool canRelease)
		{
			this.canRelease = canRelease;
		}

		public GorillaClimbable GetClosestClimbable()
		{
			if (this.potentialClimbables.Count == 0)
			{
				return null;
			}
			if (this.potentialClimbables.Count == 1)
			{
				return this.potentialClimbables[0];
			}
			Vector3 position = base.transform.position;
			Bounds bounds = this.col.bounds;
			float num = 0.15f;
			GorillaClimbable result = null;
			foreach (GorillaClimbable gorillaClimbable in this.potentialClimbables)
			{
				float num2;
				if (gorillaClimbable.colliderCache)
				{
					if (!bounds.Intersects(gorillaClimbable.colliderCache.bounds))
					{
						continue;
					}
					Vector3 b = gorillaClimbable.colliderCache.ClosestPoint(position);
					num2 = Vector3.Distance(position, b);
				}
				else
				{
					num2 = Vector3.Distance(position, gorillaClimbable.transform.position);
				}
				if (num2 < num)
				{
					result = gorillaClimbable;
					num = num2;
				}
			}
			return result;
		}

		private void OnTriggerEnter(Collider other)
		{
			GorillaClimbable item;
			if (other.TryGetComponent<GorillaClimbable>(out item))
			{
				this.potentialClimbables.Add(item);
				return;
			}
			GorillaClimbableRef item2;
			if (other.TryGetComponent<GorillaClimbableRef>(out item2))
			{
				this.potentialClimbables.Add(item2);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			GorillaClimbable item;
			if (other.TryGetComponent<GorillaClimbable>(out item))
			{
				this.potentialClimbables.Remove(item);
				return;
			}
			GorillaClimbableRef item2;
			if (other.TryGetComponent<GorillaClimbableRef>(out item2))
			{
				this.potentialClimbables.Remove(item2);
			}
		}

		public void ForceStopClimbing(bool startingNewClimb = false, bool doDontReclimb = false)
		{
			this.player.EndClimbing(this, startingNewClimb, doDontReclimb);
		}

		[SerializeField]
		private GTPlayer player;

		[SerializeField]
		private EquipmentInteractor equipmentInteractor;

		private List<GorillaClimbable> potentialClimbables = new List<GorillaClimbable>();

		[Header("Non-hand input should have the component disabled")]
		public XRNode xrNode = XRNode.LeftHand;

		[NonSerialized]
		public bool isClimbing;

		[NonSerialized]
		public bool queuedToBecomeValidToGrabAgain;

		[NonSerialized]
		public GorillaClimbable dontReclimbLast;

		[NonSerialized]
		public Vector3 lastAutoReleasePos = Vector3.zero;

		public GorillaGrabber grabber;

		public Transform handRoot;

		private const float DIST_FOR_CLEAR_RELEASE = 0.35f;

		private const float DIST_FOR_GRAB = 0.15f;

		private Collider col;

		private bool canRelease = true;
	}
}
