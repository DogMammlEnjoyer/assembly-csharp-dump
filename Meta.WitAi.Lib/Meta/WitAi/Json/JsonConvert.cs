using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Scripting;

namespace Meta.WitAi.Json
{
	public static class JsonConvert
	{
		public static JsonConverter[] DefaultConverters
		{
			get
			{
				return JsonConvert._defaultConverters;
			}
		}

		private static object EnsureExists(Type objType, object obj)
		{
			if (obj != null || !(objType != null))
			{
				return obj;
			}
			if (objType == typeof(string))
			{
				return string.Empty;
			}
			if (objType.IsArray)
			{
				return Activator.CreateInstance(objType, new object[]
				{
					0
				});
			}
			return Activator.CreateInstance(objType);
		}

		public static WitResponseNode DeserializeToken(string jsonString)
		{
			if (string.IsNullOrEmpty(jsonString))
			{
				VLog.W("Parse Failed\nNo content provided", null);
				return null;
			}
			WitResponseNode result;
			try
			{
				result = WitResponseNode.Parse(jsonString);
			}
			catch (Exception e)
			{
				VLog.W("Parse Failed\n\n" + jsonString, e);
				result = null;
			}
			return result;
		}

		public static Task<WitResponseNode> DeserializeTokenAsync(string jsonString)
		{
			JsonConvert.<DeserializeTokenAsync>d__6 <DeserializeTokenAsync>d__;
			<DeserializeTokenAsync>d__.<>t__builder = AsyncTaskMethodBuilder<WitResponseNode>.Create();
			<DeserializeTokenAsync>d__.jsonString = jsonString;
			<DeserializeTokenAsync>d__.<>1__state = -1;
			<DeserializeTokenAsync>d__.<>t__builder.Start<JsonConvert.<DeserializeTokenAsync>d__6>(ref <DeserializeTokenAsync>d__);
			return <DeserializeTokenAsync>d__.<>t__builder.Task;
		}

		public static IN_TYPE DeserializeObject<IN_TYPE>(string jsonString, JsonConverter[] customConverters = null, bool suppressWarnings = false)
		{
			return JsonConvert.DeserializeIntoObject<IN_TYPE>((IN_TYPE)((object)JsonConvert.EnsureExists(typeof(IN_TYPE), null)), jsonString, customConverters, suppressWarnings);
		}

		public static Task<IN_TYPE> DeserializeObjectAsync<IN_TYPE>(string jsonString, JsonConverter[] customConverters = null, bool suppressWarnings = false)
		{
			JsonConvert.<DeserializeObjectAsync>d__8<IN_TYPE> <DeserializeObjectAsync>d__;
			<DeserializeObjectAsync>d__.<>t__builder = AsyncTaskMethodBuilder<IN_TYPE>.Create();
			<DeserializeObjectAsync>d__.jsonString = jsonString;
			<DeserializeObjectAsync>d__.customConverters = customConverters;
			<DeserializeObjectAsync>d__.suppressWarnings = suppressWarnings;
			<DeserializeObjectAsync>d__.<>1__state = -1;
			<DeserializeObjectAsync>d__.<>t__builder.Start<JsonConvert.<DeserializeObjectAsync>d__8<IN_TYPE>>(ref <DeserializeObjectAsync>d__);
			return <DeserializeObjectAsync>d__.<>t__builder.Task;
		}

		public static IN_TYPE DeserializeObject<IN_TYPE>(WitResponseNode jsonToken, JsonConverter[] customConverters = null, bool suppressWarnings = false)
		{
			return JsonConvert.DeserializeIntoObject<IN_TYPE>((IN_TYPE)((object)JsonConvert.EnsureExists(typeof(IN_TYPE), null)), jsonToken, customConverters, suppressWarnings);
		}

		public static Task<IN_TYPE> DeserializeObjectAsync<IN_TYPE>(WitResponseNode jsonToken, JsonConverter[] customConverters = null, bool suppressWarnings = false)
		{
			JsonConvert.<DeserializeObjectAsync>d__10<IN_TYPE> <DeserializeObjectAsync>d__;
			<DeserializeObjectAsync>d__.<>t__builder = AsyncTaskMethodBuilder<IN_TYPE>.Create();
			<DeserializeObjectAsync>d__.jsonToken = jsonToken;
			<DeserializeObjectAsync>d__.customConverters = customConverters;
			<DeserializeObjectAsync>d__.suppressWarnings = suppressWarnings;
			<DeserializeObjectAsync>d__.<>1__state = -1;
			<DeserializeObjectAsync>d__.<>t__builder.Start<JsonConvert.<DeserializeObjectAsync>d__10<IN_TYPE>>(ref <DeserializeObjectAsync>d__);
			return <DeserializeObjectAsync>d__.<>t__builder.Task;
		}

