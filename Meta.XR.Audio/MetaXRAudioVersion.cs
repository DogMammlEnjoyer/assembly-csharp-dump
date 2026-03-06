using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class MetaXRAudioVersion : MonoBehaviour
{
	private void Awake()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		MetaXRAudioVersion.MetaXRAudio_GetVersion(ref num, ref num2, ref num3);
		Debug.Log(string.Format(string.Format("MetaXRAudio Version: {0}.{1}.{2}", num, num2, num3), Array.Empty<object>()));
	}

	[DllImport("MetaXRAudioUnity")]
	private static extern void MetaXRAudio_GetVersion(ref int Major, ref int Minor, ref int Patch);
}
