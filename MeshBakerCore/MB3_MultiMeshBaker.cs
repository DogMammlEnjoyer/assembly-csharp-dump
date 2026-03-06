using System;
using System.Text;
using DigitalOpus.MB.Core;
using UnityEngine;

public class MB3_MultiMeshBaker : MB3_MeshBakerCommon
{
	public void PrintTimings()
	{
		double num = 0.0;
		double num2 = 0.0;
		double num3 = 0.0;
		double num4 = 0.0;
		double num5 = 0.0;
		double num6 = 0.0;
		double num7 = 0.0;
		double num8 = 0.0;
		double num9 = 0.0;
		double num10 = 0.0;
		for (int i = 0; i < this._meshCombiner.meshCombiners.Count; i++)
		{
			MB3_MeshCombinerSingle combinedMesh = this._meshCombiner.meshCombiners[i].combinedMesh;
			num += combinedMesh.db_showHideGameObjects.Elapsed.TotalSeconds;
			num2 += combinedMesh.db_addDeleteGameObjects.Elapsed.TotalSeconds;
			num7 += combinedMesh.db_addDeleteGameObjects_CollectMeshData.Elapsed.TotalSeconds;
			num3 += combinedMesh.db_addDeleteGameObjects_InitFromMeshCombiner.Elapsed.TotalSeconds;
			num4 += combinedMesh.db_addDeleteGameObjects_Init.Elapsed.TotalSeconds;
			num5 += combinedMesh.db_addDeleteGameObjects_CopyArraysFromPreviousBakeBuffersToNewBuffers.Elapsed.TotalSeconds;
			num6 += combinedMesh.db_addDeleteGameObjects_CopyFromDGOMeshToBuffers.Elapsed.TotalSeconds;
			num8 += combinedMesh.db_apply.Elapsed.TotalSeconds;
			num9 += combinedMesh.db_applyShowHide.Elapsed.TotalSeconds;
			num10 += combinedMesh.db_updateGameObjects.Elapsed.TotalSeconds;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Timings  " + ((this._meshCombiner.settings.meshAPI == MB_MeshCombineAPIType.betaNativeArrayAPI) ? "  newMeshAPI " : " oldMeshAPI"));
		stringBuilder.AppendLine("db_showHideGameObjects\t" + num.ToString());
		stringBuilder.AppendLine("db_addDeleteGameObjects\t" + num2.ToString());
		stringBuilder.AppendLine("\t\tdb_addDeleteGameObjects_CollectMeshData\t" + num7.ToString());
		stringBuilder.AppendLine("\t\tdb_addDeleteGameObjects_InitFromMeshCombiner\t" + num3.ToString());
		stringBuilder.AppendLine("\t\tdb_addDeleteGameObjects_Init\t" + num4.ToString());
		stringBuilder.AppendLine("\t\tdb_addDeleteGameObjects_CopyArraysFromPreviousBakeBuffersToNewBuffers\t" + num5.ToString());
		stringBuilder.AppendLine("\t\tdb_addDeleteGameObjects_CopyFromDGOMeshToBuffers\t" + num6.ToString());
		stringBuilder.AppendLine("\t\tdb_addDeleteGameObjects_CollectMeshData  tdb_addDeleteGameObjects_CollectMeshData ");
		stringBuilder.AppendLine("db_apply\t" + num8.ToString());
		stringBuilder.AppendLine("db_applyShowHide\t" + num9.ToString());
		stringBuilder.AppendLine("db_updateGameObjects\t" + num10.ToString());
		Debug.Log(stringBuilder.ToString());
	}

	public override MB3_MeshCombiner meshCombiner
	{
		get
		{
			return this._meshCombiner;
		}
	}

	public override bool AddDeleteGameObjects(GameObject[] gos, GameObject[] deleteGOs, bool disableRendererInSource)
	{
		base.UpgradeToCurrentVersionIfNecessary();
		if (this._meshCombiner.resultSceneObject == null)
		{
			this._meshCombiner.resultSceneObject = new GameObject("CombinedMesh-" + base.name);
		}
		this.meshCombiner.name = base.name + "-mesh";
		return this._meshCombiner.AddDeleteGameObjects(gos, deleteGOs, disableRendererInSource);
	}

	public override bool AddDeleteGameObjectsByID(GameObject[] gos, int[] deleteGOs, bool disableRendererInSource)
	{
		base.UpgradeToCurrentVersionIfNecessary();
		if (this._meshCombiner.resultSceneObject == null)
		{
			this._meshCombiner.resultSceneObject = new GameObject("CombinedMesh-" + base.name);
		}
		this.meshCombiner.name = base.name + "-mesh";
		return this._meshCombiner.AddDeleteGameObjectsByID(gos, deleteGOs, disableRendererInSource);
	}

	public void OnDestroy()
	{
		if (this._meshCombiner != null)
		{
			this._meshCombiner.Dispose();
		}
	}

	[SerializeField]
	protected MB3_MultiMeshCombiner _meshCombiner = new MB3_MultiMeshCombiner();
}
