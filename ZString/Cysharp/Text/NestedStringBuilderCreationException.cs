using System;
using System.Runtime.CompilerServices;

namespace Cysharp.Text
{
	[NullableContext(1)]
	[Nullable(0)]
	internal class NestedStringBuilderCreationException : InvalidOperationException
	{
		protected internal NestedStringBuilderCreationException(string typeName, string extraMessage = "") : base("A nested call with `notNested: true`, or Either You forgot to call " + typeName + ".Dispose() of  in the past." + extraMessage)
		{
		}

		protected internal NestedStringBuilderCreationException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
