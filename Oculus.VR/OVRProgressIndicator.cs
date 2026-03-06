using System;
using UnityEngine;

[HelpURL("https://developer.oculus.com/reference/unity/latest/class_o_v_r_progress_indicator")]
public class OVRProgressIndicator : MonoBehaviour
{
	private void Awake()
	{
		this.progressImage.sortingOrder = 150;
	}

	private void Update()
	{
		this.progressImage.sharedMaterial.SetFloat("_AlphaCutoff", 1f - this.currentProgress);
	}

	public MeshRenderer progressImage;

	[Range(0f, 1f)]
	public float currentProgress = 0.7f;
}
