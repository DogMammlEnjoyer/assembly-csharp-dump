using System;
using System.Collections.Generic;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;

public class BuilderShelf : MonoBehaviour
{
	public void Init()
	{
		this.shelfSlot = 0;
		this.buildPieceSpawnIndex = 0;
		this.spawnCount = 0;
		this.count = 0;
		this.spawnCosts = new List<BuilderResources>(this.buildPieceSpawns.Count);
		for (int i = 0; i < this.buildPieceSpawns.Count; i++)
		{
			this.count += this.buildPieceSpawns[i].count;
			BuilderPiece component = this.buildPieceSpawns[i].buildPiecePrefab.GetComponent<BuilderPiece>();
			this.spawnCosts.Add(component.cost);
		}
	}

	public bool HasOpenSlot()
	{
		return this.shelfSlot < this.count;
	}

	public void BuildNextPiece(BuilderTable table)
	{
		if (!this.HasOpenSlot())
		{
			return;
		}
		BuilderShelf.BuildPieceSpawn buildPieceSpawn = this.buildPieceSpawns[this.buildPieceSpawnIndex];
		BuilderResources resources = this.spawnCosts[this.buildPieceSpawnIndex];
		while (!table.HasEnoughUnreservedResources(resources) && this.buildPieceSpawnIndex < this.buildPieceSpawns.Count - 1)
		{
			int num = buildPieceSpawn.count - this.spawnCount;
			this.shelfSlot += num;
			this.spawnCount = 0;
			this.buildPieceSpawnIndex++;
			buildPieceSpawn = this.buildPieceSpawns[this.buildPieceSpawnIndex];
			resources = this.spawnCosts[this.buildPieceSpawnIndex];
		}
		if (!table.HasEnoughUnreservedResources(resources))
		{
			int num2 = buildPieceSpawn.count - this.spawnCount;
			this.shelfSlot += num2;
			this.spawnCount = 0;
			return;
		}
		int staticHash = buildPieceSpawn.buildPiecePrefab.name.GetStaticHash();
		int materialType = string.IsNullOrEmpty(buildPieceSpawn.materialID) ? -1 : buildPieceSpawn.materialID.GetHashCode();
		Vector3 position;
		Quaternion rotation;
		this.GetSpawnLocation(this.shelfSlot, buildPieceSpawn, out position, out rotation);
		int pieceId = table.CreatePieceId();
		table.CreatePiece(staticHash, pieceId, position, rotation, materialType, BuilderPiece.State.OnShelf, PhotonNetwork.LocalPlayer);
		this.spawnCount++;
		this.shelfSlot++;
		if (this.spawnCount >= buildPieceSpawn.count)
		{
			this.buildPieceSpawnIndex++;
			this.spawnCount = 0;
		}
	}

	public void InitCount()
	{
		this.count = 0;
		for (int i = 0; i < this.buildPieceSpawns.Count; i++)
		{
			this.count += this.buildPieceSpawns[i].count;
		}
	}

	public void BuildItems(BuilderTable table)
	{
		int num = 0;
		this.InitCount();
		for (int i = 0; i < this.buildPieceSpawns.Count; i++)
		{
			BuilderShelf.BuildPieceSpawn buildPieceSpawn = this.buildPieceSpawns[i];
			if (buildPieceSpawn != null && buildPieceSpawn.count != 0)
			{
				int staticHash = buildPieceSpawn.buildPiecePrefab.name.GetStaticHash();
				int materialType = string.IsNullOrEmpty(buildPieceSpawn.materialID) ? -1 : buildPieceSpawn.materialID.GetHashCode();
				int num2 = 0;
				while (num2 < buildPieceSpawn.count && num < this.count)
				{
					Vector3 position;
					Quaternion rotation;
					this.GetSpawnLocation(num, buildPieceSpawn, out position, out rotation);
					int pieceId = table.CreatePieceId();
					table.CreatePiece(staticHash, pieceId, position, rotation, materialType, BuilderPiece.State.OnShelf, PhotonNetwork.LocalPlayer);
					num++;
					num2++;
				}
			}
		}
	}

	public void GetSpawnLocation(int slot, BuilderShelf.BuildPieceSpawn spawn, out Vector3 spawnPosition, out Quaternion spawnRotation)
	{
		if (this.center == null)
		{
			this.center = base.transform;
		}
		Vector3 b = Vector3.zero;
		Vector3 euler = Vector3.zero;
		BuilderPiece component = spawn.buildPiecePrefab.GetComponent<BuilderPiece>();
		if (component != null)
		{
			b = component.desiredShelfOffset;
			euler = component.desiredShelfRotationOffset;
		}
		spawnRotation = this.center.rotation * Quaternion.Euler(euler);
		float d = (float)slot * this.separation - (float)(this.count - 1) * this.separation / 2f;
		spawnPosition = this.center.position + this.center.rotation * (spawn.localAxis * d + b);
	}

	private int count;

	public float separation;

	public Transform center;

	public List<BuilderShelf.BuildPieceSpawn> buildPieceSpawns;

	private List<BuilderResources> spawnCosts;

	private int shelfSlot;

	private int buildPieceSpawnIndex;

	private int spawnCount;

	[Serializable]
	public class BuildPieceSpawn
	{
		public GameObject buildPiecePrefab;

		public string materialID;

		public int count = 1;

		public Vector3 localAxis = Vector3.right;

		[Tooltip("Optional Editor Visual")]
		public Mesh previewMesh;
	}
}
