using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[StaticAccessor("GetGraphicsSettings()", StaticAccessorType.Dot)]
	[NativeHeader("Runtime/Camera/GraphicsSettings.h")]
	public sealed class GraphicsSettings : Object
	{
		private GraphicsSettings()
		{
		}

		public static extern TransparencySortMode transparencySortMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		public static Vector3 transparencySortAxis
		{
			get
			{
				Vector3 result;
				GraphicsSettings.get_transparencySortAxis_Injected(out result);
				return result;
			}
			set
			{
				GraphicsSettings.set_transparencySortAxis_Injected(ref value);
			}
		}

		public static extern bool realtimeDirectRectangularAreaLights { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		public static extern bool lightsUseLinearIntensity { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		public static extern bool lightsUseColorTemperature { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		[Obsolete("This property is obsolete. Use RenderingLayerMask API and Tags & Layers project settings instead. #from(23.3)")]
		public static extern uint defaultRenderingLayerMask { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		public static extern Camera.GateFitMode defaultGateFitMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		public static extern bool useScriptableRenderPipelineBatching { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		public static extern bool logWhenShaderIsCompiled { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		public static extern bool disableBuiltinCustomRenderTextureUpdate { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		public static extern VideoShadersIncludeMode videoShadersIncludeMode { [MethodImpl(MethodImplOptions.InternalCall)] get; }

		public static extern LightProbeOutsideHullStrategy lightProbeOutsideHullStrategy { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasShaderDefine(GraphicsTier tier, BuiltinShaderDefine defineHash);

		public static bool HasShaderDefine(BuiltinShaderDefine defineHash)
		{
			return GraphicsSettings.HasShaderDefine(Graphics.activeTier, defineHash);
		}

		[NativeName("CurrentRenderPipeline")]
		private static ScriptableObject INTERNAL_currentRenderPipeline
		{
			get
			{
				return Unmarshal.UnmarshalUnityObject<ScriptableObject>(GraphicsSettings.get_INTERNAL_currentRenderPipeline_Injected());
			}
		}

		public static RenderPipelineAsset currentRenderPipeline
		{
			get
			{
				return GraphicsSettings.INTERNAL_currentRenderPipeline as RenderPipelineAsset;
			}
		}

		public static bool isScriptableRenderPipelineEnabled
		{
			get
			{
				return GraphicsSettings.INTERNAL_currentRenderPipeline != null;
			}
		}

		public static Type currentRenderPipelineAssetType
		{
			get
			{
				return GraphicsSettings.isScriptableRenderPipelineEnabled ? GraphicsSettings.INTERNAL_currentRenderPipeline.GetType() : null;
			}
		}

		[Obsolete("renderPipelineAsset has been deprecated. Use defaultRenderPipeline instead (UnityUpgradable) -> defaultRenderPipeline", false)]
		public static RenderPipelineAsset renderPipelineAsset
		{
			get
			{
				return GraphicsSettings.defaultRenderPipeline;
			}
			set
			{
				GraphicsSettings.defaultRenderPipeline = value;
			}
		}

		[NativeName("DefaultRenderPipeline")]
		private static ScriptableObject INTERNAL_defaultRenderPipeline
		{
			get
			{
				return Unmarshal.UnmarshalUnityObject<ScriptableObject>(GraphicsSettings.get_INTERNAL_defaultRenderPipeline_Injected());
			}
			set
			{
				GraphicsSettings.set_INTERNAL_defaultRenderPipeline_Injected(Object.MarshalledUnityObject.Marshal<ScriptableObject>(value));
			}
		}

		public static RenderPipelineAsset defaultRenderPipeline
		{
			get
			{
				return GraphicsSettings.INTERNAL_defaultRenderPipeline as RenderPipelineAsset;
			}
			set
			{
				GraphicsSettings.INTERNAL_defaultRenderPipeline = value;
			}
		}

		[NativeName("GetAllConfiguredRenderPipelinesForScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ScriptableObject[] GetAllConfiguredRenderPipelines();

		public static RenderPipelineAsset[] allConfiguredRenderPipelines
		{
			get
			{
				return GraphicsSettings.GetAllConfiguredRenderPipelines().Cast<RenderPipelineAsset>().ToArray<RenderPipelineAsset>();
			}
		}

		[FreeFunction]
		public static Object GetGraphicsSettings()
		{
			return Unmarshal.UnmarshalUnityObject<Object>(GraphicsSettings.GetGraphicsSettings_Injected());
		}

		[NativeName("SetShaderModeScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetShaderMode(BuiltinShaderType type, BuiltinShaderMode mode);

		[NativeName("GetShaderModeScript")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern BuiltinShaderMode GetShaderMode(BuiltinShaderType type);

		[NativeName("SetCustomShaderScript")]
		public static void SetCustomShader(BuiltinShaderType type, Shader shader)
		{
			GraphicsSettings.SetCustomShader_Injected(type, Object.MarshalledUnityObject.Marshal<Shader>(shader));
		}

		[NativeName("GetCustomShaderScript")]
		public static Shader GetCustomShader(BuiltinShaderType type)
		{
			return Unmarshal.UnmarshalUnityObject<Shader>(GraphicsSettings.GetCustomShader_Injected(type));
		}

		public static extern bool cameraRelativeLightCulling { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		public static extern bool cameraRelativeShadowCulling { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		[RequiredByNativeCode]
		[VisibleToOtherModules]
		internal static Shader GetDefaultShader(DefaultShaderType type)
		{
			RenderPipelineAsset currentRenderPipeline = GraphicsSettings.currentRenderPipeline;
			bool flag = GraphicsSettings.currentRenderPipeline == null;
			Shader result;
			if (flag)
			{
				result = null;
			}
			else
			{
				if (!true)
				{
				}
				Shader shader;
				switch (type)
				{
				case DefaultShaderType.Default:
					shader = currentRenderPipeline.defaultShader;
					break;
				case DefaultShaderType.AutodeskInteractive:
					shader = currentRenderPipeline.autodeskInteractiveShader;
					break;
				case DefaultShaderType.AutodeskInteractiveTransparent:
					shader = currentRenderPipeline.autodeskInteractiveTransparentShader;
					break;
				case DefaultShaderType.AutodeskInteractiveMasked:
					shader = currentRenderPipeline.autodeskInteractiveMaskedShader;
					break;
				case DefaultShaderType.TerrainDetailLit:
					shader = currentRenderPipeline.terrainDetailLitShader;
					break;
				case DefaultShaderType.TerrainDetailGrass:
					shader = currentRenderPipeline.terrainDetailGrassShader;
					break;
				case DefaultShaderType.TerrainDetailGrassBillboard:
					shader = currentRenderPipeline.terrainDetailGrassBillboardShader;
					break;
				case DefaultShaderType.SpeedTree7:
					shader = currentRenderPipeline.defaultSpeedTree7Shader;
					break;
				case DefaultShaderType.SpeedTree8:
					shader = currentRenderPipeline.defaultSpeedTree8Shader;
					break;
				case DefaultShaderType.SpeedTree9:
					shader = currentRenderPipeline.defaultSpeedTree9Shader;
					break;
				default:
					throw new NotImplementedException(string.Format("DefaultShaderType {0} not implemented", type));
				}
				if (!true)
				{
				}
				result = shader;
			}
			return result;
		}

		[RequiredByNativeCode]
		[VisibleToOtherModules]
		internal static Material GetDefaultMaterial(DefaultMaterialType type)
		{
			RenderPipelineAsset currentRenderPipeline = GraphicsSettings.currentRenderPipeline;
			bool flag = GraphicsSettings.currentRenderPipeline == null;
			Material result;
			if (flag)
			{
				result = null;
			}
			else
			{
				if (!true)
				{
				}
				Material material;
				switch (type)
				{
				case DefaultMaterialType.Default:
					material = currentRenderPipeline.defaultMaterial;
					break;
				case DefaultMaterialType.Particle:
					material = currentRenderPipeline.defaultParticleMaterial;
					break;
				case DefaultMaterialType.Line:
					material = currentRenderPipeline.defaultLineMaterial;
					break;
				case DefaultMaterialType.Terrain:
					material = currentRenderPipeline.defaultTerrainMaterial;
					break;
				case DefaultMaterialType.Sprite:
					material = currentRenderPipeline.default2DMaterial;
					break;
				case DefaultMaterialType.SpriteMask:
					material = currentRenderPipeline.default2DMaskMaterial;
					break;
				case DefaultMaterialType.UGUI:
					material = currentRenderPipeline.defaultUIMaterial;
					break;
				case DefaultMaterialType.UGUI_Overdraw:
					material = currentRenderPipeline.defaultUIOverdrawMaterial;
					break;
				case DefaultMaterialType.UGUI_ETC1Supported:
					material = currentRenderPipeline.defaultUIETC1SupportedMaterial;
					break;
				default:
					throw new NotImplementedException(string.Format("DefaultMaterialType {0} not implemented", type));
				}
				if (!true)
				{
				}
				result = material;
			}
			return result;
		}

		[NativeName("RegisterRenderPipelineSettings")]
		private unsafe static void Internal_RegisterRenderPipeline(string renderpipelineName, Object settings)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(renderpipelineName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = renderpipelineName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				GraphicsSettings.Internal_RegisterRenderPipeline_Injected(ref managedSpanWrapper, Object.MarshalledUnityObject.Marshal<Object>(settings));
			}
			finally
			{
				char* ptr = null;
			}
		}

		[NativeName("UnregisterRenderPipelineSettings")]
		private unsafe static void Internal_UnregisterRenderPipeline(string renderpipelineName)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(renderpipelineName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = renderpipelineName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				GraphicsSettings.Internal_UnregisterRenderPipeline_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[NativeName("GetSettingsForRenderPipeline")]
		private unsafe static Object Internal_GetSettingsForRenderPipeline(string renderpipelineName)
		{
			Object result;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(renderpipelineName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = renderpipelineName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				IntPtr gcHandlePtr = GraphicsSettings.Internal_GetSettingsForRenderPipeline_Injected(ref managedSpanWrapper);
			}
			finally
			{
				IntPtr gcHandlePtr;
				result = Unmarshal.UnmarshalUnityObject<Object>(gcHandlePtr);
				char* ptr = null;
			}
			return result;
		}

		[NativeName("CurrentRenderPipelineGlobalSettings")]
		private static Object INTERNAL_currentRenderPipelineGlobalSettings
		{
			get
			{
				return Unmarshal.UnmarshalUnityObject<Object>(GraphicsSettings.get_INTERNAL_currentRenderPipelineGlobalSettings_Injected());
			}
			set
			{
				GraphicsSettings.set_INTERNAL_currentRenderPipelineGlobalSettings_Injected(Object.MarshalledUnityObject.Marshal<Object>(value));
			}
		}

		internal static RenderPipelineGlobalSettings currentRenderPipelineGlobalSettings
		{
			get
			{
				return GraphicsSettings.INTERNAL_currentRenderPipelineGlobalSettings as RenderPipelineGlobalSettings;
			}
			set
			{
				GraphicsSettings.INTERNAL_currentRenderPipelineGlobalSettings = value;
			}
		}

		private static void CheckRenderPipelineType(Type renderPipelineType)
		{
			bool flag = renderPipelineType == null;
			if (flag)
			{
				throw new ArgumentNullException("renderPipelineType");
			}
			bool flag2 = !typeof(RenderPipeline).IsAssignableFrom(renderPipelineType);
			if (flag2)
			{
				throw new ArgumentException(string.Format("{0} must be a valid {1}", renderPipelineType, "RenderPipeline"));
			}
		}

		[Obsolete("Please use EditorGraphicsSettings.SetRenderPipelineGlobalSettingsAsset(renderPipelineType, newSettings). #from(23.2)", false)]
		public static void UpdateGraphicsSettings(RenderPipelineGlobalSettings newSettings, Type renderPipelineType)
		{
			GraphicsSettings.CheckRenderPipelineType(renderPipelineType);
			bool flag = newSettings != null;
			if (flag)
			{
				GraphicsSettings.Internal_RegisterRenderPipeline(renderPipelineType.FullName, newSettings);
			}
			else
			{
				GraphicsSettings.Internal_UnregisterRenderPipeline(renderPipelineType.FullName);
			}
		}

		[Obsolete("Please use EditorGraphicsSettings.SetRenderPipelineGlobalSettingsAsset(renderPipelineType, settings). #from(23.2)", false)]
		public static void RegisterRenderPipelineSettings(Type renderPipelineType, RenderPipelineGlobalSettings settings)
		{
			GraphicsSettings.CheckRenderPipelineType(renderPipelineType);
			GraphicsSettings.Internal_RegisterRenderPipeline(renderPipelineType.FullName, settings);
		}

		[Obsolete("Please use EditorGraphicsSettings.SetRenderPipelineGlobalSettingsAsset<TRenderPipelineType>(settings). #from(23.2)", false)]
		public static void RegisterRenderPipelineSettings<T>(RenderPipelineGlobalSettings settings) where T : RenderPipeline
		{
			GraphicsSettings.Internal_RegisterRenderPipeline(typeof(T).FullName, settings);
		}

		[Obsolete("Please use EditorGraphicsSettings.SetRenderPipelineGlobalSettingsAsset<TRenderPipelineType>(null). #from(23.2)", false)]
		public static void UnregisterRenderPipelineSettings<T>() where T : RenderPipeline
		{
			GraphicsSettings.Internal_UnregisterRenderPipeline(typeof(T).FullName);
		}

		[Obsolete("Please use EditorGraphicsSettings.SetRenderPipelineGlobalSettingsAsset(renderPipelineType, null). #from(23.2)", false)]
		public static void UnregisterRenderPipelineSettings(Type renderPipelineType)
		{
			GraphicsSettings.CheckRenderPipelineType(renderPipelineType);
			GraphicsSettings.Internal_UnregisterRenderPipeline(renderPipelineType.FullName);
		}

		public static RenderPipelineGlobalSettings GetSettingsForRenderPipeline<T>() where T : RenderPipeline
		{
			return GraphicsSettings.Internal_GetSettingsForRenderPipeline(typeof(T).FullName) as RenderPipelineGlobalSettings;
		}

		public static RenderPipelineGlobalSettings GetSettingsForRenderPipeline(Type renderPipelineType)
		{
			GraphicsSettings.CheckRenderPipelineType(renderPipelineType);
			return GraphicsSettings.Internal_GetSettingsForRenderPipeline(renderPipelineType.FullName) as RenderPipelineGlobalSettings;
		}

		private static RenderPipelineGlobalSettings Internal_GetCurrentRenderPipelineGlobalSettings()
		{
			RenderPipelineGlobalSettings result = null;
			bool flag = GraphicsSettings.currentRenderPipeline != null;
			if (flag)
			{
				result = (GraphicsSettings.Internal_GetSettingsForRenderPipeline(GraphicsSettings.currentRenderPipeline.pipelineTypeFullName) as RenderPipelineGlobalSettings);
			}
			return result;
		}

		public static bool TryGetCurrentRenderPipelineGlobalSettings(out RenderPipelineGlobalSettings asset)
		{
			asset = GraphicsSettings.s_CurrentRenderPipelineGlobalSettings.Value;
			return asset != null;
		}

		public static T GetRenderPipelineSettings<T>() where T : class, IRenderPipelineGraphicsSettings
		{
			T result;
			GraphicsSettings.TryGetRenderPipelineSettings<T>(out result);
			return result;
		}

		public static bool TryGetRenderPipelineSettings<T>(out T settings) where T : class, IRenderPipelineGraphicsSettings
		{
			settings = default(T);
			RenderPipelineGlobalSettings renderPipelineGlobalSettings;
			bool flag = !GraphicsSettings.TryGetCurrentRenderPipelineGlobalSettings(out renderPipelineGlobalSettings);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				IRenderPipelineGraphicsSettings renderPipelineGraphicsSettings;
				bool flag2 = renderPipelineGlobalSettings.TryGet(typeof(T), out renderPipelineGraphicsSettings);
				if (flag2)
				{
					settings = (renderPipelineGraphicsSettings as T);
				}
				result = (settings != null);
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_transparencySortAxis_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_transparencySortAxis_Injected([In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_INTERNAL_currentRenderPipeline_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_INTERNAL_defaultRenderPipeline_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_INTERNAL_defaultRenderPipeline_Injected(IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetGraphicsSettings_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetCustomShader_Injected(BuiltinShaderType type, IntPtr shader);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetCustomShader_Injected(BuiltinShaderType type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_RegisterRenderPipeline_Injected(ref ManagedSpanWrapper renderpipelineName, IntPtr settings);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_UnregisterRenderPipeline_Injected(ref ManagedSpanWrapper renderpipelineName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_GetSettingsForRenderPipeline_Injected(ref ManagedSpanWrapper renderpipelineName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_INTERNAL_currentRenderPipelineGlobalSettings_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_INTERNAL_currentRenderPipelineGlobalSettings_Injected(IntPtr value);

		private static Lazy<RenderPipelineGlobalSettings> s_CurrentRenderPipelineGlobalSettings = new Lazy<RenderPipelineGlobalSettings>(() => GraphicsSettings.Internal_GetCurrentRenderPipelineGlobalSettings());
	}
}
