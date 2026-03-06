using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net
{
	internal class BufferedReadStream : WebReadStream
	{
		public BufferedReadStream(WebOperation operation, Stream innerStream, BufferOffsetSize readBuffer) : base(operation, innerStream)
		{
			this.readBuffer = readBuffer;
		}

		protected override Task<int> ProcessReadAsync(byte[] buffer, int offset, int size, CancellationToken cancellationToken)
		{
			BufferedReadStream.<ProcessReadAsync>d__2 <ProcessReadAsync>d__;
			<ProcessReadAsync>d__.<>4__this = this;
			<ProcessReadAsync>d__.buffer = buffer;
			<ProcessReadAsync>d__.offset = offset;
			<ProcessReadAsync>d__.size = size;
			<ProcessReadAsync>d__.cancellationToken = cancellationToken;
			<ProcessReadAsync>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<ProcessReadAsync>d__.<>1__state = -1;
			<ProcessReadAsync>d__.<>t__builder.Start<BufferedReadStream.<ProcessReadAsync>d__2>(ref <ProcessReadAsync>d__);
			return <ProcessReadAsync>d__.<>t__builder.Task;
		}

		internal bool TryReadFromBuffer(byte[] buffer, int offset, int size, out int result)
		{
			BufferOffsetSize bufferOffsetSize = this.readBuffer;
			int num = (bufferOffsetSize != null) ? bufferOffsetSize.Size : 0;
			if (num <= 0)
			{
				result = 0;
				return base.InnerStream == null;
			}
			int num2 = (num > size) ? size : num;
			Buffer.BlockCopy(this.readBuffer.Buffer, this.readBuffer.Offset, buffer, offset, num2);
			this.readBuffer.Offset += num2;
			this.readBuffer.Size -= num2;
			offset += num2;
			size -= num2;
			result = num2;
			return true;
		}

		private readonly BufferOffsetSize readBuffer;
	}
}
