using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnityEngine.Accessibility
{
	public class AccessibilityNode
	{
		internal AccessibilityNode(int id, AccessibilityHierarchy hierarchy)
		{
			this.id = id;
			this.m_Hierarchy = hierarchy;
			this.m_Children = new AccessibilityNode.ObservableList<AccessibilityNode>();
			this.m_Actions = new AccessibilityNode.ObservableList<AccessibilityAction>();
			bool flag = !this.IsInActiveHierarchy();
			if (!flag)
			{
				AccessibilityNodeData accessibilityNodeData = new AccessibilityNodeData
				{
					id = id,
					isActive = this.isActive,
					parentId = -1
				};
				this.CreateNativeNodeWithData(ref accessibilityNodeData);
				this.m_Actions.listChanged += this.ActionsChanged;
				this.m_Children.listChanged += this.ChildrenChanged;
			}
		}

		private void CreateNativeNodeWithData(ref AccessibilityNodeData nodeData)
		{
			bool isSupportedPlatform = AccessibilityManager.isSupportedPlatform;
			if (isSupportedPlatform)
			{
				while (!AccessibilityNodeManager.CreateNativeNodeWithData(nodeData))
				{
					Debug.LogWarning(string.Format("AccessibilityNode.CreateNativeNodeWithData: id '{0}' is already used", nodeData.id));
					int id = nodeData.id;
					nodeData.id = id + 1;
				}
			}
			this.id = nodeData.id;
		}

		internal void AllocateNative()
		{
			bool flag = !this.IsInActiveHierarchy();
			if (!flag)
			{
				AccessibilityNodeData accessibilityNodeData = default(AccessibilityNodeData);
				accessibilityNodeData.id = this.id;
				accessibilityNodeData.label = this.label;
				accessibilityNodeData.value = this.value;
				accessibilityNodeData.hint = this.hint;
				accessibilityNodeData.isActive = this.isActive;
				accessibilityNodeData.role = this.role;
				accessibilityNodeData.allowsDirectInteraction = this.allowsDirectInteraction;
				accessibilityNodeData.state = this.state;
				AccessibilityNode parent = this.parent;
				accessibilityNodeData.parentId = ((parent != null) ? parent.id : -1);
				accessibilityNodeData.frame = this.frame;
				accessibilityNodeData.language = this.language;
				accessibilityNodeData.implementsSelected = (this.selected != null);
				accessibilityNodeData.implementsDismissed = (this.dismissed != null);
				AccessibilityNodeData accessibilityNodeData2 = accessibilityNodeData;
				this.CreateNativeNodeWithData(ref accessibilityNodeData2);
				this.ActionsChanged();
				this.m_Actions.listChanged += this.ActionsChanged;
				foreach (AccessibilityNode accessibilityNode in this.m_Children)
				{
					accessibilityNode.AllocateNative();
				}
				this.ChildrenChanged();
				this.m_Children.listChanged += this.ChildrenChanged;
			}
		}

		internal void FreeNative(bool freeChildren)
		{
			if (freeChildren)
			{
				foreach (AccessibilityNode accessibilityNode in this.m_Children)
				{
					accessibilityNode.FreeNative(true);
				}
			}
			this.m_Children.listChanged -= this.ChildrenChanged;
			this.m_Actions.listChanged -= this.ActionsChanged;
			bool flag = this.IsInActiveHierarchy();
			if (flag)
			{
				AccessibilityNode parent = this.parent;
				int parentId = (parent != null) ? parent.id : -1;
				AccessibilityNodeManager.DestroyNativeNode(this.id, parentId);
			}
		}

		public int id { get; private set; }

		public string label
		{
			get
			{
				return this.m_Label;
			}
			set
			{
				bool flag = string.Equals(this.m_Label, value);
				if (!flag)
				{
					this.m_Label = value;
					bool flag2 = this.IsInActiveHierarchy();
					if (flag2)
					{
						AccessibilityNodeManager.SetLabel(this.id, value);
					}
				}
			}
		}

		public string value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				bool flag = string.Equals(this.m_Value, value);
				if (!flag)
				{
					this.m_Value = value;
					bool flag2 = this.IsInActiveHierarchy();
					if (flag2)
					{
						AccessibilityNodeManager.SetValue(this.id, value);
					}
				}
			}
		}

		public string hint
		{
			get
			{
				return this.m_Hint;
			}
			set
			{
				bool flag = string.Equals(this.m_Hint, value);
				if (!flag)
				{
					this.m_Hint = value;
					bool flag2 = this.IsInActiveHierarchy();
					if (flag2)
					{
						AccessibilityNodeManager.SetHint(this.id, value);
					}
				}
			}
		}

		public bool isActive
		{
			get
			{
				return this.m_IsActive;
			}
			set
			{
				bool flag = this.m_IsActive == value;
				if (!flag)
				{
					this.m_IsActive = value;
					bool flag2 = this.IsInActiveHierarchy();
					if (flag2)
					{
						AccessibilityNodeManager.SetIsActive(this.id, value);
					}
				}
			}
		}

		public AccessibilityRole role
		{
			get
			{
				return this.m_Role;
			}
			set
			{
				bool flag = this.m_Role == value;
				if (!flag)
				{
					this.m_Role = value;
					bool flag2 = this.IsInActiveHierarchy();
					if (flag2)
					{
						AccessibilityNodeManager.SetRole(this.id, value);
					}
				}
			}
		}

		public bool allowsDirectInteraction
		{
			get
			{
				return this.m_AllowsDirectInteraction;
			}
			set
			{
				bool flag = value && !Application.isEditor && Application.platform != RuntimePlatform.IPhonePlayer;
				if (flag)
				{
					throw new PlatformNotSupportedException("allowsDirectInteraction is only supported on iOS.");
				}
				bool flag2 = this.m_AllowsDirectInteraction == value;
				if (!flag2)
				{
					this.m_AllowsDirectInteraction = value;
					bool flag3 = this.IsInActiveHierarchy();
					if (flag3)
					{
						AccessibilityNodeManager.SetAllowsDirectInteraction(this.id, value);
					}
				}
			}
		}

		public AccessibilityState state
		{
			get
			{
				return this.m_State;
			}
			set
			{
				bool flag = this.m_State == value;
				if (!flag)
				{
					this.m_State = value;
					bool flag2 = this.IsInActiveHierarchy();
					if (flag2)
					{
						AccessibilityNodeManager.SetState(this.id, value);
					}
				}
			}
		}

		public AccessibilityNode parent
		{
			get
			{
				return this.m_Parent;
			}
		}

		internal void SetParent(AccessibilityNode parent, int index = -1)
		{
			this.m_Parent = parent;
			bool flag = this.IsInActiveHierarchy();
			if (flag)
			{
				int parentId = (parent != null) ? parent.id : -1;
				AccessibilityNodeManager.SetParent(this.id, parentId, index);
			}
		}

		internal IList<AccessibilityNode> childList
		{
			get
			{
				return this.m_Children;
			}
			set
			{
				bool flag = this.m_Children != null;
				if (flag)
				{
					this.m_Children.listChanged -= this.ChildrenChanged;
				}
				this.m_Children = new AccessibilityNode.ObservableList<AccessibilityNode>(value);
				this.ChildrenChanged();
				this.m_Children.listChanged += this.ChildrenChanged;
			}
		}

		public IReadOnlyList<AccessibilityNode> children
		{
			get
			{
				return this.m_Children;
			}
		}

		internal IList<AccessibilityAction> actions
		{
			get
			{
				return this.m_Actions;
			}
			set
			{
				bool flag = this.m_Actions != null;
				if (flag)
				{
					this.m_Actions.listChanged -= this.ActionsChanged;
				}
				this.m_Actions = new AccessibilityNode.ObservableList<AccessibilityAction>(value);
				this.ActionsChanged();
				this.m_Actions.listChanged += this.ActionsChanged;
			}
		}

		public Rect frame
		{
			get
			{
				bool flag = this.m_Frame == default(Rect);
				if (flag)
				{
					this.CalculateFrame();
				}
				return this.m_Frame;
			}
			set
			{
				this.SetFrame(value);
			}
		}

		private void SetFrame(Rect frame)
		{
			bool flag = this.m_Frame == frame;
			if (!flag)
			{
				this.m_Frame = frame;
				bool flag2 = this.IsInActiveHierarchy();
				if (flag2)
				{
					AccessibilityNodeManager.SetFrame(this.id, frame);
				}
			}
		}

		public Func<Rect> frameGetter
		{
			get
			{
				return this.m_FrameGetter;
			}
			set
			{
				bool flag = this.m_FrameGetter == value;
				if (!flag)
				{
					this.m_FrameGetter = value;
					bool flag2 = this.IsInActiveHierarchy();
					if (flag2)
					{
						AccessibilityNodeManager.SetFrame(this.id, this.frame);
					}
				}
			}
		}

		internal void CalculateFrame()
		{
			Func<Rect> frameGetter = this.frameGetter;
			this.SetFrame((frameGetter != null) ? frameGetter() : Rect.zero);
		}

		internal SystemLanguage language
		{
			get
			{
				return this.m_Language;
			}
			set
			{
				bool flag = this.m_Language == value;
				if (!flag)
				{
					this.m_Language = value;
					bool flag2 = this.IsInActiveHierarchy();
					if (flag2)
					{
						AccessibilityNodeManager.SetLanguage(this.id, value);
					}
				}
			}
		}

		public bool isFocused
		{
			get
			{
				return this.IsInActiveHierarchy() && AccessibilityNodeManager.GetIsFocused(this.id);
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<AccessibilityNode, bool> focusChanged;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Func<bool> selected;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action incremented;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action decremented;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Func<bool> dismissed;

		internal void GetNodeData(ref AccessibilityNodeData nodeData)
		{
			nodeData.id = this.id;
			nodeData.isActive = this.isActive;
			nodeData.label = this.label;
			nodeData.value = this.value;
			nodeData.hint = this.hint;
			nodeData.role = this.role;
			nodeData.allowsDirectInteraction = this.allowsDirectInteraction;
			nodeData.state = this.state;
			nodeData.frame = this.frame;
			AccessibilityNode parent = this.parent;
			nodeData.parentId = ((parent != null) ? parent.id : -1);
			int[] array = new int[this.m_Children.Count];
			for (int i = 0; i < this.m_Children.Count; i++)
			{
				array[i] = this.m_Children[i].id;
			}
			nodeData.childIds = array;
			nodeData.language = this.language;
			nodeData.implementsSelected = (this.selected != null);
			nodeData.implementsDismissed = (this.dismissed != null);
		}

		internal void Destroy(bool destroyChildren)
		{
			this.FreeNative(destroyChildren);
			AccessibilityNode parent = this.parent;
			if (parent != null)
			{
				parent.childList.Remove(this);
			}
			if (destroyChildren)
			{
				for (int i = this.childList.Count - 1; i >= 0; i--)
				{
					this.childList[i].Destroy(true);
				}
			}
			else
			{
				foreach (AccessibilityNode accessibilityNode in this.childList)
				{
					accessibilityNode.SetParent(this.parent, -1);
					AccessibilityNode parent2 = this.parent;
					if (parent2 != null)
					{
						parent2.childList.Add(accessibilityNode);
					}
				}
			}
			this.childList.Clear();
			this.m_Hierarchy = null;
		}

		public override int GetHashCode()
		{
			return this.id;
		}

		public override string ToString()
		{
			return string.Format("AccessibilityNode(ID: {0}, Label: {1})", this.id, this.label);
		}

		private void ChildrenChanged()
		{
			bool flag = !this.IsInActiveHierarchy();
			if (!flag)
			{
				int[] array = new int[this.m_Children.Count];
				for (int i = 0; i < this.m_Children.Count; i++)
				{
					array[i] = this.m_Children[i].id;
				}
				AccessibilityNodeManager.SetChildren(this.id, array);
			}
		}

		private void ActionsChanged()
		{
			bool flag = !this.IsInActiveHierarchy();
			if (!flag)
			{
				AccessibilityAction[] array = new AccessibilityAction[this.m_Actions.Count];
				for (int i = 0; i < this.m_Actions.Count; i++)
				{
					array[i] = this.m_Actions[i];
				}
				AccessibilityNodeManager.SetActions(this.id, array);
			}
		}

		private bool IsInActiveHierarchy()
		{
			return this.m_Hierarchy != null && AssistiveSupport.activeHierarchy == this.m_Hierarchy;
		}

		internal void NotifyFocusChanged(bool isNodeFocused)
		{
			AccessibilityManager.QueueNotification(new AccessibilityManager.NotificationContext
			{
				notification = (isNodeFocused ? AccessibilityNotification.ElementFocused : AccessibilityNotification.ElementUnfocused),
				currentNode = this
			});
		}

		internal void InvokeFocusChanged(bool isNodeFocused)
		{
			Action<AccessibilityNode, bool> action = this.focusChanged;
			if (action != null)
			{
				action(this, isNodeFocused);
			}
		}

		internal bool InvokeSelected()
		{
			Func<bool> func = this.selected;
			return func != null && func();
		}

		internal void InvokeIncremented()
		{
			Action action = this.incremented;
			if (action != null)
			{
				action();
			}
		}

		internal void InvokeDecremented()
		{
			Action action = this.decremented;
			if (action != null)
			{
				action();
			}
		}

		internal bool Dismissed()
		{
			Func<bool> func = this.dismissed;
			return func != null && func();
		}

		private Func<Rect> m_FrameGetter;

		private string m_Label;

		private string m_Value;

		private string m_Hint;

		private bool m_IsActive = true;

		private AccessibilityRole m_Role;

		private bool m_AllowsDirectInteraction;

		private AccessibilityState m_State;

		private AccessibilityNode m_Parent;

		private AccessibilityNode.ObservableList<AccessibilityNode> m_Children;

		private AccessibilityNode.ObservableList<AccessibilityAction> m_Actions;

		private Rect m_Frame;

		private SystemLanguage m_Language = SystemLanguage.Unknown;

		private AccessibilityHierarchy m_Hierarchy;

		private class ObservableList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IReadOnlyList<T>, IReadOnlyCollection<T>, IList, ICollection
		{
			public int Count
			{
				get
				{
					return this.m_Items.Count;
				}
			}

			public bool IsSynchronized
			{
				get
				{
					List<T> items = this.m_Items;
					return items != null && ((ICollection)items).IsSynchronized;
				}
			}

			public object SyncRoot
			{
				get
				{
					List<T> items = this.m_Items;
					return ((items != null) ? ((ICollection)items).SyncRoot : null) ?? false;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					List<T> items = this.m_Items;
					return items != null && ((IList)items).IsReadOnly;
				}
			}

			object IList.this[int index]
			{
				get
				{
					return this.m_Items[index];
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public ObservableList()
			{
				this.m_Items = new List<T>();
			}

			public ObservableList(IEnumerable<T> enumerable)
			{
				this.m_Items = new List<T>(enumerable);
			}

			public void CopyTo(Array array, int index)
			{
				List<T> items = this.m_Items;
				if (items != null)
				{
					((ICollection)items).CopyTo(array, index);
				}
			}

			public void Add(T item)
			{
				this.m_Items.Add(item);
				Action action = this.listChanged;
				if (action != null)
				{
					action();
				}
			}

			public void Insert(int index, T item)
			{
				this.m_Items.Insert(index, item);
				Action action = this.listChanged;
				if (action != null)
				{
					action();
				}
			}

			public void Remove(T item)
			{
				this.m_Items.Remove(item);
				Action action = this.listChanged;
				if (action != null)
				{
					action();
				}
			}

			bool ICollection<!0>.Remove(T item)
			{
				bool flag = this.m_Items.Remove(item);
				bool flag2 = flag;
				if (flag2)
				{
					Action action = this.listChanged;
					if (action != null)
					{
						action();
					}
				}
				return flag;
			}

			public void Remove(object value)
			{
				throw new NotImplementedException();
			}

			public void RemoveAt(int index)
			{
				this.m_Items.RemoveAt(index);
				Action action = this.listChanged;
				if (action != null)
				{
					action();
				}
			}

			public bool IsFixedSize { get; }

			public int Add(object value)
			{
				throw new NotImplementedException();
			}

			public void Clear()
			{
				this.m_Items.Clear();
				Action action = this.listChanged;
				if (action != null)
				{
					action();
				}
			}

			public bool Contains(object value)
			{
				throw new NotImplementedException();
			}

			public int IndexOf(object value)
			{
				throw new NotImplementedException();
			}

			public void Insert(int index, object value)
			{
				throw new NotImplementedException();
			}

			public T this[int index]
			{
				get
				{
					return this.m_Items[index];
				}
				set
				{
					this.m_Items[index] = value;
				}
			}

			public int IndexOf(T item)
			{
				return this.m_Items.IndexOf(item);
			}

			public bool Contains(T item)
			{
				return this.m_Items.Contains(item);
			}

			public void CopyTo(T[] array, int arrayIndex)
			{
				this.m_Items.CopyTo(array, arrayIndex);
			}

			public IEnumerator<T> GetEnumerator()
			{
				return this.m_Items.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.m_Items.GetEnumerator();
			}

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			public event Action listChanged;

			private readonly List<T> m_Items;
		}
	}
}
