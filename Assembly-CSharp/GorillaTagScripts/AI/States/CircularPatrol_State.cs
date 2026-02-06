using System;
using UnityEngine;

namespace GorillaTagScripts.AI.States
{
	public class CircularPatrol_State : IState
	{
		public CircularPatrol_State(AIEntity entity)
		{
			this.entity = entity;
		}

		public void Tick()
		{
			Vector3 position = this.entity.circleCenter.position;
			float x = position.x + Mathf.Cos(this.angle) * this.entity.angularSpeed;
			float y = position.y;
			float z = position.z + Mathf.Sin(this.angle) * this.entity.angularSpeed;
			this.entity.transform.position = new Vector3(x, y, z);
			this.angle += this.entity.angularSpeed * Time.deltaTime;
		}

		public void OnEnter()
		{
		}

		public void OnExit()
		{
		}

		private AIEntity entity;

		private float angle;
	}
}
