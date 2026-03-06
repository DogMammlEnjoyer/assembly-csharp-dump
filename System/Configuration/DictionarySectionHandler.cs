using System;
using System.Collections;
using System.Xml;

namespace System.Configuration
{
	/// <summary>Provides key/value pair configuration information from a configuration section.</summary>
	public class DictionarySectionHandler : IConfigurationSectionHandler
	{
		/// <summary>Creates a new configuration handler and adds it to the section-handler collection based on the specified parameters.</summary>
		/// <param name="parent">Parent object.</param>
		/// <param name="context">Configuration context object.</param>
		/// <param name="section">Section XML node.</param>
		/// <returns>A configuration object.</returns>
		public virtual object Create(object parent, object context, XmlNode section)
		{
			return ConfigHelper.GetDictionary(parent as IDictionary, section, this.KeyAttributeName, this.ValueAttributeName);
		}

		/// <summary>Gets the XML attribute name to use as the key in a key/value pair.</summary>
		/// <returns>A string value containing the name of the key attribute.</returns>
		protected virtual string KeyAttributeName
		{
			get
			{
				return "key";
			}
		}

		/// <summary>Gets the XML attribute name to use as the value in a key/value pair.</summary>
		/// <returns>A string value containing the name of the value attribute.</returns>
		protected virtual string ValueAttributeName
		{
			get
			{
				return "value";
			}
		}
	}
}
