using System;
using System.Collections;
using System.ComponentModel;
using System.Data.ProviderBase;

namespace System.Data.Common
{
	/// <summary>Exposes the <see cref="M:System.Collections.IEnumerable.GetEnumerator" /> method, which supports a simple iteration over a collection by a .NET Framework data provider.</summary>
	public class DbEnumerator : IEnumerator
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Data.Common.DbEnumerator" /> class using the specified <see langword="DataReader" />.</summary>
		/// <param name="reader">The <see langword="DataReader" /> through which to iterate.</param>
		public DbEnumerator(IDataReader reader)
		{
			if (reader == null)
			{
				throw ADP.ArgumentNull("reader");
			}
			this._reader = reader;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.Common.DbEnumerator" /> class using the specified <see langword="DataReader" />, and indicates whether to automatically close the <see langword="DataReader" /> after iterating through its data.</summary>
		/// <param name="reader">The <see langword="DataReader" /> through which to iterate.</param>
		/// <param name="closeReader">
		///   <see langword="true" /> to automatically close the <see langword="DataReader" /> after iterating through its data; otherwise, <see langword="false" />.</param>
		public DbEnumerator(IDataReader reader, bool closeReader)
		{
			if (reader == null)
			{
				throw ADP.ArgumentNull("reader");
			}
			this._reader = reader;
			this._closeReader = closeReader;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.Common.DbEnumerator" /> class with the give n data reader.</summary>
		/// <param name="reader">The DataReader through which to iterate.</param>
		public DbEnumerator(DbDataReader reader) : this(reader)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.Common.DbEnumerator" /> class using the specified reader and indicates whether to automatically close the reader after iterating through its data.</summary>
		/// <param name="reader">The DataReader through which to iterate.</param>
		/// <param name="closeReader">
		///   <see langword="true" /> to automatically close the DataReader after iterating through its data; otherwise, <see langword="false" />.</param>
		public DbEnumerator(DbDataReader reader, bool closeReader) : this(reader, closeReader)
		{
		}

		/// <summary>Gets the current element in the collection.</summary>
		/// <returns>The current element in the collection.</returns>
		/// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element.</exception>
		public object Current
		{
			get
			{
				return this._current;
			}
		}

		/// <summary>Advances the enumerator to the next element of the collection.</summary>
		/// <returns>
		///   <see langword="true" /> if the enumerator was successfully advanced to the next element; <see langword="false" /> if the enumerator has passed the end of the collection.</returns>
		/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created.</exception>
		public bool MoveNext()
		{
			if (this._schemaInfo == null)
			{
				this.BuildSchemaInfo();
			}
			this._current = null;
			if (this._reader.Read())
			{
				object[] values = new object[this._schemaInfo.Length];
				this._reader.GetValues(values);
				this._current = new DataRecordInternal(this._schemaInfo, values, this._descriptors, this._fieldNameLookup);
				return true;
			}
			if (this._closeReader)
			{
				this._reader.Close();
			}
			return false;
		}

		/// <summary>Sets the enumerator to its initial position, which is before the first element in the collection.</summary>
		/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Reset()
		{
			throw ADP.NotSupported();
		}

		private void BuildSchemaInfo()
		{
			int fieldCount = this._reader.FieldCount;
			string[] array = new string[fieldCount];
			for (int i = 0; i < fieldCount; i++)
			{
				array[i] = this._reader.GetName(i);
			}
			ADP.BuildSchemaTableInfoTableNames(array);
			SchemaInfo[] array2 = new SchemaInfo[fieldCount];
			PropertyDescriptor[] array3 = new PropertyDescriptor[this._reader.FieldCount];
			for (int j = 0; j < array2.Length; j++)
			{
				SchemaInfo schemaInfo = default(SchemaInfo);
				schemaInfo.name = this._reader.GetName(j);
				schemaInfo.type = this._reader.GetFieldType(j);
				schemaInfo.typeName = this._reader.GetDataTypeName(j);
				array3[j] = new DbEnumerator.DbColumnDescriptor(j, array[j], schemaInfo.type);
				array2[j] = schemaInfo;
			}
			this._schemaInfo = array2;
			this._fieldNameLookup = new FieldNameLookup(this._reader, -1);
			this._descriptors = new PropertyDescriptorCollection(array3);
		}

		internal IDataReader _reader;

		internal DbDataRecord _current;

		internal SchemaInfo[] _schemaInfo;

		internal PropertyDescriptorCollection _descriptors;

		private FieldNameLookup _fieldNameLookup;

		private bool _closeReader;

		private sealed class DbColumnDescriptor : PropertyDescriptor
		{
			internal DbColumnDescriptor(int ordinal, string name, Type type) : base(name, null)
			{
				this._ordinal = ordinal;
				this._type = type;
			}

			public override Type ComponentType
			{
				get
				{
					return typeof(IDataRecord);
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override Type PropertyType
			{
				get
				{
					return this._type;
				}
			}

			public override bool CanResetValue(object component)
			{
				return false;
			}

			public override object GetValue(object component)
			{
				return ((IDataRecord)component)[this._ordinal];
			}

			public override void ResetValue(object component)
			{
				throw ADP.NotSupported();
			}

			public override void SetValue(object component, object value)
			{
				throw ADP.NotSupported();
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}

			private int _ordinal;

			private Type _type;
		}
	}
}
