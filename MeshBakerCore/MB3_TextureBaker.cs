using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DigitalOpus.MB.Core;
using UnityEngine;

public class MB3_TextureBaker : MB3_MeshBakerRoot
{
	public override MB2_TextureBakeResults textureBakeResults
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

	public virtual int atlasPadding
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

	public virtual int maxAtlasSize
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

	public virtual bool resizePowerOfTwoTextures
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

	public virtual bool fixOutOfBoundsUVs
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

	public virtual int maxTilingBakeSize
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

	public virtual MB2_PackingAlgorithmEnum packingAlgorithm
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

	public virtual int layerForTexturePackerFastMesh
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

	public virtual List<ShaderTextureProperty> customShaderProperties
	{
		get
		{
			return this._customShaderProperties;
		}
		set
		{
			this._customShaderProperties = value;
		}
	}

	public virtual List<string> texturePropNamesToIgnore
	{
		get
		{
			return this._texturePropNamesToIgnore;
		}
		set
		{
			this._texturePropNamesToIgnore = value;
		}
	}

	public virtual List<string> customShaderPropNames
	{
		get
		{
			return this._customShaderPropNames_Depricated;
		}
		set
		{
			this._customShaderPropNames_Depricated = value;
		}
	}

	public virtual MB2_TextureBakeResults.ResultType resultType
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

	public virtual bool doMultiMaterial
	{
		get
		{
			return this._doMultiMaterial;
		}
		set
		{
			this._doMultiMaterial = value;
		}
	}

	public virtual bool doMultiMaterialSplitAtlasesIfTooBig
	{
		get
		{
			return this._doMultiMaterialSplitAtlasesIfTooBig;
		}
		set
		{
			this._doMultiMaterialSplitAtlasesIfTooBig = value;
		}
	}

	public virtual bool doMultiMaterialSplitAtlasesIfOBUVs
	{
		get
		{
			return this._doMultiMaterialSplitAtlasesIfOBUVs;
		}
		set
		{
			this._doMultiMaterialSplitAtlasesIfOBUVs = value;
		}
	}

