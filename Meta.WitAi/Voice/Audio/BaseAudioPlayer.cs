using System;
using Meta.WitAi.Json;
using UnityEngine;

namespace Meta.Voice.Audio
{
	public abstract class BaseAudioPlayer : MonoBehaviour, IAudioPlayer
	{
		public IAudioClipStream ClipStream { get; private set; }

		public WitResponseNode SpeechNode { get; private set; }

		public virtual bool IsPlaying
		{
			get
			{
				return this.ClipStream != null;
			}
		}

		public virtual bool CanSetElapsedSamples
		{
			get
			{
				return false;
			}
		}

		public virtual int ElapsedSamples
		{
			get
			{
				return 0;
			}
		}

		public abstract void Init();

		public abstract string GetPlaybackErrors();

		public void Play(IAudioClipStream clipStream, int offsetSamples, WitResponseNode speechNode)
		{
			this.Stop();
			this.ClipStream = clipStream;
			this.SpeechNode = speechNode;
			this.Play(offsetSamples);
		}

		protected abstract void Play(int offsetSamples);

		public abstract void Pause();

		public abstract void Resume();

		public virtual void Stop()
		{
			this.ClipStream = null;
		}
	}
}
