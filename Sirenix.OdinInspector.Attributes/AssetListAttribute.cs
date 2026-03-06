using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public sealed class AssetListAttribute : Attribute
	{
		public AssetListAttribute()
		{
			this.AutoPopulate = false;
			this.Tags = null;
			this.LayerNames = null;
			this.AssetNamePrefix = null;
			this.CustomFilterMethod = null;
		}

		public bool AutoPopulate;

		public string Tags;

		public string LayerNames;

		public string AssetNamePrefix;

		public string Path;

		public string CustomFilterMethod;
	}
}
