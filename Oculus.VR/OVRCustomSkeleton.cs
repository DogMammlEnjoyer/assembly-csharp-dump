using System;
using System.Collections.Generic;
using Meta.XR.Util;
using UnityEngine;

[HelpURL("https://developer.oculus.com/documentation/unity/move-samples/")]
[Feature(Feature.BodyTracking)]
public class OVRCustomSkeleton : OVRSkeleton, ISerializationCallbackReceiver
{
	public List<Transform> CustomBones
	{
		get
		{
			return this._customBones_V2;
		}
	}

	protected override Transform GetBoneTransform(OVRSkeleton.BoneId boneId)
	{
		return this._customBones_V2[(int)boneId];
	}

	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
	}

	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
		this.AllocateBones();
	}

	private void AllocateBones()
	{
		int num = 84;
		if (this._customBones_V2.Count == num)
		{
			return;
		}
		while (this._customBones_V2.Count < num)
		{
			this._customBones_V2.Add(null);
		}
	}

	internal override void SetSkeletonType(OVRSkeleton.SkeletonType skeletonType)
	{
		base.SetSkeletonType(skeletonType);
		if (this._customBones_V2 == null)
		{
			this._customBones_V2 = new List<Transform>();
		}
		this.AllocateBones();
	}

	[HideInInspector]
	[SerializeField]
	private List<Transform> _customBones_V2;

	[SerializeField]
	[HideInInspector]
	internal OVRCustomSkeleton.RetargetingType retargetingType;

	public enum RetargetingType
	{
		OculusSkeleton
	}
}
