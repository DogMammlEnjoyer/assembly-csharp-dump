using System;
using Meta.WitAi.Attributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Meta.WitAi.Utilities
{
	[AddComponentMenu("Wit.ai/Utilities/Conversions/Float to String")]
	public class FloatToStringEvent : MonoBehaviour
	{
		public void ConvertFloatToString(float value)
		{
			string arg;
			if (string.IsNullOrEmpty(this._floatFormat))
			{
				arg = value.ToString();
			}
			else
			{
				arg = value.ToString(this._floatFormat);
			}
			if (string.IsNullOrEmpty(this._stringFormat))
			{
				StringEvent stringEvent = this.onFloatToString;
				if (stringEvent == null)
				{
					return;
				}
				stringEvent.Invoke(arg);
				return;
			}
			else
			{
				StringEvent stringEvent2 = this.onFloatToString;
				if (stringEvent2 == null)
				{
					return;
				}
				stringEvent2.Invoke(string.Format(this._stringFormat, arg));
				return;
			}
		}

		[FormerlySerializedAs("format")]
		[Tooltip("The format value to be used on the float")]
		[SerializeField]
		private string _floatFormat;

		[Tooltip("The format of the string itself. {0} will represent the float value provided")]
		[SerializeField]
		private string _stringFormat;

		[Space(8f)]
		[TooltipBox("Triggered when ConvertFloatToString(float) is called. The string in this event will be formatted based on the format fields.")]
		[SerializeField]
		private StringEvent onFloatToString = new StringEvent();
	}
}
