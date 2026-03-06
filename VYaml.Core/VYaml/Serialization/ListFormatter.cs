using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class ListFormatter<[Nullable(2)] T> : IYamlFormatter<List<T>>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, [Nullable(new byte[]
		{
			2,
			1
		})] List<T> value, YamlSerializationContext context)
		{
			if (value == null)
			{
				emitter.WriteNull();
				return;
			}
			emitter.BeginSequence(SequenceStyle.Block);
			if (value.Count > 0)
			{
				IYamlFormatter<T> formatterWithVerify = context.Resolver.GetFormatterWithVerify<T>();
				foreach (T value2 in value)
				{
					formatterWithVerify.Serialize(ref emitter, value2, context);
				}
			}
			emitter.EndSequence();
		}

		[NullableContext(1)]
		[return: Nullable(new byte[]
		{
			2,
			1
		})]
		public List<T> Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				parser.Read();
				return null;
			}
			parser.ReadWithVerify(ParseEventType.SequenceStart);
			List<T> list = new List<T>();
			IYamlFormatter<T> formatterWithVerify = context.Resolver.GetFormatterWithVerify<T>();
			while (!parser.End && parser.CurrentEventType != ParseEventType.SequenceEnd)
			{
				T item = context.DeserializeWithAlias<T>(formatterWithVerify, ref parser);
				list.Add(item);
			}
			parser.ReadWithVerify(ParseEventType.SequenceEnd);
			return list;
		}
	}
}
