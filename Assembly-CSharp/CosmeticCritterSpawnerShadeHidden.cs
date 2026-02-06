using System;
using UnityEngine;

public class CosmeticCritterSpawnerShadeHidden : CosmeticCritterSpawnerTimed
{
	public override void SetRandomVariables(CosmeticCritter critter)
	{
		float y = Random.Range(this.orbitHeightOffsetMinMax.x, this.orbitHeightOffsetMinMax.y);
		float radius = Random.Range(this.orbitRadiusMinMax.x, this.orbitRadiusMinMax.y);
		(critter as CosmeticCritterShadeHidden).SetCenterAndRadius(base.transform.position + new Vector3(0f, y, 0f), radius);
	}

	[Tooltip("Add between X and Y extra height to the base orbit height.")]
	[SerializeField]
	private Vector2 orbitHeightOffsetMinMax = new Vector2(0f, 2f);

	[Tooltip("Orbit between X (green sphere) and Y (red sphere) units away from this spawner's position when first spawned.")]
	[SerializeField]
	private Vector2 orbitRadiusMinMax = new Vector2(5f, 10f);
}
