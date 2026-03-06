using System;
using Meta.XR.Util;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
	[Feature(Feature.Scene)]
	public class ColliderMask : Mask
	{
		public override float SampleMask(Candidate c)
		{
			return 0f;
		}

		public override bool Check(Candidate c)
		{
			Collider componentInChildren = c.decorationPrefab.GetComponentInChildren<Collider>();
			if (componentInChildren == null)
			{
				return true;
			}
			Collider[] array = new Collider[this.MaxCheckColliders];
			BoxCollider boxCollider = componentInChildren as BoxCollider;
			if (boxCollider != null)
			{
				int size = Physics.OverlapBoxNonAlloc(c.hit.point, boxCollider.size / 2f, array, Quaternion.identity, this.CheckLayers);
				return this.CheckColliderHitsForMRUK(array, size);
			}
			MeshCollider meshCollider = componentInChildren as MeshCollider;
			if (meshCollider != null)
			{
				BoxCollider boxCollider2 = c.decorationPrefab.AddComponent<BoxCollider>();
				boxCollider2.center = meshCollider.bounds.center - c.decorationPrefab.transform.position;
				boxCollider2.size = meshCollider.bounds.size;
				int size2 = Physics.OverlapBoxNonAlloc(c.hit.point, boxCollider2.size / 2f, array, Quaternion.identity, this.CheckLayers);
				Object.Destroy(boxCollider2);
				return this.CheckColliderHitsForMRUK(array, size2);
			}
			CapsuleCollider capsuleCollider = componentInChildren as CapsuleCollider;
			if (capsuleCollider != null)
			{
				int size3 = Physics.OverlapCapsuleNonAlloc(capsuleCollider.transform.position, c.hit.point + Vector3.up * capsuleCollider.height, capsuleCollider.radius, array, this.CheckLayers);
				return this.CheckColliderHitsForMRUK(array, size3);
			}
			SphereCollider sphereCollider = componentInChildren as SphereCollider;
			if (sphereCollider != null)
			{
				int size4 = Physics.OverlapSphereNonAlloc(c.hit.point, sphereCollider.radius, array, this.CheckLayers);
				return this.CheckColliderHitsForMRUK(array, size4);
			}
			return false;
		}

		private bool CheckColliderHitsForMRUK(Collider[] colliders, int size)
		{
			bool result = true;
			for (int i = 0; i < this.MaxCheckColliders; i++)
			{
				if (!(colliders[i] == null))
				{
					if (colliders[i].gameObject.GetComponentsInParent<MRUKAnchor>().Length == 0)
					{
						result = false;
						break;
					}
					MRUKAnchor mrukanchor = colliders[i].gameObject.GetComponentsInParent<MRUKAnchor>()[0];
					if (mrukanchor.Label == MRUKAnchor.SceneLabels.FLOOR)
					{
						result = this.IgnoreFloorCollision;
					}
					if (mrukanchor.Label == MRUKAnchor.SceneLabels.GLOBAL_MESH)
					{
						result = this.IgnoreGlobalMeshCollision;
					}
				}
			}
			return result;
		}

		[SerializeField]
		private int MaxCheckColliders = 10;

		[SerializeField]
		private bool IgnoreFloorCollision = true;

		[SerializeField]
		private bool IgnoreGlobalMeshCollision = true;

		[SerializeField]
		private LayerMask CheckLayers = -1;
	}
}
