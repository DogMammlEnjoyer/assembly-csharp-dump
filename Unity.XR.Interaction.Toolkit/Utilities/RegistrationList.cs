using System;
using System.Collections.Generic;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
	internal sealed class RegistrationList<T> : BaseRegistrationList<T>
	{
		public override bool IsRegistered(T item)
		{
			return this.m_UnorderedRegisteredItems.Contains(item);
		}

		public override bool IsStillRegistered(T item)
		{
			return this.m_BufferedRemoveEmpty || !this.m_UnorderedBufferedRemove.Contains(item);
		}

		public override bool Register(T item)
		{
			if (this.m_UnorderedBufferedAdd.Count > 0 && this.m_UnorderedBufferedAdd.Contains(item))
			{
				return false;
			}
			bool flag = this.m_UnorderedRegisteredSnapshot.Contains(item);
			if ((!this.m_BufferedRemoveEmpty && this.m_UnorderedBufferedRemove.Remove(item)) || !flag)
			{
				base.RemoveFromBufferedRemove(item);
				this.m_BufferedRemoveEmpty = (this.m_UnorderedBufferedRemove.Count == 0);
				this.m_UnorderedRegisteredItems.Add(item);
				if (!flag)
				{
					base.AddToBufferedAdd(item);
					this.m_UnorderedBufferedAdd.Add(item);
				}
				return true;
			}
			return false;
		}

		public override bool Unregister(T item)
		{
			if (!this.m_BufferedRemoveEmpty && this.m_UnorderedBufferedRemove.Contains(item))
			{
				return false;
			}
			if (this.m_UnorderedBufferedAdd.Count > 0 && this.m_UnorderedBufferedAdd.Remove(item))
			{
				base.RemoveFromBufferedAdd(item);
				this.m_UnorderedRegisteredItems.Remove(item);
				return true;
			}
			if (this.m_UnorderedRegisteredSnapshot.Contains(item))
			{
				base.AddToBufferedRemove(item);
				this.m_UnorderedBufferedRemove.Add(item);
				this.m_BufferedRemoveEmpty = false;
				this.m_UnorderedRegisteredItems.Remove(item);
				return true;
			}
			return false;
		}

		public override void Flush()
		{
			if (!this.m_BufferedRemoveEmpty)
			{
				foreach (T item in this.m_BufferedRemove)
				{
					base.registeredSnapshot.Remove(item);
					this.m_UnorderedRegisteredSnapshot.Remove(item);
				}
				base.ClearBufferedRemove();
				this.m_UnorderedBufferedRemove.Clear();
				this.m_BufferedRemoveEmpty = true;
			}
			if (base.bufferedAddCount > 0)
			{
				foreach (T item2 in this.m_BufferedAdd)
				{
					if (!this.m_UnorderedRegisteredSnapshot.Contains(item2))
					{
						base.registeredSnapshot.Add(item2);
						this.m_UnorderedRegisteredSnapshot.Add(item2);
					}
				}
				base.ClearBufferedAdd();
				this.m_UnorderedBufferedAdd.Clear();
			}
		}

		public override void GetRegisteredItems(List<T> results)
		{
			if (results == null)
			{
				throw new ArgumentNullException("results");
			}
			results.Clear();
			BaseRegistrationList<T>.EnsureCapacity(results, base.flushedCount);
			foreach (T item in base.registeredSnapshot)
			{
				if (this.m_BufferedRemoveEmpty || !this.m_UnorderedBufferedRemove.Contains(item))
				{
					results.Add(item);
				}
			}
			if (base.bufferedAddCount > 0)
			{
				results.AddRange(this.m_BufferedAdd);
			}
		}

		public override T GetRegisteredItemAt(int index)
		{
			if (index < 0 || index >= base.flushedCount)
			{
				throw new ArgumentOutOfRangeException("index", "Index was out of range. Must be non-negative and less than the size of the registration collection.");
			}
			if (base.bufferedRemoveCount == 0 && base.bufferedAddCount == 0)
			{
				return base.registeredSnapshot[index];
			}
			if (index >= base.registeredSnapshot.Count - base.bufferedRemoveCount)
			{
				return this.m_BufferedAdd[index - (base.registeredSnapshot.Count - base.bufferedRemoveCount)];
			}
			int num = 0;
			foreach (T item in base.registeredSnapshot)
			{
				if (!this.m_UnorderedBufferedRemove.Contains(item))
				{
					if (num == index)
					{
						return base.registeredSnapshot[index];
					}
					num++;
				}
			}
			throw new ArgumentOutOfRangeException("index", "Index was out of range. Must be non-negative and less than the size of the registration collection.");
		}

		protected override void OnItemMovedImmediately(T item, int newIndex)
		{
			base.OnItemMovedImmediately(item, newIndex);
			this.m_UnorderedRegisteredItems.Add(item);
			this.m_UnorderedRegisteredSnapshot.Add(item);
		}

		private readonly HashSet<T> m_UnorderedBufferedAdd = new HashSet<T>();

		private readonly HashSet<T> m_UnorderedBufferedRemove = new HashSet<T>();

		private readonly HashSet<T> m_UnorderedRegisteredSnapshot = new HashSet<T>();

		private readonly HashSet<T> m_UnorderedRegisteredItems = new HashSet<T>();

		private bool m_BufferedRemoveEmpty = true;
	}
}
