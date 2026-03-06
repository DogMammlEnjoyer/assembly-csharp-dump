using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GameEntityDelayedDestroy : MonoBehaviour, IGorillaSliceableSimple
{
	public void Configure(float delay, AudioClip beepClip, AudioClip explosionClip, GameEntityDelayedDestroy.BeepPhase[] beepPhases, float beepVolume, float explosionVolume)
	{
		this.m_delay = delay;
		this.m_beepClip = beepClip;
		this.m_explosionClip = explosionClip;
		if (beepPhases != null)
		{
			this.m_beepPhases = beepPhases;
		}
		this.m_beepVolume = beepVolume;
		this.m_explosionVolume = explosionVolume;
		if ((this.m_beepClip != null || this.m_explosionClip != null) && this.m_audioSource == null)
		{
			this.m_audioSource = base.GetComponentInChildren<AudioSource>();
		}
	}

	protected void Start()
	{
		this._entity = base.GetComponent<GameEntity>();
		if (this._entity == null)
		{
			Debug.LogError("GameEntityDelayedDestroy: No GameEntity found. Must be added to the same GameObject of the GameEntity you are trying to destroy with a delay.");
			return;
		}
		this._startTime = Time.unscaledTime;
		GameEntityDelayedDestroy.BeepPhase[] beepPhases = this.m_beepPhases;
		this._nextBeepTime = ((beepPhases != null && beepPhases.Length > 0) ? (this._startTime + (this.m_delay - this.m_beepPhases[0].timeRemaining)) : float.MaxValue);
		GorillaSlicerSimpleManager.RegisterSliceable(this);
	}

	protected void OnDestroy()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this);
	}

	void IGorillaSliceableSimple.SliceUpdate()
	{
		float unscaledTime = Time.unscaledTime;
		float num = unscaledTime - this._startTime;
		if (num >= this.m_delay && this._entity != null)
		{
			if (this.m_audioSource != null && this.m_explosionClip != null)
			{
				this.m_audioSource.GTPlayOneShot(this.m_explosionClip, this.m_explosionVolume);
			}
			this._entity.manager.RequestDestroyItem(this._entity.id);
			return;
		}
		if (unscaledTime >= this._nextBeepTime && this.m_audioSource != null && this.m_beepClip != null)
		{
			this.m_audioSource.GTPlayOneShot(this.m_beepClip, this.m_beepVolume);
			float remaining = this.m_delay - num;
			float interval = this.GetInterval(remaining);
			this._nextBeepTime = ((interval > 0f) ? (unscaledTime + interval) : -1f);
		}
	}

	private float GetInterval(float remaining)
	{
		if (this.m_beepPhases == null || this.m_beepPhases.Length == 0)
		{
			return float.MaxValue;
		}
		for (int i = this.m_beepPhases.Length - 1; i >= 0; i--)
		{
			if (remaining <= this.m_beepPhases[i].timeRemaining)
			{
				return this.m_beepPhases[i].interval;
			}
		}
		return this.m_beepPhases[0].interval;
	}

	[FormerlySerializedAs("Lifetime")]
	[SerializeField]
	internal float m_delay = 3f;

	[Header("Countdown Audio")]
	[SerializeField]
	private AudioSource m_audioSource;

	[SerializeField]
	private AudioClip m_beepClip;

	[SerializeField]
	private AudioClip m_explosionClip;

	[Tooltip("Beep phases keyed by seconds remaining. Must be ordered from most to least time remaining.")]
	[SerializeField]
	private GameEntityDelayedDestroy.BeepPhase[] m_beepPhases = new GameEntityDelayedDestroy.BeepPhase[]
	{
		new GameEntityDelayedDestroy.BeepPhase
		{
			timeRemaining = 10f,
			interval = 1f
		},
		new GameEntityDelayedDestroy.BeepPhase
		{
			timeRemaining = 5f,
			interval = 0.5f
		},
		new GameEntityDelayedDestroy.BeepPhase
		{
			timeRemaining = 2f,
			interval = 0.1f
		}
	};

	[SerializeField]
	private float m_beepVolume = 1f;

	[SerializeField]
	private float m_explosionVolume = 1f;

	private GameEntity _entity;

	private float _startTime;

	private float _nextBeepTime;

	[Serializable]
	public struct BeepPhase
	{
		[Tooltip("Beeping starts when this many seconds remain.")]
		public float timeRemaining;

		[Tooltip("Seconds between beeps during this phase.")]
		public float interval;
	}
}
