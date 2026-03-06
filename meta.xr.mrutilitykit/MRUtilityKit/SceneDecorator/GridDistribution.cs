using System;
using Meta.XR.Util;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
	[Feature(Feature.Scene)]
	[Serializable]
	public class GridDistribution : SceneDecorator.IDistribution
	{
		public void Distribute(SceneDecorator sceneDecorator, MRUKAnchor sceneAnchor, SceneDecoration sceneDecoration)
		{
			Vector3 vector = Vector3.one;
			if (sceneAnchor.PlaneRect != null)
			{
				vector = ((sceneAnchor.PlaneRect != null) ? new Vector3(sceneAnchor.PlaneRect.Value.width, sceneAnchor.PlaneRect.Value.height, 1f) : Vector3.one);
			}
			Bounds? volumeBounds = sceneAnchor.VolumeBounds;
			if (volumeBounds != null)
			{
				volumeBounds = sceneAnchor.VolumeBounds;
				vector = ((volumeBounds != null) ? volumeBounds.GetValueOrDefault().size : Vector3.one);
			}
			Vector2 vector2 = new Vector2(Mathf.Max(Mathf.Ceil(vector.x / this.spacingX), 1f), Mathf.Max(Mathf.Ceil(vector.y / this.spacingY), 1f));
			Vector2 b = vector / vector2;
			int num = 0;
			while ((float)num < vector2.x)
			{
				int num2 = 0;
				while ((float)num2 < vector2.y)
				{
					Vector2 localPos = new Vector2((new Vector2((float)num, (float)num2) * b).x - vector.x / 2f, (new Vector2((float)num, (float)num2) * b).y - vector.y / 2f);
					Vector2 localPosNormalized = new Vector2((new Vector2((float)num, (float)num2) * b).x / vector.x, (new Vector2((float)num, (float)num2) * b).y / vector.y);
					sceneDecorator.GenerateOn(localPos, localPosNormalized, sceneAnchor, sceneDecoration);
					num2++;
				}
				num++;
			}
		}

		[SerializeField]
		private float spacingX = 1f;

		[SerializeField]
		private float spacingY = 1f;
	}
}
