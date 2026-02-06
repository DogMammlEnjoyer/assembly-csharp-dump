using System;
using UnityEngine;

namespace Fusion
{
	public class NormalizedRectAttribute : PropertyAttribute
	{
		public NormalizedRectAttribute(bool invertY = true, float aspectRatio = 0f)
		{
			this.InvertY = invertY;
			this.AspectRatio = aspectRatio;
		}

		public bool InvertY;

		public float AspectRatio;
	}
}
