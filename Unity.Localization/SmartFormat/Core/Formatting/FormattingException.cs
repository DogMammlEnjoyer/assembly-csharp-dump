using System;
using UnityEngine.Localization.SmartFormat.Core.Parsing;

namespace UnityEngine.Localization.SmartFormat.Core.Formatting
{
	public class FormattingException : Exception
	{
		public FormattingException(FormatItem errorItem, Exception formatException, int index)
		{
			this.Format = errorItem.baseString;
			this.ErrorItem = errorItem;
			this.Issue = formatException.Message;
			this.Index = index;
		}

		public FormattingException(FormatItem errorItem, string issue, int index)
		{
			this.Format = errorItem.baseString;
			this.ErrorItem = errorItem;
			this.Issue = issue;
			this.Index = index;
		}

		public string Format { get; }

		public FormatItem ErrorItem { get; }

		public string Issue { get; }

		public int Index { get; }

		public override string Message
		{
			get
			{
				return string.Format("Error parsing format string: {0} at {1}\n{2}\n{3}", new object[]
				{
					this.Issue,
					this.Index,
					this.Format,
					new string('-', this.Index) + "^"
				});
			}
		}
	}
}
