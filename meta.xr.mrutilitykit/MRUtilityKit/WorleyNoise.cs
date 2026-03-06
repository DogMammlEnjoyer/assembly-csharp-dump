using System;
using Meta.XR.MRUtilityKit.Extensions;
using UnityEngine;

namespace Meta.XR.MRUtilityKit
{
	internal static class WorleyNoise
	{
		private static Vector2 mod289(Vector2 v)
		{
			return new Vector2(v.x - Mathf.Floor(v.x * 0.0034602077f * 289f), v.y - Mathf.Floor(v.y * 0.0034602077f * 289f));
		}

		private static Vector3 mod289(Vector3 v)
		{
			return new Vector3(v.x - Mathf.Floor(v.x * 0.0034602077f * 289f), v.y - Mathf.Floor(v.y * 0.0034602077f * 289f), v.z - Mathf.Floor(v.z * 0.0034602077f * 289f));
		}

		private static Vector3 permute(Vector3 x)
		{
			return WorleyNoise.mod289(Vector3.Scale(new Vector3(x.x * 34f + 1f, x.y * 34f + 1f, x.z * 34f + 1f), x));
		}

		private static float mod7(float v)
		{
			return v - Mathf.Floor(v / 7f) * 7f;
		}

		private static Vector3 mod7(Vector3 v)
		{
			return new Vector3(v.x - Mathf.Floor(v.x / 7f) * 7f, v.y - Mathf.Floor(v.y / 7f) * 7f, v.z - Mathf.Floor(v.z / 7f) * 7f);
		}

		internal static Vector2 cellular(Vector2 P)
		{
			Vector2 vector = WorleyNoise.mod289(P.Floor());
			Vector2 vector2 = P - P.Floor();
			Vector3 a = new Vector3(-1f, 0f, 1f);
			Vector3 a2 = new Vector3(-0.5f, 0.5f, 1.5f);
			Vector3 vector3 = WorleyNoise.permute(a.Add(vector.x));
			Vector3 a3 = WorleyNoise.permute(a.Add(vector3.x).Add(vector.y));
			Vector3 a4 = WorleyNoise.mod289(a3 * 0.14285715f).Subtract(0.42857143f);
			Vector3 a5 = (WorleyNoise.mod7(a3 * 0.14285715f).Floor() * 0.14285715f).Subtract(0.42857143f);
			Vector3 vector4 = a4 * (vector2.x + 0.5f + 1f);
			Vector3 vector5 = a2.Subtract(vector2.y) + 1f * a5;
			Vector3 vector6 = Vector3.Scale(vector4, vector4) + Vector3.Scale(vector5, vector5);
			Vector3 a6 = WorleyNoise.permute(a.Add(vector3.y + vector.y));
			a4 = WorleyNoise.mod289(a6 * 0.14285715f).Subtract(0.42857143f);
			a5 = (WorleyNoise.mod7(a6 * 0.14285715f).Floor() * 0.14285715f).Subtract(0.42857143f);
			Vector3 vector7 = a4 * (vector2.x - 0.5f + 1f);
			vector5 = Vector3.Scale(a5, a2.Subtract(vector2.y)).Add(1f);
			Vector3 vector8 = Vector3.Scale(vector7, vector7) + Vector3.Scale(vector5, vector5);
			Vector3 a7 = WorleyNoise.permute(a.Add(vector3.z + vector.y));
			a4 = WorleyNoise.mod289(a7 * 0.14285715f).Subtract(0.42857143f);
			a5 = WorleyNoise.mod7(a7.Floor() * 0.14285715f * 0.14285715f).Subtract(0.42857143f);
			Vector3 vector9 = a4 * (vector2.x - 1.5f + 1f);
			vector5 = Vector3.Scale(a5, a2.Subtract(vector2.y).Add(1f));
			Vector3 rhs = Vector3.Scale(vector9, vector9) + Vector3.Scale(vector5, vector5);
			Vector3 lhs = Vector3.Min(vector6, vector8);
			vector8 = Vector3.Max(vector6, vector8);
			vector8 = Vector3.Min(vector8, rhs);
			vector6 = Vector3.Min(lhs, vector8);
			vector8 = Vector3.Max(lhs, vector8);
			vector6.x = ((vector6.x < vector6.y) ? vector6.x : vector6.y);
			vector6.y = ((vector6.x < vector6.y) ? vector6.y : vector6.x);
			vector6.x = ((vector6.x < vector6.z) ? vector6.x : vector6.z);
			vector6.z = ((vector6.x < vector6.z) ? vector6.z : vector6.x);
			vector6.y = Mathf.Min(vector6.y, vector8.y);
			vector6.z = Mathf.Min(vector6.z, vector8.z);
			vector6.y = Mathf.Min(vector6.y, vector6.z);
			vector6.y = Mathf.Min(vector6.y, vector8.x);
			return new Vector2(Mathf.Sqrt(vector6.x), Mathf.Sqrt(vector6.y));
		}

		private const float K = 0.14285715f;

		private const float Ko = 0.42857143f;

		private const float jitter = 1f;
	}
}
