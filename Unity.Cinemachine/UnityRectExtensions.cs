using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	public static class UnityRectExtensions
	{
		public static Rect Inflated(this Rect r, Vector2 delta)
		{
			if (r.width + delta.x * 2f < 0f)
			{
				delta.x = -r.width / 2f;
			}
			if (r.height + delta.y * 2f < 0f)
			{
				delta.y = -r.height / 2f;
			}
			return new Rect(r.xMin - delta.x, r.yMin - delta.y, r.width + delta.x * 2f, r.height + delta.y * 2f);
		}
	}
}
