using System;
using Meta.XR.Util;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
	[Feature(Feature.Scene)]
	public abstract class Mask2D : Mask
	{
		private static Float3X3 GenerateAffineTransform(Vector2 position, float rotation, Vector2 scale, Vector2 shear)
		{
			float num = 0.017453292f * rotation;
			float num2 = Mathf.Cos(num);
			Float3X3 float3X = new Float3X3(scale.x, 0f, 0f, 0f, scale.y, 0f, 0f, 0f, 1f);
			float3X = Float3X3.Multiply(new Float3X3(1f, shear.x, 0f, shear.y, 1f, 0f, 0f, 0f, 1f), float3X);
			num = Mathf.Sin(num);
			float3X = Float3X3.Multiply(new Float3X3(num2, -num, 0f, num, num2, 0f, 0f, 0f, 1f), float3X);
			float3X = Float3X3.Multiply(new Float3X3(1f, 0f, position.x, 0f, 1f, position.y, 0f, 0f, 1f), float3X);
			return float3X;
		}

		internal static Float3X3 GenerateAffineTransform(float positionX, float positionY, float rotation, float scaleX, float scaleY, float shearX, float shearY)
		{
			return Mask2D.GenerateAffineTransform(new Vector2(positionX, positionY), rotation, new Vector2(scaleX, scaleY), new Vector2(shearX, shearY));
		}

		[SerializeField]
		public float offsetX;

		[SerializeField]
		public float offsetY;

		[SerializeField]
		public float rotation;

		[SerializeField]
		public float scaleX = 1f;

		[SerializeField]
		public float scaleY = 1f;

		[SerializeField]
		public float shearX;

		[SerializeField]
		public float shearY;
	}
}
