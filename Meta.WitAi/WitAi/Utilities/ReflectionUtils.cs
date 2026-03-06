using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Meta.WitAi.Utilities
{
	public class ReflectionUtils
	{
		private static FieldInfo GetCachedField(Type type, string fieldName)
		{
			string key = type.FullName + "." + fieldName;
			FieldInfo field;
			if (!ReflectionUtils._cachedFields.TryGetValue(key, out field))
			{
				field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				ReflectionUtils._cachedFields[key] = field;
			}
			return field;
		}

		private static PropertyInfo GetCachedProperty(Type type, string propertyName)
		{
			string key = type.FullName + "." + propertyName;
			PropertyInfo property;
			if (!ReflectionUtils._cachedProperties.TryGetValue(key, out property))
			{
				property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				ReflectionUtils._cachedProperties[key] = property;
			}
			return property;
		}

		private static MethodInfo GetCachedMethod(Type type, string methodName)
		{
			string key = type.FullName + "." + methodName;
			MethodInfo method;
			if (!ReflectionUtils._cachedMethods.TryGetValue(key, out method))
			{
				method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				ReflectionUtils._cachedMethods[key] = method;
			}
			return method;
		}

		public static bool ReflectFieldValue<T>(object obj, string fieldName, out T data)
		{
			FieldInfo cachedField = ReflectionUtils.GetCachedField(obj.GetType(), fieldName);
			if (null != cachedField)
			{
				data = (T)((object)cachedField.GetValue(obj));
			}
			else
			{
				data = default(T);
			}
			return null != cachedField;
		}

		public static bool ReflectPropertyValue<T>(object obj, string fieldName, out T data)
		{
			PropertyInfo cachedProperty = ReflectionUtils.GetCachedProperty(obj.GetType(), fieldName);
			if (null != cachedProperty)
			{
				data = (T)((object)cachedProperty.GetValue(obj));
			}
			else
			{
				data = default(T);
			}
			return null != cachedProperty;
		}

		public static bool ReflectMethodValue<T>(object obj, string fieldName, out T data)
		{
			MethodInfo cachedMethod = ReflectionUtils.GetCachedMethod(obj.GetType(), fieldName);
			if (null != cachedMethod)
			{
				data = (T)((object)cachedMethod.Invoke(obj, null));
			}
			else
			{
				data = default(T);
			}
			return null != cachedMethod;
		}

		public static bool TryReflectValue<T>(object obj, string fieldName, out T value)
		{
			return ReflectionUtils.ReflectFieldValue<T>(obj, fieldName, out value) || ReflectionUtils.ReflectPropertyValue<T>(obj, fieldName, out value) || ReflectionUtils.ReflectMethodValue<T>(obj, fieldName, out value);
		}

		public static T ReflectValue<T>(object obj, string fieldName)
		{
			T result;
			if (ReflectionUtils.TryReflectValue<T>(obj, fieldName, out result))
			{
				return result;
			}
			throw new ArgumentException("No field, property, or method named '" + fieldName + "' was found.");
		}

		private static bool IsValidNamespace(Type type)
		{
			return type != null && type.Namespace != null && type.Namespace.StartsWith("Meta");
		}

		private static IEnumerable<Type> GetTypes()
		{
			return AppDomain.CurrentDomain.GetAssemblies().SelectMany(delegate(Assembly assembly)
			{
				IEnumerable<Type> result;
				try
				{
					result = assembly.GetTypes();
				}
				catch
				{
					result = new Type[0];
				}
				return result;
			}).Where(new Func<Type, bool>(ReflectionUtils.IsValidNamespace));
		}

		private static IEnumerable<MethodInfo> GetMethods()
		{
			return ReflectionUtils.GetTypes().SelectMany((Type type) => type.GetMethods());
		}

		internal static Type[] GetAllAssignableTypes<T>()
		{
			return (from type in ReflectionUtils.GetTypes()
			where typeof(T).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract
			select type).ToArray<Type>();
		}

		internal static Type[] GetTypesWithAttribute<T>() where T : Attribute
		{
			return (from type in ReflectionUtils.GetTypes()
			where type.GetCustomAttributes(typeof(T), false).Length != 0
			select type).ToArray<Type>();
		}

		internal static MethodInfo[] GetMethodsWithAttribute<T>() where T : Attribute
		{
			return (from method in ReflectionUtils.GetMethods()
			where method.GetCustomAttributes(typeof(T), false).Length != 0
			select method).ToArray<MethodInfo>();
		}

		private static Dictionary<string, FieldInfo> _cachedFields = new Dictionary<string, FieldInfo>();

		private static Dictionary<string, PropertyInfo> _cachedProperties = new Dictionary<string, PropertyInfo>();

		private static Dictionary<string, MethodInfo> _cachedMethods = new Dictionary<string, MethodInfo>();

		private const string NAMESPACE_PREFIX = "Meta";
	}
}
