using System;
using Meta.XR.ImmersiveDebugger.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Meta.XR.ImmersiveDebugger.Hierarchy
{
	internal class SceneItem : ItemWithChildren<Scene, GameObjectItem, GameObject>
	{
		protected override bool CompareChildren(GameObject lhs, GameObject rhs)
		{
			return lhs == rhs;
		}

		public override string Label
		{
			get
			{
				if (!string.IsNullOrEmpty(this._owner.name))
				{
					return this._owner.name;
				}
				return "Untitled";
			}
		}

		public override bool Valid
		{
			get
			{
				return this._owner.isLoaded;
			}
		}

		protected override InstanceHandle BuildHandle()
		{
			return new InstanceHandle(this._owner);
		}

		protected override GameObject[] FetchExpectedChildren()
		{
			if (!this._owner.isLoaded)
			{
				return Array.Empty<GameObject>();
			}
			return this._owner.GetRootGameObjects();
		}
	}
}
