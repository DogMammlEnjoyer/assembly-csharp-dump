using System;
using UnityEngine;

public class SIGadgetChargeBlaster : MonoBehaviour, SIGadgetBlasterType
{
	private bool CheckInput()
	{
		return this.blaster.CheckInput();
	}

	private void OnEnable()
	{
		this.blaster = base.GetComponent<SIGadgetBlaster>();
		this.currentCharge = 0f;
	}

	public void OnUpdateAuthority(float dt)
	{
		switch (this.blaster.currentState)
		{
		case SIGadgetBlasterState.Idle:
			if (this.CheckInput())
			{
				this.FireProjectile(0f, this.blaster.NextFireId(), this.blaster.firingPosition.position, this.blaster.firingPosition.rotation);
				this.blaster.SetStateAuthority(SIGadgetBlasterState.Charging);
				return;
			}
			break;
		case SIGadgetBlasterState.Charging:
			this.currentCharge += this.chargeRatePerSecond * Time.deltaTime;
			this.UpdateChargingVisuals();
			if (this.CheckInput())
			{
				this.blaster.FireProjectileHaptics(this.chargeLevels[this.CurrentBlasterChargeLevel()].chargingHapticStrength, Time.fixedDeltaTime);
				return;
			}
			if (this.CurrentBlasterChargeLevel() > 0)
			{
				this.FireProjectile(this.currentCharge, this.blaster.NextFireId(), this.blaster.firingPosition.position, this.blaster.firingPosition.rotation);
				this.blaster.SetStateAuthority(SIGadgetBlasterState.Cooldown);
				return;
			}
			this.blaster.SetStateAuthority(SIGadgetBlasterState.Idle);
			return;
		case SIGadgetBlasterState.Cooldown:
			if (Time.time >= this.blaster.lastFired + this.fireCooldown)
			{
				if (this.CheckInput())
				{
					this.blaster.SetStateAuthority(SIGadgetBlasterState.Charging);
					return;
				}
				this.blaster.SetStateAuthority(SIGadgetBlasterState.Idle);
			}
			break;
		default:
			return;
		}
	}

	public void OnUpdateRemote(float dt)
	{
		switch (this.blaster.currentState)
		{
		case SIGadgetBlasterState.Idle:
		case SIGadgetBlasterState.Cooldown:
			break;
		case SIGadgetBlasterState.Charging:
			this.currentCharge += this.chargeRatePerSecond * Time.deltaTime;
			this.UpdateChargingVisuals();
			break;
		default:
			return;
		}
	}

	public void SetStateShared()
	{
		switch (this.blaster.currentState)
		{
		case SIGadgetBlasterState.Idle:
			this.currentCharge = 0f;
			break;
		case SIGadgetBlasterState.Charging:
			this.currentCharge = 0f;
			this.blaster.blasterSource.clip = this.chargingClip;
			this.blaster.blasterSource.volume = this.chargeLevels[0].chargingVolume;
			this.blaster.blasterSource.loop = true;
			this.blaster.blasterSource.Play();
			break;
		case SIGadgetBlasterState.Cooldown:
			this.blaster.blasterSource.Stop();
			if (Time.time > this.blaster.lastFired + this.fireCooldown)
			{
				this.blaster.lastFired = Time.time;
			}
			break;
		}
		this.UpdateChargingVisuals();
	}

