using System;
using Unity.Mathematics;
using UnityEngine;

public struct UberShaderMatUsedProps
{
	public UberShaderMatUsedProps(Material mat)
	{
		this.material = mat;
		this.kw = new GTUberShader_MaterialKeywordStates(mat);
		this._notAProp = 0;
		this._TransparencyMode = 1;
		this._Cutoff = 0;
		this._ColorSource = 0;
		this._BaseColor = 0;
		this._GChannelColor = 0;
		this._BChannelColor = 0;
		this._AChannelColor = 0;
		this._BaseMap = 0;
		this._BaseMap_ST = 0;
		this._SettingsPreset = 0;
		this._AdvancedOptions = 0;
		this._TexMipBias = 0;
		this._BaseMap_WH = 0;
		this._TexelSnapToggle = 0;
		this._TexelSnap_Factor = 0;
		this._UVSource = 0;
		this._AlphaDetailToggle = 0;
		this._AlphaDetail_ST = 0;
		this._AlphaDetail_Opacity = 0;
		this._AlphaDetail_WorldSpace = 0;
		this._MaskMapToggle = 0;
		this._MaskMap = 0;
		this._MaskMap_ST = 0;
		this._MaskMap_WH = 0;
		this._LavaLampToggle = 0;
		this._GradientMapToggle = 0;
		this._GradientMap = 0;
		this._DoTextureRotation = 0;
		this._RotateAngle = 0;
		this._RotateAnim = 0;
		this._UseWaveWarp = 0;
		this._WaveAmplitude = 0;
		this._WaveFrequency = 0;
		this._WaveScale = 0;
		this._WaveTimeScale = 0;
		this._UseWeatherMap = 0;
		this._WeatherMap = 0;
		this._WeatherMapDissolveEdgeSize = 0;
		this._ReflectToggle = 0;
		this._ReflectBoxProjectToggle = 0;
		this._ReflectBoxCubePos = 0;
		this._ReflectBoxSize = 0;
		this._ReflectBoxRotation = 0;
		this._ReflectMatcapToggle = 0;
		this._ReflectMatcapPerspToggle = 0;
		this._ReflectNormalToggle = 0;
		this._ReflectTex = 0;
		this._ReflectNormalTex = 0;
		this._ReflectAlbedoTint = 0;
		this._ReflectTint = 0;
		this._ReflectOpacity = 0;
		this._ReflectExposure = 0;
		this._ReflectOffset = 0;
		this._ReflectScale = 0;
		this._ReflectRotate = 0;
		this._HalfLambertToggle = 0;
		this._ParallaxPlanarToggle = 0;
		this._ParallaxToggle = 0;
		this._ParallaxAAToggle = 0;
		this._ParallaxAABias = 0;
		this._DepthMap = 0;
		this._ParallaxAmplitude = 0;
		this._ParallaxSamplesMinMax = 0;
		this._UvShiftToggle = 0;
		this._UvShiftSteps = 0;
		this._UvShiftRate = 0;
		this._UvShiftOffset = 0;
		this._UseGridEffect = 0;
		this._UseCrystalEffect = 0;
		this._CrystalPower = 0;
		this._CrystalRimColor = 0;
		this._LiquidVolume = 0;
		this._LiquidFill = 0;
		this._LiquidFillNormal = 0;
		this._LiquidSurfaceColor = 0;
		this._LiquidSwayX = 0;
		this._LiquidSwayY = 0;
		this._LiquidContainer = 0;
		this._LiquidPlanePosition = 0;
		this._LiquidPlaneNormal = 0;
		this._VertexFlapToggle = 0;
		this._VertexFlapAxis = 0;
		this._VertexFlapDegreesMinMax = 0;
		this._VertexFlapSpeed = 0;
		this._VertexFlapPhaseOffset = 0;
		this._VertexWaveToggle = 0;
		this._VertexWaveDebug = 0;
		this._VertexWaveEnd = 0;
		this._VertexWaveParams = 0;
		this._VertexWaveFalloff = 0;
		this._VertexWaveSphereMask = 0;
		this._VertexWavePhaseOffset = 0;
		this._VertexWaveAxes = 0;
		this._VertexRotateToggle = 0;
		this._VertexRotateAngles = 0;
		this._VertexRotateAnim = 0;
		this._VertexLightToggle = 0;
		this._InnerGlowOn = 0;
		this._InnerGlowColor = 0;
		this._InnerGlowParams = 0;
		this._InnerGlowTap = 0;
		this._InnerGlowSine = 0;
		this._InnerGlowSinePeriod = 0;
		this._InnerGlowSinePhaseShift = 0;
		this._StealthEffectOn = 0;
		this._UseEyeTracking = 0;
		this._EyeTileOffsetUV = 0;
		this._EyeOverrideUV = 0;
		this._EyeOverrideUVTransform = 0;
		this._UseMouthFlap = 0;
		this._MouthMap = 0;
		this._MouthMap_ST = 0;
		this._UseVertexColor = 0;
		this._WaterEffect = 0;
		this._HeightBasedWaterEffect = 0;
		this._WaterCaustics = 0;
		this._UseDayNightLightmap = 0;
		this._UseSpecular = 0;
		this._UseSpecularAlphaChannel = 0;
		this._Smoothness = 0;
		this._UseSpecHighlight = 0;
		this._SpecularDir = 0;
		this._SpecularPowerIntensity = 0;
		this._SpecularColor = 0;
		this._SpecularUseDiffuseColor = 0;
		this._EmissionToggle = 0;
		this._EmissionColor = 0;
		this._EmissionMap = 0;
		this._EmissionMaskByBaseMapAlpha = 0;
		this._EmissionUVScrollSpeed = 0;
		this._EmissionDissolveProgress = 0;
		this._EmissionDissolveAnimation = 0;
		this._EmissionDissolveEdgeSize = 0;
		this._EmissionIntensityInDynamic = 0;
		this._EmissionUseUVWaveWarp = 0;
		this._GreyZoneException = 0;
		this._Cull = 1;
		this._StencilReference = 1;
		this._StencilComparison = 1;
		this._StencilPassFront = 1;
		this._USE_DEFORM_MAP = 0;
		this._DeformMap = 0;
		this._DeformMapIntensity = 0;
		this._DeformMapMaskByVertColorRAmount = 0;
		this._DeformMapScrollSpeed = 0;
		this._DeformMapUV0Influence = 0;
		this._DeformMapObjectSpaceOffsetsU = 0;
		this._DeformMapObjectSpaceOffsetsV = 0;
		this._DeformMapWorldSpaceOffsetsU = 0;
		this._DeformMapWorldSpaceOffsetsV = 0;
		this._RotateOnYAxisBySinTime = 0;
		this._USE_TEX_ARRAY_ATLAS = 0;
		this._BaseMap_Atlas = 0;
		this._BaseMap_AtlasSlice = 0;
		this._BaseMap_AtlasSliceSource = 0;
		this._EmissionMap_Atlas = 0;
		this._EmissionMap_AtlasSlice = 0;
		this._DeformMap_Atlas = 0;
		this._DeformMap_AtlasSlice = 0;
		this._WeatherMap_Atlas = 0;
		this._WeatherMap_AtlasSlice = 0;
		this._DEBUG_PAWN_DATA = 0;
		this._SrcBlend = 1;
		this._DstBlend = 1;
		this._SrcBlendAlpha = 1;
		this._DstBlendAlpha = 1;
		this._ZWrite = 1;
		this._AlphaToMask = 1;
		this._Color = 0;
		this._Surface = 0;
		this._Metallic = 0;
		this._SpecColor = 0;
		this._DayNightLightmapArray = 0;
		this._DayNightLightmapArray_ST = 0;
		this._DayNightLightmapArray_AtlasSlice = 0;
		if (!this.kw._USE_TEXTURE)
		{
			bool use_TEXTURE__AS_MASK = this.kw.USE_TEXTURE__AS_MASK;
		}
		int num = 1;
		UberShaderMatUsedProps._g_Macro_DECLARE_ATLASABLE_TEX2D(this.kw, ref this._BaseMap, ref this._BaseMap_Atlas);
		if (this.kw._MASK_MAP_ON)
		{
			this._MaskMap++;
		}
		if (this.kw._GRADIENT_MAP_ON)
		{
			this._GradientMap++;
		}
		if (this.kw._USE_WEATHER_MAP)
		{
			UberShaderMatUsedProps._g_Macro_DECLARE_ATLASABLE_TEX2D(this.kw, ref this._WeatherMap, ref this._WeatherMap_Atlas);
		}
		if (this.kw._EMISSION || this.kw._CRYSTAL_EFFECT)
		{
			UberShaderMatUsedProps._g_Macro_DECLARE_ATLASABLE_TEX2D(this.kw, ref this._EmissionMap, ref this._EmissionMap_Atlas);
		}
		if (this.kw._USE_DEFORM_MAP)
		{
			UberShaderMatUsedProps._g_Macro_DECLARE_ATLASABLE_TEX2D(this.kw, ref this._DeformMap, ref this._DeformMap_Atlas);
		}
		bool flag = this.kw._ALPHA_DETAIL_MAP && (this.kw._USE_TEXTURE || this.kw.USE_TEXTURE__AS_MASK);
		bool flag2 = this.kw._WATER_EFFECT || this.kw._STEALTH_EFFECT || this.kw._ALPHA_BLUE_LIVE_ON;
		bool flag3 = this.kw._LIQUID_VOLUME || this.kw._INNER_GLOW || this.kw._VERTEX_ANIM_WAVE_DEBUG;
		bool flag4 = this.kw._WATER_EFFECT || this.kw._STEALTH_EFFECT;
		if (this.kw._REFLECTIONS)
		{
			this._ReflectTex++;
			if (this.kw._REFLECTIONS_USE_NORMAL_TEX)
			{
				this._ReflectNormalTex++;
			}
		}
		if (this.kw._PARALLAX)
		{
			this._DepthMap++;
		}
		if (this.kw.LIGHTMAP_ON)
		{
			bool use_DAY_NIGHT_LIGHTMAP = this.kw._USE_DAY_NIGHT_LIGHTMAP;
		}
		if (this.kw.LIGHTMAP_ON)
		{
			bool dirlightmap_COMBINED = this.kw.DIRLIGHTMAP_COMBINED;
		}
		bool use_WEATHER_MAP = this.kw._USE_WEATHER_MAP;
		if (this.kw._WATER_EFFECT)
		{
			if (!this.kw._WATER_CAUSTICS)
			{
				bool global_ZONE_LIQUID_TYPE__LAVA = this.kw._GLOBAL_ZONE_LIQUID_TYPE__LAVA;
			}
			if (this.kw._HEIGHT_BASED_WATER_EFFECT)
			{
				bool zone_LIQUID_SHAPE__CYLINDER = this.kw._ZONE_LIQUID_SHAPE__CYLINDER;
			}
		}
		bool eyecomp = this.kw._EYECOMP;
		if (this.kw._MOUTHCOMP)
		{
			this._MouthMap++;
		}
		if (this.kw._USE_TEXTURE || this.kw.USE_TEXTURE__AS_MASK || this.kw._USE_WEATHER_MAP || this.kw._EMISSION || this.kw._USE_DEFORM_MAP || this.kw._REFLECTIONS)
		{
			bool gt_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z = this.kw._GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z;
		}
		if (!this.kw._USE_VERTEX_COLOR && !this.kw._USE_DEFORM_MAP && !this.kw._VERTEX_ANIM_FLAP)
		{
			bool vertex_ANIM_WAVE = this.kw._VERTEX_ANIM_WAVE;
		}
		bool lightmap_ON = this.kw.LIGHTMAP_ON;
		if (num == 0 && !this.kw._PARALLAX)
		{
			bool parallax_PLANAR = this.kw._PARALLAX_PLANAR;
		}
		bool mouthcomp = this.kw._MOUTHCOMP;
		if (this.kw._USE_TEXTURE || this.kw.USE_TEXTURE__AS_MASK || this.kw._EMISSION || this.kw._REFLECTIONS)
		{
			bool gt_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z2 = this.kw._GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z;
		}
		bool inner_GLOW = this.kw._INNER_GLOW;
		if (!this.kw._USE_VERTEX_COLOR && !this.kw._VERTEX_ANIM_FLAP)
		{
			bool vertex_ANIM_WAVE2 = this.kw._VERTEX_ANIM_WAVE;
		}
		bool lightmap_ON2 = this.kw.LIGHTMAP_ON;
		if (num == 0 && !this.kw._PARALLAX)
		{
			bool parallax_PLANAR2 = this.kw._PARALLAX_PLANAR;
		}
		if (!this.kw._PARALLAX)
		{
			bool parallax_PLANAR3 = this.kw._PARALLAX_PLANAR;
		}
		bool water_EFFECT = this.kw._WATER_EFFECT;
		if (!this.kw._EMISSION)
		{
			bool crystal_EFFECT = this.kw._CRYSTAL_EFFECT;
		}
		bool liquid_VOLUME = this.kw._LIQUID_VOLUME;
		if (this.kw._REFLECTIONS)
		{
			bool reflections_MATCAP = this.kw._REFLECTIONS_MATCAP;
		}
		bool mouthcomp2 = this.kw._MOUTHCOMP;
		bool zone_DYNAMIC_LIGHTS__CUSTOMVERTEX = this.kw._ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX;
		if (this.kw._VERTEX_ROTATE)
		{
			this._VertexRotateAngles++;
		}
		if (this.kw._USE_DEFORM_MAP)
		{
			this._DeformMapUV0Influence++;
			this._DeformMapObjectSpaceOffsetsU++;
			this._DeformMapObjectSpaceOffsetsV++;
			this._DeformMapScrollSpeed++;
			UberShaderMatUsedProps._g_Macro_SAMPLE_ATLASABLE_TEX2D_LOD(this.kw, ref this._DeformMap, ref this._DeformMap_Atlas);
			this._DeformMapIntensity++;
			this._DeformMapMaskByVertColorRAmount++;
			this._RotateOnYAxisBySinTime++;
		}
		if (this.kw._VERTEX_ANIM_FLAP)
		{
			this._VertexFlapSpeed++;
			this._VertexFlapPhaseOffset++;
			this._VertexFlapDegreesMinMax++;
			this._VertexFlapAxis++;
		}
		if (this.kw._VERTEX_ANIM_WAVE)
		{
			this._VertexWavePhaseOffset++;
			this._VertexWaveParams++;
			this._VertexWaveParams++;
			this._VertexWaveParams++;
			this._VertexWaveParams++;
			this._VertexWaveEnd += 2;
			this._VertexWaveFalloff += 2;
			this._VertexWaveSphereMask++;
			this._VertexWaveAxes++;
			this._VertexWaveAxes++;
			this._VertexWaveAxes++;
			this._VertexWaveAxes++;
		}
		if (this.kw._LIQUID_VOLUME)
		{
			this._LiquidFill++;
			this._LiquidFillNormal++;
			this._LiquidSwayX++;
			this._LiquidSwayY++;
			this._LiquidFill++;
		}
		if (this.kw._USE_TEXTURE || this.kw.USE_TEXTURE__AS_MASK || this.kw._EMISSION)
		{
			bool uv_SOURCE__WORLD_PLANAR_Y = this.kw._UV_SOURCE__WORLD_PLANAR_Y;
			if (this.kw._MAINTEX_ROTATE)
			{
				this._RotateAngle++;
				this._RotateAnim++;
			}
			if (this.kw._UV_WAVE_WARP)
			{
				this._WaveAmplitude++;
				this._WaveFrequency++;
				this._WaveScale++;
			}
			if (this.kw._UV_SHIFT)
			{
				this._UvShiftRate++;
				this._UvShiftSteps++;
				this._UvShiftOffset++;
			}
			UberShaderMatUsedProps._g_Macro_TRANSFORM_TEX(this.kw, ref this._BaseMap, ref this._BaseMap_ST);
			bool gt_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z3 = this.kw._GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z;
			if (this.kw._EYECOMP)
			{
				this._BaseMap_ST++;
				this._EyeOverrideUVTransform++;
				this._EyeOverrideUV += 2;
			}
			if (this.kw._EMISSION)
			{
				this._EmissionUVScrollSpeed += 2;
				this._BaseMap_ST += 2;
				if (this.kw._EMISSION_USE_UV_WAVE_WARP)
				{
					this._WaveAmplitude++;
					this._WaveFrequency++;
					this._WaveScale++;
				}
			}
		}
		if (!this.kw._USE_VERTEX_COLOR && !this.kw._VERTEX_ANIM_FLAP)
		{
			bool vertex_ANIM_WAVE3 = this.kw._VERTEX_ANIM_WAVE;
		}
		bool lightmap_ON3 = this.kw.LIGHTMAP_ON;
		if (this.kw._WATER_EFFECT)
		{
			bool water_CAUSTICS = this.kw._WATER_CAUSTICS;
		}
		if (this.kw._REFLECTIONS && this.kw._REFLECTIONS_MATCAP)
		{
			bool reflections_MATCAP_PERSP_AWARE = this.kw._REFLECTIONS_MATCAP_PERSP_AWARE;
		}
		if (this.kw._MOUTHCOMP)
		{
			UberShaderMatUsedProps._g_Macro_TRANSFORM_TEX(this.kw, ref this._MouthMap, ref this._MouthMap_ST);
		}
		if (!this.kw._PARALLAX)
		{
			bool parallax_PLANAR4 = this.kw._PARALLAX_PLANAR;
		}
		if (this.kw._INNER_GLOW)
		{
			this._InnerGlowParams += 2;
			this._InnerGlowSinePeriod++;
			this._InnerGlowSinePhaseShift++;
			this._InnerGlowSinePeriod++;
			this._InnerGlowTap++;
		}
		if (this.kw._ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX)
		{
			bool zone_DYNAMIC_LIGHTS__CUSTOMVERTEX2 = this.kw._ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX;
		}
		this._BaseColor++;
		if (this.kw._USE_TEXTURE || this.kw.USE_TEXTURE__AS_MASK)
		{
			if (this.kw._TEXEL_SNAP_UVS)
			{
				this._BaseMap_WH++;
				this._TexelSnap_Factor++;
				this._TexelSnap_Factor++;
			}
			if (!this.kw._PARALLAX)
			{
				bool parallax_PLANAR5 = this.kw._PARALLAX_PLANAR;
			}
			if (this.kw._PARALLAX)
			{
				this._ParallaxSamplesMinMax += 2;
				this._DepthMap++;
				this._ParallaxAmplitude++;
				if (this.kw._PARALLAX_AA)
				{
					this._BaseMap_WH++;
					this._ParallaxAABias++;
				}
			}
			else if (this.kw._PARALLAX_PLANAR)
			{
				this._ParallaxAmplitude++;
			}
			if (this.kw._USE_TEX_ARRAY_ATLAS && this.kw._GT_BASE_MAP_ATLAS_SLICE_SOURCE__UV1_Z)
			{
				this._BaseMap_AtlasSlice++;
			}
			UberShaderMatUsedProps._g_Macro_SAMPLE_ATLASABLE_TEX2D(this.kw, ref this._BaseMap, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._TexMipBias);
			if (this.kw.USE_TEXTURE__AS_MASK)
			{
				this._BaseColor++;
				this._GChannelColor++;
				this._BChannelColor++;
				this._AChannelColor++;
			}
			if (this.kw._ALPHA_DETAIL_MAP)
			{
				this._AlphaDetail_ST += 2;
				this._BaseMap_WH++;
				UberShaderMatUsedProps._g_Macro_SAMPLE_ATLASABLE_TEX2D(this.kw, ref this._BaseMap, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._TexMipBias);
				this._AlphaDetail_Opacity++;
			}
		}
		if (this.kw._USE_WEATHER_MAP)
		{
			UberShaderMatUsedProps._g_Macro_SAMPLE_ATLASABLE_TEX2D(this.kw, ref this._WeatherMap, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._TexMipBias);
			this._WeatherMapDissolveEdgeSize++;
		}
		if (this.kw._EYECOMP)
		{
			UberShaderMatUsedProps._g_Macro_SAMPLE_ATLASABLE_TEX2D(this.kw, ref this._BaseMap, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._TexMipBias);
			this._EyeTileOffsetUV++;
			this._EyeTileOffsetUV++;
			this._EyeTileOffsetUV++;
			this._EyeTileOffsetUV++;
			UberShaderMatUsedProps._g_Macro_SAMPLE_ATLASABLE_TEX2D(this.kw, ref this._BaseMap, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._TexMipBias);
		}
		if (this.kw._MOUTHCOMP)
		{
			this._MouthMap++;
		}
		bool use_VERTEX_COLOR = this.kw._USE_VERTEX_COLOR;
		if (this.kw.LIGHTMAP_ON && this.kw._USE_DAY_NIGHT_LIGHTMAP && this.kw.DIRLIGHTMAP_COMBINED)
		{
			bool unity_EDIT_MODE = this.kw._UNITY_EDIT_MODE;
		}
		if (this.kw._CRYSTAL_EFFECT)
		{
			this._CrystalPower++;
			this._CrystalRimColor += 2;
		}
		if (this.kw._USE_TEXTURE && this.kw._MASK_MAP_ON && this.kw._FX_LAVA_LAMP && this.kw._GRADIENT_MAP_ON)
		{
			this._MaskMap_ST += 2;
			this._MaskMap++;
			this._GradientMap++;
		}
		if (this.kw._USE_TEXTURE && this.kw._GRID_EFFECT)
		{
			this._BaseColor++;
			this._BaseMap_WH++;
		}
		if (this.kw._REFLECTIONS)
		{
			if (!this.kw._REFLECTIONS_MATCAP)
			{
				if (this.kw._REFLECTIONS_BOX_PROJECT)
				{
					this._ReflectBoxSize++;
					this._ReflectBoxCubePos++;
					this._ReflectBoxCubePos++;
					this._ReflectBoxRotation++;
					this._ReflectBoxCubePos++;
				}
				this._ReflectRotate++;
				this._ReflectOffset++;
				this._ReflectScale++;
			}
			if (this.kw._REFLECTIONS_USE_NORMAL_TEX)
			{
				this._ReflectNormalTex++;
			}
			this._ReflectTex++;
			if (this.kw._REFLECTIONS_ALBEDO_TINT)
			{
				this._ReflectTint++;
			}
			else
			{
				this._ReflectTint++;
			}
			this._ReflectOpacity++;
			this._ReflectExposure++;
		}
		bool half_LAMBERT_TERM = this.kw._HALF_LAMBERT_TERM;
		if (this.kw._GT_RIM_LIGHT)
		{
			this._Smoothness++;
			if (this.kw._USE_TEXTURE)
			{
				bool gt_RIM_LIGHT_USE_ALPHA = this.kw._GT_RIM_LIGHT_USE_ALPHA;
			}
		}
		if (this.kw._SPECULAR_HIGHLIGHT)
		{
			this._SpecularPowerIntensity++;
			this._SpecularPowerIntensity++;
			this._SpecularDir++;
			this._SpecularColor++;
			this._SpecularColor++;
			if (this.kw._USE_TEXTURE)
			{
				this._SpecularUseDiffuseColor++;
				mat.GetInt("_SpecularUseDiffuseColor");
			}
		}
		if (this.kw._EMISSION || this.kw._CRYSTAL_EFFECT)
		{
			this._EmissionColor += 2;
			if (this.kw._ALPHA_DETAIL_MAP)
			{
				this._AlphaDetail_Opacity++;
			}
			if (this.kw._PARALLAX)
			{
				this._DepthMap++;
				this._ParallaxAmplitude++;
			}
			else if (this.kw._PARALLAX_PLANAR)
			{
				this._ParallaxAmplitude++;
			}
			UberShaderMatUsedProps._g_Macro_SAMPLE_ATLASABLE_TEX2D(this.kw, ref this._EmissionMap, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._notAProp, ref this._TexMipBias);
			this._EmissionDissolveProgress++;
			this._EmissionDissolveEdgeSize++;
			this._EmissionDissolveAnimation += 2;
			this._EmissionMaskByBaseMapAlpha++;
			bool zone_DYNAMIC_LIGHTS__CUSTOMVERTEX3 = this.kw._ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX;
		}
		if (this.kw._INNER_GLOW)
		{
			this._InnerGlowColor++;
		}
		if (this.kw._WATER_EFFECT)
		{
			bool global_ZONE_LIQUID_TYPE__LAVA2 = this.kw._GLOBAL_ZONE_LIQUID_TYPE__LAVA;
			bool height_BASED_WATER_EFFECT = this.kw._HEIGHT_BASED_WATER_EFFECT;
			if (this.kw._WATER_CAUSTICS)
			{
				bool global_ZONE_LIQUID_TYPE__LAVA3 = this.kw._GLOBAL_ZONE_LIQUID_TYPE__LAVA;
			}
			bool use_TEXTURE = this.kw._USE_TEXTURE;
			if (this.kw._HEIGHT_BASED_WATER_EFFECT)
			{
				bool zone_LIQUID_SHAPE__CYLINDER2 = this.kw._ZONE_LIQUID_SHAPE__CYLINDER;
			}
		}
		bool flag5 = !this.kw._LIQUID_CONTAINER;
		if (this.kw._LIQUID_VOLUME && flag5)
		{
			this._LiquidSwayX++;
			this._LiquidSwayY++;
			if (this.kw._USE_TEXTURE)
			{
				this._LiquidSurfaceColor++;
			}
			else
			{
				this._LiquidSurfaceColor++;
			}
		}
		if (this.kw._VERTEX_ANIM_WAVE_DEBUG)
		{
			this._VertexWaveEnd += 2;
			this._VertexWaveFalloff += 2;
			this._VertexWaveSphereMask++;
		}
		bool debug_PAWN_DATA = this.kw._DEBUG_PAWN_DATA;
		if (!this.kw._COLOR_GRADE_PROTANOMALY && !this.kw._COLOR_GRADE_PROTANOPIA && !this.kw._COLOR_GRADE_DEUTERANOMALY && !this.kw._COLOR_GRADE_DEUTERANOPIA && !this.kw._COLOR_GRADE_TRITANOMALY && !this.kw._COLOR_GRADE_TRITANOPIA && !this.kw._COLOR_GRADE_ACHROMATOMALY)
		{
			bool color_GRADE_ACHROMATOPSIA = this.kw._COLOR_GRADE_ACHROMATOPSIA;
		}
		if (this.kw._ALPHATEST_ON)
		{
			this._Cutoff++;
		}
		else if (this.kw._ALPHA_BLUE_LIVE_ON)
		{
			this._Cutoff++;
		}
		if (this.kw._LIQUID_CONTAINER)
		{
			this._LiquidPlanePosition++;
			this._LiquidPlaneNormal++;
		}
		else
		{
			bool liquid_VOLUME2 = this.kw._LIQUID_VOLUME;
		}
		if (!this.kw._ALPHATEST_ON && !this.kw._ALPHA_BLUE_LIVE_ON && !this.kw._LIQUID_CONTAINER)
		{
			bool liquid_VOLUME3 = this.kw._LIQUID_VOLUME;
		}
		if (this.kw._ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX && (this.kw._EMISSION || this.kw._CRYSTAL_EFFECT))
		{
			this._EmissionIntensityInDynamic++;
		}
		bool zone_DYNAMIC_LIGHTS__CUSTOMVERTEX4 = this.kw._ZONE_DYNAMIC_LIGHTS__CUSTOMVERTEX;
		this.IsValid = true;
		this.fingerprint = default(MaterialFingerprint);
		this.fingerprint = new MaterialFingerprint(this);
	}

