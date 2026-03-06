using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Pathfinding.Examples
{
	[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_examples_1_1_turn_based_manager.php")]
	public class TurnBasedManager : MonoBehaviour
	{
		private void Awake()
		{
			this.eventSystem = Object.FindObjectOfType<EventSystem>();
		}

		private void Update()
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (this.eventSystem.IsPointerOverGameObject())
			{
				return;
			}
			if (this.state == TurnBasedManager.State.SelectTarget)
			{
				this.HandleButtonUnderRay(ray);
			}
			if ((this.state == TurnBasedManager.State.SelectUnit || this.state == TurnBasedManager.State.SelectTarget) && Input.GetKeyDown(KeyCode.Mouse0))
			{
				TurnBasedAI byRay = this.GetByRay<TurnBasedAI>(ray);
				if (byRay != null)
				{
					this.Select(byRay);
					this.DestroyPossibleMoves();
					this.GeneratePossibleMoves(this.selected);
					this.state = TurnBasedManager.State.SelectTarget;
				}
			}
		}

		private void HandleButtonUnderRay(Ray ray)
		{
			Astar3DButton byRay = this.GetByRay<Astar3DButton>(ray);
			if (byRay != null && Input.GetKeyDown(KeyCode.Mouse0))
			{
				byRay.OnClick();
				this.DestroyPossibleMoves();
				this.state = TurnBasedManager.State.Move;
				base.StartCoroutine(this.MoveToNode(this.selected, byRay.node));
			}
		}

		private T GetByRay<T>(Ray ray) where T : class
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, float.PositiveInfinity, this.layerMask))
			{
				return raycastHit.transform.GetComponentInParent<T>();
			}
			return default(T);
		}

		private void Select(TurnBasedAI unit)
		{
			this.selected = unit;
		}

		private IEnumerator MoveToNode(TurnBasedAI unit, GraphNode node)
		{
			ABPath path = ABPath.Construct(unit.transform.position, (Vector3)node.position, null);
			path.traversalProvider = unit.traversalProvider;
			AstarPath.StartPath(path, false);
			yield return base.StartCoroutine(path.WaitForPath());
			if (path.error)
			{
				Debug.LogError("Path failed:\n" + path.errorLog);
				this.state = TurnBasedManager.State.SelectTarget;
				this.GeneratePossibleMoves(this.selected);
				yield break;
			}
			unit.targetNode = path.path[path.path.Count - 1];
			yield return base.StartCoroutine(TurnBasedManager.MoveAlongPath(unit, path, this.movementSpeed));
			unit.blocker.BlockAtCurrentPosition();
			this.state = TurnBasedManager.State.SelectUnit;
			yield break;
		}

		private static IEnumerator MoveAlongPath(TurnBasedAI unit, ABPath path, float speed)
		{
			if (path.error || path.vectorPath.Count == 0)
			{
				throw new ArgumentException("Cannot follow an empty path");
			}
			float distanceAlongSegment = 0f;
			int num;
			for (int i = 0; i < path.vectorPath.Count - 1; i = num + 1)
			{
				Vector3 p0 = path.vectorPath[Mathf.Max(i - 1, 0)];
				Vector3 p = path.vectorPath[i];
				Vector3 p2 = path.vectorPath[i + 1];
				Vector3 p3 = path.vectorPath[Mathf.Min(i + 2, path.vectorPath.Count - 1)];
				float segmentLength = Vector3.Distance(p, p2);
				while (distanceAlongSegment < segmentLength)
				{
					Vector3 position = AstarSplines.CatmullRom(p0, p, p2, p3, distanceAlongSegment / segmentLength);
					unit.transform.position = position;
					yield return null;
					distanceAlongSegment += Time.deltaTime * speed;
				}
				distanceAlongSegment -= segmentLength;
				p0 = default(Vector3);
				p = default(Vector3);
				p2 = default(Vector3);
				p3 = default(Vector3);
				num = i;
			}
			unit.transform.position = path.vectorPath[path.vectorPath.Count - 1];
			yield break;
		}

		private void DestroyPossibleMoves()
		{
			foreach (GameObject obj in this.possibleMoves)
			{
				Object.Destroy(obj);
			}
			this.possibleMoves.Clear();
		}

		private void GeneratePossibleMoves(TurnBasedAI unit)
		{
			ConstantPath constantPath = ConstantPath.Construct(unit.transform.position, unit.movementPoints * 1000 + 1, null);
			constantPath.traversalProvider = unit.traversalProvider;
			AstarPath.StartPath(constantPath, false);
			constantPath.BlockUntilCalculated();
			foreach (GraphNode graphNode in constantPath.allNodes)
			{
				if (graphNode != constantPath.startNode)
				{
					GameObject gameObject = Object.Instantiate<GameObject>(this.nodePrefab, (Vector3)graphNode.position, Quaternion.identity);
					this.possibleMoves.Add(gameObject);
					gameObject.GetComponent<Astar3DButton>().node = graphNode;
				}
			}
		}

		private TurnBasedAI selected;

		public float movementSpeed;

		public GameObject nodePrefab;

		public LayerMask layerMask;

		private List<GameObject> possibleMoves = new List<GameObject>();

		private EventSystem eventSystem;

		public TurnBasedManager.State state;

		public enum State
		{
			SelectUnit,
			SelectTarget,
			Move
		}
	}
}
