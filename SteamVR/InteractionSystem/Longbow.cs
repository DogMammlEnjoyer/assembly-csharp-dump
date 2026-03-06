using System;
using System.Collections;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	[RequireComponent(typeof(Interactable))]
	public class Longbow : MonoBehaviour
	{
		private void OnAttachedToHand(Hand attachedHand)
		{
			this.hand = attachedHand;
		}

		private void HandAttachedUpdate(Hand hand)
		{
			this.EvaluateHandedness();
			if (this.nocked)
			{
				Vector3 lhs = this.arrowHand.arrowNockTransform.parent.position - this.nockRestTransform.position;
				float num = Util.RemapNumberClamped(Time.time, this.nockLerpStartTime, this.nockLerpStartTime + this.lerpDuration, 0f, 1f);
				float d = Util.RemapNumberClamped(lhs.magnitude, 0.05f, 0.5f, 0f, 1f);
				Vector3 normalized = (Player.instance.hmdTransform.position + Vector3.down * 0.05f - this.arrowHand.arrowNockTransform.parent.position).normalized;
				Vector3 normalized2 = (this.arrowHand.arrowNockTransform.parent.position + normalized * this.drawOffset * d - this.pivotTransform.position).normalized;
				Vector3 normalized3 = (this.handleTransform.position - this.pivotTransform.position).normalized;
				this.bowLeftVector = -Vector3.Cross(normalized3, normalized2);
				this.pivotTransform.rotation = Quaternion.Lerp(this.nockLerpStartRotation, Quaternion.LookRotation(normalized2, this.bowLeftVector), num);
				if (Vector3.Dot(lhs, -this.nockTransform.forward) <= 0f)
				{
					this.nockTransform.localPosition = new Vector3(0f, 0f, 0f);
					this.bowDrawLinearMapping.value = 0f;
					return;
				}
				float num2 = lhs.magnitude * num;
				this.nockTransform.localPosition = new Vector3(0f, 0f, Mathf.Clamp(-num2, -0.5f, 0f));
				this.nockDistanceTravelled = -this.nockTransform.localPosition.z;
				this.arrowVelocity = Util.RemapNumber(this.nockDistanceTravelled, 0.05f, 0.5f, this.arrowMinVelocity, this.arrowMaxVelocity);
				this.drawTension = Util.RemapNumberClamped(this.nockDistanceTravelled, 0f, 0.5f, 0f, 1f);
				this.bowDrawLinearMapping.value = this.drawTension;
				if (this.nockDistanceTravelled > 0.05f)
				{
					this.pulled = true;
				}
				else
				{
					this.pulled = false;
				}
				if (this.nockDistanceTravelled > this.lastTickDistance + this.hapticDistanceThreshold || this.nockDistanceTravelled < this.lastTickDistance - this.hapticDistanceThreshold)
				{
					ushort microSecondsDuration = (ushort)Util.RemapNumber(this.nockDistanceTravelled, 0f, 0.5f, 100f, 500f);
					hand.TriggerHapticPulse(microSecondsDuration);
					hand.otherHand.TriggerHapticPulse(microSecondsDuration);
					this.drawSound.PlayBowTensionClicks(this.drawTension);
					this.lastTickDistance = this.nockDistanceTravelled;
				}
				if (this.nockDistanceTravelled >= 0.5f && Time.time > this.nextStrainTick)
				{
					hand.TriggerHapticPulse(400);
					hand.otherHand.TriggerHapticPulse(400);
					this.drawSound.PlayBowTensionClicks(this.drawTension);
					this.nextStrainTick = Time.time + Random.Range(this.minStrainTickTime, this.maxStrainTickTime);
					return;
				}
			}
			else if (this.lerpBackToZeroRotation)
			{
				float num3 = Util.RemapNumber(Time.time, this.lerpStartTime, this.lerpStartTime + this.lerpDuration, 0f, 1f);
				this.pivotTransform.localRotation = Quaternion.Lerp(this.lerpStartRotation, Quaternion.identity, num3);
				if (num3 >= 1f)
				{
					this.lerpBackToZeroRotation = false;
				}
			}
		}

		public void ArrowReleased()
		{
			this.nocked = false;
			this.hand.HoverUnlock(base.GetComponent<Interactable>());
			this.hand.otherHand.HoverUnlock(this.arrowHand.GetComponent<Interactable>());
			if (this.releaseSound != null)
			{
				this.releaseSound.Play();
			}
			base.StartCoroutine(this.ResetDrawAnim());
		}

		private IEnumerator ResetDrawAnim()
		{
			float startTime = Time.time;
			float startLerp = this.drawTension;
			while (Time.time < startTime + 0.02f)
			{
				float value = Util.RemapNumberClamped(Time.time, startTime, startTime + 0.02f, startLerp, 0f);
				this.bowDrawLinearMapping.value = value;
				yield return null;
			}
			this.bowDrawLinearMapping.value = 0f;
			yield break;
		}

		public float GetArrowVelocity()
		{
			return this.arrowVelocity;
		}

		public void StartRotationLerp()
		{
			this.lerpStartTime = Time.time;
			this.lerpBackToZeroRotation = true;
			this.lerpStartRotation = this.pivotTransform.localRotation;
			Util.ResetTransform(this.nockTransform, true);
		}

		public void StartNock(ArrowHand currentArrowHand)
		{
			this.arrowHand = currentArrowHand;
			this.hand.HoverLock(base.GetComponent<Interactable>());
			this.nocked = true;
			this.nockLerpStartTime = Time.time;
			this.nockLerpStartRotation = this.pivotTransform.rotation;
			this.arrowSlideSound.Play();
			this.DoHandednessCheck();
		}

		private void EvaluateHandedness()
		{
			if (this.hand.handType == SteamVR_Input_Sources.LeftHand)
			{
				if (this.possibleHandSwitch && this.currentHandGuess == Longbow.Handedness.Left)
				{
					this.possibleHandSwitch = false;
				}
				if (!this.possibleHandSwitch && this.currentHandGuess == Longbow.Handedness.Right)
				{
					this.possibleHandSwitch = true;
					this.timeOfPossibleHandSwitch = Time.time;
				}
				if (this.possibleHandSwitch && Time.time > this.timeOfPossibleHandSwitch + this.timeBeforeConfirmingHandSwitch)
				{
					this.currentHandGuess = Longbow.Handedness.Left;
					this.possibleHandSwitch = false;
					return;
				}
			}
			else
			{
				if (this.possibleHandSwitch && this.currentHandGuess == Longbow.Handedness.Right)
				{
					this.possibleHandSwitch = false;
				}
				if (!this.possibleHandSwitch && this.currentHandGuess == Longbow.Handedness.Left)
				{
					this.possibleHandSwitch = true;
					this.timeOfPossibleHandSwitch = Time.time;
				}
				if (this.possibleHandSwitch && Time.time > this.timeOfPossibleHandSwitch + this.timeBeforeConfirmingHandSwitch)
				{
					this.currentHandGuess = Longbow.Handedness.Right;
					this.possibleHandSwitch = false;
				}
			}
		}

		private void DoHandednessCheck()
		{
			if (this.currentHandGuess == Longbow.Handedness.Left)
			{
				this.pivotTransform.localScale = new Vector3(1f, 1f, 1f);
				return;
			}
			this.pivotTransform.localScale = new Vector3(1f, -1f, 1f);
		}

		public void ArrowInPosition()
		{
			this.DoHandednessCheck();
			if (this.nockSound != null)
			{
				this.nockSound.Play();
			}
		}

		public void ReleaseNock()
		{
			this.nocked = false;
			this.hand.HoverUnlock(base.GetComponent<Interactable>());
			base.StartCoroutine(this.ResetDrawAnim());
		}

		private void ShutDown()
		{
			if (this.hand != null && this.hand.otherHand.currentAttachedObject != null && this.hand.otherHand.currentAttachedObject.GetComponent<ItemPackageReference>() != null && this.hand.otherHand.currentAttachedObject.GetComponent<ItemPackageReference>().itemPackage == this.arrowHandItemPackage)
			{
				this.hand.otherHand.DetachObject(this.hand.otherHand.currentAttachedObject, true);
			}
		}

		private void OnHandFocusLost(Hand hand)
		{
			base.gameObject.SetActive(false);
		}

		private void OnHandFocusAcquired(Hand hand)
		{
			base.gameObject.SetActive(true);
			this.OnAttachedToHand(hand);
		}

		private void OnDetachedFromHand(Hand hand)
		{
			Object.Destroy(base.gameObject);
		}

		private void OnDestroy()
		{
			this.ShutDown();
		}

		public Longbow.Handedness currentHandGuess;

		private float timeOfPossibleHandSwitch;

		private float timeBeforeConfirmingHandSwitch = 1.5f;

		private bool possibleHandSwitch;

		public Transform pivotTransform;

		public Transform handleTransform;

		private Hand hand;

		private ArrowHand arrowHand;

		public Transform nockTransform;

		public Transform nockRestTransform;

		public bool autoSpawnArrowHand = true;

		public ItemPackage arrowHandItemPackage;

		public GameObject arrowHandPrefab;

		public bool nocked;

		public bool pulled;

		private const float minPull = 0.05f;

		private const float maxPull = 0.5f;

		private float nockDistanceTravelled;

		private float hapticDistanceThreshold = 0.01f;

		private float lastTickDistance;

		private const float bowPullPulseStrengthLow = 100f;

		private const float bowPullPulseStrengthHigh = 500f;

		private Vector3 bowLeftVector;

		public float arrowMinVelocity = 3f;

		public float arrowMaxVelocity = 30f;

		private float arrowVelocity = 30f;

		private float minStrainTickTime = 0.1f;

		private float maxStrainTickTime = 0.5f;

		private float nextStrainTick;

		private bool lerpBackToZeroRotation;

		private float lerpStartTime;

		private float lerpDuration = 0.15f;

		private Quaternion lerpStartRotation;

		private float nockLerpStartTime;

		private Quaternion nockLerpStartRotation;

		public float drawOffset = 0.06f;

		public LinearMapping bowDrawLinearMapping;

		private Vector3 lateUpdatePos;

		private Quaternion lateUpdateRot;

		public SoundBowClick drawSound;

		private float drawTension;

		public SoundPlayOneshot arrowSlideSound;

		public SoundPlayOneshot releaseSound;

		public SoundPlayOneshot nockSound;

		private SteamVR_Events.Action newPosesAppliedAction;

		public enum Handedness
		{
			Left,
			Right
		}
	}
}
