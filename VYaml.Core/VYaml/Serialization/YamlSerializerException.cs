using System;
using System.Runtime.CompilerServices;
using VYaml.Parser;

namespace VYaml.Serialization
{
	[NullableContext(1)]
	[Nullable(0)]
	public class YamlSerializerException : Exception
	{
		public static void ThrowInvalidType<[Nullable(2)] T>(T value)
		{
			throw new YamlSerializerException(string.Format("Cannot detect a value of enum: {0}, {1}", typeof(T), value));
		}

		[NullableContext(2)]
		public static void ThrowInvalidType<T>()
		{
			throw new YamlSerializerException(string.Format("Cannot detect a scalar value of {0}", typeof(T)));
		}

		public YamlSerializerException(string message) : base(message)
		{
		}

		public YamlSerializerException(Marker mark, string message) : base(string.Format("{0} at {1}", message, mark))
		{
		}
	}
}
