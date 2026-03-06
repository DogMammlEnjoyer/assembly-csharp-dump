using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Hosting
{
	/// <summary>Represents a single composition operation for transactional composition.</summary>
	public class AtomicComposition : IDisposable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.AtomicComposition" /> class.</summary>
		public AtomicComposition() : this(null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Hosting.AtomicComposition" /> class with the specified parent <see cref="T:System.ComponentModel.Composition.Hosting.AtomicComposition" />.</summary>
		/// <param name="outerAtomicComposition">The parent of this composition operation.</param>
		public AtomicComposition(AtomicComposition outerAtomicComposition)
		{
			if (outerAtomicComposition != null)
			{
				this._outerAtomicComposition = outerAtomicComposition;
				this._outerAtomicComposition.ContainsInnerAtomicComposition = true;
			}
		}

		/// <summary>Saves a key-value pair in the transaction to track tentative state.</summary>
		/// <param name="key">The key to save.</param>
		/// <param name="value">The value to save.</param>
		public void SetValue(object key, object value)
		{
			this.ThrowIfDisposed();
			this.ThrowIfCompleted();
			this.ThrowIfContainsInnerAtomicComposition();
			Requires.NotNull<object>(key, "key");
			this.SetValueInternal(key, value);
		}

		/// <summary>Gets a value saved by the <see cref="M:System.ComponentModel.Composition.Hosting.AtomicComposition.SetValue(System.Object,System.Object)" /> method.</summary>
		/// <param name="key">The key to retrieve from.</param>
		/// <param name="value">The retrieved value.</param>
		/// <typeparam name="T">The type of the value to be retrieved.</typeparam>
		/// <returns>
		///   <see langword="true" /> if the value was successfully retrieved; otherwise, <see langword="false" />.</returns>
		public bool TryGetValue<T>(object key, out T value)
		{
			return this.TryGetValue<T>(key, false, out value);
		}

		/// <summary>Gets a value saved by the <see cref="M:System.ComponentModel.Composition.Hosting.AtomicComposition.SetValue(System.Object,System.Object)" /> method, with the option of not searching parent transactions.</summary>
		/// <param name="key">The key to retrieve from.</param>
		/// <param name="localAtomicCompositionOnly">
		///   <see langword="true" /> to exclude parent transactions; otherwise, <see langword="false" />.</param>
		/// <param name="value">The retrieved value.</param>
		/// <typeparam name="T">The type of the value to be retrieved.</typeparam>
		/// <returns>
		///   <see langword="true" /> if the value was successfully retrieved; otherwise, <see langword="false" />.</returns>
		public bool TryGetValue<T>(object key, bool localAtomicCompositionOnly, out T value)
		{
			this.ThrowIfDisposed();
			this.ThrowIfCompleted();
			Requires.NotNull<object>(key, "key");
			return this.TryGetValueInternal<T>(key, localAtomicCompositionOnly, out value);
		}

		/// <summary>Adds an action to be executed when the overall composition operation completes successfully.</summary>
		/// <param name="completeAction">The action to be executed.</param>
		public void AddCompleteAction(Action completeAction)
		{
			this.ThrowIfDisposed();
			this.ThrowIfCompleted();
			this.ThrowIfContainsInnerAtomicComposition();
			Requires.NotNull<Action>(completeAction, "completeAction");
			if (this._completeActionList == null)
			{
				this._completeActionList = new List<Action>();
			}
			this._completeActionList.Add(completeAction);
		}

		/// <summary>Adds an action to be executed if the overall composition operation fails.</summary>
		/// <param name="revertAction">The action to be executed.</param>
		public void AddRevertAction(Action revertAction)
		{
			this.ThrowIfDisposed();
			this.ThrowIfCompleted();
			this.ThrowIfContainsInnerAtomicComposition();
			Requires.NotNull<Action>(revertAction, "revertAction");
			if (this._revertActionList == null)
			{
				this._revertActionList = new List<Action>();
			}
			this._revertActionList.Add(revertAction);
		}

		/// <summary>Marks this composition operation as complete.</summary>
		public void Complete()
		{
			this.ThrowIfDisposed();
			this.ThrowIfCompleted();
			if (this._outerAtomicComposition == null)
			{
				this.FinalComplete();
			}
			else
			{
				this.CopyComplete();
			}
			this._isCompleted = true;
		}

		/// <summary>Releases all resources used by the current instance of the <see cref="T:System.ComponentModel.Composition.Hosting.AtomicComposition" /> class, and mark this composition operation as failed.</summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.Composition.Hosting.AtomicComposition" /> and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			this.ThrowIfDisposed();
			this._isDisposed = true;
			if (this._outerAtomicComposition != null)
			{
				this._outerAtomicComposition.ContainsInnerAtomicComposition = false;
			}
			if (!this._isCompleted && this._revertActionList != null)
			{
				for (int i = this._revertActionList.Count - 1; i >= 0; i--)
				{
					this._revertActionList[i]();
				}
				this._revertActionList = null;
			}
		}

		private void FinalComplete()
		{
			if (this._completeActionList != null)
			{
				foreach (Action action in this._completeActionList)
				{
					action();
				}
				this._completeActionList = null;
			}
		}

		private void CopyComplete()
		{
			Assumes.NotNull<AtomicComposition>(this._outerAtomicComposition);
			this._outerAtomicComposition.ContainsInnerAtomicComposition = false;
			if (this._completeActionList != null)
			{
				foreach (Action completeAction in this._completeActionList)
				{
					this._outerAtomicComposition.AddCompleteAction(completeAction);
				}
			}
			if (this._revertActionList != null)
			{
				foreach (Action revertAction in this._revertActionList)
				{
					this._outerAtomicComposition.AddRevertAction(revertAction);
				}
			}
			for (int i = 0; i < this._valueCount; i++)
			{
				this._outerAtomicComposition.SetValueInternal(this._values[i].Key, this._values[i].Value);
			}
		}

		private bool ContainsInnerAtomicComposition
		{
			set
			{
				if (value && this._containsInnerAtomicComposition)
				{
					throw new InvalidOperationException(Strings.AtomicComposition_AlreadyNested);
				}
				this._containsInnerAtomicComposition = value;
			}
		}

		private bool TryGetValueInternal<T>(object key, bool localAtomicCompositionOnly, out T value)
		{
			for (int i = 0; i < this._valueCount; i++)
			{
				if (this._values[i].Key == key)
				{
					value = (T)((object)this._values[i].Value);
					return true;
				}
			}
			if (!localAtomicCompositionOnly && this._outerAtomicComposition != null)
			{
				return this._outerAtomicComposition.TryGetValueInternal<T>(key, localAtomicCompositionOnly, out value);
			}
			value = default(T);
			return false;
		}

		private void SetValueInternal(object key, object value)
		{
			for (int i = 0; i < this._valueCount; i++)
			{
				if (this._values[i].Key == key)
				{
					this._values[i] = new KeyValuePair<object, object>(key, value);
					return;
				}
			}
			if (this._values == null || this._valueCount == this._values.Length)
			{
				KeyValuePair<object, object>[] array = new KeyValuePair<object, object>[(this._valueCount == 0) ? 5 : (this._valueCount * 2)];
				if (this._values != null)
				{
					Array.Copy(this._values, array, this._valueCount);
				}
				this._values = array;
			}
			this._values[this._valueCount] = new KeyValuePair<object, object>(key, value);
			this._valueCount++;
		}

		[DebuggerStepThrough]
		private void ThrowIfContainsInnerAtomicComposition()
		{
			if (this._containsInnerAtomicComposition)
			{
				throw new InvalidOperationException(Strings.AtomicComposition_PartOfAnotherAtomicComposition);
			}
		}

		[DebuggerStepThrough]
		private void ThrowIfCompleted()
		{
			if (this._isCompleted)
			{
				throw new InvalidOperationException(Strings.AtomicComposition_AlreadyCompleted);
			}
		}

		[DebuggerStepThrough]
		private void ThrowIfDisposed()
		{
			if (this._isDisposed)
			{
				throw ExceptionBuilder.CreateObjectDisposed(this);
			}
		}

		private readonly AtomicComposition _outerAtomicComposition;

		private KeyValuePair<object, object>[] _values;

		private int _valueCount;

		private List<Action> _completeActionList;

		private List<Action> _revertActionList;

		private bool _isDisposed;

		private bool _isCompleted;

		private bool _containsInnerAtomicComposition;
	}
}
