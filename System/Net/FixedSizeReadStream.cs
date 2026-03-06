using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net
{
	internal class FixedSizeReadStream : WebReadStream
	{
		public long ContentLength { get; }

		public FixedSizeReadStream(WebOperation operation, Stream innerStream, long contentLength) : base(operation, innerStream)
		{
			this.ContentLength = contentLength;
		}

		protected override Task<int> ProcessReadAsync(byte[] buffer, int offset, int size, CancellationToken cancellationToken)
		{
			FixedSizeReadStream.<ProcessReadAsync>d__5 <ProcessReadAsync>d__;
			<ProcessReadAsync>d__.<>4__this = this;
			<ProcessReadAsync>d__.buffer = buffer;
			<ProcessReadAsync>d__.offset = offset;
			<ProcessReadAsync>d__.size = size;
			<ProcessReadAsync>d__.cancellationToken = cancellationToken;
			<ProcessReadAsync>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<ProcessReadAsync>d__.<>1__state = -1;
			<ProcessReadAsync>d__.<>t__builder.Start<FixedSizeReadStream.<ProcessReadAsync>d__5>(ref <ProcessReadAsync>d__);
			return <ProcessReadAsync>d__.<>t__builder.Task;
		}

		private long position;
	}
}
