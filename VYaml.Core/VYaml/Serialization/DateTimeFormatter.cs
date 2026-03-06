using System;
using System.Buffers;
using System.Buffers.Text;
using System.Globalization;
using System.Runtime.CompilerServices;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
	public class DateTimeFormatter : IYamlFormatter<DateTime>, IYamlFormatter
	{
		[NullableContext(1)]
		public void Serialize(ref Utf8YamlEmitter emitter, DateTime value, YamlSerializationContext context)
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
		public DateTime Deserialize(ref YamlParser parser, YamlDeserializationContext context)
		{
			ReadOnlySpan<byte> source;
			DateTime result;
			int num;
			if (parser.TryGetScalarAsSpan(out source) && Utf8Parser.TryParse(source, out result, out num, 'O') && num == source.Length)
			{
				parser.Read();
				return result;
			}
			if (DateTime.TryParse(parser.GetScalarAsString(), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out result))
			{
				parser.Read();
				return result;
			}
			throw new YamlSerializerException(string.Format("Cannot detect a scalar value of DateTime : {0} {1}", parser.CurrentEventType, parser.GetScalarAsString()));
		}

		[Nullable(1)]
		public static readonly DateTimeFormatter Instance = new DateTimeFormatter();
	}
}
