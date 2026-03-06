using System;

namespace UnityEngine.Rendering
{
	public delegate void ListChangedEventHandler<T>(ObservableList<T> sender, ListChangedEventArgs<T> e);
}
