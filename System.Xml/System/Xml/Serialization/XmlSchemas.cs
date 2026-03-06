using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Schema;

namespace System.Xml.Serialization
{
	/// <summary>Represents the collection of XML schemas.</summary>
	public class XmlSchemas : CollectionBase, IEnumerable<XmlSchema>, IEnumerable
	{
		/// <summary>Gets or sets the <see cref="T:System.Xml.Schema.XmlSchema" /> object at the specified index. </summary>
		/// <param name="index">The index of the item to retrieve.</param>
		/// <returns>The specified <see cref="T:System.Xml.Schema.XmlSchema" />.</returns>
		public XmlSchema this[int index]
		{
			get
			{
				return (XmlSchema)base.List[index];
			}
			set
			{
				base.List[index] = value;
			}
		}

		/// <summary>Gets a specified <see cref="T:System.Xml.Schema.XmlSchema" /> object that represents the XML schema associated with the specified namespace.</summary>
		/// <param name="ns">The namespace of the specified object.</param>
		/// <returns>The specified <see cref="T:System.Xml.Schema.XmlSchema" /> object.</returns>
		public XmlSchema this[string ns]
		{
			get
			{
				IList list = (IList)this.SchemaSet.Schemas(ns);
				if (list.Count == 0)
				{
					return null;
				}
				if (list.Count == 1)
				{
					return (XmlSchema)list[0];
				}
				throw new InvalidOperationException(Res.GetString("There are more then one schema with targetNamespace='{0}'.", new object[]
				{
					ns
				}));
			}
		}

		/// <summary>Gets a collection of schemas that belong to the same namespace.</summary>
		/// <param name="ns">The namespace of the schemas to retrieve.</param>
		/// <returns>An <see cref="T:System.Collections.IList" /> implementation that contains the schemas.</returns>
		public IList GetSchemas(string ns)
		{
			return (IList)this.SchemaSet.Schemas(ns);
		}

		internal SchemaObjectCache Cache
		{
			get
			{
				if (this.cache == null)
				{
					this.cache = new SchemaObjectCache();
				}
				return this.cache;
			}
		}

		internal Hashtable MergedSchemas
		{
			get
			{
				if (this.mergedSchemas == null)
				{
					this.mergedSchemas = new Hashtable();
				}
				return this.mergedSchemas;
			}
		}

		internal Hashtable References
		{
			get
			{
				if (this.references == null)
				{
					this.references = new Hashtable();
				}
				return this.references;
			}
		}

		internal XmlSchemaSet SchemaSet
		{
			get
			{
				if (this.schemaSet == null)
				{
					this.schemaSet = new XmlSchemaSet();
					this.schemaSet.XmlResolver = null;
					this.schemaSet.ValidationEventHandler += XmlSchemas.IgnoreCompileErrors;
				}
				return this.schemaSet;
			}
		}

		internal int Add(XmlSchema schema, bool delay)
		{
			if (delay)
			{
				if (this.delayedSchemas[schema] == null)
				{
					this.delayedSchemas.Add(schema, schema);
				}
				return -1;
			}
			return this.Add(schema);
		}

		/// <summary>Adds an object to the end of the collection.</summary>
		/// <param name="schema">The <see cref="T:System.Xml.Schema.XmlSchema" /> object to be added to the collection of objects. </param>
		/// <returns>The index at which the <see cref="T:System.Xml.Schema.XmlSchema" /> is added.</returns>
		public int Add(XmlSchema schema)
		{
			if (base.List.Contains(schema))
			{
				return base.List.IndexOf(schema);
			}
			return base.List.Add(schema);
		}

		/// <summary>Adds an <see cref="T:System.Xml.Schema.XmlSchema" /> object that represents an assembly reference to the collection.</summary>
		/// <param name="schema">The <see cref="T:System.Xml.Schema.XmlSchema" /> to add.</param>
		/// <param name="baseUri">The <see cref="T:System.Uri" /> of the schema object.</param>
		/// <returns>The index at which the <see cref="T:System.Xml.Schema.XmlSchema" /> is added.</returns>
		public int Add(XmlSchema schema, Uri baseUri)
		{
			if (base.List.Contains(schema))
			{
				return base.List.IndexOf(schema);
			}
			if (baseUri != null)
			{
				schema.BaseUri = baseUri;
			}
			return base.List.Add(schema);
		}

