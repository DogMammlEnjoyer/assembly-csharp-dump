using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	internal static class GTAudioOneShot
	{
		internal static bool isInitialized { get; private set; }

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Initialize()
		{
			if (GTAudioOneShot.isInitialized)
			{
				return;
			}
			AudioSource audioSource = Resources.Load<AudioSource>("AudioSourceSingleton_Prefab");
			if (audioSource == null)
			{
				Debug.LogError("GTAudioOneShot: Failed to load AudioSourceSingleton_Prefab from resources!!!");
				return;
			}
			GTAudioOneShot.audioSource = Object.Instantiate<AudioSource>(audioSource);
			GTAudioOneShot.defaultCurve = GTAudioOneShot.audioSource.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
			Object.DontDestroyOnLoad(GTAudioOneShot.audioSource);
			GTAudioOneShot.isInitialized = true;
		}

		internal static void Play(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
		{
			if (ApplicationQuittingState.IsQuitting || !GTAudioOneShot.isInitialized)
			{
				return;
			}
			GTAudioOneShot.audioSource.pitch = pitch;
			GTAudioOneShot.audioSource.transform.position = position;
			GTAudioOneShot.audioSource.GTPlayOneShot(clip, volume);
		}

		internal static void Play(AudioClip clip, Vector3 position, AnimationCurve curve, float volume = 1f, float pitch = 1f)
		{
			if (ApplicationQuittingState.IsQuitting || !GTAudioOneShot.isInitialized)
			{
				return;
			}
			GTAudioOneShot.audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);
			GTAudioOneShot.Play(clip, position, volume, pitch);
			GTAudioOneShot.audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, GTAudioOneShot.defaultCurve);
		}

		internal static int PlayDelayed(AudioClip sound, Vector3 pos, float delay, float volume = 1f, float pitch = 1f)
		{
			return GTAudioOneShot.PlayDelayed(sound, null, pos, delay, volume, pitch);
		}

		internal static int PlayDelayed(AudioClip sound, Transform xform, Vector3 pos, float delay, float volume = 1f, float pitch = 1f)
		{
			if (ApplicationQuittingState.IsQuitting || !GTAudioOneShot.isInitialized)
			{
				return -1;
			}
			int num;
			if (GTAudioOneShot._delayedFreeHead >= 0)
			{
				num = GTAudioOneShot._delayedFreeHead;
				GTAudioOneShot._delayedFreeHead = GTAudioOneShot._delayedFreeNext[num];
			}
			else
			{
				if (GTAudioOneShot._delayedHighWater >= GTAudioOneShot._delayedData.Length)
				{
					int newSize = GTAudioOneShot._delayedData.Length * 2;
					Array.Resize<GTAudioOneShot.DelayedPlayData>(ref GTAudioOneShot._delayedData, newSize);
					Array.Resize<int>(ref GTAudioOneShot._delayedFreeNext, newSize);
				}
				num = GTAudioOneShot._delayedHighWater++;
			}
			GTAudioOneShot._delayedData[num] = new GTAudioOneShot.DelayedPlayData
			{
				sound = sound,
				xform = xform,
				pos = pos,
				volume = volume,
				pitch = pitch
			};
			GTDelayedExec.Add(GTAudioOneShot._delayedListener, delay, num);
			return num;
		}

		internal static void CancelDelayed(int idx)
		{
			if (idx >= GTAudioOneShot._delayedHighWater)
			{
				return;
			}
			GTAudioOneShot._delayedData[idx].sound = null;
		}

		internal static void UpdateDelayed(int idx, Transform xform)
		{
			if (idx >= GTAudioOneShot._delayedHighWater)
			{
				return;
			}
			ref GTAudioOneShot.DelayedPlayData ptr = ref GTAudioOneShot._delayedData[idx];
			if (ptr.sound == null)
			{
				return;
			}
			ptr.xform = xform;
		}

		internal static void UpdateDelayed(int idx, Vector3 pos)
		{
			if (idx >= GTAudioOneShot._delayedHighWater)
			{
				return;
			}
			ref GTAudioOneShot.DelayedPlayData ptr = ref GTAudioOneShot._delayedData[idx];
			if (ptr.sound == null)
			{
				return;
			}
			ptr.pos = pos;
		}

		internal static void UpdateDelayed(int idx, Transform xform, Vector3 pos)
		{
			if (idx >= GTAudioOneShot._delayedHighWater)
			{
				return;
			}
			ref GTAudioOneShot.DelayedPlayData ptr = ref GTAudioOneShot._delayedData[idx];
			if (ptr.sound == null)
			{
				return;
			}
			ptr.xform = xform;
			ptr.pos = pos;
		}

		[OnEnterPlay_SetNull]
		internal static AudioSource audioSource;

		[OnEnterPlay_SetNull]
		internal static AnimationCurve defaultCurve;

		private const int k_initialDelayedCount = 32;

		[OnEnterPlay_Set(0)]
		private static int _delayedHighWater;

		[OnEnterPlay_Set(-1)]
		private static int _delayedFreeHead = -1;

		[OnEnterPlay_SetNew]
		private static GTAudioOneShot.DelayedPlayData[] _delayedData = new GTAudioOneShot.DelayedPlayData[32];

		[OnEnterPlay_SetNew]
		private static int[] _delayedFreeNext = new int[32];

		[OnEnterPlay_SetNew]
		private static readonly GTAudioOneShot.DelayedPlayListener _delayedListener = new GTAudioOneShot.DelayedPlayListener();

		private struct DelayedPlayData
		{
			public AudioClip sound;

			public Transform xform;

			public Vector3 pos;

			public float volume;

			public float pitch;
		}

		private class DelayedPlayListener : IDelayedExecListener
		{
			public void OnDelayedAction(int contextId)
			{
				if (contextId >= GTAudioOneShot._delayedHighWater)
				{
					return;
				}
				ref GTAudioOneShot.DelayedPlayData ptr = ref GTAudioOneShot._delayedData[contextId];
				if (ptr.sound != null)
				{
					Vector3 position = (ptr.xform != null) ? ptr.xform.TransformPoint(ptr.pos) : ptr.pos;
					GTAudioOneShot.Play(ptr.sound, position, ptr.volume, ptr.pitch);
				}
				ptr = default(GTAudioOneShot.DelayedPlayData);
				GTAudioOneShot._delayedFreeNext[contextId] = GTAudioOneShot._delayedFreeHead;
				GTAudioOneShot._delayedFreeHead = contextId;
			}
		}
	}
}
