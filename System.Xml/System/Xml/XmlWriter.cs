using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml.XPath;

namespace System.Xml
{
	/// <summary>Represents a writer that provides a fast, non-cached, forward-only way to generate streams or files that contain XML data.</summary>
	public abstract class XmlWriter : IDisposable
	{
		/// <summary>Gets the <see cref="T:System.Xml.XmlWriterSettings" /> object used to create this <see cref="T:System.Xml.XmlWriter" /> instance.</summary>
		/// <returns>The <see cref="T:System.Xml.XmlWriterSettings" /> object used to create this writer instance. If this writer was not created using the <see cref="Overload:System.Xml.XmlWriter.Create" /> method, this property returns <see langword="null" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual XmlWriterSettings Settings
		{
			get
			{
				return null;
			}
		}

		/// <summary>When overridden in a derived class, writes the XML declaration with the version "1.0".</summary>
		/// <exception cref="T:System.InvalidOperationException">This is not the first write method called after the constructor.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract void WriteStartDocument();

		/// <summary>When overridden in a derived class, writes the XML declaration with the version "1.0" and the standalone attribute.</summary>
		/// <param name="standalone">If <see langword="true" />, it writes "standalone=yes"; if <see langword="false" />, it writes "standalone=no".</param>
		/// <exception cref="T:System.InvalidOperationException">This is not the first write method called after the constructor. </exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract void WriteStartDocument(bool standalone);

		/// <summary>When overridden in a derived class, closes any open elements or attributes and puts the writer back in the Start state.</summary>
		/// <exception cref="T:System.ArgumentException">The XML document is invalid.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract void WriteEndDocument();

		/// <summary>When overridden in a derived class, writes the DOCTYPE declaration with the specified name and optional attributes.</summary>
		/// <param name="name">The name of the DOCTYPE. This must be non-empty.</param>
		/// <param name="pubid">If non-null it also writes PUBLIC "pubid" "sysid" where <paramref name="pubid" /> and <paramref name="sysid" /> are replaced with the value of the given arguments.</param>
		/// <param name="sysid">If <paramref name="pubid" /> is <see langword="null" /> and <paramref name="sysid" /> is non-null it writes SYSTEM "sysid" where <paramref name="sysid" /> is replaced with the value of this argument.</param>
		/// <param name="subset">If non-null it writes [subset] where subset is replaced with the value of this argument.</param>
		/// <exception cref="T:System.InvalidOperationException">This method was called outside the prolog (after the root element). </exception>
		/// <exception cref="T:System.ArgumentException">The value for <paramref name="name" /> would result in invalid XML.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract void WriteDocType(string name, string pubid, string sysid, string subset);

		/// <summary>When overridden in a derived class, writes the specified start tag and associates it with the given namespace.</summary>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="ns">The namespace URI to associate with the element. If this namespace is already in scope and has an associated prefix, the writer automatically writes that prefix also.</param>
		/// <exception cref="T:System.InvalidOperationException">The writer is closed.</exception>
		/// <exception cref="T:System.Text.EncoderFallbackException">There is a character in the buffer that is a valid XML character but is not valid for the output encoding. For example, if the output encoding is ASCII, you should only use characters from the range of 0 to 127 for element and attribute names. The invalid character might be in the argument of this method or in an argument of previous methods that were writing to the buffer. Such characters are escaped by character entity references when possible (for example, in text nodes or attribute values). However, the character entity reference is not allowed in element and attribute names, comments, processing instructions, or CDATA sections.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public void WriteStartElement(string localName, string ns)
		{
			this.WriteStartElement(null, localName, ns);
		}

		/// <summary>When overridden in a derived class, writes the specified start tag and associates it with the given namespace and prefix.</summary>
		/// <param name="prefix">The namespace prefix of the element.</param>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="ns">The namespace URI to associate with the element.</param>
		/// <exception cref="T:System.InvalidOperationException">The writer is closed.</exception>
		/// <exception cref="T:System.Text.EncoderFallbackException">There is a character in the buffer that is a valid XML character but is not valid for the output encoding. For example, if the output encoding is ASCII, you should only use characters from the range of 0 to 127 for element and attribute names. The invalid character might be in the argument of this method or in an argument of previous methods that were writing to the buffer. Such characters are escaped by character entity references when possible (for example, in text nodes or attribute values). However, the character entity reference is not allowed in element and attribute names, comments, processing instructions, or CDATA sections.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract void WriteStartElement(string prefix, string localName, string ns);

		/// <summary>When overridden in a derived class, writes out a start tag with the specified local name.</summary>
		/// <param name="localName">The local name of the element.</param>
		/// <exception cref="T:System.InvalidOperationException">The writer is closed.</exception>
		/// <exception cref="T:System.Text.EncoderFallbackException">There is a character in the buffer that is a valid XML character but is not valid for the output encoding. For example, if the output encoding is ASCII, you should only use characters from the range of 0 to 127 for element and attribute names. The invalid character might be in the argument of this method or in an argument of previous methods that were writing to the buffer. Such characters are escaped by character entity references when possible (for example, in text nodes or attribute values). However, the character entity reference is not allowed in element and attribute names, comments, processing instructions, or CDATA sections. </exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public void WriteStartElement(string localName)
		{
			this.WriteStartElement(null, localName, null);
		}

		/// <summary>When overridden in a derived class, closes one element and pops the corresponding namespace scope.</summary>
		/// <exception cref="T:System.InvalidOperationException">This results in an invalid XML document.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract void WriteEndElement();

		/// <summary>When overridden in a derived class, closes one element and pops the corresponding namespace scope.</summary>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract void WriteFullEndElement();

		/// <summary>When overridden in a derived class, writes an attribute with the specified local name, namespace URI, and value.</summary>
		/// <param name="localName">The local name of the attribute.</param>
		/// <param name="ns">The namespace URI to associate with the attribute.</param>
		/// <param name="value">The value of the attribute.</param>
		/// <exception cref="T:System.InvalidOperationException">The state of writer is not <see langword="WriteState.Element" /> or writer is closed. </exception>
		/// <exception cref="T:System.ArgumentException">The <see langword="xml:space" /> or <see langword="xml:lang" /> attribute value is invalid. </exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public void WriteAttributeString(string localName, string ns, string value)
		{
			this.WriteStartAttribute(null, localName, ns);
			this.WriteString(value);
			this.WriteEndAttribute();
		}

		/// <summary>When overridden in a derived class, writes out the attribute with the specified local name and value.</summary>
		/// <param name="localName">The local name of the attribute.</param>
		/// <param name="value">The value of the attribute.</param>
		/// <exception cref="T:System.InvalidOperationException">The state of writer is not <see langword="WriteState.Element" /> or writer is closed. </exception>
		/// <exception cref="T:System.ArgumentException">The <see langword="xml:space" /> or <see langword="xml:lang" /> attribute value is invalid. </exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public void WriteAttributeString(string localName, string value)
		{
			this.WriteStartAttribute(null, localName, null);
			this.WriteString(value);
			this.WriteEndAttribute();
		}

		/// <summary>When overridden in a derived class, writes out the attribute with the specified prefix, local name, namespace URI, and value.</summary>
		/// <param name="prefix">The namespace prefix of the attribute.</param>
		/// <param name="localName">The local name of the attribute.</param>
		/// <param name="ns">The namespace URI of the attribute.</param>
		/// <param name="value">The value of the attribute.</param>
		/// <exception cref="T:System.InvalidOperationException">The state of writer is not <see langword="WriteState.Element" /> or writer is closed. </exception>
		/// <exception cref="T:System.ArgumentException">The <see langword="xml:space" /> or <see langword="xml:lang" /> attribute value is invalid. </exception>
		/// <exception cref="T:System.Xml.XmlException">The <paramref name="localName" /> or <paramref name="ns" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public void WriteAttributeString(string prefix, string localName, string ns, string value)
		{
			this.WriteStartAttribute(prefix, localName, ns);
			this.WriteString(value);
			this.WriteEndAttribute();
		}

