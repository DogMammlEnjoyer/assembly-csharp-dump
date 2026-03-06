using System;
using System.CodeDom.Compiler;

namespace System.Xml.Xsl
{
	/// <summary>Specifies the XSLT features to support during execution of the XSLT style sheet.</summary>
	public sealed class XsltSettings
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Xsl.XsltSettings" /> class with default settings.</summary>
		public XsltSettings()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Xsl.XsltSettings" /> class with the specified settings.</summary>
		/// <param name="enableDocumentFunction">
		///       <see langword="true" /> to enable support for the XSLT document() function; otherwise, <see langword="false" />.</param>
		/// <param name="enableScript">
		///       <see langword="true" /> to enable support for embedded scripts blocks; otherwise, <see langword="false" />.</param>
		public XsltSettings(bool enableDocumentFunction, bool enableScript)
		{
			this.enableDocumentFunction = enableDocumentFunction;
			this.enableScript = enableScript;
		}

		/// <summary>Gets an <see cref="T:System.Xml.Xsl.XsltSettings" /> object with default settings. Support for the XSLT document() function and embedded script blocks is disabled.</summary>
		/// <returns>An <see cref="T:System.Xml.Xsl.XsltSettings" /> object with the <see cref="P:System.Xml.Xsl.XsltSettings.EnableDocumentFunction" /> and <see cref="P:System.Xml.Xsl.XsltSettings.EnableScript" /> properties set to <see langword="false" />.</returns>
		public static XsltSettings Default
		{
			get
			{
				return new XsltSettings(false, false);
			}
		}

		/// <summary>Gets an <see cref="T:System.Xml.Xsl.XsltSettings" /> object that enables support for the XSLT document() function and embedded script blocks.</summary>
		/// <returns>An <see cref="T:System.Xml.Xsl.XsltSettings" /> object with the <see cref="P:System.Xml.Xsl.XsltSettings.EnableDocumentFunction" /> and <see cref="P:System.Xml.Xsl.XsltSettings.EnableScript" /> properties set to <see langword="true" />.</returns>
		public static XsltSettings TrustedXslt
		{
			get
			{
				return new XsltSettings(true, true);
			}
		}

		/// <summary>Gets or sets a value indicating whether to enable support for the XSLT document() function.</summary>
		/// <returns>
		///     <see langword="true" /> to support the XSLT document() function; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool EnableDocumentFunction
		{
			get
			{
				return this.enableDocumentFunction;
			}
			set
			{
				this.enableDocumentFunction = value;
			}
		}

		/// <summary>Gets or sets a value indicating whether to enable support for embedded script blocks.</summary>
		/// <returns>
		///     <see langword="true" /> to support script blocks in XSLT style sheets; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool EnableScript
		{
			get
			{
				return this.enableScript;
			}
			set
			{
				this.enableScript = value;
			}
		}

		internal bool CheckOnly
		{
			get
			{
				return this.checkOnly;
			}
			set
			{
				this.checkOnly = value;
			}
		}

		internal bool IncludeDebugInformation
		{
			get
			{
				return this.includeDebugInformation;
			}
			set
			{
				this.includeDebugInformation = value;
			}
		}

		internal int WarningLevel
		{
			get
			{
				return this.warningLevel;
			}
			set
			{
				this.warningLevel = value;
			}
		}

		internal bool TreatWarningsAsErrors
		{
			get
			{
				return this.treatWarningsAsErrors;
			}
			set
			{
				this.treatWarningsAsErrors = value;
			}
		}

		internal TempFileCollection TempFiles
		{
			get
			{
				return this.tempFiles;
			}
			set
			{
				this.tempFiles = value;
			}
		}

		private bool enableDocumentFunction;

		private bool enableScript;

		private bool checkOnly;

		private bool includeDebugInformation;

		private int warningLevel = -1;

		private bool treatWarningsAsErrors;

		private TempFileCollection tempFiles;
	}
}
