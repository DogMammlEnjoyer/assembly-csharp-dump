using System;
using System.Collections.Generic;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.Core.Parsing;

namespace UnityEngine.Localization.SmartFormat.Core.Formatting
{
	public class FormattingInfo : IFormattingInfo, ISelectorInfo
	{
		public void Init(FormatDetails formatDetails, Format format, object currentValue)
		{
			this.Init(null, formatDetails, format, currentValue);
		}

		public void Init(FormattingInfo parent, FormatDetails formatDetails, Format format, object currentValue)
		{
			this.Parent = parent;
			this.CurrentValue = currentValue;
			this.Format = format;
			this.FormatDetails = formatDetails;
		}

		public void Init(FormattingInfo parent, FormatDetails formatDetails, Placeholder placeholder, object currentValue)
		{
			this.Parent = parent;
			this.FormatDetails = formatDetails;
			this.Placeholder = placeholder;
			this.Format = placeholder.Format;
			this.CurrentValue = currentValue;
		}

		public void ReleaseToPool()
		{
			this.Parent = null;
			this.FormatDetails = null;
			this.Placeholder = null;
			this.Format = null;
			this.CurrentValue = null;
			foreach (FormattingInfo toRelease in this.Children)
			{
				FormattingInfoPool.Release(toRelease);
			}
			this.Children.Clear();
		}

		public FormattingInfo Parent { get; private set; }

		public Selector Selector { get; set; }

		public FormatDetails FormatDetails { get; private set; }

		public object CurrentValue { get; set; }

		public Placeholder Placeholder { get; private set; }

		public int Alignment
		{
			get
			{
				return this.Placeholder.Alignment;
			}
		}

		public string FormatterOptions
		{
			get
			{
				return this.Placeholder.FormatterOptions;
			}
		}

		public Format Format { get; private set; }

		public List<FormattingInfo> Children { get; } = new List<FormattingInfo>();

		public void Write(string text)
		{
			this.FormatDetails.Output.Write(text, this);
		}

		public void Write(string text, int startIndex, int length)
		{
			this.FormatDetails.Output.Write(text, startIndex, length, this);
		}

		public void Write(Format format, object value)
		{
			FormattingInfo formattingInfo = this.CreateChild(format, value);
			this.FormatDetails.Formatter.Format(formattingInfo);
		}

		public FormattingException FormattingException(string issue, FormatItem problemItem = null, int startIndex = -1)
		{
			if (problemItem == null)
			{
				problemItem = this.Format;
			}
			if (startIndex == -1)
			{
				startIndex = problemItem.startIndex;
			}
			return new FormattingException(problemItem, issue, startIndex);
		}

		public string SelectorText
		{
			get
			{
				return this.Selector.RawText;
			}
		}

		public int SelectorIndex
		{
			get
			{
				return this.Selector.SelectorIndex;
			}
		}

		public string SelectorOperator
		{
			get
			{
				return this.Selector.Operator;
			}
		}

		public object Result { get; set; }

		private FormattingInfo CreateChild(Format format, object currentValue)
		{
			FormattingInfo formattingInfo = FormattingInfoPool.Get(this, this.FormatDetails, format, currentValue);
			this.Children.Add(formattingInfo);
			return formattingInfo;
		}

		public FormattingInfo CreateChild(Placeholder placeholder)
		{
			FormattingInfo formattingInfo = FormattingInfoPool.Get(this, this.FormatDetails, placeholder, this.CurrentValue);
			this.Children.Add(formattingInfo);
			return formattingInfo;
		}
	}
}
