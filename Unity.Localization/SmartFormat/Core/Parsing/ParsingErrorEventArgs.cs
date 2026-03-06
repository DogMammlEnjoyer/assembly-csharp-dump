using System;

namespace UnityEngine.Localization.SmartFormat.Core.Parsing
{
	public class ParsingErrorEventArgs : EventArgs
	{
		internal ParsingErrorEventArgs(ParsingErrors errors, bool throwsException)
		{
			this.Errors = errors;
			this.ThrowsException = throwsException;
		}

		public ParsingErrors Errors { get; internal set; }

		public bool ThrowsException { get; internal set; }
	}
}
