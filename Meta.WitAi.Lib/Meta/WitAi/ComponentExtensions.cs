using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Meta.WitAi
{
	public static class ComponentExtensions
	{
		public static void Copy<T>(this T toComponent, T fromComponent) where T : Component
		{
			if (toComponent == null)
			{
				return;
			}
			ComponentExtensions.ComponentCopyData copyData = fromComponent.GetCopyData<T>();
			foreach (FieldInfo fieldInfo in copyData.Fields)
			{
				fieldInfo.SetValue(toComponent, fieldInfo.GetValue(fromComponent));
			}
			foreach (PropertyInfo propertyInfo in copyData.Properties)
			{
				propertyInfo.SetValue(toComponent, propertyInfo.GetValue(fromComponent));
			}
		}

		public static void PreloadCopyData<T>(this T thisComponent) where T : Component
		{
			thisComponent.GetCopyData<T>();
		}

		private static ComponentExtensions.ComponentCopyData GetCopyData<T>(this T thisComponent) where T : Component
		{
			Type typeFromHandle = typeof(T);
			if (!ComponentExtensions._data.ContainsKey(typeFromHandle))
			{
				ComponentExtensions.ComponentCopyData value = default(ComponentExtensions.ComponentCopyData);
				value.ComponentType = typeFromHandle;
				List<FieldInfo> list = new List<FieldInfo>();
				foreach (FieldInfo fieldInfo in typeFromHandle.GetFields(BindingFlags.Instance | BindingFlags.Public))
				{
					if (!ComponentExtensions.IsObsolete(fieldInfo.CustomAttributes))
					{
						list.Add(fieldInfo);
					}
				}
				value.Fields = list.ToArray();
				List<PropertyInfo> list2 = new List<PropertyInfo>();
				foreach (PropertyInfo propertyInfo in typeFromHandle.GetProperties(BindingFlags.Instance | BindingFlags.Public))
				{
					if (!ComponentExtensions.IsObsolete(propertyInfo.CustomAttributes) && propertyInfo.CanWrite && propertyInfo.CanRead && !string.Equals(propertyInfo.Name, "name"))
					{
						list2.Add(propertyInfo);
					}
				}
				value.Properties = list2.ToArray();
				ComponentExtensions._data[typeFromHandle] = value;
			}
			return ComponentExtensions._data[typeFromHandle];
		}

		private static bool IsObsolete(IEnumerable<CustomAttributeData> attributes)
		{
			return ComponentExtensions.HasCustomAttributes<ObsoleteAttribute>(attributes);
		}

		private static bool HasCustomAttributes<TAttribute>(IEnumerable<CustomAttributeData> attributes) where TAttribute : Attribute
		{
			if (attributes != null)
			{
				using (IEnumerator<CustomAttributeData> enumerator = attributes.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.AttributeType == typeof(TAttribute))
						{
							return true;
						}
					}
				}
				return false;
			}
			return false;
		}

		public static T GetFirstComponentOfType<T>(this Component component)
		{
			return component.GetComponentsOfType<T>().First<T>();
		}

		public static IEnumerable<T> GetComponentsOfType<T>(this Component component)
		{
			return (from c in component.GetComponents<MonoBehaviour>()
			where c is T
			select c).Cast<T>();
		}

		private static Dictionary<Type, ComponentExtensions.ComponentCopyData> _data = new Dictionary<Type, ComponentExtensions.ComponentCopyData>();

		[Serializable]
		public struct ComponentCopyData
		{
			public Type ComponentType;

			public FieldInfo[] Fields;

			public PropertyInfo[] Properties;
		}
	}
}
