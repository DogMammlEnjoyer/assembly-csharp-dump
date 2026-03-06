using System;

namespace UnityEngine.Rendering.Universal
{
	internal static class ShaderGlobalKeywords
	{
		public static void InitializeShaderGlobalKeywords()
		{
			ShaderGlobalKeywords.MainLightShadows = GlobalKeyword.Create("_MAIN_LIGHT_SHADOWS");
			ShaderGlobalKeywords.MainLightShadowCascades = GlobalKeyword.Create("_MAIN_LIGHT_SHADOWS_CASCADE");
			ShaderGlobalKeywords.MainLightShadowScreen = GlobalKeyword.Create("_MAIN_LIGHT_SHADOWS_SCREEN");
			ShaderGlobalKeywords.CastingPunctualLightShadow = GlobalKeyword.Create("_CASTING_PUNCTUAL_LIGHT_SHADOW");
			ShaderGlobalKeywords.AdditionalLightsVertex = GlobalKeyword.Create("_ADDITIONAL_LIGHTS_VERTEX");
			ShaderGlobalKeywords.AdditionalLightsPixel = GlobalKeyword.Create("_ADDITIONAL_LIGHTS");
			ShaderGlobalKeywords.ClusterLightLoop = GlobalKeyword.Create("_CLUSTER_LIGHT_LOOP");
			ShaderGlobalKeywords.AdditionalLightShadows = GlobalKeyword.Create("_ADDITIONAL_LIGHT_SHADOWS");
			ShaderGlobalKeywords.ReflectionProbeBoxProjection = GlobalKeyword.Create("_REFLECTION_PROBE_BOX_PROJECTION");
			ShaderGlobalKeywords.ReflectionProbeBlending = GlobalKeyword.Create("_REFLECTION_PROBE_BLENDING");
			ShaderGlobalKeywords.ReflectionProbeAtlas = GlobalKeyword.Create("_REFLECTION_PROBE_ATLAS");
			ShaderGlobalKeywords.SoftShadows = GlobalKeyword.Create("_SHADOWS_SOFT");
			ShaderGlobalKeywords.SoftShadowsLow = GlobalKeyword.Create("_SHADOWS_SOFT_LOW");
			ShaderGlobalKeywords.SoftShadowsMedium = GlobalKeyword.Create("_SHADOWS_SOFT_MEDIUM");
			ShaderGlobalKeywords.SoftShadowsHigh = GlobalKeyword.Create("_SHADOWS_SOFT_HIGH");
			ShaderGlobalKeywords.MixedLightingSubtractive = GlobalKeyword.Create("_MIXED_LIGHTING_SUBTRACTIVE");
			ShaderGlobalKeywords.LightmapShadowMixing = GlobalKeyword.Create("LIGHTMAP_SHADOW_MIXING");
			ShaderGlobalKeywords.ShadowsShadowMask = GlobalKeyword.Create("SHADOWS_SHADOWMASK");
			ShaderGlobalKeywords.LightLayers = GlobalKeyword.Create("_LIGHT_LAYERS");
			ShaderGlobalKeywords.RenderPassEnabled = GlobalKeyword.Create("_RENDER_PASS_ENABLED");
			ShaderGlobalKeywords.BillboardFaceCameraPos = GlobalKeyword.Create("BILLBOARD_FACE_CAMERA_POS");
			ShaderGlobalKeywords.LightCookies = GlobalKeyword.Create("_LIGHT_COOKIES");
			ShaderGlobalKeywords.DepthNoMsaa = GlobalKeyword.Create("_DEPTH_NO_MSAA");
			ShaderGlobalKeywords.DepthMsaa2 = GlobalKeyword.Create("_DEPTH_MSAA_2");
			ShaderGlobalKeywords.DepthMsaa4 = GlobalKeyword.Create("_DEPTH_MSAA_4");
			ShaderGlobalKeywords.DepthMsaa8 = GlobalKeyword.Create("_DEPTH_MSAA_8");
			ShaderGlobalKeywords.DBufferMRT1 = GlobalKeyword.Create("_DBUFFER_MRT1");
			ShaderGlobalKeywords.DBufferMRT2 = GlobalKeyword.Create("_DBUFFER_MRT2");
			ShaderGlobalKeywords.DBufferMRT3 = GlobalKeyword.Create("_DBUFFER_MRT3");
			ShaderGlobalKeywords.DecalNormalBlendLow = GlobalKeyword.Create("_DECAL_NORMAL_BLEND_LOW");
			ShaderGlobalKeywords.DecalNormalBlendMedium = GlobalKeyword.Create("_DECAL_NORMAL_BLEND_MEDIUM");
			ShaderGlobalKeywords.DecalNormalBlendHigh = GlobalKeyword.Create("_DECAL_NORMAL_BLEND_HIGH");
			ShaderGlobalKeywords.DecalLayers = GlobalKeyword.Create("_DECAL_LAYERS");
			ShaderGlobalKeywords.WriteRenderingLayers = GlobalKeyword.Create("_WRITE_RENDERING_LAYERS");
			ShaderGlobalKeywords.ScreenSpaceOcclusion = GlobalKeyword.Create("_SCREEN_SPACE_OCCLUSION");
			ShaderGlobalKeywords._SPOT = GlobalKeyword.Create("_SPOT");
			ShaderGlobalKeywords._DIRECTIONAL = GlobalKeyword.Create("_DIRECTIONAL");
			ShaderGlobalKeywords._POINT = GlobalKeyword.Create("_POINT");
			ShaderGlobalKeywords._DEFERRED_STENCIL = GlobalKeyword.Create("_DEFERRED_STENCIL");
			ShaderGlobalKeywords._DEFERRED_FIRST_LIGHT = GlobalKeyword.Create("_DEFERRED_FIRST_LIGHT");
			ShaderGlobalKeywords._DEFERRED_MAIN_LIGHT = GlobalKeyword.Create("_DEFERRED_MAIN_LIGHT");
			ShaderGlobalKeywords._GBUFFER_NORMALS_OCT = GlobalKeyword.Create("_GBUFFER_NORMALS_OCT");
			ShaderGlobalKeywords._DEFERRED_MIXED_LIGHTING = GlobalKeyword.Create("_DEFERRED_MIXED_LIGHTING");
			ShaderGlobalKeywords.LIGHTMAP_ON = GlobalKeyword.Create("LIGHTMAP_ON");
			ShaderGlobalKeywords.DYNAMICLIGHTMAP_ON = GlobalKeyword.Create("DYNAMICLIGHTMAP_ON");
			ShaderGlobalKeywords._ALPHATEST_ON = GlobalKeyword.Create("_ALPHATEST_ON");
			ShaderGlobalKeywords.DIRLIGHTMAP_COMBINED = GlobalKeyword.Create("DIRLIGHTMAP_COMBINED");
			ShaderGlobalKeywords._DETAIL_MULX2 = GlobalKeyword.Create("_DETAIL_MULX2");
			ShaderGlobalKeywords._DETAIL_SCALED = GlobalKeyword.Create("_DETAIL_SCALED");
			ShaderGlobalKeywords._CLEARCOAT = GlobalKeyword.Create("_CLEARCOAT");
			ShaderGlobalKeywords._CLEARCOATMAP = GlobalKeyword.Create("_CLEARCOATMAP");
			ShaderGlobalKeywords.DEBUG_DISPLAY = GlobalKeyword.Create("DEBUG_DISPLAY");
			ShaderGlobalKeywords.LOD_FADE_CROSSFADE = GlobalKeyword.Create("LOD_FADE_CROSSFADE");
			ShaderGlobalKeywords.USE_UNITY_CROSSFADE = GlobalKeyword.Create("USE_UNITY_CROSSFADE");
			ShaderGlobalKeywords._EMISSION = GlobalKeyword.Create("_EMISSION");
			ShaderGlobalKeywords._RECEIVE_SHADOWS_OFF = GlobalKeyword.Create("_RECEIVE_SHADOWS_OFF");
			ShaderGlobalKeywords._SURFACE_TYPE_TRANSPARENT = GlobalKeyword.Create("_SURFACE_TYPE_TRANSPARENT");
			ShaderGlobalKeywords._ALPHAPREMULTIPLY_ON = GlobalKeyword.Create("_ALPHAPREMULTIPLY_ON");
			ShaderGlobalKeywords._ALPHAMODULATE_ON = GlobalKeyword.Create("_ALPHAMODULATE_ON");
			ShaderGlobalKeywords._NORMALMAP = GlobalKeyword.Create("_NORMALMAP");
			ShaderGlobalKeywords._ADD_PRECOMPUTED_VELOCITY = GlobalKeyword.Create("_ADD_PRECOMPUTED_VELOCITY");
			ShaderGlobalKeywords.EDITOR_VISUALIZATION = GlobalKeyword.Create("EDITOR_VISUALIZATION");
			ShaderGlobalKeywords.FoveatedRenderingNonUniformRaster = GlobalKeyword.Create("_FOVEATED_RENDERING_NON_UNIFORM_RASTER");
			ShaderGlobalKeywords.DisableTexture2DXArray = GlobalKeyword.Create("DISABLE_TEXTURE2D_X_ARRAY");
			ShaderGlobalKeywords.BlitSingleSlice = GlobalKeyword.Create("BLIT_SINGLE_SLICE");
			ShaderGlobalKeywords.XROcclusionMeshCombined = GlobalKeyword.Create("XR_OCCLUSION_MESH_COMBINED");
			ShaderGlobalKeywords.SCREEN_COORD_OVERRIDE = GlobalKeyword.Create("SCREEN_COORD_OVERRIDE");
			ShaderGlobalKeywords.DOWNSAMPLING_SIZE_2 = GlobalKeyword.Create("DOWNSAMPLING_SIZE_2");
			ShaderGlobalKeywords.DOWNSAMPLING_SIZE_4 = GlobalKeyword.Create("DOWNSAMPLING_SIZE_4");
			ShaderGlobalKeywords.DOWNSAMPLING_SIZE_8 = GlobalKeyword.Create("DOWNSAMPLING_SIZE_8");
			ShaderGlobalKeywords.DOWNSAMPLING_SIZE_16 = GlobalKeyword.Create("DOWNSAMPLING_SIZE_16");
			ShaderGlobalKeywords.EVALUATE_SH_MIXED = GlobalKeyword.Create("EVALUATE_SH_MIXED");
			ShaderGlobalKeywords.EVALUATE_SH_VERTEX = GlobalKeyword.Create("EVALUATE_SH_VERTEX");
			ShaderGlobalKeywords.ProbeVolumeL1 = GlobalKeyword.Create("PROBE_VOLUMES_L1");
			ShaderGlobalKeywords.ProbeVolumeL2 = GlobalKeyword.Create("PROBE_VOLUMES_L2");
			ShaderGlobalKeywords.LIGHTMAP_BICUBIC_SAMPLING = GlobalKeyword.Create("LIGHTMAP_BICUBIC_SAMPLING");
			ShaderGlobalKeywords._OUTPUT_DEPTH = GlobalKeyword.Create("_OUTPUT_DEPTH");
			ShaderGlobalKeywords.LinearToSRGBConversion = GlobalKeyword.Create("_LINEAR_TO_SRGB_CONVERSION");
			ShaderGlobalKeywords._ENABLE_ALPHA_OUTPUT = GlobalKeyword.Create("_ENABLE_ALPHA_OUTPUT");
			ShaderGlobalKeywords.ForwardPlus = GlobalKeyword.Create("_FORWARD_PLUS");
		}

