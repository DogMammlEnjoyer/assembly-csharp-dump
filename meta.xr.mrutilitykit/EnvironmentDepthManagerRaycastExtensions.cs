using System;
using System.Diagnostics;
using Meta.XR.EnvironmentDepth;
using UnityEngine;

namespace Meta.XR
{
	internal static class EnvironmentDepthManagerRaycastExtensions
	{
		public static bool Raycast(this EnvironmentDepthManager depthManager, Ray ray, out DepthRaycastHit hitInfo, float maxDistance = 100f, Eye eye = Eye.Both, bool reconstructNormal = true, bool allowOccludedRayOrigin = true)
		{
			EnvironmentDepthManagerRaycastExtensions.EnsureDepthRaycastComponentIsPresent(depthManager);
			if (reconstructNormal)
			{
				Vector3 point;
				Vector3 normal;
				float normalConfidence;
				DepthRaycastResult depthRaycastResult = EnvironmentDepthManagerRaycastExtensions._depthRaycast.Raycast(ray, out point, out normal, out normalConfidence, maxDistance, eye, allowOccludedRayOrigin);
				hitInfo = new DepthRaycastHit
				{
					result = depthRaycastResult,
					point = point,
					normal = normal,
					normalConfidence = normalConfidence
				};
				return depthRaycastResult == DepthRaycastResult.Success;
			}
			ValueTuple<DepthRaycastResult, Vector3, int> valueTuple = EnvironmentDepthManagerRaycastExtensions._depthRaycast.Raycast(ray, maxDistance, eye, allowOccludedRayOrigin);
			hitInfo = new DepthRaycastHit
			{
				result = valueTuple.Item1,
				point = valueTuple.Item2
			};
			return valueTuple.Item1 == DepthRaycastResult.Success;
		}

		public static void SetRaycastWarmUpEnabled(this EnvironmentDepthManager depthManager, bool value)
		{
			EnvironmentDepthManagerRaycastExtensions.EnsureDepthRaycastComponentIsPresent(depthManager);
			EnvironmentDepthManagerRaycastExtensions._depthRaycast._warmUpRaycast = value;
		}

		private static void EnsureDepthRaycastComponentIsPresent(EnvironmentDepthManager depthManager)
		{
			if (EnvironmentDepthManagerRaycastExtensions._depthRaycast == null)
			{
				EnvironmentDepthManagerRaycastExtensions._depthRaycast = depthManager.gameObject.AddComponent<EnvironmentDepthRaycaster>();
				depthManager.onDepthTextureUpdate += EnvironmentDepthManagerRaycastExtensions._depthRaycast.OnDepthTextureUpdate;
				EnvironmentDepthManagerRaycastExtensions._depthRaycast.depthManager = depthManager;
			}
		}

