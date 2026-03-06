using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class ArrowHand : MonoBehaviour
	{
		private void Awake()
		{
			this.allowTeleport = base.GetComponent<AllowTeleportWhileAttachedToHand>();
			this.allowTeleport.overrideHoverLock = false;
			this.arrowList = new List<GameObject>();
		}

		private void OnAttachedToHand(Hand attachedHand)
		{
			this.hand = attachedHand;
			this.FindBow();
		}

		private GameObject InstantiateArrow()
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.arrowPrefab, this.arrowNockTransform.position, this.arrowNockTransform.rotation);
			gameObject.name = "Bow Arrow";
			gameObject.transform.parent = this.arrowNockTransform;
			Util.ResetTransform(gameObject.transform, true);
			this.arrowList.Add(gameObject);
			while (this.arrowList.Count > this.maxArrowCount)
			{
				GameObject gameObject2 = this.arrowList[0];
				this.arrowList.RemoveAt(0);
				if (gameObject2)
				{
					Object.Destroy(gameObject2);
				}
			}
			return gameObject;
		}

		private void HandAttachedUpdate(Hand hand)
		{
			if (this.bow == null)
			{
				this.FindBow();
			}
			if (this.bow == null)
			{
				return;
			}
			if (this.allowArrowSpawn && this.currentArrow == null)
			{
				this.currentArrow = this.InstantiateArrow();
				this.arrowSpawnSound.Play();
			}
			float num = Vector3.Distance(base.transform.parent.position, this.bow.nockTransform.position);
			if (!this.nocked)
			{
				if (num < this.rotationLerpThreshold)
				{
					float t = Util.RemapNumber(num, this.rotationLerpThreshold, this.lerpCompleteDistance, 0f, 1f);
					this.arrowNockTransform.rotation = Quaternion.Lerp(this.arrowNockTransform.parent.rotation, this.bow.nockRestTransform.rotation, t);
				}
				else
				{
					this.arrowNockTransform.localRotation = Quaternion.identity;
				}
				if (num < this.positionLerpThreshold)
				{
					float num2 = Util.RemapNumber(num, this.positionLerpThreshold, this.lerpCompleteDistance, 0f, 1f);
					num2 = Mathf.Clamp(num2, 0f, 1f);
					this.arrowNockTransform.position = Vector3.Lerp(this.arrowNockTransform.parent.position, this.bow.nockRestTransform.position, num2);
				}
				else
				{
					this.arrowNockTransform.position = this.arrowNockTransform.parent.position;
				}
				if (num < this.lerpCompleteDistance)
				{
					if (!this.arrowLerpComplete)
					{
						this.arrowLerpComplete = true;
						hand.TriggerHapticPulse(500);
					}
				}
				else if (this.arrowLerpComplete)
				{
					this.arrowLerpComplete = false;
				}
				if (num < this.nockDistance)
				{
					if (!this.inNockRange)
					{
						this.inNockRange = true;
						this.bow.ArrowInPosition();
					}
				}
				else if (this.inNockRange)
				{
					this.inNockRange = false;
				}
				GrabTypes bestGrabbingType = hand.GetBestGrabbingType(GrabTypes.Pinch, true);
				if (num < this.nockDistance && bestGrabbingType != GrabTypes.None && !this.nocked)
				{
					if (this.currentArrow == null)
					{
						this.currentArrow = this.InstantiateArrow();
					}
					this.nocked = true;
					this.nockedWithType = bestGrabbingType;
					this.bow.StartNock(this);
					hand.HoverLock(base.GetComponent<Interactable>());
					this.allowTeleport.teleportAllowed = false;
					this.currentArrow.transform.parent = this.bow.nockTransform;
					Util.ResetTransform(this.currentArrow.transform, true);
					Util.ResetTransform(this.arrowNockTransform, true);
				}
			}
			if (this.nocked && !hand.IsGrabbingWithType(this.nockedWithType))
			{
				if (this.bow.pulled)
				{
					this.FireArrow();
				}
				else
				{
					this.arrowNockTransform.rotation = this.currentArrow.transform.rotation;
					this.currentArrow.transform.parent = this.arrowNockTransform;
					Util.ResetTransform(this.currentArrow.transform, true);
					this.nocked = false;
					this.nockedWithType = GrabTypes.None;
					this.bow.ReleaseNock();
					hand.HoverUnlock(base.GetComponent<Interactable>());
					this.allowTeleport.teleportAllowed = true;
				}
				this.bow.StartRotationLerp();
			}
		}

		private void OnDetachedFromHand(Hand hand)
		{
			Object.Destroy(base.gameObject);
		}

		private void FireArrow()
		{
			this.currentArrow.transform.parent = null;
			Arrow component = this.currentArrow.GetComponent<Arrow>();
			component.shaftRB.isKinematic = false;
			component.shaftRB.useGravity = true;
			component.shaftRB.transform.GetComponent<BoxCollider>().enabled = true;
			component.arrowHeadRB.isKinematic = false;
			component.arrowHeadRB.useGravity = true;
			component.arrowHeadRB.transform.GetComponent<BoxCollider>().enabled = true;
			component.arrowHeadRB.AddForce(this.currentArrow.transform.forward * this.bow.GetArrowVelocity() * component.arrowHeadRB.mass, ForceMode.Impulse);
			component.arrowHeadRB.AddTorque(this.currentArrow.transform.forward * 10f);
			this.nocked = false;
			this.nockedWithType = GrabTypes.None;
			this.currentArrow.GetComponent<Arrow>().ArrowReleased(this.bow.GetArrowVelocity());
			this.bow.ArrowReleased();
			this.allowArrowSpawn = false;
			base.Invoke("EnableArrowSpawn", 0.5f);
			base.StartCoroutine(this.ArrowReleaseHaptics());
			this.currentArrow = null;
			this.allowTeleport.teleportAllowed = true;
		}

		private void EnableArrowSpawn()
		{
			this.allowArrowSpawn = true;
		}

		private IEnumerator ArrowReleaseHaptics()
		{
			yield return new WaitForSeconds(0.05f);
			this.hand.otherHand.TriggerHapticPulse(1500);
			yield return new WaitForSeconds(0.05f);
			this.hand.otherHand.TriggerHapticPulse(800);
			yield return new WaitForSeconds(0.05f);
			this.hand.otherHand.TriggerHapticPulse(500);
			yield return new WaitForSeconds(0.05f);
			this.hand.otherHand.TriggerHapticPulse(300);
			yield break;
		}

		private void OnHandFocusLost(Hand hand)
		{
			base.gameObject.SetActive(false);
		}

		private void OnHandFocusAcquired(Hand hand)
		{
			base.gameObject.SetActive(true);
		}

		private void FindBow()
		{
			this.bow = this.hand.otherHand.GetComponentInChildren<Longbow>();
		}

		private Hand hand;

		private Longbow bow;

		private GameObject currentArrow;

		public GameObject arrowPrefab;

		public Transform arrowNockTransform;

		public float nockDistance = 0.1f;

		public float lerpCompleteDistance = 0.08f;

		public float rotationLerpThreshold = 0.15f;

		public float positionLerpThreshold = 0.15f;

		private bool allowArrowSpawn = true;

		private bool nocked;

		private GrabTypes nockedWithType;

		private bool inNockRange;

		private bool arrowLerpComplete;

		public SoundPlayOneshot arrowSpawnSound;

		private AllowTeleportWhileAttachedToHand allowTeleport;

		public int maxArrowCount = 10;

		private List<GameObject> arrowList;
	}
}
