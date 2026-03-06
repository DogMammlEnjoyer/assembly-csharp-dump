using System;
using System.Globalization;

namespace UnityEngine.UIElements
{
	public class UxmlIntAttributeDescription : TypedUxmlAttributeDescription<int>
	{
		public UxmlIntAttributeDescription()
		{
			base.type = "int";
			base.typeNamespace = "http://www.w3.org/2001/XMLSchema";
			base.defaultValue = 0;
		}

		public override string defaultValueAsString
		{
			get
			{
				return base.defaultValue.ToString(CultureInfo.InvariantCulture.NumberFormat);
			}
		}

		public override int GetValueFromBag(IUxmlAttributes bag, CreationContext cc)
		{
			return base.GetValueFromBag<int>(bag, cc, (string s, int i) => UxmlIntAttributeDescription.ConvertValueToInt(s, i), base.defaultValue);
		}

		public bool TryGetValueFromBag(IUxmlAttributes bag, CreationContext cc, ref int value)
		{
			return base.TryGetValueFromBag<int>(bag, cc, (string s, int i) => UxmlIntAttributeDescription.ConvertValueToInt(s, i), base.defaultValue, ref value);
		}

		private static int ConvertValueToInt(string v, int defaultValue)
		{
			int num;
			bool flag = v == null || !int.TryParse(v, out num);
			int result;
			if (flag)
			{
				result = defaultValue;
			}
			else
			{
				result = num;
			}
			return result;
		}
	}
}
