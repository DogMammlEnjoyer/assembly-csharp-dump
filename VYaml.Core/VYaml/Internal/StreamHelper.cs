using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace VYaml.Internal
{
	internal static class StreamHelper
	{
		[NullableContext(1)]
		[return: Nullable(new byte[]
		{
			0,
			1
		})]
		public static ValueTask<ReusableByteSequenceBuilder> ReadAsSequenceAsync(Stream stream, CancellationToken cancellation = default(CancellationToken))
		{
			StreamHelper.<ReadAsSequenceAsync>d__0 <ReadAsSequenceAsync>d__;
			<ReadAsSequenceAsync>d__.<>t__builder = AsyncValueTaskMethodBuilder<ReusableByteSequenceBuilder>.Create();
			<ReadAsSequenceAsync>d__.stream = stream;
			<ReadAsSequenceAsync>d__.cancellation = cancellation;
			<ReadAsSequenceAsync>d__.<>1__state = -1;
			<ReadAsSequenceAsync>d__.<>t__builder.Start<StreamHelper.<ReadAsSequenceAsync>d__0>(ref <ReadAsSequenceAsync>d__);
			return <ReadAsSequenceAsync>d__.<>t__builder.Task;
		}

		private static int NewArrayCapacity(int size)
		{
			int num = size * 2;
			if (num > 2147483591)
			{
				num = 2147483591;
			}
			return num;
		}

		private const int ArrayMexLength = 2147483591;
	}
}
