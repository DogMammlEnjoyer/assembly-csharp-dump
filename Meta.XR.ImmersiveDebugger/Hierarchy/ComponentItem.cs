using System;
using Meta.XR.ImmersiveDebugger.Manager;
using Meta.XR.ImmersiveDebugger.Utils;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.Hierarchy
{
	internal class ComponentItem : Item<Component>
	{
		public override string Label
		{
			get
			{
				return base.Handle.Type.Name;
			}
		}

		public override bool Valid
		{
			get
			{
				return this._owner != null;
			}
		}

		public override Category Category
		{
			get
			{
				return new Category
				{
					Item = base.Parent
				};
			}
		}

		protected override InstanceHandle BuildHandle()
		{
			return new InstanceHandle(this._owner.GetType(), this._owner);
		}
	}
}
