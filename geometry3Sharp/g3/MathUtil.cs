using System;
using System.Collections.Generic;

namespace g3
{
	public static class MathUtil
	{
		public static bool IsFinite(double d)
		{
			return !double.IsInfinity(d) && !double.IsNaN(d);
		}

		public static bool IsFinite(float d)
		{
			return !float.IsInfinity(d) && !float.IsNaN(d);
		}

		public static bool EpsilonEqual(double a, double b, double epsilon = 2.220446049250313E-16)
		{
			return Math.Abs(a - b) <= epsilon;
		}

		public static bool EpsilonEqual(float a, float b, float epsilon = 1.1920929E-07f)
		{
			return Math.Abs(a - b) <= epsilon;
		}

		public static T Clamp<T>(T f, T low, T high) where T : IComparable
		{
			if (f.CompareTo(low) < 0)
			{
				return low;
			}
			if (f.CompareTo(high) > 0)
			{
				return high;
			}
			return f;
		}

		public static float Clamp(float f, float low, float high)
		{
			if (f < low)
			{
				return low;
			}
			if (f <= high)
			{
				return f;
			}
			return high;
		}

		public static double Clamp(double f, double low, double high)
		{
			if (f < low)
			{
				return low;
			}
			if (f <= high)
			{
				return f;
			}
			return high;
		}

		public static int Clamp(int f, int low, int high)
		{
			if (f < low)
			{
				return low;
			}
			if (f <= high)
			{
				return f;
			}
			return high;
		}

		public static int ModuloClamp(int f, int N)
		{
			while (f < 0)
			{
				f += N;
			}
			return f % N;
		}

		public static float RangeClamp(float fValue, float fMinMaxValue)
		{
			return MathUtil.Clamp(fValue, -Math.Abs(fMinMaxValue), Math.Abs(fMinMaxValue));
		}

		public static double RangeClamp(double fValue, double fMinMaxValue)
		{
			return MathUtil.Clamp(fValue, -Math.Abs(fMinMaxValue), Math.Abs(fMinMaxValue));
		}

		public static float SignedClamp(float f, float fMax)
		{
			return MathUtil.Clamp(Math.Abs(f), 0f, fMax) * (float)Math.Sign(f);
		}

		public static double SignedClamp(double f, double fMax)
		{
			return MathUtil.Clamp(Math.Abs(f), 0.0, fMax) * (double)Math.Sign(f);
		}

		public static float SignedClamp(float f, float fMin, float fMax)
		{
			return MathUtil.Clamp(Math.Abs(f), fMin, fMax) * (float)Math.Sign(f);
		}

		public static double SignedClamp(double f, double fMin, double fMax)
		{
			return MathUtil.Clamp(Math.Abs(f), fMin, fMax) * (double)Math.Sign(f);
		}

		public static bool InRange(float f, float low, float high)
		{
			return f >= low && f <= high;
		}

		public static bool InRange(double f, double low, double high)
		{
			return f >= low && f <= high;
		}

		public static bool InRange(int f, int low, int high)
		{
			return f >= low && f <= high;
		}

		public static double ClampAngleDeg(double theta, double min, double max)
		{
			double num = (min + max) * 0.5;
			double num2 = max - num;
			theta %= 360.0;
			theta -= num;
			if (theta < -180.0)
			{
				theta += 360.0;
			}
			else if (theta > 180.0)
			{
				theta -= 360.0;
			}
			if (theta < -num2)
			{
				theta = -num2;
			}
			else if (theta > num2)
			{
				theta = num2;
			}
			return theta + num;
		}

		public static double ClampAngleRad(double theta, double min, double max)
		{
			double num = (min + max) * 0.5;
			double num2 = max - num;
			theta %= 6.283185307179586;
			theta -= num;
			if (theta < -3.141592653589793)
			{
				theta += 6.283185307179586;
			}
			else if (theta > 3.141592653589793)
			{
				theta -= 6.283185307179586;
			}
			if (theta < -num2)
			{
				theta = -num2;
			}
			else if (theta > num2)
			{
				theta = num2;
			}
			return theta + num;
		}

		public static int WrapSignedIndex(int val, int mod)
		{
			while (val < 0)
			{
				val += mod;
			}
			return val % mod;
		}

		public static void MinMax(double a, double b, double c, out double min, out double max)
		{
			if (a < b)
			{
				if (a < c)
				{
					min = a;
					max = Math.Max(b, c);
					return;
				}
				min = c;
				max = b;
				return;
			}
			else
			{
				if (a > c)
				{
					max = a;
					min = Math.Min(b, c);
					return;
				}
				min = b;
				max = c;
				return;
			}
		}

