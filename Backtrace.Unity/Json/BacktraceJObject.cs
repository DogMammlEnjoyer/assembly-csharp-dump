using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Backtrace.Unity.Json
{
	public class BacktraceJObject
	{
		public BacktraceJObject() : this(null)
		{
		}

		public BacktraceJObject(IDictionary<string, string> source)
		{
			IDictionary<string, string> userPrimitives;
			if (source != null)
			{
				userPrimitives = source;
			}
			else
			{
				IDictionary<string, string> dictionary = new Dictionary<string, string>();
				userPrimitives = dictionary;
			}
			this.UserPrimitives = userPrimitives;
		}

		public void Add(string key, bool value)
		{
			this.PrimitiveValues.Add(key, value.ToString(CultureInfo.InvariantCulture).ToLower());
		}

		public void Add(string key, float value, string format = "G")
		{
			this.PrimitiveValues.Add(key, value.ToString(format, CultureInfo.InvariantCulture));
		}

		public void Add(string key, double value, string format = "G")
		{
			this.PrimitiveValues.Add(key, value.ToString(format, CultureInfo.InvariantCulture));
		}

		public void Add(string key, string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				value = string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("\"");
			this.EscapeString(value, stringBuilder);
			stringBuilder.Append("\"");
			this.PrimitiveValues.Add(key, stringBuilder.ToString());
		}

		public void Add(string key, long value)
		{
			this.PrimitiveValues.Add(key, value.ToString(CultureInfo.InvariantCulture));
		}

		public void Add(string key, BacktraceJObject value)
		{
			if (value != null)
			{
				this.InnerObjects.Add(key, value);
				return;
			}
			this.ComplexObjects.Add(key, null);
		}

		public void Add(string key, IEnumerable value)
		{
			this.ComplexObjects.Add(key, value);
		}

		public string ToJson()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.ToJson(stringBuilder);
			return stringBuilder.ToString();
		}

		internal void ToJson(StringBuilder stringBuilder)
		{
			stringBuilder.Append("{");
			this.AppendPrimitives(stringBuilder);
			this.AddUserPrimitives(stringBuilder);
			this.AppendJObjects(stringBuilder);
			this.AppendComplexValues(stringBuilder);
			stringBuilder.Append("}");
		}

		private void AddUserPrimitives(StringBuilder stringBuilder)
		{
			if (this.UserPrimitives.Count == 0)
			{
				return;
			}
			int num = 0;
			if (this.ShouldContinueAddingJSONProperties(stringBuilder))
			{
				stringBuilder.Append(',');
			}
			using (IEnumerator<KeyValuePair<string, string>> enumerator = this.UserPrimitives.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					num++;
					KeyValuePair<string, string> keyValuePair = enumerator.Current;
					this.AppendKey(keyValuePair.Key, stringBuilder);
					if (string.IsNullOrEmpty(keyValuePair.Value))
					{
						stringBuilder.Append("\"\"");
					}
					else
					{
						stringBuilder.Append("\"");
						this.EscapeString(keyValuePair.Value, stringBuilder);
						stringBuilder.Append("\"");
					}
					if (num != this.UserPrimitives.Count)
					{
						stringBuilder.Append(",");
					}
				}
			}
		}

		private void AppendPrimitives(StringBuilder stringBuilder)
		{
			int num = 0;
			using (Dictionary<string, string>.Enumerator enumerator = this.PrimitiveValues.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					num++;
					KeyValuePair<string, string> keyValuePair = enumerator.Current;
					this.AppendKey(keyValuePair.Key, stringBuilder);
					stringBuilder.Append(string.IsNullOrEmpty(keyValuePair.Value) ? "\"\"" : keyValuePair.Value);
					if (num != this.PrimitiveValues.Count)
					{
						stringBuilder.Append(",");
					}
				}
			}
		}

		private void AppendJObjects(StringBuilder stringBuilder)
		{
			if (this.InnerObjects.Count == 0)
			{
				return;
			}
			int num = 0;
			using (Dictionary<string, BacktraceJObject>.Enumerator enumerator = this.InnerObjects.GetEnumerator())
			{
				if (this.ShouldContinueAddingJSONProperties(stringBuilder))
				{
					stringBuilder.Append(',');
				}
				while (enumerator.MoveNext())
				{
					num++;
					KeyValuePair<string, BacktraceJObject> keyValuePair = enumerator.Current;
					this.AppendKey(keyValuePair.Key, stringBuilder);
					keyValuePair.Value.ToJson(stringBuilder);
					if (num != this.InnerObjects.Count)
					{
						stringBuilder.Append(",");
					}
				}
			}
		}

		private void AppendComplexValues(StringBuilder stringBuilder)
		{
			if (this.ComplexObjects.Count == 0)
			{
				return;
			}
			int num = 0;
			using (Dictionary<string, object>.Enumerator enumerator = this.ComplexObjects.GetEnumerator())
			{
				if (this.ShouldContinueAddingJSONProperties(stringBuilder))
				{
					stringBuilder.Append(',');
				}
				while (enumerator.MoveNext())
				{
					num++;
					KeyValuePair<string, object> keyValuePair = enumerator.Current;
					this.AppendKey(keyValuePair.Key, stringBuilder);
					if (keyValuePair.Value == null)
					{
						stringBuilder.Append("null");
					}
					else if (keyValuePair.Value is IEnumerable && !(keyValuePair.Value is IDictionary))
					{
						stringBuilder.Append('[');
						int num2 = 0;
						foreach (object obj in (keyValuePair.Value as IEnumerable))
						{
							if (num2 != 0)
							{
								stringBuilder.Append(',');
							}
							if (obj == null)
							{
								stringBuilder.Append("\"\"");
							}
							else if (obj is BacktraceJObject)
							{
								(obj as BacktraceJObject).ToJson(stringBuilder);
							}
							else
							{
								stringBuilder.Append("\"");
								this.EscapeString(obj.ToString(), stringBuilder);
								stringBuilder.Append("\"");
							}
							num2++;
						}
						stringBuilder.Append(']');
					}
					if (num != this.ComplexObjects.Count)
					{
						stringBuilder.Append(",");
					}
				}
			}
		}

		private bool ShouldContinueAddingJSONProperties(StringBuilder stringBuilder)
		{
			return stringBuilder[stringBuilder.Length - 1] != ',' && stringBuilder[stringBuilder.Length - 1] != '{';
		}

		private void AppendKey(string value, StringBuilder builder)
		{
			builder.Append("\"");
			if (string.IsNullOrEmpty(value))
			{
				builder.Append("\"\"");
			}
			else
			{
				this.EscapeString(value, builder);
			}
			builder.Append("\":");
		}

		private void EscapeString(string value, StringBuilder output)
		{
			int i = 0;
			while (i < value.Length)
			{
				char c = value[i];
				switch (c)
				{
				case '\b':
					output.Append("\\b");
					break;
				case '\t':
					output.Append("\\t");
					break;
				case '\n':
					output.Append("\\n");
					break;
				case '\v':
					goto IL_9D;
				case '\f':
					output.Append("\\f");
					break;
				case '\r':
					output.Append("\\r");
					break;
				default:
					if (c != '"')
					{
						if (c != '\\')
						{
							goto IL_9D;
						}
						output.Append("\\\\");
					}
					else
					{
						output.Append("\\\"");
					}
					break;
				}
				IL_B9:
				i++;
				continue;
				IL_9D:
				if (char.GetUnicodeCategory(c) == UnicodeCategory.Control)
				{
					this.ToCharAsUnicodeToStringBuilder(c, output);
					goto IL_B9;
				}
				output.Append(c);
				goto IL_B9;
			}
		}

		private char IntToHex(int n)
		{
			if (n <= 9)
			{
				return (char)(n + 48);
			}
			return (char)(n - 10 + 97);
		}

		private void ToCharAsUnicodeToStringBuilder(char c, StringBuilder output)
		{
			output.AppendFormat("\\u{0}{1}{2}{3}", new object[]
			{
				this.IntToHex((int)(c >> 12 & '\u000f')),
				this.IntToHex((int)(c >> 8 & '\u000f')),
				this.IntToHex((int)(c >> 4 & '\u000f')),
				this.IntToHex((int)(c & '\u000f'))
			});
		}

		internal readonly Dictionary<string, string> PrimitiveValues = new Dictionary<string, string>();

		internal readonly IDictionary<string, string> UserPrimitives;

		internal readonly Dictionary<string, BacktraceJObject> InnerObjects = new Dictionary<string, BacktraceJObject>();

		internal readonly Dictionary<string, object> ComplexObjects = new Dictionary<string, object>();
	}
}
