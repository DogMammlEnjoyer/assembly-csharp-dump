using System;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace UnityEngine.AddressableAssets
{
	[Serializable]
	public class AssetReference : IKeyEvaluator
	{
		public AsyncOperationHandle OperationHandle
		{
			get
			{
				return this.m_Operation;
			}
			internal set
			{
				this.m_Operation = value;
			}
		}

		public virtual object RuntimeKey
		{
			get
			{
				if (this.m_AssetGUID == null)
				{
					this.m_AssetGUID = string.Empty;
				}
				if (!string.IsNullOrEmpty(this.m_SubObjectName))
				{
					return string.Format("{0}[{1}]", this.m_AssetGUID, this.m_SubObjectName);
				}
				return this.m_AssetGUID;
			}
		}

		public virtual string AssetGUID
		{
			get
			{
				return this.m_AssetGUID;
			}
		}

		public virtual string SubObjectName
		{
			get
			{
				return this.m_SubObjectName;
			}
			set
			{
				this.m_SubObjectName = value;
			}
		}

		internal virtual Type SubObjectType
		{
			get
			{
				if (!string.IsNullOrEmpty(this.m_SubObjectName) && this.m_SubObjectType != null)
				{
					return Type.GetType(this.m_SubObjectType);
				}
				return null;
			}
		}

		public bool IsValid()
		{
			return this.m_Operation.IsValid();
		}

		public bool IsDone
		{
			get
			{
				return this.m_Operation.IsDone;
			}
		}

		public AssetReference()
		{
		}

		public AssetReference(string guid)
		{
			this.m_AssetGUID = guid;
		}

		public virtual Object Asset
		{
			get
			{
				if (!this.m_Operation.IsValid())
				{
					return null;
				}
				return this.m_Operation.Result as Object;
			}
		}

		public override string ToString()
		{
			return "[" + this.m_AssetGUID + "]";
		}

		private static AsyncOperationHandle<T> CreateFailedOperation<T>()
		{
			Addressables.InitializeAsync();
			return Addressables.ResourceManager.CreateCompletedOperation<T>(default(T), new Exception("Attempting to load an asset reference that has no asset assigned to it.").Message);
		}

		public virtual AsyncOperationHandle<TObject> LoadAssetAsync<TObject>()
		{
			AsyncOperationHandle<TObject> asyncOperationHandle = default(AsyncOperationHandle<TObject>);
			if (this.m_Operation.IsValid())
			{
				Debug.LogError("Attempting to load AssetReference that has already been loaded. Handle is exposed through getter OperationHandle");
			}
			else
			{
				asyncOperationHandle = Addressables.LoadAssetAsync<TObject>(this.RuntimeKey);
				this.OperationHandle = asyncOperationHandle;
			}
			return asyncOperationHandle;
		}

		public virtual AsyncOperationHandle<SceneInstance> LoadSceneAsync(LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100)
		{
			AsyncOperationHandle<SceneInstance> asyncOperationHandle = default(AsyncOperationHandle<SceneInstance>);
			if (this.m_Operation.IsValid())
			{
				Debug.LogError("Attempting to load AssetReference Scene that has already been loaded. Handle is exposed through getter OperationHandle");
			}
			else
			{
				asyncOperationHandle = Addressables.LoadSceneAsync(this.RuntimeKey, loadMode, activateOnLoad, priority, SceneReleaseMode.ReleaseSceneWhenSceneUnloaded);
				this.OperationHandle = asyncOperationHandle;
			}
			return asyncOperationHandle;
		}

		public virtual AsyncOperationHandle<SceneInstance> UnLoadScene()
		{
			return Addressables.UnloadSceneAsync(this.m_Operation, true);
		}

		public virtual AsyncOperationHandle<GameObject> InstantiateAsync(Vector3 position, Quaternion rotation, Transform parent = null)
		{
			return Addressables.InstantiateAsync(this.RuntimeKey, position, rotation, parent, true);
		}

		public virtual AsyncOperationHandle<GameObject> InstantiateAsync(Transform parent = null, bool instantiateInWorldSpace = false)
		{
			return Addressables.InstantiateAsync(this.RuntimeKey, parent, instantiateInWorldSpace, true);
		}

		public virtual bool RuntimeKeyIsValid()
		{
			string text = this.RuntimeKey.ToString();
			int num = text.IndexOf('[');
			if (num != -1)
			{
				text = text.Substring(0, num);
			}
			Guid guid;
			return Guid.TryParse(text, out guid);
		}

		public virtual void ReleaseAsset()
		{
			if (!this.m_Operation.IsValid())
			{
				Debug.LogWarning("Cannot release a null or unloaded asset.");
				return;
			}
			this.m_Operation.Release();
			this.m_Operation = default(AsyncOperationHandle);
		}

		public virtual void ReleaseInstance(GameObject obj)
		{
			Addressables.ReleaseInstance(obj);
		}

		public virtual bool ValidateAsset(Object obj)
		{
			return true;
		}

		public virtual bool ValidateAsset(string path)
		{
			return true;
		}

		[FormerlySerializedAs("m_assetGUID")]
		[SerializeField]
		protected internal string m_AssetGUID = "";

		[SerializeField]
		private string m_SubObjectName;

		[SerializeField]
		private string m_SubObjectType;

		private AsyncOperationHandle m_Operation;
	}
}
