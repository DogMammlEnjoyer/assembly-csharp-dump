using System;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

public class SIGadgetWing : SIGadget
{
	private void Awake()
	{
		if (this.m_buttonActivatable == null)
		{
			this.m_buttonActivatable = base.GetComponent<GameButtonActivatable>();
		}
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this.OnSnapped));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this.OnReleased));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this.OnUnsnapped));
	}

	private void OnGrabbed()
	{
		this._lastWingPos = this.m_wingCenter.transform.position;
	}

	private void OnSnapped()
	{
		this._lastWingPos = this.m_wingCenter.transform.position;
	}

	private void OnReleased()
	{
	}

	private void OnUnsnapped()
	{
	}

	protected override void OnUpdateAuthority(float dt)
	{
		Vector3 position = this.m_wingCenter.transform.position;
		SIGadgetWing_EState state = this._state;
		this._state = (this.m_buttonActivatable.CheckInput(true, true, 0.25f, true, true) ? SIGadgetWing_EState.TriggerPressed : SIGadgetWing_EState.Idle);
		if (state != this._state)
		{
			this.gameEntity.RequestState(this.gameEntity.id, (long)this._state);
			this._lastWingPos = position;
		}
		if (this._state != SIGadgetWing_EState.TriggerPressed)
		{
			return;
		}
		Vector3 lhs = this._lastWingPos - position;
		Vector3 up = this.m_wingCenter.transform.up;
		float num = Mathf.Max(Vector3.Dot(lhs, up), 0f);
		double num2 = PhotonNetwork.Time - (double)GTPlayer.Instance.LastTouchedGroundAtNetworkTime;
		float num3 = Mathf.Lerp(this.m_flapStrength, this.m_flapDecayedStrength, (float)num2 / this.m_decayDuration);
		Vector3 force = up * (num * num3);
		GTPlayer.Instance.AddForce(force, ForceMode.Impulse);
		this._lastWingPos = position;
	}

	protected override void OnUpdateRemote(float dt)
	{
	}

	public override void OnEntityStateChange(long prevState, long newState)
	{
		if (newState == prevState || newState < 0L || newState >= 2L)
		{
			return;
		}
		this.m_gtAnimator.SetState(newState);
	}

	[SerializeField]
	private GameButtonActivatable m_buttonActivatable;

	[SerializeField]
	private float m_flapStrength;

	[SerializeField]
	private float m_flapDecayedStrength;

	[SerializeField]
	private float m_decayDuration;

	[SerializeField]
	private float m_liftStrength;

	[SerializeField]
	private float m_liftCap;

	[SerializeField]
	private Transform m_wingCenter;

	[SerializeField]
	private GTAnimator m_gtAnimator;

	private Vector3 _lastWingPos;

	private SIGadgetWing_EState _state;
}
