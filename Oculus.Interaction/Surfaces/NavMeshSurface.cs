using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

namespace Oculus.Interaction.Surfaces
{
	public class NavMeshSurface : MonoBehaviour, ISurface
	{
		public float SnapDistance
		{
			get
			{
				return this._snapDistance;
			}
			set
			{
				this._snapDistance = value;
			}
		}

		public float VoxelSize
		{
			get
			{
				return this._voxelSize;
			}
			set
			{
				this._voxelSize = Mathf.Max(0f, value);
			}
		}

		public bool CalculateHitNormals
		{
			get
			{
				return this._calculateNormals;
			}
			set
			{
				this._calculateNormals = value;
			}
		}

		public Transform Transform
		{
			get
			{
				return null;
			}
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			if (!string.IsNullOrEmpty(this._areaName))
			{
				this._areaMask = 1 << NavMesh.GetAreaFromName(this._areaName);
			}
			else
			{
				this._areaMask = -1;
			}
			this._navMeshQuery = new NavMeshQueryFilter
			{
				agentTypeID = NavMesh.GetSettingsByIndex(this._agentIndex).agentTypeID,
				areaMask = this._areaMask
			};
			this.EndStart(ref this._started);
		}

		public bool ClosestSurfacePoint(in Vector3 point, out SurfaceHit surfaceHit, float maxDistance = 0f)
		{
			NavMeshHit navMeshHit;
			if (NavMesh.SamplePosition(point, out navMeshHit, maxDistance + this._snapDistance, this._navMeshQuery))
			{
				surfaceHit = new SurfaceHit
				{
					Point = navMeshHit.position,
					Normal = Vector3.up,
					Distance = navMeshHit.distance
				};
				return true;
			}
			surfaceHit = default(SurfaceHit);
			return false;
		}

		public bool Raycast(in Ray ray, out SurfaceHit surfaceHit, float maxDistance = 0f)
		{
			Ray ray2 = ray;
			Vector3 direction = ray2.direction;
			ray2 = ray;
			Vector3 origin = ray2.origin;
			int num = Mathf.Max(1, Mathf.CeilToInt(Vector3.ProjectOnPlane(origin + direction * maxDistance - origin, Vector3.up).magnitude / this._voxelSize));
			float num2 = maxDistance / (float)num;
			bool flag = false;
			surfaceHit = default(SurfaceHit);
			surfaceHit.Distance = float.PositiveInfinity;
			Vector3 vector = origin + direction * num2 * 0.5f;
			float num3 = Mathf.Max(num2, this._snapDistance);
			float maxDistance2 = num2 + Mathf.Sqrt(num3 * num3 * 2f);
			for (int i = 0; i < num; i++)
			{
				NavMeshHit navMeshHit;
				if (NavMesh.SamplePosition(vector, out navMeshHit, maxDistance2, this._areaMask))
				{
					float num4 = Vector3.Distance(navMeshHit.position, vector);
					if (num4 >= surfaceHit.Distance)
					{
						break;
					}
					flag = true;
					surfaceHit.Distance = num4;
					surfaceHit.Point = navMeshHit.position;
					surfaceHit.Normal = Vector3.up;
				}
				vector += direction * num2;
			}
			if (flag)
			{
				Vector3 point = surfaceHit.Point;
				Vector3 navMeshNormal = this.GetNavMeshNormal(point);
				return this.AlignHits(point, navMeshNormal, ray, ref surfaceHit, maxDistance) && this.SnapSurfaceHit(ref surfaceHit, point);
			}
			return false;
		}

		private bool AlignHits(Vector3 point, Vector3 normal, Ray ray, ref SurfaceHit surfaceHit, float maxDistance)
		{
			Plane plane = new Plane(normal, point);
			float num;
			if (plane.Raycast(ray, out num) && num <= maxDistance)
			{
				surfaceHit.Point = ray.GetPoint(num);
				return true;
			}
			return false;
		}

		private bool SnapSurfaceHit(ref SurfaceHit surfaceHit, Vector3 navMeshPoint)
		{
			NavMeshHit navMeshHit;
			if (NavMesh.Raycast(navMeshPoint, surfaceHit.Point, out navMeshHit, this._navMeshQuery))
			{
				float num = Vector3.Distance(navMeshHit.position, surfaceHit.Point);
				surfaceHit.Point = navMeshHit.position;
				if (num > this._snapDistance)
				{
					return false;
				}
			}
			return true;
		}

		private Vector3 GetNavMeshNormal(Vector3 navMeshPoint)
		{
			if (!this.CalculateHitNormals)
			{
				return Vector3.up;
			}
			Vector3 rhs = this.<GetNavMeshNormal>g__CalculateTangent|25_0(Vector3.right, navMeshPoint);
			return Vector3.Cross(this.<GetNavMeshNormal>g__CalculateTangent|25_0(Vector3.forward, navMeshPoint), rhs);
		}

		private void OpenUnityNavigation()
		{
		}

		public void InjectOptionalAreaName(string areaName)
		{
			this._areaName = areaName;
		}

		public void InjectOptionalAgentIndex(int agentIndex)
		{
			this._agentIndex = agentIndex;
		}

		bool ISurface.Raycast(in Ray ray, out SurfaceHit hit, float maxDistance)
		{
			return this.Raycast(ray, out hit, maxDistance);
		}

		bool ISurface.ClosestSurfacePoint(in Vector3 point, out SurfaceHit hit, float maxDistance)
		{
			return this.ClosestSurfacePoint(point, out hit, maxDistance);
		}

		[CompilerGenerated]
		private Vector3 <GetNavMeshNormal>g__CalculateTangent|25_0(Vector3 direction, Vector3 centre)
		{
			Vector3 a;
			bool flag = this.<GetNavMeshNormal>g__CalculateStep|25_1(centre, direction, out a);
			Vector3 b;
			bool flag2 = this.<GetNavMeshNormal>g__CalculateStep|25_1(centre, -direction, out b);
			if (flag && flag2)
			{
				return (a - b).normalized;
			}
			if (flag)
			{
				return (a - centre).normalized;
			}
			if (flag2)
			{
				return (centre - b).normalized;
			}
			return direction;
		}

		[CompilerGenerated]
		private bool <GetNavMeshNormal>g__CalculateStep|25_1(Vector3 centre, Vector3 stepDir, out Vector3 value)
		{
			NavMeshHit navMeshHit;
			if (NavMesh.SamplePosition(centre + stepDir * this.VoxelSize, out navMeshHit, this.VoxelSize * 2f, this._areaMask))
			{
				value = navMeshHit.position;
				return true;
			}
			value = Vector3.zero;
			return false;
		}

		[SerializeField]
		[Optional]
		[Tooltip("Allows the specification of an area name to be used in association with Unity's NavMesh Areas feature.For more information, see Unity's documentation on NavMesh Areas.")]
		private string _areaName = string.Empty;

		[SerializeField]
		[Optional]
		[Tooltip("Allows the specification of the agent index to be used in association with Unity's NavMesh Agent feature.For more information, see Unity's documentation on NavMesh Agents.")]
		private int _agentIndex;

		[SerializeField]
		[Min(0f)]
		private float _snapDistance;

		[SerializeField]
		[Min(0f)]
		private float _voxelSize = 0.01f;

		[SerializeField]
		private bool _calculateNormals;

		[InspectorButton("OpenUnityNavigation")]
		[SerializeField]
		private string _openUnityNavigation;

		private int _areaMask;

		private NavMeshQueryFilter _navMeshQuery;

		protected bool _started;
	}
}
