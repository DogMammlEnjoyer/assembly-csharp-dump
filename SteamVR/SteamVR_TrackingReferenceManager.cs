using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR
{
	public class SteamVR_TrackingReferenceManager : MonoBehaviour
	{
		private void OnEnable()
		{
			SteamVR_Events.NewPoses.AddListener(new UnityAction<TrackedDevicePose_t[]>(this.OnNewPoses));
		}

		private void OnDisable()
		{
			SteamVR_Events.NewPoses.RemoveListener(new UnityAction<TrackedDevicePose_t[]>(this.OnNewPoses));
		}

		private void OnNewPoses(TrackedDevicePose_t[] poses)
		{
			if (poses == null)
			{
				return;
			}
			uint num = 0U;
			while ((ulong)num < (ulong)((long)poses.Length))
			{
				if (!this.trackingReferences.ContainsKey(num))
				{
					ETrackedDeviceClass trackedDeviceClass = OpenVR.System.GetTrackedDeviceClass(num);
					if (trackedDeviceClass == ETrackedDeviceClass.TrackingReference)
					{
						SteamVR_TrackingReferenceManager.TrackingReferenceObject trackingReferenceObject = new SteamVR_TrackingReferenceManager.TrackingReferenceObject();
						trackingReferenceObject.trackedDeviceClass = trackedDeviceClass;
						trackingReferenceObject.gameObject = new GameObject("Tracking Reference " + num.ToString());
						trackingReferenceObject.gameObject.transform.parent = base.transform;
						trackingReferenceObject.trackedObject = trackingReferenceObject.gameObject.AddComponent<SteamVR_TrackedObject>();
						trackingReferenceObject.renderModel = trackingReferenceObject.gameObject.AddComponent<SteamVR_RenderModel>();
						trackingReferenceObject.renderModel.createComponents = false;
						trackingReferenceObject.renderModel.updateDynamically = false;
						this.trackingReferences.Add(num, trackingReferenceObject);
						trackingReferenceObject.gameObject.SendMessage("SetDeviceIndex", (int)num, SendMessageOptions.DontRequireReceiver);
					}
					else
					{
						this.trackingReferences.Add(num, new SteamVR_TrackingReferenceManager.TrackingReferenceObject
						{
							trackedDeviceClass = trackedDeviceClass
						});
					}
				}
				num += 1U;
			}
		}

		private Dictionary<uint, SteamVR_TrackingReferenceManager.TrackingReferenceObject> trackingReferences = new Dictionary<uint, SteamVR_TrackingReferenceManager.TrackingReferenceObject>();

		private class TrackingReferenceObject
		{
			public ETrackedDeviceClass trackedDeviceClass;

			public GameObject gameObject;

			public SteamVR_RenderModel renderModel;

			public SteamVR_TrackedObject trackedObject;
		}
	}
}
