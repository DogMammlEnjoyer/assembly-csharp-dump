using System;
using System.Collections.Generic;
using System.Reflection;

namespace Unity.XR.CoreUtils
{
	public static class TypeExtensions
	{
		public static void GetAssignableTypes(this Type type, List<Type> list, Func<Type, bool> predicate = null)
		{
			ReflectionUtils.ForEachType(delegate(Type t)
			{
				if (type.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract && (predicate == null || predicate(t)))
				{
					list.Add(t);
				}
			});
		}

		public static void GetImplementationsOfInterface(this Type type, List<Type> list)
		{
			if (type.IsInterface)
			{
				type.GetAssignableTypes(list, null);
			}
		}

		public static void GetExtensionsOfClass(this Type type, List<Type> list)
		{
			if (type.IsClass)
			{
				type.GetAssignableTypes(list, null);
			}
		}

		public static void GetGenericInterfaces(this Type type, Type genericInterface, List<Type> interfaces)
		{
			foreach (Type type2 in type.GetInterfaces())
			{
				if (type2.IsGenericType && type2.GetGenericTypeDefinition() == genericInterface)
				{
					interfaces.Add(type2);
				}
			}
		}

		public static PropertyInfo GetPropertyRecursively(this Type type, string name, BindingFlags bindingAttr)
		{
			PropertyInfo propertyInfo = type.GetProperty(name, bindingAttr);
			if (propertyInfo != null)
			{
				return propertyInfo;
			}
			if (type.BaseType != null)
			{
				propertyInfo = type.BaseType.GetPropertyRecursively(name, bindingAttr);
			}
			return propertyInfo;
		}

		public static FieldInfo GetFieldRecursively(this Type type, string name, BindingFlags bindingAttr)
		{
			FieldInfo fieldInfo = type.GetField(name, bindingAttr);
			if (fieldInfo != null)
			{
				return fieldInfo;
			}
			if (type.BaseType != null)
			{
				fieldInfo = type.BaseType.GetFieldRecursively(name, bindingAttr);
			}
			return fieldInfo;
		}

		public static void GetFieldsRecursively(this Type type, List<FieldInfo> fields, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			for (;;)
			{
				foreach (FieldInfo item in type.GetFields(bindingAttr))
				{
					fields.Add(item);
				}
				Type baseType = type.BaseType;
				if (!(baseType != null))
				{
					break;
				}
				type = baseType;
			}
		}

