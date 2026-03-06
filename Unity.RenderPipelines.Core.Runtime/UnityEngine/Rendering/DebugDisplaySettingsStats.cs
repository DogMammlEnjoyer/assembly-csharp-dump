using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering
{
	public class DebugDisplaySettingsStats<TProfileId> : IDebugDisplaySettingsData, IDebugDisplaySettingsQuery where TProfileId : Enum
	{
		public DebugDisplayStats<TProfileId> debugDisplayStats { get; }

		public DebugDisplaySettingsStats(DebugDisplayStats<TProfileId> debugDisplayStats)
		{
			this.debugDisplayStats = debugDisplayStats;
		}

		public bool AreAnySettingsActive
		{
			get
			{
				return false;
			}
		}

		public IDebugDisplaySettingsPanelDisposable CreatePanel()
		{
			return new DebugDisplaySettingsStats<TProfileId>.StatsPanel(this);
		}

		[DisplayInfo(name = "Display Stats", order = -2147483648)]
		private class StatsPanel : DebugDisplaySettingsPanel
		{
			public override DebugUI.Flags Flags
			{
				get
				{
					return DebugUI.Flags.RuntimeOnly;
				}
			}

			public StatsPanel(DebugDisplaySettingsStats<TProfileId> displaySettingsStats)
			{
				this.m_Data = displaySettingsStats;
				this.m_Data.debugDisplayStats.EnableProfilingRecorders();
				List<DebugUI.Widget> list = new List<DebugUI.Widget>();
				this.m_Data.debugDisplayStats.RegisterDebugUI(list);
				foreach (DebugUI.Widget widget in list)
				{
					base.AddWidget(widget);
				}
			}

			public override void Dispose()
			{
				this.m_Data.debugDisplayStats.DisableProfilingRecorders();
				base.Dispose();
			}

			private readonly DebugDisplaySettingsStats<TProfileId> m_Data;
		}
	}
}
