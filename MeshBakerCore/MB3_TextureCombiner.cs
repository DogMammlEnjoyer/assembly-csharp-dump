using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	[Serializable]
	public class MB3_TextureCombiner
	{
		public MB2_TextureBakeResults textureBakeResults
		{
			get
			{
				return this._textureBakeResults;
			}
			set
			{
				this._textureBakeResults = value;
			}
		}

		public int atlasPadding
		{
			get
			{
				return this._atlasPadding;
			}
			set
			{
				this._atlasPadding = value;
			}
		}

		public int maxAtlasSize
		{
			get
			{
				return this._maxAtlasSize;
			}
			set
			{
				this._maxAtlasSize = value;
			}
		}

		public virtual int maxAtlasWidthOverride
		{
			get
			{
				return this._maxAtlasWidthOverride;
			}
			set
			{
				this._maxAtlasWidthOverride = value;
			}
		}

		public virtual int maxAtlasHeightOverride
		{
			get
			{
				return this._maxAtlasHeightOverride;
			}
			set
			{
				this._maxAtlasHeightOverride = value;
			}
		}

		public virtual bool useMaxAtlasWidthOverride
		{
			get
			{
				return this._useMaxAtlasWidthOverride;
			}
			set
			{
				this._useMaxAtlasWidthOverride = value;
			}
		}

		public virtual bool useMaxAtlasHeightOverride
		{
			get
			{
				return this._useMaxAtlasHeightOverride;
			}
			set
			{
				this._useMaxAtlasHeightOverride = value;
			}
		}

		public bool resizePowerOfTwoTextures
		{
			get
			{
				return this._resizePowerOfTwoTextures;
			}
			set
			{
				this._resizePowerOfTwoTextures = value;
			}
		}

		public bool fixOutOfBoundsUVs
		{
			get
			{
				return this._fixOutOfBoundsUVs;
			}
			set
			{
				this._fixOutOfBoundsUVs = value;
			}
		}

		public int layerTexturePackerFastMesh
		{
			get
			{
				return this._layerTexturePackerFastMesh;
			}
			set
			{
				this._layerTexturePackerFastMesh = value;
			}
		}

		public int maxTilingBakeSize
		{
			get
			{
				return this._maxTilingBakeSize;
			}
			set
			{
				this._maxTilingBakeSize = value;
			}
		}

		public bool saveAtlasesAsAssets
		{
			get
			{
				return this._saveAtlasesAsAssets;
			}
			set
			{
				this._saveAtlasesAsAssets = value;
			}
		}

		public MB2_TextureBakeResults.ResultType resultType
		{
			get
			{
				return this._resultType;
			}
			set
			{
				this._resultType = value;
			}
		}

		public MB2_PackingAlgorithmEnum packingAlgorithm
		{
			get
			{
				return this._packingAlgorithm;
			}
			set
			{
				this._packingAlgorithm = value;
			}
		}

		public bool meshBakerTexturePackerForcePowerOfTwo
		{
			get
			{
				return this._meshBakerTexturePackerForcePowerOfTwo;
			}
			set
			{
				this._meshBakerTexturePackerForcePowerOfTwo = value;
			}
		}

		public List<ShaderTextureProperty> customShaderPropNames
		{
			get
			{
				return this._customShaderPropNames;
			}
			set
			{
				this._customShaderPropNames = value;
			}
		}

		public bool considerNonTextureProperties
		{
			get
			{
				return this._considerNonTextureProperties;
			}
			set
			{
				this._considerNonTextureProperties = value;
			}
		}

		public bool doMergeDistinctMaterialTexturesThatWouldExceedAtlasSize
		{
			get
			{
				return this._doMergeDistinctMaterialTexturesThatWouldExceedAtlasSize;
			}
			set
			{
				this._doMergeDistinctMaterialTexturesThatWouldExceedAtlasSize = value;
			}
		}

		public static void RunCorutineWithoutPause(IEnumerator cor, int recursionDepth)
		{
			if (recursionDepth == 0)
			{
				MB3_TextureCombiner._RunCorutineWithoutPauseIsRunning = true;
			}
			if (recursionDepth > 20)
			{
				Debug.LogError("Recursion Depth Exceeded.");
				return;
			}
			while (cor.MoveNext())
			{
				object obj = cor.Current;
				if (!(obj is YieldInstruction) && obj != null && obj is IEnumerator)
				{
					MB3_TextureCombiner.RunCorutineWithoutPause((IEnumerator)cor.Current, recursionDepth + 1);
				}
			}
			if (recursionDepth == 0)
			{
				MB3_TextureCombiner._RunCorutineWithoutPauseIsRunning = false;
			}
		}

		public bool CombineTexturesIntoAtlases(ProgressUpdateDelegate progressInfo, MB_AtlasesAndRects resultAtlasesAndRects, Material resultMaterial, List<GameObject> objsToMesh, List<Material> allowedMaterialsFilter, List<string> texPropsToIgnore, MB2_EditorMethodsInterface textureEditorMethods = null, List<AtlasPackingResult> packingResults = null, bool onlyPackRects = false, bool splitAtlasWhenPackingIfTooBig = false)
		{
			MB3_TextureCombiner.CombineTexturesIntoAtlasesCoroutineResult combineTexturesIntoAtlasesCoroutineResult = new MB3_TextureCombiner.CombineTexturesIntoAtlasesCoroutineResult();
			MB3_TextureCombiner.RunCorutineWithoutPause(this._CombineTexturesIntoAtlases(progressInfo, combineTexturesIntoAtlasesCoroutineResult, resultAtlasesAndRects, resultMaterial, objsToMesh, allowedMaterialsFilter, texPropsToIgnore, textureEditorMethods, packingResults, onlyPackRects, splitAtlasWhenPackingIfTooBig), 0);
			if (!combineTexturesIntoAtlasesCoroutineResult.success)
			{
				Debug.LogError("Failed to generate atlases.");
			}
			return combineTexturesIntoAtlasesCoroutineResult.success;
		}

		public IEnumerator CombineTexturesIntoAtlasesCoroutine(ProgressUpdateDelegate progressInfo, MB_AtlasesAndRects resultAtlasesAndRects, Material resultMaterial, List<GameObject> objsToMesh, List<Material> allowedMaterialsFilter, List<string> texPropsToIgnore, MB2_EditorMethodsInterface textureEditorMethods = null, MB3_TextureCombiner.CombineTexturesIntoAtlasesCoroutineResult coroutineResult = null, float maxTimePerFrame = 0.01f, List<AtlasPackingResult> packingResults = null, bool onlyPackRects = false, bool splitAtlasWhenPackingIfTooBig = false)
		{
			coroutineResult.success = true;
			coroutineResult.isFinished = false;
			if (maxTimePerFrame <= 0f)
			{
				Debug.LogError("maxTimePerFrame must be a value greater than zero");
				coroutineResult.isFinished = true;
				yield break;
			}
			yield return this._CombineTexturesIntoAtlases(progressInfo, coroutineResult, resultAtlasesAndRects, resultMaterial, objsToMesh, allowedMaterialsFilter, texPropsToIgnore, textureEditorMethods, packingResults, onlyPackRects, splitAtlasWhenPackingIfTooBig);
			coroutineResult.isFinished = true;
			yield break;
		}

		private IEnumerator _CombineTexturesIntoAtlases(ProgressUpdateDelegate progressInfo, MB3_TextureCombiner.CombineTexturesIntoAtlasesCoroutineResult result, MB_AtlasesAndRects resultAtlasesAndRects, Material resultMaterial, List<GameObject> objsToMesh, List<Material> allowedMaterialsFilter, List<string> texPropsToIgnore, MB2_EditorMethodsInterface textureEditorMethods, List<AtlasPackingResult> atlasPackingResult, bool onlyPackRects, bool splitAtlasWhenPackingIfTooBig)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			try
			{
				this._temporaryTextures.Clear();
				MeshBakerMaterialTexture.readyToBuildAtlases = false;
				if (textureEditorMethods != null)
				{
					textureEditorMethods.Clear();
					textureEditorMethods.OnPreTextureBake();
				}
				if (splitAtlasWhenPackingIfTooBig && !onlyPackRects)
				{
					Debug.LogError("Can only use 'splitAtlasWhenPackingIfTooLarge' with 'onlyPackRects'");
					result.success = false;
					yield break;
				}
				if (objsToMesh == null || objsToMesh.Count == 0)
				{
					Debug.LogError("No meshes to combine. Please assign some meshes to combine.");
					result.success = false;
					yield break;
				}
				if (this._atlasPadding < 0)
				{
					Debug.LogError("Atlas padding must be zero or greater.");
					result.success = false;
					yield break;
				}
				if (this._maxTilingBakeSize < 2 || this._maxTilingBakeSize > 8192)
				{
					Debug.LogError("Invalid value for max tiling bake size.");
					result.success = false;
					yield break;
				}
				for (int i = 0; i < objsToMesh.Count; i++)
				{
					Material[] gomaterials = MB_Utility.GetGOMaterials(objsToMesh[i]);
					for (int j = 0; j < gomaterials.Length; j++)
					{
						if (gomaterials[j] == null)
						{
							string str = "Game object ";
							GameObject gameObject = objsToMesh[i];
							Debug.LogError(str + ((gameObject != null) ? gameObject.ToString() : null) + " has a null material");
							result.success = false;
							yield break;
						}
					}
				}
				if (progressInfo != null)
				{
					progressInfo("Collecting textures for " + objsToMesh.Count.ToString() + " meshes.", 0.01f);
				}
				MB3_TextureCombinerPipeline.TexturePipelineData texturePipelineData = this.LoadPipelineData(resultMaterial, new List<ShaderTextureProperty>(), objsToMesh, allowedMaterialsFilter, texPropsToIgnore, new List<MB_TexSet>());
				if (!MB3_TextureCombinerPipeline._CollectPropertyNames(texturePipelineData, this.LOG_LEVEL))
				{
					result.success = false;
					yield break;
				}
				if (this._fixOutOfBoundsUVs && (this._packingAlgorithm == MB2_PackingAlgorithmEnum.MeshBakerTexturePacker_Horizontal || this._packingAlgorithm == MB2_PackingAlgorithmEnum.MeshBakerTexturePacker_Vertical) && this.LOG_LEVEL >= MB2_LogLevel.info)
				{
					Debug.LogWarning("'Consider Mesh UVs' is enabled but packing algorithm is MeshBakerTexturePacker_Horizontal or MeshBakerTexturePacker_Vertical. It is recommended to use these packers without using 'Consider Mesh UVs'");
				}
				texturePipelineData.nonTexturePropertyBlender.LoadTextureBlendersIfNeeded(texturePipelineData.resultMaterial);
				if (onlyPackRects)
				{
					yield return this.__RunTexturePackerOnly(result, resultAtlasesAndRects, texturePipelineData, splitAtlasWhenPackingIfTooBig, textureEditorMethods, atlasPackingResult);
				}
				else
				{
					yield return this.__CombineTexturesIntoAtlases(progressInfo, result, resultAtlasesAndRects, texturePipelineData, textureEditorMethods);
				}
			}
			finally
			{
				this._destroyAllTemporaryTextures();
				this._restoreProceduralMaterials();
				if (textureEditorMethods != null)
				{
					textureEditorMethods.RestoreReadFlagsAndFormats(progressInfo);
					textureEditorMethods.OnPostTextureBake();
				}
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					Debug.Log("===== Done creating atlases for " + ((resultMaterial != null) ? resultMaterial.ToString() : null) + " Total time to create atlases " + sw.Elapsed.ToString());
				}
			}
			yield break;
			yield break;
		}

		private MB3_TextureCombinerPipeline.TexturePipelineData LoadPipelineData(Material resultMaterial, List<ShaderTextureProperty> texPropertyNames, List<GameObject> objsToMesh, List<Material> allowedMaterialsFilter, List<string> texPropsToIgnore, List<MB_TexSet> distinctMaterialTextures)
		{
			MB3_TextureCombinerPipeline.TexturePipelineData texturePipelineData = new MB3_TextureCombinerPipeline.TexturePipelineData();
			texturePipelineData._textureBakeResults = this._textureBakeResults;
			texturePipelineData._atlasPadding_pix = this._atlasPadding;
			if (this._packingAlgorithm == MB2_PackingAlgorithmEnum.MeshBakerTexturePacker_Vertical && this._useMaxAtlasHeightOverride)
			{
				texturePipelineData._maxAtlasHeight = this._maxAtlasHeightOverride;
				texturePipelineData._useMaxAtlasHeightOverride = true;
			}
			else
			{
				texturePipelineData._maxAtlasHeight = this._maxAtlasSize;
			}
			if (this._packingAlgorithm == MB2_PackingAlgorithmEnum.MeshBakerTexturePacker_Horizontal && this._useMaxAtlasWidthOverride)
			{
				texturePipelineData._maxAtlasWidth = this._maxAtlasWidthOverride;
				texturePipelineData._useMaxAtlasWidthOverride = true;
			}
			else
			{
				texturePipelineData._maxAtlasWidth = this._maxAtlasSize;
			}
			texturePipelineData._saveAtlasesAsAssets = this._saveAtlasesAsAssets;
			texturePipelineData.resultType = this._resultType;
			texturePipelineData._resizePowerOfTwoTextures = this._resizePowerOfTwoTextures;
			texturePipelineData._fixOutOfBoundsUVs = this._fixOutOfBoundsUVs;
			texturePipelineData._maxTilingBakeSize = this._maxTilingBakeSize;
			texturePipelineData._packingAlgorithm = this._packingAlgorithm;
			texturePipelineData._layerTexturePackerFastV2 = this._layerTexturePackerFastMesh;
			texturePipelineData._meshBakerTexturePackerForcePowerOfTwo = this._meshBakerTexturePackerForcePowerOfTwo;
			texturePipelineData._customShaderPropNames = this._customShaderPropNames;
			texturePipelineData._normalizeTexelDensity = this._normalizeTexelDensity;
			texturePipelineData._considerNonTextureProperties = this._considerNonTextureProperties;
			texturePipelineData.doMergeDistinctMaterialTexturesThatWouldExceedAtlasSize = this._doMergeDistinctMaterialTexturesThatWouldExceedAtlasSize;
			texturePipelineData.nonTexturePropertyBlender = new MB3_TextureCombinerNonTextureProperties(this.LOG_LEVEL, this._considerNonTextureProperties);
			texturePipelineData.resultMaterial = resultMaterial;
			texturePipelineData.distinctMaterialTextures = distinctMaterialTextures;
			texturePipelineData.allObjsToMesh = objsToMesh;
			texturePipelineData.allowedMaterialsFilter = allowedMaterialsFilter;
			texturePipelineData.texPropertyNames = texPropertyNames;
			texturePipelineData.texPropNamesToIgnore = texPropsToIgnore;
			texturePipelineData.colorSpace = MBVersion.GetProjectColorSpace();
			return texturePipelineData;
		}

		private IEnumerator __CombineTexturesIntoAtlases(ProgressUpdateDelegate progressInfo, MB3_TextureCombiner.CombineTexturesIntoAtlasesCoroutineResult result, MB_AtlasesAndRects resultAtlasesAndRects, MB3_TextureCombinerPipeline.TexturePipelineData data, MB2_EditorMethodsInterface textureEditorMethods)
		{
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				Debug.Log(string.Concat(new string[]
				{
					"__CombineTexturesIntoAtlases texture properties in shader:",
					data.texPropertyNames.Count.ToString(),
					" objsToMesh:",
					data.allObjsToMesh.Count.ToString(),
					" _fixOutOfBoundsUVs:",
					data._fixOutOfBoundsUVs.ToString()
				}));
			}
			if (progressInfo != null)
			{
				progressInfo("Collecting textures ", 0.01f);
			}
			MB3_TextureCombinerPipeline pipeline = new MB3_TextureCombinerPipeline();
			List<GameObject> usedObjsToMesh = new List<GameObject>();
			yield return pipeline.__Step1_CollectDistinctMatTexturesAndUsedObjects(progressInfo, result, data, this, textureEditorMethods, usedObjsToMesh, this.LOG_LEVEL);
			if (!result.success)
			{
				yield break;
			}
			yield return pipeline.CalculateIdealSizesForTexturesInAtlasAndPadding(progressInfo, result, data, this, textureEditorMethods, this.LOG_LEVEL);
			if (!result.success)
			{
				yield break;
			}
			StringBuilder report = pipeline.GenerateReport(data);
			MB_ITextureCombinerPacker texturePaker = pipeline.CreatePacker(data.OnlyOneTextureInAtlasReuseTextures(), data._packingAlgorithm);
			if (!texturePaker.Validate(data))
			{
				result.success = false;
				yield break;
			}
			yield return texturePaker.ConvertTexturesToReadableFormats(progressInfo, result, data, this, textureEditorMethods, this.LOG_LEVEL);
			if (!result.success)
			{
				yield break;
			}
			AtlasPackingResult[] array = texturePaker.CalculateAtlasRectangles(data, false, this.LOG_LEVEL);
			yield return pipeline.__Step3_BuildAndSaveAtlasesAndStoreResults(result, progressInfo, data, this, texturePaker, array[0], textureEditorMethods, resultAtlasesAndRects, report, this.LOG_LEVEL);
			yield break;
		}

		private IEnumerator __RunTexturePackerOnly(MB3_TextureCombiner.CombineTexturesIntoAtlasesCoroutineResult result, MB_AtlasesAndRects resultAtlasesAndRects, MB3_TextureCombinerPipeline.TexturePipelineData data, bool splitAtlasWhenPackingIfTooBig, MB2_EditorMethodsInterface textureEditorMethods, List<AtlasPackingResult> packingResult)
		{
			MB3_TextureCombinerPipeline pipeline = new MB3_TextureCombinerPipeline();
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				Debug.Log(string.Concat(new string[]
				{
					"__RunTexturePacker texture properties in shader:",
					data.texPropertyNames.Count.ToString(),
					" objsToMesh:",
					data.allObjsToMesh.Count.ToString(),
					" _fixOutOfBoundsUVs:",
					data._fixOutOfBoundsUVs.ToString()
				}));
			}
			List<GameObject> usedObjsToMesh = new List<GameObject>();
			yield return pipeline.__Step1_CollectDistinctMatTexturesAndUsedObjects(null, result, data, this, textureEditorMethods, usedObjsToMesh, this.LOG_LEVEL);
			if (!result.success)
			{
				yield break;
			}
			data.allTexturesAreNullAndSameColor = new MB3_TextureCombinerPipeline.CreateAtlasForProperty[data.texPropertyNames.Count];
			yield return pipeline.CalculateIdealSizesForTexturesInAtlasAndPadding(null, result, data, this, textureEditorMethods, this.LOG_LEVEL);
			if (!result.success)
			{
				yield break;
			}
			MB_ITextureCombinerPacker texturePacker = pipeline.CreatePacker(data.OnlyOneTextureInAtlasReuseTextures(), data._packingAlgorithm);
			AtlasPackingResult[] array = pipeline.RunTexturePackerOnly(data, splitAtlasWhenPackingIfTooBig, resultAtlasesAndRects, texturePacker, this.LOG_LEVEL);
			for (int i = 0; i < array.Length; i++)
			{
				packingResult.Add(array[i]);
			}
			yield break;
		}

		internal int _getNumTemporaryTextures()
		{
			return this._temporaryTextures.Count;
		}

		public Texture2D _createTemporaryTexture(string propertyName, int w, int h, TextureFormat texFormat, bool mipMaps, bool linear)
		{
			Texture2D texture2D = new Texture2D(w, h, texFormat, mipMaps, linear);
			texture2D.name = string.Format("tmp{0}_{1}x{2}", this._temporaryTextures.Count, w, h);
			MB_Utility.setSolidColor(texture2D, Color.clear);
			MB3_TextureCombiner.TemporaryTexture item = new MB3_TextureCombiner.TemporaryTexture(propertyName, texture2D);
			this._temporaryTextures.Add(item);
			return texture2D;
		}

		internal void AddTemporaryTexture(MB3_TextureCombiner.TemporaryTexture tt)
		{
			this._temporaryTextures.Add(tt);
		}

		internal Texture2D _createTextureCopy(ShaderTextureProperty propertyName, Texture2D t)
		{
			Texture2D texture2D = MB_Utility.createTextureCopy(t, propertyName.isGammaCorrected);
			texture2D.name = string.Format("tmpCopy{0}_{1}x{2}", this._temporaryTextures.Count, texture2D.width, texture2D.height);
			MB3_TextureCombiner.TemporaryTexture item = new MB3_TextureCombiner.TemporaryTexture(propertyName.name, texture2D);
			this._temporaryTextures.Add(item);
			return texture2D;
		}

		internal Texture2D _resizeTexture(ShaderTextureProperty propertyName, Texture2D t, int w, int h)
		{
			Texture2D texture2D = MB_Utility.resampleTexture(t, propertyName.isGammaCorrected, w, h);
			texture2D.name = string.Format("tmpResampled{0}_{1}x{2}", this._temporaryTextures.Count, w, h);
			MB3_TextureCombiner.TemporaryTexture item = new MB3_TextureCombiner.TemporaryTexture(propertyName.name, texture2D);
			this._temporaryTextures.Add(item);
			return texture2D;
		}

		internal void _destroyAllTemporaryTextures()
		{
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				Debug.Log("Destroying " + this._temporaryTextures.Count.ToString() + " temporary textures");
			}
			for (int i = 0; i < this._temporaryTextures.Count; i++)
			{
				MB_Utility.Destroy(this._temporaryTextures[i].texture);
			}
			this._temporaryTextures.Clear();
		}

		internal void _destroyTemporaryTextures(string propertyName)
		{
			int num = 0;
			for (int i = this._temporaryTextures.Count - 1; i >= 0; i--)
			{
				if (this._temporaryTextures[i].property.Equals(propertyName))
				{
					num++;
					MB_Utility.Destroy(this._temporaryTextures[i].texture);
					this._temporaryTextures.RemoveAt(i);
				}
			}
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				Debug.Log(string.Concat(new string[]
				{
					"Destroying ",
					num.ToString(),
					" temporary textures ",
					propertyName,
					" num remaining ",
					this._temporaryTextures.Count.ToString()
				}));
			}
		}

		public void _restoreProceduralMaterials()
		{
		}

		public void SuggestTreatment(List<GameObject> objsToMesh, Material[] resultMaterials, List<ShaderTextureProperty> _customShaderPropNames, List<string> texPropsToIgnore)
		{
			this._customShaderPropNames = _customShaderPropNames;
			StringBuilder stringBuilder = new StringBuilder();
			Dictionary<int, MB_Utility.MeshAnalysisResult[]> dictionary = new Dictionary<int, MB_Utility.MeshAnalysisResult[]>();
			for (int i = 0; i < objsToMesh.Count; i++)
			{
				GameObject gameObject = objsToMesh[i];
				if (!(gameObject == null))
				{
					Material[] gomaterials = MB_Utility.GetGOMaterials(objsToMesh[i]);
					if (gomaterials.Length > 1)
					{
						stringBuilder.AppendFormat("\nObject {0} uses {1} materials. Possible treatments:\n", objsToMesh[i].name, gomaterials.Length);
						stringBuilder.AppendFormat("  1) Collapse the submeshes together into one submesh in the combined mesh. Each of the original submesh materials will map to a different UV rectangle in the atlas(es) used by the combined material.\n", Array.Empty<object>());
						stringBuilder.AppendFormat("  2) Use the multiple materials feature to map submeshes in the source mesh to submeshes in the combined mesh.\n", Array.Empty<object>());
					}
					Mesh mesh = MB_Utility.GetMesh(gameObject);
					MB_Utility.MeshAnalysisResult[] array;
					if (!dictionary.TryGetValue(mesh.GetInstanceID(), out array))
					{
						array = new MB_Utility.MeshAnalysisResult[mesh.subMeshCount];
						MB_Utility.doSubmeshesShareVertsOrTris(mesh, ref array[0]);
						for (int j = 0; j < mesh.subMeshCount; j++)
						{
							MB_Utility.hasOutOfBoundsUVs(mesh, ref array[j], j, 0);
							array[j].hasOverlappingSubmeshVerts = array[0].hasOverlappingSubmeshVerts;
						}
						dictionary.Add(mesh.GetInstanceID(), array);
					}
					for (int k = 0; k < gomaterials.Length; k++)
					{
						if (array[k].hasOutOfBoundsUVs)
						{
							DRect drect = new DRect(array[k].uvRect);
							stringBuilder.AppendFormat("\nObject {0} submesh={1} material={2} uses UVs outside the range 0,0 .. 1,1 to create tiling that tiles the box {3},{4} .. {5},{6}. This is a problem because the UVs outside the 0,0 .. 1,1 rectangle will pick up neighboring textures in the atlas. Possible Treatments:\n", new object[]
							{
								gameObject,
								k,
								gomaterials[k],
								drect.x.ToString("G4"),
								drect.y.ToString("G4"),
								(drect.x + drect.width).ToString("G4"),
								(drect.y + drect.height).ToString("G4")
							});
							stringBuilder.AppendFormat("    1) Ignore the problem. The tiling may not affect result significantly.\n", Array.Empty<object>());
							stringBuilder.AppendFormat("    2) Use the 'Consider Mesh UVs' feature to bake the tiling and scale the UVs to fit in the 0,0 .. 1,1 rectangle.\n", Array.Empty<object>());
							stringBuilder.AppendFormat("    3) Use the Multiple Materials feature to map the material on this submesh to its own submesh in the combined mesh. No other materials should map to this submesh. This will result in only one texture in the atlas(es) and the UVs should tile correctly.\n", Array.Empty<object>());
							stringBuilder.AppendFormat("    4) Combine only meshes that use the same (or subset of) the set of materials on this mesh. The original material(s) can be applied to the result\n", Array.Empty<object>());
						}
					}
					if (array[0].hasOverlappingSubmeshVerts)
					{
						stringBuilder.AppendFormat("\nObject {0} has submeshes that share vertices. This is a problem because each vertex can have only one UV coordinate and may be required to map to different positions in the various atlases that are generated. Possible treatments:\n", objsToMesh[i]);
						stringBuilder.AppendFormat(" 1) Ignore the problem. The vertices may not affect the result.\n", Array.Empty<object>());
						stringBuilder.AppendFormat(" 2) Use the Multiple Materials feature to map the submeshs that overlap to their own submeshs in the combined mesh. No other materials should map to this submesh. This will result in only one texture in the atlas(es) and the UVs should tile correctly.\n", Array.Empty<object>());
						stringBuilder.AppendFormat(" 3) Combine only meshes that use the same (or subset of) the set of materials on this mesh. The original material(s) can be applied to the result\n", Array.Empty<object>());
					}
				}
			}
			Dictionary<Material, List<GameObject>> dictionary2 = new Dictionary<Material, List<GameObject>>();
			for (int l = 0; l < objsToMesh.Count; l++)
			{
				if (objsToMesh[l] != null)
				{
					Material[] gomaterials2 = MB_Utility.GetGOMaterials(objsToMesh[l]);
					for (int m = 0; m < gomaterials2.Length; m++)
					{
						if (gomaterials2[m] != null)
						{
							List<GameObject> list;
							if (!dictionary2.TryGetValue(gomaterials2[m], out list))
							{
								list = new List<GameObject>();
								dictionary2.Add(gomaterials2[m], list);
							}
							if (!list.Contains(objsToMesh[l]))
							{
								list.Add(objsToMesh[l]);
							}
						}
					}
				}
			}
			for (int n = 0; n < resultMaterials.Length; n++)
			{
				string shaderName = (resultMaterials[n] != null) ? "None" : resultMaterials[n].shader.name;
				MB3_TextureCombinerPipeline.TexturePipelineData texturePipelineData = this.LoadPipelineData(resultMaterials[n], new List<ShaderTextureProperty>(), objsToMesh, new List<Material>(), texPropsToIgnore, new List<MB_TexSet>());
				MB3_TextureCombinerPipeline._CollectPropertyNames(texturePipelineData, this.LOG_LEVEL);
				foreach (Material material in dictionary2.Keys)
				{
					for (int num = 0; num < texturePipelineData.texPropertyNames.Count; num++)
					{
						if (material.HasProperty(texturePipelineData.texPropertyNames[num].name))
						{
							Texture textureConsideringStandardShaderKeywords = MB3_TextureCombinerPipeline.GetTextureConsideringStandardShaderKeywords(shaderName, material, texturePipelineData.texPropertyNames[num].name);
							if (textureConsideringStandardShaderKeywords != null)
							{
								Vector2 textureOffset = material.GetTextureOffset(texturePipelineData.texPropertyNames[num].name);
								Vector3 vector = material.GetTextureScale(texturePipelineData.texPropertyNames[num].name);
								if (textureOffset.x < 0f || textureOffset.x + vector.x > 1f || textureOffset.y < 0f || textureOffset.y + vector.y > 1f)
								{
									stringBuilder.AppendFormat("\nMaterial {0} used by objects {1} uses texture {2} that is tiled (scale={3} offset={4}). If there is more than one texture in the atlas  then Mesh Baker will bake the tiling into the atlas. If the baked tiling is large then quality can be lost. Possible treatments:\n", new object[]
									{
										material,
										this.PrintList(dictionary2[material]),
										textureConsideringStandardShaderKeywords,
										vector,
										textureOffset
									});
									stringBuilder.AppendFormat("  1) Use the baked tiling.\n", Array.Empty<object>());
									stringBuilder.AppendFormat("  2) Use the Multiple Materials feature to map the material on this object/submesh to its own submesh in the combined mesh. No other materials should map to this submesh. The original material can be applied to this submesh.\n", Array.Empty<object>());
									stringBuilder.AppendFormat("  3) Combine only meshes that use the same (or subset of) the set of textures on this mesh. The original material can be applied to the result.\n", Array.Empty<object>());
								}
							}
						}
					}
				}
			}
			string message;
			if (stringBuilder.Length == 0)
			{
				message = "====== No problems detected. These meshes should combine well ====\n  If there are problems with the combined meshes please report the problem to digitalOpus.ca so we can improve Mesh Baker.";
			}
			else
			{
				message = "====== There are possible problems with these meshes that may prevent them from combining well. TREATMENT SUGGESTIONS (copy and paste to text editor if too big) =====\n" + stringBuilder.ToString();
			}
			Debug.Log(message);
		}

		public static bool ShouldTextureBeLinear(ShaderTextureProperty shaderTextureProperty)
		{
			return shaderTextureProperty.isNormalMap || !shaderTextureProperty.isGammaCorrected;
		}

		private string PrintList(List<GameObject> gos)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < gos.Count; i++)
			{
				StringBuilder stringBuilder2 = stringBuilder;
				GameObject gameObject = gos[i];
				stringBuilder2.Append(((gameObject != null) ? gameObject.ToString() : null) + ",");
			}
			return stringBuilder.ToString();
		}

		public const int TEMP_SOLID_COLOR_TEXTURE_SIZE = 16;

		public static Color NEUTRAL_NORMAL_MAP_COLOR_SWIZZLED = new Color(1f, 0.5f, 0.5f, 0.5f);

		public static Color NEUTRAL_NORMAL_MAP_COLOR_NON_SWIZZLED = new Color(0.5f, 0.5f, 1f, 0.5f);

		public MB2_LogLevel LOG_LEVEL = MB2_LogLevel.info;

		[SerializeField]
		protected MB2_TextureBakeResults _textureBakeResults;

		[SerializeField]
		protected int _atlasPadding = 1;

		[SerializeField]
		protected int _maxAtlasSize = 1;

		[SerializeField]
		protected int _maxAtlasWidthOverride = 8192;

		[SerializeField]
		protected int _maxAtlasHeightOverride = 8192;

		[SerializeField]
		protected bool _useMaxAtlasWidthOverride;

		[SerializeField]
		protected bool _useMaxAtlasHeightOverride;

		[SerializeField]
		protected bool _resizePowerOfTwoTextures;

		[SerializeField]
		protected bool _fixOutOfBoundsUVs;

		[SerializeField]
		protected int _layerTexturePackerFastMesh = -1;

		[SerializeField]
		protected int _maxTilingBakeSize = 1024;

		[SerializeField]
		protected bool _saveAtlasesAsAssets;

		[SerializeField]
		protected MB2_TextureBakeResults.ResultType _resultType;

		[SerializeField]
		protected MB2_PackingAlgorithmEnum _packingAlgorithm;

		[SerializeField]
		protected bool _meshBakerTexturePackerForcePowerOfTwo = true;

		[SerializeField]
		protected List<ShaderTextureProperty> _customShaderPropNames = new List<ShaderTextureProperty>();

		[SerializeField]
		protected bool _normalizeTexelDensity;

		[SerializeField]
		protected bool _considerNonTextureProperties;

		protected bool _doMergeDistinctMaterialTexturesThatWouldExceedAtlasSize;

		private List<MB3_TextureCombiner.TemporaryTexture> _temporaryTextures = new List<MB3_TextureCombiner.TemporaryTexture>();

		public static bool _RunCorutineWithoutPauseIsRunning = false;

		public class CreateAtlasesCoroutineResult
		{
			public bool success = true;

			public bool isFinished;
		}

		internal class TemporaryTexture
		{
			public TemporaryTexture(string prop, Texture2D tex)
			{
				this.property = prop;
				this.texture = tex;
			}

			internal string property;

			internal Texture2D texture;
		}

		public class CombineTexturesIntoAtlasesCoroutineResult
		{
			public bool success = true;

			public bool isFinished;
		}
	}
}
