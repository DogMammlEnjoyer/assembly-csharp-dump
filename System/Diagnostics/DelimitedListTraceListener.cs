using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Security.Permissions;
using System.Text;

namespace System.Diagnostics
{
	/// <summary>Directs tracing or debugging output to a text writer, such as a stream writer, or to a stream, such as a file stream.</summary>
	[HostProtection(SecurityAction.LinkDemand, Synchronization = true)]
	public class DelimitedListTraceListener : TextWriterTraceListener
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.DelimitedListTraceListener" /> class that writes to the specified output stream.</summary>
		/// <param name="stream">The <see cref="T:System.IO.Stream" /> to receive the output.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="stream" /> is <see langword="null" />.</exception>
		public DelimitedListTraceListener(Stream stream) : base(stream)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.DelimitedListTraceListener" /> class that writes to the specified output stream and has the specified name.</summary>
		/// <param name="stream">The <see cref="T:System.IO.Stream" /> to receive the output.</param>
		/// <param name="name">The name of the new instance of the trace listener.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="stream" /> is <see langword="null" />.</exception>
		public DelimitedListTraceListener(Stream stream, string name) : base(stream, name)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.DelimitedListTraceListener" /> class that writes to the specified text writer.</summary>
		/// <param name="writer">The <see cref="T:System.IO.TextWriter" /> to receive the output.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="writer" /> is <see langword="null" />.</exception>
		public DelimitedListTraceListener(TextWriter writer) : base(writer)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.DelimitedListTraceListener" /> class that writes to the specified text writer and has the specified name.</summary>
		/// <param name="writer">The <see cref="T:System.IO.TextWriter" /> to receive the output.</param>
		/// <param name="name">The name of the new instance of the trace listener.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="writer" /> is <see langword="null" />.</exception>
		public DelimitedListTraceListener(TextWriter writer, string name) : base(writer, name)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.DelimitedListTraceListener" /> class that writes to the specified file.</summary>
		/// <param name="fileName">The name of the file to receive the output.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="fileName" /> is <see langword="null" />.</exception>
		public DelimitedListTraceListener(string fileName) : base(fileName)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.DelimitedListTraceListener" /> class that writes to the specified file and has the specified name.</summary>
		/// <param name="fileName">The name of the file to receive the output.</param>
		/// <param name="name">The name of the new instance of the trace listener.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="fileName" /> is <see langword="null" />.</exception>
		public DelimitedListTraceListener(string fileName, string name) : base(fileName, name)
		{
		}

