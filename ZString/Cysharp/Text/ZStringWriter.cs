using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Cysharp.Text
{
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ZStringWriter : TextWriter
	{
		public ZStringWriter() : this(CultureInfo.CurrentCulture)
		{
		}

		public ZStringWriter(IFormatProvider formatProvider) : base(formatProvider)
		{
			this.sb = ZString.CreateStringBuilder();
			this.isOpen = true;
		}

		public override void Close()
		{
			this.Dispose(true);
		}

		protected override void Dispose(bool disposing)
		{
			this.sb.Dispose();
			this.isOpen = false;
			base.Dispose(disposing);
		}

		public override Encoding Encoding
		{
			get
			{
				return this.encoding = (this.encoding ?? new UnicodeEncoding(false, false));
			}
		}

		public override void Write(char value)
		{
			this.AssertNotDisposed();
			this.sb.Append(value);
		}

		public override void Write(char[] buffer, int index, int count)
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
			if (buffer.Length - index < count)
			{
				throw new ArgumentException();
			}
			this.AssertNotDisposed();
			this.sb.Append(buffer, index, count);
		}

		public override void Write(string value)
		{
			this.AssertNotDisposed();
			if (value != null)
			{
				this.sb.Append(value);
			}
		}

		public override Task WriteAsync(char value)
		{
			this.Write(value);
			return Task.CompletedTask;
		}

		public override Task WriteAsync(string value)
		{
			this.Write(value);
			return Task.CompletedTask;
		}

		public override Task WriteAsync(char[] buffer, int index, int count)
		{
			this.Write(buffer, index, count);
			return Task.CompletedTask;
		}

		public override Task WriteLineAsync(char value)
		{
			this.WriteLine(value);
			return Task.CompletedTask;
		}

		public override Task WriteLineAsync(string value)
		{
			this.WriteLine(value);
			return Task.CompletedTask;
		}

		public override Task WriteLineAsync(char[] buffer, int index, int count)
		{
			this.WriteLine(buffer, index, count);
			return Task.CompletedTask;
		}

		public override void Write(bool value)
		{
			this.AssertNotDisposed();
			this.sb.Append<bool>(value);
		}

		public override void Write(decimal value)
		{
			this.AssertNotDisposed();
			this.sb.Append(value);
		}

		public override Task FlushAsync()
		{
			return Task.CompletedTask;
		}

		public override string ToString()
		{
			return this.sb.ToString();
		}

		private void AssertNotDisposed()
		{
			if (!this.isOpen)
			{
				throw new ObjectDisposedException("sb");
			}
		}

		private Utf16ValueStringBuilder sb;

		private bool isOpen;

		[Nullable(2)]
		private UnicodeEncoding encoding;
	}
}
