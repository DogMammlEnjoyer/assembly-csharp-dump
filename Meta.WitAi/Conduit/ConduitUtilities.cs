using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Meta.WitAi;

namespace Meta.Conduit
{
	internal static class ConduitUtilities
	{
		public static string DelimitWithUnderscores(string input)
		{
			return ConduitUtilities.UnderscoreSplitter.Replace(input, "_$1");
		}

		public static bool IsNullableType(this Type type)
		{
			return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		public static bool ContainsIgnoringWhitespace(string stringToSearch, string value)
		{
			stringToSearch = ConduitUtilities.StripWhiteSpace(stringToSearch);
			value = ConduitUtilities.StripWhiteSpace(value);
			return stringToSearch.Contains(value);
		}

		internal static object GetTypedParameterValue(ParameterInfo formalParameter, object parameterValue)
		{
			return ConduitUtilities.GetTypedParameterValue(formalParameter.ParameterType, parameterValue);
		}

		internal static object GetTypedParameterValue(Type parameterType, object parameterValue)
		{
			if (parameterValue == null)
			{
				return null;
			}
			Type type = parameterType;
			if (type.IsNullableType())
			{
				type = Nullable.GetUnderlyingType(type);
				if (type == null)
				{
					VLog.E(string.Format("Got null underlying type for nullable parameter of type {0}", parameterType), null);
					return null;
				}
			}
			if (type == typeof(string))
			{
				return parameterValue.ToString();
			}
			if (type.IsEnum)
			{
				try
				{
					return Enum.Parse(type, ConduitUtilities.SanitizeString(parameterValue.ToString()), true);
				}
				catch (Exception arg)
				{
					VLog.E(string.Format("Parameter value '{0}' could not be cast to enum\nEnum Type: {1}\n{2}", parameterValue, type.FullName, arg), null);
					throw;
				}
			}
			object result;
			try
			{
				result = Convert.ChangeType(parameterValue, type);
			}
			catch (Exception arg2)
			{
				VLog.E(string.Format("Nullable parameter value '{0}' could not be cast to {1}\n{2}", parameterValue, type.FullName, arg2), null);
				result = null;
			}
			return result;
		}

		public static string GetEntityEnumName(string entityRole)
		{
			return ConduitUtilities.SanitizeName(entityRole);
		}

		public static string GetEntityEnumValue(string entityValue)
		{
			return ConduitUtilities.SanitizeString(entityValue);
		}

		public static string SanitizeName(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return string.Empty;
			}
			string text = ConduitUtilities.SanitizeString(input);
			return text[0].ToString().ToUpper() + text.Substring(1);
		}

		public static string SanitizeString(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return string.Empty;
			}
			string text = Regex.Replace(input, "[^\\w_-]", "");
			if (Regex.IsMatch(text[0].ToString(), "^\\d$"))
			{
				text = "N" + text;
			}
			return text;
		}

		private static string StripWhiteSpace(string input)
		{
			if (!string.IsNullOrEmpty(input))
			{
				return input.Replace(" ", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty);
			}
			return string.Empty;
		}

		private static readonly Regex UnderscoreSplitter = new Regex("(\\B[A-Z])", RegexOptions.Compiled);

		public delegate void ProgressDelegate(string status, float progress);
	}
}
