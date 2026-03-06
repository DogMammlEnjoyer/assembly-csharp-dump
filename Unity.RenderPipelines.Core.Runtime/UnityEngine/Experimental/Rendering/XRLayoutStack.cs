using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace UnityEngine.Experimental.Rendering
{
	internal class XRLayoutStack : IDisposable
	{
		public XRLayout New()
		{
			XRLayout xrlayout;
			GenericPool<XRLayout>.Get(out xrlayout);
			this.m_Stack.Push(xrlayout);
			return xrlayout;
		}

		public XRLayout top
		{
			get
			{
				return this.m_Stack.Peek();
			}
		}

		public void Release()
		{
			XRLayout xrlayout;
			if (!this.m_Stack.TryPop(out xrlayout))
			{
				throw new InvalidOperationException("Calling Release without calling New first.");
			}
			xrlayout.Clear();
			GenericPool<XRLayout>.Release(xrlayout);
		}

		public void Dispose()
		{
			if (this.m_Stack.Count != 0)
			{
				throw new Exception("Stack is not empty. Did you skip a call to Release?");
			}
		}

		private readonly Stack<XRLayout> m_Stack = new Stack<XRLayout>();
	}
}
