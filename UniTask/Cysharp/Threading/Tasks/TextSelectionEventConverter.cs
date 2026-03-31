using System;
using UnityEngine.Events;

namespace Cysharp.Threading.Tasks
{
	internal class TextSelectionEventConverter : UnityEvent<ValueTuple<string, int, int>>, IDisposable
	{
		public TextSelectionEventConverter(UnityEvent<string, int, int> unityEvent)
		{
			this.innerEvent = unityEvent;
			this.invokeDelegate = new UnityAction<string, int, int>(this.InvokeCore);
			this.innerEvent.AddListener(this.invokeDelegate);
		}

		private void InvokeCore(string item1, int item2, int item3)
		{
			this.innerEvent.Invoke(item1, item2, item3);
		}

		public void Dispose()
		{
			this.innerEvent.RemoveListener(this.invokeDelegate);
		}

		private readonly UnityEvent<string, int, int> innerEvent;

		private readonly UnityAction<string, int, int> invokeDelegate;
	}
}
