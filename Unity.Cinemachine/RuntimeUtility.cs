using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	public static class RuntimeUtility
	{
		public static void DestroyObject(Object obj)
		{
			if (obj != null)
			{
				Object.Destroy(obj);
			}
		}

		public static bool IsPrefab(GameObject gameObject)
		{
			return false;
		}

		public static bool RaycastIgnoreTag(Ray ray, out RaycastHit hitInfo, float rayLength, int layerMask, in string ignoreTag)
		{
			if (ignoreTag.Length == 0)
			{
				return Physics.Raycast(ray, out hitInfo, rayLength, layerMask, QueryTriggerInteraction.Ignore);
			}
			int num = -1;
			int num2 = Physics.RaycastNonAlloc(ray, RuntimeUtility.s_HitBuffer, rayLength, layerMask, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < num2; i++)
			{
				if (!RuntimeUtility.s_HitBuffer[i].collider.CompareTag(ignoreTag) && (num < 0 || RuntimeUtility.s_HitBuffer[i].distance < RuntimeUtility.s_HitBuffer[num].distance))
				{
					num = i;
				}
			}
			if (num >= 0)
			{
				hitInfo = RuntimeUtility.s_HitBuffer[num];
				if (num2 == RuntimeUtility.s_HitBuffer.Length)
				{
					RuntimeUtility.s_HitBuffer = new RaycastHit[RuntimeUtility.s_HitBuffer.Length * 2];
				}
				return true;
			}
			hitInfo = default(RaycastHit);
			return false;
		}

		public static bool SphereCastIgnoreTag(Ray ray, float radius, out RaycastHit hitInfo, float rayLength, int layerMask, in string ignoreTag)
		{
			if (radius < 0.0001f)
			{
				return RuntimeUtility.RaycastIgnoreTag(ray, out hitInfo, rayLength, layerMask, ignoreTag);
			}
			Vector3 origin = ray.origin;
			Vector3 direction = ray.direction;
			int num = -1;
			int num2 = 0;
			float num3 = 0f;
			int num4 = Physics.SphereCastNonAlloc(origin, radius, direction, RuntimeUtility.s_HitBuffer, rayLength, layerMask, QueryTriggerInteraction.Ignore);
			for (int i = 0; i < num4; i++)
			{
				RaycastHit raycastHit = RuntimeUtility.s_HitBuffer[i];
				if (ignoreTag.Length <= 0 || !raycastHit.collider.CompareTag(ignoreTag))
				{
					if (raycastHit.distance == 0f && raycastHit.normal == -direction)
					{
						SphereCollider scratchCollider = RuntimeUtility.GetScratchCollider();
						scratchCollider.radius = radius;
						Collider collider = raycastHit.collider;
						Vector3 vector;
						float num5;
						if (!Physics.ComputePenetration(scratchCollider, origin, Quaternion.identity, collider, collider.transform.position, collider.transform.rotation, out vector, out num5))
						{
							goto IL_17F;
						}
						raycastHit.point = origin + vector * (num5 - radius);
						raycastHit.distance = num5 - radius;
						raycastHit.normal = vector;
						RuntimeUtility.s_HitBuffer[i] = raycastHit;
						if (raycastHit.distance < -0.0001f)
						{
							num3 += raycastHit.distance;
							if (RuntimeUtility.s_PenetrationIndexBuffer.Length > num2 + 1)
							{
								RuntimeUtility.s_PenetrationIndexBuffer[num2++] = i;
							}
						}
					}
					if (raycastHit.collider != null && (num < 0 || raycastHit.distance < RuntimeUtility.s_HitBuffer[num].distance))
					{
						num = i;
					}
				}
				IL_17F:;
			}
			if (num2 > 1 && num3 > 0.0001f)
			{
				hitInfo = default(RaycastHit);
				for (int j = 0; j < num2; j++)
				{
					RaycastHit raycastHit2 = RuntimeUtility.s_HitBuffer[RuntimeUtility.s_PenetrationIndexBuffer[j]];
					float num6 = raycastHit2.distance / num3;
					hitInfo.point += raycastHit2.point * num6;
					hitInfo.distance += raycastHit2.distance * num6;
					hitInfo.normal += raycastHit2.normal * num6;
				}
				hitInfo.normal = hitInfo.normal.normalized;
				return true;
			}
			if (num >= 0)
			{
				hitInfo = RuntimeUtility.s_HitBuffer[num];
				return true;
			}
			hitInfo = default(RaycastHit);
			return false;
		}

		public static SphereCollider GetScratchCollider()
		{
			if (RuntimeUtility.s_ScratchColliderGameObject == null)
			{
				RuntimeUtility.s_ScratchColliderGameObject = new GameObject("Cinemachine Scratch Collider");
				RuntimeUtility.s_ScratchColliderGameObject.hideFlags = HideFlags.HideAndDontSave;
				RuntimeUtility.s_ScratchColliderGameObject.transform.position = Vector3.zero;
				RuntimeUtility.s_ScratchColliderGameObject.SetActive(true);
				RuntimeUtility.s_ScratchCollider = RuntimeUtility.s_ScratchColliderGameObject.AddComponent<SphereCollider>();
				RuntimeUtility.s_ScratchCollider.isTrigger = true;
				Rigidbody rigidbody = RuntimeUtility.s_ScratchColliderGameObject.AddComponent<Rigidbody>();
				rigidbody.detectCollisions = false;
				rigidbody.isKinematic = true;
			}
			RuntimeUtility.s_ScratchColliderRefCount++;
			return RuntimeUtility.s_ScratchCollider;
		}

		public static void DestroyScratchCollider()
		{
			if (--RuntimeUtility.s_ScratchColliderRefCount == 0)
			{
				RuntimeUtility.s_ScratchColliderGameObject.SetActive(false);
				RuntimeUtility.DestroyObject(RuntimeUtility.s_ScratchColliderGameObject.GetComponent<Rigidbody>());
				RuntimeUtility.DestroyObject(RuntimeUtility.s_ScratchCollider);
				RuntimeUtility.DestroyObject(RuntimeUtility.s_ScratchColliderGameObject);
				RuntimeUtility.s_ScratchColliderGameObject = null;
				RuntimeUtility.s_ScratchCollider = null;
			}
		}

		public static AnimationCurve NormalizeCurve(AnimationCurve curve, bool normalizeX, bool normalizeY)
		{
			if (!normalizeX && !normalizeY)
			{
				return curve;
			}
			Keyframe[] keys = curve.keys;
			if (keys.Length != 0)
			{
				float num = keys[0].time;
				float num2 = num;
				float num3 = keys[0].value;
				float num4 = num3;
				for (int i = 0; i < keys.Length; i++)
				{
					num = Mathf.Min(num, keys[i].time);
					num2 = Mathf.Max(num2, keys[i].time);
					num3 = Mathf.Min(num3, keys[i].value);
					num4 = Mathf.Max(num4, keys[i].value);
				}
				float num5 = num2 - num;
				float num6 = (num5 < 0.0001f) ? 1f : (1f / num5);
				num5 = num4 - num3;
				float num7 = (num5 < 1f) ? 1f : (1f / num5);
				float num8 = 0f;
				if (num5 < 1f)
				{
					if (num3 > 0f && num3 + num5 <= 1f)
					{
						num8 = num3;
					}
					else
					{
						num8 = 1f - num5;
					}
				}
				for (int j = 0; j < keys.Length; j++)
				{
					if (normalizeX)
					{
						keys[j].time = (keys[j].time - num) * num6;
					}
					if (normalizeY)
					{
						keys[j].value = (keys[j].value - num3) * num7 + num8;
					}
				}
				curve.keys = keys;
			}
			return curve;
		}

		private static RaycastHit[] s_HitBuffer = new RaycastHit[16];

		private static int[] s_PenetrationIndexBuffer = new int[16];

		private static SphereCollider s_ScratchCollider;

		private static GameObject s_ScratchColliderGameObject;

		private static int s_ScratchColliderRefCount;
	}
}
