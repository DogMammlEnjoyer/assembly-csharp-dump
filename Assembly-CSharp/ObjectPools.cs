using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPools : MonoBehaviour, IBuildValidation
{
	public bool initialized { get; private set; }

	protected void Awake()
	{
		ObjectPools.instance = this;
	}

	protected void Start()
	{
		this.InitializePools();
	}

	public void InitializePools()
	{
		if (this.initialized)
		{
			return;
		}
		this.lookUp = new Dictionary<int, SinglePool>();
		foreach (SinglePool singlePool in this.pools)
		{
			singlePool.Initialize(base.gameObject);
			int num = singlePool.PoolGUID();
			if (this.lookUp.ContainsKey(num))
			{
				using (List<SinglePool>.Enumerator enumerator2 = this.pools.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						SinglePool singlePool2 = enumerator2.Current;
						if (singlePool2.PoolGUID() == num)
						{
							Debug.LogError("Pools contain more then one instance of the same object\n" + string.Format("First object in question is {0} tag: {1}\n", singlePool2.objectToPool, singlePool2.objectToPool.tag) + string.Format("Second object is {0} tag: {1}", singlePool.objectToPool, singlePool.objectToPool.tag));
							break;
						}
					}
					continue;
				}
			}
			this.lookUp.Add(singlePool.PoolGUID(), singlePool);
		}
		this.initialized = true;
	}

	public bool DoesPoolExist(GameObject obj)
	{
		return this.DoesPoolExist(PoolUtils.GameObjHashCode(obj));
	}

	public bool DoesPoolExist(int hash)
	{
		return this.lookUp.ContainsKey(hash);
	}

	public SinglePool GetPoolByHash(int hash)
	{
		return this.lookUp[hash];
	}

	public SinglePool GetPoolByObjectType(GameObject obj)
	{
		int hash = PoolUtils.GameObjHashCode(obj);
		return this.GetPoolByHash(hash);
	}

	public GameObject Instantiate(GameObject obj, bool setActive = true)
	{
		return this.GetPoolByObjectType(obj).Instantiate(setActive);
	}

	public GameObject Instantiate(int hash, bool setActive = true)
	{
		return this.GetPoolByHash(hash).Instantiate(setActive);
	}

	public GameObject Instantiate(int hash, Vector3 position, bool setActive = true)
	{
		GameObject gameObject = this.Instantiate(hash, setActive);
		gameObject.transform.position = position;
		return gameObject;
	}

	public GameObject Instantiate(int hash, Vector3 position, Quaternion rotation, bool setActive = true)
	{
		GameObject gameObject = this.Instantiate(hash, setActive);
		gameObject.transform.SetPositionAndRotation(position, rotation);
		return gameObject;
	}

	public GameObject Instantiate(GameObject obj, Vector3 position, bool setActive = true)
	{
		GameObject gameObject = this.Instantiate(obj, setActive);
		gameObject.transform.position = position;
		return gameObject;
	}

	public GameObject Instantiate(GameObject obj, Vector3 position, Quaternion rotation, bool setActive = true)
	{
		GameObject gameObject = this.Instantiate(obj, setActive);
		gameObject.transform.SetPositionAndRotation(position, rotation);
		return gameObject;
	}

	public GameObject Instantiate(GameObject obj, Vector3 position, Quaternion rotation, float scale, bool setActive = true)
	{
		GameObject gameObject = this.Instantiate(obj, setActive);
		gameObject.transform.SetPositionAndRotation(position, rotation);
		gameObject.transform.localScale = Vector3.one * scale;
		return gameObject;
	}

	public void Destroy(GameObject obj)
	{
		this.GetPoolByObjectType(obj).Destroy(obj);
	}

	public bool BuildValidationCheck()
	{
		bool result = true;
		using (List<SinglePool>.Enumerator enumerator = this.pools.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				SinglePool pool = enumerator.Current;
				if (pool.objectToPool == null)
				{
					Debug.Log("GlobalObjectPools contains a nullref. Failing build validation.");
					result = false;
				}
				else
				{
					DelayedDestroyPooledObj[] componentsInChildren = pool.objectToPool.GetComponentsInChildren<DelayedDestroyPooledObj>(true);
					if (componentsInChildren.Length > 1)
					{
						Debug.LogError(string.Format("Pooled prefab '{0}' has {1} ", pool.objectToPool.name, componentsInChildren.Length) + "DelayedDestroyPooledObj components in its hierarchy. Only the root should have one. Children with their own will try to pool-destroy themselves and spam 'not contained in the activePool' errors. Extra components on:" + string.Concat(Array.ConvertAll<DelayedDestroyPooledObj, string>(componentsInChildren, delegate(DelayedDestroyPooledObj c)
						{
							if (!(c.gameObject == pool.objectToPool))
							{
								return "\n  - " + c.gameObject.name;
							}
							return "";
						})), pool.objectToPool);
						result = false;
					}
				}
			}
		}
		return result;
	}

	public static int InstantiateDelayed(GameObject prefab, Vector3 pos, float delay)
	{
		return ObjectPools.InstantiateDelayed(prefab, null, pos, delay);
	}

	public static int InstantiateDelayed(GameObject prefab, Transform xform, Vector3 localPos, float delay)
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return -1;
		}
		int num;
		if (ObjectPools._delayedFreeHead >= 0)
		{
			num = ObjectPools._delayedFreeHead;
			ObjectPools._delayedFreeHead = ObjectPools._delayedFreeNext[num];
		}
		else
		{
			if (ObjectPools._delayedHighWater >= ObjectPools._delayedData.Length)
			{
				int newSize = ObjectPools._delayedData.Length * 2;
				Array.Resize<ObjectPools.DelayedSpawnData>(ref ObjectPools._delayedData, newSize);
				Array.Resize<int>(ref ObjectPools._delayedFreeNext, newSize);
			}
			num = ObjectPools._delayedHighWater++;
		}
		ObjectPools._delayedData[num] = new ObjectPools.DelayedSpawnData
		{
			prefabHash = PoolUtils.GameObjHashCode(prefab),
			xform = xform,
			pos = localPos
		};
		GTDelayedExec.Add(ObjectPools._delayedListener, delay, num);
		return num;
	}

	public static void UpdateDelayedInstantiate(int idx, Transform xform)
	{
		if (idx >= ObjectPools._delayedHighWater)
		{
			return;
		}
		ObjectPools._delayedData[idx].xform = xform;
	}

	public static void UpdateDelayedInstantiate(int idx, Vector3 localPos)
	{
		if (idx >= ObjectPools._delayedHighWater)
		{
			return;
		}
		ObjectPools._delayedData[idx].pos = localPos;
	}

	public static void CancelDelayedInstantiate(int idx)
	{
		if (idx >= ObjectPools._delayedHighWater)
		{
			return;
		}
		ObjectPools._delayedData[idx].prefabHash = 0;
	}

	public static void UpdateDelayedInstantiate(int idx, Transform xform, Vector3 localPos)
	{
		if (idx >= ObjectPools._delayedHighWater)
		{
			return;
		}
		ObjectPools.DelayedSpawnData[] delayedData = ObjectPools._delayedData;
		delayedData[idx].xform = xform;
		delayedData[idx].pos = localPos;
	}

	public static ObjectPools instance = null;

	[SerializeField]
	private List<SinglePool> pools;

	private Dictionary<int, SinglePool> lookUp;

	private const int k_delayedInitialCount = 16;

	[OnEnterPlay_Set(0)]
	private static int _delayedHighWater;

	[OnEnterPlay_Set(-1)]
	private static int _delayedFreeHead = -1;

	[OnEnterPlay_SetNew]
	private static ObjectPools.DelayedSpawnData[] _delayedData = new ObjectPools.DelayedSpawnData[16];

	[OnEnterPlay_SetNew]
	private static int[] _delayedFreeNext = new int[16];

	[OnEnterPlay_SetNew]
	private static readonly ObjectPools.DelayedSpawnListener _delayedListener = new ObjectPools.DelayedSpawnListener();

	private struct DelayedSpawnData
	{
		public int prefabHash;

		public Transform xform;

		public Vector3 pos;
	}

	private class DelayedSpawnListener : IDelayedExecListener
	{
		public void OnDelayedAction(int contextId)
		{
			if (contextId >= ObjectPools._delayedHighWater)
			{
				return;
			}
			ref ObjectPools.DelayedSpawnData ptr = ref ObjectPools._delayedData[contextId];
			if (ptr.prefabHash != 0 && ObjectPools.instance != null)
			{
				Vector3 position = (ptr.xform != null) ? ptr.xform.TransformPoint(ptr.pos) : ptr.pos;
				ObjectPools.instance.Instantiate(ptr.prefabHash, position, true);
			}
			ptr = default(ObjectPools.DelayedSpawnData);
			ObjectPools._delayedFreeNext[contextId] = ObjectPools._delayedFreeHead;
			ObjectPools._delayedFreeHead = contextId;
		}
	}
}
