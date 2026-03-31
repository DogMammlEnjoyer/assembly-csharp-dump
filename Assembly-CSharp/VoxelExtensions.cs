using System;
using System.Runtime.CompilerServices;
using PlayFab.Internal;
using Unity.Mathematics;
using UnityEngine;
using Voxels;

public static class VoxelExtensions
{
	public static void Mine(this VoxelWorld world, Collision collision, VoxelAction action)
	{
		OperationType operation = action.operation;
		if (operation == OperationType.Subtract)
		{
			world.Mine(collision.ToRaycastHit(), action);
			return;
		}
		if (operation != OperationType.Add)
		{
			throw new ArgumentOutOfRangeException();
		}
		world.UnMine(collision.ToRaycastHit(), action);
	}

	public static void Mine(this VoxelWorld world, RaycastHit hit, VoxelAction action)
	{
		MeshGenerationMode meshGenerationMode = world.MeshGenerationMode;
		if (meshGenerationMode == MeshGenerationMode.MarchingCubes)
		{
			world.Mine_MarchingCubes(hit, action);
			return;
		}
		if (meshGenerationMode != MeshGenerationMode.SurfaceNets)
		{
			throw new ArgumentOutOfRangeException();
		}
		world.Mine_SurfaceNets(hit, action);
	}

	public static void UnMine(this VoxelWorld world, RaycastHit hit, VoxelAction action)
	{
		MeshGenerationMode meshGenerationMode = world.MeshGenerationMode;
		if (meshGenerationMode == MeshGenerationMode.MarchingCubes)
		{
			Debug.LogWarning("UnMine not implemented for Marching Cubes");
			return;
		}
		if (meshGenerationMode != MeshGenerationMode.SurfaceNets)
		{
			throw new ArgumentOutOfRangeException();
		}
		world.UnMine_SurfaceNets(hit, action);
	}

