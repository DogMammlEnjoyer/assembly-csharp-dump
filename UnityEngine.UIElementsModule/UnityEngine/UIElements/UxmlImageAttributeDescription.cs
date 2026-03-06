using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[]
	{
		"UnityEditor.UIBuilderModule"
	})]
	internal class UxmlImageAttributeDescription : UxmlAttributeDescription, IUxmlAssetAttributeDescription
	{
		public UxmlImageAttributeDescription()
		{
			base.type = "string";
			base.typeNamespace = "http://www.w3.org/2001/XMLSchema";
			this.defaultValue = default(Background);
		}

		public Background defaultValue { get; set; }

		public override string defaultValueAsString
		{
			get
			{
				return this.defaultValue.IsEmpty() ? "null" : this.defaultValue.ToString();
			}
		}

		public Background GetValueFromBag(IUxmlAttributes bag, CreationContext cc)
		{
			string text;
			VisualTreeAsset visualTreeAsset;
			bool flag = base.TryGetValueFromBagAsString(bag, cc, out text, out visualTreeAsset) && visualTreeAsset != null;
			Background result;
			if (flag)
			{
				bool flag2 = text == null;
				if (flag2)
				{
					result = default(Background);
				}
				else
				{
					bool flag3 = this.m_AssetType == null;
					if (flag3)
					{
						this.m_AssetType = visualTreeAsset.GetAssetType(text);
					}
					result = Background.FromObject(visualTreeAsset.GetAsset(text, this.m_AssetType));
				}
			}
			else
			{
				result = default(Background);
			}
			return result;
		}

		Type IUxmlAssetAttributeDescription.assetType
		{
			get
			{
				return this.m_AssetType ?? typeof(Texture);
			}
		}

		private Type m_AssetType;
	}
}
