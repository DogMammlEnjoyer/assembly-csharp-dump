using System;
using System.Reflection;

namespace UnityEngine.Rendering
{
	public class DebugDisplayGPUResidentDrawer : IDebugDisplaySettingsData, IDebugDisplaySettingsQuery
	{
		private bool displayBatcherStats
		{
			get
			{
				DebugRendererBatcherStats debugStats = GPUResidentDrawer.GetDebugStats();
				return debugStats != null && debugStats.enabled;
			}
			set
			{
				DebugRendererBatcherStats debugStats = GPUResidentDrawer.GetDebugStats();
				if (debugStats != null)
				{
					debugStats.enabled = value;
				}
			}
		}

		internal bool GetOccluderViewInstanceID(out int viewInstanceID)
		{
			DebugRendererBatcherStats debugStats = GPUResidentDrawer.GetDebugStats();
			if (debugStats != null && this.occluderDebugViewIndex >= 0 && this.occluderDebugViewIndex < debugStats.occluderStats.Length)
			{
				viewInstanceID = debugStats.occluderStats[this.occluderDebugViewIndex].viewInstanceID;
				return true;
			}
			viewInstanceID = 0;
			return false;
		}

		internal bool occlusionTestOverlayEnable
		{
			get
			{
				DebugRendererBatcherStats debugStats = GPUResidentDrawer.GetDebugStats();
				return debugStats != null && debugStats.occlusionOverlayEnabled;
			}
			set
			{
				DebugRendererBatcherStats debugStats = GPUResidentDrawer.GetDebugStats();
				if (debugStats != null)
				{
					debugStats.occlusionOverlayEnabled = value;
				}
			}
		}

		private bool occlusionTestOverlayCountVisible
		{
			get
			{
				DebugRendererBatcherStats debugStats = GPUResidentDrawer.GetDebugStats();
				return debugStats != null && debugStats.occlusionOverlayCountVisible;
			}
			set
			{
				DebugRendererBatcherStats debugStats = GPUResidentDrawer.GetDebugStats();
				if (debugStats != null)
				{
					debugStats.occlusionOverlayCountVisible = value;
				}
			}
		}

		private bool overrideOcclusionTestToAlwaysPass
		{
			get
			{
				DebugRendererBatcherStats debugStats = GPUResidentDrawer.GetDebugStats();
				return debugStats != null && debugStats.overrideOcclusionTestToAlwaysPass;
			}
			set
			{
				DebugRendererBatcherStats debugStats = GPUResidentDrawer.GetDebugStats();
				if (debugStats != null)
				{
					debugStats.overrideOcclusionTestToAlwaysPass = value;
				}
			}
		}

		private static InstanceCullerViewStats GetInstanceCullerViewStats(int viewIndex)
		{
			DebugRendererBatcherStats debugStats = GPUResidentDrawer.GetDebugStats();
			if (debugStats != null && viewIndex < debugStats.instanceCullerStats.Length)
			{
				return debugStats.instanceCullerStats[viewIndex];
			}
			return default(InstanceCullerViewStats);
		}

		private static InstanceOcclusionEventStats GetInstanceOcclusionEventStats(int passIndex)
		{
			DebugRendererBatcherStats debugStats = GPUResidentDrawer.GetDebugStats();
			if (debugStats != null && passIndex < debugStats.instanceOcclusionEventStats.Length)
			{
				return debugStats.instanceOcclusionEventStats[passIndex];
			}
			return default(InstanceOcclusionEventStats);
		}

		private static DebugOccluderStats GetOccluderStats(int occluderIndex)
		{
			DebugRendererBatcherStats debugStats = GPUResidentDrawer.GetDebugStats();
			if (debugStats != null && occluderIndex < debugStats.occluderStats.Length)
			{
				return debugStats.occluderStats[occluderIndex];
			}
			return default(DebugOccluderStats);
		}

		private static int GetOcclusionContextsCounts()
		{
			DebugRendererBatcherStats debugStats = GPUResidentDrawer.GetDebugStats();
			if (debugStats == null)
			{
				return 0;
			}
			return debugStats.occluderStats.Length;
		}