	public override string ToString()
	{
		string[] array = new string[178];
		array[0] = "---- MaterialFingerprint of ";
		int num = 1;
		Material material = this.material;
		array[num] = ((material != null) ? material.name : null);
		array[2] = " ----\n";
		array[3] = ((this._TransparencyMode > 0) ? ("_TransparencyMode = " + this.fingerprint._TransparencyMode.ToString() + "\n") : "");
		array[4] = ((this._Cutoff > 0) ? ("_Cutoff = " + this.fingerprint._Cutoff.ToString() + "\n") : "");
		array[5] = ((this._ColorSource > 0) ? ("_ColorSource = " + this.fingerprint._ColorSource.ToString() + "\n") : "");
		int num2 = 6;
		string text;
		if (this._BaseColor <= 0)
		{
			text = "";
		}
		else
		{
			string str = "_BaseColor = ";
			int4 @int = this.fingerprint._BaseColor;
			text = str + @int.ToString() + "\n";
		}
		array[num2] = text;
		int num3 = 7;
		string text2;
		if (this._GChannelColor <= 0)
		{
			text2 = "";
		}
		else
		{
			string str2 = "_GChannelColor = ";
			int4 @int = this.fingerprint._GChannelColor;
			text2 = str2 + @int.ToString() + "\n";
		}
		array[num3] = text2;
		int num4 = 8;
		string text3;
		if (this._BChannelColor <= 0)
		{
			text3 = "";
		}
		else
		{
			string str3 = "_BChannelColor = ";
			int4 @int = this.fingerprint._BChannelColor;
			text3 = str3 + @int.ToString() + "\n";
		}
		array[num4] = text3;
		int num5 = 9;
		string text4;
		if (this._AChannelColor <= 0)
		{
			text4 = "";
		}
		else
		{
			string str4 = "_AChannelColor = ";
			int4 @int = this.fingerprint._AChannelColor;
			text4 = str4 + @int.ToString() + "\n";
		}
		array[num5] = text4;
		array[10] = ((this._BaseMap > 0) ? ("_BaseMap = " + this.fingerprint._BaseMap + "\n") : "");
		int num6 = 11;
		string text5;
		if (this._BaseMap_ST <= 0)
		{
			text5 = "";
		}
		else
		{
			string str5 = "_BaseMap_ST = ";
			int4 @int = this.fingerprint._BaseMap_ST;
			text5 = str5 + @int.ToString() + "\n";
		}
		array[num6] = text5;
		array[12] = ((this._SettingsPreset > 0) ? ("_SettingsPreset = " + this.fingerprint._SettingsPreset.ToString() + "\n") : "");
		array[13] = ((this._AdvancedOptions > 0) ? ("_AdvancedOptions = " + this.fingerprint._AdvancedOptions.ToString() + "\n") : "");
		array[14] = ((this._TexMipBias > 0) ? ("_TexMipBias = " + this.fingerprint._TexMipBias.ToString() + "\n") : "");
		int num7 = 15;
		string text6;
		if (this._BaseMap_WH <= 0)
		{
			text6 = "";
		}
		else
		{
			string str6 = "_BaseMap_WH = ";
			int4 @int = this.fingerprint._BaseMap_WH;
			text6 = str6 + @int.ToString() + "\n";
		}
		array[num7] = text6;
		array[16] = ((this._TexelSnapToggle > 0) ? ("_TexelSnapToggle = " + this.fingerprint._TexelSnapToggle.ToString() + "\n") : "");
		array[17] = ((this._TexelSnap_Factor > 0) ? ("_TexelSnap_Factor = " + this.fingerprint._TexelSnap_Factor.ToString() + "\n") : "");
		array[18] = ((this._UVSource > 0) ? ("_UVSource = " + this.fingerprint._UVSource.ToString() + "\n") : "");
		array[19] = ((this._AlphaDetailToggle > 0) ? ("_AlphaDetailToggle = " + this.fingerprint._AlphaDetailToggle.ToString() + "\n") : "");
		int num8 = 20;
		string text7;
		if (this._AlphaDetail_ST <= 0)
		{
			text7 = "";
		}
		else
		{
			string str7 = "_AlphaDetail_ST = ";
			int4 @int = this.fingerprint._AlphaDetail_ST;
			text7 = str7 + @int.ToString() + "\n";
		}
		array[num8] = text7;
		array[21] = ((this._AlphaDetail_Opacity > 0) ? ("_AlphaDetail_Opacity = " + this.fingerprint._AlphaDetail_Opacity.ToString() + "\n") : "");
		array[22] = ((this._AlphaDetail_WorldSpace > 0) ? ("_AlphaDetail_WorldSpace = " + this.fingerprint._AlphaDetail_WorldSpace.ToString() + "\n") : "");
		array[23] = ((this._MaskMapToggle > 0) ? ("_MaskMapToggle = " + this.fingerprint._MaskMapToggle.ToString() + "\n") : "");
		array[24] = ((this._MaskMap > 0) ? ("_MaskMap = " + this.fingerprint._MaskMap + "\n") : "");
		int num9 = 25;
		string text8;
		if (this._MaskMap_ST <= 0)
		{
			text8 = "";
		}
		else
		{
			string str8 = "_MaskMap_ST = ";
			int4 @int = this.fingerprint._MaskMap_ST;
			text8 = str8 + @int.ToString() + "\n";
		}
		array[num9] = text8;
		int num10 = 26;
		string text9;
		if (this._MaskMap_WH <= 0)
		{
			text9 = "";
		}
		else
		{
			string str9 = "_MaskMap_WH = ";
			int4 @int = this.fingerprint._MaskMap_WH;
			text9 = str9 + @int.ToString() + "\n";
		}
		array[num10] = text9;
		array[27] = ((this._LavaLampToggle > 0) ? ("_LavaLampToggle = " + this.fingerprint._LavaLampToggle.ToString() + "\n") : "");
		array[28] = ((this._GradientMapToggle > 0) ? ("_GradientMapToggle = " + this.fingerprint._GradientMapToggle.ToString() + "\n") : "");
		array[29] = ((this._GradientMap > 0) ? ("_GradientMap = " + this.fingerprint._GradientMap + "\n") : "");
		array[30] = ((this._DoTextureRotation > 0) ? ("_DoTextureRotation = " + this.fingerprint._DoTextureRotation.ToString() + "\n") : "");
		array[31] = ((this._RotateAngle > 0) ? ("_RotateAngle = " + this.fingerprint._RotateAngle.ToString() + "\n") : "");
		array[32] = ((this._RotateAnim > 0) ? ("_RotateAnim = " + this.fingerprint._RotateAnim.ToString() + "\n") : "");
		array[33] = ((this._UseWaveWarp > 0) ? ("_UseWaveWarp = " + this.fingerprint._UseWaveWarp.ToString() + "\n") : "");
		array[34] = ((this._WaveAmplitude > 0) ? ("_WaveAmplitude = " + this.fingerprint._WaveAmplitude.ToString() + "\n") : "");
		array[35] = ((this._WaveFrequency > 0) ? ("_WaveFrequency = " + this.fingerprint._WaveFrequency.ToString() + "\n") : "");
		array[36] = ((this._WaveScale > 0) ? ("_WaveScale = " + this.fingerprint._WaveScale.ToString() + "\n") : "");
		array[37] = ((this._WaveTimeScale > 0) ? ("_WaveTimeScale = " + this.fingerprint._WaveTimeScale.ToString() + "\n") : "");
		array[38] = ((this._UseWeatherMap > 0) ? ("_UseWeatherMap = " + this.fingerprint._UseWeatherMap.ToString() + "\n") : "");
		array[39] = ((this._WeatherMap > 0) ? ("_WeatherMap = " + this.fingerprint._WeatherMap + "\n") : "");
		array[40] = ((this._WeatherMapDissolveEdgeSize > 0) ? ("_WeatherMapDissolveEdgeSize = " + this.fingerprint._WeatherMapDissolveEdgeSize.ToString() + "\n") : "");
		array[41] = ((this._ReflectToggle > 0) ? ("_ReflectToggle = " + this.fingerprint._ReflectToggle.ToString() + "\n") : "");
		array[42] = ((this._ReflectBoxProjectToggle > 0) ? ("_ReflectBoxProjectToggle = " + this.fingerprint._ReflectBoxProjectToggle.ToString() + "\n") : "");
		int num11 = 43;
		string text10;
		if (this._ReflectBoxCubePos <= 0)
		{
			text10 = "";
		}
		else
		{
			string str10 = "_ReflectBoxCubePos = ";
			int4 @int = this.fingerprint._ReflectBoxCubePos;
			text10 = str10 + @int.ToString() + "\n";
		}
		array[num11] = text10;
		int num12 = 44;
		string text11;
		if (this._ReflectBoxSize <= 0)
		{
			text11 = "";
		}
		else
		{
			string str11 = "_ReflectBoxSize = ";
			int4 @int = this.fingerprint._ReflectBoxSize;
			text11 = str11 + @int.ToString() + "\n";
		}
		array[num12] = text11;
		int num13 = 45;
		string text12;
		if (this._ReflectBoxRotation <= 0)
		{
			text12 = "";
		}
		else
		{
			string str12 = "_ReflectBoxRotation = ";
			int4 @int = this.fingerprint._ReflectBoxRotation;
			text12 = str12 + @int.ToString() + "\n";
		}
		array[num13] = text12;
		array[46] = ((this._ReflectMatcapToggle > 0) ? ("_ReflectMatcapToggle = " + this.fingerprint._ReflectMatcapToggle.ToString() + "\n") : "");
		array[47] = ((this._ReflectMatcapPerspToggle > 0) ? ("_ReflectMatcapPerspToggle = " + this.fingerprint._ReflectMatcapPerspToggle.ToString() + "\n") : "");
		array[48] = ((this._ReflectNormalToggle > 0) ? ("_ReflectNormalToggle = " + this.fingerprint._ReflectNormalToggle.ToString() + "\n") : "");
		array[49] = ((this._ReflectTex > 0) ? ("_ReflectTex = " + this.fingerprint._ReflectTex + "\n") : "");
		array[50] = ((this._ReflectNormalTex > 0) ? ("_ReflectNormalTex = " + this.fingerprint._ReflectNormalTex + "\n") : "");
		array[51] = ((this._ReflectAlbedoTint > 0) ? ("_ReflectAlbedoTint = " + this.fingerprint._ReflectAlbedoTint.ToString() + "\n") : "");
		int num14 = 52;
		string text13;
		if (this._ReflectTint <= 0)
		{
			text13 = "";
		}
		else
		{
			string str13 = "_ReflectTint = ";
			int4 @int = this.fingerprint._ReflectTint;
			text13 = str13 + @int.ToString() + "\n";
		}
		array[num14] = text13;
		array[53] = ((this._ReflectOpacity > 0) ? ("_ReflectOpacity = " + this.fingerprint._ReflectOpacity.ToString() + "\n") : "");
		array[54] = ((this._ReflectExposure > 0) ? ("_ReflectExposure = " + this.fingerprint._ReflectExposure.ToString() + "\n") : "");
		int num15 = 55;
		string text14;
		if (this._ReflectOffset <= 0)
		{
			text14 = "";
		}
		else
		{
			string str14 = "_ReflectOffset = ";
			int4 @int = this.fingerprint._ReflectOffset;
			text14 = str14 + @int.ToString() + "\n";
		}
		array[num15] = text14;
		int num16 = 56;
		string text15;
		if (this._ReflectScale <= 0)
		{
			text15 = "";
		}
		else
		{
			string str15 = "_ReflectScale = ";
			int4 @int = this.fingerprint._ReflectScale;
			text15 = str15 + @int.ToString() + "\n";
		}
		array[num16] = text15;
		array[57] = ((this._ReflectRotate > 0) ? ("_ReflectRotate = " + this.fingerprint._ReflectRotate.ToString() + "\n") : "");
		array[58] = ((this._HalfLambertToggle > 0) ? ("_HalfLambertToggle = " + this.fingerprint._HalfLambertToggle.ToString() + "\n") : "");
		array[59] = ((this._ParallaxPlanarToggle > 0) ? ("_ParallaxPlanarToggle = " + this.fingerprint._ParallaxPlanarToggle.ToString() + "\n") : "");
		array[60] = ((this._ParallaxToggle > 0) ? ("_ParallaxToggle = " + this.fingerprint._ParallaxToggle.ToString() + "\n") : "");
		array[61] = ((this._ParallaxAAToggle > 0) ? ("_ParallaxAAToggle = " + this.fingerprint._ParallaxAAToggle.ToString() + "\n") : "");
		array[62] = ((this._ParallaxAABias > 0) ? ("_ParallaxAABias = " + this.fingerprint._ParallaxAABias.ToString() + "\n") : "");
		array[63] = ((this._DepthMap > 0) ? ("_DepthMap = " + this.fingerprint._DepthMap + "\n") : "");
		array[64] = ((this._ParallaxAmplitude > 0) ? ("_ParallaxAmplitude = " + this.fingerprint._ParallaxAmplitude.ToString() + "\n") : "");
		int num17 = 65;
		string text16;
		if (this._ParallaxSamplesMinMax <= 0)
		{
			text16 = "";
		}
		else
		{
			string str16 = "_ParallaxSamplesMinMax = ";
			int4 @int = this.fingerprint._ParallaxSamplesMinMax;
			text16 = str16 + @int.ToString() + "\n";
		}
		array[num17] = text16;
		array[66] = ((this._UvShiftToggle > 0) ? ("_UvShiftToggle = " + this.fingerprint._UvShiftToggle.ToString() + "\n") : "");
		int num18 = 67;
		string text17;
		if (this._UvShiftSteps <= 0)
		{
			text17 = "";
		}
		else
		{
			string str17 = "_UvShiftSteps = ";
			int4 @int = this.fingerprint._UvShiftSteps;
			text17 = str17 + @int.ToString() + "\n";
		}
		array[num18] = text17;
		int num19 = 68;
		string text18;
		if (this._UvShiftRate <= 0)
		{
			text18 = "";
		}
		else
		{
			string str18 = "_UvShiftRate = ";
			int4 @int = this.fingerprint._UvShiftRate;
			text18 = str18 + @int.ToString() + "\n";
		}
		array[num19] = text18;
		int num20 = 69;
		string text19;
		if (this._UvShiftOffset <= 0)
		{
			text19 = "";
		}
		else
		{
			string str19 = "_UvShiftOffset = ";
			int4 @int = this.fingerprint._UvShiftOffset;
			text19 = str19 + @int.ToString() + "\n";
		}
		array[num20] = text19;
		array[70] = ((this._UseGridEffect > 0) ? ("_UseGridEffect = " + this.fingerprint._UseGridEffect.ToString() + "\n") : "");
		array[71] = ((this._UseCrystalEffect > 0) ? ("_UseCrystalEffect = " + this.fingerprint._UseCrystalEffect.ToString() + "\n") : "");
		array[72] = ((this._CrystalPower > 0) ? ("_CrystalPower = " + this.fingerprint._CrystalPower.ToString() + "\n") : "");
		int num21 = 73;
		string text20;
		if (this._CrystalRimColor <= 0)
		{
			text20 = "";
		}
		else
		{
			string str20 = "_CrystalRimColor = ";
			int4 @int = this.fingerprint._CrystalRimColor;
			text20 = str20 + @int.ToString() + "\n";
		}
		array[num21] = text20;
		array[74] = ((this._LiquidVolume > 0) ? ("_LiquidVolume = " + this.fingerprint._LiquidVolume.ToString() + "\n") : "");
		array[75] = ((this._LiquidFill > 0) ? ("_LiquidFill = " + this.fingerprint._LiquidFill.ToString() + "\n") : "");
		int num22 = 76;
		string text21;
		if (this._LiquidFillNormal <= 0)
		{
			text21 = "";
		}
		else
		{
			string str21 = "_LiquidFillNormal = ";
			int4 @int = this.fingerprint._LiquidFillNormal;
			text21 = str21 + @int.ToString() + "\n";
		}
		array[num22] = text21;
		int num23 = 77;
		string text22;
		if (this._LiquidSurfaceColor <= 0)
		{
			text22 = "";
		}
		else
		{
			string str22 = "_LiquidSurfaceColor = ";
			int4 @int = this.fingerprint._LiquidSurfaceColor;
			text22 = str22 + @int.ToString() + "\n";
		}
		array[num23] = text22;
		array[78] = ((this._LiquidSwayX > 0) ? ("_LiquidSwayX = " + this.fingerprint._LiquidSwayX.ToString() + "\n") : "");
		array[79] = ((this._LiquidSwayY > 0) ? ("_LiquidSwayY = " + this.fingerprint._LiquidSwayY.ToString() + "\n") : "");
		array[80] = ((this._LiquidContainer > 0) ? ("_LiquidContainer = " + this.fingerprint._LiquidContainer.ToString() + "\n") : "");
		int num24 = 81;
		string text23;
		if (this._LiquidPlanePosition <= 0)
		{
			text23 = "";
		}
		else
		{
			string str23 = "_LiquidPlanePosition = ";
			int4 @int = this.fingerprint._LiquidPlanePosition;
			text23 = str23 + @int.ToString() + "\n";
		}
		array[num24] = text23;
		int num25 = 82;
		string text24;
		if (this._LiquidPlaneNormal <= 0)
		{
			text24 = "";
		}
		else
		{
			string str24 = "_LiquidPlaneNormal = ";
			int4 @int = this.fingerprint._LiquidPlaneNormal;
			text24 = str24 + @int.ToString() + "\n";
		}
		array[num25] = text24;
		array[83] = ((this._VertexFlapToggle > 0) ? ("_VertexFlapToggle = " + this.fingerprint._VertexFlapToggle.ToString() + "\n") : "");
		int num26 = 84;
		string text25;
		if (this._VertexFlapAxis <= 0)
		{
			text25 = "";
		}
		else
		{
			string str25 = "_VertexFlapAxis = ";
			int4 @int = this.fingerprint._VertexFlapAxis;
			text25 = str25 + @int.ToString() + "\n";
		}
		array[num26] = text25;
		int num27 = 85;
		string text26;
		if (this._VertexFlapDegreesMinMax <= 0)
		{
			text26 = "";
		}
		else
		{
			string str26 = "_VertexFlapDegreesMinMax = ";
			int4 @int = this.fingerprint._VertexFlapDegreesMinMax;
			text26 = str26 + @int.ToString() + "\n";
		}
		array[num27] = text26;
		array[86] = ((this._VertexFlapSpeed > 0) ? ("_VertexFlapSpeed = " + this.fingerprint._VertexFlapSpeed.ToString() + "\n") : "");
		array[87] = ((this._VertexFlapPhaseOffset > 0) ? ("_VertexFlapPhaseOffset = " + this.fingerprint._VertexFlapPhaseOffset.ToString() + "\n") : "");
		array[88] = ((this._VertexWaveToggle > 0) ? ("_VertexWaveToggle = " + this.fingerprint._VertexWaveToggle.ToString() + "\n") : "");
		array[89] = ((this._VertexWaveDebug > 0) ? ("_VertexWaveDebug = " + this.fingerprint._VertexWaveDebug.ToString() + "\n") : "");
		int num28 = 90;
		string text27;
		if (this._VertexWaveEnd <= 0)
		{
			text27 = "";
		}
		else
		{
			string str27 = "_VertexWaveEnd = ";
			int4 @int = this.fingerprint._VertexWaveEnd;
			text27 = str27 + @int.ToString() + "\n";
		}
		array[num28] = text27;
		int num29 = 91;
		string text28;
		if (this._VertexWaveParams <= 0)
		{
			text28 = "";
		}
		else
		{
			string str28 = "_VertexWaveParams = ";
			int4 @int = this.fingerprint._VertexWaveParams;
			text28 = str28 + @int.ToString() + "\n";
		}
		array[num29] = text28;
		int num30 = 92;
		string text29;
		if (this._VertexWaveFalloff <= 0)
		{
			text29 = "";
		}
		else
		{
			string str29 = "_VertexWaveFalloff = ";
			int4 @int = this.fingerprint._VertexWaveFalloff;
			text29 = str29 + @int.ToString() + "\n";
		}
		array[num30] = text29;
		int num31 = 93;
		string text30;
		if (this._VertexWaveSphereMask <= 0)
		{
			text30 = "";
		}
		else
		{
			string str30 = "_VertexWaveSphereMask = ";
			int4 @int = this.fingerprint._VertexWaveSphereMask;
			text30 = str30 + @int.ToString() + "\n";
		}
		array[num31] = text30;
		array[94] = ((this._VertexWavePhaseOffset > 0) ? ("_VertexWavePhaseOffset = " + this.fingerprint._VertexWavePhaseOffset.ToString() + "\n") : "");
		int num32 = 95;
		string text31;
		if (this._VertexWaveAxes <= 0)
		{
			text31 = "";
		}
		else
		{
			string str31 = "_VertexWaveAxes = ";
			int4 @int = this.fingerprint._VertexWaveAxes;
			text31 = str31 + @int.ToString() + "\n";
		}
		array[num32] = text31;
		array[96] = ((this._VertexRotateToggle > 0) ? ("_VertexRotateToggle = " + this.fingerprint._VertexRotateToggle.ToString() + "\n") : "");
		int num33 = 97;
		string text32;
		if (this._VertexRotateAngles <= 0)
		{
			text32 = "";
		}
		else
		{
			string str32 = "_VertexRotateAngles = ";
			int4 @int = this.fingerprint._VertexRotateAngles;
			text32 = str32 + @int.ToString() + "\n";
		}
		array[num33] = text32;
		array[98] = ((this._VertexRotateAnim > 0) ? ("_VertexRotateAnim = " + this.fingerprint._VertexRotateAnim.ToString() + "\n") : "");
		array[99] = ((this._VertexLightToggle > 0) ? ("_VertexLightToggle = " + this.fingerprint._VertexLightToggle.ToString() + "\n") : "");
		array[100] = ((this._InnerGlowOn > 0) ? ("_InnerGlowOn = " + this.fingerprint._InnerGlowOn.ToString() + "\n") : "");
		int num34 = 101;
		string text33;
		if (this._InnerGlowColor <= 0)
		{
			text33 = "";
		}
		else
		{
			string str33 = "_InnerGlowColor = ";
			int4 @int = this.fingerprint._InnerGlowColor;
			text33 = str33 + @int.ToString() + "\n";
		}
		array[num34] = text33;
		int num35 = 102;
		string text34;
		if (this._InnerGlowParams <= 0)
		{
			text34 = "";
		}
		else
		{
			string str34 = "_InnerGlowParams = ";
			int4 @int = this.fingerprint._InnerGlowParams;
			text34 = str34 + @int.ToString() + "\n";
		}
		array[num35] = text34;
		array[103] = ((this._InnerGlowTap > 0) ? ("_InnerGlowTap = " + this.fingerprint._InnerGlowTap.ToString() + "\n") : "");
		array[104] = ((this._InnerGlowSine > 0) ? ("_InnerGlowSine = " + this.fingerprint._InnerGlowSine.ToString() + "\n") : "");
		array[105] = ((this._InnerGlowSinePeriod > 0) ? ("_InnerGlowSinePeriod = " + this.fingerprint._InnerGlowSinePeriod.ToString() + "\n") : "");
		array[106] = ((this._InnerGlowSinePhaseShift > 0) ? ("_InnerGlowSinePhaseShift = " + this.fingerprint._InnerGlowSinePhaseShift.ToString() + "\n") : "");
		array[107] = ((this._StealthEffectOn > 0) ? ("_StealthEffectOn = " + this.fingerprint._StealthEffectOn.ToString() + "\n") : "");
		array[108] = ((this._UseEyeTracking > 0) ? ("_UseEyeTracking = " + this.fingerprint._UseEyeTracking.ToString() + "\n") : "");
		int num36 = 109;
		string text35;
		if (this._EyeTileOffsetUV <= 0)
		{
			text35 = "";
		}
		else
		{
			string str35 = "_EyeTileOffsetUV = ";
			int4 @int = this.fingerprint._EyeTileOffsetUV;
			text35 = str35 + @int.ToString() + "\n";
		}
		array[num36] = text35;
		array[110] = ((this._EyeOverrideUV > 0) ? ("_EyeOverrideUV = " + this.fingerprint._EyeOverrideUV.ToString() + "\n") : "");
		int num37 = 111;
		string text36;
		if (this._EyeOverrideUVTransform <= 0)
		{
			text36 = "";
		}
		else
		{
			string str36 = "_EyeOverrideUVTransform = ";
			int4 @int = this.fingerprint._EyeOverrideUVTransform;
			text36 = str36 + @int.ToString() + "\n";
		}
		array[num37] = text36;
		array[112] = ((this._UseMouthFlap > 0) ? ("_UseMouthFlap = " + this.fingerprint._UseMouthFlap.ToString() + "\n") : "");
		array[113] = ((this._MouthMap > 0) ? ("_MouthMap = " + this.fingerprint._MouthMap + "\n") : "");
		int num38 = 114;
		string text37;
		if (this._MouthMap_ST <= 0)
		{
			text37 = "";
		}
		else
		{
			string str37 = "_MouthMap_ST = ";
			int4 @int = this.fingerprint._MouthMap_ST;
			text37 = str37 + @int.ToString() + "\n";
		}
		array[num38] = text37;
		array[115] = ((this._UseVertexColor > 0) ? ("_UseVertexColor = " + this.fingerprint._UseVertexColor.ToString() + "\n") : "");
		array[116] = ((this._WaterEffect > 0) ? ("_WaterEffect = " + this.fingerprint._WaterEffect.ToString() + "\n") : "");
		array[117] = ((this._HeightBasedWaterEffect > 0) ? ("_HeightBasedWaterEffect = " + this.fingerprint._HeightBasedWaterEffect.ToString() + "\n") : "");
		array[118] = ((this._WaterCaustics > 0) ? ("_WaterCaustics = " + this.fingerprint._WaterCaustics.ToString() + "\n") : "");
		array[119] = ((this._UseDayNightLightmap > 0) ? ("_UseDayNightLightmap = " + this.fingerprint._UseDayNightLightmap.ToString() + "\n") : "");
		array[120] = ((this._UseSpecular > 0) ? ("_UseSpecular = " + this.fingerprint._UseSpecular.ToString() + "\n") : "");
		array[121] = ((this._UseSpecularAlphaChannel > 0) ? ("_UseSpecularAlphaChannel = " + this.fingerprint._UseSpecularAlphaChannel.ToString() + "\n") : "");
		array[122] = ((this._Smoothness > 0) ? ("_Smoothness = " + this.fingerprint._Smoothness.ToString() + "\n") : "");
		array[123] = ((this._UseSpecHighlight > 0) ? ("_UseSpecHighlight = " + this.fingerprint._UseSpecHighlight.ToString() + "\n") : "");
		int num39 = 124;
		string text38;
		if (this._SpecularDir <= 0)
		{
			text38 = "";
		}
		else
		{
			string str38 = "_SpecularDir = ";
			int4 @int = this.fingerprint._SpecularDir;
			text38 = str38 + @int.ToString() + "\n";
		}
		array[num39] = text38;
		int num40 = 125;
		string text39;
		if (this._SpecularPowerIntensity <= 0)
		{
			text39 = "";
		}
		else
		{
			string str39 = "_SpecularPowerIntensity = ";
			int4 @int = this.fingerprint._SpecularPowerIntensity;
			text39 = str39 + @int.ToString() + "\n";
		}
		array[num40] = text39;
		int num41 = 126;
		string text40;
		if (this._SpecularColor <= 0)
		{
			text40 = "";
		}
		else
		{
			string str40 = "_SpecularColor = ";
			int4 @int = this.fingerprint._SpecularColor;
			text40 = str40 + @int.ToString() + "\n";
		}
		array[num41] = text40;
		array[127] = ((this._SpecularUseDiffuseColor > 0) ? ("_SpecularUseDiffuseColor = " + this.fingerprint._SpecularUseDiffuseColor.ToString() + "\n") : "");
		array[128] = ((this._EmissionToggle > 0) ? ("_EmissionToggle = " + this.fingerprint._EmissionToggle.ToString() + "\n") : "");
		int num42 = 129;
		string text41;
		if (this._EmissionColor <= 0)
		{
			text41 = "";
		}
		else
		{
			string str41 = "_EmissionColor = ";
			int4 @int = this.fingerprint._EmissionColor;
			text41 = str41 + @int.ToString() + "\n";
		}
		array[num42] = text41;
		array[130] = ((this._EmissionMap > 0) ? ("_EmissionMap = " + this.fingerprint._EmissionMap + "\n") : "");
		array[131] = ((this._EmissionMaskByBaseMapAlpha > 0) ? ("_EmissionMaskByBaseMapAlpha = " + this.fingerprint._EmissionMaskByBaseMapAlpha.ToString() + "\n") : "");
		int num43 = 132;
		string text42;
		if (this._EmissionUVScrollSpeed <= 0)
		{
			text42 = "";
		}
		else
		{
			string str42 = "_EmissionUVScrollSpeed = ";
			int4 @int = this.fingerprint._EmissionUVScrollSpeed;
			text42 = str42 + @int.ToString() + "\n";
		}
		array[num43] = text42;
		array[133] = ((this._EmissionDissolveProgress > 0) ? ("_EmissionDissolveProgress = " + this.fingerprint._EmissionDissolveProgress.ToString() + "\n") : "");
		int num44 = 134;
		string text43;
		if (this._EmissionDissolveAnimation <= 0)
		{
			text43 = "";
		}
		else
		{
			string str43 = "_EmissionDissolveAnimation = ";
			int4 @int = this.fingerprint._EmissionDissolveAnimation;
			text43 = str43 + @int.ToString() + "\n";
		}
		array[num44] = text43;
		array[135] = ((this._EmissionDissolveEdgeSize > 0) ? ("_EmissionDissolveEdgeSize = " + this.fingerprint._EmissionDissolveEdgeSize.ToString() + "\n") : "");
		array[136] = ((this._EmissionIntensityInDynamic > 0) ? ("_EmissionIntensityInDynamic = " + this.fingerprint._EmissionIntensityInDynamic.ToString() + "\n") : "");
		array[137] = ((this._EmissionUseUVWaveWarp > 0) ? ("_EmissionUseUVWaveWarp = " + this.fingerprint._EmissionUseUVWaveWarp.ToString() + "\n") : "");
		array[138] = ((this._GreyZoneException > 0) ? ("_GreyZoneException = " + this.fingerprint._GreyZoneException.ToString() + "\n") : "");
		array[139] = ((this._Cull > 0) ? ("_Cull = " + this.fingerprint._Cull.ToString() + "\n") : "");
		array[140] = ((this._StencilReference > 0) ? ("_StencilReference = " + this.fingerprint._StencilReference.ToString() + "\n") : "");
		array[141] = ((this._StencilComparison > 0) ? ("_StencilComparison = " + this.fingerprint._StencilComparison.ToString() + "\n") : "");
		array[142] = ((this._StencilPassFront > 0) ? ("_StencilPassFront = " + this.fingerprint._StencilPassFront.ToString() + "\n") : "");
		array[143] = ((this._USE_DEFORM_MAP > 0) ? ("_USE_DEFORM_MAP = " + this.fingerprint._USE_DEFORM_MAP.ToString() + "\n") : "");
		array[144] = ((this._DeformMap > 0) ? ("_DeformMap = " + this.fingerprint._DeformMap + "\n") : "");
		array[145] = ((this._DeformMapIntensity > 0) ? ("_DeformMapIntensity = " + this.fingerprint._DeformMapIntensity.ToString() + "\n") : "");
		array[146] = ((this._DeformMapMaskByVertColorRAmount > 0) ? ("_DeformMapMaskByVertColorRAmount = " + this.fingerprint._DeformMapMaskByVertColorRAmount.ToString() + "\n") : "");
		int num45 = 147;
		string text44;
		if (this._DeformMapScrollSpeed <= 0)
		{
			text44 = "";
		}
		else
		{
			string str44 = "_DeformMapScrollSpeed = ";
			int4 @int = this.fingerprint._DeformMapScrollSpeed;
			text44 = str44 + @int.ToString() + "\n";
		}
		array[num45] = text44;
		int num46 = 148;
		string text45;
		if (this._DeformMapUV0Influence <= 0)
		{
			text45 = "";
		}
		else
		{
			string str45 = "_DeformMapUV0Influence = ";
			int4 @int = this.fingerprint._DeformMapUV0Influence;
			text45 = str45 + @int.ToString() + "\n";
		}
		array[num46] = text45;
		int num47 = 149;
		string text46;
		if (this._DeformMapObjectSpaceOffsetsU <= 0)
		{
			text46 = "";
		}
		else
		{
			string str46 = "_DeformMapObjectSpaceOffsetsU = ";
			int4 @int = this.fingerprint._DeformMapObjectSpaceOffsetsU;
			text46 = str46 + @int.ToString() + "\n";
		}
		array[num47] = text46;
		int num48 = 150;
		string text47;
		if (this._DeformMapObjectSpaceOffsetsV <= 0)
		{
			text47 = "";
		}
		else
		{
			string str47 = "_DeformMapObjectSpaceOffsetsV = ";
			int4 @int = this.fingerprint._DeformMapObjectSpaceOffsetsV;
			text47 = str47 + @int.ToString() + "\n";
		}
		array[num48] = text47;
		int num49 = 151;
		string text48;
		if (this._DeformMapWorldSpaceOffsetsU <= 0)
		{
			text48 = "";
		}
		else
		{
			string str48 = "_DeformMapWorldSpaceOffsetsU = ";
			int4 @int = this.fingerprint._DeformMapWorldSpaceOffsetsU;
			text48 = str48 + @int.ToString() + "\n";
		}
		array[num49] = text48;
		int num50 = 152;
		string text49;
		if (this._DeformMapWorldSpaceOffsetsV <= 0)
		{
			text49 = "";
		}
		else
		{
			string str49 = "_DeformMapWorldSpaceOffsetsV = ";
			int4 @int = this.fingerprint._DeformMapWorldSpaceOffsetsV;
			text49 = str49 + @int.ToString() + "\n";
		}
		array[num50] = text49;
		int num51 = 153;
		string text50;
		if (this._RotateOnYAxisBySinTime <= 0)
		{
			text50 = "";
		}
		else
		{
			string str50 = "_RotateOnYAxisBySinTime = ";
			int4 @int = this.fingerprint._RotateOnYAxisBySinTime;
			text50 = str50 + @int.ToString() + "\n";
		}
		array[num51] = text50;
		array[154] = ((this._USE_TEX_ARRAY_ATLAS > 0) ? ("_USE_TEX_ARRAY_ATLAS = " + this.fingerprint._USE_TEX_ARRAY_ATLAS.ToString() + "\n") : "");
		array[155] = ((this._BaseMap_Atlas > 0) ? ("_BaseMap_Atlas = " + this.fingerprint._BaseMap_Atlas + "\n") : "");
		array[156] = ((this._BaseMap_AtlasSlice > 0) ? ("_BaseMap_AtlasSlice = " + this.fingerprint._BaseMap_AtlasSlice.ToString() + "\n") : "");
		array[157] = ((this._BaseMap_AtlasSliceSource > 0) ? ("_BaseMap_AtlasSliceSource = " + this.fingerprint._BaseMap_AtlasSliceSource.ToString() + "\n") : "");
		array[158] = ((this._EmissionMap_Atlas > 0) ? ("_EmissionMap_Atlas = " + this.fingerprint._EmissionMap_Atlas + "\n") : "");
		array[159] = ((this._EmissionMap_AtlasSlice > 0) ? ("_EmissionMap_AtlasSlice = " + this.fingerprint._EmissionMap_AtlasSlice.ToString() + "\n") : "");
		array[160] = ((this._DeformMap_Atlas > 0) ? ("_DeformMap_Atlas = " + this.fingerprint._DeformMap_Atlas + "\n") : "");
		array[161] = ((this._DeformMap_AtlasSlice > 0) ? ("_DeformMap_AtlasSlice = " + this.fingerprint._DeformMap_AtlasSlice.ToString() + "\n") : "");
		array[162] = ((this._WeatherMap_Atlas > 0) ? ("_WeatherMap_Atlas = " + this.fingerprint._WeatherMap_Atlas + "\n") : "");
		array[163] = ((this._WeatherMap_AtlasSlice > 0) ? ("_WeatherMap_AtlasSlice = " + this.fingerprint._WeatherMap_AtlasSlice.ToString() + "\n") : "");
		array[164] = ((this._DEBUG_PAWN_DATA > 0) ? ("_DEBUG_PAWN_DATA = " + this.fingerprint._DEBUG_PAWN_DATA.ToString() + "\n") : "");
		array[165] = ((this._SrcBlend > 0) ? ("_SrcBlend = " + this.fingerprint._SrcBlend.ToString() + "\n") : "");
		array[166] = ((this._DstBlend > 0) ? ("_DstBlend = " + this.fingerprint._DstBlend.ToString() + "\n") : "");
		array[167] = ((this._SrcBlendAlpha > 0) ? ("_SrcBlendAlpha = " + this.fingerprint._SrcBlendAlpha.ToString() + "\n") : "");
		array[168] = ((this._DstBlendAlpha > 0) ? ("_DstBlendAlpha = " + this.fingerprint._DstBlendAlpha.ToString() + "\n") : "");
		array[169] = ((this._ZWrite > 0) ? ("_ZWrite = " + this.fingerprint._ZWrite.ToString() + "\n") : "");
		array[170] = ((this._AlphaToMask > 0) ? ("_AlphaToMask = " + this.fingerprint._AlphaToMask.ToString() + "\n") : "");
		int num52 = 171;
		string text51;
		if (this._Color <= 0)
		{
			text51 = "";
		}
		else
		{
			string str51 = "_Color = ";
			int4 @int = this.fingerprint._Color;
			text51 = str51 + @int.ToString() + "\n";
		}
		array[num52] = text51;
		array[172] = ((this._Surface > 0) ? ("_Surface = " + this.fingerprint._Surface.ToString() + "\n") : "");
		array[173] = ((this._Metallic > 0) ? ("_Metallic = " + this.fingerprint._Metallic.ToString() + "\n") : "");
		int num53 = 174;
		string text52;
		if (this._SpecColor <= 0)
		{
			text52 = "";
		}
		else
		{
			string str52 = "_SpecColor = ";
			int4 @int = this.fingerprint._SpecColor;
			text52 = str52 + @int.ToString() + "\n";
		}
		array[num53] = text52;
		array[175] = ((this._DayNightLightmapArray > 0) ? ("_DayNightLightmapArray = " + this.fingerprint._DayNightLightmapArray + "\n") : "");
		int num54 = 176;
		string text53;
		if (this._DayNightLightmapArray_ST <= 0)
		{
			text53 = "";
		}
		else
		{
			string str53 = "_DayNightLightmapArray_ST = ";
			int4 @int = this.fingerprint._DayNightLightmapArray_ST;
			text53 = str53 + @int.ToString() + "\n";
		}
		array[num54] = text53;
		array[177] = ((this._DayNightLightmapArray_AtlasSlice > 0) ? ("_DayNightLightmapArray_AtlasSlice = " + this.fingerprint._DayNightLightmapArray_AtlasSlice.ToString() + "\n") : "");
		return string.Concat(array);
	}

