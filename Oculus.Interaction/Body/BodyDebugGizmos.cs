using System;
using Oculus.Interaction.Body.Input;
using UnityEngine;

namespace Oculus.Interaction.Body
{
	public class BodyDebugGizmos : SkeletonDebugGizmos
	{
		public BodyDebugGizmos.CoordSpace Space
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

		protected virtual void Awake()
		{
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
				this.Body.WhenBodyUpdated += this.HandleBodyUpdated;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.Body.WhenBodyUpdated -= this.HandleBodyUpdated;
			}
		}

		protected override bool TryGetJointPose(int jointId, out Pose pose)
		{
			BodyDebugGizmos.CoordSpace space = this._space;
			bool result;
			if (space == BodyDebugGizmos.CoordSpace.World || space != BodyDebugGizmos.CoordSpace.Local)
			{
				result = this.Body.GetJointPose((BodyJointId)jointId, out pose);
			}
			else
			{
				result = this.Body.GetJointPoseFromRoot((BodyJointId)jointId, out pose);
				pose.position = base.transform.TransformPoint(pose.position);
				pose.rotation = base.transform.rotation * pose.rotation;
			}
			return result;
		}

		protected override bool TryGetParentJointId(int jointId, out int parent)
		{
			BodyJointId bodyJointId;
			if (this.Body.SkeletonMapping.TryGetParentJointId((BodyJointId)jointId, out bodyJointId))
			{
				parent = (int)bodyJointId;
				return true;
			}
			parent = 0;
			return false;
		}

		private SkeletonDebugGizmos.VisibilityFlags GetModifiedDrawFlags()
		{
			SkeletonDebugGizmos.VisibilityFlags visibilityFlags = base.Visibility;
			if (base.HasNegativeScale && this.Space == BodyDebugGizmos.CoordSpace.Local)
			{
				visibilityFlags &= ~SkeletonDebugGizmos.VisibilityFlags.Axes;
			}
			return visibilityFlags;
		}

		private void HandleBodyUpdated()
		{
			foreach (BodyJointId joint in this.Body.SkeletonMapping.Joints)
			{
				base.Draw((int)joint, this.GetModifiedDrawFlags());
			}
		}

		public void InjectAllBodyJointDebugGizmos(IBody body)
		{
			this.InjectBody(body);
		}

		public void InjectBody(IBody body)
		{
			this._body = (body as Object);
			this.Body = body;
		}

		[SerializeField]
		[Interface(typeof(IBody), new Type[]
		{

		})]
		private Object _body;

		private IBody Body;

		[Tooltip("The coordinate space in which to draw the skeleton. World space draws the skeleton at the world Body location. Local draws the skeleton relative to this transform's position, and can be placed, scaled, or mirrored as desired.")]
		[SerializeField]
		private BodyDebugGizmos.CoordSpace _space;

		protected bool _started;

		public enum CoordSpace
		{
			World,
			Local
		}
	}
}