		private static int GetInstanceCullerViewCount()
		{
			DebugRendererBatcherStats debugStats = GPUResidentDrawer.GetDebugStats();
			if (debugStats == null)
			{
				return 0;
			}
			return debugStats.instanceCullerStats.Length;
		}

		private static int GetInstanceOcclusionEventCount()
		{
			DebugRendererBatcherStats debugStats = GPUResidentDrawer.GetDebugStats();
			if (debugStats == null)
			{
				return 0;
			}
			return debugStats.instanceOcclusionEventStats.Length;
		}

		private static DebugUI.Table.Row AddInstanceCullerViewDataRow(int viewIndex)
		{
			return new DebugUI.Table.Row
			{
				displayName = "",
				opened = true,
				isHiddenCallback = (() => viewIndex >= DebugDisplayGPUResidentDrawer.GetInstanceCullerViewCount()),
				children = 
				{
					new DebugUI.Value
					{
						displayName = "View Type",
						refreshRate = 0.2f,
						formatString = "{0}",
						getter = (() => DebugDisplayGPUResidentDrawer.GetInstanceCullerViewStats(viewIndex).viewType)
					},
					new DebugUI.Value
					{
						displayName = "View Instance ID",
						refreshRate = 0.2f,
						formatString = "{0}",
						getter = (() => DebugDisplayGPUResidentDrawer.GetInstanceCullerViewStats(viewIndex).viewInstanceID)
					},
					new DebugUI.Value
					{
						displayName = "Split Index",
						refreshRate = 0.2f,
						formatString = "{0}",
						getter = (() => DebugDisplayGPUResidentDrawer.GetInstanceCullerViewStats(viewIndex).splitIndex)
					},
					new DebugUI.Value
					{
						displayName = "Visible Instances CPU | GPU",
						tooltip = "Visible instances after CPU culling and after GPU culling.",
						refreshRate = 0.2f,
						formatString = "{0}",
						getter = delegate
						{
							InstanceCullerViewStats instanceCullerViewStats = DebugDisplayGPUResidentDrawer.GetInstanceCullerViewStats(viewIndex);
							return string.Format("{0} | {1}", instanceCullerViewStats.visibleInstancesOnCPU, instanceCullerViewStats.visibleInstancesOnGPU);
						}
					},
					new DebugUI.Value
					{
						displayName = "Visible Primitives CPU | GPU",
						tooltip = "Visible primitives after CPU culling and after GPU culling.",
						refreshRate = 0.2f,
						formatString = "{0}",
						getter = delegate
						{
							InstanceCullerViewStats instanceCullerViewStats = DebugDisplayGPUResidentDrawer.GetInstanceCullerViewStats(viewIndex);
							return string.Format("{0} | {1}", instanceCullerViewStats.visiblePrimitivesOnCPU, instanceCullerViewStats.visiblePrimitivesOnGPU);
						}
					},
					new DebugUI.Value
					{
						displayName = "Draw Commands",
						refreshRate = 0.2f,
						formatString = "{0}",
						getter = (() => DebugDisplayGPUResidentDrawer.GetInstanceCullerViewStats(viewIndex).drawCommands)
					}
				}
			};
		}

		private static object OccluderVersionString(in InstanceOcclusionEventStats stats)
		{
			if (stats.eventType != InstanceOcclusionEventType.OccluderUpdate && stats.occlusionTest == OcclusionTest.None)
			{
				return "-";
			}
			return stats.occluderVersion;
		}

		private static object OcclusionTestString(in InstanceOcclusionEventStats stats)
		{
			if (stats.eventType != InstanceOcclusionEventType.OcclusionTest)
			{
				return "-";
			}
			return stats.occlusionTest;
		}

		private static object VisibleInstancesString(in InstanceOcclusionEventStats stats)
		{
			if (stats.eventType != InstanceOcclusionEventType.OcclusionTest)
			{
				return "-";
			}
			return stats.visibleInstances;
		}

