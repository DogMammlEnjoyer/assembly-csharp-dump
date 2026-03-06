using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	[RequireComponent(typeof(CinemachineTargetGroup))]
	[ExecuteAlways]
	[AddComponentMenu("Cinemachine/Helpers/Cinemachine Group Weight Manipulator")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/GroupWeightManipulator.html")]
	public class GroupWeightManipulator : MonoBehaviour
	{
		private void Start()
		{
			base.TryGetComponent<CinemachineTargetGroup>(out this.m_Group);
		}

		private void OnValidate()
		{
			this.Weight0 = Mathf.Max(0f, this.Weight0);
			this.Weight1 = Mathf.Max(0f, this.Weight1);
			this.Weight2 = Mathf.Max(0f, this.Weight2);
			this.Weight3 = Mathf.Max(0f, this.Weight3);
			this.Weight4 = Mathf.Max(0f, this.Weight4);
			this.Weight5 = Mathf.Max(0f, this.Weight5);
			this.Weight6 = Mathf.Max(0f, this.Weight6);
			this.Weight7 = Mathf.Max(0f, this.Weight7);
		}

		private void Update()
		{
			if (this.m_Group != null)
			{
				this.UpdateWeights();
			}
		}

		private void UpdateWeights()
		{
			List<CinemachineTargetGroup.Target> targets = this.m_Group.Targets;
			int num = targets.Count - 1;
			if (num < 0)
			{
				return;
			}
			targets[0].Weight = this.Weight0;
			if (num < 1)
			{
				return;
			}
			targets[1].Weight = this.Weight1;
			if (num < 2)
			{
				return;
			}
			targets[2].Weight = this.Weight2;
			if (num < 3)
			{
				return;
			}
			targets[3].Weight = this.Weight3;
			if (num < 4)
			{
				return;
			}
			targets[4].Weight = this.Weight4;
			if (num < 5)
			{
				return;
			}
			targets[5].Weight = this.Weight5;
			if (num < 6)
			{
				return;
			}
			targets[6].Weight = this.Weight6;
			if (num < 7)
			{
				return;
			}
			targets[7].Weight = this.Weight7;
		}

		[Tooltip("The weight of the group member at index 0")]
		[FormerlySerializedAs("m_Weight0")]
		public float Weight0 = 1f;

		[Tooltip("The weight of the group member at index 1")]
		[FormerlySerializedAs("m_Weight1")]
		public float Weight1 = 1f;

		[Tooltip("The weight of the group member at index 2")]
		[FormerlySerializedAs("m_Weight2")]
		public float Weight2 = 1f;

		[Tooltip("The weight of the group member at index 3")]
		[FormerlySerializedAs("m_Weight3")]
		public float Weight3 = 1f;

		[Tooltip("The weight of the group member at index 4")]
		[FormerlySerializedAs("m_Weight4")]
		public float Weight4 = 1f;

		[Tooltip("The weight of the group member at index 5")]
		[FormerlySerializedAs("m_Weight5")]
		public float Weight5 = 1f;

		[Tooltip("The weight of the group member at index 6")]
		[FormerlySerializedAs("m_Weight6")]
		public float Weight6 = 1f;

		[Tooltip("The weight of the group member at index 7")]
		[FormerlySerializedAs("m_Weight7")]
		public float Weight7 = 1f;

		private CinemachineTargetGroup m_Group;
	}
}
