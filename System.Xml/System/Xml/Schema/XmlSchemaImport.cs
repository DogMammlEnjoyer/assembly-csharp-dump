using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	/// <summary>Represents the <see langword="import" /> element from XML Schema as specified by the World Wide Web Consortium (W3C). This class is used to import schema components from other schemas.</summary>
	public class XmlSchemaImport : XmlSchemaExternal
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Schema.XmlSchemaImport" /> class.</summary>
		public XmlSchemaImport()
		{
			base.Compositor = Compositor.Import;
		}

		/// <summary>Gets or sets the target namespace for the imported schema as a Uniform Resource Identifier (URI) reference.</summary>
		/// <returns>The target namespace for the imported schema as a URI reference.Optional.</returns>
		[XmlAttribute("namespace", DataType = "anyURI")]
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

		/// <summary>Gets or sets the <see langword="annotation" /> property.</summary>
		/// <returns>The annotation.</returns>
		[XmlElement("annotation", typeof(XmlSchemaAnnotation))]
		public XmlSchemaAnnotation Annotation
		{
			get
			{
				return this.annotation;
			}
			set
			{
				this.annotation = value;
			}
		}

		internal override void AddAnnotation(XmlSchemaAnnotation annotation)
		{
			this.annotation = annotation;
		}

		private string ns;

		private XmlSchemaAnnotation annotation;
	}
}
