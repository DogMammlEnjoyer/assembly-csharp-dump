using System;
using System.Collections;

namespace System.Xml.Schema
{
	internal class ActiveAxis
	{
		public int CurrentDepth
		{
			get
			{
				return this._currentDepth;
			}
		}

		internal void Reactivate()
		{
			this._isActive = true;
			this._currentDepth = -1;
		}

		internal ActiveAxis(Asttree axisTree)
		{
			this._axisTree = axisTree;
			this._currentDepth = -1;
			this._axisStack = new ArrayList(axisTree.SubtreeArray.Count);
			for (int i = 0; i < axisTree.SubtreeArray.Count; i++)
			{
				AxisStack value = new AxisStack((ForwardAxis)axisTree.SubtreeArray[i], this);
				this._axisStack.Add(value);
			}
			this._isActive = true;
		}

		public bool MoveToStartElement(string localname, string URN)
		{
			if (!this._isActive)
			{
				return false;
			}
			this._currentDepth++;
			bool result = false;
			for (int i = 0; i < this._axisStack.Count; i++)
			{
				AxisStack axisStack = (AxisStack)this._axisStack[i];
				if (axisStack.Subtree.IsSelfAxis)
				{
					if (axisStack.Subtree.IsDss || this.CurrentDepth == 0)
					{
						result = true;
					}
				}
				else if (this.CurrentDepth != 0 && axisStack.MoveToChild(localname, URN, this._currentDepth))
				{
					result = true;
				}
			}
			return result;
		}

		public virtual bool EndElement(string localname, string URN)
		{
			if (this._currentDepth == 0)
			{
				this._isActive = false;
				this._currentDepth--;
			}
			if (!this._isActive)
			{
				return false;
			}
			for (int i = 0; i < this._axisStack.Count; i++)
			{
				((AxisStack)this._axisStack[i]).MoveToParent(localname, URN, this._currentDepth);
			}
			this._currentDepth--;
			return false;
		}

		public bool MoveToAttribute(string localname, string URN)
		{
			if (!this._isActive)
			{
				return false;
			}
			bool result = false;
			for (int i = 0; i < this._axisStack.Count; i++)
			{
				if (((AxisStack)this._axisStack[i]).MoveToAttribute(localname, URN, this._currentDepth + 1))
				{
					result = true;
				}
			}
			return result;
		}

		private int _currentDepth;

		private bool _isActive;

		private Asttree _axisTree;

		private ArrayList _axisStack;
	}
}
