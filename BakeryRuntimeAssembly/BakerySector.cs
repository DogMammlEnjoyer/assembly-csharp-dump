using System;
using System.Collections.Generic;
using UnityEngine;

public class BakerySector : MonoBehaviour
{
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		for (int i = 0; i < this.cpoints.Count; i++)
		{
			if (this.cpoints[i] != null)
			{
				Gizmos.DrawWireSphere(this.cpoints[i].position, 1f);
			}
		}
	}

	public BakerySector.CaptureMode captureMode;

	public string captureAssetName = "";

	public BakerySectorCapture captureAsset;

	public bool allowUVPaddingAdjustment;

	public List<Transform> tforms = new List<Transform>();

	public List<Transform> cpoints = new List<Transform>();

	public enum CaptureMode
	{
		None = -1,
		CaptureInPlace,
		CaptureToAsset,
		LoadCaptured
	}
}
