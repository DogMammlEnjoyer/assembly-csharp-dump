using System;

namespace Sirenix.OdinInspector
{
	public struct ValueDropdownItem<T> : IValueDropdownItem
	{
		public ValueDropdownItem(string text, T value)
		{
			this.Text = text;
			this.Value = value;
		}

		string IValueDropdownItem.GetText()
		{
			return this.Text;
		}

		object IValueDropdownItem.GetValue()
		{
			return this.Value;
		}

		public override string ToString()
		{
			string result;
			if ((result = this.Text) == null)
			{
				T value = this.Value;
				result = (((value != null) ? value.ToString() : null) ?? "");
			}
			return result;
		}

		public string Text;

		public T Value;
	}
}
