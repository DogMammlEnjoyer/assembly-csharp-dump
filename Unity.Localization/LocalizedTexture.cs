using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

namespace UnityEngine.Localization
{
	[UxmlObject]
	[Serializable]
	public class LocalizedTexture : LocalizedAsset<Texture>
	{
		protected override BindingResult ApplyDataBindingValue(in BindingContext context, Texture value)
		{
			Texture2D texture2D = value as Texture2D;
			if (texture2D != null)
			{
				return base.SetDataBindingValue<Texture2D>(context, texture2D);
			}
			return base.ApplyDataBindingValue(context, value);
		}

		[CompilerGenerated]
		[Serializable]
		public new class UxmlSerializedData : LocalizedAsset<Texture>.UxmlSerializedData
		{
			[RegisterUxmlCache]
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
			}

			public override object CreateInstance()
			{
				return new LocalizedTexture();
			}
		}
	}
}
