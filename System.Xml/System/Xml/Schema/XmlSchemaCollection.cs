using System;
using System.Collections;
using System.Threading;
using System.Xml.XmlConfiguration;

namespace System.Xml.Schema
{
	/// <summary>Contains a cache of XML Schema definition language (XSD) and XML-Data Reduced (XDR) schemas. The <see cref="T:System.Xml.Schema.XmlSchemaCollection" /> class class is obsolete. Use <see cref="T:System.Xml.Schema.XmlSchemaSet" /> instead.</summary>
	[Obsolete("Use System.Xml.Schema.XmlSchemaSet for schema compilation and validation. https://go.microsoft.com/fwlink/?linkid=14202")]
	public sealed class XmlSchemaCollection : ICollection, IEnumerable
	{
		/// <summary>Initializes a new instance of the <see langword="XmlSchemaCollection" /> class.</summary>
		public XmlSchemaCollection() : this(new NameTable())
		{
		}

		/// <summary>Initializes a new instance of the <see langword="XmlSchemaCollection" /> class with the specified <see cref="T:System.Xml.XmlNameTable" />. The <see langword="XmlNameTable" /> is used when loading schemas.</summary>
		/// <param name="nametable">The <see langword="XmlNameTable" /> to use. </param>
		public XmlSchemaCollection(XmlNameTable nametable)
		{
			if (nametable == null)
			{
				throw new ArgumentNullException("nametable");
			}
			this.nameTable = nametable;
			this.collection = Hashtable.Synchronized(new Hashtable());
			this.xmlResolver = XmlReaderSection.CreateDefaultResolver();
			this.isThreadSafe = true;
			if (this.isThreadSafe)
			{
				this.wLock = new ReaderWriterLock();
			}
		}

		/// <summary>Gets the number of namespaces defined in this collection.</summary>
		/// <returns>The number of namespaces defined in this collection.</returns>
		public int Count
		{
			get
			{
				return this.collection.Count;
			}
		}

		/// <summary>Gets the default <see langword="XmlNameTable" /> used by the <see langword="XmlSchemaCollection" /> when loading new schemas.</summary>
		/// <returns>An <see langword="XmlNameTable" />.</returns>
		public XmlNameTable NameTable
		{
			get
			{
				return this.nameTable;
			}
		}

		/// <summary>Sets an event handler for receiving information about the XDR and XML schema validation errors.</summary>
		public event ValidationEventHandler ValidationEventHandler
		{
			add
			{
				this.validationEventHandler = (ValidationEventHandler)Delegate.Combine(this.validationEventHandler, value);
			}
			remove
			{
				this.validationEventHandler = (ValidationEventHandler)Delegate.Remove(this.validationEventHandler, value);
			}
		}

		internal XmlResolver XmlResolver
		{
			set
			{
				this.xmlResolver = value;
			}
		}

		/// <summary>Adds the schema located by the given URL into the schema collection.</summary>
		/// <param name="ns">The namespace URI associated with the schema. For XML Schemas, this will typically be the <see langword="targetNamespace" />. </param>
		/// <param name="uri">The URL that specifies the schema to load. </param>
		/// <returns>The <see cref="T:System.Xml.Schema.XmlSchema" /> added to the schema collection; <see langword="null" /> if the schema being added is an XDR schema or if there are compilation errors in the schema. </returns>
		/// <exception cref="T:System.Xml.XmlException">The schema is not a valid schema. </exception>
		public XmlSchema Add(string ns, string uri)
		{
			if (uri == null || uri.Length == 0)
			{
				throw new ArgumentNullException("uri");
			}
			XmlTextReader xmlTextReader = new XmlTextReader(uri, this.nameTable);
			xmlTextReader.XmlResolver = this.xmlResolver;
			XmlSchema result = null;
			try
			{
				result = this.Add(ns, xmlTextReader, this.xmlResolver);
				while (xmlTextReader.Read())
				{
				}
			}
			finally
			{
				xmlTextReader.Close();
			}
			return result;
		}

