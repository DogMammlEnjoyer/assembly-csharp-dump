using System;
using System.Collections.Generic;
using UnityEngine.Localization.SmartFormat.Core.Output;
using UnityEngine.Localization.SmartFormat.Core.Parsing;
using UnityEngine.Localization.SmartFormat.Core.Settings;

namespace UnityEngine.Localization.SmartFormat.Core.Formatting
{
	public class FormatDetails
	{
		public void Init(SmartFormatter formatter, Format originalFormat, IList<object> originalArgs, FormatCache formatCache, IFormatProvider provider, IOutput output)
		{
			this.Formatter = formatter;
			this.OriginalFormat = originalFormat;
			this.OriginalArgs = originalArgs;
			this.FormatCache = formatCache;
			this.Provider = provider;
			this.Output = output;
		}

		internal void Clear()
		{
			this.Formatter = null;
			this.OriginalFormat = null;
			this.OriginalArgs = null;
			this.FormatCache = null;
			this.Provider = null;
			this.Output = null;
			this.FormattingException = null;
		}

		public SmartFormatter Formatter { get; private set; }

		public Format OriginalFormat { get; private set; }

		public IList<object> OriginalArgs { get; private set; }

		public FormatCache FormatCache { get; private set; }

		public IFormatProvider Provider { get; private set; }

		public IOutput Output { get; private set; }

		public FormattingException FormattingException { get; set; }

		public SmartSettings Settings
		{
			get
			{
				return this.Formatter.Settings;
			}
		}
	}
}
