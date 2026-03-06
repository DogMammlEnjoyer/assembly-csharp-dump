using System;
using System.Collections.Generic;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
	internal class SmallRegistrationList<T> : BaseRegistrationList<T>
	{
		public bool bufferChanges
		{
			get
			{
				return this.m_BufferChanges;
			}
			set
			{
				if (this.m_BufferChanges && !value)
				{
					this.m_BufferChanges = false;
					this.Flush();
					return;
				}
				this.m_BufferChanges = value;
			}
		}

		public override bool IsRegistered(T item)
		{
			return (base.bufferedAddCount > 0 && this.m_BufferedAdd.Contains(item)) || (base.registeredSnapshot.Count > 0 && base.registeredSnapshot.Contains(item) && this.IsStillRegistered(item));
		}

		public override bool IsStillRegistered(T item)
		{
			return base.bufferedRemoveCount == 0 || !this.m_BufferedRemove.Contains(item);
		}

		public override bool Register(T item)
		{
			if (!this.bufferChanges)
			{
				if (base.registeredSnapshot.Contains(item))
				{
					return false;
				}
				base.registeredSnapshot.Add(item);
				return true;
			}
			else
			{
				if (base.bufferedAddCount > 0 && this.m_BufferedAdd.Contains(item))
				{
					return false;
				}
				bool flag = base.registeredSnapshot.Contains(item);
				if ((base.bufferedRemoveCount > 0 && base.RemoveFromBufferedRemove(item)) || !flag)
				{
					if (!flag)
					{
						base.AddToBufferedAdd(item);
					}
					return true;
				}
				return false;
			}
		}

		public override bool Unregister(T item)
		{
			if (!this.bufferChanges)
			{
				return base.registeredSnapshot.Remove(item);
			}
			if (base.bufferedRemoveCount > 0 && this.m_BufferedRemove.Contains(item))
			{
				return false;
			}
			if (base.bufferedAddCount > 0 && base.RemoveFromBufferedAdd(item))
			{
				return true;
			}
			if (base.registeredSnapshot.Contains(item))
			{
				base.AddToBufferedRemove(item);
				return true;
			}
			return false;
		}

		public override void Flush()
		{
			if (base.bufferedRemoveCount > 0)
			{
				foreach (T item in this.m_BufferedRemove)
				{
					base.registeredSnapshot.Remove(item);
				}
				base.ClearBufferedRemove();
			}
			if (base.bufferedAddCount > 0)
			{
				foreach (T item2 in this.m_BufferedAdd)
				{
					if (!base.registeredSnapshot.Contains(item2))
					{
						base.registeredSnapshot.Add(item2);
					}
				}
				base.ClearBufferedAdd();
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
				if (base.bufferedRemoveCount <= 0 || !this.m_BufferedRemove.Contains(item))
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
				if (base.bufferedRemoveCount <= 0 || !this.m_BufferedRemove.Contains(item))
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

		private bool m_BufferChanges = true;
	}
}
