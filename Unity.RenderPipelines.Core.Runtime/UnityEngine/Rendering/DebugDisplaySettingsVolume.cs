using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering
{
	public class DebugDisplaySettingsVolume : IDebugDisplaySettingsData, IDebugDisplaySettingsQuery
	{
		[Obsolete("This property has been obsoleted and will be removed in a future version. #from(6000.2)", false)]
		public IVolumeDebugSettings volumeDebugSettings { get; }

		public int selectedComponent
		{
			get
			{
				return this.m_SelectedComponentIndex;
			}
			set
			{
				if (value != this.m_SelectedComponentIndex)
				{
					this.m_SelectedComponentIndex = value;
					this.OnSelectionChanged();
				}
			}
		}

		private void DestroyVolumeInterpolatedResults()
		{
			if (this.m_VolumeInterpolatedResults != null)
			{
				Object.DestroyImmediate(this.m_VolumeInterpolatedResults);
			}
		}

		public Type selectedComponentType
		{
			get
			{
				if (this.selectedComponent <= 0)
				{
					return null;
				}
				return this.volumeComponentsPathAndType[this.selectedComponent - 1].Item2;
			}
			set
			{
				int num = this.volumeComponentsPathAndType.FindIndex((ValueTuple<string, Type> t) => t.Item2 == value);
				if (num != -1)
				{
					this.selectedComponent = num + 1;
				}
			}
		}

		public List<ValueTuple<string, Type>> volumeComponentsPathAndType
		{
			get
			{
				return VolumeManager.instance.GetVolumeComponentsForDisplay(GraphicsSettings.currentRenderPipelineAssetType);
			}
		}

		public Camera selectedCamera
		{
			get
			{
				return this.m_SelectedCamera;
			}
			set
			{
				if (value != this.m_SelectedCamera)
				{
					this.m_SelectedCamera = value;
					this.OnSelectionChanged();
				}
			}
		}

		private void OnSelectionChanged()
		{
			this.ClearInterpolationData();
			this.DestroyVolumeInterpolatedResults();
		}

		private void ClearInterpolationData()
		{
			this.m_VolumesWeights.Clear();
		}

		private static bool AreVolumesChanged(ObservableList<Volume> influenceVolumes, [TupleElementNames(new string[]
		{
			"volume",
			"weight"
		})] List<ValueTuple<Volume, float>> volumesWeights)
		{
			if (influenceVolumes.Count != volumesWeights.Count)
			{
				return true;
			}
			for (int i = 0; i < influenceVolumes.Count; i++)
			{
				if (influenceVolumes[i] != volumesWeights[i].Item1)
				{
					return true;
				}
			}
			return false;
		}

		private void OnBeginVolumeStackUpdate(VolumeStack stack, Camera camera)
		{
			if (camera == this.selectedCamera)
			{
				this.ClearInterpolationData();
				this.m_StoreStackInterpolatedValues = (this.selectedCamera != null && this.selectedComponentType != null);
			}
		}

		private void OnEndVolumeStackUpdate(VolumeStack stack, Camera camera)
		{
			if (this.m_StoreStackInterpolatedValues)
			{
				if (DebugDisplaySettingsVolume.AreVolumesChanged(this.m_InfluenceVolumes, this.m_VolumesWeights))
				{
					this.m_InfluenceVolumes.Clear();
					foreach (ValueTuple<Volume, float> valueTuple in this.m_VolumesWeights)
					{
						this.m_InfluenceVolumes.Add(valueTuple.Item1);
					}
				}
				VolumeComponent component = stack.GetComponent(this.selectedComponentType);
				for (int i = 0; i < component.parameters.Count; i++)
				{
					this.resultVolumeComponent.parameters[i].SetValue(component.parameters[i]);
				}
				this.m_StoreStackInterpolatedValues = false;
			}
		}

		private void OnVolumeStackInterpolated(VolumeStack stack, Volume volume, float interpolationFactor)
		{
			if (this.m_StoreStackInterpolatedValues)
			{
				this.m_VolumesWeights.Add(new ValueTuple<Volume, float>(volume, interpolationFactor));
			}
		}

		public float GetVolumeWeight(Volume volume)
		{
			if (this.m_VolumesWeights.Count == 0)
			{
				return 0f;
			}
			foreach (ValueTuple<Volume, float> valueTuple in this.m_VolumesWeights)
			{
				if (volume == valueTuple.Item1)
				{
					return valueTuple.Item2;
				}
			}
			return 0f;
		}

		public ObservableList<Volume> GetVolumesList()
		{
			return this.m_InfluenceVolumes;
		}

		void IDebugDisplaySettingsData.Reset()
		{
			this.ClearInterpolationData();
			this.DestroyVolumeInterpolatedResults();
		}

		[Obsolete("This constructor has been obsoleted and will be removed in a future version. #from(6000.2)", false)]
		public DebugDisplaySettingsVolume(IVolumeDebugSettings volumeDebugSettings) : this()
		{
			this.volumeDebugSettings = volumeDebugSettings;
		}

		public DebugDisplaySettingsVolume()
		{
		}

		internal VolumeComponent resultVolumeComponent
		{
			get
			{
				if (this.m_VolumeInterpolatedResults == null)
				{
					this.m_VolumeInterpolatedResults = (ScriptableObject.CreateInstance(this.selectedComponentType) as VolumeComponent);
				}
				return this.m_VolumeInterpolatedResults;
			}
		}

		internal static string ExtractResult(VolumeParameter param)
		{
			if (param == null)
			{
				return DebugDisplaySettingsVolume.Strings.parameterNotCalculated;
			}
			PropertyInfo property = param.GetType().GetProperty("value");
			if (property == null)
			{
				return "-";
			}
			object value = property.GetValue(param);
			Type propertyType = property.PropertyType;
			if (value == null || value.Equals(null))
			{
				return DebugDisplaySettingsVolume.Strings.none + " (" + propertyType.Name + ")";
			}
			MethodInfo method = propertyType.GetMethod("ToString", Type.EmptyTypes);
			if (!(method == null) && !(method.DeclaringType == typeof(object)) && !(method.DeclaringType == typeof(Object)))
			{
				return value.ToString();
			}
			PropertyInfo property2 = property.PropertyType.GetProperty("name");
			if (property2 == null)
			{
				return DebugDisplaySettingsVolume.Strings.debugViewNotSupported;
			}
			return string.Format("{0}", property2.GetValue(value)) ?? DebugDisplaySettingsVolume.Strings.none;
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
			return new DebugDisplaySettingsVolume.SettingsPanel(this);
		}

		private int m_SelectedComponentIndex = -1;

		private Camera m_SelectedCamera;

		private VolumeComponent m_VolumeInterpolatedResults;

		private bool m_StoreStackInterpolatedValues;

		private ObservableList<Volume> m_InfluenceVolumes = new ObservableList<Volume>();

		[TupleElementNames(new string[]
		{
			"volume",
			"weight"
		})]
		private List<ValueTuple<Volume, float>> m_VolumesWeights = new List<ValueTuple<Volume, float>>();

		internal int volumeComponentEnumIndex;

		private const string k_PanelTitle = "Volume";

		private static class Styles
		{
			public static readonly GUIContent none = new GUIContent("None");
		}

		private static class Strings
		{
			public static readonly string cameraNeedsRendering = "Values might not be fully updated if the camera you are inspecting is not rendered.";

			public static readonly string none = "None";

			public static readonly string parameter = "Parameter";

			public static readonly string component = "Component";

			public static readonly string debugViewNotSupported = "N/A";

			public static readonly string volumeInfo = "Volume Info";

			public static readonly string gameObject = "GameObject";

			public static readonly string priority = "Priority";

			public static readonly string resultValue = "Result";

			public static readonly string resultValueTooltip = "The interpolated result value of the parameter. This value is used to render the camera.";

			public static readonly string globalDefaultValue = "Graphics Settings";

			public static readonly string globalDefaultValueTooltip = "Default value for this parameter, defined by the Default Volume Profile in Global Settings.";

			public static readonly string qualityLevelValue = "Quality Settings";

			public static readonly string qualityLevelValueTooltip = "Override value for this parameter, defined by the Volume Profile in the current SRP Asset.";

			public static readonly string global = "Global";

			public static readonly string local = "Local";

			public static readonly string volumeProfile = "Volume Profile";

			public static readonly string parameterNotCalculated = "N/A";
		}

		internal static class WidgetFactory
		{
			public static DebugUI.EnumField CreateComponentSelector(DebugDisplaySettingsVolume.SettingsPanel panel, Action<DebugUI.Field<int>, int> refresh)
			{
				int num = 0;
				List<GUIContent> list = new List<GUIContent>
				{
					DebugDisplaySettingsVolume.Styles.none
				};
				List<int> list2 = new List<int>
				{
					num++
				};
				foreach (ValueTuple<string, Type> valueTuple in panel.data.volumeComponentsPathAndType)
				{
					list.Add(new GUIContent
					{
						text = valueTuple.Item1
					});
					list2.Add(num++);
				}
				return new DebugUI.EnumField
				{
					displayName = DebugDisplaySettingsVolume.Strings.component,
					getter = (() => panel.data.selectedComponent),
					setter = delegate(int value)
					{
						panel.data.selectedComponent = value;
					},
					enumNames = list.ToArray(),
					enumValues = list2.ToArray(),
					getIndex = (() => panel.data.volumeComponentEnumIndex),
					setIndex = delegate(int value)
					{
						panel.data.volumeComponentEnumIndex = value;
					},
					onValueChanged = refresh
				};
			}

			public static DebugUI.CameraSelector CreateCameraSelector(DebugDisplaySettingsVolume.SettingsPanel panel, Action<DebugUI.Field<Object>, Object> refresh)
			{
				return new DebugUI.CameraSelector
				{
					getter = (() => panel.data.selectedCamera),
					setter = delegate(Object value)
					{
						panel.data.selectedCamera = (value as Camera);
					},
					onValueChanged = refresh
				};
			}

			internal static DebugUI.Widget CreateVolumeParameterWidget(string name, bool isResultParameter, VolumeParameter param)
			{
				DebugUI.Value value = new DebugUI.Value();
				value.displayName = name;
				value.getter = (() => DebugDisplaySettingsVolume.Strings.parameterNotCalculated);
				return value;
			}

			private static VolumeComponent GetSelectedVolumeComponent(VolumeProfile profile, Type selectedType)
			{
				if (profile != null)
				{
					foreach (VolumeComponent volumeComponent in profile.components)
					{
						if (volumeComponent.GetType() == selectedType)
						{
							return volumeComponent;
						}
					}
				}
				return null;
			}

			private static List<DebugDisplaySettingsVolume.WidgetFactory.VolumeParameterChain> GetResolutionChain(DebugDisplaySettingsVolume data)
			{
				List<DebugDisplaySettingsVolume.WidgetFactory.VolumeParameterChain> list = new List<DebugDisplaySettingsVolume.WidgetFactory.VolumeParameterChain>();
				Type selectedComponentType = data.selectedComponentType;
				if (data.selectedCamera == null || selectedComponentType == null)
				{
					return list;
				}
				if (data.resultVolumeComponent == null)
				{
					return list;
				}
				DebugDisplaySettingsVolume.WidgetFactory.VolumeParameterChain item = new DebugDisplaySettingsVolume.WidgetFactory.VolumeParameterChain
				{
					nameAndTooltip = new DebugUI.Widget.NameAndTooltip
					{
						name = DebugDisplaySettingsVolume.Strings.resultValue,
						tooltip = DebugDisplaySettingsVolume.Strings.resultValueTooltip
					},
					volumeComponent = data.resultVolumeComponent
				};
				list.Add(item);
				ObservableList<Volume> volumesList = data.GetVolumesList();
				for (int i = volumesList.Count - 1; i >= 0; i--)
				{
					Volume volume = volumesList[i];
					VolumeProfile volumeProfile = volume.HasInstantiatedProfile() ? volume.profile : volume.sharedProfile;
					VolumeComponent selectedVolumeComponent = DebugDisplaySettingsVolume.WidgetFactory.GetSelectedVolumeComponent(volumeProfile, selectedComponentType);
					if (selectedVolumeComponent != null)
					{
						DebugDisplaySettingsVolume.WidgetFactory.VolumeParameterChain item2 = new DebugDisplaySettingsVolume.WidgetFactory.VolumeParameterChain
						{
							nameAndTooltip = new DebugUI.Widget.NameAndTooltip
							{
								name = volumeProfile.name,
								tooltip = volumeProfile.name
							},
							volumeProfile = volumeProfile,
							volumeComponent = selectedVolumeComponent,
							volume = volume
						};
						list.Add(item2);
					}
				}
				if (VolumeManager.instance.customDefaultProfiles != null)
				{
					foreach (VolumeProfile volumeProfile2 in VolumeManager.instance.customDefaultProfiles)
					{
						VolumeComponent selectedVolumeComponent2 = DebugDisplaySettingsVolume.WidgetFactory.GetSelectedVolumeComponent(volumeProfile2, selectedComponentType);
						if (selectedVolumeComponent2 != null)
						{
							DebugDisplaySettingsVolume.WidgetFactory.VolumeParameterChain item3 = new DebugDisplaySettingsVolume.WidgetFactory.VolumeParameterChain
							{
								nameAndTooltip = new DebugUI.Widget.NameAndTooltip
								{
									name = volumeProfile2.name,
									tooltip = volumeProfile2.name
								},
								volumeProfile = volumeProfile2,
								volumeComponent = selectedVolumeComponent2
							};
							list.Add(item3);
						}
					}
				}
				if (VolumeManager.instance.qualityDefaultProfile != null)
				{
					VolumeComponent selectedVolumeComponent3 = DebugDisplaySettingsVolume.WidgetFactory.GetSelectedVolumeComponent(VolumeManager.instance.qualityDefaultProfile, selectedComponentType);
					if (selectedVolumeComponent3 != null)
					{
						DebugDisplaySettingsVolume.WidgetFactory.VolumeParameterChain item4 = new DebugDisplaySettingsVolume.WidgetFactory.VolumeParameterChain
						{
							nameAndTooltip = new DebugUI.Widget.NameAndTooltip
							{
								name = DebugDisplaySettingsVolume.Strings.qualityLevelValue,
								tooltip = DebugDisplaySettingsVolume.Strings.qualityLevelValueTooltip
							},
							volumeProfile = VolumeManager.instance.qualityDefaultProfile,
							volumeComponent = selectedVolumeComponent3
						};
						list.Add(item4);
					}
				}
				if (VolumeManager.instance.globalDefaultProfile != null)
				{
					VolumeComponent selectedVolumeComponent4 = DebugDisplaySettingsVolume.WidgetFactory.GetSelectedVolumeComponent(VolumeManager.instance.globalDefaultProfile, selectedComponentType);
					if (selectedVolumeComponent4 != null)
					{
						DebugDisplaySettingsVolume.WidgetFactory.VolumeParameterChain item5 = new DebugDisplaySettingsVolume.WidgetFactory.VolumeParameterChain
						{
							nameAndTooltip = new DebugUI.Widget.NameAndTooltip
							{
								name = DebugDisplaySettingsVolume.Strings.globalDefaultValue,
								tooltip = DebugDisplaySettingsVolume.Strings.globalDefaultValueTooltip
							},
							volumeProfile = VolumeManager.instance.globalDefaultProfile,
							volumeComponent = selectedVolumeComponent4
						};
						list.Add(item5);
					}
				}
				return list;
			}

			public static DebugUI.Table CreateVolumeTable(DebugDisplaySettingsVolume data)
			{
				Func<bool> isHiddenCallback = () => true;
				DebugUI.Table table = new DebugUI.Table
				{
					displayName = DebugDisplaySettingsVolume.Strings.parameter,
					isReadOnly = true,
					isHiddenCallback = isHiddenCallback
				};
				List<DebugDisplaySettingsVolume.WidgetFactory.VolumeParameterChain> resolutionChain = DebugDisplaySettingsVolume.WidgetFactory.GetResolutionChain(data);
				if (resolutionChain.Count == 0)
				{
					return table;
				}
				DebugDisplaySettingsVolume.WidgetFactory.GenerateTableRows(table, resolutionChain);
				DebugDisplaySettingsVolume.WidgetFactory.GenerateTableColumns(table, data, resolutionChain);
				return table;
			}

			private static void GenerateTableColumns(DebugUI.Table table, DebugDisplaySettingsVolume data, List<DebugDisplaySettingsVolume.WidgetFactory.VolumeParameterChain> resolutionChain)
			{
				for (int i = 0; i < resolutionChain.Count; i++)
				{
					DebugDisplaySettingsVolume.WidgetFactory.VolumeParameterChain chain = resolutionChain[i];
					int num = -1;
					if (chain.volume != null)
					{
						((DebugUI.Table.Row)table.children[++num]).children.Add(new DebugUI.Value
						{
							nameAndTooltip = chain.nameAndTooltip,
							getter = delegate
							{
								string text = chain.volume.isGlobal ? DebugDisplaySettingsVolume.Strings.global : DebugDisplaySettingsVolume.Strings.local;
								float volumeWeight = data.GetVolumeWeight(chain.volume);
								if (chain.volumeComponent.active)
								{
									return string.Format("{0} ({1:F2}%)", text, volumeWeight * 100f);
								}
								return text + " (disabled)";
							},
							refreshRate = 0.2f
						});
						((DebugUI.Table.Row)table.children[++num]).children.Add(new DebugUI.ObjectField
						{
							displayName = string.Empty,
							getter = (() => chain.volume)
						});
						((DebugUI.Table.Row)table.children[++num]).children.Add(new DebugUI.Value
						{
							nameAndTooltip = chain.nameAndTooltip,
							getter = (() => chain.volume.priority)
						});
					}
					else
					{
						ObservableList<DebugUI.Widget> children = ((DebugUI.Table.Row)table.children[++num]).children;
						DebugUI.Value value = new DebugUI.Value();
						value.nameAndTooltip = chain.nameAndTooltip;
						value.getter = (() => string.Empty);
						children.Add(value);
						((DebugUI.Table.Row)table.children[++num]).children.Add(DebugDisplaySettingsVolume.WidgetFactory.s_EmptyDebugUIValue);
						((DebugUI.Table.Row)table.children[++num]).children.Add(DebugDisplaySettingsVolume.WidgetFactory.s_EmptyDebugUIValue);
					}
					ObservableList<DebugUI.Widget> children2 = ((DebugUI.Table.Row)table.children[++num]).children;
					DebugUI.Widget item;
					if (!(chain.volumeProfile != null))
					{
						item = DebugDisplaySettingsVolume.WidgetFactory.s_EmptyDebugUIValue;
					}
					else
					{
						DebugUI.ObjectField objectField = new DebugUI.ObjectField();
						objectField.displayName = string.Empty;
						item = objectField;
						objectField.getter = (() => chain.volumeProfile);
					}
					children2.Add(item);
					((DebugUI.Table.Row)table.children[++num]).children.Add(DebugDisplaySettingsVolume.WidgetFactory.s_EmptyDebugUIValue);
					bool isResultParameter = i == 0;
					for (int j = 0; j < chain.volumeComponent.parameterList.Length; j++)
					{
						VolumeParameter param = chain.volumeComponent.parameterList[j];
						((DebugUI.Table.Row)table.children[++num]).children.Add(DebugDisplaySettingsVolume.WidgetFactory.CreateVolumeParameterWidget(chain.nameAndTooltip.name, isResultParameter, param));
					}
				}
			}

			private static void GenerateTableRows(DebugUI.Table table, List<DebugDisplaySettingsVolume.WidgetFactory.VolumeParameterChain> resolutionChain)
			{
				DebugUI.Table.Row item = new DebugUI.Table.Row
				{
					displayName = DebugDisplaySettingsVolume.Strings.volumeInfo,
					opened = true
				};
				table.children.Add(item);
				DebugUI.Table.Row item2 = new DebugUI.Table.Row
				{
					displayName = DebugDisplaySettingsVolume.Strings.gameObject
				};
				table.children.Add(item2);
				DebugUI.Table.Row item3 = new DebugUI.Table.Row
				{
					displayName = DebugDisplaySettingsVolume.Strings.priority
				};
				table.children.Add(item3);
				DebugUI.Table.Row item4 = new DebugUI.Table.Row
				{
					displayName = DebugDisplaySettingsVolume.Strings.volumeProfile
				};
				table.children.Add(item4);
				DebugUI.Table.Row item5 = new DebugUI.Table.Row
				{
					displayName = string.Empty
				};
				table.children.Add(item5);
				VolumeComponent volumeComponent = resolutionChain[0].volumeComponent;
				for (int i = 0; i < volumeComponent.parameterList.Length; i++)
				{
					VolumeParameter volumeParameter = volumeComponent.parameterList[i];
					string displayName = i.ToString();
					table.children.Add(new DebugUI.Table.Row
					{
						displayName = displayName
					});
				}
			}

			private static DebugUI.Value s_EmptyDebugUIValue = new DebugUI.Value
			{
				getter = (() => string.Empty)
			};

			private struct VolumeParameterChain
			{
				public DebugUI.Widget.NameAndTooltip nameAndTooltip;

				public VolumeProfile volumeProfile;

				public VolumeComponent volumeComponent;

				public Volume volume;
			}
		}

		[DisplayInfo(name = "Volume", order = 2147483647)]
		internal class SettingsPanel : DebugDisplaySettingsPanel<DebugDisplaySettingsVolume>
		{
			public override DebugUI.Flags Flags
			{
				get
				{
					return DebugUI.Flags.EditorForceUpdate;
				}
			}

			public override void Dispose()
			{
				base.Dispose();
				base.data.GetVolumesList().ItemAdded -= this.OnVolumeInfluenceChanged;
				base.data.GetVolumesList().ItemRemoved -= this.OnVolumeInfluenceChanged;
			}

			public SettingsPanel(DebugDisplaySettingsVolume data) : base(data)
			{
				DebugDisplaySettingsVolume.SettingsPanel <>4__this = this;
				DebugUI.CameraSelector cameraSelector = DebugDisplaySettingsVolume.WidgetFactory.CreateCameraSelector(this, delegate(DebugUI.Field<Object> _, Object __)
				{
					<>4__this.Refresh();
				});
				List<Camera> list = cameraSelector.getObjects() as List<Camera>;
				if (data.selectedCamera == null && list != null && list.Count > 0)
				{
					data.selectedCamera = list[0];
				}
				base.AddWidget(cameraSelector);
				base.AddWidget(DebugDisplaySettingsVolume.WidgetFactory.CreateComponentSelector(this, delegate(DebugUI.Field<int> _, int __)
				{
					<>4__this.Refresh();
				}));
				Func<bool> isHiddenCallback = () => data.selectedCamera == null || data.selectedComponent <= 0;
				base.AddWidget(new DebugUI.MessageBox
				{
					displayName = DebugDisplaySettingsVolume.Strings.cameraNeedsRendering,
					style = DebugUI.MessageBox.Style.Warning,
					isHiddenCallback = isHiddenCallback
				});
				this.m_VolumeTable = DebugDisplaySettingsVolume.WidgetFactory.CreateVolumeTable(data);
				base.AddWidget(this.m_VolumeTable);
				data.GetVolumesList().ItemAdded += this.OnVolumeInfluenceChanged;
				data.GetVolumesList().ItemRemoved += this.OnVolumeInfluenceChanged;
			}

			private void OnVolumeInfluenceChanged(ObservableList<Volume> sender, ListChangedEventArgs<Volume> e)
			{
				this.Refresh();
				DebugManager.instance.ReDrawOnScreenDebug();
			}

			private void Refresh()
			{
				if (DebugManager.instance.GetPanel(this.PanelName, false, 0, false) == null)
				{
					return;
				}
				bool flag = false;
				if (this.m_Data.selectedComponent > 0 && this.m_Data.selectedCamera != null)
				{
					flag = true;
					DebugUI.Container container = DebugDisplaySettingsVolume.WidgetFactory.CreateVolumeTable(this.m_Data);
					this.m_VolumeTable.children.Clear();
					foreach (DebugUI.Widget item in container.children)
					{
						this.m_VolumeTable.children.Add(item);
					}
				}
				if (flag)
				{
					DebugManager.instance.ReDrawOnScreenDebug();
				}
			}

			private DebugUI.Table m_VolumeTable;
		}
	}
}