		public static IN_TYPE DeserializeIntoObject<IN_TYPE>(IN_TYPE instance, string jsonString, JsonConverter[] customConverters = null, bool suppressWarnings = false)
		{
			return JsonConvert.DeserializeIntoObject<IN_TYPE>(instance, JsonConvert.DeserializeToken(jsonString), customConverters, suppressWarnings);
		}

		public static Task<IN_TYPE> DeserializeIntoObjectAsync<IN_TYPE>(IN_TYPE instance, string jsonString, JsonConverter[] customConverters = null, bool suppressWarnings = false)
		{
			JsonConvert.<DeserializeIntoObjectAsync>d__12<IN_TYPE> <DeserializeIntoObjectAsync>d__;
			<DeserializeIntoObjectAsync>d__.<>t__builder = AsyncTaskMethodBuilder<IN_TYPE>.Create();
			<DeserializeIntoObjectAsync>d__.instance = instance;
			<DeserializeIntoObjectAsync>d__.jsonString = jsonString;
			<DeserializeIntoObjectAsync>d__.customConverters = customConverters;
			<DeserializeIntoObjectAsync>d__.suppressWarnings = suppressWarnings;
			<DeserializeIntoObjectAsync>d__.<>1__state = -1;
			<DeserializeIntoObjectAsync>d__.<>t__builder.Start<JsonConvert.<DeserializeIntoObjectAsync>d__12<IN_TYPE>>(ref <DeserializeIntoObjectAsync>d__);
			return <DeserializeIntoObjectAsync>d__.<>t__builder.Task;
		}

		public static IN_TYPE DeserializeIntoObject<IN_TYPE>(IN_TYPE instance, WitResponseNode jsonToken, JsonConverter[] customConverters = null, bool suppressWarnings = false)
		{
			IN_TYPE result;
			try
			{
				if (jsonToken == null)
				{
					result = instance;
				}
				else
				{
					if (customConverters == null)
					{
						customConverters = JsonConvert.DefaultConverters;
					}
					Type typeFromHandle = typeof(IN_TYPE);
					if (typeFromHandle == typeof(WitResponseNode))
					{
						result = (IN_TYPE)((object)jsonToken);
					}
					else if (typeFromHandle == typeof(WitResponseClass))
					{
						result = (IN_TYPE)((object)jsonToken.AsObject);
					}
					else if (typeFromHandle == typeof(WitResponseArray))
					{
						result = (IN_TYPE)((object)jsonToken.AsArray);
					}
					else
					{
						StringBuilder stringBuilder = new StringBuilder();
						IN_TYPE in_TYPE = (IN_TYPE)((object)JsonConvert.DeserializeToken(typeFromHandle, instance, jsonToken, stringBuilder, customConverters));
						if (stringBuilder.Length > 0 && !suppressWarnings)
						{
							VLog.D(string.Format("Deserialize Warnings\n{0}", stringBuilder));
						}
						result = in_TYPE;
					}
				}
			}
			catch (Exception e)
			{
				VLog.E(string.Format("Deserialize Failed\nTo: {0}", typeof(IN_TYPE)), e);
				result = instance;
			}
			return result;
		}

		public static Task<IN_TYPE> DeserializeIntoObjectAsync<IN_TYPE>(IN_TYPE instance, WitResponseNode jsonToken, JsonConverter[] customConverters = null, bool suppressWarnings = false)
		{
			JsonConvert.<DeserializeIntoObjectAsync>d__14<IN_TYPE> <DeserializeIntoObjectAsync>d__;
			<DeserializeIntoObjectAsync>d__.<>t__builder = AsyncTaskMethodBuilder<IN_TYPE>.Create();
			<DeserializeIntoObjectAsync>d__.instance = instance;
			<DeserializeIntoObjectAsync>d__.jsonToken = jsonToken;
			<DeserializeIntoObjectAsync>d__.customConverters = customConverters;
			<DeserializeIntoObjectAsync>d__.suppressWarnings = suppressWarnings;
			<DeserializeIntoObjectAsync>d__.<>1__state = -1;
			<DeserializeIntoObjectAsync>d__.<>t__builder.Start<JsonConvert.<DeserializeIntoObjectAsync>d__14<IN_TYPE>>(ref <DeserializeIntoObjectAsync>d__);
			return <DeserializeIntoObjectAsync>d__.<>t__builder.Task;
		}

