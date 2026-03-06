using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class PolymorphicDrawerSettingsAttribute : Attribute
	{
		public bool ShowBaseType
		{
			get
			{
				return this.showBaseType.GetValueOrDefault();
			}
			set
			{
				this.showBaseType = new bool?(value);
			}
		}

		public NonDefaultConstructorPreference NonDefaultConstructorPreference
		{
			get
			{
				return this.nonDefaultConstructorPreference.GetValueOrDefault(NonDefaultConstructorPreference.ConstructIdeal);
			}
			set
			{
				this.nonDefaultConstructorPreference = new NonDefaultConstructorPreference?(value);
			}
		}

		public bool ShowBaseTypeIsSet
		{
			get
			{
				return this.showBaseType != null;
			}
		}

		public bool NonDefaultConstructorPreferenceIsSet
		{
			get
			{
				return this.nonDefaultConstructorPreference != null;
			}
		}

		public bool ReadOnlyIfNotNullReference;

		public string CreateInstanceFunction;

		[Obsolete("Use OnValueChangedAttribute instead.", false)]
		public string OnInstanceAssigned;

		private bool? showBaseType;

		private NonDefaultConstructorPreference? nonDefaultConstructorPreference;
	}
}
