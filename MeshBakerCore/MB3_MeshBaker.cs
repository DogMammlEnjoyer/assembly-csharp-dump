using System;
using System.Text;
using DigitalOpus.MB.Core;
using UnityEngine;

public class MB3_MeshBaker : MB3_MeshBakerCommon
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
		double num11 = 0.0;
		double num12 = 0.0;
		double num13 = 0.0;
		MB3_MeshCombinerSingle meshCombiner = this._meshCombiner;
		num += meshCombiner.db_showHideGameObjects.Elapsed.TotalSeconds;
		num2 += meshCombiner.db_addDeleteGameObjects.Elapsed.TotalSeconds;
		num7 += meshCombiner.db_addDeleteGameObjects_CollectMeshData.Elapsed.TotalSeconds;
		num8 += meshCombiner.db_addDeleteGameObjects_CollectMeshData_a.Elapsed.TotalSeconds;
		num9 += meshCombiner.db_addDeleteGameObjects_CollectMeshData_b.Elapsed.TotalSeconds;
		num10 += meshCombiner.db_addDeleteGameObjects_CollectMeshData_c.Elapsed.TotalSeconds;
		num3 += meshCombiner.db_addDeleteGameObjects_InitFromMeshCombiner.Elapsed.TotalSeconds;
		num4 += meshCombiner.db_addDeleteGameObjects_Init.Elapsed.TotalSeconds;
		num5 += meshCombiner.db_addDeleteGameObjects_CopyArraysFromPreviousBakeBuffersToNewBuffers.Elapsed.TotalSeconds;
		num6 += meshCombiner.db_addDeleteGameObjects_CopyFromDGOMeshToBuffers.Elapsed.TotalSeconds;
		num11 += meshCombiner.db_apply.Elapsed.TotalSeconds;
		num12 += meshCombiner.db_applyShowHide.Elapsed.TotalSeconds;
		num13 += meshCombiner.db_updateGameObjects.Elapsed.TotalSeconds;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Timings  " + ((this._meshCombiner.settings.meshAPI == MB_MeshCombineAPIType.betaNativeArrayAPI) ? "  newMeshAPI " : " oldMeshAPI"));
		stringBuilder.AppendLine("db_showHideGameObjects\t" + num.ToString());
		stringBuilder.AppendLine("db_addDeleteGameObjects\t" + num2.ToString());
		stringBuilder.AppendLine("\t\tdb_addDeleteGameObjects_CollectMeshData\t" + num7.ToString());
		stringBuilder.AppendLine("\t\tdb_addDeleteGameObjects_CollectMeshDataA\t\t" + num8.ToString());
		stringBuilder.AppendLine("\t\tdb_addDeleteGameObjects_CollectMeshDataB\t\t" + num9.ToString());
		stringBuilder.AppendLine("\t\tdb_addDeleteGameObjects_CollectMeshDataC\t\t" + num10.ToString());
		stringBuilder.AppendLine("\t\tdb_addDeleteGameObjects_InitFromMeshCombiner\t" + num3.ToString());
		stringBuilder.AppendLine("\t\tdb_addDeleteGameObjects_Init\t" + num4.ToString());
		stringBuilder.AppendLine("\t\tdb_addDeleteGameObjects_CopyArraysFromPreviousBakeBuffersToNewBuffers\t" + num5.ToString());
		stringBuilder.AppendLine("\t\tdb_addDeleteGameObjects_CopyFromDGOMeshToBuffers\t" + num6.ToString());
		stringBuilder.AppendLine("\t\tdb_addDeleteGameObjects_CollectMeshData  tdb_addDeleteGameObjects_CollectMeshData ");
		stringBuilder.AppendLine("db_apply\t" + num11.ToString());
		stringBuilder.AppendLine("db_applyShowHide\t" + num12.ToString());
		stringBuilder.AppendLine("db_updateGameObjects\t" + num13.ToString());
		Debug.Log(stringBuilder.ToString());
	}

	public override MB3_MeshCombiner meshCombiner
	{
		get
		{
			return this._meshCombiner;
		}
	}

	public void BuildSceneMeshObject()
	{
		this._meshCombiner.BuildSceneMeshObject(null, false);
	}

	public virtual bool ShowHide(GameObject[] gos, GameObject[] deleteGOs)
	{
		return this._meshCombiner.ShowHideGameObjects(gos, deleteGOs);
	}

	public virtual void ApplyShowHide()
	{
		this._meshCombiner.ApplyShowHide();
	}

	public override bool AddDeleteGameObjects(GameObject[] gos, GameObject[] deleteGOs, bool disableRendererInSource)
	{
		base.UpgradeToCurrentVersionIfNecessary();
		this._meshCombiner.name = base.name + "-mesh";
		return this._meshCombiner.AddDeleteGameObjects(gos, deleteGOs, disableRendererInSource);
	}

	public override bool AddDeleteGameObjectsByID(GameObject[] gos, int[] deleteGOinstanceIDs, bool disableRendererInSource)
	{
		base.UpgradeToCurrentVersionIfNecessary();
		this._meshCombiner.name = base.name + "-mesh";
		return this._meshCombiner.AddDeleteGameObjectsByID(gos, deleteGOinstanceIDs, disableRendererInSource);
	}

	public void OnDestroy()
	{
		if (this.meshCombiner != null)
		{
			this.meshCombiner.Dispose();
		}
	}

	[SerializeField]
	protected MB3_MeshCombinerSingle _meshCombiner = new MB3_MeshCombinerSingle();
}
