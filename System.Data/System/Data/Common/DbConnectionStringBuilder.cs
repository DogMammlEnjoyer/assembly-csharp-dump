using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Threading;

namespace System.Data.Common
{
	/// <summary>Provides a base class for strongly typed connection string builders.</summary>
	public class DbConnectionStringBuilder : IDictionary, ICollection, IEnumerable, ICustomTypeDescriptor
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Data.Common.DbConnectionStringBuilder" /> class.</summary>
		public DbConnectionStringBuilder()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.Common.DbConnectionStringBuilder" /> class, optionally using ODBC rules for quoting values.</summary>
		/// <param name="useOdbcRules">
		///   <see langword="true" /> to use {} to delimit fields; <see langword="false" /> to use quotation marks.</param>
		public DbConnectionStringBuilder(bool useOdbcRules)
		{
			this._useOdbcRules = useOdbcRules;
		}

		private ICollection Collection
		{
			get
			{
				return this.CurrentValues;
			}
		}

		private IDictionary Dictionary
		{
			get
			{
				return this.CurrentValues;
			}
		}

		private Dictionary<string, object> CurrentValues
		{
			get
			{
				Dictionary<string, object> dictionary = this._currentValues;
				if (dictionary == null)
				{
					dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
					this._currentValues = dictionary;
				}
				return dictionary;
			}
		}

		/// <summary>Gets or sets the element with the specified key.</summary>
		/// <param name="keyword">The key of the element to get or set.</param>
		/// <returns>The element with the specified key.</returns>
		object IDictionary.this[object keyword]
		{
			get
			{
				return this[this.ObjectToString(keyword)];
			}
			set
			{
				this[this.ObjectToString(keyword)] = value;
			}
		}

		/// <summary>Gets or sets the value associated with the specified key.</summary>
		/// <param name="keyword">The key of the item to get or set.</param>
		/// <returns>The value associated with the specified key. If the specified key is not found, trying to get it returns a null reference (<see langword="Nothing" /> in Visual Basic), and trying to set it creates a new element using the specified key.  
		///  Passing a null (<see langword="Nothing" /> in Visual Basic) key throws an <see cref="T:System.ArgumentNullException" />. Assigning a null value removes the key/value pair.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="keyword" /> is a null reference (<see langword="Nothing" /> in Visual Basic).</exception>
		/// <exception cref="T:System.NotSupportedException">The property is set, and the <see cref="T:System.Data.Common.DbConnectionStringBuilder" /> is read-only.  
		///  -or-  
		///  The property is set, <paramref name="keyword" /> does not exist in the collection, and the <see cref="T:System.Data.Common.DbConnectionStringBuilder" /> has a fixed size.</exception>
		[Browsable(false)]
		public virtual object this[string keyword]
		{
			get
			{
				DataCommonEventSource.Log.Trace<int, string>("<comm.DbConnectionStringBuilder.get_Item|API> {0}, keyword='{1}'", this.ObjectID, keyword);
				ADP.CheckArgumentNull(keyword, "keyword");
				object result;
				if (this.CurrentValues.TryGetValue(keyword, out result))
				{
					return result;
				}
				throw ADP.KeywordNotSupported(keyword);
			}
			set
			{
				ADP.CheckArgumentNull(keyword, "keyword");
				bool flag;
				if (value != null)
				{
					string value2 = DbConnectionStringBuilderUtil.ConvertToString(value);
					DbConnectionOptions.ValidateKeyValuePair(keyword, value2);
					flag = this.CurrentValues.ContainsKey(keyword);
					this.CurrentValues[keyword] = value2;
				}
				else
				{
					flag = this.Remove(keyword);
				}
				this._connectionString = null;
				if (flag)
				{
					this._propertyDescriptors = null;
				}
			}
		}

		/// <summary>Gets or sets a value that indicates whether the <see cref="P:System.Data.Common.DbConnectionStringBuilder.ConnectionString" /> property is visible in Visual Studio designers.</summary>
		/// <returns>
		///   <see langword="true" /> if the connection string is visible within designers; <see langword="false" /> otherwise. The default is <see langword="true" />.</returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DesignOnly(true)]
		[Browsable(false)]
		public bool BrowsableConnectionString
		{
			get
			{
				return this._browsableConnectionString;
			}
			set
			{
				this._browsableConnectionString = value;
				this._propertyDescriptors = null;
			}
		}

