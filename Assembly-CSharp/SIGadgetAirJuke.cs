using System;
using GorillaLocomotion;
using UnityEngine;

[RequireComponent(typeof(GameGrabbable))]
[RequireComponent(typeof(GameSnappable))]
[RequireComponent(typeof(GameButtonActivatable))]
public class SIGadgetAirJuke : SIGadget
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
		this._fxGObj = this.m_particleSystem.gameObject;
		this._fxXform = this.m_particleSystem.transform;
		this._fxMain = this.m_particleSystem.main;
		this._fxGObj.SetActive(false);
		this._fxEmission = this.m_particleSystem.emission;
		this._fxEmission.enabled = false;
		this._groundedUseCounter = new ResettableUseCounter(this.m_maxRegularUses, this.m_maxSuperchargeUses, new Action<bool>(this.OnRecharged));
	}

	private void OnRecharged(bool recharged)
	{
		if (recharged)
		{
			this.rechargeAudio.Play();
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

	private void _HandleStartInteraction()
	{
		bool isQuitting = ApplicationQuittingState.IsQuitting;
	}

	private void _HandleStopInteraction()
	{
		if (!this.gameEntity.IsAuthority())
		{
			return;
		}
		if (this._state != SIGadgetAirJuke_EState.DashUsed)
		{
			this._SetStateAuthority(SIGadgetAirJuke_EState.Idle);
		}
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
		if (Time.unscaledTime < this._dashStartTime + this.m_slipperySurfacesTime)
		{
			instance.SetMaximumSlipThisFrame();
		}
		switch (this._state)
		{
		case SIGadgetAirJuke_EState.Idle:
			if (this._isActivated)
			{
				if (this._groundedUseCounter.IsReady)
				{
					this._PlayHaptic(0.1f);
					this._SetStateAuthority(SIGadgetAirJuke_EState.TriggerPressHold);
				}
			}
			else if (instance.IsGroundedButt || instance.IsGroundedHand)
			{
				this._groundedUseCounter.Reset();
			}
			break;
		case SIGadgetAirJuke_EState.TriggerPressHold:
			if (!this._isActivated)
			{
				this._DoDash();
			}
			break;
		case SIGadgetAirJuke_EState.DashUsed:
			this._SetStateAuthority(SIGadgetAirJuke_EState.Idle);
			break;
		}
		this._OnUpdateShared();
	}

	protected override void OnUpdateRemote(float dt)
	{
		base.OnUpdateRemote(dt);
		SIGadgetAirJuke_EState sigadgetAirJuke_EState = (SIGadgetAirJuke_EState)this.gameEntity.GetState();
		if (sigadgetAirJuke_EState == SIGadgetAirJuke_EState.DashUsed && this._state != SIGadgetAirJuke_EState.DashUsed)
		{
			this._playingFxUntilTimestamp = Time.time + 0.75f;
			this._dashStartFxPos = this._fxXform.position;
			this.singleJukeAudio.Play();
		}
		this._TrySetStateShared(sigadgetAirJuke_EState);
		this._OnUpdateShared();
	}

	private void _OnUpdateShared()
	{
		if (this._state == SIGadgetAirJuke_EState.TriggerPressHold)
		{
			this._fxGObj.SetActive(true);
			this._fxEmission.enabled = true;
			this._fxMain.startColor = new ParticleSystem.MinMaxGradient(Color.gray3);
			this._UpdateFxRotation();
			return;
		}
		if (Time.time <= this._playingFxUntilTimestamp)
		{
			this._fxGObj.SetActive(true);
			this._fxEmission.enabled = (Vector3.Distance(this._fxXform.position, this._dashStartFxPos) < this.m_fxMaxDistance);
			this._fxMain.startColor = new ParticleSystem.MinMaxGradient(Color.white);
			this._UpdateFxRotation();
			return;
		}
		this._fxGObj.SetActive(false);
	}

	private void _UpdateFxRotation()
	{
		Vector3 vector = this._fxXform.rotation.eulerAngles * 0.017453292f;
		this._fxMain.startRotationX = new ParticleSystem.MinMaxCurve(vector.x);
		this._fxMain.startRotationY = new ParticleSystem.MinMaxCurve(vector.y);
		this._fxMain.startRotationZ = new ParticleSystem.MinMaxCurve(vector.z);
	}

	private void _SetStateAuthority(SIGadgetAirJuke_EState newState)
	{
		if (this._TrySetStateShared(newState))
		{
			this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
		}
	}

	private bool _TrySetStateShared(SIGadgetAirJuke_EState newState)
	{
		long num = (long)newState;
		if (newState == this._state || num < 0L || num >= 3L)
		{
			return false;
		}
		if (newState == SIGadgetAirJuke_EState.DashUsed && this._state != SIGadgetAirJuke_EState.DashUsed)
		{
			this._playingFxUntilTimestamp = Time.time + 0.75f;
			this._dashStartFxPos = this._fxXform.position;
		}
		this._state = newState;
		return true;
	}

	private bool _CheckInput()
	{
		float sensitivity = this._wasActivated ? this.m_inputDeactivateThreshold : this.m_inputActivateThreshold;
		return this.m_buttonActivatable.CheckInput(sensitivity);
	}

	private void _DoDash()
	{
		Vector3 handVelocity = GamePlayerLocal.instance.GetHandVelocity(this._HandIndex);
		if (handVelocity.magnitude < this.m_handMinSpeed || !this._groundedUseCounter.TryUse())
		{
			this._SetStateAuthority(SIGadgetAirJuke_EState.Idle);
			return;
		}
		this._dashStartTime = Time.unscaledTime;
		float num = this._CalculateDashSpeed(handVelocity.magnitude);
		GTPlayer instance = GTPlayer.Instance;
		instance.SetMaximumSlipThisFrame();
		Vector3 normalized = handVelocity.normalized;
		instance.SetVelocity(normalized * -num);
		this._PlayHaptic(2f);
		SuperInfectionManager activeSuperInfectionManager = SuperInfectionManager.activeSuperInfectionManager;
		if (activeSuperInfectionManager != null && !activeSuperInfectionManager.IsSupercharged)
		{
			this.singleJukeAudio.Play();
		}
		else if (this._groundedUseCounter.IsReady)
		{
			this.reusableJukeAudio.Play();
		}
		else
		{
			this.finalJukeAudio.Play();
		}
		this._SetStateAuthority(SIGadgetAirJuke_EState.DashUsed);
	}

	private float _CalculateDashSpeed(float currentYankSpeed)
	{
		float time = Mathf.InverseLerp(this.m_handMinSpeed, this.m_handMaxSpeed, currentYankSpeed);
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

	public override void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
		this._maxDashSpeed = (withUpgrades.Contains(SIUpgradeType.AirControl_AirJuke_Speed) ? this.m_maxDashSpeedUpgraded : this.m_maxDashSpeedDefault);
	}

	private const string preLog = "[SIGadgetAirJuke]  ";

	private const string preErr = "[SIGadgetAirJuke]  ERROR!!!  ";

	[SerializeField]
	private GameSnappable m_snappable;

	[SerializeField]
	private GameButtonActivatable m_buttonActivatable;

	[SerializeField]
	private float m_inputActivateThreshold = 0.35f;

	[SerializeField]
	private float m_inputDeactivateThreshold = 0.25f;

	[Tooltip("Hand min speed: How fast you have to be moving your hand for the dash to trigger.")]
	[SerializeField]
	private float m_handMinSpeed = 2f;

	[Tooltip("Hand move max speed: The fastest hand speed that will be registered.")]
	[SerializeField]
	private float m_handMaxSpeed = 8f;

	[Tooltip("Dash min/max speed: The fastest speed the player will move")]
	[SerializeField]
	private float m_minDashSpeed = 4f;

	private float _maxDashSpeed;

	[SerializeField]
	private float m_maxDashSpeedDefault = 5f;

	[SerializeField]
	private float m_maxDashSpeedUpgraded = 7f;

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
	private int m_maxRegularUses = 1;

	[SerializeField]
	private int m_maxSuperchargeUses = 2;

	[SerializeField]
	private ParticleSystem m_particleSystem;

	[SerializeField]
	private SoundBankPlayer singleJukeAudio;

	[SerializeField]
	private SoundBankPlayer reusableJukeAudio;

	[SerializeField]
	private SoundBankPlayer finalJukeAudio;

	[SerializeField]
	private SoundBankPlayer rechargeAudio;

	private GameObject _fxGObj;

	private Transform _fxXform;

	private ParticleSystem.MainModule _fxMain;

	private ParticleSystem.EmissionModule _fxEmission;

	private bool _isActivated;

	private bool _wasActivated;

	private float _dashStartTime;

	private Vector3 _airReleaseVector;

	private bool _isTagged;

	private SIGadgetAirJuke_EState _state;

	private ResettableUseCounter _groundedUseCounter;

	private float _playingFxUntilTimestamp;

	private Vector3 _dashStartFxPos;

	[SerializeField]
	private float m_fxMaxDistance = 3f;
}
