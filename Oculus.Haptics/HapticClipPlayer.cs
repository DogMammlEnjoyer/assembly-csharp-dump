using System;
using UnityEngine;

namespace Oculus.Haptics
{
	public class HapticClipPlayer : IDisposable
	{
		public HapticClipPlayer()
		{
			this.SetHaptics();
			int num = this._haptics.CreateHapticPlayer();
			if (-1 != num)
			{
				this._playerId = num;
			}
		}

		public HapticClipPlayer(HapticClip clip)
		{
			this.SetHaptics();
			int num = this._haptics.CreateHapticPlayer();
			if (-1 != num)
			{
				this._playerId = num;
				this.clip = clip;
			}
		}

		protected virtual void SetHaptics()
		{
			this._haptics = Haptics.Instance;
		}

		public void Play(Controller controller)
		{
			this._haptics.PlayHapticPlayer(this._playerId, controller);
		}

		public void Pause()
		{
			this._haptics.PauseHapticPlayer(this._playerId);
		}

		public void Resume()
		{
			this._haptics.ResumeHapticPlayer(this._playerId);
		}

		public void Stop()
		{
			this._haptics.StopHapticPlayer(this._playerId);
		}

		public void Seek(float time)
		{
			this._haptics.SeekPlaybackPositionHapticPlayer(this._playerId, time);
		}

		public bool isLooping
		{
			get
			{
				return this._haptics.IsHapticPlayerLooping(this._playerId);
			}
			set
			{
				this._haptics.LoopHapticPlayer(this._playerId, value);
			}
		}

		public float clipDuration
		{
			get
			{
				return this._haptics.GetClipDuration(this._clipId);
			}
		}

		public float amplitude
		{
			get
			{
				return this._haptics.GetAmplitudeHapticPlayer(this._playerId);
			}
			set
			{
				this._haptics.SetAmplitudeHapticPlayer(this._playerId, value);
			}
		}

		public float frequencyShift
		{
			get
			{
				return this._haptics.GetFrequencyShiftHapticPlayer(this._playerId);
			}
			set
			{
				this._haptics.SetFrequencyShiftHapticPlayer(this._playerId, value);
			}
		}

		public uint priority
		{
			get
			{
				return this._haptics.GetPriorityHapticPlayer(this._playerId);
			}
			set
			{
				this._haptics.SetPriorityHapticPlayer(this._playerId, value);
			}
		}

		public HapticClip clip
		{
			set
			{
				int num = this._haptics.LoadClip(value.json);
				if (-1 != num)
				{
					this._haptics.SetHapticPlayerClip(this._playerId, num);
					if (this._clipId != -1)
					{
						this._haptics.ReleaseClip(this._clipId);
					}
					this._clipId = num;
				}
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this._playerId != -1)
			{
				if (!this._haptics.ReleaseClip(this._clipId) & this._haptics.ReleaseHapticPlayer(this._playerId))
				{
					Debug.LogError("Error: HapticClipPlayer or HapticClip could not be released");
				}
				this._clipId = -1;
				this._playerId = -1;
			}
		}

		~HapticClipPlayer()
		{
			this.Dispose(false);
		}

		private int _clipId = -1;

		private int _playerId = -1;

		protected Haptics _haptics;
	}
}
