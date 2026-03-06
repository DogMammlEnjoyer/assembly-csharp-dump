using System;

namespace UnityEngine.Localization.SmartFormat
{
	public class FormattingErrorEventArgs : EventArgs
	{
		internal FormattingErrorEventArgs(string rawText, int errorIndex, bool ignoreError)
		{
			this.Placeholder = rawText;
			this.ErrorIndex = errorIndex;
			this.IgnoreError = ignoreError;
		}

		public string Placeholder { get; internal set; }

		public int ErrorIndex { get; internal set; }

		public bool IgnoreError { get; internal set; }
	}
}
