using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Converters
{
	[NullableContext(1)]
	internal interface IXmlElement : IXmlNode
	{
		void SetAttributeNode(IXmlNode attribute);

		[return: Nullable(2)]
		string GetPrefixOfNamespace(string namespaceUri);

		bool IsEmpty { get; }
	}
}
