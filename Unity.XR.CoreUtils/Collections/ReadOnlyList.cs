using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Unity.XR.CoreUtils.Collections
{
	public class ReadOnlyList<T> : IReadOnlyList<T>, IEnumerable<!0>, IEnumerable, IReadOnlyCollection<T>, IEquatable<ReadOnlyList<T>>
	{
		public int Count
		{
			get
			{
				return this.m_List.Count;
			}
		}

		public T this[int index]
		{
			get
			{
				return this.m_List[index];
			}
		}

		public ReadOnlyList(List<T> list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			this.m_List = list;
		}

		public static ReadOnlyList<T> Empty()
		{
			if (ReadOnlyList<T>.s_EmptyList == null)
			{
				ReadOnlyList<T>.s_EmptyList = new ReadOnlyList<T>(new List<T>(0));
			}
			return ReadOnlyList<T>.s_EmptyList;
		}

		public List<T>.Enumerator GetEnumerator()
		{
			return this.m_List.GetEnumerator();
		}

		IEnumerator<T> IEnumerable<!0>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public bool Equals(ReadOnlyList<T> other)
		{
			return other != null && (this == other || object.Equals(this.m_List, other.m_List));
		}

		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (obj.GetType() == base.GetType() && this.Equals((ReadOnlyList<T>)obj)));
		}

		public static bool operator ==(ReadOnlyList<T> lhs, ReadOnlyList<T> rhs)
		{
			return (lhs == null && rhs == null) || (lhs != null && lhs.Equals(rhs));
		}

		public static bool operator !=(ReadOnlyList<T> lhs, ReadOnlyList<T> rhs)
		{
			return !(lhs == rhs);
		}

		public override int GetHashCode()
		{
			if (this.m_List == null)
			{
				return 0;
			}
			return this.m_List.GetHashCode();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("{");
			foreach (T t in this.m_List)
			{
				stringBuilder.AppendLine((t == null) ? "  null," : ("  " + t.ToString() + ","));
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}

		private static ReadOnlyList<T> s_EmptyList;

		private readonly List<T> m_List;
	}
}
