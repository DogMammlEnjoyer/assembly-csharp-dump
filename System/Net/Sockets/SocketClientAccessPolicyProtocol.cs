using System;
using System.ComponentModel;

namespace System.Net.Sockets
{
	/// <summary>Specifies the method to download a client access policy file.</summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
	public enum SocketClientAccessPolicyProtocol
	{
		/// <summary>The socket policy file is downloaded using a custom TCP protocol running on TCP port 943.</summary>
		Tcp,
		/// <summary>The socket policy file is downloaded using the HTTP protocol running on TCP port 943.</summary>
		Http
	}
}
