using System;
using Meta.XR.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Meta.XR.MRUtilityKit
{
	[Feature(Feature.Scene)]
	public static class SerializationHelpers
	{
		[JsonConverter(typeof(StringEnumConverter))]
		[Obsolete("Coordinate system is now obsolete, JSON files are now always serialized in OpenXR coordinate system")]
		[Serializable]
		public enum CoordinateSystem
		{
			Unity,
			Unreal
		}
	}
}