		private static object CulledInstancesString(in InstanceOcclusionEventStats stats)
		{
			if (stats.eventType != InstanceOcclusionEventType.OcclusionTest)
			{
				return "-";
			}
			return stats.culledInstances;
		}

		private static object VisiblePrimitivesString(in InstanceOcclusionEventStats stats)
		{
			if (stats.eventType != InstanceOcclusionEventType.OcclusionTest)
			{
				return "-";
			}
			return stats.visiblePrimitives;
		}

		private static object CulledPrimitivesString(in InstanceOcclusionEventStats stats)
		{
			if (stats.eventType != InstanceOcclusionEventType.OcclusionTest)
			{
				return "-";
			}
			return stats.culledPrimitives;
		}

		private static DebugUI.Table.Row AddInstanceOcclusionPassDataRow(int eventIndex)
		{
			return new DebugUI.Table.Row
			{
				displayName = "",
				opened = true,
				isHiddenCallback = (() => eventIndex >= DebugDisplayGPUResidentDrawer.GetInstanceOcclusionEventCount()),
				children = 
				{
					new DebugUI.Value
					{
						displayName = "View Instance ID",
						refreshRate = 0.2f,
						formatString = "{0}",
						getter = (() => DebugDisplayGPUResidentDrawer.GetInstanceOcclusionEventStats(eventIndex).viewInstanceID)
					},
					new DebugUI.Value
					{
						displayName = "Event Type",
						refreshRate = 0.2f,
						formatString = "{0}",
						getter = (() => string.Format("{0}", DebugDisplayGPUResidentDrawer.GetInstanceOcclusionEventStats(eventIndex).eventType))
					},
					new DebugUI.Value
					{
						displayName = "Occluder Version",
						refreshRate = 0.2f,
						formatString = "{0}",
						getter = delegate
						{
							InstanceOcclusionEventStats instanceOcclusionEventStats = DebugDisplayGPUResidentDrawer.GetInstanceOcclusionEventStats(eventIndex);
							return DebugDisplayGPUResidentDrawer.OccluderVersionString(instanceOcclusionEventStats);
						}
					},
					new DebugUI.Value
					{
						displayName = "Subview Mask",
						refreshRate = 0.2f,
						formatString = "{0}",
						getter = (() => string.Format("0x{0:X}", DebugDisplayGPUResidentDrawer.GetInstanceOcclusionEventStats(eventIndex).subviewMask))
					},
					new DebugUI.Value
					{
						displayName = "Occlusion Test",
						refreshRate = 0.2f,
						formatString = "{0}",
						getter = delegate
						{
							string format = "{0}";
							InstanceOcclusionEventStats instanceOcclusionEventStats = DebugDisplayGPUResidentDrawer.GetInstanceOcclusionEventStats(eventIndex);
							return string.Format(format, DebugDisplayGPUResidentDrawer.OcclusionTestString(instanceOcclusionEventStats));
						}
					},
					new DebugUI.Value
					{
						displayName = "Visible Instances",
						refreshRate = 0.2f,
						formatString = "{0}",
						getter = delegate
						{
							InstanceOcclusionEventStats instanceOcclusionEventStats = DebugDisplayGPUResidentDrawer.GetInstanceOcclusionEventStats(eventIndex);
							return DebugDisplayGPUResidentDrawer.VisibleInstancesString(instanceOcclusionEventStats);
						}
					},
					new DebugUI.Value
					{
						displayName = "Culled Instances",
						refreshRate = 0.2f,
						formatString = "{0}",
						getter = delegate
						{
							InstanceOcclusionEventStats instanceOcclusionEventStats = DebugDisplayGPUResidentDrawer.GetInstanceOcclusionEventStats(eventIndex);
							return DebugDisplayGPUResidentDrawer.CulledInstancesString(instanceOcclusionEventStats);
						}
					},
					new DebugUI.Value
					{
						displayName = "Visible Primitives",
						refreshRate = 0.2f,
						formatString = "{0}",
						getter = delegate
						{
							InstanceOcclusionEventStats instanceOcclusionEventStats = DebugDisplayGPUResidentDrawer.GetInstanceOcclusionEventStats(eventIndex);
							return DebugDisplayGPUResidentDrawer.VisiblePrimitivesString(instanceOcclusionEventStats);
						}
					},
					new DebugUI.Value
					{
						displayName = "Culled Primitives",
						refreshRate = 0.2f,
						formatString = "{0}",
						getter = delegate
						{
							InstanceOcclusionEventStats instanceOcclusionEventStats = DebugDisplayGPUResidentDrawer.GetInstanceOcclusionEventStats(eventIndex);
							return DebugDisplayGPUResidentDrawer.CulledPrimitivesString(instanceOcclusionEventStats);
						}
					}
				}
			};
		}

