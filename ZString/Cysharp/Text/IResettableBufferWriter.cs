using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Cysharp.Text
{
	[NullableContext(2)]
	public interface IResettableBufferWriter<T> : IBufferWriter<T>
	{
		void Reset();
	}
}
