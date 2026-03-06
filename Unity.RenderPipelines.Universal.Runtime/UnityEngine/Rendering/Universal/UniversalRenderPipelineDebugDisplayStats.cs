using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.Universal
{
	internal class UniversalRenderPipelineDebugDisplayStats : DebugDisplayStats<URPProfileId>
	{
		public override void EnableProfilingRecorders()
		{
			this.m_RecordedSamplers = base.GetProfilerIdsToDisplay();
		}

		public override void DisableProfilingRecorders()
		{
			foreach (URPProfileId marker in this.m_RecordedSamplers)
			{
				ProfilingSampler.Get<URPProfileId>(marker).enableRecording = false;
			}
			this.m_RecordedSamplers.Clear();
		}

		public override void RegisterDebugUI(List<DebugUI.Widget> list)
		{
			this.m_DebugFrameTiming.RegisterDebugUI(list);
			DebugUI.Foldout foldout = new DebugUI.Foldout
			{
				displayName = "Detailed Stats",
				opened = false,
				children = 
				{
					new DebugUI.BoolField
					{
						displayName = "Update every second with average",
						getter = (() => this.averageProfilerTimingsOverASecond),
						setter = delegate(bool value)
						{
							this.averageProfilerTimingsOverASecond = value;
						}
					},
					new DebugUI.BoolField
					{
						displayName = "Hide empty scopes",
						tooltip = "Hide profiling scopes where elapsed time in each category is zero",
						getter = (() => this.hideEmptyScopes),
						setter = delegate(bool value)
						{
							this.hideEmptyScopes = value;
						}
					}
				}
			};
			foldout.children.Add(base.BuildDetailedStatsList("Profiling Scopes", this.m_RecordedSamplers));
			list.Add(foldout);
		}

		public override void Update()
		{
			this.m_DebugFrameTiming.UpdateFrameTiming();
			base.UpdateDetailedStats(this.m_RecordedSamplers);
		}

		private DebugFrameTiming m_DebugFrameTiming = new DebugFrameTiming();

		private List<URPProfileId> m_RecordedSamplers = new List<URPProfileId>();
	}
}