		public static GlobalKeyword MainLightShadows;

		public static GlobalKeyword MainLightShadowCascades;

		public static GlobalKeyword MainLightShadowScreen;

		public static GlobalKeyword CastingPunctualLightShadow;

		public static GlobalKeyword AdditionalLightsVertex;

		public static GlobalKeyword AdditionalLightsPixel;

		public static GlobalKeyword ClusterLightLoop;

		public static GlobalKeyword AdditionalLightShadows;

		public static GlobalKeyword ReflectionProbeBoxProjection;

		public static GlobalKeyword ReflectionProbeBlending;

		public static GlobalKeyword ReflectionProbeAtlas;

		public static GlobalKeyword SoftShadows;

		public static GlobalKeyword SoftShadowsLow;

		public static GlobalKeyword SoftShadowsMedium;

		public static GlobalKeyword SoftShadowsHigh;

		public static GlobalKeyword MixedLightingSubtractive;

		public static GlobalKeyword LightmapShadowMixing;

		public static GlobalKeyword ShadowsShadowMask;

		public static GlobalKeyword LightLayers;

		public static GlobalKeyword RenderPassEnabled;

		public static GlobalKeyword BillboardFaceCameraPos;

		public static GlobalKeyword LightCookies;

		public static GlobalKeyword DepthNoMsaa;

