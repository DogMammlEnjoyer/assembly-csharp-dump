using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	public abstract class UxmlAttributeDescription
	{
		protected UxmlAttributeDescription()
		{
			this.use = UxmlAttributeDescription.Use.Optional;
			this.restriction = null;
		}

		public string name { get; set; }

		public IEnumerable<string> obsoleteNames
		{
			get
			{
				return this.m_ObsoleteNames;
			}
			set
			{
				string[] array = value as string[];
				bool flag = array != null;
				if (flag)
				{
					this.m_ObsoleteNames = array;
				}
				else
				{
					this.m_ObsoleteNames = value.ToArray<string>();
				}
			}
		}

		public string type { get; protected internal set; }

		public string typeNamespace { get; protected set; }

		public abstract string defaultValueAsString { get; }

		public UxmlAttributeDescription.Use use { get; set; }

		public UxmlTypeRestriction restriction { get; set; }

		internal bool TryFindValueInAttributeOverrides(string elementName, CreationContext cc, List<TemplateAsset.AttributeOverride> attributeOverrides, out string value)
		{
			value = null;
			TemplateAsset.AttributeOverride attributeOverride = default(TemplateAsset.AttributeOverride);
			foreach (TemplateAsset.AttributeOverride attributeOverride2 in attributeOverrides)
			{
				bool flag = cc.namesPath == null;
				if (flag)
				{
					bool flag2 = attributeOverride2.m_ElementName != elementName;
					if (flag2)
					{
						continue;
					}
				}
				else
				{
					bool flag3 = !attributeOverride2.NamesPathMatchesElementNamesPath(cc.namesPath);
					if (flag3)
					{
						continue;
					}
				}
				bool flag4 = attributeOverride2.m_AttributeName != this.name;
				if (flag4)
				{
					bool flag5 = this.m_ObsoleteNames != null;
					if (!flag5)
					{
						continue;
					}
					bool flag6 = false;
					foreach (string b in this.m_ObsoleteNames)
					{
						bool flag7 = attributeOverride2.m_AttributeName != b;
						if (!flag7)
						{
							flag6 = true;
							break;
						}
					}
					bool flag8 = !flag6;
					if (flag8)
					{
						continue;
					}
				}
				bool flag9 = attributeOverride.m_AttributeName == null;
				if (flag9)
				{
					attributeOverride = attributeOverride2;
					bool flag10 = attributeOverride.m_NamesPath == null;
					if (flag10)
					{
						break;
					}
				}
				else
				{
					bool flag11 = attributeOverride.m_NamesPath.Length < attributeOverride2.m_NamesPath.Length;
					if (flag11)
					{
						attributeOverride = attributeOverride2;
					}
				}
			}
			bool flag12 = attributeOverride.m_AttributeName != null;
			bool result;
			if (flag12)
			{
				value = attributeOverride.m_Value;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal bool TryGetValueFromBagAsString(IUxmlAttributes bag, CreationContext cc, out string value)
		{
			VisualTreeAsset visualTreeAsset;
			return this.TryGetValueFromBagAsString(bag, cc, out value, out visualTreeAsset);
		}

		internal bool TryGetAttributeOverrideValueFromBagAsString(IUxmlAttributes bag, CreationContext cc, out string value, out VisualTreeAsset sourceAsset)
		{
			string text;
			bag.TryGetAttributeValue("name", out text);
			bool flag = !string.IsNullOrEmpty(text) && cc.attributeOverrides != null;
			if (flag)
			{
				foreach (CreationContext.AttributeOverrideRange attributeOverrideRange in cc.attributeOverrides)
				{
					bool flag2 = this.TryFindValueInAttributeOverrides(text, cc, attributeOverrideRange.attributeOverrides, out value);
					if (flag2)
					{
						sourceAsset = attributeOverrideRange.sourceAsset;
						return true;
					}
				}
			}
			sourceAsset = null;
			value = null;
			return false;
		}

		internal bool ValidateName()
		{
			bool flag = this.name == null && (this.m_ObsoleteNames == null || this.m_ObsoleteNames.Length == 0);
			bool result;
			if (flag)
			{
				Debug.LogError("Attribute description has no name.");
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		internal bool TryGetValueFromBagAsString(IUxmlAttributes bag, CreationContext cc, out string value, out VisualTreeAsset sourceAsset)
		{
			value = null;
			sourceAsset = null;
			bool flag = !this.ValidateName();
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this.TryGetAttributeOverrideValueFromBagAsString(bag, cc, out value, out sourceAsset);
				if (flag2)
				{
					result = true;
				}
				else
				{
					bool flag3 = this.name == null;
					if (flag3)
					{
						for (int i = 0; i < this.m_ObsoleteNames.Length; i++)
						{
							bool flag4 = bag.TryGetAttributeValue(this.m_ObsoleteNames[i], out value);
							if (flag4)
							{
								sourceAsset = cc.visualTreeAsset;
								return true;
							}
						}
						result = false;
					}
					else
					{
						bool flag5 = !bag.TryGetAttributeValue(this.name, out value);
						if (flag5)
						{
							bool flag6 = this.m_ObsoleteNames != null;
							if (flag6)
							{
								for (int j = 0; j < this.m_ObsoleteNames.Length; j++)
								{
									bool flag7 = bag.TryGetAttributeValue(this.m_ObsoleteNames[j], out value);
									if (flag7)
									{
										sourceAsset = cc.visualTreeAsset;
										UxmlAsset uxmlAsset = bag as UxmlAsset;
										bool flag8 = uxmlAsset != null;
										if (flag8)
										{
											uxmlAsset.RemoveAttribute(this.m_ObsoleteNames[j]);
											uxmlAsset.SetAttribute(this.name, value);
										}
										return true;
									}
								}
							}
							result = false;
						}
						else
						{
							sourceAsset = cc.visualTreeAsset;
							result = true;
						}
					}
				}
			}
			return result;
		}

		protected bool TryGetValueFromBag<T>(IUxmlAttributes bag, CreationContext cc, Func<string, T, T> converterFunc, T defaultValue, ref T value)
		{
			string arg;
			bool flag = this.TryGetValueFromBagAsString(bag, cc, out arg);
			bool result;
			if (flag)
			{
				bool flag2 = converterFunc != null;
				if (flag2)
				{
					value = converterFunc(arg, defaultValue);
				}
				else
				{
					value = defaultValue;
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		protected T GetValueFromBag<T>(IUxmlAttributes bag, CreationContext cc, Func<string, T, T> converterFunc, T defaultValue)
		{
			bool flag = converterFunc == null;
			if (flag)
			{
				throw new ArgumentNullException("converterFunc");
			}
			string arg;
			bool flag2 = this.TryGetValueFromBagAsString(bag, cc, out arg);
			T result;
			if (flag2)
			{
				result = converterFunc(arg, defaultValue);
			}
			else
			{
				result = defaultValue;
			}
			return result;
		}

		protected const string xmlSchemaNamespace = "http://www.w3.org/2001/XMLSchema";

		private string[] m_ObsoleteNames;

		public enum Use
		{
			None,
			Optional,
			Prohibited,
			Required
		}
	}
}
