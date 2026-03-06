using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Unity.Hierarchy
{
	internal class DefaultHierarchySearchQueryParser : IHierarchySearchQueryParser
	{
		private static List<string> Tokenize(string s)
		{
			s = s.Trim();
			List<string> list = new List<string>();
			int num = 0;
			int i = 0;
			while (i < s.Length)
			{
				bool flag = char.IsWhiteSpace(s[i]);
				if (flag)
				{
					string item = s.Substring(num, i - num);
					list.Add(item);
					i++;
					while (i < s.Length && char.IsWhiteSpace(s[i]))
					{
						i++;
					}
					bool flag2 = i < s.Length;
					if (flag2)
					{
						num = i;
					}
				}
				else
				{
					bool flag3 = s[i] == '"';
					if (flag3)
					{
						i++;
						while (i < s.Length && s[i] != '"')
						{
							i++;
						}
						bool flag4 = i >= s.Length;
						if (flag4)
						{
							return null;
						}
						i++;
					}
					else
					{
						i++;
					}
				}
			}
			bool flag5 = i != num;
			if (flag5)
			{
				string item2 = s.Substring(num, i - num);
				list.Add(item2);
			}
			return list;
		}

		public HierarchySearchQueryDescriptor ParseQuery(string query)
		{
			bool flag = string.IsNullOrWhiteSpace(query);
			HierarchySearchQueryDescriptor result;
			if (flag)
			{
				result = HierarchySearchQueryDescriptor.Empty;
			}
			else
			{
				List<string> list = DefaultHierarchySearchQueryParser.Tokenize(query);
				bool flag2 = list == null;
				if (flag2)
				{
					result = HierarchySearchQueryDescriptor.InvalidQuery;
				}
				else
				{
					List<string> list2 = new List<string>();
					List<HierarchySearchFilter> list3 = new List<HierarchySearchFilter>();
					bool flag3 = true;
					foreach (string text in list)
					{
						Match match = DefaultHierarchySearchQueryParser.s_Filter.Match(text);
						bool success = match.Success;
						if (success)
						{
							bool flag4 = match.Groups.Count < 4 || string.IsNullOrEmpty(match.Groups[1].Value) || string.IsNullOrEmpty(match.Groups[2].Value) || string.IsNullOrEmpty(match.Groups[3].Value);
							if (flag4)
							{
								flag3 = false;
								break;
							}
							list3.Add(HierarchySearchFilter.CreateFilter(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value));
						}
						else
						{
							list2.Add(text);
						}
					}
					bool flag5 = !flag3;
					if (flag5)
					{
						result = HierarchySearchQueryDescriptor.InvalidQuery;
					}
					else
					{
						result = new HierarchySearchQueryDescriptor(list3.ToArray(), list2.ToArray());
					}
				}
			}
			return result;
		}

		private static readonly Regex s_Filter = new Regex("([#$\\w\\[\\]]+)(<=|<|>=|>|<|=|:)(.*)", RegexOptions.Compiled);
	}
}
