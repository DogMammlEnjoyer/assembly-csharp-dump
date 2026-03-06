using System;
using System.Xml;

namespace System.Runtime.Serialization
{
	/// <summary>Provides a mechanism for dynamically mapping types to and from <see langword="xsi:type" /> representations during serialization and deserialization.</summary>
	public abstract class DataContractResolver
	{
		/// <summary>Override this method to map a data contract type to an <see langword="xsi:type" /> name and namespace during serialization.</summary>
		/// <param name="type">The type to map.</param>
		/// <param name="declaredType">The type declared in the data contract.</param>
		/// <param name="knownTypeResolver">The known type resolver.</param>
		/// <param name="typeName">The xsi:type name.</param>
		/// <param name="typeNamespace">The xsi:type namespace.</param>
		/// <returns>
		///   <see langword="true" /> if mapping succeeded; otherwise, <see langword="false" />.</returns>
		public abstract bool TryResolveType(Type type, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace);

		/// <summary>Override this method to map the specified <see langword="xsi:type" /> name and namespace to a data contract type during deserialization.</summary>
		/// <param name="typeName">The <see langword="xsi:type" /> name to map.</param>
		/// <param name="typeNamespace">The <see langword="xsi:type" /> namespace to map.</param>
		/// <param name="declaredType">The type declared in the data contract.</param>
		/// <param name="knownTypeResolver">The known type resolver.</param>
		/// <returns>The type the <see langword="xsi:type" /> name and namespace is mapped to.</returns>
		public abstract Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver);
	}
}
