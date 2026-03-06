using System;

namespace UnityEngine.UIElements
{
	public class UxmlAssetAttributeDescription<T> : TypedUxmlAttributeDescription<T>, IUxmlAssetAttributeDescription where T : Object
	{
		public UxmlAssetAttributeDescription()
		{
			base.type = "string";
			base.typeNamespace = "http://www.w3.org/2001/XMLSchema";
			base.defaultValue = default(T);
		}

		public override string defaultValueAsString
		{
			get
			{
				T t = base.defaultValue;
				return ((t != null) ? t.ToString() : null) ?? "null";
			}
		}

		public override T GetValueFromBag(IUxmlAttributes bag, CreationContext cc)
		{
			string path;
			VisualTreeAsset visualTreeAsset;
			bool flag = base.TryGetValueFromBagAsString(bag, cc, out path, out visualTreeAsset) && visualTreeAsset != null;
			T result;
			if (flag)
			{
				result = visualTreeAsset.GetAsset<T>(path);
			}
			else
			{
				result = default(T);
			}
			return result;
		}

		public bool TryGetValueFromBag(IUxmlAttributes bag, CreationContext cc, out T value)
		{
			string path;
			VisualTreeAsset visualTreeAsset;
			bool flag = base.TryGetValueFromBagAsString(bag, cc, out path, out visualTreeAsset) && visualTreeAsset != null;
			bool result;
			if (flag)
			{
				value = visualTreeAsset.GetAsset<T>(path);
				result = true;
			}
			else
			{
				value = default(T);
				result = false;
			}
			return result;
		}

		Type IUxmlAssetAttributeDescription.assetType
		{
			get
			{
				return typeof(T);
			}
		}
	}
}
