using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements.Layout;

namespace UnityEngine.UIElements
{
	internal readonly struct VisualNode : IEnumerable<VisualNode>, IEnumerable, IEquatable<VisualNode>, IEquatable<VisualNodeHandle>
	{
		public static VisualNode Null
		{
			get
			{
				return new VisualNode(null, VisualNodeHandle.Null);
			}
		}

		public bool IsCreated
		{
			get
			{
				return !this.m_Handle.Equals(VisualNodeHandle.Null) && this.m_Manager.ContainsNode(this.m_Handle);
			}
		}

		public VisualNodeHandle Handle
		{
			get
			{
				return this.m_Handle;
			}
		}

		public bool IsRoot
		{
			get
			{
				return this.m_Handle.Id == 1;
			}
		}

		public int Id
		{
			get
			{
				return this.m_Handle.Id;
			}
		}

		public uint ControlId
		{
			get
			{
				return this.m_Manager.GetProperty<VisualNodeData>(this.m_Handle).ControlId;
			}
		}

		public VisualNode Parent
		{
			get
			{
				return new VisualNode(this.m_Manager, this.m_Manager.GetParent(this.m_Handle));
			}
		}

		public int ChildCount
		{
			get
			{
				return this.m_Manager.GetChildrenCount(this.m_Handle);
			}
		}

		public VisualNode this[int index]
		{
			get
			{
				return this.GetChildren()[index];
			}
		}

		public ref bool Enabled
		{
			get
			{
				return ref this.m_Manager.GetProperty<VisualNodeData>(this.m_Handle).Enabled;
			}
		}

		public ref VisualElementFlags Flags
		{
			get
			{
				return ref this.m_Manager.GetProperty<VisualNodeData>(this.m_Handle).Flags;
			}
		}

		public PseudoStates PseudoStates
		{
			get
			{
				return this.m_Manager.GetPseudoStates(this.m_Handle);
			}
			set
			{
				this.m_Manager.SetPseudoStates(this.m_Handle, value);
			}
		}

		public bool EnabledInHierarchy
		{
			get
			{
				return (this.PseudoStates & PseudoStates.Disabled) != PseudoStates.Disabled;
			}
		}

		public RenderHints RenderHints
		{
			get
			{
				return this.m_Manager.GetRenderHints(this.m_Handle);
			}
			set
			{
				this.m_Manager.SetRenderHints(this.m_Handle, value);
			}
		}

		public LanguageDirection LanguageDirection
		{
			get
			{
				return this.m_Manager.GetLanguageDirection(this.m_Handle);
			}
			set
			{
				this.m_Manager.SetLanguageDirection(this.m_Handle, value);
			}
		}

		public LanguageDirection LocalLanguageDirection
		{
			get
			{
				return this.m_Manager.GetLocalLanguageDirection(this.m_Handle);
			}
			set
			{
				this.m_Manager.SetLocalLanguageDirection(this.m_Handle, value);
			}
		}

		internal unsafe bool areAncestorsAndSelfDisplayed
		{
			get
			{
				return (*this.Flags & VisualElementFlags.HierarchyDisplayed) == VisualElementFlags.HierarchyDisplayed;
			}
		}

		public ref VisualNodeCallbackInterest CallbackInterest
		{
			get
			{
				return ref this.m_Manager.GetProperty<VisualNodeData>(this.m_Handle).CallbackInterest;
			}
		}

		internal VisualNode(VisualManager manager, VisualNodeHandle handle)
		{
			this.m_Manager = manager;
			this.m_Handle = handle;
		}

		internal void Destroy()
		{
			this.m_Manager.RemoveNode(this.m_Handle);
		}

		public VisualPanel GetPanel()
		{
			return new VisualPanel(this.m_Manager, this.m_Manager.GetProperty<VisualNodeData>(this.m_Handle).Panel);
		}

		public void SetPanel(VisualPanel panel)
		{
			this.m_Manager.GetProperty<VisualNodeData>(this.m_Handle).Panel = panel.Handle;
		}

		public VisualElement GetOwner()
		{
			return this.m_Manager.GetOwner(this.m_Handle);
		}

		public void SetOwner(VisualElement owner)
		{
			this.m_Manager.SetOwner(this.m_Handle, owner);
		}

		public LayoutNode GetLayout()
		{
			return this.m_Manager.GetProperty<VisualNodeData>(this.m_Handle).LayoutNode;
		}

		public void SetLayout(LayoutNode value)
		{
			this.m_Manager.GetProperty<VisualNodeData>(this.m_Handle).LayoutNode = value;
		}

		public VisualNodeChildren GetChildren()
		{
			return new VisualNodeChildren(this.m_Manager, this.m_Handle);
		}

		public void InsertChildAtIndex(int index, in VisualNode child)
		{
			this.m_Manager.InsertChildAtIndex(this.m_Handle, index, child.m_Handle);
		}

		public void AddChild(in VisualNode child)
		{
			this.m_Manager.AddChild(this.m_Handle, child.m_Handle);
		}

		public void RemoveChild(in VisualNode child)
		{
			this.m_Manager.RemoveChild(this.m_Handle, child.m_Handle);
		}

		public int IndexOfChild(in VisualNode child)
		{
			return this.m_Manager.IndexOfChild(this.m_Handle, child.m_Handle);
		}

		public void RemoveChildAtIndex(int index)
		{
			this.m_Manager.RemoveChildAtIndex(this.m_Handle, index);
		}

		public void ClearChildren()
		{
			this.m_Manager.ClearChildren(this.m_Handle);
		}

		public void RemoveFromParent()
		{
			this.m_Manager.RemoveFromParent(this.m_Handle);
		}

		public VisualNodeClassList GetClassList()
		{
			return new VisualNodeClassList(this.m_Manager, this.m_Handle);
		}

		public void AddToClassList(string className)
		{
			bool flag = string.IsNullOrEmpty(className);
			if (!flag)
			{
				this.m_Manager.AddToClassList(this.m_Handle, className);
			}
		}

		public bool RemoveFromClassList(string className)
		{
			bool flag = string.IsNullOrEmpty(className);
			return !flag && this.m_Manager.RemoveFromClassList(this.m_Handle, className);
		}

		public bool ClassListContains(string className)
		{
			bool flag = string.IsNullOrEmpty(className);
			return !flag && this.m_Manager.ClassListContains(this.m_Handle, className);
		}

		public bool ClearClassList()
		{
			return this.m_Manager.ClearClassList(this.m_Handle);
		}

		public void SetEnabled(bool value)
		{
			this.m_Manager.SetEnabled(this.m_Handle, value);
		}

		public VisualNode.Enumerator GetEnumerator()
		{
			return new VisualNode.Enumerator(ref this);
		}

		IEnumerator<VisualNode> IEnumerable<VisualNode>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public bool Equals(VisualNode other)
		{
			return this.m_Handle.Equals(other.m_Handle);
		}

		public bool Equals(VisualNodeHandle other)
		{
			return this.m_Handle.Equals(other);
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is VisualNode)
			{
				VisualNode other = (VisualNode)obj;
				result = this.Equals(other);
			}
			else if (obj is VisualNodeHandle)
			{
				VisualNodeHandle other2 = (VisualNodeHandle)obj;
				result = this.Equals(other2);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<VisualManager, VisualNodeHandle>(this.m_Manager, this.m_Handle);
		}

		private readonly VisualManager m_Manager;

		private readonly VisualNodeHandle m_Handle;

		public struct Enumerator : IEnumerator<VisualNode>, IEnumerator, IDisposable
		{
			public Enumerator(in VisualNode node)
			{
				this.m_Node = node;
				this.m_Position = -1;
			}

			public bool MoveNext()
			{
				int num = this.m_Position + 1;
				this.m_Position = num;
				return num < this.m_Node.ChildCount;
			}

			public void Reset()
			{
				this.m_Position = -1;
			}

			public VisualNode Current
			{
				get
				{
					return this.m_Node[this.m_Position];
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public void Dispose()
			{
			}

			private readonly VisualNode m_Node;

			private int m_Position;
		}
	}
}
