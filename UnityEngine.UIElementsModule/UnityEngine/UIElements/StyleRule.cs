using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[]
	{
		"UnityEditor.UIBuilderModule"
	})]
	[Serializable]
	internal class StyleRule
	{
		public StyleProperty[] properties
		{
			get
			{
				return this.m_Properties;
			}
			[VisibleToOtherModules(new string[]
			{
				"UnityEditor.UIBuilderModule"
			})]
			internal set
			{
				this.m_Properties = value;
			}
		}

		[SerializeField]
		private StyleProperty[] m_Properties = Array.Empty<StyleProperty>();

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		[SerializeField]
		internal int line;

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		[NonSerialized]
		internal int customPropertiesCount;
	}
}
