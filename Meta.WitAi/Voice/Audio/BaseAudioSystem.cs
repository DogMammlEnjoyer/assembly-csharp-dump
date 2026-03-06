using System;
using Lib.Wit.Runtime.Utilities.Logging;
using Meta.Voice.Logging;
using Meta.WitAi;
using UnityEngine;

namespace Meta.Voice.Audio
{
	[LogCategory(LogCategory.Audio)]
	public abstract class BaseAudioSystem<TAudioClipStream, TAudioPlayer> : MonoBehaviour, IAudioSystem, ILogSource where TAudioClipStream : IAudioClipStream where TAudioPlayer : MonoBehaviour, IAudioPlayer
	{
		public IVLogger Logger { get; } = LoggerRegistry.Instance.GetLogger(LogCategory.Audio, null);

		public AudioClipSettings ClipSettings
		{
			get
			{
				return this._clipSettings;
			}
			set
			{
				if (this._clipSettings.Equals(value))
				{
					return;
				}
				this._clipSettings = value;
				if (this._pool != null)
				{
					this.Logger.Warning("Due to a settings change, the pool is being cleared.", Array.Empty<object>());
					this._pool.Dispose();
					this._pool = null;
				}
			}
		}

		protected virtual void GeneratePool()
		{
			if (this._pool != null)
			{
				return;
			}
			this._pool = new ObjectPool<TAudioClipStream>(new Func<TAudioClipStream>(this.GenerateClip), 0);
		}

		protected virtual TAudioClipStream GenerateClip()
		{
			if (typeof(TAudioClipStream) == typeof(RawAudioClipStream))
			{
				return (TAudioClipStream)((object)new RawAudioClipStream(this.ClipSettings.Channels, this.ClipSettings.SampleRate, this.ClipSettings.ReadyDuration, this.ClipSettings.MaxDuration));
			}
			this.Logger.Warning("{0}.GenerateClip() is missing clip instantiation for {1}", new object[]
			{
				base.GetType().Name,
				typeof(TAudioClipStream).Name
			});
			return default(TAudioClipStream);
		}

		protected virtual void OnDestroy()
		{
			this._pool.Dispose();
			this._pool = null;
		}

		public virtual void PreloadClipStreams(int total)
		{
			this.GeneratePool();
			this._pool.Preload(total);
		}

		public virtual IAudioClipStream GetAudioClipStream()
		{
			this.GeneratePool();
			TAudioClipStream taudioClipStream = this._pool.Get();
			taudioClipStream.OnStreamUnloaded = (AudioClipStreamDelegate)Delegate.Combine(taudioClipStream.OnStreamUnloaded, new AudioClipStreamDelegate(this.UnloadAudioClipStream));
			return taudioClipStream;
		}

		protected virtual void UnloadAudioClipStream(IAudioClipStream clipStream)
		{
			if (clipStream is TAudioClipStream)
			{
				TAudioClipStream item = (TAudioClipStream)((object)clipStream);
				item.OnStreamUnloaded = (AudioClipStreamDelegate)Delegate.Remove(item.OnStreamUnloaded, new AudioClipStreamDelegate(this.UnloadAudioClipStream));
				this._pool.Return(item);
			}
		}

		public virtual IAudioPlayer GetAudioPlayer(GameObject root)
		{
			return root.AddComponent<TAudioPlayer>();
		}

		private AudioClipSettings _clipSettings = new AudioClipSettings
		{
			Channels = 1,
			SampleRate = 24000,
			ReadyDuration = 1.5f,
			MaxDuration = 15f
		};

		private ObjectPool<TAudioClipStream> _pool;
	}
}
