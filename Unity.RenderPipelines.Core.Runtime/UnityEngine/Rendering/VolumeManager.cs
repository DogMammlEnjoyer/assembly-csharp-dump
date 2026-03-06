using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Profiling;

namespace UnityEngine.Rendering
{
	public sealed class VolumeManager
	{
		[Obsolete("Please use the Register without a given layer index #from(6000.0)", false)]
		public void Register(Volume volume, int layer)
		{
			if (volume.gameObject.layer != layer)
			{
				Debug.LogWarning(string.Format("Trying to register Volume {0} on layer index {1}, when the GameObject {2} is on layer index {3}.", new object[]
				{
					volume.name,
					layer,
					volume.gameObject.name,
					volume.gameObject.layer
				}) + Environment.NewLine + "The Volume Manager will respect the GameObject's layer.");
			}
			this.Register(volume);
		}

		[Obsolete("Please use the Register without a given layer index #from(6000.0)", false)]
		public void Unregister(Volume volume, int layer)
		{
			if (volume.gameObject.layer != layer)
			{
				Debug.LogWarning(string.Format("Trying to unregister Volume {0} on layer index {1}, when the GameObject {2} is on layer index {3}.", new object[]
				{
					volume.name,
					layer,
					volume.gameObject.name,
					volume.gameObject.layer
				}) + Environment.NewLine + "The Volume Manager will respect the GameObject's layer.");
			}
			this.Unregister(volume);
		}

		public static VolumeManager instance
		{
			get
			{
				return VolumeManager.s_Instance.Value;
			}
		}

		public VolumeStack stack { get; set; }

		[Obsolete("Please use baseComponentTypeArray instead.")]
		public IEnumerable<Type> baseComponentTypes
		{
			get
			{
				return this.baseComponentTypeArray;
			}
		}

		internal List<ValueTuple<string, Type>> GetVolumeComponentsForDisplay(Type currentPipelineAssetType)
		{
			if (currentPipelineAssetType == null)
			{
				return new List<ValueTuple<string, Type>>();
			}
			if (!currentPipelineAssetType.IsSubclassOf(typeof(RenderPipelineAsset)))
			{
				throw new ArgumentException("currentPipelineAssetType");
			}
			List<ValueTuple<string, Type>> list;
			if (VolumeManager.s_SupportedVolumeComponentsForRenderPipeline.TryGetValue(currentPipelineAssetType, out list))
			{
				return list;
			}
			if (this.baseComponentTypeArray == null)
			{
				this.LoadBaseTypes(currentPipelineAssetType);
			}
			list = this.BuildVolumeComponentDisplayList(this.baseComponentTypeArray);
			VolumeManager.s_SupportedVolumeComponentsForRenderPipeline[currentPipelineAssetType] = list;
			return list;
		}

		private List<ValueTuple<string, Type>> BuildVolumeComponentDisplayList(Type[] types)
		{
			if (types == null)
			{
				throw new ArgumentNullException("types");
			}
			List<ValueTuple<string, Type>> list = new List<ValueTuple<string, Type>>();
			foreach (Type type in types)
			{
				string text = string.Empty;
				bool flag = false;
				foreach (object obj in type.GetCustomAttributes(false))
				{
					VolumeComponentMenu volumeComponentMenu = obj as VolumeComponentMenu;
					if (volumeComponentMenu == null)
					{
						if (obj is HideInInspector || obj is ObsoleteAttribute)
						{
							flag = true;
						}
					}
					else
					{
						text = volumeComponentMenu.menu;
					}
				}
				if (!flag)
				{
					if (string.IsNullOrEmpty(text))
					{
						text = type.Name;
					}
					list.Add(new ValueTuple<string, Type>(text, type));
				}
			}
			return (from i in list
			orderby i.Item1
			select i).ToList<ValueTuple<string, Type>>();
		}

