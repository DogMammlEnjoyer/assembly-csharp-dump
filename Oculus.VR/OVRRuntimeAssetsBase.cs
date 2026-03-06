using System;
using UnityEngine;

public class OVRRuntimeAssetsBase : ScriptableObject
{
	internal static void LoadAsset<T>(out T assetInstance, string assetName, Action<T> onCreateAsset = null) where T : OVRRuntimeAssetsBase
	{
		assetInstance = default(T);
		assetInstance = Resources.Load<T>(assetName);
	}
}
