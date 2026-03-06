using System;

namespace NexusSDK
{
	public static class SDKInitializer
	{
		public static string ApiKey { get; private set; }

		public static string ApiBaseUrl { get; private set; }

		public static void Init(string apiKey, string environment = "sandbox")
		{
			if (string.IsNullOrEmpty(apiKey))
			{
				throw new ArgumentException("API Key cannot be null or empty", "apiKey");
			}
			SDKInitializer.ApiKey = apiKey;
			if (string.Equals(environment, "production", StringComparison.OrdinalIgnoreCase))
			{
				SDKInitializer.ApiBaseUrl = "https://api.nexus.gg/v1";
				return;
			}
			SDKInitializer.ApiBaseUrl = "https://api.nexus-dev.gg/v1";
		}
	}
}
