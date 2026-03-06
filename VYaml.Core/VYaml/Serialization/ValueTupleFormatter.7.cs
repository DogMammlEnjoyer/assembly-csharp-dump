using System;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	[NullableContext(2)]
	[Nullable(0)]
	public class ValueTupleFormatter<T1, T2, T3, T4, T5, T6, T7> : IYamlFormatter<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, [Nullable(new byte[]
		{
			0,
			1,
			1,
			1,
			1,
			1,
			1,
			1
		})] ValueTuple<T1, T2, T3, T4, T5, T6, T7> value, YamlSerializationContext context)
		{
			emitter.BeginSequence(SequenceStyle.Flow);
			context.Serialize<T1>(ref emitter, value.Item1);
			context.Serialize<T2>(ref emitter, value.Item2);
			context.Serialize<T3>(ref emitter, value.Item3);
			context.Serialize<T4>(ref emitter, value.Item4);
			context.Serialize<T5>(ref emitter, value.Item5);
			context.Serialize<T6>(ref emitter, value.Item6);
			context.Serialize<T7>(ref emitter, value.Item7);
			emitter.EndSequence();
		}

		[NullableContext(1)]
		[return: Nullable(new byte[]
		{
			0,
			1,
			1,
			1,
			1,
			1,
			1,
			1
		})]
		public ValueTuple<T1, T2, T3, T4, T5, T6, T7> Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			if (parser.IsNullScalar())
			{
				return default(ValueTuple<T1, T2, T3, T4, T5, T6, T7>);
			}
			parser.ReadWithVerify(ParseEventType.SequenceStart);
			T1 item = context.DeserializeWithAlias<T1>(ref parser);
			T2 item2 = context.DeserializeWithAlias<T2>(ref parser);
			T3 item3 = context.DeserializeWithAlias<T3>(ref parser);
			T4 item4 = context.DeserializeWithAlias<T4>(ref parser);
			T5 item5 = context.DeserializeWithAlias<T5>(ref parser);
			T6 item6 = context.DeserializeWithAlias<T6>(ref parser);
			T7 item7 = context.DeserializeWithAlias<T7>(ref parser);
			parser.ReadWithVerify(ParseEventType.SequenceEnd);
			return new ValueTuple<T1, T2, T3, T4, T5, T6, T7>(item, item2, item3, item4, item5, item6, item7);
		}
	}
}
