using System;
using Meta.XR.Util;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
	[Feature(Feature.Scene)]
	public static class SingletonMonoBehaviour
	{
		public class InstantiationSettings : Attribute
		{
			public bool dontDestroyOnLoad;
		}
	}
}
