using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine
{
	internal class GCHandlePool
	{
		public GCHandlePool()
		{
			this.m_handles = new GCHandle[128];
		}

		public GCHandle Alloc()
		{
			bool flag = this.m_current > 0;
			GCHandle result;
			if (flag)
			{
				GCHandle[] handles = this.m_handles;
				int num = this.m_current - 1;
				this.m_current = num;
				result = handles[num];
			}
			else
			{
				result = GCHandle.Alloc(null);
			}
			return result;
		}

		public GCHandle Alloc(object o)
		{
			bool flag = this.m_current > 0;
			GCHandle result;
			if (flag)
			{
				GCHandle[] handles = this.m_handles;
				int num = this.m_current - 1;
				this.m_current = num;
				GCHandle gchandle = handles[num];
				gchandle.Target = o;
				result = gchandle;
			}
			else
			{
				result = GCHandle.Alloc(o);
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IntPtr AllocHandleIfNotNull(object o)
		{
			bool flag = o == null;
			IntPtr result;
			if (flag)
			{
				result = IntPtr.Zero;
			}
			else
			{
				result = (IntPtr)this.Alloc(o);
			}
			return result;
		}

		public void Free(GCHandle h)
		{
			bool flag = this.m_current == this.m_handles.Length;
			if (flag)
			{
				int num = this.m_handles.Length * 2;
				GCHandle[] array = new GCHandle[num];
				Array.Copy(this.m_handles, array, this.m_handles.Length);
				this.m_handles = array;
			}
			h.Target = null;
			GCHandle[] handles = this.m_handles;
			int current = this.m_current;
			this.m_current = current + 1;
			handles[current] = h;
		}

		private GCHandle[] m_handles;

		private int m_current;
	}
}
