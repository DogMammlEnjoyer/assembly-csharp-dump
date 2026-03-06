using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

public static class OVRTask
{
	public static OVRTask<TResult[]> WhenAll<TResult>(IEnumerable<OVRTask<TResult>> tasks)
	{
		return OVRTask<TResult>.WhenAll(tasks);
	}

	public static OVRTask<List<TResult>> WhenAll<TResult>(IEnumerable<OVRTask<TResult>> tasks, List<TResult> results)
	{
		return OVRTask<TResult>.WhenAll(tasks, results);
	}

	internal static OVRTask<TResult> FromGuid<TResult>(Guid id)
	{
		return OVRTask.Create<TResult>(id);
	}

	[Obsolete("Consider OVRTask.Build instead.")]
	internal static OVRTask<TResult> FromRequest<TResult>(ulong id)
	{
		return OVRTask.Create<TResult>(OVRTask.GetId(id));
	}

	[Obsolete("Consider OVRTask.Build instead.")]
	internal static OVRTask<TResult> FromRequest<TResult>(ulong id, OVRPlugin.EventType eventType)
	{
		return OVRTask.Create<TResult>(OVRTask.GetId(id, eventType));
	}

	internal static OVRTask.Builder Build(bool success, ulong requestId)
	{
		return new OVRTask.Builder(success ? OVRPlugin.Result.Success : OVRPlugin.Result.Failure, OVRTask.GetId(requestId));
	}

	internal static OVRTask.Builder Build(OVRPlugin.Result result, ulong requestId)
	{
		return new OVRTask.Builder(result, OVRTask.GetId(requestId));
	}

	internal static OVRTask.Builder Build(OVRPlugin.Result result, ulong requestId, OVRPlugin.EventType eventType)
	{
		return new OVRTask.Builder(result, OVRTask.GetId(requestId, eventType));
	}

	public static OVRTask<TResult> FromResult<TResult>(TResult result)
	{
		OVRTask<TResult> result2 = OVRTask.Create<TResult>(Guid.NewGuid());
		result2.SetResult(result);
		return result2;
	}

	[Obsolete("This method does not ensure the task exists; it just returns an OVRTask with the given id. Use TryGetPending instead.", true)]
	internal static OVRTask<TResult> GetExisting<TResult>(Guid id)
	{
		return OVRTask.Get<TResult>(id);
	}

	internal static bool TryGetPendingTask<TResult>(Guid id, out OVRTask<TResult> task)
	{
		task = OVRTask.Get<TResult>(id);
		return task.IsPending;
	}

	[Obsolete("This method does not ensure the task exists; it just returns an OVRTask with the given id. Use TryGetPending instead.", true)]
	internal static OVRTask<TResult> GetExisting<TResult>(ulong id)
	{
		return OVRTask.Get<TResult>(OVRTask.GetId(id));
	}

	internal static bool TryGetPendingTask<TResult>(ulong id, out OVRTask<TResult> task)
	{
		return OVRTask.TryGetPendingTask<TResult>(OVRTask.GetId(id), out task);
	}

	public static void SetResult<TResult>(Guid id, TResult result)
	{
		OVRTask<TResult> ovrtask = OVRTask.Get<TResult>(id);
		if (ovrtask.HasResult)
		{
			throw new InvalidOperationException(string.Format("Task {0} already has a result.", id));
		}
		ovrtask.SetResult(result);
	}

	internal static void SetResult<TResult>(ulong id, TResult result)
	{
		OVRTask.Get<TResult>(OVRTask.GetId(id)).SetResult(result);
	}

	private static OVRTask<TResult> Get<TResult>(Guid id)
	{
		return new OVRTask<TResult>(id);
	}

	public static OVRTask<TResult> Create<TResult>(Guid taskId)
	{
		OVRTask.RegisterType<TResult>();
		OVRTask<TResult> result = OVRTask.Get<TResult>(taskId);
		if (!result.AddToPending())
		{
			throw new ArgumentException(string.Format("The task with id {0} already exists.", taskId), "taskId");
		}
		return result;
	}