		private static DebugUI.Table.Row AddOcclusionContextDataRow(int index)
		{
			return new DebugUI.Table.Row
			{
				displayName = "",
				opened = true,
				isHiddenCallback = (() => index >= DebugDisplayGPUResidentDrawer.GetOcclusionContextsCounts()),
				children = 
				{
					new DebugUI.Value
					{
						displayName = "View Instance ID",
						refreshRate = 0.2f,
						formatString = "{0}",
						getter = (() => DebugDisplayGPUResidentDrawer.GetOccluderStats(index).viewInstanceID)
					},
					new DebugUI.Value
					{
						displayName = "Subview Count",
						refreshRate = 0.2f,
						formatString = "{0}",
						getter = (() => DebugDisplayGPUResidentDrawer.GetOccluderStats(index).subviewCount)
					},
					new DebugUI.Value
					{
						displayName = "Size Per Subview",
						refreshRate = 0.2f,
						formatString = "{0}",
						getter = delegate
						{
							Vector2Int occluderMipLayoutSize = DebugDisplayGPUResidentDrawer.GetOccluderStats(index).occluderMipLayoutSize;
							return string.Format("{0}x{1}", occluderMipLayoutSize.x, occluderMipLayoutSize.y);
						}
					}
				}
			};
		}

		public bool AreAnySettingsActive
		{
			get
			{
				return this.displayBatcherStats;
			}
		}

		public bool IsPostProcessingAllowed
		{
			get
			{
				return true;
			}
		}

		public bool IsLightingActive
		{
			get
			{
				return true;
			}
		}

		public bool TryGetScreenClearColor(ref Color color)
		{
			return false;
		}

		IDebugDisplaySettingsPanelDisposable IDebugDisplaySettingsData.CreatePanel()
		{
			return new DebugDisplayGPUResidentDrawer.SettingsPanel(this);
		}

		private const string k_FormatString = "{0}";

		private const float k_RefreshRate = 0.2f;

		private const int k_MaxViewCount = 32;

		private const int k_MaxOcclusionPassCount = 32;

		private const int k_MaxContextCount = 16;

		public bool occluderDebugViewEnable;

		internal bool occluderContextStats;

		internal Vector2 occluderDebugViewRange = new Vector2(0f, 1f);

		internal int occluderDebugViewIndex;

		private static class Strings
		{
			public const string drawerSettingsContainerName = "GPU Resident Drawer Settings";

			public static readonly DebugUI.Widget.NameAndTooltip displayBatcherStats = new DebugUI.Widget.NameAndTooltip
			{
				name = "Display Culling Stats",
				tooltip = "Enable the checkbox to display stats for instance culling."
			};

			public const string occlusionCullingTitle = "Occlusion Culling";

			public static readonly DebugUI.Widget.NameAndTooltip occlusionTestOverlayEnable = new DebugUI.Widget.NameAndTooltip
			{
				name = "Occlusion Test Overlay",
				tooltip = "Occlusion test visualisation."
			};

			public static readonly DebugUI.Widget.NameAndTooltip occlusionTestOverlayCountVisible = new DebugUI.Widget.NameAndTooltip
			{
				name = "Occlusion Test Overlay Count Visible",
				tooltip = "Occlusion test visualisation should count visible instances instead of occluded instances."
			};

			public static readonly DebugUI.Widget.NameAndTooltip overrideOcclusionTestToAlwaysPass = new DebugUI.Widget.NameAndTooltip
			{
				name = "Override Occlusion Test To Always Pass",
				tooltip = "Occlusion test always passes."
			};

