using System;
using System.Collections;
using UnityEngine;

namespace Pathfinding.Examples
{
	[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_examples_1_1_object_placer.php")]
	public class ObjectPlacer : MonoBehaviour
	{
		private void Update()
		{
			if (Input.GetKeyDown("p"))
			{
				this.PlaceObject();
			}
			if (Input.GetKeyDown("r"))
			{
				base.StartCoroutine(this.RemoveObject());
			}
		}

		public void PlaceObject()
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit, float.PositiveInfinity))
			{
				Vector3 point = raycastHit.point;
				GameObject gameObject = Object.Instantiate<GameObject>(this.go, point, Quaternion.identity);
				if (this.issueGUOs)
				{
					GraphUpdateObject ob = new GraphUpdateObject(gameObject.GetComponent<Collider>().bounds);
					AstarPath.active.UpdateGraphs(ob);
					if (this.direct)
					{
						AstarPath.active.FlushGraphUpdates();
					}
				}
			}
		}

		public IEnumerator RemoveObject()
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit, float.PositiveInfinity))
			{
				if (raycastHit.collider.isTrigger || raycastHit.transform.gameObject.name == "Ground")
				{
					yield break;
				}
				Bounds b = raycastHit.collider.bounds;
				Object.Destroy(raycastHit.collider);
				Object.Destroy(raycastHit.collider.gameObject);
				if (this.issueGUOs)
				{
					yield return new WaitForEndOfFrame();
					GraphUpdateObject ob = new GraphUpdateObject(b);
					AstarPath.active.UpdateGraphs(ob);
					if (this.direct)
					{
						AstarPath.active.FlushGraphUpdates();
					}
				}
				b = default(Bounds);
			}
			yield break;
		}

		public GameObject go;

		public bool direct;

		public bool issueGUOs = true;
	}
}
