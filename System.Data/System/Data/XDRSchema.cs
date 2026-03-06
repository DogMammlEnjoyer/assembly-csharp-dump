using System;
using System.Collections;
using System.Data.Common;
using System.Globalization;
using System.Xml;

namespace System.Data
{
	internal sealed class XDRSchema : XMLSchema
	{
		internal XDRSchema(DataSet ds, bool fInline)
		{
			this._schemaUri = string.Empty;
			this._schemaName = string.Empty;
			this._schemaRoot = null;
			this._ds = ds;
		}

		internal void LoadSchema(XmlElement schemaRoot, DataSet ds)
		{
			if (schemaRoot == null)
			{
				return;
			}
			this._schemaRoot = schemaRoot;
			this._ds = ds;
			this._schemaName = schemaRoot.GetAttribute("name");
			this._schemaUri = string.Empty;
			if (this._schemaName == null || this._schemaName.Length == 0)
			{
				this._schemaName = "NewDataSet";
			}
			ds.Namespace = this._schemaUri;
			for (XmlNode xmlNode = schemaRoot.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode is XmlElement)
				{
					XmlElement node = (XmlElement)xmlNode;
					if (XMLSchema.FEqualIdentity(node, "ElementType", "urn:schemas-microsoft-com:xml-data"))
					{
						this.HandleTable(node);
					}
				}
			}
			this._schemaName = XmlConvert.DecodeName(this._schemaName);
			if (ds.Tables[this._schemaName] == null)
			{
				ds.DataSetName = this._schemaName;
			}
		}

		internal XmlElement FindTypeNode(XmlElement node)
		{
			if (XMLSchema.FEqualIdentity(node, "ElementType", "urn:schemas-microsoft-com:xml-data"))
			{
				return node;
			}
			string attribute = node.GetAttribute("type");
			if (!XMLSchema.FEqualIdentity(node, "element", "urn:schemas-microsoft-com:xml-data") && !XMLSchema.FEqualIdentity(node, "attribute", "urn:schemas-microsoft-com:xml-data"))
			{
				return null;
			}
			if (attribute == null || attribute.Length == 0)
			{
				return null;
			}
			XmlNode xmlNode = node.OwnerDocument.FirstChild;
			XmlNode ownerDocument = node.OwnerDocument;
			while (xmlNode != ownerDocument)
			{
				if (((XMLSchema.FEqualIdentity(xmlNode, "ElementType", "urn:schemas-microsoft-com:xml-data") && XMLSchema.FEqualIdentity(node, "element", "urn:schemas-microsoft-com:xml-data")) || (XMLSchema.FEqualIdentity(xmlNode, "AttributeType", "urn:schemas-microsoft-com:xml-data") && XMLSchema.FEqualIdentity(node, "attribute", "urn:schemas-microsoft-com:xml-data"))) && xmlNode is XmlElement && ((XmlElement)xmlNode).GetAttribute("name") == attribute)
				{
					return (XmlElement)xmlNode;
				}
				if (xmlNode.FirstChild != null)
				{
					xmlNode = xmlNode.FirstChild;
				}
				else if (xmlNode.NextSibling != null)
				{
					xmlNode = xmlNode.NextSibling;
				}
				else
				{
					while (xmlNode != ownerDocument)
					{
						xmlNode = xmlNode.ParentNode;
						if (xmlNode.NextSibling != null)
						{
							xmlNode = xmlNode.NextSibling;
							break;
						}
					}
				}
			}
			return null;
		}

		internal bool IsTextOnlyContent(XmlElement node)
		{
			string attribute = node.GetAttribute("content");
			if (attribute == null || attribute.Length == 0)
			{
				return !string.IsNullOrEmpty(node.GetAttribute("type", "urn:schemas-microsoft-com:datatypes"));
			}
			if (attribute == "empty" || attribute == "eltOnly" || attribute == "elementOnly" || attribute == "mixed")
			{
				return false;
			}
			if (attribute == "textOnly")
			{
				return true;
			}
			throw ExceptionBuilder.InvalidAttributeValue("content", attribute);
		}

