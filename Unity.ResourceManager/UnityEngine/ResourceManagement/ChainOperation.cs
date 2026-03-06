using System;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.Exceptions;

namespace UnityEngine.ResourceManagement
{
	internal class ChainOperation<TObject, TObjectDependency> : AsyncOperationBase<TObject>
	{
		public ChainOperation()
		{
			this.m_CachedOnWrappedCompleted = new Action<AsyncOperationHandle<TObject>>(this.OnWrappedCompleted);
		}

		protected override string DebugName
		{
			get
			{
				return string.Concat(new string[]
				{
					"ChainOperation<",
					typeof(TObject).Name,
					",",
					typeof(TObjectDependency).Name,
					"> - ",
					this.m_DepOp.DebugName
				});
			}
		}

		public override void GetDependencies(List<AsyncOperationHandle> deps)
		{
			if (this.m_DepOp.IsValid())
			{
				deps.Add(this.m_DepOp);
			}
		}

		public void Init(AsyncOperationHandle<TObjectDependency> dependentOp, Func<AsyncOperationHandle<TObjectDependency>, AsyncOperationHandle<TObject>> callback, bool releaseDependenciesOnFailure)
		{
			this.m_DepOp = dependentOp;
			this.m_DepOp.Acquire();
			this.m_Callback = callback;
			this.m_ReleaseDependenciesOnFailure = releaseDependenciesOnFailure;
			this.RefreshDownloadStatus(null);
		}

		protected override bool InvokeWaitForCompletion()
		{
			if (base.IsDone)
			{
				return true;
			}
			if (!this.m_DepOp.IsDone)
			{
				this.m_DepOp.WaitForCompletion();
			}
			ResourceManager rm = this.m_RM;
			if (rm != null)
			{
				rm.Update(Time.unscaledDeltaTime);
			}
			if (!this.HasExecuted)
			{
				base.InvokeExecute();
			}
			if (!this.m_WrappedOp.IsValid())
			{
				return this.m_WrappedOp.IsDone;
			}
			this.m_WrappedOp.WaitForCompletion();
			return this.m_WrappedOp.IsDone;
		}

		protected override void Execute()
		{
			this.m_WrappedOp = this.m_Callback(this.m_DepOp);
			this.m_WrappedOp.Completed += this.m_CachedOnWrappedCompleted;
			this.m_Callback = null;
		}

		private void OnWrappedCompleted(AsyncOperationHandle<TObject> x)
		{
			OperationException exception = null;
			if (x.Status == AsyncOperationStatus.Failed)
			{
				exception = new OperationException("ChainOperation failed because dependent operation failed", x.OperationException);
			}
			base.Complete(this.m_WrappedOp.Result, x.Status == AsyncOperationStatus.Succeeded, exception, this.m_ReleaseDependenciesOnFailure);
		}

		protected override void Destroy()
		{
			if (this.m_WrappedOp.IsValid())
			{
				this.m_WrappedOp.Release();
			}
			if (this.m_DepOp.IsValid())
			{
				this.m_DepOp.Release();
			}
		}

		internal override void ReleaseDependencies()
		{
			if (this.m_DepOp.IsValid())
			{
				this.m_DepOp.Release();
			}
		}

		internal override DownloadStatus GetDownloadStatus(HashSet<object> visited)
		{
			this.RefreshDownloadStatus(visited);
			return new DownloadStatus
			{
				DownloadedBytes = this.m_depStatus.DownloadedBytes + this.m_wrapStatus.DownloadedBytes,
				TotalBytes = this.m_depStatus.TotalBytes + this.m_wrapStatus.TotalBytes,
				IsDone = base.IsDone
			};
		}

		private void RefreshDownloadStatus(HashSet<object> visited = null)
		{
			this.m_depStatus = (this.m_DepOp.IsValid() ? this.m_DepOp.InternalGetDownloadStatus(visited) : this.m_depStatus);
			this.m_wrapStatus = (this.m_WrappedOp.IsValid() ? this.m_WrappedOp.InternalGetDownloadStatus(visited) : this.m_wrapStatus);
		}

		protected override float Progress
		{
			get
			{
				DownloadStatus downloadStatus = this.GetDownloadStatus(new HashSet<object>());
				if (!downloadStatus.IsDone && downloadStatus.DownloadedBytes == 0L)
				{
					return 0f;
				}
				float num = 0f;
				int num2 = 2;
				if (this.m_DepOp.IsValid())
				{
					num += this.m_DepOp.PercentComplete;
				}
				else
				{
					num += 1f;
				}
				if (this.m_WrappedOp.IsValid())
				{
					num += this.m_WrappedOp.PercentComplete;
				}
				else
				{
					num += 1f;
				}
				return num / (float)num2;
			}
		}

		private AsyncOperationHandle<TObjectDependency> m_DepOp;

		private AsyncOperationHandle<TObject> m_WrappedOp;

		private DownloadStatus m_depStatus;

		private DownloadStatus m_wrapStatus;

		private Func<AsyncOperationHandle<TObjectDependency>, AsyncOperationHandle<TObject>> m_Callback;

		private Action<AsyncOperationHandle<TObject>> m_CachedOnWrappedCompleted;

		private bool m_ReleaseDependenciesOnFailure = true;
	}
}
