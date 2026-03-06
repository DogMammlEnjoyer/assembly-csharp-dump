using System;
using System.Collections.Generic;
using Oculus.Interaction.Body.Input;
using UnityEngine;

namespace Oculus.Interaction.Body.PoseDetection
{
	public class PoseFromBody : MonoBehaviour, IBodyPose
	{
		public event Action WhenBodyPoseUpdated = delegate()
		{
		};

		public bool AutoUpdate
		{
			get
			{
				return this._autoUpdate;
			}
			set
			{
				this._autoUpdate = value;
			}
		}

		public ISkeletonMapping SkeletonMapping
		{
			get
			{
				return this.Body.SkeletonMapping;
			}
		}

		public bool GetJointPoseLocal(BodyJointId bodyJointId, out Pose pose)
		{
			return this._jointPosesLocal.TryGetValue(bodyJointId, out pose);
		}

		public bool GetJointPoseFromRoot(BodyJointId bodyJointId, out Pose pose)
		{
			return this._jointPosesFromRoot.TryGetValue(bodyJointId, out pose);
		}

		protected virtual void Awake()
		{
			this._jointPosesLocal = new Dictionary<BodyJointId, Pose>();
			this._jointPosesFromRoot = new Dictionary<BodyJointId, Pose>();
			this.Body = (this._body as IBody);
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
				this.Body.WhenBodyUpdated += this.Body_WhenBodyUpdated;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.Body.WhenBodyUpdated -= this.Body_WhenBodyUpdated;
			}
		}

		private void Body_WhenBodyUpdated()
		{
			if (this._autoUpdate)
			{
				this.UpdatePose();
			}
		}

		public void UpdatePose()
		{
			this._jointPosesLocal.Clear();
			this._jointPosesFromRoot.Clear();
			foreach (BodyJointId bodyJointId in this.Body.SkeletonMapping.Joints)
			{
				Pose value;
				if (this.Body.GetJointPoseLocal(bodyJointId, out value))
				{
					this._jointPosesLocal[bodyJointId] = value;
				}
				Pose value2;
				if (this.Body.GetJointPoseFromRoot(bodyJointId, out value2))
				{
					this._jointPosesFromRoot[bodyJointId] = value2;
				}
			}
			this.WhenBodyPoseUpdated();
		}

		public void InjectAllPoseFromBody(IBody body)
		{
			this.InjectBody(body);
		}

		public void InjectBody(IBody body)
		{
			this._body = (body as Object);
			this.Body = body;
		}

		[Tooltip("The IBodyPose will be derived from this IBody.")]
		[SerializeField]
		[Interface(typeof(IBody), new Type[]
		{

		})]
		private Object _body;

		private IBody Body;

		[Tooltip("If true, this component will track the provided IBody as its data is updated. If false, you must call UpdatePose to update joint data.")]
		[SerializeField]
		private bool _autoUpdate = true;

		protected bool _started;

		private Dictionary<BodyJointId, Pose> _jointPosesLocal;

		private Dictionary<BodyJointId, Pose> _jointPosesFromRoot;
	}
}
