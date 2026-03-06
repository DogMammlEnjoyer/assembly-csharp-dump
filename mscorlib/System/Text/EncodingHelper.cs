using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;

namespace System.Text
{
	internal static class EncodingHelper
	{
		internal static Encoding UTF8Unmarked
		{
			get
			{
				if (EncodingHelper.utf8EncodingWithoutMarkers == null)
				{
					object obj = EncodingHelper.lockobj;
					lock (obj)
					{
						if (EncodingHelper.utf8EncodingWithoutMarkers == null)
						{
							EncodingHelper.utf8EncodingWithoutMarkers = new UTF8Encoding(false, false);
							EncodingHelper.utf8EncodingWithoutMarkers.setReadOnly(true);
						}
					}
				}
				return EncodingHelper.utf8EncodingWithoutMarkers;
			}
		}

		internal static Encoding UTF8UnmarkedUnsafe
		{
			get
			{
				if (EncodingHelper.utf8EncodingUnsafe == null)
				{
					object obj = EncodingHelper.lockobj;
					lock (obj)
					{
						if (EncodingHelper.utf8EncodingUnsafe == null)
						{
							EncodingHelper.utf8EncodingUnsafe = new UTF8Encoding(false, false);
							EncodingHelper.utf8EncodingUnsafe.setReadOnly(false);
							EncodingHelper.utf8EncodingUnsafe.DecoderFallback = new DecoderReplacementFallback(string.Empty);
							EncodingHelper.utf8EncodingUnsafe.setReadOnly(true);
						}
					}
				}
				return EncodingHelper.utf8EncodingUnsafe;
			}
		}

		internal static Encoding BigEndianUTF32
		{
			get
			{
				if (EncodingHelper.bigEndianUTF32Encoding == null)
				{
					object obj = EncodingHelper.lockobj;
					lock (obj)
					{
						if (EncodingHelper.bigEndianUTF32Encoding == null)
						{
							EncodingHelper.bigEndianUTF32Encoding = new UTF32Encoding(true, true);
							EncodingHelper.bigEndianUTF32Encoding.setReadOnly(true);
						}
					}
				}
				return EncodingHelper.bigEndianUTF32Encoding;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string InternalCodePage(ref int code_page);

		internal static Encoding GetDefaultEncoding()
		{
			Encoding result = null;
			int num = 1;
			string name = EncodingHelper.InternalCodePage(ref num);
			try
			{
				if (num == -1)
				{
					result = Encoding.GetEncoding(name);
				}
				else
				{
					num &= 268435455;
					switch (num)
					{
					case 1:
						num = 20127;
						break;
					case 2:
						num = 65007;
						break;
					case 3:
						num = 65001;
						break;
					case 4:
						num = 1200;
						break;
					case 5:
						num = 1201;
						break;
					case 6:
						num = 1252;
						break;
					}
					result = Encoding.GetEncoding(num);
				}
			}
			catch (NotSupportedException)
			{
				result = EncodingHelper.UTF8Unmarked;
			}
			catch (ArgumentException)
			{
				result = EncodingHelper.UTF8Unmarked;
			}
			return result;
		}

		internal static object InvokeI18N(string name, params object[] args)
		{
			object obj = EncodingHelper.lockobj;
			object result;
			lock (obj)
			{
				if (EncodingHelper.i18nDisabled)
				{
					result = null;
				}
				else
				{
					if (EncodingHelper.i18nAssembly == null)
					{
						try
						{
							try
							{
								EncodingHelper.i18nAssembly = Assembly.Load("I18N, Version=4.0.0.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756");
							}
							catch (NotImplementedException)
							{
								EncodingHelper.i18nDisabled = true;
								return null;
							}
							if (EncodingHelper.i18nAssembly == null)
							{
								return null;
							}
						}
						catch (SystemException)
						{
							return null;
						}
					}
					Type type;
					try
					{
						type = EncodingHelper.i18nAssembly.GetType("I18N.Common.Manager");
					}
					catch (NotImplementedException)
					{
						EncodingHelper.i18nDisabled = true;
						return null;
					}
					if (type == null)
					{
						result = null;
					}
					else
					{
						object obj2;
						try
						{
							obj2 = type.InvokeMember("PrimaryManager", BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty, null, null, null, null, null, null);
							if (obj2 == null)
							{
								return null;
							}
						}
						catch (MissingMethodException)
						{
							return null;
						}
						catch (SecurityException)
						{
							return null;
						}
						catch (NotImplementedException)
						{
							EncodingHelper.i18nDisabled = true;
							return null;
						}
						try
						{
							result = type.InvokeMember(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod, null, obj2, args, null, null, null);
						}
						catch (MissingMethodException)
						{
							result = null;
						}
						catch (SecurityException)
						{
							result = null;
						}
					}
				}
			}
			return result;
		}

		private static volatile Encoding utf8EncodingWithoutMarkers;

		private static volatile Encoding utf8EncodingUnsafe;

		private static volatile Encoding bigEndianUTF32Encoding;

		private static readonly object lockobj = new object();

		private static Assembly i18nAssembly;

		private static bool i18nDisabled;
	}
}
