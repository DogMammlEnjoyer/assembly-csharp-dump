using System;
using System.Globalization;
using System.IO;
using System.Runtime;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace System.Xml
{
	/// <summary>Represents an abstract class that Windows Communication Foundation (WCF) derives from <see cref="T:System.Xml.XmlWriter" /> to do serialization and deserialization.</summary>
	public abstract class XmlDictionaryWriter : XmlWriter
	{
		internal virtual bool FastAsync
		{
			get
			{
				return false;
			}
		}

		internal virtual AsyncCompletionResult WriteBase64Async(AsyncEventArgs<XmlWriteBase64AsyncArguments> state)
		{
			throw FxTrace.Exception.AsError(new NotSupportedException());
		}

		/// <summary>Asynchronously encodes the specified binary bytes as Base64 and writes out the resulting text.</summary>
		/// <param name="buffer">Byte array to encode.</param>
		/// <param name="index">The position in the buffer indicating the start of the bytes to write.</param>
		/// <param name="count">The number of bytes to write.</param>
		/// <returns>The task that represents the asynchronous <see langword="WriteBase64" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlDictionaryWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message "An asynchronous operation is already in progress."
		/// -or-
		/// An <see cref="T:System.Xml.XmlDictionaryWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message "Set XmlWriterSettings.Async to true if you want to use Async Methods."</exception>
		public override Task WriteBase64Async(byte[] buffer, int index, int count)
		{
			return Task.Factory.FromAsync<byte[], int, int>(new Func<byte[], int, int, AsyncCallback, object, IAsyncResult>(this.BeginWriteBase64), new Action<IAsyncResult>(this.EndWriteBase64), buffer, index, count, null);
		}

		internal virtual IAsyncResult BeginWriteBase64(byte[] buffer, int index, int count, AsyncCallback callback, object state)
		{
			return new XmlDictionaryWriter.WriteBase64AsyncResult(buffer, index, count, this, callback, state);
		}

		internal virtual void EndWriteBase64(IAsyncResult result)
		{
			ScheduleActionItemAsyncResult.End(result);
		}

		/// <summary>Creates an instance of <see cref="T:System.Xml.XmlDictionaryWriter" /> that writes WCF binary XML format.</summary>
		/// <param name="stream">The stream to write to.</param>
		/// <returns>An instance of <see cref="T:System.Xml.XmlDictionaryWriter" />.</returns>
		public static XmlDictionaryWriter CreateBinaryWriter(Stream stream)
		{
			return XmlDictionaryWriter.CreateBinaryWriter(stream, null);
		}

		/// <summary>Creates an instance of <see cref="T:System.Xml.XmlDictionaryWriter" /> that writes WCF binary XML format.</summary>
		/// <param name="stream">The stream to write to.</param>
		/// <param name="dictionary">The <see cref="T:System.Xml.XmlDictionary" /> to use as the shared dictionary.</param>
		/// <returns>An instance of <see cref="T:System.Xml.XmlDictionaryWriter" />.</returns>
		public static XmlDictionaryWriter CreateBinaryWriter(Stream stream, IXmlDictionary dictionary)
		{
			return XmlDictionaryWriter.CreateBinaryWriter(stream, dictionary, null);
		}

		/// <summary>Creates an instance of <see cref="T:System.Xml.XmlDictionaryWriter" /> that writes WCF binary XML format.</summary>
		/// <param name="stream">The stream to write to.</param>
		/// <param name="dictionary">The <see cref="T:System.Xml.XmlDictionary" /> to use as the shared dictionary.</param>
		/// <param name="session">The <see cref="T:System.Xml.XmlBinaryWriterSession" /> to use.</param>
		/// <returns>An instance of <see cref="T:System.Xml.XmlDictionaryWriter" />.</returns>
		public static XmlDictionaryWriter CreateBinaryWriter(Stream stream, IXmlDictionary dictionary, XmlBinaryWriterSession session)
		{
			return XmlDictionaryWriter.CreateBinaryWriter(stream, dictionary, session, true);
		}

		/// <summary>Creates an instance of <see cref="T:System.Xml.XmlDictionaryWriter" /> that writes WCF binary XML format.</summary>
		/// <param name="stream">The stream from which to read.</param>
		/// <param name="dictionary">The <see cref="T:System.Xml.XmlDictionary" /> to use as the shared dictionary.</param>
		/// <param name="session">The <see cref="T:System.Xml.XmlBinaryWriterSession" /> to use.</param>
		/// <param name="ownsStream">
		///   <see langword="true" /> to indicate that the stream is closed by the writer when done; otherwise <see langword="false" />.</param>
		/// <returns>An instance of <see cref="T:System.Xml.XmlDictionaryWriter" />.</returns>
		public static XmlDictionaryWriter CreateBinaryWriter(Stream stream, IXmlDictionary dictionary, XmlBinaryWriterSession session, bool ownsStream)
		{
			XmlBinaryWriter xmlBinaryWriter = new XmlBinaryWriter();
			xmlBinaryWriter.SetOutput(stream, dictionary, session, ownsStream);
			return xmlBinaryWriter;
		}

		/// <summary>Creates an instance of <see cref="T:System.Xml.XmlDictionaryWriter" /> that writes text XML.</summary>
		/// <param name="stream">The stream to write to.</param>
		/// <returns>An instance of <see cref="T:System.Xml.XmlDictionaryWriter" />.</returns>
		public static XmlDictionaryWriter CreateTextWriter(Stream stream)
		{
			return XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, true);
		}

		/// <summary>Creates an instance of <see cref="T:System.Xml.XmlDictionaryWriter" /> that writes text XML.</summary>
		/// <param name="stream">The stream to write to.</param>
		/// <param name="encoding">The character encoding of the output.</param>
		/// <returns>An instance of <see cref="T:System.Xml.XmlDictionaryWriter" />.</returns>
		public static XmlDictionaryWriter CreateTextWriter(Stream stream, Encoding encoding)
		{
			return XmlDictionaryWriter.CreateTextWriter(stream, encoding, true);
		}

		/// <summary>Creates an instance of <see cref="T:System.Xml.XmlDictionaryWriter" /> that writes text XML.</summary>
		/// <param name="stream">The stream to write to.</param>
		/// <param name="encoding">The character encoding of the stream.</param>
		/// <param name="ownsStream">
		///   <see langword="true" /> to indicate that the stream is closed by the writer when done; otherwise <see langword="false" />.</param>
		/// <returns>An instance of <see cref="T:System.Xml.XmlDictionaryWriter" />.</returns>
		public static XmlDictionaryWriter CreateTextWriter(Stream stream, Encoding encoding, bool ownsStream)
		{
			XmlUTF8TextWriter xmlUTF8TextWriter = new XmlUTF8TextWriter();
			xmlUTF8TextWriter.SetOutput(stream, encoding, ownsStream);
			return xmlUTF8TextWriter;
		}

		/// <summary>Creates an instance of <see cref="T:System.Xml.XmlDictionaryWriter" /> that writes XML in the MTOM format.</summary>
		/// <param name="stream">The stream to write to.</param>
		/// <param name="encoding">The character encoding of the stream.</param>
		/// <param name="maxSizeInBytes">The maximum number of bytes that are buffered in the writer.</param>
		/// <param name="startInfo">An attribute in the ContentType SOAP header.</param>
		/// <returns>An instance of <see cref="T:System.Xml.XmlDictionaryWriter" />.</returns>
		public static XmlDictionaryWriter CreateMtomWriter(Stream stream, Encoding encoding, int maxSizeInBytes, string startInfo)
		{
			return XmlDictionaryWriter.CreateMtomWriter(stream, encoding, maxSizeInBytes, startInfo, null, null, true, true);
		}

		/// <summary>Creates an instance of <see cref="T:System.Xml.XmlDictionaryWriter" /> that writes XML in the MTOM format.</summary>
		/// <param name="stream">The stream to write to.</param>
		/// <param name="encoding">The character encoding of the stream.</param>
		/// <param name="maxSizeInBytes">The maximum number of bytes that are buffered in the writer.</param>
		/// <param name="startInfo">The content-type of the MIME part that contains the Infoset.</param>
		/// <param name="boundary">The MIME boundary in the message.</param>
		/// <param name="startUri">The content-id URI of the MIME part that contains the Infoset.</param>
		/// <param name="writeMessageHeaders">
		///   <see langword="true" /> to write message headers.</param>
		/// <param name="ownsStream">
		///   <see langword="true" /> to indicate that the stream is closed by the writer when done; otherwise <see langword="false" />.</param>
		/// <returns>An instance of <see cref="T:System.Xml.XmlDictionaryWriter" />.</returns>
		public static XmlDictionaryWriter CreateMtomWriter(Stream stream, Encoding encoding, int maxSizeInBytes, string startInfo, string boundary, string startUri, bool writeMessageHeaders, bool ownsStream)
		{
			XmlMtomWriter xmlMtomWriter = new XmlMtomWriter();
			xmlMtomWriter.SetOutput(stream, encoding, maxSizeInBytes, startInfo, boundary, startUri, writeMessageHeaders, ownsStream);
			return xmlMtomWriter;
		}

		/// <summary>Creates an instance of <see cref="T:System.Xml.XmlDictionaryWriter" /> from an existing <see cref="T:System.Xml.XmlWriter" />.</summary>
		/// <param name="writer">An instance of <see cref="T:System.Xml.XmlWriter" />.</param>
		/// <returns>An instance of <see cref="T:System.Xml.XmlDictionaryWriter" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="writer" /> is <see langword="null" />.</exception>
		public static XmlDictionaryWriter CreateDictionaryWriter(XmlWriter writer)
		{
			if (writer == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("writer");
			}
			XmlDictionaryWriter xmlDictionaryWriter = writer as XmlDictionaryWriter;
			if (xmlDictionaryWriter == null)
			{
				xmlDictionaryWriter = new XmlDictionaryWriter.XmlWrappedWriter(writer);
			}
			return xmlDictionaryWriter;
		}

		/// <summary>Writes the specified start tag and associates it with the given namespace.</summary>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceUri">The namespace URI of the element.</param>
		/// <exception cref="T:System.InvalidOperationException">The writer is closed.</exception>
		public void WriteStartElement(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			this.WriteStartElement(null, localName, namespaceUri);
		}

		/// <summary>Writes the specified start tag and associates it with the given namespace and prefix.</summary>
		/// <param name="prefix">The prefix of the element.</param>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceUri">The namespace URI of the element.</param>
		/// <exception cref="T:System.InvalidOperationException">The writer is closed.</exception>
		public virtual void WriteStartElement(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			this.WriteStartElement(prefix, XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri));
		}

		/// <summary>Writes the start of an attribute with the specified local name, and namespace URI.</summary>
		/// <param name="localName">The local name of the attribute.</param>
		/// <param name="namespaceUri">The namespace URI of the attribute.</param>
		public void WriteStartAttribute(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			this.WriteStartAttribute(null, localName, namespaceUri);
		}

		/// <summary>Writes the start of an attribute with the specified prefix, local name, and namespace URI.</summary>
		/// <param name="prefix">The namespace prefix.</param>
		/// <param name="localName">The local name of the attribute.</param>
		/// <param name="namespaceUri">The namespace URI of the attribute.</param>
		public virtual void WriteStartAttribute(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			this.WriteStartAttribute(prefix, XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri));
		}

		/// <summary>Writes an attribute qualified name and value.</summary>
		/// <param name="localName">The local name of the attribute.</param>
		/// <param name="namespaceUri">The namespace URI of the attribute.</param>
		/// <param name="value">The attribute.</param>
		public void WriteAttributeString(XmlDictionaryString localName, XmlDictionaryString namespaceUri, string value)
		{
			this.WriteAttributeString(null, localName, namespaceUri, value);
		}

		/// <summary>Writes a namespace declaration attribute.</summary>
		/// <param name="prefix">The prefix that is bound to the given namespace.</param>
		/// <param name="namespaceUri">The namespace to which the prefix is bound.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="namespaceUri" /> is <see langword="null" />.</exception>
		public virtual void WriteXmlnsAttribute(string prefix, string namespaceUri)
		{
			if (namespaceUri == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("namespaceUri");
			}
			if (prefix == null)
			{
				if (this.LookupPrefix(namespaceUri) != null)
				{
					return;
				}
				prefix = ((namespaceUri.Length == 0) ? string.Empty : ("d" + namespaceUri.Length.ToString(NumberFormatInfo.InvariantInfo)));
			}
			base.WriteAttributeString("xmlns", prefix, null, namespaceUri);
		}

		/// <summary>Writes a namespace declaration attribute.</summary>
		/// <param name="prefix">The prefix that is bound to the given namespace.</param>
		/// <param name="namespaceUri">The namespace to which the prefix is bound.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="namespaceUri" /> is <see langword="null" />.</exception>
		public virtual void WriteXmlnsAttribute(string prefix, XmlDictionaryString namespaceUri)
		{
			this.WriteXmlnsAttribute(prefix, XmlDictionaryString.GetString(namespaceUri));
		}

		/// <summary>Writes a standard XML attribute in the current node.</summary>
		/// <param name="localName">The local name of the attribute.</param>
		/// <param name="value">The value of the attribute.</param>
		public virtual void WriteXmlAttribute(string localName, string value)
		{
			base.WriteAttributeString("xml", localName, null, value);
		}

		/// <summary>Writes an XML attribute in the current node.</summary>
		/// <param name="localName">The local name of the attribute.</param>
		/// <param name="value">The value of the attribute.</param>
		public virtual void WriteXmlAttribute(XmlDictionaryString localName, XmlDictionaryString value)
		{
			this.WriteXmlAttribute(XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(value));
		}

		/// <summary>Writes an attribute qualified name and value.</summary>
		/// <param name="prefix">The prefix of the attribute.</param>
		/// <param name="localName">The local name of the attribute.</param>
		/// <param name="namespaceUri">The namespace URI of the attribute.</param>
		/// <param name="value">The attribute.</param>
		public void WriteAttributeString(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, string value)
		{
			this.WriteStartAttribute(prefix, localName, namespaceUri);
			this.WriteString(value);
			this.WriteEndAttribute();
		}

		/// <summary>Writes an element with a text content.</summary>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceUri">The namespace URI of the element.</param>
		/// <param name="value">The element content.</param>
		public void WriteElementString(XmlDictionaryString localName, XmlDictionaryString namespaceUri, string value)
		{
			this.WriteElementString(null, localName, namespaceUri, value);
		}

		/// <summary>Writes an element with a text content.</summary>
		/// <param name="prefix">The prefix of the element.</param>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceUri">The namespace URI of the element.</param>
		/// <param name="value">The element content.</param>
		public void WriteElementString(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, string value)
		{
			this.WriteStartElement(prefix, localName, namespaceUri);
			this.WriteString(value);
			this.WriteEndElement();
		}

		/// <summary>Writes the given text content.</summary>
		/// <param name="value">The text to write.</param>
		public virtual void WriteString(XmlDictionaryString value)
		{
			this.WriteString(XmlDictionaryString.GetString(value));
		}

		/// <summary>Writes out the namespace-qualified name. This method looks up the prefix that is in scope for the given namespace.</summary>
		/// <param name="localName">The local name of the qualified name.</param>
		/// <param name="namespaceUri">The namespace URI of the qualified name.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="localName" /> is <see langword="null" />.</exception>
		public virtual void WriteQualifiedName(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
		{
			if (localName == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("localName"));
			}
			if (namespaceUri == null)
			{
				namespaceUri = XmlDictionaryString.Empty;
			}
			this.WriteQualifiedName(localName.Value, namespaceUri.Value);
		}

		/// <summary>Writes a <see cref="T:System.Xml.XmlDictionaryString" /> value.</summary>
		/// <param name="value">The <see cref="T:System.Xml.XmlDictionaryString" /> value.</param>
		public virtual void WriteValue(XmlDictionaryString value)
		{
			this.WriteValue(XmlDictionaryString.GetString(value));
		}

		/// <summary>Writes a value from an <see cref="T:System.Xml.IStreamProvider" />.</summary>
		/// <param name="value">The <see cref="T:System.Xml.IStreamProvider" /> value to write.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Xml.XmlException">
		///   <paramref name="value" /> returns a <see langword="null" /> stream object.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlDictionaryWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message "An asynchronous operation is already in progress."</exception>
		public virtual void WriteValue(IStreamProvider value)
		{
			if (value == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
			}
			Stream stream = value.GetStream();
			if (stream == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(System.Runtime.Serialization.SR.GetString("Stream returned by IStreamProvider cannot be null.")));
			}
			int num = 256;
			byte[] buffer = new byte[num];
			for (;;)
			{
				int num2 = stream.Read(buffer, 0, num);
				if (num2 <= 0)
				{
					break;
				}
				this.WriteBase64(buffer, 0, num2);
				if (num < 65536 && num2 == num)
				{
					num *= 16;
					buffer = new byte[num];
				}
			}
			value.ReleaseStream(stream);
		}

		/// <summary>Asynchronously writes a value from an <see cref="T:System.Xml.IStreamProvider" />.</summary>
		/// <param name="value">The <see cref="T:System.Xml.IStreamProvider" /> value to write.</param>
		/// <returns>The task that represents the asynchronous <see langword="WriteValue" /> operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlDictionaryWriter" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message "An asynchronous operation is already in progress."
		/// -or-
		/// An <see cref="T:System.Xml.XmlDictionaryWriter" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlWriterSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message "Set XmlWriterSettings.Async to true if you want to use Async Methods."</exception>
		public virtual Task WriteValueAsync(IStreamProvider value)
		{
			return Task.Factory.FromAsync<IStreamProvider>(new Func<IStreamProvider, AsyncCallback, object, IAsyncResult>(this.BeginWriteValue), new Action<IAsyncResult>(this.EndWriteValue), value, null);
		}

		internal virtual IAsyncResult BeginWriteValue(IStreamProvider value, AsyncCallback callback, object state)
		{
			if (value == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
			}
			if (this.FastAsync)
			{
				return new XmlDictionaryWriter.WriteValueFastAsyncResult(this, value, callback, state);
			}
			return new XmlDictionaryWriter.WriteValueAsyncResult(this, value, callback, state);
		}

		internal virtual void EndWriteValue(IAsyncResult result)
		{
			if (this.FastAsync)
			{
				XmlDictionaryWriter.WriteValueFastAsyncResult.End(result);
				return;
			}
			XmlDictionaryWriter.WriteValueAsyncResult.End(result);
		}

		/// <summary>Writes a Unique Id value.</summary>
		/// <param name="value">The Unique Id value to write.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is <see langword="null" />.</exception>
		public virtual void WriteValue(UniqueId value)
		{
			if (value == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
			}
			this.WriteString(value.ToString());
		}

		/// <summary>Writes a <see cref="T:System.Guid" /> value.</summary>
		/// <param name="value">The <see cref="T:System.Guid" /> value to write.</param>
		public virtual void WriteValue(Guid value)
		{
			this.WriteString(value.ToString());
		}

		/// <summary>Writes a <see cref="T:System.TimeSpan" /> value.</summary>
		/// <param name="value">The <see cref="T:System.TimeSpan" /> value to write.</param>
		public virtual void WriteValue(TimeSpan value)
		{
			this.WriteString(XmlConvert.ToString(value));
		}

		/// <summary>This property always returns <see langword="false" />. Its derived classes can override to return <see langword="true" /> if they support canonicalization.</summary>
		/// <returns>
		///   <see langword="false" /> in all cases.</returns>
		public virtual bool CanCanonicalize
		{
			get
			{
				return false;
			}
		}

		/// <summary>When implemented by a derived class, it starts the canonicalization.</summary>
		/// <param name="stream">The stream to write to.</param>
		/// <param name="includeComments">
		///   <see langword="true" /> to include comments; otherwise, <see langword="false" />.</param>
		/// <param name="inclusivePrefixes">The prefixes to be included.</param>
		/// <exception cref="T:System.NotSupportedException">Method is not implemented yet.</exception>
		public virtual void StartCanonicalization(Stream stream, bool includeComments, string[] inclusivePrefixes)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
		}

		/// <summary>When implemented by a derived class, it stops the canonicalization started by the matching <see cref="M:System.Xml.XmlDictionaryWriter.StartCanonicalization(System.IO.Stream,System.Boolean,System.String[])" /> call.</summary>
		/// <exception cref="T:System.NotSupportedException">Method is not implemented yet.</exception>
		public virtual void EndCanonicalization()
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
		}

		private void WriteElementNode(XmlDictionaryReader reader, bool defattr)
		{
			XmlDictionaryString localName;
			XmlDictionaryString namespaceUri;
			if (reader.TryGetLocalNameAsDictionaryString(out localName) && reader.TryGetNamespaceUriAsDictionaryString(out namespaceUri))
			{
				this.WriteStartElement(reader.Prefix, localName, namespaceUri);
			}
			else
			{
				this.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
			}
			if ((defattr || (!reader.IsDefault && (reader.SchemaInfo == null || !reader.SchemaInfo.IsDefault))) && reader.MoveToFirstAttribute())
			{
				do
				{
					if (reader.TryGetLocalNameAsDictionaryString(out localName) && reader.TryGetNamespaceUriAsDictionaryString(out namespaceUri))
					{
						this.WriteStartAttribute(reader.Prefix, localName, namespaceUri);
					}
					else
					{
						this.WriteStartAttribute(reader.Prefix, reader.LocalName, reader.NamespaceURI);
					}
					while (reader.ReadAttributeValue())
					{
						if (reader.NodeType == XmlNodeType.EntityReference)
						{
							this.WriteEntityRef(reader.Name);
						}
						else
						{
							this.WriteTextNode(reader, true);
						}
					}
					this.WriteEndAttribute();
				}
				while (reader.MoveToNextAttribute());
				reader.MoveToElement();
			}
			if (reader.IsEmptyElement)
			{
				this.WriteEndElement();
			}
		}

		private void WriteArrayNode(XmlDictionaryReader reader, string prefix, string localName, string namespaceUri, Type type)
		{
			if (type == typeof(bool))
			{
				BooleanArrayHelperWithString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(short))
			{
				Int16ArrayHelperWithString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(int))
			{
				Int32ArrayHelperWithString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(long))
			{
				Int64ArrayHelperWithString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(float))
			{
				SingleArrayHelperWithString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(double))
			{
				DoubleArrayHelperWithString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(decimal))
			{
				DecimalArrayHelperWithString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(DateTime))
			{
				DateTimeArrayHelperWithString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(Guid))
			{
				GuidArrayHelperWithString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(TimeSpan))
			{
				TimeSpanArrayHelperWithString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			this.WriteElementNode(reader, false);
			reader.Read();
		}

		private void WriteArrayNode(XmlDictionaryReader reader, string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, Type type)
		{
			if (type == typeof(bool))
			{
				BooleanArrayHelperWithDictionaryString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(short))
			{
				Int16ArrayHelperWithDictionaryString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(int))
			{
				Int32ArrayHelperWithDictionaryString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(long))
			{
				Int64ArrayHelperWithDictionaryString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(float))
			{
				SingleArrayHelperWithDictionaryString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(double))
			{
				DoubleArrayHelperWithDictionaryString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(decimal))
			{
				DecimalArrayHelperWithDictionaryString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(DateTime))
			{
				DateTimeArrayHelperWithDictionaryString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(Guid))
			{
				GuidArrayHelperWithDictionaryString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			if (type == typeof(TimeSpan))
			{
				TimeSpanArrayHelperWithDictionaryString.Instance.WriteArray(this, prefix, localName, namespaceUri, reader);
				return;
			}
			this.WriteElementNode(reader, false);
			reader.Read();
		}

		private void WriteArrayNode(XmlDictionaryReader reader, Type type)
		{
			XmlDictionaryString localName;
			XmlDictionaryString namespaceUri;
			if (reader.TryGetLocalNameAsDictionaryString(out localName) && reader.TryGetNamespaceUriAsDictionaryString(out namespaceUri))
			{
				this.WriteArrayNode(reader, reader.Prefix, localName, namespaceUri, type);
				return;
			}
			this.WriteArrayNode(reader, reader.Prefix, reader.LocalName, reader.NamespaceURI, type);
		}

		/// <summary>Writes the text node that an <see cref="T:System.Xml.XmlDictionaryReader" /> is currently positioned on.</summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlDictionaryReader" /> to get the text value from.</param>
		/// <param name="isAttribute">
		///   <see langword="true" /> to indicate that the reader is positioned on an attribute value or element content; otherwise, <see langword="false" />.</param>
		protected virtual void WriteTextNode(XmlDictionaryReader reader, bool isAttribute)
		{
			XmlDictionaryString value;
			if (reader.TryGetValueAsDictionaryString(out value))
			{
				this.WriteString(value);
			}
			else
			{
				this.WriteString(reader.Value);
			}
			if (!isAttribute)
			{
				reader.Read();
			}
		}

		/// <summary>Writes the current XML node from an <see cref="T:System.Xml.XmlReader" />.</summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader" />.</param>
		/// <param name="defattr">
		///   <see langword="true" /> to copy the default attributes from the <see cref="T:System.Xml.XmlReader" />; otherwise, <see langword="false" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="reader" /> is <see langword="null" />.</exception>
		public override void WriteNode(XmlReader reader, bool defattr)
		{
			XmlDictionaryReader xmlDictionaryReader = reader as XmlDictionaryReader;
			if (xmlDictionaryReader != null)
			{
				this.WriteNode(xmlDictionaryReader, defattr);
				return;
			}
			base.WriteNode(reader, defattr);
		}

		/// <summary>Writes the current XML node from an <see cref="T:System.Xml.XmlDictionaryReader" />.</summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlDictionaryReader" />.</param>
		/// <param name="defattr">
		///   <see langword="true" /> to copy the default attributes from the <see langword="XmlReader" />; otherwise, <see langword="false" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="reader" /> is <see langword="null" />.</exception>
		public virtual void WriteNode(XmlDictionaryReader reader, bool defattr)
		{
			if (reader == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("reader"));
			}
			int num = (reader.NodeType == XmlNodeType.None) ? -1 : reader.Depth;
			do
			{
				XmlNodeType nodeType = reader.NodeType;
				if (nodeType == XmlNodeType.Text || nodeType == XmlNodeType.Whitespace || nodeType == XmlNodeType.SignificantWhitespace)
				{
					this.WriteTextNode(reader, false);
				}
				else
				{
					Type type;
					if (reader.Depth <= num || !reader.IsStartArray(out type))
					{
						switch (nodeType)
						{
						case XmlNodeType.Element:
							this.WriteElementNode(reader, defattr);
							break;
						case XmlNodeType.Attribute:
						case XmlNodeType.Text:
						case XmlNodeType.Entity:
						case XmlNodeType.Document:
							break;
						case XmlNodeType.CDATA:
							this.WriteCData(reader.Value);
							break;
						case XmlNodeType.EntityReference:
							this.WriteEntityRef(reader.Name);
							break;
						case XmlNodeType.ProcessingInstruction:
							goto IL_C9;
						case XmlNodeType.Comment:
							this.WriteComment(reader.Value);
							break;
						case XmlNodeType.DocumentType:
							this.WriteDocType(reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value);
							break;
						default:
							if (nodeType != XmlNodeType.EndElement)
							{
								if (nodeType == XmlNodeType.XmlDeclaration)
								{
									goto IL_C9;
								}
							}
							else
							{
								this.WriteFullEndElement();
							}
							break;
						}
						IL_11B:
						if (reader.Read())
						{
							goto IL_123;
						}
						break;
						IL_C9:
						this.WriteProcessingInstruction(reader.Name, reader.Value);
						goto IL_11B;
					}
					this.WriteArrayNode(reader, type);
				}
				IL_123:;
			}
			while (num < reader.Depth || (num == reader.Depth && reader.NodeType == XmlNodeType.EndElement));
		}

		private void CheckArray(Array array, int offset, int count)
		{
			if (array == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("array"));
			}
			if (offset < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (offset > array.Length)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", System.Runtime.Serialization.SR.GetString("The specified offset exceeds the buffer size ({0} bytes).", new object[]
				{
					array.Length
				})));
			}
			if (count < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			if (count > array.Length - offset)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", System.Runtime.Serialization.SR.GetString("The specified size exceeds the remaining buffer space ({0} bytes).", new object[]
				{
					array.Length - offset
				})));
			}
		}

		/// <summary>Writes nodes from a <see cref="T:System.Boolean" /> array.</summary>
		/// <param name="prefix">The namespace prefix.</param>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceUri">The namespace URI of the element.</param>
		/// <param name="array">The array that contains the data.</param>
		/// <param name="offset">The starting index in the array.</param>
		/// <param name="count">The number of values to write from the array.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is &lt; 0 or &gt; <paramref name="array" /> length.
		/// -or-
		/// <paramref name="count" /> is &lt; 0 or &gt; <paramref name="array" /> length minus <paramref name="offset" />.</exception>
		public virtual void WriteArray(string prefix, string localName, string namespaceUri, bool[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			for (int i = 0; i < count; i++)
			{
				this.WriteStartElement(prefix, localName, namespaceUri);
				this.WriteValue(array[offset + i]);
				this.WriteEndElement();
			}
		}

		/// <summary>Writes nodes from a <see cref="T:System.Boolean" /> array.</summary>
		/// <param name="prefix">The namespace prefix.</param>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceUri">The namespace URI of the element.</param>
		/// <param name="array">The array that contains the nodes.</param>
		/// <param name="offset">The starting index in the array.</param>
		/// <param name="count">The number of nodes to get from the array.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is &lt; 0 or &gt; <paramref name="array" /> length.
		/// -or-
		/// <paramref name="count" /> is &lt; 0 or &gt; <paramref name="array" /> length minus <paramref name="offset" />.</exception>
		public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, bool[] array, int offset, int count)
		{
			this.WriteArray(prefix, XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri), array, offset, count);
		}

		/// <summary>Writes nodes from a <see cref="T:System.Int16" /> array.</summary>
		/// <param name="prefix">The namespace prefix.</param>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceUri">The namespace URI of the element.</param>
		/// <param name="array">The array that contains the nodes.</param>
		/// <param name="offset">The starting index in the array.</param>
		/// <param name="count">The number of nodes to get from the array.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is &lt; 0 or &gt; <paramref name="array" /> length.
		/// -or-
		/// <paramref name="count" /> is &lt; 0 or &gt; <paramref name="array" /> length minus <paramref name="offset" />.</exception>
		public virtual void WriteArray(string prefix, string localName, string namespaceUri, short[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			for (int i = 0; i < count; i++)
			{
				this.WriteStartElement(prefix, localName, namespaceUri);
				this.WriteValue((int)array[offset + i]);
				this.WriteEndElement();
			}
		}

		/// <summary>Writes nodes from a <see cref="T:System.Int16" /> array.</summary>
		/// <param name="prefix">The namespace prefix.</param>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceUri">The namespace URI of the element.</param>
		/// <param name="array">The array that contains the nodes.</param>
		/// <param name="offset">The starting index in the array.</param>
		/// <param name="count">The number of nodes to get from the array.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is &lt; 0 or &gt; <paramref name="array" /> length.
		/// -or-
		/// <paramref name="count" /> is &lt; 0 or &gt; <paramref name="array" /> length minus <paramref name="offset" />.</exception>
		public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, short[] array, int offset, int count)
		{
			this.WriteArray(prefix, XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri), array, offset, count);
		}

		/// <summary>Writes nodes from a <see cref="T:System.Int32" /> array.</summary>
		/// <param name="prefix">The namespace prefix.</param>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceUri">The namespace URI of the element.</param>
		/// <param name="array">The array that contains the nodes.</param>
		/// <param name="offset">The starting index in the array.</param>
		/// <param name="count">The number of nodes to get from the array.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is &lt; 0 or &gt; <paramref name="array" /> length.
		/// -or-
		/// <paramref name="count" /> is &lt; 0 or &gt; <paramref name="array" /> length minus <paramref name="offset" />.</exception>
		public virtual void WriteArray(string prefix, string localName, string namespaceUri, int[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			for (int i = 0; i < count; i++)
			{
				this.WriteStartElement(prefix, localName, namespaceUri);
				this.WriteValue(array[offset + i]);
				this.WriteEndElement();
			}
		}

		/// <summary>Writes nodes from a <see cref="T:System.Int32" /> array.</summary>
		/// <param name="prefix">The namespace prefix.</param>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceUri">The namespace URI of the element.</param>
		/// <param name="array">The array that contains the nodes.</param>
		/// <param name="offset">The starting index in the array.</param>
		/// <param name="count">The number of nodes to get from the array.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is &lt; 0 or &gt; <paramref name="array" /> length.
		/// -or-
		/// <paramref name="count" /> is &lt; 0 or &gt; <paramref name="array" /> length minus <paramref name="offset" />.</exception>
		public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, int[] array, int offset, int count)
		{
			this.WriteArray(prefix, XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri), array, offset, count);
		}

		/// <summary>Writes nodes from a <see cref="T:System.Int64" /> array.</summary>
		/// <param name="prefix">The namespace prefix.</param>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceUri">The namespace URI of the element.</param>
		/// <param name="array">The array that contains the nodes.</param>
		/// <param name="offset">The starting index in the array.</param>
		/// <param name="count">The number of nodes to get from the array.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is &lt; 0 or &gt; <paramref name="array" /> length.
		/// -or-
		/// <paramref name="count" /> is &lt; 0 or &gt; <paramref name="array" /> length minus <paramref name="offset" />.</exception>
		public virtual void WriteArray(string prefix, string localName, string namespaceUri, long[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			for (int i = 0; i < count; i++)
			{
				this.WriteStartElement(prefix, localName, namespaceUri);
				this.WriteValue(array[offset + i]);
				this.WriteEndElement();
			}
		}

		/// <summary>Writes nodes from a <see cref="T:System.Int64" /> array.</summary>
		/// <param name="prefix">The namespace prefix.</param>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceUri">The namespace URI of the element.</param>
		/// <param name="array">The array that contains the nodes.</param>
		/// <param name="offset">The starting index in the array.</param>
		/// <param name="count">The number of nodes to get from the array.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is &lt; 0 or &gt; <paramref name="array" /> length.
		/// -or-
		/// <paramref name="count" /> is &lt; 0 or &gt; <paramref name="array" /> length minus <paramref name="offset" />.</exception>
		public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, long[] array, int offset, int count)
		{
			this.WriteArray(prefix, XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri), array, offset, count);
		}

		/// <summary>Writes nodes from a <see cref="T:System.Single" /> array.</summary>
		/// <param name="prefix">The namespace prefix.</param>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceUri">The namespace URI of the element.</param>
		/// <param name="array">The array that contains the nodes.</param>
		/// <param name="offset">The starting index in the array.</param>
		/// <param name="count">The number of nodes to get from the array.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is &lt; 0 or &gt; <paramref name="array" /> length.
		/// -or-
		/// <paramref name="count" /> is &lt; 0 or &gt; <paramref name="array" /> length minus <paramref name="offset" />.</exception>
		public virtual void WriteArray(string prefix, string localName, string namespaceUri, float[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			for (int i = 0; i < count; i++)
			{
				this.WriteStartElement(prefix, localName, namespaceUri);
				this.WriteValue(array[offset + i]);
				this.WriteEndElement();
			}
		}

		/// <summary>Writes nodes from a <see cref="T:System.Single" /> array.</summary>
		/// <param name="prefix">The namespace prefix.</param>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceUri">The namespace URI of the element.</param>
		/// <param name="array">The array that contains the nodes.</param>
		/// <param name="offset">The starting index in the array.</param>
		/// <param name="count">The number of nodes to get from the array.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is &lt; 0 or &gt; <paramref name="array" /> length.
		/// -or-
		/// <paramref name="count" /> is &lt; 0 or &gt; <paramref name="array" /> length minus <paramref name="offset" />.</exception>
		public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, float[] array, int offset, int count)
		{
			this.WriteArray(prefix, XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri), array, offset, count);
		}

		/// <summary>Writes nodes from a <see cref="T:System.Double" /> array.</summary>
		/// <param name="prefix">The namespace prefix.</param>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceUri">The namespace URI of the element.</param>
		/// <param name="array">The array that contains the nodes.</param>
		/// <param name="offset">The starting index in the array.</param>
		/// <param name="count">The number of nodes to get from the array.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is &lt; 0 or &gt; <paramref name="array" /> length.
		/// -or-
		/// <paramref name="count" /> is &lt; 0 or &gt; <paramref name="array" /> length minus <paramref name="offset" />.</exception>
		public virtual void WriteArray(string prefix, string localName, string namespaceUri, double[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			for (int i = 0; i < count; i++)
			{
				this.WriteStartElement(prefix, localName, namespaceUri);
				this.WriteValue(array[offset + i]);
				this.WriteEndElement();
			}
		}

		/// <summary>Writes nodes from a <see cref="T:System.Double" /> array.</summary>
		/// <param name="prefix">The namespace prefix.</param>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceUri">The namespace URI of the element.</param>
		/// <param name="array">The array that contains the nodes.</param>
		/// <param name="offset">The starting index in the array.</param>
		/// <param name="count">The number of nodes to get from the array.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is &lt; 0 or &gt; <paramref name="array" /> length.
		/// -or-
		/// <paramref name="count" /> is &lt; 0 or &gt; <paramref name="array" /> length minus <paramref name="offset" />.</exception>
		public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, double[] array, int offset, int count)
		{
			this.WriteArray(prefix, XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri), array, offset, count);
		}

		/// <summary>Writes nodes from a <see cref="T:System.Decimal" /> array.</summary>
		/// <param name="prefix">The namespace prefix.</param>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceUri">The namespace URI of the element.</param>
		/// <param name="array">The array that contains the nodes.</param>
		/// <param name="offset">The starting index in the array.</param>
		/// <param name="count">The number of nodes to get from the array.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is &lt; 0 or &gt; <paramref name="array" /> length.
		/// -or-
		/// <paramref name="count" /> is &lt; 0 or &gt; <paramref name="array" /> length minus <paramref name="offset" />.</exception>
		public virtual void WriteArray(string prefix, string localName, string namespaceUri, decimal[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			for (int i = 0; i < count; i++)
			{
				this.WriteStartElement(prefix, localName, namespaceUri);
				this.WriteValue(array[offset + i]);
				this.WriteEndElement();
			}
		}

		/// <summary>Writes nodes from a <see cref="T:System.Decimal" /> array.</summary>
		/// <param name="prefix">The namespace prefix.</param>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceUri">The namespace URI of the element.</param>
		/// <param name="array">The array that contains the nodes.</param>
		/// <param name="offset">The starting index in the array.</param>
		/// <param name="count">The number of nodes to get from the array.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is &lt; 0 or &gt; <paramref name="array" /> length.
		/// -or-
		/// <paramref name="count" /> is &lt; 0 or &gt; <paramref name="array" /> length minus <paramref name="offset" />.</exception>
		public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, decimal[] array, int offset, int count)
		{
			this.WriteArray(prefix, XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri), array, offset, count);
		}

		/// <summary>Writes nodes from a <see cref="T:System.DateTime" /> array.</summary>
		/// <param name="prefix">The namespace prefix.</param>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceUri">The namespace URI of the element.</param>
		/// <param name="array">The array that contains the nodes.</param>
		/// <param name="offset">The starting index in the array.</param>
		/// <param name="count">The number of nodes to get from the array.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is &lt; 0 or &gt; <paramref name="array" /> length.
		/// -or-
		/// <paramref name="count" /> is &lt; 0 or &gt; <paramref name="array" /> length minus <paramref name="offset" />.</exception>
		public virtual void WriteArray(string prefix, string localName, string namespaceUri, DateTime[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			for (int i = 0; i < count; i++)
			{
				this.WriteStartElement(prefix, localName, namespaceUri);
				this.WriteValue(array[offset + i]);
				this.WriteEndElement();
			}
		}

		/// <summary>Writes nodes from a <see cref="T:System.DateTime" /> array.</summary>
		/// <param name="prefix">The namespace prefix.</param>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceUri">The namespace URI of the element.</param>
		/// <param name="array">The array that contains the nodes.</param>
		/// <param name="offset">The starting index in the array.</param>
		/// <param name="count">The number of nodes to get from the array.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is &lt; 0 or &gt; <paramref name="array" /> length.
		/// -or-
		/// <paramref name="count" /> is &lt; 0 or &gt; <paramref name="array" /> length minus <paramref name="offset" />.</exception>
		public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, DateTime[] array, int offset, int count)
		{
			this.WriteArray(prefix, XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri), array, offset, count);
		}

		/// <summary>Writes nodes from a <see cref="T:System.Guid" /> array.</summary>
		/// <param name="prefix">The namespace prefix.</param>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceUri">The namespace URI of the element.</param>
		/// <param name="array">The array that contains the nodes.</param>
		/// <param name="offset">The starting index in the array.</param>
		/// <param name="count">The number of nodes to get from the array.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is &lt; 0 or &gt; <paramref name="array" /> length.
		/// -or-
		/// <paramref name="count" /> is &lt; 0 or &gt; <paramref name="array" /> length minus <paramref name="offset" />.</exception>
		public virtual void WriteArray(string prefix, string localName, string namespaceUri, Guid[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			for (int i = 0; i < count; i++)
			{
				this.WriteStartElement(prefix, localName, namespaceUri);
				this.WriteValue(array[offset + i]);
				this.WriteEndElement();
			}
		}

		/// <summary>Writes nodes from a <see cref="T:System.Guid" /> array.</summary>
		/// <param name="prefix">The namespace prefix.</param>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceUri">The namespace URI of the element.</param>
		/// <param name="array">The array that contains the nodes.</param>
		/// <param name="offset">The starting index in the array.</param>
		/// <param name="count">The number of nodes to get from the array.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is &lt; 0 or &gt; <paramref name="array" /> length.
		/// -or-
		/// <paramref name="count" /> is &lt; 0 or &gt; <paramref name="array" /> length minus <paramref name="offset" />.</exception>
		public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, Guid[] array, int offset, int count)
		{
			this.WriteArray(prefix, XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri), array, offset, count);
		}

		/// <summary>Writes nodes from a <see cref="T:System.TimeSpan" /> array.</summary>
		/// <param name="prefix">The namespace prefix.</param>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceUri">The namespace URI of the element.</param>
		/// <param name="array">The array that contains the nodes.</param>
		/// <param name="offset">The starting index in the array.</param>
		/// <param name="count">The number of nodes to get from the array.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is &lt; 0 or &gt; <paramref name="array" /> length.
		/// -or-
		/// <paramref name="count" /> is &lt; 0 or &gt; <paramref name="array" /> length minus <paramref name="offset" />.</exception>
		public virtual void WriteArray(string prefix, string localName, string namespaceUri, TimeSpan[] array, int offset, int count)
		{
			this.CheckArray(array, offset, count);
			for (int i = 0; i < count; i++)
			{
				this.WriteStartElement(prefix, localName, namespaceUri);
				this.WriteValue(array[offset + i]);
				this.WriteEndElement();
			}
		}

		/// <summary>Writes nodes from a <see cref="T:System.TimeSpan" /> array.</summary>
		/// <param name="prefix">The namespace prefix.</param>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceUri">The namespace URI of the element.</param>
		/// <param name="array">The array that contains the nodes.</param>
		/// <param name="offset">The starting index in the array.</param>
		/// <param name="count">The number of nodes to get from the array.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is &lt; 0 or &gt; <paramref name="array" /> length.
		/// -or-
		/// <paramref name="count" /> is &lt; 0 or &gt; <paramref name="array" /> length minus <paramref name="offset" />.</exception>
		public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, TimeSpan[] array, int offset, int count)
		{
			this.WriteArray(prefix, XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(namespaceUri), array, offset, count);
		}

		private class WriteValueFastAsyncResult : AsyncResult
		{
			public WriteValueFastAsyncResult(XmlDictionaryWriter writer, IStreamProvider value, AsyncCallback callback, object state) : base(callback, state)
			{
				this.streamProvider = value;
				this.writer = writer;
				this.stream = value.GetStream();
				if (this.stream == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(System.Runtime.Serialization.SR.GetString("Stream returned by IStreamProvider cannot be null.")));
				}
				this.blockSize = 256;
				this.bytesRead = 0;
				this.block = new byte[this.blockSize];
				this.nextOperation = XmlDictionaryWriter.WriteValueFastAsyncResult.Operation.Read;
				this.ContinueWork(true, null);
			}

			private void CompleteAndReleaseStream(bool completedSynchronously, Exception completionException = null)
			{
				if (completionException == null)
				{
					this.streamProvider.ReleaseStream(this.stream);
					this.stream = null;
				}
				base.Complete(completedSynchronously, completionException);
			}

			private void ContinueWork(bool completedSynchronously, Exception completionException = null)
			{
				try
				{
					for (;;)
					{
						if (this.nextOperation == XmlDictionaryWriter.WriteValueFastAsyncResult.Operation.Read)
						{
							if (this.ReadAsync() != AsyncCompletionResult.Completed)
							{
								break;
							}
						}
						else if (this.nextOperation == XmlDictionaryWriter.WriteValueFastAsyncResult.Operation.Write)
						{
							if (this.WriteAsync() != AsyncCompletionResult.Completed)
							{
								break;
							}
						}
						else if (this.nextOperation == XmlDictionaryWriter.WriteValueFastAsyncResult.Operation.Complete)
						{
							goto Block_6;
						}
					}
					return;
					Block_6:;
				}
				catch (Exception ex)
				{
					if (Fx.IsFatal(ex))
					{
						throw;
					}
					if (completedSynchronously)
					{
						throw;
					}
					if (completionException == null)
					{
						completionException = ex;
					}
				}
				if (!this.completed)
				{
					this.completed = true;
					this.CompleteAndReleaseStream(completedSynchronously, completionException);
				}
			}

			private AsyncCompletionResult ReadAsync()
			{
				IAsyncResult asyncResult = this.stream.BeginRead(this.block, 0, this.blockSize, XmlDictionaryWriter.WriteValueFastAsyncResult.onReadComplete, this);
				if (asyncResult.CompletedSynchronously)
				{
					this.HandleReadComplete(asyncResult);
					return AsyncCompletionResult.Completed;
				}
				return AsyncCompletionResult.Queued;
			}

			private void HandleReadComplete(IAsyncResult result)
			{
				this.bytesRead = this.stream.EndRead(result);
				if (this.bytesRead > 0)
				{
					this.nextOperation = XmlDictionaryWriter.WriteValueFastAsyncResult.Operation.Write;
					return;
				}
				this.nextOperation = XmlDictionaryWriter.WriteValueFastAsyncResult.Operation.Complete;
			}

			private static void OnReadComplete(IAsyncResult result)
			{
				if (result.CompletedSynchronously)
				{
					return;
				}
				Exception completionException = null;
				XmlDictionaryWriter.WriteValueFastAsyncResult writeValueFastAsyncResult = (XmlDictionaryWriter.WriteValueFastAsyncResult)result.AsyncState;
				bool flag = false;
				try
				{
					writeValueFastAsyncResult.HandleReadComplete(result);
					flag = true;
				}
				catch (Exception ex)
				{
					if (Fx.IsFatal(ex))
					{
						throw;
					}
					completionException = ex;
				}
				if (!flag)
				{
					writeValueFastAsyncResult.nextOperation = XmlDictionaryWriter.WriteValueFastAsyncResult.Operation.Complete;
				}
				writeValueFastAsyncResult.ContinueWork(false, completionException);
			}

			private AsyncCompletionResult WriteAsync()
			{
				if (this.writerAsyncState == null)
				{
					this.writerAsyncArgs = new XmlWriteBase64AsyncArguments();
					this.writerAsyncState = new AsyncEventArgs<XmlWriteBase64AsyncArguments>();
				}
				if (XmlDictionaryWriter.WriteValueFastAsyncResult.onWriteComplete == null)
				{
					XmlDictionaryWriter.WriteValueFastAsyncResult.onWriteComplete = new AsyncEventArgsCallback(XmlDictionaryWriter.WriteValueFastAsyncResult.OnWriteComplete);
				}
				this.writerAsyncArgs.Buffer = this.block;
				this.writerAsyncArgs.Offset = 0;
				this.writerAsyncArgs.Count = this.bytesRead;
				this.writerAsyncState.Set(XmlDictionaryWriter.WriteValueFastAsyncResult.onWriteComplete, this.writerAsyncArgs, this);
				if (this.writer.WriteBase64Async(this.writerAsyncState) == AsyncCompletionResult.Completed)
				{
					this.HandleWriteComplete();
					this.writerAsyncState.Complete(true);
					return AsyncCompletionResult.Completed;
				}
				return AsyncCompletionResult.Queued;
			}

			private void HandleWriteComplete()
			{
				this.nextOperation = XmlDictionaryWriter.WriteValueFastAsyncResult.Operation.Read;
				if (this.blockSize < 65536 && this.bytesRead == this.blockSize)
				{
					this.blockSize *= 16;
					this.block = new byte[this.blockSize];
				}
			}

			private static void OnWriteComplete(IAsyncEventArgs asyncState)
			{
				XmlDictionaryWriter.WriteValueFastAsyncResult writeValueFastAsyncResult = (XmlDictionaryWriter.WriteValueFastAsyncResult)asyncState.AsyncState;
				Exception completionException = null;
				bool flag = false;
				try
				{
					if (asyncState.Exception != null)
					{
						completionException = asyncState.Exception;
					}
					else
					{
						writeValueFastAsyncResult.HandleWriteComplete();
						flag = true;
					}
				}
				catch (Exception ex)
				{
					if (Fx.IsFatal(ex))
					{
						throw;
					}
					completionException = ex;
				}
				if (!flag)
				{
					writeValueFastAsyncResult.nextOperation = XmlDictionaryWriter.WriteValueFastAsyncResult.Operation.Complete;
				}
				writeValueFastAsyncResult.ContinueWork(false, completionException);
			}

			internal static void End(IAsyncResult result)
			{
				AsyncResult.End<XmlDictionaryWriter.WriteValueFastAsyncResult>(result);
			}

			private bool completed;

			private int blockSize;

			private byte[] block;

			private int bytesRead;

			private Stream stream;

			private XmlDictionaryWriter.WriteValueFastAsyncResult.Operation nextOperation;

			private IStreamProvider streamProvider;

			private XmlDictionaryWriter writer;

			private AsyncEventArgs<XmlWriteBase64AsyncArguments> writerAsyncState;

			private XmlWriteBase64AsyncArguments writerAsyncArgs;

			private static AsyncCallback onReadComplete = Fx.ThunkCallback(new AsyncCallback(XmlDictionaryWriter.WriteValueFastAsyncResult.OnReadComplete));

			private static AsyncEventArgsCallback onWriteComplete;

			private enum Operation
			{
				Read,
				Write,
				Complete
			}
		}

		private class WriteValueAsyncResult : AsyncResult
		{
			public WriteValueAsyncResult(XmlDictionaryWriter writer, IStreamProvider value, AsyncCallback callback, object state) : base(callback, state)
			{
				this.streamProvider = value;
				this.writer = writer;
				this.writeBlockHandler = ((this.writer.Settings != null && this.writer.Settings.Async) ? XmlDictionaryWriter.WriteValueAsyncResult.handleWriteBlockAsync : XmlDictionaryWriter.WriteValueAsyncResult.handleWriteBlock);
				this.stream = value.GetStream();
				if (this.stream == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(System.Runtime.Serialization.SR.GetString("Stream returned by IStreamProvider cannot be null.")));
				}
				this.blockSize = 256;
				this.bytesRead = 0;
				this.block = new byte[this.blockSize];
				if (this.ContinueWork(null))
				{
					this.CompleteAndReleaseStream(true, null);
				}
			}

			private void AdjustBlockSize()
			{
				if (this.blockSize < 65536 && this.bytesRead == this.blockSize)
				{
					this.blockSize *= 16;
					this.block = new byte[this.blockSize];
				}
			}

			private void CompleteAndReleaseStream(bool completedSynchronously, Exception completionException)
			{
				if (completionException == null)
				{
					this.streamProvider.ReleaseStream(this.stream);
					this.stream = null;
				}
				base.Complete(completedSynchronously, completionException);
			}

			private bool ContinueWork(IAsyncResult result)
			{
				for (;;)
				{
					if (this.operation == XmlDictionaryWriter.WriteValueAsyncResult.Operation.Read)
					{
						if (!this.HandleReadBlock(result))
						{
							return false;
						}
						if (this.bytesRead <= 0)
						{
							break;
						}
						this.operation = XmlDictionaryWriter.WriteValueAsyncResult.Operation.Write;
					}
					else
					{
						if (!this.writeBlockHandler(result, this))
						{
							return false;
						}
						this.AdjustBlockSize();
						this.operation = XmlDictionaryWriter.WriteValueAsyncResult.Operation.Read;
					}
					result = null;
				}
				return true;
			}

			private bool HandleReadBlock(IAsyncResult result)
			{
				if (result == null)
				{
					result = this.stream.BeginRead(this.block, 0, this.blockSize, XmlDictionaryWriter.WriteValueAsyncResult.onContinueWork, this);
					if (!result.CompletedSynchronously)
					{
						return false;
					}
				}
				this.bytesRead = this.stream.EndRead(result);
				return true;
			}

			private static bool HandleWriteBlock(IAsyncResult result, XmlDictionaryWriter.WriteValueAsyncResult thisPtr)
			{
				if (result == null)
				{
					result = thisPtr.writer.BeginWriteBase64(thisPtr.block, 0, thisPtr.bytesRead, XmlDictionaryWriter.WriteValueAsyncResult.onContinueWork, thisPtr);
					if (!result.CompletedSynchronously)
					{
						return false;
					}
				}
				thisPtr.writer.EndWriteBase64(result);
				return true;
			}

			private static bool HandleWriteBlockAsync(IAsyncResult result, XmlDictionaryWriter.WriteValueAsyncResult thisPtr)
			{
				Task task = (Task)result;
				if (task == null)
				{
					task = thisPtr.writer.WriteBase64Async(thisPtr.block, 0, thisPtr.bytesRead);
					task.AsAsyncResult(XmlDictionaryWriter.WriteValueAsyncResult.onContinueWork, thisPtr);
					return false;
				}
				task.GetAwaiter().GetResult();
				return true;
			}

			private static void OnContinueWork(IAsyncResult result)
			{
				if (result.CompletedSynchronously && !(result is Task))
				{
					return;
				}
				Exception completionException = null;
				XmlDictionaryWriter.WriteValueAsyncResult writeValueAsyncResult = (XmlDictionaryWriter.WriteValueAsyncResult)result.AsyncState;
				bool flag = false;
				try
				{
					flag = writeValueAsyncResult.ContinueWork(result);
				}
				catch (Exception ex)
				{
					if (Fx.IsFatal(ex))
					{
						throw;
					}
					flag = true;
					completionException = ex;
				}
				if (flag)
				{
					writeValueAsyncResult.CompleteAndReleaseStream(false, completionException);
				}
			}

			public static void End(IAsyncResult result)
			{
				AsyncResult.End<XmlDictionaryWriter.WriteValueAsyncResult>(result);
			}

			private int blockSize;

			private byte[] block;

			private int bytesRead;

			private Stream stream;

			private XmlDictionaryWriter.WriteValueAsyncResult.Operation operation;

			private IStreamProvider streamProvider;

			private XmlDictionaryWriter writer;

			private Func<IAsyncResult, XmlDictionaryWriter.WriteValueAsyncResult, bool> writeBlockHandler;

			private static Func<IAsyncResult, XmlDictionaryWriter.WriteValueAsyncResult, bool> handleWriteBlock = new Func<IAsyncResult, XmlDictionaryWriter.WriteValueAsyncResult, bool>(XmlDictionaryWriter.WriteValueAsyncResult.HandleWriteBlock);

			private static Func<IAsyncResult, XmlDictionaryWriter.WriteValueAsyncResult, bool> handleWriteBlockAsync = new Func<IAsyncResult, XmlDictionaryWriter.WriteValueAsyncResult, bool>(XmlDictionaryWriter.WriteValueAsyncResult.HandleWriteBlockAsync);

			private static AsyncCallback onContinueWork = Fx.ThunkCallback(new AsyncCallback(XmlDictionaryWriter.WriteValueAsyncResult.OnContinueWork));

			private enum Operation
			{
				Read,
				Write
			}
		}

		private class WriteBase64AsyncResult : ScheduleActionItemAsyncResult
		{
			public WriteBase64AsyncResult(byte[] buffer, int index, int count, XmlDictionaryWriter writer, AsyncCallback callback, object state) : base(callback, state)
			{
				this.buffer = buffer;
				this.index = index;
				this.count = count;
				this.writer = writer;
				base.Schedule();
			}

			protected override void OnDoWork()
			{
				this.writer.WriteBase64(this.buffer, this.index, this.count);
			}

			private byte[] buffer;

			private int index;

			private int count;

			private XmlDictionaryWriter writer;
		}

		private class XmlWrappedWriter : XmlDictionaryWriter
		{
			public XmlWrappedWriter(XmlWriter writer)
			{
				this.writer = writer;
				this.depth = 0;
			}

			public override void Close()
			{
				this.writer.Close();
			}

			public override void Flush()
			{
				this.writer.Flush();
			}

			public override string LookupPrefix(string namespaceUri)
			{
				return this.writer.LookupPrefix(namespaceUri);
			}

			public override void WriteAttributes(XmlReader reader, bool defattr)
			{
				this.writer.WriteAttributes(reader, defattr);
			}

			public override void WriteBase64(byte[] buffer, int index, int count)
			{
				this.writer.WriteBase64(buffer, index, count);
			}

			public override void WriteBinHex(byte[] buffer, int index, int count)
			{
				this.writer.WriteBinHex(buffer, index, count);
			}

			public override void WriteCData(string text)
			{
				this.writer.WriteCData(text);
			}

			public override void WriteCharEntity(char ch)
			{
				this.writer.WriteCharEntity(ch);
			}

			public override void WriteChars(char[] buffer, int index, int count)
			{
				this.writer.WriteChars(buffer, index, count);
			}

			public override void WriteComment(string text)
			{
				this.writer.WriteComment(text);
			}

			public override void WriteDocType(string name, string pubid, string sysid, string subset)
			{
				this.writer.WriteDocType(name, pubid, sysid, subset);
			}

			public override void WriteEndAttribute()
			{
				this.writer.WriteEndAttribute();
			}

			public override void WriteEndDocument()
			{
				this.writer.WriteEndDocument();
			}

			public override void WriteEndElement()
			{
				this.writer.WriteEndElement();
				this.depth--;
			}

			public override void WriteEntityRef(string name)
			{
				this.writer.WriteEntityRef(name);
			}

			public override void WriteFullEndElement()
			{
				this.writer.WriteFullEndElement();
			}

			public override void WriteName(string name)
			{
				this.writer.WriteName(name);
			}

			public override void WriteNmToken(string name)
			{
				this.writer.WriteNmToken(name);
			}

			public override void WriteNode(XmlReader reader, bool defattr)
			{
				this.writer.WriteNode(reader, defattr);
			}

			public override void WriteProcessingInstruction(string name, string text)
			{
				this.writer.WriteProcessingInstruction(name, text);
			}

			public override void WriteQualifiedName(string localName, string namespaceUri)
			{
				this.writer.WriteQualifiedName(localName, namespaceUri);
			}

			public override void WriteRaw(char[] buffer, int index, int count)
			{
				this.writer.WriteRaw(buffer, index, count);
			}

			public override void WriteRaw(string data)
			{
				this.writer.WriteRaw(data);
			}

			public override void WriteStartAttribute(string prefix, string localName, string namespaceUri)
			{
				this.writer.WriteStartAttribute(prefix, localName, namespaceUri);
				this.prefix++;
			}

			public override void WriteStartDocument()
			{
				this.writer.WriteStartDocument();
			}

			public override void WriteStartDocument(bool standalone)
			{
				this.writer.WriteStartDocument(standalone);
			}

			public override void WriteStartElement(string prefix, string localName, string namespaceUri)
			{
				this.writer.WriteStartElement(prefix, localName, namespaceUri);
				this.depth++;
				this.prefix = 1;
			}

			public override WriteState WriteState
			{
				get
				{
					return this.writer.WriteState;
				}
			}

			public override void WriteString(string text)
			{
				this.writer.WriteString(text);
			}

			public override void WriteSurrogateCharEntity(char lowChar, char highChar)
			{
				this.writer.WriteSurrogateCharEntity(lowChar, highChar);
			}

			public override void WriteWhitespace(string whitespace)
			{
				this.writer.WriteWhitespace(whitespace);
			}

			public override void WriteValue(object value)
			{
				this.writer.WriteValue(value);
			}

			public override void WriteValue(string value)
			{
				this.writer.WriteValue(value);
			}

			public override void WriteValue(bool value)
			{
				this.writer.WriteValue(value);
			}

			public override void WriteValue(DateTime value)
			{
				this.writer.WriteValue(value);
			}

			public override void WriteValue(double value)
			{
				this.writer.WriteValue(value);
			}

			public override void WriteValue(int value)
			{
				this.writer.WriteValue(value);
			}

			public override void WriteValue(long value)
			{
				this.writer.WriteValue(value);
			}

			public override void WriteXmlnsAttribute(string prefix, string namespaceUri)
			{
				if (namespaceUri == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("namespaceUri");
				}
				if (prefix == null)
				{
					if (this.LookupPrefix(namespaceUri) != null)
					{
						return;
					}
					if (namespaceUri.Length == 0)
					{
						prefix = string.Empty;
					}
					else
					{
						string str = this.depth.ToString(NumberFormatInfo.InvariantInfo);
						string str2 = this.prefix.ToString(NumberFormatInfo.InvariantInfo);
						prefix = "d" + str + "p" + str2;
					}
				}
				base.WriteAttributeString("xmlns", prefix, null, namespaceUri);
			}

			public override string XmlLang
			{
				get
				{
					return this.writer.XmlLang;
				}
			}

			public override XmlSpace XmlSpace
			{
				get
				{
					return this.writer.XmlSpace;
				}
			}

			private XmlWriter writer;

			private int depth;

			private int prefix;
		}
	}
}
