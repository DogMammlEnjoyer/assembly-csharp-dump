using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	[UsedByNativeCode]
	public abstract class PropertyAttribute : Attribute
	{
		public int order { get; set; }

		public bool applyToCollection { get; }

		protected PropertyAttribute() : this(false)
		{
		}

		protected PropertyAttribute(bool applyToCollection)
		{
			this.applyToCollection = applyToCollection;
		}
	}
}
