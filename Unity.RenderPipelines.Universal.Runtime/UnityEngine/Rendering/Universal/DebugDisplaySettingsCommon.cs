using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.Universal
{
	internal class DebugDisplaySettingsCommon : IDebugDisplaySettingsData, IDebugDisplaySettingsQuery
	{
		public bool AreAnySettingsActive
		{
			get
			{
				return false;
			}
		}

		public IDebugDisplaySettingsPanelDisposable CreatePanel()
		{
			return new DebugDisplaySettingsCommon.SettingsPanel();
		}

		[DisplayInfo(name = "Frequently Used", order = -1)]
		private class SettingsPanel : DebugDisplaySettingsPanel
		{
			public override DebugUI.Flags Flags
			{
				get
				{
					return DebugUI.Flags.FrequentlyUsed;
				}
			}

			public SettingsPanel()
			{
				base.AddWidget(new DebugUI.RuntimeDebugShadersMessageBox());
				DebugUI.Widget[] items = DebugManager.instance.GetItems(DebugUI.Flags.FrequentlyUsed);
				for (int i = 0; i < items.Length; i++)
				{
					DebugUI.Widget widget = items[i];
					DebugUI.Foldout foldout = widget as DebugUI.Foldout;
					if (foldout != null)
					{
						if (foldout.contextMenuItems == null)
						{
							foldout.contextMenuItems = new List<DebugUI.Foldout.ContextMenuItem>();
						}
						foldout.contextMenuItems.Add(new DebugUI.Foldout.ContextMenuItem
						{
							displayName = "Go to Section...",
							action = delegate()
							{
								int num = DebugManager.instance.PanelIndex(foldout.panel.displayName);
								if (num >= 0)
								{
									DebugManager.instance.RequestEditorWindowPanelIndex(num);
								}
							}
						});
					}
					base.AddWidget(widget);
				}
			}

			private const string k_GoToSectionString = "Go to Section...";
		}
	}
}
