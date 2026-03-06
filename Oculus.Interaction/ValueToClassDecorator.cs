using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Oculus.Interaction
{
	public abstract class ValueToClassDecorator<InstanceT, DecorationT> : DecoratorBase<InstanceT, DecorationT> where InstanceT : struct where DecorationT : class
	{
		public void AddDecoration(InstanceT instance, DecorationT decoration)
		{
			if (this._instanceToDecoration.ContainsKey(instance))
			{
				this.RemoveDecoration(instance);
			}
			this._instanceToDecoration.Add(instance, new WeakReference<DecorationT>(decoration));
			this._cleanupActions.Add(decoration, new FinalAction(delegate()
			{
				WeakReference<DecorationT> weakReference;
				this._instanceToDecoration.Remove(instance, out weakReference);
			}));
			base.CompleteAsynchronousRequests(instance, decoration);
		}

		public void RemoveDecoration(InstanceT instance)
		{
			WeakReference<DecorationT> weakReference;
			if (this._instanceToDecoration.TryGetValue(instance, out weakReference))
			{
				DecorationT key;
				if (weakReference.TryGetTarget(out key))
				{
					FinalAction finalAction;
					this._cleanupActions.TryGetValue(key, out finalAction);
					finalAction.Cancel();
					this._cleanupActions.Remove(key);
				}
				this._instanceToDecoration.Remove(instance);
			}
		}

		public bool TryGetDecoration(InstanceT instance, out DecorationT decoration)
		{
			WeakReference<DecorationT> weakReference;
			if (this._instanceToDecoration.TryGetValue(instance, out weakReference))
			{
				return weakReference.TryGetTarget(out decoration);
			}
			decoration = default(DecorationT);
			return false;
		}

		public Task<DecorationT> GetDecorationAsync(InstanceT instance)
		{
			WeakReference<DecorationT> weakReference;
			DecorationT result;
			if (this._instanceToDecoration.TryGetValue(instance, out weakReference) && weakReference.TryGetTarget(out result))
			{
				return Task.FromResult<DecorationT>(result);
			}
			return base.GetAsynchronousRequest(instance);
		}

		private readonly Dictionary<InstanceT, WeakReference<DecorationT>> _instanceToDecoration = new Dictionary<InstanceT, WeakReference<DecorationT>>();

		private readonly ConditionalWeakTable<DecorationT, FinalAction> _cleanupActions = new ConditionalWeakTable<DecorationT, FinalAction>();
	}
}
