using System;
using UnityEngine;

public class LineRendererUpdateTarget : MonoBehaviourPostTick
{
	public override void PostTick()
	{
		if (this.lineRenderer == null || this.targetTransform == null || this.lineRenderer.positionCount != 2)
		{
			return;
		}
		if (!this.targetTransform.gameObject.activeSelf)
		{
			this.lineRenderer.enabled = false;
			return;
		}
		this.lineRenderer.enabled = true;
		this.lineRenderer.SetPosition(0, base.transform.position);
		this.lineRenderer.SetPosition(1, this.targetTransform.position);
	}

	private void Awake()
	{
		this.lineRenderer = base.GetComponent<LineRenderer>();
		this.lineRenderer.useWorldSpace = true;
	}

	private LineRenderer lineRenderer;

	public Transform targetTransform;
}
