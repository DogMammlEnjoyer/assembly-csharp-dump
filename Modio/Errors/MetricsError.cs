using System;

namespace Modio.Errors
{
	public class MetricsError : Error
	{
		public new MetricsErrorCode Code
		{
			get
			{
				return (MetricsErrorCode)this.Code;
			}
		}

		public MetricsError(MetricsErrorCode code) : base((ErrorCode)code)
		{
		}

		public new static readonly MetricsError None = new MetricsError(MetricsErrorCode.NONE);
	}
}
