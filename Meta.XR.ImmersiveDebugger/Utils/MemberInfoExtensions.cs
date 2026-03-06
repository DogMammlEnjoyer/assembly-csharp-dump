using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.Utils
{
	internal static class MemberInfoExtensions
	{
		public static object GetValue(this MemberInfo memberInfo, object instance)
		{
			MemberTypes memberType = memberInfo.MemberType;
			if ((memberType & MemberTypes.Field) != (MemberTypes)0)
			{
				return ((FieldInfo)memberInfo).GetValue(instance);
			}
			if ((memberType & MemberTypes.Property) == (MemberTypes)0)
			{
				Debug.LogWarning("Calling GetValue() from wrong member type, expect field/property");
				return null;
			}
			PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
			if (propertyInfo.CanRead)
			{
				return propertyInfo.GetValue(instance);
			}
			Debug.LogWarning("Calling GetValue() from property cannot be read");
			return null;
		}

		public static void SetValue(this MemberInfo memberInfo, object instance, object value)
		{
			MemberTypes memberType = memberInfo.MemberType;
			if ((memberType & MemberTypes.Field) != (MemberTypes)0)
			{
				((FieldInfo)memberInfo).SetValue(instance, value);
				return;
			}
			if ((memberType & MemberTypes.Property) == (MemberTypes)0)
			{
				Debug.LogWarning("Calling SetValue() from wrong member type, expect field/property");
				return;
			}
			PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
			if (propertyInfo.CanWrite)
			{
				propertyInfo.SetValue(instance, value);
				return;
			}
			Debug.LogWarning("Calling SetValue() from property cannot be written");
		}

		public static Type GetDataType(this MemberInfo memberInfo)
		{
			MemberTypes memberType = memberInfo.MemberType;
			if ((memberType & MemberTypes.Field) != (MemberTypes)0)
			{
				return ((FieldInfo)memberInfo).FieldType;
			}
			if ((memberType & MemberTypes.Property) != (MemberTypes)0)
			{
				return ((PropertyInfo)memberInfo).PropertyType;
			}
			Debug.LogWarning("Calling GetDataType() from wrong member type, expect field/property");
			return null;
		}

		public static bool IsStatic(this MemberInfo memberInfo)
		{
			MemberTypes memberType = memberInfo.MemberType;
			if ((memberType & MemberTypes.Field) != (MemberTypes)0)
			{
				return ((FieldInfo)memberInfo).IsStatic;
			}
			if ((memberType & MemberTypes.Property) != (MemberTypes)0)
			{
				PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
				return (propertyInfo.CanRead && propertyInfo.GetMethod.IsStatic) || (propertyInfo.CanWrite && propertyInfo.SetMethod.IsStatic);
			}
			if ((memberType & MemberTypes.Method) != (MemberTypes)0)
			{
				return ((MethodInfo)memberInfo).IsStatic;
			}
			Debug.LogWarning("Calling IsStatic() from wrong member type, expect field/property");
			return false;
		}

		public static bool IsPublic(this MemberInfo memberInfo)
		{
			MemberTypes memberType = memberInfo.MemberType;
			if ((memberType & MemberTypes.Field) != (MemberTypes)0)
			{
				return ((FieldInfo)memberInfo).IsPublic;
			}
			if ((memberType & MemberTypes.Property) != (MemberTypes)0)
			{
				PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
				return (propertyInfo.CanRead && propertyInfo.GetMethod.IsPublic) || (propertyInfo.CanWrite && propertyInfo.SetMethod.IsPublic);
			}
			return (memberType & MemberTypes.Method) != (MemberTypes)0 && ((MethodInfo)memberInfo).IsPublic;
		}

		public static string BuildSignatureForDebugInspector(this MemberInfo memberInfo)
		{
			MemberTypes memberType = memberInfo.MemberType;
			if ((memberType & MemberTypes.Field) != (MemberTypes)0)
			{
				FieldInfo fieldInfo = (FieldInfo)memberInfo;
				string text = fieldInfo.IsPublic ? "public" : (fieldInfo.IsPrivate ? "private" : (fieldInfo.IsFamily ? "protected" : "internal"));
				return string.Concat(new string[]
				{
					"<i>",
					text,
					" ",
					fieldInfo.FieldType.Name,
					"</i> <b>",
					fieldInfo.Name,
					"</b>"
				});
			}
			if ((memberType & MemberTypes.Method) != (MemberTypes)0)
			{
				MethodInfo methodInfo = (MethodInfo)memberInfo;
				string text2 = methodInfo.IsPublic ? "public" : (methodInfo.IsPrivate ? "private" : (methodInfo.IsFamily ? "protected" : "internal"));
				return string.Concat(new string[]
				{
					"<i>",
					text2,
					" ",
					methodInfo.ReturnType.Name,
					"</i> <b>",
					methodInfo.Name,
					"</b>()"
				});
			}
			if ((memberType & MemberTypes.Property) != (MemberTypes)0)
			{
				PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
				MethodInfo getMethod = propertyInfo.GetMethod;
				string text3 = getMethod.IsPublic ? "public" : (getMethod.IsPrivate ? "private" : (getMethod.IsFamily ? "protected" : "internal"));
				return string.Concat(new string[]
				{
					"<i>",
					text3,
					" ",
					propertyInfo.PropertyType.Name,
					"</i> <b>",
					propertyInfo.Name,
					"</b>"
				});
			}
			return memberInfo.Name;
		}

		public static bool IsCompatibleWithDebugInspector(this MemberInfo memberInfo)
		{
			if (memberInfo as ConstructorInfo != null)
			{
				return false;
			}
			MemberTypes memberType = memberInfo.MemberType;
			if ((memberType & (MemberTypes.Field | MemberTypes.Method | MemberTypes.Property)) == (MemberTypes)0)
			{
				return false;
			}
			if (memberInfo.GetCustomAttribute<CompilerGeneratedAttribute>() != null)
			{
				return false;
			}
			if ((memberType & MemberTypes.Method) != (MemberTypes)0)
			{
				MethodInfo methodInfo = (MethodInfo)memberInfo;
				if (methodInfo.GetParameters().Length != 0 || methodInfo.ReturnType != typeof(void))
				{
					return false;
				}
			}
			PropertyInfo propertyInfo = memberInfo as PropertyInfo;
			return propertyInfo == null || propertyInfo.CanRead;
		}

		public static bool IsTypeEqual(this MemberInfo member, Type type)
		{
			FieldInfo fieldInfo = member as FieldInfo;
			if (!(((fieldInfo != null) ? fieldInfo.FieldType : null) == type))
			{
				PropertyInfo propertyInfo = member as PropertyInfo;
				return ((propertyInfo != null) ? propertyInfo.PropertyType : null) == type;
			}
			return true;
		}

		public static bool IsBaseTypeEqual(this MemberInfo member, Type type)
		{
			FieldInfo fieldInfo = member as FieldInfo;
			if (!(((fieldInfo != null) ? fieldInfo.FieldType.BaseType : null) == type))
			{
				PropertyInfo propertyInfo = member as PropertyInfo;
				return ((propertyInfo != null) ? propertyInfo.PropertyType.BaseType : null) == type;
			}
			return true;
		}

		public static bool CanBeChanged(this MemberInfo memberInfo)
		{
			return (memberInfo.MemberType & (MemberTypes.Field | MemberTypes.Property)) > (MemberTypes)0;
		}
	}
}
