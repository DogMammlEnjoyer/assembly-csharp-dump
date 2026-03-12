using System;
using UnityEngine;

namespace GorillaTag
{
	public class SimpleAABB : MonoBehaviour
	{
		private void Awake()
		{
			this.m_bounds = new Bounds(this.m_center, this.m_size);
		}

		public bool IsInBounds(Vector3 point)
		{
			Vector3 point2 = base.transform.InverseTransformPoint(point);
			return this.m_bounds.Contains(point2);
		}

		[SerializeField]
		private Vector3 m_center;

		[SerializeField]
		private Vector3 m_size;

		private Bounds m_bounds;
	}
}
