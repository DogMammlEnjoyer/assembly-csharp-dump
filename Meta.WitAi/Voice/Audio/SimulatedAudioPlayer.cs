using System;
using Meta.Voice.Logging;
using UnityEngine;

namespace Meta.Voice.Audio
{
	[LogCategory((LogCategory)25)]
	public class SimulatedAudioPlayer : BaseAudioPlayer
	{
		public override bool IsPlaying
		{
			get
			{
				return this._playing;
			}
		}

		public override bool CanSetElapsedSamples
		{
			get
			{
				return true;
			}
		}

		public override int ElapsedSamples
		{
			get
			{
				if (base.ClipStream != null)
				{
					return this.GetSamplesFromSeconds(this._elapsedTime);
				}
				return 0;
			}
		}

		public IVLogger Logger { get; } = LoggerRegistry.Instance.GetLogger(LogCategory.Audio, null);

		public override void Init()
		{
		}

		public override string GetPlaybackErrors()
		{
			return string.Empty;
		}

		protected override void Play(int offsetSamples = 0)
		{
			if (base.ClipStream == null)
			{
				this.Logger.Error("{0} cannot play null Audio clip stream", new object[]
				{
					base.GetType().Name
				});
				return;
			}
			this._elapsedTime = this.GetSecondsFromSamples(offsetSamples);
			this._playing = true;
		}

		public override void Pause()
		{
			if (this.IsPlaying)
			{
				this._playing = false;
			}
		}

		public override void Resume()
		{
			if (!this.IsPlaying)
			{
				this._playing = true;
			}
		}

		public override void Stop()
		{
			if (this.IsPlaying)
			{
				this._playing = false;
			}
			base.Stop();
		}

		private void Update()
		{
			if (!this.IsPlaying || base.ClipStream == null)
			{
				return;
			}
			this._elapsedTime += Time.deltaTime;
			if (base.ClipStream.IsComplete && this._elapsedTime >= base.ClipStream.Length)
			{
				this._playing = false;
			}
		}

		private int GetSamplesFromSeconds(float elapsedSeconds)
		{
			return Mathf.FloorToInt(elapsedSeconds * (float)base.ClipStream.Channels * (float)base.ClipStream.SampleRate);
		}

		private float GetSecondsFromSamples(int samples)
		{
			return (float)samples / (float)(base.ClipStream.Channels * base.ClipStream.SampleRate);
		}

		private float _elapsedTime;

		private bool _playing;
	}
}
