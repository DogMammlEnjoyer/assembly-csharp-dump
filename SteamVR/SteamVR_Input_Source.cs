using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Valve.VR
{
	public static class SteamVR_Input_Source
	{
		public static ulong GetHandle(SteamVR_Input_Sources inputSource)
		{
			if (inputSource < (SteamVR_Input_Sources)SteamVR_Input_Source.inputSourceHandlesBySource.Length)
			{
				return SteamVR_Input_Source.inputSourceHandlesBySource[(int)inputSource];
			}
			return 0UL;
		}

		public static SteamVR_Input_Sources GetSource(ulong handle)
		{
			if (SteamVR_Input_Source.inputSourceSourcesByHandle.ContainsKey(handle))
			{
				return SteamVR_Input_Source.inputSourceSourcesByHandle[handle];
			}
			return SteamVR_Input_Sources.Any;
		}

		public static SteamVR_Input_Sources[] GetAllSources()
		{
			if (SteamVR_Input_Source.allSources == null)
			{
				SteamVR_Input_Source.allSources = (SteamVR_Input_Sources[])Enum.GetValues(typeof(SteamVR_Input_Sources));
			}
			return SteamVR_Input_Source.allSources;
		}

		private static string GetPath(string inputSourceEnumName)
		{
			return ((DescriptionAttribute)SteamVR_Input_Source.enumType.GetMember(inputSourceEnumName)[0].GetCustomAttributes(SteamVR_Input_Source.descriptionType, false)[0]).Description;
		}

		public static void Initialize()
		{
			List<SteamVR_Input_Sources> list = new List<SteamVR_Input_Sources>();
			string[] names = Enum.GetNames(SteamVR_Input_Source.enumType);
			SteamVR_Input_Source.inputSourceHandlesBySource = new ulong[names.Length];
			SteamVR_Input_Source.inputSourceSourcesByHandle = new Dictionary<ulong, SteamVR_Input_Sources>();
			for (int i = 0; i < names.Length; i++)
			{
				string path = SteamVR_Input_Source.GetPath(names[i]);
				ulong num = 0UL;
				EVRInputError inputSourceHandle = OpenVR.Input.GetInputSourceHandle(path, ref num);
				if (inputSourceHandle != EVRInputError.None)
				{
					Debug.LogError("<b>[SteamVR]</b> GetInputSourceHandle (" + path + ") error: " + inputSourceHandle.ToString());
				}
				if (names[i] == SteamVR_Input_Sources.Any.ToString())
				{
					SteamVR_Input_Source.inputSourceHandlesBySource[i] = 0UL;
					SteamVR_Input_Source.inputSourceSourcesByHandle.Add(0UL, (SteamVR_Input_Sources)i);
				}
				else
				{
					SteamVR_Input_Source.inputSourceHandlesBySource[i] = num;
					SteamVR_Input_Source.inputSourceSourcesByHandle.Add(num, (SteamVR_Input_Sources)i);
				}
				list.Add((SteamVR_Input_Sources)i);
			}
			SteamVR_Input_Source.allSources = list.ToArray();
		}

		public static int numSources = Enum.GetValues(typeof(SteamVR_Input_Sources)).Length;

		private static ulong[] inputSourceHandlesBySource;

		private static Dictionary<ulong, SteamVR_Input_Sources> inputSourceSourcesByHandle = new Dictionary<ulong, SteamVR_Input_Sources>();

		private static Type enumType = typeof(SteamVR_Input_Sources);

		private static Type descriptionType = typeof(DescriptionAttribute);

		private static SteamVR_Input_Sources[] allSources;
	}
}
