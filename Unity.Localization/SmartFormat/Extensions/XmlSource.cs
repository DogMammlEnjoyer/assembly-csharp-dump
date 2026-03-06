using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine.Localization.SmartFormat.Core.Extensions;

namespace UnityEngine.Localization.SmartFormat.Extensions
{
	[Serializable]
	public class XmlSource : ISource
	{
		public XmlSource(SmartFormatter formatter)
		{
			formatter.Parser.AddAlphanumericSelectors();
			formatter.Parser.AddAdditionalSelectorChars("_");
			formatter.Parser.AddOperators(".");
		}

		public bool TryEvaluateSelector(ISelectorInfo selectorInfo)
		{
			XElement xelement = selectorInfo.CurrentValue as XElement;
			if (xelement != null)
			{
				string selector = selectorInfo.SelectorText;
				List<XElement> list = (from x in xelement.Elements()
				where x.Name.LocalName == selector
				select x).ToList<XElement>();
				if (list.Any<XElement>())
				{
					selectorInfo.Result = list;
					return true;
				}
			}
			return false;
		}
	}
}
