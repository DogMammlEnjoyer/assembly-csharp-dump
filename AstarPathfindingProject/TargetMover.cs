using System;
using System.Linq;
using UnityEngine;

namespace Pathfinding
{
	[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_target_mover.php")]
	public class TargetMover : MonoBehaviour
	{
		public void Start()
		{
			this.cam = Camera.main;
			this.ais = Object.FindObjectsOfType<MonoBehaviour>().OfType<IAstarAI>().ToArray<IAstarAI>();
			base.useGUILayout = false;
		}

		public void OnGUI()
		{
			if (this.onlyOnDoubleClick && this.cam != null && Event.current.type == EventType.MouseDown && Event.current.clickCount == 2)
			{
				this.UpdateTargetPosition();
			}
		}

		private void Update()
		{
			if (!this.onlyOnDoubleClick && this.cam != null)
			{
				this.UpdateTargetPosition();
			}
		}

		public void UpdateTargetPosition()
		{
			Vector3 vector = Vector3.zero;
			bool flag = false;
			RaycastHit raycastHit;
			if (this.use2D)
			{
				vector = this.cam.ScreenToWorldPoint(Input.mousePosition);
				vector.z = 0f;
				flag = true;
			}
			else if (Physics.Raycast(this.cam.ScreenPointToRay(Input.mousePosition), out raycastHit, float.PositiveInfinity, this.mask))
			{
				vector = raycastHit.point;
				flag = true;
			}
			if (flag && vector != this.target.position)
			{
				this.target.position = vector;
				if (this.onlyOnDoubleClick)
				{
					for (int i = 0; i < this.ais.Length; i++)
					{
						if (this.ais[i] != null)
						{
							this.ais[i].SearchPath();
						}
					}
				}
			}
		}

		public LayerMask mask;

		public Transform target;

		private IAstarAI[] ais;

		public bool onlyOnDoubleClick;

		public bool use2D;

		private Camera cam;
	}
}
