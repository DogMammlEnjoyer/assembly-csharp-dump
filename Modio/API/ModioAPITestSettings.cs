using System;
using System.Text.RegularExpressions;

namespace Modio.API
{
	public class ModioAPITestSettings : IModioServiceSettings
	{
		public bool ShouldFakeDisconnected(string url)
		{
			return this.FakeDisconnected || (!string.IsNullOrEmpty(this.FakeDisconnectedOnEndpointRegex) && Regex.IsMatch(url, this.FakeDisconnectedOnEndpointRegex));
		}

		public bool ShouldFakeRateLimit(string url)
		{
			return this.RateLimitError || (!string.IsNullOrEmpty(this.RateLimitOnEndpointRegex) && Regex.IsMatch(url, this.RateLimitOnEndpointRegex));
		}

		public bool FakeDisconnected;

		public string FakeDisconnectedOnEndpointRegex;

		public float FakeDisconnectedTimeoutDuration;

		public bool RateLimitError;

		public string RateLimitOnEndpointRegex;
	}
}
