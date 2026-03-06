using System;
using System.Runtime.InteropServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEditor.Analytics
{
	[ExcludeFromDocs]
	[RequiredByNativeCode(GenerateProxy = true)]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class PackageManagerStartServerPackageAnalytic : PackageManagerBaseAnalytic
	{
		public PackageManagerStartServerPackageAnalytic() : base("startPackageManagerServer")
		{
		}

		[RequiredByNativeCode]
		internal static PackageManagerStartServerPackageAnalytic CreatePackageManagerStartServerPackageAnalytic()
		{
			return new PackageManagerStartServerPackageAnalytic();
		}
	}
}