		public unsafe static bool PlaceBox(this IEnvironmentRaycastProvider provider, Ray ray, Vector3 boxSize, Vector3 upwards, out EnvironmentRaycastHit hit, float maxDistance = 100f)
		{
			if (boxSize.x < 0.05f || boxSize.y < 0.05f)
			{
				Debug.LogWarning(string.Format("'x' and 'y' components of the '{0}' should be greater than {1} to determine the surface normal.", "boxSize", 0.05f));
				hit = new EnvironmentRaycastHit
				{
					status = EnvironmentRaycastHitStatus.NoHit
				};
				return false;
			}
			if (boxSize.z < 0f)
			{
				Debug.LogWarning("'z' component of the 'boxSize' should be >= 0f.");
				hit = new EnvironmentRaycastHit
				{
					status = EnvironmentRaycastHitStatus.NoHit
				};
				return false;
			}
			if (!provider.Raycast(ray, out hit, maxDistance, true, true) || hit.normalConfidence < 0.5f)
			{
				return false;
			}
			IntPtr intPtr = stackalloc byte[checked(unchecked((UIntPtr)4) * (UIntPtr)sizeof(Vector3))];
			*intPtr = new Vector3(-1f, -1f);
			*(intPtr + (IntPtr)sizeof(Vector3)) = new Vector3(-1f, 1f);
			*(intPtr + (IntPtr)2 * (IntPtr)sizeof(Vector3)) = new Vector3(1f, 1f);
			*(intPtr + (IntPtr)3 * (IntPtr)sizeof(Vector3)) = new Vector3(1f, -1f);
			Span<Vector3> span = new Span<Vector3>(intPtr, 4);
			for (int i = 0; i < span.Length; i++)
			{
				*span[i] = Vector3.Scale(boxSize * 0.5f, *span[i]);
			}
			Quaternion quaternion = Quaternion.LookRotation(hit.normal, upwards);
			int length = span.Length;
			Span<Vector3> span2 = new Span<Vector3>(stackalloc byte[checked(unchecked((UIntPtr)length) * (UIntPtr)sizeof(Vector3))], length);
			float num = Mathf.Pow(Mathf.Max(boxSize.x, boxSize.y) * 0.2f, 2f);
			for (int j = 0; j < span2.Length; j++)
			{
				Vector3 a = hit.point + quaternion * *span[j];
				EnvironmentRaycastHit environmentRaycastHit;
				if (!provider.Raycast(new Ray(ray.origin, a - ray.origin), out environmentRaycastHit, 100f, true, true))
				{
					return false;
				}
				if (Vector3.Project(environmentRaycastHit.point - hit.point, hit.normal).sqrMagnitude > num)
				{
					return false;
				}
				if (Vector3.Dot(environmentRaycastHit.normal, hit.normal) < 0.6f)
				{
					return false;
				}
				*span2[j] = environmentRaycastHit.point;
			}
			Vector3 a2 = -Vector3.Cross(*span2[1] - *span2[0], *span2[2] - *span2[0]).normalized;
			Vector3 b = -Vector3.Cross(*span2[1] - *span2[3], *span2[2] - *span2[3]).normalized;
			Vector3 vector = Vector3.Normalize(a2 + b);
			if (Vector3.Dot(vector, hit.normal) < 0.9f)
			{
				return false;
			}
			hit.normal = vector;
			quaternion = Quaternion.LookRotation(hit.normal, upwards);
			if (boxSize.z >= 0.05f)
			{
				return !provider.CheckBox(hit.point + hit.normal * (boxSize.z * 0.5f + 0.05f), boxSize * 0.5f, quaternion);
			}
			IntPtr intPtr2 = stackalloc byte[(UIntPtr)48];
			*intPtr2 = 0;
			*(intPtr2 + 4) = 1;
			*(intPtr2 + (IntPtr)2 * 4) = 1;
			*(intPtr2 + (IntPtr)3 * 4) = 2;
			*(intPtr2 + (IntPtr)4 * 4) = 2;
			*(intPtr2 + (IntPtr)5 * 4) = 3;
			*(intPtr2 + (IntPtr)6 * 4) = 3;
			*(intPtr2 + (IntPtr)7 * 4) = 0;
			*(intPtr2 + (IntPtr)8 * 4) = 0;
			*(intPtr2 + (IntPtr)9 * 4) = 2;
			*(intPtr2 + (IntPtr)10 * 4) = 1;
			*(intPtr2 + (IntPtr)11 * 4) = 3;
			Span<int> span3 = new Span<int>(intPtr2, 12);
			for (int k = 0; k < span3.Length; k += 2)
			{
				Vector3 vector2 = hit.point + quaternion * *span[*span3[k]] + hit.normal * 0.05f;
				Vector3 direction = hit.point + quaternion * *span[*span3[k + 1]] + hit.normal * 0.05f - vector2;
				Ray ray2 = new Ray(vector2, direction);
				EnvironmentRaycastHit environmentRaycastHit2;
				if (provider.Raycast(ray2, out environmentRaycastHit2, direction.magnitude, true, true))
				{
					return false;
				}
			}
			return true;
		}

		public unsafe static bool CheckBox(this IEnvironmentRaycastProvider provider, Vector3 center, Vector3 halfExtents, Quaternion orientation)
		{
			IntPtr intPtr = stackalloc byte[checked(unchecked((UIntPtr)3) * (UIntPtr)sizeof(Vector3))];
			*intPtr = Vector3.right;
			*(intPtr + (IntPtr)sizeof(Vector3)) = Vector3.up;
			*(intPtr + (IntPtr)2 * (IntPtr)sizeof(Vector3)) = Vector3.forward;
			Span<Vector3> span = new Span<Vector3>(intPtr, 3);
			int length = span.Length;
			for (int i = 0; i < length; i++)
			{
				for (int j = -1; j <= 1; j += 2)
				{
					Vector3 vector = center - orientation * halfExtents * (float)j;
					Vector3 a = vector + orientation * *span[i] * (halfExtents[i % 3] * 2f) * (float)j;
					Vector3 b = orientation * *span[(i + 1) % length] * (halfExtents[(i + 1) % 3] * 2f) / 2f * (float)j;
					for (int k = 0; k < 3; k++)
					{
						Vector3 direction = a - vector;
						float magnitude = direction.magnitude;
						if (magnitude > 0.01f)
						{
							EnvironmentRaycastHit environmentRaycastHit;
							provider.Raycast(new Ray(vector, direction), out environmentRaycastHit, magnitude, false, false);
							if (environmentRaycastHit.status != EnvironmentRaycastHitStatus.NoHit)
							{
								return true;
							}
						}
						vector += b;
						a += b;
					}
				}
			}
			return false;
		}

		[Conditional("DEBUG_DEPTH_RAYCAST")]
		private static void Log(string msg)
		{
			Debug.Log(string.Format("{0} {1}", Time.frameCount, msg));
		}

		[Conditional("DEBUG_DEPTH_RAYCAST")]
		internal static void DrawLine(Vector3 start, Vector3 end, Color color)
		{
			Debug.DrawLine(start, end, color);
		}

		private const Eye DefaultEye = Eye.Both;

		internal const float MinXYSize = 0.05f;

		private static EnvironmentDepthRaycaster _depthRaycast;
	}
}
