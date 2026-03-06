using System;
using UnityEngine;

namespace Technie.PhysicsCreator
{
	public class UnpackedMesh
	{
		public SkinnedMeshRenderer SkinnedRenderer
		{
			get
			{
				return this.skinnedRenderer;
			}
		}

		public Mesh Mesh
		{
			get
			{
				return this.srcMesh;
			}
		}

		public Transform ModelSpaceTransform
		{
			get
			{
				if (this.skinnedRenderer != null)
				{
					return this.skinnedRenderer.rootBone.parent;
				}
				return this.rigidRenderer.transform;
			}
		}

		public Vector3[] RawVertices
		{
			get
			{
				return this.vertices;
			}
		}

		public Vector3[] ModelSpaceVertices
		{
			get
			{
				return this.modelSpaceVertices;
			}
		}

		public BoneWeight[] BoneWeights
		{
			get
			{
				return this.weights;
			}
		}

		public int NumVertices
		{
			get
			{
				return this.vertices.Length;
			}
		}

		public int[] Indices
		{
			get
			{
				return this.indices;
			}
		}

		public static UnpackedMesh Create(Renderer renderer)
		{
			SkinnedMeshRenderer x = renderer as SkinnedMeshRenderer;
			MeshRenderer x2 = renderer as MeshRenderer;
			if (x != null)
			{
				return new UnpackedMesh(x);
			}
			if (x2 != null)
			{
				return new UnpackedMesh(x2);
			}
			return null;
		}

		public UnpackedMesh(MeshRenderer rigidRenderer)
		{
			this.rigidRenderer = rigidRenderer;
			MeshFilter component = rigidRenderer.GetComponent<MeshFilter>();
			this.srcMesh = ((component != null) ? component.sharedMesh : null);
			if (this.srcMesh != null)
			{
				this.vertices = this.srcMesh.vertices;
				this.normals = this.srcMesh.normals;
				this.indices = this.srcMesh.triangles;
				this.weights = null;
				this.modelSpaceVertices = this.srcMesh.vertices;
			}
		}

		public UnpackedMesh(SkinnedMeshRenderer skinnedRenderer)
		{
			this.skinnedRenderer = skinnedRenderer;
			this.srcMesh = skinnedRenderer.sharedMesh;
			this.vertices = this.srcMesh.vertices;
			this.normals = this.srcMesh.normals;
			this.weights = this.srcMesh.boneWeights;
			this.indices = this.srcMesh.triangles;
			Transform[] bones = skinnedRenderer.bones;
			Transform parent = skinnedRenderer.rootBone.parent;
			Matrix4x4[] bindposes = this.srcMesh.bindposes;
			this.modelSpaceVertices = new Vector3[this.vertices.Length];
			for (int i = 0; i < this.vertices.Length; i++)
			{
				this.modelSpaceVertices[i] = UnpackedMesh.ApplyBindPoseWeighted(this.vertices[i], this.weights[i], bindposes, bones, parent);
			}
		}

		private static Vector3 ApplyBindPoseWeighted(Vector3 inputVertex, BoneWeight weight, Matrix4x4[] bindPoses, Transform[] bones, Transform outputLocalSpace)
		{
			Vector3 position = bindPoses[weight.boneIndex0].MultiplyPoint(inputVertex);
			Vector3 position2 = bindPoses[weight.boneIndex1].MultiplyPoint(inputVertex);
			Vector3 position3 = bindPoses[weight.boneIndex2].MultiplyPoint(inputVertex);
			Vector3 position4 = bindPoses[weight.boneIndex3].MultiplyPoint(inputVertex);
			Vector3 a = bones[weight.boneIndex0].TransformPoint(position);
			Vector3 a2 = bones[weight.boneIndex1].TransformPoint(position2);
			Vector3 a3 = bones[weight.boneIndex2].TransformPoint(position3);
			Vector3 a4 = bones[weight.boneIndex3].TransformPoint(position4);
			Vector3 position5 = a * weight.weight0 + a2 * weight.weight1 + a3 * weight.weight2 + a4 * weight.weight3;
			return outputLocalSpace.InverseTransformPoint(position5);
		}

		private MeshRenderer rigidRenderer;

		private SkinnedMeshRenderer skinnedRenderer;

		private Mesh srcMesh;

		private Vector3[] vertices;

		private Vector3[] normals;

		private BoneWeight[] weights;

		private int[] indices;

		private Vector3[] modelSpaceVertices;
	}
}
