using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Valve.Newtonsoft.Json.Utilities;

namespace Valve.Newtonsoft.Json.Serialization
{
	public class JsonDictionaryContract : JsonContainerContract
	{
		[Obsolete("PropertyNameResolver is obsolete. Use DictionaryKeyResolver instead.")]
		public Func<string, string> PropertyNameResolver
		{
			get
			{
				return this.DictionaryKeyResolver;
			}
			set
			{
				this.DictionaryKeyResolver = value;
			}
		}

		public Func<string, string> DictionaryKeyResolver { get; set; }

		public Type DictionaryKeyType { get; private set; }

		public Type DictionaryValueType { get; private set; }

		internal JsonContract KeyContract { get; set; }

		internal bool ShouldCreateWrapper { get; private set; }

		internal ObjectConstructor<object> ParameterizedCreator
		{
			get
			{
				if (this._parameterizedCreator == null)
				{
					this._parameterizedCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(this._parameterizedConstructor);
				}
				return this._parameterizedCreator;
			}
		}

		public ObjectConstructor<object> OverrideCreator
		{
			get
			{
				return this._overrideCreator;
			}
			set
			{
				this._overrideCreator = value;
			}
		}

		public bool HasParameterizedCreator { get; set; }

		internal bool HasParameterizedCreatorInternal
		{
			get
			{
				return this.HasParameterizedCreator || this._parameterizedCreator != null || this._parameterizedConstructor != null;
			}
		}

		public JsonDictionaryContract(Type underlyingType) : base(underlyingType)
		{
			this.ContractType = JsonContractType.Dictionary;
			Type type;
			Type type2;
			if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof(IDictionary<, >), out this._genericCollectionDefinitionType))
			{
				type = this._genericCollectionDefinitionType.GetGenericArguments()[0];
				type2 = this._genericCollectionDefinitionType.GetGenericArguments()[1];
				if (ReflectionUtils.IsGenericDefinition(base.UnderlyingType, typeof(IDictionary<, >)))
				{
					base.CreatedType = typeof(Dictionary<, >).MakeGenericType(new Type[]
					{
						type,
						type2
					});
				}
			}
			else
			{
				ReflectionUtils.GetDictionaryKeyValueTypes(base.UnderlyingType, out type, out type2);
				if (base.UnderlyingType == typeof(IDictionary))
				{
					base.CreatedType = typeof(Dictionary<object, object>);
				}
			}
			if (type != null && type2 != null)
			{
				this._parameterizedConstructor = CollectionUtils.ResolveEnumerableCollectionConstructor(base.CreatedType, typeof(KeyValuePair<, >).MakeGenericType(new Type[]
				{
					type,
					type2
				}), typeof(IDictionary<, >).MakeGenericType(new Type[]
				{
					type,
					type2
				}));
			}
			this.ShouldCreateWrapper = !typeof(IDictionary).IsAssignableFrom(base.CreatedType);
			this.DictionaryKeyType = type;
			this.DictionaryValueType = type2;
			Type type3;
			if (this.DictionaryValueType != null && ReflectionUtils.IsNullableType(this.DictionaryValueType) && ReflectionUtils.InheritsGenericDefinition(base.CreatedType, typeof(Dictionary<, >), out type3))
			{
				this.ShouldCreateWrapper = true;
			}
		}

		internal IWrappedDictionary CreateWrapper(object dictionary)
		{
			if (this._genericWrapperCreator == null)
			{
				this._genericWrapperType = typeof(DictionaryWrapper<, >).MakeGenericType(new Type[]
				{
					this.DictionaryKeyType,
					this.DictionaryValueType
				});
				ConstructorInfo constructor = this._genericWrapperType.GetConstructor(new Type[]
				{
					this._genericCollectionDefinitionType
				});
				this._genericWrapperCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(constructor);
			}
			return (IWrappedDictionary)this._genericWrapperCreator(new object[]
			{
				dictionary
			});
		}

		internal IDictionary CreateTemporaryDictionary()
		{
			if (this._genericTemporaryDictionaryCreator == null)
			{
				Type type = typeof(Dictionary<, >).MakeGenericType(new Type[]
				{
					this.DictionaryKeyType ?? typeof(object),
					this.DictionaryValueType ?? typeof(object)
				});
				this._genericTemporaryDictionaryCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(type);
			}
			return (IDictionary)this._genericTemporaryDictionaryCreator();
		}

		private readonly Type _genericCollectionDefinitionType;

		private Type _genericWrapperType;

		private ObjectConstructor<object> _genericWrapperCreator;

		private Func<object> _genericTemporaryDictionaryCreator;

		private readonly ConstructorInfo _parameterizedConstructor;

		private ObjectConstructor<object> _overrideCreator;

		private ObjectConstructor<object> _parameterizedCreator;
	}
}