	internal unsafe static Guid GetId(ulong part1, ulong part2)
	{
		IntPtr intPtr = stackalloc byte[(UIntPtr)16];
		*intPtr = (long)(part1 + 3573116690164977347UL);
		*(intPtr + 8) = (long)(part2 + 10871156337175269513UL);
		return *intPtr;
	}

	internal static Guid GetId(ulong handle, OVRPlugin.EventType eventType)
	{
		return OVRTask.GetId(handle, (ulong)((long)eventType));
	}

	internal static Guid GetId(ulong value)
	{
		return OVRTask.GetId(value, 0UL);
	}

	internal static ulong GetId(Guid value)
	{
		return OVRTask.GetIdParts(value).Item1;
	}

	internal unsafe static ValueTuple<ulong, ulong> GetIdParts(Guid id)
	{
		ulong* ptr = stackalloc ulong[(UIntPtr)16];
		UnsafeUtility.MemCpy((void*)ptr, (void*)(&id), (long)sizeof(Guid));
		return new ValueTuple<ulong, ulong>(*ptr - 3573116690164977347UL, ptr[1] - 10871156337175269513UL);
	}

	internal static void RegisterType<TResult>()
	{
	}

	public static OVRTask<ValueTuple<T1, T2>> WhenAll<T1, T2>(OVRTask<T1> task1, OVRTask<T2> task2)
	{
		return OVRTask.MultiTaskData<T1, T2>.Get(task1, task2);
	}

	public static OVRTask<ValueTuple<T1, T2, T3>> WhenAll<T1, T2, T3>(OVRTask<T1> task1, OVRTask<T2> task2, OVRTask<T3> task3)
	{
		return OVRTask.MultiTaskData<T1, T2, T3>.Get(task1, task2, task3);
	}

	public static OVRTask<ValueTuple<T1, T2, T3, T4>> WhenAll<T1, T2, T3, T4>(OVRTask<T1> task1, OVRTask<T2> task2, OVRTask<T3> task3, OVRTask<T4> task4)
	{
		return OVRTask.MultiTaskData<T1, T2, T3, T4>.Get(task1, task2, task3, task4);
	}

	public static OVRTask<ValueTuple<T1, T2, T3, T4, T5>> WhenAll<T1, T2, T3, T4, T5>(OVRTask<T1> task1, OVRTask<T2> task2, OVRTask<T3> task3, OVRTask<T4> task4, OVRTask<T5> task5)
	{
		return OVRTask.MultiTaskData<T1, T2, T3, T4, T5>.Get(task1, task2, task3, task4, task5);
	}

	public static OVRTask<ValueTuple<T1, T2, T3, T4, T5, T6>> WhenAll<T1, T2, T3, T4, T5, T6>(OVRTask<T1> task1, OVRTask<T2> task2, OVRTask<T3> task3, OVRTask<T4> task4, OVRTask<T5> task5, OVRTask<T6> task6)
	{
		return OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>.Get(task1, task2, task3, task4, task5, task6);
	}

	public static OVRTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7>> WhenAll<T1, T2, T3, T4, T5, T6, T7>(OVRTask<T1> task1, OVRTask<T2> task2, OVRTask<T3> task3, OVRTask<T4> task4, OVRTask<T5> task5, OVRTask<T6> task6, OVRTask<T7> task7)
	{
		return OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>.Get(task1, task2, task3, task4, task5, task6, task7);
	}

	public static OVRTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8>>> WhenAll<T1, T2, T3, T4, T5, T6, T7, T8>(OVRTask<T1> task1, OVRTask<T2> task2, OVRTask<T3> task3, OVRTask<T4> task4, OVRTask<T5> task5, OVRTask<T6> task6, OVRTask<T7> task7, OVRTask<T8> task8)
	{
		return OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>.Get(task1, task2, task3, task4, task5, task6, task7, task8);
	}

	private const ulong HashModifier1 = 3573116690164977347UL;

