using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.Universal
{
	internal class ShadowCasterGroup2DManager
	{
		public static List<ShadowCasterGroup2D> shadowCasterGroups
		{
			get
			{
				return ShadowCasterGroup2DManager.s_ShadowCasterGroups;
			}
		}

		public static void CacheValues()
		{
			if (ShadowCasterGroup2DManager.shadowCasterGroups != null)
			{
				for (int i = 0; i < ShadowCasterGroup2DManager.shadowCasterGroups.Count; i++)
				{
					if (ShadowCasterGroup2DManager.shadowCasterGroups[i] != null)
					{
						ShadowCasterGroup2DManager.shadowCasterGroups[i].CacheValues();
					}
				}
			}
		}

		public static void AddShadowCasterGroupToList(ShadowCasterGroup2D shadowCaster, List<ShadowCasterGroup2D> list)
		{
			if (list.Contains(shadowCaster))
			{
				return;
			}
			int num = 0;
			while (num < list.Count && shadowCaster.m_Priority >= list[num].m_Priority)
			{
				num++;
			}
			list.Insert(num, shadowCaster);
		}

		public static void RemoveShadowCasterGroupFromList(ShadowCasterGroup2D shadowCaster, List<ShadowCasterGroup2D> list)
		{
			list.Remove(shadowCaster);
		}

		private static CompositeShadowCaster2D FindTopMostCompositeShadowCaster(ShadowCaster2D shadowCaster)
		{
			CompositeShadowCaster2D result = null;
			Transform parent = shadowCaster.transform.parent;
			while (parent != null)
			{
				CompositeShadowCaster2D compositeShadowCaster2D;
				if (parent.TryGetComponent<CompositeShadowCaster2D>(out compositeShadowCaster2D))
				{
					result = compositeShadowCaster2D;
				}
				parent = parent.parent;
			}
			return result;
		}

		public static int GetRendereringPriority(ShadowCaster2D shadowCaster)
		{
			int result = 0;
			Renderer renderer;
			if (shadowCaster.TryGetComponent<Renderer>(out renderer))
			{
				result = renderer.sortingOrder;
			}
			return result;
		}

		public static bool AddToShadowCasterGroup(ShadowCaster2D shadowCaster, ref ShadowCasterGroup2D shadowCasterGroup, ref int priority)
		{
			ShadowCasterGroup2D shadowCasterGroup2D = ShadowCasterGroup2DManager.FindTopMostCompositeShadowCaster(shadowCaster);
			int num = 0;
			if (shadowCasterGroup2D == null)
			{
				num = ShadowCasterGroup2DManager.GetRendereringPriority(shadowCaster);
				shadowCaster.TryGetComponent<ShadowCasterGroup2D>(out shadowCasterGroup2D);
			}
			if (shadowCasterGroup2D != null && (shadowCasterGroup != shadowCasterGroup2D || priority != num))
			{
				shadowCasterGroup2D.RegisterShadowCaster2D(shadowCaster);
				shadowCasterGroup = shadowCasterGroup2D;
				priority = num;
				return true;
			}
			return false;
		}

		public static void RemoveFromShadowCasterGroup(ShadowCaster2D shadowCaster, ShadowCasterGroup2D shadowCasterGroup)
		{
			if (shadowCasterGroup != null)
			{
				shadowCasterGroup.UnregisterShadowCaster2D(shadowCaster);
			}
			if (shadowCasterGroup == shadowCaster)
			{
				ShadowCasterGroup2DManager.RemoveGroup(shadowCasterGroup);
			}
		}

		public static void AddGroup(ShadowCasterGroup2D group)
		{
			if (group == null)
			{
				return;
			}
			if (ShadowCasterGroup2DManager.s_ShadowCasterGroups == null)
			{
				ShadowCasterGroup2DManager.s_ShadowCasterGroups = new List<ShadowCasterGroup2D>();
			}
			ShadowCasterGroup2DManager.AddShadowCasterGroupToList(group, ShadowCasterGroup2DManager.s_ShadowCasterGroups);
		}

		public static void RemoveGroup(ShadowCasterGroup2D group)
		{
			if (group != null && ShadowCasterGroup2DManager.s_ShadowCasterGroups != null)
			{
				ShadowCasterGroup2DManager.RemoveShadowCasterGroupFromList(group, ShadowCasterGroup2DManager.s_ShadowCasterGroups);
			}
		}

		private static List<ShadowCasterGroup2D> s_ShadowCasterGroups;
	}
}
