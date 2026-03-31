using System;
using UnityEngine;

namespace Voxels
{
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(MeshCollider))]
	public class ChunkComponent : MonoBehaviour
	{
		public VoxelWorld World { get; set; }

		private void Reset()
		{
			this.meshFilter = base.GetComponent<MeshFilter>();
			this.meshRenderer = base.GetComponent<MeshRenderer>();
			this.meshCollider = base.GetComponent<MeshCollider>();
		}

		public MeshFilter meshFilter;

		public MeshRenderer meshRenderer;

		public MeshCollider meshCollider;
	}
}
