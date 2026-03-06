using System;

namespace Oculus.Platform
{
	public class ChallengeOptions
	{
		public ChallengeOptions()
		{
			this.Handle = CAPI.ovr_ChallengeOptions_Create();
		}

		public void SetDescription(string value)
		{
			CAPI.ovr_ChallengeOptions_SetDescription(this.Handle, value);
		}

		public void SetEndDate(DateTime value)
		{
			CAPI.ovr_ChallengeOptions_SetEndDate(this.Handle, value);
		}

		public void SetIncludeActiveChallenges(bool value)
		{
			CAPI.ovr_ChallengeOptions_SetIncludeActiveChallenges(this.Handle, value);
		}

		public void SetIncludeFutureChallenges(bool value)
		{
			CAPI.ovr_ChallengeOptions_SetIncludeFutureChallenges(this.Handle, value);
		}

		public void SetIncludePastChallenges(bool value)
		{
			CAPI.ovr_ChallengeOptions_SetIncludePastChallenges(this.Handle, value);
		}

		public void SetLeaderboardName(string value)
		{
			CAPI.ovr_ChallengeOptions_SetLeaderboardName(this.Handle, value);
		}

		public void SetStartDate(DateTime value)
		{
			CAPI.ovr_ChallengeOptions_SetStartDate(this.Handle, value);
		}

		public void SetTitle(string value)
		{
			CAPI.ovr_ChallengeOptions_SetTitle(this.Handle, value);
		}

		public void SetViewerFilter(ChallengeViewerFilter value)
		{
			CAPI.ovr_ChallengeOptions_SetViewerFilter(this.Handle, value);
		}

		public void SetVisibility(ChallengeVisibility value)
		{
			CAPI.ovr_ChallengeOptions_SetVisibility(this.Handle, value);
		}

		public static explicit operator IntPtr(ChallengeOptions options)
		{
			if (options == null)
			{
				return IntPtr.Zero;
			}
			return options.Handle;
		}

		~ChallengeOptions()
		{
			CAPI.ovr_ChallengeOptions_Destroy(this.Handle);
		}

		private IntPtr Handle;
	}
}
