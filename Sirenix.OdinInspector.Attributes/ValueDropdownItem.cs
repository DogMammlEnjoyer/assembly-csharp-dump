using System;

namespace Sirenix.OdinInspector
{
	public struct ValueDropdownItem : IValueDropdownItem
	{
		public ValueDropdownItem(string text, object value)
		{
			this.Text = text;
			this.Value = value;
		}

		public override string ToString()
		{
			string result;
			if ((result = this.Text) == null)
			{
				object value = this.Value;
				result = (((value != null) ? value.ToString() : null) ?? "");
			}
			return result;
		}

		string IValueDropdownItem.GetText()
		{
			return this.Text;
		}

		object IValueDropdownItem.GetValue()
		{
			return this.Value;
		}

		public string Text;

		public object Value;
	}
}
