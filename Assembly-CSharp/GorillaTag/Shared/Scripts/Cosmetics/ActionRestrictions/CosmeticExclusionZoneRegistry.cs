using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Shared.Scripts.Cosmetics.ActionRestrictions
{
	public static class CosmeticExclusionZoneRegistry
	{
		public static void Enter(VRRig rig)
		{
			if (rig != null)
			{
				CosmeticExclusionZoneRegistry.restrictedRigs.Add(rig);
			}
		}

		public static void Exit(VRRig rig)
		{
			if (rig != null)
			{
				CosmeticExclusionZoneRegistry.restrictedRigs.Remove(rig);
			}
		}

		public static bool IsRestricted(VRRig rig)
		{
			return rig != null && CosmeticExclusionZoneRegistry.restrictedRigs.Contains(rig);
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Reset()
		{
			CosmeticExclusionZoneRegistry.restrictedRigs.Clear();
		}

		private static readonly HashSet<VRRig> restrictedRigs = new HashSet<VRRig>();
	}
}
