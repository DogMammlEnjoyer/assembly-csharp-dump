using System;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.GlobalIllumination
{
	public static class LightmapperUtils
	{
		public static LightMode Extract(LightmapBakeType baketype)
		{
			return (baketype == LightmapBakeType.Realtime) ? LightMode.Realtime : ((baketype == LightmapBakeType.Mixed) ? LightMode.Mixed : LightMode.Baked);
		}

		public static LinearColor ExtractIndirect(Light l)
		{
			return LinearColor.Convert(l.color, l.intensity * l.bounceIntensity);
		}

		public static float ExtractInnerCone(Light l)
		{
			return 2f * Mathf.Atan(Mathf.Tan(l.spotAngle * 0.5f * 0.017453292f) * 46f / 64f);
		}

		private static Color ExtractColorTemperature(Light l)
		{
			Color result = new Color(1f, 1f, 1f);
			bool flag = l.useColorTemperature && GraphicsSettings.lightsUseLinearIntensity;
			if (flag)
			{
				result = Mathf.CorrelatedColorTemperatureToRGB(l.colorTemperature);
			}
			return result;
		}

		private static void ApplyColorTemperature(Color cct, ref LinearColor lightColor)
		{
			lightColor.red *= cct.r;
			lightColor.green *= cct.g;
			lightColor.blue *= cct.b;
		}

		public static void Extract(Light l, ref DirectionalLight dir)
		{
			dir.instanceID = l.GetInstanceID();
			dir.mode = LightmapperUtils.Extract(l.bakingOutput.lightmapBakeType);
			dir.shadow = (l.shadows > LightShadows.None);
			dir.position = l.transform.position;
			dir.orientation = l.transform.rotation;
			Color cct = LightmapperUtils.ExtractColorTemperature(l);
			LinearColor color = LinearColor.Convert(l.color, l.intensity);
			LinearColor indirectColor = LightmapperUtils.ExtractIndirect(l);
			LightmapperUtils.ApplyColorTemperature(cct, ref color);
			LightmapperUtils.ApplyColorTemperature(cct, ref indirectColor);
			dir.color = color;
			dir.indirectColor = indirectColor;
			dir.penumbraWidthRadian = 0f;
		}

		public static void Extract(Light l, ref PointLight point)
		{
			point.instanceID = l.GetInstanceID();
			point.mode = LightmapperUtils.Extract(l.bakingOutput.lightmapBakeType);
			point.shadow = (l.shadows > LightShadows.None);
			point.position = l.transform.position;
			point.orientation = l.transform.rotation;
			Color cct = LightmapperUtils.ExtractColorTemperature(l);
			LinearColor color = LinearColor.Convert(l.color, l.intensity);
			LinearColor indirectColor = LightmapperUtils.ExtractIndirect(l);
			LightmapperUtils.ApplyColorTemperature(cct, ref color);
			LightmapperUtils.ApplyColorTemperature(cct, ref indirectColor);
			point.color = color;
			point.indirectColor = indirectColor;
			point.range = l.range;
			point.sphereRadius = 0f;
			point.falloff = FalloffType.Legacy;
		}

		public static void Extract(Light l, ref SpotLight spot)
		{
			spot.instanceID = l.GetInstanceID();
			spot.mode = LightmapperUtils.Extract(l.bakingOutput.lightmapBakeType);
			spot.shadow = (l.shadows > LightShadows.None);
			spot.position = l.transform.position;
			spot.orientation = l.transform.rotation;
			Color cct = LightmapperUtils.ExtractColorTemperature(l);
			LinearColor color = LinearColor.Convert(l.color, l.intensity);
			LinearColor indirectColor = LightmapperUtils.ExtractIndirect(l);
			LightmapperUtils.ApplyColorTemperature(cct, ref color);
			LightmapperUtils.ApplyColorTemperature(cct, ref indirectColor);
			spot.color = color;
			spot.indirectColor = indirectColor;
			spot.range = l.range;
			spot.sphereRadius = 0f;
			spot.coneAngle = l.spotAngle * 0.017453292f;
			spot.innerConeAngle = LightmapperUtils.ExtractInnerCone(l);
			spot.falloff = FalloffType.Legacy;
			spot.angularFalloff = AngularFalloffType.LUT;
		}

		public static void Extract(Light l, ref RectangleLight rect)
		{
			rect.instanceID = l.GetInstanceID();
			rect.mode = LightmapperUtils.Extract(l.bakingOutput.lightmapBakeType);
			rect.shadow = (l.shadows > LightShadows.None);
			rect.position = l.transform.position;
			rect.orientation = l.transform.rotation;
			Color cct = LightmapperUtils.ExtractColorTemperature(l);
			LinearColor color = LinearColor.Convert(l.color, l.intensity);
			LinearColor indirectColor = LightmapperUtils.ExtractIndirect(l);
			LightmapperUtils.ApplyColorTemperature(cct, ref color);
			LightmapperUtils.ApplyColorTemperature(cct, ref indirectColor);
			rect.color = color;
			rect.indirectColor = indirectColor;
			rect.range = l.dilatedRange;
			rect.width = 0f;
			rect.height = 0f;
			rect.falloff = FalloffType.Legacy;
		}

		public static void Extract(Light l, ref DiscLight disc)
		{
			disc.instanceID = l.GetInstanceID();
			disc.mode = LightmapperUtils.Extract(l.bakingOutput.lightmapBakeType);
			disc.shadow = (l.shadows > LightShadows.None);
			disc.position = l.transform.position;
			disc.orientation = l.transform.rotation;
			Color cct = LightmapperUtils.ExtractColorTemperature(l);
			LinearColor color = LinearColor.Convert(l.color, l.intensity);
			LinearColor indirectColor = LightmapperUtils.ExtractIndirect(l);
			LightmapperUtils.ApplyColorTemperature(cct, ref color);
			LightmapperUtils.ApplyColorTemperature(cct, ref indirectColor);
			disc.color = color;
			disc.indirectColor = indirectColor;
			disc.range = l.dilatedRange;
			disc.radius = 0f;
			disc.falloff = FalloffType.Legacy;
		}

		public static void Extract(Light l, out Cookie cookie)
		{
			cookie.instanceID = (l.cookie ? l.cookie.GetInstanceID() : 0);
			cookie.scale = 1f;
			cookie.sizes = ((l.type == LightType.Directional && l.cookie) ? new Vector2(l.cookieSize, l.cookieSize) : new Vector2(1f, 1f));
		}
	}
}
