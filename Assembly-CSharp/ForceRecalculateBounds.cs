using System;
using UnityEngine;

public class ForceRecalculateBounds : MonoBehaviourTick
{
	private void Awake()
	{
		this.skinnedMesh = base.GetComponent<SkinnedMeshRenderer>();
	}

	public override void Tick()
	{
		if (this.skinnedMesh == null)
		{
			return;
		}
		this.skinnedMesh.bounds = new Bounds(base.transform.position, Vector3.one * 1000f);
	}

	private SkinnedMeshRenderer skinnedMesh;
}