		public static double Min(double a, double b, double c)
		{
			return Math.Min(a, Math.Min(b, c));
		}

		public static float Min(float a, float b, float c)
		{
			return Math.Min(a, Math.Min(b, c));
		}

		public static int Min(int a, int b, int c)
		{
			return Math.Min(a, Math.Min(b, c));
		}

		public static double Max(double a, double b, double c)
		{
			return Math.Max(a, Math.Max(b, c));
		}

		public static float Max(float a, float b, float c)
		{
			return Math.Max(a, Math.Max(b, c));
		}

		public static int Max(int a, int b, int c)
		{
			return Math.Max(a, Math.Max(b, c));
		}

		public static double InvSqrt(double f)
		{
			return f / Math.Sqrt(f);
		}

		public static double Atan2Positive(double y, double x)
		{
			double num = Math.Atan2(y, x);
			if (num < 0.0)
			{
				num = 6.283185307179586 + num;
			}
			return num;
		}

		public static float PlaneAngleD(Vector3f a, Vector3f b, int nPlaneNormalIdx = 1)
		{
			a[nPlaneNormalIdx] = (b[nPlaneNormalIdx] = 0f);
			a.Normalize(1.1920929E-07f);
			b.Normalize(1.1920929E-07f);
			return Vector3f.AngleD(a, b);
		}

		public static double PlaneAngleD(Vector3d a, Vector3d b, int nPlaneNormalIdx = 1)
		{
			a[nPlaneNormalIdx] = (b[nPlaneNormalIdx] = 0.0);
			a.Normalize(2.220446049250313E-16);
			b.Normalize(2.220446049250313E-16);
			return Vector3d.AngleD(a, b);
		}

		public static float PlaneAngleSignedD(Vector3f vFrom, Vector3f vTo, int nPlaneNormalIdx = 1)
		{
			vFrom[nPlaneNormalIdx] = (vTo[nPlaneNormalIdx] = 0f);
			vFrom.Normalize(1.1920929E-07f);
			vTo.Normalize(1.1920929E-07f);
			Vector3f vector3f = vFrom.Cross(vTo);
			if (vector3f.LengthSquared >= 1E-06f)
			{
				return (float)Math.Sign(vector3f[nPlaneNormalIdx]) * Vector3f.AngleD(vFrom, vTo);
			}
			if (vFrom.Dot(vTo) >= 0f)
			{
				return 0f;
			}
			return 180f;
		}

		public static double PlaneAngleSignedD(Vector3d vFrom, Vector3d vTo, int nPlaneNormalIdx = 1)
		{
			vFrom[nPlaneNormalIdx] = (vTo[nPlaneNormalIdx] = 0.0);
			vFrom.Normalize(2.220446049250313E-16);
			vTo.Normalize(2.220446049250313E-16);
			Vector3d vector3d = vFrom.Cross(vTo);
			if (vector3d.LengthSquared >= 1E-08)
			{
				return (double)Math.Sign(vector3d[nPlaneNormalIdx]) * Vector3d.AngleD(vFrom, vTo);
			}
			if (vFrom.Dot(vTo) >= 0.0)
			{
				return 0.0;
			}
			return 180.0;
		}

		public static float PlaneAngleSignedD(Vector3f vFrom, Vector3f vTo, Vector3f planeN)
		{
			vFrom -= Vector3f.Dot(vFrom, planeN) * planeN;
			vTo -= Vector3f.Dot(vTo, planeN) * planeN;
			vFrom.Normalize(1.1920929E-07f);
			vTo.Normalize(1.1920929E-07f);
			Vector3f v = Vector3f.Cross(vFrom, vTo);
			if (v.LengthSquared >= 1E-06f)
			{
				return (float)Math.Sign(Vector3f.Dot(v, planeN)) * Vector3f.AngleD(vFrom, vTo);
			}
			if (vFrom.Dot(vTo) >= 0f)
			{
				return 0f;
			}
			return 180f;
		}

