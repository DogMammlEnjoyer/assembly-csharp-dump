using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ICSharpCode.SharpZipLib.Core
{
	public class NameFilter : IScanFilter
	{
		public NameFilter(string filter)
		{
			this.filter_ = filter;
			this.inclusions_ = new List<Regex>();
			this.exclusions_ = new List<Regex>();
			this.Compile();
		}

		public static bool IsValidExpression(string expression)
		{
			bool result = true;
			try
			{
				new Regex(expression, RegexOptions.IgnoreCase | RegexOptions.Singleline);
			}
			catch (ArgumentException)
			{
				result = false;
			}
			return result;
		}

		public static bool IsValidFilterExpression(string toTest)
		{
			bool result = true;
			try
			{
				if (toTest != null)
				{
					string[] array = NameFilter.SplitQuoted(toTest);
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i] != null && array[i].Length > 0)
						{
							string pattern;
							if (array[i][0] == '+')
							{
								pattern = array[i].Substring(1, array[i].Length - 1);
							}
							else if (array[i][0] == '-')
							{
								pattern = array[i].Substring(1, array[i].Length - 1);
							}
							else
							{
								pattern = array[i];
							}
							new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
						}
					}
				}
			}
			catch (ArgumentException)
			{
				result = false;
			}
			return result;
		}

		public static string[] SplitQuoted(string original)
		{
			char c = '\\';
			char[] array = new char[]
			{
				';'
			};
			List<string> list = new List<string>();
			if (!string.IsNullOrEmpty(original))
			{
				int i = -1;
				StringBuilder stringBuilder = new StringBuilder();
				while (i < original.Length)
				{
					i++;
					if (i >= original.Length)
					{
						list.Add(stringBuilder.ToString());
					}
					else if (original[i] == c)
					{
						i++;
						if (i >= original.Length)
						{
							throw new ArgumentException("Missing terminating escape character", "original");
						}
						if (Array.IndexOf<char>(array, original[i]) < 0)
						{
							stringBuilder.Append(c);
						}
						stringBuilder.Append(original[i]);
					}
					else if (Array.IndexOf<char>(array, original[i]) >= 0)
					{
						list.Add(stringBuilder.ToString());
						stringBuilder.Length = 0;
					}
					else
					{
						stringBuilder.Append(original[i]);
					}
				}
			}
			return list.ToArray();
		}

		public override string ToString()
		{
			return this.filter_;
		}

		public bool IsIncluded(string name)
		{
			bool result = false;
			if (this.inclusions_.Count == 0)
			{
				result = true;
			}
			else
			{
				using (List<Regex>.Enumerator enumerator = this.inclusions_.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.IsMatch(name))
						{
							result = true;
							break;
						}
					}
				}
			}
			return result;
		}

		public bool IsExcluded(string name)
		{
			bool result = false;
			using (List<Regex>.Enumerator enumerator = this.exclusions_.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsMatch(name))
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		public bool IsMatch(string name)
		{
			return this.IsIncluded(name) && !this.IsExcluded(name);
		}

		private void Compile()
		{
			if (this.filter_ == null)
			{
				return;
			}
			string[] array = NameFilter.SplitQuoted(this.filter_);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null && array[i].Length > 0)
				{
					bool flag = array[i][0] != '-';
					string pattern;
					if (array[i][0] == '+')
					{
						pattern = array[i].Substring(1, array[i].Length - 1);
					}
					else if (array[i][0] == '-')
					{
						pattern = array[i].Substring(1, array[i].Length - 1);
					}
					else
					{
						pattern = array[i];
					}
					if (flag)
					{
						this.inclusions_.Add(new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline));
					}
					else
					{
						this.exclusions_.Add(new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline));
					}
				}
			}
		}

		private string filter_;

		private List<Regex> inclusions_;

		private List<Regex> exclusions_;
	}
}
