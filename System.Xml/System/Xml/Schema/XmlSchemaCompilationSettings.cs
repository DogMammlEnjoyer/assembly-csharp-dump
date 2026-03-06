using System;

namespace System.Xml.Schema
{
	/// <summary>Provides schema compilation options for the <see cref="T:System.Xml.Schema.XmlSchemaSet" /> class This class cannot be inherited.</summary>
	public sealed class XmlSchemaCompilationSettings
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Schema.XmlSchemaCompilationSettings" /> class. </summary>
		public XmlSchemaCompilationSettings()
		{
			this.enableUpaCheck = true;
		}

		/// <summary>Gets or sets a value indicating whether the <see cref="T:System.Xml.Schema.XmlSchemaSet" /> should check for Unique Particle Attribution (UPA) violations.</summary>
		/// <returns>
		///     <see langword="true" /> if the <see cref="T:System.Xml.Schema.XmlSchemaSet" /> should check for Unique Particle Attribution (UPA) violations; otherwise, <see langword="false" />. The default is <see langword="true" />.</returns>
		public bool EnableUpaCheck
		{
			get
			{
				return this.enableUpaCheck;
			}
			set
			{
				this.enableUpaCheck = value;
			}
		}

		private bool enableUpaCheck;
	}
}
