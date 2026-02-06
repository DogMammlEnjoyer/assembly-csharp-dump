using System;
using System.Collections.Generic;
using Fusion.LagCompensation;
using UnityEngine;

namespace Fusion
{
	public struct LagCompensatedHit
	{
		public static explicit operator LagCompensatedHit(RaycastHit raycastHit)
		{
			return new LagCompensatedHit
			{
				Normal = raycastHit.normal,
				Distance = raycastHit.distance,
				Point = raycastHit.point,
				HitboxColliderPosition = default(Vector3),
				HitboxColliderRotation = default(Quaternion),
				GameObject = raycastHit.collider.gameObject,
				Hitbox = null,
				Collider = raycastHit.collider,
				Type = HitType.PhysX
			};
		}

		public static explicit operator LagCompensatedHit(RaycastHit2D raycastHit2D)
		{
			return new LagCompensatedHit
			{
				Normal = raycastHit2D.normal,
				Distance = raycastHit2D.distance,
				Point = raycastHit2D.point,
				HitboxColliderPosition = default(Vector3),
				HitboxColliderRotation = default(Quaternion),
				GameObject = raycastHit2D.collider.gameObject,
				Hitbox = null,
				Collider2D = raycastHit2D.collider,
				Type = HitType.Box2D
			};
		}

		internal static LagCompensatedHit FromHitboxHit(ref HitboxHit hitboxHit)
		{
			return new LagCompensatedHit
			{
				Normal = hitboxHit.Normal,
				Distance = hitboxHit.Distance,
				Point = hitboxHit.Point,
				HitboxColliderPosition = hitboxHit.DebugPosition,
				HitboxColliderRotation = hitboxHit.DebugRotation,
				GameObject = hitboxHit.Hitbox.gameObject,
				Hitbox = hitboxHit.Hitbox,
				Collider = null,
				Type = HitType.Hitbox
			};
		}

		internal static void QuickSort(List<LagCompensatedHit> hits, int low, int high)
		{
			bool flag = low >= high;
			if (!flag)
			{
				float sortAux = hits[high]._sortAux;
				int num = low;
				LagCompensatedHit value;
				for (int i = low; i < high; i++)
				{
					bool flag2 = hits[i]._sortAux >= sortAux;
					if (!flag2)
					{
						value = hits[num];
						hits[num] = hits[i];
						hits[i] = value;
						num++;
					}
				}
				value = hits[num];
				hits[num] = hits[high];
				hits[high] = value;
				LagCompensatedHit.QuickSort(hits, low, num - 1);
				LagCompensatedHit.QuickSort(hits, num + 1, high);
			}
		}

		internal static void QuickSortDistance(List<LagCompensatedHit> hits, int low, int high)
		{
			bool flag = low >= high;
			if (!flag)
			{
				float distance = hits[high].Distance;
				int num = low;
				LagCompensatedHit value;
				for (int i = low; i < high; i++)
				{
					bool flag2 = hits[i].Distance >= distance;
					if (!flag2)
					{
						value = hits[num];
						hits[num] = hits[i];
						hits[i] = value;
						num++;
					}
				}
				value = hits[num];
				hits[num] = hits[high];
				hits[high] = value;
				LagCompensatedHit.QuickSortDistance(hits, low, num - 1);
				LagCompensatedHit.QuickSortDistance(hits, num + 1, high);
			}
		}

		public HitType Type;

		public GameObject GameObject;

		public Vector3 Normal;

		public Vector3 Point;

		public Vector3 HitboxColliderPosition;

		public Quaternion HitboxColliderRotation;

		public float Distance;

		public Hitbox Hitbox;

		public Collider Collider;

		public Collider2D Collider2D;

		internal float _sortAux;
	}
}
