using System;

namespace Oculus.Platform.Models
{
	public class LivestreamingVideoStats
	{
		public LivestreamingVideoStats(IntPtr o)
		{
			this.CommentCount = CAPI.ovr_LivestreamingVideoStats_GetCommentCount(o);
			this.ReactionCount = CAPI.ovr_LivestreamingVideoStats_GetReactionCount(o);
			this.TotalViews = CAPI.ovr_LivestreamingVideoStats_GetTotalViews(o);
		}

		public readonly int CommentCount;

		public readonly int ReactionCount;

		public readonly string TotalViews;
	}
}
