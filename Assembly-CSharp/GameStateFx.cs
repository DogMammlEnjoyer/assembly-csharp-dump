using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Audio;

public class GameStateFx : MonoBehaviour, IGameStateReceiver, IDelayedExecListener
{
	protected void Awake()
	{
		IGameStateProvider gameStateProvider = this.m_stateProvider as IGameStateProvider;
		if (gameStateProvider == null)
		{
			GTDev.LogError<string>("[GT/GameStateFx]  ERROR!!!  Awake: The supplied State Provider is not type `IGameStateProvider`. Path=" + base.transform.GetPathQ(), null);
			this._isValid = false;
			base.enabled = false;
			return;
		}
		this._stateProvider = gameStateProvider;
		if (!this._IsAllValid())
		{
			return;
		}
		foreach (GameStateFx.StateReaction[] array in this.m_stateMap.Values)
		{
			if (array != null)
			{
				Array.Sort<GameStateFx.StateReaction>(array, new Comparison<GameStateFx.StateReaction>(GameStateFx._DelaySortCompare));
			}
		}
	}

	private static int _DelaySortCompare(GameStateFx.StateReaction a, GameStateFx.StateReaction b)
	{
		return a.delay.CompareTo(b.delay);
	}

	protected void OnEnable()
	{
		if (!this._isValid || ApplicationQuittingState.IsQuitting)
		{
			base.enabled = false;
			return;
		}
		this._stateProvider.GameStateReceiverRegister(this);
	}

