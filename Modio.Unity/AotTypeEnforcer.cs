using System;
using Modio.API;
using UnityEngine;

namespace Modio.Unity
{
	internal class AotTypeEnforcer : MonoBehaviour
	{
		private void Awake()
		{
			AotTypeEnforcer.Hello();
		}
	}
}
