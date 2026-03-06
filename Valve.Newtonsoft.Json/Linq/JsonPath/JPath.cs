using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Valve.Newtonsoft.Json.Utilities;

namespace Valve.Newtonsoft.Json.Linq.JsonPath
{
	internal class JPath
	{
		public List<PathFilter> Filters { get; private set; }

		public JPath(string expression)
		{
			ValidationUtils.ArgumentNotNull(expression, "expression");
			this._expression = expression;
			this.Filters = new List<PathFilter>();
			this.ParseMain();
		}

		private void ParseMain()
		{
			int currentIndex = this._currentIndex;
			this.EatWhitespace();
			if (this._expression.Length == this._currentIndex)
			{
				return;
			}
			if (this._expression[this._currentIndex] == '$')
			{
				if (this._expression.Length == 1)
				{
					return;
				}
				char c = this._expression[this._currentIndex + 1];
				if (c == '.' || c == '[')
				{
					this._currentIndex++;
					currentIndex = this._currentIndex;
				}
			}
			if (!this.ParsePath(this.Filters, currentIndex, false))
			{
				int currentIndex2 = this._currentIndex;
				this.EatWhitespace();
				if (this._currentIndex < this._expression.Length)
				{
					throw new JsonException("Unexpected character while parsing path: " + this._expression[currentIndex2].ToString());
				}
			}
		}

		private bool ParsePath(List<PathFilter> filters, int currentPartStartIndex, bool query)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			while (this._currentIndex < this._expression.Length && !flag4)
			{
				char c = this._expression[this._currentIndex];
				if (c <= ')')
				{
					if (c != ' ')
					{
						if (c != '(')
						{
							if (c != ')')
							{
								goto IL_1BF;
							}
							goto IL_E8;
						}
					}
					else
					{
						if (this._currentIndex < this._expression.Length)
						{
							flag4 = true;
							continue;
						}
						continue;
					}
				}
				else
				{
					if (c == '.')
					{
						if (this._currentIndex > currentPartStartIndex)
						{
							string text = this._expression.Substring(currentPartStartIndex, this._currentIndex - currentPartStartIndex);
							if (text == "*")
							{
								text = null;
							}
							FieldFilter fieldFilter;
							if (!flag)
							{
								(fieldFilter = new FieldFilter()).Name = text;
							}
							else
							{
								(fieldFilter = new ScanFilter()).Name = text;
							}
							PathFilter item = fieldFilter;
							filters.Add(item);
							flag = false;
						}
						if (this._currentIndex + 1 < this._expression.Length && this._expression[this._currentIndex + 1] == '.')
						{
							flag = true;
							this._currentIndex++;
						}
						this._currentIndex++;
						currentPartStartIndex = this._currentIndex;
						flag2 = false;
						flag3 = true;
						continue;
					}
					if (c != '[')
					{
						if (c != ']')
						{
							goto IL_1BF;
						}
						goto IL_E8;
					}
				}
				if (this._currentIndex > currentPartStartIndex)
				{
					string text2 = this._expression.Substring(currentPartStartIndex, this._currentIndex - currentPartStartIndex);
					if (text2 == "*")
					{
						text2 = null;
					}
					FieldFilter fieldFilter2;
					if (!flag)
					{
						(fieldFilter2 = new FieldFilter()).Name = text2;
					}
					else
					{
						(fieldFilter2 = new ScanFilter()).Name = text2;
					}
					PathFilter item2 = fieldFilter2;
					filters.Add(item2);
					flag = false;
				}
				filters.Add(this.ParseIndexer(c));
				this._currentIndex++;
				currentPartStartIndex = this._currentIndex;
				flag2 = true;
				flag3 = false;
				continue;
				IL_E8:
				flag4 = true;
				continue;
				IL_1BF:
				if (query && (c == '=' || c == '<' || c == '!' || c == '>' || c == '|' || c == '&'))
				{
					flag4 = true;
				}
				else
				{
					if (flag2)
					{
						throw new JsonException("Unexpected character following indexer: " + c.ToString());
					}
					this._currentIndex++;
				}
			}
			bool flag5 = this._currentIndex == this._expression.Length;
			if (this._currentIndex > currentPartStartIndex)
			{
				string text3 = this._expression.Substring(currentPartStartIndex, this._currentIndex - currentPartStartIndex).TrimEnd(new char[0]);
				if (text3 == "*")
				{
					text3 = null;
				}
				FieldFilter fieldFilter3;
				if (!flag)
				{
					(fieldFilter3 = new FieldFilter()).Name = text3;
				}
				else
				{
					(fieldFilter3 = new ScanFilter()).Name = text3;
				}
				PathFilter item3 = fieldFilter3;
				filters.Add(item3);
			}
			else if (flag3 && (flag5 || query))
			{
				throw new JsonException("Unexpected end while parsing path.");
			}
			return flag5;
		}

