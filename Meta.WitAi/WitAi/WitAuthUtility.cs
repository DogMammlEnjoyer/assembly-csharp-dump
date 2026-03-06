using System;
using Meta.WitAi.Data.Configuration;

namespace Meta.WitAi
{
	public class WitAuthUtility
	{
		public static bool IsServerTokenValid()
		{
			return WitAuthUtility.tokenValidator.IsServerTokenValid(WitAuthUtility.ServerToken);
		}

		public static bool IsServerTokenValid(string token)
		{
			return WitAuthUtility.tokenValidator.IsServerTokenValid(token);
		}

		public static string GetAppServerToken(WitConfiguration configuration, string defaultValue = "")
		{
			return WitAuthUtility.GetAppServerToken(configuration.GetApplicationId(), defaultValue);
		}

		public static string GetAppServerToken(string appId, string defaultServerToken = "")
		{
			return "";
		}

		public static string GetAppId(string serverToken, string defaultAppID = "")
		{
			return "";
		}

		public static void SetAppServerToken(string appId, string token)
		{
		}

		public static string ServerToken
		{
			get
			{
				return "";
			}
		}

		private static string serverToken;

		public static WitAuthUtility.ITokenValidationProvider tokenValidator = new WitAuthUtility.DefaultTokenValidatorProvider();

		public const string SERVER_TOKEN_ID = "SharedServerToken";

		public class DefaultTokenValidatorProvider : WitAuthUtility.ITokenValidationProvider
		{
			public bool IsTokenValid(string appId, string token)
			{
				return this.IsServerTokenValid(token);
			}

			public bool IsServerTokenValid(string serverToken)
			{
				return serverToken != null && serverToken.Length == 32;
			}
		}

		public interface ITokenValidationProvider
		{
			bool IsTokenValid(string appId, string token);

			bool IsServerTokenValid(string serverToken);
		}
	}
}
