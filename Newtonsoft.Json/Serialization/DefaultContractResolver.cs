using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	[NullableContext(1)]
	[Nullable(0)]
	public class DefaultContractResolver : IContractResolver
	{
		internal static IContractResolver Instance
		{
			get
			{
				return DefaultContractResolver._instance;
			}
		}

		public bool DynamicCodeGeneration
		{
			get
			{
				return JsonTypeReflector.DynamicCodeGeneration;
			}
		}

		[Obsolete("DefaultMembersSearchFlags is obsolete. To modify the members serialized inherit from DefaultContractResolver and override the GetSerializableMembers method instead.")]
		public BindingFlags DefaultMembersSearchFlags { get; set; }

		public bool SerializeCompilerGeneratedMembers { get; set; }

		public bool IgnoreSerializableInterface { get; set; }

		public bool IgnoreSerializableAttribute { get; set; }

		public bool IgnoreIsSpecifiedMembers { get; set; }

		public bool IgnoreShouldSerializeMembers { get; set; }

		[Nullable(2)]
		public NamingStrategy NamingStrategy { [NullableContext(2)] get; [NullableContext(2)] set; }

		public DefaultContractResolver()
		{
			this.IgnoreSerializableAttribute = true;
			this.DefaultMembersSearchFlags = (BindingFlags.Instance | BindingFlags.Public);
			this._contractCache = new ThreadSafeStore<Type, JsonContract>(new Func<Type, JsonContract>(this.CreateContract));
		}

		public virtual JsonContract ResolveContract(Type type)
		{
			ValidationUtils.ArgumentNotNull(type, "type");
			return this._contractCache.Get(type);
		}

		private static bool FilterMembers(MemberInfo member)
		{
			PropertyInfo propertyInfo = member as PropertyInfo;
			if (propertyInfo != null)
			{
				return !ReflectionUtils.IsIndexedProperty(propertyInfo) && !ReflectionUtils.IsByRefLikeType(propertyInfo.PropertyType);
			}
			FieldInfo fieldInfo = member as FieldInfo;
			return fieldInfo == null || !ReflectionUtils.IsByRefLikeType(fieldInfo.FieldType);
		}

		protected virtual List<MemberInfo> GetSerializableMembers(Type objectType)
		{
			bool ignoreSerializableAttribute = this.IgnoreSerializableAttribute;
			MemberSerialization objectMemberSerialization = JsonTypeReflector.GetObjectMemberSerialization(objectType, ignoreSerializableAttribute);
			IEnumerable<MemberInfo> enumerable = ReflectionUtils.GetFieldsAndProperties(objectType, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Where(delegate(MemberInfo m)
			{
				PropertyInfo propertyInfo = m as PropertyInfo;
				return propertyInfo == null || !ReflectionUtils.IsIndexedProperty(propertyInfo);
			});
			List<MemberInfo> list = new List<MemberInfo>();
			if (objectMemberSerialization != MemberSerialization.Fields)
			{
				DataContractAttribute dataContractAttribute = JsonTypeReflector.GetDataContractAttribute(objectType);
				List<MemberInfo> list2 = ReflectionUtils.GetFieldsAndProperties(objectType, this.DefaultMembersSearchFlags).Where(new Func<MemberInfo, bool>(DefaultContractResolver.FilterMembers)).ToList<MemberInfo>();
				foreach (MemberInfo memberInfo in enumerable)
				{
					if (this.SerializeCompilerGeneratedMembers || !memberInfo.IsDefined(typeof(CompilerGeneratedAttribute), true))
					{
						if (list2.Contains(memberInfo))
						{
							list.Add(memberInfo);
						}
						else if (JsonTypeReflector.GetAttribute<JsonPropertyAttribute>(memberInfo) != null)
						{
							list.Add(memberInfo);
						}
						else if (JsonTypeReflector.GetAttribute<JsonRequiredAttribute>(memberInfo) != null)
						{
							list.Add(memberInfo);
						}
						else if (dataContractAttribute != null && JsonTypeReflector.GetAttribute<DataMemberAttribute>(memberInfo) != null)
						{
							list.Add(memberInfo);
						}
						else if (objectMemberSerialization == MemberSerialization.Fields && memberInfo.MemberType() == MemberTypes.Field)
						{
							list.Add(memberInfo);
						}
					}
				}
				Type type;
				if (objectType.AssignableToTypeName("System.Data.Objects.DataClasses.EntityObject", false, out type))
				{
					list = list.Where(new Func<MemberInfo, bool>(this.ShouldSerializeEntityMember)).ToList<MemberInfo>();
				}
				if (typeof(Exception).IsAssignableFrom(objectType))
				{
					list = (from m in list
					where !string.Equals(m.Name, "TargetSite", StringComparison.Ordinal)
					select m).ToList<MemberInfo>();
				}
			}
			else
			{
				foreach (MemberInfo memberInfo2 in enumerable)
				{
					FieldInfo fieldInfo = memberInfo2 as FieldInfo;
					if (fieldInfo != null && !fieldInfo.IsStatic)
					{
						list.Add(memberInfo2);
					}
				}
			}
			return list;
		}

		private bool ShouldSerializeEntityMember(MemberInfo memberInfo)
		{
			PropertyInfo propertyInfo = memberInfo as PropertyInfo;
			return propertyInfo == null || !propertyInfo.PropertyType.IsGenericType() || !(propertyInfo.PropertyType.GetGenericTypeDefinition().FullName == "System.Data.Objects.DataClasses.EntityReference`1");
		}

		protected virtual JsonObjectContract CreateObjectContract(Type objectType)
		{
			JsonObjectContract jsonObjectContract = new JsonObjectContract(objectType);
			this.InitializeContract(jsonObjectContract);
			bool ignoreSerializableAttribute = this.IgnoreSerializableAttribute;
			jsonObjectContract.MemberSerialization = JsonTypeReflector.GetObjectMemberSerialization(jsonObjectContract.NonNullableUnderlyingType, ignoreSerializableAttribute);
			jsonObjectContract.Properties.AddRange(this.CreateProperties(jsonObjectContract.NonNullableUnderlyingType, jsonObjectContract.MemberSerialization));
			Func<string, string> func = null;
			JsonObjectAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonObjectAttribute>(jsonObjectContract.NonNullableUnderlyingType);
			if (cachedAttribute != null)
			{
				jsonObjectContract.ItemRequired = cachedAttribute._itemRequired;
				jsonObjectContract.ItemNullValueHandling = cachedAttribute._itemNullValueHandling;
				jsonObjectContract.MissingMemberHandling = cachedAttribute._missingMemberHandling;
				if (cachedAttribute.NamingStrategyType != null)
				{
					NamingStrategy namingStrategy = JsonTypeReflector.GetContainerNamingStrategy(cachedAttribute);
					func = ((string s) => namingStrategy.GetDictionaryKey(s));
				}
			}
			if (func == null)
			{
				func = new Func<string, string>(this.ResolveExtensionDataName);
			}
			jsonObjectContract.ExtensionDataNameResolver = func;
			if (jsonObjectContract.IsInstantiable)
			{
				ConstructorInfo attributeConstructor = this.GetAttributeConstructor(jsonObjectContract.NonNullableUnderlyingType);
				if (attributeConstructor != null)
				{
					jsonObjectContract.OverrideCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(attributeConstructor);
					jsonObjectContract.CreatorParameters.AddRange(this.CreateConstructorParameters(attributeConstructor, jsonObjectContract.Properties));
				}
				else if (jsonObjectContract.MemberSerialization == MemberSerialization.Fields)
				{
					if (JsonTypeReflector.FullyTrusted)
					{
						jsonObjectContract.DefaultCreator = new Func<object>(jsonObjectContract.GetUninitializedObject);
					}
				}
				else if (jsonObjectContract.DefaultCreator == null || jsonObjectContract.DefaultCreatorNonPublic)
				{
					ConstructorInfo parameterizedConstructor = this.GetParameterizedConstructor(jsonObjectContract.NonNullableUnderlyingType);
					if (parameterizedConstructor != null)
					{
						jsonObjectContract.ParameterizedCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(parameterizedConstructor);
						jsonObjectContract.CreatorParameters.AddRange(this.CreateConstructorParameters(parameterizedConstructor, jsonObjectContract.Properties));
					}
				}
				else if (jsonObjectContract.NonNullableUnderlyingType.IsValueType())
				{
					ConstructorInfo immutableConstructor = this.GetImmutableConstructor(jsonObjectContract.NonNullableUnderlyingType, jsonObjectContract.Properties);
					if (immutableConstructor != null)
					{
						jsonObjectContract.OverrideCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(immutableConstructor);
						jsonObjectContract.CreatorParameters.AddRange(this.CreateConstructorParameters(immutableConstructor, jsonObjectContract.Properties));
					}
				}
			}
			MemberInfo extensionDataMemberForType = this.GetExtensionDataMemberForType(jsonObjectContract.NonNullableUnderlyingType);
			if (extensionDataMemberForType != null)
			{
				DefaultContractResolver.SetExtensionDataDelegates(jsonObjectContract, extensionDataMemberForType);
			}
			if (Array.IndexOf<string>(DefaultContractResolver.BlacklistedTypeNames, objectType.FullName) != -1)
			{
				jsonObjectContract.OnSerializingCallbacks.Add(new SerializationCallback(DefaultContractResolver.ThrowUnableToSerializeError));
			}
			return jsonObjectContract;
		}

		private static void ThrowUnableToSerializeError(object o, StreamingContext context)
		{
			throw new JsonSerializationException("Unable to serialize instance of '{0}'.".FormatWith(CultureInfo.InvariantCulture, o.GetType()));
		}

		[return: Nullable(2)]
		private MemberInfo GetExtensionDataMemberForType(Type type)
		{
			return this.GetClassHierarchyForType(type).SelectMany(delegate(Type baseType)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.AddRange(baseType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
				list.AddRange(baseType.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
				return list;
			}).LastOrDefault(delegate(MemberInfo m)
			{
				MemberTypes memberTypes = m.MemberType();
				if (memberTypes != MemberTypes.Property && memberTypes != MemberTypes.Field)
				{
					return false;
				}
				if (!m.IsDefined(typeof(JsonExtensionDataAttribute), false))
				{
					return false;
				}
				if (!ReflectionUtils.CanReadMemberValue(m, true))
				{
					throw new JsonException("Invalid extension data attribute on '{0}'. Member '{1}' must have a getter.".FormatWith(CultureInfo.InvariantCulture, DefaultContractResolver.GetClrTypeFullName(m.DeclaringType), m.Name));
				}
				Type type2;
				if (ReflectionUtils.ImplementsGenericDefinition(ReflectionUtils.GetMemberUnderlyingType(m), typeof(IDictionary<, >), out type2))
				{
					Type type3 = type2.GetGenericArguments()[0];
					Type type4 = type2.GetGenericArguments()[1];
					if (type3.IsAssignableFrom(typeof(string)) && type4.IsAssignableFrom(typeof(JToken)))
					{
						return true;
					}
				}
				throw new JsonException("Invalid extension data attribute on '{0}'. Member '{1}' type must implement IDictionary<string, JToken>.".FormatWith(CultureInfo.InvariantCulture, DefaultContractResolver.GetClrTypeFullName(m.DeclaringType), m.Name));
			});
		}

		private static void SetExtensionDataDelegates(JsonObjectContract contract, MemberInfo member)
		{
			JsonExtensionDataAttribute attribute = ReflectionUtils.GetAttribute<JsonExtensionDataAttribute>(member);
			if (attribute == null)
			{
				return;
			}
			Type memberUnderlyingType = ReflectionUtils.GetMemberUnderlyingType(member);
			Type type;
			ReflectionUtils.ImplementsGenericDefinition(memberUnderlyingType, typeof(IDictionary<, >), out type);
			Type type2 = type.GetGenericArguments()[0];
			Type type3 = type.GetGenericArguments()[1];
			Type type4;
			if (ReflectionUtils.IsGenericDefinition(memberUnderlyingType, typeof(IDictionary<, >)))
			{
				type4 = typeof(Dictionary<, >).MakeGenericType(new Type[]
				{
					type2,
					type3
				});
			}
			else
			{
				type4 = memberUnderlyingType;
			}
			Func<object, object> getExtensionDataDictionary = JsonTypeReflector.ReflectionDelegateFactory.CreateGet<object>(member);
			if (attribute.ReadData)
			{
				Action<object, object> setExtensionDataDictionary = ReflectionUtils.CanSetMemberValue(member, true, false) ? JsonTypeReflector.ReflectionDelegateFactory.CreateSet<object>(member) : null;
				Func<object> createExtensionDataDictionary = JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(type4);
				PropertyInfo property = memberUnderlyingType.GetProperty("Item", BindingFlags.Instance | BindingFlags.Public, null, type3, new Type[]
				{
					type2
				}, null);
				MethodInfo methodInfo = (property != null) ? property.GetSetMethod() : null;
				if (methodInfo == null)
				{
					PropertyInfo property2 = type.GetProperty("Item", BindingFlags.Instance | BindingFlags.Public, null, type3, new Type[]
					{
						type2
					}, null);
					methodInfo = ((property2 != null) ? property2.GetSetMethod() : null);
				}
				MethodCall<object, object> setExtensionDataDictionaryValue = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(methodInfo);
				ExtensionDataSetter extensionDataSetter = delegate(object o, string key, [Nullable(2)] object value)
				{
					object obj = getExtensionDataDictionary(o);
					if (obj == null)
					{
						if (setExtensionDataDictionary == null)
						{
							throw new JsonSerializationException("Cannot set value onto extension data member '{0}'. The extension data collection is null and it cannot be set.".FormatWith(CultureInfo.InvariantCulture, member.Name));
						}
						obj = createExtensionDataDictionary();
						setExtensionDataDictionary(o, obj);
					}
					setExtensionDataDictionaryValue(obj, new object[]
					{
						key,
						value
					});
				};
				contract.ExtensionDataSetter = extensionDataSetter;
			}
			if (attribute.WriteData)
			{
				ConstructorInfo method = typeof(DefaultContractResolver.EnumerableDictionaryWrapper<, >).MakeGenericType(new Type[]
				{
					type2,
					type3
				}).GetConstructors().First<ConstructorInfo>();
				ObjectConstructor<object> createEnumerableWrapper = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(method);
				ExtensionDataGetter extensionDataGetter = delegate(object o)
				{
					object obj = getExtensionDataDictionary(o);
					if (obj == null)
					{
						return null;
					}
					return (IEnumerable<KeyValuePair<object, object>>)createEnumerableWrapper(new object[]
					{
						obj
					});
				};
				contract.ExtensionDataGetter = extensionDataGetter;
			}
			contract.ExtensionDataValueType = type3;
		}

		[return: Nullable(2)]
		private ConstructorInfo GetAttributeConstructor(Type objectType)
		{
			IEnumerator<ConstructorInfo> enumerator = (from c in objectType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
			where c.IsDefined(typeof(JsonConstructorAttribute), true)
			select c).GetEnumerator();
			if (enumerator.MoveNext())
			{
				ConstructorInfo result = enumerator.Current;
				if (enumerator.MoveNext())
				{
					throw new JsonException("Multiple constructors with the JsonConstructorAttribute.");
				}
				return result;
			}
			else
			{
				if (objectType == typeof(Version))
				{
					return objectType.GetConstructor(new Type[]
					{
						typeof(int),
						typeof(int),
						typeof(int),
						typeof(int)
					});
				}
				return null;
			}
		}

		[return: Nullable(2)]
		private ConstructorInfo GetImmutableConstructor(Type objectType, JsonPropertyCollection memberProperties)
		{
			IEnumerator<ConstructorInfo> enumerator = ((IEnumerable<ConstructorInfo>)objectType.GetConstructors()).GetEnumerator();
			if (enumerator.MoveNext())
			{
				ConstructorInfo constructorInfo = enumerator.Current;
				if (!enumerator.MoveNext())
				{
					ParameterInfo[] parameters = constructorInfo.GetParameters();
					if (parameters.Length != 0)
					{
						foreach (ParameterInfo parameterInfo in parameters)
						{
							JsonProperty jsonProperty = this.MatchProperty(memberProperties, parameterInfo.Name, parameterInfo.ParameterType);
							if (jsonProperty == null || jsonProperty.Writable)
							{
								return null;
							}
						}
						return constructorInfo;
					}
				}
			}
			return null;
		}

		[return: Nullable(2)]
		private ConstructorInfo GetParameterizedConstructor(Type objectType)
		{
			ConstructorInfo[] constructors = objectType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
			if (constructors.Length == 1)
			{
				return constructors[0];
			}
			return null;
		}

		protected virtual IList<JsonProperty> CreateConstructorParameters(ConstructorInfo constructor, JsonPropertyCollection memberProperties)
		{
			ParameterInfo[] parameters = constructor.GetParameters();
			JsonPropertyCollection jsonPropertyCollection = new JsonPropertyCollection(constructor.DeclaringType);
			foreach (ParameterInfo parameterInfo in parameters)
			{
				if (parameterInfo.Name != null)
				{
					JsonProperty jsonProperty = this.MatchProperty(memberProperties, parameterInfo.Name, parameterInfo.ParameterType);
					if (jsonProperty != null || parameterInfo.Name != null)
					{
						JsonProperty jsonProperty2 = this.CreatePropertyFromConstructorParameter(jsonProperty, parameterInfo);
						if (jsonProperty2 != null)
						{
							jsonPropertyCollection.AddProperty(jsonProperty2);
						}
					}
				}
			}
			return jsonPropertyCollection;
		}

		[return: Nullable(2)]
		private JsonProperty MatchProperty(JsonPropertyCollection properties, string name, Type type)
		{
			if (name == null)
			{
				return null;
			}
			JsonProperty closestMatchProperty = properties.GetClosestMatchProperty(name);
			if (closestMatchProperty == null || closestMatchProperty.PropertyType != type)
			{
				return null;
			}
			return closestMatchProperty;
		}

		protected virtual JsonProperty CreatePropertyFromConstructorParameter([Nullable(2)] JsonProperty matchingMemberProperty, ParameterInfo parameterInfo)
		{
			JsonProperty jsonProperty = new JsonProperty();
			jsonProperty.PropertyType = parameterInfo.ParameterType;
			jsonProperty.AttributeProvider = new ReflectionAttributeProvider(parameterInfo);
			bool flag;
			this.SetPropertySettingsFromAttributes(jsonProperty, parameterInfo, parameterInfo.Name, parameterInfo.Member.DeclaringType, MemberSerialization.OptOut, out flag);
			jsonProperty.Readable = false;
			jsonProperty.Writable = true;
			if (matchingMemberProperty != null)
			{
				jsonProperty.PropertyName = ((jsonProperty.PropertyName != parameterInfo.Name) ? jsonProperty.PropertyName : matchingMemberProperty.PropertyName);
				jsonProperty.Converter = (jsonProperty.Converter ?? matchingMemberProperty.Converter);
				if (!jsonProperty._hasExplicitDefaultValue && matchingMemberProperty._hasExplicitDefaultValue)
				{
					jsonProperty.DefaultValue = matchingMemberProperty.DefaultValue;
				}
				JsonProperty jsonProperty2 = jsonProperty;
				Required? required = jsonProperty._required;
				jsonProperty2._required = ((required != null) ? required : matchingMemberProperty._required);
				JsonProperty jsonProperty3 = jsonProperty;
				bool? isReference = jsonProperty.IsReference;
				jsonProperty3.IsReference = ((isReference != null) ? isReference : matchingMemberProperty.IsReference);
				JsonProperty jsonProperty4 = jsonProperty;
				NullValueHandling? nullValueHandling = jsonProperty.NullValueHandling;
				jsonProperty4.NullValueHandling = ((nullValueHandling != null) ? nullValueHandling : matchingMemberProperty.NullValueHandling);
				JsonProperty jsonProperty5 = jsonProperty;
				DefaultValueHandling? defaultValueHandling = jsonProperty.DefaultValueHandling;
				jsonProperty5.DefaultValueHandling = ((defaultValueHandling != null) ? defaultValueHandling : matchingMemberProperty.DefaultValueHandling);
				JsonProperty jsonProperty6 = jsonProperty;
				ReferenceLoopHandling? referenceLoopHandling = jsonProperty.ReferenceLoopHandling;
				jsonProperty6.ReferenceLoopHandling = ((referenceLoopHandling != null) ? referenceLoopHandling : matchingMemberProperty.ReferenceLoopHandling);
				JsonProperty jsonProperty7 = jsonProperty;
				ObjectCreationHandling? objectCreationHandling = jsonProperty.ObjectCreationHandling;
				jsonProperty7.ObjectCreationHandling = ((objectCreationHandling != null) ? objectCreationHandling : matchingMemberProperty.ObjectCreationHandling);
				JsonProperty jsonProperty8 = jsonProperty;
				TypeNameHandling? typeNameHandling = jsonProperty.TypeNameHandling;
				jsonProperty8.TypeNameHandling = ((typeNameHandling != null) ? typeNameHandling : matchingMemberProperty.TypeNameHandling);
			}
			return jsonProperty;
		}

		[return: Nullable(2)]
		protected virtual JsonConverter ResolveContractConverter(Type objectType)
		{
			return JsonTypeReflector.GetJsonConverter(objectType);
		}

		private Func<object> GetDefaultCreator(Type createdType)
		{
			return JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(createdType);
		}

		private void InitializeContract(JsonContract contract)
		{
			JsonContainerAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>(contract.NonNullableUnderlyingType);
			if (cachedAttribute != null)
			{
				contract.IsReference = cachedAttribute._isReference;
			}
			else
			{
				DataContractAttribute dataContractAttribute = JsonTypeReflector.GetDataContractAttribute(contract.NonNullableUnderlyingType);
				if (dataContractAttribute != null && dataContractAttribute.IsReference)
				{
					contract.IsReference = new bool?(true);
				}
			}
			contract.Converter = this.ResolveContractConverter(contract.NonNullableUnderlyingType);
			contract.InternalConverter = JsonSerializer.GetMatchingConverter(DefaultContractResolver.BuiltInConverters, contract.NonNullableUnderlyingType);
			if (contract.IsInstantiable && (ReflectionUtils.HasDefaultConstructor(contract.CreatedType, true) || contract.CreatedType.IsValueType()))
			{
				contract.DefaultCreator = this.GetDefaultCreator(contract.CreatedType);
				contract.DefaultCreatorNonPublic = (!contract.CreatedType.IsValueType() && ReflectionUtils.GetDefaultConstructor(contract.CreatedType) == null);
			}
			this.ResolveCallbackMethods(contract, contract.NonNullableUnderlyingType);
		}

		private void ResolveCallbackMethods(JsonContract contract, Type t)
		{
			List<SerializationCallback> list;
			List<SerializationCallback> list2;
			List<SerializationCallback> list3;
			List<SerializationCallback> list4;
			List<SerializationErrorCallback> list5;
			this.GetCallbackMethodsForType(t, out list, out list2, out list3, out list4, out list5);
			if (list != null)
			{
				contract.OnSerializingCallbacks.AddRange(list);
			}
			if (list2 != null)
			{
				contract.OnSerializedCallbacks.AddRange(list2);
			}
			if (list3 != null)
			{
				contract.OnDeserializingCallbacks.AddRange(list3);
			}
			if (list4 != null)
			{
				contract.OnDeserializedCallbacks.AddRange(list4);
			}
			if (list5 != null)
			{
				contract.OnErrorCallbacks.AddRange(list5);
			}
		}

		private void GetCallbackMethodsForType(Type type, [Nullable(new byte[]
		{
			2,
			1
		})] out List<SerializationCallback> onSerializing, [Nullable(new byte[]
		{
			2,
			1
		})] out List<SerializationCallback> onSerialized, [Nullable(new byte[]
		{
			2,
			1
		})] out List<SerializationCallback> onDeserializing, [Nullable(new byte[]
		{
			2,
			1
		})] out List<SerializationCallback> onDeserialized, [Nullable(new byte[]
		{
			2,
			1
		})] out List<SerializationErrorCallback> onError)
		{
			onSerializing = null;
			onSerialized = null;
			onDeserializing = null;
			onDeserialized = null;
			onError = null;
			foreach (Type type2 in this.GetClassHierarchyForType(type))
			{
				MethodInfo currentCallback = null;
				MethodInfo currentCallback2 = null;
				MethodInfo currentCallback3 = null;
				MethodInfo currentCallback4 = null;
				MethodInfo currentCallback5 = null;
				bool flag = DefaultContractResolver.ShouldSkipSerializing(type2);
				bool flag2 = DefaultContractResolver.ShouldSkipDeserialized(type2);
				foreach (MethodInfo methodInfo in type2.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					if (!methodInfo.ContainsGenericParameters)
					{
						Type type3 = null;
						ParameterInfo[] parameters = methodInfo.GetParameters();
						if (!flag && DefaultContractResolver.IsValidCallback(methodInfo, parameters, typeof(OnSerializingAttribute), currentCallback, ref type3))
						{
							onSerializing = (onSerializing ?? new List<SerializationCallback>());
							onSerializing.Add(JsonContract.CreateSerializationCallback(methodInfo));
							currentCallback = methodInfo;
						}
						if (DefaultContractResolver.IsValidCallback(methodInfo, parameters, typeof(OnSerializedAttribute), currentCallback2, ref type3))
						{
							onSerialized = (onSerialized ?? new List<SerializationCallback>());
							onSerialized.Add(JsonContract.CreateSerializationCallback(methodInfo));
							currentCallback2 = methodInfo;
						}
						if (DefaultContractResolver.IsValidCallback(methodInfo, parameters, typeof(OnDeserializingAttribute), currentCallback3, ref type3))
						{
							onDeserializing = (onDeserializing ?? new List<SerializationCallback>());
							onDeserializing.Add(JsonContract.CreateSerializationCallback(methodInfo));
							currentCallback3 = methodInfo;
						}
						if (!flag2 && DefaultContractResolver.IsValidCallback(methodInfo, parameters, typeof(OnDeserializedAttribute), currentCallback4, ref type3))
						{
							onDeserialized = (onDeserialized ?? new List<SerializationCallback>());
							onDeserialized.Add(JsonContract.CreateSerializationCallback(methodInfo));
							currentCallback4 = methodInfo;
						}
						if (DefaultContractResolver.IsValidCallback(methodInfo, parameters, typeof(OnErrorAttribute), currentCallback5, ref type3))
						{
							onError = (onError ?? new List<SerializationErrorCallback>());
							onError.Add(JsonContract.CreateSerializationErrorCallback(methodInfo));
							currentCallback5 = methodInfo;
						}
					}
				}
			}
		}

		private static bool IsConcurrentOrObservableCollection(Type t)
		{
			if (t.IsGenericType())
			{
				string fullName = t.GetGenericTypeDefinition().FullName;
				if (fullName == "System.Collections.Concurrent.ConcurrentQueue`1" || fullName == "System.Collections.Concurrent.ConcurrentStack`1" || fullName == "System.Collections.Concurrent.ConcurrentBag`1" || fullName == "System.Collections.Concurrent.ConcurrentDictionary`2" || fullName == "System.Collections.ObjectModel.ObservableCollection`1")
				{
					return true;
				}
			}
			return false;
		}

		private static bool ShouldSkipDeserialized(Type t)
		{
			return DefaultContractResolver.IsConcurrentOrObservableCollection(t) || (t.Name == "FSharpSet`1" || t.Name == "FSharpMap`2");
		}

		private static bool ShouldSkipSerializing(Type t)
		{
			return DefaultContractResolver.IsConcurrentOrObservableCollection(t) || (t.Name == "FSharpSet`1" || t.Name == "FSharpMap`2");
		}

		private List<Type> GetClassHierarchyForType(Type type)
		{
			List<Type> list = new List<Type>();
			Type type2 = type;
			while (type2 != null && type2 != typeof(object))
			{
				list.Add(type2);
				type2 = type2.BaseType();
			}
			list.Reverse();
			return list;
		}

		protected virtual JsonDictionaryContract CreateDictionaryContract(Type objectType)
		{
			JsonDictionaryContract jsonDictionaryContract = new JsonDictionaryContract(objectType);
			this.InitializeContract(jsonDictionaryContract);
			JsonContainerAttribute attribute = JsonTypeReflector.GetAttribute<JsonContainerAttribute>(objectType);
			if (((attribute != null) ? attribute.NamingStrategyType : null) != null)
			{
				NamingStrategy namingStrategy = JsonTypeReflector.GetContainerNamingStrategy(attribute);
				jsonDictionaryContract.DictionaryKeyResolver = ((string s) => namingStrategy.GetDictionaryKey(s));
			}
			else
			{
				jsonDictionaryContract.DictionaryKeyResolver = new Func<string, string>(this.ResolveDictionaryKey);
			}
			ConstructorInfo attributeConstructor = this.GetAttributeConstructor(jsonDictionaryContract.NonNullableUnderlyingType);
			if (attributeConstructor != null)
			{
				ParameterInfo[] parameters = attributeConstructor.GetParameters();
				Type type = (jsonDictionaryContract.DictionaryKeyType != null && jsonDictionaryContract.DictionaryValueType != null) ? typeof(IEnumerable<>).MakeGenericType(new Type[]
				{
					typeof(KeyValuePair<, >).MakeGenericType(new Type[]
					{
						jsonDictionaryContract.DictionaryKeyType,
						jsonDictionaryContract.DictionaryValueType
					})
				}) : typeof(IDictionary);
				if (parameters.Length == 0)
				{
					jsonDictionaryContract.HasParameterizedCreator = false;
				}
				else
				{
					if (parameters.Length != 1 || !type.IsAssignableFrom(parameters[0].ParameterType))
					{
						throw new JsonException("Constructor for '{0}' must have no parameters or a single parameter that implements '{1}'.".FormatWith(CultureInfo.InvariantCulture, jsonDictionaryContract.UnderlyingType, type));
					}
					jsonDictionaryContract.HasParameterizedCreator = true;
				}
				jsonDictionaryContract.OverrideCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(attributeConstructor);
			}
			return jsonDictionaryContract;
		}

		protected virtual JsonArrayContract CreateArrayContract(Type objectType)
		{
			JsonArrayContract jsonArrayContract = new JsonArrayContract(objectType);
			this.InitializeContract(jsonArrayContract);
			ConstructorInfo attributeConstructor = this.GetAttributeConstructor(jsonArrayContract.NonNullableUnderlyingType);
			if (attributeConstructor != null)
			{
				ParameterInfo[] parameters = attributeConstructor.GetParameters();
				Type type = (jsonArrayContract.CollectionItemType != null) ? typeof(IEnumerable<>).MakeGenericType(new Type[]
				{
					jsonArrayContract.CollectionItemType
				}) : typeof(IEnumerable);
				if (parameters.Length == 0)
				{
					jsonArrayContract.HasParameterizedCreator = false;
				}
				else
				{
					if (parameters.Length != 1 || !type.IsAssignableFrom(parameters[0].ParameterType))
					{
						throw new JsonException("Constructor for '{0}' must have no parameters or a single parameter that implements '{1}'.".FormatWith(CultureInfo.InvariantCulture, jsonArrayContract.UnderlyingType, type));
					}
					jsonArrayContract.HasParameterizedCreator = true;
				}
				jsonArrayContract.OverrideCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(attributeConstructor);
			}
			return jsonArrayContract;
		}

		protected virtual JsonPrimitiveContract CreatePrimitiveContract(Type objectType)
		{
			JsonPrimitiveContract jsonPrimitiveContract = new JsonPrimitiveContract(objectType);
			this.InitializeContract(jsonPrimitiveContract);
			return jsonPrimitiveContract;
		}

		protected virtual JsonLinqContract CreateLinqContract(Type objectType)
		{
			JsonLinqContract jsonLinqContract = new JsonLinqContract(objectType);
			this.InitializeContract(jsonLinqContract);
			return jsonLinqContract;
		}

		protected virtual JsonISerializableContract CreateISerializableContract(Type objectType)
		{
			JsonISerializableContract jsonISerializableContract = new JsonISerializableContract(objectType);
			this.InitializeContract(jsonISerializableContract);
			if (jsonISerializableContract.IsInstantiable)
			{
				ConstructorInfo constructor = jsonISerializableContract.NonNullableUnderlyingType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[]
				{
					typeof(SerializationInfo),
					typeof(StreamingContext)
				}, null);
				if (constructor != null)
				{
					ObjectConstructor<object> iserializableCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(constructor);
					jsonISerializableContract.ISerializableCreator = iserializableCreator;
				}
			}
			return jsonISerializableContract;
		}

		protected virtual JsonDynamicContract CreateDynamicContract(Type objectType)
		{
			JsonDynamicContract jsonDynamicContract = new JsonDynamicContract(objectType);
			this.InitializeContract(jsonDynamicContract);
			JsonContainerAttribute attribute = JsonTypeReflector.GetAttribute<JsonContainerAttribute>(objectType);
			if (((attribute != null) ? attribute.NamingStrategyType : null) != null)
			{
				NamingStrategy namingStrategy = JsonTypeReflector.GetContainerNamingStrategy(attribute);
				jsonDynamicContract.PropertyNameResolver = ((string s) => namingStrategy.GetDictionaryKey(s));
			}
			else
			{
				jsonDynamicContract.PropertyNameResolver = new Func<string, string>(this.ResolveDictionaryKey);
			}
			jsonDynamicContract.Properties.AddRange(this.CreateProperties(objectType, MemberSerialization.OptOut));
			return jsonDynamicContract;
		}

		protected virtual JsonStringContract CreateStringContract(Type objectType)
		{
			JsonStringContract jsonStringContract = new JsonStringContract(objectType);
			this.InitializeContract(jsonStringContract);
			return jsonStringContract;
		}

		protected virtual JsonContract CreateContract(Type objectType)
		{
			Type type = ReflectionUtils.EnsureNotByRefType(objectType);
			if (DefaultContractResolver.IsJsonPrimitiveType(type))
			{
				return this.CreatePrimitiveContract(objectType);
			}
			type = ReflectionUtils.EnsureNotNullableType(type);
			JsonContainerAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>(type);
			if (cachedAttribute is JsonObjectAttribute)
			{
				return this.CreateObjectContract(objectType);
			}
			if (cachedAttribute is JsonArrayAttribute)
			{
				return this.CreateArrayContract(objectType);
			}
			if (cachedAttribute is JsonDictionaryAttribute)
			{
				return this.CreateDictionaryContract(objectType);
			}
			if (type == typeof(JToken) || type.IsSubclassOf(typeof(JToken)))
			{
				return this.CreateLinqContract(objectType);
			}
			if (CollectionUtils.IsDictionaryType(type))
			{
				return this.CreateDictionaryContract(objectType);
			}
			if (typeof(IEnumerable).IsAssignableFrom(type))
			{
				return this.CreateArrayContract(objectType);
			}
			if (DefaultContractResolver.CanConvertToString(type))
			{
				return this.CreateStringContract(objectType);
			}
			if (!this.IgnoreSerializableInterface && typeof(ISerializable).IsAssignableFrom(type) && JsonTypeReflector.IsSerializable(type))
			{
				return this.CreateISerializableContract(objectType);
			}
			if (typeof(IDynamicMetaObjectProvider).IsAssignableFrom(type))
			{
				return this.CreateDynamicContract(objectType);
			}
			if (DefaultContractResolver.IsIConvertible(type))
			{
				return this.CreatePrimitiveContract(type);
			}
			return this.CreateObjectContract(objectType);
		}

		internal static bool IsJsonPrimitiveType(Type t)
		{
			PrimitiveTypeCode typeCode = ConvertUtils.GetTypeCode(t);
			return typeCode != PrimitiveTypeCode.Empty && typeCode != PrimitiveTypeCode.Object;
		}

		internal static bool IsIConvertible(Type t)
		{
			return (typeof(IConvertible).IsAssignableFrom(t) || (ReflectionUtils.IsNullableType(t) && typeof(IConvertible).IsAssignableFrom(Nullable.GetUnderlyingType(t)))) && !typeof(JToken).IsAssignableFrom(t);
		}

		internal static bool CanConvertToString(Type type)
		{
			TypeConverter typeConverter;
			return JsonTypeReflector.CanTypeDescriptorConvertString(type, out typeConverter) || (type == typeof(Type) || type.IsSubclassOf(typeof(Type)));
		}

		private static bool IsValidCallback(MethodInfo method, ParameterInfo[] parameters, Type attributeType, [Nullable(2)] MethodInfo currentCallback, [Nullable(2)] ref Type prevAttributeType)
		{
			if (!method.IsDefined(attributeType, false))
			{
				return false;
			}
			if (currentCallback != null)
			{
				throw new JsonException("Invalid attribute. Both '{0}' and '{1}' in type '{2}' have '{3}'.".FormatWith(CultureInfo.InvariantCulture, method, currentCallback, DefaultContractResolver.GetClrTypeFullName(method.DeclaringType), attributeType));
			}
			if (prevAttributeType != null)
			{
				throw new JsonException("Invalid Callback. Method '{3}' in type '{2}' has both '{0}' and '{1}'.".FormatWith(CultureInfo.InvariantCulture, prevAttributeType, attributeType, DefaultContractResolver.GetClrTypeFullName(method.DeclaringType), method));
			}
			if (method.IsVirtual)
			{
				throw new JsonException("Virtual Method '{0}' of type '{1}' cannot be marked with '{2}' attribute.".FormatWith(CultureInfo.InvariantCulture, method, DefaultContractResolver.GetClrTypeFullName(method.DeclaringType), attributeType));
			}
			if (method.ReturnType != typeof(void))
			{
				throw new JsonException("Serialization Callback '{1}' in type '{0}' must return void.".FormatWith(CultureInfo.InvariantCulture, DefaultContractResolver.GetClrTypeFullName(method.DeclaringType), method));
			}
			if (attributeType == typeof(OnErrorAttribute))
			{
				if (parameters == null || parameters.Length != 2 || parameters[0].ParameterType != typeof(StreamingContext) || parameters[1].ParameterType != typeof(ErrorContext))
				{
					throw new JsonException("Serialization Error Callback '{1}' in type '{0}' must have two parameters of type '{2}' and '{3}'.".FormatWith(CultureInfo.InvariantCulture, DefaultContractResolver.GetClrTypeFullName(method.DeclaringType), method, typeof(StreamingContext), typeof(ErrorContext)));
				}
			}
			else if (parameters == null || parameters.Length != 1 || parameters[0].ParameterType != typeof(StreamingContext))
			{
				throw new JsonException("Serialization Callback '{1}' in type '{0}' must have a single parameter of type '{2}'.".FormatWith(CultureInfo.InvariantCulture, DefaultContractResolver.GetClrTypeFullName(method.DeclaringType), method, typeof(StreamingContext)));
			}
			prevAttributeType = attributeType;
			return true;
		}

		internal static string GetClrTypeFullName(Type type)
		{
			if (type.IsGenericTypeDefinition() || !type.ContainsGenericParameters())
			{
				return type.FullName;
			}
			return "{0}.{1}".FormatWith(CultureInfo.InvariantCulture, type.Namespace, type.Name);
		}

		protected virtual IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
		{
			List<MemberInfo> serializableMembers = this.GetSerializableMembers(type);
			if (serializableMembers == null)
			{
				throw new JsonSerializationException("Null collection of serializable members returned.");
			}
			DefaultJsonNameTable nameTable = this.GetNameTable();
			JsonPropertyCollection jsonPropertyCollection = new JsonPropertyCollection(type);
			foreach (MemberInfo member in serializableMembers)
			{
				JsonProperty jsonProperty = this.CreateProperty(member, memberSerialization);
				if (jsonProperty != null)
				{
					DefaultJsonNameTable obj = nameTable;
					lock (obj)
					{
						jsonProperty.PropertyName = nameTable.Add(jsonProperty.PropertyName);
					}
					jsonPropertyCollection.AddProperty(jsonProperty);
				}
			}
			return jsonPropertyCollection.OrderBy(delegate(JsonProperty p)
			{
				int? order = p.Order;
				if (order == null)
				{
					return -1;
				}
				return order.GetValueOrDefault();
			}).ToList<JsonProperty>();
		}

		internal virtual DefaultJsonNameTable GetNameTable()
		{
			return this._nameTable;
		}

		protected virtual IValueProvider CreateMemberValueProvider(MemberInfo member)
		{
			return new ReflectionValueProvider(member);
		}

		protected virtual JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			JsonProperty jsonProperty = new JsonProperty();
			jsonProperty.PropertyType = ReflectionUtils.GetMemberUnderlyingType(member);
			jsonProperty.DeclaringType = member.DeclaringType;
			jsonProperty.ValueProvider = this.CreateMemberValueProvider(member);
			jsonProperty.AttributeProvider = new ReflectionAttributeProvider(member);
			bool flag;
			this.SetPropertySettingsFromAttributes(jsonProperty, member, member.Name, member.DeclaringType, memberSerialization, out flag);
			if (memberSerialization != MemberSerialization.Fields)
			{
				jsonProperty.Readable = ReflectionUtils.CanReadMemberValue(member, flag);
				jsonProperty.Writable = ReflectionUtils.CanSetMemberValue(member, flag, jsonProperty.HasMemberAttribute);
			}
			else
			{
				jsonProperty.Readable = true;
				jsonProperty.Writable = true;
			}
			if (!this.IgnoreShouldSerializeMembers)
			{
				jsonProperty.ShouldSerialize = this.CreateShouldSerializeTest(member);
			}
			if (!this.IgnoreIsSpecifiedMembers)
			{
				this.SetIsSpecifiedActions(jsonProperty, member, flag);
			}
			return jsonProperty;
		}

		private void SetPropertySettingsFromAttributes(JsonProperty property, object attributeProvider, string name, Type declaringType, MemberSerialization memberSerialization, out bool allowNonPublicAccess)
		{
			bool dataContractAttribute = JsonTypeReflector.GetDataContractAttribute(declaringType) != null;
			MemberInfo memberInfo = attributeProvider as MemberInfo;
			DataMemberAttribute dataMemberAttribute;
			if (dataContractAttribute && memberInfo != null)
			{
				dataMemberAttribute = JsonTypeReflector.GetDataMemberAttribute(memberInfo);
			}
			else
			{
				dataMemberAttribute = null;
			}
			JsonPropertyAttribute attribute = JsonTypeReflector.GetAttribute<JsonPropertyAttribute>(attributeProvider);
			bool attribute2 = JsonTypeReflector.GetAttribute<JsonRequiredAttribute>(attributeProvider) != null;
			string text;
			bool hasSpecifiedName;
			if (attribute != null && attribute.PropertyName != null)
			{
				text = attribute.PropertyName;
				hasSpecifiedName = true;
			}
			else if (dataMemberAttribute != null && dataMemberAttribute.Name != null)
			{
				text = dataMemberAttribute.Name;
				hasSpecifiedName = true;
			}
			else
			{
				text = name;
				hasSpecifiedName = false;
			}
			JsonContainerAttribute attribute3 = JsonTypeReflector.GetAttribute<JsonContainerAttribute>(declaringType);
			NamingStrategy namingStrategy;
			if (((attribute != null) ? attribute.NamingStrategyType : null) != null)
			{
				namingStrategy = JsonTypeReflector.CreateNamingStrategyInstance(attribute.NamingStrategyType, attribute.NamingStrategyParameters);
			}
			else if (((attribute3 != null) ? attribute3.NamingStrategyType : null) != null)
			{
				namingStrategy = JsonTypeReflector.GetContainerNamingStrategy(attribute3);
			}
			else
			{
				namingStrategy = this.NamingStrategy;
			}
			if (namingStrategy != null)
			{
				property.PropertyName = namingStrategy.GetPropertyName(text, hasSpecifiedName);
			}
			else
			{
				property.PropertyName = this.ResolvePropertyName(text);
			}
			property.UnderlyingName = name;
			bool flag = false;
			if (attribute != null)
			{
				property._required = attribute._required;
				property.Order = attribute._order;
				property.DefaultValueHandling = attribute._defaultValueHandling;
				flag = true;
				property.NullValueHandling = attribute._nullValueHandling;
				property.ReferenceLoopHandling = attribute._referenceLoopHandling;
				property.ObjectCreationHandling = attribute._objectCreationHandling;
				property.TypeNameHandling = attribute._typeNameHandling;
				property.IsReference = attribute._isReference;
				property.ItemIsReference = attribute._itemIsReference;
				property.ItemConverter = ((attribute.ItemConverterType != null) ? JsonTypeReflector.CreateJsonConverterInstance(attribute.ItemConverterType, attribute.ItemConverterParameters) : null);
				property.ItemReferenceLoopHandling = attribute._itemReferenceLoopHandling;
				property.ItemTypeNameHandling = attribute._itemTypeNameHandling;
			}
			else
			{
				property.NullValueHandling = null;
				property.ReferenceLoopHandling = null;
				property.ObjectCreationHandling = null;
				property.TypeNameHandling = null;
				property.IsReference = null;
				property.ItemIsReference = null;
				property.ItemConverter = null;
				property.ItemReferenceLoopHandling = null;
				property.ItemTypeNameHandling = null;
				if (dataMemberAttribute != null)
				{
					property._required = new Required?(dataMemberAttribute.IsRequired ? Required.AllowNull : Required.Default);
					property.Order = ((dataMemberAttribute.Order != -1) ? new int?(dataMemberAttribute.Order) : null);
					property.DefaultValueHandling = ((!dataMemberAttribute.EmitDefaultValue) ? new DefaultValueHandling?(DefaultValueHandling.Ignore) : null);
					flag = true;
				}
			}
			if (attribute2)
			{
				property._required = new Required?(Required.Always);
				flag = true;
			}
			property.HasMemberAttribute = flag;
			bool flag2 = JsonTypeReflector.GetAttribute<JsonIgnoreAttribute>(attributeProvider) != null || JsonTypeReflector.GetAttribute<JsonExtensionDataAttribute>(attributeProvider) != null || JsonTypeReflector.IsNonSerializable(attributeProvider);
			if (memberSerialization != MemberSerialization.OptIn)
			{
				bool flag3 = JsonTypeReflector.GetAttribute<IgnoreDataMemberAttribute>(attributeProvider) != null;
				property.Ignored = (flag2 || flag3);
			}
			else
			{
				property.Ignored = (flag2 || !flag);
			}
			property.Converter = JsonTypeReflector.GetJsonConverter(attributeProvider);
			DefaultValueAttribute attribute4 = JsonTypeReflector.GetAttribute<DefaultValueAttribute>(attributeProvider);
			if (attribute4 != null)
			{
				property.DefaultValue = attribute4.Value;
			}
			allowNonPublicAccess = false;
			if ((this.DefaultMembersSearchFlags & BindingFlags.NonPublic) == BindingFlags.NonPublic)
			{
				allowNonPublicAccess = true;
			}
			if (flag)
			{
				allowNonPublicAccess = true;
			}
			if (memberSerialization == MemberSerialization.Fields)
			{
				allowNonPublicAccess = true;
			}
		}

		[return: Nullable(new byte[]
		{
			2,
			1
		})]
		private Predicate<object> CreateShouldSerializeTest(MemberInfo member)
		{
			MethodInfo method = member.DeclaringType.GetMethod("ShouldSerialize" + member.Name, ReflectionUtils.EmptyTypes);
			if (method == null || method.ReturnType != typeof(bool))
			{
				return null;
			}
			MethodCall<object, object> shouldSerializeCall = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(method);
			return (object o) => (bool)shouldSerializeCall(o, Array.Empty<object>());
		}

		private void SetIsSpecifiedActions(JsonProperty property, MemberInfo member, bool allowNonPublicAccess)
		{
			MemberInfo memberInfo = member.DeclaringType.GetProperty(member.Name + "Specified", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (memberInfo == null)
			{
				memberInfo = member.DeclaringType.GetField(member.Name + "Specified", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			}
			if (memberInfo == null || ReflectionUtils.GetMemberUnderlyingType(memberInfo) != typeof(bool))
			{
				return;
			}
			Func<object, object> specifiedPropertyGet = JsonTypeReflector.ReflectionDelegateFactory.CreateGet<object>(memberInfo);
			property.GetIsSpecified = ((object o) => (bool)specifiedPropertyGet(o));
			if (ReflectionUtils.CanSetMemberValue(memberInfo, allowNonPublicAccess, false))
			{
				property.SetIsSpecified = JsonTypeReflector.ReflectionDelegateFactory.CreateSet<object>(memberInfo);
			}
		}

		protected virtual string ResolvePropertyName(string propertyName)
		{
			if (this.NamingStrategy != null)
			{
				return this.NamingStrategy.GetPropertyName(propertyName, false);
			}
			return propertyName;
		}

		protected virtual string ResolveExtensionDataName(string extensionDataName)
		{
			if (this.NamingStrategy != null)
			{
				return this.NamingStrategy.GetExtensionDataName(extensionDataName);
			}
			return extensionDataName;
		}

		protected virtual string ResolveDictionaryKey(string dictionaryKey)
		{
			if (this.NamingStrategy != null)
			{
				return this.NamingStrategy.GetDictionaryKey(dictionaryKey);
			}
			return this.ResolvePropertyName(dictionaryKey);
		}

		public string GetResolvedPropertyName(string propertyName)
		{
			return this.ResolvePropertyName(propertyName);
		}

		private static readonly IContractResolver _instance = new DefaultContractResolver();

		private static readonly string[] BlacklistedTypeNames = new string[]
		{
			"System.IO.DriveInfo",
			"System.IO.FileInfo",
			"System.IO.DirectoryInfo"
		};

		private static readonly JsonConverter[] BuiltInConverters = new JsonConverter[]
		{
			new EntityKeyMemberConverter(),
			new ExpandoObjectConverter(),
			new XmlNodeConverter(),
			new BinaryConverter(),
			new DataSetConverter(),
			new DataTableConverter(),
			new DiscriminatedUnionConverter(),
			new KeyValuePairConverter(),
			new BsonObjectIdConverter(),
			new RegexConverter()
		};

		private readonly DefaultJsonNameTable _nameTable = new DefaultJsonNameTable();

		private readonly ThreadSafeStore<Type, JsonContract> _contractCache;

		[NullableContext(0)]
		internal class EnumerableDictionaryWrapper<[Nullable(2)] TEnumeratorKey, [Nullable(2)] TEnumeratorValue> : IEnumerable<KeyValuePair<object, object>>, IEnumerable
		{
			public EnumerableDictionaryWrapper([Nullable(new byte[]
			{
				1,
				0,
				1,
				1
			})] IEnumerable<KeyValuePair<TEnumeratorKey, TEnumeratorValue>> e)
			{
				ValidationUtils.ArgumentNotNull(e, "e");
				this._e = e;
			}

			[return: Nullable(new byte[]
			{
				1,
				0,
				1,
				1
			})]
			public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
			{
				foreach (KeyValuePair<TEnumeratorKey, TEnumeratorValue> keyValuePair in this._e)
				{
					yield return new KeyValuePair<object, object>(keyValuePair.Key, keyValuePair.Value);
				}
				IEnumerator<KeyValuePair<TEnumeratorKey, TEnumeratorValue>> enumerator = null;
				yield break;
				yield break;
			}

			[NullableContext(1)]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			[Nullable(new byte[]
			{
				1,
				0,
				1,
				1
			})]
			private readonly IEnumerable<KeyValuePair<TEnumeratorKey, TEnumeratorValue>> _e;
		}
	}
}