		private PathFilter ParseIndexer(char indexerOpenChar)
		{
			this._currentIndex++;
			char indexerCloseChar = (indexerOpenChar == '[') ? ']' : ')';
			this.EnsureLength("Path ended with open indexer.");
			this.EatWhitespace();
			if (this._expression[this._currentIndex] == '\'')
			{
				return this.ParseQuotedField(indexerCloseChar);
			}
			if (this._expression[this._currentIndex] == '?')
			{
				return this.ParseQuery(indexerCloseChar);
			}
			return this.ParseArrayIndexer(indexerCloseChar);
		}

		private PathFilter ParseArrayIndexer(char indexerCloseChar)
		{
			int currentIndex = this._currentIndex;
			int? num = null;
			List<int> list = null;
			int num2 = 0;
			int? start = null;
			int? end = null;
			int? step = null;
			while (this._currentIndex < this._expression.Length)
			{
				char c = this._expression[this._currentIndex];
				if (c == ' ')
				{
					num = new int?(this._currentIndex);
					this.EatWhitespace();
				}
				else if (c == indexerCloseChar)
				{
					int num3 = (num ?? this._currentIndex) - currentIndex;
					if (list != null)
					{
						if (num3 == 0)
						{
							throw new JsonException("Array index expected.");
						}
						int item = Convert.ToInt32(this._expression.Substring(currentIndex, num3), CultureInfo.InvariantCulture);
						list.Add(item);
						return new ArrayMultipleIndexFilter
						{
							Indexes = list
						};
					}
					else
					{
						if (num2 > 0)
						{
							if (num3 > 0)
							{
								int value = Convert.ToInt32(this._expression.Substring(currentIndex, num3), CultureInfo.InvariantCulture);
								if (num2 == 1)
								{
									end = new int?(value);
								}
								else
								{
									step = new int?(value);
								}
							}
							return new ArraySliceFilter
							{
								Start = start,
								End = end,
								Step = step
							};
						}
						if (num3 == 0)
						{
							throw new JsonException("Array index expected.");
						}
						int value2 = Convert.ToInt32(this._expression.Substring(currentIndex, num3), CultureInfo.InvariantCulture);
						return new ArrayIndexFilter
						{
							Index = new int?(value2)
						};
					}
				}
				else if (c == ',')
				{
					int num4 = (num ?? this._currentIndex) - currentIndex;
					if (num4 == 0)
					{
						throw new JsonException("Array index expected.");
					}
					if (list == null)
					{
						list = new List<int>();
					}
					string value3 = this._expression.Substring(currentIndex, num4);
					list.Add(Convert.ToInt32(value3, CultureInfo.InvariantCulture));
					this._currentIndex++;
					this.EatWhitespace();
					currentIndex = this._currentIndex;
					num = null;
				}
				else if (c == '*')
				{
					this._currentIndex++;
					this.EnsureLength("Path ended with open indexer.");
					this.EatWhitespace();
					if (this._expression[this._currentIndex] != indexerCloseChar)
					{
						throw new JsonException("Unexpected character while parsing path indexer: " + c.ToString());
					}
					return new ArrayIndexFilter();
				}
				else if (c == ':')
				{
					int num5 = (num ?? this._currentIndex) - currentIndex;
					if (num5 > 0)
					{
						int value4 = Convert.ToInt32(this._expression.Substring(currentIndex, num5), CultureInfo.InvariantCulture);
						if (num2 == 0)
						{
							start = new int?(value4);
						}
						else if (num2 == 1)
						{
							end = new int?(value4);
						}
						else
						{
							step = new int?(value4);
						}
					}
					num2++;
					this._currentIndex++;
					this.EatWhitespace();
					currentIndex = this._currentIndex;
					num = null;
				}
				else
				{
					if (!char.IsDigit(c) && c != '-')
					{
						throw new JsonException("Unexpected character while parsing path indexer: " + c.ToString());
					}
					if (num != null)
					{
						throw new JsonException("Unexpected character while parsing path indexer: " + c.ToString());
					}
					this._currentIndex++;
				}
			}
			throw new JsonException("Path ended with open indexer.");
		}

