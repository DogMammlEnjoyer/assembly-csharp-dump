using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Shared.Scripts.Cosmetics.ActionRestrictions
{
	public static class CosmeticExclusionZoneRegistryUtility
	{
		public static void RegisterZone(Collider zone)
		{
			if (zone != null && !CosmeticExclusionZoneRegistryUtility.exclusionZones.Contains(zone))
			{
				CosmeticExclusionZoneRegistryUtility.exclusionZones.Add(zone);
			}
		}

		public static void UnregisterZone(Collider zone)
		{
			CosmeticExclusionZoneRegistryUtility.exclusionZones.Remove(zone);
		}

		public static bool IsPositionRestricted(Vector3 worldPos)
		{
			for (int i = 0; i < CosmeticExclusionZoneRegistryUtility.exclusionZones.Count; i++)
			{
				Collider collider = CosmeticExclusionZoneRegistryUtility.exclusionZones[i];
				if (collider != null && collider.bounds.Contains(worldPos))
				{
					return true;
				}
			}
			return false;
		}

		private static readonly List<Collider> exclusionZones = new List<Collider>();
	}
}
