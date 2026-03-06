using System;

namespace Sirenix.OdinInspector
{
	public interface IValueDropdownItem
	{
		string GetText();

		object GetValue();
	}
}
