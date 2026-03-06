using System;
using DigitalOpus.MB.Core;
using UnityEngine;

public class BakeTexturesAtRuntime : MonoBehaviour
{
	public string GetShaderNameForPipeline()
	{
		if (MBVersion.DetectPipeline() == MBVersion.PipelineType.URP)
		{
			return "Universal Render Pipeline/Lit";
		}
		if (MBVersion.DetectPipeline() == MBVersion.PipelineType.HDRP)
		{
			return "HDRP/Lit";
		}
		return "Diffuse";
	}

	private void OnGUI()
	{
		GUILayout.Label("Time to bake textures: " + this.elapsedTime.ToString(), Array.Empty<GUILayoutOption>());
		if (GUILayout.Button("Combine textures & build combined mesh all at once", Array.Empty<GUILayoutOption>()))
		{
			MB3_MeshBaker componentInChildren = this.target.GetComponentInChildren<MB3_MeshBaker>();
			MB3_TextureBaker component = this.target.GetComponent<MB3_TextureBaker>();
			((MB3_MeshCombinerSingle)componentInChildren.meshCombiner).SetMesh(null);
			component.textureBakeResults = ScriptableObject.CreateInstance<MB2_TextureBakeResults>();
			component.resultMaterial = new Material(Shader.Find(this.GetShaderNameForPipeline()));
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			component.CreateAtlases();
			this.elapsedTime = Time.realtimeSinceStartup - realtimeSinceStartup;
			componentInChildren.ClearMesh();
			componentInChildren.textureBakeResults = component.textureBakeResults;
			if (componentInChildren.AddDeleteGameObjects(component.GetObjectsToCombine().ToArray(), null, true))
			{
				componentInChildren.Apply(null);
			}
		}
		if (GUILayout.Button("Combine textures & build combined mesh using coroutine", Array.Empty<GUILayoutOption>()))
		{
			Debug.Log("Starting to bake textures on frame " + Time.frameCount.ToString());
			MB3_MeshBakerCommon componentInChildren2 = this.target.GetComponentInChildren<MB3_MeshBaker>();
			MB3_TextureBaker component2 = this.target.GetComponent<MB3_TextureBaker>();
			((MB3_MeshCombinerSingle)componentInChildren2.meshCombiner).SetMesh(null);
			component2.textureBakeResults = ScriptableObject.CreateInstance<MB2_TextureBakeResults>();
			component2.resultMaterial = new Material(Shader.Find(this.GetShaderNameForPipeline()));
			component2.onBuiltAtlasesSuccess = new MB3_TextureBaker.OnCombinedTexturesCoroutineSuccess(this.OnBuiltAtlasesSuccess);
			base.StartCoroutine(component2.CreateAtlasesCoroutine(null, this.result, false, null, 0.01f));
		}
	}

	private void OnBuiltAtlasesSuccess()
	{
		Debug.Log("Calling success callback. baking meshes");
		MB3_MeshBaker componentInChildren = this.target.GetComponentInChildren<MB3_MeshBaker>();
		MB3_TextureBaker component = this.target.GetComponent<MB3_TextureBaker>();
		if (this.result.isFinished && this.result.success)
		{
			componentInChildren.ClearMesh();
			componentInChildren.textureBakeResults = component.textureBakeResults;
			if (componentInChildren.AddDeleteGameObjects(component.GetObjectsToCombine().ToArray(), null, true))
			{
				componentInChildren.Apply(null);
			}
		}
		Debug.Log("Completed baking textures on frame " + Time.frameCount.ToString());
	}

	public GameObject target;

	private float elapsedTime;

	private MB3_TextureCombiner.CreateAtlasesCoroutineResult result = new MB3_TextureCombiner.CreateAtlasesCoroutineResult();
}
