using System;
using System.ComponentModel;

namespace System.Net
{
	/// <summary>Represents the method that will handle the <see cref="E:System.Net.WebClient.WriteStreamClosed" /> event of a <see cref="T:System.Net.WebClient" />.</summary>
	/// <param name="sender" />
	/// <param name="e" />
	[EditorBrowsable(EditorBrowsableState.Never)]
	public delegate void WriteStreamClosedEventHandler(object sender, WriteStreamClosedEventArgs e);
}
