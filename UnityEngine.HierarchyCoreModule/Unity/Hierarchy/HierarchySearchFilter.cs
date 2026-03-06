using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace Unity.Hierarchy
{
	[RequiredByNativeCode]
	[NativeHeader("Modules/HierarchyCore/Public/HierarchySearch.h")]
	[Serializable]
	public struct HierarchySearchFilter
	{
		public static ref readonly HierarchySearchFilter Invalid
		{
			get
			{
				return ref HierarchySearchFilter.s_Invalid;
			}
		}

		public bool IsValid
		{
			get
			{
				return !string.IsNullOrEmpty(this.Name);
			}
		}

		public string Name { readonly get; set; }

		public string Value { readonly get; set; }

		public float NumValue { readonly get; set; }

		public HierarchySearchFilterOperator Op { readonly get; set; }

		public static string ToString(HierarchySearchFilterOperator op)
		{
			string result;
			switch (op)
			{
			case HierarchySearchFilterOperator.Equal:
				result = "=";
				break;
			case HierarchySearchFilterOperator.Contains:
				result = ":";
				break;
			case HierarchySearchFilterOperator.Greater:
				result = ">";
				break;
			case HierarchySearchFilterOperator.GreaterOrEqual:
				result = ">=";
				break;
			case HierarchySearchFilterOperator.Lesser:
				result = "<";
				break;
			case HierarchySearchFilterOperator.LesserOrEqual:
				result = "<=";
				break;
			case HierarchySearchFilterOperator.NotEqual:
				result = "!=";
				break;
			case HierarchySearchFilterOperator.Not:
				result = "-";
				break;
			default:
				throw new NotImplementedException(string.Format("Cannot convert {0} to string", op));
			}
			return result;
		}

		public static HierarchySearchFilterOperator ToOp(string op)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(op);
			if (num <= 957132539U)
			{
				if (num <= 671913016U)
				{
					if (num != 284975636U)
					{
						if (num == 671913016U)
						{
							if (op == "-")
							{
								return HierarchySearchFilterOperator.Not;
							}
						}
					}
					else if (op == ">=")
					{
						return HierarchySearchFilterOperator.GreaterOrEqual;
					}
				}
				else if (num != 940354920U)
				{
					if (num == 957132539U)
					{
						if (op == "<")
						{
							return HierarchySearchFilterOperator.Lesser;
						}
					}
				}
				else if (op == "=")
				{
					return HierarchySearchFilterOperator.Equal;
				}
			}
			else if (num <= 1057798253U)
			{
				if (num != 990687777U)
				{
					if (num == 1057798253U)
					{
						if (op == ":")
						{
							return HierarchySearchFilterOperator.Contains;
						}
					}
				}
				else if (op == ">")
				{
					return HierarchySearchFilterOperator.Greater;
				}
			}
			else if (num != 2428715011U)
			{
				if (num == 2499223986U)
				{
					if (op == "<=")
					{
						return HierarchySearchFilterOperator.LesserOrEqual;
					}
				}
			}
			else if (op == "!=")
			{
				return HierarchySearchFilterOperator.NotEqual;
			}
			throw new NotImplementedException("Cannot convert " + op + " to SearchFilterOperator");
		}

		public override string ToString()
		{
			string s = float.IsNaN(this.NumValue) ? this.Value : this.NumValue.ToString();
			return this.Name + HierarchySearchFilter.ToString(this.Op) + HierarchySearchFilter.QuoteStringIfNeeded(s);
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.HierarchyModule"
		})]
		internal static HierarchySearchFilter CreateFilter(string name, string op, string value)
		{
			return HierarchySearchFilter.CreateFilter(name, HierarchySearchFilter.ToOp(op), value);
		}

		internal static HierarchySearchFilter CreateFilter(string name, HierarchySearchFilterOperator op, string str)
		{
			string value = str;
			float numValue = float.NaN;
			try
			{
				numValue = Convert.ToSingle(str);
				value = null;
			}
			catch (Exception)
			{
			}
			return new HierarchySearchFilter
			{
				Name = name,
				Op = op,
				Value = value,
				NumValue = numValue
			};
		}

		internal static string QuoteStringIfNeeded(string s)
		{
			bool flag = s.Length > 0 && s.IndexOfAny(HierarchySearchFilter.s_WhiteSpaces) != -1 && s[0] != '"';
			string result;
			if (flag)
			{
				result = "\"" + s + "\"";
			}
			else
			{
				result = s;
			}
			return result;
		}

		private static readonly char[] s_WhiteSpaces = new char[]
		{
			' ',
			'\t',
			'\n'
		};

		private static readonly HierarchySearchFilter s_Invalid;
	}
}
