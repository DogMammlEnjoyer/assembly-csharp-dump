using System;
using System.Collections.Generic;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection
{
	public class JointDeltaProvider : MonoBehaviour, IJointDeltaProvider
	{
		private int PrevDataIndex
		{
			get
			{
				return 1 - this.CurDataIndex;
			}
		}

		public bool GetPositionDelta(HandJointId joint, out Vector3 delta)
		{
			this.UpdateData();
			JointDeltaProvider.PoseData poseData = this._poseDataCache[joint][this.PrevDataIndex];
			JointDeltaProvider.PoseData poseData2 = this._poseDataCache[joint][this.CurDataIndex];
			if (!poseData.IsValid || !poseData2.IsValid)
			{
				delta = Vector3.zero;
				return false;
			}
			delta = poseData2.Pose.position - poseData.Pose.position;
			return true;
		}

		public bool GetRotationDelta(HandJointId joint, out Quaternion delta)
		{
			this.UpdateData();
			JointDeltaProvider.PoseData poseData = this._poseDataCache[joint][this.PrevDataIndex];
			JointDeltaProvider.PoseData poseData2 = this._poseDataCache[joint][this.CurDataIndex];
			if (!poseData.IsValid || !poseData2.IsValid)
			{
				delta = Quaternion.identity;
				return false;
			}
			delta = poseData2.Pose.rotation * Quaternion.Inverse(poseData.Pose.rotation);
			return true;
		}

		public bool GetPrevJointPose(HandJointId joint, out Pose pose)
		{
			this.UpdateData();
			JointDeltaProvider.PoseData poseData = this._poseDataCache[joint][this.PrevDataIndex];
			pose = poseData.Pose;
			return poseData.IsValid;
		}

		public void RegisterConfig(JointDeltaConfig config)
		{
			this._requestors.ContainsKey(config.InstanceID);
			this._requestors.Add(config.InstanceID, new List<HandJointId>(config.JointIDs));
			foreach (HandJointId handJointId in config.JointIDs)
			{
				if (!this._poseDataCache.ContainsKey(handJointId))
				{
					this._poseDataCache.Add(handJointId, new JointDeltaProvider.PoseData[]
					{
						new JointDeltaProvider.PoseData(),
						new JointDeltaProvider.PoseData()
					});
					JointDeltaProvider.PoseData poseData = this._poseDataCache[handJointId][this.CurDataIndex];
					poseData.IsValid = this.Hand.GetJointPose(handJointId, out poseData.Pose);
				}
			}
		}

		public void UnRegisterConfig(JointDeltaConfig config)
		{
			this._requestors.Remove(config.InstanceID);
		}

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
				this.Hand.WhenHandUpdated += this.UpdateData;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.Hand.WhenHandUpdated -= this.UpdateData;
			}
		}

		private void UpdateData()
		{
			if (this.Hand.CurrentDataVersion <= this._lastUpdateDataVersion)
			{
				return;
			}
			this._lastUpdateDataVersion = this.Hand.CurrentDataVersion;
			this.CurDataIndex = 1 - this.CurDataIndex;
			this._trackedJoints.Clear();
			foreach (int key in this._requestors.Keys)
			{
				IList<HandJointId> other = this._requestors[key];
				this._trackedJoints.UnionWithNonAlloc(other);
			}
			foreach (HandJointId handJointId in this._poseDataCache.Keys)
			{
				JointDeltaProvider.PoseData poseData = this._poseDataCache[handJointId][this.CurDataIndex];
				poseData.IsValid = (this._trackedJoints.Contains(handJointId) && this.Hand.GetJointPose(handJointId, out poseData.Pose));
			}
		}

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		private IHand Hand;

		private Dictionary<HandJointId, JointDeltaProvider.PoseData[]> _poseDataCache = new Dictionary<HandJointId, JointDeltaProvider.PoseData[]>();

		private HashSet<HandJointId> _trackedJoints = new HashSet<HandJointId>();

		private Dictionary<int, List<HandJointId>> _requestors = new Dictionary<int, List<HandJointId>>();

		private int CurDataIndex;

		private int _lastUpdateDataVersion;

		protected bool _started;

		private class PoseData
		{
			public bool IsValid;

			public Pose Pose = Pose.identity;
		}
	}
}
