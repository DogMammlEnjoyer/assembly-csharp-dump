using System;
using UnityEngine;

namespace Fusion
{
	public class NetworkObjectPrefabData : Behaviour
	{
		private void OnValidate()
		{
			bool flag = Application.isEditor && base.gameObject.scene.IsValid();
			if (flag)
			{
				base.hideFlags |= (HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild | HideFlags.DontUnloadUnusedAsset);
			}
		}

		public NetworkObjectGuid Guid;
	}
}
