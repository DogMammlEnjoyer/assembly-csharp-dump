using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

namespace UnityEngine.Localization
{
	[UxmlObject]
	[Serializable]
	public class LocalizedObject : LocalizedAsset<Object>
	{
		[CompilerGenerated]
		[Serializable]
		public new class UxmlSerializedData : LocalizedAsset<Object>.UxmlSerializedData
		{
			[RegisterUxmlCache]
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
			}

			public override object CreateInstance()
			{
				return new LocalizedObject();
			}
		}
	}
}
