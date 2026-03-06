using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

namespace UnityEngine.Localization.SmartFormat.PersistentVariables
{
	[UxmlObject]
	[DisplayName("Byte", null)]
	[Serializable]
	public class ByteVariable : Variable<byte>
	{
		[CompilerGenerated]
		[DisplayName("Byte", null)]
		[Serializable]
		public new class UxmlSerializedData : Variable<byte>.UxmlSerializedData
		{
			[RegisterUxmlCache]
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
			}

			public override object CreateInstance()
			{
				return new ByteVariable();
			}
		}
	}
}
