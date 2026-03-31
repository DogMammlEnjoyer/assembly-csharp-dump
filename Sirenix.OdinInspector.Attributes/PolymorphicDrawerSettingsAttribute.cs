using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class PolymorphicDrawerSettingsAttribute : Attribute
	{
		[ShowInInspector]
		[OdinDesignerBinding(new string[]
		{
			"showBaseType"
		})]
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

		[ShowInInspector]
		[OdinDesignerBinding(new string[]
		{
			"nonDefaultConstructorPreference"
		})]
		[LabelWidth(210f)]
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

		[LabelWidth(190f)]
		public bool ReadOnlyIfNotNullReference;

		public string CreateInstanceFunction;

		[Obsolete("Use OnValueChangedAttribute instead.", false)]
		public string OnInstanceAssigned;

		private bool? showBaseType;

		private NonDefaultConstructorPreference? nonDefaultConstructorPreference;
	}
}
