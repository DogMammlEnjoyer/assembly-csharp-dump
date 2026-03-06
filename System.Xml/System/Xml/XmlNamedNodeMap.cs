using System;
using System.Collections;
using Unity;

namespace System.Xml
{
	/// <summary>Represents a collection of nodes that can be accessed by name or index.</summary>
	public class XmlNamedNodeMap : IEnumerable
	{
		internal XmlNamedNodeMap(XmlNode parent)
		{
			this.parent = parent;
		}

		/// <summary>Retrieves an <see cref="T:System.Xml.XmlNode" /> specified by name.</summary>
		/// <param name="name">The qualified name of the node to retrieve. It is matched against the <see cref="P:System.Xml.XmlNode.Name" /> property of the matching node.</param>
		/// <returns>An <see langword="XmlNode" /> with the specified name or <see langword="null" /> if a matching node is not found.</returns>
		public virtual XmlNode GetNamedItem(string name)
		{
			int num = this.FindNodeOffset(name);
			if (num >= 0)
			{
				return (XmlNode)this.nodes[num];
			}
			return null;
		}

		/// <summary>Adds an <see cref="T:System.Xml.XmlNode" /> using its <see cref="P:System.Xml.XmlNode.Name" /> property.</summary>
		/// <param name="node">An <see langword="XmlNode" /> to store in the <see langword="XmlNamedNodeMap" />. If a node with that name is already present in the map, it is replaced by the new one.</param>
		/// <returns>If the <paramref name="node" /> replaces an existing node with the same name, the old node is returned; otherwise, <see langword="null" /> is returned.</returns>
		/// <exception cref="T:System.ArgumentException">The <paramref name="node" /> was created from a different <see cref="T:System.Xml.XmlDocument" /> than the one that created the <see langword="XmlNamedNodeMap" />; or the <see langword="XmlNamedNodeMap" /> is read-only.</exception>
		public virtual XmlNode SetNamedItem(XmlNode node)
		{
			if (node == null)
			{
				return null;
			}
			int num = this.FindNodeOffset(node.LocalName, node.NamespaceURI);
			if (num == -1)
			{
				this.AddNode(node);
				return null;
			}
			return this.ReplaceNodeAt(num, node);
		}

		/// <summary>Removes the node from the <see langword="XmlNamedNodeMap" />.</summary>
		/// <param name="name">The qualified name of the node to remove. The name is matched against the <see cref="P:System.Xml.XmlNode.Name" /> property of the matching node.</param>
		/// <returns>The <see langword="XmlNode" /> removed from this <see langword="XmlNamedNodeMap" /> or <see langword="null" /> if a matching node was not found.</returns>
		public virtual XmlNode RemoveNamedItem(string name)
		{
			int num = this.FindNodeOffset(name);
			if (num >= 0)
			{
				return this.RemoveNodeAt(num);
			}
			return null;
		}

		/// <summary>Gets the number of nodes in the <see langword="XmlNamedNodeMap" />.</summary>
		/// <returns>The number of nodes.</returns>
		public virtual int Count
		{
			get
			{
				return this.nodes.Count;
			}
		}

		/// <summary>Retrieves the node at the specified index in the <see langword="XmlNamedNodeMap" />.</summary>
		/// <param name="index">The index position of the node to retrieve from the <see langword="XmlNamedNodeMap" />. The index is zero-based; therefore, the index of the first node is 0 and the index of the last node is <see cref="P:System.Xml.XmlNamedNodeMap.Count" /> -1.</param>
		/// <returns>The <see cref="T:System.Xml.XmlNode" /> at the specified index. If <paramref name="index" /> is less than 0 or greater than or equal to the <see cref="P:System.Xml.XmlNamedNodeMap.Count" /> property, <see langword="null" /> is returned.</returns>
		public virtual XmlNode Item(int index)
		{
			if (index < 0 || index >= this.nodes.Count)
			{
				return null;
			}
			XmlNode result;
			try
			{
				result = (XmlNode)this.nodes[index];
			}
			catch (ArgumentOutOfRangeException)
			{
				throw new IndexOutOfRangeException(Res.GetString("The index being passed in is out of range."));
			}
			return result;
		}