	public virtual Material resultMaterial
	{
		get
		{
			return this._resultMaterial;
		}
		set
		{
			this._resultMaterial = value;
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

	public bool doSuggestTreatment
	{
		get
		{
			return this._doSuggestTreatment;
		}
		set
		{
			this._doSuggestTreatment = value;
		}
	}

	public MB3_TextureCombiner.CreateAtlasesCoroutineResult CoroutineResult
	{
		get
		{
			return this._coroutineResult;
		}
	}

	public override List<GameObject> GetObjectsToCombine()
	{
		if (this.objsToMesh == null)
		{
			this.objsToMesh = new List<GameObject>();
		}
		return this.objsToMesh;
	}

	[ContextMenu("Purge Objects to Combine of null references")]
	public override void PurgeNullsFromObjectsToCombine()
	{
		if (this.objsToMesh == null)
		{
			this.objsToMesh = new List<GameObject>();
		}
		Debug.Log(string.Format("Purged {0} null references from objects to combine list.", this.objsToMesh.RemoveAll((GameObject obj) => obj == null)));
	}

	public MB_AtlasesAndRects[] CreateAtlases()
	{
		return this.CreateAtlases(null, false, null);
	}

	public IEnumerator CreateAtlasesCoroutine(ProgressUpdateDelegate progressInfo, MB3_TextureCombiner.CreateAtlasesCoroutineResult coroutineResult, bool saveAtlasesAsAssets = false, MB2_EditorMethodsInterface editorMethods = null, float maxTimePerFrame = 0.01f)
	{
		yield return this._CreateAtlasesCoroutine(progressInfo, coroutineResult, saveAtlasesAsAssets, editorMethods, maxTimePerFrame);
		if (coroutineResult.success && this.onBuiltAtlasesSuccess != null)
		{
			this.onBuiltAtlasesSuccess();
		}
		if (!coroutineResult.success && this.onBuiltAtlasesFail != null)
		{
			this.onBuiltAtlasesFail();
		}
		yield break;
	}

	private IEnumerator _CreateAtlasesCoroutineAtlases(MB3_TextureCombiner combiner, ProgressUpdateDelegate progressInfo, MB3_TextureCombiner.CreateAtlasesCoroutineResult coroutineResult, bool saveAtlasesAsAssets = false, MB2_EditorMethodsInterface editorMethods = null, float maxTimePerFrame = 0.01f)
	{
		int num = 1;
		if (this._doMultiMaterial)
		{
			num = this.resultMaterials.Length;
		}
		this.OnCombinedTexturesCoroutineAtlasesAndRects = new MB_AtlasesAndRects[num];
		for (int j = 0; j < this.OnCombinedTexturesCoroutineAtlasesAndRects.Length; j++)
		{
			this.OnCombinedTexturesCoroutineAtlasesAndRects[j] = new MB_AtlasesAndRects();
		}
		int num2;
		for (int i = 0; i < this.OnCombinedTexturesCoroutineAtlasesAndRects.Length; i = num2 + 1)
		{
			List<Material> allowedMaterialsFilter = null;
			Material resultMaterial;
			if (this._doMultiMaterial)
			{
				allowedMaterialsFilter = this.resultMaterials[i].sourceMaterials;
				resultMaterial = this.resultMaterials[i].combinedMaterial;
				combiner.fixOutOfBoundsUVs = this.resultMaterials[i].considerMeshUVs;
			}
			else
			{
				resultMaterial = this._resultMaterial;
			}
			MB3_TextureCombiner.CombineTexturesIntoAtlasesCoroutineResult coroutineResult2 = new MB3_TextureCombiner.CombineTexturesIntoAtlasesCoroutineResult();
			yield return combiner.CombineTexturesIntoAtlasesCoroutine(progressInfo, this.OnCombinedTexturesCoroutineAtlasesAndRects[i], resultMaterial, this.objsToMesh, allowedMaterialsFilter, this.texturePropNamesToIgnore, editorMethods, coroutineResult2, maxTimePerFrame, null, false, false);
			coroutineResult.success = coroutineResult2.success;
			if (!coroutineResult.success)
			{
				coroutineResult.isFinished = true;
				yield break;
			}
			coroutineResult2 = null;
			num2 = i;
		}
		this.unpackMat2RectMap(this.OnCombinedTexturesCoroutineAtlasesAndRects);
		if (coroutineResult.success && editorMethods != null)
		{
			editorMethods.GetMaterialPrimaryKeysIfAddressables(this.textureBakeResults);
		}
		this.textureBakeResults.resultType = MB2_TextureBakeResults.ResultType.atlas;
		this.textureBakeResults.resultMaterialsTexArray = new MB_MultiMaterialTexArray[0];
		this.textureBakeResults.doMultiMaterial = this._doMultiMaterial;
		if (this._doMultiMaterial)
		{
			this.textureBakeResults.resultMaterials = this.resultMaterials;
		}
		else
		{
			MB_MultiMaterial[] array = new MB_MultiMaterial[]
			{
				new MB_MultiMaterial()
			};
			array[0].combinedMaterial = this._resultMaterial;
			array[0].considerMeshUVs = this._fixOutOfBoundsUVs;
			array[0].sourceMaterials = new List<Material>();
			for (int k = 0; k < this.textureBakeResults.materialsAndUVRects.Length; k++)
			{
				array[0].sourceMaterials.Add(this.textureBakeResults.materialsAndUVRects[k].material);
			}
			this.textureBakeResults.resultMaterials = array;
		}
		if (this.LOG_LEVEL >= MB2_LogLevel.info)
		{
			Debug.Log("Created Atlases");
		}
		yield break;
	}

	internal IEnumerator _CreateAtlasesCoroutineTextureArray(MB3_TextureCombiner combiner, ProgressUpdateDelegate progressInfo, MB3_TextureCombiner.CreateAtlasesCoroutineResult coroutineResult, bool saveAtlasesAsAssets = false, MB2_EditorMethodsInterface editorMethods = null, float maxTimePerFrame = 0.01f)
	{
		MB_TextureArrayResultMaterial[] bakedMatsAndSlices = null;
		if (this.textureArrayOutputFormats == null || this.textureArrayOutputFormats.Length == 0)
		{
			Debug.LogError("No Texture Array Output Formats. There must be at least one entry.");
			coroutineResult.isFinished = true;
			yield break;
		}
		for (int i = 0; i < this.textureArrayOutputFormats.Length; i++)
		{
			if (!this.textureArrayOutputFormats[i].ValidateTextureImporterFormatsExistsForTextureFormats(editorMethods, i))
			{
				Debug.LogError("Could not map the selected texture format to a Texture Importer Format. Safest options are ARGB32, or RGB24.");
				coroutineResult.isFinished = true;
				yield break;
			}
		}
		for (int j = 0; j < this.resultMaterialsTexArray.Length; j++)
		{
			MB_MultiMaterialTexArray mb_MultiMaterialTexArray = this.resultMaterialsTexArray[j];
			if (mb_MultiMaterialTexArray.combinedMaterial == null)
			{
				Debug.LogError("Material is null for Texture Array Slice Configuration: " + j.ToString() + ".");
				coroutineResult.isFinished = true;
				yield break;
			}
			List<MB_TexArraySlice> slices = mb_MultiMaterialTexArray.slices;
			for (int k = 0; k < slices.Count; k++)
			{
				for (int l = 0; l < slices[k].sourceMaterials.Count; l++)
				{
					MB_TexArraySliceRendererMatPair mb_TexArraySliceRendererMatPair = slices[k].sourceMaterials[l];
					if (mb_TexArraySliceRendererMatPair.sourceMaterial == null)
					{
						Debug.LogError("Source material is null for Texture Array Slice Configuration: " + j.ToString() + " slice: " + k.ToString());
						coroutineResult.isFinished = true;
						yield break;
					}
					if (slices[k].considerMeshUVs && mb_TexArraySliceRendererMatPair.renderer == null)
					{
						Debug.LogError(string.Concat(new string[]
						{
							"Renderer is null for Texture Array Slice Configuration: ",
							j.ToString(),
							" slice: ",
							k.ToString(),
							". If considerUVs is enabled then a renderer must be supplied for each source material. The same source material can be used multiple times."
						}));
						coroutineResult.isFinished = true;
						yield break;
					}
				}
			}
		}
		int num = this.resultMaterialsTexArray.Length;
		bakedMatsAndSlices = new MB_TextureArrayResultMaterial[num];
		for (int m = 0; m < bakedMatsAndSlices.Length; m++)
		{
			bakedMatsAndSlices[m] = new MB_TextureArrayResultMaterial();
			int count = this.resultMaterialsTexArray[m].slices.Count;
			MB_AtlasesAndRects[] array = bakedMatsAndSlices[m].slices = new MB_AtlasesAndRects[count];
			for (int n = 0; n < count; n++)
			{
				array[n] = new MB_AtlasesAndRects();
			}
		}
		int num2;
		for (int resMatIdx = 0; resMatIdx < bakedMatsAndSlices.Length; resMatIdx = num2 + 1)
		{
			yield return MB_TextureArrays._CreateAtlasesCoroutineSingleResultMaterial(resMatIdx, bakedMatsAndSlices[resMatIdx], this.resultMaterialsTexArray[resMatIdx], this.objsToMesh, combiner, this.textureArrayOutputFormats, this.resultMaterialsTexArray, this.customShaderProperties, this.texturePropNamesToIgnore, progressInfo, coroutineResult, saveAtlasesAsAssets, editorMethods, maxTimePerFrame);
			if (!coroutineResult.success)
			{
				yield break;
			}
			num2 = resMatIdx;
		}
		if (coroutineResult.success)
		{
			this.unpackMat2RectMap(bakedMatsAndSlices);
			if (editorMethods != null)
			{
				editorMethods.GetMaterialPrimaryKeysIfAddressables(this.textureBakeResults);
			}
			this.textureBakeResults.resultType = MB2_TextureBakeResults.ResultType.textureArray;
			this.textureBakeResults.resultMaterials = new MB_MultiMaterial[0];
			this.textureBakeResults.resultMaterialsTexArray = this.resultMaterialsTexArray;
			if (this.LOG_LEVEL >= MB2_LogLevel.info)
			{
				Debug.Log("Created Texture2DArrays");
			}
		}
		else if (this.LOG_LEVEL >= MB2_LogLevel.info)
		{
			Debug.Log("Failed to create Texture2DArrays");
		}
		yield break;
	}

	private IEnumerator _CreateAtlasesCoroutine(ProgressUpdateDelegate progressInfo, MB3_TextureCombiner.CreateAtlasesCoroutineResult coroutineResult, bool saveAtlasesAsAssets = false, MB2_EditorMethodsInterface editorMethods = null, float maxTimePerFrame = 0.01f)
	{
		new MBVersionConcrete();
		this.OnCombinedTexturesCoroutineAtlasesAndRects = null;
		if (maxTimePerFrame <= 0f)
		{
			Debug.LogError("maxTimePerFrame must be a value greater than zero");
			coroutineResult.isFinished = true;
			yield break;
		}
		MB2_ValidationLevel validationLevel = Application.isPlaying ? MB2_ValidationLevel.quick : MB2_ValidationLevel.robust;
		if (!MB3_MeshBakerRoot.DoCombinedValidate(this, MB_ObjsToCombineTypes.dontCare, null, validationLevel))
		{
			coroutineResult.isFinished = true;
			yield break;
		}
		if (this._doMultiMaterial && !this._ValidateResultMaterials())
		{
			coroutineResult.isFinished = true;
			yield break;
		}
		if (this.resultType != MB2_TextureBakeResults.ResultType.textureArray && !this._doMultiMaterial)
		{
			if (this._resultMaterial == null)
			{
				Debug.LogError("Combined Material is null please create and assign a result material.");
				coroutineResult.isFinished = true;
				yield break;
			}
			Shader shader = this._resultMaterial.shader;
			for (int i = 0; i < this.objsToMesh.Count; i++)
			{
				foreach (Material material in MB_Utility.GetGOMaterials(this.objsToMesh[i]))
				{
					if (material != null && material.shader != shader)
					{
						string[] array = new string[5];
						array[0] = "Game object ";
						int num = 1;
						GameObject gameObject = this.objsToMesh[i];
						array[num] = ((gameObject != null) ? gameObject.ToString() : null);
						array[2] = " does not use shader ";
						int num2 = 3;
						Shader shader2 = shader;
						array[num2] = ((shader2 != null) ? shader2.ToString() : null);
						array[4] = " it may not have the required textures. If not small solid color textures will be generated.";
						Debug.LogWarning(string.Concat(array));
					}
				}
			}
		}
		MB3_TextureCombiner mb3_TextureCombiner = this.CreateAndConfigureTextureCombiner();
		mb3_TextureCombiner.saveAtlasesAsAssets = saveAtlasesAsAssets;
		this.OnCombinedTexturesCoroutineAtlasesAndRects = null;
		if (this.resultType == MB2_TextureBakeResults.ResultType.textureArray)
		{
			yield return this._CreateAtlasesCoroutineTextureArray(mb3_TextureCombiner, progressInfo, coroutineResult, saveAtlasesAsAssets, editorMethods, maxTimePerFrame);
			if (!coroutineResult.success)
			{
				yield break;
			}
		}
		else
		{
			yield return this._CreateAtlasesCoroutineAtlases(mb3_TextureCombiner, progressInfo, coroutineResult, saveAtlasesAsAssets, editorMethods, maxTimePerFrame);
			if (!coroutineResult.success)
			{
				yield break;
			}
		}
		MB3_MeshBakerCommon[] componentsInChildren = base.GetComponentsInChildren<MB3_MeshBakerCommon>();
		for (int k = 0; k < componentsInChildren.Length; k++)
		{
			componentsInChildren[k].textureBakeResults = this.textureBakeResults;
		}
		coroutineResult.isFinished = true;
		yield break;
	}

	public MB_AtlasesAndRects[] CreateAtlases(ProgressUpdateDelegate progressInfo, bool saveAtlasesAsAssets = false, MB2_EditorMethodsInterface editorMethods = null)
	{
		MB_AtlasesAndRects[] array = null;
		try
		{
			this._coroutineResult = new MB3_TextureCombiner.CreateAtlasesCoroutineResult();
			MB3_TextureCombiner.RunCorutineWithoutPause(this.CreateAtlasesCoroutine(progressInfo, this._coroutineResult, saveAtlasesAsAssets, editorMethods, 1000f), 0);
			if (this._coroutineResult.success && this.textureBakeResults != null)
			{
				array = this.OnCombinedTexturesCoroutineAtlasesAndRects;
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message + "\n" + ex.StackTrace.ToString());
		}
		finally
		{
			if (saveAtlasesAsAssets && array != null)
			{
				foreach (MB_AtlasesAndRects mb_AtlasesAndRects in array)
				{
					if (mb_AtlasesAndRects != null && mb_AtlasesAndRects.atlases != null)
					{
						for (int j = 0; j < mb_AtlasesAndRects.atlases.Length; j++)
						{
							if (mb_AtlasesAndRects.atlases[j] != null)
							{
								if (editorMethods != null)
								{
									editorMethods.Destroy(mb_AtlasesAndRects.atlases[j]);
								}
								else
								{
									MB_Utility.Destroy(mb_AtlasesAndRects.atlases[j]);
								}
							}
						}
					}
				}
			}
		}
		return array;
	}

	private void unpackMat2RectMap(MB_AtlasesAndRects[] rawResults)
	{
		List<MB_MaterialAndUVRect> list = new List<MB_MaterialAndUVRect>();
		for (int i = 0; i < rawResults.Length; i++)
		{
			List<MB_MaterialAndUVRect> mat2rect_map = rawResults[i].mat2rect_map;
			if (mat2rect_map != null)
			{
				for (int j = 0; j < mat2rect_map.Count; j++)
				{
					mat2rect_map[j].textureArraySliceIdx = -1;
					list.Add(mat2rect_map[j]);
				}
			}
		}
		this.textureBakeResults.version = MB2_TextureBakeResults.VERSION;
		this.textureBakeResults.materialsAndUVRects = list.ToArray();
	}

	internal void unpackMat2RectMap(MB_TextureArrayResultMaterial[] rawResults)
	{
		List<MB_MaterialAndUVRect> list = new List<MB_MaterialAndUVRect>();
		for (int i = 0; i < rawResults.Length; i++)
		{
			MB_AtlasesAndRects[] slices = rawResults[i].slices;
			for (int j = 0; j < slices.Length; j++)
			{
				List<MB_MaterialAndUVRect> mat2rect_map = slices[j].mat2rect_map;
				if (mat2rect_map != null)
				{
					for (int k = 0; k < mat2rect_map.Count; k++)
					{
						mat2rect_map[k].textureArraySliceIdx = j;
						list.Add(mat2rect_map[k]);
					}
				}
			}
		}
		this.textureBakeResults.version = MB2_TextureBakeResults.VERSION;
		this.textureBakeResults.materialsAndUVRects = list.ToArray();
	}

	public MB3_TextureCombiner CreateAndConfigureTextureCombiner()
	{
		return new MB3_TextureCombiner
		{
			LOG_LEVEL = this.LOG_LEVEL,
			atlasPadding = this._atlasPadding,
			maxAtlasSize = this._maxAtlasSize,
			maxAtlasHeightOverride = this._maxAtlasHeightOverride,
			maxAtlasWidthOverride = this._maxAtlasWidthOverride,
			useMaxAtlasHeightOverride = this._useMaxAtlasHeightOverride,
			useMaxAtlasWidthOverride = this._useMaxAtlasWidthOverride,
			customShaderPropNames = this._customShaderProperties,
			fixOutOfBoundsUVs = this._fixOutOfBoundsUVs,
			maxTilingBakeSize = this._maxTilingBakeSize,
			packingAlgorithm = this._packingAlgorithm,
			layerTexturePackerFastMesh = this._layerTexturePackerFastMesh,
			resultType = this._resultType,
			meshBakerTexturePackerForcePowerOfTwo = this._meshBakerTexturePackerForcePowerOfTwo,
			resizePowerOfTwoTextures = this._resizePowerOfTwoTextures,
			considerNonTextureProperties = this._considerNonTextureProperties
		};
	}

	public static void ConfigureNewMaterialToMatchOld(Material newMat, Material original)
	{
		if (original == null)
		{
			string str = "Original material is null, could not copy properties to ";
			string str2 = (newMat != null) ? newMat.ToString() : null;
			string str3 = ". Setting shader to ";
			Shader shader = newMat.shader;
			Debug.LogWarning(str + str2 + str3 + ((shader != null) ? shader.ToString() : null));
			return;
		}
		newMat.shader = original.shader;
		newMat.CopyPropertiesFromMaterial(original);
		ShaderTextureProperty[] shaderTexPropertyNames = MB3_TextureCombinerPipeline.shaderTexPropertyNames;
		for (int i = 0; i < shaderTexPropertyNames.Length; i++)
		{
			Vector2 one = Vector2.one;
			Vector2 zero = Vector2.zero;
			if (newMat.HasProperty(shaderTexPropertyNames[i].name))
			{
				newMat.SetTextureOffset(shaderTexPropertyNames[i].name, zero);
				newMat.SetTextureScale(shaderTexPropertyNames[i].name, one);
			}
		}
	}

	private string PrintSet(HashSet<Material> s)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (Material material in s)
		{
			StringBuilder stringBuilder2 = stringBuilder;
			Material material2 = material;
			stringBuilder2.Append(((material2 != null) ? material2.ToString() : null) + ",");
		}
		return stringBuilder.ToString();
	}

	private bool _ValidateResultMaterials()
	{
		HashSet<Material> hashSet = new HashSet<Material>();
		for (int i = 0; i < this.objsToMesh.Count; i++)
		{
			if (this.objsToMesh[i] != null)
			{
				Material[] gomaterials = MB_Utility.GetGOMaterials(this.objsToMesh[i]);
				for (int j = 0; j < gomaterials.Length; j++)
				{
					if (gomaterials[j] != null)
					{
						hashSet.Add(gomaterials[j]);
					}
				}
			}
		}
		if (this.resultMaterials.Length < 1)
		{
			Debug.LogError("Using multiple materials but there are no 'Source Material To Combined Mappings'. You need at least one.");
		}
		HashSet<Material> hashSet2 = new HashSet<Material>();
		for (int k = 0; k < this.resultMaterials.Length; k++)
		{
			for (int l = k + 1; l < this.resultMaterials.Length; l++)
			{
				if (this.resultMaterials[k].combinedMaterial == this.resultMaterials[l].combinedMaterial)
				{
					Debug.LogError(string.Format("Source To Combined Mapping: Submesh {0} and Submesh {1} use the same combined material. These should be different", k, l));
					return false;
				}
			}
			MB_MultiMaterial mb_MultiMaterial = this.resultMaterials[k];
			if (mb_MultiMaterial.combinedMaterial == null)
			{
				Debug.LogError("Combined Material is null please create and assign a result material.");
				return false;
			}
			Shader shader = mb_MultiMaterial.combinedMaterial.shader;
			for (int m = 0; m < mb_MultiMaterial.sourceMaterials.Count; m++)
			{
				if (mb_MultiMaterial.sourceMaterials[m] == null)
				{
					Debug.LogError("There are null entries in the list of Source Materials");
					return false;
				}
				if (shader != mb_MultiMaterial.sourceMaterials[m].shader)
				{
					string[] array = new string[5];
					array[0] = "Source material ";
					int num = 1;
					Material material = mb_MultiMaterial.sourceMaterials[m];
					array[num] = ((material != null) ? material.ToString() : null);
					array[2] = " does not use shader ";
					int num2 = 3;
					Shader shader2 = shader;
					array[num2] = ((shader2 != null) ? shader2.ToString() : null);
					array[4] = " it may not have the required textures. If not empty textures will be generated.";
					Debug.LogWarning(string.Concat(array));
				}
				if (hashSet2.Contains(mb_MultiMaterial.sourceMaterials[m]))
				{
					string str = "A Material ";
					Material material2 = mb_MultiMaterial.sourceMaterials[m];
					Debug.LogError(str + ((material2 != null) ? material2.ToString() : null) + " appears more than once in the list of source materials in the source material to combined mapping. Each source material must be unique.");
					return false;
				}
				hashSet2.Add(mb_MultiMaterial.sourceMaterials[m]);
			}
		}
		if (hashSet.IsProperSubsetOf(hashSet2))
		{
			hashSet2.ExceptWith(hashSet);
			Debug.LogWarning("There are materials in the mapping that are not used on your source objects: " + this.PrintSet(hashSet2));
		}
		if (this.resultMaterials != null && this.resultMaterials.Length != 0 && hashSet2.IsProperSubsetOf(hashSet))
		{
			hashSet.ExceptWith(hashSet2);
			Debug.LogError("There are materials on the objects to combine that are not in the mapping: " + this.PrintSet(hashSet));
			return false;
		}
		return true;
	}

	public MB2_LogLevel LOG_LEVEL = MB2_LogLevel.info;

	[SerializeField]
	protected MB2_TextureBakeResults _textureBakeResults;

	[SerializeField]
	protected int _atlasPadding = 1;

	[SerializeField]
	protected int _maxAtlasSize = 8192;

	[SerializeField]
	protected bool _useMaxAtlasWidthOverride;

	[SerializeField]
	protected int _maxAtlasWidthOverride = 8192;

	[SerializeField]
	protected bool _useMaxAtlasHeightOverride;

	[SerializeField]
	protected int _maxAtlasHeightOverride = 8192;

	[SerializeField]
	protected bool _resizePowerOfTwoTextures;

	[SerializeField]
	protected bool _fixOutOfBoundsUVs;

	[SerializeField]
	protected int _maxTilingBakeSize = 1024;

	[SerializeField]
	protected MB2_PackingAlgorithmEnum _packingAlgorithm = MB2_PackingAlgorithmEnum.MeshBakerTexturePacker;

	[SerializeField]
	protected int _layerTexturePackerFastMesh = -1;

	[SerializeField]
	protected bool _meshBakerTexturePackerForcePowerOfTwo = true;

	[SerializeField]
	[NonReorderable]
	protected List<ShaderTextureProperty> _customShaderProperties = new List<ShaderTextureProperty>();

	[SerializeField]
	[NonReorderable]
	protected List<string> _texturePropNamesToIgnore = new List<string>();

	[SerializeField]
	protected List<string> _customShaderPropNames_Depricated = new List<string>();

	[SerializeField]
	protected MB2_TextureBakeResults.ResultType _resultType;

	[SerializeField]
	protected bool _doMultiMaterial;

	[SerializeField]
	protected bool _doMultiMaterialSplitAtlasesIfTooBig = true;

	[SerializeField]
	protected bool _doMultiMaterialSplitAtlasesIfOBUVs = true;

	[SerializeField]
	protected Material _resultMaterial;

	[SerializeField]
	protected bool _considerNonTextureProperties;

	[SerializeField]
	protected bool _doSuggestTreatment = true;

	private MB3_TextureCombiner.CreateAtlasesCoroutineResult _coroutineResult;

	[NonReorderable]
	public MB_MultiMaterial[] resultMaterials = new MB_MultiMaterial[0];

	[NonReorderable]
	public MB_MultiMaterialTexArray[] resultMaterialsTexArray = new MB_MultiMaterialTexArray[0];

	[NonReorderable]
	public MB_TextureArrayFormatSet[] textureArrayOutputFormats;

	[NonReorderable]
	public List<GameObject> objsToMesh;

	public MB3_TextureBaker.OnCombinedTexturesCoroutineSuccess onBuiltAtlasesSuccess;

	public MB3_TextureBaker.OnCombinedTexturesCoroutineFail onBuiltAtlasesFail;

	public MB_AtlasesAndRects[] OnCombinedTexturesCoroutineAtlasesAndRects;

	public delegate void OnCombinedTexturesCoroutineSuccess();

	public delegate void OnCombinedTexturesCoroutineFail();
}
