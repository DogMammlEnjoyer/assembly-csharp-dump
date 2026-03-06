using System;
using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.SharpZipLib.Tar
{
	public class TarExtendedHeaderReader
	{
		public TarExtendedHeaderReader()
		{
			this.ResetBuffers();
		}

		public void Read(byte[] buffer, int length)
		{
			for (int i = 0; i < length; i++)
			{
				byte b = buffer[i];
				if (b == TarExtendedHeaderReader.StateNext[this.state])
				{
					this.Flush();
					this.headerParts[this.state] = this.sb.ToString();
					this.sb.Clear();
					int num = this.state + 1;
					this.state = num;
					if (num == 3)
					{
						this.headers.Add(this.headerParts[1], this.headerParts[2]);
						this.headerParts = new string[3];
						this.state = 0;
					}
				}
				else
				{
					byte[] array = this.byteBuffer;
					int num = this.bbIndex;
					this.bbIndex = num + 1;
					array[num] = b;
					if (this.bbIndex == 4)
					{
						this.Flush();
					}
				}
			}
		}

		private void Flush()
		{
			int num;
			int charCount;
			bool flag;
			this.decoder.Convert(this.byteBuffer, 0, this.bbIndex, this.charBuffer, 0, 4, false, out num, out charCount, out flag);
			this.sb.Append(this.charBuffer, 0, charCount);
			this.ResetBuffers();
		}

		private void ResetBuffers()
		{
			this.charBuffer = new char[4];
			this.byteBuffer = new byte[4];
			this.bbIndex = 0;
		}

		public Dictionary<string, string> Headers
		{
			get
			{
				return this.headers;
			}
		}

		private const byte LENGTH = 0;

		private const byte KEY = 1;

		private const byte VALUE = 2;

		private const byte END = 3;

		private readonly Dictionary<string, string> headers = new Dictionary<string, string>();

		private string[] headerParts = new string[3];

		private int bbIndex;

		private byte[] byteBuffer;

		private char[] charBuffer;

		private readonly StringBuilder sb = new StringBuilder();

		private readonly Decoder decoder = Encoding.UTF8.GetDecoder();

		private int state;

		private static readonly byte[] StateNext = new byte[]
		{
			32,
			61,
			10
		};
	}
}
