using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Unity.Cinemachine
{
	[NullableContext(1)]
	[Nullable(0)]
	internal class PolyPathEnum : IEnumerator
	{
		public PolyPathEnum(List<PolyPathBase> childs)
		{
			this._ppbList = childs;
		}

		public bool MoveNext()
		{
			this.position++;
			return this.position < this._ppbList.Count;
		}

		public void Reset()
		{
			this.position = -1;
		}

		public PolyPathBase Current
		{
			get
			{
				if (this.position < 0 || this.position >= this._ppbList.Count)
				{
					throw new InvalidOperationException();
				}
				return this._ppbList[this.position];
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this.Current;
			}
		}

		public List<PolyPathBase> _ppbList;

		private int position = -1;
	}
}
