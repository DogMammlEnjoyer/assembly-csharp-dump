using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GT_CustomMapSupportRuntime
{
	[RequireComponent(typeof(Light))]
	public class UberShaderDynamicLight : MonoBehaviour
	{
		private void Awake()
		{
			if (this.dynamicLight == null)
			{
				this.dynamicLight = base.GetComponent<Light>();
			}
		}

		[Nullable(2)]
		public Light dynamicLight;
	}
}