	public string ToStringTSV()
	{
		string[] array = new string[703];
		array[0] = "---- MaterialFingerprint of ";
		int num = 1;
		Material material = this.material;
		array[num] = ((material != null) ? material.name : null);
		array[2] = " ----\nName,\tUsed?,\tRounded Value_TransparencyMode,\t";
		array[3] = (this._TransparencyMode > 0).ToString();
		array[4] = ",\t";
		array[5] = this.fingerprint._TransparencyMode.ToString();
		array[6] = "\n_Cutoff,\t";
		array[7] = (this._Cutoff > 0).ToString();
		array[8] = ",\t";
		array[9] = this.fingerprint._Cutoff.ToString();
		array[10] = "\n_ColorSource,\t";
		array[11] = (this._ColorSource > 0).ToString();
		array[12] = ",\t";
		array[13] = this.fingerprint._ColorSource.ToString();
		array[14] = "\n_BaseColor,\t";
		array[15] = (this._BaseColor > 0).ToString();
		array[16] = ",\t";
		int num2 = 17;
		int4 @int = this.fingerprint._BaseColor;
		array[num2] = @int.ToString();
		array[18] = "\n_GChannelColor,\t";
		array[19] = (this._GChannelColor > 0).ToString();
		array[20] = ",\t";
		int num3 = 21;
		@int = this.fingerprint._GChannelColor;
		array[num3] = @int.ToString();
		array[22] = "\n_BChannelColor,\t";
		array[23] = (this._BChannelColor > 0).ToString();
		array[24] = ",\t";
		int num4 = 25;
		@int = this.fingerprint._BChannelColor;
		array[num4] = @int.ToString();
		array[26] = "\n_AChannelColor,\t";
		array[27] = (this._AChannelColor > 0).ToString();
		array[28] = ",\t";
		int num5 = 29;
		@int = this.fingerprint._AChannelColor;
		array[num5] = @int.ToString();
		array[30] = "\n_BaseMap,\t";
		array[31] = (this._BaseMap > 0).ToString();
		array[32] = ",\t";
		array[33] = this.fingerprint._BaseMap;
		array[34] = "\n_BaseMap_ST,\t";
		array[35] = (this._BaseMap_ST > 0).ToString();
		array[36] = ",\t";
		int num6 = 37;
		@int = this.fingerprint._BaseMap_ST;
		array[num6] = @int.ToString();
		array[38] = "\n_SettingsPreset,\t";
		array[39] = (this._SettingsPreset > 0).ToString();
		array[40] = ",\t";
		array[41] = this.fingerprint._SettingsPreset.ToString();
		array[42] = "\n_AdvancedOptions,\t";
		array[43] = (this._AdvancedOptions > 0).ToString();
		array[44] = ",\t";
		array[45] = this.fingerprint._AdvancedOptions.ToString();
		array[46] = "\n_TexMipBias,\t";
		array[47] = (this._TexMipBias > 0).ToString();
		array[48] = ",\t";
		array[49] = this.fingerprint._TexMipBias.ToString();
		array[50] = "\n_BaseMap_WH,\t";
		array[51] = (this._BaseMap_WH > 0).ToString();
		array[52] = ",\t";
		int num7 = 53;
		@int = this.fingerprint._BaseMap_WH;
		array[num7] = @int.ToString();
		array[54] = "\n_TexelSnapToggle,\t";
		array[55] = (this._TexelSnapToggle > 0).ToString();
		array[56] = ",\t";
		array[57] = this.fingerprint._TexelSnapToggle.ToString();
		array[58] = "\n_TexelSnap_Factor,\t";
		array[59] = (this._TexelSnap_Factor > 0).ToString();
		array[60] = ",\t";
		array[61] = this.fingerprint._TexelSnap_Factor.ToString();
		array[62] = "\n_UVSource,\t";
		array[63] = (this._UVSource > 0).ToString();
		array[64] = ",\t";
		array[65] = this.fingerprint._UVSource.ToString();
		array[66] = "\n_AlphaDetailToggle,\t";
		array[67] = (this._AlphaDetailToggle > 0).ToString();
		array[68] = ",\t";
		array[69] = this.fingerprint._AlphaDetailToggle.ToString();
		array[70] = "\n_AlphaDetail_ST,\t";
		array[71] = (this._AlphaDetail_ST > 0).ToString();
		array[72] = ",\t";
		int num8 = 73;
		@int = this.fingerprint._AlphaDetail_ST;
		array[num8] = @int.ToString();
		array[74] = "\n_AlphaDetail_Opacity,\t";
		array[75] = (this._AlphaDetail_Opacity > 0).ToString();
		array[76] = ",\t";
		array[77] = this.fingerprint._AlphaDetail_Opacity.ToString();
		array[78] = "\n_AlphaDetail_WorldSpace,\t";
		array[79] = (this._AlphaDetail_WorldSpace > 0).ToString();
		array[80] = ",\t";
		array[81] = this.fingerprint._AlphaDetail_WorldSpace.ToString();
		array[82] = "\n_MaskMapToggle,\t";
		array[83] = (this._MaskMapToggle > 0).ToString();
		array[84] = ",\t";
		array[85] = this.fingerprint._MaskMapToggle.ToString();
		array[86] = "\n_MaskMap,\t";
		array[87] = (this._MaskMap > 0).ToString();
		array[88] = ",\t";
		array[89] = this.fingerprint._MaskMap;
		array[90] = "\n_MaskMap_ST,\t";
		array[91] = (this._MaskMap_ST > 0).ToString();
		array[92] = ",\t";
		int num9 = 93;
		@int = this.fingerprint._MaskMap_ST;
		array[num9] = @int.ToString();
		array[94] = "\n_MaskMap_WH,\t";
		array[95] = (this._MaskMap_WH > 0).ToString();
		array[96] = ",\t";
		int num10 = 97;
		@int = this.fingerprint._MaskMap_WH;
		array[num10] = @int.ToString();
		array[98] = "\n_LavaLampToggle,\t";
		array[99] = (this._LavaLampToggle > 0).ToString();
		array[100] = ",\t";
		array[101] = this.fingerprint._LavaLampToggle.ToString();
		array[102] = "\n_GradientMapToggle,\t";
		array[103] = (this._GradientMapToggle > 0).ToString();
		array[104] = ",\t";
		array[105] = this.fingerprint._GradientMapToggle.ToString();
		array[106] = "\n_GradientMap,\t";
		array[107] = (this._GradientMap > 0).ToString();
		array[108] = ",\t";
		array[109] = this.fingerprint._GradientMap;
		array[110] = "\n_DoTextureRotation,\t";
		array[111] = (this._DoTextureRotation > 0).ToString();
		array[112] = ",\t";
		array[113] = this.fingerprint._DoTextureRotation.ToString();
		array[114] = "\n_RotateAngle,\t";
		array[115] = (this._RotateAngle > 0).ToString();
		array[116] = ",\t";
		array[117] = this.fingerprint._RotateAngle.ToString();
		array[118] = "\n_RotateAnim,\t";
		array[119] = (this._RotateAnim > 0).ToString();
		array[120] = ",\t";
		array[121] = this.fingerprint._RotateAnim.ToString();
		array[122] = "\n_UseWaveWarp,\t";
		array[123] = (this._UseWaveWarp > 0).ToString();
		array[124] = ",\t";
		array[125] = this.fingerprint._UseWaveWarp.ToString();
		array[126] = "\n_WaveAmplitude,\t";
		array[127] = (this._WaveAmplitude > 0).ToString();
		array[128] = ",\t";
		array[129] = this.fingerprint._WaveAmplitude.ToString();
		array[130] = "\n_WaveFrequency,\t";
		array[131] = (this._WaveFrequency > 0).ToString();
		array[132] = ",\t";
		array[133] = this.fingerprint._WaveFrequency.ToString();
		array[134] = "\n_WaveScale,\t";
		array[135] = (this._WaveScale > 0).ToString();
		array[136] = ",\t";
		array[137] = this.fingerprint._WaveScale.ToString();
		array[138] = "\n_WaveTimeScale,\t";
		array[139] = (this._WaveTimeScale > 0).ToString();
		array[140] = ",\t";
		array[141] = this.fingerprint._WaveTimeScale.ToString();
		array[142] = "\n_UseWeatherMap,\t";
		array[143] = (this._UseWeatherMap > 0).ToString();
		array[144] = ",\t";
		array[145] = this.fingerprint._UseWeatherMap.ToString();
		array[146] = "\n_WeatherMap,\t";
		array[147] = (this._WeatherMap > 0).ToString();
		array[148] = ",\t";
		array[149] = this.fingerprint._WeatherMap;
		array[150] = "\n_WeatherMapDissolveEdgeSize,\t";
		array[151] = (this._WeatherMapDissolveEdgeSize > 0).ToString();
		array[152] = ",\t";
		array[153] = this.fingerprint._WeatherMapDissolveEdgeSize.ToString();
		array[154] = "\n_ReflectToggle,\t";
		array[155] = (this._ReflectToggle > 0).ToString();
		array[156] = ",\t";
		array[157] = this.fingerprint._ReflectToggle.ToString();
		array[158] = "\n_ReflectBoxProjectToggle,\t";
		array[159] = (this._ReflectBoxProjectToggle > 0).ToString();
		array[160] = ",\t";
		array[161] = this.fingerprint._ReflectBoxProjectToggle.ToString();
		array[162] = "\n_ReflectBoxCubePos,\t";
		array[163] = (this._ReflectBoxCubePos > 0).ToString();
		array[164] = ",\t";
		int num11 = 165;
		@int = this.fingerprint._ReflectBoxCubePos;
		array[num11] = @int.ToString();
		array[166] = "\n_ReflectBoxSize,\t";
		array[167] = (this._ReflectBoxSize > 0).ToString();
		array[168] = ",\t";
		int num12 = 169;
		@int = this.fingerprint._ReflectBoxSize;
		array[num12] = @int.ToString();
		array[170] = "\n_ReflectBoxRotation,\t";
		array[171] = (this._ReflectBoxRotation > 0).ToString();
		array[172] = ",\t";
		int num13 = 173;
		@int = this.fingerprint._ReflectBoxRotation;
		array[num13] = @int.ToString();
		array[174] = "\n_ReflectMatcapToggle,\t";
		array[175] = (this._ReflectMatcapToggle > 0).ToString();
		array[176] = ",\t";
		array[177] = this.fingerprint._ReflectMatcapToggle.ToString();
		array[178] = "\n_ReflectMatcapPerspToggle,\t";
		array[179] = (this._ReflectMatcapPerspToggle > 0).ToString();
		array[180] = ",\t";
		array[181] = this.fingerprint._ReflectMatcapPerspToggle.ToString();
		array[182] = "\n_ReflectNormalToggle,\t";
		array[183] = (this._ReflectNormalToggle > 0).ToString();
		array[184] = ",\t";
		array[185] = this.fingerprint._ReflectNormalToggle.ToString();
		array[186] = "\n_ReflectTex,\t";
		array[187] = (this._ReflectTex > 0).ToString();
		array[188] = ",\t";
		array[189] = this.fingerprint._ReflectTex;
		array[190] = "\n_ReflectNormalTex,\t";
		array[191] = (this._ReflectNormalTex > 0).ToString();
		array[192] = ",\t";
		array[193] = this.fingerprint._ReflectNormalTex;
		array[194] = "\n_ReflectAlbedoTint,\t";
		array[195] = (this._ReflectAlbedoTint > 0).ToString();
		array[196] = ",\t";
		array[197] = this.fingerprint._ReflectAlbedoTint.ToString();
		array[198] = "\n_ReflectTint,\t";
		array[199] = (this._ReflectTint > 0).ToString();
		array[200] = ",\t";
		int num14 = 201;
		@int = this.fingerprint._ReflectTint;
		array[num14] = @int.ToString();
		array[202] = "\n_ReflectOpacity,\t";
		array[203] = (this._ReflectOpacity > 0).ToString();
		array[204] = ",\t";
		array[205] = this.fingerprint._ReflectOpacity.ToString();
		array[206] = "\n_ReflectExposure,\t";
		array[207] = (this._ReflectExposure > 0).ToString();
		array[208] = ",\t";
		array[209] = this.fingerprint._ReflectExposure.ToString();
		array[210] = "\n_ReflectOffset,\t";
		array[211] = (this._ReflectOffset > 0).ToString();
		array[212] = ",\t";
		int num15 = 213;
		@int = this.fingerprint._ReflectOffset;
		array[num15] = @int.ToString();
		array[214] = "\n_ReflectScale,\t";
		array[215] = (this._ReflectScale > 0).ToString();
		array[216] = ",\t";
		int num16 = 217;
		@int = this.fingerprint._ReflectScale;
		array[num16] = @int.ToString();
		array[218] = "\n_ReflectRotate,\t";
		array[219] = (this._ReflectRotate > 0).ToString();
		array[220] = ",\t";
		array[221] = this.fingerprint._ReflectRotate.ToString();
		array[222] = "\n_HalfLambertToggle,\t";
		array[223] = (this._HalfLambertToggle > 0).ToString();
		array[224] = ",\t";
		array[225] = this.fingerprint._HalfLambertToggle.ToString();
		array[226] = "\n_ParallaxPlanarToggle,\t";
		array[227] = (this._ParallaxPlanarToggle > 0).ToString();
		array[228] = ",\t";
		array[229] = this.fingerprint._ParallaxPlanarToggle.ToString();
		array[230] = "\n_ParallaxToggle,\t";
		array[231] = (this._ParallaxToggle > 0).ToString();
		array[232] = ",\t";
		array[233] = this.fingerprint._ParallaxToggle.ToString();
		array[234] = "\n_ParallaxAAToggle,\t";
		array[235] = (this._ParallaxAAToggle > 0).ToString();
		array[236] = ",\t";
		array[237] = this.fingerprint._ParallaxAAToggle.ToString();
		array[238] = "\n_ParallaxAABias,\t";
		array[239] = (this._ParallaxAABias > 0).ToString();
		array[240] = ",\t";
		array[241] = this.fingerprint._ParallaxAABias.ToString();
		array[242] = "\n_DepthMap,\t";
		array[243] = (this._DepthMap > 0).ToString();
		array[244] = ",\t";
		array[245] = this.fingerprint._DepthMap;
		array[246] = "\n_ParallaxAmplitude,\t";
		array[247] = (this._ParallaxAmplitude > 0).ToString();
		array[248] = ",\t";
		array[249] = this.fingerprint._ParallaxAmplitude.ToString();
		array[250] = "\n_ParallaxSamplesMinMax,\t";
		array[251] = (this._ParallaxSamplesMinMax > 0).ToString();
		array[252] = ",\t";
		int num17 = 253;
		@int = this.fingerprint._ParallaxSamplesMinMax;
		array[num17] = @int.ToString();
		array[254] = "\n_UvShiftToggle,\t";
		array[255] = (this._UvShiftToggle > 0).ToString();
		array[256] = ",\t";
		array[257] = this.fingerprint._UvShiftToggle.ToString();
		array[258] = "\n_UvShiftSteps,\t";
		array[259] = (this._UvShiftSteps > 0).ToString();
		array[260] = ",\t";
		int num18 = 261;
		@int = this.fingerprint._UvShiftSteps;
		array[num18] = @int.ToString();
		array[262] = "\n_UvShiftRate,\t";
		array[263] = (this._UvShiftRate > 0).ToString();
		array[264] = ",\t";
		int num19 = 265;
		@int = this.fingerprint._UvShiftRate;
		array[num19] = @int.ToString();
		array[266] = "\n_UvShiftOffset,\t";
		array[267] = (this._UvShiftOffset > 0).ToString();
		array[268] = ",\t";
		int num20 = 269;
		@int = this.fingerprint._UvShiftOffset;
		array[num20] = @int.ToString();
		array[270] = "\n_UseGridEffect,\t";
		array[271] = (this._UseGridEffect > 0).ToString();
		array[272] = ",\t";
		array[273] = this.fingerprint._UseGridEffect.ToString();
		array[274] = "\n_UseCrystalEffect,\t";
		array[275] = (this._UseCrystalEffect > 0).ToString();
		array[276] = ",\t";
		array[277] = this.fingerprint._UseCrystalEffect.ToString();
		array[278] = "\n_CrystalPower,\t";
		array[279] = (this._CrystalPower > 0).ToString();
		array[280] = ",\t";
		array[281] = this.fingerprint._CrystalPower.ToString();
		array[282] = "\n_CrystalRimColor,\t";
		array[283] = (this._CrystalRimColor > 0).ToString();
		array[284] = ",\t";
		int num21 = 285;
		@int = this.fingerprint._CrystalRimColor;
		array[num21] = @int.ToString();
		array[286] = "\n_LiquidVolume,\t";
		array[287] = (this._LiquidVolume > 0).ToString();
		array[288] = ",\t";
		array[289] = this.fingerprint._LiquidVolume.ToString();
		array[290] = "\n_LiquidFill,\t";
		array[291] = (this._LiquidFill > 0).ToString();
		array[292] = ",\t";
		array[293] = this.fingerprint._LiquidFill.ToString();
		array[294] = "\n_LiquidFillNormal,\t";
		array[295] = (this._LiquidFillNormal > 0).ToString();
		array[296] = ",\t";
		int num22 = 297;
		@int = this.fingerprint._LiquidFillNormal;
		array[num22] = @int.ToString();
		array[298] = "\n_LiquidSurfaceColor,\t";
		array[299] = (this._LiquidSurfaceColor > 0).ToString();
		array[300] = ",\t";
		int num23 = 301;
		@int = this.fingerprint._LiquidSurfaceColor;
		array[num23] = @int.ToString();
		array[302] = "\n_LiquidSwayX,\t";
		array[303] = (this._LiquidSwayX > 0).ToString();
		array[304] = ",\t";
		array[305] = this.fingerprint._LiquidSwayX.ToString();
		array[306] = "\n_LiquidSwayY,\t";
		array[307] = (this._LiquidSwayY > 0).ToString();
		array[308] = ",\t";
		array[309] = this.fingerprint._LiquidSwayY.ToString();
		array[310] = "\n_LiquidContainer,\t";
		array[311] = (this._LiquidContainer > 0).ToString();
		array[312] = ",\t";
		array[313] = this.fingerprint._LiquidContainer.ToString();
		array[314] = "\n_LiquidPlanePosition,\t";
		array[315] = (this._LiquidPlanePosition > 0).ToString();
		array[316] = ",\t";
		int num24 = 317;
		@int = this.fingerprint._LiquidPlanePosition;
		array[num24] = @int.ToString();
		array[318] = "\n_LiquidPlaneNormal,\t";
		array[319] = (this._LiquidPlaneNormal > 0).ToString();
		array[320] = ",\t";
		int num25 = 321;
		@int = this.fingerprint._LiquidPlaneNormal;
		array[num25] = @int.ToString();
		array[322] = "\n_VertexFlapToggle,\t";
		array[323] = (this._VertexFlapToggle > 0).ToString();
		array[324] = ",\t";
		array[325] = this.fingerprint._VertexFlapToggle.ToString();
		array[326] = "\n_VertexFlapAxis,\t";
		array[327] = (this._VertexFlapAxis > 0).ToString();
		array[328] = ",\t";
		int num26 = 329;
		@int = this.fingerprint._VertexFlapAxis;
		array[num26] = @int.ToString();
		array[330] = "\n_VertexFlapDegreesMinMax,\t";
		array[331] = (this._VertexFlapDegreesMinMax > 0).ToString();
		array[332] = ",\t";
		int num27 = 333;
		@int = this.fingerprint._VertexFlapDegreesMinMax;
		array[num27] = @int.ToString();
		array[334] = "\n_VertexFlapSpeed,\t";
		array[335] = (this._VertexFlapSpeed > 0).ToString();
		array[336] = ",\t";
		array[337] = this.fingerprint._VertexFlapSpeed.ToString();
		array[338] = "\n_VertexFlapPhaseOffset,\t";
		array[339] = (this._VertexFlapPhaseOffset > 0).ToString();
		array[340] = ",\t";
		array[341] = this.fingerprint._VertexFlapPhaseOffset.ToString();
		array[342] = "\n_VertexWaveToggle,\t";
		array[343] = (this._VertexWaveToggle > 0).ToString();
		array[344] = ",\t";
		array[345] = this.fingerprint._VertexWaveToggle.ToString();
		array[346] = "\n_VertexWaveDebug,\t";
		array[347] = (this._VertexWaveDebug > 0).ToString();
		array[348] = ",\t";
		array[349] = this.fingerprint._VertexWaveDebug.ToString();
		array[350] = "\n_VertexWaveEnd,\t";
		array[351] = (this._VertexWaveEnd > 0).ToString();
		array[352] = ",\t";
		int num28 = 353;
		@int = this.fingerprint._VertexWaveEnd;
		array[num28] = @int.ToString();
		array[354] = "\n_VertexWaveParams,\t";
		array[355] = (this._VertexWaveParams > 0).ToString();
		array[356] = ",\t";
		int num29 = 357;
		@int = this.fingerprint._VertexWaveParams;
		array[num29] = @int.ToString();
		array[358] = "\n_VertexWaveFalloff,\t";
		array[359] = (this._VertexWaveFalloff > 0).ToString();
		array[360] = ",\t";
		int num30 = 361;
		@int = this.fingerprint._VertexWaveFalloff;
		array[num30] = @int.ToString();
		array[362] = "\n_VertexWaveSphereMask,\t";
		array[363] = (this._VertexWaveSphereMask > 0).ToString();
		array[364] = ",\t";
		int num31 = 365;
		@int = this.fingerprint._VertexWaveSphereMask;
		array[num31] = @int.ToString();
		array[366] = "\n_VertexWavePhaseOffset,\t";
		array[367] = (this._VertexWavePhaseOffset > 0).ToString();
		array[368] = ",\t";
		array[369] = this.fingerprint._VertexWavePhaseOffset.ToString();
		array[370] = "\n_VertexWaveAxes,\t";
		array[371] = (this._VertexWaveAxes > 0).ToString();
		array[372] = ",\t";
		int num32 = 373;
		@int = this.fingerprint._VertexWaveAxes;
		array[num32] = @int.ToString();
		array[374] = "\n_VertexRotateToggle,\t";
		array[375] = (this._VertexRotateToggle > 0).ToString();
		array[376] = ",\t";
		array[377] = this.fingerprint._VertexRotateToggle.ToString();
		array[378] = "\n_VertexRotateAngles,\t";
		array[379] = (this._VertexRotateAngles > 0).ToString();
		array[380] = ",\t";
		int num33 = 381;
		@int = this.fingerprint._VertexRotateAngles;
		array[num33] = @int.ToString();
		array[382] = "\n_VertexRotateAnim,\t";
		array[383] = (this._VertexRotateAnim > 0).ToString();
		array[384] = ",\t";
		array[385] = this.fingerprint._VertexRotateAnim.ToString();
		array[386] = "\n_VertexLightToggle,\t";
		array[387] = (this._VertexLightToggle > 0).ToString();
		array[388] = ",\t";
		array[389] = this.fingerprint._VertexLightToggle.ToString();
		array[390] = "\n_InnerGlowOn,\t";
		array[391] = (this._InnerGlowOn > 0).ToString();
		array[392] = ",\t";
		array[393] = this.fingerprint._InnerGlowOn.ToString();
		array[394] = "\n_InnerGlowColor,\t";
		array[395] = (this._InnerGlowColor > 0).ToString();
		array[396] = ",\t";
		int num34 = 397;
		@int = this.fingerprint._InnerGlowColor;
		array[num34] = @int.ToString();
		array[398] = "\n_InnerGlowParams,\t";
		array[399] = (this._InnerGlowParams > 0).ToString();
		array[400] = ",\t";
		int num35 = 401;
		@int = this.fingerprint._InnerGlowParams;
		array[num35] = @int.ToString();
		array[402] = "\n_InnerGlowTap,\t";
		array[403] = (this._InnerGlowTap > 0).ToString();
		array[404] = ",\t";
		array[405] = this.fingerprint._InnerGlowTap.ToString();
		array[406] = "\n_InnerGlowSine,\t";
		array[407] = (this._InnerGlowSine > 0).ToString();
		array[408] = ",\t";
		array[409] = this.fingerprint._InnerGlowSine.ToString();
		array[410] = "\n_InnerGlowSinePeriod,\t";
		array[411] = (this._InnerGlowSinePeriod > 0).ToString();
		array[412] = ",\t";
		array[413] = this.fingerprint._InnerGlowSinePeriod.ToString();
		array[414] = "\n_InnerGlowSinePhaseShift,\t";
		array[415] = (this._InnerGlowSinePhaseShift > 0).ToString();
		array[416] = ",\t";
		array[417] = this.fingerprint._InnerGlowSinePhaseShift.ToString();
		array[418] = "\n_StealthEffectOn,\t";
		array[419] = (this._StealthEffectOn > 0).ToString();
		array[420] = ",\t";
		array[421] = this.fingerprint._StealthEffectOn.ToString();
		array[422] = "\n_UseEyeTracking,\t";
		array[423] = (this._UseEyeTracking > 0).ToString();
		array[424] = ",\t";
		array[425] = this.fingerprint._UseEyeTracking.ToString();
		array[426] = "\n_EyeTileOffsetUV,\t";
		array[427] = (this._EyeTileOffsetUV > 0).ToString();
		array[428] = ",\t";
		int num36 = 429;
		@int = this.fingerprint._EyeTileOffsetUV;
		array[num36] = @int.ToString();
		array[430] = "\n_EyeOverrideUV,\t";
		array[431] = (this._EyeOverrideUV > 0).ToString();
		array[432] = ",\t";
		array[433] = this.fingerprint._EyeOverrideUV.ToString();
		array[434] = "\n_EyeOverrideUVTransform,\t";
		array[435] = (this._EyeOverrideUVTransform > 0).ToString();
		array[436] = ",\t";
		int num37 = 437;
		@int = this.fingerprint._EyeOverrideUVTransform;
		array[num37] = @int.ToString();
		array[438] = "\n_UseMouthFlap,\t";
		array[439] = (this._UseMouthFlap > 0).ToString();
		array[440] = ",\t";
		array[441] = this.fingerprint._UseMouthFlap.ToString();
		array[442] = "\n_MouthMap,\t";
		array[443] = (this._MouthMap > 0).ToString();
		array[444] = ",\t";
		array[445] = this.fingerprint._MouthMap;
		array[446] = "\n_MouthMap_ST,\t";
		array[447] = (this._MouthMap_ST > 0).ToString();
		array[448] = ",\t";
		int num38 = 449;
		@int = this.fingerprint._MouthMap_ST;
		array[num38] = @int.ToString();
		array[450] = "\n_UseVertexColor,\t";
		array[451] = (this._UseVertexColor > 0).ToString();
		array[452] = ",\t";
		array[453] = this.fingerprint._UseVertexColor.ToString();
		array[454] = "\n_WaterEffect,\t";
		array[455] = (this._WaterEffect > 0).ToString();
		array[456] = ",\t";
		array[457] = this.fingerprint._WaterEffect.ToString();
		array[458] = "\n_HeightBasedWaterEffect,\t";
		array[459] = (this._HeightBasedWaterEffect > 0).ToString();
		array[460] = ",\t";
		array[461] = this.fingerprint._HeightBasedWaterEffect.ToString();
		array[462] = "\n_WaterCaustics,\t";
		array[463] = (this._WaterCaustics > 0).ToString();
		array[464] = ",\t";
		array[465] = this.fingerprint._WaterCaustics.ToString();
		array[466] = "\n_UseDayNightLightmap,\t";
		array[467] = (this._UseDayNightLightmap > 0).ToString();
		array[468] = ",\t";
		array[469] = this.fingerprint._UseDayNightLightmap.ToString();
		array[470] = "\n_UseSpecular,\t";
		array[471] = (this._UseSpecular > 0).ToString();
		array[472] = ",\t";
		array[473] = this.fingerprint._UseSpecular.ToString();
		array[474] = "\n_UseSpecularAlphaChannel,\t";
		array[475] = (this._UseSpecularAlphaChannel > 0).ToString();
		array[476] = ",\t";
		array[477] = this.fingerprint._UseSpecularAlphaChannel.ToString();
		array[478] = "\n_Smoothness,\t";
		array[479] = (this._Smoothness > 0).ToString();
		array[480] = ",\t";
		array[481] = this.fingerprint._Smoothness.ToString();
		array[482] = "\n_UseSpecHighlight,\t";
		array[483] = (this._UseSpecHighlight > 0).ToString();
		array[484] = ",\t";
		array[485] = this.fingerprint._UseSpecHighlight.ToString();
		array[486] = "\n_SpecularDir,\t";
		array[487] = (this._SpecularDir > 0).ToString();
		array[488] = ",\t";
		int num39 = 489;
		@int = this.fingerprint._SpecularDir;
		array[num39] = @int.ToString();
		array[490] = "\n_SpecularPowerIntensity,\t";
		array[491] = (this._SpecularPowerIntensity > 0).ToString();
		array[492] = ",\t";
		int num40 = 493;
		@int = this.fingerprint._SpecularPowerIntensity;
		array[num40] = @int.ToString();
		array[494] = "\n_SpecularColor,\t";
		array[495] = (this._SpecularColor > 0).ToString();
		array[496] = ",\t";
		int num41 = 497;
		@int = this.fingerprint._SpecularColor;
		array[num41] = @int.ToString();
		array[498] = "\n_SpecularUseDiffuseColor,\t";
		array[499] = (this._SpecularUseDiffuseColor > 0).ToString();
		array[500] = ",\t";
		array[501] = this.fingerprint._SpecularUseDiffuseColor.ToString();
		array[502] = "\n_EmissionToggle,\t";
		array[503] = (this._EmissionToggle > 0).ToString();
		array[504] = ",\t";
		array[505] = this.fingerprint._EmissionToggle.ToString();
		array[506] = "\n_EmissionColor,\t";
		array[507] = (this._EmissionColor > 0).ToString();
		array[508] = ",\t";
		int num42 = 509;
		@int = this.fingerprint._EmissionColor;
		array[num42] = @int.ToString();
		array[510] = "\n_EmissionMap,\t";
		array[511] = (this._EmissionMap > 0).ToString();
		array[512] = ",\t";
		array[513] = this.fingerprint._EmissionMap;
		array[514] = "\n_EmissionMaskByBaseMapAlpha,\t";
		array[515] = (this._EmissionMaskByBaseMapAlpha > 0).ToString();
		array[516] = ",\t";
		array[517] = this.fingerprint._EmissionMaskByBaseMapAlpha.ToString();
		array[518] = "\n_EmissionUVScrollSpeed,\t";
		array[519] = (this._EmissionUVScrollSpeed > 0).ToString();
		array[520] = ",\t";
		int num43 = 521;
		@int = this.fingerprint._EmissionUVScrollSpeed;
		array[num43] = @int.ToString();
		array[522] = "\n_EmissionDissolveProgress,\t";
		array[523] = (this._EmissionDissolveProgress > 0).ToString();
		array[524] = ",\t";
		array[525] = this.fingerprint._EmissionDissolveProgress.ToString();
		array[526] = "\n_EmissionDissolveAnimation,\t";
		array[527] = (this._EmissionDissolveAnimation > 0).ToString();
		array[528] = ",\t";
		int num44 = 529;
		@int = this.fingerprint._EmissionDissolveAnimation;
		array[num44] = @int.ToString();
		array[530] = "\n_EmissionDissolveEdgeSize,\t";
		array[531] = (this._EmissionDissolveEdgeSize > 0).ToString();
		array[532] = ",\t";
		array[533] = this.fingerprint._EmissionDissolveEdgeSize.ToString();
		array[534] = "\n_EmissionIntensityInDynamic,\t";
		array[535] = (this._EmissionIntensityInDynamic > 0).ToString();
		array[536] = ",\t";
		array[537] = this.fingerprint._EmissionIntensityInDynamic.ToString();
		array[538] = "\n_EmissionUseUVWaveWarp,\t";
		array[539] = (this._EmissionUseUVWaveWarp > 0).ToString();
		array[540] = ",\t";
		array[541] = this.fingerprint._EmissionUseUVWaveWarp.ToString();
		array[542] = "\n_GreyZoneException,\t";
		array[543] = (this._GreyZoneException > 0).ToString();
		array[544] = ",\t";
		array[545] = this.fingerprint._GreyZoneException.ToString();
		array[546] = "\n_Cull,\t";
		array[547] = (this._Cull > 0).ToString();
		array[548] = ",\t";
		array[549] = this.fingerprint._Cull.ToString();
		array[550] = "\n_StencilReference,\t";
		array[551] = (this._StencilReference > 0).ToString();
		array[552] = ",\t";
		array[553] = this.fingerprint._StencilReference.ToString();
		array[554] = "\n_StencilComparison,\t";
		array[555] = (this._StencilComparison > 0).ToString();
		array[556] = ",\t";
		array[557] = this.fingerprint._StencilComparison.ToString();
		array[558] = "\n_StencilPassFront,\t";
		array[559] = (this._StencilPassFront > 0).ToString();
		array[560] = ",\t";
		array[561] = this.fingerprint._StencilPassFront.ToString();
		array[562] = "\n_USE_DEFORM_MAP,\t";
		array[563] = (this._USE_DEFORM_MAP > 0).ToString();
		array[564] = ",\t";
		array[565] = this.fingerprint._USE_DEFORM_MAP.ToString();
		array[566] = "\n_DeformMap,\t";
		array[567] = (this._DeformMap > 0).ToString();
		array[568] = ",\t";
		array[569] = this.fingerprint._DeformMap;
		array[570] = "\n_DeformMapIntensity,\t";
		array[571] = (this._DeformMapIntensity > 0).ToString();
		array[572] = ",\t";
		array[573] = this.fingerprint._DeformMapIntensity.ToString();
		array[574] = "\n_DeformMapMaskByVertColorRAmount,\t";
		array[575] = (this._DeformMapMaskByVertColorRAmount > 0).ToString();
		array[576] = ",\t";
		array[577] = this.fingerprint._DeformMapMaskByVertColorRAmount.ToString();
		array[578] = "\n_DeformMapScrollSpeed,\t";
		array[579] = (this._DeformMapScrollSpeed > 0).ToString();
		array[580] = ",\t";
		int num45 = 581;
		@int = this.fingerprint._DeformMapScrollSpeed;
		array[num45] = @int.ToString();
		array[582] = "\n_DeformMapUV0Influence,\t";
		array[583] = (this._DeformMapUV0Influence > 0).ToString();
		array[584] = ",\t";
		int num46 = 585;
		@int = this.fingerprint._DeformMapUV0Influence;
		array[num46] = @int.ToString();
		array[586] = "\n_DeformMapObjectSpaceOffsetsU,\t";
		array[587] = (this._DeformMapObjectSpaceOffsetsU > 0).ToString();
		array[588] = ",\t";
		int num47 = 589;
		@int = this.fingerprint._DeformMapObjectSpaceOffsetsU;
		array[num47] = @int.ToString();
		array[590] = "\n_DeformMapObjectSpaceOffsetsV,\t";
		array[591] = (this._DeformMapObjectSpaceOffsetsV > 0).ToString();
		array[592] = ",\t";
		int num48 = 593;
		@int = this.fingerprint._DeformMapObjectSpaceOffsetsV;
		array[num48] = @int.ToString();
		array[594] = "\n_DeformMapWorldSpaceOffsetsU,\t";
		array[595] = (this._DeformMapWorldSpaceOffsetsU > 0).ToString();
		array[596] = ",\t";
		int num49 = 597;
		@int = this.fingerprint._DeformMapWorldSpaceOffsetsU;
		array[num49] = @int.ToString();
		array[598] = "\n_DeformMapWorldSpaceOffsetsV,\t";
		array[599] = (this._DeformMapWorldSpaceOffsetsV > 0).ToString();
		array[600] = ",\t";
		int num50 = 601;
		@int = this.fingerprint._DeformMapWorldSpaceOffsetsV;
		array[num50] = @int.ToString();
		array[602] = "\n_RotateOnYAxisBySinTime,\t";
		array[603] = (this._RotateOnYAxisBySinTime > 0).ToString();
		array[604] = ",\t";
		int num51 = 605;
		@int = this.fingerprint._RotateOnYAxisBySinTime;
		array[num51] = @int.ToString();
		array[606] = "\n_USE_TEX_ARRAY_ATLAS,\t";
		array[607] = (this._USE_TEX_ARRAY_ATLAS > 0).ToString();
		array[608] = ",\t";
		array[609] = this.fingerprint._USE_TEX_ARRAY_ATLAS.ToString();
		array[610] = "\n_BaseMap_Atlas,\t";
		array[611] = (this._BaseMap_Atlas > 0).ToString();
		array[612] = ",\t";
		array[613] = this.fingerprint._BaseMap_Atlas;
		array[614] = "\n_BaseMap_AtlasSlice,\t";
		array[615] = (this._BaseMap_AtlasSlice > 0).ToString();
		array[616] = ",\t";
		array[617] = this.fingerprint._BaseMap_AtlasSlice.ToString();
		array[618] = "\n_BaseMap_AtlasSliceSource,\t";
		array[619] = (this._BaseMap_AtlasSliceSource > 0).ToString();
		array[620] = ",\t";
		array[621] = this.fingerprint._BaseMap_AtlasSliceSource.ToString();
		array[622] = "\n_EmissionMap_Atlas,\t";
		array[623] = (this._EmissionMap_Atlas > 0).ToString();
		array[624] = ",\t";
		array[625] = this.fingerprint._EmissionMap_Atlas;
		array[626] = "\n_EmissionMap_AtlasSlice,\t";
		array[627] = (this._EmissionMap_AtlasSlice > 0).ToString();
		array[628] = ",\t";
		array[629] = this.fingerprint._EmissionMap_AtlasSlice.ToString();
		array[630] = "\n_DeformMap_Atlas,\t";
		array[631] = (this._DeformMap_Atlas > 0).ToString();
		array[632] = ",\t";
		array[633] = this.fingerprint._DeformMap_Atlas;
		array[634] = "\n_DeformMap_AtlasSlice,\t";
		array[635] = (this._DeformMap_AtlasSlice > 0).ToString();
		array[636] = ",\t";
		array[637] = this.fingerprint._DeformMap_AtlasSlice.ToString();
		array[638] = "\n_WeatherMap_Atlas,\t";
		array[639] = (this._WeatherMap_Atlas > 0).ToString();
		array[640] = ",\t";
		array[641] = this.fingerprint._WeatherMap_Atlas;
		array[642] = "\n_WeatherMap_AtlasSlice,\t";
		array[643] = (this._WeatherMap_AtlasSlice > 0).ToString();
		array[644] = ",\t";
		array[645] = this.fingerprint._WeatherMap_AtlasSlice.ToString();
		array[646] = "\n_DEBUG_PAWN_DATA,\t";
		array[647] = (this._DEBUG_PAWN_DATA > 0).ToString();
		array[648] = ",\t";
		array[649] = this.fingerprint._DEBUG_PAWN_DATA.ToString();
		array[650] = "\n_SrcBlend,\t";
		array[651] = (this._SrcBlend > 0).ToString();
		array[652] = ",\t";
		array[653] = this.fingerprint._SrcBlend.ToString();
		array[654] = "\n_DstBlend,\t";
		array[655] = (this._DstBlend > 0).ToString();
		array[656] = ",\t";
		array[657] = this.fingerprint._DstBlend.ToString();
		array[658] = "\n_SrcBlendAlpha,\t";
		array[659] = (this._SrcBlendAlpha > 0).ToString();
		array[660] = ",\t";
		array[661] = this.fingerprint._SrcBlendAlpha.ToString();
		array[662] = "\n_DstBlendAlpha,\t";
		array[663] = (this._DstBlendAlpha > 0).ToString();
		array[664] = ",\t";
		array[665] = this.fingerprint._DstBlendAlpha.ToString();
		array[666] = "\n_ZWrite,\t";
		array[667] = (this._ZWrite > 0).ToString();
		array[668] = ",\t";
		array[669] = this.fingerprint._ZWrite.ToString();
		array[670] = "\n_AlphaToMask,\t";
		array[671] = (this._AlphaToMask > 0).ToString();
		array[672] = ",\t";
		array[673] = this.fingerprint._AlphaToMask.ToString();
		array[674] = "\n_Color,\t";
		array[675] = (this._Color > 0).ToString();
		array[676] = ",\t";
		int num52 = 677;
		@int = this.fingerprint._Color;
		array[num52] = @int.ToString();
		array[678] = "\n_Surface,\t";
		array[679] = (this._Surface > 0).ToString();
		array[680] = ",\t";
		array[681] = this.fingerprint._Surface.ToString();
		array[682] = "\n_Metallic,\t";
		array[683] = (this._Metallic > 0).ToString();
		array[684] = ",\t";
		array[685] = this.fingerprint._Metallic.ToString();
		array[686] = "\n_SpecColor,\t";
		array[687] = (this._SpecColor > 0).ToString();
		array[688] = ",\t";
		int num53 = 689;
		@int = this.fingerprint._SpecColor;
		array[num53] = @int.ToString();
		array[690] = "\n_DayNightLightmapArray,\t";
		array[691] = (this._DayNightLightmapArray > 0).ToString();
		array[692] = ",\t";
		array[693] = this.fingerprint._DayNightLightmapArray;
		array[694] = "\n_DayNightLightmapArray_ST,\t";
		array[695] = (this._DayNightLightmapArray_ST > 0).ToString();
		array[696] = ",\t";
		int num54 = 697;
		@int = this.fingerprint._DayNightLightmapArray_ST;
		array[num54] = @int.ToString();
		array[698] = "\n_DayNightLightmapArray_AtlasSlice,\t";
		array[699] = (this._DayNightLightmapArray_AtlasSlice > 0).ToString();
		array[700] = ",\t";
		array[701] = this.fingerprint._DayNightLightmapArray_AtlasSlice.ToString();
		array[702] = "\n";
		return string.Concat(array);
	}

