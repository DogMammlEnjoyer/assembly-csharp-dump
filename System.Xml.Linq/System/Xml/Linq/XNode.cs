using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Xml.Linq
{
	/// <summary>Represents the abstract concept of a node (element, comment, document type, processing instruction, or text node) in the XML tree.</summary>
	public abstract class XNode : XObject
	{
		internal XNode()
		{
		}

		/// <summary>Gets the next sibling node of this node.</summary>
		/// <returns>The <see cref="T:System.Xml.Linq.XNode" /> that contains the next sibling node.</returns>
		public XNode NextNode
		{
			get
			{
				if (this.parent != null && this != this.parent.content)
				{
					return this.next;
				}
				return null;
			}
		}

		/// <summary>Gets the previous sibling node of this node.</summary>
		/// <returns>The <see cref="T:System.Xml.Linq.XNode" /> that contains the previous sibling node.</returns>
		public XNode PreviousNode
		{
			get
			{
				if (this.parent == null)
				{
					return null;
				}
				XNode xnode = ((XNode)this.parent.content).next;
				XNode result = null;
				while (xnode != this)
				{
					result = xnode;
					xnode = xnode.next;
				}
				return result;
			}
		}

		/// <summary>Gets a comparer that can compare the relative position of two nodes.</summary>
		/// <returns>An <see cref="T:System.Xml.Linq.XNodeDocumentOrderComparer" /> that can compare the relative position of two nodes.</returns>
		public static XNodeDocumentOrderComparer DocumentOrderComparer
		{
			get
			{
				if (XNode.s_documentOrderComparer == null)
				{
					XNode.s_documentOrderComparer = new XNodeDocumentOrderComparer();
				}
				return XNode.s_documentOrderComparer;
			}
		}

		/// <summary>Gets a comparer that can compare two nodes for value equality.</summary>
		/// <returns>A <see cref="T:System.Xml.Linq.XNodeEqualityComparer" /> that can compare two nodes for value equality.</returns>
		public static XNodeEqualityComparer EqualityComparer
		{
			get
			{
				if (XNode.s_equalityComparer == null)
				{
					XNode.s_equalityComparer = new XNodeEqualityComparer();
				}
				return XNode.s_equalityComparer;
			}
		}

		/// <summary>Adds the specified content immediately after this node.</summary>
		/// <param name="content">A content object that contains simple content or a collection of content objects to be added after this node.</param>
		/// <exception cref="T:System.InvalidOperationException">The parent is <see langword="null" />.</exception>
		public void AddAfterSelf(object content)
		{
			if (this.parent == null)
			{
				throw new InvalidOperationException("The parent is missing.");
			}
			new Inserter(this.parent, this).Add(content);
		}

		/// <summary>Adds the specified content immediately after this node.</summary>
		/// <param name="content">A parameter list of content objects.</param>
		/// <exception cref="T:System.InvalidOperationException">The parent is <see langword="null" />.</exception>
		public void AddAfterSelf(params object[] content)
		{
			this.AddAfterSelf(content);
		}

		/// <summary>Adds the specified content immediately before this node.</summary>
		/// <param name="content">A content object that contains simple content or a collection of content objects to be added before this node.</param>
		/// <exception cref="T:System.InvalidOperationException">The parent is <see langword="null" />.</exception>
		public void AddBeforeSelf(object content)
		{
			if (this.parent == null)
			{
				throw new InvalidOperationException("The parent is missing.");
			}
			XNode xnode = (XNode)this.parent.content;
			while (xnode.next != this)
			{
				xnode = xnode.next;
			}
			if (xnode == this.parent.content)
			{
				xnode = null;
			}
			new Inserter(this.parent, xnode).Add(content);
		}

		/// <summary>Adds the specified content immediately before this node.</summary>
		/// <param name="content">A parameter list of content objects.</param>
		/// <exception cref="T:System.InvalidOperationException">The parent is <see langword="null" />.</exception>
		public void AddBeforeSelf(params object[] content)
		{
			this.AddBeforeSelf(content);
		}

		/// <summary>Returns a collection of the ancestor elements of this node.</summary>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> of the ancestor elements of this node.</returns>
		public IEnumerable<XElement> Ancestors()
		{
			return this.GetAncestors(null, false);
		}

		/// <summary>Returns a filtered collection of the ancestor elements of this node. Only elements that have a matching <see cref="T:System.Xml.Linq.XName" /> are included in the collection.</summary>
		/// <param name="name">The <see cref="T:System.Xml.Linq.XName" /> to match.</param>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> of the ancestor elements of this node. Only elements that have a matching <see cref="T:System.Xml.Linq.XName" /> are included in the collection.  
		///  The nodes in the returned collection are in reverse document order.  
		///  This method uses deferred execution.</returns>
		public IEnumerable<XElement> Ancestors(XName name)
		{
			if (!(name != null))
			{
				return XElement.EmptySequence;
			}
			return this.GetAncestors(name, false);
		}

		/// <summary>Compares two nodes to determine their relative XML document order.</summary>
		/// <param name="n1">First <see cref="T:System.Xml.Linq.XNode" /> to compare.</param>
		/// <param name="n2">Second <see cref="T:System.Xml.Linq.XNode" /> to compare.</param>
		/// <returns>An <see langword="int" /> containing 0 if the nodes are equal; -1 if <paramref name="n1" /> is before <paramref name="n2" />; 1 if <paramref name="n1" /> is after <paramref name="n2" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">The two nodes do not share a common ancestor.</exception>
		public static int CompareDocumentOrder(XNode n1, XNode n2)
		{
			if (n1 == n2)
			{
				return 0;
			}
			if (n1 == null)
			{
				return -1;
			}
			if (n2 == null)
			{
				return 1;
			}
			if (n1.parent != n2.parent)
			{
				int num = 0;
				XNode xnode = n1;
				while (xnode.parent != null)
				{
					xnode = xnode.parent;
					num++;
				}
				XNode xnode2 = n2;
				while (xnode2.parent != null)
				{
					xnode2 = xnode2.parent;
					num--;
				}
				if (xnode != xnode2)
				{
					throw new InvalidOperationException("A common ancestor is missing.");
				}
				if (num < 0)
				{
					do
					{
						n2 = n2.parent;
						num++;
					}
					while (num != 0);
					if (n1 == n2)
					{
						return -1;
					}
				}
				else if (num > 0)
				{
					do
					{
						n1 = n1.parent;
						num--;
					}
					while (num != 0);
					if (n1 == n2)
					{
						return 1;
					}
				}
				while (n1.parent != n2.parent)
				{
					n1 = n1.parent;
					n2 = n2.parent;
				}
			}
			else if (n1.parent == null)
			{
				throw new InvalidOperationException("A common ancestor is missing.");
			}
			XNode xnode3 = (XNode)n1.parent.content;
			for (;;)
			{
				xnode3 = xnode3.next;
				if (xnode3 == n1)
				{
					break;
				}
				if (xnode3 == n2)
				{
					return 1;
				}
			}
			return -1;
		}

		/// <summary>Creates an <see cref="T:System.Xml.XmlReader" /> for this node.</summary>
		/// <returns>An <see cref="T:System.Xml.XmlReader" /> that can be used to read this node and its descendants.</returns>
		public XmlReader CreateReader()
		{
			return new XNodeReader(this, null);
		}

		/// <summary>Creates an <see cref="T:System.Xml.XmlReader" /> with the options specified by the <paramref name="readerOptions" /> parameter.</summary>
		/// <param name="readerOptions">A <see cref="T:System.Xml.Linq.ReaderOptions" /> object that specifies whether to omit duplicate namespaces.</param>
		/// <returns>An <see cref="T:System.Xml.XmlReader" /> object.</returns>
		public XmlReader CreateReader(ReaderOptions readerOptions)
		{
			return new XNodeReader(this, null, readerOptions);
		}

		/// <summary>Returns a collection of the sibling nodes after this node, in document order.</summary>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XNode" /> of the sibling nodes after this node, in document order.</returns>
		public IEnumerable<XNode> NodesAfterSelf()
		{
			XNode i = this;
			while (i.parent != null && i != i.parent.content)
			{
				i = i.next;
				yield return i;
			}
			yield break;
		}

		/// <summary>Returns a collection of the sibling nodes before this node, in document order.</summary>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XNode" /> of the sibling nodes before this node, in document order.</returns>
		public IEnumerable<XNode> NodesBeforeSelf()
		{
			if (this.parent != null)
			{
				XNode i = (XNode)this.parent.content;
				do
				{
					i = i.next;
					if (i == this)
					{
						break;
					}
					yield return i;
				}
				while (this.parent != null && this.parent == i.parent);
				i = null;
			}
			yield break;
		}

		/// <summary>Returns a collection of the sibling elements after this node, in document order.</summary>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> of the sibling elements after this node, in document order.</returns>
		public IEnumerable<XElement> ElementsAfterSelf()
		{
			return this.GetElementsAfterSelf(null);
		}

		/// <summary>Returns a filtered collection of the sibling elements after this node, in document order. Only elements that have a matching <see cref="T:System.Xml.Linq.XName" /> are included in the collection.</summary>
		/// <param name="name">The <see cref="T:System.Xml.Linq.XName" /> to match.</param>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> of the sibling elements after this node, in document order. Only elements that have a matching <see cref="T:System.Xml.Linq.XName" /> are included in the collection.</returns>
		public IEnumerable<XElement> ElementsAfterSelf(XName name)
		{
			if (!(name != null))
			{
				return XElement.EmptySequence;
			}
			return this.GetElementsAfterSelf(name);
		}

		/// <summary>Returns a collection of the sibling elements before this node, in document order.</summary>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> of the sibling elements before this node, in document order.</returns>
		public IEnumerable<XElement> ElementsBeforeSelf()
		{
			return this.GetElementsBeforeSelf(null);
		}

		/// <summary>Returns a filtered collection of the sibling elements before this node, in document order. Only elements that have a matching <see cref="T:System.Xml.Linq.XName" /> are included in the collection.</summary>
		/// <param name="name">The <see cref="T:System.Xml.Linq.XName" /> to match.</param>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> of the sibling elements before this node, in document order. Only elements that have a matching <see cref="T:System.Xml.Linq.XName" /> are included in the collection.</returns>
		public IEnumerable<XElement> ElementsBeforeSelf(XName name)
		{
			if (!(name != null))
			{
				return XElement.EmptySequence;
			}
			return this.GetElementsBeforeSelf(name);
		}

		/// <summary>Determines if the current node appears after a specified node in terms of document order.</summary>
		/// <param name="node">The <see cref="T:System.Xml.Linq.XNode" /> to compare for document order.</param>
		/// <returns>
		///   <see langword="true" /> if this node appears after the specified node; otherwise <see langword="false" />.</returns>
		public bool IsAfter(XNode node)
		{
			return XNode.CompareDocumentOrder(this, node) > 0;
		}

		/// <summary>Determines if the current node appears before a specified node in terms of document order.</summary>
		/// <param name="node">The <see cref="T:System.Xml.Linq.XNode" /> to compare for document order.</param>
		/// <returns>
		///   <see langword="true" /> if this node appears before the specified node; otherwise <see langword="false" />.</returns>
		public bool IsBefore(XNode node)
		{
			return XNode.CompareDocumentOrder(this, node) < 0;
		}

		/// <summary>Creates an <see cref="T:System.Xml.Linq.XNode" /> from an <see cref="T:System.Xml.XmlReader" />.</summary>
		/// <param name="reader">An <see cref="T:System.Xml.XmlReader" /> positioned at the node to read into this <see cref="T:System.Xml.Linq.XNode" />.</param>
		/// <returns>An <see cref="T:System.Xml.Linq.XNode" /> that contains the node and its descendant nodes that were read from the reader. The runtime type of the node is determined by the node type (<see cref="P:System.Xml.Linq.XObject.NodeType" />) of the first node encountered in the reader.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on a recognized node type.</exception>
		/// <exception cref="T:System.Xml.XmlException">The underlying <see cref="T:System.Xml.XmlReader" /> throws an exception.</exception>
		public static XNode ReadFrom(XmlReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (reader.ReadState != ReadState.Interactive)
			{
				throw new InvalidOperationException("The XmlReader state should be Interactive.");
			}
			switch (reader.NodeType)
			{
			case XmlNodeType.Element:
				return new XElement(reader);
			case XmlNodeType.Text:
			case XmlNodeType.Whitespace:
			case XmlNodeType.SignificantWhitespace:
				return new XText(reader);
			case XmlNodeType.CDATA:
				return new XCData(reader);
			case XmlNodeType.ProcessingInstruction:
				return new XProcessingInstruction(reader);
			case XmlNodeType.Comment:
				return new XComment(reader);
			case XmlNodeType.DocumentType:
				return new XDocumentType(reader);
			}
			throw new InvalidOperationException(SR.Format("The XmlReader should not be on a node of type {0}.", reader.NodeType));
		}

		public static Task<XNode> ReadFromAsync(XmlReader reader, CancellationToken cancellationToken)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<XNode>(cancellationToken);
			}
			return XNode.ReadFromAsyncInternal(reader, cancellationToken);
		}

		private static Task<XNode> ReadFromAsyncInternal(XmlReader reader, CancellationToken cancellationToken)
		{
			XNode.<ReadFromAsyncInternal>d__31 <ReadFromAsyncInternal>d__;
			<ReadFromAsyncInternal>d__.reader = reader;
			<ReadFromAsyncInternal>d__.cancellationToken = cancellationToken;
			<ReadFromAsyncInternal>d__.<>t__builder = AsyncTaskMethodBuilder<XNode>.Create();
			<ReadFromAsyncInternal>d__.<>1__state = -1;
			<ReadFromAsyncInternal>d__.<>t__builder.Start<XNode.<ReadFromAsyncInternal>d__31>(ref <ReadFromAsyncInternal>d__);
			return <ReadFromAsyncInternal>d__.<>t__builder.Task;
		}

		/// <summary>Removes this node from its parent.</summary>
		/// <exception cref="T:System.InvalidOperationException">The parent is <see langword="null" />.</exception>
		public void Remove()
		{
			if (this.parent == null)
			{
				throw new InvalidOperationException("The parent is missing.");
			}
			this.parent.RemoveNode(this);
		}

		/// <summary>Replaces this node with the specified content.</summary>
		/// <param name="content">Content that replaces this node.</param>
		public void ReplaceWith(object content)
		{
			if (this.parent == null)
			{
				throw new InvalidOperationException("The parent is missing.");
			}
			XContainer parent = this.parent;
			XNode xnode = (XNode)this.parent.content;
			while (xnode.next != this)
			{
				xnode = xnode.next;
			}
			if (xnode == this.parent.content)
			{
				xnode = null;
			}
			this.parent.RemoveNode(this);
			if (xnode != null && xnode.parent != parent)
			{
				throw new InvalidOperationException("This operation was corrupted by external code.");
			}
			new Inserter(parent, xnode).Add(content);
		}

		/// <summary>Replaces this node with the specified content.</summary>
		/// <param name="content">A parameter list of the new content.</param>
		public void ReplaceWith(params object[] content)
		{
			this.ReplaceWith(content);
		}

		/// <summary>Returns the indented XML for this node.</summary>
		/// <returns>A <see cref="T:System.String" /> containing the indented XML.</returns>
		public override string ToString()
		{
			return this.GetXmlString(base.GetSaveOptionsFromAnnotations());
		}

		/// <summary>Returns the XML for this node, optionally disabling formatting.</summary>
		/// <param name="options">A <see cref="T:System.Xml.Linq.SaveOptions" /> that specifies formatting behavior.</param>
		/// <returns>A <see cref="T:System.String" /> containing the XML.</returns>
		public string ToString(SaveOptions options)
		{
			return this.GetXmlString(options);
		}

		/// <summary>Compares the values of two nodes, including the values of all descendant nodes.</summary>
		/// <param name="n1">The first <see cref="T:System.Xml.Linq.XNode" /> to compare.</param>
		/// <param name="n2">The second <see cref="T:System.Xml.Linq.XNode" /> to compare.</param>
		/// <returns>
		///   <see langword="true" /> if the nodes are equal; otherwise <see langword="false" />.</returns>
		public static bool DeepEquals(XNode n1, XNode n2)
		{
			return n1 == n2 || (n1 != null && n2 != null && n1.DeepEquals(n2));
		}

		/// <summary>Writes this node to an <see cref="T:System.Xml.XmlWriter" />.</summary>
		/// <param name="writer">An <see cref="T:System.Xml.XmlWriter" /> into which this method will write.</param>
		public abstract void WriteTo(XmlWriter writer);

		public abstract Task WriteToAsync(XmlWriter writer, CancellationToken cancellationToken);

		internal virtual void AppendText(StringBuilder sb)
		{
		}

		internal abstract XNode CloneNode();

		internal abstract bool DeepEquals(XNode node);

		internal IEnumerable<XElement> GetAncestors(XName name, bool self)
		{
			for (XElement e = (self ? this : this.parent) as XElement; e != null; e = (e.parent as XElement))
			{
				if (name == null || e.name == name)
				{
					yield return e;
				}
			}
			yield break;
		}

		private IEnumerable<XElement> GetElementsAfterSelf(XName name)
		{
			XNode i = this;
			while (i.parent != null && i != i.parent.content)
			{
				i = i.next;
				XElement xelement = i as XElement;
				if (xelement != null && (name == null || xelement.name == name))
				{
					yield return xelement;
				}
			}
			yield break;
		}

		private IEnumerable<XElement> GetElementsBeforeSelf(XName name)
		{
			if (this.parent != null)
			{
				XNode i = (XNode)this.parent.content;
				do
				{
					i = i.next;
					if (i == this)
					{
						break;
					}
					XElement xelement = i as XElement;
					if (xelement != null && (name == null || xelement.name == name))
					{
						yield return xelement;
					}
				}
				while (this.parent != null && this.parent == i.parent);
				i = null;
			}
			yield break;
		}

		internal abstract int GetDeepHashCode();

		internal static XmlReaderSettings GetXmlReaderSettings(LoadOptions o)
		{
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			if ((o & LoadOptions.PreserveWhitespace) == LoadOptions.None)
			{
				xmlReaderSettings.IgnoreWhitespace = true;
			}
			xmlReaderSettings.DtdProcessing = DtdProcessing.Parse;
			xmlReaderSettings.MaxCharactersFromEntities = 10000000L;
			return xmlReaderSettings;
		}

		internal static XmlWriterSettings GetXmlWriterSettings(SaveOptions o)
		{
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			if ((o & SaveOptions.DisableFormatting) == SaveOptions.None)
			{
				xmlWriterSettings.Indent = true;
			}
			if ((o & SaveOptions.OmitDuplicateNamespaces) != SaveOptions.None)
			{
				xmlWriterSettings.NamespaceHandling |= NamespaceHandling.OmitDuplicates;
			}
			return xmlWriterSettings;
		}

		private string GetXmlString(SaveOptions o)
		{
			string result;
			using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
			{
				XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
				xmlWriterSettings.OmitXmlDeclaration = true;
				if ((o & SaveOptions.DisableFormatting) == SaveOptions.None)
				{
					xmlWriterSettings.Indent = true;
				}
				if ((o & SaveOptions.OmitDuplicateNamespaces) != SaveOptions.None)
				{
					xmlWriterSettings.NamespaceHandling |= NamespaceHandling.OmitDuplicates;
				}
				if (this is XText)
				{
					xmlWriterSettings.ConformanceLevel = ConformanceLevel.Fragment;
				}
				using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, xmlWriterSettings))
				{
					XDocument xdocument = this as XDocument;
					if (xdocument != null)
					{
						xdocument.WriteContentTo(xmlWriter);
					}
					else
					{
						this.WriteTo(xmlWriter);
					}
				}
				result = stringWriter.ToString();
			}
			return result;
		}

		private static XNodeDocumentOrderComparer s_documentOrderComparer;

		private static XNodeEqualityComparer s_equalityComparer;

		internal XNode next;
	}
}