		internal bool IsXDRField(XmlElement node, XmlElement typeNode)
		{
			int num = 1;
			int num2 = 1;
			if (!this.IsTextOnlyContent(typeNode))
			{
				return false;
			}
			for (XmlNode xmlNode = typeNode.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (XMLSchema.FEqualIdentity(xmlNode, "element", "urn:schemas-microsoft-com:xml-data") || XMLSchema.FEqualIdentity(xmlNode, "attribute", "urn:schemas-microsoft-com:xml-data"))
				{
					return false;
				}
			}
			if (XMLSchema.FEqualIdentity(node, "element", "urn:schemas-microsoft-com:xml-data"))
			{
				this.GetMinMax(node, ref num, ref num2);
				if (num2 == -1 || num2 > 1)
				{
					return false;
				}
			}
			return true;
		}

		internal DataTable HandleTable(XmlElement node)
		{
			XmlElement xmlElement = this.FindTypeNode(node);
			string attribute = node.GetAttribute("minOccurs");
			if (attribute != null && attribute.Length > 0 && Convert.ToInt32(attribute, CultureInfo.InvariantCulture) > 1 && xmlElement == null)
			{
				return this.InstantiateSimpleTable(this._ds, node);
			}
			attribute = node.GetAttribute("maxOccurs");
			if (attribute != null && attribute.Length > 0 && !string.Equals(attribute, "1", StringComparison.Ordinal) && xmlElement == null)
			{
				return this.InstantiateSimpleTable(this._ds, node);
			}
			if (xmlElement == null)
			{
				return null;
			}
			if (this.IsXDRField(node, xmlElement))
			{
				return null;
			}
			return this.InstantiateTable(this._ds, node, xmlElement);
		}

		private static XDRSchema.NameType FindNameType(string name)
		{
			int num = Array.BinarySearch(XDRSchema.s_mapNameTypeXdr, name);
			if (num < 0)
			{
				throw ExceptionBuilder.UndefinedDatatype(name);
			}
			return XDRSchema.s_mapNameTypeXdr[num];
		}

		private Type ParseDataType(string dt, string dtValues)
		{
			string name = dt;
			string[] array = dt.Split(XDRSchema.s_colonArray);
			if (array.Length > 2)
			{
				throw ExceptionBuilder.InvalidAttributeValue("type", dt);
			}
			if (array.Length == 2)
			{
				name = array[1];
			}
			XDRSchema.NameType nameType = XDRSchema.FindNameType(name);
			if (nameType == XDRSchema.s_enumerationNameType && (dtValues == null || dtValues.Length == 0))
			{
				throw ExceptionBuilder.MissingAttribute("type", "values");
			}
			return nameType.type;
		}

		internal string GetInstanceName(XmlElement node)
		{
			string attribute;
			if (XMLSchema.FEqualIdentity(node, "ElementType", "urn:schemas-microsoft-com:xml-data") || XMLSchema.FEqualIdentity(node, "AttributeType", "urn:schemas-microsoft-com:xml-data"))
			{
				attribute = node.GetAttribute("name");
				if (attribute == null || attribute.Length == 0)
				{
					throw ExceptionBuilder.MissingAttribute("Element", "name");
				}
			}
			else
			{
				attribute = node.GetAttribute("type");
				if (attribute == null || attribute.Length == 0)
				{
					throw ExceptionBuilder.MissingAttribute("Element", "type");
				}
			}
			return attribute;
		}

