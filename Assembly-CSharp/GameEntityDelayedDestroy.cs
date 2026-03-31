using System;
using GorillaTag.Audio;
using UnityEngine;

public class GameEntityDelayedDestroy : MonoBehaviour, IDelayedExecListener
{
	internal void Configure(GameEntityDelayedDestroy.Options options)
	{
		this.m_options = options;
		if ((this.m_options.beepSound != null || this.m_options.explosionSound != null) && this.m_options.audioSource == null)
		{
			this.m_options.audioSource = base.GetComponentInChildren<AudioSource>();
		}
	}

	protected void OnDestroy()
	{
		if (this._delayedExplosionAudioIndex >= 0)
		{
			GTAudioOneShot.CancelDelayed(this._delayedExplosionAudioIndex);
			this._delayedExplosionAudioIndex = -1;
		}
		if (this._delayedExplosionPoolIndex >= 0)
		{
			ObjectPools.CancelDelayedInstantiate(this._delayedExplosionPoolIndex);
			this._delayedExplosionPoolIndex = -1;
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
		GTDelayedExec.Add(this, 0f, 0);
	}

	internal void ResetTimer()
	{
		this._callGenerationId++;
		int callGenerationId = this._callGenerationId;
		int contextId = callGenerationId << 1 | 1;
		GameEntityDelayedDestroy.Options options = this.m_options;
		GTDelayedExec.Add(this, options.delay, contextId);
		if (options.explosionSound != null)
		{
			this._delayedExplosionAudioIndex = GTAudioOneShot.PlayDelayed(options.explosionSound, base.transform.parent, base.transform.localPosition, options.delay, options.explosionVolume, 1f);
		}
		else
		{
			this._delayedExplosionAudioIndex = -1;
		}
		if (options.pooledExplosionPrefab != null)
		{
			this._delayedExplosionPoolIndex = ObjectPools.InstantiateDelayed(options.pooledExplosionPrefab, base.transform.parent, base.transform.localPosition, options.delay);
		}
		else
		{
			this._delayedExplosionPoolIndex = -1;
		}
		if (options.beepSound == null || options.beepPhases == null || options.beepPhases.Length == 0)
		{
			return;
		}
		int contextId2 = callGenerationId << 1;
		for (int i = 0; i < options.beepPhases.Length; i++)
		{
			float interval = options.beepPhases[i].interval;
			if (interval > 0f)
			{
				float num = (i + 1 < options.beepPhases.Length) ? options.beepPhases[i + 1].timeRemaining : 0f;
				float num2 = Mathf.Min(options.beepPhases[i].timeRemaining, options.delay);
				if (num2 > num)
				{
					float num3 = options.delay - num2;
					float num4 = options.delay - num;
					for (float num5 = num3; num5 < num4; num5 += interval)
					{
						GTDelayedExec.Add(this, num5, contextId2);
					}
				}
			}
		}
	}

	void IDelayedExecListener.OnDelayedAction(int contextId)
	{
		if (contextId == 0)
		{
			if (this._callGenerationId == 0 && this._entity != null)
			{
				this.ResetTimer();
			}
			return;
		}
		if (contextId >> 1 != this._callGenerationId)
		{
			return;
		}
		if (this._entity == null)
		{
			return;
		}
		GameEntityDelayedDestroy.Options options = this.m_options;
		if ((contextId & 1) != 0)
		{
			this._entity.manager.RequestDestroyItem(this._entity.id);
			return;
		}
		if (this._delayedExplosionAudioIndex >= 0)
		{
			GTAudioOneShot.UpdateDelayed(this._delayedExplosionAudioIndex, base.transform.parent, base.transform.localPosition);
		}
		if (this._delayedExplosionPoolIndex >= 0)
		{
			ObjectPools.UpdateDelayedInstantiate(this._delayedExplosionPoolIndex, base.transform.parent, base.transform.localPosition);
		}
		if (options.beepSound != null)
		{
			if (options.audioSource != null && options.audioSource.isActiveAndEnabled)
			{
				options.audioSource.GTPlayOneShot(options.beepSound, options.beepVolume);
				return;
			}
			GTAudioOneShot.Play(options.beepSound, base.transform.position, options.beepVolume, 1f);
		}
	}

	[SerializeField]
	private GameEntityDelayedDestroy.Options m_options = new GameEntityDelayedDestroy.Options
	{
		delay = 3f,
		audioSource = null,
		explosionSound = null,
		explosionVolume = 1f,
		pooledExplosionPrefab = null,
		beepSound = null,
		beepVolume = 1f,
		beepPhases = null
	};

	private GameEntity _entity;

	private int _callGenerationId;

	private int _delayedExplosionAudioIndex = -1;

	private int _delayedExplosionPoolIndex = -1;

	private const int k_contextId_deferredStart = 0;

	[Serializable]
	public struct Options
	{
		public float delay;

		[Tooltip("Optional. If not set then a sound will be played at the transforms position. Which if it is a long clip on a transform that moves a lot then it will feel wrong without this set.")]
		public AudioSource audioSource;

		public AudioClip explosionSound;

		public float explosionVolume;

		public GameObject pooledExplosionPrefab;

		public AudioClip beepSound;

		public float beepVolume;

		[Tooltip("Beep phases keyed by seconds remaining. Must be ordered from most to least time remaining.")]
		public GameEntityDelayedDestroy.BeepPhase[] beepPhases;
	}

	[Serializable]
	public struct BeepPhase
	{
		[Tooltip("Beeping starts when this many seconds remain.")]
		public float timeRemaining;

		[Tooltip("Seconds between beeps during this phase.")]
		public float interval;
	}
}
