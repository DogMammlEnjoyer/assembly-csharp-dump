using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace Voxels
{
	public class VoxelActions_Imported : MonoBehaviour
	{
		private void OnEnable()
		{
			this._voxelWorld = VoxelWorld.GetFor(base.gameObject);
		}

		public void Dig(Vector3 origin)
		{
			VoxelActions_Imported.<>c__DisplayClass7_0 CS$<>8__locals1 = new VoxelActions_Imported.<>c__DisplayClass7_0();
			CS$<>8__locals1.origin = origin;
			CS$<>8__locals1.<>4__this = this;
			UnityEngine.BoundsInt bounds = this.GetBounds(CS$<>8__locals1.origin);
			this._voxelWorld.SetVoxelDensityCustom(bounds, new Func<int3, byte, byte>(CS$<>8__locals1.<Dig>g__DigAt|0));
		}

		public void Add(Vector3 origin)
		{
			VoxelActions_Imported.<>c__DisplayClass8_0 CS$<>8__locals1 = new VoxelActions_Imported.<>c__DisplayClass8_0();
			CS$<>8__locals1.origin = origin;
			CS$<>8__locals1.<>4__this = this;
			UnityEngine.BoundsInt bounds = this.GetBounds(CS$<>8__locals1.origin);
			this._voxelWorld.SetVoxelDensityCustom(bounds, new Func<int3, byte, byte>(CS$<>8__locals1.<Add>g__AddAt|0));
		}

		public void Mine(RaycastHit hit)
		{
			MeshGenerationMode meshGenerationMode = this._voxelWorld.MeshGenerationMode;
			if (meshGenerationMode == MeshGenerationMode.MarchingCubes)
			{
				this.Mine_MarchingCubes(hit);
				return;
			}
			if (meshGenerationMode != MeshGenerationMode.SurfaceNets)
			{
				throw new ArgumentOutOfRangeException();
			}
			this.Mine_SurfaceNets(hit);
		}

		public void UnMine(RaycastHit hit)
		{
			MeshGenerationMode meshGenerationMode = this._voxelWorld.MeshGenerationMode;
			if (meshGenerationMode == MeshGenerationMode.MarchingCubes)
			{
				Debug.LogWarning("UnMine not implemented for Marching Cubes");
				return;
			}
			if (meshGenerationMode != MeshGenerationMode.SurfaceNets)
			{
				throw new ArgumentOutOfRangeException();
			}
			this.UnMine_SurfaceNets(hit);
		}

		private void Mine_MarchingCubes(RaycastHit hit)
		{
			VoxelActions_Imported.<>c__DisplayClass15_0 CS$<>8__locals1 = new VoxelActions_Imported.<>c__DisplayClass15_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.origin = hit.point;
			this._lastHitPoint = CS$<>8__locals1.origin;
			int triangleIndex = hit.triangleIndex;
			if (this._showDebug)
			{
				Debug.DrawLine(Camera.main.transform.position, CS$<>8__locals1.origin, Color.red, 20f);
			}
			MeshCollider meshCollider = hit.collider as MeshCollider;
			if (meshCollider != null)
			{
				Mesh sharedMesh = meshCollider.sharedMesh;
				if (sharedMesh == null || triangleIndex < 0 || triangleIndex >= sharedMesh.triangles.Length / 3)
				{
					Debug.LogWarning(string.Format("Invalid triangle index {0} for mesh {1}", triangleIndex, (sharedMesh != null) ? sharedMesh.name : null));
					return;
				}
				Vector3 a = meshCollider.transform.InverseTransformPoint(CS$<>8__locals1.origin);
				int[] triangles = sharedMesh.triangles;
				Vector3[] vertices = sharedMesh.vertices;
				Vector3 vector = vertices[triangles[triangleIndex * 3]];
				Vector3 vector2 = vertices[triangles[triangleIndex * 3 + 1]];
				Vector3 vector3 = vertices[triangles[triangleIndex * 3 + 2]];
				Vector3 vector4 = ((a - vector).sqrMagnitude < (a - vector2).sqrMagnitude) ? (((a - vector).sqrMagnitude < (a - vector3).sqrMagnitude) ? vector : vector3) : (((a - vector2).sqrMagnitude < (a - vector3).sqrMagnitude) ? vector2 : vector3);
				vector4 = (this._lastVertex = meshCollider.transform.TransformPoint(vector4));
				if (this._showDebug)
				{
					Debug.Log(string.Format("Closest vertex to {0}: {1}", CS$<>8__locals1.origin, vector4));
				}
				if (this._showDebug)
				{
					Debug.DrawLine(CS$<>8__locals1.origin, vector4, Color.blue, 20f);
				}
				CS$<>8__locals1.origin = vector4.SnapToInt();
				if (this._cascade && !this._voxelWorld.GetDensityAt(CS$<>8__locals1.origin).IsSolid())
				{
					if (this._showDebug)
					{
						Debug.Log(string.Format("Hit air at {0}, moving to next voxel", CS$<>8__locals1.origin));
					}
					if (this._showDebug)
					{
						Debug.DrawLine(CS$<>8__locals1.origin, CS$<>8__locals1.origin + (vector4 - CS$<>8__locals1.origin).normalized, Color.cyan, 20f);
					}
					CS$<>8__locals1.origin += (vector4 - CS$<>8__locals1.origin).normalized;
				}
			}
			this._lastGridPoint = CS$<>8__locals1.origin;
			if (this._showDebug)
			{
				Debug.DrawLine(Camera.main.transform.position, CS$<>8__locals1.origin, Color.white, 20f);
			}
			CS$<>8__locals1.total = 0;
			CS$<>8__locals1.dirt = 0;
			CS$<>8__locals1.stone = 0;
			int3 v = CS$<>8__locals1.origin.ToInt3();
			if (this._showDebug)
			{
				Debug.DrawLine(Camera.main.transform.position, v.ToVector3(), Color.yellow, 20f);
			}
			Vector3Int vector3Int = Vector3Int.one * Mathf.CeilToInt(this.miningRadius);
			Vector3Int position = v.ToVectorInt() - vector3Int;
			this._lastBounds = new UnityEngine.BoundsInt(position, vector3Int * 2);
			this._voxelWorld.SetVoxelDataCustom(this._lastBounds, new Func<int3, ValueTuple<byte, byte>, ValueTuple<byte, byte>>(CS$<>8__locals1.<Mine_MarchingCubes>g__MineAt|0));
			Debug.Log(string.Format("Mined {0}S + {1}D = {2} at {3} with bounds {4}", new object[]
			{
				CS$<>8__locals1.stone,
				CS$<>8__locals1.dirt,
				CS$<>8__locals1.total,
				CS$<>8__locals1.origin,
				this._lastBounds
			}));
		}

		private void Mine_SurfaceNets(RaycastHit hit)
		{
			VoxelActions_Imported.<>c__DisplayClass16_0 CS$<>8__locals1 = new VoxelActions_Imported.<>c__DisplayClass16_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.origin = hit.point;
			this._lastHitPoint = CS$<>8__locals1.origin;
			if (this._showDebug)
			{
				Debug.DrawLine(Camera.main.transform.position, CS$<>8__locals1.origin, Color.red, 20f);
			}
			Vector3 triangleCenter = this.GetTriangleCenter(hit);
			CS$<>8__locals1.origin = triangleCenter.SnapToInt();
			if (this._cascade && this._voxelWorld.GetDensityAt(CS$<>8__locals1.origin) == 0)
			{
				if (this._showDebug)
				{
					Debug.Log(string.Format("Hit air at {0}, moving to next voxel", CS$<>8__locals1.origin));
				}
				int3 closestCardinalNeighbour = CS$<>8__locals1.origin.ToInt3().GetClosestCardinalNeighbour(triangleCenter - hit.normal * 0.5f);
				if (this._showDebug)
				{
					Debug.DrawLine(CS$<>8__locals1.origin, closestCardinalNeighbour.ToFloat3(), Color.cyan, 20f);
				}
				CS$<>8__locals1.origin = closestCardinalNeighbour.ToFloat3();
			}
			this._lastGridPoint = CS$<>8__locals1.origin;
			if (this._showDebug)
			{
				Debug.DrawLine(Camera.main.transform.position, CS$<>8__locals1.origin, Color.white, 20f);
			}
			CS$<>8__locals1.total = 0;
			CS$<>8__locals1.dirt = 0;
			CS$<>8__locals1.stone = 0;
			int3 v = CS$<>8__locals1.origin.ToInt3();
			if (this._showDebug)
			{
				Debug.DrawLine(Camera.main.transform.position, v.ToVector3(), Color.yellow, 20f);
			}
			Vector3Int vector3Int = Vector3Int.one * Mathf.CeilToInt(this.miningRadius);
			Vector3Int position = v.ToVectorInt() - vector3Int;
			this._lastBounds = new UnityEngine.BoundsInt(position, vector3Int * 2);
			this._voxelWorld.SetVoxelDataCustom(this._lastBounds, new Func<int3, ValueTuple<byte, byte>, ValueTuple<byte, byte>>(CS$<>8__locals1.<Mine_SurfaceNets>g__MineAt|0));
			Debug.Log(string.Format("Mined {0}S + {1}D = {2} at {3} with bounds {4}", new object[]
			{
				CS$<>8__locals1.stone,
				CS$<>8__locals1.dirt,
				CS$<>8__locals1.total,
				CS$<>8__locals1.origin,
				this._lastBounds
			}));
		}

		private void UnMine_SurfaceNets(RaycastHit hit)
		{
			VoxelActions_Imported.<>c__DisplayClass17_0 CS$<>8__locals1 = new VoxelActions_Imported.<>c__DisplayClass17_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.origin = hit.point;
			this._lastHitPoint = CS$<>8__locals1.origin;
			if (this._showDebug)
			{
				Debug.DrawLine(Camera.main.transform.position, CS$<>8__locals1.origin, Color.red, 20f);
			}
			Vector3 triangleCenter = this.GetTriangleCenter(hit);
			CS$<>8__locals1.origin = triangleCenter.SnapToInt();
			if (this._cascade && this._voxelWorld.GetDensityAt(CS$<>8__locals1.origin) == 255)
			{
				if (this._showDebug)
				{
					Debug.Log(string.Format("Hit solid block at {0}, moving to next voxel", CS$<>8__locals1.origin));
				}
				int3 closestCardinalNeighbour = CS$<>8__locals1.origin.ToInt3().GetClosestCardinalNeighbour(triangleCenter + hit.normal * 0.5f);
				if (this._showDebug)
				{
					Debug.DrawLine(CS$<>8__locals1.origin, closestCardinalNeighbour.ToFloat3(), Color.cyan, 20f);
				}
				CS$<>8__locals1.origin = closestCardinalNeighbour.ToFloat3();
			}
			this._lastGridPoint = CS$<>8__locals1.origin;
			if (this._showDebug)
			{
				Debug.DrawLine(Camera.main.transform.position, CS$<>8__locals1.origin, Color.white, 20f);
			}
			CS$<>8__locals1.total = 0;
			CS$<>8__locals1.dirt = 0;
			CS$<>8__locals1.stone = 0;
			int3 v = CS$<>8__locals1.origin.ToInt3();
			if (this._showDebug)
			{
				Debug.DrawLine(Camera.main.transform.position, v.ToVector3(), Color.yellow, 20f);
			}
			Vector3Int vector3Int = Vector3Int.one * Mathf.CeilToInt(this.miningRadius);
			Vector3Int position = v.ToVectorInt() - vector3Int;
			this._lastBounds = new UnityEngine.BoundsInt(position, vector3Int * 2);
			this._voxelWorld.SetVoxelDataCustom(this._lastBounds, new Func<int3, ValueTuple<byte, byte>, ValueTuple<byte, byte>>(CS$<>8__locals1.<UnMine_SurfaceNets>g__UnMineAt|0));
			Debug.Log(string.Format("UnMined {0}S + {1}D = {2} at {3} with bounds {4}", new object[]
			{
				CS$<>8__locals1.stone,
				CS$<>8__locals1.dirt,
				CS$<>8__locals1.total,
				CS$<>8__locals1.origin,
				this._lastBounds
			}));
		}

		private Vector3 GetTriangleCenter(RaycastHit hit)
		{
			ValueTuple<Vector3, Vector3, Vector3> worldTriangle = this.GetWorldTriangle(hit);
			Vector3 item = worldTriangle.Item1;
			Vector3 item2 = worldTriangle.Item2;
			Vector3 item3 = worldTriangle.Item3;
			return (item + item2 + item3) / 3f;
		}

		[return: TupleElementNames(new string[]
		{
			"v1",
			"v2",
			"v3"
		})]
		private ValueTuple<Vector3, Vector3, Vector3> GetWorldTriangle(RaycastHit hit)
		{
			MeshCollider meshCollider = hit.collider as MeshCollider;
			if (meshCollider == null)
			{
				return new ValueTuple<Vector3, Vector3, Vector3>(Vector3.zero, Vector3.zero, Vector3.zero);
			}
			Mesh sharedMesh = meshCollider.sharedMesh;
			if (sharedMesh == null || hit.triangleIndex < 0 || hit.triangleIndex >= sharedMesh.triangles.Length / 3)
			{
				Debug.LogWarning(string.Format("Invalid triangle index {0} for mesh {1}", hit.triangleIndex, (sharedMesh != null) ? sharedMesh.name : null));
				return new ValueTuple<Vector3, Vector3, Vector3>(Vector3.zero, Vector3.zero, Vector3.zero);
			}
			int[] triangles = sharedMesh.triangles;
			Vector3[] vertices = sharedMesh.vertices;
			Transform transform = meshCollider.transform;
			Vector3 item = transform.TransformPoint(vertices[triangles[hit.triangleIndex * 3]]);
			Vector3 item2 = transform.TransformPoint(vertices[triangles[hit.triangleIndex * 3 + 1]]);
			Vector3 item3 = transform.TransformPoint(vertices[triangles[hit.triangleIndex * 3 + 2]]);
			return new ValueTuple<Vector3, Vector3, Vector3>(item, item2, item3);
		}

		private UnityEngine.BoundsInt GetBounds(float3 point)
		{
			int3 lhs = point.RoundToInt();
			int num = Mathf.CeilToInt(this.radius);
			int3 rhs = num;
			return new UnityEngine.BoundsInt((lhs - rhs).ToVectorInt(), new int3(num * 2).ToVectorInt());
		}

		public float strength = 0.1f;

		public float radius = 3f;

		public float miningStrength = 1f;

		public float miningRadius = 0.5f;

		public byte unmineMaterial;

		private VoxelWorld _voxelWorld;

		private UnityEngine.BoundsInt _lastBounds;

		private Vector3 _lastHitPoint;

		private Vector3 _lastGridPoint;

		private Vector3 _lastVertex;

		[SerializeField]
		private bool _showDebug;

		[SerializeField]
		private bool _centerOnly;

		[SerializeField]
		private bool _cascade;
	}
}
