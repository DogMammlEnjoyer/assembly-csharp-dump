using System;
using UnityEngine;

[RequireComponent(typeof(GameTriggerInteractable))]
public class SIGadgetPumpBlaster : MonoBehaviour, SIGadgetBlasterType
{
	private bool CheckInput()
	{
		return this.blaster.CheckInput();
	}

	private void OnEnable()
	{
		this.blaster = base.GetComponent<SIGadgetBlaster>();
		this.triggerInteractable = base.GetComponent<GameTriggerInteractable>();
		this.strokeLength = (this.pumpFullyClosed.position - this.pumpFullyOpen.position).magnitude;
	}

	public void OnUpdateAuthority(float dt)
	{
		SIGadgetBlasterState currentState = this.blaster.currentState;
		if (currentState != SIGadgetBlasterState.Idle)
		{
			if (currentState != SIGadgetBlasterState.Pumping)
			{
				return;
			}
			if (!this.triggerInteractable.triggerInteractionActive)
			{
				this.blaster.SetStateAuthority(SIGadgetBlasterState.Idle);
			}
			Vector3 vector = this.pumpFullyOpen.position - this.pumpFullyClosed.position;
			Vector3 vector2 = this.pumpingTransform.position - this.pumpFullyClosed.position;
			if (Vector3.Dot(vector, vector2) < 0f)
			{
				vector2 = Vector3.zero;
			}
			Vector3 vector3 = Vector3.Project(vector2, vector);
			this.pumpHandlePosition.position = this.pumpFullyClosed.position + vector.normalized * Mathf.Clamp(vector3.magnitude, 0f, vector.magnitude);
			if (!this.pumpFullyOpened && vector3.magnitude > (1f - this.pumpThresholdPercent) * this.strokeLength)
			{
				this.pumpFullyOpened = true;
			}
			else if (this.pumpFullyOpen && vector3.magnitude < this.pumpThresholdPercent * this.strokeLength)
			{
				this.pumpFullyOpened = false;
				this.currentPumpChargeAmount = Mathf.Min(this.currentPumpChargeAmount + this.chargePerPump, this.maxPumpCharge);
			}
			if (this.CheckInput() && this.currentPumpChargeAmount > 0f)
			{
				this.AttemptFireProjectile(this.blaster.NextFireId(), this.currentPumpChargeAmount, this.blaster.firingPosition.position, this.blaster.firingPosition.rotation);
			}
		}
		else
		{
			if (this.triggerInteractable.triggerInteractionActive)
			{
				this.blaster.SetStateAuthority(SIGadgetBlasterState.Pumping);
				return;
			}
			if (this.CheckInput() && this.currentPumpChargeAmount > 0f)
			{
				this.AttemptFireProjectile(this.blaster.NextFireId(), this.currentPumpChargeAmount, this.blaster.firingPosition.position, this.blaster.firingPosition.rotation);
				return;
			}
		}
	}

	public void OnUpdateRemote(float dt)
	{
		SIGadgetBlasterState currentState = this.blaster.currentState;
		if (currentState != SIGadgetBlasterState.Idle && currentState == SIGadgetBlasterState.Pumping)
		{
			Vector3 vector = this.pumpFullyOpen.position - this.pumpFullyClosed.position;
			Vector3 vector2 = this.pumpingTransform.position - this.pumpFullyClosed.position;
			if (Vector3.Dot(vector, vector2) < 0f)
			{
				vector2 = Vector3.zero;
			}
			Vector3 vector3 = Vector3.Project(vector2, vector);
			this.pumpHandlePosition.position = this.pumpFullyClosed.position + vector.normalized * Mathf.Clamp(vector3.magnitude, 0f, vector.magnitude);
			this.currentPumpChargeAmount = Mathf.Min(this.maxPumpCharge, this.currentPumpChargeAmount + Time.deltaTime * this.remotePumpChargePerSecond);
		}
	}

	public void SetStateShared()
	{
		SIGadgetBlasterState currentState = this.blaster.currentState;
		if (currentState == SIGadgetBlasterState.Idle)
		{
			this.blaster.blasterSource.clip = this.idleClip;
			this.blaster.blasterSource.volume = this.idleVolume;
			this.pumpingTransform = null;
			return;
		}
		if (currentState != SIGadgetBlasterState.Pumping)
		{
			return;
		}
		bool flag;
		if (this.blaster.FindAttachedHand(out flag, true, true))
		{
			if (flag)
			{
				this.pumpingTransform = SIPlayer.Get(this.blaster.GetAttachedPlayerActorNumber()).gamePlayer.rightHand;
				return;
			}
			this.pumpingTransform = SIPlayer.Get(this.blaster.GetAttachedPlayerActorNumber()).gamePlayer.leftHand;
		}
	}

	public void AttemptFireProjectile(int fireId, float pumpChargeAmount, Vector3 position, Quaternion rotation)
	{
		if (pumpChargeAmount <= 0f)
		{
			return;
		}
		if (pumpChargeAmount - this.maxPumpDiff > this.currentPumpChargeAmount)
		{
			return;
		}
		if (this.blaster.projectileCount > this.blaster.maxProjectileCount)
		{
			return;
		}
		if (this.blaster.LocalEquippedOrActivated)
		{
			this.blaster.SendClientToClientRPC(0, new object[]
			{
				fireId,
				position,
				rotation
			});
		}
		this.currentPumpChargeAmount = Mathf.Min(this.maxPumpCharge, pumpChargeAmount);
		this.blaster.firingSource.time = 0f;
		this.blaster.firingSource.Play();
		this.blaster.firingSource.loop = false;
		this.blaster.InstantiateProjectile(this.projectilePrefab, position, rotation, fireId);
		this.currentPumpChargeAmount = 0f;
	}

	public void NetworkFireProjectile(object[] data)
	{
		if (data == null || data.Length != 4)
		{
			return;
		}
		int fireId;
		if (!GameEntityManager.ValidateDataType<int>(data[0], out fireId))
		{
			return;
		}
		float pumpChargeAmount;
		if (!GameEntityManager.ValidateDataType<float>(data[1], out pumpChargeAmount))
		{
			return;
		}
		Vector3 position;
		if (!GameEntityManager.ValidateDataType<Vector3>(data[2], out position))
		{
			return;
		}
		Quaternion rotation;
		if (!GameEntityManager.ValidateDataType<Quaternion>(data[3], out rotation))
		{
			return;
		}
		this.AttemptFireProjectile(fireId, pumpChargeAmount, position, rotation);
	}

	public void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
	}

	public SIGadgetBlasterProjectile projectilePrefab;

	public AudioClip idleClip;

	public AudioClip cooldownClip;

	public float idleVolume;

	public float cooldownVolume;

	public AudioClip firingClip;

	public float firingVolume;

	public ParticleSystem fireFX;

	public Transform pumpHandlePosition;

	public Transform pumpFullyClosed;

	public Transform pumpFullyOpen;

	private GameTriggerInteractable triggerInteractable;

	private SIGadgetBlaster blaster;

	private Transform pumpingTransform;

	public float currentPumpChargeAmount;

	public float maxPumpCharge = 1f;

	public float remotePumpChargePerSecond = 2f;

	public float maxPumpDiff = 0.5f;

	private float chargePerPump = 1f;

	private bool pumpFullyOpened;

	private float pumpThresholdPercent = 0.1f;

	private float strokeLength;
}
