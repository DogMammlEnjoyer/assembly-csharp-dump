using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class InterfaceDictionaryFormatter<[Nullable(2)] TKey, [Nullable(2)] TValue> : IYamlFormatter<IDictionary<TKey, TValue>>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, [Nullable(new byte[]
		{
			2,
			1,
			1
		})] IDictionary<TKey, TValue> value, YamlSerializationContext context)
		{
			if (value == null)
			{
				emitter.WriteNull();
				return;
			}
			emitter.BeginMapping(MappingStyle.Block);
			if (value.Count > 0)
			{
				IYamlFormatter<TKey> formatterWithVerify = context.Resolver.GetFormatterWithVerify<TKey>();
				IYamlFormatter<TValue> formatterWithVerify2 = context.Resolver.GetFormatterWithVerify<TValue>();
				foreach (KeyValuePair<TKey, TValue> keyValuePair in value)
				{
					formatterWithVerify.Serialize(ref emitter, keyValuePair.Key, context);
					formatterWithVerify2.Serialize(ref emitter, keyValuePair.Value, context);
				}
			}
			emitter.EndMapping();
		}

		[NullableContext(1)]
		[return: Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		public IDictionary<TKey, TValue> Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				parser.Read();
				return null;
			}
			parser.ReadWithVerify(ParseEventType.MappingStart);
			Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
			IYamlFormatter<TKey> formatterWithVerify = context.Resolver.GetFormatterWithVerify<TKey>();
			IYamlFormatter<TValue> formatterWithVerify2 = context.Resolver.GetFormatterWithVerify<TValue>();
			while (!parser.End && parser.CurrentEventType != ParseEventType.MappingEnd)
			{
				TKey key = context.DeserializeWithAlias<TKey>(formatterWithVerify, ref parser);
				TValue value = context.DeserializeWithAlias<TValue>(formatterWithVerify2, ref parser);
				dictionary.Add(key, value);
			}
			parser.ReadWithVerify(ParseEventType.MappingEnd);
			return dictionary;
		}
	}
}
