using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata
{
	/// <summary>Customizes SOAP generation and processing for a field. This class cannot be inherited.</summary>
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class SoapFieldAttribute : SoapAttribute
	{
		/// <summary>Gets or sets the order of the current field attribute.</summary>
		/// <returns>The order of the current field attribute.</returns>
		public int Order
		{
			get
			{
				return this._order;
			}
			set
			{
				this._order = value;
			}
		}

		/// <summary>Gets or sets the XML element name of the field contained in the <see cref="T:System.Runtime.Remoting.Metadata.SoapFieldAttribute" /> attribute.</summary>
		/// <returns>The XML element name of the field contained in this attribute.</returns>
		public string XmlElementName
		{
			get
			{
				return this._elementName;
			}
			set
			{
				this._isElement = (value != null);
				this._elementName = value;
			}
		}

		/// <summary>Returns a value indicating whether the current attribute contains interop XML element values.</summary>
		/// <returns>
		///   <see langword="true" /> if the current attribute contains interop XML element values; otherwise, <see langword="false" />.</returns>
		public bool IsInteropXmlElement()
		{
			return this._isElement;
		}

		internal override void SetReflectionObject(object reflectionObject)
		{
			FieldInfo fieldInfo = (FieldInfo)reflectionObject;
			if (this._elementName == null)
			{
				this._elementName = fieldInfo.Name;
			}
		}

		private int _order;

		private string _elementName;

		private bool _isElement;
	}
}
