using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Oculus.Interaction.HandGrab.Visuals
{
	[RequireComponent(typeof(HandPuppet))]
	public class HandGhost : MonoBehaviour
	{
		public Transform Root
		{
			get
			{
				return this._root;
			}
		}

		protected virtual void Reset()
		{
			this._puppet = base.GetComponent<HandPuppet>();
			this._handGrabPose = base.GetComponentInParent<HandGrabPose>();
		}

		protected virtual void OnValidate()
		{
			if (this._puppet == null)
			{
				return;
			}
			if (this._handGrabPose == null)
			{
				HandGrabPose componentInParent = base.GetComponentInParent<HandGrabPose>();
				if (componentInParent != null)
				{
					this.SetPose(componentInParent);
					return;
				}
			}
			else if (this._handGrabPose != null)
			{
				this.SetPose(this._handGrabPose);
			}
		}

		protected virtual void Start()
		{
			if (this._root == null)
			{
				this._root = base.transform;
			}
		}

		public void SetPose(HandGrabPose handGrabPose)
		{
			HandPose handPose = handGrabPose.HandPose;
			if (handPose == null)
			{
				return;
			}
			HandPuppet puppet = this._puppet;
			Quaternion[] jointRotations = handPose.JointRotations;
			puppet.SetJointRotations(jointRotations);
			this.SetRootPose(handGrabPose.RelativePose, handGrabPose.RelativeTo);
		}

		public void SetPose(HandPose userPose, Pose rootPose)
		{
			HandPuppet puppet = this._puppet;
			Quaternion[] jointRotations = userPose.JointRotations;
			puppet.SetJointRotations(jointRotations);
			this._puppet.SetRootPose(rootPose);
		}

		public void SetRootPose(Pose rootPose, Transform relativeTo)
		{
			Pose pose = rootPose;
			if (relativeTo != null)
			{
				pose = PoseUtils.GlobalPoseScaled(relativeTo, rootPose);
			}
			this._puppet.SetRootPose(pose);
		}

		public void InjectAllHandGhost(HandPuppet puppet)
		{
			this.InjectHandPuppet(puppet);
		}

		public void InjectHandPuppet(HandPuppet puppet)
		{
			this._puppet = puppet;
		}

		public void InjectOptionalHandGrabPose(HandGrabPose handGrabPose)
		{
			this._handGrabPose = handGrabPose;
		}

		public void InjectOptionalRoot(Transform root)
		{
			this._root = root;
		}

		[SerializeField]
		private HandPuppet _puppet;

		[SerializeField]
		[Optional]
		private Transform _root;

		[SerializeField]
		[Optional]
		[FormerlySerializedAs("_handGrabPoint")]
		private HandGrabPose _handGrabPose;
	}
}
