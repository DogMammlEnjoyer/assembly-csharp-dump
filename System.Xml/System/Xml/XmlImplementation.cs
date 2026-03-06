using System;

namespace System.Xml
{
	/// <summary>Defines the context for a set of <see cref="T:System.Xml.XmlDocument" /> objects.</summary>
	public class XmlImplementation
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.XmlImplementation" /> class.</summary>
		public XmlImplementation() : this(new NameTable())
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.XmlImplementation" /> class with the <see cref="T:System.Xml.XmlNameTable" /> specified.</summary>
		/// <param name="nt">An <see cref="T:System.Xml.XmlNameTable" /> object.</param>
		public XmlImplementation(XmlNameTable nt)
		{
			this.nameTable = nt;
		}

		/// <summary>Tests if the Document Object Model (DOM) implementation implements a specific feature.</summary>
		/// <param name="strFeature">The package name of the feature to test. This name is not case-sensitive. </param>
		/// <param name="strVersion">This is the version number of the package name to test. If the version is not specified (<see langword="null" />), supporting any version of the feature causes the method to return <see langword="true" />. </param>
		/// <returns>
		///     <see langword="true" /> if the feature is implemented in the specified version; otherwise, <see langword="false" />.The following table shows the combinations that cause <see langword="HasFeature" /> to return <see langword="true" />.strFeature strVersion XML 1.0 XML 2.0 </returns>
		public bool HasFeature(string strFeature, string strVersion)
		{
			return string.Compare("XML", strFeature, StringComparison.OrdinalIgnoreCase) == 0 && (strVersion == null || strVersion == "1.0" || strVersion == "2.0");
		}

		/// <summary>Creates a new <see cref="T:System.Xml.XmlDocument" />.</summary>
		/// <returns>The new <see langword="XmlDocument" /> object.</returns>
		public virtual XmlDocument CreateDocument()
		{
			return new XmlDocument(this);
		}

		internal XmlNameTable NameTable
		{
			get
			{
				return this.nameTable;
			}
		}

		private XmlNameTable nameTable;
	}
}
