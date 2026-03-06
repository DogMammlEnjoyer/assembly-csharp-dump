using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace System.Diagnostics
{
	/// <summary>Provides access to local and remote processes and enables you to start and stop local system processes.</summary>
	[DefaultEvent("Exited")]
	[Designer("System.Diagnostics.Design.ProcessDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[MonitoringDescription("Provides access to local and remote processes, enabling starting and stopping of local processes.")]
	[DefaultProperty("StartInfo")]
	[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
	[HostProtection(SecurityAction.LinkDemand, SharedState = true, Synchronization = true, ExternalProcessMgmt = true, SelfAffectingProcessMgmt = true)]
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	public class Process : Component
	{
		/// <summary>Occurs each time an application writes a line to its redirected <see cref="P:System.Diagnostics.Process.StandardOutput" /> stream.</summary>
		[Browsable(true)]
		[MonitoringDescription("Indicates if the process component is associated with a real process.")]
		public event DataReceivedEventHandler OutputDataReceived;

		/// <summary>Occurs when an application writes to its redirected <see cref="P:System.Diagnostics.Process.StandardError" /> stream.</summary>
		[MonitoringDescription("Indicates if the process component is associated with a real process.")]
		[Browsable(true)]
		public event DataReceivedEventHandler ErrorDataReceived;

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.Process" /> class.</summary>
		public Process()
		{
			this.machineName = ".";
			this.outputStreamReadMode = Process.StreamReadMode.undefined;
			this.errorStreamReadMode = Process.StreamReadMode.undefined;
			this.m_processAccess = 2035711;
		}

		private Process(string machineName, bool isRemoteMachine, int processId, ProcessInfo processInfo)
		{
			this.machineName = machineName;
			this.isRemoteMachine = isRemoteMachine;
			this.processId = processId;
			this.haveProcessId = true;
			this.outputStreamReadMode = Process.StreamReadMode.undefined;
			this.errorStreamReadMode = Process.StreamReadMode.undefined;
			this.m_processAccess = 2035711;
		}

		[Browsable(false)]
		[MonitoringDescription("Indicates if the process component is associated with a real process.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		private bool Associated
		{
			get
			{
				return this.haveProcessId || this.haveProcessHandle;
			}
		}

		/// <summary>Gets the value that the associated process specified when it terminated.</summary>
		/// <returns>The code that the associated process specified when it terminated.</returns>
		/// <exception cref="T:System.InvalidOperationException">The process has not exited.  
		///  -or-  
		///  The process <see cref="P:System.Diagnostics.Process.Handle" /> is not valid.</exception>
		/// <exception cref="T:System.NotSupportedException">You are trying to access the <see cref="P:System.Diagnostics.Process.ExitCode" /> property for a process that is running on a remote computer. This property is available only for processes that are running on the local computer.</exception>
		[MonitoringDescription("The value returned from the associated process when it terminated.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public int ExitCode
		{
			get
			{
				this.EnsureState(Process.State.Exited);
				if (this.exitCode == -1 && !RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					throw new InvalidOperationException("Cannot get the exit code from a non-child process on Unix");
				}
				return this.exitCode;
			}
		}

		/// <summary>Gets a value indicating whether the associated process has been terminated.</summary>
		/// <returns>
		///   <see langword="true" /> if the operating system process referenced by the <see cref="T:System.Diagnostics.Process" /> component has terminated; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">There is no process associated with the object.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">The exit code for the process could not be retrieved.</exception>
		/// <exception cref="T:System.NotSupportedException">You are trying to access the <see cref="P:System.Diagnostics.Process.HasExited" /> property for a process that is running on a remote computer. This property is available only for processes that are running on the local computer.</exception>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("Indicates if the associated process has been terminated.")]
		public bool HasExited
		{
			get
			{
				if (!this.exited)
				{
					this.EnsureState(Process.State.Associated);
					SafeProcessHandle safeProcessHandle = null;
					try
					{
						safeProcessHandle = this.GetProcessHandle(1049600, false);
						int num;
						if (safeProcessHandle.IsInvalid)
						{
							this.exited = true;
						}
						else if (NativeMethods.GetExitCodeProcess(safeProcessHandle, out num) && num != 259)
						{
							this.exited = true;
							this.exitCode = num;
						}
						else
						{
							if (!this.signaled)
							{
								ProcessWaitHandle processWaitHandle = null;
								try
								{
									processWaitHandle = new ProcessWaitHandle(safeProcessHandle);
									this.signaled = processWaitHandle.WaitOne(0, false);
								}
								finally
								{
									if (processWaitHandle != null)
									{
										processWaitHandle.Close();
									}
								}
							}
							if (this.signaled)
							{
								if (!NativeMethods.GetExitCodeProcess(safeProcessHandle, out num))
								{
									throw new Win32Exception();
								}
								this.exited = true;
								this.exitCode = num;
							}
						}
					}
					finally
					{
						this.ReleaseProcessHandle(safeProcessHandle);
					}
					if (this.exited)
					{
						this.RaiseOnExited();
					}
				}
				return this.exited;
			}
		}

		private ProcessThreadTimes GetProcessTimes()
		{
			ProcessThreadTimes processThreadTimes = new ProcessThreadTimes();
			SafeProcessHandle safeProcessHandle = null;
			try
			{
				int access = 1024;
				if (EnvironmentHelpers.IsWindowsVistaOrAbove())
				{
					access = 4096;
				}
				safeProcessHandle = this.GetProcessHandle(access, false);
				if (safeProcessHandle.IsInvalid)
				{
					throw new InvalidOperationException(SR.GetString("Cannot process request because the process ({0}) has exited.", new object[]
					{
						this.processId.ToString(CultureInfo.CurrentCulture)
					}));
				}
				if (!NativeMethods.GetProcessTimes(safeProcessHandle, out processThreadTimes.create, out processThreadTimes.exit, out processThreadTimes.kernel, out processThreadTimes.user))
				{
					throw new Win32Exception();
				}
			}
			finally
			{
				this.ReleaseProcessHandle(safeProcessHandle);
			}
			return processThreadTimes;
		}

		/// <summary>Gets the time that the associated process exited.</summary>
		/// <returns>A <see cref="T:System.DateTime" /> that indicates when the associated process was terminated.</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support this property.</exception>
		/// <exception cref="T:System.NotSupportedException">You are trying to access the <see cref="P:System.Diagnostics.Process.ExitTime" /> property for a process that is running on a remote computer. This property is available only for processes that are running on the local computer.</exception>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The time that the associated process exited.")]
		public DateTime ExitTime
		{
			get
			{
				if (!this.haveExitTime)
				{
					this.EnsureState((Process.State)20);
					this.exitTime = this.GetProcessTimes().ExitTime;
					this.haveExitTime = true;
				}
				return this.exitTime;
			}
		}

		/// <summary>Gets the native handle of the associated process.</summary>
		/// <returns>The handle that the operating system assigned to the associated process when the process was started. The system uses this handle to keep track of process attributes.</returns>
		/// <exception cref="T:System.InvalidOperationException">The process has not been started or has exited. The <see cref="P:System.Diagnostics.Process.Handle" /> property cannot be read because there is no process associated with this <see cref="T:System.Diagnostics.Process" /> instance.  
		///  -or-  
		///  The <see cref="T:System.Diagnostics.Process" /> instance has been attached to a running process but you do not have the necessary permissions to get a handle with full access rights.</exception>
		/// <exception cref="T:System.NotSupportedException">You are trying to access the <see cref="P:System.Diagnostics.Process.Handle" /> property for a process that is running on a remote computer. This property is available only for processes that are running on the local computer.</exception>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("Returns the native handle for this process.   The handle is only available if the process was started using this component.")]
		public IntPtr Handle
		{
			get
			{
				this.EnsureState(Process.State.Associated);
				return this.OpenProcessHandle(this.m_processAccess).DangerousGetHandle();
			}
		}

		/// <summary>Gets the native handle to this process.</summary>
		/// <returns>The native handle to this process.</returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public SafeProcessHandle SafeHandle
		{
			get
			{
				this.EnsureState(Process.State.Associated);
				return this.OpenProcessHandle(this.m_processAccess);
			}
		}

		/// <summary>Gets the unique identifier for the associated process.</summary>
		/// <returns>The system-generated unique identifier of the process that is referenced by this <see cref="T:System.Diagnostics.Process" /> instance.</returns>
		/// <exception cref="T:System.InvalidOperationException">The process's <see cref="P:System.Diagnostics.Process.Id" /> property has not been set.  
		///  -or-  
		///  There is no process associated with this <see cref="T:System.Diagnostics.Process" /> object.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me); set the <see cref="P:System.Diagnostics.ProcessStartInfo.UseShellExecute" /> property to <see langword="false" /> to access this property on Windows 98 and Windows Me.</exception>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The unique identifier for the process.")]
		public int Id
		{
			get
			{
				this.EnsureState(Process.State.HaveId);
				return this.processId;
			}
		}

		/// <summary>Gets the name of the computer the associated process is running on.</summary>
		/// <returns>The name of the computer that the associated process is running on.</returns>
		/// <exception cref="T:System.InvalidOperationException">There is no process associated with this <see cref="T:System.Diagnostics.Process" /> object.</exception>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[MonitoringDescription("The name of the machine the running the process.")]
		public string MachineName
		{
			get
			{
				this.EnsureState(Process.State.Associated);
				return this.machineName;
			}
		}

		/// <summary>Gets or sets the maximum allowable working set size, in bytes, for the associated process.</summary>
		/// <returns>The maximum working set size that is allowed in memory for the process, in bytes.</returns>
		/// <exception cref="T:System.ArgumentException">The maximum working set size is invalid. It must be greater than or equal to the minimum working set size.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">Working set information cannot be retrieved from the associated process resource.  
		///  -or-  
		///  The process identifier or process handle is zero because the process has not been started.</exception>
		/// <exception cref="T:System.NotSupportedException">You are trying to access the <see cref="P:System.Diagnostics.Process.MaxWorkingSet" /> property for a process that is running on a remote computer. This property is available only for processes that are running on the local computer.</exception>
		/// <exception cref="T:System.InvalidOperationException">The process <see cref="P:System.Diagnostics.Process.Id" /> is not available.  
		///  -or-  
		///  The process has exited.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support this property.</exception>
		[MonitoringDescription("The maximum amount of physical memory the process has required since it was started.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IntPtr MaxWorkingSet
		{
			get
			{
				this.EnsureWorkingSetLimits();
				return this.maxWorkingSet;
			}
			set
			{
				this.SetWorkingSetLimits(null, value);
			}
		}

		/// <summary>Gets or sets the minimum allowable working set size, in bytes, for the associated process.</summary>
		/// <returns>The minimum working set size that is required in memory for the process, in bytes.</returns>
		/// <exception cref="T:System.ArgumentException">The minimum working set size is invalid. It must be less than or equal to the maximum working set size.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">Working set information cannot be retrieved from the associated process resource.  
		///  -or-  
		///  The process identifier or process handle is zero because the process has not been started.</exception>
		/// <exception cref="T:System.NotSupportedException">You are trying to access the <see cref="P:System.Diagnostics.Process.MinWorkingSet" /> property for a process that is running on a remote computer. This property is available only for processes that are running on the local computer.</exception>
		/// <exception cref="T:System.InvalidOperationException">The process <see cref="P:System.Diagnostics.Process.Id" /> is not available.  
		///  -or-  
		///  The process has exited.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support this property.</exception>
		[MonitoringDescription("The minimum amount of physical memory the process has required since it was started.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IntPtr MinWorkingSet
		{
			get
			{
				this.EnsureWorkingSetLimits();
				return this.minWorkingSet;
			}
			set
			{
				this.SetWorkingSetLimits(value, null);
			}
		}

		private OperatingSystem OperatingSystem
		{
			get
			{
				if (this.operatingSystem == null)
				{
					this.operatingSystem = Environment.OSVersion;
				}
				return this.operatingSystem;
			}
		}

		/// <summary>Gets or sets the overall priority category for the associated process.</summary>
		/// <returns>The priority category for the associated process, from which the <see cref="P:System.Diagnostics.Process.BasePriority" /> of the process is calculated.</returns>
		/// <exception cref="T:System.ComponentModel.Win32Exception">Process priority information could not be set or retrieved from the associated process resource.  
		///  -or-  
		///  The process identifier or process handle is zero. (The process has not been started.)</exception>
		/// <exception cref="T:System.NotSupportedException">You are attempting to access the <see cref="P:System.Diagnostics.Process.PriorityClass" /> property for a process that is running on a remote computer. This property is available only for processes that are running on the local computer.</exception>
		/// <exception cref="T:System.InvalidOperationException">The process <see cref="P:System.Diagnostics.Process.Id" /> is not available.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">You have set the <see cref="P:System.Diagnostics.Process.PriorityClass" /> to <see langword="AboveNormal" /> or <see langword="BelowNormal" /> when using Windows 98 or Windows Millennium Edition (Windows Me). These platforms do not support those values for the priority class.</exception>
		/// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">Priority class cannot be set because it does not use a valid value, as defined in the <see cref="T:System.Diagnostics.ProcessPriorityClass" /> enumeration.</exception>
		[MonitoringDescription("The priority that the threads in the process run relative to.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ProcessPriorityClass PriorityClass
		{
			get
			{
				if (!this.havePriorityClass)
				{
					SafeProcessHandle handle = null;
					try
					{
						handle = this.GetProcessHandle(1024);
						int num = NativeMethods.GetPriorityClass(handle);
						if (num == 0)
						{
							throw new Win32Exception();
						}
						this.priorityClass = (ProcessPriorityClass)num;
						this.havePriorityClass = true;
					}
					finally
					{
						this.ReleaseProcessHandle(handle);
					}
				}
				return this.priorityClass;
			}
			set
			{
				if (!Enum.IsDefined(typeof(ProcessPriorityClass), value))
				{
					throw new InvalidEnumArgumentException("value", (int)value, typeof(ProcessPriorityClass));
				}
				SafeProcessHandle handle = null;
				try
				{
					handle = this.GetProcessHandle(512);
					if (!NativeMethods.SetPriorityClass(handle, (int)value))
					{
						throw new Win32Exception();
					}
					this.priorityClass = value;
					this.havePriorityClass = true;
				}
				finally
				{
					this.ReleaseProcessHandle(handle);
				}
			}
		}

		/// <summary>Gets the privileged processor time for this process.</summary>
		/// <returns>A <see cref="T:System.TimeSpan" /> that indicates the amount of time that the process has spent running code inside the operating system core.</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support this property.</exception>
		/// <exception cref="T:System.NotSupportedException">You are attempting to access the <see cref="P:System.Diagnostics.Process.PrivilegedProcessorTime" /> property for a process that is running on a remote computer. This property is available only for processes that are running on the local computer.</exception>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The amount of CPU time the process spent inside the operating system core.")]
		public TimeSpan PrivilegedProcessorTime
		{
			get
			{
				this.EnsureState(Process.State.IsNt);
				return this.GetProcessTimes().PrivilegedProcessorTime;
			}
		}

		/// <summary>Gets or sets the properties to pass to the <see cref="M:System.Diagnostics.Process.Start" /> method of the <see cref="T:System.Diagnostics.Process" />.</summary>
		/// <returns>The <see cref="T:System.Diagnostics.ProcessStartInfo" /> that represents the data with which to start the process. These arguments include the name of the executable file or document used to start the process.</returns>
		/// <exception cref="T:System.ArgumentNullException">The value that specifies the <see cref="P:System.Diagnostics.Process.StartInfo" /> is <see langword="null" />.</exception>
		[MonitoringDescription("Specifies information used to start a process.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Browsable(false)]
		public ProcessStartInfo StartInfo
		{
			get
			{
				if (this.startInfo == null)
				{
					this.startInfo = new ProcessStartInfo(this);
				}
				return this.startInfo;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.startInfo = value;
			}
		}

		/// <summary>Gets the time that the associated process was started.</summary>
		/// <returns>An object  that indicates when the process started. An exception is thrown if the process is not running.</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support this property.</exception>
		/// <exception cref="T:System.NotSupportedException">You are attempting to access the <see cref="P:System.Diagnostics.Process.StartTime" /> property for a process that is running on a remote computer. This property is available only for processes that are running on the local computer.</exception>
		/// <exception cref="T:System.InvalidOperationException">The process has exited.  
		///  -or-  
		///  The process has not been started.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">An error occurred in the call to the Windows function.</exception>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The time at which the process was started.")]
		public DateTime StartTime
		{
			get
			{
				this.EnsureState(Process.State.IsNt);
				return this.GetProcessTimes().StartTime;
			}
		}

		/// <summary>Gets or sets the object used to marshal the event handler calls that are issued as a result of a process exit event.</summary>
		/// <returns>The <see cref="T:System.ComponentModel.ISynchronizeInvoke" /> used to marshal event handler calls that are issued as a result of an <see cref="E:System.Diagnostics.Process.Exited" /> event on the process.</returns>
		[Browsable(false)]
		[MonitoringDescription("The object used to marshal the event handler calls issued as a result of a Process exit.")]
		[DefaultValue(null)]
		public ISynchronizeInvoke SynchronizingObject
		{
			get
			{
				if (this.synchronizingObject == null && base.DesignMode)
				{
					IDesignerHost designerHost = (IDesignerHost)this.GetService(typeof(IDesignerHost));
					if (designerHost != null)
					{
						object rootComponent = designerHost.RootComponent;
						if (rootComponent != null && rootComponent is ISynchronizeInvoke)
						{
							this.synchronizingObject = (ISynchronizeInvoke)rootComponent;
						}
					}
				}
				return this.synchronizingObject;
			}
			set
			{
				this.synchronizingObject = value;
			}
		}

		/// <summary>Gets the total processor time for this process.</summary>
		/// <returns>A <see cref="T:System.TimeSpan" /> that indicates the amount of time that the associated process has spent utilizing the CPU. This value is the sum of the <see cref="P:System.Diagnostics.Process.UserProcessorTime" /> and the <see cref="P:System.Diagnostics.Process.PrivilegedProcessorTime" />.</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support this property.</exception>
		/// <exception cref="T:System.NotSupportedException">You are attempting to access the <see cref="P:System.Diagnostics.Process.TotalProcessorTime" /> property for a process that is running on a remote computer. This property is available only for processes that are running on the local computer.</exception>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The amount of CPU time the process has used.")]
		public TimeSpan TotalProcessorTime
		{
			get
			{
				this.EnsureState(Process.State.IsNt);
				return this.GetProcessTimes().TotalProcessorTime;
			}
		}

		/// <summary>Gets the user processor time for this process.</summary>
		/// <returns>A <see cref="T:System.TimeSpan" /> that indicates the amount of time that the associated process has spent running code inside the application portion of the process (not inside the operating system core).</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support this property.</exception>
		/// <exception cref="T:System.NotSupportedException">You are attempting to access the <see cref="P:System.Diagnostics.Process.UserProcessorTime" /> property for a process that is running on a remote computer. This property is available only for processes that are running on the local computer.</exception>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The amount of CPU time the process spent outside the operating system core.")]
		public TimeSpan UserProcessorTime
		{
			get
			{
				this.EnsureState(Process.State.IsNt);
				return this.GetProcessTimes().UserProcessorTime;
			}
		}

		/// <summary>Gets or sets whether the <see cref="E:System.Diagnostics.Process.Exited" /> event should be raised when the process terminates.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="E:System.Diagnostics.Process.Exited" /> event should be raised when the associated process is terminated (through either an exit or a call to <see cref="M:System.Diagnostics.Process.Kill" />); otherwise, <see langword="false" />. The default is <see langword="false" />. Note that the <see cref="E:System.Diagnostics.Process.Exited" /> event is raised even if the value of <see cref="P:System.Diagnostics.Process.EnableRaisingEvents" /> is <see langword="false" /> when the process exits during or before the user performs a <see cref="P:System.Diagnostics.Process.HasExited" /> check.</returns>
		[Browsable(false)]
		[DefaultValue(false)]
		[MonitoringDescription("Whether the process component should watch for the associated process to exit, and raise the Exited event.")]
		public bool EnableRaisingEvents
		{
			get
			{
				return this.watchForExit;
			}
			set
			{
				if (value != this.watchForExit)
				{
					if (this.Associated)
					{
						if (value)
						{
							this.OpenProcessHandle();
							this.EnsureWatchingForExit();
						}
						else
						{
							this.StopWatchingForExit();
						}
					}
					this.watchForExit = value;
				}
			}
		}

		/// <summary>Gets a stream used to write the input of the application.</summary>
		/// <returns>A <see cref="T:System.IO.StreamWriter" /> that can be used to write the standard input stream of the application.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.Diagnostics.Process.StandardInput" /> stream has not been defined because <see cref="P:System.Diagnostics.ProcessStartInfo.RedirectStandardInput" /> is set to <see langword="false" />.</exception>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("Standard input stream of the process.")]
		public StreamWriter StandardInput
		{
			get
			{
				if (this.standardInput == null)
				{
					throw new InvalidOperationException(SR.GetString("StandardIn has not been redirected."));
				}
				this.inputStreamReadMode = Process.StreamReadMode.syncMode;
				return this.standardInput;
			}
		}

		/// <summary>Gets a stream used to read the textual output of the application.</summary>
		/// <returns>A <see cref="T:System.IO.StreamReader" /> that can be used to read the standard output stream of the application.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.Diagnostics.Process.StandardOutput" /> stream has not been defined for redirection; ensure <see cref="P:System.Diagnostics.ProcessStartInfo.RedirectStandardOutput" /> is set to <see langword="true" /> and <see cref="P:System.Diagnostics.ProcessStartInfo.UseShellExecute" /> is set to <see langword="false" />.  
		/// -or-
		///  The <see cref="P:System.Diagnostics.Process.StandardOutput" /> stream has been opened for asynchronous read operations with <see cref="M:System.Diagnostics.Process.BeginOutputReadLine" />.</exception>
		[MonitoringDescription("Standard output stream of the process.")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public StreamReader StandardOutput
		{
			get
			{
				if (this.standardOutput == null)
				{
					throw new InvalidOperationException(SR.GetString("StandardOut has not been redirected or the process hasn't started yet."));
				}
				if (this.outputStreamReadMode == Process.StreamReadMode.undefined)
				{
					this.outputStreamReadMode = Process.StreamReadMode.syncMode;
				}
				else if (this.outputStreamReadMode != Process.StreamReadMode.syncMode)
				{
					throw new InvalidOperationException(SR.GetString("Cannot mix synchronous and asynchronous operation on process stream."));
				}
				return this.standardOutput;
			}
		}

		/// <summary>Gets a stream used to read the error output of the application.</summary>
		/// <returns>A <see cref="T:System.IO.StreamReader" /> that can be used to read the standard error stream of the application.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.Diagnostics.Process.StandardError" /> stream has not been defined for redirection; ensure <see cref="P:System.Diagnostics.ProcessStartInfo.RedirectStandardError" /> is set to <see langword="true" /> and <see cref="P:System.Diagnostics.ProcessStartInfo.UseShellExecute" /> is set to <see langword="false" />.  
		/// -or-
		///  The <see cref="P:System.Diagnostics.Process.StandardError" /> stream has been opened for asynchronous read operations with <see cref="M:System.Diagnostics.Process.BeginErrorReadLine" />.</exception>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("Standard error stream of the process.")]
		public StreamReader StandardError
		{
			get
			{
				if (this.standardError == null)
				{
					throw new InvalidOperationException(SR.GetString("StandardError has not been redirected."));
				}
				if (this.errorStreamReadMode == Process.StreamReadMode.undefined)
				{
					this.errorStreamReadMode = Process.StreamReadMode.syncMode;
				}
				else if (this.errorStreamReadMode != Process.StreamReadMode.syncMode)
				{
					throw new InvalidOperationException(SR.GetString("Cannot mix synchronous and asynchronous operation on process stream."));
				}
				return this.standardError;
			}
		}

		/// <summary>Occurs when a process exits.</summary>
		[Category("Behavior")]
		[MonitoringDescription("If the WatchForExit property is set to true, then this event is raised when the associated process exits.")]
		public event EventHandler Exited
		{
			add
			{
				this.onExited = (EventHandler)Delegate.Combine(this.onExited, value);
			}
			remove
			{
				this.onExited = (EventHandler)Delegate.Remove(this.onExited, value);
			}
		}

		private void ReleaseProcessHandle(SafeProcessHandle handle)
		{
			if (handle == null)
			{
				return;
			}
			if (this.haveProcessHandle && handle == this.m_processHandle)
			{
				return;
			}
			handle.Close();
		}

		private void CompletionCallback(object context, bool wasSignaled)
		{
			this.StopWatchingForExit();
			this.RaiseOnExited();
		}

		/// <summary>Release all resources used by this process.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					this.Close();
				}
				this.disposed = true;
				base.Dispose(disposing);
			}
		}

		/// <summary>Frees all the resources that are associated with this component.</summary>
		public void Close()
		{
			if (this.Associated)
			{
				if (this.haveProcessHandle)
				{
					this.StopWatchingForExit();
					this.m_processHandle.Close();
					this.m_processHandle = null;
					this.haveProcessHandle = false;
				}
				this.haveProcessId = false;
				this.isRemoteMachine = false;
				this.machineName = ".";
				this.raisedOnExited = false;
				StreamWriter streamWriter = this.standardInput;
				this.standardInput = null;
				if (this.inputStreamReadMode == Process.StreamReadMode.undefined && streamWriter != null)
				{
					streamWriter.Close();
				}
				StreamReader streamReader = this.standardOutput;
				this.standardOutput = null;
				if (this.outputStreamReadMode == Process.StreamReadMode.undefined && streamReader != null)
				{
					streamReader.Close();
				}
				streamReader = this.standardError;
				this.standardError = null;
				if (this.errorStreamReadMode == Process.StreamReadMode.undefined && streamReader != null)
				{
					streamReader.Close();
				}
				AsyncStreamReader asyncStreamReader = this.output;
				this.output = null;
				if (this.outputStreamReadMode == Process.StreamReadMode.asyncMode && asyncStreamReader != null)
				{
					asyncStreamReader.CancelOperation();
					asyncStreamReader.Close();
				}
				asyncStreamReader = this.error;
				this.error = null;
				if (this.errorStreamReadMode == Process.StreamReadMode.asyncMode && asyncStreamReader != null)
				{
					asyncStreamReader.CancelOperation();
					asyncStreamReader.Close();
				}
				this.Refresh();
			}
		}

		private void EnsureState(Process.State state)
		{
			if ((state & Process.State.Associated) != (Process.State)0 && !this.Associated)
			{
				throw new InvalidOperationException(SR.GetString("No process is associated with this object."));
			}
			if ((state & Process.State.HaveId) != (Process.State)0 && !this.haveProcessId)
			{
				this.EnsureState(Process.State.Associated);
				throw new InvalidOperationException(SR.GetString("Feature requires a process identifier."));
			}
			if ((state & Process.State.IsLocal) != (Process.State)0 && this.isRemoteMachine)
			{
				throw new NotSupportedException(SR.GetString("Feature is not supported for remote machines."));
			}
			if ((state & Process.State.HaveProcessInfo) != (Process.State)0)
			{
				throw new InvalidOperationException(SR.GetString("Process has exited, so the requested information is not available."));
			}
			if ((state & Process.State.Exited) != (Process.State)0)
			{
				if (!this.HasExited)
				{
					throw new InvalidOperationException(SR.GetString("Process must exit before requested information can be determined."));
				}
				if (!this.haveProcessHandle)
				{
					throw new InvalidOperationException(SR.GetString("Process was not started by this object, so requested information cannot be determined."));
				}
			}
		}

		private void EnsureWatchingForExit()
		{
			if (!this.watchingForExit)
			{
				lock (this)
				{
					if (!this.watchingForExit)
					{
						this.watchingForExit = true;
						try
						{
							this.waitHandle = new ProcessWaitHandle(this.m_processHandle);
							this.registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(this.waitHandle, new WaitOrTimerCallback(this.CompletionCallback), null, -1, true);
						}
						catch
						{
							this.watchingForExit = false;
							throw;
						}
					}
				}
			}
		}

		private void EnsureWorkingSetLimits()
		{
			this.EnsureState(Process.State.IsNt);
			if (!this.haveWorkingSetLimits)
			{
				SafeProcessHandle handle = null;
				try
				{
					handle = this.GetProcessHandle(1024);
					IntPtr intPtr;
					IntPtr intPtr2;
					if (!NativeMethods.GetProcessWorkingSetSize(handle, out intPtr, out intPtr2))
					{
						throw new Win32Exception();
					}
					this.minWorkingSet = intPtr;
					this.maxWorkingSet = intPtr2;
					this.haveWorkingSetLimits = true;
				}
				finally
				{
					this.ReleaseProcessHandle(handle);
				}
			}
		}

		/// <summary>Puts a <see cref="T:System.Diagnostics.Process" /> component in state to interact with operating system processes that run in a special mode by enabling the native property <see langword="SeDebugPrivilege" /> on the current thread.</summary>
		public static void EnterDebugMode()
		{
		}

		/// <summary>Takes a <see cref="T:System.Diagnostics.Process" /> component out of the state that lets it interact with operating system processes that run in a special mode.</summary>
		public static void LeaveDebugMode()
		{
		}

		/// <summary>Returns a new <see cref="T:System.Diagnostics.Process" /> component, given the identifier of a process on the local computer.</summary>
		/// <param name="processId">The system-unique identifier of a process resource.</param>
		/// <returns>A <see cref="T:System.Diagnostics.Process" /> component that is associated with the local process resource identified by the <paramref name="processId" /> parameter.</returns>
		/// <exception cref="T:System.ArgumentException">The process specified by the <paramref name="processId" /> parameter is not running. The identifier might be expired.</exception>
		/// <exception cref="T:System.InvalidOperationException">The process was not started by this object.</exception>
		public static Process GetProcessById(int processId)
		{
			return Process.GetProcessById(processId, ".");
		}

		/// <summary>Creates an array of new <see cref="T:System.Diagnostics.Process" /> components and associates them with all the process resources on the local computer that share the specified process name.</summary>
		/// <param name="processName">The friendly name of the process.</param>
		/// <returns>An array of type <see cref="T:System.Diagnostics.Process" /> that represents the process resources running the specified application or file.</returns>
		/// <exception cref="T:System.InvalidOperationException">There are problems accessing the performance counter API's used to get process information. This exception is specific to Windows NT, Windows 2000, and Windows XP.</exception>
		public static Process[] GetProcessesByName(string processName)
		{
			return Process.GetProcessesByName(processName, ".");
		}

		/// <summary>Creates a new <see cref="T:System.Diagnostics.Process" /> component for each process resource on the local computer.</summary>
		/// <returns>An array of type <see cref="T:System.Diagnostics.Process" /> that represents all the process resources running on the local computer.</returns>
		public static Process[] GetProcesses()
		{
			return Process.GetProcesses(".");
		}

		/// <summary>Gets a new <see cref="T:System.Diagnostics.Process" /> component and associates it with the currently active process.</summary>
		/// <returns>A new <see cref="T:System.Diagnostics.Process" /> component associated with the process resource that is running the calling application.</returns>
		public static Process GetCurrentProcess()
		{
			return new Process(".", false, NativeMethods.GetCurrentProcessId(), null);
		}

		/// <summary>Raises the <see cref="E:System.Diagnostics.Process.Exited" /> event.</summary>
		protected void OnExited()
		{
			EventHandler eventHandler = this.onExited;
			if (eventHandler != null)
			{
				if (this.SynchronizingObject != null && this.SynchronizingObject.InvokeRequired)
				{
					this.SynchronizingObject.BeginInvoke(eventHandler, new object[]
					{
						this,
						EventArgs.Empty
					});
					return;
				}
				eventHandler(this, EventArgs.Empty);
			}
		}

		private SafeProcessHandle GetProcessHandle(int access, bool throwIfExited)
		{
			if (this.haveProcessHandle)
			{
				if (throwIfExited)
				{
					ProcessWaitHandle processWaitHandle = null;
					try
					{
						processWaitHandle = new ProcessWaitHandle(this.m_processHandle);
						if (processWaitHandle.WaitOne(0, false))
						{
							if (this.haveProcessId)
							{
								throw new InvalidOperationException(SR.GetString("Cannot process request because the process ({0}) has exited.", new object[]
								{
									this.processId.ToString(CultureInfo.CurrentCulture)
								}));
							}
							throw new InvalidOperationException(SR.GetString("Cannot process request because the process has exited."));
						}
					}
					finally
					{
						if (processWaitHandle != null)
						{
							processWaitHandle.Close();
						}
					}
				}
				return this.m_processHandle;
			}
			this.EnsureState((Process.State)3);
			SafeProcessHandle invalidHandle = SafeProcessHandle.InvalidHandle;
			IntPtr currentProcess = NativeMethods.GetCurrentProcess();
			if (!NativeMethods.DuplicateHandle(new HandleRef(this, currentProcess), new HandleRef(this, currentProcess), new HandleRef(this, currentProcess), out invalidHandle, 0, false, 3))
			{
				throw new Win32Exception();
			}
			if (throwIfExited && (access & 1024) != 0 && NativeMethods.GetExitCodeProcess(invalidHandle, out this.exitCode) && this.exitCode != 259)
			{
				throw new InvalidOperationException(SR.GetString("Cannot process request because the process ({0}) has exited.", new object[]
				{
					this.processId.ToString(CultureInfo.CurrentCulture)
				}));
			}
			return invalidHandle;
		}

		private SafeProcessHandle GetProcessHandle(int access)
		{
			return this.GetProcessHandle(access, true);
		}

		private SafeProcessHandle OpenProcessHandle()
		{
			return this.OpenProcessHandle(2035711);
		}

		private SafeProcessHandle OpenProcessHandle(int access)
		{
			if (!this.haveProcessHandle)
			{
				if (this.disposed)
				{
					throw new ObjectDisposedException(base.GetType().Name);
				}
				this.SetProcessHandle(this.GetProcessHandle(access));
			}
			return this.m_processHandle;
		}

		/// <summary>Discards any information about the associated process that has been cached inside the process component.</summary>
		public void Refresh()
		{
			this.threads = null;
			this.modules = null;
			this.exited = false;
			this.signaled = false;
			this.haveWorkingSetLimits = false;
			this.havePriorityClass = false;
			this.haveExitTime = false;
		}

		private void SetProcessHandle(SafeProcessHandle processHandle)
		{
			this.m_processHandle = processHandle;
			this.haveProcessHandle = true;
			if (this.watchForExit)
			{
				this.EnsureWatchingForExit();
			}
		}

		private void SetProcessId(int processId)
		{
			this.processId = processId;
			this.haveProcessId = true;
		}

		private void SetWorkingSetLimits(object newMin, object newMax)
		{
			this.EnsureState(Process.State.IsNt);
			SafeProcessHandle handle = null;
			try
			{
				handle = this.GetProcessHandle(1280);
				IntPtr intPtr;
				IntPtr intPtr2;
				if (!NativeMethods.GetProcessWorkingSetSize(handle, out intPtr, out intPtr2))
				{
					throw new Win32Exception();
				}
				if (newMin != null)
				{
					intPtr = (IntPtr)newMin;
				}
				if (newMax != null)
				{
					intPtr2 = (IntPtr)newMax;
				}
				if ((long)intPtr > (long)intPtr2)
				{
					if (newMin != null)
					{
						throw new ArgumentException(SR.GetString("Minimum working set size is invalid. It must be less than or equal to the maximum working set size."));
					}
					throw new ArgumentException(SR.GetString("Maximum working set size is invalid. It must be greater than or equal to the minimum working set size."));
				}
				else
				{
					if (!NativeMethods.SetProcessWorkingSetSize(handle, intPtr, intPtr2))
					{
						throw new Win32Exception();
					}
					if (!NativeMethods.GetProcessWorkingSetSize(handle, out intPtr, out intPtr2))
					{
						throw new Win32Exception();
					}
					this.minWorkingSet = intPtr;
					this.maxWorkingSet = intPtr2;
					this.haveWorkingSetLimits = true;
				}
			}
			finally
			{
				this.ReleaseProcessHandle(handle);
			}
		}

		/// <summary>Starts (or reuses) the process resource that is specified by the <see cref="P:System.Diagnostics.Process.StartInfo" /> property of this <see cref="T:System.Diagnostics.Process" /> component and associates it with the component.</summary>
		/// <returns>
		///   <see langword="true" /> if a process resource is started; <see langword="false" /> if no new process resource is started (for example, if an existing process is reused).</returns>
		/// <exception cref="T:System.InvalidOperationException">No file name was specified in the <see cref="T:System.Diagnostics.Process" /> component's <see cref="P:System.Diagnostics.Process.StartInfo" />.
		///  -or-
		///  The <see cref="P:System.Diagnostics.ProcessStartInfo.UseShellExecute" /> member of the <see cref="P:System.Diagnostics.Process.StartInfo" /> property is <see langword="true" /> while <see cref="P:System.Diagnostics.ProcessStartInfo.RedirectStandardInput" />, <see cref="P:System.Diagnostics.ProcessStartInfo.RedirectStandardOutput" />, or <see cref="P:System.Diagnostics.ProcessStartInfo.RedirectStandardError" /> is <see langword="true" />.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">There was an error in opening the associated file.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The process object has already been disposed.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">Method not supported on operating systems without shell support such as Nano Server (.NET Core only).</exception>
		public bool Start()
		{
			this.Close();
			ProcessStartInfo processStartInfo = this.StartInfo;
			if (processStartInfo.FileName.Length == 0)
			{
				throw new InvalidOperationException(SR.GetString("Cannot start process because a file name has not been provided."));
			}
			if (processStartInfo.UseShellExecute)
			{
				return this.StartWithShellExecuteEx(processStartInfo);
			}
			return this.StartWithCreateProcess(processStartInfo);
		}

		/// <summary>Starts a process resource by specifying the name of an application, a user name, a password, and a domain and associates the resource with a new <see cref="T:System.Diagnostics.Process" /> component.</summary>
		/// <param name="fileName">The name of an application file to run in the process.</param>
		/// <param name="userName">The user name to use when starting the process.</param>
		/// <param name="password">A <see cref="T:System.Security.SecureString" /> that contains the password to use when starting the process.</param>
		/// <param name="domain">The domain to use when starting the process.</param>
		/// <returns>A new <see cref="T:System.Diagnostics.Process" /> that is associated with the process resource, or <see langword="null" /> if no process resource is started. Note that a new process that's started alongside already running instances of the same process will be independent from the others. In addition, Start may return a non-null Process with its <see cref="P:System.Diagnostics.Process.HasExited" /> property already set to <see langword="true" />. In this case, the started process may have activated an existing instance of itself and then exited.</returns>
		/// <exception cref="T:System.InvalidOperationException">No file name was specified.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">There was an error in opening the associated file.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The process object has already been disposed.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">Method not supported on Linux or macOS (.NET Core only).</exception>
		public static Process Start(string fileName, string userName, SecureString password, string domain)
		{
			return Process.Start(new ProcessStartInfo(fileName)
			{
				UserName = userName,
				Password = password,
				Domain = domain,
				UseShellExecute = false
			});
		}

		/// <summary>Starts a process resource by specifying the name of an application, a set of command-line arguments, a user name, a password, and a domain and associates the resource with a new <see cref="T:System.Diagnostics.Process" /> component.</summary>
		/// <param name="fileName">The name of an application file to run in the process.</param>
		/// <param name="arguments">Command-line arguments to pass when starting the process.</param>
		/// <param name="userName">The user name to use when starting the process.</param>
		/// <param name="password">A <see cref="T:System.Security.SecureString" /> that contains the password to use when starting the process.</param>
		/// <param name="domain">The domain to use when starting the process.</param>
		/// <returns>A new <see cref="T:System.Diagnostics.Process" /> that is associated with the process resource, or <see langword="null" /> if no process resource is started. Note that a new process that's started alongside already running instances of the same process will be independent from the others. In addition, Start may return a non-null Process with its <see cref="P:System.Diagnostics.Process.HasExited" /> property already set to <see langword="true" />. In this case, the started process may have activated an existing instance of itself and then exited.</returns>
		/// <exception cref="T:System.InvalidOperationException">No file name was specified.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">An error occurred when opening the associated file.  
		///  -or-  
		///  The sum of the length of the arguments and the length of the full path to the associated file exceeds 2080. The error message associated with this exception can be one of the following: "The data area passed to a system call is too small." or "Access is denied."</exception>
		/// <exception cref="T:System.ObjectDisposedException">The process object has already been disposed.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">Method not supported on Linux or macOS (.NET Core only).</exception>
		public static Process Start(string fileName, string arguments, string userName, SecureString password, string domain)
		{
			return Process.Start(new ProcessStartInfo(fileName, arguments)
			{
				UserName = userName,
				Password = password,
				Domain = domain,
				UseShellExecute = false
			});
		}

		/// <summary>Starts a process resource by specifying the name of a document or application file and associates the resource with a new <see cref="T:System.Diagnostics.Process" /> component.</summary>
		/// <param name="fileName">The name of a document or application file to run in the process.</param>
		/// <returns>A new <see cref="T:System.Diagnostics.Process" /> that is associated with the process resource, or <see langword="null" /> if no process resource is started. Note that a new process that's started alongside already running instances of the same process will be independent from the others. In addition, Start may return a non-null Process with its <see cref="P:System.Diagnostics.Process.HasExited" /> property already set to <see langword="true" />. In this case, the started process may have activated an existing instance of itself and then exited.</returns>
		/// <exception cref="T:System.ComponentModel.Win32Exception">An error occurred when opening the associated file.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The process object has already been disposed.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">The PATH environment variable has a string containing quotes.</exception>
		public static Process Start(string fileName)
		{
			return Process.Start(new ProcessStartInfo(fileName));
		}

		/// <summary>Starts a process resource by specifying the name of an application and a set of command-line arguments, and associates the resource with a new <see cref="T:System.Diagnostics.Process" /> component.</summary>
		/// <param name="fileName">The name of an application file to run in the process.</param>
		/// <param name="arguments">Command-line arguments to pass when starting the process.</param>
		/// <returns>A new <see cref="T:System.Diagnostics.Process" /> that is associated with the process resource, or <see langword="null" /> if no process resource is started. Note that a new process that's started alongside already running instances of the same process will be independent from the others. In addition, Start may return a non-null Process with its <see cref="P:System.Diagnostics.Process.HasExited" /> property already set to <see langword="true" />. In this case, the started process may have activated an existing instance of itself and then exited.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <paramref name="fileName" /> or <paramref name="arguments" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">An error occurred when opening the associated file.  
		///  -or-  
		///  The sum of the length of the arguments and the length of the full path to the process exceeds 2080. The error message associated with this exception can be one of the following: "The data area passed to a system call is too small." or "Access is denied."</exception>
		/// <exception cref="T:System.ObjectDisposedException">The process object has already been disposed.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">The PATH environment variable has a string containing quotes.</exception>
		public static Process Start(string fileName, string arguments)
		{
			return Process.Start(new ProcessStartInfo(fileName, arguments));
		}

		/// <summary>Starts the process resource that is specified by the parameter containing process start information (for example, the file name of the process to start) and associates the resource with a new <see cref="T:System.Diagnostics.Process" /> component.</summary>
		/// <param name="startInfo">The <see cref="T:System.Diagnostics.ProcessStartInfo" /> that contains the information that is used to start the process, including the file name and any command-line arguments.</param>
		/// <returns>A new <see cref="T:System.Diagnostics.Process" /> that is associated with the process resource, or <see langword="null" /> if no process resource is started. Note that a new process that's started alongside already running instances of the same process will be independent from the others. In addition, Start may return a non-null Process with its <see cref="P:System.Diagnostics.Process.HasExited" /> property already set to <see langword="true" />. In this case, the started process may have activated an existing instance of itself and then exited.</returns>
		/// <exception cref="T:System.InvalidOperationException">No file name was specified in the <paramref name="startInfo" /> parameter's <see cref="P:System.Diagnostics.ProcessStartInfo.FileName" /> property.  
		///  -or-  
		///  The <see cref="P:System.Diagnostics.ProcessStartInfo.UseShellExecute" /> property of the <paramref name="startInfo" /> parameter is <see langword="true" /> and the <see cref="P:System.Diagnostics.ProcessStartInfo.RedirectStandardInput" />, <see cref="P:System.Diagnostics.ProcessStartInfo.RedirectStandardOutput" />, or <see cref="P:System.Diagnostics.ProcessStartInfo.RedirectStandardError" /> property is also <see langword="true" />.  
		///  -or-  
		///  The <see cref="P:System.Diagnostics.ProcessStartInfo.UseShellExecute" /> property of the <paramref name="startInfo" /> parameter is <see langword="true" /> and the <see cref="P:System.Diagnostics.ProcessStartInfo.UserName" /> property is not <see langword="null" /> or empty or the <see cref="P:System.Diagnostics.ProcessStartInfo.Password" /> property is not <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="startInfo" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ObjectDisposedException">The process object has already been disposed.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">The file specified in the <paramref name="startInfo" /> parameter's <see cref="P:System.Diagnostics.ProcessStartInfo.FileName" /> property could not be found.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">An error occurred when opening the associated file.  
		///  -or-  
		///  The sum of the length of the arguments and the length of the full path to the process exceeds 2080. The error message associated with this exception can be one of the following: "The data area passed to a system call is too small." or "Access is denied."</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">Method not supported on operating systems without shell support such as Nano Server (.NET Core only).</exception>
		public static Process Start(ProcessStartInfo startInfo)
		{
			Process process = new Process();
			if (startInfo == null)
			{
				throw new ArgumentNullException("startInfo");
			}
			process.StartInfo = startInfo;
			if (process.Start())
			{
				return process;
			}
			return null;
		}

		/// <summary>Immediately stops the associated process.</summary>
		/// <exception cref="T:System.ComponentModel.Win32Exception">The associated process could not be terminated.  
		///  -or-  
		///  The process is terminating.  
		///  -or-  
		///  The associated process is a Win16 executable.</exception>
		/// <exception cref="T:System.NotSupportedException">You are attempting to call <see cref="M:System.Diagnostics.Process.Kill" /> for a process that is running on a remote computer. The method is available only for processes running on the local computer.</exception>
		/// <exception cref="T:System.InvalidOperationException">The process has already exited.  
		///  -or-  
		///  There is no process associated with this <see cref="T:System.Diagnostics.Process" /> object.</exception>
		public void Kill()
		{
			SafeProcessHandle safeProcessHandle = null;
			try
			{
				safeProcessHandle = this.GetProcessHandle(1);
				if (!NativeMethods.TerminateProcess(safeProcessHandle, -1))
				{
					throw new Win32Exception();
				}
			}
			finally
			{
				this.ReleaseProcessHandle(safeProcessHandle);
			}
		}

		private void StopWatchingForExit()
		{
			if (this.watchingForExit)
			{
				lock (this)
				{
					if (this.watchingForExit)
					{
						this.watchingForExit = false;
						this.registeredWaitHandle.Unregister(null);
						this.waitHandle.Close();
						this.waitHandle = null;
						this.registeredWaitHandle = null;
					}
				}
			}
		}

		/// <summary>Formats the process's name as a string, combined with the parent component type, if applicable.</summary>
		/// <returns>The <see cref="P:System.Diagnostics.Process.ProcessName" />, combined with the base component's <see cref="M:System.Object.ToString" /> return value.</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">
		///   <see cref="M:System.Diagnostics.Process.ToString" /> is not supported on Windows 98.</exception>
		public override string ToString()
		{
			if (!this.Associated)
			{
				return base.ToString();
			}
			string text = string.Empty;
			try
			{
				text = this.ProcessName;
			}
			catch (PlatformNotSupportedException)
			{
			}
			if (text.Length != 0)
			{
				return string.Format(CultureInfo.CurrentCulture, "{0} ({1})", base.ToString(), text);
			}
			return base.ToString();
		}

		/// <summary>Instructs the <see cref="T:System.Diagnostics.Process" /> component to wait the specified number of milliseconds for the associated process to exit.</summary>
		/// <param name="milliseconds">The amount of time, in milliseconds, to wait for the associated process to exit. The maximum is the largest possible value of a 32-bit integer, which represents infinity to the operating system.</param>
		/// <returns>
		///   <see langword="true" /> if the associated process has exited; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ComponentModel.Win32Exception">The wait setting could not be accessed.</exception>
		/// <exception cref="T:System.SystemException">No process <see cref="P:System.Diagnostics.Process.Id" /> has been set, and a <see cref="P:System.Diagnostics.Process.Handle" /> from which the <see cref="P:System.Diagnostics.Process.Id" /> property can be determined does not exist.  
		///  -or-  
		///  There is no process associated with this <see cref="T:System.Diagnostics.Process" /> object.  
		///  -or-  
		///  You are attempting to call <see cref="M:System.Diagnostics.Process.WaitForExit(System.Int32)" /> for a process that is running on a remote computer. This method is available only for processes that are running on the local computer.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="milliseconds" /> is a negative number other than -1, which represents an infinite time-out.</exception>
		public bool WaitForExit(int milliseconds)
		{
			SafeProcessHandle safeProcessHandle = null;
			ProcessWaitHandle processWaitHandle = null;
			bool flag;
			try
			{
				safeProcessHandle = this.GetProcessHandle(1048576, false);
				if (safeProcessHandle.IsInvalid)
				{
					flag = true;
				}
				else
				{
					processWaitHandle = new ProcessWaitHandle(safeProcessHandle);
					if (processWaitHandle.WaitOne(milliseconds, false))
					{
						flag = true;
						this.signaled = true;
					}
					else
					{
						flag = false;
						this.signaled = false;
					}
				}
				if (this.output != null && milliseconds == -1)
				{
					this.output.WaitUtilEOF();
				}
				if (this.error != null && milliseconds == -1)
				{
					this.error.WaitUtilEOF();
				}
			}
			finally
			{
				if (processWaitHandle != null)
				{
					processWaitHandle.Close();
				}
				this.ReleaseProcessHandle(safeProcessHandle);
			}
			if (flag && this.watchForExit)
			{
				this.RaiseOnExited();
			}
			return flag;
		}

		/// <summary>Instructs the <see cref="T:System.Diagnostics.Process" /> component to wait indefinitely for the associated process to exit.</summary>
		/// <exception cref="T:System.ComponentModel.Win32Exception">The wait setting could not be accessed.</exception>
		/// <exception cref="T:System.SystemException">No process <see cref="P:System.Diagnostics.Process.Id" /> has been set, and a <see cref="P:System.Diagnostics.Process.Handle" /> from which the <see cref="P:System.Diagnostics.Process.Id" /> property can be determined does not exist.  
		///  -or-  
		///  There is no process associated with this <see cref="T:System.Diagnostics.Process" /> object.  
		///  -or-  
		///  You are attempting to call <see cref="M:System.Diagnostics.Process.WaitForExit" /> for a process that is running on a remote computer. This method is available only for processes that are running on the local computer.</exception>
		public void WaitForExit()
		{
			this.WaitForExit(-1);
		}

		/// <summary>Causes the <see cref="T:System.Diagnostics.Process" /> component to wait the specified number of milliseconds for the associated process to enter an idle state. This overload applies only to processes with a user interface and, therefore, a message loop.</summary>
		/// <param name="milliseconds">A value of 1 to <see cref="F:System.Int32.MaxValue" /> that specifies the amount of time, in milliseconds, to wait for the associated process to become idle. A value of 0 specifies an immediate return, and a value of -1 specifies an infinite wait.</param>
		/// <returns>
		///   <see langword="true" /> if the associated process has reached an idle state; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">The process does not have a graphical interface.  
		///  -or-  
		///  An unknown error occurred. The process failed to enter an idle state.  
		///  -or-  
		///  The process has already exited.  
		///  -or-  
		///  No process is associated with this <see cref="T:System.Diagnostics.Process" /> object.</exception>
		public bool WaitForInputIdle(int milliseconds)
		{
			SafeProcessHandle handle = null;
			try
			{
				handle = this.GetProcessHandle(1049600);
				int num = NativeMethods.WaitForInputIdle(handle, milliseconds);
				if (num != -1)
				{
					if (num == 0)
					{
						return true;
					}
					if (num == 258)
					{
						return false;
					}
				}
				throw new InvalidOperationException(SR.GetString("WaitForInputIdle failed.  This could be because the process does not have a graphical interface."));
			}
			finally
			{
				this.ReleaseProcessHandle(handle);
			}
			bool result;
			return result;
		}

		/// <summary>Causes the <see cref="T:System.Diagnostics.Process" /> component to wait indefinitely for the associated process to enter an idle state. This overload applies only to processes with a user interface and, therefore, a message loop.</summary>
		/// <returns>
		///   <see langword="true" /> if the associated process has reached an idle state.</returns>
		/// <exception cref="T:System.InvalidOperationException">The process does not have a graphical interface.  
		///  -or-  
		///  An unknown error occurred. The process failed to enter an idle state.  
		///  -or-  
		///  The process has already exited.  
		///  -or-  
		///  No process is associated with this <see cref="T:System.Diagnostics.Process" /> object.</exception>
		public bool WaitForInputIdle()
		{
			return this.WaitForInputIdle(int.MaxValue);
		}

		/// <summary>Begins asynchronous read operations on the redirected <see cref="P:System.Diagnostics.Process.StandardOutput" /> stream of the application.</summary>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.Diagnostics.ProcessStartInfo.RedirectStandardOutput" /> property is <see langword="false" />.  
		/// -or-
		///  An asynchronous read operation is already in progress on the <see cref="P:System.Diagnostics.Process.StandardOutput" /> stream.  
		/// -or-
		///  The <see cref="P:System.Diagnostics.Process.StandardOutput" /> stream has been used by a synchronous read operation.</exception>
		[ComVisible(false)]
		public void BeginOutputReadLine()
		{
			if (this.outputStreamReadMode == Process.StreamReadMode.undefined)
			{
				this.outputStreamReadMode = Process.StreamReadMode.asyncMode;
			}
			else if (this.outputStreamReadMode != Process.StreamReadMode.asyncMode)
			{
				throw new InvalidOperationException(SR.GetString("Cannot mix synchronous and asynchronous operation on process stream."));
			}
			if (this.pendingOutputRead)
			{
				throw new InvalidOperationException(SR.GetString("An async read operation has already been started on the stream."));
			}
			this.pendingOutputRead = true;
			if (this.output == null)
			{
				if (this.standardOutput == null)
				{
					throw new InvalidOperationException(SR.GetString("StandardOut has not been redirected or the process hasn't started yet."));
				}
				Stream baseStream = this.standardOutput.BaseStream;
				this.output = new AsyncStreamReader(this, baseStream, new UserCallBack(this.OutputReadNotifyUser), this.standardOutput.CurrentEncoding);
			}
			this.output.BeginReadLine();
		}

		/// <summary>Begins asynchronous read operations on the redirected <see cref="P:System.Diagnostics.Process.StandardError" /> stream of the application.</summary>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.Diagnostics.ProcessStartInfo.RedirectStandardError" /> property is <see langword="false" />.  
		/// -or-
		///  An asynchronous read operation is already in progress on the <see cref="P:System.Diagnostics.Process.StandardError" /> stream.  
		/// -or-
		///  The <see cref="P:System.Diagnostics.Process.StandardError" /> stream has been used by a synchronous read operation.</exception>
		[ComVisible(false)]
		public void BeginErrorReadLine()
		{
			if (this.errorStreamReadMode == Process.StreamReadMode.undefined)
			{
				this.errorStreamReadMode = Process.StreamReadMode.asyncMode;
			}
			else if (this.errorStreamReadMode != Process.StreamReadMode.asyncMode)
			{
				throw new InvalidOperationException(SR.GetString("Cannot mix synchronous and asynchronous operation on process stream."));
			}
			if (this.pendingErrorRead)
			{
				throw new InvalidOperationException(SR.GetString("An async read operation has already been started on the stream."));
			}
			this.pendingErrorRead = true;
			if (this.error == null)
			{
				if (this.standardError == null)
				{
					throw new InvalidOperationException(SR.GetString("StandardError has not been redirected."));
				}
				Stream baseStream = this.standardError.BaseStream;
				this.error = new AsyncStreamReader(this, baseStream, new UserCallBack(this.ErrorReadNotifyUser), this.standardError.CurrentEncoding);
			}
			this.error.BeginReadLine();
		}

		/// <summary>Cancels the asynchronous read operation on the redirected <see cref="P:System.Diagnostics.Process.StandardOutput" /> stream of an application.</summary>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.Diagnostics.Process.StandardOutput" /> stream is not enabled for asynchronous read operations.</exception>
		[ComVisible(false)]
		public void CancelOutputRead()
		{
			if (this.output != null)
			{
				this.output.CancelOperation();
				this.pendingOutputRead = false;
				return;
			}
			throw new InvalidOperationException(SR.GetString("No async read operation is in progress on the stream."));
		}

		/// <summary>Cancels the asynchronous read operation on the redirected <see cref="P:System.Diagnostics.Process.StandardError" /> stream of an application.</summary>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.Diagnostics.Process.StandardError" /> stream is not enabled for asynchronous read operations.</exception>
		[ComVisible(false)]
		public void CancelErrorRead()
		{
			if (this.error != null)
			{
				this.error.CancelOperation();
				this.pendingErrorRead = false;
				return;
			}
			throw new InvalidOperationException(SR.GetString("No async read operation is in progress on the stream."));
		}

		internal void OutputReadNotifyUser(string data)
		{
			DataReceivedEventHandler outputDataReceived = this.OutputDataReceived;
			if (outputDataReceived != null)
			{
				DataReceivedEventArgs dataReceivedEventArgs = new DataReceivedEventArgs(data);
				if (this.SynchronizingObject != null && this.SynchronizingObject.InvokeRequired)
				{
					this.SynchronizingObject.Invoke(outputDataReceived, new object[]
					{
						this,
						dataReceivedEventArgs
					});
					return;
				}
				outputDataReceived(this, dataReceivedEventArgs);
			}
		}

		internal void ErrorReadNotifyUser(string data)
		{
			DataReceivedEventHandler errorDataReceived = this.ErrorDataReceived;
			if (errorDataReceived != null)
			{
				DataReceivedEventArgs dataReceivedEventArgs = new DataReceivedEventArgs(data);
				if (this.SynchronizingObject != null && this.SynchronizingObject.InvokeRequired)
				{
					this.SynchronizingObject.Invoke(errorDataReceived, new object[]
					{
						this,
						dataReceivedEventArgs
					});
					return;
				}
				errorDataReceived(this, dataReceivedEventArgs);
			}
		}

		private Process(SafeProcessHandle handle, int id)
		{
			this.SetProcessHandle(handle);
			this.SetProcessId(id);
		}

		/// <summary>Gets the base priority of the associated process.</summary>
		/// <returns>The base priority, which is computed from the <see cref="P:System.Diagnostics.Process.PriorityClass" /> of the associated process.</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me); set the <see cref="P:System.Diagnostics.ProcessStartInfo.UseShellExecute" /> property to <see langword="false" /> to access this property on Windows 98 and Windows Me.</exception>
		/// <exception cref="T:System.InvalidOperationException">The process has exited.  
		///  -or-  
		///  The process has not started, so there is no process ID.</exception>
		[MonoTODO]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("Base process priority.")]
		public int BasePriority
		{
			get
			{
				return 0;
			}
		}

		/// <summary>Gets the number of handles opened by the process.</summary>
		/// <returns>The number of operating system handles the process has opened.</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me); set the <see cref="P:System.Diagnostics.ProcessStartInfo.UseShellExecute" /> property to <see langword="false" /> to access this property on Windows 98 and Windows Me.</exception>
		[MonitoringDescription("Handles for this process.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonoTODO]
		public int HandleCount
		{
			get
			{
				return 0;
			}
		}

		/// <summary>Gets the main module for the associated process.</summary>
		/// <returns>The <see cref="T:System.Diagnostics.ProcessModule" /> that was used to start the process.</returns>
		/// <exception cref="T:System.NotSupportedException">You are trying to access the <see cref="P:System.Diagnostics.Process.MainModule" /> property for a process that is running on a remote computer. This property is available only for processes that are running on the local computer.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">A 32-bit process is trying to access the modules of a 64-bit process.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me); set <see cref="P:System.Diagnostics.ProcessStartInfo.UseShellExecute" /> to <see langword="false" /> to access this property on Windows 98 and Windows Me.</exception>
		/// <exception cref="T:System.InvalidOperationException">The process <see cref="P:System.Diagnostics.Process.Id" /> is not available.  
		///  -or-  
		///  The process has exited.</exception>
		[MonitoringDescription("The main module of the process.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public ProcessModule MainModule
		{
			get
			{
				if (this.processId == NativeMethods.GetCurrentProcessId())
				{
					if (Process.current_main_module == null)
					{
						Process.current_main_module = this.Modules[0];
					}
					return Process.current_main_module;
				}
				return this.Modules[0];
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr MainWindowHandle_icall(int pid);

		/// <summary>Gets the window handle of the main window of the associated process.</summary>
		/// <returns>The system-generated window handle of the main window of the associated process.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.Diagnostics.Process.MainWindowHandle" /> is not defined because the process has exited.</exception>
		/// <exception cref="T:System.NotSupportedException">You are trying to access the <see cref="P:System.Diagnostics.Process.MainWindowHandle" /> property for a process that is running on a remote computer. This property is available only for processes that are running on the local computer.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me); set <see cref="P:System.Diagnostics.ProcessStartInfo.UseShellExecute" /> to <see langword="false" /> to access this property on Windows 98 and Windows Me.</exception>
		[MonitoringDescription("The handle of the main window of the process.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IntPtr MainWindowHandle
		{
			get
			{
				return Process.MainWindowHandle_icall(this.processId);
			}
		}

		/// <summary>Gets the caption of the main window of the process.</summary>
		/// <returns>The main window title of the process.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.Diagnostics.Process.MainWindowTitle" /> property is not defined because the process has exited.</exception>
		/// <exception cref="T:System.NotSupportedException">You are trying to access the <see cref="P:System.Diagnostics.Process.MainWindowTitle" /> property for a process that is running on a remote computer. This property is available only for processes that are running on the local computer.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me); set <see cref="P:System.Diagnostics.ProcessStartInfo.UseShellExecute" /> to <see langword="false" /> to access this property on Windows 98 and Windows Me.</exception>
		[MonoTODO]
		[MonitoringDescription("The title of the main window of the process.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string MainWindowTitle
		{
			get
			{
				return "null";
			}
		}

		private static void AppendArguments(StringBuilder stringBuilder, Collection<string> argumentList)
		{
			if (argumentList.Count > 0)
			{
				foreach (string argument in argumentList)
				{
					PasteArguments.AppendArgument(stringBuilder, argument);
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern ProcessModule[] GetModules_icall(IntPtr handle);

		private ProcessModule[] GetModules_internal(SafeProcessHandle handle)
		{
			bool flag = false;
			ProcessModule[] modules_icall;
			try
			{
				handle.DangerousAddRef(ref flag);
				modules_icall = this.GetModules_icall(handle.DangerousGetHandle());
			}
			finally
			{
				if (flag)
				{
					handle.DangerousRelease();
				}
			}
			return modules_icall;
		}

		/// <summary>Gets the modules that have been loaded by the associated process.</summary>
		/// <returns>An array of type <see cref="T:System.Diagnostics.ProcessModule" /> that represents the modules that have been loaded by the associated process.</returns>
		/// <exception cref="T:System.NotSupportedException">You are attempting to access the <see cref="P:System.Diagnostics.Process.Modules" /> property for a process that is running on a remote computer. This property is available only for processes that are running on the local computer.</exception>
		/// <exception cref="T:System.InvalidOperationException">The process <see cref="P:System.Diagnostics.Process.Id" /> is not available.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me); set <see cref="P:System.Diagnostics.ProcessStartInfo.UseShellExecute" /> to <see langword="false" /> to access this property on Windows 98 and Windows Me.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">You are attempting to access the <see cref="P:System.Diagnostics.Process.Modules" /> property for either the system process or the idle process. These processes do not have modules.</exception>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The modules that are loaded as part of this process.")]
		public ProcessModuleCollection Modules
		{
			get
			{
				if (this.modules == null)
				{
					SafeProcessHandle handle = null;
					try
					{
						handle = this.GetProcessHandle(1024);
						this.modules = new ProcessModuleCollection(this.GetModules_internal(handle));
					}
					finally
					{
						this.ReleaseProcessHandle(handle);
					}
				}
				return this.modules;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long GetProcessData(int pid, int data_type, out int error);

		/// <summary>Gets the amount of nonpaged system memory, in bytes, allocated for the associated process.</summary>
		/// <returns>The amount of memory, in bytes, the system has allocated for the associated process that cannot be written to the virtual memory paging file.</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support this property.</exception>
		[Obsolete("Use NonpagedSystemMemorySize64")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The number of bytes that are not pageable.")]
		[MonoTODO]
		public int NonpagedSystemMemorySize
		{
			get
			{
				return 0;
			}
		}

		/// <summary>Gets the amount of paged memory, in bytes, allocated for the associated process.</summary>
		/// <returns>The amount of memory, in bytes, allocated by the associated process that can be written to the virtual memory paging file.</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support this property.</exception>
		[Obsolete("Use PagedMemorySize64")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The number of bytes that are paged.")]
		public int PagedMemorySize
		{
			get
			{
				return (int)this.PagedMemorySize64;
			}
		}

		/// <summary>Gets the amount of pageable system memory, in bytes, allocated for the associated process.</summary>
		/// <returns>The amount of memory, in bytes, the system has allocated for the associated process that can be written to the virtual memory paging file.</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support this property.</exception>
		[Obsolete("Use PagedSystemMemorySize64")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The amount of paged system memory in bytes.")]
		public int PagedSystemMemorySize
		{
			get
			{
				return (int)this.PagedMemorySize64;
			}
		}

		/// <summary>Gets the maximum amount of memory in the virtual memory paging file, in bytes, used by the associated process.</summary>
		/// <returns>The maximum amount of memory, in bytes, allocated by the associated process that could be written to the virtual memory paging file.</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support this property.</exception>
		[MonitoringDescription("The maximum amount of paged memory used by this process.")]
		[MonoTODO]
		[Obsolete("Use PeakPagedMemorySize64")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int PeakPagedMemorySize
		{
			get
			{
				return 0;
			}
		}

		/// <summary>Gets the maximum amount of virtual memory, in bytes, used by the associated process.</summary>
		/// <returns>The maximum amount of virtual memory, in bytes, that the associated process has requested.</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support this property.</exception>
		[MonitoringDescription("The maximum amount of virtual memory used by this process.")]
		[Obsolete("Use PeakVirtualMemorySize64")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int PeakVirtualMemorySize
		{
			get
			{
				int num;
				return (int)Process.GetProcessData(this.processId, 8, out num);
			}
		}

		/// <summary>Gets the peak working set size for the associated process, in bytes.</summary>
		/// <returns>The maximum amount of physical memory that the associated process has required all at once, in bytes.</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support this property.</exception>
		[Obsolete("Use PeakWorkingSet64")]
		[MonitoringDescription("The maximum amount of system memory used by this process.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int PeakWorkingSet
		{
			get
			{
				int num;
				return (int)Process.GetProcessData(this.processId, 5, out num);
			}
		}

		/// <summary>Gets the amount of nonpaged system memory, in bytes, allocated for the associated process.</summary>
		/// <returns>The amount of system memory, in bytes, allocated for the associated process that cannot be written to the virtual memory paging file.</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support this property.</exception>
		[MonitoringDescription("The number of bytes that are not pageable.")]
		[ComVisible(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonoTODO]
		public long NonpagedSystemMemorySize64
		{
			get
			{
				return 0L;
			}
		}

		/// <summary>Gets the amount of paged memory, in bytes, allocated for the associated process.</summary>
		/// <returns>The amount of memory, in bytes, allocated in the virtual memory paging file for the associated process.</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support this property.</exception>
		[ComVisible(false)]
		[MonitoringDescription("The number of bytes that are paged.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public long PagedMemorySize64
		{
			get
			{
				int num;
				return Process.GetProcessData(this.processId, 12, out num);
			}
		}

		/// <summary>Gets the amount of pageable system memory, in bytes, allocated for the associated process.</summary>
		/// <returns>The amount of system memory, in bytes, allocated for the associated process that can be written to the virtual memory paging file.</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support this property.</exception>
		[MonitoringDescription("The amount of paged system memory in bytes.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[ComVisible(false)]
		public long PagedSystemMemorySize64
		{
			get
			{
				return this.PagedMemorySize64;
			}
		}

		/// <summary>Gets the maximum amount of memory in the virtual memory paging file, in bytes, used by the associated process.</summary>
		/// <returns>The maximum amount of memory, in bytes, allocated in the virtual memory paging file for the associated process since it was started.</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support this property.</exception>
		[ComVisible(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonoTODO]
		[MonitoringDescription("The maximum amount of paged memory used by this process.")]
		public long PeakPagedMemorySize64
		{
			get
			{
				return 0L;
			}
		}

		/// <summary>Gets the maximum amount of virtual memory, in bytes, used by the associated process.</summary>
		/// <returns>The maximum amount of virtual memory, in bytes, allocated for the associated process since it was started.</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support this property.</exception>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[ComVisible(false)]
		[MonitoringDescription("The maximum amount of virtual memory used by this process.")]
		public long PeakVirtualMemorySize64
		{
			get
			{
				int num;
				return Process.GetProcessData(this.processId, 8, out num);
			}
		}

		/// <summary>Gets the maximum amount of physical memory, in bytes, used by the associated process.</summary>
		/// <returns>The maximum amount of physical memory, in bytes, allocated for the associated process since it was started.</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support this property.</exception>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The maximum amount of system memory used by this process.")]
		[ComVisible(false)]
		public long PeakWorkingSet64
		{
			get
			{
				int num;
				return Process.GetProcessData(this.processId, 5, out num);
			}
		}

		/// <summary>Gets or sets a value indicating whether the associated process priority should temporarily be boosted by the operating system when the main window has the focus.</summary>
		/// <returns>
		///   <see langword="true" /> if dynamic boosting of the process priority should take place for a process when it is taken out of the wait state; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		/// <exception cref="T:System.ComponentModel.Win32Exception">Priority boost information could not be retrieved from the associated process resource.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support this property.  
		///  -or-  
		///  The process identifier or process handle is zero. (The process has not been started.)</exception>
		/// <exception cref="T:System.NotSupportedException">You are attempting to access the <see cref="P:System.Diagnostics.Process.PriorityBoostEnabled" /> property for a process that is running on a remote computer. This property is available only for processes that are running on the local computer.</exception>
		/// <exception cref="T:System.InvalidOperationException">The process <see cref="P:System.Diagnostics.Process.Id" /> is not available.</exception>
		[MonoTODO]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("Process will be of higher priority while it is actively used.")]
		public bool PriorityBoostEnabled
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		/// <summary>Gets the amount of private memory, in bytes, allocated for the associated process.</summary>
		/// <returns>The number of bytes allocated by the associated process that cannot be shared with other processes.</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support this property.</exception>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The amount of memory exclusively used by this process.")]
		[Obsolete("Use PrivateMemorySize64")]
		public int PrivateMemorySize
		{
			get
			{
				int num;
				return (int)Process.GetProcessData(this.processId, 6, out num);
			}
		}

		/// <summary>Gets the Terminal Services session identifier for the associated process.</summary>
		/// <returns>The Terminal Services session identifier for the associated process.</returns>
		/// <exception cref="T:System.NullReferenceException">There is no session associated with this process.</exception>
		/// <exception cref="T:System.InvalidOperationException">There is no process associated with this session identifier.  
		///  -or-  
		///  The associated process is not on this machine.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The <see cref="P:System.Diagnostics.Process.SessionId" /> property is not supported on Windows 98.</exception>
		[MonoNotSupported("")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The session ID for this process.")]
		public int SessionId
		{
			get
			{
				return 0;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string ProcessName_icall(IntPtr handle);

		private static string ProcessName_internal(SafeProcessHandle handle)
		{
			bool flag = false;
			string result;
			try
			{
				handle.DangerousAddRef(ref flag);
				result = Process.ProcessName_icall(handle.DangerousGetHandle());
			}
			finally
			{
				if (flag)
				{
					handle.DangerousRelease();
				}
			}
			return result;
		}

		/// <summary>Gets the name of the process.</summary>
		/// <returns>The name that the system uses to identify the process to the user.</returns>
		/// <exception cref="T:System.InvalidOperationException">The process does not have an identifier, or no process is associated with the <see cref="T:System.Diagnostics.Process" />.  
		///  -or-  
		///  The associated process has exited.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me); set <see cref="P:System.Diagnostics.ProcessStartInfo.UseShellExecute" /> to <see langword="false" /> to access this property on Windows 98 and Windows Me.</exception>
		/// <exception cref="T:System.NotSupportedException">The process is not on this computer.</exception>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The name of this process.")]
		public string ProcessName
		{
			get
			{
				if (this.process_name == null)
				{
					SafeProcessHandle handle = null;
					try
					{
						handle = this.GetProcessHandle(1024);
						this.process_name = Process.ProcessName_internal(handle);
						if (this.process_name == null)
						{
							throw new InvalidOperationException("Process has exited or is inaccessible, so the requested information is not available.");
						}
						if (this.process_name.EndsWith(".exe") || this.process_name.EndsWith(".bat") || this.process_name.EndsWith(".com"))
						{
							this.process_name = this.process_name.Substring(0, this.process_name.Length - 4);
						}
					}
					finally
					{
						this.ReleaseProcessHandle(handle);
					}
				}
				return this.process_name;
			}
		}

		/// <summary>Gets or sets the processors on which the threads in this process can be scheduled to run.</summary>
		/// <returns>A bitmask representing the processors that the threads in the associated process can run on. The default depends on the number of processors on the computer. The default value is 2 n -1, where n is the number of processors.</returns>
		/// <exception cref="T:System.ComponentModel.Win32Exception">
		///   <see cref="P:System.Diagnostics.Process.ProcessorAffinity" /> information could not be set or retrieved from the associated process resource.  
		/// -or-  
		/// The process identifier or process handle is zero. (The process has not been started.)</exception>
		/// <exception cref="T:System.NotSupportedException">You are attempting to access the <see cref="P:System.Diagnostics.Process.ProcessorAffinity" /> property for a process that is running on a remote computer. This property is available only for processes that are running on the local computer.</exception>
		/// <exception cref="T:System.InvalidOperationException">The process <see cref="P:System.Diagnostics.Process.Id" /> was not available.  
		///  -or-  
		///  The process has exited.</exception>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("Allowed processor that can be used by this process.")]
		[MonoTODO]
		public IntPtr ProcessorAffinity
		{
			get
			{
				return (IntPtr)0;
			}
			set
			{
			}
		}

		/// <summary>Gets a value indicating whether the user interface of the process is responding.</summary>
		/// <returns>
		///   <see langword="true" /> if the user interface of the associated process is responding to the system; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me); set <see cref="P:System.Diagnostics.ProcessStartInfo.UseShellExecute" /> to <see langword="false" /> to access this property on Windows 98 and Windows Me.</exception>
		/// <exception cref="T:System.InvalidOperationException">There is no process associated with this <see cref="T:System.Diagnostics.Process" /> object.</exception>
		/// <exception cref="T:System.NotSupportedException">You are attempting to access the <see cref="P:System.Diagnostics.Process.Responding" /> property for a process that is running on a remote computer. This property is available only for processes that are running on the local computer.</exception>
		[MonoTODO]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("Is this process responsive.")]
		public bool Responding
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets the set of threads that are running in the associated process.</summary>
		/// <returns>An array of type <see cref="T:System.Diagnostics.ProcessThread" /> representing the operating system threads currently running in the associated process.</returns>
		/// <exception cref="T:System.SystemException">The process does not have an <see cref="P:System.Diagnostics.Process.Id" />, or no process is associated with the <see cref="T:System.Diagnostics.Process" /> instance.  
		///  -or-  
		///  The associated process has exited.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me); set <see cref="P:System.Diagnostics.ProcessStartInfo.UseShellExecute" /> to <see langword="false" /> to access this property on Windows 98 and Windows Me.</exception>
		[MonoTODO]
		[MonitoringDescription("The number of threads of this process.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ProcessThreadCollection Threads
		{
			get
			{
				if (this.threads == null)
				{
					int num;
					this.threads = new ProcessThreadCollection(new ProcessThread[Process.GetProcessData(this.processId, 0, out num)]);
				}
				return this.threads;
			}
		}

		/// <summary>Gets the size of the process's virtual memory, in bytes.</summary>
		/// <returns>The amount of virtual memory, in bytes, that the associated process has requested.</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support this property.</exception>
		[MonitoringDescription("The amount of virtual memory currently used for this process.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Obsolete("Use VirtualMemorySize64")]
		public int VirtualMemorySize
		{
			get
			{
				int num;
				return (int)Process.GetProcessData(this.processId, 7, out num);
			}
		}

		/// <summary>Gets the associated process's physical memory usage, in bytes.</summary>
		/// <returns>The total amount of physical memory the associated process is using, in bytes.</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support this property.</exception>
		[Obsolete("Use WorkingSet64")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The amount of physical memory currently used for this process.")]
		public int WorkingSet
		{
			get
			{
				int num;
				return (int)Process.GetProcessData(this.processId, 4, out num);
			}
		}

		/// <summary>Gets the amount of private memory, in bytes, allocated for the associated process.</summary>
		/// <returns>The amount of memory, in bytes, allocated for the associated process that cannot be shared with other processes.</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support this property.</exception>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The amount of memory exclusively used by this process.")]
		[ComVisible(false)]
		public long PrivateMemorySize64
		{
			get
			{
				int num;
				return Process.GetProcessData(this.processId, 6, out num);
			}
		}

		/// <summary>Gets the amount of the virtual memory, in bytes, allocated for the associated process.</summary>
		/// <returns>The amount of virtual memory, in bytes, allocated for the associated process.</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support this property.</exception>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[ComVisible(false)]
		[MonitoringDescription("The amount of virtual memory currently used for this process.")]
		public long VirtualMemorySize64
		{
			get
			{
				int num;
				return Process.GetProcessData(this.processId, 7, out num);
			}
		}

		/// <summary>Gets the amount of physical memory, in bytes, allocated for the associated process.</summary>
		/// <returns>The amount of physical memory, in bytes, allocated for the associated process.</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me), which does not support this property.</exception>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[MonitoringDescription("The amount of physical memory currently used for this process.")]
		[ComVisible(false)]
		public long WorkingSet64
		{
			get
			{
				int num;
				return Process.GetProcessData(this.processId, 4, out num);
			}
		}

		/// <summary>Closes a process that has a user interface by sending a close message to its main window.</summary>
		/// <returns>
		///   <see langword="true" /> if the close message was successfully sent; <see langword="false" /> if the associated process does not have a main window or if the main window is disabled (for example if a modal dialog is being shown).</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">The platform is Windows 98 or Windows Millennium Edition (Windows Me); set the <see cref="P:System.Diagnostics.ProcessStartInfo.UseShellExecute" /> property to <see langword="false" /> to access this property on Windows 98 and Windows Me.</exception>
		/// <exception cref="T:System.InvalidOperationException">The process has already exited.  
		///  -or-  
		///  No process is associated with this <see cref="T:System.Diagnostics.Process" /> object.</exception>
		public bool CloseMainWindow()
		{
			SafeProcessHandle safeProcessHandle = null;
			bool result;
			try
			{
				safeProcessHandle = this.GetProcessHandle(1);
				result = NativeMethods.TerminateProcess(safeProcessHandle, -2);
			}
			finally
			{
				this.ReleaseProcessHandle(safeProcessHandle);
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetProcess_internal(int pid);

		/// <summary>Returns a new <see cref="T:System.Diagnostics.Process" /> component, given a process identifier and the name of a computer on the network.</summary>
		/// <param name="processId">The system-unique identifier of a process resource.</param>
		/// <param name="machineName">The name of a computer on the network.</param>
		/// <returns>A <see cref="T:System.Diagnostics.Process" /> component that is associated with a remote process resource identified by the <paramref name="processId" /> parameter.</returns>
		/// <exception cref="T:System.ArgumentException">The process specified by the <paramref name="processId" /> parameter is not running. The identifier might be expired.  
		///  -or-  
		///  The <paramref name="machineName" /> parameter syntax is invalid. The name might have length zero (0).</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="machineName" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The process was not started by this object.</exception>
		[MonoTODO("There is no support for retrieving process information from a remote machine")]
		public static Process GetProcessById(int processId, string machineName)
		{
			if (machineName == null)
			{
				throw new ArgumentNullException("machineName");
			}
			if (!Process.IsLocalMachine(machineName))
			{
				throw new NotImplementedException();
			}
			IntPtr process_internal = Process.GetProcess_internal(processId);
			if (process_internal == IntPtr.Zero)
			{
				throw new ArgumentException("Can't find process with ID " + processId.ToString());
			}
			return new Process(new SafeProcessHandle(process_internal, true), processId);
		}

		/// <summary>Creates an array of new <see cref="T:System.Diagnostics.Process" /> components and associates them with all the process resources on a remote computer that share the specified process name.</summary>
		/// <param name="processName">The friendly name of the process.</param>
		/// <param name="machineName">The name of a computer on the network.</param>
		/// <returns>An array of type <see cref="T:System.Diagnostics.Process" /> that represents the process resources running the specified application or file.</returns>
		/// <exception cref="T:System.ArgumentException">The <paramref name="machineName" /> parameter syntax is invalid. It might have length zero (0).</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="machineName" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The operating system platform does not support this operation on remote computers.</exception>
		/// <exception cref="T:System.InvalidOperationException">The attempt to connect to <paramref name="machineName" /> has failed.
		///  -or- 
		/// There are problems accessing the performance counter API's used to get process information. This exception is specific to Windows NT, Windows 2000, and Windows XP.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">A problem occurred accessing an underlying system API.</exception>
		public static Process[] GetProcessesByName(string processName, string machineName)
		{
			if (machineName == null)
			{
				throw new ArgumentNullException("machineName");
			}
			if (!Process.IsLocalMachine(machineName))
			{
				throw new NotImplementedException();
			}
			Process[] processes = Process.GetProcesses();
			if (processes.Length == 0)
			{
				return processes;
			}
			int newSize = 0;
			foreach (Process process in processes)
			{
				try
				{
					if (string.Compare(processName, process.ProcessName, true) == 0)
					{
						processes[newSize++] = process;
					}
					else
					{
						process.Dispose();
					}
				}
				catch (SystemException)
				{
				}
			}
			Array.Resize<Process>(ref processes, newSize);
			return processes;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int[] GetProcesses_internal();

		/// <summary>Creates a new <see cref="T:System.Diagnostics.Process" /> component for each process resource on the specified computer.</summary>
		/// <param name="machineName">The computer from which to read the list of processes.</param>
		/// <returns>An array of type <see cref="T:System.Diagnostics.Process" /> that represents all the process resources running on the specified computer.</returns>
		/// <exception cref="T:System.ArgumentException">The <paramref name="machineName" /> parameter syntax is invalid. It might have length zero (0).</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="machineName" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The operating system platform does not support this operation on remote computers.</exception>
		/// <exception cref="T:System.InvalidOperationException">There are problems accessing the performance counter API's used to get process information. This exception is specific to Windows NT, Windows 2000, and Windows XP.</exception>
		/// <exception cref="T:System.ComponentModel.Win32Exception">A problem occurred accessing an underlying system API.</exception>
		[MonoTODO("There is no support for retrieving process information from a remote machine")]
		public static Process[] GetProcesses(string machineName)
		{
			if (machineName == null)
			{
				throw new ArgumentNullException("machineName");
			}
			if (!Process.IsLocalMachine(machineName))
			{
				throw new NotImplementedException();
			}
			int[] processes_internal = Process.GetProcesses_internal();
			if (processes_internal == null)
			{
				return new Process[0];
			}
			List<Process> list = new List<Process>(processes_internal.Length);
			for (int i = 0; i < processes_internal.Length; i++)
			{
				try
				{
					list.Add(Process.GetProcessById(processes_internal[i]));
				}
				catch (SystemException)
				{
				}
			}
			return list.ToArray();
		}

		private static bool IsLocalMachine(string machineName)
		{
			return machineName == "." || machineName.Length == 0 || string.Compare(machineName, Environment.MachineName, true) == 0;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ShellExecuteEx_internal(ProcessStartInfo startInfo, ref Process.ProcInfo procInfo);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CreateProcess_internal(ProcessStartInfo startInfo, IntPtr stdin, IntPtr stdout, IntPtr stderr, ref Process.ProcInfo procInfo);

		private bool StartWithShellExecuteEx(ProcessStartInfo startInfo)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			if (!string.IsNullOrEmpty(startInfo.UserName) || startInfo.Password != null)
			{
				throw new InvalidOperationException(SR.GetString("The Process object must have the UseShellExecute property set to false in order to start a process as a user."));
			}
			if (startInfo.RedirectStandardInput || startInfo.RedirectStandardOutput || startInfo.RedirectStandardError)
			{
				throw new InvalidOperationException(SR.GetString("The Process object must have the UseShellExecute property set to false in order to redirect IO streams."));
			}
			if (startInfo.StandardErrorEncoding != null)
			{
				throw new InvalidOperationException(SR.GetString("StandardErrorEncoding is only supported when standard error is redirected."));
			}
			if (startInfo.StandardOutputEncoding != null)
			{
				throw new InvalidOperationException(SR.GetString("StandardOutputEncoding is only supported when standard output is redirected."));
			}
			if (startInfo.environmentVariables != null)
			{
				throw new InvalidOperationException(SR.GetString("The Process object must have the UseShellExecute property set to false in order to use environment variables."));
			}
			Process.ProcInfo procInfo = default(Process.ProcInfo);
			Process.FillUserInfo(startInfo, ref procInfo);
			bool flag;
			try
			{
				flag = Process.ShellExecuteEx_internal(startInfo, ref procInfo);
			}
			finally
			{
				if (procInfo.Password != IntPtr.Zero)
				{
					Marshal.ZeroFreeBSTR(procInfo.Password);
				}
				procInfo.Password = IntPtr.Zero;
			}
			if (!flag)
			{
				throw new Win32Exception(-procInfo.pid);
			}
			this.SetProcessHandle(new SafeProcessHandle(procInfo.process_handle, true));
			this.SetProcessId(procInfo.pid);
			return flag;
		}

		private static void CreatePipe(out IntPtr read, out IntPtr write, bool writeDirection)
		{
			MonoIOError monoIOError;
			if (!MonoIO.CreatePipe(out read, out write, out monoIOError))
			{
				throw MonoIO.GetException(monoIOError);
			}
			if (Process.IsWindows)
			{
				IntPtr intPtr = writeDirection ? write : read;
				if (!MonoIO.DuplicateHandle(Process.GetCurrentProcess().Handle, intPtr, Process.GetCurrentProcess().Handle, out intPtr, 0, 0, 2, out monoIOError))
				{
					throw MonoIO.GetException(monoIOError);
				}
				if (writeDirection)
				{
					if (!MonoIO.Close(write, out monoIOError))
					{
						throw MonoIO.GetException(monoIOError);
					}
					write = intPtr;
					return;
				}
				else
				{
					if (!MonoIO.Close(read, out monoIOError))
					{
						throw MonoIO.GetException(monoIOError);
					}
					read = intPtr;
				}
			}
		}

		private static bool IsWindows
		{
			get
			{
				PlatformID platform = Environment.OSVersion.Platform;
				return platform == PlatformID.Win32S || platform == PlatformID.Win32Windows || platform == PlatformID.Win32NT || platform == PlatformID.WinCE;
			}
		}

		private bool StartWithCreateProcess(ProcessStartInfo startInfo)
		{
			if (startInfo.StandardOutputEncoding != null && !startInfo.RedirectStandardOutput)
			{
				throw new InvalidOperationException(SR.GetString("StandardOutputEncoding is only supported when standard output is redirected."));
			}
			if (startInfo.StandardErrorEncoding != null && !startInfo.RedirectStandardError)
			{
				throw new InvalidOperationException(SR.GetString("StandardErrorEncoding is only supported when standard error is redirected."));
			}
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().Name);
			}
			Process.ProcInfo procInfo = default(Process.ProcInfo);
			if (startInfo.HaveEnvVars)
			{
				List<string> list = new List<string>();
				foreach (object obj in startInfo.EnvironmentVariables)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					if (dictionaryEntry.Value != null)
					{
						list.Add((string)dictionaryEntry.Key + "=" + (string)dictionaryEntry.Value);
					}
				}
				procInfo.envVariables = list.ToArray();
			}
			if (startInfo.ArgumentList.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string argument in startInfo.ArgumentList)
				{
					PasteArguments.AppendArgument(stringBuilder, argument);
				}
				startInfo.Arguments = stringBuilder.ToString();
			}
			IntPtr intPtr = IntPtr.Zero;
			IntPtr zero = IntPtr.Zero;
			IntPtr zero2 = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			IntPtr zero3 = IntPtr.Zero;
			IntPtr intPtr3 = IntPtr.Zero;
			try
			{
				if (startInfo.RedirectStandardInput)
				{
					Process.CreatePipe(out intPtr, out zero, true);
				}
				else
				{
					intPtr = MonoIO.ConsoleInput;
					zero = IntPtr.Zero;
				}
				if (startInfo.RedirectStandardOutput)
				{
					Process.CreatePipe(out zero2, out intPtr2, false);
				}
				else
				{
					zero2 = IntPtr.Zero;
					intPtr2 = MonoIO.ConsoleOutput;
				}
				if (startInfo.RedirectStandardError)
				{
					Process.CreatePipe(out zero3, out intPtr3, false);
				}
				else
				{
					zero3 = IntPtr.Zero;
					intPtr3 = MonoIO.ConsoleError;
				}
				Process.FillUserInfo(startInfo, ref procInfo);
				if (!Process.CreateProcess_internal(startInfo, intPtr, intPtr2, intPtr3, ref procInfo))
				{
					throw new Win32Exception(-procInfo.pid, string.Concat(new string[]
					{
						"ApplicationName='",
						startInfo.FileName,
						"', CommandLine='",
						startInfo.Arguments,
						"', CurrentDirectory='",
						startInfo.WorkingDirectory,
						"', Native error= ",
						Win32Exception.GetErrorMessage(-procInfo.pid)
					}));
				}
			}
			catch
			{
				if (startInfo.RedirectStandardInput)
				{
					if (intPtr != IntPtr.Zero)
					{
						MonoIOError monoIOError;
						MonoIO.Close(intPtr, out monoIOError);
					}
					if (zero != IntPtr.Zero)
					{
						MonoIOError monoIOError;
						MonoIO.Close(zero, out monoIOError);
					}
				}
				if (startInfo.RedirectStandardOutput)
				{
					if (zero2 != IntPtr.Zero)
					{
						MonoIOError monoIOError;
						MonoIO.Close(zero2, out monoIOError);
					}
					if (intPtr2 != IntPtr.Zero)
					{
						MonoIOError monoIOError;
						MonoIO.Close(intPtr2, out monoIOError);
					}
				}
				if (startInfo.RedirectStandardError)
				{
					if (zero3 != IntPtr.Zero)
					{
						MonoIOError monoIOError;
						MonoIO.Close(zero3, out monoIOError);
					}
					if (intPtr3 != IntPtr.Zero)
					{
						MonoIOError monoIOError;
						MonoIO.Close(intPtr3, out monoIOError);
					}
				}
				throw;
			}
			finally
			{
				if (procInfo.Password != IntPtr.Zero)
				{
					Marshal.ZeroFreeBSTR(procInfo.Password);
					procInfo.Password = IntPtr.Zero;
				}
			}
			this.SetProcessHandle(new SafeProcessHandle(procInfo.process_handle, true));
			this.SetProcessId(procInfo.pid);
			if (startInfo.RedirectStandardInput)
			{
				MonoIOError monoIOError;
				MonoIO.Close(intPtr, out monoIOError);
				Encoding encoding = startInfo.StandardInputEncoding ?? Console.InputEncoding;
				this.standardInput = new StreamWriter(new FileStream(zero, FileAccess.Write, true, 8192), encoding)
				{
					AutoFlush = true
				};
			}
			if (startInfo.RedirectStandardOutput)
			{
				MonoIOError monoIOError;
				MonoIO.Close(intPtr2, out monoIOError);
				Encoding encoding2 = startInfo.StandardOutputEncoding ?? Console.OutputEncoding;
				this.standardOutput = new StreamReader(new FileStream(zero2, FileAccess.Read, true, 8192), encoding2, true);
			}
			if (startInfo.RedirectStandardError)
			{
				MonoIOError monoIOError;
				MonoIO.Close(intPtr3, out monoIOError);
				Encoding encoding3 = startInfo.StandardErrorEncoding ?? Console.OutputEncoding;
				this.standardError = new StreamReader(new FileStream(zero3, FileAccess.Read, true, 8192), encoding3, true);
			}
			return true;
		}

		private static void FillUserInfo(ProcessStartInfo startInfo, ref Process.ProcInfo procInfo)
		{
			if (startInfo.UserName.Length != 0)
			{
				procInfo.UserName = startInfo.UserName;
				procInfo.Domain = startInfo.Domain;
				if (startInfo.Password != null)
				{
					procInfo.Password = Marshal.SecureStringToBSTR(startInfo.Password);
				}
				else
				{
					procInfo.Password = IntPtr.Zero;
				}
				procInfo.LoadUserProfile = startInfo.LoadUserProfile;
			}
		}

		private void RaiseOnExited()
		{
			if (!this.watchForExit)
			{
				return;
			}
			if (!this.raisedOnExited)
			{
				lock (this)
				{
					if (!this.raisedOnExited)
					{
						this.raisedOnExited = true;
						this.OnExited();
					}
				}
			}
		}

		private bool haveProcessId;

		private int processId;

		private bool haveProcessHandle;

		private SafeProcessHandle m_processHandle;

		private bool isRemoteMachine;

		private string machineName;

		private int m_processAccess;

		private ProcessThreadCollection threads;

		private ProcessModuleCollection modules;

		private bool haveWorkingSetLimits;

		private IntPtr minWorkingSet;

		private IntPtr maxWorkingSet;

		private bool havePriorityClass;

		private ProcessPriorityClass priorityClass;

		private ProcessStartInfo startInfo;

		private bool watchForExit;

		private bool watchingForExit;

		private EventHandler onExited;

		private bool exited;

		private int exitCode;

		private bool signaled;

		private DateTime exitTime;

		private bool haveExitTime;

		private bool raisedOnExited;

		private RegisteredWaitHandle registeredWaitHandle;

		private WaitHandle waitHandle;

		private ISynchronizeInvoke synchronizingObject;

		private StreamReader standardOutput;

		private StreamWriter standardInput;

		private StreamReader standardError;

		private OperatingSystem operatingSystem;

		private bool disposed;

		private Process.StreamReadMode outputStreamReadMode;

		private Process.StreamReadMode errorStreamReadMode;

		private Process.StreamReadMode inputStreamReadMode;

		internal AsyncStreamReader output;

		internal AsyncStreamReader error;

		internal bool pendingOutputRead;

		internal bool pendingErrorRead;

		internal static TraceSwitch processTracing;

		private string process_name;

		private static ProcessModule current_main_module;

		private enum StreamReadMode
		{
			undefined,
			syncMode,
			asyncMode
		}

		private enum State
		{
			HaveId = 1,
			IsLocal,
			IsNt = 4,
			HaveProcessInfo = 8,
			Exited = 16,
			Associated = 32,
			IsWin2k = 64,
			HaveNtProcessInfo = 12
		}

		private struct ProcInfo
		{
			public IntPtr process_handle;

			public int pid;

			public string[] envVariables;

			public string UserName;

			public string Domain;

			public IntPtr Password;

			public bool LoadUserProfile;
		}
	}
}