		/// <summary>Adds an instance of the <see cref="T:System.Xml.Serialization.XmlSchemas" /> class to the end of the collection.</summary>
		/// <param name="schemas">The <see cref="T:System.Xml.Serialization.XmlSchemas" /> object to be added to the end of the collection. </param>
		public void Add(XmlSchemas schemas)
		{
			foreach (object obj in schemas)
			{
				XmlSchema schema = (XmlSchema)obj;
				this.Add(schema);
			}
		}

		/// <summary>Adds an <see cref="T:System.Xml.Schema.XmlSchema" /> object that represents an assembly reference to the collection.</summary>
		/// <param name="schema">The <see cref="T:System.Xml.Schema.XmlSchema" /> to add.</param>
		public void AddReference(XmlSchema schema)
		{
			this.References[schema] = schema;
		}

		/// <summary>Inserts a schema into the <see cref="T:System.Xml.Serialization.XmlSchemas" /> at the specified index.</summary>
		/// <param name="index">The zero-based index at which <paramref name="schema" /> should be inserted. </param>
		/// <param name="schema">The <see cref="T:System.Xml.Schema.XmlSchema" /> object to be inserted. </param>
		public void Insert(int index, XmlSchema schema)
		{
			base.List.Insert(index, schema);
		}

		/// <summary>Searches for the specified schema and returns the zero-based index of the first occurrence within the entire <see cref="T:System.Xml.Serialization.XmlSchemas" />.</summary>
		/// <param name="schema">The <see cref="T:System.Xml.Schema.XmlSchema" /> to locate. </param>
		/// <returns>The zero-based index of the first occurrence of the value within the entire <see cref="T:System.Xml.Serialization.XmlSchemas" />, if found; otherwise, -1.</returns>
		public int IndexOf(XmlSchema schema)
		{
			return base.List.IndexOf(schema);
		}

		/// <summary>Determines whether the <see cref="T:System.Xml.Serialization.XmlSchemas" /> contains a specific schema.</summary>
		/// <param name="schema">The <see cref="T:System.Xml.Schema.XmlSchema" /> object to locate. </param>
		/// <returns>
		///     <see langword="true" />, if the collection contains the specified item; otherwise, <see langword="false" />.</returns>
		public bool Contains(XmlSchema schema)
		{
			return base.List.Contains(schema);
		}

		/// <summary>Returns a value that indicates whether the collection contains an <see cref="T:System.Xml.Schema.XmlSchema" /> object that belongs to the specified namespace.</summary>
		/// <param name="targetNamespace">The namespace of the item to check for.</param>
		/// <returns>
		///     <see langword="true" /> if the item is found; otherwise, <see langword="false" />.</returns>
		public bool Contains(string targetNamespace)
		{
			return this.SchemaSet.Contains(targetNamespace);
		}

		/// <summary>Removes the first occurrence of a specific schema from the <see cref="T:System.Xml.Serialization.XmlSchemas" />.</summary>
		/// <param name="schema">The <see cref="T:System.Xml.Schema.XmlSchema" /> to remove. </param>
		public void Remove(XmlSchema schema)
		{
			base.List.Remove(schema);
		}

		/// <summary>Copies the entire <see cref="T:System.Xml.Serialization.XmlSchemas" /> to a compatible one-dimensional <see cref="T:System.Array" />, which starts at the specified index of the target array.</summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the schemas copied from <see cref="T:System.Xml.Serialization.XmlSchemas" />. The <see cref="T:System.Array" /> must have zero-based indexing. </param>
		/// <param name="index">A 32-bit integer that represents the index in the array where copying begins.</param>
		public void CopyTo(XmlSchema[] array, int index)
		{
			base.List.CopyTo(array, index);
		}

		/// <summary>Performs additional custom processes before inserting a new element into the <see cref="T:System.Xml.Serialization.XmlSchemas" /> instance.</summary>
		/// <param name="index">The zero-based index at which to insert <paramref name="value" />. </param>
		/// <param name="value">The new value of the element at <paramref name="index" />. </param>
		protected override void OnInsert(int index, object value)
		{
			this.AddName((XmlSchema)value);
		}

		/// <summary>Performs additional custom processes when removing an element from the <see cref="T:System.Xml.Serialization.XmlSchemas" /> instance.</summary>
		/// <param name="index">The zero-based index at which <paramref name="value" /> can be found. </param>
		/// <param name="value">The value of the element to remove at <paramref name="index" />. </param>
		protected override void OnRemove(int index, object value)
		{
			this.RemoveName((XmlSchema)value);
		}