		private static object DeserializeToken(Type toType, object oldValue, WitResponseNode jsonToken, StringBuilder log, JsonConverter[] customConverters)
		{
			if (customConverters != null)
			{
				foreach (JsonConverter jsonConverter in customConverters)
				{
					if (jsonConverter.CanRead && jsonConverter.CanConvert(toType))
					{
						return jsonConverter.ReadJson(jsonToken, toType, oldValue);
					}
				}
			}
			if (toType == typeof(string))
			{
				return jsonToken.Value;
			}
			if (toType.IsEnum)
			{
				string enumString = jsonToken.Value;
				foreach (object obj in Enum.GetValues(toType))
				{
					foreach (JsonPropertyAttribute jsonPropertyAttribute in toType.GetMember(obj.ToString())[0].GetCustomAttributes(typeof(JsonPropertyAttribute), false))
					{
						if (!string.IsNullOrEmpty(jsonPropertyAttribute.PropertyName) && string.Equals(jsonToken.Value, jsonPropertyAttribute.PropertyName, StringComparison.CurrentCultureIgnoreCase))
						{
							enumString = obj.ToString();
							break;
						}
					}
				}
				return JsonConvert.DeserializeEnum(toType, JsonConvert.EnsureExists(toType, oldValue), enumString, log);
			}
			if (toType.GetInterfaces().Contains(typeof(IDictionary)))
			{
				return JsonConvert.DeserializeDictionary(toType, JsonConvert.EnsureExists(toType, oldValue), jsonToken.AsObject, log, customConverters);
			}
			if (toType.GetInterfaces().Contains(typeof(IEnumerable)))
			{
				Type type = toType.GetElementType();
				if (type == null)
				{
					Type[] genericArguments = toType.GetGenericArguments();
					if (genericArguments != null && genericArguments.Length != 0)
					{
						type = genericArguments[0];
					}
				}
				if (type != null)
				{
					object obj2 = typeof(JsonConvert).GetMethod("DeserializeArray", BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(new Type[]
					{
						type
					}).Invoke(null, new object[]
					{
						oldValue,
						jsonToken,
						log,
						customConverters
					});
					if (toType.IsArray)
					{
						return obj2;
					}
					if (toType.GetInterfaces().Contains(typeof(IList)))
					{
						return Activator.CreateInstance(toType, new object[]
						{
							obj2
						});
					}
				}
			}
			if (toType.IsClass)
			{
				return JsonConvert.DeserializeClass(toType, oldValue, jsonToken.AsObject, log, customConverters);
			}
			if (toType.IsValueType && !toType.IsPrimitive)
			{
				object oldObject = Activator.CreateInstance(toType);
				return JsonConvert.DeserializeClass(toType, oldObject, jsonToken.AsObject, log, customConverters);
			}
			object result;
			try
			{
				result = Convert.ChangeType(jsonToken.Value, toType);
			}
			catch (Exception arg)
			{
				log.AppendLine(string.Format("\nJson Deserializer failed to cast '{0}' to type '{1}'\n{2}", jsonToken.Value, toType, arg));
				result = oldValue;
			}
			return result;
		}

		private static object DeserializeEnum(Type toType, object oldValue, string enumString, StringBuilder log)
		{
			if (JsonConvert._enumParseMethod == null)
			{
				JsonConvert._enumParseMethod = typeof(Enum).GetMethods().ToList<MethodInfo>().Find((MethodInfo method) => method.IsGenericMethod && method.GetParameters().Length == 3 && string.Equals(method.Name, "TryParse"));
			}
			MethodInfo methodInfo = JsonConvert._enumParseMethod.MakeGenericMethod(new Type[]
			{
				toType
			});
			object[] array = new object[]
			{
				enumString,
				false,
				Activator.CreateInstance(toType)
			};
			if ((bool)methodInfo.Invoke(null, array))
			{
				return array[2];
			}
			log.AppendLine(string.Format("\nJson Deserializer Failed to cast '{0}' to enum type '{1}'", enumString, toType));
			return oldValue;
		}

