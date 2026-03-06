using System;

namespace System.Net
{
	/// <summary>Specifies the security protocols that are supported by the Schannel security package.</summary>
	[Flags]
	public enum SecurityProtocolType
	{
		/// <summary>Allows the operating system to choose the best protocol to use, and to block protocols that are not secure. Unless your app has a specific reason not to, you should use this value.</summary>
		SystemDefault = 0,
		/// <summary>Specifies the Secure Socket Layer (SSL) 3.0 security protocol. SSL 3.0 has been superseded by the Transport Layer Security (TLS) protocol and is provided for backward compatibility only.</summary>
		Ssl3 = 48,
		/// <summary>Specifies the Transport Layer Security (TLS) 1.0 security protocol. The TLS 1.0 protocol is defined in IETF RFC 2246.</summary>
		Tls = 192,
		/// <summary>Specifies the Transport Layer Security (TLS) 1.1 security protocol. The TLS 1.1 protocol is defined in IETF RFC 4346. On Windows systems, this value is supported starting with Windows 7.</summary>
		Tls11 = 768,
		/// <summary>Specifies the Transport Layer Security (TLS) 1.2 security protocol. The TLS 1.2 protocol is defined in IETF RFC 5246. On Windows systems, this value is supported starting with Windows 7.</summary>
		Tls12 = 3072,
		/// <summary>Specifies the TLS 1.3 security protocol. The TLS protocol is defined in IETF RFC 8446.</summary>
		Tls13 = 12288
	}
}
