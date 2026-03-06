using System;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics
{
	/// <summary>Provides unescaped XML data for the logging of user-provided trace data.</summary>
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public class UnescapedXmlDiagnosticData
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.UnescapedXmlDiagnosticData" /> class by using the specified XML data string.</summary>
		/// <param name="xmlPayload">The XML data to be logged in the <see langword="UserData" /> node of the event schema.  </param>
		public UnescapedXmlDiagnosticData(string xmlPayload)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Gets or sets the unescaped XML data string.</summary>
		/// <returns>An unescaped XML string.</returns>
		public string UnescapedXml
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
			set
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}
	}
}
