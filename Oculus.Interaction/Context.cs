using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Oculus.Interaction
{
	public class Context : MonoBehaviour
	{
		public static void ExecuteOnMainThread(Action work)
		{
			Context._unityMainThreadWorkMutex.WaitOne();
			if (Context._unityMainThreadSynchronizationContext != null)
			{
				Context._unityMainThreadSynchronizationContext.Post(delegate(object _)
				{
					work();
				}, null);
			}
			else
			{
				Context._unityMainThreadWork.Enqueue(work);
			}
			Context._unityMainThreadWorkMutex.ReleaseMutex();
		}

		public static Context.Instance Global { get; } = new Context.Instance("Global Context");

		public event Action WhenDestroyed;

		private void Awake()
		{
			if (Context._unityMainThreadSynchronizationContext == null)
			{
				Context._unityMainThreadWorkMutex.WaitOne();
				Context._unityMainThreadSynchronizationContext = SynchronizationContext.Current;
				Action action;
				while (Context._unityMainThreadWork.TryDequeue(out action))
				{
					action();
				}
				Context._unityMainThreadWorkMutex.ReleaseMutex();
			}
		}

		public T GetOrCreateSingleton<T>() where T : class, new()
		{
			Type typeFromHandle = typeof(T);
			object obj;
			if (!this._singletons.TryGetValue(typeFromHandle, out obj))
			{
				obj = Activator.CreateInstance<T>();
				this._singletons.TryAdd(typeFromHandle, obj);
			}
			return obj as T;
		}

		public T GetOrCreateSingleton<T>(Func<T> factory) where T : class
		{
			Type typeFromHandle = typeof(T);
			object obj;
			if (!this._singletons.TryGetValue(typeFromHandle, out obj))
			{
				obj = factory();
				this._singletons.TryAdd(typeFromHandle, obj);
			}
			return obj as T;
		}

		private void OnDestroy()
		{
			Action whenDestroyed = this.WhenDestroyed;
			if (whenDestroyed == null)
			{
				return;
			}
			whenDestroyed();
		}

		private static SynchronizationContext _unityMainThreadSynchronizationContext = null;

		private static Queue<Action> _unityMainThreadWork = new Queue<Action>();

		private static Mutex _unityMainThreadWorkMutex = new Mutex();

		private readonly ConcurrentDictionary<Type, object> _singletons = new ConcurrentDictionary<Type, object>();

		public class Instance
		{
			public Instance(string name)
			{
				this._name = name;
			}

			public Context GetInstance()
			{
				if (this._instance == null)
				{
					GameObject gameObject = new GameObject();
					gameObject.name = this._name;
					if (Application.isPlaying)
					{
						Object.DontDestroyOnLoad(gameObject);
					}
					this._instance = gameObject.AddComponent<Context>();
				}
				return this._instance;
			}

			private readonly string _name;

			private Context _instance;
		}
	}
}
