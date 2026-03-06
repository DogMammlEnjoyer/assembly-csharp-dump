using System;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.Core.Parsing;

namespace UnityEngine.Localization.SmartFormat.Extensions
{
	[Serializable]
	public class DefaultFormatter : FormatterBase
	{
		public DefaultFormatter()
		{
			base.Names = this.DefaultNames;
		}

		public override string[] DefaultNames
		{
			get
			{
				return new string[]
				{
					"default",
					"d",
					""
				};
			}
		}

		public override bool TryEvaluateFormat(IFormattingInfo formattingInfo)
		{
			Format format = formattingInfo.Format;
			object obj = formattingInfo.CurrentValue;
			if (format != null && format.HasNested)
			{
				formattingInfo.Write(format, obj);
				return true;
			}
			if (obj == null)
			{
				obj = "";
			}
			IFormatProvider provider = formattingInfo.FormatDetails.Provider;
			string text;
			if (provider != null)
			{
				ICustomFormatter customFormatter = provider.GetFormat(typeof(ICustomFormatter)) as ICustomFormatter;
				if (customFormatter != null)
				{
					string format2 = (format != null) ? format.GetLiteralText() : null;
					text = customFormatter.Format(format2, obj, provider);
					goto IL_A0;
				}
			}
			IFormattable formattable = obj as IFormattable;
			if (formattable != null)
			{
				string format3 = (format != null) ? format.ToString() : null;
				text = formattable.ToString(format3, provider);
			}
			else
			{
				text = obj.ToString();
			}
			IL_A0:
			if (formattingInfo.Alignment > 0)
			{
				int num = formattingInfo.Alignment - text.Length;
				if (num > 0)
				{
					formattingInfo.Write(new string(' ', num));
				}
			}
			formattingInfo.Write(text);
			if (formattingInfo.Alignment < 0)
			{
				int num2 = -formattingInfo.Alignment - text.Length;
				if (num2 > 0)
				{
					formattingInfo.Write(new string(' ', num2));
				}
			}
			return true;
		}
	}
}
