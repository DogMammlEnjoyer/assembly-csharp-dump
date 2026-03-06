using System;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.UserInterface
{
	internal class SeverityEntry
	{
		internal Console Owner
		{
			get
			{
				return this._owner;
			}
		}

		public ImageStyle PillStyle { get; }

		public SeverityEntry(Console owner, string label, Texture2D icon, ImageStyle imageStyle, ImageStyle pillStyle)
		{
			SeverityEntry <>4__this = this;
			this._owner = owner;
			this._countLabel = owner.RegisterCount();
			this._button = owner.RegisterControl(label, icon, imageStyle, delegate
			{
				<>4__this.ShouldShow = !<>4__this.ShouldShow;
				owner.Dirty = true;
			});
			this.Count = 0;
			this.PillStyle = pillStyle;
		}

		public void Reset()
		{
			this.Count = 0;
		}

		public bool ShouldShow
		{
			get
			{
				return this._button.State;
			}
			set
			{
				if (this._button.State == value)
				{
					return;
				}
				this._button.State = value;
				this._owner.Dirty = true;
			}
		}

		public int Count
		{
			get
			{
				return this._count;
			}
			set
			{
				if (this._count == value)
				{
					return;
				}
				this._count = value;
				this._countLabel.Content = this._count.ToString();
			}
		}

		private readonly Console _owner;

		private readonly Toggle _button;

		private readonly Label _countLabel;

		private int _count = -1;
	}
}