			public static readonly DebugUI.Widget.NameAndTooltip occluderContextStats = new DebugUI.Widget.NameAndTooltip
			{
				name = "Occluder Context Stats",
				tooltip = "Show all the active occluder context textures."
			};

			public static readonly DebugUI.Widget.NameAndTooltip occluderDebugViewEnable = new DebugUI.Widget.NameAndTooltip
			{
				name = "Occluder Debug View",
				tooltip = "Debug view of occluder texture."
			};

			public static readonly DebugUI.Widget.NameAndTooltip occluderDebugViewIndex = new DebugUI.Widget.NameAndTooltip
			{
				name = "Occluder Debug View Index",
				tooltip = "Index of the view for which the occluder texture is displayed. Use the Occlusion Test Context Stats for a list of the views."
			};

			public static readonly DebugUI.Widget.NameAndTooltip occluderDebugViewRangeMin = new DebugUI.Widget.NameAndTooltip
			{
				name = "Occluder Debug View Range Min",
				tooltip = "Range in which the occluder debug texture are displayed."
			};

			public static readonly DebugUI.Widget.NameAndTooltip occluderDebugViewRangeMax = new DebugUI.Widget.NameAndTooltip
			{
				name = "Occluder Debug View Range Max",
				tooltip = "Range in which the occluder debug texture are displayed."
			};
		}

		[DisplayInfo(name = "Rendering", order = 5)]
		private class SettingsPanel : DebugDisplaySettingsPanel
		{
			public override DebugUI.Flags Flags
			{
				get
				{
					return DebugUI.Flags.EditorForceUpdate;
				}
			}

