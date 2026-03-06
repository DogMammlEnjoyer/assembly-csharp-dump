using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.AI;

namespace GorillaTagScripts.AI
{
	public class AIEntity : MonoBehaviour
	{
		protected void Awake()
		{
			this.navMeshAgent = base.gameObject.GetComponent<NavMeshAgent>();
			this.animator = base.gameObject.GetComponent<Animator>();
			if (this.waypointsContainer != null)
			{
				foreach (Transform item in this.waypointsContainer.GetComponentsInChildren<Transform>())
				{
					this.waypoints.Add(item);
				}
			}
		}

		protected void ChooseRandomTarget()
		{
			int randomTarget = Random.Range(0, VRRigCache.ActiveRigs.Count);
			int num = VRRigCache.ActiveRigContainers.FindIndex((RigContainer x) => x.Rig.creator != null && x.Rig.creator == VRRigCache.ActiveRigContainers[randomTarget].Rig.creator);
			if (num == -1)
			{
				num = Random.Range(0, VRRigCache.ActiveRigs.Count);
			}
			if (num < VRRigCache.ActiveRigContainers.Count)
			{
				this.targetPlayer = VRRigCache.ActiveRigContainers[num].Rig.creator;
				this.followTarget = VRRigCache.ActiveRigContainers[num].Rig.head.rigTarget;
				NavMeshHit navMeshHit;
				this.targetIsOnNavMesh = NavMesh.SamplePosition(this.followTarget.position, out navMeshHit, this.navMeshSampleRange, 1);
				return;
			}
			this.targetPlayer = null;
			this.followTarget = null;
		}

		protected void ChooseClosestTarget()
		{
			VRRig vrrig = null;
			float num = float.MaxValue;
			foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
			{
				VRRig rig = rigContainer.Rig;
				if (rig.head != null && !rig.head.rigTarget.IsNull())
				{
					float sqrMagnitude = (base.transform.position - rig.head.rigTarget.transform.position).sqrMagnitude;
					if (sqrMagnitude < this.minChaseRange * this.minChaseRange && sqrMagnitude < num)
					{
						num = sqrMagnitude;
						vrrig = rig;
					}
				}
			}
			if (vrrig.IsNotNull())
			{
				this.targetPlayer = vrrig.creator;
				this.followTarget = vrrig.head.rigTarget;
				NavMeshHit navMeshHit;
				this.targetIsOnNavMesh = NavMesh.SamplePosition(this.followTarget.position, out navMeshHit, this.navMeshSampleRange, 1);
				return;
			}
			this.targetPlayer = null;
			this.followTarget = null;
		}

		public GameObject waypointsContainer;

		public Transform circleCenter;

		public float circleRadius;

		public float angularSpeed;

		public float patrolSpeed;

		public float fleeSpeed;

		public NavMeshAgent navMeshAgent;

		public Animator animator;

		public float fleeRang;

		public float fleeSpeedMult;

		public float minChaseRange;

		public float attackDistance;

		public float navMeshSampleRange = 5f;

		internal readonly List<Transform> waypoints = new List<Transform>();

		internal float defaultSpeed;

		public Transform followTarget;

		public NetPlayer targetPlayer;

		public bool targetIsOnNavMesh;
	}
}