		[Preserve]
		public static ITEM_TYPE[] DeserializeArray<ITEM_TYPE>(object oldArray, WitResponseNode jsonToken, StringBuilder log, JsonConverter[] customConverters)
		{
			if (jsonToken == null)
			{
				return (ITEM_TYPE[])oldArray;
			}
			WitResponseArray asArray = jsonToken.AsArray;
			ITEM_TYPE[] array = new ITEM_TYPE[asArray.Count];
			Type typeFromHandle = typeof(ITEM_TYPE);
			for (int i = 0; i < asArray.Count; i++)
			{
				object oldValue = JsonConvert.EnsureExists(typeFromHandle, null);
				ITEM_TYPE item_TYPE = (ITEM_TYPE)((object)JsonConvert.DeserializeToken(typeFromHandle, oldValue, asArray[i], log, customConverters));
				array[i] = item_TYPE;
			}
			return array;
		}

		private static object DeserializeClass(Type toType, object oldObject, WitResponseClass jsonClass, StringBuilder log, JsonConverter[] customConverters)
		{
			if (jsonClass == null)
			{
				return oldObject;
			}
			object obj = oldObject;
			if (obj == null)
			{
				obj = Activator.CreateInstance(toType);
			}
			Dictionary<string, IJsonVariableInfo> varDictionary = JsonConvert.GetVarDictionary(toType, log);
			foreach (string text in jsonClass.ChildNodeNames)
			{
				if (!varDictionary.ContainsKey(text))
				{
					log.AppendLine(string.Concat(new string[]
					{
						"\t",
						toType.FullName,
						" does not have a matching '",
						text,
						"' field or property."
					}));
				}
				else
				{
					IJsonVariableInfo jsonVariableInfo = varDictionary[text];
					if (!jsonVariableInfo.GetShouldDeserialize())
					{
						log.AppendLine(string.Concat(new string[]
						{
							"\t",
							toType.FullName,
							" cannot deserialize '",
							text,
							"' to the matching ",
							(jsonVariableInfo is JsonPropertyInfo) ? "property" : "field",
							"."
						}));
					}
					else
					{
						object oldValue = jsonVariableInfo.GetShouldSerialize() ? jsonVariableInfo.GetValue(obj) : null;
						object newValue = JsonConvert.DeserializeToken(jsonVariableInfo.GetVariableType(), oldValue, jsonClass[text], log, customConverters);
						jsonVariableInfo.SetValue(obj, newValue);
					}
				}
			}
			if (toType.GetInterfaces().Contains(typeof(IJsonDeserializer)) && !(obj as IJsonDeserializer).DeserializeObject(jsonClass))
			{
				log.AppendLine(string.Format("\tIJsonDeserializer '{0}' failed", toType));
			}
			return obj;
		}

		private static object DeserializeDictionary(Type toType, object oldObject, WitResponseClass jsonClass, StringBuilder log, JsonConverter[] customConverters)
		{
			Type[] genericArguments = toType.GetGenericArguments();
			if (genericArguments == null || genericArguments.Length != 2)
			{
				return oldObject;
			}
			IDictionary dictionary = oldObject as IDictionary;
			Type conversionType = genericArguments[0];
			Type toType2 = genericArguments[1];
			foreach (string text in jsonClass.ChildNodeNames)
			{
				object key = Convert.ChangeType(text, conversionType);
				object value = JsonConvert.DeserializeToken(toType2, null, jsonClass[text], log, customConverters);
				dictionary[key] = value;
			}
			return dictionary;
		}

		public static string SerializeObject<TFromType>(TFromType inObject, JsonConverter[] customConverters = null, bool suppressWarnings = false)
		{
			WitResponseNode witResponseNode = JsonConvert.SerializeToken<TFromType>(inObject, customConverters, suppressWarnings);
			if (witResponseNode != null)
			{
				try
				{
					return witResponseNode.ToString();
				}
				catch (Exception e)
				{
					VLog.E("Serialize Object Failed", e);
				}
			}
			return "{}";
		}

