using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Converters
{
	[NullableContext(2)]
	internal interface IXmlDocumentType : IXmlNode
	{
		[Nullable(1)]
		string Name { [NullableContext(1)] get; }

		string System { get; }

		string Public { get; }

		string InternalSubset { get; }
	}
}
