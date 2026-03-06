using System;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.Localization.Operations
{
	internal abstract class WaitForCurrentOperationAsyncOperationBase<TObject> : AsyncOperationBase<TObject>
	{
		protected AsyncOperationHandle CurrentOperation { get; set; }

		public AsyncOperationHandle Dependency { get; set; }

		protected override bool InvokeWaitForCompletion()
		{
			if (!base.IsRunning || this.m_Waiting)
			{
				return true;
			}
			bool result;
			try
			{
				this.m_Waiting = true;
				if (!this.HasExecuted && this.Dependency.IsValid())
				{
					this.Dependency.WaitForCompletion();
					if (this.Dependency.IsValid() && this.Dependency.Status == AsyncOperationStatus.Failed)
					{
						base.Complete(default(TObject), false, "Dependency `" + base.Handle.DebugName + "` failed to complete.");
						result = true;
					}
					else
					{
						result = false;
					}
				}
				else if (!this.CurrentOperation.IsValid())
				{
					base.Complete(default(TObject), false, "Expected to have a current operation to wait on. Can not complete " + this.ToString() + ".");
					result = true;
				}
				else
				{
					this.CurrentOperation.WaitForCompletion();
					result = false;
				}
			}
			finally
			{
				this.m_Waiting = false;
			}
			return result;
		}

		protected override void Destroy()
		{
			base.Destroy();
			this.Dependency = default(AsyncOperationHandle);
			this.CurrentOperation = default(AsyncOperationHandle);
			this.HasExecuted = false;
		}

		private bool m_Waiting;
	}
}
