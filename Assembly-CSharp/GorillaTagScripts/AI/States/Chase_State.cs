using System;
using UnityEngine;
using UnityEngine.AI;

namespace GorillaTagScripts.AI.States
{
	public class Chase_State : IState
	{
		public Transform FollowTarget { get; set; }

		public Chase_State(AIEntity entity)
		{
			this.entity = entity;
			this.agent = this.entity.navMeshAgent;
		}

		public void Tick()
		{
			this.agent.SetDestination(this.FollowTarget.position);
			if (this.agent.remainingDistance < this.entity.attackDistance)
			{
				this.chaseOver = true;
			}
		}

		public void OnEnter()
		{
			this.chaseOver = false;
			string str = "Current State: ";
			Type typeFromHandle = typeof(Chase_State);
			Debug.Log(str + ((typeFromHandle != null) ? typeFromHandle.ToString() : null));
		}

		public void OnExit()
		{
			this.chaseOver = true;
		}

		private AIEntity entity;

		private NavMeshAgent agent;

		public bool chaseOver;
	}
}
