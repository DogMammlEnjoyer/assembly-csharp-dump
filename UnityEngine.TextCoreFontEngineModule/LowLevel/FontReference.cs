using System;
using System.Diagnostics;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.LowLevel
{
	[DebuggerDisplay("{familyName} - {styleName}")]
	[VisibleToOtherModules(new string[]
	{
		"UnityEngine.TextCoreTextEngineModule"
	})]
	[UsedByNativeCode]
	internal struct FontReference
	{
		public string familyName;

		public string styleName;

		public int faceIndex;

		public string filePath;
	}
}
