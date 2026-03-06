using System;
using Meta.XR.ImmersiveDebugger.Utils;

namespace Meta.XR.ImmersiveDebugger.Hierarchy
{
	internal abstract class Item<T> : Item
	{
		public override object Owner
		{
			get
			{
				return this._owner;
			}
		}

		public T TypedOwner
		{
			get
			{
				return this._owner;
			}
		}

		public void SetOwner(T owner)
		{
			this._owner = owner;
			this._handle = this.BuildHandle();
		}

		protected abstract InstanceHandle BuildHandle();

		protected T _owner;
	}
}
