using System;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	/// <summary>Represents XML data stored in or retrieved from a server.</summary>
	[XmlSchemaProvider("GetXsdType")]
	[Serializable]
	public sealed class SqlXml : INullable, IXmlSerializable
	{
		/// <summary>Creates a new <see cref="T:System.Data.SqlTypes.SqlXml" /> instance.</summary>
		public SqlXml()
		{
			this.SetNull();
		}

		private SqlXml(bool fNull)
		{
			this.SetNull();
		}

		/// <summary>Creates a new <see cref="T:System.Data.SqlTypes.SqlXml" /> instance and associates it with the content of the supplied <see cref="T:System.Xml.XmlReader" />.</summary>
		/// <param name="value">An <see cref="T:System.Xml.XmlReader" />-derived class instance to be used as the value of the new <see cref="T:System.Data.SqlTypes.SqlXml" /> instance.</param>
		public SqlXml(XmlReader value)
		{
			if (value == null)
			{
				this.SetNull();
				return;
			}
			this._fNotNull = true;
			this._firstCreateReader = true;
			this._stream = this.CreateMemoryStreamFromXmlReader(value);
		}

		/// <summary>Creates a new <see cref="T:System.Data.SqlTypes.SqlXml" /> instance, supplying the XML value from the supplied <see cref="T:System.IO.Stream" />-derived instance.</summary>
		/// <param name="value">A <see cref="T:System.IO.Stream" />-derived instance (such as <see cref="T:System.IO.FileStream" />) from which to load the <see cref="T:System.Data.SqlTypes.SqlXml" /> instance's Xml content.</param>
		public SqlXml(Stream value)
		{
			if (value == null)
			{
				this.SetNull();
				return;
			}
			this._firstCreateReader = true;
			this._fNotNull = true;
			this._stream = value;
		}

		/// <summary>Gets the value of the XML content of this <see cref="T:System.Data.SqlTypes.SqlXml" /> as a <see cref="T:System.Xml.XmlReader" />.</summary>
		/// <returns>A <see cref="T:System.Xml.XmlReader" />-derived instance that contains the XML content. The actual type may vary (for example, the return value might be <see cref="T:System.Xml.XmlTextReader" />) depending on how the information is represented internally, on the server.</returns>
		/// <exception cref="T:System.Data.SqlTypes.SqlNullValueException">Attempt was made to access this property on a null instance of <see cref="T:System.Data.SqlTypes.SqlXml" />.</exception>
		public XmlReader CreateReader()
		{
			if (this.IsNull)
			{
				throw new SqlNullValueException();
			}
			SqlXmlStreamWrapper sqlXmlStreamWrapper = new SqlXmlStreamWrapper(this._stream);
			if ((!this._firstCreateReader || sqlXmlStreamWrapper.CanSeek) && sqlXmlStreamWrapper.Position != 0L)
			{
				sqlXmlStreamWrapper.Seek(0L, SeekOrigin.Begin);
			}
			if (this._createSqlReaderMethodInfo == null)
			{
				this._createSqlReaderMethodInfo = SqlXml.CreateSqlReaderMethodInfo;
			}
			XmlReader result = SqlXml.CreateSqlXmlReader(sqlXmlStreamWrapper, false, false);
			this._firstCreateReader = false;
			return result;
		}

		internal static XmlReader CreateSqlXmlReader(Stream stream, bool closeInput = false, bool throwTargetInvocationExceptions = false)
		{
			XmlReaderSettings arg = closeInput ? SqlXml.s_defaultXmlReaderSettingsCloseInput : SqlXml.s_defaultXmlReaderSettings;
			XmlReader result;
			try
			{
				result = SqlXml.s_sqlReaderDelegate(stream, arg, null);
			}
			catch (Exception ex)
			{
				if (!throwTargetInvocationExceptions || !ADP.IsCatchableExceptionType(ex))
				{
					throw;
				}
				throw new TargetInvocationException(ex);
			}
			return result;
		}

		private static Func<Stream, XmlReaderSettings, XmlParserContext, XmlReader> CreateSqlReaderDelegate()
		{
			return (Func<Stream, XmlReaderSettings, XmlParserContext, XmlReader>)SqlXml.CreateSqlReaderMethodInfo.CreateDelegate(typeof(Func<Stream, XmlReaderSettings, XmlParserContext, XmlReader>));
		}

		private static MethodInfo CreateSqlReaderMethodInfo
		{
			get
			{
				if (SqlXml.s_createSqlReaderMethodInfo == null)
				{
					SqlXml.s_createSqlReaderMethodInfo = typeof(XmlReader).GetMethod("CreateSqlReader", BindingFlags.Static | BindingFlags.NonPublic);
				}
				return SqlXml.s_createSqlReaderMethodInfo;
			}
		}

		/// <summary>Indicates whether this instance represents a null <see cref="T:System.Data.SqlTypes.SqlXml" /> value.</summary>
		/// <returns>
		///   <see langword="true" /> if <see langword="Value" /> is null. Otherwise, <see langword="false" />.</returns>
		public bool IsNull
		{
			get
			{
				return !this._fNotNull;
			}
		}

		/// <summary>Gets the string representation of the XML content of this <see cref="T:System.Data.SqlTypes.SqlXml" /> instance.</summary>
		/// <returns>The string representation of the XML content.</returns>
		public string Value
		{
			get
			{
				if (this.IsNull)
				{
					throw new SqlNullValueException();
				}
				StringWriter stringWriter = new StringWriter(null);
				XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
				{
					CloseOutput = false,
					ConformanceLevel = ConformanceLevel.Fragment
				});
				XmlReader xmlReader = this.CreateReader();
				if (xmlReader.ReadState == ReadState.Initial)
				{
					xmlReader.Read();
				}
				while (!xmlReader.EOF)
				{
					xmlWriter.WriteNode(xmlReader, true);
				}
				xmlWriter.Flush();
				return stringWriter.ToString();
			}
		}

		/// <summary>Represents a null instance of the <see cref="T:System.Data.SqlTypes.SqlXml" /> type.</summary>
		/// <returns>A null instance of the <see cref="T:System.Data.SqlTypes.SqlXml" /> type.</returns>
		public static SqlXml Null
		{
			get
			{
				return new SqlXml(true);
			}
		}

		private void SetNull()
		{
			this._fNotNull = false;
			this._stream = null;
			this._firstCreateReader = true;
		}

		private Stream CreateMemoryStreamFromXmlReader(XmlReader reader)
		{
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.CloseOutput = false;
			xmlWriterSettings.ConformanceLevel = ConformanceLevel.Fragment;
			xmlWriterSettings.Encoding = Encoding.GetEncoding("utf-16");
			xmlWriterSettings.OmitXmlDeclaration = true;
			MemoryStream memoryStream = new MemoryStream();
			XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
			if (reader.ReadState == ReadState.Closed)
			{
				throw new InvalidOperationException(SQLResource.ClosedXmlReaderMessage);
			}
			if (reader.ReadState == ReadState.Initial)
			{
				reader.Read();
			}
			while (!reader.EOF)
			{
				xmlWriter.WriteNode(reader, true);
			}
			xmlWriter.Flush();
			memoryStream.Seek(0L, SeekOrigin.Begin);
			return memoryStream;
		}

		/// <summary>For a description of this member, see <see cref="M:System.Xml.Serialization.IXmlSerializable.GetSchema" />.</summary>
		/// <returns>An <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" /> method.</returns>
		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		/// <summary>For a description of this member, see <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" />.</summary>
		/// <param name="r">An XmlReader.</param>
		void IXmlSerializable.ReadXml(XmlReader r)
		{
			string attribute = r.GetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance");
			if (attribute != null && XmlConvert.ToBoolean(attribute))
			{
				r.ReadInnerXml();
				this.SetNull();
				return;
			}
			this._fNotNull = true;
			this._firstCreateReader = true;
			this._stream = new MemoryStream();
			StreamWriter streamWriter = new StreamWriter(this._stream);
			streamWriter.Write(r.ReadInnerXml());
			streamWriter.Flush();
			if (this._stream.CanSeek)
			{
				this._stream.Seek(0L, SeekOrigin.Begin);
			}
		}

		/// <summary>For a description of this member, see <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" />.</summary>
		/// <param name="writer">An XmlWriter</param>
		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			if (this.IsNull)
			{
				writer.WriteAttributeString("xsi", "nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
			}
			else
			{
				XmlReader xmlReader = this.CreateReader();
				if (xmlReader.ReadState == ReadState.Initial)
				{
					xmlReader.Read();
				}
				while (!xmlReader.EOF)
				{
					writer.WriteNode(xmlReader, true);
				}
			}
			writer.Flush();
		}

		/// <summary>Returns the XML Schema definition language (XSD) of the specified <see cref="T:System.Xml.Schema.XmlSchemaSet" />.</summary>
		/// <param name="schemaSet">An <see cref="T:System.Xml.Schema.XmlSchemaSet" />.</param>
		/// <returns>A string that indicates the XSD of the specified <see cref="T:System.Xml.Schema.XmlSchemaSet" />.</returns>
		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			return new XmlQualifiedName("anyType", "http://www.w3.org/2001/XMLSchema");
		}

		private static readonly Func<Stream, XmlReaderSettings, XmlParserContext, XmlReader> s_sqlReaderDelegate = SqlXml.CreateSqlReaderDelegate();

		private static readonly XmlReaderSettings s_defaultXmlReaderSettings = new XmlReaderSettings
		{
			ConformanceLevel = ConformanceLevel.Fragment
		};

		private static readonly XmlReaderSettings s_defaultXmlReaderSettingsCloseInput = new XmlReaderSettings
		{
			ConformanceLevel = ConformanceLevel.Fragment,
			CloseInput = true
		};

		private static MethodInfo s_createSqlReaderMethodInfo;

		private MethodInfo _createSqlReaderMethodInfo;

		private bool _fNotNull;

		private Stream _stream;

		private bool _firstCreateReader;
	}
}
