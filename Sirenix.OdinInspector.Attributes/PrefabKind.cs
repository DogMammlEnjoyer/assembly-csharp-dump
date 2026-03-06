using System;

namespace Sirenix.OdinInspector
{
	[Flags]
	public enum PrefabKind
	{
		None = 0,
		InstanceInScene = 1,
		InstanceInPrefab = 2,
		Regular = 4,
		Variant = 8,
		NonPrefabInstance = 16,
		PrefabInstance = 3,
		PrefabAsset = 12,
		PrefabInstanceAndNonPrefabInstance = 19,
		All = 31
	}
}
