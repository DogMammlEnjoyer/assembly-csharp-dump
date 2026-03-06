using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class HandDebugGizmos : SkeletonDebugGizmos, IHandVisual
	{
		public IHand Hand { get; private set; }

		public HandDebugGizmos.CoordSpace Space
		{
			get
			{
				return this._space;
			}
			set
			{
				this._space = value;
			}
		}

		public bool ForceOffVisibility { get; set; }

		public bool IsVisible
		{
			get
			{
				return this._isVisible;
			}
		}

		public event Action WhenHandVisualUpdated = delegate()
		{
		};

		protected virtual void Awake()
		{
			this.Hand = (this._hand as IHand);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this.Hand.WhenHandUpdated += this.HandleHandUpdated;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.Hand.WhenHandUpdated -= this.HandleHandUpdated;
			}
		}

		public Pose GetJointPose(HandJointId jointId, Space space)
		{
			Pose result2;
			if (space == UnityEngine.Space.Self)
			{
				Pose result;
				if (this.Hand.GetJointPoseLocal(jointId, out result))
				{
					return result;
				}
			}
			else if (space == UnityEngine.Space.World && this.Hand.GetJointPose(jointId, out result2))
			{
				return result2;
			}
			return default(Pose);
		}

		private void HandleHandUpdated()
		{
			this._isVisible = (this.Hand.IsTrackedDataValid && !this.ForceOffVisibility);
			if (this._isVisible)
			{
				for (HandJointId handJointId = HandJointId.HandStart; handJointId < HandJointId.HandEnd; handJointId++)
				{
					base.Draw((int)handJointId, base.Visibility);
				}
			}
			this.WhenHandVisualUpdated();
		}

		protected override bool TryGetParentJointId(int jointId, out int parent)
		{
			if (jointId >= HandJointUtils.JointParentList.Length)
			{
				parent = -1;
				return false;
			}
			parent = (int)HandJointUtils.JointParentList[jointId];
			return parent > -1;
		}

		protected override bool TryGetJointPose(int jointId, out Pose pose)
		{
			HandDebugGizmos.CoordSpace space = this._space;
			bool result;
			if (space == HandDebugGizmos.CoordSpace.World || space != HandDebugGizmos.CoordSpace.Local)
			{
				result = this.Hand.GetJointPose((HandJointId)jointId, out pose);
			}
			else
			{
				result = this.Hand.GetJointPoseFromWrist((HandJointId)jointId, out pose);
				pose.position = base.transform.TransformPoint(pose.position);
				pose.rotation = base.transform.rotation * pose.rotation;
			}
			return result;
		}

		public void InjectAllHandDebugGizmos(IHand hand)
		{
			this.InjectHand(hand);
		}

		public void InjectHand(IHand hand)
		{
			this._hand = (hand as Object);
			this.Hand = hand;
		}

		[Tooltip("The IHand that will drive the visuals.")]
		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		[Tooltip("The coordinate space in which to draw the skeleton. World space draws the skeleton at the world Body location. Local draws the skeleton relative to this transform's position, and can be placed, scaled, or mirrored as desired.")]
		[SerializeField]
		private HandDebugGizmos.CoordSpace _space;

		private bool _isVisible;

		protected bool _started;

		public enum CoordSpace
		{
			World,
			Local
		}
	}
}
