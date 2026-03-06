using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.ProBuilder
{
	public sealed class WingedEdgeEnumerator : IEnumerator<WingedEdge>, IEnumerator, IDisposable
	{
		public WingedEdgeEnumerator(WingedEdge start)
		{
			this.m_Start = start;
			this.m_Current = null;
		}

		public bool MoveNext()
		{
			if (this.m_Current == null)
			{
				this.m_Current = this.m_Start;
				return this.m_Current != null;
			}
			this.m_Current = this.m_Current.next;
			return this.m_Current != null && this.m_Current != this.m_Start;
		}

		public void Reset()
		{
			this.m_Current = null;
		}

		public WingedEdge Current
		{
			get
			{
				WingedEdge current;
				try
				{
					current = this.m_Current;
				}
				catch (IndexOutOfRangeException)
				{
					throw new InvalidOperationException();
				}
				return current;
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this.Current;
			}
		}

		public void Dispose()
		{
		}

		private WingedEdge m_Start;

		private WingedEdge m_Current;
	}
}