		private void EatWhitespace()
		{
			while (this._currentIndex < this._expression.Length && this._expression[this._currentIndex] == ' ')
			{
				this._currentIndex++;
			}
		}

		private PathFilter ParseQuery(char indexerCloseChar)
		{
			this._currentIndex++;
			this.EnsureLength("Path ended with open indexer.");
			if (this._expression[this._currentIndex] != '(')
			{
				throw new JsonException("Unexpected character while parsing path indexer: " + this._expression[this._currentIndex].ToString());
			}
			this._currentIndex++;
			QueryExpression expression = this.ParseExpression();
			this._currentIndex++;
			this.EnsureLength("Path ended with open indexer.");
			this.EatWhitespace();
			if (this._expression[this._currentIndex] != indexerCloseChar)
			{
				throw new JsonException("Unexpected character while parsing path indexer: " + this._expression[this._currentIndex].ToString());
			}
			return new QueryFilter
			{
				Expression = expression
			};
		}

		private QueryExpression ParseExpression()
		{
			QueryExpression queryExpression = null;
			CompositeExpression compositeExpression = null;
			while (this._currentIndex < this._expression.Length)
			{
				this.EatWhitespace();
				if (this._expression[this._currentIndex] != '@')
				{
					throw new JsonException("Unexpected character while parsing path query: " + this._expression[this._currentIndex].ToString());
				}
				this._currentIndex++;
				List<PathFilter> list = new List<PathFilter>();
				if (this.ParsePath(list, this._currentIndex, true))
				{
					throw new JsonException("Path ended with open query.");
				}
				this.EatWhitespace();
				this.EnsureLength("Path ended with open query.");
				object value = null;
				QueryOperator queryOperator;
				if (this._expression[this._currentIndex] == ')' || this._expression[this._currentIndex] == '|' || this._expression[this._currentIndex] == '&')
				{
					queryOperator = QueryOperator.Exists;
				}
				else
				{
					queryOperator = this.ParseOperator();
					this.EatWhitespace();
					this.EnsureLength("Path ended with open query.");
					value = this.ParseValue();
					this.EatWhitespace();
					this.EnsureLength("Path ended with open query.");
				}
				BooleanQueryExpression booleanQueryExpression = new BooleanQueryExpression
				{
					Path = list,
					Operator = queryOperator,
					Value = ((queryOperator != QueryOperator.Exists) ? new JValue(value) : null)
				};
				if (this._expression[this._currentIndex] == ')')
				{
					if (compositeExpression != null)
					{
						compositeExpression.Expressions.Add(booleanQueryExpression);
						return queryExpression;
					}
					return booleanQueryExpression;
				}
				else
				{
					if (this._expression[this._currentIndex] == '&' && this.Match("&&"))
					{
						if (compositeExpression == null || compositeExpression.Operator != QueryOperator.And)
						{
							CompositeExpression compositeExpression2 = new CompositeExpression
							{
								Operator = QueryOperator.And
							};
							if (compositeExpression != null)
							{
								compositeExpression.Expressions.Add(compositeExpression2);
							}
							compositeExpression = compositeExpression2;
							if (queryExpression == null)
							{
								queryExpression = compositeExpression;
							}
						}
						compositeExpression.Expressions.Add(booleanQueryExpression);
					}
					if (this._expression[this._currentIndex] == '|' && this.Match("||"))
					{
						if (compositeExpression == null || compositeExpression.Operator != QueryOperator.Or)
						{
							CompositeExpression compositeExpression3 = new CompositeExpression
							{
								Operator = QueryOperator.Or
							};
							if (compositeExpression != null)
							{
								compositeExpression.Expressions.Add(compositeExpression3);
							}
							compositeExpression = compositeExpression3;
							if (queryExpression == null)
							{
								queryExpression = compositeExpression;
							}
						}
						compositeExpression.Expressions.Add(booleanQueryExpression);
					}
				}
			}
			throw new JsonException("Path ended with open query.");
		}

