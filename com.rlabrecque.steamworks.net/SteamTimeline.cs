using System;

namespace Steamworks
{
	public static class SteamTimeline
	{
		public static void SetTimelineStateDescription(string pchDescription, float flTimeDelta)
		{
			InteropHelp.TestIfAvailableClient();
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchDescription))
			{
				NativeMethods.ISteamTimeline_SetTimelineStateDescription(CSteamAPIContext.GetSteamTimeline(), utf8StringHandle, flTimeDelta);
			}
		}

		public static void ClearTimelineStateDescription(float flTimeDelta)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamTimeline_ClearTimelineStateDescription(CSteamAPIContext.GetSteamTimeline(), flTimeDelta);
		}

		public static void AddTimelineEvent(string pchIcon, string pchTitle, string pchDescription, uint unPriority, float flStartOffsetSeconds, float flDurationSeconds, ETimelineEventClipPriority ePossibleClip)
		{
			InteropHelp.TestIfAvailableClient();
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchIcon))
			{
				using (InteropHelp.UTF8StringHandle utf8StringHandle2 = new InteropHelp.UTF8StringHandle(pchTitle))
				{
					using (InteropHelp.UTF8StringHandle utf8StringHandle3 = new InteropHelp.UTF8StringHandle(pchDescription))
					{
						NativeMethods.ISteamTimeline_AddTimelineEvent(CSteamAPIContext.GetSteamTimeline(), utf8StringHandle, utf8StringHandle2, utf8StringHandle3, unPriority, flStartOffsetSeconds, flDurationSeconds, ePossibleClip);
					}
				}
			}
		}

		public static void SetTimelineGameMode(ETimelineGameMode eMode)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamTimeline_SetTimelineGameMode(CSteamAPIContext.GetSteamTimeline(), eMode);
		}
	}
}
