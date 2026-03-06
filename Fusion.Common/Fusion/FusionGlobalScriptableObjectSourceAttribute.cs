using System;

namespace Fusion
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public abstract class FusionGlobalScriptableObjectSourceAttribute : Attribute
	{
		public FusionGlobalScriptableObjectSourceAttribute(Type objectType)
		{
			this.ObjectType = objectType;
		}

		public Type ObjectType { get; }

		public int Order { get; set; }

		public bool AllowEditMode { get; set; } = false;

		public bool AllowFallback { get; set; } = false;

		public abstract FusionGlobalScriptableObjectLoadResult Load(Type type);
	}
}
