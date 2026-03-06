using System;

namespace UnityEngine.InputSystem.Utilities
{
	internal struct Substring : IComparable<Substring>, IEquatable<Substring>
	{
		public bool isEmpty
		{
			get
			{
				return this.m_Length == 0;
			}
		}

		public Substring(string str)
		{
			this.m_String = str;
			this.m_Index = 0;
			if (str != null)
			{
				this.m_Length = str.Length;
				return;
			}
			this.m_Length = 0;
		}

		public Substring(string str, int index, int length)
		{
			this.m_String = str;
			this.m_Index = index;
			this.m_Length = length;
		}

		public Substring(string str, int index)
		{
			this.m_String = str;
			this.m_Index = index;
			this.m_Length = str.Length - index;
		}

		public override bool Equals(object obj)
		{
			if (obj is Substring)
			{
				Substring other = (Substring)obj;
				return this.Equals(other);
			}
			string text = obj as string;
			return text != null && this.Equals(text);
		}

		public bool Equals(string other)
		{
			if (string.IsNullOrEmpty(other))
			{
				return this.m_Length == 0;
			}
			if (other.Length != this.m_Length)
			{
				return false;
			}
			for (int i = 0; i < this.m_Length; i++)
			{
				if (other[i] != this.m_String[this.m_Index + i])
				{
					return false;
				}
			}
			return true;
		}

		public bool Equals(Substring other)
		{
			return this.CompareTo(other) == 0;
		}

		public bool Equals(InternedString other)
		{
			return this.length == other.length && string.Compare(this.m_String, this.m_Index, other.ToString(), 0, this.length, StringComparison.OrdinalIgnoreCase) == 0;
		}

		public int CompareTo(Substring other)
		{
			return Substring.Compare(this, other, StringComparison.CurrentCulture);
		}

		public static int Compare(Substring left, Substring right, StringComparison comparison)
		{
			if (left.m_Length == right.m_Length)
			{
				return string.Compare(left.m_String, left.m_Index, right.m_String, right.m_Index, left.m_Length, comparison);
			}
			if (left.m_Length < right.m_Length)
			{
				return -1;
			}
			return 1;
		}

		public bool StartsWith(string str)
		{
			if (str.Length > this.length)
			{
				return false;
			}
			for (int i = 0; i < str.Length; i++)
			{
				if (this.m_String[this.m_Index + i] != str[i])
				{
					return false;
				}
			}
			return true;
		}

		public string Substr(int index = 0, int length = -1)
		{
			if (length < 0)
			{
				length = this.length - index;
			}
			return this.m_String.Substring(this.m_Index + index, length);
		}

		public override string ToString()
		{
			if (this.m_String == null)
			{
				return string.Empty;
			}
			return this.m_String.Substring(this.m_Index, this.m_Length);
		}

		public override int GetHashCode()
		{
			if (this.m_String == null)
			{
				return 0;
			}
			if (this.m_Index == 0 && this.m_Length == this.m_String.Length)
			{
				return this.m_String.GetHashCode();
			}
			return this.ToString().GetHashCode();
		}

		public static bool operator ==(Substring a, Substring b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Substring a, Substring b)
		{
			return !a.Equals(b);
		}

		public static bool operator ==(Substring a, InternedString b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Substring a, InternedString b)
		{
			return !a.Equals(b);
		}

		public static bool operator ==(InternedString a, Substring b)
		{
			return b.Equals(a);
		}

		public static bool operator !=(InternedString a, Substring b)
		{
			return !b.Equals(a);
		}

		public static implicit operator Substring(string s)
		{
			return new Substring(s);
		}

		public int length
		{
			get
			{
				return this.m_Length;
			}
		}

		public int index
		{
			get
			{
				return this.m_Index;
			}
		}

		public char this[int index]
		{
			get
			{
				if (index < 0 || index >= this.m_Length)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return this.m_String[this.m_Index + index];
			}
		}

		private readonly string m_String;

		private readonly int m_Index;

		private readonly int m_Length;
	}
}
