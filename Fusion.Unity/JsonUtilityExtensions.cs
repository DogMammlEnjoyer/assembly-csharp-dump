using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Fusion
{
	public static class JsonUtilityExtensions
	{
		public static string EnquoteIntegers(string json, int minDigits = 8)
		{
			return Regex.Replace(json, string.Format("(?<=\":\\s*)(-?[0-9]{{{0},}})(?=[,}}\\n\\r\\s])", minDigits), "\"$1\"", RegexOptions.Compiled);
		}

		public static string ToJsonWithTypeAnnotation(object obj, JsonUtilityExtensions.InstanceIDHandlerDelegate instanceIDHandler = null)
		{
			StringBuilder stringBuilder = new StringBuilder(1000);
			using (StringWriter stringWriter = new StringWriter(stringBuilder))
			{
				JsonUtilityExtensions.ToJsonWithTypeAnnotation(obj, stringWriter, null, null, instanceIDHandler);
			}
			return stringBuilder.ToString();
		}

		public static void ToJsonWithTypeAnnotation(object obj, TextWriter writer, int? integerEnquoteMinDigits = null, JsonUtilityExtensions.TypeSerializerDelegate typeSerializer = null, JsonUtilityExtensions.InstanceIDHandlerDelegate instanceIDHandler = null)
		{
			if (obj == null)
			{
				writer.Write("null");
				return;
			}
			IList list = obj as IList;
			if (list != null)
			{
				writer.Write("[");
				for (int i = 0; i < list.Count; i++)
				{
					if (i > 0)
					{
						writer.Write(",");
					}
					JsonUtilityExtensions.ToJsonInternal(list[i], writer, integerEnquoteMinDigits, typeSerializer, instanceIDHandler);
				}
				writer.Write("]");
				return;
			}
			JsonUtilityExtensions.ToJsonInternal(obj, writer, integerEnquoteMinDigits, typeSerializer, instanceIDHandler);
		}

		public static T FromJsonWithTypeAnnotation<T>(string json, JsonUtilityExtensions.TypeResolverDelegate typeResolver = null)
		{
			if (typeof(T).IsArray)
			{
				IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[]
				{
					typeof(T).GetElementType()
				}));
				JsonUtilityExtensions.FromJsonWithTypeAnnotationInternal(json, typeResolver, list);
				Array array = Array.CreateInstance(typeof(T).GetElementType(), list.Count);
				list.CopyTo(array, 0);
				return (T)((object)array);
			}
			if (typeof(T).GetInterface(typeof(IList).FullName) != null)
			{
				IList list2 = (IList)Activator.CreateInstance(typeof(T));
				JsonUtilityExtensions.FromJsonWithTypeAnnotationInternal(json, typeResolver, list2);
				return (T)((object)list2);
			}
			return (T)((object)JsonUtilityExtensions.FromJsonWithTypeAnnotationInternal(json, typeResolver, null));
		}

		public static object FromJsonWithTypeAnnotation(string json, JsonUtilityExtensions.TypeResolverDelegate typeResolver = null)
		{
			JsonUtilityExtensions.<>c__DisplayClass8_0 CS$<>8__locals1;
			CS$<>8__locals1.json = json;
			int num = JsonUtilityExtensions.<FromJsonWithTypeAnnotation>g__SkipWhiteOrThrow|8_0(0, ref CS$<>8__locals1);
			if (CS$<>8__locals1.json[num] == '[')
			{
				List<object> list = new List<object>();
				num++;
				bool flag = false;
				for (;;)
				{
					num = JsonUtilityExtensions.<FromJsonWithTypeAnnotation>g__SkipWhiteOrThrow|8_0(num, ref CS$<>8__locals1);
					if (CS$<>8__locals1.json[num] == ']')
					{
						goto IL_96;
					}
					if (flag)
					{
						if (CS$<>8__locals1.json[num] != ',')
						{
							break;
						}
						num = JsonUtilityExtensions.<FromJsonWithTypeAnnotation>g__SkipWhiteOrThrow|8_0(num + 1, ref CS$<>8__locals1);
					}
					object item = JsonUtilityExtensions.FromJsonWithTypeAnnotationToObject(ref num, CS$<>8__locals1.json, typeResolver);
					list.Add(item);
					flag = true;
				}
				throw new InvalidOperationException(string.Format("Malformed at {0}: expected ,", num));
				IL_96:
				return list.ToArray();
			}
			return JsonUtilityExtensions.FromJsonWithTypeAnnotationToObject(ref num, CS$<>8__locals1.json, typeResolver);
		}

		private static object FromJsonWithTypeAnnotationInternal(string json, JsonUtilityExtensions.TypeResolverDelegate typeResolver = null, IList targetList = null)
		{
			JsonUtilityExtensions.<>c__DisplayClass9_0 CS$<>8__locals1;
			CS$<>8__locals1.json = json;
			int num = JsonUtilityExtensions.<FromJsonWithTypeAnnotationInternal>g__SkipWhiteOrThrow|9_0(0, ref CS$<>8__locals1);
			if (CS$<>8__locals1.json[num] == '[')
			{
				IList list = targetList ?? new List<object>();
				num++;
				bool flag = false;
				for (;;)
				{
					num = JsonUtilityExtensions.<FromJsonWithTypeAnnotationInternal>g__SkipWhiteOrThrow|9_0(num, ref CS$<>8__locals1);
					if (CS$<>8__locals1.json[num] == ']')
					{
						goto IL_9F;
					}
					if (flag)
					{
						if (CS$<>8__locals1.json[num] != ',')
						{
							break;
						}
						num = JsonUtilityExtensions.<FromJsonWithTypeAnnotationInternal>g__SkipWhiteOrThrow|9_0(num + 1, ref CS$<>8__locals1);
					}
					object value = JsonUtilityExtensions.FromJsonWithTypeAnnotationToObject(ref num, CS$<>8__locals1.json, typeResolver);
					list.Add(value);
					flag = true;
				}
				throw new InvalidOperationException(string.Format("Malformed at {0}: expected ,", num));
				IL_9F:
				return targetList ?? ((List<object>)list).ToArray();
			}
			if (targetList != null)
			{
				throw new InvalidOperationException(string.Format("Expected list, got {0}", CS$<>8__locals1.json[num]));
			}
			return JsonUtilityExtensions.FromJsonWithTypeAnnotationToObject(ref num, CS$<>8__locals1.json, typeResolver);
		}

		private static void ToJsonInternal(object obj, TextWriter writer, int? integerEnquoteMinDigits = null, JsonUtilityExtensions.TypeSerializerDelegate typeResolver = null, JsonUtilityExtensions.InstanceIDHandlerDelegate instanceIDHandler = null)
		{
			string text = JsonUtility.ToJson(obj);
			if (integerEnquoteMinDigits != null)
			{
				text = JsonUtilityExtensions.EnquoteIntegers(text, integerEnquoteMinDigits.Value);
			}
			Type type = obj.GetType();
			writer.Write("{\"");
			writer.Write("$type");
			writer.Write("\":\"");
			writer.Write(((typeResolver != null) ? typeResolver(type) : null) ?? SerializableType.GetShortAssemblyQualifiedName(type));
			writer.Write('"');
			if (text == "{}")
			{
				writer.Write("}");
				return;
			}
			writer.Write(',');
			if (instanceIDHandler != null)
			{
				int num = 1;
				for (;;)
				{
					int num2 = text.IndexOf("{\"instanceID\":", num, StringComparison.Ordinal);
					if (num2 < 0)
					{
						break;
					}
					int num3 = num2 + "{\"instanceID\":".Length;
					int num4 = text.IndexOf('}', num3);
					int value = int.Parse(text.AsSpan(num3, num4 - num3), NumberStyles.Integer, null);
					writer.Write(text.AsSpan(num, num2 - num));
					writer.Write(instanceIDHandler(obj, value));
					num = num4 + 1;
				}
				writer.Write(text.AsSpan(num, text.Length - num));
				return;
			}
			writer.Write(text.AsSpan(1, text.Length - 1));
		}

		private static object FromJsonWithTypeAnnotationToObject(ref int i, string json, JsonUtilityExtensions.TypeResolverDelegate typeResolver)
		{
			if (json[i] == '{')
			{
				int num = JsonUtilityExtensions.FindScopeEnd(json, i, '{', '}');
				if (num < 0)
				{
					throw new InvalidOperationException(string.Format("Unable to find end of object's end (starting at {0})", i));
				}
				string text = json.Substring(i, num - i + 1);
				i = num + 1;
				JsonUtilityExtensions.TypeNameWrapper typeNameWrapper = JsonUtility.FromJson<JsonUtilityExtensions.TypeNameWrapper>(text.Replace("$type", "__TypeName", StringComparison.Ordinal));
				Type type;
				if (typeResolver != null)
				{
					type = typeResolver(typeNameWrapper.__TypeName);
					if (type == null)
					{
						return null;
					}
				}
				else
				{
					type = Type.GetType(typeNameWrapper.__TypeName, true);
				}
				if (type.IsSubclassOf(typeof(ScriptableObject)))
				{
					ScriptableObject scriptableObject = ScriptableObject.CreateInstance(type);
					JsonUtility.FromJsonOverwrite(text, scriptableObject);
					return scriptableObject;
				}
				return JsonUtility.FromJson(text, type);
			}
			else
			{
				if (i + 4 < json.Length && json.AsSpan(i, 4).SequenceEqual("null"))
				{
					i += 4;
					return null;
				}
				throw new InvalidOperationException(string.Format("Malformed at {0}: expected {{ or null", i));
			}
		}

		internal static int FindObjectEnd(string json, int start = 0)
		{
			return JsonUtilityExtensions.FindScopeEnd(json, start, '{', '}');
		}

		private static int FindScopeEnd(string json, int start, char cstart = '{', char cend = '}')
		{
			int num = 0;
			if (json[start] != cstart)
			{
				return -1;
			}
			for (int i = start; i < json.Length; i++)
			{
				if (json[i] == '"')
				{
					while (i < json.Length)
					{
						if (json[++i] == '"' && json[i - 1] != '\\')
						{
							break;
						}
					}
				}
				else if (json[i] == cstart)
				{
					num++;
				}
				else if (json[i] == cend)
				{
					num--;
					if (num == 0)
					{
						return i;
					}
				}
			}
			return -1;
		}

		[CompilerGenerated]
		internal static int <FromJsonWithTypeAnnotation>g__SkipWhiteOrThrow|8_0(int i, ref JsonUtilityExtensions.<>c__DisplayClass8_0 A_1)
		{
			while (i < A_1.json.Length && char.IsWhiteSpace(A_1.json[i]))
			{
				i++;
			}
			if (i == A_1.json.Length)
			{
				throw new InvalidOperationException(string.Format("Malformed at {0}: expected more", i));
			}
			return i;
		}

		[CompilerGenerated]
		internal static int <FromJsonWithTypeAnnotationInternal>g__SkipWhiteOrThrow|9_0(int i, ref JsonUtilityExtensions.<>c__DisplayClass9_0 A_1)
		{
			while (i < A_1.json.Length && char.IsWhiteSpace(A_1.json[i]))
			{
				i++;
			}
			if (i == A_1.json.Length)
			{
				throw new InvalidOperationException(string.Format("Malformed at {0}: expected more", i));
			}
			return i;
		}

		private const string TypePropertyName = "$type";

		public delegate Type TypeResolverDelegate(string typeName);

		public delegate string TypeSerializerDelegate(Type type);

		public delegate string InstanceIDHandlerDelegate(object context, int value);

		[Serializable]
		private class TypeNameWrapper
		{
			public string __TypeName;
		}
	}
}
