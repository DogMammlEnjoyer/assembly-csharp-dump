using System;

namespace System.Xml.Schema
{
	internal class Datatype_char : Datatype_anySimpleType
	{
		public override Type ValueType
		{
			get
			{
				return Datatype_char.atomicValueType;
			}
		}

		internal override Type ListValueType
		{
			get
			{
				return Datatype_char.listValueType;
			}
		}

		internal override RestrictionFlags ValidRestrictionFlags
		{
			get
			{
				return (RestrictionFlags)0;
			}
		}

		internal override int Compare(object value1, object value2)
		{
			return ((char)value1).CompareTo(value2);
		}

		public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
		{
			object result;
			try
			{
				result = XmlConvert.ToChar(s);
			}
			catch (XmlSchemaException ex)
			{
				throw ex;
			}
			catch (Exception innerException)
			{
				throw new XmlSchemaException(Res.GetString("The value '{0}' is invalid according to its data type.", new object[]
				{
					s
				}), innerException);
			}
			return result;
		}

		internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
		{
			typedValue = null;
			char c;
			Exception ex = XmlConvert.TryToChar(s, out c);
			if (ex == null)
			{
				typedValue = c;
				return null;
			}
			return ex;
		}

		private static readonly Type atomicValueType = typeof(char);

		private static readonly Type listValueType = typeof(char[]);
	}
}