	public static void _g_Macro_TRANSFORM_TEX(in GTUberShader_MaterialKeywordStates kw, ref int tex, ref int tex_ST)
	{
		tex++;
		tex_ST++;
	}

	private static void _g_Macro_DECLARE_ATLASABLE_TEX2D(in GTUberShader_MaterialKeywordStates kw, ref int tex, ref int tex_Atlas)
	{
		tex += ((!kw._USE_TEX_ARRAY_ATLAS) ? 1 : 0);
		tex_Atlas += (kw._USE_TEX_ARRAY_ATLAS ? 1 : 0);
	}

	private static void _g_Macro_DECLARE_ATLASABLE_SAMPLER(in GTUberShader_MaterialKeywordStates kw, ref int sampler, ref int sampler_Atlas)
	{
		sampler += ((!kw._USE_TEX_ARRAY_ATLAS) ? 1 : 0);
		sampler_Atlas += (kw._USE_TEX_ARRAY_ATLAS ? 1 : 0);
	}

	private static void _g_Macro_SAMPLE_ATLASABLE_TEX2D(in GTUberShader_MaterialKeywordStates kw, ref int tex, ref int tex_Atlas, ref int tex_AtlasSlice, ref int sampler, ref int sampler_Atlas, ref int coord2, ref int mipBias)
	{
		tex += ((!kw._USE_TEX_ARRAY_ATLAS) ? 1 : 0);
		tex_Atlas += (kw._USE_TEX_ARRAY_ATLAS ? 1 : 0);
		tex_AtlasSlice += (kw._USE_TEX_ARRAY_ATLAS ? 1 : 0);
		sampler += ((!kw._USE_TEX_ARRAY_ATLAS) ? 1 : 0);
		sampler_Atlas += (kw._USE_TEX_ARRAY_ATLAS ? 1 : 0);
		mipBias++;
		coord2++;
	}

