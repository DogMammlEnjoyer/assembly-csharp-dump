using System;
using UnityEngine;

[RequireComponent(typeof(SIGadgetBlasterProjectile))]
public class SIGadgetProjectileStretchVisuals : MonoBehaviourTick
{
	public new void OnEnable()
	{
		base.OnEnable();
		this.projectile = base.GetComponent<SIGadgetBlasterProjectile>();
		this.totalLength = (this.frontStretch.position - this.rearStretch.position).magnitude;
		this.distancePerFrame = this.projectile.startingVelocity * Time.fixedDeltaTime;
		this.maxStretchRatio = this.distancePerFrame / this.totalLength * this.framesPerPosition;
		this.timeSpawned = Time.time;
		this.maxSizeReached = false;
		this.baseVisuals.transform.localPosition = new Vector3(0f, 0f, 0f);
		this.baseVisuals.transform.localScale = new Vector3(1f, 1f, 1f);
		this.frontDistance = (this.frontStretch.position - base.transform.position).magnitude;
	}

	public override void Tick()
	{
		if (this.maxSizeReached)
		{
			return;
		}
		float num = (Time.time - this.timeSpawned) * this.projectile.startingVelocity / this.totalLength / 2f + 1f;
		if (num >= this.maxStretchRatio)
		{
			num = this.maxStretchRatio;
			this.maxSizeReached = true;
		}
		this.baseVisuals.transform.localPosition = new Vector3(0f, 0f, -(num - 1f) * this.frontDistance);
		this.baseVisuals.transform.localScale = new Vector3(1f, 1f, num);
	}

	private SIGadgetBlasterProjectile projectile;

	public GameObject baseVisuals;

	public Transform frontStretch;

	public Transform rearStretch;

	public float framesPerPosition;

	private float totalLength;

	private float distancePerFrame;

	private float maxStretchRatio;

	private bool maxSizeReached;

	private float frontDistance;

	private float timeSpawned;
}
