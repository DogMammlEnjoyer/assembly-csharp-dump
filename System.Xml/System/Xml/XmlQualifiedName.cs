using System;
using System.Reflection;
using System.Security;
using System.Security.Permissions;

namespace System.Xml
{
	/// <summary>Represents an XML qualified name.</summary>
	[Serializable]
	public class XmlQualifiedName
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.XmlQualifiedName" /> class.</summary>
		public XmlQualifiedName() : this(string.Empty, string.Empty)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.XmlQualifiedName" /> class with the specified name.</summary>
		/// <param name="name">The local name to use as the name of the <see cref="T:System.Xml.XmlQualifiedName" /> object. </param>
		public XmlQualifiedName(string name) : this(name, string.Empty)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.XmlQualifiedName" /> class with the specified name and namespace.</summary>
		/// <param name="name">The local name to use as the name of the <see cref="T:System.Xml.XmlQualifiedName" /> object. </param>
		/// <param name="ns">The namespace for the <see cref="T:System.Xml.XmlQualifiedName" /> object. </param>
		public XmlQualifiedName(string name, string ns)
		{
			this.ns = ((ns == null) ? string.Empty : ns);
			this.name = ((name == null) ? string.Empty : name);
		}

		/// <summary>Gets a string representation of the namespace of the <see cref="T:System.Xml.XmlQualifiedName" />.</summary>
		/// <returns>A string representation of the namespace or String.Empty if a namespace is not defined for the object.</returns>
		public string Namespace
		{
			get
			{
				return this.ns;
			}
		}

