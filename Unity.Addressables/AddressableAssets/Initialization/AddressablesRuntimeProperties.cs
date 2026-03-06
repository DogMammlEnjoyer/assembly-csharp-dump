using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnityEngine.AddressableAssets.Initialization
{
	public static class AddressablesRuntimeProperties
	{
		private static Assembly[] GetAssemblies()
		{
			return AppDomain.CurrentDomain.GetAssemblies();
		}

		internal static int GetCachedValueCount()
		{
			return AddressablesRuntimeProperties.s_CachedValues.Count;
		}

		public static void SetPropertyValue(string name, string val)
		{
			AddressablesRuntimeProperties.s_CachedValues[name] = val;
		}

		public static void ClearCachedPropertyValues()
		{
			AddressablesRuntimeProperties.s_CachedValues.Clear();
		}

		public static string EvaluateProperty(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return string.Empty;
			}
			string result;
			if (AddressablesRuntimeProperties.s_CachedValues.TryGetValue(name, out result))
			{
				return result;
			}
			int num = name.LastIndexOf('.');
			if (num < 0)
			{
				return name;
			}
			string name2 = name.Substring(0, num);
			string name3 = name.Substring(num + 1);
			Assembly[] assemblies = AddressablesRuntimeProperties.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				Type type = assemblies[i].GetType(name2, false, false);
				if (!(type == null))
				{
					try
					{
						PropertyInfo property = type.GetProperty(name3, BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
						if (property != null)
						{
							object value = property.GetValue(null, null);
							if (value != null)
							{
								AddressablesRuntimeProperties.s_CachedValues.Add(name, value.ToString());
								return value.ToString();
							}
						}
						FieldInfo field = type.GetField(name3, BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
						if (field != null)
						{
							object value2 = field.GetValue(null);
							if (value2 != null)
							{
								AddressablesRuntimeProperties.s_CachedValues.Add(name, value2.ToString());
								return value2.ToString();
							}
						}
					}
					catch (Exception)
					{
					}
				}
			}
			return name;
		}

		public static string EvaluateString(string inputString)
		{
			if (string.IsNullOrEmpty(inputString))
			{
				return string.Empty;
			}
			if (!inputString.Contains('{', StringComparison.Ordinal))
			{
				return inputString;
			}
			return AddressablesRuntimeProperties.EvaluateStringInternal(inputString, '{', '}', new Func<string, string>(AddressablesRuntimeProperties.EvaluateProperty));
		}

		public static string EvaluateString(string inputString, char startDelimiter, char endDelimiter, Func<string, string> varFunc)
		{
			if (string.IsNullOrEmpty(inputString))
			{
				return string.Empty;
			}
			if (!inputString.Contains(startDelimiter, StringComparison.Ordinal))
			{
				return inputString;
			}
			return AddressablesRuntimeProperties.EvaluateStringInternal(inputString, startDelimiter, endDelimiter, varFunc);
		}

		private static string EvaluateStringInternal(string inputString, char startDelimiter, char endDelimiter, Func<string, string> varFunc)
		{
			string str = inputString;
			Stack<string> stack;
			Stack<int> stack2;
			if (!AddressablesRuntimeProperties.s_StaticStacksAreInUse)
			{
				stack = AddressablesRuntimeProperties.s_TokenStack;
				stack2 = AddressablesRuntimeProperties.s_TokenStartStack;
				AddressablesRuntimeProperties.s_StaticStacksAreInUse = true;
			}
			else
			{
				stack = new Stack<string>(32);
				stack2 = new Stack<int>(32);
			}
			stack.Push(inputString);
			int num = inputString.Length;
			char[] anyOf = new char[]
			{
				startDelimiter,
				endDelimiter
			};
			bool flag = startDelimiter == endDelimiter;
			int i = inputString.IndexOf(startDelimiter);
			int num2 = -2;
			while (i >= 0)
			{
				char c = inputString[i];
				if (c == startDelimiter && (!flag || stack2.Count == 0))
				{
					stack2.Push(i);
					i++;
				}
				else if (c == endDelimiter && stack2.Count > 0)
				{
					int num3 = stack2.Peek();
					string text = inputString.Substring(num3 + 1, i - num3 - 1);
					if (num <= i)
					{
						stack.Pop();
					}
					string text2;
					if (stack.Contains(text))
					{
						text2 = "#ERROR-CyclicToken#";
					}
					else
					{
						text2 = ((varFunc == null) ? string.Empty : varFunc(text));
						stack.Push(text);
					}
					i = stack2.Pop();
					num = i + text2.Length + 1;
					if (i > 0)
					{
						int num4 = i + text.Length + 2;
						if (num4 == inputString.Length)
						{
							inputString = inputString.Substring(0, i) + text2;
						}
						else
						{
							inputString = inputString.Substring(0, i) + text2 + inputString.Substring(num4);
						}
					}
					else
					{
						inputString = text2 + inputString.Substring(i + text.Length + 2);
					}
				}
				if (num2 == i)
				{
					return "#ERROR-" + str + " contains unmatched delimiters#";
				}
				num2 = i;
				i = inputString.IndexOfAny(anyOf, i);
			}
			stack.Clear();
			stack2.Clear();
			if (stack == AddressablesRuntimeProperties.s_TokenStack)
			{
				AddressablesRuntimeProperties.s_StaticStacksAreInUse = false;
			}
			return inputString;
		}

		private static Stack<string> s_TokenStack = new Stack<string>(32);

		private static Stack<int> s_TokenStartStack = new Stack<int>(32);

		private static bool s_StaticStacksAreInUse = false;

		private static Dictionary<string, string> s_CachedValues = new Dictionary<string, string>();
	}
}