	private const ulong HashModifier2 = 10871156337175269513UL;

	internal readonly struct Builder
	{
		public Builder(OVRPlugin.Result synchronousResult, Guid taskId)
		{
			this._synchronousResult = synchronousResult;
			this._taskId = taskId;
		}

		public OVRTask<OVRPlugin.Result> ToTask()
		{
			return this.ToTask<OVRPlugin.Result>(this._synchronousResult);
		}

		public OVRTask<TStatus> ToTask<TStatus>() where TStatus : struct, Enum
		{
			return this.ToTask<TStatus>(this.CastResult<TStatus>());
		}

		public OVRTask<TResult> ToTask<TResult>(TResult failureValue)
		{
			if (!this._synchronousResult.IsSuccess())
			{
				return OVRTask.FromResult<TResult>(failureValue);
			}
			return OVRTask.FromGuid<TResult>(this._taskId);
		}

		public OVRTask<OVRResult<TStatus>> ToResultTask<TStatus>() where TStatus : struct, Enum
		{
			return this.ToTask<OVRResult<TStatus>>(this._synchronousResult.IsSuccess() ? default(OVRResult<TStatus>) : OVRResult<TStatus>.FromFailure(this.CastResult<TStatus>()));
		}

		public OVRTask<OVRResult<TValue, TStatus>> ToTask<TValue, TStatus>() where TStatus : struct, Enum
		{
			return this.ToTask<OVRResult<TValue, TStatus>>(this._synchronousResult.IsSuccess() ? default(OVRResult<TValue, TStatus>) : OVRResult<TValue, TStatus>.FromFailure(this.CastResult<TStatus>()));
		}

		private unsafe TResult CastResult<TResult>() where TResult : struct, Enum
		{
			Type enumUnderlyingType = typeof(TResult).GetEnumUnderlyingType();
			if (enumUnderlyingType != typeof(int) && enumUnderlyingType != typeof(uint))
			{
				throw new InvalidCastException(typeof(TResult).Name + " must have an underlying type of Int32 or UInt32.");
			}
			OVRPlugin.Result synchronousResult = this._synchronousResult;
			return *UnsafeUtility.As<OVRPlugin.Result, TResult>(ref synchronousResult);
		}

		private readonly OVRPlugin.Result _synchronousResult;

		private readonly Guid _taskId;
	}

	private class MultiTaskData<T> : OVRObjectPool.IPoolObject
	{
		void OVRObjectPool.IPoolObject.OnGet()
		{
			this.CombinedTask = OVRTask.FromGuid<T>(Guid.NewGuid());
			this.Result = default(T);
			this.Remaining = OVRObjectPool.HashSet<Guid>();
		}

		void OVRObjectPool.IPoolObject.OnReturn()
		{
			this.Result = default(T);
			OVRObjectPool.Return<Guid>(this.Remaining);
		}

		protected void AddTask(Guid id)
		{
			this.Remaining.Add(id);
		}

		protected void OnResult(Guid taskId)
		{
			this.Remaining.Remove(taskId);
			if (this.Remaining.Count != 0)
			{
				return;
			}
			try
			{
				this.CombinedTask.SetResult(this.Result);
			}
			finally
			{
				OVRObjectPool.Return<OVRTask.MultiTaskData<T>>(this);
			}
		}

		protected OVRTask<T> CombinedTask;

		protected T Result;

		protected HashSet<Guid> Remaining;
	}

	private class MultiTaskData<T1, T2> : OVRTask.MultiTaskData<ValueTuple<T1, T2>>
	{
		public static OVRTask<ValueTuple<T1, T2>> Get(OVRTask<T1> task1, OVRTask<T2> task2)
		{
			OVRTask.MultiTaskData<T1, T2> multiTaskData = OVRObjectPool.Get<OVRTask.MultiTaskData<T1, T2>>();
			multiTaskData.AddTask(task1._id);
			multiTaskData.AddTask(task2._id);
			task1.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2>>>(OVRTask.MultiTaskData<T1, T2>._onResult1, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2>>(task1._id, multiTaskData));
			task2.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2>>>(OVRTask.MultiTaskData<T1, T2>._onResult2, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2>>(task2._id, multiTaskData));
			return multiTaskData.CombinedTask;
		}

