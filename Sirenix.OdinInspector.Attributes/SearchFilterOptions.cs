using System;

namespace Sirenix.OdinInspector
{
	[Flags]
	public enum SearchFilterOptions
	{
		PropertyName = 1,
		PropertyNiceName = 2,
		TypeOfValue = 4,
		ValueToString = 8,
		ISearchFilterableInterface = 16,
		All = -1
	}
}
