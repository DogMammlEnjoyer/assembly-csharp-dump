using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Unity.Cinemachine
{
	[NullableContext(1)]
	[Nullable(0)]
	internal abstract class PolyPathBase : IEnumerable
	{
		public PolyPathEnum GetEnumerator()
		{
			return new PolyPathEnum(this._childs);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public bool IsHole
		{
			get
			{
				return this.GetIsHole();
			}
		}

		[NullableContext(2)]
		public PolyPathBase(PolyPathBase parent = null)
		{
			this._parent = parent;
		}

		private bool GetIsHole()
		{
			bool flag = true;
			for (PolyPathBase parent = this._parent; parent != null; parent = parent._parent)
			{
				flag = !flag;
			}
			return flag;
		}

		public int Count
		{
			get
			{
				return this._childs.Count;
			}
		}

		internal abstract PolyPathBase AddChild(List<Point64> p);

		public void Clear()
		{
			this._childs.Clear();
		}

		[Nullable(2)]
		internal PolyPathBase _parent;

		internal List<PolyPathBase> _childs = new List<PolyPathBase>();
	}
}
