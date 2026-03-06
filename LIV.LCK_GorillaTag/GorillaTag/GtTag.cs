using System;
using System.Collections.Generic;
using UnityEngine;

namespace Liv.Lck.GorillaTag
{
	public class GtTag : MonoBehaviour
	{
		public static bool TryGetTransform(GtTagType gtTagType, out Transform transform)
		{
			return GtTag._cache.TryGetValue(gtTagType, out transform);
		}

		private void Awake()
		{
			base.enabled = false;
		}

		private void OnEnable()
		{
			GtTag._cache.Add(this.gtTagType, base.transform);
		}

		private void OnDisable()
		{
			GtTag._cache.Remove(this.gtTagType);
		}

		public GtTagType gtTagType;

		private static Dictionary<GtTagType, Transform> _cache = new Dictionary<GtTagType, Transform>();
	}
}