	public void FireProjectile(float firedAtChargeLevel, int fireId, Vector3 position, Quaternion rotation)
	{
		if (this.blaster.projectileCount > this.blaster.maxProjectileCount)
		{
			return;
		}
		if (this.blaster.LocalEquippedOrActivated)
		{
			if (Time.time < this.blaster.lastFired + this.fireCooldown)
			{
				return;
			}
			this.blaster.SendClientToClientRPC(0, new object[]
			{
				firedAtChargeLevel,
				fireId,
				position,
				rotation
			});
		}
		if (Mathf.Abs(this.currentCharge - firedAtChargeLevel) <= this.maxChargeDiff)
		{
			this.currentCharge = firedAtChargeLevel;
		}
		int num = this.CurrentBlasterChargeLevel();
		this.blaster.firingSource.clip = this.chargeLevels[num].firingClip;
		this.blaster.firingSource.volume = this.chargeLevels[num].firingVolume;
		this.chargeLevels[num].fireFX.Play();
		SIGadgetBlasterProjectile projectilePrefab = this.chargeLevels[num].projectilePrefab;
		this.blaster.firingSource.time = 0f;
		this.blaster.firingSource.Play();
		this.blaster.firingSource.loop = false;
		if (this.blaster.LocalEquippedOrActivated)
		{
			this.blaster.FireProjectileHaptics(this.chargeLevels[num].firingHapticStrength, this.chargeLevels[num].firingHapticDuration);
		}
		this.currentCharge = 0f;
		this.blaster.InstantiateProjectile(projectilePrefab, position, rotation, fireId);
	}

	private void UpdateChargingVisuals()
	{
		bool flag = this.blaster.currentState == SIGadgetBlasterState.Charging;
		int num = this.CurrentBlasterChargeLevel();
		for (int i = 0; i < this.chargeLevels.Length; i++)
		{
			bool flag2 = flag && i == num;
			if (this.chargeLevels[i].chargingFX.activeSelf != flag2)
			{
				this.chargeLevels[i].chargingFX.SetActive(flag2);
			}
		}
		if (this.blaster.blasterSource.clip != this.chargingClip)
		{
			this.blaster.blasterSource.clip = this.chargingClip;
		}
		this.blaster.blasterSource.volume = this.chargeLevels[num].chargingVolume;
		if (!flag && this.blaster.blasterSource.isPlaying)
		{
			this.blaster.blasterSource.Stop();
		}
	}

	public void NetworkFireProjectile(object[] data)
	{
		if (data == null || data.Length != 4)
		{
			return;
		}
		float num;
		if (!GameEntityManager.ValidateDataType<float>(data[0], out num))
		{
			return;
		}
		if (float.IsNaN(num) || float.IsInfinity(num))
		{
			return;
		}
		int fireId;
		if (!GameEntityManager.ValidateDataType<int>(data[1], out fireId))
		{
			return;
		}
		Vector3 vector;
		if (!GameEntityManager.ValidateDataType<Vector3>(data[2], out vector))
		{
			return;
		}
		if (!vector.IsFinite())
		{
			return;
		}
		Quaternion rotation;
		if (!GameEntityManager.ValidateDataType<Quaternion>(data[3], out rotation))
		{
			return;
		}
		if ((vector - this.blaster.firingPosition.position).magnitude > this.blaster.maxLagDistance)
		{
			return;
		}
		if (this.blaster.CurrentFireRate() > 1f / this.fireCooldown * (1f + this.fireRateGracePercentage))
		{
			return;
		}
		this.FireProjectile(num, fireId, vector, rotation);
	}

	public void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
	}

	public int CurrentBlasterChargeLevel()
	{
		int result = -1;
		for (int i = 0; i < this.chargeLevels.Length; i++)
		{
			if (this.currentCharge < this.chargeLevels[i].chargeThreshold)
			{
				return result;
			}
			result = i;
		}
		return result;
	}

	[SerializeField]
	private float fireCooldown = 0.2f;

	[SerializeField]
	private float chargeRatePerSecond = 20f;

	public float fireRateGracePercentage = 0.25f;

	public float maxChargeDiff = 5f;

	private float currentCharge;

	public AudioClip chargingClip;

	public SIGadgetChargeBlaster.BlasterChargeLevel[] chargeLevels;

	private SIGadgetBlaster blaster;

	[Serializable]
	public struct BlasterChargeLevel
	{
		public float chargeThreshold;

		public float chargingVolume;

		public float firingVolume;

		public float chargingHapticStrength;

		public float firingHapticStrength;

		public float firingHapticDuration;

		public AudioClip firingClip;

		public ParticleSystem fireFX;

		public GameObject chargingFX;

		public SIGadgetBlasterProjectile projectilePrefab;
	}
}
