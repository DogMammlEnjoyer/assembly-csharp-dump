using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnityEngine.UIElements
{
	public static class UxmlSerializedDataUtility
	{
		public static object CopySerialized(object value)
		{
			bool flag = value == null;
			object result;
			if (flag)
			{
				result = null;
			}
			else
			{
				object obj = null;
				try
				{
					UxmlSerializableAdapterBase uxmlSerializableAdapterBase;
					bool flag2 = !UxmlSerializedDataUtility.s_Adapters.TryGetValue(value.GetType(), out uxmlSerializableAdapterBase);
					if (flag2)
					{
						Type type = typeof(UxmlSerializableAdapter<>).MakeGenericType(new Type[]
						{
							value.GetType()
						});
						FieldInfo field = type.GetField("SharedInstance", BindingFlags.Static | BindingFlags.Public);
						uxmlSerializableAdapterBase = (UxmlSerializableAdapterBase)field.GetValue(null);
						UxmlSerializedDataUtility.s_Adapters[value.GetType()] = uxmlSerializableAdapterBase;
					}
					obj = uxmlSerializableAdapterBase.CloneInstanceBoxed(value);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
				result = obj;
			}
			return result;
		}

		public static T CopySerialized<T>(object value)
		{
			UxmlSerializableAdapter<T> sharedInstance = UxmlSerializableAdapter<T>.SharedInstance;
			return sharedInstance.CloneInstance((T)((object)value));
		}

		internal static Dictionary<Type, UxmlSerializableAdapterBase> s_Adapters = new Dictionary<Type, UxmlSerializableAdapterBase>();
	}
}
