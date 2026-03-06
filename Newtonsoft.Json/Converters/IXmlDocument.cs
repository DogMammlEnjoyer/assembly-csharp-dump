using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Converters
{
	[NullableContext(1)]
	internal interface IXmlDocument : IXmlNode
	{
		IXmlNode CreateComment([Nullable(2)] string text);

		IXmlNode CreateTextNode([Nullable(2)] string text);

		IXmlNode CreateCDataSection([Nullable(2)] string data);

		IXmlNode CreateWhitespace([Nullable(2)] string text);

		IXmlNode CreateSignificantWhitespace([Nullable(2)] string text);

		IXmlNode CreateXmlDeclaration(string version, [Nullable(2)] string encoding, [Nullable(2)] string standalone);

		[NullableContext(2)]
		[return: Nullable(1)]
		IXmlNode CreateXmlDocumentType([Nullable(1)] string name, string publicId, string systemId, string internalSubset);

		IXmlNode CreateProcessingInstruction(string target, string data);

		IXmlElement CreateElement(string elementName);

		IXmlElement CreateElement(string qualifiedName, string namespaceUri);

		IXmlNode CreateAttribute(string name, string value);

		IXmlNode CreateAttribute(string qualifiedName, string namespaceUri, string value);

		[Nullable(2)]
		IXmlElement DocumentElement { [NullableContext(2)] get; }
	}
}
