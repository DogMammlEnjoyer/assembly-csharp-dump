using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[Conditional("UNITY_EDITOR")]
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class RegisterAssetReferenceAttributeForwardToChildAttribute : Attribute
	{
		public RegisterAssetReferenceAttributeForwardToChildAttribute(Type attributeType)
		{
			this.AttributeType = attributeType;
		}

		public readonly Type AttributeType;
	}
}
