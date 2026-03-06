using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VYaml.Emitter;
using VYaml.Internal;
using VYaml.Parser;

namespace VYaml.Serialization
{
	[NullableContext(2)]
	[Nullable(0)]
	public static class YamlSerializer
	{
		[NullableContext(1)]
		private static YamlDeserializationContext GetThreadLocalDeserializationContext([Nullable(2)] YamlSerializerOptions options = null)
		{
			if (options == null)
			{
				options = YamlSerializer.DefaultOptions;
			}
			YamlDeserializationContext yamlDeserializationContext;
			if ((yamlDeserializationContext = YamlSerializer.deserializationContext) == null)
			{
				yamlDeserializationContext = (YamlSerializer.deserializationContext = new YamlDeserializationContext(options));
			}
			YamlDeserializationContext yamlDeserializationContext2 = yamlDeserializationContext;
			yamlDeserializationContext2.Resolver = options.Resolver;
			return yamlDeserializationContext2;
		}

		[NullableContext(1)]
		private static YamlSerializationContext GetThreadLocalSerializationContext([Nullable(2)] YamlSerializerOptions options = null)
		{
			if (options == null)
			{
				options = YamlSerializer.DefaultOptions;
			}
			YamlSerializationContext yamlSerializationContext;
			if ((yamlSerializationContext = YamlSerializer.serializationContext) == null)
			{
				yamlSerializationContext = (YamlSerializer.serializationContext = new YamlSerializationContext(options));
			}
			YamlSerializationContext yamlSerializationContext2 = yamlSerializationContext;
			yamlSerializationContext2.Resolver = options.Resolver;
			yamlSerializationContext2.EmitOptions = options.EmitOptions;
			return yamlSerializationContext2;
		}

		[Nullable(1)]
		public static YamlSerializerOptions DefaultOptions
		{
			[NullableContext(1)]
			get
			{
				YamlSerializerOptions result;
				if ((result = YamlSerializer.defaultOptions) == null)
				{
					result = (YamlSerializer.defaultOptions = YamlSerializerOptions.Standard);
				}
				return result;
			}
			[NullableContext(1)]
			set
			{
				YamlSerializer.defaultOptions = value;
			}
		}

		[return: Nullable(0)]
		public static ReadOnlyMemory<byte> Serialize<T>([Nullable(1)] T value, YamlSerializerOptions options = null)
		{
			if (options == null)
			{
				options = YamlSerializer.DefaultOptions;
			}
			YamlSerializationContext threadLocalSerializationContext = YamlSerializer.GetThreadLocalSerializationContext(options);
			ArrayBufferWriter<byte> arrayBufferWriter = threadLocalSerializationContext.GetArrayBufferWriter();
			Utf8YamlEmitter utf8YamlEmitter = new Utf8YamlEmitter(arrayBufferWriter, null);
			threadLocalSerializationContext.Reset();
			threadLocalSerializationContext.Resolver.GetFormatterWithVerify<T>().Serialize(ref utf8YamlEmitter, value, threadLocalSerializationContext);
			return arrayBufferWriter.WrittenMemory;
		}

		[NullableContext(1)]
		public static void Serialize<[Nullable(2)] T>(IBufferWriter<byte> writer, T value, [Nullable(2)] YamlSerializerOptions options = null)
		{
			Utf8YamlEmitter utf8YamlEmitter = new Utf8YamlEmitter(writer, null);
			YamlSerializer.Serialize<T>(ref utf8YamlEmitter, value, options);
		}

		public static void Serialize<T>(ref Utf8YamlEmitter emitter, [Nullable(1)] T value, YamlSerializerOptions options = null)
		{
			if (options == null)
			{
				options = YamlSerializer.DefaultOptions;
			}
			YamlSerializationContext threadLocalSerializationContext = YamlSerializer.GetThreadLocalSerializationContext(options);
			threadLocalSerializationContext.Reset();
			threadLocalSerializationContext.Resolver.GetFormatterWithVerify<T>().Serialize(ref emitter, value, threadLocalSerializationContext);
		}

		[NullableContext(1)]
		public static string SerializeToString<[Nullable(2)] T>(T value, [Nullable(2)] YamlSerializerOptions options = null)
		{
			ReadOnlyMemory<byte> readOnlyMemory = YamlSerializer.Serialize<T>(value, options);
			return StringEncoding.Utf8.GetString(readOnlyMemory.Span);
		}

		[return: Nullable(1)]
		public static T Deserialize<T>([Nullable(0)] ReadOnlyMemory<byte> memory, YamlSerializerOptions options = null)
		{
			ReadOnlySequence<byte> readOnlySequence = new ReadOnlySequence<byte>(memory);
			YamlParser yamlParser = YamlParser.FromSequence(readOnlySequence);
			return YamlSerializer.Deserialize<T>(ref yamlParser, options);
		}

