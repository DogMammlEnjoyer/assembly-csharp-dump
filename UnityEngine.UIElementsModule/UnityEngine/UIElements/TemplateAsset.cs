using System;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.Bindings;
using UnityEngine.Pool;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[]
	{
		"UnityEditor.UIBuilderModule"
	})]
	[Serializable]
	internal class TemplateAsset : VisualElementAsset
	{
		public string templateAlias
		{
			get
			{
				return this.m_TemplateAlias;
			}
			set
			{
				this.m_TemplateAlias = value;
			}
		}

		public List<TemplateAsset.AttributeOverride> attributeOverrides
		{
			get
			{
				return this.m_AttributeOverrides;
			}
			set
			{
				this.m_AttributeOverrides = value;
			}
		}

		public bool hasAttributeOverride
		{
			get
			{
				List<TemplateAsset.AttributeOverride> attributeOverrides = this.m_AttributeOverrides;
				return attributeOverrides != null && attributeOverrides.Count > 0;
			}
		}

		public List<TemplateAsset.UxmlSerializedDataOverride> serializedDataOverrides
		{
			get
			{
				return this.m_SerializedDataOverride;
			}
			set
			{
				this.m_SerializedDataOverride = value;
			}
		}

		internal override VisualElement Instantiate(CreationContext cc)
		{
			TemplateContainer templateContainer = (TemplateContainer)base.Instantiate(cc);
			bool flag = templateContainer.templateSource == null;
			if (flag)
			{
				TemplateContainer templateContainer2 = templateContainer;
				VisualTreeAsset visualTreeAsset = cc.visualTreeAsset;
				templateContainer2.templateSource = ((visualTreeAsset != null) ? visualTreeAsset.ResolveTemplate(templateContainer.templateId) : null);
				bool flag2 = templateContainer.templateSource == null;
				if (flag2)
				{
					templateContainer.Add(new Label("Unknown Template: '" + templateContainer.templateId + "'"));
					return templateContainer;
				}
			}
			List<CreationContext.AttributeOverrideRange> list;
			VisualElement result;
			using (CollectionPool<List<CreationContext.AttributeOverrideRange>, CreationContext.AttributeOverrideRange>.Get(out list))
			{
				List<CreationContext.SerializedDataOverrideRange> list2;
				using (CollectionPool<List<CreationContext.SerializedDataOverrideRange>, CreationContext.SerializedDataOverrideRange>.Get(out list2))
				{
					bool flag3 = cc.attributeOverrides != null;
					if (flag3)
					{
						list.AddRange(cc.attributeOverrides);
					}
					bool flag4 = this.attributeOverrides.Count > 0;
					if (flag4)
					{
						list.Add(new CreationContext.AttributeOverrideRange(cc.visualTreeAsset, this.attributeOverrides));
					}
					bool flag5 = cc.serializedDataOverrides != null;
					if (flag5)
					{
						list2.AddRange(cc.serializedDataOverrides);
					}
					bool flag6 = this.serializedDataOverrides.Count > 0;
					if (flag6)
					{
						list2.Add(new CreationContext.SerializedDataOverrideRange(cc.visualTreeAsset, this.serializedDataOverrides, base.id));
					}
					List<int> veaIdsPath = (cc.veaIdsPath != null) ? new List<int>(cc.veaIdsPath) : new List<int>();
					CreationContext cc2 = new CreationContext(cc.slotInsertionPoints, list, list2, null, null, veaIdsPath, null);
					templateContainer.templateSource.CloneTree(templateContainer, cc2);
					result = templateContainer;
				}
			}
			return result;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal List<VisualTreeAsset.SlotUsageEntry> slotUsages
		{
			[VisibleToOtherModules(new string[]
			{
				"UnityEditor.UIBuilderModule"
			})]
			get
			{
				return this.m_SlotUsages;
			}
			set
			{
				this.m_SlotUsages = value;
			}
		}

		public TemplateAsset(string templateAlias, string fullTypeName, UxmlNamespaceDefinition xmlNamespace = default(UxmlNamespaceDefinition)) : base(fullTypeName, xmlNamespace)
		{
			Assert.IsFalse(string.IsNullOrEmpty(templateAlias), "Template alias must not be null or empty");
			this.m_TemplateAlias = templateAlias;
		}

		public void AddSlotUsage(string slotName, int resId)
		{
			bool flag = this.m_SlotUsages == null;
			if (flag)
			{
				this.m_SlotUsages = new List<VisualTreeAsset.SlotUsageEntry>();
			}
			this.m_SlotUsages.Add(new VisualTreeAsset.SlotUsageEntry(slotName, resId));
		}

		[SerializeField]
		private string m_TemplateAlias;

		[SerializeField]
		private List<TemplateAsset.AttributeOverride> m_AttributeOverrides = new List<TemplateAsset.AttributeOverride>();

		[SerializeField]
		private List<TemplateAsset.UxmlSerializedDataOverride> m_SerializedDataOverride = new List<TemplateAsset.UxmlSerializedDataOverride>();

		[SerializeField]
		private List<VisualTreeAsset.SlotUsageEntry> m_SlotUsages;

		[Serializable]
		public struct AttributeOverride
		{
			public bool NamesPathMatchesElementNamesPath(IList<string> elementNamesPath)
			{
				bool flag = elementNamesPath == null || this.m_NamesPath == null || elementNamesPath.Count == 0 || this.m_NamesPath.Length == 0;
				bool result;
				if (flag)
				{
					result = false;
				}
				else
				{
					bool flag2 = this.m_NamesPath.Length == 1;
					if (flag2)
					{
						result = (this.m_NamesPath[0] == elementNamesPath[elementNamesPath.Count - 1]);
					}
					else
					{
						bool flag3 = this.m_NamesPath.Length != elementNamesPath.Count;
						if (flag3)
						{
							result = false;
						}
						else
						{
							for (int i = elementNamesPath.Count - 1; i >= 0; i--)
							{
								bool flag4 = elementNamesPath[i] != this.m_NamesPath[i];
								if (flag4)
								{
									return false;
								}
							}
							result = true;
						}
					}
				}
				return result;
			}

			public string m_ElementName;

			public string[] m_NamesPath;

			public string m_AttributeName;

			public string m_Value;
		}

		[Serializable]
		public struct UxmlSerializedDataOverride
		{
			public int m_ElementId;

			public List<int> m_ElementIdsPath;

			[SerializeReference]
			public UxmlSerializedData m_SerializedData;
		}
	}
}