			public SettingsPanel(DebugDisplayGPUResidentDrawer data)
			{
				DebugUI.Foldout foldout = new DebugUI.Foldout();
				foldout.displayName = "GPU Resident Drawer Settings";
				HelpURLAttribute customAttribute = typeof(DebugDisplayGPUResidentDrawer).GetCustomAttribute<HelpURLAttribute>();
				foldout.documentationUrl = ((customAttribute != null) ? customAttribute.URL : null);
				DebugUI.Foldout foldout2 = foldout;
				base.AddWidget(foldout2);
				DebugUI.MessageBox messageBox = new DebugUI.MessageBox();
				messageBox.displayName = "Not Supported";
				messageBox.style = DebugUI.MessageBox.Style.Warning;
				messageBox.messageCallback = delegate()
				{
					string result;
					LogType logType;
					if (!GPUResidentDrawer.IsGPUResidentDrawerSupportedBySRP(GPUResidentDrawer.GetGlobalSettingsFromRPAsset(), out result, out logType))
					{
						return result;
					}
					return string.Empty;
				};
				messageBox.isHiddenCallback = (() => GPUResidentDrawer.IsEnabled());
				DebugUI.MessageBox item = messageBox;
				foldout2.children.Add(item);
				ObservableList<DebugUI.Widget> children = foldout2.children;
				DebugUI.Container container = new DebugUI.Container();
				container.displayName = "Occlusion Culling";
				container.isHiddenCallback = (() => !GPUResidentDrawer.IsEnabled());
				container.children.Add(new DebugUI.BoolField
				{
					nameAndTooltip = DebugDisplayGPUResidentDrawer.Strings.occlusionTestOverlayEnable,
					getter = (() => data.occlusionTestOverlayEnable),
					setter = delegate(bool value)
					{
						data.occlusionTestOverlayEnable = value;
					}
				});
				container.children.Add(new DebugUI.BoolField
				{
					nameAndTooltip = DebugDisplayGPUResidentDrawer.Strings.occlusionTestOverlayCountVisible,
					getter = (() => data.occlusionTestOverlayCountVisible),
					setter = delegate(bool value)
					{
						data.occlusionTestOverlayCountVisible = value;
					}
				});
				container.children.Add(new DebugUI.BoolField
				{
					nameAndTooltip = DebugDisplayGPUResidentDrawer.Strings.overrideOcclusionTestToAlwaysPass,
					getter = (() => data.overrideOcclusionTestToAlwaysPass),
					setter = delegate(bool value)
					{
						data.overrideOcclusionTestToAlwaysPass = value;
					}
				});
				container.children.Add(new DebugUI.BoolField
				{
					nameAndTooltip = DebugDisplayGPUResidentDrawer.Strings.occluderContextStats,
					getter = (() => data.occluderContextStats),
					setter = delegate(bool value)
					{
						data.occluderContextStats = value;
					}
				});
				container.children.Add(new DebugUI.BoolField
				{
					nameAndTooltip = DebugDisplayGPUResidentDrawer.Strings.occluderDebugViewEnable,
					getter = (() => data.occluderDebugViewEnable),
					setter = delegate(bool value)
					{
						data.occluderDebugViewEnable = value;
					}
				});
				ObservableList<DebugUI.Widget> children2 = container.children;
				DebugUI.IntField intField = new DebugUI.IntField();
				intField.nameAndTooltip = DebugDisplayGPUResidentDrawer.Strings.occluderDebugViewIndex;
				intField.getter = (() => data.occluderDebugViewIndex);
				intField.setter = delegate(int value)
				{
					data.occluderDebugViewIndex = value;
				};
				intField.isHiddenCallback = (() => !data.occluderDebugViewEnable);
				intField.min = (() => 0);
				intField.max = (() => Math.Max(DebugDisplayGPUResidentDrawer.GetOcclusionContextsCounts() - 1, 0));
				children2.Add(intField);
				container.children.Add(new DebugUI.FloatField
				{
					nameAndTooltip = DebugDisplayGPUResidentDrawer.Strings.occluderDebugViewRangeMin,
					getter = (() => data.occluderDebugViewRange.x),
					setter = delegate(float value)
					{
						data.occluderDebugViewRange.x = value;
					},
					isHiddenCallback = (() => !data.occluderDebugViewEnable)
				});
				container.children.Add(new DebugUI.FloatField
				{
					nameAndTooltip = DebugDisplayGPUResidentDrawer.Strings.occluderDebugViewRangeMax,
					getter = (() => data.occluderDebugViewRange.y),
					setter = delegate(float value)
					{
						data.occluderDebugViewRange.y = value;
					},
					isHiddenCallback = (() => !data.occluderDebugViewEnable)
				});
				children.Add(container);
				this.AddOcclusionContextStatsWidget(data);
				ObservableList<DebugUI.Widget> children3 = foldout2.children;
				DebugUI.BoolField boolField = new DebugUI.BoolField();
				boolField.nameAndTooltip = DebugDisplayGPUResidentDrawer.Strings.displayBatcherStats;
				boolField.getter = (() => data.displayBatcherStats);
				boolField.setter = delegate(bool value)
				{
					data.displayBatcherStats = value;
				};
				boolField.isHiddenCallback = (() => !GPUResidentDrawer.IsEnabled());
				children3.Add(boolField);
				this.AddInstanceCullingStatsWidget(data);
			}

