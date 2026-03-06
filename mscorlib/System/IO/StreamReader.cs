using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO
{
	/// <summary>Implements a <see cref="T:System.IO.TextReader" /> that reads characters from a byte stream in a particular encoding.</summary>
	[Serializable]
	public class StreamReader : TextReader
	{
		private void CheckAsyncTaskInProgress()
		{
			if (!this._asyncReadTask.IsCompleted)
			{
				StreamReader.ThrowAsyncIOInProgress();
			}
		}

		private static void ThrowAsyncIOInProgress()
		{
			throw new InvalidOperationException("The stream is currently in use by a previous operation on the stream.");
		}

		internal StreamReader()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.StreamReader" /> class for the specified stream.</summary>
		/// <param name="stream">The stream to be read.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="stream" /> does not support reading.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="stream" /> is <see langword="null" />.</exception>
		public StreamReader(Stream stream) : this(stream, true)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.StreamReader" /> class for the specified stream, with the specified byte order mark detection option.</summary>
		/// <param name="stream">The stream to be read.</param>
		/// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the beginning of the file.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="stream" /> does not support reading.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="stream" /> is <see langword="null" />.</exception>
		public StreamReader(Stream stream, bool detectEncodingFromByteOrderMarks) : this(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks, 1024, false)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.StreamReader" /> class for the specified stream, with the specified character encoding.</summary>
		/// <param name="stream">The stream to be read.</param>
		/// <param name="encoding">The character encoding to use.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="stream" /> does not support reading.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="stream" /> or <paramref name="encoding" /> is <see langword="null" />.</exception>
		public StreamReader(Stream stream, Encoding encoding) : this(stream, encoding, true, 1024, false)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.StreamReader" /> class for the specified stream, with the specified character encoding and byte order mark detection option.</summary>
		/// <param name="stream">The stream to be read.</param>
		/// <param name="encoding">The character encoding to use.</param>
		/// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the beginning of the file.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="stream" /> does not support reading.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="stream" /> or <paramref name="encoding" /> is <see langword="null" />.</exception>
		public StreamReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks) : this(stream, encoding, detectEncodingFromByteOrderMarks, 1024, false)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.StreamReader" /> class for the specified stream, with the specified character encoding, byte order mark detection option, and buffer size.</summary>
		/// <param name="stream">The stream to be read.</param>
		/// <param name="encoding">The character encoding to use.</param>
		/// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the beginning of the file.</param>
		/// <param name="bufferSize">The minimum buffer size.</param>
		/// <exception cref="T:System.ArgumentException">The stream does not support reading.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="stream" /> or <paramref name="encoding" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="bufferSize" /> is less than or equal to zero.</exception>
		public StreamReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize) : this(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize, false)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.StreamReader" /> class for the specified stream based on the specified character encoding, byte order mark detection option, and buffer size, and optionally leaves the stream open.</summary>
		/// <param name="stream">The stream to read.</param>
		/// <param name="encoding">The character encoding to use.</param>
		/// <param name="detectEncodingFromByteOrderMarks">
		///   <see langword="true" /> to look for byte order marks at the beginning of the file; otherwise, <see langword="false" />.</param>
		/// <param name="bufferSize">The minimum buffer size.</param>
		/// <param name="leaveOpen">
		///   <see langword="true" /> to leave the stream open after the <see cref="T:System.IO.StreamReader" /> object is disposed; otherwise, <see langword="false" />.</param>
		public StreamReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize, bool leaveOpen)
		{
			if (stream == null || encoding == null)
			{
				throw new ArgumentNullException((stream == null) ? "stream" : "encoding");
			}
			if (!stream.CanRead)
			{
				throw new ArgumentException("Stream was not readable.");
			}
			if (bufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("bufferSize", "Positive number required.");
			}
			this.Init(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize, leaveOpen);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.StreamReader" /> class for the specified file name.</summary>
		/// <param name="path">The complete file path to be read.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="path" /> is an empty string ("").</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="path" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">The file cannot be found.</exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
		/// <exception cref="T:System.IO.IOException">
		///   <paramref name="path" /> includes an incorrect or invalid syntax for file name, directory name, or volume label.</exception>
		public StreamReader(string path) : this(path, true)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.StreamReader" /> class for the specified file name, with the specified byte order mark detection option.</summary>
		/// <param name="path">The complete file path to be read.</param>
		/// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the beginning of the file.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="path" /> is an empty string ("").</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="path" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">The file cannot be found.</exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
		/// <exception cref="T:System.IO.IOException">
		///   <paramref name="path" /> includes an incorrect or invalid syntax for file name, directory name, or volume label.</exception>
		public StreamReader(string path, bool detectEncodingFromByteOrderMarks) : this(path, Encoding.UTF8, detectEncodingFromByteOrderMarks, 1024)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.StreamReader" /> class for the specified file name, with the specified character encoding.</summary>
		/// <param name="path">The complete file path to be read.</param>
		/// <param name="encoding">The character encoding to use.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="path" /> is an empty string ("").</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="path" /> or <paramref name="encoding" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">The file cannot be found.</exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="path" /> includes an incorrect or invalid syntax for file name, directory name, or volume label.</exception>
		public StreamReader(string path, Encoding encoding) : this(path, encoding, true, 1024)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.StreamReader" /> class for the specified file name, with the specified character encoding and byte order mark detection option.</summary>
		/// <param name="path">The complete file path to be read.</param>
		/// <param name="encoding">The character encoding to use.</param>
		/// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the beginning of the file.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="path" /> is an empty string ("").</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="path" /> or <paramref name="encoding" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">The file cannot be found.</exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="path" /> includes an incorrect or invalid syntax for file name, directory name, or volume label.</exception>
		public StreamReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks) : this(path, encoding, detectEncodingFromByteOrderMarks, 1024)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.StreamReader" /> class for the specified file name, with the specified character encoding, byte order mark detection option, and buffer size.</summary>
		/// <param name="path">The complete file path to be read.</param>
		/// <param name="encoding">The character encoding to use.</param>
		/// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the beginning of the file.</param>
		/// <param name="bufferSize">The minimum buffer size, in number of 16-bit characters.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="path" /> is an empty string ("").</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="path" /> or <paramref name="encoding" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">The file cannot be found.</exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="path" /> includes an incorrect or invalid syntax for file name, directory name, or volume label.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="buffersize" /> is less than or equal to zero.</exception>
		public StreamReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException("Empty path name is not legal.");
			}
			if (bufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("bufferSize", "Positive number required.");
			}
			Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);
			this.Init(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize, false);
		}

		private void Init(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize, bool leaveOpen)
		{
			this._stream = stream;
			this._encoding = encoding;
			this._decoder = encoding.GetDecoder();
			if (bufferSize < 128)
			{
				bufferSize = 128;
			}
			this._byteBuffer = new byte[bufferSize];
			this._maxCharsPerBuffer = encoding.GetMaxCharCount(bufferSize);
			this._charBuffer = new char[this._maxCharsPerBuffer];
			this._byteLen = 0;
			this._bytePos = 0;
			this._detectEncoding = detectEncodingFromByteOrderMarks;
			this._checkPreamble = (encoding.Preamble.Length > 0);
			this._isBlocked = false;
			this._closable = !leaveOpen;
		}

		internal void Init(Stream stream)
		{
			this._stream = stream;
			this._closable = true;
		}

		/// <summary>Closes the <see cref="T:System.IO.StreamReader" /> object and the underlying stream, and releases any system resources associated with the reader.</summary>
		public override void Close()
		{
			this.Dispose(true);
		}

		/// <summary>Closes the underlying stream, releases the unmanaged resources used by the <see cref="T:System.IO.StreamReader" />, and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (!this.LeaveOpen && disposing && this._stream != null)
				{
					this._stream.Close();
				}
			}
			finally
			{
				if (!this.LeaveOpen && this._stream != null)
				{
					this._stream = null;
					this._encoding = null;
					this._decoder = null;
					this._byteBuffer = null;
					this._charBuffer = null;
					this._charPos = 0;
					this._charLen = 0;
					base.Dispose(disposing);
				}
			}
		}

		/// <summary>Gets the current character encoding that the current <see cref="T:System.IO.StreamReader" /> object is using.</summary>
		/// <returns>The current character encoding used by the current reader. The value can be different after the first call to any <see cref="Overload:System.IO.StreamReader.Read" /> method of <see cref="T:System.IO.StreamReader" />, since encoding autodetection is not done until the first call to a <see cref="Overload:System.IO.StreamReader.Read" /> method.</returns>
		public virtual Encoding CurrentEncoding
		{
			get
			{
				return this._encoding;
			}
		}

		/// <summary>Returns the underlying stream.</summary>
		/// <returns>The underlying stream.</returns>
		public virtual Stream BaseStream
		{
			get
			{
				return this._stream;
			}
		}

		internal bool LeaveOpen
		{
			get
			{
				return !this._closable;
			}
		}

		/// <summary>Clears the internal buffer.</summary>
		public void DiscardBufferedData()
		{
			this.CheckAsyncTaskInProgress();
			this._byteLen = 0;
			this._charLen = 0;
			this._charPos = 0;
			if (this._encoding != null)
			{
				this._decoder = this._encoding.GetDecoder();
			}
			this._isBlocked = false;
		}

		/// <summary>Gets a value that indicates whether the current stream position is at the end of the stream.</summary>
		/// <returns>
		///   <see langword="true" /> if the current stream position is at the end of the stream; otherwise <see langword="false" />.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The underlying stream has been disposed.</exception>
		public bool EndOfStream
		{
			get
			{
				if (this._stream == null)
				{
					throw new ObjectDisposedException(null, "Cannot read from a closed TextReader.");
				}
				this.CheckAsyncTaskInProgress();
				return this._charPos >= this._charLen && this.ReadBuffer() == 0;
			}
		}

		/// <summary>Returns the next available character but does not consume it.</summary>
		/// <returns>An integer representing the next character to be read, or -1 if there are no characters to be read or if the stream does not support seeking.</returns>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		public override int Peek()
		{
			if (this._stream == null)
			{
				throw new ObjectDisposedException(null, "Cannot read from a closed TextReader.");
			}
			this.CheckAsyncTaskInProgress();
			if (this._charPos == this._charLen && (this._isBlocked || this.ReadBuffer() == 0))
			{
				return -1;
			}
			return (int)this._charBuffer[this._charPos];
		}

		/// <summary>Reads the next character from the input stream and advances the character position by one character.</summary>
		/// <returns>The next character from the input stream represented as an <see cref="T:System.Int32" /> object, or -1 if no more characters are available.</returns>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		public override int Read()
		{
			if (this._stream == null)
			{
				throw new ObjectDisposedException(null, "Cannot read from a closed TextReader.");
			}
			this.CheckAsyncTaskInProgress();
			if (this._charPos == this._charLen && this.ReadBuffer() == 0)
			{
				return -1;
			}
			int result = (int)this._charBuffer[this._charPos];
			this._charPos++;
			return result;
		}

		/// <summary>Reads a specified maximum of characters from the current stream into a buffer, beginning at the specified index.</summary>
		/// <param name="buffer">When this method returns, contains the specified character array with the values between <paramref name="index" /> and (index + count - 1) replaced by the characters read from the current source.</param>
		/// <param name="index">The index of <paramref name="buffer" /> at which to begin writing.</param>
		/// <param name="count">The maximum number of characters to read.</param>
		/// <returns>The number of characters that have been read, or 0 if at the end of the stream and no data was read. The number will be less than or equal to the <paramref name="count" /> parameter, depending on whether the data is available within the stream.</returns>
		/// <exception cref="T:System.ArgumentException">The buffer length minus <paramref name="index" /> is less than <paramref name="count" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> or <paramref name="count" /> is negative.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs, such as the stream is closed.</exception>
		public override int Read(char[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer", "Buffer cannot be null.");
			}
			if (index < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException((index < 0) ? "index" : "count", "Non-negative number required.");
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			}
			return this.ReadSpan(new Span<char>(buffer, index, count));
		}

		public override int Read(Span<char> buffer)
		{
			if (!(base.GetType() == typeof(StreamReader)))
			{
				return base.Read(buffer);
			}
			return this.ReadSpan(buffer);
		}

		private int ReadSpan(Span<char> buffer)
		{
			if (this._stream == null)
			{
				throw new ObjectDisposedException(null, "Cannot read from a closed TextReader.");
			}
			this.CheckAsyncTaskInProgress();
			int num = 0;
			bool flag = false;
			int i = buffer.Length;
			while (i > 0)
			{
				int num2 = this._charLen - this._charPos;
				if (num2 == 0)
				{
					num2 = this.ReadBuffer(buffer.Slice(num), out flag);
				}
				if (num2 == 0)
				{
					break;
				}
				if (num2 > i)
				{
					num2 = i;
				}
				if (!flag)
				{
					new Span<char>(this._charBuffer, this._charPos, num2).CopyTo(buffer.Slice(num));
					this._charPos += num2;
				}
				num += num2;
				i -= num2;
				if (this._isBlocked)
				{
					break;
				}
			}
			return num;
		}

		/// <summary>Reads all characters from the current position to the end of the stream.</summary>
		/// <returns>The rest of the stream as a string, from the current position to the end. If the current position is at the end of the stream, returns an empty string ("").</returns>
		/// <exception cref="T:System.OutOfMemoryException">There is insufficient memory to allocate a buffer for the returned string.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		public override string ReadToEnd()
		{
			if (this._stream == null)
			{
				throw new ObjectDisposedException(null, "Cannot read from a closed TextReader.");
			}
			this.CheckAsyncTaskInProgress();
			StringBuilder stringBuilder = new StringBuilder(this._charLen - this._charPos);
			do
			{
				stringBuilder.Append(this._charBuffer, this._charPos, this._charLen - this._charPos);
				this._charPos = this._charLen;
				this.ReadBuffer();
			}
			while (this._charLen > 0);
			return stringBuilder.ToString();
		}

		/// <summary>Reads a specified maximum number of characters from the current stream and writes the data to a buffer, beginning at the specified index.</summary>
		/// <param name="buffer">When this method returns, contains the specified character array with the values between <paramref name="index" /> and (index + count - 1) replaced by the characters read from the current source.</param>
		/// <param name="index">The position in <paramref name="buffer" /> at which to begin writing.</param>
		/// <param name="count">The maximum number of characters to read.</param>
		/// <returns>The number of characters that have been read. The number will be less than or equal to <paramref name="count" />, depending on whether all input characters have been read.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The buffer length minus <paramref name="index" /> is less than <paramref name="count" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> or <paramref name="count" /> is negative.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.StreamReader" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurred.</exception>
		public override int ReadBlock(char[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer", "Buffer cannot be null.");
			}
			if (index < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException((index < 0) ? "index" : "count", "Non-negative number required.");
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			}
			if (this._stream == null)
			{
				throw new ObjectDisposedException(null, "Cannot read from a closed TextReader.");
			}
			this.CheckAsyncTaskInProgress();
			return base.ReadBlock(buffer, index, count);
		}

		public override int ReadBlock(Span<char> buffer)
		{
			if (base.GetType() != typeof(StreamReader))
			{
				return base.ReadBlock(buffer);
			}
			int num = 0;
			int num2;
			do
			{
				num2 = this.ReadSpan(buffer.Slice(num));
				num += num2;
			}
			while (num2 > 0 && num < buffer.Length);
			return num;
		}

		private void CompressBuffer(int n)
		{
			Buffer.BlockCopy(this._byteBuffer, n, this._byteBuffer, 0, this._byteLen - n);
			this._byteLen -= n;
		}

		private void DetectEncoding()
		{
			if (this._byteLen < 2)
			{
				return;
			}
			this._detectEncoding = false;
			bool flag = false;
			if (this._byteBuffer[0] == 254 && this._byteBuffer[1] == 255)
			{
				this._encoding = Encoding.BigEndianUnicode;
				this.CompressBuffer(2);
				flag = true;
			}
			else if (this._byteBuffer[0] == 255 && this._byteBuffer[1] == 254)
			{
				if (this._byteLen < 4 || this._byteBuffer[2] != 0 || this._byteBuffer[3] != 0)
				{
					this._encoding = Encoding.Unicode;
					this.CompressBuffer(2);
					flag = true;
				}
				else
				{
					this._encoding = Encoding.UTF32;
					this.CompressBuffer(4);
					flag = true;
				}
			}
			else if (this._byteLen >= 3 && this._byteBuffer[0] == 239 && this._byteBuffer[1] == 187 && this._byteBuffer[2] == 191)
			{
				this._encoding = Encoding.UTF8;
				this.CompressBuffer(3);
				flag = true;
			}
			else if (this._byteLen >= 4 && this._byteBuffer[0] == 0 && this._byteBuffer[1] == 0 && this._byteBuffer[2] == 254 && this._byteBuffer[3] == 255)
			{
				this._encoding = new UTF32Encoding(true, true);
				this.CompressBuffer(4);
				flag = true;
			}
			else if (this._byteLen == 2)
			{
				this._detectEncoding = true;
			}
			if (flag)
			{
				this._decoder = this._encoding.GetDecoder();
				int maxCharCount = this._encoding.GetMaxCharCount(this._byteBuffer.Length);
				if (maxCharCount > this._maxCharsPerBuffer)
				{
					this._charBuffer = new char[maxCharCount];
				}
				this._maxCharsPerBuffer = maxCharCount;
			}
		}

		private unsafe bool IsPreamble()
		{
			if (!this._checkPreamble)
			{
				return this._checkPreamble;
			}
			ReadOnlySpan<byte> preamble = this._encoding.Preamble;
			int num = (this._byteLen >= preamble.Length) ? (preamble.Length - this._bytePos) : (this._byteLen - this._bytePos);
			int i = 0;
			while (i < num)
			{
				if (this._byteBuffer[this._bytePos] != *preamble[this._bytePos])
				{
					this._bytePos = 0;
					this._checkPreamble = false;
					break;
				}
				i++;
				this._bytePos++;
			}
			if (this._checkPreamble && this._bytePos == preamble.Length)
			{
				this.CompressBuffer(preamble.Length);
				this._bytePos = 0;
				this._checkPreamble = false;
				this._detectEncoding = false;
			}
			return this._checkPreamble;
		}

		internal virtual int ReadBuffer()
		{
			this._charLen = 0;
			this._charPos = 0;
			if (!this._checkPreamble)
			{
				this._byteLen = 0;
			}
			for (;;)
			{
				if (this._checkPreamble)
				{
					int num = this._stream.Read(this._byteBuffer, this._bytePos, this._byteBuffer.Length - this._bytePos);
					if (num == 0)
					{
						break;
					}
					this._byteLen += num;
				}
				else
				{
					this._byteLen = this._stream.Read(this._byteBuffer, 0, this._byteBuffer.Length);
					if (this._byteLen == 0)
					{
						goto Block_5;
					}
				}
				this._isBlocked = (this._byteLen < this._byteBuffer.Length);
				if (!this.IsPreamble())
				{
					if (this._detectEncoding && this._byteLen >= 2)
					{
						this.DetectEncoding();
					}
					this._charLen += this._decoder.GetChars(this._byteBuffer, 0, this._byteLen, this._charBuffer, this._charLen);
				}
				if (this._charLen != 0)
				{
					goto Block_9;
				}
			}
			if (this._byteLen > 0)
			{
				this._charLen += this._decoder.GetChars(this._byteBuffer, 0, this._byteLen, this._charBuffer, this._charLen);
				this._bytePos = (this._byteLen = 0);
			}
			return this._charLen;
			Block_5:
			return this._charLen;
			Block_9:
			return this._charLen;
		}

		private int ReadBuffer(Span<char> userBuffer, out bool readToUserBuffer)
		{
			this._charLen = 0;
			this._charPos = 0;
			if (!this._checkPreamble)
			{
				this._byteLen = 0;
			}
			int num = 0;
			readToUserBuffer = (userBuffer.Length >= this._maxCharsPerBuffer);
			for (;;)
			{
				if (this._checkPreamble)
				{
					int num2 = this._stream.Read(this._byteBuffer, this._bytePos, this._byteBuffer.Length - this._bytePos);
					if (num2 == 0)
					{
						break;
					}
					this._byteLen += num2;
				}
				else
				{
					this._byteLen = this._stream.Read(this._byteBuffer, 0, this._byteBuffer.Length);
					if (this._byteLen == 0)
					{
						goto IL_1CD;
					}
				}
				this._isBlocked = (this._byteLen < this._byteBuffer.Length);
				if (!this.IsPreamble())
				{
					if (this._detectEncoding && this._byteLen >= 2)
					{
						this.DetectEncoding();
						readToUserBuffer = (userBuffer.Length >= this._maxCharsPerBuffer);
					}
					this._charPos = 0;
					if (readToUserBuffer)
					{
						num += this._decoder.GetChars(new ReadOnlySpan<byte>(this._byteBuffer, 0, this._byteLen), userBuffer.Slice(num), false);
						this._charLen = 0;
					}
					else
					{
						num = this._decoder.GetChars(this._byteBuffer, 0, this._byteLen, this._charBuffer, num);
						this._charLen += num;
					}
				}
				if (num != 0)
				{
					goto IL_1CD;
				}
			}
			if (this._byteLen > 0)
			{
				if (readToUserBuffer)
				{
					num = this._decoder.GetChars(new ReadOnlySpan<byte>(this._byteBuffer, 0, this._byteLen), userBuffer.Slice(num), false);
					this._charLen = 0;
				}
				else
				{
					num = this._decoder.GetChars(this._byteBuffer, 0, this._byteLen, this._charBuffer, num);
					this._charLen += num;
				}
			}
			return num;
			IL_1CD:
			this._isBlocked &= (num < userBuffer.Length);
			return num;
		}

		/// <summary>Reads a line of characters from the current stream and returns the data as a string.</summary>
		/// <returns>The next line from the input stream, or <see langword="null" /> if the end of the input stream is reached.</returns>
		/// <exception cref="T:System.OutOfMemoryException">There is insufficient memory to allocate a buffer for the returned string.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		public override string ReadLine()
		{
			if (this._stream == null)
			{
				throw new ObjectDisposedException(null, "Cannot read from a closed TextReader.");
			}
			this.CheckAsyncTaskInProgress();
			if (this._charPos == this._charLen && this.ReadBuffer() == 0)
			{
				return null;
			}
			StringBuilder stringBuilder = null;
			int num;
			char c;
			for (;;)
			{
				num = this._charPos;
				do
				{
					c = this._charBuffer[num];
					if (c == '\r' || c == '\n')
					{
						goto IL_51;
					}
					num++;
				}
				while (num < this._charLen);
				num = this._charLen - this._charPos;
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder(num + 80);
				}
				stringBuilder.Append(this._charBuffer, this._charPos, num);
				if (this.ReadBuffer() <= 0)
				{
					goto Block_11;
				}
			}
			IL_51:
			string result;
			if (stringBuilder != null)
			{
				stringBuilder.Append(this._charBuffer, this._charPos, num - this._charPos);
				result = stringBuilder.ToString();
			}
			else
			{
				result = new string(this._charBuffer, this._charPos, num - this._charPos);
			}
			this._charPos = num + 1;
			if (c == '\r' && (this._charPos < this._charLen || this.ReadBuffer() > 0) && this._charBuffer[this._charPos] == '\n')
			{
				this._charPos++;
			}
			return result;
			Block_11:
			return stringBuilder.ToString();
		}

		/// <summary>Reads a line of characters asynchronously from the current stream and returns the data as a string.</summary>
		/// <returns>A task that represents the asynchronous read operation. The value of the <paramref name="TResult" /> parameter contains the next line from the stream, or is <see langword="null" /> if all the characters have been read.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The number of characters in the next line is larger than <see cref="F:System.Int32.MaxValue" />.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The stream has been disposed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The reader is currently in use by a previous read operation.</exception>
		public override Task<string> ReadLineAsync()
		{
			if (base.GetType() != typeof(StreamReader))
			{
				return base.ReadLineAsync();
			}
			if (this._stream == null)
			{
				throw new ObjectDisposedException(null, "Cannot read from a closed TextReader.");
			}
			this.CheckAsyncTaskInProgress();
			Task<string> task = this.ReadLineAsyncInternal();
			this._asyncReadTask = task;
			return task;
		}

		private Task<string> ReadLineAsyncInternal()
		{
			StreamReader.<ReadLineAsyncInternal>d__61 <ReadLineAsyncInternal>d__;
			<ReadLineAsyncInternal>d__.<>4__this = this;
			<ReadLineAsyncInternal>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<ReadLineAsyncInternal>d__.<>1__state = -1;
			<ReadLineAsyncInternal>d__.<>t__builder.Start<StreamReader.<ReadLineAsyncInternal>d__61>(ref <ReadLineAsyncInternal>d__);
			return <ReadLineAsyncInternal>d__.<>t__builder.Task;
		}

		/// <summary>Reads all characters from the current position to the end of the stream asynchronously and returns them as one string.</summary>
		/// <returns>A task that represents the asynchronous read operation. The value of the <paramref name="TResult" /> parameter contains a string with the characters from the current position to the end of the stream.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The number of characters is larger than <see cref="F:System.Int32.MaxValue" />.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The stream has been disposed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The reader is currently in use by a previous read operation.</exception>
		public override Task<string> ReadToEndAsync()
		{
			if (base.GetType() != typeof(StreamReader))
			{
				return base.ReadToEndAsync();
			}
			if (this._stream == null)
			{
				throw new ObjectDisposedException(null, "Cannot read from a closed TextReader.");
			}
			this.CheckAsyncTaskInProgress();
			Task<string> task = this.ReadToEndAsyncInternal();
			this._asyncReadTask = task;
			return task;
		}

		private Task<string> ReadToEndAsyncInternal()
		{
			StreamReader.<ReadToEndAsyncInternal>d__63 <ReadToEndAsyncInternal>d__;
			<ReadToEndAsyncInternal>d__.<>4__this = this;
			<ReadToEndAsyncInternal>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<ReadToEndAsyncInternal>d__.<>1__state = -1;
			<ReadToEndAsyncInternal>d__.<>t__builder.Start<StreamReader.<ReadToEndAsyncInternal>d__63>(ref <ReadToEndAsyncInternal>d__);
			return <ReadToEndAsyncInternal>d__.<>t__builder.Task;
		}

		/// <summary>Reads a specified maximum number of characters from the current stream asynchronously and writes the data to a buffer, beginning at the specified index.</summary>
		/// <param name="buffer">When this method returns, contains the specified character array with the values between <paramref name="index" /> and (<paramref name="index" /> + <paramref name="count" /> - 1) replaced by the characters read from the current source.</param>
		/// <param name="index">The position in <paramref name="buffer" /> at which to begin writing.</param>
		/// <param name="count">The maximum number of characters to read. If the end of the stream is reached before the specified number of characters is written into the buffer, the current method returns.</param>
		/// <returns>A task that represents the asynchronous read operation. The value of the <paramref name="TResult" /> parameter contains the total number of characters read into the buffer. The result value can be less than the number of characters requested if the number of characters currently available is less than the requested number, or it can be 0 (zero) if the end of the stream has been reached.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> or <paramref name="count" /> is negative.</exception>
		/// <exception cref="T:System.ArgumentException">The sum of <paramref name="index" /> and <paramref name="count" /> is larger than the buffer length.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The stream has been disposed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The reader is currently in use by a previous read operation.</exception>
		public override Task<int> ReadAsync(char[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer", "Buffer cannot be null.");
			}
			if (index < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException((index < 0) ? "index" : "count", "Non-negative number required.");
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			}
			if (base.GetType() != typeof(StreamReader))
			{
				return base.ReadAsync(buffer, index, count);
			}
			if (this._stream == null)
			{
				throw new ObjectDisposedException(null, "Cannot read from a closed TextReader.");
			}
			this.CheckAsyncTaskInProgress();
			Task<int> task = this.ReadAsyncInternal(new Memory<char>(buffer, index, count), default(CancellationToken)).AsTask();
			this._asyncReadTask = task;
			return task;
		}

		public override ValueTask<int> ReadAsync(Memory<char> buffer, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (base.GetType() != typeof(StreamReader))
			{
				return base.ReadAsync(buffer, cancellationToken);
			}
			if (this._stream == null)
			{
				throw new ObjectDisposedException(null, "Cannot read from a closed TextReader.");
			}
			this.CheckAsyncTaskInProgress();
			if (cancellationToken.IsCancellationRequested)
			{
				return new ValueTask<int>(Task.FromCanceled<int>(cancellationToken));
			}
			return this.ReadAsyncInternal(buffer, cancellationToken);
		}

		internal override ValueTask<int> ReadAsyncInternal(Memory<char> buffer, CancellationToken cancellationToken)
		{
			StreamReader.<ReadAsyncInternal>d__66 <ReadAsyncInternal>d__;
			<ReadAsyncInternal>d__.<>4__this = this;
			<ReadAsyncInternal>d__.buffer = buffer;
			<ReadAsyncInternal>d__.cancellationToken = cancellationToken;
			<ReadAsyncInternal>d__.<>t__builder = AsyncValueTaskMethodBuilder<int>.Create();
			<ReadAsyncInternal>d__.<>1__state = -1;
			<ReadAsyncInternal>d__.<>t__builder.Start<StreamReader.<ReadAsyncInternal>d__66>(ref <ReadAsyncInternal>d__);
			return <ReadAsyncInternal>d__.<>t__builder.Task;
		}

		/// <summary>Reads a specified maximum number of characters from the current stream asynchronously and writes the data to a buffer, beginning at the specified index.</summary>
		/// <param name="buffer">When this method returns, contains the specified character array with the values between <paramref name="index" /> and (<paramref name="index" /> + <paramref name="count" /> - 1) replaced by the characters read from the current source.</param>
		/// <param name="index">The position in <paramref name="buffer" /> at which to begin writing.</param>
		/// <param name="count">The maximum number of characters to read. If the end of the stream is reached before the specified number of characters is written into the buffer, the method returns.</param>
		/// <returns>A task that represents the asynchronous read operation. The value of the <paramref name="TResult" /> parameter contains the total number of characters read into the buffer. The result value can be less than the number of characters requested if the number of characters currently available is less than the requested number, or it can be 0 (zero) if the end of the stream has been reached.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> or <paramref name="count" /> is negative.</exception>
		/// <exception cref="T:System.ArgumentException">The sum of <paramref name="index" /> and <paramref name="count" /> is larger than the buffer length.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The stream has been disposed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The reader is currently in use by a previous read operation.</exception>
		public override Task<int> ReadBlockAsync(char[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer", "Buffer cannot be null.");
			}
			if (index < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException((index < 0) ? "index" : "count", "Non-negative number required.");
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			}
			if (base.GetType() != typeof(StreamReader))
			{
				return base.ReadBlockAsync(buffer, index, count);
			}
			if (this._stream == null)
			{
				throw new ObjectDisposedException(null, "Cannot read from a closed TextReader.");
			}
			this.CheckAsyncTaskInProgress();
			Task<int> task = base.ReadBlockAsync(buffer, index, count);
			this._asyncReadTask = task;
			return task;
		}

		public override ValueTask<int> ReadBlockAsync(Memory<char> buffer, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (base.GetType() != typeof(StreamReader))
			{
				return base.ReadBlockAsync(buffer, cancellationToken);
			}
			if (this._stream == null)
			{
				throw new ObjectDisposedException(null, "Cannot read from a closed TextReader.");
			}
			this.CheckAsyncTaskInProgress();
			if (cancellationToken.IsCancellationRequested)
			{
				return new ValueTask<int>(Task.FromCanceled<int>(cancellationToken));
			}
			ValueTask<int> result = base.ReadBlockAsyncInternal(buffer, cancellationToken);
			if (result.IsCompletedSuccessfully)
			{
				return result;
			}
			Task<int> task = result.AsTask();
			this._asyncReadTask = task;
			return new ValueTask<int>(task);
		}

		private Task<int> ReadBufferAsync()
		{
			StreamReader.<ReadBufferAsync>d__69 <ReadBufferAsync>d__;
			<ReadBufferAsync>d__.<>4__this = this;
			<ReadBufferAsync>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<ReadBufferAsync>d__.<>1__state = -1;
			<ReadBufferAsync>d__.<>t__builder.Start<StreamReader.<ReadBufferAsync>d__69>(ref <ReadBufferAsync>d__);
			return <ReadBufferAsync>d__.<>t__builder.Task;
		}

		internal bool DataAvailable()
		{
			return this._charPos < this._charLen;
		}

		/// <summary>A <see cref="T:System.IO.StreamReader" /> object around an empty stream.</summary>
		public new static readonly StreamReader Null = new StreamReader.NullStreamReader();

		private const int DefaultBufferSize = 1024;

		private const int DefaultFileStreamBufferSize = 4096;

		private const int MinBufferSize = 128;

		private Stream _stream;

		private Encoding _encoding;

		private Decoder _decoder;

		private byte[] _byteBuffer;

		private char[] _charBuffer;

		private int _charPos;

		private int _charLen;

		private int _byteLen;

		private int _bytePos;

		private int _maxCharsPerBuffer;

		private bool _detectEncoding;

		private bool _checkPreamble;

		private bool _isBlocked;

		private bool _closable;

		private Task _asyncReadTask = Task.CompletedTask;

		private class NullStreamReader : StreamReader
		{
			internal NullStreamReader()
			{
				base.Init(Stream.Null);
			}

			public override Stream BaseStream
			{
				get
				{
					return Stream.Null;
				}
			}

			public override Encoding CurrentEncoding
			{
				get
				{
					return Encoding.Unicode;
				}
			}

			protected override void Dispose(bool disposing)
			{
			}

			public override int Peek()
			{
				return -1;
			}

			public override int Read()
			{
				return -1;
			}

			public override int Read(char[] buffer, int index, int count)
			{
				return 0;
			}

			public override string ReadLine()
			{
				return null;
			}

			public override string ReadToEnd()
			{
				return string.Empty;
			}

			internal override int ReadBuffer()
			{
				return 0;
			}
		}
	}
}
