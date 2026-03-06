using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.InputSystem.Utilities
{
	public struct NameAndParameters
	{
		public string name { readonly get; set; }

		public ReadOnlyArray<NamedValue> parameters { readonly get; set; }

		public override string ToString()
		{
			if (this.parameters.Count == 0)
			{
				return this.name;
			}
			string str = string.Join(",", (from x in this.parameters
			select x.ToString()).ToArray<string>());
			return this.name + "(" + str + ")";
		}

		public static IEnumerable<NameAndParameters> ParseMultiple(string text)
		{
			List<NameAndParameters> result = null;
			if (!NameAndParameters.ParseMultiple(text, ref result))
			{
				return Enumerable.Empty<NameAndParameters>();
			}
			return result;
		}

		internal static bool ParseMultiple(string text, ref List<NameAndParameters> list)
		{
			text = text.Trim();
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			if (list == null)
			{
				list = new List<NameAndParameters>();
			}
			else
			{
				list.Clear();
			}
			int i = 0;
			int length = text.Length;
			while (i < length)
			{
				list.Add(NameAndParameters.ParseNameAndParameters(text, ref i, false));
			}
			return true;
		}

		internal static string ParseName(string text)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}
			int num = 0;
			return NameAndParameters.ParseNameAndParameters(text, ref num, true).name;
		}

		public static NameAndParameters Parse(string text)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}
			int num = 0;
			return NameAndParameters.ParseNameAndParameters(text, ref num, false);
		}

		private static NameAndParameters ParseNameAndParameters(string text, ref int index, bool nameOnly = false)
		{
			int length = text.Length;
			while (index < length && char.IsWhiteSpace(text[index]))
			{
				index++;
			}
			int num = index;
			while (index < length)
			{
				char c = text[index];
				if (c == '(' || c == ","[0] || char.IsWhiteSpace(c))
				{
					break;
				}
				index++;
			}
			if (index - num == 0)
			{
				throw new ArgumentException(string.Format("Expecting name at position {0} in '{1}'", num, text), "text");
			}
			string name = text.Substring(num, index - num);
			if (nameOnly)
			{
				return new NameAndParameters
				{
					name = name
				};
			}
			while (index < length && char.IsWhiteSpace(text[index]))
			{
				index++;
			}
			NamedValue[] array = null;
			if (index < length && text[index] == '(')
			{
				index++;
				int num2 = text.IndexOf(')', index);
				if (num2 == -1)
				{
					throw new ArgumentException(string.Format("Expecting ')' after '(' at position {0} in '{1}'", index, text), "text");
				}
				array = NamedValue.ParseMultiple(text.Substring(index, num2 - index));
				index = num2 + 1;
			}
			if (index < length && (text[index] == ',' || text[index] == ';'))
			{
				index++;
			}
			return new NameAndParameters
			{
				name = name,
				parameters = new ReadOnlyArray<NamedValue>(array)
			};
		}
	}
}
