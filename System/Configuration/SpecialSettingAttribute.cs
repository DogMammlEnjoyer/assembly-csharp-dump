using System;

namespace System.Configuration
{
	/// <summary>Indicates that an application settings property has a special significance. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
	public sealed class SpecialSettingAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SpecialSettingAttribute" /> class.</summary>
		/// <param name="specialSetting">A <see cref="T:System.Configuration.SpecialSetting" /> enumeration value defining the category of the application settings property.</param>
		public SpecialSettingAttribute(SpecialSetting specialSetting)
		{
			this.setting = specialSetting;
		}

		/// <summary>Gets the value describing the special setting category of the application settings property.</summary>
		/// <returns>A <see cref="T:System.Configuration.SpecialSetting" /> enumeration value defining the category of the application settings property.</returns>
		public SpecialSetting SpecialSetting
		{
			get
			{
				return this.setting;
			}
		}

		private SpecialSetting setting;
	}
}
