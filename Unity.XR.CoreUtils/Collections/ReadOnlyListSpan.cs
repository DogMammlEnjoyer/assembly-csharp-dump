using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Unity.XR.CoreUtils.Collections
{
	public struct ReadOnlyListSpan<T> : IReadOnlyList<T>, IEnumerable<!0>, IEnumerable, IReadOnlyCollection<T>, IEquatable<ReadOnlyListSpan<T>>
	{
		public int Count
		{
			get
			{
				return this.m_Enumerator.end - this.m_Enumerator.start;
			}
		}

		public T this[int index]
		{
			get
			{
				index += this.m_Enumerator.start;
				if (index < this.m_Enumerator.start || index >= this.m_Enumerator.end)
				{
					throw new ArgumentOutOfRangeException();
				}
				return this.m_Enumerator.list[index];
			}
		}

		public ReadOnlyListSpan(IReadOnlyList<T> list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			this.m_Enumerator = new ReadOnlyListSpan<T>.Enumerator(list);
		}

		public ReadOnlyListSpan(IReadOnlyList<T> list, int start, int length)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (start < 0 || start + length > list.Count)
			{
				throw new ArgumentOutOfRangeException();
			}
			this.m_Enumerator = new ReadOnlyListSpan<T>.Enumerator(list, start, start + length);
		}

		public ReadOnlyListSpan<T> Slice(int start, int length)
		{
			int num = this.m_Enumerator.start + start;
			if (num < this.m_Enumerator.start || num + length > this.m_Enumerator.end)
			{
				throw new ArgumentOutOfRangeException();
			}
			return new ReadOnlyListSpan<T>(this.m_Enumerator.list, this.m_Enumerator.start + start, length);
		}

		public static ReadOnlyListSpan<T> Empty()
		{
			return ReadOnlyListSpan<T>.s_EmptyList;
		}

		public ReadOnlyListSpan<T>.Enumerator GetEnumerator()
		{
			return this.m_Enumerator;
		}

		IEnumerator<T> IEnumerable<!0>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public bool Equals(ReadOnlyListSpan<T> other)
		{
			return this.m_Enumerator.list == other.m_Enumerator.list && this.m_Enumerator.start == other.m_Enumerator.start && this.m_Enumerator.end == other.m_Enumerator.end;
		}

		public override bool Equals(object obj)
		{
			if (obj is ReadOnlyListSpan<T>)
			{
				ReadOnlyListSpan<T> other = (ReadOnlyListSpan<T>)obj;
				return this.Equals(other);
			}
			return false;
		}

		public static bool operator ==(ReadOnlyListSpan<T> lhs, ReadOnlyListSpan<T> rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(ReadOnlyListSpan<T> lhs, ReadOnlyListSpan<T> rhs)
		{
			return !(lhs == rhs);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<IReadOnlyList<T>, int, int>(this.m_Enumerator.list, this.m_Enumerator.start, this.m_Enumerator.end);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("{");
			for (int i = this.m_Enumerator.start; i < this.m_Enumerator.end; i++)
			{
				T t = this.m_Enumerator.list[i];
				StringBuilder stringBuilder2 = stringBuilder;
				string value;
				if (t != null)
				{
					string str = "  ";
					T t2 = this.m_Enumerator.list[i];
					value = str + t2.ToString() + ",";
				}
				else
				{
					value = "  null,";
				}
				stringBuilder2.AppendLine(value);
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}

		private static ReadOnlyListSpan<T> s_EmptyList;

		private ReadOnlyListSpan<T>.Enumerator m_Enumerator;

		public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
		{
			public readonly int start { get; }

			public readonly int end { get; }

			public T Current
			{
				get
				{
					if (this.m_CurrentIndex < this.start || this.m_CurrentIndex >= this.end)
					{
						throw new ArgumentOutOfRangeException();
					}
					return this.list[this.m_CurrentIndex];
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			internal Enumerator(IReadOnlyList<T> list)
			{
				this = new ReadOnlyListSpan<T>.Enumerator(list, 0, list.Count);
			}

			internal Enumerator(IReadOnlyList<T> list, int start, int end)
			{
				this.list = list;
				this.start = start;
				this.end = end;
				this.m_CurrentIndex = this.start - 1;
			}

			public bool MoveNext()
			{
				this.m_CurrentIndex++;
				return this.m_CurrentIndex < this.end;
			}

			public void Reset()
			{
				this.m_CurrentIndex = this.start - 1;
			}

			void IDisposable.Dispose()
			{
			}

			internal IReadOnlyList<T> list;

			private int m_CurrentIndex;
		}
	}
}
