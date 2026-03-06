using System;
using System.Collections.Generic;

namespace Unity.Properties
{
	public class KeyValuePairPropertyBag<TKey, TValue> : PropertyBag<KeyValuePair<TKey, TValue>>, INamedProperties<KeyValuePair<TKey, TValue>>
	{
		public override PropertyCollection<KeyValuePair<TKey, TValue>> GetProperties()
		{
			return new PropertyCollection<KeyValuePair<TKey, TValue>>(KeyValuePairPropertyBag<TKey, TValue>.GetPropertiesEnumerable());
		}

		public override PropertyCollection<KeyValuePair<TKey, TValue>> GetProperties(ref KeyValuePair<TKey, TValue> container)
		{
			return new PropertyCollection<KeyValuePair<TKey, TValue>>(KeyValuePairPropertyBag<TKey, TValue>.GetPropertiesEnumerable());
		}

		private static IEnumerable<IProperty<KeyValuePair<TKey, TValue>>> GetPropertiesEnumerable()
		{
			yield return KeyValuePairPropertyBag<TKey, TValue>.s_KeyProperty;
			yield return KeyValuePairPropertyBag<TKey, TValue>.s_ValueProperty;
			yield break;
		}

		public bool TryGetProperty(ref KeyValuePair<TKey, TValue> container, string name, out IProperty<KeyValuePair<TKey, TValue>> property)
		{
			bool flag = name == "Key";
			bool result;
			if (flag)
			{
				property = KeyValuePairPropertyBag<TKey, TValue>.s_KeyProperty;
				result = true;
			}
			else
			{
				bool flag2 = name == "Value";
				if (flag2)
				{
					property = KeyValuePairPropertyBag<TKey, TValue>.s_ValueProperty;
					result = true;
				}
				else
				{
					property = null;
					result = false;
				}
			}
			return result;
		}

		private static readonly DelegateProperty<KeyValuePair<TKey, TValue>, TKey> s_KeyProperty = new DelegateProperty<KeyValuePair<TKey, TValue>, TKey>("Key", delegate(ref KeyValuePair<TKey, TValue> container)
		{
			return container.Key;
		}, null);

		private static readonly DelegateProperty<KeyValuePair<TKey, TValue>, TValue> s_ValueProperty = new DelegateProperty<KeyValuePair<TKey, TValue>, TValue>("Value", delegate(ref KeyValuePair<TKey, TValue> container)
		{
			return container.Value;
		}, null);
	}
}
