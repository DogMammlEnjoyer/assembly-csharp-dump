using System;
using System.Data.SqlClient;
using System.Reflection;

namespace System.Data.Common
{
	internal static class DbConnectionStringBuilderUtil
	{
		internal static bool ConvertToBoolean(object value)
		{
			string text = value as string;
			if (text == null)
			{
				bool result;
				try
				{
					result = Convert.ToBoolean(value);
				}
				catch (InvalidCastException innerException)
				{
					throw ADP.ConvertFailed(value.GetType(), typeof(bool), innerException);
				}
				return result;
			}
			if (StringComparer.OrdinalIgnoreCase.Equals(text, "true") || StringComparer.OrdinalIgnoreCase.Equals(text, "yes"))
			{
				return true;
			}
			if (StringComparer.OrdinalIgnoreCase.Equals(text, "false") || StringComparer.OrdinalIgnoreCase.Equals(text, "no"))
			{
				return false;
			}
			string x = text.Trim();
			return StringComparer.OrdinalIgnoreCase.Equals(x, "true") || StringComparer.OrdinalIgnoreCase.Equals(x, "yes") || (!StringComparer.OrdinalIgnoreCase.Equals(x, "false") && !StringComparer.OrdinalIgnoreCase.Equals(x, "no") && bool.Parse(text));
		}

		internal static bool ConvertToIntegratedSecurity(object value)
		{
			string text = value as string;
			if (text == null)
			{
				bool result;
				try
				{
					result = Convert.ToBoolean(value);
				}
				catch (InvalidCastException innerException)
				{
					throw ADP.ConvertFailed(value.GetType(), typeof(bool), innerException);
				}
				return result;
			}
			if (StringComparer.OrdinalIgnoreCase.Equals(text, "sspi") || StringComparer.OrdinalIgnoreCase.Equals(text, "true") || StringComparer.OrdinalIgnoreCase.Equals(text, "yes"))
			{
				return true;
			}
			if (StringComparer.OrdinalIgnoreCase.Equals(text, "false") || StringComparer.OrdinalIgnoreCase.Equals(text, "no"))
			{
				return false;
			}
			string x = text.Trim();
			return StringComparer.OrdinalIgnoreCase.Equals(x, "sspi") || StringComparer.OrdinalIgnoreCase.Equals(x, "true") || StringComparer.OrdinalIgnoreCase.Equals(x, "yes") || (!StringComparer.OrdinalIgnoreCase.Equals(x, "false") && !StringComparer.OrdinalIgnoreCase.Equals(x, "no") && bool.Parse(text));
		}

		internal static int ConvertToInt32(object value)
		{
			int result;
			try
			{
				result = Convert.ToInt32(value);
			}
			catch (InvalidCastException innerException)
			{
				throw ADP.ConvertFailed(value.GetType(), typeof(int), innerException);
			}
			return result;
		}

		internal static string ConvertToString(object value)
		{
			string result;
			try
			{
				result = Convert.ToString(value);
			}
			catch (InvalidCastException innerException)
			{
				throw ADP.ConvertFailed(value.GetType(), typeof(string), innerException);
			}
			return result;
		}

		internal static bool TryConvertToApplicationIntent(string value, out ApplicationIntent result)
		{
			if (StringComparer.OrdinalIgnoreCase.Equals(value, "ReadOnly"))
			{
				result = ApplicationIntent.ReadOnly;
				return true;
			}
			if (StringComparer.OrdinalIgnoreCase.Equals(value, "ReadWrite"))
			{
				result = ApplicationIntent.ReadWrite;
				return true;
			}
			result = ApplicationIntent.ReadWrite;
			return false;
		}

		internal static bool IsValidApplicationIntentValue(ApplicationIntent value)
		{
			return value == ApplicationIntent.ReadOnly || value == ApplicationIntent.ReadWrite;
		}

		internal static string ApplicationIntentToString(ApplicationIntent value)
		{
			if (value == ApplicationIntent.ReadOnly)
			{
				return "ReadOnly";
			}
			return "ReadWrite";
		}

		internal static ApplicationIntent ConvertToApplicationIntent(string keyword, object value)
		{
			string text = value as string;
			if (text != null)
			{
				ApplicationIntent result;
				if (DbConnectionStringBuilderUtil.TryConvertToApplicationIntent(text, out result))
				{
					return result;
				}
				text = text.Trim();
				if (DbConnectionStringBuilderUtil.TryConvertToApplicationIntent(text, out result))
				{
					return result;
				}
				throw ADP.InvalidConnectionOptionValue(keyword);
			}
			else
			{
				ApplicationIntent applicationIntent;
				if (value is ApplicationIntent)
				{
					applicationIntent = (ApplicationIntent)value;
				}
				else
				{
					if (value.GetType().GetTypeInfo().IsEnum)
					{
						throw ADP.ConvertFailed(value.GetType(), typeof(ApplicationIntent), null);
					}
					try
					{
						applicationIntent = (ApplicationIntent)Enum.ToObject(typeof(ApplicationIntent), value);
					}
					catch (ArgumentException innerException)
					{
						throw ADP.ConvertFailed(value.GetType(), typeof(ApplicationIntent), innerException);
					}
				}
				if (DbConnectionStringBuilderUtil.IsValidApplicationIntentValue(applicationIntent))
				{
					return applicationIntent;
				}
				throw ADP.InvalidEnumerationValue(typeof(ApplicationIntent), (int)applicationIntent);
			}
		}

		private const string ApplicationIntentReadWriteString = "ReadWrite";

		private const string ApplicationIntentReadOnlyString = "ReadOnly";
	}
}
