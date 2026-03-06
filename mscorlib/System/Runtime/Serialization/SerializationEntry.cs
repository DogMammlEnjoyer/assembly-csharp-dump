using System;

namespace System.Runtime.Serialization
{
	/// <summary>Holds the value, <see cref="T:System.Type" />, and name of a serialized object.</summary>
	public readonly struct SerializationEntry
	{
		internal SerializationEntry(string entryName, object entryValue, Type entryType)
		{
			this._name = entryName;
			this._value = entryValue;
			this._type = entryType;
		}

		/// <summary>Gets the value contained in the object.</summary>
		/// <returns>The value contained in the object.</returns>
		public object Value
		{
			get
			{
				return this._value;
			}
		}

		/// <summary>Gets the name of the object.</summary>
		/// <returns>The name of the object.</returns>
		public string Name
		{
			get
			{
				return this._name;
			}
		}

		/// <summary>Gets the <see cref="T:System.Type" /> of the object.</summary>
		/// <returns>The <see cref="T:System.Type" /> of the object.</returns>
		public Type ObjectType
		{
			get
			{
				return this._type;
			}
		}

		private readonly string _name;

		private readonly object _value;

		private readonly Type _type;
	}
}
