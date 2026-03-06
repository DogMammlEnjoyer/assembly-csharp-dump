using System;

namespace UnityEngine.Localization.PropertyVariants.TrackedProperties
{
	[Serializable]
	public class BoolTrackedProperty : TrackedProperty<bool>
	{
		protected override bool ConvertFromString(string value)
		{
			int num;
			if (int.TryParse(value, out num))
			{
				return (bool)Convert.ChangeType(num, typeof(bool));
			}
			return base.ConvertFromString(value);
		}

		protected override string ConvertToString(bool value)
		{
			if (!value)
			{
				return "0";
			}
			return "1";
		}
	}
}
