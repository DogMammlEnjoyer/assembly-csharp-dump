using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace UnityEngine.InputSystem.Utilities
{
	internal struct JsonParser
	{
		public JsonParser(string json)
		{
			this = default(JsonParser);
			if (json == null)
			{
				throw new ArgumentNullException("json");
			}
			this.m_Text = json;
			this.m_Length = json.Length;
		}

		public void Reset()
		{
			this.m_Position = 0;
			this.m_MatchAnyElementInArray = false;
			this.m_DryRun = false;
		}

		public override string ToString()
		{
			if (this.m_Text != null)
			{
				return string.Format("{0}: {1}", this.m_Position, this.m_Text.Substring(this.m_Position));
			}
			return base.ToString();
		}

		public bool NavigateToProperty(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			int length = path.Length;
			int i = 0;
			this.m_DryRun = true;
			if (!this.ParseToken('{'))
			{
				return false;
			}
			while (this.m_Position < this.m_Length && i < length)
			{
				this.SkipWhitespace();
				if (this.m_Position == this.m_Length)
				{
					return false;
				}
				if (this.m_Text[this.m_Position] != '"')
				{
					return false;
				}
				this.m_Position++;
				int num = i;
				while (i < length)
				{
					char c = path[i];
					if (c == '/' || c == '[' || this.m_Text[this.m_Position] != c)
					{
						break;
					}
					this.m_Position++;
					i++;
				}
				if (this.m_Position < this.m_Length && this.m_Text[this.m_Position] == '"' && (i >= length || path[i] == '/' || path[i] == '['))
				{
					this.m_Position++;
					if (!this.SkipToValue())
					{
						return false;
					}
					if (i >= length)
					{
						return true;
					}
					if (path[i] == '/')
					{
						i++;
						if (!this.ParseToken('{'))
						{
							return false;
						}
					}
					else if (path[i] == '[')
					{
						i++;
						if (i == length)
						{
							throw new ArgumentException("Malformed JSON property path: " + path, "path");
						}
						if (path[i] != ']')
						{
							throw new NotImplementedException("Navigating to specific array element");
						}
						this.m_MatchAnyElementInArray = true;
						i++;
						if (i == length)
						{
							return true;
						}
					}
				}
				else
				{
					i = num;
					while (this.m_Position < this.m_Length && this.m_Text[this.m_Position] != '"')
					{
						this.m_Position++;
					}
					if (this.m_Position == this.m_Length || this.m_Text[this.m_Position] != '"')
					{
						return false;
					}
					this.m_Position++;
					if (!this.SkipToValue() || !this.ParseValue())
					{
						return false;
					}
					this.SkipWhitespace();
					if (this.m_Position == this.m_Length || this.m_Text[this.m_Position] == '}' || this.m_Text[this.m_Position] != ',')
					{
						return false;
					}
					this.m_Position++;
				}
			}
			return false;
		}

		public bool CurrentPropertyHasValueEqualTo(JsonParser.JsonValue expectedValue)
		{
			int position = this.m_Position;
			this.m_DryRun = false;
			JsonParser.JsonValue jsonValue;
			if (!this.ParseValue(out jsonValue))
			{
				this.m_Position = position;
				return false;
			}
			this.m_Position = position;
			bool flag = false;
			if (jsonValue.type == JsonParser.JsonValueType.Array && this.m_MatchAnyElementInArray)
			{
				List<JsonParser.JsonValue> arrayValue = jsonValue.arrayValue;
				int num = 0;
				while (!flag)
				{
					if (num >= arrayValue.Count)
					{
						break;
					}
					flag = (arrayValue[num] == expectedValue);
					num++;
				}
			}
			else
			{
				flag = (jsonValue == expectedValue);
			}
			return flag;
		}

		public bool ParseToken(char token)
		{
			this.SkipWhitespace();
			if (this.m_Position == this.m_Length)
			{
				return false;
			}
			if (this.m_Text[this.m_Position] != token)
			{
				return false;
			}
			this.m_Position++;
			this.SkipWhitespace();
			return this.m_Position < this.m_Length;
		}

		public bool ParseValue()
		{
			JsonParser.JsonValue jsonValue;
			return this.ParseValue(out jsonValue);
		}

		public bool ParseValue(out JsonParser.JsonValue result)
		{
			result = default(JsonParser.JsonValue);
			this.SkipWhitespace();
			if (this.m_Position == this.m_Length)
			{
				return false;
			}
			char c = this.m_Text[this.m_Position];
			if (c <= 'f')
			{
				if (c != '"')
				{
					if (c != '[')
					{
						if (c != 'f')
						{
							goto IL_8D;
						}
					}
					else
					{
						if (this.ParseArrayValue(out result))
						{
							return true;
						}
						return false;
					}
				}
				else
				{
					if (this.ParseStringValue(out result))
					{
						return true;
					}
					return false;
				}
			}
			else if (c != 'n')
			{
				if (c != 't')
				{
					if (c != '{')
					{
						goto IL_8D;
					}
					if (this.ParseObjectValue(out result))
					{
						return true;
					}
					return false;
				}
			}
			else
			{
				if (this.ParseNullValue(out result))
				{
					return true;
				}
				return false;
			}
			if (this.ParseBooleanValue(out result))
			{
				return true;
			}
			return false;
			IL_8D:
			if (this.ParseNumber(out result))
			{
				return true;
			}
			return false;
		}

		public bool ParseStringValue(out JsonParser.JsonValue result)
		{
			result = default(JsonParser.JsonValue);
			this.SkipWhitespace();
			if (this.m_Position == this.m_Length || this.m_Text[this.m_Position] != '"')
			{
				return false;
			}
			this.m_Position++;
			int position = this.m_Position;
			bool hasEscapes = false;
			while (this.m_Position < this.m_Length)
			{
				char c = this.m_Text[this.m_Position];
				if (c == '\\')
				{
					this.m_Position++;
					if (this.m_Position == this.m_Length)
					{
						break;
					}
					hasEscapes = true;
				}
				else if (c == '"')
				{
					this.m_Position++;
					result = new JsonParser.JsonString
					{
						text = new Substring(this.m_Text, position, this.m_Position - position - 1),
						hasEscapes = hasEscapes
					};
					return true;
				}
				this.m_Position++;
			}
			return false;
		}

		public bool ParseArrayValue(out JsonParser.JsonValue result)
		{
			result = default(JsonParser.JsonValue);
			this.SkipWhitespace();
			if (this.m_Position == this.m_Length || this.m_Text[this.m_Position] != '[')
			{
				return false;
			}
			this.m_Position++;
			if (this.m_Position == this.m_Length)
			{
				return false;
			}
			if (this.m_Text[this.m_Position] == ']')
			{
				result = new JsonParser.JsonValue
				{
					type = JsonParser.JsonValueType.Array
				};
				this.m_Position++;
				return true;
			}
			List<JsonParser.JsonValue> list = null;
			if (!this.m_DryRun)
			{
				list = new List<JsonParser.JsonValue>();
			}
			while (this.m_Position < this.m_Length)
			{
				JsonParser.JsonValue item;
				if (!this.ParseValue(out item))
				{
					return false;
				}
				if (!this.m_DryRun)
				{
					list.Add(item);
				}
				this.SkipWhitespace();
				if (this.m_Position == this.m_Length)
				{
					return false;
				}
				char c = this.m_Text[this.m_Position];
				if (c == ']')
				{
					this.m_Position++;
					if (!this.m_DryRun)
					{
						result = list;
					}
					return true;
				}
				if (c == ',')
				{
					this.m_Position++;
				}
			}
			return false;
		}

		public bool ParseObjectValue(out JsonParser.JsonValue result)
		{
			result = default(JsonParser.JsonValue);
			if (!this.ParseToken('{'))
			{
				return false;
			}
			if (this.m_Position < this.m_Length && this.m_Text[this.m_Position] == '}')
			{
				result = new JsonParser.JsonValue
				{
					type = JsonParser.JsonValueType.Object
				};
				this.m_Position++;
				return true;
			}
			while (this.m_Position < this.m_Length)
			{
				JsonParser.JsonValue jsonValue;
				if (!this.ParseStringValue(out jsonValue))
				{
					return false;
				}
				if (!this.SkipToValue())
				{
					return false;
				}
				JsonParser.JsonValue jsonValue2;
				if (!this.ParseValue(out jsonValue2))
				{
					return false;
				}
				if (!this.m_DryRun)
				{
					throw new NotImplementedException();
				}
				this.SkipWhitespace();
				if (this.m_Position < this.m_Length && this.m_Text[this.m_Position] == '}')
				{
					if (!this.m_DryRun)
					{
						throw new NotImplementedException();
					}
					this.m_Position++;
					return true;
				}
			}
			return false;
		}

		public bool ParseNumber(out JsonParser.JsonValue result)
		{
			result = default(JsonParser.JsonValue);
			this.SkipWhitespace();
			if (this.m_Position == this.m_Length)
			{
				return false;
			}
			bool flag = false;
			bool flag2 = false;
			long num = 0L;
			double num2 = 0.0;
			double num3 = 10.0;
			int num4 = 0;
			if (this.m_Text[this.m_Position] == '-')
			{
				flag = true;
				this.m_Position++;
			}
			if (this.m_Position == this.m_Length || !char.IsDigit(this.m_Text[this.m_Position]))
			{
				return false;
			}
			while (this.m_Position < this.m_Length)
			{
				char c = this.m_Text[this.m_Position];
				if (c == '.' || c < '0' || c > '9')
				{
					break;
				}
				num = num * 10L + (long)((ulong)c) - 48L;
				this.m_Position++;
			}
			if (this.m_Position < this.m_Length && this.m_Text[this.m_Position] == '.')
			{
				flag2 = true;
				this.m_Position++;
				if (this.m_Position == this.m_Length || !char.IsDigit(this.m_Text[this.m_Position]))
				{
					return false;
				}
				while (this.m_Position < this.m_Length)
				{
					char c2 = this.m_Text[this.m_Position];
					if (c2 < '0' || c2 > '9')
					{
						break;
					}
					num2 = (double)(c2 - '0') / num3 + num2;
					num3 *= 10.0;
					this.m_Position++;
				}
			}
			if (this.m_Position < this.m_Length && (this.m_Text[this.m_Position] == 'e' || this.m_Text[this.m_Position] == 'E'))
			{
				this.m_Position++;
				bool flag3 = false;
				if (this.m_Position < this.m_Length && this.m_Text[this.m_Position] == '-')
				{
					flag3 = true;
					this.m_Position++;
				}
				else if (this.m_Position < this.m_Length && this.m_Text[this.m_Position] == '+')
				{
					this.m_Position++;
				}
				int num5 = 1;
				while (this.m_Position < this.m_Length && char.IsDigit(this.m_Text[this.m_Position]))
				{
					int num6 = (int)(this.m_Text[this.m_Position] - '0');
					num4 *= num5;
					num4 += num6;
					num5 *= 10;
					this.m_Position++;
				}
				if (flag3)
				{
					num4 *= -1;
				}
			}
			if (!this.m_DryRun)
			{
				if (!flag2 && num4 == 0)
				{
					if (flag)
					{
						result = -num;
					}
					else
					{
						result = num;
					}
				}
				else
				{
					float num7;
					if (flag)
					{
						num7 = (float)(-(float)((double)num + num2));
					}
					else
					{
						num7 = (float)((double)num + num2);
					}
					if (num4 != 0)
					{
						num7 *= Mathf.Pow(10f, (float)num4);
					}
					result = (double)num7;
				}
			}
			return true;
		}

		public bool ParseBooleanValue(out JsonParser.JsonValue result)
		{
			this.SkipWhitespace();
			if (this.SkipString("true"))
			{
				result = true;
				return true;
			}
			if (this.SkipString("false"))
			{
				result = false;
				return true;
			}
			result = default(JsonParser.JsonValue);
			return false;
		}

		public bool ParseNullValue(out JsonParser.JsonValue result)
		{
			result = default(JsonParser.JsonValue);
			return this.SkipString("null");
		}

		public bool SkipToValue()
		{
			this.SkipWhitespace();
			if (this.m_Position == this.m_Length || this.m_Text[this.m_Position] != ':')
			{
				return false;
			}
			this.m_Position++;
			this.SkipWhitespace();
			return true;
		}

		private bool SkipString(string text)
		{
			this.SkipWhitespace();
			int length = text.Length;
			if (this.m_Position + length >= this.m_Length)
			{
				return false;
			}
			for (int i = 0; i < length; i++)
			{
				if (this.m_Text[this.m_Position + i] != text[i])
				{
					return false;
				}
			}
			this.m_Position += length;
			return true;
		}

		private void SkipWhitespace()
		{
			while (this.m_Position < this.m_Length && char.IsWhiteSpace(this.m_Text[this.m_Position]))
			{
				this.m_Position++;
			}
		}

		public bool isAtEnd
		{
			get
			{
				return this.m_Position >= this.m_Length;
			}
		}

		private readonly string m_Text;

		private readonly int m_Length;

		private int m_Position;

		private bool m_MatchAnyElementInArray;

		private bool m_DryRun;

		public enum JsonValueType
		{
			None,
			Bool,
			Real,
			Integer,
			String,
			Array,
			Object,
			Any
		}

		public struct JsonString : IEquatable<JsonParser.JsonString>
		{
			public override string ToString()
			{
				if (!this.hasEscapes)
				{
					return this.text.ToString();
				}
				StringBuilder stringBuilder = new StringBuilder();
				int length = this.text.length;
				for (int i = 0; i < length; i++)
				{
					char c = this.text[i];
					if (c == '\\')
					{
						i++;
						if (i == length)
						{
							break;
						}
						c = this.text[i];
					}
					stringBuilder.Append(c);
				}
				return stringBuilder.ToString();
			}

			public bool Equals(JsonParser.JsonString other)
			{
				if (this.hasEscapes == other.hasEscapes)
				{
					return Substring.Compare(this.text, other.text, StringComparison.InvariantCultureIgnoreCase) == 0;
				}
				int length = this.text.length;
				int length2 = other.text.length;
				int num = 0;
				int num2 = 0;
				while (num < length && num2 < length2)
				{
					char c = this.text[num];
					char c2 = other.text[num2];
					if (c == '\\')
					{
						num++;
						if (num == length)
						{
							return false;
						}
						c = this.text[num];
					}
					if (c2 == '\\')
					{
						num2++;
						if (num2 == length2)
						{
							return false;
						}
						c2 = other.text[num2];
					}
					if (char.ToUpperInvariant(c) != char.ToUpperInvariant(c2))
					{
						return false;
					}
					num++;
					num2++;
				}
				return num == length && num2 == length2;
			}

			public override bool Equals(object obj)
			{
				if (obj is JsonParser.JsonString)
				{
					JsonParser.JsonString other = (JsonParser.JsonString)obj;
					return this.Equals(other);
				}
				return false;
			}

			public override int GetHashCode()
			{
				return this.text.GetHashCode() * 397 ^ this.hasEscapes.GetHashCode();
			}

			public static bool operator ==(JsonParser.JsonString left, JsonParser.JsonString right)
			{
				return left.Equals(right);
			}

			public static bool operator !=(JsonParser.JsonString left, JsonParser.JsonString right)
			{
				return !left.Equals(right);
			}

			public static implicit operator JsonParser.JsonString(string str)
			{
				return new JsonParser.JsonString
				{
					text = str
				};
			}

			public Substring text;

			public bool hasEscapes;
		}

		public struct JsonValue : IEquatable<JsonParser.JsonValue>
		{
			public bool ToBoolean()
			{
				switch (this.type)
				{
				case JsonParser.JsonValueType.Bool:
					return this.boolValue;
				case JsonParser.JsonValueType.Real:
					return NumberHelpers.Approximately(0.0, this.realValue);
				case JsonParser.JsonValueType.Integer:
					return this.integerValue != 0L;
				case JsonParser.JsonValueType.String:
					return Convert.ToBoolean(this.ToString());
				default:
					return false;
				}
			}

			public long ToInteger()
			{
				switch (this.type)
				{
				case JsonParser.JsonValueType.Bool:
					return this.boolValue ? 1L : 0L;
				case JsonParser.JsonValueType.Real:
					return (long)this.realValue;
				case JsonParser.JsonValueType.Integer:
					return this.integerValue;
				case JsonParser.JsonValueType.String:
					return Convert.ToInt64(this.ToString());
				default:
					return 0L;
				}
			}

			public double ToDouble()
			{
				switch (this.type)
				{
				case JsonParser.JsonValueType.Bool:
					return (double)(this.boolValue ? 1 : 0);
				case JsonParser.JsonValueType.Real:
					return this.realValue;
				case JsonParser.JsonValueType.Integer:
					return (double)this.integerValue;
				case JsonParser.JsonValueType.String:
					return (double)Convert.ToSingle(this.ToString());
				default:
					return 0.0;
				}
			}

			public override string ToString()
			{
				switch (this.type)
				{
				case JsonParser.JsonValueType.None:
					return "null";
				case JsonParser.JsonValueType.Bool:
					return this.boolValue.ToString();
				case JsonParser.JsonValueType.Real:
					return this.realValue.ToString(CultureInfo.InvariantCulture);
				case JsonParser.JsonValueType.Integer:
					return this.integerValue.ToString(CultureInfo.InvariantCulture);
				case JsonParser.JsonValueType.String:
					return this.stringValue.ToString();
				case JsonParser.JsonValueType.Array:
					if (this.arrayValue == null)
					{
						return "[]";
					}
					return "[" + string.Join(",", from x in this.arrayValue
					select x.ToString()) + "]";
				case JsonParser.JsonValueType.Object:
				{
					if (this.objectValue == null)
					{
						return "{}";
					}
					IEnumerable<string> values = from pair in this.objectValue
					select string.Format("\"{0}\" : \"{1}\"", pair.Key, pair.Value);
					return "{" + string.Join(",", values) + "}";
				}
				case JsonParser.JsonValueType.Any:
					return this.anyValue.ToString();
				default:
					return base.ToString();
				}
			}

			public static implicit operator JsonParser.JsonValue(bool val)
			{
				return new JsonParser.JsonValue
				{
					type = JsonParser.JsonValueType.Bool,
					boolValue = val
				};
			}

			public static implicit operator JsonParser.JsonValue(long val)
			{
				return new JsonParser.JsonValue
				{
					type = JsonParser.JsonValueType.Integer,
					integerValue = val
				};
			}

			public static implicit operator JsonParser.JsonValue(double val)
			{
				return new JsonParser.JsonValue
				{
					type = JsonParser.JsonValueType.Real,
					realValue = val
				};
			}

			public static implicit operator JsonParser.JsonValue(string str)
			{
				return new JsonParser.JsonValue
				{
					type = JsonParser.JsonValueType.String,
					stringValue = new JsonParser.JsonString
					{
						text = str
					}
				};
			}

			public static implicit operator JsonParser.JsonValue(JsonParser.JsonString str)
			{
				return new JsonParser.JsonValue
				{
					type = JsonParser.JsonValueType.String,
					stringValue = str
				};
			}

			public static implicit operator JsonParser.JsonValue(List<JsonParser.JsonValue> array)
			{
				return new JsonParser.JsonValue
				{
					type = JsonParser.JsonValueType.Array,
					arrayValue = array
				};
			}

			public static implicit operator JsonParser.JsonValue(Dictionary<string, JsonParser.JsonValue> obj)
			{
				return new JsonParser.JsonValue
				{
					type = JsonParser.JsonValueType.Object,
					objectValue = obj
				};
			}

			public static implicit operator JsonParser.JsonValue(Enum val)
			{
				return new JsonParser.JsonValue
				{
					type = JsonParser.JsonValueType.Any,
					anyValue = val
				};
			}

			public bool Equals(JsonParser.JsonValue other)
			{
				if (this.type == other.type)
				{
					switch (this.type)
					{
					case JsonParser.JsonValueType.None:
						return true;
					case JsonParser.JsonValueType.Bool:
						return this.boolValue == other.boolValue;
					case JsonParser.JsonValueType.Real:
						return NumberHelpers.Approximately(this.realValue, other.realValue);
					case JsonParser.JsonValueType.Integer:
						return this.integerValue == other.integerValue;
					case JsonParser.JsonValueType.String:
						return this.stringValue == other.stringValue;
					case JsonParser.JsonValueType.Array:
						throw new NotImplementedException();
					case JsonParser.JsonValueType.Object:
						throw new NotImplementedException();
					case JsonParser.JsonValueType.Any:
						return this.anyValue.Equals(other.anyValue);
					default:
						return false;
					}
				}
				else
				{
					if (this.anyValue != null)
					{
						return JsonParser.JsonValue.Equals(this.anyValue, other);
					}
					return other.anyValue != null && JsonParser.JsonValue.Equals(other.anyValue, this);
				}
			}

			private static bool Equals(object obj, JsonParser.JsonValue value)
			{
				if (obj == null)
				{
					return false;
				}
				Regex regex = obj as Regex;
				if (regex != null)
				{
					return regex.IsMatch(value.ToString());
				}
				string text = obj as string;
				if (text != null)
				{
					switch (value.type)
					{
					case JsonParser.JsonValueType.Bool:
						if (value.boolValue)
						{
							return text == "True" || text == "true" || text == "1";
						}
						return text == "False" || text == "false" || text == "0";
					case JsonParser.JsonValueType.Real:
					{
						double a;
						return double.TryParse(text, out a) && NumberHelpers.Approximately(a, value.realValue);
					}
					case JsonParser.JsonValueType.Integer:
					{
						long num;
						return long.TryParse(text, out num) && num == value.integerValue;
					}
					case JsonParser.JsonValueType.String:
						return value.stringValue == text;
					}
				}
				if (obj is float)
				{
					float num2 = (float)obj;
					if (value.type == JsonParser.JsonValueType.Real)
					{
						return NumberHelpers.Approximately((double)num2, value.realValue);
					}
					if (value.type == JsonParser.JsonValueType.String)
					{
						float b;
						return float.TryParse(value.ToString(), out b) && Mathf.Approximately(num2, b);
					}
				}
				if (obj is double)
				{
					double a2 = (double)obj;
					if (value.type == JsonParser.JsonValueType.Real)
					{
						return NumberHelpers.Approximately(a2, value.realValue);
					}
					if (value.type == JsonParser.JsonValueType.String)
					{
						double b2;
						return double.TryParse(value.ToString(), out b2) && NumberHelpers.Approximately(a2, b2);
					}
				}
				if (obj is int)
				{
					int num3 = (int)obj;
					if (value.type == JsonParser.JsonValueType.Integer)
					{
						return (long)num3 == value.integerValue;
					}
					if (value.type == JsonParser.JsonValueType.String)
					{
						int num4;
						return int.TryParse(value.ToString(), out num4) && num3 == num4;
					}
				}
				if (obj is long)
				{
					long num5 = (long)obj;
					if (value.type == JsonParser.JsonValueType.Integer)
					{
						return num5 == value.integerValue;
					}
					if (value.type == JsonParser.JsonValueType.String)
					{
						long num6;
						return long.TryParse(value.ToString(), out num6) && num5 == num6;
					}
				}
				if (obj is bool)
				{
					bool flag = (bool)obj;
					if (value.type == JsonParser.JsonValueType.Bool)
					{
						return flag == value.boolValue;
					}
					if (value.type == JsonParser.JsonValueType.String)
					{
						if (flag)
						{
							return value.stringValue == "true" || value.stringValue == "True" || value.stringValue == "1";
						}
						return value.stringValue == "false" || value.stringValue == "False" || value.stringValue == "0";
					}
				}
				if (obj is Enum)
				{
					if (value.type == JsonParser.JsonValueType.Integer)
					{
						return Convert.ToInt64(obj) == value.integerValue;
					}
					if (value.type == JsonParser.JsonValueType.String)
					{
						return value.stringValue == Enum.GetName(obj.GetType(), obj);
					}
				}
				return false;
			}

			public override bool Equals(object obj)
			{
				if (obj is JsonParser.JsonValue)
				{
					JsonParser.JsonValue other = (JsonParser.JsonValue)obj;
					return this.Equals(other);
				}
				return false;
			}

			public override int GetHashCode()
			{
				return (int)(((((((this.type * (JsonParser.JsonValueType)397 ^ (JsonParser.JsonValueType)this.boolValue.GetHashCode()) * (JsonParser.JsonValueType)397 ^ (JsonParser.JsonValueType)this.realValue.GetHashCode()) * (JsonParser.JsonValueType)397 ^ (JsonParser.JsonValueType)this.integerValue.GetHashCode()) * (JsonParser.JsonValueType)397 ^ (JsonParser.JsonValueType)this.stringValue.GetHashCode()) * (JsonParser.JsonValueType)397 ^ (JsonParser.JsonValueType)((this.arrayValue != null) ? this.arrayValue.GetHashCode() : 0)) * (JsonParser.JsonValueType)397 ^ (JsonParser.JsonValueType)((this.objectValue != null) ? this.objectValue.GetHashCode() : 0)) * (JsonParser.JsonValueType)397 ^ (JsonParser.JsonValueType)((this.anyValue != null) ? this.anyValue.GetHashCode() : 0));
			}

			public static bool operator ==(JsonParser.JsonValue left, JsonParser.JsonValue right)
			{
				return left.Equals(right);
			}

			public static bool operator !=(JsonParser.JsonValue left, JsonParser.JsonValue right)
			{
				return !left.Equals(right);
			}

			public JsonParser.JsonValueType type;

			public bool boolValue;

			public double realValue;

			public long integerValue;

			public JsonParser.JsonString stringValue;

			public List<JsonParser.JsonValue> arrayValue;

			public Dictionary<string, JsonParser.JsonValue> objectValue;

			public object anyValue;
		}
	}
}
