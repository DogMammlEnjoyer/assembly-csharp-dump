using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using VYaml.Annotations;
using VYaml.Internal;

namespace VYaml.Serialization
{
	[NullableContext(1)]
	[Nullable(0)]
	internal class EnumAsStringNonGenericCache
	{
		public string GetStringValue(Type type, object value)
		{
			string result;
			if (this.stringValues.TryGetValue(value, out result))
			{
				return result;
			}
			return this.stringValues.GetOrAdd<Type>(value, this.valueFactory, type);
		}

		private static string CreateValue(object value, Type type)
		{
			YamlObjectAttribute customAttribute = type.GetCustomAttribute<YamlObjectAttribute>();
			NamingConvention namingConvention = (customAttribute != null) ? customAttribute.NamingConvention : NamingConvention.LowerCamelCase;
			return KeyNameMutator.Mutate(Enum.GetName(type, value), namingConvention);
		}

		public static readonly EnumAsStringNonGenericCache Instance = new EnumAsStringNonGenericCache();

		private readonly ConcurrentDictionary<object, string> stringValues = new ConcurrentDictionary<object, string>();

		private readonly Func<object, Type, string> valueFactory = new Func<object, Type, string>(EnumAsStringNonGenericCache.CreateValue);
	}
}