		/// <summary>Writes the start of an attribute with the specified local name and namespace URI.</summary>
		/// <param name="localName">The local name of the attribute.</param>
		/// <param name="ns">The namespace URI of the attribute.</param>
		/// <exception cref="T:System.Text.EncoderFallbackException">There is a character in the buffer that is a valid XML character but is not valid for the output encoding. For example, if the output encoding is ASCII, you should only use characters from the range of 0 to 127 for element and attribute names. The invalid character might be in the argument of this method or in an argument of previous methods that were writing to the buffer. Such characters are escaped by character entity references when possible (for example, in text nodes or attribute values). However, the character entity reference is not allowed in element and attribute names, comments, processing instructions, or CDATA sections.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public void WriteStartAttribute(string localName, string ns)
		{
			this.WriteStartAttribute(null, localName, ns);
		}

		/// <summary>When overridden in a derived class, writes the start of an attribute with the specified prefix, local name, and namespace URI.</summary>
		/// <param name="prefix">The namespace prefix of the attribute.</param>
		/// <param name="localName">The local name of the attribute.</param>
		/// <param name="ns">The namespace URI for the attribute.</param>
		/// <exception cref="T:System.Text.EncoderFallbackException">There is a character in the buffer that is a valid XML character but is not valid for the output encoding. For example, if the output encoding is ASCII, you should only use characters from the range of 0 to 127 for element and attribute names. The invalid character might be in the argument of this method or in an argument of previous methods that were writing to the buffer. Such characters are escaped by character entity references when possible (for example, in text nodes or attribute values). However, the character entity reference is not allowed in element and attribute names, comments, processing instructions, or CDATA sections. </exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract void WriteStartAttribute(string prefix, string localName, string ns);

		/// <summary>Writes the start of an attribute with the specified local name.</summary>
		/// <param name="localName">The local name of the attribute.</param>
		/// <exception cref="T:System.InvalidOperationException">The writer is closed.</exception>
		/// <exception cref="T:System.Text.EncoderFallbackException">There is a character in the buffer that is a valid XML character but is not valid for the output encoding. For example, if the output encoding is ASCII, you should only use characters from the range of 0 to 127 for element and attribute names. The invalid character might be in the argument of this method or in an argument of previous methods that were writing to the buffer. Such characters are escaped by character entity references when possible (for example, in text nodes or attribute values). However, the character entity reference is not allowed in element and attribute names, comments, processing instructions, or CDATA sections.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public void WriteStartAttribute(string localName)
		{
			this.WriteStartAttribute(null, localName, null);
		}

		/// <summary>When overridden in a derived class, closes the previous <see cref="M:System.Xml.XmlWriter.WriteStartAttribute(System.String,System.String)" /> call.</summary>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract void WriteEndAttribute();

		/// <summary>When overridden in a derived class, writes out a &lt;![CDATA[...]]&gt; block containing the specified text.</summary>
		/// <param name="text">The text to place inside the CDATA block.</param>
		/// <exception cref="T:System.ArgumentException">The text would result in a non-well formed XML document.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract void WriteCData(string text);

		/// <summary>When overridden in a derived class, writes out a comment &lt;!--...--&gt; containing the specified text.</summary>
		/// <param name="text">Text to place inside the comment.</param>
		/// <exception cref="T:System.ArgumentException">The text would result in a non-well-formed XML document.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract void WriteComment(string text);

		/// <summary>When overridden in a derived class, writes out a processing instruction with a space between the name and text as follows: &lt;?name text?&gt;.</summary>
		/// <param name="name">The name of the processing instruction.</param>
		/// <param name="text">The text to include in the processing instruction.</param>
		/// <exception cref="T:System.ArgumentException">The text would result in a non-well formed XML document.
		///         <paramref name="name" /> is either <see langword="null" /> or <see langword="String.Empty" />.This method is being used to create an XML declaration after <see cref="M:System.Xml.XmlWriter.WriteStartDocument" /> has already been called. </exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract void WriteProcessingInstruction(string name, string text);

		/// <summary>When overridden in a derived class, writes out an entity reference as <see langword="&amp;name;" />.</summary>
		/// <param name="name">The name of the entity reference.</param>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="name" /> is either <see langword="null" /> or <see langword="String.Empty" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract void WriteEntityRef(string name);

		/// <summary>When overridden in a derived class, forces the generation of a character entity for the specified Unicode character value.</summary>
		/// <param name="ch">The Unicode character for which to generate a character entity.</param>
		/// <exception cref="T:System.ArgumentException">The character is in the surrogate pair character range, <see langword="0xd800" /> - <see langword="0xdfff" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract void WriteCharEntity(char ch);

		/// <summary>When overridden in a derived class, writes out the given white space.</summary>
		/// <param name="ws">The string of white space characters.</param>
		/// <exception cref="T:System.ArgumentException">The string contains non-white space characters.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract void WriteWhitespace(string ws);

		/// <summary>When overridden in a derived class, writes the given text content.</summary>
		/// <param name="text">The text to write.</param>
		/// <exception cref="T:System.ArgumentException">The text string contains an invalid surrogate pair.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract void WriteString(string text);

		/// <summary>When overridden in a derived class, generates and writes the surrogate character entity for the surrogate character pair.</summary>
		/// <param name="lowChar">The low surrogate. This must be a value between 0xDC00 and 0xDFFF.</param>
		/// <param name="highChar">The high surrogate. This must be a value between 0xD800 and 0xDBFF.</param>
		/// <exception cref="T:System.ArgumentException">An invalid surrogate character pair was passed.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract void WriteSurrogateCharEntity(char lowChar, char highChar);

		/// <summary>When overridden in a derived class, writes text one buffer at a time.</summary>
		/// <param name="buffer">Character array containing the text to write.</param>
		/// <param name="index">The position in the buffer indicating the start of the text to write.</param>
		/// <param name="count">The number of characters to write.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///         <paramref name="index" /> or <paramref name="count" /> is less than zero.-or-The buffer length minus <paramref name="index" /> is less than <paramref name="count" />; the call results in surrogate pair characters being split or an invalid surrogate pair being written.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="buffer" /> parameter value is not valid.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract void WriteChars(char[] buffer, int index, int count);

		/// <summary>When overridden in a derived class, writes raw markup manually from a character buffer.</summary>
		/// <param name="buffer">Character array containing the text to write.</param>
		/// <param name="index">The position within the buffer indicating the start of the text to write.</param>
		/// <param name="count">The number of characters to write.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///         <paramref name="index" /> or <paramref name="count" /> is less than zero. -or-The buffer length minus <paramref name="index" /> is less than <paramref name="count" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract void WriteRaw(char[] buffer, int index, int count);

		/// <summary>When overridden in a derived class, writes raw markup manually from a string.</summary>
		/// <param name="data">String containing the text to write.</param>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="data" /> is either <see langword="null" /> or <see langword="String.Empty" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract void WriteRaw(string data);

		/// <summary>When overridden in a derived class, encodes the specified binary bytes as Base64 and writes out the resulting text.</summary>
		/// <param name="buffer">Byte array to encode.</param>
		/// <param name="index">The position in the buffer indicating the start of the bytes to write.</param>
		/// <param name="count">The number of bytes to write.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="buffer" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///         <paramref name="index" /> or <paramref name="count" /> is less than zero. -or-The buffer length minus <paramref name="index" /> is less than <paramref name="count" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract void WriteBase64(byte[] buffer, int index, int count);

		/// <summary>When overridden in a derived class, encodes the specified binary bytes as <see langword="BinHex" /> and writes out the resulting text.</summary>
		/// <param name="buffer">Byte array to encode.</param>
		/// <param name="index">The position in the buffer indicating the start of the bytes to write.</param>
		/// <param name="count">The number of bytes to write.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The writer is closed or in error state.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///         <paramref name="index" /> or <paramref name="count" /> is less than zero. -or-The buffer length minus <paramref name="index" /> is less than <paramref name="count" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual void WriteBinHex(byte[] buffer, int index, int count)
		{
			BinHexEncoder.Encode(buffer, index, count, this);
		}

		/// <summary>When overridden in a derived class, gets the state of the writer.</summary>
		/// <returns>One of the <see cref="T:System.Xml.WriteState" /> values.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract WriteState WriteState { get; }

