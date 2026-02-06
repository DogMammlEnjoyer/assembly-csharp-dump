using System;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;

public class CrankableToyCarHoldable : TransferrableObject
{
	protected override void Start()
	{
		base.Start();
		this.crank.SetOnCrankedCallback(new Action<float>(this.OnCranked));
	}

	internal override void OnEnable()
	{
		base.OnEnable();
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (this._events == null)
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
		}
		NetPlayer netPlayer = (base.myOnlineRig != null) ? base.myOnlineRig.creator : ((base.myRig != null) ? ((base.myRig.creator != null) ? base.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null);
		if (netPlayer != null && this._events != null)
		{
			this._events.Init(netPlayer);
			this._events.Activate += this.OnDeployRPC;
		}
		else
		{
			Debug.LogError("Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
		}
		this.itemState &= (TransferrableObject.ItemStates)(-2);
	}

	internal override void OnDisable()
	{
		base.OnDisable();
		if (this._events != null)
		{
			this._events.Dispose();
		}
	}

	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
		if (this.itemState.HasFlag(TransferrableObject.ItemStates.State0))
		{
			if (!this.deployablePart.activeSelf)
			{
				this.OnCarDeployed();
				return;
			}
		}
		else if (this.deployablePart.activeSelf)
		{
			this.OnCarReturned();
		}
	}

	private void OnCranked(float deltaAngle)
	{
		this.currentCrankStrength += Mathf.Abs(deltaAngle);
		this.currentCrankClickAmount += deltaAngle;
		if (Mathf.Abs(this.currentCrankClickAmount) > this.crankAnglePerClick)
		{
			if (this.currentCrankStrength >= this.maxCrankStrength)
			{
				this.overCrankedSound.Play();
				VRRig ownerRig = this.ownerRig;
				if (ownerRig != null && ownerRig.isLocal)
				{
					GorillaTagger.Instance.StartVibration(base.InRightHand(), this.overcrankHapticStrength, this.overcrankHapticDuration);
				}
			}
			else
			{
				float value = Mathf.Lerp(this.minClickPitch, this.maxClickPitch, Mathf.InverseLerp(0f, this.maxCrankStrength, this.currentCrankStrength));
				SoundBankPlayer soundBankPlayer = this.clickSound;
				float? pitchOverride = new float?(value);
				soundBankPlayer.Play(null, pitchOverride);
				VRRig ownerRig2 = this.ownerRig;
				if (ownerRig2 != null && ownerRig2.isLocal)
				{
					GorillaTagger.Instance.StartVibration(base.InRightHand(), this.crankHapticStrength, this.crankHapticDuration);
				}
			}
			this.currentCrankClickAmount = 0f;
		}
	}

	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (VRRigCache.Instance.localRig.Rig != this.ownerRig)
		{
			return false;
		}
		if (this.currentCrankStrength == 0f)
		{
			return true;
		}
		bool isLeftHand = releasingHand == EquipmentInteractor.instance.leftHand;
		GorillaVelocityTracker interactPointVelocityTracker = GTPlayer.Instance.GetInteractPointVelocityTracker(isLeftHand);
		Vector3 vector = base.transform.TransformPoint(Vector3.zero);
		Quaternion rotation = base.transform.rotation;
		Vector3 averageVelocity = interactPointVelocityTracker.GetAverageVelocity(true, 0.15f, false);
		float num = Mathf.Lerp(this.minLifetime, this.maxLifetime, Mathf.Clamp01(Mathf.InverseLerp(0f, this.maxCrankStrength, this.currentCrankStrength)));
		this.DeployCarLocal(vector, rotation, averageVelocity, num, false);
		if (PhotonNetwork.InRoom)
		{
			this._events.Activate.RaiseOthers(new object[]
			{
				BitPackUtils.PackWorldPosForNetwork(vector),
				BitPackUtils.PackQuaternionForNetwork(rotation),
				BitPackUtils.PackWorldPosForNetwork(averageVelocity * 100f),
				num
			});
		}
		this.currentCrankStrength = 0f;
		return true;
	}

	private void DeployCarLocal(Vector3 launchPos, Quaternion launchRot, Vector3 releaseVel, float lifetime, bool isRemote = false)
	{
		if (!this.disabledWhileDeployed.activeSelf)
		{
			return;
		}
		this.deployedCar.Deploy(this, launchPos, launchRot, releaseVel, lifetime, isRemote);
	}

	private void OnDeployRPC(int sender, int receiver, object[] args, PhotonMessageInfoWrapped info)
	{
		if (!this || sender != receiver || info.senderID != this.ownerRig.creator.ActorNumber)
		{
			return;
		}
		GorillaNot.IncrementRPCCall(info, "OnDeployRPC");
		Vector3 launchPos = BitPackUtils.UnpackWorldPosFromNetwork((long)args[0]);
		Quaternion launchRot = BitPackUtils.UnpackQuaternionFromNetwork((int)args[1]);
		Vector3 releaseVel = BitPackUtils.UnpackWorldPosFromNetwork((long)args[2]) / 100f;
		float lifetime = (float)args[3];
		float num = 10000f;
		if (launchPos.IsValid(num) && launchRot.IsValid())
		{
			float num2 = 10000f;
			if (releaseVel.IsValid(num2))
			{
				this.DeployCarLocal(launchPos, launchRot, releaseVel, lifetime, true);
				return;
			}
		}
	}

	public void OnCarDeployed()
	{
		this.itemState |= TransferrableObject.ItemStates.State0;
		this.deployablePart.SetActive(true);
		this.disabledWhileDeployed.SetActive(false);
	}

	public void OnCarReturned()
	{
		this.itemState &= (TransferrableObject.ItemStates)(-2);
		this.deployablePart.SetActive(false);
		this.disabledWhileDeployed.SetActive(true);
		this.clickSound.RestartSequence();
	}

	[SerializeField]
	private TransferrableObjectHoldablePart_Crank crank;

	[SerializeField]
	private CrankableToyCarDeployed deployedCar;

	[SerializeField]
	private GameObject deployablePart;

	[SerializeField]
	private GameObject disabledWhileDeployed;

	[SerializeField]
	private float crankAnglePerClick;

	[SerializeField]
	private float maxCrankStrength;

	[SerializeField]
	private float minClickPitch;

	[SerializeField]
	private float maxClickPitch;

	[SerializeField]
	private float minLifetime;

	[SerializeField]
	private float maxLifetime;

	[SerializeField]
	private SoundBankPlayer clickSound;

	[SerializeField]
	private SoundBankPlayer overCrankedSound;

	[SerializeField]
	private float crankHapticStrength = 0.1f;

	[SerializeField]
	private float crankHapticDuration = 0.05f;

	[SerializeField]
	private float overcrankHapticStrength = 0.8f;

	[SerializeField]
	private float overcrankHapticDuration = 0.05f;

	private float currentCrankStrength;

	private float currentCrankClickAmount;

	private RubberDuckEvents _events;
}