		private static Action<T1, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2>>> _onResult1 = delegate(T1 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2>> data)
		{
			data.Item2.Result.Item1 = result;
			data.Item2.OnResult(data.Item1);
		};

		private static Action<T2, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2>>> _onResult2 = delegate(T2 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2>> data)
		{
			data.Item2.Result.Item2 = result;
			data.Item2.OnResult(data.Item1);
		};
	}

	private class MultiTaskData<T1, T2, T3> : OVRTask.MultiTaskData<ValueTuple<T1, T2, T3>>
	{
		public static OVRTask<ValueTuple<T1, T2, T3>> Get(OVRTask<T1> task1, OVRTask<T2> task2, OVRTask<T3> task3)
		{
			OVRTask.MultiTaskData<T1, T2, T3> multiTaskData = OVRObjectPool.Get<OVRTask.MultiTaskData<T1, T2, T3>>();
			multiTaskData.AddTask(task1._id);
			multiTaskData.AddTask(task2._id);
			multiTaskData.AddTask(task3._id);
			task1.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3>>>(OVRTask.MultiTaskData<T1, T2, T3>._onResult1, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3>>(task1._id, multiTaskData));
			task2.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3>>>(OVRTask.MultiTaskData<T1, T2, T3>._onResult2, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3>>(task2._id, multiTaskData));
			task3.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3>>>(OVRTask.MultiTaskData<T1, T2, T3>._onResult3, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3>>(task3._id, multiTaskData));
			return multiTaskData.CombinedTask;
		}

		private static Action<T1, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3>>> _onResult1 = delegate(T1 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3>> data)
		{
			data.Item2.Result.Item1 = result;
			data.Item2.OnResult(data.Item1);
		};

		private static Action<T2, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3>>> _onResult2 = delegate(T2 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3>> data)
		{
			data.Item2.Result.Item2 = result;
			data.Item2.OnResult(data.Item1);
		};

		private static Action<T3, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3>>> _onResult3 = delegate(T3 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3>> data)
		{
			data.Item2.Result.Item3 = result;
			data.Item2.OnResult(data.Item1);
		};
	}

	private class MultiTaskData<T1, T2, T3, T4> : OVRTask.MultiTaskData<ValueTuple<T1, T2, T3, T4>>
	{
		public static OVRTask<ValueTuple<T1, T2, T3, T4>> Get(OVRTask<T1> task1, OVRTask<T2> task2, OVRTask<T3> task3, OVRTask<T4> task4)
		{
			OVRTask.MultiTaskData<T1, T2, T3, T4> multiTaskData = OVRObjectPool.Get<OVRTask.MultiTaskData<T1, T2, T3, T4>>();
			multiTaskData.AddTask(task1._id);
			multiTaskData.AddTask(task2._id);
			multiTaskData.AddTask(task3._id);
			multiTaskData.AddTask(task4._id);
			task1.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4>>>(OVRTask.MultiTaskData<T1, T2, T3, T4>._onResult1, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4>>(task1._id, multiTaskData));
			task2.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4>>>(OVRTask.MultiTaskData<T1, T2, T3, T4>._onResult2, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4>>(task2._id, multiTaskData));
			task3.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4>>>(OVRTask.MultiTaskData<T1, T2, T3, T4>._onResult3, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4>>(task3._id, multiTaskData));
			task4.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4>>>(OVRTask.MultiTaskData<T1, T2, T3, T4>._onResult4, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4>>(task4._id, multiTaskData));
			return multiTaskData.CombinedTask;
		}

		private static Action<T1, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4>>> _onResult1 = delegate(T1 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4>> data)
		{
			data.Item2.Result.Item1 = result;
			data.Item2.OnResult(data.Item1);
		};

