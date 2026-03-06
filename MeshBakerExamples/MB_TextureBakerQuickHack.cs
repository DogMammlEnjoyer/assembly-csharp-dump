using System;
using System.Collections.Generic;
using System.Text;
using DigitalOpus.MB.Core;
using UnityEngine;

namespace MeshBaker_Examples_2017.HackTextureAtlas
{
	public class MB_TextureBakerQuickHack : MonoBehaviour
	{
		[ContextMenu("Generate Material Bake Result")]
		public void CreateAtlas(Material[] passedInSourceMaterials)
		{
			Debug.Log("Validating source materials");
			bool flag = true;
			if (new HashSet<Material>(passedInSourceMaterials).Count != passedInSourceMaterials.Length)
			{
				Debug.LogError("Source materials are not unique");
				flag = false;
			}
			if (passedInSourceMaterials.Length == 0)
			{
				Debug.LogError("No source materials were passed in");
				flag = false;
			}
			if (string.IsNullOrEmpty(this.colorTintPropertyName))
			{
				Debug.LogError("ColorTintProperty is not set");
				flag = false;
			}
			this.sourceMaterials = new Material[passedInSourceMaterials.Length];
			for (int i = 0; i < passedInSourceMaterials.Length; i++)
			{
				if (passedInSourceMaterials[i] == null)
				{
					Debug.LogError("Source material " + i.ToString() + " is null");
					flag = false;
				}
				if (!passedInSourceMaterials[i].HasProperty(this.colorTintPropertyName))
				{
					Debug.LogError("Source material " + i.ToString() + " does not have the colorTint property");
					flag = false;
				}
				this.sourceMaterials[i] = passedInSourceMaterials[i];
				this.sourceMaterials[i].shader = Shader.Find(this.shaderName);
			}
			if (!flag)
			{
				Debug.LogError("Some validation of the source materials failed and the atlas was not generated.");
				return;
			}
			int num = 2;
			int num2 = 8;
			bool linear = MBVersion.GetProjectColorSpace() == ColorSpace.Linear;
			Texture2D[] array = new Texture2D[this.sourceMaterials.Length];
			StringBuilder stringBuilder = new StringBuilder("Collecting color tints from source materials: \n");
			Color[] array2 = new Color[num2 * num2];
			for (int j = 0; j < this.sourceMaterials.Length; j++)
			{
				Material material = this.sourceMaterials[j];
				Color color = material.GetColor(this.colorTintPropertyName);
				StringBuilder stringBuilder2 = stringBuilder;
				string[] array3 = new string[5];
				array3[0] = "Material: ";
				array3[1] = material.name;
				array3[2] = " - colorTint: ";
				int num3 = 3;
				Color color2 = color;
				array3[num3] = color2.ToString();
				array3[4] = "\n";
				stringBuilder2.Append(string.Concat(array3));
				Texture2D texture2D = array[j] = new Texture2D(num2, num2, TextureFormat.ARGB32, false, linear);
				for (int k = 0; k < array2.Length; k++)
				{
					array2[k] = color;
				}
				texture2D.SetPixels(array2);
				texture2D.Apply();
			}
			Debug.Log(stringBuilder);
			Debug.Log("Calculating the atlas dimensions");
			int num4 = (int)Mathf.Ceil(Mathf.Sqrt((float)this.sourceMaterials.Length)) * num2;
			Debug.Log("Creating atlas for " + this.sourceMaterials.Length.ToString() + " textures");
			this.atlasTexture = new Texture2D(num4, num4, TextureFormat.ARGB32, false, linear);
			Rect[] array4 = this.atlasTexture.PackTextures(array, 0, num4);
			Debug.Log(string.Concat(new string[]
			{
				"Atlas size: w:",
				this.atlasTexture.width.ToString(),
				"  h:",
				this.atlasTexture.height.ToString(),
				"  numTex: ",
				array.Length.ToString(),
				" (",
				num2.ToString(),
				"x",
				num2.ToString(),
				" each)"
			}));
			this.atlasTexture.filterMode = FilterMode.Point;
			this.atlasMaterial = new Material(Shader.Find(this.shaderName));
			this.atlasMaterial.SetTexture(this.albedoTexturePropertyName, this.atlasTexture);
			this.atlasMaterial.SetColor(this.colorTintPropertyName, Color.white);
			StringBuilder stringBuilder3 = new StringBuilder("Creating MB2_TextureBakeResult for storing atlas rectangle information: \n");
			for (int l = 0; l < array.Length; l++)
			{
				StringBuilder stringBuilder4 = stringBuilder3;
				string[] array5 = new string[5];
				array5[0] = "Material: ";
				array5[1] = this.sourceMaterials[l].name;
				array5[2] = " will use rectangle: ";
				int num5 = 3;
				Rect rect = array4[l];
				array5[num5] = rect.ToString();
				array5[4] = "\n";
				stringBuilder4.Append(string.Concat(array5));
			}
			Debug.Log(stringBuilder3);
			Debug.Log("Creating and setting up MB2_TextureBakeResults");
			this.materialBakeResult = ScriptableObject.CreateInstance<MB2_TextureBakeResults>();
			this.materialBakeResult.resultType = MB2_TextureBakeResults.ResultType.atlas;
			this.materialBakeResult.materialsAndUVRects = new MB_MaterialAndUVRect[array.Length];
			float num6 = (float)num / (float)this.atlasTexture.width;
			float num7 = (float)num / (float)this.atlasTexture.height;
			for (int m = 0; m < array.Length; m++)
			{
				Rect destRect = array4[m];
				destRect.x += num6;
				destRect.y += num7;
				destRect.width -= 2f * num6;
				destRect.height -= 2f * num7;
				bool allPropsUseSameTiling = true;
				this.materialBakeResult.materialsAndUVRects[m] = new MB_MaterialAndUVRect(this.sourceMaterials[m], destRect, allPropsUseSameTiling, new Rect(0f, 0f, 1f, 1f), new Rect(0f, 0f, 1f, 1f), new Rect(0f, 0f, 0f, 0f), MB_TextureTilingTreatment.none, this.sourceMaterials[m].name);
			}
			this.materialBakeResult.resultMaterials = new MB_MultiMaterial[1];
			this.materialBakeResult.resultMaterials[0] = new MB_MultiMaterial();
			this.materialBakeResult.resultMaterials[0].combinedMaterial = this.atlasMaterial;
			this.materialBakeResult.resultMaterials[0].considerMeshUVs = false;
			List<Material> list = new List<Material>();
			list.AddRange(this.sourceMaterials);
			this.materialBakeResult.resultMaterials[0].sourceMaterials = list;
		}

		[Header("Hack Atlas Generation")]
		public string colorTintPropertyName;

		public string albedoTexturePropertyName;

		public string shaderName;

		public Material[] sourceMaterials;

		[Space(20f)]
		[Header("Generated Output")]
		public MB2_TextureBakeResults materialBakeResult;

		public Material atlasMaterial;

		public Texture2D atlasTexture;
	}
}
