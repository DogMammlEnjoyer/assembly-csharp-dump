using System;

namespace Sirenix.OdinInspector
{
	public interface ISearchFilterable
	{
		bool IsMatch(string searchString);
	}
}
