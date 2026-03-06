using System;
using Meta.WitAi;
using UnityEngine;

namespace Meta.Voice.Audio
{
	public abstract class BaseAudioClipStream : IAudioClipStream
	{
		public int Channels { get; }

		public int SampleRate { get; }

		public float StreamReadyLength { get; }

		public bool IsReady { get; private set; }

		public bool IsComplete { get; private set; }

		public int AddedSamples { get; protected set; }

		public int ExpectedSamples { get; protected set; }

		public int TotalSamples
		{
			get
			{
				return Mathf.Max(this.AddedSamples, this.ExpectedSamples);
			}
		}

		public float Length
		{
			get
			{
				return this.GetSampleLength(this.TotalSamples);
			}
		}

		public AudioClipStreamSampleDelegate OnAddSamples { get; set; }

		public AudioClipStreamDelegate OnStreamReady { get; set; }

		public AudioClipStreamDelegate OnStreamUpdated { get; set; }

		public AudioClipStreamDelegate OnStreamComplete { get; set; }

		public AudioClipStreamDelegate OnStreamUnloaded { get; set; }

		protected BaseAudioClipStream(int newChannels, int newSampleRate, float newStreamReadyLength = 1.5f)
		{
			this.Channels = newChannels;
			this.SampleRate = newSampleRate;
			this.StreamReadyLength = newStreamReadyLength;
		}

		protected virtual void Reset()
		{
			this.AddedSamples = 0;
			this.ExpectedSamples = 0;
			this.IsReady = false;
			this.IsComplete = false;
			this.OnAddSamples = null;
			this.OnStreamReady = null;
			this.OnStreamUpdated = null;
			this.OnStreamComplete = null;
			this.OnStreamUnloaded = null;
		}

		public abstract void AddSamples(float[] samples, int offset, int length);

		public virtual void SetExpectedSamples(int expectedSamples)
		{
			if (expectedSamples <= 0)
			{
				return;
			}
			this.ExpectedSamples = expectedSamples;
			this.UpdateState();
		}

		public virtual void UpdateState()
		{
			if (!this.IsReady && this.IsEnoughBuffered())
			{
				this.HandleStreamReady();
			}
			if (!this.IsComplete && this.ExpectedSamples > 0 && this.AddedSamples >= this.ExpectedSamples)
			{
				this.HandleStreamComplete();
			}
		}

		protected virtual bool IsEnoughBuffered()
		{
			float num = this.StreamReadyLength;
			if (num <= 0f)
			{
				return this.AddedSamples > 0;
			}
			if (this.ExpectedSamples > 0)
			{
				num = Mathf.Min(this.StreamReadyLength, this.GetSampleLength(this.ExpectedSamples));
			}
			return this.GetSampleLength(this.AddedSamples) >= num;
		}

		private void HandleStreamReady()
		{
			if (this.IsReady)
			{
				return;
			}
			this.IsReady = true;
			ThreadUtility.CallOnMainThread(new Action(this.RaiseStreamReady));
		}

		protected virtual void RaiseStreamReady()
		{
			AudioClipStreamDelegate onStreamReady = this.OnStreamReady;
			if (onStreamReady == null)
			{
				return;
			}
			onStreamReady(this);
		}

		protected virtual void HandleStreamUpdated()
		{
			if (!this.IsReady)
			{
				return;
			}
			ThreadUtility.CallOnMainThread(new Action(this.RaiseStreamUpdated));
		}

		protected virtual void RaiseStreamUpdated()
		{
			AudioClipStreamDelegate onStreamUpdated = this.OnStreamUpdated;
			if (onStreamUpdated == null)
			{
				return;
			}
			onStreamUpdated(this);
		}

		private void HandleStreamComplete()
		{
			if (this.IsComplete)
			{
				return;
			}
			this.IsComplete = true;
			ThreadUtility.CallOnMainThread(new Action(this.RaiseStreamComplete));
		}

		protected virtual void RaiseStreamComplete()
		{
			AudioClipStreamDelegate onStreamComplete = this.OnStreamComplete;
			if (onStreamComplete == null)
			{
				return;
			}
			onStreamComplete(this);
		}

		public virtual void Unload()
		{
			AudioClipStreamDelegate onStreamUnloaded = this.OnStreamUnloaded;
			this.Reset();
			if (onStreamUnloaded == null)
			{
				return;
			}
			onStreamUnloaded(this);
		}

		private float GetSampleLength(int totalSamples)
		{
			return BaseAudioClipStream.GetLength(totalSamples, this.Channels, this.SampleRate);
		}

		public static float GetLength(int totalSamples, int channels, int samplesPerSecond)
		{
			return (float)totalSamples / (float)(channels * samplesPerSecond);
		}
	}
}
