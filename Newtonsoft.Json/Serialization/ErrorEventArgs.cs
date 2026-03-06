using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Serialization
{
	[NullableContext(1)]
	[Nullable(0)]
	public class ErrorEventArgs : EventArgs
	{
		[Nullable(2)]
		public object CurrentObject { [NullableContext(2)] get; }

		public ErrorContext ErrorContext { get; }

		public ErrorEventArgs([Nullable(2)] object currentObject, ErrorContext errorContext)
		{
			this.CurrentObject = currentObject;
			this.ErrorContext = errorContext;
		}
	}
}
