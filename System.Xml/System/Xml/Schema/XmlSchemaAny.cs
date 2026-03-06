using System;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	/// <summary>Represents the World Wide Web Consortium (W3C) <see langword="any" /> element.</summary>
	public class XmlSchemaAny : XmlSchemaParticle
	{
		/// <summary>Gets or sets the namespaces containing the elements that can be used.</summary>
		/// <returns>Namespaces for elements that are available for use. The default is <see langword="##any" />.Optional.</returns>
		[XmlAttribute("namespace")]
		public string Namespace
		{
			get
			{
				return this.ns;
			}
			set
			{
				this.ns = value;
			}
		}

		/// <summary>Gets or sets information about how an application or XML processor should handle the validation of XML documents for the elements specified by the <see langword="any" /> element.</summary>
		/// <returns>One of the <see cref="T:System.Xml.Schema.XmlSchemaContentProcessing" /> values. If no <see langword="processContents" /> attribute is specified, the default is <see langword="Strict" />.</returns>
		[DefaultValue(XmlSchemaContentProcessing.None)]
		[XmlAttribute("processContents")]
		public XmlSchemaContentProcessing ProcessContents
		{
			get
			{
				return this.processContents;
			}
			set
			{
				this.processContents = value;
			}
		}

		[XmlIgnore]
		internal NamespaceList NamespaceList
		{
			get
			{
				return this.namespaceList;
			}
		}

		[XmlIgnore]
		internal string ResolvedNamespace
		{
			get
			{
				if (this.ns == null || this.ns.Length == 0)
				{
					return "##any";
				}
				return this.ns;
			}
		}

		[XmlIgnore]
		internal XmlSchemaContentProcessing ProcessContentsCorrect
		{
			get
			{
				if (this.processContents != XmlSchemaContentProcessing.None)
				{
					return this.processContents;
				}
				return XmlSchemaContentProcessing.Strict;
			}
		}

		internal override string NameString
		{
			get
			{
				switch (this.namespaceList.Type)
				{
				case NamespaceList.ListType.Any:
					return "##any:*";
				case NamespaceList.ListType.Other:
					return "##other:*";
				case NamespaceList.ListType.Set:
				{
					StringBuilder stringBuilder = new StringBuilder();
					int num = 1;
					foreach (object obj in this.namespaceList.Enumerate)
					{
						string str = (string)obj;
						stringBuilder.Append(str + ":*");
						if (num < this.namespaceList.Enumerate.Count)
						{
							stringBuilder.Append(" ");
						}
						num++;
					}
					return stringBuilder.ToString();
				}
				default:
					return string.Empty;
				}
			}
		}

		internal void BuildNamespaceList(string targetNamespace)
		{
			if (this.ns != null)
			{
				this.namespaceList = new NamespaceList(this.ns, targetNamespace);
				return;
			}
			this.namespaceList = new NamespaceList();
		}

		internal void BuildNamespaceListV1Compat(string targetNamespace)
		{
			if (this.ns != null)
			{
				this.namespaceList = new NamespaceListV1Compat(this.ns, targetNamespace);
				return;
			}
			this.namespaceList = new NamespaceList();
		}

		internal bool Allows(XmlQualifiedName qname)
		{
			return this.namespaceList.Allows(qname.Namespace);
		}

		private string ns;

		private XmlSchemaContentProcessing processContents;

		private NamespaceList namespaceList;
	}
}
