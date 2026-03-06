using System;

namespace UnityEngine.Rendering.UI
{
	public class DebugUIHandlerWidget : MonoBehaviour
	{
		public DebugUIHandlerWidget parentUIHandler { get; set; }

		public DebugUIHandlerWidget previousUIHandler { get; set; }

		public DebugUIHandlerWidget nextUIHandler { get; set; }

		protected virtual void OnEnable()
		{
		}

		internal virtual void SetWidget(DebugUI.Widget widget)
		{
			this.m_Widget = widget;
		}

		internal DebugUI.Widget GetWidget()
		{
			return this.m_Widget;
		}

		protected T CastWidget<T>() where T : DebugUI.Widget
		{
			T t = this.m_Widget as T;
			string text = (this.m_Widget == null) ? "null" : this.m_Widget.GetType().ToString();
			if (t == null)
			{
				string str = "Can't cast ";
				string str2 = text;
				string str3 = " to ";
				Type typeFromHandle = typeof(T);
				throw new InvalidOperationException(str + str2 + str3 + ((typeFromHandle != null) ? typeFromHandle.ToString() : null));
			}
			return t;
		}

		public virtual bool OnSelection(bool fromNext, DebugUIHandlerWidget previous)
		{
			return true;
		}

		public virtual void OnDeselection()
		{
		}

		public virtual void OnAction()
		{
		}

		public virtual void OnIncrement(bool fast)
		{
		}

		public virtual void OnDecrement(bool fast)
		{
		}

		public virtual DebugUIHandlerWidget Previous()
		{
			if (!(this.previousUIHandler != null))
			{
				return this.parentUIHandler;
			}
			return this.previousUIHandler;
		}

		public virtual DebugUIHandlerWidget Next()
		{
			if (this.nextUIHandler != null)
			{
				return this.nextUIHandler;
			}
			if (this.parentUIHandler != null)
			{
				DebugUIHandlerWidget parentUIHandler = this.parentUIHandler;
				while (parentUIHandler != null)
				{
					DebugUIHandlerWidget nextUIHandler = parentUIHandler.nextUIHandler;
					if (nextUIHandler != null)
					{
						return nextUIHandler;
					}
					parentUIHandler = parentUIHandler.parentUIHandler;
				}
			}
			return null;
		}

		[HideInInspector]
		public Color colorDefault = new Color(0.8f, 0.8f, 0.8f, 1f);

		[HideInInspector]
		public Color colorSelected = new Color(0.25f, 0.65f, 0.8f, 1f);

		protected DebugUI.Widget m_Widget;
	}
}
