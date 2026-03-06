using System;
using System.Text;

namespace System.Xml.Linq
{
	/// <summary>Represents an XML declaration.</summary>
	public class XDeclaration
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Linq.XDeclaration" /> class with the specified version, encoding, and standalone status.</summary>
		/// <param name="version">The version of the XML, usually "1.0".</param>
		/// <param name="encoding">The encoding for the XML document.</param>
		/// <param name="standalone">A string containing "yes" or "no" that specifies whether the XML is standalone or requires external entities to be resolved.</param>
		public XDeclaration(string version, string encoding, string standalone)
		{
			this._version = version;
			this._encoding = encoding;
			this._standalone = standalone;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Linq.XDeclaration" /> class from another <see cref="T:System.Xml.Linq.XDeclaration" /> object.</summary>
		/// <param name="other">The <see cref="T:System.Xml.Linq.XDeclaration" /> used to initialize this <see cref="T:System.Xml.Linq.XDeclaration" /> object.</param>
		public XDeclaration(XDeclaration other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			this._version = other._version;
			this._encoding = other._encoding;
			this._standalone = other._standalone;
		}

		internal XDeclaration(XmlReader r)
		{
			this._version = r.GetAttribute("version");
			this._encoding = r.GetAttribute("encoding");
			this._standalone = r.GetAttribute("standalone");
			r.Read();
		}

		/// <summary>Gets or sets the encoding for this document.</summary>
		/// <returns>A <see cref="T:System.String" /> containing the code page name for this document.</returns>
		public string Encoding
		{
			get
			{
				return this._encoding;
			}
			set
			{
				this._encoding = value;
			}
		}

		/// <summary>Gets or sets the standalone property for this document.</summary>
		/// <returns>A <see cref="T:System.String" /> containing the standalone property for this document.</returns>
		public string Standalone
		{
			get
			{
				return this._standalone;
			}
			set
			{
				this._standalone = value;
			}
		}

		/// <summary>Gets or sets the version property for this document.</summary>
		/// <returns>A <see cref="T:System.String" /> containing the version property for this document.</returns>
		public string Version
		{
			get
			{
				return this._version;
			}
			set
			{
				this._version = value;
			}
		}

		/// <summary>Provides the declaration as a formatted string.</summary>
		/// <returns>A <see cref="T:System.String" /> that contains the formatted XML string.</returns>
		public override string ToString()
		{
			StringBuilder stringBuilder = StringBuilderCache.Acquire(16);
			stringBuilder.Append("<?xml");
			if (this._version != null)
			{
				stringBuilder.Append(" version=\"");
				stringBuilder.Append(this._version);
				stringBuilder.Append('"');
			}
			if (this._encoding != null)
			{
				stringBuilder.Append(" encoding=\"");
				stringBuilder.Append(this._encoding);
				stringBuilder.Append('"');
			}
			if (this._standalone != null)
			{
				stringBuilder.Append(" standalone=\"");
				stringBuilder.Append(this._standalone);
				stringBuilder.Append('"');
			}
			stringBuilder.Append("?>");
			return StringBuilderCache.GetStringAndRelease(stringBuilder);
		}

		private string _version;

		private string _encoding;

		private string _standalone;
	}
}