	private static void _g_Macro_SAMPLE_ATLASABLE_TEX2D_LOD(in GTUberShader_MaterialKeywordStates kw, ref int texName, ref int texName_Atlas)
	{
		texName += ((!kw._USE_TEX_ARRAY_ATLAS) ? 1 : 0);
		texName_Atlas += (kw._USE_TEX_ARRAY_ATLAS ? 1 : 0);
	}

	private static void _g_Macro_SAMPLE_ATLASABLE_TEX2D_LOD(in GTUberShader_MaterialKeywordStates kw, ref int texName, ref int texName_Atlas, ref int sampler, ref int coord2, ref int lod)
	{
		texName += ((!kw._USE_TEX_ARRAY_ATLAS) ? 1 : 0);
		texName_Atlas += (kw._USE_TEX_ARRAY_ATLAS ? 1 : 0);
		sampler++;
		coord2++;
		lod++;
	}

	public Material material;

	public GTUberShader_MaterialKeywordStates kw;

	public MaterialFingerprint fingerprint;

	public bool IsValid;

	private readonly int _notAProp;

	public int _TransparencyMode;

	public int _Cutoff;

	public int _ColorSource;

	public int _BaseColor;

	public int _GChannelColor;

	public int _BChannelColor;

	public int _AChannelColor;

