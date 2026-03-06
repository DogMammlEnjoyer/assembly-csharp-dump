using System;
using System.Collections;
using System.Xml.XPath;
using MS.Internal.Xml.XPath;

namespace System.Xml.Schema
{
	internal class Asttree
	{
		internal ArrayList SubtreeArray
		{
			get
			{
				return this._fAxisArray;
			}
		}

		public Asttree(string xPath, bool isField, XmlNamespaceManager nsmgr)
		{
			this._xpathexpr = xPath;
			this._isField = isField;
			this._nsmgr = nsmgr;
			this.CompileXPath(xPath, isField, nsmgr);
		}

		private static bool IsNameTest(Axis ast)
		{
			return ast.TypeOfAxis == Axis.AxisType.Child && ast.NodeType == XPathNodeType.Element;
		}

		internal static bool IsAttribute(Axis ast)
		{
			return ast.TypeOfAxis == Axis.AxisType.Attribute && ast.NodeType == XPathNodeType.Attribute;
		}

		private static bool IsDescendantOrSelf(Axis ast)
		{
			return ast.TypeOfAxis == Axis.AxisType.DescendantOrSelf && ast.NodeType == XPathNodeType.All && ast.AbbrAxis;
		}

		internal static bool IsSelf(Axis ast)
		{
			return ast.TypeOfAxis == Axis.AxisType.Self && ast.NodeType == XPathNodeType.All && ast.AbbrAxis;
		}

		public void CompileXPath(string xPath, bool isField, XmlNamespaceManager nsmgr)
		{
			if (xPath == null || xPath.Length == 0)
			{
				throw new XmlSchemaException("The XPath for selector or field cannot be empty.", string.Empty);
			}
			string[] array = xPath.Split('|', StringSplitOptions.None);
			ArrayList arrayList = new ArrayList(array.Length);
			this._fAxisArray = new ArrayList(array.Length);
			try
			{
				for (int i = 0; i < array.Length; i++)
				{
					Axis value = (Axis)XPathParser.ParseXPathExpression(array[i]);
					arrayList.Add(value);
				}
			}
			catch
			{
				throw new XmlSchemaException("'{0}' is an invalid XPath for selector or field.", xPath);
			}
			int j = 0;
			while (j < arrayList.Count)
			{
				Axis axis = (Axis)arrayList[j];
				Axis axis2;
				if ((axis2 = axis) == null)
				{
					throw new XmlSchemaException("'{0}' is an invalid XPath for selector or field.", xPath);
				}
				Axis axis3 = axis2;
				if (Asttree.IsAttribute(axis2))
				{
					if (!isField)
					{
						throw new XmlSchemaException("'{0}' is an invalid XPath for selector. Selector cannot have an XPath selection with an attribute node.", xPath);
					}
					this.SetURN(axis2, nsmgr);
					try
					{
						axis2 = (Axis)axis2.Input;
						goto IL_122;
					}
					catch
					{
						throw new XmlSchemaException("'{0}' is an invalid XPath for selector or field.", xPath);
					}
					goto IL_D7;
				}
				IL_122:
				if (axis2 == null || (!Asttree.IsNameTest(axis2) && !Asttree.IsSelf(axis2)))
				{
					axis3.Input = null;
					if (axis2 == null)
					{
						if (Asttree.IsSelf(axis) && axis.Input != null)
						{
							this._fAxisArray.Add(new ForwardAxis(DoubleLinkAxis.ConvertTree((Axis)axis.Input), false));
						}
						else
						{
							this._fAxisArray.Add(new ForwardAxis(DoubleLinkAxis.ConvertTree(axis), false));
						}
					}
					else
					{
						if (!Asttree.IsDescendantOrSelf(axis2))
						{
							throw new XmlSchemaException("'{0}' is an invalid XPath for selector or field.", xPath);
						}
						try
						{
							axis2 = (Axis)axis2.Input;
						}
						catch
						{
							throw new XmlSchemaException("'{0}' is an invalid XPath for selector or field.", xPath);
						}
						if (axis2 == null || !Asttree.IsSelf(axis2) || axis2.Input != null)
						{
							throw new XmlSchemaException("'{0}' is an invalid XPath for selector or field.", xPath);
						}
						if (Asttree.IsSelf(axis) && axis.Input != null)
						{
							this._fAxisArray.Add(new ForwardAxis(DoubleLinkAxis.ConvertTree((Axis)axis.Input), true));
						}
						else
						{
							this._fAxisArray.Add(new ForwardAxis(DoubleLinkAxis.ConvertTree(axis), true));
						}
					}
					j++;
					continue;
				}
				IL_D7:
				if (Asttree.IsSelf(axis2) && axis != axis2)
				{
					axis3.Input = axis2.Input;
				}
				else
				{
					axis3 = axis2;
					if (Asttree.IsNameTest(axis2))
					{
						this.SetURN(axis2, nsmgr);
					}
				}
				try
				{
					axis2 = (Axis)axis2.Input;
				}
				catch
				{
					throw new XmlSchemaException("'{0}' is an invalid XPath for selector or field.", xPath);
				}
				goto IL_122;
			}
		}

		private void SetURN(Axis axis, XmlNamespaceManager nsmgr)
		{
			if (axis.Prefix.Length != 0)
			{
				axis.Urn = nsmgr.LookupNamespace(axis.Prefix);
				if (axis.Urn == null)
				{
					throw new XmlSchemaException("The prefix '{0}' in XPath cannot be resolved.", axis.Prefix);
				}
			}
			else
			{
				if (axis.Name.Length != 0)
				{
					axis.Urn = null;
					return;
				}
				axis.Urn = "";
			}
		}

		private ArrayList _fAxisArray;

		private string _xpathexpr;

		private bool _isField;

		private XmlNamespaceManager _nsmgr;
	}
}
