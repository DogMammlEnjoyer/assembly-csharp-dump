using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace System.Xml
{
	internal abstract class Base64Encoder
	{
		internal Base64Encoder()
		{
			this.charsLine = new char[76];
		}

		internal abstract void WriteChars(char[] chars, int index, int count);

		internal void Encode(byte[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (count > buffer.Length - index)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (this.leftOverBytesCount > 0)
			{
				int num = this.leftOverBytesCount;
				while (num < 3 && count > 0)
				{
					this.leftOverBytes[num++] = buffer[index++];
					count--;
				}
				if (count == 0 && num < 3)
				{
					this.leftOverBytesCount = num;
					return;
				}
				int count2 = Convert.ToBase64CharArray(this.leftOverBytes, 0, 3, this.charsLine, 0);
				this.WriteChars(this.charsLine, 0, count2);
			}
			this.leftOverBytesCount = count % 3;
			if (this.leftOverBytesCount > 0)
			{
				count -= this.leftOverBytesCount;
				if (this.leftOverBytes == null)
				{
					this.leftOverBytes = new byte[3];
				}
				for (int i = 0; i < this.leftOverBytesCount; i++)
				{
					this.leftOverBytes[i] = buffer[index + count + i];
				}
			}
			int num2 = index + count;
			int num3 = 57;
			while (index < num2)
			{
				if (index + num3 > num2)
				{
					num3 = num2 - index;
				}
				int count3 = Convert.ToBase64CharArray(buffer, index, num3, this.charsLine, 0);
				this.WriteChars(this.charsLine, 0, count3);
				index += num3;
			}
		}

		internal void Flush()
		{
			if (this.leftOverBytesCount > 0)
			{
				int count = Convert.ToBase64CharArray(this.leftOverBytes, 0, this.leftOverBytesCount, this.charsLine, 0);
				this.WriteChars(this.charsLine, 0, count);
				this.leftOverBytesCount = 0;
			}
		}

		internal abstract Task WriteCharsAsync(char[] chars, int index, int count);

		internal Task EncodeAsync(byte[] buffer, int index, int count)
		{
			Base64Encoder.<EncodeAsync>d__10 <EncodeAsync>d__;
			<EncodeAsync>d__.<>4__this = this;
			<EncodeAsync>d__.buffer = buffer;
			<EncodeAsync>d__.index = index;
			<EncodeAsync>d__.count = count;
			<EncodeAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<EncodeAsync>d__.<>1__state = -1;
			<EncodeAsync>d__.<>t__builder.Start<Base64Encoder.<EncodeAsync>d__10>(ref <EncodeAsync>d__);
			return <EncodeAsync>d__.<>t__builder.Task;
		}

		internal Task FlushAsync()
		{
			Base64Encoder.<FlushAsync>d__11 <FlushAsync>d__;
			<FlushAsync>d__.<>4__this = this;
			<FlushAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<FlushAsync>d__.<>1__state = -1;
			<FlushAsync>d__.<>t__builder.Start<Base64Encoder.<FlushAsync>d__11>(ref <FlushAsync>d__);
			return <FlushAsync>d__.<>t__builder.Task;
		}

		private byte[] leftOverBytes;

		private int leftOverBytesCount;

		private char[] charsLine;

		internal const int Base64LineSize = 76;

		internal const int LineSizeInBytes = 57;
	}
}
