using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.CoreUtils
{
	public static class BoundsUtils
	{
		public static Bounds GetBounds(List<GameObject> gameObjects)
		{
			Bounds? bounds = null;
			foreach (GameObject gameObject in gameObjects)
			{
				Bounds bounds2 = BoundsUtils.GetBounds(gameObject.transform);
				if (bounds == null)
				{
					bounds = new Bounds?(bounds2);
				}
				else
				{
					bounds2.Encapsulate(bounds.Value);
					bounds = new Bounds?(bounds2);
				}
			}
			return bounds.GetValueOrDefault();
		}

		public static Bounds GetBounds(Transform[] transforms)
		{
			Bounds? bounds = null;
			for (int i = 0; i < transforms.Length; i++)
			{
				Bounds bounds2 = BoundsUtils.GetBounds(transforms[i]);
				if (bounds == null)
				{
					bounds = new Bounds?(bounds2);
				}
				else
				{
					bounds2.Encapsulate(bounds.Value);
					bounds = new Bounds?(bounds2);
				}
			}
			return bounds.GetValueOrDefault();
		}

		public static Bounds GetBounds(Transform transform)
		{
			transform.GetComponentsInChildren<Renderer>(BoundsUtils.k_Renderers);
			Bounds bounds = BoundsUtils.GetBounds(BoundsUtils.k_Renderers);
			if (bounds.size == Vector3.zero)
			{
				transform.GetComponentsInChildren<Transform>(BoundsUtils.k_Transforms);
				if (BoundsUtils.k_Transforms.Count > 0)
				{
					bounds.center = BoundsUtils.k_Transforms[0].position;
				}
				foreach (Transform transform2 in BoundsUtils.k_Transforms)
				{
					bounds.Encapsulate(transform2.position);
				}
			}
			return bounds;
		}

		public static Bounds GetBounds(List<Renderer> renderers)
		{
			if (renderers.Count > 0)
			{
				Renderer renderer = renderers[0];
				Bounds result = new Bounds(renderer.transform.position, Vector3.zero);
				foreach (Renderer renderer2 in renderers)
				{
					if (renderer2.bounds.size != Vector3.zero)
					{
						result.Encapsulate(renderer2.bounds);
					}
				}
				return result;
			}
			return default(Bounds);
		}

		public static Bounds GetBounds<T>(List<T> colliders) where T : Collider
		{
			if (colliders.Count > 0)
			{
				T t = colliders[0];
				Bounds result = new Bounds(t.transform.position, Vector3.zero);
				foreach (T t2 in colliders)
				{
					if (t2.bounds.size != Vector3.zero)
					{
						result.Encapsulate(t2.bounds);
					}
				}
				return result;
			}
			return default(Bounds);
		}

		public static Bounds GetBounds(List<Vector3> points)
		{
			Bounds result = default(Bounds);
			if (points.Count < 1)
			{
				return result;
			}
			Vector3 vector = points[0];
			Vector3 vector2 = vector;
			for (int i = 1; i < points.Count; i++)
			{
				Vector3 vector3 = points[i];
				if (vector3.x < vector.x)
				{
					vector.x = vector3.x;
				}
				if (vector3.y < vector.y)
				{
					vector.y = vector3.y;
				}
				if (vector3.z < vector.z)
				{
					vector.z = vector3.z;
				}
				if (vector3.x > vector2.x)
				{
					vector2.x = vector3.x;
				}
				if (vector3.y > vector2.y)
				{
					vector2.y = vector3.y;
				}
				if (vector3.z > vector2.z)
				{
					vector2.z = vector3.z;
				}
			}
			result.SetMinMax(vector, vector2);
			return result;
		}

		private static readonly List<Renderer> k_Renderers = new List<Renderer>();

		private static readonly List<Transform> k_Transforms = new List<Transform>();
	}
}
