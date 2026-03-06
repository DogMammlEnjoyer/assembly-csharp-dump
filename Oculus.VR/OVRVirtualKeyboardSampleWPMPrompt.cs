using System;
using Meta.XR.Util;
using UnityEngine;

[Obsolete]
[ExecuteInEditMode]
[Feature(Feature.VirtualKeyboard)]
public class OVRVirtualKeyboardSampleWPMPrompt : MonoBehaviour
{
	private void Awake()
	{
		Object.DestroyImmediate(this);
	}
}
