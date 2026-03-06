using System;

namespace UnityEngine.Rendering
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class ReloadAttribute : Attribute
	{
		public ReloadAttribute(string[] paths, ReloadAttribute.Package package = ReloadAttribute.Package.Root)
		{
		}

		public ReloadAttribute(string path, ReloadAttribute.Package package = ReloadAttribute.Package.Root) : this(new string[]
		{
			path
		}, package)
		{
		}

		public ReloadAttribute(string pathFormat, int rangeMin, int rangeMax, ReloadAttribute.Package package = ReloadAttribute.Package.Root)
		{
		}

		public enum Package
		{
			Builtin,
			Root,
			BuiltinExtra
		}
	}
}
