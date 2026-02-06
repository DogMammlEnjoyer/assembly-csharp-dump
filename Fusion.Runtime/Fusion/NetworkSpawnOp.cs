using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Fusion
{
	public readonly struct NetworkSpawnOp
	{
		internal NetworkSpawnOp(NetworkRunner runner, NetworkSpawnStatus status, NetworkObject data)
		{
			this.Runner = runner;
			this._status = status;
			this._data = data;
		}

		internal NetworkSpawnOp(NetworkRunner runner, NetworkSpawnStatus status, NetworkSpawnOp.AsyncOpData data)
		{
			this.Runner = runner;
			this._status = status;
			this._data = data;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal NetworkObject ConsumeSyncSpawn(NetworkObjectTypeId typeId)
		{
			bool flag = this._status == NetworkSpawnStatus.Queued || this._status == NetworkSpawnStatus.Spawned;
			if (flag)
			{
				return (NetworkObject)this._data;
			}
			throw new NetworkObjectSpawnException(this._status, new NetworkObjectTypeId?(typeId));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal NetworkSpawnStatus ConsumeSyncSpawn(out NetworkObject obj)
		{
			obj = (NetworkObject)this._data;
			return this._status;
		}

		public NetworkObject Object
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				NetworkSpawnOp.AsyncOpData asyncOpData;
				bool flag;
				if (this._status == NetworkSpawnStatus.Queued)
				{
					asyncOpData = (this._data as NetworkSpawnOp.AsyncOpData);
					flag = (asyncOpData != null);
				}
				else
				{
					flag = false;
				}
				bool flag2 = flag;
				NetworkObject result;
				if (flag2)
				{
					result = asyncOpData.Object;
				}
				else
				{
					result = (NetworkObject)this._data;
				}
				return result;
			}
		}

		public NetworkSpawnStatus Status
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				NetworkSpawnOp.AsyncOpData asyncOpData;
				bool flag;
				if (this._status == NetworkSpawnStatus.Queued)
				{
					asyncOpData = (this._data as NetworkSpawnOp.AsyncOpData);
					flag = (asyncOpData != null);
				}
				else
				{
					flag = false;
				}
				bool flag2 = flag;
				NetworkSpawnStatus status;
				if (flag2)
				{
					status = asyncOpData.Status;
				}
				else
				{
					status = this._status;
				}
				return status;
			}
		}

		public bool IsSpawned
		{
			get
			{
				return this.Status == NetworkSpawnStatus.Spawned;
			}
		}

		public bool IsQueued
		{
			get
			{
				return this.Status == NetworkSpawnStatus.Queued;
			}
		}

		public bool IsFailed
		{
			get
			{
				return this.Status != NetworkSpawnStatus.Spawned && this.Status > NetworkSpawnStatus.Queued;
			}
		}

		public NetworkSpawnOp.Awaiter GetAwaiter()
		{
			return new NetworkSpawnOp.Awaiter(ref this);
		}

		public readonly NetworkRunner Runner;

		internal readonly NetworkSpawnStatus _status;

		internal readonly object _data;

		internal class AsyncOpData
		{
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			public event Action Completed;

			public void Complete(in NetworkSpawnOp op)
			{
				Assert.Check(this.Status == NetworkSpawnStatus.Queued);
				Assert.Check(op.Status > NetworkSpawnStatus.Queued);
				this.Status = op.Status;
				this.Object = op.Object;
				Action completed = this.Completed;
				if (completed != null)
				{
					completed();
				}
			}

			public NetworkSpawnStatus Status;

			public NetworkObject Object;
		}

		public struct Awaiter : INotifyCompletion
		{
			public Awaiter(in NetworkSpawnOp op)
			{
				this._op = op;
			}

			public bool IsCompleted
			{
				get
				{
					return this._op.Status > NetworkSpawnStatus.Queued;
				}
			}

			public NetworkObject GetResult()
			{
				bool flag = !this.IsCompleted;
				if (flag)
				{
					SpinWait spinWait = default(SpinWait);
					while (!this.IsCompleted)
					{
						spinWait.SpinOnce();
					}
				}
				NetworkSpawnStatus status = this._op.Status;
				Assert.Check(status > NetworkSpawnStatus.Queued);
				bool flag2 = status != NetworkSpawnStatus.Spawned;
				if (flag2)
				{
					throw new NetworkObjectSpawnException(status, null);
				}
				return this._op.Object;
			}

			public void OnCompleted(Action continuation)
			{
				bool isCompleted = this.IsCompleted;
				if (isCompleted)
				{
					continuation();
				}
				else
				{
					NetworkSpawnOp.AsyncOpData asyncOpData = this._op._data as NetworkSpawnOp.AsyncOpData;
					bool flag = asyncOpData != null;
					if (!flag)
					{
						throw new NotSupportedException();
					}
					SynchronizationContext capturedContext = SynchronizationContext.Current;
					SendOrPostCallback <>9__1;
					asyncOpData.Completed += delegate()
					{
						SynchronizationContext capturedContext;
						bool flag2 = capturedContext != null;
						if (flag2)
						{
							capturedContext = capturedContext;
							SendOrPostCallback d;
							if ((d = <>9__1) == null)
							{
								d = (<>9__1 = delegate(object _)
								{
									continuation();
								});
							}
							capturedContext.Post(d, null);
						}
						else
						{
							continuation();
						}
					};
				}
			}

			private NetworkSpawnOp _op;
		}
	}
}
