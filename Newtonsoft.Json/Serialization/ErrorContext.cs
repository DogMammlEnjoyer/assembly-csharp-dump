using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Serialization
{
	[NullableContext(1)]
	[Nullable(0)]
	public class ErrorContext
	{
		internal ErrorContext([Nullable(2)] object originalObject, [Nullable(2)] object member, string path, Exception error)
		{
			this.OriginalObject = originalObject;
			this.Member = member;
			this.Error = error;
			this.Path = path;
		}

		internal bool Traced { get; set; }

		public Exception Error { get; }

		[Nullable(2)]
		public object OriginalObject { [NullableContext(2)] get; }

		[Nullable(2)]
		public object Member { [NullableContext(2)] get; }

		public string Path { get; }

		public bool Handled { get; set; }
	}
}
