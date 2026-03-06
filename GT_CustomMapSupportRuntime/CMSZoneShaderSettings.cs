using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GT_CustomMapSupportRuntime
{
	[NullableContext(2)]
	[Nullable(0)]
	public class CMSZoneShaderSettings : MonoBehaviour
	{
		public bool isActiveInstance
		{
			get
			{
				return CMSZoneShaderSettings.activeInstance == this;
			}
		}

		public int GetGroundFogColorOverrideMode()
		{
			if (this.isDefaultValues)
			{
				return 1;
			}
			return (int)this.groundFogColor_overrideMode;
		}

		public int GetGroundFogDepthFadeOverrideMode()
		{
			if (this.isDefaultValues)
			{
				return 1;
			}
			return (int)this.groundFogDepthFade_overrideMode;
		}

		public float GroundFogDepthFadeSq
		{
			get
			{
				return 1f / Mathf.Max(1E-05f, this.groundFogDepthFadeSize * this.groundFogDepthFadeSize);
			}
		}

		public int GetGroundFogHeightOverrideMode()
		{
			if (this.isDefaultValues)
			{
				return 1;
			}
			return (int)this.groundFogHeight_overrideMode;
		}

		public int GetGroundFogHeightFadeOverrideMode()
		{
			if (this.isDefaultValues)
			{
				return 1;
			}
			return (int)this.groundFogHeightFade_overrideMode;
		}

		public float GroundFogHeightFade
		{
			get
			{
				return 1f / Mathf.Max(1E-05f, this.groundFogHeightFadeSize);
			}
		}

		public void SetZoneLiquidTypeKeywordEnum(CMSZoneShaderSettings.EZoneLiquidType liquidType)
		{
			if (this.isExported)
			{
				return;
			}
			if (liquidType == CMSZoneShaderSettings.EZoneLiquidType.None)
			{
				Shader.EnableKeyword("_GLOBAL_ZONE_LIQUID_TYPE__NONE");
			}
			else
			{
				Shader.DisableKeyword("_GLOBAL_ZONE_LIQUID_TYPE__NONE");
			}
			if (liquidType == CMSZoneShaderSettings.EZoneLiquidType.Water)
			{
				Shader.EnableKeyword("_GLOBAL_ZONE_LIQUID_TYPE__WATER");
			}
			else
			{
				Shader.DisableKeyword("_GLOBAL_ZONE_LIQUID_TYPE__WATER");
			}
			if (liquidType == CMSZoneShaderSettings.EZoneLiquidType.Lava)
			{
				Shader.EnableKeyword("_GLOBAL_ZONE_LIQUID_TYPE__LAVA");
				return;
			}
			Shader.DisableKeyword("_GLOBAL_ZONE_LIQUID_TYPE__LAVA");
		}

		public int GetZoneLiquidTypeOverrideMode()
		{
			if (this.isDefaultValues)
			{
				return 1;
			}
			return (int)this.zoneLiquidType_overrideMode;
		}

		public int GetZoneLiquidType()
		{
			return (int)this.zoneLiquidType;
		}

		public void SetZoneLiquidShapeKeywordEnum(CMSZoneShaderSettings.ELiquidShape shape)
		{
			if (this.isExported)
			{
				return;
			}
			if (shape == CMSZoneShaderSettings.ELiquidShape.Plane)
			{
				Shader.EnableKeyword("_ZONE_LIQUID_SHAPE__PLANE");
			}
			else
			{
				Shader.DisableKeyword("_ZONE_LIQUID_SHAPE__PLANE");
			}
			if (shape == CMSZoneShaderSettings.ELiquidShape.Cylinder)
			{
				Debug.Log("Enable CYLINDER liquid...");
				Shader.EnableKeyword("_ZONE_LIQUID_SHAPE__CYLINDER");
				return;
			}
			Shader.DisableKeyword("_ZONE_LIQUID_SHAPE__CYLINDER");
		}

		public int GetLiquidShapeOverrideMode()
		{
			if (this.isDefaultValues)
			{
				return 1;
			}
			return (int)this.liquidShape_overrideMode;
		}

		public int GetZoneLiquidShape()
		{
			return (int)this.liquidShape;
		}

		private static int shaderParam_ZoneLiquidPosRadiusSq { get; set; } = Shader.PropertyToID("_ZoneLiquidPosRadiusSq");

		public int GetLiquidShapeRadiusOverrideMode()
		{
			if (this.isDefaultValues)
			{
				return 1;
			}
			return (int)this.liquidShapeRadius_overrideMode;
		}

		public int GetLiquidBottomTransformOverrideMode()
		{
			if (this.isDefaultValues)
			{
				return 1;
			}
			return (int)this.liquidBottomTransform_overrideMode;
		}

		public static float GetWaterY()
		{
			if (!(CMSZoneShaderSettings.activeInstance == null) && !(CMSZoneShaderSettings.activeInstance.mainWaterSurfacePlane == null))
			{
				return CMSZoneShaderSettings.activeInstance.mainWaterSurfacePlane.position.y;
			}
			return -1f;
		}

		public int GetZoneLiquidUVScaleOverrideMode()
		{
			if (this.isDefaultValues)
			{
				return 1;
			}
			return (int)this.zoneLiquidUVScale_overrideMode;
		}

		public int GetUnderwaterTintColorOverrideMode()
		{
			if (this.isDefaultValues)
			{
				return 1;
			}
			return (int)this.underwaterTintColor_overrideMode;
		}

		public int GetUnderwaterFogColorOverrideMode()
		{
			if (this.isDefaultValues)
			{
				return 1;
			}
			return (int)this.underwaterFogColor_overrideMode;
		}

		public int GetUnderwaterFogParamsOverrideMode()
		{
			if (this.isDefaultValues)
			{
				return 1;
			}
			return (int)this.underwaterFogParams_overrideMode;
		}

		public int GetUnderwaterCausticsParamsOverrideMode()
		{
			if (this.isDefaultValues)
			{
				return 1;
			}
			return (int)this.underwaterCausticsParams_overrideMode;
		}

		public int GetUnderwaterCausticsTextureOverrideMode()
		{
			if (this.isDefaultValues)
			{
				return 1;
			}
			return (int)this.underwaterCausticsTexture_overrideMode;
		}

		public int GetUnderwaterEffectsDistanceToSurfaceFadeOverrideMode()
		{
			if (this.isDefaultValues)
			{
				return 1;
			}
			return (int)this.underwaterEffectsDistanceToSurfaceFade_overrideMode;
		}

		public int GetLiquidResidueTextureOverrideMode()
		{
			if (this.isDefaultValues)
			{
				return 1;
			}
			return (int)this.liquidResidueTex_overrideMode;
		}

		public int GetMainWaterSurfacePlaneOverrideMode()
		{
			if (this.isDefaultValues)
			{
				return 1;
			}
			return (int)this.mainWaterSurfacePlane_overrideMode;
		}

		public void Initialize()
		{
			if (this.isExported)
			{
				return;
			}
			if (this.mainWaterSurfacePlane == null)
			{
				this.hasMainWaterSurfacePlane = false;
				this.hasDynamicWaterSurfacePlane = false;
			}
			else
			{
				this.hasMainWaterSurfacePlane = (this.mainWaterSurfacePlane_overrideMode == CMSZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues);
				this.hasDynamicWaterSurfacePlane = (this.hasMainWaterSurfacePlane && !this.mainWaterSurfacePlane.gameObject.isStatic);
			}
			this.hasLiquidBottomTransform = (this.liquidBottomTransform != null && (this.liquidBottomTransform_overrideMode == CMSZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues));
			this.CheckDefaultsInstance();
			if (this.activateOnLoad)
			{
				this.BecomeActiveInstance(false);
			}
		}

		protected void OnDestroy()
		{
			if (this.isExported)
			{
				return;
			}
			if (CMSZoneShaderSettings.defaultsInstance == this)
			{
				CMSZoneShaderSettings.hasDefaultsInstance = false;
			}
			if (CMSZoneShaderSettings.activeInstance == this)
			{
				CMSZoneShaderSettings.hasActiveInstance = false;
			}
		}

		private void UpdateMainPlaneShaderProperty()
		{
			if (this.isExported)
			{
				return;
			}
			Transform transform = null;
			if (this.hasMainWaterSurfacePlane && (this.mainWaterSurfacePlane_overrideMode == CMSZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues))
			{
				transform = this.mainWaterSurfacePlane;
			}
			else if (this.mainWaterSurfacePlane_overrideMode == CMSZoneShaderSettings.EOverrideMode.ApplyDefaultValue && CMSZoneShaderSettings.defaultsInstance != null && CMSZoneShaderSettings.defaultsInstance.hasMainWaterSurfacePlane)
			{
				transform = CMSZoneShaderSettings.defaultsInstance.mainWaterSurfacePlane;
			}
			if (transform == null)
			{
				return;
			}
			Vector3 position = transform.position;
			Vector3 up = transform.up;
			float w = -Vector3.Dot(up, position);
			Shader.SetGlobalVector(this.shaderParam_GlobalMainWaterSurfacePlane, new Vector4(up.x, up.y, up.z, w));
			CMSZoneShaderSettings.ELiquidShape eliquidShape;
			if (this.liquidShape_overrideMode == CMSZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				eliquidShape = this.liquidShape;
			}
			else if (this.liquidShape_overrideMode == CMSZoneShaderSettings.EOverrideMode.ApplyDefaultValue && CMSZoneShaderSettings.defaultsInstance != null)
			{
				eliquidShape = CMSZoneShaderSettings.defaultsInstance.liquidShape;
			}
			else
			{
				eliquidShape = CMSZoneShaderSettings.liquidShape_previousValue;
			}
			CMSZoneShaderSettings.liquidShape_previousValue = eliquidShape;
			float y;
			if ((this.liquidBottomTransform_overrideMode == CMSZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues) && this.liquidBottomTransform != null)
			{
				y = this.liquidBottomTransform.position.y;
			}
			else if (this.liquidBottomTransform_overrideMode == CMSZoneShaderSettings.EOverrideMode.ApplyDefaultValue && CMSZoneShaderSettings.defaultsInstance != null && CMSZoneShaderSettings.defaultsInstance.liquidBottomTransform != null)
			{
				y = CMSZoneShaderSettings.defaultsInstance.liquidBottomTransform.position.y;
			}
			else
			{
				y = this.liquidBottomPosY_previousValue;
			}
			this.liquidBottomPosY_previousValue = y;
			float num;
			if (this.liquidShapeRadius_overrideMode == CMSZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				num = this.liquidShapeRadius;
			}
			else if (this.liquidShape_overrideMode == CMSZoneShaderSettings.EOverrideMode.ApplyDefaultValue && CMSZoneShaderSettings.defaultsInstance != null)
			{
				num = CMSZoneShaderSettings.defaultsInstance.liquidShapeRadius;
			}
			else
			{
				num = CMSZoneShaderSettings.liquidShapeRadius_previousValue;
			}
			if (eliquidShape == CMSZoneShaderSettings.ELiquidShape.Cylinder)
			{
				Debug.Log("Setting Cylinder Liquid Radius...");
				Shader.SetGlobalVector(CMSZoneShaderSettings.shaderParam_ZoneLiquidPosRadiusSq, new Vector4(position.x, y, position.z, num * num));
				CMSZoneShaderSettings.liquidShapeRadius_previousValue = num;
			}
		}

		private void CheckDefaultsInstance()
		{
			if (this.isExported)
			{
				return;
			}
			if (!this.isDefaultValues)
			{
				return;
			}
			if (!CMSZoneShaderSettings.hasDefaultsInstance || !(CMSZoneShaderSettings.defaultsInstance != null) || !(CMSZoneShaderSettings.defaultsInstance != this))
			{
				CMSZoneShaderSettings.defaultsInstance = this;
				CMSZoneShaderSettings.hasDefaultsInstance = true;
				this.BecomeActiveInstance(false);
				return;
			}
			if (!Application.isPlaying)
			{
				Debug.LogWarning("CMSZoneShaderSettings: (Edit time warning) Deactivating instance with `isDefaultValues` set to true. CMSZoneShaderSettings Object: " + base.name, this);
				base.gameObject.SetActive(false);
				return;
			}
			string hierarchyPath = CMSZoneShaderSettings.defaultsInstance.transform.GetHierarchyPath();
			Debug.LogError(string.Concat(new string[]
			{
				"CMSZoneShaderSettings: Destroying conflicting defaults instance.\n- keeping: \"",
				hierarchyPath,
				"\"\n- destroying (this): \"",
				base.transform.GetHierarchyPath(),
				"\""
			}), this);
			Object.Destroy(base.gameObject);
		}

		public void BecomeActiveInstance(bool force = false)
		{
			if (this.isExported)
			{
				return;
			}
			if (CMSZoneShaderSettings.activeInstance == this && !force)
			{
				return;
			}
			this.ApplyValues();
			CMSZoneShaderSettings.activeInstance = this;
			CMSZoneShaderSettings.hasActiveInstance = true;
		}

		public static void ActivateDefaultSettings()
		{
			if (CMSZoneShaderSettings.defaultsInstance != null)
			{
				CMSZoneShaderSettings.defaultsInstance.BecomeActiveInstance(false);
			}
		}

		public void SetGroundFogValue(Color fogColor, float fogDepthFade, float fogHeight, float fogHeightFade)
		{
			if (this.isExported)
			{
				return;
			}
			this.groundFogColor_overrideMode = CMSZoneShaderSettings.EOverrideMode.ApplyNewValue;
			this.groundFogColor = fogColor;
			this.groundFogDepthFade_overrideMode = CMSZoneShaderSettings.EOverrideMode.ApplyNewValue;
			this.groundFogDepthFadeSize = fogDepthFade;
			this.groundFogHeight_overrideMode = CMSZoneShaderSettings.EOverrideMode.ApplyNewValue;
			this.groundFogHeight = fogHeight;
			this.groundFogHeightFade_overrideMode = CMSZoneShaderSettings.EOverrideMode.ApplyNewValue;
			this.groundFogHeightFadeSize = fogHeightFade;
			this.BecomeActiveInstance(true);
		}

		private void ApplyValues()
		{
			if (this.isExported)
			{
				return;
			}
			if (CMSZoneShaderSettings.defaultsInstance == null)
			{
				return;
			}
			if (!this.applyGroundFog)
			{
				this.ApplyColor(CMSZoneShaderSettings.groundFogColor_shaderProp, this.groundFogColor_overrideMode, new Color(0f, 0f, 0f, 0f), CMSZoneShaderSettings.defaultsInstance.groundFogColor);
			}
			else
			{
				this.ApplyColor(CMSZoneShaderSettings.groundFogColor_shaderProp, this.groundFogColor_overrideMode, this.groundFogColor, CMSZoneShaderSettings.defaultsInstance.groundFogColor);
				this.ApplyFloat(CMSZoneShaderSettings.groundFogDepthFadeSq_shaderProp, this.groundFogDepthFade_overrideMode, this.GroundFogDepthFadeSq, CMSZoneShaderSettings.defaultsInstance.GroundFogDepthFadeSq);
				if (this.groundFogHeightPlane != null)
				{
					this.groundFogHeight = this.groundFogHeightPlane.position.y;
				}
				this.ApplyFloat(CMSZoneShaderSettings.groundFogHeight_shaderProp, this.groundFogHeight_overrideMode, this.groundFogHeight, CMSZoneShaderSettings.defaultsInstance.groundFogHeight);
				this.ApplyFloat(CMSZoneShaderSettings.groundFogHeightFade_shaderProp, this.groundFogHeightFade_overrideMode, this.GroundFogHeightFade, CMSZoneShaderSettings.defaultsInstance.GroundFogHeightFade);
			}
			if (!this.applyLiquidEffects)
			{
				this.SetZoneLiquidTypeKeywordEnum(CMSZoneShaderSettings.EZoneLiquidType.None);
				this.SetZoneLiquidShapeKeywordEnum(CMSZoneShaderSettings.ELiquidShape.Plane);
				this.ApplyColor(CMSZoneShaderSettings.shaderParam_GlobalWaterTintColor, this.underwaterTintColor_overrideMode, new Color(0f, 0f, 0f, 0f), CMSZoneShaderSettings.defaultsInstance.underwaterTintColor);
				this.ApplyColor(CMSZoneShaderSettings.shaderParam_GlobalUnderwaterFogColor, this.underwaterFogColor_overrideMode, new Color(0f, 0f, 0f, 0f), CMSZoneShaderSettings.defaultsInstance.underwaterFogColor);
				this.ApplyTexture(CMSZoneShaderSettings.shaderParam_GlobalLiquidResidueTex, this.liquidResidueTex_overrideMode, null, CMSZoneShaderSettings.defaultsInstance.liquidResidueTex);
				Shader.SetGlobalVector(this.shaderParam_GlobalMainWaterSurfacePlane, new Vector4(0f, 1f, 0f, 10000f));
			}
			else
			{
				if (this.zoneLiquidType_overrideMode != CMSZoneShaderSettings.EOverrideMode.LeaveUnchanged || this.isDefaultValues)
				{
					CMSZoneShaderSettings.EZoneLiquidType ezoneLiquidType = (this.zoneLiquidType_overrideMode == CMSZoneShaderSettings.EOverrideMode.ApplyNewValue) ? this.zoneLiquidType : CMSZoneShaderSettings.defaultsInstance.zoneLiquidType;
					if (ezoneLiquidType != CMSZoneShaderSettings.liquidType_previousValue || !CMSZoneShaderSettings.isInitialized)
					{
						this.SetZoneLiquidTypeKeywordEnum(ezoneLiquidType);
						CMSZoneShaderSettings.liquidType_previousValue = ezoneLiquidType;
					}
				}
				Debug.Log("Applying Liquid Shape...");
				if (this.liquidShape_overrideMode != CMSZoneShaderSettings.EOverrideMode.LeaveUnchanged || this.isDefaultValues)
				{
					Debug.Log("Override Mode != LeaveUnchanged");
					CMSZoneShaderSettings.ELiquidShape eliquidShape = (this.liquidShape_overrideMode == CMSZoneShaderSettings.EOverrideMode.ApplyNewValue) ? this.liquidShape : CMSZoneShaderSettings.defaultsInstance.liquidShape;
					if (eliquidShape != CMSZoneShaderSettings.liquidShape_previousValue || !CMSZoneShaderSettings.isInitialized)
					{
						Debug.Log("Set Liquid Shape...");
						this.SetZoneLiquidShapeKeywordEnum(eliquidShape);
						CMSZoneShaderSettings.liquidShape_previousValue = eliquidShape;
					}
					else
					{
						Debug.Log("Same liquid shape AND already initialized");
					}
				}
				this.ApplyFloat(CMSZoneShaderSettings.shaderParam_GlobalZoneLiquidUVScale, this.zoneLiquidUVScale_overrideMode, this.zoneLiquidUVScale, CMSZoneShaderSettings.defaultsInstance.zoneLiquidUVScale);
				this.ApplyColor(CMSZoneShaderSettings.shaderParam_GlobalWaterTintColor, this.underwaterTintColor_overrideMode, this.underwaterTintColor, CMSZoneShaderSettings.defaultsInstance.underwaterTintColor);
				this.ApplyColor(CMSZoneShaderSettings.shaderParam_GlobalUnderwaterFogColor, this.underwaterFogColor_overrideMode, this.underwaterFogColor, CMSZoneShaderSettings.defaultsInstance.underwaterFogColor);
				this.ApplyVector(CMSZoneShaderSettings.shaderParam_GlobalUnderwaterFogParams, this.underwaterFogParams_overrideMode, this.underwaterFogParams, CMSZoneShaderSettings.defaultsInstance.underwaterFogParams);
				this.ApplyVector(CMSZoneShaderSettings.shaderParam_GlobalUnderwaterCausticsParams, this.underwaterCausticsParams_overrideMode, this.underwaterCausticsParams, CMSZoneShaderSettings.defaultsInstance.underwaterCausticsParams);
				this.ApplyTexture(CMSZoneShaderSettings.shaderParam_GlobalUnderwaterCausticsTex, this.underwaterCausticsTexture_overrideMode, this.underwaterCausticsTexture, CMSZoneShaderSettings.defaultsInstance.underwaterCausticsTexture);
				this.ApplyVector(CMSZoneShaderSettings.shaderParam_GlobalUnderwaterEffectsDistanceToSurfaceFade, this.underwaterEffectsDistanceToSurfaceFade_overrideMode, this.underwaterEffectsDistanceToSurfaceFade, CMSZoneShaderSettings.defaultsInstance.underwaterEffectsDistanceToSurfaceFade);
				this.ApplyTexture(CMSZoneShaderSettings.shaderParam_GlobalLiquidResidueTex, this.liquidResidueTex_overrideMode, this.liquidResidueTex, CMSZoneShaderSettings.defaultsInstance.liquidResidueTex);
				this.ApplyFloat(CMSZoneShaderSettings.shaderParam_ZoneWeatherMapDissolveProgress, this.zoneWeatherMapDissolveProgress_overrideMode, this.zoneWeatherMapDissolveProgress, CMSZoneShaderSettings.defaultsInstance.zoneWeatherMapDissolveProgress);
				this.UpdateMainPlaneShaderProperty();
			}
			CMSZoneShaderSettings.isInitialized = true;
		}

		public void RefreshValues()
		{
			if (this.isExported)
			{
				return;
			}
			if (this.mainWaterSurfacePlane == null)
			{
				this.hasMainWaterSurfacePlane = false;
				this.hasDynamicWaterSurfacePlane = false;
			}
			else
			{
				this.hasMainWaterSurfacePlane = (this.mainWaterSurfacePlane_overrideMode == CMSZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues);
				this.hasDynamicWaterSurfacePlane = (this.hasMainWaterSurfacePlane && !this.mainWaterSurfacePlane.gameObject.isStatic);
			}
			this.hasLiquidBottomTransform = (this.liquidBottomTransform != null && (this.liquidBottomTransform_overrideMode == CMSZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues));
		}

		private void ApplyColor(int shaderProp, CMSZoneShaderSettings.EOverrideMode overrideMode, Color value, Color defaultValue)
		{
			if (this.isExported)
			{
				return;
			}
			if (overrideMode == CMSZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalColor(shaderProp, value.linear);
				return;
			}
			if (overrideMode == CMSZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalColor(shaderProp, defaultValue.linear);
			}
		}

		private void ApplyFloat(int shaderProp, CMSZoneShaderSettings.EOverrideMode overrideMode, float value, float defaultValue)
		{
			if (this.isExported)
			{
				return;
			}
			if (overrideMode == CMSZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalFloat(shaderProp, value);
				return;
			}
			if (overrideMode == CMSZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalFloat(shaderProp, defaultValue);
			}
		}

		private void ApplyVector(int shaderProp, CMSZoneShaderSettings.EOverrideMode overrideMode, Vector2 value, Vector2 defaultValue)
		{
			if (this.isExported)
			{
				return;
			}
			if (overrideMode == CMSZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalVector(shaderProp, value);
				return;
			}
			if (overrideMode == CMSZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalVector(shaderProp, defaultValue);
			}
		}

		private void ApplyVector(int shaderProp, CMSZoneShaderSettings.EOverrideMode overrideMode, Vector3 value, Vector3 defaultValue)
		{
			if (this.isExported)
			{
				return;
			}
			if (overrideMode == CMSZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalVector(shaderProp, value);
				return;
			}
			if (overrideMode == CMSZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalVector(shaderProp, defaultValue);
			}
		}

		private void ApplyVector(int shaderProp, CMSZoneShaderSettings.EOverrideMode overrideMode, Vector4 value, Vector4 defaultValue)
		{
			if (this.isExported)
			{
				return;
			}
			if (overrideMode == CMSZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalVector(shaderProp, value);
				return;
			}
			if (overrideMode == CMSZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalVector(shaderProp, defaultValue);
			}
		}

		public void ApplyTexture(int shaderProp, CMSZoneShaderSettings.EOverrideMode overrideMode, Texture2D value, Texture2D defaultValue)
		{
			if (this.isExported)
			{
				return;
			}
			if (overrideMode == CMSZoneShaderSettings.EOverrideMode.ApplyNewValue || this.isDefaultValues)
			{
				Shader.SetGlobalTexture(shaderProp, value);
				return;
			}
			if (overrideMode == CMSZoneShaderSettings.EOverrideMode.ApplyDefaultValue)
			{
				Shader.SetGlobalTexture(shaderProp, defaultValue);
			}
		}

		public CMSZoneShaderSettings.CMSZoneShaderProperties GetProperties()
		{
			return new CMSZoneShaderSettings.CMSZoneShaderProperties
			{
				groundFogColor = this.groundFogColor,
				groundFogDepthFadeSize = this.groundFogDepthFadeSize,
				groundFogHeightPlane = this.groundFogHeightPlane,
				groundFogHeight = this.groundFogHeight,
				groundFogHeightFadeSize = this.groundFogDepthFadeSize,
				zoneLiquidType = this.GetZoneLiquidType(),
				liquidShape = this.GetZoneLiquidShape(),
				liquidShapeRadius = this.liquidShapeRadius,
				liquidBottomTransform = this.liquidBottomTransform,
				zoneLiquidUVScale = this.zoneLiquidUVScale,
				underwaterTintColor = this.underwaterTintColor,
				underwaterFogColor = this.underwaterFogColor,
				underwaterFogParams = this.underwaterFogParams,
				underwaterCausticsParams = this.underwaterCausticsParams,
				underwaterCausticsTexture = this.underwaterCausticsTexture,
				underwaterEffectsDistanceToSurfaceFade = this.underwaterEffectsDistanceToSurfaceFade,
				liquidResidueTex = this.liquidResidueTex,
				mainWaterSurfacePlane = this.mainWaterSurfacePlane,
				zoneWeatherMapDissolveProgress = this.zoneWeatherMapDissolveProgress,
				isInitialized = true
			};
		}

		[Nullable(new byte[]
		{
			2,
			1
		})]
		[NonSerialized]
		public Collider[] edZoneColliders;

		[NonSerialized]
		public bool edWasInitialized;

		public static bool isInitialized;

		public static CMSZoneShaderSettings defaultsInstance;

		public static bool hasDefaultsInstance;

		public static CMSZoneShaderSettings activeInstance;

		public static bool hasActiveInstance;

		public bool isExported;

		[Tooltip("Set this to true for cases like it is the first CMSZoneShaderSettings that should be activated when a scene is loaded.")]
		public bool activateOnLoad;

		[Tooltip("These values will be used as the default global values that will be fallen back to when not in a zone and that the other scripts will reference.")]
		public bool isDefaultValues;

		public bool applyGroundFog;

		private static readonly int groundFogColor_shaderProp = Shader.PropertyToID("_ZoneGroundFogColor");

		public CMSZoneShaderSettings.EOverrideMode groundFogColor_overrideMode;

		public Color groundFogColor = new Color(0.7f, 0.9f, 1f, 1f);

		private static readonly int groundFogDepthFadeSq_shaderProp = Shader.PropertyToID("_ZoneGroundFogDepthFadeSq");

		public CMSZoneShaderSettings.EOverrideMode groundFogDepthFade_overrideMode;

		public float groundFogDepthFadeSize = 20f;

		private static readonly int groundFogHeight_shaderProp = Shader.PropertyToID("_ZoneGroundFogHeight");

		public CMSZoneShaderSettings.EOverrideMode groundFogHeight_overrideMode;

		public Transform groundFogHeightPlane;

		public float groundFogHeight = 7.45f;

		private static readonly int groundFogHeightFade_shaderProp = Shader.PropertyToID("_ZoneGroundFogHeightFade");

		public CMSZoneShaderSettings.EOverrideMode groundFogHeightFade_overrideMode;

		public float groundFogHeightFadeSize = 20f;

		public bool applyLiquidEffects;

		public CMSZoneShaderSettings.EOverrideMode zoneLiquidType_overrideMode;

		public CMSZoneShaderSettings.EZoneLiquidType zoneLiquidType = CMSZoneShaderSettings.EZoneLiquidType.Water;

		private static CMSZoneShaderSettings.EZoneLiquidType liquidType_previousValue = CMSZoneShaderSettings.EZoneLiquidType.None;

		public CMSZoneShaderSettings.EOverrideMode liquidShape_overrideMode;

		public CMSZoneShaderSettings.ELiquidShape liquidShape;

		private static CMSZoneShaderSettings.ELiquidShape liquidShape_previousValue = CMSZoneShaderSettings.ELiquidShape.Plane;

		public CMSZoneShaderSettings.EOverrideMode liquidShapeRadius_overrideMode;

		public float liquidShapeRadius = 1f;

		private static float liquidShapeRadius_previousValue;

		private bool hasLiquidBottomTransform;

		public CMSZoneShaderSettings.EOverrideMode liquidBottomTransform_overrideMode;

		public Transform liquidBottomTransform;

		private float liquidBottomPosY_previousValue;

		private static readonly int shaderParam_GlobalZoneLiquidUVScale = Shader.PropertyToID("_GlobalZoneLiquidUVScale");

		public CMSZoneShaderSettings.EOverrideMode zoneLiquidUVScale_overrideMode;

		public float zoneLiquidUVScale = 0.01f;

		private static readonly int shaderParam_GlobalWaterTintColor = Shader.PropertyToID("_GlobalWaterTintColor");

		public CMSZoneShaderSettings.EOverrideMode underwaterTintColor_overrideMode;

		public Color underwaterTintColor = new Color(0.3f, 0.65f, 1f, 0.2f);

		private static readonly int shaderParam_GlobalUnderwaterFogColor = Shader.PropertyToID("_GlobalUnderwaterFogColor");

		public CMSZoneShaderSettings.EOverrideMode underwaterFogColor_overrideMode;

		public Color underwaterFogColor = new Color(0.12f, 0.41f, 0.77f);

		private static readonly int shaderParam_GlobalUnderwaterFogParams = Shader.PropertyToID("_GlobalUnderwaterFogParams");

		public CMSZoneShaderSettings.EOverrideMode underwaterFogParams_overrideMode;

		public float underwaterFogStart = -5f;

		public float underwaterFogDistance = 40f;

		[Tooltip("Fog params are: start, distance (end - start), unused, unused")]
		public Vector4 underwaterFogParams = new Vector4(-5f, 40f, 0f, 0f);

		private static readonly int shaderParam_GlobalUnderwaterCausticsParams = Shader.PropertyToID("_GlobalUnderwaterCausticsParams");

		public CMSZoneShaderSettings.EOverrideMode underwaterCausticsParams_overrideMode;

		public float underwaterCausticsSpeed = 0.075f;

		public float underwaterCausticsScale = 0.075f;

		[Tooltip("Caustics params are: Speed, Scale, Alpha, unused")]
		public Vector4 underwaterCausticsParams = new Vector4(0.075f, 0.075f, 1f, 0f);

		public static readonly int shaderParam_GlobalUnderwaterCausticsTex = Shader.PropertyToID("_GlobalUnderwaterCausticsTex");

		public CMSZoneShaderSettings.EOverrideMode underwaterCausticsTexture_overrideMode;

		public CMSZoneShaderSettings.ETextureOverrideType underwaterCausticsTextureOverrideType;

		public Texture2D underwaterCausticsTexture;

		private static readonly int shaderParam_GlobalUnderwaterEffectsDistanceToSurfaceFade = Shader.PropertyToID("_GlobalUnderwaterEffectsDistanceToSurfaceFade");

		public CMSZoneShaderSettings.EOverrideMode underwaterEffectsDistanceToSurfaceFade_overrideMode;

		[Range(0.0001f, 50f)]
		public float underwaterFogDistanceToSurfaceFadeMinimum = 0.0001f;

		[Range(0.0001f, 50f)]
		public float underwaterFogDistanceToSurfaceFadeMaximum = 50f;

		public Vector2 underwaterEffectsDistanceToSurfaceFade = new Vector2(0.0001f, 50f);

		[Nullable(1)]
		private const string kEdTooltip_liquidResidueTex = "This is used for things like the charred surface effect when lava burns static geo.";

		public static readonly int shaderParam_GlobalLiquidResidueTex = Shader.PropertyToID("_GlobalLiquidResidueTex");

		[Tooltip("This is used for things like the charred surface effect when lava burns static geo.")]
		public CMSZoneShaderSettings.EOverrideMode liquidResidueTex_overrideMode;

		public CMSZoneShaderSettings.ETextureOverrideType liquidResidueTextureOverrideType;

		[Tooltip("This is used for things like the charred surface effect when lava burns static geo.")]
		public Texture2D liquidResidueTex;

		private readonly int shaderParam_GlobalMainWaterSurfacePlane = Shader.PropertyToID("_GlobalMainWaterSurfacePlane");

		public bool hasMainWaterSurfacePlane;

		public bool hasDynamicWaterSurfacePlane;

		public CMSZoneShaderSettings.EOverrideMode mainWaterSurfacePlane_overrideMode;

		public Transform mainWaterSurfacePlane;

		private static readonly int shaderParam_ZoneWeatherMapDissolveProgress = Shader.PropertyToID("_ZoneWeatherMapDissolveProgress");

		public CMSZoneShaderSettings.EOverrideMode zoneWeatherMapDissolveProgress_overrideMode;

		[Range(0f, 1f)]
		public float zoneWeatherMapDissolveProgress = 1f;

		[NullableContext(0)]
		public enum EOverrideMode
		{
			LeaveUnchanged,
			ApplyNewValue,
			ApplyDefaultValue
		}

		[NullableContext(0)]
		public enum ETextureOverrideType
		{
			Default,
			Custom
		}

		[Nullable(0)]
		public struct CMSZoneShaderProperties
		{
			public float GroundFogHeightFade
			{
				get
				{
					return 1f / Mathf.Max(1E-05f, this.groundFogHeightFadeSize);
				}
			}

			public bool isInitialized;

			public Color groundFogColor;

			public float groundFogDepthFadeSize;

			public Transform groundFogHeightPlane;

			public float groundFogHeight;

			public float groundFogHeightFadeSize;

			public int zoneLiquidType;

			public int liquidShape;

			public float liquidShapeRadius;

			public Transform liquidBottomTransform;

			public float zoneLiquidUVScale;

			public Color underwaterTintColor;

			public Color underwaterFogColor;

			public Vector4 underwaterFogParams;

			public Vector4 underwaterCausticsParams;

			public Texture2D underwaterCausticsTexture;

			public Vector2 underwaterEffectsDistanceToSurfaceFade;

			public Texture2D liquidResidueTex;

			public Transform mainWaterSurfacePlane;

			public float zoneWeatherMapDissolveProgress;
		}

		[NullableContext(0)]
		public enum EZoneLiquidType
		{
			None,
			Water,
			Lava
		}

		[NullableContext(0)]
		public enum ELiquidShape
		{
			Plane,
			Cylinder
		}
	}
}
