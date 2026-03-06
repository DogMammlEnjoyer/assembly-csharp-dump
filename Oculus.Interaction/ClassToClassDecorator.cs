using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Oculus.Interaction
{
	public abstract class ClassToClassDecorator<InstanceT, DecorationT> : DecoratorBase<InstanceT, DecorationT> where InstanceT : class where DecorationT : class
	{
		public void AddDecoration(InstanceT instance, DecorationT decoration)
		{
			this._instanceToDecoration.Add(instance, decoration);
			base.CompleteAsynchronousRequests(instance, decoration);
		}

		public void RemoveDecoration(InstanceT instance)
		{
			this._instanceToDecoration.Remove(instance);
		}

		public bool TryGetDecoration(InstanceT instance, out DecorationT decoration)
		{
			return this._instanceToDecoration.TryGetValue(instance, out decoration);
		}

		public Task<DecorationT> GetDecorationAsync(InstanceT instance)
		{
			DecorationT result;
			if (this._instanceToDecoration.TryGetValue(instance, out result))
			{
				return Task.FromResult<DecorationT>(result);
			}
			return base.GetAsynchronousRequest(instance);
		}

		private readonly ConditionalWeakTable<InstanceT, DecorationT> _instanceToDecoration = new ConditionalWeakTable<InstanceT, DecorationT>();
	}
}
