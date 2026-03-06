using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace PlayFab.Json
{
	[GeneratedCode("simple-json", "1.0.0")]
	public class PocoJsonSerializerStrategy : IJsonSerializerStrategy
	{
		public PocoJsonSerializerStrategy()
		{
			this.ConstructorCache = new ReflectionUtils.ThreadSafeDictionary<Type, ReflectionUtils.ConstructorDelegate>(new ReflectionUtils.ThreadSafeDictionaryValueFactory<Type, ReflectionUtils.ConstructorDelegate>(this.ContructorDelegateFactory));
			this.GetCache = new ReflectionUtils.ThreadSafeDictionary<Type, IDictionary<MemberInfo, ReflectionUtils.GetDelegate>>(new ReflectionUtils.ThreadSafeDictionaryValueFactory<Type, IDictionary<MemberInfo, ReflectionUtils.GetDelegate>>(this.GetterValueFactory));
			this.SetCache = new ReflectionUtils.ThreadSafeDictionary<Type, IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>>(new ReflectionUtils.ThreadSafeDictionaryValueFactory<Type, IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>>(this.SetterValueFactory));
		}

		protected virtual string MapClrMemberNameToJsonFieldName(MemberInfo memberInfo)
		{
			foreach (JsonProperty jsonProperty in memberInfo.GetCustomAttributes(typeof(JsonProperty), true))
			{
				if (!string.IsNullOrEmpty(jsonProperty.PropertyName))
				{
					return jsonProperty.PropertyName;
				}
			}
			return memberInfo.Name;
		}

		protected virtual void MapClrMemberNameToJsonFieldName(MemberInfo memberInfo, out string jsonName, out JsonProperty jsonProp)
		{
			jsonName = memberInfo.Name;
			jsonProp = null;
			foreach (JsonProperty jsonProperty in memberInfo.GetCustomAttributes(typeof(JsonProperty), true))
			{
				jsonProp = jsonProperty;
				if (!string.IsNullOrEmpty(jsonProperty.PropertyName))
				{
					jsonName = jsonProperty.PropertyName;
				}
			}
		}

		internal virtual ReflectionUtils.ConstructorDelegate ContructorDelegateFactory(Type key)
		{
			return ReflectionUtils.GetContructor(key, key.IsArray ? PocoJsonSerializerStrategy.ArrayConstructorParameterTypes : PocoJsonSerializerStrategy.EmptyTypes);
		}

		internal virtual IDictionary<MemberInfo, ReflectionUtils.GetDelegate> GetterValueFactory(Type type)
		{
			IDictionary<MemberInfo, ReflectionUtils.GetDelegate> dictionary = new Dictionary<MemberInfo, ReflectionUtils.GetDelegate>();
			foreach (PropertyInfo propertyInfo in ReflectionUtils.GetProperties(type))
			{
				if (propertyInfo.CanRead)
				{
					MethodInfo getterMethodInfo = ReflectionUtils.GetGetterMethodInfo(propertyInfo);
					if (!getterMethodInfo.IsStatic && getterMethodInfo.IsPublic)
					{
						dictionary[propertyInfo] = ReflectionUtils.GetGetMethod(propertyInfo);
					}
				}
			}
			foreach (FieldInfo fieldInfo in ReflectionUtils.GetFields(type))
			{
				if (!fieldInfo.IsStatic && fieldInfo.IsPublic)
				{
					dictionary[fieldInfo] = ReflectionUtils.GetGetMethod(fieldInfo);
				}
			}
			return dictionary;
		}

		internal virtual IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>> SetterValueFactory(Type type)
		{
			IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>> dictionary = new Dictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>();
			foreach (PropertyInfo propertyInfo in ReflectionUtils.GetProperties(type))
			{
				if (propertyInfo.CanWrite)
				{
					MethodInfo setterMethodInfo = ReflectionUtils.GetSetterMethodInfo(propertyInfo);
					if (!setterMethodInfo.IsStatic && setterMethodInfo.IsPublic)
					{
						dictionary[this.MapClrMemberNameToJsonFieldName(propertyInfo)] = new KeyValuePair<Type, ReflectionUtils.SetDelegate>(propertyInfo.PropertyType, ReflectionUtils.GetSetMethod(propertyInfo));
					}
				}
			}
			foreach (FieldInfo fieldInfo in ReflectionUtils.GetFields(type))
			{
				if (!fieldInfo.IsInitOnly && !fieldInfo.IsStatic && fieldInfo.IsPublic)
				{
					dictionary[this.MapClrMemberNameToJsonFieldName(fieldInfo)] = new KeyValuePair<Type, ReflectionUtils.SetDelegate>(fieldInfo.FieldType, ReflectionUtils.GetSetMethod(fieldInfo));
				}
			}
			return dictionary;
		}

		public virtual bool TrySerializeNonPrimitiveObject(object input, out object output)
		{
			return this.TrySerializeKnownTypes(input, out output) || this.TrySerializeUnknownTypes(input, out output);
		}

		public virtual object DeserializeObject(object value, Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (value != null && type.IsInstanceOfType(value))
			{
				return value;
			}
			string text = value as string;
			if (type == typeof(Guid) && string.IsNullOrEmpty(text))
			{
				return default(Guid);
			}
			if (value == null)
			{
				return null;
			}
			object obj = null;
			if (text != null)
			{
				if (text.Length != 0)
				{
					if (type == typeof(DateTime) || (ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(DateTime)))
					{
						return DateTime.ParseExact(text, PocoJsonSerializerStrategy.Iso8601Format, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
					}
					if (type == typeof(DateTimeOffset) || (ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(DateTimeOffset)))
					{
						return DateTimeOffset.ParseExact(text, PocoJsonSerializerStrategy.Iso8601Format, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
					}
					if (type == typeof(Guid) || (ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(Guid)))
					{
						return new Guid(text);
					}
					if (type == typeof(Uri))
					{
						Uri result;
						if (Uri.IsWellFormedUriString(text, UriKind.RelativeOrAbsolute) && Uri.TryCreate(text, UriKind.RelativeOrAbsolute, out result))
						{
							return result;
						}
						return null;
					}
					else
					{
						if (type == typeof(string))
						{
							return text;
						}
						return Convert.ChangeType(text, type, CultureInfo.InvariantCulture);
					}
				}
				else
				{
					if (type == typeof(Guid))
					{
						obj = default(Guid);
					}
					else if (ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(Guid))
					{
						obj = null;
					}
					else
					{
						obj = text;
					}
					if (!ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(Guid))
					{
						return text;
					}
				}
			}
			else if (value is bool)
			{
				return value;
			}
			bool flag = value is long;
			bool flag2 = value is ulong;
			bool flag3 = value is double;
			Type underlyingType = Nullable.GetUnderlyingType(type);
			if (underlyingType != null && PlayFabSimpleJson.NumberTypes.IndexOf(underlyingType) != -1)
			{
				type = underlyingType;
			}
			bool flag4 = PlayFabSimpleJson.NumberTypes.IndexOf(type) != -1;
			bool isEnum = type.GetTypeInfo().IsEnum;
			if ((flag && type == typeof(long)) || (flag2 && type == typeof(ulong)) || (flag3 && type == typeof(double)))
			{
				return value;
			}
			if ((flag || flag2 || flag3) && isEnum)
			{
				return Enum.ToObject(type, Convert.ChangeType(value, Enum.GetUnderlyingType(type), CultureInfo.InvariantCulture));
			}
			if ((flag || flag2 || flag3) && flag4)
			{
				return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
			}
			IDictionary<string, object> dictionary = value as IDictionary<string, object>;
			if (dictionary != null)
			{
				IDictionary<string, object> dictionary2 = dictionary;
				if (ReflectionUtils.IsTypeDictionary(type))
				{
					Type[] genericTypeArguments = ReflectionUtils.GetGenericTypeArguments(type);
					Type type2 = genericTypeArguments[0];
					Type type3 = genericTypeArguments[1];
					Type key = typeof(Dictionary<, >).MakeGenericType(new Type[]
					{
						type2,
						type3
					});
					IDictionary dictionary3 = (IDictionary)this.ConstructorCache[key](Array.Empty<object>());
					foreach (KeyValuePair<string, object> keyValuePair in dictionary2)
					{
						dictionary3.Add(keyValuePair.Key, this.DeserializeObject(keyValuePair.Value, type3));
					}
					obj = dictionary3;
				}
				else
				{
					if (!(type == typeof(object)))
					{
						obj = this.ConstructorCache[type](Array.Empty<object>());
						using (IEnumerator<KeyValuePair<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>> enumerator2 = this.SetCache[type].GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								KeyValuePair<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>> keyValuePair2 = enumerator2.Current;
								object value2;
								if (dictionary2.TryGetValue(keyValuePair2.Key, out value2))
								{
									value2 = this.DeserializeObject(value2, keyValuePair2.Value.Key);
									keyValuePair2.Value.Value(obj, value2);
								}
							}
							goto IL_5B5;
						}
						goto IL_443;
					}
					obj = value;
				}
				IL_5B5:
				if (ReflectionUtils.IsNullableType(type))
				{
					return ReflectionUtils.ToNullableType(obj, type);
				}
				return obj;
			}
			IL_443:
			IList<object> list = value as IList<object>;
			if (list != null)
			{
				IList<object> list2 = list;
				IList list3 = null;
				if (type.IsArray)
				{
					list3 = (IList)this.ConstructorCache[type](new object[]
					{
						list2.Count
					});
					int num = 0;
					using (IEnumerator<object> enumerator3 = list2.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							object value3 = enumerator3.Current;
							list3[num++] = this.DeserializeObject(value3, type.GetElementType());
						}
						goto IL_5B0;
					}
				}
				if (ReflectionUtils.IsTypeGenericeCollectionInterface(type) || ReflectionUtils.IsAssignableFrom(typeof(IList), type) || type == typeof(object))
				{
					Type genericListElementType = ReflectionUtils.GetGenericListElementType(type);
					ReflectionUtils.ConstructorDelegate constructorDelegate = null;
					if (type != typeof(object))
					{
						constructorDelegate = this.ConstructorCache[type];
					}
					if (constructorDelegate == null)
					{
						constructorDelegate = this.ConstructorCache[typeof(List<>).MakeGenericType(new Type[]
						{
							genericListElementType
						})];
					}
					list3 = (IList)constructorDelegate(Array.Empty<object>());
					foreach (object value4 in list2)
					{
						list3.Add(this.DeserializeObject(value4, genericListElementType));
					}
				}
				IL_5B0:
				obj = list3;
			}
			return obj;
		}

		protected virtual object SerializeEnum(Enum p)
		{
			return Convert.ToDouble(p, CultureInfo.InvariantCulture);
		}

		protected virtual bool TrySerializeKnownTypes(object input, out object output)
		{
			bool result = true;
			if (input is DateTime)
			{
				output = ((DateTime)input).ToUniversalTime().ToString(PocoJsonSerializerStrategy.Iso8601Format[0], CultureInfo.InvariantCulture);
			}
			else if (input is DateTimeOffset)
			{
				output = ((DateTimeOffset)input).ToUniversalTime().ToString(PocoJsonSerializerStrategy.Iso8601Format[0], CultureInfo.InvariantCulture);
			}
			else if (input is Guid)
			{
				output = ((Guid)input).ToString("D");
			}
			else if (input is Uri)
			{
				output = input.ToString();
			}
			else
			{
				Enum @enum = input as Enum;
				if (@enum != null)
				{
					output = this.SerializeEnum(@enum);
				}
				else
				{
					result = false;
					output = null;
				}
			}
			return result;
		}

		protected virtual bool TrySerializeUnknownTypes(object input, out object output)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			output = null;
			Type type = input.GetType();
			if (type.FullName == null)
			{
				return false;
			}
			IDictionary<string, object> dictionary = new JsonObject();
			foreach (KeyValuePair<MemberInfo, ReflectionUtils.GetDelegate> keyValuePair in this.GetCache[type])
			{
				if (keyValuePair.Value != null)
				{
					string text;
					JsonProperty jsonProperty;
					this.MapClrMemberNameToJsonFieldName(keyValuePair.Key, out text, out jsonProperty);
					if (dictionary.ContainsKey(text))
					{
						throw new Exception("The given key is defined multiple times in the same type: " + input.GetType().Name + "." + text);
					}
					object obj = keyValuePair.Value(input);
					if (jsonProperty == null || jsonProperty.NullValueHandling == NullValueHandling.Include || obj != null)
					{
						dictionary.Add(text, obj);
					}
				}
			}
			output = dictionary;
			return true;
		}

		internal IDictionary<Type, ReflectionUtils.ConstructorDelegate> ConstructorCache;

		internal IDictionary<Type, IDictionary<MemberInfo, ReflectionUtils.GetDelegate>> GetCache;

		internal IDictionary<Type, IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>> SetCache;

		internal static readonly Type[] EmptyTypes = new Type[0];

		internal static readonly Type[] ArrayConstructorParameterTypes = new Type[]
		{
			typeof(int)
		};

		private static readonly string[] Iso8601Format = new string[]
		{
			"yyyy-MM-dd\\THH:mm:ss.FFFFFFF\\Z",
			"yyyy-MM-dd\\THH:mm:ss\\Z",
			"yyyy-MM-dd\\THH:mm:ssK"
		};
	}
}