		/// <summary>Performs additional custom processes when clearing the contents of the <see cref="T:System.Xml.Serialization.XmlSchemas" /> instance.</summary>
		protected override void OnClear()
		{
			this.schemaSet = null;
		}

		/// <summary>Performs additional custom processes before setting a value in the <see cref="T:System.Xml.Serialization.XmlSchemas" /> instance.</summary>
		/// <param name="index">The zero-based index at which <paramref name="oldValue" /> can be found. </param>
		/// <param name="oldValue">The value to replace with <paramref name="newValue" />. </param>
		/// <param name="newValue">The new value of the element at <paramref name="index" />. </param>
		protected override void OnSet(int index, object oldValue, object newValue)
		{
			this.RemoveName((XmlSchema)oldValue);
			this.AddName((XmlSchema)newValue);
		}

		private void AddName(XmlSchema schema)
		{
			if (this.isCompiled)
			{
				throw new InvalidOperationException(Res.GetString("Cannot add schema to compiled schemas collection."));
			}
			if (this.SchemaSet.Contains(schema))
			{
				this.SchemaSet.Reprocess(schema);
				return;
			}
			this.Prepare(schema);
			this.SchemaSet.Add(schema);
		}

		private void Prepare(XmlSchema schema)
		{
			ArrayList arrayList = new ArrayList();
			string targetNamespace = schema.TargetNamespace;
			foreach (XmlSchemaObject xmlSchemaObject in schema.Includes)
			{
				XmlSchemaExternal xmlSchemaExternal = (XmlSchemaExternal)xmlSchemaObject;
				if (xmlSchemaExternal is XmlSchemaImport && targetNamespace == ((XmlSchemaImport)xmlSchemaExternal).Namespace)
				{
					arrayList.Add(xmlSchemaExternal);
				}
			}
			foreach (object obj in arrayList)
			{
				XmlSchemaObject item = (XmlSchemaObject)obj;
				schema.Includes.Remove(item);
			}
		}

		private void RemoveName(XmlSchema schema)
		{
			this.SchemaSet.Remove(schema);
		}

		/// <summary>Locates in one of the XML schemas an <see cref="T:System.Xml.Schema.XmlSchemaObject" /> of the specified name and type. </summary>
		/// <param name="name">An <see cref="T:System.Xml.XmlQualifiedName" /> that specifies a fully qualified name with a namespace used to locate an <see cref="T:System.Xml.Schema.XmlSchema" /> object in the collection.</param>
		/// <param name="type">The <see cref="T:System.Type" /> of the object to find. Possible types include: <see cref="T:System.Xml.Schema.XmlSchemaGroup" />, <see cref="T:System.Xml.Schema.XmlSchemaAttributeGroup" />, <see cref="T:System.Xml.Schema.XmlSchemaElement" />, <see cref="T:System.Xml.Schema.XmlSchemaAttribute" />, and <see cref="T:System.Xml.Schema.XmlSchemaNotation" />.</param>
		/// <returns>An <see cref="T:System.Xml.Schema.XmlSchemaObject" /> instance, such as an <see cref="T:System.Xml.Schema.XmlSchemaElement" /> or <see cref="T:System.Xml.Schema.XmlSchemaAttribute" />.</returns>
		public object Find(XmlQualifiedName name, Type type)
		{
			return this.Find(name, type, true);
		}

		internal object Find(XmlQualifiedName name, Type type, bool checkCache)
		{
			if (!this.IsCompiled)
			{
				foreach (object obj in base.List)
				{
					XmlSchemas.Preprocess((XmlSchema)obj);
				}
			}
			IList list = (IList)this.SchemaSet.Schemas(name.Namespace);
			if (list == null)
			{
				return null;
			}
			foreach (object obj2 in list)
			{
				XmlSchema xmlSchema = (XmlSchema)obj2;
				XmlSchemas.Preprocess(xmlSchema);
				XmlSchemaObject xmlSchemaObject = null;
				if (typeof(XmlSchemaType).IsAssignableFrom(type))
				{
					xmlSchemaObject = xmlSchema.SchemaTypes[name];
					if (xmlSchemaObject == null)
					{
						continue;
					}
					if (!type.IsAssignableFrom(xmlSchemaObject.GetType()))
					{
						continue;
					}
				}
				else if (type == typeof(XmlSchemaGroup))
				{
					xmlSchemaObject = xmlSchema.Groups[name];
				}
				else if (type == typeof(XmlSchemaAttributeGroup))
				{
					xmlSchemaObject = xmlSchema.AttributeGroups[name];
				}
				else if (type == typeof(XmlSchemaElement))
				{
					xmlSchemaObject = xmlSchema.Elements[name];
				}
				else if (type == typeof(XmlSchemaAttribute))
				{
					xmlSchemaObject = xmlSchema.Attributes[name];
				}
				else if (type == typeof(XmlSchemaNotation))
				{
					xmlSchemaObject = xmlSchema.Notations[name];
				}
				if (xmlSchemaObject != null && this.shareTypes && checkCache && !this.IsReference(xmlSchemaObject))
				{
					xmlSchemaObject = this.Cache.AddItem(xmlSchemaObject, name, this);
				}
				if (xmlSchemaObject != null)
				{
					return xmlSchemaObject;
				}
			}
			return null;
		}

