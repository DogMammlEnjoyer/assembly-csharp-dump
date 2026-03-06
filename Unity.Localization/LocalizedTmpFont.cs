using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine.UIElements;

namespace UnityEngine.Localization
{
	[UxmlObject]
	[Serializable]
	public class LocalizedTmpFont : LocalizedAsset<TMP_FontAsset>
	{
		[CompilerGenerated]
		[Serializable]
		public new class UxmlSerializedData : LocalizedAsset<TMP_FontAsset>.UxmlSerializedData
		{
			[RegisterUxmlCache]
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
			}

			public override object CreateInstance()
			{
				return new LocalizedTmpFont();
			}
		}
	}
}
