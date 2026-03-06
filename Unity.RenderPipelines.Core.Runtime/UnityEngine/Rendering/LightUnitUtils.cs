using System;

namespace UnityEngine.Rendering
{
	public static class LightUnitUtils
	{
		private static float k_LuminanceToEvFactor
		{
			get
			{
				return Mathf.Log(100f / ColorUtils.s_LightMeterCalibrationConstant, 2f);
			}
		}

		private static float k_EvToLuminanceFactor
		{
			get
			{
				return -LightUnitUtils.k_LuminanceToEvFactor;
			}
		}

		public static LightUnit GetNativeLightUnit(LightType lightType)
		{
			switch (lightType)
			{
			case LightType.Spot:
			case LightType.Point:
			case LightType.Pyramid:
				return LightUnit.Candela;
			case LightType.Directional:
			case LightType.Box:
				return LightUnit.Lux;
			case LightType.Area:
			case LightType.Disc:
			case LightType.Tube:
				return LightUnit.Nits;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public static bool IsLightUnitSupported(LightType lightType, LightUnit lightUnit)
		{
			int num = 1 << (int)lightUnit;
			switch (lightType)
			{
			case LightType.Spot:
			case LightType.Point:
			case LightType.Pyramid:
				return (num & 23) > 0;
			case LightType.Directional:
			case LightType.Box:
				return (num & 4) > 0;
			case LightType.Area:
			case LightType.Disc:
			case LightType.Tube:
				return (num & 25) > 0;
			default:
				return false;
			}
		}

		public static float GetSolidAngleFromPointLight()
		{
			return 12.566371f;
		}

		public static float GetSolidAngleFromSpotLight(float spotAngle)
		{
			double num = 3.141592653589793 * (double)spotAngle / 180.0;
			return (float)(6.283185307179586 * (1.0 - Math.Cos(num * 0.5)));
		}

		public static float GetSolidAngleFromPyramidLight(float spotAngle, float aspectRatio)
		{
			if (aspectRatio < 1f)
			{
				aspectRatio = (float)(1.0 / (double)aspectRatio);
			}
			double num = 3.141592653589793 * (double)spotAngle / 180.0;
			double num2 = Math.Atan(Math.Tan(0.5 * num) * (double)aspectRatio) * 2.0;
			return (float)(4.0 * Math.Asin(Math.Sin(num * 0.5) * Math.Sin(num2 * 0.5)));
		}

		internal static float GetSolidAngle(LightType lightType, bool spotReflector, float spotAngle, float aspectRatio)
		{
			float result;
			if (lightType != LightType.Spot)
			{
				if (lightType != LightType.Point)
				{
					if (lightType != LightType.Pyramid)
					{
						throw new ArgumentException("Solid angle is undefined for lights of type " + lightType.ToString());
					}
					result = (spotReflector ? LightUnitUtils.GetSolidAngleFromPyramidLight(spotAngle, aspectRatio) : 12.566371f);
				}
				else
				{
					result = LightUnitUtils.GetSolidAngleFromPointLight();
				}
			}
			else
			{
				result = (spotReflector ? LightUnitUtils.GetSolidAngleFromSpotLight(spotAngle) : 12.566371f);
			}
			return result;
		}

		public static float GetAreaFromRectangleLight(float rectSizeX, float rectSizeY)
		{
			return Mathf.Abs(rectSizeX * rectSizeY) * 3.1415927f;
		}

		public static float GetAreaFromRectangleLight(Vector2 rectSize)
		{
			return LightUnitUtils.GetAreaFromRectangleLight(rectSize.x, rectSize.y);
		}

		public static float GetAreaFromDiscLight(float discRadius)
		{
			return discRadius * discRadius * 3.1415927f * 3.1415927f;
		}

		public static float GetAreaFromTubeLight(float tubeLength)
		{
			return Mathf.Abs(tubeLength) * 4f * 3.1415927f;
		}

		public static float LumenToCandela(float lumen, float solidAngle)
		{
			return lumen / solidAngle;
		}

		public static float CandelaToLumen(float candela, float solidAngle)
		{
			return candela * solidAngle;
		}

		public static float LumenToNits(float lumen, float area)
		{
			return lumen / area;
		}

		public static float NitsToLumen(float nits, float area)
		{
			return nits * area;
		}

		public static float LuxToCandela(float lux, float distance)
		{
			return lux * (distance * distance);
		}

		public static float CandelaToLux(float candela, float distance)
		{
			return candela / (distance * distance);
		}

		public static float Ev100ToNits(float ev100)
		{
			return Mathf.Pow(2f, ev100 + LightUnitUtils.k_EvToLuminanceFactor);
		}

		public static float NitsToEv100(float nits)
		{
			return Mathf.Log(nits, 2f) + LightUnitUtils.k_LuminanceToEvFactor;
		}

		public static float Ev100ToCandela(float ev100)
		{
			return LightUnitUtils.Ev100ToNits(ev100);
		}

		public static float CandelaToEv100(float candela)
		{
			return LightUnitUtils.NitsToEv100(candela);
		}

		internal static float ConvertIntensityInternal(float intensity, LightUnit fromUnit, LightUnit toUnit, LightType lightType, float area, float luxAtDistance, float solidAngle)
		{
			if (!LightUnitUtils.IsLightUnitSupported(lightType, fromUnit) || !LightUnitUtils.IsLightUnitSupported(lightType, toUnit))
			{
				throw new ArgumentException(string.Concat(new string[]
				{
					"Converting ",
					fromUnit.ToString(),
					" to ",
					toUnit.ToString(),
					" is undefined for lights of type ",
					lightType.ToString()
				}));
			}
			if (fromUnit == toUnit)
			{
				return intensity;
			}
			switch (fromUnit)
			{
			case LightUnit.Lumen:
				switch (toUnit)
				{
				case LightUnit.Candela:
					return LightUnitUtils.LumenToCandela(intensity, solidAngle);
				case LightUnit.Lux:
					return LightUnitUtils.CandelaToLux(LightUnitUtils.LumenToCandela(intensity, solidAngle), luxAtDistance);
				case LightUnit.Nits:
					return LightUnitUtils.LumenToNits(intensity, area);
				case LightUnit.Ev100:
				{
					float nits;
					switch (lightType)
					{
					case LightType.Spot:
					case LightType.Point:
					case LightType.Pyramid:
						nits = LightUnitUtils.LumenToCandela(intensity, solidAngle);
						goto IL_12A;
					case LightType.Area:
					case LightType.Disc:
					case LightType.Tube:
						nits = LightUnitUtils.LumenToNits(intensity, area);
						goto IL_12A;
					}
					throw new ArgumentException("Converting from Lumen to Ev100 is undefined for light type " + lightType.ToString());
					IL_12A:
					return LightUnitUtils.NitsToEv100(nits);
				}
				default:
					throw new ArgumentOutOfRangeException("toUnit", toUnit, null);
				}
				break;
			case LightUnit.Candela:
				switch (toUnit)
				{
				case LightUnit.Lumen:
					return LightUnitUtils.CandelaToLumen(intensity, solidAngle);
				case LightUnit.Lux:
					return LightUnitUtils.CandelaToLux(intensity, luxAtDistance);
				case LightUnit.Ev100:
					return LightUnitUtils.NitsToEv100(intensity);
				}
				throw new ArgumentOutOfRangeException("toUnit", toUnit, null);
			case LightUnit.Lux:
				switch (toUnit)
				{
				case LightUnit.Lumen:
					return LightUnitUtils.CandelaToLumen(LightUnitUtils.LuxToCandela(intensity, luxAtDistance), solidAngle);
				case LightUnit.Candela:
					return LightUnitUtils.LuxToCandela(intensity, luxAtDistance);
				case LightUnit.Ev100:
					return LightUnitUtils.NitsToEv100(LightUnitUtils.LuxToCandela(intensity, luxAtDistance));
				}
				throw new ArgumentOutOfRangeException("toUnit", toUnit, null);
			case LightUnit.Nits:
				if (toUnit == LightUnit.Lumen)
				{
					return LightUnitUtils.NitsToLumen(intensity, area);
				}
				if (toUnit != LightUnit.Ev100)
				{
					throw new ArgumentOutOfRangeException("toUnit", toUnit, null);
				}
				return LightUnitUtils.NitsToEv100(intensity);
			case LightUnit.Ev100:
				switch (toUnit)
				{
				case LightUnit.Lumen:
				{
					float num = LightUnitUtils.Ev100ToNits(intensity);
					switch (lightType)
					{
					case LightType.Spot:
					case LightType.Point:
					case LightType.Pyramid:
						return LightUnitUtils.CandelaToLumen(num, solidAngle);
					case LightType.Area:
					case LightType.Disc:
					case LightType.Tube:
						return LightUnitUtils.NitsToLumen(num, area);
					}
					throw new ArgumentException("Converting from Lumen to Ev100 is undefined for light type " + lightType.ToString());
				}
				case LightUnit.Candela:
				case LightUnit.Nits:
					return LightUnitUtils.Ev100ToNits(intensity);
				case LightUnit.Lux:
					return LightUnitUtils.CandelaToLux(LightUnitUtils.Ev100ToNits(intensity), luxAtDistance);
				default:
					throw new ArgumentOutOfRangeException("toUnit", toUnit, null);
				}
				break;
			default:
				throw new ArgumentOutOfRangeException("fromUnit", fromUnit, null);
			}
		}

		public static float ConvertIntensity(Light light, float intensity, LightUnit fromUnit, LightUnit toUnit)
		{
			LightType type = light.type;
			float num;
			switch (type)
			{
			case LightType.Area:
				num = LightUnitUtils.GetAreaFromRectangleLight(light.areaSize);
				goto IL_63;
			case LightType.Disc:
				num = LightUnitUtils.GetAreaFromDiscLight(light.areaSize.x);
				goto IL_63;
			case LightType.Tube:
				num = LightUnitUtils.GetAreaFromTubeLight(light.areaSize.x);
				goto IL_63;
			}
			num = 0f;
			IL_63:
			float area = num;
			float luxAtDistance = light.luxAtDistance;
			if (type == LightType.Spot || type == LightType.Point || type == LightType.Pyramid)
			{
				num = LightUnitUtils.GetSolidAngle(type, light.enableSpotReflector, light.spotAngle, light.areaSize.x);
			}
			else
			{
				num = 0f;
			}
			float solidAngle = num;
			return LightUnitUtils.ConvertIntensityInternal(intensity, fromUnit, toUnit, type, area, luxAtDistance, solidAngle);
		}

		public const float SphereSolidAngle = 12.566371f;
	}
}
