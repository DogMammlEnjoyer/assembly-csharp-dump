using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.Xml.Linq
{
	/// <summary>Represents an XML Document Type Definition (DTD).</summary>
	public class XDocumentType : XNode
	{
		/// <summary>Initializes an instance of the <see cref="T:System.Xml.Linq.XDocumentType" /> class.</summary>
		/// <param name="name">A <see cref="T:System.String" /> that contains the qualified name of the DTD, which is the same as the qualified name of the root element of the XML document.</param>
		/// <param name="publicId">A <see cref="T:System.String" /> that contains the public identifier of an external public DTD.</param>
		/// <param name="systemId">A <see cref="T:System.String" /> that contains the system identifier of an external private DTD.</param>
		/// <param name="internalSubset">A <see cref="T:System.String" /> that contains the internal subset for an internal DTD.</param>
		public XDocumentType(string name, string publicId, string systemId, string internalSubset)
		{
			this._name = XmlConvert.VerifyName(name);
			this._publicId = publicId;
			this._systemId = systemId;
			this._internalSubset = internalSubset;
		}

		/// <summary>Initializes an instance of the <see cref="T:System.Xml.Linq.XDocumentType" /> class from another <see cref="T:System.Xml.Linq.XDocumentType" /> object.</summary>
		/// <param name="other">An <see cref="T:System.Xml.Linq.XDocumentType" /> object to copy from.</param>
		public XDocumentType(XDocumentType other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			this._name = other._name;
			this._publicId = other._publicId;
			this._systemId = other._systemId;
			this._internalSubset = other._internalSubset;
		}

		internal XDocumentType(XmlReader r)
		{
			this._name = r.Name;
			this._publicId = r.GetAttribute("PUBLIC");
			this._systemId = r.GetAttribute("SYSTEM");
			this._internalSubset = r.Value;
			r.Read();
		}

		/// <summary>Gets or sets the internal subset for this Document Type Definition (DTD).</summary>
		/// <returns>A <see cref="T:System.String" /> that contains the internal subset for this Document Type Definition (DTD).</returns>
		public string InternalSubset
		{
			get
			{
				return this._internalSubset;
			}
			set
			{
				bool flag = base.NotifyChanging(this, XObjectChangeEventArgs.Value);
				this._internalSubset = value;
				if (flag)
				{
					base.NotifyChanged(this, XObjectChangeEventArgs.Value);
				}
			}
		}

		/// <summary>Gets or sets the name for this Document Type Definition (DTD).</summary>
		/// <returns>A <see cref="T:System.String" /> that contains the name for this Document Type Definition (DTD).</returns>
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				value = XmlConvert.VerifyName(value);
				bool flag = base.NotifyChanging(this, XObjectChangeEventArgs.Name);
				this._name = value;
				if (flag)
				{
					base.NotifyChanged(this, XObjectChangeEventArgs.Name);
				}
			}
		}

		/// <summary>Gets the node type for this node.</summary>
		/// <returns>The node type. For <see cref="T:System.Xml.Linq.XDocumentType" /> objects, this value is <see cref="F:System.Xml.XmlNodeType.DocumentType" />.</returns>
		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.DocumentType;
			}
		}

		/// <summary>Gets or sets the public identifier for this Document Type Definition (DTD).</summary>
		/// <returns>A <see cref="T:System.String" /> that contains the public identifier for this Document Type Definition (DTD).</returns>
		public string PublicId
		{
			get
			{
				return this._publicId;
			}
			set
			{
				bool flag = base.NotifyChanging(this, XObjectChangeEventArgs.Value);
				this._publicId = value;
				if (flag)
				{
					base.NotifyChanged(this, XObjectChangeEventArgs.Value);
				}
			}
		}

		/// <summary>Gets or sets the system identifier for this Document Type Definition (DTD).</summary>
		/// <returns>A <see cref="T:System.String" /> that contains the system identifier for this Document Type Definition (DTD).</returns>
		public string SystemId
		{
			get
			{
				return this._systemId;
			}
			set
			{
				bool flag = base.NotifyChanging(this, XObjectChangeEventArgs.Value);
				this._systemId = value;
				if (flag)
				{
					base.NotifyChanged(this, XObjectChangeEventArgs.Value);
				}
			}
		}

		/// <summary>Write this <see cref="T:System.Xml.Linq.XDocumentType" /> to an <see cref="T:System.Xml.XmlWriter" />.</summary>
		/// <param name="writer">An <see cref="T:System.Xml.XmlWriter" /> into which this method will write.</param>
		public override void WriteTo(XmlWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.WriteDocType(this._name, this._publicId, this._systemId, this._internalSubset);
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
			return writer.WriteDocTypeAsync(this._name, this._publicId, this._systemId, this._internalSubset);
		}

		internal override XNode CloneNode()
		{
			return new XDocumentType(this);
		}

		internal override bool DeepEquals(XNode node)
		{
			XDocumentType xdocumentType = node as XDocumentType;
			return xdocumentType != null && this._name == xdocumentType._name && this._publicId == xdocumentType._publicId && this._systemId == xdocumentType.SystemId && this._internalSubset == xdocumentType._internalSubset;
		}

		internal override int GetDeepHashCode()
		{
			return this._name.GetHashCode() ^ ((this._publicId != null) ? this._publicId.GetHashCode() : 0) ^ ((this._systemId != null) ? this._systemId.GetHashCode() : 0) ^ ((this._internalSubset != null) ? this._internalSubset.GetHashCode() : 0);
		}

		private string _name;

		private string _publicId;

		private string _systemId;

		private string _internalSubset;
	}
}
