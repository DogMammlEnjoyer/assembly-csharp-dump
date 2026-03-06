using System;
using System.Collections.Generic;
using UnityEngine.SpatialTracking;

namespace UnityEngine.XR.LegacyInputHelpers
{
	public class TransitionArmModel : ArmModel
	{
		public ArmModel currentArmModelComponent
		{
			get
			{
				return this.m_CurrentArmModelComponent;
			}
			set
			{
				this.m_CurrentArmModelComponent = value;
			}
		}

		public bool Queue(string key)
		{
			foreach (ArmModelTransition armModelTransition in this.m_ArmModelTransitions)
			{
				if (armModelTransition.transitionKeyName == key)
				{
					this.Queue(armModelTransition.armModel);
					return true;
				}
			}
			return false;
		}

		public void Queue(ArmModel newArmModel)
		{
			if (newArmModel == null)
			{
				return;
			}
			if (this.m_CurrentArmModelComponent == null)
			{
				this.m_CurrentArmModelComponent = newArmModel;
			}
			this.RemoveJustStartingTransitions();
			if (this.armModelBlendData.Count == 10)
			{
				this.RemoveOldestTransition();
			}
			TransitionArmModel.ArmModelBlendData item = default(TransitionArmModel.ArmModelBlendData);
			item.armModel = newArmModel;
			item.currentBlendAmount = 0f;
			this.armModelBlendData.Add(item);
		}

		private void RemoveJustStartingTransitions()
		{
			for (int i = 0; i < this.armModelBlendData.Count; i++)
			{
				if (this.armModelBlendData[i].currentBlendAmount < 0.035f)
				{
					this.armModelBlendData.RemoveAt(i);
				}
			}
		}

		private void RemoveOldestTransition()
		{
			this.armModelBlendData.RemoveAt(0);
		}

		public override PoseDataFlags GetPoseFromProvider(out Pose output)
		{
			if (this.UpdateBlends())
			{
				output = base.finalPose;
				return PoseDataFlags.Position | PoseDataFlags.Rotation;
			}
			output = Pose.identity;
			return PoseDataFlags.NoData;
		}

		private bool UpdateBlends()
		{
			if (this.currentArmModelComponent == null)
			{
				return false;
			}
			if (this.m_CurrentArmModelComponent.OnControllerInputUpdated())
			{
				this.m_NeckPosition = this.m_CurrentArmModelComponent.neckPosition;
				this.m_ElbowPosition = this.m_CurrentArmModelComponent.elbowPosition;
				this.m_WristPosition = this.m_CurrentArmModelComponent.wristPosition;
				this.m_ControllerPosition = this.m_CurrentArmModelComponent.controllerPosition;
				this.m_ElbowRotation = this.m_CurrentArmModelComponent.elbowRotation;
				this.m_WristRotation = this.m_CurrentArmModelComponent.wristRotation;
				this.m_ControllerRotation = this.m_CurrentArmModelComponent.controllerRotation;
				Vector3 vector;
				if (base.TryGetAngularVelocity(base.poseSource, out vector) && this.armModelBlendData.Count > 0)
				{
					float t = Mathf.Clamp((vector.magnitude - 0.2f) / 45f, 0f, 0.1f);
					for (int i = 0; i < this.armModelBlendData.Count; i++)
					{
						TransitionArmModel.ArmModelBlendData armModelBlendData = this.armModelBlendData[i];
						armModelBlendData.currentBlendAmount = Mathf.Lerp(armModelBlendData.currentBlendAmount, 1f, t);
						if (armModelBlendData.currentBlendAmount > 0.95f)
						{
							armModelBlendData.currentBlendAmount = 1f;
						}
						else
						{
							armModelBlendData.armModel.OnControllerInputUpdated();
							this.m_NeckPosition = Vector3.Slerp(base.neckPosition, armModelBlendData.armModel.neckPosition, armModelBlendData.currentBlendAmount);
							this.m_ElbowPosition = Vector3.Slerp(base.elbowPosition, armModelBlendData.armModel.elbowPosition, armModelBlendData.currentBlendAmount);
							this.m_WristPosition = Vector3.Slerp(base.wristPosition, armModelBlendData.armModel.wristPosition, armModelBlendData.currentBlendAmount);
							this.m_ControllerPosition = Vector3.Slerp(base.controllerPosition, armModelBlendData.armModel.controllerPosition, armModelBlendData.currentBlendAmount);
							this.m_ElbowRotation = Quaternion.Slerp(base.elbowRotation, armModelBlendData.armModel.elbowRotation, armModelBlendData.currentBlendAmount);
							this.m_WristRotation = Quaternion.Slerp(base.wristRotation, armModelBlendData.armModel.wristRotation, armModelBlendData.currentBlendAmount);
							this.m_ControllerRotation = Quaternion.Slerp(base.controllerRotation, armModelBlendData.armModel.controllerRotation, armModelBlendData.currentBlendAmount);
						}
						this.armModelBlendData[i] = armModelBlendData;
						if (armModelBlendData.currentBlendAmount >= 1f)
						{
							this.m_CurrentArmModelComponent = armModelBlendData.armModel;
							this.armModelBlendData.RemoveRange(0, i + 1);
						}
					}
				}
				else if (this.armModelBlendData.Count > 0)
				{
					Debug.LogErrorFormat(base.gameObject, "Unable to get angular acceleration for node", Array.Empty<object>());
					return false;
				}
				base.finalPose = new Pose(base.controllerPosition, base.controllerRotation);
				return true;
			}
			return false;
		}

		[SerializeField]
		private ArmModel m_CurrentArmModelComponent;

		[SerializeField]
		public List<ArmModelTransition> m_ArmModelTransitions = new List<ArmModelTransition>();

		private const int MAX_ACTIVE_TRANSITIONS = 10;

		private const float DROP_TRANSITION_THRESHOLD = 0.035f;

		private const float LERP_CLAMP_THRESHOLD = 0.95f;

		private const float MIN_ANGULAR_VELOCITY = 0.2f;

		private const float ANGULAR_VELOCITY_DIVISOR = 45f;

		internal List<TransitionArmModel.ArmModelBlendData> armModelBlendData = new List<TransitionArmModel.ArmModelBlendData>(10);

		private TransitionArmModel.ArmModelBlendData currentBlendingArmModel;

		internal struct ArmModelBlendData
		{
			public ArmModel armModel;

			public float currentBlendAmount;
		}
	}
}
