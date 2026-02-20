using System;
using GorillaLocomotion;
using UnityEngine;

[RequireComponent(typeof(GameGrabbable))]
[RequireComponent(typeof(GameSnappable))]
[RequireComponent(typeof(GameButtonActivatable))]
public class SIGadgetAirGrab : SIGadget
{
	private int _HandIndex
	{
		get
		{
			if ((this.m_snappable.snappedToJoint != null && this.m_snappable.snappedToJoint.jointType == SnapJointType.HandL) || this.gameEntity.heldByHandIndex == 0)
			{
				return 0;
			}
			if ((this.m_snappable.snappedToJoint != null && this.m_snappable.snappedToJoint.jointType == SnapJointType.HandR) || this.gameEntity.heldByHandIndex == 1)
			{
				return 1;
			}
			return -1;
		}
	}

	private void Awake()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this._HandleStartInteraction));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this._HandleStartInteraction));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this._HandleStopInteraction));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this._HandleStopInteraction));
		this._groundedUseCounter = new ResettableUseCounter(1, this.m_maxSuperchargeUses, new Action<bool>(this.OnRecharge));
		foreach (AudioClip audioClip in this.m_clips)
		{
			if (audioClip)
			{
				audioClip.LoadAudioData();
			}
		}
		this._grabXformInitialScale = this.m_airGrabXform.localScale;
	}

	private void OnRecharge(bool recharged)
	{
		if (recharged)
		{
			this.rechargeSound.Play();
		}
	}

	private void OnDestroy()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Remove(gameEntity.OnGrabbed, new Action(this._HandleStartInteraction));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Remove(gameEntity2.OnSnapped, new Action(this._HandleStartInteraction));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Remove(gameEntity3.OnReleased, new Action(this._HandleStopInteraction));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Remove(gameEntity4.OnUnsnapped, new Action(this._HandleStopInteraction));
	}

	private void ClearGravityOverride()
	{
		GTPlayer.Instance.UnsetGravityOverride(this);
		this.hasGravityOverride = false;
	}

	private new void OnDisable()
	{
		if (this.hasGravityOverride)
		{
			this.ClearGravityOverride();
		}
	}

	private void _HandleStartInteraction()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this._attachedPlayerActorNr = this.gameEntity.AttachedPlayerActorNr;
		this._attachedNetPlayer = NetworkSystem.Instance.GetPlayer(this._attachedPlayerActorNr);
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this._attachedPlayerActorNr, out gamePlayer))
		{
			return;
		}
		this._attachedVRRig = gamePlayer.rig;
	}

	private void _HandleStopInteraction()
	{
		if (this.hasGravityOverride)
		{
			this.ClearGravityOverride();
		}
		this._attachedPlayerActorNr = -1;
		this._attachedNetPlayer = null;
		this._attachedVRRig = null;
		this.m_airGrabXform.gameObject.SetActive(false);
		if (!this.gameEntity.IsAuthority())
		{
			return;
		}
		if (this._state == SIGadgetAirGrab.EState.DashUsed)
		{
			this.SetStateAuthority(SIGadgetAirGrab.EState.DashUsed);
			return;
		}
		this.SetStateAuthority(SIGadgetAirGrab.EState.Idle);
	}

	protected void FixedUpdate()
	{
		if ((!this.IsEquippedLocal() && !this.activatedLocally) || ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this._wasActivated = this._isActivated;
		this._isActivated = this._CheckInput();
		GTPlayer instance = GTPlayer.Instance;
		if (Time.unscaledTime < this._airGrabTime + this.m_slipperySurfacesTime)
		{
			instance.SetMaximumSlipThisFrame();
		}
		switch (this._state)
		{
		case SIGadgetAirGrab.EState.Idle:
			if (this._isActivated)
			{
				if (this._groundedUseCounter.TryUse())
				{
					this.UpdateUsageIndicator();
					this._PlayHaptic(2f);
					this.SetStateAuthority(SIGadgetAirGrab.EState.StartAirGrabbing);
					return;
				}
			}
			else if (instance.IsGroundedButt || instance.IsGroundedHand)
			{
				this._groundedUseCounter.Reset();
				this.UpdateUsageIndicator();
				return;
			}
			break;
		case SIGadgetAirGrab.EState.StartAirGrabbing:
			if (this._isActivated)
			{
				this._grabStartTime = Time.unscaledTime;
				this._airReleaseSpeed = 0f;
				this.m_airGrabXform.SetParent(null, false);
				this.m_airGrabXform.position = GTPlayer.Instance.GetControllerTransform(this._HandIndex == 0).position;
				this.m_airGrabXform.gameObject.SetActive(true);
				this.m_airGrabXform.transform.localScale = this._grabXformInitialScale;
				GTPlayer.Instance.SetVelocity(Vector3.zero);
				this.lastRequestedPlayerPos = GTPlayer.Instance.transform.position;
				GTPlayer.Instance.SetGravityOverride(this, new Action<GTPlayer>(this.GravityOverrideFunction));
				this.hasGravityOverride = true;
				this.SetStateAuthority(SIGadgetAirGrab.EState.PreparedToDash);
				return;
			}
			this.m_airGrabXform.transform.parent = base.transform;
			this.m_airGrabXform.gameObject.SetActive(false);
			return;
		case SIGadgetAirGrab.EState.PreparedToDash:
		{
			if (!this._isActivated)
			{
				this._DoDash();
				return;
			}
			if (Time.unscaledTime > this._grabStartTime + this._maxHoldTime)
			{
				this._DoDash();
				return;
			}
			float num = (Time.unscaledTime - this._grabStartTime) / this._maxHoldTime;
			this.m_airGrabXform.localScale = this._grabXformInitialScale * (1f - num);
			this._UpdateAirGrab();
			return;
		}
		case SIGadgetAirGrab.EState.DashUsed:
			this.m_airGrabXform.transform.parent = base.transform;
			this.m_airGrabXform.gameObject.SetActive(false);
			this.ClearGravityOverride();
			this.SetStateAuthority(SIGadgetAirGrab.EState.Idle);
			break;
		default:
			return;
		}
	}

	private void UpdateUsageIndicator()
	{
		GameObject canActivateIndicator = this.m_canActivateIndicator;
		if (canActivateIndicator == null)
		{
			return;
		}
		canActivateIndicator.SetActive(this._groundedUseCounter.IsReady);
	}

	private void GravityOverrideFunction(GTPlayer player)
	{
	}

	protected override void OnUpdateRemote(float dt)
	{
		base.OnUpdateRemote(dt);
		SIGadgetAirGrab.EState estate = (SIGadgetAirGrab.EState)this.gameEntity.GetState();
		if (estate != this._state)
		{
			this._SetStateShared(estate);
			if (this._state == SIGadgetAirGrab.EState.PreparedToDash)
			{
				this.m_airGrabXform.transform.parent = base.transform;
				this.m_airGrabXform.transform.position = ((this._HandIndex == 0) ? base.GetAttachedPlayerRig().leftHand : base.GetAttachedPlayerRig().rightHand).GetExtrapolatedControllerPosition();
				this.m_airGrabXform.gameObject.SetActive(true);
			}
		}
	}

	private static bool _CanChangeState(long newStateIndex)
	{
		return newStateIndex >= 0L && newStateIndex < 4L;
	}

	private void SetStateAuthority(SIGadgetAirGrab.EState newState)
	{
		this._SetStateShared(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	private void _SetStateShared(SIGadgetAirGrab.EState newState)
	{
		if (newState == this._state || !SIGadgetAirGrab._CanChangeState((long)newState))
		{
			return;
		}
		SIGadgetAirGrab.EState state = this._state;
		this._state = newState;
		switch (this._state)
		{
		case SIGadgetAirGrab.EState.Idle:
			this.m_airGrabXform.gameObject.SetActive(false);
			return;
		case SIGadgetAirGrab.EState.StartAirGrabbing:
			if (state != SIGadgetAirGrab.EState.PreparedToDash)
			{
				this.onGrabSound.Play();
				return;
			}
			break;
		case SIGadgetAirGrab.EState.PreparedToDash:
			break;
		case SIGadgetAirGrab.EState.DashUsed:
			this._PlayAudio(2);
			break;
		default:
			return;
		}
	}

	private bool _CheckInput()
	{
		float sensitivity = this._wasActivated ? this.m_inputDeactivateThreshold : this.m_inputActivateThreshold;
		return this.m_buttonActivatable.CheckInput(sensitivity);
	}

	private void _UpdateAirGrab()
	{
		GTPlayer instance = GTPlayer.Instance;
		Vector3 b = instance.transform.position - this.lastRequestedPlayerPos;
		this.m_airGrabXform.position += b;
		Transform controllerTransform = instance.GetControllerTransform(this._HandIndex == 0);
		Vector3 b2 = this.m_airGrabXform.position - controllerTransform.position;
		instance.SetVelocity(Vector3.zero);
		this.lastRequestedPlayerPos = instance.transform.position + b2;
		instance.RigidbodyMovePosition(this.lastRequestedPlayerPos);
		float magnitude = GamePlayerLocal.instance.GetHandVelocity(this._HandIndex).magnitude;
	}

	private void _DoDash()
	{
		this._airGrabTime = Time.unscaledTime;
		Vector3 averagedVelocity = GTPlayer.Instance.AveragedVelocity;
		float d = this._CalculateDashSpeed(averagedVelocity.magnitude);
		GTPlayer instance = GTPlayer.Instance;
		instance.SetMaximumSlipThisFrame();
		instance.SetVelocity(averagedVelocity.normalized * d);
		this._PlayHaptic(2f);
		this.SetStateAuthority(SIGadgetAirGrab.EState.DashUsed);
	}

	private float _CalculateDashSpeed(float currentYankSpeed)
	{
		float time = Mathf.InverseLerp(this.m_yankMinSpeed, this.m_yankMaxSpeed, currentYankSpeed);
		float t = this.m_speedMappingCurve.Evaluate(time);
		return Mathf.Lerp(this.m_minDashSpeed, this._maxDashSpeed, t);
	}

	private void _PlayHaptic(float strengthMultiplier)
	{
		bool forLeftController;
		if (base.FindAttachedHand(out forLeftController))
		{
			GorillaTagger.Instance.StartVibration(forLeftController, GorillaTagger.Instance.tapHapticStrength * strengthMultiplier, GorillaTagger.Instance.tapHapticDuration);
		}
	}

	private void _PlayAudio(int index)
	{
		this.m_audioSource.clip = this.m_clips[index];
		this.m_audioSource.volume = this.m_clipVolumes[index];
		this.m_audioSource.GTPlay();
	}

	public override void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
		this._maxDashSpeed = (withUpgrades.Contains(SIUpgradeType.AirControl_AirGrab_Speed) ? this.m_maxDashSpeedUpgraded : this.m_maxDashSpeedDefault);
		this._maxHoldTime = (withUpgrades.Contains(SIUpgradeType.AirControl_AirGrab_HoldTime) ? this.m_maxHoldTimeUpgraded : this.m_maxHoldTimeDefault);
	}

	private const string preLog = "[SIGadgetAirGrab]  ";

	private const string preErr = "[SIGadgetAirGrab]  ERROR!!!  ";

	[SerializeField]
	private GameSnappable m_snappable;

	[SerializeField]
	private GameButtonActivatable m_buttonActivatable;

	[SerializeField]
	private float m_inputActivateThreshold = 0.35f;

	[SerializeField]
	private float m_inputDeactivateThreshold = 0.25f;

	[SerializeField]
	private AudioSource m_audioSource;

	[SerializeField]
	private SoundBankPlayer onGrabSound;

	[SerializeField]
	private SoundBankPlayer rechargeSound;

	[SerializeField]
	public AudioClip[] m_clips;

	[SerializeField]
	public float[] m_clipVolumes;

	[Tooltip("Yank min/max: How fast you have to be moving your hand for the yank to register and result in a dash.")]
	[SerializeField]
	private float m_yankMinSpeed = 2f;

	[Tooltip("Yank min/max: How fast you have to be moving your hand for the yank to register and result in a dash.")]
	[SerializeField]
	private float m_yankMaxSpeed = 8f;

	[Tooltip("Dash min/max speed: The fastest speed the player will move")]
	[SerializeField]
	private float m_minDashSpeed = 4f;

	private float _maxDashSpeed;

	[SerializeField]
	private float m_maxDashSpeedDefault = 7f;

	[SerializeField]
	private float m_maxDashSpeedUpgraded = 9f;

	private float _maxHoldTime;

	[SerializeField]
	private float m_maxHoldTimeDefault = 3f;

	[SerializeField]
	private float m_maxHoldTimeUpgraded = 5f;

	[Tooltip("Maps yank speed to dash speed.\nX = Yank Speed (min to max)\nY = Dash Speed (min to max).")]
	[SerializeField]
	private AnimationCurve m_speedMappingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private float m_slipperySurfacesTime = 0.25f;

	[SerializeField]
	private float m_maxInfluenceAngleDefault = 10f;

	[SerializeField]
	private float m_maxInfluenceAngleUpgrade = 15f;

	[SerializeField]
	private float m_cooldownDurationDefault = 6f;

	[SerializeField]
	private float m_cooldownDurationUpgrade = 5f;

	[SerializeField]
	private int m_maxSuperchargeUses = 2;

	[SerializeField]
	private Transform m_airGrabXform;

	[SerializeField]
	private GameObject m_canActivateIndicator;

	private bool _isActivated;

	private bool _wasActivated;

	private float _airGrabTime;

	private float _airReleaseSpeed;

	private Vector3 _airReleaseVector;

	private VRRig _attachedVRRig;

	private int _lastAttachedPlayerActorNr;

	private int _attachedPlayerActorNr = int.MinValue;

	private NetPlayer _attachedNetPlayer;

	private bool _isTagged;

	private readonly object[] _launchYoyoRPCArgs = new object[5];

	private SIGadgetAirGrab.EState _state;

	private ResettableUseCounter _groundedUseCounter;

	private bool hasGravityOverride;

	private float _grabStartTime;

	private Vector3 _grabXformInitialScale;

	private Vector3 lastRequestedPlayerPos;

	private enum EState
	{
		Idle,
		StartAirGrabbing,
		PreparedToDash,
		DashUsed,
		Count
	}
}