	private static void Mine_MarchingCubes(this VoxelWorld world, RaycastHit hit, VoxelAction action)
	{
		VoxelExtensions.<>c__DisplayClass10_0 CS$<>8__locals1 = new VoxelExtensions.<>c__DisplayClass10_0();
		CS$<>8__locals1.action = action;
		VoxelExtensions.<>c__DisplayClass10_0 CS$<>8__locals2 = CS$<>8__locals1;
		CS$<>8__locals2.action.radius = CS$<>8__locals2.action.radius / world.Scale;
		CS$<>8__locals1.origin = hit.point;
		VoxelExtensions._lastHitPoint = CS$<>8__locals1.origin;
		int triangleIndex = hit.triangleIndex;
		if (VoxelExtensions._showDebug)
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
			vector4 = (VoxelExtensions._lastVertex = meshCollider.transform.TransformPoint(vector4));
			if (VoxelExtensions._showDebug)
			{
				Debug.Log(string.Format("Closest vertex to {0}: {1}", CS$<>8__locals1.origin, vector4));
			}
			if (VoxelExtensions._showDebug)
			{
				Debug.DrawLine(CS$<>8__locals1.origin, vector4, Color.blue, 20f);
			}
			vector4 = world.GetLocalPosition(vector4);
			CS$<>8__locals1.origin = vector4.SnapToInt();
			if (VoxelExtensions._cascade && !world.GetDensityAt(CS$<>8__locals1.origin).IsSolid())
			{
				if (VoxelExtensions._showDebug)
				{
					Debug.Log(string.Format("Hit air at {0}, moving to next voxel", CS$<>8__locals1.origin));
				}
				if (VoxelExtensions._showDebug)
				{
					Debug.DrawLine(CS$<>8__locals1.origin, CS$<>8__locals1.origin + (vector4 - CS$<>8__locals1.origin).normalized, Color.cyan, 20f);
				}
				CS$<>8__locals1.origin += (vector4 - CS$<>8__locals1.origin).normalized;
			}
		}
		VoxelExtensions._lastGridPoint = CS$<>8__locals1.origin;
		if (VoxelExtensions._showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, CS$<>8__locals1.origin, Color.white, 20f);
		}
		CS$<>8__locals1.total = 0;
		CS$<>8__locals1.dirt = 0;
		CS$<>8__locals1.stone = 0;
		int3 v = CS$<>8__locals1.origin.ToInt3();
		if (VoxelExtensions._showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, v.ToVector3(), Color.yellow, 20f);
		}
		Vector3Int vector3Int = Vector3Int.one * Mathf.CeilToInt(CS$<>8__locals1.action.radius);
		Vector3Int position = v.ToVectorInt() - vector3Int;
		VoxelExtensions._lastBounds = new UnityEngine.BoundsInt(position, vector3Int * 2);
		world.SetVoxelDataCustom(VoxelExtensions._lastBounds, new Func<int3, ValueTuple<byte, byte>, ValueTuple<byte, byte>>(CS$<>8__locals1.<Mine_MarchingCubes>g__MineAt|0));
		Debug.Log(string.Format("Mined [{0} = {1}S + {2}D] at {3} with bounds {4}", new object[]
		{
			CS$<>8__locals1.total,
			CS$<>8__locals1.stone,
			CS$<>8__locals1.dirt,
			CS$<>8__locals1.origin,
			VoxelExtensions._lastBounds
		}));
		if (CS$<>8__locals1.total > 0)
		{
			SingletonMonoBehaviour<VoxelActions>.instance.PlayDigFX(hit.point, hit.normal, CS$<>8__locals1.dirt, CS$<>8__locals1.stone);
		}
	}

	private static void Mine_SurfaceNets(this VoxelWorld world, RaycastHit hit, VoxelAction action)
	{
		VoxelExtensions.<>c__DisplayClass11_0 CS$<>8__locals1 = new VoxelExtensions.<>c__DisplayClass11_0();
		CS$<>8__locals1.action = action;
		CS$<>8__locals1.origin = hit.point;
		VoxelExtensions._lastHitPoint = CS$<>8__locals1.origin;
		if (VoxelExtensions._showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, CS$<>8__locals1.origin, Color.red, 20f);
		}
		Vector3 localPosition = world.GetLocalPosition(VoxelExtensions.GetTriangleCenter(hit));
		CS$<>8__locals1.origin = localPosition.SnapToInt();
		if (VoxelExtensions._cascade && world.GetDensityAt(CS$<>8__locals1.origin) == 0)
		{
			if (VoxelExtensions._showDebug)
			{
				Debug.Log(string.Format("Hit air at {0}, moving to next voxel", CS$<>8__locals1.origin));
			}
			int3 closestCardinalNeighbour = CS$<>8__locals1.origin.ToInt3().GetClosestCardinalNeighbour(localPosition - hit.normal * 0.5f);
			if (VoxelExtensions._showDebug)
			{
				Debug.DrawLine(CS$<>8__locals1.origin, closestCardinalNeighbour.ToFloat3(), Color.cyan, 20f);
			}
			CS$<>8__locals1.origin = closestCardinalNeighbour.ToFloat3();
		}
		VoxelExtensions._lastGridPoint = CS$<>8__locals1.origin;
		if (VoxelExtensions._showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, CS$<>8__locals1.origin, Color.white, 20f);
		}
		CS$<>8__locals1.total = 0;
		CS$<>8__locals1.dirt = 0;
		CS$<>8__locals1.stone = 0;
		int3 v = CS$<>8__locals1.origin.ToInt3();
		if (VoxelExtensions._showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, v.ToVector3(), Color.yellow, 20f);
		}
		Vector3Int vector3Int = Vector3Int.one * Mathf.CeilToInt(CS$<>8__locals1.action.radius);
		Vector3Int position = v.ToVectorInt() - vector3Int;
		VoxelExtensions._lastBounds = new UnityEngine.BoundsInt(position, vector3Int * 2);
		world.SetVoxelDataCustom(VoxelExtensions._lastBounds, new Func<int3, ValueTuple<byte, byte>, ValueTuple<byte, byte>>(CS$<>8__locals1.<Mine_SurfaceNets>g__MineAt|0));
		Debug.Log(string.Format("Mined {0}S + {1}D = {2} at {3} with bounds {4}", new object[]
		{
			CS$<>8__locals1.stone,
			CS$<>8__locals1.dirt,
			CS$<>8__locals1.total,
			CS$<>8__locals1.origin,
			VoxelExtensions._lastBounds
		}));
		if (CS$<>8__locals1.total > 0)
		{
			SingletonMonoBehaviour<VoxelActions>.instance.PlayDigFX(hit.point, hit.normal, CS$<>8__locals1.dirt, CS$<>8__locals1.stone);
		}
	}

	private static void UnMine_SurfaceNets(this VoxelWorld world, RaycastHit hit, VoxelAction action)
	{
		VoxelExtensions.<>c__DisplayClass12_0 CS$<>8__locals1 = new VoxelExtensions.<>c__DisplayClass12_0();
		CS$<>8__locals1.action = action;
		VoxelExtensions.<>c__DisplayClass12_0 CS$<>8__locals2 = CS$<>8__locals1;
		CS$<>8__locals2.action.radius = CS$<>8__locals2.action.radius / world.Scale;
		CS$<>8__locals1.origin = hit.point;
		VoxelExtensions._lastHitPoint = CS$<>8__locals1.origin;
		if (VoxelExtensions._showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, CS$<>8__locals1.origin, Color.red, 20f);
		}
		Vector3 localPosition = world.GetLocalPosition(VoxelExtensions.GetTriangleCenter(hit));
		CS$<>8__locals1.origin = localPosition.SnapToInt();
		if (VoxelExtensions._cascade && world.GetDensityAt(CS$<>8__locals1.origin) == 255)
		{
			if (VoxelExtensions._showDebug)
			{
				Debug.Log(string.Format("Hit solid block at {0}, moving to next voxel", CS$<>8__locals1.origin));
			}
			int3 closestCardinalNeighbour = CS$<>8__locals1.origin.ToInt3().GetClosestCardinalNeighbour(localPosition + hit.normal * 0.5f);
			if (VoxelExtensions._showDebug)
			{
				Debug.DrawLine(CS$<>8__locals1.origin, closestCardinalNeighbour.ToFloat3(), Color.cyan, 20f);
			}
			CS$<>8__locals1.origin = closestCardinalNeighbour.ToFloat3();
		}
		VoxelExtensions._lastGridPoint = CS$<>8__locals1.origin;
		if (VoxelExtensions._showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, CS$<>8__locals1.origin, Color.white, 20f);
		}
		CS$<>8__locals1.total = 0;
		CS$<>8__locals1.dirt = 0;
		CS$<>8__locals1.stone = 0;
		int3 v = CS$<>8__locals1.origin.ToInt3();
		if (VoxelExtensions._showDebug)
		{
			Debug.DrawLine(Camera.main.transform.position, v.ToVector3(), Color.yellow, 20f);
		}
		Vector3Int vector3Int = Vector3Int.one * Mathf.CeilToInt(CS$<>8__locals1.action.radius);
		Vector3Int position = v.ToVectorInt() - vector3Int;
		VoxelExtensions._lastBounds = new UnityEngine.BoundsInt(position, vector3Int * 2);
		world.SetVoxelDataCustom(VoxelExtensions._lastBounds, new Func<int3, ValueTuple<byte, byte>, ValueTuple<byte, byte>>(CS$<>8__locals1.<UnMine_SurfaceNets>g__UnMineAt|0));
		Debug.Log(string.Format("UnMined {0}S + {1}D = {2} at {3} with bounds {4}", new object[]
		{
			CS$<>8__locals1.stone,
			CS$<>8__locals1.dirt,
			CS$<>8__locals1.total,
			CS$<>8__locals1.origin,
			VoxelExtensions._lastBounds
		}));
	}

	public static void SetVoxel(this VoxelWorld world, int x, int y, int z, byte density, byte materialId)
	{
		VoxelExtensions.<>c__DisplayClass13_0 CS$<>8__locals1 = new VoxelExtensions.<>c__DisplayClass13_0();
		CS$<>8__locals1.density = density;
		CS$<>8__locals1.materialId = materialId;
		Vector3Int position = new Vector3Int(x, y, z);
		UnityEngine.BoundsInt worldBounds = new UnityEngine.BoundsInt(position, Vector3Int.zero);
		world.SetVoxelDataCustom(worldBounds, new Func<int3, ValueTuple<byte, byte>, ValueTuple<byte, byte>>(CS$<>8__locals1.<SetVoxel>g__SetVoxelAt|0));
	}

	public static void SetVoxels(this VoxelWorld world, int3[] voxels, byte density, byte materialId)
	{
		VoxelExtensions.<>c__DisplayClass14_0 CS$<>8__locals1 = new VoxelExtensions.<>c__DisplayClass14_0();
		CS$<>8__locals1.density = density;
		CS$<>8__locals1.materialId = materialId;
		world.SetVoxelDataCustom(voxels, new Func<int3, ValueTuple<byte, byte>, ValueTuple<byte, byte>>(CS$<>8__locals1.<SetVoxels>g__SetVoxelAt|0));
	}

	public static void Dig(this VoxelWorld world, Vector3 position, float radius, float strength)
	{
		VoxelExtensions.<>c__DisplayClass15_0 CS$<>8__locals1 = new VoxelExtensions.<>c__DisplayClass15_0();
		CS$<>8__locals1.position = position;
		CS$<>8__locals1.radius = radius;
		CS$<>8__locals1.strength = strength;
		CS$<>8__locals1.position = world.GetLocalPosition(CS$<>8__locals1.position);
		CS$<>8__locals1.radius /= world.Scale;
		UnityEngine.BoundsInt bounds = world.GetBounds(CS$<>8__locals1.position, CS$<>8__locals1.radius);
		world.SetVoxelDensityCustom(bounds, new Func<int3, byte, byte>(CS$<>8__locals1.<Dig>g__DigAt|0));
	}

	public static void Add(this VoxelWorld world, Vector3 position, float radius, float strength)
	{
		VoxelExtensions.<>c__DisplayClass16_0 CS$<>8__locals1 = new VoxelExtensions.<>c__DisplayClass16_0();
		CS$<>8__locals1.position = position;
		CS$<>8__locals1.radius = radius;
		CS$<>8__locals1.strength = strength;
		CS$<>8__locals1.position = world.GetLocalPosition(CS$<>8__locals1.position);
		CS$<>8__locals1.radius /= world.Scale;
		UnityEngine.BoundsInt bounds = world.GetBounds(CS$<>8__locals1.position, CS$<>8__locals1.radius);
		world.SetVoxelDensityCustom(bounds, new Func<int3, byte, byte>(CS$<>8__locals1.<Add>g__AddAt|0));
	}

	private static UnityEngine.BoundsInt GetBounds(this VoxelWorld world, float3 point, float radius)
	{
		int3 voxelForLocalPosition = world.GetVoxelForLocalPosition(point);
		int num = Mathf.CeilToInt(radius);
		return new UnityEngine.BoundsInt((voxelForLocalPosition - num).ToVectorInt(), new int3(num * 2).ToVectorInt());
	}

	private static Vector3 GetTriangleCenter(RaycastHit hit)
	{
		ValueTuple<Vector3, Vector3, Vector3> worldTriangle = VoxelExtensions.GetWorldTriangle(hit);
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
	private static ValueTuple<Vector3, Vector3, Vector3> GetWorldTriangle(RaycastHit hit)
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

	private static UnityEngine.BoundsInt _lastBounds;

	private static Vector3 _lastHitPoint;

	private static Vector3 _lastGridPoint;

	private static Vector3 _lastVertex;

	private static bool _showDebug;

	private static bool _centerOnly;

	private static bool _cascade = true;
}
