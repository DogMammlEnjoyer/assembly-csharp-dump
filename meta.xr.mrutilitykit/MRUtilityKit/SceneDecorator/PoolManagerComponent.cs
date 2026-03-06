using System;
using Meta.XR.Util;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
	[Feature(Feature.Scene)]
	public class PoolManagerComponent : MonoBehaviour
	{
		protected internal virtual void InitDefaultPools(Pool<GameObject>.Callbacks? defaultCallbacks = null)
		{
			if (defaultCallbacks == null)
			{
				defaultCallbacks = new Pool<GameObject>.Callbacks?(PoolManagerComponent.DEFAULT_CALLBACKS);
			}
			foreach (PoolManagerComponent.PoolDesc poolDesc in this.defaultPools)
			{
				Pool<GameObject>.Callbacks callbacks = defaultCallbacks.Value;
				PoolManagerComponent.CallbackProvider callbackProvider = (poolDesc.callbackProviderOverride == null) ? poolDesc.primitive.GetComponent<PoolManagerComponent.CallbackProvider>() : poolDesc.callbackProviderOverride;
				if (callbackProvider != null)
				{
					callbacks = callbackProvider.GetPoolCallbacks();
				}
				PoolManagerComponent.PoolDesc.PoolType poolType = poolDesc.poolType;
				Pool<GameObject> pool;
				if (poolType != PoolManagerComponent.PoolDesc.PoolType.CIRCULAR && poolType == PoolManagerComponent.PoolDesc.PoolType.FIXED)
				{
					pool = new FixedPool<GameObject>(poolDesc.primitive, poolDesc.size, callbacks);
				}
				else
				{
					pool = new CircularPool<GameObject>(poolDesc.primitive, poolDesc.size, callbacks);
				}
				this.poolManager.AddPool(poolDesc.primitive, pool);
			}
		}

		public GameObject Create(GameObject primitive, Vector3 position, Quaternion rotation, MRUKAnchor anchor, Transform parent = null)
		{
			Pool<GameObject> pool = this.poolManager.GetPool(primitive);
			if (pool == null)
			{
				return Object.Instantiate<GameObject>(primitive, position, rotation, parent);
			}
			Action<GameObject> onGet = pool.callbacks.OnGet;
			pool.callbacks.OnGet = null;
			GameObject gameObject = pool.Get();
			if (gameObject == null)
			{
				pool.callbacks.OnGet = onGet;
				return null;
			}
			PoolManagerComponent.PoolableData poolableData;
			if (!gameObject.TryGetComponent<PoolManagerComponent.PoolableData>(out poolableData))
			{
				poolableData = gameObject.AddComponent<PoolManagerComponent.PoolableData>();
			}
			poolableData.Scale = gameObject.transform.localScale;
			poolableData.Anchor = anchor;
			poolableData.Pool = pool;
			gameObject.transform.SetParent(parent);
			gameObject.transform.SetPositionAndRotation(position, rotation);
			onGet(gameObject);
			pool.callbacks.OnGet = onGet;
			return gameObject;
		}

		public GameObject Create(GameObject primitive, MRUKAnchor anchor, Transform parent = null, bool instantiateInWorldSpace = false)
		{
			Pool<GameObject> pool = this.poolManager.GetPool(primitive);
			if (pool == null)
			{
				return Object.Instantiate<GameObject>(primitive, parent, instantiateInWorldSpace);
			}
			Action<GameObject> onGet = pool.callbacks.OnGet;
			pool.callbacks.OnGet = null;
			GameObject gameObject = pool.Get();
			if (gameObject == null)
			{
				pool.callbacks.OnGet = onGet;
				return null;
			}
			PoolManagerComponent.PoolableData poolableData;
			if (!gameObject.TryGetComponent<PoolManagerComponent.PoolableData>(out poolableData))
			{
				poolableData = gameObject.AddComponent<PoolManagerComponent.PoolableData>();
			}
			poolableData.Scale = gameObject.transform.localScale;
			poolableData.Anchor = anchor;
			poolableData.Pool = pool;
			gameObject.transform.SetParent(parent);
			if (parent)
			{
				if (instantiateInWorldSpace)
				{
					gameObject.transform.SetPositionAndRotation(parent.position, parent.rotation);
				}
				else
				{
					gameObject.transform.localRotation = parent.localRotation;
					gameObject.transform.localPosition = parent.localPosition;
				}
			}
			onGet(gameObject);
			pool.callbacks.OnGet = onGet;
			return gameObject;
		}

		public T Create<T>(T primitive, Vector3 position, Quaternion rotation, MRUKAnchor anchor, Transform parent = null) where T : Component
		{
			GameObject gameObject = this.Create(primitive.gameObject, position, rotation, anchor, parent);
			if (!(gameObject == null))
			{
				return gameObject.GetComponent<T>();
			}
			return default(T);
		}

		public T Create<T>(T primitive, MRUKAnchor anchor, Transform parent = null, bool instantiateInWorldSpace = false) where T : Component
		{
			GameObject gameObject = this.Create(primitive.gameObject, anchor, parent, instantiateInWorldSpace);
			if (!(gameObject == null))
			{
				return gameObject.GetComponent<T>();
			}
			return default(T);
		}

		public void Release(GameObject go)
		{
			PoolManagerComponent.PoolableData poolableData;
			if (go.TryGetComponent<PoolManagerComponent.PoolableData>(out poolableData) && poolableData.Pool != null)
			{
				go.transform.localScale = poolableData.Scale;
				poolableData.Anchor = null;
				poolableData.Pool.Release(go);
				return;
			}
			Object.Destroy(go);
		}

		public static readonly Pool<GameObject>.Callbacks DEFAULT_CALLBACKS = new Pool<GameObject>.Callbacks
		{
			Create = new Func<GameObject, GameObject>(PoolManagerComponent.DefaultCallbacks.Create),
			OnGet = new Action<GameObject>(PoolManagerComponent.DefaultCallbacks.OnGet),
			OnRelease = new Action<GameObject>(PoolManagerComponent.DefaultCallbacks.OnRelease)
		};

		[SerializeField]
		internal PoolManagerComponent.PoolDesc[] defaultPools;

		[NonSerialized]
		public PoolManager<GameObject, Pool<GameObject>> poolManager = new PoolManager<GameObject, Pool<GameObject>>();

		[Serializable]
		public abstract class CallbackProvider : MonoBehaviour
		{
			public abstract Pool<GameObject>.Callbacks GetPoolCallbacks();
		}

		[Serializable]
		internal class PoolableData : MonoBehaviour
		{
			internal Pool<GameObject> Pool;

			internal Vector3 Scale;

			internal MRUKAnchor Anchor;
		}

		[Serializable]
		internal struct PoolDesc
		{
			public PoolManagerComponent.PoolDesc.PoolType poolType;

			public GameObject primitive;

			public int size;

			public PoolManagerComponent.CallbackProvider callbackProviderOverride;

			public enum PoolType
			{
				CIRCULAR,
				FIXED
			}
		}

		private static class DefaultCallbacks
		{
			public static GameObject Create(GameObject primitive)
			{
				bool activeSelf = primitive.activeSelf;
				primitive.SetActive(false);
				GameObject result = Object.Instantiate<GameObject>(primitive, Vector3.zero, Quaternion.identity);
				primitive.SetActive(activeSelf);
				return result;
			}

			public static void OnGet(GameObject go)
			{
				go.SetActive(true);
			}

			public static void OnRelease(GameObject go)
			{
				go.SetActive(false);
			}
		}
	}
}
