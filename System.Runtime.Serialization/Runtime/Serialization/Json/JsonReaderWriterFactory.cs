using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace System.Runtime.Serialization.Json
{
	/// <summary>Produces instances of <see cref="T:System.Xml.XmlDictionaryReader" /> that can read data encoded with JavaScript Object Notation (JSON) from a stream or buffer and map it to an XML Infoset and instances of <see cref="T:System.Xml.XmlDictionaryWriter" /> that can map an XML Infoset to JSON and write JSON-encoded data to a stream.</summary>
	[TypeForwardedFrom("System.ServiceModel.Web, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")]
	public static class JsonReaderWriterFactory
	{
		/// <summary>Creates an <see cref="T:System.Xml.XmlDictionaryReader" /> that can map streams encoded with JavaScript Object Notation (JSON) to an XML Infoset.</summary>
		/// <param name="stream">The input <see cref="T:System.IO.Stream" /> from which to read.</param>
		/// <param name="quotas">The <see cref="T:System.Xml.XmlDictionaryReaderQuotas" /> used to prevent Denial of Service attacks when reading untrusted data.</param>
		/// <returns>An <see cref="T:System.Xml.XmlDictionaryReader" /> that can read JavaScript Object Notation (JSON).</returns>
		public static XmlDictionaryReader CreateJsonReader(Stream stream, XmlDictionaryReaderQuotas quotas)
		{
			return JsonReaderWriterFactory.CreateJsonReader(stream, null, quotas, null);
		}

		/// <summary>Creates an <see cref="T:System.Xml.XmlDictionaryReader" /> that can map buffers encoded with JavaScript Object Notation (JSON) to an XML Infoset.</summary>
		/// <param name="buffer">The input <see cref="T:System.Byte" /> buffer array from which to read.</param>
		/// <param name="quotas">The <see cref="T:System.Xml.XmlDictionaryReaderQuotas" /> used to prevent Denial of Service attacks when reading untrusted data.</param>
		/// <returns>An <see cref="T:System.Xml.XmlDictionaryReader" /> that can process JavaScript Object Notation (JSON) data.</returns>
		public static XmlDictionaryReader CreateJsonReader(byte[] buffer, XmlDictionaryReaderQuotas quotas)
		{
			if (buffer == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("buffer");
			}
			return JsonReaderWriterFactory.CreateJsonReader(buffer, 0, buffer.Length, null, quotas, null);
		}

		/// <summary>Creates an <see cref="T:System.Xml.XmlDictionaryReader" /> that can map streams encoded with JavaScript Object Notation (JSON), of a specified size and offset, to an XML Infoset.</summary>
		/// <param name="stream">The input <see cref="T:System.IO.Stream" /> from which to read.</param>
		/// <param name="encoding">The <see cref="T:System.Text.Encoding" /> that specifies the character encoding used by the reader. If <see langword="null" /> is specified as the value, the reader attempts to auto-detect the encoding.</param>
		/// <param name="quotas">The <see cref="T:System.Xml.XmlDictionaryReaderQuotas" /> used to prevent Denial of Service attacks when reading untrusted data.</param>
		/// <param name="onClose">The <see cref="T:System.Xml.OnXmlDictionaryReaderClose" /> delegate to call when the reader is closed.</param>
		/// <returns>An <see cref="T:System.Xml.XmlDictionaryReader" /> that can read JavaScript Object Notation (JSON).</returns>
		public static XmlDictionaryReader CreateJsonReader(Stream stream, Encoding encoding, XmlDictionaryReaderQuotas quotas, OnXmlDictionaryReaderClose onClose)
		{
			XmlJsonReader xmlJsonReader = new XmlJsonReader();
			xmlJsonReader.SetInput(stream, encoding, quotas, onClose);
			return xmlJsonReader;
		}

		/// <summary>Creates an <see cref="T:System.Xml.XmlDictionaryReader" /> that can map buffers encoded with JavaScript Object Notation (JSON), of a specified size and offset, to an XML Infoset.</summary>
		/// <param name="buffer">The input <see cref="T:System.Byte" /> buffer array from which to read.</param>
		/// <param name="offset">Starting position from which to read in <paramref name="buffer" />.</param>
		/// <param name="count">Number of bytes that can be read from <paramref name="buffer" />.</param>
		/// <param name="quotas">The <see cref="T:System.Xml.XmlDictionaryReaderQuotas" /> used to prevent Denial of Service attacks when reading untrusted data.</param>
		/// <returns>An <see cref="T:System.Xml.XmlDictionaryReader" /> that can read JavaScript Object Notation (JSON).</returns>
		public static XmlDictionaryReader CreateJsonReader(byte[] buffer, int offset, int count, XmlDictionaryReaderQuotas quotas)
		{
			return JsonReaderWriterFactory.CreateJsonReader(buffer, offset, count, null, quotas, null);
		}

		/// <summary>Creates an <see cref="T:System.Xml.XmlDictionaryReader" /> that can map buffers encoded with JavaScript Object Notation (JSON), with a specified size and offset and character encoding, to an XML Infoset.</summary>
		/// <param name="buffer">The input <see cref="T:System.Byte" /> buffer array from which to read.</param>
		/// <param name="offset">Starting position from which to read in <paramref name="buffer" />.</param>
		/// <param name="count">Number of bytes that can be read from <paramref name="buffer" />.</param>
		/// <param name="encoding">The <see cref="T:System.Text.Encoding" /> that specifies the character encoding used by the reader. If <see langword="null" /> is specified as the value, the reader attempts to auto-detect the encoding.</param>
		/// <param name="quotas">The <see cref="T:System.Xml.XmlDictionaryReaderQuotas" /> used to prevent Denial of Service attacks when reading untrusted data.</param>
		/// <param name="onClose">The <see cref="T:System.Xml.OnXmlDictionaryReaderClose" /> delegate to call when the reader is closed. The default value is <see langword="null" />.</param>
		/// <returns>An <see cref="T:System.Xml.XmlDictionaryReader" /> that can read JavaScript Object Notation (JSON).</returns>
		public static XmlDictionaryReader CreateJsonReader(byte[] buffer, int offset, int count, Encoding encoding, XmlDictionaryReaderQuotas quotas, OnXmlDictionaryReaderClose onClose)
		{
			XmlJsonReader xmlJsonReader = new XmlJsonReader();
			xmlJsonReader.SetInput(buffer, offset, count, encoding, quotas, onClose);
			return xmlJsonReader;
		}

		/// <summary>Creates an <see cref="T:System.Xml.XmlDictionaryWriter" /> that writes data encoded with JSON to a stream.</summary>
		/// <param name="stream">The output <see cref="T:System.IO.Stream" /> for the JSON writer.</param>
		/// <returns>An <see cref="T:System.Xml.XmlDictionaryWriter" /> that writes data encoded with JSON to the stream based on an XML Infoset.</returns>
		public static XmlDictionaryWriter CreateJsonWriter(Stream stream)
		{
			return JsonReaderWriterFactory.CreateJsonWriter(stream, Encoding.UTF8, true);
		}

		/// <summary>Creates an <see cref="T:System.Xml.XmlDictionaryWriter" /> that writes data encoded with JSON to a stream with a specified character encoding.</summary>
		/// <param name="stream">The output <see cref="T:System.IO.Stream" /> for the JSON writer.</param>
		/// <param name="encoding">The <see cref="T:System.Text.Encoding" /> that specifies the character encoding used by the writer. The default encoding is UTF-8.</param>
		/// <returns>An <see cref="T:System.Xml.XmlDictionaryWriter" /> that writes data encoded with JSON to the stream based on an XML Infoset.</returns>
		public static XmlDictionaryWriter CreateJsonWriter(Stream stream, Encoding encoding)
		{
			return JsonReaderWriterFactory.CreateJsonWriter(stream, encoding, true);
		}

		/// <summary>Creates an <see cref="T:System.Xml.XmlDictionaryWriter" /> that writes data encoded with JSON to a stream with a specified character encoding.</summary>
		/// <param name="stream">The output <see cref="T:System.IO.Stream" /> for the JSON writer.</param>
		/// <param name="encoding">The <see cref="T:System.Text.Encoding" /> that specifies the character encoding used by the writer. The default encoding is UTF-8.</param>
		/// <param name="ownsStream">If <see langword="true" />, the output stream is closed by the writer when done; otherwise <see langword="false" />. The default value is <see langword="true" />.</param>
		/// <returns>An <see cref="T:System.Xml.XmlDictionaryWriter" /> that writes data encoded with JSON to the stream based on an XML Infoset.</returns>
		public static XmlDictionaryWriter CreateJsonWriter(Stream stream, Encoding encoding, bool ownsStream)
		{
			return JsonReaderWriterFactory.CreateJsonWriter(stream, encoding, ownsStream, false);
		}

		/// <summary>Creates an <see cref="T:System.Xml.XmlDictionaryWriter" /> that writes data encoded with JSON to a stream with a specified character.</summary>
		/// <param name="stream">The output <see cref="T:System.IO.Stream" /> for the JSON writer.</param>
		/// <param name="encoding">The <see cref="T:System.Text.Encoding" /> that specifies the character encoding used by the writer. The default encoding is UTF-8.</param>
		/// <param name="ownsStream">If <see langword="true" />, the output stream is closed by the writer when done; otherwise <see langword="false" />. The default value is <see langword="true" />.</param>
		/// <param name="indent">If <see langword="true" />, the output uses multiline format, indenting each level properly; otherwise, <see langword="false" />.</param>
		/// <returns>An <see cref="T:System.Xml.XmlDictionaryWriter" /> that writes data encoded with JSON to the stream based on an XML Infoset.</returns>
		public static XmlDictionaryWriter CreateJsonWriter(Stream stream, Encoding encoding, bool ownsStream, bool indent)
		{
			return JsonReaderWriterFactory.CreateJsonWriter(stream, encoding, ownsStream, indent, "  ");
		}

		/// <summary>Creates an <see cref="T:System.Xml.XmlDictionaryWriter" /> that writes data encoded with JSON to a stream with a specified character.</summary>
		/// <param name="stream">The output <see cref="T:System.IO.Stream" /> for the JSON writer.</param>
		/// <param name="encoding">The <see cref="T:System.Text.Encoding" /> that specifies the character encoding used by the writer. The default encoding is UTF-8.</param>
		/// <param name="ownsStream">If <see langword="true" />, the output stream is closed by the writer when done; otherwise <see langword="false" />. The default value is <see langword="true" />.</param>
		/// <param name="indent">If <see langword="true" />, the output uses multiline format, indenting each level properly; otherwise, <see langword="false" />.</param>
		/// <param name="indentChars">The string used to indent each level.</param>
		/// <returns>An <see cref="T:System.Xml.XmlDictionaryWriter" /> that writes data encoded with JSON to the stream based on an XML Infoset.</returns>
		public static XmlDictionaryWriter CreateJsonWriter(Stream stream, Encoding encoding, bool ownsStream, bool indent, string indentChars)
		{
			XmlJsonWriter xmlJsonWriter = new XmlJsonWriter(indent, indentChars);
			xmlJsonWriter.SetOutput(stream, encoding, ownsStream);
			return xmlJsonWriter;
		}

		private const string DefaultIndentChars = "  ";
	}
}