		public static double PlaneAngleSignedD(Vector3d vFrom, Vector3d vTo, Vector3d planeN)
		{
			vFrom -= Vector3d.Dot(vFrom, planeN) * planeN;
			vTo -= Vector3d.Dot(vTo, planeN) * planeN;
			vFrom.Normalize(2.220446049250313E-16);
			vTo.Normalize(2.220446049250313E-16);
			Vector3d v = Vector3d.Cross(vFrom, vTo);
			if (v.LengthSquared >= 1E-08)
			{
				return (double)Math.Sign(Vector3d.Dot(v, planeN)) * Vector3d.AngleD(vFrom, vTo);
			}
			if (vFrom.Dot(vTo) >= 0.0)
			{
				return 0.0;
			}
			return 180.0;
		}

		public static float PlaneAngleSignedD(Vector2f vFrom, Vector2f vTo)
		{
			vFrom.Normalize(1.1920929E-07f);
			vTo.Normalize(1.1920929E-07f);
			return (float)Math.Sign(vFrom.Cross(vTo)) * Vector2f.AngleD(vFrom, vTo);
		}

		public static double PlaneAngleSignedD(Vector2d vFrom, Vector2d vTo)
		{
			vFrom.Normalize(2.220446049250313E-16);
			vTo.Normalize(2.220446049250313E-16);
			return (double)Math.Sign(vFrom.Cross(vTo)) * Vector2d.AngleD(vFrom, vTo);
		}

		public static int MostParallelAxis(Frame3f f, Vector3f vDir)
		{
			double num = (double)Math.Abs(f.X.Dot(vDir));
			double num2 = (double)Math.Abs(f.Y.Dot(vDir));
			double val = (double)Math.Abs(f.Z.Dot(vDir));
			double num3 = Math.Max(num, Math.Max(num2, val));
			if (num3 == num)
			{
				return 0;
			}
			if (num3 != num2)
			{
				return 2;
			}
			return 1;
		}

		public static float Lerp(float a, float b, float t)
		{
			return (1f - t) * a + t * b;
		}

		public static double Lerp(double a, double b, double t)
		{
			return (1.0 - t) * a + t * b;
		}

		public static float SmoothStep(float a, float b, float t)
		{
			t = t * t * (3f - 2f * t);
			return (1f - t) * a + t * b;
		}

		public static double SmoothStep(double a, double b, double t)
		{
			t = t * t * (3.0 - 2.0 * t);
			return (1.0 - t) * a + t * b;
		}

		public static float SmoothInterp(float a, float b, float t)
		{
			float num = MathUtil.WyvillRise01(t);
			return (1f - num) * a + num * b;
		}

		public static double SmoothInterp(double a, double b, double t)
		{
			double num = MathUtil.WyvillRise01(t);
			return (1.0 - num) * a + num * b;
		}

		public static float SmoothRise0To1(float fX, float yshift, float xZero, float speed)
		{
			double num = Math.Pow((double)(fX - (xZero - 1f)), (double)speed);
			return MathUtil.Clamp((float)((double)(1f + yshift) + 1.0 / -(float)num), 0f, 1f);
		}

		public static float WyvillRise01(float fX)
		{
			float num = MathUtil.Clamp(1f - fX * fX, 0f, 1f);
			return 1f - num * num * num;
		}

		public static double WyvillRise01(double fX)
		{
			double num = MathUtil.Clamp(1.0 - fX * fX, 0.0, 1.0);
			return 1.0 - num * num * num;
		}

		public static float WyvillFalloff01(float fX)
		{
			float num = 1f - fX * fX;
			if (num < 0f)
			{
				return 0f;
			}
			return num * num * num;
		}

		public static double WyvillFalloff01(double fX)
		{
			double num = 1.0 - fX * fX;
			if (num < 0.0)
			{
				return 0.0;
			}
			return num * num * num;
		}

		public static float WyvillFalloff(float fD, float fInnerRad, float fOuterRad)
		{
			if (fD > fOuterRad)
			{
				return 0f;
			}
			if (fD > fInnerRad)
			{
				fD -= fInnerRad;
				fD /= fOuterRad - fInnerRad;
				fD = Math.Max(0f, Math.Min(1f, fD));
				float num = 1f - fD * fD;
				return num * num * num;
			}
			return 1f;
		}

		public static double WyvillFalloff(double fD, double fInnerRad, double fOuterRad)
		{
			if (fD > fOuterRad)
			{
				return 0.0;
			}
			if (fD > fInnerRad)
			{
				fD -= fInnerRad;
				fD /= fOuterRad - fInnerRad;
				fD = Math.Max(0.0, Math.Min(1.0, fD));
				double num = 1.0 - fD * fD;
				return num * num * num;
			}
			return 1.0;
		}

