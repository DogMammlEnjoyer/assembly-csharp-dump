using System;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace UnityEngine.ResourceManagement.ResourceProviders
{
	public struct ProvideHandle
	{
		internal ProvideHandle(ResourceManager rm, IGenericProviderOperation op)
		{
			this.m_ResourceManager = rm;
			this.m_InternalOp = op;
			this.m_Version = op.ProvideHandleVersion;
		}

		internal bool IsValid
		{
			get
			{
				return this.m_InternalOp != null && this.m_InternalOp.ProvideHandleVersion == this.m_Version;
			}
		}

		internal IGenericProviderOperation InternalOp
		{
			get
			{
				if (this.m_InternalOp.ProvideHandleVersion != this.m_Version)
				{
					throw new Exception("The ProvideHandle is invalid. After the handle has been completed, it can no longer be used");
				}
				return this.m_InternalOp;
			}
		}

		public ResourceManager ResourceManager
		{
			get
			{
				return this.m_ResourceManager;
			}
		}

		public Type Type
		{
			get
			{
				return this.InternalOp.RequestedType;
			}
		}

		public IResourceLocation Location
		{
			get
			{
				return this.InternalOp.Location;
			}
		}

		public int DependencyCount
		{
			get
			{
				return this.InternalOp.DependencyCount;
			}
		}

		public TDepObject GetDependency<TDepObject>(int index)
		{
			return this.InternalOp.GetDependency<TDepObject>(index);
		}

		public void GetDependencies(IList<object> list)
		{
			this.InternalOp.GetDependencies(list);
		}

		public void SetProgressCallback(Func<float> callback)
		{
			this.InternalOp.SetProgressCallback(callback);
		}

		public void SetDownloadProgressCallbacks(Func<DownloadStatus> callback)
		{
			this.InternalOp.SetDownloadProgressCallback(callback);
		}

		public void SetWaitForCompletionCallback(Func<bool> callback)
		{
			this.InternalOp.SetWaitForCompletionCallback(callback);
		}

		public void Complete<T>(T result, bool status, Exception exception)
		{
			this.InternalOp.ProviderCompleted<T>(result, status, exception);
		}

		private int m_Version;

		private IGenericProviderOperation m_InternalOp;

		private ResourceManager m_ResourceManager;
	}
}
