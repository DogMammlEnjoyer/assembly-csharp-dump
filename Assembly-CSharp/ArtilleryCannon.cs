using System;
using GorillaTagScripts.Builder;
using Photon.Pun;
using UnityEngine;

public class ArtilleryCannon : MonoBehaviour
{
	private int LocalActorNr
	{
		get
		{
			if (PhotonNetwork.LocalPlayer == null)
			{
				return -1;
			}
			return PhotonNetwork.LocalPlayer.ActorNumber;
		}
	}

	private void Awake()
	{
		if (this.projectilePrefab != null)
		{
			this.projectileHash = PoolUtils.GameObjHashCode(this.projectilePrefab);
		}
	}

	private void OnEnable()
	{
		if (this.fireHitNotifier != null)
		{
			this.fireHitNotifier.OnProjectileHit += this.OnFireProjectileHit;
		}
		ArtilleryCannonState newState;
		if (this.stateRef.TryResolve<ArtilleryCannonState>(out newState))
		{
			this.Bind(newState);
			return;
		}
		this.stateRef.AddCallbackOnLoad(new Action(this.OnStateSceneLoaded));
	}

	private void OnDisable()
	{
		if (this.fireHitNotifier != null)
		{
			this.fireHitNotifier.OnProjectileHit -= this.OnFireProjectileHit;
		}
		this.stateRef.RemoveCallbackOnLoad(new Action(this.OnStateSceneLoaded));
		this.Unbind();
	}

	private void OnStateSceneLoaded()
	{
		ArtilleryCannonState newState;
		if (this.stateRef.TryResolve<ArtilleryCannonState>(out newState))
		{
			this.Bind(newState);
		}
	}

	private void Bind(ArtilleryCannonState newState)
	{
		if (this.state == newState)
		{
			return;
		}
		this.Unbind();
		this.state = newState;
		if (this.state == null)
		{
			return;
		}
		this.state.onRotationChanged += this.OnRotationChanged;
		this.state.onFired += this.OnFiredRemote;
		this.ApplyRotation();
	}

	private void Unbind()
	{
		if (this.state == null)
		{
			return;
		}
		this.state.onRotationChanged -= this.OnRotationChanged;
		this.state.onFired -= this.OnFiredRemote;
		this.state = null;
	}

	private void LateUpdate()
	{
		if (this.state == null)
		{
			return;
		}
		int localActorNr = this.LocalActorNr;
		if (this.pitchCrank != null && this.state.pitchCrankSync.holderActorNr == localActorNr)
		{
			this.state.UpdateLocalCrankState(0, this.pitchCrank.IsHeldLeftHand, this.pitchCrank.CurrentAngle);
		}
		if (this.yawCrank != null && this.state.yawCrankSync.holderActorNr == localActorNr)
		{
			this.state.UpdateLocalCrankState(1, this.yawCrank.IsHeldLeftHand, this.yawCrank.CurrentAngle);
		}
		this.UpdateRemoteCrankVisual(this.pitchCrank, this.state.pitchCrankSync, localActorNr);
		this.UpdateRemoteCrankVisual(this.yawCrank, this.state.yawCrankSync, localActorNr);
	}

	private void UpdateRemoteCrankVisual(ArtilleryCrank crank, ArtilleryCannonState.CrankSyncState syncState, int localActor)
	{
		if (crank == null || syncState.holderActorNr == localActor)
		{
			return;
		}
		if (syncState.holderActorNr != -1)
		{
			VRRig vrrig = ArtilleryCannonState.FindRigForActor(syncState.holderActorNr);
			if (vrrig != null)
			{
				crank.UpdateFromRemoteHand(vrrig, syncState.isLeftHand);
				return;
			}
		}
		crank.SetVisualAngle(syncState.angle);
	}

	internal bool IsCrankHeldLocally(int crankIndex)
	{
		return (ref (crankIndex == 0) ? ref this.state.pitchCrankSync : ref this.state.yawCrankSync).holderActorNr == this.LocalActorNr;
	}

	internal bool OnCrankGrabbed(int crankIndex, bool isLeftHand)
	{
		return this.state.NotifyCrankGrabbed(crankIndex, isLeftHand);
	}

	internal void OnCrankReleased(int crankIndex, float finalAngle)
	{
		this.state.NotifyCrankReleased(crankIndex, finalAngle);
	}

	internal void OnCrankInput(int crankIndex, float degrees)
	{
		this.state.NotifyCrankInput(crankIndex, degrees);
		this.ApplyRotation();
	}

	private void OnRotationChanged()
	{
		this.ApplyRotation();
	}

	private void ApplyRotation()
	{
		if (this.state == null)
		{
			return;
		}
		if (this.yawTransform != null)
		{
			this.yawTransform.localRotation = Quaternion.Euler(0f, this.state.CurrentYaw, 0f);
		}
		if (this.pitchTransform != null)
		{
			this.pitchTransform.localRotation = Quaternion.Euler(-this.state.CurrentPitch, 0f, 0f);
		}
	}

	public void Fire()
	{
		if (this.state == null)
		{
			return;
		}
		if (this.state.TryFire())
		{
			this.FireLocal();
		}
	}

	private void OnFireProjectileHit(SlingshotProjectile projectile, Collision collision)
	{
		this.Fire();
	}

	private void OnFiredRemote()
	{
		this.FireLocal();
	}

	private void FireLocal()
	{
		if (this.projectilePrefab == null || this.muzzle == null)
		{
			return;
		}
		Vector3 position = this.muzzle.position;
		Vector3 forward = this.muzzle.forward;
		GameObject gameObject = ObjectPools.instance.Instantiate(this.projectileHash, true);
		gameObject.transform.position = position;
		gameObject.transform.rotation = Quaternion.LookRotation(forward);
		BuilderProjectile component = gameObject.GetComponent<BuilderProjectile>();
		if (component != null)
		{
			component.aoeKnockbackConfig = new SlingshotProjectile.AOEKnockbackConfig?(this.knockbackConfig);
		}
		Rigidbody component2 = gameObject.GetComponent<Rigidbody>();
		if (component2 != null)
		{
			component2.linearVelocity = forward * this.launchSpeed;
		}
		if (this.fireSound != null)
		{
			this.fireSound.GTPlay();
		}
	}

	[Header("Network State")]
	[SerializeField]
	private XSceneRef stateRef;

	[Header("Cranks")]
	[SerializeField]
	private ArtilleryCrank pitchCrank;

	[SerializeField]
	private ArtilleryCrank yawCrank;

	[Header("Rotation")]
	[SerializeField]
	private Transform yawTransform;

	[SerializeField]
	private Transform pitchTransform;

	[Header("Firing")]
	[SerializeField]
	private Transform muzzle;

	[SerializeField]
	private GameObject projectilePrefab;

	[SerializeField]
	private float launchSpeed = 30f;

	[SerializeField]
	private AudioSource fireSound;

	[SerializeField]
	private SlingshotProjectile.AOEKnockbackConfig knockbackConfig;

	[Header("Fire Trigger")]
	[Tooltip("When a projectile hits this notifier, the cannon fires.")]
	[SerializeField]
	private SlingshotProjectileHitNotifier fireHitNotifier;

	private ArtilleryCannonState state;

	private int projectileHash;
}
