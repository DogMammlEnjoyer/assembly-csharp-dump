using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.Core.Parsing;

namespace UnityEngine.Localization.SmartFormat.Extensions
{
	[Serializable]
	public class XElementFormatter : FormatterBase
	{
		public XElementFormatter()
		{
			base.Names = this.DefaultNames;
		}

		public override string[] DefaultNames
		{
			get
			{
				return new string[]
				{
					"xelement",
					"xml",
					"x",
					""
				};
			}
		}

		public override bool TryEvaluateFormat(IFormattingInfo formattingInfo)
		{
			Format format = formattingInfo.Format;
			object currentValue = formattingInfo.CurrentValue;
			XElement xelement = null;
			if (format != null && format.HasNested)
			{
				return false;
			}
			IList<XElement> list = currentValue as IList<XElement>;
			if (list != null && list.Count > 0)
			{
				xelement = list[0];
			}
			XElement xelement2 = xelement ?? (currentValue as XElement);
			if (xelement2 != null)
			{
				formattingInfo.Write(xelement2.Value);
				return true;
			}
			return false;
		}
	}
}