		public static float LinearRampT(float R, float deadzoneR, float x)
		{
			float num = (float)Math.Sign(x);
			x = Math.Abs(x);
			if (x < deadzoneR)
			{
				return 0f;
			}
			if (x > R)
			{
				return num * 1f;
			}
			x = Math.Min(x, R);
			float num2 = (x - deadzoneR) / (R - deadzoneR);
			return num * num2;
		}

		public static double Area(ref Vector3d v1, ref Vector3d v2, ref Vector3d v3)
		{
			return 0.5 * (v2 - v1).Cross(v3 - v1).Length;
		}

		public static double Area(Vector3d v1, Vector3d v2, Vector3d v3)
		{
			return 0.5 * (v2 - v1).Cross(v3 - v1).Length;
		}

		public static Vector3d Normal(ref Vector3d v1, ref Vector3d v2, ref Vector3d v3)
		{
			Vector3d vector3d = v2 - v1;
			Vector3d v4 = v3 - v2;
			vector3d.Normalize(2.220446049250313E-16);
			v4.Normalize(2.220446049250313E-16);
			Vector3d result = vector3d.Cross(v4);
			result.Normalize(2.220446049250313E-16);
			return result;
		}

		public static Vector3d Normal(Vector3d v1, Vector3d v2, Vector3d v3)
		{
			return MathUtil.Normal(ref v1, ref v2, ref v3);
		}

		public static Vector3d FastNormalDirection(ref Vector3d v1, ref Vector3d v2, ref Vector3d v3)
		{
			Vector3d vector3d = v2 - v1;
			Vector3d v4 = v3 - v1;
			return vector3d.Cross(v4);
		}

		public static Vector3d FastNormalArea(ref Vector3d v1, ref Vector3d v2, ref Vector3d v3, out double area)
		{
			Vector3d vector3d = v2 - v1;
			Vector3d v4 = v3 - v1;
			Vector3d result = vector3d.Cross(v4);
			area = 0.5 * result.Normalize(2.220446049250313E-16);
			return result;
		}

		public static double AspectRatio(ref Vector3d v1, ref Vector3d v2, ref Vector3d v3)
		{
			double num = v1.Distance(ref v2);
			double num2 = v2.Distance(ref v3);
			double num3 = v3.Distance(ref v1);
			double num4 = (num + num2 + num3) / 2.0;
			return num * num2 * num3 / (8.0 * (num4 - num) * (num4 - num2) * (num4 - num3));
		}

		public static double AspectRatio(Vector3d v1, Vector3d v2, Vector3d v3)
		{
			return MathUtil.AspectRatio(ref v1, ref v2, ref v3);
		}

		public static double VectorCot(Vector3d v1, Vector3d v2)
		{
			double num = v1.Dot(v2);
			double lengthSquared = v1.LengthSquared;
			double lengthSquared2 = v2.LengthSquared;
			double num2 = MathUtil.Clamp(lengthSquared * lengthSquared2 - num * num, 0.0, double.MaxValue);
			if (num2 < 1E-08)
			{
				return 0.0;
			}
			return num / Math.Sqrt(num2);
		}

		public static double VectorTan(Vector3d v1, Vector3d v2)
		{
			double num = v1.Dot(v2);
			double lengthSquared = v1.LengthSquared;
			double lengthSquared2 = v2.LengthSquared;
			double num2 = MathUtil.Clamp(lengthSquared * lengthSquared2 - num * num, 0.0, double.MaxValue);
			if (num2 == 0.0)
			{
				return 0.0;
			}
			return Math.Sqrt(num2) / num;
		}

		public static bool IsObtuse(Vector3d v1, Vector3d v2, Vector3d v3)
		{
			double num = v1.DistanceSquared(v2);
			double num2 = v1.DistanceSquared(v3);
			double num3 = v2.DistanceSquared(v3);
			return num + num2 < num3 || num2 + num3 < num || num3 + num < num2;
		}

		public static double IsLeft(Vector2d P0, Vector2d P1, Vector2d P2)
		{
			return (double)Math.Sign((P1.x - P0.x) * (P2.y - P0.y) - (P2.x - P0.x) * (P1.y - P0.y));
		}

		public static double IsLeft(ref Vector2d P0, ref Vector2d P1, ref Vector2d P2)
		{
			return (double)Math.Sign((P1.x - P0.x) * (P2.y - P0.y) - (P2.x - P0.x) * (P1.y - P0.y));
		}