		/// <summary>Adds the schema contained in the <see cref="T:System.Xml.XmlReader" /> to the schema collection.</summary>
		/// <param name="ns">The namespace URI associated with the schema. For XML Schemas, this will typically be the <see langword="targetNamespace" />. </param>
		/// <param name="reader">
		///       <see cref="T:System.Xml.XmlReader" /> containing the schema to add. </param>
		/// <returns>The <see cref="T:System.Xml.Schema.XmlSchema" /> added to the schema collection; <see langword="null" /> if the schema being added is an XDR schema or if there are compilation errors in the schema.</returns>
		/// <exception cref="T:System.Xml.XmlException">The schema is not a valid schema. </exception>
		public XmlSchema Add(string ns, XmlReader reader)
		{
			return this.Add(ns, reader, this.xmlResolver);
		}

		/// <summary>Adds the schema contained in the <see cref="T:System.Xml.XmlReader" /> to the schema collection. The specified <see cref="T:System.Xml.XmlResolver" /> is used to resolve any external resources.</summary>
		/// <param name="ns">The namespace URI associated with the schema. For XML Schemas, this will typically be the <see langword="targetNamespace" />. </param>
		/// <param name="reader">
		///       <see cref="T:System.Xml.XmlReader" /> containing the schema to add. </param>
		/// <param name="resolver">The <see cref="T:System.Xml.XmlResolver" /> used to resolve namespaces referenced in <see langword="include" /> and <see langword="import" /> elements or <see langword="x-schema" /> attribute (XDR schemas). If this is <see langword="null" />, external references are not resolved. </param>
		/// <returns>The <see cref="T:System.Xml.Schema.XmlSchema" /> added to the schema collection; <see langword="null" /> if the schema being added is an XDR schema or if there are compilation errors in the schema.</returns>
		/// <exception cref="T:System.Xml.XmlException">The schema is not a valid schema. </exception>
		public XmlSchema Add(string ns, XmlReader reader, XmlResolver resolver)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			XmlNameTable nt = reader.NameTable;
			SchemaInfo schemaInfo = new SchemaInfo();
			Parser parser = new Parser(SchemaType.None, nt, this.GetSchemaNames(nt), this.validationEventHandler);
			parser.XmlResolver = resolver;
			SchemaType schemaType;
			try
			{
				schemaType = parser.Parse(reader, ns);
			}
			catch (XmlSchemaException e)
			{
				this.SendValidationEvent(e);
				return null;
			}
			if (schemaType == SchemaType.XSD)
			{
				schemaInfo.SchemaType = SchemaType.XSD;
				return this.Add(ns, schemaInfo, parser.XmlSchema, true, resolver);
			}
			SchemaInfo xdrSchema = parser.XdrSchema;
			return this.Add(ns, parser.XdrSchema, null, true, resolver);
		}

		/// <summary>Adds the <see cref="T:System.Xml.Schema.XmlSchema" /> to the collection.</summary>
		/// <param name="schema">The <see langword="XmlSchema" /> to add to the collection. </param>
		/// <returns>The <see langword="XmlSchema" /> object.</returns>
		public XmlSchema Add(XmlSchema schema)
		{
			return this.Add(schema, this.xmlResolver);
		}

		/// <summary>Adds the <see cref="T:System.Xml.Schema.XmlSchema" /> to the collection. The specified <see cref="T:System.Xml.XmlResolver" /> is used to resolve any external references.</summary>
		/// <param name="schema">The <see langword="XmlSchema" /> to add to the collection. </param>
		/// <param name="resolver">The <see cref="T:System.Xml.XmlResolver" /> used to resolve namespaces referenced in <see langword="include" /> and <see langword="import" /> elements. If this is <see langword="null" />, external references are not resolved. </param>
		/// <returns>The <see langword="XmlSchema" /> added to the schema collection.</returns>
		/// <exception cref="T:System.Xml.XmlException">The schema is not a valid schema. </exception>
		public XmlSchema Add(XmlSchema schema, XmlResolver resolver)
		{
			if (schema == null)
			{
				throw new ArgumentNullException("schema");
			}
			SchemaInfo schemaInfo = new SchemaInfo();
			schemaInfo.SchemaType = SchemaType.XSD;
			return this.Add(schema.TargetNamespace, schemaInfo, schema, true, resolver);
		}

