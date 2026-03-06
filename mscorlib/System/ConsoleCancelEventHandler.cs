using System;

namespace System
{
	/// <summary>Represents the method that will handle the <see cref="E:System.Console.CancelKeyPress" /> event of a <see cref="T:System.Console" />.</summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">A <see cref="T:System.ConsoleCancelEventArgs" /> object that contains the event data.</param>
	public delegate void ConsoleCancelEventHandler(object sender, ConsoleCancelEventArgs e);
}
