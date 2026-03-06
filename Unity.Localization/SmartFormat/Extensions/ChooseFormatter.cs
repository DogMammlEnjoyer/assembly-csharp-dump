using System;
using System.Collections.Generic;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.Core.Parsing;

namespace UnityEngine.Localization.SmartFormat.Extensions
{
	[Serializable]
	public class ChooseFormatter : FormatterBase, IFormatterLiteralExtractor
	{
		public char SplitChar
		{
			get
			{
				return this.m_SplitChar;
			}
			set
			{
				this.m_SplitChar = value;
			}
		}

		public ChooseFormatter()
		{
			base.Names = this.DefaultNames;
		}

		public override string[] DefaultNames
		{
			get
			{
				return new string[]
				{
					"choose",
					"c"
				};
			}
		}

		public override bool TryEvaluateFormat(IFormattingInfo formattingInfo)
		{
			if (formattingInfo.FormatterOptions == "")
			{
				return false;
			}
			string[] chooseOptions = formattingInfo.FormatterOptions.Split(this.SplitChar, StringSplitOptions.None);
			IList<Format> list = formattingInfo.Format.Split(this.SplitChar);
			if (list.Count < 2)
			{
				return false;
			}
			Format format = ChooseFormatter.DetermineChosenFormat(formattingInfo, list, chooseOptions);
			formattingInfo.Write(format, formattingInfo.CurrentValue);
			return true;
		}

		private static Format DetermineChosenFormat(IFormattingInfo formattingInfo, IList<Format> choiceFormats, string[] chooseOptions)
		{
			object currentValue = formattingInfo.CurrentValue;
			string text = (currentValue == null) ? "null" : currentValue.ToString();
			int num = Array.IndexOf<string>(chooseOptions, text);
			if (choiceFormats.Count < chooseOptions.Length)
			{
				throw formattingInfo.FormattingException("You must specify at least " + chooseOptions.Length.ToString() + " choices", null, -1);
			}
			if (choiceFormats.Count > chooseOptions.Length + 1)
			{
				throw formattingInfo.FormattingException("You cannot specify more than " + (chooseOptions.Length + 1).ToString() + " choices", null, -1);
			}
			if (num == -1 && choiceFormats.Count == chooseOptions.Length)
			{
				throw formattingInfo.FormattingException("\"" + text + "\" is not a valid choice, and a \"default\" choice was not supplied", null, -1);
			}
			if (num == -1)
			{
				num = choiceFormats.Count - 1;
			}
			return choiceFormats[num];
		}

		public void WriteAllLiterals(IFormattingInfo formattingInfo)
		{
			if (formattingInfo.FormatterOptions == "")
			{
				return;
			}
			IList<Format> list = formattingInfo.Format.Split(this.SplitChar);
			if (list.Count < 2)
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				formattingInfo.Write(list[i], null);
			}
		}

		[SerializeField]
		private char m_SplitChar = '|';
	}
}
