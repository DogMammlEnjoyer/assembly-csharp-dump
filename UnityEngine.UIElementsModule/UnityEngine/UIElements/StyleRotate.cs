using System;

namespace UnityEngine.UIElements
{
	public struct StyleRotate : IStyleValue<Rotate>, IEquatable<StyleRotate>
	{
		public Rotate value
		{
			get
			{
				StyleKeyword keyword = this.m_Keyword;
				if (!true)
				{
				}
				Rotate result;
				switch (keyword)
				{
				case StyleKeyword.Undefined:
					result = this.m_Value;
					goto IL_4F;
				case StyleKeyword.Null:
					result = Rotate.None();
					goto IL_4F;
				case StyleKeyword.None:
					result = Rotate.None();
					goto IL_4F;
				case StyleKeyword.Initial:
					result = Rotate.Initial();
					goto IL_4F;
				}
				throw new NotImplementedException();
				IL_4F:
				if (!true)
				{
				}
				return result;
			}
			set
			{
				this.m_Value = value;
				this.m_Keyword = StyleKeyword.Undefined;
			}
		}

		public StyleKeyword keyword
		{
			get
			{
				return this.m_Keyword;
			}
			set
			{
				this.m_Keyword = value;
			}
		}

		public StyleRotate(Rotate v)
		{
			this = new StyleRotate(v, StyleKeyword.Undefined);
		}

		public StyleRotate(StyleKeyword keyword)
		{
			this = new StyleRotate(default(Rotate), keyword);
		}

		public StyleRotate(Quaternion quaternion)
		{
			this = new StyleRotate(quaternion, StyleKeyword.Undefined);
		}

		internal StyleRotate(Rotate v, StyleKeyword keyword)
		{
			this.m_Keyword = keyword;
			this.m_Value = v;
		}

		public static bool operator ==(StyleRotate lhs, StyleRotate rhs)
		{
			return lhs.m_Keyword == rhs.m_Keyword && lhs.m_Value == rhs.m_Value;
		}

		public static bool operator !=(StyleRotate lhs, StyleRotate rhs)
		{
			return !(lhs == rhs);
		}

		public static implicit operator StyleRotate(StyleKeyword keyword)
		{
			return new StyleRotate(keyword);
		}

		public static implicit operator StyleRotate(Rotate v)
		{
			return new StyleRotate(v);
		}

		public static implicit operator StyleRotate(Quaternion v)
		{
			return new Rotate(v);
		}

		public bool Equals(StyleRotate other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is StyleRotate)
			{
				StyleRotate other = (StyleRotate)obj;
				result = this.Equals(other);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override int GetHashCode()
		{
			return this.m_Value.GetHashCode() * 397 ^ (int)this.m_Keyword;
		}

		public override string ToString()
		{
			return this.DebugString<Rotate>();
		}

		private Rotate m_Value;

		private StyleKeyword m_Keyword;
	}
}
