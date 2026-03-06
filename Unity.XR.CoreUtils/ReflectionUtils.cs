using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Unity.XR.CoreUtils
{
	public static class ReflectionUtils
	{
		private static Assembly[] GetCachedAssemblies()
		{
			Assembly[] result;
			if ((result = ReflectionUtils.s_Assemblies) == null)
			{
				result = (ReflectionUtils.s_Assemblies = AppDomain.CurrentDomain.GetAssemblies());
			}
			return result;
		}

		private static List<Type[]> GetCachedTypesPerAssembly()
		{
			if (ReflectionUtils.s_TypesPerAssembly == null)
			{
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				ReflectionUtils.s_TypesPerAssembly = new List<Type[]>(assemblies.Length);
				foreach (Assembly assembly in assemblies)
				{
					try
					{
						ReflectionUtils.s_TypesPerAssembly.Add(assembly.GetTypes());
					}
					catch (ReflectionTypeLoadException)
					{
					}
				}
			}
			return ReflectionUtils.s_TypesPerAssembly;
		}

		private static List<Dictionary<string, Type>> GetCachedAssemblyTypeMaps()
		{
			if (ReflectionUtils.s_AssemblyTypeMaps == null)
			{
				List<Type[]> cachedTypesPerAssembly = ReflectionUtils.GetCachedTypesPerAssembly();
				ReflectionUtils.s_AssemblyTypeMaps = new List<Dictionary<string, Type>>(cachedTypesPerAssembly.Count);
				foreach (Type[] array in cachedTypesPerAssembly)
				{
					try
					{
						Dictionary<string, Type> dictionary = new Dictionary<string, Type>();
						foreach (Type type in array)
						{
							dictionary[type.FullName] = type;
						}
						ReflectionUtils.s_AssemblyTypeMaps.Add(dictionary);
					}
					catch (ReflectionTypeLoadException)
					{
					}
				}
			}
			return ReflectionUtils.s_AssemblyTypeMaps;
		}

		public static void PreWarmTypeCache()
		{
			ReflectionUtils.GetCachedAssemblyTypeMaps();
		}

		public static void ForEachAssembly(Action<Assembly> callback)
		{
			foreach (Assembly obj in ReflectionUtils.GetCachedAssemblies())
			{
				try
				{
					callback(obj);
				}
				catch (ReflectionTypeLoadException)
				{
				}
			}
		}

		public static void ForEachType(Action<Type> callback)
		{
			foreach (Type[] array in ReflectionUtils.GetCachedTypesPerAssembly())
			{
				foreach (Type obj in array)
				{
					callback(obj);
				}
			}
		}

		public static Type FindType(Func<Type, bool> predicate)
		{
			foreach (Type[] array in ReflectionUtils.GetCachedTypesPerAssembly())
			{
				foreach (Type type in array)
				{
					if (predicate(type))
					{
						return type;
					}
				}
			}
			return null;
		}

		public static Type FindTypeByFullName(string fullName)
		{
			using (List<Dictionary<string, Type>>.Enumerator enumerator = ReflectionUtils.GetCachedAssemblyTypeMaps().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Type result;
					if (enumerator.Current.TryGetValue(fullName, out result))
					{
						return result;
					}
				}
			}
			return null;
		}

		public static void FindTypesBatch(List<Func<Type, bool>> predicates, List<Type> resultList)
		{
			List<Type[]> cachedTypesPerAssembly = ReflectionUtils.GetCachedTypesPerAssembly();
			for (int i = 0; i < predicates.Count; i++)
			{
				Func<Type, bool> func = predicates[i];
				foreach (Type[] array in cachedTypesPerAssembly)
				{
					foreach (Type type in array)
					{
						if (func(type))
						{
							resultList[i] = type;
						}
					}
				}
			}
		}

		public static void FindTypesByFullNameBatch(List<string> typeNames, List<Type> resultList)
		{
			List<Dictionary<string, Type>> cachedAssemblyTypeMaps = ReflectionUtils.GetCachedAssemblyTypeMaps();
			foreach (string key in typeNames)
			{
				bool flag = false;
				using (List<Dictionary<string, Type>>.Enumerator enumerator2 = cachedAssemblyTypeMaps.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Type item;
						if (enumerator2.Current.TryGetValue(key, out item))
						{
							resultList.Add(item);
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					resultList.Add(null);
				}
			}
		}

		public static Type FindTypeInAssemblyByFullName(string assemblyName, string typeName)
		{
			Assembly[] cachedAssemblies = ReflectionUtils.GetCachedAssemblies();
			List<Dictionary<string, Type>> cachedAssemblyTypeMaps = ReflectionUtils.GetCachedAssemblyTypeMaps();
			int i = 0;
			while (i < cachedAssemblies.Length)
			{
				if (!(cachedAssemblies[i].GetName().Name != assemblyName))
				{
					Type result;
					if (!cachedAssemblyTypeMaps[i].TryGetValue(typeName, out result))
					{
						return null;
					}
					return result;
				}
				else
				{
					i++;
				}
			}
			return null;
		}

		public static string NicifyVariableName(string name)
		{
			if (name.StartsWith("m_"))
			{
				name = name.Substring(2, name.Length - 2);
			}
			else if (name.StartsWith("_"))
			{
				name = name.Substring(1, name.Length - 1);
			}
			if (name[0] == 'k' && name[1] >= 'A' && name[1] <= 'Z')
			{
				name = name.Substring(1, name.Length - 1);
			}
			name = Regex.Replace(name, "(\\B[A-Z]+?(?=[A-Z][^A-Z])|\\B[A-Z]+?(?=[^A-Z]))", " $1");
			name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);
			return name;
		}

		private static Assembly[] s_Assemblies;

		private static List<Type[]> s_TypesPerAssembly;

		private static List<Dictionary<string, Type>> s_AssemblyTypeMaps;
	}
}
