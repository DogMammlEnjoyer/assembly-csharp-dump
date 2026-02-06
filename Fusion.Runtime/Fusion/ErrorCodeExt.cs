using System;

namespace Fusion
{
	internal static class ErrorCodeExt
	{
		public static ShutdownReason ConvertToShutdownReason(short errorCode)
		{
			ShutdownReason result;
			if (errorCode != 0)
			{
				switch (errorCode)
				{
				case 32751:
				{
					DebugLogStream logDebug = InternalLogStreams.LogDebug;
					if (logDebug != null)
					{
						logDebug.Error("Fusion plug-in not found. Make sure to use a Fusion-type Photon Application ID.");
					}
					return ShutdownReason.IncompatibleConfiguration;
				}
				case 32752:
					return ShutdownReason.Error;
				case 32753:
					return ShutdownReason.AuthenticationTicketExpired;
				case 32755:
					return ShutdownReason.CustomAuthenticationFailed;
				case 32756:
					return ShutdownReason.InvalidRegion;
				case 32757:
					return ShutdownReason.MaxCcuReached;
				case 32758:
				case 32760:
					return ShutdownReason.GameNotFound;
				case 32762:
				{
					DebugLogStream logDebug2 = InternalLogStreams.LogDebug;
					if (logDebug2 != null)
					{
						logDebug2.Error("All servers are busy. This is a temporary issue and the game logic should try again after a brief wait time.");
					}
					return ShutdownReason.Error;
				}
				case 32764:
					return ShutdownReason.GameClosed;
				case 32765:
					return ShutdownReason.GameIsFull;
				case 32766:
					return ShutdownReason.GameIdAlreadyExists;
				case 32767:
					return ShutdownReason.InvalidAuthentication;
				}
				result = ShutdownReason.Error;
			}
			else
			{
				result = ShutdownReason.Ok;
			}
			return result;
		}
	}
}
