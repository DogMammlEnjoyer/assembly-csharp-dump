using System;

namespace ExitGames.Client.Photon
{
	public enum StatusCode
	{
		Connect = 1024,
		Disconnect,
		Exception,
		ExceptionOnConnect = 1023,
		ServerAddressInvalid = 1050,
		DnsExceptionOnConnect,
		SecurityExceptionOnConnect = 1022,
		SendError = 1030,
		ExceptionOnReceive = 1039,
		TimeoutDisconnect,
		DisconnectByServerTimeout,
		DisconnectByServerUserLimit,
		DisconnectByServerLogic,
		DisconnectByServerReasonUnknown,
		EncryptionEstablished = 1048,
		EncryptionFailedToEstablish
	}
}
