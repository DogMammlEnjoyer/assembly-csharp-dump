using System;
using System.Threading;
using Unity;

namespace System.Xml.Linq
{
	/// <summary>Represents an XML namespace. This class cannot be inherited.</summary>
	public sealed class XNamespace
	{
		internal XNamespace(string namespaceName)
		{
			this._namespaceName = namespaceName;
			this._hashCode = namespaceName.GetHashCode();
			this._names = new XHashtable<XName>(new XHashtable<XName>.ExtractKeyDelegate(XNamespace.ExtractLocalName), 8);
		}

		/// <summary>Gets the Uniform Resource Identifier (URI) of this namespace.</summary>
		/// <returns>A <see cref="T:System.String" /> that contains the URI of the namespace.</returns>
		public string NamespaceName
		{
			get
			{
				return this._namespaceName;
			}
		}

		/// <summary>Returns an <see cref="T:System.Xml.Linq.XName" /> object created from this <see cref="T:System.Xml.Linq.XNamespace" /> and the specified local name.</summary>
		/// <param name="localName">A <see cref="T:System.String" /> that contains a local name.</param>
		/// <returns>An <see cref="T:System.Xml.Linq.XName" /> created from this <see cref="T:System.Xml.Linq.XNamespace" /> and the specified local name.</returns>
		public XName GetName(string localName)
		{
			if (localName == null)
			{
				throw new ArgumentNullException("localName");
			}
			return this.GetName(localName, 0, localName.Length);
		}

		/// <summary>Returns the URI of this <see cref="T:System.Xml.Linq.XNamespace" />.</summary>
		/// <returns>The URI of this <see cref="T:System.Xml.Linq.XNamespace" />.</returns>
		public override string ToString()
		{
			return this._namespaceName;
		}

		/// <summary>Gets the <see cref="T:System.Xml.Linq.XNamespace" /> object that corresponds to no namespace.</summary>
		/// <returns>The <see cref="T:System.Xml.Linq.XNamespace" /> that corresponds to no namespace.</returns>
		public static XNamespace None
		{
			get
			{
				return XNamespace.EnsureNamespace(ref XNamespace.s_refNone, string.Empty);
			}
		}

		/// <summary>Gets the <see cref="T:System.Xml.Linq.XNamespace" /> object that corresponds to the XML URI (http://www.w3.org/XML/1998/namespace).</summary>
		/// <returns>The <see cref="T:System.Xml.Linq.XNamespace" /> that corresponds to the XML URI (http://www.w3.org/XML/1998/namespace).</returns>
		public static XNamespace Xml
		{
			get
			{
				return XNamespace.EnsureNamespace(ref XNamespace.s_refXml, "http://www.w3.org/XML/1998/namespace");
			}
		}

		/// <summary>Gets the <see cref="T:System.Xml.Linq.XNamespace" /> object that corresponds to the xmlns URI (http://www.w3.org/2000/xmlns/).</summary>
		/// <returns>The <see cref="T:System.Xml.Linq.XNamespace" /> that corresponds to the xmlns URI (http://www.w3.org/2000/xmlns/).</returns>
		public static XNamespace Xmlns
		{
			get
			{
				return XNamespace.EnsureNamespace(ref XNamespace.s_refXmlns, "http://www.w3.org/2000/xmlns/");
			}
		}

		/// <summary>Gets an <see cref="T:System.Xml.Linq.XNamespace" /> for the specified Uniform Resource Identifier (URI).</summary>
		/// <param name="namespaceName">A <see cref="T:System.String" /> that contains a namespace URI.</param>
		/// <returns>An <see cref="T:System.Xml.Linq.XNamespace" /> created from the specified URI.</returns>
		public static XNamespace Get(string namespaceName)
		{
			if (namespaceName == null)
			{
				throw new ArgumentNullException("namespaceName");
			}
			return XNamespace.Get(namespaceName, 0, namespaceName.Length);
		}

		/// <summary>Converts a string containing a Uniform Resource Identifier (URI) to an <see cref="T:System.Xml.Linq.XNamespace" />.</summary>
		/// <param name="namespaceName">A <see cref="T:System.String" /> that contains the namespace URI.</param>
		/// <returns>An <see cref="T:System.Xml.Linq.XNamespace" /> constructed from the URI string.</returns>
		[CLSCompliant(false)]
		public static implicit operator XNamespace(string namespaceName)
		{
			if (namespaceName == null)
			{
				return null;
			}
			return XNamespace.Get(namespaceName);
		}

		/// <summary>Combines an <see cref="T:System.Xml.Linq.XNamespace" /> object with a local name to create an <see cref="T:System.Xml.Linq.XName" />.</summary>
		/// <param name="ns">An <see cref="T:System.Xml.Linq.XNamespace" /> that contains the namespace.</param>
		/// <param name="localName">A <see cref="T:System.String" /> that contains the local name.</param>
		/// <returns>The new <see cref="T:System.Xml.Linq.XName" /> constructed from the namespace and local name.</returns>
		public static XName operator +(XNamespace ns, string localName)
		{
			if (ns == null)
			{
				throw new ArgumentNullException("ns");
			}
			return ns.GetName(localName);
		}

