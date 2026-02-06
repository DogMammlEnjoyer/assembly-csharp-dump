using System;
using UnityEngine;

public class HandTrackingFingerCurl : MonoBehaviour
{
	public float ThumbCurl { get; private set; }

	public float TriggerCurl { get; private set; }

	public float GripCurl { get; private set; }

	private void Awake()
	{
		this.skeleton = base.GetComponent<OVRSkeleton>();
		this.boneXforms = new Transform[84];
	}

	private void LateUpdate()
	{
		if (this.skeleton == null || this.skeleton.Bones == null || this.skeleton.Bones.Count == 0)
		{
			return;
		}
		if (this.boneXforms[0] == null)
		{
			foreach (OVRBone ovrbone in this.skeleton.Bones)
			{
				this.boneXforms[(int)ovrbone.Id] = ovrbone.Transform;
			}
		}
		this.ThumbCurl = this.CalcFingerCurl(OVRSkeleton.BoneId.Hand_Thumb3, OVRSkeleton.BoneId.Hand_Thumb2, OVRSkeleton.BoneId.Hand_Thumb1, OVRSkeleton.BoneId.Hand_Thumb0);
		this.TriggerCurl = this.CalcFingerCurl(OVRSkeleton.BoneId.Hand_Middle1, OVRSkeleton.BoneId.Hand_Index3, OVRSkeleton.BoneId.Hand_Index2, OVRSkeleton.BoneId.Hand_Index1);
		this.GripCurl = this.CalcFingerCurl(OVRSkeleton.BoneId.Hand_Ring3, OVRSkeleton.BoneId.Hand_Ring2, OVRSkeleton.BoneId.Hand_Ring1, OVRSkeleton.BoneId.Hand_Middle3);
	}

	private float CalcFingerCurl(OVRSkeleton.BoneId distal, OVRSkeleton.BoneId intermediate, OVRSkeleton.BoneId proximal, OVRSkeleton.BoneId metacarpal)
	{
		Transform transform = this.boneXforms[(int)distal];
		Transform transform2 = this.boneXforms[(int)intermediate];
		Transform transform3 = this.boneXforms[(int)proximal];
		Transform transform4 = this.boneXforms[(int)metacarpal];
		if (transform == null || transform2 == null || transform3 == null || transform4 == null)
		{
			return 0f;
		}
		Vector3 from = transform.position - transform2.position;
		Vector3 vector = transform2.position - transform3.position;
		Vector3 to = transform3.position - transform4.position;
		float num = Vector3.Angle(from, vector);
		float num2 = Vector3.Angle(vector, to);
		float num3 = (num + num2) * 0.5f;
		num3 *= this.CurlMultiplier;
		num3 = Mathf.InverseLerp(this.ActivationStart, this.ActivationEnd, num3);
		return Mathf.Clamp01(num3);
	}

	private OVRSkeleton skeleton;

	public float ActivationStart = 5f;

	public float ActivationEnd = 95f;

	public float CurlMultiplier = 1.2f;

	private Transform[] boneXforms;
}
