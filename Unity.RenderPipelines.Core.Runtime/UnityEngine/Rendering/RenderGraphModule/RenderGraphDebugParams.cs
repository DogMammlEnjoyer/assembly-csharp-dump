using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.RenderGraphModule
{
	internal class RenderGraphDebugParams : IDebugDisplaySettingsQuery
	{
		public bool enableLogging
		{
			get
			{
				return this.logFrameInformation || this.logResources;
			}
		}

		public void ResetLogging()
		{
			this.logFrameInformation = false;
			this.logResources = false;
		}

		internal void Reset()
		{
			this.clearRenderTargetsAtCreation = false;
			this.clearRenderTargetsAtRelease = false;
			this.disablePassCulling = false;
			this.disablePassMerging = false;
			this.immediateMode = false;
			this.ResetLogging();
		}

		internal List<DebugUI.Widget> GetWidgetList(string name)
		{
			List<DebugUI.Widget> list = new List<DebugUI.Widget>();
			DebugUI.Container container = new DebugUI.Container();
			container.displayName = name + " Render Graph";
			container.children.Add(new DebugUI.BoolField
			{
				nameAndTooltip = RenderGraphDebugParams.Strings.ClearRenderTargetsAtCreation,
				getter = (() => this.clearRenderTargetsAtCreation),
				setter = delegate(bool value)
				{
					this.clearRenderTargetsAtCreation = value;
				}
			});
			container.children.Add(new DebugUI.BoolField
			{
				nameAndTooltip = RenderGraphDebugParams.Strings.ClearRenderTargetsAtFree,
				getter = (() => this.clearRenderTargetsAtRelease),
				setter = delegate(bool value)
				{
					this.clearRenderTargetsAtRelease = value;
				}
			});
			container.children.Add(new DebugUI.BoolField
			{
				nameAndTooltip = RenderGraphDebugParams.Strings.DisablePassCulling,
				getter = (() => this.disablePassCulling),
				setter = delegate(bool value)
				{
					this.disablePassCulling = value;
				}
			});
			ObservableList<DebugUI.Widget> children = container.children;
			DebugUI.BoolField boolField = new DebugUI.BoolField();
			boolField.nameAndTooltip = RenderGraphDebugParams.Strings.DisablePassMerging;
			boolField.getter = (() => this.disablePassMerging);
			boolField.setter = delegate(bool value)
			{
				this.disablePassMerging = value;
			};
			boolField.isHiddenCallback = (() => !RenderGraph.hasAnyRenderGraphWithNativeRenderPassesEnabled);
			children.Add(boolField);
			container.children.Add(new DebugUI.BoolField
			{
				nameAndTooltip = RenderGraphDebugParams.Strings.ImmediateMode,
				getter = (() => this.immediateMode),
				setter = delegate(bool value)
				{
					this.immediateMode = value;
				},
				isHiddenCallback = (() => !this.IsImmediateModeSupported())
			});
			container.children.Add(new DebugUI.Button
			{
				nameAndTooltip = RenderGraphDebugParams.Strings.LogFrameInformation,
				action = delegate
				{
					this.logFrameInformation = true;
				}
			});
			container.children.Add(new DebugUI.Button
			{
				nameAndTooltip = RenderGraphDebugParams.Strings.LogResources,
				action = delegate
				{
					this.logResources = true;
				}
			});
			list.Add(container);
			return list;
		}

		private bool IsImmediateModeSupported()
		{
			IRenderGraphEnabledRenderPipeline renderGraphEnabledRenderPipeline = GraphicsSettings.currentRenderPipeline as IRenderGraphEnabledRenderPipeline;
			return renderGraphEnabledRenderPipeline != null && renderGraphEnabledRenderPipeline.isImmediateModeSupported;
		}

		public void RegisterDebug(string name, DebugUI.Panel debugPanel = null)
		{
			List<DebugUI.Widget> widgetList = this.GetWidgetList(name);
			this.m_DebugItems = widgetList.ToArray();
			this.m_DebugPanel = ((debugPanel != null) ? debugPanel : DebugManager.instance.GetPanel((name.Length == 0) ? "Rendering" : name, true, 0, false));
			DebugUI.Foldout foldout = new DebugUI.Foldout
			{
				displayName = name
			};
			foldout.children.Add(this.m_DebugItems);
			this.m_DebugPanel.children.Add(foldout);
		}

		public void UnRegisterDebug(string name)
		{
			if (this.m_DebugPanel != null)
			{
				this.m_DebugPanel.children.Remove(this.m_DebugItems);
			}
			this.m_DebugPanel = null;
			this.m_DebugItems = null;
		}

		public bool AreAnySettingsActive
		{
			get
			{
				return this.clearRenderTargetsAtCreation || this.clearRenderTargetsAtRelease || this.disablePassCulling || this.disablePassMerging || this.immediateMode || this.enableLogging;
			}
		}

		private DebugUI.Widget[] m_DebugItems;

		private DebugUI.Panel m_DebugPanel;

		public bool clearRenderTargetsAtCreation;

		public bool clearRenderTargetsAtRelease;

		public bool disablePassCulling;

		public bool disablePassMerging;

		public bool immediateMode;

		public bool logFrameInformation;

		public bool logResources;

		private static class Strings
		{
			public static readonly DebugUI.Widget.NameAndTooltip ClearRenderTargetsAtCreation = new DebugUI.Widget.NameAndTooltip
			{
				name = "Clear Render Targets At Creation",
				tooltip = "Enable to clear all render textures before any rendergraph passes to check if some clears are missing."
			};

			public static readonly DebugUI.Widget.NameAndTooltip ClearRenderTargetsAtFree = new DebugUI.Widget.NameAndTooltip
			{
				name = "Clear Render Targets When Freed",
				tooltip = "Enable to clear all render textures when textures are freed by the graph to detect use after free of textures."
			};

			public static readonly DebugUI.Widget.NameAndTooltip DisablePassCulling = new DebugUI.Widget.NameAndTooltip
			{
				name = "Disable Pass Culling",
				tooltip = "Enable to temporarily disable culling to assess if a pass is culled."
			};

			public static readonly DebugUI.Widget.NameAndTooltip DisablePassMerging = new DebugUI.Widget.NameAndTooltip
			{
				name = "Disable Pass Merging",
				tooltip = "Enable to temporarily disable pass merging to diagnose issues or analyze performance."
			};

			public static readonly DebugUI.Widget.NameAndTooltip ImmediateMode = new DebugUI.Widget.NameAndTooltip
			{
				name = "Immediate Mode",
				tooltip = "Enable to force render graph to execute all passes in the order you registered them."
			};

			public static readonly DebugUI.Widget.NameAndTooltip EnableLogging = new DebugUI.Widget.NameAndTooltip
			{
				name = "Enable Logging",
				tooltip = "Enable to allow HDRP to capture information in the log."
			};

			public static readonly DebugUI.Widget.NameAndTooltip LogFrameInformation = new DebugUI.Widget.NameAndTooltip
			{
				name = "Log Frame Information",
				tooltip = "Enable to log information output from each frame."
			};

			public static readonly DebugUI.Widget.NameAndTooltip LogResources = new DebugUI.Widget.NameAndTooltip
			{
				name = "Log Resources",
				tooltip = "Enable to log the current render graph's global resource usage."
			};
		}
	}
}
