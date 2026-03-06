using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TextCore;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;

namespace TMPro
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(CanvasRenderer))]
	[AddComponentMenu("UI/TextMeshPro - Text (UI)", 11)]
	[ExecuteAlways]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/TextMeshPro/index.html")]
	public class TextMeshProUGUI : TMP_Text, ILayoutElement
	{
		public override Material materialForRendering
		{
			get
			{
				return TMP_MaterialManager.GetMaterialForRendering(this, this.m_sharedMaterial);
			}
		}

		public override bool autoSizeTextContainer
		{
			get
			{
				return this.m_autoSizeTextContainer;
			}
			set
			{
				if (this.m_autoSizeTextContainer == value)
				{
					return;
				}
				this.m_autoSizeTextContainer = value;
				if (this.m_autoSizeTextContainer)
				{
					CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
					this.SetLayoutDirty();
				}
			}
		}

		public override Mesh mesh
		{
			get
			{
				return this.m_mesh;
			}
		}

		public new CanvasRenderer canvasRenderer
		{
			get
			{
				if (this.m_canvasRenderer == null)
				{
					this.m_canvasRenderer = base.GetComponent<CanvasRenderer>();
				}
				return this.m_canvasRenderer;
			}
		}

		public void CalculateLayoutInputHorizontal()
		{
		}

		public void CalculateLayoutInputVertical()
		{
		}

		public override void SetVerticesDirty()
		{
			if (this == null || !this.IsActive())
			{
				return;
			}
			if (CanvasUpdateRegistry.IsRebuildingGraphics())
			{
				return;
			}
			CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
			if (this.m_OnDirtyVertsCallback != null)
			{
				this.m_OnDirtyVertsCallback();
			}
		}

		public override void SetLayoutDirty()
		{
			this.m_isPreferredWidthDirty = true;
			this.m_isPreferredHeightDirty = true;
			if (this == null || !this.IsActive())
			{
				return;
			}
			LayoutRebuilder.MarkLayoutForRebuild(base.rectTransform);
			this.m_isLayoutDirty = true;
			if (this.m_OnDirtyLayoutCallback != null)
			{
				this.m_OnDirtyLayoutCallback();
			}
		}

		public override void SetMaterialDirty()
		{
			if (this == null || !this.IsActive())
			{
				return;
			}
			if (CanvasUpdateRegistry.IsRebuildingGraphics())
			{
				return;
			}
			this.m_isMaterialDirty = true;
			CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
			if (this.m_OnDirtyMaterialCallback != null)
			{
				this.m_OnDirtyMaterialCallback();
			}
		}

		public override void SetAllDirty()
		{
			this.SetLayoutDirty();
			this.SetVerticesDirty();
			this.SetMaterialDirty();
		}

		private IEnumerator DelayedGraphicRebuild()
		{
			yield return null;
			CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
			if (this.m_OnDirtyVertsCallback != null)
			{
				this.m_OnDirtyVertsCallback();
			}
			this.m_DelayedGraphicRebuild = null;
			yield break;
		}

		private IEnumerator DelayedMaterialRebuild()
		{
			yield return null;
			this.m_isMaterialDirty = true;
			CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
			if (this.m_OnDirtyMaterialCallback != null)
			{
				this.m_OnDirtyMaterialCallback();
			}
			this.m_DelayedMaterialRebuild = null;
			yield break;
		}

		public override void Rebuild(CanvasUpdate update)
		{
			if (this == null)
			{
				return;
			}
			if (update == CanvasUpdate.Prelayout)
			{
				if (this.m_autoSizeTextContainer)
				{
					this.m_rectTransform.sizeDelta = base.GetPreferredValues(float.PositiveInfinity, float.PositiveInfinity);
					return;
				}
			}
			else if (update == CanvasUpdate.PreRender)
			{
				this.OnPreRenderCanvas();
				if (!this.m_isMaterialDirty)
				{
					return;
				}
				this.UpdateMaterial();
				this.m_isMaterialDirty = false;
			}
		}

		private void UpdateSubObjectPivot()
		{
			if (this.m_textInfo == null)
			{
				return;
			}
			int num = 1;
			while (num < this.m_subTextObjects.Length && this.m_subTextObjects[num] != null)
			{
				this.m_subTextObjects[num].SetPivotDirty();
				num++;
			}
		}

		public override Material GetModifiedMaterial(Material baseMaterial)
		{
			Material material = baseMaterial;
			if (this.m_ShouldRecalculateStencil)
			{
				Transform stopAfter = MaskUtilities.FindRootSortOverrideCanvas(base.transform);
				this.m_StencilValue = (base.maskable ? MaskUtilities.GetStencilDepth(base.transform, stopAfter) : 0);
				this.m_ShouldRecalculateStencil = false;
			}
			if (this.m_StencilValue > 0)
			{
				Material maskMaterial = StencilMaterial.Add(material, (1 << this.m_StencilValue) - 1, StencilOp.Keep, CompareFunction.Equal, ColorWriteMask.All, (1 << this.m_StencilValue) - 1, 0);
				StencilMaterial.Remove(this.m_MaskMaterial);
				this.m_MaskMaterial = maskMaterial;
				material = this.m_MaskMaterial;
			}
			return material;
		}

		protected override void UpdateMaterial()
		{
			if (this.m_sharedMaterial == null || this.canvasRenderer == null)
			{
				return;
			}
			this.m_canvasRenderer.materialCount = 1;
			this.m_canvasRenderer.SetMaterial(this.materialForRendering, 0);
		}

		public Vector4 maskOffset
		{
			get
			{
				return this.m_maskOffset;
			}
			set
			{
				this.m_maskOffset = value;
				this.UpdateMask();
				this.m_havePropertiesChanged = true;
			}
		}

		public override void RecalculateClipping()
		{
			base.RecalculateClipping();
		}

		public override void Cull(Rect clipRect, bool validRect)
		{
			this.m_ShouldUpdateCulling = false;
			if (this.m_isLayoutDirty)
			{
				this.m_ShouldUpdateCulling = true;
				this.m_ClipRect = clipRect;
				this.m_ValidRect = validRect;
				return;
			}
			Rect canvasSpaceClippingRect = this.GetCanvasSpaceClippingRect();
			bool flag = !validRect || !clipRect.Overlaps(canvasSpaceClippingRect, true);
			if (this.m_canvasRenderer.cull != flag)
			{
				this.m_canvasRenderer.cull = flag;
				base.onCullStateChanged.Invoke(flag);
				this.OnCullingChanged();
				int num = 1;
				while (num < this.m_subTextObjects.Length && this.m_subTextObjects[num] != null)
				{
					this.m_subTextObjects[num].canvasRenderer.cull = flag;
					num++;
				}
			}
		}

		internal override void UpdateCulling()
		{
			Rect canvasSpaceClippingRect = this.GetCanvasSpaceClippingRect();
			bool flag = !this.m_ValidRect || !this.m_ClipRect.Overlaps(canvasSpaceClippingRect, true);
			if (this.m_canvasRenderer.cull != flag)
			{
				this.m_canvasRenderer.cull = flag;
				base.onCullStateChanged.Invoke(flag);
				this.OnCullingChanged();
				int num = 1;
				while (num < this.m_subTextObjects.Length && this.m_subTextObjects[num] != null)
				{
					this.m_subTextObjects[num].canvasRenderer.cull = flag;
					num++;
				}
			}
			this.m_ShouldUpdateCulling = false;
		}

		public override void UpdateMeshPadding()
		{
			this.m_padding = ShaderUtilities.GetPadding(this.m_sharedMaterial, this.m_enableExtraPadding, this.m_isUsingBold);
			this.m_isMaskingEnabled = ShaderUtilities.IsMaskingEnabled(this.m_sharedMaterial);
			this.m_havePropertiesChanged = true;
			this.checkPaddingRequired = false;
			if (this.m_textInfo == null)
			{
				return;
			}
			for (int i = 1; i < this.m_textInfo.materialCount; i++)
			{
				this.m_subTextObjects[i].UpdateMeshPadding(this.m_enableExtraPadding, this.m_isUsingBold);
			}
		}

		protected override void InternalCrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha)
		{
			if (this.m_textInfo == null)
			{
				return;
			}
			int materialCount = this.m_textInfo.materialCount;
			for (int i = 1; i < materialCount; i++)
			{
				this.m_subTextObjects[i].CrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha);
			}
		}

		protected override void InternalCrossFadeAlpha(float alpha, float duration, bool ignoreTimeScale)
		{
			if (this.m_textInfo == null)
			{
				return;
			}
			int materialCount = this.m_textInfo.materialCount;
			for (int i = 1; i < materialCount; i++)
			{
				this.m_subTextObjects[i].CrossFadeAlpha(alpha, duration, ignoreTimeScale);
			}
		}

		public override void ForceMeshUpdate(bool ignoreActiveState = false, bool forceTextReparsing = false)
		{
			this.m_havePropertiesChanged = true;
			this.m_ignoreActiveState = ignoreActiveState;
			if (this.m_canvas == null)
			{
				this.m_canvas = base.GetComponentInParent<Canvas>();
			}
			this.OnPreRenderCanvas();
		}

		public override TMP_TextInfo GetTextInfo(string text)
		{
			base.SetText(text);
			this.SetArraySizes(this.m_TextProcessingArray);
			this.m_renderMode = TextRenderFlags.DontRender;
			this.ComputeMarginSize();
			if (this.m_canvas == null)
			{
				this.m_canvas = base.canvas;
			}
			this.GenerateTextMesh();
			this.m_renderMode = TextRenderFlags.Render;
			return base.textInfo;
		}

		public override void ClearMesh()
		{
			this.m_canvasRenderer.SetMesh(null);
			int num = 1;
			while (num < this.m_subTextObjects.Length && this.m_subTextObjects[num] != null)
			{
				this.m_subTextObjects[num].canvasRenderer.SetMesh(null);
				num++;
			}
		}

		public override event Action<TMP_TextInfo> OnPreRenderText;

		public override void UpdateGeometry(Mesh mesh, int index)
		{
			mesh.RecalculateBounds();
			if (index == 0)
			{
				this.m_canvasRenderer.SetMesh(mesh);
				return;
			}
			this.m_subTextObjects[index].canvasRenderer.SetMesh(mesh);
		}

		public override void UpdateVertexData(TMP_VertexDataUpdateFlags flags)
		{
			int materialCount = this.m_textInfo.materialCount;
			for (int i = 0; i < materialCount; i++)
			{
				Mesh mesh;
				if (i == 0)
				{
					mesh = this.m_mesh;
				}
				else
				{
					mesh = this.m_subTextObjects[i].mesh;
				}
				if ((flags & TMP_VertexDataUpdateFlags.Vertices) == TMP_VertexDataUpdateFlags.Vertices)
				{
					mesh.vertices = this.m_textInfo.meshInfo[i].vertices;
				}
				if ((flags & TMP_VertexDataUpdateFlags.Uv0) == TMP_VertexDataUpdateFlags.Uv0)
				{
					mesh.SetUVs(0, this.m_textInfo.meshInfo[i].uvs0);
				}
				if ((flags & TMP_VertexDataUpdateFlags.Uv2) == TMP_VertexDataUpdateFlags.Uv2)
				{
					mesh.uv2 = this.m_textInfo.meshInfo[i].uvs2;
				}
				if ((flags & TMP_VertexDataUpdateFlags.Colors32) == TMP_VertexDataUpdateFlags.Colors32)
				{
					mesh.colors32 = this.m_textInfo.meshInfo[i].colors32;
				}
				mesh.RecalculateBounds();
				if (i == 0)
				{
					this.m_canvasRenderer.SetMesh(mesh);
				}
				else
				{
					this.m_subTextObjects[i].canvasRenderer.SetMesh(mesh);
				}
			}
		}

		public override void UpdateVertexData()
		{
			int materialCount = this.m_textInfo.materialCount;
			for (int i = 0; i < materialCount; i++)
			{
				Mesh mesh;
				if (i == 0)
				{
					mesh = this.m_mesh;
				}
				else
				{
					this.m_textInfo.meshInfo[i].ClearUnusedVertices();
					mesh = this.m_subTextObjects[i].mesh;
				}
				mesh.vertices = this.m_textInfo.meshInfo[i].vertices;
				mesh.SetUVs(0, this.m_textInfo.meshInfo[i].uvs0);
				mesh.uv2 = this.m_textInfo.meshInfo[i].uvs2;
				mesh.colors32 = this.m_textInfo.meshInfo[i].colors32;
				mesh.RecalculateBounds();
				if (i == 0)
				{
					this.m_canvasRenderer.SetMesh(mesh);
				}
				else
				{
					this.m_subTextObjects[i].canvasRenderer.SetMesh(mesh);
				}
			}
		}

		public void UpdateFontAsset()
		{
			this.LoadFontAsset();
		}

		protected override void Awake()
		{
			this.m_canvas = base.canvas;
			this.m_isOrthographic = true;
			this.m_rectTransform = base.gameObject.GetComponent<RectTransform>();
			if (this.m_rectTransform == null)
			{
				this.m_rectTransform = base.gameObject.AddComponent<RectTransform>();
			}
			this.m_canvasRenderer = base.GetComponent<CanvasRenderer>();
			if (this.m_canvasRenderer == null)
			{
				this.m_canvasRenderer = base.gameObject.AddComponent<CanvasRenderer>();
			}
			if (this.m_mesh == null)
			{
				this.m_mesh = new Mesh();
				this.m_mesh.hideFlags = HideFlags.HideAndDontSave;
				this.m_textInfo = new TMP_TextInfo(this);
			}
			base.LoadDefaultSettings();
			this.LoadFontAsset();
			if (this.m_TextProcessingArray == null)
			{
				this.m_TextProcessingArray = new TMP_Text.TextProcessingElement[this.m_max_characters];
			}
			this.m_cached_TextElement = new TMP_Character();
			this.m_isFirstAllocation = true;
			this.m_havePropertiesChanged = true;
			this.m_isAwake = true;
		}

		protected override void OnEnable()
		{
			if (!this.m_isAwake)
			{
				return;
			}
			if (!this.m_isRegisteredForEvents)
			{
				this.m_isRegisteredForEvents = true;
			}
			this.m_canvas = this.GetCanvas();
			this.SetActiveSubMeshes(true);
			GraphicRegistry.RegisterGraphicForCanvas(this.m_canvas, this);
			if (!this.m_IsTextObjectScaleStatic)
			{
				TMP_UpdateManager.RegisterTextObjectForUpdate(this);
			}
			this.ComputeMarginSize();
			this.SetAllDirty();
			this.RecalculateClipping();
			this.RecalculateMasking();
		}

		protected override void OnDisable()
		{
			if (!this.m_isAwake)
			{
				return;
			}
			GraphicRegistry.UnregisterGraphicForCanvas(this.m_canvas, this);
			CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
			TMP_UpdateManager.UnRegisterTextObjectForUpdate(this);
			if (this.m_canvasRenderer != null)
			{
				this.m_canvasRenderer.Clear();
			}
			this.SetActiveSubMeshes(false);
			LayoutRebuilder.MarkLayoutForRebuild(this.m_rectTransform);
			this.RecalculateClipping();
			this.RecalculateMasking();
		}

		protected override void OnDestroy()
		{
			GraphicRegistry.UnregisterGraphicForCanvas(this.m_canvas, this);
			TMP_UpdateManager.UnRegisterTextObjectForUpdate(this);
			if (this.m_mesh != null)
			{
				Object.DestroyImmediate(this.m_mesh);
			}
			if (this.m_MaskMaterial != null)
			{
				TMP_MaterialManager.ReleaseStencilMaterial(this.m_MaskMaterial);
				this.m_MaskMaterial = null;
			}
			this.m_isRegisteredForEvents = false;
		}

		protected override void LoadFontAsset()
		{
			ShaderUtilities.GetShaderPropertyIDs();
			if (this.m_fontAsset == null)
			{
				if (TMP_Settings.defaultFontAsset != null)
				{
					this.m_fontAsset = TMP_Settings.defaultFontAsset;
				}
				if (this.m_fontAsset == null)
				{
					Debug.LogWarning("The LiberationSans SDF Font Asset was not found. There is no Font Asset assigned to " + base.gameObject.name + ".", this);
					return;
				}
				if (this.m_fontAsset.characterLookupTable == null)
				{
					Debug.Log("Dictionary is Null!");
				}
				this.m_sharedMaterial = this.m_fontAsset.material;
			}
			else
			{
				if (this.m_fontAsset.characterLookupTable == null)
				{
					this.m_fontAsset.ReadFontAssetDefinition();
				}
				if (this.m_sharedMaterial == null && this.m_baseMaterial != null)
				{
					this.m_sharedMaterial = this.m_baseMaterial;
					this.m_baseMaterial = null;
				}
				if (this.m_sharedMaterial == null || this.m_sharedMaterial.GetTexture(ShaderUtilities.ID_MainTex) == null || this.m_fontAsset.atlasTexture.GetInstanceID() != this.m_sharedMaterial.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID())
				{
					if (this.m_fontAsset.material == null)
					{
						Debug.LogWarning(string.Concat(new string[]
						{
							"The Font Atlas Texture of the Font Asset ",
							this.m_fontAsset.name,
							" assigned to ",
							base.gameObject.name,
							" is missing."
						}), this);
					}
					else
					{
						this.m_sharedMaterial = this.m_fontAsset.material;
					}
				}
			}
			this.ValidateEnvMapProperty();
			base.GetSpecialCharacters(this.m_fontAsset);
			this.m_padding = this.GetPaddingForMaterial();
			this.SetMaterialDirty();
		}

		private Canvas GetCanvas()
		{
			Canvas result = null;
			List<Canvas> list = TMP_ListPool<Canvas>.Get();
			base.gameObject.GetComponentsInParent<Canvas>(false, list);
			if (list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].isActiveAndEnabled)
					{
						result = list[i];
						break;
					}
				}
			}
			TMP_ListPool<Canvas>.Release(list);
			return result;
		}

		private void ValidateEnvMapProperty()
		{
			if (this.m_sharedMaterial != null)
			{
				this.m_hasEnvMapProperty = (this.m_sharedMaterial.HasProperty(ShaderUtilities.ID_EnvMap) && this.m_sharedMaterial.GetTexture(ShaderUtilities.ID_EnvMap) != null);
				return;
			}
			this.m_hasEnvMapProperty = false;
		}

		private void UpdateEnvMapMatrix()
		{
			if (!this.m_hasEnvMapProperty)
			{
				return;
			}
			Vector3 vector = this.m_sharedMaterial.GetVector(ShaderUtilities.ID_EnvMatrixRotation);
			if (this.m_currentEnvMapRotation == vector)
			{
				return;
			}
			this.m_currentEnvMapRotation = vector;
			this.m_EnvMapMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(this.m_currentEnvMapRotation), Vector3.one);
			this.m_sharedMaterial.SetMatrix(ShaderUtilities.ID_EnvMatrix, this.m_EnvMapMatrix);
		}

		private void EnableMasking()
		{
			if (this.m_fontMaterial == null)
			{
				this.m_fontMaterial = this.CreateMaterialInstance(this.m_sharedMaterial);
				this.m_canvasRenderer.SetMaterial(this.m_fontMaterial, this.m_sharedMaterial.GetTexture(ShaderUtilities.ID_MainTex));
			}
			this.m_sharedMaterial = this.m_fontMaterial;
			if (this.m_sharedMaterial.HasProperty(ShaderUtilities.ID_ClipRect))
			{
				this.m_sharedMaterial.EnableKeyword(ShaderUtilities.Keyword_MASK_SOFT);
				this.m_sharedMaterial.DisableKeyword(ShaderUtilities.Keyword_MASK_HARD);
				this.m_sharedMaterial.DisableKeyword(ShaderUtilities.Keyword_MASK_TEX);
				this.UpdateMask();
			}
			this.m_isMaskingEnabled = true;
		}

		private void DisableMasking()
		{
		}

		private void UpdateMask()
		{
			if (this.m_rectTransform != null)
			{
				if (!ShaderUtilities.isInitialized)
				{
					ShaderUtilities.GetShaderPropertyIDs();
				}
				this.m_isScrollRegionSet = true;
				float num = Mathf.Min(Mathf.Min(this.m_margin.x, this.m_margin.z), this.m_sharedMaterial.GetFloat(ShaderUtilities.ID_MaskSoftnessX));
				float num2 = Mathf.Min(Mathf.Min(this.m_margin.y, this.m_margin.w), this.m_sharedMaterial.GetFloat(ShaderUtilities.ID_MaskSoftnessY));
				num = ((num > 0f) ? num : 0f);
				num2 = ((num2 > 0f) ? num2 : 0f);
				float z = (this.m_rectTransform.rect.width - Mathf.Max(this.m_margin.x, 0f) - Mathf.Max(this.m_margin.z, 0f)) / 2f + num;
				float w = (this.m_rectTransform.rect.height - Mathf.Max(this.m_margin.y, 0f) - Mathf.Max(this.m_margin.w, 0f)) / 2f + num2;
				Vector2 vector = this.m_rectTransform.localPosition + new Vector3((0.5f - this.m_rectTransform.pivot.x) * this.m_rectTransform.rect.width + (Mathf.Max(this.m_margin.x, 0f) - Mathf.Max(this.m_margin.z, 0f)) / 2f, (0.5f - this.m_rectTransform.pivot.y) * this.m_rectTransform.rect.height + (-Mathf.Max(this.m_margin.y, 0f) + Mathf.Max(this.m_margin.w, 0f)) / 2f);
				Vector4 value = new Vector4(vector.x, vector.y, z, w);
				this.m_sharedMaterial.SetVector(ShaderUtilities.ID_ClipRect, value);
			}
		}

		protected override Material GetMaterial(Material mat)
		{
			ShaderUtilities.GetShaderPropertyIDs();
			if (this.m_fontMaterial == null || this.m_fontMaterial.GetInstanceID() != mat.GetInstanceID())
			{
				this.m_fontMaterial = this.CreateMaterialInstance(mat);
			}
			this.m_sharedMaterial = this.m_fontMaterial;
			this.m_padding = this.GetPaddingForMaterial();
			this.m_ShouldRecalculateStencil = true;
			this.SetVerticesDirty();
			this.SetMaterialDirty();
			return this.m_sharedMaterial;
		}

		protected override Material[] GetMaterials(Material[] mats)
		{
			int materialCount = this.m_textInfo.materialCount;
			if (this.m_fontMaterials == null)
			{
				this.m_fontMaterials = new Material[materialCount];
			}
			else if (this.m_fontMaterials.Length != materialCount)
			{
				TMP_TextInfo.Resize<Material>(ref this.m_fontMaterials, materialCount, false);
			}
			for (int i = 0; i < materialCount; i++)
			{
				if (i == 0)
				{
					this.m_fontMaterials[i] = base.fontMaterial;
				}
				else
				{
					this.m_fontMaterials[i] = this.m_subTextObjects[i].material;
				}
			}
			this.m_fontSharedMaterials = this.m_fontMaterials;
			return this.m_fontMaterials;
		}

		protected override void SetSharedMaterial(Material mat)
		{
			this.m_sharedMaterial = mat;
			this.m_padding = this.GetPaddingForMaterial();
			this.SetMaterialDirty();
		}

		protected override Material[] GetSharedMaterials()
		{
			int materialCount = this.m_textInfo.materialCount;
			if (this.m_fontSharedMaterials == null)
			{
				this.m_fontSharedMaterials = new Material[materialCount];
			}
			else if (this.m_fontSharedMaterials.Length != materialCount)
			{
				TMP_TextInfo.Resize<Material>(ref this.m_fontSharedMaterials, materialCount, false);
			}
			for (int i = 0; i < materialCount; i++)
			{
				if (i == 0)
				{
					this.m_fontSharedMaterials[i] = this.m_sharedMaterial;
				}
				else
				{
					this.m_fontSharedMaterials[i] = this.m_subTextObjects[i].sharedMaterial;
				}
			}
			return this.m_fontSharedMaterials;
		}

		protected override void SetSharedMaterials(Material[] materials)
		{
			int materialCount = this.m_textInfo.materialCount;
			if (this.m_fontSharedMaterials == null)
			{
				this.m_fontSharedMaterials = new Material[materialCount];
			}
			else if (this.m_fontSharedMaterials.Length != materialCount)
			{
				TMP_TextInfo.Resize<Material>(ref this.m_fontSharedMaterials, materialCount, false);
			}
			for (int i = 0; i < materialCount; i++)
			{
				if (i == 0)
				{
					if (!(materials[i].GetTexture(ShaderUtilities.ID_MainTex) == null) && materials[i].GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID() == this.m_sharedMaterial.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID())
					{
						this.m_sharedMaterial = (this.m_fontSharedMaterials[i] = materials[i]);
						this.m_padding = this.GetPaddingForMaterial(this.m_sharedMaterial);
					}
				}
				else if (!(materials[i].GetTexture(ShaderUtilities.ID_MainTex) == null) && materials[i].GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID() == this.m_subTextObjects[i].sharedMaterial.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID() && this.m_subTextObjects[i].isDefaultMaterial)
				{
					this.m_subTextObjects[i].sharedMaterial = (this.m_fontSharedMaterials[i] = materials[i]);
				}
			}
		}

		protected override void SetOutlineThickness(float thickness)
		{
			if (this.m_fontMaterial != null && this.m_sharedMaterial.GetInstanceID() != this.m_fontMaterial.GetInstanceID())
			{
				this.m_sharedMaterial = this.m_fontMaterial;
				this.m_canvasRenderer.SetMaterial(this.m_sharedMaterial, this.m_sharedMaterial.GetTexture(ShaderUtilities.ID_MainTex));
			}
			else if (this.m_fontMaterial == null)
			{
				this.m_fontMaterial = this.CreateMaterialInstance(this.m_sharedMaterial);
				this.m_sharedMaterial = this.m_fontMaterial;
				this.m_canvasRenderer.SetMaterial(this.m_sharedMaterial, this.m_sharedMaterial.GetTexture(ShaderUtilities.ID_MainTex));
			}
			thickness = Mathf.Clamp01(thickness);
			this.m_sharedMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, thickness);
			this.m_padding = this.GetPaddingForMaterial();
		}

		protected override void SetFaceColor(Color32 color)
		{
			if (this.m_fontMaterial == null)
			{
				this.m_fontMaterial = this.CreateMaterialInstance(this.m_sharedMaterial);
			}
			this.m_sharedMaterial = this.m_fontMaterial;
			this.m_padding = this.GetPaddingForMaterial();
			this.m_sharedMaterial.SetColor(ShaderUtilities.ID_FaceColor, color);
		}

		protected override void SetOutlineColor(Color32 color)
		{
			if (this.m_fontMaterial == null)
			{
				this.m_fontMaterial = this.CreateMaterialInstance(this.m_sharedMaterial);
			}
			this.m_sharedMaterial = this.m_fontMaterial;
			this.m_padding = this.GetPaddingForMaterial();
			this.m_sharedMaterial.SetColor(ShaderUtilities.ID_OutlineColor, color);
		}

		protected override void SetShaderDepth()
		{
			if (this.m_canvas == null || this.m_sharedMaterial == null)
			{
				return;
			}
			if (this.m_canvas.renderMode != RenderMode.ScreenSpaceOverlay)
			{
				bool isOverlay = this.m_isOverlay;
			}
		}

		protected override void SetCulling()
		{
			if (this.m_isCullingEnabled)
			{
				Material materialForRendering = this.materialForRendering;
				if (materialForRendering != null)
				{
					materialForRendering.SetFloat("_CullMode", 2f);
				}
				for (int i = 1; i < this.m_subTextObjects.Length; i++)
				{
					if (!(this.m_subTextObjects[i] != null))
					{
						return;
					}
					materialForRendering = this.m_subTextObjects[i].materialForRendering;
					if (materialForRendering != null)
					{
						materialForRendering.SetFloat(ShaderUtilities.ShaderTag_CullMode, 2f);
					}
				}
			}
			else
			{
				Material materialForRendering2 = this.materialForRendering;
				if (materialForRendering2 != null)
				{
					materialForRendering2.SetFloat("_CullMode", 0f);
				}
				int num = 1;
				while (num < this.m_subTextObjects.Length && this.m_subTextObjects[num] != null)
				{
					materialForRendering2 = this.m_subTextObjects[num].materialForRendering;
					if (materialForRendering2 != null)
					{
						materialForRendering2.SetFloat(ShaderUtilities.ShaderTag_CullMode, 0f);
					}
					num++;
				}
			}
		}

		private void SetPerspectiveCorrection()
		{
			if (this.m_isOrthographic)
			{
				this.m_sharedMaterial.SetFloat(ShaderUtilities.ID_PerspectiveFilter, 0f);
				return;
			}
			this.m_sharedMaterial.SetFloat(ShaderUtilities.ID_PerspectiveFilter, 0.875f);
		}

		private void SetMeshArrays(int size)
		{
			this.m_textInfo.meshInfo[0].ResizeMeshInfo(size);
			this.m_canvasRenderer.SetMesh(this.m_textInfo.meshInfo[0].mesh);
		}

		internal override int SetArraySizes(TMP_Text.TextProcessingElement[] textProcessingArray)
		{
			int num = 0;
			this.m_totalCharacterCount = 0;
			this.m_isUsingBold = false;
			this.m_isTextLayoutPhase = false;
			this.tag_NoParsing = false;
			this.m_FontStyleInternal = this.m_fontStyle;
			this.m_fontStyleStack.Clear();
			this.m_FontWeightInternal = (((this.m_FontStyleInternal & FontStyles.Bold) == FontStyles.Bold) ? FontWeight.Bold : this.m_fontWeight);
			this.m_FontWeightStack.SetDefault(this.m_FontWeightInternal);
			this.m_currentFontAsset = this.m_fontAsset;
			this.m_currentMaterial = this.m_sharedMaterial;
			this.m_currentMaterialIndex = 0;
			this.materialIndexPairs.Clear();
			TMP_Text.m_materialReferenceStack.SetDefault(new MaterialReference(this.m_currentMaterialIndex, this.m_currentFontAsset, null, this.m_currentMaterial, this.m_padding));
			TMP_Text.m_materialReferenceIndexLookup.Clear();
			MaterialReference.AddMaterialReference(this.m_currentMaterial, this.m_currentFontAsset, ref TMP_Text.m_materialReferences, TMP_Text.m_materialReferenceIndexLookup);
			if (this.m_textInfo == null)
			{
				this.m_textInfo = new TMP_TextInfo(this.m_InternalTextProcessingArraySize);
			}
			else if (this.m_textInfo.characterInfo.Length < this.m_InternalTextProcessingArraySize)
			{
				TMP_TextInfo.Resize<TMP_CharacterInfo>(ref this.m_textInfo.characterInfo, this.m_InternalTextProcessingArraySize, false);
			}
			this.m_textElementType = TMP_TextElementType.Character;
			if (this.m_overflowMode == TextOverflowModes.Ellipsis)
			{
				base.GetEllipsisSpecialCharacter(this.m_currentFontAsset);
				if (this.m_Ellipsis.character != null)
				{
					if (this.m_Ellipsis.fontAsset.GetInstanceID() != this.m_currentFontAsset.GetInstanceID())
					{
						if (TMP_Settings.matchMaterialPreset && this.m_currentMaterial.GetInstanceID() != this.m_Ellipsis.fontAsset.material.GetInstanceID())
						{
							this.m_Ellipsis.material = TMP_MaterialManager.GetFallbackMaterial(this.m_currentMaterial, this.m_Ellipsis.fontAsset.material);
						}
						else
						{
							this.m_Ellipsis.material = this.m_Ellipsis.fontAsset.material;
						}
						this.m_Ellipsis.materialIndex = MaterialReference.AddMaterialReference(this.m_Ellipsis.material, this.m_Ellipsis.fontAsset, ref TMP_Text.m_materialReferences, TMP_Text.m_materialReferenceIndexLookup);
						TMP_Text.m_materialReferences[this.m_Ellipsis.materialIndex].referenceCount = 0;
					}
				}
				else
				{
					this.m_overflowMode = TextOverflowModes.Truncate;
					if (!TMP_Settings.warningsDisabled)
					{
						Debug.LogWarning("The character used for Ellipsis is not available in font asset [" + this.m_currentFontAsset.name + "] or any potential fallbacks. Switching Text Overflow mode to Truncate.", this);
					}
				}
			}
			bool flag = this.m_ActiveFontFeatures.Contains(OTL_FeatureTag.liga);
			if (this.m_overflowMode == TextOverflowModes.Linked && this.m_linkedTextComponent != null && !this.m_isCalculatingPreferredValues)
			{
				TMP_Text linkedTextComponent = this.m_linkedTextComponent;
				while (linkedTextComponent != null)
				{
					linkedTextComponent.text = string.Empty;
					linkedTextComponent.ClearMesh();
					linkedTextComponent.textInfo.Clear();
					linkedTextComponent = linkedTextComponent.linkedTextComponent;
				}
			}
			int num2 = 0;
			while (num2 < textProcessingArray.Length && textProcessingArray[num2].unicode != 0U)
			{
				if (this.m_textInfo.characterInfo == null || this.m_totalCharacterCount >= this.m_textInfo.characterInfo.Length)
				{
					TMP_TextInfo.Resize<TMP_CharacterInfo>(ref this.m_textInfo.characterInfo, this.m_totalCharacterCount + 1, true);
				}
				uint num3 = textProcessingArray[num2].unicode;
				if (!this.m_isRichText || num3 != 60U)
				{
					goto IL_4B5;
				}
				int currentMaterialIndex = this.m_currentMaterialIndex;
				int num4;
				if (!base.ValidateHtmlTag(textProcessingArray, num2 + 1, out num4))
				{
					goto IL_4B5;
				}
				int stringIndex = textProcessingArray[num2].stringIndex;
				num2 = num4;
				if ((this.m_FontStyleInternal & FontStyles.Bold) == FontStyles.Bold)
				{
					this.m_isUsingBold = true;
				}
				if (this.m_textElementType == TMP_TextElementType.Sprite)
				{
					MaterialReference[] materialReferences = TMP_Text.m_materialReferences;
					int currentMaterialIndex2 = this.m_currentMaterialIndex;
					materialReferences[currentMaterialIndex2].referenceCount = materialReferences[currentMaterialIndex2].referenceCount + 1;
					this.m_textInfo.characterInfo[this.m_totalCharacterCount].character = (char)(57344 + this.m_spriteIndex);
					this.m_textInfo.characterInfo[this.m_totalCharacterCount].fontAsset = this.m_currentFontAsset;
					this.m_textInfo.characterInfo[this.m_totalCharacterCount].materialReferenceIndex = this.m_currentMaterialIndex;
					this.m_textInfo.characterInfo[this.m_totalCharacterCount].textElement = this.m_currentSpriteAsset.spriteCharacterTable[this.m_spriteIndex];
					this.m_textInfo.characterInfo[this.m_totalCharacterCount].elementType = this.m_textElementType;
					this.m_textInfo.characterInfo[this.m_totalCharacterCount].index = stringIndex;
					this.m_textInfo.characterInfo[this.m_totalCharacterCount].stringLength = textProcessingArray[num2].stringIndex - stringIndex + 1;
					this.m_textElementType = TMP_TextElementType.Character;
					this.m_currentMaterialIndex = currentMaterialIndex;
					num++;
					this.m_totalCharacterCount++;
				}
				IL_E50:
				num2++;
				continue;
				IL_4B5:
				bool isUsingAlternateTypeface = false;
				bool flag2 = false;
				TMP_FontAsset currentFontAsset = this.m_currentFontAsset;
				Material currentMaterial = this.m_currentMaterial;
				int currentMaterialIndex3 = this.m_currentMaterialIndex;
				if (this.m_textElementType == TMP_TextElementType.Character)
				{
					if ((this.m_FontStyleInternal & FontStyles.UpperCase) == FontStyles.UpperCase)
					{
						if (char.IsLower((char)num3))
						{
							num3 = (uint)char.ToUpper((char)num3);
						}
					}
					else if ((this.m_FontStyleInternal & FontStyles.LowerCase) == FontStyles.LowerCase)
					{
						if (char.IsUpper((char)num3))
						{
							num3 = (uint)char.ToLower((char)num3);
						}
					}
					else if ((this.m_FontStyleInternal & FontStyles.SmallCaps) == FontStyles.SmallCaps && char.IsLower((char)num3))
					{
						num3 = (uint)char.ToUpper((char)num3);
					}
				}
				TMP_TextElement tmp_TextElement = null;
				uint num5 = (num2 + 1 < textProcessingArray.Length) ? textProcessingArray[num2 + 1].unicode : 0U;
				if (base.emojiFallbackSupport && ((TMP_TextParsingUtilities.IsEmojiPresentationForm(num3) && num5 != 65038U) || (TMP_TextParsingUtilities.IsEmoji(num3) && num5 == 65039U)) && TMP_Settings.emojiFallbackTextAssets != null && TMP_Settings.emojiFallbackTextAssets.Count > 0)
				{
					tmp_TextElement = TMP_FontAssetUtilities.GetTextElementFromTextAssets(num3, this.m_currentFontAsset, TMP_Settings.emojiFallbackTextAssets, true, base.fontStyle, base.fontWeight, out isUsingAlternateTypeface);
				}
				if (tmp_TextElement == null)
				{
					tmp_TextElement = base.GetTextElement(num3, this.m_currentFontAsset, this.m_FontStyleInternal, this.m_FontWeightInternal, out isUsingAlternateTypeface);
				}
				if (tmp_TextElement == null)
				{
					base.DoMissingGlyphCallback((int)num3, textProcessingArray[num2].stringIndex, this.m_currentFontAsset);
					uint num6 = num3;
					num3 = (textProcessingArray[num2].unicode = (uint)((TMP_Settings.missingGlyphCharacter == 0) ? 9633 : TMP_Settings.missingGlyphCharacter));
					tmp_TextElement = TMP_FontAssetUtilities.GetCharacterFromFontAsset(num3, this.m_currentFontAsset, true, FontStyles.Normal, FontWeight.Regular, out isUsingAlternateTypeface);
					if (tmp_TextElement == null && TMP_Settings.fallbackFontAssets != null && TMP_Settings.fallbackFontAssets.Count > 0)
					{
						tmp_TextElement = TMP_FontAssetUtilities.GetCharacterFromFontAssets(num3, this.m_currentFontAsset, TMP_Settings.fallbackFontAssets, true, FontStyles.Normal, FontWeight.Regular, out isUsingAlternateTypeface);
					}
					if (tmp_TextElement == null && TMP_Settings.defaultFontAsset != null)
					{
						tmp_TextElement = TMP_FontAssetUtilities.GetCharacterFromFontAsset(num3, TMP_Settings.defaultFontAsset, true, FontStyles.Normal, FontWeight.Regular, out isUsingAlternateTypeface);
					}
					if (tmp_TextElement == null)
					{
						num3 = (textProcessingArray[num2].unicode = 32U);
						tmp_TextElement = TMP_FontAssetUtilities.GetCharacterFromFontAsset(num3, this.m_currentFontAsset, true, FontStyles.Normal, FontWeight.Regular, out isUsingAlternateTypeface);
					}
					if (tmp_TextElement == null)
					{
						num3 = (textProcessingArray[num2].unicode = 3U);
						tmp_TextElement = TMP_FontAssetUtilities.GetCharacterFromFontAsset(num3, this.m_currentFontAsset, true, FontStyles.Normal, FontWeight.Regular, out isUsingAlternateTypeface);
					}
					if (!TMP_Settings.warningsDisabled)
					{
						Debug.LogWarning((num6 > 65535U) ? string.Format("The character with Unicode value \\U{0:X8} was not found in the [{1}] font asset or any potential fallbacks. It was replaced by Unicode character \\u{2:X4} in text object [{3}].", new object[]
						{
							num6,
							this.m_fontAsset.name,
							tmp_TextElement.unicode,
							base.name
						}) : string.Format("The character with Unicode value \\u{0:X4} was not found in the [{1}] font asset or any potential fallbacks. It was replaced by Unicode character \\u{2:X4} in text object [{3}].", new object[]
						{
							num6,
							this.m_fontAsset.name,
							tmp_TextElement.unicode,
							base.name
						}), this);
					}
				}
				this.m_textInfo.characterInfo[this.m_totalCharacterCount].alternativeGlyph = null;
				if (tmp_TextElement.elementType == TextElementType.Character)
				{
					if (tmp_TextElement.textAsset.instanceID != this.m_currentFontAsset.instanceID)
					{
						flag2 = true;
						this.m_currentFontAsset = (tmp_TextElement.textAsset as TMP_FontAsset);
					}
					if ((num5 >= 65024U && num5 <= 65039U) || (num5 >= 917760U && num5 <= 917999U))
					{
						uint glyphVariantIndex = this.m_currentFontAsset.GetGlyphVariantIndex(num3, num5);
						Glyph alternativeGlyph;
						if (glyphVariantIndex != 0U && this.m_currentFontAsset.TryAddGlyphInternal(glyphVariantIndex, out alternativeGlyph))
						{
							this.m_textInfo.characterInfo[this.m_totalCharacterCount].alternativeGlyph = alternativeGlyph;
						}
						textProcessingArray[num2 + 1].unicode = 26U;
						num2++;
					}
					List<LigatureSubstitutionRecord> list;
					if (flag && this.m_currentFontAsset.fontFeatureTable.m_LigatureSubstitutionRecordLookup.TryGetValue(tmp_TextElement.glyphIndex, out list))
					{
						if (list == null)
						{
							break;
						}
						for (int i = 0; i < list.Count; i++)
						{
							LigatureSubstitutionRecord ligatureSubstitutionRecord = list[i];
							uint[] componentGlyphIDs = ligatureSubstitutionRecord.componentGlyphIDs;
							int num7 = ligatureSubstitutionRecord.componentGlyphIDs.Length;
							uint num8 = ligatureSubstitutionRecord.ligatureGlyphID;
							int num9 = num2 + 1;
							int num10 = 1;
							while (num8 != 0U && num10 < num7)
							{
								if (num9 >= textProcessingArray.Length)
								{
									num8 = 0U;
									break;
								}
								uint unicode = textProcessingArray[num9].unicode;
								uint glyphIndex = this.m_currentFontAsset.GetGlyphIndex(unicode);
								uint num11 = componentGlyphIDs[num10];
								if (glyphIndex != num11 && TMP_TextParsingUtilities.IsIgnorableForLigature(unicode))
								{
									num9++;
								}
								else
								{
									if (glyphIndex != num11)
									{
										num8 = 0U;
										break;
									}
									num10++;
									num9++;
								}
							}
							if (num8 != 0U && num10 == num7)
							{
								int num12 = num9 - num2;
								Glyph alternativeGlyph2;
								if (this.m_currentFontAsset.TryAddGlyphInternal(num8, out alternativeGlyph2))
								{
									this.m_textInfo.characterInfo[this.m_totalCharacterCount].alternativeGlyph = alternativeGlyph2;
									textProcessingArray[num2].length = num12;
									for (int j = 1; j < num12; j++)
									{
										textProcessingArray[num2 + j].unicode = 26U;
									}
									num2 += num12 - 1;
									break;
								}
							}
						}
					}
				}
				this.m_textInfo.characterInfo[this.m_totalCharacterCount].elementType = TMP_TextElementType.Character;
				this.m_textInfo.characterInfo[this.m_totalCharacterCount].textElement = tmp_TextElement;
				this.m_textInfo.characterInfo[this.m_totalCharacterCount].isUsingAlternateTypeface = isUsingAlternateTypeface;
				this.m_textInfo.characterInfo[this.m_totalCharacterCount].character = (char)num3;
				this.m_textInfo.characterInfo[this.m_totalCharacterCount].index = textProcessingArray[num2].stringIndex;
				this.m_textInfo.characterInfo[this.m_totalCharacterCount].stringLength = textProcessingArray[num2].length;
				this.m_textInfo.characterInfo[this.m_totalCharacterCount].fontAsset = this.m_currentFontAsset;
				if (tmp_TextElement.elementType == TextElementType.Sprite)
				{
					TMP_SpriteAsset tmp_SpriteAsset = tmp_TextElement.textAsset as TMP_SpriteAsset;
					this.m_currentMaterialIndex = MaterialReference.AddMaterialReference(tmp_SpriteAsset.material, tmp_SpriteAsset, ref TMP_Text.m_materialReferences, TMP_Text.m_materialReferenceIndexLookup);
					MaterialReference[] materialReferences2 = TMP_Text.m_materialReferences;
					int currentMaterialIndex4 = this.m_currentMaterialIndex;
					materialReferences2[currentMaterialIndex4].referenceCount = materialReferences2[currentMaterialIndex4].referenceCount + 1;
					this.m_textInfo.characterInfo[this.m_totalCharacterCount].elementType = TMP_TextElementType.Sprite;
					this.m_textInfo.characterInfo[this.m_totalCharacterCount].materialReferenceIndex = this.m_currentMaterialIndex;
					this.m_textElementType = TMP_TextElementType.Character;
					this.m_currentMaterialIndex = currentMaterialIndex3;
					num++;
					this.m_totalCharacterCount++;
					goto IL_E50;
				}
				if (flag2 && this.m_currentFontAsset.instanceID != this.m_fontAsset.instanceID)
				{
					if (TMP_Settings.matchMaterialPreset)
					{
						this.m_currentMaterial = TMP_MaterialManager.GetFallbackMaterial(this.m_currentMaterial, this.m_currentFontAsset.material);
					}
					else
					{
						this.m_currentMaterial = this.m_currentFontAsset.material;
					}
					this.m_currentMaterialIndex = MaterialReference.AddMaterialReference(this.m_currentMaterial, this.m_currentFontAsset, ref TMP_Text.m_materialReferences, TMP_Text.m_materialReferenceIndexLookup);
				}
				int num13 = (this.m_textInfo.characterInfo[this.m_totalCharacterCount].alternativeGlyph != null) ? this.m_textInfo.characterInfo[this.m_totalCharacterCount].alternativeGlyph.atlasIndex : ((tmp_TextElement == null) ? 0 : tmp_TextElement.glyph.atlasIndex);
				if (tmp_TextElement != null && num13 > 0)
				{
					this.m_currentMaterial = TMP_MaterialManager.GetFallbackMaterial(this.m_currentFontAsset, this.m_currentMaterial, num13);
					this.m_currentMaterialIndex = MaterialReference.AddMaterialReference(this.m_currentMaterial, this.m_currentFontAsset, ref TMP_Text.m_materialReferences, TMP_Text.m_materialReferenceIndexLookup);
					flag2 = true;
				}
				if (!char.IsWhiteSpace((char)num3) && num3 != 8203U)
				{
					if (TMP_Text.m_materialReferences[this.m_currentMaterialIndex].referenceCount < 16383)
					{
						MaterialReference[] materialReferences3 = TMP_Text.m_materialReferences;
						int currentMaterialIndex5 = this.m_currentMaterialIndex;
						materialReferences3[currentMaterialIndex5].referenceCount = materialReferences3[currentMaterialIndex5].referenceCount + 1;
					}
					else if (flag2)
					{
						int num14;
						if (this.materialIndexPairs.TryGetValue(this.m_currentMaterialIndex, out num14) && TMP_Text.m_materialReferences[num14].referenceCount < 16383)
						{
							this.m_currentMaterialIndex = num14;
						}
						else
						{
							int num15 = MaterialReference.AddMaterialReference(new Material(this.m_currentMaterial), this.m_currentFontAsset, ref TMP_Text.m_materialReferences, TMP_Text.m_materialReferenceIndexLookup);
							this.materialIndexPairs[this.m_currentMaterialIndex] = num15;
							this.m_currentMaterialIndex = num15;
						}
						MaterialReference[] materialReferences4 = TMP_Text.m_materialReferences;
						int currentMaterialIndex6 = this.m_currentMaterialIndex;
						materialReferences4[currentMaterialIndex6].referenceCount = materialReferences4[currentMaterialIndex6].referenceCount + 1;
					}
					else
					{
						this.m_currentMaterialIndex = MaterialReference.AddMaterialReference(new Material(this.m_currentMaterial), this.m_currentFontAsset, ref TMP_Text.m_materialReferences, TMP_Text.m_materialReferenceIndexLookup);
						MaterialReference[] materialReferences5 = TMP_Text.m_materialReferences;
						int currentMaterialIndex7 = this.m_currentMaterialIndex;
						materialReferences5[currentMaterialIndex7].referenceCount = materialReferences5[currentMaterialIndex7].referenceCount + 1;
					}
				}
				this.m_textInfo.characterInfo[this.m_totalCharacterCount].material = this.m_currentMaterial;
				this.m_textInfo.characterInfo[this.m_totalCharacterCount].materialReferenceIndex = this.m_currentMaterialIndex;
				TMP_Text.m_materialReferences[this.m_currentMaterialIndex].isFallbackMaterial = flag2;
				if (flag2)
				{
					TMP_Text.m_materialReferences[this.m_currentMaterialIndex].fallbackMaterial = currentMaterial;
					this.m_currentFontAsset = currentFontAsset;
					this.m_currentMaterial = currentMaterial;
					this.m_currentMaterialIndex = currentMaterialIndex3;
				}
				this.m_totalCharacterCount++;
				goto IL_E50;
			}
			if (this.m_isCalculatingPreferredValues)
			{
				this.m_isCalculatingPreferredValues = false;
				return this.m_totalCharacterCount;
			}
			this.m_textInfo.spriteCount = num;
			int num16 = this.m_textInfo.materialCount = TMP_Text.m_materialReferenceIndexLookup.Count;
			if (num16 > this.m_textInfo.meshInfo.Length)
			{
				TMP_TextInfo.Resize<TMP_MeshInfo>(ref this.m_textInfo.meshInfo, num16, false);
			}
			if (num16 > this.m_subTextObjects.Length)
			{
				TMP_TextInfo.Resize<TMP_SubMeshUI>(ref this.m_subTextObjects, Mathf.NextPowerOfTwo(num16 + 1));
			}
			if (this.m_VertexBufferAutoSizeReduction && this.m_textInfo.characterInfo.Length - this.m_totalCharacterCount > 256)
			{
				TMP_TextInfo.Resize<TMP_CharacterInfo>(ref this.m_textInfo.characterInfo, Mathf.Max(this.m_totalCharacterCount + 1, 256), true);
			}
			for (int k = 0; k < num16; k++)
			{
				if (k > 0)
				{
					if (this.m_subTextObjects[k] == null)
					{
						this.m_subTextObjects[k] = TMP_SubMeshUI.AddSubTextObject(this, TMP_Text.m_materialReferences[k]);
						this.m_textInfo.meshInfo[k].vertices = null;
					}
					if (this.m_rectTransform.pivot != this.m_subTextObjects[k].rectTransform.pivot)
					{
						this.m_subTextObjects[k].rectTransform.pivot = this.m_rectTransform.pivot;
					}
					if (this.m_subTextObjects[k].sharedMaterial == null || this.m_subTextObjects[k].sharedMaterial.GetInstanceID() != TMP_Text.m_materialReferences[k].material.GetInstanceID())
					{
						this.m_subTextObjects[k].sharedMaterial = TMP_Text.m_materialReferences[k].material;
						this.m_subTextObjects[k].fontAsset = TMP_Text.m_materialReferences[k].fontAsset;
						this.m_subTextObjects[k].spriteAsset = TMP_Text.m_materialReferences[k].spriteAsset;
					}
					if (TMP_Text.m_materialReferences[k].isFallbackMaterial)
					{
						this.m_subTextObjects[k].fallbackMaterial = TMP_Text.m_materialReferences[k].material;
						this.m_subTextObjects[k].fallbackSourceMaterial = TMP_Text.m_materialReferences[k].fallbackMaterial;
					}
				}
				int referenceCount = TMP_Text.m_materialReferences[k].referenceCount;
				if (this.m_textInfo.meshInfo[k].vertices == null || this.m_textInfo.meshInfo[k].vertices.Length < referenceCount * 4)
				{
					if (this.m_textInfo.meshInfo[k].vertices == null)
					{
						if (k == 0)
						{
							this.m_textInfo.meshInfo[k] = new TMP_MeshInfo(this.m_mesh, referenceCount + 1);
						}
						else
						{
							this.m_textInfo.meshInfo[k] = new TMP_MeshInfo(this.m_subTextObjects[k].mesh, referenceCount + 1);
						}
					}
					else
					{
						this.m_textInfo.meshInfo[k].ResizeMeshInfo((referenceCount > 1024) ? (referenceCount + 256) : Mathf.NextPowerOfTwo(referenceCount + 1));
					}
				}
				else if (this.m_VertexBufferAutoSizeReduction && referenceCount > 0 && this.m_textInfo.meshInfo[k].vertices.Length / 4 - referenceCount > 256)
				{
					this.m_textInfo.meshInfo[k].ResizeMeshInfo((referenceCount > 1024) ? (referenceCount + 256) : Mathf.NextPowerOfTwo(referenceCount + 1));
				}
				this.m_textInfo.meshInfo[k].material = TMP_Text.m_materialReferences[k].material;
			}
			int num17 = num16;
			while (num17 < this.m_subTextObjects.Length && this.m_subTextObjects[num17] != null)
			{
				if (num17 < this.m_textInfo.meshInfo.Length)
				{
					this.m_subTextObjects[num17].canvasRenderer.SetMesh(null);
				}
				num17++;
			}
			return this.m_totalCharacterCount;
		}

		public override void ComputeMarginSize()
		{
			if (base.rectTransform != null)
			{
				Rect rect = this.m_rectTransform.rect;
				this.m_marginWidth = rect.width - this.m_margin.x - this.m_margin.z;
				this.m_marginHeight = rect.height - this.m_margin.y - this.m_margin.w;
				this.m_PreviousRectTransformSize = rect.size;
				this.m_PreviousPivotPosition = this.m_rectTransform.pivot;
				this.m_RectTransformCorners = this.GetTextContainerLocalCorners();
			}
		}

		protected override void OnDidApplyAnimationProperties()
		{
			this.m_havePropertiesChanged = true;
			this.SetVerticesDirty();
			this.SetLayoutDirty();
		}

		protected override void OnCanvasHierarchyChanged()
		{
			base.OnCanvasHierarchyChanged();
			this.m_canvas = base.canvas;
			if (!this.m_isAwake || !base.isActiveAndEnabled)
			{
				return;
			}
			if (this.m_canvas == null || !this.m_canvas.enabled)
			{
				TMP_UpdateManager.UnRegisterTextObjectForUpdate(this);
				return;
			}
			if (!this.m_IsTextObjectScaleStatic)
			{
				TMP_UpdateManager.RegisterTextObjectForUpdate(this);
			}
		}

		protected override void OnTransformParentChanged()
		{
			base.OnTransformParentChanged();
			this.m_canvas = base.canvas;
			this.ComputeMarginSize();
			this.m_havePropertiesChanged = true;
		}

		protected override void OnRectTransformDimensionsChange()
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			bool flag = false;
			if (this.m_canvas != null && !Mathf.Approximately(this.m_CanvasScaleFactor, this.m_canvas.scaleFactor))
			{
				this.m_CanvasScaleFactor = this.m_canvas.scaleFactor;
				flag = true;
			}
			if (!flag && base.rectTransform != null && Mathf.Abs(this.m_rectTransform.rect.width - this.m_PreviousRectTransformSize.x) < 0.0001f && Mathf.Abs(this.m_rectTransform.rect.height - this.m_PreviousRectTransformSize.y) < 0.0001f && Mathf.Abs(this.m_rectTransform.pivot.x - this.m_PreviousPivotPosition.x) < 0.0001f && Mathf.Abs(this.m_rectTransform.pivot.y - this.m_PreviousPivotPosition.y) < 0.0001f)
			{
				return;
			}
			this.ComputeMarginSize();
			this.UpdateSubObjectPivot();
			this.SetVerticesDirty();
			this.SetLayoutDirty();
		}

		internal override void InternalUpdate()
		{
			if (!this.m_havePropertiesChanged)
			{
				float y = this.m_rectTransform.lossyScale.y;
				if (y != this.m_previousLossyScaleY && this.m_TextProcessingArray[0].unicode != 0U)
				{
					float num = y / this.m_previousLossyScaleY;
					if (num < 0.8f || num > 1.25f)
					{
						this.UpdateSDFScale(num);
						this.m_previousLossyScaleY = y;
					}
				}
			}
			if (this.m_isUsingLegacyAnimationComponent)
			{
				this.m_havePropertiesChanged = true;
				this.OnPreRenderCanvas();
			}
			this.UpdateEnvMapMatrix();
		}

		private void OnPreRenderCanvas()
		{
			if (!this.m_isAwake || (!this.IsActive() && !this.m_ignoreActiveState))
			{
				return;
			}
			if (this.m_canvas == null)
			{
				this.m_canvas = base.canvas;
				if (this.m_canvas == null)
				{
					return;
				}
			}
			if (this.m_havePropertiesChanged || this.m_isLayoutDirty)
			{
				if (this.m_fontAsset == null)
				{
					Debug.LogWarning("Please assign a Font Asset to this " + base.transform.name + " gameobject.", this);
					return;
				}
				if (this.checkPaddingRequired)
				{
					this.UpdateMeshPadding();
				}
				base.ParseInputText();
				TMP_FontAsset.UpdateFontAssetsInUpdateQueue();
				if (this.m_enableAutoSizing)
				{
					this.m_fontSize = Mathf.Clamp(this.m_fontSizeBase, this.m_fontSizeMin, this.m_fontSizeMax);
				}
				this.m_maxFontSize = this.m_fontSizeMax;
				this.m_minFontSize = this.m_fontSizeMin;
				this.m_lineSpacingDelta = 0f;
				this.m_charWidthAdjDelta = 0f;
				this.m_isTextTruncated = false;
				this.m_havePropertiesChanged = false;
				this.m_isLayoutDirty = false;
				this.m_ignoreActiveState = false;
				this.m_IsAutoSizePointSizeSet = false;
				this.m_AutoSizeIterationCount = 0;
				while (!this.m_IsAutoSizePointSizeSet)
				{
					this.GenerateTextMesh();
					this.m_AutoSizeIterationCount++;
				}
			}
		}

		protected virtual void GenerateTextMesh()
		{
			if (this.m_fontAsset == null || this.m_fontAsset.characterLookupTable == null)
			{
				Debug.LogWarning("Can't Generate Mesh! No Font Asset has been assigned to Object ID: " + base.GetInstanceID().ToString());
				this.m_IsAutoSizePointSizeSet = true;
				return;
			}
			if (this.m_textInfo != null)
			{
				this.m_textInfo.Clear();
			}
			if (this.m_TextProcessingArray == null || this.m_TextProcessingArray.Length == 0 || this.m_TextProcessingArray[0].unicode == 0U)
			{
				this.ClearMesh();
				this.m_preferredWidth = 0f;
				this.m_preferredHeight = 0f;
				TMPro_EventManager.ON_TEXT_CHANGED(this);
				this.m_IsAutoSizePointSizeSet = true;
				return;
			}
			this.m_currentFontAsset = this.m_fontAsset;
			this.m_currentMaterial = this.m_sharedMaterial;
			this.m_currentMaterialIndex = 0;
			TMP_Text.m_materialReferenceStack.SetDefault(new MaterialReference(this.m_currentMaterialIndex, this.m_currentFontAsset, null, this.m_currentMaterial, this.m_padding));
			this.m_currentSpriteAsset = this.m_spriteAsset;
			if (this.m_spriteAnimator != null)
			{
				this.m_spriteAnimator.StopAllAnimations();
			}
			int totalCharacterCount = this.m_totalCharacterCount;
			float num = this.m_isOrthographic ? 1f : 0.1f;
			float num2 = this.m_fontSize / this.m_fontAsset.m_FaceInfo.pointSize * this.m_fontAsset.m_FaceInfo.scale * num;
			float num3 = num2;
			float num4 = this.m_fontSize * 0.01f * num;
			this.m_fontScaleMultiplier = 1f;
			this.m_currentFontSize = this.m_fontSize;
			this.m_sizeStack.SetDefault(this.m_currentFontSize);
			uint num5 = 0U;
			this.m_FontStyleInternal = this.m_fontStyle;
			this.m_FontWeightInternal = (((this.m_FontStyleInternal & FontStyles.Bold) == FontStyles.Bold) ? FontWeight.Bold : this.m_fontWeight);
			this.m_FontWeightStack.SetDefault(this.m_FontWeightInternal);
			this.m_fontStyleStack.Clear();
			this.m_lineJustification = this.m_HorizontalAlignment;
			this.m_lineJustificationStack.SetDefault(this.m_lineJustification);
			float num6 = 0f;
			this.m_baselineOffset = 0f;
			this.m_baselineOffsetStack.Clear();
			bool flag = false;
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			bool flag2 = false;
			Vector3 zero3 = Vector3.zero;
			Vector3 zero4 = Vector3.zero;
			bool flag3 = false;
			Vector3 vector = Vector3.zero;
			Vector3 vector2 = Vector3.zero;
			this.m_fontColor32 = this.m_fontColor;
			this.m_htmlColor = this.m_fontColor32;
			this.m_underlineColor = this.m_htmlColor;
			this.m_strikethroughColor = this.m_htmlColor;
			this.m_colorStack.SetDefault(this.m_htmlColor);
			this.m_underlineColorStack.SetDefault(this.m_htmlColor);
			this.m_strikethroughColorStack.SetDefault(this.m_htmlColor);
			this.m_HighlightStateStack.SetDefault(new HighlightState(this.m_htmlColor, TMP_Offset.zero));
			this.m_colorGradientPreset = null;
			this.m_colorGradientStack.SetDefault(null);
			this.m_ItalicAngle = (int)this.m_currentFontAsset.italicStyle;
			this.m_ItalicAngleStack.SetDefault(this.m_ItalicAngle);
			this.m_actionStack.Clear();
			this.m_FXScale = Vector3.one;
			this.m_FXRotation = Quaternion.identity;
			this.m_lineOffset = 0f;
			this.m_lineHeight = -32767f;
			float num7 = this.m_currentFontAsset.m_FaceInfo.lineHeight - (this.m_currentFontAsset.m_FaceInfo.ascentLine - this.m_currentFontAsset.m_FaceInfo.descentLine);
			this.m_cSpacing = 0f;
			this.m_monoSpacing = 0f;
			this.m_xAdvance = 0f;
			this.tag_LineIndent = 0f;
			this.tag_Indent = 0f;
			this.m_indentStack.SetDefault(0f);
			this.tag_NoParsing = false;
			this.m_characterCount = 0;
			this.m_firstCharacterOfLine = this.m_firstVisibleCharacter;
			this.m_lastCharacterOfLine = 0;
			this.m_firstVisibleCharacterOfLine = 0;
			this.m_lastVisibleCharacterOfLine = 0;
			this.m_maxLineAscender = TMP_Text.k_LargeNegativeFloat;
			this.m_maxLineDescender = TMP_Text.k_LargePositiveFloat;
			this.m_lineNumber = 0;
			this.m_startOfLineAscender = 0f;
			this.m_startOfLineDescender = 0f;
			this.m_lineVisibleCharacterCount = 0;
			this.m_lineVisibleSpaceCount = 0;
			bool flag4 = true;
			this.m_IsDrivenLineSpacing = false;
			this.m_firstOverflowCharacterIndex = -1;
			this.m_LastBaseGlyphIndex = int.MinValue;
			bool flag5 = this.m_ActiveFontFeatures.Contains(OTL_FeatureTag.kern);
			bool flag6 = this.m_ActiveFontFeatures.Contains(OTL_FeatureTag.mark);
			bool flag7 = this.m_ActiveFontFeatures.Contains(OTL_FeatureTag.mkmk);
			this.m_pageNumber = 0;
			int num8 = Mathf.Clamp(this.m_pageToDisplay - 1, 0, this.m_textInfo.pageInfo.Length - 1);
			this.m_textInfo.ClearPageInfo();
			Vector4 margin = this.m_margin;
			float num9 = (this.m_marginWidth > 0f) ? this.m_marginWidth : 0f;
			float num10 = (this.m_marginHeight > 0f) ? this.m_marginHeight : 0f;
			this.m_marginLeft = 0f;
			this.m_marginRight = 0f;
			this.m_width = -1f;
			float num11 = num9 + 0.0001f - this.m_marginLeft - this.m_marginRight;
			this.m_meshExtents.min = TMP_Text.k_LargePositiveVector2;
			this.m_meshExtents.max = TMP_Text.k_LargeNegativeVector2;
			this.m_textInfo.ClearLineInfo();
			this.m_maxCapHeight = 0f;
			this.m_maxTextAscender = 0f;
			this.m_ElementDescender = 0f;
			this.m_PageAscender = 0f;
			float num12 = 0f;
			bool flag8 = false;
			this.m_isNewPage = false;
			bool flag9 = true;
			this.m_isNonBreakingSpace = false;
			bool flag10 = false;
			int num13 = 0;
			TMP_Text.CharacterSubstitution characterSubstitution = new TMP_Text.CharacterSubstitution(-1, 0U);
			bool flag11 = false;
			base.SaveWordWrappingState(ref TMP_Text.m_SavedWordWrapState, -1, -1);
			base.SaveWordWrappingState(ref TMP_Text.m_SavedLineState, -1, -1);
			base.SaveWordWrappingState(ref TMP_Text.m_SavedEllipsisState, -1, -1);
			base.SaveWordWrappingState(ref TMP_Text.m_SavedLastValidState, -1, -1);
			base.SaveWordWrappingState(ref TMP_Text.m_SavedSoftLineBreakState, -1, -1);
			TMP_Text.m_EllipsisInsertionCandidateStack.Clear();
			int num14 = 0;
			int num15 = 0;
			while (num15 < this.m_TextProcessingArray.Length && this.m_TextProcessingArray[num15].unicode != 0U)
			{
				num5 = this.m_TextProcessingArray[num15].unicode;
				if (num14 > 5)
				{
					Debug.LogError("Line breaking recursion max threshold hit... Character [" + num5.ToString() + "] index: " + num15.ToString());
					characterSubstitution.index = this.m_characterCount;
					characterSubstitution.unicode = 3U;
				}
				if (num5 != 26U)
				{
					if (this.m_isRichText && num5 == 60U)
					{
						this.m_isTextLayoutPhase = true;
						this.m_textElementType = TMP_TextElementType.Character;
						int num16;
						if (base.ValidateHtmlTag(this.m_TextProcessingArray, num15 + 1, out num16))
						{
							num15 = num16;
							if (this.m_textElementType == TMP_TextElementType.Character)
							{
								goto IL_3FF8;
							}
						}
					}
					else
					{
						this.m_textElementType = this.m_textInfo.characterInfo[this.m_characterCount].elementType;
						this.m_currentMaterialIndex = this.m_textInfo.characterInfo[this.m_characterCount].materialReferenceIndex;
						this.m_currentFontAsset = this.m_textInfo.characterInfo[this.m_characterCount].fontAsset;
					}
					int currentMaterialIndex = this.m_currentMaterialIndex;
					bool isUsingAlternateTypeface = this.m_textInfo.characterInfo[this.m_characterCount].isUsingAlternateTypeface;
					this.m_isTextLayoutPhase = false;
					bool flag12 = false;
					if (characterSubstitution.index == this.m_characterCount)
					{
						num5 = characterSubstitution.unicode;
						this.m_textElementType = TMP_TextElementType.Character;
						flag12 = true;
						if (num5 != 3U)
						{
							if (num5 != 45U)
							{
								if (num5 == 8230U)
								{
									this.m_textInfo.characterInfo[this.m_characterCount].textElement = this.m_Ellipsis.character;
									this.m_textInfo.characterInfo[this.m_characterCount].elementType = TMP_TextElementType.Character;
									this.m_textInfo.characterInfo[this.m_characterCount].fontAsset = this.m_Ellipsis.fontAsset;
									this.m_textInfo.characterInfo[this.m_characterCount].material = this.m_Ellipsis.material;
									this.m_textInfo.characterInfo[this.m_characterCount].materialReferenceIndex = this.m_Ellipsis.materialIndex;
									MaterialReference[] materialReferences = TMP_Text.m_materialReferences;
									int materialIndex = this.m_Underline.materialIndex;
									materialReferences[materialIndex].referenceCount = materialReferences[materialIndex].referenceCount + 1;
									this.m_isTextTruncated = true;
									characterSubstitution.index = this.m_characterCount + 1;
									characterSubstitution.unicode = 3U;
								}
							}
						}
						else
						{
							this.m_textInfo.characterInfo[this.m_characterCount].textElement = this.m_currentFontAsset.characterLookupTable[3U];
							this.m_isTextTruncated = true;
						}
					}
					if (this.m_characterCount < this.m_firstVisibleCharacter && num5 != 3U)
					{
						this.m_textInfo.characterInfo[this.m_characterCount].isVisible = false;
						this.m_textInfo.characterInfo[this.m_characterCount].character = '​';
						this.m_textInfo.characterInfo[this.m_characterCount].lineNumber = 0;
						this.m_characterCount++;
					}
					else
					{
						float num17 = 1f;
						if (this.m_textElementType == TMP_TextElementType.Character)
						{
							if ((this.m_FontStyleInternal & FontStyles.UpperCase) == FontStyles.UpperCase)
							{
								if (char.IsLower((char)num5))
								{
									num5 = (uint)char.ToUpper((char)num5);
								}
							}
							else if ((this.m_FontStyleInternal & FontStyles.LowerCase) == FontStyles.LowerCase)
							{
								if (char.IsUpper((char)num5))
								{
									num5 = (uint)char.ToLower((char)num5);
								}
							}
							else if ((this.m_FontStyleInternal & FontStyles.SmallCaps) == FontStyles.SmallCaps && char.IsLower((char)num5))
							{
								num17 = 0.8f;
								num5 = (uint)char.ToUpper((char)num5);
							}
						}
						float num18 = 0f;
						float num19 = 0f;
						float num20 = 0f;
						FaceInfo faceInfo = this.m_currentFontAsset.m_FaceInfo;
						if (this.m_textElementType == TMP_TextElementType.Sprite)
						{
							TMP_SpriteCharacter tmp_SpriteCharacter = (TMP_SpriteCharacter)base.textInfo.characterInfo[this.m_characterCount].textElement;
							if (tmp_SpriteCharacter == null)
							{
								goto IL_3FF8;
							}
							this.m_currentSpriteAsset = (tmp_SpriteCharacter.textAsset as TMP_SpriteAsset);
							this.m_spriteIndex = (int)tmp_SpriteCharacter.glyphIndex;
							if (num5 == 60U)
							{
								num5 = (uint)(57344 + this.m_spriteIndex);
							}
							else
							{
								this.m_spriteColor = TMP_Text.s_colorWhite;
							}
							float num21 = this.m_currentFontSize / faceInfo.pointSize * faceInfo.scale * num;
							FaceInfo faceInfo2 = this.m_currentSpriteAsset.m_FaceInfo;
							if (faceInfo2.pointSize > 0f)
							{
								float num22 = this.m_currentFontSize / faceInfo2.pointSize * faceInfo2.scale * num;
								num3 = tmp_SpriteCharacter.m_Scale * tmp_SpriteCharacter.m_Glyph.scale * num22;
								num19 = faceInfo2.ascentLine;
								num18 = faceInfo2.baseline * num21 * this.m_fontScaleMultiplier * faceInfo2.scale;
								num20 = faceInfo2.descentLine;
							}
							else
							{
								float num23 = this.m_currentFontSize / faceInfo.pointSize * faceInfo.scale * num;
								num3 = faceInfo.ascentLine / tmp_SpriteCharacter.m_Glyph.metrics.height * tmp_SpriteCharacter.m_Scale * tmp_SpriteCharacter.m_Glyph.scale * num23;
								float num24 = (num3 != 0f) ? (num23 / num3) : 0f;
								num19 = faceInfo.ascentLine * num24;
								num18 = faceInfo.baseline * num21 * this.m_fontScaleMultiplier * faceInfo.scale;
								num20 = faceInfo.descentLine * num24;
							}
							this.m_cached_TextElement = tmp_SpriteCharacter;
							this.m_textInfo.characterInfo[this.m_characterCount].elementType = TMP_TextElementType.Sprite;
							this.m_textInfo.characterInfo[this.m_characterCount].scale = num3;
							this.m_textInfo.characterInfo[this.m_characterCount].fontAsset = this.m_currentFontAsset;
							this.m_textInfo.characterInfo[this.m_characterCount].materialReferenceIndex = this.m_currentMaterialIndex;
							this.m_currentMaterialIndex = currentMaterialIndex;
							num6 = 0f;
						}
						else if (this.m_textElementType == TMP_TextElementType.Character)
						{
							this.m_cached_TextElement = this.m_textInfo.characterInfo[this.m_characterCount].textElement;
							if (this.m_cached_TextElement == null)
							{
								goto IL_3FF8;
							}
							this.m_currentFontAsset = this.m_textInfo.characterInfo[this.m_characterCount].fontAsset;
							this.m_currentMaterial = this.m_textInfo.characterInfo[this.m_characterCount].material;
							this.m_currentMaterialIndex = this.m_textInfo.characterInfo[this.m_characterCount].materialReferenceIndex;
							float num25;
							if (flag12 && this.m_TextProcessingArray[num15].unicode == 10U && this.m_characterCount != this.m_firstCharacterOfLine)
							{
								num25 = this.m_textInfo.characterInfo[this.m_characterCount - 1].pointSize * num17 / faceInfo.pointSize * faceInfo.scale * num;
							}
							else
							{
								num25 = this.m_currentFontSize * num17 / faceInfo.pointSize * faceInfo.scale * num;
							}
							if (flag12 && num5 == 8230U)
							{
								num19 = 0f;
								num20 = 0f;
							}
							else
							{
								num19 = faceInfo.ascentLine;
								num20 = faceInfo.descentLine;
							}
							num3 = num25 * this.m_fontScaleMultiplier * this.m_cached_TextElement.m_Scale * this.m_cached_TextElement.m_Glyph.scale;
							num18 = faceInfo.baseline * num25 * this.m_fontScaleMultiplier * faceInfo.scale;
							this.m_textInfo.characterInfo[this.m_characterCount].elementType = TMP_TextElementType.Character;
							this.m_textInfo.characterInfo[this.m_characterCount].scale = num3;
							num6 = ((this.m_currentMaterialIndex == 0) ? this.m_padding : this.m_subTextObjects[this.m_currentMaterialIndex].padding);
						}
						float num26 = num3;
						if (num5 == 173U || num5 == 3U)
						{
							num3 = 0f;
						}
						this.m_textInfo.characterInfo[this.m_characterCount].character = (char)num5;
						this.m_textInfo.characterInfo[this.m_characterCount].pointSize = this.m_currentFontSize;
						this.m_textInfo.characterInfo[this.m_characterCount].color = this.m_htmlColor;
						this.m_textInfo.characterInfo[this.m_characterCount].underlineColor = this.m_underlineColor;
						this.m_textInfo.characterInfo[this.m_characterCount].strikethroughColor = this.m_strikethroughColor;
						this.m_textInfo.characterInfo[this.m_characterCount].highlightState = this.m_HighlightState;
						this.m_textInfo.characterInfo[this.m_characterCount].style = this.m_FontStyleInternal;
						Glyph alternativeGlyph = this.m_textInfo.characterInfo[this.m_characterCount].alternativeGlyph;
						GlyphMetrics glyphMetrics = (alternativeGlyph == null) ? this.m_cached_TextElement.m_Glyph.metrics : alternativeGlyph.metrics;
						bool flag13 = num5 <= 65535U && char.IsWhiteSpace((char)num5);
						GlyphValueRecord a = default(GlyphValueRecord);
						float num27 = this.m_characterSpacing;
						if (flag5 && this.m_textElementType == TMP_TextElementType.Character)
						{
							uint glyphIndex = this.m_cached_TextElement.m_GlyphIndex;
							if (this.m_characterCount < totalCharacterCount - 1 && this.m_textInfo.characterInfo[this.m_characterCount + 1].elementType == TMP_TextElementType.Character)
							{
								uint key = this.m_textInfo.characterInfo[this.m_characterCount + 1].textElement.m_GlyphIndex << 16 | glyphIndex;
								GlyphPairAdjustmentRecord glyphPairAdjustmentRecord;
								if (this.m_currentFontAsset.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.TryGetValue(key, out glyphPairAdjustmentRecord))
								{
									a = glyphPairAdjustmentRecord.firstAdjustmentRecord.glyphValueRecord;
									num27 = (((glyphPairAdjustmentRecord.featureLookupFlags & FontFeatureLookupFlags.IgnoreSpacingAdjustments) == FontFeatureLookupFlags.IgnoreSpacingAdjustments) ? 0f : num27);
								}
							}
							if (this.m_characterCount >= 1)
							{
								uint glyphIndex2 = this.m_textInfo.characterInfo[this.m_characterCount - 1].textElement.m_GlyphIndex;
								uint key2 = glyphIndex << 16 | glyphIndex2;
								GlyphPairAdjustmentRecord glyphPairAdjustmentRecord;
								if (base.textInfo.characterInfo[this.m_characterCount - 1].elementType == TMP_TextElementType.Character && this.m_currentFontAsset.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.TryGetValue(key2, out glyphPairAdjustmentRecord))
								{
									a += glyphPairAdjustmentRecord.secondAdjustmentRecord.glyphValueRecord;
									num27 = (((glyphPairAdjustmentRecord.featureLookupFlags & FontFeatureLookupFlags.IgnoreSpacingAdjustments) == FontFeatureLookupFlags.IgnoreSpacingAdjustments) ? 0f : num27);
								}
							}
						}
						this.m_textInfo.characterInfo[this.m_characterCount].adjustedHorizontalAdvance = a.xAdvance;
						bool flag14 = TMP_TextParsingUtilities.IsBaseGlyph(num5);
						if (flag14)
						{
							this.m_LastBaseGlyphIndex = this.m_characterCount;
						}
						if (this.m_characterCount > 0 && !flag14)
						{
							if (flag6 && this.m_LastBaseGlyphIndex != -2147483648 && this.m_LastBaseGlyphIndex == this.m_characterCount - 1)
							{
								uint index = this.m_textInfo.characterInfo[this.m_LastBaseGlyphIndex].textElement.glyph.index;
								uint key3 = this.m_cached_TextElement.glyphIndex << 16 | index;
								MarkToBaseAdjustmentRecord markToBaseAdjustmentRecord;
								if (this.m_currentFontAsset.fontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.TryGetValue(key3, out markToBaseAdjustmentRecord))
								{
									float num28 = (this.m_textInfo.characterInfo[this.m_LastBaseGlyphIndex].origin - this.m_xAdvance) / num3;
									a.xPlacement = num28 + markToBaseAdjustmentRecord.baseGlyphAnchorPoint.xCoordinate - markToBaseAdjustmentRecord.markPositionAdjustment.xPositionAdjustment;
									a.yPlacement = markToBaseAdjustmentRecord.baseGlyphAnchorPoint.yCoordinate - markToBaseAdjustmentRecord.markPositionAdjustment.yPositionAdjustment;
									num27 = 0f;
								}
							}
							else
							{
								bool flag15 = false;
								if (flag7)
								{
									int num29 = this.m_characterCount - 1;
									while (num29 >= 0 && num29 != this.m_LastBaseGlyphIndex)
									{
										uint index2 = this.m_textInfo.characterInfo[num29].textElement.glyph.index;
										uint key4 = this.m_cached_TextElement.glyphIndex << 16 | index2;
										MarkToMarkAdjustmentRecord markToMarkAdjustmentRecord;
										if (this.m_currentFontAsset.fontFeatureTable.m_MarkToMarkAdjustmentRecordLookup.TryGetValue(key4, out markToMarkAdjustmentRecord))
										{
											float num30 = (this.m_textInfo.characterInfo[num29].origin - this.m_xAdvance) / num3;
											float num31 = num18 - this.m_lineOffset + this.m_baselineOffset;
											float num32 = (this.m_textInfo.characterInfo[num29].baseLine - num31) / num3;
											a.xPlacement = num30 + markToMarkAdjustmentRecord.baseMarkGlyphAnchorPoint.xCoordinate - markToMarkAdjustmentRecord.combiningMarkPositionAdjustment.xPositionAdjustment;
											a.yPlacement = num32 + markToMarkAdjustmentRecord.baseMarkGlyphAnchorPoint.yCoordinate - markToMarkAdjustmentRecord.combiningMarkPositionAdjustment.yPositionAdjustment;
											num27 = 0f;
											flag15 = true;
											break;
										}
										num29--;
									}
								}
								if (flag6 && this.m_LastBaseGlyphIndex != -2147483648 && !flag15)
								{
									uint index3 = this.m_textInfo.characterInfo[this.m_LastBaseGlyphIndex].textElement.glyph.index;
									uint key5 = this.m_cached_TextElement.glyphIndex << 16 | index3;
									MarkToBaseAdjustmentRecord markToBaseAdjustmentRecord2;
									if (this.m_currentFontAsset.fontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.TryGetValue(key5, out markToBaseAdjustmentRecord2))
									{
										float num33 = (this.m_textInfo.characterInfo[this.m_LastBaseGlyphIndex].origin - this.m_xAdvance) / num3;
										a.xPlacement = num33 + markToBaseAdjustmentRecord2.baseGlyphAnchorPoint.xCoordinate - markToBaseAdjustmentRecord2.markPositionAdjustment.xPositionAdjustment;
										a.yPlacement = markToBaseAdjustmentRecord2.baseGlyphAnchorPoint.yCoordinate - markToBaseAdjustmentRecord2.markPositionAdjustment.yPositionAdjustment;
										num27 = 0f;
									}
								}
							}
						}
						num19 += a.yPlacement;
						num20 += a.yPlacement;
						if (this.m_isRightToLeft)
						{
							this.m_xAdvance -= glyphMetrics.horizontalAdvance * (1f - this.m_charWidthAdjDelta) * num3;
							if (flag13 || num5 == 8203U)
							{
								this.m_xAdvance -= this.m_wordSpacing * num4;
							}
						}
						float num34 = 0f;
						if (this.m_monoSpacing != 0f)
						{
							if (this.m_duoSpace && (num5 == 46U || num5 == 58U || num5 == 44U))
							{
								num34 = (this.m_monoSpacing / 4f - (glyphMetrics.width / 2f + glyphMetrics.horizontalBearingX) * num3) * (1f - this.m_charWidthAdjDelta);
							}
							else
							{
								num34 = (this.m_monoSpacing / 2f - (glyphMetrics.width / 2f + glyphMetrics.horizontalBearingX) * num3) * (1f - this.m_charWidthAdjDelta);
							}
							this.m_xAdvance += num34;
						}
						float num35;
						float num36;
						if (this.m_textElementType == TMP_TextElementType.Character && !isUsingAlternateTypeface && (this.m_FontStyleInternal & FontStyles.Bold) == FontStyles.Bold)
						{
							if (this.m_currentMaterial != null && this.m_currentMaterial.HasProperty(ShaderUtilities.ID_GradientScale))
							{
								float @float = this.m_currentMaterial.GetFloat(ShaderUtilities.ID_GradientScale);
								num35 = this.m_currentFontAsset.boldStyle / 4f * @float * this.m_currentMaterial.GetFloat(ShaderUtilities.ID_ScaleRatio_A);
								if (num35 + num6 > @float)
								{
									num6 = @float - num35;
								}
							}
							else
							{
								num35 = 0f;
							}
							num36 = this.m_currentFontAsset.boldSpacing;
						}
						else
						{
							if (this.m_currentMaterial != null && this.m_currentMaterial.HasProperty(ShaderUtilities.ID_GradientScale) && this.m_currentMaterial.HasProperty(ShaderUtilities.ID_ScaleRatio_A))
							{
								float float2 = this.m_currentMaterial.GetFloat(ShaderUtilities.ID_GradientScale);
								num35 = this.m_currentFontAsset.normalStyle / 4f * float2 * this.m_currentMaterial.GetFloat(ShaderUtilities.ID_ScaleRatio_A);
								if (num35 + num6 > float2)
								{
									num6 = float2 - num35;
								}
							}
							else
							{
								num35 = 0f;
							}
							num36 = 0f;
						}
						Vector3 vector3;
						vector3.x = this.m_xAdvance + (glyphMetrics.horizontalBearingX * this.m_FXScale.x - num6 - num35 + a.xPlacement) * num3 * (1f - this.m_charWidthAdjDelta);
						vector3.y = num18 + (glyphMetrics.horizontalBearingY + num6 + a.yPlacement) * num3 - this.m_lineOffset + this.m_baselineOffset;
						vector3.z = 0f;
						Vector3 vector4;
						vector4.x = vector3.x;
						vector4.y = vector3.y - (glyphMetrics.height + num6 * 2f) * num3;
						vector4.z = 0f;
						Vector3 vector5;
						vector5.x = vector4.x + (glyphMetrics.width * this.m_FXScale.x + num6 * 2f + num35 * 2f) * num3 * (1f - this.m_charWidthAdjDelta);
						vector5.y = vector3.y;
						vector5.z = 0f;
						Vector3 vector6;
						vector6.x = vector5.x;
						vector6.y = vector4.y;
						vector6.z = 0f;
						if (this.m_textElementType == TMP_TextElementType.Character && !isUsingAlternateTypeface && (this.m_FontStyleInternal & FontStyles.Italic) == FontStyles.Italic)
						{
							float num37 = (float)this.m_ItalicAngle * 0.01f;
							float num38 = (this.m_currentFontAsset.m_FaceInfo.capLine - (this.m_currentFontAsset.m_FaceInfo.baseline + this.m_baselineOffset)) / 2f * this.m_fontScaleMultiplier * this.m_currentFontAsset.m_FaceInfo.scale;
							Vector3 b = new Vector3(num37 * ((glyphMetrics.horizontalBearingY + num6 + num35 - num38) * num3), 0f, 0f);
							Vector3 b2 = new Vector3(num37 * ((glyphMetrics.horizontalBearingY - glyphMetrics.height - num6 - num35 - num38) * num3), 0f, 0f);
							vector3 += b;
							vector4 += b2;
							vector5 += b;
							vector6 += b2;
						}
						if (this.m_FXRotation != Quaternion.identity)
						{
							Matrix4x4 matrix4x = Matrix4x4.Rotate(this.m_FXRotation);
							Vector3 b3 = (vector5 + vector4) / 2f;
							vector3 = matrix4x.MultiplyPoint3x4(vector3 - b3) + b3;
							vector4 = matrix4x.MultiplyPoint3x4(vector4 - b3) + b3;
							vector5 = matrix4x.MultiplyPoint3x4(vector5 - b3) + b3;
							vector6 = matrix4x.MultiplyPoint3x4(vector6 - b3) + b3;
						}
						this.m_textInfo.characterInfo[this.m_characterCount].bottomLeft = vector4;
						this.m_textInfo.characterInfo[this.m_characterCount].topLeft = vector3;
						this.m_textInfo.characterInfo[this.m_characterCount].topRight = vector5;
						this.m_textInfo.characterInfo[this.m_characterCount].bottomRight = vector6;
						this.m_textInfo.characterInfo[this.m_characterCount].origin = this.m_xAdvance + a.xPlacement * num3;
						this.m_textInfo.characterInfo[this.m_characterCount].baseLine = num18 - this.m_lineOffset + this.m_baselineOffset + a.yPlacement * num3;
						this.m_textInfo.characterInfo[this.m_characterCount].aspectRatio = (vector5.x - vector4.x) / (vector3.y - vector4.y);
						float num39 = (this.m_textElementType == TMP_TextElementType.Character) ? (num19 * num3 / num17 + this.m_baselineOffset) : (num19 * num3 + this.m_baselineOffset);
						float num40 = (this.m_textElementType == TMP_TextElementType.Character) ? (num20 * num3 / num17 + this.m_baselineOffset) : (num20 * num3 + this.m_baselineOffset);
						float num41 = num39;
						float num42 = num40;
						bool flag16 = this.m_characterCount == this.m_firstCharacterOfLine;
						if (flag16 || !flag13)
						{
							if (this.m_baselineOffset != 0f)
							{
								num41 = Mathf.Max((num39 - this.m_baselineOffset) / this.m_fontScaleMultiplier, num41);
								num42 = Mathf.Min((num40 - this.m_baselineOffset) / this.m_fontScaleMultiplier, num42);
							}
							this.m_maxLineAscender = Mathf.Max(num41, this.m_maxLineAscender);
							this.m_maxLineDescender = Mathf.Min(num42, this.m_maxLineDescender);
						}
						if (flag16 || !flag13)
						{
							this.m_textInfo.characterInfo[this.m_characterCount].adjustedAscender = num41;
							this.m_textInfo.characterInfo[this.m_characterCount].adjustedDescender = num42;
							this.m_ElementAscender = (this.m_textInfo.characterInfo[this.m_characterCount].ascender = num39 - this.m_lineOffset);
							this.m_ElementDescender = (this.m_textInfo.characterInfo[this.m_characterCount].descender = num40 - this.m_lineOffset);
						}
						else
						{
							this.m_textInfo.characterInfo[this.m_characterCount].adjustedAscender = this.m_maxLineAscender;
							this.m_textInfo.characterInfo[this.m_characterCount].adjustedDescender = this.m_maxLineDescender;
							this.m_ElementAscender = (this.m_textInfo.characterInfo[this.m_characterCount].ascender = this.m_maxLineAscender - this.m_lineOffset);
							this.m_ElementDescender = (this.m_textInfo.characterInfo[this.m_characterCount].descender = this.m_maxLineDescender - this.m_lineOffset);
						}
						if ((this.m_lineNumber == 0 || this.m_isNewPage) && (flag16 || !flag13))
						{
							this.m_maxTextAscender = this.m_maxLineAscender;
							this.m_maxCapHeight = Mathf.Max(this.m_maxCapHeight, this.m_currentFontAsset.m_FaceInfo.capLine * num3 / num17);
						}
						if (this.m_lineOffset == 0f && (flag16 || !flag13))
						{
							this.m_PageAscender = ((this.m_PageAscender > num39) ? this.m_PageAscender : num39);
						}
						this.m_textInfo.characterInfo[this.m_characterCount].isVisible = false;
						bool flag17 = (this.m_lineJustification & HorizontalAlignmentOptions.Flush) == HorizontalAlignmentOptions.Flush || (this.m_lineJustification & HorizontalAlignmentOptions.Justified) == HorizontalAlignmentOptions.Justified;
						if (num5 == 9U || ((this.m_TextWrappingMode == TextWrappingModes.PreserveWhitespace || this.m_TextWrappingMode == TextWrappingModes.PreserveWhitespaceNoWrap) && (flag13 || num5 == 8203U)) || (!flag13 && num5 != 8203U && num5 != 173U && num5 != 3U) || (num5 == 173U && !flag11) || this.m_textElementType == TMP_TextElementType.Sprite)
						{
							this.m_textInfo.characterInfo[this.m_characterCount].isVisible = true;
							float marginLeft = this.m_marginLeft;
							float marginRight = this.m_marginRight;
							if (flag12)
							{
								marginLeft = this.m_textInfo.lineInfo[this.m_lineNumber].marginLeft;
								marginRight = this.m_textInfo.lineInfo[this.m_lineNumber].marginRight;
							}
							num11 = ((this.m_width != -1f) ? Mathf.Min(num9 + 0.0001f - marginLeft - marginRight, this.m_width) : (num9 + 0.0001f - marginLeft - marginRight));
							float num43 = Mathf.Abs(this.m_xAdvance) + ((!this.m_isRightToLeft) ? glyphMetrics.horizontalAdvance : 0f) * (1f - this.m_charWidthAdjDelta) * ((num5 == 173U) ? num26 : num3);
							float num44 = this.m_maxTextAscender - (this.m_maxLineDescender - this.m_lineOffset) + ((this.m_lineOffset > 0f && !this.m_IsDrivenLineSpacing) ? (this.m_maxLineAscender - this.m_startOfLineAscender) : 0f);
							int characterCount = this.m_characterCount;
							if (num44 > num10 + 0.0001f)
							{
								if (this.m_firstOverflowCharacterIndex == -1)
								{
									this.m_firstOverflowCharacterIndex = this.m_characterCount;
								}
								if (this.m_enableAutoSizing)
								{
									if (this.m_lineSpacingDelta > this.m_lineSpacingMax && this.m_lineOffset > 0f && this.m_AutoSizeIterationCount < this.m_AutoSizeMaxIterationCount)
									{
										float num45 = (num10 - num44) / (float)this.m_lineNumber;
										this.m_lineSpacingDelta = Mathf.Max(this.m_lineSpacingDelta + num45 / num2, this.m_lineSpacingMax);
										return;
									}
									if (this.m_fontSize > this.m_fontSizeMin && this.m_AutoSizeIterationCount < this.m_AutoSizeMaxIterationCount)
									{
										this.m_maxFontSize = this.m_fontSize;
										float num46 = Mathf.Max((this.m_fontSize - this.m_minFontSize) / 2f, 0.05f);
										this.m_fontSize -= num46;
										this.m_fontSize = Mathf.Max((float)((int)(this.m_fontSize * 20f + 0.5f)) / 20f, this.m_fontSizeMin);
										return;
									}
								}
								switch (this.m_overflowMode)
								{
								case TextOverflowModes.Ellipsis:
								{
									if (TMP_Text.m_EllipsisInsertionCandidateStack.Count == 0)
									{
										num15 = -1;
										this.m_characterCount = 0;
										characterSubstitution.index = 0;
										characterSubstitution.unicode = 3U;
										this.m_firstCharacterOfLine = 0;
										goto IL_3FF8;
									}
									WordWrapState wordWrapState = TMP_Text.m_EllipsisInsertionCandidateStack.Pop();
									num15 = base.RestoreWordWrappingState(ref wordWrapState);
									num15--;
									this.m_characterCount--;
									characterSubstitution.index = this.m_characterCount;
									characterSubstitution.unicode = 8230U;
									num14++;
									goto IL_3FF8;
								}
								case TextOverflowModes.Truncate:
									num15 = base.RestoreWordWrappingState(ref TMP_Text.m_SavedLastValidState);
									characterSubstitution.index = characterCount;
									characterSubstitution.unicode = 3U;
									goto IL_3FF8;
								case TextOverflowModes.Page:
									if (num15 < 0 || characterCount == 0)
									{
										num15 = -1;
										this.m_characterCount = 0;
										characterSubstitution.index = 0;
										characterSubstitution.unicode = 3U;
										goto IL_3FF8;
									}
									if (this.m_maxLineAscender - this.m_maxLineDescender > num10 + 0.0001f)
									{
										num15 = base.RestoreWordWrappingState(ref TMP_Text.m_SavedLineState);
										characterSubstitution.index = characterCount;
										characterSubstitution.unicode = 3U;
										goto IL_3FF8;
									}
									num15 = base.RestoreWordWrappingState(ref TMP_Text.m_SavedLineState);
									this.m_isNewPage = true;
									this.m_firstCharacterOfLine = this.m_characterCount;
									this.m_maxLineAscender = TMP_Text.k_LargeNegativeFloat;
									this.m_maxLineDescender = TMP_Text.k_LargePositiveFloat;
									this.m_startOfLineAscender = 0f;
									this.m_xAdvance = 0f + this.tag_Indent;
									this.m_lineOffset = 0f;
									this.m_maxTextAscender = 0f;
									this.m_PageAscender = 0f;
									this.m_lineNumber++;
									this.m_pageNumber++;
									goto IL_3FF8;
								case TextOverflowModes.Linked:
									num15 = base.RestoreWordWrappingState(ref TMP_Text.m_SavedLastValidState);
									if (this.m_linkedTextComponent != null)
									{
										this.m_linkedTextComponent.text = this.text;
										this.m_linkedTextComponent.m_inputSource = this.m_inputSource;
										this.m_linkedTextComponent.firstVisibleCharacter = this.m_characterCount;
										this.m_linkedTextComponent.ForceMeshUpdate(false, false);
										this.m_isTextTruncated = true;
									}
									characterSubstitution.index = characterCount;
									characterSubstitution.unicode = 3U;
									goto IL_3FF8;
								}
							}
							if (flag14 && num43 > num11 * (flag17 ? 1.05f : 1f))
							{
								if (this.m_TextWrappingMode != TextWrappingModes.NoWrap && this.m_TextWrappingMode != TextWrappingModes.PreserveWhitespaceNoWrap && this.m_characterCount != this.m_firstCharacterOfLine)
								{
									num15 = base.RestoreWordWrappingState(ref TMP_Text.m_SavedWordWrapState);
									float num47;
									if (this.m_lineHeight == -32767f)
									{
										float adjustedAscender = this.m_textInfo.characterInfo[this.m_characterCount].adjustedAscender;
										num47 = ((this.m_lineOffset > 0f && !this.m_IsDrivenLineSpacing) ? (this.m_maxLineAscender - this.m_startOfLineAscender) : 0f) - this.m_maxLineDescender + adjustedAscender + (num7 + this.m_lineSpacingDelta) * num2 + this.m_lineSpacing * num4;
									}
									else
									{
										num47 = this.m_lineHeight + this.m_lineSpacing * num4;
										this.m_IsDrivenLineSpacing = true;
									}
									float num48 = this.m_maxTextAscender + num47 + this.m_lineOffset - this.m_textInfo.characterInfo[this.m_characterCount].adjustedDescender;
									if (this.m_textInfo.characterInfo[this.m_characterCount - 1].character == '­' && !flag11 && (this.m_overflowMode == TextOverflowModes.Overflow || num48 < num10 + 0.0001f))
									{
										characterSubstitution.index = this.m_characterCount - 1;
										characterSubstitution.unicode = 45U;
										num15--;
										this.m_characterCount--;
										goto IL_3FF8;
									}
									flag11 = false;
									if (this.m_textInfo.characterInfo[this.m_characterCount].character == '­')
									{
										flag11 = true;
										goto IL_3FF8;
									}
									if (this.m_enableAutoSizing && flag9)
									{
										if (this.m_charWidthAdjDelta < this.m_charWidthMaxAdj / 100f && this.m_AutoSizeIterationCount < this.m_AutoSizeMaxIterationCount)
										{
											float num49 = num43;
											if (this.m_charWidthAdjDelta > 0f)
											{
												num49 /= 1f - this.m_charWidthAdjDelta;
											}
											float num50 = num43 - (num11 - 0.0001f) * (flag17 ? 1.05f : 1f);
											this.m_charWidthAdjDelta += num50 / num49;
											this.m_charWidthAdjDelta = Mathf.Min(this.m_charWidthAdjDelta, this.m_charWidthMaxAdj / 100f);
											return;
										}
										if (this.m_fontSize > this.m_fontSizeMin && this.m_AutoSizeIterationCount < this.m_AutoSizeMaxIterationCount)
										{
											this.m_maxFontSize = this.m_fontSize;
											float num51 = Mathf.Max((this.m_fontSize - this.m_minFontSize) / 2f, 0.05f);
											this.m_fontSize -= num51;
											this.m_fontSize = Mathf.Max((float)((int)(this.m_fontSize * 20f + 0.5f)) / 20f, this.m_fontSizeMin);
											return;
										}
									}
									int previous_WordBreak = TMP_Text.m_SavedSoftLineBreakState.previous_WordBreak;
									if (flag9 && previous_WordBreak != -1 && previous_WordBreak != num13)
									{
										num15 = base.RestoreWordWrappingState(ref TMP_Text.m_SavedSoftLineBreakState);
										num13 = previous_WordBreak;
										if (this.m_textInfo.characterInfo[this.m_characterCount - 1].character == '­')
										{
											characterSubstitution.index = this.m_characterCount - 1;
											characterSubstitution.unicode = 45U;
											num15--;
											this.m_characterCount--;
											goto IL_3FF8;
										}
									}
									if (num48 <= num10 + 0.0001f)
									{
										base.InsertNewLine(num15, num2, num3, num4, num36, num27, num11, num7, ref flag8, ref num12);
										flag4 = true;
										flag9 = true;
										goto IL_3FF8;
									}
									if (this.m_firstOverflowCharacterIndex == -1)
									{
										this.m_firstOverflowCharacterIndex = this.m_characterCount;
									}
									if (this.m_enableAutoSizing)
									{
										if (this.m_lineSpacingDelta > this.m_lineSpacingMax && this.m_AutoSizeIterationCount < this.m_AutoSizeMaxIterationCount)
										{
											float num52 = (num10 - num48) / (float)(this.m_lineNumber + 1);
											this.m_lineSpacingDelta = Mathf.Max(this.m_lineSpacingDelta + num52 / num2, this.m_lineSpacingMax);
											return;
										}
										if (this.m_charWidthAdjDelta < this.m_charWidthMaxAdj / 100f && this.m_AutoSizeIterationCount < this.m_AutoSizeMaxIterationCount)
										{
											float num53 = num43;
											if (this.m_charWidthAdjDelta > 0f)
											{
												num53 /= 1f - this.m_charWidthAdjDelta;
											}
											float num54 = num43 - (num11 - 0.0001f) * (flag17 ? 1.05f : 1f);
											this.m_charWidthAdjDelta += num54 / num53;
											this.m_charWidthAdjDelta = Mathf.Min(this.m_charWidthAdjDelta, this.m_charWidthMaxAdj / 100f);
											return;
										}
										if (this.m_fontSize > this.m_fontSizeMin && this.m_AutoSizeIterationCount < this.m_AutoSizeMaxIterationCount)
										{
											this.m_maxFontSize = this.m_fontSize;
											float num55 = Mathf.Max((this.m_fontSize - this.m_minFontSize) / 2f, 0.05f);
											this.m_fontSize -= num55;
											this.m_fontSize = Mathf.Max((float)((int)(this.m_fontSize * 20f + 0.5f)) / 20f, this.m_fontSizeMin);
											return;
										}
									}
									switch (this.m_overflowMode)
									{
									case TextOverflowModes.Overflow:
									case TextOverflowModes.Masking:
									case TextOverflowModes.ScrollRect:
										base.InsertNewLine(num15, num2, num3, num4, num36, num27, num11, num7, ref flag8, ref num12);
										flag4 = true;
										flag9 = true;
										goto IL_3FF8;
									case TextOverflowModes.Ellipsis:
									{
										if (TMP_Text.m_EllipsisInsertionCandidateStack.Count == 0)
										{
											num15 = -1;
											this.m_characterCount = 0;
											characterSubstitution.index = 0;
											characterSubstitution.unicode = 3U;
											this.m_firstCharacterOfLine = 0;
											goto IL_3FF8;
										}
										WordWrapState wordWrapState2 = TMP_Text.m_EllipsisInsertionCandidateStack.Pop();
										num15 = base.RestoreWordWrappingState(ref wordWrapState2);
										num15--;
										this.m_characterCount--;
										characterSubstitution.index = this.m_characterCount;
										characterSubstitution.unicode = 8230U;
										num14++;
										goto IL_3FF8;
									}
									case TextOverflowModes.Truncate:
										num15 = base.RestoreWordWrappingState(ref TMP_Text.m_SavedLastValidState);
										characterSubstitution.index = characterCount;
										characterSubstitution.unicode = 3U;
										goto IL_3FF8;
									case TextOverflowModes.Page:
										this.m_isNewPage = true;
										base.InsertNewLine(num15, num2, num3, num4, num36, num27, num11, num7, ref flag8, ref num12);
										this.m_startOfLineAscender = 0f;
										this.m_lineOffset = 0f;
										this.m_maxTextAscender = 0f;
										this.m_PageAscender = 0f;
										this.m_pageNumber++;
										flag4 = true;
										flag9 = true;
										goto IL_3FF8;
									case TextOverflowModes.Linked:
										if (this.m_linkedTextComponent != null)
										{
											this.m_linkedTextComponent.text = this.text;
											this.m_linkedTextComponent.m_inputSource = this.m_inputSource;
											this.m_linkedTextComponent.firstVisibleCharacter = this.m_characterCount;
											this.m_linkedTextComponent.ForceMeshUpdate(false, false);
											this.m_isTextTruncated = true;
										}
										characterSubstitution.index = this.m_characterCount;
										characterSubstitution.unicode = 3U;
										goto IL_3FF8;
									}
								}
								else
								{
									if (this.m_enableAutoSizing && this.m_AutoSizeIterationCount < this.m_AutoSizeMaxIterationCount)
									{
										if (this.m_charWidthAdjDelta < this.m_charWidthMaxAdj / 100f)
										{
											float num56 = num43;
											if (this.m_charWidthAdjDelta > 0f)
											{
												num56 /= 1f - this.m_charWidthAdjDelta;
											}
											float num57 = num43 - (num11 - 0.0001f) * (flag17 ? 1.05f : 1f);
											this.m_charWidthAdjDelta += num57 / num56;
											this.m_charWidthAdjDelta = Mathf.Min(this.m_charWidthAdjDelta, this.m_charWidthMaxAdj / 100f);
											return;
										}
										if (this.m_fontSize > this.m_fontSizeMin)
										{
											this.m_maxFontSize = this.m_fontSize;
											float num58 = Mathf.Max((this.m_fontSize - this.m_minFontSize) / 2f, 0.05f);
											this.m_fontSize -= num58;
											this.m_fontSize = Mathf.Max((float)((int)(this.m_fontSize * 20f + 0.5f)) / 20f, this.m_fontSizeMin);
											return;
										}
									}
									switch (this.m_overflowMode)
									{
									case TextOverflowModes.Ellipsis:
									{
										if (TMP_Text.m_EllipsisInsertionCandidateStack.Count == 0)
										{
											num15 = -1;
											this.m_characterCount = 0;
											characterSubstitution.index = 0;
											characterSubstitution.unicode = 3U;
											this.m_firstCharacterOfLine = 0;
											goto IL_3FF8;
										}
										WordWrapState wordWrapState3 = TMP_Text.m_EllipsisInsertionCandidateStack.Pop();
										num15 = base.RestoreWordWrappingState(ref wordWrapState3);
										num15--;
										this.m_characterCount--;
										characterSubstitution.index = this.m_characterCount;
										characterSubstitution.unicode = 8230U;
										num14++;
										goto IL_3FF8;
									}
									case TextOverflowModes.Truncate:
										num15 = base.RestoreWordWrappingState(ref TMP_Text.m_SavedWordWrapState);
										characterSubstitution.index = characterCount;
										characterSubstitution.unicode = 3U;
										goto IL_3FF8;
									case TextOverflowModes.Linked:
										num15 = base.RestoreWordWrappingState(ref TMP_Text.m_SavedWordWrapState);
										if (this.m_linkedTextComponent != null)
										{
											this.m_linkedTextComponent.text = this.text;
											this.m_linkedTextComponent.m_inputSource = this.m_inputSource;
											this.m_linkedTextComponent.firstVisibleCharacter = this.m_characterCount;
											this.m_linkedTextComponent.ForceMeshUpdate(false, false);
											this.m_isTextTruncated = true;
										}
										characterSubstitution.index = this.m_characterCount;
										characterSubstitution.unicode = 3U;
										goto IL_3FF8;
									}
								}
							}
							if (flag13)
							{
								this.m_textInfo.characterInfo[this.m_characterCount].isVisible = false;
								this.m_lastVisibleCharacterOfLine = this.m_characterCount;
								TMP_LineInfo[] lineInfo = this.m_textInfo.lineInfo;
								int lineNumber = this.m_lineNumber;
								this.m_lineVisibleSpaceCount = (lineInfo[lineNumber].spaceCount = lineInfo[lineNumber].spaceCount + 1);
								this.m_textInfo.lineInfo[this.m_lineNumber].marginLeft = marginLeft;
								this.m_textInfo.lineInfo[this.m_lineNumber].marginRight = marginRight;
								this.m_textInfo.spaceCount++;
								if (num5 == 160U)
								{
									TMP_LineInfo[] lineInfo2 = this.m_textInfo.lineInfo;
									int lineNumber2 = this.m_lineNumber;
									lineInfo2[lineNumber2].controlCharacterCount = lineInfo2[lineNumber2].controlCharacterCount + 1;
								}
							}
							else if (num5 == 173U)
							{
								this.m_textInfo.characterInfo[this.m_characterCount].isVisible = false;
							}
							else
							{
								Color32 vertexColor;
								if (this.m_overrideHtmlColors)
								{
									vertexColor = this.m_fontColor32;
								}
								else
								{
									vertexColor = this.m_htmlColor;
								}
								if (this.m_textElementType == TMP_TextElementType.Character)
								{
									this.SaveGlyphVertexInfo(num6, num35, vertexColor);
								}
								else if (this.m_textElementType == TMP_TextElementType.Sprite)
								{
									this.SaveSpriteVertexInfo(vertexColor);
								}
								if (flag4)
								{
									flag4 = false;
									this.m_firstVisibleCharacterOfLine = this.m_characterCount;
								}
								this.m_lineVisibleCharacterCount++;
								this.m_lastVisibleCharacterOfLine = this.m_characterCount;
								this.m_textInfo.lineInfo[this.m_lineNumber].marginLeft = marginLeft;
								this.m_textInfo.lineInfo[this.m_lineNumber].marginRight = marginRight;
							}
						}
						else
						{
							if (this.m_overflowMode == TextOverflowModes.Linked && (num5 == 10U || num5 == 11U))
							{
								float num59 = this.m_maxTextAscender - (this.m_maxLineDescender - this.m_lineOffset) + ((this.m_lineOffset > 0f && !this.m_IsDrivenLineSpacing) ? (this.m_maxLineAscender - this.m_startOfLineAscender) : 0f);
								int characterCount2 = this.m_characterCount;
								if (num59 > num10 + 0.0001f)
								{
									if (this.m_firstOverflowCharacterIndex == -1)
									{
										this.m_firstOverflowCharacterIndex = this.m_characterCount;
									}
									num15 = base.RestoreWordWrappingState(ref TMP_Text.m_SavedLastValidState);
									if (this.m_linkedTextComponent != null)
									{
										this.m_linkedTextComponent.text = this.text;
										this.m_linkedTextComponent.m_inputSource = this.m_inputSource;
										this.m_linkedTextComponent.firstVisibleCharacter = this.m_characterCount;
										this.m_linkedTextComponent.ForceMeshUpdate(false, false);
										this.m_isTextTruncated = true;
									}
									characterSubstitution.index = characterCount2;
									characterSubstitution.unicode = 3U;
									goto IL_3FF8;
								}
							}
							if ((num5 == 10U || num5 == 11U || num5 == 160U || num5 == 8199U || num5 == 8232U || num5 == 8233U || char.IsSeparator((char)num5)) && num5 != 173U && num5 != 8203U && num5 != 8288U)
							{
								TMP_LineInfo[] lineInfo3 = this.m_textInfo.lineInfo;
								int lineNumber3 = this.m_lineNumber;
								lineInfo3[lineNumber3].spaceCount = lineInfo3[lineNumber3].spaceCount + 1;
								this.m_textInfo.spaceCount++;
							}
							if (num5 == 160U)
							{
								TMP_LineInfo[] lineInfo4 = this.m_textInfo.lineInfo;
								int lineNumber4 = this.m_lineNumber;
								lineInfo4[lineNumber4].controlCharacterCount = lineInfo4[lineNumber4].controlCharacterCount + 1;
							}
						}
						if (this.m_overflowMode == TextOverflowModes.Ellipsis && (!flag12 || num5 == 45U))
						{
							float num60 = this.m_currentFontSize / this.m_Ellipsis.fontAsset.m_FaceInfo.pointSize * this.m_Ellipsis.fontAsset.m_FaceInfo.scale * num * this.m_fontScaleMultiplier * this.m_Ellipsis.character.m_Scale * this.m_Ellipsis.character.m_Glyph.scale;
							float marginLeft2 = this.m_marginLeft;
							float marginRight2 = this.m_marginRight;
							if (num5 == 10U && this.m_characterCount != this.m_firstCharacterOfLine)
							{
								num60 = this.m_textInfo.characterInfo[this.m_characterCount - 1].pointSize / this.m_Ellipsis.fontAsset.m_FaceInfo.pointSize * this.m_Ellipsis.fontAsset.m_FaceInfo.scale * num * this.m_fontScaleMultiplier * this.m_Ellipsis.character.m_Scale * this.m_Ellipsis.character.m_Glyph.scale;
								marginLeft2 = this.m_textInfo.lineInfo[this.m_lineNumber].marginLeft;
								marginRight2 = this.m_textInfo.lineInfo[this.m_lineNumber].marginRight;
							}
							float num61 = this.m_maxTextAscender - (this.m_maxLineDescender - this.m_lineOffset) + ((this.m_lineOffset > 0f && !this.m_IsDrivenLineSpacing) ? (this.m_maxLineAscender - this.m_startOfLineAscender) : 0f);
							float num62 = Mathf.Abs(this.m_xAdvance) + ((!this.m_isRightToLeft) ? this.m_Ellipsis.character.m_Glyph.metrics.horizontalAdvance : 0f) * (1f - this.m_charWidthAdjDelta) * num60;
							float num63 = (this.m_width != -1f) ? Mathf.Min(num9 + 0.0001f - marginLeft2 - marginRight2, this.m_width) : (num9 + 0.0001f - marginLeft2 - marginRight2);
							if (num62 < num63 * (flag17 ? 1.05f : 1f) && num61 < num10 + 0.0001f)
							{
								base.SaveWordWrappingState(ref TMP_Text.m_SavedEllipsisState, num15, this.m_characterCount);
								TMP_Text.m_EllipsisInsertionCandidateStack.Push(TMP_Text.m_SavedEllipsisState);
							}
						}
						this.m_textInfo.characterInfo[this.m_characterCount].lineNumber = this.m_lineNumber;
						this.m_textInfo.characterInfo[this.m_characterCount].pageNumber = this.m_pageNumber;
						if ((num5 != 10U && num5 != 11U && num5 != 13U && !flag12) || this.m_textInfo.lineInfo[this.m_lineNumber].characterCount == 1)
						{
							this.m_textInfo.lineInfo[this.m_lineNumber].alignment = this.m_lineJustification;
						}
						if (num5 == 9U)
						{
							float num64 = this.m_currentFontAsset.m_FaceInfo.tabWidth * (float)this.m_currentFontAsset.tabSize * num3;
							if (this.m_isRightToLeft)
							{
								float num65 = Mathf.Floor(this.m_xAdvance / num64) * num64;
								this.m_xAdvance = ((num65 < this.m_xAdvance) ? num65 : (this.m_xAdvance - num64));
							}
							else
							{
								float num66 = Mathf.Ceil(this.m_xAdvance / num64) * num64;
								this.m_xAdvance = ((num66 > this.m_xAdvance) ? num66 : (this.m_xAdvance + num64));
							}
						}
						else if (this.m_monoSpacing != 0f)
						{
							float num67;
							if (this.m_duoSpace && (num5 == 46U || num5 == 58U || num5 == 44U))
							{
								num67 = this.m_monoSpacing / 2f - num34;
							}
							else
							{
								num67 = this.m_monoSpacing - num34;
							}
							this.m_xAdvance += (num67 + (this.m_currentFontAsset.normalSpacingOffset + num27) * num4 + this.m_cSpacing) * (1f - this.m_charWidthAdjDelta);
							if (flag13 || num5 == 8203U)
							{
								this.m_xAdvance += this.m_wordSpacing * num4;
							}
						}
						else if (this.m_isRightToLeft)
						{
							this.m_xAdvance -= (a.xAdvance * num3 + (this.m_currentFontAsset.normalSpacingOffset + num27 + num36) * num4 + this.m_cSpacing) * (1f - this.m_charWidthAdjDelta);
							if (flag13 || num5 == 8203U)
							{
								this.m_xAdvance -= this.m_wordSpacing * num4;
							}
						}
						else
						{
							this.m_xAdvance += ((glyphMetrics.horizontalAdvance * this.m_FXScale.x + a.xAdvance) * num3 + (this.m_currentFontAsset.normalSpacingOffset + num27 + num36) * num4 + this.m_cSpacing) * (1f - this.m_charWidthAdjDelta);
							if (flag13 || num5 == 8203U)
							{
								this.m_xAdvance += this.m_wordSpacing * num4;
							}
						}
						this.m_textInfo.characterInfo[this.m_characterCount].xAdvance = this.m_xAdvance;
						if (num5 == 13U)
						{
							this.m_xAdvance = 0f + this.tag_Indent;
						}
						if (this.m_overflowMode == TextOverflowModes.Page && num5 != 10U && num5 != 11U && num5 != 13U && num5 != 8232U && num5 != 8233U)
						{
							if (this.m_pageNumber + 1 > this.m_textInfo.pageInfo.Length)
							{
								TMP_TextInfo.Resize<TMP_PageInfo>(ref this.m_textInfo.pageInfo, this.m_pageNumber + 1, true);
							}
							this.m_textInfo.pageInfo[this.m_pageNumber].ascender = this.m_PageAscender;
							this.m_textInfo.pageInfo[this.m_pageNumber].descender = ((this.m_ElementDescender < this.m_textInfo.pageInfo[this.m_pageNumber].descender) ? this.m_ElementDescender : this.m_textInfo.pageInfo[this.m_pageNumber].descender);
							if (this.m_isNewPage)
							{
								this.m_isNewPage = false;
								this.m_textInfo.pageInfo[this.m_pageNumber].firstCharacterIndex = this.m_characterCount;
							}
							this.m_textInfo.pageInfo[this.m_pageNumber].lastCharacterIndex = this.m_characterCount;
						}
						if (num5 == 10U || num5 == 11U || num5 == 3U || num5 == 8232U || num5 == 8233U || (num5 == 45U && flag12) || this.m_characterCount == totalCharacterCount - 1)
						{
							float num68 = this.m_maxLineAscender - this.m_startOfLineAscender;
							if (this.m_lineOffset > 0f && Math.Abs(num68) > 0.01f && !this.m_IsDrivenLineSpacing && !this.m_isNewPage)
							{
								base.AdjustLineOffset(this.m_firstCharacterOfLine, this.m_characterCount, num68);
								this.m_ElementDescender -= num68;
								this.m_lineOffset += num68;
								if (TMP_Text.m_SavedEllipsisState.lineNumber == this.m_lineNumber)
								{
									TMP_Text.m_SavedEllipsisState = TMP_Text.m_EllipsisInsertionCandidateStack.Pop();
									TMP_Text.m_SavedEllipsisState.startOfLineAscender = TMP_Text.m_SavedEllipsisState.startOfLineAscender + num68;
									TMP_Text.m_SavedEllipsisState.lineOffset = TMP_Text.m_SavedEllipsisState.lineOffset + num68;
									TMP_Text.m_EllipsisInsertionCandidateStack.Push(TMP_Text.m_SavedEllipsisState);
								}
							}
							this.m_isNewPage = false;
							float num69 = this.m_maxLineAscender - this.m_lineOffset;
							float num70 = this.m_maxLineDescender - this.m_lineOffset;
							this.m_ElementDescender = ((this.m_ElementDescender < num70) ? this.m_ElementDescender : num70);
							if (!flag8)
							{
								num12 = this.m_ElementDescender;
							}
							if (this.m_useMaxVisibleDescender && (this.m_characterCount >= this.m_maxVisibleCharacters || this.m_lineNumber >= this.m_maxVisibleLines))
							{
								flag8 = true;
							}
							this.m_textInfo.lineInfo[this.m_lineNumber].firstCharacterIndex = this.m_firstCharacterOfLine;
							this.m_textInfo.lineInfo[this.m_lineNumber].firstVisibleCharacterIndex = (this.m_firstVisibleCharacterOfLine = ((this.m_firstCharacterOfLine > this.m_firstVisibleCharacterOfLine) ? this.m_firstCharacterOfLine : this.m_firstVisibleCharacterOfLine));
							this.m_textInfo.lineInfo[this.m_lineNumber].lastCharacterIndex = (this.m_lastCharacterOfLine = this.m_characterCount);
							this.m_textInfo.lineInfo[this.m_lineNumber].lastVisibleCharacterIndex = (this.m_lastVisibleCharacterOfLine = ((this.m_lastVisibleCharacterOfLine < this.m_firstVisibleCharacterOfLine) ? this.m_firstVisibleCharacterOfLine : this.m_lastVisibleCharacterOfLine));
							this.m_textInfo.lineInfo[this.m_lineNumber].characterCount = this.m_textInfo.lineInfo[this.m_lineNumber].lastCharacterIndex - this.m_textInfo.lineInfo[this.m_lineNumber].firstCharacterIndex + 1;
							this.m_textInfo.lineInfo[this.m_lineNumber].visibleCharacterCount = this.m_lineVisibleCharacterCount;
							this.m_textInfo.lineInfo[this.m_lineNumber].visibleSpaceCount = this.m_textInfo.lineInfo[this.m_lineNumber].lastVisibleCharacterIndex + 1 - this.m_textInfo.lineInfo[this.m_lineNumber].firstCharacterIndex - this.m_lineVisibleCharacterCount;
							this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.min = new Vector2(this.m_textInfo.characterInfo[this.m_firstVisibleCharacterOfLine].bottomLeft.x, num70);
							this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.max = new Vector2(this.m_textInfo.characterInfo[this.m_lastVisibleCharacterOfLine].topRight.x, num69);
							this.m_textInfo.lineInfo[this.m_lineNumber].length = this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.max.x - num6 * num3;
							this.m_textInfo.lineInfo[this.m_lineNumber].width = num11;
							if (this.m_textInfo.lineInfo[this.m_lineNumber].characterCount == 1)
							{
								this.m_textInfo.lineInfo[this.m_lineNumber].alignment = this.m_lineJustification;
							}
							float num71 = ((this.m_currentFontAsset.normalSpacingOffset + num27 + num36) * num4 + this.m_cSpacing) * (1f - this.m_charWidthAdjDelta);
							if (this.m_textInfo.characterInfo[this.m_lastVisibleCharacterOfLine].isVisible)
							{
								this.m_textInfo.lineInfo[this.m_lineNumber].maxAdvance = this.m_textInfo.characterInfo[this.m_lastVisibleCharacterOfLine].xAdvance + (this.m_isRightToLeft ? num71 : (-num71));
							}
							else
							{
								this.m_textInfo.lineInfo[this.m_lineNumber].maxAdvance = this.m_textInfo.characterInfo[this.m_lastCharacterOfLine].xAdvance + (this.m_isRightToLeft ? num71 : (-num71));
							}
							this.m_textInfo.lineInfo[this.m_lineNumber].baseline = 0f - this.m_lineOffset;
							this.m_textInfo.lineInfo[this.m_lineNumber].ascender = num69;
							this.m_textInfo.lineInfo[this.m_lineNumber].descender = num70;
							this.m_textInfo.lineInfo[this.m_lineNumber].lineHeight = num69 - num70 + num7 * num2;
							if (num5 == 10U || num5 == 11U || (num5 == 45U && flag12) || num5 == 8232U || num5 == 8233U)
							{
								base.SaveWordWrappingState(ref TMP_Text.m_SavedLineState, num15, this.m_characterCount);
								this.m_lineNumber++;
								flag4 = true;
								flag10 = false;
								flag9 = true;
								this.m_firstCharacterOfLine = this.m_characterCount + 1;
								this.m_lineVisibleCharacterCount = 0;
								this.m_lineVisibleSpaceCount = 0;
								if (this.m_lineNumber >= this.m_textInfo.lineInfo.Length)
								{
									base.ResizeLineExtents(this.m_lineNumber);
								}
								float adjustedAscender2 = this.m_textInfo.characterInfo[this.m_characterCount].adjustedAscender;
								if (this.m_lineHeight == -32767f)
								{
									float num72 = 0f - this.m_maxLineDescender + adjustedAscender2 + (num7 + this.m_lineSpacingDelta) * num2 + (this.m_lineSpacing + ((num5 == 10U || num5 == 8233U) ? this.m_paragraphSpacing : 0f)) * num4;
									this.m_lineOffset += num72;
									this.m_IsDrivenLineSpacing = false;
								}
								else
								{
									this.m_lineOffset += this.m_lineHeight + (this.m_lineSpacing + ((num5 == 10U || num5 == 8233U) ? this.m_paragraphSpacing : 0f)) * num4;
									this.m_IsDrivenLineSpacing = true;
								}
								this.m_maxLineAscender = TMP_Text.k_LargeNegativeFloat;
								this.m_maxLineDescender = TMP_Text.k_LargePositiveFloat;
								this.m_startOfLineAscender = adjustedAscender2;
								this.m_xAdvance = 0f + this.tag_LineIndent + this.tag_Indent;
								base.SaveWordWrappingState(ref TMP_Text.m_SavedWordWrapState, num15, this.m_characterCount);
								base.SaveWordWrappingState(ref TMP_Text.m_SavedLastValidState, num15, this.m_characterCount);
								this.m_characterCount++;
								goto IL_3FF8;
							}
							if (num5 == 3U)
							{
								num15 = this.m_TextProcessingArray.Length;
							}
						}
						if (this.m_textInfo.characterInfo[this.m_characterCount].isVisible)
						{
							this.m_meshExtents.min.x = Mathf.Min(this.m_meshExtents.min.x, this.m_textInfo.characterInfo[this.m_characterCount].bottomLeft.x);
							this.m_meshExtents.min.y = Mathf.Min(this.m_meshExtents.min.y, this.m_textInfo.characterInfo[this.m_characterCount].bottomLeft.y);
							this.m_meshExtents.max.x = Mathf.Max(this.m_meshExtents.max.x, this.m_textInfo.characterInfo[this.m_characterCount].topRight.x);
							this.m_meshExtents.max.y = Mathf.Max(this.m_meshExtents.max.y, this.m_textInfo.characterInfo[this.m_characterCount].topRight.y);
						}
						if ((this.m_TextWrappingMode != TextWrappingModes.NoWrap && this.m_TextWrappingMode != TextWrappingModes.PreserveWhitespaceNoWrap) || this.m_overflowMode == TextOverflowModes.Truncate || this.m_overflowMode == TextOverflowModes.Ellipsis || this.m_overflowMode == TextOverflowModes.Linked)
						{
							bool flag18 = false;
							bool flag19 = false;
							uint num73 = (uint)((this.m_characterCount + 1 < totalCharacterCount) ? this.m_textInfo.characterInfo[this.m_characterCount + 1].character : '\0');
							if ((flag13 || num5 == 8203U || num5 == 45U || num5 == 173U) && (!this.m_isNonBreakingSpace || flag10) && num5 != 160U && num5 != 8199U && num5 != 8209U && num5 != 8239U && num5 != 8288U)
							{
								if (num5 != 45U || this.m_characterCount <= 0 || !char.IsWhiteSpace(this.m_textInfo.characterInfo[this.m_characterCount - 1].character) || this.m_textInfo.characterInfo[this.m_characterCount - 1].lineNumber != this.m_lineNumber)
								{
									flag9 = false;
									flag18 = true;
									TMP_Text.m_SavedSoftLineBreakState.previous_WordBreak = -1;
								}
							}
							else if (!this.m_isNonBreakingSpace && ((TMP_TextParsingUtilities.IsHangul(num5) && !TMP_Settings.useModernHangulLineBreakingRules) || TMP_TextParsingUtilities.IsCJK(num5)))
							{
								bool flag20 = TMP_Settings.linebreakingRules.leadingCharacters.Contains(num5);
								bool flag21 = this.m_characterCount < totalCharacterCount - 1 && TMP_Settings.linebreakingRules.followingCharacters.Contains(num73);
								if (!flag20)
								{
									if (!flag21)
									{
										flag9 = false;
										flag18 = true;
									}
									if (flag9)
									{
										if (flag13)
										{
											flag19 = true;
										}
										flag18 = true;
									}
								}
								else if (flag9 && flag16)
								{
									if (flag13)
									{
										flag19 = true;
									}
									flag18 = true;
								}
							}
							else if (!this.m_isNonBreakingSpace && TMP_TextParsingUtilities.IsCJK(num73) && !TMP_Settings.linebreakingRules.followingCharacters.Contains(num73))
							{
								flag18 = true;
							}
							else if (flag9)
							{
								if ((flag13 && num5 != 160U) || (num5 == 173U && !flag11))
								{
									flag19 = true;
								}
								flag18 = true;
							}
							if (flag18)
							{
								base.SaveWordWrappingState(ref TMP_Text.m_SavedWordWrapState, num15, this.m_characterCount);
							}
							if (flag19)
							{
								base.SaveWordWrappingState(ref TMP_Text.m_SavedSoftLineBreakState, num15, this.m_characterCount);
							}
						}
						base.SaveWordWrappingState(ref TMP_Text.m_SavedLastValidState, num15, this.m_characterCount);
						this.m_characterCount++;
					}
				}
				IL_3FF8:
				num15++;
			}
			float num74 = this.m_maxFontSize - this.m_minFontSize;
			if (this.m_enableAutoSizing && num74 > 0.051f && this.m_fontSize < this.m_fontSizeMax && this.m_AutoSizeIterationCount < this.m_AutoSizeMaxIterationCount)
			{
				if (this.m_charWidthAdjDelta < this.m_charWidthMaxAdj / 100f)
				{
					this.m_charWidthAdjDelta = 0f;
				}
				this.m_minFontSize = this.m_fontSize;
				float num75 = Mathf.Max((this.m_maxFontSize - this.m_fontSize) / 2f, 0.05f);
				this.m_fontSize += num75;
				this.m_fontSize = Mathf.Min((float)((int)(this.m_fontSize * 20f + 0.5f)) / 20f, this.m_fontSizeMax);
				return;
			}
			this.m_IsAutoSizePointSizeSet = true;
			if (this.m_AutoSizeIterationCount >= this.m_AutoSizeMaxIterationCount)
			{
				Debug.Log("Auto Size Iteration Count: " + this.m_AutoSizeIterationCount.ToString() + ". Final Point Size: " + this.m_fontSize.ToString());
			}
			if (this.m_characterCount == 0 || (this.m_characterCount == 1 && num5 == 3U))
			{
				this.ClearMesh();
				TMPro_EventManager.ON_TEXT_CHANGED(this);
				return;
			}
			int num76 = TMP_Text.m_materialReferences[this.m_Underline.materialIndex].referenceCount * 4;
			this.m_textInfo.meshInfo[0].Clear(false);
			Vector3 a2 = Vector3.zero;
			Vector3[] rectTransformCorners = this.m_RectTransformCorners;
			VerticalAlignmentOptions verticalAlignment = this.m_VerticalAlignment;
			if (verticalAlignment <= VerticalAlignmentOptions.Bottom)
			{
				if (verticalAlignment != VerticalAlignmentOptions.Top)
				{
					if (verticalAlignment != VerticalAlignmentOptions.Middle)
					{
						if (verticalAlignment == VerticalAlignmentOptions.Bottom)
						{
							if (this.m_overflowMode != TextOverflowModes.Page)
							{
								a2 = rectTransformCorners[0] + new Vector3(0f + margin.x, 0f - num12 + margin.w, 0f);
							}
							else
							{
								a2 = rectTransformCorners[0] + new Vector3(0f + margin.x, 0f - this.m_textInfo.pageInfo[num8].descender + margin.w, 0f);
							}
						}
					}
					else if (this.m_overflowMode != TextOverflowModes.Page)
					{
						a2 = (rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(0f + margin.x, 0f - (this.m_maxTextAscender + margin.y + num12 - margin.w) / 2f, 0f);
					}
					else
					{
						a2 = (rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(0f + margin.x, 0f - (this.m_textInfo.pageInfo[num8].ascender + margin.y + this.m_textInfo.pageInfo[num8].descender - margin.w) / 2f, 0f);
					}
				}
				else if (this.m_overflowMode != TextOverflowModes.Page)
				{
					a2 = rectTransformCorners[1] + new Vector3(0f + margin.x, 0f - this.m_maxTextAscender - margin.y, 0f);
				}
				else
				{
					a2 = rectTransformCorners[1] + new Vector3(0f + margin.x, 0f - this.m_textInfo.pageInfo[num8].ascender - margin.y, 0f);
				}
			}
			else if (verticalAlignment != VerticalAlignmentOptions.Baseline)
			{
				if (verticalAlignment != VerticalAlignmentOptions.Geometry)
				{
					if (verticalAlignment == VerticalAlignmentOptions.Capline)
					{
						a2 = (rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(0f + margin.x, 0f - (this.m_maxCapHeight - margin.y - margin.w) / 2f, 0f);
					}
				}
				else
				{
					a2 = (rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(0f + margin.x, 0f - (this.m_meshExtents.max.y + margin.y + this.m_meshExtents.min.y - margin.w) / 2f, 0f);
				}
			}
			else
			{
				a2 = (rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(0f + margin.x, 0f, 0f);
			}
			Vector3 vector7 = Vector3.zero;
			Vector3 vector8 = Vector3.zero;
			int num77 = 0;
			int lineCount = 0;
			int num78 = 0;
			bool flag22 = false;
			bool flag23 = false;
			int num79 = 0;
			bool flag24 = !(this.m_canvas.worldCamera == null);
			float f = this.m_previousLossyScaleY = base.transform.lossyScale.y;
			RenderMode renderMode = this.m_canvas.renderMode;
			float scaleFactor = this.m_canvas.scaleFactor;
			Color32 color = Color.white;
			Color32 underlineColor = Color.white;
			HighlightState highlightState = new HighlightState(new Color32(byte.MaxValue, byte.MaxValue, 0, 64), TMP_Offset.zero);
			float num80 = 0f;
			float num81 = 0f;
			float num82 = 0f;
			float num83 = 0f;
			float num84 = TMP_Text.k_LargePositiveFloat;
			int num85 = 0;
			float num86 = 0f;
			float num87 = 0f;
			float b4 = 0f;
			TMP_CharacterInfo[] characterInfo = this.m_textInfo.characterInfo;
			int i = 0;
			while (i < this.m_characterCount)
			{
				TMP_FontAsset fontAsset = characterInfo[i].fontAsset;
				char character = characterInfo[i].character;
				bool flag25 = char.IsWhiteSpace(character);
				int lineNumber5 = characterInfo[i].lineNumber;
				TMP_LineInfo tmp_LineInfo = this.m_textInfo.lineInfo[lineNumber5];
				lineCount = lineNumber5 + 1;
				HorizontalAlignmentOptions alignment = tmp_LineInfo.alignment;
				if (alignment <= HorizontalAlignmentOptions.Justified)
				{
					switch (alignment)
					{
					case HorizontalAlignmentOptions.Left:
						if (!this.m_isRightToLeft)
						{
							vector7 = new Vector3(0f + tmp_LineInfo.marginLeft, 0f, 0f);
						}
						else
						{
							vector7 = new Vector3(0f - tmp_LineInfo.maxAdvance, 0f, 0f);
						}
						break;
					case HorizontalAlignmentOptions.Center:
						vector7 = new Vector3(tmp_LineInfo.marginLeft + tmp_LineInfo.width / 2f - tmp_LineInfo.maxAdvance / 2f, 0f, 0f);
						break;
					case (HorizontalAlignmentOptions)3:
						break;
					case HorizontalAlignmentOptions.Right:
						if (!this.m_isRightToLeft)
						{
							vector7 = new Vector3(tmp_LineInfo.marginLeft + tmp_LineInfo.width - tmp_LineInfo.maxAdvance, 0f, 0f);
						}
						else
						{
							vector7 = new Vector3(tmp_LineInfo.marginLeft + tmp_LineInfo.width, 0f, 0f);
						}
						break;
					default:
						if (alignment == HorizontalAlignmentOptions.Justified)
						{
							goto IL_4813;
						}
						break;
					}
				}
				else
				{
					if (alignment == HorizontalAlignmentOptions.Flush)
					{
						goto IL_4813;
					}
					if (alignment == HorizontalAlignmentOptions.Geometry)
					{
						vector7 = new Vector3(tmp_LineInfo.marginLeft + tmp_LineInfo.width / 2f - (tmp_LineInfo.lineExtents.min.x + tmp_LineInfo.lineExtents.max.x) / 2f, 0f, 0f);
					}
				}
				IL_4A91:
				vector8 = a2 + vector7;
				bool isVisible = characterInfo[i].isVisible;
				if (isVisible)
				{
					TMP_TextElementType elementType = characterInfo[i].elementType;
					if (elementType != TMP_TextElementType.Character)
					{
						if (elementType != TMP_TextElementType.Sprite)
						{
						}
					}
					else
					{
						Extents lineExtents = tmp_LineInfo.lineExtents;
						float num88 = this.m_uvLineOffset * (float)lineNumber5 % 1f;
						switch (this.m_horizontalMapping)
						{
						case TextureMappingOptions.Character:
							characterInfo[i].vertex_BL.uv2.x = 0f;
							characterInfo[i].vertex_TL.uv2.x = 0f;
							characterInfo[i].vertex_TR.uv2.x = 1f;
							characterInfo[i].vertex_BR.uv2.x = 1f;
							break;
						case TextureMappingOptions.Line:
							if (this.m_textAlignment != TextAlignmentOptions.Justified)
							{
								characterInfo[i].vertex_BL.uv2.x = (characterInfo[i].vertex_BL.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num88;
								characterInfo[i].vertex_TL.uv2.x = (characterInfo[i].vertex_TL.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num88;
								characterInfo[i].vertex_TR.uv2.x = (characterInfo[i].vertex_TR.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num88;
								characterInfo[i].vertex_BR.uv2.x = (characterInfo[i].vertex_BR.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num88;
							}
							else
							{
								characterInfo[i].vertex_BL.uv2.x = (characterInfo[i].vertex_BL.position.x + vector7.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num88;
								characterInfo[i].vertex_TL.uv2.x = (characterInfo[i].vertex_TL.position.x + vector7.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num88;
								characterInfo[i].vertex_TR.uv2.x = (characterInfo[i].vertex_TR.position.x + vector7.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num88;
								characterInfo[i].vertex_BR.uv2.x = (characterInfo[i].vertex_BR.position.x + vector7.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num88;
							}
							break;
						case TextureMappingOptions.Paragraph:
							characterInfo[i].vertex_BL.uv2.x = (characterInfo[i].vertex_BL.position.x + vector7.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num88;
							characterInfo[i].vertex_TL.uv2.x = (characterInfo[i].vertex_TL.position.x + vector7.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num88;
							characterInfo[i].vertex_TR.uv2.x = (characterInfo[i].vertex_TR.position.x + vector7.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num88;
							characterInfo[i].vertex_BR.uv2.x = (characterInfo[i].vertex_BR.position.x + vector7.x - this.m_meshExtents.min.x) / (this.m_meshExtents.max.x - this.m_meshExtents.min.x) + num88;
							break;
						case TextureMappingOptions.MatchAspect:
						{
							switch (this.m_verticalMapping)
							{
							case TextureMappingOptions.Character:
								characterInfo[i].vertex_BL.uv2.y = 0f;
								characterInfo[i].vertex_TL.uv2.y = 1f;
								characterInfo[i].vertex_TR.uv2.y = 0f;
								characterInfo[i].vertex_BR.uv2.y = 1f;
								break;
							case TextureMappingOptions.Line:
								characterInfo[i].vertex_BL.uv2.y = (characterInfo[i].vertex_BL.position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + num88;
								characterInfo[i].vertex_TL.uv2.y = (characterInfo[i].vertex_TL.position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + num88;
								characterInfo[i].vertex_TR.uv2.y = characterInfo[i].vertex_BL.uv2.y;
								characterInfo[i].vertex_BR.uv2.y = characterInfo[i].vertex_TL.uv2.y;
								break;
							case TextureMappingOptions.Paragraph:
								characterInfo[i].vertex_BL.uv2.y = (characterInfo[i].vertex_BL.position.y - this.m_meshExtents.min.y) / (this.m_meshExtents.max.y - this.m_meshExtents.min.y) + num88;
								characterInfo[i].vertex_TL.uv2.y = (characterInfo[i].vertex_TL.position.y - this.m_meshExtents.min.y) / (this.m_meshExtents.max.y - this.m_meshExtents.min.y) + num88;
								characterInfo[i].vertex_TR.uv2.y = characterInfo[i].vertex_BL.uv2.y;
								characterInfo[i].vertex_BR.uv2.y = characterInfo[i].vertex_TL.uv2.y;
								break;
							case TextureMappingOptions.MatchAspect:
								Debug.Log("ERROR: Cannot Match both Vertical & Horizontal.");
								break;
							}
							float num89 = (1f - (characterInfo[i].vertex_BL.uv2.y + characterInfo[i].vertex_TL.uv2.y) * characterInfo[i].aspectRatio) / 2f;
							characterInfo[i].vertex_BL.uv2.x = characterInfo[i].vertex_BL.uv2.y * characterInfo[i].aspectRatio + num89 + num88;
							characterInfo[i].vertex_TL.uv2.x = characterInfo[i].vertex_BL.uv2.x;
							characterInfo[i].vertex_TR.uv2.x = characterInfo[i].vertex_TL.uv2.y * characterInfo[i].aspectRatio + num89 + num88;
							characterInfo[i].vertex_BR.uv2.x = characterInfo[i].vertex_TR.uv2.x;
							break;
						}
						}
						switch (this.m_verticalMapping)
						{
						case TextureMappingOptions.Character:
							characterInfo[i].vertex_BL.uv2.y = 0f;
							characterInfo[i].vertex_TL.uv2.y = 1f;
							characterInfo[i].vertex_TR.uv2.y = 1f;
							characterInfo[i].vertex_BR.uv2.y = 0f;
							break;
						case TextureMappingOptions.Line:
							characterInfo[i].vertex_BL.uv2.y = (characterInfo[i].vertex_BL.position.y - tmp_LineInfo.descender) / (tmp_LineInfo.ascender - tmp_LineInfo.descender);
							characterInfo[i].vertex_TL.uv2.y = (characterInfo[i].vertex_TL.position.y - tmp_LineInfo.descender) / (tmp_LineInfo.ascender - tmp_LineInfo.descender);
							characterInfo[i].vertex_TR.uv2.y = characterInfo[i].vertex_TL.uv2.y;
							characterInfo[i].vertex_BR.uv2.y = characterInfo[i].vertex_BL.uv2.y;
							break;
						case TextureMappingOptions.Paragraph:
							characterInfo[i].vertex_BL.uv2.y = (characterInfo[i].vertex_BL.position.y - this.m_meshExtents.min.y) / (this.m_meshExtents.max.y - this.m_meshExtents.min.y);
							characterInfo[i].vertex_TL.uv2.y = (characterInfo[i].vertex_TL.position.y - this.m_meshExtents.min.y) / (this.m_meshExtents.max.y - this.m_meshExtents.min.y);
							characterInfo[i].vertex_TR.uv2.y = characterInfo[i].vertex_TL.uv2.y;
							characterInfo[i].vertex_BR.uv2.y = characterInfo[i].vertex_BL.uv2.y;
							break;
						case TextureMappingOptions.MatchAspect:
						{
							float num90 = (1f - (characterInfo[i].vertex_BL.uv2.x + characterInfo[i].vertex_TR.uv2.x) / characterInfo[i].aspectRatio) / 2f;
							characterInfo[i].vertex_BL.uv2.y = num90 + characterInfo[i].vertex_BL.uv2.x / characterInfo[i].aspectRatio;
							characterInfo[i].vertex_TL.uv2.y = num90 + characterInfo[i].vertex_TR.uv2.x / characterInfo[i].aspectRatio;
							characterInfo[i].vertex_BR.uv2.y = characterInfo[i].vertex_BL.uv2.y;
							characterInfo[i].vertex_TR.uv2.y = characterInfo[i].vertex_TL.uv2.y;
							break;
						}
						}
						num80 = characterInfo[i].scale * (1f - this.m_charWidthAdjDelta);
						if (!characterInfo[i].isUsingAlternateTypeface && (characterInfo[i].style & FontStyles.Bold) == FontStyles.Bold)
						{
							num80 *= -1f;
						}
						switch (renderMode)
						{
						case RenderMode.ScreenSpaceOverlay:
							num80 *= Mathf.Abs(f) / scaleFactor;
							break;
						case RenderMode.ScreenSpaceCamera:
							num80 *= (flag24 ? Mathf.Abs(f) : 1f);
							break;
						case RenderMode.WorldSpace:
							num80 *= Mathf.Abs(f);
							break;
						}
						characterInfo[i].vertex_BL.uv.w = num80;
						characterInfo[i].vertex_TL.uv.w = num80;
						characterInfo[i].vertex_TR.uv.w = num80;
						characterInfo[i].vertex_BR.uv.w = num80;
					}
					if (i < this.m_maxVisibleCharacters && num77 < this.m_maxVisibleWords && lineNumber5 < this.m_maxVisibleLines && this.m_overflowMode != TextOverflowModes.Page)
					{
						TMP_CharacterInfo[] array = characterInfo;
						int num91 = i;
						array[num91].vertex_BL.position = array[num91].vertex_BL.position + vector8;
						TMP_CharacterInfo[] array2 = characterInfo;
						int num92 = i;
						array2[num92].vertex_TL.position = array2[num92].vertex_TL.position + vector8;
						TMP_CharacterInfo[] array3 = characterInfo;
						int num93 = i;
						array3[num93].vertex_TR.position = array3[num93].vertex_TR.position + vector8;
						TMP_CharacterInfo[] array4 = characterInfo;
						int num94 = i;
						array4[num94].vertex_BR.position = array4[num94].vertex_BR.position + vector8;
					}
					else if (i < this.m_maxVisibleCharacters && num77 < this.m_maxVisibleWords && lineNumber5 < this.m_maxVisibleLines && this.m_overflowMode == TextOverflowModes.Page && characterInfo[i].pageNumber == num8)
					{
						TMP_CharacterInfo[] array5 = characterInfo;
						int num95 = i;
						array5[num95].vertex_BL.position = array5[num95].vertex_BL.position + vector8;
						TMP_CharacterInfo[] array6 = characterInfo;
						int num96 = i;
						array6[num96].vertex_TL.position = array6[num96].vertex_TL.position + vector8;
						TMP_CharacterInfo[] array7 = characterInfo;
						int num97 = i;
						array7[num97].vertex_TR.position = array7[num97].vertex_TR.position + vector8;
						TMP_CharacterInfo[] array8 = characterInfo;
						int num98 = i;
						array8[num98].vertex_BR.position = array8[num98].vertex_BR.position + vector8;
					}
					else
					{
						characterInfo[i].vertex_BL.position = Vector3.zero;
						characterInfo[i].vertex_TL.position = Vector3.zero;
						characterInfo[i].vertex_TR.position = Vector3.zero;
						characterInfo[i].vertex_BR.position = Vector3.zero;
						characterInfo[i].isVisible = false;
					}
					if (elementType == TMP_TextElementType.Character)
					{
						this.FillCharacterVertexBuffers(i);
					}
					else if (elementType == TMP_TextElementType.Sprite)
					{
						this.FillSpriteVertexBuffers(i);
					}
				}
				TMP_CharacterInfo[] characterInfo2 = this.m_textInfo.characterInfo;
				int num99 = i;
				characterInfo2[num99].bottomLeft = characterInfo2[num99].bottomLeft + vector8;
				TMP_CharacterInfo[] characterInfo3 = this.m_textInfo.characterInfo;
				int num100 = i;
				characterInfo3[num100].topLeft = characterInfo3[num100].topLeft + vector8;
				TMP_CharacterInfo[] characterInfo4 = this.m_textInfo.characterInfo;
				int num101 = i;
				characterInfo4[num101].topRight = characterInfo4[num101].topRight + vector8;
				TMP_CharacterInfo[] characterInfo5 = this.m_textInfo.characterInfo;
				int num102 = i;
				characterInfo5[num102].bottomRight = characterInfo5[num102].bottomRight + vector8;
				TMP_CharacterInfo[] characterInfo6 = this.m_textInfo.characterInfo;
				int num103 = i;
				characterInfo6[num103].origin = characterInfo6[num103].origin + vector8.x;
				TMP_CharacterInfo[] characterInfo7 = this.m_textInfo.characterInfo;
				int num104 = i;
				characterInfo7[num104].xAdvance = characterInfo7[num104].xAdvance + vector8.x;
				TMP_CharacterInfo[] characterInfo8 = this.m_textInfo.characterInfo;
				int num105 = i;
				characterInfo8[num105].ascender = characterInfo8[num105].ascender + vector8.y;
				TMP_CharacterInfo[] characterInfo9 = this.m_textInfo.characterInfo;
				int num106 = i;
				characterInfo9[num106].descender = characterInfo9[num106].descender + vector8.y;
				TMP_CharacterInfo[] characterInfo10 = this.m_textInfo.characterInfo;
				int num107 = i;
				characterInfo10[num107].baseLine = characterInfo10[num107].baseLine + vector8.y;
				if (lineNumber5 != num78 || i == this.m_characterCount - 1)
				{
					if (lineNumber5 != num78)
					{
						TMP_LineInfo[] lineInfo5 = this.m_textInfo.lineInfo;
						int num108 = num78;
						lineInfo5[num108].baseline = lineInfo5[num108].baseline + vector8.y;
						TMP_LineInfo[] lineInfo6 = this.m_textInfo.lineInfo;
						int num109 = num78;
						lineInfo6[num109].ascender = lineInfo6[num109].ascender + vector8.y;
						TMP_LineInfo[] lineInfo7 = this.m_textInfo.lineInfo;
						int num110 = num78;
						lineInfo7[num110].descender = lineInfo7[num110].descender + vector8.y;
						TMP_LineInfo[] lineInfo8 = this.m_textInfo.lineInfo;
						int num111 = num78;
						lineInfo8[num111].maxAdvance = lineInfo8[num111].maxAdvance + vector8.x;
						this.m_textInfo.lineInfo[num78].lineExtents.min = new Vector2(this.m_textInfo.characterInfo[this.m_textInfo.lineInfo[num78].firstCharacterIndex].bottomLeft.x, this.m_textInfo.lineInfo[num78].descender);
						this.m_textInfo.lineInfo[num78].lineExtents.max = new Vector2(this.m_textInfo.characterInfo[this.m_textInfo.lineInfo[num78].lastVisibleCharacterIndex].topRight.x, this.m_textInfo.lineInfo[num78].ascender);
					}
					if (i == this.m_characterCount - 1)
					{
						TMP_LineInfo[] lineInfo9 = this.m_textInfo.lineInfo;
						int num112 = lineNumber5;
						lineInfo9[num112].baseline = lineInfo9[num112].baseline + vector8.y;
						TMP_LineInfo[] lineInfo10 = this.m_textInfo.lineInfo;
						int num113 = lineNumber5;
						lineInfo10[num113].ascender = lineInfo10[num113].ascender + vector8.y;
						TMP_LineInfo[] lineInfo11 = this.m_textInfo.lineInfo;
						int num114 = lineNumber5;
						lineInfo11[num114].descender = lineInfo11[num114].descender + vector8.y;
						TMP_LineInfo[] lineInfo12 = this.m_textInfo.lineInfo;
						int num115 = lineNumber5;
						lineInfo12[num115].maxAdvance = lineInfo12[num115].maxAdvance + vector8.x;
						this.m_textInfo.lineInfo[lineNumber5].lineExtents.min = new Vector2(this.m_textInfo.characterInfo[this.m_textInfo.lineInfo[lineNumber5].firstCharacterIndex].bottomLeft.x, this.m_textInfo.lineInfo[lineNumber5].descender);
						this.m_textInfo.lineInfo[lineNumber5].lineExtents.max = new Vector2(this.m_textInfo.characterInfo[this.m_textInfo.lineInfo[lineNumber5].lastVisibleCharacterIndex].topRight.x, this.m_textInfo.lineInfo[lineNumber5].ascender);
					}
				}
				if (char.IsLetterOrDigit(character) || character == '-' || character == '­' || character == '‐' || character == '‑')
				{
					if (!flag23)
					{
						flag23 = true;
						num79 = i;
					}
					if (flag23 && i == this.m_characterCount - 1)
					{
						int num116 = this.m_textInfo.wordInfo.Length;
						int wordCount = this.m_textInfo.wordCount;
						if (this.m_textInfo.wordCount + 1 > num116)
						{
							TMP_TextInfo.Resize<TMP_WordInfo>(ref this.m_textInfo.wordInfo, num116 + 1);
						}
						int num117 = i;
						this.m_textInfo.wordInfo[wordCount].firstCharacterIndex = num79;
						this.m_textInfo.wordInfo[wordCount].lastCharacterIndex = num117;
						this.m_textInfo.wordInfo[wordCount].characterCount = num117 - num79 + 1;
						this.m_textInfo.wordInfo[wordCount].textComponent = this;
						num77++;
						this.m_textInfo.wordCount++;
						TMP_LineInfo[] lineInfo13 = this.m_textInfo.lineInfo;
						int num118 = lineNumber5;
						lineInfo13[num118].wordCount = lineInfo13[num118].wordCount + 1;
					}
				}
				else if ((flag23 || (i == 0 && (!char.IsPunctuation(character) || flag25 || character == '​' || i == this.m_characterCount - 1))) && (i <= 0 || i >= characterInfo.Length - 1 || i >= this.m_characterCount || (character != '\'' && character != '’') || !char.IsLetterOrDigit(characterInfo[i - 1].character) || !char.IsLetterOrDigit(characterInfo[i + 1].character)))
				{
					int num117 = (i == this.m_characterCount - 1 && char.IsLetterOrDigit(character)) ? i : (i - 1);
					flag23 = false;
					int num119 = this.m_textInfo.wordInfo.Length;
					int wordCount2 = this.m_textInfo.wordCount;
					if (this.m_textInfo.wordCount + 1 > num119)
					{
						TMP_TextInfo.Resize<TMP_WordInfo>(ref this.m_textInfo.wordInfo, num119 + 1);
					}
					this.m_textInfo.wordInfo[wordCount2].firstCharacterIndex = num79;
					this.m_textInfo.wordInfo[wordCount2].lastCharacterIndex = num117;
					this.m_textInfo.wordInfo[wordCount2].characterCount = num117 - num79 + 1;
					this.m_textInfo.wordInfo[wordCount2].textComponent = this;
					num77++;
					this.m_textInfo.wordCount++;
					TMP_LineInfo[] lineInfo14 = this.m_textInfo.lineInfo;
					int num120 = lineNumber5;
					lineInfo14[num120].wordCount = lineInfo14[num120].wordCount + 1;
				}
				if ((this.m_textInfo.characterInfo[i].style & FontStyles.Underline) == FontStyles.Underline)
				{
					bool flag26 = true;
					int pageNumber = this.m_textInfo.characterInfo[i].pageNumber;
					this.m_textInfo.characterInfo[i].underlineVertexIndex = num76;
					if (i > this.m_maxVisibleCharacters || lineNumber5 > this.m_maxVisibleLines || (this.m_overflowMode == TextOverflowModes.Page && pageNumber + 1 != this.m_pageToDisplay))
					{
						flag26 = false;
					}
					if (!flag25 && character != '​')
					{
						num83 = Mathf.Max(num83, this.m_textInfo.characterInfo[i].scale);
						num81 = Mathf.Max(num81, Mathf.Abs(num80));
						num84 = Mathf.Min((pageNumber == num85) ? num84 : TMP_Text.k_LargePositiveFloat, this.m_textInfo.characterInfo[i].baseLine + base.font.m_FaceInfo.underlineOffset * num83);
						num85 = pageNumber;
					}
					if (!flag && flag26 && i <= tmp_LineInfo.lastVisibleCharacterIndex && character != '\n' && character != '\v' && character != '\r' && (i != tmp_LineInfo.lastVisibleCharacterIndex || !char.IsSeparator(character)))
					{
						flag = true;
						num82 = this.m_textInfo.characterInfo[i].scale;
						if (num83 == 0f)
						{
							num83 = num82;
							num81 = num80;
						}
						zero = new Vector3(this.m_textInfo.characterInfo[i].bottomLeft.x, num84, 0f);
						color = this.m_textInfo.characterInfo[i].underlineColor;
					}
					if (flag && this.m_characterCount == 1)
					{
						flag = false;
						zero2 = new Vector3(this.m_textInfo.characterInfo[i].topRight.x, num84, 0f);
						float scale = this.m_textInfo.characterInfo[i].scale;
						this.DrawUnderlineMesh(zero, zero2, ref num76, num82, scale, num83, num81, color);
						num83 = 0f;
						num81 = 0f;
						num84 = TMP_Text.k_LargePositiveFloat;
					}
					else if (flag && (i == tmp_LineInfo.lastCharacterIndex || i >= tmp_LineInfo.lastVisibleCharacterIndex))
					{
						float scale;
						if (flag25 || character == '​')
						{
							int lastVisibleCharacterIndex = tmp_LineInfo.lastVisibleCharacterIndex;
							zero2 = new Vector3(this.m_textInfo.characterInfo[lastVisibleCharacterIndex].topRight.x, num84, 0f);
							scale = this.m_textInfo.characterInfo[lastVisibleCharacterIndex].scale;
						}
						else
						{
							zero2 = new Vector3(this.m_textInfo.characterInfo[i].topRight.x, num84, 0f);
							scale = this.m_textInfo.characterInfo[i].scale;
						}
						flag = false;
						this.DrawUnderlineMesh(zero, zero2, ref num76, num82, scale, num83, num81, color);
						num83 = 0f;
						num81 = 0f;
						num84 = TMP_Text.k_LargePositiveFloat;
					}
					else if (flag && !flag26)
					{
						flag = false;
						zero2 = new Vector3(this.m_textInfo.characterInfo[i - 1].topRight.x, num84, 0f);
						float scale = this.m_textInfo.characterInfo[i - 1].scale;
						this.DrawUnderlineMesh(zero, zero2, ref num76, num82, scale, num83, num81, color);
						num83 = 0f;
						num81 = 0f;
						num84 = TMP_Text.k_LargePositiveFloat;
					}
					else if (flag && i < this.m_characterCount - 1 && !color.Compare(this.m_textInfo.characterInfo[i + 1].underlineColor))
					{
						flag = false;
						zero2 = new Vector3(this.m_textInfo.characterInfo[i].topRight.x, num84, 0f);
						float scale = this.m_textInfo.characterInfo[i].scale;
						this.DrawUnderlineMesh(zero, zero2, ref num76, num82, scale, num83, num81, color);
						num83 = 0f;
						num81 = 0f;
						num84 = TMP_Text.k_LargePositiveFloat;
					}
				}
				else if (flag)
				{
					flag = false;
					zero2 = new Vector3(this.m_textInfo.characterInfo[i - 1].topRight.x, num84, 0f);
					float scale = this.m_textInfo.characterInfo[i - 1].scale;
					this.DrawUnderlineMesh(zero, zero2, ref num76, num82, scale, num83, num81, color);
					num83 = 0f;
					num81 = 0f;
					num84 = TMP_Text.k_LargePositiveFloat;
				}
				bool flag27 = (this.m_textInfo.characterInfo[i].style & FontStyles.Strikethrough) == FontStyles.Strikethrough;
				float strikethroughOffset = fontAsset.m_FaceInfo.strikethroughOffset;
				if (flag27)
				{
					bool flag28 = true;
					this.m_textInfo.characterInfo[i].strikethroughVertexIndex = num76;
					if (i > this.m_maxVisibleCharacters || lineNumber5 > this.m_maxVisibleLines || (this.m_overflowMode == TextOverflowModes.Page && this.m_textInfo.characterInfo[i].pageNumber + 1 != this.m_pageToDisplay))
					{
						flag28 = false;
					}
					if (!flag2 && flag28 && i <= tmp_LineInfo.lastVisibleCharacterIndex && character != '\n' && character != '\v' && character != '\r' && (i != tmp_LineInfo.lastVisibleCharacterIndex || !char.IsSeparator(character)))
					{
						flag2 = true;
						num86 = this.m_textInfo.characterInfo[i].pointSize;
						num87 = this.m_textInfo.characterInfo[i].scale;
						zero3 = new Vector3(this.m_textInfo.characterInfo[i].bottomLeft.x, this.m_textInfo.characterInfo[i].baseLine + strikethroughOffset * num87, 0f);
						underlineColor = this.m_textInfo.characterInfo[i].strikethroughColor;
						b4 = this.m_textInfo.characterInfo[i].baseLine;
					}
					if (flag2 && this.m_characterCount == 1)
					{
						flag2 = false;
						zero4 = new Vector3(this.m_textInfo.characterInfo[i].topRight.x, this.m_textInfo.characterInfo[i].baseLine + strikethroughOffset * num87, 0f);
						this.DrawUnderlineMesh(zero3, zero4, ref num76, num87, num87, num87, num80, underlineColor);
					}
					else if (flag2 && i == tmp_LineInfo.lastCharacterIndex)
					{
						if (flag25 || character == '​')
						{
							int lastVisibleCharacterIndex2 = tmp_LineInfo.lastVisibleCharacterIndex;
							zero4 = new Vector3(this.m_textInfo.characterInfo[lastVisibleCharacterIndex2].topRight.x, this.m_textInfo.characterInfo[lastVisibleCharacterIndex2].baseLine + strikethroughOffset * num87, 0f);
						}
						else
						{
							zero4 = new Vector3(this.m_textInfo.characterInfo[i].topRight.x, this.m_textInfo.characterInfo[i].baseLine + strikethroughOffset * num87, 0f);
						}
						flag2 = false;
						this.DrawUnderlineMesh(zero3, zero4, ref num76, num87, num87, num87, num80, underlineColor);
					}
					else if (flag2 && i < this.m_characterCount && (this.m_textInfo.characterInfo[i + 1].pointSize != num86 || !TMP_Math.Approximately(this.m_textInfo.characterInfo[i + 1].baseLine + vector8.y, b4)))
					{
						flag2 = false;
						int lastVisibleCharacterIndex3 = tmp_LineInfo.lastVisibleCharacterIndex;
						if (i > lastVisibleCharacterIndex3)
						{
							zero4 = new Vector3(this.m_textInfo.characterInfo[lastVisibleCharacterIndex3].topRight.x, this.m_textInfo.characterInfo[lastVisibleCharacterIndex3].baseLine + strikethroughOffset * num87, 0f);
						}
						else
						{
							zero4 = new Vector3(this.m_textInfo.characterInfo[i].topRight.x, this.m_textInfo.characterInfo[i].baseLine + strikethroughOffset * num87, 0f);
						}
						this.DrawUnderlineMesh(zero3, zero4, ref num76, num87, num87, num87, num80, underlineColor);
					}
					else if (flag2 && i < this.m_characterCount && fontAsset.GetInstanceID() != characterInfo[i + 1].fontAsset.GetInstanceID())
					{
						flag2 = false;
						zero4 = new Vector3(this.m_textInfo.characterInfo[i].topRight.x, this.m_textInfo.characterInfo[i].baseLine + strikethroughOffset * num87, 0f);
						this.DrawUnderlineMesh(zero3, zero4, ref num76, num87, num87, num87, num80, underlineColor);
					}
					else if (flag2 && !flag28)
					{
						flag2 = false;
						zero4 = new Vector3(this.m_textInfo.characterInfo[i - 1].topRight.x, this.m_textInfo.characterInfo[i - 1].baseLine + strikethroughOffset * num87, 0f);
						this.DrawUnderlineMesh(zero3, zero4, ref num76, num87, num87, num87, num80, underlineColor);
					}
				}
				else if (flag2)
				{
					flag2 = false;
					zero4 = new Vector3(this.m_textInfo.characterInfo[i - 1].topRight.x, this.m_textInfo.characterInfo[i - 1].baseLine + strikethroughOffset * num87, 0f);
					this.DrawUnderlineMesh(zero3, zero4, ref num76, num87, num87, num87, num80, underlineColor);
				}
				if ((this.m_textInfo.characterInfo[i].style & FontStyles.Highlight) == FontStyles.Highlight)
				{
					bool flag29 = true;
					int pageNumber2 = this.m_textInfo.characterInfo[i].pageNumber;
					if (i > this.m_maxVisibleCharacters || lineNumber5 > this.m_maxVisibleLines || (this.m_overflowMode == TextOverflowModes.Page && pageNumber2 + 1 != this.m_pageToDisplay))
					{
						flag29 = false;
					}
					if (!flag3 && flag29 && i <= tmp_LineInfo.lastVisibleCharacterIndex && character != '\n' && character != '\v' && character != '\r' && (i != tmp_LineInfo.lastVisibleCharacterIndex || !char.IsSeparator(character)))
					{
						flag3 = true;
						vector = TMP_Text.k_LargePositiveVector2;
						vector2 = TMP_Text.k_LargeNegativeVector2;
						highlightState = this.m_textInfo.characterInfo[i].highlightState;
					}
					if (flag3)
					{
						TMP_CharacterInfo tmp_CharacterInfo = this.m_textInfo.characterInfo[i];
						HighlightState highlightState2 = tmp_CharacterInfo.highlightState;
						bool flag30 = false;
						if (highlightState != highlightState2)
						{
							if (flag25)
							{
								vector2.x = (vector2.x - highlightState.padding.right + tmp_CharacterInfo.origin) / 2f;
							}
							else
							{
								vector2.x = (vector2.x - highlightState.padding.right + tmp_CharacterInfo.bottomLeft.x) / 2f;
							}
							vector.y = Mathf.Min(vector.y, tmp_CharacterInfo.descender);
							vector2.y = Mathf.Max(vector2.y, tmp_CharacterInfo.ascender);
							this.DrawTextHighlight(vector, vector2, ref num76, highlightState.color);
							flag3 = true;
							vector = new Vector2(vector2.x, tmp_CharacterInfo.descender - highlightState2.padding.bottom);
							if (flag25)
							{
								vector2 = new Vector2(tmp_CharacterInfo.xAdvance + highlightState2.padding.right, tmp_CharacterInfo.ascender + highlightState2.padding.top);
							}
							else
							{
								vector2 = new Vector2(tmp_CharacterInfo.topRight.x + highlightState2.padding.right, tmp_CharacterInfo.ascender + highlightState2.padding.top);
							}
							highlightState = highlightState2;
							flag30 = true;
						}
						if (!flag30)
						{
							if (flag25)
							{
								vector.x = Mathf.Min(vector.x, tmp_CharacterInfo.origin - highlightState.padding.left);
								vector2.x = Mathf.Max(vector2.x, tmp_CharacterInfo.xAdvance + highlightState.padding.right);
							}
							else
							{
								vector.x = Mathf.Min(vector.x, tmp_CharacterInfo.bottomLeft.x - highlightState.padding.left);
								vector2.x = Mathf.Max(vector2.x, tmp_CharacterInfo.topRight.x + highlightState.padding.right);
							}
							vector.y = Mathf.Min(vector.y, tmp_CharacterInfo.descender - highlightState.padding.bottom);
							vector2.y = Mathf.Max(vector2.y, tmp_CharacterInfo.ascender + highlightState.padding.top);
						}
					}
					if (flag3 && this.m_characterCount == 1)
					{
						flag3 = false;
						this.DrawTextHighlight(vector, vector2, ref num76, highlightState.color);
					}
					else if (flag3 && (i == tmp_LineInfo.lastCharacterIndex || i >= tmp_LineInfo.lastVisibleCharacterIndex))
					{
						flag3 = false;
						this.DrawTextHighlight(vector, vector2, ref num76, highlightState.color);
					}
					else if (flag3 && !flag29)
					{
						flag3 = false;
						this.DrawTextHighlight(vector, vector2, ref num76, highlightState.color);
					}
				}
				else if (flag3)
				{
					flag3 = false;
					this.DrawTextHighlight(vector, vector2, ref num76, highlightState.color);
				}
				num78 = lineNumber5;
				i++;
				continue;
				IL_4813:
				if (i > tmp_LineInfo.lastVisibleCharacterIndex || character == '\n' || character == '­' || character == '​' || character == '⁠' || character == '\u0003')
				{
					goto IL_4A91;
				}
				char character2 = characterInfo[tmp_LineInfo.lastCharacterIndex].character;
				bool flag31 = (alignment & HorizontalAlignmentOptions.Flush) == HorizontalAlignmentOptions.Flush;
				if ((!char.IsControl(character2) && lineNumber5 < this.m_lineNumber) || flag31 || tmp_LineInfo.maxAdvance > tmp_LineInfo.width)
				{
					if (lineNumber5 != num78 || i == 0 || i == this.m_firstVisibleCharacter)
					{
						if (!this.m_isRightToLeft)
						{
							vector7 = new Vector3(tmp_LineInfo.marginLeft, 0f, 0f);
						}
						else
						{
							vector7 = new Vector3(tmp_LineInfo.marginLeft + tmp_LineInfo.width, 0f, 0f);
						}
						flag22 = char.IsSeparator(character);
						goto IL_4A91;
					}
					float num121 = (!this.m_isRightToLeft) ? (tmp_LineInfo.width - tmp_LineInfo.maxAdvance) : (tmp_LineInfo.width + tmp_LineInfo.maxAdvance);
					int num122 = tmp_LineInfo.visibleCharacterCount - 1 + tmp_LineInfo.controlCharacterCount;
					int num123 = tmp_LineInfo.visibleSpaceCount - tmp_LineInfo.controlCharacterCount;
					if (flag22)
					{
						num123--;
						num122++;
					}
					float num124 = (num123 > 0) ? this.m_wordWrappingRatios : 1f;
					if (num123 < 1)
					{
						num123 = 1;
					}
					if (character != '\u00a0' && (character == '\t' || char.IsSeparator(character)))
					{
						if (!this.m_isRightToLeft)
						{
							vector7 += new Vector3(num121 * (1f - num124) / (float)num123, 0f, 0f);
							goto IL_4A91;
						}
						vector7 -= new Vector3(num121 * (1f - num124) / (float)num123, 0f, 0f);
						goto IL_4A91;
					}
					else
					{
						if (!this.m_isRightToLeft)
						{
							vector7 += new Vector3(num121 * num124 / (float)num122, 0f, 0f);
							goto IL_4A91;
						}
						vector7 -= new Vector3(num121 * num124 / (float)num122, 0f, 0f);
						goto IL_4A91;
					}
				}
				else
				{
					if (!this.m_isRightToLeft)
					{
						vector7 = new Vector3(tmp_LineInfo.marginLeft, 0f, 0f);
						goto IL_4A91;
					}
					vector7 = new Vector3(tmp_LineInfo.marginLeft + tmp_LineInfo.width, 0f, 0f);
					goto IL_4A91;
				}
			}
			this.m_textInfo.meshInfo[this.m_Underline.materialIndex].vertexCount = num76;
			this.m_textInfo.characterCount = this.m_characterCount;
			this.m_textInfo.spriteCount = this.m_spriteCount;
			this.m_textInfo.lineCount = lineCount;
			this.m_textInfo.wordCount = ((num77 != 0 && this.m_characterCount > 0) ? num77 : 1);
			this.m_textInfo.pageCount = this.m_pageNumber + 1;
			if (this.m_renderMode == TextRenderFlags.Render && this.IsActive())
			{
				Action<TMP_TextInfo> onPreRenderText = this.OnPreRenderText;
				if (onPreRenderText != null)
				{
					onPreRenderText(this.m_textInfo);
				}
				if (this.m_canvas.additionalShaderChannels != (AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.Tangent))
				{
					this.m_canvas.additionalShaderChannels |= (AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.Tangent);
				}
				if (this.m_geometrySortingOrder != VertexSortingOrder.Normal)
				{
					this.m_textInfo.meshInfo[0].SortGeometry(VertexSortingOrder.Reverse);
				}
				this.m_mesh.MarkDynamic();
				this.m_mesh.vertices = this.m_textInfo.meshInfo[0].vertices;
				this.m_mesh.SetUVs(0, this.m_textInfo.meshInfo[0].uvs0);
				this.m_mesh.uv2 = this.m_textInfo.meshInfo[0].uvs2;
				this.m_mesh.colors32 = this.m_textInfo.meshInfo[0].colors32;
				this.m_mesh.RecalculateBounds();
				this.m_canvasRenderer.SetMesh(this.m_mesh);
				Color color2 = this.m_canvasRenderer.GetColor();
				bool cullTransparentMesh = this.m_canvasRenderer.cullTransparentMesh;
				for (int j = 1; j < this.m_textInfo.materialCount; j++)
				{
					this.m_textInfo.meshInfo[j].ClearUnusedVertices();
					if (!(this.m_subTextObjects[j] == null))
					{
						if (this.m_geometrySortingOrder != VertexSortingOrder.Normal)
						{
							this.m_textInfo.meshInfo[j].SortGeometry(VertexSortingOrder.Reverse);
						}
						this.m_subTextObjects[j].mesh.vertices = this.m_textInfo.meshInfo[j].vertices;
						this.m_subTextObjects[j].mesh.SetUVs(0, this.m_textInfo.meshInfo[j].uvs0);
						this.m_subTextObjects[j].mesh.uv2 = this.m_textInfo.meshInfo[j].uvs2;
						this.m_subTextObjects[j].mesh.colors32 = this.m_textInfo.meshInfo[j].colors32;
						this.m_subTextObjects[j].mesh.RecalculateBounds();
						this.m_subTextObjects[j].canvasRenderer.SetMesh(this.m_subTextObjects[j].mesh);
						this.m_subTextObjects[j].canvasRenderer.SetColor(color2);
						this.m_subTextObjects[j].canvasRenderer.cullTransparentMesh = cullTransparentMesh;
						this.m_subTextObjects[j].raycastTarget = this.raycastTarget;
					}
				}
			}
			if (this.m_ShouldUpdateCulling)
			{
				this.UpdateCulling();
			}
			TMPro_EventManager.ON_TEXT_CHANGED(this);
		}

		protected override Vector3[] GetTextContainerLocalCorners()
		{
			if (this.m_rectTransform == null)
			{
				this.m_rectTransform = base.rectTransform;
			}
			this.m_rectTransform.GetLocalCorners(this.m_RectTransformCorners);
			return this.m_RectTransformCorners;
		}

		protected override void SetActiveSubMeshes(bool state)
		{
			int num = 1;
			while (num < this.m_subTextObjects.Length && this.m_subTextObjects[num] != null)
			{
				if (this.m_subTextObjects[num].enabled != state)
				{
					this.m_subTextObjects[num].enabled = state;
				}
				num++;
			}
		}

		protected override void DestroySubMeshObjects()
		{
			int num = 1;
			while (num < this.m_subTextObjects.Length && this.m_subTextObjects[num] != null)
			{
				Object.DestroyImmediate(this.m_subTextObjects[num]);
				num++;
			}
		}

		protected override Bounds GetCompoundBounds()
		{
			Bounds bounds = this.m_mesh.bounds;
			Vector3 min = bounds.min;
			Vector3 max = bounds.max;
			int num = 1;
			while (num < this.m_subTextObjects.Length && this.m_subTextObjects[num] != null)
			{
				Bounds bounds2 = this.m_subTextObjects[num].mesh.bounds;
				min.x = ((min.x < bounds2.min.x) ? min.x : bounds2.min.x);
				min.y = ((min.y < bounds2.min.y) ? min.y : bounds2.min.y);
				max.x = ((max.x > bounds2.max.x) ? max.x : bounds2.max.x);
				max.y = ((max.y > bounds2.max.y) ? max.y : bounds2.max.y);
				num++;
			}
			Vector3 center = (min + max) / 2f;
			Vector2 v = max - min;
			return new Bounds(center, v);
		}

		internal override Rect GetCanvasSpaceClippingRect()
		{
			if (this.m_canvas == null || this.m_canvas.rootCanvas == null || this.m_mesh == null)
			{
				return Rect.zero;
			}
			Transform transform = this.m_canvas.rootCanvas.transform;
			Bounds compoundBounds = this.GetCompoundBounds();
			Vector2 a = transform.InverseTransformPoint(this.m_rectTransform.position);
			Vector2 b = transform.lossyScale;
			Vector2 b2 = this.m_rectTransform.lossyScale / b;
			return new Rect(a + compoundBounds.min * b2, compoundBounds.size * b2);
		}

		private void UpdateSDFScale(float scaleDelta)
		{
			if (scaleDelta == 0f || scaleDelta == float.PositiveInfinity || scaleDelta == float.NegativeInfinity)
			{
				this.m_havePropertiesChanged = true;
				this.OnPreRenderCanvas();
				return;
			}
			for (int i = 0; i < this.m_textInfo.materialCount; i++)
			{
				TMP_MeshInfo tmp_MeshInfo = this.m_textInfo.meshInfo[i];
				for (int j = 0; j < tmp_MeshInfo.uvs0.Length; j++)
				{
					Vector4[] uvs = tmp_MeshInfo.uvs0;
					int num = j;
					uvs[num].w = uvs[num].w * Mathf.Abs(scaleDelta);
				}
			}
			for (int k = 0; k < this.m_textInfo.materialCount; k++)
			{
				if (k == 0)
				{
					this.m_mesh.SetUVs(0, this.m_textInfo.meshInfo[0].uvs0);
					this.m_canvasRenderer.SetMesh(this.m_mesh);
				}
				else
				{
					this.m_subTextObjects[k].mesh.SetUVs(0, this.m_textInfo.meshInfo[k].uvs0);
					this.m_subTextObjects[k].canvasRenderer.SetMesh(this.m_subTextObjects[k].mesh);
				}
			}
		}

		private bool m_isRebuildingLayout;

		private Coroutine m_DelayedGraphicRebuild;

		private Coroutine m_DelayedMaterialRebuild;

		private bool m_ShouldUpdateCulling;

		private Rect m_ClipRect;

		private bool m_ValidRect;

		[SerializeField]
		private bool m_hasFontAssetChanged;

		protected TMP_SubMeshUI[] m_subTextObjects = new TMP_SubMeshUI[8];

		private float m_previousLossyScaleY = -1f;

		private Vector3[] m_RectTransformCorners = new Vector3[4];

		private CanvasRenderer m_canvasRenderer;

		private Canvas m_canvas;

		private float m_CanvasScaleFactor;

		private bool m_isFirstAllocation;

		private int m_max_characters = 8;

		[SerializeField]
		private Material m_baseMaterial;

		private bool m_isScrollRegionSet;

		[SerializeField]
		private Vector4 m_maskOffset;

		private Matrix4x4 m_EnvMapMatrix;

		[NonSerialized]
		private bool m_isRegisteredForEvents;

		private static ProfilerMarker k_GenerateTextMarker = new ProfilerMarker("TMP.GenerateText");

		private static ProfilerMarker k_SetArraySizesMarker = new ProfilerMarker("TMP.SetArraySizes");

		private static ProfilerMarker k_GenerateTextPhaseIMarker = new ProfilerMarker("TMP GenerateText - Phase I");

		private static ProfilerMarker k_ParseMarkupTextMarker = new ProfilerMarker("TMP Parse Markup Text");

		private static ProfilerMarker k_CharacterLookupMarker = new ProfilerMarker("TMP Lookup Character & Glyph Data");

		private static ProfilerMarker k_HandleGPOSFeaturesMarker = new ProfilerMarker("TMP Handle GPOS Features");

		private static ProfilerMarker k_CalculateVerticesPositionMarker = new ProfilerMarker("TMP Calculate Vertices Position");

		private static ProfilerMarker k_ComputeTextMetricsMarker = new ProfilerMarker("TMP Compute Text Metrics");

		private static ProfilerMarker k_HandleVisibleCharacterMarker = new ProfilerMarker("TMP Handle Visible Character");

		private static ProfilerMarker k_HandleWhiteSpacesMarker = new ProfilerMarker("TMP Handle White Space & Control Character");

		private static ProfilerMarker k_HandleHorizontalLineBreakingMarker = new ProfilerMarker("TMP Handle Horizontal Line Breaking");

		private static ProfilerMarker k_HandleVerticalLineBreakingMarker = new ProfilerMarker("TMP Handle Vertical Line Breaking");

		private static ProfilerMarker k_SaveGlyphVertexDataMarker = new ProfilerMarker("TMP Save Glyph Vertex Data");

		private static ProfilerMarker k_ComputeCharacterAdvanceMarker = new ProfilerMarker("TMP Compute Character Advance");

		private static ProfilerMarker k_HandleCarriageReturnMarker = new ProfilerMarker("TMP Handle Carriage Return");

		private static ProfilerMarker k_HandleLineTerminationMarker = new ProfilerMarker("TMP Handle Line Termination");

		private static ProfilerMarker k_SavePageInfoMarker = new ProfilerMarker("TMP Save Page Info");

		private static ProfilerMarker k_SaveTextExtentMarker = new ProfilerMarker("TMP Save Text Extent");

		private static ProfilerMarker k_SaveProcessingStatesMarker = new ProfilerMarker("TMP Save Processing States");

		private static ProfilerMarker k_GenerateTextPhaseIIMarker = new ProfilerMarker("TMP GenerateText - Phase II");

		private static ProfilerMarker k_GenerateTextPhaseIIIMarker = new ProfilerMarker("TMP GenerateText - Phase III");

		private Dictionary<int, int> materialIndexPairs = new Dictionary<int, int>();
	}
}