		/// <summary>Determines whether the specified <see cref="T:System.Xml.Linq.XNamespace" /> is equal to the current <see cref="T:System.Xml.Linq.XNamespace" />.</summary>
		/// <param name="obj">The <see cref="T:System.Xml.Linq.XNamespace" /> to compare to the current <see cref="T:System.Xml.Linq.XNamespace" />.</param>
		/// <returns>A <see cref="T:System.Boolean" /> that indicates whether the specified <see cref="T:System.Xml.Linq.XNamespace" /> is equal to the current <see cref="T:System.Xml.Linq.XNamespace" />.</returns>
		public override bool Equals(object obj)
		{
			return this == obj;
		}

		/// <summary>Gets a hash code for this <see cref="T:System.Xml.Linq.XNamespace" />.</summary>
		/// <returns>An <see cref="T:System.Int32" /> that contains the hash code for the <see cref="T:System.Xml.Linq.XNamespace" />.</returns>
		public override int GetHashCode()
		{
			return this._hashCode;
		}

		/// <summary>Returns a value indicating whether two instances of <see cref="T:System.Xml.Linq.XNamespace" /> are equal.</summary>
		/// <param name="left">The first <see cref="T:System.Xml.Linq.XNamespace" /> to compare.</param>
		/// <param name="right">The second <see cref="T:System.Xml.Linq.XNamespace" /> to compare.</param>
		/// <returns>A <see cref="T:System.Boolean" /> that indicates whether <paramref name="left" /> and <paramref name="right" /> are equal.</returns>
		public static bool operator ==(XNamespace left, XNamespace right)
		{
			return left == right;
		}

		/// <summary>Returns a value indicating whether two instances of <see cref="T:System.Xml.Linq.XNamespace" /> are not equal.</summary>
		/// <param name="left">The first <see cref="T:System.Xml.Linq.XNamespace" /> to compare.</param>
		/// <param name="right">The second <see cref="T:System.Xml.Linq.XNamespace" /> to compare.</param>
		/// <returns>A <see cref="T:System.Boolean" /> that indicates whether <paramref name="left" /> and <paramref name="right" /> are not equal.</returns>
		public static bool operator !=(XNamespace left, XNamespace right)
		{
			return left != right;
		}

		internal XName GetName(string localName, int index, int count)
		{
			XName result;
			if (this._names.TryGetValue(localName, index, count, out result))
			{
				return result;
			}
			return this._names.Add(new XName(this, localName.Substring(index, count)));
		}

		internal static XNamespace Get(string namespaceName, int index, int count)
		{
			if (count == 0)
			{
				return XNamespace.None;
			}
			if (XNamespace.s_namespaces == null)
			{
				Interlocked.CompareExchange<XHashtable<WeakReference>>(ref XNamespace.s_namespaces, new XHashtable<WeakReference>(new XHashtable<WeakReference>.ExtractKeyDelegate(XNamespace.ExtractNamespace), 32), null);
			}
			for (;;)
			{
				WeakReference weakReference;
				if (!XNamespace.s_namespaces.TryGetValue(namespaceName, index, count, out weakReference))
				{
					if (count == "http://www.w3.org/XML/1998/namespace".Length && string.CompareOrdinal(namespaceName, index, "http://www.w3.org/XML/1998/namespace", 0, count) == 0)
					{
						break;
					}
					if (count == "http://www.w3.org/2000/xmlns/".Length && string.CompareOrdinal(namespaceName, index, "http://www.w3.org/2000/xmlns/", 0, count) == 0)
					{
						goto Block_7;
					}
					weakReference = XNamespace.s_namespaces.Add(new WeakReference(new XNamespace(namespaceName.Substring(index, count))));
				}
				XNamespace xnamespace = (weakReference != null) ? ((XNamespace)weakReference.Target) : null;
				if (!(xnamespace == null))
				{
					return xnamespace;
				}
			}
			return XNamespace.Xml;
			Block_7:
			return XNamespace.Xmlns;
		}

		private static string ExtractLocalName(XName n)
		{
			return n.LocalName;
		}

		private static string ExtractNamespace(WeakReference r)
		{
			XNamespace xnamespace;
			if (r == null || (xnamespace = (XNamespace)r.Target) == null)
			{
				return null;
			}
			return xnamespace.NamespaceName;
		}

		private static XNamespace EnsureNamespace(ref WeakReference refNmsp, string namespaceName)
		{
			XNamespace xnamespace;
			for (;;)
			{
				WeakReference weakReference = refNmsp;
				if (weakReference != null)
				{
					xnamespace = (XNamespace)weakReference.Target;
					if (xnamespace != null)
					{
						break;
					}
				}
				Interlocked.CompareExchange<WeakReference>(ref refNmsp, new WeakReference(new XNamespace(namespaceName)), weakReference);
			}
			return xnamespace;
		}

		internal XNamespace()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		internal const string xmlPrefixNamespace = "http://www.w3.org/XML/1998/namespace";

		internal const string xmlnsPrefixNamespace = "http://www.w3.org/2000/xmlns/";

		private static XHashtable<WeakReference> s_namespaces;

		private static WeakReference s_refNone;

		private static WeakReference s_refXml;

		private static WeakReference s_refXmlns;

		private string _namespaceName;

		private int _hashCode;

		private XHashtable<XName> _names;

		private const int NamesCapacity = 8;

		private const int NamespacesCapacity = 32;
	}
}
