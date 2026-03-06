using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Xml.Linq
{
	/// <summary>Represents an XML element.  See XElement Class Overview and the Remarks section on this page for usage information and examples.</summary>
	[XmlSchemaProvider(null, IsAny = true)]
	public class XElement : XContainer, IXmlSerializable
	{
		/// <summary>Gets an empty collection of elements.</summary>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> that contains an empty collection.</returns>
		public static IEnumerable<XElement> EmptySequence
		{
			get
			{
				return Array.Empty<XElement>();
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Linq.XElement" /> class with the specified name.</summary>
		/// <param name="name">An <see cref="T:System.Xml.Linq.XName" /> that contains the name of the element.</param>
		public XElement(XName name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			this.name = name;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Linq.XElement" /> class with the specified name and content.</summary>
		/// <param name="name">An <see cref="T:System.Xml.Linq.XName" /> that contains the element name.</param>
		/// <param name="content">The contents of the element.</param>
		public XElement(XName name, object content) : this(name)
		{
			base.AddContentSkipNotify(content);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Linq.XElement" /> class with the specified name and content.</summary>
		/// <param name="name">An <see cref="T:System.Xml.Linq.XName" /> that contains the element name.</param>
		/// <param name="content">The initial content of the element.</param>
		public XElement(XName name, params object[] content) : this(name, content)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Linq.XElement" /> class from another <see cref="T:System.Xml.Linq.XElement" /> object.</summary>
		/// <param name="other">An <see cref="T:System.Xml.Linq.XElement" /> object to copy from.</param>
		public XElement(XElement other) : base(other)
		{
			this.name = other.name;
			XAttribute next = other.lastAttr;
			if (next != null)
			{
				do
				{
					next = next.next;
					this.AppendAttributeSkipNotify(new XAttribute(next));
				}
				while (next != other.lastAttr);
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Linq.XElement" /> class from an <see cref="T:System.Xml.Linq.XStreamingElement" /> object.</summary>
		/// <param name="other">An <see cref="T:System.Xml.Linq.XStreamingElement" /> that contains unevaluated queries that will be iterated for the contents of this <see cref="T:System.Xml.Linq.XElement" />.</param>
		public XElement(XStreamingElement other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			this.name = other.name;
			base.AddContentSkipNotify(other.content);
		}

		internal XElement() : this("default")
		{
		}

		internal XElement(XmlReader r) : this(r, LoadOptions.None)
		{
		}

		private XElement(XElement.AsyncConstructionSentry s)
		{
		}

		internal XElement(XmlReader r, LoadOptions o)
		{
			this.ReadElementFrom(r, o);
		}

		internal static Task<XElement> CreateAsync(XmlReader r, CancellationToken cancellationToken)
		{
			XElement.<CreateAsync>d__14 <CreateAsync>d__;
			<CreateAsync>d__.r = r;
			<CreateAsync>d__.cancellationToken = cancellationToken;
			<CreateAsync>d__.<>t__builder = AsyncTaskMethodBuilder<XElement>.Create();
			<CreateAsync>d__.<>1__state = -1;
			<CreateAsync>d__.<>t__builder.Start<XElement.<CreateAsync>d__14>(ref <CreateAsync>d__);
			return <CreateAsync>d__.<>t__builder.Task;
		}

		/// <summary>Serialize this element to a file.</summary>
		/// <param name="fileName">A <see cref="T:System.String" /> that contains the name of the file.</param>
		public void Save(string fileName)
		{
			this.Save(fileName, base.GetSaveOptionsFromAnnotations());
		}

		/// <summary>Serialize this element to a file, optionally disabling formatting.</summary>
		/// <param name="fileName">A <see cref="T:System.String" /> that contains the name of the file.</param>
		/// <param name="options">A <see cref="T:System.Xml.Linq.SaveOptions" /> that specifies formatting behavior.</param>
		public void Save(string fileName, SaveOptions options)
		{
			XmlWriterSettings xmlWriterSettings = XNode.GetXmlWriterSettings(options);
			using (XmlWriter xmlWriter = XmlWriter.Create(fileName, xmlWriterSettings))
			{
				this.Save(xmlWriter);
			}
		}

		/// <summary>Gets the first attribute of this element.</summary>
		/// <returns>An <see cref="T:System.Xml.Linq.XAttribute" /> that contains the first attribute of this element.</returns>
		public XAttribute FirstAttribute
		{
			get
			{
				if (this.lastAttr == null)
				{
					return null;
				}
				return this.lastAttr.next;
			}
		}

		/// <summary>Gets a value indicating whether this element has at least one attribute.</summary>
		/// <returns>
		///   <see langword="true" /> if this element has at least one attribute; otherwise <see langword="false" />.</returns>
		public bool HasAttributes
		{
			get
			{
				return this.lastAttr != null;
			}
		}

		/// <summary>Gets a value indicating whether this element has at least one child element.</summary>
		/// <returns>
		///   <see langword="true" /> if this element has at least one child element; otherwise <see langword="false" />.</returns>
		public bool HasElements
		{
			get
			{
				XNode xnode = this.content as XNode;
				if (xnode != null)
				{
					while (!(xnode is XElement))
					{
						xnode = xnode.next;
						if (xnode == this.content)
						{
							return false;
						}
					}
					return true;
				}
				return false;
			}
		}

		/// <summary>Gets a value indicating whether this element contains no content.</summary>
		/// <returns>
		///   <see langword="true" /> if this element contains no content; otherwise <see langword="false" />.</returns>
		public bool IsEmpty
		{
			get
			{
				return this.content == null;
			}
		}

		/// <summary>Gets the last attribute of this element.</summary>
		/// <returns>An <see cref="T:System.Xml.Linq.XAttribute" /> that contains the last attribute of this element.</returns>
		public XAttribute LastAttribute
		{
			get
			{
				return this.lastAttr;
			}
		}

		/// <summary>Gets or sets the name of this element.</summary>
		/// <returns>An <see cref="T:System.Xml.Linq.XName" /> that contains the name of this element.</returns>
		public XName Name
		{
			get
			{
				return this.name;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				bool flag = base.NotifyChanging(this, XObjectChangeEventArgs.Name);
				this.name = value;
				if (flag)
				{
					base.NotifyChanged(this, XObjectChangeEventArgs.Name);
				}
			}
		}

		/// <summary>Gets the node type for this node.</summary>
		/// <returns>The node type. For <see cref="T:System.Xml.Linq.XElement" /> objects, this value is <see cref="F:System.Xml.XmlNodeType.Element" />.</returns>
		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.Element;
			}
		}

		/// <summary>Gets or sets the concatenated text contents of this element.</summary>
		/// <returns>A <see cref="T:System.String" /> that contains all of the text content of this element. If there are multiple text nodes, they will be concatenated.</returns>
		public string Value
		{
			get
			{
				if (this.content == null)
				{
					return string.Empty;
				}
				string text = this.content as string;
				if (text != null)
				{
					return text;
				}
				StringBuilder sb = StringBuilderCache.Acquire(16);
				this.AppendText(sb);
				return StringBuilderCache.GetStringAndRelease(sb);
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				base.RemoveNodes();
				base.Add(value);
			}
		}

		/// <summary>Returns a collection of elements that contain this element, and the ancestors of this element.</summary>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> of elements that contain this element, and the ancestors of this element.</returns>
		public IEnumerable<XElement> AncestorsAndSelf()
		{
			return base.GetAncestors(null, true);
		}

		/// <summary>Returns a filtered collection of elements that contain this element, and the ancestors of this element. Only elements that have a matching <see cref="T:System.Xml.Linq.XName" /> are included in the collection.</summary>
		/// <param name="name">The <see cref="T:System.Xml.Linq.XName" /> to match.</param>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> that contain this element, and the ancestors of this element. Only elements that have a matching <see cref="T:System.Xml.Linq.XName" /> are included in the collection.</returns>
		public IEnumerable<XElement> AncestorsAndSelf(XName name)
		{
			if (!(name != null))
			{
				return XElement.EmptySequence;
			}
			return base.GetAncestors(name, true);
		}

		/// <summary>Returns the <see cref="T:System.Xml.Linq.XAttribute" /> of this <see cref="T:System.Xml.Linq.XElement" /> that has the specified <see cref="T:System.Xml.Linq.XName" />.</summary>
		/// <param name="name">The <see cref="T:System.Xml.Linq.XName" /> of the <see cref="T:System.Xml.Linq.XAttribute" /> to get.</param>
		/// <returns>An <see cref="T:System.Xml.Linq.XAttribute" /> that has the specified <see cref="T:System.Xml.Linq.XName" />; <see langword="null" /> if there is no attribute with the specified name.</returns>
		public XAttribute Attribute(XName name)
		{
			XAttribute next = this.lastAttr;
			if (next != null)
			{
				for (;;)
				{
					next = next.next;
					if (next.name == name)
					{
						break;
					}
					if (next == this.lastAttr)
					{
						goto IL_2A;
					}
				}
				return next;
			}
			IL_2A:
			return null;
		}

		/// <summary>Returns a collection of attributes of this element.</summary>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XAttribute" /> of attributes of this element.</returns>
		public IEnumerable<XAttribute> Attributes()
		{
			return this.GetAttributes(null);
		}

		/// <summary>Returns a filtered collection of attributes of this element. Only elements that have a matching <see cref="T:System.Xml.Linq.XName" /> are included in the collection.</summary>
		/// <param name="name">The <see cref="T:System.Xml.Linq.XName" /> to match.</param>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XAttribute" /> that contains the attributes of this element. Only elements that have a matching <see cref="T:System.Xml.Linq.XName" /> are included in the collection.</returns>
		public IEnumerable<XAttribute> Attributes(XName name)
		{
			if (!(name != null))
			{
				return XAttribute.EmptySequence;
			}
			return this.GetAttributes(name);
		}

		/// <summary>Returns a collection of nodes that contain this element, and all descendant nodes of this element, in document order.</summary>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XNode" /> that contain this element, and all descendant nodes of this element, in document order.</returns>
		public IEnumerable<XNode> DescendantNodesAndSelf()
		{
			return base.GetDescendantNodes(true);
		}

		/// <summary>Returns a collection of elements that contain this element, and all descendant elements of this element, in document order.</summary>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> of elements that contain this element, and all descendant elements of this element, in document order.</returns>
		public IEnumerable<XElement> DescendantsAndSelf()
		{
			return base.GetDescendants(null, true);
		}

		/// <summary>Returns a filtered collection of elements that contain this element, and all descendant elements of this element, in document order. Only elements that have a matching <see cref="T:System.Xml.Linq.XName" /> are included in the collection.</summary>
		/// <param name="name">The <see cref="T:System.Xml.Linq.XName" /> to match.</param>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> that contain this element, and all descendant elements of this element, in document order. Only elements that have a matching <see cref="T:System.Xml.Linq.XName" /> are included in the collection.</returns>
		public IEnumerable<XElement> DescendantsAndSelf(XName name)
		{
			if (!(name != null))
			{
				return XElement.EmptySequence;
			}
			return base.GetDescendants(name, true);
		}

		/// <summary>Gets the default <see cref="T:System.Xml.Linq.XNamespace" /> of this <see cref="T:System.Xml.Linq.XElement" />.</summary>
		/// <returns>An <see cref="T:System.Xml.Linq.XNamespace" /> that contains the default namespace of this <see cref="T:System.Xml.Linq.XElement" />.</returns>
		public XNamespace GetDefaultNamespace()
		{
			string namespaceOfPrefixInScope = this.GetNamespaceOfPrefixInScope("xmlns", null);
			if (namespaceOfPrefixInScope == null)
			{
				return XNamespace.None;
			}
			return XNamespace.Get(namespaceOfPrefixInScope);
		}

		/// <summary>Gets the namespace associated with a particular prefix for this <see cref="T:System.Xml.Linq.XElement" />.</summary>
		/// <param name="prefix">A string that contains the namespace prefix to look up.</param>
		/// <returns>An <see cref="T:System.Xml.Linq.XNamespace" /> for the namespace associated with the prefix for this <see cref="T:System.Xml.Linq.XElement" />.</returns>
		public XNamespace GetNamespaceOfPrefix(string prefix)
		{
			if (prefix == null)
			{
				throw new ArgumentNullException("prefix");
			}
			if (prefix.Length == 0)
			{
				throw new ArgumentException(SR.Format("'{0}' is an invalid prefix.", prefix));
			}
			if (prefix == "xmlns")
			{
				return XNamespace.Xmlns;
			}
			string namespaceOfPrefixInScope = this.GetNamespaceOfPrefixInScope(prefix, null);
			if (namespaceOfPrefixInScope != null)
			{
				return XNamespace.Get(namespaceOfPrefixInScope);
			}
			if (prefix == "xml")
			{
				return XNamespace.Xml;
			}
			return null;
		}

		/// <summary>Gets the prefix associated with a namespace for this <see cref="T:System.Xml.Linq.XElement" />.</summary>
		/// <param name="ns">An <see cref="T:System.Xml.Linq.XNamespace" /> to look up.</param>
		/// <returns>A <see cref="T:System.String" /> that contains the namespace prefix.</returns>
		public string GetPrefixOfNamespace(XNamespace ns)
		{
			if (ns == null)
			{
				throw new ArgumentNullException("ns");
			}
			string namespaceName = ns.NamespaceName;
			bool flag = false;
			XElement xelement = this;
			XAttribute next;
			for (;;)
			{
				next = xelement.lastAttr;
				if (next != null)
				{
					bool flag2 = false;
					do
					{
						next = next.next;
						if (next.IsNamespaceDeclaration)
						{
							if (next.Value == namespaceName && next.Name.NamespaceName.Length != 0 && (!flag || this.GetNamespaceOfPrefixInScope(next.Name.LocalName, xelement) == null))
							{
								goto IL_72;
							}
							flag2 = true;
						}
					}
					while (next != xelement.lastAttr);
					flag = (flag || flag2);
				}
				xelement = (xelement.parent as XElement);
				if (xelement == null)
				{
					goto Block_8;
				}
			}
			IL_72:
			return next.Name.LocalName;
			Block_8:
			if (namespaceName == "http://www.w3.org/XML/1998/namespace")
			{
				if (!flag || this.GetNamespaceOfPrefixInScope("xml", null) == null)
				{
					return "xml";
				}
			}
			else if (namespaceName == "http://www.w3.org/2000/xmlns/")
			{
				return "xmlns";
			}
			return null;
		}

		/// <summary>Loads an <see cref="T:System.Xml.Linq.XElement" /> from a file.</summary>
		/// <param name="uri">A URI string referencing the file to load into a new <see cref="T:System.Xml.Linq.XElement" />.</param>
		/// <returns>An <see cref="T:System.Xml.Linq.XElement" /> that contains the contents of the specified file.</returns>
		public static XElement Load(string uri)
		{
			return XElement.Load(uri, LoadOptions.None);
		}

		/// <summary>Loads an <see cref="T:System.Xml.Linq.XElement" /> from a file, optionally preserving white space, setting the base URI, and retaining line information.</summary>
		/// <param name="uri">A URI string referencing the file to load into an <see cref="T:System.Xml.Linq.XElement" />.</param>
		/// <param name="options">A <see cref="T:System.Xml.Linq.LoadOptions" /> that specifies white space behavior, and whether to load base URI and line information.</param>
		/// <returns>An <see cref="T:System.Xml.Linq.XElement" /> that contains the contents of the specified file.</returns>
		public static XElement Load(string uri, LoadOptions options)
		{
			XmlReaderSettings xmlReaderSettings = XNode.GetXmlReaderSettings(options);
			XElement result;
			using (XmlReader xmlReader = XmlReader.Create(uri, xmlReaderSettings))
			{
				result = XElement.Load(xmlReader, options);
			}
			return result;
		}

		/// <summary>Creates a new <see cref="T:System.Xml.Linq.XElement" /> instance by using the specified stream.</summary>
		/// <param name="stream">The stream that contains the XML data.</param>
		/// <returns>An <see cref="T:System.Xml.Linq.XElement" /> object used to read the data that is contained in the stream.</returns>
		public static XElement Load(Stream stream)
		{
			return XElement.Load(stream, LoadOptions.None);
		}

		/// <summary>Creates a new <see cref="T:System.Xml.Linq.XElement" /> instance by using the specified stream, optionally preserving white space, setting the base URI, and retaining line information.</summary>
		/// <param name="stream">The stream containing the XML data.</param>
		/// <param name="options">A <see cref="T:System.Xml.Linq.LoadOptions" /> object that specifies whether to load base URI and line information.</param>
		/// <returns>An <see cref="T:System.Xml.Linq.XElement" /> object used to read the data that the stream contains.</returns>
		public static XElement Load(Stream stream, LoadOptions options)
		{
			XmlReaderSettings xmlReaderSettings = XNode.GetXmlReaderSettings(options);
			XElement result;
			using (XmlReader xmlReader = XmlReader.Create(stream, xmlReaderSettings))
			{
				result = XElement.Load(xmlReader, options);
			}
			return result;
		}

		public static Task<XElement> LoadAsync(Stream stream, LoadOptions options, CancellationToken cancellationToken)
		{
			XElement.<LoadAsync>d__50 <LoadAsync>d__;
			<LoadAsync>d__.stream = stream;
			<LoadAsync>d__.options = options;
			<LoadAsync>d__.cancellationToken = cancellationToken;
			<LoadAsync>d__.<>t__builder = AsyncTaskMethodBuilder<XElement>.Create();
			<LoadAsync>d__.<>1__state = -1;
			<LoadAsync>d__.<>t__builder.Start<XElement.<LoadAsync>d__50>(ref <LoadAsync>d__);
			return <LoadAsync>d__.<>t__builder.Task;
		}

		/// <summary>Loads an <see cref="T:System.Xml.Linq.XElement" /> from a <see cref="T:System.IO.TextReader" />.</summary>
		/// <param name="textReader">A <see cref="T:System.IO.TextReader" /> that will be read for the <see cref="T:System.Xml.Linq.XElement" /> content.</param>
		/// <returns>An <see cref="T:System.Xml.Linq.XElement" /> that contains the XML that was read from the specified <see cref="T:System.IO.TextReader" />.</returns>
		public static XElement Load(TextReader textReader)
		{
			return XElement.Load(textReader, LoadOptions.None);
		}

		/// <summary>Loads an <see cref="T:System.Xml.Linq.XElement" /> from a <see cref="T:System.IO.TextReader" />, optionally preserving white space and retaining line information.</summary>
		/// <param name="textReader">A <see cref="T:System.IO.TextReader" /> that will be read for the <see cref="T:System.Xml.Linq.XElement" /> content.</param>
		/// <param name="options">A <see cref="T:System.Xml.Linq.LoadOptions" /> that specifies white space behavior, and whether to load base URI and line information.</param>
		/// <returns>An <see cref="T:System.Xml.Linq.XElement" /> that contains the XML that was read from the specified <see cref="T:System.IO.TextReader" />.</returns>
		public static XElement Load(TextReader textReader, LoadOptions options)
		{
			XmlReaderSettings xmlReaderSettings = XNode.GetXmlReaderSettings(options);
			XElement result;
			using (XmlReader xmlReader = XmlReader.Create(textReader, xmlReaderSettings))
			{
				result = XElement.Load(xmlReader, options);
			}
			return result;
		}

		public static Task<XElement> LoadAsync(TextReader textReader, LoadOptions options, CancellationToken cancellationToken)
		{
			XElement.<LoadAsync>d__53 <LoadAsync>d__;
			<LoadAsync>d__.textReader = textReader;
			<LoadAsync>d__.options = options;
			<LoadAsync>d__.cancellationToken = cancellationToken;
			<LoadAsync>d__.<>t__builder = AsyncTaskMethodBuilder<XElement>.Create();
			<LoadAsync>d__.<>1__state = -1;
			<LoadAsync>d__.<>t__builder.Start<XElement.<LoadAsync>d__53>(ref <LoadAsync>d__);
			return <LoadAsync>d__.<>t__builder.Task;
		}

		/// <summary>Loads an <see cref="T:System.Xml.Linq.XElement" /> from an <see cref="T:System.Xml.XmlReader" />.</summary>
		/// <param name="reader">A <see cref="T:System.Xml.XmlReader" /> that will be read for the content of the <see cref="T:System.Xml.Linq.XElement" />.</param>
		/// <returns>An <see cref="T:System.Xml.Linq.XElement" /> that contains the XML that was read from the specified <see cref="T:System.Xml.XmlReader" />.</returns>
		public static XElement Load(XmlReader reader)
		{
			return XElement.Load(reader, LoadOptions.None);
		}

		/// <summary>Loads an <see cref="T:System.Xml.Linq.XElement" /> from an <see cref="T:System.Xml.XmlReader" />, optionally preserving white space, setting the base URI, and retaining line information.</summary>
		/// <param name="reader">A <see cref="T:System.Xml.XmlReader" /> that will be read for the content of the <see cref="T:System.Xml.Linq.XElement" />.</param>
		/// <param name="options">A <see cref="T:System.Xml.Linq.LoadOptions" /> that specifies white space behavior, and whether to load base URI and line information.</param>
		/// <returns>An <see cref="T:System.Xml.Linq.XElement" /> that contains the XML that was read from the specified <see cref="T:System.Xml.XmlReader" />.</returns>
		public static XElement Load(XmlReader reader, LoadOptions options)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (reader.MoveToContent() != XmlNodeType.Element)
			{
				throw new InvalidOperationException(SR.Format("The XmlReader must be on a node of type {0} instead of a node of type {1}.", XmlNodeType.Element, reader.NodeType));
			}
			XElement result = new XElement(reader, options);
			reader.MoveToContent();
			if (!reader.EOF)
			{
				throw new InvalidOperationException("The XmlReader state should be EndOfFile after this operation.");
			}
			return result;
		}

		public static Task<XElement> LoadAsync(XmlReader reader, LoadOptions options, CancellationToken cancellationToken)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<XElement>(cancellationToken);
			}
			return XElement.LoadAsyncInternal(reader, options, cancellationToken);
		}

		private static Task<XElement> LoadAsyncInternal(XmlReader reader, LoadOptions options, CancellationToken cancellationToken)
		{
			XElement.<LoadAsyncInternal>d__57 <LoadAsyncInternal>d__;
			<LoadAsyncInternal>d__.reader = reader;
			<LoadAsyncInternal>d__.options = options;
			<LoadAsyncInternal>d__.cancellationToken = cancellationToken;
			<LoadAsyncInternal>d__.<>t__builder = AsyncTaskMethodBuilder<XElement>.Create();
			<LoadAsyncInternal>d__.<>1__state = -1;
			<LoadAsyncInternal>d__.<>t__builder.Start<XElement.<LoadAsyncInternal>d__57>(ref <LoadAsyncInternal>d__);
			return <LoadAsyncInternal>d__.<>t__builder.Task;
		}

		/// <summary>Load an <see cref="T:System.Xml.Linq.XElement" /> from a string that contains XML.</summary>
		/// <param name="text">A <see cref="T:System.String" /> that contains XML.</param>
		/// <returns>An <see cref="T:System.Xml.Linq.XElement" /> populated from the string that contains XML.</returns>
		public static XElement Parse(string text)
		{
			return XElement.Parse(text, LoadOptions.None);
		}

		/// <summary>Load an <see cref="T:System.Xml.Linq.XElement" /> from a string that contains XML, optionally preserving white space and retaining line information.</summary>
		/// <param name="text">A <see cref="T:System.String" /> that contains XML.</param>
		/// <param name="options">A <see cref="T:System.Xml.Linq.LoadOptions" /> that specifies white space behavior, and whether to load base URI and line information.</param>
		/// <returns>An <see cref="T:System.Xml.Linq.XElement" /> populated from the string that contains XML.</returns>
		public static XElement Parse(string text, LoadOptions options)
		{
			XElement result;
			using (StringReader stringReader = new StringReader(text))
			{
				XmlReaderSettings xmlReaderSettings = XNode.GetXmlReaderSettings(options);
				using (XmlReader xmlReader = XmlReader.Create(stringReader, xmlReaderSettings))
				{
					result = XElement.Load(xmlReader, options);
				}
			}
			return result;
		}

		/// <summary>Removes nodes and attributes from this <see cref="T:System.Xml.Linq.XElement" />.</summary>
		public void RemoveAll()
		{
			this.RemoveAttributes();
			base.RemoveNodes();
		}

		/// <summary>Removes the attributes of this <see cref="T:System.Xml.Linq.XElement" />.</summary>
		public void RemoveAttributes()
		{
			if (base.SkipNotify())
			{
				this.RemoveAttributesSkipNotify();
				return;
			}
			while (this.lastAttr != null)
			{
				XAttribute next = this.lastAttr.next;
				base.NotifyChanging(next, XObjectChangeEventArgs.Remove);
				if (this.lastAttr == null || next != this.lastAttr.next)
				{
					throw new InvalidOperationException("This operation was corrupted by external code.");
				}
				if (next != this.lastAttr)
				{
					this.lastAttr.next = next.next;
				}
				else
				{
					this.lastAttr = null;
				}
				next.parent = null;
				next.next = null;
				base.NotifyChanged(next, XObjectChangeEventArgs.Remove);
			}
		}

		/// <summary>Replaces the child nodes and the attributes of this element with the specified content.</summary>
		/// <param name="content">The content that will replace the child nodes and attributes of this element.</param>
		public void ReplaceAll(object content)
		{
			content = XContainer.GetContentSnapshot(content);
			this.RemoveAll();
			base.Add(content);
		}

		/// <summary>Replaces the child nodes and the attributes of this element with the specified content.</summary>
		/// <param name="content">A parameter list of content objects.</param>
		public void ReplaceAll(params object[] content)
		{
			this.ReplaceAll(content);
		}

		/// <summary>Replaces the attributes of this element with the specified content.</summary>
		/// <param name="content">The content that will replace the attributes of this element.</param>
		public void ReplaceAttributes(object content)
		{
			content = XContainer.GetContentSnapshot(content);
			this.RemoveAttributes();
			base.Add(content);
		}

		/// <summary>Replaces the attributes of this element with the specified content.</summary>
		/// <param name="content">A parameter list of content objects.</param>
		public void ReplaceAttributes(params object[] content)
		{
			this.ReplaceAttributes(content);
		}

		/// <summary>Outputs this <see cref="T:System.Xml.Linq.XElement" /> to the specified <see cref="T:System.IO.Stream" />.</summary>
		/// <param name="stream">The stream to output this <see cref="T:System.Xml.Linq.XElement" /> to.</param>
		public void Save(Stream stream)
		{
			this.Save(stream, base.GetSaveOptionsFromAnnotations());
		}

		/// <summary>Outputs this <see cref="T:System.Xml.Linq.XElement" /> to the specified <see cref="T:System.IO.Stream" />, optionally specifying formatting behavior.</summary>
		/// <param name="stream">The stream to output this <see cref="T:System.Xml.Linq.XElement" /> to.</param>
		/// <param name="options">A <see cref="T:System.Xml.Linq.SaveOptions" /> object that specifies formatting behavior.</param>
		public void Save(Stream stream, SaveOptions options)
		{
			XmlWriterSettings xmlWriterSettings = XNode.GetXmlWriterSettings(options);
			using (XmlWriter xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
			{
				this.Save(xmlWriter);
			}
		}

		public Task SaveAsync(Stream stream, SaveOptions options, CancellationToken cancellationToken)
		{
			XElement.<SaveAsync>d__68 <SaveAsync>d__;
			<SaveAsync>d__.<>4__this = this;
			<SaveAsync>d__.stream = stream;
			<SaveAsync>d__.options = options;
			<SaveAsync>d__.cancellationToken = cancellationToken;
			<SaveAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SaveAsync>d__.<>1__state = -1;
			<SaveAsync>d__.<>t__builder.Start<XElement.<SaveAsync>d__68>(ref <SaveAsync>d__);
			return <SaveAsync>d__.<>t__builder.Task;
		}

		/// <summary>Serialize this element to a <see cref="T:System.IO.TextWriter" />.</summary>
		/// <param name="textWriter">A <see cref="T:System.IO.TextWriter" /> that the <see cref="T:System.Xml.Linq.XElement" /> will be written to.</param>
		public void Save(TextWriter textWriter)
		{
			this.Save(textWriter, base.GetSaveOptionsFromAnnotations());
		}

		/// <summary>Serialize this element to a <see cref="T:System.IO.TextWriter" />, optionally disabling formatting.</summary>
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

		public Task SaveAsync(TextWriter textWriter, SaveOptions options, CancellationToken cancellationToken)
		{
			XElement.<SaveAsync>d__71 <SaveAsync>d__;
			<SaveAsync>d__.<>4__this = this;
			<SaveAsync>d__.textWriter = textWriter;
			<SaveAsync>d__.options = options;
			<SaveAsync>d__.cancellationToken = cancellationToken;
			<SaveAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SaveAsync>d__.<>1__state = -1;
			<SaveAsync>d__.<>t__builder.Start<XElement.<SaveAsync>d__71>(ref <SaveAsync>d__);
			return <SaveAsync>d__.<>t__builder.Task;
		}

		/// <summary>Serialize this element to an <see cref="T:System.Xml.XmlWriter" />.</summary>
		/// <param name="writer">A <see cref="T:System.Xml.XmlWriter" /> that the <see cref="T:System.Xml.Linq.XElement" /> will be written to.</param>
		public void Save(XmlWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.WriteStartDocument();
			this.WriteTo(writer);
			writer.WriteEndDocument();
		}

		public Task SaveAsync(XmlWriter writer, CancellationToken cancellationToken)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled(cancellationToken);
			}
			return this.SaveAsyncInternal(writer, cancellationToken);
		}

		private Task SaveAsyncInternal(XmlWriter writer, CancellationToken cancellationToken)
		{
			XElement.<SaveAsyncInternal>d__74 <SaveAsyncInternal>d__;
			<SaveAsyncInternal>d__.<>4__this = this;
			<SaveAsyncInternal>d__.writer = writer;
			<SaveAsyncInternal>d__.cancellationToken = cancellationToken;
			<SaveAsyncInternal>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SaveAsyncInternal>d__.<>1__state = -1;
			<SaveAsyncInternal>d__.<>t__builder.Start<XElement.<SaveAsyncInternal>d__74>(ref <SaveAsyncInternal>d__);
			return <SaveAsyncInternal>d__.<>t__builder.Task;
		}

		/// <summary>Sets the value of an attribute, adds an attribute, or removes an attribute.</summary>
		/// <param name="name">An <see cref="T:System.Xml.Linq.XName" /> that contains the name of the attribute to change.</param>
		/// <param name="value">The value to assign to the attribute. The attribute is removed if the value is <see langword="null" />. Otherwise, the value is converted to its string representation and assigned to the <see cref="P:System.Xml.Linq.XAttribute.Value" /> property of the attribute.</param>
		/// <exception cref="T:System.ArgumentException">The <paramref name="value" /> is an instance of <see cref="T:System.Xml.Linq.XObject" />.</exception>
		public void SetAttributeValue(XName name, object value)
		{
			XAttribute xattribute = this.Attribute(name);
			if (value == null)
			{
				if (xattribute != null)
				{
					this.RemoveAttribute(xattribute);
					return;
				}
			}
			else
			{
				if (xattribute != null)
				{
					xattribute.Value = XContainer.GetStringValue(value);
					return;
				}
				this.AppendAttribute(new XAttribute(name, value));
			}
		}

		/// <summary>Sets the value of a child element, adds a child element, or removes a child element.</summary>
		/// <param name="name">An <see cref="T:System.Xml.Linq.XName" /> that contains the name of the child element to change.</param>
		/// <param name="value">The value to assign to the child element. The child element is removed if the value is <see langword="null" />. Otherwise, the value is converted to its string representation and assigned to the <see cref="P:System.Xml.Linq.XElement.Value" /> property of the child element.</param>
		/// <exception cref="T:System.ArgumentException">The <paramref name="value" /> is an instance of <see cref="T:System.Xml.Linq.XObject" />.</exception>
		public void SetElementValue(XName name, object value)
		{
			XElement xelement = base.Element(name);
			if (value == null)
			{
				if (xelement != null)
				{
					base.RemoveNode(xelement);
					return;
				}
			}
			else
			{
				if (xelement != null)
				{
					xelement.Value = XContainer.GetStringValue(value);
					return;
				}
				base.AddNode(new XElement(name, XContainer.GetStringValue(value)));
			}
		}

		/// <summary>Sets the value of this element.</summary>
		/// <param name="value">The value to assign to this element. The value is converted to its string representation and assigned to the <see cref="P:System.Xml.Linq.XElement.Value" /> property.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="value" /> is an <see cref="T:System.Xml.Linq.XObject" />.</exception>
		public void SetValue(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.Value = XContainer.GetStringValue(value);
		}

		/// <summary>Write this element to an <see cref="T:System.Xml.XmlWriter" />.</summary>
		/// <param name="writer">An <see cref="T:System.Xml.XmlWriter" /> into which this method will write.</param>
		public override void WriteTo(XmlWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			new ElementWriter(writer).WriteElement(this);
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
			return new ElementWriter(writer).WriteElementAsync(this, cancellationToken);
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XElement" /> to a <see cref="T:System.String" />.</summary>
		/// <param name="element">The <see cref="T:System.Xml.Linq.XElement" /> to cast to <see cref="T:System.String" />.</param>
		/// <returns>A <see cref="T:System.String" /> that contains the content of this <see cref="T:System.Xml.Linq.XElement" />.</returns>
		[CLSCompliant(false)]
		public static explicit operator string(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return element.Value;
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XElement" /> to a <see cref="T:System.Boolean" />.</summary>
		/// <param name="element">The <see cref="T:System.Xml.Linq.XElement" /> to cast to <see cref="T:System.Boolean" />.</param>
		/// <returns>A <see cref="T:System.Boolean" /> that contains the content of this <see cref="T:System.Xml.Linq.XElement" />.</returns>
		/// <exception cref="T:System.FormatException">The element does not contain a valid <see cref="T:System.Boolean" /> value.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="element" /> parameter is <see langword="null" />.</exception>
		[CLSCompliant(false)]
		public static explicit operator bool(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XmlConvert.ToBoolean(element.Value.ToLowerInvariant());
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XElement" /> to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Boolean" />.</summary>
		/// <param name="element">The <see cref="T:System.Xml.Linq.XElement" /> to cast to <see cref="T:System.Nullable`1" /> of <see cref="T:System.Boolean" />.</param>
		/// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.Boolean" /> that contains the content of this <see cref="T:System.Xml.Linq.XElement" />.</returns>
		/// <exception cref="T:System.FormatException">The element is not <see langword="null" /> and does not contain a valid <see cref="T:System.Boolean" /> value.</exception>
		[CLSCompliant(false)]
		public static explicit operator bool?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return new bool?(XmlConvert.ToBoolean(element.Value.ToLowerInvariant()));
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XElement" /> to an <see cref="T:System.Int32" />.</summary>
		/// <param name="element">The <see cref="T:System.Xml.Linq.XElement" /> to cast to <see cref="T:System.Int32" />.</param>
		/// <returns>A <see cref="T:System.Int32" /> that contains the content of this <see cref="T:System.Xml.Linq.XElement" />.</returns>
		/// <exception cref="T:System.FormatException">The element does not contain a valid <see cref="T:System.Int32" /> value.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="element" /> parameter is <see langword="null" />.</exception>
		[CLSCompliant(false)]
		public static explicit operator int(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XmlConvert.ToInt32(element.Value);
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XElement" /> to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int32" />.</summary>
		/// <param name="element">The <see cref="T:System.Xml.Linq.XElement" /> to cast to <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int32" />.</param>
		/// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int32" /> that contains the content of this <see cref="T:System.Xml.Linq.XElement" />.</returns>
		/// <exception cref="T:System.FormatException">The element is not <see langword="null" /> and does not contain a valid <see cref="T:System.Int32" /> value.</exception>
		[CLSCompliant(false)]
		public static explicit operator int?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return new int?(XmlConvert.ToInt32(element.Value));
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XElement" /> to a <see cref="T:System.UInt32" />.</summary>
		/// <param name="element">The <see cref="T:System.Xml.Linq.XElement" /> to cast to <see cref="T:System.UInt32" />.</param>
		/// <returns>A <see cref="T:System.UInt32" /> that contains the content of this <see cref="T:System.Xml.Linq.XElement" />.</returns>
		/// <exception cref="T:System.FormatException">The element does not contain a valid <see cref="T:System.UInt32" /> value.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="element" /> parameter is <see langword="null" />.</exception>
		[CLSCompliant(false)]
		public static explicit operator uint(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XmlConvert.ToUInt32(element.Value);
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XElement" /> to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.UInt32" />.</summary>
		/// <param name="element">The <see cref="T:System.Xml.Linq.XElement" /> to cast to <see cref="T:System.Nullable`1" /> of <see cref="T:System.UInt32" />.</param>
		/// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.UInt32" /> that contains the content of this <see cref="T:System.Xml.Linq.XElement" />.</returns>
		/// <exception cref="T:System.FormatException">The element is not <see langword="null" /> and does not contain a valid <see cref="T:System.UInt32" /> value.</exception>
		[CLSCompliant(false)]
		public static explicit operator uint?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return new uint?(XmlConvert.ToUInt32(element.Value));
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XElement" /> to an <see cref="T:System.Int64" />.</summary>
		/// <param name="element">The <see cref="T:System.Xml.Linq.XElement" /> to cast to <see cref="T:System.Int64" />.</param>
		/// <returns>A <see cref="T:System.Int64" /> that contains the content of this <see cref="T:System.Xml.Linq.XElement" />.</returns>
		/// <exception cref="T:System.FormatException">The element does not contain a valid <see cref="T:System.Int64" /> value.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="element" /> parameter is <see langword="null" />.</exception>
		[CLSCompliant(false)]
		public static explicit operator long(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XmlConvert.ToInt64(element.Value);
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XElement" /> to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int64" />.</summary>
		/// <param name="element">The <see cref="T:System.Xml.Linq.XElement" /> to cast to <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int64" />.</param>
		/// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.Int64" /> that contains the content of this <see cref="T:System.Xml.Linq.XElement" />.</returns>
		/// <exception cref="T:System.FormatException">The element is not <see langword="null" /> and does not contain a valid <see cref="T:System.Int64" /> value.</exception>
		[CLSCompliant(false)]
		public static explicit operator long?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return new long?(XmlConvert.ToInt64(element.Value));
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XElement" /> to a <see cref="T:System.UInt64" />.</summary>
		/// <param name="element">The <see cref="T:System.Xml.Linq.XElement" /> to cast to <see cref="T:System.UInt64" />.</param>
		/// <returns>A <see cref="T:System.UInt64" /> that contains the content of this <see cref="T:System.Xml.Linq.XElement" />.</returns>
		/// <exception cref="T:System.FormatException">The element does not contain a valid <see cref="T:System.UInt64" /> value.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="element" /> parameter is <see langword="null" />.</exception>
		[CLSCompliant(false)]
		public static explicit operator ulong(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XmlConvert.ToUInt64(element.Value);
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XElement" /> to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.UInt64" />.</summary>
		/// <param name="element">The <see cref="T:System.Xml.Linq.XElement" /> to cast to <see cref="T:System.Nullable`1" /> of <see cref="T:System.UInt64" />.</param>
		/// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.UInt64" /> that contains the content of this <see cref="T:System.Xml.Linq.XElement" />.</returns>
		/// <exception cref="T:System.FormatException">The element is not <see langword="null" /> and does not contain a valid <see cref="T:System.UInt64" /> value.</exception>
		[CLSCompliant(false)]
		public static explicit operator ulong?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return new ulong?(XmlConvert.ToUInt64(element.Value));
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XElement" /> to a <see cref="T:System.Single" />.</summary>
		/// <param name="element">The <see cref="T:System.Xml.Linq.XElement" /> to cast to <see cref="T:System.Single" />.</param>
		/// <returns>A <see cref="T:System.Single" /> that contains the content of this <see cref="T:System.Xml.Linq.XElement" />.</returns>
		/// <exception cref="T:System.FormatException">The element does not contain a valid <see cref="T:System.Single" /> value.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="element" /> parameter is <see langword="null" />.</exception>
		[CLSCompliant(false)]
		public static explicit operator float(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XmlConvert.ToSingle(element.Value);
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XElement" /> to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Single" />.</summary>
		/// <param name="element">The <see cref="T:System.Xml.Linq.XElement" /> to cast to <see cref="T:System.Nullable`1" /> of <see cref="T:System.Single" />.</param>
		/// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.Single" /> that contains the content of this <see cref="T:System.Xml.Linq.XElement" />.</returns>
		/// <exception cref="T:System.FormatException">The element is not <see langword="null" /> and does not contain a valid <see cref="T:System.Single" /> value.</exception>
		[CLSCompliant(false)]
		public static explicit operator float?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return new float?(XmlConvert.ToSingle(element.Value));
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XElement" /> to a <see cref="T:System.Double" />.</summary>
		/// <param name="element">The <see cref="T:System.Xml.Linq.XElement" /> to cast to <see cref="T:System.Double" />.</param>
		/// <returns>A <see cref="T:System.Double" /> that contains the content of this <see cref="T:System.Xml.Linq.XElement" />.</returns>
		/// <exception cref="T:System.FormatException">The element does not contain a valid <see cref="T:System.Double" /> value.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="element" /> parameter is <see langword="null" />.</exception>
		[CLSCompliant(false)]
		public static explicit operator double(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XmlConvert.ToDouble(element.Value);
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XElement" /> to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Double" />.</summary>
		/// <param name="element">The <see cref="T:System.Xml.Linq.XElement" /> to cast to <see cref="T:System.Nullable`1" /> of <see cref="T:System.Double" />.</param>
		/// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.Double" /> that contains the content of this <see cref="T:System.Xml.Linq.XElement" />.</returns>
		/// <exception cref="T:System.FormatException">The element is not <see langword="null" /> and does not contain a valid <see cref="T:System.Double" /> value.</exception>
		[CLSCompliant(false)]
		public static explicit operator double?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return new double?(XmlConvert.ToDouble(element.Value));
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XElement" /> to a <see cref="T:System.Decimal" />.</summary>
		/// <param name="element">The <see cref="T:System.Xml.Linq.XElement" /> to cast to <see cref="T:System.Decimal" />.</param>
		/// <returns>A <see cref="T:System.Decimal" /> that contains the content of this <see cref="T:System.Xml.Linq.XElement" />.</returns>
		/// <exception cref="T:System.FormatException">The element does not contain a valid <see cref="T:System.Decimal" /> value.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="element" /> parameter is <see langword="null" />.</exception>
		[CLSCompliant(false)]
		public static explicit operator decimal(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XmlConvert.ToDecimal(element.Value);
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XElement" /> to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Decimal" />.</summary>
		/// <param name="element">The <see cref="T:System.Xml.Linq.XElement" /> to cast to <see cref="T:System.Nullable`1" /> of <see cref="T:System.Decimal" />.</param>
		/// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.Decimal" /> that contains the content of this <see cref="T:System.Xml.Linq.XElement" />.</returns>
		/// <exception cref="T:System.FormatException">The element is not <see langword="null" /> and does not contain a valid <see cref="T:System.Decimal" /> value.</exception>
		[CLSCompliant(false)]
		public static explicit operator decimal?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return new decimal?(XmlConvert.ToDecimal(element.Value));
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XElement" /> to a <see cref="T:System.DateTime" />.</summary>
		/// <param name="element">The <see cref="T:System.Xml.Linq.XElement" /> to cast to <see cref="T:System.DateTime" />.</param>
		/// <returns>A <see cref="T:System.DateTime" /> that contains the content of this <see cref="T:System.Xml.Linq.XElement" />.</returns>
		/// <exception cref="T:System.FormatException">The element does not contain a valid <see cref="T:System.DateTime" /> value.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="element" /> parameter is <see langword="null" />.</exception>
		[CLSCompliant(false)]
		public static explicit operator DateTime(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return DateTime.Parse(element.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XElement" /> to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTime" />.</summary>
		/// <param name="element">The <see cref="T:System.Xml.Linq.XElement" /> to cast to <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTime" />.</param>
		/// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTime" /> that contains the content of this <see cref="T:System.Xml.Linq.XElement" />.</returns>
		/// <exception cref="T:System.FormatException">The element is not <see langword="null" /> and does not contain a valid <see cref="T:System.DateTime" /> value.</exception>
		[CLSCompliant(false)]
		public static explicit operator DateTime?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return new DateTime?(DateTime.Parse(element.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind));
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XAttribute" /> to a <see cref="T:System.DateTimeOffset" />.</summary>
		/// <param name="element">The <see cref="T:System.Xml.Linq.XElement" /> to cast to <see cref="T:System.DateTimeOffset" />.</param>
		/// <returns>A <see cref="T:System.DateTimeOffset" /> that contains the content of this <see cref="T:System.Xml.Linq.XElement" />.</returns>
		/// <exception cref="T:System.FormatException">The element does not contain a valid <see cref="T:System.DateTimeOffset" /> value.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="element" /> parameter is <see langword="null" />.</exception>
		[CLSCompliant(false)]
		public static explicit operator DateTimeOffset(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XmlConvert.ToDateTimeOffset(element.Value);
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XElement" /> to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTimeOffset" />.</summary>
		/// <param name="element">The <see cref="T:System.Xml.Linq.XElement" /> to cast to an <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTimeOffset" />.</param>
		/// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.DateTimeOffset" /> that contains the content of this <see cref="T:System.Xml.Linq.XElement" />.</returns>
		/// <exception cref="T:System.FormatException">The element is not <see langword="null" /> and does not contain a valid <see cref="T:System.DateTimeOffset" /> value.</exception>
		[CLSCompliant(false)]
		public static explicit operator DateTimeOffset?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return new DateTimeOffset?(XmlConvert.ToDateTimeOffset(element.Value));
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XElement" /> to a <see cref="T:System.TimeSpan" />.</summary>
		/// <param name="element">The <see cref="T:System.Xml.Linq.XElement" /> to cast to <see cref="T:System.TimeSpan" />.</param>
		/// <returns>A <see cref="T:System.TimeSpan" /> that contains the content of this <see cref="T:System.Xml.Linq.XElement" />.</returns>
		/// <exception cref="T:System.FormatException">The element does not contain a valid <see cref="T:System.TimeSpan" /> value.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="element" /> parameter is <see langword="null" />.</exception>
		[CLSCompliant(false)]
		public static explicit operator TimeSpan(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XmlConvert.ToTimeSpan(element.Value);
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XElement" /> to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.TimeSpan" />.</summary>
		/// <param name="element">The <see cref="T:System.Xml.Linq.XElement" /> to cast to <see cref="T:System.Nullable`1" /> of <see cref="T:System.TimeSpan" />.</param>
		/// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.TimeSpan" /> that contains the content of this <see cref="T:System.Xml.Linq.XElement" />.</returns>
		/// <exception cref="T:System.FormatException">The element is not <see langword="null" /> and does not contain a valid <see cref="T:System.TimeSpan" /> value.</exception>
		[CLSCompliant(false)]
		public static explicit operator TimeSpan?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return new TimeSpan?(XmlConvert.ToTimeSpan(element.Value));
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XElement" /> to a <see cref="T:System.Guid" />.</summary>
		/// <param name="element">The <see cref="T:System.Xml.Linq.XElement" /> to cast to <see cref="T:System.Guid" />.</param>
		/// <returns>A <see cref="T:System.Guid" /> that contains the content of this <see cref="T:System.Xml.Linq.XElement" />.</returns>
		/// <exception cref="T:System.FormatException">The element does not contain a valid <see cref="T:System.Guid" /> value.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="element" /> parameter is <see langword="null" />.</exception>
		[CLSCompliant(false)]
		public static explicit operator Guid(XElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return XmlConvert.ToGuid(element.Value);
		}

		/// <summary>Cast the value of this <see cref="T:System.Xml.Linq.XElement" /> to a <see cref="T:System.Nullable`1" /> of <see cref="T:System.Guid" />.</summary>
		/// <param name="element">The <see cref="T:System.Xml.Linq.XElement" /> to cast to <see cref="T:System.Nullable`1" /> of <see cref="T:System.Guid" />.</param>
		/// <returns>A <see cref="T:System.Nullable`1" /> of <see cref="T:System.Guid" /> that contains the content of this <see cref="T:System.Xml.Linq.XElement" />.</returns>
		/// <exception cref="T:System.FormatException">The element is not <see langword="null" /> and does not contain a valid <see cref="T:System.Guid" /> value.</exception>
		[CLSCompliant(false)]
		public static explicit operator Guid?(XElement element)
		{
			if (element == null)
			{
				return null;
			}
			return new Guid?(XmlConvert.ToGuid(element.Value));
		}

		/// <summary>Gets an XML schema definition that describes the XML representation of this object.</summary>
		/// <returns>An <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" /> method.</returns>
		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		/// <summary>Generates an object from its XML representation.</summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> from which the object is deserialized.</param>
		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (this.parent != null || this.annotations != null || this.content != null || this.lastAttr != null)
			{
				throw new InvalidOperationException("This instance cannot be deserialized.");
			}
			if (reader.MoveToContent() != XmlNodeType.Element)
			{
				throw new InvalidOperationException(SR.Format("The XmlReader must be on a node of type {0} instead of a node of type {1}.", XmlNodeType.Element, reader.NodeType));
			}
			this.ReadElementFrom(reader, LoadOptions.None);
		}

		/// <summary>Converts an object into its XML representation.</summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> to which this object is serialized.</param>
		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			this.WriteTo(writer);
		}

		internal override void AddAttribute(XAttribute a)
		{
			if (this.Attribute(a.Name) != null)
			{
				throw new InvalidOperationException("Duplicate attribute.");
			}
			if (a.parent != null)
			{
				a = new XAttribute(a);
			}
			this.AppendAttribute(a);
		}

		internal override void AddAttributeSkipNotify(XAttribute a)
		{
			if (this.Attribute(a.Name) != null)
			{
				throw new InvalidOperationException("Duplicate attribute.");
			}
			if (a.parent != null)
			{
				a = new XAttribute(a);
			}
			this.AppendAttributeSkipNotify(a);
		}

		internal void AppendAttribute(XAttribute a)
		{
			bool flag = base.NotifyChanging(a, XObjectChangeEventArgs.Add);
			if (a.parent != null)
			{
				throw new InvalidOperationException("This operation was corrupted by external code.");
			}
			this.AppendAttributeSkipNotify(a);
			if (flag)
			{
				base.NotifyChanged(a, XObjectChangeEventArgs.Add);
			}
		}

		internal void AppendAttributeSkipNotify(XAttribute a)
		{
			a.parent = this;
			if (this.lastAttr == null)
			{
				a.next = a;
			}
			else
			{
				a.next = this.lastAttr.next;
				this.lastAttr.next = a;
			}
			this.lastAttr = a;
		}

		private bool AttributesEqual(XElement e)
		{
			XAttribute next = this.lastAttr;
			XAttribute next2 = e.lastAttr;
			if (next != null && next2 != null)
			{
				for (;;)
				{
					next = next.next;
					next2 = next2.next;
					if (next.name != next2.name || next.value != next2.value)
					{
						break;
					}
					if (next == this.lastAttr)
					{
						goto Block_3;
					}
				}
				return false;
				Block_3:
				return next2 == e.lastAttr;
			}
			return next == null && next2 == null;
		}

		internal override XNode CloneNode()
		{
			return new XElement(this);
		}

		internal override bool DeepEquals(XNode node)
		{
			XElement xelement = node as XElement;
			return xelement != null && this.name == xelement.name && base.ContentsEqual(xelement) && this.AttributesEqual(xelement);
		}

		private IEnumerable<XAttribute> GetAttributes(XName name)
		{
			XAttribute a = this.lastAttr;
			if (a != null)
			{
				do
				{
					a = a.next;
					if (name == null || a.name == name)
					{
						yield return a;
					}
				}
				while (a.parent == this && a != this.lastAttr);
			}
			yield break;
		}

		private string GetNamespaceOfPrefixInScope(string prefix, XElement outOfScope)
		{
			for (XElement xelement = this; xelement != outOfScope; xelement = (xelement.parent as XElement))
			{
				XAttribute next = xelement.lastAttr;
				if (next != null)
				{
					for (;;)
					{
						next = next.next;
						if (next.IsNamespaceDeclaration && next.Name.LocalName == prefix)
						{
							break;
						}
						if (next == xelement.lastAttr)
						{
							goto IL_40;
						}
					}
					return next.Value;
				}
				IL_40:;
			}
			return null;
		}

		internal override int GetDeepHashCode()
		{
			int num = this.name.GetHashCode();
			num ^= base.ContentsHashCode();
			XAttribute next = this.lastAttr;
			if (next != null)
			{
				do
				{
					next = next.next;
					num ^= next.GetDeepHashCode();
				}
				while (next != this.lastAttr);
			}
			return num;
		}

		private void ReadElementFrom(XmlReader r, LoadOptions o)
		{
			this.ReadElementFromImpl(r, o);
			if (!r.IsEmptyElement)
			{
				r.Read();
				base.ReadContentFrom(r, o);
			}
			r.Read();
		}

		private Task ReadElementFromAsync(XmlReader r, LoadOptions o, CancellationToken cancellationTokentoken)
		{
			XElement.<ReadElementFromAsync>d__119 <ReadElementFromAsync>d__;
			<ReadElementFromAsync>d__.<>4__this = this;
			<ReadElementFromAsync>d__.r = r;
			<ReadElementFromAsync>d__.o = o;
			<ReadElementFromAsync>d__.cancellationTokentoken = cancellationTokentoken;
			<ReadElementFromAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ReadElementFromAsync>d__.<>1__state = -1;
			<ReadElementFromAsync>d__.<>t__builder.Start<XElement.<ReadElementFromAsync>d__119>(ref <ReadElementFromAsync>d__);
			return <ReadElementFromAsync>d__.<>t__builder.Task;
		}

		private void ReadElementFromImpl(XmlReader r, LoadOptions o)
		{
			if (r.ReadState != ReadState.Interactive)
			{
				throw new InvalidOperationException("The XmlReader state should be Interactive.");
			}
			this.name = XNamespace.Get(r.NamespaceURI).GetName(r.LocalName);
			if ((o & LoadOptions.SetBaseUri) != LoadOptions.None)
			{
				string baseURI = r.BaseURI;
				if (!string.IsNullOrEmpty(baseURI))
				{
					base.SetBaseUri(baseURI);
				}
			}
			IXmlLineInfo xmlLineInfo = null;
			if ((o & LoadOptions.SetLineInfo) != LoadOptions.None)
			{
				xmlLineInfo = (r as IXmlLineInfo);
				if (xmlLineInfo != null && xmlLineInfo.HasLineInfo())
				{
					base.SetLineInfo(xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
				}
			}
			if (r.MoveToFirstAttribute())
			{
				do
				{
					XAttribute xattribute = new XAttribute(XNamespace.Get((r.Prefix.Length == 0) ? string.Empty : r.NamespaceURI).GetName(r.LocalName), r.Value);
					if (xmlLineInfo != null && xmlLineInfo.HasLineInfo())
					{
						xattribute.SetLineInfo(xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
					}
					this.AppendAttributeSkipNotify(xattribute);
				}
				while (r.MoveToNextAttribute());
				r.MoveToElement();
			}
		}

		internal void RemoveAttribute(XAttribute a)
		{
			bool flag = base.NotifyChanging(a, XObjectChangeEventArgs.Remove);
			if (a.parent != this)
			{
				throw new InvalidOperationException("This operation was corrupted by external code.");
			}
			XAttribute xattribute = this.lastAttr;
			XAttribute next;
			while ((next = xattribute.next) != a)
			{
				xattribute = next;
			}
			if (xattribute == a)
			{
				this.lastAttr = null;
			}
			else
			{
				if (this.lastAttr == a)
				{
					this.lastAttr = xattribute;
				}
				xattribute.next = a.next;
			}
			a.parent = null;
			a.next = null;
			if (flag)
			{
				base.NotifyChanged(a, XObjectChangeEventArgs.Remove);
			}
		}

		private void RemoveAttributesSkipNotify()
		{
			if (this.lastAttr != null)
			{
				XAttribute xattribute = this.lastAttr;
				do
				{
					XAttribute next = xattribute.next;
					xattribute.parent = null;
					xattribute.next = null;
					xattribute = next;
				}
				while (xattribute != this.lastAttr);
				this.lastAttr = null;
			}
		}

		internal void SetEndElementLineInfo(int lineNumber, int linePosition)
		{
			base.AddAnnotation(new LineInfoEndElementAnnotation(lineNumber, linePosition));
		}

		internal override void ValidateNode(XNode node, XNode previous)
		{
			if (node is XDocument)
			{
				throw new ArgumentException(SR.Format("A node of type {0} cannot be added to content.", XmlNodeType.Document));
			}
			if (node is XDocumentType)
			{
				throw new ArgumentException(SR.Format("A node of type {0} cannot be added to content.", XmlNodeType.DocumentType));
			}
		}

		internal XName name;

		internal XAttribute lastAttr;

		private struct AsyncConstructionSentry
		{
		}
	}
}
