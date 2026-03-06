using System;
using Meta.XR.Util;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
	[Feature(Feature.Scene)]
	[Serializable]
	public class StaggeredConcentricDistribution : SceneDecorator.IDistribution
	{
		public void Distribute(SceneDecorator sceneDecorator, MRUKAnchor sceneAnchor, SceneDecoration sceneDecoration)
		{
			Vector2 vector = new Vector2(-1f, -1f);
			Vector2 a = new Vector2(-1f, 1f);
			Vector2 a2 = a - vector;
			float magnitude = a2.magnitude;
			float num = Mathf.Sqrt(magnitude);
			float sqrMagnitude = vector.sqrMagnitude;
			float num2 = a.sqrMagnitude;
			int num3 = (int)Mathf.Floor((sqrMagnitude + 0.70710677f) / this.stepSize);
			int num4 = (int)Mathf.Floor((num2 + 0.70710677f) / this.stepSize);
			num2 -= 0.70710677f;
			for (int i = (int)Mathf.Ceil((sqrMagnitude - 0.70710677f) / this.stepSize); i < num3; i++)
			{
				float num5 = this.stepSize * (float)i;
				for (int j = (int)Mathf.Ceil(Mathf.Max(num2, num - num5) / this.stepSize); j < num4; j++)
				{
					float num6 = this.stepSize * (float)j;
					float num7 = num5 + ((j % 3 != 0) ? 0f : (0.15f * this.stepSize));
					float num8 = num6 + ((i % 3 != 0) ? 0f : (0.15f * this.stepSize));
					if (num <= num7 + num8)
					{
						float num9 = num7 * num7;
						float num10 = (num9 - num8 * num8 + magnitude) / (2f * num);
						Vector2 vector2 = vector + num10 * a2 / num;
						Vector2 vector3 = a2 * Mathf.Sqrt(num9 - num10 * num10) / num;
						Vector2 vector4 = new Vector2(vector2.x + vector3.y, vector2.y - vector3.x);
						vector2 = new Vector2(vector2.x - vector3.y, vector2.y + vector3.x);
						sceneDecorator.GenerateOn(vector2, vector2, sceneAnchor, sceneDecoration);
						if (vector4 != vector2)
						{
							sceneDecorator.GenerateOn(vector4, vector4, sceneAnchor, sceneDecoration);
						}
					}
				}
			}
		}

		[SerializeField]
		public float stepSize;

		private const float regionRadius = 0.70710677f;
	}
}
