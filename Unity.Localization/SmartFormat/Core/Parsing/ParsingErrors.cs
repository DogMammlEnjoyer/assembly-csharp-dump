using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.Localization.SmartFormat.Core.Parsing
{
	public class ParsingErrors : Exception
	{
		public void Init(Format result)
		{
			this.result = result;
		}

		public void Clear()
		{
			this.Issues.Clear();
		}

		public List<ParsingErrors.ParsingIssue> Issues { get; } = new List<ParsingErrors.ParsingIssue>();

		public bool HasIssues
		{
			get
			{
				return this.Issues.Count > 0;
			}
		}

		public string MessageShort
		{
			get
			{
				return string.Format("The format string has {0} issue{1}: {2}", this.Issues.Count, (this.Issues.Count == 1) ? "" : "s", string.Join(", ", (from i in this.Issues
				select i.Issue).ToArray<string>()));
			}
		}

		public override string Message
		{
			get
			{
				string text = "";
				int num = 0;
				foreach (ParsingErrors.ParsingIssue parsingIssue in this.Issues)
				{
					text += new string('-', parsingIssue.Index - num);
					if (parsingIssue.Length > 0)
					{
						text += new string('^', Math.Max(parsingIssue.Length, 1));
						num = parsingIssue.Index + parsingIssue.Length;
					}
					else
					{
						text += "^";
						num = parsingIssue.Index + 1;
					}
				}
				string format = "The format string has {0} issue{1}:\n{2}\nIn: \"{3}\"\nAt:  {4} ";
				object[] array = new object[5];
				array[0] = this.Issues.Count;
				array[1] = ((this.Issues.Count == 1) ? "" : "s");
				array[2] = string.Join(", ", (from i in this.Issues
				select i.Issue).ToArray<string>());
				array[3] = this.result.baseString;
				array[4] = text;
				return string.Format(format, array);
			}
		}

		public void AddIssue(string issue, int startIndex, int endIndex)
		{
			this.Issues.Add(new ParsingErrors.ParsingIssue(issue, startIndex, endIndex - startIndex));
		}

		private Format result;

		public class ParsingIssue
		{
			public ParsingIssue(string issue, int index, int length)
			{
				this.Issue = issue;
				this.Index = index;
				this.Length = length;
			}

			public int Index { get; }

			public int Length { get; }

			public string Issue { get; }
		}
	}
}
