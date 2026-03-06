using System;
using System.Collections.Generic;
using System.Text;

namespace UnityEngine.ProBuilder.KdTree
{
	[Serializable]
	internal class KdTreeNode<TKey, TValue>
	{
		public KdTreeNode()
		{
		}

		public KdTreeNode(TKey[] point, TValue value)
		{
			this.Point = point;
			this.Value = value;
		}

		internal KdTreeNode<TKey, TValue> this[int compare]
		{
			get
			{
				if (compare <= 0)
				{
					return this.LeftChild;
				}
				return this.RightChild;
			}
			set
			{
				if (compare <= 0)
				{
					this.LeftChild = value;
					return;
				}
				this.RightChild = value;
			}
		}

		public bool IsLeaf
		{
			get
			{
				return this.LeftChild == null && this.RightChild == null;
			}
		}

		public void AddDuplicate(TValue value)
		{
			if (this.Duplicates == null)
			{
				this.Duplicates = new List<TValue>
				{
					value
				};
				return;
			}
			this.Duplicates.Add(value);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.Point.Length; i++)
			{
				stringBuilder.Append(this.Point[i].ToString());
			}
			if (this.Value == null)
			{
				stringBuilder.Append("null");
			}
			else
			{
				stringBuilder.Append(this.Value.ToString());
			}
			return stringBuilder.ToString();
		}

		public TKey[] Point;

		public TValue Value;

		public List<TValue> Duplicates;

		internal KdTreeNode<TKey, TValue> LeftChild;

		internal KdTreeNode<TKey, TValue> RightChild;
	}
}
