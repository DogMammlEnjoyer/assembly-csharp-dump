using System;
using System.Collections.Generic;
using Unity.Jobs;

namespace UnityEngine.UIElements.UIR
{
	internal class MeshGenerationDeferrer : IDisposable
	{
		public void AddMeshGenerationJob(JobHandle jobHandle)
		{
			this.m_Dependencies.Enqueue(jobHandle);
		}

		public void AddMeshGenerationCallback(MeshGenerationCallback callback, object userData, MeshGenerationCallbackType callbackType, bool isJobDependent)
		{
			bool flag = callback == null;
			if (flag)
			{
				throw new ArgumentNullException("callback");
			}
			MeshGenerationDeferrer.CallbackInfo item = new MeshGenerationDeferrer.CallbackInfo
			{
				callback = callback,
				userData = userData
			};
			bool flag2 = !isJobDependent;
			if (flag2)
			{
				switch (callbackType)
				{
				case MeshGenerationCallbackType.Fork:
					this.m_Fork.Enqueue(item);
					break;
				case MeshGenerationCallbackType.WorkThenFork:
					this.m_WorkThenFork.Enqueue(item);
					break;
				case MeshGenerationCallbackType.Work:
					this.m_Work.Enqueue(item);
					break;
				default:
					throw new NotImplementedException();
				}
			}
			else
			{
				switch (callbackType)
				{
				case MeshGenerationCallbackType.Fork:
					this.m_JobDependentFork.Enqueue(item);
					break;
				case MeshGenerationCallbackType.WorkThenFork:
					this.m_JobDependentWorkThenFork.Enqueue(item);
					break;
				case MeshGenerationCallbackType.Work:
					this.m_JobDependentWork.Enqueue(item);
					break;
				default:
					throw new NotImplementedException();
				}
			}
		}

		public void ProcessDeferredWork(MeshGenerationContext meshGenerationContext)
		{
			for (;;)
			{
				int count = this.m_Fork.Count;
				int count2 = this.m_WorkThenFork.Count;
				int count3 = this.m_Work.Count;
				int count4 = this.m_JobDependentFork.Count;
				int count5 = this.m_JobDependentWorkThenFork.Count;
				int count6 = this.m_JobDependentWork.Count;
				int count7 = this.m_Dependencies.Count;
				bool flag = count + count2 + count3 + count7 == 0;
				if (flag)
				{
					break;
				}
				for (int i = 0; i < count; i++)
				{
					MeshGenerationDeferrer.CallbackInfo ci = this.m_Fork.Dequeue();
					MeshGenerationDeferrer.Invoke(ci, meshGenerationContext);
				}
				for (int j = 0; j < count2; j++)
				{
					MeshGenerationDeferrer.CallbackInfo ci2 = this.m_WorkThenFork.Dequeue();
					MeshGenerationDeferrer.Invoke(ci2, meshGenerationContext);
				}
				for (int k = 0; k < count3; k++)
				{
					MeshGenerationDeferrer.CallbackInfo ci3 = this.m_Work.Dequeue();
					MeshGenerationDeferrer.Invoke(ci3, meshGenerationContext);
				}
				for (int l = 0; l < count7; l++)
				{
					this.m_DependencyMerger.Add(this.m_Dependencies.Dequeue());
				}
				this.m_DependencyMerger.MergeAndReset().Complete();
				for (int m = 0; m < count4; m++)
				{
					MeshGenerationDeferrer.CallbackInfo ci4 = this.m_JobDependentFork.Dequeue();
					MeshGenerationDeferrer.Invoke(ci4, meshGenerationContext);
				}
				for (int n = 0; n < count5; n++)
				{
					MeshGenerationDeferrer.CallbackInfo ci5 = this.m_JobDependentWorkThenFork.Dequeue();
					MeshGenerationDeferrer.Invoke(ci5, meshGenerationContext);
				}
				for (int num = 0; num < count6; num++)
				{
					MeshGenerationDeferrer.CallbackInfo ci6 = this.m_JobDependentWork.Dequeue();
					MeshGenerationDeferrer.Invoke(ci6, meshGenerationContext);
				}
			}
		}

		private static void Invoke(MeshGenerationDeferrer.CallbackInfo ci, MeshGenerationContext mgc)
		{
			try
			{
				ci.callback(mgc, ci.userData);
				bool flag = mgc.visualElement != null;
				if (flag)
				{
					Debug.LogWarning(string.Format("MeshGenerationContext is assigned to a VisualElement after calling '{0}'. Did you forget to call '{1}'?", ci.callback, "End"));
					mgc.End();
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
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
					this.m_DependencyMerger.Dispose();
					this.m_DependencyMerger = null;
				}
				this.disposed = true;
			}
		}

		private Queue<MeshGenerationDeferrer.CallbackInfo> m_Fork = new Queue<MeshGenerationDeferrer.CallbackInfo>(32);

		private Queue<MeshGenerationDeferrer.CallbackInfo> m_WorkThenFork = new Queue<MeshGenerationDeferrer.CallbackInfo>(32);

		private Queue<MeshGenerationDeferrer.CallbackInfo> m_Work = new Queue<MeshGenerationDeferrer.CallbackInfo>(32);

		private Queue<MeshGenerationDeferrer.CallbackInfo> m_JobDependentFork = new Queue<MeshGenerationDeferrer.CallbackInfo>(32);

		private Queue<MeshGenerationDeferrer.CallbackInfo> m_JobDependentWorkThenFork = new Queue<MeshGenerationDeferrer.CallbackInfo>(32);

		private Queue<MeshGenerationDeferrer.CallbackInfo> m_JobDependentWork = new Queue<MeshGenerationDeferrer.CallbackInfo>(32);

		private Queue<JobHandle> m_Dependencies = new Queue<JobHandle>(32);

		private JobMerger m_DependencyMerger = new JobMerger(64);

		private struct CallbackInfo
		{
			public MeshGenerationCallback callback;

			public object userData;
		}
	}
}