		internal void HandleColumn(XmlElement node, DataTable table)
		{
			int num = 0;
			int num2 = 1;
			node.GetAttribute("use");
			string name;
			DataColumn dataColumn;
			if (node.Attributes.Count > 0)
			{
				string attribute = node.GetAttribute("ref");
				if (attribute != null && attribute.Length > 0)
				{
					return;
				}
				string instanceName;
				name = (instanceName = this.GetInstanceName(node));
				dataColumn = table.Columns[name, this._schemaUri];
				if (dataColumn != null)
				{
					if (dataColumn.ColumnMapping == MappingType.Attribute)
					{
						if (XMLSchema.FEqualIdentity(node, "attribute", "urn:schemas-microsoft-com:xml-data"))
						{
							throw ExceptionBuilder.DuplicateDeclaration(instanceName);
						}
					}
					else if (XMLSchema.FEqualIdentity(node, "element", "urn:schemas-microsoft-com:xml-data"))
					{
						throw ExceptionBuilder.DuplicateDeclaration(instanceName);
					}
					name = XMLSchema.GenUniqueColumnName(instanceName, table);
				}
			}
			else
			{
				name = string.Empty;
			}
			XmlElement xmlElement = this.FindTypeNode(node);
			SimpleType simpleType = null;
			string text;
			if (xmlElement == null)
			{
				text = node.GetAttribute("type");
				throw ExceptionBuilder.UndefinedDatatype(text);
			}
			text = xmlElement.GetAttribute("type", "urn:schemas-microsoft-com:datatypes");
			string attribute2 = xmlElement.GetAttribute("values", "urn:schemas-microsoft-com:datatypes");
			Type type;
			if (text == null || text.Length == 0)
			{
				text = string.Empty;
				type = typeof(string);
			}
			else
			{
				type = this.ParseDataType(text, attribute2);
				if (text == "float")
				{
					text = string.Empty;
				}
				if (text == "char")
				{
					text = string.Empty;
					simpleType = SimpleType.CreateSimpleType(StorageType.Char, type);
				}
				if (text == "enumeration")
				{
					text = string.Empty;
					simpleType = SimpleType.CreateEnumeratedType(attribute2);
				}
				if (text == "bin.base64")
				{
					text = string.Empty;
					simpleType = SimpleType.CreateByteArrayType("base64");
				}
				if (text == "bin.hex")
				{
					text = string.Empty;
					simpleType = SimpleType.CreateByteArrayType("hex");
				}
			}
			bool flag = XMLSchema.FEqualIdentity(node, "attribute", "urn:schemas-microsoft-com:xml-data");
			this.GetMinMax(node, flag, ref num, ref num2);
			string text2 = null;
			text2 = node.GetAttribute("default");
			bool flag2 = false;
			dataColumn = new DataColumn(XmlConvert.DecodeName(name), type, null, flag ? MappingType.Attribute : MappingType.Element);
			XMLSchema.SetProperties(dataColumn, node.Attributes);
			dataColumn.XmlDataType = text;
			dataColumn.SimpleType = simpleType;
			dataColumn.AllowDBNull = (num == 0 || flag2);
			dataColumn.Namespace = (flag ? string.Empty : this._schemaUri);
			if (node.Attributes != null)
			{
				for (int i = 0; i < node.Attributes.Count; i++)
				{
					if (node.Attributes[i].NamespaceURI == "urn:schemas-microsoft-com:xml-msdata" && node.Attributes[i].LocalName == "Expression")
					{
						dataColumn.Expression = node.Attributes[i].Value;
						break;
					}
				}
			}
			string attribute3 = node.GetAttribute("targetNamespace");
			if (attribute3 != null && attribute3.Length > 0)
			{
				dataColumn.Namespace = attribute3;
			}
			table.Columns.Add(dataColumn);
			if (text2 != null && text2.Length != 0)
			{
				try
				{
					dataColumn.DefaultValue = SqlConvert.ChangeTypeForXML(text2, type);
				}
				catch (FormatException)
				{
					throw ExceptionBuilder.CannotConvert(text2, type.FullName);
				}
			}
		}

		internal void GetMinMax(XmlElement elNode, ref int minOccurs, ref int maxOccurs)
		{
			this.GetMinMax(elNode, false, ref minOccurs, ref maxOccurs);
		}

