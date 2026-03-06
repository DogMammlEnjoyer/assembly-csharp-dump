using System;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.UserInterface.Generic
{
	public class LayoutStyle : Style
	{
		public float LeftMargin
		{
			get
			{
				return this.margin.x;
			}
		}

		public float TopMargin
		{
			get
			{
				return this.margin.y;
			}
		}

		public float RightMargin
		{
			get
			{
				if (!this.useBottomRightMargin)
				{
					return this.margin.x;
				}
				return this.bottomRightMargin.x;
			}
		}

		public float BottomMargin
		{
			get
			{
				if (!this.useBottomRightMargin)
				{
					return this.margin.y;
				}
				return this.bottomRightMargin.y;
			}
		}

		public Vector2 TopLeftMargin
		{
			get
			{
				return this.margin;
			}
		}

		public Vector2 BottomRightMargin
		{
			get
			{
				if (!this.useBottomRightMargin)
				{
					return this.margin;
				}
				return this.bottomRightMargin;
			}
		}

		internal bool SetHeight(float height)
		{
			if (!this._instantiated || this.size.y == height)
			{
				return false;
			}
			this.size.y = height;
			return true;
		}

		internal bool SetWidth(float width)
		{
			if (!this._instantiated || this.size.x == width)
			{
				return false;
			}
			this.size.x = width;
			return true;
		}

		internal bool SetIndent(float value)
		{
			if (!this._instantiated || this.margin.x == value)
			{
				return false;
			}
			if (!this.useBottomRightMargin)
			{
				this.useBottomRightMargin = true;
				this.bottomRightMargin.x = this.margin.x;
				this.bottomRightMargin.y = this.margin.y;
			}
			this.margin.x = value;
			return true;
		}

		public LayoutStyle.Direction flexDirection;

		public LayoutStyle.Layout layout;

		public TextAnchor anchor;

		public TextAnchor pivot;

		public Vector2 size;

		public Vector2 margin;

		public bool useBottomRightMargin;

		public Vector2 bottomRightMargin;

		public float spacing;

		public bool masks;

		public bool adaptHeight;

		public bool autoFitChildren;

		public bool isOverlayCanvas;

		public enum Layout
		{
			Fixed,
			Fill,
			FillHorizontal,
			FillVertical
		}

		public enum Direction
		{
			Left,
			Right,
			Down,
			Up
		}
	}
}