	public int _BaseMap;

	public int _BaseMap_ST;

	public int _SettingsPreset;

	public int _AdvancedOptions;

	public int _TexMipBias;

	public int _BaseMap_WH;

	public int _TexelSnapToggle;

	public int _TexelSnap_Factor;

	public int _UVSource;

	public int _AlphaDetailToggle;

	public int _AlphaDetail_ST;

	public int _AlphaDetail_Opacity;

	public int _AlphaDetail_WorldSpace;

	public int _MaskMapToggle;

	public int _MaskMap;

	public int _MaskMap_ST;

	public int _MaskMap_WH;

	public int _LavaLampToggle;

	public int _GradientMapToggle;

	public int _GradientMap;

	public int _DoTextureRotation;

	public int _RotateAngle;

	public int _RotateAnim;

	public int _UseWaveWarp;

	public int _WaveAmplitude;

	public int _WaveFrequency;

	public int _WaveScale;

	public int _WaveTimeScale;

	public int _UseWeatherMap;

	public int _WeatherMap;

	public int _WeatherMapDissolveEdgeSize;

	public int _ReflectToggle;

	public int _ReflectBoxProjectToggle;

	public int _ReflectBoxCubePos;

	public int _ReflectBoxSize;

	public int _ReflectBoxRotation;

