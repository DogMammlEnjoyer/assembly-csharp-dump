using System;
using System.Globalization;

namespace UnityEngine.InputSystem.Utilities
{
	public struct InternedString : IEquatable<InternedString>, IComparable<InternedString>
	{
		public int length
		{
			get
			{
				string stringLowerCase = this.m_StringLowerCase;
				if (stringLowerCase == null)
				{
					return 0;
				}
				return stringLowerCase.Length;
			}
		}

		public InternedString(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				this.m_StringOriginalCase = null;
				this.m_StringLowerCase = null;
				return;
			}
			this.m_StringOriginalCase = string.Intern(text);
			this.m_StringLowerCase = string.Intern(text.ToLower(CultureInfo.InvariantCulture));
		}

		public bool IsEmpty()
		{
			return this.m_StringLowerCase == null;
		}

		public string ToLower()
		{
			return this.m_StringLowerCase;
		}

		public override bool Equals(object obj)
		{
			if (obj is InternedString)
			{
				InternedString other = (InternedString)obj;
				return this.Equals(other);
			}
			string text = obj as string;
			if (text == null)
			{
				return false;
			}
			if (this.m_StringLowerCase == null)
			{
				return string.IsNullOrEmpty(text);
			}
			return string.Equals(this.m_StringLowerCase, text.ToLower(CultureInfo.InvariantCulture));
		}

		public bool Equals(InternedString other)
		{
			return this.m_StringLowerCase == other.m_StringLowerCase;
		}

		public int CompareTo(InternedString other)
		{
			return string.Compare(this.m_StringLowerCase, other.m_StringLowerCase, StringComparison.InvariantCultureIgnoreCase);
		}

		public override int GetHashCode()
		{
			if (this.m_StringLowerCase == null)
			{
				return 0;
			}
			return this.m_StringLowerCase.GetHashCode();
		}

		public override string ToString()
		{
			return this.m_StringOriginalCase ?? string.Empty;
		}

		public static bool operator ==(InternedString a, InternedString b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(InternedString a, InternedString b)
		{
			return !a.Equals(b);
		}

		public static bool operator ==(InternedString a, string b)
		{
			return string.Compare(a.m_StringLowerCase, b, StringComparison.InvariantCultureIgnoreCase) == 0;
		}

		public static bool operator !=(InternedString a, string b)
		{
			return string.Compare(a.m_StringLowerCase, b, StringComparison.InvariantCultureIgnoreCase) != 0;
		}

		public static bool operator ==(string a, InternedString b)
		{
			return string.Compare(a, b.m_StringLowerCase, StringComparison.InvariantCultureIgnoreCase) == 0;
		}

		public static bool operator !=(string a, InternedString b)
		{
			return string.Compare(a, b.m_StringLowerCase, StringComparison.InvariantCultureIgnoreCase) != 0;
		}

		public static bool operator <(InternedString left, InternedString right)
		{
			return string.Compare(left.m_StringLowerCase, right.m_StringLowerCase, StringComparison.InvariantCultureIgnoreCase) < 0;
		}

		public static bool operator >(InternedString left, InternedString right)
		{
			return string.Compare(left.m_StringLowerCase, right.m_StringLowerCase, StringComparison.InvariantCultureIgnoreCase) > 0;
		}

		public static implicit operator string(InternedString str)
		{
			return str.ToString();
		}

		private readonly string m_StringOriginalCase;

		private readonly string m_StringLowerCase;
	}
}
