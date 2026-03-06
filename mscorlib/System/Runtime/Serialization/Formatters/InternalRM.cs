using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Serialization.Formatters
{
	/// <summary>Logs tracing messages when the .NET Framework serialization infrastructure is compiled.</summary>
	[SecurityCritical]
	[ComVisible(true)]
	public sealed class InternalRM
	{
		/// <summary>Prints SOAP trace messages.</summary>
		/// <param name="messages">An array of trace messages to print.</param>
		[Conditional("_LOGGING")]
		public static void InfoSoap(params object[] messages)
		{
		}

		/// <summary>Checks if SOAP tracing is enabled.</summary>
		/// <returns>
		///   <see langword="true" />, if tracing is enabled; otherwise, <see langword="false" />.</returns>
		public static bool SoapCheckEnabled()
		{
			return BCLDebug.CheckEnabled("SOAP");
		}
	}
}