	public int _ReflectMatcapToggle;

	public int _ReflectMatcapPerspToggle;

	public int _ReflectNormalToggle;

	public int _ReflectTex;

	public int _ReflectNormalTex;

	public int _ReflectAlbedoTint;

	public int _ReflectTint;

	public int _ReflectOpacity;

	public int _ReflectExposure;

	public int _ReflectOffset;

	public int _ReflectScale;

	public int _ReflectRotate;

	public int _HalfLambertToggle;

	public int _ParallaxPlanarToggle;

	public int _ParallaxToggle;

	public int _ParallaxAAToggle;

	public int _ParallaxAABias;

	public int _DepthMap;

	public int _ParallaxAmplitude;

	public int _ParallaxSamplesMinMax;

	public int _UvShiftToggle;

	public int _UvShiftSteps;

	public int _UvShiftRate;

	public int _UvShiftOffset;

	public int _UseGridEffect;

	public int _UseCrystalEffect;

	public int _CrystalPower;

	public int _CrystalRimColor;

	public int _LiquidVolume;

	public int _LiquidFill;

	public int _LiquidFillNormal;

	public int _LiquidSurfaceColor;

	public int _LiquidSwayX;

	public int _LiquidSwayY;

	public int _LiquidContainer;

	public int _LiquidPlanePosition;

