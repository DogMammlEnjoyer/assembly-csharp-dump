using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Oculus.Interaction
{
	public abstract class ValueToValueDecorator<InstanceT, DecorationT> : DecoratorBase<InstanceT, DecorationT> where InstanceT : struct where DecorationT : struct
	{
		public void AddDecoration(InstanceT instance, DecorationT decoration)
		{
			if (this._instanceToDecoration.ContainsKey(instance))
			{
				this.RemoveDecoration(instance);
			}
			this._instanceToDecoration.Add(instance, decoration);
			base.CompleteAsynchronousRequests(instance, decoration);
		}

		public void RemoveDecoration(InstanceT instance)
		{
			DecorationT decorationT;
			if (this._instanceToDecoration.TryGetValue(instance, out decorationT))
			{
				this._instanceToDecoration.Remove(instance);
			}
		}

		public bool TryGetDecoration(InstanceT instance, out DecorationT decoration)
		{
			if (this._instanceToDecoration.TryGetValue(instance, out decoration))
			{
				return true;
			}
			decoration = default(DecorationT);
			return false;
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

		private readonly Dictionary<InstanceT, DecorationT> _instanceToDecoration = new Dictionary<InstanceT, DecorationT>();
	}
}
