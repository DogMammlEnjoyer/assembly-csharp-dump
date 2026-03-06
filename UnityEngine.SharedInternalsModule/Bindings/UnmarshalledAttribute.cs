using System;

namespace UnityEngine.Bindings
{
	[VisibleToOtherModules]
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	internal class UnmarshalledAttribute : Attribute, IBindingsAttribute
	{
	}
}
