using System;
using System.Collections.Generic;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text
{
	[VisibleToOtherModules(new string[]
	{
		"UnityEngine.UIElementsModule"
	})]
	internal class TextHandleTemporaryCache
	{
		public void ClearTemporaryCache()
		{
			for (int i = 0; i < this.s_TextInfoPool.Count; i++)
			{
				this.s_TextInfoPool.First.Value.RemoveFromCache();
			}
			this.s_TextInfoPool.Clear();
		}

		public void AddTextInfoToCache(TextHandle textHandle, int hashCode)
		{
			object obj = this.syncRoot;
			lock (obj)
			{
				bool isCachedPermanent = textHandle.IsCachedPermanent;
				if (isCachedPermanent)
				{
					return;
				}
				bool flag2 = !TextGenerator.IsExecutingJob;
				bool flag3 = flag2;
				if (flag3)
				{
					this.currentFrame = Time.frameCount;
				}
				bool flag4 = this.s_TextInfoPool.Count > 0 && ((double)this.currentFrame - this.s_TextInfoPool.Last.Value.lastTimeInCache < 0.0 || (double)this.currentFrame - this.s_TextInfoPool.First.Value.lastTimeInCache < 0.0);
				if (flag4)
				{
					this.ClearTemporaryCache();
				}
				bool isCachedTemporary = textHandle.IsCachedTemporary;
				if (isCachedTemporary)
				{
					this.RefreshCaching(textHandle);
					return;
				}
				bool flag5 = this.s_TextInfoPool.Count > 0 && (double)this.currentFrame - this.s_TextInfoPool.Last.Value.lastTimeInCache > 2.0;
				if (flag5)
				{
					this.RecycleTextInfoFromCache(textHandle);
				}
				else
				{
					TextInfo textInfo = new TextInfo();
					textHandle.TextInfoNode = new LinkedListNode<TextInfo>(textInfo);
					this.s_TextInfoPool.AddFirst(textHandle.TextInfoNode);
					textInfo.lastTimeInCache = (double)this.currentFrame;
					TextInfo textInfo2 = textInfo;
					textInfo2.removedFromCache = (Action)Delegate.Combine(textInfo2.removedFromCache, new Action(textHandle.RemoveTextInfoFromTemporaryCache));
				}
			}
			textHandle.IsCachedTemporary = true;
			textHandle.SetDirty();
			textHandle.UpdateWithHash(hashCode);
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEngine.UIElementsModule"
		})]
		public virtual void RemoveTextInfoFromCache(TextHandle textHandle)
		{
			object obj = this.syncRoot;
			lock (obj)
			{
				bool flag2 = !textHandle.IsCachedTemporary;
				if (!flag2)
				{
					textHandle.IsCachedTemporary = false;
					textHandle.TextInfoNode.Value.lastTimeInCache = 0.0;
					textHandle.TextInfoNode.Value.removedFromCache = null;
					bool flag3 = textHandle.TextInfoNode != null;
					if (flag3)
					{
						this.s_TextInfoPool.Remove(textHandle.TextInfoNode);
						this.s_TextInfoPool.AddLast(textHandle.TextInfoNode);
					}
					textHandle.TextInfoNode = null;
				}
			}
		}

		private void RefreshCaching(TextHandle textHandle)
		{
			bool flag = !TextGenerator.IsExecutingJob;
			if (flag)
			{
				this.currentFrame = Time.frameCount;
			}
			textHandle.TextInfoNode.Value.lastTimeInCache = (double)this.currentFrame;
			this.s_TextInfoPool.Remove(textHandle.TextInfoNode);
			this.s_TextInfoPool.AddFirst(textHandle.TextInfoNode);
		}

		private void RecycleTextInfoFromCache(TextHandle textHandle)
		{
			bool flag = !TextGenerator.IsExecutingJob;
			if (flag)
			{
				this.currentFrame = Time.frameCount;
			}
			textHandle.TextInfoNode = this.s_TextInfoPool.Last;
			textHandle.TextInfoNode.Value.RemoveFromCache();
			this.s_TextInfoPool.RemoveLast();
			this.s_TextInfoPool.AddFirst(textHandle.TextInfoNode);
			textHandle.IsCachedTemporary = true;
			TextInfo value = textHandle.TextInfoNode.Value;
			value.removedFromCache = (Action)Delegate.Combine(value.removedFromCache, new Action(textHandle.RemoveTextInfoFromTemporaryCache));
			textHandle.TextInfoNode.Value.lastTimeInCache = (double)this.currentFrame;
		}

		public void UpdateCurrentFrame()
		{
			this.currentFrame = Time.frameCount;
		}

		internal LinkedList<TextInfo> s_TextInfoPool = new LinkedList<TextInfo>();

		internal const int s_MinFramesInCache = 2;

		internal int currentFrame;

		private object syncRoot = new object();
	}
}
