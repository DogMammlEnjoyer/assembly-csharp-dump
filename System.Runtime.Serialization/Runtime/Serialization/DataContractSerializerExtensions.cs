using System;
using System.CodeDom;
using System.Collections.ObjectModel;
using System.Reflection;

namespace System.Runtime.Serialization
{
	/// <summary>Extends the <see cref="T:System.Runtime.Serialization.DataContractSerializer" /> class by providing methods for setting and getting an <see cref="T:System.Runtime.Serialization.ISerializationSurrogateProvider" />.</summary>
	public static class DataContractSerializerExtensions
	{
		/// <summary>Returns the surrogate serialization provider for this serializer.</summary>
		/// <param name="serializer">The serializer which is being surrogated.</param>
		/// <returns>The surrogate serializer.</returns>
		public static ISerializationSurrogateProvider GetSerializationSurrogateProvider(this DataContractSerializer serializer)
		{
			DataContractSerializerExtensions.SurrogateProviderAdapter surrogateProviderAdapter = serializer.DataContractSurrogate as DataContractSerializerExtensions.SurrogateProviderAdapter;
			if (surrogateProviderAdapter != null)
			{
				return surrogateProviderAdapter.Provider;
			}
			return null;
		}

		/// <summary>Specifies a surrogate serialization provider for this <see cref="T:System.Runtime.Serialization.DataContractSerializer" />.</summary>
		/// <param name="serializer">The serializer which is being surrogated.</param>
		/// <param name="provider">The surrogate serialization provider.</param>
		public static void SetSerializationSurrogateProvider(this DataContractSerializer serializer, ISerializationSurrogateProvider provider)
		{
			IDataContractSurrogate value = (provider != null) ? new DataContractSerializerExtensions.SurrogateProviderAdapter(provider) : null;
			typeof(DataContractSerializer).GetField("dataContractSurrogate", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(serializer, value);
		}

		private class SurrogateProviderAdapter : IDataContractSurrogate
		{
			public SurrogateProviderAdapter(ISerializationSurrogateProvider provider)
			{
				this._provider = provider;
			}

			public ISerializationSurrogateProvider Provider
			{
				get
				{
					return this._provider;
				}
			}

			public object GetCustomDataToExport(Type clrType, Type dataContractType)
			{
				throw NotImplemented.ByDesign;
			}

			public object GetCustomDataToExport(MemberInfo memberInfo, Type dataContractType)
			{
				throw NotImplemented.ByDesign;
			}

			public Type GetDataContractType(Type type)
			{
				return this._provider.GetSurrogateType(type);
			}

			public object GetDeserializedObject(object obj, Type targetType)
			{
				return this._provider.GetDeserializedObject(obj, targetType);
			}

			public void GetKnownCustomDataTypes(Collection<Type> customDataTypes)
			{
				throw NotImplemented.ByDesign;
			}

			public object GetObjectToSerialize(object obj, Type targetType)
			{
				return this._provider.GetObjectToSerialize(obj, targetType);
			}

			public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
			{
				throw NotImplemented.ByDesign;
			}

			public CodeTypeDeclaration ProcessImportedType(CodeTypeDeclaration typeDeclaration, CodeCompileUnit compileUnit)
			{
				throw NotImplemented.ByDesign;
			}

			private ISerializationSurrogateProvider _provider;
		}
	}
}