		public static GlobalKeyword DepthMsaa2;

		public static GlobalKeyword DepthMsaa4;

		public static GlobalKeyword DepthMsaa8;

		public static GlobalKeyword DBufferMRT1;

		public static GlobalKeyword DBufferMRT2;

		public static GlobalKeyword DBufferMRT3;

		public static GlobalKeyword DecalNormalBlendLow;

		public static GlobalKeyword DecalNormalBlendMedium;

		public static GlobalKeyword DecalNormalBlendHigh;

		public static GlobalKeyword DecalLayers;

		public static GlobalKeyword WriteRenderingLayers;

		public static GlobalKeyword ScreenSpaceOcclusion;

		public static GlobalKeyword _SPOT;

		public static GlobalKeyword _DIRECTIONAL;

		public static GlobalKeyword _POINT;

		public static GlobalKeyword _DEFERRED_STENCIL;

		public static GlobalKeyword _DEFERRED_FIRST_LIGHT;

		public static GlobalKeyword _DEFERRED_MAIN_LIGHT;

		public static GlobalKeyword _GBUFFER_NORMALS_OCT;

		public static GlobalKeyword _DEFERRED_MIXED_LIGHTING;

		public static GlobalKeyword LIGHTMAP_ON;

		public static GlobalKeyword DYNAMICLIGHTMAP_ON;