		/// <summary>When overridden in a derived class, closes this stream and the underlying stream.</summary>
		/// <exception cref="T:System.InvalidOperationException">A call is made to write more output after <see langword="Close" /> has been called or the result of this call is an invalid XML document.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual void Close()
		{
		}

		/// <summary>When overridden in a derived class, flushes whatever is in the buffer to the underlying streams and also flushes the underlying stream.</summary>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract void Flush();

		/// <summary>When overridden in a derived class, returns the closest prefix defined in the current namespace scope for the namespace URI.</summary>
		/// <param name="ns">The namespace URI whose prefix you want to find.</param>
		/// <returns>The matching prefix or <see langword="null" /> if no matching namespace URI is found in the current scope.</returns>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="ns" /> is either <see langword="null" /> or <see langword="String.Empty" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract string LookupPrefix(string ns);

		/// <summary>When overridden in a derived class, gets an <see cref="T:System.Xml.XmlSpace" /> representing the current <see langword="xml:space" /> scope.</summary>
		/// <returns>An <see langword="XmlSpace" /> representing the current <see langword="xml:space" /> scope.Value Meaning 
		///             <see langword="None" />
		///           This is the default if no <see langword="xml:space" /> scope exists.
		///             <see langword="Default" />
		///           The current scope is <see langword="xml:space" />="default".
		///             <see langword="Preserve" />
		///           The current scope is <see langword="xml:space" />="preserve".</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual XmlSpace XmlSpace
		{
			get
			{
				return XmlSpace.Default;
			}
		}

		/// <summary>When overridden in a derived class, gets the current <see langword="xml:lang" /> scope.</summary>
		/// <returns>The current <see langword="xml:lang" /> scope.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual string XmlLang
		{
			get
			{
				return string.Empty;
			}
		}

		/// <summary>When overridden in a derived class, writes out the specified name, ensuring it is a valid NmToken according to the W3C XML 1.0 recommendation (http://www.w3.org/TR/1998/REC-xml-19980210#NT-Name).</summary>
		/// <param name="name">The name to write.</param>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="name" /> is not a valid NmToken; or <paramref name="name" /> is either <see langword="null" /> or <see langword="String.Empty" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual void WriteNmToken(string name)
		{
			if (name == null || name.Length == 0)
			{
				throw new ArgumentException(Res.GetString("The empty string '' is not a valid name."));
			}
			this.WriteString(XmlConvert.VerifyNMTOKEN(name, ExceptionType.ArgumentException));
		}

		/// <summary>When overridden in a derived class, writes out the specified name, ensuring it is a valid name according to the W3C XML 1.0 recommendation (http://www.w3.org/TR/1998/REC-xml-19980210#NT-Name).</summary>
		/// <param name="name">The name to write.</param>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="name" /> is not a valid XML name; or <paramref name="name" /> is either <see langword="null" /> or <see langword="String.Empty" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual void WriteName(string name)
		{
			this.WriteString(XmlConvert.VerifyQName(name, ExceptionType.ArgumentException));
		}

		/// <summary>When overridden in a derived class, writes out the namespace-qualified name. This method looks up the prefix that is in scope for the given namespace.</summary>
		/// <param name="localName">The local name to write.</param>
		/// <param name="ns">The namespace URI for the name.</param>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="localName" /> is either <see langword="null" /> or <see langword="String.Empty" />.
		///         <paramref name="localName" /> is not a valid name. </exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual void WriteQualifiedName(string localName, string ns)
		{
			if (ns != null && ns.Length > 0)
			{
				string text = this.LookupPrefix(ns);
				if (text == null)
				{
					throw new ArgumentException(Res.GetString("The '{0}' namespace is not defined.", new object[]
					{
						ns
					}));
				}
				this.WriteString(text);
				this.WriteString(":");
			}
			this.WriteString(localName);
		}

		/// <summary>Writes the object value.</summary>
		/// <param name="value">The object value to write.
		///       Note   With the release of the .NET Framework 3.5, this method accepts <see cref="T:System.DateTimeOffset" /> as a parameter.</param>
		/// <exception cref="T:System.ArgumentException">An invalid value was specified.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The writer is closed or in error state.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual void WriteValue(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.WriteString(XmlUntypedConverter.Untyped.ToString(value, null));
		}

		/// <summary>Writes a <see cref="T:System.String" /> value.</summary>
		/// <param name="value">The <see cref="T:System.String" /> value to write.</param>
		/// <exception cref="T:System.ArgumentException">An invalid value was specified.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual void WriteValue(string value)
		{
			if (value == null)
			{
				return;
			}
			this.WriteString(value);
		}

		/// <summary>Writes a <see cref="T:System.Boolean" /> value.</summary>
		/// <param name="value">The <see cref="T:System.Boolean" /> value to write.</param>
		/// <exception cref="T:System.ArgumentException">An invalid value was specified.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual void WriteValue(bool value)
		{
			this.WriteString(XmlConvert.ToString(value));
		}

		/// <summary>Writes a <see cref="T:System.DateTime" /> value.</summary>
		/// <param name="value">The <see cref="T:System.DateTime" /> value to write.</param>
		/// <exception cref="T:System.ArgumentException">An invalid value was specified.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual void WriteValue(DateTime value)
		{
			this.WriteString(XmlConvert.ToString(value, XmlDateTimeSerializationMode.RoundtripKind));
		}

		/// <summary>Writes a <see cref="T:System.DateTimeOffset" /> value.</summary>
		/// <param name="value">The <see cref="T:System.DateTimeOffset" /> value to write.</param>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual void WriteValue(DateTimeOffset value)
		{
			if (value.Offset != TimeSpan.Zero)
			{
				this.WriteValue(value.LocalDateTime);
				return;
			}
			this.WriteValue(value.UtcDateTime);
		}

		/// <summary>Writes a <see cref="T:System.Double" /> value.</summary>
		/// <param name="value">The <see cref="T:System.Double" /> value to write.</param>
		/// <exception cref="T:System.ArgumentException">An invalid value was specified.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual void WriteValue(double value)
		{
			this.WriteString(XmlConvert.ToString(value));
		}

		/// <summary>Writes a single-precision floating-point number.</summary>
		/// <param name="value">The single-precision floating-point number to write.</param>
		/// <exception cref="T:System.ArgumentException">An invalid value was specified.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual void WriteValue(float value)
		{
			this.WriteString(XmlConvert.ToString(value));
		}

		/// <summary>Writes a <see cref="T:System.Decimal" /> value.</summary>
		/// <param name="value">The <see cref="T:System.Decimal" /> value to write.</param>
		/// <exception cref="T:System.ArgumentException">An invalid value was specified.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual void WriteValue(decimal value)
		{
			this.WriteString(XmlConvert.ToString(value));
		}

		/// <summary>Writes a <see cref="T:System.Int32" /> value.</summary>
		/// <param name="value">The <see cref="T:System.Int32" /> value to write.</param>
		/// <exception cref="T:System.ArgumentException">An invalid value was specified.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual void WriteValue(int value)
		{
			this.WriteString(XmlConvert.ToString(value));
		}

		/// <summary>Writes a <see cref="T:System.Int64" /> value.</summary>
		/// <param name="value">The <see cref="T:System.Int64" /> value to write.</param>
		/// <exception cref="T:System.ArgumentException">An invalid value was specified.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual void WriteValue(long value)
		{
			this.WriteString(XmlConvert.ToString(value));
		}

