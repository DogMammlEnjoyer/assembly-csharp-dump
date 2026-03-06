using System;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.Util;
using UnityEngine.Scripting;

namespace UnityEngine.ResourceManagement.AsyncOperations
{
	[Preserve]
	internal class ProviderOperation<TObject> : AsyncOperationBase<TObject>, IGenericProviderOperation, ICachable
	{
		IOperationCacheKey ICachable.Key { get; set; }

		public int ProvideHandleVersion
		{
			get
			{
				return this.m_ProvideHandleVersion;
			}
		}

		public IResourceLocation Location
		{
			get
			{
				return this.m_Location;
			}
		}

		public void SetDownloadProgressCallback(Func<DownloadStatus> callback)
		{
			this.m_GetDownloadProgressCallback = callback;
			if (this.m_GetDownloadProgressCallback != null)
			{
				this.m_DownloadStatus = this.m_GetDownloadProgressCallback();
			}
		}

		public void SetWaitForCompletionCallback(Func<bool> callback)
		{
			this.m_WaitForCompletionCallback = callback;
		}

		protected override bool InvokeWaitForCompletion()
		{
			if (base.IsDone || this.m_ProviderCompletedCalled)
			{
				return true;
			}
			if (this.m_DepOp.IsValid() && !this.m_DepOp.IsDone)
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
			return this.m_WaitForCompletionCallback != null && this.m_WaitForCompletionCallback();
		}

		internal override DownloadStatus GetDownloadStatus(HashSet<object> visited)
		{
			DownloadStatus downloadStatus = this.m_DepOp.IsValid() ? this.m_DepOp.InternalGetDownloadStatus(visited) : default(DownloadStatus);
			if (this.m_GetDownloadProgressCallback != null)
			{
				this.m_DownloadStatus = this.m_GetDownloadProgressCallback();
			}
			if (base.Status == AsyncOperationStatus.Succeeded)
			{
				this.m_DownloadStatus.DownloadedBytes = this.m_DownloadStatus.TotalBytes;
			}
			return new DownloadStatus
			{
				DownloadedBytes = this.m_DownloadStatus.DownloadedBytes + downloadStatus.DownloadedBytes,
				TotalBytes = this.m_DownloadStatus.TotalBytes + downloadStatus.TotalBytes,
				IsDone = base.IsDone
			};
		}

		public override void GetDependencies(List<AsyncOperationHandle> deps)
		{
			if (this.m_DepOp.IsValid())
			{
				deps.Add(this.m_DepOp);
			}
		}

		internal override void ReleaseDependencies()
		{
			if (this.m_DepOp.IsValid())
			{
				this.m_DepOp.Release();
			}
		}

		protected override string DebugName
		{
			get
			{
				return string.Format("Resource<{0}>({1})", typeof(TObject).Name, (this.m_Location == null) ? "Invalid" : AsyncOperationBase<TObject>.ShortenPath(this.m_Location.InternalId, true));
			}
		}

		public void GetDependencies(IList<object> dstList)
		{
			dstList.Clear();
			if (!this.m_DepOp.IsValid())
			{
				return;
			}
			if (this.m_DepOp.Result == null)
			{
				return;
			}
			for (int i = 0; i < this.m_DepOp.Result.Count; i++)
			{
				dstList.Add(this.m_DepOp.Result[i].Result);
			}
		}

		public Type RequestedType
		{
			get
			{
				return typeof(TObject);
			}
		}

		public int DependencyCount
		{
			get
			{
				if (this.m_DepOp.IsValid() && this.m_DepOp.Result != null)
				{
					return this.m_DepOp.Result.Count;
				}
				return 0;
			}
		}

		public TDepObject GetDependency<TDepObject>(int index)
		{
			if (!this.m_DepOp.IsValid() || this.m_DepOp.Result == null)
			{
				throw new Exception("Cannot get dependency because no dependencies were available");
			}
			return (TDepObject)((object)this.m_DepOp.Result[index].Result);
		}

		public void SetProgressCallback(Func<float> callback)
		{
			this.m_GetProgressCallback = callback;
		}

		public void ProviderCompleted<T>(T result, bool status, Exception e)
		{
			this.m_ProvideHandleVersion++;
			this.m_GetProgressCallback = null;
			this.m_GetDownloadProgressCallback = null;
			this.m_WaitForCompletionCallback = null;
			this.m_NeedsRelease = status;
			this.m_ProviderCompletedCalled = true;
			ProviderOperation<T> providerOperation = this as ProviderOperation<T>;
			if (providerOperation != null)
			{
				providerOperation.Result = result;
			}
			else if (result == null && !typeof(TObject).IsValueType)
			{
				base.Result = (TObject)((object)null);
			}
			else
			{
				if (result == null || !typeof(TObject).IsAssignableFrom(result.GetType()))
				{
					string text = string.Format("Provider of type {0} with id {1} has provided a result of type {2} which cannot be converted to requested type {3}. The operation will be marked as failed.", new object[]
					{
						this.m_Provider.GetType().ToString(),
						this.m_Provider.ProviderId,
						typeof(T),
						typeof(TObject)
					});
					base.Complete(base.Result, false, text);
					throw new Exception(text);
				}
				base.Result = (TObject)((object)result);
			}
			base.Complete(base.Result, status, e, this.m_ReleaseDependenciesOnFailure);
		}

