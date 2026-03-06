using System;
using System.ComponentModel;
using Meta.XR.Util;
using UnityEngine;

namespace Oculus.Haptics
{
	[Feature(Feature.Haptics)]
	public class HapticSource : MonoBehaviour, ISerializationCallbackReceiver
	{
		private void Awake()
		{
			this._player = new HapticClipPlayer();
			this._player.clip = this._clip;
			this.SyncSerializedFieldsToPlayer();
		}

		public void Play()
		{
			this._player.Play(this._controller);
		}

		public void Play(Controller controller)
		{
			this.controller = controller;
			this._player.Play(this._controller);
		}

		public void Pause()
		{
			this._player.Pause();
		}

		public void Resume()
		{
			this._player.Resume();
		}

		public void Stop()
		{
			this._player.Stop();
		}

		public void Seek(float time)
		{
			this._player.Seek(time);
		}

		public HapticClip clip
		{
			set
			{
				this._clip = value;
				if (this._player != null)
				{
					this._player.clip = this._clip;
				}
			}
		}

		public float clipDuration
		{
			get
			{
				return this._player.clipDuration;
			}
		}

		public Controller controller
		{
			set
			{
				this._controller = value;
			}
		}

		[DefaultValue(false)]
		public bool loop
		{
			get
			{
				return this._loop;
			}
			set
			{
				this._loop = value;
				this._player.isLooping = this._loop;
			}
		}

		[DefaultValue(1.0)]
		public float amplitude
		{
			get
			{
				return this._amplitude;
			}
			set
			{
				this._amplitude = value;
				this._player.amplitude = this._amplitude;
			}
		}

		[DefaultValue(0.0)]
		public float frequencyShift
		{
			get
			{
				return this._frequencyShift;
			}
			set
			{
				this._frequencyShift = value;
				this._player.frequencyShift = this._frequencyShift;
			}
		}

		[DefaultValue(128)]
		public uint priority
		{
			get
			{
				return this._priority;
			}
			set
			{
				this._priority = value;
				this._player.priority = this._priority;
			}
		}

		private void SyncSerializedFieldsToPlayer()
		{
			if (this._player == null)
			{
				return;
			}
			this._player.isLooping = this._loop;
			this._player.amplitude = this._amplitude;
			this._player.frequencyShift = this._frequencyShift;
			this._player.priority = this._priority;
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			if (this._player != null)
			{
				this.SyncSerializedFieldsToPlayer();
			}
		}

		protected virtual void OnDestroy()
		{
			this._player.Dispose();
		}

		private HapticClipPlayer _player;

		[SerializeField]
		private HapticClip _clip;

		[SerializeField]
		private Controller _controller = Controller.Both;

		[SerializeField]
		private bool _loop;

		[SerializeField]
		[Range(0f, 3.4028235E+38f)]
		private float _amplitude = 1f;

		[SerializeField]
		[Range(-1f, 1f)]
		private float _frequencyShift;

		[SerializeField]
		[Range(0f, 255f)]
		private uint _priority = 128U;
	}
}