		/// <summary>When overridden in a derived class, writes out all the attributes found at the current position in the <see cref="T:System.Xml.XmlReader" />.</summary>
		/// <param name="reader">The <see langword="XmlReader" /> from which to copy the attributes.</param>
		/// <param name="defattr">
		///       <see langword="true" /> to copy the default attributes from the <see langword="XmlReader" />; otherwise, <see langword="false" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="reader" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.Xml.XmlException">The reader is not positioned on an <see langword="element" />, <see langword="attribute" /> or <see langword="XmlDeclaration" /> node. </exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual void WriteAttributes(XmlReader reader, bool defattr)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.XmlDeclaration)
			{
				if (reader.MoveToFirstAttribute())
				{
					this.WriteAttributes(reader, defattr);
					reader.MoveToElement();
					return;
				}
			}
			else
			{
				if (reader.NodeType != XmlNodeType.Attribute)
				{
					throw new XmlException("The current position on the Reader is neither an element nor an attribute.", string.Empty);
				}
				do
				{
					if (defattr || !reader.IsDefaultInternal)
					{
						this.WriteStartAttribute(reader.Prefix, reader.LocalName, reader.NamespaceURI);
						while (reader.ReadAttributeValue())
						{
							if (reader.NodeType == XmlNodeType.EntityReference)
							{
								this.WriteEntityRef(reader.Name);
							}
							else
							{
								this.WriteString(reader.Value);
							}
						}
						this.WriteEndAttribute();
					}
				}
				while (reader.MoveToNextAttribute());
			}
		}

		/// <summary>When overridden in a derived class, copies everything from the reader to the writer and moves the reader to the start of the next sibling.</summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> to read from.</param>
		/// <param name="defattr">
		///       <see langword="true" /> to copy the default attributes from the <see langword="XmlReader" />; otherwise, <see langword="false" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="reader" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="reader" /> contains invalid characters.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual void WriteNode(XmlReader reader, bool defattr)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			bool canReadValueChunk = reader.CanReadValueChunk;
			int num = (reader.NodeType == XmlNodeType.None) ? -1 : reader.Depth;
			do
			{
				switch (reader.NodeType)
				{
				case XmlNodeType.Element:
					this.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
					this.WriteAttributes(reader, defattr);
					if (reader.IsEmptyElement)
					{
						this.WriteEndElement();
					}
					break;
				case XmlNodeType.Text:
					if (canReadValueChunk)
					{
						if (this.writeNodeBuffer == null)
						{
							this.writeNodeBuffer = new char[1024];
						}
						int count;
						while ((count = reader.ReadValueChunk(this.writeNodeBuffer, 0, 1024)) > 0)
						{
							this.WriteChars(this.writeNodeBuffer, 0, count);
						}
					}
					else
					{
						this.WriteString(reader.Value);
					}
					break;
				case XmlNodeType.CDATA:
					this.WriteCData(reader.Value);
					break;
				case XmlNodeType.EntityReference:
					this.WriteEntityRef(reader.Name);
					break;
				case XmlNodeType.ProcessingInstruction:
				case XmlNodeType.XmlDeclaration:
					this.WriteProcessingInstruction(reader.Name, reader.Value);
					break;
				case XmlNodeType.Comment:
					this.WriteComment(reader.Value);
					break;
				case XmlNodeType.DocumentType:
					this.WriteDocType(reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value);
					break;
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					this.WriteWhitespace(reader.Value);
					break;
				case XmlNodeType.EndElement:
					this.WriteFullEndElement();
					break;
				}
			}
			while (reader.Read() && (num < reader.Depth || (num == reader.Depth && reader.NodeType == XmlNodeType.EndElement)));
		}

		/// <summary>Copies everything from the <see cref="T:System.Xml.XPath.XPathNavigator" /> object to the writer. The position of the <see cref="T:System.Xml.XPath.XPathNavigator" /> remains unchanged.</summary>
		/// <param name="navigator">The <see cref="T:System.Xml.XPath.XPathNavigator" /> to copy from.</param>
		/// <param name="defattr">
		///       <see langword="true" /> to copy the default attributes; otherwise, <see langword="false" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="navigator" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual void WriteNode(XPathNavigator navigator, bool defattr)
		{
			if (navigator == null)
			{
				throw new ArgumentNullException("navigator");
			}
			int num = 0;
			navigator = navigator.Clone();
			for (;;)
			{
				IL_18:
				bool flag = false;
				switch (navigator.NodeType)
				{
				case XPathNodeType.Root:
					flag = true;
					break;
				case XPathNodeType.Element:
					this.WriteStartElement(navigator.Prefix, navigator.LocalName, navigator.NamespaceURI);
					if (navigator.MoveToFirstAttribute())
					{
						do
						{
							IXmlSchemaInfo schemaInfo = navigator.SchemaInfo;
							if (defattr || schemaInfo == null || !schemaInfo.IsDefault)
							{
								this.WriteStartAttribute(navigator.Prefix, navigator.LocalName, navigator.NamespaceURI);
								this.WriteString(navigator.Value);
								this.WriteEndAttribute();
							}
						}
						while (navigator.MoveToNextAttribute());
						navigator.MoveToParent();
					}
					if (navigator.MoveToFirstNamespace(XPathNamespaceScope.Local))
					{
						this.WriteLocalNamespaces(navigator);
						navigator.MoveToParent();
					}
					flag = true;
					break;
				case XPathNodeType.Text:
					this.WriteString(navigator.Value);
					break;
				case XPathNodeType.SignificantWhitespace:
				case XPathNodeType.Whitespace:
					this.WriteWhitespace(navigator.Value);
					break;
				case XPathNodeType.ProcessingInstruction:
					this.WriteProcessingInstruction(navigator.LocalName, navigator.Value);
					break;
				case XPathNodeType.Comment:
					this.WriteComment(navigator.Value);
					break;
				}
				if (flag)
				{
					if (navigator.MoveToFirstChild())
					{
						num++;
						continue;
					}
					if (navigator.NodeType == XPathNodeType.Element)
					{
						if (navigator.IsEmptyElement)
						{
							this.WriteEndElement();
						}
						else
						{
							this.WriteFullEndElement();
						}
					}
				}
				while (num != 0)
				{
					if (navigator.MoveToNext())
					{
						goto IL_18;
					}
					num--;
					navigator.MoveToParent();
					if (navigator.NodeType == XPathNodeType.Element)
					{
						this.WriteFullEndElement();
					}
				}
				break;
			}
		}

		/// <summary>Writes an element with the specified local name and value.</summary>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="value">The value of the element.</param>
		/// <exception cref="T:System.ArgumentException">The <paramref name="localName" /> value is <see langword="null" /> or an empty string.-or-The parameter values are not valid.</exception>
		/// <exception cref="T:System.Text.EncoderFallbackException">There is a character in the buffer that is a valid XML character but is not valid for the output encoding. For example, if the output encoding is ASCII, you should only use characters from the range of 0 to 127 for element and attribute names. The invalid character might be in the argument of this method or in an argument of previous methods that were writing to the buffer. Such characters are escaped by character entity references when possible (for example, in text nodes or attribute values). However, the character entity reference is not allowed in element and attribute names, comments, processing instructions, or CDATA sections.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public void WriteElementString(string localName, string value)
		{
			this.WriteElementString(localName, null, value);
		}

		/// <summary>Writes an element with the specified local name, namespace URI, and value.</summary>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="ns">The namespace URI to associate with the element.</param>
		/// <param name="value">The value of the element.</param>
		/// <exception cref="T:System.ArgumentException">The <paramref name="localName" /> value is <see langword="null" /> or an empty string.-or-The parameter values are not valid.</exception>
		/// <exception cref="T:System.Text.EncoderFallbackException">There is a character in the buffer that is a valid XML character but is not valid for the output encoding. For example, if the output encoding is ASCII, you should only use characters from the range of 0 to 127 for element and attribute names. The invalid character might be in the argument of this method or in an argument of previous methods that were writing to the buffer. Such characters are escaped by character entity references when possible (for example, in text nodes or attribute values). However, the character entity reference is not allowed in element and attribute names, comments, processing instructions, or CDATA sections.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public void WriteElementString(string localName, string ns, string value)
		{
			this.WriteStartElement(localName, ns);
			if (value != null && value.Length != 0)
			{
				this.WriteString(value);
			}
			this.WriteEndElement();
		}

		/// <summary>Writes an element with the specified prefix, local name, namespace URI, and value.</summary>
		/// <param name="prefix">The prefix of the element.</param>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="ns">The namespace URI of the element.</param>
		/// <param name="value">The value of the element.</param>
		/// <exception cref="T:System.ArgumentException">The <paramref name="localName" /> value is <see langword="null" /> or an empty string.-or-The parameter values are not valid.</exception>
		/// <exception cref="T:System.Text.EncoderFallbackException">There is a character in the buffer that is a valid XML character but is not valid for the output encoding. For example, if the output encoding is ASCII, you should only use characters from the range of 0 to 127 for element and attribute names. The invalid character might be in the argument of this method or in an argument of previous methods that were writing to the buffer. Such characters are escaped by character entity references when possible (for example, in text nodes or attribute values). However, the character entity reference is not allowed in element and attribute names, comments, processing instructions, or CDATA sections.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public void WriteElementString(string prefix, string localName, string ns, string value)
		{
			this.WriteStartElement(prefix, localName, ns);
			if (value != null && value.Length != 0)
			{
				this.WriteString(value);
			}
			this.WriteEndElement();
		}

		/// <summary>Releases all resources used by the current instance of the <see cref="T:System.Xml.XmlWriter" /> class.</summary>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public void Dispose()
		{
			this.Dispose(true);
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Xml.XmlWriter" /> and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///       <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing && this.WriteState != WriteState.Closed)
			{
				this.Close();
			}
		}

		private void WriteLocalNamespaces(XPathNavigator nsNav)
		{
			string localName = nsNav.LocalName;
			string value = nsNav.Value;
			if (nsNav.MoveToNextNamespace(XPathNamespaceScope.Local))
			{
				this.WriteLocalNamespaces(nsNav);
			}
			if (localName.Length == 0)
			{
				this.WriteAttributeString(string.Empty, "xmlns", "http://www.w3.org/2000/xmlns/", value);
				return;
			}
			this.WriteAttributeString("xmlns", localName, "http://www.w3.org/2000/xmlns/", value);
		}

		/// <summary>Creates a new <see cref="T:System.Xml.XmlWriter" /> instance using the specified filename.</summary>
		/// <param name="outputFileName">The file to which you want to write. The <see cref="T:System.Xml.XmlWriter" /> creates a file at the specified path and writes to it in XML 1.0 text syntax. The <paramref name="outputFileName" /> must be a file system path.</param>
		/// <returns>An <see cref="T:System.Xml.XmlWriter" /> object.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="url" /> value is <see langword="null" />.</exception>
		public static XmlWriter Create(string outputFileName)
		{
			return XmlWriter.Create(outputFileName, null);
		}

		/// <summary>Creates a new <see cref="T:System.Xml.XmlWriter" /> instance using the filename and <see cref="T:System.Xml.XmlWriterSettings" /> object.</summary>
		/// <param name="outputFileName">The file to which you want to write. The <see cref="T:System.Xml.XmlWriter" /> creates a file at the specified path and writes to it in XML 1.0 text syntax. The <paramref name="outputFileName" /> must be a file system path.</param>
		/// <param name="settings">The <see cref="T:System.Xml.XmlWriterSettings" /> object used to configure the new <see cref="T:System.Xml.XmlWriter" /> instance. If this is <see langword="null" />, a <see cref="T:System.Xml.XmlWriterSettings" /> with default settings is used.If the <see cref="T:System.Xml.XmlWriter" /> is being used with the <see cref="M:System.Xml.Xsl.XslCompiledTransform.Transform(System.String,System.Xml.XmlWriter)" /> method, you should use the <see cref="P:System.Xml.Xsl.XslCompiledTransform.OutputSettings" /> property to obtain an <see cref="T:System.Xml.XmlWriterSettings" /> object with the correct settings. This ensures that the created <see cref="T:System.Xml.XmlWriter" /> object has the correct output settings.</param>
		/// <returns>An <see cref="T:System.Xml.XmlWriter" /> object.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="url" /> value is <see langword="null" />.</exception>
		public static XmlWriter Create(string outputFileName, XmlWriterSettings settings)
		{
			if (settings == null)
			{
				settings = new XmlWriterSettings();
			}
			return settings.CreateWriter(outputFileName);
		}

		/// <summary>Creates a new <see cref="T:System.Xml.XmlWriter" /> instance using the specified stream.</summary>
		/// <param name="output">The stream to which you want to write. The <see cref="T:System.Xml.XmlWriter" /> writes XML 1.0 text syntax and appends it to the specified stream.</param>
		/// <returns>An <see cref="T:System.Xml.XmlWriter" /> object.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="stream" /> value is <see langword="null" />.</exception>
		public static XmlWriter Create(Stream output)
		{
			return XmlWriter.Create(output, null);
		}

		/// <summary>Creates a new <see cref="T:System.Xml.XmlWriter" /> instance using the stream and <see cref="T:System.Xml.XmlWriterSettings" /> object.</summary>
		/// <param name="output">The stream to which you want to write. The <see cref="T:System.Xml.XmlWriter" /> writes XML 1.0 text syntax and appends it to the specified stream.</param>
		/// <param name="settings">The <see cref="T:System.Xml.XmlWriterSettings" /> object used to configure the new <see cref="T:System.Xml.XmlWriter" /> instance. If this is <see langword="null" />, a <see cref="T:System.Xml.XmlWriterSettings" /> with default settings is used.If the <see cref="T:System.Xml.XmlWriter" /> is being used with the <see cref="M:System.Xml.Xsl.XslCompiledTransform.Transform(System.String,System.Xml.XmlWriter)" /> method, you should use the <see cref="P:System.Xml.Xsl.XslCompiledTransform.OutputSettings" /> property to obtain an <see cref="T:System.Xml.XmlWriterSettings" /> object with the correct settings. This ensures that the created <see cref="T:System.Xml.XmlWriter" /> object has the correct output settings.</param>
		/// <returns>An <see cref="T:System.Xml.XmlWriter" /> object.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="stream" /> value is <see langword="null" />.</exception>
		public static XmlWriter Create(Stream output, XmlWriterSettings settings)
		{
			if (settings == null)
			{
				settings = new XmlWriterSettings();
			}
			return settings.CreateWriter(output);
		}

		/// <summary>Creates a new <see cref="T:System.Xml.XmlWriter" /> instance using the specified <see cref="T:System.IO.TextWriter" />.</summary>
		/// <param name="output">The <see cref="T:System.IO.TextWriter" /> to which you want to write. The <see cref="T:System.Xml.XmlWriter" /> writes XML 1.0 text syntax and appends it to the specified <see cref="T:System.IO.TextWriter" />.</param>
		/// <returns>An <see cref="T:System.Xml.XmlWriter" /> object.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="text" /> value is <see langword="null" />.</exception>
		public static XmlWriter Create(TextWriter output)
		{
			return XmlWriter.Create(output, null);
		}

		/// <summary>Creates a new <see cref="T:System.Xml.XmlWriter" /> instance using the <see cref="T:System.IO.TextWriter" /> and <see cref="T:System.Xml.XmlWriterSettings" /> objects.</summary>
		/// <param name="output">The <see cref="T:System.IO.TextWriter" /> to which you want to write. The <see cref="T:System.Xml.XmlWriter" /> writes XML 1.0 text syntax and appends it to the specified <see cref="T:System.IO.TextWriter" />.</param>
		/// <param name="settings">The <see cref="T:System.Xml.XmlWriterSettings" /> object used to configure the new <see cref="T:System.Xml.XmlWriter" /> instance. If this is <see langword="null" />, a <see cref="T:System.Xml.XmlWriterSettings" /> with default settings is used.If the <see cref="T:System.Xml.XmlWriter" /> is being used with the <see cref="M:System.Xml.Xsl.XslCompiledTransform.Transform(System.String,System.Xml.XmlWriter)" /> method, you should use the <see cref="P:System.Xml.Xsl.XslCompiledTransform.OutputSettings" /> property to obtain an <see cref="T:System.Xml.XmlWriterSettings" /> object with the correct settings. This ensures that the created <see cref="T:System.Xml.XmlWriter" /> object has the correct output settings.</param>
		/// <returns>An <see cref="T:System.Xml.XmlWriter" /> object.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="text" /> value is <see langword="null" />.</exception>
		public static XmlWriter Create(TextWriter output, XmlWriterSettings settings)
		{
			if (settings == null)
			{
				settings = new XmlWriterSettings();
			}
			return settings.CreateWriter(output);
		}

		/// <summary>Creates a new <see cref="T:System.Xml.XmlWriter" /> instance using the specified <see cref="T:System.Text.StringBuilder" />.</summary>
		/// <param name="output">The <see cref="T:System.Text.StringBuilder" /> to which to write to. Content written by the <see cref="T:System.Xml.XmlWriter" /> is appended to the <see cref="T:System.Text.StringBuilder" />.</param>
		/// <returns>An <see cref="T:System.Xml.XmlWriter" /> object.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="builder" /> value is <see langword="null" />.</exception>
		public static XmlWriter Create(StringBuilder output)
		{
			return XmlWriter.Create(output, null);
		}

		/// <summary>Creates a new <see cref="T:System.Xml.XmlWriter" /> instance using the <see cref="T:System.Text.StringBuilder" /> and <see cref="T:System.Xml.XmlWriterSettings" /> objects.</summary>
		/// <param name="output">The <see cref="T:System.Text.StringBuilder" /> to which to write to. Content written by the <see cref="T:System.Xml.XmlWriter" /> is appended to the <see cref="T:System.Text.StringBuilder" />.</param>
		/// <param name="settings">The <see cref="T:System.Xml.XmlWriterSettings" /> object used to configure the new <see cref="T:System.Xml.XmlWriter" /> instance. If this is <see langword="null" />, a <see cref="T:System.Xml.XmlWriterSettings" /> with default settings is used.If the <see cref="T:System.Xml.XmlWriter" /> is being used with the <see cref="M:System.Xml.Xsl.XslCompiledTransform.Transform(System.String,System.Xml.XmlWriter)" /> method, you should use the <see cref="P:System.Xml.Xsl.XslCompiledTransform.OutputSettings" /> property to obtain an <see cref="T:System.Xml.XmlWriterSettings" /> object with the correct settings. This ensures that the created <see cref="T:System.Xml.XmlWriter" /> object has the correct output settings.</param>
		/// <returns>An <see cref="T:System.Xml.XmlWriter" /> object.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="builder" /> value is <see langword="null" />.</exception>
		public static XmlWriter Create(StringBuilder output, XmlWriterSettings settings)
		{
			if (settings == null)
			{
				settings = new XmlWriterSettings();
			}
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			return settings.CreateWriter(new StringWriter(output, CultureInfo.InvariantCulture));
		}

		/// <summary>Creates a new <see cref="T:System.Xml.XmlWriter" /> instance using the specified <see cref="T:System.Xml.XmlWriter" /> object.</summary>
		/// <param name="output">The <see cref="T:System.Xml.XmlWriter" /> object that you want to use as the underlying writer.</param>
		/// <returns>An <see cref="T:System.Xml.XmlWriter" /> object that is wrapped around the specified <see cref="T:System.Xml.XmlWriter" /> object.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="writer" /> value is <see langword="null" />.</exception>
		public static XmlWriter Create(XmlWriter output)
		{
			return XmlWriter.Create(output, null);
		}

		/// <summary>Creates a new <see cref="T:System.Xml.XmlWriter" /> instance using the specified <see cref="T:System.Xml.XmlWriter" /> and <see cref="T:System.Xml.XmlWriterSettings" /> objects.</summary>
		/// <param name="output">The <see cref="T:System.Xml.XmlWriter" /> object that you want to use as the underlying writer.</param>
		/// <param name="settings">The <see cref="T:System.Xml.XmlWriterSettings" /> object used to configure the new <see cref="T:System.Xml.XmlWriter" /> instance. If this is <see langword="null" />, a <see cref="T:System.Xml.XmlWriterSettings" /> with default settings is used.If the <see cref="T:System.Xml.XmlWriter" /> is being used with the <see cref="M:System.Xml.Xsl.XslCompiledTransform.Transform(System.String,System.Xml.XmlWriter)" /> method, you should use the <see cref="P:System.Xml.Xsl.XslCompiledTransform.OutputSettings" /> property to obtain an <see cref="T:System.Xml.XmlWriterSettings" /> object with the correct settings. This ensures that the created <see cref="T:System.Xml.XmlWriter" /> object has the correct output settings.</param>
		/// <returns>An <see cref="T:System.Xml.XmlWriter" /> object that is wrapped around the specified <see cref="T:System.Xml.XmlWriter" /> object.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="writer" /> value is <see langword="null" />.</exception>
		public static XmlWriter Create(XmlWriter output, XmlWriterSettings settings)
		{
			if (settings == null)
			{
				settings = new XmlWriterSettings();
			}
			return settings.CreateWriter(output);
		}

		/// <summary>Asynchronously writes the XML declaration with the version "1.0".</summary>
		/// <returns>The task that represents the asynchronous <see langword="WriteStartDocument" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task WriteStartDocumentAsync()
		{
			throw new NotImplementedException();
		}

		/// <summary>Asynchronously writes the XML declaration with the version "1.0" and the standalone attribute.</summary>
		/// <param name="standalone">If <see langword="true" />, it writes "standalone=yes"; if <see langword="false" />, it writes "standalone=no".</param>
		/// <returns>The task that represents the asynchronous <see langword="WriteStartDocument" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task WriteStartDocumentAsync(bool standalone)
		{
			throw new NotImplementedException();
		}

		/// <summary>Asynchronously closes any open elements or attributes and puts the writer back in the Start state.</summary>
		/// <returns>The task that represents the asynchronous <see langword="WriteEndDocument" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task WriteEndDocumentAsync()
		{
			throw new NotImplementedException();
		}

		/// <summary>Asynchronously writes the DOCTYPE declaration with the specified name and optional attributes.</summary>
		/// <param name="name">The name of the DOCTYPE. This must be non-empty.</param>
		/// <param name="pubid">If non-null it also writes PUBLIC "pubid" "sysid" where <paramref name="pubid" /> and <paramref name="sysid" /> are replaced with the value of the given arguments.</param>
		/// <param name="sysid">If <paramref name="pubid" /> is <see langword="null" /> and <paramref name="sysid" /> is non-null it writes SYSTEM "sysid" where <paramref name="sysid" /> is replaced with the value of this argument.</param>
		/// <param name="subset">If non-null it writes [subset] where subset is replaced with the value of this argument.</param>
		/// <returns>The task that represents the asynchronous <see langword="WriteDocType" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task WriteDocTypeAsync(string name, string pubid, string sysid, string subset)
		{
			throw new NotImplementedException();
		}

		/// <summary>Asynchronously writes the specified start tag and associates it with the given namespace and prefix.</summary>
		/// <param name="prefix">The namespace prefix of the element.</param>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="ns">The namespace URI to associate with the element.</param>
		/// <returns>The task that represents the asynchronous <see langword="WriteStartElement" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task WriteStartElementAsync(string prefix, string localName, string ns)
		{
			throw new NotImplementedException();
		}

		/// <summary>Asynchronously closes one element and pops the corresponding namespace scope.</summary>
		/// <returns>The task that represents the asynchronous <see langword="WriteEndElement" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task WriteEndElementAsync()
		{
			throw new NotImplementedException();
		}

		/// <summary>Asynchronously closes one element and pops the corresponding namespace scope.</summary>
		/// <returns>The task that represents the asynchronous <see langword="WriteFullEndElement" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task WriteFullEndElementAsync()
		{
			throw new NotImplementedException();
		}

		/// <summary>Asynchronously writes out the attribute with the specified prefix, local name, namespace URI, and value.</summary>
		/// <param name="prefix">The namespace prefix of the attribute.</param>
		/// <param name="localName">The local name of the attribute.</param>
		/// <param name="ns">The namespace URI of the attribute.</param>
		/// <param name="value">The value of the attribute.</param>
		/// <returns>The task that represents the asynchronous <see langword="WriteAttributeString" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
		public Task WriteAttributeStringAsync(string prefix, string localName, string ns, string value)
		{
			Task task = this.WriteStartAttributeAsync(prefix, localName, ns);
			if (task.IsSuccess())
			{
				return this.WriteStringAsync(value).CallTaskFuncWhenFinish(new Func<Task>(this.WriteEndAttributeAsync));
			}
			return this.WriteAttributeStringAsyncHelper(task, value);
		}

		private Task WriteAttributeStringAsyncHelper(Task task, string value)
		{
			XmlWriter.<WriteAttributeStringAsyncHelper>d__82 <WriteAttributeStringAsyncHelper>d__;
			<WriteAttributeStringAsyncHelper>d__.<>4__this = this;
			<WriteAttributeStringAsyncHelper>d__.task = task;
			<WriteAttributeStringAsyncHelper>d__.value = value;
			<WriteAttributeStringAsyncHelper>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteAttributeStringAsyncHelper>d__.<>1__state = -1;
			<WriteAttributeStringAsyncHelper>d__.<>t__builder.Start<XmlWriter.<WriteAttributeStringAsyncHelper>d__82>(ref <WriteAttributeStringAsyncHelper>d__);
			return <WriteAttributeStringAsyncHelper>d__.<>t__builder.Task;
		}

		/// <summary>Asynchronously writes the start of an attribute with the specified prefix, local name, and namespace URI.</summary>
		/// <param name="prefix">The namespace prefix of the attribute.</param>
		/// <param name="localName">The local name of the attribute.</param>
		/// <param name="ns">The namespace URI for the attribute.</param>
		/// <returns>The task that represents the asynchronous <see langword="WriteStartAttribute" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
		protected internal virtual Task WriteStartAttributeAsync(string prefix, string localName, string ns)
		{
			throw new NotImplementedException();
		}

		/// <summary>Asynchronously closes the previous <see cref="M:System.Xml.XmlWriter.WriteStartAttribute(System.String,System.String)" /> call.</summary>
		/// <returns>The task that represents the asynchronous <see langword="WriteEndAttribute" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
		protected internal virtual Task WriteEndAttributeAsync()
		{
			throw new NotImplementedException();
		}

		/// <summary>Asynchronously writes out a &lt;![CDATA[...]]&gt; block containing the specified text.</summary>
		/// <param name="text">The text to place inside the CDATA block.</param>
		/// <returns>The task that represents the asynchronous <see langword="WriteCData" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task WriteCDataAsync(string text)
		{
			throw new NotImplementedException();
		}

		/// <summary>Asynchronously writes out a comment &lt;!--...--&gt; containing the specified text.</summary>
		/// <param name="text">Text to place inside the comment.</param>
		/// <returns>The task that represents the asynchronous <see langword="WriteComment" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task WriteCommentAsync(string text)
		{
			throw new NotImplementedException();
		}

		/// <summary>Asynchronously writes out a processing instruction with a space between the name and text as follows: &lt;?name text?&gt;.</summary>
		/// <param name="name">The name of the processing instruction.</param>
		/// <param name="text">The text to include in the processing instruction.</param>
		/// <returns>The task that represents the asynchronous <see langword="WriteProcessingInstruction" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task WriteProcessingInstructionAsync(string name, string text)
		{
			throw new NotImplementedException();
		}

		/// <summary>Asynchronously writes out an entity reference as <see langword="&amp;name;" />.</summary>
		/// <param name="name">The name of the entity reference.</param>
		/// <returns>The task that represents the asynchronous <see langword="WriteEntityRef" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task WriteEntityRefAsync(string name)
		{
			throw new NotImplementedException();
		}

		/// <summary>Asynchronously forces the generation of a character entity for the specified Unicode character value.</summary>
		/// <param name="ch">The Unicode character for which to generate a character entity.</param>
		/// <returns>The task that represents the asynchronous <see langword="WriteCharEntity" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task WriteCharEntityAsync(char ch)
		{
			throw new NotImplementedException();
		}

		/// <summary>Asynchronously writes out the given white space.</summary>
		/// <param name="ws">The string of white space characters.</param>
		/// <returns>The task that represents the asynchronous <see langword="WriteWhitespace" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task WriteWhitespaceAsync(string ws)
		{
			throw new NotImplementedException();
		}

		/// <summary>Asynchronously writes the given text content.</summary>
		/// <param name="text">The text to write.</param>
		/// <returns>The task that represents the asynchronous <see langword="WriteString" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task WriteStringAsync(string text)
		{
			throw new NotImplementedException();
		}

		/// <summary>Asynchronously generates and writes the surrogate character entity for the surrogate character pair.</summary>
		/// <param name="lowChar">The low surrogate. This must be a value between 0xDC00 and 0xDFFF.</param>
		/// <param name="highChar">The high surrogate. This must be a value between 0xD800 and 0xDBFF.</param>
		/// <returns>The task that represents the asynchronous <see langword="WriteSurrogateCharEntity" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task WriteSurrogateCharEntityAsync(char lowChar, char highChar)
		{
			throw new NotImplementedException();
		}

		/// <summary>Asynchronously writes text one buffer at a time.</summary>
		/// <param name="buffer">Character array containing the text to write.</param>
		/// <param name="index">The position in the buffer indicating the start of the text to write.</param>
		/// <param name="count">The number of characters to write.</param>
		/// <returns>The task that represents the asynchronous <see langword="WriteChars" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task WriteCharsAsync(char[] buffer, int index, int count)
		{
			throw new NotImplementedException();
		}

		/// <summary>Asynchronously writes raw markup manually from a character buffer.</summary>
		/// <param name="buffer">Character array containing the text to write.</param>
		/// <param name="index">The position within the buffer indicating the start of the text to write.</param>
		/// <param name="count">The number of characters to write.</param>
		/// <returns>The task that represents the asynchronous <see langword="WriteRaw" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task WriteRawAsync(char[] buffer, int index, int count)
		{
			throw new NotImplementedException();
		}

		/// <summary>Asynchronously writes raw markup manually from a string.</summary>
		/// <param name="data">String containing the text to write.</param>
		/// <returns>The task that represents the asynchronous <see langword="WriteRaw" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task WriteRawAsync(string data)
		{
			throw new NotImplementedException();
		}

		/// <summary>Asynchronously encodes the specified binary bytes as Base64 and writes out the resulting text.</summary>
		/// <param name="buffer">Byte array to encode.</param>
		/// <param name="index">The position in the buffer indicating the start of the bytes to write.</param>
		/// <param name="count">The number of bytes to write.</param>
		/// <returns>The task that represents the asynchronous <see langword="WriteBase64" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task WriteBase64Async(byte[] buffer, int index, int count)
		{
			throw new NotImplementedException();
		}

		/// <summary>Asynchronously encodes the specified binary bytes as <see langword="BinHex" /> and writes out the resulting text.</summary>
		/// <param name="buffer">Byte array to encode.</param>
		/// <param name="index">The position in the buffer indicating the start of the bytes to write.</param>
		/// <param name="count">The number of bytes to write.</param>
		/// <returns>The task that represents the asynchronous <see langword="WriteBinHex" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task WriteBinHexAsync(byte[] buffer, int index, int count)
		{
			return BinHexEncoder.EncodeAsync(buffer, index, count, this);
		}

		/// <summary>Asynchronously flushes whatever is in the buffer to the underlying streams and also flushes the underlying stream.</summary>
		/// <returns>The task that represents the asynchronous <see langword="Flush" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task FlushAsync()
		{
			throw new NotImplementedException();
		}

		/// <summary>Asynchronously writes out the specified name, ensuring it is a valid NmToken according to the W3C XML 1.0 recommendation (http://www.w3.org/TR/1998/REC-xml-19980210#NT-Name).</summary>
		/// <param name="name">The name to write.</param>
		/// <returns>The task that represents the asynchronous <see langword="WriteNmToken" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task WriteNmTokenAsync(string name)
		{
			if (name == null || name.Length == 0)
			{
				throw new ArgumentException(Res.GetString("The empty string '' is not a valid name."));
			}
			return this.WriteStringAsync(XmlConvert.VerifyNMTOKEN(name, ExceptionType.ArgumentException));
		}

		/// <summary>Asynchronously writes out the specified name, ensuring it is a valid name according to the W3C XML 1.0 recommendation (http://www.w3.org/TR/1998/REC-xml-19980210#NT-Name).</summary>
		/// <param name="name">The name to write.</param>
		/// <returns>The task that represents the asynchronous <see langword="WriteName" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task WriteNameAsync(string name)
		{
			return this.WriteStringAsync(XmlConvert.VerifyQName(name, ExceptionType.ArgumentException));
		}

		/// <summary>Asynchronously writes out the namespace-qualified name. This method looks up the prefix that is in scope for the given namespace.</summary>
		/// <param name="localName">The local name to write.</param>
		/// <param name="ns">The namespace URI for the name.</param>
		/// <returns>The task that represents the asynchronous <see langword="WriteQualifiedName" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task WriteQualifiedNameAsync(string localName, string ns)
		{
			XmlWriter.<WriteQualifiedNameAsync>d__101 <WriteQualifiedNameAsync>d__;
			<WriteQualifiedNameAsync>d__.<>4__this = this;
			<WriteQualifiedNameAsync>d__.localName = localName;
			<WriteQualifiedNameAsync>d__.ns = ns;
			<WriteQualifiedNameAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteQualifiedNameAsync>d__.<>1__state = -1;
			<WriteQualifiedNameAsync>d__.<>t__builder.Start<XmlWriter.<WriteQualifiedNameAsync>d__101>(ref <WriteQualifiedNameAsync>d__);
			return <WriteQualifiedNameAsync>d__.<>t__builder.Task;
		}

		/// <summary>Asynchronously writes out all the attributes found at the current position in the <see cref="T:System.Xml.XmlReader" />.</summary>
		/// <param name="reader">The <see langword="XmlReader" /> from which to copy the attributes.</param>
		/// <param name="defattr">
		///       <see langword="true" /> to copy the default attributes from the <see langword="XmlReader" />; otherwise, <see langword="false" />.</param>
		/// <returns>The task that represents the asynchronous <see langword="WriteAttributes" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task WriteAttributesAsync(XmlReader reader, bool defattr)
		{
			XmlWriter.<WriteAttributesAsync>d__102 <WriteAttributesAsync>d__;
			<WriteAttributesAsync>d__.<>4__this = this;
			<WriteAttributesAsync>d__.reader = reader;
			<WriteAttributesAsync>d__.defattr = defattr;
			<WriteAttributesAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteAttributesAsync>d__.<>1__state = -1;
			<WriteAttributesAsync>d__.<>t__builder.Start<XmlWriter.<WriteAttributesAsync>d__102>(ref <WriteAttributesAsync>d__);
			return <WriteAttributesAsync>d__.<>t__builder.Task;
		}

		/// <summary>Asynchronously copies everything from the reader to the writer and moves the reader to the start of the next sibling.</summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> to read from.</param>
		/// <param name="defattr">
		///       <see langword="true" /> to copy the default attributes from the <see langword="XmlReader" />; otherwise, <see langword="false" />.</param>
		/// <returns>The task that represents the asynchronous <see langword="WriteNode" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task WriteNodeAsync(XmlReader reader, bool defattr)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (reader.Settings != null && reader.Settings.Async)
			{
				return this.WriteNodeAsync_CallAsyncReader(reader, defattr);
			}
			return this.WriteNodeAsync_CallSyncReader(reader, defattr);
		}

		internal Task WriteNodeAsync_CallSyncReader(XmlReader reader, bool defattr)
		{
			XmlWriter.<WriteNodeAsync_CallSyncReader>d__104 <WriteNodeAsync_CallSyncReader>d__;
			<WriteNodeAsync_CallSyncReader>d__.<>4__this = this;
			<WriteNodeAsync_CallSyncReader>d__.reader = reader;
			<WriteNodeAsync_CallSyncReader>d__.defattr = defattr;
			<WriteNodeAsync_CallSyncReader>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteNodeAsync_CallSyncReader>d__.<>1__state = -1;
			<WriteNodeAsync_CallSyncReader>d__.<>t__builder.Start<XmlWriter.<WriteNodeAsync_CallSyncReader>d__104>(ref <WriteNodeAsync_CallSyncReader>d__);
			return <WriteNodeAsync_CallSyncReader>d__.<>t__builder.Task;
		}

		internal Task WriteNodeAsync_CallAsyncReader(XmlReader reader, bool defattr)
		{
			XmlWriter.<WriteNodeAsync_CallAsyncReader>d__105 <WriteNodeAsync_CallAsyncReader>d__;
			<WriteNodeAsync_CallAsyncReader>d__.<>4__this = this;
			<WriteNodeAsync_CallAsyncReader>d__.reader = reader;
			<WriteNodeAsync_CallAsyncReader>d__.defattr = defattr;
			<WriteNodeAsync_CallAsyncReader>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteNodeAsync_CallAsyncReader>d__.<>1__state = -1;
			<WriteNodeAsync_CallAsyncReader>d__.<>t__builder.Start<XmlWriter.<WriteNodeAsync_CallAsyncReader>d__105>(ref <WriteNodeAsync_CallAsyncReader>d__);
			return <WriteNodeAsync_CallAsyncReader>d__.<>t__builder.Task;
		}

		/// <summary>Asynchronously copies everything from the <see cref="T:System.Xml.XPath.XPathNavigator" /> object to the writer. The position of the <see cref="T:System.Xml.XPath.XPathNavigator" /> remains unchanged.</summary>
		/// <param name="navigator">The <see cref="T:System.Xml.XPath.XPathNavigator" /> to copy from.</param>
		/// <param name="defattr">
		///       <see langword="true" /> to copy the default attributes; otherwise, <see langword="false" />.</param>
		/// <returns>The task that represents the asynchronous <see langword="WriteNode" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task WriteNodeAsync(XPathNavigator navigator, bool defattr)
		{
			XmlWriter.<WriteNodeAsync>d__106 <WriteNodeAsync>d__;
			<WriteNodeAsync>d__.<>4__this = this;
			<WriteNodeAsync>d__.navigator = navigator;
			<WriteNodeAsync>d__.defattr = defattr;
			<WriteNodeAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteNodeAsync>d__.<>1__state = -1;
			<WriteNodeAsync>d__.<>t__builder.Start<XmlWriter.<WriteNodeAsync>d__106>(ref <WriteNodeAsync>d__);
			return <WriteNodeAsync>d__.<>t__builder.Task;
		}

		/// <summary>Asynchronously writes an element with the specified prefix, local name, namespace URI, and value.</summary>
		/// <param name="prefix">The prefix of the element.</param>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="ns">The namespace URI of the element.</param>
		/// <param name="value">The value of the element.</param>
		/// <returns>The task that represents the asynchronous <see langword="WriteElementString" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlWriterSettings.Async to true if you want to use Async Methods.”</exception>
		public Task WriteElementStringAsync(string prefix, string localName, string ns, string value)
		{
			XmlWriter.<WriteElementStringAsync>d__107 <WriteElementStringAsync>d__;
			<WriteElementStringAsync>d__.<>4__this = this;
			<WriteElementStringAsync>d__.prefix = prefix;
			<WriteElementStringAsync>d__.localName = localName;
			<WriteElementStringAsync>d__.ns = ns;
			<WriteElementStringAsync>d__.value = value;
			<WriteElementStringAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteElementStringAsync>d__.<>1__state = -1;
			<WriteElementStringAsync>d__.<>t__builder.Start<XmlWriter.<WriteElementStringAsync>d__107>(ref <WriteElementStringAsync>d__);
			return <WriteElementStringAsync>d__.<>t__builder.Task;
		}

		private Task WriteLocalNamespacesAsync(XPathNavigator nsNav)
		{
			XmlWriter.<WriteLocalNamespacesAsync>d__108 <WriteLocalNamespacesAsync>d__;
			<WriteLocalNamespacesAsync>d__.<>4__this = this;
			<WriteLocalNamespacesAsync>d__.nsNav = nsNav;
			<WriteLocalNamespacesAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteLocalNamespacesAsync>d__.<>1__state = -1;
			<WriteLocalNamespacesAsync>d__.<>t__builder.Start<XmlWriter.<WriteLocalNamespacesAsync>d__108>(ref <WriteLocalNamespacesAsync>d__);
			return <WriteLocalNamespacesAsync>d__.<>t__builder.Task;
		}

		private char[] writeNodeBuffer;

		private const int WriteNodeBufferSize = 1024;
	}
}
