using System;
using UnityEngine;

public class OVRBone : IDisposable
{
	public OVRSkeleton.BoneId Id { get; set; }

	public short ParentBoneIndex { get; set; }

	public Transform Transform { get; set; }

	public OVRBone()
	{
	}

	public OVRBone(OVRSkeleton.BoneId id, short parentBoneIndex, Transform trans)
	{
		this.Id = id;
		this.ParentBoneIndex = parentBoneIndex;
		this.Transform = trans;
	}

	public void Dispose()
	{
		if (this.Transform != null)
		{
			Object.Destroy(this.Transform.gameObject);
			this.Transform = null;
		}
	}
}