		internal void GetMinMax(XmlElement elNode, bool isAttribute, ref int minOccurs, ref int maxOccurs)
		{
			string attribute = elNode.GetAttribute("minOccurs");
			if (attribute != null && attribute.Length > 0)
			{
				try
				{
					minOccurs = int.Parse(attribute, CultureInfo.InvariantCulture);
				}
				catch (Exception e) when (ADP.IsCatchableExceptionType(e))
				{
					throw ExceptionBuilder.AttributeValues("minOccurs", "0", "1");
				}
			}
			attribute = elNode.GetAttribute("maxOccurs");
			if (attribute != null && attribute.Length > 0)
			{
				if (string.Compare(attribute, "*", StringComparison.Ordinal) == 0)
				{
					maxOccurs = -1;
					return;
				}
				try
				{
					maxOccurs = int.Parse(attribute, CultureInfo.InvariantCulture);
				}
				catch (Exception e2) when (ADP.IsCatchableExceptionType(e2))
				{
					throw ExceptionBuilder.AttributeValues("maxOccurs", "1", "*");
				}
				if (maxOccurs != 1)
				{
					throw ExceptionBuilder.AttributeValues("maxOccurs", "1", "*");
				}
			}
		}

		internal void HandleTypeNode(XmlElement typeNode, DataTable table, ArrayList tableChildren)
		{
			for (XmlNode xmlNode = typeNode.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode is XmlElement)
				{
					if (XMLSchema.FEqualIdentity(xmlNode, "element", "urn:schemas-microsoft-com:xml-data"))
					{
						DataTable dataTable = this.HandleTable((XmlElement)xmlNode);
						if (dataTable != null)
						{
							tableChildren.Add(dataTable);
							goto IL_6E;
						}
					}
					if (XMLSchema.FEqualIdentity(xmlNode, "attribute", "urn:schemas-microsoft-com:xml-data") || XMLSchema.FEqualIdentity(xmlNode, "element", "urn:schemas-microsoft-com:xml-data"))
					{
						this.HandleColumn((XmlElement)xmlNode, table);
					}
				}
				IL_6E:;
			}
		}

		internal DataTable InstantiateTable(DataSet dataSet, XmlElement node, XmlElement typeNode)
		{
			string name = string.Empty;
			XmlAttributeCollection attributes = node.Attributes;
			int value = 1;
			int value2 = 1;
			string text = null;
			ArrayList arrayList = new ArrayList();
			DataTable dataTable;
			if (attributes.Count > 0)
			{
				name = this.GetInstanceName(node);
				dataTable = dataSet.Tables.GetTable(name, this._schemaUri);
				if (dataTable != null)
				{
					return dataTable;
				}
			}
			dataTable = new DataTable(XmlConvert.DecodeName(name));
			dataTable.Namespace = this._schemaUri;
			this.GetMinMax(node, ref value, ref value2);
			dataTable.MinOccurs = value;
			dataTable.MaxOccurs = value2;
			this._ds.Tables.Add(dataTable);
			this.HandleTypeNode(typeNode, dataTable, arrayList);
			XMLSchema.SetProperties(dataTable, attributes);
			if (text != null)
			{
				string[] array = text.TrimEnd(null).Split(null);
				int num = array.Length;
				DataColumn[] array2 = new DataColumn[num];
				for (int i = 0; i < num; i++)
				{
					DataColumn dataColumn = dataTable.Columns[array[i], this._schemaUri];
					if (dataColumn == null)
					{
						throw ExceptionBuilder.ElementTypeNotFound(array[i]);
					}
					array2[i] = dataColumn;
				}
				dataTable.PrimaryKey = array2;
			}
			foreach (object obj in arrayList)
			{
				DataTable dataTable2 = (DataTable)obj;
				DataRelation dataRelation = null;
				DataRelationCollection childRelations = dataTable.ChildRelations;
				for (int j = 0; j < childRelations.Count; j++)
				{
					if (childRelations[j].Nested && dataTable2 == childRelations[j].ChildTable)
					{
						dataRelation = childRelations[j];
					}
				}
				if (dataRelation == null)
				{
					DataColumn dataColumn2 = dataTable.AddUniqueKey();
					DataColumn childColumn = dataTable2.AddForeignKey(dataColumn2);
					dataRelation = new DataRelation(dataTable.TableName + "_" + dataTable2.TableName, dataColumn2, childColumn, true);
					dataRelation.CheckMultipleNested = false;
					dataRelation.Nested = true;
					dataTable2.DataSet.Relations.Add(dataRelation);
					dataRelation.CheckMultipleNested = true;
				}
			}
			return dataTable;
		}

		internal DataTable InstantiateSimpleTable(DataSet dataSet, XmlElement node)
		{
			XmlAttributeCollection attributes = node.Attributes;
			int value = 1;
			int value2 = 1;
			string instanceName = this.GetInstanceName(node);
			DataTable dataTable = dataSet.Tables.GetTable(instanceName, this._schemaUri);
			if (dataTable != null)
			{
				throw ExceptionBuilder.DuplicateDeclaration(instanceName);
			}
			string text = XmlConvert.DecodeName(instanceName);
			dataTable = new DataTable(text);
			dataTable.Namespace = this._schemaUri;
			this.GetMinMax(node, ref value, ref value2);
			dataTable.MinOccurs = value;
			dataTable.MaxOccurs = value2;
			XMLSchema.SetProperties(dataTable, attributes);
			dataTable._repeatableElement = true;
			this.HandleColumn(node, dataTable);
			dataTable.Columns[0].ColumnName = text + "_Column";
			this._ds.Tables.Add(dataTable);
			return dataTable;
		}

		internal string _schemaName;

		internal string _schemaUri;

		internal XmlElement _schemaRoot;

		internal DataSet _ds;

		private static readonly char[] s_colonArray = new char[]
		{
			':'
		};

		private static XDRSchema.NameType[] s_mapNameTypeXdr = new XDRSchema.NameType[]
		{
			new XDRSchema.NameType("bin.base64", typeof(byte[])),
			new XDRSchema.NameType("bin.hex", typeof(byte[])),
			new XDRSchema.NameType("boolean", typeof(bool)),
			new XDRSchema.NameType("byte", typeof(sbyte)),
			new XDRSchema.NameType("char", typeof(char)),
			new XDRSchema.NameType("date", typeof(DateTime)),
			new XDRSchema.NameType("dateTime", typeof(DateTime)),
			new XDRSchema.NameType("dateTime.tz", typeof(DateTime)),
			new XDRSchema.NameType("entities", typeof(string)),
			new XDRSchema.NameType("entity", typeof(string)),
			new XDRSchema.NameType("enumeration", typeof(string)),
			new XDRSchema.NameType("fixed.14.4", typeof(decimal)),
			new XDRSchema.NameType("float", typeof(double)),
			new XDRSchema.NameType("i1", typeof(sbyte)),
			new XDRSchema.NameType("i2", typeof(short)),
			new XDRSchema.NameType("i4", typeof(int)),
			new XDRSchema.NameType("i8", typeof(long)),
			new XDRSchema.NameType("id", typeof(string)),
			new XDRSchema.NameType("idref", typeof(string)),
			new XDRSchema.NameType("idrefs", typeof(string)),
			new XDRSchema.NameType("int", typeof(int)),
			new XDRSchema.NameType("nmtoken", typeof(string)),
			new XDRSchema.NameType("nmtokens", typeof(string)),
			new XDRSchema.NameType("notation", typeof(string)),
			new XDRSchema.NameType("number", typeof(decimal)),
			new XDRSchema.NameType("r4", typeof(float)),
			new XDRSchema.NameType("r8", typeof(double)),
			new XDRSchema.NameType("string", typeof(string)),
			new XDRSchema.NameType("time", typeof(DateTime)),
			new XDRSchema.NameType("time.tz", typeof(DateTime)),
			new XDRSchema.NameType("ui1", typeof(byte)),
			new XDRSchema.NameType("ui2", typeof(ushort)),
			new XDRSchema.NameType("ui4", typeof(uint)),
			new XDRSchema.NameType("ui8", typeof(ulong)),
			new XDRSchema.NameType("uri", typeof(string)),
			new XDRSchema.NameType("uuid", typeof(Guid))
		};

		private static XDRSchema.NameType s_enumerationNameType = XDRSchema.FindNameType("enumeration");

		private sealed class NameType : IComparable
		{
			public NameType(string n, Type t)
			{
				this.name = n;
				this.type = t;
			}

			public int CompareTo(object obj)
			{
				return string.Compare(this.name, (string)obj, StringComparison.Ordinal);
			}

			public string name;

			public Type type;
		}
	}
}
