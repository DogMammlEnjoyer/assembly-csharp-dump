using System;
using System.Buffers;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class DecimalFormatter : IYamlFormatter<decimal>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, decimal value, YamlSerializationContext context)
		{
			byte[] buffer = context.GetBuffer64();
			int value2;
			if (Utf8Formatter.TryFormat(value, buffer, out value2, default(StandardFormat)))
			{
				emitter.WriteScalar(RuntimeHelpers.GetSubArray<byte>(buffer, Range.EndAt(value2)));
				return;
			}
			throw new YamlSerializerException(string.Format("Cannot serialize a value: {0}", value));
		}

		[NullableContext(1)]
		public decimal Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			ReadOnlySpan<byte> source;
			decimal result;
			int num;
			if (parser.TryGetScalarAsSpan(out source) && Utf8Parser.TryParse(source, out result, out num, '\0') && num == source.Length)
			{
				parser.Read();
				return result;
			}
			throw new YamlSerializerException(string.Format("Cannot detect a scalar value of decimal : {0} {1}", parser.CurrentEventType, parser.GetScalarAsString()));
		}

		[Nullable(1)]
		public static readonly DecimalFormatter Instance = new DecimalFormatter();
	}
}