		/// <summary>Retrieves a node with the matching <see cref="P:System.Xml.XmlNode.LocalName" /> and <see cref="P:System.Xml.XmlNode.NamespaceURI" />.</summary>
		/// <param name="localName">The local name of the node to retrieve.</param>
		/// <param name="namespaceURI">The namespace Uniform Resource Identifier (URI) of the node to retrieve.</param>
		/// <returns>An <see cref="T:System.Xml.XmlNode" /> with the matching local name and namespace URI or <see langword="null" /> if a matching node was not found.</returns>
		public virtual XmlNode GetNamedItem(string localName, string namespaceURI)
		{
			int num = this.FindNodeOffset(localName, namespaceURI);
			if (num >= 0)
			{
				return (XmlNode)this.nodes[num];
			}
			return null;
		}

		/// <summary>Removes a node with the matching <see cref="P:System.Xml.XmlNode.LocalName" /> and <see cref="P:System.Xml.XmlNode.NamespaceURI" />.</summary>
		/// <param name="localName">The local name of the node to remove.</param>
		/// <param name="namespaceURI">The namespace URI of the node to remove.</param>
		/// <returns>The <see cref="T:System.Xml.XmlNode" /> removed or <see langword="null" /> if a matching node was not found.</returns>
		public virtual XmlNode RemoveNamedItem(string localName, string namespaceURI)
		{
			int num = this.FindNodeOffset(localName, namespaceURI);
			if (num >= 0)
			{
				return this.RemoveNodeAt(num);
			}
			return null;
		}

		/// <summary>Provides support for the "foreach" style iteration over the collection of nodes in the <see langword="XmlNamedNodeMap" />.</summary>
		/// <returns>An enumerator object.</returns>
		public virtual IEnumerator GetEnumerator()
		{
			return this.nodes.GetEnumerator();
		}

		internal int FindNodeOffset(string name)
		{
			int count = this.Count;
			for (int i = 0; i < count; i++)
			{
				XmlNode xmlNode = (XmlNode)this.nodes[i];
				if (name == xmlNode.Name)
				{
					return i;
				}
			}
			return -1;
		}

		internal int FindNodeOffset(string localName, string namespaceURI)
		{
			int count = this.Count;
			for (int i = 0; i < count; i++)
			{
				XmlNode xmlNode = (XmlNode)this.nodes[i];
				if (xmlNode.LocalName == localName && xmlNode.NamespaceURI == namespaceURI)
				{
					return i;
				}
			}
			return -1;
		}

		internal virtual XmlNode AddNode(XmlNode node)
		{
			XmlNode oldParent;
			if (node.NodeType == XmlNodeType.Attribute)
			{
				oldParent = ((XmlAttribute)node).OwnerElement;
			}
			else
			{
				oldParent = node.ParentNode;
			}
			string value = node.Value;
			XmlNodeChangedEventArgs eventArgs = this.parent.GetEventArgs(node, oldParent, this.parent, value, value, XmlNodeChangedAction.Insert);
			if (eventArgs != null)
			{
				this.parent.BeforeEvent(eventArgs);
			}
			this.nodes.Add(node);
			node.SetParent(this.parent);
			if (eventArgs != null)
			{
				this.parent.AfterEvent(eventArgs);
			}
			return node;
		}

		internal virtual XmlNode AddNodeForLoad(XmlNode node, XmlDocument doc)
		{
			XmlNodeChangedEventArgs insertEventArgsForLoad = doc.GetInsertEventArgsForLoad(node, this.parent);
			if (insertEventArgsForLoad != null)
			{
				doc.BeforeEvent(insertEventArgsForLoad);
			}
			this.nodes.Add(node);
			node.SetParent(this.parent);
			if (insertEventArgsForLoad != null)
			{
				doc.AfterEvent(insertEventArgsForLoad);
			}
			return node;
		}

		internal virtual XmlNode RemoveNodeAt(int i)
		{
			XmlNode xmlNode = (XmlNode)this.nodes[i];
			string value = xmlNode.Value;
			XmlNodeChangedEventArgs eventArgs = this.parent.GetEventArgs(xmlNode, this.parent, null, value, value, XmlNodeChangedAction.Remove);
			if (eventArgs != null)
			{
				this.parent.BeforeEvent(eventArgs);
			}
			this.nodes.RemoveAt(i);
			xmlNode.SetParent(null);
			if (eventArgs != null)
			{
				this.parent.AfterEvent(eventArgs);
			}
			return xmlNode;
		}

