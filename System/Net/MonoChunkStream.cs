using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net
{
	internal class MonoChunkStream : WebReadStream
	{
		protected WebHeaderCollection Headers { get; }

		protected MonoChunkParser Decoder { get; }

		public MonoChunkStream(WebOperation operation, Stream innerStream, WebHeaderCollection headers) : base(operation, innerStream)
		{
			this.Headers = headers;
			this.Decoder = new MonoChunkParser(headers);
		}

		protected override Task<int> ProcessReadAsync(byte[] buffer, int offset, int size, CancellationToken cancellationToken)
		{
			MonoChunkStream.<ProcessReadAsync>d__7 <ProcessReadAsync>d__;
			<ProcessReadAsync>d__.<>4__this = this;
			<ProcessReadAsync>d__.buffer = buffer;
			<ProcessReadAsync>d__.offset = offset;
			<ProcessReadAsync>d__.size = size;
			<ProcessReadAsync>d__.cancellationToken = cancellationToken;
			<ProcessReadAsync>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<ProcessReadAsync>d__.<>1__state = -1;
			<ProcessReadAsync>d__.<>t__builder.Start<MonoChunkStream.<ProcessReadAsync>d__7>(ref <ProcessReadAsync>d__);
			return <ProcessReadAsync>d__.<>t__builder.Task;
		}

		internal override Task FinishReading(CancellationToken cancellationToken)
		{
			MonoChunkStream.<FinishReading>d__8 <FinishReading>d__;
			<FinishReading>d__.<>4__this = this;
			<FinishReading>d__.cancellationToken = cancellationToken;
			<FinishReading>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<FinishReading>d__.<>1__state = -1;
			<FinishReading>d__.<>t__builder.Start<MonoChunkStream.<FinishReading>d__8>(ref <FinishReading>d__);
			return <FinishReading>d__.<>t__builder.Task;
		}

		private static void ThrowExpectingChunkTrailer()
		{
			throw new WebException("Expecting chunk trailer.", null, WebExceptionStatus.ServerProtocolViolation, null);
		}
	}
}
