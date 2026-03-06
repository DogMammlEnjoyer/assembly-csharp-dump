using System;

namespace UnityEngine
{
	[Obsolete("Derive collection attributes from 'PropertyAttribute' and set its 'applyToCollection' property to 'true'.", false)]
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public abstract class PropertyCollectionAttribute : PropertyAttribute
	{
		protected PropertyCollectionAttribute() : base(true)
		{
		}
	}
}
