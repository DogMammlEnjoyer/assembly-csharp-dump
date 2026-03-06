using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;

namespace System.Collections.Generic
{
	/// <summary>Represents a doubly linked list.</summary>
	/// <typeparam name="T">Specifies the element type of the linked list.</typeparam>
	[DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
	[Serializable]
	public class LinkedList<T> : ICollection<T>, IEnumerable<T>, IEnumerable, ICollection, IReadOnlyCollection<T>, ISerializable, IDeserializationCallback
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.LinkedList`1" /> class that is empty.</summary>
		public LinkedList()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.LinkedList`1" /> class that contains elements copied from the specified <see cref="T:System.Collections.IEnumerable" /> and has sufficient capacity to accommodate the number of elements copied.</summary>
		/// <param name="collection">The <see cref="T:System.Collections.IEnumerable" /> whose elements are copied to the new <see cref="T:System.Collections.Generic.LinkedList`1" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="collection" /> is <see langword="null" />.</exception>
		public LinkedList(IEnumerable<T> collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			foreach (T value in collection)
			{
				this.AddLast(value);
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.LinkedList`1" /> class that is serializable with the specified <see cref="T:System.Runtime.Serialization.SerializationInfo" /> and <see cref="T:System.Runtime.Serialization.StreamingContext" />.</summary>
		/// <param name="info">A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object containing the information required to serialize the <see cref="T:System.Collections.Generic.LinkedList`1" />.</param>
		/// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> object containing the source and destination of the serialized stream associated with the <see cref="T:System.Collections.Generic.LinkedList`1" />.</param>
		protected LinkedList(SerializationInfo info, StreamingContext context)
		{
			this._siInfo = info;
		}

		/// <summary>Gets the number of nodes actually contained in the <see cref="T:System.Collections.Generic.LinkedList`1" />.</summary>
		/// <returns>The number of nodes actually contained in the <see cref="T:System.Collections.Generic.LinkedList`1" />.</returns>
		public int Count
		{
			get
			{
				return this.count;
			}
		}

		/// <summary>Gets the first node of the <see cref="T:System.Collections.Generic.LinkedList`1" />.</summary>
		/// <returns>The first <see cref="T:System.Collections.Generic.LinkedListNode`1" /> of the <see cref="T:System.Collections.Generic.LinkedList`1" />.</returns>
		public LinkedListNode<T> First
		{
			get
			{
				return this.head;
			}
		}

		/// <summary>Gets the last node of the <see cref="T:System.Collections.Generic.LinkedList`1" />.</summary>
		/// <returns>The last <see cref="T:System.Collections.Generic.LinkedListNode`1" /> of the <see cref="T:System.Collections.Generic.LinkedList`1" />.</returns>
		public LinkedListNode<T> Last
		{
			get
			{
				if (this.head != null)
				{
					return this.head.prev;
				}
				return null;
			}
		}

		bool ICollection<!0>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		void ICollection<!0>.Add(T value)
		{
			this.AddLast(value);
		}

		/// <summary>Adds a new node containing the specified value after the specified existing node in the <see cref="T:System.Collections.Generic.LinkedList`1" />.</summary>
		/// <param name="node">The <see cref="T:System.Collections.Generic.LinkedListNode`1" /> after which to insert a new <see cref="T:System.Collections.Generic.LinkedListNode`1" /> containing <paramref name="value" />.</param>
		/// <param name="value">The value to add to the <see cref="T:System.Collections.Generic.LinkedList`1" />.</param>
		/// <returns>The new <see cref="T:System.Collections.Generic.LinkedListNode`1" /> containing <paramref name="value" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="node" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="node" /> is not in the current <see cref="T:System.Collections.Generic.LinkedList`1" />.</exception>
		public LinkedListNode<T> AddAfter(LinkedListNode<T> node, T value)
		{
			this.ValidateNode(node);
			LinkedListNode<T> linkedListNode = new LinkedListNode<T>(node.list, value);
			this.InternalInsertNodeBefore(node.next, linkedListNode);
			return linkedListNode;
		}

		/// <summary>Adds the specified new node after the specified existing node in the <see cref="T:System.Collections.Generic.LinkedList`1" />.</summary>
		/// <param name="node">The <see cref="T:System.Collections.Generic.LinkedListNode`1" /> after which to insert <paramref name="newNode" />.</param>
		/// <param name="newNode">The new <see cref="T:System.Collections.Generic.LinkedListNode`1" /> to add to the <see cref="T:System.Collections.Generic.LinkedList`1" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="node" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="newNode" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="node" /> is not in the current <see cref="T:System.Collections.Generic.LinkedList`1" />.  
		/// -or-  
		/// <paramref name="newNode" /> belongs to another <see cref="T:System.Collections.Generic.LinkedList`1" />.</exception>
		public void AddAfter(LinkedListNode<T> node, LinkedListNode<T> newNode)
		{
			this.ValidateNode(node);
			this.ValidateNewNode(newNode);
			this.InternalInsertNodeBefore(node.next, newNode);
			newNode.list = this;
		}

		/// <summary>Adds a new node containing the specified value before the specified existing node in the <see cref="T:System.Collections.Generic.LinkedList`1" />.</summary>
		/// <param name="node">The <see cref="T:System.Collections.Generic.LinkedListNode`1" /> before which to insert a new <see cref="T:System.Collections.Generic.LinkedListNode`1" /> containing <paramref name="value" />.</param>
		/// <param name="value">The value to add to the <see cref="T:System.Collections.Generic.LinkedList`1" />.</param>
		/// <returns>The new <see cref="T:System.Collections.Generic.LinkedListNode`1" /> containing <paramref name="value" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="node" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="node" /> is not in the current <see cref="T:System.Collections.Generic.LinkedList`1" />.</exception>
		public LinkedListNode<T> AddBefore(LinkedListNode<T> node, T value)
		{
			this.ValidateNode(node);
			LinkedListNode<T> linkedListNode = new LinkedListNode<T>(node.list, value);
			this.InternalInsertNodeBefore(node, linkedListNode);
			if (node == this.head)
			{
				this.head = linkedListNode;
			}
			return linkedListNode;
		}

		/// <summary>Adds the specified new node before the specified existing node in the <see cref="T:System.Collections.Generic.LinkedList`1" />.</summary>
		/// <param name="node">The <see cref="T:System.Collections.Generic.LinkedListNode`1" /> before which to insert <paramref name="newNode" />.</param>
		/// <param name="newNode">The new <see cref="T:System.Collections.Generic.LinkedListNode`1" /> to add to the <see cref="T:System.Collections.Generic.LinkedList`1" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="node" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="newNode" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="node" /> is not in the current <see cref="T:System.Collections.Generic.LinkedList`1" />.  
		/// -or-  
		/// <paramref name="newNode" /> belongs to another <see cref="T:System.Collections.Generic.LinkedList`1" />.</exception>
		public void AddBefore(LinkedListNode<T> node, LinkedListNode<T> newNode)
		{
			this.ValidateNode(node);
			this.ValidateNewNode(newNode);
			this.InternalInsertNodeBefore(node, newNode);
			newNode.list = this;
			if (node == this.head)
			{
				this.head = newNode;
			}
		}

		/// <summary>Adds a new node containing the specified value at the start of the <see cref="T:System.Collections.Generic.LinkedList`1" />.</summary>
		/// <param name="value">The value to add at the start of the <see cref="T:System.Collections.Generic.LinkedList`1" />.</param>
		/// <returns>The new <see cref="T:System.Collections.Generic.LinkedListNode`1" /> containing <paramref name="value" />.</returns>
		public LinkedListNode<T> AddFirst(T value)
		{
			LinkedListNode<T> linkedListNode = new LinkedListNode<T>(this, value);
			if (this.head == null)
			{
				this.InternalInsertNodeToEmptyList(linkedListNode);
			}
			else
			{
				this.InternalInsertNodeBefore(this.head, linkedListNode);
				this.head = linkedListNode;
			}
			return linkedListNode;
		}

		/// <summary>Adds the specified new node at the start of the <see cref="T:System.Collections.Generic.LinkedList`1" />.</summary>
		/// <param name="node">The new <see cref="T:System.Collections.Generic.LinkedListNode`1" /> to add at the start of the <see cref="T:System.Collections.Generic.LinkedList`1" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="node" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="node" /> belongs to another <see cref="T:System.Collections.Generic.LinkedList`1" />.</exception>
		public void AddFirst(LinkedListNode<T> node)
		{
			this.ValidateNewNode(node);
			if (this.head == null)
			{
				this.InternalInsertNodeToEmptyList(node);
			}
			else
			{
				this.InternalInsertNodeBefore(this.head, node);
				this.head = node;
			}
			node.list = this;
		}

		/// <summary>Adds a new node containing the specified value at the end of the <see cref="T:System.Collections.Generic.LinkedList`1" />.</summary>
		/// <param name="value">The value to add at the end of the <see cref="T:System.Collections.Generic.LinkedList`1" />.</param>
		/// <returns>The new <see cref="T:System.Collections.Generic.LinkedListNode`1" /> containing <paramref name="value" />.</returns>
		public LinkedListNode<T> AddLast(T value)
		{
			LinkedListNode<T> linkedListNode = new LinkedListNode<T>(this, value);
			if (this.head == null)
			{
				this.InternalInsertNodeToEmptyList(linkedListNode);
			}
			else
			{
				this.InternalInsertNodeBefore(this.head, linkedListNode);
			}
			return linkedListNode;
		}

		/// <summary>Adds the specified new node at the end of the <see cref="T:System.Collections.Generic.LinkedList`1" />.</summary>
		/// <param name="node">The new <see cref="T:System.Collections.Generic.LinkedListNode`1" /> to add at the end of the <see cref="T:System.Collections.Generic.LinkedList`1" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="node" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="node" /> belongs to another <see cref="T:System.Collections.Generic.LinkedList`1" />.</exception>
		public void AddLast(LinkedListNode<T> node)
		{
			this.ValidateNewNode(node);
			if (this.head == null)
			{
				this.InternalInsertNodeToEmptyList(node);
			}
			else
			{
				this.InternalInsertNodeBefore(this.head, node);
			}
			node.list = this;
		}

		/// <summary>Removes all nodes from the <see cref="T:System.Collections.Generic.LinkedList`1" />.</summary>
		public void Clear()
		{
			LinkedListNode<T> next = this.head;
			while (next != null)
			{
				LinkedListNode<T> linkedListNode = next;
				next = next.Next;
				linkedListNode.Invalidate();
			}
			this.head = null;
			this.count = 0;
			this.version++;
		}

		/// <summary>Determines whether a value is in the <see cref="T:System.Collections.Generic.LinkedList`1" />.</summary>
		/// <param name="value">The value to locate in the <see cref="T:System.Collections.Generic.LinkedList`1" />. The value can be <see langword="null" /> for reference types.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="value" /> is found in the <see cref="T:System.Collections.Generic.LinkedList`1" />; otherwise, <see langword="false" />.</returns>
		public bool Contains(T value)
		{
			return this.Find(value) != null;
		}

		/// <summary>Copies the entire <see cref="T:System.Collections.Generic.LinkedList`1" /> to a compatible one-dimensional <see cref="T:System.Array" />, starting at the specified index of the target array.</summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.LinkedList`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> is less than zero.</exception>
		/// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.LinkedList`1" /> is greater than the available space from <paramref name="index" /> to the end of the destination <paramref name="array" />.</exception>
		public void CopyTo(T[] array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", index, "Non-negative number required.");
			}
			if (index > array.Length)
			{
				throw new ArgumentOutOfRangeException("index", index, "Must be less than or equal to the size of the collection.");
			}
			if (array.Length - index < this.Count)
			{
				throw new ArgumentException("Insufficient space in the target location to copy the information.");
			}
			LinkedListNode<T> next = this.head;
			if (next != null)
			{
				do
				{
					array[index++] = next.item;
					next = next.next;
				}
				while (next != this.head);
			}
		}

		/// <summary>Finds the first node that contains the specified value.</summary>
		/// <param name="value">The value to locate in the <see cref="T:System.Collections.Generic.LinkedList`1" />.</param>
		/// <returns>The first <see cref="T:System.Collections.Generic.LinkedListNode`1" /> that contains the specified value, if found; otherwise, <see langword="null" />.</returns>
		public LinkedListNode<T> Find(T value)
		{
			LinkedListNode<T> next = this.head;
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			if (next != null)
			{
				if (value != null)
				{
					while (!@default.Equals(next.item, value))
					{
						next = next.next;
						if (next == this.head)
						{
							goto IL_5A;
						}
					}
					return next;
				}
				while (next.item != null)
				{
					next = next.next;
					if (next == this.head)
					{
						goto IL_5A;
					}
				}
				return next;
			}
			IL_5A:
			return null;
		}

		/// <summary>Finds the last node that contains the specified value.</summary>
		/// <param name="value">The value to locate in the <see cref="T:System.Collections.Generic.LinkedList`1" />.</param>
		/// <returns>The last <see cref="T:System.Collections.Generic.LinkedListNode`1" /> that contains the specified value, if found; otherwise, <see langword="null" />.</returns>
		public LinkedListNode<T> FindLast(T value)
		{
			if (this.head == null)
			{
				return null;
			}
			LinkedListNode<T> prev = this.head.prev;
			LinkedListNode<T> linkedListNode = prev;
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			if (linkedListNode != null)
			{
				if (value != null)
				{
					while (!@default.Equals(linkedListNode.item, value))
					{
						linkedListNode = linkedListNode.prev;
						if (linkedListNode == prev)
						{
							goto IL_61;
						}
					}
					return linkedListNode;
				}
				while (linkedListNode.item != null)
				{
					linkedListNode = linkedListNode.prev;
					if (linkedListNode == prev)
					{
						goto IL_61;
					}
				}
				return linkedListNode;
			}
			IL_61:
			return null;
		}

		/// <summary>Returns an enumerator that iterates through the <see cref="T:System.Collections.Generic.LinkedList`1" />.</summary>
		/// <returns>An <see cref="T:System.Collections.Generic.LinkedList`1.Enumerator" /> for the <see cref="T:System.Collections.Generic.LinkedList`1" />.</returns>
		public LinkedList<T>.Enumerator GetEnumerator()
		{
			return new LinkedList<T>.Enumerator(this);
		}

		IEnumerator<T> IEnumerable<!0>.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		/// <summary>Removes the first occurrence of the specified value from the <see cref="T:System.Collections.Generic.LinkedList`1" />.</summary>
		/// <param name="value">The value to remove from the <see cref="T:System.Collections.Generic.LinkedList`1" />.</param>
		/// <returns>
		///   <see langword="true" /> if the element containing <paramref name="value" /> is successfully removed; otherwise, <see langword="false" />.  This method also returns <see langword="false" /> if <paramref name="value" /> was not found in the original <see cref="T:System.Collections.Generic.LinkedList`1" />.</returns>
		public bool Remove(T value)
		{
			LinkedListNode<T> linkedListNode = this.Find(value);
			if (linkedListNode != null)
			{
				this.InternalRemoveNode(linkedListNode);
				return true;
			}
			return false;
		}

		/// <summary>Removes the specified node from the <see cref="T:System.Collections.Generic.LinkedList`1" />.</summary>
		/// <param name="node">The <see cref="T:System.Collections.Generic.LinkedListNode`1" /> to remove from the <see cref="T:System.Collections.Generic.LinkedList`1" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="node" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="node" /> is not in the current <see cref="T:System.Collections.Generic.LinkedList`1" />.</exception>
		public void Remove(LinkedListNode<T> node)
		{
			this.ValidateNode(node);
			this.InternalRemoveNode(node);
		}

		/// <summary>Removes the node at the start of the <see cref="T:System.Collections.Generic.LinkedList`1" />.</summary>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.Generic.LinkedList`1" /> is empty.</exception>
		public void RemoveFirst()
		{
			if (this.head == null)
			{
				throw new InvalidOperationException("The LinkedList is empty.");
			}
			this.InternalRemoveNode(this.head);
		}

		/// <summary>Removes the node at the end of the <see cref="T:System.Collections.Generic.LinkedList`1" />.</summary>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Collections.Generic.LinkedList`1" /> is empty.</exception>
		public void RemoveLast()
		{
			if (this.head == null)
			{
				throw new InvalidOperationException("The LinkedList is empty.");
			}
			this.InternalRemoveNode(this.head.prev);
		}

		/// <summary>Implements the <see cref="T:System.Runtime.Serialization.ISerializable" /> interface and returns the data needed to serialize the <see cref="T:System.Collections.Generic.LinkedList`1" /> instance.</summary>
		/// <param name="info">A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object that contains the information required to serialize the <see cref="T:System.Collections.Generic.LinkedList`1" /> instance.</param>
		/// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> object that contains the source and destination of the serialized stream associated with the <see cref="T:System.Collections.Generic.LinkedList`1" /> instance.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="info" /> is <see langword="null" />.</exception>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("Version", this.version);
			info.AddValue("Count", this.count);
			if (this.count != 0)
			{
				T[] array = new T[this.count];
				this.CopyTo(array, 0);
				info.AddValue("Data", array, typeof(T[]));
			}
		}

		/// <summary>Implements the <see cref="T:System.Runtime.Serialization.ISerializable" /> interface and raises the deserialization event when the deserialization is complete.</summary>
		/// <param name="sender">The source of the deserialization event.</param>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object associated with the current <see cref="T:System.Collections.Generic.LinkedList`1" /> instance is invalid.</exception>
		public virtual void OnDeserialization(object sender)
		{
			if (this._siInfo == null)
			{
				return;
			}
			int @int = this._siInfo.GetInt32("Version");
			if (this._siInfo.GetInt32("Count") != 0)
			{
				T[] array = (T[])this._siInfo.GetValue("Data", typeof(T[]));
				if (array == null)
				{
					throw new SerializationException("The values for this dictionary are missing.");
				}
				for (int i = 0; i < array.Length; i++)
				{
					this.AddLast(array[i]);
				}
			}
			else
			{
				this.head = null;
			}
			this.version = @int;
			this._siInfo = null;
		}

		private void InternalInsertNodeBefore(LinkedListNode<T> node, LinkedListNode<T> newNode)
		{
			newNode.next = node;
			newNode.prev = node.prev;
			node.prev.next = newNode;
			node.prev = newNode;
			this.version++;
			this.count++;
		}

		private void InternalInsertNodeToEmptyList(LinkedListNode<T> newNode)
		{
			newNode.next = newNode;
			newNode.prev = newNode;
			this.head = newNode;
			this.version++;
			this.count++;
		}

		internal void InternalRemoveNode(LinkedListNode<T> node)
		{
			if (node.next == node)
			{
				this.head = null;
			}
			else
			{
				node.next.prev = node.prev;
				node.prev.next = node.next;
				if (this.head == node)
				{
					this.head = node.next;
				}
			}
			node.Invalidate();
			this.count--;
			this.version++;
		}

		internal void ValidateNewNode(LinkedListNode<T> node)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			if (node.list != null)
			{
				throw new InvalidOperationException("The LinkedList node already belongs to a LinkedList.");
			}
		}

