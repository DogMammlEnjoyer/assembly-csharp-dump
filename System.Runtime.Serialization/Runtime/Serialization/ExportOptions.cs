using System;
using System.Collections.ObjectModel;

namespace System.Runtime.Serialization
{
	/// <summary>Represents the options that can be set for an <see cref="T:System.Runtime.Serialization.XsdDataContractExporter" />.</summary>
	public class ExportOptions
	{
		/// <summary>Gets or sets a serialization surrogate.</summary>
		/// <returns>An implementation of the <see cref="T:System.Runtime.Serialization.IDataContractSurrogate" /> interface that can be used to customize how an XML schema representation is exported for a specific type.</returns>
		public IDataContractSurrogate DataContractSurrogate
		{
			get
			{
				return this.dataContractSurrogate;
			}
			set
			{
				this.dataContractSurrogate = value;
			}
		}

		internal IDataContractSurrogate GetSurrogate()
		{
			return this.dataContractSurrogate;
		}

		/// <summary>Gets the collection of types that may be encountered during serialization or deserialization.</summary>
		/// <returns>A <see langword="KnownTypes" /> collection that contains types that may be encountered during serialization or deserialization. XML schema representations are exported for all the types specified in this collection by the <see cref="T:System.Runtime.Serialization.XsdDataContractExporter" />.</returns>
		public Collection<Type> KnownTypes
		{
			get
			{
				if (this.knownTypes == null)
				{
					this.knownTypes = new Collection<Type>();
				}
				return this.knownTypes;
			}
		}

		private Collection<Type> knownTypes;

		private IDataContractSurrogate dataContractSurrogate;
	}
}
