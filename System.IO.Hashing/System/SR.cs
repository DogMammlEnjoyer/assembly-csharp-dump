using System;
using System.Resources;
using FxResources.System.IO.Hashing;

namespace System
{
	internal static class SR
	{
		private static bool GetUsingResourceKeysSwitchValue()
		{
			bool flag;
			return AppContext.TryGetSwitch("System.Resources.UseSystemResourceKeys", out flag) && flag;
		}

		internal static bool UsingResourceKeys()
		{
			return System.SR.s_usingResourceKeys;
		}

		private static string GetResourceString(string resourceKey)
		{
			if (System.SR.UsingResourceKeys())
			{
				return resourceKey;
			}
			string result = null;
			try
			{
				result = System.SR.ResourceManager.GetString(resourceKey);
			}
			catch (MissingManifestResourceException)
			{
			}
			return result;
		}

		private static string GetResourceString(string resourceKey, string defaultString)
		{
			string resourceString = System.SR.GetResourceString(resourceKey);
			if (!(resourceKey == resourceString) && resourceString != null)
			{
				return resourceString;
			}
			return defaultString;
		}

		internal static string Format(string resourceFormat, object p1)
		{
			if (System.SR.UsingResourceKeys())
			{
				return string.Join(", ", new object[]
				{
					resourceFormat,
					p1
				});
			}
			return string.Format(resourceFormat, p1);
		}

		internal static string Format(string resourceFormat, object p1, object p2)
		{
			if (System.SR.UsingResourceKeys())
			{
				return string.Join(", ", new object[]
				{
					resourceFormat,
					p1,
					p2
				});
			}
			return string.Format(resourceFormat, p1, p2);
		}

		internal static string Format(string resourceFormat, object p1, object p2, object p3)
		{
			if (System.SR.UsingResourceKeys())
			{
				return string.Join(", ", new object[]
				{
					resourceFormat,
					p1,
					p2,
					p3
				});
			}
			return string.Format(resourceFormat, p1, p2, p3);
		}

		internal static string Format(string resourceFormat, params object[] args)
		{
			if (args == null)
			{
				return resourceFormat;
			}
			if (System.SR.UsingResourceKeys())
			{
				return resourceFormat + ", " + string.Join(", ", args);
			}
			return string.Format(resourceFormat, args);
		}

		internal static string Format(IFormatProvider provider, string resourceFormat, object p1)
		{
			if (System.SR.UsingResourceKeys())
			{
				return string.Join(", ", new object[]
				{
					resourceFormat,
					p1
				});
			}
			return string.Format(provider, resourceFormat, p1);
		}

		internal static string Format(IFormatProvider provider, string resourceFormat, object p1, object p2)
		{
			if (System.SR.UsingResourceKeys())
			{
				return string.Join(", ", new object[]
				{
					resourceFormat,
					p1,
					p2
				});
			}
			return string.Format(provider, resourceFormat, p1, p2);
		}

		internal static string Format(IFormatProvider provider, string resourceFormat, object p1, object p2, object p3)
		{
			if (System.SR.UsingResourceKeys())
			{
				return string.Join(", ", new object[]
				{
					resourceFormat,
					p1,
					p2,
					p3
				});
			}
			return string.Format(provider, resourceFormat, p1, p2, p3);
		}

		internal static string Format(IFormatProvider provider, string resourceFormat, params object[] args)
		{
			if (args == null)
			{
				return resourceFormat;
			}
			if (System.SR.UsingResourceKeys())
			{
				return resourceFormat + ", " + string.Join(", ", args);
			}
			return string.Format(provider, resourceFormat, args);
		}

		internal static ResourceManager ResourceManager
		{
			get
			{
				ResourceManager result;
				if ((result = System.SR.s_resourceManager) == null)
				{
					result = (System.SR.s_resourceManager = new ResourceManager(typeof(FxResources.System.IO.Hashing.SR)));
				}
				return result;
			}
		}

		internal static string Argument_DestinationTooShort
		{
			get
			{
				return System.SR.GetResourceString("Argument_DestinationTooShort");
			}
		}

		internal static string NotSupported_GetHashCode
		{
			get
			{
				return System.SR.GetResourceString("NotSupported_GetHashCode");
			}
		}

		private static readonly bool s_usingResourceKeys = System.SR.GetUsingResourceKeysSwitchValue();

		private static ResourceManager s_resourceManager;
	}
}
