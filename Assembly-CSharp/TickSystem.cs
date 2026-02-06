using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaTag;
using UnityEngine;

[DefaultExecutionOrder(0)]
internal abstract class TickSystem<T> : MonoBehaviour
{
	private void Awake()
	{
		base.transform.SetParent(null, true);
		Object.DontDestroyOnLoad(this);
	}

	private void Update()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		TickSystem<T>.preTickCallbacks.TryRunCallbacks();
		TickSystem<T>.tickCallbacks.TryRunCallbacks();
	}

	private void LateUpdate()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		TickSystem<T>.postTickCallbacks.TryRunCallbacks();
	}

	static TickSystem()
	{
		TickSystem<T>.preTickWrapperPool = new ObjectPool<TickSystem<T>.TickCallbackWrapperPre>(100);
		TickSystem<T>.tickWrapperPool = new ObjectPool<TickSystem<T>.TickCallbackWrapperTick>(100);
		TickSystem<T>.postTickWrapperPool = new ObjectPool<TickSystem<T>.TickCallbackWrapperPost>(100);
	}

	private static void OnEnterPlay()
	{
		TickSystem<T>.preTickCallbacks.Clear();
		TickSystem<T>.preTickWrapperTable.Clear();
		TickSystem<T>.tickCallbacks.Clear();
		TickSystem<T>.tickWrapperTable.Clear();
		TickSystem<T>.postTickCallbacks.Clear();
		TickSystem<T>.postTickWrapperTable.Clear();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddPreTickCallback(ITickSystemPre callback)
	{
		if (callback.PreTickRunning)
		{
			return;
		}
		TickSystem<T>.TickCallbackWrapperPre tickCallbackWrapperPre = TickSystem<T>.preTickWrapperPool.Take();
		tickCallbackWrapperPre.target = callback;
		TickSystem<T>.preTickWrapperTable[callback] = tickCallbackWrapperPre;
		TickSystem<T>.preTickCallbacks.Add(tickCallbackWrapperPre);
		callback.PreTickRunning = true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddTickCallback(ITickSystemTick callback)
	{
		if (callback.TickRunning)
		{
			return;
		}
		TickSystem<T>.TickCallbackWrapperTick tickCallbackWrapperTick = TickSystem<T>.tickWrapperPool.Take();
		tickCallbackWrapperTick.target = callback;
		TickSystem<T>.tickWrapperTable[callback] = tickCallbackWrapperTick;
		TickSystem<T>.tickCallbacks.Add(tickCallbackWrapperTick);
		callback.TickRunning = true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddPostTickCallback(ITickSystemPost callback)
	{
		if (callback.PostTickRunning)
		{
			return;
		}
		TickSystem<T>.TickCallbackWrapperPost tickCallbackWrapperPost = TickSystem<T>.postTickWrapperPool.Take();
		tickCallbackWrapperPost.target = callback;
		TickSystem<T>.postTickWrapperTable[callback] = tickCallbackWrapperPost;
		TickSystem<T>.postTickCallbacks.Add(tickCallbackWrapperPost);
		callback.PostTickRunning = true;
	}

	public static void AddTickSystemCallBack(ITickSystem callback)
	{
		TickSystem<T>.AddPreTickCallback(callback);
		TickSystem<T>.AddTickCallback(callback);
		TickSystem<T>.AddPostTickCallback(callback);
	}

	public static void AddCallbackTarget(object target)
	{
		ITickSystem tickSystem = target as ITickSystem;
		if (tickSystem != null)
		{
			TickSystem<T>.AddTickSystemCallBack(tickSystem);
			return;
		}
		ITickSystemPre tickSystemPre = target as ITickSystemPre;
		if (tickSystemPre != null)
		{
			TickSystem<T>.AddPreTickCallback(tickSystemPre);
		}
		ITickSystemTick tickSystemTick = target as ITickSystemTick;
		if (tickSystemTick != null)
		{
			TickSystem<T>.AddTickCallback(tickSystemTick);
		}
		ITickSystemPost tickSystemPost = target as ITickSystemPost;
		if (tickSystemPost != null)
		{
			TickSystem<T>.AddPostTickCallback(tickSystemPost);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemovePreTickCallback(ITickSystemPre callback)
	{
		TickSystem<T>.TickCallbackWrapperPre instance;
		if (!callback.PreTickRunning || !TickSystem<T>.preTickWrapperTable.TryGetValue(callback, out instance))
		{
			return;
		}
		TickSystem<T>.preTickCallbacks.Remove(instance);
		callback.PreTickRunning = false;
		TickSystem<T>.preTickWrapperPool.Return(instance);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemoveTickCallback(ITickSystemTick callback)
	{
		TickSystem<T>.TickCallbackWrapperTick instance;
		if (!callback.TickRunning || !TickSystem<T>.tickWrapperTable.TryGetValue(callback, out instance))
		{
			return;
		}
		TickSystem<T>.tickCallbacks.Remove(instance);
		callback.TickRunning = false;
		TickSystem<T>.tickWrapperPool.Return(instance);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RemovePostTickCallback(ITickSystemPost callback)
	{
		TickSystem<T>.TickCallbackWrapperPost instance;
		if (!callback.PostTickRunning || !TickSystem<T>.postTickWrapperTable.TryGetValue(callback, out instance))
		{
			return;
		}
		TickSystem<T>.postTickCallbacks.Remove(instance);
		callback.PostTickRunning = false;
		TickSystem<T>.postTickWrapperPool.Return(instance);
	}

	public static void RemoveTickSystemCallback(ITickSystem callback)
	{
		TickSystem<T>.RemovePreTickCallback(callback);
		TickSystem<T>.RemoveTickCallback(callback);
		TickSystem<T>.RemovePostTickCallback(callback);
	}

	public static void RemoveCallbackTarget(object target)
	{
		ITickSystem tickSystem = target as ITickSystem;
		if (tickSystem != null)
		{
			TickSystem<T>.RemoveTickSystemCallback(tickSystem);
			return;
		}
		ITickSystemPre tickSystemPre = target as ITickSystemPre;
		if (tickSystemPre != null)
		{
			TickSystem<T>.RemovePreTickCallback(tickSystemPre);
		}
		ITickSystemTick tickSystemTick = target as ITickSystemTick;
		if (tickSystemTick != null)
		{
			TickSystem<T>.RemoveTickCallback(tickSystemTick);
		}
		ITickSystemPost tickSystemPost = target as ITickSystemPost;
		if (tickSystemPost != null)
		{
			TickSystem<T>.RemovePostTickCallback(tickSystemPost);
		}
	}

	private static readonly ObjectPool<TickSystem<T>.TickCallbackWrapperPre> preTickWrapperPool;

	private static readonly CallbackContainer<TickSystem<T>.TickCallbackWrapperPre> preTickCallbacks = new CallbackContainer<TickSystem<T>.TickCallbackWrapperPre>();

	private static readonly Dictionary<ITickSystemPre, TickSystem<T>.TickCallbackWrapperPre> preTickWrapperTable = new Dictionary<ITickSystemPre, TickSystem<T>.TickCallbackWrapperPre>(100);

	private static readonly ObjectPool<TickSystem<T>.TickCallbackWrapperTick> tickWrapperPool;

	private static readonly CallbackContainer<TickSystem<T>.TickCallbackWrapperTick> tickCallbacks = new CallbackContainer<TickSystem<T>.TickCallbackWrapperTick>();

	private static readonly Dictionary<ITickSystemTick, TickSystem<T>.TickCallbackWrapperTick> tickWrapperTable = new Dictionary<ITickSystemTick, TickSystem<T>.TickCallbackWrapperTick>(100);

	private static readonly ObjectPool<TickSystem<T>.TickCallbackWrapperPost> postTickWrapperPool;

	private static readonly CallbackContainer<TickSystem<T>.TickCallbackWrapperPost> postTickCallbacks = new CallbackContainer<TickSystem<T>.TickCallbackWrapperPost>();

	private static readonly Dictionary<ITickSystemPost, TickSystem<T>.TickCallbackWrapperPost> postTickWrapperTable = new Dictionary<ITickSystemPost, TickSystem<T>.TickCallbackWrapperPost>(100);

	private abstract class TickCallbackWrapper<U> : ObjectPoolEvents, ICallBack where U : class
	{
		public U target
		{
			get
			{
				return this.m_target;
			}
			set
			{
				this.m_target = value;
			}
		}

		public abstract void CallBack();

		public void OnTaken()
		{
		}

		public void OnReturned()
		{
			this.target = default(U);
		}

		protected U m_target;
	}

	private class TickCallbackWrapperPre : TickSystem<T>.TickCallbackWrapper<ITickSystemPre>
	{
		public override void CallBack()
		{
			this.m_target.PreTick();
		}
	}

	private class TickCallbackWrapperTick : TickSystem<T>.TickCallbackWrapper<ITickSystemTick>
	{
		public override void CallBack()
		{
			this.m_target.Tick();
		}
	}

	private class TickCallbackWrapperPost : TickSystem<T>.TickCallbackWrapper<ITickSystemPost>
	{
		public override void CallBack()
		{
			this.m_target.PostTick();
		}
	}
}
