using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Runtime.CompilerServices;

[CompilerGenerated]
[EditorBrowsable(EditorBrowsableState.Never)]
[GeneratedCode("Unity.MonoScriptGenerator.MonoScriptInfoGenerator", null)]
internal class UnitySourceGeneratedAssemblyMonoScriptTypes_v1
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static UnitySourceGeneratedAssemblyMonoScriptTypes_v1.MonoScriptData Get()
	{
		UnitySourceGeneratedAssemblyMonoScriptTypes_v1.MonoScriptData result = default(UnitySourceGeneratedAssemblyMonoScriptTypes_v1.MonoScriptData);
		result.FilePathsData = new byte[0];
		result.TypesData = new byte[0];
		result.TotalFiles = 0;
		result.TotalTypes = 0;
		result.IsEditorOnly = false;
		return result;
	}

	private struct MonoScriptData
	{
		public byte[] FilePathsData;

		public byte[] TypesData;

		public int TotalTypes;

		public int TotalFiles;

		public bool IsEditorOnly;
	}
}
