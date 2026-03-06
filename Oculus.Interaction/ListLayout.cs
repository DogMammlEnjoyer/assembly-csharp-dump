using System;
using System.Collections.Generic;

namespace Oculus.Interaction
{
	public class ListLayout
	{
		public float Size
		{
			get
			{
				return this._size;
			}
		}

		public ListLayout()
		{
			this._root = null;
			this._elements = new Dictionary<int, ListLayout.ListElement>();
			this.WhenElementAdded = delegate(int <p0>)
			{
			};
			this.WhenElementUpdated = delegate(int <p0>, bool <p1>)
			{
			};
			this.WhenElementRemoved = delegate(int <p0>)
			{
			};
		}

		public void AddElement(int id, float size, float target = 3.4028235E+38f)
		{
			if (this._elements.ContainsKey(id))
			{
				return;
			}
			ListLayout.ListElement listElement = new ListLayout.ListElement(id, size);
			this._size += size;
			this._elements[id] = listElement;
			this.WhenElementAdded(id);
			if (this._root == null)
			{
				this._elements[id] = listElement;
				this._root = listElement;
				this.UpdatePositionsFromRoot();
				return;
			}
			ListLayout.ListElement listElement2 = this._root;
			while (listElement2.next != null)
			{
				listElement2 = listElement2.next;
			}
			listElement2.next = listElement;
			listElement.prev = listElement2;
			this.UpdatePositionsFromRoot();
			this.MoveElement(id, target);
			this.UpdatePos(listElement, listElement.pos, true);
		}

		public void RemoveElement(int id)
		{
			ListLayout.ListElement listElement;
			if (!this._elements.TryGetValue(id, out listElement))
			{
				return;
			}
			if (listElement.prev != null)
			{
				listElement.prev.next = listElement.next;
			}
			if (listElement.next != null)
			{
				listElement.next.prev = listElement.prev;
			}
			if (this._root == listElement)
			{
				if (listElement.next != null)
				{
					this._root = listElement.next;
				}
				else
				{
					this._root = null;
				}
			}
			this._size -= listElement.halfSize * 2f;
			this.UpdatePositionsFromRoot();
			this._elements.Remove(id);
			this.WhenElementRemoved(id);
		}

		private void UpdatePos(ListLayout.ListElement element, float pos, bool force = false)
		{
			if (pos != element.pos || force)
			{
				element.pos = pos;
				this.WhenElementUpdated(element.id, this._sizeUpdate || this._moveElement == element.id || force);
			}
		}

		private void UpdatePositionsFromRoot()
		{
			if (this._root == null)
			{
				return;
			}
			this.UpdatePos(this._root, this._root.halfSize - this._size / 2f, false);
			this.UpdatePositionsRight(this._root);
		}

		private void UpdatePositionsRight(ListLayout.ListElement current)
		{
			while (current.next != null)
			{
				this.UpdatePos(current.next, current.pos + current.halfSize + current.next.halfSize, false);
				current = current.next;
			}
		}

		private void SwapWithNext(ListLayout.ListElement element)
		{
			if (element.prev != null)
			{
				element.prev.next = element.next;
			}
			if (element.next.next != null)
			{
				element.next.next.prev = element;
			}
			element.next.prev = element.prev;
			element.prev = element.next;
			element.next = element.prev.next;
			element.prev.next = element;
			if (element == this._root || element.prev == this._root)
			{
				this._root = ((element == this._root) ? element.prev : element);
				this.UpdatePositionsFromRoot();
				return;
			}
			this.UpdatePos(element.prev, element.prev.prev.pos + element.prev.prev.halfSize + element.prev.halfSize, false);
			this.UpdatePos(element, element.prev.pos + element.prev.halfSize + element.halfSize, false);
		}

		private void SwapWithPrev(ListLayout.ListElement element)
		{
			this.SwapWithNext(element.prev);
		}

		public void MoveElement(int id, float target)
		{
			this._moveElement = id;
			ListLayout.ListElement listElement;
			if (!this._elements.TryGetValue(id, out listElement))
			{
				this._moveElement = -1;
				return;
			}
			if (target > listElement.pos)
			{
				while (listElement.next != null)
				{
					float num = listElement.pos + (listElement.halfSize + listElement.next.halfSize) / 2f;
					if (target < num)
					{
						break;
					}
					this.SwapWithNext(listElement);
				}
			}
			else
			{
				while (listElement.prev != null)
				{
					float num2 = listElement.pos - (listElement.halfSize + listElement.prev.halfSize) / 2f;
					if (target > num2)
					{
						break;
					}
					this.SwapWithPrev(listElement);
				}
			}
			this._moveElement = -1;
		}

		public void UpdateElementSize(int id, float size)
		{
			ListLayout.ListElement listElement;
			if (!this._elements.TryGetValue(id, out listElement))
			{
				return;
			}
			this._sizeUpdate = true;
			float num = size - listElement.halfSize * 2f;
			this._size += num;
			listElement.halfSize = size / 2f;
			this.UpdatePositionsFromRoot();
			this._sizeUpdate = false;
		}

		public float GetElementPosition(int id)
		{
			ListLayout.ListElement listElement;
			if (!this._elements.TryGetValue(id, out listElement))
			{
				return 0f;
			}
			return listElement.pos;
		}

		public float GetElementSize(int id)
		{
			ListLayout.ListElement listElement;
			if (!this._elements.TryGetValue(id, out listElement))
			{
				return 0f;
			}
			return listElement.halfSize * 2f;
		}

		public float GetTargetPosition(int id, float target, float size)
		{
			if (this._elements.ContainsKey(id))
			{
				return this.GetElementPosition(id);
			}
			if (this._root == null)
			{
				return 0f;
			}
			float num = -(this._size + size) / 2f + size / 2f;
			for (ListLayout.ListElement listElement = this._root; listElement != null; listElement = listElement.next)
			{
				float num2 = size / 2f + listElement.halfSize;
				float num3 = num + num2 / 2f;
				if (target < num3)
				{
					break;
				}
				num += num2;
			}
			return num;
		}

		private ListLayout.ListElement _root;

		private Dictionary<int, ListLayout.ListElement> _elements;

		public Action<int> WhenElementAdded;

		public Action<int, bool> WhenElementUpdated;

		public Action<int> WhenElementRemoved;

		private bool _sizeUpdate;

		private int _moveElement = -1;

		private float _size;

		public class ListElement
		{
			public ListElement(int id, float size)
			{
				this.id = id;
				this.halfSize = size / 2f;
				this.pos = 0f;
				this.prev = null;
				this.next = null;
			}

			public int id;

			public float pos;

			public float halfSize;

			public ListLayout.ListElement prev;

			public ListLayout.ListElement next;
		}
	}
}
