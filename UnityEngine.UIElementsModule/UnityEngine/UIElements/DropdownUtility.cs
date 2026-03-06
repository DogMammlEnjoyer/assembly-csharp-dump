using System;

namespace UnityEngine.UIElements
{
	internal static class DropdownUtility
	{
		internal static IGenericMenu CreateDropdown()
		{
			IGenericMenu result;
			if (DropdownUtility.MakeDropdownFunc == null)
			{
				IGenericMenu genericMenu = new GenericDropdownMenu();
				result = genericMenu;
			}
			else
			{
				result = DropdownUtility.MakeDropdownFunc();
			}
			return result;
		}

		internal static Func<IGenericMenu> MakeDropdownFunc;
	}
}
