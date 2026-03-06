using System;
using Unity;

namespace System.Xml.Schema
{
	/// <summary>Returns detailed information related to the <see langword="ValidationEventHandler" />.</summary>
	public class ValidationEventArgs : EventArgs
	{
		internal ValidationEventArgs(XmlSchemaException ex)
		{
			this.ex = ex;
			this.severity = XmlSeverityType.Error;
		}

		internal ValidationEventArgs(XmlSchemaException ex, XmlSeverityType severity)
		{
			this.ex = ex;
			this.severity = severity;
		}

		/// <summary>Gets the severity of the validation event.</summary>
		/// <returns>An <see cref="T:System.Xml.Schema.XmlSeverityType" /> value representing the severity of the validation event.</returns>
		public XmlSeverityType Severity
		{
			get
			{
				return this.severity;
			}
		}

		/// <summary>Gets the <see cref="T:System.Xml.Schema.XmlSchemaException" /> associated with the validation event.</summary>
		/// <returns>The <see langword="XmlSchemaException" /> associated with the validation event.</returns>
		public XmlSchemaException Exception
		{
			get
			{
				return this.ex;
			}
		}

		/// <summary>Gets the text description corresponding to the validation event.</summary>
		/// <returns>The text description.</returns>
		public string Message
		{
			get
			{
				return this.ex.Message;
			}
		}

		internal ValidationEventArgs()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private XmlSchemaException ex;

		private XmlSeverityType severity;
	}
}
