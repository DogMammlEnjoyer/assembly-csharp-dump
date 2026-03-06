using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	/// <summary>Defines the access modes for a dynamic assembly.</summary>
	[ComVisible(true)]
	[Flags]
	[Serializable]
	public enum AssemblyBuilderAccess
	{
		/// <summary>The dynamic assembly can be executed, but not saved.</summary>
		Run = 1,
		/// <summary>The dynamic assembly can be saved, but not executed.</summary>
		Save = 2,
		/// <summary>The dynamic assembly can be executed and saved.</summary>
		RunAndSave = 3,
		/// <summary>The dynamic assembly is loaded into the reflection-only context, and cannot be executed.</summary>
		ReflectionOnly = 6,
		/// <summary>The dynamic assembly will be automatically unloaded and its memory reclaimed, when it's no longer accessible.</summary>
		RunAndCollect = 9
	}
}
