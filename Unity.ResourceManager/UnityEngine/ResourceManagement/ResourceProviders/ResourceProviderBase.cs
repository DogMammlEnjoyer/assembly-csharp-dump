using System;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.Util;

namespace UnityEngine.ResourceManagement.ResourceProviders
{
	public abstract class ResourceProviderBase : IResourceProvider, IInitializableObject
	{
		public virtual string ProviderId
		{
			get
			{
				if (string.IsNullOrEmpty(this.m_ProviderId))
				{
					this.m_ProviderId = base.GetType().FullName;
				}
				return this.m_ProviderId;
			}
		}

		public virtual bool Initialize(string id, string data)
		{
			this.m_ProviderId = id;
			return !string.IsNullOrEmpty(this.m_ProviderId);
		}

		public virtual bool CanProvide(Type t, IResourceLocation location)
		{
			return this.GetDefaultType(location).IsAssignableFrom(t);
		}

		public override string ToString()
		{
			return this.ProviderId;
		}

		public virtual void Release(IResourceLocation location, object obj)
		{
		}

		public virtual Type GetDefaultType(IResourceLocation location)
		{
			return typeof(object);
		}

		public abstract void Provide(ProvideHandle provideHandle);

		public virtual AsyncOperationHandle<bool> InitializeAsync(ResourceManager rm, string id, string data)
		{
			ResourceProviderBase.BaseInitAsyncOp baseInitAsyncOp = new ResourceProviderBase.BaseInitAsyncOp();
			baseInitAsyncOp.Init(() => this.Initialize(id, data));
			return rm.StartOperation<bool>(baseInitAsyncOp, default(AsyncOperationHandle));
		}

		ProviderBehaviourFlags IResourceProvider.BehaviourFlags
		{
			get
			{
				return this.m_BehaviourFlags;
			}
		}

		protected string m_ProviderId;

		protected ProviderBehaviourFlags m_BehaviourFlags;

		private class BaseInitAsyncOp : AsyncOperationBase<bool>
		{
			public void Init(Func<bool> callback)
			{
				this.m_CallBack = callback;
			}

			protected override bool InvokeWaitForCompletion()
			{
				ResourceManager rm = this.m_RM;
				if (rm != null)
				{
					rm.Update(Time.unscaledDeltaTime);
				}
				if (!this.HasExecuted)
				{
					base.InvokeExecute();
				}
				return true;
			}

			protected override void Execute()
			{
				if (this.m_CallBack != null)
				{
					base.Complete(this.m_CallBack(), true, "");
					return;
				}
				base.Complete(true, true, "");
			}

			private Func<bool> m_CallBack;
		}
	}
}
