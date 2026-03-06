using System;

namespace System.Xml
{
	/// <summary>Represents an entity reference node.</summary>
	public class XmlEntityReference : XmlLinkedNode
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.XmlEntityReference" /> class.</summary>
		/// <param name="name">The name of the entity reference; see the <see cref="P:System.Xml.XmlEntityReference.Name" /> property.</param>
		/// <param name="doc">The parent XML document.</param>
		protected internal XmlEntityReference(string name, XmlDocument doc) : base(doc)
		{
			if (!doc.IsLoading && name.Length > 0 && name[0] == '#')
			{
				throw new ArgumentException(Res.GetString("Cannot create an 'EntityReference' node with a name starting with '#'."));
			}
			this.name = doc.NameTable.Add(name);
			doc.fEntRefNodesPresent = true;
		}

		/// <summary>Gets the name of the node.</summary>
		/// <returns>The name of the entity referenced.</returns>
		public override string Name
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>Gets the local name of the node.</summary>
		/// <returns>For <see langword="XmlEntityReference" /> nodes, this property returns the name of the entity referenced.</returns>
		public override string LocalName
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>Gets or sets the value of the node.</summary>
		/// <returns>The value of the node. For <see langword="XmlEntityReference" /> nodes, this property returns <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentException">Node is read-only. </exception>
		/// <exception cref="T:System.InvalidOperationException">Setting the property. </exception>
		public override string Value
		{
			get
			{
				return null;
			}
			set
			{
				throw new InvalidOperationException(Res.GetString("'EntityReference' nodes have no support for setting value."));
			}
		}

		/// <summary>Gets the type of the node.</summary>
		/// <returns>The node type. For <see langword="XmlEntityReference" /> nodes, the value is XmlNodeType.EntityReference.</returns>
		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.EntityReference;
			}
		}

		/// <summary>Creates a duplicate of this node.</summary>
		/// <param name="deep">
		///       <see langword="true" /> to recursively clone the subtree under the specified node; <see langword="false" /> to clone only the node itself. For <see langword="XmlEntityReference" /> nodes, this method always returns an entity reference node with no children. The replacement text is set when the node is inserted into a parent. </param>
		/// <returns>The cloned node.</returns>
		public override XmlNode CloneNode(bool deep)
		{
			return this.OwnerDocument.CreateEntityReference(this.name);
		}

		/// <summary>Gets a value indicating whether the node is read-only.</summary>
		/// <returns>
		///     <see langword="true" /> if the node is read-only; otherwise <see langword="false" />.Because <see langword="XmlEntityReference" /> nodes are read-only, this property always returns <see langword="true" />.</returns>
		public override bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		internal override bool IsContainer
		{
			get
			{
				return true;
			}
		}

		internal override void SetParent(XmlNode node)
		{
			base.SetParent(node);
			if (this.LastNode == null && node != null && node != this.OwnerDocument)
			{
				new XmlLoader().ExpandEntityReference(this);
			}
		}

		internal override void SetParentForLoad(XmlNode node)
		{
			this.SetParent(node);
		}

		internal override XmlLinkedNode LastNode
		{
			get
			{
				return this.lastChild;
			}
			set
			{
				this.lastChild = value;
			}
		}

		internal override bool IsValidChildType(XmlNodeType type)
		{
			switch (type)
			{
			case XmlNodeType.Element:
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
			case XmlNodeType.EntityReference:
			case XmlNodeType.ProcessingInstruction:
			case XmlNodeType.Comment:
			case XmlNodeType.Whitespace:
			case XmlNodeType.SignificantWhitespace:
				return true;
			}
			return false;
		}

		/// <summary>Saves the node to the specified <see cref="T:System.Xml.XmlWriter" />.</summary>
		/// <param name="w">The <see langword="XmlWriter" /> to which you want to save. </param>
		public override void WriteTo(XmlWriter w)
		{
			w.WriteEntityRef(this.name);
		}

		/// <summary>Saves all the children of the node to the specified <see cref="T:System.Xml.XmlWriter" />.</summary>
		/// <param name="w">The <see langword="XmlWriter" /> to which you want to save. </param>
		public override void WriteContentTo(XmlWriter w)
		{
			foreach (object obj in this)
			{
				((XmlNode)obj).WriteTo(w);
			}
		}

		/// <summary>Gets the base Uniform Resource Identifier (URI) of the current node.</summary>
		/// <returns>The location from which the node was loaded.</returns>
		public override string BaseURI
		{
			get
			{
				return this.OwnerDocument.BaseURI;
			}
		}

		private string ConstructBaseURI(string baseURI, string systemId)
		{
			if (baseURI == null)
			{
				return systemId;
			}
			int num = baseURI.LastIndexOf('/') + 1;
			string str = baseURI;
			if (num > 0 && num < baseURI.Length)
			{
				str = baseURI.Substring(0, num);
			}
			else if (num == 0)
			{
				str += "\\";
			}
			return str + systemId.Replace('\\', '/');
		}

		internal string ChildBaseURI
		{
			get
			{
				XmlEntity entityNode = this.OwnerDocument.GetEntityNode(this.name);
				if (entityNode == null)
				{
					return string.Empty;
				}
				if (entityNode.SystemId != null && entityNode.SystemId.Length > 0)
				{
					return this.ConstructBaseURI(entityNode.BaseURI, entityNode.SystemId);
				}
				return entityNode.BaseURI;
			}
		}

		private string name;

		private XmlLinkedNode lastChild;
	}
}
