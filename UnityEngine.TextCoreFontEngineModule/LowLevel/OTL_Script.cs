using System;
using System.Diagnostics;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.LowLevel
{
	[DebuggerDisplay("Script = {tag},  Language Count = {languages.Length}")]
	[UsedByNativeCode]
	internal struct OTL_Script
	{
		public OTL_Tag tag;

		public OTL_Language[] languages;
	}
}
