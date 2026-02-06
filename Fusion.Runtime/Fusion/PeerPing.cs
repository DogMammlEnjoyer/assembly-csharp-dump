using System;
using Fusion.Protocol;

namespace Fusion
{
	internal class PeerPing
	{
		public PeerPing(ReflexiveInfo reflexiveInfo)
		{
			this.ReflexiveInfo = reflexiveInfo;
		}

		public override string ToString()
		{
			return string.Format("[{0}: {1}={2}, {3}={4}]", new object[]
			{
				"PeerPing",
				"AttemptCount",
				this.AttemptCount,
				"ReflexiveInfo",
				this.ReflexiveInfo
			});
		}

		public const float PING_DELAY = 0.1f;

		public int AttemptCount = 10;

		public float NextAttemptCountDown = 0.1f;

		public ReflexiveInfo ReflexiveInfo = null;
	}
}
