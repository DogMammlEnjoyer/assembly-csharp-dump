using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_dynamic_grid_obstacle.php")]
	public class DynamicGridObstacle : GraphModifier
	{
		private Bounds bounds
		{
			get
			{
				if (this.coll != null)
				{
					return this.coll.bounds;
				}
				Bounds bounds = this.coll2D.bounds;
				bounds.extents += new Vector3(0f, 0f, 10000f);
				return bounds;
			}
		}

		private bool colliderEnabled
		{
			get
			{
				if (!(this.coll != null))
				{
					return this.coll2D.enabled;
				}
				return this.coll.enabled;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			this.coll = base.GetComponent<Collider>();
			this.coll2D = base.GetComponent<Collider2D>();
			this.tr = base.transform;
			if (this.coll == null && this.coll2D == null && Application.isPlaying)
			{
				throw new Exception("A collider or 2D collider must be attached to the GameObject(" + base.gameObject.name + ") for the DynamicGridObstacle to work");
			}
			this.prevBounds = this.bounds;
			this.prevRotation = this.tr.rotation;
			this.prevEnabled = false;
		}

		public override void OnPostScan()
		{
			if (this.coll == null)
			{
				this.Awake();
			}
			if (this.coll != null)
			{
				this.prevEnabled = this.colliderEnabled;
			}
		}

		private void Update()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			if (this.coll == null && this.coll2D == null)
			{
				Debug.LogError("Removed collider from DynamicGridObstacle", this);
				base.enabled = false;
				return;
			}
			while (this.pendingGraphUpdates.Count > 0 && this.pendingGraphUpdates.Peek().stage != GraphUpdateStage.Pending)
			{
				this.pendingGraphUpdates.Dequeue();
			}
			if (AstarPath.active == null || AstarPath.active.isScanning || Time.realtimeSinceStartup - this.lastCheckTime < this.checkTime || !Application.isPlaying || this.pendingGraphUpdates.Count > 0)
			{
				return;
			}
			this.lastCheckTime = Time.realtimeSinceStartup;
			if (this.colliderEnabled)
			{
				Bounds bounds = this.bounds;
				Quaternion rotation = this.tr.rotation;
				Vector3 vector = this.prevBounds.min - bounds.min;
				Vector3 vector2 = this.prevBounds.max - bounds.max;
				float num = bounds.extents.magnitude * Quaternion.Angle(this.prevRotation, rotation) * 0.017453292f;
				if (vector.sqrMagnitude > this.updateError * this.updateError || vector2.sqrMagnitude > this.updateError * this.updateError || num > this.updateError || !this.prevEnabled)
				{
					this.DoUpdateGraphs();
					return;
				}
			}
			else if (this.prevEnabled)
			{
				this.DoUpdateGraphs();
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (AstarPath.active != null && Application.isPlaying)
			{
				GraphUpdateObject graphUpdateObject = new GraphUpdateObject(this.prevBounds);
				this.pendingGraphUpdates.Enqueue(graphUpdateObject);
				AstarPath.active.UpdateGraphs(graphUpdateObject);
				this.prevEnabled = false;
			}
			this.pendingGraphUpdates.Clear();
		}

		public void DoUpdateGraphs()
		{
			if (this.coll == null && this.coll2D == null)
			{
				return;
			}
			Physics.SyncTransforms();
			Physics2D.SyncTransforms();
			if (!this.colliderEnabled)
			{
				GraphUpdateObject graphUpdateObject = new GraphUpdateObject(this.prevBounds);
				this.pendingGraphUpdates.Enqueue(graphUpdateObject);
				AstarPath.active.UpdateGraphs(graphUpdateObject);
			}
			else
			{
				Bounds bounds = this.bounds;
				Bounds b = bounds;
				b.Encapsulate(this.prevBounds);
				if (DynamicGridObstacle.BoundsVolume(b) < DynamicGridObstacle.BoundsVolume(bounds) + DynamicGridObstacle.BoundsVolume(this.prevBounds))
				{
					GraphUpdateObject graphUpdateObject2 = new GraphUpdateObject(b);
					this.pendingGraphUpdates.Enqueue(graphUpdateObject2);
					AstarPath.active.UpdateGraphs(graphUpdateObject2);
				}
				else
				{
					GraphUpdateObject graphUpdateObject3 = new GraphUpdateObject(this.prevBounds);
					GraphUpdateObject graphUpdateObject4 = new GraphUpdateObject(bounds);
					this.pendingGraphUpdates.Enqueue(graphUpdateObject3);
					this.pendingGraphUpdates.Enqueue(graphUpdateObject4);
					AstarPath.active.UpdateGraphs(graphUpdateObject3);
					AstarPath.active.UpdateGraphs(graphUpdateObject4);
				}
				this.prevBounds = bounds;
			}
			this.prevEnabled = this.colliderEnabled;
			this.prevRotation = this.tr.rotation;
			this.lastCheckTime = Time.realtimeSinceStartup;
		}

		private static float BoundsVolume(Bounds b)
		{
			return Math.Abs(b.size.x * b.size.y * b.size.z);
		}

		private Collider coll;

		private Collider2D coll2D;

		private Transform tr;

		public float updateError = 1f;

		public float checkTime = 0.2f;

		private Bounds prevBounds;

		private Quaternion prevRotation;

		private bool prevEnabled;

		private float lastCheckTime = -9999f;

		private Queue<GraphUpdateObject> pendingGraphUpdates = new Queue<GraphUpdateObject>();
	}
}
