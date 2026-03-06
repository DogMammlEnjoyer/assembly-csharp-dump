using System;
using System.Diagnostics;

namespace UnityEngine.Rendering
{
	[DebuggerDisplay("Size = {size} Capacity = {capacity}")]
	public class DynamicString : DynamicArray<char>
	{
		public DynamicString()
		{
		}

		public DynamicString(string s) : base(s.Length, true)
		{
			for (int i = 0; i < s.Length; i++)
			{
				this.m_Array[i] = s[i];
			}
		}

		public DynamicString(int capacity) : base(capacity, false)
		{
		}

		public void Append(string s)
		{
			int size = base.size;
			base.Reserve(base.size + s.Length, true);
			for (int i = 0; i < s.Length; i++)
			{
				this.m_Array[size + i] = s[i];
			}
			base.size += s.Length;
			base.BumpVersion();
		}

		public void Append(DynamicString s)
		{
			base.AddRange(s);
		}

		public override string ToString()
		{
			return new string(this.m_Array, 0, base.size);
		}
	}
}
