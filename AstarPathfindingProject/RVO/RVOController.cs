using System;
using Pathfinding.Util;
using UnityEngine;
using UnityEngine.Serialization;

namespace Pathfinding.RVO
{
	[AddComponentMenu("Pathfinding/Local Avoidance/RVO Controller")]
	[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_r_v_o_1_1_r_v_o_controller.php")]
	public class RVOController : VersionedMonoBehaviour
	{
		public float radius
		{
			get
			{
				if (this.ai != null)
				{
					return this.ai.radius;
				}
				return this.radiusBackingField;
			}
			set
			{
				if (this.ai != null)
				{
					this.ai.radius = value;
				}
				this.radiusBackingField = value;
			}
		}

		public float height
		{
			get
			{
				if (this.ai != null)
				{
					return this.ai.height;
				}
				return this.heightBackingField;
			}
			set
			{
				if (this.ai != null)
				{
					this.ai.height = value;
				}
				this.heightBackingField = value;
			}
		}

		public float center
		{
			get
			{
				if (this.ai != null)
				{
					return this.ai.height / 2f;
				}
				return this.centerBackingField;
			}
			set
			{
				this.centerBackingField = value;
			}
		}

		[Obsolete("This field is obsolete in version 4.0 and will not affect anything. Use the LegacyRVOController if you need the old behaviour")]
		public LayerMask mask
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		[Obsolete("This field is obsolete in version 4.0 and will not affect anything. Use the LegacyRVOController if you need the old behaviour")]
		public bool enableRotation
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		[Obsolete("This field is obsolete in version 4.0 and will not affect anything. Use the LegacyRVOController if you need the old behaviour")]
		public float rotationSpeed
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		[Obsolete("This field is obsolete in version 4.0 and will not affect anything. Use the LegacyRVOController if you need the old behaviour")]
		public float maxSpeed
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		public MovementPlane movementPlane
		{
			get
			{
				if (this.simulator != null)
				{
					return this.simulator.movementPlane;
				}
				if (RVOSimulator.active)
				{
					return RVOSimulator.active.movementPlane;
				}
				return MovementPlane.XZ;
			}
		}

		public IAgent rvoAgent { get; private set; }

		public Simulator simulator { get; private set; }

		protected IAstarAI ai
		{
			get
			{
				if (this.aiBackingField as MonoBehaviour == null)
				{
					this.aiBackingField = null;
				}
				return this.aiBackingField;
			}
			set
			{
				this.aiBackingField = value;
			}
		}

		public Vector3 position
		{
			get
			{
				return this.To3D(this.rvoAgent.Position, this.rvoAgent.ElevationCoordinate);
			}
		}

		public Vector3 velocity
		{
			get
			{
				float num = (Time.deltaTime > 0.0001f) ? Time.deltaTime : 0.02f;
				return this.CalculateMovementDelta(num) / num;
			}
			set
			{
				this.rvoAgent.ForceSetVelocity(this.To2D(value));
			}
		}

		public Vector3 CalculateMovementDelta(float deltaTime)
		{
			if (this.rvoAgent == null)
			{
				return Vector3.zero;
			}
			return this.To3D(Vector2.ClampMagnitude(this.rvoAgent.CalculatedTargetPoint - this.To2D((this.ai != null) ? this.ai.position : this.tr.position), this.rvoAgent.CalculatedSpeed * deltaTime), 0f);
		}

		public Vector3 CalculateMovementDelta(Vector3 position, float deltaTime)
		{
			return this.To3D(Vector2.ClampMagnitude(this.rvoAgent.CalculatedTargetPoint - this.To2D(position), this.rvoAgent.CalculatedSpeed * deltaTime), 0f);
		}

		public void SetCollisionNormal(Vector3 normal)
		{
			this.rvoAgent.SetCollisionNormal(this.To2D(normal));
		}

		[Obsolete("Set the 'velocity' property instead")]
		public void ForceSetVelocity(Vector3 velocity)
		{
			this.velocity = velocity;
		}

		public Vector2 To2D(Vector3 p)
		{
			float num;
			return this.To2D(p, out num);
		}

		public Vector2 To2D(Vector3 p, out float elevation)
		{
			if (this.movementPlane == MovementPlane.XY)
			{
				elevation = -p.z;
				return new Vector2(p.x, p.y);
			}
			elevation = p.y;
			return new Vector2(p.x, p.z);
		}

		public Vector3 To3D(Vector2 p, float elevationCoordinate)
		{
			if (this.movementPlane == MovementPlane.XY)
			{
				return new Vector3(p.x, p.y, -elevationCoordinate);
			}
			return new Vector3(p.x, elevationCoordinate, p.y);
		}

		private void OnDisable()
		{
			if (this.simulator == null)
			{
				return;
			}
			this.simulator.RemoveAgent(this.rvoAgent);
		}

		private void OnEnable()
		{
			this.tr = base.transform;
			this.ai = base.GetComponent<IAstarAI>();
			AIBase aibase = this.ai as AIBase;
			if (aibase != null)
			{
				aibase.FindComponents();
			}
			if (RVOSimulator.active == null)
			{
				Debug.LogError("No RVOSimulator component found in the scene. Please add one.");
				base.enabled = false;
				return;
			}
			this.simulator = RVOSimulator.active.GetSimulator();
			if (this.rvoAgent != null)
			{
				this.simulator.AddAgent(this.rvoAgent);
				return;
			}
			this.rvoAgent = this.simulator.AddAgent(Vector2.zero, 0f);
			this.rvoAgent.PreCalculationCallback = new Action(this.UpdateAgentProperties);
		}

		protected void UpdateAgentProperties()
		{
			Vector3 localScale = this.tr.localScale;
			this.rvoAgent.Radius = Mathf.Max(0.001f, this.radius * localScale.x);
			this.rvoAgent.AgentTimeHorizon = this.agentTimeHorizon;
			this.rvoAgent.ObstacleTimeHorizon = this.obstacleTimeHorizon;
			this.rvoAgent.Locked = this.locked;
			this.rvoAgent.MaxNeighbours = this.maxNeighbours;
			this.rvoAgent.DebugDraw = this.debug;
			this.rvoAgent.Layer = this.layer;
			this.rvoAgent.CollidesWith = this.collidesWith;
			this.rvoAgent.Priority = this.priority;
			float num;
			this.rvoAgent.Position = this.To2D((this.ai != null) ? this.ai.position : this.tr.position, out num);
			if (this.movementPlane == MovementPlane.XZ)
			{
				this.rvoAgent.Height = this.height * localScale.y;
				this.rvoAgent.ElevationCoordinate = num + (this.center - 0.5f * this.height) * localScale.y;
				return;
			}
			this.rvoAgent.Height = 1f;
			this.rvoAgent.ElevationCoordinate = 0f;
		}

		public void SetTarget(Vector3 pos, float speed, float maxSpeed)
		{
			if (this.simulator == null)
			{
				return;
			}
			this.rvoAgent.SetTarget(this.To2D(pos), speed, maxSpeed);
			if (this.lockWhenNotMoving)
			{
				this.locked = (speed < 0.001f);
			}
		}

		public void Move(Vector3 vel)
		{
			if (this.simulator == null)
			{
				return;
			}
			Vector2 b = this.To2D(vel);
			float magnitude = b.magnitude;
			this.rvoAgent.SetTarget(this.To2D((this.ai != null) ? this.ai.position : this.tr.position) + b, magnitude, magnitude);
			if (this.lockWhenNotMoving)
			{
				this.locked = (magnitude < 0.001f);
			}
		}

		[Obsolete("Use transform.position instead, the RVOController can now handle that without any issues.")]
		public void Teleport(Vector3 pos)
		{
			this.tr.position = pos;
		}

		private void OnDrawGizmos()
		{
			this.tr = base.transform;
			if (this.ai == null)
			{
				Color color = AIBase.ShapeGizmoColor * (this.locked ? 0.5f : 1f);
				Vector3 position = base.transform.position;
				Vector3 localScale = this.tr.localScale;
				if (this.movementPlane == MovementPlane.XY)
				{
					Draw.Gizmos.Cylinder(position, Vector3.forward, 0f, this.radius * localScale.x, color);
					return;
				}
				Draw.Gizmos.Cylinder(position + this.To3D(Vector2.zero, this.center - this.height * 0.5f) * localScale.y, this.To3D(Vector2.zero, 1f), this.height * localScale.y, this.radius * localScale.x, color);
			}
		}

		protected override int OnUpgradeSerializedData(int version, bool unityThread)
		{
			if (version <= 1)
			{
				if (!unityThread)
				{
					return -1;
				}
				if (base.transform.localScale.y != 0f)
				{
					this.centerBackingField /= Mathf.Abs(base.transform.localScale.y);
				}
				if (base.transform.localScale.y != 0f)
				{
					this.heightBackingField /= Mathf.Abs(base.transform.localScale.y);
				}
				if (base.transform.localScale.x != 0f)
				{
					this.radiusBackingField /= Mathf.Abs(base.transform.localScale.x);
				}
			}
			return 2;
		}

		[SerializeField]
		[FormerlySerializedAs("radius")]
		internal float radiusBackingField = 0.5f;

		[SerializeField]
		[FormerlySerializedAs("height")]
		private float heightBackingField = 2f;

		[SerializeField]
		[FormerlySerializedAs("center")]
		private float centerBackingField = 1f;

		[Tooltip("A locked unit cannot move. Other units will still avoid it. But avoidance quality is not the best")]
		public bool locked;

		[Tooltip("Automatically set #locked to true when desired velocity is approximately zero")]
		public bool lockWhenNotMoving;

		[Tooltip("How far into the future to look for collisions with other agents (in seconds)")]
		public float agentTimeHorizon = 2f;

		[Tooltip("How far into the future to look for collisions with obstacles (in seconds)")]
		public float obstacleTimeHorizon = 2f;

		[Tooltip("Max number of other agents to take into account.\nA smaller value can reduce CPU load, a higher value can lead to better local avoidance quality.")]
		public int maxNeighbours = 10;

		public RVOLayer layer = RVOLayer.DefaultAgent;

		[EnumFlag]
		public RVOLayer collidesWith = (RVOLayer)(-1);

		[HideInInspector]
		[Obsolete]
		public float wallAvoidForce = 1f;

		[HideInInspector]
		[Obsolete]
		public float wallAvoidFalloff = 1f;

		[Tooltip("How strongly other agents will avoid this agent")]
		[Range(0f, 1f)]
		public float priority = 0.5f;

		protected Transform tr;

		[SerializeField]
		[FormerlySerializedAs("ai")]
		private IAstarAI aiBackingField;

		public bool debug;
	}
}
