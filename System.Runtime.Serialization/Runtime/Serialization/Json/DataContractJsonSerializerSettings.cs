using System;
using System.Collections.Generic;

namespace System.Runtime.Serialization.Json
{
	/// <summary>Specifies <see cref="T:System.Runtime.Serialization.Json.DataContractJsonSerializer" /> settings.</summary>
	public class DataContractJsonSerializerSettings
	{
		/// <summary>Gets or sets the root name of the selected object.</summary>
		/// <returns>The root name of the selected object.</returns>
		public string RootName { get; set; }

		/// <summary>Gets or sets a collection of types that may be present in the object graph serialized using this instance the DataContractJsonSerializerSettings.</summary>
		/// <returns>A collection of types that may be present in the object graph serialized using this instance the DataContractJsonSerializerSettings.</returns>
		public IEnumerable<Type> KnownTypes { get; set; }

		/// <summary>Gets or sets the maximum number of items in an object graph to serialize or deserialize.</summary>
		/// <returns>The maximum number of items in an object graph to serialize or deserialize.</returns>
		public int MaxItemsInObjectGraph
		{
			get
			{
				return this.maxItemsInObjectGraph;
			}
			set
			{
				this.maxItemsInObjectGraph = value;
			}
		}

		/// <summary>Gets or sets a value that specifies whether to ignore data supplied by an extension of the class when the class is being serialized or deserialized.</summary>
		/// <returns>
		///   <see langword="true" /> to ignore data supplied by an extension of the class when the class is being serialized or deserialized; otherwise, <see langword="false" />.</returns>
		public bool IgnoreExtensionDataObject { get; set; }

		/// <summary>Gets or sets a surrogate type that is currently active for given IDataContractSurrogate instance.</summary>
		/// <returns>The surrogate type that is currently active for given IDataContractSurrogate instance.</returns>
		public IDataContractSurrogate DataContractSurrogate { get; set; }

		/// <summary>Gets or sets the data contract JSON serializer settings to emit type information.</summary>
		/// <returns>The data contract JSON serializer settings to emit type information.</returns>
		public EmitTypeInformation EmitTypeInformation { get; set; }

		/// <summary>Gets or sets a DateTimeFormat that defines the culturally appropriate format of displaying dates and times.</summary>
		/// <returns>The DateTimeFormat that defines the culturally appropriate format of displaying dates and times.</returns>
		public DateTimeFormat DateTimeFormat { get; set; }

		/// <summary>Gets or sets a value that specifies whether to serialize read only types.</summary>
		/// <returns>
		///   <see langword="true" /> to serialize read only types; otherwise <see langword="false" />.</returns>
		public bool SerializeReadOnlyTypes { get; set; }

		/// <summary>Gets or sets a value that specifies whether to use a simple dictionary format.</summary>
		/// <returns>
		///   <see langword="true" /> to use a simple dictionary format; otherwise, <see langword="false" />.</returns>
		public bool UseSimpleDictionaryFormat { get; set; }

		private int maxItemsInObjectGraph = int.MaxValue;
	}
}
