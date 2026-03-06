using System;
using UnityEngine;

namespace Meta.XR.MultiplayerBlocks.Shared
{
	internal static class CustomMatchmakingUtils
	{
		internal static MatchInfo DecodeMatchInfoWithStruct(string matchInfoString)
		{
			if (string.IsNullOrEmpty(matchInfoString))
			{
				throw new InvalidOperationException("matchInfoString can not be null or empty");
			}
			MatchInfo result;
			try
			{
				result = SerializationUtils.DeserializeFromString<MatchInfo>(matchInfoString);
			}
			catch (Exception arg)
			{
				Debug.LogWarning(string.Format("Failed to decode the matchInfo from string {0}, {1}", matchInfoString, arg));
				result = default(MatchInfo);
			}
			return result;
		}

		internal static string EncodeMatchInfoWithStruct(string roomId, string roomPassword = null, string extra = null)
		{
			if (string.IsNullOrEmpty(roomId))
			{
				throw new InvalidOperationException("roomId can not be null or empty");
			}
			return SerializationUtils.SerializeToString<MatchInfo>(new MatchInfo
			{
				RoomId = roomId,
				RoomPassword = roomPassword,
				Extra = extra
			});
		}

		public static ValueTuple<string, string> ExtractMatchInfoFromSessionId(string matchSessionId)
		{
			if (string.IsNullOrEmpty(matchSessionId))
			{
				throw new InvalidOperationException("matchSessionId can not be null or empty");
			}
			if (!matchSessionId.Contains(":"))
			{
				return new ValueTuple<string, string>(matchSessionId, null);
			}
			string[] array = matchSessionId.Split(':', StringSplitOptions.None);
			int num = array.Length;
			ValueTuple<string, string> valueTuple;
			if (num != 0)
			{
				if (num != 1)
				{
					valueTuple = new ValueTuple<string, string>(array[0], array[1]);
				}
				else
				{
					valueTuple = new ValueTuple<string, string>(array[0], null);
				}
			}
			else
			{
				valueTuple = new ValueTuple<string, string>(null, null);
			}
			ValueTuple<string, string> valueTuple2 = valueTuple;
			if (valueTuple2.Item1 == string.Empty)
			{
				valueTuple2.Item1 = null;
			}
			if (valueTuple2.Item2 == string.Empty)
			{
				valueTuple2.Item2 = null;
			}
			return valueTuple2;
		}

		public static string EncodeMatchInfoToSessionId(string roomId, string roomPassword = null)
		{
			if (string.IsNullOrEmpty(roomId))
			{
				throw new InvalidOperationException("roomId can not be null or empty");
			}
			if (!string.IsNullOrEmpty(roomPassword))
			{
				return roomId + ":" + roomPassword;
			}
			return roomId;
		}
	}
}
