using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Converters
{
	[NullableContext(2)]
	internal interface IXmlDeclaration : IXmlNode
	{
		string Version { get; }

		string Encoding { get; set; }

		string Standalone { get; set; }
	}
}