		private static Action<T2, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4>>> _onResult2 = delegate(T2 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4>> data)
		{
			data.Item2.Result.Item2 = result;
			data.Item2.OnResult(data.Item1);
		};

		private static Action<T3, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4>>> _onResult3 = delegate(T3 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4>> data)
		{
			data.Item2.Result.Item3 = result;
			data.Item2.OnResult(data.Item1);
		};

		private static Action<T4, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4>>> _onResult4 = delegate(T4 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4>> data)
		{
			data.Item2.Result.Item4 = result;
			data.Item2.OnResult(data.Item1);
		};
	}

	private class MultiTaskData<T1, T2, T3, T4, T5> : OVRTask.MultiTaskData<ValueTuple<T1, T2, T3, T4, T5>>
	{
		public static OVRTask<ValueTuple<T1, T2, T3, T4, T5>> Get(OVRTask<T1> task1, OVRTask<T2> task2, OVRTask<T3> task3, OVRTask<T4> task4, OVRTask<T5> task5)
		{
			OVRTask.MultiTaskData<T1, T2, T3, T4, T5> multiTaskData = OVRObjectPool.Get<OVRTask.MultiTaskData<T1, T2, T3, T4, T5>>();
			multiTaskData.AddTask(task1._id);
			multiTaskData.AddTask(task2._id);
			multiTaskData.AddTask(task3._id);
			multiTaskData.AddTask(task4._id);
			multiTaskData.AddTask(task5._id);
			task1.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5>>>(OVRTask.MultiTaskData<T1, T2, T3, T4, T5>._onResult1, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5>>(task1._id, multiTaskData));
			task2.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5>>>(OVRTask.MultiTaskData<T1, T2, T3, T4, T5>._onResult2, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5>>(task2._id, multiTaskData));
			task3.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5>>>(OVRTask.MultiTaskData<T1, T2, T3, T4, T5>._onResult3, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5>>(task3._id, multiTaskData));
			task4.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5>>>(OVRTask.MultiTaskData<T1, T2, T3, T4, T5>._onResult4, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5>>(task4._id, multiTaskData));
			task5.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5>>>(OVRTask.MultiTaskData<T1, T2, T3, T4, T5>._onResult5, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5>>(task5._id, multiTaskData));
			return multiTaskData.CombinedTask;
		}

		private static Action<T1, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5>>> _onResult1 = delegate(T1 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5>> data)
		{
			data.Item2.Result.Item1 = result;
			data.Item2.OnResult(data.Item1);
		};

		private static Action<T2, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5>>> _onResult2 = delegate(T2 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5>> data)
		{
			data.Item2.Result.Item2 = result;
			data.Item2.OnResult(data.Item1);
		};

		private static Action<T3, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5>>> _onResult3 = delegate(T3 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5>> data)
		{
			data.Item2.Result.Item3 = result;
			data.Item2.OnResult(data.Item1);
		};

		private static Action<T4, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5>>> _onResult4 = delegate(T4 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5>> data)
		{
			data.Item2.Result.Item4 = result;
			data.Item2.OnResult(data.Item1);
		};

		private static Action<T5, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5>>> _onResult5 = delegate(T5 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5>> data)
		{
			data.Item2.Result.Item5 = result;
			data.Item2.OnResult(data.Item1);
		};
	}

	private class MultiTaskData<T1, T2, T3, T4, T5, T6> : OVRTask.MultiTaskData<ValueTuple<T1, T2, T3, T4, T5, T6>>
	{
		public static OVRTask<ValueTuple<T1, T2, T3, T4, T5, T6>> Get(OVRTask<T1> task1, OVRTask<T2> task2, OVRTask<T3> task3, OVRTask<T4> task4, OVRTask<T5> task5, OVRTask<T6> task6)
		{
			OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6> multiTaskData = OVRObjectPool.Get<OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>>();
			multiTaskData.AddTask(task1._id);
			multiTaskData.AddTask(task2._id);
			multiTaskData.AddTask(task3._id);
			multiTaskData.AddTask(task4._id);
			multiTaskData.AddTask(task5._id);
			multiTaskData.AddTask(task6._id);
			task1.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>>>(OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>._onResult1, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>>(task1._id, multiTaskData));
			task2.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>>>(OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>._onResult2, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>>(task2._id, multiTaskData));
			task3.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>>>(OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>._onResult3, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>>(task3._id, multiTaskData));
			task4.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>>>(OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>._onResult4, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>>(task4._id, multiTaskData));
			task5.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>>>(OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>._onResult5, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>>(task5._id, multiTaskData));
			task6.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>>>(OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>._onResult6, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>>(task6._id, multiTaskData));
			return multiTaskData.CombinedTask;
		}

