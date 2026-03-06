using System;
using System.Security.Permissions;

namespace System.ComponentModel
{
	/// <summary>Provides data for the <see cref="E:System.ComponentModel.BackgroundWorker.DoWork" /> event handler.</summary>
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	public class DoWorkEventArgs : CancelEventArgs
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.DoWorkEventArgs" /> class.</summary>
		/// <param name="argument">Specifies an argument for an asynchronous operation.</param>
		public DoWorkEventArgs(object argument)
		{
			this.argument = argument;
		}

		/// <summary>Gets a value that represents the argument of an asynchronous operation.</summary>
		/// <returns>An <see cref="T:System.Object" /> representing the argument of an asynchronous operation.</returns>
		[SRDescription("Argument passed into the worker handler from BackgroundWorker.RunWorkerAsync.")]
		public object Argument
		{
			get
			{
				return this.argument;
			}
		}

		/// <summary>Gets or sets a value that represents the result of an asynchronous operation.</summary>
		/// <returns>An <see cref="T:System.Object" /> representing the result of an asynchronous operation.</returns>
		[SRDescription("Result from the worker function.")]
		public object Result
		{
			get
			{
				return this.result;
			}
			set
			{
				this.result = value;
			}
		}

		private object result;

		private object argument;
	}
}
