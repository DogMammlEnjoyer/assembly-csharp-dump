using System;
using Meta.XR.ImmersiveDebugger.Hierarchy;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.UserInterface
{
	internal class HierarchyItemButton : Flex
	{
		internal Item Item
		{
			get
			{
				return this._item;
			}
			set
			{
				this._item = value;
				this._label.Label = this._item.Label;
				if (this._item.ComputeNumberOfChildren() > 0)
				{
					this._foldout.IconStyle = Style.Load<ImageStyle>("FoldoutIcon");
				}
				else
				{
					this._foldout.IconStyle = Style.Load<ImageStyle>("None");
				}
				this.UpdateGameObjectState(true);
			}
		}

		internal int Counter
		{
			get
			{
				return this._counter;
			}
			set
			{
				this._counter = value;
				this._counter = Math.Max(0, this._counter);
			}
		}

		internal Toggle Foldout
		{
			get
			{
				return this._foldout;
			}
		}

		internal ToggleWithLabel Label
		{
			get
			{
				return this._label;
			}
		}

		protected override void Setup(Controller owner)
		{
			base.Setup(owner);
			this._foldout = base.Append<Toggle>("foldout");
			this._foldout.LayoutStyle = Style.Load<LayoutStyle>("Foldout");
			this._foldout.Icon = Resources.Load<Texture2D>("Textures/caret_right_icon");
			this._foldout.IconStyle = Style.Load<ImageStyle>("FoldoutIcon");
			this._foldout.StateChanged = new Action<bool>(this.OnStateChanged);
			this._label = base.Append<ToggleWithLabel>("label");
			this._label.LayoutStyle = Style.Load<LayoutStyle>("HierarchyItemLabel");
			this._label.TextStyle = Style.Load<TextStyle>("HierarchyItemLabel");
			this._label.BackgroundStyle = Style.Instantiate<ImageStyle>("HierarchyItemBackground");
			this._label.LabelLayoutStyle = Style.Load<LayoutStyle>("HierarchyItemLabelInner");
		}

		private void OnStateChanged(bool state)
		{
			this._foldout.Icon = Resources.Load<Texture2D>(state ? "Textures/caret_down_icon" : "Textures/caret_right_icon");
			if (state)
			{
				this.Item.BuildChildren();
				return;
			}
			this.Item.ClearChildren();
		}

		private void Update()
		{
			if (!this.Item.Valid)
			{
				this.Item.Clear();
				return;
			}
			this.UpdateGameObjectState(false);
			if (this._foldout.State && this.Item.ComputeNeedsRefresh())
			{
				this.Item.BuildChildren();
			}
		}

		private void UpdateGameObjectState(bool force = false)
		{
			GameObject gameObject = this.Item.Owner as GameObject;
			if (gameObject != null)
			{
				this.UpdateGameObjectState(gameObject.activeSelf, force);
				return;
			}
			this.UpdateGameObjectState(true, force);
		}

		private void UpdateGameObjectState(bool state, bool force = false)
		{
			if (this._previousEnabledState == state && !force)
			{
				return;
			}
			this._label.TextStyle = Style.Load<TextStyle>(state ? "HierarchyItemLabel" : "HierarchyItemLabelDeactivated");
			this._previousEnabledState = state;
		}

		private Item _item;

		private int _counter;

		private ToggleWithLabel _label;

		private Toggle _foldout;

		private bool _previousEnabledState;
	}
}
