using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.Universal
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class VolumeRequiresRendererFeatures : Attribute
	{
		public VolumeRequiresRendererFeatures(params Type[] featureTypes)
		{
			this.TargetFeatureTypes = ((featureTypes != null) ? new HashSet<Type>(featureTypes) : new HashSet<Type>());
			this.TargetFeatureTypes.Remove(null);
		}

		internal HashSet<Type> TargetFeatureTypes;
	}
}
