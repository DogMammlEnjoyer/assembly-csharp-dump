using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

public class SIGadgetLaserZipline : SIGadget, ICallBack
{
	private void Awake()
	{
		this.m_buttonActivatable = base.GetComponent<GameButtonActivatable>();
		this.laserBeam.SetActive(false);
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

	private void OnDestroy()
	{
		this.ClearCallback();
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
		this.ClearCallback();
	}

	private void OnUnsnapped()
	{
		this.wasTriggerPressed = false;
		this.ClearCallback();
	}

	protected override void OnUpdateAuthority(float dt)
	{
		bool flag = this.m_buttonActivatable.CheckInput(true, true, 0.25f, true, true);
		bool flag2 = GTPlayer.Instance.IsGroundedButt || GTPlayer.Instance.IsGroundedHand || GTPlayer.Instance.IsTentacleActive;
		if (this.coolingDownUntilNextTouchGround && flag2)
		{
			this.coolingDownUntilNextTouchGround = false;
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
				if (Time.time < this.coolingDownUntilTimestamp || this.coolingDownUntilNextTouchGround)
				{
					this.isLineBroken = true;
					this.laserBeam.SetActive(false);
					return;
				}
				this.laserBeam.SetActive(true);
				this.laserBeam.transform.localPosition = Vector3.zero;
				VRRig.LocalRig.AddLateUpdateCallback(this);
				SIPlayer.LocalPlayer.OnKnockback += this.OnKnockback;
				this.activeCallbackOnRig = VRRig.LocalRig;
				this.hasActiveCallback = true;
				this.activatedAtPoint = this.zipline.transform.position;
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
			if (num < 0f)
			{
				GTPlayer.Instance.SetVelocity(this.ziplineDirection * num);
			}
			else
			{
				float d = Mathf.Lerp(num, magnitude, 0.5f) - this.speedBoost * this.ziplineDirection.y * Time.deltaTime;
				GTPlayer.Instance.SetVelocity(this.ziplineDirection * d);
			}
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
				this.coolingDownUntilNextTouchGround = this.cooldownOnUseUntilTouchGround;
				GTPlayer.Instance.SetVelocity(GTPlayer.Instance.AveragedVelocity);
				this.gameEntity.RequestState(this.gameEntity.id, this.GetStateLong());
				return;
			}
			this.isLineBroken = false;
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
			int attachedPlayerActorNumber = base.GetAttachedPlayerActorNumber();
			GamePlayer gamePlayer;
			if (attachedPlayerActorNumber >= 1 && GamePlayer.TryGetGamePlayer(attachedPlayerActorNumber, out gamePlayer))
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
			Vector3 vector = this.activatedAtPoint - this.zipline.transform.position;
			vector = vector.ProjectOnPlane(Vector3.zero, this.ziplineDirection);
			if (vector.sqrMagnitude > 1f)
			{
				this.isLineBroken = true;
				this.laserBeam.SetActive(false);
				return;
			}
			GTPlayer.Instance.transform.position += vector;
		}
		this.zipline.transform.rotation = this.activatedAtRotation;
		Vector3 position = this.activatedAtPoint + Vector3.Project(this.zipline.transform.position - this.activatedAtPoint, this.ziplineDirection);
		this.laserBeam.transform.position = position;
	}

	[SerializeField]
	private GameButtonActivatable m_buttonActivatable;

	[SerializeField]
	private Transform zipline;

	[SerializeField]
	private GameObject laserBeam;

	[SerializeField]
	private float speedBoost;

	[SerializeField]
	private float cooldownDuration;

	[SerializeField]
	private bool cooldownOnUseUntilTouchGround;

	private bool hasActiveCallback;

	private VRRig activeCallbackOnRig;

	private bool wasTriggerPressed;

	private bool isLineBroken;

	private bool wasSlidingUngrounded;

	private Quaternion activatedAtRotation;

	private Vector3 activatedAtPoint;

	private Vector3 ziplineDirection;

	private float coolingDownUntilTimestamp;

	private bool coolingDownUntilNextTouchGround;
}