		public static Vector3d BarycentricCoords(ref Vector3d vPoint, ref Vector3d V0, ref Vector3d V1, ref Vector3d V2)
		{
			Vector3d v = V0 - V2;
			Vector3d v2 = V1 - V2;
			Vector3d v3 = vPoint - V2;
			double num = v.Dot(v);
			double num2 = v.Dot(v2);
			double num3 = v2.Dot(v2);
			double num4 = v.Dot(v3);
			double num5 = v2.Dot(v3);
			double num6 = num * num3 - num2 * num2;
			double num7 = 1.0 / num6;
			double num8 = (num3 * num4 - num2 * num5) * num7;
			double num9 = (num * num5 - num2 * num4) * num7;
			double z = 1.0 - num8 - num9;
			return new Vector3d(num8, num9, z);
		}

		public static Vector3d BarycentricCoords(Vector3d vPoint, Vector3d V0, Vector3d V1, Vector3d V2)
		{
			return MathUtil.BarycentricCoords(ref vPoint, ref V0, ref V1, ref V2);
		}

		public static Vector3d BarycentricCoords(Vector2d vPoint, Vector2d V0, Vector2d V1, Vector2d V2)
		{
			Vector2d v = V0 - V2;
			Vector2d v2 = V1 - V2;
			Vector2d v3 = vPoint - V2;
			double num = v.Dot(v);
			double num2 = v.Dot(v2);
			double num3 = v2.Dot(v2);
			double num4 = v.Dot(v3);
			double num5 = v2.Dot(v3);
			double num6 = num * num3 - num2 * num2;
			double num7 = 1.0 / num6;
			double num8 = (num3 * num4 - num2 * num5) * num7;
			double num9 = (num * num5 - num2 * num4) * num7;
			double z = 1.0 - num8 - num9;
			return new Vector3d(num8, num9, z);
		}

		public static double TriSolidAngle(Vector3d a, Vector3d b, Vector3d c, ref Vector3d p)
		{
			a -= p;
			b -= p;
			c -= p;
			double length = a.Length;
			double length2 = b.Length;
			double length3 = c.Length;
			double x = length * length2 * length3 + a.Dot(ref b) * length3 + b.Dot(ref c) * length + c.Dot(ref a) * length2;
			double y = a.x * (b.y * c.z - c.y * b.z) - a.y * (b.x * c.z - c.x * b.z) + a.z * (b.x * c.y - c.x * b.y);
			return 2.0 * Math.Atan2(y, x);
		}

		public static bool SolveQuadratic(double a, double b, double c, out double minT, out double maxT)
		{
			minT = (maxT = 0.0);
			if (a == 0.0 && b == 0.0)
			{
				return true;
			}
			double num = b * b - 4.0 * a * c;
			if (num < 0.0)
			{
				return false;
			}
			double num2 = -0.5 * (b + (double)Math.Sign(b) * Math.Sqrt(num));
			minT = num2 / a;
			maxT = c / num2;
			if (minT > maxT)
			{
				a = minT;
				minT = maxT;
				maxT = a;
			}
			return true;
		}

		public static int PowerOf10(int n)
		{
			return MathUtil.powers_of_10[n];
		}

		public static IEnumerable<int> ModuloIteration(int nMaxExclusive, int nPrime = 31337)
		{
			int i = 0;
			bool flag = false;
			while (!flag)
			{
				yield return i;
				i = (i + nPrime) % nMaxExclusive;
				flag = (i == 0);
			}
			yield break;
		}

		public const double Deg2Rad = 0.017453292519943295;

		public const double Rad2Deg = 57.29577951308232;

		public const double TwoPI = 6.283185307179586;

		public const double FourPI = 12.566370614359172;

		public const double HalfPI = 1.5707963267948966;

		public const double ZeroTolerance = 1E-08;

		public const double Epsilon = 2.220446049250313E-16;

		public const double SqrtTwo = 1.4142135623730951;

		public const double SqrtTwoInv = 0.7071067811865475;

		public const double SqrtThree = 1.7320508075688772;

		public const float Deg2Radf = 0.017453292f;

		public const float Rad2Degf = 57.29578f;

		public const float PIf = 3.1415927f;

		public const float TwoPIf = 6.2831855f;

		public const float HalfPIf = 1.5707964f;

		public const float SqrtTwof = 1.4142135f;

		public const float ZeroTolerancef = 1E-06f;

		public const float Epsilonf = 1.1920929E-07f;

		private static readonly int[] powers_of_10 = new int[]
		{
			1,
			10,
			100,
			1000,
			10000,
			100000,
			1000000,
			10000000,
			100000000,
			1000000000
		};
	}
}
