using System;
using UnityEngine;

public class CreateBonesLol : MonoBehaviour
{
	private void Update()
	{
		if (this.skeleton.Bones.Count <= 0)
		{
			return;
		}
		foreach (OVRBone ovrbone in this.skeleton.Bones)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.cube);
			gameObject.transform.parent = ovrbone.Transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
		}
		base.enabled = false;
	}

	public GameObject cube;

	public OVRSkeleton skeleton;
}
