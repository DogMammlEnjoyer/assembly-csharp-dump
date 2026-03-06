using System;
using System.Collections.Generic;
using UnityEngine;

public class OVRResources : MonoBehaviour
{
	public static Object Load(string path)
	{
		if (!Debug.isDebugBuild)
		{
			return Resources.Load(path);
		}
		if (OVRResources.resourceBundle == null)
		{
			Debug.Log("[OVRResources] Resource bundle was not loaded successfully");
			return null;
		}
		string name = OVRResources.assetNames.Find((string s) => s.Contains(path.ToLower()));
		return OVRResources.resourceBundle.LoadAsset(name);
	}

	public static T Load<T>(string path) where T : Object
	{
		if (!Debug.isDebugBuild)
		{
			return Resources.Load<T>(path);
		}
		if (OVRResources.resourceBundle == null)
		{
			Debug.Log("[OVRResources] Resource bundle was not loaded successfully");
			return default(T);
		}
		string name = OVRResources.assetNames.Find((string s) => s.Contains(path.ToLower()));
		return OVRResources.resourceBundle.LoadAsset<T>(name);
	}

	public static void SetResourceBundle(AssetBundle bundle)
	{
		OVRResources.resourceBundle = bundle;
		OVRResources.assetNames = new List<string>();
		OVRResources.assetNames.AddRange(OVRResources.resourceBundle.GetAllAssetNames());
	}

	private static AssetBundle resourceBundle;

	private static List<string> assetNames;
}
