using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Utilities
{
	[NullableContext(2)]
	[Nullable(0)]
	internal class FSharpFunction
	{
		public FSharpFunction(object instance, [Nullable(new byte[]
		{
			1,
			2,
			1
		})] MethodCall<object, object> invoker)
		{
			this._instance = instance;
			this._invoker = invoker;
		}

		[NullableContext(1)]
		public object Invoke(params object[] args)
		{
			return this._invoker(this._instance, args);
		}

		private readonly object _instance;

		[Nullable(new byte[]
		{
			1,
			2,
			1
		})]
		private readonly MethodCall<object, object> _invoker;
	}
}
