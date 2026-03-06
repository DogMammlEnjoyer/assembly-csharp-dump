using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnityEngine.ResourceManagement.AsyncOperations
{
	public struct AsyncOperationHandle : IEnumerator
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

		internal AsyncOperationHandle(IAsyncOperation op)
		{
			this.m_InternalOp = op;
			this.m_Version = ((op != null) ? op.Version : 0);
			this.m_LocationName = null;
		}

		internal AsyncOperationHandle(IAsyncOperation op, int version)
		{
			this.m_InternalOp = op;
			this.m_Version = version;
			this.m_LocationName = null;
		}

		internal AsyncOperationHandle(IAsyncOperation op, string locationName)
		{
			this.m_InternalOp = op;
			this.m_Version = ((op != null) ? op.Version : 0);
			this.m_LocationName = locationName;
		}

		internal AsyncOperationHandle(IAsyncOperation op, int version, string locationName)
		{
			this.m_InternalOp = op;
			this.m_Version = version;
			this.m_LocationName = locationName;
		}

		internal AsyncOperationHandle Acquire()
		{
			this.InternalOp.IncrementReferenceCount();
			return this;
		}

		public event Action<AsyncOperationHandle> Completed
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

		public void ReleaseHandleOnCompletion()
		{
			this.Completed += delegate(AsyncOperationHandle op)
			{
				op.Release();
			};
		}

		public AsyncOperationHandle<T> Convert<T>()
		{
			return new AsyncOperationHandle<T>(this.InternalOp, this.m_Version, this.m_LocationName);
		}

		public bool Equals(AsyncOperationHandle other)
		{
			return this.m_Version == other.m_Version && this.m_InternalOp == other.m_InternalOp;
		}

		public string DebugName
		{
			get
			{
				if (!this.IsValid())
				{
					return "InvalidHandle";
				}
				return this.InternalOp.DebugName;
			}
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

		public void GetDependencies(List<AsyncOperationHandle> deps)
		{
			this.InternalOp.GetDependencies(deps);
		}

		public override int GetHashCode()
		{
			if (this.m_InternalOp != null)
			{
				return this.m_InternalOp.GetHashCode() * 17 + this.m_Version;
			}
			return 0;
		}

		private IAsyncOperation InternalOp
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

		public object Result
		{
			get
			{
				return this.InternalOp.GetResultAsObject();
			}
		}

		public AsyncOperationStatus Status
		{
			get
			{
				return this.InternalOp.Status;
			}
		}

		public Task<object> Task
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

		public object WaitForCompletion()
		{
			if (this.IsValid() && !this.InternalOp.IsDone)
			{
				this.InternalOp.WaitForCompletion();
			}
			if (this.IsValid())
			{
				return this.Result;
			}
			return null;
		}

		internal IAsyncOperation m_InternalOp;

		private int m_Version;

		private string m_LocationName;
	}
}
