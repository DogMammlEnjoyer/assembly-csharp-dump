using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Meta.XR.ImmersiveDebugger.UserInterface.Generic
{
	public class Controller : MonoBehaviour
	{
		internal Controller Owner { get; set; }

		public Transform Transform { get; protected set; }

		public RectTransform RectTransform { get; protected set; }

		protected GameObject GameObject { get; set; }

		public List<Controller> Children
		{
			get
			{
				return this._children;
			}
		}

		public LayoutStyle LayoutStyle
		{
			get
			{
				return this._layoutStyle;
			}
			set
			{
				if (value == null)
				{
					return;
				}
				if (this._layoutStyle == value)
				{
					return;
				}
				this._layoutStyle = value;
				this._layoutStyleHasChanged = true;
				this.RefreshLayout();
				LayoutStyle layoutStyle = this._layoutStyle;
				if (layoutStyle != null && layoutStyle.isOverlayCanvas)
				{
					this.UpdateRefreshLayout(false);
				}
			}
		}

		public event Action<Controller> OnVisibilityChangedEvent;

		public bool Transparent
		{
			get
			{
				return this._transparent;
			}
			set
			{
				if (this._transparent == value)
				{
					return;
				}
				this._transparent = value;
				this.OnTransparencyChanged();
			}
		}

		protected virtual void OnTransparencyChanged()
		{
		}

		protected virtual void Setup(Controller owner)
		{
			this.Owner = owner;
			this.GameObject = base.gameObject;
			this.GameObject.layer = RuntimeSettings.Instance.PanelLayer;
			this.RectTransform = (this.GameObject.AddComponent<RectTransform>() ?? this.GameObject.GetComponent<RectTransform>());
			this.Transform = (this.RectTransform ? this.RectTransform : this.GameObject.transform);
			if (this.Owner != this && this.Owner != null)
			{
				this.Transform.SetParent(this.Owner.Transform, false);
			}
			this.LayoutStyle = Style.Default<LayoutStyle>();
			this._hasRectTransform = (this.RectTransform != null);
		}

		internal T Append<T>(string childName) where T : Controller, new()
		{
			T t = this.SetupChildController<T>(childName);
			if (this._children == null)
			{
				this._children = new List<Controller>();
			}
			this._children.Add(t);
			return t;
		}

		internal T Prepend<T>(string childName) where T : Controller, new()
		{
			T t = this.SetupChildController<T>(childName);
			if (this._children == null)
			{
				this._children = new List<Controller>();
			}
			this._children.Insert(0, t);
			return t;
		}

		internal T InsertAfter<T>(string childName, Controller previous) where T : Controller, new()
		{
			T t = this.SetupChildController<T>(childName);
			if (this._children == null)
			{
				this._children = new List<Controller>();
			}
			int num = this._children.IndexOf(previous);
			this._children.Insert(num + 1, t);
			return t;
		}

		internal T InsertBefore<T>(string childName, Controller next) where T : Controller, new()
		{
			T t = this.SetupChildController<T>(childName);
			if (this._children == null)
			{
				this._children = new List<Controller>();
			}
			int index = this._children.IndexOf(next);
			this._children.Insert(index, t);
			return t;
		}

		private T SetupChildController<T>(string childName) where T : Controller, new()
		{
			T t = new GameObject(childName).AddComponent<T>();
			t.Setup(this);
			return t;
		}

		protected void Append(Controller controller)
		{
			if (this._children == null)
			{
				return;
			}
			if (this._children.Contains(controller))
			{
				return;
			}
			this._children.Add(controller);
			controller.RefreshLayout();
		}

		internal void Remove(Controller controller, bool destroy)
		{
			if (this._children == null)
			{
				return;
			}
			this._children.Remove(controller);
			if (destroy)
			{
				if (Application.isPlaying)
				{
					Object.Destroy(controller.gameObject);
				}
				else
				{
					Object.DestroyImmediate(controller.gameObject);
				}
			}
			this.RefreshLayout();
		}

		protected void Clear(bool destroy)
		{
			while (this._children.Count > 0)
			{
				List<Controller> children = this._children;
				this.Remove(children[children.Count - 1], destroy);
			}
		}

		public bool Visibility
		{
			get
			{
				return this._visibility;
			}
			private set
			{
				if (this._visibility != value)
				{
					this._visibility = value;
					this.OnVisibilityChanged();
				}
			}
		}

		public void Hide()
		{
			this.Visibility = false;
		}

		public void Show()
		{
			this.Visibility = true;
		}

		internal void ToggleVisibility()
		{
			this.Visibility = !this.GameObject.activeSelf;
		}

		protected virtual void OnVisibilityChanged()
		{
			this.GameObject.SetActive(this.Visibility);
			Action<Controller> onVisibilityChangedEvent = this.OnVisibilityChangedEvent;
			if (onVisibilityChangedEvent == null)
			{
				return;
			}
			onVisibilityChangedEvent(this);
		}

		private static Vector2 GetVec2FromLayout(TextAnchor anchor)
		{
			return new Vector2((float)(anchor % TextAnchor.MiddleLeft) * 0.5f, 1f - (float)(anchor / TextAnchor.MiddleLeft) * 0.5f);
		}

		protected void UpdateRefreshLayout(bool force)
		{
			if (!force && !this._refreshLayoutRequested)
			{
				return;
			}
			this._refreshLayoutRequested = false;
			this.RefreshLayoutPreChildren();
			if (this._children != null)
			{
				bool force2 = this._layoutStyle.adaptHeight || this._layoutStyle.autoFitChildren;
				foreach (Controller controller in this._children)
				{
					controller.UpdateRefreshLayout(force2);
				}
			}
			this.RefreshLayoutPostChildren();
		}

		internal void RefreshLayout()
		{
			this._refreshLayoutRequested = true;
			Controller owner = this.Owner;
			if (owner == null)
			{
				return;
			}
			owner.RefreshLayout();
		}

		protected virtual void RefreshLayoutPreChildren()
		{
			if (!this._hasRectTransform)
			{
				return;
			}
			if (this._layoutStyleHasChanged)
			{
				this._layoutStyleHasChanged = false;
				this.RectTransform.pivot = Controller.GetVec2FromLayout(this._layoutStyle.pivot);
				this.RectTransform.anchorMin = Controller.GetVec2FromLayout(this._layoutStyle.anchor);
				this.RectTransform.anchorMax = Controller.GetVec2FromLayout(this._layoutStyle.anchor);
				switch (this._layoutStyle.layout)
				{
				case LayoutStyle.Layout.Fill:
					this.RectTransform.anchorMin = new Vector2(0f, 0f);
					this.RectTransform.anchorMax = new Vector2(1f, 1f);
					break;
				case LayoutStyle.Layout.FillHorizontal:
					this.RectTransform.anchorMin = new Vector2(0f, this.RectTransform.anchorMin.y);
					this.RectTransform.anchorMax = new Vector2(1f, this.RectTransform.anchorMax.y);
					break;
				case LayoutStyle.Layout.FillVertical:
					this.RectTransform.anchorMin = new Vector2(this.RectTransform.anchorMin.x, 0f);
					this.RectTransform.anchorMax = new Vector2(this.RectTransform.anchorMax.x, 1f);
					break;
				}
				if (this._layoutStyle.masks)
				{
					if (this._mask == null)
					{
						this._mask = this.GameObject.AddComponent<RectMask2D>();
					}
					this._mask.enabled = true;
				}
				else if (this._mask != null)
				{
					this._mask.enabled = false;
				}
			}
			Vector2 offsetMin = new Vector2(this._layoutStyle.LeftMargin, this._layoutStyle.BottomMargin);
			Vector2 offsetMax = new Vector2(-this._layoutStyle.RightMargin, -this._layoutStyle.TopMargin);
			this.RectTransform.SetSizeOptimized(offsetMin, offsetMax, this._layoutStyle.size, !this._layoutStyle.isOverlayCanvas);
		}

		protected virtual void RefreshLayoutPostChildren()
		{
			if (!this._hasRectTransform)
			{
				return;
			}
			if (this.LayoutStyle.adaptHeight)
			{
				float num = 0f;
				if (this._children != null)
				{
					foreach (Controller controller in this._children)
					{
						Flex flex = controller as Flex;
						if (flex != null)
						{
							num = Mathf.Max(num, flex.SizeDeltaWithMargin.y);
						}
					}
				}
				this.RectTransform.sizeDelta = new Vector2(this.RectTransform.sizeDelta.x, num);
			}
		}

		private void OnDestroy()
		{
			if (this.Owner != null)
			{
				this.Owner.Remove(this, false);
			}
		}

		internal void SetHeight(float height)
		{
			if (!this._layoutStyle.SetHeight(height))
			{
				return;
			}
			this.RefreshLayout();
		}

		internal void SetWidth(float width)
		{
			if (!this._layoutStyle.SetWidth(width))
			{
				return;
			}
			this.RefreshLayout();
		}

		private bool _visibility = true;

		private bool _refreshLayoutRequested;

		protected bool _hasRectTransform;

		private bool _layoutStyleHasChanged;

		[SerializeField]
		protected LayoutStyle _layoutStyle;

		protected List<Controller> _children;

		private RectMask2D _mask;

		private bool _transparent;
	}
}