		internal XmlNode ReplaceNodeAt(int i, XmlNode node)
		{
			XmlNode result = this.RemoveNodeAt(i);
			this.InsertNodeAt(i, node);
			return result;
		}

		internal virtual XmlNode InsertNodeAt(int i, XmlNode node)
		{
			XmlNode oldParent;
			if (node.NodeType == XmlNodeType.Attribute)
			{
				oldParent = ((XmlAttribute)node).OwnerElement;
			}
			else
			{
				oldParent = node.ParentNode;
			}
			string value = node.Value;
			XmlNodeChangedEventArgs eventArgs = this.parent.GetEventArgs(node, oldParent, this.parent, value, value, XmlNodeChangedAction.Insert);
			if (eventArgs != null)
			{
				this.parent.BeforeEvent(eventArgs);
			}
			this.nodes.Insert(i, node);
			node.SetParent(this.parent);
			if (eventArgs != null)
			{
				this.parent.AfterEvent(eventArgs);
			}
			return node;
		}

		internal XmlNamedNodeMap()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		internal XmlNode parent;

		internal XmlNamedNodeMap.SmallXmlNodeList nodes;

		internal struct SmallXmlNodeList
		{
			public int Count
			{
				get
				{
					if (this.field == null)
					{
						return 0;
					}
					ArrayList arrayList = this.field as ArrayList;
					if (arrayList != null)
					{
						return arrayList.Count;
					}
					return 1;
				}
			}

			public object this[int index]
			{
				get
				{
					if (this.field == null)
					{
						throw new ArgumentOutOfRangeException("index");
					}
					ArrayList arrayList = this.field as ArrayList;
					if (arrayList != null)
					{
						return arrayList[index];
					}
					if (index != 0)
					{
						throw new ArgumentOutOfRangeException("index");
					}
					return this.field;
				}
			}

			public void Add(object value)
			{
				if (this.field == null)
				{
					if (value == null)
					{
						this.field = new ArrayList
						{
							null
						};
						return;
					}
					this.field = value;
					return;
				}
				else
				{
					ArrayList arrayList = this.field as ArrayList;
					if (arrayList != null)
					{
						arrayList.Add(value);
						return;
					}
					this.field = new ArrayList
					{
						this.field,
						value
					};
					return;
				}
			}

			public void RemoveAt(int index)
			{
				if (this.field == null)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				ArrayList arrayList = this.field as ArrayList;
				if (arrayList != null)
				{
					arrayList.RemoveAt(index);
					return;
				}
				if (index != 0)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				this.field = null;
			}

			public void Insert(int index, object value)
			{
				if (this.field == null)
				{
					if (index != 0)
					{
						throw new ArgumentOutOfRangeException("index");
					}
					this.Add(value);
					return;
				}
				else
				{
					ArrayList arrayList = this.field as ArrayList;
					if (arrayList != null)
					{
						arrayList.Insert(index, value);
						return;
					}
					if (index == 0)
					{
						this.field = new ArrayList
						{
							value,
							this.field
						};
						return;
					}
					if (index == 1)
					{
						this.field = new ArrayList
						{
							this.field,
							value
						};
						return;
					}
					throw new ArgumentOutOfRangeException("index");
				}
			}

			public IEnumerator GetEnumerator()
			{
				if (this.field == null)
				{
					return XmlDocument.EmptyEnumerator;
				}
				ArrayList arrayList = this.field as ArrayList;
				if (arrayList != null)
				{
					return arrayList.GetEnumerator();
				}
				return new XmlNamedNodeMap.SmallXmlNodeList.SingleObjectEnumerator(this.field);
			}

			private object field;

			private class SingleObjectEnumerator : IEnumerator
			{
				public SingleObjectEnumerator(object value)
				{
					this.loneValue = value;
				}

				public object Current
				{
					get
					{
						if (this.position != 0)
						{
							throw new InvalidOperationException();
						}
						return this.loneValue;
					}
				}

				public bool MoveNext()
				{
					if (this.position < 0)
					{
						this.position = 0;
						return true;
					}
					this.position = 1;
					return false;
				}

				public void Reset()
				{
					this.position = -1;
				}

				private object loneValue;

				private int position = -1;
			}
		}
	}
}
