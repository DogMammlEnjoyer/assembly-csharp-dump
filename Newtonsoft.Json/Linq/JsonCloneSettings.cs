using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Linq
{
	public class JsonCloneSettings
	{
		public JsonCloneSettings()
		{
			this.CopyAnnotations = true;
		}

		public bool CopyAnnotations { get; set; }

		[Nullable(1)]
		internal static readonly JsonCloneSettings SkipCopyAnnotations = new JsonCloneSettings
		{
			CopyAnnotations = false
		};
	}
}
