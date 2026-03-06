using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml;

namespace System.Data.Common
{
	/// <summary>This class can be used by any provider to support a provider-specific configuration section.</summary>
	public class DbProviderConfigurationHandler : IConfigurationSectionHandler
	{
		internal static NameValueCollection CloneParent(NameValueCollection parentConfig)
		{
			if (parentConfig == null)
			{
				parentConfig = new NameValueCollection();
			}
			else
			{
				parentConfig = new NameValueCollection(parentConfig);
			}
			return parentConfig;
		}

		/// <summary>Creates a new <see cref="T:System.Collections.Specialized.NameValueCollection" /> expression.</summary>
		/// <param name="parent">This type supports the .NET Framework infrastructure and is not intended to be used directly from your code.</param>
		/// <param name="configContext">This type supports the .NET Framework infrastructure and is not intended to be used directly from your code.</param>
		/// <param name="section">This type supports the .NET Framework infrastructure and is not intended to be used directly from your code.</param>
		/// <returns>The new expression.</returns>
		public virtual object Create(object parent, object configContext, XmlNode section)
		{
			return DbProviderConfigurationHandler.CreateStatic(parent, configContext, section);
		}

		internal static object CreateStatic(object parent, object configContext, XmlNode section)
		{
			object obj = parent;
			if (section != null)
			{
				obj = DbProviderConfigurationHandler.CloneParent(parent as NameValueCollection);
				bool flag = false;
				HandlerBase.CheckForUnrecognizedAttributes(section);
				foreach (object obj2 in section.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj2;
					if (!HandlerBase.IsIgnorableAlsoCheckForNonElement(xmlNode))
					{
						if (!(xmlNode.Name == "settings"))
						{
							throw ADP.ConfigUnrecognizedElement(xmlNode);
						}
						if (flag)
						{
							throw ADP.ConfigSectionsUnique("settings");
						}
						flag = true;
						DbProviderConfigurationHandler.DbProviderDictionarySectionHandler.CreateStatic(obj as NameValueCollection, configContext, xmlNode);
					}
				}
			}
			return obj;
		}

		internal static string RemoveAttribute(XmlNode node, string name)
		{
			XmlNode xmlNode = node.Attributes.RemoveNamedItem(name);
			if (xmlNode == null)
			{
				throw ADP.ConfigRequiredAttributeMissing(name, node);
			}
			string value = xmlNode.Value;
			if (value.Length == 0)
			{
				throw ADP.ConfigRequiredAttributeEmpty(name, node);
			}
			return value;
		}

		internal const string settings = "settings";

		private sealed class DbProviderDictionarySectionHandler
		{
			internal static NameValueCollection CreateStatic(NameValueCollection config, object context, XmlNode section)
			{
				if (section != null)
				{
					HandlerBase.CheckForUnrecognizedAttributes(section);
				}
				foreach (object obj in section.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					if (!HandlerBase.IsIgnorableAlsoCheckForNonElement(xmlNode))
					{
						string name = xmlNode.Name;
						if (!(name == "add"))
						{
							if (!(name == "remove"))
							{
								if (!(name == "clear"))
								{
									throw ADP.ConfigUnrecognizedElement(xmlNode);
								}
								DbProviderConfigurationHandler.DbProviderDictionarySectionHandler.HandleClear(xmlNode, config);
							}
							else
							{
								DbProviderConfigurationHandler.DbProviderDictionarySectionHandler.HandleRemove(xmlNode, config);
							}
						}
						else
						{
							DbProviderConfigurationHandler.DbProviderDictionarySectionHandler.HandleAdd(xmlNode, config);
						}
					}
				}
				return config;
			}

			private static void HandleAdd(XmlNode child, NameValueCollection config)
			{
				HandlerBase.CheckForChildNodes(child);
				string name = DbProviderConfigurationHandler.RemoveAttribute(child, "name");
				string value = DbProviderConfigurationHandler.RemoveAttribute(child, "value");
				HandlerBase.CheckForUnrecognizedAttributes(child);
				config.Add(name, value);
			}

			private static void HandleRemove(XmlNode child, NameValueCollection config)
			{
				HandlerBase.CheckForChildNodes(child);
				string name = DbProviderConfigurationHandler.RemoveAttribute(child, "name");
				HandlerBase.CheckForUnrecognizedAttributes(child);
				config.Remove(name);
			}

			private static void HandleClear(XmlNode child, NameValueCollection config)
			{
				HandlerBase.CheckForChildNodes(child);
				HandlerBase.CheckForUnrecognizedAttributes(child);
				config.Clear();
			}
		}
	}
}
