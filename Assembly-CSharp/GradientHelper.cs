using System;
using UnityEngine;

public static class GradientHelper
{
	public static Gradient FromColor(Color color)
	{
		float a = color.a;
		Color col = color;
		col.a = 1f;
		return new Gradient
		{
			colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(col, 1f)
			},
			alphaKeys = new GradientAlphaKey[]
			{
				new GradientAlphaKey(a, 1f)
			}
		};
	}
}
