using System;
using System.Collections.Generic;
using UnityEngine;

namespace Technie.PhysicsCreator.Skinned
{
	[Serializable]
	public class BoneHullData : IHull
	{
		public string Name
		{
			get
			{
				return this.targetBoneName;
			}
		}

		public float MinThreshold
		{
			get
			{
				return this.minThreshold;
			}
		}

		public float MaxThreshold
		{
			get
			{
				return this.maxThreshold;
			}
		}

		public int NumSelectedTriangles
		{
			get
			{
				return this.selectedFaces.Count;
			}
		}

		public Vector3[] CachedTriangleVertices
		{
			get
			{
				return this.cachedTriangleVertices.ToArray();
			}
			set
			{
				this.cachedTriangleVertices.Clear();
				this.cachedTriangleVertices.AddRange(value);
			}
		}

		public bool IsTriangleSelected(int triIndex, Renderer renderer, Mesh targetMesh)
		{
			if (this.type == HullType.Manual)
			{
				return this.selectedFaces.Contains(triIndex);
			}
			if (this.type == HullType.Auto)
			{
				SkinnedMeshRenderer skinnedRenderer = renderer as SkinnedMeshRenderer;
				BoneWeight[] boneWeights = targetMesh.boneWeights;
				int[] triangles = targetMesh.triangles;
				int num = triangles[triIndex * 3];
				int num2 = triangles[triIndex * 3 + 1];
				int num3 = triangles[triIndex * 3 + 2];
				BoneWeight weights = boneWeights[num];
				BoneWeight weights2 = boneWeights[num2];
				BoneWeight weights3 = boneWeights[num3];
				Transform bone = SkinnedColliderCreator.FindBone(skinnedRenderer, this.targetBoneName);
				int ownBoneIndex = Utils.FindBoneIndex(skinnedRenderer, bone);
				if (Utils.IsWeightAboveThreshold(weights, ownBoneIndex, this.minThreshold, this.maxThreshold) && Utils.IsWeightAboveThreshold(weights2, ownBoneIndex, this.minThreshold, this.maxThreshold) && Utils.IsWeightAboveThreshold(weights3, ownBoneIndex, this.minThreshold, this.maxThreshold))
				{
					return true;
				}
			}
			return false;
		}

		public int[] GetSelectedFaces()
		{
			return this.selectedFaces.ToArray();
		}

		public void AddToSelection(int newTriangleIndex, Mesh srcMesh)
		{
			if (this.selectedFaces.Contains(newTriangleIndex))
			{
				return;
			}
			this.selectedFaces.Add(newTriangleIndex);
			Utils.UpdateCachedVertices(this, srcMesh);
		}

		public void RemoveFromSelection(int existingTriangleIndex, Mesh srcMesh)
		{
			this.selectedFaces.Remove(existingTriangleIndex);
			Utils.UpdateCachedVertices(this, srcMesh);
		}

		public void SetMinThreshold(float newMinThreshold)
		{
			this.minThreshold = newMinThreshold;
		}

		public void SetMaxThreshold(float newMaxThreshold)
		{
			this.maxThreshold = newMaxThreshold;
		}

		public void SetThresholds(float newMinThreshold, float newMaxThreshold, SkinnedMeshRenderer renderer, Mesh targetMesh)
		{
			this.minThreshold = newMinThreshold;
			this.maxThreshold = newMaxThreshold;
		}

		public void ClearSelectedFaces()
		{
			if (this.type == HullType.Manual)
			{
				this.selectedFaces.Clear();
				this.cachedTriangleVertices.Clear();
			}
		}

		public void SetSelectedFaces(List<int> newSelectedFaceIndices, Mesh srcMesh)
		{
			if (this.type == HullType.Manual)
			{
				this.selectedFaces.Clear();
				this.selectedFaces.AddRange(newSelectedFaceIndices);
				Utils.UpdateCachedVertices(this, srcMesh);
			}
		}

		public Vector3[] GetCachedTriangleVertices()
		{
			return this.cachedTriangleVertices.ToArray();
		}

		public string targetBoneName;

		public HullType type;

		public ColliderType colliderType;

		public Color previewColour;

		public Mesh hullMesh;

		public PhysicsMaterial material;

		public bool isTrigger;

		[SerializeField]
		private float minThreshold;

		[SerializeField]
		private float maxThreshold;

		[SerializeField]
		private List<int> selectedFaces = new List<int>();

		public List<Vector3> cachedTriangleVertices = new List<Vector3>();
	}
}
