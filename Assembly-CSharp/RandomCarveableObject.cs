using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using Voxels;

public class RandomCarveableObject : MonoBehaviour
{
	private void Start()
	{
		if (this.world == null)
		{
			this.world = base.GetComponentInParent<VoxelWorld>();
		}
		this.world.UpdateWorld = false;
		for (int i = this.spawnPoint.childCount - 1; i >= 0; i--)
		{
			JamUtil.Destroy(this.spawnPoint.GetChild(i).gameObject);
		}
		this.SpawnNewCarveable();
	}

	private void SelectCarveable()
	{
		for (int i = this.spawnPoint.childCount - 1; i >= 0; i--)
		{
			JamUtil.Destroy(this.spawnPoint.GetChild(i).gameObject);
		}
		this._carveable = Object.Instantiate<GameObject>(this.prefabs[UnityEngine.Random.Range(0, this.prefabs.Length)], this.spawnPoint.position, this.spawnPoint.rotation, this.spawnPoint);
		this._carveable.transform.localScale = Vector3.one;
	}

	private void ClearBounds()
	{
		this.SetBoundsDensity(0);
	}

	private void FillBounds()
	{
		this.SetBoundsDensity(byte.MaxValue);
	}

	private void SetBoundsDensity(byte density)
	{
		this.world.SetVoxels(this._voxels, density, this.materialId);
	}

	private void CollectVoxelSet()
	{
		Bounds localBounds = this._carveable.GetComponentInChildren<Renderer>().localBounds;
		Vector3 min = localBounds.min;
		Vector3 max = localBounds.max;
		HashSet<int3> hashSet = new HashSet<int3>();
		for (float num = min.x; num <= max.x; num += 0.1f)
		{
			for (float num2 = min.y; num2 <= max.y; num2 += 0.1f)
			{
				for (float num3 = min.z; num3 <= max.z; num3 += 0.1f)
				{
					hashSet.Add(this.<CollectVoxelSet>g__GetVoxel|12_0(num, num2, num3));
				}
			}
		}
		this._voxels = hashSet.ToArray<int3>();
		this._voxelBounds = VoxelWorld.GetBoundsFor(this._voxels);
	}

	public void SpawnNewCarveable()
	{
		RandomCarveableObject.<SpawnNewCarveable>d__13 <SpawnNewCarveable>d__;
		<SpawnNewCarveable>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<SpawnNewCarveable>d__.<>4__this = this;
		<SpawnNewCarveable>d__.<>1__state = -1;
		<SpawnNewCarveable>d__.<>t__builder.Start<RandomCarveableObject.<SpawnNewCarveable>d__13>(ref <SpawnNewCarveable>d__);
	}

	[CompilerGenerated]
	private int3 <CollectVoxelSet>g__GetVoxel|12_0(float x, float y, float z)
	{
		return this.world.GetVoxelForWorldPosition(this._carveable.transform.TransformPoint(new Vector3(x, y, z)));
	}

	[SerializeField]
	private Transform spawnPoint;

	[SerializeField]
	private GameObject[] prefabs;

	[SerializeField]
	private byte materialId;

	[SerializeField]
	private VoxelWorld world;

	private GameObject _carveable;

	private int3[] _voxels;

	private UnityEngine.BoundsInt _voxelBounds;
}
