using System;
using System.ComponentModel;
using System.Configuration;

namespace System.Xml.XmlConfiguration
{
	/// <summary>Represents an XML reader section.</summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class XmlReaderSection : ConfigurationSection
	{
		/// <summary>Gets or sets the string that represents the prohibit default resolver.</summary>
		/// <returns>A <see cref="T:System.String" /> that represents the prohibit default resolver.</returns>
		[ConfigurationProperty("prohibitDefaultResolver", DefaultValue = "false")]
		public string ProhibitDefaultResolverString
		{
			get
			{
				return (string)base["prohibitDefaultResolver"];
			}
			set
			{
				base["prohibitDefaultResolver"] = value;
			}
		}

		private bool _ProhibitDefaultResolver
		{
			get
			{
				bool result;
				XmlConvert.TryToBoolean(this.ProhibitDefaultResolverString, out result);
				return result;
			}
		}

		internal static bool ProhibitDefaultUrlResolver
		{
			get
			{
				XmlReaderSection xmlReaderSection = ConfigurationManager.GetSection(XmlConfigurationString.XmlReaderSectionPath) as XmlReaderSection;
				return xmlReaderSection != null && xmlReaderSection._ProhibitDefaultResolver;
			}
		}

		internal static XmlResolver CreateDefaultResolver()
		{
			if (XmlReaderSection.ProhibitDefaultUrlResolver)
			{
				return null;
			}
			return new XmlUrlResolver();
		}

		/// <summary>Gets or sets the string that represents a boolean value indicating whether white spaces are collapsed into empty strings. The default value is "false".</summary>
		/// <returns>A string that represents a boolean value indicating whether white spaces are collapsed into empty strings.</returns>
		[ConfigurationProperty("CollapseWhiteSpaceIntoEmptyString", DefaultValue = "false")]
		public string CollapseWhiteSpaceIntoEmptyStringString
		{
			get
			{
				return (string)base["CollapseWhiteSpaceIntoEmptyString"];
			}
			set
			{
				base["CollapseWhiteSpaceIntoEmptyString"] = value;
			}
		}

		private bool _CollapseWhiteSpaceIntoEmptyString
		{
			get
			{
				bool result;
				XmlConvert.TryToBoolean(this.CollapseWhiteSpaceIntoEmptyStringString, out result);
				return result;
			}
		}

		internal static bool CollapseWhiteSpaceIntoEmptyString
		{
			get
			{
				XmlReaderSection xmlReaderSection = ConfigurationManager.GetSection(XmlConfigurationString.XmlReaderSectionPath) as XmlReaderSection;
				return xmlReaderSection != null && xmlReaderSection._CollapseWhiteSpaceIntoEmptyString;
			}
		}
	}
}
