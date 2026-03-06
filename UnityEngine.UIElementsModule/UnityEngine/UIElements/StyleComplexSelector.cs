using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[]
	{
		"UnityEditor.UIBuilderModule"
	})]
	[Serializable]
	internal class StyleComplexSelector : ISerializationCallbackReceiver
	{
		public int specificity
		{
			get
			{
				return this.m_Specificity;
			}
			[VisibleToOtherModules(new string[]
			{
				"UnityEditor.UIBuilderModule"
			})]
			internal set
			{
				this.m_Specificity = value;
			}
		}

		public StyleRule rule { get; [VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})] internal set; }

		public bool isSimple
		{
			get
			{
				return this.m_isSimple;
			}
		}

		public StyleSelector[] selectors
		{
			get
			{
				return this.m_Selectors;
			}
			[VisibleToOtherModules(new string[]
			{
				"UnityEditor.UIBuilderModule"
			})]
			internal set
			{
				this.m_Selectors = value;
				this.m_isSimple = (this.m_Selectors.Length == 1);
			}
		}

		public void OnBeforeSerialize()
		{
		}

		public virtual void OnAfterDeserialize()
		{
			this.m_isSimple = (this.m_Selectors.Length == 1);
		}

		internal void CachePseudoStateMasks(StyleSheet styleSheet)
		{
			bool flag = StyleComplexSelector.s_PseudoStates == null;
			if (flag)
			{
				StyleComplexSelector.s_PseudoStates = new Dictionary<string, StyleComplexSelector.PseudoStateData>();
				StyleComplexSelector.s_PseudoStates["active"] = new StyleComplexSelector.PseudoStateData(PseudoStates.Active, false);
				StyleComplexSelector.s_PseudoStates["hover"] = new StyleComplexSelector.PseudoStateData(PseudoStates.Hover, false);
				StyleComplexSelector.s_PseudoStates["checked"] = new StyleComplexSelector.PseudoStateData(PseudoStates.Checked, false);
				StyleComplexSelector.s_PseudoStates["selected"] = new StyleComplexSelector.PseudoStateData(PseudoStates.Checked, false);
				StyleComplexSelector.s_PseudoStates["disabled"] = new StyleComplexSelector.PseudoStateData(PseudoStates.Disabled, false);
				StyleComplexSelector.s_PseudoStates["focus"] = new StyleComplexSelector.PseudoStateData(PseudoStates.Focus, false);
				StyleComplexSelector.s_PseudoStates["root"] = new StyleComplexSelector.PseudoStateData(PseudoStates.Root, false);
				StyleComplexSelector.s_PseudoStates["inactive"] = new StyleComplexSelector.PseudoStateData(PseudoStates.Active, true);
				StyleComplexSelector.s_PseudoStates["enabled"] = new StyleComplexSelector.PseudoStateData(PseudoStates.Disabled, true);
			}
			int i = 0;
			int num = this.selectors.Length;
			while (i < num)
			{
				StyleSelector styleSelector = this.selectors[i];
				StyleSelectorPart[] parts = styleSelector.parts;
				PseudoStates pseudoStates = (PseudoStates)0;
				PseudoStates pseudoStates2 = (PseudoStates)0;
				bool flag2 = true;
				int num2 = 0;
				while (num2 < styleSelector.parts.Length && flag2)
				{
					bool flag3 = styleSelector.parts[num2].type == StyleSelectorType.PseudoClass;
					if (flag3)
					{
						StyleComplexSelector.PseudoStateData pseudoStateData;
						bool flag4 = StyleComplexSelector.s_PseudoStates.TryGetValue(parts[num2].value, out pseudoStateData);
						if (flag4)
						{
							bool flag5 = !pseudoStateData.negate;
							if (flag5)
							{
								pseudoStates |= pseudoStateData.state;
							}
							else
							{
								pseudoStates2 |= pseudoStateData.state;
							}
						}
						else
						{
							Debug.LogWarningFormat(styleSheet, "Unknown pseudo class \"{0}\" in StyleSheet {1}", new object[]
							{
								parts[num2].value,
								styleSheet.name
							});
							flag2 = false;
						}
					}
					num2++;
				}
				bool flag6 = flag2;
				if (flag6)
				{
					styleSelector.pseudoStateMask = (int)pseudoStates;
					styleSelector.negatedPseudoStateMask = (int)pseudoStates2;
				}
				else
				{
					styleSelector.pseudoStateMask = -1;
					styleSelector.negatedPseudoStateMask = -1;
				}
				i++;
			}
		}

		public override string ToString()
		{
			return string.Format("[{0}]", string.Join(", ", (from x in this.m_Selectors
			select x.ToString()).ToArray<string>()));
		}

		private static int StyleSelectorPartCompare(StyleSelectorPart x, StyleSelectorPart y)
		{
			bool flag = y.type < x.type;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				bool flag2 = y.type > x.type;
				if (flag2)
				{
					result = 1;
				}
				else
				{
					result = y.value.CompareTo(x.value);
				}
			}
			return result;
		}

		internal unsafe void CalculateHashes()
		{
			bool isSimple = this.isSimple;
			if (!isSimple)
			{
				for (int i = this.selectors.Length - 2; i > -1; i--)
				{
					StyleComplexSelector.m_HashList.AddRange(this.selectors[i].parts);
				}
				StyleComplexSelector.m_HashList.RemoveAll((StyleSelectorPart p) => p.type != StyleSelectorType.Class && p.type != StyleSelectorType.ID && p.type != StyleSelectorType.Type);
				StyleComplexSelector.m_HashList.Sort(new Comparison<StyleSelectorPart>(StyleComplexSelector.StyleSelectorPartCompare));
				bool flag = true;
				StyleSelectorType styleSelectorType = StyleSelectorType.Unknown;
				string text = "";
				int num = 0;
				int num2 = Math.Min(4, StyleComplexSelector.m_HashList.Count);
				for (int j = 0; j < num2; j++)
				{
					bool flag2 = flag;
					if (flag2)
					{
						flag = false;
					}
					else
					{
						while (num < StyleComplexSelector.m_HashList.Count && StyleComplexSelector.m_HashList[num].type == styleSelectorType && StyleComplexSelector.m_HashList[num].value == text)
						{
							num++;
						}
						bool flag3 = num == StyleComplexSelector.m_HashList.Count;
						if (flag3)
						{
							break;
						}
					}
					styleSelectorType = StyleComplexSelector.m_HashList[num].type;
					text = StyleComplexSelector.m_HashList[num].value;
					bool flag4 = styleSelectorType == StyleSelectorType.ID;
					Salt salt;
					if (flag4)
					{
						salt = Salt.IdSalt;
					}
					else
					{
						bool flag5 = styleSelectorType == StyleSelectorType.Class;
						if (flag5)
						{
							salt = Salt.ClassSalt;
						}
						else
						{
							salt = Salt.TagNameSalt;
						}
					}
					*(ref this.ancestorHashes.hashes.FixedElementField + (IntPtr)j * 4) = text.GetHashCode() * (int)salt;
				}
				StyleComplexSelector.m_HashList.Clear();
			}
		}

		[NonSerialized]
		public Hashes ancestorHashes;

		[SerializeField]
		private int m_Specificity;

		[NonSerialized]
		private bool m_isSimple;

		[SerializeField]
		private StyleSelector[] m_Selectors;

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		[SerializeField]
		internal int ruleIndex;

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		[NonSerialized]
		internal StyleComplexSelector nextInTable;

		[NonSerialized]
		internal int orderInStyleSheet;

		private static Dictionary<string, StyleComplexSelector.PseudoStateData> s_PseudoStates;

		private static List<StyleSelectorPart> m_HashList = new List<StyleSelectorPart>();

		private struct PseudoStateData
		{
			public PseudoStateData(PseudoStates state, bool negate)
			{
				this.state = state;
				this.negate = negate;
			}

			public readonly PseudoStates state;

			public readonly bool negate;
		}
	}
}
