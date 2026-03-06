using System;
using System.Linq;
using System.Reflection;

namespace UnityEngine.Rendering
{
	public static class DocumentationUtils
	{
		public static string GetHelpURL<TEnum>(TEnum mask = default(TEnum)) where TEnum : struct, IConvertible
		{
			HelpURLAttribute helpURLAttribute = (HelpURLAttribute)mask.GetType().GetCustomAttributes(typeof(HelpURLAttribute), false).FirstOrDefault<object>();
			if (helpURLAttribute != null)
			{
				return string.Format("{0}#{1}", helpURLAttribute.URL, mask);
			}
			return string.Empty;
		}

		public static bool TryGetHelpURL(Type type, out string url)
		{
			HelpURLAttribute customAttribute = type.GetCustomAttribute(false);
			url = ((customAttribute != null) ? customAttribute.URL : null);
			return customAttribute != null;
		}
	}
}
