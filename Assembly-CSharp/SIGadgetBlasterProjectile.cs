using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SIGadgetProjectileType))]
[RequireComponent(typeof(Rigidbody))]
public class SIGadgetBlasterProjectile : MonoBehaviourTick
{
	public override void Tick()
	{
		if (Time.time > this.timeSpawned + this.maxLifetime)
		{
			this.parentBlaster.DespawnProjectile(this);
		}
	}

	public void InitializeProjectile()
	{
		this.rb.angularVelocity = Vector3.zero;
		this.rb.linearVelocity = base.transform.forward * this.startingVelocity;
		this.timeSpawned = Time.realtimeSinceStartup;
		if (this.audioSource == null)
		{
			this.audioSource = base.GetComponentInChildren<AudioSource>();
		}
		this.audioSource.time = 0f;
		this.projectileType = base.GetComponent<SIGadgetProjectileType>();
		SIGadgetProjectileModifier[] components = base.GetComponents<SIGadgetProjectileModifier>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].ModifyProjectile(this);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<SIExclusionZone>() != null && Time.realtimeSinceStartup > this.timeSpawned + 0.02f)
		{
			if (this.exclusionZoneDespawnEffect != null)
			{
				SIGadgetBlasterProjectile.SpawnExplosion(this.exclusionZoneDespawnEffect, base.transform.position, base.transform.rotation);
			}
			this.DespawnProjectile();
			return;
		}
		SIPlayer componentInParent = other.GetComponentInParent<SIPlayer>();
		if (componentInParent == null)
		{
			return;
		}
		if (componentInParent == this.firedByPlayer)
		{
			return;
		}
		if (this.firedByPlayer != SIPlayer.LocalPlayer || componentInParent == SIPlayer.LocalPlayer)
		{
			return;
		}
		this.projectileType.LocalProjectileHit(componentInParent);
	}

	private void OnCollisionEnter(Collision collision)
	{
		this.projectileType.LocalProjectileHit(null);
		HitTargetNetworkState hitTargetNetworkState;
		if (collision.collider.gameObject.TryGetComponent<HitTargetNetworkState>(out hitTargetNetworkState))
		{
			hitTargetNetworkState.TargetHit((Time.time - this.timeSpawned) * this.startingVelocity * -base.transform.forward + base.transform.position, base.transform.position);
		}
	}

	public void DespawnProjectile()
	{
		this.parentBlaster.DespawnProjectile(this);
	}

	public void KnockbackWithHaptics(Vector3 directionAndMagnitude, bool adjustForDirection = true)
	{
		this.KnockbackWithHaptics(directionAndMagnitude, this.hapticHitStrength, this.hapticHitDuration, adjustForDirection);
	}

	public void KnockbackWithHaptics(Vector3 directionAndMagnitude, float hapticStrength, float hapticDuration, bool adjustForDirection = true)
	{
		SIPlayer.LocalPlayer.PlayerKnockback(directionAndMagnitude, true, true);
		SIPlayer.LocalPlayer.NotifyBlasterHit();
		if (adjustForDirection)
		{
			Vector3 from = GorillaTagger.Instance.leftHandTransform.position - GorillaTagger.Instance.bodyCollider.transform.position;
			Vector3 from2 = GorillaTagger.Instance.rightHandTransform.position - GorillaTagger.Instance.bodyCollider.transform.position;
			float num = 0.5f;
			float num2 = 45f;
			float num3 = Vector3.Angle(from, directionAndMagnitude);
			float num4 = Vector3.Angle(from2, directionAndMagnitude);
			float hapticStrength2 = (1f - Mathf.Max(num3 - num2, 0f) / (180f - num2)) * num + (1f - num);
			float hapticStrength3 = (1f - Mathf.Max(num4 - num2, 0f) / (180f - num2)) * num + (1f - num);
			SIPlayer.LocalPlayer.PlayerHandHaptic(true, hapticStrength2, hapticDuration, true);
			SIPlayer.LocalPlayer.PlayerHandHaptic(false, hapticStrength3, hapticDuration, true);
			return;
		}
		SIPlayer.LocalPlayer.PlayerHandHaptic(true, hapticStrength, hapticDuration, true);
		SIPlayer.LocalPlayer.PlayerHandHaptic(false, hapticStrength, hapticDuration, true);
	}

	public static GameObject SpawnExplosion(GameObject explosionPrefab, Vector3 position, Quaternion rotation)
	{
		if (SIGadgetBlasterProjectile.blasterProjectileExplosionPools == null)
		{
			SIGadgetBlasterProjectile.blasterProjectileExplosionPools = new Dictionary<int, List<GameObject>>();
		}
		if (SIGadgetBlasterProjectile.explosionTypeKey == null)
		{
			SIGadgetBlasterProjectile.explosionTypeKey = new Dictionary<GameObject, int>();
		}
		int instanceID = explosionPrefab.GetInstanceID();
		if (!SIGadgetBlasterProjectile.blasterProjectileExplosionPools.ContainsKey(instanceID))
		{
			SIGadgetBlasterProjectile.blasterProjectileExplosionPools.Add(instanceID, new List<GameObject>());
		}
		List<GameObject> list = SIGadgetBlasterProjectile.blasterProjectileExplosionPools[instanceID];
		GameObject gameObject;
		if (list.Count <= 0)
		{
			gameObject = Object.Instantiate<GameObject>(explosionPrefab, position, rotation);
			SIGadgetBlasterProjectile.explosionTypeKey.Add(gameObject, instanceID);
		}
		else
		{
			gameObject = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);
			gameObject.SetActive(true);
		}
		gameObject.transform.position = position;
		gameObject.transform.rotation = rotation;
		return gameObject;
	}

	public static void DespawnExplosion(GameObject explosion)
	{
		SIGadgetBlasterProjectile.blasterProjectileExplosionPools[SIGadgetBlasterProjectile.explosionTypeKey[explosion]].Add(explosion);
		explosion.SetActive(false);
	}

	[OnEnterPlay_SetNull]
	public static Dictionary<int, List<GameObject>> blasterProjectileExplosionPools;

	[OnEnterPlay_SetNull]
	public static Dictionary<GameObject, int> explosionTypeKey;

	[NonSerialized]
	public int poolId;

	public SIGadgetProjectileType projectileType;

	public Rigidbody rb;

	public GameObject hitEffect;

	public GameObject hitEffectPlayer;

	public float maxLifetime = 10f;

	[NonSerialized]
	public float timeSpawned;

	public float hapticHitStrength = 0.75f;

	public float hapticHitDuration = 0.1f;

	[NonSerialized]
	public SIGadgetBlaster parentBlaster;

	[NonSerialized]
	public int projectileId;

	[NonSerialized]
	public SIPlayer firedByPlayer;

	public float startingVelocity;

	public const float EXCLUSION_ZONE_MINIMUM_LIFETIME = 0.02f;

	public GameObject exclusionZoneDespawnEffect;

	private AudioSource audioSource;
}