		public Type[] baseComponentTypeArray
		{
			get
			{
				if (this.isInitialized)
				{
					return this.m_BaseComponentTypeArray;
				}
				throw new InvalidOperationException("VolumeManager.instance.baseComponentTypeArray cannot be called before the VolumeManager is initialized. (See VolumeManager.instance.isInitialized and RenderPipelineManager for creation callback).");
			}
			internal set
			{
				this.m_BaseComponentTypeArray = value;
			}
		}

		public VolumeProfile globalDefaultProfile { get; private set; }

		public VolumeProfile qualityDefaultProfile { get; private set; }

		public ReadOnlyCollection<VolumeProfile> customDefaultProfiles { get; private set; }

		public VolumeComponent GetVolumeComponentDefaultState(Type volumeComponentType)
		{
			if (!typeof(VolumeComponent).IsAssignableFrom(volumeComponentType))
			{
				return null;
			}
			foreach (VolumeComponent volumeComponent in this.m_ComponentsDefaultState)
			{
				if (volumeComponent.GetType() == volumeComponentType)
				{
					return volumeComponent;
				}
			}
			return null;
		}

		internal VolumeManager()
		{
		}

		public bool isInitialized { get; private set; }

		public void Initialize(VolumeProfile globalDefaultVolumeProfile = null, VolumeProfile qualityDefaultVolumeProfile = null)
		{
			this.LoadBaseTypes(GraphicsSettings.currentRenderPipelineAssetType);
			this.InitializeInternal(globalDefaultVolumeProfile, qualityDefaultVolumeProfile);
		}

		internal void InitializeInternal(VolumeProfile globalDefaultVolumeProfile = null, VolumeProfile qualityDefaultVolumeProfile = null)
		{
			this.InitializeVolumeComponents();
			this.globalDefaultProfile = globalDefaultVolumeProfile;
			this.qualityDefaultProfile = qualityDefaultVolumeProfile;
			this.EvaluateVolumeDefaultState();
			this.m_DefaultStack = this.CreateStackInternal();
			this.stack = this.m_DefaultStack;
			this.isInitialized = true;
		}

		public void Deinitialize()
		{
			this.DestroyStack(this.m_DefaultStack);
			this.m_DefaultStack = null;
			foreach (VolumeStack volumeStack in this.m_CreatedVolumeStacks)
			{
				volumeStack.Dispose();
			}
			this.m_CreatedVolumeStacks.Clear();
			this.baseComponentTypeArray = null;
			this.globalDefaultProfile = null;
			this.qualityDefaultProfile = null;
			this.customDefaultProfiles = null;
			this.isInitialized = false;
		}

		public void SetGlobalDefaultProfile(VolumeProfile profile)
		{
			this.globalDefaultProfile = profile;
			this.EvaluateVolumeDefaultState();
		}

		public void SetQualityDefaultProfile(VolumeProfile profile)
		{
			this.qualityDefaultProfile = profile;
			this.EvaluateVolumeDefaultState();
		}

		public void SetCustomDefaultProfiles(List<VolumeProfile> profiles)
		{
			List<VolumeProfile> list = profiles ?? new List<VolumeProfile>();
			list.RemoveAll((VolumeProfile x) => x == null);
			this.customDefaultProfiles = new ReadOnlyCollection<VolumeProfile>(list);
			this.EvaluateVolumeDefaultState();
		}

		public void OnVolumeProfileChanged(VolumeProfile profile)
		{
			if (!this.isInitialized)
			{
				return;
			}
			if (this.globalDefaultProfile == profile || this.qualityDefaultProfile == profile || (this.customDefaultProfiles != null && this.customDefaultProfiles.Contains(profile)))
			{
				this.EvaluateVolumeDefaultState();
			}
		}