		public static void GetPropertiesRecursively(this Type type, List<PropertyInfo> fields, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		{
			for (;;)
			{
				foreach (PropertyInfo item in type.GetProperties(bindingAttr))
				{
					fields.Add(item);
				}
				Type baseType = type.BaseType;
				if (!(baseType != null))
				{
					break;
				}
				type = baseType;
			}
		}

		public static void GetInterfaceFieldsFromClasses(this IEnumerable<Type> classes, List<FieldInfo> fields, List<Type> interfaceTypes, BindingFlags bindingAttr)
		{
			foreach (Type type in interfaceTypes)
			{
				if (!type.IsInterface)
				{
					throw new ArgumentException(string.Format("Type {0} in interfaceTypes is not an interface!", type));
				}
			}
			foreach (Type type2 in classes)
			{
				if (!type2.IsClass)
				{
					throw new ArgumentException(string.Format("Type {0} in classes is not a class!", type2));
				}
				TypeExtensions.k_Fields.Clear();
				type2.GetFieldsRecursively(TypeExtensions.k_Fields, bindingAttr);
				foreach (FieldInfo fieldInfo in TypeExtensions.k_Fields)
				{
					foreach (Type item in fieldInfo.FieldType.GetInterfaces())
					{
						if (interfaceTypes.Contains(item))
						{
							fields.Add(fieldInfo);
							break;
						}
					}
				}
			}
		}

		public static TAttribute GetAttribute<TAttribute>(this Type type, bool inherit = false) where TAttribute : Attribute
		{
			return (TAttribute)((object)type.GetCustomAttributes(typeof(TAttribute), inherit)[0]);
		}

		public static void IsDefinedGetInheritedTypes<TAttribute>(this Type type, List<Type> types) where TAttribute : Attribute
		{
			while (type != null)
			{
				if (type.IsDefined(typeof(TAttribute), true))
				{
					types.Add(type);
				}
				type = type.BaseType;
			}
		}

		public static FieldInfo GetFieldInTypeOrBaseType(this Type type, string fieldName)
		{
			while (!(type == null))
			{
				FieldInfo field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
				if (field != null)
				{
					return field;
				}
				type = type.BaseType;
			}
			return null;
		}

		public static string GetNameWithGenericArguments(this Type type)
		{
			string text = type.Name;
			text = text.Replace('+', '.');
			if (!type.IsGenericType)
			{
				return text;
			}
			text = text.Split('`', StringSplitOptions.None)[0];
			Type[] genericArguments = type.GetGenericArguments();
			int num = genericArguments.Length;
			string[] array = new string[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = genericArguments[i].GetNameWithGenericArguments();
			}
			return text + "<" + string.Join(", ", array) + ">";
		}

		public static string GetNameWithFullGenericArguments(this Type type)
		{
			string text = type.Name;
			text = text.Replace('+', '.');
			if (!type.IsGenericType)
			{
				return text;
			}
			text = text.Split('`', StringSplitOptions.None)[0];
			Type[] genericArguments = type.GetGenericArguments();
			int num = genericArguments.Length;
			string[] array = new string[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = genericArguments[i].GetFullNameWithGenericArgumentsInternal();
			}
			return text + "<" + string.Join(", ", array) + ">";
		}

		public static string GetFullNameWithGenericArguments(this Type type)
		{
			Type type2 = type.DeclaringType;
			if (type2 != null && !type.IsGenericParameter)
			{
				TypeExtensions.k_TypeNames.Clear();
				string item = type.GetNameWithFullGenericArguments();
				TypeExtensions.k_TypeNames.Add(item);
				for (;;)
				{
					Type declaringType = type2.DeclaringType;
					if (declaringType == null)
					{
						break;
					}
					item = type2.GetNameWithFullGenericArguments();
					TypeExtensions.k_TypeNames.Insert(0, item);
					type2 = declaringType;
				}
				item = type2.GetFullNameWithGenericArguments();
				TypeExtensions.k_TypeNames.Insert(0, item);
				return string.Join(".", TypeExtensions.k_TypeNames.ToArray());
			}
			return type.GetFullNameWithGenericArgumentsInternal();
		}

		private static string GetFullNameWithGenericArgumentsInternal(this Type type)
		{
			string text = type.FullName;
			if (!type.IsGenericType)
			{
				return text;
			}
			text = text.Split('`', StringSplitOptions.None)[0];
			Type[] genericArguments = type.GetGenericArguments();
			int num = genericArguments.Length;
			string[] array = new string[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = genericArguments[i].GetFullNameWithGenericArguments();
			}
			return text + "<" + string.Join(", ", array) + ">";
		}

		public static bool IsAssignableFromOrSubclassOf(this Type checkType, Type baseType)
		{
			return checkType.IsAssignableFrom(baseType) || checkType.IsSubclassOf(baseType);
		}

		public static MethodInfo GetMethodRecursively(this Type type, string name, BindingFlags bindingAttr)
		{
			MethodInfo methodInfo = type.GetMethod(name, bindingAttr);
			if (methodInfo != null)
			{
				return methodInfo;
			}
			if (type.BaseType != null)
			{
				methodInfo = type.BaseType.GetMethodRecursively(name, bindingAttr);
			}
			return methodInfo;
		}

		private static readonly List<FieldInfo> k_Fields = new List<FieldInfo>();

		private static readonly List<string> k_TypeNames = new List<string>();
	}
}
