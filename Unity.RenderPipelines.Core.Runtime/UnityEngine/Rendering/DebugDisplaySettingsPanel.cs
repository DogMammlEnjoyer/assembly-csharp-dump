using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnityEngine.Rendering
{
	public abstract class DebugDisplaySettingsPanel : IDebugDisplaySettingsPanelDisposable, IDebugDisplaySettingsPanel, IDisposable
	{
		public virtual string PanelName
		{
			get
			{
				DisplayInfoAttribute displayInfo = this.m_DisplayInfo;
				return ((displayInfo != null) ? displayInfo.name : null) ?? string.Empty;
			}
		}

		public virtual int Order
		{
			get
			{
				DisplayInfoAttribute displayInfo = this.m_DisplayInfo;
				if (displayInfo == null)
				{
					return 0;
				}
				return displayInfo.order;
			}
		}

		public DebugUI.Widget[] Widgets
		{
			get
			{
				return this.m_Widgets.ToArray();
			}
		}

		public virtual DebugUI.Flags Flags
		{
			get
			{
				return DebugUI.Flags.None;
			}
		}

		protected void AddWidget(DebugUI.Widget widget)
		{
			if (widget == null)
			{
				throw new ArgumentNullException("widget");
			}
			this.m_Widgets.Add(widget);
		}

		protected void Clear()
		{
			this.m_Widgets.Clear();
		}

		public virtual void Dispose()
		{
			this.Clear();
		}

		protected DebugDisplaySettingsPanel()
		{
			this.m_DisplayInfo = base.GetType().GetCustomAttribute<DisplayInfoAttribute>();
			if (this.m_DisplayInfo == null)
			{
				Debug.Log(string.Format("Type {0} should specify the attribute {1}", base.GetType(), "DisplayInfoAttribute"));
			}
		}

		private readonly List<DebugUI.Widget> m_Widgets = new List<DebugUI.Widget>();

		private readonly DisplayInfoAttribute m_DisplayInfo;
	}
}
