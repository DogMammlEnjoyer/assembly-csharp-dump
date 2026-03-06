using System;
using System.Collections.Generic;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text
{
	[VisibleToOtherModules(new string[]
	{
		"UnityEngine.UIElementsModule"
	})]
	internal class TextHandlePermanentCache
	{
		public virtual void AddTextInfoToCache(TextHandle textHandle)
		{
			object obj = this.syncRoot;
			lock (obj)
			{
				bool isCachedPermanent = textHandle.IsCachedPermanent;
				if (isCachedPermanent)
				{
					return;
				}
				bool isCachedTemporary = textHandle.IsCachedTemporary;
				if (isCachedTemporary)
				{
					textHandle.RemoveTextInfoFromTemporaryCache();
				}
				bool flag2 = this.s_TextInfoPool.Count > 0;
				if (flag2)
				{
					textHandle.TextInfoNode = this.s_TextInfoPool.Last;
					this.s_TextInfoPool.RemoveLast();
				}
				else
				{
					TextInfo value = new TextInfo();
					textHandle.TextInfoNode = new LinkedListNode<TextInfo>(value);
				}
			}
			textHandle.IsCachedPermanent = true;
			textHandle.SetDirty();
			textHandle.Update();
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEngine.UIElementsModule"
		})]
		public void RemoveTextInfoFromCache(TextHandle textHandle)
		{
			object obj = this.syncRoot;
			lock (obj)
			{
				bool flag2 = !textHandle.IsCachedPermanent;
				if (!flag2)
				{
					this.s_TextInfoPool.AddFirst(textHandle.TextInfoNode);
					textHandle.TextInfoNode = null;
					textHandle.IsCachedPermanent = false;
				}
			}
		}

		internal LinkedList<TextInfo> s_TextInfoPool = new LinkedList<TextInfo>();

		private object syncRoot = new object();
	}
}
