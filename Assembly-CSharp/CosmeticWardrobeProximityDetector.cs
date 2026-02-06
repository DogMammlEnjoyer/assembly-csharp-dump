using System;
using System.Collections.Generic;
using GorillaLocomotion;
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

	public static bool IsUserNearWardrobe(string userID)
	{
		int layerMask = LayerMask.GetMask(new string[]
		{
			"Gorilla Tag Collider"
		}) | LayerMask.GetMask(new string[]
		{
			"Gorilla Body Collider"
		});
		foreach (SphereCollider sphereCollider in CosmeticWardrobeProximityDetector.wardrobeNearbyDetection)
		{
			int num = Physics.OverlapSphereNonAlloc(sphereCollider.transform.position, sphereCollider.radius, CosmeticWardrobeProximityDetector.overlapColliders, layerMask);
			num = Mathf.Min(num, CosmeticWardrobeProximityDetector.overlapColliders.Length);
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					Collider collider = CosmeticWardrobeProximityDetector.overlapColliders[i];
					if (!(collider == null))
					{
						GameObject gameObject = collider.attachedRigidbody.gameObject;
						VRRig component = gameObject.GetComponent<VRRig>();
						if (component == null || component.creator == null || component.creator.IsNull || string.IsNullOrEmpty(component.creator.UserId))
						{
							if (gameObject.GetComponent<GTPlayer>() == null || NetworkSystem.Instance.LocalPlayer == null)
							{
								goto IL_135;
							}
							if (userID == NetworkSystem.Instance.LocalPlayer.UserId)
							{
								return true;
							}
						}
						else if (userID == component.creator.UserId)
						{
							return true;
						}
						CosmeticWardrobeProximityDetector.overlapColliders[i] = null;
					}
					IL_135:;
				}
			}
		}
		return false;
	}

	[SerializeField]
	private SphereCollider wardrobeNearbyCollider;

	private static List<SphereCollider> wardrobeNearbyDetection = new List<SphereCollider>();

	private static readonly Collider[] overlapColliders = new Collider[20];
}
