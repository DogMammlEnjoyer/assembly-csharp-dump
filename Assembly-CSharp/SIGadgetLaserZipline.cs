using System;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using UnityEngine;

public class SIGadgetLaserZipline : SIGadget, ICallBack
{
	private static void AccumulateVelocity(Vector3 desiredVelocity)
	{
		if (SIGadgetLaserZipline.s_localPlayerVelocityFrame != Time.frameCount)
		{
			SIGadgetLaserZipline.s_localPlayerVelocityFrame = Time.frameCount;
			SIGadgetLaserZipline.s_LocalPlayerAccumulatedVelocity = Vector3.zero;
			SIGadgetLaserZipline.s_LocalPlayerNumAccumulatedVelocities = 0;
		}
		SIGadgetLaserZipline.s_LocalPlayerAccumulatedVelocity += desiredVelocity;
		SIGadgetLaserZipline.s_LocalPlayerNumAccumulatedVelocities++;
		GTPlayer.Instance.SetVelocity(SIGadgetLaserZipline.s_LocalPlayerAccumulatedVelocity / (float)SIGadgetLaserZipline.s_LocalPlayerNumAccumulatedVelocities);
	}

	private static void ResetLocalAppliedPositionOffset()
	{
		if (SIGadgetLaserZipline.s_LocalPlayerPositionFrame != Time.frameCount)
		{
			SIGadgetLaserZipline.s_LocalPlayerPositionFrame = Time.frameCount;
			SIGadgetLaserZipline.s_LocalPlayerAccumulatedPositionOffset = Vector3.zero;
			SIGadgetLaserZipline.s_LocalPlayerAppliedPositionOffset = Vector3.zero;
			return;
		}
		GTPlayer.Instance.transform.position -= SIGadgetLaserZipline.s_LocalPlayerAppliedPositionOffset;
	}

	private static void ReapplyPositionOffset()
	{
		GTPlayer.Instance.transform.position += SIGadgetLaserZipline.s_LocalPlayerAppliedPositionOffset;
	}

	private static void AccumulateAndApplyLocalPositionOffset(Vector3 offset)
	{
		SIGadgetLaserZipline.s_LocalPlayerAccumulatedPositionOffset += offset;
		SIGadgetLaserZipline.s_LocalPlayerNumAccumulatedVelocities++;
		SIGadgetLaserZipline.s_LocalPlayerAppliedPositionOffset = SIGadgetLaserZipline.s_LocalPlayerAccumulatedPositionOffset / (float)SIGadgetLaserZipline.s_LocalPlayerNumAccumulatedVelocities;
		GTPlayer.Instance.transform.position += SIGadgetLaserZipline.s_LocalPlayerAppliedPositionOffset;
	}

