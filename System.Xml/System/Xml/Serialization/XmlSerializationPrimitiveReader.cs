using System;

namespace System.Xml.Serialization
{
	internal class XmlSerializationPrimitiveReader : XmlSerializationReader
	{
		internal object Read_string()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id1_string || base.Reader.NamespaceURI != this.id2_Item)
				{
					throw base.CreateUnknownNodeException();
				}
				if (base.ReadNull())
				{
					result = null;
				}
				else
				{
					result = base.Reader.ReadElementString();
				}
			}
			else
			{
				base.UnknownNode(null);
			}
			return result;
		}

		internal object Read_int()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id3_int || base.Reader.NamespaceURI != this.id2_Item)
				{
					throw base.CreateUnknownNodeException();
				}
				result = XmlConvert.ToInt32(base.Reader.ReadElementString());
			}
			else
			{
				base.UnknownNode(null);
			}
			return result;
		}

		internal object Read_boolean()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id4_boolean || base.Reader.NamespaceURI != this.id2_Item)
				{
					throw base.CreateUnknownNodeException();
				}
				result = XmlConvert.ToBoolean(base.Reader.ReadElementString());
			}
			else
			{
				base.UnknownNode(null);
			}
			return result;
		}

		internal object Read_short()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id5_short || base.Reader.NamespaceURI != this.id2_Item)
				{
					throw base.CreateUnknownNodeException();
				}
				result = XmlConvert.ToInt16(base.Reader.ReadElementString());
			}
			else
			{
				base.UnknownNode(null);
			}
			return result;
		}

		internal object Read_long()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id6_long || base.Reader.NamespaceURI != this.id2_Item)
				{
					throw base.CreateUnknownNodeException();
				}
				result = XmlConvert.ToInt64(base.Reader.ReadElementString());
			}
			else
			{
				base.UnknownNode(null);
			}
			return result;
		}

		internal object Read_float()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id7_float || base.Reader.NamespaceURI != this.id2_Item)
				{
					throw base.CreateUnknownNodeException();
				}
				result = XmlConvert.ToSingle(base.Reader.ReadElementString());
			}
			else
			{
				base.UnknownNode(null);
			}
			return result;
		}

		internal object Read_double()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id8_double || base.Reader.NamespaceURI != this.id2_Item)
				{
					throw base.CreateUnknownNodeException();
				}
				result = XmlConvert.ToDouble(base.Reader.ReadElementString());
			}
			else
			{
				base.UnknownNode(null);
			}
			return result;
		}

		internal object Read_decimal()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id9_decimal || base.Reader.NamespaceURI != this.id2_Item)
				{
					throw base.CreateUnknownNodeException();
				}
				result = XmlConvert.ToDecimal(base.Reader.ReadElementString());
			}
			else
			{
				base.UnknownNode(null);
			}
			return result;
		}

		internal object Read_dateTime()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id10_dateTime || base.Reader.NamespaceURI != this.id2_Item)
				{
					throw base.CreateUnknownNodeException();
				}
				result = XmlSerializationReader.ToDateTime(base.Reader.ReadElementString());
			}
			else
			{
				base.UnknownNode(null);
			}
			return result;
		}

		internal object Read_unsignedByte()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id11_unsignedByte || base.Reader.NamespaceURI != this.id2_Item)
				{
					throw base.CreateUnknownNodeException();
				}
				result = XmlConvert.ToByte(base.Reader.ReadElementString());
			}
			else
			{
				base.UnknownNode(null);
			}
			return result;
		}

		internal object Read_byte()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id12_byte || base.Reader.NamespaceURI != this.id2_Item)
				{
					throw base.CreateUnknownNodeException();
				}
				result = XmlConvert.ToSByte(base.Reader.ReadElementString());
			}
			else
			{
				base.UnknownNode(null);
			}
			return result;
		}

		internal object Read_unsignedShort()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id13_unsignedShort || base.Reader.NamespaceURI != this.id2_Item)
				{
					throw base.CreateUnknownNodeException();
				}
				result = XmlConvert.ToUInt16(base.Reader.ReadElementString());
			}
			else
			{
				base.UnknownNode(null);
			}
			return result;
		}

		internal object Read_unsignedInt()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id14_unsignedInt || base.Reader.NamespaceURI != this.id2_Item)
				{
					throw base.CreateUnknownNodeException();
				}
				result = XmlConvert.ToUInt32(base.Reader.ReadElementString());
			}
			else
			{
				base.UnknownNode(null);
			}
			return result;
		}

		internal object Read_unsignedLong()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id15_unsignedLong || base.Reader.NamespaceURI != this.id2_Item)
				{
					throw base.CreateUnknownNodeException();
				}
				result = XmlConvert.ToUInt64(base.Reader.ReadElementString());
			}
			else
			{
				base.UnknownNode(null);
			}
			return result;
		}

		internal object Read_base64Binary()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id16_base64Binary || base.Reader.NamespaceURI != this.id2_Item)
				{
					throw base.CreateUnknownNodeException();
				}
				if (base.ReadNull())
				{
					result = null;
				}
				else
				{
					result = base.ToByteArrayBase64(false);
				}
			}
			else
			{
				base.UnknownNode(null);
			}
			return result;
		}

		internal object Read_guid()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id17_guid || base.Reader.NamespaceURI != this.id2_Item)
				{
					throw base.CreateUnknownNodeException();
				}
				result = XmlConvert.ToGuid(base.Reader.ReadElementString());
			}
			else
			{
				base.UnknownNode(null);
			}
			return result;
		}

		internal object Read_TimeSpan()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id19_TimeSpan || base.Reader.NamespaceURI != this.id2_Item)
				{
					throw base.CreateUnknownNodeException();
				}
				if (base.Reader.IsEmptyElement)
				{
					base.Reader.Skip();
					result = default(TimeSpan);
				}
				else
				{
					result = XmlConvert.ToTimeSpan(base.Reader.ReadElementString());
				}
			}
			else
			{
				base.UnknownNode(null);
			}
			return result;
		}

		internal object Read_char()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id18_char || base.Reader.NamespaceURI != this.id2_Item)
				{
					throw base.CreateUnknownNodeException();
				}
				result = XmlSerializationReader.ToChar(base.Reader.ReadElementString());
			}
			else
			{
				base.UnknownNode(null);
			}
			return result;
		}

		internal object Read_QName()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id1_QName || base.Reader.NamespaceURI != this.id2_Item)
				{
					throw base.CreateUnknownNodeException();
				}
				if (base.ReadNull())
				{
					result = null;
				}
				else
				{
					result = base.ReadElementQualifiedName();
				}
			}
			else
			{
				base.UnknownNode(null);
			}
			return result;
		}

		protected override void InitCallbacks()
		{
		}

		protected override void InitIDs()
		{
			this.id4_boolean = base.Reader.NameTable.Add("boolean");
			this.id14_unsignedInt = base.Reader.NameTable.Add("unsignedInt");
			this.id15_unsignedLong = base.Reader.NameTable.Add("unsignedLong");
			this.id7_float = base.Reader.NameTable.Add("float");
			this.id10_dateTime = base.Reader.NameTable.Add("dateTime");
			this.id6_long = base.Reader.NameTable.Add("long");
			this.id9_decimal = base.Reader.NameTable.Add("decimal");
			this.id8_double = base.Reader.NameTable.Add("double");
			this.id17_guid = base.Reader.NameTable.Add("guid");
			if (LocalAppContextSwitches.EnableTimeSpanSerialization)
			{
				this.id19_TimeSpan = base.Reader.NameTable.Add("TimeSpan");
			}
			this.id2_Item = base.Reader.NameTable.Add("");
			this.id13_unsignedShort = base.Reader.NameTable.Add("unsignedShort");
			this.id18_char = base.Reader.NameTable.Add("char");
			this.id3_int = base.Reader.NameTable.Add("int");
			this.id12_byte = base.Reader.NameTable.Add("byte");
			this.id16_base64Binary = base.Reader.NameTable.Add("base64Binary");
			this.id11_unsignedByte = base.Reader.NameTable.Add("unsignedByte");
			this.id5_short = base.Reader.NameTable.Add("short");
			this.id1_string = base.Reader.NameTable.Add("string");
			this.id1_QName = base.Reader.NameTable.Add("QName");
		}

		private string id4_boolean;

		private string id14_unsignedInt;

		private string id15_unsignedLong;

		private string id7_float;

		private string id10_dateTime;

		private string id6_long;

		private string id9_decimal;

		private string id8_double;

		private string id17_guid;

		private string id19_TimeSpan;

		private string id2_Item;

		private string id13_unsignedShort;

		private string id18_char;

		private string id3_int;

		private string id12_byte;

		private string id16_base64Binary;

		private string id11_unsignedByte;

		private string id5_short;

		private string id1_string;

		private string id1_QName;
	}
}
