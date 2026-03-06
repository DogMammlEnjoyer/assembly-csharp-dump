using System;
using System.Collections.Generic;
using Meta.XR.ImmersiveDebugger.Utils;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.Hierarchy
{
	internal class GameObjectItem : ItemWithChildren<GameObject, GameObjectItem, GameObject>
	{
		public override string Label
		{
			get
			{
				return this._owner.name;
			}
		}

		public override bool Valid
		{
			get
			{
				return this._owner != null;
			}
		}

		protected override InstanceHandle BuildHandle()
		{
			return new InstanceHandle(typeof(GameObject), this._owner);
		}

		protected override bool CompareChildren(GameObject lhs, GameObject rhs)
		{
			return lhs == rhs;
		}

		protected override GameObject[] FetchExpectedChildren()
		{
			Transform transform = this._owner.transform;
			int childCount = transform.childCount;
			GameObject[] array = new GameObject[childCount];
			for (int i = 0; i < childCount; i++)
			{
				array[i] = transform.GetChild(i).gameObject;
			}
			return array;
		}

		public override void BuildContent()
		{
			if (!this.Valid)
			{
				base.Clear();
				return;
			}
			this.BuildContentInternal();
		}

		private void BuildContentInternal()
		{
			foreach (Component owner in this._owner.GetComponents<Component>())
			{
				ComponentItem componentItem = new ComponentItem();
				componentItem.SetOwner(owner);
				this._components.Add(componentItem);
				componentItem.Register(this);
			}
		}

		public override void ClearContent()
		{
			foreach (ComponentItem componentItem in this._components)
			{
				componentItem.Clear();
			}
			this._components.Clear();
			base.ClearContent();
		}

		private readonly List<ComponentItem> _components = new List<ComponentItem>();
	}
}
