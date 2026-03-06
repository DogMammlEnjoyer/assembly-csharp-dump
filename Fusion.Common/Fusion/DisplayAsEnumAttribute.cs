using System;

namespace Fusion
{
	[AttributeUsage(AttributeTargets.Field)]
	public class DisplayAsEnumAttribute : DrawerPropertyAttribute
	{
		public DisplayAsEnumAttribute(Type enumType)
		{
			this.EnumType = enumType;
		}

		public DisplayAsEnumAttribute(string enumTypeMemberName)
		{
			this.EnumTypeMemberName = enumTypeMemberName;
		}

		public Type EnumType { get; }

		public string EnumTypeMemberName { get; }
	}
}