		public static Task<string> SerializeObjectAsync<TFromType>(TFromType inObject, JsonConverter[] customConverters = null, bool suppressWarnings = false)
		{
			JsonConvert.<SerializeObjectAsync>d__22<TFromType> <SerializeObjectAsync>d__;
			<SerializeObjectAsync>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<SerializeObjectAsync>d__.inObject = inObject;
			<SerializeObjectAsync>d__.customConverters = customConverters;
			<SerializeObjectAsync>d__.suppressWarnings = suppressWarnings;
			<SerializeObjectAsync>d__.<>1__state = -1;
			<SerializeObjectAsync>d__.<>t__builder.Start<JsonConvert.<SerializeObjectAsync>d__22<TFromType>>(ref <SerializeObjectAsync>d__);
			return <SerializeObjectAsync>d__.<>t__builder.Task;
		}

		public static WitResponseNode SerializeToken<TFromType>(TFromType inObject, JsonConverter[] customConverters = null, bool suppressWarnings = false)
		{
			WitResponseNode witResponseNode = inObject as WitResponseNode;
			if (witResponseNode != null)
			{
				return witResponseNode;
			}
			if (customConverters == null)
			{
				customConverters = JsonConvert.DefaultConverters;
			}
			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				WitResponseNode result = JsonConvert.SerializeToken(typeof(TFromType), inObject, stringBuilder, customConverters);
				if (stringBuilder.Length > 0 && !suppressWarnings)
				{
					VLog.W(string.Format("Serialize Token Warnings\n{0}", stringBuilder), null);
				}
				return result;
			}
			catch (Exception e)
			{
				VLog.E(string.Format("Serialize Token Failed for {0}\n{1}", inObject.GetType().Name, inObject), e);
			}
			return null;
		}

		public static Task<WitResponseNode> SerializeTokenAsync<TFromType>(TFromType inObject, JsonConverter[] customConverters = null, bool suppressWarnings = false)
		{
			JsonConvert.<SerializeTokenAsync>d__24<TFromType> <SerializeTokenAsync>d__;
			<SerializeTokenAsync>d__.<>t__builder = AsyncTaskMethodBuilder<WitResponseNode>.Create();
			<SerializeTokenAsync>d__.inObject = inObject;
			<SerializeTokenAsync>d__.customConverters = customConverters;
			<SerializeTokenAsync>d__.suppressWarnings = suppressWarnings;
			<SerializeTokenAsync>d__.<>1__state = -1;
			<SerializeTokenAsync>d__.<>t__builder.Start<JsonConvert.<SerializeTokenAsync>d__24<TFromType>>(ref <SerializeTokenAsync>d__);
			return <SerializeTokenAsync>d__.<>t__builder.Task;
		}

		private static WitResponseNode SerializeToken(Type inType, object inObject, StringBuilder log, JsonConverter[] customConverters)
		{
			if (inObject != null && inType == typeof(object))
			{
				inType = inObject.GetType();
			}
			WitResponseNode witResponseNode = inObject as WitResponseNode;
			if (witResponseNode != null)
			{
				return witResponseNode;
			}
			if (customConverters != null)
			{
				foreach (JsonConverter jsonConverter in customConverters)
				{
					if (jsonConverter.CanWrite && jsonConverter.CanConvert(inType))
					{
						return jsonConverter.WriteJson(inObject);
					}
				}
			}
			if (inObject == null)
			{
				return null;
			}
			if (inType == null)
			{
				throw new ArgumentException("In Type cannot be null");
			}
			if (inType == typeof(string))
			{
				return new WitResponseData((string)inObject);
			}
			if (inType == typeof(bool))
			{
				return new WitResponseData((bool)inObject);
			}
			if (inType == typeof(int))
			{
				return new WitResponseData((int)inObject);
			}
			if (inType == typeof(float))
			{
				return new WitResponseData((float)inObject);
			}
			if (inType == typeof(double))
			{
				return new WitResponseData((double)inObject);
			}
			if (inType == typeof(short))
			{
				return new WitResponseData((int)((short)inObject));
			}
			if (inType == typeof(long))
			{
				return new WitResponseData((float)((long)inObject));
			}
			if (inType.IsEnum)
			{
				return new WitResponseData(inObject.ToString());
			}
			if (inType.GetInterfaces().Contains(typeof(IDictionary)))
			{
				IDictionary dictionary = (IDictionary)inObject;
				WitResponseClass witResponseClass = new WitResponseClass();
				Type type = inType.GetGenericArguments()[1];
				foreach (object obj in dictionary.Keys)
				{
					object obj2 = dictionary[obj];
					if (obj2 == null)
					{
						if (type == typeof(string))
						{
							obj2 = string.Empty;
						}
						else
						{
							obj2 = Activator.CreateInstance(type);
						}
					}
					witResponseClass.Add(obj.ToString(), JsonConvert.SerializeToken(type, obj2, log, customConverters));
				}
				return witResponseClass;
			}
			if (inType.GetInterfaces().Contains(typeof(IEnumerable)))
			{
				WitResponseArray witResponseArray = new WitResponseArray();
				IEnumerator enumerator2 = ((IEnumerable)inObject).GetEnumerator();
				Type type2 = inType.GetElementType();
				if (type2 == null)
				{
					Type[] genericArguments = inType.GetGenericArguments();
					if (genericArguments != null && genericArguments.Length != 0)
					{
						type2 = genericArguments[0];
					}
				}
				while (enumerator2.MoveNext())
				{
					object obj3 = enumerator2.Current;
					object inObject2 = JsonConvert.EnsureExists(type2, obj3);
					witResponseArray.Add(string.Empty, JsonConvert.SerializeToken(type2, inObject2, log, customConverters));
				}
				return witResponseArray;
			}
			if (inType.IsClass || (inType.IsValueType && !inType.IsPrimitive))
			{
				return JsonConvert.SerializeClass(inType, inObject, log, customConverters);
			}
			log.AppendLine(string.Format("\tJson Serializer cannot serialize: {0}", inType));
			if (inObject != null)
			{
				return new WitResponseData(inObject.ToString());
			}
			return null;
		}