	private void Awake()
	{
		this.m_buttonActivatable = base.GetComponent<GameButtonActivatable>();
		this.laserBeam.SetActive(false);
		this.laserBeam.transform.SetParent(null);
		this.groundedCooldown = new ResettableUseCounter(1, this.maxSuperchargeUses, new Action<bool>(this.ShowReady));
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this.OnSnapped));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this.OnReleased));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this.OnUnsnapped));
		this.gameEntity.OnStateChanged += this.OnEntityStateChanged;
	}

	private void ClearCallback()
	{
		if (this.hasActiveCallback)
		{
			this.activeCallbackOnRig.RemoveLateUpdateCallback(this);
			this.activeCallbackOnRig = null;
			this.hasActiveCallback = false;
			SIPlayer.LocalPlayer.OnKnockback -= this.OnKnockback;
		}
	}

	private void ShowReady(bool isReady)
	{
		if (isReady)
		{
			this.audioRecharged.Play();
		}
	}

	private void OnDestroy()
	{
		this.ClearCallback();
		this.laserBeam.gameObject.Destroy();
	}

	private void OnGrabbed()
	{
	}

	private void OnSnapped()
	{
	}

	private void OnReleased()
	{
		this.wasTriggerPressed = false;
		this.laserBeam.SetActive(false);
		this.ClearCallback();
	}

	private void OnUnsnapped()
	{
		this.wasTriggerPressed = false;
		this.laserBeam.SetActive(false);
		this.ClearCallback();
	}

	protected override void OnUpdateAuthority(float dt)
	{
		bool flag = this.m_buttonActivatable.CheckInput(0.25f);
		bool flag2 = GTPlayer.Instance.IsGroundedButt || GTPlayer.Instance.IsGroundedHand || GTPlayer.Instance.IsTentacleActive;
		if (flag2)
		{
			this.groundedCooldown.Reset();
		}
		if (flag)
		{
			if (this.isLineBroken)
			{
				return;
			}
			if (flag2)
			{
				if (this.wasSlidingUngrounded)
				{
					this.isLineBroken = true;
					this.laserBeam.SetActive(false);
					this.gameEntity.RequestState(this.gameEntity.id, this.GetStateLong());
					return;
				}
			}
			else
			{
				this.wasSlidingUngrounded = true;
			}
			if (!this.wasTriggerPressed)
			{
				if (Time.time < this.coolingDownUntilTimestamp)
				{
					this.isLineBroken = true;
					this.laserBeam.SetActive(false);
					return;
				}
				if (this.cooldownOnUseUntilTouchGround && !this.groundedCooldown.TryUse())
				{
					this.isLineBroken = true;
					this.laserBeam.SetActive(false);
					return;
				}
				SuperInfectionManager activeSuperInfectionManager = SuperInfectionManager.activeSuperInfectionManager;
				if (activeSuperInfectionManager == null || !activeSuperInfectionManager.IsSupercharged)
				{
					this.onUseAudio.PlayOneShot(this.audioSingleUse);
				}
				else
				{
					this.onUseAudio.PlayOneShot(this.groundedCooldown.IsReady ? this.audioReusable : this.audioUsedUp);
				}
				this.laserBeam.SetActive(true);
				this.laserBeam.transform.localPosition = Vector3.zero;
				VRRig.LocalRig.AddLateUpdateCallback(this);
				SIPlayer.LocalPlayer.OnKnockback += this.OnKnockback;
				this.activeCallbackOnRig = VRRig.LocalRig;
				this.hasActiveCallback = true;
				this.activatedAtPoint = this.zipline.transform.TransformPoint(this.ziplineAnchorOffset);
				this.ziplineDirection = this.zipline.transform.forward;
				if (this.ziplineDirection.y > 0f)
				{
					this.ziplineDirection = -this.ziplineDirection;
				}
				if (this.ziplineDirection.y > -0.5f)
				{
					this.ziplineDirection.y = 0f;
					this.ziplineDirection.Normalize();
					this.ziplineDirection.y = -0.5f;
					this.ziplineDirection.Normalize();
				}
				this.activatedAtRotation = Quaternion.LookRotation(this.ziplineDirection);
				this.wasTriggerPressed = true;
				this.wasSlidingUngrounded = (!GTPlayer.Instance.IsGroundedButt && !GTPlayer.Instance.IsGroundedHand);
				this.gameEntity.RequestState(this.gameEntity.id, this.GetStateLong());
			}
			Vector3 rigidbodyVelocity = GTPlayer.Instance.RigidbodyVelocity;
			GTPlayer.Instance.LaserZiplineActiveAtFrame = Time.frameCount + 1;
			float magnitude = rigidbodyVelocity.magnitude;
			float num = Vector3.Dot(GTPlayer.Instance.RigidbodyVelocity, this.ziplineDirection);
			if (this._speedBoost > 0f && num < this.speedBoostVelocityCap)
			{
				num += Time.deltaTime * this._speedBoost;
			}
			SIGadgetLaserZipline.AccumulateVelocity(this.ziplineDirection * num);
			this.UpdateAudioPitch(magnitude);
			this.wasTriggerPressed = true;
			return;
		}
		else
		{
			if (this.wasTriggerPressed)
			{
				this.laserBeam.SetActive(false);
				this.zipline.transform.localRotation = Quaternion.identity;
				this.isLineBroken = false;
				this.wasTriggerPressed = false;
				this.wasSlidingUngrounded = false;
				this.coolingDownUntilTimestamp = Time.time + this.cooldownDuration;
				float d = Vector3.Dot(GTPlayer.Instance.RigidbodyVelocity, this.ziplineDirection);
				SIGadgetLaserZipline.AccumulateVelocity(this.ziplineDirection * d);
				bool isLeftHand;
				if (base.FindAttachedHand(out isLeftHand))
				{
					GorillaVelocityTracker interactPointVelocityTracker = GTPlayer.Instance.GetInteractPointVelocityTracker(isLeftHand);
					float scale = GTPlayer.Instance.scale;
					Vector3 vector = GTPlayer.Instance.turnParent.transform.rotation * -interactPointVelocityTracker.GetAverageVelocity(false, 0.1f, true) * scale;
					vector = Vector3.ClampMagnitude(vector, 5.5f * scale);
					GTPlayer.Instance.AddForce(vector, ForceMode.VelocityChange);
				}
				this.gameEntity.RequestState(this.gameEntity.id, this.GetStateLong());
				return;
			}
			this.isLineBroken = false;
			Vector3 forward = this.zipline.parent.forward;
			if (Mathf.Abs(forward.y) < 0.5f)
			{
				forward.y = 0f;
				forward.Normalize();
				forward.y = -0.5f;
			}
			Quaternion b = this.zipline.parent.InverseTransformRotation(Quaternion.LookRotation(forward));
			this.zipline.transform.localRotation = Quaternion.Lerp(this.zipline.transform.localRotation, b, Time.deltaTime * 25f);
			return;
		}
	}

	private long GetStateLong()
	{
		if (this.wasTriggerPressed && !this.isLineBroken)
		{
			return BitPackUtils.PackAnchoredPosRotForNetwork(this.activatedAtPoint, this.activatedAtRotation);
		}
		return 0L;
	}

	protected override void OnUpdateRemote(float dt)
	{
		if (this.laserBeam.activeSelf && this.activeCallbackOnRig != null)
		{
			this.UpdateAudioPitch(this.activeCallbackOnRig.LatestVelocity().magnitude);
		}
	}

	private void OnKnockback(Vector3 knockbackVector)
	{
		if (this.wasTriggerPressed)
		{
			this.isLineBroken = true;
			this.laserBeam.SetActive(false);
		}
	}

	private void OnEntityStateChanged(long oldState, long newState)
	{
		if (this.IsEquippedLocal() || this.activatedLocally)
		{
			return;
		}
		if (newState != 0L)
		{
			int attachedPlayerActorNr = this.gameEntity.AttachedPlayerActorNr;
			GamePlayer gamePlayer;
			if (attachedPlayerActorNr >= 1 && GamePlayer.TryGetGamePlayer(attachedPlayerActorNr, out gamePlayer))
			{
				Vector3 vector;
				Quaternion rotation;
				BitPackUtils.UnpackAnchoredPosRotForNetwork(newState, gamePlayer.rig.transform.position, out vector, out rotation);
				this.activatedAtPoint = vector;
				this.activatedAtRotation = rotation;
				this.ziplineDirection = rotation * Vector3.forward;
				this.laserBeam.SetActive(true);
				gamePlayer.rig.AddLateUpdateCallback(this);
				this.activeCallbackOnRig = gamePlayer.rig;
				this.hasActiveCallback = true;
				this.wasTriggerPressed = true;
				this.isLineBroken = false;
				return;
			}
		}
		else
		{
			this.wasTriggerPressed = false;
			this.isLineBroken = false;
			this.laserBeam.SetActive(false);
			this.ClearCallback();
		}
	}

	public void CallBack()
	{
		if (!this.wasTriggerPressed || this.isLineBroken)
		{
			this.ClearCallback();
			return;
		}
		if (this.IsEquippedLocal())
		{
			SIGadgetLaserZipline.ResetLocalAppliedPositionOffset();
			Vector3 vector = this.activatedAtPoint - this.zipline.transform.TransformPoint(this.ziplineAnchorOffset);
			vector = vector.ProjectOnPlane(Vector3.zero, this.ziplineDirection);
			if (vector.sqrMagnitude > 1f)
			{
				this.isLineBroken = true;
				this.laserBeam.SetActive(false);
				SIGadgetLaserZipline.ReapplyPositionOffset();
				return;
			}
			SIGadgetLaserZipline.AccumulateAndApplyLocalPositionOffset(vector);
		}
		this.zipline.transform.rotation = this.activatedAtRotation;
		Vector3 position = this.activatedAtPoint + Vector3.Project(this.zipline.transform.TransformPoint(this.ziplineAnchorOffset) - this.activatedAtPoint, this.ziplineDirection);
		this.laserBeam.transform.position = position;
		this.laserBeam.transform.rotation = this.activatedAtRotation;
	}

	private void UpdateAudioPitch(float playerSpeed)
	{
		this.laserBeamAudio.pitch = 1f + playerSpeed / 30f;
	}

	public override void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
		this._speedBoost = (withUpgrades.Contains(SIUpgradeType.AirControl_Zipline_Speed) ? this.upgradedSpeedBoost : 0f);
	}

	[SerializeField]
	private GameButtonActivatable m_buttonActivatable;

	[SerializeField]
	private Transform zipline;

	[SerializeField]
	private Vector3 ziplineAnchorOffset;

	[SerializeField]
	private GameObject laserBeam;

	[SerializeField]
	private AudioSource laserBeamAudio;

	[SerializeField]
	private AudioSource onUseAudio;

	[SerializeField]
	private float cooldownDuration;

	[SerializeField]
	private bool cooldownOnUseUntilTouchGround;

	[SerializeField]
	private int maxSuperchargeUses = 2;

	[SerializeField]
	private AudioClip audioSingleUse;

	[SerializeField]
	private AudioClip audioReusable;

	[SerializeField]
	private AudioClip audioUsedUp;

	[SerializeField]
	private SoundBankPlayer audioRecharged;

	[Header("Upgrades")]
	[SerializeField]
	private float upgradedSpeedBoost = 5f;

	[SerializeField]
	private float speedBoostVelocityCap = 10f;

	private bool hasActiveCallback;

	private VRRig activeCallbackOnRig;

	private bool wasTriggerPressed;

	private bool isLineBroken;

	private bool wasSlidingUngrounded;

	private Quaternion activatedAtRotation;

	private Vector3 activatedAtPoint;

	private Vector3 ziplineDirection;

	private float coolingDownUntilTimestamp;

	private ResettableUseCounter groundedCooldown;

	private float _speedBoost;

	private static int s_localPlayerVelocityFrame = -1;

	private static Vector3 s_LocalPlayerAccumulatedVelocity;

	private static int s_LocalPlayerNumAccumulatedVelocities;

	private static int s_LocalPlayerPositionFrame = -1;

	private static Vector3 s_LocalPlayerAccumulatedPositionOffset;

	private static Vector3 s_LocalPlayerAppliedPositionOffset;
}
