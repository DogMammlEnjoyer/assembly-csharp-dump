using System;
using System.Collections.Generic;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[]
	{
		"UnityEditor.UIBuilderModule"
	})]
	[Serializable]
	internal class VisualElementAsset : UxmlAsset, ISerializationCallbackReceiver
	{
		public int ruleIndex
		{
			get
			{
				return this.m_RuleIndex;
			}
			set
			{
				this.m_RuleIndex = value;
			}
		}

		public string[] classes
		{
			get
			{
				return this.m_Classes;
			}
			set
			{
				this.m_Classes = value;
			}
		}

		public List<string> stylesheetPaths
		{
			get
			{
				List<string> result;
				if ((result = this.m_StylesheetPaths) == null)
				{
					result = (this.m_StylesheetPaths = new List<string>());
				}
				return result;
			}
			set
			{
				this.m_StylesheetPaths = value;
			}
		}

		public bool hasStylesheetPaths
		{
			get
			{
				return this.m_StylesheetPaths != null;
			}
		}

		public List<StyleSheet> stylesheets
		{
			get
			{
				List<StyleSheet> result;
				if ((result = this.m_Stylesheets) == null)
				{
					result = (this.m_Stylesheets = new List<StyleSheet>());
				}
				return result;
			}
			set
			{
				this.m_Stylesheets = value;
			}
		}

		public bool hasStylesheets
		{
			get
			{
				return this.m_Stylesheets != null;
			}
		}

		public UxmlSerializedData serializedData
		{
			get
			{
				return this.m_SerializedData;
			}
			set
			{
				this.m_SerializedData = value;
			}
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal bool skipClone
		{
			get
			{
				return this.m_SkipClone;
			}
			set
			{
				this.m_SkipClone = value;
			}
		}

		public VisualElementAsset(string fullTypeName, UxmlNamespaceDefinition xmlNamespace = default(UxmlNamespaceDefinition)) : base(fullTypeName, xmlNamespace)
		{
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			bool flag = !string.IsNullOrEmpty(this.m_Name) && !this.m_Properties.Contains("name");
			if (flag)
			{
				base.SetAttribute("name", this.m_Name);
			}
			bool flag2 = !string.IsNullOrEmpty(this.m_Text) && !this.m_Properties.Contains("text");
			if (flag2)
			{
				base.SetAttribute("text", this.m_Text);
			}
			bool flag3 = this.m_PickingMode != PickingMode.Position && !this.m_Properties.Contains("picking-mode") && !this.m_Properties.Contains("pickingMode");
			if (flag3)
			{
				base.SetAttribute("picking-mode", this.m_PickingMode.ToString());
			}
		}

		private static bool IdsPathMatchesAttributeOverrideIdsPath(List<int> idsPath, List<int> attributeOverrideIdsPath, int templateId)
		{
			bool flag = idsPath == null || attributeOverrideIdsPath == null || idsPath.Count == 0 || attributeOverrideIdsPath.Count == 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				int num = idsPath.IndexOf(templateId);
				bool flag2 = idsPath.Count != attributeOverrideIdsPath.Count + num + 1;
				if (flag2)
				{
					result = false;
				}
				else
				{
					for (int i = idsPath.Count - 1; i > num; i--)
					{
						bool flag3 = idsPath[i] != attributeOverrideIdsPath[i - num - 1];
						if (flag3)
						{
							return false;
						}
					}
					result = true;
				}
			}
			return result;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal virtual VisualElement Instantiate(CreationContext cc)
		{
			VisualElement visualElement = (VisualElement)this.serializedData.CreateInstance();
			this.serializedData.Deserialize(visualElement);
			bool hasOverrides = cc.hasOverrides;
			if (hasOverrides)
			{
				cc.veaIdsPath.Add(base.id);
				for (int i = cc.serializedDataOverrides.Count - 1; i >= 0; i--)
				{
					foreach (TemplateAsset.UxmlSerializedDataOverride uxmlSerializedDataOverride in cc.serializedDataOverrides[i].attributeOverrides)
					{
						bool flag = uxmlSerializedDataOverride.m_ElementId == base.id && VisualElementAsset.IdsPathMatchesAttributeOverrideIdsPath(cc.veaIdsPath, uxmlSerializedDataOverride.m_ElementIdsPath, cc.serializedDataOverrides[i].templateId);
						if (flag)
						{
							uxmlSerializedDataOverride.m_SerializedData.Deserialize(visualElement);
						}
					}
				}
				cc.veaIdsPath.Remove(base.id);
			}
			bool hasStylesheetPaths = this.hasStylesheetPaths;
			if (hasStylesheetPaths)
			{
				for (int j = 0; j < this.stylesheetPaths.Count; j++)
				{
					visualElement.AddStyleSheetPath(this.stylesheetPaths[j]);
				}
			}
			bool hasStylesheets = this.hasStylesheets;
			if (hasStylesheets)
			{
				for (int k = 0; k < this.stylesheets.Count; k++)
				{
					bool flag2 = this.stylesheets[k] != null;
					if (flag2)
					{
						visualElement.styleSheets.Add(this.stylesheets[k]);
					}
				}
			}
			bool flag3 = this.classes != null;
			if (flag3)
			{
				for (int l = 0; l < this.classes.Length; l++)
				{
					visualElement.AddToClassList(this.classes[l]);
				}
			}
			return visualElement;
		}

		public override string ToString()
		{
			return string.Format("{0}({1})({2})", this.m_Name, base.fullTypeName, base.id);
		}

		[SerializeField]
		private string m_Name = string.Empty;

		[SerializeField]
		private int m_RuleIndex = -1;

		[SerializeField]
		private string m_Text = string.Empty;

		[SerializeField]
		private PickingMode m_PickingMode = PickingMode.Position;

		[SerializeField]
		private string[] m_Classes;

		[SerializeField]
		private List<string> m_StylesheetPaths;

		[SerializeField]
		private List<StyleSheet> m_Stylesheets;

		[SerializeReference]
		internal UxmlSerializedData m_SerializedData;

		[SerializeField]
		private bool m_SkipClone;
	}
}
