using System;

namespace Fusion
{
	[Serializable]
	public struct LogSettings
	{
		public LogSettings(LogLevel level, TraceChannels traceChannels)
		{
			this.Level = level;
			this.TraceChannels = traceChannels;
		}

		public LogLevel Level;

		public TraceChannels TraceChannels;
	}
}
