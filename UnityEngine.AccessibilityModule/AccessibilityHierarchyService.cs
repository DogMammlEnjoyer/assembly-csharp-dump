using System;
using System.Collections.Generic;

namespace UnityEngine.Accessibility
{
	internal class AccessibilityHierarchyService : IService
	{
		internal AccessibilityHierarchy hierarchy
		{
			get
			{
				return this.m_Hierarchy;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					this.RemoveActiveHierarchy(true);
				}
				else
				{
					this.RemoveActiveHierarchy(false);
					this.m_Hierarchy = value;
					this.m_Hierarchy.AllocateNative();
					AssistiveSupport.notificationDispatcher.SendScreenChanged(null);
				}
			}
		}

		public void Start()
		{
		}

		public void Stop()
		{
			bool flag = this.m_Hierarchy == null;
			if (!flag)
			{
				this.RemoveActiveHierarchy(true);
			}
		}

		private void RemoveActiveHierarchy(bool notifyScreenChanged)
		{
			bool flag = this.m_Hierarchy == null;
			if (!flag)
			{
				this.m_Hierarchy.FreeNative();
				this.m_Hierarchy = null;
				if (notifyScreenChanged)
				{
					AssistiveSupport.notificationDispatcher.SendScreenChanged(null);
				}
			}
		}

		internal bool TryGetNode(int id, out AccessibilityNode node)
		{
			node = null;
			return this.m_Hierarchy != null && this.m_Hierarchy.TryGetNode(id, out node);
		}

		internal List<AccessibilityNode> GetRootNodes()
		{
			AccessibilityHierarchy hierarchy = this.m_Hierarchy;
			return (hierarchy != null) ? hierarchy.m_RootNodes : null;
		}

		internal bool TryGetNodeAt(float x, float y, out AccessibilityNode node)
		{
			node = null;
			return this.m_Hierarchy != null && this.m_Hierarchy.TryGetNodeAt(x, y, out node);
		}

		private AccessibilityHierarchy m_Hierarchy;
	}
}
