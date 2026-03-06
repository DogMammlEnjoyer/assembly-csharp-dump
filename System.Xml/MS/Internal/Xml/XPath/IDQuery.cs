using System;
using System.Xml;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal sealed class IDQuery : CacheOutputQuery
	{
		public IDQuery(Query arg) : base(arg)
		{
		}

		private IDQuery(IDQuery other) : base(other)
		{
		}

		public override object Evaluate(XPathNodeIterator context)
		{
			object obj = base.Evaluate(context);
			XPathNavigator contextNode = context.Current.Clone();
			switch (base.GetXPathType(obj))
			{
			case XPathResultType.Number:
				this.ProcessIds(contextNode, StringFunctions.toString((double)obj));
				break;
			case XPathResultType.String:
				this.ProcessIds(contextNode, (string)obj);
				break;
			case XPathResultType.Boolean:
				this.ProcessIds(contextNode, StringFunctions.toString((bool)obj));
				break;
			case XPathResultType.NodeSet:
			{
				XPathNavigator xpathNavigator;
				while ((xpathNavigator = this.input.Advance()) != null)
				{
					this.ProcessIds(contextNode, xpathNavigator.Value);
				}
				break;
			}
			case (XPathResultType)4:
				this.ProcessIds(contextNode, ((XPathNavigator)obj).Value);
				break;
			}
			return this;
		}

		private void ProcessIds(XPathNavigator contextNode, string val)
		{
			string[] array = XmlConvert.SplitString(val);
			for (int i = 0; i < array.Length; i++)
			{
				if (contextNode.MoveToId(array[i]))
				{
					Query.Insert(this.outputBuffer, contextNode);
				}
			}
		}

		public override XPathNavigator MatchNode(XPathNavigator context)
		{
			this.Evaluate(new XPathSingletonIterator(context, true));
			XPathNavigator xpathNavigator;
			while ((xpathNavigator = this.Advance()) != null)
			{
				if (xpathNavigator.IsSamePosition(context))
				{
					return context;
				}
			}
			return null;
		}

		public override XPathNodeIterator Clone()
		{
			return new IDQuery(this);
		}
	}
}
