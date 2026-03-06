using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace PlayFab.Internal
{
	public static class PlayFabUtil
	{
		public static string timeStamp
		{
			get
			{
				return DateTime.Now.ToString(PlayFabUtil._defaultDateTimeFormats[9]);
			}
		}

		public static string utcTimeStamp
		{
			get
			{
				return DateTime.UtcNow.ToString(PlayFabUtil._defaultDateTimeFormats[2]);
			}
		}

		public static string Format(string text, params object[] args)
		{
			if (args.Length == 0)
			{
				return text;
			}
			return string.Format(text, args);
		}

		public static string ReadAllFileText(string filename)
		{
			if (!File.Exists(filename))
			{
				return string.Empty;
			}
			if (PlayFabUtil._sb == null)
			{
				PlayFabUtil._sb = new StringBuilder();
			}
			PlayFabUtil._sb.Length = 0;
			using (FileStream fileStream = new FileStream(filename, FileMode.Open))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					while (binaryReader.BaseStream.Position != binaryReader.BaseStream.Length)
					{
						PlayFabUtil._sb.Append(binaryReader.ReadChar());
					}
				}
			}
			return PlayFabUtil._sb.ToString();
		}

		public static T TryEnumParse<T>(string value, T defaultValue)
		{
			T result;
			try
			{
				result = (T)((object)Enum.Parse(typeof(T), value));
			}
			catch (InvalidCastException)
			{
				result = defaultValue;
			}
			catch (Exception ex)
			{
				Debug.LogError("Enum cast failed with unknown error: " + ex.Message);
				result = defaultValue;
			}
			return result;
		}

		internal static string GetLocalSettingsFileProperty(string propertyKey)
		{
			string text = null;
			string text2 = Path.Combine(Directory.GetCurrentDirectory(), PlayFabUtil._localSettingsFileName);
			if (File.Exists(text2))
			{
				text = PlayFabUtil.ReadAllFileText(text2);
			}
			else
			{
				string text3 = Path.Combine(Path.GetTempPath(), PlayFabUtil._localSettingsFileName);
				if (File.Exists(text3))
				{
					text = PlayFabUtil.ReadAllFileText(text3);
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				Dictionary<string, object> dictionary = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer, "").DeserializeObject<Dictionary<string, object>>(text);
				try
				{
					object obj;
					if (dictionary.TryGetValue(propertyKey, out obj))
					{
						return (obj == null) ? null : obj.ToString();
					}
					return null;
				}
				catch (KeyNotFoundException)
				{
					return string.Empty;
				}
			}
			return string.Empty;
		}

		private static string _localSettingsFileName = "playfab.local.settings.json";

		public static readonly string[] _defaultDateTimeFormats = new string[]
		{
			"yyyy-MM-ddTHH:mm:ss.FFFFFFZ",
			"yyyy-MM-ddTHH:mm:ss.FFFFZ",
			"yyyy-MM-ddTHH:mm:ss.FFFZ",
			"yyyy-MM-ddTHH:mm:ss.FFZ",
			"yyyy-MM-ddTHH:mm:ssZ",
			"yyyy-MM-dd HH:mm:ssZ",
			"yyyy-MM-dd HH:mm:ss.FFFFFF",
			"yyyy-MM-dd HH:mm:ss.FFFF",
			"yyyy-MM-dd HH:mm:ss.FFF",
			"yyyy-MM-dd HH:mm:ss.FF",
			"yyyy-MM-dd HH:mm:ss",
			"yyyy-MM-dd HH:mm.ss.FFFF",
			"yyyy-MM-dd HH:mm.ss.FFF",
			"yyyy-MM-dd HH:mm.ss.FF",
			"yyyy-MM-dd HH:mm.ss"
		};

		public const int DEFAULT_UTC_OUTPUT_INDEX = 2;

		public const int DEFAULT_LOCAL_OUTPUT_INDEX = 9;

		public static DateTimeStyles DateTimeStyles = DateTimeStyles.RoundtripKind;

		[ThreadStatic]
		private static StringBuilder _sb;
	}
}
