using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	public struct StartDragArgs
	{
		public StartDragArgs(string title, DragVisualMode visualMode)
		{
			this.title = title;
			this.visualMode = visualMode;
			this.genericData = null;
			this.assetPaths = null;
			this.unityObjectReferences = null;
		}

		internal StartDragArgs(string title, object target)
		{
			this.title = title;
			this.visualMode = 2;
			this.genericData = null;
			this.assetPaths = null;
			this.unityObjectReferences = null;
			this.SetGenericData("__unity-drag-and-drop__source-view", target);
		}

		public readonly string title { get; }

		public readonly DragVisualMode visualMode { get; }

		internal Hashtable genericData { readonly get; private set; }

		internal IEnumerable<Object> unityObjectReferences { readonly get; private set; }

		internal string[] assetPaths { readonly get; private set; }

		public void SetGenericData(string key, object data)
		{
			if (this.genericData == null)
			{
				this.genericData = new Hashtable();
			}
			this.genericData[key] = data;
		}

		public void SetUnityObjectReferences(IEnumerable<Object> references)
		{
			this.unityObjectReferences = references;
		}

		public void SetPaths(string[] paths)
		{
			this.assetPaths = paths;
		}
	}
}
