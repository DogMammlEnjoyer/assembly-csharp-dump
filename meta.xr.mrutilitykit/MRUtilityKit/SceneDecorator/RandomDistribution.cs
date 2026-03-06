using System;
using Meta.XR.Util;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
	[Feature(Feature.Scene)]
	[Serializable]
	public class RandomDistribution : SceneDecorator.IDistribution
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
			for (int i = Mathf.Max((int)Mathf.Ceil(vector.x * vector.y * this.numPerUnit), 1); i > 0; i--)
			{
				float value = Random.value;
				float value2 = Random.value;
				sceneDecorator.GenerateOn(new Vector2(value * vector.x - vector.x / 2f, value2 * vector.y - vector.y / 2f), new Vector2(value, value2), sceneAnchor, sceneDecoration);
			}
		}

		[SerializeField]
		[Tooltip("How many entries to generate per unit (1m)")]
		private float numPerUnit = 10f;
	}
}