		[return: Nullable(1)]
		public static T Deserialize<T>([Nullable(0)] in ReadOnlySequence<byte> sequence, YamlSerializerOptions options = null)
		{
			YamlParser yamlParser = YamlParser.FromSequence(sequence);
			return YamlSerializer.Deserialize<T>(ref yamlParser, options);
		}

		[return: Nullable(new byte[]
		{
			0,
			1
		})]
		public static ValueTask<T> DeserializeAsync<T>([Nullable(1)] Stream stream, YamlSerializerOptions options = null)
		{
			YamlSerializer.<DeserializeAsync>d__14<T> <DeserializeAsync>d__;
			<DeserializeAsync>d__.<>t__builder = AsyncValueTaskMethodBuilder<T>.Create();
			<DeserializeAsync>d__.stream = stream;
			<DeserializeAsync>d__.options = options;
			<DeserializeAsync>d__.<>1__state = -1;
			<DeserializeAsync>d__.<>t__builder.Start<YamlSerializer.<DeserializeAsync>d__14<T>>(ref <DeserializeAsync>d__);
			return <DeserializeAsync>d__.<>t__builder.Task;
		}

		[return: Nullable(1)]
		public static T Deserialize<T>(ref YamlParser parser, YamlSerializerOptions options = null)
		{
			if (options == null)
			{
				options = YamlSerializer.DefaultOptions;
			}
			YamlDeserializationContext threadLocalDeserializationContext = YamlSerializer.GetThreadLocalDeserializationContext(options);
			threadLocalDeserializationContext.Reset();
			parser.SkipAfter(ParseEventType.DocumentStart);
			IYamlFormatter<T> formatterWithVerify = options.Resolver.GetFormatterWithVerify<T>();
			return threadLocalDeserializationContext.DeserializeWithAlias<T>(formatterWithVerify, ref parser);
		}

		[return: Nullable(new byte[]
		{
			0,
			1,
			1
		})]
		public static ValueTask<IEnumerable<T>> DeserializeMultipleDocumentsAsync<T>([Nullable(1)] Stream stream, YamlSerializerOptions options = null)
		{
			YamlSerializer.<DeserializeMultipleDocumentsAsync>d__16<T> <DeserializeMultipleDocumentsAsync>d__;
			<DeserializeMultipleDocumentsAsync>d__.<>t__builder = AsyncValueTaskMethodBuilder<IEnumerable<T>>.Create();
			<DeserializeMultipleDocumentsAsync>d__.stream = stream;
			<DeserializeMultipleDocumentsAsync>d__.options = options;
			<DeserializeMultipleDocumentsAsync>d__.<>1__state = -1;
			<DeserializeMultipleDocumentsAsync>d__.<>t__builder.Start<YamlSerializer.<DeserializeMultipleDocumentsAsync>d__16<T>>(ref <DeserializeMultipleDocumentsAsync>d__);
			return <DeserializeMultipleDocumentsAsync>d__.<>t__builder.Task;
		}

		[return: Nullable(1)]
		public static IEnumerable<T> DeserializeMultipleDocuments<T>([Nullable(0)] ReadOnlyMemory<byte> memory, YamlSerializerOptions options = null)
		{
			ReadOnlySequence<byte> readOnlySequence = new ReadOnlySequence<byte>(memory);
			YamlParser yamlParser = YamlParser.FromSequence(readOnlySequence);
			return YamlSerializer.DeserializeMultipleDocuments<T>(ref yamlParser, options);
		}

		[return: Nullable(1)]
		public static IEnumerable<T> DeserializeMultipleDocuments<T>([Nullable(0)] in ReadOnlySequence<byte> sequence, YamlSerializerOptions options = null)
		{
			YamlParser yamlParser = YamlParser.FromSequence(sequence);
			return YamlSerializer.DeserializeMultipleDocuments<T>(ref yamlParser, options);
		}

		[return: Nullable(1)]
		public static IEnumerable<T> DeserializeMultipleDocuments<T>(ref YamlParser parser, YamlSerializerOptions options = null)
		{
			if (options == null)
			{
				options = YamlSerializer.DefaultOptions;
			}
			YamlDeserializationContext threadLocalDeserializationContext = YamlSerializer.GetThreadLocalDeserializationContext(options);
			IYamlFormatter<T> formatterWithVerify = options.Resolver.GetFormatterWithVerify<T>();
			List<T> list = new List<T>();
			for (;;)
			{
				parser.SkipAfter(ParseEventType.DocumentStart);
				if (parser.End)
				{
					break;
				}
				threadLocalDeserializationContext.Reset();
				T item = threadLocalDeserializationContext.DeserializeWithAlias<T>(formatterWithVerify, ref parser);
				list.Add(item);
			}
			return list;
		}

		[ThreadStatic]
		private static YamlDeserializationContext deserializationContext;

		[ThreadStatic]
		private static YamlSerializationContext serializationContext;

		private static YamlSerializerOptions defaultOptions;
	}
}
