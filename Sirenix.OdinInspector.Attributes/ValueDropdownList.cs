using System;
using System.Collections.Generic;

namespace Sirenix.OdinInspector
{
	public class ValueDropdownList<T> : List<ValueDropdownItem<T>>
	{
		public void Add(string text, T value)
		{
			base.Add(new ValueDropdownItem<T>(text, value));
		}

		public void Add(T value)
		{
			base.Add(new ValueDropdownItem<T>(value.ToString(), value));
		}
	}
}