		private static Action<T1, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>>> _onResult1 = delegate(T1 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>> data)
		{
			data.Item2.Result.Item1 = result;
			data.Item2.OnResult(data.Item1);
		};

		private static Action<T2, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>>> _onResult2 = delegate(T2 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>> data)
		{
			data.Item2.Result.Item2 = result;
			data.Item2.OnResult(data.Item1);
		};

		private static Action<T3, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>>> _onResult3 = delegate(T3 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>> data)
		{
			data.Item2.Result.Item3 = result;
			data.Item2.OnResult(data.Item1);
		};

		private static Action<T4, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>>> _onResult4 = delegate(T4 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>> data)
		{
			data.Item2.Result.Item4 = result;
			data.Item2.OnResult(data.Item1);
		};

		private static Action<T5, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>>> _onResult5 = delegate(T5 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>> data)
		{
			data.Item2.Result.Item5 = result;
			data.Item2.OnResult(data.Item1);
		};

		private static Action<T6, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>>> _onResult6 = delegate(T6 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6>> data)
		{
			data.Item2.Result.Item6 = result;
			data.Item2.OnResult(data.Item1);
		};
	}

	private class MultiTaskData<T1, T2, T3, T4, T5, T6, T7> : OVRTask.MultiTaskData<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>
	{
		public static OVRTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7>> Get(OVRTask<T1> task1, OVRTask<T2> task2, OVRTask<T3> task3, OVRTask<T4> task4, OVRTask<T5> task5, OVRTask<T6> task6, OVRTask<T7> task7)
		{
			OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7> multiTaskData = OVRObjectPool.Get<OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>>();
			multiTaskData.AddTask(task1._id);
			multiTaskData.AddTask(task2._id);
			multiTaskData.AddTask(task3._id);
			multiTaskData.AddTask(task4._id);
			multiTaskData.AddTask(task5._id);
			multiTaskData.AddTask(task6._id);
			multiTaskData.AddTask(task7._id);
			task1.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>>>(OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>._onResult1, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>>(task1._id, multiTaskData));
			task2.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>>>(OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>._onResult2, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>>(task2._id, multiTaskData));
			task3.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>>>(OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>._onResult3, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>>(task3._id, multiTaskData));
			task4.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>>>(OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>._onResult4, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>>(task4._id, multiTaskData));
			task5.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>>>(OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>._onResult5, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>>(task5._id, multiTaskData));
			task6.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>>>(OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>._onResult6, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>>(task6._id, multiTaskData));
			task7.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>>>(OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>._onResult7, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>>(task7._id, multiTaskData));
			return multiTaskData.CombinedTask;
		}

		private static Action<T1, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>>> _onResult1 = delegate(T1 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>> data)
		{
			data.Item2.Result.Item1 = result;
			data.Item2.OnResult(data.Item1);
		};

		private static Action<T2, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>>> _onResult2 = delegate(T2 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>> data)
		{
			data.Item2.Result.Item2 = result;
			data.Item2.OnResult(data.Item1);
		};

		private static Action<T3, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>>> _onResult3 = delegate(T3 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>> data)
		{
			data.Item2.Result.Item3 = result;
			data.Item2.OnResult(data.Item1);
		};

		private static Action<T4, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>>> _onResult4 = delegate(T4 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>> data)
		{
			data.Item2.Result.Item4 = result;
			data.Item2.OnResult(data.Item1);
		};

