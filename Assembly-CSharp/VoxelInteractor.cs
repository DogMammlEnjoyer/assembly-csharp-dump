using System;
using UnityEngine;
using Voxels;

public class VoxelInteractor : MonoBehaviour
{
	public bool ApplyVoxelAction(Collision collision)
	{
		ChunkComponent component = collision.gameObject.GetComponent<ChunkComponent>();
		if (component)
		{
			component.World.Mine(collision, this.action);
		}
		return component;
	}

	public bool ApplyVoxelAction(RaycastHit hit)
	{
		ChunkComponent component = hit.collider.GetComponent<ChunkComponent>();
		if (component)
		{
			component.World.Mine(hit, this.action);
		}
		return component;
	}

	public VoxelAction action = new VoxelAction
	{
		strength = 1f,
		radius = 0.5f,
		operation = OperationType.Subtract
	};
}
