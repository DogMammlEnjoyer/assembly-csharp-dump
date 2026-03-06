using System;
using Unity;

namespace System.Xml
{
	/// <summary>Represents an entity declaration, such as &lt;!ENTITY... &gt;.</summary>
	public class XmlEntity : XmlNode
	{
		internal XmlEntity(string name, string strdata, string publicId, string systemId, string notationName, XmlDocument doc) : base(doc)
		{
			this.name = doc.NameTable.Add(name);
			this.publicId = publicId;
			this.systemId = systemId;
			this.notationName = notationName;
			this.unparsedReplacementStr = strdata;
			this.childrenFoliating = false;
		}

		/// <summary>Creates a duplicate of this node. Entity nodes cannot be cloned. Calling this method on an <see cref="T:System.Xml.XmlEntity" /> object throws an exception.</summary>
		/// <param name="deep">
		///       <see langword="true" /> to recursively clone the subtree under the specified node; <see langword="false" /> to clone only the node itself.</param>
		/// <returns>Returns a copy of the <see cref="T:System.Xml.XmlNode" /> from which the method is called.</returns>
		/// <exception cref="T:System.InvalidOperationException">Entity nodes cannot be cloned. Calling this method on an <see cref="T:System.Xml.XmlEntity" /> object throws an exception.</exception>
		public override XmlNode CloneNode(bool deep)
		{
			throw new InvalidOperationException(Res.GetString("'Entity' and 'Notation' nodes cannot be cloned."));
		}

		/// <summary>Gets a value indicating whether the node is read-only.</summary>
		/// <returns>
		///     <see langword="true" /> if the node is read-only; otherwise <see langword="false" />.Because <see langword="XmlEntity" /> nodes are read-only, this property always returns <see langword="true" />.</returns>
		public override bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		/// <summary>Gets the name of the node.</summary>
		/// <returns>The name of the entity.</returns>
		public override string Name
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>Gets the name of the node without the namespace prefix.</summary>
		/// <returns>For <see langword="XmlEntity" /> nodes, this property returns the name of the entity.</returns>
		public override string LocalName
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>Gets the concatenated values of the entity node and all its children.</summary>
		/// <returns>The concatenated values of the node and all its children.</returns>
		/// <exception cref="T:System.InvalidOperationException">Attempting to set the property. </exception>
		public override string InnerText
		{
			get
			{
				return base.InnerText;
			}
			set
			{
				throw new InvalidOperationException(Res.GetString("The 'InnerText' of an 'Entity' node is read-only and cannot be set."));
			}
		}

		internal override bool IsContainer
		{
			get
			{
				return true;
			}
		}

		internal override XmlLinkedNode LastNode
		{
			get
			{
				if (this.lastChild == null && !this.childrenFoliating)
				{
					this.childrenFoliating = true;
					new XmlLoader().ExpandEntity(this);
				}
				return this.lastChild;
			}
			set
			{
				this.lastChild = value;
			}
		}

		internal override bool IsValidChildType(XmlNodeType type)
		{
			return type == XmlNodeType.Text || type == XmlNodeType.Element || type == XmlNodeType.ProcessingInstruction || type == XmlNodeType.Comment || type == XmlNodeType.CDATA || type == XmlNodeType.Whitespace || type == XmlNodeType.SignificantWhitespace || type == XmlNodeType.EntityReference;
		}

		/// <summary>Gets the type of the node.</summary>
		/// <returns>The node type. For <see langword="XmlEntity" /> nodes, the value is XmlNodeType.Entity.</returns>
		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.Entity;
			}
		}

		/// <summary>Gets the value of the public identifier on the entity declaration.</summary>
		/// <returns>The public identifier on the entity. If there is no public identifier, <see langword="null" /> is returned.</returns>
		public string PublicId
		{
			get
			{
				return this.publicId;
			}
		}

		/// <summary>Gets the value of the system identifier on the entity declaration.</summary>
		/// <returns>The system identifier on the entity. If there is no system identifier, <see langword="null" /> is returned.</returns>
		public string SystemId
		{
			get
			{
				return this.systemId;
			}
		}

		/// <summary>Gets the name of the optional NDATA attribute on the entity declaration.</summary>
		/// <returns>The name of the NDATA attribute. If there is no NDATA, <see langword="null" /> is returned.</returns>
		public string NotationName
		{
			get
			{
				return this.notationName;
			}
		}

		/// <summary>Gets the markup representing this node and all its children.</summary>
		/// <returns>For <see langword="XmlEntity" /> nodes, String.Empty is returned.</returns>
		public override string OuterXml
		{
			get
			{
				return string.Empty;
			}
		}

		/// <summary>Gets the markup representing the children of this node.</summary>
		/// <returns>For <see langword="XmlEntity" /> nodes, String.Empty is returned.</returns>
		/// <exception cref="T:System.InvalidOperationException">Attempting to set the property. </exception>
		public override string InnerXml
		{
			get
			{
				return string.Empty;
			}
			set
			{
				throw new InvalidOperationException(Res.GetString("Cannot set the 'InnerXml' for the current node because it is either read-only or cannot have children."));
			}
		}

		/// <summary>Saves the node to the specified <see cref="T:System.Xml.XmlWriter" />. For <see langword="XmlEntity" /> nodes, this method has no effect.</summary>
		/// <param name="w">The <see langword="XmlWriter" /> to which you want to save. </param>
		public override void WriteTo(XmlWriter w)
		{
		}

		/// <summary>Saves all the children of the node to the specified <see cref="T:System.Xml.XmlWriter" />. For <see langword="XmlEntity" /> nodes, this method has no effect.</summary>
		/// <param name="w">The <see langword="XmlWriter" /> to which you want to save. </param>
		public override void WriteContentTo(XmlWriter w)
		{
		}

		/// <summary>Gets the base Uniform Resource Identifier (URI) of the current node.</summary>
		/// <returns>The location from which the node was loaded.</returns>
		public override string BaseURI
		{
			get
			{
				return this.baseURI;
			}
		}

		internal void SetBaseURI(string inBaseURI)
		{
			this.baseURI = inBaseURI;
		}

		internal XmlEntity()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private string publicId;

		private string systemId;

		private string notationName;

		private string name;

		private string unparsedReplacementStr;

		private string baseURI;

		private XmlLinkedNode lastChild;

		private bool childrenFoliating;
	}
}
