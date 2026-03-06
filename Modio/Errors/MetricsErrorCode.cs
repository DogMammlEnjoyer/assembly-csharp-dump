using System;

namespace Modio.Errors
{
	public enum MetricsErrorCode : long
	{
		NONE,
		UNKNOWN = -2147483648L,
		INVALID_METRICS_SECRET = -2147483551L
	}
}
