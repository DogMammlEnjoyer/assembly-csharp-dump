using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Fusion
{
	internal static class FusionRealtimeProxy
	{
		[DebuggerStepThrough]
		internal static Task<List<RegionInfo>> GetEnabledRegions(string appId, CancellationToken cancellationToken)
		{
			FusionRealtimeProxy.<GetEnabledRegions>d__3 <GetEnabledRegions>d__ = new FusionRealtimeProxy.<GetEnabledRegions>d__3();
			<GetEnabledRegions>d__.<>t__builder = AsyncTaskMethodBuilder<List<RegionInfo>>.Create();
			<GetEnabledRegions>d__.appId = appId;
			<GetEnabledRegions>d__.cancellationToken = cancellationToken;
			<GetEnabledRegions>d__.<>1__state = -1;
			<GetEnabledRegions>d__.<>t__builder.Start<FusionRealtimeProxy.<GetEnabledRegions>d__3>(ref <GetEnabledRegions>d__);
			return <GetEnabledRegions>d__.<>t__builder.Task;
		}

		private const float REGION_INFO_CACHE_TIME = 10f;

		private static float _lastRegionRequestTime;

		private static List<RegionInfo> _cachedRegionInfo;
	}
}
