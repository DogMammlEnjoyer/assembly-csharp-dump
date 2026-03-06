using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Profiling;
using UnityEngine.Bindings;
using UnityEngine.Serialization;
using UnityEngine.TextCore.LowLevel;

namespace UnityEngine.TextCore.Text
{
	[NativeHeader("Modules/TextCoreTextEngine/Native/FontAsset.h")]
	[ExcludeFromPreset]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class FontAsset : TextAsset
	{
		private static void EnsureAdditionalCapacity<T>(List<T> container, int additionalCapacity)
		{
			int num = container.Count + additionalCapacity;
			bool flag = container.Capacity < num;
			if (flag)
			{
				container.Capacity = num;
			}
		}

		private static void EnsureAdditionalCapacity<TKey, TValue>(Dictionary<TKey, TValue> container, int additionalCapacity)
		{
			int capacity = container.Count + additionalCapacity;
			container.EnsureCapacity(capacity);
		}

		public FontAssetCreationEditorSettings fontAssetCreationEditorSettings
		{
			get
			{
				return this.m_fontAssetCreationEditorSettings;
			}
			set
			{
				this.m_fontAssetCreationEditorSettings = value;
			}
		}

		public Font sourceFontFile
		{
			get
			{
				return this.m_SourceFontFile;
			}
			internal set
			{
				this.m_SourceFontFile = value;
			}
		}

		public AtlasPopulationMode atlasPopulationMode
		{
			get
			{
				return this.m_AtlasPopulationMode;
			}
			set
			{
				this.m_AtlasPopulationMode = value;
			}
		}

		public FaceInfo faceInfo
		{
			get
			{
				return this.m_FaceInfo;
			}
			set
			{
				this.m_FaceInfo = value;
				bool flag = this.m_NativeFontAsset != IntPtr.Zero;
				if (flag)
				{
					this.UpdateFaceInfo();
				}
			}
		}

		internal int familyNameHashCode
		{
			get
			{
				bool flag = this.m_FamilyNameHashCode == 0;
				if (flag)
				{
					this.m_FamilyNameHashCode = TextUtilities.GetHashCodeCaseInSensitive(this.m_FaceInfo.familyName);
				}
				return this.m_FamilyNameHashCode;
			}
			set
			{
				this.m_FamilyNameHashCode = value;
			}
		}

		internal int styleNameHashCode
		{
			get
			{
				bool flag = this.m_StyleNameHashCode == 0;
				if (flag)
				{
					this.m_StyleNameHashCode = TextUtilities.GetHashCodeCaseInSensitive(this.m_FaceInfo.styleName);
				}
				return this.m_StyleNameHashCode;
			}
			set
			{
				this.m_StyleNameHashCode = value;
			}
		}

		[Nullable(1)]
		public List<Glyph> glyphTable
		{
			[NullableContext(1)]
			get
			{
				return this.m_GlyphTable;
			}
			[NullableContext(1)]
			internal set
			{
				this.m_GlyphTable = value;
			}
		}

		public Dictionary<uint, Glyph> glyphLookupTable
		{
			get
			{
				bool flag = this.m_GlyphLookupDictionary == null;
				if (flag)
				{
					this.ReadFontAssetDefinition();
				}
				return this.m_GlyphLookupDictionary;
			}
		}

		public List<Character> characterTable
		{
			get
			{
				return this.m_CharacterTable;
			}
			internal set
			{
				this.m_CharacterTable = value;
			}
		}

		public Dictionary<uint, Character> characterLookupTable
		{
			get
			{
				bool flag = this.m_CharacterLookupDictionary == null;
				if (flag)
				{
					this.ReadFontAssetDefinition();
				}
				return this.m_CharacterLookupDictionary;
			}
		}

		public Texture2D atlasTexture
		{
			get
			{
				bool flag = this.m_AtlasTexture == null;
				if (flag)
				{
					this.m_AtlasTexture = this.atlasTextures[0];
				}
				return this.m_AtlasTexture;
			}
		}

		public Texture2D[] atlasTextures
		{
			get
			{
				return this.m_AtlasTextures;
			}
			set
			{
				this.m_AtlasTextures = value;
			}
		}

		public int atlasTextureCount
		{
			get
			{
				return this.m_AtlasTextureIndex + 1;
			}
		}

		public bool isMultiAtlasTexturesEnabled
		{
			get
			{
				return this.m_IsMultiAtlasTexturesEnabled;
			}
			set
			{
				this.m_IsMultiAtlasTexturesEnabled = value;
			}
		}

		public bool getFontFeatures
		{
			get
			{
				return this.m_GetFontFeatures;
			}
			set
			{
				this.m_GetFontFeatures = value;
			}
		}

		internal bool clearDynamicDataOnBuild
		{
			get
			{
				return this.m_ClearDynamicDataOnBuild;
			}
			set
			{
				this.m_ClearDynamicDataOnBuild = value;
			}
		}

		public int atlasWidth
		{
			get
			{
				return this.m_AtlasWidth;
			}
			internal set
			{
				this.m_AtlasWidth = value;
			}
		}

		public int atlasHeight
		{
			get
			{
				return this.m_AtlasHeight;
			}
			internal set
			{
				this.m_AtlasHeight = value;
			}
		}

		public int atlasPadding
		{
			get
			{
				return this.m_AtlasPadding;
			}
			internal set
			{
				this.m_AtlasPadding = value;
			}
		}

		public GlyphRenderMode atlasRenderMode
		{
			get
			{
				return this.m_AtlasRenderMode;
			}
			internal set
			{
				this.m_AtlasRenderMode = value;
			}
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEngine.IMGUIModule",
			"UnityEngine.UIElementsModule"
		})]
		internal bool IsBitmap()
		{
			return ((GlyphRasterModes)this.m_AtlasRenderMode).HasFlag(GlyphRasterModes.RASTER_MODE_BITMAP) && !((GlyphRasterModes)this.m_AtlasRenderMode).HasFlag(GlyphRasterModes.RASTER_MODE_COLOR);
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEngine.IMGUIModule",
			"UnityEngine.UIElementsModule"
		})]
		internal bool IsRaster()
		{
			return this.m_AtlasRenderMode == GlyphRenderMode.RASTER_HINTED;
		}

		internal List<GlyphRect> usedGlyphRects
		{
			get
			{
				return this.m_UsedGlyphRects;
			}
			set
			{
				this.m_UsedGlyphRects = value;
			}
		}

		internal List<GlyphRect> freeGlyphRects
		{
			get
			{
				return this.m_FreeGlyphRects;
			}
			set
			{
				this.m_FreeGlyphRects = value;
			}
		}

		public FontFeatureTable fontFeatureTable
		{
			get
			{
				return this.m_FontFeatureTable;
			}
			internal set
			{
				this.m_FontFeatureTable = value;
			}
		}

		public List<FontAsset> fallbackFontAssetTable
		{
			get
			{
				return this.m_FallbackFontAssetTable;
			}
			set
			{
				this.m_FallbackFontAssetTable = value;
			}
		}

		public FontWeightPair[] fontWeightTable
		{
			get
			{
				return this.m_FontWeightTable;
			}
			internal set
			{
				this.m_FontWeightTable = value;
			}
		}

		public float regularStyleWeight
		{
			get
			{
				return this.m_RegularStyleWeight;
			}
			set
			{
				this.m_RegularStyleWeight = value;
			}
		}

		public float regularStyleSpacing
		{
			get
			{
				return this.m_RegularStyleSpacing;
			}
			set
			{
				this.m_RegularStyleSpacing = value;
			}
		}

		public float boldStyleWeight
		{
			get
			{
				return this.m_BoldStyleWeight;
			}
			set
			{
				this.m_BoldStyleWeight = value;
			}
		}

		public float boldStyleSpacing
		{
			get
			{
				return this.m_BoldStyleSpacing;
			}
			set
			{
				this.m_BoldStyleSpacing = value;
			}
		}

		public byte italicStyleSlant
		{
			get
			{
				return this.m_ItalicStyleSlant;
			}
			set
			{
				this.m_ItalicStyleSlant = value;
			}
		}

		public byte tabMultiple
		{
			get
			{
				return this.m_TabMultiple;
			}
			set
			{
				this.m_TabMultiple = value;
			}
		}

		public static FontAsset CreateFontAsset(string familyName, string styleName, int pointSize = 90)
		{
			FontAsset fontAsset = FontAsset.CreateFontAssetInternal(familyName, styleName, pointSize);
			bool flag = fontAsset == null;
			FontAsset result;
			if (flag)
			{
				Debug.Log(string.Concat(new string[]
				{
					"Unable to find a font file with the specified Family Name [",
					familyName,
					"] and Style [",
					styleName,
					"]."
				}));
				result = null;
			}
			else
			{
				result = fontAsset;
			}
			return result;
		}

		[NullableContext(1)]
		[return: Nullable(2)]
		internal static FontAsset CreateFontAssetInternal(string familyName, string styleName, int pointSize = 90)
		{
			FontReference fontReference;
			bool flag = FontEngine.TryGetSystemFontReference(familyName, styleName, out fontReference);
			FontAsset result;
			if (flag)
			{
				result = FontAsset.CreateFontAsset(fontReference.filePath, fontReference.faceIndex, pointSize, 9, GlyphRenderMode.DEFAULT, 1024, 1024, AtlasPopulationMode.DynamicOS, true);
			}
			else
			{
				result = null;
			}
			return result;
		}

		[NullableContext(1)]
		[return: Nullable(2)]
		internal static FontAsset CreateFontAsset(string familyName, string styleName, int pointSize, int padding, GlyphRenderMode renderMode)
		{
			FontReference fontReference;
			bool flag = FontEngine.TryGetSystemFontReference(familyName, styleName, out fontReference);
			FontAsset result;
			if (flag)
			{
				result = FontAsset.CreateFontAsset(fontReference.filePath, fontReference.faceIndex, pointSize, padding, renderMode, 1024, 1024, AtlasPopulationMode.DynamicOS, true);
			}
			else
			{
				result = null;
			}
			return result;
		}

		internal static List<FontAsset> CreateFontAssetOSFallbackList(string[] fallbacksFamilyNames, int pointSize = 90)
		{
			List<FontAsset> list = new List<FontAsset>();
			foreach (string familyName in fallbacksFamilyNames)
			{
				FontAsset fontAsset = FontAsset.CreateFontAssetFromFamilyName(familyName, pointSize);
				bool flag = fontAsset == null;
				if (!flag)
				{
					list.Add(fontAsset);
				}
			}
			return list;
		}

		internal static FontAsset CreateFontAssetWithOSFallbackList(string[] fallbacksFamilyNames, int pointSize = 90)
		{
			FontAsset fontAsset = null;
			foreach (string familyName in fallbacksFamilyNames)
			{
				FontAsset fontAsset2 = FontAsset.CreateFontAssetFromFamilyName(familyName, pointSize);
				bool flag = fontAsset2 == null;
				if (!flag)
				{
					bool flag2 = fontAsset == null;
					if (flag2)
					{
						fontAsset = fontAsset2;
					}
					bool flag3 = fontAsset.fallbackFontAssetTable == null;
					if (flag3)
					{
						fontAsset.fallbackFontAssetTable = new List<FontAsset>();
					}
					fontAsset.fallbackFontAssetTable.Add(fontAsset2);
				}
			}
			return fontAsset;
		}

		private static FontAsset CreateFontAssetFromFamilyName(string familyName, int pointSize = 90)
		{
			FontAsset fontAsset = null;
			FontReference fontReference;
			bool flag = FontEngine.TryGetSystemFontReference(familyName, null, out fontReference);
			if (flag)
			{
				fontAsset = FontAsset.CreateFontAsset(fontReference.filePath, fontReference.faceIndex, pointSize, 9, GlyphRenderMode.DEFAULT, 1024, 1024, AtlasPopulationMode.DynamicOS, true);
			}
			bool flag2 = fontAsset == null;
			FontAsset result;
			if (flag2)
			{
				result = null;
			}
			else
			{
				FontAssetFactory.SetHideFlags(fontAsset);
				fontAsset.isMultiAtlasTexturesEnabled = true;
				fontAsset.InternalDynamicOS = true;
				result = fontAsset;
			}
			return result;
		}

		public static FontAsset CreateFontAsset(string fontFilePath, int faceIndex, int samplingPointSize, int atlasPadding, GlyphRenderMode renderMode, int atlasWidth, int atlasHeight)
		{
			return FontAsset.CreateFontAsset(fontFilePath, faceIndex, samplingPointSize, atlasPadding, renderMode, atlasWidth, atlasHeight, AtlasPopulationMode.Dynamic, true);
		}

		private static FontAsset CreateFontAsset(string fontFilePath, int faceIndex, int samplingPointSize, int atlasPadding, GlyphRenderMode renderMode, int atlasWidth, int atlasHeight, AtlasPopulationMode atlasPopulationMode, bool enableMultiAtlasSupport = true)
		{
			bool flag = FontEngine.LoadFontFace(fontFilePath, (float)samplingPointSize, faceIndex) > FontEngineError.Success;
			FontAsset result;
			if (flag)
			{
				Debug.Log("Unable to load font face from [" + fontFilePath + "].");
				result = null;
			}
			else
			{
				FontAsset fontAsset = FontAsset.CreateFontAssetInstance(null, atlasPadding, renderMode, atlasWidth, atlasHeight, atlasPopulationMode, enableMultiAtlasSupport);
				bool flag2 = fontAsset;
				if (flag2)
				{
					fontAsset.m_SourceFontFilePath = fontFilePath;
				}
				result = fontAsset;
			}
			return result;
		}

		public static FontAsset CreateFontAsset(Font font)
		{
			return FontAsset.CreateFontAsset(font, 90, 9, GlyphRenderMode.SDFAA, 1024, 1024, AtlasPopulationMode.Dynamic, true);
		}

		public static FontAsset CreateFontAsset(Font font, int samplingPointSize, int atlasPadding, GlyphRenderMode renderMode, int atlasWidth, int atlasHeight, AtlasPopulationMode atlasPopulationMode = AtlasPopulationMode.Dynamic, bool enableMultiAtlasSupport = true)
		{
			return FontAsset.CreateFontAsset(font, 0, samplingPointSize, atlasPadding, renderMode, atlasWidth, atlasHeight, atlasPopulationMode, enableMultiAtlasSupport);
		}

		private static FontAsset CreateFontAsset(Font font, int faceIndex, int samplingPointSize, int atlasPadding, GlyphRenderMode renderMode, int atlasWidth, int atlasHeight, AtlasPopulationMode atlasPopulationMode = AtlasPopulationMode.Dynamic, bool enableMultiAtlasSupport = true)
		{
			bool flag = font.name == "LegacyRuntime";
			if (flag)
			{
				string[] osfallbacks = Font.GetOSFallbacks();
				bool flag2 = FontEngine.LoadFontFace(font, (float)samplingPointSize, faceIndex) == FontEngineError.Success;
				if (flag2)
				{
					FontAsset fontAsset = FontAsset.CreateFontAssetInstance(font, atlasPadding, renderMode, atlasWidth, atlasHeight, atlasPopulationMode, enableMultiAtlasSupport);
					List<FontAsset> fallbackFontAssetTable = FontAsset.CreateFontAssetOSFallbackList(osfallbacks, samplingPointSize);
					fontAsset.fallbackFontAssetTable = fallbackFontAssetTable;
					return fontAsset;
				}
				FontAsset fontAsset2 = FontAsset.CreateFontAssetWithOSFallbackList(osfallbacks, samplingPointSize);
				bool flag3 = fontAsset2 != null;
				if (flag3)
				{
					return fontAsset2;
				}
			}
			bool flag4 = FontEngine.LoadFontFace(font, (float)samplingPointSize, faceIndex) > FontEngineError.Success;
			FontAsset result;
			if (flag4)
			{
				FontAsset fontAsset3 = FontAsset.CreateFontAsset(font.name, "Regular", 90);
				bool flag5 = fontAsset3 != null;
				if (flag5)
				{
					result = fontAsset3;
				}
				else
				{
					Debug.LogWarning("Unable to load font face for [" + font.name + "]. Make sure \"Include Font Data\" is enabled in the Font Import Settings.", font);
					result = null;
				}
			}
			else
			{
				result = FontAsset.CreateFontAssetInstance(font, atlasPadding, renderMode, atlasWidth, atlasHeight, atlasPopulationMode, enableMultiAtlasSupport);
			}
			return result;
		}

		private static FontAsset CreateFontAssetInstance(Font font, int atlasPadding, GlyphRenderMode renderMode, int atlasWidth, int atlasHeight, AtlasPopulationMode atlasPopulationMode, bool enableMultiAtlasSupport)
		{
			FontAsset fontAsset = ScriptableObject.CreateInstance<FontAsset>();
			fontAsset.m_Version = "1.1.0";
			fontAsset.faceInfo = FontEngine.GetFaceInfo();
			bool flag = renderMode == GlyphRenderMode.DEFAULT;
			if (flag)
			{
				renderMode = (FontEngine.IsColorFontFace() ? GlyphRenderMode.COLOR : GlyphRenderMode.SDFAA);
			}
			bool flag2 = atlasPopulationMode == AtlasPopulationMode.Dynamic && font != null;
			if (flag2)
			{
				fontAsset.sourceFontFile = font;
			}
			fontAsset.atlasPopulationMode = atlasPopulationMode;
			fontAsset.atlasWidth = atlasWidth;
			fontAsset.atlasHeight = atlasHeight;
			fontAsset.atlasPadding = atlasPadding;
			fontAsset.atlasRenderMode = renderMode;
			fontAsset.atlasTextures = new Texture2D[1];
			TextureFormat textureFormat = ((renderMode & (GlyphRenderMode)65536) == (GlyphRenderMode)65536) ? TextureFormat.RGBA32 : TextureFormat.Alpha8;
			Texture2D texture2D = new Texture2D(1, 1, textureFormat, false);
			fontAsset.atlasTextures[0] = texture2D;
			fontAsset.isMultiAtlasTexturesEnabled = enableMultiAtlasSupport;
			bool flag3 = (renderMode & (GlyphRenderMode)16) == (GlyphRenderMode)16;
			int num;
			if (flag3)
			{
				num = 0;
				bool flag4 = textureFormat == TextureFormat.Alpha8;
				Material material;
				if (flag4)
				{
					bool flag5 = TextShaderUtilities.ShaderRef_MobileBitmap;
					if (!flag5)
					{
						return null;
					}
					material = new Material(TextShaderUtilities.ShaderRef_MobileBitmap);
				}
				else
				{
					bool flag6 = TextShaderUtilities.ShaderRef_Sprite;
					if (!flag6)
					{
						return null;
					}
					material = new Material(TextShaderUtilities.ShaderRef_Sprite);
				}
				material.SetTexture(TextShaderUtilities.ID_MainTex, texture2D);
				material.SetFloat(TextShaderUtilities.ID_TextureWidth, (float)atlasWidth);
				material.SetFloat(TextShaderUtilities.ID_TextureHeight, (float)atlasHeight);
				fontAsset.material = material;
			}
			else
			{
				num = 1;
				Material material2 = new Material(TextShaderUtilities.ShaderRef_MobileSDF);
				material2.SetTexture(TextShaderUtilities.ID_MainTex, texture2D);
				material2.SetFloat(TextShaderUtilities.ID_TextureWidth, (float)atlasWidth);
				material2.SetFloat(TextShaderUtilities.ID_TextureHeight, (float)atlasHeight);
				material2.SetFloat(TextShaderUtilities.ID_GradientScale, (float)(atlasPadding + num));
				material2.SetFloat(TextShaderUtilities.ID_WeightNormal, fontAsset.regularStyleWeight);
				material2.SetFloat(TextShaderUtilities.ID_WeightBold, fontAsset.boldStyleWeight);
				fontAsset.material = material2;
			}
			fontAsset.freeGlyphRects = new List<GlyphRect>(8)
			{
				new GlyphRect(0, 0, atlasWidth - num, atlasHeight - num)
			};
			fontAsset.usedGlyphRects = new List<GlyphRect>(8);
			fontAsset.ReadFontAssetDefinition();
			return fontAsset;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEngine.UIElementsModule"
		})]
		internal static FontAsset GetFontAssetByID(int id)
		{
			return FontAsset.kFontAssetByInstanceId[id];
		}

		private void RegisterCallbackInstance(FontAsset instance)
		{
			for (int i = 0; i < FontAsset.s_CallbackInstances.Count; i++)
			{
				FontAsset x;
				bool flag = FontAsset.s_CallbackInstances[i].TryGetTarget(out x) && x == instance;
				if (flag)
				{
					return;
				}
			}
			for (int j = 0; j < FontAsset.s_CallbackInstances.Count; j++)
			{
				FontAsset fontAsset;
				bool flag2 = !FontAsset.s_CallbackInstances[j].TryGetTarget(out fontAsset);
				if (flag2)
				{
					FontAsset.s_CallbackInstances[j] = new WeakReference<FontAsset>(instance);
					return;
				}
			}
			FontAsset.s_CallbackInstances.Add(new WeakReference<FontAsset>(this));
		}

		private void OnDestroy()
		{
			FontAsset.kFontAssetByInstanceId.Remove(base.instanceID);
			bool flag = !this.m_IsClone;
			if (flag)
			{
				this.DestroyAtlasTextures();
				bool flag2 = this.m_Material;
				if (flag2)
				{
					Object.Destroy(this.m_Material);
				}
				this.m_Material = null;
			}
			bool flag3 = this.m_NativeFontAsset != IntPtr.Zero;
			if (flag3)
			{
				FontAsset.Destroy(this.m_NativeFontAsset);
				this.m_NativeFontAsset = IntPtr.Zero;
			}
		}

		public void ReadFontAssetDefinition()
		{
			this.InitializeDictionaryLookupTables();
			this.AddSynthesizedCharactersAndFaceMetrics();
			Character character;
			bool flag = this.m_FaceInfo.capLine == 0f && this.m_CharacterLookupDictionary.TryGetValue(88U, out character);
			if (flag)
			{
				uint glyphIndex = character.glyphIndex;
				this.m_FaceInfo.capLine = this.m_GlyphLookupDictionary[glyphIndex].metrics.horizontalBearingY;
			}
			bool flag2 = this.m_FaceInfo.meanLine == 0f && this.m_CharacterLookupDictionary.TryGetValue(88U, out character);
			if (flag2)
			{
				uint glyphIndex2 = character.glyphIndex;
				this.m_FaceInfo.meanLine = this.m_GlyphLookupDictionary[glyphIndex2].metrics.horizontalBearingY;
			}
			bool flag3 = this.m_FaceInfo.scale == 0f;
			if (flag3)
			{
				this.m_FaceInfo.scale = 1f;
			}
			bool flag4 = this.m_FaceInfo.strikethroughOffset == 0f;
			if (flag4)
			{
				this.m_FaceInfo.strikethroughOffset = this.m_FaceInfo.capLine / 2.5f;
			}
			bool flag5 = this.m_AtlasPadding == 0;
			if (flag5)
			{
				bool flag6 = base.material.HasProperty(TextShaderUtilities.ID_GradientScale);
				if (flag6)
				{
					this.m_AtlasPadding = (int)base.material.GetFloat(TextShaderUtilities.ID_GradientScale) - 1;
				}
			}
			bool flag7 = this.m_FaceInfo.unitsPerEM == 0 && this.atlasPopulationMode > AtlasPopulationMode.Static;
			if (flag7)
			{
				bool flag8 = !JobsUtility.IsExecutingJob;
				if (flag8)
				{
					this.m_FaceInfo.unitsPerEM = FontEngine.GetFaceInfo().unitsPerEM;
					Debug.Log(string.Concat(new string[]
					{
						"Font Asset [",
						base.name,
						"] Units Per EM set to ",
						this.m_FaceInfo.unitsPerEM.ToString(),
						". Please commit the newly serialized value."
					}), this);
				}
				else
				{
					Debug.LogError(string.Concat(new string[]
					{
						"Font Asset [",
						base.name,
						"] is missing Units Per EM. Please select the 'Reset FaceInfo' menu item on Font Asset [",
						base.name,
						"] to ensure proper serialization."
					}), this);
				}
			}
			base.hashCode = TextUtilities.GetHashCodeCaseInSensitive(base.name);
			this.familyNameHashCode = TextUtilities.GetHashCodeCaseInSensitive(this.m_FaceInfo.familyName);
			this.styleNameHashCode = TextUtilities.GetHashCodeCaseInSensitive(this.m_FaceInfo.styleName);
			base.materialHashCode = TextUtilities.GetHashCodeCaseInSensitive(base.name + FontAsset.s_DefaultMaterialSuffix);
			TextResourceManager.AddFontAsset(this);
			this.IsFontAssetLookupTablesDirty = false;
			this.RegisterCallbackInstance(this);
		}

		internal void InitializeDictionaryLookupTables()
		{
			this.InitializeGlyphLookupDictionary();
			this.InitializeCharacterLookupDictionary();
			bool flag = (this.m_AtlasPopulationMode == AtlasPopulationMode.Dynamic || this.m_AtlasPopulationMode == AtlasPopulationMode.DynamicOS) && this.m_ShouldReimportFontFeatures;
			if (flag)
			{
				this.ImportFontFeatures();
			}
			this.InitializeLigatureSubstitutionLookupDictionary();
			this.InitializeGlyphPairAdjustmentRecordsLookupDictionary();
			this.InitializeMarkToBaseAdjustmentRecordsLookupDictionary();
			this.InitializeMarkToMarkAdjustmentRecordsLookupDictionary();
		}

		private static void InitializeLookup<T>(ICollection source, ref Dictionary<uint, T> lookup, int defaultCapacity = 16)
		{
			int capacity = (source != null) ? source.Count : defaultCapacity;
			bool flag = lookup == null;
			if (flag)
			{
				lookup = new Dictionary<uint, T>(capacity);
			}
			else
			{
				lookup.Clear();
				lookup.EnsureCapacity(capacity);
			}
		}

		private static void InitializeList<T>(ICollection source, ref List<T> list, int defaultCapacity = 16)
		{
			int capacity = (source != null) ? source.Count : defaultCapacity;
			bool flag = list == null;
			if (flag)
			{
				list = new List<T>(capacity);
			}
			else
			{
				list.Clear();
				list.Capacity = capacity;
			}
		}

		internal void InitializeGlyphLookupDictionary()
		{
			FontAsset.InitializeLookup<Glyph>(this.m_GlyphTable, ref this.m_GlyphLookupDictionary, 16);
			FontAsset.InitializeList<uint>(this.m_GlyphTable, ref this.m_GlyphIndexList, 16);
			FontAsset.InitializeList<uint>(null, ref this.m_GlyphIndexListNewlyAdded, 16);
			foreach (Glyph glyph in this.m_GlyphTable)
			{
				uint index = glyph.index;
				bool flag = this.m_GlyphLookupDictionary.TryAdd(index, glyph);
				if (flag)
				{
					this.m_GlyphIndexList.Add(index);
				}
			}
		}

		internal void InitializeCharacterLookupDictionary()
		{
			FontAsset.InitializeLookup<Character>(this.m_CharacterTable, ref this.m_CharacterLookupDictionary, 16);
			foreach (Character character in this.m_CharacterTable)
			{
				uint unicode = character.unicode;
				uint glyphIndex = character.glyphIndex;
				bool flag = this.m_CharacterLookupDictionary.TryAdd(unicode, character);
				if (flag)
				{
					character.textAsset = this;
					character.glyph = this.m_GlyphLookupDictionary[glyphIndex];
				}
			}
			HashSet<uint> missingUnicodesFromFontFile = this.m_MissingUnicodesFromFontFile;
			if (missingUnicodesFromFontFile != null)
			{
				missingUnicodesFromFontFile.Clear();
			}
		}

		internal void ClearFallbackCharacterTable()
		{
			List<uint> list = new List<uint>();
			foreach (KeyValuePair<uint, Character> keyValuePair in this.m_CharacterLookupDictionary)
			{
				Character value = keyValuePair.Value;
				bool flag = value.textAsset != this;
				if (flag)
				{
					list.Add(keyValuePair.Key);
				}
			}
			foreach (uint key in list)
			{
				this.m_CharacterLookupDictionary.Remove(key);
			}
		}

		internal void InitializeLigatureSubstitutionLookupDictionary()
		{
			List<LigatureSubstitutionRecord> ligatureSubstitutionRecords = this.m_FontFeatureTable.m_LigatureSubstitutionRecords;
			FontAsset.InitializeLookup<List<LigatureSubstitutionRecord>>(ligatureSubstitutionRecords, ref this.m_FontFeatureTable.m_LigatureSubstitutionRecordLookup, 16);
			bool flag = ligatureSubstitutionRecords == null;
			if (!flag)
			{
				foreach (LigatureSubstitutionRecord item in ligatureSubstitutionRecords)
				{
					bool flag2 = item.componentGlyphIDs == null || item.componentGlyphIDs.Length == 0;
					if (!flag2)
					{
						uint key = item.componentGlyphIDs[0];
						List<LigatureSubstitutionRecord> list;
						bool flag3 = this.m_FontFeatureTable.m_LigatureSubstitutionRecordLookup.TryGetValue(key, out list);
						if (flag3)
						{
							list.Add(item);
						}
						else
						{
							this.m_FontFeatureTable.m_LigatureSubstitutionRecordLookup.Add(key, new List<LigatureSubstitutionRecord>
							{
								item
							});
						}
					}
				}
			}
		}

		internal void InitializeGlyphPairAdjustmentRecordsLookupDictionary()
		{
			List<GlyphPairAdjustmentRecord> glyphPairAdjustmentRecords = this.m_FontFeatureTable.glyphPairAdjustmentRecords;
			FontAsset.InitializeLookup<GlyphPairAdjustmentRecord>(glyphPairAdjustmentRecords, ref this.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup, 16);
			bool flag = glyphPairAdjustmentRecords == null;
			if (!flag)
			{
				foreach (GlyphPairAdjustmentRecord value in glyphPairAdjustmentRecords)
				{
					uint key = value.secondAdjustmentRecord.glyphIndex << 16 | value.firstAdjustmentRecord.glyphIndex;
					this.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.TryAdd(key, value);
				}
			}
		}

		internal void InitializeMarkToBaseAdjustmentRecordsLookupDictionary()
		{
			List<MarkToBaseAdjustmentRecord> markToBaseAdjustmentRecords = this.m_FontFeatureTable.m_MarkToBaseAdjustmentRecords;
			FontAsset.InitializeLookup<MarkToBaseAdjustmentRecord>(markToBaseAdjustmentRecords, ref this.m_FontFeatureTable.m_MarkToBaseAdjustmentRecordLookup, 16);
			bool flag = markToBaseAdjustmentRecords == null;
			if (!flag)
			{
				foreach (MarkToBaseAdjustmentRecord value in markToBaseAdjustmentRecords)
				{
					uint key = value.markGlyphID << 16 | value.baseGlyphID;
					this.m_FontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.TryAdd(key, value);
				}
			}
		}

		internal void InitializeMarkToMarkAdjustmentRecordsLookupDictionary()
		{
			List<MarkToMarkAdjustmentRecord> markToMarkAdjustmentRecords = this.m_FontFeatureTable.m_MarkToMarkAdjustmentRecords;
			FontAsset.InitializeLookup<MarkToMarkAdjustmentRecord>(markToMarkAdjustmentRecords, ref this.m_FontFeatureTable.m_MarkToMarkAdjustmentRecordLookup, 16);
			bool flag = markToMarkAdjustmentRecords == null;
			if (!flag)
			{
				foreach (MarkToMarkAdjustmentRecord value in markToMarkAdjustmentRecords)
				{
					uint key = value.combiningMarkGlyphID << 16 | value.baseMarkGlyphID;
					this.m_FontFeatureTable.m_MarkToMarkAdjustmentRecordLookup.TryAdd(key, value);
				}
			}
		}

		internal void AddSynthesizedCharactersAndFaceMetrics()
		{
			bool flag = false;
			bool flag2 = this.m_AtlasPopulationMode == AtlasPopulationMode.Dynamic || this.m_AtlasPopulationMode == AtlasPopulationMode.DynamicOS;
			if (flag2)
			{
				flag = (this.LoadFontFace() == FontEngineError.Success);
				bool flag3 = !flag && !this.InternalDynamicOS;
				if (flag3)
				{
					Debug.LogWarning("Unable to load font face for [" + base.name + "] font asset.", this);
				}
			}
			this.AddSynthesizedCharacter(3U, flag, true);
			this.AddSynthesizedCharacter(9U, flag, true);
			this.AddSynthesizedCharacter(10U, flag, false);
			this.AddSynthesizedCharacter(11U, flag, false);
			this.AddSynthesizedCharacter(13U, flag, false);
			this.AddSynthesizedCharacter(1564U, flag, false);
			this.AddSynthesizedCharacter(8203U, flag, false);
			this.AddSynthesizedCharacter(8206U, flag, false);
			this.AddSynthesizedCharacter(8207U, flag, false);
			this.AddSynthesizedCharacter(8232U, flag, false);
			this.AddSynthesizedCharacter(8233U, flag, false);
			this.AddSynthesizedCharacter(8288U, flag, false);
		}

		private void AddSynthesizedCharacter(uint unicode, bool isFontFaceLoaded, bool addImmediately = false)
		{
			bool flag = this.m_CharacterLookupDictionary.ContainsKey(unicode);
			if (!flag)
			{
				Glyph glyph;
				Character value;
				if (isFontFaceLoaded)
				{
					bool flag2 = FontEngine.GetGlyphIndex(unicode) > 0U;
					if (flag2)
					{
						bool flag3 = !addImmediately;
						if (flag3)
						{
							return;
						}
						GlyphLoadFlags flags = ((this.m_AtlasRenderMode & (GlyphRenderMode)4) == (GlyphRenderMode)4) ? (GlyphLoadFlags.LOAD_NO_HINTING | GlyphLoadFlags.LOAD_NO_BITMAP) : GlyphLoadFlags.LOAD_NO_BITMAP;
						bool flag4 = FontEngine.TryGetGlyphWithUnicodeValue(unicode, flags, out glyph);
						if (flag4)
						{
							value = new Character(unicode, this, glyph);
							foreach (object obj in Enum.GetValues(typeof(TextFontWeight)))
							{
								TextFontWeight fontWeight = (TextFontWeight)obj;
								this.m_CharacterLookupDictionary.Add(this.CreateCompositeKey(unicode, FontStyles.Normal, fontWeight), value);
								this.m_CharacterLookupDictionary.Add(this.CreateCompositeKey(unicode, FontStyles.Italic, fontWeight), value);
							}
						}
						return;
					}
				}
				glyph = new Glyph(0U, new GlyphMetrics(0f, 0f, 0f, 0f, 0f), GlyphRect.zero, 1f, 0);
				value = new Character(unicode, this, glyph);
				foreach (object obj2 in Enum.GetValues(typeof(TextFontWeight)))
				{
					TextFontWeight fontWeight2 = (TextFontWeight)obj2;
					this.m_CharacterLookupDictionary.Add(this.CreateCompositeKey(unicode, FontStyles.Normal, fontWeight2), value);
					this.m_CharacterLookupDictionary.Add(this.CreateCompositeKey(unicode, FontStyles.Italic, fontWeight2), value);
				}
			}
		}

		internal void AddCharacterToLookupCache(uint unicode, Character character)
		{
			this.AddCharacterToLookupCache(unicode, character, FontStyles.Normal, TextFontWeight.Regular);
		}

		internal void AddCharacterToLookupCache(uint unicode, Character character, FontStyles fontStyle, TextFontWeight fontWeight)
		{
			bool flag = this.m_CharacterLookupDictionary == null;
			if (flag)
			{
				this.ReadFontAssetDefinition();
			}
			this.m_CharacterLookupDictionary.TryAdd(this.CreateCompositeKey(unicode, fontStyle, fontWeight), character);
		}

		internal bool GetCharacterInLookupCache(uint unicode, FontStyles fontStyle, TextFontWeight fontWeight, out Character character)
		{
			bool flag = this.m_CharacterLookupDictionary == null;
			if (flag)
			{
				this.ReadFontAssetDefinition();
			}
			return this.m_CharacterLookupDictionary.TryGetValue(this.CreateCompositeKey(unicode, fontStyle, fontWeight), out character);
		}

		internal void RemoveCharacterInLookupCache(uint unicode, FontStyles fontStyle, TextFontWeight fontWeight)
		{
			bool flag = this.m_CharacterLookupDictionary == null;
			if (flag)
			{
				this.ReadFontAssetDefinition();
			}
			this.m_CharacterLookupDictionary.Remove(this.CreateCompositeKey(unicode, fontStyle, fontWeight));
		}

		internal bool ContainsCharacterInLookupCache(uint unicode, FontStyles fontStyle, TextFontWeight fontWeight)
		{
			bool flag = this.m_CharacterLookupDictionary == null;
			if (flag)
			{
				this.ReadFontAssetDefinition();
			}
			return this.m_CharacterLookupDictionary.ContainsKey(this.CreateCompositeKey(unicode, fontStyle, fontWeight));
		}

		private uint CreateCompositeKey(uint unicode, FontStyles fontStyle = FontStyles.Normal, TextFontWeight fontWeight = TextFontWeight.Regular)
		{
			bool flag = fontStyle == FontStyles.Normal && fontWeight == TextFontWeight.Regular;
			uint result;
			if (flag)
			{
				result = unicode;
			}
			else
			{
				bool flag2 = (fontStyle & FontStyles.Italic) == FontStyles.Italic;
				int num = 0;
				bool flag3 = fontWeight != TextFontWeight.Regular;
				if (flag3)
				{
					num = TextUtilities.GetTextFontWeightIndex(fontWeight);
				}
				uint num2 = unicode & 2097151U;
				uint num3 = (uint)((uint)(num & 15) << 21);
				uint num4 = flag2 ? 33554432U : 0U;
				uint num5 = num2 | num3 | num4;
				result = num5;
			}
			return result;
		}

		internal FontEngineError LoadFontFace()
		{
			bool flag = this.m_AtlasPopulationMode == AtlasPopulationMode.Dynamic;
			FontEngineError result;
			if (flag)
			{
				bool flag2 = FontEngine.LoadFontFace(this.m_SourceFontFile, this.m_FaceInfo.pointSize, this.m_FaceInfo.faceIndex) == FontEngineError.Success;
				if (flag2)
				{
					result = FontEngineError.Success;
				}
				else
				{
					bool flag3 = !string.IsNullOrEmpty(this.m_SourceFontFilePath);
					if (flag3)
					{
						result = FontEngine.LoadFontFace(this.m_SourceFontFilePath, this.m_FaceInfo.pointSize, this.m_FaceInfo.faceIndex);
					}
					else
					{
						result = FontEngineError.Invalid_Face;
					}
				}
			}
			else
			{
				result = FontEngine.LoadFontFace(this.m_FaceInfo.familyName, this.m_FaceInfo.styleName, this.m_FaceInfo.pointSize);
			}
			return result;
		}

		internal void SortCharacterTable()
		{
			bool flag = this.m_CharacterTable != null && this.m_CharacterTable.Count > 0;
			if (flag)
			{
				this.m_CharacterTable = (from c in this.m_CharacterTable
				orderby c.unicode
				select c).ToList<Character>();
			}
		}

		internal void SortGlyphTable()
		{
			bool flag = this.m_GlyphTable != null && this.m_GlyphTable.Count > 0;
			if (flag)
			{
				this.m_GlyphTable = (from c in this.m_GlyphTable
				orderby c.index
				select c).ToList<Glyph>();
			}
		}

		internal void SortFontFeatureTable()
		{
			this.m_FontFeatureTable.SortGlyphPairAdjustmentRecords();
			this.m_FontFeatureTable.SortMarkToBaseAdjustmentRecords();
			this.m_FontFeatureTable.SortMarkToMarkAdjustmentRecords();
		}

		internal void SortAllTables()
		{
			this.SortGlyphTable();
			this.SortCharacterTable();
			this.SortFontFeatureTable();
		}

		public bool HasCharacter(int character)
		{
			bool flag = this.characterLookupTable == null;
			return !flag && this.m_CharacterLookupDictionary.ContainsKey((uint)character);
		}

		public bool HasCharacter(char character, bool searchFallbacks = false, bool tryAddCharacter = false)
		{
			return this.HasCharacter((uint)character, searchFallbacks, tryAddCharacter);
		}

		public bool HasCharacter(uint character, bool searchFallbacks = false, bool tryAddCharacter = false)
		{
			bool flag = this.characterLookupTable == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = this.m_CharacterLookupDictionary.ContainsKey(character);
				if (flag2)
				{
					result = true;
				}
				else
				{
					bool flag3 = tryAddCharacter && (this.m_AtlasPopulationMode == AtlasPopulationMode.Dynamic || this.m_AtlasPopulationMode == AtlasPopulationMode.DynamicOS);
					if (flag3)
					{
						Character character2;
						bool flag4 = this.TryAddCharacterInternal(character, FontStyles.Normal, TextFontWeight.Regular, out character2, true);
						if (flag4)
						{
							return true;
						}
					}
					if (searchFallbacks)
					{
						bool flag5 = FontAsset.k_SearchedFontAssetLookup == null;
						if (flag5)
						{
							FontAsset.k_SearchedFontAssetLookup = new HashSet<int>();
						}
						else
						{
							FontAsset.k_SearchedFontAssetLookup.Clear();
						}
						FontAsset.k_SearchedFontAssetLookup.Add(base.GetInstanceID());
						bool flag6 = this.fallbackFontAssetTable != null && this.fallbackFontAssetTable.Count > 0;
						if (flag6)
						{
							int num = 0;
							while (num < this.fallbackFontAssetTable.Count && this.fallbackFontAssetTable[num] != null)
							{
								FontAsset fontAsset = this.fallbackFontAssetTable[num];
								int instanceID = fontAsset.GetInstanceID();
								bool flag7 = FontAsset.k_SearchedFontAssetLookup.Add(instanceID);
								if (flag7)
								{
									bool flag8 = fontAsset.HasCharacter_Internal(character, FontStyles.Normal, TextFontWeight.Regular, true, tryAddCharacter);
									if (flag8)
									{
										return true;
									}
								}
								num++;
							}
						}
					}
					result = false;
				}
			}
			return result;
		}

		private bool HasCharacterWithStyle_Internal(uint character, FontStyles fontStyle, TextFontWeight fontWeight, bool searchFallbacks = false, bool tryAddCharacter = false)
		{
			return this.HasCharacter_Internal(character, fontStyle, fontWeight, searchFallbacks, tryAddCharacter);
		}

		private bool HasCharacter_Internal(uint character, FontStyles fontStyle = FontStyles.Normal, TextFontWeight fontWeight = TextFontWeight.Regular, bool searchFallbacks = false, bool tryAddCharacter = false)
		{
			bool flag = this.m_CharacterLookupDictionary == null;
			if (flag)
			{
				this.ReadFontAssetDefinition();
				bool flag2 = this.m_CharacterLookupDictionary == null;
				if (flag2)
				{
					return false;
				}
			}
			bool flag3 = this.ContainsCharacterInLookupCache(character, fontStyle, fontWeight);
			bool result;
			if (flag3)
			{
				result = true;
			}
			else
			{
				bool flag4 = tryAddCharacter && (this.atlasPopulationMode == AtlasPopulationMode.Dynamic || this.m_AtlasPopulationMode == AtlasPopulationMode.DynamicOS);
				if (flag4)
				{
					Character character2;
					bool flag5 = this.TryAddCharacterInternal(character, fontStyle, fontWeight, out character2, true);
					if (flag5)
					{
						return true;
					}
				}
				if (searchFallbacks)
				{
					bool flag6 = this.fallbackFontAssetTable == null || this.fallbackFontAssetTable.Count == 0;
					if (flag6)
					{
						return false;
					}
					int num = 0;
					while (num < this.fallbackFontAssetTable.Count && this.fallbackFontAssetTable[num] != null)
					{
						FontAsset fontAsset = this.fallbackFontAssetTable[num];
						int instanceID = fontAsset.GetInstanceID();
						bool flag7 = FontAsset.k_SearchedFontAssetLookup.Add(instanceID);
						if (flag7)
						{
							bool flag8 = fontAsset.HasCharacter_Internal(character, fontStyle, fontWeight, true, tryAddCharacter);
							if (flag8)
							{
								return true;
							}
						}
						num++;
					}
				}
				result = false;
			}
			return result;
		}

		public bool HasCharacters(string text, out List<char> missingCharacters)
		{
			bool flag = this.characterLookupTable == null;
			bool result;
			if (flag)
			{
				missingCharacters = null;
				result = false;
			}
			else
			{
				missingCharacters = new List<char>();
				for (int i = 0; i < text.Length; i++)
				{
					uint codePoint = FontAssetUtilities.GetCodePoint(text, ref i);
					bool flag2 = !this.m_CharacterLookupDictionary.ContainsKey(codePoint);
					if (flag2)
					{
						missingCharacters.Add((char)codePoint);
					}
				}
				bool flag3 = missingCharacters.Count == 0;
				result = flag3;
			}
			return result;
		}

		public bool HasCharacters(string text, out uint[] missingCharacters, bool searchFallbacks = false, bool tryAddCharacter = false)
		{
			missingCharacters = null;
			bool flag = this.characterLookupTable == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this.s_MissingCharacterList.Clear();
				for (int i = 0; i < text.Length; i++)
				{
					bool flag2 = true;
					uint codePoint = FontAssetUtilities.GetCodePoint(text, ref i);
					bool flag3 = this.m_CharacterLookupDictionary.ContainsKey(codePoint);
					if (!flag3)
					{
						bool flag4 = tryAddCharacter && (this.atlasPopulationMode == AtlasPopulationMode.Dynamic || this.m_AtlasPopulationMode == AtlasPopulationMode.DynamicOS);
						if (flag4)
						{
							Character character;
							bool flag5 = this.TryAddCharacterInternal(codePoint, FontStyles.Normal, TextFontWeight.Regular, out character, true);
							if (flag5)
							{
								goto IL_190;
							}
						}
						if (searchFallbacks)
						{
							bool flag6 = FontAsset.k_SearchedFontAssetLookup == null;
							if (flag6)
							{
								FontAsset.k_SearchedFontAssetLookup = new HashSet<int>();
							}
							else
							{
								FontAsset.k_SearchedFontAssetLookup.Clear();
							}
							FontAsset.k_SearchedFontAssetLookup.Add(base.GetInstanceID());
							bool flag7 = this.fallbackFontAssetTable != null && this.fallbackFontAssetTable.Count > 0;
							if (flag7)
							{
								int num = 0;
								while (num < this.fallbackFontAssetTable.Count && this.fallbackFontAssetTable[num] != null)
								{
									FontAsset fontAsset = this.fallbackFontAssetTable[num];
									int instanceID = fontAsset.GetInstanceID();
									bool flag8 = FontAsset.k_SearchedFontAssetLookup.Add(instanceID);
									if (flag8)
									{
										bool flag9 = !fontAsset.HasCharacter_Internal(codePoint, FontStyles.Normal, TextFontWeight.Regular, true, tryAddCharacter);
										if (!flag9)
										{
											flag2 = false;
											break;
										}
									}
									num++;
								}
							}
						}
						bool flag10 = flag2;
						if (flag10)
						{
							this.s_MissingCharacterList.Add(codePoint);
						}
					}
					IL_190:;
				}
				bool flag11 = this.s_MissingCharacterList.Count > 0;
				if (flag11)
				{
					missingCharacters = this.s_MissingCharacterList.ToArray();
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		public bool HasCharacters(string text)
		{
			bool flag = this.characterLookupTable == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < text.Length; i++)
				{
					uint codePoint = FontAssetUtilities.GetCodePoint(text, ref i);
					bool flag2 = !this.m_CharacterLookupDictionary.ContainsKey(codePoint);
					if (flag2)
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		public static string GetCharacters(FontAsset fontAsset)
		{
			string text = string.Empty;
			for (int i = 0; i < fontAsset.characterTable.Count; i++)
			{
				text += ((char)fontAsset.characterTable[i].unicode).ToString();
			}
			return text;
		}

		public static int[] GetCharactersArray(FontAsset fontAsset)
		{
			int[] array = new int[fontAsset.characterTable.Count];
			for (int i = 0; i < fontAsset.characterTable.Count; i++)
			{
				array[i] = (int)fontAsset.characterTable[i].unicode;
			}
			return array;
		}

		internal uint GetGlyphIndex(uint unicode)
		{
			bool flag;
			return this.GetGlyphIndex(unicode, out flag);
		}

		internal uint GetGlyphIndex(uint unicode, out bool success)
		{
			success = true;
			Character character;
			bool flag = this.characterLookupTable.TryGetValue(unicode, out character);
			uint result;
			if (flag)
			{
				result = character.glyphIndex;
			}
			else
			{
				bool isExecutingJob = TextGenerator.IsExecutingJob;
				if (isExecutingJob)
				{
					success = false;
					result = 0U;
				}
				else
				{
					result = ((this.LoadFontFace() == FontEngineError.Success) ? FontEngine.GetGlyphIndex(unicode) : 0U);
				}
			}
			return result;
		}

		internal uint GetGlyphVariantIndex(uint unicode, uint variantSelectorUnicode)
		{
			return (this.LoadFontFace() == FontEngineError.Success) ? FontEngine.GetVariantGlyphIndex(unicode, variantSelectorUnicode) : 0U;
		}

		internal void UpdateFontAssetData()
		{
			uint[] array = new uint[this.m_CharacterTable.Count];
			for (int i = 0; i < this.m_CharacterTable.Count; i++)
			{
				array[i] = this.m_CharacterTable[i].unicode;
			}
			this.ClearCharacterAndGlyphTables();
			this.ClearFontFeaturesTables();
			this.ClearAtlasTextures(true);
			this.ReadFontAssetDefinition();
			bool flag = array.Length != 0;
			if (flag)
			{
				this.TryAddCharacters(array, this.m_GetFontFeatures);
			}
		}

		public void ClearFontAssetData(bool setAtlasSizeToZero = false)
		{
			using (FontAsset.k_ClearFontAssetDataMarker.Auto())
			{
				this.ClearCharacterAndGlyphTables();
				this.ClearFontFeaturesTables();
				this.ClearAtlasTextures(setAtlasSizeToZero);
				this.ReadFontAssetDefinition();
				for (int i = 0; i < FontAsset.s_CallbackInstances.Count; i++)
				{
					FontAsset fontAsset;
					bool flag = FontAsset.s_CallbackInstances[i].TryGetTarget(out fontAsset) && fontAsset != this;
					if (flag)
					{
						fontAsset.ClearFallbackCharacterTable();
					}
				}
				TextEventManager.ON_FONT_PROPERTY_CHANGED(true, this);
			}
		}

		internal void ClearCharacterAndGlyphTablesInternal()
		{
			this.ClearCharacterAndGlyphTables();
			this.ClearAtlasTextures(true);
			this.ReadFontAssetDefinition();
		}

		private void ClearCharacterAndGlyphTables()
		{
			bool flag = this.m_GlyphTable != null;
			if (flag)
			{
				this.m_GlyphTable.Clear();
			}
			bool flag2 = this.m_CharacterTable != null;
			if (flag2)
			{
				this.m_CharacterTable.Clear();
			}
			bool flag3 = this.m_UsedGlyphRects != null;
			if (flag3)
			{
				this.m_UsedGlyphRects.Clear();
			}
			bool flag4 = this.m_FreeGlyphRects != null;
			if (flag4)
			{
				int num = ((this.m_AtlasRenderMode & (GlyphRenderMode)16) == (GlyphRenderMode)16) ? 0 : 1;
				this.m_FreeGlyphRects.Clear();
				this.m_FreeGlyphRects.Add(new GlyphRect(0, 0, this.m_AtlasWidth - num, this.m_AtlasHeight - num));
			}
			bool flag5 = this.m_GlyphsToRender != null;
			if (flag5)
			{
				this.m_GlyphsToRender.Clear();
			}
			bool flag6 = this.m_GlyphsRendered != null;
			if (flag6)
			{
				this.m_GlyphsRendered.Clear();
			}
		}

		private void ClearFontFeaturesTables()
		{
			bool flag = this.m_FontFeatureTable != null && this.m_FontFeatureTable.m_LigatureSubstitutionRecords != null;
			if (flag)
			{
				this.m_FontFeatureTable.m_LigatureSubstitutionRecords.Clear();
			}
			bool flag2 = this.m_FontFeatureTable != null && this.m_FontFeatureTable.glyphPairAdjustmentRecords != null;
			if (flag2)
			{
				this.m_FontFeatureTable.glyphPairAdjustmentRecords.Clear();
			}
			bool flag3 = this.m_FontFeatureTable != null && this.m_FontFeatureTable.m_MarkToBaseAdjustmentRecords != null;
			if (flag3)
			{
				this.m_FontFeatureTable.m_MarkToBaseAdjustmentRecords.Clear();
			}
			bool flag4 = this.m_FontFeatureTable != null && this.m_FontFeatureTable.m_MarkToMarkAdjustmentRecords != null;
			if (flag4)
			{
				this.m_FontFeatureTable.m_MarkToMarkAdjustmentRecords.Clear();
			}
		}

		internal void ClearAtlasTextures(bool setAtlasSizeToZero = false)
		{
			this.m_AtlasTextureIndex = 0;
			bool flag = this.m_AtlasTextures == null;
			if (!flag)
			{
				Texture2D texture2D;
				for (int i = 1; i < this.m_AtlasTextures.Length; i++)
				{
					texture2D = this.m_AtlasTextures[i];
					bool flag2 = !texture2D;
					if (!flag2)
					{
						Object.Destroy(texture2D);
					}
				}
				Array.Resize<Texture2D>(ref this.m_AtlasTextures, 1);
				texture2D = (this.m_AtlasTexture = this.m_AtlasTextures[0]);
				bool flag3 = !texture2D.isReadable;
				if (flag3)
				{
				}
				TextureFormat format = ((this.m_AtlasRenderMode & (GlyphRenderMode)65536) == (GlyphRenderMode)65536) ? TextureFormat.RGBA32 : TextureFormat.Alpha8;
				if (setAtlasSizeToZero)
				{
					texture2D.Reinitialize(1, 1, format, false);
				}
				else
				{
					bool flag4 = texture2D.width != this.m_AtlasWidth || texture2D.height != this.m_AtlasHeight;
					if (flag4)
					{
						texture2D.Reinitialize(this.m_AtlasWidth, this.m_AtlasHeight, format, false);
					}
				}
				FontEngine.ResetAtlasTexture(texture2D);
				texture2D.Apply();
			}
		}

		private void DestroyAtlasTextures()
		{
			this.m_AtlasTexture = null;
			this.m_AtlasTextureIndex = -1;
			bool flag = this.m_AtlasTextures == null;
			if (!flag)
			{
				foreach (Texture2D texture2D in this.m_AtlasTextures)
				{
					bool flag2 = texture2D != null;
					if (flag2)
					{
						Object.Destroy(texture2D);
					}
				}
				this.m_AtlasTextures = null;
			}
		}

		internal static void RegisterFontAssetForFontFeatureUpdate(FontAsset fontAsset)
		{
			int instanceID = fontAsset.instanceID;
			bool flag = FontAsset.k_FontAssets_FontFeaturesUpdateQueueLookup.Add(instanceID);
			if (flag)
			{
				FontAsset.k_FontAssets_FontFeaturesUpdateQueue.Add(fontAsset);
			}
		}

		internal static void RegisterFontAssetForKerningUpdate(FontAsset fontAsset)
		{
			int instanceID = fontAsset.instanceID;
			bool flag = FontAsset.k_FontAssets_KerningUpdateQueueLookup.Add(instanceID);
			if (flag)
			{
				FontAsset.k_FontAssets_KerningUpdateQueue.Add(fontAsset);
			}
		}

		internal static void UpdateFontFeaturesForFontAssetsInQueue()
		{
			int count = FontAsset.k_FontAssets_FontFeaturesUpdateQueue.Count;
			for (int i = 0; i < count; i++)
			{
				FontAsset.k_FontAssets_FontFeaturesUpdateQueue[i].UpdateGPOSFontFeaturesForNewlyAddedGlyphs();
			}
			bool flag = count > 0;
			if (flag)
			{
				FontAsset.k_FontAssets_FontFeaturesUpdateQueue.Clear();
				FontAsset.k_FontAssets_FontFeaturesUpdateQueueLookup.Clear();
			}
			count = FontAsset.k_FontAssets_KerningUpdateQueue.Count;
			for (int j = 0; j < count; j++)
			{
				FontAsset.k_FontAssets_KerningUpdateQueue[j].UpdateGlyphAdjustmentRecordsForNewGlyphs();
			}
			bool flag2 = count > 0;
			if (flag2)
			{
				FontAsset.k_FontAssets_KerningUpdateQueue.Clear();
				FontAsset.k_FontAssets_KerningUpdateQueueLookup.Clear();
			}
		}

		internal static void RegisterAtlasTextureForApply(Texture2D texture)
		{
			int instanceID = texture.GetInstanceID();
			bool flag = FontAsset.k_FontAssets_AtlasTexturesUpdateQueueLookup.Add(instanceID);
			if (flag)
			{
				FontAsset.k_FontAssets_AtlasTexturesUpdateQueue.Add(texture);
			}
		}

		internal static void UpdateAtlasTexturesInQueue()
		{
			int count = FontAsset.k_FontAssets_AtlasTexturesUpdateQueueLookup.Count;
			for (int i = 0; i < count; i++)
			{
				FontAsset.k_FontAssets_AtlasTexturesUpdateQueue[i].Apply(false, false);
			}
			bool flag = count > 0;
			if (flag)
			{
				FontAsset.k_FontAssets_AtlasTexturesUpdateQueue.Clear();
				FontAsset.k_FontAssets_AtlasTexturesUpdateQueueLookup.Clear();
			}
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEngine.IMGUIModule",
			"UnityEngine.UIElementsModule"
		})]
		internal static void UpdateFontAssetsInUpdateQueue()
		{
			FontAsset.UpdateAtlasTexturesInQueue();
			FontAsset.UpdateFontFeaturesForFontAssetsInQueue();
		}

		public bool TryAddCharacters(uint[] unicodes, bool includeFontFeatures = false)
		{
			uint[] array;
			return this.TryAddCharacters(unicodes, out array, includeFontFeatures);
		}

		public bool TryAddCharacters(uint[] unicodes, out uint[] missingUnicodes, bool includeFontFeatures = false)
		{
			bool result;
			using (FontAsset.k_TryAddCharactersMarker.Auto())
			{
				bool flag = unicodes == null || unicodes.Length == 0 || this.m_AtlasPopulationMode == AtlasPopulationMode.Static;
				if (flag)
				{
					bool flag2 = this.m_AtlasPopulationMode == AtlasPopulationMode.Static;
					if (flag2)
					{
						Debug.LogWarning("Unable to add characters to font asset [" + base.name + "] because its AtlasPopulationMode is set to Static.", this);
					}
					else
					{
						Debug.LogWarning("Unable to add characters to font asset [" + base.name + "] because the provided Unicode list is Null or Empty.", this);
					}
					missingUnicodes = null;
					result = false;
				}
				else
				{
					bool flag3 = this.LoadFontFace() > FontEngineError.Success;
					if (flag3)
					{
						missingUnicodes = new uint[unicodes.Length];
						int num = 0;
						foreach (uint num2 in unicodes)
						{
							missingUnicodes[num++] = num2;
						}
						result = false;
					}
					else
					{
						bool flag4 = this.m_CharacterLookupDictionary == null || this.m_GlyphLookupDictionary == null;
						if (flag4)
						{
							this.ReadFontAssetDefinition();
						}
						Dictionary<uint, Character> characterLookupDictionary = this.m_CharacterLookupDictionary;
						Dictionary<uint, Glyph> glyphLookupDictionary = this.m_GlyphLookupDictionary;
						this.m_GlyphsToAdd.Clear();
						this.m_GlyphsToAddLookup.Clear();
						this.m_CharactersToAdd.Clear();
						this.m_CharactersToAddLookup.Clear();
						this.s_MissingCharacterList.Clear();
						bool flag5 = false;
						int num3 = unicodes.Length;
						for (int j = 0; j < num3; j++)
						{
							uint codePoint = FontAssetUtilities.GetCodePoint(unicodes, ref j);
							bool flag6 = characterLookupDictionary.ContainsKey(codePoint);
							if (!flag6)
							{
								uint glyphIndex = FontEngine.GetGlyphIndex(codePoint);
								bool flag7 = glyphIndex == 0U;
								if (flag7)
								{
									uint num4 = codePoint;
									uint num5 = num4;
									if (num5 != 160U)
									{
										if (num5 == 173U || num5 == 8209U)
										{
											glyphIndex = FontEngine.GetGlyphIndex(45U);
										}
									}
									else
									{
										glyphIndex = FontEngine.GetGlyphIndex(32U);
									}
									bool flag8 = glyphIndex == 0U;
									if (flag8)
									{
										this.s_MissingCharacterList.Add(codePoint);
										flag5 = true;
										goto IL_266;
									}
								}
								Character character = new Character(codePoint, glyphIndex);
								Glyph glyph;
								bool flag9 = glyphLookupDictionary.TryGetValue(glyphIndex, out glyph);
								if (flag9)
								{
									character.glyph = glyph;
									character.textAsset = this;
									this.m_CharacterTable.Add(character);
									characterLookupDictionary.Add(codePoint, character);
								}
								else
								{
									bool flag10 = this.m_GlyphsToAddLookup.Add(glyphIndex);
									if (flag10)
									{
										this.m_GlyphsToAdd.Add(glyphIndex);
									}
									bool flag11 = this.m_CharactersToAddLookup.Add(codePoint);
									if (flag11)
									{
										this.m_CharactersToAdd.Add(character);
									}
								}
							}
							IL_266:;
						}
						bool flag12 = this.m_GlyphsToAdd.Count == 0;
						if (flag12)
						{
							missingUnicodes = unicodes;
							result = !flag5;
						}
						else
						{
							bool flag13 = this.m_AtlasTextures[this.m_AtlasTextureIndex].width <= 1 || this.m_AtlasTextures[this.m_AtlasTextureIndex].height <= 1;
							if (flag13)
							{
								this.m_AtlasTextures[this.m_AtlasTextureIndex].Reinitialize(this.m_AtlasWidth, this.m_AtlasHeight);
								FontEngine.ResetAtlasTexture(this.m_AtlasTextures[this.m_AtlasTextureIndex]);
							}
							Glyph[] array;
							bool flag14 = FontEngine.TryAddGlyphsToTexture(this.m_GlyphsToAdd, this.m_AtlasPadding, GlyphPackingMode.BestShortSideFit, this.m_FreeGlyphRects, this.m_UsedGlyphRects, this.m_AtlasRenderMode, this.m_AtlasTextures[this.m_AtlasTextureIndex], out array);
							int additionalCapacity = array.Length;
							FontAsset.EnsureAdditionalCapacity<Glyph>(this.m_GlyphTable, additionalCapacity);
							FontAsset.EnsureAdditionalCapacity<uint, Glyph>(glyphLookupDictionary, additionalCapacity);
							FontAsset.EnsureAdditionalCapacity<uint>(this.m_GlyphIndexListNewlyAdded, additionalCapacity);
							FontAsset.EnsureAdditionalCapacity<uint>(this.m_GlyphIndexList, additionalCapacity);
							int num6 = 0;
							while (num6 < array.Length && array[num6] != null)
							{
								Glyph glyph2 = array[num6];
								uint index = glyph2.index;
								glyph2.atlasIndex = this.m_AtlasTextureIndex;
								this.m_GlyphTable.Add(glyph2);
								glyphLookupDictionary.Add(index, glyph2);
								this.m_GlyphIndexListNewlyAdded.Add(index);
								this.m_GlyphIndexList.Add(index);
								num6++;
							}
							this.m_GlyphsToAdd.Clear();
							int count = this.m_CharactersToAdd.Count;
							FontAsset.EnsureAdditionalCapacity<uint>(this.m_GlyphsToAdd, count);
							FontAsset.EnsureAdditionalCapacity<Character>(this.m_CharacterTable, count);
							FontAsset.EnsureAdditionalCapacity<uint, Character>(characterLookupDictionary, count);
							for (int k = 0; k < this.m_CharactersToAdd.Count; k++)
							{
								Character character2 = this.m_CharactersToAdd[k];
								Glyph glyph3;
								bool flag15 = !glyphLookupDictionary.TryGetValue(character2.glyphIndex, out glyph3);
								if (flag15)
								{
									this.m_GlyphsToAdd.Add(character2.glyphIndex);
								}
								else
								{
									character2.glyph = glyph3;
									character2.textAsset = this;
									this.m_CharacterTable.Add(character2);
									characterLookupDictionary.Add(character2.unicode, character2);
									this.m_CharactersToAdd.RemoveAt(k);
									k--;
								}
							}
							bool flag16 = this.m_IsMultiAtlasTexturesEnabled && !flag14;
							if (flag16)
							{
								while (!flag14)
								{
									flag14 = this.TryAddGlyphsToNewAtlasTexture();
								}
							}
							else
							{
								bool flag17 = !flag14;
								if (flag17)
								{
									Debug.Log("Atlas is full, consider enabling multi-atlas textures in the Font Asset: " + base.name);
								}
							}
							if (includeFontFeatures)
							{
								this.UpdateFontFeaturesForNewlyAddedGlyphs();
							}
							foreach (Character character3 in this.m_CharactersToAdd)
							{
								this.s_MissingCharacterList.Add(character3.unicode);
							}
							missingUnicodes = null;
							bool flag18 = this.s_MissingCharacterList.Count > 0;
							if (flag18)
							{
								missingUnicodes = this.s_MissingCharacterList.ToArray();
							}
							result = (flag14 && !flag5);
						}
					}
				}
			}
			return result;
		}

		public bool TryAddCharacters(string characters, bool includeFontFeatures = false)
		{
			string text;
			return this.TryAddCharacters(characters, out text, includeFontFeatures);
		}

		public bool TryAddCharacters(string characters, out string missingCharacters, bool includeFontFeatures = false)
		{
			uint[] array = new uint[characters.Length];
			for (int i = 0; i < characters.Length; i++)
			{
				array[i] = (uint)characters[i];
			}
			uint[] array2;
			bool flag = this.TryAddCharacters(array, out array2, includeFontFeatures);
			bool flag2 = array2 == null || array2.Length == 0;
			bool result;
			if (flag2)
			{
				missingCharacters = null;
				result = flag;
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder(array2.Length);
				foreach (uint num in array2)
				{
					stringBuilder.Append((char)num);
				}
				missingCharacters = stringBuilder.ToString();
				result = flag;
			}
			return result;
		}

		internal bool TryAddGlyphVariantIndexInternal(uint unicode, uint nextCharacter, uint variantGlyphIndex)
		{
			return this.m_VariantGlyphIndexes.TryAdd(new ValueTuple<uint, uint>(unicode, nextCharacter), variantGlyphIndex);
		}

		internal bool TryGetGlyphVariantIndexInternal(uint unicode, uint nextCharacter, out uint variantGlyphIndex)
		{
			return this.m_VariantGlyphIndexes.TryGetValue(new ValueTuple<uint, uint>(unicode, nextCharacter), out variantGlyphIndex);
		}

		internal bool TryAddGlyphInternal(uint glyphIndex, out Glyph glyph)
		{
			bool result;
			using (FontAsset.k_TryAddGlyphMarker.Auto())
			{
				glyph = null;
				bool flag = this.glyphLookupTable.TryGetValue(glyphIndex, out glyph);
				if (flag)
				{
					result = true;
				}
				else
				{
					bool flag2 = this.LoadFontFace() > FontEngineError.Success;
					if (flag2)
					{
						result = false;
					}
					else
					{
						result = this.TryAddGlyphToAtlas(glyphIndex, out glyph, true);
					}
				}
			}
			return result;
		}

		internal bool TryAddCharacterInternal(uint unicode, out Character character)
		{
			return this.TryAddCharacterInternal(unicode, FontStyles.Normal, TextFontWeight.Regular, out character, true);
		}

		internal bool TryAddCharacterInternal(uint unicode, FontStyles fontStyle, TextFontWeight fontWeight, out Character character, bool populateLigatures = true)
		{
			bool result;
			using (FontAsset.k_TryAddCharacterMarker.Auto())
			{
				character = null;
				bool flag = this.m_MissingUnicodesFromFontFile.Contains(unicode);
				if (flag)
				{
					result = false;
				}
				else
				{
					bool flag2 = this.LoadFontFace() > FontEngineError.Success;
					if (flag2)
					{
						result = false;
					}
					else
					{
						uint glyphIndex = FontEngine.GetGlyphIndex(unicode);
						bool flag3 = glyphIndex == 0U;
						if (flag3)
						{
							if (unicode != 160U)
							{
								if (unicode == 173U || unicode == 8209U)
								{
									glyphIndex = FontEngine.GetGlyphIndex(45U);
								}
							}
							else
							{
								glyphIndex = FontEngine.GetGlyphIndex(32U);
							}
							bool flag4 = glyphIndex == 0U;
							if (flag4)
							{
								this.m_MissingUnicodesFromFontFile.Add(unicode);
								return false;
							}
						}
						bool flag5 = this.glyphLookupTable.ContainsKey(glyphIndex);
						if (flag5)
						{
							character = this.CreateCharacterAndAddToCache(unicode, this.m_GlyphLookupDictionary[glyphIndex], fontStyle, fontWeight);
							result = true;
						}
						else
						{
							Glyph glyph = null;
							bool flag6 = this.TryAddGlyphToAtlas(glyphIndex, out glyph, populateLigatures);
							if (flag6)
							{
								character = this.CreateCharacterAndAddToCache(unicode, glyph, fontStyle, fontWeight);
								result = true;
							}
							else
							{
								result = false;
							}
						}
					}
				}
			}
			return result;
		}

		private bool TryAddGlyphToAtlas(uint glyphIndex, out Glyph glyph, bool populateLigatures = true)
		{
			glyph = null;
			bool flag = !this.m_AtlasTextures[this.m_AtlasTextureIndex].isReadable;
			bool result;
			if (flag)
			{
				Debug.LogWarning(string.Concat(new string[]
				{
					"Unable to add the requested glyph to font asset [",
					base.name,
					"]'s atlas texture. Please make the texture [",
					this.m_AtlasTextures[this.m_AtlasTextureIndex].name,
					"] readable."
				}), this.m_AtlasTextures[this.m_AtlasTextureIndex]);
				result = false;
			}
			else
			{
				bool flag2 = this.m_AtlasTextures[this.m_AtlasTextureIndex].width <= 1 || this.m_AtlasTextures[this.m_AtlasTextureIndex].height <= 1;
				if (flag2)
				{
					this.m_AtlasTextures[this.m_AtlasTextureIndex].Reinitialize(this.m_AtlasWidth, this.m_AtlasHeight);
					FontEngine.ResetAtlasTexture(this.m_AtlasTextures[this.m_AtlasTextureIndex]);
				}
				FontEngine.SetTextureUploadMode(false);
				bool flag3 = this.TryAddGlyphToTexture(glyphIndex, out glyph, populateLigatures);
				if (flag3)
				{
					result = true;
				}
				else
				{
					bool flag4 = this.m_IsMultiAtlasTexturesEnabled && this.m_UsedGlyphRects.Count > 0;
					if (flag4)
					{
						this.SetupNewAtlasTexture();
						FontEngine.SetTextureUploadMode(false);
						bool flag5 = this.TryAddGlyphToTexture(glyphIndex, out glyph, populateLigatures);
						if (flag5)
						{
							return true;
						}
					}
					else
					{
						bool flag6 = this.m_UsedGlyphRects.Count > 0;
						if (flag6)
						{
							Debug.Log("Atlas is full, consider enabling multi-atlas textures in the Font Asset: " + base.name);
						}
					}
					result = false;
				}
			}
			return result;
		}

		private bool TryAddGlyphToTexture(uint glyphIndex, out Glyph glyph, bool populateLigatures = true)
		{
			bool flag = FontEngine.TryAddGlyphToTexture(glyphIndex, this.m_AtlasPadding, GlyphPackingMode.BestShortSideFit, this.m_FreeGlyphRects, this.m_UsedGlyphRects, this.m_AtlasRenderMode, this.m_AtlasTextures[this.m_AtlasTextureIndex], out glyph);
			bool result;
			if (flag)
			{
				glyph.atlasIndex = this.m_AtlasTextureIndex;
				this.m_GlyphTable.Add(glyph);
				this.m_GlyphLookupDictionary.Add(glyphIndex, glyph);
				this.m_GlyphIndexList.Add(glyphIndex);
				this.m_GlyphIndexListNewlyAdded.Add(glyphIndex);
				bool getFontFeatures = this.m_GetFontFeatures;
				if (getFontFeatures)
				{
					if (populateLigatures)
					{
						this.UpdateGSUBFontFeaturesForNewGlyphIndex(glyphIndex);
						FontAsset.RegisterFontAssetForFontFeatureUpdate(this);
					}
					else
					{
						FontAsset.RegisterFontAssetForKerningUpdate(this);
					}
				}
				FontAsset.RegisterAtlasTextureForApply(this.m_AtlasTextures[this.m_AtlasTextureIndex]);
				FontEngine.SetTextureUploadMode(true);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private bool TryAddGlyphsToNewAtlasTexture()
		{
			this.SetupNewAtlasTexture();
			Glyph[] array;
			bool result = FontEngine.TryAddGlyphsToTexture(this.m_GlyphsToAdd, this.m_AtlasPadding, GlyphPackingMode.BestShortSideFit, this.m_FreeGlyphRects, this.m_UsedGlyphRects, this.m_AtlasRenderMode, this.m_AtlasTextures[this.m_AtlasTextureIndex], out array);
			int num = 0;
			while (num < array.Length && array[num] != null)
			{
				Glyph glyph = array[num];
				uint index = glyph.index;
				glyph.atlasIndex = this.m_AtlasTextureIndex;
				this.m_GlyphTable.Add(glyph);
				this.m_GlyphLookupDictionary.Add(index, glyph);
				this.m_GlyphIndexListNewlyAdded.Add(index);
				this.m_GlyphIndexList.Add(index);
				num++;
			}
			this.m_GlyphsToAdd.Clear();
			for (int i = 0; i < this.m_CharactersToAdd.Count; i++)
			{
				Character character = this.m_CharactersToAdd[i];
				Glyph glyph2;
				bool flag = !this.m_GlyphLookupDictionary.TryGetValue(character.glyphIndex, out glyph2);
				if (flag)
				{
					this.m_GlyphsToAdd.Add(character.glyphIndex);
				}
				else
				{
					character.glyph = glyph2;
					character.textAsset = this;
					this.m_CharacterTable.Add(character);
					this.m_CharacterLookupDictionary.Add(character.unicode, character);
					this.m_CharactersToAdd.RemoveAt(i);
					i--;
				}
			}
			return result;
		}

		private void SetupNewAtlasTexture()
		{
			this.m_AtlasTextureIndex++;
			bool flag = this.m_AtlasTextures.Length == this.m_AtlasTextureIndex;
			if (flag)
			{
				Array.Resize<Texture2D>(ref this.m_AtlasTextures, this.m_AtlasTextures.Length * 2);
			}
			TextureFormat textureFormat = ((this.m_AtlasRenderMode & (GlyphRenderMode)65536) == (GlyphRenderMode)65536) ? TextureFormat.RGBA32 : TextureFormat.Alpha8;
			this.m_AtlasTextures[this.m_AtlasTextureIndex] = new Texture2D(this.m_AtlasWidth, this.m_AtlasHeight, textureFormat, false);
			this.m_AtlasTextures[this.m_AtlasTextureIndex].hideFlags = this.m_AtlasTextures[0].hideFlags;
			FontEngine.ResetAtlasTexture(this.m_AtlasTextures[this.m_AtlasTextureIndex]);
			int num = ((this.m_AtlasRenderMode & (GlyphRenderMode)16) == (GlyphRenderMode)16) ? 0 : 1;
			this.m_FreeGlyphRects.Clear();
			this.m_FreeGlyphRects.Add(new GlyphRect(0, 0, this.m_AtlasWidth - num, this.m_AtlasHeight - num));
			this.m_UsedGlyphRects.Clear();
		}

		private Character CreateCharacterAndAddToCache(uint unicode, Glyph glyph, FontStyles fontStyle, TextFontWeight fontWeight)
		{
			Character character;
			bool flag = !this.m_CharacterLookupDictionary.TryGetValue(unicode, out character);
			if (flag)
			{
				character = new Character(unicode, this, glyph);
				this.m_CharacterTable.Add(character);
				this.AddCharacterToLookupCache(unicode, character, FontStyles.Normal, TextFontWeight.Regular);
			}
			bool flag2 = fontStyle != FontStyles.Normal || fontWeight != TextFontWeight.Regular;
			if (flag2)
			{
				this.AddCharacterToLookupCache(unicode, character, fontStyle, fontWeight);
			}
			return character;
		}

		private void UpdateFontFeaturesForNewlyAddedGlyphs()
		{
			this.UpdateLigatureSubstitutionRecords();
			this.UpdateGlyphAdjustmentRecords();
			this.UpdateDiacriticalMarkAdjustmentRecords();
			this.m_GlyphIndexListNewlyAdded.Clear();
		}

		private void UpdateGlyphAdjustmentRecordsForNewGlyphs()
		{
			this.UpdateGlyphAdjustmentRecords();
			this.m_GlyphIndexListNewlyAdded.Clear();
		}

		private void UpdateGPOSFontFeaturesForNewlyAddedGlyphs()
		{
			this.UpdateGlyphAdjustmentRecords();
			this.UpdateDiacriticalMarkAdjustmentRecords();
			this.m_GlyphIndexListNewlyAdded.Clear();
		}

		internal void ImportFontFeatures()
		{
			bool flag = this.LoadFontFace() > FontEngineError.Success;
			if (!flag)
			{
				GlyphPairAdjustmentRecord[] allPairAdjustmentRecords = FontEngine.GetAllPairAdjustmentRecords();
				bool flag2 = allPairAdjustmentRecords != null;
				if (flag2)
				{
					this.AddPairAdjustmentRecords(allPairAdjustmentRecords);
				}
				MarkToBaseAdjustmentRecord[] allMarkToBaseAdjustmentRecords = FontEngine.GetAllMarkToBaseAdjustmentRecords();
				bool flag3 = allMarkToBaseAdjustmentRecords != null;
				if (flag3)
				{
					this.AddMarkToBaseAdjustmentRecords(allMarkToBaseAdjustmentRecords);
				}
				MarkToMarkAdjustmentRecord[] allMarkToMarkAdjustmentRecords = FontEngine.GetAllMarkToMarkAdjustmentRecords();
				bool flag4 = allMarkToMarkAdjustmentRecords != null;
				if (flag4)
				{
					this.AddMarkToMarkAdjustmentRecords(allMarkToMarkAdjustmentRecords);
				}
				LigatureSubstitutionRecord[] allLigatureSubstitutionRecords = FontEngine.GetAllLigatureSubstitutionRecords();
				bool flag5 = allLigatureSubstitutionRecords != null;
				if (flag5)
				{
					this.AddLigatureSubstitutionRecords(allLigatureSubstitutionRecords);
				}
				this.m_ShouldReimportFontFeatures = false;
			}
		}

		private void UpdateGSUBFontFeaturesForNewGlyphIndex(uint glyphIndex)
		{
			LigatureSubstitutionRecord[] ligatureSubstitutionRecords = FontEngine.GetLigatureSubstitutionRecords(glyphIndex);
			bool flag = ligatureSubstitutionRecords != null;
			if (flag)
			{
				this.AddLigatureSubstitutionRecords(ligatureSubstitutionRecords);
			}
		}

		internal void UpdateLigatureSubstitutionRecords()
		{
			LigatureSubstitutionRecord[] ligatureSubstitutionRecords = FontEngine.GetLigatureSubstitutionRecords(this.m_GlyphIndexListNewlyAdded);
			bool flag = ligatureSubstitutionRecords != null;
			if (flag)
			{
				this.AddLigatureSubstitutionRecords(ligatureSubstitutionRecords);
			}
		}

		private void AddLigatureSubstitutionRecords(LigatureSubstitutionRecord[] records)
		{
			Dictionary<uint, List<LigatureSubstitutionRecord>> ligatureSubstitutionRecordLookup = this.m_FontFeatureTable.m_LigatureSubstitutionRecordLookup;
			List<LigatureSubstitutionRecord> ligatureSubstitutionRecords = this.m_FontFeatureTable.m_LigatureSubstitutionRecords;
			FontAsset.EnsureAdditionalCapacity<uint, List<LigatureSubstitutionRecord>>(ligatureSubstitutionRecordLookup, records.Length);
			FontAsset.EnsureAdditionalCapacity<LigatureSubstitutionRecord>(ligatureSubstitutionRecords, records.Length);
			foreach (LigatureSubstitutionRecord ligatureSubstitutionRecord in records)
			{
				bool flag = ligatureSubstitutionRecord.componentGlyphIDs == null || ligatureSubstitutionRecord.ligatureGlyphID == 0U;
				if (flag)
				{
					break;
				}
				uint key = ligatureSubstitutionRecord.componentGlyphIDs[0];
				LigatureSubstitutionRecord ligatureSubstitutionRecord2 = new LigatureSubstitutionRecord
				{
					componentGlyphIDs = ligatureSubstitutionRecord.componentGlyphIDs,
					ligatureGlyphID = ligatureSubstitutionRecord.ligatureGlyphID
				};
				List<LigatureSubstitutionRecord> list;
				bool flag2 = ligatureSubstitutionRecordLookup.TryGetValue(key, out list);
				if (flag2)
				{
					foreach (LigatureSubstitutionRecord rhs in list)
					{
						bool flag3 = ligatureSubstitutionRecord2 == rhs;
						if (flag3)
						{
							return;
						}
					}
					ligatureSubstitutionRecordLookup[key].Add(ligatureSubstitutionRecord2);
				}
				else
				{
					ligatureSubstitutionRecordLookup.Add(key, new List<LigatureSubstitutionRecord>
					{
						ligatureSubstitutionRecord2
					});
				}
				ligatureSubstitutionRecords.Add(ligatureSubstitutionRecord2);
			}
		}

		internal void UpdateGlyphAdjustmentRecords()
		{
			GlyphPairAdjustmentRecord[] pairAdjustmentRecords = FontEngine.GetPairAdjustmentRecords(this.m_GlyphIndexListNewlyAdded);
			bool flag = pairAdjustmentRecords != null;
			if (flag)
			{
				this.AddPairAdjustmentRecords(pairAdjustmentRecords);
			}
		}

		private void AddPairAdjustmentRecords(GlyphPairAdjustmentRecord[] records)
		{
			float num = this.m_FaceInfo.pointSize / (float)this.m_FaceInfo.unitsPerEM;
			List<GlyphPairAdjustmentRecord> glyphPairAdjustmentRecords = this.m_FontFeatureTable.glyphPairAdjustmentRecords;
			Dictionary<uint, GlyphPairAdjustmentRecord> glyphPairAdjustmentRecordLookup = this.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup;
			FontAsset.EnsureAdditionalCapacity<uint, GlyphPairAdjustmentRecord>(glyphPairAdjustmentRecordLookup, records.Length);
			FontAsset.EnsureAdditionalCapacity<GlyphPairAdjustmentRecord>(glyphPairAdjustmentRecords, records.Length);
			foreach (GlyphPairAdjustmentRecord glyphPairAdjustmentRecord in records)
			{
				GlyphAdjustmentRecord firstAdjustmentRecord = glyphPairAdjustmentRecord.firstAdjustmentRecord;
				GlyphAdjustmentRecord secondAdjustmentRecord = glyphPairAdjustmentRecord.secondAdjustmentRecord;
				uint glyphIndex = firstAdjustmentRecord.glyphIndex;
				uint glyphIndex2 = secondAdjustmentRecord.glyphIndex;
				bool flag = glyphIndex == 0U && glyphIndex2 == 0U;
				if (flag)
				{
					break;
				}
				uint key = glyphIndex2 << 16 | glyphIndex;
				GlyphPairAdjustmentRecord glyphPairAdjustmentRecord2 = glyphPairAdjustmentRecord;
				GlyphValueRecord glyphValueRecord = firstAdjustmentRecord.glyphValueRecord;
				glyphValueRecord.xAdvance *= num;
				glyphPairAdjustmentRecord2.firstAdjustmentRecord = new GlyphAdjustmentRecord(glyphIndex, glyphValueRecord);
				bool flag2 = glyphPairAdjustmentRecordLookup.TryAdd(key, glyphPairAdjustmentRecord2);
				if (flag2)
				{
					glyphPairAdjustmentRecords.Add(glyphPairAdjustmentRecord2);
				}
			}
		}

		internal void UpdateDiacriticalMarkAdjustmentRecords()
		{
			using (FontAsset.k_UpdateDiacriticalMarkAdjustmentRecordsMarker.Auto())
			{
				MarkToBaseAdjustmentRecord[] markToBaseAdjustmentRecords = FontEngine.GetMarkToBaseAdjustmentRecords(this.m_GlyphIndexListNewlyAdded);
				bool flag = markToBaseAdjustmentRecords != null;
				if (flag)
				{
					this.AddMarkToBaseAdjustmentRecords(markToBaseAdjustmentRecords);
				}
				MarkToMarkAdjustmentRecord[] markToMarkAdjustmentRecords = FontEngine.GetMarkToMarkAdjustmentRecords(this.m_GlyphIndexListNewlyAdded);
				bool flag2 = markToMarkAdjustmentRecords != null;
				if (flag2)
				{
					this.AddMarkToMarkAdjustmentRecords(markToMarkAdjustmentRecords);
				}
			}
		}

		private void AddMarkToBaseAdjustmentRecords(MarkToBaseAdjustmentRecord[] records)
		{
			float num = this.m_FaceInfo.pointSize / (float)this.m_FaceInfo.unitsPerEM;
			foreach (MarkToBaseAdjustmentRecord markToBaseAdjustmentRecord in records)
			{
				bool flag = markToBaseAdjustmentRecord.baseGlyphID == 0U || markToBaseAdjustmentRecord.markGlyphID == 0U;
				if (flag)
				{
					break;
				}
				uint key = markToBaseAdjustmentRecord.markGlyphID << 16 | markToBaseAdjustmentRecord.baseGlyphID;
				bool flag2 = this.m_FontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.ContainsKey(key);
				if (!flag2)
				{
					MarkToBaseAdjustmentRecord markToBaseAdjustmentRecord2 = new MarkToBaseAdjustmentRecord
					{
						baseGlyphID = markToBaseAdjustmentRecord.baseGlyphID,
						baseGlyphAnchorPoint = new GlyphAnchorPoint
						{
							xCoordinate = markToBaseAdjustmentRecord.baseGlyphAnchorPoint.xCoordinate * num,
							yCoordinate = markToBaseAdjustmentRecord.baseGlyphAnchorPoint.yCoordinate * num
						},
						markGlyphID = markToBaseAdjustmentRecord.markGlyphID,
						markPositionAdjustment = new MarkPositionAdjustment
						{
							xPositionAdjustment = markToBaseAdjustmentRecord.markPositionAdjustment.xPositionAdjustment * num,
							yPositionAdjustment = markToBaseAdjustmentRecord.markPositionAdjustment.yPositionAdjustment * num
						}
					};
					this.m_FontFeatureTable.MarkToBaseAdjustmentRecords.Add(markToBaseAdjustmentRecord2);
					this.m_FontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.Add(key, markToBaseAdjustmentRecord2);
				}
			}
		}

		private void AddMarkToMarkAdjustmentRecords(MarkToMarkAdjustmentRecord[] records)
		{
			float num = this.m_FaceInfo.pointSize / (float)this.m_FaceInfo.unitsPerEM;
			for (int i = 0; i < records.Length; i++)
			{
				MarkToMarkAdjustmentRecord markToMarkAdjustmentRecord = records[i];
				bool flag = records[i].baseMarkGlyphID == 0U || records[i].combiningMarkGlyphID == 0U;
				if (flag)
				{
					break;
				}
				uint key = markToMarkAdjustmentRecord.combiningMarkGlyphID << 16 | markToMarkAdjustmentRecord.baseMarkGlyphID;
				bool flag2 = this.m_FontFeatureTable.m_MarkToMarkAdjustmentRecordLookup.ContainsKey(key);
				if (!flag2)
				{
					MarkToMarkAdjustmentRecord markToMarkAdjustmentRecord2 = new MarkToMarkAdjustmentRecord
					{
						baseMarkGlyphID = markToMarkAdjustmentRecord.baseMarkGlyphID,
						baseMarkGlyphAnchorPoint = new GlyphAnchorPoint
						{
							xCoordinate = markToMarkAdjustmentRecord.baseMarkGlyphAnchorPoint.xCoordinate * num,
							yCoordinate = markToMarkAdjustmentRecord.baseMarkGlyphAnchorPoint.yCoordinate * num
						},
						combiningMarkGlyphID = markToMarkAdjustmentRecord.combiningMarkGlyphID,
						combiningMarkPositionAdjustment = new MarkPositionAdjustment
						{
							xPositionAdjustment = markToMarkAdjustmentRecord.combiningMarkPositionAdjustment.xPositionAdjustment * num,
							yPositionAdjustment = markToMarkAdjustmentRecord.combiningMarkPositionAdjustment.yPositionAdjustment * num
						}
					};
					this.m_FontFeatureTable.MarkToMarkAdjustmentRecords.Add(markToMarkAdjustmentRecord2);
					this.m_FontFeatureTable.m_MarkToMarkAdjustmentRecordLookup.Add(key, markToMarkAdjustmentRecord2);
				}
			}
		}

		internal IntPtr nativeFontAsset
		{
			[VisibleToOtherModules(new string[]
			{
				"UnityEngine.UIElementsModule"
			})]
			get
			{
				this.EnsureNativeFontAssetIsCreated();
				return this.m_NativeFontAsset;
			}
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEngine.UIElementsModule"
		})]
		internal void EnsureNativeFontAssetIsCreated()
		{
			bool flag = this.m_NativeFontAsset != IntPtr.Zero;
			if (!flag)
			{
				IntPtr[] fallbacks = this.GetFallbacks();
				ValueTuple<IntPtr[], IntPtr[]> weightFallbacks = this.GetWeightFallbacks();
				FontAsset.kFontAssetByInstanceId.TryAdd(base.instanceID, this);
				Font sourceFont_EditorRef = null;
				this.m_NativeFontAsset = FontAsset.Create(this.faceInfo, this.sourceFontFile, sourceFont_EditorRef, this.m_SourceFontFilePath, base.instanceID, fallbacks, weightFallbacks.Item1, weightFallbacks.Item2);
			}
		}

		internal void UpdateFallbacks()
		{
			FontAsset.UpdateFallbacks(this.nativeFontAsset, this.GetFallbacks());
		}

		internal void UpdateWeightFallbacks()
		{
			ValueTuple<IntPtr[], IntPtr[]> weightFallbacks = this.GetWeightFallbacks();
			FontAsset.UpdateWeightFallbacks(this.nativeFontAsset, weightFallbacks.Item1, weightFallbacks.Item2);
		}

		internal void UpdateFaceInfo()
		{
			FontAsset.UpdateFaceInfo(this.nativeFontAsset, this.faceInfo);
		}

		internal IntPtr[] GetFallbacks()
		{
			List<IntPtr> list = new List<IntPtr>();
			bool flag = this.fallbackFontAssetTable == null;
			IntPtr[] result;
			if (flag)
			{
				result = list.ToArray();
			}
			else
			{
				foreach (FontAsset fontAsset in this.fallbackFontAssetTable)
				{
					bool flag2 = fontAsset == null;
					if (!flag2)
					{
						bool flag3 = fontAsset.atlasPopulationMode == AtlasPopulationMode.Static && fontAsset.characterTable.Count > 0;
						if (flag3)
						{
							Debug.LogWarning("Advanced text system cannot use static font asset " + fontAsset.name + " as fallback.");
						}
						else
						{
							bool flag4 = this.HasRecursion(fontAsset);
							if (flag4)
							{
								Debug.LogWarning("Circular reference detected. Cannot add " + fontAsset.name + " to the fallbacks.");
							}
							else
							{
								list.Add(fontAsset.nativeFontAsset);
							}
						}
					}
				}
				result = list.ToArray();
			}
			return result;
		}

		private bool HasRecursion(FontAsset fontAsset)
		{
			FontAsset.visitedFontAssets.Clear();
			return this.HasRecursionInternal(fontAsset);
		}

		private bool HasRecursionInternal(FontAsset fontAsset)
		{
			bool flag = FontAsset.visitedFontAssets.Contains(fontAsset.instanceID);
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				FontAsset.visitedFontAssets.Add(fontAsset.instanceID);
				bool flag2 = fontAsset.fallbackFontAssetTable != null;
				if (flag2)
				{
					foreach (FontAsset fontAsset2 in fontAsset.fallbackFontAssetTable)
					{
						bool flag3 = this.HasRecursionInternal(fontAsset2);
						if (flag3)
						{
							return true;
						}
					}
				}
				for (int i = 0; i < fontAsset.fontWeightTable.Length; i++)
				{
					FontWeightPair fontWeightPair = fontAsset.fontWeightTable[i];
					bool flag4 = fontWeightPair.regularTypeface != null;
					if (flag4)
					{
						bool flag5 = this.HasRecursionInternal(fontWeightPair.regularTypeface);
						if (flag5)
						{
							return true;
						}
					}
					bool flag6 = fontWeightPair.italicTypeface != null;
					if (flag6)
					{
						bool flag7 = this.HasRecursionInternal(fontWeightPair.italicTypeface);
						if (flag7)
						{
							return true;
						}
					}
				}
				FontAsset.visitedFontAssets.Remove(fontAsset.instanceID);
				result = false;
			}
			return result;
		}

		private ValueTuple<IntPtr[], IntPtr[]> GetWeightFallbacks()
		{
			IntPtr[] array = new IntPtr[10];
			IntPtr[] array2 = new IntPtr[10];
			int i = 0;
			while (i < this.fontWeightTable.Length)
			{
				FontWeightPair fontWeightPair = this.fontWeightTable[i];
				bool flag = fontWeightPair.regularTypeface != null;
				if (!flag)
				{
					goto IL_D2;
				}
				bool flag2 = fontWeightPair.regularTypeface.atlasPopulationMode == AtlasPopulationMode.Static && fontWeightPair.regularTypeface.characterTable.Count > 0;
				if (flag2)
				{
					Debug.LogWarning("Advanced text system cannot use static font asset " + fontWeightPair.regularTypeface.name + " as fallback.");
				}
				else
				{
					bool flag3 = this.HasRecursion(fontWeightPair.regularTypeface);
					if (!flag3)
					{
						array[i] = fontWeightPair.regularTypeface.nativeFontAsset;
						goto IL_D2;
					}
					Debug.LogWarning("Circular reference detected. Cannot add " + fontWeightPair.regularTypeface.name + " to the fallbacks.");
				}
				IL_179:
				i++;
				continue;
				IL_D2:
				bool flag4 = fontWeightPair.italicTypeface != null;
				if (flag4)
				{
					bool flag5 = fontWeightPair.italicTypeface.atlasPopulationMode == AtlasPopulationMode.Static && fontWeightPair.italicTypeface.characterTable.Count > 0;
					if (flag5)
					{
						Debug.LogWarning("Advanced text system cannot use static font asset " + fontWeightPair.italicTypeface.name + " as fallback.");
					}
					else
					{
						bool flag6 = this.HasRecursion(fontWeightPair.italicTypeface);
						if (flag6)
						{
							Debug.LogWarning("Circular reference detected. Cannot add " + fontWeightPair.italicTypeface.name + " to the fallbacks.");
						}
						else
						{
							array2[i] = fontWeightPair.italicTypeface.nativeFontAsset;
						}
					}
				}
				goto IL_179;
			}
			return new ValueTuple<IntPtr[], IntPtr[]>(array, array2);
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEngine.UIElementsModule"
		})]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void CreateHbFaceIfNeeded();

		private unsafe static void UpdateFallbacks(IntPtr ptr, IntPtr[] fallbacks)
		{
			Span<IntPtr> span = new Span<IntPtr>(fallbacks);
			fixed (IntPtr* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				FontAsset.UpdateFallbacks_Injected(ptr, ref managedSpanWrapper);
			}
		}

		private unsafe static void UpdateWeightFallbacks(IntPtr ptr, IntPtr[] regularFallbacks, IntPtr[] italicFallbacks)
		{
			Span<IntPtr> span = new Span<IntPtr>(regularFallbacks);
			fixed (IntPtr* ptr2 = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr2, span.Length);
				Span<IntPtr> span2 = new Span<IntPtr>(italicFallbacks);
				fixed (IntPtr* pinnableReference = span2.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, span2.Length);
					FontAsset.UpdateWeightFallbacks_Injected(ptr, ref managedSpanWrapper, ref managedSpanWrapper2);
					ptr2 = null;
				}
			}
		}

		private unsafe static IntPtr Create(FaceInfo faceInfo, Font sourceFontFile, Font sourceFont_EditorRef, string sourceFontFilePath, int fontInstanceID, IntPtr[] fallbacks, IntPtr[] weightFallbacks, IntPtr[] italicFallbacks)
		{
			IntPtr result;
			try
			{
				IntPtr sourceFontFile2 = Object.MarshalledUnityObject.Marshal<Font>(sourceFontFile);
				IntPtr sourceFont_EditorRef2 = Object.MarshalledUnityObject.Marshal<Font>(sourceFont_EditorRef);
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(sourceFontFilePath, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = sourceFontFilePath.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Span<IntPtr> span = new Span<IntPtr>(fallbacks);
				fixed (IntPtr* ptr2 = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, span.Length);
					Span<IntPtr> span2 = new Span<IntPtr>(weightFallbacks);
					fixed (IntPtr* ptr3 = span2.GetPinnableReference())
					{
						ManagedSpanWrapper managedSpanWrapper3 = new ManagedSpanWrapper((void*)ptr3, span2.Length);
						Span<IntPtr> span3 = new Span<IntPtr>(italicFallbacks);
						fixed (IntPtr* ptr4 = span3.GetPinnableReference())
						{
							ManagedSpanWrapper managedSpanWrapper4 = new ManagedSpanWrapper((void*)ptr4, span3.Length);
							result = FontAsset.Create_Injected(ref faceInfo, sourceFontFile2, sourceFont_EditorRef2, ref managedSpanWrapper, fontInstanceID, ref managedSpanWrapper2, ref managedSpanWrapper3, ref managedSpanWrapper4);
						}
					}
				}
			}
			finally
			{
				char* ptr = null;
				IntPtr* ptr2 = null;
				IntPtr* ptr3 = null;
				IntPtr* ptr4 = null;
			}
			return result;
		}

		private static void UpdateFaceInfo(IntPtr ptr, FaceInfo faceInfo)
		{
			FontAsset.UpdateFaceInfo_Injected(ptr, ref faceInfo);
		}

		[FreeFunction("FontAsset::Destroy")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Destroy(IntPtr ptr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateFallbacks_Injected(IntPtr ptr, ref ManagedSpanWrapper fallbacks);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateWeightFallbacks_Injected(IntPtr ptr, ref ManagedSpanWrapper regularFallbacks, ref ManagedSpanWrapper italicFallbacks);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create_Injected([In] ref FaceInfo faceInfo, IntPtr sourceFontFile, IntPtr sourceFont_EditorRef, ref ManagedSpanWrapper sourceFontFilePath, int fontInstanceID, ref ManagedSpanWrapper fallbacks, ref ManagedSpanWrapper weightFallbacks, ref ManagedSpanWrapper italicFallbacks);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateFaceInfo_Injected(IntPtr ptr, [In] ref FaceInfo faceInfo);

		private static Dictionary<int, FontAsset> kFontAssetByInstanceId = new Dictionary<int, FontAsset>();

		[SerializeField]
		internal string m_SourceFontFileGUID;

		[SerializeField]
		internal FontAssetCreationEditorSettings m_fontAssetCreationEditorSettings;

		[SerializeField]
		private Font m_SourceFontFile;

		[SerializeField]
		[VisibleToOtherModules(new string[]
		{
			"UnityEngine.UIElementsModule"
		})]
		internal string m_SourceFontFilePath;

		[SerializeField]
		private AtlasPopulationMode m_AtlasPopulationMode;

		[SerializeField]
		internal bool InternalDynamicOS;

		[SerializeField]
		[VisibleToOtherModules(new string[]
		{
			"UnityEngine.IMGUIModule",
			"UnityEngine.UIElementsModule"
		})]
		internal bool IsEditorFont = false;

		[SerializeField]
		internal FaceInfo m_FaceInfo;

		private int m_FamilyNameHashCode;

		private int m_StyleNameHashCode;

		[SerializeField]
		[Nullable(1)]
		internal List<Glyph> m_GlyphTable = new List<Glyph>();

		internal Dictionary<uint, Glyph> m_GlyphLookupDictionary;

		[SerializeField]
		internal List<Character> m_CharacterTable = new List<Character>();

		[VisibleToOtherModules(new string[]
		{
			"UnityEngine.IMGUIModule",
			"UnityEngine.UIElementsModule"
		})]
		internal Dictionary<uint, Character> m_CharacterLookupDictionary;

		internal Texture2D m_AtlasTexture;

		[SerializeField]
		internal Texture2D[] m_AtlasTextures;

		[SerializeField]
		internal int m_AtlasTextureIndex;

		[SerializeField]
		private bool m_IsMultiAtlasTexturesEnabled = true;

		[SerializeField]
		private bool m_GetFontFeatures = true;

		[SerializeField]
		private bool m_ClearDynamicDataOnBuild = true;

		[SerializeField]
		internal int m_AtlasWidth;

		[SerializeField]
		internal int m_AtlasHeight;

		[SerializeField]
		internal int m_AtlasPadding;

		[SerializeField]
		internal GlyphRenderMode m_AtlasRenderMode;

		[SerializeField]
		private List<GlyphRect> m_UsedGlyphRects;

		[SerializeField]
		private List<GlyphRect> m_FreeGlyphRects;

		[SerializeField]
		internal FontFeatureTable m_FontFeatureTable = new FontFeatureTable();

		[SerializeField]
		internal bool m_ShouldReimportFontFeatures;

		[SerializeField]
		internal List<FontAsset> m_FallbackFontAssetTable;

		[SerializeField]
		private FontWeightPair[] m_FontWeightTable = new FontWeightPair[10];

		[SerializeField]
		[FormerlySerializedAs("normalStyle")]
		internal float m_RegularStyleWeight = 0f;

		[FormerlySerializedAs("normalSpacingOffset")]
		[SerializeField]
		internal float m_RegularStyleSpacing = 0f;

		[FormerlySerializedAs("boldStyle")]
		[SerializeField]
		internal float m_BoldStyleWeight = 0.75f;

		[SerializeField]
		[FormerlySerializedAs("boldSpacing")]
		internal float m_BoldStyleSpacing = 7f;

		[SerializeField]
		[FormerlySerializedAs("italicStyle")]
		internal byte m_ItalicStyleSlant = 35;

		[SerializeField]
		[FormerlySerializedAs("tabSize")]
		internal byte m_TabMultiple = 10;

		internal bool IsFontAssetLookupTablesDirty;

		private IntPtr m_NativeFontAsset = IntPtr.Zero;

		private List<Glyph> m_GlyphsToRender = new List<Glyph>();

		private List<Glyph> m_GlyphsRendered = new List<Glyph>();

		private List<uint> m_GlyphIndexList = new List<uint>();

		private List<uint> m_GlyphIndexListNewlyAdded = new List<uint>();

		internal List<uint> m_GlyphsToAdd = new List<uint>();

		internal HashSet<uint> m_GlyphsToAddLookup = new HashSet<uint>();

		internal List<Character> m_CharactersToAdd = new List<Character>();

		internal HashSet<uint> m_CharactersToAddLookup = new HashSet<uint>();

		internal List<uint> s_MissingCharacterList = new List<uint>();

		internal HashSet<uint> m_MissingUnicodesFromFontFile = new HashSet<uint>();

		internal Dictionary<ValueTuple<uint, uint>, uint> m_VariantGlyphIndexes = new Dictionary<ValueTuple<uint, uint>, uint>();

		internal bool m_IsClone;

		private static readonly List<WeakReference<FontAsset>> s_CallbackInstances = new List<WeakReference<FontAsset>>();

		private static ProfilerMarker k_ReadFontAssetDefinitionMarker = new ProfilerMarker("FontAsset.ReadFontAssetDefinition");

		private static ProfilerMarker k_AddSynthesizedCharactersMarker = new ProfilerMarker("FontAsset.AddSynthesizedCharacters");

		private static ProfilerMarker k_TryAddGlyphMarker = new ProfilerMarker("FontAsset.TryAddGlyph");

		private static ProfilerMarker k_TryAddCharacterMarker = new ProfilerMarker("FontAsset.TryAddCharacter");

		private static ProfilerMarker k_TryAddCharactersMarker = new ProfilerMarker("FontAsset.TryAddCharacters");

		private static ProfilerMarker k_UpdateLigatureSubstitutionRecordsMarker = new ProfilerMarker("FontAsset.UpdateLigatureSubstitutionRecords");

		private static ProfilerMarker k_UpdateGlyphAdjustmentRecordsMarker = new ProfilerMarker("FontAsset.UpdateGlyphAdjustmentRecords");

		private static ProfilerMarker k_UpdateDiacriticalMarkAdjustmentRecordsMarker = new ProfilerMarker("FontAsset.UpdateDiacriticalAdjustmentRecords");

		private static ProfilerMarker k_ClearFontAssetDataMarker = new ProfilerMarker("FontAsset.ClearFontAssetData");

		private static ProfilerMarker k_UpdateFontAssetDataMarker = new ProfilerMarker("FontAsset.UpdateFontAssetData");

		private static string s_DefaultMaterialSuffix = " Atlas Material";

		private static HashSet<int> k_SearchedFontAssetLookup;

		private static List<FontAsset> k_FontAssets_FontFeaturesUpdateQueue = new List<FontAsset>();

		private static HashSet<int> k_FontAssets_FontFeaturesUpdateQueueLookup = new HashSet<int>();

		private static List<FontAsset> k_FontAssets_KerningUpdateQueue = new List<FontAsset>();

		private static HashSet<int> k_FontAssets_KerningUpdateQueueLookup = new HashSet<int>();

		private static List<Texture2D> k_FontAssets_AtlasTexturesUpdateQueue = new List<Texture2D>();

		private static HashSet<int> k_FontAssets_AtlasTexturesUpdateQueueLookup = new HashSet<int>();

		internal static uint[] k_GlyphIndexArray;

		private static HashSet<int> visitedFontAssets = new HashSet<int>();

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(FontAsset fontAsset)
			{
				return fontAsset.m_NativeFontAsset;
			}
		}
	}
}
