using System;
using Unity.Collections;
using Unity.Jobs;

namespace UnityEngine.UIElements.UIR
{
	internal class JobMerger : IDisposable
	{
		public JobMerger(int capacity)
		{
			Debug.Assert(capacity > 1);
			this.m_Jobs = new NativeArray<JobHandle>(capacity, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		}

		public void Add(JobHandle job)
		{
			bool flag = this.m_JobCount < this.m_Jobs.Length;
			if (flag)
			{
				int jobCount = this.m_JobCount;
				this.m_JobCount = jobCount + 1;
				this.m_Jobs[jobCount] = job;
			}
			else
			{
				this.m_Jobs[0] = JobHandle.CombineDependencies(this.m_Jobs);
				this.m_Jobs[1] = job;
				this.m_JobCount = 2;
			}
		}

		public JobHandle MergeAndReset()
		{
			JobHandle result = default(JobHandle);
			bool flag = this.m_JobCount > 1;
			if (flag)
			{
				result = JobHandle.CombineDependencies(this.m_Jobs.Slice(0, this.m_JobCount));
			}
			else
			{
				bool flag2 = this.m_JobCount == 1;
				if (flag2)
				{
					result = this.m_Jobs[0];
				}
			}
			this.m_JobCount = 0;
			return result;
		}

		private protected bool disposed { protected get; private set; }

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			bool disposed = this.disposed;
			if (!disposed)
			{
				if (disposing)
				{
					this.m_Jobs.Dispose();
				}
				this.disposed = true;
			}
		}

		private NativeArray<JobHandle> m_Jobs;

		private int m_JobCount;
	}
}
