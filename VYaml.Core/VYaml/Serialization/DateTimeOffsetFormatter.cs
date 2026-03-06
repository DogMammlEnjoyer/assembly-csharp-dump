using System;
using System.Buffers;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class DateTimeOffsetFormatter : IYamlFormatter<DateTimeOffset>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, DateTimeOffset value, YamlSerializationContext context)
		{
			byte[] buffer = context.GetBuffer64();
			int value2;
			if (Utf8Formatter.TryFormat(value, buffer, out value2, new StandardFormat('O', 255)))
			{
				emitter.WriteScalar(RuntimeHelpers.GetSubArray<byte>(buffer, Range.EndAt(value2)));
				return;
			}
			throw new YamlSerializerException(string.Format("Cannot format {0}", value));
		}

		[NullableContext(1)]
		public DateTimeOffset Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			ReadOnlySpan<byte> source;
			DateTimeOffset result;
			int num;
			if (parser.TryGetScalarAsSpan(out source) && Utf8Parser.TryParse(source, out result, out num, '\0') && num == source.Length)
			{
				parser.Read();
				return result;
			}
			throw new YamlSerializerException(string.Format("Cannot detect a scalar value of DateTimeOffset : {0} {1}", parser.CurrentEventType, parser.GetScalarAsString()));
		}

		[Nullable(1)]
		public static readonly DateTimeOffsetFormatter Instance = new DateTimeOffsetFormatter();
	}
}
