using System;
using Meta.WitAi.Attributes;
using UnityEngine;

namespace Meta.WitAi.Utilities
{
	[AddComponentMenu("Wit.ai/Utilities/Conversions/String to String")]
	public class StringToStringEvent : MonoBehaviour
	{
		public void FormatString(string format, string value)
		{
			if (string.IsNullOrEmpty(format))
			{
				StringEvent stringEvent = this.onStringEvent;
				if (stringEvent == null)
				{
					return;
				}
				stringEvent.Invoke(value);
				return;
			}
			else
			{
				StringEvent stringEvent2 = this.onStringEvent;
				if (stringEvent2 == null)
				{
					return;
				}
				stringEvent2.Invoke(string.Format(format, value));
				return;
			}
		}

		public void FormatString(string value)
		{
			this.FormatString(this._format, value);
		}

		[Tooltip("The string format string that will be used to reformat input strings. Ex: I don't know how to respond to {0}")]
		[SerializeField]
		private string _format;

		[Space(8f)]
		[TooltipBox("Triggered when FormatString(float) is called. The string in this event will be formatted based on the format field.")]
		[SerializeField]
		public StringEvent onStringEvent = new StringEvent();
	}
}