	protected void OnDisable()
	{
		if (!this._isValid || ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this._stateProvider.GameStateReceiverUnregister(this);
	}

	void IGameStateReceiver.GameStateReceiverOnStateChanged(long oldState, long newState)
	{
		GameStateFx.StateReaction[] array;
		if (!this.m_stateMap.TryGet(newState, out array))
		{
			return;
		}
		this._delayedExecContextFrameNum = Time.frameCount;
		this._reactionQueue.Clear();
		foreach (GameStateFx.StateReaction stateReaction in array)
		{
			if ((stateReaction.options & GameStateFx.StateReaction.EOptions.Delay) != (GameStateFx.StateReaction.EOptions)0)
			{
				this._reactionQueue.Enqueue(stateReaction);
				GTDelayedExec.Add(this, stateReaction.delay, Time.frameCount);
			}
			else
			{
				GameStateFx._PerformReactions(stateReaction);
			}
		}
	}

	void IDelayedExecListener.OnDelayedAction(int contextFrameNum)
	{
		if (contextFrameNum != this._delayedExecContextFrameNum || !base.isActiveAndEnabled)
		{
			return;
		}
		GameStateFx._PerformReactions(this._reactionQueue.Dequeue());
	}

	private static void _PerformReactions(GameStateFx.StateReaction reaction)
	{
		if ((reaction.options & GameStateFx.StateReaction.EOptions.Sound) != (GameStateFx.StateReaction.EOptions)0)
		{
			if ((reaction.soundInfo.options & GameStateFx.SoundEntry.EOptions.Sound) != (GameStateFx.SoundEntry.EOptions)0)
			{
				reaction.soundInfo.source.resource = reaction.soundInfo.sound;
			}
			if ((reaction.soundInfo.options & GameStateFx.SoundEntry.EOptions.Volume) != (GameStateFx.SoundEntry.EOptions)0)
			{
				reaction.soundInfo.source.volume = reaction.soundInfo.volume;
			}
			if ((reaction.soundInfo.options & GameStateFx.SoundEntry.EOptions.Pitch) != (GameStateFx.SoundEntry.EOptions)0)
			{
				reaction.soundInfo.source.pitch = reaction.soundInfo.pitch;
			}
			reaction.soundInfo.source.GTPlay();
		}
		if ((reaction.options & GameStateFx.StateReaction.EOptions.GameObjects) != (GameStateFx.StateReaction.EOptions)0)
		{
			foreach (GameStateFx.GameObjectInfo gameObjectInfo in reaction.gameObjectInfos)
			{
				gameObjectInfo.gameObject.SetActive(gameObjectInfo.activate);
			}
		}
		if ((reaction.options & GameStateFx.StateReaction.EOptions.Behaviours) != (GameStateFx.StateReaction.EOptions)0)
		{
			foreach (GameStateFx.BehaviourInfo behaviourInfo in reaction.behaviourInfos)
			{
				behaviourInfo.behaviour.enabled = behaviourInfo.enable;
			}
		}
		if ((reaction.options & GameStateFx.StateReaction.EOptions.Renderers) != (GameStateFx.StateReaction.EOptions)0)
		{
			foreach (GameStateFx.RenderInfo renderInfo in reaction.renderers)
			{
				renderInfo.renderer.enabled = renderInfo.enable;
			}
		}
		if ((reaction.options & GameStateFx.StateReaction.EOptions.Materials) != (GameStateFx.StateReaction.EOptions)0)
		{
			GameStateFx.MaterialInfo[] materialInfos = reaction.materialInfos;
			for (int i = 0; i < materialInfos.Length; i++)
			{
				foreach (GameStateFx.MaterialInfo.Entry entry in materialInfos[i].entries)
				{
					entry.slotInfo.renderer.GetSharedMaterials(GameStateFx._g_materialsCache);
					if (entry.slotInfo.slot >= 0 && entry.slotInfo.slot < GameStateFx._g_materialsCache.Count)
					{
						GameStateFx._g_materialsCache[entry.slotInfo.slot] = entry.material;
						entry.slotInfo.renderer.SetSharedMaterials(GameStateFx._g_materialsCache);
					}
				}
			}
		}
	}

	private bool _IsAllValid()
	{
		this._isValid = true;
		bool flag = false;
		this._hasDefaultAudioSource = (this.m_defaultAudioSource != null);
		foreach (GameStateFx.StateReaction[] array in this.m_stateMap.Values)
		{
			foreach (GameStateFx.StateReaction stateReaction in array)
			{
				if ((stateReaction.options & GameStateFx.StateReaction.EOptions.Sound) != (GameStateFx.StateReaction.EOptions)0)
				{
					if ((stateReaction.soundInfo.options & GameStateFx.SoundEntry.EOptions.Source) != (GameStateFx.SoundEntry.EOptions)0)
					{
						if (!this._IsOneValid(stateReaction.soundInfo.source != null, "an AudioSource is unassigned."))
						{
							return false;
						}
					}
					else
					{
						flag = true;
						stateReaction.soundInfo.source = this.m_defaultAudioSource;
					}
					if (!this._IsOneValid(stateReaction.soundInfo.sound != null, "A sound is unassigned."))
					{
						return false;
					}
				}
				if ((stateReaction.options & GameStateFx.StateReaction.EOptions.GameObjects) != (GameStateFx.StateReaction.EOptions)0)
				{
					foreach (GameStateFx.GameObjectInfo gameObjectInfo in stateReaction.gameObjectInfos)
					{
						if (!this._IsOneValid(gameObjectInfo.gameObject != null, "A GameObject is unassigned."))
						{
							return false;
						}
					}
				}
				if ((stateReaction.options & GameStateFx.StateReaction.EOptions.Behaviours) != (GameStateFx.StateReaction.EOptions)0)
				{
					foreach (GameStateFx.BehaviourInfo behaviourInfo in stateReaction.behaviourInfos)
					{
						if (!this._IsOneValid(behaviourInfo.behaviour != null, "A Behaviour is unassigned."))
						{
							return false;
						}
					}
				}
				if ((stateReaction.options & GameStateFx.StateReaction.EOptions.Renderers) != (GameStateFx.StateReaction.EOptions)0)
				{
					foreach (GameStateFx.RenderInfo renderInfo in stateReaction.renderers)
					{
						if (!this._IsOneValid(renderInfo.renderer != null, "A Renderer is unassigned."))
						{
							return false;
						}
					}
				}
				if ((stateReaction.options & GameStateFx.StateReaction.EOptions.Materials) != (GameStateFx.StateReaction.EOptions)0)
				{
					GameStateFx.MaterialInfo[] materialInfos = stateReaction.materialInfos;
					for (int j = 0; j < materialInfos.Length; j++)
					{
						foreach (GameStateFx.MaterialInfo.Entry entry in materialInfos[j].entries)
						{
							if (!this._IsOneValid(entry.slotInfo.renderer != null, "A mat swap Renderer is unassigned"))
							{
								return false;
							}
						}
					}
				}
			}
		}
		if (flag && !this._hasDefaultAudioSource)
		{
			base.enabled = false;
			this._isValid = false;
			return false;
		}
		return true;
	}

	private bool _IsOneValid(bool isValidCondition, string msgFailReason)
	{
		if (isValidCondition)
		{
			return true;
		}
		this._isValid = false;
		base.enabled = false;
		return false;
	}

	private const string preLog = "[GT/GameStateFx]  ";

	private const string preErr = "[GT/GameStateFx]  ERROR!!!  ";

	private bool _isValid;

	[SerializeField]
	private MonoBehaviour m_stateProvider;

	private IGameStateProvider _stateProvider;

	[SerializeField]
	private AudioSource m_defaultAudioSource;

	private bool _hasDefaultAudioSource;

	[SerializeField]
	private GTEnumValueMap<GameStateFx.StateReaction[]> m_stateMap;

	private int _delayedExecContextFrameNum;

	private Queue<GameStateFx.StateReaction> _reactionQueue = new Queue<GameStateFx.StateReaction>(4);

	private static readonly List<Material> _g_materialsCache = new List<Material>(8);

	[Serializable]
	internal class StateReaction
	{
		[Tooltip("Options for what this reaction should do.")]
		public GameStateFx.StateReaction.EOptions options;

		public float delay;

		public GameStateFx.SoundEntry soundInfo;

		public GameStateFx.GameObjectInfo[] gameObjectInfos;

		public GameStateFx.BehaviourInfo[] behaviourInfos;

		public GameStateFx.RenderInfo[] renderers;

		public GameStateFx.MaterialInfo[] materialInfos;

		[Flags]
		public enum EOptions
		{
			Delay = 1,
			Sound = 2,
			GameObjects = 4,
			Behaviours = 8,
			Renderers = 16,
			Materials = 32
		}
	}

	[Serializable]
	public struct SoundEntry
	{
		public GameStateFx.SoundEntry.EOptions options;

		public AudioSource source;

		public AudioResource sound;

		public float volume;

		public float pitch;

		[Flags]
		public enum EOptions
		{
			Source = 1,
			Sound = 2,
			Volume = 4,
			Pitch = 8
		}
	}

	[Serializable]
	internal struct GameObjectInfo
	{
		public bool activate;

		public GameObject gameObject;
	}

	[Serializable]
	internal struct BehaviourInfo
	{
		public bool enable;

		public Behaviour behaviour;
	}

	[Serializable]
	internal struct RenderInfo
	{
		public bool enable;

		public Renderer renderer;
	}

	[Serializable]
	internal struct MaterialInfo
	{
		public GameStateFx.MaterialInfo.Entry[] entries;

		[Serializable]
		internal struct Entry
		{
			public GTRendererMatSlot slotInfo;

			public Material material;
		}
	}
}
