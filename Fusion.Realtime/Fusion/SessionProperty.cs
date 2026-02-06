using System;

namespace Fusion
{
	public sealed class SessionProperty
	{
		public object PropertyValue
		{
			get
			{
				return this._value;
			}
		}

		public Type PropertyType
		{
			get
			{
				return this._value.GetType();
			}
		}

		public bool IsInt
		{
			get
			{
				return this._value is int;
			}
		}

		public bool IsString
		{
			get
			{
				return this._value is string;
			}
		}

		public bool Isbool
		{
			get
			{
				return this._value is bool;
			}
		}

		private SessionProperty()
		{
		}

		public static implicit operator int(SessionProperty sessionProperty)
		{
			object value = sessionProperty._value;
			int result;
			bool flag;
			if (value is int)
			{
				result = (int)value;
				flag = true;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				return result;
			}
			throw new InvalidCastException();
		}

		public static implicit operator SessionProperty(int v)
		{
			return new SessionProperty
			{
				_value = v
			};
		}

		public static implicit operator string(SessionProperty sessionProperty)
		{
			string text = sessionProperty._value as string;
			bool flag = text != null;
			if (flag)
			{
				return text;
			}
			throw new InvalidCastException();
		}

		public static implicit operator SessionProperty(string v)
		{
			return new SessionProperty
			{
				_value = v
			};
		}

		public static implicit operator bool(SessionProperty sessionProperty)
		{
			object value = sessionProperty._value;
			bool result;
			bool flag;
			if (value is bool)
			{
				result = (bool)value;
				flag = true;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				return result;
			}
			throw new InvalidCastException();
		}

		public static implicit operator SessionProperty(bool v)
		{
			return new SessionProperty
			{
				_value = v
			};
		}

		public static bool Support(object obj)
		{
			return obj is int || obj is string || obj is bool;
		}

		public static SessionProperty Convert(object obj)
		{
			int v;
			bool flag;
			if (obj is int)
			{
				v = (int)obj;
				flag = true;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			SessionProperty result;
			if (flag2)
			{
				result = v;
			}
			else
			{
				string text = obj as string;
				bool flag3 = text != null;
				if (flag3)
				{
					result = text;
				}
				else
				{
					bool v2;
					bool flag4;
					if (obj is bool)
					{
						v2 = (bool)obj;
						flag4 = true;
					}
					else
					{
						flag4 = false;
					}
					bool flag5 = flag4;
					if (!flag5)
					{
						throw new ArgumentException("Invalid Object type, not supported");
					}
					result = v2;
				}
			}
			return result;
		}

		public override string ToString()
		{
			return string.Format("[{0}: {1}, Type={2}]", "SessionProperty", this._value, this.PropertyType);
		}

		private object _value = null;
	}
}
