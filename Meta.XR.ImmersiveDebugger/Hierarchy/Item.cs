using System;
using Meta.XR.ImmersiveDebugger.Manager;
using Meta.XR.ImmersiveDebugger.Utils;

namespace Meta.XR.ImmersiveDebugger.Hierarchy
{
	internal abstract class Item
	{
		public Item Parent
		{
			get
			{
				return this._parent;
			}
		}

		public int Depth
		{
			get
			{
				return this._depth;
			}
		}

		public InstanceHandle Handle
		{
			get
			{
				return this._handle;
			}
		}

		public int Id
		{
			get
			{
				return this.Handle.InstanceId;
			}
		}

		public virtual Category Category
		{
			get
			{
				return new Category
				{
					Item = this
				};
			}
		}

		public bool Dirty { get; set; }

		public void Clear()
		{
			this.ClearContent();
			this.ClearChildren();
			this.Unregister();
		}

		public virtual void Unregister()
		{
			DebugManagerAddon<Manager>.Instance.UnprocessItem(this);
			this._parent = null;
		}

		public virtual void Register(Item parent)
		{
			this._parent = parent;
			Item parent2 = this._parent;
			this._depth = ((parent2 != null) ? parent2.Depth : -1) + 1;
			DebugManagerAddon<Manager>.Instance.ProcessItem(this);
		}

		public abstract object Owner { get; }

		public abstract string Label { get; }

		public virtual int ComputeNumberOfChildren()
		{
			return 0;
		}

		public abstract bool Valid { get; }

		public virtual bool ComputeNeedsRefresh()
		{
			return false;
		}

		public virtual void BuildContent()
		{
		}

		public virtual void ClearContent()
		{
		}

		public virtual void BuildChildren()
		{
		}

		public virtual void ClearChildren()
		{
		}

		private Item _parent;

		private int _depth;

		protected InstanceHandle _handle;
	}
}
