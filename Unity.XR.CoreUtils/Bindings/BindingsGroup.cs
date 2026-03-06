using System;
using System.Collections.Generic;

namespace Unity.XR.CoreUtils.Bindings
{
	public class BindingsGroup
	{
		public void AddBinding(IEventBinding binding)
		{
			this.m_Bindings.Add(binding);
		}

		public void ClearBinding(IEventBinding binding)
		{
			this.m_Bindings.Remove(binding);
			if (binding != null)
			{
				binding.ClearBinding();
			}
		}

		public void Bind()
		{
			for (int i = 0; i < this.m_Bindings.Count; i++)
			{
				IEventBinding eventBinding = this.m_Bindings[i];
				if (eventBinding != null)
				{
					eventBinding.Bind();
				}
			}
		}

		public void Unbind()
		{
			for (int i = 0; i < this.m_Bindings.Count; i++)
			{
				IEventBinding eventBinding = this.m_Bindings[i];
				if (eventBinding != null)
				{
					eventBinding.Unbind();
				}
			}
		}

		public void Clear()
		{
			for (int i = 0; i < this.m_Bindings.Count; i++)
			{
				IEventBinding eventBinding = this.m_Bindings[i];
				if (eventBinding != null)
				{
					eventBinding.ClearBinding();
				}
			}
			this.m_Bindings.Clear();
		}

		private readonly List<IEventBinding> m_Bindings = new List<IEventBinding>();
	}
}
