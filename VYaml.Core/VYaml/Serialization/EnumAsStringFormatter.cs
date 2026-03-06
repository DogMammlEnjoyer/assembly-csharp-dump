using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using VYaml.Annotations;
using VYaml.Emitter;
using VYaml.Internal;
using VYaml.Parser;

namespace VYaml.Serialization
{
	[NullableContext(1)]
	[Nullable(0)]
	public class EnumAsStringFormatter<[Nullable(0)] T> : IYamlFormatter<T>, IYamlFormatter where T : Enum
	{
		static EnumAsStringFormatter()
		{
			List<string> list = new List<string>();
			List<object> list2 = new List<object>();
			Type type = typeof(T);
			YamlObjectAttribute customAttribute = type.GetCustomAttribute<YamlObjectAttribute>();
			NamingConvention namingConvention = (customAttribute != null) ? customAttribute.NamingConvention : NamingConvention.LowerCamelCase;
			IEnumerable<FieldInfo> fields = type.GetFields();
			Func<FieldInfo, bool> <>9__0;
			Func<FieldInfo, bool> predicate;
			if ((predicate = <>9__0) == null)
			{
				predicate = (<>9__0 = ((FieldInfo x) => x.FieldType == type));
			}
			foreach (FieldInfo fieldInfo in fields.Where(predicate))
			{
				object value = fieldInfo.GetValue(null);
				list2.Add(value);
				object[] customAttributes = fieldInfo.GetCustomAttributes(true);
				EnumMemberAttribute enumMemberAttribute = customAttributes.OfType<EnumMemberAttribute>().FirstOrDefault<EnumMemberAttribute>();
				if (enumMemberAttribute != null)
				{
					string value2 = enumMemberAttribute.Value;
					if (value2 != null)
					{
						list.Add(value2);
						continue;
					}
				}
				DataMemberAttribute dataMemberAttribute = customAttributes.OfType<DataMemberAttribute>().FirstOrDefault<DataMemberAttribute>();
				if (dataMemberAttribute != null)
				{
					string name = dataMemberAttribute.Name;
					if (name != null)
					{
						list.Add(name);
						continue;
					}
				}
				string name2 = Enum.GetName(type, value);
				list.Add(KeyNameMutator.Mutate(name2, namingConvention));
			}
			EnumAsStringFormatter<T>.NameValueMapping = new Dictionary<string, T>(list.Count);
			EnumAsStringFormatter<T>.ValueNameMapping = new Dictionary<T, string>(list.Count);
			foreach (ValueTuple<object, string> valueTuple in list2.Zip(list, (object v, string n) => new ValueTuple<object, string>(v, n)))
			{
				object item = valueTuple.Item1;
				string item2 = valueTuple.Item2;
				EnumAsStringFormatter<T>.NameValueMapping[item2] = (T)((object)item);
				EnumAsStringFormatter<T>.ValueNameMapping[(T)((object)item)] = item2;
			}
		}

		public void Serialize(ref Utf8YamlEmitter emitter, T value, YamlSerializationContext context)
		{
			string value2;
			if (EnumAsStringFormatter<T>.ValueNameMapping.TryGetValue(value, out value2))
			{
				emitter.WriteString(value2, ScalarStyle.Plain);
				return;
			}
			YamlSerializerException.ThrowInvalidType<T>(value);
		}

		public T Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			string text = parser.ReadScalarAsString();
			T result;
			if (text == null)
			{
				YamlSerializerException.ThrowInvalidType<T>();
			}
			else if (EnumAsStringFormatter<T>.NameValueMapping.TryGetValue(text, out result))
			{
				return result;
			}
			YamlSerializerException.ThrowInvalidType<T>();
			return default(T);
		}

		private static readonly Dictionary<string, T> NameValueMapping;

		private static readonly Dictionary<T, string> ValueNameMapping;
	}
}
