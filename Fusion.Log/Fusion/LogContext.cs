using System;

namespace Fusion
{
	[Obsolete]
	public readonly struct LogContext
	{
		public LogContext(string prefix, object source)
		{
			this.Prefix = prefix;
			this.Source = source;
		}

		public readonly string Prefix;

		public readonly object Source;
	}
}
