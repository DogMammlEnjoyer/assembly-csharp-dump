using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel
{
	/// <summary>Executes an operation on a separate thread.</summary>
	[DefaultEvent("DoWork")]
	public class BackgroundWorker : Component
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.BackgroundWorker" /> class.</summary>
		public BackgroundWorker()
		{
			this._operationCompleted = new SendOrPostCallback(this.AsyncOperationCompleted);
			this._progressReporter = new SendOrPostCallback(this.ProgressReporter);
		}

		private void AsyncOperationCompleted(object arg)
		{
			this._isRunning = false;
			this._cancellationPending = false;
			this.OnRunWorkerCompleted((RunWorkerCompletedEventArgs)arg);
		}

		/// <summary>Gets a value indicating whether the application has requested cancellation of a background operation.</summary>
		/// <returns>
		///   <see langword="true" /> if the application has requested cancellation of a background operation; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool CancellationPending
		{
			get
			{
				return this._cancellationPending;
			}
		}

		/// <summary>Requests cancellation of a pending background operation.</summary>
		/// <exception cref="T:System.InvalidOperationException">
		///   <see cref="P:System.ComponentModel.BackgroundWorker.WorkerSupportsCancellation" /> is <see langword="false" />.</exception>
		public void CancelAsync()
		{
			if (!this.WorkerSupportsCancellation)
			{
				throw new InvalidOperationException("This BackgroundWorker states that it doesn't support cancellation. Modify WorkerSupportsCancellation to state that it does support cancellation.");
			}
			this._cancellationPending = true;
		}

		/// <summary>Occurs when <see cref="M:System.ComponentModel.BackgroundWorker.RunWorkerAsync" /> is called.</summary>
		public event DoWorkEventHandler DoWork;

		/// <summary>Gets a value indicating whether the <see cref="T:System.ComponentModel.BackgroundWorker" /> is running an asynchronous operation.</summary>
		/// <returns>
		///   <see langword="true" />, if the <see cref="T:System.ComponentModel.BackgroundWorker" /> is running an asynchronous operation; otherwise, <see langword="false" />.</returns>
		public bool IsBusy
		{
			get
			{
				return this._isRunning;
			}
		}

		/// <summary>Raises the <see cref="E:System.ComponentModel.BackgroundWorker.DoWork" /> event.</summary>
		/// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
		protected virtual void OnDoWork(DoWorkEventArgs e)
		{
			DoWorkEventHandler doWork = this.DoWork;
			if (doWork != null)
			{
				doWork(this, e);
			}
		}

		/// <summary>Raises the <see cref="E:System.ComponentModel.BackgroundWorker.RunWorkerCompleted" /> event.</summary>
		/// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
		protected virtual void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
		{
			RunWorkerCompletedEventHandler runWorkerCompleted = this.RunWorkerCompleted;
			if (runWorkerCompleted != null)
			{
				runWorkerCompleted(this, e);
			}
		}

		/// <summary>Raises the <see cref="E:System.ComponentModel.BackgroundWorker.ProgressChanged" /> event.</summary>
		/// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
		protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
		{
			ProgressChangedEventHandler progressChanged = this.ProgressChanged;
			if (progressChanged != null)
			{
				progressChanged(this, e);
			}
		}

		/// <summary>Occurs when <see cref="M:System.ComponentModel.BackgroundWorker.ReportProgress(System.Int32)" /> is called.</summary>
		public event ProgressChangedEventHandler ProgressChanged;

		private void ProgressReporter(object arg)
		{
			this.OnProgressChanged((ProgressChangedEventArgs)arg);
		}

		/// <summary>Raises the <see cref="E:System.ComponentModel.BackgroundWorker.ProgressChanged" /> event.</summary>
		/// <param name="percentProgress">The percentage, from 0 to 100, of the background operation that is complete.</param>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.ComponentModel.BackgroundWorker.WorkerReportsProgress" /> property is set to <see langword="false" />.</exception>
		public void ReportProgress(int percentProgress)
		{
			this.ReportProgress(percentProgress, null);
		}

		/// <summary>Raises the <see cref="E:System.ComponentModel.BackgroundWorker.ProgressChanged" /> event.</summary>
		/// <param name="percentProgress">The percentage, from 0 to 100, of the background operation that is complete.</param>
		/// <param name="userState">The state object passed to <see cref="M:System.ComponentModel.BackgroundWorker.RunWorkerAsync(System.Object)" />.</param>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.ComponentModel.BackgroundWorker.WorkerReportsProgress" /> property is set to <see langword="false" />.</exception>
		public void ReportProgress(int percentProgress, object userState)
		{
			if (!this.WorkerReportsProgress)
			{
				throw new InvalidOperationException("This BackgroundWorker states that it doesn't report progress. Modify WorkerReportsProgress to state that it does report progress.");
			}
			ProgressChangedEventArgs progressChangedEventArgs = new ProgressChangedEventArgs(percentProgress, userState);
			if (this._asyncOperation != null)
			{
				this._asyncOperation.Post(this._progressReporter, progressChangedEventArgs);
				return;
			}
			this._progressReporter(progressChangedEventArgs);
		}

		/// <summary>Starts execution of a background operation.</summary>
		/// <exception cref="T:System.InvalidOperationException">
		///   <see cref="P:System.ComponentModel.BackgroundWorker.IsBusy" /> is <see langword="true" />.</exception>
		public void RunWorkerAsync()
		{
			this.RunWorkerAsync(null);
		}

		/// <summary>Starts execution of a background operation.</summary>
		/// <param name="argument">A parameter for use by the background operation to be executed in the <see cref="E:System.ComponentModel.BackgroundWorker.DoWork" /> event handler.</param>
		/// <exception cref="T:System.InvalidOperationException">
		///   <see cref="P:System.ComponentModel.BackgroundWorker.IsBusy" /> is <see langword="true" />.</exception>
		public void RunWorkerAsync(object argument)
		{
			if (this._isRunning)
			{
				throw new InvalidOperationException("This BackgroundWorker is currently busy and cannot run multiple tasks concurrently.");
			}
			this._isRunning = true;
			this._cancellationPending = false;
			this._asyncOperation = AsyncOperationManager.CreateOperation(null);
			Task.Factory.StartNew(delegate(object arg)
			{
				this.WorkerThreadStart(arg);
			}, argument, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
		}

		/// <summary>Occurs when the background operation has completed, has been canceled, or has raised an exception.</summary>
		public event RunWorkerCompletedEventHandler RunWorkerCompleted;

		/// <summary>Gets or sets a value indicating whether the <see cref="T:System.ComponentModel.BackgroundWorker" /> can report progress updates.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.ComponentModel.BackgroundWorker" /> supports progress updates; otherwise <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool WorkerReportsProgress
		{
			get
			{
				return this._workerReportsProgress;
			}
			set
			{
				this._workerReportsProgress = value;
			}
		}

		/// <summary>Gets or sets a value indicating whether the <see cref="T:System.ComponentModel.BackgroundWorker" /> supports asynchronous cancellation.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.ComponentModel.BackgroundWorker" /> supports cancellation; otherwise <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool WorkerSupportsCancellation
		{
			get
			{
				return this._canCancelWorker;
			}
			set
			{
				this._canCancelWorker = value;
			}
		}

		private void WorkerThreadStart(object argument)
		{
			object result = null;
			Exception error = null;
			bool cancelled = false;
			try
			{
				DoWorkEventArgs doWorkEventArgs = new DoWorkEventArgs(argument);
				this.OnDoWork(doWorkEventArgs);
				if (doWorkEventArgs.Cancel)
				{
					cancelled = true;
				}
				else
				{
					result = doWorkEventArgs.Result;
				}
			}
			catch (Exception error)
			{
			}
			RunWorkerCompletedEventArgs arg = new RunWorkerCompletedEventArgs(result, error, cancelled);
			this._asyncOperation.PostOperationCompleted(this._operationCompleted, arg);
		}

		protected override void Dispose(bool disposing)
		{
		}

		private bool _canCancelWorker;

		private bool _workerReportsProgress;

		private bool _cancellationPending;

		private bool _isRunning;

		private AsyncOperation _asyncOperation;

		private readonly SendOrPostCallback _operationCompleted;

		private readonly SendOrPostCallback _progressReporter;
	}
}
