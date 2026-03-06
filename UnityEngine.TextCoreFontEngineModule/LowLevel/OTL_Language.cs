using System;
using System.Diagnostics;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.LowLevel
{
	[DebuggerDisplay("Language = {tag},  Feature Count = {featureIndexes.Length}")]
	[UsedByNativeCode]
	internal struct OTL_Language
	{
		public OTL_Tag tag;

		public uint[] featureIndexes;
	}
}
