using System;
using UnityEngine;

namespace Valve.VR.Extras
{
	[RequireComponent(typeof(SteamVR_TrackedObject))]
	public class SteamVR_TestThrow : MonoBehaviour
	{
		private void Awake()
		{
			this.trackedObj = base.GetComponent<SteamVR_Behaviour_Pose>();
		}

		private void FixedUpdate()
		{
			if (this.joint == null && this.spawn.GetStateDown(this.trackedObj.inputSource))
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.prefab);
				gameObject.transform.position = this.attachPoint.transform.position;
				this.joint = gameObject.AddComponent<FixedJoint>();
				this.joint.connectedBody = this.attachPoint;
				return;
			}
			if (this.joint != null && this.spawn.GetStateUp(this.trackedObj.inputSource))
			{
				GameObject gameObject2 = this.joint.gameObject;
				Rigidbody component = gameObject2.GetComponent<Rigidbody>();
				Object.DestroyImmediate(this.joint);
				this.joint = null;
				Object.Destroy(gameObject2, 15f);
				Transform transform = this.trackedObj.origin ? this.trackedObj.origin : this.trackedObj.transform.parent;
				if (transform != null)
				{
					component.linearVelocity = transform.TransformVector(this.trackedObj.GetVelocity());
					component.angularVelocity = transform.TransformVector(this.trackedObj.GetAngularVelocity());
				}
				else
				{
					component.linearVelocity = this.trackedObj.GetVelocity();
					component.angularVelocity = this.trackedObj.GetAngularVelocity();
				}
				component.maxAngularVelocity = component.angularVelocity.magnitude;
			}
		}

		public GameObject prefab;

		public Rigidbody attachPoint;

		public SteamVR_Action_Boolean spawn = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI", false);

		private SteamVR_Behaviour_Pose trackedObj;

		private FixedJoint joint;
	}
}