		internal void ValidateNode(LinkedListNode<T> node)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			if (node.list != this)
			{
				throw new InvalidOperationException("The LinkedList node does not belong to current LinkedList.");
			}
		}

		/// <summary>Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).</summary>
		/// <returns>
		///   <see langword="true" /> if access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe); otherwise, <see langword="false" />.  In the default implementation of <see cref="T:System.Collections.Generic.LinkedList`1" />, this property always returns <see langword="false" />.</returns>
		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.</summary>
		/// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.  In the default implementation of <see cref="T:System.Collections.Generic.LinkedList`1" />, this property always returns the current instance.</returns>
		object ICollection.SyncRoot
		{
			get
			{
				if (this._syncRoot == null)
				{
					Interlocked.CompareExchange<object>(ref this._syncRoot, new object(), null);
				}
				return this._syncRoot;
			}
		}

		/// <summary>Copies the elements of the <see cref="T:System.Collections.ICollection" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.</summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="index" /> is less than zero.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="array" /> is multidimensional.  
		/// -or-  
		/// <paramref name="array" /> does not have zero-based indexing.  
		/// -or-  
		/// The number of elements in the source <see cref="T:System.Collections.ICollection" /> is greater than the available space from <paramref name="index" /> to the end of the destination <paramref name="array" />.  
		/// -or-  
		/// The type of the source <see cref="T:System.Collections.ICollection" /> cannot be cast automatically to the type of the destination <paramref name="array" />.</exception>
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
				throw new ArgumentException("Insufficient space in the target location to copy the information.");
			}
			T[] array2 = array as T[];
			if (array2 != null)
			{
				this.CopyTo(array2, index);
				return;
			}
			object[] array3 = array as object[];
			if (array3 == null)
			{
				throw new ArgumentException("Target array type is not compatible with the type of items in the collection.", "array");
			}
			LinkedListNode<T> next = this.head;
			try
			{
				if (next != null)
				{
					do
					{
						array3[index++] = next.item;
						next = next.next;
					}
					while (next != this.head);
				}
			}
			catch (ArrayTypeMismatchException)
			{
				throw new ArgumentException("Target array type is not compatible with the type of items in the collection.", "array");
			}
		}

		/// <summary>Returns an enumerator that iterates through the linked list as a collection.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate through the linked list as a collection.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		internal LinkedListNode<T> head;

		internal int count;

		internal int version;

		private object _syncRoot;

		private SerializationInfo _siInfo;

		private const string VersionName = "Version";

		private const string CountName = "Count";

		private const string ValuesName = "Data";

		/// <summary>Enumerates the elements of a <see cref="T:System.Collections.Generic.LinkedList`1" />.</summary>
		/// <typeparam name="T" />
		[Serializable]
		public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator, ISerializable, IDeserializationCallback
		{
			internal Enumerator(LinkedList<T> list)
			{
				this._list = list;
				this._version = list.version;
				this._node = list.head;
				this._current = default(T);
				this._index = 0;
			}

			private Enumerator(SerializationInfo info, StreamingContext context)
			{
				throw new PlatformNotSupportedException();
			}

			/// <summary>Gets the element at the current position of the enumerator.</summary>
			/// <returns>The element in the <see cref="T:System.Collections.Generic.LinkedList`1" /> at the current position of the enumerator.</returns>
			public T Current
			{
				get
				{
					return this._current;
				}
			}

			/// <summary>Gets the element at the current position of the enumerator.</summary>
			/// <returns>The element in the collection at the current position of the enumerator.</returns>
			/// <exception cref="T:System.InvalidOperationException">The enumerator is positioned before the first element of the collection or after the last element.</exception>
			object IEnumerator.Current
			{
				get
				{
					if (this._index == 0 || this._index == this._list.Count + 1)
					{
						throw new InvalidOperationException("Enumeration has either not started or has already finished.");
					}
					return this._current;
				}
			}

			/// <summary>Advances the enumerator to the next element of the <see cref="T:System.Collections.Generic.LinkedList`1" />.</summary>
			/// <returns>
			///   <see langword="true" /> if the enumerator was successfully advanced to the next element; <see langword="false" /> if the enumerator has passed the end of the collection.</returns>
			/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created.</exception>
			public bool MoveNext()
			{
				if (this._version != this._list.version)
				{
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
				}
				if (this._node == null)
				{
					this._index = this._list.Count + 1;
					return false;
				}
				this._index++;
				this._current = this._node.item;
				this._node = this._node.next;
				if (this._node == this._list.head)
				{
					this._node = null;
				}
				return true;
			}

			/// <summary>Sets the enumerator to its initial position, which is before the first element in the collection. This class cannot be inherited.</summary>
			/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created.</exception>
			void IEnumerator.Reset()
			{
				if (this._version != this._list.version)
				{
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
				}
				this._current = default(T);
				this._node = this._list.head;
				this._index = 0;
			}

			/// <summary>Releases all resources used by the <see cref="T:System.Collections.Generic.LinkedList`1.Enumerator" />.</summary>
			public void Dispose()
			{
			}

			/// <summary>Implements the <see cref="T:System.Runtime.Serialization.ISerializable" /> interface and returns the data needed to serialize the <see cref="T:System.Collections.Generic.LinkedList`1" /> instance.</summary>
			/// <param name="info">A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object that contains the information required to serialize the <see cref="T:System.Collections.Generic.LinkedList`1" /> instance.</param>
			/// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> object that contains the source and destination of the serialized stream associated with the <see cref="T:System.Collections.Generic.LinkedList`1" /> instance.</param>
			/// <exception cref="T:System.ArgumentNullException">
			///   <paramref name="info" /> is <see langword="null" />.</exception>
			void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
			{
				throw new PlatformNotSupportedException();
			}

			/// <summary>Implements the <see cref="T:System.Runtime.Serialization.ISerializable" /> interface and raises the deserialization event when the deserialization is complete.</summary>
			/// <param name="sender">The source of the deserialization event.</param>
			/// <exception cref="T:System.Runtime.Serialization.SerializationException">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object associated with the current <see cref="T:System.Collections.Generic.LinkedList`1" /> instance is invalid.</exception>
			void IDeserializationCallback.OnDeserialization(object sender)
			{
				throw new PlatformNotSupportedException();
			}

			private LinkedList<T> _list;

			private LinkedListNode<T> _node;

			private int _version;

			private T _current;

			private int _index;

			private const string LinkedListName = "LinkedList";

			private const string CurrentValueName = "Current";

			private const string VersionName = "Version";

			private const string IndexName = "Index";
		}
	}
}
