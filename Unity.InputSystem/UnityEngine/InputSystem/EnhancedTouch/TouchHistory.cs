using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.LowLevel;

namespace UnityEngine.InputSystem.EnhancedTouch
{
	public struct TouchHistory : IReadOnlyList<Touch>, IEnumerable<Touch>, IEnumerable, IReadOnlyCollection<Touch>
	{
		internal TouchHistory(Finger finger, InputStateHistory<TouchState> history, int startIndex = -1, int count = -1)
		{
			this.m_Finger = finger;
			this.m_History = history;
			this.m_Version = history.version;
			this.m_Count = ((count >= 0) ? count : this.m_History.Count);
			this.m_StartIndex = ((startIndex >= 0) ? startIndex : (this.m_History.Count - 1));
		}

		public IEnumerator<Touch> GetEnumerator()
		{
			return new TouchHistory.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public int Count
		{
			get
			{
				return this.m_Count;
			}
		}

		public Touch this[int index]
		{
			get
			{
				this.CheckValid();
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException(string.Format("Index {0} is out of range for history with {1} entries", index, this.Count), "index");
				}
				return new Touch(this.m_Finger, this.m_History[this.m_StartIndex - index]);
			}
		}

		internal void CheckValid()
		{
			if (this.m_Finger == null || this.m_History == null)
			{
				throw new InvalidOperationException("Touch history not initialized");
			}
			if (this.m_History.version != this.m_Version)
			{
				throw new InvalidOperationException("Touch history is no longer valid; the recorded history has been changed");
			}
		}

		private readonly InputStateHistory<TouchState> m_History;

		private readonly Finger m_Finger;

		private readonly int m_Count;

		private readonly int m_StartIndex;

		private readonly uint m_Version;

		private class Enumerator : IEnumerator<Touch>, IEnumerator, IDisposable
		{
			internal Enumerator(TouchHistory owner)
			{
				this.m_Owner = owner;
				this.m_Index = -1;
			}

			public bool MoveNext()
			{
				if (this.m_Index >= this.m_Owner.Count - 1)
				{
					return false;
				}
				this.m_Index++;
				return true;
			}

			public void Reset()
			{
				this.m_Index = -1;
			}

			public Touch Current
			{
				get
				{
					return this.m_Owner[this.m_Index];
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

			private readonly TouchHistory m_Owner;

			private int m_Index;
		}
	}
}
