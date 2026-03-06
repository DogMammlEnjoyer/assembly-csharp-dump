using System;
using System.Collections.Generic;
using System.Linq;

namespace Fusion
{
	public abstract class FusionGlobalScriptableObject : FusionScriptableObject
	{
		private static IEnumerable<T> GetAssemblyAttributes<T>() where T : Attribute
		{
			return new FusionGlobalScriptableObject.<GetAssemblyAttributes>d__0<T>(-2);
		}

		internal static FusionGlobalScriptableObjectSourceAttribute[] SourceAttributes
		{
			get
			{
				return FusionGlobalScriptableObject.s_sourceAttributes.Value;
			}
		}

		private static readonly Lazy<FusionGlobalScriptableObjectSourceAttribute[]> s_sourceAttributes = new Lazy<FusionGlobalScriptableObjectSourceAttribute[]>(() => (from x in FusionGlobalScriptableObject.GetAssemblyAttributes<FusionGlobalScriptableObjectSourceAttribute>()
		orderby x.Order
		select x).ToArray<FusionGlobalScriptableObjectSourceAttribute>());
	}
}