		private object ParseValue()
		{
			char c = this._expression[this._currentIndex];
			if (c == '\'')
			{
				return this.ReadQuotedString();
			}
			if (char.IsDigit(c) || c == '-')
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(c);
				this._currentIndex++;
				while (this._currentIndex < this._expression.Length)
				{
					c = this._expression[this._currentIndex];
					if (c == ' ' || c == ')')
					{
						string text = stringBuilder.ToString();
						if (text.IndexOfAny(new char[]
						{
							'.',
							'E',
							'e'
						}) != -1)
						{
							double num;
							if (double.TryParse(text, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out num))
							{
								return num;
							}
							throw new JsonException("Could not read query value.");
						}
						else
						{
							long num2;
							if (long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out num2))
							{
								return num2;
							}
							throw new JsonException("Could not read query value.");
						}
					}
					else
					{
						stringBuilder.Append(c);
						this._currentIndex++;
					}
				}
			}
			else if (c == 't')
			{
				if (this.Match("true"))
				{
					return true;
				}
			}
			else if (c == 'f')
			{
				if (this.Match("false"))
				{
					return false;
				}
			}
			else if (c == 'n' && this.Match("null"))
			{
				return null;
			}
			throw new JsonException("Could not read query value.");
		}

		private string ReadQuotedString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this._currentIndex++;
			while (this._currentIndex < this._expression.Length)
			{
				char c = this._expression[this._currentIndex];
				if (c == '\\' && this._currentIndex + 1 < this._expression.Length)
				{
					this._currentIndex++;
					if (this._expression[this._currentIndex] == '\'')
					{
						stringBuilder.Append('\'');
					}
					else
					{
						if (this._expression[this._currentIndex] != '\\')
						{
							throw new JsonException("Unknown escape chracter: \\" + this._expression[this._currentIndex].ToString());
						}
						stringBuilder.Append('\\');
					}
					this._currentIndex++;
				}
				else
				{
					if (c == '\'')
					{
						this._currentIndex++;
						return stringBuilder.ToString();
					}
					this._currentIndex++;
					stringBuilder.Append(c);
				}
			}
			throw new JsonException("Path ended with an open string.");
		}

		private bool Match(string s)
		{
			int num = this._currentIndex;
			foreach (char c in s)
			{
				if (num >= this._expression.Length || this._expression[num] != c)
				{
					return false;
				}
				num++;
			}
			this._currentIndex = num;
			return true;
		}

		private QueryOperator ParseOperator()
		{
			if (this._currentIndex + 1 >= this._expression.Length)
			{
				throw new JsonException("Path ended with open query.");
			}
			if (this.Match("=="))
			{
				return QueryOperator.Equals;
			}
			if (this.Match("!=") || this.Match("<>"))
			{
				return QueryOperator.NotEquals;
			}
			if (this.Match("<="))
			{
				return QueryOperator.LessThanOrEquals;
			}
			if (this.Match("<"))
			{
				return QueryOperator.LessThan;
			}
			if (this.Match(">="))
			{
				return QueryOperator.GreaterThanOrEquals;
			}
			if (this.Match(">"))
			{
				return QueryOperator.GreaterThan;
			}
			throw new JsonException("Could not read query operator.");
		}

		private PathFilter ParseQuotedField(char indexerCloseChar)
		{
			List<string> list = null;
			while (this._currentIndex < this._expression.Length)
			{
				string text = this.ReadQuotedString();
				this.EatWhitespace();
				this.EnsureLength("Path ended with open indexer.");
				if (this._expression[this._currentIndex] == indexerCloseChar)
				{
					if (list != null)
					{
						list.Add(text);
						return new FieldMultipleFilter
						{
							Names = list
						};
					}
					return new FieldFilter
					{
						Name = text
					};
				}
				else
				{
					if (this._expression[this._currentIndex] != ',')
					{
						throw new JsonException("Unexpected character while parsing path indexer: " + this._expression[this._currentIndex].ToString());
					}
					this._currentIndex++;
					this.EatWhitespace();
					if (list == null)
					{
						list = new List<string>();
					}
					list.Add(text);
				}
			}
			throw new JsonException("Path ended with open indexer.");
		}

		private void EnsureLength(string message)
		{
			if (this._currentIndex >= this._expression.Length)
			{
				throw new JsonException(message);
			}
		}

		internal IEnumerable<JToken> Evaluate(JToken t, bool errorWhenNoMatch)
		{
			return JPath.Evaluate(this.Filters, t, errorWhenNoMatch);
		}

		internal static IEnumerable<JToken> Evaluate(List<PathFilter> filters, JToken t, bool errorWhenNoMatch)
		{
			IEnumerable<JToken> enumerable = new JToken[]
			{
				t
			};
			foreach (PathFilter pathFilter in filters)
			{
				enumerable = pathFilter.ExecuteFilter(enumerable, errorWhenNoMatch);
			}
			return enumerable;
		}

		private readonly string _expression;

		private int _currentIndex;
	}
}
