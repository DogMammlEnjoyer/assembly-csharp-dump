using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	public static class UxmlDescriptionCache
	{
		public static void RegisterType(Type type, UxmlAttributeNames[] attributeNames)
		{
			UxmlDescriptionCache.s_NamesPerType[type] = attributeNames;
		}

		internal static bool TryGetCachedDescription(Type type, out UxmlAttributeNames[] attributes)
		{
			return UxmlDescriptionCache.s_NamesPerType.TryGetValue(type, out attributes);
		}

		private static readonly Dictionary<Type, UxmlAttributeNames[]> s_NamesPerType = new Dictionary<Type, UxmlAttributeNames[]>();
	}
}
