using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace System.Xml
{
	internal class XsdValidatingReader : XmlReader, IXmlSchemaInfo, IXmlLineInfo, IXmlNamespaceResolver
	{
		internal XsdValidatingReader(XmlReader reader, XmlResolver xmlResolver, XmlReaderSettings readerSettings, XmlSchemaObject partialValidationType)
		{
			this.coreReader = reader;
			this.coreReaderNSResolver = (reader as IXmlNamespaceResolver);
			this.lineInfo = (reader as IXmlLineInfo);
			this.coreReaderNameTable = this.coreReader.NameTable;
			if (this.coreReaderNSResolver == null)
			{
				this.nsManager = new XmlNamespaceManager(this.coreReaderNameTable);
				this.manageNamespaces = true;
			}
			this.thisNSResolver = this;
			this.xmlResolver = xmlResolver;
			this.processInlineSchema = ((readerSettings.ValidationFlags & XmlSchemaValidationFlags.ProcessInlineSchema) > XmlSchemaValidationFlags.None);
			this.Init();
			this.SetupValidator(readerSettings, reader, partialValidationType);
			this.validationEvent = readerSettings.GetEventHandler();
		}

		internal XsdValidatingReader(XmlReader reader, XmlResolver xmlResolver, XmlReaderSettings readerSettings) : this(reader, xmlResolver, readerSettings, null)
		{
		}

		private void Init()
		{
			this.validationState = XsdValidatingReader.ValidatingReaderState.Init;
			this.defaultAttributes = new ArrayList();
			this.currentAttrIndex = -1;
			this.attributePSVINodes = new AttributePSVIInfo[8];
			this.valueGetter = new XmlValueGetter(this.GetStringValue);
			XsdValidatingReader.TypeOfString = typeof(string);
			this.xmlSchemaInfo = new XmlSchemaInfo();
			this.NsXmlNs = this.coreReaderNameTable.Add("http://www.w3.org/2000/xmlns/");
			this.NsXs = this.coreReaderNameTable.Add("http://www.w3.org/2001/XMLSchema");
			this.NsXsi = this.coreReaderNameTable.Add("http://www.w3.org/2001/XMLSchema-instance");
			this.XsiType = this.coreReaderNameTable.Add("type");
			this.XsiNil = this.coreReaderNameTable.Add("nil");
			this.XsiSchemaLocation = this.coreReaderNameTable.Add("schemaLocation");
			this.XsiNoNamespaceSchemaLocation = this.coreReaderNameTable.Add("noNamespaceSchemaLocation");
			this.XsdSchema = this.coreReaderNameTable.Add("schema");
		}

		private void SetupValidator(XmlReaderSettings readerSettings, XmlReader reader, XmlSchemaObject partialValidationType)
		{
			this.validator = new XmlSchemaValidator(this.coreReaderNameTable, readerSettings.Schemas, this.thisNSResolver, readerSettings.ValidationFlags);
			this.validator.XmlResolver = this.xmlResolver;
			this.validator.SourceUri = XmlConvert.ToUri(reader.BaseURI);
			this.validator.ValidationEventSender = this;
			this.validator.ValidationEventHandler += readerSettings.GetEventHandler();
			this.validator.LineInfoProvider = this.lineInfo;
			if (this.validator.ProcessSchemaHints)
			{
				this.validator.SchemaSet.ReaderSettings.DtdProcessing = readerSettings.DtdProcessing;
			}
			this.validator.SetDtdSchemaInfo(reader.DtdInfo);
			if (partialValidationType != null)
			{
				this.validator.Initialize(partialValidationType);
				return;
			}
			this.validator.Initialize();
		}

		public override XmlReaderSettings Settings
		{
			get
			{
				XmlReaderSettings xmlReaderSettings = this.coreReader.Settings;
				if (xmlReaderSettings != null)
				{
					xmlReaderSettings = xmlReaderSettings.Clone();
				}
				if (xmlReaderSettings == null)
				{
					xmlReaderSettings = new XmlReaderSettings();
				}
				xmlReaderSettings.Schemas = this.validator.SchemaSet;
				xmlReaderSettings.ValidationType = ValidationType.Schema;
				xmlReaderSettings.ValidationFlags = this.validator.ValidationFlags;
				xmlReaderSettings.ReadOnly = true;
				return xmlReaderSettings;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				if (this.validationState < XsdValidatingReader.ValidatingReaderState.None)
				{
					return this.cachedNode.NodeType;
				}
				XmlNodeType nodeType = this.coreReader.NodeType;
				if (nodeType == XmlNodeType.Whitespace && (this.validator.CurrentContentType == XmlSchemaContentType.TextOnly || this.validator.CurrentContentType == XmlSchemaContentType.Mixed))
				{
					return XmlNodeType.SignificantWhitespace;
				}
				return nodeType;
			}
		}

		public override string Name
		{
			get
			{
				if (this.validationState != XsdValidatingReader.ValidatingReaderState.OnDefaultAttribute)
				{
					return this.coreReader.Name;
				}
				string defaultAttributePrefix = this.validator.GetDefaultAttributePrefix(this.cachedNode.Namespace);
				if (defaultAttributePrefix != null && defaultAttributePrefix.Length != 0)
				{
					return string.Concat(new string[]
					{
						defaultAttributePrefix + ":" + this.cachedNode.LocalName
					});
				}
				return this.cachedNode.LocalName;
			}
		}

		public override string LocalName
		{
			get
			{
				if (this.validationState < XsdValidatingReader.ValidatingReaderState.None)
				{
					return this.cachedNode.LocalName;
				}
				return this.coreReader.LocalName;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				if (this.validationState < XsdValidatingReader.ValidatingReaderState.None)
				{
					return this.cachedNode.Namespace;
				}
				return this.coreReader.NamespaceURI;
			}
		}

		public override string Prefix
		{
			get
			{
				if (this.validationState < XsdValidatingReader.ValidatingReaderState.None)
				{
					return this.cachedNode.Prefix;
				}
				return this.coreReader.Prefix;
			}
		}

		public override bool HasValue
		{
			get
			{
				return this.validationState < XsdValidatingReader.ValidatingReaderState.None || this.coreReader.HasValue;
			}
		}

		public override string Value
		{
			get
			{
				if (this.validationState < XsdValidatingReader.ValidatingReaderState.None)
				{
					return this.cachedNode.RawValue;
				}
				return this.coreReader.Value;
			}
		}

		public override int Depth
		{
			get
			{
				if (this.validationState < XsdValidatingReader.ValidatingReaderState.None)
				{
					return this.cachedNode.Depth;
				}
				return this.coreReader.Depth;
			}
		}

		public override string BaseURI
		{
			get
			{
				return this.coreReader.BaseURI;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return this.coreReader.IsEmptyElement;
			}
		}

		public override bool IsDefault
		{
			get
			{
				return this.validationState == XsdValidatingReader.ValidatingReaderState.OnDefaultAttribute || this.coreReader.IsDefault;
			}
		}

		public override char QuoteChar
		{
			get
			{
				return this.coreReader.QuoteChar;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return this.coreReader.XmlSpace;
			}
		}

		public override string XmlLang
		{
			get
			{
				return this.coreReader.XmlLang;
			}
		}

		public override IXmlSchemaInfo SchemaInfo
		{
			get
			{
				return this;
			}
		}

		public override Type ValueType
		{
			get
			{
				XmlNodeType nodeType = this.NodeType;
				if (nodeType != XmlNodeType.Element)
				{
					if (nodeType != XmlNodeType.Attribute)
					{
						if (nodeType != XmlNodeType.EndElement)
						{
							goto IL_62;
						}
					}
					else
					{
						if (this.attributePSVI != null && this.AttributeSchemaInfo.ContentType == XmlSchemaContentType.TextOnly)
						{
							return this.AttributeSchemaInfo.SchemaType.Datatype.ValueType;
						}
						goto IL_62;
					}
				}
				if (this.xmlSchemaInfo.ContentType == XmlSchemaContentType.TextOnly)
				{
					return this.xmlSchemaInfo.SchemaType.Datatype.ValueType;
				}
				IL_62:
				return XsdValidatingReader.TypeOfString;
			}
		}

		public override object ReadContentAsObject()
		{
			if (!XmlReader.CanReadContentAs(this.NodeType))
			{
				throw base.CreateReadContentAsException("ReadContentAsObject");
			}
			return this.InternalReadContentAsObject(true);
		}

		public override bool ReadContentAsBoolean()
		{
			if (!XmlReader.CanReadContentAs(this.NodeType))
			{
				throw base.CreateReadContentAsException("ReadContentAsBoolean");
			}
			object value = this.InternalReadContentAsObject();
			XmlSchemaType xmlSchemaType = (this.NodeType == XmlNodeType.Attribute) ? this.AttributeXmlType : this.ElementXmlType;
			bool result;
			try
			{
				if (xmlSchemaType != null)
				{
					result = xmlSchemaType.ValueConverter.ToBoolean(value);
				}
				else
				{
					result = XmlUntypedConverter.Untyped.ToBoolean(value);
				}
			}
			catch (InvalidCastException innerException)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Boolean", innerException, this);
			}
			catch (FormatException innerException2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Boolean", innerException2, this);
			}
			catch (OverflowException innerException3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Boolean", innerException3, this);
			}
			return result;
		}

		public override DateTime ReadContentAsDateTime()
		{
			if (!XmlReader.CanReadContentAs(this.NodeType))
			{
				throw base.CreateReadContentAsException("ReadContentAsDateTime");
			}
			object value = this.InternalReadContentAsObject();
			XmlSchemaType xmlSchemaType = (this.NodeType == XmlNodeType.Attribute) ? this.AttributeXmlType : this.ElementXmlType;
			DateTime result;
			try
			{
				if (xmlSchemaType != null)
				{
					result = xmlSchemaType.ValueConverter.ToDateTime(value);
				}
				else
				{
					result = XmlUntypedConverter.Untyped.ToDateTime(value);
				}
			}
			catch (InvalidCastException innerException)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "DateTime", innerException, this);
			}
			catch (FormatException innerException2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "DateTime", innerException2, this);
			}
			catch (OverflowException innerException3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "DateTime", innerException3, this);
			}
			return result;
		}

		public override double ReadContentAsDouble()
		{
			if (!XmlReader.CanReadContentAs(this.NodeType))
			{
				throw base.CreateReadContentAsException("ReadContentAsDouble");
			}
			object value = this.InternalReadContentAsObject();
			XmlSchemaType xmlSchemaType = (this.NodeType == XmlNodeType.Attribute) ? this.AttributeXmlType : this.ElementXmlType;
			double result;
			try
			{
				if (xmlSchemaType != null)
				{
					result = xmlSchemaType.ValueConverter.ToDouble(value);
				}
				else
				{
					result = XmlUntypedConverter.Untyped.ToDouble(value);
				}
			}
			catch (InvalidCastException innerException)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Double", innerException, this);
			}
			catch (FormatException innerException2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Double", innerException2, this);
			}
			catch (OverflowException innerException3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Double", innerException3, this);
			}
			return result;
		}

		public override float ReadContentAsFloat()
		{
			if (!XmlReader.CanReadContentAs(this.NodeType))
			{
				throw base.CreateReadContentAsException("ReadContentAsFloat");
			}
			object value = this.InternalReadContentAsObject();
			XmlSchemaType xmlSchemaType = (this.NodeType == XmlNodeType.Attribute) ? this.AttributeXmlType : this.ElementXmlType;
			float result;
			try
			{
				if (xmlSchemaType != null)
				{
					result = xmlSchemaType.ValueConverter.ToSingle(value);
				}
				else
				{
					result = XmlUntypedConverter.Untyped.ToSingle(value);
				}
			}
			catch (InvalidCastException innerException)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Float", innerException, this);
			}
			catch (FormatException innerException2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Float", innerException2, this);
			}
			catch (OverflowException innerException3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Float", innerException3, this);
			}
			return result;
		}

		public override decimal ReadContentAsDecimal()
		{
			if (!XmlReader.CanReadContentAs(this.NodeType))
			{
				throw base.CreateReadContentAsException("ReadContentAsDecimal");
			}
			object value = this.InternalReadContentAsObject();
			XmlSchemaType xmlSchemaType = (this.NodeType == XmlNodeType.Attribute) ? this.AttributeXmlType : this.ElementXmlType;
			decimal result;
			try
			{
				if (xmlSchemaType != null)
				{
					result = xmlSchemaType.ValueConverter.ToDecimal(value);
				}
				else
				{
					result = XmlUntypedConverter.Untyped.ToDecimal(value);
				}
			}
			catch (InvalidCastException innerException)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Decimal", innerException, this);
			}
			catch (FormatException innerException2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Decimal", innerException2, this);
			}
			catch (OverflowException innerException3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Decimal", innerException3, this);
			}
			return result;
		}

		public override int ReadContentAsInt()
		{
			if (!XmlReader.CanReadContentAs(this.NodeType))
			{
				throw base.CreateReadContentAsException("ReadContentAsInt");
			}
			object value = this.InternalReadContentAsObject();
			XmlSchemaType xmlSchemaType = (this.NodeType == XmlNodeType.Attribute) ? this.AttributeXmlType : this.ElementXmlType;
			int result;
			try
			{
				if (xmlSchemaType != null)
				{
					result = xmlSchemaType.ValueConverter.ToInt32(value);
				}
				else
				{
					result = XmlUntypedConverter.Untyped.ToInt32(value);
				}
			}
			catch (InvalidCastException innerException)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Int", innerException, this);
			}
			catch (FormatException innerException2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Int", innerException2, this);
			}
			catch (OverflowException innerException3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Int", innerException3, this);
			}
			return result;
		}

		public override long ReadContentAsLong()
		{
			if (!XmlReader.CanReadContentAs(this.NodeType))
			{
				throw base.CreateReadContentAsException("ReadContentAsLong");
			}
			object value = this.InternalReadContentAsObject();
			XmlSchemaType xmlSchemaType = (this.NodeType == XmlNodeType.Attribute) ? this.AttributeXmlType : this.ElementXmlType;
			long result;
			try
			{
				if (xmlSchemaType != null)
				{
					result = xmlSchemaType.ValueConverter.ToInt64(value);
				}
				else
				{
					result = XmlUntypedConverter.Untyped.ToInt64(value);
				}
			}
			catch (InvalidCastException innerException)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Long", innerException, this);
			}
			catch (FormatException innerException2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Long", innerException2, this);
			}
			catch (OverflowException innerException3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Long", innerException3, this);
			}
			return result;
		}

		public override string ReadContentAsString()
		{
			if (!XmlReader.CanReadContentAs(this.NodeType))
			{
				throw base.CreateReadContentAsException("ReadContentAsString");
			}
			object obj = this.InternalReadContentAsObject();
			XmlSchemaType xmlSchemaType = (this.NodeType == XmlNodeType.Attribute) ? this.AttributeXmlType : this.ElementXmlType;
			string result;
			try
			{
				if (xmlSchemaType != null)
				{
					result = xmlSchemaType.ValueConverter.ToString(obj);
				}
				else
				{
					result = (obj as string);
				}
			}
			catch (InvalidCastException innerException)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "String", innerException, this);
			}
			catch (FormatException innerException2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "String", innerException2, this);
			}
			catch (OverflowException innerException3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "String", innerException3, this);
			}
			return result;
		}

		public override object ReadContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver)
		{
			if (!XmlReader.CanReadContentAs(this.NodeType))
			{
				throw base.CreateReadContentAsException("ReadContentAs");
			}
			string text;
			object value = this.InternalReadContentAsObject(false, out text);
			XmlSchemaType xmlSchemaType = (this.NodeType == XmlNodeType.Attribute) ? this.AttributeXmlType : this.ElementXmlType;
			object result;
			try
			{
				if (xmlSchemaType != null)
				{
					if (returnType == typeof(DateTimeOffset) && xmlSchemaType.Datatype is Datatype_dateTimeBase)
					{
						value = text;
					}
					result = xmlSchemaType.ValueConverter.ChangeType(value, returnType);
				}
				else
				{
					result = XmlUntypedConverter.Untyped.ChangeType(value, returnType, namespaceResolver);
				}
			}
			catch (FormatException innerException)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", returnType.ToString(), innerException, this);
			}
			catch (InvalidCastException innerException2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", returnType.ToString(), innerException2, this);
			}
			catch (OverflowException innerException3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", returnType.ToString(), innerException3, this);
			}
			return result;
		}

		public override object ReadElementContentAsObject()
		{
			if (this.NodeType != XmlNodeType.Element)
			{
				throw base.CreateReadElementContentAsException("ReadElementContentAsObject");
			}
			XmlSchemaType xmlSchemaType;
			return this.InternalReadElementContentAsObject(out xmlSchemaType, true);
		}

		public override bool ReadElementContentAsBoolean()
		{
			if (this.NodeType != XmlNodeType.Element)
			{
				throw base.CreateReadElementContentAsException("ReadElementContentAsBoolean");
			}
			XmlSchemaType xmlSchemaType;
			object value = this.InternalReadElementContentAsObject(out xmlSchemaType);
			bool result;
			try
			{
				if (xmlSchemaType != null)
				{
					result = xmlSchemaType.ValueConverter.ToBoolean(value);
				}
				else
				{
					result = XmlUntypedConverter.Untyped.ToBoolean(value);
				}
			}
			catch (FormatException innerException)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Boolean", innerException, this);
			}
			catch (InvalidCastException innerException2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Boolean", innerException2, this);
			}
			catch (OverflowException innerException3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Boolean", innerException3, this);
			}
			return result;
		}

		public override DateTime ReadElementContentAsDateTime()
		{
			if (this.NodeType != XmlNodeType.Element)
			{
				throw base.CreateReadElementContentAsException("ReadElementContentAsDateTime");
			}
			XmlSchemaType xmlSchemaType;
			object value = this.InternalReadElementContentAsObject(out xmlSchemaType);
			DateTime result;
			try
			{
				if (xmlSchemaType != null)
				{
					result = xmlSchemaType.ValueConverter.ToDateTime(value);
				}
				else
				{
					result = XmlUntypedConverter.Untyped.ToDateTime(value);
				}
			}
			catch (FormatException innerException)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "DateTime", innerException, this);
			}
			catch (InvalidCastException innerException2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "DateTime", innerException2, this);
			}
			catch (OverflowException innerException3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "DateTime", innerException3, this);
			}
			return result;
		}

		public override double ReadElementContentAsDouble()
		{
			if (this.NodeType != XmlNodeType.Element)
			{
				throw base.CreateReadElementContentAsException("ReadElementContentAsDouble");
			}
			XmlSchemaType xmlSchemaType;
			object value = this.InternalReadElementContentAsObject(out xmlSchemaType);
			double result;
			try
			{
				if (xmlSchemaType != null)
				{
					result = xmlSchemaType.ValueConverter.ToDouble(value);
				}
				else
				{
					result = XmlUntypedConverter.Untyped.ToDouble(value);
				}
			}
			catch (FormatException innerException)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Double", innerException, this);
			}
			catch (InvalidCastException innerException2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Double", innerException2, this);
			}
			catch (OverflowException innerException3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Double", innerException3, this);
			}
			return result;
		}

		public override float ReadElementContentAsFloat()
		{
			if (this.NodeType != XmlNodeType.Element)
			{
				throw base.CreateReadElementContentAsException("ReadElementContentAsFloat");
			}
			XmlSchemaType xmlSchemaType;
			object value = this.InternalReadElementContentAsObject(out xmlSchemaType);
			float result;
			try
			{
				if (xmlSchemaType != null)
				{
					result = xmlSchemaType.ValueConverter.ToSingle(value);
				}
				else
				{
					result = XmlUntypedConverter.Untyped.ToSingle(value);
				}
			}
			catch (FormatException innerException)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Float", innerException, this);
			}
			catch (InvalidCastException innerException2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Float", innerException2, this);
			}
			catch (OverflowException innerException3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Float", innerException3, this);
			}
			return result;
		}

		public override decimal ReadElementContentAsDecimal()
		{
			if (this.NodeType != XmlNodeType.Element)
			{
				throw base.CreateReadElementContentAsException("ReadElementContentAsDecimal");
			}
			XmlSchemaType xmlSchemaType;
			object value = this.InternalReadElementContentAsObject(out xmlSchemaType);
			decimal result;
			try
			{
				if (xmlSchemaType != null)
				{
					result = xmlSchemaType.ValueConverter.ToDecimal(value);
				}
				else
				{
					result = XmlUntypedConverter.Untyped.ToDecimal(value);
				}
			}
			catch (FormatException innerException)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Decimal", innerException, this);
			}
			catch (InvalidCastException innerException2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Decimal", innerException2, this);
			}
			catch (OverflowException innerException3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Decimal", innerException3, this);
			}
			return result;
		}

		public override int ReadElementContentAsInt()
		{
			if (this.NodeType != XmlNodeType.Element)
			{
				throw base.CreateReadElementContentAsException("ReadElementContentAsInt");
			}
			XmlSchemaType xmlSchemaType;
			object value = this.InternalReadElementContentAsObject(out xmlSchemaType);
			int result;
			try
			{
				if (xmlSchemaType != null)
				{
					result = xmlSchemaType.ValueConverter.ToInt32(value);
				}
				else
				{
					result = XmlUntypedConverter.Untyped.ToInt32(value);
				}
			}
			catch (FormatException innerException)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Int", innerException, this);
			}
			catch (InvalidCastException innerException2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Int", innerException2, this);
			}
			catch (OverflowException innerException3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Int", innerException3, this);
			}
			return result;
		}

		public override long ReadElementContentAsLong()
		{
			if (this.NodeType != XmlNodeType.Element)
			{
				throw base.CreateReadElementContentAsException("ReadElementContentAsLong");
			}
			XmlSchemaType xmlSchemaType;
			object value = this.InternalReadElementContentAsObject(out xmlSchemaType);
			long result;
			try
			{
				if (xmlSchemaType != null)
				{
					result = xmlSchemaType.ValueConverter.ToInt64(value);
				}
				else
				{
					result = XmlUntypedConverter.Untyped.ToInt64(value);
				}
			}
			catch (FormatException innerException)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Long", innerException, this);
			}
			catch (InvalidCastException innerException2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Long", innerException2, this);
			}
			catch (OverflowException innerException3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Long", innerException3, this);
			}
			return result;
		}

		public override string ReadElementContentAsString()
		{
			if (this.NodeType != XmlNodeType.Element)
			{
				throw base.CreateReadElementContentAsException("ReadElementContentAsString");
			}
			XmlSchemaType xmlSchemaType;
			object obj = this.InternalReadElementContentAsObject(out xmlSchemaType);
			string result;
			try
			{
				if (xmlSchemaType != null)
				{
					result = xmlSchemaType.ValueConverter.ToString(obj);
				}
				else
				{
					result = (obj as string);
				}
			}
			catch (InvalidCastException innerException)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "String", innerException, this);
			}
			catch (FormatException innerException2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "String", innerException2, this);
			}
			catch (OverflowException innerException3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "String", innerException3, this);
			}
			return result;
		}

		public override object ReadElementContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver)
		{
			if (this.NodeType != XmlNodeType.Element)
			{
				throw base.CreateReadElementContentAsException("ReadElementContentAs");
			}
			XmlSchemaType xmlSchemaType;
			string text;
			object value = this.InternalReadElementContentAsObject(out xmlSchemaType, false, out text);
			object result;
			try
			{
				if (xmlSchemaType != null)
				{
					if (returnType == typeof(DateTimeOffset) && xmlSchemaType.Datatype is Datatype_dateTimeBase)
					{
						value = text;
					}
					result = xmlSchemaType.ValueConverter.ChangeType(value, returnType, namespaceResolver);
				}
				else
				{
					result = XmlUntypedConverter.Untyped.ChangeType(value, returnType, namespaceResolver);
				}
			}
			catch (FormatException innerException)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", returnType.ToString(), innerException, this);
			}
			catch (InvalidCastException innerException2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", returnType.ToString(), innerException2, this);
			}
			catch (OverflowException innerException3)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", returnType.ToString(), innerException3, this);
			}
			return result;
		}

		public override int AttributeCount
		{
			get
			{
				return this.attributeCount;
			}
		}

		public override string GetAttribute(string name)
		{
			string text = this.coreReader.GetAttribute(name);
			if (text == null && this.attributeCount > 0)
			{
				ValidatingReaderNodeData defaultAttribute = this.GetDefaultAttribute(name, false);
				if (defaultAttribute != null)
				{
					text = defaultAttribute.RawValue;
				}
			}
			return text;
		}

		public override string GetAttribute(string name, string namespaceURI)
		{
			string attribute = this.coreReader.GetAttribute(name, namespaceURI);
			if (attribute == null && this.attributeCount > 0)
			{
				namespaceURI = ((namespaceURI == null) ? string.Empty : this.coreReaderNameTable.Get(namespaceURI));
				name = this.coreReaderNameTable.Get(name);
				if (name == null || namespaceURI == null)
				{
					return null;
				}
				ValidatingReaderNodeData defaultAttribute = this.GetDefaultAttribute(name, namespaceURI, false);
				if (defaultAttribute != null)
				{
					return defaultAttribute.RawValue;
				}
			}
			return attribute;
		}

		public override string GetAttribute(int i)
		{
			if (this.attributeCount == 0)
			{
				return null;
			}
			if (i < this.coreReaderAttributeCount)
			{
				return this.coreReader.GetAttribute(i);
			}
			int index = i - this.coreReaderAttributeCount;
			return ((ValidatingReaderNodeData)this.defaultAttributes[index]).RawValue;
		}

		public override bool MoveToAttribute(string name)
		{
			if (!this.coreReader.MoveToAttribute(name))
			{
				if (this.attributeCount > 0)
				{
					ValidatingReaderNodeData defaultAttribute = this.GetDefaultAttribute(name, true);
					if (defaultAttribute != null)
					{
						this.validationState = XsdValidatingReader.ValidatingReaderState.OnDefaultAttribute;
						this.attributePSVI = defaultAttribute.AttInfo;
						this.cachedNode = defaultAttribute;
						goto IL_57;
					}
				}
				return false;
			}
			this.validationState = XsdValidatingReader.ValidatingReaderState.OnAttribute;
			this.attributePSVI = this.GetAttributePSVI(name);
			IL_57:
			if (this.validationState == XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent)
			{
				this.readBinaryHelper.Finish();
				this.validationState = this.savedState;
			}
			return true;
		}

		public override bool MoveToAttribute(string name, string ns)
		{
			name = this.coreReaderNameTable.Get(name);
			ns = ((ns != null) ? this.coreReaderNameTable.Get(ns) : string.Empty);
			if (name == null || ns == null)
			{
				return false;
			}
			if (this.coreReader.MoveToAttribute(name, ns))
			{
				this.validationState = XsdValidatingReader.ValidatingReaderState.OnAttribute;
				if (this.inlineSchemaParser == null)
				{
					this.attributePSVI = this.GetAttributePSVI(name, ns);
				}
				else
				{
					this.attributePSVI = null;
				}
			}
			else
			{
				ValidatingReaderNodeData defaultAttribute = this.GetDefaultAttribute(name, ns, true);
				if (defaultAttribute == null)
				{
					return false;
				}
				this.attributePSVI = defaultAttribute.AttInfo;
				this.cachedNode = defaultAttribute;
				this.validationState = XsdValidatingReader.ValidatingReaderState.OnDefaultAttribute;
			}
			if (this.validationState == XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent)
			{
				this.readBinaryHelper.Finish();
				this.validationState = this.savedState;
			}
			return true;
		}

		public override void MoveToAttribute(int i)
		{
			if (i < 0 || i >= this.attributeCount)
			{
				throw new ArgumentOutOfRangeException("i");
			}
			this.currentAttrIndex = i;
			if (i < this.coreReaderAttributeCount)
			{
				this.coreReader.MoveToAttribute(i);
				if (this.inlineSchemaParser == null)
				{
					this.attributePSVI = this.attributePSVINodes[i];
				}
				else
				{
					this.attributePSVI = null;
				}
				this.validationState = XsdValidatingReader.ValidatingReaderState.OnAttribute;
			}
			else
			{
				int index = i - this.coreReaderAttributeCount;
				this.cachedNode = (ValidatingReaderNodeData)this.defaultAttributes[index];
				this.attributePSVI = this.cachedNode.AttInfo;
				this.validationState = XsdValidatingReader.ValidatingReaderState.OnDefaultAttribute;
			}
			if (this.validationState == XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent)
			{
				this.readBinaryHelper.Finish();
				this.validationState = this.savedState;
			}
		}

		public override bool MoveToFirstAttribute()
		{
			if (this.coreReader.MoveToFirstAttribute())
			{
				this.currentAttrIndex = 0;
				if (this.inlineSchemaParser == null)
				{
					this.attributePSVI = this.attributePSVINodes[0];
				}
				else
				{
					this.attributePSVI = null;
				}
				this.validationState = XsdValidatingReader.ValidatingReaderState.OnAttribute;
			}
			else
			{
				if (this.defaultAttributes.Count <= 0)
				{
					return false;
				}
				this.cachedNode = (ValidatingReaderNodeData)this.defaultAttributes[0];
				this.attributePSVI = this.cachedNode.AttInfo;
				this.currentAttrIndex = 0;
				this.validationState = XsdValidatingReader.ValidatingReaderState.OnDefaultAttribute;
			}
			if (this.validationState == XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent)
			{
				this.readBinaryHelper.Finish();
				this.validationState = this.savedState;
			}
			return true;
		}

		public override bool MoveToNextAttribute()
		{
			if (this.currentAttrIndex + 1 < this.coreReaderAttributeCount)
			{
				this.coreReader.MoveToNextAttribute();
				this.currentAttrIndex++;
				if (this.inlineSchemaParser == null)
				{
					this.attributePSVI = this.attributePSVINodes[this.currentAttrIndex];
				}
				else
				{
					this.attributePSVI = null;
				}
				this.validationState = XsdValidatingReader.ValidatingReaderState.OnAttribute;
			}
			else
			{
				if (this.currentAttrIndex + 1 >= this.attributeCount)
				{
					return false;
				}
				int num = this.currentAttrIndex + 1;
				this.currentAttrIndex = num;
				int index = num - this.coreReaderAttributeCount;
				this.cachedNode = (ValidatingReaderNodeData)this.defaultAttributes[index];
				this.attributePSVI = this.cachedNode.AttInfo;
				this.validationState = XsdValidatingReader.ValidatingReaderState.OnDefaultAttribute;
			}
			if (this.validationState == XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent)
			{
				this.readBinaryHelper.Finish();
				this.validationState = this.savedState;
			}
			return true;
		}

		public override bool MoveToElement()
		{
			if (this.coreReader.MoveToElement() || this.validationState < XsdValidatingReader.ValidatingReaderState.None)
			{
				this.currentAttrIndex = -1;
				this.validationState = XsdValidatingReader.ValidatingReaderState.ClearAttributes;
				return true;
			}
			return false;
		}

		public override bool Read()
		{
			switch (this.validationState)
			{
			case XsdValidatingReader.ValidatingReaderState.OnReadAttributeValue:
			case XsdValidatingReader.ValidatingReaderState.OnDefaultAttribute:
			case XsdValidatingReader.ValidatingReaderState.OnAttribute:
			case XsdValidatingReader.ValidatingReaderState.ClearAttributes:
				this.ClearAttributesInfo();
				if (this.inlineSchemaParser != null)
				{
					this.validationState = XsdValidatingReader.ValidatingReaderState.ParseInlineSchema;
					goto IL_7C;
				}
				this.validationState = XsdValidatingReader.ValidatingReaderState.Read;
				break;
			case XsdValidatingReader.ValidatingReaderState.None:
				return false;
			case XsdValidatingReader.ValidatingReaderState.Init:
				this.validationState = XsdValidatingReader.ValidatingReaderState.Read;
				if (this.coreReader.ReadState == ReadState.Interactive)
				{
					this.ProcessReaderEvent();
					return true;
				}
				break;
			case XsdValidatingReader.ValidatingReaderState.Read:
				break;
			case XsdValidatingReader.ValidatingReaderState.ParseInlineSchema:
				goto IL_7C;
			case XsdValidatingReader.ValidatingReaderState.ReadAhead:
				this.ClearAttributesInfo();
				this.ProcessReaderEvent();
				this.validationState = XsdValidatingReader.ValidatingReaderState.Read;
				return true;
			case XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent:
				this.validationState = this.savedState;
				this.readBinaryHelper.Finish();
				return this.Read();
			case XsdValidatingReader.ValidatingReaderState.ReaderClosed:
			case XsdValidatingReader.ValidatingReaderState.EOF:
				return false;
			default:
				return false;
			}
			if (this.coreReader.Read())
			{
				this.ProcessReaderEvent();
				return true;
			}
			this.validator.EndValidation();
			if (this.coreReader.EOF)
			{
				this.validationState = XsdValidatingReader.ValidatingReaderState.EOF;
			}
			return false;
			IL_7C:
			this.ProcessInlineSchema();
			return true;
		}

		public override bool EOF
		{
			get
			{
				return this.coreReader.EOF;
			}
		}

		public override void Close()
		{
			this.coreReader.Close();
			this.validationState = XsdValidatingReader.ValidatingReaderState.ReaderClosed;
		}

		public override ReadState ReadState
		{
			get
			{
				if (this.validationState != XsdValidatingReader.ValidatingReaderState.Init)
				{
					return this.coreReader.ReadState;
				}
				return ReadState.Initial;
			}
		}

		public override void Skip()
		{
			int depth = this.Depth;
			XmlNodeType nodeType = this.NodeType;
			if (nodeType != XmlNodeType.Element)
			{
				if (nodeType != XmlNodeType.Attribute)
				{
					goto IL_81;
				}
				this.MoveToElement();
			}
			if (!this.coreReader.IsEmptyElement)
			{
				bool flag = true;
				if ((this.xmlSchemaInfo.IsUnionType || this.xmlSchemaInfo.IsDefault) && this.coreReader is XsdCachingReader)
				{
					flag = false;
				}
				this.coreReader.Skip();
				this.validationState = XsdValidatingReader.ValidatingReaderState.ReadAhead;
				if (flag)
				{
					this.validator.SkipToEndElement(this.xmlSchemaInfo);
				}
			}
			IL_81:
			this.Read();
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return this.coreReaderNameTable;
			}
		}

		public override string LookupNamespace(string prefix)
		{
			return this.thisNSResolver.LookupNamespace(prefix);
		}

		public override void ResolveEntity()
		{
			throw new InvalidOperationException();
		}

		public override bool ReadAttributeValue()
		{
			if (this.validationState == XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent)
			{
				this.readBinaryHelper.Finish();
				this.validationState = this.savedState;
			}
			if (this.NodeType != XmlNodeType.Attribute)
			{
				return false;
			}
			if (this.validationState == XsdValidatingReader.ValidatingReaderState.OnDefaultAttribute)
			{
				this.cachedNode = this.CreateDummyTextNode(this.cachedNode.RawValue, this.cachedNode.Depth + 1);
				this.validationState = XsdValidatingReader.ValidatingReaderState.OnReadAttributeValue;
				return true;
			}
			return this.coreReader.ReadAttributeValue();
		}

		public override bool CanReadBinaryContent
		{
			get
			{
				return true;
			}
		}

		public override int ReadContentAsBase64(byte[] buffer, int index, int count)
		{
			if (this.ReadState != ReadState.Interactive)
			{
				return 0;
			}
			if (this.validationState != XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent)
			{
				this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
				this.savedState = this.validationState;
			}
			this.validationState = this.savedState;
			int result = this.readBinaryHelper.ReadContentAsBase64(buffer, index, count);
			this.savedState = this.validationState;
			this.validationState = XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent;
			return result;
		}

		public override int ReadContentAsBinHex(byte[] buffer, int index, int count)
		{
			if (this.ReadState != ReadState.Interactive)
			{
				return 0;
			}
			if (this.validationState != XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent)
			{
				this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
				this.savedState = this.validationState;
			}
			this.validationState = this.savedState;
			int result = this.readBinaryHelper.ReadContentAsBinHex(buffer, index, count);
			this.savedState = this.validationState;
			this.validationState = XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent;
			return result;
		}

		public override int ReadElementContentAsBase64(byte[] buffer, int index, int count)
		{
			if (this.ReadState != ReadState.Interactive)
			{
				return 0;
			}
			if (this.validationState != XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent)
			{
				this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
				this.savedState = this.validationState;
			}
			this.validationState = this.savedState;
			int result = this.readBinaryHelper.ReadElementContentAsBase64(buffer, index, count);
			this.savedState = this.validationState;
			this.validationState = XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent;
			return result;
		}

		public override int ReadElementContentAsBinHex(byte[] buffer, int index, int count)
		{
			if (this.ReadState != ReadState.Interactive)
			{
				return 0;
			}
			if (this.validationState != XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent)
			{
				this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
				this.savedState = this.validationState;
			}
			this.validationState = this.savedState;
			int result = this.readBinaryHelper.ReadElementContentAsBinHex(buffer, index, count);
			this.savedState = this.validationState;
			this.validationState = XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent;
			return result;
		}

		bool IXmlSchemaInfo.IsDefault
		{
			get
			{
				XmlNodeType nodeType = this.NodeType;
				if (nodeType != XmlNodeType.Element)
				{
					if (nodeType != XmlNodeType.Attribute)
					{
						if (nodeType == XmlNodeType.EndElement)
						{
							return this.xmlSchemaInfo.IsDefault;
						}
					}
					else if (this.attributePSVI != null)
					{
						return this.AttributeSchemaInfo.IsDefault;
					}
					return false;
				}
				if (!this.coreReader.IsEmptyElement)
				{
					this.GetIsDefault();
				}
				return this.xmlSchemaInfo.IsDefault;
			}
		}

		bool IXmlSchemaInfo.IsNil
		{
			get
			{
				XmlNodeType nodeType = this.NodeType;
				return (nodeType == XmlNodeType.Element || nodeType == XmlNodeType.EndElement) && this.xmlSchemaInfo.IsNil;
			}
		}

		XmlSchemaValidity IXmlSchemaInfo.Validity
		{
			get
			{
				XmlNodeType nodeType = this.NodeType;
				if (nodeType != XmlNodeType.Element)
				{
					if (nodeType != XmlNodeType.Attribute)
					{
						if (nodeType == XmlNodeType.EndElement)
						{
							return this.xmlSchemaInfo.Validity;
						}
					}
					else if (this.attributePSVI != null)
					{
						return this.AttributeSchemaInfo.Validity;
					}
					return XmlSchemaValidity.NotKnown;
				}
				if (this.coreReader.IsEmptyElement)
				{
					return this.xmlSchemaInfo.Validity;
				}
				if (this.xmlSchemaInfo.Validity == XmlSchemaValidity.Valid)
				{
					return XmlSchemaValidity.NotKnown;
				}
				return this.xmlSchemaInfo.Validity;
			}
		}

		XmlSchemaSimpleType IXmlSchemaInfo.MemberType
		{
			get
			{
				XmlNodeType nodeType = this.NodeType;
				if (nodeType == XmlNodeType.Element)
				{
					if (!this.coreReader.IsEmptyElement)
					{
						this.GetMemberType();
					}
					return this.xmlSchemaInfo.MemberType;
				}
				if (nodeType != XmlNodeType.Attribute)
				{
					if (nodeType != XmlNodeType.EndElement)
					{
						return null;
					}
					return this.xmlSchemaInfo.MemberType;
				}
				else
				{
					if (this.attributePSVI != null)
					{
						return this.AttributeSchemaInfo.MemberType;
					}
					return null;
				}
			}
		}

		XmlSchemaType IXmlSchemaInfo.SchemaType
		{
			get
			{
				XmlNodeType nodeType = this.NodeType;
				if (nodeType != XmlNodeType.Element)
				{
					if (nodeType != XmlNodeType.Attribute)
					{
						if (nodeType != XmlNodeType.EndElement)
						{
							return null;
						}
					}
					else
					{
						if (this.attributePSVI != null)
						{
							return this.AttributeSchemaInfo.SchemaType;
						}
						return null;
					}
				}
				return this.xmlSchemaInfo.SchemaType;
			}
		}

		XmlSchemaElement IXmlSchemaInfo.SchemaElement
		{
			get
			{
				if (this.NodeType == XmlNodeType.Element || this.NodeType == XmlNodeType.EndElement)
				{
					return this.xmlSchemaInfo.SchemaElement;
				}
				return null;
			}
		}

		XmlSchemaAttribute IXmlSchemaInfo.SchemaAttribute
		{
			get
			{
				if (this.NodeType == XmlNodeType.Attribute && this.attributePSVI != null)
				{
					return this.AttributeSchemaInfo.SchemaAttribute;
				}
				return null;
			}
		}

		public bool HasLineInfo()
		{
			return true;
		}

		public int LineNumber
		{
			get
			{
				if (this.lineInfo != null)
				{
					return this.lineInfo.LineNumber;
				}
				return 0;
			}
		}

		public int LinePosition
		{
			get
			{
				if (this.lineInfo != null)
				{
					return this.lineInfo.LinePosition;
				}
				return 0;
			}
		}

		IDictionary<string, string> IXmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope scope)
		{
			if (this.coreReaderNSResolver != null)
			{
				return this.coreReaderNSResolver.GetNamespacesInScope(scope);
			}
			return this.nsManager.GetNamespacesInScope(scope);
		}

		string IXmlNamespaceResolver.LookupNamespace(string prefix)
		{
			if (this.coreReaderNSResolver != null)
			{
				return this.coreReaderNSResolver.LookupNamespace(prefix);
			}
			return this.nsManager.LookupNamespace(prefix);
		}

		string IXmlNamespaceResolver.LookupPrefix(string namespaceName)
		{
			if (this.coreReaderNSResolver != null)
			{
				return this.coreReaderNSResolver.LookupPrefix(namespaceName);
			}
			return this.nsManager.LookupPrefix(namespaceName);
		}

		private object GetStringValue()
		{
			return this.coreReader.Value;
		}

		private XmlSchemaType ElementXmlType
		{
			get
			{
				return this.xmlSchemaInfo.XmlType;
			}
		}

		private XmlSchemaType AttributeXmlType
		{
			get
			{
				if (this.attributePSVI != null)
				{
					return this.AttributeSchemaInfo.XmlType;
				}
				return null;
			}
		}

		private XmlSchemaInfo AttributeSchemaInfo
		{
			get
			{
				return this.attributePSVI.attributeSchemaInfo;
			}
		}

		private void ProcessReaderEvent()
		{
			if (this.replayCache)
			{
				return;
			}
			switch (this.coreReader.NodeType)
			{
			case XmlNodeType.Element:
				this.ProcessElementEvent();
				return;
			case XmlNodeType.Attribute:
			case XmlNodeType.Entity:
			case XmlNodeType.ProcessingInstruction:
			case XmlNodeType.Comment:
			case XmlNodeType.Document:
			case XmlNodeType.DocumentFragment:
			case XmlNodeType.Notation:
				break;
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
				this.validator.ValidateText(new XmlValueGetter(this.GetStringValue));
				return;
			case XmlNodeType.EntityReference:
				throw new InvalidOperationException();
			case XmlNodeType.DocumentType:
				this.validator.SetDtdSchemaInfo(this.coreReader.DtdInfo);
				break;
			case XmlNodeType.Whitespace:
			case XmlNodeType.SignificantWhitespace:
				this.validator.ValidateWhitespace(new XmlValueGetter(this.GetStringValue));
				return;
			case XmlNodeType.EndElement:
				this.ProcessEndElementEvent();
				return;
			default:
				return;
			}
		}

		private void ProcessElementEvent()
		{
			if (!this.processInlineSchema || !this.IsXSDRoot(this.coreReader.LocalName, this.coreReader.NamespaceURI) || this.coreReader.Depth <= 0)
			{
				this.atomicValue = null;
				this.originalAtomicValueString = null;
				this.xmlSchemaInfo.Clear();
				if (this.manageNamespaces)
				{
					this.nsManager.PushScope();
				}
				string xsiSchemaLocation = null;
				string xsiNoNamespaceSchemaLocation = null;
				string xsiNil = null;
				string xsiType = null;
				if (this.coreReader.MoveToFirstAttribute())
				{
					do
					{
						string namespaceURI = this.coreReader.NamespaceURI;
						string localName = this.coreReader.LocalName;
						if (Ref.Equal(namespaceURI, this.NsXsi))
						{
							if (Ref.Equal(localName, this.XsiSchemaLocation))
							{
								xsiSchemaLocation = this.coreReader.Value;
							}
							else if (Ref.Equal(localName, this.XsiNoNamespaceSchemaLocation))
							{
								xsiNoNamespaceSchemaLocation = this.coreReader.Value;
							}
							else if (Ref.Equal(localName, this.XsiType))
							{
								xsiType = this.coreReader.Value;
							}
							else if (Ref.Equal(localName, this.XsiNil))
							{
								xsiNil = this.coreReader.Value;
							}
						}
						if (this.manageNamespaces && Ref.Equal(this.coreReader.NamespaceURI, this.NsXmlNs))
						{
							this.nsManager.AddNamespace((this.coreReader.Prefix.Length == 0) ? string.Empty : this.coreReader.LocalName, this.coreReader.Value);
						}
					}
					while (this.coreReader.MoveToNextAttribute());
					this.coreReader.MoveToElement();
				}
				this.validator.ValidateElement(this.coreReader.LocalName, this.coreReader.NamespaceURI, this.xmlSchemaInfo, xsiType, xsiNil, xsiSchemaLocation, xsiNoNamespaceSchemaLocation);
				this.ValidateAttributes();
				this.validator.ValidateEndOfAttributes(this.xmlSchemaInfo);
				if (this.coreReader.IsEmptyElement)
				{
					this.ProcessEndElementEvent();
				}
				this.validationState = XsdValidatingReader.ValidatingReaderState.ClearAttributes;
				return;
			}
			this.xmlSchemaInfo.Clear();
			this.attributeCount = (this.coreReaderAttributeCount = this.coreReader.AttributeCount);
			if (!this.coreReader.IsEmptyElement)
			{
				this.inlineSchemaParser = new Parser(SchemaType.XSD, this.coreReaderNameTable, this.validator.SchemaSet.GetSchemaNames(this.coreReaderNameTable), this.validationEvent);
				this.inlineSchemaParser.StartParsing(this.coreReader, null);
				this.inlineSchemaParser.ParseReaderNode();
				this.validationState = XsdValidatingReader.ValidatingReaderState.ParseInlineSchema;
				return;
			}
			this.validationState = XsdValidatingReader.ValidatingReaderState.ClearAttributes;
		}

		private void ProcessEndElementEvent()
		{
			this.atomicValue = this.validator.ValidateEndElement(this.xmlSchemaInfo);
			this.originalAtomicValueString = this.GetOriginalAtomicValueStringOfElement();
			if (this.xmlSchemaInfo.IsDefault)
			{
				int depth = this.coreReader.Depth;
				this.coreReader = this.GetCachingReader();
				this.cachingReader.RecordTextNode(this.xmlSchemaInfo.XmlType.ValueConverter.ToString(this.atomicValue), this.originalAtomicValueString, depth + 1, 0, 0);
				this.cachingReader.RecordEndElementNode();
				this.cachingReader.SetToReplayMode();
				this.replayCache = true;
				return;
			}
			if (this.manageNamespaces)
			{
				this.nsManager.PopScope();
			}
		}

		private void ValidateAttributes()
		{
			this.attributeCount = (this.coreReaderAttributeCount = this.coreReader.AttributeCount);
			int num = 0;
			bool flag = false;
			if (this.coreReader.MoveToFirstAttribute())
			{
				do
				{
					string localName = this.coreReader.LocalName;
					string namespaceURI = this.coreReader.NamespaceURI;
					AttributePSVIInfo attributePSVIInfo = this.AddAttributePSVI(num);
					attributePSVIInfo.localName = localName;
					attributePSVIInfo.namespaceUri = namespaceURI;
					if (namespaceURI == this.NsXmlNs)
					{
						num++;
					}
					else
					{
						attributePSVIInfo.typedAttributeValue = this.validator.ValidateAttribute(localName, namespaceURI, this.valueGetter, attributePSVIInfo.attributeSchemaInfo);
						if (!flag)
						{
							flag = (attributePSVIInfo.attributeSchemaInfo.Validity == XmlSchemaValidity.Invalid);
						}
						num++;
					}
				}
				while (this.coreReader.MoveToNextAttribute());
			}
			this.coreReader.MoveToElement();
			if (flag)
			{
				this.xmlSchemaInfo.Validity = XmlSchemaValidity.Invalid;
			}
			this.validator.GetUnspecifiedDefaultAttributes(this.defaultAttributes, true);
			this.attributeCount += this.defaultAttributes.Count;
		}

		private void ClearAttributesInfo()
		{
			this.attributeCount = 0;
			this.coreReaderAttributeCount = 0;
			this.currentAttrIndex = -1;
			this.defaultAttributes.Clear();
			this.attributePSVI = null;
		}

		private AttributePSVIInfo GetAttributePSVI(string name)
		{
			if (this.inlineSchemaParser != null)
			{
				return null;
			}
			string text;
			string text2;
			ValidateNames.SplitQName(name, out text, out text2);
			text = this.coreReaderNameTable.Add(text);
			text2 = this.coreReaderNameTable.Add(text2);
			string ns;
			if (text.Length == 0)
			{
				ns = string.Empty;
			}
			else
			{
				ns = this.thisNSResolver.LookupNamespace(text);
			}
			return this.GetAttributePSVI(text2, ns);
		}

		private AttributePSVIInfo GetAttributePSVI(string localName, string ns)
		{
			for (int i = 0; i < this.coreReaderAttributeCount; i++)
			{
				AttributePSVIInfo attributePSVIInfo = this.attributePSVINodes[i];
				if (attributePSVIInfo != null && Ref.Equal(localName, attributePSVIInfo.localName) && Ref.Equal(ns, attributePSVIInfo.namespaceUri))
				{
					this.currentAttrIndex = i;
					return attributePSVIInfo;
				}
			}
			return null;
		}

		private ValidatingReaderNodeData GetDefaultAttribute(string name, bool updatePosition)
		{
			string text;
			string text2;
			ValidateNames.SplitQName(name, out text, out text2);
			text = this.coreReaderNameTable.Add(text);
			text2 = this.coreReaderNameTable.Add(text2);
			string ns;
			if (text.Length == 0)
			{
				ns = string.Empty;
			}
			else
			{
				ns = this.thisNSResolver.LookupNamespace(text);
			}
			return this.GetDefaultAttribute(text2, ns, updatePosition);
		}

		private ValidatingReaderNodeData GetDefaultAttribute(string attrLocalName, string ns, bool updatePosition)
		{
			for (int i = 0; i < this.defaultAttributes.Count; i++)
			{
				ValidatingReaderNodeData validatingReaderNodeData = (ValidatingReaderNodeData)this.defaultAttributes[i];
				if (Ref.Equal(validatingReaderNodeData.LocalName, attrLocalName) && Ref.Equal(validatingReaderNodeData.Namespace, ns))
				{
					if (updatePosition)
					{
						this.currentAttrIndex = this.coreReader.AttributeCount + i;
					}
					return validatingReaderNodeData;
				}
			}
			return null;
		}

		private AttributePSVIInfo AddAttributePSVI(int attIndex)
		{
			AttributePSVIInfo attributePSVIInfo = this.attributePSVINodes[attIndex];
			if (attributePSVIInfo != null)
			{
				attributePSVIInfo.Reset();
				return attributePSVIInfo;
			}
			if (attIndex >= this.attributePSVINodes.Length - 1)
			{
				AttributePSVIInfo[] destinationArray = new AttributePSVIInfo[this.attributePSVINodes.Length * 2];
				Array.Copy(this.attributePSVINodes, 0, destinationArray, 0, this.attributePSVINodes.Length);
				this.attributePSVINodes = destinationArray;
			}
			attributePSVIInfo = this.attributePSVINodes[attIndex];
			if (attributePSVIInfo == null)
			{
				attributePSVIInfo = new AttributePSVIInfo();
				this.attributePSVINodes[attIndex] = attributePSVIInfo;
			}
			return attributePSVIInfo;
		}

		private bool IsXSDRoot(string localName, string ns)
		{
			return Ref.Equal(ns, this.NsXs) && Ref.Equal(localName, this.XsdSchema);
		}

		private void ProcessInlineSchema()
		{
			if (this.coreReader.Read())
			{
				if (this.coreReader.NodeType == XmlNodeType.Element)
				{
					this.attributeCount = (this.coreReaderAttributeCount = this.coreReader.AttributeCount);
				}
				else
				{
					this.ClearAttributesInfo();
				}
				if (!this.inlineSchemaParser.ParseReaderNode())
				{
					this.inlineSchemaParser.FinishParsing();
					XmlSchema xmlSchema = this.inlineSchemaParser.XmlSchema;
					this.validator.AddSchema(xmlSchema);
					this.inlineSchemaParser = null;
					this.validationState = XsdValidatingReader.ValidatingReaderState.Read;
				}
			}
		}

		private object InternalReadContentAsObject()
		{
			return this.InternalReadContentAsObject(false);
		}

		private object InternalReadContentAsObject(bool unwrapTypedValue)
		{
			string text;
			return this.InternalReadContentAsObject(unwrapTypedValue, out text);
		}

		private object InternalReadContentAsObject(bool unwrapTypedValue, out string originalStringValue)
		{
			XmlNodeType nodeType = this.NodeType;
			if (nodeType == XmlNodeType.Attribute)
			{
				originalStringValue = this.Value;
				if (this.attributePSVI != null && this.attributePSVI.typedAttributeValue != null)
				{
					if (this.validationState == XsdValidatingReader.ValidatingReaderState.OnDefaultAttribute)
					{
						XmlSchemaAttribute schemaAttribute = this.attributePSVI.attributeSchemaInfo.SchemaAttribute;
						originalStringValue = ((schemaAttribute.DefaultValue != null) ? schemaAttribute.DefaultValue : schemaAttribute.FixedValue);
					}
					return this.ReturnBoxedValue(this.attributePSVI.typedAttributeValue, this.AttributeSchemaInfo.XmlType, unwrapTypedValue);
				}
				return this.Value;
			}
			else if (nodeType == XmlNodeType.EndElement)
			{
				if (this.atomicValue != null)
				{
					originalStringValue = this.originalAtomicValueString;
					return this.atomicValue;
				}
				originalStringValue = string.Empty;
				return string.Empty;
			}
			else
			{
				if (this.validator.CurrentContentType == XmlSchemaContentType.TextOnly)
				{
					object result = this.ReturnBoxedValue(this.ReadTillEndElement(), this.xmlSchemaInfo.XmlType, unwrapTypedValue);
					originalStringValue = this.originalAtomicValueString;
					return result;
				}
				XsdCachingReader xsdCachingReader = this.coreReader as XsdCachingReader;
				if (xsdCachingReader != null)
				{
					originalStringValue = xsdCachingReader.ReadOriginalContentAsString();
				}
				else
				{
					originalStringValue = base.InternalReadContentAsString();
				}
				return originalStringValue;
			}
		}

		private object InternalReadElementContentAsObject(out XmlSchemaType xmlType)
		{
			return this.InternalReadElementContentAsObject(out xmlType, false);
		}

		private object InternalReadElementContentAsObject(out XmlSchemaType xmlType, bool unwrapTypedValue)
		{
			string text;
			return this.InternalReadElementContentAsObject(out xmlType, unwrapTypedValue, out text);
		}

		private object InternalReadElementContentAsObject(out XmlSchemaType xmlType, bool unwrapTypedValue, out string originalString)
		{
			xmlType = null;
			object result;
			if (this.IsEmptyElement)
			{
				if (this.xmlSchemaInfo.ContentType == XmlSchemaContentType.TextOnly)
				{
					result = this.ReturnBoxedValue(this.atomicValue, this.xmlSchemaInfo.XmlType, unwrapTypedValue);
				}
				else
				{
					result = this.atomicValue;
				}
				originalString = this.originalAtomicValueString;
				xmlType = this.ElementXmlType;
				this.Read();
				return result;
			}
			this.Read();
			if (this.NodeType == XmlNodeType.EndElement)
			{
				if (this.xmlSchemaInfo.IsDefault)
				{
					if (this.xmlSchemaInfo.ContentType == XmlSchemaContentType.TextOnly)
					{
						result = this.ReturnBoxedValue(this.atomicValue, this.xmlSchemaInfo.XmlType, unwrapTypedValue);
					}
					else
					{
						result = this.atomicValue;
					}
					originalString = this.originalAtomicValueString;
				}
				else
				{
					result = string.Empty;
					originalString = string.Empty;
				}
			}
			else
			{
				if (this.NodeType == XmlNodeType.Element)
				{
					throw new XmlException("ReadElementContentAs() methods cannot be called on an element that has child elements.", string.Empty, this);
				}
				result = this.InternalReadContentAsObject(unwrapTypedValue, out originalString);
				if (this.NodeType != XmlNodeType.EndElement)
				{
					throw new XmlException("ReadElementContentAs() methods cannot be called on an element that has child elements.", string.Empty, this);
				}
			}
			xmlType = this.ElementXmlType;
			this.Read();
			return result;
		}

		private object ReadTillEndElement()
		{
			if (this.atomicValue == null)
			{
				while (this.coreReader.Read())
				{
					if (!this.replayCache)
					{
						switch (this.coreReader.NodeType)
						{
						case XmlNodeType.Element:
							this.ProcessReaderEvent();
							goto IL_10B;
						case XmlNodeType.Text:
						case XmlNodeType.CDATA:
							this.validator.ValidateText(new XmlValueGetter(this.GetStringValue));
							break;
						case XmlNodeType.Whitespace:
						case XmlNodeType.SignificantWhitespace:
							this.validator.ValidateWhitespace(new XmlValueGetter(this.GetStringValue));
							break;
						case XmlNodeType.EndElement:
							this.atomicValue = this.validator.ValidateEndElement(this.xmlSchemaInfo);
							this.originalAtomicValueString = this.GetOriginalAtomicValueStringOfElement();
							if (this.manageNamespaces)
							{
								this.nsManager.PopScope();
								goto IL_10B;
							}
							goto IL_10B;
						}
					}
				}
			}
			else
			{
				if (this.atomicValue == this)
				{
					this.atomicValue = null;
				}
				this.SwitchReader();
			}
			IL_10B:
			return this.atomicValue;
		}

		private void SwitchReader()
		{
			XsdCachingReader xsdCachingReader = this.coreReader as XsdCachingReader;
			if (xsdCachingReader != null)
			{
				this.coreReader = xsdCachingReader.GetCoreReader();
			}
			this.replayCache = false;
		}

		private void ReadAheadForMemberType()
		{
			while (this.coreReader.Read())
			{
				switch (this.coreReader.NodeType)
				{
				case XmlNodeType.Text:
				case XmlNodeType.CDATA:
					this.validator.ValidateText(new XmlValueGetter(this.GetStringValue));
					break;
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					this.validator.ValidateWhitespace(new XmlValueGetter(this.GetStringValue));
					break;
				case XmlNodeType.EndElement:
					this.atomicValue = this.validator.ValidateEndElement(this.xmlSchemaInfo);
					this.originalAtomicValueString = this.GetOriginalAtomicValueStringOfElement();
					if (this.atomicValue == null)
					{
						this.atomicValue = this;
						return;
					}
					if (this.xmlSchemaInfo.IsDefault)
					{
						this.cachingReader.SwitchTextNodeAndEndElement(this.xmlSchemaInfo.XmlType.ValueConverter.ToString(this.atomicValue), this.originalAtomicValueString);
						return;
					}
					return;
				}
			}
		}

		private void GetIsDefault()
		{
			if (!(this.coreReader is XsdCachingReader) && this.xmlSchemaInfo.HasDefaultValue)
			{
				this.coreReader = this.GetCachingReader();
				if (this.xmlSchemaInfo.IsUnionType && !this.xmlSchemaInfo.IsNil)
				{
					this.ReadAheadForMemberType();
				}
				else if (this.coreReader.Read())
				{
					switch (this.coreReader.NodeType)
					{
					case XmlNodeType.Text:
					case XmlNodeType.CDATA:
						this.validator.ValidateText(new XmlValueGetter(this.GetStringValue));
						break;
					case XmlNodeType.Whitespace:
					case XmlNodeType.SignificantWhitespace:
						this.validator.ValidateWhitespace(new XmlValueGetter(this.GetStringValue));
						break;
					case XmlNodeType.EndElement:
						this.atomicValue = this.validator.ValidateEndElement(this.xmlSchemaInfo);
						this.originalAtomicValueString = this.GetOriginalAtomicValueStringOfElement();
						if (this.xmlSchemaInfo.IsDefault)
						{
							this.cachingReader.SwitchTextNodeAndEndElement(this.xmlSchemaInfo.XmlType.ValueConverter.ToString(this.atomicValue), this.originalAtomicValueString);
						}
						break;
					}
				}
				this.cachingReader.SetToReplayMode();
				this.replayCache = true;
			}
		}

		private void GetMemberType()
		{
			if (this.xmlSchemaInfo.MemberType != null || this.atomicValue == this)
			{
				return;
			}
			if (!(this.coreReader is XsdCachingReader) && this.xmlSchemaInfo.IsUnionType && !this.xmlSchemaInfo.IsNil)
			{
				this.coreReader = this.GetCachingReader();
				this.ReadAheadForMemberType();
				this.cachingReader.SetToReplayMode();
				this.replayCache = true;
			}
		}

		private object ReturnBoxedValue(object typedValue, XmlSchemaType xmlType, bool unWrap)
		{
			if (typedValue != null)
			{
				if (unWrap && xmlType.Datatype.Variety == XmlSchemaDatatypeVariety.List && (xmlType.Datatype as Datatype_List).ItemType.Variety == XmlSchemaDatatypeVariety.Union)
				{
					typedValue = xmlType.ValueConverter.ChangeType(typedValue, xmlType.Datatype.ValueType, this.thisNSResolver);
				}
				return typedValue;
			}
			typedValue = this.validator.GetConcatenatedValue();
			return typedValue;
		}

		private XsdCachingReader GetCachingReader()
		{
			if (this.cachingReader == null)
			{
				this.cachingReader = new XsdCachingReader(this.coreReader, this.lineInfo, new CachingEventHandler(this.CachingCallBack));
			}
			else
			{
				this.cachingReader.Reset(this.coreReader);
			}
			this.lineInfo = this.cachingReader;
			return this.cachingReader;
		}

		internal ValidatingReaderNodeData CreateDummyTextNode(string attributeValue, int depth)
		{
			if (this.textNode == null)
			{
				this.textNode = new ValidatingReaderNodeData(XmlNodeType.Text);
			}
			this.textNode.Depth = depth;
			this.textNode.RawValue = attributeValue;
			return this.textNode;
		}

		internal void CachingCallBack(XsdCachingReader cachingReader)
		{
			this.coreReader = cachingReader.GetCoreReader();
			this.lineInfo = cachingReader.GetLineInfo();
			this.replayCache = false;
		}

		private string GetOriginalAtomicValueStringOfElement()
		{
			if (!this.xmlSchemaInfo.IsDefault)
			{
				return this.validator.GetConcatenatedValue();
			}
			XmlSchemaElement schemaElement = this.xmlSchemaInfo.SchemaElement;
			if (schemaElement == null)
			{
				return string.Empty;
			}
			if (schemaElement.DefaultValue == null)
			{
				return schemaElement.FixedValue;
			}
			return schemaElement.DefaultValue;
		}

		public override Task<string> GetValueAsync()
		{
			if (this.validationState < XsdValidatingReader.ValidatingReaderState.None)
			{
				return Task.FromResult<string>(this.cachedNode.RawValue);
			}
			return this.coreReader.GetValueAsync();
		}

		public override Task<object> ReadContentAsObjectAsync()
		{
			if (!XmlReader.CanReadContentAs(this.NodeType))
			{
				throw base.CreateReadContentAsException("ReadContentAsObject");
			}
			return this.InternalReadContentAsObjectAsync(true);
		}

		public override Task<string> ReadContentAsStringAsync()
		{
			XsdValidatingReader.<ReadContentAsStringAsync>d__187 <ReadContentAsStringAsync>d__;
			<ReadContentAsStringAsync>d__.<>4__this = this;
			<ReadContentAsStringAsync>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<ReadContentAsStringAsync>d__.<>1__state = -1;
			<ReadContentAsStringAsync>d__.<>t__builder.Start<XsdValidatingReader.<ReadContentAsStringAsync>d__187>(ref <ReadContentAsStringAsync>d__);
			return <ReadContentAsStringAsync>d__.<>t__builder.Task;
		}

		public override Task<object> ReadContentAsAsync(Type returnType, IXmlNamespaceResolver namespaceResolver)
		{
			XsdValidatingReader.<ReadContentAsAsync>d__188 <ReadContentAsAsync>d__;
			<ReadContentAsAsync>d__.<>4__this = this;
			<ReadContentAsAsync>d__.returnType = returnType;
			<ReadContentAsAsync>d__.namespaceResolver = namespaceResolver;
			<ReadContentAsAsync>d__.<>t__builder = AsyncTaskMethodBuilder<object>.Create();
			<ReadContentAsAsync>d__.<>1__state = -1;
			<ReadContentAsAsync>d__.<>t__builder.Start<XsdValidatingReader.<ReadContentAsAsync>d__188>(ref <ReadContentAsAsync>d__);
			return <ReadContentAsAsync>d__.<>t__builder.Task;
		}

		public override Task<object> ReadElementContentAsObjectAsync()
		{
			XsdValidatingReader.<ReadElementContentAsObjectAsync>d__189 <ReadElementContentAsObjectAsync>d__;
			<ReadElementContentAsObjectAsync>d__.<>4__this = this;
			<ReadElementContentAsObjectAsync>d__.<>t__builder = AsyncTaskMethodBuilder<object>.Create();
			<ReadElementContentAsObjectAsync>d__.<>1__state = -1;
			<ReadElementContentAsObjectAsync>d__.<>t__builder.Start<XsdValidatingReader.<ReadElementContentAsObjectAsync>d__189>(ref <ReadElementContentAsObjectAsync>d__);
			return <ReadElementContentAsObjectAsync>d__.<>t__builder.Task;
		}

		public override Task<string> ReadElementContentAsStringAsync()
		{
			XsdValidatingReader.<ReadElementContentAsStringAsync>d__190 <ReadElementContentAsStringAsync>d__;
			<ReadElementContentAsStringAsync>d__.<>4__this = this;
			<ReadElementContentAsStringAsync>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<ReadElementContentAsStringAsync>d__.<>1__state = -1;
			<ReadElementContentAsStringAsync>d__.<>t__builder.Start<XsdValidatingReader.<ReadElementContentAsStringAsync>d__190>(ref <ReadElementContentAsStringAsync>d__);
			return <ReadElementContentAsStringAsync>d__.<>t__builder.Task;
		}

		public override Task<object> ReadElementContentAsAsync(Type returnType, IXmlNamespaceResolver namespaceResolver)
		{
			XsdValidatingReader.<ReadElementContentAsAsync>d__191 <ReadElementContentAsAsync>d__;
			<ReadElementContentAsAsync>d__.<>4__this = this;
			<ReadElementContentAsAsync>d__.returnType = returnType;
			<ReadElementContentAsAsync>d__.namespaceResolver = namespaceResolver;
			<ReadElementContentAsAsync>d__.<>t__builder = AsyncTaskMethodBuilder<object>.Create();
			<ReadElementContentAsAsync>d__.<>1__state = -1;
			<ReadElementContentAsAsync>d__.<>t__builder.Start<XsdValidatingReader.<ReadElementContentAsAsync>d__191>(ref <ReadElementContentAsAsync>d__);
			return <ReadElementContentAsAsync>d__.<>t__builder.Task;
		}

		private Task<bool> ReadAsync_Read(Task<bool> task)
		{
			if (!task.IsSuccess())
			{
				return this._ReadAsync_Read(task);
			}
			if (task.Result)
			{
				return this.ProcessReaderEventAsync().ReturnTaskBoolWhenFinish(true);
			}
			this.validator.EndValidation();
			if (this.coreReader.EOF)
			{
				this.validationState = XsdValidatingReader.ValidatingReaderState.EOF;
			}
			return AsyncHelper.DoneTaskFalse;
		}

		private Task<bool> _ReadAsync_Read(Task<bool> task)
		{
			XsdValidatingReader.<_ReadAsync_Read>d__193 <_ReadAsync_Read>d__;
			<_ReadAsync_Read>d__.<>4__this = this;
			<_ReadAsync_Read>d__.task = task;
			<_ReadAsync_Read>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<_ReadAsync_Read>d__.<>1__state = -1;
			<_ReadAsync_Read>d__.<>t__builder.Start<XsdValidatingReader.<_ReadAsync_Read>d__193>(ref <_ReadAsync_Read>d__);
			return <_ReadAsync_Read>d__.<>t__builder.Task;
		}

		private Task<bool> ReadAsync_ReadAhead(Task task)
		{
			if (task.IsSuccess())
			{
				this.validationState = XsdValidatingReader.ValidatingReaderState.Read;
				return AsyncHelper.DoneTaskTrue;
			}
			return this._ReadAsync_ReadAhead(task);
		}

		private Task<bool> _ReadAsync_ReadAhead(Task task)
		{
			XsdValidatingReader.<_ReadAsync_ReadAhead>d__195 <_ReadAsync_ReadAhead>d__;
			<_ReadAsync_ReadAhead>d__.<>4__this = this;
			<_ReadAsync_ReadAhead>d__.task = task;
			<_ReadAsync_ReadAhead>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<_ReadAsync_ReadAhead>d__.<>1__state = -1;
			<_ReadAsync_ReadAhead>d__.<>t__builder.Start<XsdValidatingReader.<_ReadAsync_ReadAhead>d__195>(ref <_ReadAsync_ReadAhead>d__);
			return <_ReadAsync_ReadAhead>d__.<>t__builder.Task;
		}

		public override Task<bool> ReadAsync()
		{
			switch (this.validationState)
			{
			case XsdValidatingReader.ValidatingReaderState.OnReadAttributeValue:
			case XsdValidatingReader.ValidatingReaderState.OnDefaultAttribute:
			case XsdValidatingReader.ValidatingReaderState.OnAttribute:
			case XsdValidatingReader.ValidatingReaderState.ClearAttributes:
				this.ClearAttributesInfo();
				if (this.inlineSchemaParser != null)
				{
					this.validationState = XsdValidatingReader.ValidatingReaderState.ParseInlineSchema;
					goto IL_59;
				}
				this.validationState = XsdValidatingReader.ValidatingReaderState.Read;
				break;
			case XsdValidatingReader.ValidatingReaderState.None:
				goto IL_F0;
			case XsdValidatingReader.ValidatingReaderState.Init:
				this.validationState = XsdValidatingReader.ValidatingReaderState.Read;
				if (this.coreReader.ReadState == ReadState.Interactive)
				{
					return this.ProcessReaderEventAsync().ReturnTaskBoolWhenFinish(true);
				}
				break;
			case XsdValidatingReader.ValidatingReaderState.Read:
				break;
			case XsdValidatingReader.ValidatingReaderState.ParseInlineSchema:
				goto IL_59;
			case XsdValidatingReader.ValidatingReaderState.ReadAhead:
			{
				this.ClearAttributesInfo();
				Task task = this.ProcessReaderEventAsync();
				return this.ReadAsync_ReadAhead(task);
			}
			case XsdValidatingReader.ValidatingReaderState.OnReadBinaryContent:
				this.validationState = this.savedState;
				return this.readBinaryHelper.FinishAsync().CallBoolTaskFuncWhenFinish(new Func<Task<bool>>(this.ReadAsync));
			case XsdValidatingReader.ValidatingReaderState.ReaderClosed:
			case XsdValidatingReader.ValidatingReaderState.EOF:
				return AsyncHelper.DoneTaskFalse;
			default:
				goto IL_F0;
			}
			Task<bool> task2 = this.coreReader.ReadAsync();
			return this.ReadAsync_Read(task2);
			IL_59:
			return this.ProcessInlineSchemaAsync().ReturnTaskBoolWhenFinish(true);
			IL_F0:
			return AsyncHelper.DoneTaskFalse;
		}

		public override Task SkipAsync()
		{
			XsdValidatingReader.<SkipAsync>d__197 <SkipAsync>d__;
			<SkipAsync>d__.<>4__this = this;
			<SkipAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SkipAsync>d__.<>1__state = -1;
			<SkipAsync>d__.<>t__builder.Start<XsdValidatingReader.<SkipAsync>d__197>(ref <SkipAsync>d__);
			return <SkipAsync>d__.<>t__builder.Task;
		}

		public override Task<int> ReadContentAsBase64Async(byte[] buffer, int index, int count)
		{
			XsdValidatingReader.<ReadContentAsBase64Async>d__198 <ReadContentAsBase64Async>d__;
			<ReadContentAsBase64Async>d__.<>4__this = this;
			<ReadContentAsBase64Async>d__.buffer = buffer;
			<ReadContentAsBase64Async>d__.index = index;
			<ReadContentAsBase64Async>d__.count = count;
			<ReadContentAsBase64Async>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<ReadContentAsBase64Async>d__.<>1__state = -1;
			<ReadContentAsBase64Async>d__.<>t__builder.Start<XsdValidatingReader.<ReadContentAsBase64Async>d__198>(ref <ReadContentAsBase64Async>d__);
			return <ReadContentAsBase64Async>d__.<>t__builder.Task;
		}

		public override Task<int> ReadContentAsBinHexAsync(byte[] buffer, int index, int count)
		{
			XsdValidatingReader.<ReadContentAsBinHexAsync>d__199 <ReadContentAsBinHexAsync>d__;
			<ReadContentAsBinHexAsync>d__.<>4__this = this;
			<ReadContentAsBinHexAsync>d__.buffer = buffer;
			<ReadContentAsBinHexAsync>d__.index = index;
			<ReadContentAsBinHexAsync>d__.count = count;
			<ReadContentAsBinHexAsync>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<ReadContentAsBinHexAsync>d__.<>1__state = -1;
			<ReadContentAsBinHexAsync>d__.<>t__builder.Start<XsdValidatingReader.<ReadContentAsBinHexAsync>d__199>(ref <ReadContentAsBinHexAsync>d__);
			return <ReadContentAsBinHexAsync>d__.<>t__builder.Task;
		}

		public override Task<int> ReadElementContentAsBase64Async(byte[] buffer, int index, int count)
		{
			XsdValidatingReader.<ReadElementContentAsBase64Async>d__200 <ReadElementContentAsBase64Async>d__;
			<ReadElementContentAsBase64Async>d__.<>4__this = this;
			<ReadElementContentAsBase64Async>d__.buffer = buffer;
			<ReadElementContentAsBase64Async>d__.index = index;
			<ReadElementContentAsBase64Async>d__.count = count;
			<ReadElementContentAsBase64Async>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<ReadElementContentAsBase64Async>d__.<>1__state = -1;
			<ReadElementContentAsBase64Async>d__.<>t__builder.Start<XsdValidatingReader.<ReadElementContentAsBase64Async>d__200>(ref <ReadElementContentAsBase64Async>d__);
			return <ReadElementContentAsBase64Async>d__.<>t__builder.Task;
		}

		public override Task<int> ReadElementContentAsBinHexAsync(byte[] buffer, int index, int count)
		{
			XsdValidatingReader.<ReadElementContentAsBinHexAsync>d__201 <ReadElementContentAsBinHexAsync>d__;
			<ReadElementContentAsBinHexAsync>d__.<>4__this = this;
			<ReadElementContentAsBinHexAsync>d__.buffer = buffer;
			<ReadElementContentAsBinHexAsync>d__.index = index;
			<ReadElementContentAsBinHexAsync>d__.count = count;
			<ReadElementContentAsBinHexAsync>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<ReadElementContentAsBinHexAsync>d__.<>1__state = -1;
			<ReadElementContentAsBinHexAsync>d__.<>t__builder.Start<XsdValidatingReader.<ReadElementContentAsBinHexAsync>d__201>(ref <ReadElementContentAsBinHexAsync>d__);
			return <ReadElementContentAsBinHexAsync>d__.<>t__builder.Task;
		}

		private Task ProcessReaderEventAsync()
		{
			if (this.replayCache)
			{
				return AsyncHelper.DoneTask;
			}
			switch (this.coreReader.NodeType)
			{
			case XmlNodeType.Element:
				return this.ProcessElementEventAsync();
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
				this.validator.ValidateText(new XmlValueGetter(this.GetStringValue));
				break;
			case XmlNodeType.EntityReference:
				throw new InvalidOperationException();
			case XmlNodeType.DocumentType:
				this.validator.SetDtdSchemaInfo(this.coreReader.DtdInfo);
				break;
			case XmlNodeType.Whitespace:
			case XmlNodeType.SignificantWhitespace:
				this.validator.ValidateWhitespace(new XmlValueGetter(this.GetStringValue));
				break;
			case XmlNodeType.EndElement:
				return this.ProcessEndElementEventAsync();
			}
			return AsyncHelper.DoneTask;
		}

		private Task ProcessElementEventAsync()
		{
			XsdValidatingReader.<ProcessElementEventAsync>d__203 <ProcessElementEventAsync>d__;
			<ProcessElementEventAsync>d__.<>4__this = this;
			<ProcessElementEventAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ProcessElementEventAsync>d__.<>1__state = -1;
			<ProcessElementEventAsync>d__.<>t__builder.Start<XsdValidatingReader.<ProcessElementEventAsync>d__203>(ref <ProcessElementEventAsync>d__);
			return <ProcessElementEventAsync>d__.<>t__builder.Task;
		}

		private Task ProcessEndElementEventAsync()
		{
			XsdValidatingReader.<ProcessEndElementEventAsync>d__204 <ProcessEndElementEventAsync>d__;
			<ProcessEndElementEventAsync>d__.<>4__this = this;
			<ProcessEndElementEventAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ProcessEndElementEventAsync>d__.<>1__state = -1;
			<ProcessEndElementEventAsync>d__.<>t__builder.Start<XsdValidatingReader.<ProcessEndElementEventAsync>d__204>(ref <ProcessEndElementEventAsync>d__);
			return <ProcessEndElementEventAsync>d__.<>t__builder.Task;
		}

		private Task ProcessInlineSchemaAsync()
		{
			XsdValidatingReader.<ProcessInlineSchemaAsync>d__205 <ProcessInlineSchemaAsync>d__;
			<ProcessInlineSchemaAsync>d__.<>4__this = this;
			<ProcessInlineSchemaAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ProcessInlineSchemaAsync>d__.<>1__state = -1;
			<ProcessInlineSchemaAsync>d__.<>t__builder.Start<XsdValidatingReader.<ProcessInlineSchemaAsync>d__205>(ref <ProcessInlineSchemaAsync>d__);
			return <ProcessInlineSchemaAsync>d__.<>t__builder.Task;
		}

		private Task<object> InternalReadContentAsObjectAsync()
		{
			return this.InternalReadContentAsObjectAsync(false);
		}

		private Task<object> InternalReadContentAsObjectAsync(bool unwrapTypedValue)
		{
			XsdValidatingReader.<InternalReadContentAsObjectAsync>d__207 <InternalReadContentAsObjectAsync>d__;
			<InternalReadContentAsObjectAsync>d__.<>4__this = this;
			<InternalReadContentAsObjectAsync>d__.unwrapTypedValue = unwrapTypedValue;
			<InternalReadContentAsObjectAsync>d__.<>t__builder = AsyncTaskMethodBuilder<object>.Create();
			<InternalReadContentAsObjectAsync>d__.<>1__state = -1;
			<InternalReadContentAsObjectAsync>d__.<>t__builder.Start<XsdValidatingReader.<InternalReadContentAsObjectAsync>d__207>(ref <InternalReadContentAsObjectAsync>d__);
			return <InternalReadContentAsObjectAsync>d__.<>t__builder.Task;
		}

		private Task<Tuple<string, object>> InternalReadContentAsObjectTupleAsync(bool unwrapTypedValue)
		{
			XsdValidatingReader.<InternalReadContentAsObjectTupleAsync>d__208 <InternalReadContentAsObjectTupleAsync>d__;
			<InternalReadContentAsObjectTupleAsync>d__.<>4__this = this;
			<InternalReadContentAsObjectTupleAsync>d__.unwrapTypedValue = unwrapTypedValue;
			<InternalReadContentAsObjectTupleAsync>d__.<>t__builder = AsyncTaskMethodBuilder<Tuple<string, object>>.Create();
			<InternalReadContentAsObjectTupleAsync>d__.<>1__state = -1;
			<InternalReadContentAsObjectTupleAsync>d__.<>t__builder.Start<XsdValidatingReader.<InternalReadContentAsObjectTupleAsync>d__208>(ref <InternalReadContentAsObjectTupleAsync>d__);
			return <InternalReadContentAsObjectTupleAsync>d__.<>t__builder.Task;
		}

		private Task<Tuple<XmlSchemaType, object>> InternalReadElementContentAsObjectAsync()
		{
			return this.InternalReadElementContentAsObjectAsync(false);
		}

		private Task<Tuple<XmlSchemaType, object>> InternalReadElementContentAsObjectAsync(bool unwrapTypedValue)
		{
			XsdValidatingReader.<InternalReadElementContentAsObjectAsync>d__210 <InternalReadElementContentAsObjectAsync>d__;
			<InternalReadElementContentAsObjectAsync>d__.<>4__this = this;
			<InternalReadElementContentAsObjectAsync>d__.unwrapTypedValue = unwrapTypedValue;
			<InternalReadElementContentAsObjectAsync>d__.<>t__builder = AsyncTaskMethodBuilder<Tuple<XmlSchemaType, object>>.Create();
			<InternalReadElementContentAsObjectAsync>d__.<>1__state = -1;
			<InternalReadElementContentAsObjectAsync>d__.<>t__builder.Start<XsdValidatingReader.<InternalReadElementContentAsObjectAsync>d__210>(ref <InternalReadElementContentAsObjectAsync>d__);
			return <InternalReadElementContentAsObjectAsync>d__.<>t__builder.Task;
		}

		private Task<Tuple<XmlSchemaType, string, object>> InternalReadElementContentAsObjectTupleAsync(bool unwrapTypedValue)
		{
			XsdValidatingReader.<InternalReadElementContentAsObjectTupleAsync>d__211 <InternalReadElementContentAsObjectTupleAsync>d__;
			<InternalReadElementContentAsObjectTupleAsync>d__.<>4__this = this;
			<InternalReadElementContentAsObjectTupleAsync>d__.unwrapTypedValue = unwrapTypedValue;
			<InternalReadElementContentAsObjectTupleAsync>d__.<>t__builder = AsyncTaskMethodBuilder<Tuple<XmlSchemaType, string, object>>.Create();
			<InternalReadElementContentAsObjectTupleAsync>d__.<>1__state = -1;
			<InternalReadElementContentAsObjectTupleAsync>d__.<>t__builder.Start<XsdValidatingReader.<InternalReadElementContentAsObjectTupleAsync>d__211>(ref <InternalReadElementContentAsObjectTupleAsync>d__);
			return <InternalReadElementContentAsObjectTupleAsync>d__.<>t__builder.Task;
		}

		private Task<object> ReadTillEndElementAsync()
		{
			XsdValidatingReader.<ReadTillEndElementAsync>d__212 <ReadTillEndElementAsync>d__;
			<ReadTillEndElementAsync>d__.<>4__this = this;
			<ReadTillEndElementAsync>d__.<>t__builder = AsyncTaskMethodBuilder<object>.Create();
			<ReadTillEndElementAsync>d__.<>1__state = -1;
			<ReadTillEndElementAsync>d__.<>t__builder.Start<XsdValidatingReader.<ReadTillEndElementAsync>d__212>(ref <ReadTillEndElementAsync>d__);
			return <ReadTillEndElementAsync>d__.<>t__builder.Task;
		}

		private XmlReader coreReader;

		private IXmlNamespaceResolver coreReaderNSResolver;

		private IXmlNamespaceResolver thisNSResolver;

		private XmlSchemaValidator validator;

		private XmlResolver xmlResolver;

		private ValidationEventHandler validationEvent;

		private XsdValidatingReader.ValidatingReaderState validationState;

		private XmlValueGetter valueGetter;

		private XmlNamespaceManager nsManager;

		private bool manageNamespaces;

		private bool processInlineSchema;

		private bool replayCache;

		private ValidatingReaderNodeData cachedNode;

		private AttributePSVIInfo attributePSVI;

		private int attributeCount;

		private int coreReaderAttributeCount;

		private int currentAttrIndex;

		private AttributePSVIInfo[] attributePSVINodes;

		private ArrayList defaultAttributes;

		private Parser inlineSchemaParser;

		private object atomicValue;

		private XmlSchemaInfo xmlSchemaInfo;

		private string originalAtomicValueString;

		private XmlNameTable coreReaderNameTable;

		private XsdCachingReader cachingReader;

		private ValidatingReaderNodeData textNode;

		private string NsXmlNs;

		private string NsXs;

		private string NsXsi;

		private string XsiType;

		private string XsiNil;

		private string XsdSchema;

		private string XsiSchemaLocation;

		private string XsiNoNamespaceSchemaLocation;

		private XmlCharType xmlCharType = XmlCharType.Instance;

		private IXmlLineInfo lineInfo;

		private ReadContentAsBinaryHelper readBinaryHelper;

		private XsdValidatingReader.ValidatingReaderState savedState;

		private const int InitialAttributeCount = 8;

		private static volatile Type TypeOfString;

		private enum ValidatingReaderState
		{
			None,
			Init,
			Read,
			OnDefaultAttribute = -1,
			OnReadAttributeValue = -2,
			OnAttribute = 3,
			ClearAttributes,
			ParseInlineSchema,
			ReadAhead,
			OnReadBinaryContent,
			ReaderClosed,
			EOF,
			Error
		}
	}
}