		/// <summary>Gets or sets the connection string associated with the <see cref="T:System.Data.Common.DbConnectionStringBuilder" />.</summary>
		/// <returns>The current connection string, created from the key/value pairs that are contained within the <see cref="T:System.Data.Common.DbConnectionStringBuilder" />. The default value is an empty string.</returns>
		/// <exception cref="T:System.ArgumentException">An invalid connection string argument has been supplied.</exception>
		[RefreshProperties(RefreshProperties.All)]
		public string ConnectionString
		{
			get
			{
				DataCommonEventSource.Log.Trace<int>("<comm.DbConnectionStringBuilder.get_ConnectionString|API> {0}", this.ObjectID);
				string text = this._connectionString;
				if (text == null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					foreach (object obj in this.Keys)
					{
						string keyword = (string)obj;
						object value;
						if (this.ShouldSerialize(keyword) && this.TryGetValue(keyword, out value))
						{
							string value2 = this.ConvertValueToString(value);
							DbConnectionStringBuilder.AppendKeyValuePair(stringBuilder, keyword, value2, this._useOdbcRules);
						}
					}
					text = stringBuilder.ToString();
					this._connectionString = text;
				}
				return text;
			}
			set
			{
				DataCommonEventSource.Log.Trace<int>("<comm.DbConnectionStringBuilder.set_ConnectionString|API> {0}", this.ObjectID);
				DbConnectionOptions dbConnectionOptions = new DbConnectionOptions(value, null, this._useOdbcRules);
				string connectionString = this.ConnectionString;
				this.Clear();
				try
				{
					for (NameValuePair nameValuePair = dbConnectionOptions._keyChain; nameValuePair != null; nameValuePair = nameValuePair.Next)
					{
						if (nameValuePair.Value != null)
						{
							this[nameValuePair.Name] = nameValuePair.Value;
						}
						else
						{
							this.Remove(nameValuePair.Name);
						}
					}
					this._connectionString = null;
				}
				catch (ArgumentException)
				{
					this.ConnectionString = connectionString;
					this._connectionString = connectionString;
					throw;
				}
			}
		}

		/// <summary>Gets the current number of keys that are contained within the <see cref="P:System.Data.Common.DbConnectionStringBuilder.ConnectionString" /> property.</summary>
		/// <returns>The number of keys that are contained within the connection string maintained by the <see cref="T:System.Data.Common.DbConnectionStringBuilder" /> instance.</returns>
		[Browsable(false)]
		public virtual int Count
		{
			get
			{
				return this.CurrentValues.Count;
			}
		}

