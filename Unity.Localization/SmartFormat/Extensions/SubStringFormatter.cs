using System;
using UnityEngine.Localization.SmartFormat.Core.Extensions;

namespace UnityEngine.Localization.SmartFormat.Extensions
{
	[Serializable]
	public class SubStringFormatter : FormatterBase
	{
		public SubStringFormatter.SubStringOutOfRangeBehavior OutOfRangeBehavior
		{
			get
			{
				return this.m_OutOfRangeBehavior;
			}
			set
			{
				this.m_OutOfRangeBehavior = value;
			}
		}

		public SubStringFormatter()
		{
			base.Names = this.DefaultNames;
		}

		public override string[] DefaultNames
		{
			get
			{
				return new string[]
				{
					"substr"
				};
			}
		}

		public char ParameterDelimiter
		{
			get
			{
				return this.m_ParameterDelimiter;
			}
			set
			{
				this.m_ParameterDelimiter = value;
			}
		}

		public string NullDisplayString
		{
			get
			{
				return this.m_NullDisplayString;
			}
			set
			{
				this.m_NullDisplayString = value;
			}
		}

		public override bool TryEvaluateFormat(IFormattingInfo formattingInfo)
		{
			if (formattingInfo.FormatterOptions == string.Empty)
			{
				return false;
			}
			string[] array = formattingInfo.FormatterOptions.Split(this.ParameterDelimiter, StringSplitOptions.None);
			object currentValue = formattingInfo.CurrentValue;
			string text = (currentValue != null) ? currentValue.ToString() : null;
			if (text == null)
			{
				formattingInfo.Write(this.NullDisplayString);
				return true;
			}
			int num = int.Parse(array[0]);
			int num2 = (array.Length > 1) ? int.Parse(array[1]) : 0;
			if (num < 0)
			{
				num = text.Length + num;
			}
			if (num > text.Length)
			{
				num = text.Length;
			}
			if (num2 < 0)
			{
				num2 = text.Length - num + num2;
			}
			SubStringFormatter.SubStringOutOfRangeBehavior outOfRangeBehavior = this.OutOfRangeBehavior;
			if (outOfRangeBehavior != SubStringFormatter.SubStringOutOfRangeBehavior.ReturnEmptyString)
			{
				if (outOfRangeBehavior == SubStringFormatter.SubStringOutOfRangeBehavior.ReturnStartIndexToEndOfString)
				{
					if (num > text.Length)
					{
						num = text.Length;
					}
					if (num + num2 > text.Length)
					{
						num2 = text.Length - num;
					}
				}
			}
			else if (num + num2 > text.Length)
			{
				num2 = 0;
			}
			string text2 = (array.Length > 1) ? text.Substring(num, num2) : text.Substring(num);
			formattingInfo.Write(text2);
			return true;
		}

		[SerializeField]
		private char m_ParameterDelimiter = ',';

		[SerializeField]
		private string m_NullDisplayString = "(null)";

		[SerializeField]
		private SubStringFormatter.SubStringOutOfRangeBehavior m_OutOfRangeBehavior;

		public enum SubStringOutOfRangeBehavior
		{
			ReturnEmptyString,
			ReturnStartIndexToEndOfString,
			ThrowException
		}
	}
}