			private void AddInstanceCullingStatsWidget(DebugDisplayGPUResidentDrawer data)
			{
				DebugUI.Foldout foldout = new DebugUI.Foldout
				{
					displayName = "Instance Culler Stats",
					isHeader = true,
					opened = true,
					isHiddenCallback = (() => !data.displayBatcherStats)
				};
				ObservableList<DebugUI.Widget> children = foldout.children;
				DebugUI.ValueTuple valueTuple = new DebugUI.ValueTuple();
				valueTuple.displayName = "View Count";
				DebugUI.ValueTuple valueTuple2 = valueTuple;
				DebugUI.Value[] array = new DebugUI.Value[1];
				int num = 0;
				DebugUI.Value value = new DebugUI.Value();
				value.refreshRate = 0.2f;
				value.formatString = "{0}";
				value.getter = (() => DebugDisplayGPUResidentDrawer.GetInstanceCullerViewCount());
				array[num] = value;
				valueTuple2.values = array;
				children.Add(valueTuple);
				ObservableList<DebugUI.Widget> children2 = foldout.children;
				valueTuple = new DebugUI.ValueTuple();
				valueTuple.displayName = "Total Visible Instances (Cameras | Lights | Both)";
				DebugUI.ValueTuple valueTuple3 = valueTuple;
				DebugUI.Value[] array2 = new DebugUI.Value[3];
				int num2 = 0;
				DebugUI.Value value2 = new DebugUI.Value();
				value2.refreshRate = 0.2f;
				value2.formatString = "{0}";
				value2.getter = delegate()
				{
					int num8 = 0;
					for (int k = 0; k < DebugDisplayGPUResidentDrawer.GetInstanceCullerViewCount(); k++)
					{
						InstanceCullerViewStats instanceCullerViewStats = DebugDisplayGPUResidentDrawer.GetInstanceCullerViewStats(k);
						if (instanceCullerViewStats.viewType == BatchCullingViewType.Camera)
						{
							num8 += instanceCullerViewStats.visibleInstancesOnGPU;
						}
					}
					return num8;
				};
				array2[num2] = value2;
				int num3 = 1;
				DebugUI.Value value3 = new DebugUI.Value();
				value3.refreshRate = 0.2f;
				value3.formatString = "{0}";
				value3.getter = delegate()
				{
					int num8 = 0;
					for (int k = 0; k < DebugDisplayGPUResidentDrawer.GetInstanceCullerViewCount(); k++)
					{
						InstanceCullerViewStats instanceCullerViewStats = DebugDisplayGPUResidentDrawer.GetInstanceCullerViewStats(k);
						if (instanceCullerViewStats.viewType == BatchCullingViewType.Light)
						{
							num8 += instanceCullerViewStats.visibleInstancesOnGPU;
						}
					}
					return num8;
				};
				array2[num3] = value3;
				int num4 = 2;
				DebugUI.Value value4 = new DebugUI.Value();
				value4.refreshRate = 0.2f;
				value4.formatString = "{0}";
				value4.getter = delegate()
				{
					int num8 = 0;
					for (int k = 0; k < DebugDisplayGPUResidentDrawer.GetInstanceCullerViewCount(); k++)
					{
						InstanceCullerViewStats instanceCullerViewStats = DebugDisplayGPUResidentDrawer.GetInstanceCullerViewStats(k);
						if (instanceCullerViewStats.viewType != BatchCullingViewType.Filtering && instanceCullerViewStats.viewType != BatchCullingViewType.Picking && instanceCullerViewStats.viewType != BatchCullingViewType.SelectionOutline)
						{
							num8 += instanceCullerViewStats.visibleInstancesOnGPU;
						}
					}
					return num8;
				};
				array2[num4] = value4;
				valueTuple3.values = array2;
				children2.Add(valueTuple);
				ObservableList<DebugUI.Widget> children3 = foldout.children;
				valueTuple = new DebugUI.ValueTuple();
				valueTuple.displayName = "Total Visible Primitives (Cameras | Lights | Both)";
				DebugUI.ValueTuple valueTuple4 = valueTuple;
				DebugUI.Value[] array3 = new DebugUI.Value[3];
				int num5 = 0;
				DebugUI.Value value5 = new DebugUI.Value();
				value5.refreshRate = 0.2f;
				value5.formatString = "{0}";
				value5.getter = delegate()
				{
					int num8 = 0;
					for (int k = 0; k < DebugDisplayGPUResidentDrawer.GetInstanceCullerViewCount(); k++)
					{
						InstanceCullerViewStats instanceCullerViewStats = DebugDisplayGPUResidentDrawer.GetInstanceCullerViewStats(k);
						if (instanceCullerViewStats.viewType == BatchCullingViewType.Camera)
						{
							num8 += instanceCullerViewStats.visiblePrimitivesOnGPU;
						}
					}
					return num8;
				};
				array3[num5] = value5;
				int num6 = 1;
				DebugUI.Value value6 = new DebugUI.Value();
				value6.refreshRate = 0.2f;
				value6.formatString = "{0}";
				value6.getter = delegate()
				{
					int num8 = 0;
					for (int k = 0; k < DebugDisplayGPUResidentDrawer.GetInstanceCullerViewCount(); k++)
					{
						InstanceCullerViewStats instanceCullerViewStats = DebugDisplayGPUResidentDrawer.GetInstanceCullerViewStats(k);
						if (instanceCullerViewStats.viewType == BatchCullingViewType.Light)
						{
							num8 += instanceCullerViewStats.visiblePrimitivesOnGPU;
						}
					}
					return num8;
				};
				array3[num6] = value6;
				int num7 = 2;
				DebugUI.Value value7 = new DebugUI.Value();
				value7.refreshRate = 0.2f;
				value7.formatString = "{0}";
				value7.getter = delegate()
				{
					int num8 = 0;
					for (int k = 0; k < DebugDisplayGPUResidentDrawer.GetInstanceCullerViewCount(); k++)
					{
						InstanceCullerViewStats instanceCullerViewStats = DebugDisplayGPUResidentDrawer.GetInstanceCullerViewStats(k);
						if (instanceCullerViewStats.viewType != BatchCullingViewType.Filtering && instanceCullerViewStats.viewType != BatchCullingViewType.Picking && instanceCullerViewStats.viewType != BatchCullingViewType.SelectionOutline)
						{
							num8 += instanceCullerViewStats.visiblePrimitivesOnGPU;
						}
					}
					return num8;
				};
				array3[num7] = value7;
				valueTuple4.values = array3;
				children3.Add(valueTuple);
				DebugUI.Table table = new DebugUI.Table
				{
					displayName = "",
					isReadOnly = true
				};
				for (int i = 0; i < 32; i++)
				{
					table.children.Add(DebugDisplayGPUResidentDrawer.AddInstanceCullerViewDataRow(i));
				}
				DebugUI.Foldout foldout2 = new DebugUI.Foldout
				{
					displayName = "Per View Stats",
					isHeader = true,
					opened = false,
					isHiddenCallback = (() => !data.displayBatcherStats)
				};
				foldout2.children.Add(table);
				foldout.children.Add(foldout2);
				DebugUI.Table table2 = new DebugUI.Table
				{
					displayName = "",
					isReadOnly = true
				};
				for (int j = 0; j < 32; j++)
				{
					table2.children.Add(DebugDisplayGPUResidentDrawer.AddInstanceOcclusionPassDataRow(j));
				}
				DebugUI.Foldout foldout3 = new DebugUI.Foldout
				{
					displayName = "Occlusion Culling Events",
					isHeader = true,
					opened = false,
					isHiddenCallback = (() => !data.displayBatcherStats)
				};
				foldout3.children.Add(table2);
				foldout.children.Add(foldout3);
				base.AddWidget(foldout);
			}