		public static GlobalKeyword _ALPHATEST_ON;

		public static GlobalKeyword DIRLIGHTMAP_COMBINED;

		public static GlobalKeyword _DETAIL_MULX2;

		public static GlobalKeyword _DETAIL_SCALED;

		public static GlobalKeyword _CLEARCOAT;

		public static GlobalKeyword _CLEARCOATMAP;

		public static GlobalKeyword DEBUG_DISPLAY;

		public static GlobalKeyword LOD_FADE_CROSSFADE;

		public static GlobalKeyword USE_UNITY_CROSSFADE;

		public static GlobalKeyword _EMISSION;

		public static GlobalKeyword _RECEIVE_SHADOWS_OFF;

		public static GlobalKeyword _SURFACE_TYPE_TRANSPARENT;

		public static GlobalKeyword _ALPHAPREMULTIPLY_ON;

		public static GlobalKeyword _ALPHAMODULATE_ON;

		public static GlobalKeyword _NORMALMAP;

		public static GlobalKeyword _ADD_PRECOMPUTED_VELOCITY;

		public static GlobalKeyword EDITOR_VISUALIZATION;

		public static GlobalKeyword FoveatedRenderingNonUniformRaster;

		public static GlobalKeyword DisableTexture2DXArray;

		public static GlobalKeyword BlitSingleSlice;

		public static GlobalKeyword XROcclusionMeshCombined;

		public static GlobalKeyword SCREEN_COORD_OVERRIDE;

		public static GlobalKeyword DOWNSAMPLING_SIZE_2;

		public static GlobalKeyword DOWNSAMPLING_SIZE_4;

		public static GlobalKeyword DOWNSAMPLING_SIZE_8;

		public static GlobalKeyword DOWNSAMPLING_SIZE_16;

		public static GlobalKeyword EVALUATE_SH_MIXED;

		public static GlobalKeyword EVALUATE_SH_VERTEX;

		public static GlobalKeyword ProbeVolumeL1;

		public static GlobalKeyword ProbeVolumeL2;

		public static GlobalKeyword LIGHTMAP_BICUBIC_SAMPLING;

		public static GlobalKeyword _OUTPUT_DEPTH;

		public static GlobalKeyword LinearToSRGBConversion;

		public static GlobalKeyword _ENABLE_ALPHA_OUTPUT;

		public static GlobalKeyword ForwardPlus;
	}
}
