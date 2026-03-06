using System;
using System.Collections;

namespace System.Xml.Serialization
{
	/// <summary>Contains the XML namespaces and prefixes that the <see cref="T:System.Xml.Serialization.XmlSerializer" /> uses to generate qualified names in an XML-document instance.</summary>
	public class XmlSerializerNamespaces
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Serialization.XmlSerializerNamespaces" /> class.</summary>
		public XmlSerializerNamespaces()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Serialization.XmlSerializerNamespaces" /> class, using the specified instance of <see langword="XmlSerializerNamespaces" /> containing the collection of prefix and namespace pairs.</summary>
		/// <param name="namespaces">An instance of the <see cref="T:System.Xml.Serialization.XmlSerializerNamespaces" />containing the namespace and prefix pairs. </param>
		public XmlSerializerNamespaces(XmlSerializerNamespaces namespaces)
		{
			this.namespaces = (Hashtable)namespaces.Namespaces.Clone();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Serialization.XmlSerializerNamespaces" /> class.</summary>
		/// <param name="namespaces">An array of <see cref="T:System.Xml.XmlQualifiedName" /> objects. </param>
		public XmlSerializerNamespaces(XmlQualifiedName[] namespaces)
		{
			foreach (XmlQualifiedName xmlQualifiedName in namespaces)
			{
				this.Add(xmlQualifiedName.Name, xmlQualifiedName.Namespace);
			}
		}

		/// <summary>Adds a prefix and namespace pair to an <see cref="T:System.Xml.Serialization.XmlSerializerNamespaces" /> object.</summary>
		/// <param name="prefix">The prefix associated with an XML namespace. </param>
		/// <param name="ns">An XML namespace. </param>
		public void Add(string prefix, string ns)
		{
			if (prefix != null && prefix.Length > 0)
			{
				XmlConvert.VerifyNCName(prefix);
			}
			if (ns != null && ns.Length > 0)
			{
				XmlConvert.ToUri(ns);
			}
			this.AddInternal(prefix, ns);
		}

		internal void AddInternal(string prefix, string ns)
		{
			this.Namespaces[prefix] = ns;
		}

		/// <summary>Gets the array of prefix and namespace pairs in an <see cref="T:System.Xml.Serialization.XmlSerializerNamespaces" /> object.</summary>
		/// <returns>An array of <see cref="T:System.Xml.XmlQualifiedName" /> objects that are used as qualified names in an XML document.</returns>
		public XmlQualifiedName[] ToArray()
		{
			if (this.NamespaceList == null)
			{
				return new XmlQualifiedName[0];
			}
			return (XmlQualifiedName[])this.NamespaceList.ToArray(typeof(XmlQualifiedName));
		}

		/// <summary>Gets the number of prefix and namespace pairs in the collection.</summary>
		/// <returns>The number of prefix and namespace pairs in the collection.</returns>
		public int Count
		{
			get
			{
				return this.Namespaces.Count;
			}
		}

		internal ArrayList NamespaceList
		{
			get
			{
				if (this.namespaces == null || this.namespaces.Count == 0)
				{
					return null;
				}
				ArrayList arrayList = new ArrayList();
				foreach (object obj in this.Namespaces.Keys)
				{
					string text = (string)obj;
					arrayList.Add(new XmlQualifiedName(text, (string)this.Namespaces[text]));
				}
				return arrayList;
			}
		}

		internal Hashtable Namespaces
		{
			get
			{
				if (this.namespaces == null)
				{
					this.namespaces = new Hashtable();
				}
				return this.namespaces;
			}
			set
			{
				this.namespaces = value;
			}
		}

		internal string LookupPrefix(string ns)
		{
			if (string.IsNullOrEmpty(ns))
			{
				return null;
			}
			if (this.namespaces == null || this.namespaces.Count == 0)
			{
				return null;
			}
			foreach (object obj in this.namespaces.Keys)
			{
				string text = (string)obj;
				if (!string.IsNullOrEmpty(text) && (string)this.namespaces[text] == ns)
				{
					return text;
				}
			}
			return null;
		}

		private Hashtable namespaces;
	}
}