		/// <summary>Adds all the namespaces defined in the given collection (including their associated schemas) to this collection.</summary>
		/// <param name="schema">The <see langword="XmlSchemaCollection" /> you want to add to this collection. </param>
		public void Add(XmlSchemaCollection schema)
		{
			if (schema == null)
			{
				throw new ArgumentNullException("schema");
			}
			if (this == schema)
			{
				return;
			}
			IDictionaryEnumerator enumerator = schema.collection.GetEnumerator();
			while (enumerator.MoveNext())
			{
				XmlSchemaCollectionNode xmlSchemaCollectionNode = (XmlSchemaCollectionNode)enumerator.Value;
				this.Add(xmlSchemaCollectionNode.NamespaceURI, xmlSchemaCollectionNode);
			}
		}

		/// <summary>Gets the <see cref="T:System.Xml.Schema.XmlSchema" /> associated with the given namespace URI.</summary>
		/// <param name="ns">The namespace URI associated with the schema you want to return. This will typically be the <see langword="targetNamespace" /> of the schema. </param>
		/// <returns>The <see langword="XmlSchema" /> associated with the namespace URI; <see langword="null" /> if there is no loaded schema associated with the given namespace or if the namespace is associated with an XDR schema.</returns>
		public XmlSchema this[string ns]
		{
			get
			{
				XmlSchemaCollectionNode xmlSchemaCollectionNode = (XmlSchemaCollectionNode)this.collection[(ns != null) ? ns : string.Empty];
				if (xmlSchemaCollectionNode == null)
				{
					return null;
				}
				return xmlSchemaCollectionNode.Schema;
			}
		}

		/// <summary>Gets a value indicating whether the <see langword="targetNamespace" /> of the specified <see cref="T:System.Xml.Schema.XmlSchema" /> is in the collection.</summary>
		/// <param name="schema">The <see langword="XmlSchema" /> object. </param>
		/// <returns>
		///     <see langword="true" /> if there is a schema in the collection with the same <see langword="targetNamespace" />; otherwise, <see langword="false" />.</returns>
		public bool Contains(XmlSchema schema)
		{
			if (schema == null)
			{
				throw new ArgumentNullException("schema");
			}
			return this[schema.TargetNamespace] != null;
		}

		/// <summary>Gets a value indicating whether a schema with the specified namespace is in the collection.</summary>
		/// <param name="ns">The namespace URI associated with the schema. For XML Schemas, this will typically be the target namespace. </param>
		/// <returns>
		///     <see langword="true" /> if a schema with the specified namespace is in the collection; otherwise, <see langword="false" />.</returns>
		public bool Contains(string ns)
		{
			return this.collection[(ns != null) ? ns : string.Empty] != null;
		}

