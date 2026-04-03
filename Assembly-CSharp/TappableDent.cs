using System;
using UnityEngine;

public class TappableDent : Tappable
{
	private void Start()
	{
		if (this.parent == null)
		{
			this.parent = base.gameObject;
		}
		this.offsetPerTap = base.transform.parent.InverseTransformVector(base.transform.TransformVector(this.finalLocalOffset / (float)this.numTapsToDestroy));
		this.scaleOffsetPerTap = (this.finalLocalScale - base.transform.localScale) / (float)this.numTapsToDestroy;
	}

	public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
	{
		this.numTapsSoFar++;
		if (this.numTapsSoFar >= this.numTapsToDestroy)
		{
			this.parent.SetActive(false);
			return;
		}
		base.transform.localPosition += this.offsetPerTap;
		base.transform.localScale += this.scaleOffsetPerTap;
	}

	[SerializeField]
	private int numTapsToDestroy = 3;

	[SerializeField]
	private Vector3 finalLocalOffset;

	[SerializeField]
	private Vector3 finalLocalScale;

	[SerializeField]
	private GameObject parent;

	private int numTapsSoFar;

	private Vector3 offsetPerTap;

	private Vector3 scaleOffsetPerTap;
}