		public void OnVolumeComponentChanged(VolumeComponent component)
		{
			List<VolumeProfile> list = new List<VolumeProfile>
			{
				this.globalDefaultProfile,
				this.globalDefaultProfile
			};
			if (this.customDefaultProfiles != null)
			{
				list.AddRange(this.customDefaultProfiles);
			}
			using (List<VolumeProfile>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.components.Contains(component))
					{
						this.EvaluateVolumeDefaultState();
						break;
					}
				}
			}
		}

		public VolumeStack CreateStack()
		{
			if (!this.isInitialized)
			{
				throw new InvalidOperationException("VolumeManager.instance.CreateStack() cannot be called before the VolumeManager is initialized. (See VolumeManager.instance.isInitialized and RenderPipelineManager for creation callback).");
			}
			return this.CreateStackInternal();
		}

		private VolumeStack CreateStackInternal()
		{
			VolumeStack volumeStack = new VolumeStack();
			volumeStack.Reload(this.m_BaseComponentTypeArray);
			this.m_CreatedVolumeStacks.Add(volumeStack);
			return volumeStack;
		}

		public void ResetMainStack()
		{
			this.stack = this.m_DefaultStack;
		}

		public void DestroyStack(VolumeStack stack)
		{
			this.m_CreatedVolumeStacks.Remove(stack);
			stack.Dispose();
		}

		private bool IsSupportedByObsoleteVolumeComponentMenuForRenderPipeline(Type t, Type pipelineAssetType)
		{
			bool result = false;
			if (t.GetCustomAttribute<VolumeComponentMenuForRenderPipeline>() != null)
			{
				Debug.LogWarning(string.Format("{0} is deprecated, use {1} and {2} with {3} instead. #from(2023.1)", new object[]
				{
					"VolumeComponentMenuForRenderPipeline",
					"SupportedOnRenderPipelineAttribute",
					"VolumeComponentMenu",
					t
				}));
			}
			return result;
		}

		internal void LoadBaseTypes(Type pipelineAssetType)
		{
			List<Type> list;
			using (ListPool<Type>.Get(out list))
			{
				foreach (Type type in CoreUtils.GetAllTypesDerivedFrom<VolumeComponent>())
				{
					if (!type.IsAbstract && (SupportedOnRenderPipelineAttribute.IsTypeSupportedOnRenderPipeline(type, pipelineAssetType) || this.IsSupportedByObsoleteVolumeComponentMenuForRenderPipeline(type, pipelineAssetType)))
					{
						list.Add(type);
					}
				}
				this.m_BaseComponentTypeArray = list.ToArray();
			}
		}

		internal void InitializeVolumeComponents()
		{
			if (this.m_BaseComponentTypeArray == null || this.m_BaseComponentTypeArray.Length == 0)
			{
				return;
			}
			BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			Type[] baseComponentTypeArray = this.m_BaseComponentTypeArray;
			for (int i = 0; i < baseComponentTypeArray.Length; i++)
			{
				MethodInfo method = baseComponentTypeArray[i].GetMethod("Init", bindingAttr);
				if (method != null)
				{
					method.Invoke(null, null);
				}
			}
		}

		private void EvaluateVolumeDefaultState()
		{
			if (this.m_BaseComponentTypeArray == null || this.m_BaseComponentTypeArray.Length == 0)
			{
				return;
			}
			using (VolumeManager.k_ProfilerMarkerEvaluateVolumeDefaultState.Auto())
			{
				VolumeManager.<>c__DisplayClass59_0 CS$<>8__locals1;
				CS$<>8__locals1.componentsDefaultStateList = new List<VolumeComponent>();
				foreach (Type type in this.m_BaseComponentTypeArray)
				{
					CS$<>8__locals1.componentsDefaultStateList.Add((VolumeComponent)ScriptableObject.CreateInstance(type));
				}
				VolumeManager.<EvaluateVolumeDefaultState>g__ApplyDefaultProfile|59_0(this.globalDefaultProfile, ref CS$<>8__locals1);
				VolumeManager.<EvaluateVolumeDefaultState>g__ApplyDefaultProfile|59_0(this.qualityDefaultProfile, ref CS$<>8__locals1);
				if (this.customDefaultProfiles != null)
				{
					foreach (VolumeProfile profile in this.customDefaultProfiles)
					{
						VolumeManager.<EvaluateVolumeDefaultState>g__ApplyDefaultProfile|59_0(profile, ref CS$<>8__locals1);
					}
				}
				List<VolumeParameter> list = new List<VolumeParameter>();
				foreach (VolumeComponent volumeComponent in CS$<>8__locals1.componentsDefaultStateList)
				{
					list.AddRange(volumeComponent.parameters);
				}
				this.m_ComponentsDefaultState = CS$<>8__locals1.componentsDefaultStateList.ToArray();
				this.m_ParametersDefaultState = list.ToArray();
				foreach (VolumeStack volumeStack in this.m_CreatedVolumeStacks)
				{
					volumeStack.requiresReset = true;
					volumeStack.requiresResetForAllProperties = true;
				}
			}
		}

		public void Register(Volume volume)
		{
			this.m_VolumeCollection.Register(volume, volume.gameObject.layer);
		}

		public void Unregister(Volume volume)
		{
			this.m_VolumeCollection.Unregister(volume, volume.gameObject.layer);
		}

		public bool IsComponentActiveInMask<T>(LayerMask layerMask) where T : VolumeComponent
		{
			return this.m_VolumeCollection.IsComponentActiveInMask<T>(layerMask);
		}

		internal void SetLayerDirty(int layer)
		{
			this.m_VolumeCollection.SetLayerIndexDirty(layer);
		}

		internal void UpdateVolumeLayer(Volume volume, int prevLayer, int newLayer)
		{
			this.m_VolumeCollection.ChangeLayer(volume, prevLayer, newLayer);
		}

		private void OverrideData(VolumeStack stack, Volume volume, float interpFactor)
		{
			List<VolumeComponent> components = volume.profileRef.components;
			int count = components.Count;
			for (int i = 0; i < count; i++)
			{
				VolumeComponent volumeComponent = components[i];
				if (volumeComponent.active)
				{
					VolumeComponent component = stack.GetComponent(volumeComponent.GetType());
					if (component != null)
					{
						volumeComponent.Override(component, interpFactor);
					}
				}
			}
		}

		internal void ReplaceData(VolumeStack stack)
		{
			using (VolumeManager.k_ProfilerMarkerReplaceData.Auto())
			{
				VolumeParameter[] parameters = stack.parameters;
				bool requiresResetForAllProperties = stack.requiresResetForAllProperties;
				int num = parameters.Length;
				for (int i = 0; i < num; i++)
				{
					VolumeParameter volumeParameter = parameters[i];
					if (volumeParameter.overrideState || requiresResetForAllProperties)
					{
						volumeParameter.overrideState = false;
						volumeParameter.SetValue(this.m_ParametersDefaultState[i]);
					}
				}
				stack.requiresResetForAllProperties = false;
			}
		}

		[Conditional("UNITY_EDITOR")]
		public void CheckDefaultVolumeState()
		{
			if (this.m_ComponentsDefaultState == null || (this.m_ComponentsDefaultState.Length != 0 && this.m_ComponentsDefaultState[0] == null))
			{
				this.EvaluateVolumeDefaultState();
			}
		}

		[Conditional("UNITY_EDITOR")]
		public void CheckStack(VolumeStack stack)
		{
			if (stack.components == null)
			{
				stack.Reload(this.baseComponentTypeArray);
				return;
			}
			foreach (KeyValuePair<Type, VolumeComponent> keyValuePair in stack.components)
			{
				if (keyValuePair.Key == null || keyValuePair.Value == null)
				{
					stack.Reload(this.baseComponentTypeArray);
					break;
				}
			}
		}

		private bool CheckUpdateRequired(VolumeStack stack)
		{
			if (this.m_VolumeCollection.count != 0)
			{
				stack.requiresReset = true;
				return true;
			}
			if (stack.requiresReset)
			{
				stack.requiresReset = false;
				return true;
			}
			return false;
		}

		public void Update(Transform trigger, LayerMask layerMask)
		{
			this.Update(this.stack, trigger, layerMask);
		}

		public void Update(VolumeStack stack, Transform trigger, LayerMask layerMask)
		{
			using (VolumeManager.k_ProfilerMarkerUpdate.Auto())
			{
				if (this.isInitialized)
				{
					if (this.CheckUpdateRequired(stack))
					{
						this.ReplaceData(stack);
						bool flag = trigger == null;
						Vector3 vector = flag ? Vector3.zero : trigger.position;
						List<Volume> list = this.GrabVolumes(layerMask);
						Camera camera = null;
						if (!flag)
						{
							trigger.TryGetComponent<Camera>(out camera);
						}
						int count = list.Count;
						for (int i = 0; i < count; i++)
						{
							Volume volume = list[i];
							if (!(volume == null) && volume.enabled && !(volume.profileRef == null) && volume.weight > 0f)
							{
								if (volume.isGlobal)
								{
									this.OverrideData(stack, volume, Mathf.Clamp01(volume.weight));
								}
								else if (!flag)
								{
									List<Collider> colliders = volume.colliders;
									int count2 = colliders.Count;
									if (count2 != 0)
									{
										float num = float.PositiveInfinity;
										for (int j = 0; j < count2; j++)
										{
											Collider collider = colliders[j];
											if (collider.enabled)
											{
												float sqrMagnitude = (collider.ClosestPoint(vector) - vector).sqrMagnitude;
												if (sqrMagnitude < num)
												{
													num = sqrMagnitude;
												}
											}
										}
										float num2 = volume.blendDistance * volume.blendDistance;
										if (num <= num2)
										{
											float num3 = 1f;
											if (num2 > 0f)
											{
												num3 = 1f - num / num2;
											}
											this.OverrideData(stack, volume, num3 * Mathf.Clamp01(volume.weight));
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public Volume[] GetVolumes(LayerMask layerMask)
		{
			List<Volume> list = this.GrabVolumes(layerMask);
			list.RemoveAll((Volume v) => v == null);
			return list.ToArray();
		}

		private List<Volume> GrabVolumes(LayerMask mask)
		{
			return this.m_VolumeCollection.GrabVolumes(mask);
		}

		private static bool IsVolumeRenderedByCamera(Volume volume, Camera camera)
		{
			return true;
		}

		[CompilerGenerated]
		internal static void <EvaluateVolumeDefaultState>g__ApplyDefaultProfile|59_0(VolumeProfile profile, ref VolumeManager.<>c__DisplayClass59_0 A_1)
		{
			if (profile == null)
			{
				return;
			}
			for (int i = 0; i < profile.components.Count; i++)
			{
				VolumeComponent profileComponent = profile.components[i];
				VolumeComponent volumeComponent = A_1.componentsDefaultStateList.FirstOrDefault((VolumeComponent x) => x.GetType() == profileComponent.GetType());
				if (volumeComponent != null && profileComponent.active)
				{
					profileComponent.Override(volumeComponent, 1f);
				}
			}
		}

		private static readonly ProfilerMarker k_ProfilerMarkerUpdate = new ProfilerMarker("VolumeManager.Update");

		private static readonly ProfilerMarker k_ProfilerMarkerReplaceData = new ProfilerMarker("VolumeManager.ReplaceData");

		private static readonly ProfilerMarker k_ProfilerMarkerEvaluateVolumeDefaultState = new ProfilerMarker("VolumeManager.EvaluateVolumeDefaultState");

		private static readonly Lazy<VolumeManager> s_Instance = new Lazy<VolumeManager>(() => new VolumeManager());

		private static readonly Dictionary<Type, List<ValueTuple<string, Type>>> s_SupportedVolumeComponentsForRenderPipeline = new Dictionary<Type, List<ValueTuple<string, Type>>>();

		private Type[] m_BaseComponentTypeArray;

		private readonly VolumeCollection m_VolumeCollection = new VolumeCollection();

		private VolumeComponent[] m_ComponentsDefaultState;

		internal VolumeParameter[] m_ParametersDefaultState;

		private VolumeStack m_DefaultStack;

		private readonly List<VolumeStack> m_CreatedVolumeStacks = new List<VolumeStack>();
	}
}
