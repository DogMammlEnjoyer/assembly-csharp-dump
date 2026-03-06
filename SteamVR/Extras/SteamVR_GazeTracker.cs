using System;
using UnityEngine;

namespace Valve.VR.Extras
{
	public class SteamVR_GazeTracker : MonoBehaviour
	{
		public event GazeEventHandler GazeOn;

		public event GazeEventHandler GazeOff;

		public virtual void OnGazeOn(GazeEventArgs gazeEventArgs)
		{
			if (this.GazeOn != null)
			{
				this.GazeOn(this, gazeEventArgs);
			}
		}

		public virtual void OnGazeOff(GazeEventArgs gazeEventArgs)
		{
			if (this.GazeOff != null)
			{
				this.GazeOff(this, gazeEventArgs);
			}
		}

		protected virtual void Update()
		{
			if (this.hmdTrackedObject == null)
			{
				foreach (SteamVR_TrackedObject steamVR_TrackedObject in Object.FindObjectsOfType<SteamVR_TrackedObject>())
				{
					if (steamVR_TrackedObject.index == SteamVR_TrackedObject.EIndex.Hmd)
					{
						this.hmdTrackedObject = steamVR_TrackedObject.transform;
						break;
					}
				}
			}
			if (this.hmdTrackedObject)
			{
				Ray ray = new Ray(this.hmdTrackedObject.position, this.hmdTrackedObject.forward);
				Plane plane = new Plane(this.hmdTrackedObject.forward, base.transform.position);
				float d = 0f;
				if (plane.Raycast(ray, out d))
				{
					float num = Vector3.Distance(this.hmdTrackedObject.position + this.hmdTrackedObject.forward * d, base.transform.position);
					if (num < this.gazeInCutoff && !this.isInGaze)
					{
						this.isInGaze = true;
						GazeEventArgs gazeEventArgs;
						gazeEventArgs.distance = num;
						this.OnGazeOn(gazeEventArgs);
						return;
					}
					if (num >= this.gazeOutCutoff && this.isInGaze)
					{
						this.isInGaze = false;
						GazeEventArgs gazeEventArgs2;
						gazeEventArgs2.distance = num;
						this.OnGazeOff(gazeEventArgs2);
					}
				}
			}
		}

		public bool isInGaze;

		public float gazeInCutoff = 0.15f;

		public float gazeOutCutoff = 0.4f;

		protected Transform hmdTrackedObject;
	}
}
