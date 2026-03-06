using System;

namespace UnityEngine.UIElements
{
	public class UxmlTypeAttributeDescription<TBase> : TypedUxmlAttributeDescription<Type>
	{
		public UxmlTypeAttributeDescription()
		{
			base.type = "string";
			base.typeNamespace = "http://www.w3.org/2001/XMLSchema";
			base.defaultValue = null;
		}

		public override string defaultValueAsString
		{
			get
			{
				return (base.defaultValue == null) ? "null" : base.defaultValue.FullName;
			}
		}

		public override Type GetValueFromBag(IUxmlAttributes bag, CreationContext cc)
		{
			return base.GetValueFromBag<Type>(bag, cc, (string s, Type type1) => this.ConvertValueToType(s, type1), base.defaultValue);
		}

		public bool TryGetValueFromBag(IUxmlAttributes bag, CreationContext cc, ref Type value)
		{
			return base.TryGetValueFromBag<Type>(bag, cc, (string s, Type type1) => this.ConvertValueToType(s, type1), base.defaultValue, ref value);
		}

		private Type ConvertValueToType(string v, Type defaultValue)
		{
			bool flag = string.IsNullOrEmpty(v);
			Type result;
			if (flag)
			{
				result = defaultValue;
			}
			else
			{
				try
				{
					Type type = Type.GetType(v, true);
					bool flag2 = !typeof(TBase).IsAssignableFrom(type);
					if (!flag2)
					{
						return type;
					}
					Debug.LogError(string.Concat(new string[]
					{
						"Type: Invalid type \"",
						v,
						"\". Type must derive from ",
						typeof(TBase).FullName,
						"."
					}));
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
				result = defaultValue;
			}
			return result;
		}
	}
}