		/// <summary>For a description of this member, see <see cref="M:System.Xml.Schema.XmlSchemaCollection.GetEnumerator" />.</summary>
		/// <returns>Returns the <see cref="T:System.Collections.IEnumerator" /> for the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new XmlSchemaCollectionEnumerator(this.collection);
		}

		/// <summary>Provides support for the "for each" style iteration over the collection of schemas.</summary>
		/// <returns>An enumerator for iterating over all schemas in the current collection.</returns>
		public XmlSchemaCollectionEnumerator GetEnumerator()
		{
			return new XmlSchemaCollectionEnumerator(this.collection);
		}

		/// <summary>For a description of this member, see <see cref="M:System.Xml.Schema.XmlSchemaCollection.CopyTo(System.Xml.Schema.XmlSchema[],System.Int32)" />.</summary>
		/// <param name="array">The array to copy the objects to. </param>
		/// <param name="index">The index in <paramref name="array" /> where copying will begin. </param>
		void ICollection.CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			XmlSchemaCollectionEnumerator enumerator = this.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (index == array.Length && array.IsFixedSize)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				array.SetValue(enumerator.Current, index++);
			}
		}

		/// <summary>Copies all the <see langword="XmlSchema" /> objects from this collection into the given array starting at the given index.</summary>
		/// <param name="array">The array to copy the objects to. </param>
		/// <param name="index">The index in <paramref name="array" /> where copying will begin. </param>
		public void CopyTo(XmlSchema[] array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			XmlSchemaCollectionEnumerator enumerator = this.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current != null)
				{
					if (index == array.Length)
					{
						throw new ArgumentOutOfRangeException("index");
					}
					array[index++] = enumerator.Current;
				}
			}
		}

		/// <summary>For a description of this member, see <see cref="P:System.Xml.Schema.XmlSchemaCollection.System#Collections#ICollection#IsSynchronized" />.</summary>
		/// <returns>Returns <see langword="true" /> if the collection is synchronized, otherwise <see langword="false" />.</returns>
		bool ICollection.IsSynchronized
		{
			get
			{
				return true;
			}
		}

		/// <summary>For a description of this member, see <see cref="P:System.Xml.Schema.XmlSchemaCollection.System#Collections#ICollection#SyncRoot" />.</summary>
		/// <returns>Returns a <see cref="P:System.Collections.ICollection.SyncRoot" /> object that can be used to synchronize access to the collection.</returns>
		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		/// <summary>For a description of this member, see <see cref="P:System.Xml.Schema.XmlSchemaCollection.Count" />.</summary>
		/// <returns>Returns the count of the items in the collection.</returns>
		int ICollection.Count
		{
			get
			{
				return this.collection.Count;
			}
		}

		internal SchemaInfo GetSchemaInfo(string ns)
		{
			XmlSchemaCollectionNode xmlSchemaCollectionNode = (XmlSchemaCollectionNode)this.collection[(ns != null) ? ns : string.Empty];
			if (xmlSchemaCollectionNode == null)
			{
				return null;
			}
			return xmlSchemaCollectionNode.SchemaInfo;
		}

		internal SchemaNames GetSchemaNames(XmlNameTable nt)
		{
			if (this.nameTable != nt)
			{
				return new SchemaNames(nt);
			}
			if (this.schemaNames == null)
			{
				this.schemaNames = new SchemaNames(this.nameTable);
			}
			return this.schemaNames;
		}

		internal XmlSchema Add(string ns, SchemaInfo schemaInfo, XmlSchema schema, bool compile)
		{
			return this.Add(ns, schemaInfo, schema, compile, this.xmlResolver);
		}

		private XmlSchema Add(string ns, SchemaInfo schemaInfo, XmlSchema schema, bool compile, XmlResolver resolver)
		{
			int num = 0;
			if (schema != null)
			{
				if (schema.ErrorCount == 0 && compile)
				{
					if (!schema.CompileSchema(this, resolver, schemaInfo, ns, this.validationEventHandler, this.nameTable, true))
					{
						num = 1;
					}
					ns = ((schema.TargetNamespace == null) ? string.Empty : schema.TargetNamespace);
				}
				num += schema.ErrorCount;
			}
			else
			{
				num += schemaInfo.ErrorCount;
				ns = this.NameTable.Add(ns);
			}
			if (num == 0)
			{
				this.Add(ns, new XmlSchemaCollectionNode
				{
					NamespaceURI = ns,
					SchemaInfo = schemaInfo,
					Schema = schema
				});
				return schema;
			}
			return null;
		}

		private void Add(string ns, XmlSchemaCollectionNode node)
		{
			if (this.isThreadSafe)
			{
				this.wLock.AcquireWriterLock(this.timeout);
			}
			try
			{
				if (this.collection[ns] != null)
				{
					this.collection.Remove(ns);
				}
				this.collection.Add(ns, node);
			}
			finally
			{
				if (this.isThreadSafe)
				{
					this.wLock.ReleaseWriterLock();
				}
			}
		}

		private void SendValidationEvent(XmlSchemaException e)
		{
			if (this.validationEventHandler != null)
			{
				this.validationEventHandler(this, new ValidationEventArgs(e));
				return;
			}
			throw e;
		}

		internal ValidationEventHandler EventHandler
		{
			get
			{
				return this.validationEventHandler;
			}
			set
			{
				this.validationEventHandler = value;
			}
		}

		private Hashtable collection;

		private XmlNameTable nameTable;

		private SchemaNames schemaNames;

		private ReaderWriterLock wLock;

		private int timeout = -1;

		private bool isThreadSafe = true;

		private ValidationEventHandler validationEventHandler;

		private XmlResolver xmlResolver;
	}
}
