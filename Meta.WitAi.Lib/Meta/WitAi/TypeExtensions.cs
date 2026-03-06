using System;
using System.Collections.Generic;
using System.Reflection;

namespace Meta.WitAi
{
	public static class TypeExtensions
	{
		private static List<Type> GetTypes(Func<Type, bool> isValid, bool firstOnly)
		{
			List<Type> list = new List<Type>();
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				Type[] array;
				try
				{
					array = assembly.GetTypes();
				}
				catch
				{
					array = new Type[0];
				}
				foreach (Type type in array)
				{
					if (isValid(type))
					{
						list.Add(type);
						if (firstOnly)
						{
							return list;
						}
					}
				}
			}
			return list;
		}

		public static List<Type> GetSubclassTypes(this Type baseType, bool firstOnly = false)
		{
			return TypeExtensions.GetTypes((Type type) => type.IsSubclassOf(baseType), firstOnly);
		}
	}
}
