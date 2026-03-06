using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnityEngine.ResourceManagement.AsyncOperations
{
	public struct AsyncOperationHandle<TObject> : IEnumerator, IEquatable<AsyncOperationHandle<TObject>>
	{
		internal int Version
		{
			get
			{
				return this.m_Version;
			}
		}

		internal string LocationName
		{
			get
			{
				return this.m_LocationName;
			}
			set
			{
				this.m_LocationName = value;
			}
		}

		public static implicit operator AsyncOperationHandle(AsyncOperationHandle<TObject> obj)
		{
			return new AsyncOperationHandle(obj.m_InternalOp, obj.m_Version, obj.m_LocationName);
		}

		internal AsyncOperationHandle(AsyncOperationBase<TObject> op)
		{
			this.m_InternalOp = op;
			this.m_Version = ((op != null) ? op.Version : 0);
			this.m_LocationName = null;
		}

		public DownloadStatus GetDownloadStatus()
		{
			return this.InternalGetDownloadStatus(new HashSet<object>());
		}

		internal DownloadStatus InternalGetDownloadStatus(HashSet<object> visited)
		{
			if (visited == null)
			{
				visited = new HashSet<object>();
			}
			if (!visited.Add(this.InternalOp))
			{
				return new DownloadStatus
				{
					IsDone = this.IsDone
				};
			}
			return this.InternalOp.GetDownloadStatus(visited);
		}

		internal AsyncOperationHandle(IAsyncOperation op)
		{
			this.m_InternalOp = (AsyncOperationBase<TObject>)op;
			this.m_Version = ((op != null) ? op.Version : 0);
			this.m_LocationName = null;
		}

		internal AsyncOperationHandle(IAsyncOperation op, int version)
		{
			this.m_InternalOp = (AsyncOperationBase<TObject>)op;
			this.m_Version = version;
			this.m_LocationName = null;
		}

		internal AsyncOperationHandle(IAsyncOperation op, string locationName)
		{
			this.m_InternalOp = (AsyncOperationBase<TObject>)op;
			this.m_Version = ((op != null) ? op.Version : 0);
			this.m_LocationName = locationName;
		}

		internal AsyncOperationHandle(IAsyncOperation op, int version, string locationName)
		{
			this.m_InternalOp = (AsyncOperationBase<TObject>)op;
			this.m_Version = version;
			this.m_LocationName = locationName;
		}

		internal AsyncOperationHandle<TObject> Acquire()
		{
			this.InternalOp.IncrementReferenceCount();
			return this;
		}

		public event Action<AsyncOperationHandle<TObject>> Completed
		{
			add
			{
				this.InternalOp.Completed += value;
			}
			remove
			{
				this.InternalOp.Completed -= value;
			}
		}

		public void ReleaseHandleOnCompletion()
		{
			this.Completed += delegate(AsyncOperationHandle<TObject> op)
			{
				op.Release();
			};
		}

		public event Action<AsyncOperationHandle> CompletedTypeless
		{
			add
			{
				this.InternalOp.CompletedTypeless += value;
			}
			remove
			{
				this.InternalOp.CompletedTypeless -= value;
			}
		}

		public string DebugName
		{
			get
			{
				if (!this.IsValid())
				{
					return "InvalidHandle";
				}
				return ((IAsyncOperation)this.InternalOp).DebugName;
			}
		}

		public void GetDependencies(List<AsyncOperationHandle> deps)
		{
			this.InternalOp.GetDependencies(deps);
		}

		public event Action<AsyncOperationHandle> Destroyed
		{
			add
			{
				this.InternalOp.Destroyed += value;
			}
			remove
			{
				this.InternalOp.Destroyed -= value;
			}
		}

		public bool Equals(AsyncOperationHandle<TObject> other)
		{
			return this.m_Version == other.m_Version && this.m_InternalOp == other.m_InternalOp;
		}

		public override int GetHashCode()
		{
			if (this.m_InternalOp != null)
			{
				return this.m_InternalOp.GetHashCode() * 17 + this.m_Version;
			}
			return 0;
		}

		public TObject WaitForCompletion()
		{
			if (this.IsValid() && !this.InternalOp.IsDone)
			{
				this.InternalOp.WaitForCompletion();
			}
			AsyncOperationBase<TObject> internalOp = this.m_InternalOp;
			if (internalOp != null)
			{
				ResourceManager rm = internalOp.m_RM;
				if (rm != null)
				{
					rm.Update(Time.unscaledDeltaTime);
				}
			}
			if (this.IsValid())
			{
				return this.Result;
			}
			return default(TObject);
		}

		internal AsyncOperationBase<TObject> InternalOp
		{
			get
			{
				if (this.m_InternalOp == null || this.m_InternalOp.Version != this.m_Version)
				{
					throw new Exception("Attempting to use an invalid operation handle");
				}
				return this.m_InternalOp;
			}
		}

		public bool IsDone
		{
			get
			{
				return !this.IsValid() || this.InternalOp.IsDone;
			}
		}

		public bool IsValid()
		{
			return this.m_InternalOp != null && this.m_InternalOp.Version == this.m_Version;
		}

		public Exception OperationException
		{
			get
			{
				return this.InternalOp.OperationException;
			}
		}

		public float PercentComplete
		{
			get
			{
				return this.InternalOp.PercentComplete;
			}
		}

		internal int ReferenceCount
		{
			get
			{
				return this.InternalOp.ReferenceCount;
			}
		}

		public void Release()
		{
			this.InternalOp.DecrementReferenceCount();
			this.m_InternalOp = null;
		}

		public TObject Result
		{
			get
			{
				return this.InternalOp.Result;
			}
		}

		public AsyncOperationStatus Status
		{
			get
			{
				return this.InternalOp.Status;
			}
		}

		public Task<TObject> Task
		{
			get
			{
				return this.InternalOp.Task;
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this.Result;
			}
		}

		bool IEnumerator.MoveNext()
		{
			return !this.IsDone;
		}

		void IEnumerator.Reset()
		{
		}

		internal AsyncOperationBase<TObject> m_InternalOp;

		private int m_Version;

		private string m_LocationName;
	}
}
