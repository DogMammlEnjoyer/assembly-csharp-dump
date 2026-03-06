using System;
using System.Collections.Generic;
using System.Xml;

namespace System.Data
{
	internal sealed class XmlIgnoreNamespaceReader : XmlNodeReader
	{
		internal XmlIgnoreNamespaceReader(XmlDocument xdoc, string[] namespacesToIgnore) : base(xdoc)
		{
			this._namespacesToIgnore = new List<string>(namespacesToIgnore);
		}

		public override bool MoveToFirstAttribute()
		{
			return base.MoveToFirstAttribute() && ((!this._namespacesToIgnore.Contains(this.NamespaceURI) && (!(this.NamespaceURI == "http://www.w3.org/XML/1998/namespace") || !(this.LocalName != "lang"))) || this.MoveToNextAttribute());
		}

		public override bool MoveToNextAttribute()
		{
			bool result;
			bool flag;
			do
			{
				result = false;
				flag = false;
				if (base.MoveToNextAttribute())
				{
					result = true;
					if (this._namespacesToIgnore.Contains(this.NamespaceURI) || (this.NamespaceURI == "http://www.w3.org/XML/1998/namespace" && this.LocalName != "lang"))
					{
						flag = true;
					}
				}
			}
			while (flag);
			return result;
		}

		private List<string> _namespacesToIgnore;
	}
}
