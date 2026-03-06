using System;
using DigitalOpus.MB.Core;
using UnityEngine;

namespace MeshBaker_Examples_2017.HackTextureAtlas
{
	public class MB_CustomizeCharacterGUI : MonoBehaviour
	{
		private void Start()
		{
			switch (MBVersion.DetectPipeline())
			{
			case MBVersion.PipelineType.Default:
				this.colorTintPropertyName = "_Color";
				this.albedoTexturePropertyName = "_MainTex";
				this.shaderName = "Standard";
				break;
			case MBVersion.PipelineType.URP:
				this.colorTintPropertyName = "_BaseColor";
				this.albedoTexturePropertyName = "_BaseMap";
				this.shaderName = "Universal Render Pipeline/Lit";
				break;
			case MBVersion.PipelineType.HDRP:
				this.colorTintPropertyName = "_BaseColor";
				this.albedoTexturePropertyName = "_BaseColorMap";
				this.shaderName = "HDRP/Lit";
				break;
			default:
				Debug.LogError("Unknown pipeline type");
				break;
			}
			this.textureBakerQuickHack = base.GetComponent<MB_TextureBakerQuickHack>();
			Debug.Log("Creating atlas using TextureBakerQuickHack method");
			this.textureBakerQuickHack.colorTintPropertyName = this.colorTintPropertyName;
			this.textureBakerQuickHack.albedoTexturePropertyName = this.albedoTexturePropertyName;
			this.textureBakerQuickHack.shaderName = this.shaderName;
			this.textureBakerQuickHack.CreateAtlas(this.sourceMaterials);
			Debug.Log("Baking MeshBaker using TextureBakerQuickHack output");
			this.BakeMeshBaker();
		}

		private void OnGUI()
		{
			object obj = "This example demonstrates how to create\r\nsolid-color-rectangle texture atlases at\r\nruntime for character customization. This\r\nis MUCH faster and more flexible than using\r\nthe full TextureBaker. These atlases can be\r\nused at runtime with a Mesh Baker.\r\n";
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(obj.ToString(), Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("Hoof Color", Array.Empty<GUILayoutOption>());
			if (GUILayout.Button("Red", Array.Empty<GUILayoutOption>()))
			{
				this.SetColorInMaterialBakeResultAndBakeMeshBaker(this.sourceMaterials[0], Color.red);
			}
			if (GUILayout.Button("Green", Array.Empty<GUILayoutOption>()))
			{
				this.SetColorInMaterialBakeResultAndBakeMeshBaker(this.sourceMaterials[0], Color.green);
			}
			if (GUILayout.Button("Blue", Array.Empty<GUILayoutOption>()))
			{
				this.SetColorInMaterialBakeResultAndBakeMeshBaker(this.sourceMaterials[0], Color.blue);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("Body Color", Array.Empty<GUILayoutOption>());
			if (GUILayout.Button("Red", Array.Empty<GUILayoutOption>()))
			{
				this.SetColorInMaterialBakeResultAndBakeMeshBaker(this.sourceMaterials[1], Color.red);
			}
			if (GUILayout.Button("Green", Array.Empty<GUILayoutOption>()))
			{
				this.SetColorInMaterialBakeResultAndBakeMeshBaker(this.sourceMaterials[1], Color.green);
			}
			if (GUILayout.Button("Blue", Array.Empty<GUILayoutOption>()))
			{
				this.SetColorInMaterialBakeResultAndBakeMeshBaker(this.sourceMaterials[1], Color.blue);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("Horns Color", Array.Empty<GUILayoutOption>());
			if (GUILayout.Button("Red", Array.Empty<GUILayoutOption>()))
			{
				this.SetColorInMaterialBakeResultAndBakeMeshBaker(this.sourceMaterials[2], Color.red);
			}
			if (GUILayout.Button("Green", Array.Empty<GUILayoutOption>()))
			{
				this.SetColorInMaterialBakeResultAndBakeMeshBaker(this.sourceMaterials[2], Color.green);
			}
			if (GUILayout.Button("Blue", Array.Empty<GUILayoutOption>()))
			{
				this.SetColorInMaterialBakeResultAndBakeMeshBaker(this.sourceMaterials[2], Color.blue);
			}
			GUILayout.EndHorizontal();
		}

		private void SetColorInMaterialBakeResultAndBakeMeshBaker(Material bodyPartMaterial, Color color)
		{
			Debug.Log("Changing color of material " + ((bodyPartMaterial != null) ? bodyPartMaterial.ToString() : null) + " used in atlas generation");
			bodyPartMaterial.SetColor(this.colorTintPropertyName, color);
			Debug.Log("Creating atlas using TextureBakerQuickHack method");
			this.textureBakerQuickHack.CreateAtlas(this.sourceMaterials);
			Debug.Log("Baking MeshBaker using TextureBakerQuickHack output");
			this.BakeMeshBaker();
		}

		[ContextMenu("Bake Mesh Baker")]
		private void BakeMeshBaker()
		{
			this.targetMeshBaker.textureBakeResults = this.textureBakerQuickHack.materialBakeResult;
			this.targetMeshBaker.ClearMesh();
			if (this.targetMeshBaker.AddDeleteGameObjects(this.objectsToBeCombined, null, true))
			{
				this.targetMeshBaker.Apply(null);
			}
		}

		public Material[] sourceMaterials;

		public GameObject[] objectsToBeCombined;

		[Header("Mesh Baker Config")]
		public MB3_MeshBaker targetMeshBaker;

		private MB_TextureBakerQuickHack textureBakerQuickHack;

		private string colorTintPropertyName;

		private string albedoTexturePropertyName;

		private string shaderName;
	}
}
