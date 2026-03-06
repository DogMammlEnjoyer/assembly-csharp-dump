using System;
using Oculus.Interaction.Grab;
using Oculus.Interaction.Grab.GrabSurfaces;
using Oculus.Interaction.HandGrab.Visuals;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.HandGrab
{
	public class HandGrabPose : MonoBehaviour
	{
		public IGrabSurface SnapSurface
		{
			get
			{
				return this._snapSurface ?? (this._surface as IGrabSurface);
			}
			private set
			{
				this._snapSurface = value;
			}
		}

		public HandPose HandPose
		{
			get
			{
				if (!this._usesHandPose)
				{
					return null;
				}
				return this._targetHandPose;
			}
		}

		internal static Pose GetOVROffset(Handedness handedness)
		{
			if (handedness != Handedness.Left)
			{
				return HandGrabPose.OVR_OFFSET_RH;
			}
			return HandGrabPose.OVR_OFFSET_LH;
		}

		public float RelativeScale
		{
			get
			{
				return base.transform.lossyScale.x / this._relativeTo.lossyScale.x;
			}
		}

		public Pose RelativePose
		{
			get
			{
				if (this._relativeTo != null)
				{
					return PoseUtils.DeltaScaled(this._relativeTo, this.WorldPose);
				}
				return this.LocalPose;
			}
		}

		public Transform RelativeTo
		{
			get
			{
				return this._relativeTo;
			}
		}

		private Pose LocalPose
		{
			get
			{
				return base.transform.GetPose(Space.Self);
			}
		}

		private Pose WorldPose
		{
			get
			{
				return base.transform.GetPose(Space.World);
			}
		}

		protected virtual void Awake()
		{
		}

		protected virtual void Reset()
		{
			IRelativeToRef componentInParent = base.GetComponentInParent<IRelativeToRef>();
			this._relativeTo = ((componentInParent != null) ? componentInParent.RelativeTo : null);
		}

		public bool UsesHandPose()
		{
			return this._usesHandPose;
		}

		[Obsolete("Use CalculateBestPose with offset instead")]
		public virtual bool CalculateBestPose(Pose userPose, Handedness handedness, PoseMeasureParameters scoringModifier, Transform relativeTo, ref HandGrabResult result)
		{
			Pose identity = Pose.identity;
			this.CalculateBestPose(userPose, identity, relativeTo, handedness, scoringModifier, ref result);
			return true;
		}

		public virtual void CalculateBestPose(in Pose userPose, in Pose offset, Transform relativeTo, Handedness handedness, PoseMeasureParameters scoringModifier, ref HandGrabResult result)
		{
			result.HasHandPose = false;
			Pose pose;
			result.Score = this.CompareNearPoses(userPose, offset, relativeTo, scoringModifier, out pose);
			result.RelativePose = relativeTo.Delta(pose);
			if (this.HandPose != null)
			{
				result.HasHandPose = true;
				result.HandPose.CopyFrom(this.HandPose, false);
			}
		}

		private GrabPoseScore CompareNearPoses(in Pose worldPoint, in Pose offset, Transform relativeTo, PoseMeasureParameters scoringModifier, out Pose bestWorldPose)
		{
			GrabPoseScore result;
			if (this.SnapSurface != null)
			{
				result = this.SnapSurface.CalculateBestPoseAtSurface(worldPoint, offset, out bestWorldPose, scoringModifier, relativeTo);
			}
			else
			{
				bestWorldPose = PoseUtils.GlobalPoseScaled(relativeTo, this.RelativePose);
				result = new GrabPoseScore(ref worldPoint, ref bestWorldPose, ref offset, scoringModifier);
			}
			return result;
		}

		public void InjectAllHandGrabPose(Transform relativeTo)
		{
			this.InjectRelativeTo(relativeTo);
		}

		public void InjectRelativeTo(Transform relativeTo)
		{
			this._relativeTo = relativeTo;
		}

		public void InjectOptionalSurface(IGrabSurface surface)
		{
			this._surface = (surface as Object);
			this.SnapSurface = surface;
		}

		public void InjectOptionalHandPose(HandPose handPose)
		{
			this._targetHandPose = handPose;
			this._usesHandPose = (this._targetHandPose != null);
		}

		private static readonly Pose OVR_OFFSET_LH = new Pose(Vector3.zero, Quaternion.Euler(0f, 90f, 180f));

		private static readonly Pose OVR_OFFSET_RH = new Pose(Vector3.zero, Quaternion.Euler(0f, 90f, 0f));

		[SerializeField]
		[Optional]
		[Interface(typeof(IGrabSurface), new Type[]
		{

		})]
		private Object _surface;

		private IGrabSurface _snapSurface;

		[SerializeField]
		[Tooltip("Transform used as a reference to measure the local data of the HandGrabPose")]
		private Transform _relativeTo;

		[SerializeField]
		private bool _usesHandPose = true;

		[SerializeField]
		[Optional]
		[HideInInspector]
		[InspectorName("Hand Pose")]
		private HandPose _handPose = new HandPose();

		[SerializeField]
		[Optional]
		[HideInInspector]
		private HandPose _targetHandPose = new HandPose();

		[SerializeField]
		[HideInInspector]
		private HandGhostProvider _ghostProvider;

		[SerializeField]
		[HideInInspector]
		private HandGhostProvider _handGhostProvider;

		[SerializeField]
		[HideInInspector]
		private HandGrabPose.OVROffsetMode _ovrOffsetMode;

		internal enum OVROffsetMode
		{
			None,
			Apply,
			Ignore
		}
	}
}
