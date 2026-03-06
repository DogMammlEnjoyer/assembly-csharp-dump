using System;

namespace System.Xml
{
	/// <summary>
	///   <see langword="delegate" /> for a callback method when closing the reader.</summary>
	/// <param name="reader">The <see cref="T:System.Xml.XmlDictionaryReader" /> that fires the <c>OnClose</c> event.</param>
	public delegate void OnXmlDictionaryReaderClose(XmlDictionaryReader reader);
}
