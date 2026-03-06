using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using VYaml.Annotations;

namespace VYaml.Serialization
{
	public class GeneratedResolver : IYamlFormatterResolver
	{
		[NullableContext(1)]
		private static bool TryInvokeRegisterYamlFormatter(Type type)
		{
			if (type.GetCustomAttribute<YamlObjectAttribute>() == null)
			{
				return false;
			}
			MethodInfo method = type.GetMethod("__RegisterVYamlFormatter", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (method == null)
			{
				return false;
			}
			method.Invoke(null, null);
			return true;
		}

		[NullableContext(1)]
		[Preserve]
		public static void Register<[Nullable(2)] T>(IYamlFormatter<T> formatter)
		{
			GeneratedResolver.Check<T>.Registered = true;
			GeneratedResolver.Cache<T>.Formatter = formatter;
		}

		[NullableContext(2)]
		[return: Nullable(new byte[]
		{
			2,
			1
		})]
		public IYamlFormatter<T> GetFormatter<T>()
		{
			return GeneratedResolver.Cache<T>.Formatter;
		}

		[Nullable(1)]
		public static readonly GeneratedResolver Instance = new GeneratedResolver();

		private static class Check<[Nullable(2)] T>
		{
			internal static bool Registered;
		}

		private static class Cache<[Nullable(2)] T>
		{
			static Cache()
			{
				if (GeneratedResolver.Check<T>.Registered)
				{
					return;
				}
				GeneratedResolver.TryInvokeRegisterYamlFormatter(typeof(T));
			}

			[Nullable(new byte[]
			{
				2,
				1
			})]
			internal static IYamlFormatter<T> Formatter;
		}
	}
}
