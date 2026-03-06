using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data.Odbc
{
	/// <summary>Provides a simple way to create and manage the contents of connection strings used by the <see cref="T:System.Data.Odbc.OdbcConnection" /> class.</summary>
	public sealed class OdbcConnectionStringBuilder : DbConnectionStringBuilder
	{
		static OdbcConnectionStringBuilder()
		{
			string[] array = new string[]
			{
				null,
				"Driver"
			};
			array[0] = "Dsn";
			OdbcConnectionStringBuilder.s_validKeywords = array;
			OdbcConnectionStringBuilder.s_keywords = new Dictionary<string, OdbcConnectionStringBuilder.Keywords>(2, StringComparer.OrdinalIgnoreCase)
			{
				{
					"Driver",
					OdbcConnectionStringBuilder.Keywords.Driver
				},
				{
					"Dsn",
					OdbcConnectionStringBuilder.Keywords.Dsn
				}
			};
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.Odbc.OdbcConnectionStringBuilder" /> class.</summary>
		public OdbcConnectionStringBuilder() : this(null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.Odbc.OdbcConnectionStringBuilder" /> class. The provided connection string provides the data for the instance's internal connection information.</summary>
		/// <param name="connectionString">The basis for the object's internal connection information. Parsed into key/value pairs.</param>
		/// <exception cref="T:System.ArgumentException">The connection string is incorrectly formatted (perhaps missing the required "=" within a key/value pair).</exception>
		public OdbcConnectionStringBuilder(string connectionString) : base(true)
		{
			if (!string.IsNullOrEmpty(connectionString))
			{
				base.ConnectionString = connectionString;
			}
		}

		/// <summary>Gets or sets the value associated with the specified key. In C#, this property is the indexer.</summary>
		/// <param name="keyword">The key of the item to get or set.</param>
		/// <returns>The value associated with the specified key.</returns>
		/// <exception cref="T:System.ArgumentException">The connection string is incorrectly formatted (perhaps missing the required "=" within a key/value pair).</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="keyword" /> is a null reference (<see langword="Nothing" /> in Visual Basic).</exception>
		public override object this[string keyword]
		{
			get
			{
				ADP.CheckArgumentNull(keyword, "keyword");
				OdbcConnectionStringBuilder.Keywords index;
				if (OdbcConnectionStringBuilder.s_keywords.TryGetValue(keyword, out index))
				{
					return this.GetAt(index);
				}
				return base[keyword];
			}
			set
			{
				ADP.CheckArgumentNull(keyword, "keyword");
				if (value == null)
				{
					this.Remove(keyword);
					return;
				}
				OdbcConnectionStringBuilder.Keywords keywords;
				if (!OdbcConnectionStringBuilder.s_keywords.TryGetValue(keyword, out keywords))
				{
					base[keyword] = value;
					base.ClearPropertyDescriptors();
					this._knownKeywords = null;
					return;
				}
				if (keywords == OdbcConnectionStringBuilder.Keywords.Dsn)
				{
					this.Dsn = OdbcConnectionStringBuilder.ConvertToString(value);
					return;
				}
				if (keywords == OdbcConnectionStringBuilder.Keywords.Driver)
				{
					this.Driver = OdbcConnectionStringBuilder.ConvertToString(value);
					return;
				}
				throw ADP.KeywordNotSupported(keyword);
			}
		}

		/// <summary>Gets or sets the name of the ODBC driver associated with the connection.</summary>
		/// <returns>The value of the <see cref="P:System.Data.Odbc.OdbcConnectionStringBuilder.Driver" /> property, or <see langword="String.Empty" /> if none has been supplied.</returns>
		[DisplayName("Driver")]
		public string Driver
		{
			get
			{
				return this._driver;
			}
			set
			{
				this.SetValue("Driver", value);
				this._driver = value;
			}
		}

		/// <summary>Gets or sets the name of the data source name (DSN) associated with the connection.</summary>
		/// <returns>The value of the <see cref="P:System.Data.Odbc.OdbcConnectionStringBuilder.Dsn" /> property, or <see langword="String.Empty" /> if none has been supplied.</returns>
		[DisplayName("Dsn")]
		public string Dsn
		{
			get
			{
				return this._dsn;
			}
			set
			{
				this.SetValue("Dsn", value);
				this._dsn = value;
			}
		}

		/// <summary>Gets an <see cref="T:System.Collections.ICollection" /> that contains the keys in the <see cref="T:System.Data.Odbc.OdbcConnectionStringBuilder" />.</summary>
		/// <returns>An <see cref="T:System.Collections.ICollection" /> that contains the keys in the <see cref="T:System.Data.Odbc.OdbcConnectionStringBuilder" />.</returns>
		public override ICollection Keys
		{
			get
			{
				string[] array = this._knownKeywords;
				if (array == null)
				{
					array = OdbcConnectionStringBuilder.s_validKeywords;
					int num = 0;
					foreach (object obj in base.Keys)
					{
						string b = (string)obj;
						bool flag = true;
						string[] array2 = array;
						for (int i = 0; i < array2.Length; i++)
						{
							if (array2[i] == b)
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							num++;
						}
					}
					if (0 < num)
					{
						string[] array3 = new string[array.Length + num];
						array.CopyTo(array3, 0);
						int num2 = array.Length;
						foreach (object obj2 in base.Keys)
						{
							string text = (string)obj2;
							bool flag2 = true;
							string[] array2 = array;
							for (int i = 0; i < array2.Length; i++)
							{
								if (array2[i] == text)
								{
									flag2 = false;
									break;
								}
							}
							if (flag2)
							{
								array3[num2++] = text;
							}
						}
						array = array3;
					}
					this._knownKeywords = array;
				}
				return new ReadOnlyCollection<string>(array);
			}
		}

		/// <summary>Clears the contents of the <see cref="T:System.Data.Odbc.OdbcConnectionStringBuilder" /> instance.</summary>
		public override void Clear()
		{
			base.Clear();
			for (int i = 0; i < OdbcConnectionStringBuilder.s_validKeywords.Length; i++)
			{
				this.Reset((OdbcConnectionStringBuilder.Keywords)i);
			}
			this._knownKeywords = OdbcConnectionStringBuilder.s_validKeywords;
		}

		/// <summary>Determines whether the <see cref="T:System.Data.Odbc.OdbcConnectionStringBuilder" /> contains a specific key.</summary>
		/// <param name="keyword">The key to locate in the <see cref="T:System.Data.Odbc.OdbcConnectionStringBuilder" />.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Data.Odbc.OdbcConnectionStringBuilder" /> contains an element that has the specified key; otherwise <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="keyword" /> is null (<see langword="Nothing" /> in Visual Basic).</exception>
		public override bool ContainsKey(string keyword)
		{
			ADP.CheckArgumentNull(keyword, "keyword");
			return OdbcConnectionStringBuilder.s_keywords.ContainsKey(keyword) || base.ContainsKey(keyword);
		}

		private static string ConvertToString(object value)
		{
			return DbConnectionStringBuilderUtil.ConvertToString(value);
		}

		private object GetAt(OdbcConnectionStringBuilder.Keywords index)
		{
			if (index == OdbcConnectionStringBuilder.Keywords.Dsn)
			{
				return this.Dsn;
			}
			if (index == OdbcConnectionStringBuilder.Keywords.Driver)
			{
				return this.Driver;
			}
			throw ADP.KeywordNotSupported(OdbcConnectionStringBuilder.s_validKeywords[(int)index]);
		}

		/// <summary>Removes the entry with the specified key from the <see cref="T:System.Data.Odbc.OdbcConnectionStringBuilder" /> instance.</summary>
		/// <param name="keyword">The key of the key/value pair to be removed from the connection string in this <see cref="T:System.Data.Odbc.OdbcConnectionStringBuilder" />.</param>
		/// <returns>
		///   <see langword="true" /> if the key existed within the connection string and was removed; <see langword="false" /> if the key did not exist.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="keyword" /> is null (<see langword="Nothing" /> in Visual Basic).</exception>
		public override bool Remove(string keyword)
		{
			ADP.CheckArgumentNull(keyword, "keyword");
			if (base.Remove(keyword))
			{
				OdbcConnectionStringBuilder.Keywords index;
				if (OdbcConnectionStringBuilder.s_keywords.TryGetValue(keyword, out index))
				{
					this.Reset(index);
				}
				else
				{
					base.ClearPropertyDescriptors();
					this._knownKeywords = null;
				}
				return true;
			}
			return false;
		}

		private void Reset(OdbcConnectionStringBuilder.Keywords index)
		{
			if (index == OdbcConnectionStringBuilder.Keywords.Dsn)
			{
				this._dsn = "";
				return;
			}
			if (index == OdbcConnectionStringBuilder.Keywords.Driver)
			{
				this._driver = "";
				return;
			}
			throw ADP.KeywordNotSupported(OdbcConnectionStringBuilder.s_validKeywords[(int)index]);
		}

		private void SetValue(string keyword, string value)
		{
			ADP.CheckArgumentNull(value, keyword);
			base[keyword] = value;
		}

		/// <summary>Retrieves a value corresponding to the supplied key from this <see cref="T:System.Data.Odbc.OdbcConnectionStringBuilder" />.</summary>
		/// <param name="keyword">The key of the item to retrieve.</param>
		/// <param name="value">The value corresponding to <paramref name="keyword" />.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="keyword" /> was found within the connection string; otherwise <see langword="false" />.</returns>
		public override bool TryGetValue(string keyword, out object value)
		{
			ADP.CheckArgumentNull(keyword, "keyword");
			OdbcConnectionStringBuilder.Keywords index;
			if (OdbcConnectionStringBuilder.s_keywords.TryGetValue(keyword, out index))
			{
				value = this.GetAt(index);
				return true;
			}
			return base.TryGetValue(keyword, out value);
		}

		private static readonly string[] s_validKeywords;

		private static readonly Dictionary<string, OdbcConnectionStringBuilder.Keywords> s_keywords;

		private string[] _knownKeywords;

		private string _dsn = "";

		private string _driver = "";

		private enum Keywords
		{
			Dsn,
			Driver
		}
	}
}
