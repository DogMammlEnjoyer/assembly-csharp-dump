using System;
using Modio.Unity.UI.Components.Localization;
using UnityEngine;

namespace Modio.Unity.UI
{
	internal static class StringFormat
	{
		public static string Bytes(StringFormatBytes format, long bytes, string custom = null, bool reducePrecision = false)
		{
			string result;
			switch (format)
			{
			case StringFormatBytes.Bytes:
				result = bytes.ToString();
				break;
			case StringFormatBytes.BytesComma:
				result = StringFormat.BytesComma(bytes);
				break;
			case StringFormatBytes.Suffix:
				result = StringFormat.BytesSuffix(bytes, reducePrecision);
				break;
			case StringFormatBytes.Custom:
				result = (string.IsNullOrEmpty(custom) ? bytes.ToString(ModioUILocalizationManager.CultureInfo) : bytes.ToString(custom, ModioUILocalizationManager.CultureInfo));
				break;
			default:
				result = bytes.ToString();
				break;
			}
			return result;
		}

		public static string BytesComma(long bytes)
		{
			return bytes.ToString("N0", ModioUILocalizationManager.CultureInfo);
		}

		public static string BytesSuffix(long bytes, bool reducePrecision = false)
		{
			int num = Mathf.Clamp((int)Math.Log((double)bytes, 1024.0), 1, StringFormat.BytesSuffixes.Length - 1);
			double num2 = (double)bytes / Math.Pow(1024.0, (double)num);
			string text = StringFormat.BytesSuffixes[num];
			text = (ModioUILocalizationManager.GetLocalizedText(StringFormat.BytesSuffixesLoc[num], false) ?? text);
			int num3 = reducePrecision ? Mathf.Max(0, 2 - (int)Mathf.Log10((float)bytes)) : 2;
			return num2.ToString(string.Format("F{0}", num3)) + " " + text;
		}

		public static string Kilo(StringFormatKilo format, long value, string custom = null)
		{
			string result;
			switch (format)
			{
			case StringFormatKilo.None:
				result = value.ToString();
				break;
			case StringFormatKilo.Comma:
				result = value.ToString("N0");
				break;
			case StringFormatKilo.Kilo:
				result = StringFormat.Kilo(value);
				break;
			case StringFormatKilo.Custom:
				result = (string.IsNullOrEmpty(custom) ? value.ToString() : value.ToString(custom));
				break;
			default:
				result = value.ToString();
				break;
			}
			return result;
		}

		public static string Kilo(long value)
		{
			if (value > 1000000000000L)
			{
				return ((double)value / 1000000000000.0).ToString("0.#T");
			}
			if (value > 100000000000L)
			{
				return ((double)value / 1000000000000.0).ToString("0.##T");
			}
			if (value > 10000000000L)
			{
				return ((double)value / 1000000000.0).ToString("0.#G");
			}
			if (value > 1000000000L)
			{
				return ((double)value / 1000000000.0).ToString("0.##G");
			}
			if (value > 100000000L)
			{
				return ((double)value / 1000000.0).ToString("0.#M");
			}
			if (value > 1000000L)
			{
				return ((double)value / 1000000.0).ToString("0.##M");
			}
			if (value > 100000L)
			{
				return ((double)value / 1000.0).ToString("0.#k");
			}
			if (value > 10000L)
			{
				return ((double)value / 1000.0).ToString("0.##k");
			}
			return value.ToString("#,0");
		}

		public const string BYTES_FORMAT_TOOLTIP = "Bytes: \"1048576\".\r\nBytesComma: \"1,048,576\".\r\nSuffix: \"1 MB\".";

		private static readonly string[] BytesSuffixes = new string[]
		{
			"B",
			"KB",
			"MB",
			"GB"
		};

		private static readonly string[] BytesSuffixesLoc = new string[]
		{
			"modio_unit_bytes",
			"modio_unit_kb",
			"modio_unit_mb",
			"modio_unit_gb"
		};

		public const string KILO_FORMAT_TOOLTIP = "None: \"10500\".\r\nComma: \"10,500\".\r\nKilo: \"10.5k\".";
	}
}