		/// <summary>Gets or sets the delimiter for the delimited list.</summary>
		/// <returns>The delimiter for the delimited list.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <see cref="P:System.Diagnostics.DelimitedListTraceListener.Delimiter" /> is set to <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <see cref="P:System.Diagnostics.DelimitedListTraceListener.Delimiter" /> is set to an empty string ("").</exception>
		public string Delimiter
		{
			get
			{
				lock (this)
				{
					if (!this.initializedDelim)
					{
						if (base.Attributes.ContainsKey("delimiter"))
						{
							this.delimiter = base.Attributes["delimiter"];
						}
						this.initializedDelim = true;
					}
				}
				return this.delimiter;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Delimiter");
				}
				if (value.Length == 0)
				{
					throw new ArgumentException(SR.GetString("Generic_ArgCantBeEmptyString", new object[]
					{
						"Delimiter"
					}));
				}
				lock (this)
				{
					this.delimiter = value;
					this.initializedDelim = true;
				}
				if (this.delimiter == ",")
				{
					this.secondaryDelim = ";";
					return;
				}
				this.secondaryDelim = ",";
			}
		}

		/// <summary>Returns the custom configuration file attribute supported by the delimited trace listener.</summary>
		/// <returns>A string array that contains the single value "delimiter".</returns>
		protected internal override string[] GetSupportedAttributes()
		{
			return new string[]
			{
				"delimiter"
			};
		}

		/// <summary>Writes trace information, a formatted array of objects, and event information to the output file or stream.</summary>
		/// <param name="eventCache">A <see cref="T:System.Diagnostics.TraceEventCache" /> object that contains the current process ID, thread ID, and stack trace information.</param>
		/// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
		/// <param name="eventType">One of the <see cref="T:System.Diagnostics.TraceEventType" /> values specifying the type of event that has caused the trace.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="format">A format string that contains zero or more format items that correspond to objects in the <paramref name="args" /> array.</param>
		/// <param name="args">An array containing zero or more objects to format.</param>
		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
		{
			if (base.Filter != null && !base.Filter.ShouldTrace(eventCache, source, eventType, id, format, args))
			{
				return;
			}
			this.WriteHeader(source, eventType, id);
			if (args != null)
			{
				this.WriteEscaped(string.Format(CultureInfo.InvariantCulture, format, args));
			}
			else
			{
				this.WriteEscaped(format);
			}
			this.Write(this.Delimiter);
			this.Write(this.Delimiter);
			this.WriteFooter(eventCache);
		}

		/// <summary>Writes trace information, a message, and event information to the output file or stream.</summary>
		/// <param name="eventCache">A <see cref="T:System.Diagnostics.TraceEventCache" /> object that contains the current process ID, thread ID, and stack trace information.</param>
		/// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
		/// <param name="eventType">One of the <see cref="T:System.Diagnostics.TraceEventType" /> values specifying the type of event that has caused the trace.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="message">The trace message to write to the output file or stream.</param>
		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
		{
			if (base.Filter != null && !base.Filter.ShouldTrace(eventCache, source, eventType, id, message))
			{
				return;
			}
			this.WriteHeader(source, eventType, id);
			this.WriteEscaped(message);
			this.Write(this.Delimiter);
			this.Write(this.Delimiter);
			this.WriteFooter(eventCache);
		}

		/// <summary>Writes trace information, a data object, and event information to the output file or stream.</summary>
		/// <param name="eventCache">A <see cref="T:System.Diagnostics.TraceEventCache" /> object that contains the current process ID, thread ID, and stack trace information.</param>
		/// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
		/// <param name="eventType">One of the <see cref="T:System.Diagnostics.TraceEventType" /> values specifying the type of event that has caused the trace.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="data">A data object to write to the output file or stream.</param>
		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
		{
			if (base.Filter != null && !base.Filter.ShouldTrace(eventCache, source, eventType, id, null, null, data))
			{
				return;
			}
			this.WriteHeader(source, eventType, id);
			this.Write(this.Delimiter);
			this.WriteEscaped(data.ToString());
			this.Write(this.Delimiter);
			this.WriteFooter(eventCache);
		}

		/// <summary>Writes trace information, an array of data objects, and event information to the output file or stream.</summary>
		/// <param name="eventCache">A <see cref="T:System.Diagnostics.TraceEventCache" /> object that contains the current process ID, thread ID, and stack trace information.</param>
		/// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
		/// <param name="eventType">One of the <see cref="T:System.Diagnostics.TraceEventType" /> values specifying the type of event that has caused the trace.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="data">An array of data objects to write to the output file or stream.</param>
		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
		{
			if (base.Filter != null && !base.Filter.ShouldTrace(eventCache, source, eventType, id, null, null, null, data))
			{
				return;
			}
			this.WriteHeader(source, eventType, id);
			this.Write(this.Delimiter);
			if (data != null)
			{
				for (int i = 0; i < data.Length; i++)
				{
					if (i != 0)
					{
						this.Write(this.secondaryDelim);
					}
					this.WriteEscaped(data[i].ToString());
				}
			}
			this.Write(this.Delimiter);
			this.WriteFooter(eventCache);
		}

		private void WriteHeader(string source, TraceEventType eventType, int id)
		{
			this.WriteEscaped(source);
			this.Write(this.Delimiter);
			this.Write(eventType.ToString());
			this.Write(this.Delimiter);
			this.Write(id.ToString(CultureInfo.InvariantCulture));
			this.Write(this.Delimiter);
		}

		private void WriteFooter(TraceEventCache eventCache)
		{
			if (eventCache != null)
			{
				if (base.IsEnabled(TraceOptions.ProcessId))
				{
					this.Write(eventCache.ProcessId.ToString(CultureInfo.InvariantCulture));
				}
				this.Write(this.Delimiter);
				if (base.IsEnabled(TraceOptions.LogicalOperationStack))
				{
					this.WriteStackEscaped(eventCache.LogicalOperationStack);
				}
				this.Write(this.Delimiter);
				if (base.IsEnabled(TraceOptions.ThreadId))
				{
					this.WriteEscaped(eventCache.ThreadId.ToString(CultureInfo.InvariantCulture));
				}
				this.Write(this.Delimiter);
				if (base.IsEnabled(TraceOptions.DateTime))
				{
					this.WriteEscaped(eventCache.DateTime.ToString("o", CultureInfo.InvariantCulture));
				}
				this.Write(this.Delimiter);
				if (base.IsEnabled(TraceOptions.Timestamp))
				{
					this.Write(eventCache.Timestamp.ToString(CultureInfo.InvariantCulture));
				}
				this.Write(this.Delimiter);
				if (base.IsEnabled(TraceOptions.Callstack))
				{
					this.WriteEscaped(eventCache.Callstack);
				}
			}
			else
			{
				for (int i = 0; i < 5; i++)
				{
					this.Write(this.Delimiter);
				}
			}
			this.WriteLine("");
		}

		private void WriteEscaped(string message)
		{
			if (!string.IsNullOrEmpty(message))
			{
				StringBuilder stringBuilder = new StringBuilder("\"");
				int num = 0;
				int num2;
				while ((num2 = message.IndexOf('"', num)) != -1)
				{
					stringBuilder.Append(message, num, num2 - num);
					stringBuilder.Append("\"\"");
					num = num2 + 1;
				}
				stringBuilder.Append(message, num, message.Length - num);
				stringBuilder.Append("\"");
				this.Write(stringBuilder.ToString());
			}
		}

		private void WriteStackEscaped(Stack stack)
		{
			StringBuilder stringBuilder = new StringBuilder("\"");
			bool flag = true;
			foreach (object obj in stack)
			{
				if (!flag)
				{
					stringBuilder.Append(", ");
				}
				else
				{
					flag = false;
				}
				string text = obj.ToString();
				int num = 0;
				int num2;
				while ((num2 = text.IndexOf('"', num)) != -1)
				{
					stringBuilder.Append(text, num, num2 - num);
					stringBuilder.Append("\"\"");
					num = num2 + 1;
				}
				stringBuilder.Append(text, num, text.Length - num);
			}
			stringBuilder.Append("\"");
			this.Write(stringBuilder.ToString());
		}

		private string delimiter = ";";

		private string secondaryDelim = ",";

		private bool initializedDelim;
	}
}
