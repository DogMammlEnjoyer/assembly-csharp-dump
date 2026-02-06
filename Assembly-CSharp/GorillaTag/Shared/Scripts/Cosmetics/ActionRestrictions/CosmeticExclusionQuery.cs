using System;
using UnityEngine;

namespace GorillaTag.Shared.Scripts.Cosmetics.ActionRestrictions
{
	public static class CosmeticExclusionQuery
	{
		public static bool IsRestricted(VRRig ownerRig = null, GameObject effectSource = null)
		{
			CosmeticExclusionSource cosmeticExclusionSource;
			return (ownerRig != null && CosmeticExclusionZoneRegistry.IsRestricted(ownerRig)) || (effectSource != null && effectSource.TryGetComponent<CosmeticExclusionSource>(out cosmeticExclusionSource) && cosmeticExclusionSource.IsRestricted());
		}
	}
}
