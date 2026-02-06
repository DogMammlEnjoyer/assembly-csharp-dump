using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(9999)]
public class PostVRRigPhysicsSynch : MonoBehaviour
{
	private void LateUpdate()
	{
		for (int i = 0; i < PostVRRigPhysicsSynch.k_syncList.Count; i++)
		{
			AutoSyncTransforms autoSyncTransforms = PostVRRigPhysicsSynch.k_syncList[i];
			Transform targetTransform = autoSyncTransforms.TargetTransform;
			Rigidbody targetRigidbody = autoSyncTransforms.TargetRigidbody;
			Vector3 position = targetTransform.position;
			Quaternion rotation = targetTransform.rotation;
			targetRigidbody.position = position;
			targetRigidbody.rotation = rotation;
		}
	}

	public static void AddSyncTarget(AutoSyncTransforms body)
	{
		PostVRRigPhysicsSynch.k_syncList.Add(body);
	}

	public static void RemoveSyncTarget(AutoSyncTransforms body)
	{
		PostVRRigPhysicsSynch.k_syncList.Remove(body);
	}

	private static readonly List<AutoSyncTransforms> k_syncList = new List<AutoSyncTransforms>(5);
}
