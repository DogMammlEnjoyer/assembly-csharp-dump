using System;
using System.Linq;
using UnityEngine;

namespace Oculus.Interaction.Input.Filter
{
	public class HandFilter : Hand
	{
		protected virtual void Awake()
		{
			for (int i = 0; i < 26; i++)
			{
				this._jointPosFilter[i] = OneEuroFilter.CreateVector3();
				this._jointRotFilter[i] = OneEuroFilter.CreateQuaternion();
			}
		}

		protected override void Apply(HandDataAsset handDataAsset)
		{
			base.Apply(handDataAsset);
			if (!handDataAsset.IsTracked)
			{
				return;
			}
			if (this.UpdateFilterParameters())
			{
				this.UpdateHandData(handDataAsset);
				return;
			}
		}

		protected bool UpdateFilterParameters()
		{
			if (this._filterParameters == null)
			{
				return true;
			}
			this._rootRotFilter.SetProperties(this._filterParameters.wristRotationParameters);
			this._rootPosFilter.SetProperties(this._filterParameters.wristPositionParameters);
			for (int i = 0; i < 26; i++)
			{
				this._jointRotFilter[i].SetProperties(this._filterParameters.fingerRotationParameters);
			}
			return true;
		}

		protected bool UpdateHandData(HandDataAsset handDataAsset)
		{
			if (this._filterParameters == null)
			{
				return true;
			}
			float deltaTime = 1f / this._filterParameters.frequency;
			Pose root = handDataAsset.Root;
			this._shadowHand.FromJoints(handDataAsset.JointPoses.ToList<Pose>(), false);
			handDataAsset.Root = new Pose(this._rootPosFilter.Step(root.position, deltaTime), this._rootRotFilter.Step(root.rotation, deltaTime));
			for (int i = 0; i < 26; i++)
			{
				HandJointId handJointId = (HandJointId)i;
				Pose localPose = this._shadowHand.GetLocalPose(handJointId);
				Quaternion rotation = this._jointRotFilter[i].Step(localPose.rotation, deltaTime);
				localPose.rotation = rotation;
				this._shadowHand.SetLocalPose(handJointId, localPose);
			}
			handDataAsset.JointPoses = this._shadowHand.GetWorldPoses();
			for (int j = 0; j < 26; j++)
			{
				int num = (int)HandJointUtils.JointParentList[j];
				handDataAsset.Joints[j] = ((num < 0) ? Quaternion.identity : (Quaternion.Inverse(handDataAsset.JointPoses[num].rotation) * handDataAsset.JointPoses[j].rotation));
			}
			return true;
		}

		[Header("Settings", order = -1)]
		[Tooltip("Applies a One Euro Filter when filter parameters are provided")]
		[SerializeField]
		[Optional]
		private HandFilterParameterBlock _filterParameters;

		private readonly IOneEuroFilter<Quaternion> _rootRotFilter = OneEuroFilter.CreateQuaternion();

		private readonly IOneEuroFilter<Vector3> _rootPosFilter = OneEuroFilter.CreateVector3();

		private readonly IOneEuroFilter<Vector3>[] _jointPosFilter = new IOneEuroFilter<Vector3>[26];

		private readonly IOneEuroFilter<Quaternion>[] _jointRotFilter = new IOneEuroFilter<Quaternion>[26];

		private ShadowHand _shadowHand = new ShadowHand();
	}
}