		/// <summary>Gets a value that indicates whether the <see cref="T:System.Data.Common.DbConnectionStringBuilder" /> is read-only.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Data.Common.DbConnectionStringBuilder" /> is read-only; otherwise <see langword="false" />. The default is <see langword="false" />.</returns>
		[Browsable(false)]
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets a value that indicates whether the <see cref="T:System.Data.Common.DbConnectionStringBuilder" /> has a fixed size.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Data.Common.DbConnectionStringBuilder" /> has a fixed size; otherwise <see langword="false" />.</returns>
		[Browsable(false)]
		public virtual bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).</summary>
		/// <returns>
		///   <see langword="true" /> if access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe); otherwise, <see langword="false" />.</returns>
		bool ICollection.IsSynchronized
		{
			get
			{
				return this.Collection.IsSynchronized;
			}
		}

		/// <summary>Gets an <see cref="T:System.Collections.ICollection" /> that contains the keys in the <see cref="T:System.Data.Common.DbConnectionStringBuilder" />.</summary>
		/// <returns>An <see cref="T:System.Collections.ICollection" /> that contains the keys in the <see cref="T:System.Data.Common.DbConnectionStringBuilder" />.</returns>
		[Browsable(false)]
		public virtual ICollection Keys
		{
			get
			{
				DataCommonEventSource.Log.Trace<int>("<comm.DbConnectionStringBuilder.Keys|API> {0}", this.ObjectID);
				return this.Dictionary.Keys;
			}
		}

		internal int ObjectID
		{
			get
			{
				return this._objectID;
			}
		}

		/// <summary>Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.</summary>
		/// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.</returns>
		object ICollection.SyncRoot
		{
			get
			{
				return this.Collection.SyncRoot;
			}
		}

		/// <summary>Gets an <see cref="T:System.Collections.ICollection" /> that contains the values in the <see cref="T:System.Data.Common.DbConnectionStringBuilder" />.</summary>
		/// <returns>An <see cref="T:System.Collections.ICollection" /> that contains the values in the <see cref="T:System.Data.Common.DbConnectionStringBuilder" />.</returns>
		[Browsable(false)]
		public virtual ICollection Values
		{
			get
			{
				DataCommonEventSource.Log.Trace<int>("<comm.DbConnectionStringBuilder.Values|API> {0}", this.ObjectID);
				ICollection<string> collection = (ICollection<string>)this.Keys;
				IEnumerator<string> enumerator = collection.GetEnumerator();
				object[] array = new object[collection.Count];
				for (int i = 0; i < array.Length; i++)
				{
					enumerator.MoveNext();
					array[i] = this[enumerator.Current];
				}
				return new ReadOnlyCollection<object>(array);
			}
		}

		internal virtual string ConvertValueToString(object value)
		{
			if (value != null)
			{
				return Convert.ToString(value, CultureInfo.InvariantCulture);
			}
			return null;
		}

		/// <summary>Adds an element with the provided key and value to the <see cref="T:System.Collections.IDictionary" /> object.</summary>
		/// <param name="keyword">The <see cref="T:System.Object" /> to use as the key of the element to add.</param>
		/// <param name="value">The <see cref="T:System.Object" /> to use as the value of the element to add.</param>
		void IDictionary.Add(object keyword, object value)
		{
			this.Add(this.ObjectToString(keyword), value);
		}

		/// <summary>Adds an entry with the specified key and value into the <see cref="T:System.Data.Common.DbConnectionStringBuilder" />.</summary>
		/// <param name="keyword">The key to add to the <see cref="T:System.Data.Common.DbConnectionStringBuilder" />.</param>
		/// <param name="value">The value for the specified key.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="keyword" /> is a null reference (<see langword="Nothing" /> in Visual Basic).</exception>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Data.Common.DbConnectionStringBuilder" /> is read-only.  
		///  -or-  
		///  The <see cref="T:System.Data.Common.DbConnectionStringBuilder" /> has a fixed size.</exception>
		public void Add(string keyword, object value)
		{
			this[keyword] = value;
		}

		/// <summary>Provides an efficient and safe way to append a key and value to an existing <see cref="T:System.Text.StringBuilder" /> object.</summary>
		/// <param name="builder">The <see cref="T:System.Text.StringBuilder" /> to which to add the key/value pair.</param>
		/// <param name="keyword">The key to be added.</param>
		/// <param name="value">The value for the supplied key.</param>
		public static void AppendKeyValuePair(StringBuilder builder, string keyword, string value)
		{
			DbConnectionOptions.AppendKeyValuePairBuilder(builder, keyword, value, false);
		}

		/// <summary>Provides an efficient and safe way to append a key and value to an existing <see cref="T:System.Text.StringBuilder" /> object.</summary>
		/// <param name="builder">The <see cref="T:System.Text.StringBuilder" /> to which to add the key/value pair.</param>
		/// <param name="keyword">The key to be added.</param>
		/// <param name="value">The value for the supplied key.</param>
		/// <param name="useOdbcRules">
		///   <see langword="true" /> to use {} to delimit fields, <see langword="false" /> to use quotation marks.</param>
		public static void AppendKeyValuePair(StringBuilder builder, string keyword, string value, bool useOdbcRules)
		{
			DbConnectionOptions.AppendKeyValuePairBuilder(builder, keyword, value, useOdbcRules);
		}

		/// <summary>Clears the contents of the <see cref="T:System.Data.Common.DbConnectionStringBuilder" /> instance.</summary>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Data.Common.DbConnectionStringBuilder" /> is read-only.</exception>
		public virtual void Clear()
		{
			DataCommonEventSource.Log.Trace("<comm.DbConnectionStringBuilder.Clear|API>");
			this._connectionString = string.Empty;
			this._propertyDescriptors = null;
			this.CurrentValues.Clear();
		}

		/// <summary>Clears the collection of <see cref="T:System.ComponentModel.PropertyDescriptor" /> objects on the associated <see cref="T:System.Data.Common.DbConnectionStringBuilder" />.</summary>
		protected internal void ClearPropertyDescriptors()
		{
			this._propertyDescriptors = null;
		}

		/// <summary>Determines whether the <see cref="T:System.Collections.IDictionary" /> object contains an element with the specified key.</summary>
		/// <param name="keyword">The key to locate in the <see cref="T:System.Collections.IDictionary" /> object.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Collections.IDictionary" /> contains an element with the key; otherwise, <see langword="false" />.</returns>
		bool IDictionary.Contains(object keyword)
		{
			return this.ContainsKey(this.ObjectToString(keyword));
		}

		/// <summary>Determines whether the <see cref="T:System.Data.Common.DbConnectionStringBuilder" /> contains a specific key.</summary>
		/// <param name="keyword">The key to locate in the <see cref="T:System.Data.Common.DbConnectionStringBuilder" />.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Data.Common.DbConnectionStringBuilder" /> contains an entry with the specified key; otherwise <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="keyword" /> is a null reference (<see langword="Nothing" /> in Visual Basic).</exception>
		public virtual bool ContainsKey(string keyword)
		{
			ADP.CheckArgumentNull(keyword, "keyword");
			return this.CurrentValues.ContainsKey(keyword);
		}

		/// <summary>Copies the elements of the <see cref="T:System.Collections.ICollection" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.</summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		void ICollection.CopyTo(Array array, int index)
		{
			DataCommonEventSource.Log.Trace<int>("<comm.DbConnectionStringBuilder.ICollection.CopyTo|API> {0}", this.ObjectID);
			this.Collection.CopyTo(array, index);
		}

		/// <summary>Compares the connection information in this <see cref="T:System.Data.Common.DbConnectionStringBuilder" /> object with the connection information in the supplied object.</summary>
		/// <param name="connectionStringBuilder">The <see cref="T:System.Data.Common.DbConnectionStringBuilder" /> to be compared with this <see cref="T:System.Data.Common.DbConnectionStringBuilder" /> object.</param>
		/// <returns>
		///   <see langword="true" /> if the connection information in both of the <see cref="T:System.Data.Common.DbConnectionStringBuilder" /> objects causes an equivalent connection string; otherwise <see langword="false" />.</returns>
		public virtual bool EquivalentTo(DbConnectionStringBuilder connectionStringBuilder)
		{
			ADP.CheckArgumentNull(connectionStringBuilder, "connectionStringBuilder");
			DataCommonEventSource.Log.Trace<int, int>("<comm.DbConnectionStringBuilder.EquivalentTo|API> {0}, connectionStringBuilder={1}", this.ObjectID, connectionStringBuilder.ObjectID);
			if (base.GetType() != connectionStringBuilder.GetType() || this.CurrentValues.Count != connectionStringBuilder.CurrentValues.Count)
			{
				return false;
			}
			foreach (KeyValuePair<string, object> keyValuePair in this.CurrentValues)
			{
				object obj;
				if (!connectionStringBuilder.CurrentValues.TryGetValue(keyValuePair.Key, out obj) || !keyValuePair.Value.Equals(obj))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>Returns an enumerator that iterates through a collection.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			DataCommonEventSource.Log.Trace<int>("<comm.DbConnectionStringBuilder.IEnumerable.GetEnumerator|API> {0}", this.ObjectID);
			return this.Collection.GetEnumerator();
		}

		/// <summary>Returns an <see cref="T:System.Collections.IDictionaryEnumerator" /> object for the <see cref="T:System.Collections.IDictionary" /> object.</summary>
		/// <returns>An <see cref="T:System.Collections.IDictionaryEnumerator" /> object for the <see cref="T:System.Collections.IDictionary" /> object.</returns>
		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			DataCommonEventSource.Log.Trace<int>("<comm.DbConnectionStringBuilder.IDictionary.GetEnumerator|API> {0}", this.ObjectID);
			return this.Dictionary.GetEnumerator();
		}

		private string ObjectToString(object keyword)
		{
			string result;
			try
			{
				result = (string)keyword;
			}
			catch (InvalidCastException)
			{
				throw new ArgumentException("not a string", "keyword");
			}
			return result;
		}

		/// <summary>Removes the element with the specified key from the <see cref="T:System.Collections.IDictionary" /> object.</summary>
		/// <param name="keyword">The key of the element to remove.</param>
		void IDictionary.Remove(object keyword)
		{
			this.Remove(this.ObjectToString(keyword));
		}

		/// <summary>Removes the entry with the specified key from the <see cref="T:System.Data.Common.DbConnectionStringBuilder" /> instance.</summary>
		/// <param name="keyword">The key of the key/value pair to be removed from the connection string in this <see cref="T:System.Data.Common.DbConnectionStringBuilder" />.</param>
		/// <returns>
		///   <see langword="true" /> if the key existed within the connection string and was removed; <see langword="false" /> if the key did not exist.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="keyword" /> is null (<see langword="Nothing" /> in Visual Basic)</exception>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Data.Common.DbConnectionStringBuilder" /> is read-only, or the <see cref="T:System.Data.Common.DbConnectionStringBuilder" /> has a fixed size.</exception>
		public virtual bool Remove(string keyword)
		{
			DataCommonEventSource.Log.Trace<int, string>("<comm.DbConnectionStringBuilder.Remove|API> {0}, keyword='{1}'", this.ObjectID, keyword);
			ADP.CheckArgumentNull(keyword, "keyword");
			if (this.CurrentValues.Remove(keyword))
			{
				this._connectionString = null;
				this._propertyDescriptors = null;
				return true;
			}
			return false;
		}

		/// <summary>Indicates whether the specified key exists in this <see cref="T:System.Data.Common.DbConnectionStringBuilder" /> instance.</summary>
		/// <param name="keyword">The key to locate in the <see cref="T:System.Data.Common.DbConnectionStringBuilder" />.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Data.Common.DbConnectionStringBuilder" /> contains an entry with the specified key; otherwise <see langword="false" />.</returns>
		public virtual bool ShouldSerialize(string keyword)
		{
			ADP.CheckArgumentNull(keyword, "keyword");
			return this.CurrentValues.ContainsKey(keyword);
		}

		/// <summary>Returns the connection string associated with this <see cref="T:System.Data.Common.DbConnectionStringBuilder" />.</summary>
		/// <returns>The current <see cref="P:System.Data.Common.DbConnectionStringBuilder.ConnectionString" /> property.</returns>
		public override string ToString()
		{
			return this.ConnectionString;
		}

		/// <summary>Retrieves a value corresponding to the supplied key from this <see cref="T:System.Data.Common.DbConnectionStringBuilder" />.</summary>
		/// <param name="keyword">The key of the item to retrieve.</param>
		/// <param name="value">The value corresponding to the <paramref name="keyword" />.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="keyword" /> was found within the connection string, <see langword="false" /> otherwise.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="keyword" /> contains a null value (<see langword="Nothing" /> in Visual Basic).</exception>
		public virtual bool TryGetValue(string keyword, out object value)
		{
			ADP.CheckArgumentNull(keyword, "keyword");
			return this.CurrentValues.TryGetValue(keyword, out value);
		}

		internal Attribute[] GetAttributesFromCollection(AttributeCollection collection)
		{
			Attribute[] array = new Attribute[collection.Count];
			collection.CopyTo(array, 0);
			return array;
		}

		private PropertyDescriptorCollection GetProperties()
		{
			PropertyDescriptorCollection propertyDescriptorCollection = this._propertyDescriptors;
			if (propertyDescriptorCollection == null)
			{
				long scopeId = DataCommonEventSource.Log.EnterScope<int>("<comm.DbConnectionStringBuilder.GetProperties|INFO> {0}", this.ObjectID);
				try
				{
					Hashtable hashtable = new Hashtable(StringComparer.OrdinalIgnoreCase);
					this.GetProperties(hashtable);
					PropertyDescriptor[] array = new PropertyDescriptor[hashtable.Count];
					hashtable.Values.CopyTo(array, 0);
					propertyDescriptorCollection = new PropertyDescriptorCollection(array);
					this._propertyDescriptors = propertyDescriptorCollection;
				}
				finally
				{
					DataCommonEventSource.Log.ExitScope(scopeId);
				}
			}
			return propertyDescriptorCollection;
		}

		/// <summary>Fills a supplied <see cref="T:System.Collections.Hashtable" /> with information about all the properties of this <see cref="T:System.Data.Common.DbConnectionStringBuilder" />.</summary>
		/// <param name="propertyDescriptors">The <see cref="T:System.Collections.Hashtable" /> to be filled with information about this <see cref="T:System.Data.Common.DbConnectionStringBuilder" />.</param>
		protected virtual void GetProperties(Hashtable propertyDescriptors)
		{
			long scopeId = DataCommonEventSource.Log.EnterScope<int>("<comm.DbConnectionStringBuilder.GetProperties|API> {0}", this.ObjectID);
			try
			{
				foreach (object obj in TypeDescriptor.GetProperties(this, true))
				{
					PropertyDescriptor propertyDescriptor = (PropertyDescriptor)obj;
					if ("ConnectionString" != propertyDescriptor.Name)
					{
						string displayName = propertyDescriptor.DisplayName;
						if (!propertyDescriptors.ContainsKey(displayName))
						{
							Attribute[] array = this.GetAttributesFromCollection(propertyDescriptor.Attributes);
							PropertyDescriptor value = new DbConnectionStringBuilderDescriptor(propertyDescriptor.Name, propertyDescriptor.ComponentType, propertyDescriptor.PropertyType, propertyDescriptor.IsReadOnly, array);
							propertyDescriptors[displayName] = value;
						}
					}
					else if (this.BrowsableConnectionString)
					{
						propertyDescriptors["ConnectionString"] = propertyDescriptor;
					}
					else
					{
						propertyDescriptors.Remove("ConnectionString");
					}
				}
				if (!this.IsFixedSize)
				{
					Attribute[] array = null;
					foreach (object obj2 in this.Keys)
					{
						string text = (string)obj2;
						if (!propertyDescriptors.ContainsKey(text))
						{
							object obj3 = this[text];
							Type type;
							if (obj3 != null)
							{
								type = obj3.GetType();
								if (typeof(string) == type)
								{
									int num;
									bool flag;
									if (int.TryParse((string)obj3, out num))
									{
										type = typeof(int);
									}
									else if (bool.TryParse((string)obj3, out flag))
									{
										type = typeof(bool);
									}
								}
							}
							else
							{
								type = typeof(string);
							}
							Attribute[] attributes = array;
							if (StringComparer.OrdinalIgnoreCase.Equals("Password", text) || StringComparer.OrdinalIgnoreCase.Equals("pwd", text))
							{
								attributes = new Attribute[]
								{
									BrowsableAttribute.Yes,
									PasswordPropertyTextAttribute.Yes,
									RefreshPropertiesAttribute.All
								};
							}
							else if (array == null)
							{
								array = new Attribute[]
								{
									BrowsableAttribute.Yes,
									RefreshPropertiesAttribute.All
								};
								attributes = array;
							}
							PropertyDescriptor value2 = new DbConnectionStringBuilderDescriptor(text, base.GetType(), type, false, attributes);
							propertyDescriptors[text] = value2;
						}
					}
				}
			}
			finally
			{
				DataCommonEventSource.Log.ExitScope(scopeId);
			}
		}

		private PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = this.GetProperties();
			if (attributes == null || attributes.Length == 0)
			{
				return properties;
			}
			PropertyDescriptor[] array = new PropertyDescriptor[properties.Count];
			int num = 0;
			foreach (object obj in properties)
			{
				PropertyDescriptor propertyDescriptor = (PropertyDescriptor)obj;
				bool flag = true;
				foreach (Attribute attribute in attributes)
				{
					Attribute attribute2 = propertyDescriptor.Attributes[attribute.GetType()];
					if ((attribute2 == null && !attribute.IsDefaultAttribute()) || !attribute2.Match(attribute))
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					array[num] = propertyDescriptor;
					num++;
				}
			}
			PropertyDescriptor[] array2 = new PropertyDescriptor[num];
			Array.Copy(array, array2, num);
			return new PropertyDescriptorCollection(array2);
		}

		/// <summary>Returns the class name of this instance of a component.</summary>
		/// <returns>The class name of the object, or <see langword="null" /> if the class does not have a name.</returns>
		string ICustomTypeDescriptor.GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		/// <summary>Returns the name of this instance of a component.</summary>
		/// <returns>The name of the object, or <see langword="null" /> if the object does not have a name.</returns>
		string ICustomTypeDescriptor.GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		/// <summary>Returns a collection of custom attributes for this instance of a component.</summary>
		/// <returns>An <see cref="T:System.ComponentModel.AttributeCollection" /> containing the attributes for this object.</returns>
		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		/// <summary>Returns an editor of the specified type for this instance of a component.</summary>
		/// <param name="editorBaseType">A <see cref="T:System.Type" /> that represents the editor for this object.</param>
		/// <returns>An <see cref="T:System.Object" /> of the specified type that is the editor for this object, or <see langword="null" /> if the editor cannot be found.</returns>
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		/// <summary>Returns a type converter for this instance of a component.</summary>
		/// <returns>A <see cref="T:System.ComponentModel.TypeConverter" /> that is the converter for this object, or <see langword="null" /> if there is no <see cref="T:System.ComponentModel.TypeConverter" /> for this object.</returns>
		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		/// <summary>Returns the default property for this instance of a component.</summary>
		/// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptor" /> that represents the default property for this object, or <see langword="null" /> if this object does not have properties.</returns>
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		/// <summary>Returns the properties for this instance of a component.</summary>
		/// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptorCollection" /> that represents the properties for this component instance.</returns>
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			return this.GetProperties();
		}

		/// <summary>Returns the properties for this instance of a component using the attribute array as a filter.</summary>
		/// <param name="attributes">An array of type <see cref="T:System.Attribute" /> that is used as a filter.</param>
		/// <returns>A <see cref="T:System.ComponentModel.PropertyDescriptorCollection" /> that represents the filtered properties for this component instance.</returns>
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			return this.GetProperties(attributes);
		}

		/// <summary>Returns the default event for this instance of a component.</summary>
		/// <returns>An <see cref="T:System.ComponentModel.EventDescriptor" /> that represents the default event for this object, or <see langword="null" /> if this object does not have events.</returns>
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		/// <summary>Returns the events for this instance of a component.</summary>
		/// <returns>An <see cref="T:System.ComponentModel.EventDescriptorCollection" /> that represents the events for this component instance.</returns>
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		/// <summary>Returns the events for this instance of a component using the specified attribute array as a filter.</summary>
		/// <param name="attributes">An array of type <see cref="T:System.Attribute" /> that is used as a filter.</param>
		/// <returns>An <see cref="T:System.ComponentModel.EventDescriptorCollection" /> that represents the filtered events for this component instance.</returns>
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		/// <summary>Returns an object that contains the property described by the specified property descriptor.</summary>
		/// <param name="pd">A <see cref="T:System.ComponentModel.PropertyDescriptor" /> that represents the property whose owner is to be found.</param>
		/// <returns>An <see cref="T:System.Object" /> that represents the owner of the specified property.</returns>
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		private Dictionary<string, object> _currentValues;

		private string _connectionString = string.Empty;

		private PropertyDescriptorCollection _propertyDescriptors;

		private bool _browsableConnectionString = true;

		private readonly bool _useOdbcRules;

		private static int s_objectTypeCount;

		internal readonly int _objectID = Interlocked.Increment(ref DbConnectionStringBuilder.s_objectTypeCount);
	}
}
