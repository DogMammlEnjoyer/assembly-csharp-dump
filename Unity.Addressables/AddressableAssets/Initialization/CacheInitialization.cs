using System;
using System.IO;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.Util;

namespace UnityEngine.AddressableAssets.Initialization
{
	[Serializable]
	public class CacheInitialization : IInitializableObject
	{
		public bool Initialize(string id, string dataStr)
		{
			CacheInitializationData cacheInitializationData = JsonUtility.FromJson<CacheInitializationData>(dataStr);
			if (cacheInitializationData != null)
			{
				Caching.compressionEnabled = cacheInitializationData.CompressionEnabled;
				Cache currentCacheForWriting = Caching.currentCacheForWriting;
				if (!string.IsNullOrEmpty(cacheInitializationData.CacheDirectoryOverride))
				{
					string text = Addressables.ResolveInternalId(cacheInitializationData.CacheDirectoryOverride);
					if (!Directory.Exists(text))
					{
						Directory.CreateDirectory(text);
					}
					currentCacheForWriting = Caching.GetCacheByPath(text);
					if (!currentCacheForWriting.valid)
					{
						currentCacheForWriting = Caching.AddCache(text);
					}
					Caching.currentCacheForWriting = currentCacheForWriting;
				}
				if (cacheInitializationData.LimitCacheSize)
				{
					currentCacheForWriting.maximumAvailableStorageSpace = cacheInitializationData.MaximumCacheSize;
				}
				else
				{
					currentCacheForWriting.maximumAvailableStorageSpace = long.MaxValue;
				}
				currentCacheForWriting.expirationDelay = 12960000;
			}
			return true;
		}

		public virtual AsyncOperationHandle<bool> InitializeAsync(ResourceManager rm, string id, string data)
		{
			CacheInitialization.CacheInitOp cacheInitOp = new CacheInitialization.CacheInitOp();
			cacheInitOp.Init(() => this.Initialize(id, data));
			return rm.StartOperation<bool>(cacheInitOp, default(AsyncOperationHandle));
		}

		public static string RootPath
		{
			get
			{
				return Path.GetDirectoryName(Caching.defaultCache.path);
			}
		}

		private class CacheInitOp : AsyncOperationBase<bool>, IUpdateReceiver
		{
			public void Init(Func<bool> callback)
			{
				this.m_Callback = callback;
			}

			protected override bool InvokeWaitForCompletion()
			{
				ResourceManager rm = this.m_RM;
				if (rm != null)
				{
					rm.Update(Time.unscaledDeltaTime);
				}
				if (!base.IsDone)
				{
					base.InvokeExecute();
				}
				return base.IsDone;
			}

			public void Update(float unscaledDeltaTime)
			{
				if (Caching.ready && this.m_UpdateRequired)
				{
					this.m_UpdateRequired = false;
					if (this.m_Callback != null)
					{
						base.Complete(this.m_Callback(), true, "");
						return;
					}
					base.Complete(true, true, "");
				}
			}

			protected override void Execute()
			{
				((IUpdateReceiver)this).Update(0f);
			}

			private Func<bool> m_Callback;

			private bool m_UpdateRequired = true;
		}
	}
}
