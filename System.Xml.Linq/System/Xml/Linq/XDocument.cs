using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Xml.Linq
{
	/// <summary>Represents an XML document. For the components and usage of an <see cref="T:System.Xml.Linq.XDocument" /> object, see XDocument Class Overview.</summary>
	public class XDocument : XContainer
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Linq.XDocument" /> class.</summary>
		public XDocument()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Linq.XDocument" /> class with the specified content.</summary>
		/// <param name="content">A parameter list of content objects to add to this document.</param>
		public XDocument(params object[] content) : this()
		{
			base.AddContentSkipNotify(content);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Linq.XDocument" /> class with the specified <see cref="T:System.Xml.Linq.XDeclaration" /> and content.</summary>
		/// <param name="declaration">An <see cref="T:System.Xml.Linq.XDeclaration" /> for the document.</param>
		/// <param name="content">The content of the document.</param>
		public XDocument(XDeclaration declaration, params object[] content) : this(content)
		{
			this._declaration = declaration;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Linq.XDocument" /> class from an existing <see cref="T:System.Xml.Linq.XDocument" /> object.</summary>
		/// <param name="other">The <see cref="T:System.Xml.Linq.XDocument" /> object that will be copied.</param>
		public XDocument(XDocument other) : base(other)
		{
			if (other._declaration != null)
			{
				this._declaration = new XDeclaration(other._declaration);
			}
		}

		/// <summary>Gets or sets the XML declaration for this document.</summary>
		/// <returns>An <see cref="T:System.Xml.Linq.XDeclaration" /> that contains the XML declaration for this document.</returns>
		public XDeclaration Declaration
		{
			get
			{
				return this._declaration;
			}
			set
			{
				this._declaration = value;
			}
		}

		/// <summary>Gets the Document Type Definition (DTD) for this document.</summary>
		/// <returns>A <see cref="T:System.Xml.Linq.XDocumentType" /> that contains the DTD for this document.</returns>
		public XDocumentType DocumentType
		{
			get
			{
				return this.GetFirstNode<XDocumentType>();
			}
		}

		/// <summary>Gets the node type for this node.</summary>
		/// <returns>The node type. For <see cref="T:System.Xml.Linq.XDocument" /> objects, this value is <see cref="F:System.Xml.XmlNodeType.Document" />.</returns>
		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.Document;
			}
		}

		/// <summary>Gets the root element of the XML Tree for this document.</summary>
		/// <returns>The root <see cref="T:System.Xml.Linq.XElement" /> of the XML tree.</returns>
		public XElement Root
		{
			get
			{
				return this.GetFirstNode<XElement>();
			}
		}

		/// <summary>Creates a new <see cref="T:System.Xml.Linq.XDocument" /> from a file.</summary>
		/// <param name="uri">A URI string that references the file to load into a new <see cref="T:System.Xml.Linq.XDocument" />.</param>
		/// <returns>An <see cref="T:System.Xml.Linq.XDocument" /> that contains the contents of the specified file.</returns>
		public static XDocument Load(string uri)
		{
			return XDocument.Load(uri, LoadOptions.None);
		}

		/// <summary>Creates a new <see cref="T:System.Xml.Linq.XDocument" /> from a file, optionally preserving white space, setting the base URI, and retaining line information.</summary>
		/// <param name="uri">A URI string that references the file to load into a new <see cref="T:System.Xml.Linq.XDocument" />.</param>
		/// <param name="options">A <see cref="T:System.Xml.Linq.LoadOptions" /> that specifies white space behavior, and whether to load base URI and line information.</param>
		/// <returns>An <see cref="T:System.Xml.Linq.XDocument" /> that contains the contents of the specified file.</returns>
		public static XDocument Load(string uri, LoadOptions options)
		{
			XmlReaderSettings xmlReaderSettings = XNode.GetXmlReaderSettings(options);
			XDocument result;
			using (XmlReader xmlReader = XmlReader.Create(uri, xmlReaderSettings))
			{
				result = XDocument.Load(xmlReader, options);
			}
			return result;
		}

		/// <summary>Creates a new <see cref="T:System.Xml.Linq.XDocument" /> instance by using the specified stream.</summary>
		/// <param name="stream">The stream that contains the XML data.</param>
		/// <returns>An <see cref="T:System.Xml.Linq.XDocument" /> object that reads the data that is contained in the stream.</returns>
		public static XDocument Load(Stream stream)
		{
			return XDocument.Load(stream, LoadOptions.None);
		}

		/// <summary>Creates a new <see cref="T:System.Xml.Linq.XDocument" /> instance by using the specified stream, optionally preserving white space, setting the base URI, and retaining line information.</summary>
		/// <param name="stream">The stream containing the XML data.</param>
		/// <param name="options">A <see cref="T:System.Xml.Linq.LoadOptions" /> that specifies whether to load base URI and line information.</param>
		/// <returns>An <see cref="T:System.Xml.Linq.XDocument" /> object that reads the data that is contained in the stream.</returns>
		public static XDocument Load(Stream stream, LoadOptions options)
		{
			XmlReaderSettings xmlReaderSettings = XNode.GetXmlReaderSettings(options);
			XDocument result;
			using (XmlReader xmlReader = XmlReader.Create(stream, xmlReaderSettings))
			{
				result = XDocument.Load(xmlReader, options);
			}
			return result;
		}

		public static Task<XDocument> LoadAsync(Stream stream, LoadOptions options, CancellationToken cancellationToken)
		{
			XDocument.<LoadAsync>d__18 <LoadAsync>d__;
			<LoadAsync>d__.stream = stream;
			<LoadAsync>d__.options = options;
			<LoadAsync>d__.cancellationToken = cancellationToken;
			<LoadAsync>d__.<>t__builder = AsyncTaskMethodBuilder<XDocument>.Create();
			<LoadAsync>d__.<>1__state = -1;
			<LoadAsync>d__.<>t__builder.Start<XDocument.<LoadAsync>d__18>(ref <LoadAsync>d__);
			return <LoadAsync>d__.<>t__builder.Task;
		}

		/// <summary>Creates a new <see cref="T:System.Xml.Linq.XDocument" /> from a <see cref="T:System.IO.TextReader" />.</summary>
		/// <param name="textReader">A <see cref="T:System.IO.TextReader" /> that contains the content for the <see cref="T:System.Xml.Linq.XDocument" />.</param>
		/// <returns>An <see cref="T:System.Xml.Linq.XDocument" /> that contains the contents of the specified <see cref="T:System.IO.TextReader" />.</returns>
		public static XDocument Load(TextReader textReader)
		{
			return XDocument.Load(textReader, LoadOptions.None);
		}

		/// <summary>Creates a new <see cref="T:System.Xml.Linq.XDocument" /> from a <see cref="T:System.IO.TextReader" />, optionally preserving white space, setting the base URI, and retaining line information.</summary>
		/// <param name="textReader">A <see cref="T:System.IO.TextReader" /> that contains the content for the <see cref="T:System.Xml.Linq.XDocument" />.</param>
		/// <param name="options">A <see cref="T:System.Xml.Linq.LoadOptions" /> that specifies white space behavior, and whether to load base URI and line information.</param>
		/// <returns>An <see cref="T:System.Xml.Linq.XDocument" /> that contains the XML that was read from the specified <see cref="T:System.IO.TextReader" />.</returns>
		public static XDocument Load(TextReader textReader, LoadOptions options)
		{
			XmlReaderSettings xmlReaderSettings = XNode.GetXmlReaderSettings(options);
			XDocument result;
			using (XmlReader xmlReader = XmlReader.Create(textReader, xmlReaderSettings))
			{
				result = XDocument.Load(xmlReader, options);
			}
			return result;
		}

		public static Task<XDocument> LoadAsync(TextReader textReader, LoadOptions options, CancellationToken cancellationToken)
		{
			XDocument.<LoadAsync>d__21 <LoadAsync>d__;
			<LoadAsync>d__.textReader = textReader;
			<LoadAsync>d__.options = options;
			<LoadAsync>d__.cancellationToken = cancellationToken;
			<LoadAsync>d__.<>t__builder = AsyncTaskMethodBuilder<XDocument>.Create();
			<LoadAsync>d__.<>1__state = -1;
			<LoadAsync>d__.<>t__builder.Start<XDocument.<LoadAsync>d__21>(ref <LoadAsync>d__);
			return <LoadAsync>d__.<>t__builder.Task;
		}

		/// <summary>Creates a new <see cref="T:System.Xml.Linq.XDocument" /> from an <see cref="T:System.Xml.XmlReader" />.</summary>
		/// <param name="reader">A <see cref="T:System.Xml.XmlReader" /> that contains the content for the <see cref="T:System.Xml.Linq.XDocument" />.</param>
		/// <returns>An <see cref="T:System.Xml.Linq.XDocument" /> that contains the contents of the specified <see cref="T:System.Xml.XmlReader" />.</returns>
		public static XDocument Load(XmlReader reader)
		{
			return XDocument.Load(reader, LoadOptions.None);
		}

		/// <summary>Loads an <see cref="T:System.Xml.Linq.XDocument" /> from an <see cref="T:System.Xml.XmlReader" />, optionally setting the base URI, and retaining line information.</summary>
		/// <param name="reader">A <see cref="T:System.Xml.XmlReader" /> that will be read for the content of the <see cref="T:System.Xml.Linq.XDocument" />.</param>
		/// <param name="options">A <see cref="T:System.Xml.Linq.LoadOptions" /> that specifies whether to load base URI and line information.</param>
		/// <returns>An <see cref="T:System.Xml.Linq.XDocument" /> that contains the XML that was read from the specified <see cref="T:System.Xml.XmlReader" />.</returns>
		public static XDocument Load(XmlReader reader, LoadOptions options)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (reader.ReadState == ReadState.Initial)
			{
				reader.Read();
			}
			XDocument xdocument = XDocument.InitLoad(reader, options);
			xdocument.ReadContentFrom(reader, options);
			if (!reader.EOF)
			{
				throw new InvalidOperationException("The XmlReader state should be EndOfFile after this operation.");
			}
			if (xdocument.Root == null)
			{
				throw new InvalidOperationException("The root element is missing.");
			}
			return xdocument;
		}

		public static Task<XDocument> LoadAsync(XmlReader reader, LoadOptions options, CancellationToken cancellationToken)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<XDocument>(cancellationToken);
			}
			return XDocument.LoadAsyncInternal(reader, options, cancellationToken);
		}

		private static Task<XDocument> LoadAsyncInternal(XmlReader reader, LoadOptions options, CancellationToken cancellationToken)
		{
			XDocument.<LoadAsyncInternal>d__25 <LoadAsyncInternal>d__;
			<LoadAsyncInternal>d__.reader = reader;
			<LoadAsyncInternal>d__.options = options;
			<LoadAsyncInternal>d__.cancellationToken = cancellationToken;
			<LoadAsyncInternal>d__.<>t__builder = AsyncTaskMethodBuilder<XDocument>.Create();
			<LoadAsyncInternal>d__.<>1__state = -1;
			<LoadAsyncInternal>d__.<>t__builder.Start<XDocument.<LoadAsyncInternal>d__25>(ref <LoadAsyncInternal>d__);
			return <LoadAsyncInternal>d__.<>t__builder.Task;
		}

		private static XDocument InitLoad(XmlReader reader, LoadOptions options)
		{
			XDocument xdocument = new XDocument();
			if ((options & LoadOptions.SetBaseUri) != LoadOptions.None)
			{
				string baseURI = reader.BaseURI;
				if (!string.IsNullOrEmpty(baseURI))
				{
					xdocument.SetBaseUri(baseURI);
				}
			}
			if ((options & LoadOptions.SetLineInfo) != LoadOptions.None)
			{
				IXmlLineInfo xmlLineInfo = reader as IXmlLineInfo;
				if (xmlLineInfo != null && xmlLineInfo.HasLineInfo())
				{
					xdocument.SetLineInfo(xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
				}
			}
			if (reader.NodeType == XmlNodeType.XmlDeclaration)
			{
				xdocument.Declaration = new XDeclaration(reader);
			}
			return xdocument;
		}

		/// <summary>Creates a new <see cref="T:System.Xml.Linq.XDocument" /> from a string.</summary>
		/// <param name="text">A string that contains XML.</param>
		/// <returns>An <see cref="T:System.Xml.Linq.XDocument" /> populated from the string that contains XML.</returns>
		public static XDocument Parse(string text)
		{
			return XDocument.Parse(text, LoadOptions.None);
		}

		/// <summary>Creates a new <see cref="T:System.Xml.Linq.XDocument" /> from a string, optionally preserving white space, setting the base URI, and retaining line information.</summary>
		/// <param name="text">A string that contains XML.</param>
		/// <param name="options">A <see cref="T:System.Xml.Linq.LoadOptions" /> that specifies white space behavior, and whether to load base URI and line information.</param>
		/// <returns>An <see cref="T:System.Xml.Linq.XDocument" /> populated from the string that contains XML.</returns>
		public static XDocument Parse(string text, LoadOptions options)
		{
			XDocument result;
			using (StringReader stringReader = new StringReader(text))
			{
				XmlReaderSettings xmlReaderSettings = XNode.GetXmlReaderSettings(options);
				using (XmlReader xmlReader = XmlReader.Create(stringReader, xmlReaderSettings))
				{
					result = XDocument.Load(xmlReader, options);
				}
			}
			return result;
		}

		/// <summary>Outputs this <see cref="T:System.Xml.Linq.XDocument" /> to the specified <see cref="T:System.IO.Stream" />.</summary>
		/// <param name="stream">The stream to output this <see cref="T:System.Xml.Linq.XDocument" /> to.</param>
		public void Save(Stream stream)
		{
			this.Save(stream, base.GetSaveOptionsFromAnnotations());
		}

		/// <summary>Outputs this <see cref="T:System.Xml.Linq.XDocument" /> to the specified <see cref="T:System.IO.Stream" />, optionally specifying formatting behavior.</summary>
		/// <param name="stream">The stream to output this <see cref="T:System.Xml.Linq.XDocument" /> to.</param>
		/// <param name="options">A <see cref="T:System.Xml.Linq.SaveOptions" /> that specifies formatting behavior.</param>
		public void Save(Stream stream, SaveOptions options)
		{
			XmlWriterSettings xmlWriterSettings = XNode.GetXmlWriterSettings(options);
			if (this._declaration != null && !string.IsNullOrEmpty(this._declaration.Encoding))
			{
				try
				{
					xmlWriterSettings.Encoding = Encoding.GetEncoding(this._declaration.Encoding);
				}
				catch (ArgumentException)
				{
				}
			}
			using (XmlWriter xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
			{
				this.Save(xmlWriter);
			}
		}

		public Task SaveAsync(Stream stream, SaveOptions options, CancellationToken cancellationToken)
		{
			XDocument.<SaveAsync>d__31 <SaveAsync>d__;
			<SaveAsync>d__.<>4__this = this;
			<SaveAsync>d__.stream = stream;
			<SaveAsync>d__.options = options;
			<SaveAsync>d__.cancellationToken = cancellationToken;
			<SaveAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SaveAsync>d__.<>1__state = -1;
			<SaveAsync>d__.<>t__builder.Start<XDocument.<SaveAsync>d__31>(ref <SaveAsync>d__);
			return <SaveAsync>d__.<>t__builder.Task;
		}

		/// <summary>Serialize this <see cref="T:System.Xml.Linq.XDocument" /> to a <see cref="T:System.IO.TextWriter" />.</summary>
		/// <param name="textWriter">A <see cref="T:System.IO.TextWriter" /> that the <see cref="T:System.Xml.Linq.XDocument" /> will be written to.</param>
		public void Save(TextWriter textWriter)
		{
			this.Save(textWriter, base.GetSaveOptionsFromAnnotations());
		}

		/// <summary>Serialize this <see cref="T:System.Xml.Linq.XDocument" /> to a <see cref="T:System.IO.TextWriter" />, optionally disabling formatting.</summary>
		/// <param name="textWriter">The <see cref="T:System.IO.TextWriter" /> to output the XML to.</param>
		/// <param name="options">A <see cref="T:System.Xml.Linq.SaveOptions" /> that specifies formatting behavior.</param>
		public void Save(TextWriter textWriter, SaveOptions options)
		{
			XmlWriterSettings xmlWriterSettings = XNode.GetXmlWriterSettings(options);
			using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, xmlWriterSettings))
			{
				this.Save(xmlWriter);
			}
		}

		/// <summary>Serialize this <see cref="T:System.Xml.Linq.XDocument" /> to an <see cref="T:System.Xml.XmlWriter" />.</summary>
		/// <param name="writer">A <see cref="T:System.Xml.XmlWriter" /> that the <see cref="T:System.Xml.Linq.XDocument" /> will be written to.</param>
		public void Save(XmlWriter writer)
		{
			this.WriteTo(writer);
		}

		public Task SaveAsync(TextWriter textWriter, SaveOptions options, CancellationToken cancellationToken)
		{
			XDocument.<SaveAsync>d__35 <SaveAsync>d__;
			<SaveAsync>d__.<>4__this = this;
			<SaveAsync>d__.textWriter = textWriter;
			<SaveAsync>d__.options = options;
			<SaveAsync>d__.cancellationToken = cancellationToken;
			<SaveAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SaveAsync>d__.<>1__state = -1;
			<SaveAsync>d__.<>t__builder.Start<XDocument.<SaveAsync>d__35>(ref <SaveAsync>d__);
			return <SaveAsync>d__.<>t__builder.Task;
		}

		/// <summary>Serialize this <see cref="T:System.Xml.Linq.XDocument" /> to a file, overwriting an existing file, if it exists.</summary>
		/// <param name="fileName">A string that contains the name of the file.</param>
		public void Save(string fileName)
		{
			this.Save(fileName, base.GetSaveOptionsFromAnnotations());
		}

		public Task SaveAsync(XmlWriter writer, CancellationToken cancellationToken)
		{
			return this.WriteToAsync(writer, cancellationToken);
		}

		/// <summary>Serialize this <see cref="T:System.Xml.Linq.XDocument" /> to a file, optionally disabling formatting.</summary>
		/// <param name="fileName">A string that contains the name of the file.</param>
		/// <param name="options">A <see cref="T:System.Xml.Linq.SaveOptions" /> that specifies formatting behavior.</param>
		public void Save(string fileName, SaveOptions options)
		{
			XmlWriterSettings xmlWriterSettings = XNode.GetXmlWriterSettings(options);
			if (this._declaration != null && !string.IsNullOrEmpty(this._declaration.Encoding))
			{
				try
				{
					xmlWriterSettings.Encoding = Encoding.GetEncoding(this._declaration.Encoding);
				}
				catch (ArgumentException)
				{
				}
			}
			using (XmlWriter xmlWriter = XmlWriter.Create(fileName, xmlWriterSettings))
			{
				this.Save(xmlWriter);
			}
		}

		/// <summary>Write this document to an <see cref="T:System.Xml.XmlWriter" />.</summary>
		/// <param name="writer">An <see cref="T:System.Xml.XmlWriter" /> into which this method will write.</param>
		public override void WriteTo(XmlWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (this._declaration != null && this._declaration.Standalone == "yes")
			{
				writer.WriteStartDocument(true);
			}
			else if (this._declaration != null && this._declaration.Standalone == "no")
			{
				writer.WriteStartDocument(false);
			}
			else
			{
				writer.WriteStartDocument();
			}
			base.WriteContentTo(writer);
			writer.WriteEndDocument();
		}

		public override Task WriteToAsync(XmlWriter writer, CancellationToken cancellationToken)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled(cancellationToken);
			}
			return this.WriteToAsyncInternal(writer, cancellationToken);
		}

		private Task WriteToAsyncInternal(XmlWriter writer, CancellationToken cancellationToken)
		{
			XDocument.<WriteToAsyncInternal>d__41 <WriteToAsyncInternal>d__;
			<WriteToAsyncInternal>d__.<>4__this = this;
			<WriteToAsyncInternal>d__.writer = writer;
			<WriteToAsyncInternal>d__.cancellationToken = cancellationToken;
			<WriteToAsyncInternal>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteToAsyncInternal>d__.<>1__state = -1;
			<WriteToAsyncInternal>d__.<>t__builder.Start<XDocument.<WriteToAsyncInternal>d__41>(ref <WriteToAsyncInternal>d__);
			return <WriteToAsyncInternal>d__.<>t__builder.Task;
		}

		internal override void AddAttribute(XAttribute a)
		{
			throw new ArgumentException("An attribute cannot be added to content.");
		}

		internal override void AddAttributeSkipNotify(XAttribute a)
		{
			throw new ArgumentException("An attribute cannot be added to content.");
		}

		internal override XNode CloneNode()
		{
			return new XDocument(this);
		}

		internal override bool DeepEquals(XNode node)
		{
			XDocument xdocument = node as XDocument;
			return xdocument != null && base.ContentsEqual(xdocument);
		}

		internal override int GetDeepHashCode()
		{
			return base.ContentsHashCode();
		}

		private T GetFirstNode<T>() where T : XNode
		{
			XNode xnode = this.content as XNode;
			if (xnode != null)
			{
				T t;
				for (;;)
				{
					xnode = xnode.next;
					t = (xnode as T);
					if (t != null)
					{
						break;
					}
					if (xnode == this.content)
					{
						goto IL_35;
					}
				}
				return t;
			}
			IL_35:
			return default(T);
		}

		internal static bool IsWhitespace(string s)
		{
			foreach (char c in s)
			{
				if (c != ' ' && c != '\t' && c != '\r' && c != '\n')
				{
					return false;
				}
			}
			return true;
		}

		internal override void ValidateNode(XNode node, XNode previous)
		{
			XmlNodeType nodeType = node.NodeType;
			switch (nodeType)
			{
			case XmlNodeType.Element:
				this.ValidateDocument(previous, XmlNodeType.DocumentType, XmlNodeType.None);
				return;
			case XmlNodeType.Attribute:
				return;
			case XmlNodeType.Text:
				this.ValidateString(((XText)node).Value);
				return;
			case XmlNodeType.CDATA:
				throw new ArgumentException(SR.Format("A node of type {0} cannot be added to content.", XmlNodeType.CDATA));
			default:
				if (nodeType == XmlNodeType.Document)
				{
					throw new ArgumentException(SR.Format("A node of type {0} cannot be added to content.", XmlNodeType.Document));
				}
				if (nodeType != XmlNodeType.DocumentType)
				{
					return;
				}
				this.ValidateDocument(previous, XmlNodeType.None, XmlNodeType.Element);
				return;
			}
		}

		private void ValidateDocument(XNode previous, XmlNodeType allowBefore, XmlNodeType allowAfter)
		{
			XNode xnode = this.content as XNode;
			if (xnode != null)
			{
				if (previous == null)
				{
					allowBefore = allowAfter;
				}
				for (;;)
				{
					xnode = xnode.next;
					XmlNodeType nodeType = xnode.NodeType;
					if (nodeType == XmlNodeType.Element || nodeType == XmlNodeType.DocumentType)
					{
						if (nodeType != allowBefore)
						{
							break;
						}
						allowBefore = XmlNodeType.None;
					}
					if (xnode == previous)
					{
						allowBefore = allowAfter;
					}
					if (xnode == this.content)
					{
						return;
					}
				}
				throw new InvalidOperationException("This operation would create an incorrectly structured document.");
			}
		}

		internal override void ValidateString(string s)
		{
			if (!XDocument.IsWhitespace(s))
			{
				throw new ArgumentException("Non-whitespace characters cannot be added to content.");
			}
		}

		private XDeclaration _declaration;
	}
}
