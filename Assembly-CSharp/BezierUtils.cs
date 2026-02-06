using System;
using UnityEngine;

public class BezierUtils
{
	public static Vector3 BezierSolve(float t, Vector3 startPos, Vector3 ctrl1, Vector3 ctrl2, Vector3 endPos)
	{
		float num = 1f - t;
		float d = num * num * num;
		float d2 = 3f * num * num * t;
		float d3 = 3f * num * t * t;
		float d4 = t * t * t;
		return startPos * d + ctrl1 * d2 + ctrl2 * d3 + endPos * d4;
	}
}
