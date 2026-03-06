using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class CosmeticWardrobeProximityDetector : MonoBehaviour
{
	private void OnEnable()
	{
		if (this.wardrobeNearbyCollider != null)
		{
			CosmeticWardrobeProximityDetector.wardrobeNearbyDetection.Add(this.wardrobeNearbyCollider);
		}
	}

	private void OnDisable()
	{
		if (this.wardrobeNearbyCollider != null)
		{
			CosmeticWardrobeProximityDetector.wardrobeNearbyDetection.Remove(this.wardrobeNearbyCollider);
		}
	}

	public static bool IsUserNearWardrobe(int actorNr)
	{
		LayerMask.GetMask(new string[]
		{
			"Gorilla Tag Collider"
		});
		LayerMask.GetMask(new string[]
		{
			"Gorilla Body Collider"
		});
		VRRigCache.Instance.GetActiveRigs(CosmeticWardrobeProximityDetector.rigs);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetPlayer.Get(actorNr).GetPlayerRef(), out rigContainer))
		{
			return false;
		}
		foreach (SphereCollider sphereCollider in CosmeticWardrobeProximityDetector.wardrobeNearbyDetection)
		{
			if ((rigContainer.HeadCollider.transform.position - sphereCollider.transform.position).magnitude <= sphereCollider.radius)
			{
				return true;
			}
		}
		return false;
	}

	[SerializeField]
	private SphereCollider wardrobeNearbyCollider;

	private static List<VRRig> rigs = new List<VRRig>();

	private static List<SphereCollider> wardrobeNearbyDetection = new List<SphereCollider>();

	private static readonly Collider[] overlapColliders = new Collider[20];
}
