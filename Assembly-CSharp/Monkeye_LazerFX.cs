using System;
using UnityEngine;

public class Monkeye_LazerFX : MonoBehaviour
{
	private void Awake()
	{
		base.enabled = false;
		foreach (LineRenderer lineRenderer in this.lines)
		{
			lineRenderer.positionCount = 2;
			lineRenderer.enabled = false;
		}
		if (this.targetFx != null)
		{
			this.targetFx.SetActive(false);
		}
	}

	public void EnableLazer(Transform[] eyes_, VRRig rig_, float maxDist = 10000f)
	{
		if (rig_ == this.targetRig)
		{
			return;
		}
		this.eyeBones = eyes_;
		this.targetRig = rig_;
		this.targetPos = this.targetRig.transform.position;
		base.enabled = true;
		LineRenderer[] array = this.lines;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = true;
		}
		if (this.targetFx != null)
		{
			this.targetFx.transform.position = this.targetPos;
			this.targetFx.SetActive(true);
		}
	}

	public void EnableLazer(Transform[] eyes_, Vector3 targetPos_)
	{
		this.eyeBones = eyes_;
		this.targetRig = null;
		this.targetPos = targetPos_;
		base.enabled = true;
		LineRenderer[] array = this.lines;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = true;
		}
		if (this.targetFx != null)
		{
			this.targetFx.transform.position = this.targetPos;
			this.targetFx.SetActive(true);
		}
	}

	public void DisableLazer()
	{
		this.targetRig = null;
		if (base.enabled)
		{
			base.enabled = false;
			LineRenderer[] array = this.lines;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
			if (this.targetFx != null)
			{
				this.targetFx.SetActive(false);
			}
		}
	}

	private void Update()
	{
		if (this.targetRig != null)
		{
			this.targetPos = this.targetRig.transform.position;
		}
		for (int i = 0; i < this.lines.Length; i++)
		{
			this.lines[i].SetPosition(0, this.eyeBones[i].transform.position);
			this.lines[i].SetPosition(1, this.targetPos);
		}
		if (this.targetFx != null)
		{
			this.targetFx.transform.position = this.targetPos;
		}
	}

	private Transform[] eyeBones;

	private VRRig targetRig;

	private Vector3 targetPos;

	public LineRenderer[] lines;

	public GameObject targetFx;
}