		/// <summary>Gets a string representation of the qualified name of the <see cref="T:System.Xml.XmlQualifiedName" />.</summary>
		/// <returns>A string representation of the qualified name or String.Empty if a name is not defined for the object.</returns>
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>Returns the hash code for the <see cref="T:System.Xml.XmlQualifiedName" />.</summary>
		/// <returns>A hash code for this object.</returns>
		public override int GetHashCode()
		{
			if (this.hash == 0)
			{
				if (XmlQualifiedName.hashCodeDelegate == null)
				{
					XmlQualifiedName.hashCodeDelegate = XmlQualifiedName.GetHashCodeDelegate();
				}
				this.hash = XmlQualifiedName.hashCodeDelegate(this.Name, this.Name.Length, 0L);
			}
			return this.hash;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Xml.XmlQualifiedName" /> is empty.</summary>
		/// <returns>
		///     <see langword="true" /> if name and namespace are empty strings; otherwise, <see langword="false" />.</returns>
		public bool IsEmpty
		{
			get
			{
				return this.Name.Length == 0 && this.Namespace.Length == 0;
			}
		}

		/// <summary>Returns the string value of the <see cref="T:System.Xml.XmlQualifiedName" />.</summary>
		/// <returns>The string value of the <see cref="T:System.Xml.XmlQualifiedName" /> in the format of <see langword="namespace:localname" />. If the object does not have a namespace defined, this method returns just the local name.</returns>
		public override string ToString()
		{
			if (this.Namespace.Length != 0)
			{
				return this.Namespace + ":" + this.Name;
			}
			return this.Name;
		}

		/// <summary>Determines whether the specified <see cref="T:System.Xml.XmlQualifiedName" /> object is equal to the current <see cref="T:System.Xml.XmlQualifiedName" /> object. </summary>
		/// <param name="other">The <see cref="T:System.Xml.XmlQualifiedName" /> to compare. </param>
		/// <returns>
		///     <see langword="true" /> if the two are the same instance object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object other)
		{
			if (this == other)
			{
				return true;
			}
			XmlQualifiedName xmlQualifiedName = other as XmlQualifiedName;
			return xmlQualifiedName != null && this.Name == xmlQualifiedName.Name && this.Namespace == xmlQualifiedName.Namespace;
		}

		/// <summary>Compares two <see cref="T:System.Xml.XmlQualifiedName" /> objects.</summary>
		/// <param name="a">An <see cref="T:System.Xml.XmlQualifiedName" /> to compare. </param>
		/// <param name="b">An <see cref="T:System.Xml.XmlQualifiedName" /> to compare. </param>
		/// <returns>
		///     <see langword="true" /> if the two objects have the same name and namespace values; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(XmlQualifiedName a, XmlQualifiedName b)
		{
			return a == b || (a != null && b != null && a.Name == b.Name && a.Namespace == b.Namespace);
		}

		/// <summary>Compares two <see cref="T:System.Xml.XmlQualifiedName" /> objects.</summary>
		/// <param name="a">An <see cref="T:System.Xml.XmlQualifiedName" /> to compare. </param>
		/// <param name="b">An <see cref="T:System.Xml.XmlQualifiedName" /> to compare. </param>
		/// <returns>
		///     <see langword="true" /> if the name and namespace values for the two objects differ; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(XmlQualifiedName a, XmlQualifiedName b)
		{
			return !(a == b);
		}

		/// <summary>Returns the string value of the <see cref="T:System.Xml.XmlQualifiedName" />.</summary>
		/// <param name="name">The name of the object. </param>
		/// <param name="ns">The namespace of the object. </param>
		/// <returns>The string value of the <see cref="T:System.Xml.XmlQualifiedName" /> in the format of <see langword="namespace:localname" />. If the object does not have a namespace defined, this method returns just the local name.</returns>
		public static string ToString(string name, string ns)
		{
			if (ns != null && ns.Length != 0)
			{
				return ns + ":" + name;
			}
			return name;
		}

		[SecuritySafeCritical]
		[ReflectionPermission(SecurityAction.Assert, Unrestricted = true)]
		private static XmlQualifiedName.HashCodeOfStringDelegate GetHashCodeDelegate()
		{
			if (!XmlQualifiedName.IsRandomizedHashingDisabled())
			{
				MethodInfo method = typeof(string).GetMethod("InternalMarvin32HashString", BindingFlags.Static | BindingFlags.NonPublic);
				if (method != null)
				{
					return (XmlQualifiedName.HashCodeOfStringDelegate)Delegate.CreateDelegate(typeof(XmlQualifiedName.HashCodeOfStringDelegate), method);
				}
			}
			return new XmlQualifiedName.HashCodeOfStringDelegate(XmlQualifiedName.GetHashCodeOfString);
		}

		private static bool IsRandomizedHashingDisabled()
		{
			return false;
		}

		private static int GetHashCodeOfString(string s, int length, long additionalEntropy)
		{
			return s.GetHashCode();
		}

		internal void Init(string name, string ns)
		{
			this.name = name;
			this.ns = ns;
			this.hash = 0;
		}

		internal void SetNamespace(string ns)
		{
			this.ns = ns;
		}

		internal void Verify()
		{
			XmlConvert.VerifyNCName(this.name);
			if (this.ns.Length != 0)
			{
				XmlConvert.ToUri(this.ns);
			}
		}

		internal void Atomize(XmlNameTable nameTable)
		{
			this.name = nameTable.Add(this.name);
			this.ns = nameTable.Add(this.ns);
		}

		internal static XmlQualifiedName Parse(string s, IXmlNamespaceResolver nsmgr, out string prefix)
		{
			string text;
			ValidateNames.ParseQNameThrow(s, out prefix, out text);
			string text2 = nsmgr.LookupNamespace(prefix);
			if (text2 == null)
			{
				if (prefix.Length != 0)
				{
					throw new XmlException("'{0}' is an undeclared prefix.", prefix);
				}
				text2 = string.Empty;
			}
			return new XmlQualifiedName(text, text2);
		}

		internal XmlQualifiedName Clone()
		{
			return (XmlQualifiedName)base.MemberwiseClone();
		}

		internal static int Compare(XmlQualifiedName a, XmlQualifiedName b)
		{
			if (null == a)
			{
				if (!(null == b))
				{
					return -1;
				}
				return 0;
			}
			else
			{
				if (null == b)
				{
					return 1;
				}
				int num = string.CompareOrdinal(a.Namespace, b.Namespace);
				if (num == 0)
				{
					num = string.CompareOrdinal(a.Name, b.Name);
				}
				return num;
			}
		}

		private static XmlQualifiedName.HashCodeOfStringDelegate hashCodeDelegate = null;

		private string name;

		private string ns;

		[NonSerialized]
		private int hash;

		/// <summary>Provides an empty <see cref="T:System.Xml.XmlQualifiedName" />.</summary>
		public static readonly XmlQualifiedName Empty = new XmlQualifiedName(string.Empty);

		private delegate int HashCodeOfStringDelegate(string s, int sLen, long additionalEntropy);
	}
}
