using System;

namespace Meta.Net.NativeWebSocket
{
	public enum WebSocketCloseCode
	{
		NotSet,
		Normal = 1000,
		Away,
		ProtocolError,
		UnsupportedData,
		Undefined,
		NoStatus,
		Abnormal,
		InvalidData,
		PolicyViolation,
		TooBig,
		MandatoryExtension,
		ServerError,
		TlsHandshakeFailure = 1015
	}
}
