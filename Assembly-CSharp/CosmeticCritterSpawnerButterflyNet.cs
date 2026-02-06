using System;
using UnityEngine;

public class CosmeticCritterSpawnerButterflyNet : CosmeticCritterSpawnerTimed
{
	public override void SetRandomVariables(CosmeticCritter critter)
	{
		Vector3 startPos = base.transform.position + Random.onUnitSphere * this.spawnRadius;
		(critter as CosmeticCritterButterfly).SetStartPos(startPos);
	}

	[Tooltip("Spawn a butterfly on the surface of a sphere with this radius, and with a center on this object.")]
	[SerializeField]
	private float spawnRadius = 1f;
}