		private static WitResponseClass SerializeClass(Type inType, object inObject, StringBuilder log, JsonConverter[] customConverters)
		{
			WitResponseClass witResponseClass = new WitResponseClass();
			foreach (IJsonVariableInfo jsonVariableInfo in JsonConvert.GetVarInfos(inType))
			{
				if (jsonVariableInfo.GetShouldSerialize())
				{
					foreach (string text in jsonVariableInfo.GetSerializeNames())
					{
						try
						{
							object value = jsonVariableInfo.GetValue(inObject);
							if (value != null)
							{
								witResponseClass.Add(text, JsonConvert.SerializeToken(jsonVariableInfo.GetVariableType(), value, log, customConverters));
							}
						}
						catch (Exception ex)
						{
							throw new ArgumentException(string.Concat(new string[]
							{
								"Cannot encode '",
								inType.Name,
								".",
								text,
								"': ",
								ex.Message
							}), ex);
						}
					}
				}
			}
			return witResponseClass;
		}

		private static List<IJsonVariableInfo> GetVarInfos(Type forType)
		{
			List<IJsonVariableInfo> list = new List<IJsonVariableInfo>();
			FieldInfo[] fields = forType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < fields.Length; i++)
			{
				JsonFieldInfo jsonFieldInfo = new JsonFieldInfo(fields[i]);
				if (jsonFieldInfo.GetShouldSerialize() || jsonFieldInfo.GetShouldDeserialize())
				{
					list.Add(jsonFieldInfo);
				}
			}
			PropertyInfo[] properties = forType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < properties.Length; i++)
			{
				JsonPropertyInfo jsonPropertyInfo = new JsonPropertyInfo(properties[i]);
				if (jsonPropertyInfo.GetShouldSerialize() || jsonPropertyInfo.GetShouldDeserialize())
				{
					list.Add(jsonPropertyInfo);
				}
			}
			return list;
		}

		private static Dictionary<string, IJsonVariableInfo> GetVarDictionary(Type forType, StringBuilder log)
		{
			Dictionary<string, IJsonVariableInfo> dictionary = new Dictionary<string, IJsonVariableInfo>();
			foreach (IJsonVariableInfo jsonVariableInfo in JsonConvert.GetVarInfos(forType))
			{
				foreach (string text in jsonVariableInfo.GetSerializeNames())
				{
					if (!string.IsNullOrEmpty(text))
					{
						if (dictionary.ContainsKey(text))
						{
							log.AppendLine(string.Concat(new string[]
							{
								"\t",
								forType.FullName,
								" has two fields/properties with the same name '",
								text,
								"' exposed to JsonConvert."
							}));
						}
						else
						{
							dictionary[text] = jsonVariableInfo;
						}
					}
				}
			}
			return dictionary;
		}

		private static JsonConverter[] _defaultConverters = new JsonConverter[]
		{
			new ColorConverter(),
			new DateTimeConverter(),
			new HashSetConverter<string>()
		};

		private const BindingFlags BIND_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		private static MethodInfo _enumParseMethod;
	}
}
