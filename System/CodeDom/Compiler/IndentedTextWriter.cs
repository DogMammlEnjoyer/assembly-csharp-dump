using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace System.CodeDom.Compiler
{
	/// <summary>Provides a text writer that can indent new lines by a tab string token.</summary>
	public class IndentedTextWriter : TextWriter
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.Compiler.IndentedTextWriter" /> class using the specified text writer and default tab string.</summary>
		/// <param name="writer">The <see cref="T:System.IO.TextWriter" /> to use for output.</param>
		public IndentedTextWriter(TextWriter writer) : this(writer, "    ")
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.CodeDom.Compiler.IndentedTextWriter" /> class using the specified text writer and tab string.</summary>
		/// <param name="writer">The <see cref="T:System.IO.TextWriter" /> to use for output.</param>
		/// <param name="tabString">The tab string to use for indentation.</param>
		public IndentedTextWriter(TextWriter writer, string tabString) : base(CultureInfo.InvariantCulture)
		{
			this._writer = writer;
			this._tabString = tabString;
			this._indentLevel = 0;
			this._tabsPending = false;
		}

		/// <summary>Gets the encoding for the text writer to use.</summary>
		/// <returns>An <see cref="T:System.Text.Encoding" /> that indicates the encoding for the text writer to use.</returns>
		public override Encoding Encoding
		{
			get
			{
				return this._writer.Encoding;
			}
		}

		/// <summary>Gets or sets the new line character to use.</summary>
		/// <returns>The new line character to use.</returns>
		public override string NewLine
		{
			get
			{
				return this._writer.NewLine;
			}
			set
			{
				this._writer.NewLine = value;
			}
		}

		/// <summary>Gets or sets the number of spaces to indent.</summary>
		/// <returns>The number of spaces to indent.</returns>
		public int Indent
		{
			get
			{
				return this._indentLevel;
			}
			set
			{
				this._indentLevel = Math.Max(value, 0);
			}
		}

		/// <summary>Gets the <see cref="T:System.IO.TextWriter" /> to use.</summary>
		/// <returns>The <see cref="T:System.IO.TextWriter" /> to use.</returns>
		public TextWriter InnerWriter
		{
			get
			{
				return this._writer;
			}
		}

		/// <summary>Closes the document being written to.</summary>
		public override void Close()
		{
			this._writer.Close();
		}

		/// <summary>Flushes the stream.</summary>
		public override void Flush()
		{
			this._writer.Flush();
		}

		/// <summary>Outputs the tab string once for each level of indentation according to the <see cref="P:System.CodeDom.Compiler.IndentedTextWriter.Indent" /> property.</summary>
		protected virtual void OutputTabs()
		{
			if (this._tabsPending)
			{
				for (int i = 0; i < this._indentLevel; i++)
				{
					this._writer.Write(this._tabString);
				}
				this._tabsPending = false;
			}
		}

		/// <summary>Writes the specified string to the text stream.</summary>
		/// <param name="s">The string to write.</param>
		public override void Write(string s)
		{
			this.OutputTabs();
			this._writer.Write(s);
		}

		/// <summary>Writes the text representation of a Boolean value to the text stream.</summary>
		/// <param name="value">The Boolean value to write.</param>
		public override void Write(bool value)
		{
			this.OutputTabs();
			this._writer.Write(value);
		}

		/// <summary>Writes a character to the text stream.</summary>
		/// <param name="value">The character to write.</param>
		public override void Write(char value)
		{
			this.OutputTabs();
			this._writer.Write(value);
		}

		/// <summary>Writes a character array to the text stream.</summary>
		/// <param name="buffer">The character array to write.</param>
		public override void Write(char[] buffer)
		{
			this.OutputTabs();
			this._writer.Write(buffer);
		}

		/// <summary>Writes a subarray of characters to the text stream.</summary>
		/// <param name="buffer">The character array to write data from.</param>
		/// <param name="index">Starting index in the buffer.</param>
		/// <param name="count">The number of characters to write.</param>
		public override void Write(char[] buffer, int index, int count)
		{
			this.OutputTabs();
			this._writer.Write(buffer, index, count);
		}

		/// <summary>Writes the text representation of a Double to the text stream.</summary>
		/// <param name="value">The <see langword="double" /> to write.</param>
		public override void Write(double value)
		{
			this.OutputTabs();
			this._writer.Write(value);
		}

		/// <summary>Writes the text representation of a Single to the text stream.</summary>
		/// <param name="value">The <see langword="single" /> to write.</param>
		public override void Write(float value)
		{
			this.OutputTabs();
			this._writer.Write(value);
		}

		/// <summary>Writes the text representation of an integer to the text stream.</summary>
		/// <param name="value">The integer to write.</param>
		public override void Write(int value)
		{
			this.OutputTabs();
			this._writer.Write(value);
		}

		/// <summary>Writes the text representation of an 8-byte integer to the text stream.</summary>
		/// <param name="value">The 8-byte integer to write.</param>
		public override void Write(long value)
		{
			this.OutputTabs();
			this._writer.Write(value);
		}

		/// <summary>Writes the text representation of an object to the text stream.</summary>
		/// <param name="value">The object to write.</param>
		public override void Write(object value)
		{
			this.OutputTabs();
			this._writer.Write(value);
		}

		/// <summary>Writes out a formatted string, using the same semantics as specified.</summary>
		/// <param name="format">The formatting string.</param>
		/// <param name="arg0">The object to write into the formatted string.</param>
		public override void Write(string format, object arg0)
		{
			this.OutputTabs();
			this._writer.Write(format, arg0);
		}

		/// <summary>Writes out a formatted string, using the same semantics as specified.</summary>
		/// <param name="format">The formatting string to use.</param>
		/// <param name="arg0">The first object to write into the formatted string.</param>
		/// <param name="arg1">The second object to write into the formatted string.</param>
		public override void Write(string format, object arg0, object arg1)
		{
			this.OutputTabs();
			this._writer.Write(format, arg0, arg1);
		}

		/// <summary>Writes out a formatted string, using the same semantics as specified.</summary>
		/// <param name="format">The formatting string to use.</param>
		/// <param name="arg">The argument array to output.</param>
		public override void Write(string format, params object[] arg)
		{
			this.OutputTabs();
			this._writer.Write(format, arg);
		}

		/// <summary>Writes the specified string to a line without tabs.</summary>
		/// <param name="s">The string to write.</param>
		public void WriteLineNoTabs(string s)
		{
			this._writer.WriteLine(s);
		}

		/// <summary>Writes the specified string, followed by a line terminator, to the text stream.</summary>
		/// <param name="s">The string to write.</param>
		public override void WriteLine(string s)
		{
			this.OutputTabs();
			this._writer.WriteLine(s);
			this._tabsPending = true;
		}

		/// <summary>Writes a line terminator.</summary>
		public override void WriteLine()
		{
			this.OutputTabs();
			this._writer.WriteLine();
			this._tabsPending = true;
		}

		/// <summary>Writes the text representation of a Boolean, followed by a line terminator, to the text stream.</summary>
		/// <param name="value">The Boolean to write.</param>
		public override void WriteLine(bool value)
		{
			this.OutputTabs();
			this._writer.WriteLine(value);
			this._tabsPending = true;
		}

		/// <summary>Writes a character, followed by a line terminator, to the text stream.</summary>
		/// <param name="value">The character to write.</param>
		public override void WriteLine(char value)
		{
			this.OutputTabs();
			this._writer.WriteLine(value);
			this._tabsPending = true;
		}

		/// <summary>Writes a character array, followed by a line terminator, to the text stream.</summary>
		/// <param name="buffer">The character array to write.</param>
		public override void WriteLine(char[] buffer)
		{
			this.OutputTabs();
			this._writer.WriteLine(buffer);
			this._tabsPending = true;
		}

		/// <summary>Writes a subarray of characters, followed by a line terminator, to the text stream.</summary>
		/// <param name="buffer">The character array to write data from.</param>
		/// <param name="index">Starting index in the buffer.</param>
		/// <param name="count">The number of characters to write.</param>
		public override void WriteLine(char[] buffer, int index, int count)
		{
			this.OutputTabs();
			this._writer.WriteLine(buffer, index, count);
			this._tabsPending = true;
		}

		/// <summary>Writes the text representation of a Double, followed by a line terminator, to the text stream.</summary>
		/// <param name="value">The <see langword="double" /> to write.</param>
		public override void WriteLine(double value)
		{
			this.OutputTabs();
			this._writer.WriteLine(value);
			this._tabsPending = true;
		}

		/// <summary>Writes the text representation of a Single, followed by a line terminator, to the text stream.</summary>
		/// <param name="value">The <see langword="single" /> to write.</param>
		public override void WriteLine(float value)
		{
			this.OutputTabs();
			this._writer.WriteLine(value);
			this._tabsPending = true;
		}

		/// <summary>Writes the text representation of an integer, followed by a line terminator, to the text stream.</summary>
		/// <param name="value">The integer to write.</param>
		public override void WriteLine(int value)
		{
			this.OutputTabs();
			this._writer.WriteLine(value);
			this._tabsPending = true;
		}

		/// <summary>Writes the text representation of an 8-byte integer, followed by a line terminator, to the text stream.</summary>
		/// <param name="value">The 8-byte integer to write.</param>
		public override void WriteLine(long value)
		{
			this.OutputTabs();
			this._writer.WriteLine(value);
			this._tabsPending = true;
		}

		/// <summary>Writes the text representation of an object, followed by a line terminator, to the text stream.</summary>
		/// <param name="value">The object to write.</param>
		public override void WriteLine(object value)
		{
			this.OutputTabs();
			this._writer.WriteLine(value);
			this._tabsPending = true;
		}

		/// <summary>Writes out a formatted string, followed by a line terminator, using the same semantics as specified.</summary>
		/// <param name="format">The formatting string.</param>
		/// <param name="arg0">The object to write into the formatted string.</param>
		public override void WriteLine(string format, object arg0)
		{
			this.OutputTabs();
			this._writer.WriteLine(format, arg0);
			this._tabsPending = true;
		}

		/// <summary>Writes out a formatted string, followed by a line terminator, using the same semantics as specified.</summary>
		/// <param name="format">The formatting string to use.</param>
		/// <param name="arg0">The first object to write into the formatted string.</param>
		/// <param name="arg1">The second object to write into the formatted string.</param>
		public override void WriteLine(string format, object arg0, object arg1)
		{
			this.OutputTabs();
			this._writer.WriteLine(format, arg0, arg1);
			this._tabsPending = true;
		}

		/// <summary>Writes out a formatted string, followed by a line terminator, using the same semantics as specified.</summary>
		/// <param name="format">The formatting string to use.</param>
		/// <param name="arg">The argument array to output.</param>
		public override void WriteLine(string format, params object[] arg)
		{
			this.OutputTabs();
			this._writer.WriteLine(format, arg);
			this._tabsPending = true;
		}

		/// <summary>Writes the text representation of a UInt32, followed by a line terminator, to the text stream.</summary>
		/// <param name="value">A UInt32 to output.</param>
		[CLSCompliant(false)]
		public override void WriteLine(uint value)
		{
			this.OutputTabs();
			this._writer.WriteLine(value);
			this._tabsPending = true;
		}

		private readonly TextWriter _writer;

		private readonly string _tabString;

		private int _indentLevel;

		private bool _tabsPending;

		/// <summary>Specifies the default tab string. This field is constant.</summary>
		public const string DefaultTabString = "    ";
	}
}
