using System;
using System.Collections.Generic;
using UnityEngine;

public class SkinnedRoundedBoxMesh : MonoBehaviour
{
	private void Start()
	{
		if (this.generateOnStart)
		{
			SkinnedRoundedBoxMesh.GenerateMesh(this._cornerSegmentCount, this._borderRadius, this._cylinderFaceCount, this._cylinderRadius, this._skinnedMeshRenderer, this._topLeft, this._topRight, this._bottomLeft, this._bottomRight);
		}
	}

	public static Vector2[] GenerateArcPath(float startAngle, float endAngle, int steps, float radius, bool closed)
	{
		float num = (endAngle - startAngle) / (float)steps;
		List<Vector2> list = new List<Vector2>();
		int num2 = closed ? (steps + 1) : steps;
		for (int i = 0; i < num2; i++)
		{
			Vector2 a = new Vector2(Mathf.Cos(startAngle + num * (float)i), Mathf.Sin(startAngle + num * (float)i));
			list.Add(a * radius);
		}
		return list.ToArray();
	}

	public static Vector3[] GenerateCylinderAroundPath(List<Vector2> path, int cylinderFaceCount, float cylinderRadius)
	{
		Vector2[] array = SkinnedRoundedBoxMesh.GenerateArcPath(0f, 6.2831855f, cylinderFaceCount, cylinderRadius, false);
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < path.Count; i++)
		{
			Vector2 vector = path[i];
			Vector3 a = Vector3.Normalize(Vector3.Cross(Vector3.Normalize(path[(i + 1) % path.Count] - vector), new Vector3(0f, 0f, 1f)));
			for (int j = 0; j < array.Length; j++)
			{
				Vector3 item = vector + array[j].x * a + array[j].y * Vector3.forward;
				list.Add(item);
			}
		}
		return list.ToArray();
	}

	public static int[] GenerateCylinderIndices(int cornerSegmentCount, int cylinderFaceCount)
	{
		List<int> list = new List<int>();
		int num = cornerSegmentCount * 4;
		for (int i = 0; i < num; i++)
		{
			int num2 = cylinderFaceCount * i;
			int num3 = cylinderFaceCount * ((i + 1) % num);
			for (int j = 0; j < cylinderFaceCount; j++)
			{
				int num4 = j;
				int num5 = (j + 1) % cylinderFaceCount;
				list.Add(num2 + num4);
				list.Add(num3 + num4);
				list.Add(num3 + num5);
				list.Add(num2 + num4);
				list.Add(num3 + num5);
				list.Add(num2 + num5);
			}
		}
		return list.ToArray();
	}

	private static void PushBoneWeigth(int boneIndex, List<BoneWeight> weights, int cornerSegmentCount, int cylinderFaceCount)
	{
		int num = cornerSegmentCount * cylinderFaceCount;
		for (int i = 0; i < num; i++)
		{
			weights.Add(new BoneWeight
			{
				boneIndex0 = boneIndex,
				weight0 = 1f,
				boneIndex1 = 0,
				boneIndex2 = 0,
				boneIndex3 = 0
			});
		}
	}

	public static void GenerateMesh(int cornerSegmentCount, float borderRadius, int cylinderFaceCount, float cylinderRadius, SkinnedMeshRenderer skinnedMeshRenderer, Transform topLeft, Transform topRight, Transform bottomLeft, Transform bottomRight)
	{
		Mesh mesh = new Mesh();
		List<Vector2> list = new List<Vector2>();
		list.AddRange(SkinnedRoundedBoxMesh.GenerateArcPath(3.1415927f, 1.5707964f, cornerSegmentCount, borderRadius, false));
		list.AddRange(SkinnedRoundedBoxMesh.GenerateArcPath(1.5707964f, 0f, cornerSegmentCount, borderRadius, false));
		list.AddRange(SkinnedRoundedBoxMesh.GenerateArcPath(0f, -1.5707964f, cornerSegmentCount, borderRadius, false));
		list.AddRange(SkinnedRoundedBoxMesh.GenerateArcPath(-1.5707964f, -3.1415927f, cornerSegmentCount, borderRadius, false));
		Vector3[] vertices = SkinnedRoundedBoxMesh.GenerateCylinderAroundPath(list, cylinderFaceCount, cylinderRadius);
		int[] indices = SkinnedRoundedBoxMesh.GenerateCylinderIndices(cornerSegmentCount, cylinderFaceCount);
		List<BoneWeight> list2 = new List<BoneWeight>();
		SkinnedRoundedBoxMesh.PushBoneWeigth(0, list2, cornerSegmentCount, cylinderFaceCount);
		SkinnedRoundedBoxMesh.PushBoneWeigth(1, list2, cornerSegmentCount, cylinderFaceCount);
		SkinnedRoundedBoxMesh.PushBoneWeigth(3, list2, cornerSegmentCount, cylinderFaceCount);
		SkinnedRoundedBoxMesh.PushBoneWeigth(2, list2, cornerSegmentCount, cylinderFaceCount);
		mesh.vertices = vertices;
		mesh.SetIndices(indices, MeshTopology.Triangles, 0);
		mesh.boneWeights = list2.ToArray();
		mesh.bindposes = new Matrix4x4[]
		{
			Matrix4x4.identity,
			Matrix4x4.identity,
			Matrix4x4.identity,
			Matrix4x4.identity
		};
		mesh.name = "SkinnedRoundedBoxMesh";
		skinnedMeshRenderer.bones = new Transform[]
		{
			topLeft,
			topRight,
			bottomLeft,
			bottomRight
		};
		skinnedMeshRenderer.rootBone = topLeft;
		skinnedMeshRenderer.sharedMesh = mesh;
		mesh.RecalculateBounds();
	}

	[ContextMenu("Generate Mesh")]
	public void GenerateMeshFromMenu()
	{
		SkinnedRoundedBoxMesh.GenerateMesh(this._cornerSegmentCount, this._borderRadius, this._cylinderFaceCount, this._cylinderRadius, this._skinnedMeshRenderer, this._topLeft, this._topRight, this._bottomLeft, this._bottomRight);
	}

	[SerializeField]
	private Transform _topLeft;

	[SerializeField]
	private Transform _topRight;

	[SerializeField]
	private Transform _bottomLeft;

	[SerializeField]
	private Transform _bottomRight;

	[SerializeField]
	private int _cornerSegmentCount;

	[SerializeField]
	private int _cylinderFaceCount;

	[SerializeField]
	private float _borderRadius;

	[SerializeField]
	private float _cylinderRadius;

	[SerializeField]
	private SkinnedMeshRenderer _skinnedMeshRenderer;

	public bool generateOnStart;
}
