using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace System.Runtime.Serialization.Json
{
	/// <summary>Serializes objects to the JavaScript Object Notation (JSON) and deserializes JSON data to objects. This class cannot be inherited.</summary>
	[TypeForwardedFrom("System.ServiceModel.Web, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")]
	public sealed class DataContractJsonSerializer : XmlObjectSerializer
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.Serialization.Json.DataContractJsonSerializer" /> class to serialize or deserialize an object of the specified type.</summary>
		/// <param name="type">The type of the instances that is serialized or deserialized.</param>
		public DataContractJsonSerializer(Type type) : this(type, null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.Serialization.Json.DataContractJsonSerializer" /> class to serialize or deserialize an object of a specified type using the XML root element specified by a parameter.</summary>
		/// <param name="type">The type of the instances that is serialized or deserialized.</param>
		/// <param name="rootName">The name of the XML element that encloses the content to serialize or deserialize.</param>
		public DataContractJsonSerializer(Type type, string rootName) : this(type, rootName, null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.Serialization.Json.DataContractJsonSerializer" /> class to serialize or deserialize an object of a specified type using the XML root element specified by a parameter of type <see cref="T:System.Xml.XmlDictionaryString" />.</summary>
		/// <param name="type">The type of the instances that is serialized or deserialized.</param>
		/// <param name="rootName">An <see cref="T:System.Xml.XmlDictionaryString" /> that contains the root element name of the content.</param>
		public DataContractJsonSerializer(Type type, XmlDictionaryString rootName) : this(type, rootName, null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.Serialization.Json.DataContractJsonSerializer" /> class to serialize or deserialize an object of the specified type, with a collection of known types that may be present in the object graph.</summary>
		/// <param name="type">The type of the instances that are serialized or deserialized.</param>
		/// <param name="knownTypes">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Type" /> that contains the types that may be present in the object graph.</param>
		public DataContractJsonSerializer(Type type, IEnumerable<Type> knownTypes) : this(type, knownTypes, int.MaxValue, false, null, false)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.Serialization.Json.DataContractJsonSerializer" /> class to serialize or deserialize an object of a specified type using the XML root element specified by a parameter, with a collection of known types that may be present in the object graph.</summary>
		/// <param name="type">The type of the instances that is serialized or deserialized.</param>
		/// <param name="rootName">The name of the XML element that encloses the content to serialize or deserialize. The default is "root".</param>
		/// <param name="knownTypes">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Type" /> that contains the types that may be present in the object graph.</param>
		public DataContractJsonSerializer(Type type, string rootName, IEnumerable<Type> knownTypes) : this(type, rootName, knownTypes, int.MaxValue, false, null, false)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.Serialization.Json.DataContractJsonSerializer" /> class to serialize or deserialize an object of a specified type using the XML root element specified by a parameter of type <see cref="T:System.Xml.XmlDictionaryString" />, with a collection of known types that may be present in the object graph.</summary>
		/// <param name="type">The type of the instances that is serialized or deserialized.</param>
		/// <param name="rootName">An <see cref="T:System.Xml.XmlDictionaryString" /> that contains the root element name of the content.</param>
		/// <param name="knownTypes">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Type" /> that contains the types that may be present in the object graph.</param>
		public DataContractJsonSerializer(Type type, XmlDictionaryString rootName, IEnumerable<Type> knownTypes) : this(type, rootName, knownTypes, int.MaxValue, false, null, false)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.Serialization.Json.DataContractJsonSerializer" /> class to serialize or deserialize an object of the specified type. This method also specifies a list of known types that may be present in the object graph, the maximum number of graph items to serialize or deserialize, whether to ignore unexpected data or emit type information, and a surrogate for custom serialization.</summary>
		/// <param name="type">The type of the instances that is serialized or deserialized.</param>
		/// <param name="knownTypes">An <see cref="T:System.Xml.XmlDictionaryString" /> that contains the root element name of the content.</param>
		/// <param name="maxItemsInObjectGraph">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Type" /> that contains the types that may be present in the object graph.</param>
		/// <param name="ignoreExtensionDataObject">
		///   <see langword="true" /> to ignore the <see cref="T:System.Runtime.Serialization.IExtensibleDataObject" /> interface upon serialization and ignore unexpected data upon deserialization; otherwise, <see langword="false" />. The default is <see langword="false" />.</param>
		/// <param name="dataContractSurrogate">An implementation of the <see cref="T:System.Runtime.Serialization.IDataContractSurrogate" /> to customize the serialization process.</param>
		/// <param name="alwaysEmitTypeInformation">
		///   <see langword="true" /> to emit type information; otherwise, <see langword="false" />. The default is <see langword="false" />.</param>
		public DataContractJsonSerializer(Type type, IEnumerable<Type> knownTypes, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, IDataContractSurrogate dataContractSurrogate, bool alwaysEmitTypeInformation)
		{
			EmitTypeInformation emitTypeInformation = alwaysEmitTypeInformation ? EmitTypeInformation.Always : EmitTypeInformation.AsNeeded;
			this.Initialize(type, knownTypes, maxItemsInObjectGraph, ignoreExtensionDataObject, dataContractSurrogate, emitTypeInformation, false, null, false);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.Serialization.Json.DataContractJsonSerializer" /> class to serialize or deserialize an object of the specified type. This method also specifies the root name of the XML element, a list of known types that may be present in the object graph, the maximum number of graph items to serialize or deserialize, whether to ignore unexpected data or emit type information, and a surrogate for custom serialization.</summary>
		/// <param name="type">The type of the instances that is serialized or deserialized.</param>
		/// <param name="rootName">The name of the XML element that encloses the content to serialize or deserialize. The default is "root".</param>
		/// <param name="knownTypes">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Type" /> that contains the types that may be present in the object graph.</param>
		/// <param name="maxItemsInObjectGraph">The maximum number of items in the graph to serialize or deserialize. The default is the value returned by the <see cref="F:System.Int32.MaxValue" /> property.</param>
		/// <param name="ignoreExtensionDataObject">
		///   <see langword="true" /> to ignore the <see cref="T:System.Runtime.Serialization.IExtensibleDataObject" /> interface upon serialization and ignore unexpected data upon deserialization; otherwise, <see langword="false" />. The default is <see langword="false" />.</param>
		/// <param name="dataContractSurrogate">An implementation of the <see cref="T:System.Runtime.Serialization.IDataContractSurrogate" /> to customize the serialization process.</param>
		/// <param name="alwaysEmitTypeInformation">
		///   <see langword="true" /> to emit type information; otherwise, <see langword="false" />. The default is <see langword="false" />.</param>
		public DataContractJsonSerializer(Type type, string rootName, IEnumerable<Type> knownTypes, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, IDataContractSurrogate dataContractSurrogate, bool alwaysEmitTypeInformation)
		{
			EmitTypeInformation emitTypeInformation = alwaysEmitTypeInformation ? EmitTypeInformation.Always : EmitTypeInformation.AsNeeded;
			XmlDictionary xmlDictionary = new XmlDictionary(2);
			this.Initialize(type, xmlDictionary.Add(rootName), knownTypes, maxItemsInObjectGraph, ignoreExtensionDataObject, dataContractSurrogate, emitTypeInformation, false, null, false);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.Serialization.Json.DataContractJsonSerializer" /> class to serialize or deserialize an object of the specified type. This method also specifies the root name of the XML element, a list of known types that may be present in the object graph, the maximum number of graph items to serialize or deserialize, whether to ignore unexpected data or emit type information, and a surrogate for custom serialization.</summary>
		/// <param name="type">The type of the instances that are serialized or deserialized.</param>
		/// <param name="rootName">An <see cref="T:System.Xml.XmlDictionaryString" /> that contains the root element name of the content.</param>
		/// <param name="knownTypes">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Type" /> that contains the known types that may be present in the object graph.</param>
		/// <param name="maxItemsInObjectGraph">The maximum number of items in the graph to serialize or deserialize. The default is the value returned by the <see cref="F:System.Int32.MaxValue" /> property.</param>
		/// <param name="ignoreExtensionDataObject">
		///   <see langword="true" /> to ignore the <see cref="T:System.Runtime.Serialization.IExtensibleDataObject" /> interface upon serialization and ignore unexpected data upon deserialization; otherwise, <see langword="false" />. The default is <see langword="false" />.</param>
		/// <param name="dataContractSurrogate">An implementation of the <see cref="T:System.Runtime.Serialization.IDataContractSurrogate" /> to customize the serialization process.</param>
		/// <param name="alwaysEmitTypeInformation">
		///   <see langword="true" /> to emit type information; otherwise, <see langword="false" />. The default is <see langword="false" />.</param>
		public DataContractJsonSerializer(Type type, XmlDictionaryString rootName, IEnumerable<Type> knownTypes, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, IDataContractSurrogate dataContractSurrogate, bool alwaysEmitTypeInformation)
		{
			EmitTypeInformation emitTypeInformation = alwaysEmitTypeInformation ? EmitTypeInformation.Always : EmitTypeInformation.AsNeeded;
			this.Initialize(type, rootName, knownTypes, maxItemsInObjectGraph, ignoreExtensionDataObject, dataContractSurrogate, emitTypeInformation, false, null, false);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.Serialization.Json.DataContractJsonSerializer" /> class to serialize or deserialize an object of the specified type and serializer settings.</summary>
		/// <param name="type">The type of the instances that is serialized or deserialized.</param>
		/// <param name="settings">The serializer settings for the JSON serializer.</param>
		public DataContractJsonSerializer(Type type, DataContractJsonSerializerSettings settings)
		{
			if (settings == null)
			{
				settings = new DataContractJsonSerializerSettings();
			}
			XmlDictionaryString xmlDictionaryString = (settings.RootName == null) ? null : new XmlDictionary(1).Add(settings.RootName);
			this.Initialize(type, xmlDictionaryString, settings.KnownTypes, settings.MaxItemsInObjectGraph, settings.IgnoreExtensionDataObject, settings.DataContractSurrogate, settings.EmitTypeInformation, settings.SerializeReadOnlyTypes, settings.DateTimeFormat, settings.UseSimpleDictionaryFormat);
		}

		/// <summary>Gets a surrogate type that is currently active for a given <see cref="T:System.Runtime.Serialization.IDataContractSurrogate" /> instance. Surrogates can extend the serialization or deserialization process.</summary>
		/// <returns>An implementation of the <see cref="T:System.Runtime.Serialization.IDataContractSurrogate" /> class.</returns>
		public IDataContractSurrogate DataContractSurrogate
		{
			get
			{
				return this.dataContractSurrogate;
			}
		}

		/// <summary>Gets a value that specifies whether unknown data is ignored on deserialization and whether the <see cref="T:System.Runtime.Serialization.IExtensibleDataObject" /> interface is ignored on serialization.</summary>
		/// <returns>
		///   <see langword="true" /> to ignore unknown data and <see cref="T:System.Runtime.Serialization.IExtensibleDataObject" />; otherwise, <see langword="false" />.</returns>
		public bool IgnoreExtensionDataObject
		{
			get
			{
				return this.ignoreExtensionDataObject;
			}
		}

		/// <summary>Gets a collection of types that may be present in the object graph serialized using this instance of the <see cref="T:System.Runtime.Serialization.Json.DataContractJsonSerializer" />.</summary>
		/// <returns>A <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> that contains the expected types passed in as known types to the <see cref="T:System.Runtime.Serialization.Json.DataContractJsonSerializer" /> constructor.</returns>
		public ReadOnlyCollection<Type> KnownTypes
		{
			get
			{
				if (this.knownTypeCollection == null)
				{
					if (this.knownTypeList != null)
					{
						this.knownTypeCollection = new ReadOnlyCollection<Type>(this.knownTypeList);
					}
					else
					{
						this.knownTypeCollection = new ReadOnlyCollection<Type>(Globals.EmptyTypeArray);
					}
				}
				return this.knownTypeCollection;
			}
		}

		internal override Dictionary<XmlQualifiedName, DataContract> KnownDataContracts
		{
			get
			{
				if (this.knownDataContracts == null && this.knownTypeList != null)
				{
					this.knownDataContracts = XmlObjectSerializerContext.GetDataContractsForKnownTypes(this.knownTypeList);
				}
				return this.knownDataContracts;
			}
		}

		/// <summary>Gets the maximum number of items in an object graph that the serializer serializes or deserializes in one read or write call.</summary>
		/// <returns>The maximum number of items to serialize or deserialize.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The number of items exceeds the maximum value.</exception>
		public int MaxItemsInObjectGraph
		{
			get
			{
				return this.maxItemsInObjectGraph;
			}
		}

		internal bool AlwaysEmitTypeInformation
		{
			get
			{
				return this.emitTypeInformation == EmitTypeInformation.Always;
			}
		}

		/// <summary>Gets or sets the data contract JSON serializer settings to emit type information.</summary>
		/// <returns>The data contract JSON serializer settings to emit type information.</returns>
		public EmitTypeInformation EmitTypeInformation
		{
			get
			{
				return this.emitTypeInformation;
			}
		}

		/// <summary>Gets or sets a value that specifies whether to serialize read only types.</summary>
		/// <returns>
		///   <see langword="true" /> to serialize read only types; otherwise <see langword="false" />.</returns>
		public bool SerializeReadOnlyTypes
		{
			get
			{
				return this.serializeReadOnlyTypes;
			}
		}

		/// <summary>Gets the format of the date and time type items in object graph.</summary>
		/// <returns>The format of the date and time type items in object graph.</returns>
		public DateTimeFormat DateTimeFormat
		{
			get
			{
				return this.dateTimeFormat;
			}
		}

		/// <summary>Gets a value that specifies whether to use a simple dictionary format.</summary>
		/// <returns>
		///   <see langword="true" /> to use a simple dictionary format; otherwise, <see langword="false" />.</returns>
		public bool UseSimpleDictionaryFormat
		{
			get
			{
				return this.useSimpleDictionaryFormat;
			}
		}

		private DataContract RootContract
		{
			get
			{
				if (this.rootContract == null)
				{
					this.rootContract = DataContract.GetDataContract((this.dataContractSurrogate == null) ? this.rootType : DataContractSerializer.GetSurrogatedType(this.dataContractSurrogate, this.rootType));
					DataContractJsonSerializer.CheckIfTypeIsReference(this.rootContract);
				}
				return this.rootContract;
			}
		}

		private XmlDictionaryString RootName
		{
			get
			{
				return this.rootName ?? JsonGlobals.rootDictionaryString;
			}
		}

		/// <summary>Determines whether the <see cref="T:System.Xml.XmlReader" /> is positioned on an object that can be deserialized.</summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> used to read the XML stream.</param>
		/// <returns>
		///   <see langword="true" /> if the reader is positioned correctly; otherwise, <see langword="false" />.</returns>
		public override bool IsStartObject(XmlReader reader)
		{
			return base.IsStartObjectHandleExceptions(new JsonReaderDelegator(reader));
		}

		/// <summary>Gets a value that specifies whether the <see cref="T:System.Xml.XmlDictionaryReader" /> is positioned over an XML element that represents an object the serializer can deserialize from.</summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlDictionaryReader" /> used to read the XML stream mapped from JSON.</param>
		/// <returns>
		///   <see langword="true" /> if the reader is positioned correctly; otherwise, <see langword="false" />.</returns>
		public override bool IsStartObject(XmlDictionaryReader reader)
		{
			return base.IsStartObjectHandleExceptions(new JsonReaderDelegator(reader));
		}

		/// <summary>Reads a document stream in the JSON (JavaScript Object Notation) format and returns the deserialized object.</summary>
		/// <param name="stream">The <see cref="T:System.IO.Stream" /> to be read.</param>
		/// <returns>The deserialized object.</returns>
		public override object ReadObject(Stream stream)
		{
			XmlObjectSerializer.CheckNull(stream, "stream");
			return this.ReadObject(JsonReaderWriterFactory.CreateJsonReader(stream, XmlDictionaryReaderQuotas.Max));
		}

		/// <summary>Reads the XML document mapped from JSON (JavaScript Object Notation) with an <see cref="T:System.Xml.XmlReader" /> and returns the deserialized object.</summary>
		/// <param name="reader">An <see cref="T:System.Xml.XmlReader" /> used to read the XML document mapped from JSON.</param>
		/// <returns>The deserialized object.</returns>
		public override object ReadObject(XmlReader reader)
		{
			return base.ReadObjectHandleExceptions(new JsonReaderDelegator(reader, this.DateTimeFormat), true);
		}

		/// <summary>Reads an XML document mapped from JSON with an <see cref="T:System.Xml.XmlReader" /> and returns the deserialized object; it also enables you to specify whether the serializer should verify that it is positioned on an appropriate element before attempting to deserialize.</summary>
		/// <param name="reader">An <see cref="T:System.Xml.XmlReader" /> used to read the XML document mapped from JSON.</param>
		/// <param name="verifyObjectName">
		///   <see langword="true" /> to check whether the enclosing XML element name and namespace correspond to the expected name and namespace; otherwise, <see langword="false" />, which skips the verification. The default is <see langword="true" />.</param>
		/// <returns>The deserialized object.</returns>
		public override object ReadObject(XmlReader reader, bool verifyObjectName)
		{
			return base.ReadObjectHandleExceptions(new JsonReaderDelegator(reader, this.DateTimeFormat), verifyObjectName);
		}

		/// <summary>Reads the XML document mapped from JSON (JavaScript Object Notation) with an <see cref="T:System.Xml.XmlDictionaryReader" /> and returns the deserialized object.</summary>
		/// <param name="reader">An <see cref="T:System.Xml.XmlDictionaryReader" /> used to read the XML document mapped from JSON.</param>
		/// <returns>The deserialized object.</returns>
		public override object ReadObject(XmlDictionaryReader reader)
		{
			return base.ReadObjectHandleExceptions(new JsonReaderDelegator(reader, this.DateTimeFormat), true);
		}

		/// <summary>Reads the XML document mapped from JSON with an <see cref="T:System.Xml.XmlDictionaryReader" /> and returns the deserialized object; it also enables you to specify whether the serializer should verify that it is positioned on an appropriate element before attempting to deserialize.</summary>
		/// <param name="reader">An <see cref="T:System.Xml.XmlDictionaryReader" /> used to read the XML document mapped from JSON.</param>
		/// <param name="verifyObjectName">
		///   <see langword="true" /> to check whether the enclosing XML element name and namespace correspond to the expected name and namespace; otherwise, <see langword="false" /> to skip the verification. The default is <see langword="true" />.</param>
		/// <returns>The deserialized object.</returns>
		public override object ReadObject(XmlDictionaryReader reader, bool verifyObjectName)
		{
			return base.ReadObjectHandleExceptions(new JsonReaderDelegator(reader, this.DateTimeFormat), verifyObjectName);
		}

		/// <summary>Writes the closing XML element to an XML document, using an <see cref="T:System.Xml.XmlWriter" />, which can be mapped to JavaScript Object Notation (JSON).</summary>
		/// <param name="writer">An <see cref="T:System.Xml.XmlWriter" /> used to write the XML document mapped to JSON.</param>
		public override void WriteEndObject(XmlWriter writer)
		{
			base.WriteEndObjectHandleExceptions(new JsonWriterDelegator(writer));
		}

		/// <summary>Writes the closing XML element to an XML document, using an <see cref="T:System.Xml.XmlDictionaryWriter" />, which can be mapped to JavaScript Object Notation (JSON).</summary>
		/// <param name="writer">An <see cref="T:System.Xml.XmlDictionaryWriter" /> used to write the XML document to map to JSON.</param>
		public override void WriteEndObject(XmlDictionaryWriter writer)
		{
			base.WriteEndObjectHandleExceptions(new JsonWriterDelegator(writer));
		}

		/// <summary>Serializes a specified object to JavaScript Object Notation (JSON) data and writes the resulting JSON to a stream.</summary>
		/// <param name="stream">The <see cref="T:System.IO.Stream" /> that is written to.</param>
		/// <param name="graph">The object that contains the data to write to the stream.</param>
		/// <exception cref="T:System.Runtime.Serialization.InvalidDataContractException">The type being serialized does not conform to data contract rules. For example, the <see cref="T:System.Runtime.Serialization.DataContractAttribute" /> attribute has not been applied to the type.</exception>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">There is a problem with the instance being written.</exception>
		/// <exception cref="T:System.ServiceModel.QuotaExceededException">The maximum number of objects to serialize has been exceeded. Check the <see cref="P:System.Runtime.Serialization.DataContractSerializer.MaxItemsInObjectGraph" /> property.</exception>
		public override void WriteObject(Stream stream, object graph)
		{
			XmlObjectSerializer.CheckNull(stream, "stream");
			XmlDictionaryWriter xmlDictionaryWriter = JsonReaderWriterFactory.CreateJsonWriter(stream, Encoding.UTF8, false);
			this.WriteObject(xmlDictionaryWriter, graph);
			xmlDictionaryWriter.Flush();
		}

		/// <summary>Serializes an object to XML that may be mapped to JavaScript Object Notation (JSON). Writes all the object data, including the starting XML element, content, and closing element, with an <see cref="T:System.Xml.XmlWriter" />.</summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> used to write the XML document to map to JSON.</param>
		/// <param name="graph">The object that contains the data to write.</param>
		/// <exception cref="T:System.Runtime.Serialization.InvalidDataContractException">The type being serialized does not conform to data contract rules. For example, the <see cref="T:System.Runtime.Serialization.DataContractAttribute" /> attribute has not been applied to the type.</exception>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">There is a problem with the instance being written.</exception>
		/// <exception cref="T:System.ServiceModel.QuotaExceededException">The maximum number of objects to serialize has been exceeded. Check the <see cref="P:System.Runtime.Serialization.DataContractSerializer.MaxItemsInObjectGraph" /> property.</exception>
		public override void WriteObject(XmlWriter writer, object graph)
		{
			base.WriteObjectHandleExceptions(new JsonWriterDelegator(writer, this.DateTimeFormat), graph);
		}

		/// <summary>Serializes an object to XML that may be mapped to JavaScript Object Notation (JSON). Writes all the object data, including the starting XML element, content, and closing element, with an <see cref="T:System.Xml.XmlDictionaryWriter" />.</summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlDictionaryWriter" /> used to write the XML document or stream to map to JSON.</param>
		/// <param name="graph">The object that contains the data to write.</param>
		/// <exception cref="T:System.Runtime.Serialization.InvalidDataContractException">The type being serialized does not conform to data contract rules. For example, the <see cref="T:System.Runtime.Serialization.DataContractAttribute" /> attribute has not been applied to the type.</exception>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">There is a problem with the instance being written.</exception>
		/// <exception cref="T:System.ServiceModel.QuotaExceededException">The maximum number of objects to serialize has been exceeded. Check the <see cref="P:System.Runtime.Serialization.DataContractSerializer.MaxItemsInObjectGraph" /> property.</exception>
		public override void WriteObject(XmlDictionaryWriter writer, object graph)
		{
			base.WriteObjectHandleExceptions(new JsonWriterDelegator(writer, this.DateTimeFormat), graph);
		}

		/// <summary>Writes the XML content that can be mapped to JavaScript Object Notation (JSON) using an <see cref="T:System.Xml.XmlWriter" />.</summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> used to write to.</param>
		/// <param name="graph">The object to write.</param>
		/// <exception cref="T:System.Runtime.Serialization.InvalidDataContractException">The type being serialized does not conform to data contract rules. For example, the <see cref="T:System.Runtime.Serialization.DataContractAttribute" /> attribute has not been applied to the type.</exception>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">There is a problem with the instance being written.</exception>
		/// <exception cref="T:System.ServiceModel.QuotaExceededException">The maximum number of objects to serialize has been exceeded. Check the <see cref="P:System.Runtime.Serialization.DataContractSerializer.MaxItemsInObjectGraph" /> property.</exception>
		public override void WriteObjectContent(XmlWriter writer, object graph)
		{
			base.WriteObjectContentHandleExceptions(new JsonWriterDelegator(writer, this.DateTimeFormat), graph);
		}

		/// <summary>Writes the XML content that can be mapped to JavaScript Object Notation (JSON) using an <see cref="T:System.Xml.XmlDictionaryWriter" />.</summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlDictionaryWriter" /> to write to.</param>
		/// <param name="graph">The object to write.</param>
		/// <exception cref="T:System.Runtime.Serialization.InvalidDataContractException">The type being serialized does not conform to data contract rules. For example, the <see cref="T:System.Runtime.Serialization.DataContractAttribute" /> attribute has not been applied to the type.</exception>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">There is a problem with the instance being written.</exception>
		/// <exception cref="T:System.ServiceModel.QuotaExceededException">The maximum number of objects to serialize has been exceeded. Check the <see cref="P:System.Runtime.Serialization.DataContractSerializer.MaxItemsInObjectGraph" /> property.</exception>
		public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
		{
			base.WriteObjectContentHandleExceptions(new JsonWriterDelegator(writer, this.DateTimeFormat), graph);
		}

		/// <summary>Writes the opening XML element for serializing an object to XML that can be mapped to JavaScript Object Notation (JSON) using an <see cref="T:System.Xml.XmlWriter" />.</summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> used to write the XML start element.</param>
		/// <param name="graph">The object to write.</param>
		public override void WriteStartObject(XmlWriter writer, object graph)
		{
			base.WriteStartObjectHandleExceptions(new JsonWriterDelegator(writer), graph);
		}

		/// <summary>Writes the opening XML element for serializing an object to XML that can be mapped to JavaScript Object Notation (JSON) using an <see cref="T:System.Xml.XmlDictionaryWriter" />.</summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlDictionaryWriter" /> used to write the XML start element.</param>
		/// <param name="graph">The object to write.</param>
		public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
		{
			base.WriteStartObjectHandleExceptions(new JsonWriterDelegator(writer), graph);
		}

		internal static bool CheckIfJsonNameRequiresMapping(string jsonName)
		{
			if (jsonName != null)
			{
				if (!DataContract.IsValidNCName(jsonName))
				{
					return true;
				}
				for (int i = 0; i < jsonName.Length; i++)
				{
					if (XmlJsonWriter.CharacterNeedsEscaping(jsonName[i]))
					{
						return true;
					}
				}
			}
			return false;
		}

		internal static bool CheckIfJsonNameRequiresMapping(XmlDictionaryString jsonName)
		{
			return jsonName != null && DataContractJsonSerializer.CheckIfJsonNameRequiresMapping(jsonName.Value);
		}

		internal static bool CheckIfXmlNameRequiresMapping(string xmlName)
		{
			return xmlName != null && DataContractJsonSerializer.CheckIfJsonNameRequiresMapping(DataContractJsonSerializer.ConvertXmlNameToJsonName(xmlName));
		}

		internal static bool CheckIfXmlNameRequiresMapping(XmlDictionaryString xmlName)
		{
			return xmlName != null && DataContractJsonSerializer.CheckIfXmlNameRequiresMapping(xmlName.Value);
		}

		internal static string ConvertXmlNameToJsonName(string xmlName)
		{
			return XmlConvert.DecodeName(xmlName);
		}

		internal static XmlDictionaryString ConvertXmlNameToJsonName(XmlDictionaryString xmlName)
		{
			if (xmlName != null)
			{
				return new XmlDictionary().Add(DataContractJsonSerializer.ConvertXmlNameToJsonName(xmlName.Value));
			}
			return null;
		}

		internal static bool IsJsonLocalName(XmlReaderDelegator reader, string elementName)
		{
			string b;
			return XmlObjectSerializerReadContextComplexJson.TryGetJsonLocalName(reader, out b) && elementName == b;
		}

		internal static object ReadJsonValue(DataContract contract, XmlReaderDelegator reader, XmlObjectSerializerReadContextComplexJson context)
		{
			return JsonDataContract.GetJsonDataContract(contract).ReadJsonValue(reader, context);
		}

		internal static void WriteJsonNull(XmlWriterDelegator writer)
		{
			writer.WriteAttributeString(null, "type", null, "null");
		}

		internal static void WriteJsonValue(JsonDataContract contract, XmlWriterDelegator writer, object graph, XmlObjectSerializerWriteContextComplexJson context, RuntimeTypeHandle declaredTypeHandle)
		{
			contract.WriteJsonValue(writer, graph, context, declaredTypeHandle);
		}

		internal override Type GetDeserializeType()
		{
			return this.rootType;
		}

		internal override Type GetSerializeType(object graph)
		{
			if (graph != null)
			{
				return graph.GetType();
			}
			return this.rootType;
		}

		internal override bool InternalIsStartObject(XmlReaderDelegator reader)
		{
			return base.IsRootElement(reader, this.RootContract, this.RootName, XmlDictionaryString.Empty) || DataContractJsonSerializer.IsJsonLocalName(reader, this.RootName.Value);
		}

		internal override object InternalReadObject(XmlReaderDelegator xmlReader, bool verifyObjectName)
		{
			if (this.MaxItemsInObjectGraph == 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.GetString("Maximum number of items that can be serialized or deserialized in an object graph is '{0}'.", new object[]
				{
					this.MaxItemsInObjectGraph
				})));
			}
			if (verifyObjectName)
			{
				if (!this.InternalIsStartObject(xmlReader))
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationExceptionWithReaderDetails(SR.GetString("Expecting element '{1}' from namespace '{0}'.", new object[]
					{
						XmlDictionaryString.Empty,
						this.RootName
					}), xmlReader));
				}
			}
			else if (!base.IsStartElement(xmlReader))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationExceptionWithReaderDetails(SR.GetString("Expecting state '{0}' when ReadObject is called.", new object[]
				{
					XmlNodeType.Element
				}), xmlReader));
			}
			DataContract dataContract = this.RootContract;
			if (dataContract.IsPrimitive && dataContract.UnderlyingType == this.rootType)
			{
				return DataContractJsonSerializer.ReadJsonValue(dataContract, xmlReader, null);
			}
			return XmlObjectSerializerReadContextComplexJson.CreateContext(this, dataContract).InternalDeserialize(xmlReader, this.rootType, dataContract, null, null);
		}

		internal override void InternalWriteEndObject(XmlWriterDelegator writer)
		{
			writer.WriteEndElement();
		}

		internal override void InternalWriteObject(XmlWriterDelegator writer, object graph)
		{
			this.InternalWriteStartObject(writer, graph);
			this.InternalWriteObjectContent(writer, graph);
			this.InternalWriteEndObject(writer);
		}

		internal override void InternalWriteObjectContent(XmlWriterDelegator writer, object graph)
		{
			if (this.MaxItemsInObjectGraph == 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.GetString("Maximum number of items that can be serialized or deserialized in an object graph is '{0}'.", new object[]
				{
					this.MaxItemsInObjectGraph
				})));
			}
			DataContract dataContract = this.RootContract;
			Type underlyingType = dataContract.UnderlyingType;
			Type type = (graph == null) ? underlyingType : graph.GetType();
			if (this.dataContractSurrogate != null)
			{
				graph = DataContractSerializer.SurrogateToDataContractType(this.dataContractSurrogate, graph, underlyingType, ref type);
			}
			if (graph == null)
			{
				DataContractJsonSerializer.WriteJsonNull(writer);
				return;
			}
			if (underlyingType == type)
			{
				if (dataContract.CanContainReferences)
				{
					XmlObjectSerializerWriteContextComplexJson xmlObjectSerializerWriteContextComplexJson = XmlObjectSerializerWriteContextComplexJson.CreateContext(this, dataContract);
					xmlObjectSerializerWriteContextComplexJson.OnHandleReference(writer, graph, true);
					xmlObjectSerializerWriteContextComplexJson.SerializeWithoutXsiType(dataContract, writer, graph, underlyingType.TypeHandle);
					return;
				}
				DataContractJsonSerializer.WriteJsonValue(JsonDataContract.GetJsonDataContract(dataContract), writer, graph, null, underlyingType.TypeHandle);
				return;
			}
			else
			{
				XmlObjectSerializerWriteContextComplexJson xmlObjectSerializerWriteContextComplexJson2 = XmlObjectSerializerWriteContextComplexJson.CreateContext(this, this.RootContract);
				dataContract = DataContractJsonSerializer.GetDataContract(dataContract, underlyingType, type);
				if (dataContract.CanContainReferences)
				{
					xmlObjectSerializerWriteContextComplexJson2.OnHandleReference(writer, graph, true);
					xmlObjectSerializerWriteContextComplexJson2.SerializeWithXsiTypeAtTopLevel(dataContract, writer, graph, underlyingType.TypeHandle, type);
					return;
				}
				xmlObjectSerializerWriteContextComplexJson2.SerializeWithoutXsiType(dataContract, writer, graph, underlyingType.TypeHandle);
				return;
			}
		}

		internal override void InternalWriteStartObject(XmlWriterDelegator writer, object graph)
		{
			if (this.rootNameRequiresMapping)
			{
				writer.WriteStartElement("a", "item", "item");
				writer.WriteAttributeString(null, "item", null, this.RootName.Value);
				return;
			}
			writer.WriteStartElement(this.RootName, XmlDictionaryString.Empty);
		}

		private void AddCollectionItemTypeToKnownTypes(Type knownType)
		{
			Type type = knownType;
			Type type2;
			while (CollectionDataContract.IsCollection(type, out type2))
			{
				if (type2.IsGenericType && type2.GetGenericTypeDefinition() == Globals.TypeOfKeyValue)
				{
					type2 = Globals.TypeOfKeyValuePair.MakeGenericType(type2.GetGenericArguments());
				}
				this.knownTypeList.Add(type2);
				type = type2;
			}
		}

		private void Initialize(Type type, IEnumerable<Type> knownTypes, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, IDataContractSurrogate dataContractSurrogate, EmitTypeInformation emitTypeInformation, bool serializeReadOnlyTypes, DateTimeFormat dateTimeFormat, bool useSimpleDictionaryFormat)
		{
			XmlObjectSerializer.CheckNull(type, "type");
			this.rootType = type;
			if (knownTypes != null)
			{
				this.knownTypeList = new List<Type>();
				foreach (Type type2 in knownTypes)
				{
					this.knownTypeList.Add(type2);
					if (type2 != null)
					{
						this.AddCollectionItemTypeToKnownTypes(type2);
					}
				}
			}
			if (maxItemsInObjectGraph < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("maxItemsInObjectGraph", SR.GetString("The value of this argument must be non-negative.")));
			}
			this.maxItemsInObjectGraph = maxItemsInObjectGraph;
			this.ignoreExtensionDataObject = ignoreExtensionDataObject;
			this.dataContractSurrogate = dataContractSurrogate;
			this.emitTypeInformation = emitTypeInformation;
			this.serializeReadOnlyTypes = serializeReadOnlyTypes;
			this.dateTimeFormat = dateTimeFormat;
			this.useSimpleDictionaryFormat = useSimpleDictionaryFormat;
		}

		private void Initialize(Type type, XmlDictionaryString rootName, IEnumerable<Type> knownTypes, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, IDataContractSurrogate dataContractSurrogate, EmitTypeInformation emitTypeInformation, bool serializeReadOnlyTypes, DateTimeFormat dateTimeFormat, bool useSimpleDictionaryFormat)
		{
			this.Initialize(type, knownTypes, maxItemsInObjectGraph, ignoreExtensionDataObject, dataContractSurrogate, emitTypeInformation, serializeReadOnlyTypes, dateTimeFormat, useSimpleDictionaryFormat);
			this.rootName = DataContractJsonSerializer.ConvertXmlNameToJsonName(rootName);
			this.rootNameRequiresMapping = DataContractJsonSerializer.CheckIfJsonNameRequiresMapping(this.rootName);
		}

		internal static void CheckIfTypeIsReference(DataContract dataContract)
		{
			if (dataContract.IsReference)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.GetString("Unsupported value for IsReference for type '{0}', IsReference value is {1}.", new object[]
				{
					DataContract.GetClrTypeFullName(dataContract.UnderlyingType),
					dataContract.IsReference
				})));
			}
		}

		internal static DataContract GetDataContract(DataContract declaredTypeContract, Type declaredType, Type objectType)
		{
			DataContract dataContract = DataContractSerializer.GetDataContract(declaredTypeContract, declaredType, objectType);
			DataContractJsonSerializer.CheckIfTypeIsReference(dataContract);
			return dataContract;
		}

		internal IList<Type> knownTypeList;

		internal Dictionary<XmlQualifiedName, DataContract> knownDataContracts;

		private EmitTypeInformation emitTypeInformation;

		private IDataContractSurrogate dataContractSurrogate;

		private bool ignoreExtensionDataObject;

		private ReadOnlyCollection<Type> knownTypeCollection;

		private int maxItemsInObjectGraph;

		private DataContract rootContract;

		private XmlDictionaryString rootName;

		private bool rootNameRequiresMapping;

		private Type rootType;

		private bool serializeReadOnlyTypes;

		private DateTimeFormat dateTimeFormat;

		private bool useSimpleDictionaryFormat;
	}
}
