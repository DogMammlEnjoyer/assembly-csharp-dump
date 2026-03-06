using System;
using Oculus.Interaction.Body.Input;
using Oculus.Interaction.Body.PoseDetection;
using UnityEngine;

namespace Oculus.Interaction.Body.Samples
{
	public class BodyPoseSwitcher : MonoBehaviour, IBodyPose
	{
		public event Action WhenBodyPoseUpdated = delegate()
		{
		};

		public ISkeletonMapping SkeletonMapping
		{
			get
			{
				return this.GetPose().SkeletonMapping;
			}
		}

		public bool GetJointPoseFromRoot(BodyJointId bodyJointId, out Pose pose)
		{
			return this.GetPose().GetJointPoseFromRoot(bodyJointId, out pose);
		}

		public bool GetJointPoseLocal(BodyJointId bodyJointId, out Pose pose)
		{
			return this.GetPose().GetJointPoseLocal(bodyJointId, out pose);
		}

		public BodyPoseSwitcher.PoseSource Source
		{
			get
			{
				return this._source;
			}
			set
			{
				bool flag = value != this._source;
				this._source = value;
				if (flag)
				{
					this.WhenBodyPoseUpdated();
				}
			}
		}

		public void UsePoseA()
		{
			this.Source = BodyPoseSwitcher.PoseSource.PoseA;
		}

		public void UsePoseB()
		{
			this.Source = BodyPoseSwitcher.PoseSource.PoseB;
		}

		protected virtual void Awake()
		{
			this.PoseA = (this._poseA as IBodyPose);
			this.PoseB = (this._poseB as IBodyPose);
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
				this.PoseA.WhenBodyPoseUpdated += delegate()
				{
					this.OnPoseUpdated(BodyPoseSwitcher.PoseSource.PoseA);
				};
				this.PoseB.WhenBodyPoseUpdated += delegate()
				{
					this.OnPoseUpdated(BodyPoseSwitcher.PoseSource.PoseB);
				};
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.PoseA.WhenBodyPoseUpdated -= delegate()
				{
					this.OnPoseUpdated(BodyPoseSwitcher.PoseSource.PoseA);
				};
				this.PoseB.WhenBodyPoseUpdated -= delegate()
				{
					this.OnPoseUpdated(BodyPoseSwitcher.PoseSource.PoseB);
				};
			}
		}

		private void OnPoseUpdated(BodyPoseSwitcher.PoseSource source)
		{
			if (source == this.Source)
			{
				this.WhenBodyPoseUpdated();
			}
		}

		private IBodyPose GetPose()
		{
			BodyPoseSwitcher.PoseSource source = this.Source;
			if (source == BodyPoseSwitcher.PoseSource.PoseA || source != BodyPoseSwitcher.PoseSource.PoseB)
			{
				return this.PoseA;
			}
			return this.PoseB;
		}

		[SerializeField]
		[Interface(typeof(IBodyPose), new Type[]
		{

		})]
		private Object _poseA;

		private IBodyPose PoseA;

		[SerializeField]
		[Interface(typeof(IBodyPose), new Type[]
		{

		})]
		private Object _poseB;

		private IBodyPose PoseB;

		[SerializeField]
		private BodyPoseSwitcher.PoseSource _source;

		protected bool _started;

		public enum PoseSource
		{
			PoseA,
			PoseB
		}
	}
}
