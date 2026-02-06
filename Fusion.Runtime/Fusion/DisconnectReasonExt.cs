using System;
using Fusion.Protocol;

namespace Fusion
{
	internal static class DisconnectReasonExt
	{
		public static ShutdownReason ConvertToShutdownReason(DisconnectReason disconnectCause)
		{
			switch (disconnectCause)
			{
			case DisconnectReason.ServerLogic:
				return ShutdownReason.DisconnectedByPluginLogic;
			case DisconnectReason.IncompatibleConfiguration:
				return ShutdownReason.IncompatibleConfiguration;
			case DisconnectReason.ServerAlreadyInRoom:
				return ShutdownReason.ServerInRoom;
			}
			return ShutdownReason.Error;
		}
	}
}