		protected override float Progress
		{
			get
			{
				float result;
				try
				{
					float num = 1f;
					float num2 = 0f;
					if (this.m_GetProgressCallback != null)
					{
						num2 += this.m_GetProgressCallback();
					}
					if (!this.m_DepOp.IsValid() || this.m_DepOp.Result == null || this.m_DepOp.Result.Count == 0)
					{
						num2 += 1f;
						num += 1f;
					}
					else
					{
						foreach (AsyncOperationHandle asyncOperationHandle in this.m_DepOp.Result)
						{
							num2 += asyncOperationHandle.PercentComplete;
							num += 1f;
						}
					}
					result = Mathf.Min(num2 / num, 0.99f);
				}
				catch
				{
					result = 0f;
				}
				return result;
			}
		}

		protected override void Execute()
		{
			if (this.m_DepOp.IsValid() && this.m_DepOp.Status == AsyncOperationStatus.Failed && (this.m_Provider.BehaviourFlags & ProviderBehaviourFlags.CanProvideWithFailedDependencies) == ProviderBehaviourFlags.None)
			{
				this.ProviderCompleted<TObject>(default(TObject), false, new Exception("Dependency Exception", this.m_DepOp.OperationException));
				return;
			}
			try
			{
				this.m_Provider.Provide(new ProvideHandle(this.m_ResourceManager, this));
			}
			catch (Exception e)
			{
				this.ProviderCompleted<TObject>(default(TObject), false, e);
			}
		}

		public void Init(ResourceManager rm, IResourceProvider provider, IResourceLocation location, AsyncOperationHandle<IList<AsyncOperationHandle>> depOp)
		{
			this.m_DownloadStatus = default(DownloadStatus);
			this.m_ResourceManager = rm;
			this.m_DepOp = depOp;
			if (this.m_DepOp.IsValid())
			{
				this.m_DepOp.Acquire();
			}
			this.m_Provider = provider;
			this.m_Location = location;
			this.m_ReleaseDependenciesOnFailure = true;
			this.m_ProviderCompletedCalled = false;
			this.SetWaitForCompletionCallback(new Func<bool>(this.WaitForCompletionHandler));
		}

		public void Init(ResourceManager rm, IResourceProvider provider, IResourceLocation location, AsyncOperationHandle<IList<AsyncOperationHandle>> depOp, bool releaseDependenciesOnFailure)
		{
			this.m_DownloadStatus = default(DownloadStatus);
			this.m_ResourceManager = rm;
			this.m_DepOp = depOp;
			if (this.m_DepOp.IsValid())
			{
				this.m_DepOp.Acquire();
			}
			this.m_Provider = provider;
			this.m_Location = location;
			this.m_ReleaseDependenciesOnFailure = releaseDependenciesOnFailure;
			this.m_ProviderCompletedCalled = false;
			this.SetWaitForCompletionCallback(new Func<bool>(this.WaitForCompletionHandler));
		}

		private bool WaitForCompletionHandler()
		{
			if (base.IsDone)
			{
				return true;
			}
			if (!this.m_DepOp.IsDone)
			{
				this.m_DepOp.WaitForCompletion();
			}
			if (!this.HasExecuted)
			{
				base.InvokeExecute();
			}
			return base.IsDone;
		}

		protected override void Destroy()
		{
			if (this.m_NeedsRelease)
			{
				this.m_Provider.Release(this.m_Location, base.Result);
			}
			if (this.m_DepOp.IsValid())
			{
				this.m_DepOp.Release();
			}
			base.Result = default(TObject);
			this.m_Location = null;
		}

		private bool m_ReleaseDependenciesOnFailure = true;

		private Func<float> m_GetProgressCallback;

		private Func<DownloadStatus> m_GetDownloadProgressCallback;

		private Func<bool> m_WaitForCompletionCallback;

		private bool m_ProviderCompletedCalled;

		private DownloadStatus m_DownloadStatus;

		private IResourceProvider m_Provider;

		internal AsyncOperationHandle<IList<AsyncOperationHandle>> m_DepOp;

		private IResourceLocation m_Location;

		private int m_ProvideHandleVersion;

		private bool m_NeedsRelease;

		private ResourceManager m_ResourceManager;

		private const float k_OperationWaitingToCompletePercentComplete = 0.99f;

		internal const string kInvalidHandleMsg = "The ProvideHandle is invalid. After the handle has been completed, it can no longer be used";
	}
}
