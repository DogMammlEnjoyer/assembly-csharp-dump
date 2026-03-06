using System;
using Pathfinding.RVO;
using UnityEngine;

namespace Pathfinding.Examples
{
	[RequireComponent(typeof(RVOController))]
	[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_examples_1_1_manual_r_v_o_agent.php")]
	public class ManualRVOAgent : MonoBehaviour
	{
		private void Awake()
		{
			this.rvo = base.GetComponent<RVOController>();
		}

		private void Update()
		{
			float axis = Input.GetAxis("Horizontal");
			float axis2 = Input.GetAxis("Vertical");
			Vector3 vector = new Vector3(axis, 0f, axis2) * this.speed;
			this.rvo.velocity = vector;
			base.transform.position += vector * Time.deltaTime;
		}

		private RVOController rvo;

		public float speed = 1f;
	}
}
