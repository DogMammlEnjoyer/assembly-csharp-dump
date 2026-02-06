using System;

namespace Fusion
{
	public struct RenderTimeline
	{
		public static void GetRenderBuffers(NetworkBehaviour behaviour, out NetworkBehaviourBuffer from, out NetworkBehaviourBuffer to, out float alpha)
		{
			NetworkObjectMeta meta = behaviour.Object.Meta;
			Simulation simulation = behaviour.Object.Runner.Simulation;
			RenderTimeframe renderTimeframe = meta.Instance.RenderTimeframe;
			RenderSource renderSource = meta.Instance.RenderSource;
			InterpolationParams @params;
			NetworkObjectHeaderSnapshot networkObjectHeaderSnapshot;
			NetworkObjectHeaderSnapshot networkObjectHeaderSnapshot2;
			for (;;)
			{
				bool flag = renderTimeframe == RenderTimeframe.Local || simulation.IsServer;
				if (flag)
				{
					break;
				}
				bool flag2 = renderSource != RenderSource.Latest;
				if (flag2)
				{
					@params = meta.Timeline.Params;
					bool flag3 = meta.TryFindSnapshot(@params.From, out networkObjectHeaderSnapshot) && meta.TryFindSnapshot(@params.To, out networkObjectHeaderSnapshot2);
					if (flag3)
					{
						goto Block_7;
					}
				}
				bool hasSnapshots = meta.HasSnapshots;
				if (hasSnapshots)
				{
					goto Block_10;
				}
				renderTimeframe = RenderTimeframe.Local;
			}
			to._ptr = behaviour.Ptr;
			to._tick = simulation.Tick;
			to._length = behaviour.WordCount;
			from._ptr = meta.Previous.GetBehaviourPtr(behaviour);
			from._tick = simulation.TickPrevious;
			from._length = behaviour.WordCount;
			alpha = simulation.LocalAlpha;
			RenderSource renderSource2 = renderSource;
			RenderSource renderSource3 = renderSource2;
			if (renderSource3 != RenderSource.From)
			{
				if (renderSource3 - RenderSource.To <= 1)
				{
					from = to;
					alpha = 1f;
				}
			}
			else
			{
				to = from;
				alpha = 0f;
			}
			return;
			Block_7:
			from._ptr = networkObjectHeaderSnapshot.GetBehaviourPtr(behaviour);
			from._tick = @params.From;
			from._length = behaviour.WordCount;
			to._ptr = networkObjectHeaderSnapshot2.GetBehaviourPtr(behaviour);
			to._tick = @params.To;
			to._length = behaviour.WordCount;
			alpha = @params.Alpha;
			bool flag4 = renderSource == RenderSource.From;
			if (flag4)
			{
				to = from;
				alpha = 0f;
			}
			else
			{
				bool flag5 = renderSource == RenderSource.To;
				if (flag5)
				{
					from = to;
					alpha = 1f;
				}
			}
			return;
			Block_10:
			to._ptr = meta.SnapshotLatest.GetBehaviourPtr(behaviour);
			to._tick = meta.SnapshotLatest.Tick;
			to._length = behaviour.WordCount;
			from = to;
			alpha = 1f;
		}
	}
}
