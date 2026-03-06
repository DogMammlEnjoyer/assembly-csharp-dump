using System;
using System.Runtime.CompilerServices;

namespace VYaml.Emitter
{
	public class YamlEmitOptions
	{
		public int IndentWidth { get; set; } = 2;

		[Nullable(1)]
		public static readonly YamlEmitOptions Default = new YamlEmitOptions();
	}
}