	public int _LiquidPlaneNormal;

	public int _VertexFlapToggle;

	public int _VertexFlapAxis;

	public int _VertexFlapDegreesMinMax;

	public int _VertexFlapSpeed;

	public int _VertexFlapPhaseOffset;

	public int _VertexWaveToggle;

	public int _VertexWaveDebug;

	public int _VertexWaveEnd;

	public int _VertexWaveParams;

	public int _VertexWaveFalloff;

	public int _VertexWaveSphereMask;

	public int _VertexWavePhaseOffset;

	public int _VertexWaveAxes;

	public int _VertexRotateToggle;

	public int _VertexRotateAngles;

	public int _VertexRotateAnim;

	public int _VertexLightToggle;

	public int _InnerGlowOn;

	public int _InnerGlowColor;

	public int _InnerGlowParams;

	public int _InnerGlowTap;

	public int _InnerGlowSine;

	public int _InnerGlowSinePeriod;

	public int _InnerGlowSinePhaseShift;

	public int _StealthEffectOn;

	public int _UseEyeTracking;

	public int _EyeTileOffsetUV;

	public int _EyeOverrideUV;

	public int _EyeOverrideUVTransform;

	public int _UseMouthFlap;

	public int _MouthMap;

	public int _MouthMap_ST;

	public int _UseVertexColor;

	public int _WaterEffect;

	public int _HeightBasedWaterEffect;

	public int _WaterCaustics;

	public int _UseDayNightLightmap;

	public int _UseSpecular;

	public int _UseSpecularAlphaChannel;

	public int _Smoothness;

	public int _UseSpecHighlight;

	public int _SpecularDir;

	public int _SpecularPowerIntensity;

	public int _SpecularColor;

	public int _SpecularUseDiffuseColor;

	public int _EmissionToggle;

	public int _EmissionColor;

	public int _EmissionMap;

	public int _EmissionMaskByBaseMapAlpha;

	public int _EmissionUVScrollSpeed;

	public int _EmissionDissolveProgress;

	public int _EmissionDissolveAnimation;

	public int _EmissionDissolveEdgeSize;

	public int _EmissionIntensityInDynamic;

	public int _EmissionUseUVWaveWarp;

	public int _GreyZoneException;

	public int _Cull;

	public int _StencilReference;

	public int _StencilComparison;

	public int _StencilPassFront;

	public int _USE_DEFORM_MAP;

	public int _DeformMap;

	public int _DeformMapIntensity;

	public int _DeformMapMaskByVertColorRAmount;

	public int _DeformMapScrollSpeed;

	public int _DeformMapUV0Influence;

	public int _DeformMapObjectSpaceOffsetsU;

	public int _DeformMapObjectSpaceOffsetsV;

	public int _DeformMapWorldSpaceOffsetsU;

	public int _DeformMapWorldSpaceOffsetsV;

	public int _RotateOnYAxisBySinTime;

	public int _USE_TEX_ARRAY_ATLAS;

	public int _BaseMap_Atlas;

	public int _BaseMap_AtlasSlice;

	public int _BaseMap_AtlasSliceSource;

	public int _EmissionMap_Atlas;

	public int _EmissionMap_AtlasSlice;

	public int _DeformMap_Atlas;

	public int _DeformMap_AtlasSlice;

	public int _WeatherMap_Atlas;

	public int _WeatherMap_AtlasSlice;

	public int _DEBUG_PAWN_DATA;

	public int _SrcBlend;

	public int _DstBlend;

	public int _SrcBlendAlpha;

	public int _DstBlendAlpha;

	public int _ZWrite;

	public int _AlphaToMask;

	public int _Color;

	public int _Surface;

	public int _Metallic;

	public int _SpecColor;

	public int _DayNightLightmapArray;

	public int _DayNightLightmapArray_ST;

	public int _DayNightLightmapArray_AtlasSlice;
}
