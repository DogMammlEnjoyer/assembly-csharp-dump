using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security;
using System.Threading;

namespace System.Collections.Generic
{
	/// <summary>Represents a collection of objects that is maintained in sorted order.</summary>
	/// <typeparam name="T">The type of elements in the set.</typeparam>
	[DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
	[DebuggerDisplay("Count = {Count}")]
	[Serializable]
	public class SortedSet<T> : ISet<T>, ICollection<T>, IEnumerable<T>, IEnumerable, ICollection, IReadOnlyCollection<T>, ISerializable, IDeserializationCallback
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedSet`1" /> class.</summary>
		public SortedSet()
		{
			this.comparer = Comparer<T>.Default;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedSet`1" /> class that uses a specified comparer.</summary>
		/// <param name="comparer">The default comparer to use for comparing objects.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="comparer" /> is <see langword="null" />.</exception>
		public SortedSet(IComparer<T> comparer)
		{
			this.comparer = (comparer ?? Comparer<T>.Default);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedSet`1" /> class that contains elements copied from a specified enumerable collection.</summary>
		/// <param name="collection">The enumerable collection to be copied.</param>
		public SortedSet(IEnumerable<T> collection) : this(collection, Comparer<T>.Default)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedSet`1" /> class that contains elements copied from a specified enumerable collection and that uses a specified comparer.</summary>
		/// <param name="collection">The enumerable collection to be copied.</param>
		/// <param name="comparer">The default comparer to use for comparing objects.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="collection" /> is <see langword="null" />.</exception>
		public SortedSet(IEnumerable<T> collection, IComparer<T> comparer) : this(comparer)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			SortedSet<T> sortedSet = collection as SortedSet<T>;
			if (sortedSet != null && !(sortedSet is SortedSet<T>.TreeSubSet) && this.HasEqualComparer(sortedSet))
			{
				if (sortedSet.Count > 0)
				{
					this.count = sortedSet.count;
					this.root = sortedSet.root.DeepClone(this.count);
				}
				return;
			}
			int num;
			T[] array = EnumerableHelpers.ToArray<T>(collection, out num);
			if (num > 0)
			{
				comparer = this.comparer;
				Array.Sort<T>(array, 0, num, comparer);
				int num2 = 1;
				for (int i = 1; i < num; i++)
				{
					if (comparer.Compare(array[i], array[i - 1]) != 0)
					{
						array[num2++] = array[i];
					}
				}
				num = num2;
				this.root = SortedSet<T>.ConstructRootFromSortedArray(array, 0, num - 1, null);
				this.count = num;
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.SortedSet`1" /> class that contains serialized data.</summary>
		/// <param name="info">The object that contains the information that is required to serialize the <see cref="T:System.Collections.Generic.SortedSet`1" /> object.</param>
		/// <param name="context">The structure that contains the source and destination of the serialized stream associated with the <see cref="T:System.Collections.Generic.SortedSet`1" /> object.</param>
		protected SortedSet(SerializationInfo info, StreamingContext context)
		{
			this.siInfo = info;
		}

		private void AddAllElements(IEnumerable<T> collection)
		{
			foreach (T item in collection)
			{
				if (!this.Contains(item))
				{
					this.Add(item);
				}
			}
		}

		private void RemoveAllElements(IEnumerable<T> collection)
		{
			T min = this.Min;
			T max = this.Max;
			foreach (T t in collection)
			{
				if (this.comparer.Compare(t, min) >= 0 && this.comparer.Compare(t, max) <= 0 && this.Contains(t))
				{
					this.Remove(t);
				}
			}
		}

		private bool ContainsAllElements(IEnumerable<T> collection)
		{
			foreach (T item in collection)
			{
				if (!this.Contains(item))
				{
					return false;
				}
			}
			return true;
		}

		internal virtual bool InOrderTreeWalk(TreeWalkPredicate<T> action)
		{
			if (this.root == null)
			{
				return true;
			}
			Stack<SortedSet<T>.Node> stack = new Stack<SortedSet<T>.Node>(2 * SortedSet<T>.Log2(this.Count + 1));
			for (SortedSet<T>.Node node = this.root; node != null; node = node.Left)
			{
				stack.Push(node);
			}
			while (stack.Count != 0)
			{
				SortedSet<T>.Node node = stack.Pop();
				if (!action(node))
				{
					return false;
				}
				for (SortedSet<T>.Node node2 = node.Right; node2 != null; node2 = node2.Left)
				{
					stack.Push(node2);
				}
			}
			return true;
		}

		internal virtual bool BreadthFirstTreeWalk(TreeWalkPredicate<T> action)
		{
			if (this.root == null)
			{
				return true;
			}
			Queue<SortedSet<T>.Node> queue = new Queue<SortedSet<T>.Node>();
			queue.Enqueue(this.root);
			while (queue.Count != 0)
			{
				SortedSet<T>.Node node = queue.Dequeue();
				if (!action(node))
				{
					return false;
				}
				if (node.Left != null)
				{
					queue.Enqueue(node.Left);
				}
				if (node.Right != null)
				{
					queue.Enqueue(node.Right);
				}
			}
			return true;
		}

		/// <summary>Gets the number of elements in the <see cref="T:System.Collections.Generic.SortedSet`1" />.</summary>
		/// <returns>The number of elements in the <see cref="T:System.Collections.Generic.SortedSet`1" />.</returns>
		public int Count
		{
			get
			{
				this.VersionCheck();
				return this.count;
			}
		}

		/// <summary>Gets the <see cref="T:System.Collections.Generic.IComparer`1" /> object that is used to order the values in the <see cref="T:System.Collections.Generic.SortedSet`1" />.</summary>
		/// <returns>The comparer that is used to order the values in the <see cref="T:System.Collections.Generic.SortedSet`1" />.</returns>
		public IComparer<T> Comparer
		{
			get
			{
				return this.comparer;
			}
		}

		bool ICollection<!0>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets a value that indicates whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).</summary>
		/// <returns>
		///   <see langword="true" /> if access to the <see cref="T:System.Collections.ICollection" /> is synchronized; otherwise, <see langword="false" />.</returns>
		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.</summary>
		/// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />. In the default implementation of <see cref="T:System.Collections.Generic.Dictionary`2.KeyCollection" />, this property always returns the current instance.</returns>
		object ICollection.SyncRoot
		{
			get
			{
				if (this._syncRoot == null)
				{
					Interlocked.CompareExchange(ref this._syncRoot, new object(), null);
				}
				return this._syncRoot;
			}
		}

		internal virtual void VersionCheck()
		{
		}

		internal virtual bool IsWithinRange(T item)
		{
			return true;
		}

		/// <summary>Adds an element to the set and returns a value that indicates if it was successfully added.</summary>
		/// <param name="item">The element to add to the set.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="item" /> is added to the set; otherwise, <see langword="false" />.</returns>
		public bool Add(T item)
		{
			return this.AddIfNotPresent(item);
		}

		void ICollection<!0>.Add(T item)
		{
			this.Add(item);
		}

		internal virtual bool AddIfNotPresent(T item)
		{
			if (this.root == null)
			{
				this.root = new SortedSet<T>.Node(item, NodeColor.Black);
				this.count = 1;
				this.version++;
				return true;
			}
			SortedSet<T>.Node node = this.root;
			SortedSet<T>.Node node2 = null;
			SortedSet<T>.Node node3 = null;
			SortedSet<T>.Node greatGrandParent = null;
			this.version++;
			int num = 0;
			while (node != null)
			{
				num = this.comparer.Compare(item, node.Item);
				if (num == 0)
				{
					this.root.ColorBlack();
					return false;
				}
				if (node.Is4Node)
				{
					node.Split4Node();
					if (SortedSet<T>.Node.IsNonNullRed(node2))
					{
						this.InsertionBalance(node, ref node2, node3, greatGrandParent);
					}
				}
				greatGrandParent = node3;
				node3 = node2;
				node2 = node;
				node = ((num < 0) ? node.Left : node.Right);
			}
			SortedSet<T>.Node node4 = new SortedSet<T>.Node(item, NodeColor.Red);
			if (num > 0)
			{
				node2.Right = node4;
			}
			else
			{
				node2.Left = node4;
			}
			if (node2.IsRed)
			{
				this.InsertionBalance(node4, ref node2, node3, greatGrandParent);
			}
			this.root.ColorBlack();
			this.count++;
			return true;
		}

		/// <summary>Removes a specified item from the <see cref="T:System.Collections.Generic.SortedSet`1" />.</summary>
		/// <param name="item">The element to remove.</param>
		/// <returns>
		///   <see langword="true" /> if the element is found and successfully removed; otherwise, <see langword="false" />.</returns>
		public bool Remove(T item)
		{
			return this.DoRemove(item);
		}

		internal virtual bool DoRemove(T item)
		{
			if (this.root == null)
			{
				return false;
			}
			this.version++;
			SortedSet<T>.Node node = this.root;
			SortedSet<T>.Node node2 = null;
			SortedSet<T>.Node node3 = null;
			SortedSet<T>.Node node4 = null;
			SortedSet<T>.Node parentOfMatch = null;
			bool flag = false;
			while (node != null)
			{
				if (node.Is2Node)
				{
					if (node2 == null)
					{
						node.ColorRed();
					}
					else
					{
						SortedSet<T>.Node sibling = node2.GetSibling(node);
						if (sibling.IsRed)
						{
							if (node2.Right == sibling)
							{
								node2.RotateLeft();
							}
							else
							{
								node2.RotateRight();
							}
							node2.ColorRed();
							sibling.ColorBlack();
							this.ReplaceChildOrRoot(node3, node2, sibling);
							node3 = sibling;
							if (node2 == node4)
							{
								parentOfMatch = sibling;
							}
							sibling = node2.GetSibling(node);
						}
						if (sibling.Is2Node)
						{
							node2.Merge2Nodes();
						}
						else
						{
							SortedSet<T>.Node node5 = node2.Rotate(node2.GetRotation(node, sibling));
							node5.Color = node2.Color;
							node2.ColorBlack();
							node.ColorRed();
							this.ReplaceChildOrRoot(node3, node2, node5);
							if (node2 == node4)
							{
								parentOfMatch = node5;
							}
						}
					}
				}
				int num = flag ? -1 : this.comparer.Compare(item, node.Item);
				if (num == 0)
				{
					flag = true;
					node4 = node;
					parentOfMatch = node2;
				}
				node3 = node2;
				node2 = node;
				node = ((num < 0) ? node.Left : node.Right);
			}
			if (node4 != null)
			{
				this.ReplaceNode(node4, parentOfMatch, node2, node3);
				this.count--;
			}
			SortedSet<T>.Node node6 = this.root;
			if (node6 != null)
			{
				node6.ColorBlack();
			}
			return flag;
		}

		/// <summary>Removes all elements from the set.</summary>
		public virtual void Clear()
		{
			this.root = null;
			this.count = 0;
			this.version++;
		}

		/// <summary>Determines whether the set contains a specific element.</summary>
		/// <param name="item">The element to locate in the set.</param>
		/// <returns>
		///   <see langword="true" /> if the set contains <paramref name="item" />; otherwise, <see langword="false" />.</returns>
		public virtual bool Contains(T item)
		{
			return this.FindNode(item) != null;
		}

		/// <summary>Copies the complete <see cref="T:System.Collections.Generic.SortedSet`1" /> to a compatible one-dimensional array, starting at the beginning of the target array.</summary>
		/// <param name="array">A one-dimensional array that is the destination of the elements copied from the <see cref="T:System.Collections.Generic.SortedSet`1" />.</param>
		/// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.SortedSet`1" /> exceeds the number of elements that the destination array can contain.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		public void CopyTo(T[] array)
		{
			this.CopyTo(array, 0, this.Count);
		}

		/// <summary>Copies the complete <see cref="T:System.Collections.Generic.SortedSet`1" /> to a compatible one-dimensional array, starting at the specified array index.</summary>
		/// <param name="array">A one-dimensional array that is the destination of the elements copied from the <see cref="T:System.Collections.Generic.SortedSet`1" />. The array must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		/// <exception cref="T:System.ArgumentException">The number of elements in the source array is greater than the available space from <paramref name="index" /> to the end of the destination array.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> is less than zero.</exception>
		public void CopyTo(T[] array, int index)
		{
			this.CopyTo(array, index, this.Count);
		}

		/// <summary>Copies a specified number of elements from <see cref="T:System.Collections.Generic.SortedSet`1" /> to a compatible one-dimensional array, starting at the specified array index.</summary>
		/// <param name="array">A one-dimensional array that is the destination of the elements copied from the <see cref="T:System.Collections.Generic.SortedSet`1" />. The array must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		/// <param name="count">The number of elements to copy.</param>
		/// <exception cref="T:System.ArgumentException">The number of elements in the source array is greater than the available space from <paramref name="index" /> to the end of the destination array.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> is less than zero.  
		/// -or-  
		/// <paramref name="count" /> is less than zero.</exception>
		public void CopyTo(T[] array, int index, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", index, "Non-negative number required.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Non-negative number required.");
			}
			if (count > array.Length - index)
			{
				throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
			}
			count += index;
			this.InOrderTreeWalk(delegate(SortedSet<T>.Node node)
			{
				if (index >= count)
				{
					return false;
				}
				T[] array2 = array;
				int index2 = index;
				index = index2 + 1;
				array2[index2] = node.Item;
				return true;
			});
		}

		/// <summary>Copies the complete <see cref="T:System.Collections.Generic.SortedSet`1" /> to a compatible one-dimensional array, starting at the specified array index.</summary>
		/// <param name="array">A one-dimensional array that is the destination of the elements copied from the <see cref="T:System.Collections.Generic.SortedSet`1" />. The array must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		/// <exception cref="T:System.ArgumentException">The number of elements in the source array is greater than the available space from <paramref name="index" /> to the end of the destination array.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> is less than zero.</exception>
		void ICollection.CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank != 1)
			{
				throw new ArgumentException("Only single dimensional arrays are supported for the requested action.", "array");
			}
			if (array.GetLowerBound(0) != 0)
			{
				throw new ArgumentException("The lower bound of target array must be zero.", "array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", index, "Non-negative number required.");
			}
			if (array.Length - index < this.Count)
			{
				throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
			}
			T[] array2 = array as T[];
			if (array2 != null)
			{
				this.CopyTo(array2, index);
				return;
			}
			object[] objects = array as object[];
			if (objects == null)
			{
				throw new ArgumentException("Target array type is not compatible with the type of items in the collection.", "array");
			}
			try
			{
				this.InOrderTreeWalk(delegate(SortedSet<T>.Node node)
				{
					object[] objects = objects;
					int index2 = index;
					index = index2 + 1;
					objects[index2] = node.Item;
					return true;
				});
			}
			catch (ArrayTypeMismatchException)
			{
				throw new ArgumentException("Target array type is not compatible with the type of items in the collection.", "array");
			}
		}

		/// <summary>Returns an enumerator that iterates through the <see cref="T:System.Collections.Generic.SortedSet`1" />.</summary>
		/// <returns>An enumerator that iterates through the <see cref="T:System.Collections.Generic.SortedSet`1" /> in sorted order.</returns>
		public SortedSet<T>.Enumerator GetEnumerator()
		{
			return new SortedSet<T>.Enumerator(this);
		}

		IEnumerator<T> IEnumerable<!0>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		/// <summary>Returns an enumerator that iterates through a collection.</summary>
		/// <returns>An enumerator that can be used to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private void InsertionBalance(SortedSet<T>.Node current, ref SortedSet<T>.Node parent, SortedSet<T>.Node grandParent, SortedSet<T>.Node greatGrandParent)
		{
			bool flag = grandParent.Right == parent;
			bool flag2 = parent.Right == current;
			SortedSet<T>.Node node;
			if (flag == flag2)
			{
				node = (flag2 ? grandParent.RotateLeft() : grandParent.RotateRight());
			}
			else
			{
				node = (flag2 ? grandParent.RotateLeftRight() : grandParent.RotateRightLeft());
				parent = greatGrandParent;
			}
			grandParent.ColorRed();
			node.ColorBlack();
			this.ReplaceChildOrRoot(greatGrandParent, grandParent, node);
		}

		private void ReplaceChildOrRoot(SortedSet<T>.Node parent, SortedSet<T>.Node child, SortedSet<T>.Node newChild)
		{
			if (parent != null)
			{
				parent.ReplaceChild(child, newChild);
				return;
			}
			this.root = newChild;
		}

		private void ReplaceNode(SortedSet<T>.Node match, SortedSet<T>.Node parentOfMatch, SortedSet<T>.Node successor, SortedSet<T>.Node parentOfSuccessor)
		{
			if (successor == match)
			{
				successor = match.Left;
			}
			else
			{
				SortedSet<T>.Node right = successor.Right;
				if (right != null)
				{
					right.ColorBlack();
				}
				if (parentOfSuccessor != match)
				{
					parentOfSuccessor.Left = successor.Right;
					successor.Right = match.Right;
				}
				successor.Left = match.Left;
			}
			if (successor != null)
			{
				successor.Color = match.Color;
			}
			this.ReplaceChildOrRoot(parentOfMatch, match, successor);
		}

		internal virtual SortedSet<T>.Node FindNode(T item)
		{
			int num;
			for (SortedSet<T>.Node node = this.root; node != null; node = ((num < 0) ? node.Left : node.Right))
			{
				num = this.comparer.Compare(item, node.Item);
				if (num == 0)
				{
					return node;
				}
			}
			return null;
		}

		internal virtual int InternalIndexOf(T item)
		{
			SortedSet<T>.Node node = this.root;
			int num = 0;
			while (node != null)
			{
				int num2 = this.comparer.Compare(item, node.Item);
				if (num2 == 0)
				{
					return num;
				}
				node = ((num2 < 0) ? node.Left : node.Right);
				num = ((num2 < 0) ? (2 * num + 1) : (2 * num + 2));
			}
			return -1;
		}

		internal SortedSet<T>.Node FindRange(T from, T to)
		{
			return this.FindRange(from, to, true, true);
		}

		internal SortedSet<T>.Node FindRange(T from, T to, bool lowerBoundActive, bool upperBoundActive)
		{
			SortedSet<T>.Node node = this.root;
			while (node != null)
			{
				if (lowerBoundActive && this.comparer.Compare(from, node.Item) > 0)
				{
					node = node.Right;
				}
				else
				{
					if (!upperBoundActive || this.comparer.Compare(to, node.Item) >= 0)
					{
						return node;
					}
					node = node.Left;
				}
			}
			return null;
		}

		internal void UpdateVersion()
		{
			this.version++;
		}

		/// <summary>Returns an <see cref="T:System.Collections.IEqualityComparer" /> object that can be used to create a collection that contains individual sets.</summary>
		/// <returns>A comparer for creating a collection of sets.</returns>
		public static IEqualityComparer<SortedSet<T>> CreateSetComparer()
		{
			return SortedSet<T>.CreateSetComparer(null);
		}

		/// <summary>Returns an <see cref="T:System.Collections.IEqualityComparer" /> object, according to a specified comparer, that can be used to create a collection that contains individual sets.</summary>
		/// <param name="memberEqualityComparer">The comparer to use for creating the returned comparer.</param>
		/// <returns>A comparer for creating a collection of sets.</returns>
		public static IEqualityComparer<SortedSet<T>> CreateSetComparer(IEqualityComparer<T> memberEqualityComparer)
		{
			return new SortedSetEqualityComparer<T>(memberEqualityComparer);
		}

		internal static bool SortedSetEquals(SortedSet<T> set1, SortedSet<T> set2, IComparer<T> comparer)
		{
			if (set1 == null)
			{
				return set2 == null;
			}
			if (set2 == null)
			{
				return false;
			}
			if (set1.HasEqualComparer(set2))
			{
				return set1.Count == set2.Count && set1.SetEquals(set2);
			}
			bool flag = false;
			foreach (T x in set1)
			{
				flag = false;
				foreach (T y in set2)
				{
					if (comparer.Compare(x, y) == 0)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		private bool HasEqualComparer(SortedSet<T> other)
		{
			return this.Comparer == other.Comparer || this.Comparer.Equals(other.Comparer);
		}

		/// <summary>Modifies the current <see cref="T:System.Collections.Generic.SortedSet`1" /> object so that it contains all elements that are present in either the current object or the specified collection.</summary>
		/// <param name="other">The collection to compare to the current <see cref="T:System.Collections.Generic.SortedSet`1" /> object.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="other" /> is <see langword="null" />.</exception>
		public void UnionWith(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			SortedSet<T> sortedSet = other as SortedSet<T>;
			SortedSet<T>.TreeSubSet treeSubSet = this as SortedSet<T>.TreeSubSet;
			if (treeSubSet != null)
			{
				this.VersionCheck();
			}
			if (sortedSet != null && treeSubSet == null && this.count == 0)
			{
				SortedSet<T> sortedSet2 = new SortedSet<T>(sortedSet, this.comparer);
				this.root = sortedSet2.root;
				this.count = sortedSet2.count;
				this.version++;
				return;
			}
			if (sortedSet != null && treeSubSet == null && this.HasEqualComparer(sortedSet) && sortedSet.Count > this.Count / 2)
			{
				T[] array = new T[sortedSet.Count + this.Count];
				int num = 0;
				SortedSet<T>.Enumerator enumerator = this.GetEnumerator();
				SortedSet<T>.Enumerator enumerator2 = sortedSet.GetEnumerator();
				bool flag = !enumerator.MoveNext();
				bool flag2 = !enumerator2.MoveNext();
				while (!flag && !flag2)
				{
					int num2 = this.Comparer.Compare(enumerator.Current, enumerator2.Current);
					if (num2 < 0)
					{
						array[num++] = enumerator.Current;
						flag = !enumerator.MoveNext();
					}
					else if (num2 == 0)
					{
						array[num++] = enumerator2.Current;
						flag = !enumerator.MoveNext();
						flag2 = !enumerator2.MoveNext();
					}
					else
					{
						array[num++] = enumerator2.Current;
						flag2 = !enumerator2.MoveNext();
					}
				}
				if (!flag || !flag2)
				{
					SortedSet<T>.Enumerator enumerator3 = flag ? enumerator2 : enumerator;
					do
					{
						array[num++] = enumerator3.Current;
					}
					while (enumerator3.MoveNext());
				}
				this.root = null;
				this.root = SortedSet<T>.ConstructRootFromSortedArray(array, 0, num - 1, null);
				this.count = num;
				this.version++;
				return;
			}
			this.AddAllElements(other);
		}

		private static SortedSet<T>.Node ConstructRootFromSortedArray(T[] arr, int startIndex, int endIndex, SortedSet<T>.Node redNode)
		{
			int num = endIndex - startIndex + 1;
			SortedSet<T>.Node node;
			switch (num)
			{
			case 0:
				return null;
			case 1:
				node = new SortedSet<T>.Node(arr[startIndex], NodeColor.Black);
				if (redNode != null)
				{
					node.Left = redNode;
				}
				break;
			case 2:
				node = new SortedSet<T>.Node(arr[startIndex], NodeColor.Black);
				node.Right = new SortedSet<T>.Node(arr[endIndex], NodeColor.Black);
				node.Right.ColorRed();
				if (redNode != null)
				{
					node.Left = redNode;
				}
				break;
			case 3:
				node = new SortedSet<T>.Node(arr[startIndex + 1], NodeColor.Black);
				node.Left = new SortedSet<T>.Node(arr[startIndex], NodeColor.Black);
				node.Right = new SortedSet<T>.Node(arr[endIndex], NodeColor.Black);
				if (redNode != null)
				{
					node.Left.Left = redNode;
				}
				break;
			default:
			{
				int num2 = (startIndex + endIndex) / 2;
				node = new SortedSet<T>.Node(arr[num2], NodeColor.Black);
				node.Left = SortedSet<T>.ConstructRootFromSortedArray(arr, startIndex, num2 - 1, redNode);
				node.Right = ((num % 2 == 0) ? SortedSet<T>.ConstructRootFromSortedArray(arr, num2 + 2, endIndex, new SortedSet<T>.Node(arr[num2 + 1], NodeColor.Red)) : SortedSet<T>.ConstructRootFromSortedArray(arr, num2 + 1, endIndex, null));
				break;
			}
			}
			return node;
		}

		/// <summary>Modifies the current <see cref="T:System.Collections.Generic.SortedSet`1" /> object so that it contains only elements that are also in a specified collection.</summary>
		/// <param name="other">The collection to compare to the current <see cref="T:System.Collections.Generic.SortedSet`1" /> object.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="other" /> is <see langword="null" />.</exception>
		public virtual void IntersectWith(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (this.Count == 0)
			{
				return;
			}
			if (other == this)
			{
				return;
			}
			SortedSet<T> sortedSet = other as SortedSet<T>;
			SortedSet<T>.TreeSubSet treeSubSet = this as SortedSet<T>.TreeSubSet;
			if (treeSubSet != null)
			{
				this.VersionCheck();
			}
			if (sortedSet != null && treeSubSet == null && this.HasEqualComparer(sortedSet))
			{
				T[] array = new T[this.Count];
				int num = 0;
				SortedSet<T>.Enumerator enumerator = this.GetEnumerator();
				SortedSet<T>.Enumerator enumerator2 = sortedSet.GetEnumerator();
				bool flag = !enumerator.MoveNext();
				bool flag2 = !enumerator2.MoveNext();
				T max = this.Max;
				T min = this.Min;
				while (!flag && !flag2 && this.Comparer.Compare(enumerator2.Current, max) <= 0)
				{
					int num2 = this.Comparer.Compare(enumerator.Current, enumerator2.Current);
					if (num2 < 0)
					{
						flag = !enumerator.MoveNext();
					}
					else if (num2 == 0)
					{
						array[num++] = enumerator2.Current;
						flag = !enumerator.MoveNext();
						flag2 = !enumerator2.MoveNext();
					}
					else
					{
						flag2 = !enumerator2.MoveNext();
					}
				}
				this.root = null;
				this.root = SortedSet<T>.ConstructRootFromSortedArray(array, 0, num - 1, null);
				this.count = num;
				this.version++;
				return;
			}
			this.IntersectWithEnumerable(other);
		}

		internal virtual void IntersectWithEnumerable(IEnumerable<T> other)
		{
			List<T> list = new List<T>(this.Count);
			foreach (T item in other)
			{
				if (this.Contains(item))
				{
					list.Add(item);
				}
			}
			this.Clear();
			foreach (T item2 in list)
			{
				this.Add(item2);
			}
		}

		/// <summary>Removes all elements that are in a specified collection from the current <see cref="T:System.Collections.Generic.SortedSet`1" /> object.</summary>
		/// <param name="other">The collection of items to remove from the <see cref="T:System.Collections.Generic.SortedSet`1" /> object.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="other" /> is <see langword="null" />.</exception>
		public void ExceptWith(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (this.count == 0)
			{
				return;
			}
			if (other == this)
			{
				this.Clear();
				return;
			}
			SortedSet<T> sortedSet = other as SortedSet<T>;
			if (sortedSet != null && this.HasEqualComparer(sortedSet))
			{
				if (this.comparer.Compare(sortedSet.Max, this.Min) < 0 || this.comparer.Compare(sortedSet.Min, this.Max) > 0)
				{
					return;
				}
				T min = this.Min;
				T max = this.Max;
				using (IEnumerator<T> enumerator = other.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						T t = enumerator.Current;
						if (this.comparer.Compare(t, min) >= 0)
						{
							if (this.comparer.Compare(t, max) > 0)
							{
								break;
							}
							this.Remove(t);
						}
					}
					return;
				}
			}
			this.RemoveAllElements(other);
		}

		/// <summary>Modifies the current <see cref="T:System.Collections.Generic.SortedSet`1" /> object so that it contains only elements that are present either in the current object or in the specified collection, but not both.</summary>
		/// <param name="other">The collection to compare to the current <see cref="T:System.Collections.Generic.SortedSet`1" /> object.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="other" /> is <see langword="null" />.</exception>
		public void SymmetricExceptWith(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (this.Count == 0)
			{
				this.UnionWith(other);
				return;
			}
			if (other == this)
			{
				this.Clear();
				return;
			}
			SortedSet<T> sortedSet = other as SortedSet<T>;
			if (sortedSet != null && this.HasEqualComparer(sortedSet))
			{
				this.SymmetricExceptWithSameComparer(sortedSet);
				return;
			}
			int length;
			T[] array = EnumerableHelpers.ToArray<T>(other, out length);
			Array.Sort<T>(array, 0, length, this.Comparer);
			this.SymmetricExceptWithSameComparer(array, length);
		}

		private void SymmetricExceptWithSameComparer(SortedSet<T> other)
		{
			foreach (T item in other)
			{
				if (!this.Contains(item))
				{
					this.Add(item);
				}
				else
				{
					this.Remove(item);
				}
			}
		}

		private void SymmetricExceptWithSameComparer(T[] other, int count)
		{
			if (count == 0)
			{
				return;
			}
			T y = other[0];
			for (int i = 0; i < count; i++)
			{
				while (i < count && i != 0 && this.comparer.Compare(other[i], y) == 0)
				{
					i++;
				}
				if (i >= count)
				{
					break;
				}
				T t = other[i];
				if (!this.Contains(t))
				{
					this.Add(t);
				}
				else
				{
					this.Remove(t);
				}
				y = t;
			}
		}

		/// <summary>Determines whether a <see cref="T:System.Collections.Generic.SortedSet`1" /> object is a subset of the specified collection.</summary>
		/// <param name="other">The collection to compare to the current <see cref="T:System.Collections.Generic.SortedSet`1" /> object.</param>
		/// <returns>
		///   <see langword="true" /> if the current <see cref="T:System.Collections.Generic.SortedSet`1" /> object is a subset of <paramref name="other" />; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="other" /> is <see langword="null" />.</exception>
		[SecuritySafeCritical]
		public bool IsSubsetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (this.Count == 0)
			{
				return true;
			}
			SortedSet<T> sortedSet = other as SortedSet<T>;
			if (sortedSet != null && this.HasEqualComparer(sortedSet))
			{
				return this.Count <= sortedSet.Count && this.IsSubsetOfSortedSetWithSameComparer(sortedSet);
			}
			SortedSet<T>.ElementCount elementCount = this.CheckUniqueAndUnfoundElements(other, false);
			return elementCount.UniqueCount == this.Count && elementCount.UnfoundCount >= 0;
		}

		private bool IsSubsetOfSortedSetWithSameComparer(SortedSet<T> asSorted)
		{
			SortedSet<T> viewBetween = asSorted.GetViewBetween(this.Min, this.Max);
			foreach (T item in this)
			{
				if (!viewBetween.Contains(item))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>Determines whether a <see cref="T:System.Collections.Generic.SortedSet`1" /> object is a proper subset of the specified collection.</summary>
		/// <param name="other">The collection to compare to the current <see cref="T:System.Collections.Generic.SortedSet`1" /> object.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Collections.Generic.SortedSet`1" /> object is a proper subset of <paramref name="other" />; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="other" /> is <see langword="null" />.</exception>
		[SecuritySafeCritical]
		public bool IsProperSubsetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (other is ICollection && this.Count == 0)
			{
				return (other as ICollection).Count > 0;
			}
			SortedSet<T> sortedSet = other as SortedSet<T>;
			if (sortedSet != null && this.HasEqualComparer(sortedSet))
			{
				return this.Count < sortedSet.Count && this.IsSubsetOfSortedSetWithSameComparer(sortedSet);
			}
			SortedSet<T>.ElementCount elementCount = this.CheckUniqueAndUnfoundElements(other, false);
			return elementCount.UniqueCount == this.Count && elementCount.UnfoundCount > 0;
		}

		/// <summary>Determines whether a <see cref="T:System.Collections.Generic.SortedSet`1" /> object is a superset of the specified collection.</summary>
		/// <param name="other">The collection to compare to the current <see cref="T:System.Collections.Generic.SortedSet`1" /> object.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Collections.Generic.SortedSet`1" /> object is a superset of <paramref name="other" />; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="other" /> is <see langword="null" />.</exception>
		public bool IsSupersetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (other is ICollection && (other as ICollection).Count == 0)
			{
				return true;
			}
			SortedSet<T> sortedSet = other as SortedSet<T>;
			if (sortedSet == null || !this.HasEqualComparer(sortedSet))
			{
				return this.ContainsAllElements(other);
			}
			if (this.Count < sortedSet.Count)
			{
				return false;
			}
			SortedSet<T> viewBetween = this.GetViewBetween(sortedSet.Min, sortedSet.Max);
			foreach (T item in sortedSet)
			{
				if (!viewBetween.Contains(item))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>Determines whether a <see cref="T:System.Collections.Generic.SortedSet`1" /> object is a proper superset of the specified collection.</summary>
		/// <param name="other">The collection to compare to the current <see cref="T:System.Collections.Generic.SortedSet`1" /> object.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Collections.Generic.SortedSet`1" /> object is a proper superset of <paramref name="other" />; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="other" /> is <see langword="null" />.</exception>
		[SecuritySafeCritical]
		public bool IsProperSupersetOf(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (this.Count == 0)
			{
				return false;
			}
			if (other is ICollection && (other as ICollection).Count == 0)
			{
				return true;
			}
			SortedSet<T> sortedSet = other as SortedSet<T>;
			if (sortedSet == null || !this.HasEqualComparer(sortedSet))
			{
				SortedSet<T>.ElementCount elementCount = this.CheckUniqueAndUnfoundElements(other, true);
				return elementCount.UniqueCount < this.Count && elementCount.UnfoundCount == 0;
			}
			if (sortedSet.Count >= this.Count)
			{
				return false;
			}
			SortedSet<T> viewBetween = this.GetViewBetween(sortedSet.Min, sortedSet.Max);
			foreach (T item in sortedSet)
			{
				if (!viewBetween.Contains(item))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>Determines whether the current <see cref="T:System.Collections.Generic.SortedSet`1" /> object and the specified collection contain the same elements.</summary>
		/// <param name="other">The collection to compare to the current <see cref="T:System.Collections.Generic.SortedSet`1" /> object.</param>
		/// <returns>
		///   <see langword="true" /> if the current <see cref="T:System.Collections.Generic.SortedSet`1" /> object is equal to <paramref name="other" />; otherwise, false.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="other" /> is <see langword="null" />.</exception>
		[SecuritySafeCritical]
		public bool SetEquals(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			SortedSet<T> sortedSet = other as SortedSet<T>;
			if (sortedSet != null && this.HasEqualComparer(sortedSet))
			{
				SortedSet<T>.Enumerator enumerator = this.GetEnumerator();
				SortedSet<T>.Enumerator enumerator2 = sortedSet.GetEnumerator();
				bool flag = !enumerator.MoveNext();
				bool flag2 = !enumerator2.MoveNext();
				while (!flag && !flag2)
				{
					if (this.Comparer.Compare(enumerator.Current, enumerator2.Current) != 0)
					{
						return false;
					}
					flag = !enumerator.MoveNext();
					flag2 = !enumerator2.MoveNext();
				}
				return flag && flag2;
			}
			SortedSet<T>.ElementCount elementCount = this.CheckUniqueAndUnfoundElements(other, true);
			return elementCount.UniqueCount == this.Count && elementCount.UnfoundCount == 0;
		}

		/// <summary>Determines whether the current <see cref="T:System.Collections.Generic.SortedSet`1" /> object and a specified collection share common elements.</summary>
		/// <param name="other">The collection to compare to the current <see cref="T:System.Collections.Generic.SortedSet`1" /> object.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Collections.Generic.SortedSet`1" /> object and <paramref name="other" /> share at least one common element; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="other" /> is <see langword="null" />.</exception>
		public bool Overlaps(IEnumerable<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (this.Count == 0)
			{
				return false;
			}
			if (other is ICollection<!0> && (other as ICollection<!0>).Count == 0)
			{
				return false;
			}
			SortedSet<T> sortedSet = other as SortedSet<T>;
			if (sortedSet != null && this.HasEqualComparer(sortedSet) && (this.comparer.Compare(this.Min, sortedSet.Max) > 0 || this.comparer.Compare(this.Max, sortedSet.Min) < 0))
			{
				return false;
			}
			foreach (T item in other)
			{
				if (this.Contains(item))
				{
					return true;
				}
			}
			return false;
		}

		private unsafe SortedSet<T>.ElementCount CheckUniqueAndUnfoundElements(IEnumerable<T> other, bool returnIfUnfound)
		{
			SortedSet<T>.ElementCount result;
			if (this.Count == 0)
			{
				int num = 0;
				using (IEnumerator<T> enumerator = other.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						T t = enumerator.Current;
						num++;
					}
				}
				result.UniqueCount = 0;
				result.UnfoundCount = num;
				return result;
			}
			int num2 = BitHelper.ToIntArrayLength(this.Count);
			BitHelper bitHelper;
			if (num2 <= 100)
			{
				bitHelper = new BitHelper(stackalloc int[checked(unchecked((UIntPtr)num2) * 4)], num2);
			}
			else
			{
				bitHelper = new BitHelper(new int[num2], num2);
			}
			int num3 = 0;
			int num4 = 0;
			foreach (T item in other)
			{
				int num5 = this.InternalIndexOf(item);
				if (num5 >= 0)
				{
					if (!bitHelper.IsMarked(num5))
					{
						bitHelper.MarkBit(num5);
						num4++;
					}
				}
				else
				{
					num3++;
					if (returnIfUnfound)
					{
						break;
					}
				}
			}
			result.UniqueCount = num4;
			result.UnfoundCount = num3;
			return result;
		}

		/// <summary>Removes all elements that match the conditions defined by the specified predicate from a <see cref="T:System.Collections.Generic.SortedSet`1" />.</summary>
		/// <param name="match">The delegate that defines the conditions of the elements to remove.</param>
		/// <returns>The number of elements that were removed from the <see cref="T:System.Collections.Generic.SortedSet`1" /> collection.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="match" /> is <see langword="null" />.</exception>
		public int RemoveWhere(Predicate<T> match)
		{
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
			List<T> matches = new List<T>(this.Count);
			this.BreadthFirstTreeWalk(delegate(SortedSet<T>.Node n)
			{
				if (match(n.Item))
				{
					matches.Add(n.Item);
				}
				return true;
			});
			int num = 0;
			for (int i = matches.Count - 1; i >= 0; i--)
			{
				if (this.Remove(matches[i]))
				{
					num++;
				}
			}
			return num;
		}

		/// <summary>Gets the minimum value in the <see cref="T:System.Collections.Generic.SortedSet`1" />, as defined by the comparer.</summary>
		/// <returns>The minimum value in the set.</returns>
		public T Min
		{
			get
			{
				return this.MinInternal;
			}
		}

		internal virtual T MinInternal
		{
			get
			{
				if (this.root == null)
				{
					return default(T);
				}
				SortedSet<T>.Node left = this.root;
				while (left.Left != null)
				{
					left = left.Left;
				}
				return left.Item;
			}
		}

		/// <summary>Gets the maximum value in the <see cref="T:System.Collections.Generic.SortedSet`1" />, as defined by the comparer.</summary>
		/// <returns>The maximum value in the set.</returns>
		public T Max
		{
			get
			{
				return this.MaxInternal;
			}
		}

		internal virtual T MaxInternal
		{
			get
			{
				if (this.root == null)
				{
					return default(T);
				}
				SortedSet<T>.Node right = this.root;
				while (right.Right != null)
				{
					right = right.Right;
				}
				return right.Item;
			}
		}

		/// <summary>Returns an <see cref="T:System.Collections.Generic.IEnumerable`1" /> that iterates over the <see cref="T:System.Collections.Generic.SortedSet`1" /> in reverse order.</summary>
		/// <returns>An enumerator that iterates over the <see cref="T:System.Collections.Generic.SortedSet`1" /> in reverse order.</returns>
		public IEnumerable<T> Reverse()
		{
			SortedSet<T>.Enumerator e = new SortedSet<T>.Enumerator(this, true);
			while (e.MoveNext())
			{
				T t = e.Current;
				yield return t;
			}
			yield break;
		}

		/// <summary>Returns a view of a subset in a <see cref="T:System.Collections.Generic.SortedSet`1" />.</summary>
		/// <param name="lowerValue">The lowest desired value in the view.</param>
		/// <param name="upperValue">The highest desired value in the view.</param>
		/// <returns>A subset view that contains only the values in the specified range.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="lowerValue" /> is more than <paramref name="upperValue" /> according to the comparer.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">A tried operation on the view was outside the range specified by <paramref name="lowerValue" /> and <paramref name="upperValue" />.</exception>
		public virtual SortedSet<T> GetViewBetween(T lowerValue, T upperValue)
		{
			if (this.Comparer.Compare(lowerValue, upperValue) > 0)
			{
				throw new ArgumentException("Must be less than or equal to upperValue.", "lowerValue");
			}
			return new SortedSet<T>.TreeSubSet(this, lowerValue, upperValue, true, true);
		}

		/// <summary>Implements the <see cref="T:System.Runtime.Serialization.ISerializable" /> interface, and returns the data that you need to serialize the <see cref="T:System.Collections.Generic.SortedSet`1" /> instance.</summary>
		/// <param name="info">A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object that contains the information that is required to serialize the <see cref="T:System.Collections.Generic.SortedSet`1" /> instance.</param>
		/// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> structure that contains the source and destination of the serialized stream associated with the <see cref="T:System.Collections.Generic.SortedSet`1" /> instance.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="info" /> is <see langword="null" />.</exception>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			this.GetObjectData(info, context);
		}

		/// <summary>Implements the <see cref="T:System.Runtime.Serialization.ISerializable" /> interface and returns the data that you must have to serialize a <see cref="T:System.Collections.Generic.SortedSet`1" /> object.</summary>
		/// <param name="info">A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object that contains the information that is required to serialize the <see cref="T:System.Collections.Generic.SortedSet`1" /> object.</param>
		/// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> structure that contains the source and destination of the serialized stream associated with the <see cref="T:System.Collections.Generic.SortedSet`1" /> object.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="info" /> is <see langword="null" />.</exception>
		protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("Count", this.count);
			info.AddValue("Comparer", this.comparer, typeof(IComparer<T>));
			info.AddValue("Version", this.version);
			if (this.root != null)
			{
				T[] array = new T[this.Count];
				this.CopyTo(array, 0);
				info.AddValue("Items", array, typeof(T[]));
			}
		}

		/// <summary>Implements the <see cref="T:System.Runtime.Serialization.IDeserializationCallback" /> interface, and raises the deserialization event when the deserialization is completed.</summary>
		/// <param name="sender">The source of the deserialization event.</param>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object associated with the current <see cref="T:System.Collections.Generic.SortedSet`1" /> instance is invalid.</exception>
		void IDeserializationCallback.OnDeserialization(object sender)
		{
			this.OnDeserialization(sender);
		}

		/// <summary>Implements the <see cref="T:System.Runtime.Serialization.ISerializable" /> interface, and raises the deserialization event when the deserialization is completed.</summary>
		/// <param name="sender">The source of the deserialization event.</param>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object associated with the current <see cref="T:System.Collections.Generic.SortedSet`1" /> object is invalid.</exception>
		protected virtual void OnDeserialization(object sender)
		{
			if (this.comparer != null)
			{
				return;
			}
			if (this.siInfo == null)
			{
				throw new SerializationException("OnDeserialization method was called while the object was not being deserialized.");
			}
			this.comparer = (IComparer<T>)this.siInfo.GetValue("Comparer", typeof(IComparer<T>));
			int @int = this.siInfo.GetInt32("Count");
			if (@int != 0)
			{
				T[] array = (T[])this.siInfo.GetValue("Items", typeof(T[]));
				if (array == null)
				{
					throw new SerializationException("The values for this dictionary are missing.");
				}
				for (int i = 0; i < array.Length; i++)
				{
					this.Add(array[i]);
				}
			}
			this.version = this.siInfo.GetInt32("Version");
			if (this.count != @int)
			{
				throw new SerializationException("The serialized Count information doesn't match the number of items.");
			}
			this.siInfo = null;
		}

		/// <summary>Searches the set for a given value and returns the equal value it finds, if any.</summary>
		/// <param name="equalValue">The value to search for.</param>
		/// <param name="actualValue">The value from the set that the search found, or the default value of T when the search yielded no match.</param>
		/// <returns>A value indicating whether the search was successful.</returns>
		public bool TryGetValue(T equalValue, out T actualValue)
		{
			SortedSet<T>.Node node = this.FindNode(equalValue);
			if (node != null)
			{
				actualValue = node.Item;
				return true;
			}
			actualValue = default(T);
			return false;
		}

		private static int Log2(int value)
		{
			int num = 0;
			while (value > 0)
			{
				num++;
				value >>= 1;
			}
			return num;
		}

		private SortedSet<T>.Node root;

		private IComparer<T> comparer;

		private int count;

		private int version;

		[NonSerialized]
		private object _syncRoot;

		private SerializationInfo siInfo;

		private const string ComparerName = "Comparer";

		private const string CountName = "Count";

		private const string ItemsName = "Items";

		private const string VersionName = "Version";

		private const string TreeName = "Tree";

		private const string NodeValueName = "Item";

		private const string EnumStartName = "EnumStarted";

		private const string ReverseName = "Reverse";

		private const string EnumVersionName = "EnumVersion";

		private const string MinName = "Min";

		private const string MaxName = "Max";

		private const string LowerBoundActiveName = "lBoundActive";

		private const string UpperBoundActiveName = "uBoundActive";

		internal const int StackAllocThreshold = 100;

		[Serializable]
		internal sealed class TreeSubSet : SortedSet<T>, ISerializable, IDeserializationCallback
		{
			public TreeSubSet(SortedSet<T> Underlying, T Min, T Max, bool lowerBoundActive, bool upperBoundActive) : base(Underlying.Comparer)
			{
				this._underlying = Underlying;
				this._min = Min;
				this._max = Max;
				this._lBoundActive = lowerBoundActive;
				this._uBoundActive = upperBoundActive;
				this.root = this._underlying.FindRange(this._min, this._max, this._lBoundActive, this._uBoundActive);
				this.count = 0;
				this.version = -1;
				this.VersionCheckImpl();
			}

			internal override bool AddIfNotPresent(T item)
			{
				if (!this.IsWithinRange(item))
				{
					throw new ArgumentOutOfRangeException("item");
				}
				bool result = this._underlying.AddIfNotPresent(item);
				this.VersionCheck();
				return result;
			}

			public override bool Contains(T item)
			{
				this.VersionCheck();
				return base.Contains(item);
			}

			internal override bool DoRemove(T item)
			{
				if (!this.IsWithinRange(item))
				{
					return false;
				}
				bool result = this._underlying.Remove(item);
				this.VersionCheck();
				return result;
			}

			public override void Clear()
			{
				if (this.count == 0)
				{
					return;
				}
				List<T> toRemove = new List<T>();
				this.BreadthFirstTreeWalk(delegate(SortedSet<T>.Node n)
				{
					toRemove.Add(n.Item);
					return true;
				});
				while (toRemove.Count != 0)
				{
					this._underlying.Remove(toRemove[toRemove.Count - 1]);
					toRemove.RemoveAt(toRemove.Count - 1);
				}
				this.root = null;
				this.count = 0;
				this.version = this._underlying.version;
			}

			internal override bool IsWithinRange(T item)
			{
				return (this._lBoundActive ? base.Comparer.Compare(this._min, item) : -1) <= 0 && (this._uBoundActive ? base.Comparer.Compare(this._max, item) : 1) >= 0;
			}

			internal override T MinInternal
			{
				get
				{
					SortedSet<T>.Node node = this.root;
					T result = default(T);
					while (node != null)
					{
						int num = this._lBoundActive ? base.Comparer.Compare(this._min, node.Item) : -1;
						if (num == 1)
						{
							node = node.Right;
						}
						else
						{
							result = node.Item;
							if (num == 0)
							{
								break;
							}
							node = node.Left;
						}
					}
					return result;
				}
			}

			internal override T MaxInternal
			{
				get
				{
					SortedSet<T>.Node node = this.root;
					T result = default(T);
					while (node != null)
					{
						int num = this._uBoundActive ? base.Comparer.Compare(this._max, node.Item) : 1;
						if (num == -1)
						{
							node = node.Left;
						}
						else
						{
							result = node.Item;
							if (num == 0)
							{
								break;
							}
							node = node.Right;
						}
					}
					return result;
				}
			}

			internal override bool InOrderTreeWalk(TreeWalkPredicate<T> action)
			{
				this.VersionCheck();
				if (this.root == null)
				{
					return true;
				}
				Stack<SortedSet<T>.Node> stack = new Stack<SortedSet<T>.Node>(2 * SortedSet<T>.Log2(this.count + 1));
				SortedSet<T>.Node node = this.root;
				while (node != null)
				{
					if (this.IsWithinRange(node.Item))
					{
						stack.Push(node);
						node = node.Left;
					}
					else if (this._lBoundActive && base.Comparer.Compare(this._min, node.Item) > 0)
					{
						node = node.Right;
					}
					else
					{
						node = node.Left;
					}
				}
				while (stack.Count != 0)
				{
					node = stack.Pop();
					if (!action(node))
					{
						return false;
					}
					SortedSet<T>.Node node2 = node.Right;
					while (node2 != null)
					{
						if (this.IsWithinRange(node2.Item))
						{
							stack.Push(node2);
							node2 = node2.Left;
						}
						else if (this._lBoundActive && base.Comparer.Compare(this._min, node2.Item) > 0)
						{
							node2 = node2.Right;
						}
						else
						{
							node2 = node2.Left;
						}
					}
				}
				return true;
			}

			internal override bool BreadthFirstTreeWalk(TreeWalkPredicate<T> action)
			{
				this.VersionCheck();
				if (this.root == null)
				{
					return true;
				}
				Queue<SortedSet<T>.Node> queue = new Queue<SortedSet<T>.Node>();
				queue.Enqueue(this.root);
				while (queue.Count != 0)
				{
					SortedSet<T>.Node node = queue.Dequeue();
					if (this.IsWithinRange(node.Item) && !action(node))
					{
						return false;
					}
					if (node.Left != null && (!this._lBoundActive || base.Comparer.Compare(this._min, node.Item) < 0))
					{
						queue.Enqueue(node.Left);
					}
					if (node.Right != null && (!this._uBoundActive || base.Comparer.Compare(this._max, node.Item) > 0))
					{
						queue.Enqueue(node.Right);
					}
				}
				return true;
			}

			internal override SortedSet<T>.Node FindNode(T item)
			{
				if (!this.IsWithinRange(item))
				{
					return null;
				}
				this.VersionCheck();
				return base.FindNode(item);
			}

			internal override int InternalIndexOf(T item)
			{
				int num = -1;
				foreach (T y in this)
				{
					num++;
					if (base.Comparer.Compare(item, y) == 0)
					{
						return num;
					}
				}
				return -1;
			}

			internal override void VersionCheck()
			{
				this.VersionCheckImpl();
			}

			private void VersionCheckImpl()
			{
				if (this.version != this._underlying.version)
				{
					this.root = this._underlying.FindRange(this._min, this._max, this._lBoundActive, this._uBoundActive);
					this.version = this._underlying.version;
					this.count = 0;
					this.InOrderTreeWalk(delegate(SortedSet<T>.Node n)
					{
						this.count++;
						return true;
					});
				}
			}

			public override SortedSet<T> GetViewBetween(T lowerValue, T upperValue)
			{
				if (this._lBoundActive && base.Comparer.Compare(this._min, lowerValue) > 0)
				{
					throw new ArgumentOutOfRangeException("lowerValue");
				}
				if (this._uBoundActive && base.Comparer.Compare(this._max, upperValue) < 0)
				{
					throw new ArgumentOutOfRangeException("upperValue");
				}
				return (SortedSet<T>.TreeSubSet)this._underlying.GetViewBetween(lowerValue, upperValue);
			}

			void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
			{
				this.GetObjectData(info, context);
			}

			protected override void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				throw new PlatformNotSupportedException();
			}

			void IDeserializationCallback.OnDeserialization(object sender)
			{
				throw new PlatformNotSupportedException();
			}

			protected override void OnDeserialization(object sender)
			{
				throw new PlatformNotSupportedException();
			}

			private SortedSet<T> _underlying;

			private T _min;

			private T _max;

			private bool _lBoundActive;

			private bool _uBoundActive;
		}

		[Serializable]
		internal sealed class Node
		{
			public Node(T item, NodeColor color)
			{
				this.Item = item;
				this.Color = color;
			}

			public static bool IsNonNullBlack(SortedSet<T>.Node node)
			{
				return node != null && node.IsBlack;
			}

			public static bool IsNonNullRed(SortedSet<T>.Node node)
			{
				return node != null && node.IsRed;
			}

			public static bool IsNullOrBlack(SortedSet<T>.Node node)
			{
				return node == null || node.IsBlack;
			}

			public T Item { get; set; }

			public SortedSet<T>.Node Left { get; set; }

			public SortedSet<T>.Node Right { get; set; }

			public NodeColor Color { get; set; }

			public bool IsBlack
			{
				get
				{
					return this.Color == NodeColor.Black;
				}
			}

			public bool IsRed
			{
				get
				{
					return this.Color == NodeColor.Red;
				}
			}

			public bool Is2Node
			{
				get
				{
					return this.IsBlack && SortedSet<T>.Node.IsNullOrBlack(this.Left) && SortedSet<T>.Node.IsNullOrBlack(this.Right);
				}
			}

			public bool Is4Node
			{
				get
				{
					return SortedSet<T>.Node.IsNonNullRed(this.Left) && SortedSet<T>.Node.IsNonNullRed(this.Right);
				}
			}

			public void ColorBlack()
			{
				this.Color = NodeColor.Black;
			}

			public void ColorRed()
			{
				this.Color = NodeColor.Red;
			}

			public SortedSet<T>.Node DeepClone(int count)
			{
				Stack<SortedSet<T>.Node> stack = new Stack<SortedSet<T>.Node>(2 * SortedSet<T>.Log2(count) + 2);
				Stack<SortedSet<T>.Node> stack2 = new Stack<SortedSet<T>.Node>(2 * SortedSet<T>.Log2(count) + 2);
				SortedSet<T>.Node node = this.ShallowClone();
				SortedSet<T>.Node node2 = this;
				SortedSet<T>.Node node3 = node;
				while (node2 != null)
				{
					stack.Push(node2);
					stack2.Push(node3);
					SortedSet<T>.Node node4 = node3;
					SortedSet<T>.Node left = node2.Left;
					node4.Left = ((left != null) ? left.ShallowClone() : null);
					node2 = node2.Left;
					node3 = node3.Left;
				}
				while (stack.Count != 0)
				{
					node2 = stack.Pop();
					node3 = stack2.Pop();
					SortedSet<T>.Node node5 = node2.Right;
					SortedSet<T>.Node node6 = (node5 != null) ? node5.ShallowClone() : null;
					node3.Right = node6;
					while (node5 != null)
					{
						stack.Push(node5);
						stack2.Push(node6);
						SortedSet<T>.Node node7 = node6;
						SortedSet<T>.Node left2 = node5.Left;
						node7.Left = ((left2 != null) ? left2.ShallowClone() : null);
						node5 = node5.Left;
						node6 = node6.Left;
					}
				}
				return node;
			}

			public TreeRotation GetRotation(SortedSet<T>.Node current, SortedSet<T>.Node sibling)
			{
				bool flag = this.Left == current;
				if (!SortedSet<T>.Node.IsNonNullRed(sibling.Left))
				{
					if (!flag)
					{
						return TreeRotation.LeftRight;
					}
					return TreeRotation.Left;
				}
				else
				{
					if (!flag)
					{
						return TreeRotation.Right;
					}
					return TreeRotation.RightLeft;
				}
			}

			public SortedSet<T>.Node GetSibling(SortedSet<T>.Node node)
			{
				if (node != this.Left)
				{
					return this.Left;
				}
				return this.Right;
			}

			public SortedSet<T>.Node ShallowClone()
			{
				return new SortedSet<T>.Node(this.Item, this.Color);
			}

			public void Split4Node()
			{
				this.ColorRed();
				this.Left.ColorBlack();
				this.Right.ColorBlack();
			}

			public SortedSet<T>.Node Rotate(TreeRotation rotation)
			{
				switch (rotation)
				{
				case TreeRotation.Left:
					this.Right.Right.ColorBlack();
					return this.RotateLeft();
				case TreeRotation.LeftRight:
					return this.RotateLeftRight();
				case TreeRotation.Right:
					this.Left.Left.ColorBlack();
					return this.RotateRight();
				case TreeRotation.RightLeft:
					return this.RotateRightLeft();
				default:
					return null;
				}
			}

			public SortedSet<T>.Node RotateLeft()
			{
				SortedSet<T>.Node right = this.Right;
				this.Right = right.Left;
				right.Left = this;
				return right;
			}

			public SortedSet<T>.Node RotateLeftRight()
			{
				SortedSet<T>.Node left = this.Left;
				SortedSet<T>.Node right = left.Right;
				this.Left = right.Right;
				right.Right = this;
				left.Right = right.Left;
				right.Left = left;
				return right;
			}

			public SortedSet<T>.Node RotateRight()
			{
				SortedSet<T>.Node left = this.Left;
				this.Left = left.Right;
				left.Right = this;
				return left;
			}

			public SortedSet<T>.Node RotateRightLeft()
			{
				SortedSet<T>.Node right = this.Right;
				SortedSet<T>.Node left = right.Left;
				this.Right = left.Left;
				left.Left = this;
				right.Left = left.Right;
				left.Right = right;
				return left;
			}

			public void Merge2Nodes()
			{
				this.ColorBlack();
				this.Left.ColorRed();
				this.Right.ColorRed();
			}

			public void ReplaceChild(SortedSet<T>.Node child, SortedSet<T>.Node newChild)
			{
				if (this.Left == child)
				{
					this.Left = newChild;
					return;
				}
				this.Right = newChild;
			}
		}

		/// <summary>Enumerates the elements of a <see cref="T:System.Collections.Generic.SortedSet`1" /> object.</summary>
		/// <typeparam name="T" />
		[Serializable]
		public struct Enumerator : IEnumerator<!0>, IDisposable, IEnumerator, ISerializable, IDeserializationCallback
		{
			internal Enumerator(SortedSet<T> set)
			{
				this = new SortedSet<T>.Enumerator(set, false);
			}

			internal Enumerator(SortedSet<T> set, bool reverse)
			{
				this._tree = set;
				set.VersionCheck();
				this._version = set.version;
				this._stack = new Stack<SortedSet<T>.Node>(2 * SortedSet<T>.Log2(set.Count + 1));
				this._current = null;
				this._reverse = reverse;
				this.Initialize();
			}

			/// <summary>Implements the <see cref="T:System.Runtime.Serialization.ISerializable" /> interface and returns the data needed to serialize the <see cref="T:System.Collections.Generic.SortedSet`1" /> instance.</summary>
			/// <param name="info">A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object that contains the information required to serialize the <see cref="T:System.Collections.Generic.SortedSet`1" /> instance.</param>
			/// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> object that contains the source and destination of the serialized stream associated with the <see cref="T:System.Collections.Generic.SortedSet`1" /> instance.</param>
			/// <exception cref="T:System.ArgumentNullException">
			///   <paramref name="info" /> is <see langword="null" />.</exception>
			void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
			{
				throw new PlatformNotSupportedException();
			}

			/// <summary>Implements the <see cref="T:System.Runtime.Serialization.ISerializable" /> interface and raises the deserialization event when the deserialization is complete.</summary>
			/// <param name="sender">The source of the deserialization event.</param>
			/// <exception cref="T:System.Runtime.Serialization.SerializationException">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object associated with the current <see cref="T:System.Collections.Generic.SortedSet`1" /> instance is invalid.</exception>
			void IDeserializationCallback.OnDeserialization(object sender)
			{
				throw new PlatformNotSupportedException();
			}

			private void Initialize()
			{
				this._current = null;
				SortedSet<T>.Node node = this._tree.root;
				while (node != null)
				{
					SortedSet<T>.Node node2 = this._reverse ? node.Right : node.Left;
					SortedSet<T>.Node node3 = this._reverse ? node.Left : node.Right;
					if (this._tree.IsWithinRange(node.Item))
					{
						this._stack.Push(node);
						node = node2;
					}
					else if (node2 == null || !this._tree.IsWithinRange(node2.Item))
					{
						node = node3;
					}
					else
					{
						node = node2;
					}
				}
			}

			/// <summary>Advances the enumerator to the next element of the <see cref="T:System.Collections.Generic.SortedSet`1" /> collection.</summary>
			/// <returns>
			///   <see langword="true" /> if the enumerator was successfully advanced to the next element; <see langword="false" /> if the enumerator has passed the end of the collection.</returns>
			/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created.</exception>
			public bool MoveNext()
			{
				this._tree.VersionCheck();
				if (this._version != this._tree.version)
				{
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
				}
				if (this._stack.Count == 0)
				{
					this._current = null;
					return false;
				}
				this._current = this._stack.Pop();
				SortedSet<T>.Node node = this._reverse ? this._current.Left : this._current.Right;
				while (node != null)
				{
					SortedSet<T>.Node node2 = this._reverse ? node.Right : node.Left;
					SortedSet<T>.Node node3 = this._reverse ? node.Left : node.Right;
					if (this._tree.IsWithinRange(node.Item))
					{
						this._stack.Push(node);
						node = node2;
					}
					else if (node3 == null || !this._tree.IsWithinRange(node3.Item))
					{
						node = node2;
					}
					else
					{
						node = node3;
					}
				}
				return true;
			}

			/// <summary>Releases all resources used by the <see cref="T:System.Collections.Generic.SortedSet`1.Enumerator" />.</summary>
			public void Dispose()
			{
			}

			/// <summary>Gets the element at the current position of the enumerator.</summary>
			/// <returns>The element in the collection at the current position of the enumerator.</returns>
			public T Current
			{
				get
				{
					if (this._current != null)
					{
						return this._current.Item;
					}
					return default(T);
				}
			}

			/// <summary>Gets the element at the current position of the enumerator.</summary>
			/// <returns>The element in the collection at the current position of the enumerator.</returns>
			/// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element.</exception>
			object IEnumerator.Current
			{
				get
				{
					if (this._current == null)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					return this._current.Item;
				}
			}

			internal bool NotStartedOrEnded
			{
				get
				{
					return this._current == null;
				}
			}

			internal void Reset()
			{
				if (this._version != this._tree.version)
				{
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
				}
				this._stack.Clear();
				this.Initialize();
			}

			/// <summary>Sets the enumerator to its initial position, which is before the first element in the collection.</summary>
			/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created.</exception>
			void IEnumerator.Reset()
			{
				this.Reset();
			}

			private static readonly SortedSet<T>.Node s_dummyNode = new SortedSet<T>.Node(default(T), NodeColor.Red);

			private SortedSet<T> _tree;

			private int _version;

			private Stack<SortedSet<T>.Node> _stack;

			private SortedSet<T>.Node _current;

			private bool _reverse;
		}

		internal struct ElementCount
		{
			internal int UniqueCount;

			internal int UnfoundCount;
		}
	}
}