		private static Action<T5, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>>> _onResult5 = delegate(T5 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>> data)
		{
			data.Item2.Result.Item5 = result;
			data.Item2.OnResult(data.Item1);
		};

		private static Action<T6, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>>> _onResult6 = delegate(T6 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>> data)
		{
			data.Item2.Result.Item6 = result;
			data.Item2.OnResult(data.Item1);
		};

		private static Action<T7, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>>> _onResult7 = delegate(T7 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7>> data)
		{
			data.Item2.Result.Item7 = result;
			data.Item2.OnResult(data.Item1);
		};
	}

	private class MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8> : OVRTask.MultiTaskData<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8>>>
	{
		public static OVRTask<ValueTuple<T1, T2, T3, T4, T5, T6, T7, ValueTuple<T8>>> Get(OVRTask<T1> task1, OVRTask<T2> task2, OVRTask<T3> task3, OVRTask<T4> task4, OVRTask<T5> task5, OVRTask<T6> task6, OVRTask<T7> task7, OVRTask<T8> task8)
		{
			OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8> multiTaskData = OVRObjectPool.Get<OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>>();
			multiTaskData.AddTask(task1._id);
			multiTaskData.AddTask(task2._id);
			multiTaskData.AddTask(task3._id);
			multiTaskData.AddTask(task4._id);
			multiTaskData.AddTask(task5._id);
			multiTaskData.AddTask(task6._id);
			multiTaskData.AddTask(task7._id);
			multiTaskData.AddTask(task8._id);
			task1.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>>>(OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>._onResult1, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>>(task1._id, multiTaskData));
			task2.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>>>(OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>._onResult2, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>>(task2._id, multiTaskData));
			task3.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>>>(OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>._onResult3, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>>(task3._id, multiTaskData));
			task4.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>>>(OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>._onResult4, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>>(task4._id, multiTaskData));
			task5.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>>>(OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>._onResult5, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>>(task5._id, multiTaskData));
			task6.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>>>(OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>._onResult6, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>>(task6._id, multiTaskData));
			task7.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>>>(OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>._onResult7, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>>(task7._id, multiTaskData));
			task8.ContinueWith<ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>>>(OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>._onResult8, new ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>>(task8._id, multiTaskData));
			return multiTaskData.CombinedTask;
		}

		private static Action<T1, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>>> _onResult1 = delegate(T1 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>> data)
		{
			data.Item2.Result.Item1 = result;
			data.Item2.OnResult(data.Item1);
		};

		private static Action<T2, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>>> _onResult2 = delegate(T2 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>> data)
		{
			data.Item2.Result.Item2 = result;
			data.Item2.OnResult(data.Item1);
		};

		private static Action<T3, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>>> _onResult3 = delegate(T3 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>> data)
		{
			data.Item2.Result.Item3 = result;
			data.Item2.OnResult(data.Item1);
		};

		private static Action<T4, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>>> _onResult4 = delegate(T4 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>> data)
		{
			data.Item2.Result.Item4 = result;
			data.Item2.OnResult(data.Item1);
		};

		private static Action<T5, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>>> _onResult5 = delegate(T5 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>> data)
		{
			data.Item2.Result.Item5 = result;
			data.Item2.OnResult(data.Item1);
		};

		private static Action<T6, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>>> _onResult6 = delegate(T6 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>> data)
		{
			data.Item2.Result.Item6 = result;
			data.Item2.OnResult(data.Item1);
		};

		private static Action<T7, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>>> _onResult7 = delegate(T7 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>> data)
		{
			data.Item2.Result.Item7 = result;
			data.Item2.OnResult(data.Item1);
		};

		private static Action<T8, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>>> _onResult8 = delegate(T8 result, ValueTuple<Guid, OVRTask.MultiTaskData<T1, T2, T3, T4, T5, T6, T7, T8>> data)
		{
			data.Item2.Result.Rest.Item1 = result;
			data.Item2.OnResult(data.Item1);
		};
	}
}
