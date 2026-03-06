using System;
using System.Collections.Generic;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger
{
	[Serializable]
	public class DebugData
	{
		public DebugData(string assemblyName, List<string> types)
		{
			this.AssemblyName = assemblyName;
			this.DebugTypes = types;
		}

		[SerializeField]
		public string AssemblyName;

		[SerializeField]
		public List<string> DebugTypes;
	}
}