		IEnumerator<XmlSchema> IEnumerable<XmlSchema>.GetEnumerator()
		{
			return new XmlSchemaEnumerator(this);
		}

		internal static void Preprocess(XmlSchema schema)
		{
			if (!schema.IsPreprocessed)
			{
				try
				{
					NameTable nameTable = new NameTable();
					new Preprocessor(nameTable, new SchemaNames(nameTable), null)
					{
						SchemaLocations = new Hashtable()
					}.Execute(schema, schema.TargetNamespace, false);
				}
				catch (XmlSchemaException ex)
				{
					throw XmlSchemas.CreateValidationException(ex, ex.Message);
				}
			}
		}

		/// <summary>Static method that determines whether the specified XML schema contains a custom <see langword="IsDataSet" /> attribute set to <see langword="true" />, or its equivalent. </summary>
		/// <param name="schema">The XML schema to check for an <see langword="IsDataSet" /> attribute with a <see langword="true" /> value.</param>
		/// <returns>
		///     <see langword="true" /> if the specified schema exists; otherwise, <see langword="false" />.</returns>
		public static bool IsDataSet(XmlSchema schema)
		{
			foreach (XmlSchemaObject xmlSchemaObject in schema.Items)
			{
				if (xmlSchemaObject is XmlSchemaElement)
				{
					XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)xmlSchemaObject;
					if (xmlSchemaElement.UnhandledAttributes != null)
					{
						foreach (XmlAttribute xmlAttribute in xmlSchemaElement.UnhandledAttributes)
						{
							if (xmlAttribute.LocalName == "IsDataSet" && xmlAttribute.NamespaceURI == "urn:schemas-microsoft-com:xml-msdata" && (xmlAttribute.Value == "True" || xmlAttribute.Value == "true" || xmlAttribute.Value == "1"))
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		private void Merge(XmlSchema schema)
		{
			if (this.MergedSchemas[schema] != null)
			{
				return;
			}
			IList list = (IList)this.SchemaSet.Schemas(schema.TargetNamespace);
			if (list != null && list.Count > 0)
			{
				this.MergedSchemas.Add(schema, schema);
				this.Merge(list, schema);
				return;
			}
			this.Add(schema);
			this.MergedSchemas.Add(schema, schema);
		}

		private void AddImport(IList schemas, string ns)
		{
			foreach (object obj in schemas)
			{
				XmlSchema xmlSchema = (XmlSchema)obj;
				bool flag = true;
				foreach (XmlSchemaObject xmlSchemaObject in xmlSchema.Includes)
				{
					XmlSchemaExternal xmlSchemaExternal = (XmlSchemaExternal)xmlSchemaObject;
					if (xmlSchemaExternal is XmlSchemaImport && ((XmlSchemaImport)xmlSchemaExternal).Namespace == ns)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					XmlSchemaImport xmlSchemaImport = new XmlSchemaImport();
					xmlSchemaImport.Namespace = ns;
					xmlSchema.Includes.Add(xmlSchemaImport);
				}
			}
		}

		private void Merge(IList originals, XmlSchema schema)
		{
			foreach (object obj in originals)
			{
				XmlSchema xmlSchema = (XmlSchema)obj;
				if (schema == xmlSchema)
				{
					return;
				}
			}
			foreach (XmlSchemaObject xmlSchemaObject in schema.Includes)
			{
				XmlSchemaExternal xmlSchemaExternal = (XmlSchemaExternal)xmlSchemaObject;
				if (xmlSchemaExternal is XmlSchemaImport)
				{
					xmlSchemaExternal.SchemaLocation = null;
					if (xmlSchemaExternal.Schema != null)
					{
						this.Merge(xmlSchemaExternal.Schema);
					}
					else
					{
						this.AddImport(originals, ((XmlSchemaImport)xmlSchemaExternal).Namespace);
					}
				}
				else if (xmlSchemaExternal.Schema == null)
				{
					if (xmlSchemaExternal.SchemaLocation != null)
					{
						throw new InvalidOperationException(Res.GetString("Schema attribute schemaLocation='{1}' is not supported on objects of type {0}.  Please set {0}.Schema property.", new object[]
						{
							base.GetType().Name,
							xmlSchemaExternal.SchemaLocation
						}));
					}
				}
				else
				{
					xmlSchemaExternal.SchemaLocation = null;
					this.Merge(originals, xmlSchemaExternal.Schema);
				}
			}
			bool[] array = new bool[schema.Items.Count];
			int num = 0;
			for (int i = 0; i < schema.Items.Count; i++)
			{
				XmlSchemaObject xmlSchemaObject2 = schema.Items[i];
				XmlSchemaObject xmlSchemaObject3 = this.Find(xmlSchemaObject2, originals);
				if (xmlSchemaObject3 != null)
				{
					if (!this.Cache.Match(xmlSchemaObject3, xmlSchemaObject2, this.shareTypes))
					{
						throw new InvalidOperationException(XmlSchemas.MergeFailedMessage(xmlSchemaObject2, xmlSchemaObject3, schema.TargetNamespace));
					}
					array[i] = true;
					num++;
				}
			}
			if (num != schema.Items.Count)
			{
				XmlSchema xmlSchema2 = (XmlSchema)originals[0];
				for (int j = 0; j < schema.Items.Count; j++)
				{
					if (!array[j])
					{
						xmlSchema2.Items.Add(schema.Items[j]);
					}
				}
				xmlSchema2.IsPreprocessed = false;
				XmlSchemas.Preprocess(xmlSchema2);
			}
		}

		private static string ItemName(XmlSchemaObject o)
		{
			if (o is XmlSchemaNotation)
			{
				return ((XmlSchemaNotation)o).Name;
			}
			if (o is XmlSchemaGroup)
			{
				return ((XmlSchemaGroup)o).Name;
			}
			if (o is XmlSchemaElement)
			{
				return ((XmlSchemaElement)o).Name;
			}
			if (o is XmlSchemaType)
			{
				return ((XmlSchemaType)o).Name;
			}
			if (o is XmlSchemaAttributeGroup)
			{
				return ((XmlSchemaAttributeGroup)o).Name;
			}
			if (o is XmlSchemaAttribute)
			{
				return ((XmlSchemaAttribute)o).Name;
			}
			return null;
		}

		internal static XmlQualifiedName GetParentName(XmlSchemaObject item)
		{
			while (item.Parent != null)
			{
				if (item.Parent is XmlSchemaType)
				{
					XmlSchemaType xmlSchemaType = (XmlSchemaType)item.Parent;
					if (xmlSchemaType.Name != null && xmlSchemaType.Name.Length != 0)
					{
						return xmlSchemaType.QualifiedName;
					}
				}
				item = item.Parent;
			}
			return XmlQualifiedName.Empty;
		}

		private static string GetSchemaItem(XmlSchemaObject o, string ns, string details)
		{
			if (o == null)
			{
				return null;
			}
			while (o.Parent != null && !(o.Parent is XmlSchema))
			{
				o = o.Parent;
			}
			if (ns == null || ns.Length == 0)
			{
				XmlSchemaObject xmlSchemaObject = o;
				while (xmlSchemaObject.Parent != null)
				{
					xmlSchemaObject = xmlSchemaObject.Parent;
				}
				if (xmlSchemaObject is XmlSchema)
				{
					ns = ((XmlSchema)xmlSchemaObject).TargetNamespace;
				}
			}
			string @string;
			if (o is XmlSchemaNotation)
			{
				@string = Res.GetString("Schema item '{1}' named '{2}' from namespace '{0}'. {3}", new object[]
				{
					ns,
					"notation",
					((XmlSchemaNotation)o).Name,
					details
				});
			}
			else if (o is XmlSchemaGroup)
			{
				@string = Res.GetString("Schema item '{1}' named '{2}' from namespace '{0}'. {3}", new object[]
				{
					ns,
					"group",
					((XmlSchemaGroup)o).Name,
					details
				});
			}
			else if (o is XmlSchemaElement)
			{
				XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)o;
				if (xmlSchemaElement.Name == null || xmlSchemaElement.Name.Length == 0)
				{
					XmlQualifiedName parentName = XmlSchemas.GetParentName(o);
					@string = Res.GetString("Element reference '{0}' declared in schema type '{1}' from namespace '{2}'.", new object[]
					{
						xmlSchemaElement.RefName.ToString(),
						parentName.Name,
						parentName.Namespace
					});
				}
				else
				{
					@string = Res.GetString("Schema item '{1}' named '{2}' from namespace '{0}'. {3}", new object[]
					{
						ns,
						"element",
						xmlSchemaElement.Name,
						details
					});
				}
			}
			else if (o is XmlSchemaType)
			{
				string name = "Schema item '{1}' named '{2}' from namespace '{0}'. {3}";
				object[] array = new object[4];
				array[0] = ns;
				array[1] = ((o.GetType() == typeof(XmlSchemaSimpleType)) ? "simpleType" : "complexType");
				array[2] = ((XmlSchemaType)o).Name;
				@string = Res.GetString(name, array);
			}
			else if (o is XmlSchemaAttributeGroup)
			{
				@string = Res.GetString("Schema item '{1}' named '{2}' from namespace '{0}'. {3}", new object[]
				{
					ns,
					"attributeGroup",
					((XmlSchemaAttributeGroup)o).Name,
					details
				});
			}
			else if (o is XmlSchemaAttribute)
			{
				XmlSchemaAttribute xmlSchemaAttribute = (XmlSchemaAttribute)o;
				if (xmlSchemaAttribute.Name == null || xmlSchemaAttribute.Name.Length == 0)
				{
					XmlQualifiedName parentName2 = XmlSchemas.GetParentName(o);
					return Res.GetString("Attribute reference '{0}' declared in schema type '{1}' from namespace '{2}'.", new object[]
					{
						xmlSchemaAttribute.RefName.ToString(),
						parentName2.Name,
						parentName2.Namespace
					});
				}
				@string = Res.GetString("Schema item '{1}' named '{2}' from namespace '{0}'. {3}", new object[]
				{
					ns,
					"attribute",
					xmlSchemaAttribute.Name,
					details
				});
			}
			else if (o is XmlSchemaContent)
			{
				XmlQualifiedName parentName3 = XmlSchemas.GetParentName(o);
				string name2 = "Check content definition of schema type '{0}' from namespace '{1}'. {2}";
				object[] array2 = new object[3];
				array2[0] = parentName3.Name;
				array2[1] = parentName3.Namespace;
				@string = Res.GetString(name2, array2);
			}
			else if (o is XmlSchemaExternal)
			{
				string text = (o is XmlSchemaImport) ? "import" : ((o is XmlSchemaInclude) ? "include" : ((o is XmlSchemaRedefine) ? "redefine" : o.GetType().Name));
				@string = Res.GetString("Schema item '{1}' from namespace '{0}'. {2}", new object[]
				{
					ns,
					text,
					details
				});
			}
			else if (o is XmlSchema)
			{
				@string = Res.GetString("Schema with targetNamespace='{0}' has invalid syntax. {1}", new object[]
				{
					ns,
					details
				});
			}
			else
			{
				@string = Res.GetString("Schema item '{1}' named '{2}' from namespace '{0}'. {3}", new object[]
				{
					ns,
					o.GetType().Name,
					null,
					details
				});
			}
			return @string;
		}

		private static string Dump(XmlSchemaObject o)
		{
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.OmitXmlDeclaration = true;
			xmlWriterSettings.Indent = true;
			XmlSerializer xmlSerializer = new XmlSerializer(o.GetType());
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			XmlWriter xmlWriter = XmlWriter.Create(stringWriter, xmlWriterSettings);
			XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
			xmlSerializerNamespaces.Add("xs", "http://www.w3.org/2001/XMLSchema");
			xmlSerializer.Serialize(xmlWriter, o, xmlSerializerNamespaces);
			return stringWriter.ToString();
		}

		private static string MergeFailedMessage(XmlSchemaObject src, XmlSchemaObject dest, string ns)
		{
			return Res.GetString("Cannot merge schemas with targetNamespace='{0}'. Several mismatched declarations were found: {1}", new object[]
			{
				ns,
				XmlSchemas.GetSchemaItem(src, ns, null)
			}) + "\r\n" + XmlSchemas.Dump(src) + "\r\n" + XmlSchemas.Dump(dest);
		}

		internal XmlSchemaObject Find(XmlSchemaObject o, IList originals)
		{
			string text = XmlSchemas.ItemName(o);
			if (text == null)
			{
				return null;
			}
			Type type = o.GetType();
			foreach (object obj in originals)
			{
				foreach (XmlSchemaObject xmlSchemaObject in ((XmlSchema)obj).Items)
				{
					if (xmlSchemaObject.GetType() == type && text == XmlSchemas.ItemName(xmlSchemaObject))
					{
						return xmlSchemaObject;
					}
				}
			}
			return null;
		}

		/// <summary>Gets a value that indicates whether the schemas have been compiled.</summary>
		/// <returns>
		///     <see langword="true" />, if the schemas have been compiled; otherwise, <see langword="false" />.</returns>
		public bool IsCompiled
		{
			get
			{
				return this.isCompiled;
			}
		}

		/// <summary>Processes the element and attribute names in the XML schemas and, optionally, validates the XML schemas. </summary>
		/// <param name="handler">A <see cref="T:System.Xml.Schema.ValidationEventHandler" /> that specifies the callback method that handles errors and warnings during XML Schema validation, if the strict parameter is set to <see langword="true" />.</param>
		/// <param name="fullCompile">
		///       <see langword="true" /> to validate the XML schemas in the collection using the <see cref="M:System.Xml.Serialization.XmlSchemas.Compile(System.Xml.Schema.ValidationEventHandler,System.Boolean)" /> method of the <see cref="T:System.Xml.Serialization.XmlSchemas" /> class; otherwise, <see langword="false" />.</param>
		public void Compile(ValidationEventHandler handler, bool fullCompile)
		{
			if (this.isCompiled)
			{
				return;
			}
			foreach (object obj in this.delayedSchemas.Values)
			{
				XmlSchema schema = (XmlSchema)obj;
				this.Merge(schema);
			}
			this.delayedSchemas.Clear();
			if (fullCompile)
			{
				this.schemaSet = new XmlSchemaSet();
				this.schemaSet.XmlResolver = null;
				this.schemaSet.ValidationEventHandler += handler;
				foreach (object obj2 in this.References.Values)
				{
					XmlSchema schema2 = (XmlSchema)obj2;
					this.schemaSet.Add(schema2);
				}
				int num = this.schemaSet.Count;
				foreach (object obj3 in base.List)
				{
					XmlSchema schema3 = (XmlSchema)obj3;
					if (!this.SchemaSet.Contains(schema3))
					{
						this.schemaSet.Add(schema3);
						num++;
					}
				}
				if (!this.SchemaSet.Contains("http://www.w3.org/2001/XMLSchema"))
				{
					this.AddReference(XmlSchemas.XsdSchema);
					this.schemaSet.Add(XmlSchemas.XsdSchema);
					num++;
				}
				if (!this.SchemaSet.Contains("http://www.w3.org/XML/1998/namespace"))
				{
					this.AddReference(XmlSchemas.XmlSchema);
					this.schemaSet.Add(XmlSchemas.XmlSchema);
					num++;
				}
				this.schemaSet.Compile();
				this.schemaSet.ValidationEventHandler -= handler;
				this.isCompiled = (this.schemaSet.IsCompiled && num == this.schemaSet.Count);
				return;
			}
			try
			{
				NameTable nameTable = new NameTable();
				Preprocessor preprocessor = new Preprocessor(nameTable, new SchemaNames(nameTable), null);
				preprocessor.XmlResolver = null;
				preprocessor.SchemaLocations = new Hashtable();
				preprocessor.ChameleonSchemas = new Hashtable();
				foreach (object obj4 in this.SchemaSet.Schemas())
				{
					XmlSchema xmlSchema = (XmlSchema)obj4;
					preprocessor.Execute(xmlSchema, xmlSchema.TargetNamespace, true);
				}
			}
			catch (XmlSchemaException ex)
			{
				throw XmlSchemas.CreateValidationException(ex, ex.Message);
			}
		}

		internal static Exception CreateValidationException(XmlSchemaException exception, string message)
		{
			XmlSchemaObject xmlSchemaObject = exception.SourceSchemaObject;
			if (exception.LineNumber == 0 && exception.LinePosition == 0)
			{
				throw new InvalidOperationException(XmlSchemas.GetSchemaItem(xmlSchemaObject, null, message), exception);
			}
			string text = null;
			if (xmlSchemaObject != null)
			{
				while (xmlSchemaObject.Parent != null)
				{
					xmlSchemaObject = xmlSchemaObject.Parent;
				}
				if (xmlSchemaObject is XmlSchema)
				{
					text = ((XmlSchema)xmlSchemaObject).TargetNamespace;
				}
			}
			throw new InvalidOperationException(Res.GetString("Schema with targetNamespace='{0}' has invalid syntax. {1} Line {2}, position {3}.", new object[]
			{
				text,
				message,
				exception.LineNumber,
				exception.LinePosition
			}), exception);
		}

		internal static void IgnoreCompileErrors(object sender, ValidationEventArgs args)
		{
		}

		internal static XmlSchema XsdSchema
		{
			get
			{
				if (XmlSchemas.xsd == null)
				{
					XmlSchemas.xsd = XmlSchemas.CreateFakeXsdSchema("http://www.w3.org/2001/XMLSchema", "schema");
				}
				return XmlSchemas.xsd;
			}
		}

		internal static XmlSchema XmlSchema
		{
			get
			{
				if (XmlSchemas.xml == null)
				{
					XmlSchemas.xml = XmlSchema.Read(new StringReader("<?xml version='1.0' encoding='UTF-8' ?> \n<xs:schema targetNamespace='http://www.w3.org/XML/1998/namespace' xmlns:xs='http://www.w3.org/2001/XMLSchema' xml:lang='en'>\n <xs:attribute name='lang' type='xs:language'/>\n <xs:attribute name='space'>\n  <xs:simpleType>\n   <xs:restriction base='xs:NCName'>\n    <xs:enumeration value='default'/>\n    <xs:enumeration value='preserve'/>\n   </xs:restriction>\n  </xs:simpleType>\n </xs:attribute>\n <xs:attribute name='base' type='xs:anyURI'/>\n <xs:attribute name='id' type='xs:ID' />\n <xs:attributeGroup name='specialAttrs'>\n  <xs:attribute ref='xml:base'/>\n  <xs:attribute ref='xml:lang'/>\n  <xs:attribute ref='xml:space'/>\n </xs:attributeGroup>\n</xs:schema>"), null);
				}
				return XmlSchemas.xml;
			}
		}

		private static XmlSchema CreateFakeXsdSchema(string ns, string name)
		{
			XmlSchema xmlSchema = new XmlSchema();
			xmlSchema.TargetNamespace = ns;
			XmlSchemaElement xmlSchemaElement = new XmlSchemaElement();
			xmlSchemaElement.Name = name;
			XmlSchemaComplexType schemaType = new XmlSchemaComplexType();
			xmlSchemaElement.SchemaType = schemaType;
			xmlSchema.Items.Add(xmlSchemaElement);
			return xmlSchema;
		}

		internal void SetCache(SchemaObjectCache cache, bool shareTypes)
		{
			this.shareTypes = shareTypes;
			this.cache = cache;
			if (shareTypes)
			{
				cache.GenerateSchemaGraph(this);
			}
		}

		internal bool IsReference(XmlSchemaObject type)
		{
			XmlSchemaObject xmlSchemaObject = type;
			while (xmlSchemaObject.Parent != null)
			{
				xmlSchemaObject = xmlSchemaObject.Parent;
			}
			return this.References.Contains(xmlSchemaObject);
		}

		private XmlSchemaSet schemaSet;

		private Hashtable references;

		private SchemaObjectCache cache;

		private bool shareTypes;

		private Hashtable mergedSchemas;

		internal Hashtable delayedSchemas = new Hashtable();

		private bool isCompiled;

		private static volatile XmlSchema xsd;

		private static volatile XmlSchema xml;

		internal const string xmlSchema = "<?xml version='1.0' encoding='UTF-8' ?> \n<xs:schema targetNamespace='http://www.w3.org/XML/1998/namespace' xmlns:xs='http://www.w3.org/2001/XMLSchema' xml:lang='en'>\n <xs:attribute name='lang' type='xs:language'/>\n <xs:attribute name='space'>\n  <xs:simpleType>\n   <xs:restriction base='xs:NCName'>\n    <xs:enumeration value='default'/>\n    <xs:enumeration value='preserve'/>\n   </xs:restriction>\n  </xs:simpleType>\n </xs:attribute>\n <xs:attribute name='base' type='xs:anyURI'/>\n <xs:attribute name='id' type='xs:ID' />\n <xs:attributeGroup name='specialAttrs'>\n  <xs:attribute ref='xml:base'/>\n  <xs:attribute ref='xml:lang'/>\n  <xs:attribute ref='xml:space'/>\n </xs:attributeGroup>\n</xs:schema>";
	}
}
