using System;

namespace Sirenix.OdinInspector
{
	public class UnitAttribute : Attribute
	{
		public UnitAttribute(Units unit)
		{
			this.Base = unit;
			this.Display = unit;
		}

		public UnitAttribute(string unit)
		{
			this.BaseName = unit;
			this.DisplayName = unit;
		}

		public UnitAttribute(Units @base, Units display)
		{
			this.Base = @base;
			this.Display = display;
		}

		public UnitAttribute(Units @base, string display)
		{
			this.Base = @base;
			this.DisplayName = display;
		}

		public UnitAttribute(string @base, Units display)
		{
			this.BaseName = @base;
			this.Display = display;
		}

		public UnitAttribute(string @base, string display)
		{
			this.BaseName = @base;
			this.DisplayName = display;
		}

		public Units Base = Units.Unset;

		public Units Display = Units.Unset;

		public string BaseName;

		public string DisplayName;

		public bool DisplayAsString;

		public bool ForceDisplayUnit;
	}
}