			private void AddOcclusionContextStatsWidget(DebugDisplayGPUResidentDrawer data)
			{
				DebugUI.Foldout foldout = new DebugUI.Foldout
				{
					displayName = "Occlusion Context Stats",
					isHeader = true,
					opened = true,
					isHiddenCallback = (() => !data.occluderContextStats)
				};
				ObservableList<DebugUI.Widget> children = foldout.children;
				DebugUI.ValueTuple valueTuple = new DebugUI.ValueTuple();
				valueTuple.displayName = "Active Occlusion Contexts";
				DebugUI.ValueTuple valueTuple2 = valueTuple;
				DebugUI.Value[] array = new DebugUI.Value[1];
				int num = 0;
				DebugUI.Value value = new DebugUI.Value();
				value.refreshRate = 0.2f;
				value.formatString = "{0}";
				value.getter = (() => DebugDisplayGPUResidentDrawer.GetOcclusionContextsCounts());
				array[num] = value;
				valueTuple2.values = array;
				children.Add(valueTuple);
				DebugUI.Table table = new DebugUI.Table
				{
					displayName = "",
					isReadOnly = true
				};
				for (int i = 0; i < 16; i++)
				{
					table.children.Add(DebugDisplayGPUResidentDrawer.AddOcclusionContextDataRow(i));
				}
				foldout.children.Add(table);
				base.AddWidget(foldout);
			}
		}
	}
}
