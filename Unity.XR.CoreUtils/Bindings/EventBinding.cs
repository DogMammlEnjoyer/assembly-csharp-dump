using System;

namespace Unity.XR.CoreUtils.Bindings
{
	public struct EventBinding : IEventBinding
	{
		public Action BindAction { readonly get; set; }

		public Action UnbindAction { readonly get; set; }

		public bool IsBound
		{
			get
			{
				return this.m_IsBound;
			}
		}

		public EventBinding(Action bindAction, Action unBindAction)
		{
			this.BindAction = bindAction;
			this.UnbindAction = unBindAction;
			this.m_IsBound = false;
		}

		public void Bind()
		{
			if (!this.m_IsBound)
			{
				Action bindAction = this.BindAction;
				if (bindAction != null)
				{
					bindAction();
				}
			}
			this.m_IsBound = true;
		}

		public void Unbind()
		{
			if (this.m_IsBound)
			{
				Action unbindAction = this.UnbindAction;
				if (unbindAction != null)
				{
					unbindAction();
				}
			}
			this.m_IsBound = false;
		}

		public void ClearBinding()
		{
			this.Unbind();
			this.BindAction = null;
			this.UnbindAction = null;
		}

		private bool m_IsBound;
	}
}
