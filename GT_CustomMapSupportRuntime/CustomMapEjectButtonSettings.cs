using System;
using UnityEngine;

namespace GT_CustomMapSupportRuntime
{
	public class CustomMapEjectButtonSettings : MonoBehaviour
	{
		public CustomMapEjectButtonSettings.EjectType ejectType;

		public enum EjectType
		{
			EjectFromVirtualStump,
			ReturnToVirtualStump
		}
	}
}
