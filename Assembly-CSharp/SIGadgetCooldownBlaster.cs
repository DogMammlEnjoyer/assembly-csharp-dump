using System;
using UnityEngine;

public class SIGadgetCooldownBlaster : MonoBehaviour, SIGadgetBlasterType
{
	private bool CheckInput()
	{
		return this.blaster.CheckInput();
	}

	private void OnEnable()
	{
		this.blaster = base.GetComponent<SIGadgetBlaster>();
		this.blaster.firingSource.clip = this.firingClip;
		this.blaster.firingSource.volume = this.firingVolume;
		this.blaster.firingSource.loop = false;
		this.blaster.blasterSource.clip = this.cooldownClip;
		this.blaster.blasterSource.volume = this.cooldownVolume;
		this.blaster.blasterSource.loop = false;
	}

	public void OnUpdateAuthority(float dt)
	{
		SIGadgetBlasterState currentState = this.blaster.currentState;
		if (currentState != SIGadgetBlasterState.Idle)
		{
			if (currentState != SIGadgetBlasterState.Cooldown)
			{
				return;
			}
			if (Time.time >= this.blaster.lastFired + this.fireCooldown)
			{
				this.blaster.FireProjectileHaptics(this.availableToFireHapticStrength, 0.02f);
				this.blaster.SetStateAuthority(SIGadgetBlasterState.Idle);
			}
		}
		else
		{
			if (!this.CheckInput())
			{
				this.triggerHeldDown = false;
				return;
			}
			if (!this.triggerHeldDown)
			{
				this.triggerHeldDown = true;
				this.FireProjectile(this.blaster.NextFireId(), this.blaster.firingPosition.position, this.blaster.firingPosition.rotation);
				this.blaster.SetStateAuthority(SIGadgetBlasterState.Cooldown);
				return;
			}
		}
	}

	public void OnUpdateRemote(float dt)
	{
		SIGadgetBlasterState currentState = this.blaster.currentState;
		if (currentState != SIGadgetBlasterState.Idle)
		{
		}
	}

	public void SetStateShared()
	{
		SIGadgetBlasterState currentState = this.blaster.currentState;
		if (currentState == SIGadgetBlasterState.Idle)
		{
			this.cooldownIndicator.sharedMaterial = this.readyToFireMaterial;
			return;
		}
		if (currentState != SIGadgetBlasterState.Cooldown)
		{
			return;
		}
		this.blaster.lastFired = Time.time;
		this.cooldownIndicator.sharedMaterial = this.onCooldownMaterial;
	}

	public void FireProjectile(int fireId, Vector3 position, Quaternion rotation)
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
			this.blaster.FireProjectileHaptics(this.firingHapticStrength, this.firingHapticDuration);
			this.blaster.SendClientToClientRPC(0, new object[]
			{
				fireId,
				position,
				rotation
			});
		}
		this.blaster.firingSource.time = 0f;
		this.blaster.firingSource.Play();
		this.blaster.blasterSource.time = 0f;
		this.blaster.blasterSource.Play();
		this.blaster.InstantiateProjectile(this.projectilePrefab, position, rotation, fireId);
	}

	public void NetworkFireProjectile(object[] data)
	{
		if (data == null || data.Length != 3)
		{
			return;
		}
		int fireId;
		if (!GameEntityManager.ValidateDataType<int>(data[0], out fireId))
		{
			return;
		}
		Vector3 vector;
		if (!GameEntityManager.ValidateDataType<Vector3>(data[1], out vector))
		{
			return;
		}
		if (!vector.IsFinite())
		{
			return;
		}
		Quaternion rotation;
		if (!GameEntityManager.ValidateDataType<Quaternion>(data[2], out rotation))
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
		this.FireProjectile(fireId, vector, rotation);
	}

	public void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
	}

	public SIGadgetBlasterProjectile projectilePrefab;

	public float fireCooldown = 0.5f;

	public float fireRateGracePercentage = 0.25f;

	public float availableToFireHapticStrength = 0.1f;

	public float availableToFireHapticDuration = 0.01f;

	public float firingHapticStrength = 0.25f;

	public float firingHapticDuration = 0.01f;

	public AudioClip firingClip;

	public AudioClip cooldownClip;

	public float firingVolume;

	public float cooldownVolume;

	public ParticleSystem fireFX;

	public MeshRenderer cooldownIndicator;

	public Material readyToFireMaterial;

	public Material onCooldownMaterial;

	private bool triggerHeldDown;

	private SIGadgetBlaster blaster;
}
