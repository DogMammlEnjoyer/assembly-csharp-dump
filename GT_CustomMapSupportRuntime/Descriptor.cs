using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace GT_CustomMapSupportRuntime
{
	[Preserve]
	public class Descriptor
	{
		[JsonConstructor]
		public Descriptor()
		{
		}

		[Nullable(1)]
		[JsonProperty(PropertyName = "objectName")]
		public string objectName = "";
	}
}
