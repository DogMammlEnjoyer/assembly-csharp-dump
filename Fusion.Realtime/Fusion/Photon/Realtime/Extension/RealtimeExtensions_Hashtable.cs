using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;

namespace Fusion.Photon.Realtime.Extension
{
	internal static class RealtimeExtensions_Hashtable
	{
		public static Dictionary<string, SessionProperty> ConvertToDictionaryProperty(this Hashtable customProperties)
		{
			Dictionary<string, SessionProperty> dictionary = new Dictionary<string, SessionProperty>();
			foreach (DictionaryEntry dictionaryEntry in customProperties)
			{
				string text = dictionaryEntry.Key as string;
				bool flag = text != null && SessionProperty.Support(dictionaryEntry.Value);
				if (flag)
				{
					dictionary[text] = SessionProperty.Convert(dictionaryEntry.Value);
				}
			}
			return dictionary;
		}

		public static Hashtable ConvertToHashtable(this Dictionary<string, SessionProperty> properties)
		{
			Hashtable hashtable = new Hashtable();
			foreach (KeyValuePair<string, SessionProperty> keyValuePair in properties)
			{
				bool flag = keyValuePair.Key != null && keyValuePair.Value != null;
				if (flag)
				{
					hashtable[keyValuePair.Key] = keyValuePair.Value.PropertyValue;
				}
			}
			return hashtable;
		}

		public static int CalculateTotalSize(this Hashtable hashtable)
		{
			RealtimeExtensions_Hashtable.buffer.Position = 0;
			RealtimeExtensions_Hashtable.protocol.Serialize(RealtimeExtensions_Hashtable.buffer, hashtable, true);
			return RealtimeExtensions_Hashtable.buffer.Position;
		}

		private static readonly StreamBuffer buffer = new StreamBuffer(1024);

		private static readonly Protocol18 protocol = new Protocol18();
	}
}
