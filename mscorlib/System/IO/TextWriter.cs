using System;
using System.Buffers;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO
{
	/// <summary>Represents a writer that can write a sequential series of characters. This class is abstract.</summary>
	[Serializable]
	public abstract class TextWriter : MarshalByRefObject, IDisposable, IAsyncDisposable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.IO.TextWriter" /> class.</summary>
		protected TextWriter()
		{
			this._internalFormatProvider = null;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.TextWriter" /> class with the specified format provider.</summary>
		/// <param name="formatProvider">An <see cref="T:System.IFormatProvider" /> object that controls formatting.</param>
		protected TextWriter(IFormatProvider formatProvider)
		{
			this._internalFormatProvider = formatProvider;
		}

		/// <summary>Gets an object that controls formatting.</summary>
		/// <returns>An <see cref="T:System.IFormatProvider" /> object for a specific culture, or the formatting of the current culture if no other culture is specified.</returns>
		public virtual IFormatProvider FormatProvider
		{
			get
			{
				if (this._internalFormatProvider == null)
				{
					return CultureInfo.CurrentCulture;
				}
				return this._internalFormatProvider;
			}
		}

		/// <summary>Closes the current writer and releases any system resources associated with the writer.</summary>
		public virtual void Close()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.IO.TextWriter" /> and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
		}

		/// <summary>Releases all resources used by the <see cref="T:System.IO.TextWriter" /> object.</summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual ValueTask DisposeAsync()
		{
			ValueTask valueTask;
			try
			{
				this.Dispose();
				valueTask = default(ValueTask);
				valueTask = valueTask;
			}
			catch (Exception exception)
			{
				valueTask = new ValueTask(Task.FromException(exception));
			}
			return valueTask;
		}

		/// <summary>Clears all buffers for the current writer and causes any buffered data to be written to the underlying device.</summary>
		public virtual void Flush()
		{
		}

		/// <summary>When overridden in a derived class, returns the character encoding in which the output is written.</summary>
		/// <returns>The character encoding in which the output is written.</returns>
		public abstract Encoding Encoding { get; }

		/// <summary>Gets or sets the line terminator string used by the current <see langword="TextWriter" />.</summary>
		/// <returns>The line terminator string for the current <see langword="TextWriter" />.</returns>
		public virtual string NewLine
		{
			get
			{
				return this.CoreNewLineStr;
			}
			set
			{
				if (value == null)
				{
					value = Environment.NewLine;
				}
				this.CoreNewLineStr = value;
				this.CoreNewLine = value.ToCharArray();
			}
		}

		/// <summary>Writes a character to the text string or stream.</summary>
		/// <param name="value">The character to write to the text stream.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		public virtual void Write(char value)
		{
		}

		/// <summary>Writes a character array to the text string or stream.</summary>
		/// <param name="buffer">The character array to write to the text stream.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		public virtual void Write(char[] buffer)
		{
			if (buffer != null)
			{
				this.Write(buffer, 0, buffer.Length);
			}
		}

		/// <summary>Writes a subarray of characters to the text string or stream.</summary>
		/// <param name="buffer">The character array to write data from.</param>
		/// <param name="index">The character position in the buffer at which to start retrieving data.</param>
		/// <param name="count">The number of characters to write.</param>
		/// <exception cref="T:System.ArgumentException">The buffer length minus <paramref name="index" /> is less than <paramref name="count" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="buffer" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> or <paramref name="count" /> is negative.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		public virtual void Write(char[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer", "Buffer cannot be null.");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "Non-negative number required.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Non-negative number required.");
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			}
			for (int i = 0; i < count; i++)
			{
				this.Write(buffer[index + i]);
			}
		}

		public virtual void Write(ReadOnlySpan<char> buffer)
		{
			char[] array = ArrayPool<char>.Shared.Rent(buffer.Length);
			try
			{
				buffer.CopyTo(new Span<char>(array));
				this.Write(array, 0, buffer.Length);
			}
			finally
			{
				ArrayPool<char>.Shared.Return(array, false);
			}
		}

		/// <summary>Writes the text representation of a <see langword="Boolean" /> value to the text string or stream.</summary>
		/// <param name="value">The <see langword="Boolean" /> value to write.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		public virtual void Write(bool value)
		{
			this.Write(value ? "True" : "False");
		}

		/// <summary>Writes the text representation of a 4-byte signed integer to the text string or stream.</summary>
		/// <param name="value">The 4-byte signed integer to write.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		public virtual void Write(int value)
		{
			this.Write(value.ToString(this.FormatProvider));
		}

		/// <summary>Writes the text representation of a 4-byte unsigned integer to the text string or stream.</summary>
		/// <param name="value">The 4-byte unsigned integer to write.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		[CLSCompliant(false)]
		public virtual void Write(uint value)
		{
			this.Write(value.ToString(this.FormatProvider));
		}

		/// <summary>Writes the text representation of an 8-byte signed integer to the text string or stream.</summary>
		/// <param name="value">The 8-byte signed integer to write.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		public virtual void Write(long value)
		{
			this.Write(value.ToString(this.FormatProvider));
		}

		/// <summary>Writes the text representation of an 8-byte unsigned integer to the text string or stream.</summary>
		/// <param name="value">The 8-byte unsigned integer to write.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		[CLSCompliant(false)]
		public virtual void Write(ulong value)
		{
			this.Write(value.ToString(this.FormatProvider));
		}

		/// <summary>Writes the text representation of a 4-byte floating-point value to the text string or stream.</summary>
		/// <param name="value">The 4-byte floating-point value to write.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		public virtual void Write(float value)
		{
			this.Write(value.ToString(this.FormatProvider));
		}

		/// <summary>Writes the text representation of an 8-byte floating-point value to the text string or stream.</summary>
		/// <param name="value">The 8-byte floating-point value to write.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		public virtual void Write(double value)
		{
			this.Write(value.ToString(this.FormatProvider));
		}

		/// <summary>Writes the text representation of a decimal value to the text string or stream.</summary>
		/// <param name="value">The decimal value to write.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		public virtual void Write(decimal value)
		{
			this.Write(value.ToString(this.FormatProvider));
		}

		/// <summary>Writes a string to the text string or stream.</summary>
		/// <param name="value">The string to write.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		public virtual void Write(string value)
		{
			if (value != null)
			{
				this.Write(value.ToCharArray());
			}
		}

		/// <summary>Writes the text representation of an object to the text string or stream by calling the <see langword="ToString" /> method on that object.</summary>
		/// <param name="value">The object to write.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		public virtual void Write(object value)
		{
			if (value != null)
			{
				IFormattable formattable = value as IFormattable;
				if (formattable != null)
				{
					this.Write(formattable.ToString(null, this.FormatProvider));
					return;
				}
				this.Write(value.ToString());
			}
		}

		/// <summary>Writes a formatted string to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object)" /> method.</summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="arg0">The object to format and write.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="format" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="format" /> is not a valid composite format string.  
		/// -or-  
		/// The index of a format item is less than 0 (zero), or greater than or equal to the number of objects to be formatted (which, for this method overload, is one).</exception>
		public virtual void Write(string format, object arg0)
		{
			this.Write(string.Format(this.FormatProvider, format, arg0));
		}

		/// <summary>Writes a formatted string to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object,System.Object)" /> method.</summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="arg0">The first object to format and write.</param>
		/// <param name="arg1">The second object to format and write.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="format" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="format" /> is not a valid composite format string.  
		/// -or-  
		/// The index of a format item is less than 0 (zero) or greater than or equal to the number of objects to be formatted (which, for this method overload, is two).</exception>
		public virtual void Write(string format, object arg0, object arg1)
		{
			this.Write(string.Format(this.FormatProvider, format, arg0, arg1));
		}

		/// <summary>Writes a formatted string to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object,System.Object,System.Object)" /> method.</summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="arg0">The first object to format and write.</param>
		/// <param name="arg1">The second object to format and write.</param>
		/// <param name="arg2">The third object to format and write.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="format" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="format" /> is not a valid composite format string.  
		/// -or-  
		/// The index of a format item is less than 0 (zero), or greater than or equal to the number of objects to be formatted (which, for this method overload, is three).</exception>
		public virtual void Write(string format, object arg0, object arg1, object arg2)
		{
			this.Write(string.Format(this.FormatProvider, format, arg0, arg1, arg2));
		}

		/// <summary>Writes a formatted string to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object[])" /> method.</summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="arg">An object array that contains zero or more objects to format and write.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="format" /> or <paramref name="arg" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="format" /> is not a valid composite format string.  
		/// -or-  
		/// The index of a format item is less than 0 (zero), or greater than or equal to the length of the <paramref name="arg" /> array.</exception>
		public virtual void Write(string format, params object[] arg)
		{
			this.Write(string.Format(this.FormatProvider, format, arg));
		}

		/// <summary>Writes a line terminator to the text string or stream.</summary>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		public virtual void WriteLine()
		{
			this.Write(this.CoreNewLine);
		}

		/// <summary>Writes a character followed by a line terminator to the text string or stream.</summary>
		/// <param name="value">The character to write to the text stream.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		public virtual void WriteLine(char value)
		{
			this.Write(value);
			this.WriteLine();
		}

		/// <summary>Writes an array of characters followed by a line terminator to the text string or stream.</summary>
		/// <param name="buffer">The character array from which data is read.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		public virtual void WriteLine(char[] buffer)
		{
			this.Write(buffer);
			this.WriteLine();
		}

		/// <summary>Writes a subarray of characters followed by a line terminator to the text string or stream.</summary>
		/// <param name="buffer">The character array from which data is read.</param>
		/// <param name="index">The character position in <paramref name="buffer" /> at which to start reading data.</param>
		/// <param name="count">The maximum number of characters to write.</param>
		/// <exception cref="T:System.ArgumentException">The buffer length minus <paramref name="index" /> is less than <paramref name="count" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="buffer" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> or <paramref name="count" /> is negative.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		public virtual void WriteLine(char[] buffer, int index, int count)
		{
			this.Write(buffer, index, count);
			this.WriteLine();
		}

		public virtual void WriteLine(ReadOnlySpan<char> buffer)
		{
			char[] array = ArrayPool<char>.Shared.Rent(buffer.Length);
			try
			{
				buffer.CopyTo(new Span<char>(array));
				this.WriteLine(array, 0, buffer.Length);
			}
			finally
			{
				ArrayPool<char>.Shared.Return(array, false);
			}
		}

		/// <summary>Writes the text representation of a <see langword="Boolean" /> value followed by a line terminator to the text string or stream.</summary>
		/// <param name="value">The <see langword="Boolean" /> value to write.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		public virtual void WriteLine(bool value)
		{
			this.Write(value);
			this.WriteLine();
		}

		/// <summary>Writes the text representation of a 4-byte signed integer followed by a line terminator to the text string or stream.</summary>
		/// <param name="value">The 4-byte signed integer to write.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		public virtual void WriteLine(int value)
		{
			this.Write(value);
			this.WriteLine();
		}

		/// <summary>Writes the text representation of a 4-byte unsigned integer followed by a line terminator to the text string or stream.</summary>
		/// <param name="value">The 4-byte unsigned integer to write.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		[CLSCompliant(false)]
		public virtual void WriteLine(uint value)
		{
			this.Write(value);
			this.WriteLine();
		}

		/// <summary>Writes the text representation of an 8-byte signed integer followed by a line terminator to the text string or stream.</summary>
		/// <param name="value">The 8-byte signed integer to write.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		public virtual void WriteLine(long value)
		{
			this.Write(value);
			this.WriteLine();
		}

		/// <summary>Writes the text representation of an 8-byte unsigned integer followed by a line terminator to the text string or stream.</summary>
		/// <param name="value">The 8-byte unsigned integer to write.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		[CLSCompliant(false)]
		public virtual void WriteLine(ulong value)
		{
			this.Write(value);
			this.WriteLine();
		}

		/// <summary>Writes the text representation of a 4-byte floating-point value followed by a line terminator to the text string or stream.</summary>
		/// <param name="value">The 4-byte floating-point value to write.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		public virtual void WriteLine(float value)
		{
			this.Write(value);
			this.WriteLine();
		}

		/// <summary>Writes the text representation of a 8-byte floating-point value followed by a line terminator to the text string or stream.</summary>
		/// <param name="value">The 8-byte floating-point value to write.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		public virtual void WriteLine(double value)
		{
			this.Write(value);
			this.WriteLine();
		}

		/// <summary>Writes the text representation of a decimal value followed by a line terminator to the text string or stream.</summary>
		/// <param name="value">The decimal value to write.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		public virtual void WriteLine(decimal value)
		{
			this.Write(value);
			this.WriteLine();
		}

		/// <summary>Writes a string followed by a line terminator to the text string or stream.</summary>
		/// <param name="value">The string to write. If <paramref name="value" /> is <see langword="null" />, only the line terminator is written.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		public virtual void WriteLine(string value)
		{
			if (value != null)
			{
				this.Write(value);
			}
			this.Write(this.CoreNewLineStr);
		}

		/// <summary>Writes the text representation of an object by calling the <see langword="ToString" /> method on that object, followed by a line terminator to the text string or stream.</summary>
		/// <param name="value">The object to write. If <paramref name="value" /> is <see langword="null" />, only the line terminator is written.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		public virtual void WriteLine(object value)
		{
			if (value == null)
			{
				this.WriteLine();
				return;
			}
			IFormattable formattable = value as IFormattable;
			if (formattable != null)
			{
				this.WriteLine(formattable.ToString(null, this.FormatProvider));
				return;
			}
			this.WriteLine(value.ToString());
		}

		/// <summary>Writes a formatted string and a new line to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object)" /> method.</summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="arg0">The object to format and write.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="format" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="format" /> is not a valid composite format string.  
		/// -or-  
		/// The index of a format item is less than 0 (zero), or greater than or equal to the number of objects to be formatted (which, for this method overload, is one).</exception>
		public virtual void WriteLine(string format, object arg0)
		{
			this.WriteLine(string.Format(this.FormatProvider, format, arg0));
		}

		/// <summary>Writes a formatted string and a new line to the text string or stream, using the same semantics as the <see cref="M:System.String.Format(System.String,System.Object,System.Object)" /> method.</summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="arg0">The first object to format and write.</param>
		/// <param name="arg1">The second object to format and write.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="format" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="format" /> is not a valid composite format string.  
		/// -or-  
		/// The index of a format item is less than 0 (zero), or greater than or equal to the number of objects to be formatted (which, for this method overload, is two).</exception>
		public virtual void WriteLine(string format, object arg0, object arg1)
		{
			this.WriteLine(string.Format(this.FormatProvider, format, arg0, arg1));
		}

		/// <summary>Writes out a formatted string and a new line, using the same semantics as <see cref="M:System.String.Format(System.String,System.Object)" />.</summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="arg0">The first object to format and write.</param>
		/// <param name="arg1">The second object to format and write.</param>
		/// <param name="arg2">The third object to format and write.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="format" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="format" /> is not a valid composite format string.  
		/// -or-  
		/// The index of a format item is less than 0 (zero), or greater than or equal to the number of objects to be formatted (which, for this method overload, is three).</exception>
		public virtual void WriteLine(string format, object arg0, object arg1, object arg2)
		{
			this.WriteLine(string.Format(this.FormatProvider, format, arg0, arg1, arg2));
		}

		/// <summary>Writes out a formatted string and a new line, using the same semantics as <see cref="M:System.String.Format(System.String,System.Object)" />.</summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="arg">An object array that contains zero or more objects to format and write.</param>
		/// <exception cref="T:System.ArgumentNullException">A string or object is passed in as <see langword="null" />.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter" /> is closed.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="format" /> is not a valid composite format string.  
		/// -or-  
		/// The index of a format item is less than 0 (zero), or greater than or equal to the length of the <paramref name="arg" /> array.</exception>
		public virtual void WriteLine(string format, params object[] arg)
		{
			this.WriteLine(string.Format(this.FormatProvider, format, arg));
		}

		/// <summary>Writes a character to the text string or stream asynchronously.</summary>
		/// <param name="value">The character to write to the text stream.</param>
		/// <returns>A task that represents the asynchronous write operation.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The text writer is disposed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The text writer is currently in use by a previous write operation.</exception>
		public virtual Task WriteAsync(char value)
		{
			Tuple<TextWriter, char> state2 = new Tuple<TextWriter, char>(this, value);
			return Task.Factory.StartNew(delegate(object state)
			{
				Tuple<TextWriter, char> tuple = (Tuple<TextWriter, char>)state;
				tuple.Item1.Write(tuple.Item2);
			}, state2, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
		}

		/// <summary>Writes a string to the text string or stream asynchronously.</summary>
		/// <param name="value">The string to write. If <paramref name="value" /> is <see langword="null" />, nothing is written to the text stream.</param>
		/// <returns>A task that represents the asynchronous write operation.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The text writer is disposed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The text writer is currently in use by a previous write operation.</exception>
		public virtual Task WriteAsync(string value)
		{
			Tuple<TextWriter, string> state2 = new Tuple<TextWriter, string>(this, value);
			return Task.Factory.StartNew(delegate(object state)
			{
				Tuple<TextWriter, string> tuple = (Tuple<TextWriter, string>)state;
				tuple.Item1.Write(tuple.Item2);
			}, state2, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
		}

		/// <summary>Writes a character array to the text string or stream asynchronously.</summary>
		/// <param name="buffer">The character array to write to the text stream. If <paramref name="buffer" /> is <see langword="null" />, nothing is written.</param>
		/// <returns>A task that represents the asynchronous write operation.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The text writer is disposed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The text writer is currently in use by a previous write operation.</exception>
		public Task WriteAsync(char[] buffer)
		{
			if (buffer == null)
			{
				return Task.CompletedTask;
			}
			return this.WriteAsync(buffer, 0, buffer.Length);
		}

		/// <summary>Writes a subarray of characters to the text string or stream asynchronously.</summary>
		/// <param name="buffer">The character array to write data from.</param>
		/// <param name="index">The character position in the buffer at which to start retrieving data.</param>
		/// <param name="count">The number of characters to write.</param>
		/// <returns>A task that represents the asynchronous write operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="index" /> plus <paramref name="count" /> is greater than the buffer length.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> or <paramref name="count" /> is negative.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The text writer is disposed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The text writer is currently in use by a previous write operation.</exception>
		public virtual Task WriteAsync(char[] buffer, int index, int count)
		{
			Tuple<TextWriter, char[], int, int> state2 = new Tuple<TextWriter, char[], int, int>(this, buffer, index, count);
			return Task.Factory.StartNew(delegate(object state)
			{
				Tuple<TextWriter, char[], int, int> tuple = (Tuple<TextWriter, char[], int, int>)state;
				tuple.Item1.Write(tuple.Item2, tuple.Item3, tuple.Item4);
			}, state2, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
		}

		public virtual Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default(CancellationToken))
		{
			ArraySegment<char> arraySegment;
			if (!MemoryMarshal.TryGetArray<char>(buffer, out arraySegment))
			{
				return Task.Factory.StartNew(delegate(object state)
				{
					Tuple<TextWriter, ReadOnlyMemory<char>> tuple = (Tuple<TextWriter, ReadOnlyMemory<char>>)state;
					tuple.Item1.Write(tuple.Item2.Span);
				}, Tuple.Create<TextWriter, ReadOnlyMemory<char>>(this, buffer), cancellationToken, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
			}
			return this.WriteAsync(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
		}

		/// <summary>Writes a character followed by a line terminator asynchronously to the text string or stream.</summary>
		/// <param name="value">The character to write to the text stream.</param>
		/// <returns>A task that represents the asynchronous write operation.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The text writer is disposed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The text writer is currently in use by a previous write operation.</exception>
		public virtual Task WriteLineAsync(char value)
		{
			Tuple<TextWriter, char> state2 = new Tuple<TextWriter, char>(this, value);
			return Task.Factory.StartNew(delegate(object state)
			{
				Tuple<TextWriter, char> tuple = (Tuple<TextWriter, char>)state;
				tuple.Item1.WriteLine(tuple.Item2);
			}, state2, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
		}

		/// <summary>Writes a string followed by a line terminator asynchronously to the text string or stream.</summary>
		/// <param name="value">The string to write. If the value is <see langword="null" />, only a line terminator is written.</param>
		/// <returns>A task that represents the asynchronous write operation.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The text writer is disposed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The text writer is currently in use by a previous write operation.</exception>
		public virtual Task WriteLineAsync(string value)
		{
			Tuple<TextWriter, string> state2 = new Tuple<TextWriter, string>(this, value);
			return Task.Factory.StartNew(delegate(object state)
			{
				Tuple<TextWriter, string> tuple = (Tuple<TextWriter, string>)state;
				tuple.Item1.WriteLine(tuple.Item2);
			}, state2, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
		}

		/// <summary>Writes an array of characters followed by a line terminator asynchronously to the text string or stream.</summary>
		/// <param name="buffer">The character array to write to the text stream. If the character array is <see langword="null" />, only the line terminator is written.</param>
		/// <returns>A task that represents the asynchronous write operation.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The text writer is disposed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The text writer is currently in use by a previous write operation.</exception>
		public Task WriteLineAsync(char[] buffer)
		{
			if (buffer == null)
			{
				return this.WriteLineAsync();
			}
			return this.WriteLineAsync(buffer, 0, buffer.Length);
		}

		/// <summary>Writes a subarray of characters followed by a line terminator asynchronously to the text string or stream.</summary>
		/// <param name="buffer">The character array to write data from.</param>
		/// <param name="index">The character position in the buffer at which to start retrieving data.</param>
		/// <param name="count">The number of characters to write.</param>
		/// <returns>A task that represents the asynchronous write operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="index" /> plus <paramref name="count" /> is greater than the buffer length.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> or <paramref name="count" /> is negative.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The text writer is disposed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The text writer is currently in use by a previous write operation.</exception>
		public virtual Task WriteLineAsync(char[] buffer, int index, int count)
		{
			Tuple<TextWriter, char[], int, int> state2 = new Tuple<TextWriter, char[], int, int>(this, buffer, index, count);
			return Task.Factory.StartNew(delegate(object state)
			{
				Tuple<TextWriter, char[], int, int> tuple = (Tuple<TextWriter, char[], int, int>)state;
				tuple.Item1.WriteLine(tuple.Item2, tuple.Item3, tuple.Item4);
			}, state2, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
		}

		public virtual Task WriteLineAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default(CancellationToken))
		{
			ArraySegment<char> arraySegment;
			if (!MemoryMarshal.TryGetArray<char>(buffer, out arraySegment))
			{
				return Task.Factory.StartNew(delegate(object state)
				{
					Tuple<TextWriter, ReadOnlyMemory<char>> tuple = (Tuple<TextWriter, ReadOnlyMemory<char>>)state;
					tuple.Item1.WriteLine(tuple.Item2.Span);
				}, Tuple.Create<TextWriter, ReadOnlyMemory<char>>(this, buffer), cancellationToken, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
			}
			return this.WriteLineAsync(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
		}

		/// <summary>Writes a line terminator asynchronously to the text string or stream.</summary>
		/// <returns>A task that represents the asynchronous write operation.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The text writer is disposed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The text writer is currently in use by a previous write operation.</exception>
		public virtual Task WriteLineAsync()
		{
			return this.WriteAsync(this.CoreNewLine);
		}

		/// <summary>Asynchronously clears all buffers for the current writer and causes any buffered data to be written to the underlying device.</summary>
		/// <returns>A task that represents the asynchronous flush operation.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The text writer is disposed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The writer is currently in use by a previous write operation.</exception>
		public virtual Task FlushAsync()
		{
			return Task.Factory.StartNew(delegate(object state)
			{
				((TextWriter)state).Flush();
			}, this, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
		}

		/// <summary>Creates a thread-safe wrapper around the specified <see langword="TextWriter" />.</summary>
		/// <param name="writer">The <see langword="TextWriter" /> to synchronize.</param>
		/// <returns>A thread-safe wrapper.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="writer" /> is <see langword="null" />.</exception>
		public static TextWriter Synchronized(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (!(writer is TextWriter.SyncTextWriter))
			{
				return new TextWriter.SyncTextWriter(writer);
			}
			return writer;
		}

		/// <summary>Provides a <see langword="TextWriter" /> with no backing store that can be written to, but not read from.</summary>
		public static readonly TextWriter Null = new TextWriter.NullTextWriter();

		private static readonly char[] s_coreNewLine = Environment.NewLine.ToCharArray();

		/// <summary>Stores the newline characters used for this <see langword="TextWriter" />.</summary>
		protected char[] CoreNewLine = TextWriter.s_coreNewLine;

		private string CoreNewLineStr = Environment.NewLine;

		private IFormatProvider _internalFormatProvider;

		[Serializable]
		private sealed class NullTextWriter : TextWriter
		{
			internal NullTextWriter() : base(CultureInfo.InvariantCulture)
			{
			}

			public override Encoding Encoding
			{
				get
				{
					return Encoding.Unicode;
				}
			}

			public override void Write(char[] buffer, int index, int count)
			{
			}

			public override void Write(string value)
			{
			}

			public override void WriteLine()
			{
			}

			public override void WriteLine(string value)
			{
			}

			public override void WriteLine(object value)
			{
			}

			public override void Write(char value)
			{
			}
		}

		[Serializable]
		internal sealed class SyncTextWriter : TextWriter, IDisposable
		{
			internal SyncTextWriter(TextWriter t) : base(t.FormatProvider)
			{
				this._out = t;
			}

			public override Encoding Encoding
			{
				get
				{
					return this._out.Encoding;
				}
			}

			public override IFormatProvider FormatProvider
			{
				get
				{
					return this._out.FormatProvider;
				}
			}

			public override string NewLine
			{
				[MethodImpl(MethodImplOptions.Synchronized)]
				get
				{
					return this._out.NewLine;
				}
				[MethodImpl(MethodImplOptions.Synchronized)]
				set
				{
					this._out.NewLine = value;
				}
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void Close()
			{
				this._out.Close();
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			protected override void Dispose(bool disposing)
			{
				if (disposing)
				{
					((IDisposable)this._out).Dispose();
				}
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void Flush()
			{
				this._out.Flush();
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void Write(char value)
			{
				this._out.Write(value);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void Write(char[] buffer)
			{
				this._out.Write(buffer);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void Write(char[] buffer, int index, int count)
			{
				this._out.Write(buffer, index, count);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void Write(bool value)
			{
				this._out.Write(value);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void Write(int value)
			{
				this._out.Write(value);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void Write(uint value)
			{
				this._out.Write(value);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void Write(long value)
			{
				this._out.Write(value);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void Write(ulong value)
			{
				this._out.Write(value);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void Write(float value)
			{
				this._out.Write(value);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void Write(double value)
			{
				this._out.Write(value);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void Write(decimal value)
			{
				this._out.Write(value);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void Write(string value)
			{
				this._out.Write(value);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void Write(object value)
			{
				this._out.Write(value);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void Write(string format, object arg0)
			{
				this._out.Write(format, arg0);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void Write(string format, object arg0, object arg1)
			{
				this._out.Write(format, arg0, arg1);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void Write(string format, object arg0, object arg1, object arg2)
			{
				this._out.Write(format, arg0, arg1, arg2);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void Write(string format, params object[] arg)
			{
				this._out.Write(format, arg);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void WriteLine()
			{
				this._out.WriteLine();
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void WriteLine(char value)
			{
				this._out.WriteLine(value);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void WriteLine(decimal value)
			{
				this._out.WriteLine(value);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void WriteLine(char[] buffer)
			{
				this._out.WriteLine(buffer);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void WriteLine(char[] buffer, int index, int count)
			{
				this._out.WriteLine(buffer, index, count);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void WriteLine(bool value)
			{
				this._out.WriteLine(value);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void WriteLine(int value)
			{
				this._out.WriteLine(value);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void WriteLine(uint value)
			{
				this._out.WriteLine(value);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void WriteLine(long value)
			{
				this._out.WriteLine(value);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void WriteLine(ulong value)
			{
				this._out.WriteLine(value);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void WriteLine(float value)
			{
				this._out.WriteLine(value);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void WriteLine(double value)
			{
				this._out.WriteLine(value);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void WriteLine(string value)
			{
				this._out.WriteLine(value);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void WriteLine(object value)
			{
				this._out.WriteLine(value);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void WriteLine(string format, object arg0)
			{
				this._out.WriteLine(format, arg0);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void WriteLine(string format, object arg0, object arg1)
			{
				this._out.WriteLine(format, arg0, arg1);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void WriteLine(string format, object arg0, object arg1, object arg2)
			{
				this._out.WriteLine(format, arg0, arg1, arg2);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override void WriteLine(string format, params object[] arg)
			{
				this._out.WriteLine(format, arg);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override Task WriteAsync(char value)
			{
				this.Write(value);
				return Task.CompletedTask;
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override Task WriteAsync(string value)
			{
				this.Write(value);
				return Task.CompletedTask;
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override Task WriteAsync(char[] buffer, int index, int count)
			{
				this.Write(buffer, index, count);
				return Task.CompletedTask;
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override Task WriteLineAsync(char value)
			{
				this.WriteLine(value);
				return Task.CompletedTask;
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override Task WriteLineAsync(string value)
			{
				this.WriteLine(value);
				return Task.CompletedTask;
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override Task WriteLineAsync(char[] buffer, int index, int count)
			{
				this.WriteLine(buffer, index, count);
				return Task.CompletedTask;
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public override Task FlushAsync()
			{
				this.Flush();
				return Task.CompletedTask;
			}

			private readonly TextWriter _out;
		}
	}
}
