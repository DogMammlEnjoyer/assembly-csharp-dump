using System;
using System.Collections;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal sealed class XPathComparerHelper : IComparer
	{
		public XPathComparerHelper(XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType)
		{
			if (lang == null)
			{
				this._cinfo = CultureInfo.CurrentCulture;
			}
			else
			{
				try
				{
					this._cinfo = new CultureInfo(lang);
				}
				catch (ArgumentException)
				{
					throw;
				}
			}
			if (order == XmlSortOrder.Descending)
			{
				if (caseOrder == XmlCaseOrder.LowerFirst)
				{
					caseOrder = XmlCaseOrder.UpperFirst;
				}
				else if (caseOrder == XmlCaseOrder.UpperFirst)
				{
					caseOrder = XmlCaseOrder.LowerFirst;
				}
			}
			this._order = order;
			this._caseOrder = caseOrder;
			this._dataType = dataType;
		}

		public int Compare(object x, object y)
		{
			XmlDataType dataType = this._dataType;
			if (dataType != XmlDataType.Text)
			{
				if (dataType != XmlDataType.Number)
				{
					throw new InvalidOperationException("Operation is not valid due to the current state of the object.");
				}
				double num = XmlConvert.ToXPathDouble(x);
				double value = XmlConvert.ToXPathDouble(y);
				int num2 = num.CompareTo(value);
				if (this._order != XmlSortOrder.Ascending)
				{
					return -num2;
				}
				return num2;
			}
			else
			{
				string @string = Convert.ToString(x, this._cinfo);
				string string2 = Convert.ToString(y, this._cinfo);
				int num2 = this._cinfo.CompareInfo.Compare(@string, string2, (this._caseOrder != XmlCaseOrder.None) ? CompareOptions.IgnoreCase : CompareOptions.None);
				if (num2 != 0 || this._caseOrder == XmlCaseOrder.None)
				{
					if (this._order != XmlSortOrder.Ascending)
					{
						return -num2;
					}
					return num2;
				}
				else
				{
					num2 = this._cinfo.CompareInfo.Compare(@string, string2);
					if (this._caseOrder != XmlCaseOrder.LowerFirst)
					{
						return -num2;
					}
					return num2;
				}
			}
		}

		private XmlSortOrder _order;

		private XmlCaseOrder _caseOrder;

		private CultureInfo _cinfo;

		private XmlDataType _dataType;
	}
}
