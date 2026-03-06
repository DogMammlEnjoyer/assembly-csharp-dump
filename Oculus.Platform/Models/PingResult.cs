using System;

namespace Oculus.Platform.Models
{
	public class PingResult
	{
		public PingResult(ulong id, ulong? pingTimeUsec)
		{
			this.ID = id;
			this.pingTimeUsec = pingTimeUsec;
		}

		public ulong ID { get; private set; }

		public ulong PingTimeUsec
		{
			get
			{
				if (this.pingTimeUsec == null)
				{
					return 0UL;
				}
				return this.pingTimeUsec.Value;
			}
		}

		public bool IsTimeout
		{
			get
			{
				return this.pingTimeUsec == null;
			}
		}

		private ulong? pingTimeUsec;
	}
}
