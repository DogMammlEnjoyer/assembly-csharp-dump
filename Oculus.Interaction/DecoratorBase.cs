using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Oculus.Interaction
{
	public abstract class DecoratorBase<InstanceT, DecorationT>
	{
		protected void CompleteAsynchronousRequests(InstanceT instance, DecorationT decoration)
		{
			TaskCompletionSource<DecorationT> taskCompletionSource;
			if (this._instanceToCompletionSource.TryGetValue(instance, out taskCompletionSource))
			{
				taskCompletionSource.SetResult(decoration);
				this._instanceToCompletionSource.Remove(instance);
			}
		}

		protected Task<DecorationT> GetAsynchronousRequest(InstanceT instance)
		{
			TaskCompletionSource<DecorationT> taskCompletionSource;
			if (!this._instanceToCompletionSource.TryGetValue(instance, out taskCompletionSource))
			{
				taskCompletionSource = new TaskCompletionSource<DecorationT>();
				this._instanceToCompletionSource.Add(instance, taskCompletionSource);
			}
			return taskCompletionSource.Task;
		}

		private readonly Dictionary<InstanceT, TaskCompletionSource<DecorationT>> _instanceToCompletionSource = new Dictionary<InstanceT, TaskCompletionSource<DecorationT>>();
	}
}
